using UnityEngine;
// using Firebase.Analytics;
namespace WolfGamer.Hold_My_Eggs{
    public class Firebaseanayltics : MonoBehaviour {
        
        #region singleton........
        public static Firebaseanayltics current;
        private void Awake(){
            if(current == null){
                current = this;
            }else{
                Destroy(current.gameObject);
            }
            DontDestroyOnLoad(current.gameObject);
        }

        #endregion



        private void Start(){
            // FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>{
            //     FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            // });
            // FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAppOpen);
        }

        // public void SetPlayerOnLevel(int _levelNumber){
        //     FirebaseAnalytics.LogEvent("Level_Data","level number",_levelNumber);
        //     Debug.Log("Firebase Events Loged for Level Number");
        // }
        public void SetCoinCollected(int coinAmount){
            // FirebaseAnalytics.LogEvent("Coin_Data","Coin Amount",coinAmount);
            Debug.Log("Firebase Events Loged for Coin Data");
        }
        public void SetPlayerDeathCount(int DeathCount){
            // FirebaseAnalytics.LogEvent("Death_Data","Death Number",DeathCount);
            Debug.Log("Firebase Events Loged for Death Count");
        }
            
        
        private void OnApplicationQuit(){
            SetviewTime();
        }
        public void SetPlayerLevelAnaylytics(int currentLevel){
            // FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelUp,"Level",currentLevel);
        }
        public void SetAdImpressionDataAnayltyics(){
            // FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAdImpression);
        }
        public void SetviewTime(){
            // FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventScreenView);
        }
        public void SetSpendRealCurrencyForCoins(int amount){
            // FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventPurchase,"Purchase Coin ",amount);
        }
                
                
    }     

}