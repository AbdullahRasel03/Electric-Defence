using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SkillTreeData", menuName = "ScriptableObjects/SkillTreeDataSO")]
public class SkillTreeDataSO : ScriptableObject
{
    public List<SkillTreeData> skillAttributes;

    public int GetAttributeBasedLevelFromIndex(SKILL_TREE_ATTRIBUTE_TYPE skill, int index)
    {
        int count = skillAttributes.Count;
        int level = 1;
        for (int i = 0; i < count; ++i)
        {
            if (i < index && skillAttributes[i].skillType == skill)
            {
                level++;
            }
            if (i == index)
            {
                break;
            }
        }
        return level;
    }

    public float GetAttibuteValueFromIndex(SKILL_TREE_ATTRIBUTE_TYPE skill, SKILL_TREE_TYPE skillType, int index)
    {
        if (skillAttributes == null || skillAttributes.Count <= index)
        {
            return 0f;
        }

        SkillTreeData data = skillAttributes[index];

        if (skillType == SKILL_TREE_TYPE.WALL)
        {
            int indexInHut = index / 3;

            WallSavedData hutSavedData = GameManager.GetInstance().GetWallSavedData();
            SkillTreeSavedData skillTreeSavedData = GameManager.GetInstance().GetSkillTreeSavedData();

            int skillLevel = skillTreeSavedData.GetStillAttributeLevel(skill);

            switch (skill)
            {
                case SKILL_TREE_ATTRIBUTE_TYPE.WALL_HEALTH:
                    return hutSavedData.BASE_WALL_HEALTH + (indexInHut * data.skillValue);
                case SKILL_TREE_ATTRIBUTE_TYPE.WALL_DAMAGE_REDUCTION:
                    return hutSavedData.BASE_WALL_DAMAGE_REDUCTION + (indexInHut * data.skillValue);
                case SKILL_TREE_ATTRIBUTE_TYPE.WALL_HEALTH_REGENERATION:
                    return hutSavedData.BASE_WALL_HEALTH_REGENERATION + (indexInHut * data.skillValue);
            }
        }

        return data.skillValue;
    }

    public string GetSkillTitle(SKILL_TREE_ATTRIBUTE_TYPE skill)
    {
        switch (skill)
        {
            case SKILL_TREE_ATTRIBUTE_TYPE.WALL_HEALTH:
                return "Wall Health";
            case SKILL_TREE_ATTRIBUTE_TYPE.WALL_DAMAGE_REDUCTION:
                return "Wall Damage Reduction";
            case SKILL_TREE_ATTRIBUTE_TYPE.WALL_HEALTH_REGENERATION:
                return "Wall Health Regeneration";
            default:
                return "Unknown Skill";
        }
    }

    [ContextMenu("Fill Data")]
    public void FillData()
    {
        skillAttributes = new List<SkillTreeData>();
        for (int i = 0; i < 40; i++)
        {
            SKILL_TREE_ATTRIBUTE_TYPE skillType = (SKILL_TREE_ATTRIBUTE_TYPE)(i % Enum.GetValues(typeof(SKILL_TREE_ATTRIBUTE_TYPE)).Length);

            float val = 0f;

            if (skillType == SKILL_TREE_ATTRIBUTE_TYPE.WALL_HEALTH)
            {
                val = 1f;
            }

            else if (skillType == SKILL_TREE_ATTRIBUTE_TYPE.WALL_DAMAGE_REDUCTION)
            {
                val = 0.15f;
            }

            else if (skillType == SKILL_TREE_ATTRIBUTE_TYPE.WALL_HEALTH_REGENERATION)
            {
                val = 1f;
            }

            SkillTreeData data = new SkillTreeData
            {
                skillType = skillType,
                skillValue = val,
                tokenCost = 1
            };

            if (skillAttributes == null)
            {
                skillAttributes = new List<SkillTreeData>();
            }

            skillAttributes.Add(data);
        }
    }
}
[Serializable]
public struct SkillTreeData
{
    public SKILL_TREE_ATTRIBUTE_TYPE skillType;
    public float skillValue;
    public int tokenCost;
}