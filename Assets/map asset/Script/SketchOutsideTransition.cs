using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SketchOutsideTransition : MonoBehaviour
{
    private const float MapSlideDuration = 0.62f;
    private const float MapRevealDistance = 5.8f;
    private const float FenceRevealDistance = 8.5f;
    private const float FenceRevealTriggerSpacing = 5.5f;
    private const float MapRevealDuration = 1.05f;
    private const float SpriteRevealStartScale = 0.82f;
    private const float BackgroundRevealDuration = 1.15f;
    private const float HouseDrawDuration = 4.2f;
    private const float HouseLineWidth = 0.065f;
    private const int BackgroundSortingOrder = -20;
    private const int PropSortingOrder = 20;
    private const int GrassSortingOrder = PropSortingOrder + 1;
    private const int TreeSortingOrder = PropSortingOrder + 3;
    private const int MailboxSortingOrder = PropSortingOrder + 4;
    private const int QuestNpcSortingOrder = MailboxSortingOrder + 1;
    private const int DecorationRandomSeed = 20260520;
    private const int TreeSpawnCount = 20;
    private const int GrassSpawnCount = 70;
    private const int DecorationPickAttempts = 1200;
    private const float BoundaryColliderThickness = 0.55f;
    private const float MailboxHouseThirdScale = 0.52f;
    private const float DecorationOverlapPadding = 0.28f;
    private const float QuestNpcPromptDistance = 2.3f;
    private const float QuestNpcPromptPulseSpeed = 4.4f;
    private const string GrassResourcePath = "Outside/grass_thick";
    private const string MailboxResourcePath = "Outside/mailbox_thick";
    private static readonly Vector2 GrassScaleRange = new Vector2(0.5508f, 0.8424f);
    private static readonly Vector2 TreeScaleRange = new Vector2(1.12f, 1.32f);
    private static readonly Vector3 OutsidePlayerOffset = Vector3.zero;
    private static readonly Vector3 HouseOffset = new Vector3(0f, 2f, 0f);
    private static readonly Vector3 DoorOffset = new Vector3(0f, 1.35f, 0f);
    private static readonly Vector3 NeighborHouseOffset = new Vector3(18.4f, 3.1f, 0f);
    private static readonly Vector3 ForestEntranceOffset = new Vector3(21.5f, -14.2f, 0f);
    private static readonly Vector3 PondOffset = new Vector3(-23.5f, -13.6f, 0f);
    private static readonly Vector3 MapSlideStartOffset = new Vector3(0f, -1.4f, 0f);
    private static readonly Vector2 BackgroundSize = new Vector2(70f, 48f);
    private static readonly Vector2 DecorationMin = new Vector2(-33f, -21f);
    private static readonly Vector2 DecorationMax = new Vector2(33f, 21f);

    private static SketchOutsideTransition instance;

    private GameObject contentRoot;
    private GameObject propRoot;
    private GameObject questNpcPromptObject;
    private TextMesh questNpcPromptText;
    private TextMesh questNpcPromptInnerGlowText;
    private TextMesh questNpcPromptOuterGlowText;
    private SpriteRenderer backgroundRenderer;
    private SketchWorldLineDrawing houseDrawing;
    private SketchWorldLineDrawing questNpcDrawing;
    private SketchOutsideDoorInteract houseDoorInteract;
    private BoxCollider2D houseDoorCollider;
    private readonly List<MapRevealItem> mapRevealItems = new List<MapRevealItem>();
    private Transform outsidePlayer;
    private Coroutine transitionRoutine;
    private Sprite whiteSprite;
    private bool hasCompletedOutsideMap;
    private bool isOutsideActive;

    private sealed class MapRevealItem
    {
        public readonly Vector3[] LocalTriggerPositions;
        public readonly SketchWorldLineDrawing LineDrawing;
        public readonly SpriteRenderer SpriteRenderer;
        public readonly Transform SpriteTransform;
        public readonly Vector3 BaseScale;
        public readonly float RevealDistance;
        public readonly Behaviour EnableWhenRevealed;

        public float Progress;
        public bool IsRevealing;

        public MapRevealItem(Vector3 localPosition, SketchWorldLineDrawing lineDrawing, Behaviour enableWhenRevealed = null)
        {
            LocalTriggerPositions = new[] { localPosition };
            LineDrawing = lineDrawing;
            RevealDistance = MapRevealDistance;
            EnableWhenRevealed = enableWhenRevealed;
        }

        public MapRevealItem(Vector3[] localTriggerPositions, SketchWorldLineDrawing lineDrawing, Behaviour enableWhenRevealed = null)
        {
            LocalTriggerPositions = localTriggerPositions != null && localTriggerPositions.Length > 0
                ? localTriggerPositions
                : new[] { Vector3.zero };
            LineDrawing = lineDrawing;
            RevealDistance = MapRevealDistance;
            EnableWhenRevealed = enableWhenRevealed;
        }

        public MapRevealItem(Vector3[] localTriggerPositions, float revealDistance, SketchWorldLineDrawing lineDrawing, Behaviour enableWhenRevealed = null)
        {
            LocalTriggerPositions = localTriggerPositions != null && localTriggerPositions.Length > 0
                ? localTriggerPositions
                : new[] { Vector3.zero };
            RevealDistance = Mathf.Max(0.01f, revealDistance);
            LineDrawing = lineDrawing;
            EnableWhenRevealed = enableWhenRevealed;
        }

        public MapRevealItem(Vector3 localPosition, SpriteRenderer spriteRenderer, Transform spriteTransform, Behaviour enableWhenRevealed = null)
        {
            LocalTriggerPositions = new[] { localPosition };
            SpriteRenderer = spriteRenderer;
            SpriteTransform = spriteTransform;
            BaseScale = spriteTransform.localScale;
            RevealDistance = MapRevealDistance;
            EnableWhenRevealed = enableWhenRevealed;
        }

        public void ApplyProgress(float progress)
        {
            Progress = Mathf.Clamp01(progress);
            float easedProgress = Mathf.SmoothStep(0f, 1f, Progress);

            if (LineDrawing != null)
            {
                LineDrawing.RevealProgress = easedProgress;
            }

            if (SpriteRenderer != null)
            {
                Color color = SpriteRenderer.color;
                color.a = easedProgress;
                SpriteRenderer.color = color;
            }

            if (SpriteTransform != null)
            {
                SpriteTransform.localScale = BaseScale * Mathf.Lerp(SpriteRevealStartScale, 1f, easedProgress);
            }

            if (EnableWhenRevealed != null)
            {
                EnableWhenRevealed.enabled = Progress >= 1f;
            }
        }
    }

    private struct DecorationFootprint
    {
        public readonly Vector2 GroundPosition;
        public readonly Vector2 Center;
        public readonly Vector2 HalfSize;

        public DecorationFootprint(Vector2 groundPosition, Vector2 center, Vector2 halfSize)
        {
            GroundPosition = groundPosition;
            Center = center;
            HalfSize = halfSize;
        }
    }

    public static void PlayExit(GameObject player, Vector3 exitTargetPosition, Vector3 interiorEntryPosition)
    {
        SketchOutsideTransition transition = GetOrCreateInstance();
        transition.BeginExit(player, exitTargetPosition, interiorEntryPosition);
    }

    public void EnterWoodhouse(GameObject player, Vector3 interiorEntryPosition)
    {
        if (transitionRoutine != null)
        {
            StopCoroutine(transitionRoutine);
            transitionRoutine = null;
        }

        SetPlayerMovementEnabled(player, true);
        player.transform.position = interiorEntryPosition;
        SnapCameraToPlayer(player);
        isOutsideActive = false;
        outsidePlayer = null;

        if (contentRoot != null)
        {
            houseDoorInteract.SetInteractionEnabled(false);
        }

        SetQuestNpcPromptVisible(false);
    }

    private static SketchOutsideTransition GetOrCreateInstance()
    {
        if (instance != null)
        {
            return instance;
        }

        instance = FindAnyObjectByType<SketchOutsideTransition>();

        if (instance != null)
        {
            return instance;
        }

        GameObject transitionObject = new GameObject("GeneratedSketchOutsideTransition");
        instance = transitionObject.AddComponent<SketchOutsideTransition>();
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (!isOutsideActive || outsidePlayer == null || propRoot == null || !propRoot.activeInHierarchy)
        {
            return;
        }

        UpdateMapRevealItems(outsidePlayer.position);
        UpdateQuestNpcPrompt();
    }

    private void BeginExit(GameObject player, Vector3 exitTargetPosition, Vector3 interiorEntryPosition)
    {
        EnsureSetup();

        if (transitionRoutine != null)
        {
            StopCoroutine(transitionRoutine);
        }

        transitionRoutine = StartCoroutine(PlayExitRoutine(player, exitTargetPosition, interiorEntryPosition));
    }

    private IEnumerator PlayExitRoutine(GameObject player, Vector3 exitTargetPosition, Vector3 interiorEntryPosition)
    {
        isOutsideActive = false;
        outsidePlayer = player.transform;
        transform.position = exitTargetPosition;
        contentRoot.SetActive(true);

        player.transform.position = exitTargetPosition + OutsidePlayerOffset;
        SnapCameraToPlayer(player);
        houseDoorInteract.Configure(this, interiorEntryPosition, 1.6f);

        SetPlayerMovementEnabled(player, false);

        if (hasCompletedOutsideMap)
        {
            SetCompletedOutsideMapState();
            yield return SlideMapIntoPlace();
            isOutsideActive = true;
            SetPlayerMovementEnabled(player, true);
            transitionRoutine = null;
            yield break;
        }

        SetInitialOutsideMapState();

        yield return SlideMapIntoPlace();
        yield return RevealWhiteBackground();
        yield return DrawHouse();

        if (propRoot != null)
        {
            propRoot.SetActive(true);
            UpdateMapRevealItems(player.transform.position);
            UpdateQuestNpcPrompt();
        }

        hasCompletedOutsideMap = true;
        houseDoorCollider.enabled = true;
        houseDoorInteract.SetInteractionEnabled(true);
        isOutsideActive = true;
        SetPlayerMovementEnabled(player, true);
        transitionRoutine = null;
    }

    private void SetInitialOutsideMapState()
    {
        contentRoot.transform.localPosition = MapSlideStartOffset;
        backgroundRenderer.transform.localPosition = Vector3.zero;
        backgroundRenderer.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
        backgroundRenderer.color = Color.white;
        houseDrawing.RevealProgress = 0f;
        houseDoorCollider.enabled = false;
        houseDoorInteract.SetInteractionEnabled(false);
        ResetMapRevealItems();

        if (propRoot != null)
        {
            propRoot.SetActive(false);
        }
    }

    private void SetCompletedOutsideMapState()
    {
        contentRoot.transform.localPosition = MapSlideStartOffset;
        backgroundRenderer.transform.localPosition = Vector3.zero;
        backgroundRenderer.transform.localScale = new Vector3(BackgroundSize.x, BackgroundSize.y, 1f);
        backgroundRenderer.color = Color.white;
        houseDrawing.RevealProgress = 1f;
        houseDoorCollider.enabled = true;
        houseDoorInteract.SetInteractionEnabled(true);
        ApplyMapRevealItems();

        if (propRoot != null)
        {
            propRoot.SetActive(true);
        }
    }

    private IEnumerator SlideMapIntoPlace()
    {
        Vector3 startPosition = contentRoot.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < MapSlideDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / MapSlideDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            contentRoot.transform.localPosition = Vector3.Lerp(startPosition, Vector3.zero, easedProgress);
            yield return null;
        }

        contentRoot.transform.localPosition = Vector3.zero;
    }

    private void ResetMapRevealItems()
    {
        for (int i = 0; i < mapRevealItems.Count; i++)
        {
            mapRevealItems[i].IsRevealing = false;
            mapRevealItems[i].ApplyProgress(0f);
        }

        SetQuestNpcPromptVisible(false);
    }

    private void ApplyMapRevealItems()
    {
        for (int i = 0; i < mapRevealItems.Count; i++)
        {
            mapRevealItems[i].ApplyProgress(mapRevealItems[i].Progress);
        }
    }

    private void UpdateMapRevealItems(Vector3 playerPosition)
    {
        for (int i = 0; i < mapRevealItems.Count; i++)
        {
            MapRevealItem item = mapRevealItems[i];

            if (item.Progress >= 1f)
            {
                continue;
            }

            if (IsPlayerNearRevealItem(playerPosition, item))
            {
                item.IsRevealing = true;
            }

            if (!item.IsRevealing)
            {
                continue;
            }

            item.ApplyProgress(item.Progress + Time.deltaTime / MapRevealDuration);
        }
    }

    private bool IsPlayerNearRevealItem(Vector3 playerPosition, MapRevealItem item)
    {
        for (int i = 0; i < item.LocalTriggerPositions.Length; i++)
        {
            Vector3 itemPosition = propRoot.transform.TransformPoint(item.LocalTriggerPositions[i]);

            if (Vector2.Distance(playerPosition, itemPosition) <= item.RevealDistance)
            {
                return true;
            }
        }

        return false;
    }

    private void RegisterLineReveal(SketchWorldLineDrawing lineDrawing, params Vector3[] localTriggerPositions)
    {
        if (lineDrawing == null)
        {
            return;
        }

        MapRevealItem item = new MapRevealItem(localTriggerPositions, lineDrawing);
        item.ApplyProgress(0f);
        mapRevealItems.Add(item);
    }

    private void RegisterLineReveal(SketchWorldLineDrawing lineDrawing, Behaviour enableWhenRevealed, params Vector3[] localTriggerPositions)
    {
        if (lineDrawing == null)
        {
            return;
        }

        MapRevealItem item = new MapRevealItem(localTriggerPositions, lineDrawing, enableWhenRevealed);
        item.ApplyProgress(0f);
        mapRevealItems.Add(item);
    }

    private void RegisterLineReveal(SketchWorldLineDrawing lineDrawing, float revealDistance, params Vector3[] localTriggerPositions)
    {
        if (lineDrawing == null)
        {
            return;
        }

        MapRevealItem item = new MapRevealItem(localTriggerPositions, revealDistance, lineDrawing);
        item.ApplyProgress(0f);
        mapRevealItems.Add(item);
    }

    private void RegisterSpriteReveal(SpriteRenderer spriteRenderer, Vector3 localPosition, Behaviour enableWhenRevealed = null)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        MapRevealItem item = new MapRevealItem(localPosition, spriteRenderer, spriteRenderer.transform, enableWhenRevealed);
        item.ApplyProgress(0f);
        mapRevealItems.Add(item);
    }

    private void UpdateQuestNpcPrompt()
    {
        if (questNpcPromptObject == null || questNpcDrawing == null || outsidePlayer == null || propRoot == null)
        {
            return;
        }

        bool isNpcVisible = questNpcDrawing.RevealProgress >= 0.95f;
        Vector3 npcPosition = propRoot.transform.TransformPoint(GetQuestNpcPosition());
        bool isNearNpc = Vector2.Distance(outsidePlayer.position, npcPosition) <= QuestNpcPromptDistance;

        SetQuestNpcPromptVisible(isOutsideActive && isNpcVisible && isNearNpc);
        UpdateQuestNpcPromptGlow();
    }

    private void SetQuestNpcPromptVisible(bool visible)
    {
        if (questNpcPromptObject != null && questNpcPromptObject.activeSelf != visible)
        {
            questNpcPromptObject.SetActive(visible);
        }
    }

    private void UpdateQuestNpcPromptGlow()
    {
        if (questNpcPromptObject == null || !questNpcPromptObject.activeSelf)
        {
            return;
        }

        float pulse = Mathf.SmoothStep(0f, 1f, (Mathf.Sin(Time.time * QuestNpcPromptPulseSpeed) + 1f) * 0.5f);

        if (questNpcPromptText != null)
        {
            questNpcPromptText.color = Color.Lerp(
                new Color32(35, 32, 28, 255),
                new Color32(82, 62, 8, 255),
                pulse);
        }

        if (questNpcPromptInnerGlowText != null)
        {
            questNpcPromptInnerGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.02f, 1.12f, pulse);
            questNpcPromptInnerGlowText.color = new Color(1f, 0.84f, 0.24f, Mathf.Lerp(0.45f, 0.82f, pulse));
        }

        if (questNpcPromptOuterGlowText != null)
        {
            questNpcPromptOuterGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.08f, 1.24f, pulse);
            questNpcPromptOuterGlowText.color = new Color(1f, 0.74f, 0.12f, Mathf.Lerp(0.16f, 0.42f, pulse));
        }
    }

    private IEnumerator RevealWhiteBackground()
    {
        float elapsed = 0f;

        while (elapsed < BackgroundRevealDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / BackgroundRevealDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            backgroundRenderer.transform.localScale = new Vector3(
                Mathf.Lerp(0.01f, BackgroundSize.x, easedProgress),
                Mathf.Lerp(0.01f, BackgroundSize.y, easedProgress),
                1f);

            yield return null;
        }

        backgroundRenderer.transform.localScale = new Vector3(BackgroundSize.x, BackgroundSize.y, 1f);
    }

    private IEnumerator DrawHouse()
    {
        float elapsed = 0f;
        float revealProgress = 0f;

        while (elapsed < HouseDrawDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / HouseDrawDuration);
            revealProgress = Mathf.Max(revealProgress, ApplyHandDrawnPace(progress));
            houseDrawing.RevealProgress = revealProgress;
            yield return null;
        }

        houseDrawing.RevealProgress = 1f;
    }

    private void EnsureSetup()
    {
        if (contentRoot != null)
        {
            return;
        }

        contentRoot = new GameObject("GeneratedSketchOutsideContent");
        contentRoot.transform.SetParent(transform, false);

        backgroundRenderer = CreateBackground();
        houseDrawing = CreateHouseDrawing();
        houseDoorInteract = CreateHouseDoor();
        CreateOutsideProps();
        contentRoot.SetActive(false);
    }

    private SpriteRenderer CreateBackground()
    {
        GameObject backgroundObject = new GameObject("GeneratedWhiteOutsideBackground");
        backgroundObject.transform.SetParent(contentRoot.transform, false);
        backgroundObject.transform.localPosition = Vector3.zero;

        SpriteRenderer spriteRenderer = backgroundObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetWhiteSprite();
        spriteRenderer.sortingOrder = BackgroundSortingOrder;
        spriteRenderer.color = Color.white;
        return spriteRenderer;
    }

    private SketchWorldLineDrawing CreateHouseDrawing()
    {
        GameObject drawingObject = new GameObject("GeneratedOutsideHouseLineDrawing");
        drawingObject.transform.SetParent(contentRoot.transform, false);
        drawingObject.transform.localPosition = HouseOffset;

        SketchWorldLineDrawing drawing = drawingObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(HouseLineWidth, new Color32(35, 32, 28, 255), PropSortingOrder);
        drawing.SetStrokes(BuildHouseStrokes());
        drawing.RevealProgress = 0f;
        return drawing;
    }

    private SketchOutsideDoorInteract CreateHouseDoor()
    {
        GameObject doorObject = new GameObject("GeneratedOutsideHouseDoor");
        doorObject.transform.SetParent(contentRoot.transform, false);
        doorObject.transform.localPosition = DoorOffset;

        houseDoorCollider = doorObject.AddComponent<BoxCollider2D>();
        houseDoorCollider.isTrigger = true;
        houseDoorCollider.size = new Vector2(0.9f, 1.2f);
        houseDoorCollider.enabled = false;

        return doorObject.AddComponent<SketchOutsideDoorInteract>();
    }

    private void CreateOutsideProps()
    {
        mapRevealItems.Clear();
        propRoot = new GameObject("GeneratedOutsideProps");
        propRoot.transform.SetParent(contentRoot.transform, false);

        CreateOutsideBoundary();

        SketchWorldLineDrawing neighborHouse = CreateLineDrawing(
            "GeneratedNeighborHouseWithMailbox",
            NeighborHouseOffset,
            BuildNeighborHouseStrokes(),
            PropSortingOrder,
            propRoot.transform);
        RegisterLineReveal(neighborHouse, NeighborHouseOffset);

        CreateSpriteProp(
            "GeneratedNeighborMailbox",
            MailboxResourcePath,
            GetNeighborMailboxPosition(),
            new Vector3(MailboxHouseThirdScale, MailboxHouseThirdScale, 1f),
            MailboxSortingOrder,
            propRoot.transform);

        CreateQuestNpc();
        CreateForestEntrance();
        CreatePondAndFishingRod();
        CreateRandomOutsideDecorations();
        propRoot.SetActive(false);
    }

    private void CreateQuestNpc()
    {
        Vector3 npcPosition = GetQuestNpcPosition();

        questNpcDrawing = CreateLineDrawing(
            "GeneratedQuestGiverNpcLineDrawing",
            npcPosition,
            BuildQuestNpcStrokes(),
            QuestNpcSortingOrder,
            propRoot.transform);
        RegisterLineReveal(questNpcDrawing, npcPosition);

        questNpcPromptObject = new GameObject("GeneratedQuestNpcPrompt");
        questNpcPromptObject.transform.SetParent(questNpcDrawing.transform, false);
        questNpcPromptObject.transform.localPosition = new Vector3(0f, 2.15f, 0f);

        questNpcPromptOuterGlowText = CreateQuestNpcPromptText(
            "GeneratedQuestNpcPromptOuterGlow",
            questNpcPromptObject.transform,
            0.104f,
            new Color(1f, 0.74f, 0.12f, 0.26f),
            48);
        questNpcPromptInnerGlowText = CreateQuestNpcPromptText(
            "GeneratedQuestNpcPromptInnerGlow",
            questNpcPromptObject.transform,
            0.086f,
            new Color(1f, 0.84f, 0.24f, 0.62f),
            49);
        questNpcPromptText = CreateQuestNpcPromptText(
            "GeneratedQuestNpcPromptText",
            questNpcPromptObject.transform,
            0.07f,
            new Color32(35, 32, 28, 255),
            50);

        questNpcPromptObject.SetActive(false);
    }

    private static TextMesh CreateQuestNpcPromptText(
        string objectName,
        Transform parent,
        float characterSize,
        Color color,
        int sortingOrder)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);
        textObject.transform.localPosition = Vector3.zero;

        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = "[\uD3B8\uC9C0]+[?]";
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 64;
        textMesh.characterSize = characterSize;
        textMesh.color = color;

        MeshRenderer meshRenderer = textObject.GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = sortingOrder;

        return textMesh;
    }

    private void CreateOutsideBoundary()
    {
        float halfWidth = BackgroundSize.x * 0.5f;
        float halfHeight = BackgroundSize.y * 0.5f;
        float thickness = BoundaryColliderThickness;

        SketchWorldLineDrawing topFence = CreateLineDrawing("GeneratedOutsideFence_Top", Vector3.zero, BuildFenceTopStrokes(), PropSortingOrder + 3, propRoot.transform);
        RegisterLineReveal(topFence, FenceRevealDistance, BuildHorizontalFenceRevealPoints(halfHeight, halfWidth));

        SketchWorldLineDrawing bottomFence = CreateLineDrawing("GeneratedOutsideFence_Bottom", Vector3.zero, BuildFenceBottomStrokes(), PropSortingOrder + 3, propRoot.transform);
        RegisterLineReveal(bottomFence, FenceRevealDistance, BuildHorizontalFenceRevealPoints(-halfHeight, halfWidth));

        SketchWorldLineDrawing leftFence = CreateLineDrawing("GeneratedOutsideFence_Left", Vector3.zero, BuildFenceLeftStrokes(), PropSortingOrder + 3, propRoot.transform);
        RegisterLineReveal(leftFence, FenceRevealDistance, BuildVerticalFenceRevealPoints(-halfWidth, halfHeight));

        SketchWorldLineDrawing rightFence = CreateLineDrawing("GeneratedOutsideFence_Right", Vector3.zero, BuildFenceRightStrokes(), PropSortingOrder + 3, propRoot.transform);
        RegisterLineReveal(rightFence, FenceRevealDistance, BuildVerticalFenceRevealPoints(halfWidth, halfHeight));

        GameObject boundaryRoot = new GameObject("GeneratedOutsideBoundaryColliders");
        boundaryRoot.transform.SetParent(propRoot.transform, false);

        CreateBoundaryCollider("GeneratedOutsideBoundary_Left", boundaryRoot.transform, new Vector2(-halfWidth - thickness * 0.5f, 0f), new Vector2(thickness, BackgroundSize.y + thickness * 2f));
        CreateBoundaryCollider("GeneratedOutsideBoundary_Right", boundaryRoot.transform, new Vector2(halfWidth + thickness * 0.5f, 0f), new Vector2(thickness, BackgroundSize.y + thickness * 2f));
        CreateBoundaryCollider("GeneratedOutsideBoundary_Top", boundaryRoot.transform, new Vector2(0f, halfHeight + thickness * 0.5f), new Vector2(BackgroundSize.x + thickness * 2f, thickness));
        CreateBoundaryCollider("GeneratedOutsideBoundary_Bottom", boundaryRoot.transform, new Vector2(0f, -halfHeight - thickness * 0.5f), new Vector2(BackgroundSize.x + thickness * 2f, thickness));
    }

    private void CreateForestEntrance()
    {
        SketchWorldLineDrawing forestEntrance = CreateLineDrawing(
            "GeneratedForestEntranceLineDrawing",
            ForestEntranceOffset,
            BuildForestEntranceStrokes(),
            TreeSortingOrder + 2,
            propRoot.transform);
        RegisterLineReveal(forestEntrance, ForestEntranceOffset);
    }

    private void CreatePondAndFishingRod()
    {
        SketchWorldLineDrawing pondDrawing = CreateLineDrawing(
            "GeneratedPondLineDrawing",
            PondOffset,
            BuildPondStrokes(),
            PropSortingOrder + 2,
            propRoot.transform,
            new Color32(57, 104, 132, 255));
        RegisterLineReveal(pondDrawing, PondOffset);
        CreatePondCollider();

        Vector3 fishingRodPosition = PondOffset + new Vector3(4.55f, 1.05f, 0f);
        SketchWorldLineDrawing fishingRodDrawing = CreateLineDrawing(
            "GeneratedFishingRodLineDrawing",
            fishingRodPosition,
            BuildFishingRodStrokes(),
            PropSortingOrder + 5,
            propRoot.transform,
            new Color32(64, 44, 30, 255));
        FishingRodInteract fishingRodInteract = fishingRodDrawing.gameObject.AddComponent<FishingRodInteract>();
        fishingRodInteract.Configure(PondOffset, PropSortingOrder + 50);
        fishingRodInteract.enabled = false;

        RegisterLineReveal(fishingRodDrawing, fishingRodInteract, fishingRodPosition);
    }

    private void CreatePondCollider()
    {
        GameObject colliderObject = new GameObject("GeneratedPondCollider");
        colliderObject.transform.SetParent(propRoot.transform, false);
        colliderObject.transform.localPosition = PondOffset;

        PolygonCollider2D collider = colliderObject.AddComponent<PolygonCollider2D>();
        collider.isTrigger = false;
        collider.SetPath(0, EllipseColliderPoints(4.9f, 2.52f, 32));
    }

    private void CreateBoundaryCollider(string objectName, Transform parent, Vector2 localPosition, Vector2 size)
    {
        GameObject colliderObject = new GameObject(objectName);
        colliderObject.transform.SetParent(parent, false);
        colliderObject.transform.localPosition = localPosition;

        BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>();
        collider.size = size;
        collider.isTrigger = false;
    }

    private void CreateRandomOutsideDecorations()
    {
        System.Random random = new System.Random(DecorationRandomSeed);
        List<DecorationFootprint> usedFootprints = new List<DecorationFootprint>();

        SpawnRandomPineTrees("GeneratedPineTree", TreeSpawnCount, 7f, TreeScaleRange.x, TreeScaleRange.y, TreeSortingOrder, random, usedFootprints, -31f, 31f, DecorationMin.y, 12.5f);
        SpawnRandomSpriteProps("GeneratedGrass", GrassResourcePath, GrassSpawnCount, 2.1f, GrassScaleRange.x, GrassScaleRange.y, GrassSortingOrder, random, usedFootprints, DecorationMin.x, DecorationMax.x, DecorationMin.y, DecorationMax.y);
    }

    private void SpawnRandomPineTrees(
        string objectPrefix,
        int count,
        float minimumSpacing,
        float minScale,
        float maxScale,
        int sortingOrder,
        System.Random random,
        List<DecorationFootprint> usedFootprints,
        float minX,
        float maxX,
        float minY,
        float maxY)
    {
        for (int i = 0; i < count; i++)
        {
            float scale = RandomRange(random, minScale, maxScale);
            Vector2 halfSize = GetPineDecorationHalfSize(scale);

            if (!TryPickDecorationPosition(random, usedFootprints, halfSize, minimumSpacing, minX, maxX, minY, maxY, out Vector2 position))
            {
                continue;
            }

            DecorationFootprint footprint = BuildDecorationFootprint(position, halfSize);

            CreatePineTreeProp(
                $"{objectPrefix}_{i:00}",
                new Vector3(position.x, position.y, 0f),
                scale,
                sortingOrder,
                propRoot.transform);

            usedFootprints.Add(footprint);
        }
    }

    private void SpawnRandomSpriteProps(
        string objectPrefix,
        string resourcePath,
        int count,
        float minimumSpacing,
        float minScale,
        float maxScale,
        int sortingOrder,
        System.Random random,
        List<DecorationFootprint> usedFootprints,
        float minX,
        float maxX,
        float minY,
        float maxY)
    {
        Sprite sprite = LoadSketchSprite(resourcePath);

        if (sprite == null)
        {
            Debug.LogWarning($"Outside prop sprite not found: {resourcePath}", this);
            return;
        }

        for (int i = 0; i < count; i++)
        {
            float scale = RandomRange(random, minScale, maxScale);
            float scaleX = random.NextDouble() < 0.5d ? -scale : scale;
            Vector3 localScale = new Vector3(scaleX, scale, 1f);
            Vector2 halfSize = GetDecorationHalfSize(sprite, localScale);

            if (!TryPickDecorationPosition(random, usedFootprints, halfSize, minimumSpacing, minX, maxX, minY, maxY, out Vector2 position))
            {
                continue;
            }

            DecorationFootprint footprint = BuildDecorationFootprint(position, halfSize);

            CreateSpriteProp(
                $"{objectPrefix}_{i:00}",
                sprite,
                new Vector3(position.x, position.y, 0f),
                localScale,
                sortingOrder,
                propRoot.transform,
                objectPrefix == "GeneratedGrass");

            usedFootprints.Add(footprint);
        }
    }

    private bool TryPickDecorationPosition(
        System.Random random,
        List<DecorationFootprint> usedFootprints,
        Vector2 halfSize,
        float minimumSpacing,
        float minX,
        float maxX,
        float minY,
        float maxY,
        out Vector2 position)
    {
        float minimumSpacingSqr = minimumSpacing * minimumSpacing;

        for (int attempt = 0; attempt < DecorationPickAttempts; attempt++)
        {
            position = new Vector2(
                RandomRange(random, minX, maxX),
                RandomRange(random, minY, maxY));

            if (IsInsideClearZone(position))
            {
                continue;
            }

            DecorationFootprint candidate = BuildDecorationFootprint(position, halfSize);
            bool hasEnoughSpace = true;

            for (int i = 0; i < usedFootprints.Count; i++)
            {
                if (DoFootprintsOverlap(candidate, usedFootprints[i])
                    || (usedFootprints[i].GroundPosition - position).sqrMagnitude < minimumSpacingSqr)
                {
                    hasEnoughSpace = false;
                    break;
                }
            }

            if (hasEnoughSpace)
            {
                return true;
            }
        }

        position = Vector2.zero;
        return false;
    }

    private static Vector2 GetDecorationHalfSize(Sprite sprite, Vector3 localScale)
    {
        return new Vector2(
            sprite.bounds.extents.x * Mathf.Abs(localScale.x),
            sprite.bounds.extents.y * Mathf.Abs(localScale.y));
    }

    private static Vector2 GetPineDecorationHalfSize(float scale)
    {
        return new Vector2(1.35f * scale, 1.72f * scale);
    }

    private static DecorationFootprint BuildDecorationFootprint(Vector2 groundPosition, Vector2 halfSize)
    {
        Vector2 center = groundPosition + Vector2.up * halfSize.y;
        return new DecorationFootprint(groundPosition, center, halfSize);
    }

    private static bool DoFootprintsOverlap(DecorationFootprint first, DecorationFootprint second)
    {
        return Mathf.Abs(first.Center.x - second.Center.x) < first.HalfSize.x + second.HalfSize.x + DecorationOverlapPadding
            && Mathf.Abs(first.Center.y - second.Center.y) < first.HalfSize.y + second.HalfSize.y + DecorationOverlapPadding;
    }

    private static bool IsInsideClearZone(Vector2 position)
    {
        return IsInsideBox(position, HouseOffset, new Vector2(5.4f, 4.2f))
            || IsInsideBox(position, NeighborHouseOffset, new Vector2(5.2f, 3.8f))
            || IsInsideBox(position, ForestEntranceOffset, new Vector2(3.6f, 3.2f))
            || IsInsideBox(position, PondOffset, new Vector2(7.4f, 4.6f))
            || IsInsideBox(position, GetQuestNpcPosition(), new Vector2(2.2f, 2.6f))
            || IsInsideBox(position, Vector3.zero, new Vector2(4.4f, 3.2f));
    }

    private static Vector3 GetNeighborMailboxPosition()
    {
        return NeighborHouseOffset + new Vector3(-2.02f, -0.96f, 0f);
    }

    private static Vector3 GetQuestNpcPosition()
    {
        return GetNeighborMailboxPosition() + new Vector3(-1.7f, 0.05f, 0f);
    }

    private static bool IsInsideBox(Vector2 position, Vector3 center, Vector2 halfExtents)
    {
        return Mathf.Abs(position.x - center.x) <= halfExtents.x
            && Mathf.Abs(position.y - center.y) <= halfExtents.y;
    }

    private static float RandomRange(System.Random random, float min, float max)
    {
        return min + (float)random.NextDouble() * (max - min);
    }

    private SketchWorldLineDrawing CreateLineDrawing(string objectName, Vector3 localPosition, List<Vector3[]> strokes, int sortingOrder, Transform parent = null)
    {
        return CreateLineDrawing(objectName, localPosition, strokes, sortingOrder, parent, new Color32(35, 32, 28, 255));
    }

    private SketchWorldLineDrawing CreateLineDrawing(string objectName, Vector3 localPosition, List<Vector3[]> strokes, int sortingOrder, Transform parent, Color color)
    {
        GameObject drawingObject = new GameObject(objectName);
        drawingObject.transform.SetParent(parent != null ? parent : contentRoot.transform, false);
        drawingObject.transform.localPosition = localPosition;

        SketchWorldLineDrawing drawing = drawingObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(HouseLineWidth, color, sortingOrder);
        drawing.SetStrokes(strokes);
        return drawing;
    }

    private void CreateSpriteProp(string objectName, string resourcePath, Vector3 localPosition, Vector3 localScale, int sortingOrder, Transform parent = null, bool addGrassSway = false)
    {
        Sprite sprite = LoadSketchSprite(resourcePath);
        if (sprite == null)
        {
            Debug.LogWarning($"Outside prop sprite not found: {resourcePath}", this);
            return;
        }

        CreateSpriteProp(objectName, sprite, localPosition, localScale, sortingOrder, parent, addGrassSway);
    }

    private void CreateSpriteProp(string objectName, Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Transform parent = null, bool addGrassSway = false)
    {
        GameObject propObject = new GameObject(objectName);
        propObject.transform.SetParent(parent != null ? parent : contentRoot.transform, false);
        propObject.transform.localScale = localScale;

        SpriteRenderer spriteRenderer = propObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = sortingOrder;

        float groundedYOffset = sprite.bounds.extents.y * Mathf.Abs(localScale.y);
        propObject.transform.localPosition = localPosition + new Vector3(0f, groundedYOffset, 0f);
        Behaviour enableWhenRevealed = null;

        if (addGrassSway)
        {
            GrassSwayOnPlayerNear grassSway = propObject.AddComponent<GrassSwayOnPlayerNear>();
            grassSway.enabled = false;
            enableWhenRevealed = grassSway;
        }

        RegisterSpriteReveal(spriteRenderer, localPosition, enableWhenRevealed);
        CreatePropGroundStroke(objectName, localPosition, sprite.bounds.extents.x * Mathf.Abs(localScale.x), sortingOrder - 1, parent);
    }

    private void CreatePineTreeProp(string objectName, Vector3 localPosition, float scale, int sortingOrder, Transform parent = null)
    {
        SketchWorldLineDrawing pineTree = CreateLineDrawing(
            objectName,
            localPosition,
            BuildPineTreeStrokes(scale),
            sortingOrder,
            parent);
        PineTreeShakeInteract treeInteract = pineTree.gameObject.AddComponent<PineTreeShakeInteract>();
        treeInteract.Configure(scale, sortingOrder + 47);
        treeInteract.enabled = false;

        RegisterLineReveal(pineTree, treeInteract, localPosition);
        CreatePropGroundStroke(objectName, localPosition, 1.16f * scale, sortingOrder - 1, parent);
    }

    private void CreatePropGroundStroke(string objectName, Vector3 localPosition, float halfWidth, int sortingOrder, Transform parent)
    {
        float strokeHalfWidth = Mathf.Clamp(halfWidth * 0.72f, 0.45f, 2.2f);
        float unevenLift = Mathf.Clamp(strokeHalfWidth * 0.08f, 0.04f, 0.12f);
        List<Vector3[]> strokes = new List<Vector3[]>
        {
            Points(-strokeHalfWidth, 0f, -strokeHalfWidth * 0.35f, unevenLift, strokeHalfWidth * 0.28f, -unevenLift, strokeHalfWidth, 0f)
        };

        SketchWorldLineDrawing groundStroke = CreateLineDrawing(
            $"{objectName}_GroundStroke",
            localPosition,
            strokes,
            sortingOrder,
            parent);
        RegisterLineReveal(groundStroke, localPosition);
    }

    private static Sprite LoadSketchSprite(string resourcePath)
    {
        Sprite sprite = Resources.Load<Sprite>(resourcePath);
        if (sprite != null)
        {
            return sprite;
        }

        Texture2D texture = Resources.Load<Texture2D>(resourcePath);
        if (texture == null)
        {
            return null;
        }

        return Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f);
    }

    private Sprite GetWhiteSprite()
    {
        if (whiteSprite != null)
        {
            return whiteSprite;
        }

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        whiteSprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            1f);

        whiteSprite.name = "GeneratedWhiteOutsideSprite";
        return whiteSprite;
    }

    private static void SetPlayerMovementEnabled(GameObject player, bool enabled)
    {
        PlayerMove2D playerMove = player.GetComponent<PlayerMove2D>();
        if (playerMove != null)
        {
            if (!enabled)
            {
                playerMove.ForceIdleMotion();
            }

            playerMove.enabled = enabled;
        }

        Rigidbody2D rigidbody2D = player.GetComponent<Rigidbody2D>();
        if (rigidbody2D != null)
        {
            rigidbody2D.linearVelocity = Vector2.zero;
        }
    }

    private static void SnapCameraToPlayer(GameObject player)
    {
        CameraFollow2D cameraFollow = FindAnyObjectByType<CameraFollow2D>();
        if (cameraFollow != null)
        {
            cameraFollow.SnapToTarget();
            return;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = player.transform.position + new Vector3(0f, 0f, -10f);
        }
    }

    private static float ApplyHandDrawnPace(float progress)
    {
        float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
        float unevenPace = Mathf.Sin(progress * Mathf.PI * 7f) * 0.015f;
        float middleWeight = 1f - Mathf.Abs(progress * 2f - 1f);

        return Mathf.Clamp01(easedProgress + unevenPace * middleWeight);
    }

    private static Vector3[] BuildHorizontalFenceRevealPoints(float y, float halfWidth)
    {
        float width = halfWidth * 2f;
        int segmentCount = Mathf.Max(1, Mathf.CeilToInt(width / FenceRevealTriggerSpacing));
        Vector3[] points = new Vector3[segmentCount + 1];

        for (int i = 0; i <= segmentCount; i++)
        {
            float x = Mathf.Lerp(-halfWidth, halfWidth, (float)i / segmentCount);
            points[i] = new Vector3(x, y, 0f);
        }

        return points;
    }

    private static Vector3[] BuildVerticalFenceRevealPoints(float x, float halfHeight)
    {
        float height = halfHeight * 2f;
        int segmentCount = Mathf.Max(1, Mathf.CeilToInt(height / FenceRevealTriggerSpacing));
        Vector3[] points = new Vector3[segmentCount + 1];

        for (int i = 0; i <= segmentCount; i++)
        {
            float y = Mathf.Lerp(-halfHeight, halfHeight, (float)i / segmentCount);
            points[i] = new Vector3(x, y, 0f);
        }

        return points;
    }

    private static List<Vector3[]> BuildPondStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(EllipsePoints(0f, 0f, 5.25f, 2.85f, 42));
        strokes.Add(EllipsePoints(0.35f, 0.04f, 4.55f, 2.28f, 36));
        strokes.Add(Points(-3.55f, 0.52f, -2.75f, 0.72f, -1.95f, 0.54f, -1.14f, 0.7f));
        strokes.Add(Points(0.05f, -0.1f, 0.82f, 0.06f, 1.6f, -0.11f, 2.44f, 0.04f));
        strokes.Add(Points(-2.95f, -0.86f, -2.18f, -0.72f, -1.42f, -0.88f, -0.68f, -0.74f));
        strokes.Add(Points(2.5f, -0.88f, 3.12f, -0.72f, 3.8f, -0.88f));
        strokes.Add(Points(-5.0f, -0.1f, -5.58f, -0.36f, -5.16f, -0.72f));
        strokes.Add(Points(5.02f, 0.16f, 5.56f, -0.08f, 5.22f, -0.46f));

        return strokes;
    }

    private static List<Vector3[]> BuildFishingRodStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(0.92f, -1.26f, 0.36f, -0.38f, -0.28f, 0.5f, -0.9f, 1.28f, -1.42f, 1.92f));
        strokes.Add(Points(0.7f, -0.98f, 1.08f, -1.34f));
        strokes.Add(Points(0.52f, -0.72f, 0.92f, -1.12f));
        strokes.Add(Points(-1.42f, 1.92f, -1.72f, 0.78f, -1.54f, -0.16f, -1.82f, -1.04f));
        strokes.Add(Points(-1.82f, -1.04f, -1.62f, -1.25f, -1.82f, -1.43f));
        strokes.Add(Points(0.18f, -0.5f, 0.4f, -0.3f, 0.63f, -0.52f, 0.42f, -0.74f, 0.18f, -0.5f));
        strokes.Add(Points(0.12f, -1.44f, 0.84f, -1.3f, 1.5f, -1.44f));

        return strokes;
    }

    private static Vector3[] EllipsePoints(float centerX, float centerY, float radiusX, float radiusY, int segmentCount)
    {
        Vector3[] points = new Vector3[segmentCount + 1];

        for (int i = 0; i <= segmentCount; i++)
        {
            float angle = Mathf.PI * 2f * i / segmentCount;
            points[i] = new Vector3(
                centerX + Mathf.Cos(angle) * radiusX,
                centerY + Mathf.Sin(angle) * radiusY,
                0f);
        }

        return points;
    }

    private static Vector2[] EllipseColliderPoints(float radiusX, float radiusY, int segmentCount)
    {
        Vector2[] points = new Vector2[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            float angle = Mathf.PI * 2f * i / segmentCount;
            points[i] = new Vector2(
                Mathf.Cos(angle) * radiusX,
                Mathf.Sin(angle) * radiusY);
        }

        return points;
    }

    private static List<Vector3[]> BuildFenceTopStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        float left = -BackgroundSize.x * 0.5f + 0.45f;
        float right = BackgroundSize.x * 0.5f - 0.45f;
        float top = BackgroundSize.y * 0.5f - 0.45f;
        const float railGap = 0.36f;
        const float postLength = 0.95f;
        const float postStep = 2f;

        strokes.Add(Points(left, top, right, top));
        strokes.Add(Points(left, top - railGap, right, top - railGap));

        for (float x = left; x <= right + 0.01f; x += postStep)
        {
            strokes.Add(Points(x, top + 0.22f, x, top - postLength));
        }

        return strokes;
    }

    private static List<Vector3[]> BuildFenceBottomStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        float left = -BackgroundSize.x * 0.5f + 0.45f;
        float right = BackgroundSize.x * 0.5f - 0.45f;
        float bottom = -BackgroundSize.y * 0.5f + 0.45f;
        const float railGap = 0.36f;
        const float postLength = 0.95f;
        const float postStep = 2f;

        strokes.Add(Points(left, bottom, right, bottom));
        strokes.Add(Points(left, bottom + railGap, right, bottom + railGap));

        for (float x = left; x <= right + 0.01f; x += postStep)
        {
            strokes.Add(Points(x, bottom - 0.22f, x, bottom + postLength));
        }

        return strokes;
    }

    private static List<Vector3[]> BuildFenceLeftStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        float left = -BackgroundSize.x * 0.5f + 0.45f;
        float bottom = -BackgroundSize.y * 0.5f + 0.45f;
        float top = BackgroundSize.y * 0.5f - 0.45f;
        const float railGap = 0.36f;
        const float postLength = 0.95f;
        const float postStep = 2f;

        strokes.Add(Points(left, bottom, left, top));
        strokes.Add(Points(left + railGap, bottom, left + railGap, top));

        for (float y = bottom; y <= top + 0.01f; y += postStep)
        {
            strokes.Add(Points(left - 0.22f, y, left + postLength, y));
        }

        return strokes;
    }

    private static List<Vector3[]> BuildFenceRightStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        float right = BackgroundSize.x * 0.5f - 0.45f;
        float bottom = -BackgroundSize.y * 0.5f + 0.45f;
        float top = BackgroundSize.y * 0.5f - 0.45f;
        const float railGap = 0.36f;
        const float postLength = 0.95f;
        const float postStep = 2f;

        strokes.Add(Points(right, bottom, right, top));
        strokes.Add(Points(right - railGap, bottom, right - railGap, top));

        for (float y = bottom; y <= top + 0.01f; y += postStep)
        {
            strokes.Add(Points(right + 0.22f, y, right - postLength, y));
        }

        return strokes;
    }

    private static List<Vector3[]> BuildForestEntranceStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(-3.25f, -1.38f, -2.3f, -1.22f, -1.38f, -1.34f, -0.48f, -1.18f, 0.42f, -1.34f, 1.35f, -1.2f, 2.28f, -1.36f, 3.16f, -1.24f));
        strokes.Add(Points(-2.95f, 0.58f, -2.42f, 1.12f, -1.98f, 0.74f, -1.52f, 1.32f, -1.02f, 0.78f, -0.48f, 1.46f, 0.12f, 0.86f, 0.72f, 1.38f, 1.28f, 0.76f, 1.88f, 1.2f, 2.46f, 0.64f, 3.02f, 0.98f));
        AddOutsidePine(strokes, -2.5f, -1.36f, 0.82f);
        AddOutsidePine(strokes, -1.42f, -1.34f, 1.02f);
        AddOutsidePine(strokes, -0.24f, -1.38f, 0.9f);
        AddOutsidePine(strokes, 1.0f, -1.34f, 1.14f);
        AddOutsidePine(strokes, 2.35f, -1.36f, 0.86f);
        strokes.Add(Points(-0.66f, -1.4f, -0.42f, -0.78f, -0.18f, -0.34f, 0.14f, -0.04f));
        strokes.Add(Points(0.76f, -1.4f, 0.52f, -0.78f, 0.26f, -0.34f, 0f, -0.04f));
        strokes.Add(Points(-2.82f, -0.12f, -2.22f, -0.42f, -1.55f, -0.28f, -0.88f, -0.5f, -0.2f, -0.24f, 0.48f, -0.46f, 1.16f, -0.18f, 1.84f, -0.4f, 2.54f, -0.1f));

        return strokes;
    }

    private static List<Vector3[]> BuildQuestNpcStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(-0.58f, -0.5f, -0.34f, -0.56f, -0.08f, -0.52f, 0.18f, -0.56f, 0.48f, -0.48f));
        strokes.Add(Points(-0.3f, -0.5f, -0.18f, 0.05f, -0.34f, 0.52f, -0.22f, 1.18f));
        strokes.Add(Points(0.3f, -0.5f, 0.18f, 0.05f, 0.38f, 0.52f, 0.24f, 1.18f));
        strokes.Add(Points(-0.42f, 1.14f, -0.52f, 0.56f, -0.28f, 0.08f, 0.26f, 0.06f, 0.54f, 0.56f, 0.42f, 1.14f, -0.42f, 1.14f));
        strokes.Add(Points(-0.38f, 0.9f, -0.76f, 0.62f, -0.62f, 0.32f));
        strokes.Add(Points(0.4f, 0.88f, 0.74f, 0.62f, 0.82f, 0.06f));
        strokes.Add(Points(0.82f, 0.06f, 0.92f, -0.32f));
        strokes.Add(Points(-0.27f, 1.56f, -0.14f, 1.3f, 0.14f, 1.28f, 0.31f, 1.48f, 0.24f, 1.72f, 0f, 1.84f, -0.24f, 1.72f, -0.27f, 1.56f));
        strokes.Add(Points(-0.42f, 1.72f, -0.14f, 1.96f, 0.2f, 1.9f, 0.42f, 1.66f));
        strokes.Add(Points(-0.16f, 1.48f, -0.02f, 1.42f, 0.14f, 1.48f));
        strokes.Add(Points(-0.18f, 1.36f, 0.16f, 1.34f));
        strokes.Add(Points(-0.32f, 0.46f, 0.3f, 0.46f));

        return strokes;
    }

    private static List<Vector3[]> BuildPineTreeStrokes(float scale)
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        AddOutsidePine(strokes, 0f, 0f, scale);
        return strokes;
    }

    private static void AddOutsidePine(List<Vector3[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(PinePoints(centerX, groundY, scale, 0f, 2.48f, -0.7f, 1.62f, -0.42f, 1.62f, -0.92f, 0.86f, -0.5f, 0.86f, -1.16f, 0f, 1.16f, 0f, 0.5f, 0.86f, 0.92f, 0.86f, 0.42f, 1.62f, 0.7f, 1.62f, 0f, 2.48f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.14f, 0f, -0.14f, -0.34f, 0.14f, -0.34f, 0.14f, 0f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.5f, 0.86f, -0.16f, 1.22f, 0.16f, 0.86f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.42f, 1.62f, -0.12f, 1.92f, 0.2f, 1.62f));
    }

    private static Vector3[] PinePoints(float centerX, float groundY, float scale, params float[] values)
    {
        int pointCount = values.Length / 2;
        Vector3[] points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector3(
                centerX + values[i * 2] * scale,
                groundY + values[i * 2 + 1] * scale,
                0f);
        }

        return points;
    }

    private static List<Vector3[]> BuildHouseStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(-2.6f, -1.2f, -1.6f, -1.16f, -0.6f, -1.22f, 0.4f, -1.16f, 1.5f, -1.22f, 2.6f, -1.16f));
        strokes.Add(Points(-2.2f, -1.1f, -2.2f, 0.65f, 2.2f, 0.65f, 2.2f, -1.1f));
        strokes.Add(Points(-2.55f, 0.62f, 0f, 2.05f, 2.55f, 0.62f));
        strokes.Add(Points(-1.65f, 1.05f, -1.65f, 1.55f, -1.2f, 1.55f, -1.2f, 1.3f));
        strokes.Add(Points(-0.35f, -1.1f, -0.35f, -0.2f, 0.35f, -0.2f, 0.35f, -1.1f));
        strokes.Add(Points(0.23f, -0.66f, 0.27f, -0.66f));
        strokes.Add(Points(-1.55f, -0.35f, -0.85f, -0.35f, -0.85f, 0.2f, -1.55f, 0.2f, -1.55f, -0.35f));
        strokes.Add(Points(0.85f, -0.35f, 1.55f, -0.35f, 1.55f, 0.2f, 0.85f, 0.2f, 0.85f, -0.35f));
        strokes.Add(Points(-1.2f, -0.35f, -1.2f, 0.2f));
        strokes.Add(Points(1.2f, -0.35f, 1.2f, 0.2f));

        return strokes;
    }

    private static List<Vector3[]> BuildNeighborHouseStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(-1.9f, -1.05f, -1.9f, 0.35f, 1.9f, 0.35f, 1.9f, -1.05f));
        strokes.Add(Points(-2.25f, 0.32f, 0f, 1.58f, 2.25f, 0.32f));
        strokes.Add(Points(-1.42f, 0.77f, -1.42f, 1.22f, -1.02f, 1.22f, -1.02f, 0.98f));
        strokes.Add(Points(-0.32f, -1.05f, -0.32f, -0.23f, 0.32f, -0.23f, 0.32f, -1.05f));
        strokes.Add(Points(0.2f, -0.66f, 0.25f, -0.66f));
        strokes.Add(Points(-1.35f, -0.35f, -0.78f, -0.35f, -0.78f, 0.1f, -1.35f, 0.1f, -1.35f, -0.35f));
        strokes.Add(Points(0.82f, -0.35f, 1.4f, -0.35f, 1.4f, 0.1f, 0.82f, 0.1f, 0.82f, -0.35f));
        strokes.Add(Points(-2.15f, -1.08f, -1.2f, -1.02f, -0.28f, -1.08f, 0.74f, -1.02f, 1.92f, -1.08f));

        return strokes;
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
