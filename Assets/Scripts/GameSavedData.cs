using System.Collections.Generic;


[System.Serializable]
public class PreferenceData
{
    public bool isBgmOn;
    public bool isSfxOn;
    public bool isVibrationOn;

    public PreferenceData(bool isBgmOn, bool isSfxOn, bool isVibrationOn)
    {
        this.isBgmOn = isBgmOn;
        this.isSfxOn = isSfxOn;
        this.isVibrationOn = isVibrationOn;
    }
}

[System.Serializable]
public class EconomyData
{
    // public int coinCount;
    public Dictionary<ECONOMY_TYPE, int> economyData;
    public Dictionary<int, int> troopFragmentsData = new Dictionary<int, int>();

    public EconomyData(Dictionary<ECONOMY_TYPE, int> economyData)
    {
        this.economyData = economyData;
    }

    public EconomyData()
    {
        // this.coinCount = coinCount;
        this.economyData = new Dictionary<ECONOMY_TYPE, int>();
        this.economyData.Add(ECONOMY_TYPE.COIN, 0);
        this.economyData.Add(ECONOMY_TYPE.GEM, 0);
        this.economyData.Add(ECONOMY_TYPE.SKILL_TOKEN, 0);
    }
}

[System.Serializable]
public class PlayerData
{
    public int currentLevelId;
    public int currentWorldId;
    public int currentAreaId;
    // public Dictionary<PowerupType, int> powerupData;

    public PlayerData(int currentLevelId)
    {
        this.currentLevelId = currentLevelId;
        this.currentWorldId = 1;
        this.currentAreaId = 1;
    }
}

[System.Serializable]
public class SkillTreeSavedData
{
    private Dictionary<SKILL_TREE_TYPE, int> skillTreeProgressData;
    private Dictionary<SKILL_TREE_ATTRIBUTE_TYPE, int> skillTreeAttributeData;

    public SkillTreeSavedData()
    {
        skillTreeProgressData = new Dictionary<SKILL_TREE_TYPE, int>();
        skillTreeAttributeData = new Dictionary<SKILL_TREE_ATTRIBUTE_TYPE, int>();
    }

    public int GetStillAttributeLevel(SKILL_TREE_ATTRIBUTE_TYPE type)
    {
        if (skillTreeAttributeData.ContainsKey(type))
        {
            return skillTreeAttributeData[type];
        }
        return 1;
    }

    public void UpdateSkillTreeAttributeLevel(SKILL_TREE_ATTRIBUTE_TYPE skill)
    {
        if (!skillTreeAttributeData.ContainsKey(skill))
        {
            skillTreeAttributeData.Add(skill, 2);
        }
        else
        {
            skillTreeAttributeData[skill]++;
        }
    }

    public int GetSkillTreeProgress(SKILL_TREE_TYPE skillTreeType)
    {
        if (skillTreeProgressData.ContainsKey(skillTreeType))
        {
            return skillTreeProgressData[skillTreeType];
        }
        return 1;
    }

    public void UpdateSkillTreeLevel(SKILL_TREE_TYPE skill)
    {
        if (!skillTreeProgressData.ContainsKey(skill))
        {
            skillTreeProgressData.Add(skill, 2);
        }
        else
        {
            skillTreeProgressData[skill]++;
        }
    }
}

[System.Serializable]
public class WallSavedData
{
    public readonly float BASE_WALL_HEALTH = 10;
    public readonly float BASE_WALL_DAMAGE_REDUCTION = 0;
    public readonly float BASE_WALL_HEALTH_REGENERATION = 0;

    public float wallHealth;
    public float wallDamageReduction;
    public float wallHealthRegeneration;

    public WallSavedData(float hutHealth, float hutDamageReduction, float hutHealthRegeneration)
    {
        this.wallHealth = hutHealth;
        this.wallDamageReduction = hutDamageReduction;
        this.wallHealthRegeneration = hutHealthRegeneration;
    }

    public WallSavedData()
    {
        this.wallHealth = BASE_WALL_HEALTH;
        this.wallDamageReduction = BASE_WALL_DAMAGE_REDUCTION;
        this.wallHealthRegeneration = BASE_WALL_HEALTH_REGENERATION;
    }
}


[System.Serializable]
public class GameMetaData
{
    public System.DateTime freeLuckySpinTime;
    //public System.DateTime freeCoinCollectionTime;
   
    public GameMetaData()
    {
        freeLuckySpinTime = Statics.resetDateTime;
        //freeCoinCollectionTime = Statics.resetDateTime;
    }
}
