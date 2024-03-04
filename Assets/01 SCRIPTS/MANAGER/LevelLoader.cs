using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    List<GameObject> listLevelIG;
    int cur_level = 0;
    [SerializeField] Transform levelHolder;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameTutorial gameTutorial;
    [SerializeField] ChangeAmmoAnimation changeAmmoAnimation;
    [SerializeField] FirebaseManager firebaseManager;
    [SerializeField] GoogleAdsManager googleAdsManager;
    void Awake()
    {
        listLevelIG = new List<GameObject>(Resources.LoadAll<GameObject>("Level"));
        //InstantiateLevel(cur_level);
        
    }

    public void InstantiateLevel(int levelNumber)
    {
        if (firebaseManager.listLevelShowInterAds.Contains(cur_level))
        {
            googleAdsManager.ShowInterWhenInGameplay();
        }
        if(googleAdsManager.CheckIfInterCanShow())
        {
            StartCoroutine(SpawnLevelAfterCertainTime(levelNumber, 0.15f));
        }
        else
        {
            StartCoroutine(SpawnLevelAfterCertainTime(levelNumber, 0f));
        }

    }

    IEnumerator SpawnLevelAfterCertainTime(int levelNumber, float timer) 
    {
        yield return new WaitForSeconds(timer);
        gameTutorial.EnableTutorialLevel1(false);

        if (levelHolder.childCount > 0)
        {
            gameManager.ResetEveryThingToDefault();
            Destroy(levelHolder.GetChild(0).gameObject);
            Instantiate(listLevelIG[levelNumber], levelHolder);
        }
        else
        {
            Instantiate(listLevelIG[levelNumber], levelHolder);
        }
        cur_level = levelNumber;

        changeAmmoAnimation.CloseDropDown();

        switch (levelNumber)
        {
            case 0:
                gameTutorial.EnableTutorialLevel1(true);
                break;
            case 40:
                gameTutorial.EnableGameTutorial(true);
                gameTutorial.EnableHandAnimWhenClick(true, false);
                break;
        }
    }

    public int GetCurrentLevel()
    {
        return cur_level;
    }

    public void ResetLevel()
    {
        InstantiateLevel(cur_level);
    }

    public void NextLevel()
    {
        if (CheckIfHasRunOutOfLevel()) return;
        cur_level++;

        InstantiateLevel(cur_level);
    }

    public void SkipLevel()
    {
        if (CheckIfHasRunOutOfLevel()) return;
        firebaseManager.SkipLevel(cur_level);
        DATA.AddLevelSkip(cur_level);
        cur_level++;

        InstantiateLevel(cur_level);
    }

    public bool CheckIfHasRunOutOfLevel()
    {
        if (cur_level + 1 >= listLevelIG.Count) return true;
        return false;
    }

    public void DestroyLevelWhenBack()
    {
        if (levelHolder.childCount > 0)
        {
            gameManager.ResetEveryThingToDefault();
            Destroy(levelHolder.GetChild(0).gameObject);
            //changeAmmoType.ResetListBulletRemain(levelNumber);
        }
    }

    public int CountTotalLevel()
    {
        return listLevelIG.Count;
    }
}
