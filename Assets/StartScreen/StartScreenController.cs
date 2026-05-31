using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "SampleScene";
    [SerializeField] private string titleText = "PILLAGE";
    [SerializeField] private string startButtonText = "GAME START";

    private readonly List<CanvasGroup> decorationGroups = new List<CanvasGroup>();
    private readonly List<WatercolorDrop> watercolorDrops = new List<WatercolorDrop>();
    private Font uiFont;
    private Button startButton;
    private CanvasGroup startButtonGroup;
    private RectTransform canvasRoot;
    private HandwrittenTextGraphic titleGraphic;
    private HandwrittenTextGraphic gameGraphic;
    private HandwrittenTextGraphic startGraphic;
    private RectTransform titleDrawCursor;
    private RectTransform gameDrawCursor;
    private RectTransform startDrawCursor;
    private readonly List<SketchStroke> startArrowStrokes = new List<SketchStroke>();
    private bool isLoading;

    private void Awake()
    {
        BuildScreen();
    }

    public void StartGame()
    {
        if (isLoading)
        {
            return;
        }

        isLoading = true;

        if (startButton != null)
        {
            startButton.interactable = false;
        }

        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        ShowLoadingCover();
        yield return null;
        GameProgress.ResetForNewGame();
        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    private void ShowLoadingCover()
    {
        if (canvasRoot == null)
        {
            return;
        }

        RectTransform cover = CreateImage("FullScreenLoadCover", canvasRoot, CreateSolidSprite(16, 16, Color.black), Color.white);
        Stretch(cover);
        cover.SetAsLastSibling();
    }

    private void BuildScreen()
    {
        EnsureCamera();
        EnsureEventSystem();

        uiFont = LoadFont();

        Canvas canvas = CreateCanvas();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRoot = canvasRect;

        CreateBackground(canvasRect);

        CanvasGroup titleGroup = CreateCanvasGroup("TitleGroup", canvasRect, new Vector2(0.5f, 0.5f), new Vector2(8f, 236f), new Vector2(1510f, 270f));
        titleGraphic = CreateHandwrittenText("Title", titleGroup.GetComponent<RectTransform>(), titleText, 21f, out titleDrawCursor);

        startButtonGroup = CreateCanvasGroup("StartButton", canvasRect, new Vector2(0.79f, 0.285f), Vector2.zero, new Vector2(610f, 310f));
        Image buttonImage = startButtonGroup.gameObject.AddComponent<Image>();
        buttonImage.sprite = CreateSolidSprite(16, 16, new Color(1f, 1f, 1f, 0.002f));

        startButton = startButtonGroup.gameObject.AddComponent<Button>();
        startButton.targetGraphic = buttonImage;
        startButton.onClick.AddListener(StartGame);
        startButton.interactable = false;

        string firstStartWord;
        string secondStartWord;
        SplitStartButtonText(out firstStartWord, out secondStartWord);

        RectTransform startButtonRect = startButtonGroup.GetComponent<RectTransform>();
        RectTransform gameTextRect = CreateRectTransform("GameTextLine", startButtonRect, new Vector2(0.5f, 0.5f), new Vector2(88f, 66f), new Vector2(360f, 112f));
        gameGraphic = CreateHandwrittenText("GameText", gameTextRect, firstStartWord, 10f, out gameDrawCursor);

        RectTransform startTextRect = CreateRectTransform("StartTextLine", startButtonRect, new Vector2(0.5f, 0.5f), new Vector2(136f, -72f), new Vector2(430f, 118f));
        startGraphic = CreateHandwrittenText("StartText", startTextRect, secondStartWord, 10f, out startDrawCursor);

        CreateStartArrow(startButtonRect);

        titleGroup.alpha = 1f;
        startButtonGroup.alpha = 0f;

        StartCoroutine(PlayIntro());
    }

    private void EnsureCamera()
    {
        if (Camera.main != null)
        {
            return;
        }

        GameObject cameraObject = new GameObject("Start Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.94f, 0.93f, 0.89f, 1f);
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.nearClipPlane = -10f;
        camera.farClipPlane = 100f;
    }

    private void EnsureEventSystem()
    {
        if (EventSystem.current != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject("Start Screen Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private void CreateBackground(RectTransform parent)
    {
        RectTransform background = CreateImage("WhitePaperBackground", parent, CreateWornPaperSprite(768, 432), Color.white);
        Stretch(background);

        CreateWearLine(parent, new Vector2(0.16f, 0.78f), new Vector2(-40f, 0f), new Vector2(420f, 2f), -2f, 0.035f);
        CreateWearLine(parent, new Vector2(0.81f, 0.68f), new Vector2(20f, 0f), new Vector2(360f, 2f), 3f, 0.03f);
        CreateWearStain(parent, new Vector2(0.18f, 0.32f), new Vector2(180f, 118f), 0.018f);
        CreateWearStain(parent, new Vector2(0.78f, 0.76f), new Vector2(220f, 145f), 0.015f);
        CreateBackgroundDoodles(parent);
    }

    private void CreateColorWash(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject panelObject = new GameObject(name + "Panel");
        RectTransform panelRect = panelObject.AddComponent<RectTransform>();
        panelRect.SetParent(parent, false);
        panelRect.anchorMin = anchorMin;
        panelRect.anchorMax = anchorMax;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelObject.AddComponent<RectMask2D>();

        CreateWatercolorDrop(panelRect, name + "DropA", color, new Vector2(-90f, 80f), new Vector2(760f, 840f), 0f);
        CreateWatercolorDrop(panelRect, name + "DropB", color, new Vector2(115f, -170f), new Vector2(640f, 620f), 0.16f);
        CreateWatercolorDrop(panelRect, name + "DropC", color, new Vector2(-170f, -260f), new Vector2(520f, 470f), 0.28f);
        CreateWatercolorDrop(panelRect, name + "DropD", color, new Vector2(190f, 250f), new Vector2(540f, 500f), 0.36f);
    }

    private void CreateWatercolorDrop(RectTransform parent, string name, Color color, Vector2 position, Vector2 size, float delay)
    {
        RectTransform dropRect = CreateImage(name, parent, CreateWatercolorBlobSprite(256, color), Color.white);
        SetRect(dropRect, new Vector2(0.5f, 0.5f), position, size);
        dropRect.localScale = Vector3.one * 0.04f;

        CanvasGroup group = dropRect.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;

        watercolorDrops.Add(new WatercolorDrop(dropRect, group, delay));
    }

    private void CreateWearLine(RectTransform parent, Vector2 anchor, Vector2 position, Vector2 size, float rotation, float alpha)
    {
        RectTransform line = CreateImage("PaperCrease", parent, CreateSolidSprite(16, 16, new Color(0f, 0f, 0f, alpha)), Color.white);
        SetRect(line, anchor, position, size);
        line.localRotation = Quaternion.Euler(0f, 0f, rotation);
    }

    private void CreateWearStain(RectTransform parent, Vector2 anchor, Vector2 size, float alpha)
    {
        RectTransform stain = CreateImage("PaperStain", parent, CreateStainSprite(96, new Color(0.38f, 0.31f, 0.18f, alpha)), Color.white);
        SetRect(stain, anchor, Vector2.zero, size);
    }

    private void CreateBackgroundDoodles(RectTransform parent)
    {
        CreateTreeDoodle(parent, "DoodleTreeLeftBottom", new Vector2(0.11f, 0.23f), Vector2.zero, 0.92f, -7f, 0.46f);
        CreateTreeDoodle(parent, "DoodleTreeRightTop", new Vector2(0.91f, 0.72f), Vector2.zero, 0.74f, 8f, 0.4f);
        CreateTreeDoodle(parent, "DoodleTreeLowerMid", new Vector2(0.37f, 0.19f), Vector2.zero, 0.58f, 4f, 0.34f);

        CreateFishDoodle(parent, "DoodleFishLeft", new Vector2(0.22f, 0.46f), Vector2.zero, 0.74f, -8f, 0.42f);
        CreateFishDoodle(parent, "DoodleFishLower", new Vector2(0.53f, 0.18f), Vector2.zero, 0.68f, 11f, 0.38f);
        CreateFishDoodle(parent, "DoodleFishRight", new Vector2(0.9f, 0.48f), Vector2.zero, 0.62f, 5f, 0.36f);
        CreateFishDoodle(parent, "DoodleFishUpperLeft", new Vector2(0.13f, 0.7f), Vector2.zero, 0.52f, -14f, 0.3f);
    }

    private RectTransform CreateDoodleRoot(RectTransform parent, string name, Vector2 anchor, Vector2 position, Vector2 size, float scale, float rotation, float alpha)
    {
        RectTransform root = CreateRectTransform(name, parent, anchor, position, size);
        root.localScale = Vector3.one * scale;
        root.localRotation = Quaternion.Euler(0f, 0f, rotation);

        CanvasGroup group = root.gameObject.AddComponent<CanvasGroup>();
        group.alpha = alpha;
        group.blocksRaycasts = false;
        group.interactable = false;

        return root;
    }

    private void CreateTreeDoodle(RectTransform parent, string name, Vector2 anchor, Vector2 position, float scale, float rotation, float alpha)
    {
        RectTransform root = CreateDoodleRoot(parent, name, anchor, position, new Vector2(260f, 260f), scale, rotation, alpha);

        CreateStaticSketchLine(root, name + "Ground", new Vector2(-72f, -96f), new Vector2(80f, -90f), 8f, 0.75f);
        CreateStaticSketchLine(root, name + "TrunkA", new Vector2(-8f, -96f), new Vector2(5f, -12f), 12f, 0.88f);
        CreateStaticSketchLine(root, name + "TrunkB", new Vector2(15f, -94f), new Vector2(5f, -12f), 8f, 0.72f);
        CreateStaticSketchLine(root, name + "BranchLeft", new Vector2(3f, -42f), new Vector2(-62f, 12f), 7f, 0.8f);
        CreateStaticSketchLine(root, name + "BranchRight", new Vector2(8f, -34f), new Vector2(70f, 18f), 7f, 0.78f);
        CreateStaticSketchLine(root, name + "BranchTop", new Vector2(6f, -12f), new Vector2(14f, 58f), 7f, 0.76f);

        CreateStaticSketchLine(root, name + "LeafA", new Vector2(-86f, 8f), new Vector2(-48f, 74f), 8f, 0.7f);
        CreateStaticSketchLine(root, name + "LeafB", new Vector2(-48f, 74f), new Vector2(-2f, 96f), 8f, 0.72f);
        CreateStaticSketchLine(root, name + "LeafC", new Vector2(-2f, 96f), new Vector2(48f, 72f), 8f, 0.72f);
        CreateStaticSketchLine(root, name + "LeafD", new Vector2(48f, 72f), new Vector2(88f, 8f), 8f, 0.7f);
        CreateStaticSketchLine(root, name + "LeafE", new Vector2(-74f, 18f), new Vector2(-16f, 28f), 6f, 0.6f);
        CreateStaticSketchLine(root, name + "LeafF", new Vector2(-16f, 28f), new Vector2(42f, 16f), 6f, 0.58f);
    }

    private void CreateFishDoodle(RectTransform parent, string name, Vector2 anchor, Vector2 position, float scale, float rotation, float alpha)
    {
        RectTransform root = CreateDoodleRoot(parent, name, anchor, position, new Vector2(270f, 160f), scale, rotation, alpha);

        Vector2 tailBase = new Vector2(-78f, 0f);
        Vector2 tailTop = new Vector2(-126f, 34f);
        Vector2 tailBottom = new Vector2(-126f, -34f);
        Vector2 nose = new Vector2(82f, 0f);
        Vector2 topFront = new Vector2(38f, 38f);
        Vector2 topBack = new Vector2(-34f, 30f);
        Vector2 bottomBack = new Vector2(-34f, -30f);
        Vector2 bottomFront = new Vector2(38f, -38f);

        CreateStaticSketchLine(root, name + "BodyTopA", tailBase, topBack, 7f, 0.86f);
        CreateStaticSketchLine(root, name + "BodyTopB", topBack, topFront, 7f, 0.82f);
        CreateStaticSketchLine(root, name + "BodyTopC", topFront, nose, 7f, 0.86f);
        CreateStaticSketchLine(root, name + "BodyBottomA", nose, bottomFront, 7f, 0.82f);
        CreateStaticSketchLine(root, name + "BodyBottomB", bottomFront, bottomBack, 7f, 0.82f);
        CreateStaticSketchLine(root, name + "BodyBottomC", bottomBack, tailBase, 7f, 0.86f);
        CreateStaticSketchLine(root, name + "TailTop", tailBase, tailTop, 7f, 0.82f);
        CreateStaticSketchLine(root, name + "TailBack", tailTop, tailBottom, 7f, 0.78f);
        CreateStaticSketchLine(root, name + "TailBottom", tailBottom, tailBase, 7f, 0.82f);
        CreateStaticSketchLine(root, name + "FinA", new Vector2(-8f, 0f), new Vector2(-28f, 22f), 5f, 0.7f);
        CreateStaticSketchLine(root, name + "FinB", new Vector2(-28f, 22f), new Vector2(10f, 8f), 5f, 0.7f);
        CreateDoodleDot(root, name + "Eye", new Vector2(52f, 12f), new Vector2(13f, 13f), 0.88f);
    }

    private void CreateStaticSketchLine(RectTransform parent, string name, Vector2 start, Vector2 end, float thickness, float alpha)
    {
        Vector2 delta = end - start;

        if (delta.sqrMagnitude <= 0.01f)
        {
            return;
        }

        RectTransform stroke = CreateImage(name, parent, CreateSketchStrokeSprite(160, 24, new Color(0f, 0f, 0f, alpha)), Color.white);
        stroke.anchorMin = new Vector2(0.5f, 0.5f);
        stroke.anchorMax = new Vector2(0.5f, 0.5f);
        stroke.pivot = new Vector2(0f, 0.5f);
        stroke.anchoredPosition = start;
        stroke.sizeDelta = new Vector2(delta.magnitude, thickness);
        stroke.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);

        Image image = stroke.GetComponent<Image>();
        image.raycastTarget = false;
    }

    private void CreateDoodleDot(RectTransform parent, string name, Vector2 position, Vector2 size, float alpha)
    {
        RectTransform dot = CreateImage(name, parent, CreateStainSprite(32, new Color(0f, 0f, 0f, alpha)), Color.white);
        SetRect(dot, new Vector2(0.5f, 0.5f), position, size);

        Image image = dot.GetComponent<Image>();
        image.raycastTarget = false;
    }

    private RectTransform CreateSketchbook(RectTransform parent)
    {
        RectTransform shadow = CreateImage("SketchbookShadow", parent, CreatePanelSprite(640, 420, new Color(0.25f, 0.2f, 0.12f, 0.24f), new Color(0.25f, 0.2f, 0.12f, 0.24f), 1), Color.white);
        SetRect(shadow, new Vector2(0.5f, 0.5f), new Vector2(18f, -18f), new Vector2(1120f, 680f));

        RectTransform page = CreateImage("Sketchbook", parent, CreatePanelSprite(640, 420, new Color(0.98f, 0.94f, 0.81f, 1f), new Color(0.28f, 0.2f, 0.1f, 1f), 8), Color.white);
        SetRect(page, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1120f, 680f));

        RectTransform spine = CreateImage("Spine", page, CreateSolidSprite(16, 16, new Color(0.78f, 0.57f, 0.31f, 1f)), Color.white);
        spine.anchorMin = new Vector2(0f, 0f);
        spine.anchorMax = new Vector2(0f, 1f);
        spine.anchoredPosition = new Vector2(58f, 0f);
        spine.sizeDelta = new Vector2(34f, 0f);

        Sprite ringSprite = CreateRingSprite(48, new Color(0.29f, 0.21f, 0.12f, 1f), 5);
        for (int i = 0; i < 7; i++)
        {
            RectTransform ring = CreateImage("BindingRing", page, ringSprite, Color.white);
            SetRect(ring, new Vector2(0f, 0.5f), new Vector2(58f, -240f + i * 80f), new Vector2(38f, 38f));
        }

        CreateSketchLine(page, new Vector2(-315f, 216f), 300f, 0f);
        CreateSketchLine(page, new Vector2(326f, 214f), 245f, -2f);
        CreateSketchLine(page, new Vector2(-340f, -220f), 220f, 2f);
        CreateSketchLine(page, new Vector2(350f, -216f), 260f, 0f);

        return page;
    }

    private void CreateDecorations(RectTransform parent)
    {
        CreateDecoration(parent, "TreeTopLeft", "Outside/tree", new Vector2(0.5f, 0.5f), new Vector2(-405f, 178f), new Vector2(130f, 130f), -8f);
        CreateDecoration(parent, "TreeTopRight", "Outside/tree", new Vector2(0.5f, 0.5f), new Vector2(410f, 172f), new Vector2(124f, 124f), 7f);
        CreateDecoration(parent, "TreeBottomLeft", "Outside/tree", new Vector2(0.5f, 0.5f), new Vector2(-458f, -135f), new Vector2(112f, 112f), 5f);
        CreateDecoration(parent, "TreeBottomRight", "Outside/tree", new Vector2(0.5f, 0.5f), new Vector2(462f, -138f), new Vector2(112f, 112f), -5f);

        CreateDecoration(parent, "GrassLeft", "Outside/grass", new Vector2(0.5f, 0.5f), new Vector2(-245f, -242f), new Vector2(118f, 92f), -7f);
        CreateDecoration(parent, "GrassMidLeft", "Outside/grass", new Vector2(0.5f, 0.5f), new Vector2(-92f, -258f), new Vector2(104f, 82f), 4f);
        CreateDecoration(parent, "GrassMidRight", "Outside/grass", new Vector2(0.5f, 0.5f), new Vector2(122f, -258f), new Vector2(104f, 82f), -3f);
        CreateDecoration(parent, "GrassRight", "Outside/grass", new Vector2(0.5f, 0.5f), new Vector2(278f, -242f), new Vector2(118f, 92f), 7f);
        CreateDecoration(parent, "GrassFarLeft", "Outside/grass_thick", new Vector2(0.5f, 0.5f), new Vector2(-500f, 18f), new Vector2(86f, 70f), -12f);
        CreateDecoration(parent, "GrassFarRight", "Outside/grass_thick", new Vector2(0.5f, 0.5f), new Vector2(508f, 18f), new Vector2(86f, 70f), 12f);
    }

    private void CreateDecoration(RectTransform parent, string name, string resourcePath, Vector2 anchor, Vector2 position, Vector2 size, float rotation)
    {
        Sprite sprite = LoadSprite(resourcePath);
        if (sprite == null)
        {
            return;
        }

        RectTransform rectTransform = CreateImage(name, parent, sprite, Color.white);
        SetRect(rectTransform, anchor, position, size);
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotation);

        Image image = rectTransform.GetComponent<Image>();
        image.preserveAspect = true;

        CanvasGroup group = rectTransform.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        rectTransform.localScale = Vector3.one * 0.88f;
        decorationGroups.Add(group);
    }

    private void CreateSketchLine(RectTransform parent, Vector2 position, float width, float rotation)
    {
        RectTransform line = CreateImage("SketchLine", parent, CreateSolidSprite(16, 16, new Color(0.35f, 0.24f, 0.12f, 0.28f)), Color.white);
        SetRect(line, new Vector2(0.5f, 0.5f), position, new Vector2(width, 3f));
        line.localRotation = Quaternion.Euler(0f, 0f, rotation);
    }

    private IEnumerator PlayIntro()
    {
        yield return new WaitForSeconds(0.12f);

        yield return DrawHandwrittenGraphic(titleGraphic, titleDrawCursor, 1.25f, 20f);
        yield return new WaitForSeconds(0.05f);

        yield return RevealGroup(startButtonGroup, 0f, 0.2f);
        yield return DrawHandwrittenGraphic(gameGraphic, gameDrawCursor, 0.45f, 8f);
        yield return DrawStartArrow(0.12f);
        yield return DrawHandwrittenGraphic(startGraphic, startDrawCursor, 0.55f, 8f);

        if (startButton != null)
        {
            startButton.interactable = true;
        }
    }

    private void SplitStartButtonText(out string firstWord, out string secondWord)
    {
        string value = string.IsNullOrWhiteSpace(startButtonText) ? "GAME START" : startButtonText;
        string[] words = value.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        firstWord = words.Length > 0 ? words[0] : "GAME";
        secondWord = words.Length > 1 ? string.Join(" ", words, 1, words.Length - 1) : "START";
    }

    private void CreateStartArrow(RectTransform parent)
    {
        startArrowStrokes.Clear();

        CreateSketchStroke(parent, "ArrowBody", new Vector2(0.5f, 0.5f), new Vector2(-248f, -4f), new Vector2(190f, 18f), -10f);
        CreateSketchStroke(parent, "ArrowTurn", new Vector2(0.5f, 0.5f), new Vector2(-68f, -36f), new Vector2(92f, 18f), -72f);
        CreateSketchStroke(parent, "ArrowHeadLeft", new Vector2(0.5f, 0.5f), new Vector2(-78f, -116f), new Vector2(58f, 17f), 30f);
        CreateSketchStroke(parent, "ArrowHeadRight", new Vector2(0.5f, 0.5f), new Vector2(-78f, -116f), new Vector2(58f, 17f), -34f);
    }

    private RectTransform CreateSketchStroke(RectTransform parent, string name, Vector2 anchor, Vector2 position, Vector2 targetSize, float rotation)
    {
        RectTransform stroke = CreateImage(name, parent, CreateSketchStrokeSprite(160, 24, new Color(0f, 0f, 0f, 0.94f)), Color.white);
        stroke.anchorMin = anchor;
        stroke.anchorMax = anchor;
        stroke.pivot = new Vector2(0f, 0.5f);
        stroke.anchoredPosition = position;
        stroke.sizeDelta = new Vector2(0f, targetSize.y);
        stroke.localRotation = Quaternion.Euler(0f, 0f, rotation);

        startArrowStrokes.Add(new SketchStroke(stroke, targetSize.x));
        return stroke;
    }

    private IEnumerator DrawStartArrow(float strokeDuration)
    {
        for (int i = 0; i < startArrowStrokes.Count; i++)
        {
            yield return DrawSketchStroke(startArrowStrokes[i].Rect, startArrowStrokes[i].TargetWidth, strokeDuration);
        }
    }

    private IEnumerator DrawSketchStroke(RectTransform stroke, float targetWidth, float duration)
    {
        if (stroke == null)
        {
            yield break;
        }

        Vector2 size = stroke.sizeDelta;
        size.x = 0f;
        stroke.sizeDelta = size;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            size.x = Mathf.Lerp(0f, targetWidth, Mathf.SmoothStep(0f, 1f, progress));
            stroke.sizeDelta = size;
            yield return null;
        }

        size.x = targetWidth;
        stroke.sizeDelta = size;
    }

    private IEnumerator DrawHandwrittenGraphic(HandwrittenTextGraphic graphic, RectTransform cursor, float duration, float cursorHeight)
    {
        if (graphic == null)
        {
            yield break;
        }

        graphic.Reveal = 0f;

        if (cursor != null)
        {
            cursor.gameObject.SetActive(true);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = Mathf.SmoothStep(0f, 1f, progress);
            graphic.Reveal = eased;

            if (cursor != null)
            {
                float roughX = Mathf.Sin(Time.time * 73f) * 2.5f;
                float roughY = Mathf.Sin(Time.time * 41f) * 2.5f;
                float strokeBounce = Mathf.Sin(progress * Mathf.PI * 18f) * cursorHeight * 0.18f;
                cursor.anchoredPosition = graphic.CursorPosition + new Vector2(roughX, strokeBounce + roughY);
                cursor.localRotation = Quaternion.Euler(0f, 0f, -18f + Mathf.Sin(Time.time * 24f) * 5f);
            }

            yield return null;
        }

        graphic.Reveal = 1f;

        if (cursor != null)
        {
            cursor.gameObject.SetActive(false);
        }
    }

    private IEnumerator PaintColorSections()
    {
        const float spreadDuration = 0.9f;
        const float totalDuration = 1.95f;

        for (int i = 0; i < watercolorDrops.Count; i++)
        {
            watercolorDrops[i].Group.alpha = 0f;
            watercolorDrops[i].Rect.localScale = Vector3.one * 0.04f;
        }

        float elapsed = 0f;
        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;

            for (int i = 0; i < watercolorDrops.Count; i++)
            {
                WatercolorDrop drop = watercolorDrops[i];
                float progress = Mathf.Clamp01((elapsed - drop.Delay) / spreadDuration);

                if (progress <= 0f)
                {
                    continue;
                }

                float spread = 1f - Mathf.Pow(1f - progress, 3f);
                float edgePulse = Mathf.Sin((elapsed + drop.Delay) * 16f) * 0.025f;
                float scale = Mathf.Lerp(0.04f, 1.02f + edgePulse, spread);
                drop.Rect.localScale = new Vector3(scale, scale, 1f);
                drop.Group.alpha = Mathf.SmoothStep(0f, 1f, progress);
            }

            yield return null;
        }

        for (int i = 0; i < watercolorDrops.Count; i++)
        {
            watercolorDrops[i].Rect.localScale = Vector3.one;
            watercolorDrops[i].Group.alpha = 1f;
        }
    }

    private IEnumerator DrawTextReveal(RectTransform clipRect, RectTransform cursor, float targetWidth, float duration)
    {
        if (clipRect == null)
        {
            yield break;
        }

        if (cursor != null)
        {
            cursor.gameObject.SetActive(true);
        }

        float startX = clipRect.anchoredPosition.x;
        Vector2 size = clipRect.sizeDelta;
        size.x = 0f;
        clipRect.sizeDelta = size;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = Mathf.SmoothStep(0f, 1f, progress);
            size.x = Mathf.Lerp(0f, targetWidth, eased);
            clipRect.sizeDelta = size;

            if (cursor != null)
            {
                float jitterY = Mathf.Sin(elapsed * 37f) * 4f;
                float jitterX = Mathf.Sin(elapsed * 61f) * 2f;
                cursor.anchoredPosition = new Vector2(startX + size.x + jitterX, jitterY);
            }

            yield return null;
        }

        size.x = targetWidth;
        clipRect.sizeDelta = size;

        if (cursor != null)
        {
            cursor.gameObject.SetActive(false);
        }
    }

    private IEnumerator DrawTextAsWriting(RectTransform clipRect, RectTransform cursor, float targetWidth, string value, float letterDuration, float letterPause, float cursorHeight)
    {
        if (clipRect == null)
        {
            yield break;
        }

        if (cursor != null)
        {
            cursor.gameObject.SetActive(true);
        }

        float startX = clipRect.anchoredPosition.x;
        Vector2 size = clipRect.sizeDelta;
        size.x = 0f;
        clipRect.sizeDelta = size;

        int visibleSteps = Mathf.Max(1, value.Length);
        float currentWidth = 0f;

        for (int i = 0; i < visibleSteps; i++)
        {
            char character = value[i];
            float nextWidth = targetWidth * ((i + 1f) / visibleSteps);
            float duration = character == ' ' ? letterDuration * 0.45f : letterDuration;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                float eased = Mathf.SmoothStep(0f, 1f, progress);
                size.x = Mathf.Lerp(currentWidth, nextWidth, eased);
                clipRect.sizeDelta = size;

                if (cursor != null)
                {
                    float stroke = Mathf.Sin(progress * Mathf.PI * 2f) * cursorHeight;
                    float roughX = Mathf.Sin((Time.time + i) * 73f) * 2.5f;
                    float roughY = Mathf.Sin((Time.time + i) * 41f) * 2.5f;
                    cursor.anchoredPosition = new Vector2(startX + size.x + roughX, stroke + roughY);
                    cursor.localRotation = Quaternion.Euler(0f, 0f, -18f + Mathf.Sin(Time.time * 24f) * 5f);
                }

                yield return null;
            }

            currentWidth = nextWidth;
            size.x = currentWidth;
            clipRect.sizeDelta = size;

            if (letterPause > 0f && character != ' ')
            {
                yield return new WaitForSeconds(letterPause);
            }
        }

        size.x = targetWidth;
        clipRect.sizeDelta = size;

        if (cursor != null)
        {
            cursor.gameObject.SetActive(false);
        }
    }

    private IEnumerator DrawUnderline(RectTransform underline, float targetWidth, float duration)
    {
        if (underline == null)
        {
            yield break;
        }

        Vector2 size = underline.sizeDelta;
        size.x = 0f;
        underline.sizeDelta = size;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            size.x = Mathf.Lerp(0f, targetWidth, Mathf.SmoothStep(0f, 1f, progress));
            underline.sizeDelta = size;
            yield return null;
        }

        size.x = targetWidth;
        underline.sizeDelta = size;
    }

    private IEnumerator DrawText(Text label, string value, float letterDelay)
    {
        label.text = string.Empty;

        for (int i = 0; i < value.Length; i++)
        {
            label.text = value.Substring(0, i + 1);
            yield return new WaitForSeconds(letterDelay);
        }
    }

    private IEnumerator RevealGroup(CanvasGroup group, float delay, float duration)
    {
        if (group == null)
        {
            yield break;
        }

        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        Transform target = group.transform;
        Vector3 startScale = target.localScale;
        Vector3 endScale = Vector3.one;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - progress, 3f);
            group.alpha = eased;
            target.localScale = Vector3.LerpUnclamped(startScale, endScale, eased);
            yield return null;
        }

        group.alpha = 1f;
        target.localScale = endScale;
    }

    private CanvasGroup CreateCanvasGroup(string name, RectTransform parent, Vector2 anchor, Vector2 position, Vector2 size)
    {
        GameObject gameObject = new GameObject(name);
        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        SetRect(rectTransform, anchor, position, size);
        return gameObject.AddComponent<CanvasGroup>();
    }

    private RectTransform CreateRectTransform(string name, RectTransform parent, Vector2 anchor, Vector2 position, Vector2 size)
    {
        GameObject gameObject = new GameObject(name);
        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        SetRect(rectTransform, anchor, position, size);
        return rectTransform;
    }

    private Text CreateText(string name, Transform parent, string text, int fontSize, FontStyle fontStyle, Color color, TextAnchor alignment)
    {
        GameObject gameObject = new GameObject(name);
        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        Stretch(rectTransform);

        Text label = gameObject.AddComponent<Text>();
        label.text = text;
        label.font = uiFont;
        label.fontSize = fontSize;
        label.fontStyle = fontStyle;
        label.color = color;
        label.alignment = alignment;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        label.raycastTarget = false;

        return label;
    }

    private Text CreateDrawnText(
        string name,
        RectTransform parent,
        string text,
        int fontSize,
        FontStyle fontStyle,
        Color color,
        out RectTransform clipRect,
        out RectTransform cursor,
        out float targetWidth)
    {
        targetWidth = Mathf.Min(parent.sizeDelta.x, EstimateTextWidth(text, fontSize));
        float startX = -targetWidth * 0.5f;

        GameObject clipObject = new GameObject(name + "Clip");
        clipRect = clipObject.AddComponent<RectTransform>();
        clipRect.SetParent(parent, false);
        clipRect.anchorMin = new Vector2(0.5f, 0.5f);
        clipRect.anchorMax = new Vector2(0.5f, 0.5f);
        clipRect.pivot = new Vector2(0f, 0.5f);
        clipRect.anchoredPosition = new Vector2(startX, 0f);
        clipRect.sizeDelta = new Vector2(0f, parent.sizeDelta.y);
        clipObject.AddComponent<RectMask2D>();

        GameObject textObject = new GameObject(name);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.SetParent(clipRect, false);
        textRect.anchorMin = new Vector2(0f, 0.5f);
        textRect.anchorMax = new Vector2(0f, 0.5f);
        textRect.pivot = new Vector2(0f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(targetWidth, parent.sizeDelta.y);

        Text label = textObject.AddComponent<Text>();
        label.text = text;
        label.font = uiFont;
        label.fontSize = fontSize;
        label.fontStyle = fontStyle;
        label.color = color;
        label.alignment = TextAnchor.MiddleCenter;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        label.raycastTarget = false;

        cursor = CreateImage(name + "DrawCursor", parent, CreateDrawCursorSprite(36, 34), Color.white);
        cursor.anchorMin = new Vector2(0.5f, 0.5f);
        cursor.anchorMax = new Vector2(0.5f, 0.5f);
        cursor.pivot = new Vector2(0.5f, 0.5f);
        cursor.anchoredPosition = new Vector2(startX, 0f);
        cursor.sizeDelta = name == "Title" ? new Vector2(44f, 42f) : new Vector2(24f, 24f);
        cursor.localRotation = Quaternion.Euler(0f, 0f, -18f);
        cursor.gameObject.SetActive(false);

        return label;
    }

    private HandwrittenTextGraphic CreateHandwrittenText(string name, RectTransform parent, string value, float strokeWidth, out RectTransform cursor)
    {
        GameObject textObject = new GameObject(name + "Handwritten");
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.SetParent(parent, false);
        Stretch(textRect);

        HandwrittenTextGraphic graphic = textObject.AddComponent<HandwrittenTextGraphic>();
        graphic.Text = value;
        graphic.StrokeWidth = strokeWidth;
        graphic.LetterSpacing = name == "Title" ? 0.16f : 0.14f;
        graphic.color = Color.black;
        graphic.Reveal = 0f;
        graphic.raycastTarget = false;

        cursor = CreateImage(name + "DrawCursor", parent, CreateDrawCursorSprite(36, 34), Color.white);
        cursor.anchorMin = new Vector2(0.5f, 0.5f);
        cursor.anchorMax = new Vector2(0.5f, 0.5f);
        cursor.pivot = new Vector2(0.5f, 0.5f);
        cursor.anchoredPosition = Vector2.zero;
        cursor.sizeDelta = name == "Title" ? new Vector2(46f, 44f) : new Vector2(24f, 24f);
        cursor.localRotation = Quaternion.Euler(0f, 0f, -18f);
        cursor.gameObject.SetActive(false);

        return graphic;
    }

    private float EstimateTextWidth(string value, int fontSize)
    {
        float width = fontSize * 0.16f;

        for (int i = 0; i < value.Length; i++)
        {
            char character = value[i];

            if (character == ' ')
            {
                width += fontSize * 0.42f;
            }
            else if (character == 'I' || character == 'l' || character == 'i')
            {
                width += fontSize * 0.36f;
            }
            else if (character == 'M' || character == 'W')
            {
                width += fontSize * 0.95f;
            }
            else if (character == 'L')
            {
                width += fontSize * 0.62f;
            }
            else
            {
                width += fontSize * 0.73f;
            }
        }

        return width;
    }

    private RectTransform CreateImage(string name, RectTransform parent, Sprite sprite, Color color)
    {
        GameObject gameObject = new GameObject(name);
        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent, false);

        Image image = gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        return rectTransform;
    }

    private void SetRect(RectTransform rectTransform, Vector2 anchor, Vector2 position, Vector2 size)
    {
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;
    }

    private void Stretch(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    private Font LoadFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        return font;
    }

    private Sprite LoadSprite(string resourcePath)
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

        return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
    }

    private Sprite CreateSolidSprite(int width, int height, Color color)
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

    private Sprite CreateWornPaperSprite(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int hash = Mathf.Abs((x * 73856093) ^ (y * 19349663) ^ 83492791);
                float grain = (hash % 1000) / 1000f;
                float fibers = Mathf.Sin((x * 0.055f) + Mathf.Sin(y * 0.021f) * 1.8f) * 0.012f;
                float edgeWear = Mathf.Max(
                    Mathf.InverseLerp(width * 0.28f, 0f, x),
                    Mathf.InverseLerp(width * 0.72f, width, x),
                    Mathf.InverseLerp(height * 0.26f, 0f, y),
                    Mathf.InverseLerp(height * 0.74f, height, y)) * 0.035f;
                float speck = grain > 0.986f ? 0.12f : 0f;
                float value = 0.955f + fibers - grain * 0.035f - edgeWear - speck;
                pixels[y * width + x] = new Color(value, value * 0.992f, value * 0.956f, 1f);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, true);
        return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
    }

    private Sprite CreateStainSprite(int size, Color color)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center) / (size * 0.5f);
                int hash = Mathf.Abs((x * 92837111) ^ (y * 689287499));
                float noise = (hash % 1000) / 1000f;
                float alpha = Mathf.Clamp01(1f - distance) * color.a * (0.6f + noise * 0.45f);
                pixels[y * size + x] = new Color(color.r, color.g, color.b, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, true);
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    private Sprite CreateCrayonWashSprite(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int hash = Mathf.Abs((x * 1103515245) ^ (y * 12345) ^ 0x2f1a9);
                float grain = (hash % 1000) / 1000f;
                float stroke = Mathf.Sin(y * 0.12f + Mathf.Sin(x * 0.018f) * 2.5f) * 0.5f + 0.5f;
                float brokenWax = grain > 0.88f ? 0.22f : 1f;
                float alpha = color.a * Mathf.Lerp(0.62f, 1.1f, stroke) * brokenWax;
                float shade = 0.92f + grain * 0.16f;
                pixels[y * width + x] = new Color(color.r * shade, color.g * shade, color.b * shade, Mathf.Clamp01(alpha));
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, true);
        return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
    }

    private Sprite CreateWatercolorBlobSprite(int size, Color color)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 point = new Vector2(x, y);
                float distance = Vector2.Distance(point, center) / (size * 0.5f);
                float angle = Mathf.Atan2(point.y - center.y, point.x - center.x);
                float wobble = Mathf.Sin(angle * 5.3f + 0.4f) * 0.055f + Mathf.Sin(angle * 9.7f) * 0.035f;
                float edge = 1f - Mathf.SmoothStep(0.5f + wobble, 1f + wobble, distance);
                int hash = Mathf.Abs((x * 374761393) ^ (y * 668265263) ^ 0x4f1bbc);
                float grain = (hash % 1000) / 1000f;
                float bloom = Mathf.Clamp01(edge) * (0.62f + grain * 0.38f);
                float paperBreak = grain > 0.94f ? 0.55f : 1f;
                float ring = Mathf.Exp(-Mathf.Pow((distance - 0.72f - wobble) * 7.5f, 2f)) * 0.22f;
                float alpha = Mathf.Clamp01((bloom + ring) * color.a * paperBreak);
                float shade = 0.88f + grain * 0.22f;
                pixels[y * size + x] = new Color(color.r * shade, color.g * shade, color.b * shade, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, true);
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    private Sprite CreateDrawCursorSprite(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        Vector2 start = new Vector2(width * 0.16f, height * 0.72f);
        Vector2 end = new Vector2(width * 0.84f, height * 0.26f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 point = new Vector2(x, y);
                float distance = DistanceToSegment(point, start, end);
                float alpha = Mathf.Clamp01(1f - distance / 4.2f);
                int hash = Mathf.Abs((x * 16777619) ^ (y * 216613626));
                float roughness = 0.72f + (hash % 100) / 360f;
                pixels[y * width + x] = new Color(0f, 0f, 0f, alpha * roughness);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, true);
        return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
    }

    private Sprite CreateSketchStrokeSprite(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        float halfHeight = height * 0.5f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int hash = Mathf.Abs((x * 374761393) ^ (y * 668265263) ^ 0x2a77);
                float grain = (hash % 1000) / 1000f;
                float center = halfHeight + Mathf.Sin(x * 0.13f) * 1.4f + Mathf.Sin(x * 0.041f) * 1.9f;
                float distance = Mathf.Abs(y - center);
                float edge = Mathf.Clamp01(1f - distance / (halfHeight * 0.72f));
                float brokenInk = grain > 0.94f ? 0.45f : 1f;
                float alpha = Mathf.SmoothStep(0f, 1f, edge) * color.a * brokenInk * (0.78f + grain * 0.3f);
                pixels[y * width + x] = new Color(color.r, color.g, color.b, Mathf.Clamp01(alpha));
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, true);
        return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0f, 0.5f), 100f);
    }

    private float DistanceToSegment(Vector2 point, Vector2 start, Vector2 end)
    {
        Vector2 segment = end - start;
        float lengthSquared = segment.sqrMagnitude;

        if (lengthSquared <= 0f)
        {
            return Vector2.Distance(point, start);
        }

        float t = Mathf.Clamp01(Vector2.Dot(point - start, segment) / lengthSquared);
        Vector2 projection = start + segment * t;
        return Vector2.Distance(point, projection);
    }

    private struct WatercolorDrop
    {
        public WatercolorDrop(RectTransform rect, CanvasGroup group, float delay)
        {
            Rect = rect;
            Group = group;
            Delay = delay;
        }

        public RectTransform Rect { get; }
        public CanvasGroup Group { get; }
        public float Delay { get; }
    }

    private struct SketchStroke
    {
        public SketchStroke(RectTransform rect, float targetWidth)
        {
            Rect = rect;
            TargetWidth = targetWidth;
        }

        public RectTransform Rect { get; }
        public float TargetWidth { get; }
    }

    private Sprite CreatePanelSprite(int width, int height, Color fillColor, Color borderColor, int borderThickness)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool border = x < borderThickness || x >= width - borderThickness || y < borderThickness || y >= height - borderThickness;
                float paperNoise = ((x * 17 + y * 31) % 23) / 800f;
                Color color = border ? borderColor : new Color(fillColor.r - paperNoise, fillColor.g - paperNoise, fillColor.b - paperNoise, fillColor.a);
                pixels[y * width + x] = color;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, true);
        return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(18f, 18f, 18f, 18f));
    }

    private Sprite CreateRingSprite(int size, Color color, int thickness)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        float center = (size - 1) * 0.5f;
        float outerRadius = size * 0.42f;
        float innerRadius = outerRadius - thickness;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                pixels[y * size + x] = distance <= outerRadius && distance >= innerRadius ? color : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, true);
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
}
