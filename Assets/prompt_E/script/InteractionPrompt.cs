using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject promptObject;
    [SerializeField] private float showDistance = 1.2f;
    [SerializeField] private Vector3 promptWorldOffset = new Vector3(0f, 1.15f, 0f);
    [SerializeField] private Vector3 promptWorldScale = Vector3.one;
    [SerializeField] private int promptFontSize = 72;
    [SerializeField] private float promptCharacterSize = 0.08f;
    [SerializeField] private Color promptColor = new Color(1f, 0.92f, 0.72f, 1f);
    [SerializeField] private Color promptShadowColor = new Color(0f, 0f, 0f, 0.65f);
    [SerializeField] private Vector3 promptShadowOffset = new Vector3(0.035f, -0.035f, 0f);
    [SerializeField] private int promptSortingOrder = 70;

    private const float CompletionGlowPulseSpeed = 4.8f;

    private GameObject simplePromptRoot;
    private TextMesh promptText;
    private TextMesh promptInnerGlowText;
    private TextMesh promptOuterGlowText;
    private bool usesCompletionGlow;

    private void Start()
    {
        FindPlayerIfNeeded();
        CreateSimplePrompt();

        SetPromptVisible(false);
    }

    private void Update()
    {
        FindPlayerIfNeeded();

        if (player == null || promptObject == null)
        {
            return;
        }

        UpdatePromptPosition();

        float distance = Vector2.Distance(transform.position, player.position);
        bool shouldShowPrompt = distance <= showDistance;
        SetPromptVisible(shouldShowPrompt);
        UpdateCompletionGlow(shouldShowPrompt);
    }

    private void OnDisable()
    {
        SetPromptVisible(false);
        UpdateCompletionGlow(false);
    }

    private void SetPromptVisible(bool visible)
    {
        if (promptObject != null && promptObject.activeSelf != visible)
        {
            promptObject.SetActive(visible);
        }

        if (!visible)
        {
            UpdateCompletionGlow(false);
        }
    }

    private void FindPlayerIfNeeded()
    {
        if (player != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void CreateSimplePrompt()
    {
        if (simplePromptRoot != null)
        {
            return;
        }

        if (promptObject == null)
        {
            promptObject = new GameObject("SimpleInteractionPrompt");
            promptObject.transform.SetParent(transform, false);
        }

        HideLegacyPromptRenderers();
        usesCompletionGlow = GetComponent<SketchbookInteract>() != null;

        simplePromptRoot = new GameObject("SimpleInteractionPromptE");
        simplePromptRoot.transform.SetParent(promptObject.transform, false);
        simplePromptRoot.transform.localPosition = Vector3.zero;
        simplePromptRoot.transform.localRotation = Quaternion.identity;
        simplePromptRoot.transform.localScale = Vector3.one;

        CreatePromptText("SimpleInteractionPromptShadow", promptShadowColor, promptShadowOffset, promptSortingOrder - 1);

        if (usesCompletionGlow)
        {
            promptOuterGlowText = CreatePromptText(
                "SimpleInteractionPromptOuterGlow",
                new Color(1f, 0.72f, 0.16f, 0f),
                Vector3.zero,
                promptSortingOrder - 3);
            promptInnerGlowText = CreatePromptText(
                "SimpleInteractionPromptInnerGlow",
                new Color(1f, 0.88f, 0.36f, 0f),
                Vector3.zero,
                promptSortingOrder - 2);
            promptOuterGlowText.characterSize = promptCharacterSize * 1.28f;
            promptInnerGlowText.characterSize = promptCharacterSize * 1.13f;
            promptOuterGlowText.gameObject.SetActive(false);
            promptInnerGlowText.gameObject.SetActive(false);
        }

        promptText = CreatePromptText("SimpleInteractionPromptText", promptColor, Vector3.zero, promptSortingOrder);
        UpdatePromptPosition();
    }

    private void HideLegacyPromptRenderers()
    {
        SpriteRenderer[] spriteRenderers = promptObject.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.enabled = false;
        }

        MeshRenderer[] meshRenderers = promptObject.GetComponentsInChildren<MeshRenderer>(true);

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = false;
        }
    }

    private TextMesh CreatePromptText(string objectName, Color color, Vector3 localOffset, int sortingOrder)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(simplePromptRoot.transform, false);
        textObject.transform.localPosition = localOffset;
        textObject.transform.localRotation = Quaternion.identity;
        textObject.transform.localScale = Vector3.one;

        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = "E";
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = promptFontSize;
        textMesh.characterSize = promptCharacterSize;
        textMesh.color = color;

        MeshRenderer meshRenderer = textObject.GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = sortingOrder;
        return textMesh;
    }

    private void UpdatePromptPosition()
    {
        promptObject.transform.position = transform.position + promptWorldOffset;
        promptObject.transform.rotation = Quaternion.identity;
        promptObject.transform.localScale = GetLocalScaleForWorldScale(promptWorldScale, promptObject.transform.parent);

        if (simplePromptRoot != null)
        {
            simplePromptRoot.transform.localPosition = Vector3.zero;
            simplePromptRoot.transform.localRotation = Quaternion.identity;
            simplePromptRoot.transform.localScale = Vector3.one;
        }
    }

    private void UpdateCompletionGlow(bool promptVisible)
    {
        if (!usesCompletionGlow)
        {
            return;
        }

        bool shouldGlow = promptVisible
            && (GameProgress.CanExtendPencilAtSketchbook || GameProgress.CanColorVillageAtSketchbook);

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
                promptText.color = promptColor;
            }

            return;
        }

        float pulse = Mathf.SmoothStep(0f, 1f, (Mathf.Sin(Time.time * CompletionGlowPulseSpeed) + 1f) * 0.5f);

        if (promptText != null)
        {
            promptText.color = Color.Lerp(promptColor, new Color(1f, 0.82f, 0.34f, 1f), pulse);
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

    private Vector3 GetLocalScaleForWorldScale(Vector3 worldScale, Transform parent)
    {
        if (parent == null)
        {
            return worldScale;
        }

        Vector3 parentScale = parent.lossyScale;

        return new Vector3(
            DivideScale(worldScale.x, parentScale.x),
            DivideScale(worldScale.y, parentScale.y),
            DivideScale(worldScale.z, parentScale.z)
        );
    }

    private float DivideScale(float value, float divisor)
    {
        if (Mathf.Approximately(divisor, 0f))
        {
            return value;
        }

        return value / divisor;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, showDistance);
    }
}
