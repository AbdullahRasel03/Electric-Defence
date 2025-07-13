using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCardTroop : DeckCard
{
    [SerializeField] private TMP_Text troopLevelTxt;
    [SerializeField] private TMP_Text fragmentsCostTxt;
    [SerializeField] private Slider fragmentsCostSlider;
    [SerializeField] private Image fragmentIcon;
    [SerializeField] private GameObject fragmentsCostPanel;
    [SerializeField] private Image sliderFillImage;
    [SerializeField] private Sprite progressOnWaySprite;
    [SerializeField] private Sprite progressEndSprite;
    [SerializeField] private UIShiny uiShiny;
    // private TroopDataSO troopDataSO;
    // private TroopDataSO troopToReplace;

    private bool isEquippedPressed = false;

    public static event Action OnTroopEquipped;
    public static event Action<int> OnTroopEquippedVfxCallback;
    public static event Action<int> OnTroopCardClickedAction;

    void Start()
    {
        // TroopDescriptionPopup.OnEquipButtonPressedEvent += OnItemEquipPressed;
        // TroopDescriptionPopup.OnTroopLevelUpEvent += OnTroopUpgrade;
        OnTroopEquipped += OnTroopEquippedCallback;
        OnTroopEquippedVfxCallback += OnTroopEquippedVfxCalled;
        // RewardUIController.OnTroopFragmentsCollected += SetFragmentsCost;
    }

    void OnDestroy()
    {
        // TroopDescriptionPopup.OnEquipButtonPressedEvent -= OnItemEquipPressed;
        // TroopDescriptionPopup.OnTroopLevelUpEvent -= OnTroopUpgrade;
        OnTroopEquipped -= OnTroopEquippedCallback;
        OnTroopEquippedVfxCallback -= OnTroopEquippedVfxCalled;
        // RewardUIController.OnTroopFragmentsCollected -= SetFragmentsCost;
    }

    private void OnTroopUpgrade(int troopId)
    {
        // SetFragmentsCost();
        // if (this.troopDataSO.TroopId != troopId) return;
        // int troopLevel = GameManager.GetInstance().GetTroopLevel(troopId);
        // SetTroopLevelText(troopLevel);
    }

    private void SetTroopLevelText(int level)
    {
        troopLevelTxt.text = $"Lvl {level}";
    }
    // private void OnItemEquipPressed(TroopDataSO troopData)
    // {
    //     isEquippedPressed = true;
    //     if (this.troopDataSO == null || !this.troopDataSO.IsTroopUnlocked()) return;
    //     if (GameManager.GetInstance().GetTroopDeckSlotByTroopId(cardId) == -1) return;
        
    //     this.troopToReplace = troopData;

    //     SetCardShake(true);
    // }

    private void OnTroopEquippedCallback()
    {
        // isEquippedPressed = false;

        // if (troopDataSO == null || !troopDataSO.IsTroopUnlocked()) return;

        // this.troopToReplace = null;

        // SetCardShake(false);

        // if (GameManager.GetInstance().GetTroopDeckSlotByTroopId(cardId) != -1)
        // {
        //     SetCardSelected(true);
        // }

        // else
        // {
        //     SetCardSelected(false);
        // }
    }

    private void OnTroopEquippedVfxCalled(int troopId)
    {
        if (troopId != cardId) return;

        transform.DOPunchScale(Vector2.one * 0.1f, 0.2f, 1, 0.5f);
        // AudioManager.CallPlaySFX(Sound.TroopEquipped);
        equipParticle.Play();
    }

    // public void SetTroopData(TroopDataSO troopDataSO)
    // {
    //     this.troopDataSO = troopDataSO;
    //     fragmentIcon.sprite = troopDataSO.TroopFragmentImg;
    //     SetLockedState(!troopDataSO.IsTroopUnlocked());
    //     SetTroopLevelText(GameManager.GetInstance().GetTroopLevel(troopDataSO.TroopId));
    //     SetFragmentsCost();
    // }

    // protected override void CheckForNotificationOff()
    // {
    //     if (!troopDataSO.IsTroopUnlocked()) return;

    //     PlayerPrefs.SetInt(CARD_NEW_NOTIFICATION_ID + cardId, 1);
    //     PlayerPrefs.Save();

    //     int currentFragments = GameManager.GetInstance().GetCurrentTroopFragmentsAmount(troopDataSO.TroopId);
    //     int requiredFragments = troopDataSO.GetUpgradeCost(GameManager.GetInstance().GetTroopLevel(troopDataSO.TroopId)).fragmentsRequired;

    //     int currentCoins = GameManager.GetInstance().GetCurrentCoinAmount();
    //     int requiredCoins = troopDataSO.GetUpgradeCost(GameManager.GetInstance().GetTroopLevel(troopDataSO.TroopId)).coinsRequired;

    //     if (currentFragments < requiredFragments || currentCoins < requiredCoins)
    //     {
    //         notificationImg.gameObject.SetActive(false);
    //         CancelNotification(receiver);
    //     }
    // }

    // private void SetFragmentsCost()
    // {
    //     int troopLevel = GameManager.GetInstance().GetTroopLevel(troopDataSO.TroopId);

    //     if (!troopDataSO.IsTroopUnlocked() || troopLevel >= Statics.maxTroopLevel)
    //     {
    //         fragmentsCostPanel.gameObject.SetActive(false);
    //         return;
    //     }

    //     CheckForUpgradability(troopLevel);

    //     fragmentsCostPanel.gameObject.SetActive(true);

    //     int currentFragments = GameManager.GetInstance().GetCurrentTroopFragmentsAmount(troopDataSO.TroopId);
    //     int requiredFragments = troopDataSO.GetUpgradeCost(troopLevel).fragmentsRequired;

    //     fragmentsCostSlider.maxValue = requiredFragments;
    //     fragmentsCostSlider.value = currentFragments;

    //     string upgradeCostFragmentsString = "";

    //     if (currentFragments >= requiredFragments)
    //     {
    //         upgradeCostFragmentsString = $" <color=white>{currentFragments}</color> / {requiredFragments}";
    //         sliderFillImage.sprite = progressEndSprite;
         
    //     }
    //     else
    //     {
    //         upgradeCostFragmentsString = $" <color=red>{currentFragments}</color> / {requiredFragments}";
    //         sliderFillImage.sprite = progressOnWaySprite;
    //     }

    //     fragmentsCostTxt.text = upgradeCostFragmentsString;
    // }

    // private void CheckForUpgradability(int troopLevel)
    // {
    //     int currentFragments = GameManager.GetInstance().GetCurrentTroopFragmentsAmount(troopDataSO.TroopId);
    //     int requiredFragments = troopDataSO.GetUpgradeCost(troopLevel).fragmentsRequired;

    //     int currentCoins = GameManager.GetInstance().GetCurrentCoinAmount();
    //     int requiredCoins = troopDataSO.GetUpgradeCost(troopLevel).coinsRequired;

    //     if (currentFragments >= requiredFragments && currentCoins >= requiredCoins)
    //     {
    //         notificationImg.gameObject.SetActive(true);
    //         uiShiny.Play();
    //         TriggerNotification(receiver);
    //     }
    //     else
    //     {
    //         uiShiny.Stop();

    //         if (PlayerPrefs.HasKey(CARD_NEW_NOTIFICATION_ID + cardId))
    //         {
    //             notificationImg.gameObject.SetActive(false);
    //             CancelNotification(receiver);
    //         }
    //     }
    // }

    public override void OnCardClicked()
    {
        // if (!TutorialManager.GetInstance().IsAllCurrentWorldTutFinished(GameManager.GetInstance().GetCurrentWorldId()) && TutorialManager.GetInstance().IsMainMenuTutorialRunning && cardId != 0)
        // {
        //     return;
        // }

        // base.OnCardClicked();
    }

    protected override void OnCardButtonClicked()
    {
        base.OnCardButtonClicked();

        // if (!troopDataSO.IsTroopUnlocked())
        // {
        //     AudioManager.CallPlaySFX(Sound.ErrorAlert);
        //     ToastMessageManager.GetInstance().ShowToastMessage(troopDataSO.GetTroopUnlockString(), 2f);
        //     return;
        // }

        // if (isEquippedPressed)
        // {
        //     if (troopToReplace == null)
        //     {
        //         AudioManager.CallPlaySFX(Sound.ErrorAlert);
        //         ToastMessageManager.GetInstance().ShowToastMessage("Cannot equip an unequipped Troops", 2f);
        //         OnTroopEquipped?.Invoke();
        //         return;
        //     }

        //     int slotIdx = GameManager.GetInstance().GetTroopDeckSlotByTroopId(cardId);

        //     if (slotIdx != -1)
        //     {
        //         OnTroopEquippedVfxCallback?.Invoke(troopToReplace.TroopId);
        //         GameManager.GetInstance().SetTroopDeckSlot(slotIdx, troopToReplace.TroopId);
        //     }

        //     else
        //     {
        //         AudioManager.CallPlaySFX(Sound.ErrorAlert);
        //         ToastMessageManager.GetInstance().ShowToastMessage("Troop already equipped", 2f);
        //     }

        //     OnTroopEquipped?.Invoke();
        // }

        // else
        // {
        //     OnTroopCardClicked();
        // }
    }

    private void OnTroopCardClicked()
    {
        // MainMenuUiController mainMenuUiController = (MainMenuUiController)UiManager.GetInstance().GetUiController();
        // mainMenuUiController.TroopDescriptionPopup.SetTroopDescription(troopDataSO);
        // mainMenuUiController.TroopDescriptionPopup.SetEquippedState(GameManager.GetInstance().GetTroopDeckSlotByTroopId(cardId) != -1);
        // mainMenuUiController.TroopDescriptionPopup.SetView(true);
        
        OnTroopCardClickedAction?.Invoke(cardId);
    }


}
