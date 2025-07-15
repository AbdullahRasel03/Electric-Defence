using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionPopup : PopupBase
{
    [SerializeField] private TMP_Text itemNameTxt;
    [SerializeField] private TMP_Text itemDescriptionTxt;
    [SerializeField] private Image itemImg;
    [SerializeField] private Button equipButton;

    // private ItemDataSO itemDataSO;

    // public static event Action<ItemDataSO> OnEquipButtonPressedEvent;


    // public void SetItemDescription(ItemDataSO itemDataSO)
    // {
    //     this.itemDataSO = itemDataSO;
    //     itemNameTxt.text = itemDataSO.ItemName;
    //     itemDescriptionTxt.text = itemDataSO.GetItemDescription();
    //     itemImg.sprite = itemDataSO.ItemIcon;
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
    //     OnEquipButtonPressedEvent?.Invoke(itemDataSO);
    //     SetView(false);
    // }
}
