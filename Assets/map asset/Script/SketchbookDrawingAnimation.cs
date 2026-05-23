using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SketchbookDrawingAnimation : MonoBehaviour
{
    private const float RequestedDrawDuration = 18f;
    private const float RequestedPencilWidth = 75f;
    private const float RequestedPencilHeight = 15f;
    private const float RequestedPencilStartLengthScale = 1f;
    private const float RequestedPencilEndLengthScale = 0.35f;

    [SerializeField] private RectTransform drawingRoot;
    [SerializeField] private bool playOnEnable;
    [SerializeField] private float startDelay = 0.12f;
    [SerializeField] private float drawDuration = RequestedDrawDuration;
    [SerializeField] private float pencilExitDuration = 0.28f;
    [SerializeField] private Vector2 drawingSize = new Vector2(820f, 540f);
    [SerializeField] private Vector2 drawingOffset = new Vector2(0f, -20f);
    [SerializeField] private float lineWidth = 5f;
    [SerializeField] private Color lineColor = new Color32(34, 32, 29, 255);
    [SerializeField] private Vector2 pencilSize = new Vector2(RequestedPencilWidth, RequestedPencilHeight);
    [SerializeField] private float pencilStartLengthScale = RequestedPencilStartLengthScale;
    [SerializeField] private float pencilEndLengthScale = RequestedPencilEndLengthScale;

    private SketchbookLineDrawingGraphic drawingGraphic;
    private RectTransform pencilRoot;
    private CanvasGroup pencilCanvasGroup;
    private Coroutine animationRoutine;
    private Action animationCompleteCallback;
    private bool isSetup;

    public bool IsPlaying => animationRoutine != null;

    private void Awake()
    {
        ApplyRequestedAnimationSettings();
    }

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

    public void Play(Action onComplete = null)
    {
        ApplyRequestedAnimationSettings();
        EnsureSetup();
        StopAnimation();
        animationCompleteCallback = onComplete;
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
        float revealProgress = 0f;

        while (elapsed < drawDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / drawDuration);
            revealProgress = Mathf.Max(revealProgress, ApplyChildDrawingPace(progress));

            drawingGraphic.RevealProgress = revealProgress;
            SetPencilAtProgress(revealProgress, elapsed, true);
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
        Action onComplete = animationCompleteCallback;
        animationCompleteCallback = null;
        onComplete?.Invoke();
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
        ApplyRequestedAnimationSettings();

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
        root.pivot = new Vector2(1f, 0.5f);
        root.sizeDelta = new Vector2(pencilSize.x * pencilStartLengthScale, pencilSize.y);
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
        pencilRoot.localScale = Vector3.one;
        SetPencilLengthAtProgress(0f);
        pencilRoot.anchoredPosition = drawingGraphic.GetPointAtProgress(0f);
    }

    private void SetPencilAtProgress(float progress)
    {
        SetPencilAtProgress(progress, 0f, false);
    }

    private void SetPencilAtProgress(float progress, float motionSeconds, bool applyChildMotion)
    {
        Vector2 point = drawingGraphic.GetPointAtProgress(progress);
        Vector2 tangent = drawingGraphic.GetTangentAtProgress(progress);
        float childAngleOffset = 0f;

        if (applyChildMotion)
        {
            Vector2 normal = new Vector2(-tangent.y, tangent.x);
            float wobble = Mathf.Sin(motionSeconds * 17.5f) * 1.35f
                + Mathf.Sin(motionSeconds * 31.7f + 0.8f) * 0.75f;
            float drag = Mathf.Sin(motionSeconds * 11.2f + 1.4f) * 0.8f;

            point += normal * wobble + tangent * drag;
            childAngleOffset = Mathf.Sin(motionSeconds * 10.4f) * 4.2f
                + Mathf.Sin(motionSeconds * 23.5f + 0.4f) * 1.4f;
        }

        float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;

        pencilRoot.anchoredPosition = point;
        pencilRoot.localRotation = Quaternion.Euler(0f, 0f, angle + childAngleOffset);
        SetPencilLengthAtProgress(progress);
    }

    private float ApplyChildDrawingPace(float progress)
    {
        float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
        float unevenPace = Mathf.Sin(progress * Mathf.PI * 13f) * 0.02f
            + Mathf.Sin(progress * Mathf.PI * 31f + 1.2f) * 0.007f;
        float middleWeight = 1f - Mathf.Abs(progress * 2f - 1f);
        float hesitation = ChildPause(progress, 0.12f, 0.025f)
            + ChildPause(progress, 0.31f, 0.02f)
            + ChildPause(progress, 0.57f, 0.026f)
            + ChildPause(progress, 0.78f, 0.018f);

        return Mathf.Clamp01(easedProgress + unevenPace * middleWeight - hesitation);
    }

    private static float ChildPause(float progress, float center, float radius)
    {
        float distance = Mathf.Abs(progress - center) / radius;

        if (distance >= 1f)
        {
            return 0f;
        }

        return Mathf.SmoothStep(0f, 1f, 1f - distance) * 0.018f;
    }

    private void SetPencilLengthAtProgress(float progress)
    {
        float lengthScale = Mathf.Lerp(
            pencilStartLengthScale,
            pencilEndLengthScale,
            Mathf.Clamp01(progress));

        pencilRoot.sizeDelta = new Vector2(pencilSize.x * lengthScale, pencilSize.y);
    }

    private void ApplyRequestedAnimationSettings()
    {
        drawDuration = RequestedDrawDuration;
        pencilSize = new Vector2(RequestedPencilWidth, RequestedPencilHeight);
        pencilStartLengthScale = RequestedPencilStartLengthScale;
        pencilEndLengthScale = RequestedPencilEndLengthScale;
    }

    private void StopAnimation()
    {
        if (animationRoutine == null)
        {
            return;
        }

        StopCoroutine(animationRoutine);
        animationRoutine = null;
        animationCompleteCallback = null;
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
        AddSketchForest(strokes);

        return strokes;
    }

    private static void AddSketchForest(List<Vector2[]> strokes)
    {
        strokes.Add(Points(-170f, -178f, -92f, -158f, -18f, -128f, 46f, -104f, 102f, -96f));
        strokes.Add(Points(76f, -178f, 126f, -166f, 176f, -172f, 230f, -164f, 286f, -176f, 350f, -168f));
        strokes.Add(Points(108f, -86f, 128f, -118f, 146f, -92f, 166f, -132f, 186f, -94f, 210f, -140f, 234f, -98f, 258f, -132f, 284f, -90f, 314f, -116f, 340f, -86f));
        AddSketchPine(strokes, 118f, -176f, 0.82f);
        AddSketchPine(strokes, 164f, -176f, 1.02f);
        AddSketchPine(strokes, 218f, -176f, 0.9f);
        AddSketchPine(strokes, 274f, -176f, 1.14f);
        AddSketchPine(strokes, 332f, -176f, 0.86f);
        strokes.Add(Points(146f, -176f, 160f, -138f, 174f, -116f, 190f, -100f));
        strokes.Add(Points(246f, -176f, 238f, -140f, 224f, -116f, 202f, -100f));
        strokes.Add(Points(96f, -54f, 124f, -68f, 154f, -60f, 184f, -72f, 216f, -58f, 248f, -70f, 280f, -52f));
    }

    private static void AddSketchPine(List<Vector2[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(PinePoints(centerX, groundY, scale, 0f, 126f, -36f, 82f, -22f, 82f, -48f, 44f, -26f, 44f, -60f, 0f, 60f, 0f, 26f, 44f, 48f, 44f, 22f, 82f, 36f, 82f, 0f, 126f));
        strokes.Add(PinePoints(centerX, groundY, scale, -8f, 0f, -8f, -18f, 8f, -18f, 8f, 0f));
        strokes.Add(PinePoints(centerX, groundY, scale, -26f, 44f, -8f, 62f, 8f, 44f));
        strokes.Add(PinePoints(centerX, groundY, scale, -22f, 82f, -6f, 98f, 10f, 82f));
    }

    private static Vector2[] PinePoints(float centerX, float groundY, float scale, params float[] values)
    {
        int pointCount = values.Length / 2;
        Vector2[] points = new Vector2[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector2(
                centerX + values[i * 2] * scale,
                groundY + values[i * 2 + 1] * scale);
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
}
