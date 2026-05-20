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
        strokes.Add(Points(-170f, -178f, -65f, -150f, 16f, -116f, 84f, -76f));
        strokes.Add(Points(348f, -178f, 286f, -150f, 232f, -112f, 174f, -76f));
        strokes.Add(Points(84f, -176f, 84f, -18f, 96f, 20f, 124f, 46f, 162f, 56f, 202f, 46f, 232f, 20f, 246f, -18f, 246f, -176f));
        strokes.Add(Points(118f, -176f, 118f, -54f));
        strokes.Add(Points(212f, -176f, 212f, -54f));
        strokes.Add(Points(120f, -54f, 144f, -28f, 166f, -22f, 190f, -28f, 212f, -54f));
        AddSketchTree(strokes, 64f, -178f, 1.45f, false);
        AddSketchTree(strokes, 248f, -178f, 1.36f, true);
        AddSketchTree(strokes, 318f, -178f, 1.12f, false);
        AddSketchTree(strokes, -30f, -178f, 1.2f, true);
        strokes.Add(Points(8f, -150f, 38f, -134f, 70f, -128f));
        strokes.Add(Points(264f, -128f, 302f, -138f, 338f, -156f));
        strokes.Add(Points(104f, -44f, 134f, -60f, 164f, -56f, 198f, -62f, 226f, -44f));

        return strokes;
    }

    private static void AddSketchTree(List<Vector2[]> strokes, float centerX, float groundY, float scale, bool flipped)
    {
        strokes.Add(TreePoints(centerX, groundY, scale, flipped, -47f, 64f, -62f, 76f, -67f, 101f, -55f, 126f, -31f, 145f, -4f, 154f, 27f, 153f, 49f, 143f, 65f, 124f, 73f, 98f, 67f, 72f, 50f, 54f, 28f, 44f));
        strokes.Add(TreePoints(centerX, groundY, scale, flipped, -47f, 64f, -38f, 43f, -16f, 34f, 4f, 35f));
        strokes.Add(TreePoints(centerX, groundY, scale, flipped, -16f, 0f, -6f, 17f, 1f, 39f, 5f, 64f));
        strokes.Add(TreePoints(centerX, groundY, scale, flipped, 5f, 64f, -47f, 76f, -29f, 59f, 1f, 48f));
        strokes.Add(TreePoints(centerX, groundY, scale, flipped, 5f, 64f, 8f, 92f, 17f, 119f, 28f, 96f, 36f, 82f, 47f, 93f, 57f, 122f, 60f, 68f));
        strokes.Add(TreePoints(centerX, groundY, scale, flipped, 60f, 68f, 75f, 68f, 91f, 62f, 72f, 50f, 50f, 48f, 39f, 36f));
        strokes.Add(TreePoints(centerX, groundY, scale, flipped, 39f, 36f, 42f, 18f, 53f, 0f));
        strokes.Add(TreePoints(centerX, groundY, scale, flipped, 27f, 96f, 37f, 80f, 48f, 92f));
        strokes.Add(TreePoints(centerX, groundY, scale, flipped, -16f, 0f, -8f, 7f, 2f, 11f));
        strokes.Add(TreePoints(centerX, groundY, scale, flipped, 53f, 0f, 44f, 8f, 36f, 13f));
    }

    private static Vector2[] TreePoints(float centerX, float groundY, float scale, bool flipped, params float[] values)
    {
        int pointCount = values.Length / 2;
        Vector2[] points = new Vector2[pointCount];
        float direction = flipped ? -1f : 1f;

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector2(
                centerX + values[i * 2] * scale * direction,
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
