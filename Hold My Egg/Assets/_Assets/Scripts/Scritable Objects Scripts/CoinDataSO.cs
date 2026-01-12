using System.IO;
using UnityEngine;

namespace WolfGamer.Hold_My_Eggs{
    [CreateAssetMenu(fileName = "coin",menuName = "ScriptableObject/Coin Collecter")]
    public class CoinDataSO : ScriptableObject {

        [SerializeField] private int totalCoinAmount;
        [SerializeField] private string savePath = "coinsAmount.json";

        public void AddCoin(int amount){
            totalCoinAmount += amount;
        }
        public int GetCoinAmount(){
            return totalCoinAmount;
        }

        [System.Serializable]
        private struct CoinSaveModel { public int totalCoinAmount; }

        [ContextMenu("Save")]
        public void Save(){
            var model = new CoinSaveModel{ totalCoinAmount = totalCoinAmount };
            string json = JsonUtility.ToJson(model, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, savePath), json);
        }

        [ContextMenu("Load")]
        public void Load(){
            string path = Path.Combine(Application.persistentDataPath, savePath);
            if(File.Exists(path)){
                string json = File.ReadAllText(path);
                var model = JsonUtility.FromJson<CoinSaveModel>(json);
                totalCoinAmount = model.totalCoinAmount;
            }
        }
    }
}