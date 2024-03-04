using Firebase;
using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class FirebaseManager : MonoBehaviour
{
    public DateTime CurrentTime;
    public List<int> listLevelShowInterAds;
    public double TimeShowInterAds;

    public bool isShowBannerAds = false;
    public bool isShowInterAds = false;
    public bool isShowRewardAds = false;

    public bool isShowInterAdsFromHome = false;
    public bool isShowInterAdsFromGameplay = false;

    public bool isShowOpenAds = false;
    public bool isOffline = false;

    private Firebase.FirebaseApp app;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync().ContinueWith(x =>
                {
                    string[] listStringFirebase = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("show_insterstitial_after_finished_levels").StringValue.Split(',');
                    foreach (string str in listStringFirebase)
                    {
                        listLevelShowInterAds.Add(int.Parse(str));
                    }
                    TimeShowInterAds = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("show_insterstitial_interval").DoubleValue;

                    isShowBannerAds = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("enable_banner_ads").BooleanValue;
                    isShowInterAds = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("enable_insterstitial_ads").BooleanValue;
                    isShowRewardAds = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("enable_reward_ads").BooleanValue;
                    isShowOpenAds = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("enable_open_app_ads").BooleanValue;
                    isShowInterAdsFromHome = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("enable_inter_ads_from_home").BooleanValue;
                    isShowInterAdsFromGameplay = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("enable_inter_ads_from_gameplay").BooleanValue;
                });

            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }

        });

        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
        CurrentTime = DateTime.Now;
    }

    public void PassLevel(int numLevel, int numStar)
    {
        if (DATA.checkIfContainsCompleteLevel(numLevel)) 
        { 
            return; 
        }
        if (numLevel < 9)
        {
            FirebaseAnalytics.LogEvent("Clear_LV_00" + (numLevel + 1).ToString());
        }
        else if (numLevel < 99)
        {
            FirebaseAnalytics.LogEvent("Clear_LV_0" + (numLevel + 1).ToString());           
        }
        else
        {
            FirebaseAnalytics.LogEvent("Clear_LV_" + (numLevel + 1).ToString());
        }
    }

    public void SkipLevel(int numLevel)
    {
        if (DATA.checkIfContainsCompleteLevel(numLevel) || DATA.checkIfContainsSkipLevel(numLevel)) return;
        if (numLevel < 9)
        {
            FirebaseAnalytics.LogEvent("Skip_LV_00" + (numLevel + 1).ToString());
        }
        else if (numLevel < 99)
        {
            FirebaseAnalytics.LogEvent("Skip_LV_0" + (numLevel + 1).ToString());
        }
        else
        {
            FirebaseAnalytics.LogEvent("Skip_LV_" + (numLevel + 1).ToString());
        }
    }

    public void BuySkin(int indexSkin)
    {
        FirebaseAnalytics.LogEvent("BuySkin_" + (indexSkin + 1).ToString());
    }

    public void PlayerIsOffline()
    {
        FirebaseAnalytics.LogEvent("User_Is_Offline");
    }
    public void PlayerIsOnline()
    {
        FirebaseAnalytics.LogEvent("User_Is_Online");
    }

    public void PlayerClickPlayButton()
    {
        FirebaseAnalytics.LogEvent("User_Click_Play_Button");
    }
}
