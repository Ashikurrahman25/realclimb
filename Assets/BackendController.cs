using FirebaseRestClient;
using RootMotion.Demos;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendController : MonoBehaviour
{

    public static BackendController Instance;

   
    RealtimeDatabase realTimeDatabase;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        realTimeDatabase = new RealtimeDatabase();
    }

    public void SubmitRequest(FormData form, Action ac)
    {
        string id = realTimeDatabase.GeneratePushID();
        realTimeDatabase.Child("ShippingData").Child(form.plEmail).Child(id).WriteValue(form).OnSuccess(() =>
        {
            ac?.Invoke();
        });
    }
}

