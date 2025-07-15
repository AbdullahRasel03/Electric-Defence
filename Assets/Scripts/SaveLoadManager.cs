using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager
{
    private const string preferenceDatafileName = "/preference.dat";
    private const string economyDatafileName = "/economy.dat";
    private const string playerDatafileName = "/playerData.dat";
    private const string skillTreefileName = "/skillTree.dat";
    private const string gameMetaDatafileName = "/gameMetaData.dat";
    private const string wallSavedDatafileName = "/wallSavedData.dat";

    #region PREFERENCE_DATA
    public static void SavePreference(PreferenceData data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + preferenceDatafileName, FileMode.Create);
        bf.Serialize(stream, data);
        stream.Close();
    }

    public static PreferenceData LoadPreferenceData()
    {
        if (File.Exists(Application.persistentDataPath + preferenceDatafileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + preferenceDatafileName, FileMode.Open);
            PreferenceData data = bf.Deserialize(stream) as PreferenceData;
            stream.Close();
            return data;
        }
        else
        {
            BbsLog.LogError("Saved PreferenceData Not Found! Returning NULL!!");
            return null;
        }

    }
    #endregion


    #region ECONOMY_DATA
    public static void SaveEconomyData(EconomyData data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + economyDatafileName, FileMode.Create);
        bf.Serialize(stream, data);
        stream.Close();
    }

    public static EconomyData LoadEconomyDataData()
    {
        if (File.Exists(Application.persistentDataPath + economyDatafileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + economyDatafileName, FileMode.Open);
            EconomyData data = bf.Deserialize(stream) as EconomyData;
            stream.Close();
            return data;
        }
        else
        {
            BbsLog.LogError("Saved EconomyData Not Found! Returning NULL!!");
            return null;
        }

    }
    #endregion

    #region ECONOMY_DATA
    public static void SavePlayerData(PlayerData data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + playerDatafileName, FileMode.Create);
        bf.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayerDataData()
    {
        if (File.Exists(Application.persistentDataPath + playerDatafileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + playerDatafileName, FileMode.Open);
            PlayerData data = bf.Deserialize(stream) as PlayerData;
            stream.Close();
            return data;
        }
        else
        {
            BbsLog.LogError("Saved PlayerData Not Found! Returning NULL!!");
            return null;
        }

    }
    #endregion

    #region META_DATA
    public static void SaveGameMetaData(GameMetaData data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + gameMetaDatafileName, FileMode.Create);
        bf.Serialize(stream, data);
        stream.Close();
    }

    public static GameMetaData LoadGameMetaData()
    {
        if (File.Exists(Application.persistentDataPath + gameMetaDatafileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + gameMetaDatafileName, FileMode.Open);
            GameMetaData data = bf.Deserialize(stream) as GameMetaData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Saved GameMetaData Not Found! Returning NULL!!");
            return null;
        }

    }
    #endregion


    #region SKILL_TREE_DATA
    public static void SaveSkillTreeData(SkillTreeSavedData data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + skillTreefileName, FileMode.Create);
        bf.Serialize(stream, data);
        stream.Close();
    }

    public static SkillTreeSavedData LoadSkillTreeData()
    {
        if (File.Exists(Application.persistentDataPath + skillTreefileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + skillTreefileName, FileMode.Open);
            SkillTreeSavedData data = bf.Deserialize(stream) as SkillTreeSavedData;
            stream.Close();
            return data;
        }
        else
        {
            return null;
        }

    }
    #endregion

    #region HUT_SAVED_DATA
    public static void SaveWallSavedData(WallSavedData data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + wallSavedDatafileName, FileMode.Create);
        bf.Serialize(stream, data);
        stream.Close();
    }

    public static WallSavedData LoadWallSavedData()
    {
        if (File.Exists(Application.persistentDataPath + wallSavedDatafileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + wallSavedDatafileName, FileMode.Open);
            WallSavedData data = bf.Deserialize(stream) as WallSavedData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Saved Hut Data Not Found! Returning NULL!!");
            return null;
        }

    }
    #endregion
}