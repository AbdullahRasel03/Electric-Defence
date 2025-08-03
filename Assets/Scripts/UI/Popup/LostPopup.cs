using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LostPopup : PopupBase
{
    [SerializeField] private CanvasGroup redBgGroup;
    [SerializeField] private GameObject headerHolder;
    [SerializeField] private GameObject restartButton;

    protected override void OnPopupEnabled()
    {
        base.OnPopupEnabled();

        headerHolder.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetUpdate(true);

        DOTween.To(() => redBgGroup.alpha, x => redBgGroup.alpha = x, 0, 0.25f).SetUpdate(true).OnComplete
            (() =>
                {
                    redBgGroup.blocksRaycasts = true;
                    redBgGroup.interactable = true;
                }
            );

        Time.timeScale = 0.25f;

        DOVirtual.DelayedCall(0.25f, () =>
        {
            if (restartButton != null)
            {
                restartButton.transform.DOScale(Vector3.one, 0.2f).SetUpdate(true).SetEase(Ease.OutBack);
            }
        });
    }
}
