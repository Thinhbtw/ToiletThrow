using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonManager : MonoBehaviour
{
    [SerializeField]GameManager gameManager;
    [SerializeField] LevelLoader levelLoader;

    [Space(20f)]
    [Header("---UI_GAMEPLAY---")]
    [SerializeField] ButtonEvent btnBack;
    [SerializeField] ButtonEvent[] btnReplay;
    [SerializeField] ButtonEvent[] btnSkip;

    [Space(20f)]
    [Header("---UI_HOME---")]
    [SerializeField] ButtonEvent btnPlay;
    [SerializeField] ButtonEvent btnShop;
    [SerializeField] GameObject gameplayCanvas, homeCanvas, backgroundCanvas;

    [Space(20f)]
    [Header("---UI_SELECTING_LEVEL---")]
    [SerializeField] ButtonEvent btnBackFromSelectingLevel;
    [SerializeField] GameObject selectLVCanvas;

    [Space(20f)]
    [Header("---UI_SHOP---")]
    [SerializeField] ButtonEvent btnBackFromShop;
    [SerializeField] ButtonEvent btnTab1, btnTab2, btnWatchAds;
    [SerializeField] GameObject shopCanvas, itemTab1, itemTab2;
    [SerializeField] ShopUI shopUI;
    bool checkTab;

    [Space(20f)]
    [Header("---UI_END---")]
    [SerializeField]ButtonEvent btnNextLevel;
    [SerializeField]ButtonEvent btnDoubleReward;
    [SerializeField] ButtonEvent btnSelectLevel;
    [SerializeField] EndUI endUI;
    [SerializeField] StarSystem starSystem;
    [SerializeField] GameObject[] animLoading;

    [Space(20f)]
    [Header("---UI_RATEUS---")]
    [SerializeField] Button btnRateUs;
    [SerializeField] ButtonEvent btnExitRateUs;
    public GameObject rateUsCanvas;

    [Space(20f)]
    [Header("---SCRIPT---")]
    [SerializeField] ChangeAmmoType changeAmmoType;
    [SerializeField] GoogleAdsManager googleAdsManager;
    [SerializeField] CameraFollow cameraFollow;
    [SerializeField] FirebaseManager firebaseManager;
    public bool clickSkip, clickDoubleReward, clickEarn20coin, hasLoadedReward;

    void Start()
    {
        checkTab = clickSkip = clickDoubleReward = clickEarn20coin = false;
        hasLoadedReward = true;

        //BUTTON NEXT LEVEL
        btnNextLevel.onClick.AddListener(() =>
        {
            if (levelLoader.CheckIfHasRunOutOfLevel())
            {
                selectLVCanvas.SetActive(true);
                levelLoader.DestroyLevelWhenBack();
                changeAmmoType.StopAllCoroutines();
                cameraFollow.ResetCamToDefault(3f);
                gameplayCanvas.SetActive(false);
                backgroundCanvas.SetActive(false);
                return;
            }

            levelLoader.NextLevel();

            gameManager.ResetEveryThingToDefault();

            shopUI.UpdateCoinText();
        });

        //BUTTON REPLAY
        btnReplay[1].onClick.AddListener(() => //trong gameplay
        {
            if (gameManager.endGame) return;

            levelLoader.ResetLevel();
            EnableButtonOnWin(false);
            changeAmmoType.StopAllCoroutines();

            cameraFollow.ResetCamToDefault(3f);

            gameManager.ResetEveryThingToDefault();
        });
        
        btnReplay[0].onClick.AddListener(() => //trong ui end
        {        

            levelLoader.ResetLevel();
            EnableButtonOnWin(false);
            cameraFollow.ResetCamToDefault(3f);

            gameManager.ResetEveryThingToDefault();

            shopUI.UpdateCoinText();
        });

        //BUTTON SKIP
        btnSkip[0].onClick.AddListener(() => //trong ui end
        {
            if (!googleAdsManager.hasLoadRewardedAds()) return;
            if (clickSkip) return;
            if (levelLoader.CheckIfHasRunOutOfLevel()) return;

            clickSkip = true;
            //levelLoader.SkipLevel();
            googleAdsManager.ShowRewardedAd();

            cameraFollow.ResetCamToDefault(3f);

            gameManager.ResetEveryThingToDefault();

        });

        btnSkip[1].onClick.AddListener(() => //trong gameplay
        {
            if (!googleAdsManager.hasLoadRewardedAds()) return;
            if (clickSkip) return;
            if (gameManager.endGame) return;
            if (levelLoader.CheckIfHasRunOutOfLevel()) return;

            clickSkip = true;
            //levelLoader.SkipLevel();
            changeAmmoType.StopAllCoroutines();

            googleAdsManager.ShowRewardedAd();

            cameraFollow.ResetCamToDefault(3f);

            gameManager.ResetEveryThingToDefault();

        });

        //UI gameplay
        btnBack.onClick.AddListener(() =>
        {
            if (gameManager.endGame) return;

            googleAdsManager.ShowInterWhenNotInGameplay();

            selectLVCanvas.SetActive(true);
            levelLoader.DestroyLevelWhenBack();

            changeAmmoType.StopAllCoroutines();

            cameraFollow.ResetCamToDefault(3f);

            gameplayCanvas.SetActive(false);
            backgroundCanvas.SetActive(false);

        });


        //UI Home
        #region UI_HOME
        btnPlay.onClick.AddListener(() =>
        {
            UserPlayGame();
            firebaseManager.PlayerClickPlayButton();
        });

        btnShop.onClick.AddListener(() =>
        {
            googleAdsManager.ShowInterWhenNotInGameplay();

            shopCanvas.SetActive(true);
            homeCanvas.SetActive(false);
            shopUI.OpenShop();
        });
        #endregion

        //UI Select Level
        btnBackFromSelectingLevel.onClick.AddListener(() =>
        {
            googleAdsManager.ShowInterWhenNotInGameplay();
            homeCanvas.SetActive(true);
            selectLVCanvas.SetActive(false);
        });


        //UIShop
        btnBackFromShop.onClick.AddListener(() =>
        {
            
            homeCanvas.SetActive(true);
            shopCanvas.SetActive(false);
            checkTab = false;
        });

        btnTab1.onClick.AddListener(() =>
        {
            if (!checkTab) return;

            itemTab1.SetActive(true);
            itemTab2.SetActive(false);
            shopUI.StartItemAnimation(1);
            checkTab = false;
        });

        btnTab2.onClick.AddListener(() =>
        {
            if (checkTab) return;

            itemTab1.SetActive(false);
            itemTab2.SetActive(true);
            shopUI.StartItemAnimation(2);
            checkTab = true;
        });

        btnWatchAds.onClick.AddListener(() =>
        {
            if (clickEarn20coin) return;
            clickEarn20coin = true;
            googleAdsManager.ShowRewardedAd();
        });

        //BUTTON UI END
        btnSelectLevel.onClick.AddListener(() =>
        {
            levelLoader.DestroyLevelWhenBack();
            selectLVCanvas.SetActive(true);
            gameplayCanvas.SetActive(false);
            backgroundCanvas.SetActive(false);
            TurnOnEndUI(false);

            googleAdsManager.ShowInterWhenNotInGameplay();
        });

        btnDoubleReward.onClick.AddListener(() =>
        {
            if (clickDoubleReward) return;
            clickDoubleReward = true;
            googleAdsManager.ShowRewardedAd();
        });

        //BUTTON UI RATE US
        btnRateUs.onClick.AddListener(() =>
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.gen.toiletthrow");
            PlayerPrefs.SetInt("HasOpenRateUs", 1);
            rateUsCanvas.SetActive(false);
        });

        btnExitRateUs.onClick.AddListener(() =>
        {
            rateUsCanvas.SetActive(false);
        });
    }

    public void UserPlayGame()
    {
        homeCanvas.SetActive(false);
        gameplayCanvas.SetActive(true);
        backgroundCanvas.SetActive(true);
        if (DATA.GetCurrentLevelPlay() == levelLoader.CountTotalLevel())
        {
            levelLoader.InstantiateLevel(DATA.GetCurrentLevelPlay() - 1);
        }
        else
        {
            levelLoader.InstantiateLevel(DATA.GetCurrentLevelPlay());
        }
    }


    //UI End
    public void EnableButtonOnWin(bool trueIfWon)
    {
        btnNextLevel.gameObject.SetActive(trueIfWon);
        btnSkip[0].gameObject.SetActive(!trueIfWon);
        btnDoubleReward.gameObject.SetActive(trueIfWon);
    }

    public void TurnOnEndUI(bool t)
    {
        endUI.gameObject.SetActive(t);
    }

    public void EndUICaseWinLose(bool trueIfWon)
    {
        EnableButtonOnWin(trueIfWon);
        if (trueIfWon)
        {
            endUI.CaseWin();
            starSystem.Gain1Star();
            SoundManager.Instance.PlaySound(SoundManager.SoundType.UIEndWin);
            StartCoroutine(starSystem.DelayStar());
            return;
        }
        starSystem.TurnOffTextCointEarn();
        endUI.CaseLose();
    }



    //UI Selecting Level
    public void FromSelectLevelToGameplayCanvas()
    {
        gameplayCanvas.SetActive(true);
        backgroundCanvas.SetActive(true);
        selectLVCanvas.SetActive(false);
    }

    public void LateUpdate()
    {
        if(!googleAdsManager.hasLoadRewardedAds())
        {
            if (!hasLoadedReward) return;
            btnWatchAds.interactable = hasLoadedReward = btnDoubleReward.interactable = btnSkip[1].interactable = btnSkip[0].interactable = false;
            foreach(GameObject o in animLoading)
            {
                o.SetActive(true);
            }
        }
        else
        {
            if (hasLoadedReward) return;
            btnWatchAds.interactable = hasLoadedReward = btnDoubleReward.interactable = btnSkip[1].interactable = btnSkip[0].interactable = true;
            foreach (GameObject o in animLoading)
            {
                o.SetActive(false);
            }
        }    
    }
}
