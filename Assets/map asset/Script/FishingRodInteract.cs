using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRodInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float promptDistance = 2.35f;
    [SerializeField] private float fishingDuration = 0.62f;
    [SerializeField, Range(0f, 1f)] private float pencilFragmentChance = PencilFragmentChanceAfterNpc;

    private const float LineWidth = 0.045f;
    private const int FishSortingOrder = 69;
    private const int PencilSortingOrder = 68;
    private const int BottleSortingOrder = 69;
    private const int LetterSortingOrder = 72;
    private const int CrayonSortingOrder = 73;
    private const float PencilFragmentChanceAfterNpc = 0.25f;
    private const float LetterBottleChanceAfterFishingPencil = 0.25f;

    private static FishingRodInteract activeRod;
    private static Sprite pixelSprite;

    private Transform player;
    private GameObject promptObject;
    private CircleCollider2D interactionCollider;
    private Quaternion baseLocalRotation;
    private Vector3 pondLocalPosition;
    private SketchWorldLineDrawing requiredPondDrawing;
    private int promptSortingOrder = 70;
    private bool isFishing;
    private bool interactionLocked;
    private int reservedInsectCount;
    private readonly List<Transform> dockedInsects = new List<Transform>();

    public void Configure(Vector3 localPondPosition, int sortingOrder, SketchWorldLineDrawing pondDrawing = null)
    {
        activeRod = this;
        pondLocalPosition = localPondPosition;
        promptSortingOrder = sortingOrder;
        requiredPondDrawing = pondDrawing;
        EnsurePrompt();
        EnsureCollider();
        UpdatePromptPosition();
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || !CanInteract() || isFishing || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(GetInteractionPosition(), interactor.transform.position) > promptDistance)
        {
            return;
        }

        if (GameProgress.CanRevealGreenCrayon)
        {
            StartCoroutine(GreenCrayonRewardRoutine());
            return;
        }

        StartCoroutine(FishingRoutine());
    }

    private void Awake()
    {
        activeRod = this;
        baseLocalRotation = transform.localRotation;
        EnsurePrompt();
        EnsureCollider();
    }

    private void OnEnable()
    {
        activeRod = this;
        FindPlayer();
        baseLocalRotation = transform.localRotation;
        SetInteractionAvailable(CanInteract());
    }

    private void Update()
    {
        if (!CanInteract())
        {
            SetInteractionAvailable(false);
            return;
        }

        SetInteractionAvailable(true);

        if (player == null)
        {
            FindPlayer();
        }

        if (promptObject == null)
        {
            return;
        }

        bool shouldShowPrompt = !isFishing
            && player != null
            && Vector2.Distance(GetInteractionPosition(), player.position) <= promptDistance;
        promptObject.SetActive(shouldShowPrompt);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isFishing = false;
        interactionLocked = false;
        transform.localRotation = baseLocalRotation;
        SetInteractionAvailable(false);
    }

    public static bool CanAcceptGlowingInsect()
    {
        return activeRod != null && activeRod.CanReserveInsect();
    }

    public static bool TryReserveInsectDockPosition(out Vector3 worldPosition, out Transform parent)
    {
        worldPosition = Vector3.zero;
        parent = null;

        if (activeRod == null)
        {
            return false;
        }

        return activeRod.TryReserveInsectDockPositionInternal(out worldPosition, out parent);
    }

    public static void CompleteInsectDock(Transform insectTransform)
    {
        if (activeRod == null)
        {
            if (insectTransform != null)
            {
                Destroy(insectTransform.gameObject);
            }

            return;
        }

        activeRod.CompleteInsectDockInternal(insectTransform);
    }

    private bool TryReserveInsectDockPositionInternal(out Vector3 worldPosition, out Transform parent)
    {
        worldPosition = Vector3.zero;
        parent = transform.parent != null ? transform.parent : transform;

        if (!CanReserveInsect())
        {
            return false;
        }

        int slotIndex = Mathf.Min(
            GameProgress.FishingRodInsectCount + reservedInsectCount,
            GameProgress.RequiredFishingRodInsectCount - 1);
        reservedInsectCount++;
        worldPosition = GetInsectDockWorldPosition(slotIndex);
        return true;
    }

    private void CompleteInsectDockInternal(Transform insectTransform)
    {
        reservedInsectCount = Mathf.Max(0, reservedInsectCount - 1);

        if (insectTransform == null)
        {
            return;
        }

        if (!GameProgress.AddFishingRodInsect())
        {
            Destroy(insectTransform.gameObject);
            return;
        }

        int slotIndex = Mathf.Clamp(GameProgress.FishingRodInsectCount - 1, 0, GameProgress.RequiredFishingRodInsectCount - 1);
        Vector3 dockPosition = GetInsectDockWorldPosition(slotIndex);
        insectTransform.SetParent(transform, true);
        insectTransform.position = dockPosition;
        insectTransform.localScale = Vector3.one * 0.82f;

        GlowingInsectInteract insectInteract = insectTransform.GetComponent<GlowingInsectInteract>();
        if (insectInteract != null)
        {
            insectInteract.MarkDockedAtFishingRod(dockPosition);
        }

        dockedInsects.Add(insectTransform);
    }

    private bool CanReserveInsect()
    {
        return GameProgress.CanAddFishingRodInsect
            && GameProgress.FishingRodInsectCount + reservedInsectCount < GameProgress.RequiredFishingRodInsectCount;
    }

    private IEnumerator FishingRoutine()
    {
        isFishing = true;

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }

        float elapsed = 0f;

        while (elapsed < fishingDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fishingDuration);
            float tug = Mathf.Sin(progress * Mathf.PI * 8f) * (1f - progress) * 4.5f;

            transform.localRotation = baseLocalRotation * Quaternion.Euler(0f, 0f, tug);
            yield return null;
        }

        transform.localRotation = baseLocalRotation;

        FishingOutcome outcome = PickFishingOutcome();

        if (outcome == FishingOutcome.PencilFragment)
        {
            GameProgress.DropPencilFragmentFrom(PencilFragmentSource.Fishing);
            interactionLocked = true;
            SetInteractionAvailable(false);
            yield return StartCoroutine(SpawnPencilFragmentJumpRoutine());
            GameProgress.CollectPencilFragmentFrom(PencilFragmentSource.Fishing);
            PencilFragmentHud.ShowCollected();
            interactionLocked = false;
            SetInteractionAvailable(true);
            isFishing = false;
            yield break;
        }

        if (outcome == FishingOutcome.LetterBottle)
        {
            interactionLocked = true;
            SetInteractionAvailable(false);
            yield return StartCoroutine(SpawnLetterBottleRoutine());
            interactionLocked = false;
            SetInteractionAvailable(true);
            isFishing = false;
            yield break;
        }

        SpawnFishJump();
        isFishing = false;
    }

    private IEnumerator GreenCrayonRewardRoutine()
    {
        isFishing = true;
        interactionLocked = true;

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }

        SetInteractionAvailable(false);
        GameProgress.RevealGreenCrayon();

        yield return StartCoroutine(ScatterDockedInsectsRoutine());
        yield return StartCoroutine(SpawnGreenCrayonRewardRoutine());
        GameProgress.CollectGreenCrayon();
        PencilFragmentHud.ShowCollected();

        interactionLocked = false;
        isFishing = false;
        SetInteractionAvailable(true);
    }

    private IEnumerator ScatterDockedInsectsRoutine()
    {
        List<Transform> insects = new List<Transform>();

        for (int i = 0; i < dockedInsects.Count; i++)
        {
            if (dockedInsects[i] != null)
            {
                insects.Add(dockedInsects[i]);
            }
        }

        dockedInsects.Clear();

        if (insects.Count <= 0)
        {
            yield break;
        }

        Vector3[] startPositions = new Vector3[insects.Count];
        Vector3[] endPositions = new Vector3[insects.Count];

        for (int i = 0; i < insects.Count; i++)
        {
            startPositions[i] = insects[i].position;
            float angle = (Mathf.PI * 2f * i / Mathf.Max(1, insects.Count)) + Random.Range(-0.28f, 0.28f);
            Vector3 scatterDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            endPositions[i] = startPositions[i] + scatterDirection * Random.Range(1.1f, 2.0f) + Vector3.up * Random.Range(0.25f, 0.9f);
        }

        const float scatterDuration = 0.68f;
        float elapsed = 0f;

        while (elapsed < scatterDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / scatterDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            for (int i = 0; i < insects.Count; i++)
            {
                Transform insect = insects[i];
                if (insect == null)
                {
                    continue;
                }

                Vector3 position = Vector3.Lerp(startPositions[i], endPositions[i], easedProgress);
                position.y += Mathf.Sin(progress * Mathf.PI) * 0.34f;
                insect.position = position;
                insect.localScale = Vector3.one * Mathf.Lerp(0.82f, 0.16f, easedProgress);
            }

            yield return null;
        }

        for (int i = 0; i < insects.Count; i++)
        {
            if (insects[i] != null)
            {
                Destroy(insects[i].gameObject);
            }
        }
    }

    private IEnumerator SpawnGreenCrayonRewardRoutine()
    {
        GameObject crayonObject = new GameObject("GeneratedGreenCrayonReward");
        Transform parent = transform.parent;
        if (parent != null)
        {
            crayonObject.transform.SetParent(parent, true);
        }

        Vector3 startPosition = GetInteractionPosition() + new Vector3(0f, 0.2f, 0f);
        Vector3 endPosition = GetInteractionPosition() + new Vector3(0.15f, 0.76f, 0f);
        crayonObject.transform.position = startPosition;
        crayonObject.transform.localScale = Vector3.one * 0.12f;
        CreateGreenCrayonVisual(crayonObject.transform, CrayonSortingOrder);

        const float revealDuration = 0.42f;
        float elapsed = 0f;

        while (elapsed < revealDuration && crayonObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / revealDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 position = Vector3.Lerp(startPosition, endPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.22f;

            crayonObject.transform.position = position;
            crayonObject.transform.localScale = Vector3.one * Mathf.Lerp(0.12f, 1f, easedProgress);
            crayonObject.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-18f, 10f, progress));
            yield return null;
        }

        if (crayonObject == null)
        {
            yield break;
        }

        crayonObject.transform.position = endPosition;
        crayonObject.transform.localScale = Vector3.one;
        crayonObject.transform.localRotation = Quaternion.Euler(0f, 0f, 10f);
        yield return new WaitForSeconds(0.08f);

        if (crayonObject != null)
        {
            Destroy(crayonObject);
        }
    }

    private FishingOutcome PickFishingOutcome()
    {
        if (!GameProgress.HasMetQuestNpc)
        {
            return FishingOutcome.Fish;
        }

        float roll = Random.value;
        float pencilChance = Mathf.Clamp01(pencilFragmentChance);

        if (GameProgress.CanDropPencilFragmentFrom(PencilFragmentSource.Fishing))
        {
            return roll < pencilChance
                ? FishingOutcome.PencilFragment
                : FishingOutcome.Fish;
        }

        if (GameProgress.HasCollectedPencilFragmentFrom(PencilFragmentSource.Fishing)
            && GameProgress.CanCatchLetterBottle
            && roll < LetterBottleChanceAfterFishingPencil)
        {
            return FishingOutcome.LetterBottle;
        }

        return FishingOutcome.Fish;
    }

    private void SpawnFishJump()
    {
        Vector3 pondPosition = GetPondWorldPosition();
        Vector3 startPosition = pondPosition + new Vector3(Random.Range(-1.7f, 1.6f), Random.Range(-0.48f, 0.42f), 0f);
        Vector3 endPosition = startPosition + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.12f, 0.12f), 0f);

        GameObject fishObject = new GameObject("GeneratedJumpingFish");
        Transform parent = transform.parent;
        if (parent != null)
        {
            fishObject.transform.SetParent(parent, true);
        }

        fishObject.transform.position = startPosition;

        SketchWorldLineDrawing drawing = fishObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(LineWidth, new Color32(53, 91, 114, 255), FishSortingOrder);
        drawing.SetStrokes(BuildFishStrokes(Random.Range(0, 3)));
        drawing.RevealProgress = 1f;

        StartCoroutine(AnimateFishJumpRoutine(fishObject.transform, startPosition, endPosition));
    }

    private IEnumerator AnimateFishJumpRoutine(Transform fishTransform, Vector3 startPosition, Vector3 endPosition)
    {
        const float jumpDuration = 0.82f;
        const float diveDuration = 0.18f;
        float elapsed = 0f;

        while (elapsed < jumpDuration && fishTransform != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / jumpDuration);
            Vector3 position = Vector3.Lerp(startPosition, endPosition, progress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 1.2f;

            float rotation = progress < 0.52f
                ? Mathf.Lerp(-28f, 10f, progress / 0.52f)
                : Mathf.Lerp(10f, -62f, (progress - 0.52f) / 0.48f);

            fishTransform.position = position;
            fishTransform.localRotation = Quaternion.Euler(0f, 0f, rotation);
            yield return null;
        }

        SpawnWaterSplash(endPosition);
        elapsed = 0f;
        Vector3 diveEndPosition = endPosition + new Vector3(0f, -0.22f, 0f);

        while (elapsed < diveDuration && fishTransform != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / diveDuration);
            fishTransform.position = Vector3.Lerp(endPosition, diveEndPosition, progress);
            fishTransform.localScale = Vector3.one * Mathf.Lerp(0.82f, 0.18f, progress);
            yield return null;
        }

        if (fishTransform != null)
        {
            Destroy(fishTransform.gameObject);
        }
    }

    private void SpawnWaterSplash(Vector3 position)
    {
        GameObject splashObject = new GameObject("GeneratedFishWaterSplash");
        Transform parent = transform.parent;
        if (parent != null)
        {
            splashObject.transform.SetParent(parent, true);
        }

        splashObject.transform.position = position;

        SketchWorldLineDrawing drawing = splashObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(0.035f, new Color32(57, 104, 132, 210), FishSortingOrder + 1);
        drawing.SetStrokes(BuildSplashStrokes());
        drawing.RevealProgress = 0f;

        StartCoroutine(AnimateWaterSplashRoutine(splashObject, drawing));
    }

    private IEnumerator AnimateWaterSplashRoutine(GameObject splashObject, SketchWorldLineDrawing drawing)
    {
        const float splashDuration = 0.46f;
        float elapsed = 0f;

        while (elapsed < splashDuration && splashObject != null && drawing != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / splashDuration);
            float fadeProgress = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.48f, 1f, progress));
            byte alpha = (byte)Mathf.RoundToInt(Mathf.Lerp(210f, 0f, fadeProgress));

            splashObject.transform.localScale = Vector3.one * Mathf.Lerp(0.72f, 1.38f, progress);
            drawing.Configure(0.035f, new Color32(57, 104, 132, alpha), FishSortingOrder + 1);
            drawing.RevealProgress = Mathf.Clamp01(progress * 2.2f);
            yield return null;
        }

        if (splashObject != null)
        {
            Destroy(splashObject);
        }
    }

    private IEnumerator SpawnPencilFragmentJumpRoutine()
    {
        Vector3 pondPosition = GetPondWorldPosition();
        Vector3 startPosition = pondPosition + new Vector3(Random.Range(-1.35f, 1.35f), Random.Range(-0.38f, 0.38f), 0f);
        Vector3 endPosition = startPosition + new Vector3(Random.Range(-0.42f, 0.42f), Random.Range(-0.08f, 0.14f), 0f);

        GameObject pencilObject = new GameObject("GeneratedJumpingPencilFragment");
        Transform parent = transform.parent;
        if (parent != null)
        {
            pencilObject.transform.SetParent(parent, true);
        }

        pencilObject.transform.position = startPosition;
        pencilObject.transform.localScale = Vector3.one;
        CreatePencilFragmentVisual(pencilObject.transform);

        const float jumpDuration = 0.92f;
        float elapsed = 0f;

        while (elapsed < jumpDuration && pencilObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / jumpDuration);
            Vector3 position = Vector3.Lerp(startPosition, endPosition, progress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 1.15f;

            pencilObject.transform.position = position;
            pencilObject.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-24f, 28f, progress));
            yield return null;
        }

        if (pencilObject != null)
        {
            Destroy(pencilObject);
        }
    }

    private IEnumerator SpawnLetterBottleRoutine()
    {
        Vector3 pondPosition = GetPondWorldPosition();
        Vector3 startPosition = pondPosition + new Vector3(Random.Range(-1.2f, 1.2f), Random.Range(-0.3f, 0.35f), 0f);
        Vector3 endPosition = startPosition + new Vector3(Random.Range(-0.3f, 0.45f), Random.Range(0.05f, 0.22f), 0f);

        GameObject bottleObject = new GameObject("GeneratedLetterBottle");
        Transform parent = transform.parent;
        if (parent != null)
        {
            bottleObject.transform.SetParent(parent, true);
        }

        bottleObject.transform.position = startPosition;
        CreateBottleVisual(bottleObject.transform);

        yield return StartCoroutine(AnimateBottleJumpRoutine(bottleObject.transform, startPosition, endPosition));
        yield return new WaitForSeconds(0.16f);

        GameObject letterObject = new GameObject("GeneratedLetterPulledFromBottle");
        if (parent != null)
        {
            letterObject.transform.SetParent(parent, true);
        }

        Vector3 letterStartPosition = endPosition + new Vector3(0.08f, 0.1f, 0f);
        Vector3 letterEndPosition = endPosition + new Vector3(0.0f, 0.78f, 0f);
        letterObject.transform.position = letterStartPosition;
        letterObject.transform.localScale = Vector3.one * 0.82f;
        CreateLetterPageVisual(letterObject.transform, LetterSortingOrder);

        const float pullDuration = 0.58f;
        float elapsed = 0f;
        while (elapsed < pullDuration && letterObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / pullDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            letterObject.transform.position = Vector3.Lerp(letterStartPosition, letterEndPosition, easedProgress);
            letterObject.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(progress * Mathf.PI * 6f) * 3.5f);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        Vector3 tearPosition = letterObject != null ? letterObject.transform.position : letterEndPosition;
        if (letterObject != null)
        {
            Destroy(letterObject);
        }

        GameObject collectedPiece = new GameObject("GeneratedBottleLetterFragment");
        if (parent != null)
        {
            collectedPiece.transform.SetParent(parent, true);
        }

        collectedPiece.transform.position = tearPosition + new Vector3(-0.16f, 0f, 0f);
        collectedPiece.transform.localScale = Vector3.one * 0.88f;
        CreateLetterFragmentVisual(collectedPiece.transform, LetterSortingOrder + 1);

        GameObject remainingLetter = new GameObject("GeneratedBottleRemainingLetter");
        if (parent != null)
        {
            remainingLetter.transform.SetParent(parent, true);
        }

        remainingLetter.transform.position = tearPosition + new Vector3(0.18f, 0.02f, 0f);
        remainingLetter.transform.localScale = Vector3.one * 0.78f;
        CreateTornLetterVisual(remainingLetter.transform, LetterSortingOrder);

        yield return StartCoroutine(AnimateLetterFragmentCollectRoutine(collectedPiece.transform));

        GameProgress.CollectLetterFragmentFrom(LetterFragmentSource.Bottle);
        LetterQuestHud.ShowProgress();
        GameProgress.StartLetterTreeChase();
        LetterQuestTreeRoute.BeginRouteFrom(remainingLetter.transform.position, parent);

        if (remainingLetter != null)
        {
            Destroy(remainingLetter);
        }

        if (bottleObject != null)
        {
            Destroy(bottleObject);
        }
    }

    private IEnumerator AnimateBottleJumpRoutine(Transform bottleTransform, Vector3 startPosition, Vector3 endPosition)
    {
        const float jumpDuration = 0.86f;
        float elapsed = 0f;

        while (elapsed < jumpDuration && bottleTransform != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / jumpDuration);
            Vector3 position = Vector3.Lerp(startPosition, endPosition, progress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 1.05f;

            bottleTransform.position = position;
            bottleTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-62f, 32f, progress));
            yield return null;
        }

        SpawnWaterSplash(endPosition);

        if (bottleTransform != null)
        {
            bottleTransform.position = endPosition;
            bottleTransform.localRotation = Quaternion.Euler(0f, 0f, 24f);
        }
    }

    private IEnumerator AnimateLetterFragmentCollectRoutine(Transform fragmentTransform)
    {
        if (fragmentTransform == null)
        {
            yield break;
        }

        yield return new WaitForSeconds(0.14f);

        Vector3 startPosition = fragmentTransform.position;
        Vector3 targetPosition = player != null
            ? player.position + new Vector3(0f, 0.72f, 0f)
            : startPosition + new Vector3(0f, 0.82f, 0f);
        Vector3 startScale = fragmentTransform.localScale;
        const float collectDuration = 0.36f;
        float elapsed = 0f;

        while (elapsed < collectDuration && fragmentTransform != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.18f;

            fragmentTransform.position = position;
            fragmentTransform.localScale = Vector3.Lerp(startScale, startScale * 0.22f, easedProgress);
            fragmentTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-10f, 8f, progress));
            yield return null;
        }

        if (fragmentTransform != null)
        {
            Destroy(fragmentTransform.gameObject);
        }
    }

    private void EnsurePrompt()
    {
        if (promptObject == null)
        {
            promptObject = new GameObject("GeneratedFishingRodPrompt");
            promptObject.transform.SetParent(transform, false);

            TextMesh textMesh = promptObject.AddComponent<TextMesh>();
            textMesh.text = "E";
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.fontSize = 64;
            textMesh.characterSize = 0.075f;
            textMesh.color = new Color32(35, 32, 28, 255);
        }

        MeshRenderer meshRenderer = promptObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = promptSortingOrder;
        }

        UpdatePromptPosition();
        promptObject.SetActive(false);
    }

    private void EnsureCollider()
    {
        if (interactionCollider != null)
        {
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = 1.45f;
        interactionCollider.offset = new Vector2(0.55f, -0.45f);
    }

    private void UpdatePromptPosition()
    {
        if (promptObject != null)
        {
            promptObject.transform.localPosition = new Vector3(0f, 2.35f, 0f);
        }
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null && interactionCollider.enabled != available)
        {
            interactionCollider.enabled = available;
        }

        if (!available && promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private bool CanInteract()
    {
        return !interactionLocked && IsPondReady();
    }

    private bool IsPondReady()
    {
        return requiredPondDrawing == null || requiredPondDrawing.RevealProgress >= 0.95f;
    }

    private Vector3 GetInteractionPosition()
    {
        Vector3 localPosition = interactionCollider != null
            ? new Vector3(interactionCollider.offset.x, interactionCollider.offset.y, 0f)
            : new Vector3(0.55f, -0.45f, 0f);

        return transform.TransformPoint(localPosition);
    }

    private Vector3 GetPondWorldPosition()
    {
        Transform parent = transform.parent;
        return parent != null ? parent.TransformPoint(pondLocalPosition) : pondLocalPosition;
    }

    private Vector3 GetInsectDockWorldPosition(int slotIndex)
    {
        int clampedIndex = Mathf.Clamp(slotIndex, 0, GameProgress.RequiredFishingRodInsectCount - 1);
        int column = clampedIndex % 5;
        int row = clampedIndex / 5;
        Vector3 localPosition = new Vector3(-0.08f + column * 0.24f, -0.92f + row * 0.18f, 0f);
        return transform.TransformPoint(localPosition);
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }

    private void CreatePencilFragmentVisual(Transform pencilTransform)
    {
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentBody", new Vector3(-0.06f, 0f, 0f), new Vector3(0.48f, 0.12f, 1f), new Color32(236, 181, 51, 255), PencilSortingOrder);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentWood", new Vector3(0.24f, 0f, 0f), new Vector3(0.16f, 0.12f, 1f), new Color32(203, 143, 77, 255), PencilSortingOrder + 1);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentLead", new Vector3(0.34f, 0f, 0f), new Vector3(0.08f, 0.08f, 1f), new Color32(44, 40, 36, 255), PencilSortingOrder + 2);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentPaintEdge", new Vector3(-0.33f, 0f, 0f), new Vector3(0.08f, 0.13f, 1f), new Color32(190, 66, 72, 255), PencilSortingOrder + 2);

        SketchWorldLineDrawing outline = pencilTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        outline.Configure(0.035f, new Color32(38, 34, 28, 255), PencilSortingOrder + 3);
        outline.SetStrokes(new[]
        {
            Points(-0.38f, -0.08f, 0.2f, -0.08f, 0.4f, 0f, 0.2f, 0.08f, -0.38f, 0.08f, -0.38f, -0.08f),
            Points(0.2f, -0.08f, 0.2f, 0.08f),
            Points(-0.28f, -0.08f, -0.28f, 0.08f)
        });
        outline.RevealProgress = 1f;
    }

    private static void CreateGreenCrayonVisual(Transform crayonTransform, int sortingOrder)
    {
        CreatePencilPart(crayonTransform, "GeneratedGreenCrayonBody", new Vector3(-0.06f, 0f, 0f), new Vector3(0.62f, 0.14f, 1f), new Color32(72, 174, 80, 255), sortingOrder);
        CreatePencilPart(crayonTransform, "GeneratedGreenCrayonTip", new Vector3(0.3f, 0f, 0f), new Vector3(0.14f, 0.1f, 1f), new Color32(36, 116, 55, 255), sortingOrder + 1);
        CreatePencilPart(crayonTransform, "GeneratedGreenCrayonWrapper", new Vector3(-0.18f, 0f, 0f), new Vector3(0.16f, 0.16f, 1f), new Color32(205, 236, 158, 255), sortingOrder + 2);
        CreatePencilPart(crayonTransform, "GeneratedGreenCrayonBackEdge", new Vector3(-0.42f, 0f, 0f), new Vector3(0.08f, 0.15f, 1f), new Color32(44, 132, 64, 255), sortingOrder + 2);

        SketchWorldLineDrawing outline = crayonTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        outline.Configure(0.035f, new Color32(30, 58, 34, 255), sortingOrder + 3);
        outline.SetStrokes(new[]
        {
            Points(-0.48f, -0.08f, 0.22f, -0.08f, 0.44f, 0f, 0.22f, 0.08f, -0.48f, 0.08f, -0.48f, -0.08f),
            Points(0.22f, -0.08f, 0.22f, 0.08f),
            Points(-0.26f, -0.08f, -0.26f, 0.08f),
            Points(-0.1f, -0.08f, -0.1f, 0.08f)
        });
        outline.RevealProgress = 1f;
    }

    private void CreateBottleVisual(Transform bottleTransform)
    {
        SketchWorldLineDrawing drawing = bottleTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(0.04f, new Color32(37, 92, 94, 230), BottleSortingOrder);
        drawing.SetStrokes(BuildBottleStrokes());
        drawing.RevealProgress = 1f;
    }

    private void CreateLetterPageVisual(Transform letterTransform, int sortingOrder)
    {
        SketchWorldLineDrawing drawing = letterTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(0.035f, new Color32(78, 57, 34, 255), sortingOrder);
        drawing.SetStrokes(BuildLetterPageStrokes());
        drawing.RevealProgress = 1f;
    }

    private void CreateTornLetterVisual(Transform letterTransform, int sortingOrder)
    {
        SketchWorldLineDrawing drawing = letterTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(0.035f, new Color32(78, 57, 34, 255), sortingOrder);
        drawing.SetStrokes(BuildTornLetterStrokes());
        drawing.RevealProgress = 1f;
    }

    private void CreateLetterFragmentVisual(Transform fragmentTransform, int sortingOrder)
    {
        SketchWorldLineDrawing drawing = fragmentTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(0.035f, new Color32(78, 57, 34, 255), sortingOrder);
        drawing.SetStrokes(BuildLetterFragmentStrokes());
        drawing.RevealProgress = 1f;
    }

    private static void CreatePencilPart(Transform parent, string objectName, Vector3 localPosition, Vector3 localScale, Color color, int sortingOrder)
    {
        GameObject partObject = new GameObject(objectName);
        partObject.transform.SetParent(parent, false);
        partObject.transform.localPosition = localPosition;
        partObject.transform.localScale = localScale;

        SpriteRenderer spriteRenderer = partObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetPixelSprite();
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = sortingOrder;
    }

    private static Sprite GetPixelSprite()
    {
        if (pixelSprite != null)
        {
            return pixelSprite;
        }

        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        pixelSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        pixelSprite.name = "GeneratedFishingPencilPixel";
        return pixelSprite;
    }

    private static List<Vector3[]> BuildFishStrokes(int variant)
    {
        switch (Mathf.Abs(variant) % 3)
        {
            case 1:
                return BuildRoundFishStrokes();
            case 2:
                return BuildLongFishStrokes();
            default:
                return BuildSmallFishStrokes();
        }
    }

    private static List<Vector3[]> BuildSmallFishStrokes()
    {
        return new List<Vector3[]>
        {
            Points(-0.42f, 0f, -0.22f, 0.18f, 0.14f, 0.2f, 0.42f, 0f, 0.14f, -0.2f, -0.22f, -0.18f, -0.42f, 0f),
            Points(0.38f, 0f, 0.62f, 0.18f, 0.58f, 0f, 0.62f, -0.18f, 0.38f, 0f),
            Points(-0.18f, 0.02f, -0.14f, 0.02f),
            Points(-0.02f, 0.18f, -0.1f, 0.34f, 0.16f, 0.2f),
            Points(-0.02f, -0.18f, -0.1f, -0.34f, 0.16f, -0.2f)
        };
    }

    private static List<Vector3[]> BuildRoundFishStrokes()
    {
        return new List<Vector3[]>
        {
            Points(-0.5f, 0f, -0.28f, 0.26f, 0.12f, 0.28f, 0.46f, 0.02f, 0.12f, -0.28f, -0.28f, -0.25f, -0.5f, 0f),
            Points(0.42f, 0.02f, 0.68f, 0.24f, 0.62f, 0f, 0.68f, -0.24f, 0.42f, 0.02f),
            Points(-0.24f, 0.06f, -0.2f, 0.06f),
            Points(-0.06f, 0.24f, 0.02f, 0.42f, 0.22f, 0.22f),
            Points(-0.02f, -0.24f, 0.08f, -0.42f, 0.24f, -0.22f),
            Points(-0.34f, -0.02f, 0.12f, -0.02f)
        };
    }

    private static List<Vector3[]> BuildLongFishStrokes()
    {
        return new List<Vector3[]>
        {
            Points(-0.58f, 0f, -0.34f, 0.14f, 0.18f, 0.16f, 0.56f, 0f, 0.18f, -0.16f, -0.34f, -0.14f, -0.58f, 0f),
            Points(0.52f, 0f, 0.78f, 0.14f, 0.72f, 0f, 0.78f, -0.14f, 0.52f, 0f),
            Points(-0.34f, 0.03f, -0.3f, 0.03f),
            Points(-0.16f, 0.14f, -0.02f, 0.32f, 0.22f, 0.16f),
            Points(-0.14f, -0.14f, 0.02f, -0.32f, 0.24f, -0.16f),
            Points(-0.42f, -0.08f, -0.2f, -0.02f, 0.02f, -0.08f, 0.24f, -0.02f)
        };
    }

    private static List<Vector3[]> BuildSplashStrokes()
    {
        return new List<Vector3[]>
        {
            Points(-0.5f, 0f, -0.24f, 0.08f, 0f, 0.1f, 0.24f, 0.08f, 0.5f, 0f),
            Points(-0.34f, -0.08f, -0.12f, -0.02f, 0.12f, -0.02f, 0.34f, -0.08f),
            Points(-0.24f, 0.08f, -0.34f, 0.28f),
            Points(0.02f, 0.1f, 0.02f, 0.34f),
            Points(0.26f, 0.08f, 0.38f, 0.24f)
        };
    }

    private static List<Vector3[]> BuildBottleStrokes()
    {
        return new List<Vector3[]>
        {
            Points(-0.18f, 0.34f, -0.06f, 0.54f, 0.18f, 0.54f, 0.3f, 0.34f, 0.2f, 0.12f, 0.44f, -0.36f, 0.18f, -0.66f, -0.26f, -0.62f, -0.42f, -0.24f, -0.16f, 0.12f, -0.18f, 0.34f),
            Points(-0.06f, 0.54f, -0.02f, 0.74f, 0.18f, 0.76f, 0.18f, 0.54f),
            Points(-0.24f, -0.2f, 0.28f, -0.16f),
            Points(-0.16f, 0.06f, 0.18f, 0.02f)
        };
    }

    private static List<Vector3[]> BuildLetterPageStrokes()
    {
        return new List<Vector3[]>
        {
            Points(-0.42f, -0.28f, 0.42f, -0.28f, 0.42f, 0.3f, -0.42f, 0.3f, -0.42f, -0.28f),
            Points(-0.34f, 0.16f, 0.3f, 0.16f),
            Points(-0.34f, -0.02f, 0.24f, -0.02f),
            Points(-0.34f, -0.18f, 0.04f, -0.18f)
        };
    }

    private static List<Vector3[]> BuildTornLetterStrokes()
    {
        return new List<Vector3[]>
        {
            Points(-0.42f, -0.28f, 0.34f, -0.22f, 0.42f, 0.3f, -0.36f, 0.24f, -0.28f, 0.06f, -0.42f, -0.28f),
            Points(-0.22f, 0.12f, 0.24f, 0.12f),
            Points(-0.18f, -0.04f, 0.2f, -0.02f)
        };
    }

    private static List<Vector3[]> BuildLetterFragmentStrokes()
    {
        return new List<Vector3[]>
        {
            Points(-0.3f, -0.22f, 0.26f, -0.16f, 0.18f, 0.24f, -0.24f, 0.18f, -0.3f, -0.22f),
            Points(-0.16f, 0.04f, 0.1f, 0.06f),
            Points(-0.1f, -0.08f, 0.14f, -0.04f)
        };
    }

    private static Vector3[] Points(params float[] values)
    {
        int pointCount = values.Length / 2;
        Vector3[] points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector3(values[i * 2], values[i * 2 + 1], 0f);
        }

        return points;
    }

    private enum FishingOutcome
    {
        Fish,
        PencilFragment,
        LetterBottle
    }
}

public class GreenCrayonCollectible : MonoBehaviour, IInteractable
{
    [SerializeField] private float promptDistance = 1.45f;

    private Transform player;
    private GameObject promptObject;
    private CircleCollider2D interactionCollider;
    private Vector3 baseScale;
    private int promptSortingOrder = 72;
    private bool isCollecting;

    public void Configure(float interactionDistance, int sortingOrder)
    {
        promptDistance = interactionDistance;
        promptSortingOrder = sortingOrder;
        EnsurePrompt();
        EnsureCollider();
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || isCollecting || GameProgress.HasCollectedGreenCrayon || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > promptDistance)
        {
            return;
        }

        StartCoroutine(CollectRoutine());
    }

    private void Awake()
    {
        baseScale = transform.localScale;
        EnsurePrompt();
        EnsureCollider();
    }

    private void OnEnable()
    {
        FindPlayer();

        if (GameProgress.HasCollectedGreenCrayon)
        {
            gameObject.SetActive(false);
            return;
        }

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }

        if (interactionCollider != null)
        {
            interactionCollider.enabled = true;
        }
    }

    private void Update()
    {
        if (GameProgress.HasCollectedGreenCrayon)
        {
            gameObject.SetActive(false);
            return;
        }

        if (player == null)
        {
            FindPlayer();
        }

        if (promptObject == null)
        {
            return;
        }

        bool shouldShowPrompt = !isCollecting
            && player != null
            && Vector2.Distance(transform.position, player.position) <= promptDistance;
        promptObject.SetActive(shouldShowPrompt);
    }

    private IEnumerator CollectRoutine()
    {
        isCollecting = true;

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }

        if (interactionCollider != null)
        {
            interactionCollider.enabled = false;
        }

        Vector3 startPosition = transform.position;
        Vector3 fallbackTargetPosition = startPosition + new Vector3(0f, 0.82f, 0f);
        Vector3 startScale = transform.localScale;
        Quaternion startRotation = transform.localRotation;
        const float collectDuration = 0.48f;
        float elapsed = 0f;

        while (elapsed < collectDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 targetPosition = player != null
                ? player.position + new Vector3(0f, 0.72f, 0f)
                : fallbackTargetPosition;
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.28f;

            transform.position = position;
            transform.localScale = Vector3.Lerp(startScale, baseScale * 0.12f, easedProgress);
            transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, 28f, progress));
            yield return null;
        }

        GameProgress.CollectGreenCrayon();
        PencilFragmentHud.ShowCollected();
        gameObject.SetActive(false);
    }

    private void EnsurePrompt()
    {
        if (promptObject == null)
        {
            promptObject = new GameObject("GeneratedGreenCrayonPrompt");
            promptObject.transform.SetParent(transform, false);
            promptObject.transform.localPosition = new Vector3(0f, 0.66f, 0f);

            TextMesh textMesh = promptObject.AddComponent<TextMesh>();
            textMesh.text = "E";
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.fontSize = 64;
            textMesh.characterSize = 0.075f;
            textMesh.color = new Color32(35, 32, 28, 255);
        }

        MeshRenderer meshRenderer = promptObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = promptSortingOrder;
        }

        promptObject.SetActive(false);
    }

    private void EnsureCollider()
    {
        if (interactionCollider != null)
        {
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = 0.58f;
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }
}
