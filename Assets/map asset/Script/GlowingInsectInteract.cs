using System.Collections;
using UnityEngine;

public class GlowingInsectInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float promptDistance = 1.45f;
    [SerializeField] private float hoverAmplitude = 0.12f;
    [SerializeField] private float hoverSpeed = 4.5f;
    [SerializeField] private float flutterSpeed = 18f;

    private const int OuterGlowSortingOrder = 57;
    private const int InnerGlowSortingOrder = 58;
    private const int CoreSortingOrder = 59;
    private const int PromptSortingOrder = 70;

    private static Sprite glowSprite;

    private Transform player;
    private GameObject visualRoot;
    private GameObject promptObject;
    private SpriteRenderer outerGlowRenderer;
    private SpriteRenderer innerGlowRenderer;
    private Transform leftWing;
    private Transform rightWing;
    private Vector3 baseLocalPosition;
    private float seed;
    private bool captured;
    private bool dockedAtFishingRod;

    public static GlowingInsectInteract Spawn(Vector3 worldPosition, Transform parent)
    {
        GameObject insectObject = new GameObject("GeneratedGlowingInsect");

        if (parent != null)
        {
            insectObject.transform.SetParent(parent, true);
        }

        insectObject.transform.position = worldPosition;
        return insectObject.AddComponent<GlowingInsectInteract>();
    }

    public void Interact(GameObject interactor)
    {
        if (captured || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > promptDistance)
        {
            return;
        }

        if (!FishingRodInteract.TryReserveInsectDockPosition(out Vector3 dockPosition, out Transform dockParent))
        {
            return;
        }

        captured = true;

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }

        SetColliderEnabled(false);
        StartCoroutine(FlyToFishingRodRoutine(dockPosition, dockParent));
    }

    private void Awake()
    {
        seed = Random.Range(0f, 1000f);
        baseLocalPosition = transform.localPosition;
        EnsureVisual();
        EnsurePrompt();
        EnsureCollider();
    }

    private void OnEnable()
    {
        FindPlayer();
        baseLocalPosition = transform.localPosition;

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (captured || dockedAtFishingRod)
        {
            return;
        }

        if (player == null)
        {
            FindPlayer();
        }

        AnimateInsect();
        UpdatePrompt();
    }

    private void OnDisable()
    {
        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void AnimateInsect()
    {
        float time = Time.time + seed;
        float hover = Mathf.Sin(time * hoverSpeed) * hoverAmplitude;
        float pulse = (Mathf.Sin(time * hoverSpeed * 1.45f) + 1f) * 0.5f;

        transform.localPosition = baseLocalPosition + new Vector3(0f, hover, 0f);

        if (visualRoot != null)
        {
            visualRoot.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(time * 2.2f) * 7f);
        }

        if (outerGlowRenderer != null)
        {
            outerGlowRenderer.transform.localScale = Vector3.one * Mathf.Lerp(0.74f, 0.98f, pulse);
            outerGlowRenderer.color = new Color(1f, 0.92f, 0.18f, Mathf.Lerp(0.18f, 0.36f, pulse));
        }

        if (innerGlowRenderer != null)
        {
            innerGlowRenderer.transform.localScale = Vector3.one * Mathf.Lerp(0.34f, 0.48f, pulse);
            innerGlowRenderer.color = new Color(1f, 0.98f, 0.48f, Mathf.Lerp(0.48f, 0.82f, pulse));
        }

        float wingAngle = Mathf.Sin(time * flutterSpeed) * 24f;

        if (leftWing != null)
        {
            leftWing.localRotation = Quaternion.Euler(0f, 0f, 22f + wingAngle);
        }

        if (rightWing != null)
        {
            rightWing.localRotation = Quaternion.Euler(0f, 0f, -22f - wingAngle);
        }
    }

    private void UpdatePrompt()
    {
        if (promptObject == null)
        {
            return;
        }

        bool shouldShowPrompt = player != null
            && FishingRodInteract.CanAcceptGlowingInsect()
            && Vector2.Distance(transform.position, player.position) <= promptDistance;
        promptObject.SetActive(shouldShowPrompt);
    }

    public void MarkDockedAtFishingRod(Vector3 worldPosition)
    {
        dockedAtFishingRod = true;
        captured = true;
        transform.position = worldPosition;
        baseLocalPosition = transform.localPosition;

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }

        SetColliderEnabled(false);
    }

    private IEnumerator FlyToFishingRodRoutine(Vector3 targetPosition, Transform targetParent)
    {
        Vector3 startPosition = transform.position;
        Vector3 startScale = transform.localScale;
        const float flightDuration = 0.72f;
        float elapsed = 0f;

        while (elapsed < flightDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / flightDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.48f;

            transform.position = position;
            transform.localScale = Vector3.Lerp(startScale, Vector3.one * 0.82f, easedProgress);

            if (visualRoot != null)
            {
                visualRoot.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin((Time.time + seed) * 12f) * 18f);
            }

            float wingAngle = Mathf.Sin((Time.time + seed) * flutterSpeed * 1.4f) * 30f;
            if (leftWing != null)
            {
                leftWing.localRotation = Quaternion.Euler(0f, 0f, 22f + wingAngle);
            }

            if (rightWing != null)
            {
                rightWing.localRotation = Quaternion.Euler(0f, 0f, -22f - wingAngle);
            }

            yield return null;
        }

        if (targetParent != null)
        {
            transform.SetParent(targetParent, true);
        }

        transform.position = targetPosition;
        FishingRodInteract.CompleteInsectDock(transform);
    }

    private void SetColliderEnabled(bool enabled)
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = enabled;
        }
    }

    private void EnsureVisual()
    {
        visualRoot = new GameObject("GeneratedGlowingInsectVisual");
        visualRoot.transform.SetParent(transform, false);

        outerGlowRenderer = CreateSpriteRenderer(
            "GeneratedGlowingInsectOuterGlow",
            visualRoot.transform,
            Vector3.zero,
            Vector3.one * 0.86f,
            new Color(1f, 0.92f, 0.18f, 0.28f),
            OuterGlowSortingOrder);
        innerGlowRenderer = CreateSpriteRenderer(
            "GeneratedGlowingInsectInnerGlow",
            visualRoot.transform,
            Vector3.zero,
            Vector3.one * 0.42f,
            new Color(1f, 0.98f, 0.48f, 0.66f),
            InnerGlowSortingOrder);

        leftWing = CreateSpriteRenderer(
            "GeneratedGlowingInsectLeftWing",
            visualRoot.transform,
            new Vector3(-0.12f, 0.02f, 0f),
            new Vector3(0.17f, 0.08f, 1f),
            new Color(1f, 1f, 0.82f, 0.62f),
            InnerGlowSortingOrder).transform;
        rightWing = CreateSpriteRenderer(
            "GeneratedGlowingInsectRightWing",
            visualRoot.transform,
            new Vector3(0.12f, 0.02f, 0f),
            new Vector3(0.17f, 0.08f, 1f),
            new Color(1f, 1f, 0.82f, 0.62f),
            InnerGlowSortingOrder).transform;

        CreateSpriteRenderer(
            "GeneratedGlowingInsectCore",
            visualRoot.transform,
            Vector3.zero,
            Vector3.one * 0.13f,
            new Color(1f, 0.98f, 0.4f, 1f),
            CoreSortingOrder);
    }

    private void EnsurePrompt()
    {
        promptObject = new GameObject("GeneratedGlowingInsectPrompt");
        promptObject.transform.SetParent(transform, false);
        promptObject.transform.localPosition = new Vector3(0f, 0.58f, 0f);

        TextMesh textMesh = promptObject.AddComponent<TextMesh>();
        textMesh.text = "E";
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 64;
        textMesh.characterSize = 0.075f;
        textMesh.color = new Color32(35, 32, 28, 255);

        MeshRenderer meshRenderer = promptObject.GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = PromptSortingOrder;

        promptObject.SetActive(false);
    }

    private void EnsureCollider()
    {
        CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        circleCollider.radius = 0.42f;
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }

    private static SpriteRenderer CreateSpriteRenderer(
        string objectName,
        Transform parent,
        Vector3 localPosition,
        Vector3 localScale,
        Color color,
        int sortingOrder)
    {
        GameObject spriteObject = new GameObject(objectName);
        spriteObject.transform.SetParent(parent, false);
        spriteObject.transform.localPosition = localPosition;
        spriteObject.transform.localScale = localScale;

        SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetGlowSprite();
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = sortingOrder;
        return spriteRenderer;
    }

    private static Sprite GetGlowSprite()
    {
        if (glowSprite != null)
        {
            return glowSprite;
        }

        const int textureSize = 32;
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        texture.hideFlags = HideFlags.HideAndDontSave;
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        Vector2 center = new Vector2((textureSize - 1) * 0.5f, (textureSize - 1) * 0.5f);
        float radius = textureSize * 0.5f;

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01(1f - distance / radius);
                alpha *= alpha;
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        glowSprite = Sprite.Create(texture, new Rect(0f, 0f, textureSize, textureSize), new Vector2(0.5f, 0.5f), textureSize);
        glowSprite.name = "GeneratedGlowingInsectSprite";
        return glowSprite;
    }
}
