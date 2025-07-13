using System;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    private static UiManager instance;

    private Stack<IOverlayUI> overlayUI = new Stack<IOverlayUI>();

    private UiController uiController;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static UiManager GetInstance()
    {
        return instance;
    }

    #region Overlay Control
    public void OnOverlayUiEnabled(IOverlayUI item)
    {
        overlayUI.Push(item);
    }

    public void OnOverlayUiDisabled(IOverlayUI item)
    {
        try
        {
            IOverlayUI ui = overlayUI.Peek();
            if (ui == item)
            {
                overlayUI.Pop();
            }
            else
            {
                BbsLog.LogError("Disabled Overlay UI ref misMatched!");
            }
        }
        catch
        {
            BbsLog.LogError("No Overlay UI to Pop!");
        }
    }


    public void UpdateSkillTokenInUI()
    {
        MainMenuUiController mainMenuUiController = null;
        try
        {
            mainMenuUiController = (MainMenuUiController)uiController;

            mainMenuUiController.TopHudController.UpdateSkillTokenText(Statics.FormatNumber(GameManager.GetInstance().GetCurrentSkillTokenAmount()));
        }
        catch (Exception ex)
        {
            BbsLog.LogError(ex.Message);
        }
    }

    public bool IsOverlayUiActive()
    {
        return overlayUI.Count > 0;
    }

    #endregion

    public UiController GetUiController()
    {
        return uiController;
    }

    public void AttachUIController(UiController controller)
    {
        uiController = controller;
    }

    public void UpdateTopHudCoins()
    {
        
    }

    public Sprite GetFlyObjectIcon(FlyObjectType rewardType)
    {
        //TODO: Update icons and types according to game
        Sprite returnSprite = null;
        return returnSprite;
    }
}
