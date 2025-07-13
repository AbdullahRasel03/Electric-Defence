using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPanelTroops : DeckPanel
{
    override protected void OnStart()
    {
        base.OnStart();

        // GameManager.GetInstance().CheckAutoEquipTroops();

        InitCards();
    }

    private void InitCards()
    {
        // int maxTroop = deckCards.Count;
        // int unlockedTroopCount = 0;

        // foreach (DeckCard card in deckCards)
        // {
        //     TroopDataSO troopDataSO = GameManager.GetInstance().GetTroopData(card.CardId);

        //     if (troopDataSO != null)
        //     {
        //         if(troopDataSO.IsTroopUnlocked())
        //         {
        //             unlockedTroopCount++;
        //         }


        //         int troopLevel = GameManager.GetInstance().GetTroopLevel(troopDataSO.TroopId);

        //         card.InitCard(troopDataSO.TroopName, troopLevel, troopDataSO.IsTroopUnlocked(), troopDataSO.TroopImg);
        //         DeckCardTroop deckCardTroop = card as DeckCardTroop;

        //         if (deckCardTroop != null)
        //         {
        //             deckCardTroop.SetTroopData(troopDataSO);
        //         }
        //     }
        // }

        // unlockedAmountText.text = $"{unlockedTroopCount}/{maxTroop} Unlocked";

        // SetSelectedState();
    }

    private void SetSelectedState()
    {
        // List<TroopDataSO> troopList = GameManager.GetInstance().GetEquippedTroops();

        // for (int i = 0; i < deckCards.Count; i++)
        // {
        //     if (troopList.Contains(GameManager.GetInstance().GetTroopData(deckCards[i].CardId)))
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
