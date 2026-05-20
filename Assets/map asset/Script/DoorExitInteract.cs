using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorExitInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform exitTarget;
    [SerializeField] private string nextSceneName;
    [SerializeField] private bool playOutsideReveal = true;
    [SerializeField] private Vector2 interiorReturnOffset = new Vector2(-1.1f, 0f);

    public void Interact(GameObject interactor)
    {
        if (!GameProgress.HasCheckedSketchbook)
        {
            Debug.Log("Check the sketchbook first.");
            return;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        if (exitTarget != null)
        {
            Vector3 exitPosition = exitTarget.position;
            interactor.transform.position = exitPosition;

            if (playOutsideReveal)
            {
                Vector3 interiorEntryPosition = transform.position + new Vector3(interiorReturnOffset.x, interiorReturnOffset.y, 0f);
                SketchOutsideTransition.PlayExit(interactor, exitPosition, interiorEntryPosition);
            }

            return;
        }

        Debug.LogWarning("Door exit target or next scene is not assigned.", this);
    }
}
