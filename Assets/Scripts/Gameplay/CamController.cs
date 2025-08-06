using UnityEngine;
using DG.Tweening;

public class CamController : MonoBehaviour
{
    public static CamController Instance { get; private set; } // Singleton

    [Header("View Settings")]
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private Ease transitionEase = Ease.InOutSine;

    [Header("View Parents")]
    [SerializeField] private Transform fightViewParent;
    [SerializeField] private Transform shopViewParent;

    [Header("Camera Settings")]
    [SerializeField] private Camera cam, cam2;

    [Header("View Objects")]
    [SerializeField] private GameObject[] shopViewObjects;

    private Tween moveTween, rotateTween;

    private void Start()
    {
        Instance = this; // Set singleton

        InitializeShopView();
    }

    private void InitializeShopView()
    {
        cam.orthographic = true;
        cam.orthographicSize = 25;
        cam2.orthographic = true;
        cam2.orthographicSize = 25;

        cam.transform.SetParent(shopViewParent);
        cam.transform.localPosition = Vector3.zero;
        cam.transform.localRotation = Quaternion.identity;

        foreach (GameObject go in shopViewObjects)
            go.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            SetFightView();

        if (Input.GetKeyDown(KeyCode.S))
            SetShopView();
    }

    public void SetFightView()
    {
        cam.orthographic = false;
        cam.fieldOfView = 55;
        cam2.orthographic = false;
        cam2.fieldOfView = 55;

        foreach (GameObject go in shopViewObjects)
            go.SetActive(false);

        cam.transform.SetParent(fightViewParent);
        SmoothTransitionToParent(30f);
    }

    public void SetShopView()
    {
        cam.orthographic = true;
        cam.orthographicSize = 25;
        cam2.orthographic = true;
        cam2.orthographicSize = 25;

        foreach (GameObject go in shopViewObjects)
            go.SetActive(true);

        cam.transform.SetParent(shopViewParent);
        SmoothTransitionToParent(25f);
    }

    private void SmoothTransitionToParent(float targetFOV)
    {
        moveTween?.Kill();
        rotateTween?.Kill();

        moveTween = cam.transform.DOLocalMove(Vector3.zero, transitionDuration).SetEase(transitionEase);
        rotateTween = cam.transform.DOLocalRotate(Vector3.zero, transitionDuration).SetEase(transitionEase);

        cam2.transform.DOLocalMove(Vector3.zero, transitionDuration).SetEase(transitionEase);
        cam2.transform.DOLocalRotate(Vector3.zero, transitionDuration).SetEase(transitionEase);

        if (!cam.orthographic)
        {
            cam.DOFieldOfView(targetFOV, transitionDuration).SetEase(transitionEase);
            cam2.DOFieldOfView(targetFOV, transitionDuration).SetEase(transitionEase);
        }
    }

    // ✅ CAMERA SHAKE HANDLER
    private bool isShaking = false;

    public void CameraShake(float duration = 0.5f, float strength = 0.25f, int vibrato = 7, float punch = 0.25f)
    {
        if (isShaking || cam == null) return;

        isShaking = true;

        cam.transform.DOShakePosition(duration, strength, vibrato, 0.45f);
        cam.transform.DOPunchRotation(Vector3.forward * punch, duration, vibrato, 0.65f)
            .OnComplete(() => isShaking = false);
    }
}
