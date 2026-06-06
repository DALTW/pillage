using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
[RequireComponent(typeof(SplineContainer))]
[RequireComponent(typeof(LineRenderer))]
public class SplineLineRenderer : MonoBehaviour
{
    [Min(2)]
    [SerializeField] private int sampleCount = 64;
    [SerializeField] private float lineWidth = 0.06f;
    [SerializeField] private Color lineColor = new Color32(35, 32, 28, 255);
    [SerializeField] private Material lineMaterial;
    [SerializeField] private string sortingLayerName = "Default";
    [SerializeField] private int sortingOrder = 8;

    private SplineContainer splineContainer;
    private LineRenderer lineRenderer;

    private void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
        RefreshLine();
    }

    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    private void OnValidate()
    {
        sampleCount = Mathf.Max(2, sampleCount);
        lineWidth = Mathf.Max(0.01f, lineWidth);
        RefreshLine();
    }

    [ContextMenu("Refresh Line")]
    public void RefreshLine()
    {
        EnsureComponents();
        ConfigureLineRenderer();

        if (splineContainer == null || splineContainer.Splines.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        Spline spline = splineContainer.Splines[0];
        if (spline == null || spline.Count < 2)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        int pointCount = Mathf.Max(2, sampleCount);
        lineRenderer.loop = spline.Closed;
        lineRenderer.positionCount = pointCount;

        float divisor = spline.Closed ? pointCount : pointCount - 1f;
        for (int i = 0; i < pointCount; i++)
        {
            float t = i / divisor;
            lineRenderer.SetPosition(i, ToVector3(SplineUtility.EvaluatePosition(spline, t)));
        }
    }

    private void EnsureComponents()
    {
        if (splineContainer == null)
        {
            splineContainer = GetComponent<SplineContainer>();
        }

        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }

    private void ConfigureLineRenderer()
    {
        if (lineRenderer == null)
        {
            return;
        }

        lineRenderer.useWorldSpace = false;
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.numCapVertices = 8;
        lineRenderer.numCornerVertices = 8;
        lineRenderer.sortingLayerName = sortingLayerName;
        lineRenderer.sortingOrder = sortingOrder;

        if (lineMaterial != null)
        {
            lineRenderer.sharedMaterial = lineMaterial;
        }
    }

    private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modification)
    {
        if (splineContainer == null || splineContainer.Splines.Count == 0)
        {
            return;
        }

        if (splineContainer.Splines[0] == spline)
        {
            RefreshLine();
        }
    }

    private static Vector3 ToVector3(float3 value)
    {
        return new Vector3(value.x, value.y, value.z);
    }
}
