using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class LevelData
{
    public List<int> levelComplete = new List<int>();
    public List<int> levelSkip = new List<int>();
    public List<int> star = new List<int>();
    public int currentLevel = 0;
   
}

[Serializable]
public class PlayerSetting
{
    public bool sound = true;
    public bool vibrateOn = true;
    public bool removeAds = false;
}

[Serializable]
public class PlayerCosmetic 
{
    public int coin = 0;
    public int addedCoin = 0;
    public int selectedSkinIndex = 0;
    public List<int> skinOwned = new List<int>();
}

public static class DATA
{
    static LevelData levelData = new LevelData();
    static PlayerSetting playerSetting = new PlayerSetting();
    static PlayerCosmetic playerCosmetic = new PlayerCosmetic();
    static Skin selectedSkin;

    static DATA()
    {
        LoadPlayerData();
    }

    #region Level
    public static void AddLevelComplete(int levelName, int newScore)
    {
        if (levelData.levelComplete.Contains(levelName))
        {
            checkIF_NewScoreIsBetter(levelName, newScore);
            SavePlayerLevel();
            return;
        }

        if (checkIfContainsSkipLevel(levelName)) //neu' choi lai, man` skip
        {
            levelData.levelComplete.Add(levelName);
            checkIF_NewScoreIsBetter(levelName, newScore);
            levelData.levelSkip.Remove(levelName);
        }
        else
        {
            levelData.levelComplete.Add(levelName);
            levelData.star.Add(newScore);
            AddCoinWhenFirstPlay(newScore);
            levelData.currentLevel++;
        }
        SavePlayerLevel();
    }

    public static void AddLevelSkip(int levelName)
    {
        if (levelData.levelSkip.Contains(levelName) || levelData.levelComplete.Contains(levelName)) return;
        levelData.levelSkip.Add(levelName);
        levelData.star.Add(0);
        levelData.currentLevel++;
        SavePlayerLevel();
    }

    public static int GetCurrentLevelPlay()
    {
        return levelData.currentLevel;
    }

    public static int CountLevelComplete()
    {
        return levelData.levelComplete.Count;
    }
    public static int CountLevelSkip()
    {
        return levelData.levelSkip.Count;
    }

    public static bool checkIfContainsCompleteLevel(int numlevel)
    {
        if (levelData.levelComplete.Contains(numlevel))
            return true;
        return false;
    }

    public static bool checkIfContainsSkipLevel(int numlevel)
    {
        if (levelData.levelSkip.Contains(numlevel))
            return true;
        return false;
    }

    public static int GetStarAtLevel(int numLevel)
    {
        if (levelData.levelComplete.Contains(numLevel))
        {
            return levelData.star[numLevel];
        }
        else
            return 0;
    }

    public static void checkIF_NewScoreIsBetter(int numLevel, int newScore)
    {
        int temp = GetStarAtLevel(numLevel);
        if (newScore >= temp)
        {
            levelData.star[numLevel] = newScore;
        }
        AddCoinWhenReplay(newScore, temp);
        SavePlayerLevel();
    }

    public static void AddCoinWhenReplay(int newScore, int oldScore)
    {
        if (newScore - oldScore == 3) AddCoin(30);
        else if (newScore - oldScore == 2) AddCoin(20);
        else if (newScore - oldScore == 1) AddCoin(10);
        else AddCoin(0);
    }

    public static void AddCoinWhenFirstPlay(int newScore)
    {
        if (newScore == 3) AddCoin(30);
        else if (newScore == 2) AddCoin(20);
        else if (newScore == 1) AddCoin(10);
        else AddCoin(0);
    }

    public static int GetDoubleCoin()
    {
        return playerCosmetic.addedCoin;
    }

    #endregion

    #region Setting
    public static void TurnOnSound(bool t)
    {
        playerSetting.sound = t;
        SavePlayerSetting();
    }
    public static void TurnOnVibration(bool t)
    {
        playerSetting.vibrateOn = t;
        SavePlayerSetting();
    }

    public static bool GetSoundState()
    {
        return playerSetting.sound;
    }
    public static bool GetVibrationState()
    {
        return playerSetting.vibrateOn;
    }

    public static bool GetRemoveAds()
    {
        return playerSetting.removeAds;
    }

    public static void SetRemoveAds(bool t)
    {
        playerSetting.removeAds = t;
        SavePlayerSetting();
    }

    #endregion

    //Coin
    #region Coin
    public static void AddCoin(int ammount)
    {
        playerCosmetic.coin += ammount;
        playerCosmetic.addedCoin = ammount;
        SavePlayerCosmetic();
    }

    public static void SpendCoin(int ammount)
    {
        playerCosmetic.coin -= ammount;
        SavePlayerCosmetic();
    }

    public static bool CheckIfEnoughCoin(int ammount)
    {
        if (playerCosmetic.coin >= ammount) 
            return true;
        return false;
    }

    public static int GetCoin()
    {
        return playerCosmetic.coin;
    }

    public static void AddPurchasedSkin(int skinIndex)
    {
        playerCosmetic.skinOwned.Add(skinIndex);
        SavePlayerCosmetic();
    }

    public static List<int> GetAllSkinPurchased()
    {
        return playerCosmetic.skinOwned;
    }

    public static int GetPurchasedSkin(int index)
    {
        return playerCosmetic.skinOwned[index];
    }

    public static int GetSelectedSkinIndex()
    {
        return playerCosmetic.selectedSkinIndex;
    }

    public static Skin GetSelectedSkin()
    {
        return selectedSkin;
    }

    public static void SetSelectedSkin(Skin skin, int index)
    {
        selectedSkin = skin;
        playerCosmetic.selectedSkinIndex = index;
        SavePlayerCosmetic();
    }

    #endregion
    public static void LoadPlayerData()
    {
        levelData = BinarySerializer.Load<LevelData>("LevelData.txt");
        playerSetting = BinarySerializer.Load<PlayerSetting>("PlayerSetting.txt");
        playerCosmetic = BinarySerializer.Load<PlayerCosmetic>("PlayerCosmetics.txt");
    }
    public static void SavePlayerLevel()
    {
        BinarySerializer.Save(levelData, "LevelData.txt");       
    }

    public static void SavePlayerSetting()
    {
        BinarySerializer.Save(playerSetting, "PlayerSetting.txt");
    }
    
    public static void SavePlayerCosmetic()
    {
        BinarySerializer.Save(playerCosmetic, "PlayerCosmetics.txt");
    }
}
