using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarSystem : Singleton<StarSystem>
{
    public int cur_star;
    [SerializeField] List<Image> star;
    [SerializeField] Sprite[] imgStar;
    int t;

    private void Start()
    {
        cur_star = 0;
    }

    public void Gain1Star()
    {
        cur_star++;
    }

    public IEnumerator DelayStar()
    {
        while (t < cur_star)
        {
            yield return new WaitForSeconds(0.8f);
            star[t].sprite = imgStar[1];
            if (t == 2)
                SoundManager.Instance.PlaySound(SoundManager.SoundType.Star2);
            else
                SoundManager.Instance.PlaySound(SoundManager.SoundType.Star1);
            t++;
        }
    }

    public void TurnOffTextCointEarn()
    {
        for (int i = 0; i < star.Count; i++)
        {
            star[i].sprite = imgStar[0];
        }
    }

    

    public void ResetStar()
    {
        cur_star = t = 0;
        StopAllCoroutines();
        for (int i = 0; i < star.Count; i++)
        {
            star[i].sprite = imgStar[0];
        }
    }

}
