using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class SketchNatureLineRendererBuilder : MonoBehaviour
{
    public enum NatureType
    {
        PineTree,
        Grass
    }

    [Header("Drawing")]
    public NatureType natureType = NatureType.PineTree;
    public float scale = 1f;
    public bool includeGroundStroke = true;

    [Header("Line")]
    public float lineWidth = 0.065f;
    public Color lineColor = new Color32(35, 32, 28, 255);
    public Material lineMaterial;

    [Header("Sorting")]
    public string sortingLayerName = "Default";
    public int sortingOrder = 23;

    [Header("Editor")]
    public bool autoRebuild = true;

    private const string GeneratedPrefix = "__GeneratedSketchNature_";

#if UNITY_EDITOR
    private bool rebuildQueued;
#endif

    private void OnEnable()
    {
        Rebuild();
    }

    [ContextMenu("Rebuild Nature Drawing")]
    public void Rebuild()
    {
        ClearGenerated();

        float safeScale = Mathf.Max(0.05f, scale);
        List<Vector3[]> strokes = natureType == NatureType.PineTree
            ? BuildPineTreeStrokes(safeScale)
            : BuildGrassStrokes(safeScale);

        for (int i = 0; i < strokes.Count; i++)
        {
            CreateStroke("Stroke_" + i.ToString("00"), strokes[i], sortingOrder);
        }

        if (includeGroundStroke)
        {
            float halfWidth = natureType == NatureType.PineTree ? 1.16f * safeScale : 0.82f * safeScale;
            CreateStroke("GroundStroke", BuildGroundStroke(halfWidth), sortingOrder - 1);
        }
    }

    [ContextMenu("Clear Generated")]
    public void ClearGenerated()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            if (!child.name.StartsWith(GeneratedPrefix))
            {
                continue;
            }

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

    private void CreateStroke(string strokeName, Vector3[] points, int order)
    {
        if (points == null || points.Length < 2)
        {
            return;
        }

        GameObject obj = CreateGeneratedObject(strokeName);
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = false;
        lineRenderer.positionCount = points.Length;
        lineRenderer.widthMultiplier = Mathf.Max(0.01f, lineWidth);
        lineRenderer.numCapVertices = 2;
        lineRenderer.numCornerVertices = 4;
        lineRenderer.sortingLayerName = sortingLayerName;
        lineRenderer.sortingOrder = order;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        if (lineMaterial != null)
        {
            lineRenderer.sharedMaterial = lineMaterial;
        }

        lineRenderer.SetPositions(points);
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

    private void OnValidate()
    {
        scale = Mathf.Max(0.05f, scale);
        lineWidth = Mathf.Max(0.01f, lineWidth);

        if (!autoRebuild || rebuildQueued)
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
#else
    private void ApplyGeneratedHideFlags(GameObject obj)
    {
    }
#endif

    private static List<Vector3[]> BuildPineTreeStrokes(float scale)
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        AddOutsidePine(strokes, 0f, 0f, scale);
        return strokes;
    }

    private static List<Vector3[]> BuildGrassStrokes(float scale)
    {
        List<Vector3[]> strokes = new List<Vector3[]>
        {
            ScaledPoints(scale, -0.78f, 0.02f, -0.96f, 0.54f, -0.86f, 1.08f, -0.58f, 1.52f),
            ScaledPoints(scale, -0.55f, 0.02f, -0.68f, 0.44f, -0.55f, 0.9f, -0.25f, 1.28f),
            ScaledPoints(scale, -0.3f, 0f, -0.38f, 0.58f, -0.24f, 1.08f, 0.02f, 1.56f),
            ScaledPoints(scale, -0.08f, 0f, -0.1f, 0.52f, 0.06f, 1.08f, 0.28f, 1.38f),
            ScaledPoints(scale, 0.18f, 0.02f, 0.28f, 0.52f, 0.22f, 1.08f, 0.48f, 1.48f),
            ScaledPoints(scale, 0.44f, 0.02f, 0.58f, 0.48f, 0.54f, 0.98f, 0.82f, 1.3f),
            ScaledPoints(scale, 0.72f, 0.02f, 0.96f, 0.46f, 0.94f, 0.94f, 0.68f, 1.18f),
            ScaledPoints(scale, -0.9f, 0.16f, -0.44f, 0.28f, 0.02f, 0.16f, 0.48f, 0.3f, 0.94f, 0.16f),
            ScaledPoints(scale, -0.7f, 0.42f, -0.28f, 0.56f, 0.16f, 0.42f, 0.58f, 0.56f),
            ScaledPoints(scale, -0.4f, 0.78f, -0.1f, 0.92f, 0.24f, 0.78f, 0.5f, 0.94f)
        };

        return strokes;
    }

    private static Vector3[] BuildGroundStroke(float halfWidth)
    {
        float strokeHalfWidth = Mathf.Clamp(halfWidth * 0.72f, 0.32f, 2.2f);
        float unevenLift = Mathf.Clamp(strokeHalfWidth * 0.08f, 0.035f, 0.12f);
        return Points(-strokeHalfWidth, 0f, -strokeHalfWidth * 0.35f, unevenLift, strokeHalfWidth * 0.28f, -unevenLift, strokeHalfWidth, 0f);
    }

    private static void AddOutsidePine(List<Vector3[]> strokes, float centerX, float groundY, float scale)
    {
        strokes.Add(PinePoints(centerX, groundY, scale, 0f, 2.48f, -0.7f, 1.62f, -0.42f, 1.62f, -0.92f, 0.86f, -0.5f, 0.86f, -1.16f, 0f, 1.16f, 0f, 0.5f, 0.86f, 0.92f, 0.86f, 0.42f, 1.62f, 0.7f, 1.62f, 0f, 2.48f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.14f, 0f, -0.14f, -0.34f, 0.14f, -0.34f, 0.14f, 0f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.5f, 0.86f, -0.16f, 1.22f, 0.16f, 0.86f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.42f, 1.62f, -0.12f, 1.92f, 0.2f, 1.62f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.18f, 2.08f, 0.18f, 2.08f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.56f, 1.52f, -0.08f, 1.72f, 0.48f, 1.52f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.82f, 1.05f, -0.24f, 1.22f, 0.34f, 1.04f, 0.82f, 1.18f));
        strokes.Add(PinePoints(centerX, groundY, scale, -1.0f, 0.5f, -0.46f, 0.72f, 0.08f, 0.52f, 0.62f, 0.72f, 1.02f, 0.5f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.9f, 0.14f, -0.34f, 0.28f, 0.18f, 0.1f, 0.76f, 0.24f));
        strokes.Add(PinePoints(centerX, groundY, scale, -0.06f, -0.3f, -0.1f, -0.04f, -0.32f, 0.34f));
        strokes.Add(PinePoints(centerX, groundY, scale, 0.06f, -0.28f, 0.1f, -0.02f, 0.34f, 0.3f));
    }

    private static Vector3[] PinePoints(float centerX, float groundY, float scale, params float[] values)
    {
        int pointCount = values.Length / 2;
        Vector3[] points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector3(
                centerX + values[i * 2] * scale,
                groundY + values[i * 2 + 1] * scale,
                0f);
        }

        return points;
    }

    private static Vector3[] ScaledPoints(float scale, params float[] values)
    {
        int pointCount = values.Length / 2;
        Vector3[] points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector3(values[i * 2] * scale, values[i * 2 + 1] * scale, 0f);
        }

        return points;
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
