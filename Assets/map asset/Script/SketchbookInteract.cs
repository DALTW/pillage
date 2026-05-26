using System.Collections;
using UnityEngine;

public class SketchbookInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject bigSketchbookUI;
    [SerializeField] private SketchbookDrawingAnimation sketchbookDrawingAnimation;
    [SerializeField] private float openScaleDuration = 0.2f;

    private bool isOpen;
    private bool isFirstDrawingLocked;
    private bool lockedPlayerMoveWasEnabled;
    private Coroutine scaleRoutine;
    private PlayerMove2D lockedPlayerMove;
    private Rigidbody2D lockedPlayerRigidbody;
    private Vector3 openScale = Vector3.one;

    private void Start()
    {
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
        if (isOpen && !isFirstDrawingLocked && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSketchbook();
        }
    }

    public void Interact(GameObject interactor)
    {
        if (isOpen)
        {
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
        bool shouldPlayGreenColoring = !shouldPlayDrawingAnimation && !shouldPlayForestExpansion && !shouldPlayDeepForestExpansion && GameProgress.CanColorVillageAtSketchbook;
        bool shouldPlayBrownColoring = !shouldPlayDrawingAnimation && !shouldPlayForestExpansion && !shouldPlayDeepForestExpansion && !shouldPlayGreenColoring && GameProgress.CanColorBrownDetailsAtSketchbook;
        GameProgress.CheckSketchbook();
        isOpen = true;
        isFirstDrawingLocked = shouldPlayDrawingAnimation || shouldPlayForestExpansion || shouldPlayDeepForestExpansion || shouldPlayGreenColoring || shouldPlayBrownColoring;
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

        scaleRoutine = StartCoroutine(OpenScaleUI(bigSketchbookUI.transform.localScale, shouldPlayDrawingAnimation, shouldPlayForestExpansion, shouldPlayDeepForestExpansion, shouldPlayGreenColoring, shouldPlayBrownColoring));
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

    private IEnumerator OpenScaleUI(Vector3 startScale, bool shouldPlayDrawingAnimation, bool shouldPlayForestExpansion, bool shouldPlayDeepForestExpansion, bool shouldPlayGreenColoring, bool shouldPlayBrownColoring)
    {
        yield return ScaleUI(startScale, openScale);

        if (isOpen && shouldPlayDrawingAnimation && sketchbookDrawingAnimation != null)
        {
            sketchbookDrawingAnimation.Play(CompleteFirstDrawing);
        }
        else if (isOpen && shouldPlayForestExpansion && sketchbookDrawingAnimation != null)
        {
            sketchbookDrawingAnimation.PlayForestExpansion(CompleteForestExpansionDrawing);
        }
        else if (isOpen && shouldPlayDeepForestExpansion && sketchbookDrawingAnimation != null)
        {
            sketchbookDrawingAnimation.PlayDeepForestExpansion(CompleteDeepForestExpansionDrawing);
        }
        else if (isOpen && shouldPlayGreenColoring && sketchbookDrawingAnimation != null)
        {
            sketchbookDrawingAnimation.PlayVillageGreenColoring(CompleteVillageGreenColoring);
        }
        else if (isOpen && shouldPlayGreenColoring)
        {
            CompleteVillageGreenColoring();
        }
        else if (isOpen && shouldPlayBrownColoring && sketchbookDrawingAnimation != null)
        {
            sketchbookDrawingAnimation.PlayBrownColoring(CompleteBrownColoring);
        }
        else if (isOpen && shouldPlayBrownColoring)
        {
            CompleteBrownColoring();
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
        GameProgress.PlaySketchbookDrawing();
        UnlockPlayerAfterFirstDrawing();
    }

    private void CompleteForestExpansionDrawing()
    {
        GameProgress.CompleteSketchbookForestDrawing();
        PencilFragmentHud.RefreshCollected();
        SketchOutsideTransition.ApplySketchbookForestUnlock();
        UnlockPlayerAfterFirstDrawing();
    }

    private void CompleteDeepForestExpansionDrawing()
    {
        GameProgress.CompleteSketchbookDeepForestDrawing();

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

        PencilFragmentHud.RefreshCollected();
        UnlockPlayerAfterFirstDrawing();
    }

    private void CompleteVillageGreenColoring()
    {
        GameProgress.ColorVillageGreen();
        PencilFragmentHud.RefreshCollected();
        SketchOutsideTransition.ApplySketchbookVillageGreenColoring();
        UnlockPlayerAfterFirstDrawing();
    }

    private void CompleteBrownColoring()
    {
        GameProgress.ColorBrownDetails();
        PencilFragmentHud.RefreshCollected();
        SketchOutsideTransition.ApplySketchbookBrownColoring();
        UnlockPlayerAfterFirstDrawing();
    }

    private void OnDisable()
    {
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
