using System;
using UnityEngine;
using GoogleMobileAds.Api;
using WolfGamer.Hold_My_Eggs;
using WolfGamer.Utils;

public class AdController : MonoBehaviour{
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
        interstitialAd?.Destroy();
        interstitialAd = new InterstitialAd(interstitialId);

        interstitialAd.OnAdClosed += (sender, args) => RequestInterstitial();

        RequestInterstitial();


        rewardedAd = new RewardedAd(rewardedId);
        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;
        RequestRewardedAd();
        isInitialized = true;
    }

    private AdRequest RequestAd(){
        if (npa == 1){
            return new AdRequest.Builder().AddExtra("npa", "1").Build();
        }

        return new AdRequest.Builder().Build();
    }

    private void RequestInterstitial(){
        interstitialAd.LoadAd(RequestAd());
    }

    private void RequestRewardedAd(){
        rewardedAd.LoadAd(RequestAd());
    }

    public void ShowInterstitialAd(){
        if (interstitialAd.IsLoaded()){
            interstitialAd.Show();
            Firebaseanayltics.current.SetAdImpressionDataAnayltyics();
        }
        else{
            RequestInterstitial();
            if (interstitialAd.IsLoaded()){
                interstitialAd.Show();
            }
            else {
                Debug.Log("Interstitial is not loaded");
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
            Firebaseanayltics.current.SetAdImpressionDataAnayltyics();
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
            }
        }
    }

    #region RewardedHandle

    public void HandleRewardedAdLoaded(object sender, EventArgs args){
        if(GameHandler.i != null){
            GameHandler.i.SetCanShowAd(true);
        }
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args){
        RequestRewardedAd();
        GameHandler.i.SetShowingAd(false);
        GameHandler.i.SetGameOver(false);
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
    
    
    
}