using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WolfGamer.Utils;
using UnityEngine.Analytics;
using System.Collections.Generic;

namespace WolfGamer.Hold_My_Eggs{
    public class UImanager : MonoBehaviour {


        public static UImanager imanager{get;private set;}
        [SerializeField] private CoinDataSO coinData;
        [SerializeField] private TextMeshProUGUI totalCoinAmountText;
        [SerializeField] private GameObject adWindow,pauseButton;
        [SerializeField] private TextMeshProUGUI[] levelNumbersText;
        [SerializeField] private Button[] menuButton;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private Image[] liveEggsImages;
        [SerializeField] private Sprite crackedEggSprite;

        public static bool isGamePaused;
        private void Awake(){
            if(imanager == null){
                imanager = this;
            }
        }
        public void PauseBGM(){
            AudioManager.i.PauseMusic(SoundType.Main_Sound);
        }
        
        private void Start(){
            
            foreach(Button b in menuButton){
                b.onClick.AddListener(() =>{
                    Play_Pause(false);
                    LevelLoader.instance.SwitchScene(SceneIndex.Main_Menu);
                });
            }
            SetCoinText();
            SetLevelName();
            Play_Pause(false);
        }
        private void SetLevelName(){
            string levelName = GameHandler.i.GetLevelData.name;
            for (int i = 0; i < levelNumbersText.Length; i++){
                levelNumbersText[i].SetText(levelName);
            }
        }
        /*
        public void SetJumpForTutorilButton(Egg egg){
            egg.ChangeEggTypeWithJump(BowlsType.White);
        }
        */

        private int adWatchCount = 0;
        public void ShowRewardedAd(){
            adWatchCount++;
            AdController.extraEgg = true;
            AdController.instance.ShowRewardedAd();
            AnalyticsResult result = Analytics.CustomEvent("Ad Data",new Dictionary<string,object>{
                {"Rewarded Ads",adWatchCount},
                
            });
            Debug.Log(result + "From " + this.name);
        }
        
        
        public void ShowHidAdWindow(bool _show){
            if(_show){
                if(!isGamePaused){
                    pauseButton.SetActive(false);
                }
                if(!adWindow.activeInHierarchy){
                    adWindow.SetActive(true);
                }

            }else{
                if(!isGamePaused){
                    pauseButton.SetActive(true);
                }
                if(adWindow.activeInHierarchy){
                    adWindow.SetActive(false);
                }
            }
        }
        public void SetLiveCount(int _livesCount){
            for (int i = 0; i < liveEggsImages.Length; i++){
                if(i >= _livesCount){
                    liveEggsImages[i].sprite = crackedEggSprite;
                }
            }
        }
        public void ShowTotalCoinAmount(){
            totalCoinAmountText.SetText(coinData.GetCoinAmount().ToString());
        }

        public void Play_Pause(bool isPause){
            isGamePaused = isPause;
            if(isGamePaused){
                AudioManager.i.PauseMusic(SoundType.Main_Sound);
                Time.timeScale = 0f;
                GameHandler.i.GamePaused();
            }else{
                AudioManager.i.PlayMusic(SoundType.Main_Sound);
                Time.timeScale = 1f;
            }
        }
        public void SetCoinText(){
            coinText.SetText(CoinCollecter.i.GetCollectedCoinCount().ToString());
        }
        public void PlayButtonClickSound(){
            AudioManager.i.PlayOneShotMusic(SoundType.ButtonClickSound);
        }

        public void MoveToNextLevel(){
            LevelLoader.instance.MoveToNextLevel();
        }
        
    }
}
