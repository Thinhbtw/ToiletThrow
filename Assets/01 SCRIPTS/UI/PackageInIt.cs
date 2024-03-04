using Mycom.Tracker.Unity;
using UnityEngine;

public class PackageInIt : MonoBehaviour
{
    public void Awake()
    {
        #if !UNITY_IOS && !UNITY_ANDROID
                return;
        #endif

                // Setting up the configuration if needed
                var myTrackerConfig = MyTracker.MyTrackerConfig;
                // ...
                // Setting up params
                // ...

                // Initialize the tracker
        #if UNITY_IOS
                MyTracker.Init("SDK_KEY_IOS");
        #elif UNITY_ANDROID
                MyTracker.Init("40936470485924556501");
        #endif
    }
}
