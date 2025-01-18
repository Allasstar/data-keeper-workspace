using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class RoundedRectGraphic : Graphic
{
    [System.Serializable]
    public class CornerRadius
    {
        public float topLeft = 0f;
        public float topRight = 0f;
        public float bottomLeft = 0f;
        public float bottomRight = 0f;

        public CornerRadius(float uniform)
        {
            topLeft = topRight = bottomLeft = bottomRight = uniform;
        }

        public CornerRadius(float tl, float tr, float bl, float br)
        {
            topLeft = tl;
            topRight = tr;
            bottomLeft = bl;
            bottomRight = br;
        }
    }

    [SerializeField]
    private CornerRadius cornerRadius = new CornerRadius(0f);

    [SerializeField]
    private bool fill = true;

    [SerializeField]
    private float thickness = 2f;

    [SerializeField]
    private int cornerSegments = 8;

    public float UniformRadius
    {
        get => cornerRadius.topLeft;
        set
        {
            cornerRadius = new CornerRadius(value);
            SetVerticesDirty();
        }
    }

    private void AddCorner(VertexHelper vh, Vector3 center, float startAngle, float radius, float width, float height, bool isOuter)
    {
        float angleStep = 90f / cornerSegments;
        int startVertex = vh.currentVertCount;

        for (int i = 0; i <= cornerSegments; i++)
        {
            float angle = (startAngle + i * angleStep) * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            if (!fill)
            {
                // Inner vertex
                Vector3 innerPos = center + new Vector3(cos * (radius - thickness), sin * (radius - thickness), 0);
                vh.AddVert(innerPos, color, new Vector2(innerPos.x / width + 0.5f, innerPos.y / height + 0.5f));
            }

            // Outer vertex
            Vector3 outerPos = center + new Vector3(cos * radius, sin * radius, 0);
            vh.AddVert(outerPos, color, new Vector2(outerPos.x / width + 0.5f, outerPos.y / height + 0.5f));

            if (i > 0)
            {
                int baseIndex = startVertex + (fill ? i : i * 2);
                if (fill)
                {
                    vh.AddTriangle(
                        baseIndex - 1,
                        baseIndex,
                        vh.currentVertCount - cornerSegments - 2
                    );
                }
                else
                {
                    vh.AddTriangle(
                        baseIndex - 2,
                        baseIndex - 1,
                        baseIndex
                    );
                    vh.AddTriangle(
                        baseIndex,
                        baseIndex - 1,
                        baseIndex + 1
                    );
                }
            }
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect rect = rectTransform.rect;
        Vector3 pivot = new Vector3(
            rect.width * (rectTransform.pivot.x - 0.5f),
            rect.height * (rectTransform.pivot.y - 0.5f),
            0
        );

        float width = rect.width;
        float height = rect.height;

        // Clamp radii to prevent overlap
        float maxRadius = Mathf.Min(width / 2, height / 2);
        cornerRadius.topLeft = Mathf.Min(cornerRadius.topLeft, maxRadius);
        cornerRadius.topRight = Mathf.Min(cornerRadius.topRight, maxRadius);
        cornerRadius.bottomLeft = Mathf.Min(cornerRadius.bottomLeft, maxRadius);
        cornerRadius.bottomRight = Mathf.Min(cornerRadius.bottomRight, maxRadius);

        if (fill)
        {
            // Center vertex for filled mode
            vh.AddVert(pivot, color, new Vector2(0.5f, 0.5f));
        }

        // Top-Left corner
        if (cornerRadius.topLeft > 0)
        {
            Vector3 center = pivot + new Vector3(-width/2 + cornerRadius.topLeft, height/2 - cornerRadius.topLeft, 0);
            AddCorner(vh, center, 180f, cornerRadius.topLeft, width, height, true);
        }

        // Top-Right corner
        if (cornerRadius.topRight > 0)
        {
            Vector3 center = pivot + new Vector3(width/2 - cornerRadius.topRight, height/2 - cornerRadius.topRight, 0);
            AddCorner(vh, center, 270f, cornerRadius.topRight, width, height, true);
        }

        // Bottom-Right corner
        if (cornerRadius.bottomRight > 0)
        {
            Vector3 center = pivot + new Vector3(width/2 - cornerRadius.bottomRight, -height/2 + cornerRadius.bottomRight, 0);
            AddCorner(vh, center, 0f, cornerRadius.bottomRight, width, height, true);
        }

        // Bottom-Left corner
        if (cornerRadius.bottomLeft > 0)
        {
            Vector3 center = pivot + new Vector3(-width/2 + cornerRadius.bottomLeft, -height/2 + cornerRadius.bottomLeft, 0);
            AddCorner(vh, center, 90f, cornerRadius.bottomLeft, width, height, true);
        }

        // Add straight edges between corners
        if (!fill)
        {
            // Add vertices for straight edges
            // Top edge
            vh.AddVert(pivot + new Vector3(-width/2 + cornerRadius.topLeft, height/2, 0), color, Vector2.up);
            vh.AddVert(pivot + new Vector3(width/2 - cornerRadius.topRight, height/2, 0), color, Vector2.one);
            
            // Right edge
            vh.AddVert(pivot + new Vector3(width/2, height/2 - cornerRadius.topRight, 0), color, Vector2.right);
            vh.AddVert(pivot + new Vector3(width/2, -height/2 + cornerRadius.bottomRight, 0), color, Vector2.right);
            
            // Bottom edge
            vh.AddVert(pivot + new Vector3(width/2 - cornerRadius.bottomRight, -height/2, 0), color, Vector2.zero);
            vh.AddVert(pivot + new Vector3(-width/2 + cornerRadius.bottomLeft, -height/2, 0), color, Vector2.zero);
            
            // Left edge
            vh.AddVert(pivot + new Vector3(-width/2, -height/2 + cornerRadius.bottomLeft, 0), color, Vector2.zero);
            vh.AddVert(pivot + new Vector3(-width/2, height/2 - cornerRadius.topLeft, 0), color, Vector2.zero);
        }
    }

    public void SetCornerRadii(float topLeft, float topRight, float bottomLeft, float bottomRight)
    {
        cornerRadius = new CornerRadius(topLeft, topRight, bottomLeft, bottomRight);
        SetVerticesDirty();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        cornerSegments = Mathf.Max(4, cornerSegments);
        thickness = Mathf.Max(0, thickness);
        SetVerticesDirty();
    }
#endif
}