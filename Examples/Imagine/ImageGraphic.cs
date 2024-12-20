using UnityEngine;
using UnityEngine.UI;

public class ImageGraphic : Image
{
    private Imagine _imagine;
    private Imagine Imagine => _imagine ?? GetComponent<Imagine>();

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        base.OnPopulateMesh(toFill);
        Imagine.SetVertexData(toFill);
    }
    
    protected override void OnTransformParentChanged()
    {
        base.OnTransformParentChanged();
        Imagine.OnTransformParentChanged();
    }
    
#if UNITY_EDITOR
    public void Update()
    {
        if (!Application.isPlaying && UnityEditor.Selection.activeTransform == transform)
        {
            this.UpdateGeometry();
        }
    }
#endif
}
