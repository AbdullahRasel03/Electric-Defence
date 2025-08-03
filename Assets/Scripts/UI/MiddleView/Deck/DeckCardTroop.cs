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
    [SerializeField] private List<UIShiny> uiShiny;
    private TroopDataSO troopDataSO;
    private TroopDataSO troopToReplace;

    private bool isEquippedPressed = false;
    private DeckView deckView;

    public static event Action OnTroopEquipped;
    public static event Action<int> OnTroopEquippedVfxCallback;
    public static event Action<int> OnTroopCardClickedAction;

    void Start()
    {
        TroopDescriptionPopup.OnEquipButtonPressedEvent += OnItemEquipPressed;
        TroopDescriptionPopup.OnTroopLevelUpEvent += OnTroopUpgrade;
        OnTroopEquipped += OnTroopEquippedCallback;
        // OnTroopEquippedVfxCallback += OnTroopEquippedVfxCalled;
        // RewardUIController.OnTroopFragmentsCollected += SetFragmentsCost;
    }

    void OnDestroy()
    {
        TroopDescriptionPopup.OnEquipButtonPressedEvent -= OnItemEquipPressed;
        TroopDescriptionPopup.OnTroopLevelUpEvent -= OnTroopUpgrade;
        OnTroopEquipped -= OnTroopEquippedCallback;
        // OnTroopEquippedVfxCallback -= OnTroopEquippedVfxCalled;
        // RewardUIController.OnTroopFragmentsCollected -= SetFragmentsCost;
    }

    private void OnTroopUpgrade(int troopId)
    {
        // SetFragmentsCost();
        if (this.troopDataSO.TroopId != troopId) return;
        int troopLevel = GameManager.GetInstance().GetHeroLevel(troopId);
        SetTroopLevelText(troopLevel);
        CheckForNotification();
    }

    private void SetTroopLevelText(int level)
    {
        troopLevelTxt.text = $"Lvl {level}";
    }
    private void OnItemEquipPressed(TroopDataSO troopData)
    {
        isEquippedPressed = true;
        if (this.troopDataSO == null || !this.troopDataSO.IsTroopUnlocked()) return;
        if (GameManager.GetInstance().GetHeroDeckSlotByTroopId(cardId) == -1) return;

        this.troopToReplace = troopData;

        SetCardShake(true);
    }

    private void OnTroopEquippedCallback()
    {
        isEquippedPressed = false;

        if (troopDataSO == null || !troopDataSO.IsTroopUnlocked()) return;

        this.troopToReplace = null;

        SetCardShake(false);

        if (GameManager.GetInstance().GetHeroDeckSlotByTroopId(cardId) != -1)
        {
            SetCardSelected(true);
        }

        else
        {
            SetCardSelected(false);
        }
    }


    public void ShowEquippedVfx()
    {
        transform.DOPunchScale(Vector2.one * 0.1f, 0.2f, 1, 0.5f);
        AudioManager.CallPlaySFX(Sound.TroopEquipped);
        equipParticle.Play();
    }

    public void SetTroopData(TroopDataSO troopDataSO, DeckView deckView)
    {
        this.deckView = deckView;
        this.troopDataSO = troopDataSO;
        this.cardId = troopDataSO.TroopId;
        // fragmentIcon.sprite = troopDataSO.TroopFragmentImg;
        cardImg.sprite = troopDataSO.TroopImg;
        SetLockedState(!troopDataSO.IsTroopUnlocked());
        SetTroopLevelText(GameManager.GetInstance().GetHeroLevel(troopDataSO.TroopId));

        MainMenuUiController mainMenuUiController = (MainMenuUiController)UiManager.GetInstance().GetUiController();
        receiver = mainMenuUiController.BottomHudController.DeckSection;

        CheckForNotification();
    }

    protected void CheckForNotification()
    {
        if (!troopDataSO.IsTroopUnlocked()) return;

        PlayerPrefs.SetInt(CARD_NEW_NOTIFICATION_ID + cardId, 1);
        PlayerPrefs.Save();

        int currentFragments = GameManager.GetInstance().GetCurrentHeroFragments(troopDataSO.TroopId);
        int requiredFragments = troopDataSO.GetUpgradeCost(GameManager.GetInstance().GetHeroLevel(troopDataSO.TroopId));


        if (currentFragments < requiredFragments)
        {
            notificationImg.gameObject.SetActive(false);
            CancelNotification(receiver);
            SetUpgradibility(false);
        }

        else
        {
            notificationImg.gameObject.SetActive(true);
            TriggerNotification(receiver);
            SetUpgradibility(true);
        }
    }

    private void SetUpgradibility(bool flag)
    {
        if (flag)
        {
            foreach (UIShiny shiny in uiShiny)
            {
                shiny.Play();
            }
        }

        else
        {
            foreach (UIShiny shiny in uiShiny)
            {
                shiny.Stop();
            }
        }
    }



    public override void OnCardClicked()
    {
        // if (!TutorialManager.GetInstance().IsAllCurrentWorldTutFinished(GameManager.GetInstance().GetCurrentWorldId()) && TutorialManager.GetInstance().IsMainMenuTutorialRunning && cardId != 0)
        // {
        //     return;
        // }

        base.OnCardClicked();
    }

    protected override void OnCardButtonClicked()
    {
        base.OnCardButtonClicked();

        if (!troopDataSO.IsTroopUnlocked())
        {
            AudioManager.CallPlaySFX(Sound.ErrorAlert);
            ToastMessageManager.GetInstance().ShowToastMessage(troopDataSO.GetTroopUnlockString(), 2f);
            return;
        }

        if (isEquippedPressed)
        {
            if (troopToReplace == null)
            {
                AudioManager.CallPlaySFX(Sound.ErrorAlert);
                ToastMessageManager.GetInstance().ShowToastMessage("Cannot equip an unequipped Troops", 2f);
                OnTroopEquipped?.Invoke();
                return;
            }

            int slotIdx = GameManager.GetInstance().GetHeroDeckSlotByTroopId(cardId);

            if (slotIdx != -1)
            {
                // OnTroopEquippedVfxCallback?.Invoke(troopToReplace.TroopId);
                GameManager.GetInstance().SetHeroDeckSlot(slotIdx, troopToReplace.TroopId);
                deckView.OnTropEquipClicked(troopDataSO, troopToReplace);
            }

            else
            {
                AudioManager.CallPlaySFX(Sound.ErrorAlert);
                ToastMessageManager.GetInstance().ShowToastMessage("Troop already equipped", 2f);
            }

            OnTroopEquipped?.Invoke();
        }

        else
        {
            OnTroopCardClicked();
        }
    }

    private void OnTroopCardClicked()
    {
        MainMenuUiController mainMenuUiController = (MainMenuUiController)UiManager.GetInstance().GetUiController();
        mainMenuUiController.TroopDescriptionPopup.SetTroopDescription(troopDataSO);
        mainMenuUiController.TroopDescriptionPopup.SetEquippedState(GameManager.GetInstance().GetHeroDeckSlotByTroopId(cardId) != -1);
        mainMenuUiController.TroopDescriptionPopup.SetView(true);

        OnTroopCardClickedAction?.Invoke(cardId);
    }


}
