using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppUpdate : MonoBehaviour
{
    AppUpdateManager appUpdateManager;
    // Start is called before the first frame update
    void Start()
    {
        appUpdateManager = new AppUpdateManager();
        StartCoroutine(CheckForUpdate());
    }
    IEnumerator CheckForUpdate()
    {
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
            // IsUpdateTypeAllowed(), etc. and decide whether to ask the user
            // to start an in-app update.
            var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions(allowAssetPackDeletion: true);
            var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfoResult,appUpdateOptions);
            yield return startUpdateRequest;
        }
        else
        {
            // Log appUpdateInfoOperation.Error.
        }
    }
}
