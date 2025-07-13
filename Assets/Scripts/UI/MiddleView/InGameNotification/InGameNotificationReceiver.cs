using System.Collections.Generic;
using UnityEngine;

public abstract class InGameNotificationReceiver: MonoBehaviour
{
    private List<InGameNotificationTriggerer> receivedNotifications = new List<InGameNotificationTriggerer>();

    public void RecevieNotification(InGameNotificationTriggerer triggerer)
    {
        if(!receivedNotifications.Contains(triggerer))
        {
            receivedNotifications.Add(triggerer);
            OnNotificationUpdate(true);
        }
    }

    public bool IsNotificationAvailable()
    {
        return receivedNotifications.Count > 0;
    }

    public void CancelNotification(InGameNotificationTriggerer triggerer)
    {
        if(receivedNotifications.Contains(triggerer))
        {
            receivedNotifications.Remove(triggerer);

            if(!IsNotificationAvailable())
            {
                OnNotificationUpdate(false);
            }
        }
    }

    protected virtual void OnNotificationUpdate(bool isAvaiable) { }
}
