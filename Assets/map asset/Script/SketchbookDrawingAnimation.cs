using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SketchbookDrawingAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform drawingRoot;
    [SerializeField] private bool playOnEnable;
    [SerializeField] private float startDelay = 0.12f;
    [SerializeField] private float drawDuration = 2.1f;
    [SerializeField] private float pencilExitDuration = 0.28f;
    [SerializeField] private Vector2 drawingSize = new Vector2(820f, 540f);
    [SerializeField] private Vector2 drawingOffset = new Vector2(0f, -20f);
    [SerializeField] private float lineWidth = 5f;
    [SerializeField] private Color lineColor = new Color32(34, 32, 29, 255);
    [SerializeField] private Vector2 pencilSize = new Vector2(150f, 30f);

    private SketchbookLineDrawingGraphic drawingGraphic;
    private RectTransform pencilRoot;
    private CanvasGroup pencilCanvasGroup;
    private Coroutine animationRoutine;
    private bool isSetup;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            Play();
        }
    }

    private void OnDisable()
    {
        StopAnimation();
    }

    public void Play()
    {
        EnsureSetup();
        StopAnimation();
        animationRoutine = StartCoroutine(PlayRoutine());
    }

    public void ResetDrawing()
    {
        StopAnimation();

        if (!isSetup)
        {
            return;
        }

        SetInitialDrawingState();
    }

    public void ShowCompletedDrawing()
    {
        EnsureSetup();
        StopAnimation();

        drawingGraphic.RevealProgress = 1f;
        pencilCanvasGroup.alpha = 0f;
        pencilRoot.gameObject.SetActive(false);
    }

    private IEnumerator PlayRoutine()
    {
        SetInitialDrawingState();

        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        pencilRoot.gameObject.SetActive(true);
        SetPencilAtProgress(0f);

        float elapsed = 0f;

        while (elapsed < drawDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / drawDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            drawingGraphic.RevealProgress = easedProgress;
            SetPencilAtProgress(easedProgress);
            yield return null;
        }

        drawingGraphic.RevealProgress = 1f;
        SetPencilAtProgress(1f);

        if (pencilExitDuration > 0f)
        {
            yield return MovePencilOut();
        }

        pencilRoot.gameObject.SetActive(false);
        animationRoutine = null;
    }

    private IEnumerator MovePencilOut()
    {
        Vector2 startPosition = pencilRoot.anchoredPosition;
        Vector2 endPosition = startPosition + new Vector2(160f, 80f);
        float elapsed = 0f;

        while (elapsed < pencilExitDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / pencilExitDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            pencilRoot.anchoredPosition = Vector2.Lerp(startPosition, endPosition, easedProgress);
            pencilCanvasGroup.alpha = 1f - easedProgress;
            yield return null;
        }
    }

    private void EnsureSetup()
    {
        if (isSetup && drawingGraphic != null && pencilRoot != null)
        {
            return;
        }

        drawingRoot = drawingRoot != null ? drawingRoot : CreateDrawingRoot();
        ConfigureDrawingRoot();

        drawingGraphic = drawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (drawingGraphic == null)
        {
            drawingGraphic = drawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        drawingGraphic.raycastTarget = false;
        drawingGraphic.color = lineColor;
        drawingGraphic.LineWidth = lineWidth;
        drawingGraphic.SetStrokes(BuildSketchbookStrokes());

        pencilRoot = CreateOrFindPencilRoot();
        pencilCanvasGroup = pencilRoot.GetComponent<CanvasGroup>();
        if (pencilCanvasGroup == null)
        {
            pencilCanvasGroup = pencilRoot.gameObject.AddComponent<CanvasGroup>();
        }

        isSetup = true;
        SetInitialDrawingState();
    }

    private RectTransform CreateDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchDrawing", typeof(RectTransform));
        drawingObject.transform.SetParent(transform, false);
        return drawingObject.GetComponent<RectTransform>();
    }

    private void ConfigureDrawingRoot()
    {
        drawingRoot.anchorMin = new Vector2(0.5f, 0.5f);
        drawingRoot.anchorMax = new Vector2(0.5f, 0.5f);
        drawingRoot.pivot = new Vector2(0.5f, 0.5f);
        drawingRoot.anchoredPosition = drawingOffset;
        drawingRoot.sizeDelta = drawingSize;
        drawingRoot.localScale = Vector3.one;
        drawingRoot.localRotation = Quaternion.identity;
    }

    private RectTransform CreateOrFindPencilRoot()
    {
        Transform existing = drawingRoot.Find("GeneratedPencil");
        RectTransform root = existing as RectTransform;

        if (root == null)
        {
            GameObject pencilObject = new GameObject("GeneratedPencil", typeof(RectTransform));
            pencilObject.transform.SetParent(drawingRoot, false);
            root = pencilObject.GetComponent<RectTransform>();
        }

        root.anchorMin = new Vector2(0.5f, 0.5f);
        root.anchorMax = new Vector2(0.5f, 0.5f);
        root.pivot = new Vector2(0.96f, 0.5f);
        root.sizeDelta = pencilSize;
        root.localScale = Vector3.one;

        SketchbookPencilGraphic pencilGraphic = root.GetComponent<SketchbookPencilGraphic>();
        if (pencilGraphic == null)
        {
            pencilGraphic = root.gameObject.AddComponent<SketchbookPencilGraphic>();
        }

        pencilGraphic.raycastTarget = false;
        return root;
    }

    private void SetInitialDrawingState()
    {
        drawingGraphic.RevealProgress = 0f;
        pencilCanvasGroup.alpha = 1f;
        pencilRoot.gameObject.SetActive(false);
        pencilRoot.localRotation = Quaternion.identity;
        pencilRoot.anchoredPosition = drawingGraphic.GetPointAtProgress(0f);
    }

    private void SetPencilAtProgress(float progress)
    {
        Vector2 point = drawingGraphic.GetPointAtProgress(progress);
        Vector2 tangent = drawingGraphic.GetTangentAtProgress(progress);
        float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;

        pencilRoot.anchoredPosition = point;
        pencilRoot.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void StopAnimation()
    {
        if (animationRoutine == null)
        {
            return;
        }

        StopCoroutine(animationRoutine);
        animationRoutine = null;
    }

    private static List<Vector2[]> BuildSketchbookStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        strokes.Add(Points(-385f, -176f, -330f, -168f, -270f, -172f, -210f, -168f, -150f, -176f));
        strokes.Add(Points(-345f, -64f, -270f, 28f, -184f, -64f, -345f, -64f));
        strokes.Add(Points(-326f, -64f, -326f, -174f, -206f, -174f, -206f, -64f));
        strokes.Add(Points(-291f, -174f, -291f, -112f, -259f, -112f, -259f, -174f));
        strokes.Add(Points(-317f, -96f, -295f, -96f, -295f, -74f, -317f, -74f, -317f, -96f));
        strokes.Add(Points(-238f, -98f, -218f, -98f, -218f, -76f, -238f, -76f, -238f, -98f));
        strokes.Add(Points(-231f, -14f, -231f, 38f, -205f, 38f, -205f, -36f));
        strokes.Add(Points(-340f, -188f, -318f, -196f, -288f, -190f, -256f, -198f, -225f, -190f, -196f, -196f));
        strokes.Add(Points(-160f, -78f, -110f, -20f, -52f, -78f, -160f, -78f));
        strokes.Add(Points(-148f, -78f, -148f, -166f, -64f, -166f, -64f, -78f));
        strokes.Add(Points(-118f, -166f, -118f, -122f, -94f, -122f, -94f, -166f));
        strokes.Add(Points(-140f, -110f, -122f, -110f, -122f, -92f, -140f, -92f, -140f, -110f));
        strokes.Add(Points(-88f, -110f, -72f, -110f, -72f, -94f, -88f, -94f, -88f, -110f));
        strokes.Add(Points(-76f, -44f, -76f, -8f, -60f, -8f, -60f, -62f));
        strokes.Add(Points(-170f, -176f, -136f, -170f, -104f, -176f, -70f, -170f, -40f, -176f));
        strokes.Add(Points(-178f, -166f, -178f, -128f));
        strokes.Add(Points(-192f, -134f, -192f, -114f, -164f, -114f, -158f, -124f, -164f, -134f, -192f, -134f));
        strokes.Add(Points(-164f, -120f, -146f, -120f, -146f, -108f));
        strokes.Add(Points(-170f, -178f, -65f, -150f, 16f, -116f, 84f, -76f));
        strokes.Add(Points(348f, -178f, 286f, -150f, 232f, -112f, 174f, -76f));
        strokes.Add(Points(84f, -176f, 84f, -18f, 96f, 20f, 124f, 46f, 162f, 56f, 202f, 46f, 232f, 20f, 246f, -18f, 246f, -176f));
        strokes.Add(Points(118f, -176f, 118f, -54f));
        strokes.Add(Points(212f, -176f, 212f, -54f));
        strokes.Add(Points(120f, -54f, 144f, -28f, 166f, -22f, 190f, -28f, 212f, -54f));
        strokes.Add(Points(54f, -174f, 58f, 48f));
        strokes.Add(Points(76f, -172f, 74f, 52f));
        strokes.Add(Points(36f, 18f, 66f, 104f, 98f, 18f, 36f, 18f));
        strokes.Add(Points(42f, 58f, 66f, 138f, 92f, 58f));
        strokes.Add(Points(238f, -174f, 238f, 58f));
        strokes.Add(Points(262f, -174f, 262f, 58f));
        strokes.Add(Points(218f, 24f, 250f, 118f, 286f, 24f, 218f, 24f));
        strokes.Add(Points(226f, 66f, 250f, 152f, 278f, 66f));
        strokes.Add(Points(300f, -176f, 302f, 28f));
        strokes.Add(Points(323f, -176f, 322f, 28f));
        strokes.Add(Points(276f, -10f, 312f, 90f, 350f, -10f, 276f, -10f));
        strokes.Add(Points(286f, 40f, 312f, 122f, 342f, 40f));
        strokes.Add(Points(-42f, -176f, -42f, 30f));
        strokes.Add(Points(-18f, -176f, -20f, 30f));
        strokes.Add(Points(-68f, -12f, -30f, 96f, 8f, -12f, -68f, -12f));
        strokes.Add(Points(-58f, 42f, -30f, 132f, 0f, 42f));
        strokes.Add(Points(8f, -150f, 38f, -134f, 70f, -128f));
        strokes.Add(Points(264f, -128f, 302f, -138f, 338f, -156f));
        strokes.Add(Points(104f, -44f, 134f, -60f, 164f, -56f, 198f, -62f, 226f, -44f));

        return strokes;
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
}
