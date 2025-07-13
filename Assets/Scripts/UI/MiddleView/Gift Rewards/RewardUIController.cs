using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;

public class RewardUIController : MonoBehaviour
{
    // [Header("References")]
    // [SerializeField] private CanvasGroup canvasGroup;
    // [SerializeField] private Button rewardButton;
    // [SerializeField] private GameObject rewardPanel;
    // [SerializeField] private Transform cardContainer;
    // [SerializeField] private GiftRewardCard rewardCardPrefab;
    // [SerializeField] private Transform startPoint;
    // [SerializeField] private ParticleSystem fadeParticles;
    // [SerializeField] private ParticleSystem giftAvailableParticles;
    // [SerializeField] private ParticleSystem giftshakingParticles;
    // [SerializeField] private ParticleSystem giftAvailableParticlesRewardPanel;
    // [SerializeField] private GameObject giftAvailable;
    // [SerializeField] private TextMeshProUGUI continueText;
    // [SerializeField] private Image obtainedImage;

    // [Header("Timer")]
    // [SerializeField] private TextMeshProUGUI cooldownText;

    // [Header("Reward Setup")]
    // // [SerializeField] private List<ShopRewardsDataSO> possibleRewards;
    // [SerializeField] private int minBaseAmount = 1;
    // [SerializeField] private int maxBaseAmount = 3;
    // [SerializeField] private float layoutSpawnInterval = 0.25f;

    // // private List<ShopRewardsDataSO> storedRewards = new();
    // private bool hasTapped = false;
    // private bool canTap = false;
    // private bool canTapToReturn = false;

    // private CanvasGroup rewardPanelCG;
    // private TimeSpan cooldownDuration = TimeSpan.FromHours(6);
    // // private TimeSpan cooldownDuration = TimeSpan.FromSeconds(10);

    // private RewardState currentRewardState = RewardState.Unavailable;

    // private const string REWARD_FIRST_SESSION_HASH = "Is First Session ";

    // public static event Action OnTroopFragmentsCollected;


    // void Start()
    // {
    //     rewardPanelCG = rewardPanel.GetComponent<CanvasGroup>();
    //     if (rewardPanelCG == null)
    //         rewardPanelCG = rewardPanel.AddComponent<CanvasGroup>();

    //     rewardButton.onClick.RemoveAllListeners();
    //     rewardButton.onClick.AddListener(OnRewardButtonClicked);

    //     rewardPanelCG.alpha = 0;
    //     rewardPanel.SetActive(false);

    //     CheckForSession();
    // }

    // private void CheckForSession()
    // {
    //     if (!PlayerPrefs.HasKey(REWARD_FIRST_SESSION_HASH))
    //     {
    //         PlayerPrefs.SetInt(REWARD_FIRST_SESSION_HASH, 1);
    //         PlayerPrefs.Save();
    //         GameManager.instance.SetFreeGiftCollectionTime(DateTime.Now);
    //         rewardButton.gameObject.SetActive(false);
    //         giftAvailableParticles.gameObject.SetActive(false);
    //         giftAvailableParticles.Stop();
    //         cooldownText.gameObject.SetActive(false);
    //         giftAvailable.SetActive(false);
    //         return;
    //     }

    //     UpdateCooldownState(); // Initial update
    //     InvokeRepeating(nameof(UpdateCooldownState), 0f, 1f); // Realtime check every second
    // }

    // void UpdateCooldownState()
    // {
    //     DateTime lastCollectedTime = GameManager.instance.GetFreeGiftCollectionTime();
    //     DateTime now = DateTime.Now;

    //     TimeSpan timeSinceLastCollection = now - lastCollectedTime;
    //     TimeSpan timeLeft = cooldownDuration - timeSinceLastCollection;

    //     bool isCooldownOver = timeLeft <= TimeSpan.Zero;

    //     if (isCooldownOver)
    //     {
    //         currentRewardState = RewardState.Available;
    //         // rewardButton.interactable = true;
    //         cooldownText.text = "Gift Available";
    //         giftAvailable.SetActive(true);
    //         if (!giftAvailableParticles.isPlaying) giftAvailableParticles.Play();


    //     }
    //     else
    //     {
    //         currentRewardState = RewardState.Unavailable;
    //         // rewardButton.interactable = false;
    //         giftAvailable.SetActive(false);
    //         if (giftAvailableParticles.isPlaying) giftAvailableParticles.Stop();

    //         cooldownText.text = $"{timeLeft.Hours:D2}:{timeLeft.Minutes:D2}:{timeLeft.Seconds:D2}";
    //     }
    // }

    // void OnRewardButtonClicked()
    // {
    //     if (currentRewardState == RewardState.Unavailable)
    //     {
    //         AudioManager.CallPlaySFX(Sound.ErrorAlert);
    //         ToastMessageManager.GetInstance().ShowToastMessage("Gifts Not Available Yet", 2f);
    //         return;
    //     }

    //     AudioManager.CallPlaySFX(Sound.ButtonClick);
    //     // Start cooldown
    //     //Reseting the cooldown time to now
    //     GameManager.instance.SetFreeGiftCollectionTime(DateTime.Now);

    //     UpdateCooldownState();

    //     // Proceed to show rewards
    //     EnableRewardCollection();
    // }

    // void EnableRewardCollection()
    // {
    //     rewardPanel.SetActive(true);
    //     // rewardPanelCG.alpha = 0;
    //     canvasGroup.alpha = 0;
    //     canvasGroup.interactable = true;
    //     canvasGroup.blocksRaycasts = true;
    //     startPoint.gameObject.SetActive(false);

    //     DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, 0.5f)
    //         .SetEase(Ease.OutQuad).OnComplete(() =>
    //     {
    //         AudioManager.CallPlaySFX(Sound.ItemMerged);

    //         startPoint.gameObject.SetActive(true);
    //         startPoint.transform.localScale = Vector3.zero;
    //         startPoint.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
    //         {
    //             giftAvailableParticlesRewardPanel.Play();
    //             continueText.gameObject.SetActive(true);
    //             continueText.transform.localScale = Vector3.zero;
    //             continueText.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    //         });
    //     });

    //     CanvasGroup cg = startPoint.GetComponent<CanvasGroup>();
    //     if (cg == null) cg = startPoint.gameObject.AddComponent<CanvasGroup>();
    //     cg.alpha = 1f;

    //     ClearCards();
    //     storedRewards.Clear();
    //     hasTapped = false;
    //     canTap = false;

    //     storedRewards.Add(possibleRewards[0]);

    //     DOVirtual.DelayedCall(0.2f, () =>
    //     {
    //         canTap = true;
    //     });

    //     // StartCoroutine(EnableTapAfterDelay(0.2f));
    // }

    // // IEnumerator EnableTapAfterDelay(float delay)
    // // {
    // //     yield return new WaitForSeconds(delay);
    // //     canTap = true;
    // // }

    // void Update()
    // {
    //     if (canTap && !hasTapped && Input.GetMouseButtonDown(0))
    //     {
    //         AudioManager.CallPlaySFX(Sound.ButtonClick);
    //         hasTapped = true;
    //         canTap = false;
    //         StartCoroutine(ShakeAndShowRewards());
    //     }

    //     if (canTapToReturn && Input.GetMouseButtonDown(0))
    //     {

    //         AudioManager.CallPlaySFX(Sound.ButtonClick);

    //         canTapToReturn = false;

    //         DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0f, 0.25f)
    //             .SetEase(Ease.InQuad).OnComplete(() =>
    //         {
    //             rewardPanel.SetActive(false);
    //             giftAvailable.SetActive(false);
    //             obtainedImage.transform.localScale = Vector3.zero;

    //             canvasGroup.interactable = false;
    //             canvasGroup.blocksRaycasts = false;

    //             CanvasGroup cg = startPoint.GetComponent<CanvasGroup>();
    //             if (cg == null) cg = startPoint.gameObject.AddComponent<CanvasGroup>();
    //             cg.alpha = 1f;
    //         });
    //     }
    // }

    // IEnumerator ShakeAndShowRewards()
    // {
    //     continueText.gameObject.SetActive(false);
    //     RectTransform startRect = startPoint.GetComponent<RectTransform>();
    //     Vector2 originalPos = startRect.anchoredPosition;


    //     giftAvailableParticlesRewardPanel.Stop();

    //     //Play shake particle
    //     giftshakingParticles.Play();

    //     AudioManager.CallPlaySFX(Sound.GiftBoxShaking);
    //     startRect.DOShakePosition(1.5f, 15f, 10, 90, false, false);

    //     yield return new WaitForSeconds(1.65f);

    //     //stop shake particle
    //     giftshakingParticles.Stop();

    //     startPoint.transform.DOScale(1.3f, 0.1f).SetEase(Ease.OutQuad)
    //         .OnComplete(() =>
    //         {
    //             startPoint.transform.DOScale(0f, 0.1f).SetEase(Ease.InBack);
    //         });

    //     if (fadeParticles != null)
    //     {
    //         AudioManager.CallPlaySFX(Sound.TroopEquipped);
    //         fadeParticles.transform.position = startPoint.position + new Vector3(0, 0, -0.5f);
    //         fadeParticles.Play();
    //         StartCoroutine(StopParticlesAfterDelay(1f));
    //     }

    //     yield return StartCoroutine(PlayGridSequence());
    // }

    // IEnumerator StopParticlesAfterDelay(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     fadeParticles.Stop();
    // }

    // IEnumerator PlayGridSequence()
    // {
    //     obtainedImage.transform.DOScale(1f, 0.5f).SetEase(Ease.InBack);
    //     yield return new WaitForSeconds(0.55f);

    //     WaitForSeconds delay = new WaitForSeconds(layoutSpawnInterval);

    //     int troopFragmentCount = storedRewards[0].shopRewards.FindAll(r => r.rewardType == ECONOMY_TYPE.TROOP_FRAGMENT).Count;

    //     List<TroopDataSO> randomTroop = GameManager.GetInstance().GetRandomUnlockedTroops(troopFragmentCount);

    //     int troopIndex = 0;

    //     for (int i = 0; i < storedRewards[0].shopRewards.Count; i++)
    //     {
    //         if (i > 1 && randomTroop[0] == randomTroop[1]) break;

    //         RewardTypeAndAmount rewards = storedRewards[0].shopRewards[i];

    //         GiftRewardCard card = Instantiate(rewardCardPrefab, cardContainer);
    //         card.transform.localScale = Vector3.zero;

    //         int finalAmount = rewards.rewardType == ECONOMY_TYPE.TROOP_FRAGMENT ?
    //             UnityEngine.Random.Range(minBaseAmount, maxBaseAmount + 1) :
    //             rewards.amount;

    //         Sprite rewardIcon = rewards.rewardType == ECONOMY_TYPE.TROOP_FRAGMENT ?
    //             randomTroop[troopIndex].TroopFragmentImg :
    //             UiManager.GetInstance().GetFlyObjectIcon(FlyObjectType.Coins);


    //         card.SetProperties(rewards.rewardType.ToString(), finalAmount, rewardIcon, rewards.rewardType);

    //         AudioManager.CallPlaySFX(Sound.TroopCardActive);

    //         card.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

    //         if (rewards.rewardType == ECONOMY_TYPE.TROOP_FRAGMENT)
    //         {
    //             troopIndex++;

    //             if (troopIndex >= randomTroop.Count)
    //             {
    //                 troopIndex = randomTroop.Count - 1;
    //             }
    //         }

    //         if (rewards.rewardType == ECONOMY_TYPE.TROOP_FRAGMENT)
    //         {
    //             GameManager.GetInstance().AddTroopFragments(randomTroop[troopIndex].TroopId, finalAmount);
    //             OnTroopFragmentsCollected?.Invoke();
    //         }

    //         else if (rewards.rewardType == ECONOMY_TYPE.COIN)
    //         {
    //             GameManager.GetInstance().AddCoins(finalAmount);
    //         }

    //         yield return delay;

    //     }

    //     yield return new WaitForSeconds(0.5f);
    //     continueText.gameObject.SetActive(true);
    //     continueText.transform.localScale = Vector3.zero;
    //     continueText.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

    //     canTapToReturn = true;
    // }

    // void ClearCards()
    // {
    //     foreach (Transform child in cardContainer)
    //     {
    //         Destroy(child.gameObject);
    //     }
    // }
}

public enum RewardState
{
    Available,
    Unavailable
}
