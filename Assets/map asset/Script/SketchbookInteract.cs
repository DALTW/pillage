using System;
using System.Collections;
using UnityEngine;

public class SketchbookInteract : MonoBehaviour, IInteractable
{
    private const string DefaultPromptText = "E";
    private const string SkipPromptText = "E: 스킵";

    [SerializeField] private GameObject bigSketchbookUI;
    [SerializeField] private SketchbookDrawingAnimation sketchbookDrawingAnimation;
    [SerializeField] private float openScaleDuration = 0.2f;

    private bool isOpen;
    private bool isFirstDrawingLocked;
    private bool isDrawing;
    private bool canSkipDrawing;
    private bool hasDrawingCompleted;
    private bool lockedPlayerMoveWasEnabled;
    private Coroutine scaleRoutine;
    private InteractionPrompt interactionPrompt;
    private PlayerMove2D lockedPlayerMove;
    private Rigidbody2D lockedPlayerRigidbody;
    private Vector3 openScale = Vector3.one;

    private void Start()
    {
        interactionPrompt = GetComponent<InteractionPrompt>();

        if (bigSketchbookUI == null)
        {
            Debug.LogWarning("Big Sketchbook UI is not assigned.", this);
            return;
        }

        openScale = bigSketchbookUI.transform.localScale;
        sketchbookDrawingAnimation = sketchbookDrawingAnimation != null
            ? sketchbookDrawingAnimation
            : bigSketchbookUI.GetComponent<SketchbookDrawingAnimation>();

        if (sketchbookDrawingAnimation == null)
        {
            sketchbookDrawingAnimation = bigSketchbookUI.AddComponent<SketchbookDrawingAnimation>();
        }

        bigSketchbookUI.SetActive(false);
        bigSketchbookUI.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (isOpen && isDrawing && canSkipDrawing && Input.GetKeyDown(KeyCode.E))
        {
            SkipDrawingAnimation();
        }

        if (isOpen && !isFirstDrawingLocked && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSketchbook();
        }
    }

    public void Interact(GameObject interactor)
    {
        if (isOpen)
        {
            if (isDrawing && canSkipDrawing)
            {
                SkipDrawingAnimation();
                return;
            }

            if (isFirstDrawingLocked)
            {
                return;
            }

            CloseSketchbook();
        }
        else
        {
            OpenSketchbook(interactor);
        }
    }

    private void OpenSketchbook(GameObject interactor)
    {
        if (bigSketchbookUI == null)
        {
            Debug.LogWarning("Big Sketchbook UI is not assigned.", this);
            return;
        }

        bool shouldPlayDrawingAnimation = !GameProgress.HasPlayedSketchbookDrawing;
        bool shouldPlayForestExpansion = !shouldPlayDrawingAnimation && GameProgress.CanExtendPencilAtSketchbook;
        bool shouldPlayDeepForestExpansion = !shouldPlayDrawingAnimation && !shouldPlayForestExpansion && GameProgress.CanExtendDeepForestAtSketchbook;
        bool shouldPlayFourthForestExpansion = !shouldPlayDrawingAnimation && !shouldPlayForestExpansion && !shouldPlayDeepForestExpansion && GameProgress.CanExtendFourthForestAtSketchbook;
        bool shouldPlayGreenColoring = !shouldPlayDrawingAnimation && !shouldPlayForestExpansion && !shouldPlayDeepForestExpansion && !shouldPlayFourthForestExpansion && GameProgress.CanColorVillageAtSketchbook;
        bool shouldPlayBrownColoring = !shouldPlayDrawingAnimation && !shouldPlayForestExpansion && !shouldPlayDeepForestExpansion && !shouldPlayFourthForestExpansion && !shouldPlayGreenColoring && GameProgress.CanColorBrownDetailsAtSketchbook;
        bool shouldPlayBlueColoring = !shouldPlayDrawingAnimation && !shouldPlayForestExpansion && !shouldPlayDeepForestExpansion && !shouldPlayFourthForestExpansion && !shouldPlayGreenColoring && !shouldPlayBrownColoring && GameProgress.CanColorWaterBlueAtSketchbook;
        GameProgress.CheckSketchbook();
        isOpen = true;
        ResetDrawingSkipState();
        isFirstDrawingLocked = shouldPlayDrawingAnimation || shouldPlayForestExpansion || shouldPlayDeepForestExpansion || shouldPlayFourthForestExpansion || shouldPlayGreenColoring || shouldPlayBrownColoring || shouldPlayBlueColoring;
        LockPlayerForFirstDrawing(interactor);
        bigSketchbookUI.SetActive(true);

        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
        }

        if (sketchbookDrawingAnimation != null)
        {
            if (shouldPlayDrawingAnimation)
            {
                sketchbookDrawingAnimation.ResetDrawing();
            }
            else
            {
                sketchbookDrawingAnimation.ShowCompletedDrawing();
            }
        }

        scaleRoutine = StartCoroutine(OpenScaleUI(bigSketchbookUI.transform.localScale, shouldPlayDrawingAnimation, shouldPlayForestExpansion, shouldPlayDeepForestExpansion, shouldPlayFourthForestExpansion, shouldPlayGreenColoring, shouldPlayBrownColoring, shouldPlayBlueColoring));
    }

    private void CloseSketchbook()
    {
        if (isFirstDrawingLocked)
        {
            return;
        }

        if (bigSketchbookUI == null)
        {
            return;
        }

        isOpen = false;
        ResetDrawingSkipState();
        UnlockPlayerAfterFirstDrawing();

        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
        }

        if (sketchbookDrawingAnimation != null)
        {
            sketchbookDrawingAnimation.ResetDrawing();
        }

        scaleRoutine = StartCoroutine(CloseScaleUI());
    }

    private IEnumerator OpenScaleUI(Vector3 startScale, bool shouldPlayDrawingAnimation, bool shouldPlayForestExpansion, bool shouldPlayDeepForestExpansion, bool shouldPlayFourthForestExpansion, bool shouldPlayGreenColoring, bool shouldPlayBrownColoring, bool shouldPlayBlueColoring)
    {
        yield return ScaleUI(startScale, openScale);

        if (isOpen && shouldPlayDrawingAnimation && sketchbookDrawingAnimation != null)
        {
            StartDrawingAnimation(() => sketchbookDrawingAnimation.Play(CompleteFirstDrawing));
        }
        else if (isOpen && shouldPlayForestExpansion && sketchbookDrawingAnimation != null)
        {
            StartDrawingAnimation(() => sketchbookDrawingAnimation.PlayForestExpansion(CompleteForestExpansionDrawing));
        }
        else if (isOpen && shouldPlayDeepForestExpansion && sketchbookDrawingAnimation != null)
        {
            StartDrawingAnimation(() => sketchbookDrawingAnimation.PlayDeepForestExpansion(CompleteDeepForestExpansionDrawing));
        }
        else if (isOpen && shouldPlayFourthForestExpansion && sketchbookDrawingAnimation != null)
        {
            StartDrawingAnimation(() => sketchbookDrawingAnimation.PlayFourthForestExpansion(CompleteFourthForestExpansionDrawing));
        }
        else if (isOpen && shouldPlayGreenColoring && sketchbookDrawingAnimation != null)
        {
            StartDrawingAnimation(() => sketchbookDrawingAnimation.PlayVillageGreenColoring(CompleteVillageGreenColoring));
        }
        else if (isOpen && shouldPlayGreenColoring)
        {
            CompleteVillageGreenColoring();
        }
        else if (isOpen && shouldPlayBrownColoring && sketchbookDrawingAnimation != null)
        {
            StartDrawingAnimation(() => sketchbookDrawingAnimation.PlayBrownColoring(CompleteBrownColoring));
        }
        else if (isOpen && shouldPlayBrownColoring)
        {
            CompleteBrownColoring();
        }
        else if (isOpen && shouldPlayBlueColoring && sketchbookDrawingAnimation != null)
        {
            StartDrawingAnimation(() => sketchbookDrawingAnimation.PlayBlueWaterColoring(CompleteBlueWaterColoring));
        }
        else if (isOpen && shouldPlayBlueColoring)
        {
            CompleteBlueWaterColoring();
        }
        else
        {
            UnlockPlayerAfterFirstDrawing();
        }
    }

    private IEnumerator CloseScaleUI()
    {
        yield return ScaleUI(bigSketchbookUI.transform.localScale, Vector3.zero);
        bigSketchbookUI.SetActive(false);
    }

    private IEnumerator ScaleUI(Vector3 start, Vector3 end)
    {
        float time = 0f;

        while (time < openScaleDuration)
        {
            time += Time.deltaTime;
            float t = time / openScaleDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            bigSketchbookUI.transform.localScale = Vector3.Lerp(start, end, t);
            yield return null;
        }

        bigSketchbookUI.transform.localScale = end;
    }

    private void StartDrawingAnimation(Action playAnimation)
    {
        isDrawing = true;
        canSkipDrawing = true;
        hasDrawingCompleted = false;
        SetInteractionPromptText(SkipPromptText);

        playAnimation?.Invoke();

        if (sketchbookDrawingAnimation == null || !sketchbookDrawingAnimation.IsPlaying)
        {
            isDrawing = false;
            canSkipDrawing = false;
            SetInteractionPromptText(DefaultPromptText);
        }
    }

    private void SkipDrawingAnimation()
    {
        if (!isDrawing || !canSkipDrawing || hasDrawingCompleted)
        {
            return;
        }

        if (sketchbookDrawingAnimation == null || !sketchbookDrawingAnimation.IsPlaying)
        {
            ResetDrawingSkipState();
            return;
        }

        canSkipDrawing = false;

        if (!sketchbookDrawingAnimation.SkipToEnd())
        {
            ResetDrawingSkipState();
        }
    }

    private void CompleteDrawingOnce(Action completionLogic)
    {
        if (hasDrawingCompleted)
        {
            return;
        }

        hasDrawingCompleted = true;
        isDrawing = false;
        canSkipDrawing = false;
        SetInteractionPromptText(DefaultPromptText);
        completionLogic?.Invoke();
    }

    private void ResetDrawingSkipState()
    {
        isDrawing = false;
        canSkipDrawing = false;
        hasDrawingCompleted = false;
        SetInteractionPromptText(DefaultPromptText);
    }

    private void SetInteractionPromptText(string text)
    {
        if (interactionPrompt == null)
        {
            interactionPrompt = GetComponent<InteractionPrompt>();
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetPromptText(text);
        }
    }

    private void LockPlayerForFirstDrawing(GameObject interactor)
    {
        if (!isFirstDrawingLocked || interactor == null)
        {
            return;
        }

        lockedPlayerMove = interactor.GetComponent<PlayerMove2D>();
        lockedPlayerRigidbody = interactor.GetComponent<Rigidbody2D>();
        lockedPlayerMoveWasEnabled = lockedPlayerMove != null && lockedPlayerMove.enabled;

        if (lockedPlayerRigidbody != null)
        {
            lockedPlayerRigidbody.linearVelocity = Vector2.zero;
        }

        if (lockedPlayerMove != null)
        {
            lockedPlayerMove.enabled = false;
        }
    }

    private void UnlockPlayerAfterFirstDrawing()
    {
        isFirstDrawingLocked = false;

        if (lockedPlayerMove != null)
        {
            lockedPlayerMove.enabled = lockedPlayerMoveWasEnabled;
        }

        lockedPlayerMove = null;
        lockedPlayerRigidbody = null;
        lockedPlayerMoveWasEnabled = false;
    }

    private void CompleteFirstDrawing()
    {
        CompleteDrawingOnce(() =>
        {
            GameProgress.PlaySketchbookDrawing();
            UnlockPlayerAfterFirstDrawing();
        });
    }

    private void CompleteForestExpansionDrawing()
    {
        CompleteDrawingOnce(() =>
        {
            GameProgress.CompleteSketchbookForestDrawing();
            PencilFragmentHud.RefreshCollected();
            SketchOutsideTransition.ApplySketchbookForestUnlock();
            UnlockPlayerAfterFirstDrawing();
        });
    }

    private void CompleteDeepForestExpansionDrawing()
    {
        CompleteDrawingOnce(() =>
        {
            GameProgress.CompleteSketchbookDeepForestDrawing();
            SketchOutsideTransition.ApplySketchbookDeepForestUnlock();

            if (GameProgress.HasCollectedGreenCrayon)
            {
                GameProgress.ColorVillageGreen();
                SketchOutsideTransition.ApplySketchbookVillageGreenColoring();
            }

            if (GameProgress.HasCollectedBrownCrayon)
            {
                GameProgress.ColorBrownDetails();
                SketchOutsideTransition.ApplySketchbookBrownColoring();
            }

            if (GameProgress.HasCollectedBlueCrayon)
            {
                GameProgress.ColorWaterBlue();
                SketchOutsideTransition.ApplySketchbookBlueWaterColoring();
            }

            PencilFragmentHud.RefreshCollected();
            UnlockPlayerAfterFirstDrawing();
        });
    }

    private void CompleteFourthForestExpansionDrawing()
    {
        CompleteDrawingOnce(() =>
        {
            GameProgress.CompleteSketchbookFourthForestDrawing();
            SketchOutsideTransition.ApplySketchbookFourthForestUnlock();

            if (GameProgress.HasCollectedGreenCrayon)
            {
                GameProgress.ColorVillageGreen();
                SketchOutsideTransition.ApplySketchbookVillageGreenColoring();
            }

            if (GameProgress.HasCollectedBrownCrayon)
            {
                GameProgress.ColorBrownDetails();
                SketchOutsideTransition.ApplySketchbookBrownColoring();
            }

            if (GameProgress.HasCollectedBlueCrayon)
            {
                GameProgress.ColorWaterBlue();
                SketchOutsideTransition.ApplySketchbookBlueWaterColoring();
            }

            PencilFragmentHud.RefreshCollected();
            UnlockPlayerAfterFirstDrawing();
        });
    }

    private void CompleteVillageGreenColoring()
    {
        CompleteDrawingOnce(() =>
        {
            GameProgress.ColorVillageGreen();
            PencilFragmentHud.RefreshCollected();
            SketchOutsideTransition.ApplySketchbookVillageGreenColoring();
            UnlockPlayerAfterFirstDrawing();
        });
    }

    private void CompleteBrownColoring()
    {
        CompleteDrawingOnce(() =>
        {
            GameProgress.ColorBrownDetails();
            PencilFragmentHud.RefreshCollected();
            SketchOutsideTransition.ApplySketchbookBrownColoring();
            UnlockPlayerAfterFirstDrawing();
        });
    }

    private void CompleteBlueWaterColoring()
    {
        CompleteDrawingOnce(() =>
        {
            GameProgress.ColorWaterBlue();
            PencilFragmentHud.RefreshCollected();
            SketchOutsideTransition.ApplySketchbookBlueWaterColoring();
            UnlockPlayerAfterFirstDrawing();
        });
    }

    private void OnDisable()
    {
        ResetDrawingSkipState();
        UnlockPlayerAfterFirstDrawing();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isOpen && !isFirstDrawingLocked)
        {
            CloseSketchbook();
        }
    }
}
