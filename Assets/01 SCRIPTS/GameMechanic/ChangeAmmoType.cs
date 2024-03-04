using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public enum AmmoType
{
    ToiletRoll,
    Stone,
    Bomb,
}

[Serializable]
public class Bullet
{
    public AmmoType type;
    public int amount;
}


[Serializable]
public class ButtonChangeAmmoType
{
    public Button btn;
    public Image btnImage;
    public Text txtAmount;
    public AmmoType type;
}

public class ChangeAmmoType : Singleton<ChangeAmmoType>
{
    [SerializeField] List<ButtonChangeAmmoType> btnChangeAmmo;
    AmmoType type;
    [SerializeField] GameManager gameManager;
    [SerializeField] LoadAllAmmoType loadAllAmmoType;
    [SerializeField] ChangeAmmoAnimation changeAmmoAnimation;
  

    private void Start()
    {
        for(int i=0; i< btnChangeAmmo.Count; i++)
        {
            int num = i;
            btnChangeAmmo[num].btn.onClick.AddListener(() =>
            {
                gameManager.hasRunOutOfAmmo = false;
                type = btnChangeAmmo[num].type;
                loadAllAmmoType.PlayerChosingAmmoType(num);
                changeAmmoAnimation.SetImageForCurrentAmmoType(btnChangeAmmo[num].btnImage, btnChangeAmmo[num].txtAmount);
            });
        }
        type = AmmoType.ToiletRoll;
    }

    public void SetLoadAmmoType(LoadAllAmmoType load)
    {
        loadAllAmmoType = load;
    }

    public void Fired()
    {
        switch(CheckBulletTypeRunOut())
        {
            case true:
                for (int i = 0; i < btnChangeAmmo.Count; i++)
                {
                    if (btnChangeAmmo[i].type == type)
                    {
                        btnChangeAmmo[i].btn.interactable = false;
                        btnChangeAmmo[i].txtAmount.text = "0";
                        changeAmmoAnimation.SetImageForCurrentAmmoType(btnChangeAmmo[i].btnImage, btnChangeAmmo[i].txtAmount);

                    }
                }
                //het' sach. dan ko con` loai. nao`
                if (loadAllAmmoType.ChangeBulletTypeWhenRunOut() == -1)
                {
                    gameManager.hasRunOutOfAmmo = true;
                    changeAmmoAnimation.SetImageForCurrentAmmoType(btnChangeAmmo[0].btnImage, btnChangeAmmo[0].txtAmount);
                    type = AmmoType.ToiletRoll;
                    return;
                }

                //Set lai. loai. dan. khi ban' het'
                changeAmmoAnimation.SetImageForCurrentAmmoType(btnChangeAmmo[loadAllAmmoType.ChangeBulletTypeWhenRunOut()].btnImage, btnChangeAmmo[loadAllAmmoType.ChangeBulletTypeWhenRunOut()].txtAmount);
                //set lai kieu? dan.
                type = (AmmoType)loadAllAmmoType.ChangeBulletTypeWhenRunOut();

                break;
            case false:
                for (int i = 0; i < btnChangeAmmo.Count; i++)
                {
                    if (btnChangeAmmo[i].type == type)
                    {
                        btnChangeAmmo[i].txtAmount.text = loadAllAmmoType.CountBulletInQueue(i).ToString();
                        changeAmmoAnimation.SetImageForCurrentAmmoType(btnChangeAmmo[i].btnImage, btnChangeAmmo[i].txtAmount);
                    }
                }
                break;
        }

    }

    public int GetAmmoType()
    {
        return (int)type;
    }

    public bool CheckBulletTypeRunOut()
    {
        if(loadAllAmmoType.CountBulletInQueue((int)type) < 1)
        {
            return true;
        }
        return false;
    }

    public void ResetAmmoTypeAndButton()
    {
        for (int i = 0; i < btnChangeAmmo.Count; i++)
        {
            if (btnChangeAmmo[i].type == type)
            {
                btnChangeAmmo[i].btn.interactable = true;
            }
        }
        type = AmmoType.ToiletRoll;
    }

    public void ChangeButtonInteractWhenCreateNewMap()
    {
        for (int i = 0; i < btnChangeAmmo.Count; i++)
        {
            if (loadAllAmmoType.CountBulletInQueue(i) == 0)
            {
                btnChangeAmmo[i].btn.interactable = false;
                btnChangeAmmo[i].txtAmount.text = "0";
            }
            else
            {
                btnChangeAmmo[i].btn.interactable = true;
                btnChangeAmmo[i].txtAmount.text = loadAllAmmoType.CountBulletInQueue(i).ToString();
            }
        }

        //mac. dinh. la` toilet
        changeAmmoAnimation.SetImageWhenFirstOpenLevel(btnChangeAmmo[0].btnImage, btnChangeAmmo[0].txtAmount);
    }
}
