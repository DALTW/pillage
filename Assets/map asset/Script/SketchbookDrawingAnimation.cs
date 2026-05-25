using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SketchbookDrawingAnimation : MonoBehaviour
{
    private const float RequestedDrawDuration = 24f;
    private const float RequestedPencilWidth = 75f;
    private const float RequestedPencilHeight = 15f;
    private const float RequestedPencilStartLengthScale = 1f;
    private const float RequestedPencilEndLengthScale = 0.35f;
    private const float ExtendedPencilStartLengthScale = 1.28f;
    private const float ExtendedPencilEndLengthScale = 0.42f;
    private const float PencilMergeDuration = 1.64f;
    private const float ForestDrawDuration = 18f;
    private const float GreenCrayonColorDuration = 5.6f;
    private const float GreenCrayonLengthScale = 0.82f;
    private const float DefaultSketchbookWidth = 1200f;
    private const float RequestedDrawingHeight = 540f;
    private const float DrawingHorizontalPadding = 28f;
    private const float DrawingVerticalPadding = 42f;
    private static readonly Color GreenCrayonColor = new Color32(74, 166, 73, 235);

    [SerializeField] private RectTransform drawingRoot;
    [SerializeField] private bool playOnEnable;
    [SerializeField] private float startDelay = 0.12f;
    [SerializeField] private float drawDuration = RequestedDrawDuration;
    [SerializeField] private float pencilExitDuration = 0.28f;
    [SerializeField] private Vector2 drawingSize = new Vector2(DefaultSketchbookWidth / 3f, RequestedDrawingHeight);
    [SerializeField] private Vector2 drawingOffset = new Vector2(0f, -20f);
    [SerializeField] private float lineWidth = 5f;
    [SerializeField] private Color lineColor = new Color32(34, 32, 29, 255);
    [SerializeField] private Vector2 pencilSize = new Vector2(RequestedPencilWidth, RequestedPencilHeight);
    [SerializeField] private float pencilStartLengthScale = RequestedPencilStartLengthScale;
    [SerializeField] private float pencilEndLengthScale = RequestedPencilEndLengthScale;

    private SketchbookLineDrawingGraphic drawingGraphic;
    private RectTransform villageGreenDrawingRoot;
    private SketchbookLineDrawingGraphic villageGreenDrawingGraphic;
    private RectTransform forestDrawingRoot;
    private SketchbookLineDrawingGraphic forestDrawingGraphic;
    private RectTransform forestGreenDrawingRoot;
    private SketchbookLineDrawingGraphic forestGreenDrawingGraphic;
    private SketchbookLineDrawingGraphic activeDrawingGraphic;
    private RectTransform pencilRoot;
    private CanvasGroup pencilCanvasGroup;
    private Coroutine animationRoutine;
    private Action animationCompleteCallback;
    private bool isSetup;
    private float activeDrawingScale = 1f;
    private float villageDrawingScale = 1f;
    private float villageGreenDrawingScale = 1f;
    private float forestDrawingScale = 1f;
    private float forestGreenDrawingScale = 1f;

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
        activeDrawingGraphic = drawingGraphic;
        activeDrawingScale = villageDrawingScale;
        animationCompleteCallback = onComplete;
        animationRoutine = StartCoroutine(PlayRoutine());
    }

    public void PlayForestExpansion(Action onComplete = null)
    {
        ApplyRequestedAnimationSettings();
        EnsureSetup();
        StopAnimation();
        activeDrawingGraphic = forestDrawingGraphic;
        activeDrawingScale = forestDrawingScale;
        pencilStartLengthScale = ExtendedPencilStartLengthScale;
        pencilEndLengthScale = ExtendedPencilEndLengthScale;
        animationCompleteCallback = onComplete;
        animationRoutine = StartCoroutine(PlayForestExpansionRoutine());
    }

    public void PlayVillageGreenColoring(Action onComplete = null)
    {
        ApplyRequestedAnimationSettings();
        EnsureSetup();
        StopAnimation();
        activeDrawingGraphic = villageGreenDrawingGraphic;
        activeDrawingScale = villageGreenDrawingScale;
        pencilStartLengthScale = GreenCrayonLengthScale;
        pencilEndLengthScale = GreenCrayonLengthScale;
        animationCompleteCallback = onComplete;
        animationRoutine = StartCoroutine(PlayVillageGreenColoringRoutine());
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
        if (villageGreenDrawingGraphic != null)
        {
            villageGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredVillageGreen ? 1f : 0f;
        }

        if (forestDrawingGraphic != null)
        {
            forestDrawingGraphic.RevealProgress = GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (forestGreenDrawingGraphic != null)
        {
            forestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredForestGreen ? 1f : 0f;
        }

        pencilCanvasGroup.alpha = 0f;
        pencilRoot.gameObject.SetActive(false);
    }

    private IEnumerator PlayRoutine()
    {
        activeDrawingGraphic = drawingGraphic;
        activeDrawingScale = villageDrawingScale;
        pencilStartLengthScale = RequestedPencilStartLengthScale;
        pencilEndLengthScale = RequestedPencilEndLengthScale;
        SetPencilParent(drawingRoot);
        SetPencilTint(Color.white);
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

    private IEnumerator PlayForestExpansionRoutine()
    {
        drawingGraphic.RevealProgress = 1f;
        forestDrawingGraphic.RevealProgress = 0f;
        if (forestGreenDrawingGraphic != null)
        {
            forestGreenDrawingGraphic.RevealProgress = 0f;
        }

        SetPencilParent(forestDrawingRoot);
        SetPencilTint(Color.white);
        pencilCanvasGroup.alpha = 1f;
        pencilRoot.gameObject.SetActive(true);
        SetPencilAtProgress(0f);

        yield return PlayPencilMergeRoutine();

        float elapsed = 0f;
        float revealProgress = 0f;

        while (elapsed < ForestDrawDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / ForestDrawDuration);
            revealProgress = Mathf.Max(revealProgress, ApplyChildDrawingPace(progress));

            forestDrawingGraphic.RevealProgress = revealProgress;
            SetPencilAtProgress(revealProgress, elapsed, true);
            yield return null;
        }

        forestDrawingGraphic.RevealProgress = 1f;
        SetPencilAtProgress(1f);

        if (GameProgress.HasColoredVillageGreen && forestGreenDrawingGraphic != null)
        {
            yield return PlayGreenColoringOnTarget(
                forestGreenDrawingGraphic,
                forestDrawingRoot,
                forestGreenDrawingScale);
        }

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

    private IEnumerator PlayVillageGreenColoringRoutine()
    {
        drawingGraphic.RevealProgress = 1f;
        bool shouldColorVillage = !GameProgress.HasColoredVillageGreen;
        bool shouldColorForest = GameProgress.HasDrawnForestSketch && !GameProgress.HasColoredForestGreen;
        villageGreenDrawingGraphic.RevealProgress = shouldColorVillage ? 0f : 1f;

        if (forestDrawingGraphic != null)
        {
            forestDrawingGraphic.RevealProgress = GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (forestGreenDrawingGraphic != null)
        {
            forestGreenDrawingGraphic.RevealProgress = shouldColorForest ? 0f : (GameProgress.HasColoredForestGreen ? 1f : 0f);
        }

        if (shouldColorVillage)
        {
            yield return PlayGreenColoringOnTarget(
                villageGreenDrawingGraphic,
                drawingRoot,
                villageGreenDrawingScale);
        }

        if (shouldColorForest && forestGreenDrawingGraphic != null)
        {
            yield return PlayGreenColoringOnTarget(
                forestGreenDrawingGraphic,
                forestDrawingRoot,
                forestGreenDrawingScale);
        }

        if (pencilExitDuration > 0f)
        {
            yield return MovePencilOut();
        }

        SetPencilTint(Color.white);
        pencilRoot.gameObject.SetActive(false);
        animationRoutine = null;
        Action onComplete = animationCompleteCallback;
        animationCompleteCallback = null;
        onComplete?.Invoke();
    }

    private IEnumerator PlayGreenColoringOnTarget(SketchbookLineDrawingGraphic greenDrawingGraphic, RectTransform targetRoot, float targetScale)
    {
        if (greenDrawingGraphic == null || targetRoot == null)
        {
            yield break;
        }

        activeDrawingGraphic = greenDrawingGraphic;
        activeDrawingScale = targetScale;
        pencilStartLengthScale = GreenCrayonLengthScale;
        pencilEndLengthScale = GreenCrayonLengthScale;
        SetPencilParent(targetRoot);
        SetPencilTint(GreenCrayonColor);
        pencilCanvasGroup.alpha = 1f;
        pencilRoot.gameObject.SetActive(true);
        SetPencilLengthScale(GreenCrayonLengthScale);
        SetPencilAtProgress(0f);

        float elapsed = 0f;
        float revealProgress = 0f;

        while (elapsed < GreenCrayonColorDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / GreenCrayonColorDuration);
            revealProgress = Mathf.Max(revealProgress, ApplyCrayonColoringPace(progress));

            greenDrawingGraphic.RevealProgress = revealProgress;
            SetPencilAtProgress(revealProgress, elapsed, true);
            yield return null;
        }

        greenDrawingGraphic.RevealProgress = 1f;
        SetPencilAtProgress(1f);
    }

    private IEnumerator PlayPencilMergeRoutine()
    {
        Vector2 targetPosition = activeDrawingGraphic.GetPointAtProgress(0f);
        Vector2[] startOffsets =
        {
            new Vector2(-96f, 64f),
            new Vector2(-78f, -58f),
            new Vector2(-142f, 4f)
        };
        RectTransform[] fragments = new RectTransform[startOffsets.Length];
        CanvasGroup[] fragmentGroups = new CanvasGroup[startOffsets.Length];
        Vector2 scaledPencilSize = GetScaledPencilSize();

        SetPencilLengthScale(RequestedPencilEndLengthScale);
        pencilRoot.anchoredPosition = targetPosition;

        for (int i = 0; i < fragments.Length; i++)
        {
            GameObject fragmentObject = new GameObject($"GeneratedMergingPencilFragment_{i:00}", typeof(RectTransform));
            fragmentObject.transform.SetParent(forestDrawingRoot, false);

            RectTransform fragment = fragmentObject.GetComponent<RectTransform>();
            fragment.anchorMin = new Vector2(0.5f, 0.5f);
            fragment.anchorMax = new Vector2(0.5f, 0.5f);
            fragment.pivot = new Vector2(1f, 0.5f);
            fragment.sizeDelta = new Vector2(scaledPencilSize.x * 0.26f, scaledPencilSize.y);
            fragment.anchoredPosition = targetPosition + startOffsets[i] * activeDrawingScale;
            fragment.localRotation = Quaternion.Euler(0f, 0f, -18f + i * 17f);

            SketchbookPencilGraphic pencilGraphic = fragmentObject.AddComponent<SketchbookPencilGraphic>();
            pencilGraphic.raycastTarget = false;

            CanvasGroup canvasGroup = fragmentObject.AddComponent<CanvasGroup>();
            fragments[i] = fragment;
            fragmentGroups[i] = canvasGroup;
        }

        float elapsed = 0f;

        while (elapsed < PencilMergeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / PencilMergeDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            SetPencilLengthScale(Mathf.Lerp(RequestedPencilEndLengthScale, ExtendedPencilStartLengthScale, easedProgress));
            pencilRoot.anchoredPosition = targetPosition + new Vector2(Mathf.Sin(progress * Mathf.PI * 8f) * 2f, 0f);

            for (int i = 0; i < fragments.Length; i++)
            {
                if (fragments[i] == null)
                {
                    continue;
                }

                Vector2 startPosition = targetPosition + startOffsets[i] * activeDrawingScale;
                Vector2 endPosition = targetPosition + new Vector2(-12f + i * 6f, 0f) * activeDrawingScale;
                fragments[i].anchoredPosition = Vector2.Lerp(startPosition, endPosition, easedProgress);
                fragments[i].localScale = Vector3.one * Mathf.Lerp(1f, 0.2f, easedProgress);
                fragments[i].localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-18f + i * 17f, 0f, easedProgress));
                fragmentGroups[i].alpha = 1f - easedProgress;
            }

            yield return null;
        }

        for (int i = 0; i < fragments.Length; i++)
        {
            if (fragments[i] != null)
            {
                Destroy(fragments[i].gameObject);
            }
        }

        SetPencilLengthScale(ExtendedPencilStartLengthScale);
        pencilRoot.anchoredPosition = targetPosition;
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
        ConfigureDrawingRoot(drawingRoot, -1f);

        drawingGraphic = drawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (drawingGraphic == null)
        {
            drawingGraphic = drawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        drawingGraphic.raycastTarget = false;
        drawingGraphic.color = lineColor;
        drawingGraphic.LineWidth = lineWidth;

        List<Vector2[]> strokes = BuildSketchbookStrokes();
        villageDrawingScale = FitStrokesToDrawingRoot(strokes, drawingRoot);
        activeDrawingScale = villageDrawingScale;
        drawingGraphic.SetStrokes(strokes);

        villageGreenDrawingRoot = villageGreenDrawingRoot != null ? villageGreenDrawingRoot : CreateVillageGreenDrawingRoot();
        ConfigureOverlayDrawingRoot(villageGreenDrawingRoot, drawingRoot);

        villageGreenDrawingGraphic = villageGreenDrawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (villageGreenDrawingGraphic == null)
        {
            villageGreenDrawingGraphic = villageGreenDrawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        villageGreenDrawingGraphic.raycastTarget = false;
        villageGreenDrawingGraphic.color = GreenCrayonColor;
        villageGreenDrawingGraphic.LineWidth = lineWidth * 2.45f;

        List<Vector2[]> villageGreenStrokes = BuildSketchbookVillageGreenColorStrokes();
        villageGreenDrawingScale = FitStrokesToReferenceRoot(villageGreenStrokes, drawingRoot, BuildSketchbookStrokes());
        villageGreenDrawingGraphic.SetStrokes(villageGreenStrokes);

        forestDrawingRoot = forestDrawingRoot != null ? forestDrawingRoot : CreateForestDrawingRoot();
        ConfigureDrawingRoot(forestDrawingRoot, 0f);

        forestDrawingGraphic = forestDrawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (forestDrawingGraphic == null)
        {
            forestDrawingGraphic = forestDrawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        forestDrawingGraphic.raycastTarget = false;
        forestDrawingGraphic.color = lineColor;
        forestDrawingGraphic.LineWidth = lineWidth;

        List<Vector2[]> forestStrokes = BuildSketchbookForestStrokes();
        forestDrawingScale = FitStrokesToDrawingRoot(forestStrokes, forestDrawingRoot);
        forestDrawingGraphic.SetStrokes(forestStrokes);

        forestGreenDrawingRoot = forestGreenDrawingRoot != null ? forestGreenDrawingRoot : CreateForestGreenDrawingRoot();
        ConfigureOverlayDrawingRoot(forestGreenDrawingRoot, forestDrawingRoot);

        forestGreenDrawingGraphic = forestGreenDrawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (forestGreenDrawingGraphic == null)
        {
            forestGreenDrawingGraphic = forestGreenDrawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        forestGreenDrawingGraphic.raycastTarget = false;
        forestGreenDrawingGraphic.color = GreenCrayonColor;
        forestGreenDrawingGraphic.LineWidth = lineWidth * 2.45f;

        List<Vector2[]> forestGreenStrokes = BuildSketchbookForestGreenColorStrokes();
        forestGreenDrawingScale = FitStrokesToReferenceRoot(forestGreenStrokes, forestDrawingRoot, BuildSketchbookForestStrokes());
        forestGreenDrawingGraphic.SetStrokes(forestGreenStrokes);

        pencilRoot = CreateOrFindPencilRoot();
        pencilCanvasGroup = pencilRoot.GetComponent<CanvasGroup>();
        if (pencilCanvasGroup == null)
        {
            pencilCanvasGroup = pencilRoot.gameObject.AddComponent<CanvasGroup>();
        }

        activeDrawingGraphic = drawingGraphic;
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

    private RectTransform CreateVillageGreenDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchVillageGreenDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchVillageGreenDrawing", typeof(RectTransform));
        drawingObject.transform.SetParent(transform, false);
        return drawingObject.GetComponent<RectTransform>();
    }

    private RectTransform CreateForestDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchForestDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchForestDrawing", typeof(RectTransform));
        drawingObject.transform.SetParent(transform, false);
        return drawingObject.GetComponent<RectTransform>();
    }

    private RectTransform CreateForestGreenDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchForestGreenDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchForestGreenDrawing", typeof(RectTransform));
        drawingObject.transform.SetParent(transform, false);
        return drawingObject.GetComponent<RectTransform>();
    }

    private void ConfigureDrawingRoot(RectTransform targetRoot, float thirdOffset)
    {
        if (targetRoot == null)
        {
            return;
        }

        float thirdWidth = GetSketchbookWidth() / 3f;

        targetRoot.anchorMin = new Vector2(0.5f, 0.5f);
        targetRoot.anchorMax = new Vector2(0.5f, 0.5f);
        targetRoot.pivot = new Vector2(0.5f, 0.5f);
        targetRoot.anchoredPosition = new Vector2(thirdWidth * thirdOffset + drawingOffset.x, drawingOffset.y);
        targetRoot.sizeDelta = new Vector2(thirdWidth, drawingSize.y);
        targetRoot.localScale = Vector3.one;
        targetRoot.localRotation = Quaternion.identity;
    }

    private static void ConfigureOverlayDrawingRoot(RectTransform overlayRoot, RectTransform sourceRoot)
    {
        if (overlayRoot == null || sourceRoot == null)
        {
            return;
        }

        overlayRoot.anchorMin = sourceRoot.anchorMin;
        overlayRoot.anchorMax = sourceRoot.anchorMax;
        overlayRoot.pivot = sourceRoot.pivot;
        overlayRoot.anchoredPosition = sourceRoot.anchoredPosition;
        overlayRoot.sizeDelta = sourceRoot.sizeDelta;
        overlayRoot.localScale = Vector3.one;
        overlayRoot.localRotation = Quaternion.identity;
    }

    private RectTransform CreateOrFindPencilRoot()
    {
        Transform existing = drawingRoot.Find("GeneratedPencil");
        if (existing == null && forestDrawingRoot != null)
        {
            existing = forestDrawingRoot.Find("GeneratedPencil");
        }

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
        Vector2 scaledPencilSize = GetScaledPencilSize();
        root.sizeDelta = new Vector2(scaledPencilSize.x * pencilStartLengthScale, scaledPencilSize.y);
        root.localScale = Vector3.one;

        SketchbookPencilGraphic pencilGraphic = root.GetComponent<SketchbookPencilGraphic>();
        if (pencilGraphic == null)
        {
            pencilGraphic = root.gameObject.AddComponent<SketchbookPencilGraphic>();
        }

        pencilGraphic.raycastTarget = false;
        return root;
    }

    private void SetPencilParent(RectTransform parentRoot)
    {
        if (pencilRoot == null || parentRoot == null)
        {
            return;
        }

        if (pencilRoot.parent != parentRoot)
        {
            pencilRoot.SetParent(parentRoot, false);
        }

        pencilRoot.anchorMin = new Vector2(0.5f, 0.5f);
        pencilRoot.anchorMax = new Vector2(0.5f, 0.5f);
        pencilRoot.pivot = new Vector2(1f, 0.5f);
        pencilRoot.localScale = Vector3.one;
    }

    private void SetInitialDrawingState()
    {
        SketchbookLineDrawingGraphic targetDrawing = activeDrawingGraphic != null ? activeDrawingGraphic : drawingGraphic;

        if (drawingGraphic != null)
        {
            drawingGraphic.RevealProgress = 0f;
        }

        if (villageGreenDrawingGraphic != null)
        {
            villageGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredVillageGreen ? 1f : 0f;
        }

        if (forestDrawingGraphic != null)
        {
            forestDrawingGraphic.RevealProgress = GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (forestGreenDrawingGraphic != null)
        {
            forestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredForestGreen ? 1f : 0f;
        }

        if (targetDrawing == null || pencilRoot == null || pencilCanvasGroup == null)
        {
            return;
        }

        pencilCanvasGroup.alpha = 1f;
        pencilRoot.gameObject.SetActive(false);
        pencilRoot.localRotation = Quaternion.identity;
        pencilRoot.localScale = Vector3.one;
        SetPencilLengthAtProgress(0f);
        pencilRoot.anchoredPosition = targetDrawing.GetPointAtProgress(0f);
    }

    private void SetPencilAtProgress(float progress)
    {
        SetPencilAtProgress(progress, 0f, false);
    }

    private void SetPencilAtProgress(float progress, float motionSeconds, bool applyChildMotion)
    {
        SketchbookLineDrawingGraphic targetDrawing = activeDrawingGraphic != null ? activeDrawingGraphic : drawingGraphic;
        if (targetDrawing == null)
        {
            return;
        }

        Vector2 point = targetDrawing.GetPointAtProgress(progress);
        Vector2 tangent = targetDrawing.GetTangentAtProgress(progress);
        float childAngleOffset = 0f;

        if (applyChildMotion)
        {
            Vector2 normal = new Vector2(-tangent.y, tangent.x);
            float wobble = Mathf.Sin(motionSeconds * 13.5f) * 2.35f
                + Mathf.Sin(motionSeconds * 27.7f + 0.8f) * 1.15f
                + Mathf.Sin(motionSeconds * 4.8f + 1.7f) * 1.05f;
            float drag = Mathf.Sin(motionSeconds * 8.2f + 1.4f) * 1.15f
                + Mathf.Sin(motionSeconds * 19.4f + 0.2f) * 0.45f;

            point += normal * wobble + tangent * drag;
            childAngleOffset = Mathf.Sin(motionSeconds * 8.4f) * 7.8f
                + Mathf.Sin(motionSeconds * 21.5f + 0.4f) * 2.8f;
        }

        float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;

        pencilRoot.anchoredPosition = point;
        pencilRoot.localRotation = Quaternion.Euler(0f, 0f, angle + childAngleOffset);
        SetPencilLengthAtProgress(progress);
    }

    private float ApplyChildDrawingPace(float progress)
    {
        float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
        float unevenPace = Mathf.Sin(progress * Mathf.PI * 9f) * 0.033f
            + Mathf.Sin(progress * Mathf.PI * 23f + 1.2f) * 0.014f
            + Mathf.Sin(progress * Mathf.PI * 41f + 0.45f) * 0.007f;
        float burst = Mathf.Max(0f, Mathf.Sin(progress * Mathf.PI * 12f + 0.35f)) * 0.012f;
        float middleWeight = 1f - Mathf.Abs(progress * 2f - 1f);
        float hesitation = ChildPause(progress, 0.07f, 0.025f, 0.024f)
            + ChildPause(progress, 0.15f, 0.03f, 0.032f)
            + ChildPause(progress, 0.28f, 0.022f, 0.026f)
            + ChildPause(progress, 0.39f, 0.027f, 0.03f)
            + ChildPause(progress, 0.55f, 0.034f, 0.038f)
            + ChildPause(progress, 0.69f, 0.024f, 0.028f)
            + ChildPause(progress, 0.82f, 0.026f, 0.03f)
            + ChildPause(progress, 0.91f, 0.018f, 0.018f);

        return Mathf.Clamp01(easedProgress + (unevenPace + burst) * middleWeight - hesitation);
    }

    private static float ChildPause(float progress, float center, float radius, float strength)
    {
        float distance = Mathf.Abs(progress - center) / radius;

        if (distance >= 1f)
        {
            return 0f;
        }

        return Mathf.SmoothStep(0f, 1f, 1f - distance) * strength;
    }

    private static float ApplyCrayonColoringPace(float progress)
    {
        float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
        float crayonDrag = Mathf.Sin(progress * Mathf.PI * 8.5f) * 0.018f
            + Mathf.Sin(progress * Mathf.PI * 19f + 0.8f) * 0.011f;
        float pause = ChildPause(progress, 0.18f, 0.028f, 0.025f)
            + ChildPause(progress, 0.43f, 0.034f, 0.028f)
            + ChildPause(progress, 0.68f, 0.032f, 0.026f);

        return Mathf.Clamp01(easedProgress + crayonDrag - pause);
    }

    private void SetPencilLengthAtProgress(float progress)
    {
        float lengthScale = Mathf.Lerp(
            pencilStartLengthScale,
            pencilEndLengthScale,
            Mathf.Clamp01(progress));
        SetPencilLengthScale(lengthScale);
    }

    private void SetPencilLengthScale(float lengthScale)
    {
        if (pencilRoot == null)
        {
            return;
        }

        Vector2 scaledPencilSize = GetScaledPencilSize();

        pencilRoot.sizeDelta = new Vector2(scaledPencilSize.x * Mathf.Max(0.01f, lengthScale), scaledPencilSize.y);
    }

    private void SetPencilTint(Color tintColor)
    {
        if (pencilRoot == null)
        {
            return;
        }

        SketchbookPencilGraphic pencilGraphic = pencilRoot.GetComponent<SketchbookPencilGraphic>();
        if (pencilGraphic != null)
        {
            pencilGraphic.color = tintColor;
        }
    }

    private void ApplyRequestedAnimationSettings()
    {
        drawDuration = RequestedDrawDuration;
        drawingSize = new Vector2(DefaultSketchbookWidth / 3f, RequestedDrawingHeight);
        drawingOffset = new Vector2(0f, -20f);
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

    private float FitStrokesToDrawingRoot(List<Vector2[]> strokes, RectTransform targetRoot)
    {
        if (strokes.Count == 0 || targetRoot == null)
        {
            return 1f;
        }

        if (!TryGetStrokeBounds(strokes, out Vector2 min, out Vector2 max))
        {
            return 1f;
        }

        Vector2 boundsSize = max - min;

        if (boundsSize.x <= 0f || boundsSize.y <= 0f)
        {
            return 1f;
        }

        Vector2 availableSize = GetDrawingAvailableSize(targetRoot);
        float scale = Mathf.Min(availableSize.x / boundsSize.x, availableSize.y / boundsSize.y);
        scale = Mathf.Max(0.01f, scale);

        Vector2 boundsCenter = (min + max) * 0.5f;

        for (int strokeIndex = 0; strokeIndex < strokes.Count; strokeIndex++)
        {
            Vector2[] stroke = strokes[strokeIndex];

            for (int pointIndex = 0; pointIndex < stroke.Length; pointIndex++)
            {
                stroke[pointIndex] = (stroke[pointIndex] - boundsCenter) * scale;
            }
        }

        return scale;
    }

    private float FitStrokesToReferenceRoot(List<Vector2[]> strokes, RectTransform targetRoot, List<Vector2[]> referenceStrokes)
    {
        if (strokes.Count == 0 || targetRoot == null || referenceStrokes == null || referenceStrokes.Count == 0)
        {
            return 1f;
        }

        if (!TryGetStrokeBounds(referenceStrokes, out Vector2 min, out Vector2 max))
        {
            return 1f;
        }

        Vector2 boundsSize = max - min;

        if (boundsSize.x <= 0f || boundsSize.y <= 0f)
        {
            return 1f;
        }

        Vector2 availableSize = GetDrawingAvailableSize(targetRoot);
        float scale = Mathf.Min(availableSize.x / boundsSize.x, availableSize.y / boundsSize.y);
        scale = Mathf.Max(0.01f, scale);
        Vector2 boundsCenter = (min + max) * 0.5f;

        for (int strokeIndex = 0; strokeIndex < strokes.Count; strokeIndex++)
        {
            Vector2[] stroke = strokes[strokeIndex];

            for (int pointIndex = 0; pointIndex < stroke.Length; pointIndex++)
            {
                stroke[pointIndex] = (stroke[pointIndex] - boundsCenter) * scale;
            }
        }

        return scale;
    }

    private bool TryGetStrokeBounds(List<Vector2[]> strokes, out Vector2 min, out Vector2 max)
    {
        min = new Vector2(float.MaxValue, float.MaxValue);
        max = new Vector2(float.MinValue, float.MinValue);
        bool hasPoint = false;

        foreach (Vector2[] stroke in strokes)
        {
            foreach (Vector2 point in stroke)
            {
                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
                hasPoint = true;
            }
        }

        return hasPoint;
    }

    private Vector2 GetDrawingAvailableSize(RectTransform targetRoot)
    {
        Rect rect = targetRoot.rect;
        float width = rect.width > 0f ? rect.width : Mathf.Abs(targetRoot.sizeDelta.x);
        float height = rect.height > 0f ? rect.height : Mathf.Abs(targetRoot.sizeDelta.y);

        return new Vector2(
            Mathf.Max(1f, width - DrawingHorizontalPadding * 2f),
            Mathf.Max(1f, height - DrawingVerticalPadding * 2f));
    }

    private float GetSketchbookWidth()
    {
        RectTransform sketchbookRect = transform as RectTransform;

        if (sketchbookRect != null)
        {
            float rectWidth = sketchbookRect.rect.width;

            if (rectWidth > 0f)
            {
                return rectWidth;
            }

            float sizeDeltaWidth = Mathf.Abs(sketchbookRect.sizeDelta.x);

            if (sizeDeltaWidth > 0f)
            {
                return sizeDeltaWidth;
            }
        }

        return DefaultSketchbookWidth;
    }

    private Vector2 GetScaledPencilSize()
    {
        return pencilSize * activeDrawingScale;
    }

    private static List<Vector2[]> BuildSketchbookStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapFence(strokes);
        AddMiniMapPaths(strokes);
        AddMiniMapPond(strokes, -23.5f, -13.6f);
        AddMiniMapHouse(strokes, 0f, 2f, 6.0f);
        AddMiniMapHouse(strokes, 18.4f, 3.1f, 5.0f);
        AddMiniMapMailbox(strokes, 16.38f, 2.14f);
        AddMiniMapNpc(strokes, 14.68f, 2.19f);
        AddMiniMapForestEntrance(strokes, 21.5f, -14.2f);
        AddMiniMapTrees(strokes);
        AddMiniMapGrass(strokes);

        return strokes;
    }

    private static List<Vector2[]> BuildSketchbookVillageGreenColorStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapForestEntranceGreenColor(strokes, 21.5f, -14.2f);
        AddMiniMapTreeGreenColor(strokes);
        AddMiniMapGrassGreenColor(strokes);

        return strokes;
    }

    private static void AddMiniMapForestEntranceGreenColor(List<Vector2[]> strokes, float centerX, float centerY)
    {
        AddMiniMapPineCrayon(strokes, centerX - 1.8f, centerY - 1.1f, 0.95f);
        AddMiniMapPineCrayon(strokes, centerX, centerY - 1.05f, 1.1f);
        AddMiniMapPineCrayon(strokes, centerX + 1.8f, centerY - 1.1f, 0.95f);
    }

    private static void AddMiniMapTreeGreenColor(List<Vector2[]> strokes)
    {
        AddMiniMapPineCrayon(strokes, -28.0f, 15.5f, 1.0f);
        AddMiniMapPineCrayon(strokes, -20.5f, 7.4f, 0.86f);
        AddMiniMapPineCrayon(strokes, -15.8f, 16.8f, 1.12f);
        AddMiniMapPineCrayon(strokes, -6.4f, 14.8f, 0.92f);
        AddMiniMapPineCrayon(strokes, 8.8f, 16.2f, 1.02f);
        AddMiniMapPineCrayon(strokes, 26.6f, 13.4f, 0.94f);
        AddMiniMapPineCrayon(strokes, 30.2f, 6.6f, 0.84f);
        AddMiniMapPineCrayon(strokes, -30.0f, -2.8f, 0.9f);
        AddMiniMapPineCrayon(strokes, -12.0f, -10.2f, 0.82f);
        AddMiniMapPineCrayon(strokes, 6.2f, -13.8f, 0.9f);
        AddMiniMapPineCrayon(strokes, 12.0f, -18.0f, 0.86f);
        AddMiniMapPineCrayon(strokes, 29.2f, -7.8f, 0.92f);
    }

    private static void AddMiniMapGrassGreenColor(List<Vector2[]> strokes)
    {
        AddMiniMapGrassCrayon(strokes, -27.6f, 3.8f, 0.86f);
        AddMiniMapGrassCrayon(strokes, -18.8f, -3.2f, 0.78f);
        AddMiniMapGrassCrayon(strokes, -9.4f, -16.4f, 0.9f);
        AddMiniMapGrassCrayon(strokes, -2.6f, -7.8f, 0.7f);
        AddMiniMapGrassCrayon(strokes, 7.6f, -6.8f, 0.82f);
        AddMiniMapGrassCrayon(strokes, 13.2f, 1.2f, 0.7f);
        AddMiniMapGrassCrayon(strokes, 21.0f, 10.6f, 0.78f);
        AddMiniMapGrassCrayon(strokes, 27.4f, -15.2f, 0.86f);
    }

    private static List<Vector2[]> BuildSketchbookForestStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapForestBoundary(strokes);
        AddMiniMapForestPaths(strokes);
        AddMiniMapForestClearing(strokes, 5.4f, -1.2f);
        AddMiniMapForestTrees(strokes);
        AddMiniMapForestGrass(strokes);

        return strokes;
    }

    private static List<Vector2[]> BuildSketchbookForestGreenColorStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapForestTreeGreenColor(strokes);
        AddMiniMapForestGrassGreenColor(strokes);

        return strokes;
    }

    private static void AddMiniMapForestBoundary(List<Vector2[]> strokes)
    {
        const float left = -34.5f;
        const float right = 34.5f;
        const float top = 23.5f;
        const float bottom = -23.5f;
        const float gateHalfHeight = 3.2f;

        strokes.Add(MapPoints(left, top, right, top, right, bottom, left, bottom, left, -gateHalfHeight));
        strokes.Add(MapPoints(left, gateHalfHeight, left, top));
        strokes.Add(MapPoints(left, gateHalfHeight, left + 3.1f, 1.1f, left + 3.1f, -1.1f, left, -gateHalfHeight));
        strokes.Add(MapPoints(left + 1.7f, top - 1.7f, right - 1.7f, top - 1.7f, right - 1.7f, bottom + 1.7f, left + 1.7f, bottom + 1.7f, left + 1.7f, -gateHalfHeight - 1.2f));
        strokes.Add(MapPoints(left + 1.7f, gateHalfHeight + 1.2f, left + 1.7f, top - 1.7f));
    }

    private static void AddMiniMapForestPaths(List<Vector2[]> strokes)
    {
        strokes.Add(MapPoints(
            -34.5f, 0.0f,
            -26.0f, 0.6f,
            -17.2f, -2.2f,
            -7.6f, 1.8f,
            3.4f, -1.0f,
            14.6f, 2.6f,
            27.8f, 0.2f));
        strokes.Add(MapPoints(
            -9.8f, 0.4f,
            -15.4f, 7.0f,
            -22.0f, 11.6f,
            -28.6f, 16.2f));
        strokes.Add(MapPoints(
            5.4f, -1.2f,
            1.2f, -8.2f,
            6.8f, -14.2f,
            15.4f, -18.0f));
        strokes.Add(MapPoints(
            13.4f, 2.4f,
            19.2f, 9.2f,
            26.4f, 15.8f));
    }

    private static void AddMiniMapForestClearing(List<Vector2[]> strokes, float centerX, float centerY)
    {
        strokes.Add(MapEllipsePoints(centerX, centerY, 5.8f, 3.5f, 28));
        strokes.Add(MapPoints(centerX - 3.5f, centerY + 0.6f, centerX - 1.2f, centerY + 1.1f, centerX + 1.4f, centerY + 0.74f, centerX + 3.7f, centerY + 1.2f));
        strokes.Add(MapPoints(centerX - 4.2f, centerY - 1.0f, centerX - 1.8f, centerY - 1.42f, centerX + 0.8f, centerY - 1.0f, centerX + 3.8f, centerY - 1.5f));
    }

    private static void AddMiniMapForestTrees(List<Vector2[]> strokes)
    {
        AddMiniMapPine(strokes, -28.6f, 18.0f, 1.12f);
        AddMiniMapPine(strokes, -23.4f, 13.8f, 0.96f);
        AddMiniMapPine(strokes, -18.4f, 18.4f, 1.04f);
        AddMiniMapPine(strokes, -12.0f, 12.6f, 1.08f);
        AddMiniMapPine(strokes, -5.6f, 17.6f, 0.92f);
        AddMiniMapPine(strokes, 2.2f, 15.0f, 1.02f);
        AddMiniMapPine(strokes, 10.4f, 18.0f, 1.14f);
        AddMiniMapPine(strokes, 18.2f, 14.0f, 0.98f);
        AddMiniMapPine(strokes, 27.2f, 17.6f, 1.06f);
        AddMiniMapPine(strokes, -29.0f, 7.4f, 0.9f);
        AddMiniMapPine(strokes, -20.6f, 4.6f, 1.0f);
        AddMiniMapPine(strokes, -2.2f, 7.0f, 0.88f);
        AddMiniMapPine(strokes, 10.4f, 8.2f, 0.96f);
        AddMiniMapPine(strokes, 23.4f, 6.2f, 1.0f);
        AddMiniMapPine(strokes, -29.2f, -6.8f, 1.04f);
        AddMiniMapPine(strokes, -20.0f, -10.0f, 0.92f);
        AddMiniMapPine(strokes, -10.4f, -14.8f, 1.06f);
        AddMiniMapPine(strokes, -1.4f, -18.0f, 0.92f);
        AddMiniMapPine(strokes, 9.8f, -10.6f, 0.94f);
        AddMiniMapPine(strokes, 21.2f, -12.8f, 1.08f);
        AddMiniMapPine(strokes, 29.0f, -5.8f, 0.96f);
        AddMiniMapPine(strokes, 29.2f, -18.0f, 1.0f);
    }

    private static void AddMiniMapForestGrass(List<Vector2[]> strokes)
    {
        AddMiniMapGrassTuft(strokes, -31.0f, -0.8f, 0.84f);
        AddMiniMapGrassTuft(strokes, -24.0f, -17.4f, 0.86f);
        AddMiniMapGrassTuft(strokes, -13.4f, -5.6f, 0.78f);
        AddMiniMapGrassTuft(strokes, -4.4f, -8.8f, 0.72f);
        AddMiniMapGrassTuft(strokes, 4.2f, 5.8f, 0.76f);
        AddMiniMapGrassTuft(strokes, 13.6f, -4.8f, 0.82f);
        AddMiniMapGrassTuft(strokes, 22.8f, 0.8f, 0.74f);
        AddMiniMapGrassTuft(strokes, 30.0f, 10.2f, 0.86f);
    }

    private static void AddMiniMapForestTreeGreenColor(List<Vector2[]> strokes)
    {
        AddMiniMapPineCrayon(strokes, -28.6f, 18.0f, 1.12f);
        AddMiniMapPineCrayon(strokes, -23.4f, 13.8f, 0.96f);
        AddMiniMapPineCrayon(strokes, -18.4f, 18.4f, 1.04f);
        AddMiniMapPineCrayon(strokes, -12.0f, 12.6f, 1.08f);
        AddMiniMapPineCrayon(strokes, -5.6f, 17.6f, 0.92f);
        AddMiniMapPineCrayon(strokes, 2.2f, 15.0f, 1.02f);
        AddMiniMapPineCrayon(strokes, 10.4f, 18.0f, 1.14f);
        AddMiniMapPineCrayon(strokes, 18.2f, 14.0f, 0.98f);
        AddMiniMapPineCrayon(strokes, 27.2f, 17.6f, 1.06f);
        AddMiniMapPineCrayon(strokes, -29.0f, 7.4f, 0.9f);
        AddMiniMapPineCrayon(strokes, -20.6f, 4.6f, 1.0f);
        AddMiniMapPineCrayon(strokes, -2.2f, 7.0f, 0.88f);
        AddMiniMapPineCrayon(strokes, 10.4f, 8.2f, 0.96f);
        AddMiniMapPineCrayon(strokes, 23.4f, 6.2f, 1.0f);
        AddMiniMapPineCrayon(strokes, -29.2f, -6.8f, 1.04f);
        AddMiniMapPineCrayon(strokes, -20.0f, -10.0f, 0.92f);
        AddMiniMapPineCrayon(strokes, -10.4f, -14.8f, 1.06f);
        AddMiniMapPineCrayon(strokes, -1.4f, -18.0f, 0.92f);
        AddMiniMapPineCrayon(strokes, 9.8f, -10.6f, 0.94f);
        AddMiniMapPineCrayon(strokes, 21.2f, -12.8f, 1.08f);
        AddMiniMapPineCrayon(strokes, 29.0f, -5.8f, 0.96f);
        AddMiniMapPineCrayon(strokes, 29.2f, -18.0f, 1.0f);
    }

    private static void AddMiniMapForestGrassGreenColor(List<Vector2[]> strokes)
    {
        AddMiniMapGrassCrayon(strokes, -31.0f, -0.8f, 0.84f);
        AddMiniMapGrassCrayon(strokes, -24.0f, -17.4f, 0.86f);
        AddMiniMapGrassCrayon(strokes, -13.4f, -5.6f, 0.78f);
        AddMiniMapGrassCrayon(strokes, -4.4f, -8.8f, 0.72f);
        AddMiniMapGrassCrayon(strokes, 4.2f, 5.8f, 0.76f);
        AddMiniMapGrassCrayon(strokes, 13.6f, -4.8f, 0.82f);
        AddMiniMapGrassCrayon(strokes, 22.8f, 0.8f, 0.74f);
        AddMiniMapGrassCrayon(strokes, 30.0f, 10.2f, 0.86f);
    }

    private static void AddMiniMapFence(List<Vector2[]> strokes)
    {
        AddMapRect(strokes, 0f, 0f, 34.5f, 23.5f);
        AddMapRect(strokes, 0f, 0f, 32.8f, 21.8f);

        for (float x = -30f; x <= 30f; x += 7.5f)
        {
            AddMapRect(strokes, x, 23.45f, 0.42f, 1.35f);
            AddMapRect(strokes, x, -23.45f, 0.42f, 1.35f);
        }

        for (float y = -18f; y <= 18f; y += 6f)
        {
            AddMapRect(strokes, -34.45f, y, 1.35f, 0.42f);
            AddMapRect(strokes, 34.45f, y, 1.35f, 0.42f);
        }
    }

    private static void AddMiniMapPaths(List<Vector2[]> strokes)
    {
        strokes.Add(MapPoints(-1.2f, 0.4f, 5.8f, -1.2f, 11.8f, -5.7f, 17.6f, -10.8f, 21.5f, -14.2f));
        strokes.Add(MapPoints(-1.6f, 0.1f, -7.8f, -3.8f, -14.8f, -8.4f, -18.4f, -11.75f));
        strokes.Add(MapPoints(2.2f, 1.3f, 7.4f, 3.7f, 12.4f, 4.0f, 17.2f, 3.0f));
        strokes.Add(MapPoints(-2.8f, 3.7f, -8.6f, 8.2f, -13.4f, 12.2f));
    }

    private static void AddMiniMapHouse(List<Vector2[]> strokes, float centerX, float centerY, float scale)
    {
        float halfWidth = scale * 0.48f;
        float bodyBottom = centerY - scale * 0.32f;
        float bodyTop = centerY + scale * 0.18f;
        float roofTop = centerY + scale * 0.62f;

        strokes.Add(MapPoints(centerX - halfWidth, bodyBottom, centerX - halfWidth, bodyTop, centerX + halfWidth, bodyTop, centerX + halfWidth, bodyBottom, centerX - halfWidth, bodyBottom));
        strokes.Add(MapPoints(centerX - halfWidth * 1.12f, bodyTop, centerX, roofTop, centerX + halfWidth * 1.12f, bodyTop));
        strokes.Add(MapPoints(centerX - scale * 0.1f, bodyBottom, centerX - scale * 0.1f, centerY, centerX + scale * 0.1f, centerY, centerX + scale * 0.1f, bodyBottom));
        AddMapRect(strokes, centerX - scale * 0.28f, centerY + scale * 0.02f, scale * 0.1f, scale * 0.08f);
        AddMapRect(strokes, centerX + scale * 0.28f, centerY + scale * 0.02f, scale * 0.1f, scale * 0.08f);
        strokes.Add(MapPoints(centerX - halfWidth, bodyBottom - scale * 0.08f, centerX - scale * 0.18f, bodyBottom - scale * 0.02f, centerX + scale * 0.18f, bodyBottom - scale * 0.08f, centerX + halfWidth, bodyBottom - scale * 0.02f));
    }

    private static void AddMiniMapPond(List<Vector2[]> strokes, float centerX, float centerY)
    {
        strokes.Add(MapEllipsePoints(centerX, centerY, 5.25f, 2.85f, 34));
        strokes.Add(MapEllipsePoints(centerX + 0.35f, centerY + 0.04f, 4.2f, 2.0f, 28));
        strokes.Add(MapPoints(centerX - 3.5f, centerY + 0.4f, centerX - 2.0f, centerY + 0.66f, centerX - 0.7f, centerY + 0.44f));
        strokes.Add(MapPoints(centerX + 0.4f, centerY - 0.35f, centerX + 1.8f, centerY - 0.12f, centerX + 3.0f, centerY - 0.36f));
        strokes.Add(MapPoints(centerX + 4.55f, centerY + 1.05f, centerX + 3.15f, centerY + 2.4f, centerX + 1.95f, centerY + 3.05f));
        strokes.Add(MapPoints(centerX + 4.55f, centerY + 1.05f, centerX + 5.0f, centerY + 0.55f));
    }

    private static void AddMiniMapMailbox(List<Vector2[]> strokes, float centerX, float centerY)
    {
        strokes.Add(MapPoints(centerX, centerY - 0.8f, centerX, centerY + 0.25f));
        strokes.Add(MapPoints(centerX - 0.48f, centerY + 0.2f, centerX - 0.48f, centerY + 0.72f, centerX + 0.55f, centerY + 0.72f, centerX + 0.55f, centerY + 0.2f, centerX - 0.48f, centerY + 0.2f));
        strokes.Add(MapPoints(centerX + 0.22f, centerY + 0.72f, centerX + 0.44f, centerY + 1.04f));
    }

    private static void AddMiniMapNpc(List<Vector2[]> strokes, float centerX, float centerY)
    {
        strokes.Add(MapEllipsePoints(centerX, centerY + 0.48f, 0.42f, 0.45f, 12));
        strokes.Add(MapPoints(centerX, centerY + 0.04f, centerX, centerY - 0.82f));
        strokes.Add(MapPoints(centerX - 0.5f, centerY - 0.24f, centerX + 0.5f, centerY - 0.24f));
        strokes.Add(MapPoints(centerX - 0.28f, centerY - 0.82f, centerX - 0.52f, centerY - 1.28f));
        strokes.Add(MapPoints(centerX + 0.28f, centerY - 0.82f, centerX + 0.52f, centerY - 1.28f));
    }

    private static void AddMiniMapForestEntrance(List<Vector2[]> strokes, float centerX, float centerY)
    {
        strokes.Add(MapPoints(centerX - 2.7f, centerY - 1.1f, centerX - 1.5f, centerY - 0.7f, centerX - 0.2f, centerY - 1.0f, centerX + 1.2f, centerY - 0.65f, centerX + 2.7f, centerY - 1.05f));
        AddMiniMapPine(strokes, centerX - 1.8f, centerY - 1.1f, 0.95f);
        AddMiniMapPine(strokes, centerX, centerY - 1.05f, 1.1f);
        AddMiniMapPine(strokes, centerX + 1.8f, centerY - 1.1f, 0.95f);
    }

    private static void AddMiniMapTrees(List<Vector2[]> strokes)
    {
        AddMiniMapPine(strokes, -28.0f, 15.5f, 1.0f);
        AddMiniMapPine(strokes, -20.5f, 7.4f, 0.86f);
        AddMiniMapPine(strokes, -15.8f, 16.8f, 1.12f);
        AddMiniMapPine(strokes, -6.4f, 14.8f, 0.92f);
        AddMiniMapPine(strokes, 8.8f, 16.2f, 1.02f);
        AddMiniMapPine(strokes, 26.6f, 13.4f, 0.94f);
        AddMiniMapPine(strokes, 30.2f, 6.6f, 0.84f);
        AddMiniMapPine(strokes, -30.0f, -2.8f, 0.9f);
        AddMiniMapPine(strokes, -12.0f, -10.2f, 0.82f);
        AddMiniMapPine(strokes, 6.2f, -13.8f, 0.9f);
        AddMiniMapPine(strokes, 12.0f, -18.0f, 0.86f);
        AddMiniMapPine(strokes, 29.2f, -7.8f, 0.92f);
    }

    private static void AddMiniMapGrass(List<Vector2[]> strokes)
    {
        AddMiniMapGrassTuft(strokes, -27.6f, 3.8f, 0.86f);
        AddMiniMapGrassTuft(strokes, -18.8f, -3.2f, 0.78f);
        AddMiniMapGrassTuft(strokes, -9.4f, -16.4f, 0.9f);
        AddMiniMapGrassTuft(strokes, -2.6f, -7.8f, 0.7f);
        AddMiniMapGrassTuft(strokes, 7.6f, -6.8f, 0.82f);
        AddMiniMapGrassTuft(strokes, 13.2f, 1.2f, 0.7f);
        AddMiniMapGrassTuft(strokes, 21.0f, 10.6f, 0.78f);
        AddMiniMapGrassTuft(strokes, 27.4f, -15.2f, 0.86f);
    }

    private static void AddMiniMapPine(List<Vector2[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(MapLocalPoints(centerX, groundY, scale, 0f, 2.3f, -0.65f, 1.45f, -0.38f, 1.45f, -0.9f, 0.72f, -0.48f, 0.72f, -1.05f, 0f, 1.05f, 0f, 0.48f, 0.72f, 0.9f, 0.72f, 0.38f, 1.45f, 0.65f, 1.45f, 0f, 2.3f));
        strokes.Add(MapLocalPoints(centerX, groundY, scale, -0.12f, 0f, -0.12f, -0.34f, 0.12f, -0.34f, 0.12f, 0f));
        strokes.Add(MapLocalPoints(centerX, groundY, scale, -0.48f, 0.72f, 0f, 1.04f, 0.48f, 0.72f));
    }

    private static void AddMiniMapPineCrayon(List<Vector2[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(MapLocalPoints(centerX, groundY, scale, -0.18f, 2.05f, 0.16f, 2.05f));
        strokes.Add(MapLocalPoints(centerX, groundY, scale, -0.52f, 1.55f, -0.08f, 1.72f, 0.46f, 1.52f));
        strokes.Add(MapLocalPoints(centerX, groundY, scale, -0.76f, 1.06f, -0.24f, 1.24f, 0.28f, 1.06f, 0.78f, 1.2f));
        strokes.Add(MapLocalPoints(centerX, groundY, scale, -0.98f, 0.48f, -0.48f, 0.7f, 0.06f, 0.52f, 0.58f, 0.72f, 1.0f, 0.48f));
        strokes.Add(MapLocalPoints(centerX, groundY, scale, -0.88f, 0.12f, -0.36f, 0.26f, 0.14f, 0.08f, 0.72f, 0.22f));
    }

    private static void AddMiniMapGrassTuft(List<Vector2[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(MapLocalPoints(centerX, groundY, scale, -0.8f, 0f, -0.42f, 0.7f, -0.1f, 0f, 0.28f, 0.86f, 0.56f, 0f, 0.92f, 0.62f));
    }

    private static void AddMiniMapGrassCrayon(List<Vector2[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(MapLocalPoints(centerX, groundY, scale, -0.64f, 0.08f, -0.28f, 0.48f, 0.02f, 0.1f, 0.34f, 0.56f, 0.68f, 0.12f));
        strokes.Add(MapLocalPoints(centerX, groundY, scale, -0.46f, 0.02f, -0.1f, 0.36f, 0.18f, 0.02f, 0.48f, 0.32f));
    }

    private static void AddMapRect(List<Vector2[]> strokes, float centerX, float centerY, float halfWidth, float halfHeight)
    {
        strokes.Add(MapPoints(
            centerX - halfWidth, centerY - halfHeight,
            centerX - halfWidth, centerY + halfHeight,
            centerX + halfWidth, centerY + halfHeight,
            centerX + halfWidth, centerY - halfHeight,
            centerX - halfWidth, centerY - halfHeight));
    }

    private static Vector2[] MapPoints(params float[] values)
    {
        int pointCount = values.Length / 2;
        Vector2[] points = new Vector2[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = MapPoint(values[i * 2], values[i * 2 + 1]);
        }

        return points;
    }

    private static Vector2[] MapLocalPoints(float centerX, float centerY, float scale, params float[] values)
    {
        int pointCount = values.Length / 2;
        Vector2[] points = new Vector2[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = MapPoint(
                centerX + values[i * 2] * scale,
                centerY + values[i * 2 + 1] * scale);
        }

        return points;
    }

    private static Vector2[] MapEllipsePoints(float centerX, float centerY, float radiusX, float radiusY, int segmentCount)
    {
        Vector2[] points = new Vector2[segmentCount + 1];

        for (int i = 0; i <= segmentCount; i++)
        {
            float angle = Mathf.PI * 2f * i / segmentCount;
            points[i] = MapPoint(
                centerX + Mathf.Cos(angle) * radiusX,
                centerY + Mathf.Sin(angle) * radiusY);
        }

        return points;
    }

    private static Vector2 MapPoint(float x, float y)
    {
        const float mapScale = 8f;
        return new Vector2(x * mapScale, y * mapScale);
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
