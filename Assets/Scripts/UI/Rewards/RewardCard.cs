using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardCard : MonoBehaviour
{
    [SerializeField] private TMP_Text rewardAmountText;
    [SerializeField] private TMP_Text rewardTypeText;
    [SerializeField] private Image rewardIcon;

    public virtual void SetProperties(string rewardType, int amount, Sprite icon, ECONOMY_TYPE economyType)
    {
        if (rewardTypeText != null)
            rewardTypeText.text = rewardType;
            
        rewardAmountText.text = amount.ToString();
        rewardIcon.sprite = icon;
    }

}
