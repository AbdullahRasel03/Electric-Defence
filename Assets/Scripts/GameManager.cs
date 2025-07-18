using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public int InitialCoinAmount = 0;
    public int InitialGemAmount = 0;
    public int InitialSkillTokenAmount = 0;

    [SerializeReference] private MobileNotificationController mobileNotificationController;

    private PreferenceData preferenceData;
    private EconomyData economyData;
    private PlayerData playerData;
    private SkillTreeSavedData skillTreeSavedData;
    private GameMetaData gameMetaData;
    private WallSavedData wallSavedData;

    public bool isDebug = true;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Application.targetFrameRate = 60;

        InitializeSavedData();
    }

    private void Start()
    {
        OnPreferenceDataLoaded();
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    private void InitializeSavedData()
    {
        //Load preference Data
        preferenceData = SaveLoadManager.LoadPreferenceData();
        if (preferenceData == null)
        {
            //Load data is NULL, initialize with Default data and save
            preferenceData = new PreferenceData(true, true, true);
            SaveLoadManager.SavePreference(preferenceData);
        }


        //Load Economy Data
        economyData = SaveLoadManager.LoadEconomyDataData();
        if (economyData == null)
        {
            Dictionary<ECONOMY_TYPE, int> initialEconomyData;
            if (isDebug)
            {
                initialEconomyData = new Dictionary<ECONOMY_TYPE, int>
                {
                    { ECONOMY_TYPE.COIN, InitialCoinAmount },
                    { ECONOMY_TYPE.GEM, InitialGemAmount },
                    { ECONOMY_TYPE.SKILL_TOKEN, InitialSkillTokenAmount }
                };
            }
            else
            {
                initialEconomyData = new Dictionary<ECONOMY_TYPE, int>
                {
                    { ECONOMY_TYPE.COIN, 200 },
                    { ECONOMY_TYPE.GEM, 0 },
                    { ECONOMY_TYPE.SKILL_TOKEN, 0 }
                };
            }

            economyData = new EconomyData(initialEconomyData);
            SaveLoadManager.SaveEconomyData(economyData);
        }

        //Load Player Data
        playerData = SaveLoadManager.LoadPlayerDataData();
        if (playerData == null)
        {
            playerData = new PlayerData(1);

            playerData = new PlayerData(1);
            SaveLoadManager.SavePlayerData(playerData);
        }

        //Load Meta Data
        gameMetaData = SaveLoadManager.LoadGameMetaData();
        if (gameMetaData == null)
        {
            gameMetaData = new GameMetaData();
            SaveLoadManager.SaveGameMetaData(gameMetaData);
        }

        skillTreeSavedData = SaveLoadManager.LoadSkillTreeData();
        if (skillTreeSavedData == null)
        {
            skillTreeSavedData = new SkillTreeSavedData();
            SaveLoadManager.SaveSkillTreeData(skillTreeSavedData);
        }

        //Load Hut Data
        wallSavedData = SaveLoadManager.LoadWallSavedData();
        if (wallSavedData == null)
        {
            wallSavedData = new WallSavedData();
            SaveLoadManager.SaveWallSavedData(wallSavedData);
        }


    }

    #region Preference Data

    public PreferenceData GetPreferenceData()
    {
        return preferenceData;
    }

    public void UpdatePreferenceData(PreferenceData pData)
    {
        preferenceData = pData;
        SaveLoadManager.SavePreference(preferenceData);
    }

    private void OnPreferenceDataLoaded()
    {
        bool isBgmOn = preferenceData.isBgmOn;
        bool isSfxOn = preferenceData.isSfxOn;
        bool isVibrationOn = preferenceData.isVibrationOn;

        AudioManager.GetInstance().SetBGMPermissionValue(isBgmOn);
        AudioManager.GetInstance().SetSFXPermissionValue(isSfxOn);
        VibrationManager.instance.SetVibration(isVibrationOn);
        AudioManager.GetInstance().Init();
    }

    #endregion


    #region Economy Data
    public int GetCurrentCoinAmount()
    {
        return economyData.economyData[ECONOMY_TYPE.COIN];
    }


    public int GetCurrentSkillTokenAmount()
    {
        return economyData.economyData[ECONOMY_TYPE.SKILL_TOKEN];
    }

    public void AddCoins(int amount)
    {
        economyData.economyData[ECONOMY_TYPE.COIN] += amount;

        SaveLoadManager.SaveEconomyData(economyData);
    }

    public void AddSkillTokens(int amount)
    {
        economyData.economyData[ECONOMY_TYPE.SKILL_TOKEN] += amount;
        SaveLoadManager.SaveEconomyData(economyData);
        UiManager.GetInstance().UpdateSkillTokenInUI();
    }


    public EconomyData GetEconomyData()
    {
        return economyData;
    }

    public void SaveEconomyData()
    {
        SaveLoadManager.SaveEconomyData(economyData);
    }
    #endregion


    #region Player Data
    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public void UpdateCurrentLevel(int levelId)
    {
        playerData.currentLevelId = levelId;
        SaveLoadManager.SavePlayerData(playerData);
    }

    #endregion

    #region SKILL_TREE

    public SkillTreeSavedData GetSkillTreeSavedData()
    {
        return skillTreeSavedData;
    }

    public void UpdateSkillTreeSavedData(SkillTreeSavedData data)
    {
        skillTreeSavedData = data;
        SaveLoadManager.SaveSkillTreeData(skillTreeSavedData);
    }
    #endregion

    #region GameMetaData

    public GameMetaData GetGameMetaData()
    {
        return gameMetaData;
    }

    public void UpdateGameMetadata(GameMetaData metaData)
    {
        gameMetaData = metaData;
        SaveLoadManager.SaveGameMetaData(gameMetaData);
    }

    #endregion

    #region Wall Data
    public WallSavedData GetWallSavedData()
    {
        return wallSavedData;
    }
    public void UpdateWallSavedData(WallSavedData data)
    {
        wallSavedData = data;
        SaveLoadManager.SaveWallSavedData(wallSavedData);
    }

    #endregion

    public void ScheduleNotification(string title, string message, int hour, int minute, int second)
    {
        // mobileNotificationController.ScheduleNotification(title, message, hour, minute, second);
    }
}
