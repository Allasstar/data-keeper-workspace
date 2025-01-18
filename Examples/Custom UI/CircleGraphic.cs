using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class CircleGraphic : Graphic
{
    [SerializeField]
    private int segments = 36;

    [SerializeField, Range(0, 360)]
    private float fillPercent = 360f;

    [SerializeField]
    private float thickness = 10f;

    [SerializeField]
    private bool fill = true;

    [SerializeField]
    private float rotation = 0f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        // Get size from RectTransform
        Rect rect = rectTransform.rect;
        float width = rect.width;
        float height = rect.height;
        
        // Use the smaller dimension to maintain circle proportions
        float radius = Mathf.Min(width, height) / 2;
        float outerRadius = radius;
        float innerRadius = fill ? 0 : outerRadius - thickness;

        // Convert rotation and fill percent to radians
        float startAngle = rotation * Mathf.Deg2Rad;
        float arcLength = fillPercent * Mathf.Deg2Rad;
        float stepAngle = arcLength / segments;

        // Convert pivot offset to Vector3
        Vector3 pivot = new Vector3(
            rect.width * (rectTransform.pivot.x - 0.5f),
            rect.height * (rectTransform.pivot.y - 0.5f),
            0f
        );

        // Scale factors for elliptical shape
        float scaleX = width / height;
        float scaleY = 1f;

        if (width > height)
        {
            scaleX = 1f;
            scaleY = height / width;
        }

        // Center vertex if filled
        if (fill)
        {
            vh.AddVert(pivot, color, new Vector2(0.5f, 0.5f));
        }

        // Generate vertices
        int vertexCount = 0;
        for (int i = 0; i <= segments; i++)
        {
            float angle = startAngle + stepAngle * i;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            // Apply scaling for elliptical shape
            Vector3 direction = new Vector3(cos * scaleX, sin * scaleY, 0);

            if (!fill)
            {
                // Inner vertex
                Vector3 innerPos = pivot + (direction * innerRadius);
                vh.AddVert(innerPos, color, new Vector2((cos + 1) / 2, (sin + 1) / 2));
            }

            // Outer vertex
            Vector3 outerPos = pivot + (direction * outerRadius);
            vh.AddVert(outerPos, color, new Vector2((cos + 1) / 2, (sin + 1) / 2));

            if (i > 0)
            {
                if (fill)
                {
                    // Create triangles for filled circle
                    vh.AddTriangle(
                        0,
                        vertexCount,
                        vertexCount + 1
                    );
                }
                else
                {
                    // Create triangles for hollow circle
                    int baseIndex = (i - 1) * 2;
                    vh.AddTriangle(
                        baseIndex,
                        baseIndex + 1,
                        baseIndex + 2
                    );
                    vh.AddTriangle(
                        baseIndex + 1,
                        baseIndex + 3,
                        baseIndex + 2
                    );
                }
            }

            vertexCount = fill ? i + 1 : (i + 1) * 2;
        }
    }

    public void SetRotation(float newRotation)
    {
        rotation = newRotation;
        SetVerticesDirty();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        SetVerticesDirty();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        segments = Mathf.Max(3, segments);
        thickness = Mathf.Clamp(thickness, 0, rectTransform.rect.width / 2);
        SetVerticesDirty();
    }
#endif
}