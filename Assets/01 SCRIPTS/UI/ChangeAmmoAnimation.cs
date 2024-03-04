using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

[Serializable]
public struct ButtonInDropDown
{
    public RectTransform rectTransform;
    public ButtonEvent button;
    public Image image;
    public Text text;
}

public class ChangeAmmoAnimation : Singleton<ChangeAmmoAnimation>
{
    [SerializeField] ChangeAmmoType changeAmmoType;
    [SerializeField] Button btnDrop;
    [SerializeField] Image currentAmmo, imgSlide;
    [SerializeField] Text curAmmoAmount;
    [SerializeField] GameTutorial gameTutorial;
    [SerializeField] GameObject first_Tutorial;
    [Space(20f)]
    [SerializeField] List<ButtonInDropDown> buttonInDropDowns;
    bool clickDropMenu;

    private void Start()
    {
        clickDropMenu = false;
        btnDrop.onClick.AddListener(() =>
        {
            if (!clickDropMenu)
            {
                OpenDropDown();
            }
            else
            {
                CloseDropDown();
            }
        });
    }
    public void OpenDropDown()
    {
        clickDropMenu = true;

        imgSlide.DOFillAmount(.8f, 0.2f);
        buttonInDropDowns[0].rectTransform.DOAnchorPosY(-111f, 0.2f);
        buttonInDropDowns[1].rectTransform.DOAnchorPosY(-210f, 0.2f);

        for (int i = 0; i < buttonInDropDowns.Count; i++)
        {
            buttonInDropDowns[i].image.DOFade(1f, 0.2f);
            buttonInDropDowns[i].text.DOFade(1f, 0.2f);
            //buttonInDropDowns[i].button.interactable = true;
        }

        if(first_Tutorial.activeInHierarchy)
        {
            gameTutorial.EnableHandAnimWhenClick(false, true);
        }
    }

    public void CloseDropDown()
    {
        clickDropMenu = false;

        imgSlide.DOFillAmount(0f, 0.3f);
        for (int i = 0; i < buttonInDropDowns.Count; i++)
        {
            buttonInDropDowns[i].rectTransform.DOAnchorPosY(0f, 0.3f);
            buttonInDropDowns[i].image.DOFade(0f, 0.3f);
            buttonInDropDowns[i].text.DOFade(0f, 0.3f);
            //buttonInDropDowns[i].button.interactable = false;
        }

        gameTutorial.EnableHandAnimWhenClick(false, false);
        gameTutorial.EnableGameTutorial(false);
    }

    public void SetImageForCurrentAmmoType(Image curAmmo, Text curAmount)
    {
        currentAmmo.sprite = curAmmo.sprite;
        curAmmoAmount.text = curAmount.text;
        CloseDropDown();
    }

    public void SetImageWhenFirstOpenLevel(Image curAmmo, Text curAmount) //ham` chi? de? sua? khi bat. tutorial o? lvl 6
    {
        currentAmmo.sprite = curAmmo.sprite;
        curAmmoAmount.text = curAmount.text;
    }

}
