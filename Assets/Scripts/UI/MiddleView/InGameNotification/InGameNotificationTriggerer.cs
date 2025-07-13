using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InGameNotificationTriggerer: MonoBehaviour
{
    [SerializeField] protected InGameNotificationReceiver receiver;

    public void TriggerNotification()
    {
        receiver.RecevieNotification(this);
    }

    public virtual void TriggerNotification(InGameNotificationReceiver rc)
    {
        rc.RecevieNotification(this);
    }

    public void CancelNotification()
    {
        receiver.CancelNotification(this);
    }

    public void CancelNotification(InGameNotificationReceiver rc)
    {
        rc.CancelNotification(this);
    }
}
