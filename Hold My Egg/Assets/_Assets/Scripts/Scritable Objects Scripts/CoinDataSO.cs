using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

namespace WolfGamer.Hold_My_Eggs{
    [CreateAssetMenu(fileName = "coin",menuName = "ScriptableObject/Coin Collecter")]
    public class CoinDataSO : ScriptableObject {
        
        [SerializeField] private int totalCoinAmount;
        [SerializeField] private string savePath = "coinsAmount.dat";
        
        public void AddCoin(int amount){
            totalCoinAmount += amount;
        }
        public int GetCoinAmount(){
            return totalCoinAmount;
        }
        [ContextMenu("Save")]
        public void Save(){
            string data = JsonUtility.ToJson(this,true);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(Application.persistentDataPath,"/",savePath));
            formatter.Serialize(file,data);
            file.Close();
        }

        [ContextMenu("Load")]
        public void Load(){
            if(File.Exists((string.Concat(Application.persistentDataPath,"/",savePath)))){
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream Stream = File.Open(string.Concat(Application.persistentDataPath,"/",savePath),FileMode.Open);
                JsonUtility.FromJsonOverwrite(formatter.Deserialize(Stream).ToString(),this);
                Stream.Close();
            }
        }

        
    }

}