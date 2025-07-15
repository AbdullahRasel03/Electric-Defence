using System;
using UnityEngine;
using DG.Tweening;

public class DeckCardPowerups : DeckCard
{
    // private PowerupsDataSO powerupsDataSO;

    // private bool isEquippedPressed = false;


    // public static event Action OnPowerupEquipped;
    // public static event Action<int> OnPowerupEquippedVfxCallback;

    // private PowerupsDataSO powerupToReplace;

    // void Start()
    // {
    //     PowerupsDescriptionPopup.OnEquipButtonPressedEvent += OnItemEquipPressed;
    //     OnPowerupEquipped += OnPowerupEquippedCallback;
    //     OnPowerupEquippedVfxCallback += OnPowerupVfxCalled;
    // }

    // void OnDestroy()
    // {
    //     PowerupsDescriptionPopup.OnEquipButtonPressedEvent -= OnItemEquipPressed;
    //     OnPowerupEquipped -= OnPowerupEquippedCallback;
    //     OnPowerupEquippedVfxCallback -= OnPowerupVfxCalled;
    // }

    // private void OnItemEquipPressed(PowerupsDataSO powerupsDataSO)
    // {
    //     isEquippedPressed = true;
    //     if (this.powerupsDataSO == null || !this.powerupsDataSO.IsPowerupUnlocked()) return;
    //     if (GameManager.GetInstance().GetPowerupDeckSlotById(cardId) == -1) return;

    //     this.powerupToReplace = powerupsDataSO;

    //     SetCardShake(true);
    // }

    // private void OnPowerupEquippedCallback()
    // {
    //     isEquippedPressed = false;

    //     if (powerupsDataSO == null || !powerupsDataSO.IsPowerupUnlocked()) return;

    //     this.powerupToReplace = null;

    //     SetCardShake(false);

    //     if (GameManager.GetInstance().GetPowerupDeckSlotById(cardId) != -1)
    //     {
    //         SetCardSelected(true);
    //     }

    //     else
    //     {
    //         SetCardSelected(false);
    //     }
    // }

    // private void OnPowerupVfxCalled(int powerupId)
    // {
    //     if (powerupId != cardId) return;

    //     transform.DOPunchScale(Vector2.one * 0.1f, 0.2f, 1, 0.5f);
    //     AudioManager.CallPlaySFX(Sound.TroopEquipped);
    //     equipParticle.Play();
    // }

    // public void SetPowerupsData(PowerupsDataSO powerupsDataSO)
    // {
    //     this.powerupsDataSO = powerupsDataSO;

    //     CheckForNotification();
    // }

    // private void CheckForNotification()
    // {
    //     if (!powerupsDataSO.IsPowerupUnlocked()) return;

    //     if (PlayerPrefs.HasKey(CARD_NEW_NOTIFICATION_ID + "Powerup_" + cardId))
    //     {
    //         notificationImg.gameObject.SetActive(false);
    //         CancelNotification(receiver);
    //     }

    //     else
    //     {
    //         notificationImg.gameObject.SetActive(true);
    //         TriggerNotification(receiver);
    //     }
    // }

    // protected override void CheckForNotificationOff()
    // {
    //     if (!powerupsDataSO.IsPowerupUnlocked()) return;

    //     PlayerPrefs.SetInt(CARD_NEW_NOTIFICATION_ID + "Powerup_" + cardId, 1);
    //     PlayerPrefs.Save();

    //     notificationImg.gameObject.SetActive(false);
    //     CancelNotification(receiver);
    // }

    // protected override void OnCardButtonClicked()
    // {
    //     base.OnCardButtonClicked();

    //     if (!powerupsDataSO.IsPowerupUnlocked())
    //     {
    //         AudioManager.CallPlaySFX(Sound.ErrorAlert);
    //         ToastMessageManager.GetInstance().ShowToastMessage(powerupsDataSO.GetTroopUnlockString(), 2f);
    //         return;
    //     }


    //     if (isEquippedPressed)
    //     {
    //         if (powerupToReplace == null)
    //         {
    //             AudioManager.CallPlaySFX(Sound.ErrorAlert);
    //             ToastMessageManager.GetInstance().ShowToastMessage("Cannot equip an unequipped Powerup", 2f);
    //             OnPowerupEquipped?.Invoke();
    //             return;
    //         }

    //         int slotIdx = GameManager.GetInstance().GetPowerupDeckSlotById(cardId);

    //         if (slotIdx != -1)
    //         {
    //             OnPowerupEquippedVfxCallback?.Invoke(powerupToReplace.PowerupId);
    //             GameManager.GetInstance().SetPowerupDeckSlot(slotIdx, powerupToReplace.PowerupId);
    //         }

    //         else
    //         {
    //             AudioManager.CallPlaySFX(Sound.ErrorAlert);
    //             ToastMessageManager.GetInstance().ShowToastMessage("Powerup already equipped", 2f);
    //         }

    //         OnPowerupEquipped?.Invoke();
    //     }

    //     else
    //     {
    //         OnPowerupsCardPressed();
    //     }
    // }

    // private void OnPowerupsCardPressed()
    // {
    //     MainMenuUiController mainMenuUiController = (MainMenuUiController)UiManager.GetInstance().GetUiController();
    //     mainMenuUiController.PowerupsDescriptionPopup.SetItemDescription(powerupsDataSO);
    //     mainMenuUiController.PowerupsDescriptionPopup.SetEquippedState(GameManager.GetInstance().GetPowerupDeckSlotById(cardId) != -1);
    //     mainMenuUiController.PowerupsDescriptionPopup.SetView(true);
    // }
}
