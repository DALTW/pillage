using System.Collections;
using UnityEngine;

public class SketchbookInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject bigSketchbookUI;
    [SerializeField] private float openScaleDuration = 0.2f;

    private bool isOpen;
    private Coroutine scaleRoutine;

    private void Start()
    {
        if (bigSketchbookUI == null)
        {
            Debug.LogWarning("Big Sketchbook UI is not assigned.", this);
            return;
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

        GameProgress.CheckSketchbook();
        isOpen = true;
        bigSketchbookUI.SetActive(true);

        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
        }

        scaleRoutine = StartCoroutine(ScaleUI(Vector3.zero, Vector3.one));
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

        scaleRoutine = StartCoroutine(CloseScaleUI());
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
