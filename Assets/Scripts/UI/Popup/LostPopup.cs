using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LostPopup : PopupBase
{
    [SerializeField] private GameObject restartButton;

    protected override void OnPopupEnabled()
    {
        base.OnPopupEnabled();

        DOVirtual.DelayedCall(0.25f, () =>
        {
            if (restartButton != null)
            {
                restartButton.transform.DOScale(Vector3.one, 0.2f).SetUpdate(true).SetEase(Ease.OutBack);
            }
        });
    }
}
