using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkinInShop : MonoBehaviour
{
    [SerializeField] Image skinImage;
    [SerializeField] Text skinName, txtPrice, txtEquip;
    [SerializeField] ButtonEvent btnBuy, btnEquip;

    public void SetSkinImage(Sprite Skinsprite)
    {
        skinImage.sprite = Skinsprite;
    }

    public void SetTextPrice(int skinPrice)
    {
        txtPrice.text = skinPrice.ToString();
    }

    public void SetSkinName(string s_name)
    {
        skinName.text = s_name;
    }

    public void SetCharacterAsPurchased()
    {
        btnBuy.gameObject.SetActive(false);
        btnEquip.gameObject.SetActive(true);
        txtEquip.text = "Equip";
    }

    public void OnItemPurchase(int itemIndex, UnityAction<int> action)
    {
        btnBuy.onClick.RemoveAllListeners();
        btnBuy.onClick.AddListener(() => action.Invoke(itemIndex));
    }

    public void OnItemSelect(int itemIndex, UnityAction<int> action)
    {
        btnEquip.gameObject.SetActive(true);
        btnEquip.onClick.RemoveAllListeners();
        btnEquip.onClick.AddListener(() => action.Invoke(itemIndex));
    }

    public void SelectedItem()
    {
        btnEquip.interactable = false;
        txtEquip.text = "Equipped";
    }

    public void DeSelectdItem()
    {
        btnEquip.interactable = true;
        txtEquip.text = "Equip";
    }
}
