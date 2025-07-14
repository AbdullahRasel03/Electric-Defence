using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TroopData", menuName = "ScriptableObjects/TroopDataSO", order = 1)]
public class TroopDataSO : ScriptableObject
{
    [SerializeField] private int _troopId;
    [SerializeField] private int _unlockWorldId;
    [SerializeField] private int _unlockAreaId;
    [SerializeField] private TROOP_TYPE _troopType;
    [SerializeField] private string _troopName;
    [SerializeField] private string _troopDescription;
    [SerializeField] private Sprite _troopImg;
    [SerializeField] private Sprite _troopFragmentImg;

    [Space]
    [Header("Upgradable Troop Stats")]
    [SerializeField] private List<float> _maxHealth;
    [SerializeField] private List<float> _damage;
    [SerializeField] private float _criticalDamage;
    [SerializeField] private List<float> _attackDelay;
    [SerializeField] private List<int> _upgradeCosts = new List<int>();


    public TROOP_TYPE TroopType => _troopType;
    public int TroopId => _troopId;
    public string TroopName => _troopName;
    public string TroopDescription => _troopDescription;
    public int UnlockWorldId => _unlockWorldId;
    public int UnlockAreaId => _unlockAreaId;
    public Sprite TroopImg => _troopImg;
    public Sprite TroopFragmentImg => _troopFragmentImg;
    public float CriticalDamage => _criticalDamage;



    public float GetMaxHealth(int level)
    {
        return _maxHealth[level - 1];
    }

    public float GetDamage(int level)
    {
        return _damage[level - 1];
    }

    public float GetAttackDelay(int level)
    {
        return _attackDelay[level - 1];
    }

    public bool IsTroopUnlocked()
    {
        int currentWorldId = GameManager.GetInstance().GetPlayerData().currentWorldId;
        int currentAreaId = GameManager.GetInstance().GetPlayerData().currentAreaId;

        return currentWorldId > _unlockWorldId || (currentWorldId == _unlockWorldId && currentAreaId >= _unlockAreaId);
    }


    public string GetTroopUnlockString()
    {
        return $"Unlocks at World {_unlockWorldId}, Chapter {_unlockAreaId}";
    }

    public int GetUpgradeCost(int level)
    {
        if (level < 0 || level > _upgradeCosts.Count)
        {
            Debug.LogError($"Invalid level {level} for troop {_troopName}");
            return 0;
        }
        return _upgradeCosts[level - 1];
    }
}


public enum TROOP_TYPE
{
    None = -1,
    Fire = 0,
    Ice = 1,
    Arrow = 2,
    Thunder = 3,
    Sword = 4,
    Turret = 5,
    Crossbow = 6,
    Shotgun = 7,
}


