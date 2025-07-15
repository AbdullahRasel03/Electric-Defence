using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public int InitialCoinAmount = 0;
    public int InitialGemAmount = 0;
    public int InitialSkillTokenAmount = 0;

    [SerializeReference] private MobileNotificationController mobileNotificationController;
    [SerializeField] private List<TroopDataSO> allHeroData; 

    private PreferenceData preferenceData;
    private EconomyData economyData;
    private PlayerData playerData;
    private SkillTreeSavedData skillTreeSavedData;
    private GameMetaData gameMetaData;
    private WallSavedData wallSavedData;

    public bool isDebug = true;

    public List<TroopDataSO> AllHeroData => allHeroData;

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

        CheckAutoEquipHeroes();


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
    
    public void AddHeroFragments(int troopId, int amount)
    {
        if (economyData.troopFragmentsData.ContainsKey(troopId))
        {
            economyData.troopFragmentsData[troopId] += amount;
        }
        else
        {
            economyData.troopFragmentsData[troopId] = amount;
        }

        SaveLoadManager.SaveEconomyData(economyData);
    }

    public int GetCurrentHeroFragments(int troopId)
    {
        if (economyData.troopFragmentsData.ContainsKey(troopId))
        {
            return economyData.troopFragmentsData[troopId];
        }
        return 0; // Default value if troopId not found
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

    #region Heros

    public TroopDataSO GetHeroData(int troopId)
    {
        return allHeroData.Find(t => t.TroopId == troopId);
    }

    public int GetHeroIdFromType(TROOP_TYPE troopType)
    {
        TroopDataSO troopData = allHeroData.Find(t => t.TroopType == troopType);

        if (troopData != null)
        {
            return troopData.TroopId;
        }
        return -1; // Troop not found
    }

    public List<TroopDataSO> GetUnlockedHeroes()
    {
        return allHeroData.Where(t => t.IsTroopUnlocked()).ToList();
    }

    public List<TroopDataSO> GetHeroesForTutorialLevel()
    {
        List<TroopDataSO> unlockedTroops = GetUnlockedHeroes();

        unlockedTroops.Add(allHeroData.Find(t => t.TroopId == 0)); // Always add the first troop for tutorial

        return unlockedTroops;
    }

    public List<TroopDataSO> GetRandomUnlockedHeroes(int count)
    {
        List<TroopDataSO> unlockedTroops = GetUnlockedHeroes();
        List<TroopDataSO> randomTroops = new List<TroopDataSO>();
        System.Random random = new System.Random();

        if (unlockedTroops.Count >= count)
        {
            // Shuffle the unlocked troops list and take the first 'count' troops
            for (int i = 0; i < unlockedTroops.Count; i++)
            {
                TroopDataSO temp = unlockedTroops[i];
                int randomIndex = random.Next(i, unlockedTroops.Count);
                unlockedTroops[i] = unlockedTroops[randomIndex];
                unlockedTroops[randomIndex] = temp;
            }

            randomTroops = unlockedTroops.Take(count).ToList();
        }

        else
        {
            // Randomly add count number of troops (with possible duplicates)
            for (int i = 0; i < count; i++)
            {
                TroopDataSO randomTroop = unlockedTroops[Random.Range(0, unlockedTroops.Count)];
                randomTroops.Add(randomTroop);
            }
        }

        return randomTroops;
    }

    public int GetHeroLevel(int troopId)
    {
        if (playerData.troopLevelData.ContainsKey(troopId))
        {
            return playerData.troopLevelData[troopId];
        }

        return 1; // Default level if not found
    }

    public void SetHeroLevel(int troopId, int level)
    {
        if (playerData.troopLevelData.ContainsKey(troopId))
        {
            playerData.troopLevelData[troopId] = level;
        }
        else
        {
            playerData.troopLevelData.Add(troopId, level);
        }

        SaveLoadManager.SavePlayerData(playerData);
    }

    public int GetHeroDeckSlotByTroopId(int troopId)
    {
        if (!playerData.troopDeckSlots.Contains(troopId))
        {
            return -1; // Troop not found in deck
        }

        for (int i = 0; i < Statics.maxTroopDeckSlots; i++)
        {
            if (playerData.troopDeckSlots[i] == troopId)
            {
                return i; // Return the slot index where the troop is found
            }
        }

        return -1; // Troop not found in any slot
    }

    public int GetEmptyHeroDeckSlot()
    {
        for (int i = 0; i < Statics.maxTroopDeckSlots; i++)
        {
            if (playerData.troopDeckSlots[i] == -1)
            {
                return i;
            }
        }
        return -1; // No empty slot found
    }

    public void SetHeroDeckSlot(int slotIndex, int troopId)
    {
        if (slotIndex < 0 || slotIndex >= Statics.maxTroopDeckSlots)
        {
            Debug.LogError("Invalid troop deck slot index: " + slotIndex);
            return;
        }
        playerData.troopDeckSlots[slotIndex] = troopId;
        SaveLoadManager.SavePlayerData(playerData);
    }

    public List<TroopDataSO> GetEquippedHeroes()
    {
        List<TroopDataSO> equippedTroops = new List<TroopDataSO>();

        for (int i = 0; i < Statics.maxTroopDeckSlots; i++)
        {
            if (playerData.troopDeckSlots[i] != -1)
            {
                equippedTroops.Add(allHeroData.Where(t => t.TroopId == playerData.troopDeckSlots[i]).FirstOrDefault());
            }

            else
            {
                equippedTroops.Add(null);
            }
        }
        return equippedTroops;
    }

    public void CheckAutoEquipHeroes()
    {
        for (int i = 0; i < Statics.maxTroopDeckSlots; i++)
        {
            if (playerData.troopDeckSlots[i] == -1)
            {
                foreach (TroopDataSO troopData in allHeroData)
                {
                    if (troopData.IsTroopUnlocked() && !playerData.troopDeckSlots.Contains(troopData.TroopId))
                    {
                        SetHeroDeckSlot(i, troopData.TroopId);
                        break;
                    }
                }
            }
        }
    }
    #endregion

    public void ScheduleNotification(string title, string message, int hour, int minute, int second)
    {
        // mobileNotificationController.ScheduleNotification(title, message, hour, minute, second);
    }
}
