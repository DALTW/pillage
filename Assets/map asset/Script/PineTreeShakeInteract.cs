using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PineTreeShakeInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float promptDistance = 2.15f;
    [SerializeField] private float shakeDuration = 0.72f;
    [SerializeField] private float maxShakeAngle = 11f;
    [SerializeField, Range(0f, 1f)] private float pencilFragmentChance = 0.1f;

    private const float DropLineWidth = 0.045f;
    private const int DropSortingOrder = 68;
    private const int PromptSortingOrder = 70;

    private Transform player;
    private GameObject promptObject;
    private CircleCollider2D interactionCollider;
    private Quaternion baseLocalRotation;
    private float treeScale = 1f;
    private bool isShaking;

    public void Configure(float scale, int promptSortingOrder)
    {
        treeScale = Mathf.Max(0.1f, scale);
        promptDistance = Mathf.Max(promptDistance, 1.75f * treeScale);
        EnsurePrompt(promptSortingOrder);
        EnsureCollider();
        UpdatePromptPosition();
        UpdateColliderSize();
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled || !CanInteractWithTree() || isShaking || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > promptDistance)
        {
            return;
        }

        StartCoroutine(ShakeAndDropRoutine());
    }

    private void Awake()
    {
        baseLocalRotation = transform.localRotation;
        EnsurePrompt(PromptSortingOrder);
        EnsureCollider();
    }

    private void OnEnable()
    {
        FindPlayer();
        baseLocalRotation = transform.localRotation;

        if (interactionCollider != null)
        {
            interactionCollider.enabled = CanInteractWithTree();
        }

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!CanInteractWithTree())
        {
            SetInteractionAvailable(false);
            return;
        }

        SetInteractionAvailable(true);

        if (player == null)
        {
            FindPlayer();
        }

        if (promptObject == null)
        {
            return;
        }

        bool shouldShowPrompt = !isShaking
            && player != null
            && Vector2.Distance(transform.position, player.position) <= promptDistance;
        promptObject.SetActive(shouldShowPrompt);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isShaking = false;
        transform.localRotation = baseLocalRotation;

        if (interactionCollider != null)
        {
            interactionCollider.enabled = false;
        }

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private IEnumerator ShakeAndDropRoutine()
    {
        isShaking = true;

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / shakeDuration);
            float fade = 1f - Mathf.SmoothStep(0f, 1f, progress);
            float angle = Mathf.Sin(progress * Mathf.PI * 10f) * maxShakeAngle * fade;

            transform.localRotation = baseLocalRotation * Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }

        transform.localRotation = baseLocalRotation;
        SpawnDropItem();
        isShaking = false;
    }

    private void SpawnDropItem()
    {
        bool isPencilFragment = GameProgress.CanDropPencilFragmentFrom(PencilFragmentSource.Tree) && Random.value < pencilFragmentChance;
        string objectName = isPencilFragment ? "GeneratedPencilFragmentDrop" : "GeneratedPineconeDrop";
        Vector3 startPosition = transform.position + new Vector3(Random.Range(-0.25f, 0.25f), 2.28f * treeScale, 0f);
        Vector3 endPosition = transform.position + new Vector3(Random.Range(-0.48f, 0.48f), 0.08f, 0f);

        GameObject dropObject = new GameObject(objectName);
        Transform parent = transform.parent;

        if (parent != null)
        {
            dropObject.transform.SetParent(parent, true);
        }

        dropObject.transform.position = startPosition;
        dropObject.transform.localScale = Vector3.one;

        if (isPencilFragment)
        {
            GameProgress.DropPencilFragmentFrom(PencilFragmentSource.Tree);
            PencilFragmentCollectible collectible = dropObject.AddComponent<PencilFragmentCollectible>();
            collectible.Configure(PencilFragmentSource.Tree);
            StartCoroutine(AnimateDropRoutine(dropObject.transform, startPosition, endPosition));
            return;
        }

        SketchWorldLineDrawing drawing = dropObject.AddComponent<SketchWorldLineDrawing>();
        drawing.Configure(
            DropLineWidth,
            new Color32(92, 55, 25, 255),
            DropSortingOrder);
        drawing.SetStrokes(BuildPineconeStrokes());
        drawing.RevealProgress = 1f;

        StartCoroutine(AnimateDropRoutine(dropObject.transform, startPosition, endPosition));
        Destroy(dropObject, 10f);
    }

    private IEnumerator AnimateDropRoutine(Transform dropTransform, Vector3 startPosition, Vector3 endPosition)
    {
        const float dropDuration = 0.54f;
        float elapsed = 0f;

        while (elapsed < dropDuration && dropTransform != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / dropDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector3 position = Vector3.Lerp(startPosition, endPosition, easedProgress);
            position.y += Mathf.Sin(progress * Mathf.PI) * 0.18f;

            dropTransform.position = position;
            dropTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-35f, 24f, progress));
            yield return null;
        }

        if (dropTransform != null)
        {
            dropTransform.position = endPosition;
            dropTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-16f, 16f));
        }
    }

    private void EnsurePrompt(int sortingOrder)
    {
        if (promptObject == null)
        {
            promptObject = new GameObject("GeneratedPineTreePrompt");
            promptObject.transform.SetParent(transform, false);

            TextMesh textMesh = promptObject.AddComponent<TextMesh>();
            textMesh.text = "E";
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.fontSize = 64;
            textMesh.characterSize = 0.075f;
            textMesh.color = new Color32(35, 32, 28, 255);
        }

        MeshRenderer meshRenderer = promptObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }

        UpdatePromptPosition();
        promptObject.SetActive(false);
    }

    private void EnsureCollider()
    {
        if (interactionCollider != null)
        {
            return;
        }

        interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
        UpdateColliderSize();
    }

    private void UpdatePromptPosition()
    {
        if (promptObject != null)
        {
            promptObject.transform.localPosition = new Vector3(0f, 2.85f * treeScale, 0f);
        }
    }

    private void UpdateColliderSize()
    {
        if (interactionCollider == null)
        {
            return;
        }

        interactionCollider.radius = 1.25f * treeScale;
        interactionCollider.offset = new Vector2(0f, 1.05f * treeScale);
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }

    private static bool CanInteractWithTree()
    {
        return !GameProgress.HasDroppedPencilFragmentFrom(PencilFragmentSource.Tree);
    }

    private void SetInteractionAvailable(bool available)
    {
        if (interactionCollider != null && interactionCollider.enabled != available)
        {
            interactionCollider.enabled = available;
        }

        if (!available && promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private static List<Vector3[]> BuildPineconeStrokes()
    {
        return new List<Vector3[]>
        {
            Points(0f, 0.24f, -0.2f, 0.13f, -0.24f, -0.08f, -0.11f, -0.28f, 0.1f, -0.29f, 0.25f, -0.09f, 0.2f, 0.13f, 0f, 0.24f),
            Points(-0.16f, 0.1f, 0.02f, -0.02f, 0.17f, 0.1f),
            Points(-0.2f, -0.08f, -0.02f, -0.2f, 0.18f, -0.08f),
            Points(-0.04f, 0.19f, -0.08f, 0.04f, -0.01f, -0.1f, -0.05f, -0.25f),
            Points(0.06f, 0.19f, 0.09f, 0.04f, 0.03f, -0.1f, 0.07f, -0.25f)
        };
    }

    private static List<Vector3[]> BuildPencilFragmentStrokes()
    {
        return new List<Vector3[]>
        {
            Points(-0.28f, -0.06f, 0.12f, -0.06f, 0.28f, 0.04f, 0.12f, 0.14f, -0.28f, 0.14f, -0.28f, -0.06f),
            Points(0.12f, -0.06f, 0.12f, 0.14f),
            Points(0.18f, 0f, 0.28f, 0.04f, 0.18f, 0.09f),
            Points(-0.19f, -0.02f, -0.19f, 0.1f),
            Points(-0.08f, -0.02f, -0.08f, 0.1f)
        };
    }

    private static Vector3[] Points(params float[] values)
    {
        int pointCount = values.Length / 2;
        Vector3[] points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector3(values[i * 2], values[i * 2 + 1], 0f);
        }

        return points;
    }
}
