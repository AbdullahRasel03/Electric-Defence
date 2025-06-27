using UnityEngine;
using UnityEngine.Events;

public class AnimationEventTrigger : MonoBehaviour
{
    public UnityEvent OnFire;

    public void TriggerEvent()
    {
        OnFire?.Invoke();
    }
}
