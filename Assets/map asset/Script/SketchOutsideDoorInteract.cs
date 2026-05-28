using UnityEngine;

public class SketchOutsideDoorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float promptDistance = 1.6f;

    private const int PromptSortingOrder = 50;
    private const float PromptGlowPulseSpeed = 4.8f;

    private SketchOutsideTransition owner;
    private Vector3 interiorEntryPosition;
    private Transform player;
    private GameObject promptObject;
    private TextMesh promptText;
    private TextMesh promptInnerGlowText;
    private TextMesh promptOuterGlowText;
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
            UpdatePromptGlow(false);
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
        bool shouldShowPrompt = interactionEnabled && distance <= promptDistance;
        promptObject.SetActive(shouldShowPrompt);
        UpdatePromptGlow(shouldShowPrompt);
    }

    private void OnDisable()
    {
        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }

        UpdatePromptGlow(false);
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
            promptText = promptObject.GetComponent<TextMesh>();
            EnsureGlowPromptTexts();
            return;
        }

        promptObject = new GameObject("GeneratedOutsideDoorPrompt");
        promptObject.transform.SetParent(transform, false);
        promptObject.transform.localPosition = new Vector3(0f, 1.05f, 0f);

        promptText = promptObject.AddComponent<TextMesh>();
        ConfigurePromptText(promptText, new Color32(35, 32, 28, 255));

        MeshRenderer meshRenderer = promptObject.GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = PromptSortingOrder;

        EnsureGlowPromptTexts();

        promptObject.SetActive(false);
    }

    private void EnsureGlowPromptTexts()
    {
        if (promptObject == null)
        {
            return;
        }

        if (promptText == null)
        {
            promptText = promptObject.GetComponent<TextMesh>();
        }

        promptOuterGlowText = CreateOrFindGlowText(
            "GeneratedOutsideDoorPromptOuterGlow",
            0.102f,
            new Color(1f, 0.72f, 0.16f, 0f),
            PromptSortingOrder - 2);
        promptInnerGlowText = CreateOrFindGlowText(
            "GeneratedOutsideDoorPromptInnerGlow",
            0.09f,
            new Color(1f, 0.88f, 0.36f, 0f),
            PromptSortingOrder - 1);
    }

    private TextMesh CreateOrFindGlowText(string objectName, float characterSize, Color color, int sortingOrder)
    {
        Transform existing = promptObject.transform.Find(objectName);
        GameObject textObject = existing != null ? existing.gameObject : new GameObject(objectName);
        textObject.transform.SetParent(promptObject.transform, false);
        textObject.transform.localPosition = Vector3.zero;
        textObject.transform.localRotation = Quaternion.identity;

        TextMesh textMesh = textObject.GetComponent<TextMesh>();
        if (textMesh == null)
        {
            textMesh = textObject.AddComponent<TextMesh>();
        }

        ConfigurePromptText(textMesh, color);
        textMesh.characterSize = characterSize;

        MeshRenderer meshRenderer = textObject.GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = sortingOrder;
        textObject.SetActive(false);
        return textMesh;
    }

    private static void ConfigurePromptText(TextMesh textMesh, Color color)
    {
        textMesh.text = "E";
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 64;
        textMesh.characterSize = 0.08f;
        textMesh.color = color;
    }

    private void UpdatePromptGlow(bool promptVisible)
    {
        bool shouldGlow = promptVisible
            && (GameProgress.CanExtendPencilAtSketchbook
                || GameProgress.CanExtendDeepForestAtSketchbook
                || GameProgress.CanExtendFourthForestAtSketchbook
                || GameProgress.CanColorVillageAtSketchbook
                || GameProgress.CanColorBrownDetailsAtSketchbook
                || GameProgress.CanColorWaterBlueAtSketchbook);

        if (promptInnerGlowText != null)
        {
            promptInnerGlowText.gameObject.SetActive(shouldGlow);
        }

        if (promptOuterGlowText != null)
        {
            promptOuterGlowText.gameObject.SetActive(shouldGlow);
        }

        if (!shouldGlow)
        {
            if (promptText != null)
            {
                promptText.color = new Color32(35, 32, 28, 255);
            }

            return;
        }

        float pulse = Mathf.SmoothStep(0f, 1f, (Mathf.Sin(Time.time * PromptGlowPulseSpeed) + 1f) * 0.5f);

        if (promptText != null)
        {
            promptText.color = Color.Lerp(
                new Color32(35, 32, 28, 255),
                new Color32(92, 64, 25, 255),
                pulse);
        }

        if (promptInnerGlowText != null)
        {
            promptInnerGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.02f, 1.15f, pulse);
            promptInnerGlowText.color = new Color(1f, 0.88f, 0.36f, Mathf.Lerp(0.42f, 0.8f, pulse));
        }

        if (promptOuterGlowText != null)
        {
            promptOuterGlowText.transform.localScale = Vector3.one * Mathf.Lerp(1.08f, 1.32f, pulse);
            promptOuterGlowText.color = new Color(1f, 0.72f, 0.16f, Mathf.Lerp(0.16f, 0.44f, pulse));
        }
    }
}
