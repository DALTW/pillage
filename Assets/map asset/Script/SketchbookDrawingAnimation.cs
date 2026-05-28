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
    private const float BrownCrayonColorDuration = 6.2f;
    private const float BlueCrayonColorDuration = 5.4f;
    private const float GreenCrayonLengthScale = 0.82f;
    private const float BrownCrayonLengthScale = 0.82f;
    private const float BlueCrayonLengthScale = 0.82f;
    private const float DeepForestPanelWidthScale = 0.5f;
    private const float SketchbookMapLayoutUnits = 3.5f;
    private const float DefaultSketchbookWidth = 1200f;
    private const float FirstPageDrawableWidth = 1200f;
    private const float RequestedDrawingHeight = 540f;
    private const float DrawingHorizontalPadding = 28f;
    private const float DrawingVerticalPadding = 42f;
    private static readonly Color GreenCrayonColor = new Color32(74, 166, 73, 235);
    private static readonly Color BrownCrayonColor = new Color32(148, 88, 43, 235);
    private static readonly Color BlueCrayonColor = new Color32(50, 142, 220, 235);
    private static readonly Vector3[] MiniMapDeepForestTreePlacements =
    {
        new Vector3(-14.0f, 18.2f, 1.02f),
        new Vector3(-8.8f, 17.4f, 0.94f),
        new Vector3(-3.2f, 18.6f, 1.08f),
        new Vector3(3.4f, 17.6f, 0.98f),
        new Vector3(9.4f, 18.4f, 1.06f),
        new Vector3(14.6f, 16.2f, 0.92f),
        new Vector3(-9.0f, 10.4f, 1.08f),
        new Vector3(-2.2f, 11.8f, 0.96f),
        new Vector3(4.4f, 10.2f, 1.02f),
        new Vector3(11.6f, 9.8f, 0.94f),
        new Vector3(-3.8f, 3.8f, 1.0f),
        new Vector3(3.0f, 3.2f, 0.92f),
        new Vector3(9.8f, 2.8f, 1.04f),
        new Vector3(14.2f, 4.6f, 0.88f),
        new Vector3(-13.8f, -6.2f, 0.9f),
        new Vector3(-7.0f, -7.8f, 1.06f),
        new Vector3(-0.4f, -6.2f, 0.94f),
        new Vector3(6.2f, -7.6f, 1.04f),
        new Vector3(13.6f, -6.8f, 0.96f),
        new Vector3(-14.4f, -15.8f, 1.0f),
        new Vector3(-8.0f, -17.6f, 0.92f),
        new Vector3(-1.4f, -16.2f, 1.08f),
        new Vector3(5.4f, -17.8f, 0.98f),
        new Vector3(12.6f, -15.6f, 1.04f),
    };
    private static readonly Vector3[] MiniMapFourthForestTreePlacements =
    {
        new Vector3(-29.5f, 18.4f, 1.08f),
        new Vector3(-22.0f, 15.8f, 0.96f),
        new Vector3(-13.8f, 18.8f, 1.04f),
        new Vector3(-5.8f, 17.2f, 0.92f),
        new Vector3(8.6f, 17.6f, 0.98f),
        new Vector3(17.0f, 18.4f, 1.08f),
        new Vector3(26.8f, 15.6f, 0.96f),
        new Vector3(-30.2f, 9.6f, 1.02f),
        new Vector3(-21.8f, 7.0f, 0.94f),
        new Vector3(-12.4f, 8.4f, 1.04f),
        new Vector3(12.8f, 7.6f, 0.98f),
        new Vector3(22.2f, 6.6f, 1.06f),
        new Vector3(30.0f, 8.8f, 0.92f),
        new Vector3(-29.0f, -1.2f, 0.98f),
        new Vector3(-20.2f, -3.8f, 1.06f),
        new Vector3(19.8f, -3.4f, 1.04f),
        new Vector3(29.2f, -1.0f, 0.96f),
        new Vector3(-27.8f, -12.2f, 1.04f),
        new Vector3(-18.4f, -15.8f, 0.96f),
        new Vector3(-8.8f, -17.4f, 1.06f),
        new Vector3(7.8f, -17.2f, 1.0f),
        new Vector3(17.6f, -15.2f, 0.94f),
        new Vector3(28.0f, -12.0f, 1.08f),
    };

    [SerializeField] private RectTransform drawingRoot;
    [SerializeField] private bool playOnEnable;
    [SerializeField] private float startDelay = 0.12f;
    [SerializeField] private float drawDuration = RequestedDrawDuration;
    [SerializeField] private float pencilExitDuration = 0.28f;
    [SerializeField] private Vector2 drawingSize = new Vector2(DefaultSketchbookWidth / SketchbookMapLayoutUnits, RequestedDrawingHeight);
    [SerializeField] private Vector2 drawingOffset = new Vector2(0f, -20f);
    [SerializeField] private float lineWidth = 5f;
    [SerializeField] private Color lineColor = new Color32(34, 32, 29, 255);
    [SerializeField] private Vector2 pencilSize = new Vector2(RequestedPencilWidth, RequestedPencilHeight);
    [SerializeField] private float pencilStartLengthScale = RequestedPencilStartLengthScale;
    [SerializeField] private float pencilEndLengthScale = RequestedPencilEndLengthScale;

    private SketchbookLineDrawingGraphic drawingGraphic;
    private RectTransform villageGreenDrawingRoot;
    private SketchbookLineDrawingGraphic villageGreenDrawingGraphic;
    private RectTransform villageBrownDrawingRoot;
    private SketchbookLineDrawingGraphic villageBrownDrawingGraphic;
    private RectTransform villageBlueWaterDrawingRoot;
    private SketchbookLineDrawingGraphic villageBlueWaterDrawingGraphic;
    private RectTransform forestDrawingRoot;
    private SketchbookLineDrawingGraphic forestDrawingGraphic;
    private RectTransform forestGreenDrawingRoot;
    private SketchbookLineDrawingGraphic forestGreenDrawingGraphic;
    private RectTransform forestBrownDrawingRoot;
    private SketchbookLineDrawingGraphic forestBrownDrawingGraphic;
    private RectTransform forestBlueWaterDrawingRoot;
    private SketchbookLineDrawingGraphic forestBlueWaterDrawingGraphic;
    private RectTransform deepForestDrawingRoot;
    private SketchbookLineDrawingGraphic deepForestDrawingGraphic;
    private RectTransform deepForestGreenDrawingRoot;
    private SketchbookLineDrawingGraphic deepForestGreenDrawingGraphic;
    private RectTransform deepForestBrownDrawingRoot;
    private SketchbookLineDrawingGraphic deepForestBrownDrawingGraphic;
    private RectTransform fourthForestDrawingRoot;
    private SketchbookLineDrawingGraphic fourthForestDrawingGraphic;
    private RectTransform fourthForestGreenDrawingRoot;
    private SketchbookLineDrawingGraphic fourthForestGreenDrawingGraphic;
    private RectTransform fourthForestBrownDrawingRoot;
    private SketchbookLineDrawingGraphic fourthForestBrownDrawingGraphic;
    private RectTransform fourthForestBlueWaterDrawingRoot;
    private SketchbookLineDrawingGraphic fourthForestBlueWaterDrawingGraphic;
    private SketchbookLineDrawingGraphic activeDrawingGraphic;
    private RectTransform pencilRoot;
    private CanvasGroup pencilCanvasGroup;
    private Coroutine animationRoutine;
    private Action animationCompleteCallback;
    private bool isSetup;
    private float activeDrawingScale = 1f;
    private float villageDrawingScale = 1f;
    private float villageGreenDrawingScale = 1f;
    private float villageBrownDrawingScale = 1f;
    private float villageBlueWaterDrawingScale = 1f;
    private float forestDrawingScale = 1f;
    private float forestGreenDrawingScale = 1f;
    private float forestBrownDrawingScale = 1f;
    private float forestBlueWaterDrawingScale = 1f;
    private float deepForestDrawingScale = 1f;
    private float deepForestGreenDrawingScale = 1f;
    private float deepForestBrownDrawingScale = 1f;
    private float fourthForestDrawingScale = 1f;
    private float fourthForestGreenDrawingScale = 1f;
    private float fourthForestBrownDrawingScale = 1f;
    private float fourthForestBlueWaterDrawingScale = 1f;

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

    public void PlayDeepForestExpansion(Action onComplete = null)
    {
        ApplyRequestedAnimationSettings();
        EnsureSetup();
        StopAnimation();
        activeDrawingGraphic = deepForestDrawingGraphic;
        activeDrawingScale = deepForestDrawingScale;
        pencilStartLengthScale = ExtendedPencilStartLengthScale;
        pencilEndLengthScale = ExtendedPencilEndLengthScale;
        animationCompleteCallback = onComplete;
        animationRoutine = StartCoroutine(PlayDeepForestExpansionRoutine());
    }

    public void PlayFourthForestExpansion(Action onComplete = null)
    {
        ApplyRequestedAnimationSettings();
        EnsureSetup();
        StopAnimation();
        activeDrawingGraphic = fourthForestDrawingGraphic;
        activeDrawingScale = fourthForestDrawingScale;
        pencilStartLengthScale = ExtendedPencilStartLengthScale;
        pencilEndLengthScale = ExtendedPencilEndLengthScale;
        animationCompleteCallback = onComplete;
        animationRoutine = StartCoroutine(PlayFourthForestExpansionRoutine());
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

    public void PlayBrownColoring(Action onComplete = null)
    {
        ApplyRequestedAnimationSettings();
        EnsureSetup();
        StopAnimation();
        activeDrawingGraphic = villageBrownDrawingGraphic;
        activeDrawingScale = villageBrownDrawingScale;
        pencilStartLengthScale = BrownCrayonLengthScale;
        pencilEndLengthScale = BrownCrayonLengthScale;
        animationCompleteCallback = onComplete;
        animationRoutine = StartCoroutine(PlayBrownColoringRoutine());
    }

    public void PlayBlueWaterColoring(Action onComplete = null)
    {
        ApplyRequestedAnimationSettings();
        EnsureSetup();
        StopAnimation();
        activeDrawingGraphic = villageBlueWaterDrawingGraphic;
        activeDrawingScale = villageBlueWaterDrawingScale;
        pencilStartLengthScale = BlueCrayonLengthScale;
        pencilEndLengthScale = BlueCrayonLengthScale;
        animationCompleteCallback = onComplete;
        animationRoutine = StartCoroutine(PlayBlueWaterColoringRoutine());
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

        if (villageBrownDrawingGraphic != null)
        {
            villageBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails ? 1f : 0f;
        }

        if (villageBlueWaterDrawingGraphic != null)
        {
            villageBlueWaterDrawingGraphic.RevealProgress = GameProgress.HasColoredWaterBlue ? 1f : 0f;
        }

        if (forestDrawingGraphic != null)
        {
            forestDrawingGraphic.RevealProgress = GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (forestGreenDrawingGraphic != null)
        {
            forestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredForestGreen ? 1f : 0f;
        }

        if (forestBrownDrawingGraphic != null)
        {
            forestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (forestBlueWaterDrawingGraphic != null)
        {
            forestBlueWaterDrawingGraphic.RevealProgress = GameProgress.HasColoredWaterBlue && GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (deepForestDrawingGraphic != null)
        {
            deepForestDrawingGraphic.RevealProgress = GameProgress.HasDrawnDeepForestSketch ? 1f : 0f;
        }

        if (deepForestGreenDrawingGraphic != null)
        {
            deepForestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredDeepForestGreen ? 1f : 0f;
        }

        if (deepForestBrownDrawingGraphic != null)
        {
            deepForestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnDeepForestSketch ? 1f : 0f;
        }

        if (fourthForestDrawingGraphic != null)
        {
            fourthForestDrawingGraphic.RevealProgress = GameProgress.HasDrawnFourthForestSketch ? 1f : 0f;
        }

        if (fourthForestGreenDrawingGraphic != null)
        {
            fourthForestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredFourthForestGreen ? 1f : 0f;
        }

        if (fourthForestBrownDrawingGraphic != null)
        {
            fourthForestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnFourthForestSketch ? 1f : 0f;
        }

        if (fourthForestBlueWaterDrawingGraphic != null)
        {
            fourthForestBlueWaterDrawingGraphic.RevealProgress = GameProgress.HasColoredWaterBlue && GameProgress.HasDrawnFourthForestSketch ? 1f : 0f;
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

        if (forestBrownDrawingGraphic != null)
        {
            forestBrownDrawingGraphic.RevealProgress = 0f;
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

        if (GameProgress.HasColoredBrownDetails && forestBrownDrawingGraphic != null)
        {
            yield return PlayBrownColoringOnTarget(
                forestBrownDrawingGraphic,
                forestDrawingRoot,
                forestBrownDrawingScale);
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

    private IEnumerator PlayDeepForestExpansionRoutine()
    {
        drawingGraphic.RevealProgress = 1f;
        forestDrawingGraphic.RevealProgress = GameProgress.HasDrawnForestSketch ? 1f : 0f;
        deepForestDrawingGraphic.RevealProgress = 0f;

        if (villageGreenDrawingGraphic != null)
        {
            villageGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredVillageGreen ? 1f : 0f;
        }
        if (forestGreenDrawingGraphic != null)
        {
            forestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredForestGreen ? 1f : 0f;
        }
        if (deepForestGreenDrawingGraphic != null)
        {
            deepForestGreenDrawingGraphic.RevealProgress = 0f;
        }

        if (villageBrownDrawingGraphic != null)
        {
            villageBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails ? 1f : 0f;
        }
        if (forestBrownDrawingGraphic != null)
        {
            forestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }
        if (deepForestBrownDrawingGraphic != null)
        {
            deepForestBrownDrawingGraphic.RevealProgress = 0f;
        }

        SetPencilParent(deepForestDrawingRoot);
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

            deepForestDrawingGraphic.RevealProgress = revealProgress;
            SetPencilAtProgress(revealProgress, elapsed, true);
            yield return null;
        }

        deepForestDrawingGraphic.RevealProgress = 1f;
        SetPencilAtProgress(1f);

        bool shouldApplyGreen = GameProgress.HasColoredVillageGreen || GameProgress.HasCollectedGreenCrayon;
        if (shouldApplyGreen)
        {
            if (!GameProgress.HasColoredVillageGreen && villageGreenDrawingGraphic != null)
            {
                yield return PlayGreenColoringOnTarget(villageGreenDrawingGraphic, drawingRoot, villageGreenDrawingScale);
            }

            if (!GameProgress.HasColoredForestGreen && forestGreenDrawingGraphic != null)
            {
                yield return PlayGreenColoringOnTarget(forestGreenDrawingGraphic, forestDrawingRoot, forestGreenDrawingScale);
            }

            if (deepForestGreenDrawingGraphic != null)
            {
                yield return PlayGreenColoringOnTarget(deepForestGreenDrawingGraphic, deepForestDrawingRoot, deepForestGreenDrawingScale);
            }
        }

        bool shouldApplyBrown = GameProgress.HasColoredBrownDetails || GameProgress.HasCollectedBrownCrayon;
        if (shouldApplyBrown)
        {
            if (!GameProgress.HasColoredBrownDetails && villageBrownDrawingGraphic != null)
            {
                yield return PlayBrownColoringOnTarget(villageBrownDrawingGraphic, drawingRoot, villageBrownDrawingScale);
            }

            if (!GameProgress.HasColoredBrownDetails && forestBrownDrawingGraphic != null)
            {
                yield return PlayBrownColoringOnTarget(forestBrownDrawingGraphic, forestDrawingRoot, forestBrownDrawingScale);
            }

            if (deepForestBrownDrawingGraphic != null)
            {
                yield return PlayBrownColoringOnTarget(deepForestBrownDrawingGraphic, deepForestDrawingRoot, deepForestBrownDrawingScale);
            }
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

    private IEnumerator PlayFourthForestExpansionRoutine()
    {
        drawingGraphic.RevealProgress = 1f;
        forestDrawingGraphic.RevealProgress = GameProgress.HasDrawnForestSketch ? 1f : 0f;
        deepForestDrawingGraphic.RevealProgress = GameProgress.HasDrawnDeepForestSketch ? 1f : 0f;
        fourthForestDrawingGraphic.RevealProgress = 0f;

        if (villageGreenDrawingGraphic != null)
        {
            villageGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredVillageGreen ? 1f : 0f;
        }
        if (forestGreenDrawingGraphic != null)
        {
            forestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredForestGreen ? 1f : 0f;
        }
        if (deepForestGreenDrawingGraphic != null)
        {
            deepForestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredDeepForestGreen ? 1f : 0f;
        }
        if (fourthForestGreenDrawingGraphic != null)
        {
            fourthForestGreenDrawingGraphic.RevealProgress = 0f;
        }

        if (villageBrownDrawingGraphic != null)
        {
            villageBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails ? 1f : 0f;
        }
        if (forestBrownDrawingGraphic != null)
        {
            forestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }
        if (deepForestBrownDrawingGraphic != null)
        {
            deepForestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnDeepForestSketch ? 1f : 0f;
        }
        if (fourthForestBrownDrawingGraphic != null)
        {
            fourthForestBrownDrawingGraphic.RevealProgress = 0f;
        }

        SetPencilParent(fourthForestDrawingRoot);
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

            fourthForestDrawingGraphic.RevealProgress = revealProgress;
            SetPencilAtProgress(revealProgress, elapsed, true);
            yield return null;
        }

        fourthForestDrawingGraphic.RevealProgress = 1f;
        SetPencilAtProgress(1f);

        bool shouldApplyGreen = GameProgress.HasColoredVillageGreen || GameProgress.HasCollectedGreenCrayon;
        if (shouldApplyGreen)
        {
            if (!GameProgress.HasColoredVillageGreen && villageGreenDrawingGraphic != null)
            {
                yield return PlayGreenColoringOnTarget(villageGreenDrawingGraphic, drawingRoot, villageGreenDrawingScale);
            }

            if (!GameProgress.HasColoredForestGreen && forestGreenDrawingGraphic != null)
            {
                yield return PlayGreenColoringOnTarget(forestGreenDrawingGraphic, forestDrawingRoot, forestGreenDrawingScale);
            }

            if (!GameProgress.HasColoredDeepForestGreen && deepForestGreenDrawingGraphic != null)
            {
                yield return PlayGreenColoringOnTarget(deepForestGreenDrawingGraphic, deepForestDrawingRoot, deepForestGreenDrawingScale);
            }

            if (fourthForestGreenDrawingGraphic != null)
            {
                yield return PlayGreenColoringOnTarget(fourthForestGreenDrawingGraphic, fourthForestDrawingRoot, fourthForestGreenDrawingScale);
            }
        }

        bool shouldApplyBrown = GameProgress.HasColoredBrownDetails || GameProgress.HasCollectedBrownCrayon;
        if (shouldApplyBrown)
        {
            if (!GameProgress.HasColoredBrownDetails && villageBrownDrawingGraphic != null)
            {
                yield return PlayBrownColoringOnTarget(villageBrownDrawingGraphic, drawingRoot, villageBrownDrawingScale);
            }

            if (!GameProgress.HasColoredBrownDetails && forestBrownDrawingGraphic != null)
            {
                yield return PlayBrownColoringOnTarget(forestBrownDrawingGraphic, forestDrawingRoot, forestBrownDrawingScale);
            }

            if (!GameProgress.HasColoredBrownDetails && deepForestBrownDrawingGraphic != null)
            {
                yield return PlayBrownColoringOnTarget(deepForestBrownDrawingGraphic, deepForestDrawingRoot, deepForestBrownDrawingScale);
            }

            if (fourthForestBrownDrawingGraphic != null)
            {
                yield return PlayBrownColoringOnTarget(fourthForestBrownDrawingGraphic, fourthForestDrawingRoot, fourthForestBrownDrawingScale);
            }
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

    private IEnumerator PlayVillageGreenColoringRoutine()
    {
        drawingGraphic.RevealProgress = 1f;
        bool shouldColorVillage = !GameProgress.HasColoredVillageGreen;
        bool shouldColorForest = GameProgress.HasDrawnForestSketch && !GameProgress.HasColoredForestGreen;
        bool shouldColorFourthForest = GameProgress.HasDrawnFourthForestSketch && !GameProgress.HasColoredFourthForestGreen;
        villageGreenDrawingGraphic.RevealProgress = shouldColorVillage ? 0f : 1f;
        if (villageBrownDrawingGraphic != null)
        {
            villageBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails ? 1f : 0f;
        }

        if (villageBlueWaterDrawingGraphic != null)
        {
            villageBlueWaterDrawingGraphic.RevealProgress = GameProgress.HasColoredWaterBlue ? 1f : 0f;
        }

        if (forestDrawingGraphic != null)
        {
            forestDrawingGraphic.RevealProgress = GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (forestGreenDrawingGraphic != null)
        {
            forestGreenDrawingGraphic.RevealProgress = shouldColorForest ? 0f : (GameProgress.HasColoredForestGreen ? 1f : 0f);
        }

        if (forestBrownDrawingGraphic != null)
        {
            forestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (forestBlueWaterDrawingGraphic != null)
        {
            forestBlueWaterDrawingGraphic.RevealProgress = GameProgress.HasColoredWaterBlue && GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (deepForestDrawingGraphic != null)
        {
            deepForestDrawingGraphic.RevealProgress = GameProgress.HasDrawnDeepForestSketch ? 1f : 0f;
        }

        if (deepForestGreenDrawingGraphic != null)
        {
            deepForestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredDeepForestGreen ? 1f : 0f;
        }

        if (deepForestBrownDrawingGraphic != null)
        {
            deepForestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnDeepForestSketch ? 1f : 0f;
        }

        if (fourthForestDrawingGraphic != null)
        {
            fourthForestDrawingGraphic.RevealProgress = GameProgress.HasDrawnFourthForestSketch ? 1f : 0f;
        }

        if (fourthForestGreenDrawingGraphic != null)
        {
            fourthForestGreenDrawingGraphic.RevealProgress = shouldColorFourthForest ? 0f : (GameProgress.HasColoredFourthForestGreen ? 1f : 0f);
        }

        if (fourthForestBrownDrawingGraphic != null)
        {
            fourthForestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnFourthForestSketch ? 1f : 0f;
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

        if (GameProgress.HasDrawnDeepForestSketch && !GameProgress.HasColoredDeepForestGreen && deepForestGreenDrawingGraphic != null)
        {
            yield return PlayGreenColoringOnTarget(
                deepForestGreenDrawingGraphic,
                deepForestDrawingRoot,
                deepForestGreenDrawingScale);
        }

        if (shouldColorFourthForest && fourthForestGreenDrawingGraphic != null)
        {
            yield return PlayGreenColoringOnTarget(
                fourthForestGreenDrawingGraphic,
                fourthForestDrawingRoot,
                fourthForestGreenDrawingScale);
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

    private IEnumerator PlayBrownColoringRoutine()
    {
        drawingGraphic.RevealProgress = 1f;
        bool shouldColorVillage = !GameProgress.HasColoredBrownDetails;
        bool shouldColorForest = GameProgress.HasDrawnForestSketch && !GameProgress.HasColoredBrownDetails;
        bool shouldColorDeepForest = GameProgress.HasDrawnDeepForestSketch && !GameProgress.HasColoredBrownDetails;
        bool shouldColorFourthForest = GameProgress.HasDrawnFourthForestSketch && !GameProgress.HasColoredBrownDetails;

        if (villageGreenDrawingGraphic != null)
        {
            villageGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredVillageGreen ? 1f : 0f;
        }

        if (villageBrownDrawingGraphic != null)
        {
            villageBrownDrawingGraphic.RevealProgress = shouldColorVillage ? 0f : 1f;
        }

        if (forestDrawingGraphic != null)
        {
            forestDrawingGraphic.RevealProgress = GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (forestGreenDrawingGraphic != null)
        {
            forestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredForestGreen ? 1f : 0f;
        }

        if (forestBrownDrawingGraphic != null)
        {
            forestBrownDrawingGraphic.RevealProgress = shouldColorForest ? 0f : (GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnForestSketch ? 1f : 0f);
        }

        if (deepForestDrawingGraphic != null)
        {
            deepForestDrawingGraphic.RevealProgress = GameProgress.HasDrawnDeepForestSketch ? 1f : 0f;
        }

        if (deepForestGreenDrawingGraphic != null)
        {
            deepForestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredDeepForestGreen ? 1f : 0f;
        }

        if (deepForestBrownDrawingGraphic != null)
        {
            deepForestBrownDrawingGraphic.RevealProgress = shouldColorDeepForest ? 0f : (GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnDeepForestSketch ? 1f : 0f);
        }

        if (fourthForestDrawingGraphic != null)
        {
            fourthForestDrawingGraphic.RevealProgress = GameProgress.HasDrawnFourthForestSketch ? 1f : 0f;
        }

        if (fourthForestGreenDrawingGraphic != null)
        {
            fourthForestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredFourthForestGreen ? 1f : 0f;
        }

        if (fourthForestBrownDrawingGraphic != null)
        {
            fourthForestBrownDrawingGraphic.RevealProgress = shouldColorFourthForest ? 0f : (GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnFourthForestSketch ? 1f : 0f);
        }

        if (shouldColorVillage && villageBrownDrawingGraphic != null)
        {
            yield return PlayBrownColoringOnTarget(
                villageBrownDrawingGraphic,
                drawingRoot,
                villageBrownDrawingScale);
        }

        if (shouldColorForest && forestBrownDrawingGraphic != null)
        {
            yield return PlayBrownColoringOnTarget(
                forestBrownDrawingGraphic,
                forestDrawingRoot,
                forestBrownDrawingScale);
        }

        if (shouldColorDeepForest && deepForestBrownDrawingGraphic != null)
        {
            yield return PlayBrownColoringOnTarget(
                deepForestBrownDrawingGraphic,
                deepForestDrawingRoot,
                deepForestBrownDrawingScale);
        }

        if (shouldColorFourthForest && fourthForestBrownDrawingGraphic != null)
        {
            yield return PlayBrownColoringOnTarget(
                fourthForestBrownDrawingGraphic,
                fourthForestDrawingRoot,
                fourthForestBrownDrawingScale);
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

    private IEnumerator PlayBlueWaterColoringRoutine()
    {
        drawingGraphic.RevealProgress = 1f;
        bool shouldColorVillage = !GameProgress.HasColoredWaterBlue;
        bool shouldColorForest = GameProgress.HasDrawnForestSketch && !GameProgress.HasColoredWaterBlue;
        bool shouldColorFourthForest = GameProgress.HasDrawnFourthForestSketch && !GameProgress.HasColoredWaterBlue;

        if (villageGreenDrawingGraphic != null)
        {
            villageGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredVillageGreen ? 1f : 0f;
        }

        if (villageBrownDrawingGraphic != null)
        {
            villageBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails ? 1f : 0f;
        }

        if (villageBlueWaterDrawingGraphic != null)
        {
            villageBlueWaterDrawingGraphic.RevealProgress = shouldColorVillage ? 0f : 1f;
        }

        if (forestDrawingGraphic != null)
        {
            forestDrawingGraphic.RevealProgress = GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (forestGreenDrawingGraphic != null)
        {
            forestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredForestGreen ? 1f : 0f;
        }

        if (forestBrownDrawingGraphic != null)
        {
            forestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (forestBlueWaterDrawingGraphic != null)
        {
            forestBlueWaterDrawingGraphic.RevealProgress = shouldColorForest ? 0f : (GameProgress.HasColoredWaterBlue && GameProgress.HasDrawnForestSketch ? 1f : 0f);
        }

        if (deepForestDrawingGraphic != null)
        {
            deepForestDrawingGraphic.RevealProgress = GameProgress.HasDrawnDeepForestSketch ? 1f : 0f;
        }

        if (deepForestGreenDrawingGraphic != null)
        {
            deepForestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredDeepForestGreen ? 1f : 0f;
        }

        if (deepForestBrownDrawingGraphic != null)
        {
            deepForestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnDeepForestSketch ? 1f : 0f;
        }

        if (fourthForestDrawingGraphic != null)
        {
            fourthForestDrawingGraphic.RevealProgress = GameProgress.HasDrawnFourthForestSketch ? 1f : 0f;
        }

        if (fourthForestGreenDrawingGraphic != null)
        {
            fourthForestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredFourthForestGreen ? 1f : 0f;
        }

        if (fourthForestBrownDrawingGraphic != null)
        {
            fourthForestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnFourthForestSketch ? 1f : 0f;
        }

        if (fourthForestBlueWaterDrawingGraphic != null)
        {
            fourthForestBlueWaterDrawingGraphic.RevealProgress = shouldColorFourthForest ? 0f : (GameProgress.HasColoredWaterBlue && GameProgress.HasDrawnFourthForestSketch ? 1f : 0f);
        }

        if (shouldColorVillage && villageBlueWaterDrawingGraphic != null)
        {
            yield return PlayBlueColoringOnTarget(
                villageBlueWaterDrawingGraphic,
                drawingRoot,
                villageBlueWaterDrawingScale);
        }

        if (shouldColorForest && forestBlueWaterDrawingGraphic != null)
        {
            yield return PlayBlueColoringOnTarget(
                forestBlueWaterDrawingGraphic,
                forestDrawingRoot,
                forestBlueWaterDrawingScale);
        }

        if (shouldColorFourthForest && fourthForestBlueWaterDrawingGraphic != null)
        {
            yield return PlayBlueColoringOnTarget(
                fourthForestBlueWaterDrawingGraphic,
                fourthForestDrawingRoot,
                fourthForestBlueWaterDrawingScale);
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

    private IEnumerator PlayBrownColoringOnTarget(SketchbookLineDrawingGraphic brownDrawingGraphic, RectTransform targetRoot, float targetScale)
    {
        if (brownDrawingGraphic == null || targetRoot == null)
        {
            yield break;
        }

        activeDrawingGraphic = brownDrawingGraphic;
        activeDrawingScale = targetScale;
        pencilStartLengthScale = BrownCrayonLengthScale;
        pencilEndLengthScale = BrownCrayonLengthScale;
        SetPencilParent(targetRoot);
        SetPencilTint(BrownCrayonColor);
        pencilCanvasGroup.alpha = 1f;
        pencilRoot.gameObject.SetActive(true);
        SetPencilLengthScale(BrownCrayonLengthScale);
        SetPencilAtProgress(0f);

        float elapsed = 0f;
        float revealProgress = 0f;

        while (elapsed < BrownCrayonColorDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / BrownCrayonColorDuration);
            revealProgress = Mathf.Max(revealProgress, ApplyCrayonColoringPace(progress));

            brownDrawingGraphic.RevealProgress = revealProgress;
            SetPencilAtProgress(revealProgress, elapsed, true);
            yield return null;
        }

        brownDrawingGraphic.RevealProgress = 1f;
        SetPencilAtProgress(1f);
    }

    private IEnumerator PlayBlueColoringOnTarget(SketchbookLineDrawingGraphic blueDrawingGraphic, RectTransform targetRoot, float targetScale)
    {
        if (blueDrawingGraphic == null || targetRoot == null)
        {
            yield break;
        }

        activeDrawingGraphic = blueDrawingGraphic;
        activeDrawingScale = targetScale;
        pencilStartLengthScale = BlueCrayonLengthScale;
        pencilEndLengthScale = BlueCrayonLengthScale;
        SetPencilParent(targetRoot);
        SetPencilTint(BlueCrayonColor);
        pencilCanvasGroup.alpha = 1f;
        pencilRoot.gameObject.SetActive(true);
        SetPencilLengthScale(BlueCrayonLengthScale);
        SetPencilAtProgress(0f);

        float elapsed = 0f;
        float revealProgress = 0f;

        while (elapsed < BlueCrayonColorDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / BlueCrayonColorDuration);
            revealProgress = Mathf.Max(revealProgress, ApplyCrayonColoringPace(progress));

            blueDrawingGraphic.RevealProgress = revealProgress;
            SetPencilAtProgress(revealProgress, elapsed, true);
            yield return null;
        }

        blueDrawingGraphic.RevealProgress = 1f;
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
        RectTransform fragmentParent = pencilRoot != null
            ? pencilRoot.parent as RectTransform
            : null;
        if (fragmentParent == null && activeDrawingGraphic != null)
        {
            fragmentParent = activeDrawingGraphic.transform as RectTransform;
        }
        if (fragmentParent == null)
        {
            fragmentParent = forestDrawingRoot;
        }

        SetPencilLengthScale(RequestedPencilEndLengthScale);
        pencilRoot.anchoredPosition = targetPosition;

        for (int i = 0; i < fragments.Length; i++)
        {
            GameObject fragmentObject = new GameObject($"GeneratedMergingPencilFragment_{i:00}", typeof(RectTransform));
            fragmentObject.transform.SetParent(fragmentParent, false);

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
        ConfigureMapDrawingRoot(drawingRoot, 0f, 1f);

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

        villageBrownDrawingRoot = villageBrownDrawingRoot != null ? villageBrownDrawingRoot : CreateVillageBrownDrawingRoot();
        ConfigureOverlayDrawingRoot(villageBrownDrawingRoot, drawingRoot);

        villageBrownDrawingGraphic = villageBrownDrawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (villageBrownDrawingGraphic == null)
        {
            villageBrownDrawingGraphic = villageBrownDrawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        villageBrownDrawingGraphic.raycastTarget = false;
        villageBrownDrawingGraphic.color = BrownCrayonColor;
        villageBrownDrawingGraphic.LineWidth = lineWidth * 2.25f;

        List<Vector2[]> villageBrownStrokes = BuildSketchbookVillageBrownColorStrokes();
        villageBrownDrawingScale = FitStrokesToReferenceRoot(villageBrownStrokes, drawingRoot, BuildSketchbookStrokes());
        villageBrownDrawingGraphic.SetStrokes(villageBrownStrokes);

        villageBlueWaterDrawingRoot = villageBlueWaterDrawingRoot != null ? villageBlueWaterDrawingRoot : CreateVillageBlueWaterDrawingRoot();
        ConfigureOverlayDrawingRoot(villageBlueWaterDrawingRoot, drawingRoot);

        villageBlueWaterDrawingGraphic = villageBlueWaterDrawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (villageBlueWaterDrawingGraphic == null)
        {
            villageBlueWaterDrawingGraphic = villageBlueWaterDrawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        villageBlueWaterDrawingGraphic.raycastTarget = false;
        villageBlueWaterDrawingGraphic.color = BlueCrayonColor;
        villageBlueWaterDrawingGraphic.LineWidth = lineWidth * 2.12f;

        List<Vector2[]> villageBlueWaterStrokes = BuildSketchbookVillageBlueWaterColorStrokes();
        villageBlueWaterDrawingScale = FitStrokesToReferenceRoot(villageBlueWaterStrokes, drawingRoot, BuildSketchbookStrokes());
        villageBlueWaterDrawingGraphic.SetStrokes(villageBlueWaterStrokes);

        forestDrawingRoot = forestDrawingRoot != null ? forestDrawingRoot : CreateForestDrawingRoot();
        ConfigureMapDrawingRoot(forestDrawingRoot, 1f, 1f);

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

        forestBrownDrawingRoot = forestBrownDrawingRoot != null ? forestBrownDrawingRoot : CreateForestBrownDrawingRoot();
        ConfigureOverlayDrawingRoot(forestBrownDrawingRoot, forestDrawingRoot);

        forestBrownDrawingGraphic = forestBrownDrawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (forestBrownDrawingGraphic == null)
        {
            forestBrownDrawingGraphic = forestBrownDrawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        forestBrownDrawingGraphic.raycastTarget = false;
        forestBrownDrawingGraphic.color = BrownCrayonColor;
        forestBrownDrawingGraphic.LineWidth = lineWidth * 2.25f;

        List<Vector2[]> forestBrownStrokes = BuildSketchbookForestBrownColorStrokes();
        forestBrownDrawingScale = FitStrokesToReferenceRoot(forestBrownStrokes, forestDrawingRoot, BuildSketchbookForestStrokes());
        forestBrownDrawingGraphic.SetStrokes(forestBrownStrokes);

        forestBlueWaterDrawingRoot = forestBlueWaterDrawingRoot != null ? forestBlueWaterDrawingRoot : CreateForestBlueWaterDrawingRoot();
        ConfigureOverlayDrawingRoot(forestBlueWaterDrawingRoot, forestDrawingRoot);

        forestBlueWaterDrawingGraphic = forestBlueWaterDrawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (forestBlueWaterDrawingGraphic == null)
        {
            forestBlueWaterDrawingGraphic = forestBlueWaterDrawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        forestBlueWaterDrawingGraphic.raycastTarget = false;
        forestBlueWaterDrawingGraphic.color = BlueCrayonColor;
        forestBlueWaterDrawingGraphic.LineWidth = lineWidth * 2.0f;

        List<Vector2[]> forestBlueWaterStrokes = BuildSketchbookForestBlueWaterColorStrokes();
        forestBlueWaterDrawingScale = FitStrokesToReferenceRoot(forestBlueWaterStrokes, forestDrawingRoot, BuildSketchbookForestStrokes());
        forestBlueWaterDrawingGraphic.SetStrokes(forestBlueWaterStrokes);

        deepForestDrawingRoot = deepForestDrawingRoot != null ? deepForestDrawingRoot : CreateDeepForestDrawingRoot();
        ConfigureMapDrawingRoot(deepForestDrawingRoot, 2f, DeepForestPanelWidthScale);

        deepForestDrawingGraphic = deepForestDrawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (deepForestDrawingGraphic == null)
        {
            deepForestDrawingGraphic = deepForestDrawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        deepForestDrawingGraphic.raycastTarget = false;
        deepForestDrawingGraphic.color = lineColor;
        deepForestDrawingGraphic.LineWidth = lineWidth;

        List<Vector2[]> deepForestStrokes = BuildSketchbookDeepForestStrokes();
        deepForestDrawingScale = FitStrokesToNarrowDrawingRoot(deepForestStrokes, deepForestDrawingRoot);
        deepForestDrawingGraphic.SetStrokes(deepForestStrokes);

        deepForestGreenDrawingRoot = deepForestGreenDrawingRoot != null ? deepForestGreenDrawingRoot : CreateDeepForestGreenDrawingRoot();
        ConfigureOverlayDrawingRoot(deepForestGreenDrawingRoot, deepForestDrawingRoot);

        deepForestGreenDrawingGraphic = deepForestGreenDrawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (deepForestGreenDrawingGraphic == null)
        {
            deepForestGreenDrawingGraphic = deepForestGreenDrawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        deepForestGreenDrawingGraphic.raycastTarget = false;
        deepForestGreenDrawingGraphic.color = GreenCrayonColor;
        deepForestGreenDrawingGraphic.LineWidth = lineWidth * 2.45f;

        List<Vector2[]> deepForestGreenStrokes = BuildSketchbookDeepForestGreenColorStrokes();
        deepForestGreenDrawingScale = FitStrokesToNarrowReferenceRoot(deepForestGreenStrokes, deepForestDrawingRoot, BuildSketchbookDeepForestStrokes());
        deepForestGreenDrawingGraphic.SetStrokes(deepForestGreenStrokes);

        deepForestBrownDrawingRoot = deepForestBrownDrawingRoot != null ? deepForestBrownDrawingRoot : CreateDeepForestBrownDrawingRoot();
        ConfigureOverlayDrawingRoot(deepForestBrownDrawingRoot, deepForestDrawingRoot);

        deepForestBrownDrawingGraphic = deepForestBrownDrawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (deepForestBrownDrawingGraphic == null)
        {
            deepForestBrownDrawingGraphic = deepForestBrownDrawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        deepForestBrownDrawingGraphic.raycastTarget = false;
        deepForestBrownDrawingGraphic.color = BrownCrayonColor;
        deepForestBrownDrawingGraphic.LineWidth = lineWidth * 2.25f;

        List<Vector2[]> deepForestBrownStrokes = BuildSketchbookDeepForestBrownColorStrokes();
        deepForestBrownDrawingScale = FitStrokesToNarrowReferenceRoot(deepForestBrownStrokes, deepForestDrawingRoot, BuildSketchbookDeepForestStrokes());
        deepForestBrownDrawingGraphic.SetStrokes(deepForestBrownStrokes);

        fourthForestDrawingRoot = fourthForestDrawingRoot != null ? fourthForestDrawingRoot : CreateFourthForestDrawingRoot();
        ConfigureMapDrawingRoot(fourthForestDrawingRoot, 2f + DeepForestPanelWidthScale, 1f);

        fourthForestDrawingGraphic = fourthForestDrawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (fourthForestDrawingGraphic == null)
        {
            fourthForestDrawingGraphic = fourthForestDrawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        fourthForestDrawingGraphic.raycastTarget = false;
        fourthForestDrawingGraphic.color = lineColor;
        fourthForestDrawingGraphic.LineWidth = lineWidth;

        List<Vector2[]> fourthForestStrokes = BuildSketchbookFourthForestStrokes();
        fourthForestDrawingScale = FitStrokesToDrawingRoot(fourthForestStrokes, fourthForestDrawingRoot);
        fourthForestDrawingGraphic.SetStrokes(fourthForestStrokes);

        fourthForestGreenDrawingRoot = fourthForestGreenDrawingRoot != null ? fourthForestGreenDrawingRoot : CreateFourthForestGreenDrawingRoot();
        ConfigureOverlayDrawingRoot(fourthForestGreenDrawingRoot, fourthForestDrawingRoot);

        fourthForestGreenDrawingGraphic = fourthForestGreenDrawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (fourthForestGreenDrawingGraphic == null)
        {
            fourthForestGreenDrawingGraphic = fourthForestGreenDrawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        fourthForestGreenDrawingGraphic.raycastTarget = false;
        fourthForestGreenDrawingGraphic.color = GreenCrayonColor;
        fourthForestGreenDrawingGraphic.LineWidth = lineWidth * 2.45f;

        List<Vector2[]> fourthForestGreenStrokes = BuildSketchbookFourthForestGreenColorStrokes();
        fourthForestGreenDrawingScale = FitStrokesToReferenceRoot(fourthForestGreenStrokes, fourthForestDrawingRoot, BuildSketchbookFourthForestStrokes());
        fourthForestGreenDrawingGraphic.SetStrokes(fourthForestGreenStrokes);

        fourthForestBrownDrawingRoot = fourthForestBrownDrawingRoot != null ? fourthForestBrownDrawingRoot : CreateFourthForestBrownDrawingRoot();
        ConfigureOverlayDrawingRoot(fourthForestBrownDrawingRoot, fourthForestDrawingRoot);

        fourthForestBrownDrawingGraphic = fourthForestBrownDrawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (fourthForestBrownDrawingGraphic == null)
        {
            fourthForestBrownDrawingGraphic = fourthForestBrownDrawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        fourthForestBrownDrawingGraphic.raycastTarget = false;
        fourthForestBrownDrawingGraphic.color = BrownCrayonColor;
        fourthForestBrownDrawingGraphic.LineWidth = lineWidth * 2.25f;

        List<Vector2[]> fourthForestBrownStrokes = BuildSketchbookFourthForestBrownColorStrokes();
        fourthForestBrownDrawingScale = FitStrokesToReferenceRoot(fourthForestBrownStrokes, fourthForestDrawingRoot, BuildSketchbookFourthForestStrokes());
        fourthForestBrownDrawingGraphic.SetStrokes(fourthForestBrownStrokes);

        fourthForestBlueWaterDrawingRoot = fourthForestBlueWaterDrawingRoot != null ? fourthForestBlueWaterDrawingRoot : CreateFourthForestBlueWaterDrawingRoot();
        ConfigureOverlayDrawingRoot(fourthForestBlueWaterDrawingRoot, fourthForestDrawingRoot);

        fourthForestBlueWaterDrawingGraphic = fourthForestBlueWaterDrawingRoot.GetComponent<SketchbookLineDrawingGraphic>();
        if (fourthForestBlueWaterDrawingGraphic == null)
        {
            fourthForestBlueWaterDrawingGraphic = fourthForestBlueWaterDrawingRoot.gameObject.AddComponent<SketchbookLineDrawingGraphic>();
        }

        fourthForestBlueWaterDrawingGraphic.raycastTarget = false;
        fourthForestBlueWaterDrawingGraphic.color = BlueCrayonColor;
        fourthForestBlueWaterDrawingGraphic.LineWidth = lineWidth * 2.18f;

        List<Vector2[]> fourthForestBlueWaterStrokes = BuildSketchbookFourthForestBlueWaterColorStrokes();
        fourthForestBlueWaterDrawingScale = FitStrokesToReferenceRoot(fourthForestBlueWaterStrokes, fourthForestDrawingRoot, BuildSketchbookFourthForestStrokes());
        fourthForestBlueWaterDrawingGraphic.SetStrokes(fourthForestBlueWaterStrokes);

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

    private RectTransform CreateVillageBrownDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchVillageBrownDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchVillageBrownDrawing", typeof(RectTransform));
        drawingObject.transform.SetParent(transform, false);
        return drawingObject.GetComponent<RectTransform>();
    }

    private RectTransform CreateVillageBlueWaterDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchVillageBlueWaterDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchVillageBlueWaterDrawing", typeof(RectTransform));
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

    private RectTransform CreateForestBrownDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchForestBrownDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchForestBrownDrawing", typeof(RectTransform));
        drawingObject.transform.SetParent(transform, false);
        return drawingObject.GetComponent<RectTransform>();
    }

    private RectTransform CreateForestBlueWaterDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchForestBlueWaterDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchForestBlueWaterDrawing", typeof(RectTransform));
        drawingObject.transform.SetParent(transform, false);
        return drawingObject.GetComponent<RectTransform>();
    }

    private RectTransform CreateDeepForestDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchDeepForestDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchDeepForestDrawing", typeof(RectTransform));
        drawingObject.transform.SetParent(transform, false);
        return drawingObject.GetComponent<RectTransform>();
    }

    private RectTransform CreateDeepForestGreenDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchDeepForestGreenDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchDeepForestGreenDrawing", typeof(RectTransform));
        drawingObject.transform.SetParent(transform, false);
        return drawingObject.GetComponent<RectTransform>();
    }

    private RectTransform CreateDeepForestBrownDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchDeepForestBrownDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchDeepForestBrownDrawing", typeof(RectTransform));
        drawingObject.transform.SetParent(transform, false);
        return drawingObject.GetComponent<RectTransform>();
    }

    private RectTransform CreateFourthForestDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchFourthForestDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchFourthForestDrawing", typeof(RectTransform));
        drawingObject.transform.SetParent(transform, false);
        return drawingObject.GetComponent<RectTransform>();
    }

    private RectTransform CreateFourthForestGreenDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchFourthForestGreenDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchFourthForestGreenDrawing", typeof(RectTransform));
        drawingObject.transform.SetParent(transform, false);
        return drawingObject.GetComponent<RectTransform>();
    }

    private RectTransform CreateFourthForestBrownDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchFourthForestBrownDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchFourthForestBrownDrawing", typeof(RectTransform));
        drawingObject.transform.SetParent(transform, false);
        return drawingObject.GetComponent<RectTransform>();
    }

    private RectTransform CreateFourthForestBlueWaterDrawingRoot()
    {
        Transform existing = transform.Find("GeneratedSketchFourthForestBlueWaterDrawing");
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject drawingObject = new GameObject("GeneratedSketchFourthForestBlueWaterDrawing", typeof(RectTransform));
        drawingObject.transform.SetParent(transform, false);
        return drawingObject.GetComponent<RectTransform>();
    }

    private void ConfigureMapDrawingRoot(RectTransform targetRoot, float unitStart, float unitWidth)
    {
        if (targetRoot == null)
        {
            return;
        }

        float layoutWidth = GetSketchbookLayoutWidth();
        float mapUnitWidth = layoutWidth / SketchbookMapLayoutUnits;
        float mapWidth = mapUnitWidth * unitWidth;
        float left = layoutWidth * -0.5f;
        float centerX = left + mapUnitWidth * (unitStart + unitWidth * 0.5f);

        targetRoot.anchorMin = new Vector2(0.5f, 0.5f);
        targetRoot.anchorMax = new Vector2(0.5f, 0.5f);
        targetRoot.pivot = new Vector2(0.5f, 0.5f);
        targetRoot.anchoredPosition = new Vector2(centerX + drawingOffset.x, drawingOffset.y);
        targetRoot.sizeDelta = new Vector2(mapWidth, GetSketchbookLayoutHeight());
        targetRoot.localScale = Vector3.one;
        targetRoot.localRotation = Quaternion.identity;
    }

    private static void ConfigureOverlayDrawingRoot(RectTransform overlayRoot, RectTransform sourceRoot)
    {
        if (overlayRoot == null || sourceRoot == null)
        {
            return;
        }

        if (overlayRoot.parent != sourceRoot.parent)
        {
            overlayRoot.SetParent(sourceRoot.parent, false);
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
        if (existing == null && villageGreenDrawingRoot != null)
        {
            existing = villageGreenDrawingRoot.Find("GeneratedPencil");
        }
        if (existing == null && villageBrownDrawingRoot != null)
        {
            existing = villageBrownDrawingRoot.Find("GeneratedPencil");
        }
        if (existing == null && villageBlueWaterDrawingRoot != null)
        {
            existing = villageBlueWaterDrawingRoot.Find("GeneratedPencil");
        }
        if (existing == null && forestDrawingRoot != null)
        {
            existing = forestDrawingRoot.Find("GeneratedPencil");
        }
        if (existing == null && forestGreenDrawingRoot != null)
        {
            existing = forestGreenDrawingRoot.Find("GeneratedPencil");
        }
        if (existing == null && forestBrownDrawingRoot != null)
        {
            existing = forestBrownDrawingRoot.Find("GeneratedPencil");
        }
        if (existing == null && forestBlueWaterDrawingRoot != null)
        {
            existing = forestBlueWaterDrawingRoot.Find("GeneratedPencil");
        }
        if (existing == null && deepForestDrawingRoot != null)
        {
            existing = deepForestDrawingRoot.Find("GeneratedPencil");
        }
        if (existing == null && deepForestGreenDrawingRoot != null)
        {
            existing = deepForestGreenDrawingRoot.Find("GeneratedPencil");
        }
        if (existing == null && deepForestBrownDrawingRoot != null)
        {
            existing = deepForestBrownDrawingRoot.Find("GeneratedPencil");
        }
        if (existing == null && fourthForestDrawingRoot != null)
        {
            existing = fourthForestDrawingRoot.Find("GeneratedPencil");
        }
        if (existing == null && fourthForestGreenDrawingRoot != null)
        {
            existing = fourthForestGreenDrawingRoot.Find("GeneratedPencil");
        }
        if (existing == null && fourthForestBrownDrawingRoot != null)
        {
            existing = fourthForestBrownDrawingRoot.Find("GeneratedPencil");
        }
        if (existing == null && fourthForestBlueWaterDrawingRoot != null)
        {
            existing = fourthForestBlueWaterDrawingRoot.Find("GeneratedPencil");
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

        if (villageBrownDrawingGraphic != null)
        {
            villageBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails ? 1f : 0f;
        }

        if (villageBlueWaterDrawingGraphic != null)
        {
            villageBlueWaterDrawingGraphic.RevealProgress = GameProgress.HasColoredWaterBlue ? 1f : 0f;
        }

        if (forestDrawingGraphic != null)
        {
            forestDrawingGraphic.RevealProgress = GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (forestGreenDrawingGraphic != null)
        {
            forestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredForestGreen ? 1f : 0f;
        }

        if (forestBrownDrawingGraphic != null)
        {
            forestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (forestBlueWaterDrawingGraphic != null)
        {
            forestBlueWaterDrawingGraphic.RevealProgress = GameProgress.HasColoredWaterBlue && GameProgress.HasDrawnForestSketch ? 1f : 0f;
        }

        if (deepForestDrawingGraphic != null)
        {
            deepForestDrawingGraphic.RevealProgress = GameProgress.HasDrawnDeepForestSketch ? 1f : 0f;
        }

        if (deepForestGreenDrawingGraphic != null)
        {
            deepForestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredDeepForestGreen ? 1f : 0f;
        }

        if (deepForestBrownDrawingGraphic != null)
        {
            deepForestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnDeepForestSketch ? 1f : 0f;
        }

        if (fourthForestDrawingGraphic != null)
        {
            fourthForestDrawingGraphic.RevealProgress = GameProgress.HasDrawnFourthForestSketch ? 1f : 0f;
        }

        if (fourthForestGreenDrawingGraphic != null)
        {
            fourthForestGreenDrawingGraphic.RevealProgress = GameProgress.HasColoredFourthForestGreen ? 1f : 0f;
        }

        if (fourthForestBrownDrawingGraphic != null)
        {
            fourthForestBrownDrawingGraphic.RevealProgress = GameProgress.HasColoredBrownDetails && GameProgress.HasDrawnFourthForestSketch ? 1f : 0f;
        }

        if (fourthForestBlueWaterDrawingGraphic != null)
        {
            fourthForestBlueWaterDrawingGraphic.RevealProgress = GameProgress.HasColoredWaterBlue && GameProgress.HasDrawnFourthForestSketch ? 1f : 0f;
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
        drawingSize = new Vector2(DefaultSketchbookWidth / SketchbookMapLayoutUnits, RequestedDrawingHeight);
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

    private float FitStrokesToNarrowDrawingRoot(List<Vector2[]> strokes, RectTransform targetRoot)
    {
        return FitStrokesToNarrowReferenceRoot(strokes, targetRoot, strokes);
    }

    private float FitStrokesToNarrowReferenceRoot(List<Vector2[]> strokes, RectTransform targetRoot, List<Vector2[]> referenceStrokes)
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
                Vector2 centeredPoint = stroke[pointIndex] - boundsCenter;
                stroke[pointIndex] = centeredPoint * scale;
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
            Mathf.Max(1f, width),
            Mathf.Max(1f, height - DrawingVerticalPadding * 2f));
    }

    private float GetSketchbookLayoutWidth()
    {
        float drawableWidth = Mathf.Min(GetSketchbookWidth(), FirstPageDrawableWidth);
        return Mathf.Max(1f, drawableWidth - DrawingHorizontalPadding * 2f - Mathf.Abs(drawingOffset.x) * 2f);
    }

    private float GetSketchbookLayoutHeight()
    {
        float height = Mathf.Min(drawingSize.y, GetSketchbookHeight() - DrawingVerticalPadding * 2f - Mathf.Abs(drawingOffset.y) * 2f);
        return Mathf.Max(1f, height);
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

    private float GetSketchbookHeight()
    {
        RectTransform sketchbookRect = transform as RectTransform;

        if (sketchbookRect != null)
        {
            float rectHeight = sketchbookRect.rect.height;

            if (rectHeight > 0f)
            {
                return rectHeight;
            }

            float sizeDeltaHeight = Mathf.Abs(sketchbookRect.sizeDelta.y);

            if (sizeDeltaHeight > 0f)
            {
                return sizeDeltaHeight;
            }
        }

        return RequestedDrawingHeight;
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

    private static List<Vector2[]> BuildSketchbookVillageBrownColorStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapFence(strokes);
        AddMiniMapPaths(strokes);
        AddMiniMapHouseBrownColor(strokes, 0f, 2f, 6.0f);
        AddMiniMapHouseBrownColor(strokes, 18.4f, 3.1f, 5.0f);
        AddMiniMapForestEntranceBrownColor(strokes, 21.5f, -14.2f);
        AddMiniMapTreeTrunkBrownColor(strokes);

        return strokes;
    }

    private static List<Vector2[]> BuildSketchbookVillageBlueWaterColorStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapPondBlueWater(strokes, -23.5f, -13.6f);

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
        AddMiniMapForestCornerFences(strokes);
        AddMiniMapForestPaths(strokes);
        AddMiniMapForestClearing(strokes, 4.6f, -1.2f);
        AddMiniMapForestTrees(strokes);
        AddMiniMapForestGrass(strokes);
        AddMiniMapForestFixedLandmarks(strokes);

        return strokes;
    }

    private static List<Vector2[]> BuildSketchbookForestGreenColorStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapForestTreeGreenColor(strokes);
        AddMiniMapForestGrassGreenColor(strokes);

        return strokes;
    }

    private static List<Vector2[]> BuildSketchbookForestBrownColorStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapForestBoundary(strokes);
        AddMiniMapForestCornerFences(strokes);
        AddMiniMapForestPaths(strokes);
        AddMiniMapForestTreeTrunkBrownColor(strokes);

        return strokes;
    }

    private static List<Vector2[]> BuildSketchbookForestBlueWaterColorStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapWellBlueWater(strokes, 10.6f, -7.8f, 0.86f);

        return strokes;
    }

    private static List<Vector2[]> BuildSketchbookDeepForestStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapDeepForestBoundary(strokes);
        AddMiniMapDeepForestConnectionPath(strokes);
        AddMiniMapDeepForestEntranceDirt(strokes);
        AddMiniMapDeepForestTrees(strokes);

        return strokes;
    }

    private static List<Vector2[]> BuildSketchbookDeepForestGreenColorStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapDeepForestTreeGreenColor(strokes);

        return strokes;
    }

    private static List<Vector2[]> BuildSketchbookDeepForestBrownColorStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapDeepForestBoundary(strokes);
        AddMiniMapDeepForestConnectionPath(strokes);
        AddMiniMapDeepForestEntranceDirt(strokes);
        AddMiniMapDeepForestTreeTrunkBrownColor(strokes);

        return strokes;
    }

    private static List<Vector2[]> BuildSketchbookFourthForestStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapFourthForestBoundary(strokes);
        AddMiniMapFourthForestConnectionPath(strokes);
        AddMiniMapFourthForestBigTree(strokes);
        AddMiniMapFourthForestTrees(strokes);

        return strokes;
    }

    private static List<Vector2[]> BuildSketchbookFourthForestGreenColorStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapFourthForestBigTreeGreenColor(strokes);
        AddMiniMapFourthForestTreeGreenColor(strokes);

        return strokes;
    }

    private static List<Vector2[]> BuildSketchbookFourthForestBrownColorStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapFourthForestBoundary(strokes);
        AddMiniMapFourthForestConnectionPath(strokes);
        AddMiniMapFourthForestBigTreeTrunkBrownColor(strokes);
        AddMiniMapFourthForestTreeTrunkBrownColor(strokes);

        return strokes;
    }

    private static List<Vector2[]> BuildSketchbookFourthForestBlueWaterColorStrokes()
    {
        List<Vector2[]> strokes = new List<Vector2[]>();

        AddMiniMapFourthForestRiverBlueWater(strokes, 26.6f, -15.2f);

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

    private static void AddMiniMapDeepForestBoundary(List<Vector2[]> strokes)
    {
        const float left = -17.25f;
        const float right = 17.25f;
        const float top = 23.5f;
        const float bottom = -23.5f;

        strokes.Add(MapPoints(left, top, right, top, right, bottom, left, bottom, left, top));
        strokes.Add(MapPoints(left + 1.7f, top - 1.7f, right - 1.7f, top - 1.7f, right - 1.7f, bottom + 1.7f, left + 1.7f, bottom + 1.7f, left + 1.7f, top - 1.7f));
    }

    private static void AddMiniMapFourthForestBoundary(List<Vector2[]> strokes)
    {
        const float left = -34.5f;
        const float right = 34.5f;
        const float top = 23.5f;
        const float bottom = -23.5f;

        strokes.Add(MapPoints(left, top, right, top, right, bottom, left, bottom));
        strokes.Add(MapPoints(left + 1.7f, top - 1.7f, right - 1.7f, top - 1.7f, right - 1.7f, bottom + 1.7f, left + 1.7f, bottom + 1.7f));
    }

    private static void AddMiniMapFourthForestConnectionPath(List<Vector2[]> strokes)
    {
        strokes.Add(MapPoints(-34.5f, 0.0f, -26.0f, 0.2f, -17.6f, -0.8f, -8.4f, 0.4f, -1.6f, -0.2f));
        strokes.Add(MapPoints(-34.5f, 0.48f, -25.8f, 0.72f, -17.4f, -0.28f, -8.0f, 0.9f, -1.4f, 0.36f));
        strokes.Add(MapPoints(-34.5f, -0.48f, -26.2f, -0.34f, -17.7f, -1.22f, -8.8f, -0.08f, -1.7f, -0.76f));
    }

    private static void AddMiniMapFourthForestBigTree(List<Vector2[]> strokes)
    {
        AddMiniMapHugeTreeOutline(strokes, 0f, -1.0f, 1f);
    }

    private static void AddMiniMapFourthForestBigTreeGreenColor(List<Vector2[]> strokes)
    {
        AddMiniMapHugeTreeLeaves(strokes, 0f, -1.0f, 1f);
        AddMiniMapHugeTreeLeafCrayonFill(strokes, 0f, -1.0f, 1f);
    }

    private static void AddMiniMapFourthForestBigTreeTrunkBrownColor(List<Vector2[]> strokes)
    {
        AddMiniMapHugeTreeTrunk(strokes, 0f, -1.0f, 1f);
        AddMiniMapHugeTreeTrunkCrayonFill(strokes, 0f, -1.0f, 1f);
    }

    private static void AddMiniMapForestCornerFences(List<Vector2[]> strokes)
    {
        const float left = -34.5f;
        const float right = 34.5f;
        const float top = 23.5f;
        const float bottom = -23.5f;

        AddMiniMapFenceCorner(strokes, left, top, 1f, -1f);
        AddMiniMapFenceCorner(strokes, right, top, -1f, -1f);
        AddMiniMapFenceCorner(strokes, left, bottom, 1f, 1f);
        AddMiniMapFenceCorner(strokes, right, bottom, -1f, 1f);
    }

    private static void AddMiniMapFenceCorner(List<Vector2[]> strokes, float cornerX, float cornerY, float inwardX, float inwardY)
    {
        AddMapRect(strokes, cornerX + inwardX * 1.05f, cornerY + inwardY * 0.55f, 0.36f, 1.25f);
        AddMapRect(strokes, cornerX + inwardX * 0.55f, cornerY + inwardY * 1.05f, 1.25f, 0.36f);
        strokes.Add(MapPoints(
            cornerX + inwardX * 0.45f,
            cornerY + inwardY * 1.92f,
            cornerX + inwardX * 2.18f,
            cornerY + inwardY * 0.52f));
        strokes.Add(MapPoints(
            cornerX + inwardX * 1.92f,
            cornerY + inwardY * 2.18f,
            cornerX + inwardX * 0.52f,
            cornerY + inwardY * 0.45f));
    }

    private static void AddMiniMapForestPaths(List<Vector2[]> strokes)
    {
        strokes.Add(MapPoints(
            -34.5f, 0.0f,
            -27.2f, 0.6f,
            -16.2f, -2.1f,
            -4.6f, 1.8f,
            6.2f, -1.2f,
            18.4f, 2.5f,
            31.2f, 0.1f));
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

    private static void AddMiniMapForestFixedLandmarks(List<Vector2[]> strokes)
    {
        AddMiniMapWell(strokes, 10.6f, -7.8f, 0.86f);

        AddMiniMapRock(strokes, -30.4f, 3.8f, 0.78f);
        AddMiniMapRock(strokes, -22.8f, -16.4f, 0.74f);
        AddMiniMapRock(strokes, -4.8f, 14.2f, 0.76f);
        AddMiniMapRock(strokes, 13.4f, 7.8f, 0.84f);
        AddMiniMapRock(strokes, 24.8f, -14.6f, 0.76f);

        AddMiniMapMoleHole(strokes, -18.4f, 12.4f, 0.68f);
        AddMiniMapMoleHole(strokes, -7.2f, 8.8f, 0.68f);
        AddMiniMapMoleHole(strokes, 7.6f, 12.0f, 0.68f);
        AddMiniMapMoleHole(strokes, 17.8f, -5.4f, 0.68f);
        AddMiniMapMoleHole(strokes, -16.2f, -10.4f, 0.68f);
    }

    private static void AddMiniMapHouseBrownColor(List<Vector2[]> strokes, float centerX, float centerY, float scale)
    {
        float halfWidth = scale * 0.48f;
        float bodyBottom = centerY - scale * 0.32f;
        float bodyTop = centerY + scale * 0.18f;
        float roofTop = centerY + scale * 0.62f;

        strokes.Add(MapPoints(centerX - halfWidth * 0.9f, bodyBottom + scale * 0.08f, centerX + halfWidth * 0.82f, bodyBottom + scale * 0.04f));
        strokes.Add(MapPoints(centerX - halfWidth * 0.86f, centerY - scale * 0.06f, centerX + halfWidth * 0.86f, centerY - scale * 0.1f));
        strokes.Add(MapPoints(centerX - halfWidth * 0.78f, bodyTop - scale * 0.08f, centerX + halfWidth * 0.74f, bodyTop - scale * 0.12f));
        strokes.Add(MapPoints(centerX - halfWidth * 1.02f, bodyTop + scale * 0.02f, centerX, roofTop - scale * 0.12f, centerX + halfWidth * 1.02f, bodyTop + scale * 0.02f));
        strokes.Add(MapPoints(centerX - scale * 0.35f, bodyBottom - scale * 0.02f, centerX - scale * 0.12f, bodyBottom + scale * 0.18f, centerX + scale * 0.18f, bodyBottom + scale * 0.02f, centerX + scale * 0.42f, bodyBottom + scale * 0.18f));
    }

    private static void AddMiniMapForestEntranceBrownColor(List<Vector2[]> strokes, float centerX, float centerY)
    {
        AddMiniMapPineTrunkCrayon(strokes, centerX - 1.8f, centerY - 1.1f, 0.95f);
        AddMiniMapPineTrunkCrayon(strokes, centerX, centerY - 1.05f, 1.1f);
        AddMiniMapPineTrunkCrayon(strokes, centerX + 1.8f, centerY - 1.1f, 0.95f);
    }

    private static void AddMiniMapTreeTrunkBrownColor(List<Vector2[]> strokes)
    {
        AddMiniMapPineTrunkCrayon(strokes, -28.0f, 15.5f, 1.0f);
        AddMiniMapPineTrunkCrayon(strokes, -20.5f, 7.4f, 0.86f);
        AddMiniMapPineTrunkCrayon(strokes, -15.8f, 16.8f, 1.12f);
        AddMiniMapPineTrunkCrayon(strokes, -6.4f, 14.8f, 0.92f);
        AddMiniMapPineTrunkCrayon(strokes, 8.8f, 16.2f, 1.02f);
        AddMiniMapPineTrunkCrayon(strokes, 26.6f, 13.4f, 0.94f);
        AddMiniMapPineTrunkCrayon(strokes, 30.2f, 6.6f, 0.84f);
        AddMiniMapPineTrunkCrayon(strokes, -30.0f, -2.8f, 0.9f);
        AddMiniMapPineTrunkCrayon(strokes, -12.0f, -10.2f, 0.82f);
        AddMiniMapPineTrunkCrayon(strokes, 6.2f, -13.8f, 0.9f);
        AddMiniMapPineTrunkCrayon(strokes, 12.0f, -18.0f, 0.86f);
        AddMiniMapPineTrunkCrayon(strokes, 29.2f, -7.8f, 0.92f);
    }

    private static void AddMiniMapForestTreeTrunkBrownColor(List<Vector2[]> strokes)
    {
        AddMiniMapPineTrunkCrayon(strokes, -28.6f, 18.0f, 1.12f);
        AddMiniMapPineTrunkCrayon(strokes, -23.4f, 13.8f, 0.96f);
        AddMiniMapPineTrunkCrayon(strokes, -18.4f, 18.4f, 1.04f);
        AddMiniMapPineTrunkCrayon(strokes, -12.0f, 12.6f, 1.08f);
        AddMiniMapPineTrunkCrayon(strokes, -5.6f, 17.6f, 0.92f);
        AddMiniMapPineTrunkCrayon(strokes, 2.2f, 15.0f, 1.02f);
        AddMiniMapPineTrunkCrayon(strokes, 10.4f, 18.0f, 1.14f);
        AddMiniMapPineTrunkCrayon(strokes, 18.2f, 14.0f, 0.98f);
        AddMiniMapPineTrunkCrayon(strokes, 27.2f, 17.6f, 1.06f);
        AddMiniMapPineTrunkCrayon(strokes, -29.0f, 7.4f, 0.9f);
        AddMiniMapPineTrunkCrayon(strokes, -20.6f, 4.6f, 1.0f);
        AddMiniMapPineTrunkCrayon(strokes, -2.2f, 7.0f, 0.88f);
        AddMiniMapPineTrunkCrayon(strokes, 10.4f, 8.2f, 0.96f);
        AddMiniMapPineTrunkCrayon(strokes, 23.4f, 6.2f, 1.0f);
        AddMiniMapPineTrunkCrayon(strokes, -29.2f, -6.8f, 1.04f);
        AddMiniMapPineTrunkCrayon(strokes, -20.0f, -10.0f, 0.92f);
        AddMiniMapPineTrunkCrayon(strokes, -10.4f, -14.8f, 1.06f);
        AddMiniMapPineTrunkCrayon(strokes, -1.4f, -18.0f, 0.92f);
        AddMiniMapPineTrunkCrayon(strokes, 9.8f, -10.6f, 0.94f);
        AddMiniMapPineTrunkCrayon(strokes, 21.2f, -12.8f, 1.08f);
        AddMiniMapPineTrunkCrayon(strokes, 29.0f, -5.8f, 0.96f);
        AddMiniMapPineTrunkCrayon(strokes, 29.2f, -18.0f, 1.0f);
    }

    private static void AddMiniMapDeepForestPaths(List<Vector2[]> strokes)
    {
        strokes.Add(MapPoints(
            -34.5f, 0.0f,
            -24.8f, -1.6f,
            -14.2f, 2.8f,
            -3.8f, 0.6f,
            7.8f, 4.8f,
            18.6f, 1.0f,
            30.6f, 3.4f));
        strokes.Add(MapPoints(
            -9.2f, 6.2f,
            -4.2f, 11.4f,
            4.8f, 13.6f,
            14.2f, 16.8f));
        strokes.Add(MapPoints(
            -3.8f, 0.6f,
            -8.8f, -5.4f,
            -5.2f, -12.8f,
            1.2f, -18.4f));
        strokes.Add(MapPoints(
            7.8f, 4.8f,
            13.2f, -2.2f,
            21.4f, -7.6f,
            30.8f, -11.4f));
    }

    private static void AddMiniMapDeepForestConnectionPath(List<Vector2[]> strokes)
    {
        strokes.Add(MapPoints(-17.25f, 0.0f, -16.15f, 0.12f, -15.05f, -0.08f, -14.2f, 0.0f));
        strokes.Add(MapPoints(-17.25f, 0.42f, -16.18f, 0.54f, -15.06f, 0.34f, -13.25f, 0.42f));
        strokes.Add(MapPoints(-17.25f, -0.42f, -16.08f, -0.55f, -14.94f, -0.36f, -13.22f, -0.44f));
    }

    private static void AddMiniMapDeepForestEntranceDirt(List<Vector2[]> strokes)
    {
        strokes.Add(MapEllipsePoints(-14.2f, 0f, 3.45f, 2.45f, 28));
        strokes.Add(MapEllipsePoints(-14.05f, -0.08f, 2.62f, 1.72f, 24));
        strokes.Add(MapEllipsePoints(-14.35f, 0.08f, 1.54f, 0.96f, 18));
        strokes.Add(MapPoints(-16.7f, 0.34f, -15.75f, 0.58f, -14.74f, 0.42f, -13.68f, 0.62f, -12.64f, 0.36f, -11.8f, 0.56f));
        strokes.Add(MapPoints(-16.92f, -0.58f, -15.92f, -0.86f, -14.74f, -0.62f, -13.58f, -0.92f, -12.54f, -0.64f, -11.66f, -0.86f));
    }

    private static void AddMiniMapDeepForestTrees(List<Vector2[]> strokes)
    {
        foreach (Vector3 tree in MiniMapDeepForestTreePlacements)
        {
            AddMiniMapPine(strokes, tree.x, tree.y, tree.z);
        }
    }

    private static void AddMiniMapDeepForestGrass(List<Vector2[]> strokes)
    {
        AddMiniMapGrassTuft(strokes, -32.0f, -0.6f, 0.84f);
        AddMiniMapGrassTuft(strokes, -23.6f, -18.2f, 0.86f);
        AddMiniMapGrassTuft(strokes, -14.0f, -6.8f, 0.78f);
        AddMiniMapGrassTuft(strokes, -3.2f, -9.2f, 0.72f);
        AddMiniMapGrassTuft(strokes, 5.0f, 10.2f, 0.76f);
        AddMiniMapGrassTuft(strokes, 12.8f, -4.2f, 0.82f);
        AddMiniMapGrassTuft(strokes, 22.8f, 5.0f, 0.74f);
        AddMiniMapGrassTuft(strokes, 31.0f, 14.4f, 0.86f);
    }

    private static void AddMiniMapDeepForestTreeGreenColor(List<Vector2[]> strokes)
    {
        foreach (Vector3 tree in MiniMapDeepForestTreePlacements)
        {
            AddMiniMapPineCrayon(strokes, tree.x, tree.y, tree.z);
        }
    }

    private static void AddMiniMapDeepForestGrassGreenColor(List<Vector2[]> strokes)
    {
        AddMiniMapGrassCrayon(strokes, -32.0f, -0.6f, 0.84f);
        AddMiniMapGrassCrayon(strokes, -23.6f, -18.2f, 0.86f);
        AddMiniMapGrassCrayon(strokes, -14.0f, -6.8f, 0.78f);
        AddMiniMapGrassCrayon(strokes, -3.2f, -9.2f, 0.72f);
        AddMiniMapGrassCrayon(strokes, 5.0f, 10.2f, 0.76f);
        AddMiniMapGrassCrayon(strokes, 12.8f, -4.2f, 0.82f);
        AddMiniMapGrassCrayon(strokes, 22.8f, 5.0f, 0.74f);
        AddMiniMapGrassCrayon(strokes, 31.0f, 14.4f, 0.86f);
    }

    private static void AddMiniMapDeepForestTreeTrunkBrownColor(List<Vector2[]> strokes)
    {
        foreach (Vector3 tree in MiniMapDeepForestTreePlacements)
        {
            AddMiniMapPineTrunkCrayon(strokes, tree.x, tree.y, tree.z);
        }
    }

    private static void AddMiniMapFourthForestTrees(List<Vector2[]> strokes)
    {
        foreach (Vector3 tree in MiniMapFourthForestTreePlacements)
        {
            AddMiniMapPine(strokes, tree.x, tree.y, tree.z);
        }
    }

    private static void AddMiniMapFourthForestTreeGreenColor(List<Vector2[]> strokes)
    {
        foreach (Vector3 tree in MiniMapFourthForestTreePlacements)
        {
            AddMiniMapPineCrayon(strokes, tree.x, tree.y, tree.z);
        }
    }

    private static void AddMiniMapFourthForestTreeTrunkBrownColor(List<Vector2[]> strokes)
    {
        foreach (Vector3 tree in MiniMapFourthForestTreePlacements)
        {
            AddMiniMapPineTrunkCrayon(strokes, tree.x, tree.y, tree.z);
        }
    }

    private static void AddMiniMapHugeTreeOutline(List<Vector2[]> strokes, float centerX, float groundY, float scale)
    {
        AddMiniMapHugeTreeLeaves(strokes, centerX, groundY, scale);
        AddMiniMapHugeTreeTrunk(strokes, centerX, groundY, scale);
        strokes.Add(MapPoints(
            centerX - 5.0f * scale,
            groundY - 0.9f * scale,
            centerX - 2.8f * scale,
            groundY - 0.4f * scale,
            centerX,
            groundY - 0.8f * scale,
            centerX + 2.9f * scale,
            groundY - 0.35f * scale,
            centerX + 5.0f * scale,
            groundY - 0.85f * scale));
    }

    private static void AddMiniMapHugeTreeLeaves(List<Vector2[]> strokes, float centerX, float groundY, float scale)
    {
        float canopyY = groundY + 7.4f * scale;
        strokes.Add(MapEllipsePoints(centerX, canopyY, 7.6f * scale, 5.4f * scale, 34));
        strokes.Add(MapEllipsePoints(centerX - 4.5f * scale, canopyY - 1.2f * scale, 4.3f * scale, 3.3f * scale, 24));
        strokes.Add(MapEllipsePoints(centerX + 4.4f * scale, canopyY - 0.9f * scale, 4.2f * scale, 3.2f * scale, 24));
        strokes.Add(MapEllipsePoints(centerX - 1.2f * scale, canopyY + 2.6f * scale, 4.8f * scale, 3.2f * scale, 24));
        strokes.Add(MapEllipsePoints(centerX + 2.0f * scale, canopyY + 2.1f * scale, 4.5f * scale, 3.0f * scale, 24));
        strokes.Add(MapPoints(centerX - 6.8f * scale, canopyY + 0.4f * scale, centerX - 3.8f * scale, canopyY + 1.5f * scale, centerX - 0.8f * scale, canopyY + 0.7f * scale, centerX + 2.2f * scale, canopyY + 1.6f * scale, centerX + 6.5f * scale, canopyY + 0.2f * scale));
        strokes.Add(MapPoints(centerX - 5.4f * scale, canopyY - 2.0f * scale, centerX - 1.6f * scale, canopyY - 1.2f * scale, centerX + 1.8f * scale, canopyY - 2.1f * scale, centerX + 5.6f * scale, canopyY - 1.2f * scale));
    }

    private static void AddMiniMapHugeTreeTrunk(List<Vector2[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(MapPoints(
            centerX - 1.85f * scale,
            groundY - 0.4f * scale,
            centerX - 1.45f * scale,
            groundY + 2.2f * scale,
            centerX - 1.0f * scale,
            groundY + 5.5f * scale,
            centerX + 1.0f * scale,
            groundY + 5.5f * scale,
            centerX + 1.45f * scale,
            groundY + 2.2f * scale,
            centerX + 1.85f * scale,
            groundY - 0.4f * scale,
            centerX - 1.85f * scale,
            groundY - 0.4f * scale));
        strokes.Add(MapPoints(centerX - 0.72f * scale, groundY + 5.0f * scale, centerX - 4.3f * scale, groundY + 7.4f * scale));
        strokes.Add(MapPoints(centerX + 0.62f * scale, groundY + 4.8f * scale, centerX + 4.2f * scale, groundY + 7.2f * scale));
        strokes.Add(MapPoints(centerX - 0.6f * scale, groundY + 2.2f * scale, centerX - 2.8f * scale, groundY + 4.0f * scale));
        strokes.Add(MapPoints(centerX + 0.65f * scale, groundY + 2.0f * scale, centerX + 2.8f * scale, groundY + 3.9f * scale));
        strokes.Add(MapPoints(centerX - 0.52f * scale, groundY + 0.2f * scale, centerX - 0.38f * scale, groundY + 2.3f * scale, centerX - 0.72f * scale, groundY + 4.4f * scale));
        strokes.Add(MapPoints(centerX + 0.56f * scale, groundY + 0.1f * scale, centerX + 0.36f * scale, groundY + 2.4f * scale, centerX + 0.78f * scale, groundY + 4.5f * scale));
        strokes.Add(MapEllipsePoints(centerX + 0.42f * scale, groundY + 1.8f * scale, 0.46f * scale, 0.34f * scale, 14));
    }

    private static void AddMiniMapHugeTreeLeafCrayonFill(List<Vector2[]> strokes, float centerX, float groundY, float scale)
    {
        float canopyY = groundY + 7.4f * scale;

        strokes.Add(MapPoints(centerX - 5.8f * scale, canopyY + 2.0f * scale, centerX - 3.6f * scale, canopyY + 2.45f * scale, centerX - 1.1f * scale, canopyY + 2.0f * scale, centerX + 1.5f * scale, canopyY + 2.42f * scale, centerX + 4.5f * scale, canopyY + 1.95f * scale));
        strokes.Add(MapPoints(centerX - 6.9f * scale, canopyY + 0.9f * scale, centerX - 4.1f * scale, canopyY + 1.35f * scale, centerX - 1.2f * scale, canopyY + 0.78f * scale, centerX + 1.8f * scale, canopyY + 1.35f * scale, centerX + 6.2f * scale, canopyY + 0.76f * scale));
        strokes.Add(MapPoints(centerX - 7.1f * scale, canopyY - 0.25f * scale, centerX - 4.5f * scale, canopyY + 0.18f * scale, centerX - 1.9f * scale, canopyY - 0.24f * scale, centerX + 0.8f * scale, canopyY + 0.26f * scale, centerX + 5.8f * scale, canopyY - 0.18f * scale));
        strokes.Add(MapPoints(centerX - 6.1f * scale, canopyY - 1.28f * scale, centerX - 3.6f * scale, canopyY - 0.82f * scale, centerX - 0.8f * scale, canopyY - 1.28f * scale, centerX + 2.1f * scale, canopyY - 0.78f * scale, centerX + 5.6f * scale, canopyY - 1.22f * scale));
        strokes.Add(MapPoints(centerX - 4.8f * scale, canopyY - 2.2f * scale, centerX - 2.4f * scale, canopyY - 1.76f * scale, centerX + 0.1f * scale, canopyY - 2.15f * scale, centerX + 2.7f * scale, canopyY - 1.78f * scale, centerX + 4.9f * scale, canopyY - 2.16f * scale));
        strokes.Add(MapPoints(centerX - 4.7f * scale, canopyY + 3.0f * scale, centerX - 2.3f * scale, canopyY + 3.35f * scale, centerX + 0.2f * scale, canopyY + 2.94f * scale, centerX + 2.8f * scale, canopyY + 3.28f * scale, centerX + 4.5f * scale, canopyY + 2.9f * scale));
        strokes.Add(MapPoints(centerX - 3.1f * scale, canopyY + 4.0f * scale, centerX - 0.9f * scale, canopyY + 4.28f * scale, centerX + 1.2f * scale, canopyY + 3.96f * scale, centerX + 3.0f * scale, canopyY + 4.16f * scale));
        strokes.Add(MapPoints(centerX - 5.4f * scale, canopyY - 3.0f * scale, centerX - 2.7f * scale, canopyY - 2.62f * scale, centerX + 0.4f * scale, canopyY - 2.95f * scale, centerX + 3.6f * scale, canopyY - 2.58f * scale));
    }

    private static void AddMiniMapHugeTreeTrunkCrayonFill(List<Vector2[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(MapPoints(centerX - 1.28f * scale, groundY + 0.26f * scale, centerX - 0.32f * scale, groundY + 0.5f * scale, centerX + 0.96f * scale, groundY + 0.22f * scale));
        strokes.Add(MapPoints(centerX - 1.18f * scale, groundY + 1.0f * scale, centerX - 0.24f * scale, groundY + 1.26f * scale, centerX + 1.18f * scale, groundY + 0.96f * scale));
        strokes.Add(MapPoints(centerX - 1.06f * scale, groundY + 1.78f * scale, centerX + 0.12f * scale, groundY + 2.02f * scale, centerX + 1.08f * scale, groundY + 1.72f * scale));
        strokes.Add(MapPoints(centerX - 0.96f * scale, groundY + 2.58f * scale, centerX - 0.04f * scale, groundY + 2.8f * scale, centerX + 0.96f * scale, groundY + 2.54f * scale));
        strokes.Add(MapPoints(centerX - 0.82f * scale, groundY + 3.36f * scale, centerX + 0.02f * scale, groundY + 3.58f * scale, centerX + 0.84f * scale, groundY + 3.32f * scale));
        strokes.Add(MapPoints(centerX - 0.68f * scale, groundY + 4.16f * scale, centerX + 0.02f * scale, groundY + 4.34f * scale, centerX + 0.66f * scale, groundY + 4.12f * scale));
        strokes.Add(MapPoints(centerX - 0.48f * scale, groundY + 5.02f * scale, centerX + 0.1f * scale, groundY + 5.18f * scale, centerX + 0.5f * scale, groundY + 5.02f * scale));
        strokes.Add(MapPoints(centerX - 3.75f * scale, groundY + 6.62f * scale, centerX - 2.58f * scale, groundY + 6.98f * scale, centerX - 1.18f * scale, groundY + 5.98f * scale));
        strokes.Add(MapPoints(centerX + 1.16f * scale, groundY + 5.86f * scale, centerX + 2.72f * scale, groundY + 6.88f * scale, centerX + 3.88f * scale, groundY + 6.55f * scale));
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

    private static void AddMiniMapPondBlueWater(List<Vector2[]> strokes, float centerX, float centerY)
    {
        strokes.Add(MapEllipsePoints(centerX + 0.1f, centerY, 4.35f, 1.95f, 28));
        strokes.Add(MapPoints(centerX - 3.25f, centerY + 0.62f, centerX - 2.0f, centerY + 0.86f, centerX - 0.7f, centerY + 0.58f, centerX + 0.65f, centerY + 0.82f, centerX + 2.12f, centerY + 0.52f));
        strokes.Add(MapPoints(centerX - 3.7f, centerY - 0.16f, centerX - 2.38f, centerY + 0.08f, centerX - 1.05f, centerY - 0.2f, centerX + 0.22f, centerY + 0.04f, centerX + 1.6f, centerY - 0.18f, centerX + 3.05f, centerY + 0.05f));
        strokes.Add(MapPoints(centerX - 2.78f, centerY - 0.95f, centerX - 1.36f, centerY - 0.68f, centerX + 0.08f, centerY - 0.96f, centerX + 1.48f, centerY - 0.7f, centerX + 2.8f, centerY - 0.92f));
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

    private static void AddMiniMapPineTrunkCrayon(List<Vector2[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(MapLocalPoints(centerX, groundY, scale, -0.15f, 0.02f, -0.12f, -0.32f, 0.13f, -0.32f, 0.15f, 0.02f));
        strokes.Add(MapLocalPoints(centerX, groundY, scale, -0.05f, -0.28f, -0.08f, -0.06f, -0.26f, 0.22f));
        strokes.Add(MapLocalPoints(centerX, groundY, scale, 0.06f, -0.25f, 0.1f, -0.04f, 0.28f, 0.18f));
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

    private static void AddMiniMapWell(List<Vector2[]> strokes, float centerX, float centerY, float scale)
    {
        strokes.Add(MapEllipsePoints(centerX, centerY + 0.24f * scale, 1.35f * scale, 0.42f * scale, 22));
        strokes.Add(MapEllipsePoints(centerX, centerY + 0.42f * scale, 1.12f * scale, 0.28f * scale, 18));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, -1.34f, 0.22f, -1.1f, -1.02f, -0.72f, -1.25f, 0f, -1.32f, 0.74f, -1.22f, 1.1f, -0.98f, 1.34f, 0.22f));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, -0.88f, 0.58f, -0.88f, 1.9f));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, 0.88f, 0.58f, 0.88f, 1.9f));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, -1.14f, 1.72f, -0.56f, 2.34f, 0f, 2.58f, 0.58f, 2.34f, 1.14f, 1.72f));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, -0.54f, 1.2f, 0.54f, 1.2f));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, 0f, 1.2f, 0f, 0.28f));
    }

    private static void AddMiniMapWellBlueWater(List<Vector2[]> strokes, float centerX, float centerY, float scale)
    {
        strokes.Add(MapEllipsePoints(centerX, centerY + 0.36f * scale, 0.92f * scale, 0.2f * scale, 18));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, -0.68f, 0.38f, -0.34f, 0.48f, 0.02f, 0.36f, 0.36f, 0.48f, 0.72f, 0.36f));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, -0.48f, 0.18f, -0.12f, 0.08f, 0.24f, 0.18f, 0.52f, 0.08f));
    }

    private static void AddMiniMapFourthForestRiverBlueWater(List<Vector2[]> strokes, float centerX, float centerY)
    {
        strokes.Add(MapEllipsePoints(centerX, centerY, 7.05f, 5.58f, 42));
        strokes.Add(MapEllipsePoints(centerX - 0.08f, centerY - 0.04f, 3.36f, 2.54f, 32));
        strokes.Add(MapPoints(centerX - 5.65f, centerY + 2.62f, centerX - 4.42f, centerY + 3.06f, centerX - 3.12f, centerY + 2.68f, centerX - 1.86f, centerY + 3.02f, centerX - 0.48f, centerY + 2.66f));
        strokes.Add(MapPoints(centerX + 1.02f, centerY + 3.02f, centerX + 2.34f, centerY + 2.62f, centerX + 3.82f, centerY + 3.04f, centerX + 5.34f, centerY + 2.56f));
        strokes.Add(MapPoints(centerX - 5.86f, centerY - 2.62f, centerX - 4.46f, centerY - 2.18f, centerX - 3.1f, centerY - 2.58f, centerX - 1.82f, centerY - 2.24f, centerX - 0.5f, centerY - 2.64f));
        strokes.Add(MapPoints(centerX + 0.92f, centerY - 2.18f, centerX + 2.18f, centerY - 2.62f, centerX + 3.56f, centerY - 2.22f, centerX + 5.2f, centerY - 2.72f));
        strokes.Add(MapPoints(centerX - 6.12f, centerY + 0.92f, centerX - 5.42f, centerY + 0.38f, centerX - 6.04f, centerY - 0.22f, centerX - 5.28f, centerY - 0.82f));
        strokes.Add(MapPoints(centerX + 5.78f, centerY + 0.96f, centerX + 6.28f, centerY + 0.36f, centerX + 5.64f, centerY - 0.18f, centerX + 6.16f, centerY - 0.82f));
        AddMiniMapFourthForestIslandDoor(strokes, centerX, centerY, 0.7f);
    }

    private static void AddMiniMapFourthForestIslandDoor(List<Vector2[]> strokes, float centerX, float centerY, float scale)
    {
        strokes.Add(MapLocalPoints(centerX, centerY, scale, -1.24f, -1.72f, -1.24f, 0.52f, -1.02f, 1.1f, -0.56f, 1.58f, 0f, 1.78f, 0.58f, 1.58f, 1.02f, 1.1f, 1.24f, 0.52f, 1.24f, -1.72f, -1.24f, -1.72f));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, -0.78f, -1.72f, -0.78f, 0.36f, -0.54f, 0.92f, 0f, 1.18f, 0.54f, 0.92f, 0.78f, 0.36f, 0.78f, -1.72f));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, 0f, -1.62f, 0f, 1.12f));
        strokes.Add(MapEllipsePoints(centerX + 0.42f * scale, centerY - 0.18f * scale, 0.11f * scale, 0.09f * scale, 10));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, -1.44f, -1.78f, 1.44f, -1.78f));
    }

    private static void AddMiniMapRock(List<Vector2[]> strokes, float centerX, float centerY, float scale)
    {
        strokes.Add(MapLocalPoints(centerX, centerY, scale, -1.05f, -0.32f, -0.78f, 0.22f, -0.28f, 0.54f, 0.34f, 0.48f, 0.92f, 0.08f, 1.12f, -0.34f, 0.64f, -0.52f, -0.1f, -0.48f, -0.72f, -0.5f, -1.05f, -0.32f));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, -0.72f, 0.18f, -0.36f, -0.02f, 0.08f, 0.22f, 0.42f, 0.04f, 0.74f, 0.18f));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, -0.44f, -0.36f, -0.12f, -0.18f, 0.22f, -0.34f, 0.56f, -0.18f));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, 0.12f, 0.42f, 0f, 0.16f, 0.18f, -0.08f));
    }

    private static void AddMiniMapMoleHole(List<Vector2[]> strokes, float centerX, float centerY, float scale)
    {
        strokes.Add(MapEllipsePoints(centerX, centerY, 0.92f * scale, 0.38f * scale, 20));
        strokes.Add(MapEllipsePoints(centerX + 0.06f * scale, centerY + 0.05f * scale, 0.62f * scale, 0.22f * scale, 16));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, -0.94f, -0.05f, -1.24f, -0.18f, -0.78f, -0.28f));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, 0.82f, -0.06f, 1.18f, -0.18f, 0.72f, -0.28f));
        strokes.Add(MapLocalPoints(centerX, centerY, scale, -0.54f, 0.36f, -0.16f, 0.48f, 0.24f, 0.38f, 0.58f, 0.5f));
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
