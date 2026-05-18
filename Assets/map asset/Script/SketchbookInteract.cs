using System.Collections;
using UnityEngine;

public class SketchbookInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject bigSketchbookUI;
    [SerializeField] private SketchbookDrawingAnimation sketchbookDrawingAnimation;
    [SerializeField] private float openScaleDuration = 0.2f;

    private bool isOpen;
    private Coroutine scaleRoutine;
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
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSketchbook();
        }
    }

    public void Interact(GameObject interactor)
    {
        if (isOpen)
        {
            CloseSketchbook();
        }
        else
        {
            OpenSketchbook();
        }
    }

    private void OpenSketchbook()
    {
        if (bigSketchbookUI == null)
        {
            Debug.LogWarning("Big Sketchbook UI is not assigned.", this);
            return;
        }

        bool shouldPlayDrawingAnimation = !GameProgress.HasPlayedSketchbookDrawing;
        GameProgress.CheckSketchbook();
        isOpen = true;
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

        scaleRoutine = StartCoroutine(OpenScaleUI(bigSketchbookUI.transform.localScale, shouldPlayDrawingAnimation));
    }

    private void CloseSketchbook()
    {
        if (bigSketchbookUI == null)
        {
            return;
        }

        isOpen = false;

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

    private IEnumerator OpenScaleUI(Vector3 startScale, bool shouldPlayDrawingAnimation)
    {
        yield return ScaleUI(startScale, openScale);

        if (isOpen && shouldPlayDrawingAnimation && sketchbookDrawingAnimation != null)
        {
            GameProgress.PlaySketchbookDrawing();
            sketchbookDrawingAnimation.Play();
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isOpen)
        {
            CloseSketchbook();
        }
    }
}
