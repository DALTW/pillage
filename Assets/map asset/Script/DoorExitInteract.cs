using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorExitInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform exitTarget;
    [SerializeField] private string nextSceneName;

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
            interactor.transform.position = exitTarget.position;
            return;
        }

        Debug.LogWarning("Door exit target or next scene is not assigned.", this);
    }
}
