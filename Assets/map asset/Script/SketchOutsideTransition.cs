using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private const float OutsideCameraSizeMultiplier = 1.5f;
    private const int BackgroundSortingOrder = -20;
    private const int PropSortingOrder = 20;
    private const int PathSortingOrder = 5;
    private const int GrassSortingOrder = PropSortingOrder + 1;
    private const int TreeSortingOrder = PropSortingOrder + 3;
    private const int MailboxSortingOrder = PropSortingOrder + 4;
    private const int QuestNpcSortingOrder = MailboxSortingOrder + 1;
    private const int DecorationRandomSeed = 20260520;
    private const int TreeSpawnCount = 28;
    private const int GrassSpawnCount = 170;
    private const int DecorationPickAttempts = 2200;
    private const float BoundaryColliderThickness = 0.55f;
    private const float MailboxHouseThirdScale = 0.52f;
    private const float DecorationOverlapPadding = 0.28f;
    private const float QuestNpcPromptDistance = 2.3f;
    private const float ForestBirdPromptDistance = 2.15f;
    private const float ForestWellPromptDistance = 1.9f;
    private const float MoleHolePromptDistance = 1.45f;
    private const float ForestSquirrelPromptDistance = 1.85f;
    private const float HungrySquirrelPromptDistance = 1.85f;
    private const float AcornPromptDistance = 1.35f;
    private const float DeepForestLeafPilePromptDistance = 1.35f;
    private const float FourthForestBigTreePromptDistance = 6.2f;
    private const float FourthForestRiverPromptDistance = 3.6f;
    private const float QuestNpcPromptPulseSpeed = 4.4f;
    private const float PathRevealDistance = 7.2f;
    private const float PathDecorationClearance = 1.15f;
    private const float ForestGateHalfHeight = 3.2f;
    private const float OutsideMapEdgeFadeWorldWidth = 7.5f;
    private const float OuterDecorativeTreeSpacing = 2.65f;
    private const float OuterDecorativeTreeBandWidth = 7.6f;
    private const int OuterDecorativeTreeCountMultiplier = 3;
    private const float OvalFenceReferenceRadiusX = 21.9f;
    private const float OvalFenceReferenceRadiusY = 17.71f;
    private const float OvalFenceTargetRadiusX = 42f;
    private const int OvalFenceRailResolution = 160;
    private const int OvalFencePostCount = 72;
    private const int OvalFenceColliderSegmentCount = 96;
    private const float OvalFenceRailGap = 0.56f;
    private const float OvalFenceOuterRailInset = 0.34f;
    private const float OvalFencePostHalfWidth = 0.16f;
    private const float OvalFencePostLength = 1.18f;
    private const float OvalFenceColliderOuterOutset = 0.34f;
    private const float OvalFenceColliderInnerInset = OvalFencePostLength + 0.16f;
    private const float OuterDecorativeTreeRevealDistance = 10.5f;
    private const int OvalFillPineCount = 30;
    private const int OvalFillGrassCount = 90;
    private const string BackgroundEdgeFadeShaderResource = "OutsideMapEdgeFade";
    private const int ForestExtensionTreeCount = 64;
    private const int DeepForestExtensionTreeCount = 74;
    private const int FourthForestExtensionTreeCount = 118;
    private const int DeepForestLeafPileCount = 10;
    private const int DeepForestMushroomLeafPileCount = 3;
    private const int ForestBranchTreePlacementIndex = 7;
    private const float LetterFragmentRevealDistance = 5.7f;
    private const float LetterFragmentPromptDistance = 1.45f;
    private const float ForestBranchTreePromptDistance = 2.1f;
    private const float ForestRockPromptDistance = 1.65f;
    private const int LetterFragmentSortingOrder = PropSortingOrder + 6;
    private const int LetterFragmentPromptSortingOrder = PropSortingOrder + 52;
    private const int QuestGuideSortingOrder = PropSortingOrder + 7;
    private const int ForestWellSortingOrder = TreeSortingOrder + 1;
    private const int ForestBirdSortingOrder = TreeSortingOrder + 4;
    private const int ForestSquirrelSortingOrder = TreeSortingOrder + 5;
    private const int HungrySquirrelSortingOrder = ForestSquirrelSortingOrder + 1;
    private const int AcornSortingOrder = PropSortingOrder + 4;
    private const int DeepForestLeafPileSortingOrder = PropSortingOrder + 2;
    private const int MoleHoleSortingOrder = PropSortingOrder + 2;
    private const int ForestBranchPromptSortingOrder = TreeSortingOrder + 48;
    private const int MoleHolePromptSortingOrder = MoleHoleSortingOrder + 48;
    private const int MoleSortingOrder = ForestBirdSortingOrder + 1;
    private const int ForestBirdPromptSortingOrder = ForestBirdSortingOrder + 45;
    private const int ForestWellPromptSortingOrder = ForestWellSortingOrder + 45;
    private const int ForestSquirrelPromptSortingOrder = ForestSquirrelSortingOrder + 45;
    private const int HungrySquirrelPromptSortingOrder = HungrySquirrelSortingOrder + 45;
    private const int AcornPromptSortingOrder = AcornSortingOrder + 45;
    private const int DeepForestLeafPilePromptSortingOrder = DeepForestLeafPileSortingOrder + 48;
    private const int FourthForestBigTreePromptSortingOrder = TreeSortingOrder + 54;
    private const int FourthForestRiverSortingOrder = PathSortingOrder + 4;
    private const int FourthForestRootBridgeSortingOrder = FourthForestRiverSortingOrder + 3;
    private const int FourthForestIslandDoorSortingOrder = FourthForestRootBridgeSortingOrder + 4;
    private const int ForestRockPromptSortingOrder = PropSortingOrder + 50;
    private const string QuestNpcSearchingPrompt = "[\uD3B8\uC9C0]+[?]";
    private const string QuestNpcCompletePrompt = "[\uD3B8\uC9C0]+[!]";
    private const string GrassResourcePath = "Outside/grass_thick";
    private const string MailboxResourcePath = "Outside/mailbox_thick";
    private static readonly Color VillageGreenColor = new Color32(69, 158, 72, 255);
    private static readonly Color VillageGrassLightGreenColor = new Color32(151, 214, 83, 255);
    private static readonly Color BrownCrayonColor = new Color32(139, 82, 39, 232);
    private static readonly Color BlueCrayonColor = new Color32(47, 135, 218, 236);
    private static readonly Dictionary<Sprite, Sprite> GrassColorOverlaySpriteCache = new Dictionary<Sprite, Sprite>();
    private static readonly int EdgeFadeXPropertyId = Shader.PropertyToID("_EdgeFadeX");
    private static readonly int EdgeFadeYPropertyId = Shader.PropertyToID("_EdgeFadeY");
    private static readonly int EdgeFadeProtectedLeftRatioPropertyId = Shader.PropertyToID("_ProtectedLeftRatio");
    private static readonly Vector2 GrassScaleRange = new Vector2(0.5508f, 0.8424f);
    private static readonly Vector2 TreeScaleRange = new Vector2(1.12f, 1.32f);
    private static readonly Vector3 OutsidePlayerOffset = Vector3.zero;
    private static readonly Vector3 HouseOffset = new Vector3(0f, 2f, 0f);
    private static readonly Vector3 DoorOffset = new Vector3(0f, 1.35f, 0f);
    private static readonly Vector3 NeighborHouseOffset = new Vector3(18.4f, 3.1f, 0f);
    private static readonly Vector3 ForestEntranceOffset = new Vector3(21.5f, -14.2f, 0f);
    private static readonly Vector3 PondOffset = new Vector3(-23.5f, -13.6f, 0f);
    private static readonly Vector3 WindLetterFragmentOffset = new Vector3(-13.4f, 12.2f, 0f);
    private static readonly Vector3 GrassLetterFragmentOffset = new Vector3(8.8f, -7.8f, 0f);
    private static readonly Vector3 PondLetterFragmentOffset = new Vector3(-14.7f, -19.2f, 0f);
    private static readonly Vector3 QuestNpcLetterGrassOffset = new Vector3(-2.05f, -1.35f, 0f);
    private static readonly Vector3 MapSlideStartOffset = new Vector3(0f, -1.4f, 0f);
    private static readonly Vector2 BackgroundSize = new Vector2(70f, 48f);
    private static readonly Vector2 OutsideOvalFenceRadius = CalculateOutsideOvalFenceRadius();
    private static readonly Vector2 DeepForestSize = new Vector2(BackgroundSize.x * 0.5f, BackgroundSize.y);
    private static readonly Vector2 FourthForestSize = BackgroundSize;
    private static readonly Vector3 ForestRegionOffset = new Vector3(BackgroundSize.x, 0f, 0f);
    private static readonly Vector3 DeepForestRegionOffset = new Vector3(BackgroundSize.x * 1.75f, 0f, 0f);
    private static readonly Vector3 FourthForestRegionOffset = DeepForestRegionOffset + new Vector3((DeepForestSize.x + BackgroundSize.x) * 0.5f, 0f, 0f);
    private static readonly Vector3 DeepForestEntranceDirtOffset = DeepForestRegionOffset + new Vector3(-DeepForestSize.x * 0.5f + 3.6f, 0f, 0f);
    private static readonly Vector3 DeepForestSquirrelOffset = DeepForestEntranceDirtOffset + new Vector3(5.8f, 0.35f, 0f);
    private static readonly Vector3 DeepForestHungrySquirrelOffset = DeepForestRegionOffset + new Vector3(7.8f, -5.8f, 0f);
    private static readonly Vector3 FourthForestBigTreeOffset = FourthForestRegionOffset + new Vector3(0f, -3.2f, 0f);
    private static readonly Vector3 FourthForestRiverOffset = FourthForestRegionOffset + new Vector3(26.6f, -15.2f, 0f);
    private static readonly Vector3 FourthForestIslandDoorOffset = FourthForestRiverOffset;
    private static readonly Vector3[] DeepForestShakingBushOffsets =
    {
        DeepForestRegionOffset + new Vector3(10.2f, 11.4f, 0f),
        DeepForestRegionOffset + new Vector3(5.8f, 16.2f, 0f),
        DeepForestRegionOffset + new Vector3(14.1f, 5.2f, 0f),
        DeepForestRegionOffset + new Vector3(13.4f, -10.8f, 0f),
        DeepForestRegionOffset + new Vector3(0.8f, -14.6f, 0f)
    };
    private static readonly Vector3 ForestSmallBirdOffset = ForestRegionOffset + new Vector3(0f, 0f, 0f);
    private static readonly Vector3 ForestFallenBirdOffset = ForestRegionOffset + new Vector3(-27.8f, -17.6f, 0f);
    private static readonly Vector3 ForestWellOffset = ForestRegionOffset + new Vector3(10.6f, -7.8f, 0f);
    private static readonly Vector3 MoleRewardOffset = ForestRegionOffset + new Vector3(-7.2f, 8.8f, 0f);
    private static readonly Vector3[] ForestRockOffsets =
    {
        ForestRegionOffset + new Vector3(-30.4f, 3.8f, 0f),
        ForestRegionOffset + new Vector3(-22.8f, -16.4f, 0f),
        ForestRegionOffset + new Vector3(-4.8f, 14.2f, 0f),
        ForestRegionOffset + new Vector3(13.4f, 7.8f, 0f),
        ForestRegionOffset + new Vector3(24.8f, -14.6f, 0f)
    };
    private const int ForestRewardRockIndex = 3;
    private static readonly Vector3[] ForestMoleHoleOffsets =
    {
        ForestRegionOffset + new Vector3(-18.4f, 12.4f, 0f),
        ForestRegionOffset + new Vector3(-7.2f, 8.8f, 0f),
        ForestRegionOffset + new Vector3(7.6f, 12.0f, 0f),
        ForestRegionOffset + new Vector3(17.8f, -5.4f, 0f),
        ForestRegionOffset + new Vector3(-16.2f, -10.4f, 0f)
    };
    private static readonly Vector2 DecorationMin = new Vector2(-33f, -21f);
    private static readonly Vector2 DecorationMax = new Vector2(33f, 21f);
    private static readonly Vector3[][] OutsidePathCenterLines =
    {
        new[]
        {
            new Vector3(2.2f, 1.3f, 0f),
            new Vector3(7.4f, 3.7f, 0f),
            new Vector3(12.4f, 4.0f, 0f),
            new Vector3(17.2f, 3.0f, 0f)
        },
        new[]
        {
            new Vector3(-1.6f, 0.1f, 0f),
            new Vector3(-7.8f, -3.8f, 0f),
            new Vector3(-14.8f, -8.4f, 0f),
            new Vector3(-18.4f, -11.75f, 0f)
        },
        new[]
        {
            new Vector3(-1.2f, 0.4f, 0f),
            new Vector3(5.8f, -1.2f, 0f),
            new Vector3(11.8f, -5.7f, 0f),
            new Vector3(17.6f, -10.8f, 0f),
            new Vector3(21.5f, -14.2f, 0f)
        },
        new[]
        {
            new Vector3(11.8f, -5.7f, 0f),
            new Vector3(16.6f, -6.1f, 0f),
            new Vector3(22.8f, -4.8f, 0f),
            new Vector3(29.0f, -2.2f, 0f),
            new Vector3(35.0f, 0f, 0f)
        }
    };

    private static SketchOutsideTransition instance;
    private static bool hasStoredInteriorCameraSize;
    private static float interiorCameraOrthographicSize;

    private GameObject contentRoot;
    private GameObject propRoot;
    private GameObject questNpcPromptObject;
    private TextMesh questNpcPromptText;
    private TextMesh questNpcPromptInnerGlowText;
    private TextMesh questNpcPromptOuterGlowText;
    private SpriteRenderer backgroundRenderer;
    private SketchWorldLineDrawing houseDrawing;
    private SketchWorldLineDrawing questNpcDrawing;
    private SketchWorldLineDrawing questNpcPondGuideDrawing;
    private SketchWorldLineDrawing rightFenceDrawing;
    private QuestNpcLetterRewardInteract questNpcRewardInteract;
    private SketchOutsideDoorInteract houseDoorInteract;
    private BoxCollider2D houseDoorCollider;
    private BoxCollider2D forestRightBoundaryCollider;
    private BoxCollider2D deepForestRightBoundaryCollider;
    private readonly List<Collider2D> villageGateBoundaryColliders = new List<Collider2D>();
    private readonly List<MapRevealItem> mapRevealItems = new List<MapRevealItem>();
    private Transform boundaryRoot;
    private Transform outsidePlayer;
    private Coroutine transitionRoutine;
    private Coroutine questNpcPondGuideRoutine;
    private Sprite whiteSprite;
    private Material backgroundEdgeFadeMaterial;
    private bool hasCompletedOutsideMap;
    private bool isOutsideActive;
    private bool hasCreatedForestExtension;
    private bool hasCreatedDeepForestExtension;
    private bool hasCreatedFourthForestExtension;
    private bool hasCreatedVillageForestGateColliders;
    private bool hasAppliedVillageGreenColoring;

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

    public static void ApplySketchbookForestUnlock()
    {
        SketchOutsideTransition transition = GetOrCreateInstance();
        transition.EnsureSetup();
        transition.EnsureForestExtensionCreated();
    }

    public static void ApplySketchbookDeepForestUnlock()
    {
        SketchOutsideTransition transition = GetOrCreateInstance();
        transition.EnsureSetup();
        transition.EnsureDeepForestExtensionCreated();
    }

    public static void ApplySketchbookFourthForestUnlock()
    {
        SketchOutsideTransition transition = GetOrCreateInstance();
        transition.EnsureSetup();
        transition.EnsureFourthForestExtensionCreated();
    }

    public static void ApplySketchbookVillageGreenColoring()
    {
        SketchOutsideTransition transition = GetOrCreateInstance();
        transition.EnsureSetup();
        transition.EnsureVillageGreenColoringApplied();
    }

    public static void ApplySketchbookBrownColoring()
    {
        SketchOutsideTransition transition = GetOrCreateInstance();
        transition.EnsureSetup();
        transition.EnsureBrownColoringApplied();
    }

    public static void ApplySketchbookBlueWaterColoring()
    {
        SketchOutsideTransition transition = GetOrCreateInstance();
        transition.EnsureSetup();
        transition.EnsureBlueWaterColoringApplied();
    }

    public static IEnumerator PlayFourthForestRootBridgeFromBigTree()
    {
        SketchOutsideTransition transition = GetOrCreateInstance();
        transition.EnsureSetup();
        yield return transition.PlayFourthForestRootBridgeRoutine();
    }

    public static Vector3 GetPreferredGlowingLeafTreeLocalPosition()
    {
        return FourthForestRegionOffset + new Vector3(-FourthForestSize.x * 0.28f, FourthForestSize.y * 0.28f, 0f);
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
        RestoreInteriorCameraSize();
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
        ApplyOutsideCameraSize();
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
        backgroundRenderer.transform.localPosition = GetBackgroundLocalPosition();
        backgroundRenderer.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
        UpdateBackgroundEdgeFade();
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
        EnsureForestExtensionCreated();
        backgroundRenderer.transform.localPosition = GetBackgroundLocalPosition();
        backgroundRenderer.transform.localScale = GetBackgroundFinalScale();
        UpdateBackgroundEdgeFade();
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

    private void RegisterLineReveal(SketchWorldLineDrawing lineDrawing, float revealDistance, Behaviour enableWhenRevealed, params Vector3[] localTriggerPositions)
    {
        if (lineDrawing == null)
        {
            return;
        }

        MapRevealItem item = new MapRevealItem(localTriggerPositions, revealDistance, lineDrawing, enableWhenRevealed);
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
        bool canShowPrompt = !GameProgress.HasDeliveredCompletedLetter;

        SetQuestNpcPromptText(GameProgress.HasCompletedLetter ? QuestNpcCompletePrompt : QuestNpcSearchingPrompt);
        SetQuestNpcPromptVisible(isOutsideActive && isNpcVisible && isNearNpc && canShowPrompt);
        UpdateQuestNpcPromptGlow();
    }

    public void PlayQuestNpcPondGuide()
    {
        if (questNpcPondGuideDrawing == null)
        {
            return;
        }

        if (questNpcPondGuideRoutine != null)
        {
            StopCoroutine(questNpcPondGuideRoutine);
        }

        questNpcPondGuideRoutine = StartCoroutine(PlayQuestNpcPondGuideRoutine());
    }

    private IEnumerator PlayQuestNpcPondGuideRoutine()
    {
        const float drawDuration = 0.34f;
        const float holdDuration = 0.62f;
        const float eraseDuration = 0.36f;
        float elapsed = 0f;

        questNpcPondGuideDrawing.RevealProgress = 0f;

        while (elapsed < drawDuration)
        {
            elapsed += Time.deltaTime;
            questNpcPondGuideDrawing.RevealProgress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / drawDuration));
            yield return null;
        }

        questNpcPondGuideDrawing.RevealProgress = 1f;
        yield return new WaitForSeconds(holdDuration);

        elapsed = 0f;
        while (elapsed < eraseDuration)
        {
            elapsed += Time.deltaTime;
            questNpcPondGuideDrawing.RevealProgress = Mathf.SmoothStep(1f, 0f, Mathf.Clamp01(elapsed / eraseDuration));
            yield return null;
        }

        questNpcPondGuideDrawing.RevealProgress = 0f;
        questNpcPondGuideRoutine = null;
    }

    private void SetQuestNpcPromptText(string promptText)
    {
        if (questNpcPromptText != null)
        {
            questNpcPromptText.text = promptText;
        }

        if (questNpcPromptInnerGlowText != null)
        {
            questNpcPromptInnerGlowText.text = promptText;
        }

        if (questNpcPromptOuterGlowText != null)
        {
            questNpcPromptOuterGlowText.text = promptText;
        }
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
        Vector3 finalScale = GetBackgroundFinalScale();
        backgroundRenderer.transform.localPosition = GetBackgroundLocalPosition();

        while (elapsed < BackgroundRevealDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / BackgroundRevealDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            backgroundRenderer.transform.localScale = new Vector3(
                Mathf.Lerp(0.01f, finalScale.x, easedProgress),
                Mathf.Lerp(0.01f, finalScale.y, easedProgress),
                1f);
            UpdateBackgroundEdgeFade();

            yield return null;
        }

        backgroundRenderer.transform.localScale = finalScale;
        UpdateBackgroundEdgeFade();
    }

    private static Vector3 GetBackgroundLocalPosition()
    {
        return GetBackgroundFinalScale().x > BackgroundSize.x
            ? new Vector3((GetBackgroundFinalScale().x - BackgroundSize.x) * 0.5f, 0f, 0f)
            : Vector3.zero;
    }

    private static Vector3 GetBackgroundFinalScale()
    {
        float width = BackgroundSize.x;
        if (GameProgress.HasDrawnFourthForestSketch)
        {
            width = BackgroundSize.x * 2f + DeepForestSize.x + FourthForestSize.x;
        }
        else if (GameProgress.HasDrawnDeepForestSketch)
        {
            width = BackgroundSize.x * 2f + DeepForestSize.x;
        }
        else if (GameProgress.HasDrawnForestSketch)
        {
            width = BackgroundSize.x * 2f;
        }

        return new Vector3(
            width,
            BackgroundSize.y,
            1f);
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
            EnsureForestExtensionCreated();
            RemoveExtensionFenceLinework();
            EnsureDeepForestState();
            EnsureFourthForestState();
            EnsureVillageGreenColoringApplied();
            EnsureBrownColoringApplied();
            EnsureBlueWaterColoringApplied();
            return;
        }

        contentRoot = new GameObject("GeneratedSketchOutsideContent");
        contentRoot.transform.SetParent(transform, false);

        backgroundRenderer = CreateBackground();
        houseDrawing = CreateHouseDrawing();
        houseDoorInteract = CreateHouseDoor();
        CreateOutsideProps();
        EnsureForestExtensionCreated();
        RemoveExtensionFenceLinework();
        EnsureDeepForestState();
        EnsureFourthForestState();
        EnsureVillageGreenColoringApplied();
        EnsureBrownColoringApplied();
        EnsureBlueWaterColoringApplied();
        contentRoot.SetActive(false);
    }

    private void EnsureDeepForestState()
    {
        if (GameProgress.HasDrawnDeepForestSketch)
        {
            DestroyGeneratedChildrenWithPrefix(boundaryRoot, "GeneratedForestDeepBoundary");
            EnsureDeepForestExtensionCreated();
        }
        else
        {
            RemoveDeepForestExtension();
        }
    }

    private void EnsureFourthForestState()
    {
        if (GameProgress.HasDrawnFourthForestSketch)
        {
            EnsureFourthForestExtensionCreated();
        }
        else
        {
            RemoveFourthForestExtension();
        }
    }

    private void RemoveDeepForestExtension()
    {
        DestroyGeneratedChildrenWithPrefix(propRoot != null ? propRoot.transform : null, "GeneratedDeepForest");
        DestroyGeneratedChildrenWithPrefix(propRoot != null ? propRoot.transform : null, "GeneratedGrassDeepForest");
        DestroyGeneratedChildrenWithPrefix(propRoot != null ? propRoot.transform : null, "GeneratedOuterDecorativePine_DeepForest");
        DestroyGeneratedChildrenWithPrefix(boundaryRoot, "GeneratedDeepForest");
        DestroyGeneratedChildrenWithPrefix(boundaryRoot, "GeneratedForestDeepBoundary");

        hasCreatedDeepForestExtension = false;

        if (forestRightBoundaryCollider != null)
        {
            forestRightBoundaryCollider.enabled = true;
        }

        if (deepForestRightBoundaryCollider != null)
        {
            deepForestRightBoundaryCollider.enabled = true;
        }

        RemoveExtensionFenceLinework();
    }

    private void RemoveFourthForestExtension()
    {
        GlowingLeafQuestRoute.ResetRoute();
        DestroyGeneratedChildrenWithPrefix(propRoot != null ? propRoot.transform : null, "GeneratedFourthForest");
        DestroyGeneratedChildrenWithPrefix(propRoot != null ? propRoot.transform : null, "GeneratedGrassFourthForest");
        DestroyGeneratedChildrenWithPrefix(propRoot != null ? propRoot.transform : null, "GeneratedOuterDecorativePine_FourthForest");
        DestroyGeneratedChildrenWithPrefix(boundaryRoot, "GeneratedFourthForest");

        hasCreatedFourthForestExtension = false;

        if (deepForestRightBoundaryCollider != null)
        {
            deepForestRightBoundaryCollider.enabled = true;
        }

        RemoveExtensionFenceLinework();
    }

    private static void DestroyGeneratedChildrenWithPrefix(Transform parent, string namePrefix)
    {
        if (parent == null)
        {
            return;
        }

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child == null || !child.name.StartsWith(namePrefix))
            {
                continue;
            }

            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    private static void DestroyGeneratedChild(Transform parent, string objectName)
    {
        if (parent == null)
        {
            return;
        }

        Transform child = parent.Find(objectName);
        if (child == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(child.gameObject);
        }
        else
        {
            DestroyImmediate(child.gameObject);
        }
    }

    private void RemoveExtensionFenceLinework()
    {
        Transform parent = propRoot != null ? propRoot.transform : null;
        DestroyGeneratedChild(parent, "GeneratedForestExtensionFence");
        DestroyGeneratedChild(parent, "GeneratedForestExtensionFence_BrownOverlay");
        DestroyGeneratedChild(parent, "GeneratedDeepForestExtensionFence");
        DestroyGeneratedChild(parent, "GeneratedDeepForestExtensionFence_BrownOverlay");
        DestroyGeneratedChild(parent, "GeneratedFourthForestExtensionFence");
        DestroyGeneratedChild(parent, "GeneratedFourthForestExtensionFence_BrownOverlay");
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
        ApplyBackgroundEdgeFadeMaterial(spriteRenderer);
        return spriteRenderer;
    }

    private void ApplyBackgroundEdgeFadeMaterial(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        Shader shader = Resources.Load<Shader>(BackgroundEdgeFadeShaderResource);
        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        if (shader == null)
        {
            return;
        }

        backgroundEdgeFadeMaterial = new Material(shader)
        {
            name = "GeneratedOutsideMapEdgeFadeMaterial"
        };
        spriteRenderer.sharedMaterial = backgroundEdgeFadeMaterial;
        UpdateBackgroundEdgeFade();
    }

    private void UpdateBackgroundEdgeFade()
    {
        if (backgroundRenderer == null || backgroundEdgeFadeMaterial == null)
        {
            return;
        }

        Vector3 scale = backgroundRenderer.transform.localScale;
        float fadeX = Mathf.Clamp01(OutsideMapEdgeFadeWorldWidth / Mathf.Max(0.01f, Mathf.Abs(scale.x)));
        float fadeY = Mathf.Clamp01(OutsideMapEdgeFadeWorldWidth / Mathf.Max(0.01f, Mathf.Abs(scale.y)));
        float protectedLeftRatio = Mathf.Clamp01(BackgroundSize.x / Mathf.Max(BackgroundSize.x, Mathf.Abs(scale.x)));

        backgroundEdgeFadeMaterial.SetFloat(EdgeFadeXPropertyId, fadeX);
        backgroundEdgeFadeMaterial.SetFloat(EdgeFadeYPropertyId, fadeY);
        backgroundEdgeFadeMaterial.SetFloat(EdgeFadeProtectedLeftRatioPropertyId, protectedLeftRatio);
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
        CreateOutsidePaths();

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
        CreateQuestNpcPondGuide();
        CreateRandomOutsideDecorations();
        propRoot.SetActive(false);
    }

    private void EnsureForestExtensionCreated()
    {
        if (!GameProgress.HasDrawnForestSketch || hasCreatedForestExtension || propRoot == null)
        {
            return;
        }

        hasCreatedForestExtension = true;

        if (backgroundRenderer != null)
        {
            backgroundRenderer.transform.localPosition = GetBackgroundLocalPosition();

            if (hasCompletedOutsideMap)
            {
                backgroundRenderer.transform.localScale = GetBackgroundFinalScale();
                UpdateBackgroundEdgeFade();
            }
        }

        if (rightFenceDrawing != null)
        {
            rightFenceDrawing.SetStrokes(BuildFenceRightGateStrokes());
        }

        SetCollidersEnabled(villageGateBoundaryColliders, false);

        CreateVillageForestGateColliders();
        CreateForestExtensionBoundary();
        CreateForestExtensionPath();
        CreateForestExtensionQuestObjects();
        CreateForestExtensionDecorations();
        EnsureBrownColoringApplied();
    }

    private void EnsureDeepForestExtensionCreated()
    {
        if (GameProgress.HasDrawnDeepForestSketch)
        {
            DestroyGeneratedChildrenWithPrefix(boundaryRoot, "GeneratedForestDeepBoundary");
        }

        if (!GameProgress.HasDrawnDeepForestSketch || hasCreatedDeepForestExtension || propRoot == null)
        {
            return;
        }

        hasCreatedDeepForestExtension = true;

        if (backgroundRenderer != null)
        {
            backgroundRenderer.transform.localPosition = GetBackgroundLocalPosition();

            if (hasCompletedOutsideMap)
            {
                backgroundRenderer.transform.localScale = GetBackgroundFinalScale();
                UpdateBackgroundEdgeFade();
            }
        }

        if (forestRightBoundaryCollider != null)
        {
            forestRightBoundaryCollider.enabled = false;
        }

        RemoveExtensionFenceLinework();

        CreateDeepForestExtensionBoundary();
        CreateDeepForestExtensionPath();
        CreateDeepForestQuestObjects();
        CreateDeepForestExtensionDecorations();
        EnsureVillageGreenColoringApplied();
        EnsureBrownColoringApplied();
    }

    private void EnsureFourthForestExtensionCreated()
    {
        if (!GameProgress.HasDrawnFourthForestSketch || hasCreatedFourthForestExtension || propRoot == null)
        {
            return;
        }

        EnsureDeepForestExtensionCreated();
        hasCreatedFourthForestExtension = true;
        GlowingLeafQuestRoute.ResetRoute();

        if (backgroundRenderer != null)
        {
            backgroundRenderer.transform.localPosition = GetBackgroundLocalPosition();

            if (hasCompletedOutsideMap)
            {
                backgroundRenderer.transform.localScale = GetBackgroundFinalScale();
                UpdateBackgroundEdgeFade();
            }
        }

        if (deepForestRightBoundaryCollider != null)
        {
            deepForestRightBoundaryCollider.enabled = false;
        }

        DestroyGeneratedChildrenWithPrefix(boundaryRoot, "GeneratedDeepForestExtensionBoundary_Right");

        RemoveExtensionFenceLinework();

        CreateFourthForestExtensionBoundary();
        CreateFourthForestExtensionPath();
        CreateFourthForestExtensionDecorations();
        EnsureVillageGreenColoringApplied();
        EnsureBrownColoringApplied();
        EnsureBlueWaterColoringApplied();
    }

    private void CreateVillageForestGateColliders()
    {
        if (hasCreatedVillageForestGateColliders || boundaryRoot == null)
        {
            return;
        }

        hasCreatedVillageForestGateColliders = true;
        SetCollidersEnabled(villageGateBoundaryColliders, false);

        float halfWidth = BackgroundSize.x * 0.5f;
        float halfHeight = BackgroundSize.y * 0.5f;
        float thickness = BoundaryColliderThickness;
        float forestLeft = ForestRegionOffset.x - halfWidth;

        DestroyGeneratedChildrenWithPrefix(boundaryRoot, "GeneratedVillageForestSeamBoundary_");
        CreateVillageForestSeamFillColliders(forestLeft, halfHeight, thickness);
    }

    private void CreateVillageForestSeamFillColliders(float forestLeft, float halfHeight, float thickness)
    {
        const int seamSegmentCount = 8;
        float firstY = ForestGateHalfHeight + thickness * 0.5f;
        float lastY = halfHeight - thickness * 0.5f;
        float rightEdge = forestLeft + thickness * 0.5f;

        for (int i = 0; i < seamSegmentCount; i++)
        {
            float t = (i + 0.5f) / seamSegmentCount;
            float y = Mathf.Lerp(firstY, lastY, t);
            float leftEdge = GetOutsideOvalExteriorRightXAtYBand(y, thickness * 0.5f);
            float width = rightEdge - leftEdge;

            if (width <= 0.05f)
            {
                continue;
            }

            CreateBoundaryCollider(
                $"GeneratedVillageForestSeamFill_Top_{i:00}",
                boundaryRoot,
                new Vector2((leftEdge + rightEdge) * 0.5f, y),
                new Vector2(width, thickness));
            CreateBoundaryCollider(
                $"GeneratedVillageForestSeamFill_Bottom_{i:00}",
                boundaryRoot,
                new Vector2((leftEdge + rightEdge) * 0.5f, -y),
                new Vector2(width, thickness));
        }
    }

    private void CreateForestExtensionBoundary()
    {
        if (boundaryRoot == null)
        {
            return;
        }

        float halfWidth = BackgroundSize.x * 0.5f;
        float halfHeight = BackgroundSize.y * 0.5f;
        float thickness = BoundaryColliderThickness;
        float centerX = ForestRegionOffset.x;
        float forestRightEdge = centerX + halfWidth + thickness;
        float extendedLeftEdge = GetOutsideOvalExteriorRightXAtYBand(halfHeight + thickness * 0.5f, thickness * 0.5f);
        float horizontalCenterX = (extendedLeftEdge + forestRightEdge) * 0.5f;
        float horizontalWidth = Mathf.Max(0.1f, forestRightEdge - extendedLeftEdge);

        forestRightBoundaryCollider = CreateBoundaryCollider(
            "GeneratedForestExtensionBoundary_Right",
            boundaryRoot,
            new Vector2(centerX + halfWidth + thickness * 0.5f, 0f),
            new Vector2(thickness, BackgroundSize.y + thickness * 2f));
        CreateBoundaryCollider(
            "GeneratedForestExtensionBoundary_Top",
            boundaryRoot,
            new Vector2(horizontalCenterX, halfHeight + thickness * 0.5f),
            new Vector2(horizontalWidth, thickness));
        CreateBoundaryCollider(
            "GeneratedForestExtensionBoundary_Bottom",
            boundaryRoot,
            new Vector2(horizontalCenterX, -halfHeight - thickness * 0.5f),
            new Vector2(horizontalWidth, thickness));
    }

    private void CreateForestExtensionPath()
    {
        Vector3[] centerLine = BuildForestExtensionPathCenterLine();
        Color pathColor = new Color32(132, 98, 61, 175);
        SketchWorldLineDrawing pathDrawing = CreateLineDrawing(
            "GeneratedForestExtensionPath",
            Vector3.zero,
            BuildOutsidePathStrokes(centerLine),
            PathSortingOrder,
            propRoot.transform,
            pathColor);
        pathDrawing.Configure(HouseLineWidth * 1.28f, pathColor, PathSortingOrder);
        RegisterLineReveal(pathDrawing, PathRevealDistance, centerLine);
    }

    private void CreateDeepForestExtensionBoundary()
    {
        if (boundaryRoot == null)
        {
            return;
        }

        float halfWidth = DeepForestSize.x * 0.5f;
        float halfHeight = DeepForestSize.y * 0.5f;
        float thickness = BoundaryColliderThickness;
        float centerX = DeepForestRegionOffset.x;

        deepForestRightBoundaryCollider = CreateBoundaryCollider(
            "GeneratedDeepForestExtensionBoundary_Right",
            boundaryRoot,
            new Vector2(centerX + halfWidth + thickness * 0.5f, 0f),
            new Vector2(thickness, BackgroundSize.y + thickness * 2f));
        CreateBoundaryCollider(
            "GeneratedDeepForestExtensionBoundary_Top",
            boundaryRoot,
            new Vector2(centerX, halfHeight + thickness * 0.5f),
            new Vector2(DeepForestSize.x + thickness * 2f, thickness));
        CreateBoundaryCollider(
            "GeneratedDeepForestExtensionBoundary_Bottom",
            boundaryRoot,
            new Vector2(centerX, -halfHeight - thickness * 0.5f),
            new Vector2(DeepForestSize.x + thickness * 2f, thickness));
    }

    private void CreateDeepForestExtensionPath()
    {
        Vector3[] centerLine = BuildDeepForestExtensionPathCenterLine();
        Color pathColor = new Color32(132, 98, 61, 175);
        SketchWorldLineDrawing pathDrawing = CreateLineDrawing(
            "GeneratedDeepForestExtensionPath",
            Vector3.zero,
            BuildOutsidePathStrokes(centerLine),
            PathSortingOrder,
            propRoot.transform,
            pathColor);
        pathDrawing.Configure(HouseLineWidth * 1.28f, pathColor, PathSortingOrder);
        RegisterLineReveal(pathDrawing, PathRevealDistance, centerLine);

        SketchWorldLineDrawing dirtFloorDrawing = CreateLineDrawing(
            "GeneratedDeepForestEntranceDirtFloor",
            DeepForestEntranceDirtOffset,
            BuildDeepForestEntranceDirtStrokes(),
            PathSortingOrder,
            propRoot.transform,
            pathColor);
        dirtFloorDrawing.Configure(HouseLineWidth * 1.4f, pathColor, PathSortingOrder);
        RegisterLineReveal(dirtFloorDrawing, PathRevealDistance, centerLine);
    }

    private void CreateFourthForestExtensionBoundary()
    {
        if (boundaryRoot == null)
        {
            return;
        }

        float halfWidth = FourthForestSize.x * 0.5f;
        float halfHeight = FourthForestSize.y * 0.5f;
        float thickness = BoundaryColliderThickness;
        float centerX = FourthForestRegionOffset.x;

        CreateBoundaryCollider(
            "GeneratedFourthForestExtensionBoundary_Right",
            boundaryRoot,
            new Vector2(centerX + halfWidth + thickness * 0.5f, 0f),
            new Vector2(thickness, FourthForestSize.y + thickness * 2f));
        CreateBoundaryCollider(
            "GeneratedFourthForestExtensionBoundary_Top",
            boundaryRoot,
            new Vector2(centerX, halfHeight + thickness * 0.5f),
            new Vector2(FourthForestSize.x + thickness * 2f, thickness));
        CreateBoundaryCollider(
            "GeneratedFourthForestExtensionBoundary_Bottom",
            boundaryRoot,
            new Vector2(centerX, -halfHeight - thickness * 0.5f),
            new Vector2(FourthForestSize.x + thickness * 2f, thickness));
    }

    private void CreateFourthForestExtensionPath()
    {
        Vector3[] centerLine = BuildFourthForestExtensionPathCenterLine();
        Color pathColor = new Color32(132, 98, 61, 175);
        SketchWorldLineDrawing pathDrawing = CreateLineDrawing(
            "GeneratedFourthForestExtensionPath",
            Vector3.zero,
            BuildOutsidePathStrokes(centerLine),
            PathSortingOrder,
            propRoot.transform,
            pathColor);
        pathDrawing.Configure(HouseLineWidth * 1.28f, pathColor, PathSortingOrder);
        RegisterLineReveal(pathDrawing, PathRevealDistance, centerLine);
    }

    private void CreateDeepForestQuestObjects()
    {
        CreateDeepForestSquirrel();
        CreateDeepForestHungrySquirrel();
        CreateDeepForestShakingBush();
    }

    private void CreateDeepForestSquirrel()
    {
        SketchWorldLineDrawing squirrelDrawing = CreateLineDrawing(
            "GeneratedDeepForestSquirrelLineDrawing",
            DeepForestSquirrelOffset,
            BuildSquirrelStrokes(),
            ForestSquirrelSortingOrder,
            propRoot.transform,
            new Color32(56, 42, 30, 255));
        squirrelDrawing.Configure(HouseLineWidth * 0.96f, new Color32(56, 42, 30, 255), ForestSquirrelSortingOrder);

        ForestSquirrelAcornQuestInteract squirrelInteract = squirrelDrawing.gameObject.AddComponent<ForestSquirrelAcornQuestInteract>();
        squirrelInteract.Configure(ForestSquirrelPromptDistance, ForestSquirrelPromptSortingOrder);
        squirrelInteract.enabled = false;

        RegisterLineReveal(squirrelDrawing, squirrelInteract, DeepForestSquirrelOffset);
        CreatePropGroundStroke("GeneratedDeepForestSquirrel", DeepForestSquirrelOffset, 0.92f, ForestSquirrelSortingOrder - 1, propRoot.transform);
    }

    private void CreateDeepForestHungrySquirrel()
    {
        SketchWorldLineDrawing squirrelDrawing = CreateLineDrawing(
            "GeneratedDeepForestHungrySquirrelLineDrawing",
            DeepForestHungrySquirrelOffset,
            BuildHungrySquirrelStrokes(),
            HungrySquirrelSortingOrder,
            propRoot.transform,
            new Color32(56, 42, 30, 255));
        squirrelDrawing.Configure(HouseLineWidth * 0.94f, new Color32(56, 42, 30, 255), HungrySquirrelSortingOrder);

        ForestHungrySquirrelMushroomQuestInteract squirrelInteract = squirrelDrawing.gameObject.AddComponent<ForestHungrySquirrelMushroomQuestInteract>();
        squirrelInteract.Configure(HungrySquirrelPromptDistance, HungrySquirrelPromptSortingOrder);
        squirrelInteract.enabled = false;

        RegisterLineReveal(squirrelDrawing, squirrelInteract, DeepForestHungrySquirrelOffset);
        CreatePropGroundStroke("GeneratedDeepForestHungrySquirrel", DeepForestHungrySquirrelOffset, 0.92f, HungrySquirrelSortingOrder - 1, propRoot.transform);
    }

    private void CreateDeepForestShakingBush()
    {
        Vector3 bushOffset = PickDeepForestShakingBushOffset();
        SketchWorldLineDrawing bushDrawing = CreateLineDrawing(
            "GeneratedDeepForestShakingBushLineDrawing",
            bushOffset,
            BuildShakingBushStrokes(),
            AcornSortingOrder,
            propRoot.transform,
            new Color32(42, 75, 38, 255));
        bushDrawing.Configure(HouseLineWidth * 0.9f, new Color32(42, 75, 38, 255), AcornSortingOrder);

        ShakingBushAcornQuestInteract bushInteract = bushDrawing.gameObject.AddComponent<ShakingBushAcornQuestInteract>();
        bushInteract.Configure(AcornPromptDistance, AcornPromptSortingOrder);
        bushInteract.enabled = false;

        RegisterLineReveal(bushDrawing, bushInteract, bushOffset);
        CreatePropGroundStroke("GeneratedDeepForestShakingBush", bushOffset, 1.08f, AcornSortingOrder - 1, propRoot.transform);
    }

    private static Vector3 PickDeepForestShakingBushOffset()
    {
        if (DeepForestShakingBushOffsets.Length == 0)
        {
            return DeepForestRegionOffset;
        }

        int selectedIndex = Random.Range(0, DeepForestShakingBushOffsets.Length);
        return DeepForestShakingBushOffsets[selectedIndex];
    }

    private void CreateForestExtensionQuestObjects()
    {
        CreateForestWell();
        CreateForestSmallBird();
        CreateForestFallenBird();
        CreateForestRocks();
        CreateMoleHoles();
    }

    private void CreateForestWell()
    {
        SketchWorldLineDrawing wellDrawing = CreateLineDrawing(
            "GeneratedForestWellLineDrawing",
            ForestWellOffset,
            BuildForestWellStrokes(),
            ForestWellSortingOrder,
            propRoot.transform,
            new Color32(62, 55, 48, 255));
        wellDrawing.Configure(HouseLineWidth * 1.05f, new Color32(62, 55, 48, 255), ForestWellSortingOrder);

        ForestWellBottleInteract wellInteract = wellDrawing.gameObject.AddComponent<ForestWellBottleInteract>();
        wellInteract.Configure(ForestWellPromptDistance, ForestWellPromptSortingOrder);
        wellInteract.enabled = false;

        RegisterLineReveal(wellDrawing, wellInteract, ForestWellOffset);
        CreatePropGroundStroke("GeneratedForestWell", ForestWellOffset, 1.55f, ForestWellSortingOrder - 1, propRoot.transform);
        CreateForestWellCollider();
    }

    private void CreateForestWellCollider()
    {
        Transform colliderParent = boundaryRoot != null ? boundaryRoot : propRoot.transform;
        GameObject colliderObject = new GameObject("GeneratedForestWellCollider");
        colliderObject.transform.SetParent(colliderParent, false);
        colliderObject.transform.localPosition = ForestWellOffset + new Vector3(0f, -0.34f, 0f);

        BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(2.75f, 1.38f);
        collider.isTrigger = false;
    }

    private void CreateForestSmallBird()
    {
        SketchWorldLineDrawing birdDrawing = CreateLineDrawing(
            "GeneratedForestSmallBirdLineDrawing",
            ForestSmallBirdOffset,
            BuildSmallBirdStrokes(),
            ForestBirdSortingOrder,
            propRoot.transform,
            new Color32(42, 37, 31, 255));
        birdDrawing.Configure(HouseLineWidth * 0.92f, new Color32(42, 37, 31, 255), ForestBirdSortingOrder);

        ForestBirdQuestInteract birdInteract = birdDrawing.gameObject.AddComponent<ForestBirdQuestInteract>();
        birdInteract.Configure(ForestBirdPromptDistance, ForestBirdPromptSortingOrder);
        birdInteract.enabled = false;

        RegisterLineReveal(birdDrawing, birdInteract, ForestSmallBirdOffset);
        CreatePropGroundStroke("GeneratedForestSmallBird", ForestSmallBirdOffset, 0.82f, ForestBirdSortingOrder - 1, propRoot.transform);
    }

    private void CreateForestFallenBird()
    {
        SketchWorldLineDrawing birdDrawing = CreateLineDrawing(
            "GeneratedForestFallenBirdLineDrawing",
            ForestFallenBirdOffset,
            BuildFallenBirdStrokes(),
            ForestBirdSortingOrder,
            propRoot.transform,
            new Color32(42, 37, 31, 255));
        birdDrawing.Configure(HouseLineWidth * 0.92f, new Color32(42, 37, 31, 255), ForestBirdSortingOrder);

        ForestFallenBirdInteract birdInteract = birdDrawing.gameObject.AddComponent<ForestFallenBirdInteract>();
        birdInteract.Configure(ForestBirdPromptDistance, ForestBirdPromptSortingOrder);
        birdInteract.enabled = false;

        RegisterLineReveal(birdDrawing, birdInteract, ForestFallenBirdOffset);
        CreatePropGroundStroke("GeneratedForestFallenBird", ForestFallenBirdOffset, 0.94f, ForestBirdSortingOrder - 1, propRoot.transform);
    }

    private void CreateMoleHoles()
    {
        for (int i = 0; i < ForestMoleHoleOffsets.Length; i++)
        {
            Vector3 holePosition = ForestMoleHoleOffsets[i];
            SketchWorldLineDrawing holeDrawing = CreateLineDrawing(
                $"GeneratedMoleHole_{i:00}",
                holePosition,
                BuildMoleHoleStrokes(),
                MoleHoleSortingOrder,
                propRoot.transform,
                new Color32(68, 47, 32, 255));
            holeDrawing.Configure(HouseLineWidth * 0.82f, new Color32(68, 47, 32, 255), MoleHoleSortingOrder);

            MoleHoleInteract holeInteract = holeDrawing.gameObject.AddComponent<MoleHoleInteract>();
            holeInteract.Configure(i, MoleHolePromptDistance, MoleHolePromptSortingOrder, propRoot.transform);
            holeInteract.enabled = false;

            RegisterLineReveal(holeDrawing, holeInteract, holePosition);
        }
    }

    private void CreateForestRocks()
    {
        for (int i = 0; i < ForestRockOffsets.Length; i++)
        {
            Vector3 rockPosition = ForestRockOffsets[i];
            bool hasReward = i == ForestRewardRockIndex;
            float scale = hasReward ? 1.08f : 0.92f + 0.08f * (i % 3);
            SketchWorldLineDrawing rockDrawing = CreateLineDrawing(
                $"GeneratedForestRock_{i:00}",
                rockPosition,
                BuildForestRockStrokes(scale),
                PropSortingOrder + 2,
                propRoot.transform,
                new Color32(74, 66, 58, 255));
            rockDrawing.Configure(HouseLineWidth * 0.9f, new Color32(74, 66, 58, 255), PropSortingOrder + 2);

            ForestRockCrackInteract rockInteract = rockDrawing.gameObject.AddComponent<ForestRockCrackInteract>();
            rockInteract.Configure(hasReward, ForestRockPromptDistance, ForestRockPromptSortingOrder);
            rockInteract.enabled = false;

            RegisterLineReveal(rockDrawing, rockInteract, rockPosition);
            CreatePropGroundStroke($"GeneratedForestRock_{i:00}", rockPosition, 1.2f * scale, PropSortingOrder + 1, propRoot.transform);
        }
    }

    private void CreateForestExtensionDecorations()
    {
        System.Random random = new System.Random(DecorationRandomSeed + 37);
        List<DecorationFootprint> usedFootprints = new List<DecorationFootprint>();
        int placedTreeCount = 0;
        bool hasCreatedBranchTree = false;

        for (int i = 0; i < ForestExtensionTreeCount; i++)
        {
            float scale = RandomRange(random, TreeScaleRange.x, TreeScaleRange.y);
            Vector2 halfSize = GetPineDecorationHalfSize(scale);

            if (!TryPickForestTreePosition(random, usedFootprints, halfSize, 4.4f, out Vector2 position))
            {
                continue;
            }

            Vector3 localPosition = new Vector3(position.x, position.y, 0f);
            bool isBranchTree = !hasCreatedBranchTree && placedTreeCount >= ForestBranchTreePlacementIndex;
            SketchWorldLineDrawing pineTree = CreateLineDrawing(
                $"GeneratedForestExtensionPine_{i:00}",
                localPosition,
                BuildPineTreeStrokes(scale),
                TreeSortingOrder,
                propRoot.transform);
            TreeCrayonColorTarget colorTarget = pineTree.gameObject.AddComponent<TreeCrayonColorTarget>();
            colorTarget.Configure(scale);

            if (isBranchTree)
            {
                ForestBranchTreeInteract branchInteract = pineTree.gameObject.AddComponent<ForestBranchTreeInteract>();
                branchInteract.Configure(scale, ForestBranchTreePromptDistance, ForestBranchPromptSortingOrder);
                branchInteract.enabled = false;
                RegisterLineReveal(pineTree, branchInteract, localPosition);
                hasCreatedBranchTree = true;
            }
            else
            {
                RegisterLineReveal(pineTree, localPosition);
            }

            CreatePropGroundStroke($"GeneratedForestExtensionPine_{i:00}", localPosition, 1.16f * scale, TreeSortingOrder - 1, propRoot.transform);

            if (GameProgress.HasColoredVillageGreen)
            {
                CreateTreeLeafOverlay(colorTarget, pineTree, TreeSortingOrder + 2);
            }

            if (GameProgress.HasColoredBrownDetails)
            {
                CreateTreeTrunkOverlay(colorTarget, pineTree, TreeSortingOrder + 3);
            }

            usedFootprints.Add(BuildDecorationFootprint(position, halfSize));
            placedTreeCount++;
        }

        CreateOuterDecorativePines("GeneratedOuterDecorativePine_Forest", ForestRegionOffset, BackgroundSize, DecorationRandomSeed + 311);
    }

    private void CreateDeepForestExtensionDecorations()
    {
        System.Random random = new System.Random(DecorationRandomSeed + 79);
        List<DecorationFootprint> usedFootprints = new List<DecorationFootprint>();
        int leafPileIndex = 0;
        int leafPileRewardIndex = random.Next(DeepForestLeafPileCount);
        HashSet<int> mushroomLeafPileIndices = PickDeepForestMushroomLeafPileIndices(leafPileRewardIndex);

        for (int i = 0; i < DeepForestExtensionTreeCount; i++)
        {
            float scale = RandomRange(random, TreeScaleRange.x * 0.88f, TreeScaleRange.y * 1.04f);
            Vector2 halfSize = GetPineDecorationHalfSize(scale);

            if (!TryPickDeepForestTreePosition(random, usedFootprints, halfSize, 2.65f, out Vector2 position))
            {
                continue;
            }

            Vector3 localPosition = new Vector3(position.x, position.y, 0f);
            SketchWorldLineDrawing pineTree = CreateLineDrawing(
                $"GeneratedDeepForestExtensionPine_{i:00}",
                localPosition,
                BuildPineTreeStrokes(scale),
                TreeSortingOrder,
                propRoot.transform);
            TreeCrayonColorTarget colorTarget = pineTree.gameObject.AddComponent<TreeCrayonColorTarget>();
            colorTarget.Configure(scale);
            RegisterLineReveal(pineTree, localPosition);
            CreatePropGroundStroke($"GeneratedDeepForestExtensionPine_{i:00}", localPosition, 1.16f * scale, TreeSortingOrder - 1, propRoot.transform);

            if (leafPileIndex < DeepForestLeafPileCount)
            {
                CreateDeepForestLeafPileUnderTree(
                    leafPileIndex,
                    localPosition,
                    scale,
                    leafPileIndex == leafPileRewardIndex,
                    mushroomLeafPileIndices.Contains(leafPileIndex));
                leafPileIndex++;
            }

            if (GameProgress.HasColoredVillageGreen)
            {
                CreateTreeLeafOverlay(colorTarget, pineTree, TreeSortingOrder + 2);
            }

            if (GameProgress.HasColoredBrownDetails)
            {
                CreateTreeTrunkOverlay(colorTarget, pineTree, TreeSortingOrder + 3);
            }

            usedFootprints.Add(BuildDecorationFootprint(position, halfSize));
        }

        CreateOuterDecorativePines("GeneratedOuterDecorativePine_DeepForest", DeepForestRegionOffset, DeepForestSize, DecorationRandomSeed + 347);
    }

    private static HashSet<int> PickDeepForestMushroomLeafPileIndices(int excludedIndex)
    {
        HashSet<int> indices = new HashSet<int>();
        System.Random random = new System.Random(DecorationRandomSeed + 131);
        int targetCount = Mathf.Min(DeepForestMushroomLeafPileCount, DeepForestLeafPileCount - 1);

        while (indices.Count < targetCount)
        {
            int index = random.Next(DeepForestLeafPileCount);
            if (index != excludedIndex)
            {
                indices.Add(index);
            }
        }

        return indices;
    }

    private void CreateDeepForestLeafPileUnderTree(int leafPileIndex, Vector3 treePosition, float treeScale, bool hasReward, bool hasMushroom)
    {
        Vector3 leafPilePosition = treePosition + new Vector3(0f, -0.56f * Mathf.Max(0.8f, treeScale), 0f);
        float scale = Mathf.Clamp(treeScale * 0.72f, 0.78f, 1.08f);
        SketchWorldLineDrawing leafPileDrawing = CreateLineDrawing(
            $"GeneratedDeepForestLeafPile_{leafPileIndex:00}",
            leafPilePosition,
            BuildLeafPileStrokes(scale),
            DeepForestLeafPileSortingOrder,
            propRoot.transform,
            new Color32(104, 70, 34, 255));
        leafPileDrawing.Configure(HouseLineWidth * 0.78f, new Color32(104, 70, 34, 255), DeepForestLeafPileSortingOrder);

        LeafPileCrayonColorTarget colorTarget = leafPileDrawing.gameObject.AddComponent<LeafPileCrayonColorTarget>();
        colorTarget.Configure(scale);

        DeepForestLeafPileInteract leafPileInteract = leafPileDrawing.gameObject.AddComponent<DeepForestLeafPileInteract>();
        leafPileInteract.Configure(hasReward, hasMushroom, DeepForestLeafPilePromptDistance, DeepForestLeafPilePromptSortingOrder);
        leafPileInteract.enabled = false;

        RegisterLineReveal(leafPileDrawing, leafPileInteract, leafPilePosition);
    }

    private void CreateFourthForestExtensionDecorations()
    {
        CreateFourthForestBigTree();

        System.Random random = new System.Random(DecorationRandomSeed + 173);
        List<DecorationFootprint> usedFootprints = new List<DecorationFootprint>
        {
            BuildDecorationFootprint(FourthForestBigTreeOffset, new Vector2(8.6f, 9.4f))
        };

        for (int i = 0; i < FourthForestExtensionTreeCount; i++)
        {
            float scale = RandomRange(random, TreeScaleRange.x * 0.76f, TreeScaleRange.y * 0.96f);
            Vector2 halfSize = GetPineDecorationHalfSize(scale);

            if (!TryPickFourthForestTreePosition(random, usedFootprints, halfSize, 2.25f, out Vector2 position))
            {
                continue;
            }

            Vector3 localPosition = new Vector3(position.x, position.y, 0f);
            SketchWorldLineDrawing pineTree = CreateLineDrawing(
                $"GeneratedFourthForestExtensionPine_{i:00}",
                localPosition,
                BuildPineTreeStrokes(scale),
                TreeSortingOrder,
                propRoot.transform);
            TreeCrayonColorTarget colorTarget = pineTree.gameObject.AddComponent<TreeCrayonColorTarget>();
            colorTarget.Configure(scale);
            int glowingLeafTreeIndex = GlowingLeafQuestRoute.RegisterTree(localPosition, propRoot.transform, pineTree, scale);
            FourthForestGlowingLeafTreeInteract glowingLeafInteract = pineTree.gameObject.AddComponent<FourthForestGlowingLeafTreeInteract>();
            glowingLeafInteract.Configure(glowingLeafTreeIndex, scale, 1.9f, FourthForestBigTreePromptSortingOrder);
            RegisterLineReveal(pineTree, localPosition);
            CreatePropGroundStroke($"GeneratedFourthForestExtensionPine_{i:00}", localPosition, 1.16f * scale, TreeSortingOrder - 1, propRoot.transform);

            if (GameProgress.HasColoredVillageGreen)
            {
                CreateTreeLeafOverlay(colorTarget, pineTree, TreeSortingOrder + 2);
            }

            if (GameProgress.HasColoredBrownDetails)
            {
                CreateTreeTrunkOverlay(colorTarget, pineTree, TreeSortingOrder + 3);
            }

            usedFootprints.Add(BuildDecorationFootprint(position, halfSize));
        }

        GlowingLeafQuestRoute.ReassignActiveQuestToPreferredTarget();
        CreateOuterDecorativePines("GeneratedOuterDecorativePine_FourthForest", FourthForestRegionOffset, FourthForestSize, DecorationRandomSeed + 389);
    }

    private void CreateOuterDecorativePines(string objectPrefix, Vector3 regionCenter, Vector2 regionSize, int randomSeed, bool revealImmediately = false, float revealDistance = MapRevealDistance)
    {
        if (propRoot == null)
        {
            return;
        }

        System.Random random = new System.Random(randomSeed);
        List<DecorationFootprint> usedFootprints = new List<DecorationFootprint>();
        float perimeter = (regionSize.x + regionSize.y) * 2f;
        int targetCount = Mathf.Max(10, Mathf.RoundToInt(perimeter / OuterDecorativeTreeSpacing * 0.38f * OuterDecorativeTreeCountMultiplier));

        for (int i = 0; i < targetCount; i++)
        {
            float scale = RandomRange(random, TreeScaleRange.x * 0.82f, TreeScaleRange.y * 1.02f);
            Vector2 halfSize = GetPineDecorationHalfSize(scale);

            if (!TryPickOuterDecorativeTreePosition(random, usedFootprints, regionCenter, regionSize, halfSize, out Vector2 position))
            {
                continue;
            }

            Vector3 localPosition = new Vector3(position.x, position.y, 0f);
            CreateDecorativePineTree($"{objectPrefix}_{i:00}", localPosition, scale, TreeSortingOrder - 1, propRoot.transform, revealImmediately, revealDistance);
            usedFootprints.Add(BuildDecorationFootprint(position, halfSize));
        }
    }

    private SketchWorldLineDrawing CreateDecorativePineTree(string objectName, Vector3 localPosition, float scale, int sortingOrder, Transform parent, bool revealImmediately = false, float revealDistance = MapRevealDistance)
    {
        SketchWorldLineDrawing pineTree = CreateLineDrawing(
            objectName,
            localPosition,
            BuildPineTreeStrokes(scale),
            sortingOrder,
            parent);
        TreeCrayonColorTarget colorTarget = pineTree.gameObject.AddComponent<TreeCrayonColorTarget>();
        colorTarget.Configure(scale);

        if (revealImmediately)
        {
            pineTree.RevealProgress = 1f;
        }
        else
        {
            RegisterLineReveal(pineTree, revealDistance, localPosition);
        }

        CreatePropGroundStroke(objectName, localPosition, 1.16f * scale, sortingOrder - 1, parent, revealImmediately, revealDistance);

        if (GameProgress.HasColoredVillageGreen)
        {
            CreateTreeLeafOverlay(colorTarget, pineTree, sortingOrder + 2);
        }

        if (GameProgress.HasColoredBrownDetails)
        {
            CreateTreeTrunkOverlay(colorTarget, pineTree, sortingOrder + 3);
        }

        if (revealImmediately)
        {
            RevealLineDrawingsImmediately(pineTree.transform);
        }

        return pineTree;
    }

    private static void RevealLineDrawingsImmediately(Transform root)
    {
        if (root == null)
        {
            return;
        }

        SketchWorldLineDrawing[] drawings = root.GetComponentsInChildren<SketchWorldLineDrawing>(true);
        for (int i = 0; i < drawings.Length; i++)
        {
            if (drawings[i] != null)
            {
                drawings[i].RevealProgress = 1f;
            }
        }
    }

    private void CreateFourthForestBigTree()
    {
        SketchWorldLineDrawing treeDrawing = CreateLineDrawing(
            "GeneratedFourthForestBigTreeLineDrawing",
            FourthForestBigTreeOffset,
            BuildAncientTreeStrokes(),
            TreeSortingOrder + 2,
            propRoot.transform,
            new Color32(43, 36, 28, 255));
        treeDrawing.Configure(HouseLineWidth * 1.05f, new Color32(43, 36, 28, 255), TreeSortingOrder + 2);
        FourthForestBigTreePrompt bigTreePrompt = treeDrawing.gameObject.AddComponent<FourthForestBigTreePrompt>();
        bigTreePrompt.Configure(FourthForestBigTreePromptDistance, FourthForestBigTreePromptSortingOrder);
        bigTreePrompt.enabled = false;
        RegisterLineReveal(treeDrawing, bigTreePrompt, FourthForestBigTreeOffset);
        CreatePropGroundStroke("GeneratedFourthForestBigTree", FourthForestBigTreeOffset, 5.8f, TreeSortingOrder - 1, propRoot.transform);

        if (GameProgress.HasColoredVillageGreen)
        {
            CreateFourthForestBigTreeGreenOverlay(treeDrawing);
        }

        if (GameProgress.HasColoredBrownDetails)
        {
            CreateFourthForestBigTreeBrownOverlay(treeDrawing);
        }
    }

    private void CreateDeepForestGrassDecorations(System.Random random, List<DecorationFootprint> usedFootprints, int treeCountSeed)
    {
        Sprite sprite = LoadSketchSprite(GrassResourcePath);
        if (sprite == null)
        {
            return;
        }

        int grassCount = Mathf.Clamp(treeCountSeed * 3, 90, 150);
        for (int i = 0; i < grassCount; i++)
        {
            float scale = RandomRange(random, GrassScaleRange.x, GrassScaleRange.y);
            float scaleX = random.NextDouble() < 0.5d ? -scale : scale;
            Vector3 localScale = new Vector3(scaleX, scale, 1f);
            Vector2 halfSize = GetDecorationHalfSize(sprite, localScale);

            if (!TryPickDeepForestDecorationPosition(random, usedFootprints, halfSize, 2.1f, out Vector2 position))
            {
                continue;
            }

            Transform grassTransform = CreateSpriteProp(
                $"GeneratedGrassDeepForest_{i:00}",
                sprite,
                new Vector3(position.x, position.y, 0f),
                localScale,
                GrassSortingOrder,
                propRoot.transform);

            if (grassTransform != null)
            {
                usedFootprints.Add(BuildDecorationFootprint(position, halfSize));
            }
        }
    }

    private static bool TryPickForestTreePosition(
        System.Random random,
        List<DecorationFootprint> usedFootprints,
        Vector2 halfSize,
        float minimumSpacing,
        out Vector2 position)
    {
        float minimumSpacingSqr = minimumSpacing * minimumSpacing;
        float minX = ForestRegionOffset.x + DecorationMin.x;
        float maxX = ForestRegionOffset.x + DecorationMax.x;

        for (int attempt = 0; attempt < DecorationPickAttempts; attempt++)
        {
            position = new Vector2(
                RandomRange(random, minX, maxX),
                RandomRange(random, DecorationMin.y, DecorationMax.y));

            if (IsInsideForestClearZone(position))
            {
                continue;
            }

            DecorationFootprint candidate = BuildDecorationFootprint(position, halfSize);

            if (DoesFootprintOverlapForestPath(candidate))
            {
                continue;
            }

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

    private static bool IsInsideForestClearZone(Vector2 position)
    {
        return IsInsideBox(position, ForestRegionOffset + new Vector3(-33f, 0f, 0f), new Vector2(5.2f, 4.6f))
            || IsInsideBox(position, ForestRegionOffset + new Vector3(4.6f, -1.2f, 0f), new Vector2(7.6f, 5.3f))
            || IsInsideBox(position, ForestRegionOffset + new Vector3(31.5f, 0f, 0f), new Vector2(4.6f, 5.2f))
            || IsInsideBox(position, ForestSmallBirdOffset, new Vector2(2.6f, 2.4f))
            || IsInsideBox(position, ForestFallenBirdOffset, new Vector2(2.6f, 2.4f))
            || IsInsideBox(position, ForestWellOffset, new Vector2(4.2f, 3.4f))
            || IsInsideBox(position, MoleRewardOffset, new Vector2(3.2f, 3.0f))
            || IsInsideForestRockClearZone(position)
            || IsInsideMoleHoleClearZone(position);
    }

    private static bool TryPickDeepForestTreePosition(
        System.Random random,
        List<DecorationFootprint> usedFootprints,
        Vector2 halfSize,
        float minimumSpacing,
        out Vector2 position)
    {
        return TryPickDeepForestDecorationPosition(random, usedFootprints, halfSize, minimumSpacing, out position);
    }

    private static bool TryPickDeepForestDecorationPosition(
        System.Random random,
        List<DecorationFootprint> usedFootprints,
        Vector2 halfSize,
        float minimumSpacing,
        out Vector2 position)
    {
        float minimumSpacingSqr = minimumSpacing * minimumSpacing;
        float minX = DeepForestRegionOffset.x - DeepForestSize.x * 0.5f + 2.0f;
        float maxX = DeepForestRegionOffset.x + DeepForestSize.x * 0.5f - 2.0f;

        for (int attempt = 0; attempt < DecorationPickAttempts; attempt++)
        {
            position = new Vector2(
                RandomRange(random, minX, maxX),
                RandomRange(random, DecorationMin.y, DecorationMax.y));

            if (IsInsideDeepForestClearZone(position))
            {
                continue;
            }

            DecorationFootprint candidate = BuildDecorationFootprint(position, halfSize);
            if (DoesFootprintOverlapDeepForestPath(candidate))
            {
                continue;
            }

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

    private static bool IsInsideDeepForestClearZone(Vector2 position)
    {
        return IsInsideBox(position, DeepForestEntranceDirtOffset, new Vector2(5.4f, 4.6f))
            || IsInsideBox(position, DeepForestSquirrelOffset, new Vector2(3.2f, 3.0f))
            || IsInsideBox(position, DeepForestHungrySquirrelOffset, new Vector2(3.4f, 3.1f))
            || IsInsideDeepForestShakingBushCandidateZone(position);
    }

    private static bool IsInsideDeepForestShakingBushCandidateZone(Vector2 position)
    {
        for (int i = 0; i < DeepForestShakingBushOffsets.Length; i++)
        {
            if (IsInsideBox(position, DeepForestShakingBushOffsets[i], new Vector2(3.0f, 2.6f)))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryPickFourthForestTreePosition(
        System.Random random,
        List<DecorationFootprint> usedFootprints,
        Vector2 halfSize,
        float minimumSpacing,
        out Vector2 position)
    {
        float minimumSpacingSqr = minimumSpacing * minimumSpacing;
        float minX = FourthForestRegionOffset.x - FourthForestSize.x * 0.5f + 2.0f;
        float maxX = FourthForestRegionOffset.x + FourthForestSize.x * 0.5f - 2.0f;

        for (int attempt = 0; attempt < DecorationPickAttempts; attempt++)
        {
            position = new Vector2(
                RandomRange(random, minX, maxX),
                RandomRange(random, DecorationMin.y, DecorationMax.y));

            if (IsInsideFourthForestClearZone(position))
            {
                continue;
            }

            DecorationFootprint candidate = BuildDecorationFootprint(position, halfSize);
            if (DoesFootprintOverlapFourthForestPath(candidate))
            {
                continue;
            }

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

    private static bool IsInsideFourthForestClearZone(Vector2 position)
    {
        return IsInsideBox(position, FourthForestBigTreeOffset, new Vector2(10.6f, 9.8f))
            || IsInsideBox(position, FourthForestRegionOffset + new Vector3(-33.0f, 0f, 0f), new Vector2(4.8f, 4.8f))
            || (GameProgress.HasColoredWaterBlue && IsInsideFourthForestRiverIslandTreeClearZone(position, 1.2f));
    }

    private static bool TryPickOuterDecorativeTreePosition(
        System.Random random,
        List<DecorationFootprint> usedFootprints,
        Vector3 regionCenter,
        Vector2 regionSize,
        Vector2 halfSize,
        out Vector2 position)
    {
        float minimumSpacingSqr = OuterDecorativeTreeSpacing * OuterDecorativeTreeSpacing;
        float halfWidth = regionSize.x * 0.5f;
        float halfHeight = regionSize.y * 0.5f;
        float left = regionCenter.x - halfWidth;
        float right = regionCenter.x + halfWidth;
        float bottom = regionCenter.y - halfHeight;
        float top = regionCenter.y + halfHeight;

        for (int attempt = 0; attempt < DecorationPickAttempts; attempt++)
        {
            float bandOffset = RandomRange(random, 1.0f, OuterDecorativeTreeBandWidth);
            switch (random.Next(4))
            {
                case 0:
                    position = new Vector2(left - bandOffset, RandomRange(random, bottom - OuterDecorativeTreeBandWidth, top + OuterDecorativeTreeBandWidth));
                    break;
                case 1:
                    position = new Vector2(right + bandOffset, RandomRange(random, bottom - OuterDecorativeTreeBandWidth, top + OuterDecorativeTreeBandWidth));
                    break;
                case 2:
                    position = new Vector2(RandomRange(random, left - OuterDecorativeTreeBandWidth, right + OuterDecorativeTreeBandWidth), top + bandOffset);
                    break;
                default:
                    position = new Vector2(RandomRange(random, left - OuterDecorativeTreeBandWidth, right + OuterDecorativeTreeBandWidth), bottom - bandOffset);
                    break;
            }

            DecorationFootprint candidate = BuildDecorationFootprint(position, halfSize);
            if (DoesFootprintOverlapPlayableRegion(candidate) || DoesFootprintOverlapAnyOutsideMapPath(candidate))
            {
                continue;
            }

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

    private static bool DoesFootprintOverlapPlayableRegion(DecorationFootprint footprint)
    {
        return DoesFootprintOverlapBox(footprint, Vector3.zero, BackgroundSize * 0.5f)
            || DoesFootprintOverlapBox(footprint, ForestRegionOffset, BackgroundSize * 0.5f)
            || DoesFootprintOverlapBox(footprint, DeepForestRegionOffset, DeepForestSize * 0.5f)
            || DoesFootprintOverlapBox(footprint, FourthForestRegionOffset, FourthForestSize * 0.5f);
    }

    private static bool DoesFootprintOverlapBox(DecorationFootprint footprint, Vector3 center, Vector2 halfExtents)
    {
        return Mathf.Abs(footprint.Center.x - center.x) < footprint.HalfSize.x + halfExtents.x
            && Mathf.Abs(footprint.Center.y - center.y) < footprint.HalfSize.y + halfExtents.y;
    }

    private static bool DoesFootprintOverlapAnyOutsideMapPath(DecorationFootprint footprint)
    {
        return DoesFootprintOverlapOutsidePath(footprint)
            || DoesFootprintOverlapForestPath(footprint)
            || DoesFootprintOverlapDeepForestPath(footprint)
            || DoesFootprintOverlapFourthForestPath(footprint);
    }

    private static bool IsInsideFourthForestRiverIslandTreeClearZone(Vector2 position, float treeScale)
    {
        float margin = 0.72f + Mathf.Max(0f, treeScale) * 0.92f;
        float radiusX = 7.05f + margin;
        float radiusY = 5.58f + margin;
        float deltaX = position.x - FourthForestRiverOffset.x;
        float deltaY = position.y - FourthForestRiverOffset.y;
        float normalizedDistance = (deltaX * deltaX) / (radiusX * radiusX) + (deltaY * deltaY) / (radiusY * radiusY);
        return normalizedDistance <= 1f;
    }

    private static bool IsFourthForestGeneratedPineName(string objectName)
    {
        return !string.IsNullOrEmpty(objectName) && objectName.StartsWith("GeneratedFourthForestExtensionPine_");
    }

    private static bool IsInsideMoleHoleClearZone(Vector2 position)
    {
        for (int i = 0; i < ForestMoleHoleOffsets.Length; i++)
        {
            if (IsInsideBox(position, ForestMoleHoleOffsets[i], new Vector2(2.2f, 1.8f)))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsInsideForestRockClearZone(Vector2 position)
    {
        for (int i = 0; i < ForestRockOffsets.Length; i++)
        {
            if (IsInsideBox(position, ForestRockOffsets[i], new Vector2(2.4f, 1.9f)))
            {
                return true;
            }
        }

        return false;
    }

    private static bool DoesFootprintOverlapForestPath(DecorationFootprint footprint)
    {
        Vector3[] centerLine = BuildForestExtensionPathCenterLine();
        Vector2 footprintCenter = footprint.Center;
        float blockingRadius = footprint.HalfSize.magnitude + PathDecorationClearance;

        for (int pointIndex = 0; pointIndex < centerLine.Length - 1; pointIndex++)
        {
            Vector2 segmentStart = centerLine[pointIndex];
            Vector2 segmentEnd = centerLine[pointIndex + 1];

            if (DistanceToSegment(footprintCenter, segmentStart, segmentEnd) <= blockingRadius)
            {
                return true;
            }
        }

        return false;
    }

    private static bool DoesFootprintOverlapDeepForestPath(DecorationFootprint footprint)
    {
        Vector3[] centerLine = BuildDeepForestExtensionPathCenterLine();
        Vector2 footprintCenter = footprint.Center;
        float blockingRadius = footprint.HalfSize.magnitude + PathDecorationClearance;

        for (int pointIndex = 0; pointIndex < centerLine.Length - 1; pointIndex++)
        {
            Vector2 segmentStart = centerLine[pointIndex];
            Vector2 segmentEnd = centerLine[pointIndex + 1];

            if (DistanceToSegment(footprintCenter, segmentStart, segmentEnd) <= blockingRadius)
            {
                return true;
            }
        }

        return false;
    }

    private static bool DoesFootprintOverlapFourthForestPath(DecorationFootprint footprint)
    {
        Vector3[] centerLine = BuildFourthForestExtensionPathCenterLine();
        Vector2 footprintCenter = footprint.Center;
        float blockingRadius = footprint.HalfSize.magnitude + PathDecorationClearance;

        for (int pointIndex = 0; pointIndex < centerLine.Length - 1; pointIndex++)
        {
            Vector2 segmentStart = centerLine[pointIndex];
            Vector2 segmentEnd = centerLine[pointIndex + 1];

            if (DistanceToSegment(footprintCenter, segmentStart, segmentEnd) <= blockingRadius)
            {
                return true;
            }
        }

        return false;
    }

    private void EnsureVillageGreenColoringApplied()
    {
        if (!GameProgress.HasColoredVillageGreen || hasAppliedVillageGreenColoring || propRoot == null)
        {
            return;
        }

        hasAppliedVillageGreenColoring = true;
        ApplyGrassGreenColoring();
        ApplyTreeLeafGreenColoring();
        ApplyForestEntranceGreenColoring();
        ApplyFourthForestBigTreeGreenColoring();
        ApplyFourthForestIslandDoorCrayonColors(null);
    }

    private void ApplyGrassGreenColoring()
    {
        SpriteRenderer[] spriteRenderers = propRoot.GetComponentsInChildren<SpriteRenderer>(true);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            SpriteRenderer spriteRenderer = spriteRenderers[i];
            if (spriteRenderer == null || !IsGrassColoringTarget(spriteRenderer.gameObject.name))
            {
                continue;
            }

            ApplyGrassLightGreenColor(spriteRenderer);
            CreateGrassColorOverlay(spriteRenderer);
        }
    }

    private void ApplyTreeLeafGreenColoring()
    {
        TreeCrayonColorTarget[] trees = propRoot.GetComponentsInChildren<TreeCrayonColorTarget>(true);

        for (int i = 0; i < trees.Length; i++)
        {
            TreeCrayonColorTarget tree = trees[i];
            if (tree == null || tree.transform.Find("GeneratedGreenLeafOverlay") != null)
            {
                continue;
            }

            PineTreeShakeInteract treeInteract = tree.GetComponent<PineTreeShakeInteract>();
            CreateTreeLeafOverlay(tree, tree.GetComponent<SketchWorldLineDrawing>(), TreeSortingOrder + 2);

            if (treeInteract != null && treeInteract.HasAttachedPinecone)
            {
                treeInteract.SetAttachedPineconeSortingOrder(TreeSortingOrder + 4);
            }
        }
    }

    private void CreateTreeLeafOverlay(TreeCrayonColorTarget colorTarget, SketchWorldLineDrawing sourceDrawing, int sortingOrder)
    {
        if (colorTarget == null || colorTarget.transform.Find("GeneratedGreenLeafOverlay") != null)
        {
            return;
        }

        GameObject overlayObject = new GameObject("GeneratedGreenLeafOverlay");
        overlayObject.transform.SetParent(colorTarget.transform, false);

        SketchWorldLineDrawing leafOverlay = overlayObject.AddComponent<SketchWorldLineDrawing>();
        leafOverlay.Configure(HouseLineWidth * 3.6f, VillageGreenColor, sortingOrder);
        leafOverlay.SetStrokes(BuildPineLeafStrokes(colorTarget.TreeScale));

        SyncTreeCrayonOverlayReveal(sourceDrawing, leafOverlay);
    }

    private void ApplyFourthForestBigTreeGreenColoring()
    {
        if (!GameProgress.HasDrawnFourthForestSketch || propRoot == null)
        {
            return;
        }

        Transform treeTransform = propRoot.transform.Find("GeneratedFourthForestBigTreeLineDrawing");
        SketchWorldLineDrawing sourceDrawing = treeTransform != null
            ? treeTransform.GetComponent<SketchWorldLineDrawing>()
            : null;
        CreateFourthForestBigTreeGreenOverlay(sourceDrawing);
    }

    private void CreateFourthForestBigTreeGreenOverlay(SketchWorldLineDrawing sourceDrawing)
    {
        Transform treeTransform = propRoot != null
            ? propRoot.transform.Find("GeneratedFourthForestBigTreeLineDrawing")
            : null;
        if (treeTransform == null || treeTransform.Find("GeneratedFourthForestBigTreeGreenOverlay") != null)
        {
            return;
        }

        GameObject overlayObject = new GameObject("GeneratedFourthForestBigTreeGreenOverlay");
        overlayObject.transform.SetParent(treeTransform, false);

        SketchWorldLineDrawing leafOverlay = overlayObject.AddComponent<SketchWorldLineDrawing>();
        leafOverlay.Configure(HouseLineWidth * 3.2f, VillageGreenColor, TreeSortingOrder + 4);
        leafOverlay.SetStrokes(BuildAncientTreeLeafColorStrokes());

        if (sourceDrawing != null && sourceDrawing.RevealProgress >= 0.95f)
        {
            leafOverlay.RevealProgress = 1f;
        }
        else
        {
            RegisterLineReveal(leafOverlay, FourthForestBigTreeOffset);
        }
    }

    private void ApplyForestEntranceGreenColoring()
    {
        if (propRoot.transform.Find("GeneratedForestEntranceGreenLeaves") != null)
        {
            return;
        }

        Transform forestEntrance = propRoot.transform.Find("GeneratedForestEntranceLineDrawing");
        SketchWorldLineDrawing sourceDrawing = forestEntrance != null
            ? forestEntrance.GetComponent<SketchWorldLineDrawing>()
            : null;

        GameObject overlayObject = new GameObject("GeneratedForestEntranceGreenLeaves");
        overlayObject.transform.SetParent(propRoot.transform, false);
        overlayObject.transform.localPosition = ForestEntranceOffset;

        SketchWorldLineDrawing leafOverlay = overlayObject.AddComponent<SketchWorldLineDrawing>();
        leafOverlay.Configure(HouseLineWidth * 1.08f, VillageGreenColor, TreeSortingOrder + 4);
        leafOverlay.SetStrokes(BuildForestEntranceLeafStrokes());

        if (sourceDrawing != null && sourceDrawing.RevealProgress >= 0.95f)
        {
            leafOverlay.RevealProgress = 1f;
        }
        else
        {
            RegisterLineReveal(leafOverlay, ForestEntranceOffset);
        }
    }

    private void EnsureBrownColoringApplied()
    {
        if (!GameProgress.HasColoredBrownDetails || propRoot == null)
        {
            return;
        }

        ApplyHouseBrownColoring();
        ApplyPathBrownColoring();
        ApplyFenceBrownColoring();
        ApplyTreeTrunkBrownColoring();
        ApplyPineconeBrownColoring();
        ApplyLeafPileBrownColoring();
        ApplyForestEntranceBrownColoring();
        ApplyFourthForestBigTreeBrownColoring();
        ApplyFourthForestIslandDoorCrayonColors(null);
    }

    private void ApplyHouseBrownColoring()
    {
        if (contentRoot != null)
        {
            CreateBrownLineOverlay(
                "GeneratedOutsideHouseBrownOverlay",
                HouseOffset,
                BuildHouseBrownStrokes(),
                PropSortingOrder + 8,
                contentRoot.transform,
                houseDrawing,
                1.75f,
                MapRevealDistance,
                HouseOffset);
        }

        Transform neighborHouse = propRoot.transform.Find("GeneratedNeighborHouseWithMailbox");
        SketchWorldLineDrawing neighborHouseDrawing = neighborHouse != null
            ? neighborHouse.GetComponent<SketchWorldLineDrawing>()
            : null;

        CreateBrownLineOverlay(
            "GeneratedNeighborHouseBrownOverlay",
            NeighborHouseOffset,
            BuildNeighborHouseBrownStrokes(),
            PropSortingOrder + 8,
            propRoot.transform,
            neighborHouseDrawing,
            1.65f,
            MapRevealDistance,
            NeighborHouseOffset);
    }

    private void ApplyPathBrownColoring()
    {
        Vector3[][] centerLines = BuildOutsidePathCenterLines();
        string[] pathObjectNames =
        {
            "GeneratedOutsidePath_ToNeighborHouse",
            "GeneratedOutsidePath_ToPond",
            "GeneratedOutsidePath_ToForestEntrance"
        };

        for (int i = 0; i < pathObjectNames.Length && i < centerLines.Length; i++)
        {
            Transform pathTransform = propRoot.transform.Find(pathObjectNames[i]);
            SketchWorldLineDrawing pathDrawing = pathTransform != null
                ? pathTransform.GetComponent<SketchWorldLineDrawing>()
                : null;

            CreateBrownLineOverlay(
                $"{pathObjectNames[i]}_BrownOverlay",
                Vector3.zero,
                BuildOutsidePathStrokes(centerLines[i]),
                PathSortingOrder + 2,
                propRoot.transform,
                pathDrawing,
                1.55f,
                PathRevealDistance,
                centerLines[i]);
        }

        if (GameProgress.HasDrawnForestSketch)
        {
            Vector3[] forestCenterLine = BuildForestExtensionPathCenterLine();
            Transform forestPathTransform = propRoot.transform.Find("GeneratedForestExtensionPath");
            SketchWorldLineDrawing forestPathDrawing = forestPathTransform != null
                ? forestPathTransform.GetComponent<SketchWorldLineDrawing>()
                : null;

            CreateBrownLineOverlay(
                "GeneratedForestExtensionPath_BrownOverlay",
                Vector3.zero,
                BuildOutsidePathStrokes(forestCenterLine),
                PathSortingOrder + 2,
                propRoot.transform,
                forestPathDrawing,
                1.55f,
                PathRevealDistance,
                forestCenterLine);
        }

        if (GameProgress.HasDrawnDeepForestSketch)
        {
            Vector3[] deepForestCenterLine = BuildDeepForestExtensionPathCenterLine();
            Transform deepForestPathTransform = propRoot.transform.Find("GeneratedDeepForestExtensionPath");
            SketchWorldLineDrawing deepForestPathDrawing = deepForestPathTransform != null
                ? deepForestPathTransform.GetComponent<SketchWorldLineDrawing>()
                : null;

            CreateBrownLineOverlay(
                "GeneratedDeepForestExtensionPath_BrownOverlay",
                Vector3.zero,
                BuildOutsidePathStrokes(deepForestCenterLine),
                PathSortingOrder + 2,
                propRoot.transform,
                deepForestPathDrawing,
                1.55f,
                PathRevealDistance,
                deepForestCenterLine);

            Transform deepForestDirtTransform = propRoot.transform.Find("GeneratedDeepForestEntranceDirtFloor");
            SketchWorldLineDrawing deepForestDirtDrawing = deepForestDirtTransform != null
                ? deepForestDirtTransform.GetComponent<SketchWorldLineDrawing>()
                : null;

            CreateBrownLineOverlay(
                "GeneratedDeepForestEntranceDirtFloor_BrownOverlay",
                DeepForestEntranceDirtOffset,
                BuildDeepForestEntranceDirtStrokes(),
                PathSortingOrder + 2,
                propRoot.transform,
                deepForestDirtDrawing,
                1.65f,
                PathRevealDistance,
                deepForestCenterLine);
        }

        if (GameProgress.HasDrawnFourthForestSketch)
        {
            Vector3[] fourthForestCenterLine = BuildFourthForestExtensionPathCenterLine();
            Transform fourthForestPathTransform = propRoot.transform.Find("GeneratedFourthForestExtensionPath");
            SketchWorldLineDrawing fourthForestPathDrawing = fourthForestPathTransform != null
                ? fourthForestPathTransform.GetComponent<SketchWorldLineDrawing>()
                : null;

            CreateBrownLineOverlay(
                "GeneratedFourthForestExtensionPath_BrownOverlay",
                Vector3.zero,
                BuildOutsidePathStrokes(fourthForestCenterLine),
                PathSortingOrder + 2,
                propRoot.transform,
                fourthForestPathDrawing,
                1.55f,
                PathRevealDistance,
                fourthForestCenterLine);
        }

    }

    private void ApplyFenceBrownColoring()
    {
        bool hasForestGate = GameProgress.HasDrawnForestSketch;
        List<Vector3[]> fenceStrokes = hasForestGate
            ? BuildFenceRightGateStrokes()
            : BuildFenceRightStrokes();
        CreateFenceBrownOverlay("GeneratedOutsideFence_Oval", "GeneratedOutsideFenceBrown_Oval", Vector3.zero, fenceStrokes, BuildOvalFenceRevealPoints(hasForestGate));

        RemoveExtensionFenceLinework();
    }

    private void CreateFenceBrownOverlay(string sourceObjectName, string overlayObjectName, Vector3 localPosition, List<Vector3[]> strokes, Vector3[] revealPositions)
    {
        Transform sourceTransform = propRoot.transform.Find(sourceObjectName);
        SketchWorldLineDrawing sourceDrawing = sourceTransform != null
            ? sourceTransform.GetComponent<SketchWorldLineDrawing>()
            : null;

        CreateBrownLineOverlay(
            overlayObjectName,
            localPosition,
            strokes,
            PropSortingOrder + 7,
            propRoot.transform,
            sourceDrawing,
            1.18f,
            FenceRevealDistance,
            revealPositions);
    }

    private void ApplyTreeTrunkBrownColoring()
    {
        TreeCrayonColorTarget[] trees = propRoot.GetComponentsInChildren<TreeCrayonColorTarget>(true);

        for (int i = 0; i < trees.Length; i++)
        {
            TreeCrayonColorTarget tree = trees[i];
            if (tree == null)
            {
                continue;
            }

            CreateTreeTrunkOverlay(tree, tree.GetComponent<SketchWorldLineDrawing>(), TreeSortingOrder + 3);
        }
    }

    private void ApplyPineconeBrownColoring()
    {
        PineTreeShakeInteract[] pineTrees = propRoot.GetComponentsInChildren<PineTreeShakeInteract>(true);

        for (int i = 0; i < pineTrees.Length; i++)
        {
            PineTreeShakeInteract pineTree = pineTrees[i];
            if (pineTree == null || !pineTree.HasAttachedPinecone)
            {
                continue;
            }

            pineTree.ApplyBrownCrayonColoring(TreeSortingOrder + 5);
        }
    }

    private void ApplyLeafPileBrownColoring()
    {
        if (!GameProgress.HasDrawnDeepForestSketch)
        {
            return;
        }

        LeafPileCrayonColorTarget[] leafPiles = propRoot.GetComponentsInChildren<LeafPileCrayonColorTarget>(true);

        for (int i = 0; i < leafPiles.Length; i++)
        {
            LeafPileCrayonColorTarget leafPile = leafPiles[i];
            if (leafPile == null)
            {
                continue;
            }

            CreateLeafPileBrownOverlay(leafPile, leafPile.GetComponent<SketchWorldLineDrawing>());
        }
    }

    private void CreateTreeTrunkOverlay(TreeCrayonColorTarget colorTarget, SketchWorldLineDrawing sourceDrawing, int sortingOrder)
    {
        if (colorTarget == null || colorTarget.transform.Find("GeneratedBrownTrunkOverlay") != null)
        {
            return;
        }

        GameObject overlayObject = new GameObject("GeneratedBrownTrunkOverlay");
        overlayObject.transform.SetParent(colorTarget.transform, false);

        SketchWorldLineDrawing trunkOverlay = overlayObject.AddComponent<SketchWorldLineDrawing>();
        trunkOverlay.Configure(HouseLineWidth * 2.25f, BrownCrayonColor, sortingOrder);
        trunkOverlay.SetStrokes(BuildPineTrunkBrownStrokes(colorTarget.TreeScale));

        SyncTreeCrayonOverlayReveal(sourceDrawing, trunkOverlay);
    }

    private static void SyncTreeCrayonOverlayReveal(SketchWorldLineDrawing sourceDrawing, SketchWorldLineDrawing overlayDrawing)
    {
        if (overlayDrawing == null)
        {
            return;
        }

        if (sourceDrawing == null)
        {
            overlayDrawing.RevealProgress = 0f;
            return;
        }

        overlayDrawing.RevealProgress = sourceDrawing.RevealProgress;
        TreeCrayonRevealSync revealSync = overlayDrawing.gameObject.AddComponent<TreeCrayonRevealSync>();
        revealSync.Configure(sourceDrawing, overlayDrawing);
    }

    private void CreateLeafPileBrownOverlay(LeafPileCrayonColorTarget colorTarget, SketchWorldLineDrawing sourceDrawing)
    {
        if (colorTarget == null || colorTarget.transform.Find("GeneratedBrownLeafPileOverlay") != null)
        {
            return;
        }

        GameObject overlayObject = new GameObject("GeneratedBrownLeafPileOverlay");
        overlayObject.transform.SetParent(colorTarget.transform, false);

        SketchWorldLineDrawing leafPileOverlay = overlayObject.AddComponent<SketchWorldLineDrawing>();
        leafPileOverlay.Configure(HouseLineWidth * 1.45f, BrownCrayonColor, DeepForestLeafPileSortingOrder + 3);
        leafPileOverlay.SetStrokes(BuildLeafPileBrownColorStrokes(colorTarget.LeafPileScale));

        if (sourceDrawing != null && sourceDrawing.RevealProgress >= 0.95f)
        {
            leafPileOverlay.RevealProgress = 1f;
        }
        else
        {
            RegisterLineReveal(leafPileOverlay, colorTarget.transform.localPosition);
        }
    }

    private void ApplyFourthForestBigTreeBrownColoring()
    {
        if (!GameProgress.HasDrawnFourthForestSketch || propRoot == null)
        {
            return;
        }

        Transform treeTransform = propRoot.transform.Find("GeneratedFourthForestBigTreeLineDrawing");
        SketchWorldLineDrawing sourceDrawing = treeTransform != null
            ? treeTransform.GetComponent<SketchWorldLineDrawing>()
            : null;
        CreateFourthForestBigTreeBrownOverlay(sourceDrawing);
    }

    private void CreateFourthForestBigTreeBrownOverlay(SketchWorldLineDrawing sourceDrawing)
    {
        Transform treeTransform = propRoot != null
            ? propRoot.transform.Find("GeneratedFourthForestBigTreeLineDrawing")
            : null;
        if (treeTransform == null || treeTransform.Find("GeneratedFourthForestBigTreeBrownOverlay") != null)
        {
            return;
        }

        GameObject overlayObject = new GameObject("GeneratedFourthForestBigTreeBrownOverlay");
        overlayObject.transform.SetParent(treeTransform, false);

        SketchWorldLineDrawing trunkOverlay = overlayObject.AddComponent<SketchWorldLineDrawing>();
        trunkOverlay.Configure(HouseLineWidth * 2.55f, BrownCrayonColor, TreeSortingOrder + 5);
        trunkOverlay.SetStrokes(BuildAncientTreeTrunkBrownColorStrokes());

        if (sourceDrawing != null && sourceDrawing.RevealProgress >= 0.95f)
        {
            trunkOverlay.RevealProgress = 1f;
        }
        else
        {
            RegisterLineReveal(trunkOverlay, FourthForestBigTreeOffset);
        }
    }

    private void ApplyForestEntranceBrownColoring()
    {
        if (propRoot.transform.Find("GeneratedForestEntranceBrownTrunks") != null)
        {
            return;
        }

        Transform forestEntrance = propRoot.transform.Find("GeneratedForestEntranceLineDrawing");
        SketchWorldLineDrawing sourceDrawing = forestEntrance != null
            ? forestEntrance.GetComponent<SketchWorldLineDrawing>()
            : null;

        CreateBrownLineOverlay(
            "GeneratedForestEntranceBrownTrunks",
            ForestEntranceOffset,
            BuildForestEntranceTrunkBrownStrokes(),
            TreeSortingOrder + 5,
            propRoot.transform,
            sourceDrawing,
            1.6f,
            MapRevealDistance,
            ForestEntranceOffset);
    }

    private SketchWorldLineDrawing CreateBrownLineOverlay(
        string objectName,
        Vector3 localPosition,
        List<Vector3[]> strokes,
        int sortingOrder,
        Transform parent,
        SketchWorldLineDrawing sourceDrawing,
        float lineWidthMultiplier,
        float revealDistance,
        params Vector3[] revealPositions)
    {
        if (parent == null || parent.Find(objectName) != null)
        {
            return null;
        }

        SketchWorldLineDrawing overlay = CreateLineDrawing(objectName, localPosition, strokes, sortingOrder, parent, BrownCrayonColor);
        overlay.Configure(HouseLineWidth * lineWidthMultiplier, BrownCrayonColor, sortingOrder);

        if (sourceDrawing != null && sourceDrawing.RevealProgress >= 0.95f)
        {
            overlay.RevealProgress = 1f;
        }
        else
        {
            Vector3[] triggerPositions = revealPositions != null && revealPositions.Length > 0
                ? revealPositions
                : new[] { localPosition };
            RegisterLineReveal(overlay, revealDistance, triggerPositions);
        }

        return overlay;
    }

    private void EnsureBlueWaterColoringApplied()
    {
        if (!GameProgress.HasColoredWaterBlue || propRoot == null)
        {
            return;
        }

        ApplyPondBlueWaterColoring();
        ApplyForestWellBlueWaterColoring();
        ApplyFourthForestRiverBlueWaterColoring();
    }

    private void ApplyPondBlueWaterColoring()
    {
        Transform pondTransform = propRoot.transform.Find("GeneratedPondLineDrawing");
        SketchWorldLineDrawing pondDrawing = pondTransform != null
            ? pondTransform.GetComponent<SketchWorldLineDrawing>()
            : null;

        CreateBlueLineOverlay(
            "GeneratedPondBlueWaterOverlay",
            PondOffset,
            BuildPondBlueWaterStrokes(),
            PropSortingOrder + 4,
            propRoot.transform,
            pondDrawing,
            1.78f,
            MapRevealDistance,
            PondOffset);
    }

    private void ApplyForestWellBlueWaterColoring()
    {
        if (!GameProgress.HasDrawnForestSketch)
        {
            return;
        }

        Transform wellTransform = propRoot.transform.Find("GeneratedForestWellLineDrawing");
        SketchWorldLineDrawing wellDrawing = wellTransform != null
            ? wellTransform.GetComponent<SketchWorldLineDrawing>()
            : null;

        CreateBlueLineOverlay(
            "GeneratedForestWellBlueWaterOverlay",
            ForestWellOffset,
            BuildForestWellBlueWaterStrokes(),
            ForestWellSortingOrder + 2,
            propRoot.transform,
            wellDrawing,
            1.28f,
            MapRevealDistance,
            ForestWellOffset);
    }

    private void ApplyFourthForestRiverBlueWaterColoring()
    {
        if (!GameProgress.HasDrawnFourthForestSketch || propRoot == null)
        {
            return;
        }

        RemoveFourthForestTreesInsideRiverIsland();

        Transform riverTransform = propRoot.transform.Find("GeneratedFourthForestRiverBlueWater");
        SketchWorldLineDrawing riverDrawing = riverTransform != null
            ? riverTransform.GetComponent<SketchWorldLineDrawing>()
            : null;

        if (riverDrawing == null)
        {
            riverDrawing = CreateLineDrawing(
                "GeneratedFourthForestRiverBlueWater",
                FourthForestRiverOffset,
                BuildFourthForestRiverStrokes(),
                FourthForestRiverSortingOrder,
                propRoot.transform,
                BlueCrayonColor);
            riverDrawing.Configure(HouseLineWidth * 2.1f, BlueCrayonColor, FourthForestRiverSortingOrder);
        }

        riverDrawing.SetStrokes(BuildFourthForestRiverStrokes());
        riverDrawing.RevealProgress = 1f;

        FourthForestRiverInteract riverInteract = riverDrawing.GetComponent<FourthForestRiverInteract>();
        if (riverInteract == null)
        {
            riverInteract = riverDrawing.gameObject.AddComponent<FourthForestRiverInteract>();
        }

        riverInteract.Configure(FourthForestRiverPromptDistance, FourthForestBigTreePromptSortingOrder);
        EnsureFourthForestIslandDoorState();
        EnsureFourthForestRiverBlockerState();
        EnsureFourthForestRootBridgeState();
    }

    private void RemoveFourthForestTreesInsideRiverIsland()
    {
        if (propRoot == null)
        {
            return;
        }

        Transform parent = propRoot.transform;
        TreeCrayonColorTarget[] trees = parent.GetComponentsInChildren<TreeCrayonColorTarget>(true);
        List<string> treeNamesToRemove = new List<string>();

        for (int i = 0; i < trees.Length; i++)
        {
            TreeCrayonColorTarget tree = trees[i];
            if (tree == null || tree.transform == null || !IsFourthForestGeneratedPineName(tree.transform.name))
            {
                continue;
            }

            Vector3 localPosition = tree.transform.localPosition;
            if (IsInsideFourthForestRiverIslandTreeClearZone(localPosition, tree.TreeScale))
            {
                treeNamesToRemove.Add(tree.transform.name);
            }
        }

        for (int i = 0; i < treeNamesToRemove.Count; i++)
        {
            string treeName = treeNamesToRemove[i];
            DestroyGeneratedChild(parent, $"{treeName}_GroundStroke");
            DestroyGeneratedChild(parent, treeName);
        }
    }

    private void EnsureFourthForestRiverBlockerState()
    {
        if (boundaryRoot == null)
        {
            return;
        }

        if (!GameProgress.HasColoredWaterBlue || !GameProgress.HasDrawnFourthForestSketch)
        {
            DestroyGeneratedChildrenWithPrefix(boundaryRoot, "GeneratedFourthForestRiverBlocker");
            return;
        }

        const float blockerWidth = 14.6f;
        const float blockerHeight = 11.8f;
        const float sideSegmentWidth = 3.1f;
        const float topBottomHeight = 3.2f;
        const float bridgeGapHeight = 2.8f;

        if (!GameProgress.HasBuiltFourthForestRootBridge)
        {
            DestroyGeneratedChild(boundaryRoot, "GeneratedFourthForestRiverBlocker_Top");
            DestroyGeneratedChild(boundaryRoot, "GeneratedFourthForestRiverBlocker_Bottom");
            DestroyGeneratedChild(boundaryRoot, "GeneratedFourthForestRiverBlocker_Right");
            DestroyGeneratedChild(boundaryRoot, "GeneratedFourthForestRiverBlocker_LeftTop");
            DestroyGeneratedChild(boundaryRoot, "GeneratedFourthForestRiverBlocker_LeftBottom");
            EnsureBoundaryCollider(
                "GeneratedFourthForestRiverBlocker",
                boundaryRoot,
                new Vector2(FourthForestRiverOffset.x, FourthForestRiverOffset.y),
                new Vector2(blockerWidth, blockerHeight));
            return;
        }

        DestroyGeneratedChild(boundaryRoot, "GeneratedFourthForestRiverBlocker");

        float halfWidth = blockerWidth * 0.5f;
        float halfHeight = blockerHeight * 0.5f;
        float topBottomCenterOffset = halfHeight - topBottomHeight * 0.5f;
        float sideHeight = blockerHeight - topBottomHeight * 2f;
        float sideSegmentHeight = Mathf.Max(0.1f, (sideHeight - bridgeGapHeight) * 0.5f);
        float sideSegmentCenterOffset = bridgeGapHeight * 0.5f + sideSegmentHeight * 0.5f;
        float sideCenterOffset = halfWidth - sideSegmentWidth * 0.5f;

        EnsureBoundaryCollider(
            "GeneratedFourthForestRiverBlocker_Top",
            boundaryRoot,
            new Vector2(FourthForestRiverOffset.x, FourthForestRiverOffset.y + topBottomCenterOffset),
            new Vector2(blockerWidth, topBottomHeight));
        EnsureBoundaryCollider(
            "GeneratedFourthForestRiverBlocker_Bottom",
            boundaryRoot,
            new Vector2(FourthForestRiverOffset.x, FourthForestRiverOffset.y - topBottomCenterOffset),
            new Vector2(blockerWidth, topBottomHeight));
        EnsureBoundaryCollider(
            "GeneratedFourthForestRiverBlocker_Right",
            boundaryRoot,
            new Vector2(FourthForestRiverOffset.x + sideCenterOffset, FourthForestRiverOffset.y),
            new Vector2(sideSegmentWidth, sideHeight));
        EnsureBoundaryCollider(
            "GeneratedFourthForestRiverBlocker_LeftTop",
            boundaryRoot,
            new Vector2(FourthForestRiverOffset.x - sideCenterOffset, FourthForestRiverOffset.y + sideSegmentCenterOffset),
            new Vector2(sideSegmentWidth, sideSegmentHeight));
        EnsureBoundaryCollider(
            "GeneratedFourthForestRiverBlocker_LeftBottom",
            boundaryRoot,
            new Vector2(FourthForestRiverOffset.x - sideCenterOffset, FourthForestRiverOffset.y - sideSegmentCenterOffset),
            new Vector2(sideSegmentWidth, sideSegmentHeight));
    }

    private void EnsureFourthForestRootBridgeState()
    {
        if (!GameProgress.HasBuiltFourthForestRootBridge)
        {
            return;
        }

        SketchWorldLineDrawing bridgeDrawing = CreateFourthForestRootBridgeDrawing(true);
        if (bridgeDrawing != null)
        {
            bridgeDrawing.RevealProgress = 1f;
        }

        EnsureFourthForestRiverBlockerState();
    }

    private void EnsureFourthForestIslandDoorState()
    {
        if (!GameProgress.HasColoredWaterBlue || !GameProgress.HasDrawnFourthForestSketch || propRoot == null)
        {
            return;
        }

        Transform doorTransform = propRoot.transform.Find("GeneratedFourthForestIslandDoor");
        SketchWorldLineDrawing doorDrawing = doorTransform != null
            ? doorTransform.GetComponent<SketchWorldLineDrawing>()
            : null;

        if (doorDrawing == null && doorTransform == null)
        {
            doorDrawing = CreateLineDrawing(
                "GeneratedFourthForestIslandDoor",
                FourthForestIslandDoorOffset,
                BuildFourthForestIslandDoorStrokes(),
                FourthForestIslandDoorSortingOrder,
                propRoot.transform,
                new Color32(35, 32, 28, 255));
            doorDrawing.Configure(HouseLineWidth * 1.18f, new Color32(35, 32, 28, 255), FourthForestIslandDoorSortingOrder);
        }
        else if (doorDrawing == null)
        {
            doorTransform.localPosition = FourthForestIslandDoorOffset;
            doorDrawing = doorTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
            doorDrawing.SetStrokes(BuildFourthForestIslandDoorStrokes());
            doorDrawing.Configure(HouseLineWidth * 1.18f, new Color32(35, 32, 28, 255), FourthForestIslandDoorSortingOrder);
        }
        else
        {
            doorDrawing.transform.localPosition = FourthForestIslandDoorOffset;
            doorDrawing.SetStrokes(BuildFourthForestIslandDoorStrokes());
            doorDrawing.Configure(HouseLineWidth * 1.18f, new Color32(35, 32, 28, 255), FourthForestIslandDoorSortingOrder);
        }

        doorDrawing.RevealProgress = 1f;
        ApplyFourthForestIslandDoorCrayonColors(doorDrawing.transform);

        FourthForestIslandDoorInteract doorInteract = doorDrawing.GetComponent<FourthForestIslandDoorInteract>();
        if (doorInteract == null)
        {
            doorInteract = doorDrawing.gameObject.AddComponent<FourthForestIslandDoorInteract>();
        }

        doorInteract.Configure(1.85f, FourthForestIslandDoorSortingOrder + 8);
    }

    private void ApplyFourthForestIslandDoorCrayonColors(Transform doorTransform)
    {
        if (propRoot == null)
        {
            return;
        }

        if (doorTransform == null)
        {
            doorTransform = propRoot.transform.Find("GeneratedFourthForestIslandDoor");
        }

        if (doorTransform == null)
        {
            return;
        }

        if (HasGreenCrayonColorForIslandDoor())
        {
            CreateOrUpdateFourthForestIslandDoorOverlay(
                doorTransform,
                "GeneratedFourthForestIslandDoorGreenOverlay",
                BuildFourthForestIslandDoorGreenStrokes(),
                VillageGreenColor,
                FourthForestIslandDoorSortingOrder + 2,
                1.52f);
        }

        if (HasBrownCrayonColorForIslandDoor())
        {
            CreateOrUpdateFourthForestIslandDoorOverlay(
                doorTransform,
                "GeneratedFourthForestIslandDoorBrownOverlay",
                BuildFourthForestIslandDoorBrownStrokes(),
                BrownCrayonColor,
                FourthForestIslandDoorSortingOrder + 1,
                1.32f);
        }

        if (HasBlueCrayonColorForIslandDoor())
        {
            CreateOrUpdateFourthForestIslandDoorOverlay(
                doorTransform,
                "GeneratedFourthForestIslandDoorBlueOverlay",
                BuildFourthForestIslandDoorBlueStrokes(),
                BlueCrayonColor,
                FourthForestIslandDoorSortingOrder + 3,
                1.18f);
        }
    }

    private void CreateOrUpdateFourthForestIslandDoorOverlay(
        Transform doorTransform,
        string objectName,
        List<Vector3[]> strokes,
        Color color,
        int sortingOrder,
        float lineWidthMultiplier)
    {
        Transform overlayTransform = doorTransform.Find(objectName);
        SketchWorldLineDrawing overlayDrawing = overlayTransform != null
            ? overlayTransform.GetComponent<SketchWorldLineDrawing>()
            : null;

        if (overlayDrawing == null && overlayTransform == null)
        {
            GameObject overlayObject = new GameObject(objectName);
            overlayObject.transform.SetParent(doorTransform, false);
            overlayDrawing = overlayObject.AddComponent<SketchWorldLineDrawing>();
        }
        else if (overlayDrawing == null)
        {
            overlayDrawing = overlayTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        }

        overlayDrawing.Configure(HouseLineWidth * lineWidthMultiplier, color, sortingOrder);
        overlayDrawing.SetStrokes(strokes);
        overlayDrawing.RevealProgress = 1f;
    }

    private static bool HasGreenCrayonColorForIslandDoor()
    {
        return GameProgress.HasCollectedGreenCrayon
            || GameProgress.HasUsedGreenCrayon
            || GameProgress.HasColoredVillageGreen;
    }

    private static bool HasBrownCrayonColorForIslandDoor()
    {
        return GameProgress.HasCollectedBrownCrayon
            || GameProgress.HasUsedBrownCrayon
            || GameProgress.HasColoredBrownDetails;
    }

    private static bool HasBlueCrayonColorForIslandDoor()
    {
        return GameProgress.HasCollectedBlueCrayon
            || GameProgress.HasUsedBlueCrayon
            || GameProgress.HasColoredWaterBlue;
    }

    private IEnumerator PlayFourthForestRootBridgeRoutine()
    {
        if (!GameProgress.HasInspectedFourthForestRiver || GameProgress.HasBuiltFourthForestRootBridge || propRoot == null)
        {
            yield break;
        }

        SketchWorldLineDrawing bridgeDrawing = CreateFourthForestRootBridgeDrawing(false);
        if (bridgeDrawing == null)
        {
            yield break;
        }

        bridgeDrawing.RevealProgress = 0f;
        const float growDuration = 1.34f;
        float elapsed = 0f;

        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / growDuration);
            bridgeDrawing.RevealProgress = Mathf.SmoothStep(0f, 1f, progress);
            yield return null;
        }

        bridgeDrawing.RevealProgress = 1f;
        GameProgress.BuildFourthForestRootBridge();
        EnsureFourthForestRiverBlockerState();
    }

    private SketchWorldLineDrawing CreateFourthForestRootBridgeDrawing(bool revealed)
    {
        if (propRoot == null)
        {
            return null;
        }

        Transform existing = propRoot.transform.Find("GeneratedFourthForestRootBridge");
        SketchWorldLineDrawing bridgeDrawing = existing != null
            ? existing.GetComponent<SketchWorldLineDrawing>()
            : null;

        if (bridgeDrawing == null)
        {
            bridgeDrawing = CreateLineDrawing(
                "GeneratedFourthForestRootBridge",
                Vector3.zero,
                BuildFourthForestRootBridgeStrokes(),
                FourthForestRootBridgeSortingOrder,
                propRoot.transform,
                BrownCrayonColor);
            bridgeDrawing.Configure(HouseLineWidth * 2.32f, BrownCrayonColor, FourthForestRootBridgeSortingOrder);
        }
        else
        {
            bridgeDrawing.SetStrokes(BuildFourthForestRootBridgeStrokes());
            bridgeDrawing.Configure(HouseLineWidth * 2.32f, BrownCrayonColor, FourthForestRootBridgeSortingOrder);
        }

        bridgeDrawing.RevealProgress = revealed ? 1f : bridgeDrawing.RevealProgress;
        return bridgeDrawing;
    }

    private SketchWorldLineDrawing CreateBlueLineOverlay(
        string objectName,
        Vector3 localPosition,
        List<Vector3[]> strokes,
        int sortingOrder,
        Transform parent,
        SketchWorldLineDrawing sourceDrawing,
        float lineWidthMultiplier,
        float revealDistance,
        params Vector3[] revealPositions)
    {
        if (parent == null || parent.Find(objectName) != null)
        {
            return null;
        }

        SketchWorldLineDrawing overlay = CreateLineDrawing(objectName, localPosition, strokes, sortingOrder, parent, BlueCrayonColor);
        overlay.Configure(HouseLineWidth * lineWidthMultiplier, BlueCrayonColor, sortingOrder);

        if (sourceDrawing != null && sourceDrawing.RevealProgress >= 0.95f)
        {
            overlay.RevealProgress = 1f;
        }
        else
        {
            Vector3[] triggerPositions = revealPositions != null && revealPositions.Length > 0
                ? revealPositions
                : new[] { localPosition };
            RegisterLineReveal(overlay, revealDistance, triggerPositions);
        }

        return overlay;
    }

    private static bool IsGrassColoringTarget(string objectName)
    {
        return objectName.StartsWith("GeneratedGrass")
            || objectName == "GeneratedQuestNpcLetterGrass";
    }

    private void CreateGrassColorOverlay(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null || spriteRenderer.transform.Find("GeneratedGrassGreenOverlay") != null)
        {
            return;
        }

        Sprite overlaySprite = GetGrassColorOverlaySprite(spriteRenderer.sprite);
        if (overlaySprite == null)
        {
            return;
        }

        GameObject overlayObject = new GameObject("GeneratedGrassGreenOverlay");
        overlayObject.transform.SetParent(spriteRenderer.transform, false);
        overlayObject.transform.localPosition = Vector3.zero;
        overlayObject.transform.localRotation = Quaternion.identity;
        overlayObject.transform.localScale = Vector3.one;

        SpriteRenderer overlayRenderer = overlayObject.AddComponent<SpriteRenderer>();
        overlayRenderer.sprite = overlaySprite;
        overlayRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;

        Color overlayColor = VillageGrassLightGreenColor;
        overlayColor.a = 0.86f;
        overlayRenderer.color = overlayColor;

        GrassCrayonColorOverlay overlaySync = overlayObject.AddComponent<GrassCrayonColorOverlay>();
        overlaySync.Configure(spriteRenderer, overlayRenderer, overlayColor);
    }

    private static Sprite GetGrassColorOverlaySprite(Sprite sourceSprite)
    {
        if (sourceSprite == null)
        {
            return null;
        }

        if (GrassColorOverlaySpriteCache.TryGetValue(sourceSprite, out Sprite cachedSprite))
        {
            return cachedSprite;
        }

        Sprite overlaySprite = CreateGrassColorOverlaySprite(sourceSprite);
        GrassColorOverlaySpriteCache[sourceSprite] = overlaySprite;
        return overlaySprite;
    }

    private static void ApplyGrassLightGreenColor(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        Color color = VillageGrassLightGreenColor;
        color.a = spriteRenderer.color.a;
        spriteRenderer.color = color;
    }

    private static Sprite CreateGrassColorOverlaySprite(Sprite sourceSprite)
    {
        Texture2D sourceTexture = sourceSprite.texture;
        if (sourceTexture == null)
        {
            return null;
        }

        Rect sourceRect = sourceSprite.rect;
        int rectX = Mathf.RoundToInt(sourceRect.x);
        int rectY = Mathf.RoundToInt(sourceRect.y);
        int width = Mathf.RoundToInt(sourceRect.width);
        int height = Mathf.RoundToInt(sourceRect.height);

        Color32[] sourcePixels;
        try
        {
            sourcePixels = sourceTexture.GetPixels32();
        }
        catch (UnityException)
        {
            return sourceSprite;
        }

        Color32[] overlayPixels = new Color32[width * height];
        const int dilationRadius = 4;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte alpha = GetMaxAlphaInRadius(sourcePixels, sourceTexture.width, sourceTexture.height, rectX + x, rectY + y, dilationRadius);
                overlayPixels[y * width + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(alpha * 0.88f));
            }
        }

        Texture2D overlayTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        overlayTexture.name = $"{sourceSprite.name}_GreenCrayonOverlayTexture";
        overlayTexture.filterMode = sourceTexture.filterMode;
        overlayTexture.wrapMode = TextureWrapMode.Clamp;
        overlayTexture.SetPixels32(overlayPixels);
        overlayTexture.Apply();

        Vector2 normalizedPivot = new Vector2(
            sourceSprite.pivot.x / Mathf.Max(1f, sourceRect.width),
            sourceSprite.pivot.y / Mathf.Max(1f, sourceRect.height));

        Sprite overlaySprite = Sprite.Create(
            overlayTexture,
            new Rect(0f, 0f, width, height),
            normalizedPivot,
            sourceSprite.pixelsPerUnit);
        overlaySprite.name = $"{sourceSprite.name}_GreenCrayonOverlay";
        return overlaySprite;
    }

    private static byte GetMaxAlphaInRadius(Color32[] pixels, int textureWidth, int textureHeight, int centerX, int centerY, int radius)
    {
        byte maxAlpha = 0;

        for (int y = centerY - radius; y <= centerY + radius; y++)
        {
            if (y < 0 || y >= textureHeight)
            {
                continue;
            }

            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                if (x < 0 || x >= textureWidth)
                {
                    continue;
                }

                byte alpha = pixels[y * textureWidth + x].a;
                if (alpha > maxAlpha)
                {
                    maxAlpha = alpha;
                }
            }
        }

        return maxAlpha;
    }

    private static bool IsInsideVillageFence(Vector3 localPosition)
    {
        const float margin = 0.55f;
        float halfWidth = BackgroundSize.x * 0.5f - margin;
        float halfHeight = BackgroundSize.y * 0.5f - margin;

        return localPosition.x >= -halfWidth
            && localPosition.x <= halfWidth
            && localPosition.y >= -halfHeight
            && localPosition.y <= halfHeight;
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
        questNpcRewardInteract = questNpcDrawing.gameObject.AddComponent<QuestNpcLetterRewardInteract>();
        questNpcRewardInteract.Configure(this, QuestNpcPromptDistance);
        questNpcRewardInteract.enabled = false;
        RegisterLineReveal(questNpcDrawing, questNpcRewardInteract, npcPosition);

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
        textMesh.text = QuestNpcSearchingPrompt;
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
        Color fenceColor = new Color32(86, 58, 34, 255);
        bool hasForestGate = GameProgress.HasDrawnForestSketch;
        rightFenceDrawing = CreateLineDrawing("GeneratedOutsideFence_Oval", Vector3.zero, hasForestGate ? BuildFenceRightGateStrokes() : BuildFenceRightStrokes(), PropSortingOrder + 3, propRoot.transform, fenceColor);
        RegisterLineReveal(rightFenceDrawing, FenceRevealDistance, BuildOvalFenceRevealPoints(hasForestGate));

        GameObject boundaryRootObject = new GameObject("GeneratedOutsideBoundaryColliders");
        boundaryRootObject.transform.SetParent(propRoot.transform, false);
        boundaryRoot = boundaryRootObject.transform;
        villageGateBoundaryColliders.Clear();
        CreateOvalBoundaryColliders(boundaryRoot, hasForestGate);
    }

    private void CreateOutsidePaths()
    {
        Vector3[][] centerLines = BuildOutsidePathCenterLines();

        CreateOutsidePathDrawing(
            "GeneratedOutsidePath_ToNeighborHouse",
            centerLines[0]);
        CreateOutsidePathDrawing(
            "GeneratedOutsidePath_ToPond",
            centerLines[1]);
        CreateOutsidePathDrawing(
            "GeneratedOutsidePath_ToForestEntrance",
            centerLines[2]);
    }

    private void CreateOutsidePathDrawing(string objectName, Vector3[] centerPoints)
    {
        Color pathColor = new Color32(132, 98, 61, 175);
        SketchWorldLineDrawing pathDrawing = CreateLineDrawing(
            objectName,
            Vector3.zero,
            BuildOutsidePathStrokes(centerPoints),
            PathSortingOrder,
            propRoot.transform,
            pathColor);
        pathDrawing.Configure(HouseLineWidth * 1.28f, pathColor, PathSortingOrder);
        RegisterLineReveal(pathDrawing, PathRevealDistance, centerPoints);
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
        fishingRodInteract.Configure(PondOffset, PropSortingOrder + 50, pondDrawing);
        fishingRodInteract.enabled = false;

        RegisterLineReveal(fishingRodDrawing, fishingRodInteract, fishingRodPosition);
    }

    private void CreateQuestNpcPondGuide()
    {
        questNpcPondGuideDrawing = CreateLineDrawing(
            "GeneratedQuestNpcPondGuide",
            Vector3.zero,
            BuildQuestNpcPondGuideStrokes(),
            QuestGuideSortingOrder,
            propRoot.transform,
            new Color32(95, 80, 44, 210));
        questNpcPondGuideDrawing.RevealProgress = 0f;
    }

    private void CreateLetterFragmentEvents()
    {
        CreateLetterFragmentEvent(
            "GeneratedLetterFragmentEvent_WindTrace",
            LetterFragmentSource.WindTrace,
            WindLetterFragmentOffset,
            BuildWindTraceLetterFragmentStrokes());

        CreateLetterFragmentEvent(
            "GeneratedLetterFragmentEvent_GrassPatch",
            LetterFragmentSource.GrassPatch,
            GrassLetterFragmentOffset,
            BuildGrassPatchLetterFragmentStrokes());

        CreateLetterFragmentEvent(
            "GeneratedLetterFragmentEvent_PondEdge",
            LetterFragmentSource.PondEdge,
            PondLetterFragmentOffset,
            BuildPondEdgeLetterFragmentStrokes());
    }

    private void CreateLetterFragmentEvent(string objectName, LetterFragmentSource source, Vector3 localPosition, List<Vector3[]> strokes)
    {
        SketchWorldLineDrawing letterDrawing = CreateLineDrawing(
            objectName,
            localPosition,
            strokes,
            LetterFragmentSortingOrder,
            propRoot.transform,
            new Color32(75, 58, 39, 255));

        LetterFragmentInteract letterInteract = letterDrawing.gameObject.AddComponent<LetterFragmentInteract>();
        letterInteract.Configure(source, LetterFragmentPromptDistance, LetterFragmentPromptSortingOrder);
        letterInteract.enabled = false;

        RegisterLineReveal(letterDrawing, LetterFragmentRevealDistance, letterInteract, localPosition);
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

    private BoxCollider2D CreateBoundaryCollider(string objectName, Transform parent, Vector2 localPosition, Vector2 size)
    {
        GameObject colliderObject = new GameObject(objectName);
        colliderObject.transform.SetParent(parent, false);
        colliderObject.transform.localPosition = localPosition;

        BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>();
        collider.size = size;
        collider.isTrigger = false;
        return collider;
    }

    private BoxCollider2D EnsureBoundaryCollider(string objectName, Transform parent, Vector2 localPosition, Vector2 size)
    {
        if (parent == null)
        {
            return null;
        }

        Transform existing = parent.Find(objectName);
        if (existing == null)
        {
            return CreateBoundaryCollider(objectName, parent, localPosition, size);
        }

        existing.localPosition = localPosition;
        BoxCollider2D collider = existing.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = existing.gameObject.AddComponent<BoxCollider2D>();
        }

        collider.size = size;
        collider.isTrigger = false;
        collider.enabled = true;
        return collider;
    }

    private void CreateOvalBoundaryColliders(Transform parent, bool leaveForestGateOpen)
    {
        if (parent == null)
        {
            return;
        }

        for (int i = 0; i < OvalFenceColliderSegmentCount; i++)
        {
            float startAngle = Mathf.PI * 2f * i / OvalFenceColliderSegmentCount;
            float endAngle = Mathf.PI * 2f * (i + 1) / OvalFenceColliderSegmentCount;
            float colliderThickness = OvalFenceColliderOuterOutset + OvalFenceColliderInnerInset;
            float colliderNormalOffset = (OvalFenceColliderOuterOutset - OvalFenceColliderInnerInset) * 0.5f;
            Vector2 baseStart = GetOvalFenceColliderPoint(startAngle);
            Vector2 baseEnd = GetOvalFenceColliderPoint(endAngle);
            Vector2 start = GetOvalFenceColliderPoint(startAngle, colliderNormalOffset);
            Vector2 end = GetOvalFenceColliderPoint(endAngle, colliderNormalOffset);
            Vector2 baseMidpoint = (baseStart + baseEnd) * 0.5f;
            bool isGateSegment = IsVillageForestGatePoint(new Vector3(baseMidpoint.x, baseMidpoint.y, 0f));

            if (leaveForestGateOpen && isGateSegment)
            {
                continue;
            }

            BoxCollider2D collider = CreateBoundarySegmentCollider(
                $"GeneratedOutsideBoundary_Oval_{i:00}",
                parent,
                start,
                end,
                colliderThickness);

            if (isGateSegment)
            {
                villageGateBoundaryColliders.Add(collider);
            }
        }
    }

    private BoxCollider2D CreateBoundarySegmentCollider(string objectName, Transform parent, Vector2 start, Vector2 end, float thickness)
    {
        GameObject colliderObject = new GameObject(objectName);
        colliderObject.transform.SetParent(parent, false);

        Vector2 midpoint = (start + end) * 0.5f;
        Vector2 delta = end - start;
        float length = Mathf.Max(0.1f, delta.magnitude);

        colliderObject.transform.localPosition = new Vector3(midpoint.x, midpoint.y, 0f);
        colliderObject.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);

        BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(length + 0.08f, thickness);
        collider.isTrigger = false;
        return collider;
    }

    private static Vector2 GetOvalFenceColliderPoint(float angle, float normalOffset = 0f)
    {
        Vector2 point = new Vector2(
            Mathf.Cos(angle) * OutsideOvalFenceRadius.x,
            Mathf.Sin(angle) * OutsideOvalFenceRadius.y);

        return point + GetOvalFenceNormal(angle) * normalOffset;
    }

    private static float GetOutsideOvalRightXAtY(float y, float inset = 0f)
    {
        float radiusX = Mathf.Max(0.1f, OutsideOvalFenceRadius.x - inset);
        float radiusY = Mathf.Max(0.1f, OutsideOvalFenceRadius.y - inset);
        float normalizedY = Mathf.Clamp(Mathf.Abs(y) / radiusY, 0f, 1f);
        return Mathf.Sqrt(Mathf.Max(0f, 1f - normalizedY * normalizedY)) * radiusX;
    }

    private static float GetOutsideOvalExteriorRightXAtYBand(float centerY, float halfBandHeight)
    {
        float closestToCenterY = Mathf.Max(0f, Mathf.Abs(centerY) - Mathf.Max(0f, halfBandHeight));
        return GetOutsideOvalRightXAtY(closestToCenterY) + halfBandHeight;
    }

    private static void SetCollidersEnabled(List<Collider2D> colliders, bool enabled)
    {
        if (colliders == null)
        {
            return;
        }

        for (int i = 0; i < colliders.Count; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = enabled;
            }
        }
    }

    private void CreateRandomOutsideDecorations()
    {
        System.Random random = new System.Random(DecorationRandomSeed);
        List<DecorationFootprint> usedFootprints = new List<DecorationFootprint>();
        LetterQuestTreeRoute.ResetRoute();
        LetterQuestWorldDrop.Reset();
        LetterQuestWorldDrop.ConfigureFinalGrassTarget(GetQuestNpcPosition(), propRoot.transform);

        CreateQuestNpcLetterGrass(usedFootprints);
        SpawnRandomPineTrees("GeneratedPineTree", TreeSpawnCount, 6.2f, TreeScaleRange.x, TreeScaleRange.y, TreeSortingOrder, random, usedFootprints, -31f, 31f, DecorationMin.y, 12.5f);
        SpawnRandomSpriteProps("GeneratedGrass", GrassResourcePath, GrassSpawnCount, 2.1f, GrassScaleRange.x, GrassScaleRange.y, GrassSortingOrder, random, usedFootprints, DecorationMin.x, DecorationMax.x, DecorationMin.y, DecorationMax.y);
        CreateOvalFillDecorations(random, usedFootprints);
        CreateOuterDecorativePines(
            "GeneratedOuterDecorativePine_Village",
            Vector3.zero,
            OutsideOvalFenceRadius * 2f,
            DecorationRandomSeed + 263,
            false,
            OuterDecorativeTreeRevealDistance);
    }

    private void CreateOvalFillDecorations(System.Random random, List<DecorationFootprint> usedFootprints)
    {
        if (propRoot == null)
        {
            return;
        }

        for (int i = 0; i < OvalFillPineCount; i++)
        {
            float scale = RandomRange(random, TreeScaleRange.x * 0.78f, TreeScaleRange.y * 0.96f);
            Vector2 halfSize = GetPineDecorationHalfSize(scale);

            if (!TryPickOvalFillDecorationPosition(random, usedFootprints, halfSize, 3.35f, 0.5f, out Vector2 position))
            {
                continue;
            }

            CreateDecorativePineTree(
                $"GeneratedOvalFillPine_{i:00}",
                new Vector3(position.x, position.y, 0f),
                scale,
                TreeSortingOrder,
                propRoot.transform);
            usedFootprints.Add(BuildDecorationFootprint(position, halfSize));
        }

        Sprite grassSprite = LoadSketchSprite(GrassResourcePath);
        if (grassSprite == null)
        {
            Debug.LogWarning($"Outside prop sprite not found: {GrassResourcePath}", this);
            return;
        }

        for (int i = 0; i < OvalFillGrassCount; i++)
        {
            float scale = RandomRange(random, GrassScaleRange.x * 0.92f, GrassScaleRange.y * 1.08f);
            float scaleX = random.NextDouble() < 0.5d ? -scale : scale;
            Vector3 localScale = new Vector3(scaleX, scale, 1f);
            Vector2 halfSize = GetDecorationHalfSize(grassSprite, localScale);

            if (!TryPickOvalFillDecorationPosition(random, usedFootprints, halfSize, 1.05f, 0.34f, out Vector2 position))
            {
                continue;
            }

            Transform grassTransform = CreateSpriteProp(
                $"GeneratedGrassOvalFill_{i:00}",
                grassSprite,
                new Vector3(position.x, position.y, 0f),
                localScale,
                GrassSortingOrder,
                propRoot.transform,
                true);

            if (grassTransform != null)
            {
                GrassSwayOnPlayerNear grassSway = grassTransform.GetComponent<GrassSwayOnPlayerNear>();
                if (grassSway != null)
                {
                    grassSway.SuppressRewardSpawns();
                }
            }

            usedFootprints.Add(BuildDecorationFootprint(position, halfSize));
        }
    }

    private void CreateQuestNpcLetterGrass(List<DecorationFootprint> usedFootprints)
    {
        Sprite sprite = LoadSketchSprite(GrassResourcePath);
        if (sprite == null)
        {
            Debug.LogWarning($"Outside prop sprite not found: {GrassResourcePath}", this);
            return;
        }

        Vector3 localPosition = GetQuestNpcPosition() + QuestNpcLetterGrassOffset;
        float scale = GrassScaleRange.y;
        Vector3 localScale = new Vector3(scale, scale, 1f);
        Transform grassTransform = CreateSpriteProp(
            "GeneratedQuestNpcLetterGrass",
            sprite,
            localPosition,
            localScale,
            GrassSortingOrder,
            propRoot.transform,
            true);

        if (grassTransform == null)
        {
            return;
        }

        GrassSwayOnPlayerNear grassSway = grassTransform.GetComponent<GrassSwayOnPlayerNear>();
        if (grassSway != null)
        {
            grassSway.MarkReservedForLetterFragment();
        }

        LetterQuestWorldDrop.RegisterGrassPatch(localPosition, propRoot.transform, grassTransform);

        Vector2 halfSize = GetDecorationHalfSize(sprite, localScale);
        usedFootprints.Add(BuildDecorationFootprint(new Vector2(localPosition.x, localPosition.y), halfSize));
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
        int nonRouteTreeIndex = 0;

        for (int i = 0; i < count; i++)
        {
            float scale = RandomRange(random, minScale, maxScale);
            Vector2 halfSize = GetPineDecorationHalfSize(scale);

            if (!TryPickDecorationPosition(random, usedFootprints, halfSize, minimumSpacing, minX, maxX, minY, maxY, out Vector2 position))
            {
                continue;
            }

            DecorationFootprint footprint = BuildDecorationFootprint(position, halfSize);

            PineTreeShakeInteract treeInteract = CreatePineTreeProp(
                $"{objectPrefix}_{i:00}",
                new Vector3(position.x, position.y, 0f),
                scale,
                sortingOrder,
                propRoot.transform);

            if (treeInteract != null)
            {
                int routeIndex = LetterQuestTreeRoute.RegisterTree(
                    new Vector3(position.x, position.y, 0f),
                    propRoot.transform,
                    treeInteract.GetComponent<SketchWorldLineDrawing>());
                if (routeIndex >= 0)
                {
                    treeInteract.ConfigureLetterRouteIndex(routeIndex);
                }
                else
                {
                    bool canGeneralTreeInteract = nonRouteTreeIndex % 3 == 0;
                    treeInteract.ConfigureGeneralInteraction(canGeneralTreeInteract, canGeneralTreeInteract, sortingOrder + 1);
                    nonRouteTreeIndex++;
                }
            }

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

            Transform propTransform = CreateSpriteProp(
                $"{objectPrefix}_{i:00}",
                sprite,
                new Vector3(position.x, position.y, 0f),
                localScale,
                sortingOrder,
                propRoot.transform,
                objectPrefix == "GeneratedGrass");

            if (objectPrefix == "GeneratedGrass" && propTransform != null)
            {
                LetterQuestWorldDrop.RegisterGrassPatch(new Vector3(position.x, position.y, 0f), propRoot.transform, propTransform);
            }

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

            if (DoesFootprintOverlapOutsidePath(candidate))
            {
                continue;
            }

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

    private bool TryPickOvalFillDecorationPosition(
        System.Random random,
        List<DecorationFootprint> usedFootprints,
        Vector2 halfSize,
        float minimumSpacing,
        float minimumNormalizedDistance,
        out Vector2 position)
    {
        float minimumSpacingSqr = minimumSpacing * minimumSpacing;
        float margin = Mathf.Max(halfSize.x, halfSize.y) + 0.55f;
        float minX = -OutsideOvalFenceRadius.x + margin;
        float maxX = OutsideOvalFenceRadius.x - margin;
        float minY = -OutsideOvalFenceRadius.y + margin;
        float maxY = OutsideOvalFenceRadius.y - margin;

        for (int attempt = 0; attempt < DecorationPickAttempts; attempt++)
        {
            position = new Vector2(
                RandomRange(random, minX, maxX),
                RandomRange(random, minY, maxY));

            if (!IsInsideOutsideOval(position, margin)
                || GetOutsideOvalNormalizedDistance(position) < minimumNormalizedDistance
                || IsVillageForestGateFillBlocked(position)
                || IsInsideClearZone(position))
            {
                continue;
            }

            DecorationFootprint candidate = BuildDecorationFootprint(position, halfSize);
            if (DoesFootprintOverlapOutsidePath(candidate))
            {
                continue;
            }

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

    private static bool IsInsideOutsideOval(Vector2 position, float inset)
    {
        float radiusX = Mathf.Max(0.1f, OutsideOvalFenceRadius.x - inset);
        float radiusY = Mathf.Max(0.1f, OutsideOvalFenceRadius.y - inset);
        return (position.x * position.x) / (radiusX * radiusX)
            + (position.y * position.y) / (radiusY * radiusY) <= 1f;
    }

    private static float GetOutsideOvalNormalizedDistance(Vector2 position)
    {
        float radiusX = Mathf.Max(0.1f, OutsideOvalFenceRadius.x);
        float radiusY = Mathf.Max(0.1f, OutsideOvalFenceRadius.y);
        return (position.x * position.x) / (radiusX * radiusX)
            + (position.y * position.y) / (radiusY * radiusY);
    }

    private static bool IsVillageForestGateFillBlocked(Vector2 position)
    {
        return position.x > OutsideOvalFenceRadius.x * 0.64f
            && Mathf.Abs(position.y) <= ForestGateHalfHeight + 3.4f;
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

    private static bool DoesFootprintOverlapOutsidePath(DecorationFootprint footprint)
    {
        Vector3[][] centerLines = BuildOutsidePathCenterLines();
        Vector2 footprintCenter = footprint.Center;
        float blockingRadius = footprint.HalfSize.magnitude + PathDecorationClearance;

        for (int pathIndex = 0; pathIndex < centerLines.Length; pathIndex++)
        {
            Vector3[] centerLine = centerLines[pathIndex];

            for (int pointIndex = 0; pointIndex < centerLine.Length - 1; pointIndex++)
            {
                Vector2 segmentStart = centerLine[pointIndex];
                Vector2 segmentEnd = centerLine[pointIndex + 1];

                if (DistanceToSegment(footprintCenter, segmentStart, segmentEnd) <= blockingRadius)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static float DistanceToSegment(Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
    {
        Vector2 segment = segmentEnd - segmentStart;
        float segmentLengthSqr = segment.sqrMagnitude;

        if (segmentLengthSqr <= 0.0001f)
        {
            return Vector2.Distance(point, segmentStart);
        }

        float t = Mathf.Clamp01(Vector2.Dot(point - segmentStart, segment) / segmentLengthSqr);
        Vector2 closestPoint = segmentStart + segment * t;
        return Vector2.Distance(point, closestPoint);
    }

    private static bool IsInsideClearZone(Vector2 position)
    {
        return IsInsideBox(position, HouseOffset, new Vector2(5.4f, 4.2f))
            || IsInsideBox(position, NeighborHouseOffset, new Vector2(5.2f, 3.8f))
            || IsInsideBox(position, ForestEntranceOffset, new Vector2(3.6f, 3.2f))
            || IsInsideBox(position, PondOffset, new Vector2(7.4f, 4.6f))
            || IsInsideBox(position, GetQuestNpcPosition(), new Vector2(2.2f, 2.6f))
            || IsInsideBox(position, WindLetterFragmentOffset, new Vector2(2.4f, 2.2f))
            || IsInsideBox(position, GrassLetterFragmentOffset, new Vector2(2.4f, 2.2f))
            || IsInsideBox(position, PondLetterFragmentOffset, new Vector2(2.4f, 2.2f))
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

    private Transform CreateSpriteProp(string objectName, string resourcePath, Vector3 localPosition, Vector3 localScale, int sortingOrder, Transform parent = null, bool addGrassSway = false)
    {
        Sprite sprite = LoadSketchSprite(resourcePath);
        if (sprite == null)
        {
            Debug.LogWarning($"Outside prop sprite not found: {resourcePath}", this);
            return null;
        }

        return CreateSpriteProp(objectName, sprite, localPosition, localScale, sortingOrder, parent, addGrassSway);
    }

    private Transform CreateSpriteProp(string objectName, Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Transform parent = null, bool addGrassSway = false)
    {
        GameObject propObject = new GameObject(objectName);
        propObject.transform.SetParent(parent != null ? parent : contentRoot.transform, false);
        propObject.transform.localScale = localScale;

        SpriteRenderer spriteRenderer = propObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = sortingOrder;
        if (GameProgress.HasColoredVillageGreen && IsGrassColoringTarget(objectName))
        {
            ApplyGrassLightGreenColor(spriteRenderer);
        }

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
        if (GameProgress.HasColoredVillageGreen && IsGrassColoringTarget(objectName))
        {
            CreateGrassColorOverlay(spriteRenderer);
        }

        CreatePropGroundStroke(objectName, localPosition, sprite.bounds.extents.x * Mathf.Abs(localScale.x), sortingOrder - 1, parent);
        return propObject.transform;
    }

    private PineTreeShakeInteract CreatePineTreeProp(string objectName, Vector3 localPosition, float scale, int sortingOrder, Transform parent = null)
    {
        SketchWorldLineDrawing pineTree = CreateLineDrawing(
            objectName,
            localPosition,
            BuildPineTreeStrokes(scale),
            sortingOrder,
            parent);
        TreeCrayonColorTarget colorTarget = pineTree.gameObject.AddComponent<TreeCrayonColorTarget>();
        colorTarget.Configure(scale);

        PineTreeShakeInteract treeInteract = pineTree.gameObject.AddComponent<PineTreeShakeInteract>();
        treeInteract.Configure(scale, sortingOrder + 47);
        treeInteract.enabled = false;

        RegisterLineReveal(pineTree, treeInteract, localPosition);
        CreatePropGroundStroke(objectName, localPosition, 1.16f * scale, sortingOrder - 1, parent);

        if (GameProgress.HasColoredVillageGreen)
        {
            CreateTreeLeafOverlay(colorTarget, pineTree, sortingOrder + 2);
        }

        if (GameProgress.HasColoredBrownDetails)
        {
            CreateTreeTrunkOverlay(colorTarget, pineTree, sortingOrder + 3);
        }

        return treeInteract;
    }

    private void CreatePropGroundStroke(string objectName, Vector3 localPosition, float halfWidth, int sortingOrder, Transform parent, bool revealImmediately = false, float revealDistance = MapRevealDistance)
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
        if (revealImmediately)
        {
            groundStroke.RevealProgress = 1f;
        }
        else
        {
            RegisterLineReveal(groundStroke, revealDistance, localPosition);
        }
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

    private static void ApplyOutsideCameraSize()
    {
        Camera mainCamera = Camera.main;

        SketchWorldLineDrawing.SetGlobalLineWidthMultiplier(OutsideCameraSizeMultiplier);

        if (mainCamera == null || !mainCamera.orthographic)
        {
            return;
        }

        if (!hasStoredInteriorCameraSize)
        {
            interiorCameraOrthographicSize = mainCamera.orthographicSize;
            hasStoredInteriorCameraSize = true;
        }

        mainCamera.orthographicSize = interiorCameraOrthographicSize * OutsideCameraSizeMultiplier;
    }

    private static void RestoreInteriorCameraSize()
    {
        Camera mainCamera = Camera.main;

        SketchWorldLineDrawing.SetGlobalLineWidthMultiplier(1f);

        if (mainCamera == null || !mainCamera.orthographic || !hasStoredInteriorCameraSize)
        {
            return;
        }

        mainCamera.orthographicSize = interiorCameraOrthographicSize;
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

    private static Vector3[] BuildHorizontalFenceRevealPoints(float y, float left, float right)
    {
        float width = Mathf.Max(0.1f, right - left);
        int segmentCount = Mathf.Max(1, Mathf.CeilToInt(width / FenceRevealTriggerSpacing));
        Vector3[] points = new Vector3[segmentCount + 1];

        for (int i = 0; i <= segmentCount; i++)
        {
            float x = Mathf.Lerp(left, right, (float)i / segmentCount);
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

    private static Vector3[][] BuildOutsidePathCenterLines()
    {
        return OutsidePathCenterLines;
    }

    private static Vector3[] BuildForestExtensionPathCenterLine()
    {
        float villageRight = BackgroundSize.x * 0.5f;

        return new[]
        {
            new Vector3(11.8f, -5.7f, 0f),
            new Vector3(16.6f, -6.1f, 0f),
            new Vector3(22.8f, -4.8f, 0f),
            new Vector3(29.0f, -2.2f, 0f),
            new Vector3(villageRight, 0f, 0f),
            ForestRegionOffset + new Vector3(-27.2f, 0.6f, 0f),
            ForestRegionOffset + new Vector3(-16.2f, -2.1f, 0f),
            ForestRegionOffset + new Vector3(-4.6f, 1.8f, 0f),
            ForestRegionOffset + new Vector3(6.2f, -1.2f, 0f),
            ForestRegionOffset + new Vector3(18.4f, 2.5f, 0f),
            ForestRegionOffset + new Vector3(31.2f, 0.1f, 0f)
        };
    }

    private static Vector3[] BuildDeepForestExtensionPathCenterLine()
    {
        float forestRight = ForestRegionOffset.x + BackgroundSize.x * 0.5f;

        return new[]
        {
            ForestRegionOffset + new Vector3(31.2f, 0.1f, 0f),
            new Vector3(forestRight, 0f, 0f),
            DeepForestEntranceDirtOffset
        };
    }

    private static Vector3[] BuildFourthForestExtensionPathCenterLine()
    {
        float deepForestRight = DeepForestRegionOffset.x + DeepForestSize.x * 0.5f;

        return new[]
        {
            DeepForestEntranceDirtOffset,
            DeepForestRegionOffset + new Vector3(3.8f, -0.6f, 0f),
            DeepForestRegionOffset + new Vector3(12.6f, 0.4f, 0f),
            new Vector3(deepForestRight, 0f, 0f),
            FourthForestRegionOffset + new Vector3(-28.4f, 0.2f, 0f),
            FourthForestRegionOffset + new Vector3(-18.2f, -0.8f, 0f),
            FourthForestRegionOffset + new Vector3(-8.4f, 0.5f, 0f),
            FourthForestBigTreeOffset + new Vector3(-2.6f, 1.0f, 0f)
        };
    }

    private static List<Vector3[]> BuildDeepForestEntranceDirtStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(EllipsePoints(0f, 0f, 3.45f, 2.45f, 36));
        strokes.Add(EllipsePoints(0.16f, -0.06f, 2.62f, 1.72f, 30));
        strokes.Add(EllipsePoints(-0.18f, 0.08f, 1.54f, 0.96f, 24));
        strokes.Add(Points(-2.5f, 0.34f, -1.55f, 0.58f, -0.54f, 0.42f, 0.52f, 0.62f, 1.56f, 0.36f, 2.4f, 0.56f));
        strokes.Add(Points(-2.72f, -0.58f, -1.72f, -0.86f, -0.54f, -0.62f, 0.62f, -0.92f, 1.66f, -0.64f, 2.54f, -0.86f));
        strokes.Add(Points(-1.82f, 1.08f, -0.74f, 1.22f, 0.26f, 1.08f, 1.32f, 1.24f));
        strokes.Add(Points(-1.56f, -1.34f, -0.54f, -1.18f, 0.5f, -1.36f, 1.48f, -1.16f));

        return strokes;
    }

    private static List<Vector3[]> BuildOutsidePathStrokes(Vector3[] centerPoints)
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        if (centerPoints == null || centerPoints.Length < 2)
        {
            return strokes;
        }

        strokes.Add(OffsetPathPoints(centerPoints, 0.42f));
        strokes.Add(OffsetPathPoints(centerPoints, -0.42f));
        strokes.Add(OffsetPathPoints(centerPoints, 0.12f));
        AddOutsidePathTextureStrokes(strokes, centerPoints);
        return strokes;
    }

    private static Vector3[] OffsetPathPoints(Vector3[] centerPoints, float offset)
    {
        Vector3[] points = new Vector3[centerPoints.Length];

        for (int i = 0; i < centerPoints.Length; i++)
        {
            Vector3 previous = centerPoints[Mathf.Max(0, i - 1)];
            Vector3 next = centerPoints[Mathf.Min(centerPoints.Length - 1, i + 1)];
            Vector3 direction = next - previous;

            if (direction.sqrMagnitude < 0.001f)
            {
                direction = Vector3.right;
            }

            direction.Normalize();
            Vector3 normal = new Vector3(-direction.y, direction.x, 0f);
            points[i] = centerPoints[i] + normal * offset;
        }

        return points;
    }

    private static void AddOutsidePathTextureStrokes(List<Vector3[]> strokes, Vector3[] centerPoints)
    {
        for (int i = 0; i < centerPoints.Length - 1; i++)
        {
            Vector3 start = centerPoints[i];
            Vector3 end = centerPoints[i + 1];
            Vector3 direction = end - start;

            if (direction.sqrMagnitude < 0.001f)
            {
                continue;
            }

            direction.Normalize();
            Vector3 normal = new Vector3(-direction.y, direction.x, 0f);

            for (int markIndex = 0; markIndex < 2; markIndex++)
            {
                float t = markIndex == 0 ? 0.32f : 0.68f;
                Vector3 center = Vector3.Lerp(start, end, t);
                float side = markIndex == 0 ? -1f : 1f;
                Vector3 markStart = center + normal * side * 0.08f - direction * 0.32f;
                Vector3 markEnd = center + normal * side * 0.26f + direction * 0.22f;
                strokes.Add(Points(markStart.x, markStart.y, markEnd.x, markEnd.y));
            }
        }
    }

    private static List<Vector3[]> BuildQuestNpcPondGuideStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        Vector3 start = GetQuestNpcPosition() + new Vector3(-0.2f, 1.1f, 0f);
        Vector3 end = PondOffset + new Vector3(3.6f, 1.2f, 0f);

        strokes.Add(Points(start.x, start.y, end.x, end.y));
        strokes.Add(Points(end.x - 0.64f, end.y + 0.12f, end.x, end.y, end.x - 0.42f, end.y - 0.48f));

        return strokes;
    }

    private static List<Vector3[]> BuildWindTraceLetterFragmentStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(-2.15f, 0.55f, -1.48f, 0.78f, -0.76f, 0.58f, -0.14f, 0.76f, 0.58f, 0.52f));
        strokes.Add(Points(-1.9f, 0.12f, -1.22f, 0.32f, -0.48f, 0.12f, 0.1f, 0.28f, 0.86f, 0.05f));
        strokes.Add(Points(-1.45f, -0.28f, -0.82f, -0.08f, -0.26f, -0.22f, 0.28f, -0.1f));
        strokes.Add(Points(0.42f, -0.54f, 0.98f, -0.42f, 1.3f, -0.68f, 1.12f, -1.08f, 0.48f, -1.18f, 0.2f, -0.86f, 0.42f, -0.54f));
        strokes.Add(Points(0.52f, -0.82f, 0.98f, -0.76f));
        strokes.Add(Points(0.5f, -1.02f, 0.82f, -0.94f));

        return strokes;
    }

    private static List<Vector3[]> BuildGrassPatchLetterFragmentStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(-1.18f, -0.9f, -1.0f, -0.18f, -0.72f, -0.72f));
        strokes.Add(Points(-0.62f, -0.98f, -0.5f, -0.22f, -0.12f, -0.84f));
        strokes.Add(Points(0.74f, -0.94f, 0.54f, -0.22f, 0.18f, -0.74f));
        strokes.Add(Points(1.08f, -0.86f, 0.88f, -0.16f, 1.32f, -0.58f));
        strokes.Add(Points(-0.36f, -0.34f, 0.36f, -0.2f, 0.74f, -0.62f, 0.38f, -1.08f, -0.42f, -0.94f, -0.68f, -0.58f, -0.36f, -0.34f));
        strokes.Add(Points(-0.2f, -0.58f, 0.42f, -0.48f));
        strokes.Add(Points(-0.18f, -0.78f, 0.22f, -0.7f));

        return strokes;
    }

    private static List<Vector3[]> BuildPondEdgeLetterFragmentStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(-1.62f, -0.92f, -1.0f, -0.78f, -0.28f, -0.88f, 0.36f, -0.76f, 1.02f, -0.9f));
        strokes.Add(Points(-1.28f, -1.18f, -0.62f, -1.04f, 0.1f, -1.12f, 0.76f, -1.02f));
        strokes.Add(Points(-0.52f, -0.08f, 0.18f, 0.08f, 0.84f, -0.1f, 1.02f, -0.62f, 0.46f, -0.9f, -0.28f, -0.72f, -0.72f, -0.38f, -0.52f, -0.08f));
        strokes.Add(Points(-0.3f, -0.32f, 0.42f, -0.48f));
        strokes.Add(Points(-0.18f, -0.54f, 0.22f, -0.62f));
        strokes.Add(Points(0.78f, 0.18f, 1.12f, 0.48f, 1.48f, 0.28f));

        return strokes;
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

    private static List<Vector3[]> BuildPondBlueWaterStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(EllipsePoints(0.1f, 0f, 4.35f, 1.95f, 34));
        strokes.Add(Points(-3.25f, 0.62f, -2.0f, 0.86f, -0.7f, 0.58f, 0.65f, 0.82f, 2.12f, 0.52f));
        strokes.Add(Points(-3.7f, -0.16f, -2.38f, 0.08f, -1.05f, -0.2f, 0.22f, 0.04f, 1.6f, -0.18f, 3.05f, 0.05f));
        strokes.Add(Points(-2.78f, -0.95f, -1.36f, -0.68f, 0.08f, -0.96f, 1.48f, -0.7f, 2.8f, -0.92f));

        return strokes;
    }

    private static List<Vector3[]> BuildForestWellStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(EllipsePoints(0f, 0.24f, 1.35f, 0.42f, 28));
        strokes.Add(EllipsePoints(0f, 0.42f, 1.12f, 0.28f, 24));
        strokes.Add(Points(-1.34f, 0.22f, -1.1f, -1.02f, -0.72f, -1.25f, 0f, -1.32f, 0.74f, -1.22f, 1.1f, -0.98f, 1.34f, 0.22f));
        strokes.Add(Points(-1.04f, -0.22f, -0.48f, -0.36f, 0.02f, -0.24f, 0.58f, -0.38f, 1.04f, -0.2f));
        strokes.Add(Points(-0.92f, -0.72f, -0.32f, -0.84f, 0.22f, -0.7f, 0.84f, -0.84f));
        strokes.Add(Points(-0.88f, 0.58f, -0.88f, 1.9f));
        strokes.Add(Points(0.88f, 0.58f, 0.88f, 1.9f));
        strokes.Add(Points(-1.14f, 1.72f, -0.56f, 2.34f, 0f, 2.58f, 0.58f, 2.34f, 1.14f, 1.72f));
        strokes.Add(Points(-0.74f, 1.75f, -0.2f, 2.2f, 0.3f, 2.2f, 0.76f, 1.75f));
        strokes.Add(Points(-0.54f, 1.2f, 0.54f, 1.2f));
        strokes.Add(Points(0f, 1.2f, 0f, 0.28f));
        strokes.Add(Points(-0.26f, 0.18f, -0.18f, -0.22f, 0.18f, -0.22f, 0.26f, 0.18f, -0.26f, 0.18f));
        strokes.Add(Points(1.02f, 1.2f, 1.28f, 1.34f, 1.44f, 1.18f, 1.28f, 1.02f, 1.02f, 1.2f));
        strokes.Add(Points(-1.02f, 1.2f, -1.28f, 1.34f, -1.44f, 1.18f, -1.28f, 1.02f, -1.02f, 1.2f));

        return strokes;
    }

    private static List<Vector3[]> BuildForestWellBlueWaterStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(EllipsePoints(0f, 0.36f, 0.92f, 0.2f, 20));
        strokes.Add(Points(-0.68f, 0.38f, -0.34f, 0.48f, 0.02f, 0.36f, 0.36f, 0.48f, 0.72f, 0.36f));
        strokes.Add(Points(-0.48f, 0.18f, -0.12f, 0.08f, 0.24f, 0.18f, 0.52f, 0.08f));

        return strokes;
    }

    private static List<Vector3[]> BuildFourthForestRiverStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(EllipsePoints(0f, 0f, 7.05f, 5.58f, 54));
        strokes.Add(EllipsePoints(-0.08f, -0.04f, 3.36f, 2.54f, 42));
        strokes.Add(Points(-5.65f, 2.62f, -4.42f, 3.06f, -3.12f, 2.68f, -1.86f, 3.02f, -0.48f, 2.66f));
        strokes.Add(Points(1.02f, 3.02f, 2.34f, 2.62f, 3.82f, 3.04f, 5.34f, 2.56f));
        strokes.Add(Points(-5.86f, -2.62f, -4.46f, -2.18f, -3.1f, -2.58f, -1.82f, -2.24f, -0.5f, -2.64f));
        strokes.Add(Points(0.92f, -2.18f, 2.18f, -2.62f, 3.56f, -2.22f, 5.2f, -2.72f));
        strokes.Add(Points(-6.12f, 0.92f, -5.42f, 0.38f, -6.04f, -0.22f, -5.28f, -0.82f));
        strokes.Add(Points(5.78f, 0.96f, 6.28f, 0.36f, 5.64f, -0.18f, 6.16f, -0.82f));

        return strokes;
    }

    private static List<Vector3[]> BuildFourthForestIslandDoorStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(-1.24f, -1.72f, -1.24f, 0.52f, -1.02f, 1.1f, -0.56f, 1.58f, 0f, 1.78f, 0.58f, 1.58f, 1.02f, 1.1f, 1.24f, 0.52f, 1.24f, -1.72f, -1.24f, -1.72f));
        strokes.Add(Points(-0.78f, -1.72f, -0.78f, 0.36f, -0.54f, 0.92f, 0f, 1.18f, 0.54f, 0.92f, 0.78f, 0.36f, 0.78f, -1.72f));
        strokes.Add(Points(0f, -1.62f, 0f, 1.12f));
        strokes.Add(EllipsePoints(0.42f, -0.18f, 0.11f, 0.09f, 12));
        strokes.Add(Points(-1.44f, -1.78f, 1.44f, -1.78f));

        return strokes;
    }

    private static List<Vector3[]> BuildFourthForestIslandDoorBrownStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(-0.82f, -1.28f, -0.28f, -1.18f, 0.28f, -1.3f, 0.86f, -1.18f));
        strokes.Add(Points(-0.76f, -0.72f, -0.18f, -0.6f, 0.3f, -0.74f, 0.78f, -0.58f));
        strokes.Add(Points(-0.72f, -0.1f, -0.22f, 0.02f, 0.24f, -0.12f, 0.72f, 0.02f));
        strokes.Add(Points(-0.56f, 0.52f, -0.14f, 0.66f, 0.24f, 0.52f, 0.54f, 0.66f));
        strokes.Add(Points(-0.36f, -1.54f, -0.28f, -0.82f, -0.38f, -0.12f, -0.24f, 0.62f));
        strokes.Add(Points(0.42f, -1.52f, 0.32f, -0.74f, 0.46f, -0.04f, 0.34f, 0.62f));

        return strokes;
    }

    private static List<Vector3[]> BuildFourthForestIslandDoorGreenStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(-1.06f, 0.68f, -1.36f, 1.18f, -1.08f, 1.56f, -0.5f, 1.54f));
        strokes.Add(Points(1.04f, 0.66f, 1.34f, 1.16f, 1.04f, 1.52f, 0.48f, 1.52f));
        strokes.Add(EllipsePoints(-1.42f, 1.24f, 0.22f, 0.12f, 12));
        strokes.Add(EllipsePoints(-0.72f, 1.58f, 0.2f, 0.11f, 12));
        strokes.Add(EllipsePoints(1.38f, 1.2f, 0.22f, 0.12f, 12));
        strokes.Add(EllipsePoints(0.72f, 1.56f, 0.2f, 0.11f, 12));
        strokes.Add(Points(-1.18f, -1.24f, -1.44f, -0.62f, -1.28f, -0.1f, -1.48f, 0.42f));

        return strokes;
    }

    private static List<Vector3[]> BuildFourthForestIslandDoorBlueStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(EllipsePoints(0f, 0.32f, 0.28f, 0.38f, 18));
        strokes.Add(Points(0f, 0.7f, -0.22f, 0.24f, 0f, -0.1f, 0.22f, 0.24f, 0f, 0.7f));
        strokes.Add(Points(-0.58f, 1.06f, -0.28f, 0.92f, 0f, 1.06f, 0.3f, 0.92f, 0.58f, 1.06f));
        strokes.Add(Points(-0.42f, -1.02f, 0.4f, -1.02f));
        strokes.Add(Points(-0.34f, -0.42f, 0.34f, -0.42f));

        return strokes;
    }

    private static List<Vector3[]> BuildFourthForestRootBridgeStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        float riverX = FourthForestRiverOffset.x;
        float riverY = FourthForestRiverOffset.y;

        strokes.Add(Points(
            riverX - 7.12f, riverY + 0.84f,
            riverX - 6.12f, riverY + 1.08f,
            riverX - 5.04f, riverY + 0.72f,
            riverX - 3.82f, riverY + 0.94f,
            riverX - 2.54f, riverY + 0.62f,
            riverX - 1.72f, riverY + 0.72f));
        strokes.Add(Points(
            riverX - 7.08f, riverY - 0.68f,
            riverX - 6.04f, riverY - 0.42f,
            riverX - 4.92f, riverY - 0.78f,
            riverX - 3.7f, riverY - 0.52f,
            riverX - 2.46f, riverY - 0.72f,
            riverX - 1.68f, riverY - 0.58f));
        strokes.Add(Points(
            riverX - 6.6f, riverY + 0.5f,
            riverX - 6.46f, riverY - 0.42f));
        strokes.Add(Points(
            riverX - 5.24f, riverY + 0.42f,
            riverX - 5.14f, riverY - 0.56f));
        strokes.Add(Points(
            riverX - 3.88f, riverY + 0.54f,
            riverX - 3.78f, riverY - 0.46f));
        strokes.Add(Points(
            riverX - 2.42f, riverY + 0.4f,
            riverX - 2.34f, riverY - 0.48f));

        return strokes;
    }

    private static List<Vector3[]> BuildMoleHoleStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(EllipsePoints(0f, 0f, 0.92f, 0.38f, 24));
        strokes.Add(EllipsePoints(0.06f, 0.05f, 0.62f, 0.22f, 18));
        strokes.Add(Points(-0.94f, -0.05f, -1.24f, -0.18f, -0.78f, -0.28f));
        strokes.Add(Points(0.82f, -0.06f, 1.18f, -0.18f, 0.72f, -0.28f));
        strokes.Add(Points(-0.54f, 0.36f, -0.16f, 0.48f, 0.24f, 0.38f, 0.58f, 0.5f));
        strokes.Add(Points(-0.7f, -0.32f, -0.24f, -0.42f, 0.26f, -0.34f, 0.72f, -0.44f));

        return strokes;
    }

    private static List<Vector3[]> BuildForestRockStrokes(float scale)
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(RockPoints(scale, -1.05f, -0.32f, -0.78f, 0.22f, -0.28f, 0.54f, 0.34f, 0.48f, 0.92f, 0.08f, 1.12f, -0.34f, 0.64f, -0.52f, -0.1f, -0.48f, -0.72f, -0.5f, -1.05f, -0.32f));
        strokes.Add(RockPoints(scale, -0.72f, 0.18f, -0.36f, -0.02f, 0.08f, 0.22f, 0.42f, 0.04f, 0.74f, 0.18f));
        strokes.Add(RockPoints(scale, -0.44f, -0.36f, -0.12f, -0.18f, 0.22f, -0.34f, 0.56f, -0.18f));
        strokes.Add(RockPoints(scale, 0.12f, 0.42f, 0.0f, 0.16f, 0.18f, -0.08f));
        strokes.Add(RockPoints(scale, -0.28f, 0.1f, -0.08f, -0.04f, -0.18f, -0.26f));

        return strokes;
    }

    private static Vector3[] RockPoints(float scale, params float[] values)
    {
        int pointCount = values.Length / 2;
        Vector3[] points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector3(values[i * 2] * scale, values[i * 2 + 1] * scale, 0f);
        }

        return points;
    }

    private static List<Vector3[]> BuildFallenBirdStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(EllipsePoints(-0.06f, 0.24f, 0.72f, 0.36f, 24));
        strokes.Add(EllipsePoints(0.68f, 0.34f, 0.28f, 0.24f, 16));
        strokes.Add(Points(0.94f, 0.36f, 1.2f, 0.5f, 0.94f, 0.56f));
        strokes.Add(Points(-0.42f, 0.34f, -0.96f, 0.62f, -0.78f, 0.08f, -0.42f, 0.34f));
        strokes.Add(Points(-0.18f, 0.38f, 0.22f, 0.08f, 0.34f, 0.32f, -0.04f, 0.55f));
        strokes.Add(Points(0.6f, 0.42f, 0.68f, 0.42f));
        strokes.Add(Points(-0.18f, -0.08f, -0.4f, -0.34f, -0.68f, -0.28f));
        strokes.Add(Points(0.18f, -0.08f, 0.32f, -0.38f, 0.62f, -0.34f));
        strokes.Add(Points(-0.92f, -0.42f, -0.42f, -0.32f, 0.1f, -0.42f, 0.76f, -0.32f));
        strokes.Add(Points(-0.58f, -0.22f, -0.3f, -0.12f, -0.02f, -0.18f, 0.26f, -0.06f, 0.56f, -0.16f));
        strokes.Add(Points(-0.08f, 0.82f, 0.08f, 1.06f, 0.28f, 0.8f));

        return strokes;
    }

    private static List<Vector3[]> BuildSmallBirdStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(EllipsePoints(0f, 0.58f, 0.58f, 0.4f, 24));
        strokes.Add(EllipsePoints(0.5f, 0.88f, 0.28f, 0.24f, 16));
        strokes.Add(Points(0.76f, 0.88f, 1.06f, 0.98f, 0.78f, 1.04f));
        strokes.Add(Points(-0.42f, 0.72f, -0.9f, 0.98f, -0.74f, 0.5f, -0.42f, 0.72f));
        strokes.Add(Points(-0.16f, 0.72f, 0.24f, 0.5f, 0.34f, 0.72f, -0.02f, 0.92f));
        strokes.Add(Points(0.42f, 0.96f, 0.5f, 0.96f));
        strokes.Add(Points(-0.16f, 0.2f, -0.16f, -0.16f, -0.38f, -0.34f));
        strokes.Add(Points(0.18f, 0.2f, 0.18f, -0.16f, 0.42f, -0.32f));
        strokes.Add(Points(-0.74f, -0.34f, -0.24f, -0.22f, 0.28f, -0.32f, 0.76f, -0.22f));
        strokes.Add(Points(-0.54f, -0.18f, -0.3f, -0.06f, 0.02f, -0.12f, 0.26f, 0.02f, 0.52f, -0.06f));

        return strokes;
    }

    private static List<Vector3[]> BuildSquirrelStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(EllipsePoints(0f, 0.56f, 0.48f, 0.56f, 26));
        strokes.Add(EllipsePoints(0.38f, 1.04f, 0.3f, 0.28f, 18));
        strokes.Add(EllipsePoints(0.12f, 1.22f, 0.12f, 0.16f, 12));
        strokes.Add(EllipsePoints(0.5f, 1.28f, 0.12f, 0.16f, 12));
        strokes.Add(Points(0.62f, 1.08f, 0.9f, 1.14f, 0.66f, 1.2f));
        strokes.Add(Points(0.5f, 1.08f, 0.58f, 1.08f));
        strokes.Add(Points(0.26f, 1.0f, 0.36f, 0.94f, 0.48f, 0.98f));
        strokes.Add(Points(-0.24f, 0.64f, -0.62f, 0.98f, -0.92f, 1.52f, -0.62f, 2.06f, -0.08f, 1.88f, -0.2f, 1.36f, -0.56f, 1.0f));
        strokes.Add(Points(-0.36f, 0.56f, -0.78f, 0.7f, -0.94f, 0.38f, -0.62f, 0.18f, -0.24f, 0.34f));
        strokes.Add(Points(-0.18f, 0.06f, -0.28f, -0.28f, -0.56f, -0.38f));
        strokes.Add(Points(0.2f, 0.08f, 0.32f, -0.26f, 0.62f, -0.34f));
        strokes.Add(Points(-0.62f, -0.42f, -0.24f, -0.32f, 0.24f, -0.42f, 0.66f, -0.32f));
        strokes.Add(Points(0.0f, 0.62f, 0.26f, 0.46f, 0.32f, 0.66f, 0.08f, 0.78f));
        strokes.Add(Points(-0.04f, 0.32f, 0.12f, 0.18f, 0.34f, 0.2f));

        return strokes;
    }

    private static List<Vector3[]> BuildHungrySquirrelStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(EllipsePoints(-0.02f, 0.48f, 0.46f, 0.52f, 26));
        strokes.Add(EllipsePoints(0.34f, 0.94f, 0.3f, 0.26f, 18));
        strokes.Add(EllipsePoints(0.1f, 1.12f, 0.12f, 0.15f, 12));
        strokes.Add(EllipsePoints(0.48f, 1.16f, 0.12f, 0.15f, 12));
        strokes.Add(Points(0.58f, 0.96f, 0.86f, 0.98f, 0.62f, 1.08f));
        strokes.Add(Points(0.46f, 0.98f, 0.54f, 0.98f));
        strokes.Add(Points(0.2f, 0.88f, 0.34f, 0.82f, 0.48f, 0.88f));
        strokes.Add(Points(0.24f, 0.74f, 0.34f, 0.68f, 0.46f, 0.72f));
        strokes.Add(Points(0.18f, 0.56f, 0.36f, 0.42f, 0.5f, 0.52f, 0.34f, 0.66f));
        strokes.Add(Points(-0.26f, 0.56f, -0.66f, 0.84f, -0.98f, 1.3f, -0.84f, 1.78f, -0.32f, 1.7f, -0.34f, 1.18f, -0.66f, 0.88f));
        strokes.Add(Points(-0.38f, 0.48f, -0.82f, 0.56f, -0.98f, 0.24f, -0.68f, 0.06f, -0.28f, 0.24f));
        strokes.Add(Points(-0.18f, 0.0f, -0.32f, -0.32f, -0.6f, -0.38f));
        strokes.Add(Points(0.2f, 0.02f, 0.28f, -0.3f, 0.58f, -0.38f));
        strokes.Add(Points(-0.62f, -0.46f, -0.24f, -0.36f, 0.2f, -0.46f, 0.6f, -0.36f));
        strokes.Add(Points(-0.02f, 0.42f, 0.16f, 0.3f, 0.28f, 0.42f));
        strokes.Add(Points(-0.34f, 0.22f, -0.18f, 0.32f, -0.02f, 0.22f, 0.14f, 0.32f, 0.3f, 0.22f));
        strokes.Add(Points(-0.54f, 1.46f, -0.76f, 1.64f, -0.86f, 1.38f));

        return strokes;
    }

    private static List<Vector3[]> BuildAcornStrokes(float scale)
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(ScaledEllipsePoints(0f, -0.06f, 0.38f, 0.5f, 22, scale));
        strokes.Add(ScaledPoints(scale, -0.42f, 0.18f, -0.22f, 0.48f, 0.1f, 0.56f, 0.42f, 0.28f));
        strokes.Add(ScaledPoints(scale, -0.4f, 0.16f, -0.14f, 0.04f, 0.14f, 0.12f, 0.42f, 0.02f));
        strokes.Add(ScaledPoints(scale, -0.16f, 0.48f, -0.04f, 0.72f, 0.12f, 0.5f));
        strokes.Add(ScaledPoints(scale, -0.22f, -0.2f, -0.06f, -0.34f, 0.14f, -0.24f));

        return strokes;
    }

    private static List<Vector3[]> BuildShakingBushStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(-1.08f, -0.1f, -0.78f, 0.38f, -0.34f, 0.66f, 0.12f, 0.54f, 0.54f, 0.76f, 1.02f, 0.32f, 1.16f, -0.18f, 0.72f, -0.34f, 0.12f, -0.28f, -0.42f, -0.42f, -1.08f, -0.1f));
        strokes.Add(Points(-0.84f, 0.12f, -0.56f, 0.72f, -0.16f, 0.18f, 0.18f, 0.82f, 0.48f, 0.2f, 0.9f, 0.52f));
        strokes.Add(Points(-0.72f, -0.18f, -0.42f, 0.16f, -0.06f, -0.12f, 0.26f, 0.22f, 0.68f, -0.18f));
        strokes.Add(Points(-0.92f, -0.4f, -0.5f, -0.28f, -0.08f, -0.46f, 0.34f, -0.32f, 0.9f, -0.44f));
        strokes.Add(Points(-0.92f, 0.62f, -1.22f, 0.86f, -1.42f, 0.58f));
        strokes.Add(Points(0.84f, 0.72f, 1.18f, 0.94f, 1.38f, 0.62f));
        strokes.Add(Points(-0.18f, 0.9f, -0.04f, 1.16f, 0.12f, 0.92f));

        return strokes;
    }

    private static List<Vector3[]> BuildLeafPileStrokes(float scale)
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(ScaledPoints(scale, -0.95f, -0.08f, -0.72f, 0.18f, -0.35f, 0.28f, 0.05f, 0.18f, 0.42f, 0.32f, 0.86f, 0.06f, 1.02f, -0.18f));
        strokes.Add(ScaledPoints(scale, -0.78f, -0.14f, -0.42f, -0.02f, -0.02f, -0.18f, 0.36f, -0.04f, 0.78f, -0.18f));
        strokes.Add(ScaledPoints(scale, -0.58f, 0.12f, -0.44f, 0.38f, -0.16f, 0.22f, 0.02f, 0.46f, 0.28f, 0.18f));
        strokes.Add(ScaledPoints(scale, 0.22f, 0.12f, 0.46f, 0.42f, 0.68f, 0.16f, 0.9f, 0.34f));
        strokes.Add(ScaledPoints(scale, -0.9f, -0.28f, -0.44f, -0.4f, 0.04f, -0.3f, 0.5f, -0.42f, 0.94f, -0.28f));
        strokes.Add(ScaledPoints(scale, -0.34f, -0.04f, -0.22f, 0.14f, -0.02f, 0.02f, 0.08f, 0.2f, 0.26f, 0.04f));

        return strokes;
    }

    private static List<Vector3[]> BuildLeafPileBrownColorStrokes(float scale)
    {
        List<Vector3[]> strokes = BuildLeafPileStrokes(scale);

        strokes.Add(ScaledPoints(scale, -0.72f, 0.02f, -0.38f, 0.12f, -0.04f, 0.04f, 0.28f, 0.14f, 0.68f, -0.02f));
        strokes.Add(ScaledPoints(scale, -0.64f, -0.18f, -0.28f, -0.08f, 0.08f, -0.2f, 0.42f, -0.1f, 0.78f, -0.22f));
        strokes.Add(ScaledPoints(scale, -0.36f, 0.28f, -0.12f, 0.36f, 0.16f, 0.24f, 0.46f, 0.34f));
        strokes.Add(ScaledPoints(scale, -0.46f, -0.34f, -0.06f, -0.28f, 0.34f, -0.36f));

        return strokes;
    }

    private static Vector3[] ScaledEllipsePoints(float centerX, float centerY, float radiusX, float radiusY, int segmentCount, float scale)
    {
        Vector3[] points = EllipsePoints(centerX, centerY, radiusX, radiusY, segmentCount);

        for (int i = 0; i < points.Length; i++)
        {
            points[i] *= scale;
        }

        return points;
    }

    private static Vector3[] ScaledPoints(float scale, params float[] values)
    {
        int pointCount = values.Length / 2;
        Vector3[] points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector3(values[i * 2] * scale, values[i * 2 + 1] * scale, 0f);
        }

        return points;
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

    private static Vector2 CalculateOutsideOvalFenceRadius()
    {
        float aspect = OvalFenceReferenceRadiusY / OvalFenceReferenceRadiusX;
        return new Vector2(OvalFenceTargetRadiusX, OvalFenceTargetRadiusX * aspect);
    }

    private static List<Vector3[]> BuildOvalFenceStrokes(bool leaveForestGateOpen)
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        AddOvalWoodRail(strokes, OvalFenceOuterRailInset, leaveForestGateOpen);
        AddOvalWoodRail(strokes, OvalFenceOuterRailInset + OvalFenceRailGap, leaveForestGateOpen);
        AddOvalFencePosts(strokes, leaveForestGateOpen);

        if (leaveForestGateOpen)
        {
            float right = OutsideOvalFenceRadius.x - OvalFencePostLength * 0.86f;
            strokes.Add(Points(right, ForestGateHalfHeight, right - 1.05f, 1.18f, right, -ForestGateHalfHeight));
        }

        return strokes;
    }

    private static void AddOvalWoodRail(List<Vector3[]> strokes, float inset, bool leaveForestGateOpen)
    {
        List<Vector3> currentStroke = new List<Vector3>();
        float postSpacing = Mathf.PI * 2f / OvalFencePostCount;
        int pointsPerPostSegment = Mathf.Max(1, Mathf.CeilToInt(OvalFenceRailResolution / (float)OvalFencePostCount));

        for (int postIndex = 0; postIndex < OvalFencePostCount; postIndex++)
        {
            float segmentStart = postIndex * postSpacing;
            float segmentEnd = segmentStart + postSpacing;

            for (int step = 0; step < pointsPerPostSegment; step++)
            {
                float progress = step / (float)pointsPerPostSegment;
                float angle = Mathf.Lerp(segmentStart, segmentEnd, progress);
                Vector3 edgePoint = GetOvalFencePoint(angle, 0f);

                if (leaveForestGateOpen && IsVillageForestGatePoint(edgePoint))
                {
                    AddCurrentOvalStroke(strokes, currentStroke);
                    continue;
                }

                currentStroke.Add(GetOvalFenceRailPoint(angle, inset));
            }
        }

        Vector3 finalEdgePoint = GetOvalFencePoint(Mathf.PI * 2f, 0f);
        if (!leaveForestGateOpen || !IsVillageForestGatePoint(finalEdgePoint))
        {
            currentStroke.Add(GetOvalFenceRailPoint(Mathf.PI * 2f, inset));
        }

        AddCurrentOvalStroke(strokes, currentStroke);
    }

    private static Vector3 GetOvalFenceRailPoint(float angle, float yInset)
    {
        Vector3 edgePoint = GetOvalFencePoint(angle, 0f);
        return edgePoint + Vector3.down * yInset;
    }

    private static void AddCurrentOvalStroke(List<Vector3[]> strokes, List<Vector3> currentStroke)
    {
        if (currentStroke.Count >= 2)
        {
            strokes.Add(currentStroke.ToArray());
        }

        currentStroke.Clear();
    }

    private static void AddOvalFencePosts(List<Vector3[]> strokes, bool leaveForestGateOpen)
    {
        for (int i = 0; i < OvalFencePostCount; i++)
        {
            float angle = Mathf.PI * 2f * i / OvalFencePostCount;
            Vector3 edgePoint = GetOvalFencePoint(angle, 0f);

            if (leaveForestGateOpen && IsVillageForestGatePoint(edgePoint))
            {
                continue;
            }

            AddOvalFencePost(strokes, edgePoint);
        }
    }

    private static void AddOvalFencePost(List<Vector3[]> strokes, Vector3 edgePoint)
    {
        Vector2 edge = new Vector2(edgePoint.x, edgePoint.y);
        Vector2 inner = edge - Vector2.up * OvalFencePostLength;
        Vector2 capBase = edge + Vector2.up * 0.08f;
        Vector2 outer = edge + Vector2.up * 0.28f;
        Vector2 halfWidth = Vector2.right * OvalFencePostHalfWidth;

        strokes.Add(new[]
        {
            ToVector3(inner - halfWidth),
            ToVector3(capBase - halfWidth),
            ToVector3(outer),
            ToVector3(capBase + halfWidth),
            ToVector3(inner + halfWidth),
            ToVector3(inner - halfWidth)
        });

        Vector2 grainStart = inner + Vector2.right * (OvalFencePostHalfWidth * 0.36f);
        Vector2 grainMiddle = edge + Vector2.right * (OvalFencePostHalfWidth * 0.18f);
        Vector2 grainEnd = capBase + Vector2.right * (OvalFencePostHalfWidth * 0.1f);
        strokes.Add(new[] { ToVector3(grainStart), ToVector3(grainMiddle), ToVector3(grainEnd) });

        Vector2 knot = Vector2.Lerp(inner, capBase, 0.48f);
        strokes.Add(EllipsePoints(knot.x, knot.y, 0.055f, OvalFencePostHalfWidth * 0.34f, 8));
    }

    private static Vector3 GetOvalFencePoint(float angle, float inset)
    {
        float radiusX = Mathf.Max(0.1f, OutsideOvalFenceRadius.x - inset);
        float radiusY = Mathf.Max(0.1f, OutsideOvalFenceRadius.y - inset);
        return new Vector3(
            Mathf.Cos(angle) * radiusX,
            Mathf.Sin(angle) * radiusY,
            0f);
    }

    private static Vector2 GetOvalFenceNormal(float angle)
    {
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        Vector2 normal = new Vector2(
            cos / Mathf.Max(0.1f, OutsideOvalFenceRadius.x),
            sin / Mathf.Max(0.1f, OutsideOvalFenceRadius.y));

        return normal.sqrMagnitude > 0.0001f ? normal.normalized : Vector2.right;
    }

    private static bool IsVillageForestGatePoint(Vector3 point)
    {
        return point.x > OutsideOvalFenceRadius.x * 0.72f && Mathf.Abs(point.y) <= ForestGateHalfHeight;
    }

    private static Vector3[] BuildOvalFenceRevealPoints(bool leaveForestGateOpen)
    {
        int pointCount = Mathf.Max(16, Mathf.CeilToInt(Mathf.PI * 2f * Mathf.Max(OutsideOvalFenceRadius.x, OutsideOvalFenceRadius.y) / FenceRevealTriggerSpacing));
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < pointCount; i++)
        {
            float angle = Mathf.PI * 2f * i / pointCount;
            Vector3 point = GetOvalFencePoint(angle, 0f);

            if (leaveForestGateOpen && IsVillageForestGatePoint(point))
            {
                continue;
            }

            points.Add(point);
        }

        return points.ToArray();
    }

    private static Vector3 ToVector3(Vector2 point)
    {
        return new Vector3(point.x, point.y, 0f);
    }

    private static List<Vector3[]> BuildFenceTopStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        float left = -BackgroundSize.x * 0.5f + 0.45f;
        float right = BackgroundSize.x * 0.5f - 0.45f;
        float top = BackgroundSize.y * 0.5f - 0.45f;
        AddHorizontalWoodFence(strokes, left, right, top, -1f);
        return strokes;
    }

    private static List<Vector3[]> BuildFenceBottomStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        float left = -BackgroundSize.x * 0.5f + 0.45f;
        float right = BackgroundSize.x * 0.5f - 0.45f;
        float bottom = -BackgroundSize.y * 0.5f + 0.45f;
        AddHorizontalWoodFence(strokes, left, right, bottom, 1f);
        return strokes;
    }

    private static List<Vector3[]> BuildFenceLeftStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        float left = -BackgroundSize.x * 0.5f + 0.45f;
        float bottom = -BackgroundSize.y * 0.5f + 0.45f;
        float top = BackgroundSize.y * 0.5f - 0.45f;
        AddVerticalWoodFence(strokes, left, bottom, top, 1f);
        return strokes;
    }

    private static List<Vector3[]> BuildFenceRightStrokes()
    {
        return BuildOvalFenceStrokes(false);
    }

    private static List<Vector3[]> BuildFenceRightGateStrokes()
    {
        return BuildOvalFenceStrokes(true);
    }

    private static List<Vector3[]> BuildForestExtensionBoundaryStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        float left = -BackgroundSize.x * 0.5f + 0.45f;
        float right = BackgroundSize.x * 0.5f - 0.45f;
        float bottom = -BackgroundSize.y * 0.5f + 0.45f;
        float top = BackgroundSize.y * 0.5f - 0.45f;

        AddHorizontalWoodFence(strokes, left, right, top, -1f);
        AddHorizontalWoodFence(strokes, left, right, bottom, 1f);
        AddVerticalWoodFence(strokes, right, bottom, top, -1f);
        return strokes;
    }

    private static List<Vector3[]> BuildForestExtensionBoundaryWithDeepGateStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        float left = -BackgroundSize.x * 0.5f + 0.45f;
        float right = BackgroundSize.x * 0.5f + DeepForestSize.x - 0.45f;
        float bottom = -BackgroundSize.y * 0.5f + 0.45f;
        float top = BackgroundSize.y * 0.5f - 0.45f;

        AddHorizontalWoodFence(strokes, left, right, top, -1f);
        AddHorizontalWoodFence(strokes, left, right, bottom, 1f);
        return strokes;
    }

    private static List<Vector3[]> BuildForestExtensionBoundaryWithFourthForestStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        float left = -BackgroundSize.x * 0.5f + 0.45f;
        float right = BackgroundSize.x * 0.5f + DeepForestSize.x + FourthForestSize.x - 0.45f;
        float bottom = -BackgroundSize.y * 0.5f + 0.45f;
        float top = BackgroundSize.y * 0.5f - 0.45f;

        AddHorizontalWoodFence(strokes, left, right, top, -1f);
        AddHorizontalWoodFence(strokes, left, right, bottom, 1f);
        return strokes;
    }

    private static List<Vector3[]> BuildDeepForestExtensionBoundaryStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        float right = DeepForestSize.x * 0.5f - 0.45f;
        float bottom = -DeepForestSize.y * 0.5f + 0.45f;
        float top = DeepForestSize.y * 0.5f - 0.45f;

        AddVerticalWoodFence(strokes, right, bottom, top, -1f);
        return strokes;
    }

    private static List<Vector3[]> BuildDeepForestExtensionBoundaryWithFourthGateStrokes()
    {
        return new List<Vector3[]>();
    }

    private static List<Vector3[]> BuildFourthForestExtensionBoundaryStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        float right = FourthForestSize.x * 0.5f - 0.45f;
        float bottom = -FourthForestSize.y * 0.5f + 0.45f;
        float top = FourthForestSize.y * 0.5f - 0.45f;

        AddVerticalWoodFence(strokes, right, bottom, top, -1f);
        return strokes;
    }

    private static Vector3[] BuildForestExtensionBoundaryRevealPoints()
    {
        float halfWidth = BackgroundSize.x * 0.5f;
        float halfHeight = BackgroundSize.y * 0.5f;
        List<Vector3> points = new List<Vector3>();

        AddOffsetRevealPoints(points, BuildHorizontalFenceRevealPoints(halfHeight, halfWidth), ForestRegionOffset);
        AddOffsetRevealPoints(points, BuildHorizontalFenceRevealPoints(-halfHeight, halfWidth), ForestRegionOffset);
        AddOffsetRevealPoints(points, BuildVerticalFenceRevealPoints(halfWidth, halfHeight), ForestRegionOffset);
        AddOffsetRevealPoints(points, BuildVerticalFenceRevealPoints(-halfWidth, halfHeight), ForestRegionOffset);
        return points.ToArray();
    }

    private static Vector3[] BuildForestExtensionBoundaryWithDeepGateRevealPoints()
    {
        float left = -BackgroundSize.x * 0.5f;
        float right = BackgroundSize.x * 0.5f + DeepForestSize.x;
        float halfHeight = BackgroundSize.y * 0.5f;
        List<Vector3> points = new List<Vector3>();

        AddOffsetRevealPoints(points, BuildHorizontalFenceRevealPoints(halfHeight, left, right), ForestRegionOffset);
        AddOffsetRevealPoints(points, BuildHorizontalFenceRevealPoints(-halfHeight, left, right), ForestRegionOffset);
        return points.ToArray();
    }

    private static Vector3[] BuildForestExtensionBoundaryWithFourthForestRevealPoints()
    {
        float left = -BackgroundSize.x * 0.5f;
        float right = BackgroundSize.x * 0.5f + DeepForestSize.x + FourthForestSize.x;
        float halfHeight = BackgroundSize.y * 0.5f;
        List<Vector3> points = new List<Vector3>();

        AddOffsetRevealPoints(points, BuildHorizontalFenceRevealPoints(halfHeight, left, right), ForestRegionOffset);
        AddOffsetRevealPoints(points, BuildHorizontalFenceRevealPoints(-halfHeight, left, right), ForestRegionOffset);
        return points.ToArray();
    }

    private static Vector3[] BuildDeepForestExtensionBoundaryRevealPoints()
    {
        float halfWidth = DeepForestSize.x * 0.5f;
        float halfHeight = DeepForestSize.y * 0.5f;
        List<Vector3> points = new List<Vector3>();

        AddOffsetRevealPoints(points, BuildVerticalFenceRevealPoints(halfWidth, halfHeight), DeepForestRegionOffset);
        return points.ToArray();
    }

    private static Vector3[] BuildFourthForestExtensionBoundaryRevealPoints()
    {
        float halfWidth = FourthForestSize.x * 0.5f;
        float halfHeight = FourthForestSize.y * 0.5f;
        List<Vector3> points = new List<Vector3>();

        AddOffsetRevealPoints(points, BuildVerticalFenceRevealPoints(halfWidth, halfHeight), FourthForestRegionOffset);
        return points.ToArray();
    }

    private static void AddOffsetRevealPoints(List<Vector3> points, Vector3[] sourcePoints, Vector3 offset)
    {
        for (int i = 0; i < sourcePoints.Length; i++)
        {
            points.Add(sourcePoints[i] + offset);
        }
    }

    private static void AddHorizontalWoodFence(List<Vector3[]> strokes, float left, float right, float edgeY, float innerSign)
    {
        const float railThickness = 0.12f;
        const float postHalfWidth = 0.16f;
        const float postLength = 1.18f;
        const float postStep = 2.35f;

        AddHorizontalWoodRail(strokes, left, right, edgeY + innerSign * 0.34f, railThickness);
        AddHorizontalWoodRail(strokes, left, right, edgeY + innerSign * 0.82f, railThickness);

        for (float x = left; x <= right + 0.01f; x += postStep)
        {
            AddHorizontalFencePost(strokes, x, edgeY, innerSign, postHalfWidth, postLength);
        }
    }

    private static void AddVerticalWoodFence(List<Vector3[]> strokes, float edgeX, float bottom, float top, float innerSign)
    {
        const float railThickness = 0.12f;
        const float postHalfWidth = 0.16f;
        const float postLength = 1.18f;
        const float postStep = 2.35f;

        AddVerticalWoodRail(strokes, edgeX + innerSign * 0.34f, bottom, top, railThickness);
        AddVerticalWoodRail(strokes, edgeX + innerSign * 0.82f, bottom, top, railThickness);

        for (float y = bottom; y <= top + 0.01f; y += postStep)
        {
            AddVerticalFencePost(strokes, edgeX, y, innerSign, postHalfWidth, postLength);
        }
    }

    private static void AddHorizontalWoodRail(List<Vector3[]> strokes, float left, float right, float centerY, float halfThickness)
    {
        strokes.Add(Points(left, centerY - halfThickness, right, centerY - halfThickness));
        strokes.Add(Points(left, centerY + halfThickness, right, centerY + halfThickness));
        strokes.Add(Points(left + 0.18f, centerY - halfThickness, left, centerY, left + 0.18f, centerY + halfThickness));
        strokes.Add(Points(right - 0.18f, centerY - halfThickness, right, centerY, right - 0.18f, centerY + halfThickness));

        for (float x = left + 1.25f; x < right - 0.9f; x += 3.2f)
        {
            strokes.Add(Points(x, centerY - halfThickness * 0.32f, x + 1.15f, centerY + halfThickness * 0.22f));
        }
    }

    private static void AddVerticalWoodRail(List<Vector3[]> strokes, float centerX, float bottom, float top, float halfThickness)
    {
        strokes.Add(Points(centerX - halfThickness, bottom, centerX - halfThickness, top));
        strokes.Add(Points(centerX + halfThickness, bottom, centerX + halfThickness, top));
        strokes.Add(Points(centerX - halfThickness, bottom + 0.18f, centerX, bottom, centerX + halfThickness, bottom + 0.18f));
        strokes.Add(Points(centerX - halfThickness, top - 0.18f, centerX, top, centerX + halfThickness, top - 0.18f));

        for (float y = bottom + 1.25f; y < top - 0.9f; y += 3.2f)
        {
            strokes.Add(Points(centerX - halfThickness * 0.3f, y, centerX + halfThickness * 0.22f, y + 1.15f));
        }
    }

    private static void AddHorizontalFencePost(List<Vector3[]> strokes, float x, float edgeY, float innerSign, float halfWidth, float length)
    {
        float outerSign = -innerSign;
        float outerY = edgeY + outerSign * 0.28f;
        float capBaseY = outerY - outerSign * 0.2f;
        float innerY = edgeY + innerSign * length;
        float grainX = x + halfWidth * 0.36f;
        float knotY = Mathf.Lerp(innerY, capBaseY, 0.48f);

        strokes.Add(Points(
            x - halfWidth, innerY,
            x - halfWidth, capBaseY,
            x, outerY,
            x + halfWidth, capBaseY,
            x + halfWidth, innerY,
            x - halfWidth, innerY));
        strokes.Add(Points(grainX, innerY + innerSign * -0.12f, grainX - halfWidth * 0.18f, edgeY, grainX + halfWidth * 0.1f, capBaseY));
        strokes.Add(EllipsePoints(x - halfWidth * 0.28f, knotY, halfWidth * 0.34f, 0.055f, 8));
    }

    private static void AddVerticalFencePost(List<Vector3[]> strokes, float edgeX, float y, float innerSign, float halfWidth, float length)
    {
        float outerSign = -innerSign;
        float outerX = edgeX + outerSign * 0.28f;
        float capBaseX = outerX - outerSign * 0.2f;
        float innerX = edgeX + innerSign * length;
        float grainY = y + halfWidth * 0.36f;
        float knotX = Mathf.Lerp(innerX, capBaseX, 0.48f);

        strokes.Add(Points(
            innerX, y - halfWidth,
            capBaseX, y - halfWidth,
            outerX, y,
            capBaseX, y + halfWidth,
            innerX, y + halfWidth,
            innerX, y - halfWidth));
        strokes.Add(Points(innerX + innerSign * -0.12f, grainY, edgeX, grainY - halfWidth * 0.18f, capBaseX, grainY + halfWidth * 0.1f));
        strokes.Add(EllipsePoints(knotX, y - halfWidth * 0.28f, 0.055f, halfWidth * 0.34f, 8));
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

    private static List<Vector3[]> BuildPineLeafStrokes(float scale)
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        AddOutsidePineLeaves(strokes, 0f, 0f, scale);
        return strokes;
    }

    private static List<Vector3[]> BuildPineTrunkBrownStrokes(float scale)
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        AddOutsidePineTrunk(strokes, 0f, 0f, scale);
        return strokes;
    }

    private static List<Vector3[]> BuildAncientTreeStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        strokes.AddRange(BuildAncientTreeLeafStrokes());
        strokes.AddRange(BuildAncientTreeTrunkBrownStrokes());
        strokes.Add(Points(-4.6f, -0.82f, -2.5f, -0.28f, -0.35f, -0.72f, 1.72f, -0.22f, 4.4f, -0.74f));
        strokes.Add(Points(-3.8f, 5.2f, -1.5f, 6.2f, 1.2f, 5.4f, 3.9f, 6.1f));
        return strokes;
    }

    private static List<Vector3[]> BuildAncientTreeLeafStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(EllipsePoints(0f, 7.6f, 7.8f, 5.35f, 38));
        strokes.Add(EllipsePoints(-4.9f, 6.1f, 4.25f, 3.25f, 28));
        strokes.Add(EllipsePoints(4.8f, 6.3f, 4.3f, 3.2f, 28));
        strokes.Add(EllipsePoints(-1.3f, 10.05f, 4.9f, 3.15f, 28));
        strokes.Add(EllipsePoints(2.1f, 9.7f, 4.5f, 3.0f, 28));
        strokes.Add(Points(-6.8f, 7.5f, -3.9f, 8.6f, -0.8f, 7.8f, 2.2f, 8.7f, 6.5f, 7.3f));
        strokes.Add(Points(-5.5f, 5.4f, -1.6f, 6.1f, 1.8f, 5.25f, 5.7f, 6.1f));
        strokes.Add(Points(-3.8f, 10.6f, -1.1f, 11.5f, 1.8f, 10.6f, 4.2f, 11.2f));

        return strokes;
    }

    private static List<Vector3[]> BuildAncientTreeLeafColorStrokes()
    {
        List<Vector3[]> strokes = BuildAncientTreeLeafStrokes();

        AddAncientTreeLeafCrayonFill(strokes);

        return strokes;
    }

    private static List<Vector3[]> BuildAncientTreeTrunkBrownStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(-1.95f, -0.55f, -1.45f, 2.3f, -1.0f, 5.55f, 1.0f, 5.55f, 1.45f, 2.3f, 1.95f, -0.55f, -1.95f, -0.55f));
        strokes.Add(Points(-0.72f, 5.0f, -4.45f, 7.55f));
        strokes.Add(Points(0.62f, 4.85f, 4.4f, 7.42f));
        strokes.Add(Points(-0.64f, 2.35f, -3.1f, 4.2f));
        strokes.Add(Points(0.68f, 2.18f, 3.0f, 4.05f));
        strokes.Add(Points(-0.52f, 0.22f, -0.38f, 2.3f, -0.72f, 4.42f));
        strokes.Add(Points(0.56f, 0.12f, 0.36f, 2.42f, 0.78f, 4.52f));
        strokes.Add(EllipsePoints(0.42f, 1.85f, 0.48f, 0.34f, 16));
        strokes.Add(Points(-2.35f, -0.5f, -3.3f, -1.35f, -4.85f, -1.28f));
        strokes.Add(Points(2.35f, -0.5f, 3.28f, -1.32f, 4.82f, -1.24f));

        return strokes;
    }

    private static List<Vector3[]> BuildAncientTreeTrunkBrownColorStrokes()
    {
        List<Vector3[]> strokes = BuildAncientTreeTrunkBrownStrokes();

        AddAncientTreeTrunkCrayonFill(strokes);

        return strokes;
    }

    private static void AddAncientTreeLeafCrayonFill(List<Vector3[]> strokes)
    {
        strokes.Add(Points(-5.8f, 9.55f, -3.6f, 10.0f, -1.1f, 9.55f, 1.5f, 9.98f, 4.5f, 9.5f));
        strokes.Add(Points(-6.9f, 8.45f, -4.1f, 8.9f, -1.2f, 8.33f, 1.8f, 8.9f, 6.2f, 8.31f));
        strokes.Add(Points(-7.1f, 7.3f, -4.5f, 7.73f, -1.9f, 7.31f, 0.8f, 7.81f, 5.8f, 7.37f));
        strokes.Add(Points(-6.1f, 6.27f, -3.6f, 6.73f, -0.8f, 6.27f, 2.1f, 6.77f, 5.6f, 6.33f));
        strokes.Add(Points(-4.8f, 5.35f, -2.4f, 5.79f, 0.1f, 5.4f, 2.7f, 5.77f, 4.9f, 5.39f));
        strokes.Add(Points(-4.7f, 10.55f, -2.3f, 10.9f, 0.2f, 10.49f, 2.8f, 10.83f, 4.5f, 10.45f));
        strokes.Add(Points(-3.1f, 11.55f, -0.9f, 11.83f, 1.2f, 11.51f, 3.0f, 11.71f));
        strokes.Add(Points(-5.4f, 4.55f, -2.7f, 4.93f, 0.4f, 4.6f, 3.6f, 4.97f));
    }

    private static void AddAncientTreeTrunkCrayonFill(List<Vector3[]> strokes)
    {
        strokes.Add(Points(-1.28f, -0.24f, -0.32f, 0.0f, 0.96f, -0.28f));
        strokes.Add(Points(-1.18f, 0.5f, -0.24f, 0.76f, 1.18f, 0.46f));
        strokes.Add(Points(-1.06f, 1.28f, 0.12f, 1.52f, 1.08f, 1.22f));
        strokes.Add(Points(-0.96f, 2.08f, -0.04f, 2.3f, 0.96f, 2.04f));
        strokes.Add(Points(-0.82f, 2.86f, 0.02f, 3.08f, 0.84f, 2.82f));
        strokes.Add(Points(-0.68f, 3.66f, 0.02f, 3.84f, 0.66f, 3.62f));
        strokes.Add(Points(-0.48f, 4.52f, 0.1f, 4.68f, 0.5f, 4.52f));
        strokes.Add(Points(-3.75f, 6.12f, -2.58f, 6.48f, -1.18f, 5.48f));
        strokes.Add(Points(1.16f, 5.36f, 2.72f, 6.38f, 3.88f, 6.05f));
    }

    private static List<Vector3[]> BuildForestEntranceLeafStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        AddOutsidePineLeaves(strokes, -2.5f, -1.36f, 0.82f);
        AddOutsidePineLeaves(strokes, -1.42f, -1.34f, 1.02f);
        AddOutsidePineLeaves(strokes, -0.24f, -1.38f, 0.9f);
        AddOutsidePineLeaves(strokes, 1.0f, -1.34f, 1.14f);
        AddOutsidePineLeaves(strokes, 2.35f, -1.36f, 0.86f);
        return strokes;
    }

    private static List<Vector3[]> BuildForestEntranceTrunkBrownStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        AddOutsidePineTrunk(strokes, -2.5f, -1.36f, 0.82f);
        AddOutsidePineTrunk(strokes, -1.42f, -1.34f, 1.02f);
        AddOutsidePineTrunk(strokes, -0.24f, -1.38f, 0.9f);
        AddOutsidePineTrunk(strokes, 1.0f, -1.34f, 1.14f);
        AddOutsidePineTrunk(strokes, 2.35f, -1.36f, 0.86f);
        return strokes;
    }

    private static void AddOutsidePine(List<Vector3[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(PinePoints(centerX, groundY, scale, 0f, 2.48f, -0.7f, 1.62f, -0.42f, 1.62f, -0.92f, 0.86f, -0.5f, 0.86f, -1.16f, 0f, 1.16f, 0f, 0.5f, 0.86f, 0.92f, 0.86f, 0.42f, 1.62f, 0.7f, 1.62f, 0f, 2.48f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.14f, 0f, -0.14f, -0.34f, 0.14f, -0.34f, 0.14f, 0f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.5f, 0.86f, -0.16f, 1.22f, 0.16f, 0.86f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.42f, 1.62f, -0.12f, 1.92f, 0.2f, 1.62f));
    }

    private static void AddOutsidePineLeaves(List<Vector3[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(PinePoints(centerX, groundY, scale, 0f, 2.48f, -0.7f, 1.62f, -0.42f, 1.62f, -0.92f, 0.86f, -0.5f, 0.86f, -1.16f, 0f, 1.16f, 0f, 0.5f, 0.86f, 0.92f, 0.86f, 0.42f, 1.62f, 0.7f, 1.62f, 0f, 2.48f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.5f, 0.86f, -0.16f, 1.22f, 0.16f, 0.86f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.42f, 1.62f, -0.12f, 1.92f, 0.2f, 1.62f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.18f, 2.08f, 0.18f, 2.08f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.56f, 1.52f, -0.08f, 1.72f, 0.48f, 1.52f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.82f, 1.05f, -0.24f, 1.22f, 0.34f, 1.04f, 0.82f, 1.18f));
        strokes.Add(PinePoints(centerX, groundY, scale, -1.0f, 0.5f, -0.46f, 0.72f, 0.08f, 0.52f, 0.62f, 0.72f, 1.02f, 0.5f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.9f, 0.14f, -0.34f, 0.28f, 0.18f, 0.1f, 0.76f, 0.24f));
    }

    private static void AddOutsidePineTrunk(List<Vector3[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(PinePoints(centerX, groundY, scale, -0.14f, 0f, -0.14f, -0.34f, 0.14f, -0.34f, 0.14f, 0f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.06f, -0.3f, -0.1f, -0.04f, -0.32f, 0.34f));
        strokes.Add(PinePoints(centerX, groundY, scale, 0.06f, -0.28f, 0.1f, -0.02f, 0.34f, 0.3f));
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

    private static List<Vector3[]> BuildHouseBrownStrokes()
    {
        List<Vector3[]> strokes = BuildHouseStrokes();
        strokes.Add(Points(-2.05f, -0.95f, -1.1f, -0.84f, -0.2f, -0.96f, 0.78f, -0.84f, 1.9f, -0.94f));
        strokes.Add(Points(-1.95f, -0.48f, -0.72f, -0.38f, 0.52f, -0.52f, 1.82f, -0.4f));
        strokes.Add(Points(-1.82f, 0.03f, -0.62f, 0.14f, 0.62f, 0f, 1.75f, 0.14f));
        strokes.Add(Points(-2.22f, 0.7f, -0.95f, 1.25f, 0f, 1.82f, 1.02f, 1.24f, 2.2f, 0.72f));
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

    private static List<Vector3[]> BuildNeighborHouseBrownStrokes()
    {
        List<Vector3[]> strokes = BuildNeighborHouseStrokes();
        strokes.Add(Points(-1.72f, -0.88f, -0.78f, -0.78f, 0.16f, -0.9f, 1.58f, -0.8f));
        strokes.Add(Points(-1.64f, -0.42f, -0.48f, -0.3f, 0.48f, -0.42f, 1.52f, -0.28f));
        strokes.Add(Points(-1.54f, 0.0f, -0.42f, 0.1f, 0.54f, -0.02f, 1.44f, 0.1f));
        strokes.Add(Points(-2.0f, 0.4f, -0.9f, 0.92f, 0f, 1.42f, 0.9f, 0.92f, 2.0f, 0.4f));
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

public class LetterFragmentInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private LetterFragmentSource source = LetterFragmentSource.WindTrace;
    [SerializeField] private float promptDistance = 1.45f;

    private GameObject promptObject;
    private CircleCollider2D interactionCollider;
    private SketchWorldLineDrawing lineDrawing;
    private Transform player;
    private Vector3 baseScale = Vector3.one;
    private bool hasConfigured;
    private bool isCollecting;
    private bool isCollected;

    public void Configure(LetterFragmentSource letterSource, float distance, int promptSortingOrder)
    {
        source = letterSource;
        promptDistance = Mathf.Max(0.2f, distance);
        hasConfigured = true;
        EnsurePrompt(promptSortingOrder);
        EnsureCollider();
        RefreshCollectedState();
        SetInteractionAvailable(CanCollect());
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || isCollecting || !CanCollect() || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > promptDistance)
        {
            return;
        }

        StartCoroutine(CollectRoutine());
    }

    public void CollectImmediately()
    {
        if (!isActiveAndEnabled || isCollecting || !CanCollect())
        {
            return;
        }

        StartCoroutine(CollectRoutine());
    }

    private void Awake()
    {
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        baseScale = transform.localScale;
        EnsurePrompt(SketchOutsideTransitionPromptDefaults.LetterPromptSortingOrder);
        EnsureCollider();
    }

    private void OnEnable()
    {
        FindPlayer();

        if (!hasConfigured)
        {
            SetInteractionAvailable(false);

            if (promptObject != null)
            {
                promptObject.SetActive(false);
            }

            return;
        }

        RefreshCollectedState();
        SetInteractionAvailable(CanCollect());

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!hasConfigured)
        {
            SetInteractionAvailable(false);
            return;
        }

        RefreshCollectedState();

        if (!CanCollect())
        {
            SetInteractionAvailable(false);
            return;
        }

        SetInteractionAvailable(true);

        if (player == null)
        {
            FindPlayer();
        }

        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        bool shouldShowPrompt = isRevealed
            && player != null
            && Vector2.Distance(transform.position, player.position) <= promptDistance;

        if (promptObject != null)
        {
            promptObject.SetActive(shouldShowPrompt);
        }
    }

    private void OnDisable()
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = false;
        }

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private IEnumerator CollectRoutine()
    {
        isCollecting = true;
        isCollected = true;
        GameProgress.CollectLetterFragmentFrom(source);
        LetterQuestHud.ShowProgress();
        SetInteractionAvailable(false);

        const float collectDuration = 0.24f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < collectDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            transform.localScale = Vector3.Lerp(startScale, baseScale * 0.24f, easedProgress);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private bool CanCollect()
    {
        return hasConfigured && !isCollected && !GameProgress.HasCollectedLetterFragmentFrom(source);
    }

    private void RefreshCollectedState()
    {
        if (!hasConfigured)
        {
            return;
        }

        isCollected = GameProgress.HasCollectedLetterFragmentFrom(source);

        if (isCollected && !isCollecting)
        {
            if (promptObject != null)
            {
                promptObject.SetActive(false);
            }

            if (interactionCollider != null)
            {
                interactionCollider.enabled = false;
            }

            gameObject.SetActive(false);
        }
    }

    private void EnsurePrompt(int sortingOrder)
    {
        if (promptObject == null)
        {
            promptObject = new GameObject("GeneratedLetterFragmentPrompt");
            promptObject.transform.SetParent(transform, false);
            promptObject.transform.localPosition = new Vector3(0f, 0.82f, 0f);

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
            meshRenderer.sortingOrder = sortingOrder;
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
        interactionCollider.radius = 0.86f;
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

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }
}

public class ForestBranchTreeInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float interactDistance = 2.1f;

    private const int BranchSortingOrder = 78;

    private static Sprite pixelSprite;

    private CircleCollider2D interactionCollider;
    private SketchWorldLineDrawing lineDrawing;
    private Transform player;
    private GameObject promptObject;
    private float treeScale = 1f;
    private int promptSortingOrder = 72;
    private bool hasConfigured;
    private bool isDropping;

    public void Configure(float scale, float distance, int sortingOrder)
    {
        treeScale = Mathf.Max(0.1f, scale);
        interactDistance = Mathf.Max(0.2f, distance);
        promptSortingOrder = sortingOrder;
        hasConfigured = true;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(CanDropBranch());
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || !hasConfigured || !CanDropBranch() || isDropping || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > interactDistance)
        {
            return;
        }

        StartCoroutine(DropBranchRoutine(interactor.transform));
    }

    private void Awake()
    {
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(false);
    }

    private void OnEnable()
    {
        FindPlayer();
        SetInteractionAvailable(CanDropBranch());

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!hasConfigured || !CanDropBranch())
        {
            SetInteractionAvailable(false);
            return;
        }

        if (player == null)
        {
            FindPlayer();
        }

        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        bool isNear = player != null && Vector2.Distance(transform.position, player.position) <= interactDistance;

        SetInteractionAvailable(isRevealed && !isDropping);

        if (promptObject != null)
        {
            promptObject.SetActive(isRevealed && isNear && !isDropping);
        }
    }

    private void OnDisable()
    {
        SetInteractionAvailable(false);
    }

    private IEnumerator DropBranchRoutine(Transform interactor)
    {
        isDropping = true;
        SetInteractionAvailable(false);

        yield return StartCoroutine(ShakeTreeRoutine());

        Transform itemParent = transform.parent != null ? transform.parent : transform;
        GameObject branchObject = CreateBranchObject(itemParent);
        Vector3 startPosition = transform.position + new Vector3(0.18f * treeScale, 2.18f * treeScale, 0f);
        Vector3 groundPosition = transform.position + new Vector3(0.66f * treeScale, 0.18f, 0f);
        branchObject.transform.position = startPosition;

        const float fallDuration = 0.36f;
        float elapsed = 0f;

        while (elapsed < fallDuration && branchObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fallDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            branchObject.transform.position = Vector3.Lerp(startPosition, groundPosition, easedProgress);
            branchObject.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-18f, 38f, progress));
            yield return null;
        }

        if (branchObject != null)
        {
            branchObject.transform.position = groundPosition;
        }

        yield return new WaitForSeconds(0.12f);

        Vector3 collectStartPosition = branchObject != null ? branchObject.transform.position : groundPosition;
        Vector3 fallbackTargetPosition = collectStartPosition + new Vector3(0f, 0.72f, 0f);
        Vector3 startScale = branchObject != null ? branchObject.transform.localScale : Vector3.one;
        const float collectDuration = 0.42f;
        elapsed = 0f;

        while (elapsed < collectDuration && branchObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 targetPosition = interactor != null
                ? interactor.position + new Vector3(0f, 0.72f, 0f)
                : fallbackTargetPosition;
            Vector3 position = Vector3.Lerp(collectStartPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.18f;

            branchObject.transform.position = position;
            branchObject.transform.localScale = Vector3.Lerp(startScale, startScale * 0.2f, easedProgress);
            yield return null;
        }

        GameProgress.CollectForestBranch();
        LetterQuestHud.ShowProgress();

        if (branchObject != null)
        {
            Destroy(branchObject);
        }

        isDropping = false;
        SetInteractionAvailable(false);
    }

    private IEnumerator ShakeTreeRoutine()
    {
        Quaternion baseRotation = transform.localRotation;
        const float duration = 0.28f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float shake = Mathf.Sin(progress * Mathf.PI * 6f) * (1f - progress);
            transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, shake * 4.2f);
            yield return null;
        }

        transform.localRotation = baseRotation;
    }

    private GameObject CreateBranchObject(Transform parent)
    {
        GameObject branchObject = new GameObject("GeneratedForestBranchReward");
        branchObject.transform.SetParent(parent, true);
        branchObject.transform.localScale = Vector3.one;

        SketchWorldLineDrawing drawing = branchObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(0.045f, new Color32(86, 58, 34, 255), BranchSortingOrder + 2);
        drawing.SetStrokes(BuildBranchStrokes());
        drawing.RevealProgress = 1f;

        CreateBranchLeaf(branchObject.transform, new Vector3(-0.18f, 0.08f, 0f), new Vector3(0.16f, 0.07f, 1f), -22f);
        CreateBranchLeaf(branchObject.transform, new Vector3(0.14f, -0.03f, 0f), new Vector3(0.14f, 0.06f, 1f), 28f);

        return branchObject;
    }

    private static List<Vector3[]> BuildBranchStrokes()
    {
        return new List<Vector3[]>
        {
            Points(-0.48f, -0.1f, -0.16f, 0.02f, 0.18f, -0.04f, 0.5f, 0.12f),
            Points(-0.08f, 0.0f, -0.24f, 0.22f),
            Points(0.18f, -0.04f, 0.32f, -0.24f)
        };
    }

    private static void CreateBranchLeaf(Transform parent, Vector3 localPosition, Vector3 localScale, float rotation)
    {
        GameObject leafObject = new GameObject("GeneratedForestBranchLeaf");
        leafObject.transform.SetParent(parent, false);
        leafObject.transform.localPosition = localPosition;
        leafObject.transform.localScale = localScale;
        leafObject.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);

        SpriteRenderer spriteRenderer = leafObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetPixelSprite();
        spriteRenderer.color = new Color32(76, 142, 58, 220);
        spriteRenderer.sortingOrder = BranchSortingOrder + 1;
    }

    private void EnsurePrompt()
    {
        if (promptObject == null)
        {
            promptObject = new GameObject("GeneratedForestBranchTreePrompt");
            promptObject.transform.SetParent(transform, false);

            TextMesh textMesh = promptObject.AddComponent<TextMesh>();
            textMesh.text = "E";
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.fontSize = 64;
            textMesh.characterSize = 0.075f;
            textMesh.color = new Color32(35, 32, 28, 255);
        }

        promptObject.transform.localPosition = new Vector3(0f, 2.7f * treeScale, 0f);

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
            interactionCollider.radius = interactDistance;
            interactionCollider.offset = new Vector2(0f, 1.05f * treeScale);
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = interactDistance;
        interactionCollider.offset = new Vector2(0f, 1.05f * treeScale);
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = available;
        }

        if (!available && promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private bool CanDropBranch()
    {
        return hasConfigured && !GameProgress.HasCollectedForestBranch;
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
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
        pixelSprite.name = "GeneratedForestBranchRewardPixel";
        return pixelSprite;
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

public class MoleHoleInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private int holeIndex;
    [SerializeField] private float interactDistance = 1.45f;

    private const int MoleSortingOrder = 86;
    private const int CrayonSortingOrder = 82;

    private static Sprite pixelSprite;

    private CircleCollider2D interactionCollider;
    private SketchWorldLineDrawing lineDrawing;
    private Transform player;
    private Transform rewardParent;
    private GameObject promptObject;
    private Vector3 baseScale;
    private int promptSortingOrder = 72;
    private bool hasConfigured;
    private bool isInteracting;

    public void Configure(int index, float distance, int sortingOrder, Transform parent)
    {
        holeIndex = index;
        interactDistance = Mathf.Max(0.2f, distance);
        promptSortingOrder = sortingOrder;
        rewardParent = parent;
        hasConfigured = true;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(CanInteractWithHole());
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || !hasConfigured || isInteracting || !CanInteractWithHole() || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > interactDistance)
        {
            return;
        }

        if (!GameProgress.InteractMoleHole(holeIndex))
        {
            SetInteractionAvailable(false);
            return;
        }

        StartCoroutine(HoleInteractRoutine(interactor.transform));
    }

    private void Awake()
    {
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        baseScale = transform.localScale;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(false);
    }

    private void OnEnable()
    {
        FindPlayer();
        SetInteractionAvailable(CanInteractWithHole());

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!hasConfigured || !CanInteractWithHole())
        {
            SetInteractionAvailable(false);
            return;
        }

        if (player == null)
        {
            FindPlayer();
        }

        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        bool isNear = player != null && Vector2.Distance(transform.position, player.position) <= interactDistance;

        SetInteractionAvailable(isRevealed && !isInteracting);

        if (promptObject != null)
        {
            promptObject.SetActive(isRevealed && isNear && !isInteracting);
        }
    }

    private void OnDisable()
    {
        SetInteractionAvailable(false);
    }

    private IEnumerator HoleInteractRoutine(Transform interactor)
    {
        isInteracting = true;
        SetInteractionAvailable(false);

        const float tapDuration = 0.18f;
        float elapsed = 0f;

        while (elapsed < tapDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / tapDuration);
            float pulse = Mathf.Sin(progress * Mathf.PI);
            transform.localScale = baseScale * Mathf.Lerp(1f, 1.16f, pulse);
            yield return null;
        }

        transform.localScale = baseScale;

        if (GameProgress.HasCompletedMoleHoles && !GameProgress.HasCollectedBrownCrayon)
        {
            yield return StartCoroutine(SpawnMoleAndCrayonRoutine(interactor));
        }

        isInteracting = false;
    }

    private IEnumerator SpawnMoleAndCrayonRoutine(Transform interactor)
    {
        Transform parent = rewardParent != null ? rewardParent : transform.parent;
        Vector3 rewardLocalPosition = parent != null
            ? parent.InverseTransformPoint(transform.position)
            : transform.localPosition;
        Vector3 moleEndLocalPosition = rewardLocalPosition + new Vector3(0f, 0.34f, 0f);
        GameObject moleObject = new GameObject("GeneratedMoleReward");
        moleObject.transform.SetParent(parent, false);
        moleObject.transform.localPosition = moleEndLocalPosition + new Vector3(0f, -0.74f, 0f);

        SketchWorldLineDrawing moleDrawing = moleObject.AddComponent<SketchWorldLineDrawing>();
        moleDrawing.Configure(0.074f, new Color32(58, 37, 25, 255), MoleSortingOrder);
        moleDrawing.SetStrokes(BuildMoleStrokes());
        moleDrawing.RevealProgress = 0f;

        Vector3 moleStart = moleObject.transform.localPosition;
        Vector3 moleEnd = moleEndLocalPosition;
        const float emergeDuration = 0.48f;
        float elapsed = 0f;

        while (elapsed < emergeDuration && moleObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / emergeDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            moleObject.transform.localPosition = Vector3.Lerp(moleStart, moleEnd, easedProgress);
            moleDrawing.RevealProgress = easedProgress;
            yield return null;
        }

        if (moleObject == null)
        {
            yield break;
        }

        moleObject.transform.localPosition = moleEnd;
        moleDrawing.RevealProgress = 1f;
        yield return new WaitForSeconds(0.12f);

        GameObject crayonObject = new GameObject("GeneratedBrownCrayonReward");
        crayonObject.transform.SetParent(parent, false);
        crayonObject.transform.localPosition = moleEndLocalPosition + new Vector3(0f, 0.92f, 0f);
        CreateBrownCrayonVisual(crayonObject.transform);

        Vector3 startPosition = crayonObject.transform.position;
        Vector3 fallbackTargetPosition = startPosition + new Vector3(0f, 0.82f, 0f);
        Vector3 startScale = crayonObject.transform.localScale;
        Quaternion startRotation = crayonObject.transform.localRotation;
        const float collectDuration = 0.52f;
        elapsed = 0f;

        while (elapsed < collectDuration && crayonObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 targetPosition = interactor != null
                ? interactor.position + new Vector3(0f, 0.72f, 0f)
                : fallbackTargetPosition;
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.22f;

            crayonObject.transform.position = position;
            crayonObject.transform.localScale = Vector3.Lerp(startScale, startScale * 0.16f, easedProgress);
            crayonObject.transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, Mathf.Lerp(-16f, 20f, progress));
            yield return null;
        }

        GameProgress.CollectBrownCrayon();
        PencilFragmentHud.ShowCollected();

        if (crayonObject != null)
        {
            Destroy(crayonObject);
        }
    }

    private void EnsurePrompt()
    {
        if (promptObject == null)
        {
            promptObject = new GameObject("GeneratedMoleHolePrompt");
            promptObject.transform.SetParent(transform, false);
            promptObject.transform.localPosition = new Vector3(0f, 0.82f, 0f);

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
            interactionCollider.radius = interactDistance;
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = interactDistance;
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = available;
        }

        if (!available && promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private bool CanInteractWithHole()
    {
        return hasConfigured
            && !GameProgress.HasCollectedBrownCrayon
            && !GameProgress.HasInteractedMoleHole(holeIndex);
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }

    private static List<Vector3[]> BuildMoleStrokes()
    {
        return new List<Vector3[]>
        {
            Points(-0.9f, -0.18f, -0.62f, 0.42f, -0.18f, 0.72f, 0.34f, 0.72f, 0.78f, 0.42f, 0.96f, -0.18f),
            Points(-0.58f, 0.26f, -0.36f, 0.48f, 0.04f, 0.56f, 0.44f, 0.46f, 0.68f, 0.22f),
            Points(-0.64f, 0.38f, -0.82f, 0.8f, -0.46f, 0.72f),
            Points(0.56f, 0.5f, 0.88f, 0.82f, 0.72f, 0.36f),
            EllipsePoints(-0.25f, 0.26f, 0.07f, 0.09f, 12),
            EllipsePoints(0.38f, 0.26f, 0.07f, 0.09f, 12),
            EllipsePoints(0.07f, 0.09f, 0.12f, 0.08f, 14),
            Points(-0.08f, -0.04f, 0.05f, -0.13f, 0.22f, -0.04f),
            Points(-0.04f, -0.14f, -0.02f, -0.3f, 0.12f, -0.3f, 0.16f, -0.14f),
            Points(-0.06f, 0.08f, -0.42f, 0.17f),
            Points(-0.07f, 0.0f, -0.43f, -0.02f),
            Points(0.2f, 0.08f, 0.58f, 0.17f),
            Points(0.2f, 0.0f, 0.58f, -0.02f),
            Points(-0.48f, -0.02f, -0.66f, -0.1f),
            Points(0.62f, -0.02f, 0.8f, -0.1f),
            Points(-1.08f, -0.2f, -0.62f, -0.34f, 0.1f, -0.28f, 0.82f, -0.36f, 1.16f, -0.22f)
        };
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

    private static void CreateBrownCrayonVisual(Transform crayonTransform)
    {
        CreateCrayonPart(crayonTransform, "GeneratedBrownCrayonBody", new Vector3(-0.06f, 0f, 0f), new Vector3(0.62f, 0.14f, 1f), new Color32(151, 87, 42, 255), CrayonSortingOrder);
        CreateCrayonPart(crayonTransform, "GeneratedBrownCrayonTip", new Vector3(0.3f, 0f, 0f), new Vector3(0.14f, 0.1f, 1f), new Color32(93, 54, 31, 255), CrayonSortingOrder + 1);
        CreateCrayonPart(crayonTransform, "GeneratedBrownCrayonWrapper", new Vector3(-0.18f, 0f, 0f), new Vector3(0.16f, 0.16f, 1f), new Color32(229, 185, 121, 255), CrayonSortingOrder + 2);
        CreateCrayonPart(crayonTransform, "GeneratedBrownCrayonBackEdge", new Vector3(-0.42f, 0f, 0f), new Vector3(0.08f, 0.15f, 1f), new Color32(111, 64, 33, 255), CrayonSortingOrder + 2);

        SketchWorldLineDrawing outline = crayonTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        outline.Configure(0.035f, new Color32(66, 42, 25, 255), CrayonSortingOrder + 3);
        outline.SetStrokes(new[]
        {
            Points(-0.48f, -0.09f, 0.22f, -0.09f, 0.44f, 0f, 0.22f, 0.09f, -0.48f, 0.09f, -0.48f, -0.09f),
            Points(0.22f, -0.09f, 0.22f, 0.09f),
            Points(-0.28f, -0.1f, -0.28f, 0.1f)
        });
        outline.RevealProgress = 1f;
    }

    private static void CreateCrayonPart(Transform parent, string objectName, Vector3 localPosition, Vector3 localScale, Color color, int sortingOrder)
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
        pixelSprite.name = "GeneratedMoleBrownCrayonRewardPixel";
        return pixelSprite;
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

public class ForestRockCrackInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private bool containsPencilFragment;
    [SerializeField] private float interactDistance = 1.65f;

    private const int RewardSortingOrder = 78;

    private static Sprite pixelSprite;

    private CircleCollider2D interactionCollider;
    private SketchWorldLineDrawing lineDrawing;
    private Transform player;
    private GameObject promptObject;
    private int promptSortingOrder = 72;
    private bool hasConfigured;
    private bool hasInspected;
    private bool isInspecting;

    public void Configure(bool hasReward, float distance, int sortingOrder)
    {
        containsPencilFragment = hasReward;
        interactDistance = Mathf.Max(0.2f, distance);
        promptSortingOrder = sortingOrder;
        hasConfigured = true;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(CanInspectRock());
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || !hasConfigured || !CanInspectRock() || isInspecting || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > interactDistance)
        {
            return;
        }

        StartCoroutine(InspectRoutine(interactor.transform));
    }

    private void Awake()
    {
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(false);
    }

    private void OnEnable()
    {
        FindPlayer();
        SetInteractionAvailable(CanInspectRock());

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!hasConfigured || !CanInspectRock())
        {
            SetInteractionAvailable(false);
            return;
        }

        if (player == null)
        {
            FindPlayer();
        }

        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        bool isNear = player != null && Vector2.Distance(transform.position, player.position) <= interactDistance;

        SetInteractionAvailable(isRevealed && !isInspecting);

        if (promptObject != null)
        {
            promptObject.SetActive(isRevealed && isNear && !isInspecting);
        }
    }

    private void OnDisable()
    {
        SetInteractionAvailable(false);
    }

    private IEnumerator InspectRoutine(Transform interactor)
    {
        isInspecting = true;
        SetInteractionAvailable(false);

        Vector3 basePosition = transform.localPosition;
        Quaternion baseRotation = transform.localRotation;
        const float duration = 0.42f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float shake = Mathf.Sin(progress * Mathf.PI * 7f) * (1f - progress);
            float lift = Mathf.Sin(progress * Mathf.PI) * 0.04f;
            transform.localPosition = basePosition + new Vector3(shake * 0.07f, lift, 0f);
            transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, shake * 5f);
            yield return null;
        }

        transform.localPosition = basePosition;
        transform.localRotation = baseRotation;
        hasInspected = true;

        if (containsPencilFragment && !GameProgress.HasCollectedPencilFragmentFrom(PencilFragmentSource.RockCrack))
        {
            GameProgress.DropPencilFragmentFrom(PencilFragmentSource.RockCrack);
            yield return DeliverPencilFragmentRoutine(interactor);
        }
        else
        {
            yield return new WaitForSeconds(0.08f);
        }

        isInspecting = false;
        SetInteractionAvailable(CanInspectRock());
    }

    private IEnumerator DeliverPencilFragmentRoutine(Transform interactor)
    {
        Transform itemParent = transform.parent != null ? transform.parent : transform;
        GameObject rewardObject = new GameObject("GeneratedForestRockPencilReward");
        rewardObject.transform.SetParent(itemParent, true);
        rewardObject.transform.position = transform.position + new Vector3(0f, 0.72f, 0f);
        CreatePencilFragmentVisual(rewardObject.transform);

        Vector3 startPosition = rewardObject.transform.position;
        Vector3 fallbackTargetPosition = startPosition + new Vector3(0f, 0.78f, 0f);
        Vector3 startScale = rewardObject.transform.localScale;
        Quaternion startRotation = rewardObject.transform.localRotation;
        const float collectDuration = 0.52f;
        float elapsed = 0f;

        while (elapsed < collectDuration && rewardObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 targetPosition = interactor != null
                ? interactor.position + new Vector3(0f, 0.72f, 0f)
                : fallbackTargetPosition;
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.24f;

            rewardObject.transform.position = position;
            rewardObject.transform.localScale = Vector3.Lerp(startScale, startScale * 0.18f, easedProgress);
            rewardObject.transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, Mathf.Lerp(12f, -24f, progress));
            yield return null;
        }

        GameProgress.CollectPencilFragmentFrom(PencilFragmentSource.RockCrack);
        PencilFragmentHud.ShowCollected();

        if (rewardObject != null)
        {
            Destroy(rewardObject);
        }
    }

    private void EnsurePrompt()
    {
        if (promptObject == null)
        {
            promptObject = new GameObject("GeneratedForestRockPrompt");
            promptObject.transform.SetParent(transform, false);
            promptObject.transform.localPosition = new Vector3(0f, 1.05f, 0f);

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
            interactionCollider.radius = interactDistance;
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = interactDistance;
        interactionCollider.offset = new Vector2(0f, 0.08f);
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = available;
        }

        if (!available && promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private bool CanInspectRock()
    {
        return hasConfigured
            && !GameProgress.HasCollectedPencilFragmentFrom(PencilFragmentSource.RockCrack)
            && (!hasInspected || containsPencilFragment);
    }

    private void CreatePencilFragmentVisual(Transform pencilTransform)
    {
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentBody", new Vector3(-0.06f, 0f, 0f), new Vector3(0.48f, 0.12f, 1f), new Color32(236, 181, 51, 255), RewardSortingOrder);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentWood", new Vector3(0.24f, 0f, 0f), new Vector3(0.16f, 0.12f, 1f), new Color32(203, 143, 77, 255), RewardSortingOrder + 1);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentLead", new Vector3(0.34f, 0f, 0f), new Vector3(0.08f, 0.08f, 1f), new Color32(44, 40, 36, 255), RewardSortingOrder + 2);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentPaintEdge", new Vector3(-0.33f, 0f, 0f), new Vector3(0.08f, 0.13f, 1f), new Color32(190, 66, 72, 255), RewardSortingOrder + 2);

        SketchWorldLineDrawing outline = pencilTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        outline.Configure(0.035f, new Color32(38, 34, 28, 255), RewardSortingOrder + 3);
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
        pixelSprite.name = "GeneratedForestRockPencilRewardPixel";
        return pixelSprite;
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

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }
}

public class ForestFallenBirdInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float interactDistance = 2.15f;

    private const string SearchingPrompt = "[\uBB3C\uBCD1]+[?]";
    private const string CompletePrompt = "[\uBB3C\uBCD1]+[!]";
    private const float PromptPulseSpeed = 4.4f;
    private const int RewardSortingOrder = 78;

    private static Sprite pixelSprite;

    private CircleCollider2D interactionCollider;
    private SketchWorldLineDrawing lineDrawing;
    private Transform player;
    private GameObject promptObject;
    private TextMesh promptText;
    private TextMesh promptInnerGlowText;
    private TextMesh promptOuterGlowText;
    private int promptSortingOrder = 72;
    private bool hasConfigured;
    private bool isReacting;
    private bool isHelping;

    public void Configure(float distance, int sortingOrder)
    {
        interactDistance = Mathf.Max(0.2f, distance);
        promptSortingOrder = sortingOrder;
        hasConfigured = true;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(CanInteractWithFallenBird());
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || !hasConfigured || !CanInteractWithFallenBird() || isHelping || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > interactDistance)
        {
            return;
        }

        if (CanHelpFallenBird())
        {
            StartCoroutine(HelpRoutine(interactor.transform));
            return;
        }

        if (!isReacting)
        {
            StartCoroutine(WeakReactionRoutine());
        }
    }

    private void Awake()
    {
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(false);
    }

    private void OnEnable()
    {
        FindPlayer();
        SetInteractionAvailable(CanInteractWithFallenBird());

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!hasConfigured || !CanInteractWithFallenBird())
        {
            SetInteractionAvailable(false);
            return;
        }

        if (player == null)
        {
            FindPlayer();
        }

        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        bool isNear = player != null && Vector2.Distance(transform.position, player.position) <= interactDistance;

        SetInteractionAvailable(isRevealed && !isHelping);

        if (promptObject != null)
        {
            SetPromptText(GameProgress.HasCollectedWellWaterBottle ? CompletePrompt : SearchingPrompt);
            promptObject.SetActive(isRevealed && isNear && !isHelping);
        }

        UpdatePromptGlow();
    }

    private void OnDisable()
    {
        SetInteractionAvailable(false);
    }

    private IEnumerator WeakReactionRoutine()
    {
        isReacting = true;
        Vector3 basePosition = transform.localPosition;
        Quaternion baseRotation = transform.localRotation;
        const float duration = 0.34f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float shake = Mathf.Sin(progress * Mathf.PI * 4f) * (1f - progress);
            transform.localPosition = basePosition + Vector3.right * shake * 0.05f;
            transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, shake * 4f);
            yield return null;
        }

        transform.localPosition = basePosition;
        transform.localRotation = baseRotation;
        isReacting = false;
    }

    private IEnumerator HelpRoutine(Transform interactor)
    {
        isHelping = true;
        SetInteractionAvailable(false);

        Vector3 basePosition = transform.localPosition;
        Quaternion baseRotation = transform.localRotation;
        const float duration = 0.54f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float lift = Mathf.Sin(progress * Mathf.PI) * 0.12f;
            float wobble = Mathf.Sin(progress * Mathf.PI * 5f) * (1f - progress);
            transform.localPosition = basePosition + new Vector3(wobble * 0.04f, lift, 0f);
            transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, wobble * 4f);
            yield return null;
        }

        transform.localPosition = basePosition;
        transform.localRotation = baseRotation;
        GameProgress.HelpFallenBird();
        yield return DeliverPencilFragmentRoutine(interactor);
        isHelping = false;
        SetInteractionAvailable(false);
    }

    private IEnumerator DeliverPencilFragmentRoutine(Transform interactor)
    {
        if (GameProgress.HasCollectedPencilFragmentFrom(PencilFragmentSource.FallenBird))
        {
            yield break;
        }

        Transform itemParent = transform.parent != null ? transform.parent : transform;
        GameObject rewardObject = new GameObject("GeneratedForestFallenBirdPencilReward");
        rewardObject.transform.SetParent(itemParent, true);
        rewardObject.transform.position = transform.position + new Vector3(0f, 0.72f, 0f);
        CreatePencilFragmentVisual(rewardObject.transform);

        Vector3 startPosition = rewardObject.transform.position;
        Vector3 fallbackTargetPosition = startPosition + new Vector3(0f, 0.78f, 0f);
        Vector3 startScale = rewardObject.transform.localScale;
        Quaternion startRotation = rewardObject.transform.localRotation;
        const float collectDuration = 0.52f;
        float elapsed = 0f;

        while (elapsed < collectDuration && rewardObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 targetPosition = interactor != null
                ? interactor.position + new Vector3(0f, 0.72f, 0f)
                : fallbackTargetPosition;
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.24f;

            rewardObject.transform.position = position;
            rewardObject.transform.localScale = Vector3.Lerp(startScale, startScale * 0.18f, easedProgress);
            rewardObject.transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, Mathf.Lerp(16f, -20f, progress));
            yield return null;
        }

        GameProgress.CollectPencilFragmentFrom(PencilFragmentSource.FallenBird);
        PencilFragmentHud.ShowCollected();

        if (rewardObject != null)
        {
            Destroy(rewardObject);
        }
    }

    private void EnsurePrompt()
    {
        if (promptObject == null)
        {
            promptObject = new GameObject("GeneratedForestFallenBirdPrompt");
            promptObject.transform.SetParent(transform, false);
            promptObject.transform.localPosition = new Vector3(0f, 1.34f, 0f);

            promptOuterGlowText = CreatePromptText(
                "GeneratedForestFallenBirdPromptOuterGlow",
                0.104f,
                new Color(1f, 0.74f, 0.12f, 0.26f),
                promptSortingOrder - 2);
            promptInnerGlowText = CreatePromptText(
                "GeneratedForestFallenBirdPromptInnerGlow",
                0.086f,
                new Color(1f, 0.84f, 0.24f, 0.62f),
                promptSortingOrder - 1);
            promptText = CreatePromptText(
                "GeneratedForestFallenBirdPromptText",
                0.07f,
                new Color32(35, 32, 28, 255),
                promptSortingOrder);
        }

        SetPromptText(GameProgress.HasCollectedWellWaterBottle ? CompletePrompt : SearchingPrompt);
        UpdatePromptSortingOrders();
        promptObject.SetActive(false);
    }

    private TextMesh CreatePromptText(string objectName, float characterSize, Color color, int sortingOrder)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(promptObject.transform, false);
        textObject.transform.localPosition = Vector3.zero;

        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = SearchingPrompt;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 64;
        textMesh.characterSize = characterSize;
        textMesh.color = color;

        MeshRenderer meshRenderer = textObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }

        return textMesh;
    }

    private void SetPromptText(string value)
    {
        if (promptText != null)
        {
            promptText.text = value;
        }

        if (promptInnerGlowText != null)
        {
            promptInnerGlowText.text = value;
        }

        if (promptOuterGlowText != null)
        {
            promptOuterGlowText.text = value;
        }
    }

    private void UpdatePromptGlow()
    {
        if (promptObject == null || !promptObject.activeSelf)
        {
            return;
        }

        float pulse = Mathf.SmoothStep(0f, 1f, (Mathf.Sin(Time.time * PromptPulseSpeed) + 1f) * 0.5f);

        if (promptText != null)
        {
            promptText.color = Color.Lerp(
                new Color32(35, 32, 28, 255),
                new Color32(82, 62, 8, 255),
                pulse);
        }

        if (promptInnerGlowText != null)
        {
            promptInnerGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.02f, 1.12f, pulse);
            promptInnerGlowText.color = new Color(1f, 0.84f, 0.24f, Mathf.Lerp(0.45f, 0.82f, pulse));
        }

        if (promptOuterGlowText != null)
        {
            promptOuterGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.08f, 1.24f, pulse);
            promptOuterGlowText.color = new Color(1f, 0.74f, 0.12f, Mathf.Lerp(0.16f, 0.42f, pulse));
        }
    }

    private void UpdatePromptSortingOrders()
    {
        SetPromptSortingOrder(promptOuterGlowText, promptSortingOrder - 2);
        SetPromptSortingOrder(promptInnerGlowText, promptSortingOrder - 1);
        SetPromptSortingOrder(promptText, promptSortingOrder);
    }

    private static void SetPromptSortingOrder(TextMesh textMesh, int sortingOrder)
    {
        if (textMesh == null)
        {
            return;
        }

        MeshRenderer meshRenderer = textMesh.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }
    }

    private void EnsureCollider()
    {
        if (interactionCollider != null)
        {
            interactionCollider.radius = interactDistance;
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = interactDistance;
        interactionCollider.offset = new Vector2(0f, 0.18f);
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = available;
        }

        if (!available && promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private bool CanHelpFallenBird()
    {
        return hasConfigured
            && GameProgress.HasCollectedWellWaterBottle
            && !GameProgress.HasCollectedPencilFragmentFrom(PencilFragmentSource.FallenBird)
            && !GameProgress.HasHelpedFallenBird;
    }

    private bool CanInteractWithFallenBird()
    {
        return hasConfigured
            && !GameProgress.HasCollectedPencilFragmentFrom(PencilFragmentSource.FallenBird);
    }

    private void CreatePencilFragmentVisual(Transform pencilTransform)
    {
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentBody", new Vector3(-0.06f, 0f, 0f), new Vector3(0.48f, 0.12f, 1f), new Color32(236, 181, 51, 255), RewardSortingOrder);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentWood", new Vector3(0.24f, 0f, 0f), new Vector3(0.16f, 0.12f, 1f), new Color32(203, 143, 77, 255), RewardSortingOrder + 1);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentLead", new Vector3(0.34f, 0f, 0f), new Vector3(0.08f, 0.08f, 1f), new Color32(44, 40, 36, 255), RewardSortingOrder + 2);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentPaintEdge", new Vector3(-0.33f, 0f, 0f), new Vector3(0.08f, 0.13f, 1f), new Color32(190, 66, 72, 255), RewardSortingOrder + 2);

        SketchWorldLineDrawing outline = pencilTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        outline.Configure(0.035f, new Color32(38, 34, 28, 255), RewardSortingOrder + 3);
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
        pixelSprite.name = "GeneratedForestFallenBirdPencilRewardPixel";
        return pixelSprite;
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

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }
}

public class FourthForestRiverInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float showDistance = 3.6f;

    private const string SearchingPrompt = "[\uAC15\uBB3C]+[?]";
    private const float PromptPulseSpeed = 4.4f;

    private static readonly Vector3 PromptLocalPosition = new Vector3(0f, 6.85f, 0f);

    private SketchWorldLineDrawing lineDrawing;
    private Transform player;
    private BoxCollider2D interactionCollider;
    private GameObject promptObject;
    private TextMesh promptText;
    private TextMesh promptInnerGlowText;
    private TextMesh promptOuterGlowText;
    private int promptSortingOrder = 77;
    private bool isInspecting;

    public void Configure(float distance, int sortingOrder)
    {
        showDistance = Mathf.Max(0.5f, distance);
        promptSortingOrder = sortingOrder;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(CanInspectRiver());
    }

    public void Interact(GameObject interactor)
    {
        if (!CanInspectRiver() || !isActiveAndEnabled || isInspecting || interactor == null)
        {
            return;
        }

        StartCoroutine(InspectRiverRoutine());
    }

    private void Awake()
    {
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        EnsurePrompt();
        EnsureCollider();
        SetPromptVisible(false);
        SetInteractionAvailable(false);
    }

    private void OnEnable()
    {
        FindPlayer();
        SetPromptVisible(false);
        SetInteractionAvailable(CanInspectRiver());
    }

    private void Update()
    {
        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        bool canInspect = isRevealed && CanInspectRiver() && !isInspecting;
        SetInteractionAvailable(canInspect);

        if (player == null)
        {
            FindPlayer();
        }

        SetPromptVisible(canInspect && IsPlayerNearRiver());
        UpdatePromptGlow();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isInspecting = false;
        SetPromptVisible(false);
        SetInteractionAvailable(false);
    }

    private IEnumerator InspectRiverRoutine()
    {
        isInspecting = true;
        SetInteractionAvailable(false);
        SetPromptVisible(false);

        Vector3 baseScale = transform.localScale;
        const float rippleDuration = 0.48f;
        float elapsed = 0f;

        while (elapsed < rippleDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / rippleDuration);
            float ripple = Mathf.Sin(progress * Mathf.PI * 4f) * (1f - progress);
            transform.localScale = baseScale * (1f + ripple * 0.025f);
            yield return null;
        }

        transform.localScale = baseScale;
        GameProgress.InspectFourthForestRiver();
        isInspecting = false;
        SetInteractionAvailable(false);
    }

    private void EnsurePrompt()
    {
        if (promptObject != null)
        {
            ApplyPromptSortingOrders();
            return;
        }

        promptObject = new GameObject("GeneratedFourthForestRiverPrompt");
        promptObject.transform.SetParent(transform, false);
        promptObject.transform.localPosition = PromptLocalPosition;

        promptOuterGlowText = CreatePromptText(
            "GeneratedFourthForestRiverPromptOuterGlow",
            0.112f,
            new Color(0.25f, 0.68f, 1f, 0.26f),
            promptSortingOrder - 2);
        promptInnerGlowText = CreatePromptText(
            "GeneratedFourthForestRiverPromptInnerGlow",
            0.092f,
            new Color(0.38f, 0.76f, 1f, 0.62f),
            promptSortingOrder - 1);
        promptText = CreatePromptText(
            "GeneratedFourthForestRiverPromptText",
            0.075f,
            new Color32(28, 34, 41, 255),
            promptSortingOrder);

        promptObject.SetActive(false);
        ApplyPromptSortingOrders();
    }

    private TextMesh CreatePromptText(string objectName, float characterSize, Color color, int sortingOrder)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(promptObject.transform, false);
        textObject.transform.localPosition = Vector3.zero;

        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = SearchingPrompt;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 64;
        textMesh.characterSize = characterSize;
        textMesh.color = color;

        MeshRenderer meshRenderer = textObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }

        return textMesh;
    }

    private void SetPromptVisible(bool visible)
    {
        if (promptObject != null && promptObject.activeSelf != visible)
        {
            promptObject.SetActive(visible);
        }
    }

    private void UpdatePromptGlow()
    {
        if (promptObject == null || !promptObject.activeSelf)
        {
            return;
        }

        float pulse = Mathf.SmoothStep(0f, 1f, (Mathf.Sin(Time.time * PromptPulseSpeed) + 1f) * 0.5f);

        if (promptText != null)
        {
            promptText.color = Color.Lerp(
                new Color32(28, 34, 41, 255),
                new Color32(24, 78, 124, 255),
                pulse);
        }

        if (promptInnerGlowText != null)
        {
            promptInnerGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.02f, 1.12f, pulse);
            promptInnerGlowText.color = new Color(0.38f, 0.76f, 1f, Mathf.Lerp(0.45f, 0.82f, pulse));
        }

        if (promptOuterGlowText != null)
        {
            promptOuterGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.08f, 1.24f, pulse);
            promptOuterGlowText.color = new Color(0.25f, 0.68f, 1f, Mathf.Lerp(0.16f, 0.42f, pulse));
        }
    }

    private void ApplyPromptSortingOrders()
    {
        SetPromptSortingOrder(promptOuterGlowText, promptSortingOrder - 2);
        SetPromptSortingOrder(promptInnerGlowText, promptSortingOrder - 1);
        SetPromptSortingOrder(promptText, promptSortingOrder);
    }

    private static void SetPromptSortingOrder(TextMesh textMesh, int sortingOrder)
    {
        if (textMesh == null)
        {
            return;
        }

        MeshRenderer meshRenderer = textMesh.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }
    }

    private void EnsureCollider()
    {
        if (interactionCollider != null)
        {
            interactionCollider.size = new Vector2(15.2f, 12.3f);
            interactionCollider.offset = Vector2.zero;
            return;
        }

        interactionCollider = gameObject.AddComponent<BoxCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.size = new Vector2(15.2f, 12.3f);
        interactionCollider.offset = Vector2.zero;
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = available && CanInspectRiver();
        }

        if (!available)
        {
            SetPromptVisible(false);
        }
    }

    private bool IsPlayerNearRiver()
    {
        if (player == null)
        {
            return false;
        }

        Vector3 delta = player.position - transform.position;
        return Mathf.Abs(delta.x) <= showDistance + 6.8f && Mathf.Abs(delta.y) <= 6.4f;
    }

    private static bool CanInspectRiver()
    {
        return GameProgress.HasColoredWaterBlue
            && !GameProgress.HasInspectedFourthForestRiver
            && !GameProgress.HasBuiltFourthForestRootBridge;
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }
}

public class FourthForestIslandDoorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float showDistance = 1.85f;

    private const string PromptTextValue = "E";
    private const float PromptPulseSpeed = 4.8f;

    private static readonly Vector3 PromptLocalPosition = new Vector3(0f, 2.28f, 0f);

    private SketchWorldLineDrawing lineDrawing;
    private Transform player;
    private BoxCollider2D interactionCollider;
    private GameObject promptObject;
    private TextMesh promptText;
    private TextMesh promptInnerGlowText;
    private TextMesh promptOuterGlowText;
    private int promptSortingOrder = 88;
    private bool isEnding;

    public void Configure(float distance, int sortingOrder)
    {
        showDistance = Mathf.Max(0.5f, distance);
        promptSortingOrder = sortingOrder;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(CanEndGame());
    }

    public void Interact(GameObject interactor)
    {
        if (!CanEndGame() || !isActiveAndEnabled || isEnding || interactor == null)
        {
            return;
        }

        StartCoroutine(EndGameRoutine(interactor));
    }

    private void Awake()
    {
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        EnsurePrompt();
        EnsureCollider();
        SetPromptVisible(false);
        SetInteractionAvailable(false);
    }

    private void OnEnable()
    {
        FindPlayer();
        SetPromptVisible(false);
        SetInteractionAvailable(CanEndGame());
    }

    private void Update()
    {
        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        bool canEnd = isRevealed && CanEndGame() && !isEnding && !FinalGameEndingController.IsEnding;
        SetInteractionAvailable(canEnd);

        if (player == null)
        {
            FindPlayer();
        }

        SetPromptVisible(canEnd && IsPlayerNearDoor());
        UpdatePromptGlow();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isEnding = false;
        SetPromptVisible(false);
        SetInteractionAvailable(false);
    }

    private IEnumerator EndGameRoutine(GameObject interactor)
    {
        isEnding = true;
        SetInteractionAvailable(false);
        SetPromptVisible(false);

        Vector3 baseScale = transform.localScale;
        const float doorPulseDuration = 0.42f;
        float elapsed = 0f;

        while (elapsed < doorPulseDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / doorPulseDuration);
            float pulse = Mathf.Sin(progress * Mathf.PI) * 0.045f;
            transform.localScale = baseScale * (1f + pulse);
            yield return null;
        }

        transform.localScale = baseScale;
        FinalGameEndingController.ShowEnding(interactor);
    }

    private void EnsurePrompt()
    {
        if (promptObject != null)
        {
            ApplyPromptSortingOrders();
            return;
        }

        promptObject = new GameObject("GeneratedFourthForestIslandDoorPrompt");
        promptObject.transform.SetParent(transform, false);
        promptObject.transform.localPosition = PromptLocalPosition;

        promptOuterGlowText = CreatePromptText(
            "GeneratedFourthForestIslandDoorPromptOuterGlow",
            0.11f,
            new Color(0.3f, 0.7f, 1f, 0.25f),
            promptSortingOrder - 2);
        promptInnerGlowText = CreatePromptText(
            "GeneratedFourthForestIslandDoorPromptInnerGlow",
            0.092f,
            new Color(0.58f, 0.88f, 1f, 0.58f),
            promptSortingOrder - 1);
        promptText = CreatePromptText(
            "GeneratedFourthForestIslandDoorPromptText",
            0.078f,
            new Color32(31, 34, 39, 255),
            promptSortingOrder);

        promptObject.SetActive(false);
        ApplyPromptSortingOrders();
    }

    private TextMesh CreatePromptText(string objectName, float characterSize, Color color, int sortingOrder)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(promptObject.transform, false);
        textObject.transform.localPosition = Vector3.zero;

        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = PromptTextValue;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 64;
        textMesh.characterSize = characterSize;
        textMesh.color = color;

        MeshRenderer meshRenderer = textObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }

        return textMesh;
    }

    private void SetPromptVisible(bool visible)
    {
        if (promptObject != null && promptObject.activeSelf != visible)
        {
            promptObject.SetActive(visible);
        }
    }

    private void UpdatePromptGlow()
    {
        if (promptObject == null || !promptObject.activeSelf)
        {
            return;
        }

        float pulse = Mathf.SmoothStep(0f, 1f, (Mathf.Sin(Time.time * PromptPulseSpeed) + 1f) * 0.5f);

        if (promptText != null)
        {
            promptText.color = Color.Lerp(
                new Color32(31, 34, 39, 255),
                new Color32(34, 92, 140, 255),
                pulse);
        }

        if (promptInnerGlowText != null)
        {
            promptInnerGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.02f, 1.16f, pulse);
            promptInnerGlowText.color = new Color(0.58f, 0.88f, 1f, Mathf.Lerp(0.42f, 0.82f, pulse));
        }

        if (promptOuterGlowText != null)
        {
            promptOuterGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.1f, 1.34f, pulse);
            promptOuterGlowText.color = new Color(0.3f, 0.7f, 1f, Mathf.Lerp(0.14f, 0.44f, pulse));
        }
    }

    private void ApplyPromptSortingOrders()
    {
        SetPromptSortingOrder(promptOuterGlowText, promptSortingOrder - 2);
        SetPromptSortingOrder(promptInnerGlowText, promptSortingOrder - 1);
        SetPromptSortingOrder(promptText, promptSortingOrder);
    }

    private static void SetPromptSortingOrder(TextMesh textMesh, int sortingOrder)
    {
        if (textMesh == null)
        {
            return;
        }

        MeshRenderer meshRenderer = textMesh.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }
    }

    private void EnsureCollider()
    {
        if (interactionCollider != null)
        {
            interactionCollider.size = new Vector2(2.45f, 3.75f);
            interactionCollider.offset = new Vector2(0f, 0.05f);
            return;
        }

        interactionCollider = gameObject.AddComponent<BoxCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.size = new Vector2(2.45f, 3.75f);
        interactionCollider.offset = new Vector2(0f, 0.05f);
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = available && CanEndGame();
        }

        if (!available)
        {
            SetPromptVisible(false);
        }
    }

    private bool IsPlayerNearDoor()
    {
        if (player == null)
        {
            return false;
        }

        return Vector2.Distance(player.position, transform.position) <= showDistance;
    }

    private static bool CanEndGame()
    {
        return GameProgress.HasColoredWaterBlue
            && GameProgress.HasBuiltFourthForestRootBridge;
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }
}

public class FinalGameEndingController : MonoBehaviour
{
    private const string EndSceneName = "EndScene";

    private static FinalGameEndingController instance;

    public static bool IsEnding { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnSubsystemRegistration()
    {
        ResetEndingState();
    }

    public static void ResetEndingState()
    {
        instance = null;
        IsEnding = false;
    }

    public static void ShowEnding(GameObject interactor)
    {
        if (instance == null)
        {
            GameObject controllerObject = new GameObject("GeneratedFinalGameEndingController");
            instance = controllerObject.AddComponent<FinalGameEndingController>();
        }

        instance.BeginEnding(interactor);
    }

    private void BeginEnding(GameObject interactor)
    {
        if (IsEnding)
        {
            return;
        }

        IsEnding = true;
        FreezePlayer(interactor);
        UnityEngine.SceneManagement.SceneManager.LoadScene(EndSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private static void FreezePlayer(GameObject interactor)
    {
        if (interactor == null)
        {
            return;
        }

        PlayerMove2D playerMove = interactor.GetComponent<PlayerMove2D>();
        if (playerMove != null)
        {
            playerMove.ForceIdleMotion();
            playerMove.enabled = false;
        }

        Rigidbody2D rigidbody2D = interactor.GetComponent<Rigidbody2D>();
        if (rigidbody2D != null)
        {
            rigidbody2D.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator PlayEndingRoutine()
    {
        CanvasGroup group = CreateEndingCanvas();
        if (group == null)
        {
            yield break;
        }

        const float fadeDuration = 1.2f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / fadeDuration));
            yield return null;
        }

        group.alpha = 1f;
    }

    private static CanvasGroup CreateEndingCanvas()
    {
        GameObject canvasObject = new GameObject("GeneratedFinalEndingCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5000;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        CanvasGroup group = canvasObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;

        RectTransform cover = CreateImage("GeneratedFinalEndingCover", canvasRect, new Color(0.02f, 0.025f, 0.03f, 0.92f));
        Stretch(cover);

        Text title = CreateText("GeneratedFinalEndingTitle", canvasRect, "THE END", 96, FontStyle.Bold, new Color32(246, 244, 231, 255));
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0f, 46f);
        titleRect.sizeDelta = new Vector2(720f, 130f);

        Text subtitle = CreateText("GeneratedFinalEndingSubtitle", canvasRect, "\uC2A4\uCF00\uCE58\uAC00 \uC644\uC131\uB418\uC5C8\uC2B5\uB2C8\uB2E4", 38, FontStyle.Normal, new Color32(212, 229, 238, 255));
        RectTransform subtitleRect = subtitle.GetComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0.5f, 0.5f);
        subtitleRect.anchorMax = new Vector2(0.5f, 0.5f);
        subtitleRect.pivot = new Vector2(0.5f, 0.5f);
        subtitleRect.anchoredPosition = new Vector2(0f, -54f);
        subtitleRect.sizeDelta = new Vector2(860f, 78f);

        return group;
    }

    private static RectTransform CreateImage(string objectName, RectTransform parent, Color color)
    {
        GameObject imageObject = new GameObject(objectName);
        RectTransform rectTransform = imageObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent, false);

        Image image = imageObject.AddComponent<Image>();
        image.sprite = CreateSolidSprite(8, 8, Color.white);
        image.color = color;
        image.raycastTarget = true;
        return rectTransform;
    }

    private static Text CreateText(string objectName, RectTransform parent, string text, int fontSize, FontStyle fontStyle, Color color)
    {
        GameObject textObject = new GameObject(objectName);
        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent, false);

        Text label = textObject.AddComponent<Text>();
        label.text = text;
        label.font = LoadFont();
        label.fontSize = fontSize;
        label.fontStyle = fontStyle;
        label.color = color;
        label.alignment = TextAnchor.MiddleCenter;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        label.raycastTarget = false;
        return label;
    }

    private static Font LoadFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        return font;
    }

    private static void Stretch(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    private static Sprite CreateSolidSprite(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }

        texture.SetPixels(pixels);
        texture.Apply(false, true);
        return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
    }
}

public class FourthForestBigTreePrompt : MonoBehaviour, IInteractable
{
    [SerializeField] private float showDistance = 6.2f;

    private const string SearchingPrompt = "[\uBE5B\uB098\uB294 \uC78E]+[?]";
    private const string CompletePrompt = "[\uBE5B\uB098\uB294 \uC78E]+[!]";
    private const string RootBridgePrompt = "[\uBFCC\uB9AC]+[?]";
    private const float PromptPulseSpeed = 4.4f;
    private const int BlueCrayonSortingOrder = 84;

    private static readonly Vector3 PromptLocalPosition = new Vector3(0f, 7.25f, 0f);

    private static Sprite blueCrayonPixelSprite;

    private SketchWorldLineDrawing lineDrawing;
    private Transform player;
    private CircleCollider2D interactionCollider;
    private GameObject promptObject;
    private TextMesh promptText;
    private TextMesh promptInnerGlowText;
    private TextMesh promptOuterGlowText;
    private int promptSortingOrder = 77;
    private bool isStartingQuest;

    public void Configure(float distance, int sortingOrder)
    {
        showDistance = Mathf.Max(0.2f, distance);
        promptSortingOrder = sortingOrder;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(true);
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || isStartingQuest || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > showDistance)
        {
            return;
        }

        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        if (!isRevealed)
        {
            return;
        }

        if (CanGrantBlueCrayon())
        {
            StartCoroutine(GrantBlueCrayonRoutine(interactor.transform));
            return;
        }

        if (CanGrowRootBridge())
        {
            StartCoroutine(GrowRootBridgeRoutine());
            return;
        }

        if (GameProgress.HasCollectedGlowingLeaf || GameProgress.HasStartedGlowingLeafQuest)
        {
            return;
        }

        StartCoroutine(StartGlowingLeafQuestRoutine());
    }

    private void Awake()
    {
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        EnsurePrompt();
        EnsureCollider();
        SetPromptVisible(false);
        SetInteractionAvailable(false);
    }

    private void OnEnable()
    {
        FindPlayer();
        SetPromptVisible(false);
        SetInteractionAvailable(true);
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
        }

        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        bool isNear = player != null && Vector2.Distance(transform.position, player.position) <= showDistance;

        bool canStartLeafQuest = !GameProgress.HasCollectedGlowingLeaf && !GameProgress.HasStartedGlowingLeafQuest;
        bool canGrantBlueCrayon = CanGrantBlueCrayon();
        bool canGrowRootBridge = CanGrowRootBridge();
        bool canInteract = isRevealed && !isStartingQuest && (canStartLeafQuest || canGrantBlueCrayon || canGrowRootBridge);

        SetPromptText(canGrowRootBridge ? RootBridgePrompt : canGrantBlueCrayon ? CompletePrompt : SearchingPrompt);
        SetInteractionAvailable(canInteract);
        SetPromptVisible(canInteract && isNear);
        UpdatePromptGlow();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isStartingQuest = false;
        SetPromptVisible(false);
        SetInteractionAvailable(false);
    }

    private IEnumerator StartGlowingLeafQuestRoutine()
    {
        isStartingQuest = true;
        SetInteractionAvailable(false);
        SetPromptVisible(false);

        Vector3 startPosition = transform.position + new Vector3(0f, 8.2f, 0f);
        yield return GlowingLeafQuestRoute.PlayLeafFromBigTree(startPosition, transform.parent);

        isStartingQuest = false;
        SetInteractionAvailable(true);
    }

    private IEnumerator GrantBlueCrayonRoutine(Transform interactor)
    {
        isStartingQuest = true;
        SetInteractionAvailable(false);
        SetPromptVisible(false);

        Vector3 baseLocalPosition = transform.localPosition;
        Quaternion baseLocalRotation = transform.localRotation;
        const float pulseDuration = 0.42f;
        float elapsed = 0f;

        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / pulseDuration);
            float pulse = Mathf.Sin(progress * Mathf.PI);
            transform.localPosition = baseLocalPosition + Vector3.up * pulse * 0.08f;
            transform.localRotation = baseLocalRotation * Quaternion.Euler(0f, 0f, Mathf.Sin(progress * Mathf.PI * 6f) * (1f - progress) * 3.2f);
            yield return null;
        }

        transform.localPosition = baseLocalPosition;
        transform.localRotation = baseLocalRotation;

        Transform parent = transform.parent;
        GameObject crayonObject = new GameObject("GeneratedBlueCrayonReward");
        if (parent != null)
        {
            crayonObject.transform.SetParent(parent, false);
            crayonObject.transform.localPosition = parent.InverseTransformPoint(transform.position + new Vector3(0f, 5.5f, 0f));
        }
        else
        {
            crayonObject.transform.position = transform.position + new Vector3(0f, 5.5f, 0f);
        }

        CreateBlueCrayonVisual(crayonObject.transform);

        Vector3 startPosition = crayonObject.transform.position;
        Vector3 fallbackTargetPosition = startPosition + new Vector3(0f, 0.82f, 0f);
        Vector3 startScale = crayonObject.transform.localScale;
        Quaternion startRotation = crayonObject.transform.localRotation;
        const float collectDuration = 0.58f;
        elapsed = 0f;

        while (elapsed < collectDuration && crayonObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 targetPosition = interactor != null
                ? interactor.position + new Vector3(0f, 0.72f, 0f)
                : fallbackTargetPosition;
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.24f;

            crayonObject.transform.position = position;
            crayonObject.transform.localScale = Vector3.Lerp(startScale, startScale * 0.16f, easedProgress);
            crayonObject.transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, Mathf.Lerp(-14f, 22f, progress));
            yield return null;
        }

        if (crayonObject != null)
        {
            Destroy(crayonObject);
        }

        GameProgress.CollectBlueCrayon();
        PencilFragmentHud.ShowCollected();
        isStartingQuest = false;
        SetInteractionAvailable(false);
    }

    private IEnumerator GrowRootBridgeRoutine()
    {
        isStartingQuest = true;
        SetInteractionAvailable(false);
        SetPromptVisible(false);

        Vector3 baseLocalPosition = transform.localPosition;
        Quaternion baseLocalRotation = transform.localRotation;
        const float wakeDuration = 0.5f;
        float elapsed = 0f;

        while (elapsed < wakeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / wakeDuration);
            float shake = Mathf.Sin(progress * Mathf.PI * 8f) * (1f - progress);
            transform.localPosition = baseLocalPosition + Vector3.right * shake * 0.08f;
            transform.localRotation = baseLocalRotation * Quaternion.Euler(0f, 0f, shake * 3.4f);
            yield return null;
        }

        transform.localPosition = baseLocalPosition;
        transform.localRotation = baseLocalRotation;

        yield return SketchOutsideTransition.PlayFourthForestRootBridgeFromBigTree();

        isStartingQuest = false;
        SetInteractionAvailable(false);
    }

    private static bool CanGrantBlueCrayon()
    {
        return GameProgress.HasCollectedGlowingLeaf
            && !GameProgress.HasCollectedBlueCrayon
            && !GameProgress.HasUsedBlueCrayon;
    }

    private static bool CanGrowRootBridge()
    {
        return GameProgress.HasInspectedFourthForestRiver
            && !GameProgress.HasBuiltFourthForestRootBridge;
    }

    private static void CreateBlueCrayonVisual(Transform crayonTransform)
    {
        CreateCrayonPart(crayonTransform, "GeneratedBlueCrayonBody", new Vector3(-0.06f, 0f, 0f), new Vector3(0.62f, 0.14f, 1f), new Color32(55, 145, 218, 255), BlueCrayonSortingOrder);
        CreateCrayonPart(crayonTransform, "GeneratedBlueCrayonTip", new Vector3(0.3f, 0f, 0f), new Vector3(0.14f, 0.1f, 1f), new Color32(30, 83, 153, 255), BlueCrayonSortingOrder + 1);
        CreateCrayonPart(crayonTransform, "GeneratedBlueCrayonWrapper", new Vector3(-0.18f, 0f, 0f), new Vector3(0.16f, 0.16f, 1f), new Color32(180, 223, 248, 255), BlueCrayonSortingOrder + 2);
        CreateCrayonPart(crayonTransform, "GeneratedBlueCrayonBackEdge", new Vector3(-0.42f, 0f, 0f), new Vector3(0.08f, 0.15f, 1f), new Color32(35, 102, 176, 255), BlueCrayonSortingOrder + 2);

        SketchWorldLineDrawing outline = crayonTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        outline.Configure(0.035f, new Color32(31, 62, 91, 255), BlueCrayonSortingOrder + 3);
        outline.SetStrokes(new[]
        {
            Points(-0.48f, -0.09f, 0.22f, -0.09f, 0.44f, 0f, 0.22f, 0.09f, -0.48f, 0.09f, -0.48f, -0.09f),
            Points(0.22f, -0.09f, 0.22f, 0.09f),
            Points(-0.28f, -0.1f, -0.28f, 0.1f)
        });
        outline.RevealProgress = 1f;
    }

    private static void CreateCrayonPart(Transform parent, string objectName, Vector3 localPosition, Vector3 localScale, Color color, int sortingOrder)
    {
        GameObject partObject = new GameObject(objectName);
        partObject.transform.SetParent(parent, false);
        partObject.transform.localPosition = localPosition;
        partObject.transform.localScale = localScale;

        SpriteRenderer renderer = partObject.AddComponent<SpriteRenderer>();
        renderer.sprite = GetBlueCrayonPixelSprite();
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;
    }

    private static Sprite GetBlueCrayonPixelSprite()
    {
        if (blueCrayonPixelSprite != null)
        {
            return blueCrayonPixelSprite;
        }

        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        blueCrayonPixelSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        blueCrayonPixelSprite.name = "GeneratedBlueCrayonRewardPixel";
        return blueCrayonPixelSprite;
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

    private void EnsurePrompt()
    {
        if (promptObject != null)
        {
            ApplyPromptSortingOrders();
            return;
        }

        promptObject = new GameObject("GeneratedFourthForestBigTreePrompt");
        promptObject.transform.SetParent(transform, false);
        promptObject.transform.localPosition = PromptLocalPosition;

        promptOuterGlowText = CreatePromptText(
            "GeneratedFourthForestBigTreePromptOuterGlow",
            0.112f,
            new Color(1f, 0.74f, 0.12f, 0.26f),
            promptSortingOrder - 2);
        promptInnerGlowText = CreatePromptText(
            "GeneratedFourthForestBigTreePromptInnerGlow",
            0.092f,
            new Color(1f, 0.84f, 0.24f, 0.62f),
            promptSortingOrder - 1);
        promptText = CreatePromptText(
            "GeneratedFourthForestBigTreePromptText",
            0.075f,
            new Color32(35, 32, 28, 255),
            promptSortingOrder);

        promptObject.SetActive(false);
        ApplyPromptSortingOrders();
    }

    private TextMesh CreatePromptText(string objectName, float characterSize, Color color, int sortingOrder)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(promptObject.transform, false);
        textObject.transform.localPosition = Vector3.zero;

        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = SearchingPrompt;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 64;
        textMesh.characterSize = characterSize;
        textMesh.color = color;

        MeshRenderer meshRenderer = textObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }

        return textMesh;
    }

    private void SetPromptText(string value)
    {
        if (promptText != null)
        {
            promptText.text = value;
        }

        if (promptInnerGlowText != null)
        {
            promptInnerGlowText.text = value;
        }

        if (promptOuterGlowText != null)
        {
            promptOuterGlowText.text = value;
        }
    }

    private void SetPromptVisible(bool visible)
    {
        if (promptObject != null && promptObject.activeSelf != visible)
        {
            promptObject.SetActive(visible);
        }
    }

    private void UpdatePromptGlow()
    {
        if (promptObject == null || !promptObject.activeSelf)
        {
            return;
        }

        float pulse = Mathf.SmoothStep(0f, 1f, (Mathf.Sin(Time.time * PromptPulseSpeed) + 1f) * 0.5f);

        if (promptText != null)
        {
            promptText.color = Color.Lerp(
                new Color32(35, 32, 28, 255),
                new Color32(82, 62, 8, 255),
                pulse);
        }

        if (promptInnerGlowText != null)
        {
            promptInnerGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.02f, 1.12f, pulse);
            promptInnerGlowText.color = new Color(1f, 0.84f, 0.24f, Mathf.Lerp(0.45f, 0.82f, pulse));
        }

        if (promptOuterGlowText != null)
        {
            promptOuterGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.08f, 1.24f, pulse);
            promptOuterGlowText.color = new Color(1f, 0.74f, 0.12f, Mathf.Lerp(0.16f, 0.42f, pulse));
        }
    }

    private void ApplyPromptSortingOrders()
    {
        SetPromptSortingOrder(promptOuterGlowText, promptSortingOrder - 2);
        SetPromptSortingOrder(promptInnerGlowText, promptSortingOrder - 1);
        SetPromptSortingOrder(promptText, promptSortingOrder);
    }

    private static void SetPromptSortingOrder(TextMesh textMesh, int sortingOrder)
    {
        if (textMesh == null)
        {
            return;
        }

        MeshRenderer meshRenderer = textMesh.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }
    }

    private void EnsureCollider()
    {
        if (interactionCollider != null)
        {
            interactionCollider.radius = showDistance;
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = showDistance;
        interactionCollider.offset = new Vector2(0f, 3.6f);
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = available;
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }
}

public class FourthForestGlowingLeafTreeInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float promptDistance = 1.9f;

    private CircleCollider2D interactionCollider;
    private SketchWorldLineDrawing lineDrawing;
    private Transform player;
    private GameObject promptObject;
    private int treeIndex = -1;
    private int promptSortingOrder = 77;
    private float treeScale = 1f;
    private bool isCollecting;

    public void Configure(int index, float scale, float distance, int sortingOrder)
    {
        treeIndex = index;
        treeScale = Mathf.Max(0.1f, scale);
        promptDistance = Mathf.Max(distance, 1.65f * treeScale);
        promptSortingOrder = sortingOrder;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(false);
    }

    public void Interact(GameObject interactor)
    {
        if (!CanCollectGlowingLeaf() || !IsTreeRevealed() || !isActiveAndEnabled || isCollecting || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > promptDistance)
        {
            return;
        }

        StartCoroutine(CollectGlowingLeafRoutine(interactor.transform));
    }

    private void Awake()
    {
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(false);
    }

    private void OnEnable()
    {
        FindPlayer();
        SetInteractionAvailable(CanCollectGlowingLeaf() && IsTreeRevealed());

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        bool isRevealed = IsTreeRevealed();
        bool canCollect = CanCollectGlowingLeaf() && isRevealed && !isCollecting;
        SetInteractionAvailable(canCollect);

        if (!canCollect)
        {
            return;
        }

        GlowingLeafQuestRoute.UpdateActiveLeafVisibility();

        if (player == null)
        {
            FindPlayer();
        }

        if (promptObject != null)
        {
            bool shouldShowPrompt = player != null && Vector2.Distance(transform.position, player.position) <= promptDistance;
            promptObject.SetActive(shouldShowPrompt);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isCollecting = false;
        SetInteractionAvailable(false);

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private IEnumerator CollectGlowingLeafRoutine(Transform interactor)
    {
        isCollecting = true;
        SetInteractionAvailable(false);

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }

        Vector3 basePosition = transform.localPosition;
        Quaternion baseRotation = transform.localRotation;
        const float shakeDuration = 0.48f;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / shakeDuration);
            float fade = 1f - Mathf.SmoothStep(0f, 1f, progress);
            float shake = Mathf.Sin(progress * Mathf.PI * 9f) * fade;
            transform.localPosition = basePosition + Vector3.right * shake * 0.05f * treeScale;
            transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, shake * 5.2f);
            yield return null;
        }

        transform.localPosition = basePosition;
        transform.localRotation = baseRotation;

        yield return GlowingLeafQuestRoute.CollectLeafFromTree(treeIndex, transform, interactor);
        GameProgress.CollectGlowingLeaf();
        LetterQuestHud.ShowProgress();
        GlowingLeafQuestRoute.UpdateActiveLeafVisibility();
        isCollecting = false;
        SetInteractionAvailable(false);
    }

    private void EnsurePrompt()
    {
        if (promptObject == null)
        {
            promptObject = new GameObject("GeneratedFourthForestGlowingLeafTreePrompt");
            promptObject.transform.SetParent(transform, false);

            TextMesh textMesh = promptObject.AddComponent<TextMesh>();
            textMesh.text = "E";
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.fontSize = 64;
            textMesh.characterSize = 0.075f;
            textMesh.color = new Color32(35, 32, 28, 255);
        }

        promptObject.transform.localPosition = new Vector3(0f, 2.75f * treeScale, 0f);

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
            UpdateColliderSize();
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        UpdateColliderSize();
    }

    private void UpdateColliderSize()
    {
        if (interactionCollider == null)
        {
            return;
        }

        interactionCollider.radius = 1.22f * treeScale;
        interactionCollider.offset = new Vector2(0f, 1.15f * treeScale);
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = available && CanCollectGlowingLeaf();
        }

        if (!available && promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private bool CanCollectGlowingLeaf()
    {
        return treeIndex >= 0
            && !GameProgress.HasCollectedBlueCrayon
            && !GameProgress.HasUsedBlueCrayon
            && GameProgress.IsGlowingLeafWaitingOnTree(treeIndex);
    }

    private bool IsTreeRevealed()
    {
        return lineDrawing != null && lineDrawing.RevealProgress >= 0.95f;
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }
}

public class GlowingLeafTreeGlow : MonoBehaviour
{
    private SpriteRenderer glowRenderer;
    private Vector3 baseScale = Vector3.one;
    private Color baseColor = Color.white;

    public void Configure(SpriteRenderer renderer)
    {
        glowRenderer = renderer;

        if (glowRenderer != null)
        {
            baseScale = glowRenderer.transform.localScale;
            baseColor = glowRenderer.color;
        }
    }

    private void Update()
    {
        if (glowRenderer == null)
        {
            return;
        }

        float pulse = Mathf.SmoothStep(0f, 1f, (Mathf.Sin(Time.time * 4.8f) + 1f) * 0.5f);
        glowRenderer.transform.localScale = baseScale * Mathf.Lerp(0.92f, 1.18f, pulse);
        glowRenderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Lerp(0.26f, 0.58f, pulse));
    }
}

public static class GlowingLeafQuestRoute
{
    private const int GlowingLeafSortingOrder = 84;
    private const float FlightDuration = 2.15f;
    private const float CollectDuration = 0.62f;

    private static readonly List<RouteTree> routeTrees = new List<RouteTree>();
    private static Sprite glowSprite;

    public static void ResetRoute()
    {
        for (int i = 0; i < routeTrees.Count; i++)
        {
            if (routeTrees[i].GlowObject != null)
            {
                Object.Destroy(routeTrees[i].GlowObject);
            }
        }

        routeTrees.Clear();
    }

    public static int RegisterTree(Vector3 localPosition, Transform parent, SketchWorldLineDrawing treeDrawing, float treeScale)
    {
        int index = routeTrees.Count;
        RouteTree routeTree = new RouteTree(localPosition, parent, treeDrawing, Mathf.Max(0.1f, treeScale));
        routeTrees.Add(routeTree);

        if (GameProgress.IsGlowingLeafWaitingOnTree(index))
        {
            EnsureTreeGlow(index);
        }

        return index;
    }

    public static void ReassignActiveQuestToPreferredTarget()
    {
        ReassignActiveQuestToPreferredTargetIfNeeded();
        UpdateActiveLeafVisibility();
    }

    public static IEnumerator PlayLeafFromBigTree(Vector3 startWorldPosition, Transform fallbackParent)
    {
        if (routeTrees.Count <= 0 || GameProgress.HasStartedGlowingLeafQuest || GameProgress.HasCollectedGlowingLeaf)
        {
            yield break;
        }

        int targetIndex = PickTargetTreeIndex();
        GameProgress.StartGlowingLeafQuest(targetIndex);
        yield return AnimateLeafToTreeRoutine(startWorldPosition, targetIndex, fallbackParent);
        EnsureTreeGlow(targetIndex);
    }

    public static IEnumerator CollectLeafFromTree(int treeIndex, Transform treeTransform, Transform playerTransform)
    {
        if (!GameProgress.IsGlowingLeafWaitingOnTree(treeIndex))
        {
            yield break;
        }

        RouteTree routeTree = GetRouteTree(treeIndex);
        Transform parent = routeTree.Parent != null ? routeTree.Parent : (treeTransform != null ? treeTransform.parent : null);
        Vector3 startPosition = GetTreeLeafWorldPosition(treeIndex);
        Vector3 targetPosition = playerTransform != null
            ? playerTransform.position + new Vector3(0f, 0.74f, 0f)
            : startPosition + new Vector3(0f, 0.88f, 0f);

        RemoveTreeGlow(treeIndex);

        GameObject leafObject = CreateGlowingLeafObject("GeneratedCollectedGlowingLeaf", parent, startPosition, 0.72f);
        WindMotionEffect windEffect = WindMotionEffect.Create(parent, startPosition, GlowingLeafSortingOrder - 2);
        Vector3 previousPosition = startPosition;
        float elapsed = 0f;

        while (elapsed < CollectDuration && leafObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / CollectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.34f;

            Vector3 movementDirection = position - previousPosition;
            leafObject.transform.position = position;
            leafObject.transform.localScale = Vector3.one * Mathf.Lerp(0.72f, 0.18f, easedProgress);
            leafObject.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-20f, 28f, progress) + Mathf.Sin(progress * Mathf.PI * 6f) * 9f);
            windEffect.UpdateEffect(position, movementDirection, progress, Mathf.Lerp(0.8f, 0.35f, easedProgress));
            previousPosition = position;
            yield return null;
        }

        if (windEffect != null)
        {
            windEffect.Finish();
        }

        if (leafObject != null)
        {
            Object.Destroy(leafObject);
        }
    }

    public static void UpdateActiveLeafVisibility()
    {
        ReassignActiveQuestToPreferredTargetIfNeeded();

        for (int i = 0; i < routeTrees.Count; i++)
        {
            if (GameProgress.IsGlowingLeafWaitingOnTree(i))
            {
                EnsureTreeGlow(i);
            }
            else
            {
                RemoveTreeGlow(i);
            }
        }
    }

    private static void ReassignActiveQuestToPreferredTargetIfNeeded()
    {
        if (routeTrees.Count <= 0 || !GameProgress.HasStartedGlowingLeafQuest || GameProgress.HasCollectedGlowingLeaf)
        {
            return;
        }

        int targetIndex = PickTargetTreeIndex();
        int currentIndex = GameProgress.ActiveGlowingLeafTreeIndex;
        if (currentIndex == targetIndex)
        {
            return;
        }

        RemoveTreeGlow(currentIndex);
        GameProgress.ReassignGlowingLeafQuestTarget(targetIndex);
    }

    private static IEnumerator AnimateLeafToTreeRoutine(Vector3 startWorldPosition, int targetTreeIndex, Transform fallbackParent)
    {
        RouteTree targetTree = GetRouteTree(targetTreeIndex);
        Transform parent = targetTree.Parent != null ? targetTree.Parent : fallbackParent;
        Vector3 targetPosition = GetTreeLeafWorldPosition(targetTreeIndex);
        GameObject leafObject = CreateGlowingLeafObject("GeneratedWindCarriedGlowingLeaf", parent, startWorldPosition, 0.92f);
        WindMotionEffect windEffect = WindMotionEffect.Create(parent, startWorldPosition, GlowingLeafSortingOrder - 2);
        Vector3 travelDirection = (targetPosition - startWorldPosition).normalized;
        Vector3 crosswindDirection = new Vector3(-travelDirection.y, travelDirection.x, 0f);
        Vector3 previousPosition = startWorldPosition;
        float elapsed = 0f;

        while (elapsed < FlightDuration && leafObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / FlightDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            float gustEnvelope = Mathf.Sin(progress * Mathf.PI);
            Vector3 position = Vector3.Lerp(startWorldPosition, targetPosition, easedProgress);
            position += crosswindDirection * (Mathf.Sin(progress * Mathf.PI * 3.6f) * 0.34f + Mathf.Sin(progress * Mathf.PI * 9.2f) * 0.08f) * gustEnvelope;
            position.y += Mathf.Sin(progress * Mathf.PI * 2.2f) * 0.46f * gustEnvelope;

            Vector3 movementDirection = position - previousPosition;
            leafObject.transform.position = position;
            leafObject.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-34f, 44f, progress) + Mathf.Sin(progress * Mathf.PI * 10f) * 12f);
            leafObject.transform.localScale = Vector3.one * Mathf.Lerp(0.92f, 0.58f, easedProgress);
            windEffect.UpdateEffect(position, movementDirection, progress, 1f + Mathf.Sin(progress * Mathf.PI * 4f) * 0.18f);
            previousPosition = position;
            yield return null;
        }

        if (windEffect != null)
        {
            windEffect.Finish();
        }

        if (leafObject != null)
        {
            Object.Destroy(leafObject);
        }
    }

    private static int PickTargetTreeIndex()
    {
        int targetIndex = 0;
        float bestDistanceSqr = float.MaxValue;
        Vector3 preferredPosition = SketchOutsideTransition.GetPreferredGlowingLeafTreeLocalPosition();

        for (int i = 0; i < routeTrees.Count; i++)
        {
            Vector3 position = routeTrees[i].LocalPosition;
            float distanceSqr = (position - preferredPosition).sqrMagnitude;

            if (distanceSqr < bestDistanceSqr)
            {
                bestDistanceSqr = distanceSqr;
                targetIndex = i;
            }
        }

        return targetIndex;
    }

    private static RouteTree GetRouteTree(int index)
    {
        if (index >= 0 && index < routeTrees.Count)
        {
            return routeTrees[index];
        }

        return default;
    }

    private static Vector3 GetTreeLeafWorldPosition(int index)
    {
        RouteTree routeTree = GetRouteTree(index);
        Vector3 localPosition = routeTree.LocalPosition + new Vector3(0f, 2.55f * routeTree.TreeScale, 0f);

        if (routeTree.TreeTransform != null)
        {
            return routeTree.TreeTransform.TransformPoint(new Vector3(0f, 2.55f * routeTree.TreeScale, 0f));
        }

        return routeTree.Parent != null ? routeTree.Parent.TransformPoint(localPosition) : localPosition;
    }

    private static void EnsureTreeGlow(int index)
    {
        if (index < 0 || index >= routeTrees.Count)
        {
            return;
        }

        RouteTree routeTree = routeTrees[index];
        if (!IsTreeReadyForLeaf(routeTree))
        {
            RemoveTreeGlow(index);
            return;
        }

        if (routeTree.GlowObject != null)
        {
            return;
        }

        GameObject glowObject = new GameObject("GeneratedFourthForestGlowingLeafTreeGlow");
        glowObject.transform.SetParent(routeTree.TreeTransform, false);
        glowObject.transform.localPosition = new Vector3(0f, 2.55f * routeTree.TreeScale, 0f);
        glowObject.transform.localScale = Vector3.one;

        SpriteRenderer glowRenderer = glowObject.AddComponent<SpriteRenderer>();
        glowRenderer.sprite = GetGlowSprite();
        glowRenderer.color = new Color(1f, 0.94f, 0.28f, 0.42f);
        glowRenderer.sortingOrder = GlowingLeafSortingOrder - 1;
        glowRenderer.transform.localScale = Vector3.one * 2.2f * routeTree.TreeScale;

        GameObject leafObject = new GameObject("GeneratedFourthForestGlowingLeaf");
        leafObject.transform.SetParent(glowObject.transform, false);
        leafObject.transform.localScale = Vector3.one * Mathf.Max(0.7f, routeTree.TreeScale * 0.82f);
        SketchWorldLineDrawing leafDrawing = leafObject.AddComponent<SketchWorldLineDrawing>();
        leafDrawing.Configure(0.04f, new Color32(231, 207, 57, 245), GlowingLeafSortingOrder);
        leafDrawing.SetStrokes(BuildGlowingLeafStrokes());
        leafDrawing.RevealProgress = 1f;

        GlowingLeafTreeGlow glow = glowObject.AddComponent<GlowingLeafTreeGlow>();
        glow.Configure(glowRenderer);

        routeTree.GlowObject = glowObject;
        routeTrees[index] = routeTree;
    }

    private static void RemoveTreeGlow(int index)
    {
        if (index < 0 || index >= routeTrees.Count)
        {
            return;
        }

        RouteTree routeTree = routeTrees[index];
        if (routeTree.GlowObject != null)
        {
            Object.Destroy(routeTree.GlowObject);
            routeTree.GlowObject = null;
            routeTrees[index] = routeTree;
        }
    }

    private static bool IsTreeReadyForLeaf(RouteTree routeTree)
    {
        return routeTree.TreeTransform != null
            && routeTree.TreeDrawing != null
            && routeTree.TreeDrawing.RevealProgress >= 0.95f;
    }

    private static GameObject CreateGlowingLeafObject(string objectName, Transform parent, Vector3 worldPosition, float scale)
    {
        GameObject leafObject = new GameObject(objectName);
        if (parent != null)
        {
            leafObject.transform.SetParent(parent, true);
        }

        leafObject.transform.position = worldPosition;
        leafObject.transform.localScale = Vector3.one * scale;

        SpriteRenderer glowRenderer = leafObject.AddComponent<SpriteRenderer>();
        glowRenderer.sprite = GetGlowSprite();
        glowRenderer.color = new Color(1f, 0.95f, 0.26f, 0.38f);
        glowRenderer.sortingOrder = GlowingLeafSortingOrder - 1;
        glowRenderer.transform.localScale = Vector3.one * 1.25f;

        GameObject drawingObject = new GameObject("GeneratedGlowingLeafDrawing");
        drawingObject.transform.SetParent(leafObject.transform, false);

        SketchWorldLineDrawing drawing = drawingObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(0.042f, new Color32(229, 207, 55, 255), GlowingLeafSortingOrder);
        drawing.SetStrokes(BuildGlowingLeafStrokes());
        drawing.RevealProgress = 1f;

        return leafObject;
    }

    private static Sprite GetGlowSprite()
    {
        if (glowSprite != null)
        {
            return glowSprite;
        }

        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.48f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center) / radius;
                float alpha = Mathf.Clamp01(1f - distance);
                alpha *= alpha;
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        glowSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        glowSprite.name = "GeneratedGlowingLeafGlowSprite";
        return glowSprite;
    }

    private static List<Vector3[]> BuildGlowingLeafStrokes()
    {
        return new List<Vector3[]>
        {
            Points(0f, 0.42f, -0.34f, 0.18f, -0.38f, -0.14f, -0.12f, -0.36f, 0.22f, -0.2f, 0.38f, 0.12f, 0f, 0.42f),
            Points(-0.08f, -0.3f, 0.18f, 0.22f),
            Points(-0.18f, -0.02f, 0.02f, 0.04f),
            Points(0.02f, -0.15f, 0.22f, -0.04f)
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

    private struct RouteTree
    {
        public readonly Vector3 LocalPosition;
        public readonly Transform Parent;
        public readonly Transform TreeTransform;
        public readonly SketchWorldLineDrawing TreeDrawing;
        public readonly float TreeScale;
        public GameObject GlowObject;

        public RouteTree(Vector3 localPosition, Transform parent, SketchWorldLineDrawing treeDrawing, float treeScale)
        {
            LocalPosition = localPosition;
            Parent = parent;
            TreeTransform = treeDrawing != null ? treeDrawing.transform : null;
            TreeDrawing = treeDrawing;
            TreeScale = treeScale;
            GlowObject = null;
        }
    }
}

public class DeepForestLeafPileInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float promptDistance = 1.35f;

    private const int RewardSortingOrder = 78;

    private static Sprite pixelSprite;

    private CircleCollider2D interactionCollider;
    private SketchWorldLineDrawing lineDrawing;
    private Transform player;
    private GameObject promptObject;
    private int promptSortingOrder = 72;
    private bool hasConfigured;
    private bool hasReward;
    private bool hasMushroom;
    private bool hasSearched;
    private bool isSearching;

    public void Configure(bool containsReward, bool containsMushroom, float distance, int sortingOrder)
    {
        hasReward = containsReward;
        hasMushroom = containsMushroom;
        promptDistance = Mathf.Max(0.2f, distance);
        promptSortingOrder = sortingOrder;
        hasConfigured = true;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(CanSearch());
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || !hasConfigured || isSearching || !CanSearch() || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > promptDistance)
        {
            return;
        }

        StartCoroutine(SearchRoutine(interactor.transform));
    }

    private void Awake()
    {
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        EnsurePrompt();
        EnsureCollider();
    }

    private void OnEnable()
    {
        FindPlayer();
        SetInteractionAvailable(hasConfigured && CanSearch());

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!hasConfigured || !CanSearch())
        {
            SetInteractionAvailable(false);
            return;
        }

        if (player == null)
        {
            FindPlayer();
        }

        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        bool isNear = player != null && Vector2.Distance(transform.position, player.position) <= promptDistance;
        SetInteractionAvailable(isRevealed && !isSearching);

        if (promptObject != null)
        {
            promptObject.SetActive(isRevealed && isNear && !isSearching);
        }
    }

    private void OnDisable()
    {
        SetInteractionAvailable(false);
    }

    private IEnumerator SearchRoutine(Transform interactor)
    {
        isSearching = true;
        SetInteractionAvailable(false);

        Vector3 basePosition = transform.localPosition;
        Quaternion baseRotation = transform.localRotation;
        const float rustleDuration = 0.38f;
        float elapsed = 0f;

        while (elapsed < rustleDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / rustleDuration);
            float shake = Mathf.Sin(progress * Mathf.PI * 8f) * (1f - progress);
            transform.localPosition = basePosition + Vector3.right * shake * 0.06f;
            transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, shake * 8f);
            yield return null;
        }

        transform.localPosition = basePosition;
        transform.localRotation = baseRotation;

        if (hasReward && GameProgress.CanDropPencilFragmentFrom(PencilFragmentSource.LeafPile))
        {
            yield return StartCoroutine(DeliverPencilFragmentRoutine(interactor));
        }
        else if (hasMushroom && !GameProgress.HasCollectedMushroom)
        {
            yield return StartCoroutine(CollectMushroomRoutine(interactor));
        }

        hasSearched = true;
        isSearching = false;
        SetInteractionAvailable(false);
    }

    private IEnumerator DeliverPencilFragmentRoutine(Transform interactor)
    {
        Transform itemParent = transform.parent != null ? transform.parent : transform;
        GameObject rewardObject = new GameObject("GeneratedLeafPilePencilReward");
        rewardObject.transform.SetParent(itemParent, true);
        rewardObject.transform.position = transform.position + new Vector3(0f, 0.48f, 0f);
        CreatePencilFragmentVisual(rewardObject.transform);

        Vector3 startPosition = rewardObject.transform.position;
        Vector3 fallbackTargetPosition = startPosition + new Vector3(0f, 0.72f, 0f);
        Vector3 startScale = rewardObject.transform.localScale;
        Quaternion startRotation = rewardObject.transform.localRotation;
        const float collectDuration = 0.52f;
        float elapsed = 0f;

        while (elapsed < collectDuration && rewardObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 targetPosition = interactor != null
                ? interactor.position + new Vector3(0f, 0.72f, 0f)
                : fallbackTargetPosition;
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.24f;

            rewardObject.transform.position = position;
            rewardObject.transform.localScale = Vector3.Lerp(startScale, startScale * 0.18f, easedProgress);
            rewardObject.transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, Mathf.Lerp(-14f, 20f, progress));
            yield return null;
        }

        GameProgress.CollectPencilFragmentFrom(PencilFragmentSource.LeafPile);
        PencilFragmentHud.ShowCollected();

        if (rewardObject != null)
        {
            Destroy(rewardObject);
        }
    }

    private IEnumerator CollectMushroomRoutine(Transform interactor)
    {
        Transform itemParent = transform.parent != null ? transform.parent : transform;
        GameObject rewardObject = new GameObject("GeneratedLeafPileMushroomReward");
        rewardObject.transform.SetParent(itemParent, true);
        rewardObject.transform.position = transform.position + new Vector3(0f, 0.42f, 0f);

        SketchWorldLineDrawing rewardDrawing = rewardObject.AddComponent<SketchWorldLineDrawing>();
        rewardDrawing.Configure(0.052f, new Color32(118, 54, 48, 255), RewardSortingOrder);
        rewardDrawing.SetStrokes(BuildMushroomRewardStrokes());
        rewardDrawing.RevealProgress = 1f;

        Vector3 startPosition = rewardObject.transform.position;
        Vector3 fallbackTargetPosition = startPosition + new Vector3(0f, 0.72f, 0f);
        Vector3 startScale = rewardObject.transform.localScale;
        Quaternion startRotation = rewardObject.transform.localRotation;
        const float collectDuration = 0.54f;
        float elapsed = 0f;

        while (elapsed < collectDuration && rewardObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 targetPosition = interactor != null
                ? interactor.position + new Vector3(0f, 0.72f, 0f)
                : fallbackTargetPosition;
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.24f;

            rewardObject.transform.position = position;
            rewardObject.transform.localScale = Vector3.Lerp(startScale, startScale * 0.2f, easedProgress);
            rewardObject.transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, Mathf.Lerp(12f, -18f, progress));
            yield return null;
        }

        GameProgress.CollectMushroom();
        LetterQuestHud.ShowProgress();

        if (rewardObject != null)
        {
            Destroy(rewardObject);
        }
    }

    private bool CanSearch()
    {
        if (hasSearched)
        {
            return false;
        }

        if (hasReward)
        {
            return !GameProgress.HasCollectedPencilFragmentFrom(PencilFragmentSource.LeafPile);
        }

        if (hasMushroom)
        {
            return !GameProgress.HasCollectedMushroom;
        }

        return true;
    }

    private void EnsurePrompt()
    {
        if (promptObject == null)
        {
            promptObject = new GameObject("GeneratedDeepForestLeafPilePrompt");
            promptObject.transform.SetParent(transform, false);
            promptObject.transform.localPosition = new Vector3(0f, 0.58f, 0f);

            TextMesh textMesh = promptObject.AddComponent<TextMesh>();
            textMesh.text = "E";
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.fontSize = 64;
            textMesh.characterSize = 0.075f;
            textMesh.color = new Color32(35, 32, 28, 255);
            promptObject.SetActive(false);
        }

        MeshRenderer meshRenderer = promptObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = promptSortingOrder;
        }
    }

    private void EnsureCollider()
    {
        if (interactionCollider != null)
        {
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = promptDistance;
        interactionCollider.offset = new Vector2(0f, 0.12f);
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = available;
        }

        if (!available && promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void CreatePencilFragmentVisual(Transform pencilTransform)
    {
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentBody", new Vector3(-0.06f, 0f, 0f), new Vector3(0.48f, 0.12f, 1f), new Color32(236, 181, 51, 255), RewardSortingOrder);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentWood", new Vector3(0.24f, 0f, 0f), new Vector3(0.16f, 0.12f, 1f), new Color32(203, 143, 77, 255), RewardSortingOrder + 1);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentLead", new Vector3(0.34f, 0f, 0f), new Vector3(0.08f, 0.08f, 1f), new Color32(44, 40, 36, 255), RewardSortingOrder + 2);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentPaintEdge", new Vector3(-0.33f, 0f, 0f), new Vector3(0.08f, 0.13f, 1f), new Color32(190, 66, 72, 255), RewardSortingOrder + 2);

        SketchWorldLineDrawing outline = pencilTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        outline.Configure(0.035f, new Color32(38, 34, 28, 255), RewardSortingOrder + 3);
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
        pixelSprite.name = "GeneratedLeafPilePencilRewardPixel";
        return pixelSprite;
    }

    private static List<Vector3[]> BuildMushroomRewardStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(-0.48f, 0.1f, -0.34f, 0.46f, 0f, 0.62f, 0.34f, 0.46f, 0.5f, 0.1f, -0.48f, 0.1f));
        strokes.Add(Points(-0.42f, 0.1f, -0.18f, -0.04f, 0.16f, -0.04f, 0.42f, 0.1f));
        strokes.Add(Points(-0.16f, -0.02f, -0.22f, -0.52f, 0.22f, -0.52f, 0.16f, -0.02f));
        strokes.Add(Points(-0.28f, -0.52f, 0.28f, -0.52f));
        strokes.Add(EllipsePoints(-0.18f, 0.32f, 0.08f, 0.06f, 10));
        strokes.Add(EllipsePoints(0.16f, 0.38f, 0.08f, 0.06f, 10));
        strokes.Add(EllipsePoints(0.02f, 0.18f, 0.06f, 0.05f, 10));

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

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }
}

public class ShakingBushAcornQuestInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float promptDistance = 1.35f;

    private const int RewardSortingOrder = 78;

    private CircleCollider2D interactionCollider;
    private SketchWorldLineDrawing lineDrawing;
    private Transform player;
    private GameObject promptObject;
    private Vector3 baseLocalPosition;
    private Quaternion baseLocalRotation = Quaternion.identity;
    private int promptSortingOrder = 72;
    private bool hasConfigured;
    private bool isCollecting;

    public void Configure(float distance, int sortingOrder)
    {
        promptDistance = Mathf.Max(0.2f, distance);
        promptSortingOrder = sortingOrder;
        hasConfigured = true;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(CanCollect());
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || !hasConfigured || isCollecting || !CanCollect() || interactor == null)
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
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        baseLocalPosition = transform.localPosition;
        baseLocalRotation = transform.localRotation;
        EnsurePrompt();
        EnsureCollider();
    }

    private void OnEnable()
    {
        FindPlayer();
        SetInteractionAvailable(hasConfigured && CanCollect());

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!hasConfigured || !CanCollect())
        {
            SetInteractionAvailable(false);
            return;
        }

        if (player == null)
        {
            FindPlayer();
        }

        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        bool isNear = player != null && Vector2.Distance(transform.position, player.position) <= promptDistance;
        SetInteractionAvailable(isRevealed);

        if (isRevealed && !isCollecting)
        {
            float sway = Mathf.Sin(Time.time * 3.8f) * 1.35f;
            transform.localRotation = baseLocalRotation * Quaternion.Euler(0f, 0f, sway);
        }

        if (promptObject != null)
        {
            promptObject.SetActive(isRevealed && isNear && !isCollecting);
        }
    }

    private void OnDisable()
    {
        SetInteractionAvailable(false);
    }

    private IEnumerator CollectRoutine()
    {
        isCollecting = true;
        SetInteractionAvailable(false);

        const float shakeDuration = 0.46f;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / shakeDuration);
            float shake = Mathf.Sin(progress * Mathf.PI * 9f) * (1f - progress);
            transform.localPosition = baseLocalPosition + Vector3.right * shake * 0.08f;
            transform.localRotation = baseLocalRotation * Quaternion.Euler(0f, 0f, shake * 7.5f);
            yield return null;
        }

        transform.localPosition = baseLocalPosition;
        transform.localRotation = baseLocalRotation;

        yield return StartCoroutine(SpawnAcornRewardRoutine());
        GameProgress.CollectAcorn();
        LetterQuestHud.ShowProgress();
        isCollecting = false;
        SetInteractionAvailable(false);
    }

    private IEnumerator SpawnAcornRewardRoutine()
    {
        Transform itemParent = transform.parent != null ? transform.parent : transform;
        GameObject rewardObject = new GameObject("GeneratedShakingBushAcornReward");
        rewardObject.transform.SetParent(itemParent, true);
        rewardObject.transform.position = transform.position + new Vector3(0f, 0.72f, 0f);

        SketchWorldLineDrawing rewardDrawing = rewardObject.AddComponent<SketchWorldLineDrawing>();
        rewardDrawing.Configure(0.052f, new Color32(92, 58, 30, 255), RewardSortingOrder);
        rewardDrawing.SetStrokes(BuildAcornRewardStrokes());
        rewardDrawing.RevealProgress = 1f;

        Vector3 startPosition = rewardObject.transform.position;
        Vector3 fallbackTargetPosition = startPosition + new Vector3(0f, 0.8f, 0f);
        Vector3 startScale = rewardObject.transform.localScale;
        const float duration = 0.56f;
        float elapsed = 0f;

        while (elapsed < duration && rewardObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 targetPosition = player != null
                ? player.position + new Vector3(0f, 0.7f, 0f)
                : fallbackTargetPosition;
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.26f;

            rewardObject.transform.position = position;
            rewardObject.transform.localScale = Vector3.Lerp(startScale, startScale * 0.2f, easedProgress);
            rewardObject.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-18f, 24f, progress));
            yield return null;
        }

        if (rewardObject != null)
        {
            Destroy(rewardObject);
        }
    }

    private bool CanCollect()
    {
        return !GameProgress.HasCollectedAcorn;
    }

    private void EnsurePrompt()
    {
        if (promptObject == null)
        {
            promptObject = new GameObject("GeneratedShakingBushPrompt");
            promptObject.transform.SetParent(transform, false);
            promptObject.transform.localPosition = new Vector3(0f, 1.12f, 0f);

            TextMesh textMesh = promptObject.AddComponent<TextMesh>();
            textMesh.text = "E";
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.fontSize = 64;
            textMesh.characterSize = 0.075f;
            textMesh.color = new Color32(35, 32, 28, 255);
            promptObject.SetActive(false);
        }

        MeshRenderer meshRenderer = promptObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = promptSortingOrder;
        }
    }

    private void EnsureCollider()
    {
        if (interactionCollider != null)
        {
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = promptDistance;
        interactionCollider.offset = new Vector2(0f, 0.16f);
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = available;
        }

        if (!available && promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }

    private static List<Vector3[]> BuildAcornRewardStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(EllipsePoints(0f, -0.06f, 0.38f, 0.5f, 22));
        strokes.Add(Points(-0.42f, 0.18f, -0.22f, 0.48f, 0.1f, 0.56f, 0.42f, 0.28f));
        strokes.Add(Points(-0.4f, 0.16f, -0.14f, 0.04f, 0.14f, 0.12f, 0.42f, 0.02f));
        strokes.Add(Points(-0.16f, 0.48f, -0.04f, 0.72f, 0.12f, 0.5f));
        strokes.Add(Points(-0.22f, -0.2f, -0.06f, -0.34f, 0.14f, -0.24f));

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

public class ForestSquirrelAcornQuestInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float interactDistance = 1.85f;

    private const string SearchingPrompt = "[\uB3C4\uD1A0\uB9AC]+[?]";
    private const string CompletePrompt = "[\uB3C4\uD1A0\uB9AC]+[!]";
    private const float PromptPulseSpeed = 4.4f;
    private const int RewardSortingOrder = 78;

    private static Sprite pixelSprite;

    private CircleCollider2D interactionCollider;
    private SketchWorldLineDrawing lineDrawing;
    private Transform player;
    private GameObject promptObject;
    private TextMesh promptText;
    private TextMesh promptInnerGlowText;
    private TextMesh promptOuterGlowText;
    private int promptSortingOrder = 72;
    private bool hasConfigured;
    private bool isReacting;
    private bool isDeliveringReward;

    public void Configure(float distance, int sortingOrder)
    {
        interactDistance = Mathf.Max(0.2f, distance);
        promptSortingOrder = sortingOrder;
        hasConfigured = true;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(CanInteractWithSquirrel());
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || !hasConfigured || isDeliveringReward || !CanInteractWithSquirrel() || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > interactDistance)
        {
            return;
        }

        if (CanDeliverReward())
        {
            StartCoroutine(DeliverPencilFragmentRoutine(interactor.transform));
            return;
        }

        if (!isReacting)
        {
            StartCoroutine(WeakReactionRoutine());
        }
    }

    private void Awake()
    {
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(false);
    }

    private void OnEnable()
    {
        FindPlayer();
        SetInteractionAvailable(CanInteractWithSquirrel());

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!hasConfigured || !CanInteractWithSquirrel())
        {
            SetInteractionAvailable(false);
            return;
        }

        if (player == null)
        {
            FindPlayer();
        }

        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        bool isNear = player != null && Vector2.Distance(transform.position, player.position) <= interactDistance;
        SetInteractionAvailable(isRevealed && !isDeliveringReward);

        if (promptObject != null)
        {
            SetPromptText(GameProgress.HasCollectedAcorn ? CompletePrompt : SearchingPrompt);
            promptObject.SetActive(isRevealed && isNear && !isDeliveringReward);
        }

        UpdatePromptGlow();
    }

    private void OnDisable()
    {
        SetInteractionAvailable(false);
    }

    private IEnumerator WeakReactionRoutine()
    {
        isReacting = true;
        Vector3 basePosition = transform.localPosition;
        Quaternion baseRotation = transform.localRotation;
        const float duration = 0.34f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float shake = Mathf.Sin(progress * Mathf.PI * 4f) * (1f - progress);
            transform.localPosition = basePosition + Vector3.right * shake * 0.06f;
            transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, shake * 5f);
            yield return null;
        }

        transform.localPosition = basePosition;
        transform.localRotation = baseRotation;
        isReacting = false;
    }

    private IEnumerator DeliverPencilFragmentRoutine(Transform interactor)
    {
        isDeliveringReward = true;
        SetInteractionAvailable(false);

        Transform itemParent = transform.parent != null ? transform.parent : transform;
        GameObject rewardObject = new GameObject("GeneratedSquirrelPencilReward");
        rewardObject.transform.SetParent(itemParent, true);
        rewardObject.transform.position = transform.position + new Vector3(0f, 0.78f, 0f);
        CreatePencilFragmentVisual(rewardObject.transform);

        Vector3 startPosition = rewardObject.transform.position;
        Vector3 fallbackTargetPosition = startPosition + new Vector3(0f, 0.78f, 0f);
        Vector3 startScale = rewardObject.transform.localScale;
        Quaternion startRotation = rewardObject.transform.localRotation;
        const float collectDuration = 0.52f;
        float elapsed = 0f;

        while (elapsed < collectDuration && rewardObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 targetPosition = interactor != null
                ? interactor.position + new Vector3(0f, 0.72f, 0f)
                : fallbackTargetPosition;
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.24f;

            rewardObject.transform.position = position;
            rewardObject.transform.localScale = Vector3.Lerp(startScale, startScale * 0.18f, easedProgress);
            rewardObject.transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, Mathf.Lerp(-14f, 20f, progress));
            yield return null;
        }

        GameProgress.CollectPencilFragmentFrom(PencilFragmentSource.Squirrel);
        PencilFragmentHud.ShowCollected();

        if (rewardObject != null)
        {
            Destroy(rewardObject);
        }

        isDeliveringReward = false;
        SetInteractionAvailable(false);
    }

    private void EnsurePrompt()
    {
        if (promptObject != null)
        {
            ApplyPromptSortingOrders();
            return;
        }

        promptObject = new GameObject("GeneratedForestSquirrelPrompt");
        promptObject.transform.SetParent(transform, false);
        promptObject.transform.localPosition = new Vector3(0f, 1.46f, 0f);

        promptOuterGlowText = CreatePromptText(
            "GeneratedForestSquirrelPromptOuterGlow",
            0.104f,
            new Color(1f, 0.74f, 0.12f, 0.26f),
            promptSortingOrder - 2);
        promptInnerGlowText = CreatePromptText(
            "GeneratedForestSquirrelPromptInnerGlow",
            0.086f,
            new Color(1f, 0.84f, 0.24f, 0.62f),
            promptSortingOrder - 1);
        promptText = CreatePromptText(
            "GeneratedForestSquirrelPromptText",
            0.07f,
            new Color32(35, 32, 28, 255),
            promptSortingOrder);

        promptObject.SetActive(false);
        ApplyPromptSortingOrders();
    }

    private void ApplyPromptSortingOrders()
    {
        SetPromptSortingOrder(promptOuterGlowText, promptSortingOrder - 2);
        SetPromptSortingOrder(promptInnerGlowText, promptSortingOrder - 1);
        SetPromptSortingOrder(promptText, promptSortingOrder);
    }

    private static void SetPromptSortingOrder(TextMesh textMesh, int sortingOrder)
    {
        if (textMesh == null)
        {
            return;
        }

        MeshRenderer meshRenderer = textMesh.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }
    }

    private TextMesh CreatePromptText(string objectName, float characterSize, Color color, int sortingOrder)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(promptObject.transform, false);
        textObject.transform.localPosition = Vector3.zero;

        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = SearchingPrompt;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 64;
        textMesh.characterSize = characterSize;
        textMesh.color = color;

        MeshRenderer meshRenderer = textObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }

        return textMesh;
    }

    private void SetPromptText(string value)
    {
        if (promptText != null)
        {
            promptText.text = value;
        }

        if (promptInnerGlowText != null)
        {
            promptInnerGlowText.text = value;
        }

        if (promptOuterGlowText != null)
        {
            promptOuterGlowText.text = value;
        }
    }

    private void UpdatePromptGlow()
    {
        if (promptObject == null || !promptObject.activeSelf)
        {
            return;
        }

        float pulse = Mathf.SmoothStep(0f, 1f, (Mathf.Sin(Time.time * PromptPulseSpeed) + 1f) * 0.5f);

        if (promptText != null)
        {
            promptText.color = Color.Lerp(
                new Color32(35, 32, 28, 255),
                new Color32(82, 62, 8, 255),
                pulse);
        }

        if (promptInnerGlowText != null)
        {
            promptInnerGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.02f, 1.12f, pulse);
            promptInnerGlowText.color = new Color(1f, 0.84f, 0.24f, Mathf.Lerp(0.45f, 0.82f, pulse));
        }

        if (promptOuterGlowText != null)
        {
            promptOuterGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.08f, 1.24f, pulse);
            promptOuterGlowText.color = new Color(1f, 0.74f, 0.12f, Mathf.Lerp(0.16f, 0.42f, pulse));
        }
    }

    private void EnsureCollider()
    {
        if (interactionCollider != null)
        {
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = interactDistance;
        interactionCollider.offset = new Vector2(0f, 0.2f);
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = available;
        }

        if (!available && promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private bool CanInteractWithSquirrel()
    {
        return hasConfigured && !GameProgress.HasCollectedPencilFragmentFrom(PencilFragmentSource.Squirrel);
    }

    private bool CanDeliverReward()
    {
        return GameProgress.HasCollectedAcorn
            && !GameProgress.HasCollectedPencilFragmentFrom(PencilFragmentSource.Squirrel);
    }

    private void CreatePencilFragmentVisual(Transform pencilTransform)
    {
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentBody", new Vector3(-0.06f, 0f, 0f), new Vector3(0.48f, 0.12f, 1f), new Color32(236, 181, 51, 255), RewardSortingOrder);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentWood", new Vector3(0.24f, 0f, 0f), new Vector3(0.16f, 0.12f, 1f), new Color32(203, 143, 77, 255), RewardSortingOrder + 1);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentLead", new Vector3(0.34f, 0f, 0f), new Vector3(0.08f, 0.08f, 1f), new Color32(44, 40, 36, 255), RewardSortingOrder + 2);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentPaintEdge", new Vector3(-0.33f, 0f, 0f), new Vector3(0.08f, 0.13f, 1f), new Color32(190, 66, 72, 255), RewardSortingOrder + 2);

        SketchWorldLineDrawing outline = pencilTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        outline.Configure(0.035f, new Color32(38, 34, 28, 255), RewardSortingOrder + 3);
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
        pixelSprite.name = "GeneratedSquirrelPencilRewardPixel";
        return pixelSprite;
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

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }
}

public class ForestHungrySquirrelMushroomQuestInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float interactDistance = 1.85f;

    private const string SearchingPrompt = "[\uBC84\uC12F]+[?]";
    private const string CompletePrompt = "[\uBC84\uC12F]+[!]";
    private const float PromptPulseSpeed = 4.4f;
    private const int RewardSortingOrder = 78;

    private static Sprite pixelSprite;

    private CircleCollider2D interactionCollider;
    private SketchWorldLineDrawing lineDrawing;
    private Transform player;
    private GameObject promptObject;
    private TextMesh promptText;
    private TextMesh promptInnerGlowText;
    private TextMesh promptOuterGlowText;
    private int promptSortingOrder = 72;
    private bool hasConfigured;
    private bool isReacting;
    private bool isDeliveringReward;

    public void Configure(float distance, int sortingOrder)
    {
        interactDistance = Mathf.Max(0.2f, distance);
        promptSortingOrder = sortingOrder;
        hasConfigured = true;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(CanInteractWithSquirrel());
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || !hasConfigured || isDeliveringReward || !CanInteractWithSquirrel() || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > interactDistance)
        {
            return;
        }

        if (CanDeliverReward())
        {
            StartCoroutine(DeliverPencilFragmentRoutine(interactor.transform));
            return;
        }

        if (!isReacting)
        {
            StartCoroutine(HungryReactionRoutine());
        }
    }

    private void Awake()
    {
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(false);
    }

    private void OnEnable()
    {
        FindPlayer();
        SetInteractionAvailable(CanInteractWithSquirrel());

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!hasConfigured || !CanInteractWithSquirrel())
        {
            SetInteractionAvailable(false);
            return;
        }

        if (player == null)
        {
            FindPlayer();
        }

        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        bool isNear = player != null && Vector2.Distance(transform.position, player.position) <= interactDistance;
        SetInteractionAvailable(isRevealed && !isDeliveringReward);

        if (promptObject != null)
        {
            SetPromptText(GameProgress.HasCollectedMushroom ? CompletePrompt : SearchingPrompt);
            promptObject.SetActive(isRevealed && isNear && !isDeliveringReward);
        }

        UpdatePromptGlow();
    }

    private void OnDisable()
    {
        SetInteractionAvailable(false);
    }

    private IEnumerator HungryReactionRoutine()
    {
        isReacting = true;
        Vector3 basePosition = transform.localPosition;
        Quaternion baseRotation = transform.localRotation;
        const float duration = 0.42f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float slump = Mathf.Sin(progress * Mathf.PI) * (1f - progress);
            transform.localPosition = basePosition + Vector3.down * slump * 0.08f;
            transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, -slump * 5f);
            yield return null;
        }

        transform.localPosition = basePosition;
        transform.localRotation = baseRotation;
        isReacting = false;
    }

    private IEnumerator DeliverPencilFragmentRoutine(Transform interactor)
    {
        isDeliveringReward = true;
        SetInteractionAvailable(false);

        Transform itemParent = transform.parent != null ? transform.parent : transform;
        GameObject rewardObject = new GameObject("GeneratedHungrySquirrelPencilReward");
        rewardObject.transform.SetParent(itemParent, true);
        rewardObject.transform.position = transform.position + new Vector3(0f, 0.78f, 0f);
        CreatePencilFragmentVisual(rewardObject.transform);

        Vector3 startPosition = rewardObject.transform.position;
        Vector3 fallbackTargetPosition = startPosition + new Vector3(0f, 0.78f, 0f);
        Vector3 startScale = rewardObject.transform.localScale;
        Quaternion startRotation = rewardObject.transform.localRotation;
        const float collectDuration = 0.52f;
        float elapsed = 0f;

        while (elapsed < collectDuration && rewardObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 targetPosition = interactor != null
                ? interactor.position + new Vector3(0f, 0.72f, 0f)
                : fallbackTargetPosition;
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.24f;

            rewardObject.transform.position = position;
            rewardObject.transform.localScale = Vector3.Lerp(startScale, startScale * 0.18f, easedProgress);
            rewardObject.transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, Mathf.Lerp(14f, -20f, progress));
            yield return null;
        }

        GameProgress.CollectPencilFragmentFrom(PencilFragmentSource.HungrySquirrel);
        PencilFragmentHud.ShowCollected();

        if (rewardObject != null)
        {
            Destroy(rewardObject);
        }

        isDeliveringReward = false;
        SetInteractionAvailable(false);
    }

    private void EnsurePrompt()
    {
        if (promptObject != null)
        {
            ApplyPromptSortingOrders();
            return;
        }

        promptObject = new GameObject("GeneratedForestHungrySquirrelPrompt");
        promptObject.transform.SetParent(transform, false);
        promptObject.transform.localPosition = new Vector3(0f, 1.38f, 0f);

        promptOuterGlowText = CreatePromptText(
            "GeneratedForestHungrySquirrelPromptOuterGlow",
            0.104f,
            new Color(1f, 0.58f, 0.22f, 0.26f),
            promptSortingOrder - 2);
        promptInnerGlowText = CreatePromptText(
            "GeneratedForestHungrySquirrelPromptInnerGlow",
            0.086f,
            new Color(1f, 0.72f, 0.3f, 0.62f),
            promptSortingOrder - 1);
        promptText = CreatePromptText(
            "GeneratedForestHungrySquirrelPromptText",
            0.07f,
            new Color32(35, 32, 28, 255),
            promptSortingOrder);

        promptObject.SetActive(false);
        ApplyPromptSortingOrders();
    }

    private void ApplyPromptSortingOrders()
    {
        SetPromptSortingOrder(promptOuterGlowText, promptSortingOrder - 2);
        SetPromptSortingOrder(promptInnerGlowText, promptSortingOrder - 1);
        SetPromptSortingOrder(promptText, promptSortingOrder);
    }

    private static void SetPromptSortingOrder(TextMesh textMesh, int sortingOrder)
    {
        if (textMesh == null)
        {
            return;
        }

        MeshRenderer meshRenderer = textMesh.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }
    }

    private TextMesh CreatePromptText(string objectName, float characterSize, Color color, int sortingOrder)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(promptObject.transform, false);
        textObject.transform.localPosition = Vector3.zero;

        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = SearchingPrompt;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 64;
        textMesh.characterSize = characterSize;
        textMesh.color = color;

        MeshRenderer meshRenderer = textObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }

        return textMesh;
    }

    private void SetPromptText(string value)
    {
        if (promptText != null)
        {
            promptText.text = value;
        }

        if (promptInnerGlowText != null)
        {
            promptInnerGlowText.text = value;
        }

        if (promptOuterGlowText != null)
        {
            promptOuterGlowText.text = value;
        }
    }

    private void UpdatePromptGlow()
    {
        if (promptObject == null || !promptObject.activeSelf)
        {
            return;
        }

        float pulse = Mathf.SmoothStep(0f, 1f, (Mathf.Sin(Time.time * PromptPulseSpeed) + 1f) * 0.5f);

        if (promptText != null)
        {
            promptText.color = Color.Lerp(
                new Color32(35, 32, 28, 255),
                new Color32(92, 47, 24, 255),
                pulse);
        }

        if (promptInnerGlowText != null)
        {
            promptInnerGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.02f, 1.12f, pulse);
            promptInnerGlowText.color = new Color(1f, 0.72f, 0.3f, Mathf.Lerp(0.45f, 0.82f, pulse));
        }

        if (promptOuterGlowText != null)
        {
            promptOuterGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.08f, 1.24f, pulse);
            promptOuterGlowText.color = new Color(1f, 0.58f, 0.22f, Mathf.Lerp(0.16f, 0.42f, pulse));
        }
    }

    private void EnsureCollider()
    {
        if (interactionCollider != null)
        {
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = interactDistance;
        interactionCollider.offset = new Vector2(0f, 0.15f);
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = available;
        }

        if (!available && promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private bool CanInteractWithSquirrel()
    {
        return hasConfigured && !GameProgress.HasCollectedPencilFragmentFrom(PencilFragmentSource.HungrySquirrel);
    }

    private bool CanDeliverReward()
    {
        return GameProgress.HasCollectedMushroom
            && !GameProgress.HasCollectedPencilFragmentFrom(PencilFragmentSource.HungrySquirrel);
    }

    private void CreatePencilFragmentVisual(Transform pencilTransform)
    {
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentBody", new Vector3(-0.06f, 0f, 0f), new Vector3(0.48f, 0.12f, 1f), new Color32(236, 181, 51, 255), RewardSortingOrder);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentWood", new Vector3(0.24f, 0f, 0f), new Vector3(0.16f, 0.12f, 1f), new Color32(203, 143, 77, 255), RewardSortingOrder + 1);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentLead", new Vector3(0.34f, 0f, 0f), new Vector3(0.08f, 0.08f, 1f), new Color32(44, 40, 36, 255), RewardSortingOrder + 2);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentPaintEdge", new Vector3(-0.33f, 0f, 0f), new Vector3(0.08f, 0.13f, 1f), new Color32(190, 66, 72, 255), RewardSortingOrder + 2);

        SketchWorldLineDrawing outline = pencilTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        outline.Configure(0.035f, new Color32(38, 34, 28, 255), RewardSortingOrder + 3);
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
        pixelSprite.name = "GeneratedHungrySquirrelPencilRewardPixel";
        return pixelSprite;
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

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }
}

public class ForestBirdQuestInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float interactDistance = 2.15f;

    private const string SearchingPrompt = "[\uB098\uBB47\uAC00\uC9C0]+[?]";
    private const string CompletePrompt = "[\uB098\uBB47\uAC00\uC9C0]+[!]";
    private const float PromptPulseSpeed = 4.4f;
    private const int RewardSortingOrder = 78;

    private static Sprite pixelSprite;

    private CircleCollider2D interactionCollider;
    private SketchWorldLineDrawing lineDrawing;
    private Transform player;
    private GameObject promptObject;
    private TextMesh promptText;
    private TextMesh promptInnerGlowText;
    private TextMesh promptOuterGlowText;
    private int promptSortingOrder = 72;
    private bool hasConfigured;
    private bool isReacting;
    private bool isDeliveringReward;

    public void Configure(float distance, int sortingOrder)
    {
        interactDistance = Mathf.Max(0.2f, distance);
        promptSortingOrder = sortingOrder;
        hasConfigured = true;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(CanInteractWithBird());
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || !hasConfigured || isDeliveringReward || !CanInteractWithBird() || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > interactDistance)
        {
            return;
        }

        GameProgress.MeetForestBird();

        if (CanDeliverReward())
        {
            StartCoroutine(DeliverPencilFragmentRoutine(interactor.transform));
            return;
        }

        if (!isReacting)
        {
            StartCoroutine(WeakReactionRoutine());
        }
    }

    private void Awake()
    {
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(false);
    }

    private void OnEnable()
    {
        FindPlayer();
        SetInteractionAvailable(CanInteractWithBird());

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!hasConfigured || !CanInteractWithBird())
        {
            SetInteractionAvailable(false);
            return;
        }

        if (player == null)
        {
            FindPlayer();
        }

        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        bool isNear = player != null && Vector2.Distance(transform.position, player.position) <= interactDistance;

        SetInteractionAvailable(isRevealed && !isDeliveringReward);

        if (promptObject != null)
        {
            SetPromptText(GameProgress.HasCollectedForestBranch ? CompletePrompt : SearchingPrompt);
            promptObject.SetActive(isRevealed && isNear && !isDeliveringReward);
        }

        UpdatePromptGlow();
    }

    private void OnDisable()
    {
        SetInteractionAvailable(false);
    }

    private IEnumerator WeakReactionRoutine()
    {
        isReacting = true;
        Vector3 basePosition = transform.localPosition;
        Quaternion baseRotation = transform.localRotation;
        const float duration = 0.34f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float shake = Mathf.Sin(progress * Mathf.PI * 4f) * (1f - progress);
            transform.localPosition = basePosition + Vector3.right * shake * 0.06f;
            transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, shake * 5f);
            yield return null;
        }

        transform.localPosition = basePosition;
        transform.localRotation = baseRotation;
        isReacting = false;
    }

    private IEnumerator DeliverPencilFragmentRoutine(Transform interactor)
    {
        isDeliveringReward = true;
        SetInteractionAvailable(false);

        Transform itemParent = transform.parent != null ? transform.parent : transform;
        GameObject rewardObject = new GameObject("GeneratedForestBirdPencilReward");
        rewardObject.transform.SetParent(itemParent, true);
        rewardObject.transform.position = transform.position + new Vector3(0f, 0.78f, 0f);
        CreatePencilFragmentVisual(rewardObject.transform);

        Vector3 startPosition = rewardObject.transform.position;
        Vector3 fallbackTargetPosition = startPosition + new Vector3(0f, 0.78f, 0f);
        Vector3 startScale = rewardObject.transform.localScale;
        Quaternion startRotation = rewardObject.transform.localRotation;
        const float collectDuration = 0.52f;
        float elapsed = 0f;

        while (elapsed < collectDuration && rewardObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 targetPosition = interactor != null
                ? interactor.position + new Vector3(0f, 0.72f, 0f)
                : fallbackTargetPosition;
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.24f;

            rewardObject.transform.position = position;
            rewardObject.transform.localScale = Vector3.Lerp(startScale, startScale * 0.18f, easedProgress);
            rewardObject.transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, Mathf.Lerp(-14f, 20f, progress));
            yield return null;
        }

        GameProgress.CollectPencilFragmentFrom(PencilFragmentSource.Bird);
        PencilFragmentHud.ShowCollected();

        if (rewardObject != null)
        {
            Destroy(rewardObject);
        }

        isDeliveringReward = false;
        SetInteractionAvailable(false);
    }

    private void EnsurePrompt()
    {
        if (promptObject != null)
        {
            return;
        }

        promptObject = new GameObject("GeneratedForestBirdPrompt");
        promptObject.transform.SetParent(transform, false);
        promptObject.transform.localPosition = new Vector3(0f, 1.38f, 0f);

        promptOuterGlowText = CreatePromptText(
            "GeneratedForestBirdPromptOuterGlow",
            0.104f,
            new Color(1f, 0.74f, 0.12f, 0.26f),
            promptSortingOrder - 2);
        promptInnerGlowText = CreatePromptText(
            "GeneratedForestBirdPromptInnerGlow",
            0.086f,
            new Color(1f, 0.84f, 0.24f, 0.62f),
            promptSortingOrder - 1);
        promptText = CreatePromptText(
            "GeneratedForestBirdPromptText",
            0.07f,
            new Color32(35, 32, 28, 255),
            promptSortingOrder);

        promptObject.SetActive(false);
    }

    private TextMesh CreatePromptText(string objectName, float characterSize, Color color, int sortingOrder)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(promptObject.transform, false);
        textObject.transform.localPosition = Vector3.zero;

        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = SearchingPrompt;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 64;
        textMesh.characterSize = characterSize;
        textMesh.color = color;

        MeshRenderer meshRenderer = textObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }

        return textMesh;
    }

    private void SetPromptText(string value)
    {
        if (promptText != null)
        {
            promptText.text = value;
        }

        if (promptInnerGlowText != null)
        {
            promptInnerGlowText.text = value;
        }

        if (promptOuterGlowText != null)
        {
            promptOuterGlowText.text = value;
        }
    }

    private void UpdatePromptGlow()
    {
        if (promptObject == null || !promptObject.activeSelf)
        {
            return;
        }

        float pulse = Mathf.SmoothStep(0f, 1f, (Mathf.Sin(Time.time * PromptPulseSpeed) + 1f) * 0.5f);

        if (promptText != null)
        {
            promptText.color = Color.Lerp(
                new Color32(35, 32, 28, 255),
                new Color32(82, 62, 8, 255),
                pulse);
        }

        if (promptInnerGlowText != null)
        {
            promptInnerGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.02f, 1.12f, pulse);
            promptInnerGlowText.color = new Color(1f, 0.84f, 0.24f, Mathf.Lerp(0.45f, 0.82f, pulse));
        }

        if (promptOuterGlowText != null)
        {
            promptOuterGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.08f, 1.24f, pulse);
            promptOuterGlowText.color = new Color(1f, 0.74f, 0.12f, Mathf.Lerp(0.16f, 0.42f, pulse));
        }
    }

    private void EnsureCollider()
    {
        if (interactionCollider != null)
        {
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = interactDistance;
        interactionCollider.offset = new Vector2(0f, 0.18f);
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = available;
        }

        if (!available && promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private bool CanInteractWithBird()
    {
        return hasConfigured && !GameProgress.HasCollectedPencilFragmentFrom(PencilFragmentSource.Bird);
    }

    private bool CanDeliverReward()
    {
        return GameProgress.HasCollectedForestBranch
            && !GameProgress.HasCollectedPencilFragmentFrom(PencilFragmentSource.Bird);
    }

    private void CreatePencilFragmentVisual(Transform pencilTransform)
    {
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentBody", new Vector3(-0.06f, 0f, 0f), new Vector3(0.48f, 0.12f, 1f), new Color32(236, 181, 51, 255), RewardSortingOrder);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentWood", new Vector3(0.24f, 0f, 0f), new Vector3(0.16f, 0.12f, 1f), new Color32(203, 143, 77, 255), RewardSortingOrder + 1);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentLead", new Vector3(0.34f, 0f, 0f), new Vector3(0.08f, 0.08f, 1f), new Color32(44, 40, 36, 255), RewardSortingOrder + 2);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentPaintEdge", new Vector3(-0.33f, 0f, 0f), new Vector3(0.08f, 0.13f, 1f), new Color32(190, 66, 72, 255), RewardSortingOrder + 2);

        SketchWorldLineDrawing outline = pencilTransform.gameObject.AddComponent<SketchWorldLineDrawing>();
        outline.Configure(0.035f, new Color32(38, 34, 28, 255), RewardSortingOrder + 3);
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
        pixelSprite.name = "GeneratedForestBirdPencilRewardPixel";
        return pixelSprite;
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

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }
}

public class ForestWellBottleInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float interactDistance = 1.9f;

    private const int BottleSortingOrder = 78;

    private Transform player;
    private GameObject promptObject;
    private CircleCollider2D interactionCollider;
    private SketchWorldLineDrawing lineDrawing;
    private int promptSortingOrder = 72;
    private bool hasConfigured;
    private bool isCollecting;

    public void Configure(float distance, int sortingOrder)
    {
        interactDistance = Mathf.Max(0.2f, distance);
        promptSortingOrder = sortingOrder;
        hasConfigured = true;
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(CanCollect());
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || !hasConfigured || !CanCollect() || isCollecting || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > interactDistance)
        {
            return;
        }

        StartCoroutine(CollectRoutine(interactor.transform));
    }

    private void Awake()
    {
        lineDrawing = GetComponent<SketchWorldLineDrawing>();
        EnsurePrompt();
        EnsureCollider();
        SetInteractionAvailable(false);
    }

    private void OnEnable()
    {
        FindPlayer();
        SetInteractionAvailable(CanCollect());

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!hasConfigured || !CanCollect())
        {
            SetInteractionAvailable(false);
            return;
        }

        if (player == null)
        {
            FindPlayer();
        }

        bool isRevealed = lineDrawing == null || lineDrawing.RevealProgress >= 0.95f;
        bool isNear = player != null && Vector2.Distance(transform.position, player.position) <= interactDistance;

        SetInteractionAvailable(isRevealed && !isCollecting);

        if (promptObject != null)
        {
            promptObject.SetActive(isRevealed && isNear && !isCollecting);
        }
    }

    private void OnDisable()
    {
        SetInteractionAvailable(false);
    }

    private IEnumerator CollectRoutine(Transform interactor)
    {
        isCollecting = true;
        SetInteractionAvailable(false);

        Transform itemParent = transform.parent != null ? transform.parent : transform;
        GameObject bottleObject = CreateWaterBottleObject(itemParent);
        bottleObject.transform.position = transform.position + new Vector3(0f, 1.42f, 0f);

        Vector3 startPosition = bottleObject.transform.position;
        Vector3 fallbackTargetPosition = startPosition + new Vector3(0f, 0.82f, 0f);
        Vector3 startScale = bottleObject.transform.localScale;
        Quaternion startRotation = bottleObject.transform.localRotation;
        const float collectDuration = 0.5f;
        float elapsed = 0f;

        while (elapsed < collectDuration && bottleObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 targetPosition = interactor != null
                ? interactor.position + new Vector3(0f, 0.72f, 0f)
                : fallbackTargetPosition;
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.26f;

            bottleObject.transform.position = position;
            bottleObject.transform.localScale = Vector3.Lerp(startScale, startScale * 0.18f, easedProgress);
            bottleObject.transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, Mathf.Lerp(-10f, 24f, progress));
            yield return null;
        }

        GameProgress.CollectWellWaterBottle();
        LetterQuestHud.ShowProgress();

        if (bottleObject != null)
        {
            Destroy(bottleObject);
        }

        isCollecting = false;
        SetInteractionAvailable(false);
    }

    private bool CanCollect()
    {
        return hasConfigured && !GameProgress.HasCollectedWellWaterBottle;
    }

    private void EnsurePrompt()
    {
        if (promptObject == null)
        {
            promptObject = new GameObject("GeneratedForestWellPrompt");
            promptObject.transform.SetParent(transform, false);
            promptObject.transform.localPosition = new Vector3(0f, 2.72f, 0f);

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
        interactionCollider.radius = interactDistance;
        interactionCollider.offset = new Vector2(0f, 0.28f);
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = available;
        }

        if (!available && promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private GameObject CreateWaterBottleObject(Transform parent)
    {
        GameObject bottleObject = new GameObject("GeneratedWellWaterBottleReward");
        bottleObject.transform.SetParent(parent, true);
        bottleObject.transform.localScale = Vector3.one;

        SketchWorldLineDrawing outline = bottleObject.AddComponent<SketchWorldLineDrawing>();
        outline.Configure(0.035f, new Color32(37, 92, 94, 230), BottleSortingOrder);
        outline.SetStrokes(BuildWaterBottleOutlineStrokes());
        outline.RevealProgress = 1f;

        GameObject waterObject = new GameObject("GeneratedWellWaterBottleFill");
        waterObject.transform.SetParent(bottleObject.transform, false);

        SketchWorldLineDrawing water = waterObject.AddComponent<SketchWorldLineDrawing>();
        water.Configure(0.05f, new Color32(72, 151, 190, 210), BottleSortingOrder + 1);
        water.SetStrokes(BuildWaterBottleWaterStrokes());
        water.RevealProgress = 1f;

        return bottleObject;
    }

    private static List<Vector3[]> BuildWaterBottleOutlineStrokes()
    {
        return new List<Vector3[]>
        {
            Points(-0.18f, 0.44f, -0.18f, 0.24f, -0.34f, 0.08f, -0.38f, -0.42f, -0.24f, -0.72f, 0.24f, -0.72f, 0.38f, -0.42f, 0.34f, 0.08f, 0.18f, 0.24f, 0.18f, 0.44f, -0.18f, 0.44f),
            Points(-0.14f, 0.6f, -0.14f, 0.44f, 0.14f, 0.44f, 0.14f, 0.6f, -0.14f, 0.6f),
            Points(-0.2f, 0.62f, 0.2f, 0.62f),
            Points(-0.28f, -0.44f, 0.28f, -0.44f)
        };
    }

    private static List<Vector3[]> BuildWaterBottleWaterStrokes()
    {
        return new List<Vector3[]>
        {
            Points(-0.28f, -0.32f, -0.12f, -0.26f, 0.08f, -0.34f, 0.26f, -0.28f),
            Points(-0.26f, -0.48f, 0.24f, -0.48f),
            Points(-0.18f, -0.58f, 0.18f, -0.58f)
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

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }
}

public class QuestNpcLetterRewardInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float interactDistance = 2.3f;

    private const int RewardSortingOrder = 78;

    private static Sprite pixelSprite;

    private SketchOutsideTransition owner;
    private CircleCollider2D interactionCollider;
    private bool isDelivering;

    public void Configure(SketchOutsideTransition transitionOwner, float distance)
    {
        owner = transitionOwner;
        interactDistance = Mathf.Max(0.2f, distance);
        EnsureCollider();
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || isDelivering || interactor == null || !CanInteractWithNpc())
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > interactDistance)
        {
            return;
        }

        if (CanDeliverReward())
        {
            StartCoroutine(DeliverRewardRoutine(interactor.transform));
            return;
        }

        if (CanShowPondGuide())
        {
            GameProgress.MeetQuestNpc();
            if (owner != null)
            {
                owner.PlayQuestNpcPondGuide();
            }
        }
    }

    private void Awake()
    {
        EnsureCollider();
    }

    private void OnEnable()
    {
        SetInteractionAvailable(CanInteractWithNpc());
    }

    private void Update()
    {
        SetInteractionAvailable(CanInteractWithNpc() && !isDelivering);
    }

    private void OnDisable()
    {
        SetInteractionAvailable(false);
    }

    private bool CanDeliverReward()
    {
        return GameProgress.HasCompletedLetter
            && !GameProgress.HasDeliveredCompletedLetter
            && !GameProgress.HasCollectedPencilFragmentFrom(PencilFragmentSource.Npc);
    }

    private bool CanShowPondGuide()
    {
        return !GameProgress.HasCompletedLetter
            && !GameProgress.HasCollectedBottleLetterFragment;
    }

    private bool CanInteractWithNpc()
    {
        return CanDeliverReward() || CanShowPondGuide();
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null && interactionCollider.enabled != available)
        {
            interactionCollider.enabled = available;
        }
    }

    private IEnumerator DeliverRewardRoutine(Transform player)
    {
        isDelivering = true;

        GameObject rewardObject = new GameObject("GeneratedQuestNpcPencilReward");
        rewardObject.transform.SetParent(transform.parent, true);
        rewardObject.transform.position = transform.position + new Vector3(0f, 1.55f, 0f);
        CreatePencilFragmentVisual(rewardObject.transform);

        Vector3 startPosition = rewardObject.transform.position;
        Vector3 endPosition = player != null
            ? player.position + new Vector3(0f, 0.72f, 0f)
            : startPosition + new Vector3(0.8f, 0.4f, 0f);

        const float rewardDuration = 0.48f;
        float elapsed = 0f;

        while (elapsed < rewardDuration && rewardObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / rewardDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 position = Vector3.Lerp(startPosition, endPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.22f;

            rewardObject.transform.position = position;
            rewardObject.transform.localScale = Vector3.one * Mathf.Lerp(1f, 0.28f, easedProgress);
            rewardObject.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-14f, 16f, progress));
            yield return null;
        }

        GameProgress.DeliverCompletedLetter();
        GameProgress.CollectPencilFragmentFrom(PencilFragmentSource.Npc);
        LetterQuestHud.ShowProgress();
        PencilFragmentHud.ShowCollected();

        if (rewardObject != null)
        {
            Destroy(rewardObject);
        }

        isDelivering = false;
    }

    private void EnsureCollider()
    {
        if (interactionCollider != null)
        {
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = 1.35f;
        interactionCollider.offset = new Vector2(0f, 0.72f);
    }

    private static void CreatePencilFragmentVisual(Transform pencilTransform)
    {
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentBody", new Vector3(-0.06f, 0f, 0f), new Vector3(0.48f, 0.12f, 1f), new Color32(236, 181, 51, 255), RewardSortingOrder);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentWood", new Vector3(0.24f, 0f, 0f), new Vector3(0.16f, 0.12f, 1f), new Color32(203, 143, 77, 255), RewardSortingOrder + 1);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentLead", new Vector3(0.34f, 0f, 0f), new Vector3(0.08f, 0.08f, 1f), new Color32(44, 40, 36, 255), RewardSortingOrder + 2);
        CreatePencilPart(pencilTransform, "GeneratedPencilFragmentPaintEdge", new Vector3(-0.33f, 0f, 0f), new Vector3(0.08f, 0.13f, 1f), new Color32(190, 66, 72, 255), RewardSortingOrder + 2);
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
        pixelSprite.name = "GeneratedQuestNpcPencilRewardPixel";
        return pixelSprite;
    }
}

public static class SketchOutsideTransitionPromptDefaults
{
    public const int LetterPromptSortingOrder = 72;
}

public static class LetterQuestTreeRoute
{
    private const int MaxRouteTrees = 5;
    private const int LetterSortingOrder = 77;
    private const float TreeCatchHeight = 2.95f;

    private static readonly RouteTree[] routeTrees = new RouteTree[MaxRouteTrees];
    private static int routeTreeCount;
    private static GameObject activeLetterObject;
    private static LetterQuestTreeRouteRunner runner;

    public static void ResetRoute()
    {
        routeTreeCount = 0;

        for (int i = 0; i < routeTrees.Length; i++)
        {
            routeTrees[i] = default;
        }

        if (activeLetterObject != null)
        {
            Object.Destroy(activeLetterObject);
            activeLetterObject = null;
        }
    }

    public static int RegisterTree(Vector3 localPosition, Transform parent, SketchWorldLineDrawing treeDrawing)
    {
        if (routeTreeCount >= MaxRouteTrees || parent == null)
        {
            return -1;
        }

        int index = routeTreeCount;
        routeTrees[index] = new RouteTree(localPosition, parent, treeDrawing);
        routeTreeCount++;
        return index;
    }

    public static void BeginRouteFrom(Vector3 startWorldPosition, Transform fallbackParent)
    {
        if (routeTreeCount <= 0 || GameProgress.HasCollectedTreeLetterFragment)
        {
            return;
        }

        GameProgress.StartLetterTreeChase();
        GameProgress.MoveLetterToTree(0);
        LetterQuestTreeRouteRunner routeRunner = GetRunner(fallbackParent);
        routeRunner.StartCoroutine(AnimateWindLetterToTreeRoutine(startWorldPosition, 0, fallbackParent));
    }

    public static bool TryHandleTreeShake(int routeIndex, Vector3 treeWorldPosition, Transform fallbackParent, MonoBehaviour owner)
    {
        if (owner == null || routeIndex < 0 || !GameProgress.IsLetterWaitingOnTree(routeIndex))
        {
            return false;
        }

        owner.StartCoroutine(HandleTreeShakeRoutine(routeIndex, treeWorldPosition, fallbackParent));
        return true;
    }

    private static IEnumerator HandleTreeShakeRoutine(int routeIndex, Vector3 treeWorldPosition, Transform fallbackParent)
    {
        if (activeLetterObject != null)
        {
            Object.Destroy(activeLetterObject);
            activeLetterObject = null;
        }

        Vector3 startPosition = treeWorldPosition + new Vector3(0f, TreeCatchHeight, 0f);

        if (routeIndex < routeTreeCount - 1)
        {
            int nextIndex = routeIndex + 1;
            GameProgress.MoveLetterToTree(nextIndex);
            yield return AnimateWindLetterToTreeRoutine(startPosition, nextIndex, fallbackParent);
            yield break;
        }

        yield return AnimateFinalTreeLetterCollectRoutine(startPosition, fallbackParent);
        GameProgress.CompleteLetterTreeChase();
        LetterQuestHud.ShowProgress();
        yield return LetterQuestWorldDrop.PlayRemainingLetterIntoGrass(startPosition, fallbackParent);
    }

    private static IEnumerator AnimateWindLetterToTreeRoutine(Vector3 startWorldPosition, int targetTreeIndex, Transform fallbackParent)
    {
        if (targetTreeIndex < 0 || targetTreeIndex >= routeTreeCount)
        {
            yield break;
        }

        Transform itemParent = GetItemParent(targetTreeIndex, fallbackParent);
        Vector3 endWorldPosition = GetTreeCatchWorldPosition(targetTreeIndex);
        GameObject letterObject = CreateTornLetterObject("GeneratedWindCarriedLetter", itemParent, startWorldPosition);
        WindMotionEffect windEffect = WindMotionEffect.Create(itemParent, startWorldPosition, LetterSortingOrder - 1);
        Vector3 previousPosition = startWorldPosition;
        Vector3 flightVector = endWorldPosition - startWorldPosition;
        Vector3 travelDirection = flightVector.sqrMagnitude > 0.001f ? flightVector.normalized : Vector3.right;
        Vector3 crosswindDirection = new Vector3(-travelDirection.y, travelDirection.x, 0f);

        const float flightDuration = 2.1f;
        float elapsed = 0f;

        while (elapsed < flightDuration && letterObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / flightDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 position = Vector3.Lerp(startWorldPosition, endWorldPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 1.25f;
            position.x += Mathf.Sin(progress * Mathf.PI * 5f) * 0.18f;
            float gustEnvelope = Mathf.Sin(progress * Mathf.PI);
            position += crosswindDirection * (Mathf.Sin(progress * Mathf.PI * 3.25f) * 0.28f + Mathf.Sin(progress * Mathf.PI * 8.5f) * 0.06f) * gustEnvelope;
            position += travelDirection * Mathf.Sin(progress * Mathf.PI * 6f) * 0.08f * gustEnvelope;
            Vector3 movementDirection = position - previousPosition;

            letterObject.transform.position = position;
            letterObject.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-18f, 26f, progress) + Mathf.Sin(progress * Mathf.PI * 8f) * 10f + Mathf.Sin(progress * Mathf.PI * 17f) * 3f);
            windEffect.UpdateEffect(position, movementDirection, progress, 1f + Mathf.Sin(progress * Mathf.PI * 4f) * 0.18f);
            previousPosition = position;
            yield return null;
        }

        if (windEffect != null)
        {
            windEffect.Finish();
        }

        if (letterObject != null)
        {
            letterObject.transform.position = endWorldPosition;
            letterObject.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-10f, 12f));
            letterObject.SetActive(IsTreeRevealed(targetTreeIndex));
        }

        activeLetterObject = letterObject;
    }

    private static IEnumerator AnimateFinalTreeLetterCollectRoutine(Vector3 startWorldPosition, Transform fallbackParent)
    {
        Transform itemParent = fallbackParent != null ? fallbackParent : GetItemParent(0, null);
        GameObject fragmentObject = CreateLetterFragmentObject("GeneratedTreeLetterFragment", itemParent, startWorldPosition);

        yield return new WaitForSeconds(0.16f);

        Vector3 targetPosition = GetPlayerCollectPosition(startWorldPosition);
        Vector3 startScale = fragmentObject.transform.localScale;
        const float collectDuration = 0.42f;
        float elapsed = 0f;

        while (elapsed < collectDuration && fragmentObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / collectDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 position = Vector3.Lerp(startWorldPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.24f;

            fragmentObject.transform.position = position;
            fragmentObject.transform.localScale = Vector3.Lerp(startScale, startScale * 0.2f, easedProgress);
            fragmentObject.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-12f, 8f, progress));
            yield return null;
        }

        if (fragmentObject != null)
        {
            Object.Destroy(fragmentObject);
        }
    }

    private static GameObject CreateTornLetterObject(string objectName, Transform parent, Vector3 position)
    {
        GameObject letterObject = new GameObject(objectName);
        letterObject.transform.SetParent(parent, true);
        letterObject.transform.position = position;
        letterObject.transform.localScale = Vector3.one * 0.82f;

        SketchWorldLineDrawing drawing = letterObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(0.035f, new Color32(78, 57, 34, 255), LetterSortingOrder);
        drawing.SetStrokes(BuildTornLetterStrokes());
        drawing.RevealProgress = 1f;
        return letterObject;
    }

    private static GameObject CreateLetterFragmentObject(string objectName, Transform parent, Vector3 position)
    {
        GameObject fragmentObject = new GameObject(objectName);
        fragmentObject.transform.SetParent(parent, true);
        fragmentObject.transform.position = position;
        fragmentObject.transform.localScale = Vector3.one * 0.88f;

        SketchWorldLineDrawing drawing = fragmentObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(0.035f, new Color32(78, 57, 34, 255), LetterSortingOrder);
        drawing.SetStrokes(BuildLetterFragmentStrokes());
        drawing.RevealProgress = 1f;
        return fragmentObject;
    }

    private static Vector3 GetTreeCatchWorldPosition(int treeIndex)
    {
        RouteTree routeTree = routeTrees[treeIndex];
        return routeTree.Parent.TransformPoint(routeTree.LocalPosition + new Vector3(0f, TreeCatchHeight, 0f));
    }

    private static Transform GetItemParent(int treeIndex, Transform fallbackParent)
    {
        if (treeIndex >= 0 && treeIndex < routeTreeCount && routeTrees[treeIndex].Parent != null)
        {
            return routeTrees[treeIndex].Parent;
        }

        return fallbackParent;
    }

    private static Vector3 GetPlayerCollectPosition(Vector3 fallbackPosition)
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        return playerObject != null
            ? playerObject.transform.position + new Vector3(0f, 0.72f, 0f)
            : fallbackPosition + new Vector3(0f, 0.82f, 0f);
    }

    private static LetterQuestTreeRouteRunner GetRunner(Transform parent)
    {
        if (runner != null)
        {
            return runner;
        }

        GameObject runnerObject = new GameObject("GeneratedLetterQuestTreeRouteRunner");
        if (parent != null)
        {
            runnerObject.transform.SetParent(parent, false);
        }

        runner = runnerObject.AddComponent<LetterQuestTreeRouteRunner>();
        return runner;
    }

    public static void UpdateActiveLetterVisibility()
    {
        if (activeLetterObject == null || GameProgress.HasCollectedTreeLetterFragment)
        {
            return;
        }

        int activeTreeIndex = GameProgress.ActiveLetterTreeIndex;
        if (activeTreeIndex < 0 || activeTreeIndex >= routeTreeCount)
        {
            activeLetterObject.SetActive(false);
            return;
        }

        activeLetterObject.SetActive(IsTreeRevealed(activeTreeIndex));
    }

    private static bool IsTreeRevealed(int treeIndex)
    {
        if (treeIndex < 0 || treeIndex >= routeTreeCount)
        {
            return false;
        }

        SketchWorldLineDrawing treeDrawing = routeTrees[treeIndex].TreeDrawing;
        return treeDrawing == null || treeDrawing.RevealProgress >= 0.95f;
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

    private struct RouteTree
    {
        public readonly Vector3 LocalPosition;
        public readonly Transform Parent;
        public readonly SketchWorldLineDrawing TreeDrawing;

        public RouteTree(Vector3 localPosition, Transform parent, SketchWorldLineDrawing treeDrawing)
        {
            LocalPosition = localPosition;
            Parent = parent;
            TreeDrawing = treeDrawing;
        }
    }
}

public class LetterQuestTreeRouteRunner : MonoBehaviour
{
    private void Update()
    {
        LetterQuestTreeRoute.UpdateActiveLetterVisibility();
    }
}

public class TreeCrayonColorTarget : MonoBehaviour
{
    public float TreeScale { get; private set; } = 1f;

    public void Configure(float treeScale)
    {
        TreeScale = Mathf.Max(0.1f, treeScale);
    }
}

public class TreeCrayonRevealSync : MonoBehaviour
{
    private SketchWorldLineDrawing sourceDrawing;
    private SketchWorldLineDrawing overlayDrawing;

    public void Configure(SketchWorldLineDrawing source, SketchWorldLineDrawing overlay)
    {
        sourceDrawing = source;
        overlayDrawing = overlay;
        SyncRevealProgress();
    }

    private void LateUpdate()
    {
        SyncRevealProgress();
    }

    private void SyncRevealProgress()
    {
        if (sourceDrawing == null || overlayDrawing == null)
        {
            return;
        }

        overlayDrawing.RevealProgress = sourceDrawing.RevealProgress;
    }
}

public class LeafPileCrayonColorTarget : MonoBehaviour
{
    public float LeafPileScale { get; private set; } = 1f;

    public void Configure(float leafPileScale)
    {
        LeafPileScale = Mathf.Max(0.1f, leafPileScale);
    }
}

public class GrassCrayonColorOverlay : MonoBehaviour
{
    private SpriteRenderer sourceRenderer;
    private SpriteRenderer overlayRenderer;
    private Color baseOverlayColor = Color.white;

    public void Configure(SpriteRenderer source, SpriteRenderer overlay, Color overlayColor)
    {
        sourceRenderer = source;
        overlayRenderer = overlay;
        baseOverlayColor = overlayColor;
        SyncOverlay();
    }

    private void LateUpdate()
    {
        SyncOverlay();
    }

    private void SyncOverlay()
    {
        if (sourceRenderer == null || overlayRenderer == null)
        {
            return;
        }

        Color color = baseOverlayColor;
        color.a *= sourceRenderer.color.a;
        overlayRenderer.color = color;
        overlayRenderer.enabled = sourceRenderer.enabled;
    }
}

public static class LetterQuestWorldDrop
{
    private const int MaxGrassPatches = 220;
    private const int LetterSortingOrder = 77;
    private static readonly GrassPatch[] grassPatches = new GrassPatch[MaxGrassPatches];
    private static int grassPatchCount;
    private static int finalGrassPatchIndex = -1;
    private static Vector3 targetLocalPosition;
    private static Transform targetParent;
    private static bool hasSpawnedFinalGrassLetterFragment;
    private static GameObject activeFinalGrassLetterFragment;

    public static void Reset()
    {
        grassPatchCount = 0;
        finalGrassPatchIndex = -1;
        targetLocalPosition = Vector3.zero;
        targetParent = null;
        hasSpawnedFinalGrassLetterFragment = false;
        activeFinalGrassLetterFragment = null;

        for (int i = 0; i < grassPatches.Length; i++)
        {
            grassPatches[i] = default;
        }

    }

    public static void ConfigureFinalGrassTarget(Vector3 localPosition, Transform parent)
    {
        targetLocalPosition = localPosition;
        targetParent = parent;
    }

    public static void RegisterGrassPatch(Vector3 localPosition, Transform parent, Transform grassTransform)
    {
        if (parent == null || grassTransform == null || grassPatchCount >= MaxGrassPatches)
        {
            return;
        }

        grassPatches[grassPatchCount] = new GrassPatch(localPosition, parent, grassTransform);
        grassPatchCount++;
    }

    public static IEnumerator PlayRemainingLetterIntoGrass(Vector3 startWorldPosition, Transform fallbackParent)
    {
        finalGrassPatchIndex = GetTargetGrassPatchIndex();
        hasSpawnedFinalGrassLetterFragment = false;
        Vector3 targetWorldPosition = GetGrassWorldPosition(finalGrassPatchIndex, startWorldPosition, fallbackParent);
        Transform parent = GetGrassParent(finalGrassPatchIndex, fallbackParent);
        GameObject remainingLetterObject = new GameObject("GeneratedRemainingLetterIntoGrass");

        if (parent != null)
        {
            remainingLetterObject.transform.SetParent(parent, true);
        }

        remainingLetterObject.transform.position = startWorldPosition + new Vector3(0.18f, 0.1f, 0f);
        remainingLetterObject.transform.localScale = Vector3.one * 0.78f;

        SketchWorldLineDrawing drawing = remainingLetterObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(0.035f, new Color32(78, 57, 34, 255), LetterSortingOrder);
        drawing.SetStrokes(BuildTornLetterStrokes());
        drawing.RevealProgress = 1f;

        Vector3 flightStartPosition = remainingLetterObject.transform.position;
        Vector3 flightEndPosition = targetWorldPosition + new Vector3(0f, 0.5f, 0f);
        WindMotionEffect windEffect = WindMotionEffect.Create(parent, flightStartPosition, LetterSortingOrder - 1);
        Vector3 previousPosition = flightStartPosition;
        Vector3 flightVector = flightEndPosition - flightStartPosition;
        Vector3 travelDirection = flightVector.sqrMagnitude > 0.001f ? flightVector.normalized : Vector3.right;
        Vector3 crosswindDirection = new Vector3(-travelDirection.y, travelDirection.x, 0f);
        const float flightDuration = 1.8f;
        float elapsed = 0f;

        while (elapsed < flightDuration && remainingLetterObject != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / flightDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 position = Vector3.Lerp(flightStartPosition, flightEndPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.82f;
            position.x += Mathf.Sin(progress * Mathf.PI * 6f) * 0.12f;
            float gustEnvelope = Mathf.Sin(progress * Mathf.PI);
            position += crosswindDirection * (Mathf.Sin(progress * Mathf.PI * 3.6f) * 0.22f + Mathf.Sin(progress * Mathf.PI * 9f) * 0.05f) * gustEnvelope;
            position += travelDirection * Mathf.Sin(progress * Mathf.PI * 5.5f) * 0.06f * gustEnvelope;
            Vector3 movementDirection = position - previousPosition;

            remainingLetterObject.transform.position = position;
            remainingLetterObject.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-18f, 28f, progress) + Mathf.Sin(progress * Mathf.PI * 9f) * 7f + Mathf.Sin(progress * Mathf.PI * 18f) * 2.5f);
            remainingLetterObject.transform.localScale = Vector3.one * Mathf.Lerp(0.78f, 0.56f, easedProgress);
            windEffect.UpdateEffect(position, movementDirection, progress, 0.92f + Mathf.Sin(progress * Mathf.PI * 4.5f) * 0.14f);
            previousPosition = position;
            yield return null;
        }

        if (remainingLetterObject != null)
        {
            Vector3 sinkStartPosition = remainingLetterObject.transform.position;
            Vector3 sinkEndPosition = targetWorldPosition + new Vector3(0f, 0.08f, 0f);
            const float sinkDuration = 0.28f;
            elapsed = 0f;

            while (elapsed < sinkDuration && remainingLetterObject != null)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / sinkDuration);
                float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
                Vector3 sinkPosition = Vector3.Lerp(sinkStartPosition, sinkEndPosition, easedProgress);

                remainingLetterObject.transform.position = sinkPosition;
                remainingLetterObject.transform.localScale = Vector3.one * Mathf.Lerp(0.56f, 0.12f, easedProgress);
                windEffect.UpdateEffect(sinkPosition, sinkPosition - previousPosition, 1f, Mathf.Lerp(0.55f, 0.15f, easedProgress));
                previousPosition = sinkPosition;
                yield return null;
            }
        }

        if (windEffect != null)
        {
            windEffect.Finish();
        }

        if (remainingLetterObject != null)
        {
            Object.Destroy(remainingLetterObject);
        }

        MarkGrassPatchContainsLetter(finalGrassPatchIndex);
    }

    public static bool TrySpawnFinalGrassLetterFragmentFrom(Transform grassTransform)
    {
        if (!GameProgress.CanFindFinalGrassLetterFragment || grassTransform == null)
        {
            return false;
        }

        if (finalGrassPatchIndex < 0 || finalGrassPatchIndex >= grassPatchCount)
        {
            return false;
        }

        GrassPatch patch = grassPatches[finalGrassPatchIndex];
        if (patch.Parent == null || patch.GrassTransform == null)
        {
            return false;
        }

        if (grassTransform != patch.GrassTransform)
        {
            return false;
        }

        if ((hasSpawnedFinalGrassLetterFragment || GameProgress.HasRevealedGrassLetterFragment)
            && activeFinalGrassLetterFragment != null
            && activeFinalGrassLetterFragment.activeInHierarchy)
        {
            return true;
        }

        hasSpawnedFinalGrassLetterFragment = true;
        GameProgress.RevealGrassLetterFragment();

        GameObject fragmentObject = new GameObject("GeneratedFinalGrassLetterFragment");
        fragmentObject.transform.SetParent(grassTransform.parent, true);
        fragmentObject.transform.position = grassTransform.position + new Vector3(0f, 0.42f, 0f);
        fragmentObject.transform.localScale = Vector3.one * 0.88f;
        activeFinalGrassLetterFragment = fragmentObject;

        SketchWorldLineDrawing drawing = fragmentObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(0.035f, new Color32(78, 57, 34, 255), LetterSortingOrder);
        drawing.SetStrokes(BuildLetterFragmentStrokes());
        drawing.RevealProgress = 1f;

        LetterFragmentInteract interact = fragmentObject.AddComponent<LetterFragmentInteract>();
        interact.Configure(LetterFragmentSource.GrassSearch, 1.45f, SketchOutsideTransitionPromptDefaults.LetterPromptSortingOrder);
        interact.enabled = true;
        interact.CollectImmediately();
        return true;
    }

    private static Vector3 GetGrassWorldPosition(int grassIndex, Vector3 fallbackStartPosition, Transform fallbackParent)
    {
        if (grassIndex >= 0 && grassIndex < grassPatchCount)
        {
            GrassPatch patch = grassPatches[grassIndex];
            return patch.Parent.TransformPoint(patch.LocalPosition);
        }

        return fallbackParent != null
            ? fallbackParent.TransformPoint(new Vector3(0f, -2.5f, 0f))
            : fallbackStartPosition + new Vector3(1.2f, -1.3f, 0f);
    }

    private static Transform GetGrassParent(int grassIndex, Transform fallbackParent)
    {
        if (grassIndex >= 0 && grassIndex < grassPatchCount)
        {
            return grassPatches[grassIndex].Parent;
        }

        return fallbackParent;
    }

    private static void MarkGrassPatchContainsLetter(int grassIndex)
    {
        if (grassIndex < 0 || grassIndex >= grassPatchCount)
        {
            return;
        }

        Transform grassTransform = grassPatches[grassIndex].GrassTransform;
        if (grassTransform == null)
        {
            return;
        }

        GrassSwayOnPlayerNear grassSway = grassTransform.GetComponent<GrassSwayOnPlayerNear>();
        if (grassSway != null)
        {
            grassSway.MarkContainsLetterFragment();
        }
    }

    private static int GetClosestGrassPatchIndex(Vector3 startWorldPosition)
    {
        int closestIndex = -1;
        float closestDistanceSqr = float.MaxValue;

        for (int i = 0; i < grassPatchCount; i++)
        {
            GrassPatch patch = grassPatches[i];
            if (patch.Parent == null)
            {
                continue;
            }

            Vector3 worldPosition = patch.Parent.TransformPoint(patch.LocalPosition);
            float distanceSqr = (worldPosition - startWorldPosition).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    private static int GetTargetGrassPatchIndex()
    {
        int selectedIndex = -1;
        float selectedDistanceSqr = float.MaxValue;
        Vector3 targetWorldPosition = targetParent != null
            ? targetParent.TransformPoint(targetLocalPosition)
            : targetLocalPosition;

        for (int i = 0; i < grassPatchCount; i++)
        {
            GrassPatch patch = grassPatches[i];
            if (patch.Parent == null)
            {
                continue;
            }

            Vector3 worldPosition = patch.Parent.TransformPoint(patch.LocalPosition);
            float distanceSqr = (worldPosition - targetWorldPosition).sqrMagnitude;

            if (distanceSqr < selectedDistanceSqr)
            {
                selectedDistanceSqr = distanceSqr;
                selectedIndex = i;
            }
        }

        return selectedIndex;
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

    private struct GrassPatch
    {
        public readonly Vector3 LocalPosition;
        public readonly Transform Parent;
        public readonly Transform GrassTransform;

        public GrassPatch(Vector3 localPosition, Transform parent, Transform grassTransform)
        {
            LocalPosition = localPosition;
            Parent = parent;
            GrassTransform = grassTransform;
        }
    }
}
