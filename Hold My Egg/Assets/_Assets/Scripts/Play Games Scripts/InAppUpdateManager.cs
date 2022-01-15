using TMPro;
using UnityEngine;
using System.Collections;
using Google.Play.Common;
using Google.Play.AppUpdate;
public class InAppUpdateManager : MonoBehaviour {
    [SerializeField] private GameObject mainCanvas,updateCanvas;

    private AppUpdateManager updateManager;
    private AppUpdateInfo appupdateInfoResut;
    private AppUpdateOptions appupdateOptions;
    private void Start(){
        #if !UNITY_EDITOR
        StartCoroutine(CheckUpdate());
        #endif
    }
    private IEnumerator CheckUpdate(){
        PlayAsyncOperation<AppUpdateInfo,AppUpdateErrorCode> appupdateInfoOperation = updateManager.GetAppUpdateInfo();
        yield return appupdateInfoOperation;

        if(appupdateInfoOperation.IsSuccessful){
            appupdateInfoResut = appupdateInfoOperation.GetResult();
            if(appupdateInfoResut.UpdateAvailability == UpdateAvailability.UpdateAvailable){
                updateCanvas.SetActive(true);
                mainCanvas.SetActive(false);
            }else{
                updateCanvas.SetActive(false);
                mainCanvas.SetActive(true);
            }

        }
        appupdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
        yield return StartCoroutine(StartImediate(appupdateInfoResut,appupdateOptions));

    }
    public void StartImediateUpdate(){
        #if !UNITY_EDITOR
        #endif
    }
    private IEnumerator StartImediate(AppUpdateInfo appUpdateInfo, AppUpdateOptions updateOptions){
        var StartUpdateRequest = updateManager.StartUpdate(appUpdateInfo,updateOptions);
        yield return StartUpdateRequest;
    }

    
}
