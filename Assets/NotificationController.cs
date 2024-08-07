using System.Collections;
using System.Collections.Generic;
using OneSignalSDK;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    //a34667ae-828e-498a-b4ec-dfe0f5fd4682
    // Start is called before the first frame update
    void Start()
    {
        // Replace 'YOUR_ONESIGNAL_APP_ID' with your OneSignal App ID from app.onesignal.com
        OneSignal.Initialize("a34667ae-828e-498a-b4ec-dfe0f5fd4682");
        PromptForPush();
    }

    public async void PromptForPush() {
        
        Debug.Log($"Can request push notification permission: {OneSignal.Notifications.CanRequestPermission}");

        if (OneSignal.Notifications.CanRequestPermission)
        {
            Debug.Log("Opening permission prompt for push notifications and awaiting result...");

            var result = await OneSignal.Notifications.RequestPermissionAsync(true);

            if (result)
                Debug.Log("Notification permission accepeted");
            else
                Debug.Log("Notification permission denied");
        }
    }
}
