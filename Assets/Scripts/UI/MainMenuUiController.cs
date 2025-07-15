using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUiController : UiController
{
    [SerializeField] private TopHudController topHudController;
    [SerializeField] private MiddlePanelController middlePanelController;
    [SerializeField] private BottomHudController bottomHudController;
    [SerializeField] private ItemDescriptionPopup itemDescriptionPopup;
    [SerializeField] private TroopDescriptionPopup troopDescriptionPopup;
    [SerializeField] private PowerupsDescriptionPopup powerupsDescriptionPopup;
    [SerializeField] private SettingsPopup settingsPopup;

    public ItemDescriptionPopup ItemDescriptionPopup => itemDescriptionPopup;
    public TroopDescriptionPopup TroopDescriptionPopup => troopDescriptionPopup;
    public PowerupsDescriptionPopup PowerupsDescriptionPopup => powerupsDescriptionPopup;
    public SettingsPopup SettingsPopup => settingsPopup;
    
    public TopHudController TopHudController => topHudController;
    public MiddlePanelController MiddlePanelController => middlePanelController;
    public BottomHudController BottomHudController => bottomHudController;

    protected override void Start()
    {
        base.Start();
        // AudioManager.GetInstance().PlayBGM_MM();
    } 



    public void OnViewSelect(UI_VIEW view)
    {
        middlePanelController.SetView(view);
        bottomHudController.SetView(view);
    }
}