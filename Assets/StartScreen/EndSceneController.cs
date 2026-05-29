using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneController : MonoBehaviour
{
    private const string StartSceneName = "StartScene";
    private const float EndingHoldDuration = 4.0f;
    private const float CutsceneFadeDuration = 0.85f;
    private const float EndingFadeInDuration = 0.85f;
    private const float BlackFadeDuration = 1.35f;
    private const float SketchbookCloseupStartScale = 0.82f;
    private const float PlayerSleepScale = 1.18f;
    private const float PlayerSitScale = 1.12f;
    private const float PlayerStandingScale = 1.06f;
    private static readonly Vector2 PlayerSleepPosition = new Vector2(-660f, 118f);
    private static readonly Vector2 PlayerSitPosition = new Vector2(-620f, 108f);
    private static readonly Vector2 PlayerStandPosition = new Vector2(-510f, 92f);

    [SerializeField] private Sprite playerIdleSprite;
    [SerializeField] private Sprite playerSleepSprite;
    [SerializeField] private Sprite playerBackSprite;
    [SerializeField] private Sprite[] playerWalkUpSprites;
    [SerializeField] private Sprite[] playerWalkRightSprites;
    [SerializeField] private Sprite[] playerWalkDownSprites;
    [SerializeField] private Sprite woodhouseInteriorSprite;
    [SerializeField] private Sprite sketchbookSprite;

    private CanvasGroup cutsceneGroup;
    private CanvasGroup endingGroup;
    private CanvasGroup blackOverlayGroup;
    private CanvasGroup doorLightGroup;
    private CanvasGroup realWorldLightGroup;
    private CanvasGroup sketchbookGroup;
    private RectTransform blanketCover;
    private RectTransform playerShadow;
    private RectTransform playerRoot;
    private RectTransform doorPanel;
    private RectTransform sketchbookRoot;
    private RectTransform creditsRoot;
    private Image playerImage;
    private Image doorPanelImage;
    private Vector2 doorPanelClosedPosition;
    private Vector2 creditsStartPosition;

    private void Awake()
    {
        EnsureCamera();
        BuildScreen();
    }

    private void Start()
    {
        StartCoroutine(PlayEndingSequence());
    }

    private void Update()
    {
        if (creditsRoot == null || endingGroup == null || endingGroup.alpha <= 0.01f)
        {
            return;
        }

        float drift = Mathf.Sin(Time.time * 0.45f) * 4f;
        creditsRoot.anchoredPosition = creditsStartPosition + new Vector2(0f, drift);
    }

    private void EnsureCamera()
    {
        if (Camera.main != null)
        {
            return;
        }

        GameObject cameraObject = new GameObject("End Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.018f, 0.02f, 0.024f, 1f);
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.nearClipPlane = -10f;
        camera.farClipPlane = 100f;
    }

    private void BuildScreen()
    {
        GameObject canvasObject = new GameObject("End Scene Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        BuildCutscene(canvasRect);
        BuildEnding(canvasRect);

        RectTransform blackOverlay = CreateImage("EndingBlackFadeOverlay", canvasRect, Color.black);
        Stretch(blackOverlay);
        blackOverlay.SetAsLastSibling();
        blackOverlayGroup = blackOverlay.gameObject.AddComponent<CanvasGroup>();
        blackOverlayGroup.alpha = 1f;
    }

    private void BuildCutscene(RectTransform canvasRect)
    {
        cutsceneGroup = CreateGroup("EndingCutscene", canvasRect, 1f);
        RectTransform root = cutsceneGroup.GetComponent<RectTransform>();

        BuildWoodhouseRoom(root);
        bool usesWoodhouseInteriorImage = woodhouseInteriorSprite != null;

        realWorldLightGroup = CreateGroup("RealWorldLight", root, 0f);
        RectTransform realWorldRoot = realWorldLightGroup.GetComponent<RectTransform>();
        RectTransform realWorldWash = CreateImage("RealWorldWash", realWorldRoot, new Color(1f, 0.92f, 0.58f, 0.76f));
        Stretch(realWorldWash);
        CreateCircle("RealWorldGlow", realWorldRoot, new Vector2(0f, -455f), new Vector2(760f, 460f), new Color(1f, 0.97f, 0.76f, 0.68f));

        doorLightGroup = CreateGroup("BedroomDoorLight", root, 0f);
        RectTransform doorLightRoot = doorLightGroup.GetComponent<RectTransform>();
        RectTransform lightColumn = CreateImage("BedroomDoorLightColumn", doorLightRoot, new Color(1f, 0.88f, 0.48f, 0.48f));
        SetRect(lightColumn, new Vector2(0.5f, 0.5f), new Vector2(0f, -438f), new Vector2(280f, 190f));
        CreateCircle("BedroomDoorLightGlow", doorLightRoot, new Vector2(0f, -470f), new Vector2(440f, 220f), new Color(1f, 0.86f, 0.42f, 0.28f));

        BuildDoor(root);
        if (!usesWoodhouseInteriorImage)
        {
            BuildBed(root);
            BuildTable(root);
        }
        BuildPlayer(root);
        if (blanketCover != null)
        {
            blanketCover.SetAsLastSibling();
        }
        BuildTableSketchbook(root);
        BuildSketchbook(root);
    }

    private void BuildWoodhouseRoom(RectTransform root)
    {
        if (woodhouseInteriorSprite != null)
        {
            RectTransform room = CreateImage("WoodhouseInteriorImage", root, woodhouseInteriorSprite, Color.white);
            Stretch(room);

            Image roomImage = room.GetComponent<Image>();
            roomImage.preserveAspect = true;

            RectTransform shadeImage = CreateImage("WoodhouseInteriorImageShade", root, new Color(0.02f, 0.016f, 0.012f, 0.12f));
            Stretch(shadeImage);
            return;
        }

        RectTransform floor = CreateImage("WoodhouseCutsceneFloor", root, new Color32(116, 68, 31, 255));
        Stretch(floor);

        for (int i = 0; i < 12; i++)
        {
            float y = -430f + i * 78f;
            CreateLine(root, "WoodPlankHorizontal" + i, new Vector2(-875f, y), new Vector2(875f, y + Mathf.Sin(i * 1.7f) * 8f), 4f, new Color(0.09f, 0.045f, 0.02f, 0.24f));
        }

        for (int i = 0; i < 15; i++)
        {
            float x = -820f + i * 118f;
            float offset = i % 2 == 0 ? 34f : -28f;
            CreateLine(root, "WoodPlankSeam" + i, new Vector2(x, -430f), new Vector2(x + offset, 430f), 3f, new Color(0.09f, 0.045f, 0.02f, 0.12f));
        }

        RectTransform topLog = CreateImage("WoodhouseTopLog", root, new Color32(82, 45, 19, 255));
        SetRect(topLog, new Vector2(0.5f, 0.5f), new Vector2(0f, 456f), new Vector2(1740f, 78f));
        RectTransform bottomLog = CreateImage("WoodhouseBottomLog", root, new Color32(82, 45, 19, 255));
        SetRect(bottomLog, new Vector2(0.5f, 0.5f), new Vector2(0f, -506f), new Vector2(1740f, 76f));
        RectTransform leftLog = CreateImage("WoodhouseLeftLog", root, new Color32(77, 42, 19, 255));
        SetRect(leftLog, new Vector2(0.5f, 0.5f), new Vector2(-888f, -18f), new Vector2(70f, 910f));
        RectTransform rightLog = CreateImage("WoodhouseRightLog", root, new Color32(77, 42, 19, 255));
        SetRect(rightLog, new Vector2(0.5f, 0.5f), new Vector2(888f, -18f), new Vector2(70f, 910f));

        CreateCircle("TopWindowGlow", root, new Vector2(0f, 356f), new Vector2(360f, 180f), new Color(1f, 0.78f, 0.36f, 0.16f));
        RectTransform topWindow = CreateImage("TopWindow", root, new Color32(255, 213, 116, 210));
        SetRect(topWindow, new Vector2(0.5f, 0.5f), new Vector2(0f, 398f), new Vector2(118f, 56f));
        CreateLine(root, "TopWindowCrossA", new Vector2(-58f, 398f), new Vector2(58f, 398f), 4f, new Color(0.18f, 0.1f, 0.04f, 0.5f));
        CreateLine(root, "TopWindowCrossB", new Vector2(0f, 371f), new Vector2(0f, 425f), 4f, new Color(0.18f, 0.1f, 0.04f, 0.5f));

        RectTransform shade = CreateImage("WoodhouseInteriorShade", root, new Color(0.04f, 0.025f, 0.015f, 0.24f));
        Stretch(shade);
    }

    private void BuildBed(RectTransform root)
    {
        RectTransform bedFrame = CreateImage("CutsceneBedFrame", root, new Color32(102, 59, 26, 255));
        SetRect(bedFrame, new Vector2(0.5f, 0.5f), new Vector2(-660f, 118f), new Vector2(240f, 370f));

        RectTransform mattress = CreateImage("CutsceneBedMattress", root, new Color32(238, 232, 214, 255));
        SetRect(mattress, new Vector2(0.5f, 0.5f), new Vector2(-660f, 108f), new Vector2(198f, 300f));

        RectTransform blanket = CreateImage("CutsceneBedBlanket", root, new Color32(154, 100, 49, 255));
        SetRect(blanket, new Vector2(0.5f, 0.5f), new Vector2(-660f, 22f), new Vector2(188f, 194f));

        RectTransform pillow = CreateImage("CutsceneBedPillow", root, new Color32(247, 244, 232, 255));
        SetRect(pillow, new Vector2(0.5f, 0.5f), new Vector2(-660f, 242f), new Vector2(144f, 70f));

        CreateCircle("BedPostTopLeft", root, new Vector2(-780f, 300f), new Vector2(42f, 42f), new Color32(115, 66, 31, 255));
        CreateCircle("BedPostTopRight", root, new Vector2(-540f, 300f), new Vector2(42f, 42f), new Color32(115, 66, 31, 255));
        CreateCircle("BedPostBottomLeft", root, new Vector2(-780f, -66f), new Vector2(42f, 42f), new Color32(115, 66, 31, 255));
        CreateCircle("BedPostBottomRight", root, new Vector2(-540f, -66f), new Vector2(42f, 42f), new Color32(115, 66, 31, 255));

        CreateLine(root, "BedBlanketFoldA", new Vector2(-750f, 84f), new Vector2(-566f, 80f), 5f, new Color(0.14f, 0.07f, 0.03f, 0.18f));
        CreateLine(root, "BedBlanketFoldB", new Vector2(-750f, -20f), new Vector2(-568f, -28f), 4f, new Color(0.14f, 0.07f, 0.03f, 0.16f));

        blanketCover = CreateImage("CutsceneBlanketCover", root, new Color32(154, 98, 43, 255));
        SetRect(blanketCover, new Vector2(0.5f, 0.5f), new Vector2(-660f, 16f), new Vector2(180f, 170f));
        blanketCover.SetAsLastSibling();
    }

    private void BuildTable(RectTransform root)
    {
        RectTransform tableShadow = CreateImage("CutsceneTableShadow", root, new Color(0f, 0f, 0f, 0.22f));
        SetRect(tableShadow, new Vector2(0.5f, 0.5f), new Vector2(26f, -102f), new Vector2(300f, 72f));

        RectTransform table = CreateImage("CutsceneCenterTable", root, new Color32(131, 78, 33, 255));
        SetRect(table, new Vector2(0.5f, 0.5f), new Vector2(0f, -48f), new Vector2(300f, 154f));

        CreateLine(root, "TableLineA", new Vector2(-126f, -20f), new Vector2(126f, -20f), 4f, new Color(0.12f, 0.06f, 0.025f, 0.22f));
        CreateLine(root, "TableLineB", new Vector2(-126f, -70f), new Vector2(126f, -70f), 4f, new Color(0.12f, 0.06f, 0.025f, 0.2f));
    }

    private void BuildDoor(RectTransform root)
    {
        doorPanelClosedPosition = new Vector2(0f, -472f);
        doorPanel = CreateImage("CutsceneBottomDoorPanel", root, new Color(0.42f, 0.22f, 0.09f, 0.82f));
        doorPanel.anchorMin = new Vector2(0.5f, 0.5f);
        doorPanel.anchorMax = new Vector2(0.5f, 0.5f);
        doorPanel.pivot = new Vector2(0.5f, 0.5f);
        doorPanel.anchoredPosition = doorPanelClosedPosition;
        doorPanel.sizeDelta = new Vector2(178f, 78f);
        doorPanelImage = doorPanel.GetComponent<Image>();

        RectTransform doorLineA = CreateLine(root, "CutsceneBottomDoorLineA", new Vector2(-82f, -452f), new Vector2(82f, -452f), 5f, new Color(0.09f, 0.045f, 0.02f, 0.35f));
        doorLineA.SetAsLastSibling();
        RectTransform doorLineB = CreateLine(root, "CutsceneBottomDoorLineB", new Vector2(-70f, -488f), new Vector2(72f, -488f), 4f, new Color(0.09f, 0.045f, 0.02f, 0.28f));
        doorLineB.SetAsLastSibling();
    }

    private void BuildPlayer(RectTransform root)
    {
        playerRoot = CreateRect("CutscenePlayer", root, PlayerSleepPosition, new Vector2(170f, 240f));
        playerRoot.localRotation = Quaternion.identity;
        playerRoot.localScale = Vector3.one * PlayerSleepScale;

        playerShadow = CreateCircle("PlayerShadow", playerRoot, new Vector2(2f, -88f), new Vector2(116f, 28f), new Color(0f, 0f, 0f, 0.16f));
        playerShadow.SetAsFirstSibling();
        playerShadow.gameObject.SetActive(false);

        Sprite initialSprite = playerSleepSprite != null ? playerSleepSprite : playerIdleSprite;
        if (initialSprite != null)
        {
            RectTransform spriteRect = CreateImage("PlayerSprite", playerRoot, initialSprite, Color.white);
            SetRect(spriteRect, new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(142f, 196f));
            playerImage = spriteRect.GetComponent<Image>();
            playerImage.preserveAspect = true;
        }
        else
        {
            CreateFallbackPlayer(root);
        }
    }

    private void BuildTableSketchbook(RectTransform root)
    {
        if (sketchbookSprite == null)
        {
            return;
        }

        RectTransform book = CreateImage("TableSketchbookHint", root, sketchbookSprite, new Color(1f, 1f, 1f, 0.92f));
        SetRect(book, new Vector2(0.5f, 0.5f), new Vector2(10f, -46f), new Vector2(118f, 78f));
        book.localRotation = Quaternion.Euler(0f, 0f, -2.5f);

        Image image = book.GetComponent<Image>();
        image.preserveAspect = true;
    }

    private void BuildSketchbook(RectTransform root)
    {
        sketchbookGroup = CreateGroup("CutsceneSketchbookLook", root, 0f);
        RectTransform sketchbookGroupRoot = sketchbookGroup.GetComponent<RectTransform>();

        RectTransform wash = CreateImage("SketchbookLookWash", sketchbookGroupRoot, new Color(0f, 0f, 0f, 0.34f));
        Stretch(wash);

        sketchbookRoot = CreateRect("SketchbookCloseup", sketchbookGroupRoot, new Vector2(0f, -12f), new Vector2(920f, 560f));
        sketchbookRoot.localScale = Vector3.one * SketchbookCloseupStartScale;

        if (sketchbookSprite != null)
        {
            RectTransform book = CreateImage("SketchbookCloseupSprite", sketchbookRoot, sketchbookSprite, Color.white);
            SetRect(book, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(860f, 520f));

            Image bookImage = book.GetComponent<Image>();
            bookImage.preserveAspect = false;
        }
        else
        {
            RectTransform leftPage = CreateImage("SketchbookLeftPage", sketchbookRoot, new Color32(246, 241, 222, 255));
            SetRect(leftPage, new Vector2(0.5f, 0.5f), new Vector2(-176f, 0f), new Vector2(330f, 360f));

            RectTransform rightPage = CreateImage("SketchbookRightPage", sketchbookRoot, new Color32(248, 244, 229, 255));
            SetRect(rightPage, new Vector2(0.5f, 0.5f), new Vector2(176f, 0f), new Vector2(330f, 360f));

            RectTransform spine = CreateImage("SketchbookSpine", sketchbookRoot, new Color32(96, 78, 57, 255));
            SetRect(spine, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(18f, 368f));
        }

        BuildSketchbookMapExcerpt(sketchbookRoot);
    }

    private void BuildSketchbookMapExcerpt(RectTransform parent)
    {
        RectTransform clip = CreateRect("SketchbookMapExcerptClip", parent, new Vector2(-18f, -18f), new Vector2(580f, 300f));
        RectMask2D mask = clip.gameObject.AddComponent<RectMask2D>();
        mask.padding = new Vector4(0f, 0f, 0f, 0f);
        mask.softness = new Vector2Int(10, 10);

        RectTransform mapRoot = CreateRect("SketchbookMapExcerptDrawing", clip, new Vector2(-134f, -6f), new Vector2(760f, 420f));
        SketchbookLineDrawingGraphic mapGraphic = mapRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        mapGraphic.raycastTarget = false;
        mapGraphic.color = new Color(0.08f, 0.07f, 0.055f, 0.64f);
        mapGraphic.LineWidth = 4.7f;
        mapGraphic.RevealProgress = 1f;
        mapGraphic.SetStrokes(BuildEndSceneMapExcerptStrokes());

        CreateLine(parent, "SketchbookMapTornEdgeA", new Vector2(-312f, 138f), new Vector2(232f, 122f), 3f, new Color(0f, 0f, 0f, 0.1f));
        CreateLine(parent, "SketchbookMapTornEdgeB", new Vector2(-318f, -160f), new Vector2(246f, -142f), 3f, new Color(0f, 0f, 0f, 0.08f));
    }

    private static List<Vector2[]> BuildEndSceneMapExcerptStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMapRect(strokes, -28f, 2f, 240f, 150f);
        AddMapRect(strokes, -28f, 2f, 222f, 132f);
        strokes.Add(Points(-40f, 16f, 0f, -4f, 46f, -34f, 94f, -76f, 126f, -106f));
        strokes.Add(Points(-42f, 13f, -92f, -24f, -142f, -62f, -176f, -102f));
        strokes.Add(Points(-30f, 34f, -86f, 72f, -128f, 106f));
        strokes.Add(Points(-12f, 26f, 40f, 50f, 92f, 54f, 142f, 44f));

        AddEndSceneMapHouse(strokes, -126f, 36f, 60f);
        AddEndSceneMapHouse(strokes, 70f, -66f, 52f);
        AddEndSceneMapPond(strokes, 98f, 72f, 1f);
        AddEndSceneMapPine(strokes, -196f, 92f, 1.1f);
        AddEndSceneMapPine(strokes, -190f, -18f, 0.95f);
        AddEndSceneMapPine(strokes, -78f, -104f, 0.9f);
        AddEndSceneMapPine(strokes, 160f, -10f, 1.0f);
        AddEndSceneMapPine(strokes, 184f, 96f, 0.84f);
        AddEndSceneMapGrass(strokes, -168f, 2f, 1f);
        AddEndSceneMapGrass(strokes, -24f, -88f, 0.86f);
        AddEndSceneMapGrass(strokes, 132f, 8f, 0.82f);

        for (float x = -228f; x <= 168f; x += 52f)
        {
            AddMapRect(strokes, x, 151f, 3.5f, 10f);
            AddMapRect(strokes, x, -147f, 3.5f, 10f);
        }

        for (float y = -98f; y <= 100f; y += 46f)
        {
            AddMapRect(strokes, -268f, y, 10f, 3.5f);
            AddMapRect(strokes, 212f, y, 10f, 3.5f);
        }

        return strokes;
    }

    private static void AddEndSceneMapHouse(List<Vector2[]> strokes, float centerX, float centerY, float size)
    {
        float halfWidth = size * 0.45f;
        float bodyBottom = centerY - size * 0.34f;
        float bodyTop = centerY + size * 0.16f;
        float roofTop = centerY + size * 0.58f;

        strokes.Add(Points(centerX - halfWidth, bodyBottom, centerX - halfWidth, bodyTop, centerX + halfWidth, bodyTop, centerX + halfWidth, bodyBottom, centerX - halfWidth, bodyBottom));
        strokes.Add(Points(centerX - halfWidth * 1.12f, bodyTop, centerX, roofTop, centerX + halfWidth * 1.12f, bodyTop));
        strokes.Add(Points(centerX - size * 0.1f, bodyBottom, centerX - size * 0.1f, centerY, centerX + size * 0.1f, centerY, centerX + size * 0.1f, bodyBottom));
        AddMapRect(strokes, centerX - size * 0.27f, centerY + size * 0.02f, size * 0.08f, size * 0.07f);
        AddMapRect(strokes, centerX + size * 0.27f, centerY + size * 0.02f, size * 0.08f, size * 0.07f);
    }

    private static void AddEndSceneMapPond(List<Vector2[]> strokes, float centerX, float centerY, float scale)
    {
        strokes.Add(EllipsePoints(centerX, centerY, 44f * scale, 24f * scale, 32));
        strokes.Add(EllipsePoints(centerX + 3f * scale, centerY + 1f * scale, 34f * scale, 15f * scale, 24));
        strokes.Add(Points(centerX - 26f * scale, centerY + 4f * scale, centerX - 12f * scale, centerY + 8f * scale, centerX + 2f * scale, centerY + 5f * scale));
        strokes.Add(Points(centerX + 8f * scale, centerY - 4f * scale, centerX + 22f * scale, centerY - 1f * scale, centerX + 33f * scale, centerY - 5f * scale));
    }

    private static void AddEndSceneMapPine(List<Vector2[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(LocalPoints(centerX, groundY, scale, 0f, 34f, -10f, 22f, -6f, 22f, -15f, 11f, -8f, 11f, -18f, 0f, 18f, 0f, 8f, 11f, 15f, 11f, 6f, 22f, 10f, 22f, 0f, 34f));
        strokes.Add(LocalPoints(centerX, groundY, scale, -2f, 0f, -2f, -7f, 2f, -7f, 2f, 0f));
        strokes.Add(LocalPoints(centerX, groundY, scale, -8f, 11f, 0f, 16f, 8f, 11f));
    }

    private static void AddEndSceneMapGrass(List<Vector2[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(LocalPoints(centerX, groundY, scale, -12f, 0f, -6f, 12f, -1f, 0f, 5f, 14f, 9f, 0f, 15f, 10f));
    }

    private static void AddMapRect(List<Vector2[]> strokes, float centerX, float centerY, float halfWidth, float halfHeight)
    {
        strokes.Add(Points(
            centerX - halfWidth, centerY - halfHeight,
            centerX - halfWidth, centerY + halfHeight,
            centerX + halfWidth, centerY + halfHeight,
            centerX + halfWidth, centerY - halfHeight,
            centerX - halfWidth, centerY - halfHeight));
    }

    private static Vector2[] LocalPoints(float centerX, float centerY, float scale, params float[] values)
    {
        int pointCount = values.Length / 2;
        Vector2[] points = new Vector2[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector2(
                centerX + values[i * 2] * scale,
                centerY + values[i * 2 + 1] * scale);
        }

        return points;
    }

    private static Vector2[] EllipsePoints(float centerX, float centerY, float radiusX, float radiusY, int segmentCount)
    {
        Vector2[] points = new Vector2[segmentCount + 1];

        for (int i = 0; i <= segmentCount; i++)
        {
            float angle = Mathf.PI * 2f * i / segmentCount;
            points[i] = new Vector2(
                centerX + Mathf.Cos(angle) * radiusX,
                centerY + Mathf.Sin(angle) * radiusY);
        }

        return points;
    }

    private static Vector2[] Points(params float[] values)
    {
        int pointCount = values.Length / 2;
        Vector2[] points = new Vector2[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector2(values[i * 2], values[i * 2 + 1]);
        }

        return points;
    }

    private void BuildEnding(RectTransform canvasRect)
    {
        endingGroup = CreateGroup("EndingTitleScreen", canvasRect, 0f);
        RectTransform root = endingGroup.GetComponent<RectTransform>();

        RectTransform background = CreateImage("EndingBackground", root, new Color(0.018f, 0.02f, 0.024f, 1f));
        Stretch(background);

        CreateText("EndingTitle", root, "THE END", 104, FontStyle.Bold, new Color32(246, 244, 231, 255), new Vector2(0f, 178f), new Vector2(780f, 130f));
        CreateText("EndingGameTitle", root, "PILLAGE", 46, FontStyle.Bold, new Color32(206, 224, 234, 255), new Vector2(0f, 62f), new Vector2(720f, 80f));

        creditsRoot = CreateRect("EndingCredits", root, new Vector2(0f, -126f), new Vector2(940f, 250f));
        creditsStartPosition = creditsRoot.anchoredPosition;

        CreateText("EndingThanks", creditsRoot, "Thank you for playing", 34, FontStyle.Normal, new Color32(232, 229, 214, 255), new Vector2(0f, 78f), new Vector2(900f, 64f));
        CreateText("EndingCreditLineA", creditsRoot, "Game Design / Art / Programming", 28, FontStyle.Normal, new Color32(160, 187, 204, 255), new Vector2(0f, 10f), new Vector2(900f, 54f));
        CreateText("EndingCreditLineB", creditsRoot, "PILLAGE", 32, FontStyle.Bold, new Color32(222, 232, 236, 255), new Vector2(0f, -48f), new Vector2(900f, 58f));
    }

    private IEnumerator PlayEndingSequence()
    {
        if (blackOverlayGroup == null || cutsceneGroup == null || endingGroup == null)
        {
            yield break;
        }

        cutsceneGroup.alpha = 1f;
        endingGroup.alpha = 0f;
        blackOverlayGroup.alpha = 1f;

        yield return FadeCanvasGroup(blackOverlayGroup, 1f, 0f, CutsceneFadeDuration);
        yield return PlayBedroomCutscene();

        yield return FadeCanvasGroup(blackOverlayGroup, 0f, 1f, 0.75f);
        cutsceneGroup.alpha = 0f;
        endingGroup.alpha = 1f;

        yield return FadeCanvasGroup(blackOverlayGroup, 1f, 0f, EndingFadeInDuration);
        yield return new WaitForSeconds(EndingHoldDuration);
        yield return FadeCanvasGroup(blackOverlayGroup, 0f, 1f, BlackFadeDuration);

        FinalGameEndingController.ResetEndingState();
        SceneManager.LoadScene(StartSceneName, LoadSceneMode.Single);
    }

    private IEnumerator PlayBedroomCutscene()
    {
        ResetCutsceneState();

        yield return new WaitForSeconds(0.55f);
        SetPlayerSprite(playerIdleSprite);
        yield return WakeFromBed();
        yield return new WaitForSeconds(0.22f);

        yield return MoveRotatePlayer(PlayerStandPosition, new Vector2(-26f, -418f), 0f, 0f, 1.65f, true);
        yield return OpenDoor(0.68f, 0.48f, false);
        yield return new WaitForSeconds(0.38f);

        SetPlayerSprite(playerIdleSprite);
        yield return MoveRotatePlayer(new Vector2(-26f, -418f), new Vector2(0f, -94f), 0f, 0f, 0.96f, true);
        yield return ShowSketchbookLook();
        yield return CloseDoor(0.45f);
        yield return new WaitForSeconds(0.18f);

        yield return MoveRotatePlayer(new Vector2(0f, -94f), new Vector2(0f, -418f), 0f, 0f, 0.92f, true);
        yield return OpenDoor(0.86f, 1f, true);
        yield return MoveRotatePlayer(new Vector2(0f, -418f), new Vector2(0f, -610f), 0f, 0f, 0.88f, true);
        yield return FadeCanvasGroup(cutsceneGroup, 1f, 0.35f, 0.55f);
    }

    private void ResetCutsceneState()
    {
        if (playerRoot != null)
        {
            playerRoot.anchoredPosition = PlayerSleepPosition;
            playerRoot.localEulerAngles = Vector3.zero;
            playerRoot.localScale = Vector3.one * PlayerSleepScale;
        }

        if (playerShadow != null)
        {
            playerShadow.gameObject.SetActive(false);
        }

        SetPlayerSprite(playerSleepSprite != null ? playerSleepSprite : playerIdleSprite);

        if (blanketCover != null)
        {
            blanketCover.anchoredPosition = new Vector2(-660f, 16f);
            blanketCover.sizeDelta = new Vector2(180f, 170f);
            blanketCover.gameObject.SetActive(true);
            blanketCover.SetAsLastSibling();
        }

        if (doorPanel != null)
        {
            doorPanel.localScale = Vector3.one;
            doorPanel.anchoredPosition = doorPanelClosedPosition;
        }

        if (doorPanelImage != null)
        {
            Color color = doorPanelImage.color;
            color.a = 0.82f;
            doorPanelImage.color = color;
        }

        if (doorLightGroup != null)
        {
            doorLightGroup.alpha = 0f;
        }

        if (realWorldLightGroup != null)
        {
            realWorldLightGroup.alpha = 0f;
        }

        if (sketchbookGroup != null)
        {
            sketchbookGroup.alpha = 0f;
        }

        if (sketchbookRoot != null)
        {
            sketchbookRoot.localScale = Vector3.one * SketchbookCloseupStartScale;
        }
    }

    private IEnumerator ShowSketchbookLook()
    {
        if (sketchbookGroup == null)
        {
            yield break;
        }

        if (sketchbookRoot != null)
        {
            sketchbookRoot.localScale = Vector3.one * SketchbookCloseupStartScale;
        }

        yield return FadeCanvasGroup(sketchbookGroup, 0f, 1f, 0.34f);

        float elapsed = 0f;
        const float duration = 0.72f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
            if (sketchbookRoot != null)
            {
                sketchbookRoot.localScale = Vector3.Lerp(Vector3.one * SketchbookCloseupStartScale, Vector3.one, progress);
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.75f);
        yield return FadeCanvasGroup(sketchbookGroup, 1f, 0f, 0.32f);
    }

    private IEnumerator WakeFromBed()
    {
        if (playerRoot == null)
        {
            yield break;
        }

        const float sitUpDuration = 0.62f;
        float elapsed = 0f;
        Vector2 startPosition = PlayerSleepPosition;
        Vector2 sitPosition = PlayerSitPosition;
        Vector3 startScale = Vector3.one * PlayerSleepScale;
        Vector3 sitScale = Vector3.one * PlayerSitScale;

        while (elapsed < sitUpDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / sitUpDuration));
            playerRoot.anchoredPosition = Vector2.Lerp(startPosition, sitPosition, progress);
            playerRoot.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, -4f, progress));
            playerRoot.localScale = Vector3.Lerp(startScale, sitScale, progress);

            if (blanketCover != null)
            {
                blanketCover.sizeDelta = Vector2.Lerp(new Vector2(180f, 170f), new Vector2(180f, 108f), progress);
                blanketCover.anchoredPosition = Vector2.Lerp(new Vector2(-660f, 16f), new Vector2(-660f, -20f), progress);
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.16f);

        const float stepDownDuration = 0.72f;
        elapsed = 0f;
        Vector2 standPosition = PlayerStandPosition;

        while (elapsed < stepDownDuration)
        {
            elapsed += Time.deltaTime;
            float linearProgress = Mathf.Clamp01(elapsed / stepDownDuration);
            float progress = Mathf.SmoothStep(0f, 1f, linearProgress);
            float bob = Mathf.Sin(linearProgress * Mathf.PI) * 12f;
            playerRoot.anchoredPosition = Vector2.Lerp(sitPosition, standPosition, progress) + new Vector2(0f, bob);
            playerRoot.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(-4f, 0f, progress));
            playerRoot.localScale = Vector3.Lerp(sitScale, Vector3.one * PlayerStandingScale, progress);

            if (playerShadow != null && linearProgress > 0.18f)
            {
                playerShadow.gameObject.SetActive(true);
            }

            if (blanketCover != null && linearProgress > 0.35f)
            {
                blanketCover.gameObject.SetActive(false);
            }

            yield return null;
        }

        playerRoot.anchoredPosition = standPosition;
        playerRoot.localEulerAngles = Vector3.zero;
        playerRoot.localScale = Vector3.one * PlayerStandingScale;
        if (playerShadow != null)
        {
            playerShadow.gameObject.SetActive(true);
        }
        if (blanketCover != null)
        {
            blanketCover.gameObject.SetActive(false);
        }
    }

    private IEnumerator WakePlayer(float duration)
    {
        if (playerRoot == null)
        {
            yield break;
        }

        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 0.92f;
        Vector3 endScale = Vector3.one * 0.99f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
            playerRoot.localScale = Vector3.Lerp(startScale, endScale, progress);
            yield return null;
        }

        playerRoot.localScale = endScale;
    }

    private IEnumerator OpenDoor(float duration, float lightAlpha, bool realWorld)
    {
        if (doorPanel == null)
        {
            yield break;
        }

        CanvasGroup targetLight = realWorld ? realWorldLightGroup : doorLightGroup;
        float elapsed = 0f;
        Vector2 startPosition = doorPanel.anchoredPosition;
        Vector2 endPosition = doorPanelClosedPosition + new Vector2(0f, -86f);
        Color doorStartColor = doorPanelImage != null ? doorPanelImage.color : Color.white;
        Color doorEndColor = doorStartColor;
        doorEndColor.a = 0.18f;
        float startAlpha = targetLight != null ? targetLight.alpha : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
            doorPanel.anchoredPosition = Vector2.Lerp(startPosition, endPosition, progress);
            if (doorPanelImage != null)
            {
                doorPanelImage.color = Color.Lerp(doorStartColor, doorEndColor, progress);
            }
            if (targetLight != null)
            {
                targetLight.alpha = Mathf.Lerp(startAlpha, lightAlpha, progress);
            }

            yield return null;
        }

        doorPanel.anchoredPosition = endPosition;
        if (doorPanelImage != null)
        {
            doorPanelImage.color = doorEndColor;
        }
        if (targetLight != null)
        {
            targetLight.alpha = lightAlpha;
        }
    }

    private IEnumerator CloseDoor(float duration)
    {
        if (doorPanel == null)
        {
            yield break;
        }

        float elapsed = 0f;
        Vector2 startPosition = doorPanel.anchoredPosition;
        Color doorStartColor = doorPanelImage != null ? doorPanelImage.color : Color.white;
        Color doorEndColor = doorStartColor;
        doorEndColor.a = 0.82f;
        float lightStart = doorLightGroup != null ? doorLightGroup.alpha : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
            doorPanel.anchoredPosition = Vector2.Lerp(startPosition, doorPanelClosedPosition, progress);
            if (doorPanelImage != null)
            {
                doorPanelImage.color = Color.Lerp(doorStartColor, doorEndColor, progress);
            }
            if (doorLightGroup != null)
            {
                doorLightGroup.alpha = Mathf.Lerp(lightStart, 0f, progress);
            }

            yield return null;
        }

        doorPanel.anchoredPosition = doorPanelClosedPosition;
        if (doorPanelImage != null)
        {
            doorPanelImage.color = doorEndColor;
        }
        if (doorLightGroup != null)
        {
            doorLightGroup.alpha = 0f;
        }
    }

    private IEnumerator MoveRotatePlayer(Vector2 from, Vector2 to, float startAngle, float endAngle, float duration, bool walkBob)
    {
        if (playerRoot == null)
        {
            yield break;
        }

        Vector2 moveDelta = to - from;
        Sprite[] walkSprites = GetWalkSpritesForDirection(moveDelta);
        Sprite restSprite = GetRestSpriteForDirection(moveDelta);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float linearProgress = Mathf.Clamp01(elapsed / duration);
            float progress = Mathf.SmoothStep(0f, 1f, linearProgress);
            float bob = walkBob ? Mathf.Sin(linearProgress * Mathf.PI * 8f) * 7f : 0f;
            float lean = walkBob ? Mathf.Sin(linearProgress * Mathf.PI * 8f) * 3.2f : 0f;

            if (walkBob)
            {
                UpdateWalkSprite(linearProgress, walkSprites);
            }

            playerRoot.anchoredPosition = Vector2.Lerp(from, to, progress) + new Vector2(0f, bob);
            playerRoot.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(startAngle, endAngle, progress) + lean);
            playerRoot.localScale = Vector3.one * PlayerStandingScale;
            yield return null;
        }

        playerRoot.anchoredPosition = to;
        playerRoot.localEulerAngles = new Vector3(0f, 0f, endAngle);
        playerRoot.localScale = Vector3.one * PlayerStandingScale;
        if (walkBob)
        {
            SetPlayerSprite(restSprite);
        }
    }

    private void SetPlayerSprite(Sprite sprite)
    {
        if (playerImage == null || sprite == null)
        {
            return;
        }

        playerImage.sprite = sprite;
    }

    private void UpdateWalkSprite(float progress, Sprite[] sprites)
    {
        if (sprites == null || sprites.Length == 0)
        {
            return;
        }

        int index = Mathf.Clamp(Mathf.FloorToInt(progress * sprites.Length * 2f), 0, sprites.Length - 1);
        SetPlayerSprite(sprites[index]);
    }

    private Sprite GetWalkRightSprite(int index)
    {
        if (playerWalkRightSprites == null || playerWalkRightSprites.Length == 0)
        {
            return playerIdleSprite;
        }

        return playerWalkRightSprites[Mathf.Clamp(index, 0, playerWalkRightSprites.Length - 1)];
    }

    private Sprite[] GetWalkSpritesForDirection(Vector2 delta)
    {
        if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x) && delta.y < 0f && playerWalkDownSprites != null && playerWalkDownSprites.Length > 0)
        {
            return playerWalkDownSprites;
        }

        if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x) && delta.y > 0f && playerWalkUpSprites != null && playerWalkUpSprites.Length > 0)
        {
            return playerWalkUpSprites;
        }

        return playerWalkRightSprites;
    }

    private Sprite GetRestSpriteForDirection(Vector2 delta)
    {
        if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x) && delta.y > 0f && playerBackSprite != null)
        {
            return playerBackSprite;
        }

        return playerIdleSprite;
    }

    private void CreateFallbackPlayer(RectTransform root)
    {
        RectTransform body = CreateImage("PlayerFallbackBody", playerRoot, new Color32(126, 78, 39, 255));
        SetRect(body, new Vector2(0.5f, 0.5f), new Vector2(0f, -22f), new Vector2(70f, 104f));
        CreateCircle("PlayerFallbackHead", playerRoot, new Vector2(0f, 58f), new Vector2(70f, 70f), new Color32(231, 187, 144, 255));
        CreateCircle("PlayerFallbackHat", playerRoot, new Vector2(0f, 84f), new Vector2(92f, 38f), new Color32(229, 178, 91, 255));
        CreateLine(playerRoot, "PlayerFallbackScarf", new Vector2(-28f, 12f), new Vector2(30f, 10f), 8f, new Color32(218, 91, 30, 255));
    }

    private static IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        if (group == null)
        {
            yield break;
        }

        float elapsed = 0f;
        group.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
            group.alpha = Mathf.Lerp(from, to, progress);
            yield return null;
        }

        group.alpha = to;
    }

    private static CanvasGroup CreateGroup(string name, RectTransform parent, float alpha)
    {
        GameObject groupObject = new GameObject(name);
        RectTransform rectTransform = groupObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        Stretch(rectTransform);

        CanvasGroup group = groupObject.AddComponent<CanvasGroup>();
        group.alpha = alpha;
        group.blocksRaycasts = false;
        group.interactable = false;
        return group;
    }

    private static Text CreateText(string name, RectTransform parent, string value, int fontSize, FontStyle style, Color color, Vector2 position, Vector2 size)
    {
        GameObject textObject = new GameObject(name);
        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        SetRect(rectTransform, new Vector2(0.5f, 0.5f), position, size);

        Text text = textObject.AddComponent<Text>();
        text.text = value;
        text.font = LoadFont();
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;

        return text;
    }

    private static RectTransform CreateImage(string name, RectTransform parent, Color color)
    {
        return CreateImage(name, parent, CreateSolidSprite(16, 16, Color.white), color);
    }

    private static RectTransform CreateCircle(string name, RectTransform parent, Vector2 position, Vector2 size, Color color)
    {
        RectTransform circle = CreateImage(name, parent, CreateCircleSprite(64), color);
        SetRect(circle, new Vector2(0.5f, 0.5f), position, size);
        return circle;
    }

    private static RectTransform CreateImage(string name, RectTransform parent, Sprite sprite, Color color)
    {
        GameObject imageObject = new GameObject(name);
        RectTransform rectTransform = imageObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent, false);

        Image image = imageObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = false;

        return rectTransform;
    }

    private static RectTransform CreateRect(string name, RectTransform parent, Vector2 position, Vector2 size)
    {
        GameObject gameObject = new GameObject(name);
        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        SetRect(rectTransform, new Vector2(0.5f, 0.5f), position, size);
        return rectTransform;
    }

    private static RectTransform CreateLine(RectTransform parent, string name, Vector2 start, Vector2 end, float thickness, Color color)
    {
        Vector2 delta = end - start;
        RectTransform line = CreateImage(name, parent, color);
        line.anchorMin = new Vector2(0.5f, 0.5f);
        line.anchorMax = new Vector2(0.5f, 0.5f);
        line.pivot = new Vector2(0f, 0.5f);
        line.anchoredPosition = start;
        line.sizeDelta = new Vector2(delta.magnitude, thickness);
        line.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        return line;
    }

    private static void SetRect(RectTransform rectTransform, Vector2 anchor, Vector2 position, Vector2 size)
    {
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;
    }

    private static void Stretch(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
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

    private static Sprite CreateCircleSprite(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.48f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01(radius - distance + 1f);
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, true);
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
}
