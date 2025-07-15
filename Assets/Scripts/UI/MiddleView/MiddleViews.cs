using UnityEngine;

public class MiddleViews : InGameNotificationTriggerer
{
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private CanvasGroup canvasGroup;

    protected virtual void Start()
    {
        RectTransform rect = parentCanvas.GetComponent<RectTransform>();
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y);
        OnStart();
    }

    protected virtual void OnStart() { }

    public virtual void OnViewSelected()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public virtual void OnViewUnSelected()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
