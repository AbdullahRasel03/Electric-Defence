using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SkillView : MiddleViews
{
    [SerializeField] private Sprite wallHealthSprite;
    [SerializeField] private Sprite wallDamageReductionSprite;
    [SerializeField] private Sprite wallHealthRegenSprite;
    [SerializeField] private SkillTreeScrollController wallSkillTree;


    [Space]
    [Header("Skill Upgrade Panel")]
    [SerializeField] private Image decorationPanel;
    [SerializeField] private Image skillIcon;
    [SerializeField] private TMP_Text skillDescriptionText;
    [SerializeField] private TMP_Text currentAttributeText;
    [SerializeField] private TMP_Text nextAttributeText;
    [SerializeField] private TMP_Text skillUpgradeCostText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private GameObject notificationIcon;

    private SkillCard parentRef;
    private int upgradeCost;
    private float updatedAttributeValue;
    private SKILL_TREE_ATTRIBUTE_TYPE skillAttributeType;
    private SKILL_TREE_TYPE skillTreeType;

    public static event Action OnSkillUpgraded;

    protected override void Start()
    {
        base.Start();
        UpdateDecorationPanel();
    }

    private void UpdateDecorationPanel()
    {
        Material mat = decorationPanel.material;
        Vector2 tiling = mat.GetVector("_Tiling");

        if (Statics.IsTab())
        {
            mat.SetVector("_Tiling", new Vector2(4f, tiling.y));
        }

        else
        {
            mat.SetVector("_Tiling", new Vector2(2f, tiling.y));
        }
    }

    public Sprite GetSkillAttributeSprite(SKILL_TREE_ATTRIBUTE_TYPE skill)
    {
        switch (skill)
        {
            case SKILL_TREE_ATTRIBUTE_TYPE.WALL_HEALTH:
                return wallHealthSprite;
            case SKILL_TREE_ATTRIBUTE_TYPE.WALL_DAMAGE_REDUCTION:
                return wallDamageReductionSprite;
            case SKILL_TREE_ATTRIBUTE_TYPE.WALL_HEALTH_REGENERATION:
                return wallHealthRegenSprite;
            default:
                return null;
        }
    }


    public void SetSkillUpgradePanel(SKILL_TREE_ATTRIBUTE_TYPE skillType, SKILL_TREE_TYPE skillTreeType, string skillDescription, string currentAttribute, float nextAttribute, int skillUpgradeCost, bool canShowUpgradeButton, SkillCard parentRef)
    {
        this.parentRef = parentRef;
        this.upgradeCost = skillUpgradeCost;
        this.skillAttributeType = skillType;
        this.skillTreeType = skillTreeType;
        this.updatedAttributeValue = nextAttribute;


        skillIcon.sprite = GetSkillAttributeSprite(skillType);
        skillDescriptionText.text = skillDescription;
        currentAttributeText.text = $"<color=orange>{currentAttribute}";
        nextAttributeText.text = $"<color=green>{nextAttribute}";
        skillUpgradeCostText.text = Statics.FormatNumber(skillUpgradeCost);

        if (canShowUpgradeButton)
        {
            upgradeButton.gameObject.SetActive(true);
            CheckForNotification();
        }
        else
        {
            upgradeButton.gameObject.SetActive(false);
        }

        CheckUpgradeCostText();
    }

    internal void OnScrollEnded()
    {
        upgradeButton.interactable = true;
    }

    private void CheckUpgradeCostText()
    {
        GameManager gameManager = GameManager.GetInstance();

        if (upgradeCost > gameManager.GetCurrentSkillTokenAmount())
        {
            skillUpgradeCostText.color = Color.red;
        }
        else
        {
            skillUpgradeCostText.color = Color.white;
        }
    }

    private void CheckForNotification()
    {
        GameManager gameManager = GameManager.GetInstance();

        if (upgradeCost > gameManager.GetCurrentSkillTokenAmount())
        {
            CancelNotification(receiver);
            notificationIcon.SetActive(false);
        }

        else
        {
            TriggerNotification(receiver);
            notificationIcon.SetActive(true);
        }
    }

    public void UpgradeButtonPressed()
    {
        GameManager gameManager = GameManager.GetInstance();

        if (upgradeCost > gameManager.GetCurrentSkillTokenAmount())
        {
            // AudioManager.CallPlaySFX(Sound.ErrorAlert);
            ToastMessageManager.GetInstance().ShowToastMessage("Not Enough Skill Tokens!", 2f);
            return;
        }

        OnSkillUpgraded?.Invoke();

        CheckUpgradeCostText();

        upgradeButton.interactable = false;

        upgradeButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.1f).OnComplete(() =>
        {
            upgradeButton.transform.localScale = Vector3.one;
        });

        AudioManager.CallPlaySFX(Sound.ButtonClick);

        gameManager.AddSkillTokens(-upgradeCost);

        SkillTreeSavedData skillTreeSavedData = gameManager.GetSkillTreeSavedData();
        skillTreeSavedData.UpdateSkillTreeAttributeLevel(skillAttributeType);
        skillTreeSavedData.UpdateSkillTreeLevel(skillTreeType);
        gameManager.UpdateSkillTreeSavedData(skillTreeSavedData);

        if (skillTreeType == SKILL_TREE_TYPE.WALL)
        {
            wallSkillTree.OnSkillTreeUpgrade(parentRef);
        }
    }
}
