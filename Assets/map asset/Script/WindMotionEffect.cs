using System.Collections.Generic;
using UnityEngine;

public class WindMotionEffect : MonoBehaviour
{
    private const float BaseLineWidth = 0.026f;
    private const float TrailBackOffset = 0.66f;
    private const int WindLineCount = 5;
    private static readonly Color BaseWindColor = new Color32(116, 135, 150, 185);

    private readonly List<SketchWorldLineDrawing> windLines = new List<SketchWorldLineDrawing>();
    private readonly List<Transform> windLineTransforms = new List<Transform>();
    private int sortingOrder;

    public static WindMotionEffect Create(Transform parent, Vector3 startWorldPosition, int sortingOrder)
    {
        GameObject effectObject = new GameObject("GeneratedWindMotionEffect");

        if (parent != null)
        {
            effectObject.transform.SetParent(parent, true);
        }

        effectObject.transform.position = startWorldPosition;

        WindMotionEffect effect = effectObject.AddComponent<WindMotionEffect>();
        effect.Configure(sortingOrder);
        return effect;
    }

    public void UpdateEffect(Vector3 worldPosition, Vector3 movementDirection, float progress, float intensity = 1f)
    {
        transform.position = worldPosition;
        float clampedIntensity = Mathf.Clamp(intensity, 0.35f, 1.35f);
        float alphaIntensity = Mathf.Clamp01(intensity);

        if (movementDirection.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        for (int i = 0; i < windLines.Count; i++)
        {
            float normalizedIndex = windLines.Count <= 1 ? 0f : i / (float)(windLines.Count - 1);
            float phase = Mathf.Repeat(progress * 2.45f + i * 0.19f, 1f);
            float fade = Mathf.Sin(phase * Mathf.PI);
            float side = i - (windLines.Count - 1) * 0.5f;
            Transform lineTransform = windLineTransforms[i];

            lineTransform.localPosition = new Vector3(
                -TrailBackOffset - phase * Mathf.Lerp(0.58f, 0.95f, normalizedIndex),
                side * 0.16f + Mathf.Sin(progress * Mathf.PI * 4.8f + i * 0.9f) * 0.13f,
                0f);
            lineTransform.localScale = new Vector3(
                Mathf.Lerp(0.76f, 1.42f, phase) * clampedIntensity,
                Mathf.Lerp(0.7f, 1.06f, phase) * clampedIntensity,
                1f);
            lineTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(progress * Mathf.PI * 5.7f + i * 1.1f) * 12f);

            Color color = BaseWindColor;
            color.a *= fade * alphaIntensity * Mathf.Lerp(0.78f, 1f, normalizedIndex);
            windLines[i].Configure(BaseLineWidth, color, sortingOrder);
            windLines[i].RevealProgress = Mathf.Clamp01(fade * 1.25f);
        }
    }

    public void Finish()
    {
        Destroy(gameObject);
    }

    private void Configure(int order)
    {
        sortingOrder = order;

        for (int i = 0; i < WindLineCount; i++)
        {
            GameObject lineObject = new GameObject($"GeneratedWindLine_{i:00}");
            lineObject.transform.SetParent(transform, false);

            SketchWorldLineDrawing lineDrawing = lineObject.AddComponent<SketchWorldLineDrawing>();
            lineDrawing.Configure(BaseLineWidth, BaseWindColor, sortingOrder);
            lineDrawing.SetStrokes(BuildWindStrokes(i));
            lineDrawing.RevealProgress = 0f;

            windLines.Add(lineDrawing);
            windLineTransforms.Add(lineObject.transform);
        }
    }

    private static List<Vector3[]> BuildWindStrokes(int index)
    {
        List<Vector3[]> strokes = new List<Vector3[]>();
        float verticalOffset = (index % 2 == 0) ? 0.03f : -0.04f;

        strokes.Add(Points(
            -0.36f, verticalOffset,
            -0.08f, 0.13f + verticalOffset,
            0.22f, 0.04f + verticalOffset,
            0.48f, 0.14f + verticalOffset));
        strokes.Add(Points(
            -0.2f, -0.16f + verticalOffset,
            0.08f, -0.05f + verticalOffset,
            0.34f, -0.12f + verticalOffset));

        if (index % 2 == 1)
        {
            strokes.Add(Points(
                0.18f, 0.16f + verticalOffset,
                0.34f, 0.24f + verticalOffset,
                0.46f, 0.12f + verticalOffset,
                0.34f, 0.02f + verticalOffset));
        }

        if (index % 3 == 2)
        {
            strokes.Add(Points(
                0.02f, 0.2f + verticalOffset,
                0.18f, 0.32f + verticalOffset,
                0.34f, 0.26f + verticalOffset,
                0.38f, 0.12f + verticalOffset,
                0.24f, 0.08f + verticalOffset));
        }

        return strokes;
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
