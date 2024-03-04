using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    public float timeSet;
    float TimeLeft;
    public Text TimerTxt;
    [SerializeField] GameManager gameManager;
    [SerializeField] StarSystem starSystem;
    [SerializeField] GameObject uiGameCoplete;
    [SerializeField] SoundManager soundManager;
    bool checkHasAddedStar;

    private void Start()
    {
        TimeLeft = timeSet;
        checkHasAddedStar = false;
    }

    void Update()
    {
        if (gameManager.endGame)
        {
            if (TimeLeft >= 20f && !checkHasAddedStar)
            {
                starSystem.Gain1Star();
                checkHasAddedStar = true;
                return;
            }
            return;
        }
        if (!gameManager.startShoot) return;
        
        if (TimeLeft > 1)
        {
            TimeLeft -= Time.deltaTime;
            updateTimer(TimeLeft);
        }
        else
        {
            gameManager.PlayerComplete(false, 1.2f);

            soundManager.PlaySound(SoundManager.SoundType.Lose2);
        }
        
    }

    void updateTimer(float currentTime)
    {
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        TimerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ResetAll()
    {
        //reset timer
        updateTimer(timeSet);
        TimeLeft = timeSet;

        //reset sao
        checkHasAddedStar = false;
        starSystem.ResetStar();
    }
}
