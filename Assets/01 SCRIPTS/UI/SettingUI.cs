using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingUI : MonoBehaviour
{
    [SerializeField]List<ButtonInDropDown> allBtnSetting;

    [SerializeField] Image imgBackground;
    bool clickDropMenu;
    [SerializeField] ButtonEvent btnSetting;
    [SerializeField] RectTransform rect_Setting;
    [Space(10f)]
    [SerializeField] Sprite[] sound_state;
    [SerializeField] Sprite[] vibrate_state;

    [Space(20f)]
    [SerializeField] SoundManager soundManager;

    private void Start()
    {
        switch(DATA.GetSoundState())
        {
            case true:
                allBtnSetting[0].image.sprite = sound_state[0];
                break;
            case false:
                allBtnSetting[0].image.sprite = sound_state[1];
                break;
        }

        switch (DATA.GetVibrationState())
        {
            case true:
                allBtnSetting[1].image.sprite = vibrate_state[0];
                break;
            case false:
                allBtnSetting[1].image.sprite = vibrate_state[1];
                break;
        }

        btnSetting.onClick.AddListener(() =>
        {
            if (!clickDropMenu)
            {
                OpenSetting();
            }
            else
            {
                CloseSetting();
            }
        });

        allBtnSetting[0].button.onClick.AddListener(() =>
        {
            switch (DATA.GetSoundState())
            {
                case true:
                    DATA.TurnOnSound(false);
                    soundManager.gameObject.SetActive(false);
                    allBtnSetting[0].image.sprite = sound_state[1];
                    break;
                case false:
                    DATA.TurnOnSound(true);
                    soundManager.gameObject.SetActive(true);
                    soundManager.PlaySound(SoundManager.SoundType.Background);
                    allBtnSetting[0].image.sprite = sound_state[0];
                    break;
            }

            
        });

        allBtnSetting[1].button.onClick.AddListener(() =>
        {
            switch (DATA.GetVibrationState())
            {
                case true:
                    DATA.TurnOnVibration(false);
                    allBtnSetting[1].image.sprite = vibrate_state[1];
                    break;
                case false:
                    DATA.TurnOnVibration(true);
                    allBtnSetting[1].image.sprite = vibrate_state[0];
                    Handheld.Vibrate();
                    break;
            }
        });

        allBtnSetting[2].button.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    public void OpenSetting()
    {
        clickDropMenu = true;

        allBtnSetting[0].rectTransform.DOAnchorPosY(-120f, 0.2f);
        allBtnSetting[1].rectTransform.DOAnchorPosY(-252f, 0.2f);
        allBtnSetting[2].rectTransform.DOAnchorPosY(-384f, 0.2f);

        for (int i = 0; i < allBtnSetting.Count; i++)
        {
            allBtnSetting[i].image.DOFade(1f, 0.2f);
        }

        imgBackground.DOFillAmount(1f, 0.2f);
    }

    public void CloseSetting()
    {
        clickDropMenu = false;

        for (int i = 0; i < allBtnSetting.Count; i++)
        {
            allBtnSetting[i].rectTransform.DOAnchorPosY(0f, 0.3f);
            allBtnSetting[i].image.DOFade(0f, 0.3f);
        }
        imgBackground.DOFillAmount(0f, 0.2f);
    }
}
