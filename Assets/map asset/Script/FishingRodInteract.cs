using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRodInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float promptDistance = 2.35f;
    [SerializeField] private float fishingDuration = 0.62f;
    [SerializeField, Range(0f, 1f)] private float pencilFragmentChance = 0.1f;

    private const float LineWidth = 0.045f;
    private const int FishSortingOrder = 69;
    private const int PencilSortingOrder = 68;

    private static Sprite pixelSprite;

    private Transform player;
    private GameObject promptObject;
    private CircleCollider2D interactionCollider;
    private Quaternion baseLocalRotation;
    private Vector3 pondLocalPosition;
    private int promptSortingOrder = 70;
    private bool isFishing;
    private bool interactionLocked;

    public void Configure(Vector3 localPondPosition, int sortingOrder)
    {
        pondLocalPosition = localPondPosition;
        promptSortingOrder = sortingOrder;
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

        StartCoroutine(FishingRoutine());
    }

    private void Awake()
    {
        baseLocalRotation = transform.localRotation;
        EnsurePrompt();
        EnsureCollider();
    }

    private void OnEnable()
    {
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

        bool canFindPencilFragment = GameProgress.CanDropPencilFragmentFrom(PencilFragmentSource.Fishing);
        if (canFindPencilFragment && Random.value < pencilFragmentChance)
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

        SpawnFishJump();
        isFishing = false;
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
        drawing.SetStrokes(BuildFishStrokes());
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
        return !interactionLocked;
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

    private static List<Vector3[]> BuildFishStrokes()
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
}
