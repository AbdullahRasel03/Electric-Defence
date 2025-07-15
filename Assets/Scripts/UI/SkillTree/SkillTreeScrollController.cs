using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Pool;
using DG.Tweening;

public class SkillTreeScrollController : MonoBehaviour
{
    [SerializeField] private SkillView skillView;

    [SerializeField] private int maxCount;
    [SerializeField] private int visibleCount;

    [Space]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private float topPadding, bottomPadding, spacing;

    [Space]
    [SerializeField] private SkillCard skillCardPrefab;
    [SerializeField] private float cellWidth;
    [SerializeField] private float cellHeight;

    [Space]
    [SerializeField] private SKILL_TREE_TYPE skillTreeType;
    [SerializeField] private SkillTreeDataSO skillTreeData;
    [SerializeField] private Image progressBar;

    [Space]
    [SerializeField] private ParticleSystem upgradeParticle;

    private float maxHeight;
    private float scrollFraction;
    private float currentScrollVal = 0;

    private int currentMax = 0;
    private int currentMin = 0;
    private int currentSkillNum;

    private List<SkillCard> skillCardList;
    private RectTransform selfRect;

    public static event Action<float, float> OnScrollValChanged;

    void Awake()
    {
        selfRect = GetComponent<RectTransform>();
    }

    void Start()
    {
        currentSkillNum = GameManager.GetInstance().GetSkillTreeSavedData().GetSkillTreeProgress(skillTreeType);
        SetView(currentSkillNum);
    }

    public void SetView(int levelId)
    {
        InitParams();
        InitSelectedView(levelId);
        StartCoroutine(LateCallbackAdd());
    }

    public void ScrollToNextCell(Action callBack)
    {
        scrollRect.vertical = false;
        float min = (topPadding + (3 * cellHeight) + (3 * spacing) + (cellHeight * 0.5f)) - maxHeight;
        float target = contentRect.anchoredPosition3D.y - cellHeight - spacing;
        if (target >= min)
        {
            contentRect.DOAnchorPos3DY(target, 0.5f).OnComplete(() =>
            {
                scrollRect.vertical = true;
                callBack?.Invoke();
            });
        }
        else
        {
            callBack?.Invoke();
        }
    }

    public void ScrollToTargetCell(int focusCellIndex, Action callBack = null)
    {
        scrollRect.vertical = false;
        float min = (topPadding + (3 * cellHeight) + (3 * spacing) + (cellHeight * 0.5f)) - maxHeight;
        float target = -bottomPadding - (cellHeight * (focusCellIndex - 1)) - (spacing * (focusCellIndex - 1)) + (cellHeight * 0.5f) + spacing;

        if (target >= min)
        {
            contentRect.DOAnchorPos3DY(target >= 0 ? 0 : target, 0.5f).OnComplete(() =>
            {
                scrollRect.vertical = true;
                skillView.OnScrollEnded();
                callBack?.Invoke();
            });
        }
    }

    public void ResetToCell(int focusCellIndex)
    {
        scrollRect.vertical = false;

        float target = -bottomPadding - (cellHeight * (focusCellIndex - 1)) - (spacing * (focusCellIndex - 1)) + (cellHeight * 0.5f) + spacing;

        if (Mathf.Abs(target - contentRect.anchoredPosition3D.y) < 0.01f)
        {
            // If the target position is very close to the current position, we can skip the movement
            scrollRect.vertical = true;
            return;
        }

        contentRect.DOAnchorPos3DY(target >= 0 ? 0 : target, 0.15f);
    }

    public void OnSkillTreeUpgrade(SkillCard skillCard)
    {
        ResetToCell(skillCard.SkillIndex);
        currentSkillNum = GameManager.GetInstance().GetSkillTreeSavedData().GetSkillTreeProgress(skillTreeType);

        if (skillTreeType == SKILL_TREE_TYPE.WALL)
        {
            WallSavedData wallSavedData = GameManager.GetInstance().GetWallSavedData();

            switch (skillCard.SkillType)
            {
                case SKILL_TREE_ATTRIBUTE_TYPE.WALL_HEALTH:
                    wallSavedData.wallHealth += skillTreeData.skillAttributes[currentSkillNum - 2].skillValue;
                    break;
                case SKILL_TREE_ATTRIBUTE_TYPE.WALL_DAMAGE_REDUCTION:
                    wallSavedData.wallDamageReduction += skillTreeData.skillAttributes[currentSkillNum - 2].skillValue;
                    break;
                case SKILL_TREE_ATTRIBUTE_TYPE.WALL_HEALTH_REGENERATION:
                    wallSavedData.wallHealthRegeneration += skillTreeData.skillAttributes[currentSkillNum - 2].skillValue;
                    break;
            }

            GameManager.GetInstance().UpdateWallSavedData(wallSavedData);
        }

        ShowSkillUpgradeFeedback(skillCard);
    }

    private void ShowSkillUpgradeFeedback(SkillCard skillCard)
    {
        DOVirtual.DelayedCall(0.25f, () =>
        {
            VibrationManager.instance.PlayHapticMedium();
            // AudioManager.CallPlaySFX(Sound.TroopUpgrade);

            upgradeParticle.transform.position = skillCard.transform.position + (Vector3.up * 0.5f);
            upgradeParticle.Play();
            skillCard.OnSkillUpgrade();

            DOVirtual.DelayedCall(upgradeParticle.main.duration, () =>
            {
                SetProgress(currentSkillNum, false);

                ScrollToTargetCell(currentSkillNum, () =>
                {
                    skillCardList[currentSkillNum - 1].SetUpgradable(currentSkillNum);
                    SetSkillData(currentSkillNum);
                });
            });
        });
    }

    // public void Reset()
    // {
    //     currentMax = 0;
    //     currentMin = 0;
    //     progressBar.fillAmount = 0;

    //     foreach (var item in skillCardList)
    //     {
    //         levelPool.Release(item);
    //     }
    //     skillCardList.Clear();
    //     contentRect.anchoredPosition3D = Vector3.zero;
    //     currentScrollVal = 0;
    //     scrollbar.onValueChanged.RemoveAllListeners();
    // }

    private void InitParams()
    {
        skillCardList = new List<SkillCard>();
        maxHeight = (maxCount * cellHeight) + ((maxCount - 1) * spacing) + topPadding + bottomPadding;
        scrollFraction = (1f / maxCount);
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, maxHeight);
        contentRect.anchoredPosition3D = Vector3.zero;

    }

    private void SetProgress(int focusIndex, bool addDirectely = true)
    {
        float target = bottomPadding + (cellHeight * (focusIndex - 1)) + (spacing * (focusIndex - 1)) + (cellHeight * 0.5f);


        if (addDirectely)
        {
            progressBar.fillAmount = target / maxHeight;
        }

        else
        {
            progressBar.DOFillAmount(target / maxHeight, 0.5f);
        }
    }

    private void InitSelectedView(int focusCellIndex)
    {
        currentMin = 0;
        currentMax = skillTreeData.skillAttributes.Count - 1;
        for (int i = currentMin; i < currentMax + 1; ++i)
        {
            SkillCard img = Instantiate(skillCardPrefab, selfRect);
            img.SetInfo(i + 1, skillTreeData.GetAttributeBasedLevelFromIndex(skillTreeData.skillAttributes[i].skillType, i),
                skillTreeType, skillTreeData.skillAttributes[i].skillType, skillView.GetSkillAttributeSprite(skillTreeData.skillAttributes[i].skillType),
                currentSkillNum > (i + 1), currentSkillNum == (i + 1), currentSkillNum, this);
            skillCardList.Add(img);

            RectTransform imgRect = img.GetComponent<RectTransform>();

            imgRect.anchoredPosition3D = Vector3.up * (bottomPadding + (i * cellHeight) + (i * spacing));
        }

        float val = -bottomPadding - (cellHeight * (focusCellIndex - 1)) - (spacing * (focusCellIndex - 1)) + (cellHeight * 0.5f) + spacing;
        contentRect.anchoredPosition3D = new Vector3(0, val, 0);

        SetProgress(focusCellIndex);

        SetSkillData(focusCellIndex);
    }

    // private void OnScrollBarValueChanged(float val)
    // {
    //     OnScrollValChanged?.Invoke(contentRect.transform.localPosition.y, spacing);

    //     if (currentScrollVal > val && Mathf.Abs(currentScrollVal - val) >= scrollFraction && currentMin > 0)
    //     {
    //         //Scrolling DOWN
    //         currentMin--;
    //         SkillCard img = levelPool.Get();
    //         img.SetInfo(currentMin + 1, skillTreeData.GetAttributeBasedLevelFromIndex(skillTreeData.skillAttributes[currentMin].skillType, currentMin),
    //             skillTreeType, skillTreeData.skillAttributes[currentMin].skillType, skillView.GetSkillAttributeSprite(skillTreeData.skillAttributes[currentMin].skillType),
    //             currentSkillNum > (currentMin + 1), currentSkillNum == (currentMin + 1), currentSkillNum, this);
    //         skillCardList.Insert(0, img);
    //         RectTransform imgRect = img.GetComponent<RectTransform>();
    //         imgRect.anchoredPosition3D = Vector3.up * (bottomPadding + (currentMin * cellHeight) + (currentMin * spacing));

    //         if (currentMin < maxCount - visibleCount - 4)
    //         {
    //             int max = skillCardList.Count - 1;
    //             SkillCard img2 = skillCardList[max];
    //             levelPool.Release(img2);
    //             skillCardList.RemoveAt(max);
    //             currentMax--;
    //         }

    //         currentScrollVal -= scrollFraction;
    //     }
    //     else if (currentScrollVal < val && Mathf.Abs(currentScrollVal - val) >= scrollFraction && currentMax < maxCount - 1)
    //     {
    //         //Scrolling UP
    //         currentMax++;
    //         SkillCard img = levelPool.Get();
    //         img.SetInfo(currentMax + 1, skillTreeData.GetAttributeBasedLevelFromIndex(skillTreeData.skillAttributes[currentMax].skillType, currentMax),
    //             skillTreeType, skillTreeData.skillAttributes[currentMax].skillType, skillView.GetSkillAttributeSprite(skillTreeData.skillAttributes[currentMax].skillType),
    //             currentSkillNum > (currentMax + 1), currentSkillNum == (currentMax + 1), currentSkillNum, this);
    //         skillCardList.Add(img);
    //         RectTransform imgRect = img.GetComponent<RectTransform>();
    //         imgRect.anchoredPosition3D = Vector3.up * (bottomPadding + (currentMax * cellHeight) + (currentMax * spacing));

    //         if (currentMax > visibleCount + 4)
    //         {
    //             SkillCard img2 = skillCardList[0];
    //             levelPool.Release(img2);
    //             skillCardList.RemoveAt(0);
    //             currentMin++;
    //         }

    //         currentScrollVal += scrollFraction;
    //     }
    // }

    private IEnumerator LateCallbackAdd()
    {
        yield return new WaitForSeconds(0.2f);
        currentScrollVal = scrollbar.value;
        // scrollbar.onValueChanged.AddListener((float val) => OnScrollBarValueChanged(val));
    }

    internal void SetSkillData(int skillIndex)
    {
        SKILL_TREE_ATTRIBUTE_TYPE skillType = skillTreeData.skillAttributes[skillIndex - 1].skillType;

        float currentAttributeLevel = skillTreeData.GetAttibuteValueFromIndex(skillType, skillTreeType, skillIndex - 1);
        float nextAttributeLevel = skillTreeData.GetAttibuteValueFromIndex(skillType, skillTreeType, skillIndex - 1) + skillTreeData.skillAttributes[skillIndex - 1].skillValue;

        skillView.SetSkillUpgradePanel(skillType, skillTreeType,
            skillTreeData.GetSkillTitle(skillType),
            currentAttributeLevel.ToString(),
            nextAttributeLevel,
            skillTreeData.skillAttributes[skillIndex - 1].tokenCost,
            currentSkillNum == skillIndex,
            skillCardList[skillIndex - 1]);
    }

}
