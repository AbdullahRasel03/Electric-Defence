using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using System.Collections;

public class MiddlePanelController : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Scrollbar scrollbar;

    [Space]
    [SerializeField] private MiddleViews deckView;
    [SerializeField] private MiddleViews battleView;
    [SerializeField] private SkillView skillView;


    private float scrollFraction;
    private MiddleViews currentView;

    public SkillView SkillView => skillView;


    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        scrollFraction = 1f / 2f; //View count - 1 = 2
        scrollbar.value = scrollFraction;
        currentView = battleView;
        battleView.OnViewSelected();

        scrollRect.horizontal = false;
    }

    public void SetView(UI_VIEW view)
    {
        currentView.OnViewUnSelected();
        switch (view)
        {
            case UI_VIEW.DECK:
                DOTween.To(() => scrollbar.value, x => scrollbar.value = x, 0f, 0.2f).SetEase(Ease.OutQuint);

                deckView.OnViewSelected();
                currentView = deckView;
                // AnalyticsEventCall.MenuOpen(PopUpMenuTypes.ShopMenu);
                break;
            case UI_VIEW.BATTLE:
                DOTween.To(() => scrollbar.value, x => scrollbar.value = x, scrollFraction, 0.2f).SetEase(Ease.OutQuint);
               
                battleView.OnViewSelected();
                currentView = battleView;
                break;
            case UI_VIEW.SKILL:
                DOTween.To(() => scrollbar.value, x => scrollbar.value = x, scrollFraction * 2, 0.2f).SetEase(Ease.OutQuint);
               
                skillView.OnViewSelected();
                currentView = skillView;
                break;
        }
    }
}