using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class BottomHudSection : InGameNotificationReceiver
{
    [SerializeField] private int unlockWorldId;
    [SerializeField] private int unlockChapterId;
    [SerializeField] private UI_VIEW sectionType;
    [SerializeField]
    private RectTransform iconRect;
    [SerializeField]
    private RectTransform nameRect, upgradeNotificationDot, lockIconRect;
    [SerializeField]
    private Image iconImg;
    public Color lockIconColor;

    private bool isCurrentlySelected = false;
    private float selectedPosY = 70f;
    private float selectedScale = 1.2f;

    private GameManager gameManager;
    private MainMenuUiController mainmenuUiController;

    public bool IsSelected => isCurrentlySelected;


    private GameManager GetGameManager()
    {
        if (gameManager == null)
            return gameManager = GameManager.GetInstance();

        return gameManager;
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        mainmenuUiController = (MainMenuUiController)UiManager.GetInstance().GetUiController();
    }

    public void InitView(bool isSelected)
    {
        isCurrentlySelected = isSelected;

        if (isCurrentlySelected)
        {
            iconRect.anchoredPosition3D = new Vector3(0, selectedPosY, 0);
            iconRect.localScale = Vector3.one * selectedScale;
            nameRect.localScale = Vector3.one;
        }
        else
        {
            iconRect.anchoredPosition3D = Vector3.zero;
            iconRect.localScale = Vector3.one;
            nameRect.localScale = Vector3.zero;
        }

        UpdateLockView();
    }

    public void SetView(bool isSelected)
    {
        if (isCurrentlySelected != isSelected)
        {
            isCurrentlySelected = isSelected;
            if (isSelected)
            {
                DOTween.Kill(gameObject.name + "UnSelect1");
                DOTween.Kill(gameObject.name + "UnSelect2");
                DOTween.Kill(gameObject.name + "UnSelect3");
                iconRect.DOAnchorPosY(selectedPosY, 0.3f).SetId(gameObject.name + "Select1");
                iconRect.DOScale(Vector3.one * selectedScale, 0.3f).SetId(gameObject.name + "Select2");
                nameRect.DOScale(Vector3.one, 0.3f).SetId(gameObject.name + "Select3");
            }
            else
            {
                DOTween.Kill(gameObject.name + "Select1");
                DOTween.Kill(gameObject.name + "Select2");
                DOTween.Kill(gameObject.name + "Select3");
                iconRect.DOAnchorPosY(0, 0.3f).SetId(gameObject.name + "UnSelect1");
                iconRect.DOScale(Vector3.one, 0.3f).SetId(gameObject.name + "UnSelect2");
                nameRect.DOScale(Vector3.zero, 0.3f).SetId(gameObject.name + "UnSelect3");
            }
        }
    }

    public UI_VIEW GetSectionType()
    {
        return sectionType;
    }

    public void SelectView()
    {
        if (!IsViewUnlocked())
            return;

        AudioManager.CallPlaySFX(Sound.ButtonClick);
        VibrationManager.instance.PlayHapticLight();

        if (mainmenuUiController == null)
            mainmenuUiController = (MainMenuUiController)UiManager.GetInstance().GetUiController();

        mainmenuUiController.OnViewSelect(sectionType);
    }

    private bool IsViewUnlocked()
    {
        int currentWorldId = GetGameManager().GetPlayerData().currentWorldId;
        int currentChapterId = GetGameManager().GetPlayerData().currentAreaId;

        if (unlockWorldId < currentWorldId)
            return true;
        if (unlockWorldId == currentWorldId && unlockChapterId <= currentChapterId)
            return true;

        return false;
    }

    public void UpdateLockView()
    {
        if (IsViewUnlocked())
        {
            lockIconRect.gameObject.SetActive(false);
            iconImg.color = Color.white;
        }
        else
        {
            lockIconRect.gameObject.SetActive(true);
            iconImg.color = lockIconColor;
        }
    }

    protected override void OnNotificationUpdate(bool isAvaiable)
    {
        base.OnNotificationUpdate(isAvaiable);

        if (!IsViewUnlocked()) return;

        upgradeNotificationDot.gameObject.SetActive(isAvaiable);
    }
}
