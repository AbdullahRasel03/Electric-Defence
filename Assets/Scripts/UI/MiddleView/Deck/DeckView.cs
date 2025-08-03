using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DeckView : MiddleViews
{
    [SerializeField] private Transform deployedHeroesParent;
    [SerializeField] private Transform unDeployedHeroesParent;
    [SerializeField] private DeckCardTroop cardPrefab;

    private List<DeckCardTroop> deployedCards = new List<DeckCardTroop>();
    private List<DeckCardTroop> undeployedCards = new List<DeckCardTroop>();

    private int drawCost = 100;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(InitializeDeck());
    }

    private IEnumerator InitializeDeck()
    {
        yield return new WaitForSeconds(0.1f);
        PopulateDeployedHeroes();
        PopulateUndeployedHeroes();
    }

    private void PopulateDeployedHeroes()
    {
        foreach (TroopDataSO troopData in GameManager.GetInstance().AllHeroData)
        {
            if (GameManager.GetInstance().GetPlayerData().troopDeckSlots.Contains(troopData.TroopId))
            {
                DeckCardTroop card = Instantiate(cardPrefab, deployedHeroesParent);
                card.SetTroopData(troopData, this);
                deployedCards.Add(card);
            }
        }
    }

    private void PopulateUndeployedHeroes()
    {
        foreach (TroopDataSO troopData in GameManager.GetInstance().AllHeroData)
        {
            if (!GameManager.GetInstance().GetPlayerData().troopDeckSlots.Contains(troopData.TroopId))
            {
                DeckCardTroop card = Instantiate(cardPrefab, unDeployedHeroesParent);
                card.SetTroopData(troopData, this);
                undeployedCards.Add(card);
            }
        }
    }

    internal void OnTropEquipClicked(TroopDataSO currentTroop, TroopDataSO troopToReplace)
    {
        if (currentTroop == null || troopToReplace == null) return;

        DeckCardTroop currentCard = deployedCards.Find(card => card.CardId == currentTroop.TroopId);
        DeckCardTroop replaceCard = undeployedCards.Find(card => card.CardId == troopToReplace.TroopId);

        if (currentCard != null && replaceCard != null)
        {
            Transform currentParent = currentCard.transform.parent;
            Transform replaceParent = replaceCard.transform.parent;

            Vector3 currentPosition = currentCard.transform.position;
            Vector3 replacePosition = replaceCard.transform.position;

            // Swap their positions in our lists
            deployedCards.Remove(currentCard);
            undeployedCards.Remove(replaceCard);
            deployedCards.Add(replaceCard);
            undeployedCards.Add(currentCard);
            int siblingIndexCurrent = currentCard.transform.GetSiblingIndex();
            int siblingIndexReplace = replaceCard.transform.GetSiblingIndex();

            Debug.LogError($"Index Current: {siblingIndexCurrent}, Index Replace: {siblingIndexReplace}");

            currentCard.transform.DOMove(replacePosition, 0.3f).OnComplete(() =>
            {
                currentCard.transform.SetParent(replaceParent);
                currentCard.transform.SetSiblingIndex(siblingIndexReplace);
                currentCard.transform.position = replacePosition;
            });

            replaceCard.transform.DOMove(currentPosition, 0.3f).OnComplete(() =>
            {
                replaceCard.transform.SetParent(currentParent);
                replaceCard.transform.SetSiblingIndex(siblingIndexCurrent);
                replaceCard.transform.position = currentPosition;
                Debug.LogError(replaceCard.transform.GetSiblingIndex());
                replaceCard.ShowEquippedVfx();

            });
        }
    }

    public void OnFragmentsDrawClicked(int drawAmount)
    {
        int currentCoins = GameManager.GetInstance().GetCurrentCoinAmount();

        int currentDrawCost = drawCost * drawAmount;

        if (currentCoins < currentDrawCost)
        {
            ToastMessageManager.GetInstance().ShowToastMessage("Not enough coins to draw troops", 2f);
            AudioManager.CallPlaySFX(Sound.ErrorAlert);
            return;
        }

        List<TroopDataSO> unlockedTroops = GameManager.GetInstance().GetRandomUnlockedHeroes(drawAmount);

        foreach (TroopDataSO troopData in unlockedTroops)
        {
            // Debug.LogError($"Unlocked Troop: {troopData.TroopId}");
        }

        GameManager.GetInstance().AddCoins(-currentDrawCost);
    }
    
    public void ButtonClickVfx(Transform buttonTransform)
    {
        if (buttonTransform != null)
        {
            buttonTransform.DOPunchScale(Vector2.one * 0.1f, 0.2f, 1, 0.5f);
            AudioManager.CallPlaySFX(Sound.ButtonClick);
        }
    }
}
