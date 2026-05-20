using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SketchOutsideTransition : MonoBehaviour
{
    private const float BackgroundRevealDuration = 1.15f;
    private const float HouseDrawDuration = 4.2f;
    private const float HouseLineWidth = 0.065f;
    private const int BackgroundSortingOrder = -20;
    private const int PropSortingOrder = 7;
    private const int GrassSortingOrder = PropSortingOrder + 1;
    private const int TreeSortingOrder = PropSortingOrder + 3;
    private const int MailboxSortingOrder = PropSortingOrder + 4;
    private const int DecorationRandomSeed = 20260520;
    private const int TreeSpawnCount = 10;
    private const int GrassSpawnCount = 34;
    private const int DecorationPickAttempts = 700;
    private const float BoundaryColliderThickness = 0.55f;
    private const float MailboxHouseThirdScale = 0.52f;
    private const float DecorationOverlapPadding = 0.28f;
    private const string TreeResourcePath = "Outside/tree_thick";
    private const string GrassResourcePath = "Outside/grass_thick";
    private const string MailboxResourcePath = "Outside/mailbox_thick";
    private static readonly Vector2 GrassScaleRange = new Vector2(0.51f, 0.78f);
    private static readonly Vector2 TreeScaleRange = new Vector2(1.12f, 1.32f);
    private static readonly Vector3 OutsidePlayerOffset = Vector3.zero;
    private static readonly Vector3 HouseOffset = new Vector3(0f, 2f, 0f);
    private static readonly Vector3 DoorOffset = new Vector3(0f, 1.35f, 0f);
    private static readonly Vector3 NeighborHouseOffset = new Vector3(12.2f, 2.05f, 0f);
    private static readonly Vector3 ForestEntranceOffset = new Vector3(13.8f, -10.25f, 0f);
    private static readonly Vector2 BackgroundSize = new Vector2(40f, 28f);
    private static readonly Vector2 DecorationMin = new Vector2(-18f, -11f);
    private static readonly Vector2 DecorationMax = new Vector2(18f, 11f);

    private static SketchOutsideTransition instance;

    private GameObject contentRoot;
    private GameObject propRoot;
    private SpriteRenderer backgroundRenderer;
    private SketchWorldLineDrawing houseDrawing;
    private SketchOutsideDoorInteract houseDoorInteract;
    private BoxCollider2D houseDoorCollider;
    private Coroutine transitionRoutine;
    private Sprite whiteSprite;
    private bool hasCompletedOutsideMap;

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

        if (contentRoot != null)
        {
            houseDoorInteract.SetInteractionEnabled(false);
        }
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
        transform.position = exitTargetPosition;
        contentRoot.SetActive(true);

        player.transform.position = exitTargetPosition + OutsidePlayerOffset;
        houseDoorInteract.Configure(this, interiorEntryPosition, 1.6f);

        if (hasCompletedOutsideMap)
        {
            SetCompletedOutsideMapState();
            SetPlayerMovementEnabled(player, true);
            transitionRoutine = null;
            yield break;
        }

        SetPlayerMovementEnabled(player, false);
        SetInitialOutsideMapState();

        yield return RevealWhiteBackground();
        yield return DrawHouse();

        if (propRoot != null)
        {
            propRoot.SetActive(true);
        }

        hasCompletedOutsideMap = true;
        houseDoorCollider.enabled = true;
        houseDoorInteract.SetInteractionEnabled(true);
        SetPlayerMovementEnabled(player, true);
        transitionRoutine = null;
    }

    private void SetInitialOutsideMapState()
    {
        backgroundRenderer.transform.localPosition = Vector3.zero;
        backgroundRenderer.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
        backgroundRenderer.color = Color.white;
        houseDrawing.RevealProgress = 0f;
        houseDoorCollider.enabled = false;
        houseDoorInteract.SetInteractionEnabled(false);

        if (propRoot != null)
        {
            propRoot.SetActive(false);
        }
    }

    private void SetCompletedOutsideMapState()
    {
        backgroundRenderer.transform.localPosition = Vector3.zero;
        backgroundRenderer.transform.localScale = new Vector3(BackgroundSize.x, BackgroundSize.y, 1f);
        backgroundRenderer.color = Color.white;
        houseDrawing.RevealProgress = 1f;
        houseDoorCollider.enabled = true;
        houseDoorInteract.SetInteractionEnabled(true);

        if (propRoot != null)
        {
            propRoot.SetActive(true);
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
        drawing.Configure(HouseLineWidth, new Color32(35, 32, 28, 255), 8);
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
        propRoot = new GameObject("GeneratedOutsideProps");
        propRoot.transform.SetParent(contentRoot.transform, false);

        CreateOutsideBoundary();

        SketchWorldLineDrawing neighborHouse = CreateLineDrawing(
            "GeneratedNeighborHouseWithMailbox",
            NeighborHouseOffset,
            BuildNeighborHouseStrokes(),
            8,
            propRoot.transform);
        neighborHouse.RevealProgress = 1f;

        CreateSpriteProp(
            "GeneratedNeighborMailbox",
            MailboxResourcePath,
            NeighborHouseOffset + new Vector3(-2.02f, -0.96f, 0f),
            new Vector3(MailboxHouseThirdScale, MailboxHouseThirdScale, 1f),
            MailboxSortingOrder,
            propRoot.transform);

        CreateForestEntrance();
        CreateRandomOutsideDecorations();
        propRoot.SetActive(false);
    }

    private void CreateOutsideBoundary()
    {
        SketchWorldLineDrawing fenceDrawing = CreateLineDrawing(
            "GeneratedOutsideFenceLineDrawing",
            Vector3.zero,
            BuildFenceStrokes(),
            PropSortingOrder + 3,
            propRoot.transform);
        fenceDrawing.RevealProgress = 1f;

        GameObject boundaryRoot = new GameObject("GeneratedOutsideBoundaryColliders");
        boundaryRoot.transform.SetParent(propRoot.transform, false);

        float halfWidth = BackgroundSize.x * 0.5f;
        float halfHeight = BackgroundSize.y * 0.5f;
        float thickness = BoundaryColliderThickness;

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
        forestEntrance.RevealProgress = 1f;
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

        SpawnRandomSpriteProps("GeneratedTree", TreeResourcePath, TreeSpawnCount, 7f, TreeScaleRange.x, TreeScaleRange.y, TreeSortingOrder, random, usedFootprints, -16f, 16f, DecorationMin.y, 6.2f);
        SpawnRandomSpriteProps("GeneratedGrass", GrassResourcePath, GrassSpawnCount, 2.1f, GrassScaleRange.x, GrassScaleRange.y, GrassSortingOrder, random, usedFootprints, DecorationMin.x, DecorationMax.x, DecorationMin.y, DecorationMax.y);
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
                propRoot.transform);

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
            || IsInsideBox(position, Vector3.zero, new Vector2(4.4f, 3.2f));
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
        GameObject drawingObject = new GameObject(objectName);
        drawingObject.transform.SetParent(parent != null ? parent : contentRoot.transform, false);
        drawingObject.transform.localPosition = localPosition;

        SketchWorldLineDrawing drawing = drawingObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(HouseLineWidth, new Color32(35, 32, 28, 255), sortingOrder);
        drawing.SetStrokes(strokes);
        return drawing;
    }

    private void CreateSpriteProp(string objectName, string resourcePath, Vector3 localPosition, Vector3 localScale, int sortingOrder, Transform parent = null)
    {
        Sprite sprite = LoadSketchSprite(resourcePath);
        if (sprite == null)
        {
            Debug.LogWarning($"Outside prop sprite not found: {resourcePath}", this);
            return;
        }

        CreateSpriteProp(objectName, sprite, localPosition, localScale, sortingOrder, parent);
    }

    private void CreateSpriteProp(string objectName, Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Transform parent = null)
    {
        GameObject propObject = new GameObject(objectName);
        propObject.transform.SetParent(parent != null ? parent : contentRoot.transform, false);
        propObject.transform.localScale = localScale;

        SpriteRenderer spriteRenderer = propObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = sortingOrder;

        float groundedYOffset = sprite.bounds.extents.y * Mathf.Abs(localScale.y);
        propObject.transform.localPosition = localPosition + new Vector3(0f, groundedYOffset, 0f);
        CreatePropGroundStroke(objectName, localPosition, sprite.bounds.extents.x * Mathf.Abs(localScale.x), sortingOrder - 1, parent);
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
        groundStroke.RevealProgress = 1f;
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
            playerMove.enabled = enabled;
        }

        Rigidbody2D rigidbody2D = player.GetComponent<Rigidbody2D>();
        if (rigidbody2D != null)
        {
            rigidbody2D.linearVelocity = Vector2.zero;
        }
    }

    private static float ApplyHandDrawnPace(float progress)
    {
        float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
        float unevenPace = Mathf.Sin(progress * Mathf.PI * 7f) * 0.015f;
        float middleWeight = 1f - Mathf.Abs(progress * 2f - 1f);

        return Mathf.Clamp01(easedProgress + unevenPace * middleWeight);
    }

    private static List<Vector3[]> BuildFenceStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        float left = -BackgroundSize.x * 0.5f + 0.45f;
        float right = BackgroundSize.x * 0.5f - 0.45f;
        float bottom = -BackgroundSize.y * 0.5f + 0.45f;
        float top = BackgroundSize.y * 0.5f - 0.45f;
        const float railGap = 0.36f;
        const float postLength = 0.95f;
        const float postStep = 2f;

        strokes.Add(Points(left, top, right, top));
        strokes.Add(Points(left, top - railGap, right, top - railGap));
        strokes.Add(Points(left, bottom, right, bottom));
        strokes.Add(Points(left, bottom + railGap, right, bottom + railGap));
        strokes.Add(Points(left, bottom, left, top));
        strokes.Add(Points(left + railGap, bottom, left + railGap, top));
        strokes.Add(Points(right, bottom, right, top));
        strokes.Add(Points(right - railGap, bottom, right - railGap, top));

        for (float x = left; x <= right + 0.01f; x += postStep)
        {
            strokes.Add(Points(x, top + 0.22f, x, top - postLength));
            strokes.Add(Points(x, bottom - 0.22f, x, bottom + postLength));
        }

        for (float y = bottom; y <= top + 0.01f; y += postStep)
        {
            strokes.Add(Points(left - 0.22f, y, left + postLength, y));
            strokes.Add(Points(right + 0.22f, y, right - postLength, y));
        }

        return strokes;
    }

    private static List<Vector3[]> BuildForestEntranceStrokes()
    {
        List<Vector3[]> strokes = new List<Vector3[]>();

        strokes.Add(Points(-2.85f, -1.2f, -2.55f, -0.2f, -2.72f, 0.72f, -2.18f, 1.45f, -1.18f, 1.82f, -0.25f, 1.92f, 0.62f, 1.78f, 1.52f, 1.42f, 2.18f, 0.72f, 2.48f, -0.22f, 2.75f, -1.2f));
        strokes.Add(Points(-2.1f, -1.25f, -2.02f, -0.38f, -1.82f, 0.42f, -1.42f, 1.08f, -0.72f, 1.36f, 0f, 1.4f, 0.72f, 1.32f, 1.36f, 1.02f, 1.78f, 0.42f, 2.02f, -0.38f, 2.1f, -1.25f));
        strokes.Add(Points(-0.92f, -1.38f, -0.66f, -0.72f, -0.42f, -0.12f, -0.28f, 0.54f));
        strokes.Add(Points(0.92f, -1.38f, 0.66f, -0.72f, 0.42f, -0.12f, 0.28f, 0.54f));
        strokes.Add(Points(-1.45f, -1.3f, -1.16f, -0.52f, -0.88f, 0.18f));
        strokes.Add(Points(1.45f, -1.3f, 1.16f, -0.52f, 0.88f, 0.18f));
        strokes.Add(Points(-2.98f, -1.42f, -2.1f, -1.32f, -1.28f, -1.42f, -0.52f, -1.28f, 0.25f, -1.42f, 1.06f, -1.3f, 1.86f, -1.42f, 2.88f, -1.34f));
        strokes.Add(Points(-0.82f, -1.48f, -0.44f, -0.86f, -0.22f, -0.28f, -0.18f, 0.16f));
        strokes.Add(Points(0.82f, -1.48f, 0.44f, -0.86f, 0.22f, -0.28f, 0.18f, 0.16f));
        strokes.Add(Points(-2.42f, 0.12f, -2.08f, 0.5f, -2.42f, 0.86f, -2.02f, 1.18f));
        strokes.Add(Points(2.42f, 0.12f, 2.08f, 0.5f, 2.42f, 0.86f, 2.02f, 1.18f));
        strokes.Add(Points(-1.68f, 1.06f, -1.18f, 1.34f, -0.7f, 1.18f));
        strokes.Add(Points(1.68f, 1.06f, 1.18f, 1.34f, 0.7f, 1.18f));
        strokes.Add(Points(-0.5f, 1.38f, -0.08f, 1.58f, 0.42f, 1.36f));

        return strokes;
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
