using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorExitInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform exitTarget;
    [SerializeField] private string nextSceneName;
    [SerializeField] private bool playOutsideReveal = true;
    [SerializeField] private Transform interiorReturnTarget;

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

            if (playOutsideReveal)
            {
                Vector3 interiorEntryPosition = interiorReturnTarget != null
                    ? interiorReturnTarget.position
                    : transform.position;

                SketchOutsideTransition.PlayExit(interactor, exitPosition, interiorEntryPosition);
            }
            else
            {
                interactor.transform.position = exitPosition;
            }

            return;
        }

        Debug.LogWarning("Door exit target or next scene is not assigned.", this);
    }
}
