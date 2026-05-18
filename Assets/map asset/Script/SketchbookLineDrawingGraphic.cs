using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class SketchbookLineDrawingGraphic : MaskableGraphic
{
    [SerializeField] private float lineWidth = 5f;
    [SerializeField, Range(0f, 1f)] private float revealProgress = 1f;

    private readonly List<Vector2[]> strokes = new List<Vector2[]>();
    private readonly List<float> strokeLengths = new List<float>();
    private float totalLength;

    public float LineWidth
    {
        get => lineWidth;
        set
        {
            lineWidth = Mathf.Max(0.1f, value);
            SetVerticesDirty();
        }
    }

    public float RevealProgress
    {
        get => revealProgress;
        set
        {
            revealProgress = Mathf.Clamp01(value);
            SetVerticesDirty();
        }
    }

    public void SetStrokes(IEnumerable<Vector2[]> newStrokes)
    {
        strokes.Clear();
        strokeLengths.Clear();
        totalLength = 0f;

        foreach (Vector2[] stroke in newStrokes)
        {
            if (stroke == null || stroke.Length < 2)
            {
                continue;
            }

            Vector2[] copy = (Vector2[])stroke.Clone();
            float length = CalculateStrokeLength(copy);

            if (length <= 0f)
            {
                continue;
            }

            strokes.Add(copy);
            strokeLengths.Add(length);
            totalLength += length;
        }

        SetVerticesDirty();
    }

    public Vector2 GetPointAtProgress(float progress)
    {
        if (strokes.Count == 0)
        {
            return Vector2.zero;
        }

        float targetDistance = totalLength * Mathf.Clamp01(progress);
        float traversed = 0f;

        for (int i = 0; i < strokes.Count; i++)
        {
            float strokeLength = strokeLengths[i];

            if (targetDistance <= traversed + strokeLength)
            {
                return GetPointAtDistance(strokes[i], targetDistance - traversed);
            }

            traversed += strokeLength;
        }

        Vector2[] lastStroke = strokes[strokes.Count - 1];
        return lastStroke[lastStroke.Length - 1];
    }

    public Vector2 GetTangentAtProgress(float progress)
    {
        float sampleDistance = totalLength > 0f ? Mathf.Clamp(12f / totalLength, 0.002f, 0.03f) : 0.01f;
        Vector2 before = GetPointAtProgress(Mathf.Clamp01(progress - sampleDistance));
        Vector2 after = GetPointAtProgress(Mathf.Clamp01(progress + sampleDistance));
        Vector2 tangent = after - before;

        if (tangent.sqrMagnitude <= 0.001f)
        {
            return Vector2.right;
        }

        return tangent.normalized;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (strokes.Count == 0 || revealProgress <= 0f)
        {
            return;
        }

        float remainingLength = totalLength * revealProgress;

        for (int i = 0; i < strokes.Count; i++)
        {
            if (remainingLength <= 0f)
            {
                break;
            }

            DrawStroke(vh, strokes[i], ref remainingLength);
        }
    }

    private void DrawStroke(VertexHelper vh, Vector2[] stroke, ref float remainingLength)
    {
        for (int i = 0; i < stroke.Length - 1; i++)
        {
            Vector2 start = stroke[i];
            Vector2 end = stroke[i + 1];
            float segmentLength = Vector2.Distance(start, end);

            if (segmentLength <= 0f)
            {
                continue;
            }

            if (remainingLength < segmentLength)
            {
                end = Vector2.Lerp(start, end, remainingLength / segmentLength);
                AddLineQuad(vh, start, end);
                remainingLength = 0f;
                return;
            }

            AddLineQuad(vh, start, end);
            remainingLength -= segmentLength;
        }
    }

    private void AddLineQuad(VertexHelper vh, Vector2 start, Vector2 end)
    {
        Vector2 direction = end - start;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        direction.Normalize();
        Vector2 normal = new Vector2(-direction.y, direction.x) * lineWidth * 0.5f;
        int index = vh.currentVertCount;
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = start - normal;
        vh.AddVert(vertex);
        vertex.position = start + normal;
        vh.AddVert(vertex);
        vertex.position = end + normal;
        vh.AddVert(vertex);
        vertex.position = end - normal;
        vh.AddVert(vertex);

        vh.AddTriangle(index, index + 1, index + 2);
        vh.AddTriangle(index + 2, index + 3, index);
    }

    private static float CalculateStrokeLength(Vector2[] stroke)
    {
        float length = 0f;

        for (int i = 0; i < stroke.Length - 1; i++)
        {
            length += Vector2.Distance(stroke[i], stroke[i + 1]);
        }

        return length;
    }

    private static Vector2 GetPointAtDistance(Vector2[] stroke, float distance)
    {
        if (distance <= 0f)
        {
            return stroke[0];
        }

        float traversed = 0f;

        for (int i = 0; i < stroke.Length - 1; i++)
        {
            Vector2 start = stroke[i];
            Vector2 end = stroke[i + 1];
            float segmentLength = Vector2.Distance(start, end);

            if (segmentLength <= 0f)
            {
                continue;
            }

            if (distance <= traversed + segmentLength)
            {
                return Vector2.Lerp(start, end, (distance - traversed) / segmentLength);
            }

            traversed += segmentLength;
        }

        return stroke[stroke.Length - 1];
    }
}
