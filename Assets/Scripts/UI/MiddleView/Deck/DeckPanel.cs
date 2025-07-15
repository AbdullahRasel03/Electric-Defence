using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckPanel : MonoBehaviour
{
    [SerializeField] protected TMP_Text unlockedAmountText;
    [SerializeField] protected List<DeckCard> deckCards;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        OnStart();
    }
    
    protected virtual void OnStart()
    {
    }
}
