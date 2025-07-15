using UnityEngine;

public class UiController : MonoBehaviour
{
    [SerializeField] private Camera uiCam;
    public RectTransform coinTransform;
    public RectTransform gemTransform;

    protected virtual void Start()
    {
        UiManager uiManager = UiManager.GetInstance();
        if (uiManager == null)
        {
            BbsLog.LogError("UiManager Not FOUND!!");
            return;
        }
        uiManager.AttachUIController(this);
        OnStart();
    }

    internal virtual void OnStart() { }

    public Camera GetUICamera()
    {
        return uiCam;
    }
}
