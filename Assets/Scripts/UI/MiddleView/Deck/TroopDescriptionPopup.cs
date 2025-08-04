using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class TroopDescriptionPopup : PopupBase
{
    [SerializeField] private TMP_Text troopNameTxt;
    [SerializeField] private TMP_Text troopDescriptionTxt;
    [SerializeField] private TMP_Text unitTypeText;
    [SerializeField] private TMP_Text currencyTextFragments;
    [SerializeField] private TMP_Text currencyTextCoins;
    [SerializeField] private TMP_Text lockText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image troopImgBG;
    [SerializeField] private Image troopImg;
    [SerializeField] private Image unitTypeImg;
    [SerializeField] private Image upgradeCurrencyImg;
    [SerializeField] private Image sliderFillImage;
    [SerializeField] private Image upgradeNotificationImg;
    [SerializeField] private Sprite progressOnWaySprite;
    [SerializeField] private Sprite progressEndSprite;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button upgaradeButton;
    [SerializeField] private GameObject maxLevelHolder;
    [SerializeField] private GameObject buttonsHolder;
    [SerializeField] private GameObject lockedHolder;
    [SerializeField] private GameObject fragmentsHolder;
    [SerializeField] private Slider fragmentsSlider;
    [SerializeField] private ParticleSystem upgradeParticles;

    [Space]
    [SerializeField] private Sprite[] unitTypeSprites = new Sprite[3];

    [Space]
    [Header("Troop Stats")]
    [SerializeField] private TMP_Text currentDamageText;
    [SerializeField] private TMP_Text nextDamageText;
    [SerializeField] private TMP_Text currentHealthText;
    [SerializeField] private TMP_Text nextHealthText;
    [SerializeField] private TMP_Text currentAttackDelayText;
    [SerializeField] private TMP_Text nextAttackDelayText;
    [SerializeField] private List<GameObject> nextStatArrows;


    private TroopDataSO troopDataSO;
    private int troopLevel;
    private bool isUnlocked = false;

    public static event Action<TroopDataSO> OnEquipButtonPressedEvent;
    public static event Action<int> OnTroopLevelUpEvent;


    public void SetTroopDescription(TroopDataSO troopDataSO)
    {
        this.troopDataSO = troopDataSO;

        SetLockedState(!troopDataSO.IsTroopUnlocked());

        troopNameTxt.text = troopDataSO.TroopName;
        troopDescriptionTxt.text = troopDataSO.TroopDescription;
        troopImg.sprite = troopDataSO.TroopImg;
        // unitTypeText.text = troopDataSO.GetUnitTypeString();

        // unitTypeImg.sprite = unitTypeSprites[(int)troopDataSO.UnitType];

        troopLevel = GameManager.GetInstance().GetHeroLevel(troopDataSO.TroopId);

        SetStats(troopDataSO, troopLevel);
        SetUpgradeCurrency(troopDataSO, troopLevel);
    }

    private void SetStats(TroopDataSO troopDataSO, int level)
    {
        currentDamageText.text = troopDataSO.GetDamage(level).ToString("F1");
        currentHealthText.text = troopDataSO.GetMaxHealth(level).ToString("F1");
        // currentSpeedText.text = troopDataSO.GetMoveSpeed(level).ToString("F1");
        currentAttackDelayText.text = troopDataSO.GetAttackDelay(level).ToString("F1");

        if (!isUnlocked)
        {
            levelText.text = $"Level {level}";
            maxLevelHolder.SetActive(false);
            nextDamageText.gameObject.SetActive(false);
            nextHealthText.gameObject.SetActive(false);
            nextAttackDelayText.gameObject.SetActive(false);
            fragmentsHolder.SetActive(false);

            foreach (GameObject arrow in nextStatArrows)
            {
                arrow.SetActive(false);
            }

            return;
        }


        if (level >= Statics.maxTroopLevel)
        {
            levelText.text = "Max Level";
            maxLevelHolder.SetActive(true);
            upgaradeButton.gameObject.SetActive(false);
            nextDamageText.gameObject.SetActive(false);
            nextHealthText.gameObject.SetActive(false);
            nextAttackDelayText.gameObject.SetActive(false);
            fragmentsHolder.SetActive(false);

            foreach (GameObject arrow in nextStatArrows)
            {
                arrow.SetActive(false);
            }
        }
        else
        {
            levelText.text = $"Level {level}";
            maxLevelHolder.SetActive(false);
            upgaradeButton.gameObject.SetActive(true);
            fragmentsHolder.SetActive(true);

            foreach (GameObject arrow in nextStatArrows)
            {
                arrow.SetActive(true);
            }

            nextDamageText.text = troopDataSO.GetDamage(level + 1).ToString("F1");
            nextHealthText.text = troopDataSO.GetMaxHealth(level + 1).ToString("F1");
            // nextSpeedText.text = troopDataSO.GetMoveSpeed(level + 1).ToString("F1");
            nextAttackDelayText.text = troopDataSO.GetAttackDelay(level + 1).ToString("F1");
        }
    }

    private void SetUpgradeCurrency(TroopDataSO troopDataSO, int level)
    {
        int upgradeCost = troopDataSO.GetUpgradeCost(level);

        int currentFragments = GameManager.GetInstance().GetCurrentHeroFragments(troopDataSO.TroopId);

        fragmentsSlider.maxValue = upgradeCost;

        DOTween.To(() => fragmentsSlider.value, x => fragmentsSlider.value = x, currentFragments, 0.25f)
            .SetEase(Ease.OutQuint);

        upgradeCurrencyImg.sprite = troopDataSO.TroopFragmentImg;

        string upgradeCostFragmentsString = "";

        if (currentFragments >= upgradeCost)
        {
            upgradeCostFragmentsString = $" <color=white>{currentFragments}</color> / {upgradeCost}";
            sliderFillImage.sprite = progressEndSprite;
        }
        else
        {
            upgradeCostFragmentsString = $" <color=red>{currentFragments}</color> / {upgradeCost}";
            sliderFillImage.sprite = progressOnWaySprite;
        }

        currencyTextFragments.text = upgradeCostFragmentsString;



        if (currentFragments >= upgradeCost)
        {
            upgradeNotificationImg.gameObject.SetActive(true);
        }
        else
        {
            upgradeNotificationImg.gameObject.SetActive(false);
        }
    }

    public void SetEquippedState(bool isEquipped)
    {
        equipButton.gameObject.SetActive(!isEquipped);
    }

    internal void SetLockedState(bool isLocked)
    {
        isUnlocked = !isLocked;
        lockedHolder.SetActive(isLocked);
        buttonsHolder.SetActive(!isLocked);

        if (isLocked)
        {
            troopNameTxt.text = "???";
            troopDescriptionTxt.text = "???";
            lockText.text = troopDataSO.GetTroopUnlockString();
            fragmentsHolder.SetActive(false);
        }
    }

    public void OnCloseButtonPressed()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);
        SetView(false);
    }

    public void OnEquipButtonPressed()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);
        OnEquipButtonPressedEvent?.Invoke(troopDataSO);
        SetView(false);
    }

    public void OnUpgradeButtonPressed()
    {
        int upgradeCost = troopDataSO.GetUpgradeCost(troopLevel);
        int currentFragments = GameManager.GetInstance().GetCurrentHeroFragments(troopDataSO.TroopId);

        if (currentFragments >= upgradeCost)
        {
            ShowUpgradeFeedbacks();
            GameManager.GetInstance().SetHeroLevel(troopDataSO.TroopId, troopLevel + 1);
            troopLevel++;
            GameManager.GetInstance().AddHeroFragments(troopDataSO.TroopId, -upgradeCost);
            SetStats(troopDataSO, troopLevel);
            SetUpgradeCurrency(troopDataSO, troopLevel);
            OnTroopLevelUpEvent?.Invoke(troopDataSO.TroopId);
            StartCoroutine(UpgradeCooldown());
        }
        else
        {
            AudioManager.CallPlaySFX(Sound.ButtonClick);
            SetUpgradeCurrency(troopDataSO, troopLevel);
            AudioManager.CallPlaySFX(Sound.ErrorAlert);
            ToastMessageManager.GetInstance().ShowToastMessage($"You need {upgradeCost - currentFragments} more {troopDataSO.TroopName} fragments!", 2f);
            return;
        }
    }

    private IEnumerator UpgradeCooldown()
    {
        upgaradeButton.interactable = false;
        yield return new WaitForSeconds(0.55f);
        upgaradeButton.interactable = true;
    }

    private void ShowUpgradeFeedbacks()
    {
        AudioManager.CallPlaySFX(Sound.TroopUpgrade);

        troopImgBG.transform.DOPunchScale(Vector2.one * 0.1f, 0.5f, 1, 0.5f);

        fragmentsHolder.transform.DOPunchScale(Vector2.one * 0.15f, 0.5f, 1, 0.5f);

        currentDamageText.transform.DOPunchScale(Vector2.one * 0.25f, 0.5f, 1, 0.5f);
        currentHealthText.transform.DOPunchScale(Vector2.one * 0.25f, 0.5f, 1, 0.5f);
        currentAttackDelayText.transform.DOPunchScale(Vector2.one * 0.25f, 0.5f, 1, 0.5f);

        upgradeParticles.Play();


    }
}