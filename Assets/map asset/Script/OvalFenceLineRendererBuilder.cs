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

    [Header("Gate")]
    public bool gateEnabled = true;
    public float gateStartAngle = 80f;
    public float gateEndAngle = 100f;

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
            if (gateEnabled && IsAngleInGate(GetGateAngle(t)))
            {
                continue;
            }

            CreatePost(i, t, GetPostAngle(t));
        }

        if (gateEnabled)
        {
            float gateStartT = GetTForGateAngle(gateStartAngle);
            float gateEndT = GetTForGateAngle(gateEndAngle);
            CreatePost("GatePost_Start", gateStartT, GetPostAngle(gateStartT));
            CreatePost("GatePost_End", gateEndT, GetPostAngle(gateEndT));
        }
    }

    private void CreateRail(string railName, float normalOffset, float yOffset, int resolution, int sortingOrder)
    {
        GameObject obj = CreateGeneratedObject(railName);
        LineRenderer lr = obj.AddComponent<LineRenderer>();

        lr.useWorldSpace = false;

        lr.widthMultiplier = railWidth;
        lr.numCapVertices = 2;
        lr.numCornerVertices = 4;

        lr.sortingLayerName = sortingLayerName;
        lr.sortingOrder = sortingOrder;

        if (lineMaterial != null)
        {
            lr.sharedMaterial = lineMaterial;
        }

        if (!gateEnabled)
        {
            lr.loop = true;
            lr.positionCount = resolution;

            for (int i = 0; i < resolution; i++)
            {
                float t = Mathf.PI * 2f * i / resolution;
                lr.SetPosition(i, GetEllipsePoint(t, normalOffset, yOffset));
            }

            return;
        }

        lr.loop = false;

        float startT = GetTForGateAngle(gateStartAngle);
        float endT = GetTForGateAngle(gateEndAngle);
        float span = Mathf.Repeat(endT - startT, Mathf.PI * 2f);
        int pointCount = Mathf.Max(2, Mathf.CeilToInt(resolution * span / (Mathf.PI * 2f)) + 1);
        lr.positionCount = pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float progress = pointCount > 1 ? i / (float)(pointCount - 1) : 0f;
            float t = startT + span * progress;
            lr.SetPosition(i, GetEllipsePoint(t, normalOffset, yOffset));
        }
    }

    private void CreatePost(int index, float t, float angle)
    {
        CreatePost("Post_" + index.ToString("00"), t, angle);
    }

    private void CreatePost(string postName, float t, float angle)
    {
        if (postPrefab == null)
        {
            return;
        }

        Vector3 center = GetEllipsePoint(t, 0f, 0f);
        GameObject post = CreateGeneratedPrefabObject(postName);
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

    private float GetGateAngle(float t)
    {
        return Mathf.Repeat(-GetPostAngle(t), 360f);
    }

    private float GetTForGateAngle(float gateAngle)
    {
        float radians = gateAngle * Mathf.Deg2Rad;
        Vector2 normal = new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
        return Mathf.Repeat(Mathf.Atan2(normal.y * radiusY, normal.x * radiusX), Mathf.PI * 2f);
    }

    private bool IsAngleInGate(float angle)
    {
        float normalizedAngle = Mathf.Repeat(angle, 360f);
        float normalizedStart = Mathf.Repeat(gateStartAngle, 360f);
        float normalizedEnd = Mathf.Repeat(gateEndAngle, 360f);

        if (normalizedStart <= normalizedEnd)
        {
            return normalizedAngle >= normalizedStart && normalizedAngle <= normalizedEnd;
        }

        return normalizedAngle >= normalizedStart || normalizedAngle <= normalizedEnd;
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
