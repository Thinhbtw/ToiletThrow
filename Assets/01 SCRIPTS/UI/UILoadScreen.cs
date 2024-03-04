using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UILoadScreen : MonoBehaviour
{
    //[SerializeField] Image progress;
    [SerializeField] Image[] shit;
    [SerializeField] float duration;
    [SerializeField] Text percentText;
    float loadingPercent;
    bool hasRun;
    [SerializeField] public Image Transition;
    [SerializeField] GameObject Home;
    [SerializeField] GameObject soundManager;
    [SerializeField] GoogleAdsManager googleAdsManager;
    [SerializeField] FirebaseManager firebaseManager;
    [SerializeField] ButtonManager buttonManager;
    float timer = 5f;
    

    // Start is called before the first frame update
    void Start()
    {

        Transition.DOFade(0f, 2f);

        StartCoroutine(ChangeSomeValue(0, 100, duration));
        
    }
    
    public IEnumerator ChangeSomeValue(int oldValue, int newValue, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            //progress.fillAmount = Mathf.Lerp(oldValue, newValue, t / duration);
            loadingPercent = Mathf.Lerp(oldValue, newValue, t / duration);
            percentText.text = loadingPercent.ToString("F0") + "%";
            switch (loadingPercent)
            {
                case float x when x > 99f:
                    shit[4].gameObject.SetActive(true);
                    break;
                case float x when x > 80f:
                    shit[3].gameObject.SetActive(true);
                    break;
                case float x when x > 60f:
                    shit[2].gameObject.SetActive(true);
                    break;
                case float x when x > 40f:
                    shit[1].gameObject.SetActive(true);
                    break;
                case float x when x > 20f:
                    shit[0].gameObject.SetActive(true);
                    break;
            }

            yield return null;
        }
        //progress.fillAmount = newValue;
        StartCoroutine(toTheGame());
    }


    public IEnumerator toTheGame()
    {
        yield return new WaitForSeconds(0.5f);
        soundManager.SetActive(true);
        Home.SetActive(true);
        gameObject.SetActive(false);
        if (!googleAdsManager.hasInternet)
        {
            firebaseManager.PlayerIsOffline();
        }
        else
        {
            
            firebaseManager.PlayerIsOnline();
        }
        if (PlayerPrefs.HasKey("FirstOpen"))
        {
            if (googleAdsManager.CanShowFullscreenWhenFirstOpen())
            {
                googleAdsManager.ShowFullScreenFirstOpen();
            }
        }

        if(!PlayerPrefs.HasKey("FirstOpen"))
        {
            PlayerPrefs.SetInt("FirstOpen", 1);
            buttonManager.UserPlayGame();
        }

    }

    private void Update()
    {
        if (googleAdsManager.CanShowFullscreenWhenFirstOpen() && !hasRun)
        {
            hasRun = true;
            StopCoroutine(ChangeSomeValue(0, 100, duration));
            StartCoroutine(toTheGame());
        }
        timer -= Time.deltaTime;
        if(timer<0 && !googleAdsManager.hasInternet || timer < 0 && DATA.GetRemoveAds())
        {
            StopCoroutine(ChangeSomeValue(0, 100, duration));
            StartCoroutine(toTheGame());
        }
    }
}
