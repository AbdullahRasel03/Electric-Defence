using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class NewTroopUnlockedPopup : PopupBase
{
    // [SerializeField] private GameObject topBar;
    // [SerializeField] private GameObject okButton;
    // [SerializeField] private RectTransform troopImageHolder;
    // [SerializeField] private List<GameObject> troopImages;
    // [SerializeField] private Image troopImageLocked;
    // [SerializeField] private Image lockedIcon;
    // [SerializeField] private Image bgOverlay;
    // [SerializeField] private TMP_Text troopName;
    // [SerializeField] private RectTransform deckRectTransform;
    // [SerializeField] private ParticleSystem troopUnlockedParticle;
    // [SerializeField] private ParticleSystem bgParticle;
    // [SerializeField] private ParticleSystem flyObjectParticle;

    // private int currentTroopIndex = 0;
    // private GameObject currentTroopImage;

    // private List<TroopDataSO> unlockedTroops = new List<TroopDataSO>();

    // public bool IsFeatureShowing { get; private set; }

    // private const string alreadyShowedHash = "alreadyShowed ";

    // internal void CheckForUnlockedTroops()
    // {
    //     Reset();
    //     currentTroopIndex = 0;
    //     InitUnlockedTroops();
    // }
    // private void Reset()
    // {
    //     bgParticle.gameObject.SetActive(true);
    //     topBar.transform.localScale = new Vector3(0, 1, 1);
    //     okButton.transform.localScale = Vector3.zero;
    //     troopImageHolder.transform.localScale = Vector3.zero;
    //     troopName.transform.localScale = Vector3.zero;
    //     troopImageLocked.gameObject.SetActive(true);
    //     lockedIcon.gameObject.SetActive(true);
    //     bgOverlay.enabled = true;
        
    //     Vector2 tiling = bgOverlay.material.GetVector("_Tiling");
    //     if (Statics.IsTab())
    //     {
    //         bgOverlay.material.SetVector("_Tiling", new Vector2(4.5f, tiling.y));
    //     }
    //     else
    //     {
    //         bgOverlay.material.SetVector("_Tiling", new Vector2(3f, tiling.y));
    //     }
    // }

    // private void InitUnlockedTroops()
    // {
    //     int currentWorldId = GameManager.GetInstance().GetPlayerData().currentWorldId;
    //     int currentChapterId = GameManager.GetInstance().GetPlayerData().currentAreaId;

    //     unlockedTroops.Clear();
    //     List<TroopDataSO> allTroops = GameManager.GetInstance().AllTroopData;


    //     foreach (TroopDataSO troop in allTroops)
    //     {
    //         if (troop.UnlockWorldId == currentWorldId && troop.UnlockAreaId == currentChapterId && troop.TroopId != 0 && !IsAlreadyUnlocked(troop))
    //         {
    //             unlockedTroops.Add(troop);
    //         }
    //     }

    //     if (unlockedTroops.Count > 0 && currentTroopIndex < unlockedTroops.Count)
    //     {
    //         SetMainMenuTut(true);
    //         SetView(true, 0.2f, 0.3f, 1f);
    //     }

    //     SetTroopData();
    // }

    // private void SetTroopData()
    // {
    //     if (unlockedTroops.Count == 0)
    //     {
    //         IsFeatureShowing = false;
    //         OnTroopShowcaseFinished();
    //         SetView(false);
    //         SetMainMenuTut(false);
    //         return;
    //     }

    //     if (currentTroopIndex >= unlockedTroops.Count)
    //     {
    //         IsFeatureShowing = false;
    //         OnTroopShowcaseFinished();
    //         SetMainMenuTut(false);
    //         SetView(false);
    //         return;
    //     }

    //     IsFeatureShowing = true;

    //     TroopDataSO troop = unlockedTroops[currentTroopIndex];

    //     currentTroopImage = troopImages[troop.TroopId];
    //     troopImageLocked.sprite = troop.TroopImg;

    //     string hexColor = ColorUtility.ToHtmlStringRGB(Color.yellow);
    //     troopName.text = $"<color=#{hexColor}>{troop.TroopName}</color> Unlocked";

    //     ShowTroop();
    // }

    // private bool IsAlreadyUnlocked(TroopDataSO troop)
    // {
    //     return PlayerPrefs.HasKey(alreadyShowedHash + troop.TroopId);
    // }

    // private void SetMainMenuTut(bool flag)
    // {
    //     TutorialManager.GetInstance().SetMainMenuTutRunning(flag);
    // }

    // private void ShowTroop()
    // {
    //     Reset();

    //     darkBg.DOFade(1f, 0.15f);

    //     DOVirtual.DelayedCall(0.5f, () =>
    //     {
    //         lockedIcon.transform.DOShakeRotation(1.25f, new Vector3(0, 0, 10f), 30, 10f, false).OnComplete
    //         (
    //             () =>
    //             {
    //                 troopUnlockedParticle.Play();
    //                 AudioManager.CallPlaySFX(Sound.TroopEquipped);
    //                 lockedIcon.gameObject.SetActive(false);
    //                 troopImageLocked.gameObject.SetActive(false);
    //                 for (int i = 0; i < troopImages.Count; i++)
    //                 {
    //                     troopImages[i].SetActive(false);
    //                 }
    //                 troopName.transform.DOScale(1, 0.15f).SetEase(Ease.OutBack);
    //                 currentTroopImage.SetActive(true);
    //                 currentTroopImage.GetComponent<Animator>().CrossFade("Idle", 0.1f);
    //                 troopImageHolder.transform.DOScale(1, 0.15f).SetEase(Ease.OutBack).OnComplete(() => bgParticle.Play());
    //                 topBar.transform.DOScale(1, 0.15f).SetEase(Ease.OutBack);

    //                 DOVirtual.DelayedCall(0.5f, () =>
    //                 {
    //                     okButton.transform.DOScale(1, 0.15f).SetEase(Ease.OutBack);
    //                 });
    //             }
    //         );
    //     });
    // }

    // public async void OnCloseClicked()
    // {
    //     AudioManager.CallPlaySFX(Sound.ButtonClick);

    //     TroopDataSO troop = unlockedTroops[currentTroopIndex];

    //     PlayerPrefs.SetInt(alreadyShowedHash + troop.TroopId, 1);
    //     currentTroopIndex++;
    //     bgParticle.Stop();
    //     bgParticle.gameObject.SetActive(false);

    //     HideTemporarily();
    //     IsFeatureShowing = false;

    //     if (deckRectTransform != null)
    //     {
    //         await FlyobjectManager.GetInstance().CreateFlyObjects(1, troopImageHolder, deckRectTransform, FlyObjectType.None, () =>
    //         {
    //             bgOverlay.enabled = false;
    //             darkBg.DOFade(0f, 0.15f);
    //             flyObjectParticle.transform.position = deckRectTransform.position;
    //             flyObjectParticle.Play();
    //         }, null, troop.TroopImg, 1.5f);
    //     }

    //     DOVirtual.DelayedCall(1.5f, SetTroopData);
    // }

    // private void HideTemporarily()
    // {
    //     troopImageHolder.transform.DOScale(0, 0.15f);
    //     topBar.transform.DOScale(new Vector3(0, 1, 1), 0.15f);
    //     troopName.transform.localScale = Vector3.zero;
    //     okButton.transform.DOScale(0, 0.15f);
    // }

    // private void OnTroopShowcaseFinished()
    // {
    //     DOVirtual.DelayedCall(0.25f, () =>
    //     {
    //         TutorialManager.GetInstance().StartMainMenuTuts();
    //     });
    // }
}
