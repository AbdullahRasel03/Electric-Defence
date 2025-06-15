using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using MoreMountains.Tools;
using Unity.Notifications.Android;
#else
using Unity.Notifications.iOS;
#endif
using UnityEngine;

public class MobileNotificationController : MonoBehaviour
{   
#if UNITY_ANDROID
    
    void Start()
    {
        //Check if Platform Android
        if (Application.platform == RuntimePlatform.Android)
        {
            //Fire Android Notification
            InitializeNotificationChannelGroup();
            StartCoroutine(RequestNotificationPermission());
        }
    }
    
    #region AndroidNotification
    
    public void ScheduleNotification(string title, string text, int hours, int minute, int second)
    {
        SendNotification(title, text, hours, minute, second);
    }

    private void SendNotification(String title, String text, int hour, int minute, int second)
    {
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = text;
        notification.FireTime = DateTime.Now.AddHours(hour).AddMinutes(minute).AddSeconds(second);
        // notification.FireTime = DateTime.Now.AddMinutes(hour*60);

        AndroidNotificationCenter.SendNotification(notification, "C3R_Ret_Channel");
    }


    IEnumerator RequestNotificationPermission()
    {
        var request = new PermissionRequest();
        while (request.Status == PermissionStatus.RequestPending)
            yield return null;
        // here use request.Status to determine users response
        if (request.Status == PermissionStatus.Allowed)
        {
            //print("Permitted");
            SendNotification("Hey Genius!", "How Many Cards You Have Collected? Come and Collect Some Cards.", 6, 0, 0);
            SendNotification("You're Awesome!", "Continue Solving Those Brain Teasing Puzzels", 25, 0, 0);
        }
    }



    private void InitializeNotificationChannelGroup()
    {
        var group = new AndroidNotificationChannelGroup()
        {
            Id = "Main",
            Name = "CardScrewJam Notifications"
        };
        AndroidNotificationCenter.RegisterNotificationChannelGroup(group);
        var channel = new AndroidNotificationChannel()
        {
            Id = "C3R_Ret_Channel",
            Name = "CardScrewJam",
            Importance = Importance.High,
            Description = "CardScrewJam Notifications",
            Group = "Main",  // must be same as Id of previously registered group
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    #endregion
#else

    void Start()
        {
            //Check if Platform IoS
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //Fire IoS Notification
                try
                {
                    StartCoroutine(RequestIoSAuthorization());
                }
                catch
                {
                }
            }
        }
#region IoSNotification

    IEnumerator RequestIoSAuthorization()
    {
        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            };

            string res = "\n RequestAuthorization:";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
        }
        
        if(iOSNotificationCenter.GetNotificationSettings().AuthorizationStatus == AuthorizationStatus.Authorized)
        {
            SendIoSNotification(6, "Master Rick !", "Monsters Are Attacking Our Payload !");
            SendIoSNotification(25, "Hey Chief !", "We Need To Defead Those Monsters !");
        }

        yield return null;
    }

    private void SendIoSNotification(int hour, string title, string msg)
    {
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(hour, 0, 0),
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            // You can specify a custom identifier which can be used to manage the notification later.
            // If you don't provide one, a unique string will be generated automatically.
            Identifier = "_notification_01",
            Title = title,
            Body = msg,
            Subtitle = "!",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);
    }

    #endregion
#endif

    
}
