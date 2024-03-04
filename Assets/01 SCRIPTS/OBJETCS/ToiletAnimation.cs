using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletAnimation : MonoBehaviour
{
    [SerializeField] GameObject idle_anim, win_anim, lose_anim;
    [SerializeField] Animator toiletAnimator, wasteAnimator, wasteAnimator2;
    [SerializeField] HandFollow handFollow;

    private void Start()
    {
        toiletAnimator.enabled = false;
        wasteAnimator.enabled = wasteAnimator2.enabled = false;
        GameManager.Instance.SetScriptToiletAnimation(this);
    }
    
    public void RunAnimationWin()
    {
        idle_anim.gameObject.SetActive(false);
        win_anim.gameObject.SetActive(true);
        handFollow.PlayWinAnimation();
    }

    public void RunAnimationLose()
    {
        idle_anim.gameObject.SetActive(false);
        lose_anim.gameObject.SetActive(true);
        toiletAnimator.enabled = true;
        wasteAnimator.enabled = true;
        wasteAnimator2.enabled = true;
        handFollow.PlayLoseAnimation();
        StartCoroutine(TurnOffAnimatonAfter(1.5f));
    }

    IEnumerator TurnOffAnimatonAfter(float second)
    {
        yield return new WaitForSeconds(second);
        lose_anim.gameObject.SetActive(false);
        toiletAnimator.enabled = false;
    }

}
