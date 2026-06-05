using UnityEngine;

[ExecuteAlways]
public class LineRendererStyleApplier : MonoBehaviour
{
    [SerializeField] private Material lineMaterial;
    [SerializeField] private Color lineColor = new Color32(35, 32, 28, 255);

    private void OnEnable()
    {
        Apply();
    }

    private void OnValidate()
    {
        Apply();
    }

    public void Apply()
    {
        LineRenderer[] lineRenderers = GetComponentsInChildren<LineRenderer>(true);
        for (int i = 0; i < lineRenderers.Length; i++)
        {
            LineRenderer lineRenderer = lineRenderers[i];
            if (lineRenderer == null)
            {
                continue;
            }

            if (lineMaterial != null)
            {
                lineRenderer.sharedMaterial = lineMaterial;
            }

            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
        }
    }
}
