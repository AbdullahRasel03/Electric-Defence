using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeatureShowcasePopup : PopupBase
{
    [SerializeField] private GameObject topBar;
    [SerializeField] private GameObject okButton;
    [SerializeField] private Image featureImage;
    [SerializeField] private Image featureImageLocked;
    [SerializeField] private Image lockedIcon;
    [SerializeField] private TMP_Text featureName;
    [SerializeField] private ParticleSystem featureUnlockedParticle;
    [SerializeField] private ParticleSystem bgParticle;
    [SerializeField] private ParticleSystem flyObjectParticle;
    [SerializeField] private NewTroopUnlockedPopup newTroopUnlockedPopup;
    [SerializeField] private List<FeatureShowcaseData> featureShowcaseData;


    private int currentFeatureIndex = 0;

    private List<FeatureShowcaseData> unlockedFeatures = new List<FeatureShowcaseData>();

    public bool IsFeatureShowing { get; private set; }

    private const string alreadyShowedHash = "alreadyShowed ";

    void Start()
    {
        Reset();
        currentFeatureIndex = 0;
        InitUnlockedFeatures();
    }

    private void Reset()
    {
        bgParticle.gameObject.SetActive(true);
        topBar.transform.localScale = new Vector3(0, 1, 1);
        okButton.transform.localScale = Vector3.zero;
        featureImage.transform.localScale = Vector3.zero;
        featureImageLocked.gameObject.SetActive(true);
        lockedIcon.gameObject.SetActive(true);
    }

    private void InitUnlockedFeatures()
    {
        int currentWorldId = GameManager.GetInstance().GetPlayerData().currentWorldId;
        int currentChapterId = GameManager.GetInstance().GetPlayerData().currentAreaId;

        foreach (FeatureShowcaseData feature in featureShowcaseData)
        {
            if (feature.unlockWorldId == currentWorldId && feature.unlockChapterId == currentChapterId && !IsAlreadyUnlocked(feature))
            {
                unlockedFeatures.Add(feature);
            }
        }
        if (unlockedFeatures.Count > 0 && currentFeatureIndex < unlockedFeatures.Count)
        {
            SetMainMenuTut(true);
            SetView(true, 0.2f, 0.3f, 0.99f);
        }

        SetFeatureData();
    }

    private void SetFeatureData()
    {
        if (unlockedFeatures.Count == 0)
        {
            IsFeatureShowing = false;
            OnFeaturesFinished();
            SetView(false);
            return;
        }

        if (currentFeatureIndex >= unlockedFeatures.Count)
        {
            IsFeatureShowing = false;
            OnFeaturesFinished();
            SetView(false);
            return;
        }

        IsFeatureShowing = true;

        FeatureShowcaseData feature = unlockedFeatures[currentFeatureIndex];

        featureImage.sprite = feature.image;
        featureImageLocked.sprite = feature.image;

        string hexColor = ColorUtility.ToHtmlStringRGB(Color.yellow);
        featureName.text = $"<color=#{hexColor}>{feature.featureName}</color> Unlocked";

        ShowFeature();
    }

    private bool IsAlreadyUnlocked(FeatureShowcaseData feature)
    {
        return PlayerPrefs.HasKey(alreadyShowedHash + feature.featureId);
    }

    private void SetMainMenuTut(bool flag)
    {
        // TutorialManager.GetInstance().SetMainMenuTutRunning(flag);
    }



    private void ShowFeature()
    {
        Reset();

        // darkBg.DOFade(0.99f, 0.15f);

        // DOVirtual.DelayedCall(0.5f, () =>
        // {
        //     lockedIcon.transform.DOShakeRotation(1.25f, new Vector3(0, 0, 10f), 30, 10f, false).OnComplete
        //     (
        //         () =>
        //         {
        //             featureUnlockedParticle.Play();
        //             AudioManager.CallPlaySFX(Sound.TroopEquipped);
        //             lockedIcon.gameObject.SetActive(false);
        //             featureImageLocked.gameObject.SetActive(false);
        //             featureImage.transform.DOScale(1, 0.15f).SetEase(Ease.OutBack).OnComplete(() => bgParticle.Play());
        //             topBar.transform.DOScale(1, 0.15f).SetEase(Ease.OutBack);

        //             DOVirtual.DelayedCall(0.5f, () =>
        //             {
        //                 okButton.transform.DOScale(1, 0.15f).SetEase(Ease.OutBack);
        //             });
        //         }
        //     );
        // });
    }

    public async void OnCloseClicked()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);

        FeatureShowcaseData feature = unlockedFeatures[currentFeatureIndex];

        PlayerPrefs.SetInt(alreadyShowedHash + feature.featureId, 1);
        currentFeatureIndex++;
        bgParticle.Stop();
        bgParticle.gameObject.SetActive(false);

        HideTemporarily();
        IsFeatureShowing = false;

        // if (feature.featureRectForAnimation != null)
        // {
        //     await FlyobjectManager.GetInstance().CreateFlyObjects(1, featureImage.rectTransform, feature.featureRectForAnimation, FlyObjectType.None, () =>
        //     {
        //         // darkBg.DOFade(0f, 0.15f);
        //         flyObjectParticle.transform.position = feature.featureRectForAnimation.position;
        //         flyObjectParticle.Play();
        //     }, null, feature.image, 1.5f);
        // }

        DOVirtual.DelayedCall(1.5f, SetFeatureData);
    }

    private void HideTemporarily()
    {
        featureImage.transform.DOScale(0, 0.15f);
        topBar.transform.DOScale(new Vector3(0, 1, 1), 0.15f);
        okButton.transform.DOScale(0, 0.15f);
    }

    private void OnFeaturesFinished()
    {
        // newTroopUnlockedPopup.CheckForUnlockedTroops();
        // DOVirtual.DelayedCall(0.25f, () =>
        // {
        //     TutorialManager.GetInstance().StartMainMenuTuts();
        // });
    }

}

[System.Serializable]
public struct FeatureShowcaseData
{
    public int unlockWorldId;
    public int unlockChapterId;
    public int featureId;
    public string featureName;
    public Sprite image;
    public RectTransform featureRectForAnimation;
}
