using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class HandwrittenTextGraphic : MaskableGraphic
{
    [SerializeField] private string text = "PILLAGE";
    [SerializeField] private float reveal;
    [SerializeField] private float strokeWidth = 14f;
    [SerializeField] private float letterSpacing = 0.16f;
    [SerializeField] private float wordSpacing = 0.5f;

    private readonly List<StrokeSegment> segments = new List<StrokeSegment>();

    public Vector2 CursorPosition { get; private set; }

    public string Text
    {
        get => text;
        set
        {
            text = value;
            SetVerticesDirty();
        }
    }

    public float Reveal
    {
        get => reveal;
        set
        {
            reveal = Mathf.Clamp01(value);
            SetVerticesDirty();
        }
    }

    public float StrokeWidth
    {
        get => strokeWidth;
        set
        {
            strokeWidth = Mathf.Max(1f, value);
            SetVerticesDirty();
        }
    }

    public float LetterSpacing
    {
        get => letterSpacing;
        set
        {
            letterSpacing = Mathf.Max(0f, value);
            SetVerticesDirty();
        }
    }

    protected override void OnPopulateMesh(VertexHelper vertexHelper)
    {
        vertexHelper.Clear();
        segments.Clear();

        if (string.IsNullOrEmpty(text))
        {
            CursorPosition = Vector2.zero;
            return;
        }

        BuildSegments();

        if (segments.Count == 0)
        {
            CursorPosition = Vector2.zero;
            return;
        }

        float totalLength = 0f;
        for (int i = 0; i < segments.Count; i++)
        {
            totalLength += segments[i].Length;
        }

        float drawLength = totalLength * reveal;
        float consumed = 0f;
        CursorPosition = segments[0].Start;

        for (int i = 0; i < segments.Count; i++)
        {
            StrokeSegment segment = segments[i];

            if (drawLength <= consumed)
            {
                break;
            }

            float segmentProgress = Mathf.Clamp01((drawLength - consumed) / segment.Length);
            Vector2 end = Vector2.Lerp(segment.Start, segment.End, segmentProgress);
            AddRoughLine(vertexHelper, segment.Start, end, strokeWidth);
            CursorPosition = end;
            consumed += segment.Length;
        }
    }

    private void BuildSegments()
    {
        Rect rect = rectTransform.rect;
        float targetHeight = rect.height * 0.78f;
        float totalUnits = GetTextUnits(text);

        if (totalUnits <= 0f)
        {
            return;
        }

        float scale = Mathf.Min(targetHeight, rect.width / totalUnits);
        float x = -totalUnits * scale * 0.5f;
        float y = -scale * 0.5f;

        for (int i = 0; i < text.Length; i++)
        {
            char character = char.ToUpperInvariant(text[i]);
            float widthUnits = GetLetterWidth(character);

            if (character == ' ')
            {
                x += widthUnits * scale;
                continue;
            }

            AddLetterSegments(character, new Vector2(x, y), scale, widthUnits);
            x += (widthUnits + letterSpacing) * scale;
        }
    }

    private float GetTextUnits(string value)
    {
        float units = 0f;

        for (int i = 0; i < value.Length; i++)
        {
            char character = char.ToUpperInvariant(value[i]);
            units += GetLetterWidth(character);

            if (i < value.Length - 1 && character != ' ')
            {
                units += letterSpacing;
            }
        }

        return units;
    }

    private float GetLetterWidth(char character)
    {
        switch (character)
        {
            case 'I':
                return 0.42f;
            case 'L':
            case 'T':
                return 0.68f;
            case 'M':
                return 1.05f;
            case ' ':
                return wordSpacing;
            default:
                return 0.86f;
        }
    }

    private void AddLetterSegments(char character, Vector2 origin, float scale, float widthUnits)
    {
        switch (character)
        {
            case 'P':
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.08f, 0f), new Vector2(0.08f, 1f), new Vector2(0.78f, 1f), new Vector2(0.84f, 0.58f), new Vector2(0.08f, 0.55f) });
                break;
            case 'I':
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.12f, 1f), new Vector2(0.88f, 1f) });
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.5f, 1f), new Vector2(0.5f, 0f) });
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.12f, 0f), new Vector2(0.88f, 0f) });
                break;
            case 'L':
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.15f, 1f), new Vector2(0.13f, 0.02f), new Vector2(0.9f, 0.03f) });
                break;
            case 'A':
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.02f, 0f), new Vector2(0.5f, 1f), new Vector2(0.98f, 0f) });
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.27f, 0.42f), new Vector2(0.74f, 0.42f) });
                break;
            case 'G':
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.82f, 0.82f), new Vector2(0.58f, 1f), new Vector2(0.19f, 0.88f), new Vector2(0.05f, 0.52f), new Vector2(0.16f, 0.16f), new Vector2(0.5f, 0.02f), new Vector2(0.88f, 0.18f), new Vector2(0.88f, 0.46f), new Vector2(0.58f, 0.46f) });
                break;
            case 'E':
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.86f, 1f), new Vector2(0.12f, 1f), new Vector2(0.12f, 0f), new Vector2(0.88f, 0f) });
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.15f, 0.52f), new Vector2(0.72f, 0.52f) });
                break;
            case 'M':
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.05f, 0f), new Vector2(0.05f, 1f), new Vector2(0.5f, 0.36f), new Vector2(0.95f, 1f), new Vector2(0.95f, 0f) });
                break;
            case 'S':
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.82f, 0.84f), new Vector2(0.58f, 1f), new Vector2(0.18f, 0.86f), new Vector2(0.17f, 0.58f), new Vector2(0.72f, 0.43f), new Vector2(0.86f, 0.16f), new Vector2(0.55f, 0.01f), new Vector2(0.1f, 0.14f) });
                break;
            case 'T':
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.05f, 1f), new Vector2(0.95f, 1f) });
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.5f, 1f), new Vector2(0.5f, 0f) });
                break;
            case 'R':
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.08f, 0f), new Vector2(0.08f, 1f), new Vector2(0.76f, 1f), new Vector2(0.84f, 0.6f), new Vector2(0.08f, 0.55f) });
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.38f, 0.52f), new Vector2(0.9f, 0f) });
                break;
            default:
                AddPolyline(origin, scale, widthUnits, new[] { new Vector2(0.08f, 0f), new Vector2(0.08f, 1f), new Vector2(0.88f, 1f), new Vector2(0.88f, 0f), new Vector2(0.08f, 0f) });
                break;
        }
    }

    private void AddPolyline(Vector2 origin, float scale, float widthUnits, Vector2[] points)
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 start = ToLocal(origin, scale, widthUnits, points[i]);
            Vector2 end = ToLocal(origin, scale, widthUnits, points[i + 1]);
            float length = Vector2.Distance(start, end);

            if (length > 0.01f)
            {
                segments.Add(new StrokeSegment(start, end, length));
            }
        }
    }

    private Vector2 ToLocal(Vector2 origin, float scale, float widthUnits, Vector2 point)
    {
        return origin + new Vector2(point.x * widthUnits * scale, point.y * scale);
    }

    private void AddRoughLine(VertexHelper vertexHelper, Vector2 start, Vector2 end, float width)
    {
        if ((end - start).sqrMagnitude <= 0.01f)
        {
            return;
        }

        AddLine(vertexHelper, start, end, width, color);
        Color secondPass = color;
        secondPass.a *= 0.3f;
        Vector2 offset = new Vector2(Mathf.Sin(start.x * 0.017f) * 1.2f, Mathf.Cos(end.y * 0.019f) * 1.2f);
        AddLine(vertexHelper, start + offset, end + offset, width * 0.46f, secondPass);
    }

    private void AddLine(VertexHelper vertexHelper, Vector2 start, Vector2 end, float width, Color lineColor)
    {
        Vector2 direction = (end - start).normalized;
        Vector2 normal = new Vector2(-direction.y, direction.x) * (width * 0.5f);

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = lineColor;

        vertex.position = start - normal;
        UIVertex v0 = vertex;
        vertex.position = start + normal;
        UIVertex v1 = vertex;
        vertex.position = end + normal;
        UIVertex v2 = vertex;
        vertex.position = end - normal;
        UIVertex v3 = vertex;

        vertexHelper.AddUIVertexQuad(new[] { v0, v1, v2, v3 });
    }

    private struct StrokeSegment
    {
        public StrokeSegment(Vector2 start, Vector2 end, float length)
        {
            Start = start;
            End = end;
            Length = length;
        }

        public Vector2 Start { get; }
        public Vector2 End { get; }
        public float Length { get; }
    }
}
