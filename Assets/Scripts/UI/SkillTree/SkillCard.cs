using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Coffee.UIEffects;
using System.Collections.Generic;
using DG.Tweening;

public class SkillCard : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform, notifyRect;
    [SerializeField] private Image cardBg, skillIconImg;
    [SerializeField] private Sprite[] cardBGSprites;
    [SerializeField] private ParticleSystem notifyParticle;
    [SerializeField] private UIShiny notifyShiny;
    [SerializeField] private Button skillButton;

    private SKILL_TREE_ATTRIBUTE_TYPE skillType;
    private SKILL_TREE_TYPE skillTreeType;
    private int skillIndex, skillLevel, currentSkillNum;
    private bool isUpgraded, isUpgradable;
    private SkillTreeScrollController skillTreeScrollController;

    private const float minYPos = -1485f;

    public static event Action OnSkillButtonPressed;

    public SKILL_TREE_ATTRIBUTE_TYPE SkillType => skillType;
    public int SkillIndex => skillIndex;

    void Start()
    {
        skillButton.onClick.AddListener(SkillButtonPressed);
    }

    public void SetInfo(int skillIndex, int skillLevel, SKILL_TREE_TYPE skillTreeType, SKILL_TREE_ATTRIBUTE_TYPE skillType, Sprite skillIcon, bool isUpgraded, bool isUpgradable, int currentSkillNum, SkillTreeScrollController skillTreeScrollController)
    {
        this.skillTreeScrollController = skillTreeScrollController;
        skillIconImg.sprite = skillIcon;
        this.skillLevel = skillLevel;
        this.skillTreeType = skillTreeType;
        this.skillType = skillType;
        this.isUpgraded = isUpgraded;

        SetUpgradableStatus(skillIndex, currentSkillNum, isUpgradable);
        SetNotifyIcon();
    }

    public void SetUpgradableStatus(int skillIndex, int currentSkillNum, bool isUpgradable)
    {
        this.skillIndex = skillIndex;
        this.currentSkillNum = currentSkillNum;
        this.isUpgradable = isUpgradable;

        cardBg.sprite = cardBGSprites[0];
        cardBg.color = Color.white;
        skillButton.interactable = true;


        if (skillIndex < currentSkillNum)
        {
            cardBg.sprite = cardBGSprites[2];
            skillButton.interactable = false;
        }

        else if (skillIndex == currentSkillNum)
        {
            cardBg.sprite = cardBGSprites[1];
        }

        SetNotifyIcon();
    }

    public void SetUpgradable(int currentSkillNum)
    {
        this.currentSkillNum = currentSkillNum;
        this.isUpgradable = true;

        // AudioManager.CallPlaySFX(Sound.TroopCardActive);

        transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.1f).OnComplete(() =>
        {
            transform.localScale = Vector3.one;
        });

        cardBg.DOFade(0.5f, 0.25f).OnComplete(() =>
        {
            cardBg.sprite = cardBGSprites[1];
            cardBg.DOFade(1, 0.25f);
        });

        SetNotifyIcon();
    }

    public void SetNotifyIcon()
    {
        notifyRect.gameObject.SetActive(this.isUpgradable);

        if (this.isUpgradable)
        {
            notifyParticle.Play();
            notifyShiny.Play();
        }
        else
        {
            notifyParticle.Stop();
            notifyShiny.Stop();
        }
    }

    public void SkillButtonPressed()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);
        transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.1f).OnComplete(() =>
        {
            transform.localScale = Vector3.one;
        });

        if (skillTreeScrollController != null)
        {
            skillTreeScrollController.SetSkillData(skillIndex);
        }
    }

    public void OnSkillUpgrade()
    {
        isUpgraded = true;
        isUpgradable = false;
        notifyShiny.Stop();
        notifyParticle.Stop();
        notifyRect.gameObject.SetActive(false);

        transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.1f).OnComplete(() =>
        {
            transform.localScale = Vector3.one;
        });

        cardBg.DOFade(0, 0.25f).OnComplete(() =>
        {
            cardBg.sprite = cardBGSprites[2];
            cardBg.DOFade(1, 0.25f);
        });
    }
}
