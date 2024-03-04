using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTutorial : MonoBehaviour
{
    [SerializeField] GameObject Tutorial, TutorialLevel1;
    [SerializeField] GameObject handPointing_anim_1, handPointing_anim_2;

    public void EnableGameTutorial(bool t)
    {
        Tutorial.SetActive(t);
    }

    public void EnableHandAnimWhenClick(bool hand1, bool hand2)
    {
        handPointing_anim_1.SetActive(hand1);
        handPointing_anim_2.SetActive(hand2);
    }

    public void EnableTutorialLevel1(bool t)
    {
        TutorialLevel1.SetActive(t);
    }
}
