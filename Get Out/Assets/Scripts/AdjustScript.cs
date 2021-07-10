using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.adjust.sdk;
using Facebook.Unity;

public class AdjustScript : MonoBehaviour
{
    AdjustEvent lvlStart = new AdjustEvent("Level Start");
    AdjustEvent lvlRestart = new AdjustEvent("Level Restart");
    AdjustEvent lvlFail = new AdjustEvent("Level Failed");
    AdjustEvent lvlComplete = new AdjustEvent("Level Complete");

    //void Awake()
    //{
    //    //4.28
    //    // import this package into the project:
    //    // https://github.com/adjust/unity_sdk/releases

    //    InitAdjust("m8y9xfi7dqf4");
    //    if (FB.IsInitialized)
    //    {
    //        FB.ActivateApp();
    //    }
    //    else
    //    {
    //        //Handle FB.Init
    //        FB.Init(() => {
    //            FB.ActivateApp();
    //        });
    //    }
    //}

    void Awake()
    {
        InitAdjust("m8y9xfi7dqf4");
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            Debug.Log("Adjust Script");
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }


    private void InitAdjust(string adjustAppToken)
    {
        var adjustConfig = new AdjustConfig(
            adjustAppToken,
            AdjustEnvironment.Production, // AdjustEnvironment.Sandbox to test in dashboard
            true
        );
        adjustConfig.setLogLevel(AdjustLogLevel.Info); // AdjustLogLevel.Suppress to disable logs
        adjustConfig.setSendInBackground(true);
        new GameObject("Adjust").AddComponent<Adjust>(); // do not remove or rename

        // Adjust.addSessionCallbackParameter("foo", "bar"); // if requested to set session-level parameters

        //adjustConfig.setAttributionChangedDelegate((adjustAttribution) => {
        //  Debug.LogFormat("Adjust Attribution Callback: ", adjustAttribution.trackerName);
        //});

        Adjust.start(adjustConfig);
    }

    public void LogAchieveLevelEvent(string level)
    {
        var parameters = new Dictionary<string, object>();
        parameters[AppEventParameterName.Level] = level;
        FB.LogAppEvent(AppEventName.AchievedLevel, null, parameters);
    }

    public void Adjust_LvlStart()
    {
        Adjust.trackEvent(lvlStart);
    }

    public void Adjust_LvlRestart()
    {
        Adjust.trackEvent(lvlRestart);
    }

    public void Adjust_LvlFail()
    {
        Adjust.trackEvent(lvlFail);
    }

    public void Adjust_LvlComplete()
    {
        Adjust.trackEvent(lvlComplete);
    }
}
