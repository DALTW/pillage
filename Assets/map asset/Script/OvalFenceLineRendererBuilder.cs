using System.Collections.Generic;
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

            CreatePost(i, t);
        }

        if (gateEnabled)
        {
            float gateStartT = GetTForGateAngle(gateStartAngle);
            float gateEndT = GetTForGateAngle(gateEndAngle);
            CreatePost("GatePost_Start", gateStartT);
            CreatePost("GatePost_End", gateEndT);
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
        }
        else
        {
            lr.loop = false;
        }

        List<Vector3> points = BuildPostAnchoredRailPoints(normalOffset, yOffset, resolution);
        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
    }

    private List<Vector3> BuildPostAnchoredRailPoints(float normalOffset, float yOffset, int resolution)
    {
        int safePostCount = Mathf.Max(4, postCount);
        float postSpacing = Mathf.PI * 2f / safePostCount;
        int pointsPerPostSegment = Mathf.Max(1, Mathf.CeilToInt(resolution / (float)safePostCount));
        List<Vector3> points = new List<Vector3>();

        if (!gateEnabled)
        {
            for (int postIndex = 0; postIndex < safePostCount; postIndex++)
            {
                float segmentStart = postIndex * postSpacing;
                float segmentEnd = segmentStart + postSpacing;
                AppendRailInterval(points, segmentStart, segmentEnd, normalOffset, yOffset, pointsPerPostSegment, false);
            }

            return points;
        }

        float startT = GetTForGateAngle(gateStartAngle);
        float endT = GetTForGateAngle(gateEndAngle);
        float drawEndT = startT + Mathf.Repeat(endT - startT, Mathf.PI * 2f);
        float previousT = startT;

        for (int postIndex = Mathf.FloorToInt(startT / postSpacing) + 1; postIndex <= Mathf.CeilToInt(drawEndT / postSpacing); postIndex++)
        {
            float currentT = postIndex * postSpacing;
            if (currentT <= startT || currentT >= drawEndT)
            {
                continue;
            }

            int subdivisions = Mathf.Max(1, Mathf.CeilToInt(resolution * (currentT - previousT) / (Mathf.PI * 2f)));
            AppendRailInterval(points, previousT, currentT, normalOffset, yOffset, subdivisions, true);
            previousT = currentT;
        }

        int finalSubdivisions = Mathf.Max(1, Mathf.CeilToInt(resolution * (drawEndT - previousT) / (Mathf.PI * 2f)));
        AppendRailInterval(points, previousT, drawEndT, normalOffset, yOffset, finalSubdivisions, true);
        return points;
    }

    private void AppendRailInterval(
        List<Vector3> points,
        float startT,
        float endT,
        float normalOffset,
        float yOffset,
        int subdivisions,
        bool includeEnd)
    {
        if (points.Count == 0 || (points[points.Count - 1] - GetEllipsePoint(startT, normalOffset, yOffset)).sqrMagnitude > 0.000001f)
        {
            points.Add(GetEllipsePoint(startT, normalOffset, yOffset));
        }

        int lastStep = includeEnd ? subdivisions : subdivisions - 1;
        for (int step = 1; step <= lastStep; step++)
        {
            float progress = step / (float)subdivisions;
            float t = Mathf.Lerp(startT, endT, progress);
            points.Add(GetEllipsePoint(t, normalOffset, yOffset));
        }
    }

    private void CreatePost(int index, float t)
    {
        CreatePost("Post_" + index.ToString("00"), t);
    }

    private void CreatePost(string postName, float t)
    {
        if (postPrefab == null)
        {
            return;
        }

        Vector3 center = GetEllipsePoint(t, 0f, 0f);
        GameObject post = CreateGeneratedPrefabObject(postName);
        post.transform.localPosition = center;
        post.transform.localRotation = Quaternion.identity;
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
