using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckCardLiftAddOn : DeckCard
{
    // private ItemDataSO itemDataSO;

    // private bool isEquippedPressed = false;

    // public static event Action OnLiftAddOnEquipped;

    // void Start()
    // {
    //     ItemDescriptionPopup.OnEquipButtonPressedEvent += OnItemEquipPressed;
    // }

    // void OnDestroy()
    // {
    //     ItemDescriptionPopup.OnEquipButtonPressedEvent -= OnItemEquipPressed;
    // }

    // private void OnItemEquipPressed(ItemDataSO itemDataSO)
    // {
    //     isEquippedPressed = true;
    // }

    // public void SetItemData(ItemDataSO itemDataSO)
    // {
    //     this.itemDataSO = itemDataSO;
    // }

    // protected override void OnCardButtonClicked()
    // {
    //     base.OnCardButtonClicked();

    //     if (isEquippedPressed)
    //     {

    //     }

    //     else
    //     {
    //         OnLiftAddOnCardClicked();
    //     }
    // }

    private void OnLiftAddOnCardClicked()
    {
        // MainMenuUiController mainMenuUiController = (MainMenuUiController)UiManager.GetInstance().GetUiController();
        // mainMenuUiController.ItemDescriptionPopup.SetItemDescription(itemDataSO);
        // mainMenuUiController.ItemDescriptionPopup.SetEquippedState(GameManager.GetInstance().GetLiftAddOnDeckSlot(cardId) != -1);
        // mainMenuUiController.ItemDescriptionPopup.SetView(true);
    }
}
