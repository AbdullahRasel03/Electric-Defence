using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCard : InGameNotificationTriggerer
{
    [SerializeField] protected int cardId;
    [SerializeField] protected RectTransform cardRect;
    [SerializeField] protected RectTransform lockPanel;
    [SerializeField] protected RectTransform selectedPanel;
    [SerializeField] protected Image cardImg;
    [SerializeField] protected Image notificationImg;
    [SerializeField] protected TMP_Text cardNameText;
    [SerializeField] protected TMP_Text cardLevelText;
    [SerializeField] protected ParticleSystem equipParticle;

    protected bool isUnlocked = false;

    public int CardId => cardId;

    private Sequence cardShakeSeq;

    protected const string CARD_NEW_NOTIFICATION_ID = "NewCard_";

    public void InitCard(string cardName, int cardLevel, bool isUnlocked, Sprite sprite)
    {
        this.isUnlocked = isUnlocked;
        cardImg.sprite = sprite;
        cardNameText.text = cardName;
        cardLevelText.text = $"Lvl {cardLevel.ToString()}";
        lockPanel.gameObject.SetActive(!isUnlocked);
        // selectedPanel.gameObject.SetActive(false);
    }


    public void SetCardSelected(bool isSelected)
    {
        // Debug.LogError($"Setting card {cardId} selected state to {isSelected} while it is {(isUnlocked ? "unlocked" : "locked")}");

        if (!isUnlocked) return;

        // selectedPanel.gameObject.SetActive(isSelected);
    }

    public virtual void OnCardClicked()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);

        transform.DOPunchScale(Vector2.one * 0.1f, 0.2f, 1, 0.5f);
        CheckForNotificationOff();
        OnCardButtonClicked();
    }

    protected virtual void CheckForNotificationOff()
    {

    }

    protected virtual void SetLockedState(bool isLocked)
    {
        lockPanel.gameObject.SetActive(isLocked);
        // selectedPanel.gameObject.SetActive(!isLocked);

        if (isLocked) return;

        // if (!PlayerPrefs.HasKey(CARD_NEW_NOTIFICATION_ID + cardId))
        // {
        //     notificationImg.gameObject.SetActive(true);
        //     TriggerNotification(receiver);
        // }
        // else
        // {
        //     notificationImg.gameObject.SetActive(false);
        //     CancelNotification(receiver);
        // }
    }

    public void SetCardShake(bool canShake)
    {
        if (!canShake)
        {
            if (cardShakeSeq != null)
            {
                cardShakeSeq.Kill();
                cardShakeSeq = null;

                transform.localRotation = Quaternion.identity; // Reset rotation
            }
            return;
        }

        cardShakeSeq = DOTween.Sequence();

        cardShakeSeq
        .Append(transform.DOLocalRotate(new Vector3(0, 0, Random.Range(1, 3f)), 0.05f))
        .AppendInterval(0.015f)
        .Append(transform.DOLocalRotate(new Vector3(0, 0, Random.Range(-1, -3f)), 0.05f))
        .AppendInterval(0.015f)
        .SetLoops(-1)
        .SetId($"Shake_{gameObject.name}{cardId}");
    }

    protected virtual void OnCardButtonClicked() { }
}
