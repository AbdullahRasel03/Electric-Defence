using System;
using System.Collections.Generic;
using UnityEngine;
public static class Statics
{
    public static float goldenAspect = 0.5625f;
    public static DateTime resetDateTime = new DateTime(2000, 1, 1, 12, 0, 0);
    public static int freeSpinResetTime = 86400;
    public static int levelWinCoinReward = 20;
    public static int[] worldWiseAreaCount = new int[6] { 4, 4, 6, 6, 6, 6 };
    public static List<int> isPowerupShownChecker = new List<int>();
    public static bool isLevelWinAnimationPending;

    public static int minWorldId = 1;
    public static int maxWorldId = 6;
    public static int maxTroopLevel = 10;

    public static string FormatNumber(long num)
    {
        if (num >= 100000000)
        {
            return (num / 1000000D).ToString("0.#M");
        }
        if (num >= 1000000)
        {
            return (num / 1000000D).ToString("0.##M");
        }
        if (num >= 100000)
        {
            return (num / 1000D).ToString("0.#k");
        }
        if (num >= 10000)
        {
            return (num / 1000D).ToString("0.##k");
        }

        return num.ToString("#,0");
    }

    public static float GetAspectRatio()
    {
        float width = Mathf.Max(Screen.width, Screen.height);
        float height = Mathf.Min(Screen.width, Screen.height);
        return width / height;
    }

    public static float GetWidth()
    {
        return Mathf.Max(Screen.width, Screen.height);
    }

    public static float GetHeight()
    {
        return Mathf.Min(Screen.width, Screen.height);
    }

    public static bool IsTab()
    {
        return GetAspectRatio() <= 1.6f;
    }
}



public enum SCENE_NUM
{
    LOADING_SCENE = 0,
    MAIN_MENU = 1,
    GAME_SCENE = 2
}

public enum SKILL_TREE_ATTRIBUTE_TYPE
{
    WALL_HEALTH,
    WALL_DAMAGE_REDUCTION,
    WALL_HEALTH_REGENERATION
}

public enum SKILL_TREE_TYPE
{
    WALL = 0,
}

// public enum UI_VIEW
// {
//     SHOP = 0,
//     HOME,
//     EQUIPMENT
// }

public enum ECONOMY_TYPE
{
    COIN = 0,
    GEM,
    SKILL_TOKEN,
    TROOP_FRAGMENT
}

public enum FlyObjectType
{
    Coins = 0,
    ExtraPin = 3,
    ExtraCardSlot = 6,
    Hammer = 10
}
