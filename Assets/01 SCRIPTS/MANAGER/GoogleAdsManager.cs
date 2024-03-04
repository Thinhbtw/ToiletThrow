using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using OneSignalSDK;

public class GoogleAdsManager : MonoBehaviour
{
    public bool gameIsOn, hasInternet, adsHint, adsSkip, loadFirstInterAdFailed;
    // Start is called before the first frame update
    private AppOpenAd appOpenAd;
    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    [SerializeField] ButtonManager buttonManager;
    [SerializeField] FirebaseManager firebaseManager;
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] ShopUI shopUI;
    //[SerializeField] Text errorTxt;
    bool hasShowFirstInterAds = false;
    bool isNotShowAppOpen = false;

    public bool hasLoadReward = false;
    string _openAppId = "ca-app-pub-4810228587303243/3967504202";

    [Obsolete]
    private void Start()
    {
        // Replace 'YOUR_ONESIGNAL_APP_ID' with your OneSignal App ID from app.onesignal.com
        OneSignal.Default.Initialize("a07fad35-2ba7-4b5d-bfe0-3d89c3216be9");


        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);

        //user treo may' nhieu qua'  thi` //
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        //}      
        //AppStateEventNotifier.AppStateChanged += OnAppStateChanged;

        StartCoroutine(CheckInternetConnection());
    }

    [Obsolete]
    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            //RequestBannerAd();
            if (!DATA.GetRemoveAds())
            {
                RequestAndLoadFullscreenNextLevelInterstitialAd();
                LoadAppOpenAd();
            }
            RequestAndLoadRewardedAd();
        });
    }


    #region HELPER METHODS

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            .AddKeyword("unity-admob-sample")
            .Build();
    }

    #endregion
    #region BANNER ADS

    public void RequestBannerAd()
    {

        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-4810228587303243/1241214618";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up banner before reusing
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        // Load a banner ad
        if (firebaseManager.isShowBannerAds)
            bannerView.LoadAd(CreateAdRequest());
    }
    public void DestroyBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
    }

    #endregion
    #region INTERSTITIAL ADS

    public void RequestAndLoadFullscreenNextLevelInterstitialAd()
    {
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-4810228587303243/9415218101";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up interstitial before using it
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }

        // Load an interstitial ad
        InterstitialAd.Load(adUnitId, CreateAdRequest(),
            (InterstitialAd ad, LoadAdError loadError) =>
            {
                // if error is not null, the load request failed.
                if (loadError != null || ad == null)
                {
                    loadFirstInterAdFailed = true;
                    return;
                }

                interstitialAd = ad;
                ad.OnAdFullScreenContentClosed += () =>
                {
                    //buttonManager.eventsManager.enabled = true;
                    RequestAndLoadFullscreenNextLevelInterstitialAd();
                };
                ad.OnAdFullScreenContentOpened += () =>
                {
                    //buttonManager.eventsManager.enabled = false;
                };
                ad.OnAdClicked += () =>
                {

                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    RequestAndLoadFullscreenNextLevelInterstitialAd();
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {

                };
            });
    }
    public void ShowInterWhenInGameplay()
    {
        if (DATA.GetRemoveAds()) return;
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            isNotShowAppOpen = true;
            if (!firebaseManager.isShowInterAds) return;
            if (firebaseManager.isShowInterAdsFromGameplay)
            {
                if ((DateTime.Now - firebaseManager.CurrentTime).TotalSeconds > firebaseManager.TimeShowInterAds)
                {
                    interstitialAd.Show();
                    firebaseManager.CurrentTime = DateTime.Now;
                }

            }
        }
    }

    public void ShowInterWhenNotInGameplay()
    {
        if (DATA.GetRemoveAds()) return;
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            isNotShowAppOpen = true;
            if (!firebaseManager.isShowInterAds) return;
            if (firebaseManager.isShowInterAdsFromHome)
            {
                if ((DateTime.Now - firebaseManager.CurrentTime).TotalSeconds > firebaseManager.TimeShowInterAds)
                {
                    interstitialAd.Show();
                    firebaseManager.CurrentTime = DateTime.Now;
                }

            }
        }
    }

    public bool CheckIfInterCanShow()
    {
        if(interstitialAd !=null && interstitialAd.CanShowAd())
        {
            if (!firebaseManager.isShowInterAds) return false;
            return true;
        }
        return false;
    }

    public void ShowFullScreenFirstOpen()
    {
        if (DATA.GetRemoveAds()) return;
        if (!firebaseManager.isShowInterAds) return;

        interstitialAd.Show();
        hasShowFirstInterAds = isNotShowAppOpen = true;
    }

    public bool CanShowFullscreenWhenFirstOpen()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            return true;
        }
        return false;
    }

    public void DestroyFullscreenNextLevelInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
    }

    #endregion
    #region AppOpen
    public bool IsAppOpenAdAvailable
    {
        get
        {
            return (appOpenAd != null
                    && appOpenAd.CanShowAd()
                   );
        }
    }
    //public void OnAppStateChanged(AppState state)
    //{
    //    // Display the app open ad when the app is foregrounded.
    //    UnityEngine.Debug.Log("App State is " + state);

    //    // OnAppStateChanged is not guaranteed to execute on the Unity UI thread.
    //    MobileAdsEventExecutor.ExecuteInUpdate(() =>
    //    {
    //        if (state == AppState.Foreground)
    //        {
    //            ShowAppOpenAd();
    //        }
    //    });
    //}
    public void ShowAppOpenAd()
    {
        if (appOpenAd != null && appOpenAd.CanShowAd())
        {
            //errorTxt.text = "Showing app open ad.";
            appOpenAd.Show();
        }
        else
        {
            return;
            //errorTxt.text = "App open ad is not ready yet.";
        }

    }
    public bool IsAdAvailable
    {
        get
        {
            return _openAppId != null;
        }
    }

    public void LoadAppOpenAd()
    {
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }
        //errorTxt.text = "Loading the app open ad.";

        // Create our request used to load the ad.
        //var adRequest = new AdRequest.Builder().Build();

        // send the request to load the ad.
        AppOpenAd.Load(_openAppId, ScreenOrientation.Portrait, CreateAdRequest(),
            (AppOpenAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    /*errorTxt.text = "app open ad failed to load an ad " +
                                   "with error : " + error;*/
                    return;
                }

                /*errorTxt.text = "App open ad loaded with response : "
                              + ad.GetResponseInfo();*/

                appOpenAd = ad;
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    /*errorTxt.text = String.Format("App open ad paid {0} {1}.",
                            adValue.Value,
                            adValue.CurrencyCode);*/
                };
                // Raised when an impression is recorded for an ad.
                ad.OnAdImpressionRecorded += () =>
                {
                    //errorTxt.text = "App open ad recorded an impression.";
                };
                // Raised when a click is recorded for an ad.
                ad.OnAdClicked += () =>
                {
                    // errorTxt.text = "App open ad was clicked.";
                };
                // Raised when an ad opened full screen content.
                ad.OnAdFullScreenContentOpened += () =>
                {
                    //buttonManager.eventsManager.enabled = false;
                    //errorTxt.text = "App open ad full screen content opened.";
                };
                // Raised when the ad closed full screen content.
                ad.OnAdFullScreenContentClosed += () =>
                {
                    //buttonManager.eventsManager.enabled = true;
                    //errorTxt.text = "App open ad full screen content closed.";
                    LoadAppOpenAd();
                };
                // Raised when the ad failed to open full screen content.
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    /*errorTxt.text = "App open ad failed to open full screen content " +
                                       "with error : " + error;*/
                    LoadAppOpenAd();
                };

            });
    }

    public void DestroyAppOpenAd()
    {
        if (this.appOpenAd != null)
        {
            this.appOpenAd.Destroy();
            this.appOpenAd = null;
        }
    }
    #endregion
    #region REWARDED ADS

    public void RequestAndLoadRewardedAd()
    {
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-4810228587303243/1532912550";
#else
        string adUnitId = "unexpected_platform";
#endif

        // create new rewarded ad instance
        RewardedAd.Load(adUnitId, CreateAdRequest(),
            (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null || ad == null)
                {
                    hasLoadReward = false;
                    return;
                }
                rewardedAd = ad;
                ad.OnAdFullScreenContentOpened += () =>
                {

                };
                ad.OnAdImpressionRecorded += () =>
                {

                };
                ad.OnAdClicked += () =>
                {

                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    hasLoadReward = false;
                    //RequestAndLoadRewardedAd();
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    hasLoadReward = false;
                    Ad_OnUserEarnedReward();
                    //RequestAndLoadRewardedAd();
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {

                };
            });
    }

    private void Ad_OnUserEarnedReward()
    {
        if (buttonManager.clickSkip)
        {
            Invoke("DelaySkipLevel", 0.3f);
            buttonManager.clickSkip = false;
        }
        if (buttonManager.clickDoubleReward)
        {
            DATA.AddCoin(DATA.GetDoubleCoin());
            shopUI.UpdateCoinText();
            Invoke("DelayNextLevel", 0.3f);
            buttonManager.clickDoubleReward = false;
        }

        if (buttonManager.clickEarn20coin)
        {
            Invoke("DelayAdd20Coin", 0.3f);
        }
    }

    private void DelayAdd20Coin()
    {
        DATA.AddCoin(20);
        shopUI.UpdateCoinText();
        buttonManager.clickEarn20coin = false;
    }

    private void DelaySkipLevel()
    {
        levelLoader.SkipLevel();
    }

    private void DelayNextLevel()
    {
        levelLoader.NextLevel();
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null)
        {
            isNotShowAppOpen = true;
            rewardedAd.Show((GoogleMobileAds.Api.Reward reward) => { });
        }
    }

    public bool hasLoadRewardedAds()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            hasLoadReward = true;
            return true;
        }
        return false;
    }
    #endregion

    public bool hasLoadOpenAds()
    {
        if (appOpenAd.CanShowAd())
            return true;
        return false;
    }

    public bool hasLoadIntersAds()
    {
        if (interstitialAd.CanShowAd())
            return true;
        return false;
    }

    [Obsolete]
    IEnumerator CheckInternetConnection() //set lai. gameIsOn thanh` true;
    {
        while (gameIsOn)
        {
            yield return new WaitForSeconds(1f);
            const string echoServer = "http://www.example.com";

            bool result;
            using (var request = UnityWebRequest.Head(echoServer))
            {
                request.timeout = 3;
                yield return request.SendWebRequest();
                result = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;
            }
            if (result)
            {
                hasInternet = true;
                firebaseManager.isOffline = false;
            }
            else
            {
                hasInternet = false;
                firebaseManager.isOffline = true;
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (DATA.GetRemoveAds()) return;
        if (!pause && !isNotShowAppOpen)
        {
            ShowAppOpenAd();
        }
        Invoke("ResetAppOpen", 0.25f);
    }

    public void ResetAppOpen()
    {
        isNotShowAppOpen = false;
    }

    private void LateUpdate()
    {
        if (hasInternet && !hasLoadReward)
        {
            hasLoadReward = true;
            RequestAndLoadRewardedAd();
        }
    }
}
