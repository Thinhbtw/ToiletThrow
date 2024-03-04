using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

[System.Serializable]
public class Products
{
    public string storeKey;
    public int coinValue;
    public productType productType;
}


public enum productType
{
    RemoveAds,
    AddCoin,
    AddCoinAndRemoveAds
}

public class IAP_Manager :Singleton<IAP_Manager>, IStoreListener
{
    public Products[] product;
    public ButtonEvent[] btn_product;
    [SerializeField] ShopUI shopUI; 
    private IStoreController controller;
    [SerializeField] GoogleAdsManager googleAdsManager;
    private bool isInit, hasBuyRemoveAds;
    [SerializeField] GameObject RemoveAdsInShop;
    private IExtensionProvider extensions;

    /*public Text debugText;*/
    

    public void OnInitializeFailed(InitializationFailureReason error)
    {
#if UNITY_EDITOR
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
#endif
        isInit = false;        
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
#if UNITY_EDITOR
        Debug.Log("OnPurchaseReward: Success. Product:" + e.purchasedProduct);
#endif
        OnReward(e.purchasedProduct.definition.id);
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
#if UNITY_EDITOR
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
            i.definition.storeSpecificId, p));
#endif
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
#if UNITY_EDITOR
        Debug.Log("OnInitialized: pass");
#endif
        this.controller = controller;
        this.extensions = extensions;
        isInit = true;            
    }

    [Obsolete]
    private void Init()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        for (int i = 0; i < product.Length; i++)
        {
            builder.AddProduct(product[i].storeKey,
                    ProductType.Consumable, new IDs()
                    {
                    {product[i].storeKey, GooglePlay.Name},
                    {product[i].storeKey, AppleAppStore.Name}
                    });
        }
        UnityPurchasing.Initialize(this, builder);

    }

    private void OnReward(string id)
    {
        for (int i = 0; i < product.Length; i++)
        {
            if (!product[i].storeKey.Equals(id))
            {
                continue;
            }
            switch(product[i].productType)
            {
                case productType.RemoveAds:
                    DATA.SetRemoveAds(true);
                    break;
                case productType.AddCoin:
                    DATA.AddCoin(product[i].coinValue);
                    shopUI.UpdateCoinText();
                    break;
                case productType.AddCoinAndRemoveAds:
                    DATA.AddCoin(product[i].coinValue);
                    DATA.SetRemoveAds(true);
                    shopUI.UpdateCoinText();
                    break;
            }
        }
    }

    [Obsolete]
    private void Start()
    {
        if(googleAdsManager.hasInternet)
        {
            Init();
        }

        if(DATA.GetRemoveAds())
        {
            btn_product[0].gameObject.SetActive(false);
            RemoveAdsInShop.gameObject.SetActive(false);
            hasBuyRemoveAds = true;
        }
        else
        {
            btn_product[0].onClick.AddListener(() =>
            {
                if (!isInit) return;
                if (DATA.GetRemoveAds()) return;
                Buy(product[0]);
            });
        }

        for (int i = 1; i< btn_product.Length;i++)
        {
            int num = i;
            if (num == 1)
            {
                if (DATA.GetRemoveAds())
                    continue;
            }
            btn_product[num].onClick.AddListener(() =>
            {
                if (!isInit) return;
                Buy(product[num-1]);
            });
        }
    }

    [Obsolete]
    private void LateUpdate()
    {
        if(googleAdsManager.hasInternet && !isInit)
        {
            Init();
            isInit = true;
        }
        if (hasBuyRemoveAds) return;
        if(DATA.GetRemoveAds())
        {
            btn_product[0].gameObject.SetActive(false);
            RemoveAdsInShop.SetActive(false);
            hasBuyRemoveAds = true;
        }
    }

    public void Buy(Products pd)
    {
        if (pd.storeKey == null)
            return;                
        try
        {
            controller.InitiatePurchase(pd.storeKey);
        }
        catch (Exception e)
        {
            
            /*debugText.text = "IPA === logIAP :::" + e;*/
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }
}
