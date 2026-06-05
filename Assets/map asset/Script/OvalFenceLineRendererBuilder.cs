using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class OvalFenceLineRendererBuilder : MonoBehaviour
{
    [Header("Oval")]
    public float radiusX = 6f;
    public float radiusY = 2.5f;

    [Min(16)]
    public int railResolution = 128;

    [Min(4)]
    public int postCount = 36;

    [Header("Rail")]
    public float railGap = 0.35f;
    public float railWidth = 0.04f;

    [Header("Post")]
    public GameObject postPrefab;
    public Vector3 postPrefabScale = Vector3.one;

    [Header("Material")]
    public Material lineMaterial;

    [Header("Sorting")]
    public string sortingLayerName = "Default";
    public int railSortingOrder = 0;

    private const string GeneratedPrefix = "__GeneratedOvalFence_";

#if UNITY_EDITOR
    private bool rebuildQueued;
#endif

    private void OnEnable()
    {
        Rebuild();
    }

    [ContextMenu("Rebuild Fence")]
    public void Rebuild()
    {
        ClearGenerated();

        int safeRailResolution = Mathf.Max(16, railResolution);
        int safePostCount = Mathf.Max(4, postCount);

        CreateRail("UpperRail", 0f, railGap * 0.5f, safeRailResolution, railSortingOrder);
        CreateRail("LowerRail", 0f, -railGap * 0.5f, safeRailResolution, railSortingOrder);

        for (int i = 0; i < safePostCount; i++)
        {
            float t = Mathf.PI * 2f * i / safePostCount;
            CreatePost(i, t, GetPostAngle(t));
        }
    }

    private void CreateRail(string railName, float normalOffset, float yOffset, int resolution, int sortingOrder)
    {
        GameObject obj = CreateGeneratedObject(railName);
        LineRenderer lr = obj.AddComponent<LineRenderer>();

        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = resolution;

        lr.widthMultiplier = railWidth;
        lr.numCapVertices = 2;
        lr.numCornerVertices = 4;

        lr.sortingLayerName = sortingLayerName;
        lr.sortingOrder = sortingOrder;

        if (lineMaterial != null)
        {
            lr.sharedMaterial = lineMaterial;
        }

        for (int i = 0; i < resolution; i++)
        {
            float t = Mathf.PI * 2f * i / resolution;
            lr.SetPosition(i, GetEllipsePoint(t, normalOffset, yOffset));
        }
    }

    private void CreatePost(int index, float t, float angle)
    {
        if (postPrefab == null)
        {
            return;
        }

        Vector3 center = GetEllipsePoint(t, 0f, 0f);
        GameObject post = CreateGeneratedPrefabObject("Post_" + index.ToString("00"));
        post.transform.localPosition = center;
        post.transform.localRotation = Quaternion.Euler(0f, RemapPostAngleForEuler(angle), 0f);
        post.transform.localScale = postPrefabScale;
    }

    private GameObject CreateGeneratedPrefabObject(string objectName)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(postPrefab, transform) as GameObject;
            if (prefabInstance != null)
            {
                prefabInstance.name = GeneratedPrefix + objectName;
                ApplyGeneratedHideFlags(prefabInstance);
                return prefabInstance;
            }
        }
#endif

        GameObject obj = Instantiate(postPrefab, transform);
        obj.name = GeneratedPrefix + objectName;
        ApplyGeneratedHideFlags(obj);
        return obj;
    }

    private GameObject CreateGeneratedObject(string objectName)
    {
        GameObject obj = new GameObject(GeneratedPrefix + objectName);
        obj.transform.SetParent(transform, false);
        ApplyGeneratedHideFlags(obj);

        return obj;
    }

#if UNITY_EDITOR
    private void ApplyGeneratedHideFlags(GameObject obj)
    {
        if (!Application.isPlaying)
        {
            obj.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
        }
    }
#else
    private void ApplyGeneratedHideFlags(GameObject obj)
    {
    }
#endif

    private Vector3 GetEllipsePoint(float t, float normalOffset, float yOffset)
    {
        float x = Mathf.Cos(t) * radiusX;
        float y = Mathf.Sin(t) * radiusY;

        Vector2 basePoint = new Vector2(x, y);
        Vector2 normal = GetEllipseNormal(t);
        Vector2 result = basePoint + normal * normalOffset + Vector2.up * yOffset;

        return new Vector3(result.x, result.y, 0f);
    }

    private Vector2 GetEllipseNormal(float t)
    {
        Vector2 normal = new Vector2(
            Mathf.Cos(t) / radiusX,
            Mathf.Sin(t) / radiusY
        );

        return normal.normalized;
    }

    private float GetPostAngle(float t)
    {
        return Vector2.SignedAngle(Vector2.up, GetEllipseNormal(t));
    }

    private float RemapPostAngleForEuler(float angle)
    {
        float normalizedAngle = Mathf.Repeat(angle, 360f);

        if (normalizedAngle <= 90f)
        {
            return Mathf.Lerp(0f, 60f, normalizedAngle / 90f);
        }

        if (normalizedAngle <= 180f)
        {
            return Mathf.Lerp(120f, 180f, (normalizedAngle - 90f) / 90f);
        }

        if (normalizedAngle <= 270f)
        {
            return Mathf.Lerp(180f, 240f, (normalizedAngle - 180f) / 90f);
        }

        return Mathf.Lerp(300f, 360f, (normalizedAngle - 270f) / 90f);
    }

    [ContextMenu("Clear Generated")]
    public void ClearGenerated()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            if (child.name.StartsWith(GeneratedPrefix))
            {
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (rebuildQueued)
        {
            return;
        }

        rebuildQueued = true;
        EditorApplication.delayCall += () =>
        {
            rebuildQueued = false;

            if (this != null)
            {
                Rebuild();
            }
        };
    }
#endif
}
