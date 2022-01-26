using System;
using UnityEngine;
using GoogleMobileAds.Api;
using WolfGamer.Hold_My_Eggs;
using WolfGamer.Utils;
using GameAnalyticsSDK;

public class AdController : MonoBehaviour, IGameAnalyticsATTListener
{
    public static AdController instance;

    private readonly string bannerId = "ca-app-pub-1447736674902262/9281669086";
    private readonly string interstitialId = "ca-app-pub-1447736674902262/5423082728";
    private readonly string rewardedId = "ca-app-pub-1447736674902262/8141224339";


    private readonly string appId = "ca-app-pub-1447736674902262~4950910632";
    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private int npa;
    private bool isInitialized = false;
    public static bool levelRewardAd = false;
    public static bool extraEgg = false;
    private string currentRewardedVideoPlacement;

    private void Awake(){
        DontDestroyOnLoad(this);
        if (instance == null){
            instance = this;
        }
        else {
            Destroy(gameObject);

        }
    }


    #region old Ads System.....

    private void Start(){
        if (!PlayerPrefs.HasKey("npa")){
            TACanvasController.CallOnAgreeEvent();
        }
        else{
            InitializeSdk();
        }
        
    }

    public void InitializeSdk(){
        npa = PlayerPrefs.GetInt("npa");
        var requestConfiguration = new RequestConfiguration.Builder().SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.False).build();

        MobileAds.SetRequestConfiguration(requestConfiguration);

        MobileAds.Initialize(initStatus => {
            var map = initStatus.getAdapterStatusMap();
            foreach (var keyValuePair in map){
                var className = keyValuePair.Key;
                var status = keyValuePair.Value;
                switch (status.InitializationState)
                {
                    case AdapterState.NotReady:
                        Debug.Log("Adapter: " + className + " not ready.");
                        break;
                    case AdapterState.Ready:
                        Debug.Log("Adapter: " + className + " is initialized.");
                        break;
                }
            }

            InitializeAd();
        });

    }

    private void InitializeAd(){
        RequestInterstitial();


        rewardedAd = new RewardedAd(rewardedId);
        GameAnalyticsILRD.SubscribeAdMobImpressions(rewardedId, rewardedAd);
        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;
        RequestRewardedAd();
        isInitialized = true;

        if (Application.platform == RuntimePlatform.IPhonePlayer){
            GameAnalytics.RequestTrackingAuthorization(this);
        }else{
            GameAnalytics.Initialize();
        }

    }

    private AdRequest RequestAd(){
        if (npa == 1){
            return new AdRequest.Builder().AddExtra("npa", "1").Build();
        }

        return new AdRequest.Builder().Build();
    }

    private void RequestInterstitial(){
#if UNITY_ANDROID
        string adUnitId = interstitialId;
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitialAd = new InterstitialAd(adUnitId);
        GameAnalyticsILRD.SubscribeAdMobImpressions(adUnitId, interstitialAd);
        // Called when an ad request has successfully loaded.
        this.interstitialAd.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitialAd.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitialAd.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
//        this.interstitialAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitialAd.LoadAd(request);
    }

    private void RequestRewardedAd(){
        rewardedAd.LoadAd(RequestAd());
    }

    public void ShowInterstitialAd(){
        if (interstitialAd.IsLoaded()){
            interstitialAd.Show();
        }else{
            RequestInterstitial();
            if (interstitialAd.IsLoaded()){
                interstitialAd.Show();
            }
            else {
                Debug.Log("Interstitial is not loaded");
                GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.Interstitial, "admob", interstitialId);
            }
        }
    }

    public bool IsRewardedAdLoaded(){
        return rewardedAd.IsLoaded();
    }

    public void ShowRewardedAd(){
        if (rewardedAd.IsLoaded()){
            rewardedAd.Show();
            
            GameHandler.i.SetShowingAd(true);
            // Firebaseanayltics.current.SetAdImpressionDataAnayltyics();
        }else{
            RequestRewardedAd();
            if (rewardedAd.IsLoaded()){
                rewardedAd.Show();
                GameHandler.i.SetShowingAd(true);
            }
            else{
                GameHandler.i.SetShowingAd(false);
                Debug.Log("RewardedAd is not loaded");
                if (levelRewardAd){
                    LevelLoader.instance.MoveToNextLevel();
                    levelRewardAd = false;
                }else if (extraEgg){
                    extraEgg = false;
                }
                GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, "admob", rewardedId);
            }
        }
    }


    #region InterstitialBannerHandle
    public void HandleOnAdLoaded(object sender, EventArgs args){
        Debug.Log("HandleAdLoaded event received");
        Debug.Log("args :- " + sender.ToString());
        // if (sender.Equals("GoogleMobileAds.Api.InterstitialAd")){

        // }
        if (sender.Equals("GoogleMobileAds.Api.bannerView")){
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Banner, "admob", bannerId);
        }
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args){
        Debug.Log("HandleFailedToReceiveAd event received with message: "+ args.ToString());
    }

    public void HandleOnAdOpened(object sender, EventArgs args){
        Debug.Log("HandleAdOpened event received");
        // send ad event
        GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.Banner, "admob", bannerId);
    }

    public void HandleOnAdClosed(object sender, EventArgs args){
        Debug.Log("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args){
        Debug.Log("HandleAdLeavingApplication event received");
        // send ad event - ad click
        GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.Interstitial, "admob", interstitialId);
    }

    #endregion

    #region RewardedHandle

    public void HandleRewardedAdLoaded(object sender, EventArgs args){
        if(GameHandler.i != null){
            GameHandler.i.SetCanShowAd(true);
        }
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args){
        RequestRewardedAd();
        GameHandler.i.SetShowingAd(false);
        GameHandler.i.SetCanShowAd(false);
        if (!rewardedAd.IsLoaded()){
            RequestRewardedAd();
        }
        else{
            GameHandler.i.SetCanShowAd(true);
        }
        // if (levelRewardAd){
        //     LevelLoader.instance.MoveToNextLevel();
        //     levelRewardAd = false;
        // }
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args){
        AudioManager.i.PauseMusic(SoundType.Main_Sound);
        // keep track of current rewarded video ad
        currentRewardedVideoPlacement = rewardedId;
        // start timer for this ad identifier
        GameAnalytics.StartTimer(currentRewardedVideoPlacement);
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args){
        if(!rewardedAd.IsLoaded()){
            RequestRewardedAd();
        }
        GameHandler.i.SetShowingAd(false);
        GameHandler.i.SetGameOver(false);
        if (levelRewardAd){
            LevelLoader.instance.MoveToNextLevel();
            levelRewardAd = false;
        }
        
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args){
        if (!rewardedAd.IsLoaded()){
            RequestRewardedAd();
        }
        if (levelRewardAd){
            LevelLoader.instance.MoveToNextLevel();
            levelRewardAd = false;
        }

        if (currentRewardedVideoPlacement != null)
        {
            long elapsedTime = GameAnalytics.StopTimer(currentRewardedVideoPlacement);
            // send ad event for tracking elapsedTime
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, "admob", rewardedId, elapsedTime);
            currentRewardedVideoPlacement = null;
        }

    }

    private void HandleUserEarnedReward(object sender, Reward args){
        if (!levelRewardAd && extraEgg){
            GameHandler.i.SetShowingAd(false);
            GameHandler.i.SetCanShowAd(false);
            GameHandler.i.GetExtraEgg();
            Debug.Log("Ads Reward Completed");
            extraEgg = false;
        }
        // GameHandler.i.SetShowingAd(false);
        // GameHandler.i.SetCanShowAd(false);
        // GameHandler.i.GetExtraEgg();
        // Debug.Log("Ads Reward Completed");
        RequestRewardedAd();

        // send ad event - reward recieved
        GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, "admob", rewardedId);

    }
    #endregion

    public void RequestBanner()
    {
        if (isInitialized)
        {
#if UNITY_ANDROID
            string adUnitId = bannerId;
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif



            // Create a 320x50 banner at the top of the screen.
            this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
            GameAnalyticsILRD.SubscribeAdMobImpressions(adUnitId, bannerView);

            // Called when an ad request has successfully loaded.
            this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
            // Called when an ad request failed to load.
            this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
            // Called when an ad is clicked.
            this.bannerView.OnAdOpening += this.HandleOnAdOpened;
            // Called when the user returned from the app after an ad click.
            this.bannerView.OnAdClosed += this.HandleOnAdClosed;
            // Called when the ad click caused the user to leave the application.
//            this.bannerView.OnAdLeavingApplication += this.HandleOnAdLeavingApplication;
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();

            // Load the banner with the request.
            this.bannerView.LoadAd(request);
        }
    }

    public void HideBanner(){
        if (bannerView != null){
            bannerView.Destroy();
        }
    }

    #endregion

    #region GameAnalytics

    public void GameAnalyticsATTListenerNotDetermined(){
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerRestricted(){
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerDenied(){
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerAuthorized(){
        GameAnalytics.Initialize();
    }

    #endregion

    // when application goes to background (during the display of a rewarded video ad) then the timer needs to stop.
    // therefore we need to call code in Unity method OnApplicationPause.
    void OnApplicationPause(bool paused){
        if (paused){
            if (currentRewardedVideoPlacement != null){
                GameAnalytics.PauseTimer(currentRewardedVideoPlacement);
            }
        }else{
            if (currentRewardedVideoPlacement != null){
                GameAnalytics.ResumeTimer(currentRewardedVideoPlacement);
            }
        }

    }



}