using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerupsDescriptionPopup : PopupBase
{
    [SerializeField] private TMP_Text itemNameTxt;
    [SerializeField] private TMP_Text itemDescriptionTxt;
    [SerializeField] private TMP_Text lockedTxt;
    [SerializeField] private Image itemImg;
    [SerializeField] private GameObject lockedPanel;
    [SerializeField] private Button equipButton;


    // public static event Action<PowerupsDataSO> OnEquipButtonPressedEvent;
    // private PowerupsDataSO powerupsDataSO;

    // public void SetItemDescription(PowerupsDataSO powerupsDataSO)
    // {
    //     this.powerupsDataSO = powerupsDataSO;
    //     itemNameTxt.text = powerupsDataSO.GetPowerupName();
    //     itemDescriptionTxt.text = powerupsDataSO.GetPowerupDescription();
    //     itemImg.sprite = powerupsDataSO.PowerupIcon;

    //     SetLockedState(!powerupsDataSO.IsPowerupUnlocked(), powerupsDataSO.GetTroopUnlockString());
    // }

    // private void SetLockedState(bool isLocked, string lockedText)
    // {
    //     lockedPanel.SetActive(isLocked);
    //     lockedTxt.text = lockedText;

    //     if (isLocked)
    //     {
    //         equipButton.gameObject.SetActive(false);
    //     }
    // }

    // public void SetEquippedState(bool isEquipped)
    // {
    //     equipButton.gameObject.SetActive(!isEquipped);
    // }

    // public void OnCloseButtonPressed()
    // {
    //     AudioManager.CallPlaySFX(Sound.ButtonClick);
    //     SetView(false);
    // }

    // public void OnEquipButtonPressed()
    // {
    //     AudioManager.CallPlaySFX(Sound.ButtonClick);
    //     OnEquipButtonPressedEvent?.Invoke(powerupsDataSO);
    //     SetView(false);
    // }
}
