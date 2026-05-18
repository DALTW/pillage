using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject promptObject;
    [SerializeField] private float showDistance = 1.2f;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        SetPromptVisible(false);
    }

    private void Update()
    {
        if (player == null || promptObject == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        SetPromptVisible(distance <= showDistance);
    }

    private void OnDisable()
    {
        SetPromptVisible(false);
    }

    private void SetPromptVisible(bool visible)
    {
        if (promptObject != null && promptObject.activeSelf != visible)
        {
            promptObject.SetActive(visible);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, showDistance);
    }
}
