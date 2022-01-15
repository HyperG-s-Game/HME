using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;

namespace WolfGamer.Hold_My_Eggs{
    public class CoinCollecter : MonoBehaviour {
        public static CoinCollecter i;


        [SerializeField] private CoinDataSO coinData;

        
        private int collectedCoinsCount;

        private void Awake() {

            i = this;
        }
        public void CollectCoin(int amount){
            collectedCoinsCount += amount;
               
        }
        public void GiveCoinToPlayer(){
            AnalyticsEvent.Custom("Coin Data",new Dictionary<string,object>{
                {"Coin Collected",collectedCoinsCount},
                {"Level",GameHandler.i.GetLevelData.sceneIndex}
            });
            coinData.AddCoin(collectedCoinsCount);
            UImanager.imanager.ShowTotalCoinAmount();
            coinData.Save();
        }
        public int GetCollectedCoinCount(){
            return collectedCoinsCount;
        }

        
    }

}