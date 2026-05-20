using System.Collections.Generic;
using UnityEngine;

public class SketchWorldLineDrawing : MonoBehaviour
{
    [SerializeField] private float lineWidth = 0.06f;
    [SerializeField] private Color lineColor = new Color32(35, 32, 28, 255);
    [SerializeField] private int sortingOrder = 8;
    [SerializeField, Range(0f, 1f)] private float revealProgress;

    private readonly List<Vector3[]> strokes = new List<Vector3[]>();
    private readonly List<float> strokeLengths = new List<float>();
    private readonly List<LineRenderer> renderers = new List<LineRenderer>();
    private Material lineMaterial;
    private float totalLength;

    public float RevealProgress
    {
        get => revealProgress;
        set
        {
            revealProgress = Mathf.Clamp01(value);
            UpdateRenderers();
        }
    }

    public void Configure(float width, Color color, int order)
    {
        lineWidth = Mathf.Max(0.01f, width);
        lineColor = color;
        sortingOrder = order;
        EnsureLineMaterial();

        for (int i = 0; i < renderers.Count; i++)
        {
            ConfigureRenderer(renderers[i]);
        }

        UpdateRenderers();
    }

    public void SetStrokes(IEnumerable<Vector3[]> newStrokes)
    {
        ClearStrokes();

        foreach (Vector3[] stroke in newStrokes)
        {
            if (stroke == null || stroke.Length < 2)
            {
                continue;
            }

            Vector3[] copy = (Vector3[])stroke.Clone();
            float length = CalculateStrokeLength(copy);

            if (length <= 0f)
            {
                continue;
            }

            strokes.Add(copy);
            strokeLengths.Add(length);
            totalLength += length;

            LineRenderer lineRenderer = CreateRenderer($"GeneratedSketchStroke_{renderers.Count:00}");
            renderers.Add(lineRenderer);
        }

        UpdateRenderers();
    }

    private void ClearStrokes()
    {
        for (int i = renderers.Count - 1; i >= 0; i--)
        {
            if (renderers[i] != null)
            {
                Destroy(renderers[i].gameObject);
            }
        }

        strokes.Clear();
        strokeLengths.Clear();
        renderers.Clear();
        totalLength = 0f;
    }

    private LineRenderer CreateRenderer(string objectName)
    {
        GameObject lineObject = new GameObject(objectName);
        lineObject.transform.SetParent(transform, false);

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        ConfigureRenderer(lineRenderer);
        return lineRenderer;
    }

    private void ConfigureRenderer(LineRenderer lineRenderer)
    {
        EnsureLineMaterial();

        lineRenderer.useWorldSpace = false;
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.numCapVertices = 4;
        lineRenderer.numCornerVertices = 4;
        lineRenderer.sortingOrder = sortingOrder;
        lineRenderer.positionCount = 0;
    }

    private void EnsureLineMaterial()
    {
        if (lineMaterial != null)
        {
            lineMaterial.color = lineColor;
            return;
        }

        Shader shader = Shader.Find("Sprites/Default");
        lineMaterial = new Material(shader);
        lineMaterial.color = lineColor;
    }

    private void UpdateRenderers()
    {
        if (renderers.Count == 0)
        {
            return;
        }

        float remainingLength = totalLength * revealProgress;

        for (int i = 0; i < strokes.Count; i++)
        {
            LineRenderer lineRenderer = renderers[i];

            if (remainingLength <= 0f)
            {
                lineRenderer.positionCount = 0;
                continue;
            }

            if (remainingLength >= strokeLengths[i])
            {
                lineRenderer.positionCount = strokes[i].Length;
                lineRenderer.SetPositions(strokes[i]);
                remainingLength -= strokeLengths[i];
                continue;
            }

            Vector3[] partialStroke = BuildPartialStroke(strokes[i], remainingLength);
            lineRenderer.positionCount = partialStroke.Length;
            lineRenderer.SetPositions(partialStroke);
            remainingLength = 0f;
        }
    }

    private static Vector3[] BuildPartialStroke(Vector3[] stroke, float revealLength)
    {
        List<Vector3> points = new List<Vector3> { stroke[0] };
        float traversed = 0f;

        for (int i = 0; i < stroke.Length - 1; i++)
        {
            Vector3 start = stroke[i];
            Vector3 end = stroke[i + 1];
            float segmentLength = Vector3.Distance(start, end);

            if (segmentLength <= 0f)
            {
                continue;
            }

            if (traversed + segmentLength >= revealLength)
            {
                float segmentProgress = Mathf.Clamp01((revealLength - traversed) / segmentLength);
                points.Add(Vector3.Lerp(start, end, segmentProgress));
                break;
            }

            points.Add(end);
            traversed += segmentLength;
        }

        return points.ToArray();
    }

    private static float CalculateStrokeLength(Vector3[] stroke)
    {
        float length = 0f;

        for (int i = 0; i < stroke.Length - 1; i++)
        {
            length += Vector3.Distance(stroke[i], stroke[i + 1]);
        }

        return length;
    }

    private void OnDestroy()
    {
        if (lineMaterial != null)
        {
            Destroy(lineMaterial);
        }
    }
}
