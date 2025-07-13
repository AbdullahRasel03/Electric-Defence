using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPanelLiftAddOn : DeckPanel
{
    override protected void OnStart()
    {
        base.OnStart();

        // GameManager.GetInstance().CheckForAutoEquipLiftAddOns();

        InitCards();
    }

    private void InitCards()
    {
        // foreach (DeckCard card in deckCards)
        // {
        //     ItemDataSO itemDataSO = GameManager.GetInstance().GetItemData(card.CardId);

        //     if (itemDataSO != null)
        //     {
        //         card.InitCard(itemDataSO.ItemName, 1, itemDataSO.IsItemUnlocked(), itemDataSO.ItemIcon);
        //         DeckCardLiftAddOn deckCardLiftAddOn = card as DeckCardLiftAddOn;

        //         if (deckCardLiftAddOn != null)
        //         {
        //             deckCardLiftAddOn.SetItemData(itemDataSO);
        //         }
        //     }
        // }

        SetSelectedState();
    }

    private void SetSelectedState()
    {
        // List<ItemDataSO> liftAddOnList = GameManager.GetInstance().GetEquippedLiftAddOns();

        // for (int i = 0; i < deckCards.Count; i++)
        // {
        //     if (liftAddOnList.Contains(GameManager.GetInstance().GetItemData(deckCards[i].CardId)))
        //     {
        //         deckCards[i].SetCardSelected(true);
        //     }
        //     else
        //     {
        //         deckCards[i].SetCardSelected(false);
        //     }
        // }
    }
}
