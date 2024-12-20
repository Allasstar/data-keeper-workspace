using System;
using DataKeeper.Attributes;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Imagine"), DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(ImageGraphic))]
public class Imagine : MonoBehaviour
{
    private static Lazy<Material> _material = new Lazy<Material>(() => Resources.Load<Material>("Imagine Material"));
    private static Lazy<Sprite> _whiteSprite = new Lazy<Sprite>(() => Resources.Load<Sprite>("4x4_white"));
    
    private ImageGraphic _imageGraphic;
    private CanvasRenderer _canvasRenderer;
    private Canvas _canvas;

    [SerializeField] private Color _color = Color.white;
    [SerializeField, Min(0)] private float _stroke = 0;
    [SerializeField, Min(0)] private float _blur = 0;
    [SerializeField] private Vector4 _cornerRadius = new Vector4(0, 0, 0, 0);
    
    private void OnValidate()
    {
        ForceUpdate();
    }
    
    private void OnDestroy()
    {
        _imageGraphic.hideFlags = HideFlags.None;
    }
    
    public void OnTransformParentChanged()
    {
        ForceUpdate();
    }

    private void GetAllComponents()
    {
        _imageGraphic ??= GetComponent<ImageGraphic>();
        _canvasRenderer ??= _imageGraphic.canvasRenderer;
        _canvas ??= GetComponentInParent<Canvas>();
    }

    [ContextMenu("Force Update"), Button("Force Update")]
    private void ForceUpdate()
    {
        GetAllComponents();
        SetAdditionalShaderChannels();
        SetMaterial();
        SetSprite();
        SetColor();
        SetVisibility();
    }

    private Vector4 GetCorners()
    {
        var rect = _imageGraphic.GetPixelAdjustedRect();

        return new Vector4(
            Mathf.Min(Mathf.Max(_cornerRadius.x, 0), Mathf.Min(rect.width, rect.height) / 2f), 
            Mathf.Min(Mathf.Max(_cornerRadius.y, 0), Mathf.Min(rect.width, rect.height) / 2f), 
            Mathf.Min(Mathf.Max(_cornerRadius.z, 0), Mathf.Min(rect.width, rect.height) / 2f),
            Mathf.Min(Mathf.Max(_cornerRadius.w, 0), Mathf.Min(rect.width, rect.height) / 2f));
    }
    
    private void SetVisibility()
    {
        _imageGraphic.hideFlags = HideFlags.NotEditable;
        _imageGraphic.material.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
    }
    
    private void SetColor()
    {
        _imageGraphic.color = _color;
    }

    private void SetMaterial()
    {
        _imageGraphic.material = _material.Value;
    }
    
    private void SetSprite()
    {
        _imageGraphic.sprite = _whiteSprite.Value;
    }
    
    protected void SetAdditionalShaderChannels()
    {
        _canvas.additionalShaderChannels |= 
            AdditionalCanvasShaderChannels.TexCoord1 
            | AdditionalCanvasShaderChannels.TexCoord2
            | AdditionalCanvasShaderChannels.TexCoord3;
    }
    
    public void SetVertexData(VertexHelper fill)
    {
        UIVertex vert = UIVertex.simpleVert;
        
        var rect = _imageGraphic.GetPixelAdjustedRect();
        var uv1 = GetCorners();
        var uv2 = new Vector4(rect.width, rect.height, _stroke == 0 ? Mathf.Max(rect.width, rect.height) : _stroke, 1f / Mathf.Max(_blur, 0.5f));
        
        for (int i = 0; i < fill.currentVertCount; i++)
        {
            fill.PopulateUIVertex(ref vert, i);

            // vert.position += ((Vector3)vert.uv0 - new Vector3(0.5f, 0.5f)) * _scale;
            vert.uv1 = uv1;
            vert.uv2 = uv2;
            fill.SetUIVertex(vert, i);
        }
    }
}


