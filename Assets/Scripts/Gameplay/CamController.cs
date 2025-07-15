using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CamController : MonoBehaviour
{
    [Header("View Settings")]
    [SerializeField] private float transitionDuration = 0f;
    [SerializeField] private Ease transitionEase = Ease.Linear;

    [Header("View Parents")]
    [SerializeField] private Transform fightViewParent;
    [SerializeField] private Transform shopViewParent;

    [Header("Camera Settings")]
    [SerializeField] private Camera cam;
    [SerializeField] private float fightFOV = 20f;
    [SerializeField] private float shopFOV = 60f;

    [Header("View Objects")]
    [SerializeField] private GameObject[] shopViewObjects;

    private Tween moveTween, rotateTween, fovTween;

    public void SetFightView()
    {
        KillTweens();
        foreach (GameObject go in shopViewObjects) go.SetActive(false);
        transform.SetParent(fightViewParent);
        StartTransition(fightFOV);
    }

    public void SetShopView()
    {
        KillTweens();
        foreach (GameObject go in shopViewObjects) go.SetActive(true);
        transform.SetParent(shopViewParent);
        StartTransition(shopFOV);
    }

    private void StartTransition(float targetFOV)
    {
        transform.localPosition = transform.localPosition; // Forces local space
        transform.localRotation = transform.localRotation;

        moveTween = transform.DOLocalMove(Vector3.zero, transitionDuration).SetEase(transitionEase).SetDelay(0.1f);
        rotateTween = transform.DOLocalRotate(Vector3.zero, transitionDuration).SetEase(transitionEase);
        fovTween = cam.DOFieldOfView(targetFOV, transitionDuration).SetEase(transitionEase);
    }

    private void KillTweens()
    {
        moveTween?.Kill();
        rotateTween?.Kill();
        fovTween?.Kill();
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        transform.DOShakePosition(duration, magnitude, vibrato: 10, randomness: 90, snapping: false, fadeOut: true)
                 .SetRelative(true);
    }
}
