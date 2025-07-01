using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveDataDistributor", menuName = "ScriptableObjects/WaveDataDistributorSO", order = 1)]
public class WaveDataDistributorSO : ScriptableObject
{
    public List<WorldWiseWaveData> WaveDataList;
    public List<int> ChapterWiseGemCount = new List<int>();
    public List<WaveCatalystData> liftItemCatalyst = new List<WaveCatalystData>();
    public List<WaveCatalystData> wheelItemCatalyst = new List<WaveCatalystData>();
    
    [Space]
    public List<PistonPlacementData> pistonPlacement = new List<PistonPlacementData>();
    [Space]
    public List<StonePositionData> stonePositionData = new List<StonePositionData>();
  
    public WorldWiseWaveData GetWaveData(int chapterId)
    {
        return WaveDataList[chapterId];
    }
}

[Serializable]
public struct WorldWiseWaveData
{
    public List<WaveDataSO> waves;
}

[Serializable]
public struct WaveCatalystData
{
    public int firstIndexToInfluence;
    public List<int> indexToMatch;
}

[Serializable]
public struct PistonPlacementData
{
    public int firstGridIndexToOccupy;
    public int secondGridIndexToOccupy;
}

[Serializable]
public struct StonePositionData
{
    public List<int> stoneGridIndices;
}