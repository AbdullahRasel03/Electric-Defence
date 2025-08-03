using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BottomHudController : MonoBehaviour
{
    [SerializeField]
    private RectTransform sectionSelectImgRect;

    [Space]
    // [SerializeField]
    // private BottomHudSection shopSection;
    [SerializeField] private BottomHudSection deckSection;
    [SerializeField] private BottomHudSection battleSection;
    // [SerializeField]
    // private BottomHudSection weaponSection;
    [SerializeField] private BottomHudSection skillSection;

    private UI_VIEW currentSelectedView;

    // public static event Action OnWeaponSectionSelected;
    public static event Action OnSkillSectionSelected;
    public static event Action OnDeckSectionSelected;
    public static event Action OnBattleSectionSelected;
    // public static event Action OnShopSectionSelected;

    private float screenWidth = 1080f;
    
    public BottomHudSection DeckSection => deckSection;


// #if UNITY_EDITOR
    //     void OnValidate()
    //     {
    //         UpdateSelectionImg();
    //     }
    // #endif


    void Start()
    {
        UpdateSelectionImg();

        currentSelectedView = UI_VIEW.BATTLE;
        deckSection.InitView(false);
        battleSection.InitView(true);
        skillSection.InitView(false);
    }

    private void UpdateSelectionImg()
    {
        screenWidth = Statics.IsTab() ? 1536f : 1080f;
        sectionSelectImgRect.sizeDelta = new Vector2(screenWidth / 3f, sectionSelectImgRect.sizeDelta.y);
    }

    public void SetView(UI_VIEW sectionType)
    {
        currentSelectedView = sectionType;
        float delta = screenWidth / 3f;

        switch (sectionType)
        {
            case UI_VIEW.DECK:

                deckSection.SetView(true);
                battleSection.SetView(false);
                skillSection.SetView(false);

                sectionSelectImgRect.DOAnchorPosX(-delta, 0.3f);

                OnDeckSectionSelected?.Invoke();
                break;

            case UI_VIEW.BATTLE:
                deckSection.SetView(false);
                battleSection.SetView(true);
                skillSection.SetView(false);

                sectionSelectImgRect.DOAnchorPosX(0, 0.3f);

                OnBattleSectionSelected?.Invoke();
                break;

            case UI_VIEW.SKILL:

                deckSection.SetView(false);
                battleSection.SetView(false);
                skillSection.SetView(true);

                sectionSelectImgRect.DOAnchorPosX(delta, 0.3f);

                OnSkillSectionSelected?.Invoke();
                break;
        }
    }
}

public enum UI_VIEW
{
    DECK = 0,
    BATTLE,
    SKILL

}
