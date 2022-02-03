using UnityEngine;
using WolfGamer.Utils;
using Lean.Localization;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.Analytics;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


namespace WolfGamer.Hold_My_Eggs{
    public class GameHandler : MonoBehaviour{
        #region Public Variables.

        [Header("Language Settings")] [SerializeField]
        private string[] languageArray;

        [SerializeField] private SettingsSO settingsSO;
        [SerializeField] private LeanLocalization localization;

        [Header("Game Events")] [SerializeField]
        private UnityEvent onGameStartEvents;

        [SerializeField]
        private UnityEvent onGamePlayingEvents, onEndEvents, onPlayerWinEvents, onPlayerLossEvents, OnGamePaused;

        [Header("References")] [SerializeField]
        private GetGooglePlayData getAchivmentData;

        [SerializeField] private UIPopUpWindow eggColorChangingCanvas;
        [SerializeField] private LevelGenerator levelGenerator;
        [SerializeField] private LevelDataSO levelData;
        [SerializeField] private CamerFollow cameraFollow;

        [Space(20)] [Header("Reviving Stats")] [SerializeField]
        private int maxReviveCount = 3;

        [SerializeField] private bool canShowAd = false, isShowingAds = false;

        #endregion

        #region Private Varialbes..

        private List<Bowls> bowlsList;
        private Bowls activeBowl;

        private UImanager uImanager;

        private Egg egg;
        private bool isGameOver, isGameStart, isWinn, isLoos;
        private bool hasAddInGame;

        #endregion

        #region Singelton.

        public static GameHandler i { get; private set; }

        private void Awake(){
            hasAddInGame = settingsSO.settingsData.hasAdInGame;
            if (i == null){
                i = this;
            } else{
                Destroy(i.gameObject);
            }
        }

        #endregion


        #region Initial Setup.........

        private void Start(){
            // Getting the Refernce to the Ad Manager and Ui Manager.
            // adManager = AdManager.current;
            
            uImanager = UImanager.imanager;
            // Localization for the Levels Scene..
            localization.CurrentLanguage = languageArray[settingsSO.settingsData.currentLanguageIndex];


            bowlsList = new List<Bowls>();
            // Creating the Level from the Level Generator.
            levelGenerator.CreateLeveL(levelData);
            LevelLoader.instance.UpdateLevelData(levelData);

            uImanager.SetLiveCount(maxReviveCount);
            StartCoroutine(nameof(StartGameRoutine));

            if (AdController.instance.IsRewardedAdLoaded()){
                SetCanShowAd(true);
                SetShowingAd(false);
            }
        }


        public void SetEggColorCanvasActive(){
            if (levelData.levelBowlsType.Count > 1){
                eggColorChangingCanvas.PopUp();
            }else{
                eggColorChangingCanvas.gameObject.SetActive(false);
            }
        }

        public void SetBowlSpeed(){
            float speed = UnityEngine.Random.Range(levelData.minBowlSpeed, levelData.maxBowlSpeed);
            for (int i = 0; i < bowlsList.Count; i++){
                bowlsList[i].SetSpeed(speed);
            }
        }

        private void StartMovingBowls(bool s){
            for (int i = 0; i < bowlsList.Count; i++){
                bowlsList[i].StartGame(s);
            }
        }


        public void SetEgg(Egg egg){
            this.egg = egg;
            onGameStartEvents.AddListener(() => { egg.SetJumping(false); });
            onGamePlayingEvents.AddListener(() => { egg.SetJumping(true); });
            onEndEvents.AddListener(() => { egg.SetJumping(false); });
        }

        #endregion

        #region Game Loop................

        private IEnumerator StartGameRoutine(){
            onGameStartEvents?.Invoke();
            uImanager.SetLiveCount(maxReviveCount);
            while (!isGameStart){
                yield return null;
            }

            cameraFollow.SetTarget(egg.transform);
            yield return StartCoroutine(GamePlayRoutine());
        }

        private IEnumerator GamePlayRoutine(){
            onGamePlayingEvents?.Invoke();
            cameraFollow.StartFollowTarget(true);
            StartMovingBowls(true);
            while (!isGameOver){
                uImanager.SetCoinText();
                if (egg.GetIsDead){
                    if (maxReviveCount > 0){
                        Debug.Log("Revive The Player");
                        yield return StartCoroutine(ReviveRotine());
                    }else if (maxReviveCount <= 0){
                        if (!canShowAd){
                            if (!isShowingAds){
                                yield return new WaitForSeconds(1f);
                                SetGameOver(false);
                                // break;
                            }
                        }else{
                            yield return new WaitForSeconds(0.4f);
                            uImanager.ShowHidAdWindow(true);
                            // Open the ad view window if can Show ad.
                            Debug.Log("Can try for another ad...");
                        }
                    }

                }
                else{
                    SetShowingAd(false);
                    uImanager.ShowHidAdWindow(false);
                }

                yield return null;
            }

            SetShowingAd(false);
            uImanager.ShowHidAdWindow(false);
            onEndEvents?.Invoke();
            StartMovingBowls(false);
            yield return new WaitForSeconds(0.5f);
            cameraFollow.StartFollowTarget(false);
            if (isWinn){
                Debug.Log("Revive The Player");
                
                onPlayerWinEvents?.Invoke();
                getAchivmentData.SetAchivementData(levelData);
            }

            if (isLoos){
                AudioManager.i.PlayOneShotMusic(SoundType.Game_Loss);
                onPlayerLossEvents?.Invoke();
            }
            AnalyticsResult result = Analytics.CustomEvent("Level Data",new Dictionary<string,object>{
                {string.Concat("Level win",levelData.sceneIndex),isWinn},
                {string.Concat("Level Loos",levelData.sceneIndex),isLoos},
            });
            ShowInterstetialAds();
            AdController.instance.RequestBanner();
            Debug.Log(result + " from " + this.name);
        }

        public void GetExtraEgg(){
            // Give Player an Extra Egg After Watching and Reward Add.
            egg.SetDead(false);
            AudioManager.i.PlayMusic(SoundType.Main_Sound);
            StartCoroutine(ReviveRotine());
            // adManager.LoadRewardAd();
        }


        private IEnumerator ReviveRotine(){
            StartMovingBowls(false);
            ResetTimerInBowls();
            yield return new WaitForSeconds(0.5f);
            egg.Revive(activeBowl);
            yield return new WaitForSeconds(0.4f);

            StartMovingBowls(true);
            if (isGameOver)
            {
                isGameOver = false;
            }

            maxReviveCount--;
            uImanager.SetLiveCount(maxReviveCount);
        }

        private void ResetTimerInBowls(){
            for (int i = 0; i < bowlsList.Count; i++){
                bowlsList[i].startTimer = false;
            }
        }

        #endregion

        #region Public Methods...................

        public void SetCurrentBowl(Bowls _boel){
            activeBowl = _boel;
        }


        public void StartGame(){
            isGameStart = true;
        }


        public void SetGameOver(bool _isWinn){
            // Set Game Over (is Win == false) if the Player has No Life and Can Not Show Ads...

            if (_isWinn){
                isLoos = false;
                isWinn = true;

                levelData.Save();
            }
            else{
                isWinn = false;
                isLoos = true;
            }

            
            levelData.SetLevelComplete(isWinn);
            LevelLoader.instance.UpdateLevelData(levelData);
            StartMovingBowls(false);
            isGameOver = isLoos | isWinn;
        }

        private void ShowInterstetialAds(){
            // int rand = UnityEngine.Random.Range(0,6);
            if (levelData.sceneIndex != SceneIndex.Level_1){
                if(hasAddInGame){
                    // Shwoing Interstetial Ads if game has any ads...
                    // No premium Membership is purchased..
                    AdController.instance.ShowInterstitialAd();
                }
            }
            AnalyticsResult result = Analytics.CustomEvent("Ad Data",new Dictionary<string,object>{
                {"Interstetial Ads",levelData.sceneIndex},
                
            });
        }


        public void NextLevel(){
            // Move to Next Level...
            AdController.levelRewardAd = true;
            AdController.instance.ShowRewardedAd();
            // LevelLoader.instance.MoveToNextLevel();

            int levelNumber = (int)levelData.sceneIndex;
            PlayGamesController.PostToLeaderboard(levelNumber);
        }

        public void Restart(){
            // Restart the Level...
            if (levelData.levelSaveData.isCompleted){
                levelData.SetLevelComplete(false);
            }

            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        public void GamePaused(){
            OnGamePaused?.Invoke();
        }

        public void SetCanShowAd(bool _hasAd){
            // Set If reward Ad is Loaded .
            canShowAd = _hasAd;
        }


        public void AddToBowlsList(Bowls _b){
            bowlsList.Add(_b);
        }

        public void ChangeToWhiteEgg(){
            egg.ChangeEggTypeWithJump(BowlsType.White);
        }

        public void ChangeToRedEgg(){
            egg.ChangeEggTypeWithJump(BowlsType.Red);
        }

        public void ChangeToGreenEgg(){
            egg.ChangeEggTypeWithJump(BowlsType.Green);
        }

        public void ChangeToBlueEgg(){
            egg.ChangeEggTypeWithJump(BowlsType.Blue);
        }

       

        public bool GetHasAdd
        {
            get { return hasAddInGame; }
        }

        public LevelDataSO GetLevelData{
            get { return levelData; }
        }

        public void SetJumpingOfEgg(bool jump){
            egg.SetJumping(jump);
        }

        public void SetShowingAd(bool _value){
            // set if rewarded Ad is Playing...
            isShowingAds = _value;
        }
        public void IncreasePlayerDeath(int count){
            levelData.SetDeathCount(count);
        }
        private void OnDestroy(){
            AdController.instance.HideBanner();
        }
        public Egg GetEgg(){
            return egg;
        }
        public List<Bowls> GetBowlsList(){
            return bowlsList;
        }

        #endregion
    }
}