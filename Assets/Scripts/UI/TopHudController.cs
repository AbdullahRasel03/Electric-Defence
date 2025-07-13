using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TopHudController : MonoBehaviour
{
    [SerializeField] private TMP_Text coinAmountText;
    [SerializeField] private TMP_Text skillTokenAmountText;
    [SerializeField] private Button settingsButton;

    private void Start()
    {
        UpdateCoinText(Statics.FormatNumber(GameManager.GetInstance().GetCurrentCoinAmount()));
        // UpdateSkillTokenText(Statics.FormatNumber(GameManager.GetInstance().GetCurrentSkillTokenAmount()));
        settingsButton.onClick.AddListener(OnSettingClicked);
    }

    public void SettingsButtonPressed()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);
    }

    public void UpdateCoinText(string amount)
    {
        coinAmountText.text = amount;
        coinAmountText.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.5f);
    }

    public void UpdateSkillTokenText(string amount)
    {
        skillTokenAmountText.SetText(amount);
        skillTokenAmountText.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.5f);
    }

    public void OnSettingClicked()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);

        settingsButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.5f);


        MainMenuUiController mainMenuUiController = (MainMenuUiController)UiManager.GetInstance().GetUiController();
        mainMenuUiController.SettingsPopup.SetView(true);
    }
}
