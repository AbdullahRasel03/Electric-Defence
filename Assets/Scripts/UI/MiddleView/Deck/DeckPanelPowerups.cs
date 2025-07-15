using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPanelPowerups : DeckPanel
{
    override protected void OnStart()
    {
        base.OnStart();

        InitCards();
    }

    private void InitCards()
    {
        // int maxPowerups = deckCards.Count;
        // int unlockedPowerupsCount = 0;

        // foreach (DeckCard card in deckCards)
        // {
        //     PowerupsDataSO powerupsDataSO = GameManager.GetInstance().GetPowerupData(card.CardId);

        //     if (powerupsDataSO != null)
        //     {
        //         if (powerupsDataSO.IsPowerupUnlocked())
        //         {
        //             unlockedPowerupsCount++;
        //         }

        //         DeckCardPowerups deckCardPowerups = card as DeckCardPowerups;

        //         if (deckCardPowerups != null)
        //         {
        //             deckCardPowerups.SetPowerupsData(powerupsDataSO);
        //         }

        //         card.InitCard(powerupsDataSO.GetPowerupName(), 1, powerupsDataSO.IsPowerupUnlocked(), powerupsDataSO.PowerupIcon);
        //     }
        // }

        // unlockedAmountText.text = $"{unlockedPowerupsCount}/{maxPowerups} Unlocked";

        // SetSelectedState();
    }

    private void SetSelectedState()
    {
        // List<PowerupsDataSO> powerupsList = GameManager.GetInstance().GetEquippedPowerups();

        // for (int i = 0; i < deckCards.Count; i++)
        // {
        //     if (powerupsList.Contains(GameManager.GetInstance().GetPowerupData(deckCards[i].CardId)))
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
