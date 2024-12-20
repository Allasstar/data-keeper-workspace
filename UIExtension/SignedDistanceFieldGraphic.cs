using System;
using UnityEngine;
using UnityEngine.UI;

public class SignedDistanceFieldGraphic : MaskableGraphic
{
    [Header("Graphic")]

    [SerializeField] Texture m_texture;

    [SerializeField, Range(0f, 1f)] float m_graphicAlphaMult = 1f;

    [Header("Outline")]

    [SerializeField, Min(0f)] float m_outlineSize = 0f;

    [SerializeField] Color32 m_outlineColor = Color.black;

    [Header("Shadow")]

    [SerializeField, Min(0f)] float m_shadowSize = 0f;

    [SerializeField, Min(0f)] float m_shadowBlur = 0f;

    [SerializeField] Color32 m_shadowColor = Color.black;
    

    Material m_material;
    

    public Texture Texture {
        get => m_texture;
        set {
            m_texture = value; 
            SetMaterialDirty();
        }
    }
    
    public virtual float Margin => Mathf.Max(m_outlineSize, m_shadowSize) + 2 + ExtraMargin;

    public virtual float ExtraMargin => 0;

    protected event Action<float, float> onMaterialUpdate;

    public override Texture mainTexture => m_texture;

    public float Alpha {get => m_graphicAlphaMult; set
    {
        m_graphicAlphaMult = value;
        SetAllDirty();

        if (value == 0) canvasRenderer.cull = true;
        else            canvasRenderer.cull = false;
    }}

    static Material RectangleRendererShader;

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateInstanciable();
    }

    private void LoadMaterial()
    {
        if (RectangleRendererShader == null)
        {
            RectangleRendererShader = new Material(Shader.Find("UI/Windinator/RectangleRenderer"));
            RectangleRendererShader.enableInstancing = true;
        }
        m_material = RectangleRendererShader;
    }

    public override Material defaultMaterial
    {
        get
        {
            if (m_material == null)
            {
                LoadMaterial();
            }
            return m_material;
        }
    }
    
    public Color OutlineColor
    {
        get => m_outlineColor;
        set
        {
            m_outlineColor = value;
            SetVerticesDirty();
        }
    }

    public float OutlineSize
    {
        get => m_outlineSize;
        set
        {
            m_outlineSize = Mathf.Max(0, value);
            SetVerticesDirty();
        }
    }

    public Color ShadowColor
    {
        get => m_shadowColor;
        set
        {
            m_shadowColor = value;
            SetVerticesDirty();
        }
    }

    public float ShadowSize
    {
        get => m_shadowSize;
        set
        {
            m_shadowSize = Mathf.Max(0, value);
            SetVerticesDirty();
        }
    }

    public float ShadowBlur
    {
        get => m_shadowBlur;
        set
        {
            m_shadowBlur = value;
            SetVerticesDirty();
        }
    }

    public void SetOutline(Color color, float size)
    {
        m_outlineSize = size;
        m_outlineColor = color;

        SetAllDirty();
    }

    public void SetShadow(Color color, float size, float blur)
    {
        m_shadowColor = color;
        m_shadowSize = size;
        m_shadowBlur = blur;

        SetAllDirty();
    }

    private Vector2 m_sizeCached;

    private void UpdateShaderDimensions()
    {
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        m_sizeCached.x = width;
        m_sizeCached.y = height;

        onMaterialUpdate?.Invoke(width, height);
    }

    public override void SetLayoutDirty()
    {
        base.SetLayoutDirty();
        UpdateShaderDimensions();
    }

    public override void SetVerticesDirty()
    {
        base.SetVerticesDirty();

        #if UNITY_EDITOR
        if (canvas != null && !Application.isPlaying)
        {
            canvas.additionalShaderChannels = 
                AdditionalCanvasShaderChannels.Normal | 
                AdditionalCanvasShaderChannels.Tangent | 
                AdditionalCanvasShaderChannels.TexCoord1 | 
                AdditionalCanvasShaderChannels.TexCoord2 | 
                AdditionalCanvasShaderChannels.TexCoord3;
        }
        #endif
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        UpdateShaderDimensions();
    }

    protected virtual Vector4 UpdateMeshData(float width, float height)
    {
        return default;
    }

    static readonly Vector4 ZERO = new Vector2(0, 0), UP = new Vector4(0, 1), ONE = new Vector4(1, 1), RIGHT = new Vector4(1, 0);

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 size = rectTransform.rect.size;

        float width = size.x;
        float height = size.y;

        Vector3 pivot = new Vector3(
            rectTransform.pivot.x * width,
            rectTransform.pivot.y * height, 0);

        var uv1 = new Vector4(
            m_shadowSize,
            m_shadowBlur,
            0,
            m_outlineSize
        );

        var uv2 = new Vector4(
            m_sizeCached.x,
            m_sizeCached.y,
            0,
            DecodeFloatRGBA(Color.white)
        );

        var uv3 = new Vector4(
            DecodeFloatRGBA(m_outlineColor),
            DecodeFloatRGBA(m_shadowColor),
            0,
            DecodeFloatRGBA(0, m_graphicAlphaMult, 0, 0)
        );

        var normal = new Vector3(
            DecodeFloatRGBA(0, (0 + 1) * 0.5f, (0 + 1) * 0.5f, 1),
            0, // Don't touch this
            0 // Don't touch this
        );


        float marginedWidth = width + Margin - pivot.x;
        float marginedHeight = height + Margin - pivot.y;
        float negativeMarginX = -Margin - pivot.x;
        float negativeMarginY = -Margin - pivot.y;

        var tangent = UpdateMeshData(width, height);
        Vector2 extraData = new Vector2(DecodeFloatRGBA(Color.white), Margin);

        Vector4 extraDataV4 = new Vector4(0, 0, extraData.x, extraData.y);

        vh.AddVert(
            new Vector3(negativeMarginX, negativeMarginY),
            color,
            ZERO + extraDataV4,
            uv1, uv2, uv3, normal, tangent);

        vh.AddVert(
            new Vector3(negativeMarginX, marginedHeight),
            color,
            UP + extraDataV4,
            uv1, uv2, uv3, normal, tangent);

        vh.AddVert(
            new Vector3(marginedWidth, marginedHeight),
            color,
            ONE + extraDataV4,
            uv1, uv2, uv3, normal, tangent);

        vh.AddVert(
            new Vector3(marginedWidth, negativeMarginY),
            color,
            RIGHT + extraDataV4,
            uv1, uv2, uv3, normal, tangent);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    const int LOSE = 20;
    const int WIN = (255 - (LOSE * 2));
    const int WIN_2 = (127 - (LOSE * 2));
    const int WIN_SHORT = (65535 - (LOSE * 2));

    byte RemapColor(byte chanel)
    {
        float f = chanel / 255f;
        return (byte)((f * WIN) + LOSE);
    }

    byte RemapColor2(byte chanel)
    {
        float f = chanel / 255f;
        return (byte)((f * WIN_2) + LOSE);
    }

    byte RemapColor(float f)
    {
        return (byte)((f * WIN) + LOSE);
    }

    ushort RemapColorShort(float f)
    {
        return (ushort)((f * WIN_SHORT) + LOSE);
    }

    byte RemapColor2(float f)
    {
        return (byte)((f * WIN_2) + LOSE);
    }

    float DecodeSizeFloat(float value, float maxLength)
    {
        return ((value / maxLength) + 1) * 0.5f;
    }

    /// <summary>
    /// Max value = 16383 
    /// Second axis (y) will lose precision if x is big
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    float DecodeUnsignedVector2(Vector2 value)
    {
        return DecodeUnsignedVector2(value.x, value.y);
    }

    struct DecodePair
    {
        public uint Value;
        public uint MaxValue;

        public DecodePair(uint v, uint max)
        {
            Value = v;
            MaxValue = max;
        }
    }

    /// <summary>
    /// Bigger "MaxValues" should ALWAYS go first in order.
    /// </summary>
    float DecodeValues(DecodePair x, DecodePair y, uint z)
    {
        return z * x.MaxValue * y.MaxValue + y.Value * x.MaxValue + x.Value;
    }

    float DecodeValues(DecodePair x, uint y)
    {
        return y * x.MaxValue + x.Value;
    }

    float DecodeUnsignedVector2(float _x, float _y)
    {
        float x = Mathf.Round(Mathf.Max(0, Mathf.Min(_x * 4, 16383)));
        float y = Mathf.Round(Mathf.Max(0, Mathf.Min(_y * 4, 16383)));

        float compiled = x * 16384 + y;

        return compiled;
    }

    float DecodeFloatRGBA(float r, float g, float b, float a)
    {
        int result = RemapColor(r);
        result |= RemapColor(g) << 8;
        result |= RemapColor(b) << 16;
        result |= RemapColor2(a) << 24;

        return Int32BitsToSingle(result);
    }

    float DecodeFloatRGBA(Color color)
    {
        int result = RemapColor(color.r);
        result |= RemapColor(color.g) << 8;
        result |= RemapColor(color.b) << 16;
        result |= RemapColor2(color.a) << 24;

        return Int32BitsToSingle(result);
    }

    float DecodeFloatRGBA(Color32 color)
    {
        int result = RemapColor(color.r);
        result |= RemapColor(color.g) << 8;
        result |= RemapColor(color.b) << 16;
        result |= RemapColor2(color.a) << 24;

        return Int32BitsToSingle(result);
    }
    
    public static float Int32BitsToSingle(int value) {
        return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
    }


    // public static unsafe float Int32BitsToSingle(int value) {
    //     return *(float*)(&value);
    // }

    // public static unsafe float UInt32BitsToSingle(uint value)
    // {
    //     return *(float*)(&value);
    // }
    //
    // public static unsafe uint SingleBitsToUInt32(float value)
    // {
    //     return *(uint*)(&value);
    // }

    public override void SetMaterialDirty()
    {
        if (defaultMaterial == null) LoadMaterial();

        base.SetMaterialDirty();

        UpdateInstanciable();
        UpdateShaderDimensions();

        defaultMaterial.SetTexture("_MainTex", mainTexture);
    }

    void UpdateInstanciable()
    {
        if (defaultMaterial == null) LoadMaterial();

        bool canInstance = mainTexture == null;
        bool isInstance = defaultMaterial == RectangleRendererShader;

        if (canInstance != isInstance)
        {
            m_material = canInstance ? RectangleRendererShader : Instantiate(RectangleRendererShader);
            SetAllDirty();
        }
    }
}
