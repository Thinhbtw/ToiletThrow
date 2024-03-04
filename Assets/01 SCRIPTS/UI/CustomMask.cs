using UnityEngine.UI;
using UnityEngine;

public class CustomMask : MaskableGraphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector3 vec_00 = new Vector3(0, 0);
        Vector3 vec_01 = new Vector3(0, 50);
        Vector3 vec_10 = new Vector3(50, 0);
        Vector3 vec_11 = new Vector3(50, 50);

        vh.AddUIVertexQuad(new UIVertex[]
        {
            new UIVertex { position = vec_00},
            new UIVertex { position = vec_01},
            new UIVertex { position = vec_10},
            new UIVertex { position = vec_11},
        });
    }
}
