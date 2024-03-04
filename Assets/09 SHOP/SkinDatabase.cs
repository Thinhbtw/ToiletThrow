using UnityEngine;

[CreateAssetMenu(fileName ="SkinDatabase",menuName ="Shop/Skin")]
public class SkinDatabase : ScriptableObject
{
    public Skin[] skins;

    public int SkinCount
    {
        get { return skins.Length; }
    }

    public Skin GetSkin(int index)
    {
        return skins[index];
    }

    public void PurchaseSkin(int index)
    {
        skins[index].hasPurchased = true;
    }
}
