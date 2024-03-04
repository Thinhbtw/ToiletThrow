using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] List<GameObject> listItem1 = new List<GameObject>();
    [SerializeField] List<GameObject> listItem2 = new List<GameObject>();
    [SerializeField] GameObject tab1, tab2;

    [SerializeField] List<Text> coins_text;
    [SerializeField] Transform skinPanel;
    [SerializeField] GameObject itemPrefabs;
    [SerializeField] SkinDatabase skinDatabase;
    [SerializeField] GameManager gameManager;
    [SerializeField] FirebaseManager firebaseManager;

    [SerializeField] GameObject totalCoin, gameObjectNotEnoughCoin;
    [SerializeField] Text txtNotEnoughCoin;
    Vector3 def_pos_TotalCoin;

    int newSelectedItemIndex = 1;
    int prevSelectedItemIndex = 1;

    private void Start()
    {
        def_pos_TotalCoin = totalCoin.transform.localPosition;
    }

    public void GenerateSkinItem()
    {
        for(int i = 0; i<skinDatabase.SkinCount; i++)
        {
            Skin skin = skinDatabase.GetSkin(i);
            SkinInShop skinInShop = Instantiate(itemPrefabs, skinPanel).GetComponent<SkinInShop>();
            listItem1.Add(skinInShop.gameObject);

            skinInShop.gameObject.name = "Item " + i;

            skinInShop.SetSkinImage(skin.skinImage);
            skinInShop.SetTextPrice(skin.price);
            skinInShop.SetSkinName(skin.name);

            if(DATA.GetAllSkinPurchased().Contains(i))
            {
                skinInShop.SetCharacterAsPurchased();
                skinInShop.OnItemSelect(i, OnItemSelected);
            }
            else if (skin.hasPurchased)
            {
                skinInShop.SetCharacterAsPurchased();
                skinInShop.OnItemSelect(i, OnItemSelected);
            }
            else
            {
                skinInShop.SetTextPrice(skin.price);
                skinInShop.OnItemPurchase(i, OnItemPurchased);
            }
        }
    }
    public void SetSelectedSkinWhenOpenGame()
    {
        //Get saved index
        int index = DATA.GetSelectedSkinIndex();

        //Set selected character
        DATA.SetSelectedSkin(skinDatabase.GetSkin(index), index);

        SelectItemUI(DATA.GetSelectedSkinIndex());
    }

    void OnItemSelected(int index)
    {
        SelectItemUI(index);
        DATA.SetSelectedSkin(skinDatabase.GetSkin(index), index);

        SoundManager.Instance.PlaySound(SoundManager.SoundType.SkinEquip);
        
    }

    void SelectItemUI(int itemIndex)
    {
        itemIndex = itemIndex + 1;
        prevSelectedItemIndex = newSelectedItemIndex;
        newSelectedItemIndex = itemIndex;

        SkinInShop prevItem = GetSkinInSkinPanel(prevSelectedItemIndex);
        SkinInShop newItem = GetSkinInSkinPanel(newSelectedItemIndex);

        prevItem.DeSelectdItem();
        newItem.SelectedItem();
    }

    void OnItemPurchased(int index)
    {

        Skin skin = skinDatabase.GetSkin(index);
        SkinInShop uiItem = GetSkinInSkinPanel(index + 1);

        if (DATA.CheckIfEnoughCoin(skin.price))
        {
            //Proceed with the purchase operation
            DATA.SpendCoin(skin.price);

            //Update Coins UI text
            UpdateCoinText();

            //Update DB's Data
            skinDatabase.PurchaseSkin(index);

            uiItem.SetCharacterAsPurchased();
            uiItem.OnItemSelect(index, OnItemSelected);
            OnItemSelected(index);

            //Add purchased item to Shop Data
            DATA.AddPurchasedSkin(index);

            firebaseManager.BuySkin(index);

            SoundManager.Instance.PlaySound(SoundManager.SoundType.ShopPurchasedSuccess);

        }
        else
        {
            if (DATA.GetVibrationState())
            {
                Handheld.Vibrate();
            }
            SoundManager.Instance.PlaySound(SoundManager.SoundType.ShopPurchasedFailed);
            totalCoin.transform.DOShakePosition(0.2f, 5f, 5, 90, true, true);
            gameObjectNotEnoughCoin.transform.DOLocalMoveY(-57f, 0f);
            txtNotEnoughCoin.DOFade(1, 0f);
            gameObjectNotEnoughCoin.transform.DOLocalMoveY(200f, 2f);
            txtNotEnoughCoin.DOFade(0, 2f);
        }
    }

    SkinInShop GetSkinInSkinPanel(int index)
    {
        return skinPanel.GetChild(index).GetComponent<SkinInShop>();
    }


    private void OnEnable()
    {
        OpenShop();
    }

    private void OnDisable()
    {
        totalCoin.transform.localPosition = def_pos_TotalCoin;
    }

    public void OpenShop()
    {
        tab2.SetActive(false);
        tab1.SetActive(true);
        StartCoroutine(ItemsAnimation(listItem1));
    }

    public void StartItemAnimation(int index)
    {
        switch(index)
        {
            case 1:
                StartCoroutine(ItemsAnimation(listItem1));
                break;
            case 2:
                StartCoroutine(ItemsAnimation(listItem2));
                break;
        }
    }

    public IEnumerator ItemsAnimation(List<GameObject> listItem)
    {
        foreach(var item in listItem)
        {
            item.transform.localScale = Vector3.zero;
        }
        foreach (var item in listItem)
        {
            item.transform.DOScale(1f, 0.5f).SetEase(Ease.InOutExpo);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void UpdateCoinText()
    {
        foreach(Text coin in coins_text)
        {
            coin.text = DATA.GetCoin().ToString();
        }
    }
}
