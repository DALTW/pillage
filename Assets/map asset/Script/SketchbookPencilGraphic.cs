using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class SketchbookPencilGraphic : MaskableGraphic
{
    [SerializeField] private Color bodyColor = new Color32(224, 183, 66, 255);
    [SerializeField] private Color bodyEdgeColor = new Color32(174, 126, 36, 255);
    [SerializeField] private Color woodColor = new Color32(205, 152, 95, 255);
    [SerializeField] private Color graphiteColor = new Color32(44, 42, 38, 255);
    [SerializeField] private Color bandColor = new Color32(156, 162, 166, 255);
    [SerializeField] private Color eraserColor = new Color32(220, 116, 126, 255);

    protected override void Awake()
    {
        base.Awake();
        raycastTarget = false;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect rect = rectTransform.rect;
        float xMin = rect.xMin;
        float xMax = rect.xMax;
        float yBodyMin = -rect.height * 0.28f;
        float yBodyMax = rect.height * 0.28f;
        float eraserEnd = Mathf.Lerp(xMin, xMax, 0.12f);
        float bandEnd = Mathf.Lerp(xMin, xMax, 0.18f);
        float tipStart = Mathf.Lerp(xMin, xMax, 0.82f);
        float graphiteStart = Mathf.Lerp(xMin, xMax, 0.94f);

        AddQuad(vh, xMin, yBodyMin, eraserEnd, yBodyMax, eraserColor);
        AddQuad(vh, eraserEnd, yBodyMin, bandEnd, yBodyMax, bandColor);
        AddQuad(vh, bandEnd, yBodyMin, tipStart, yBodyMax, bodyColor);
        AddQuad(vh, bandEnd, yBodyMax - 3f, tipStart, yBodyMax, bodyEdgeColor);
        AddQuad(vh, bandEnd, yBodyMin, tipStart, yBodyMin + 3f, bodyEdgeColor);
        AddTriangle(vh, new Vector2(tipStart, yBodyMin), new Vector2(tipStart, yBodyMax), new Vector2(xMax, 0f), woodColor);
        AddTriangle(vh, new Vector2(graphiteStart, -rect.height * 0.12f), new Vector2(graphiteStart, rect.height * 0.12f), new Vector2(xMax, 0f), graphiteColor);
    }

    private void AddQuad(VertexHelper vh, float xMin, float yMin, float xMax, float yMax, Color quadColor)
    {
        int index = vh.currentVertCount;
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = ApplyTint(quadColor);

        vertex.position = new Vector2(xMin, yMin);
        vh.AddVert(vertex);
        vertex.position = new Vector2(xMin, yMax);
        vh.AddVert(vertex);
        vertex.position = new Vector2(xMax, yMax);
        vh.AddVert(vertex);
        vertex.position = new Vector2(xMax, yMin);
        vh.AddVert(vertex);

        vh.AddTriangle(index, index + 1, index + 2);
        vh.AddTriangle(index + 2, index + 3, index);
    }

    private void AddTriangle(VertexHelper vh, Vector2 a, Vector2 b, Vector2 c, Color triangleColor)
    {
        int index = vh.currentVertCount;
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = ApplyTint(triangleColor);

        vertex.position = a;
        vh.AddVert(vertex);
        vertex.position = b;
        vh.AddVert(vertex);
        vertex.position = c;
        vh.AddVert(vertex);

        vh.AddTriangle(index, index + 1, index + 2);
    }

    private Color ApplyTint(Color source)
    {
        return new Color(
            source.r * color.r,
            source.g * color.g,
            source.b * color.b,
            source.a * color.a);
    }
}
