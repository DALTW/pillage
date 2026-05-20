using UnityEngine;

public class SketchOutsideDoorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float promptDistance = 1.6f;

    private SketchOutsideTransition owner;
    private Vector3 interiorEntryPosition;
    private Transform player;
    private GameObject promptObject;
    private bool interactionEnabled;

    public void Configure(SketchOutsideTransition transitionOwner, Vector3 entryPosition, float doorPromptDistance)
    {
        owner = transitionOwner;
        interiorEntryPosition = entryPosition;
        promptDistance = doorPromptDistance;
        EnsurePrompt();
        FindPlayer();
    }

    public void Interact(GameObject interactor)
    {
        if (!interactionEnabled)
        {
            return;
        }

        if (owner != null)
        {
            owner.EnterWoodhouse(interactor, interiorEntryPosition);
            return;
        }

        interactor.transform.position = interiorEntryPosition;
    }

    public void SetInteractionEnabled(bool enabled)
    {
        interactionEnabled = enabled;

        if (!interactionEnabled && promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Awake()
    {
        EnsurePrompt();
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
        }

        if (player == null || promptObject == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        promptObject.SetActive(interactionEnabled && distance <= promptDistance);
    }

    private void OnDisable()
    {
        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }

    private void EnsurePrompt()
    {
        if (promptObject != null)
        {
            return;
        }

        Transform existingPrompt = transform.Find("GeneratedOutsideDoorPrompt");

        if (existingPrompt != null)
        {
            promptObject = existingPrompt.gameObject;
            return;
        }

        promptObject = new GameObject("GeneratedOutsideDoorPrompt");
        promptObject.transform.SetParent(transform, false);
        promptObject.transform.localPosition = new Vector3(0f, 1.05f, 0f);

        TextMesh textMesh = promptObject.AddComponent<TextMesh>();
        textMesh.text = "E";
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 64;
        textMesh.characterSize = 0.08f;
        textMesh.color = new Color32(35, 32, 28, 255);

        MeshRenderer meshRenderer = promptObject.GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = 50;

        promptObject.SetActive(false);
    }
}
