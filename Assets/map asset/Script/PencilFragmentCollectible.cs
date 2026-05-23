using UnityEngine;

public class PencilFragmentCollectible : MonoBehaviour, IInteractable
{
    [SerializeField] private float promptDistance = 1.35f;
    [SerializeField] private PencilFragmentSource source = PencilFragmentSource.Tree;

    private const int BodySortingOrder = 68;
    private const int PromptSortingOrder = 72;

    private static Sprite pixelSprite;

    private Transform player;
    private GameObject promptObject;
    private bool collected;

    public void Configure(PencilFragmentSource pencilSource)
    {
        source = pencilSource;

        if (GameProgress.HasCollectedPencilFragmentFrom(source))
        {
            collected = true;
            Destroy(gameObject);
        }
    }

    public void Interact(GameObject interactor)
    {
        if (collected || GameProgress.HasCollectedPencilFragmentFrom(source) || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > promptDistance)
        {
            return;
        }

        collected = true;
        GameProgress.CollectPencilFragmentFrom(source);
        PencilFragmentHud.ShowCollected();
        Destroy(gameObject);
    }

    private void Awake()
    {
        if (GameProgress.HasCollectedPencilFragmentFrom(source))
        {
            Destroy(gameObject);
            return;
        }

        EnsureVisual();
        EnsurePrompt();
        EnsureCollider();
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (GameProgress.HasCollectedPencilFragmentFrom(source))
        {
            Destroy(gameObject);
            return;
        }

        if (player == null)
        {
            FindPlayer();
        }

        if (promptObject == null)
        {
            return;
        }

        bool shouldShowPrompt = player != null
            && Vector2.Distance(transform.position, player.position) <= promptDistance;
        promptObject.SetActive(shouldShowPrompt);
    }

    private void OnDisable()
    {
        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void EnsureVisual()
    {
        CreatePart("GeneratedPencilFragmentBody", new Vector3(-0.06f, 0f, 0f), new Vector3(0.48f, 0.12f, 1f), new Color32(236, 181, 51, 255), BodySortingOrder);
        CreatePart("GeneratedPencilFragmentWood", new Vector3(0.24f, 0f, 0f), new Vector3(0.16f, 0.12f, 1f), new Color32(203, 143, 77, 255), BodySortingOrder + 1);
        CreatePart("GeneratedPencilFragmentLead", new Vector3(0.34f, 0f, 0f), new Vector3(0.08f, 0.08f, 1f), new Color32(44, 40, 36, 255), BodySortingOrder + 2);
        CreatePart("GeneratedPencilFragmentPaintEdge", new Vector3(-0.33f, 0f, 0f), new Vector3(0.08f, 0.13f, 1f), new Color32(190, 66, 72, 255), BodySortingOrder + 2);

        SketchWorldLineDrawing outline = gameObject.AddComponent<SketchWorldLineDrawing>();
        outline.Configure(0.035f, new Color32(38, 34, 28, 255), BodySortingOrder + 3);
        outline.SetStrokes(new[]
        {
            Points(-0.38f, -0.08f, 0.2f, -0.08f, 0.4f, 0f, 0.2f, 0.08f, -0.38f, 0.08f, -0.38f, -0.08f),
            Points(0.2f, -0.08f, 0.2f, 0.08f),
            Points(-0.28f, -0.08f, -0.28f, 0.08f)
        });
        outline.RevealProgress = 1f;
    }

    private void EnsurePrompt()
    {
        promptObject = new GameObject("GeneratedPencilFragmentPrompt");
        promptObject.transform.SetParent(transform, false);
        promptObject.transform.localPosition = new Vector3(0f, 0.46f, 0f);

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
        circleCollider.radius = 0.5f;
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }

    private void CreatePart(string objectName, Vector3 localPosition, Vector3 localScale, Color color, int sortingOrder)
    {
        GameObject partObject = new GameObject(objectName);
        partObject.transform.SetParent(transform, false);
        partObject.transform.localPosition = localPosition;
        partObject.transform.localScale = localScale;

        SpriteRenderer spriteRenderer = partObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetPixelSprite();
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = sortingOrder;
    }

    private static Sprite GetPixelSprite()
    {
        if (pixelSprite != null)
        {
            return pixelSprite;
        }

        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        pixelSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        pixelSprite.name = "GeneratedPencilFragmentPixel";
        return pixelSprite;
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
