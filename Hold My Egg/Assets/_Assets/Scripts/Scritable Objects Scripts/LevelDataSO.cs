using System.IO;
using UnityEngine;
using WolfGamer.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace WolfGamer.Hold_My_Eggs{

    [CreateAssetMenu(fileName = "Level Data",menuName = "ScriptableObject/Level Data")]
    public class LevelDataSO : ScriptableObject {

        
        public SceneIndex sceneIndex = SceneIndex.Level_1;
        public List<BowlsType> levelBowlsType;
        public List<Bowls> bowlsList;
        public float minBowlSpeed = 30f,maxBowlSpeed = 40f;
        public LevelSaveData levelSaveData;
        public void SetLevelComplete(bool isComplete){
            levelSaveData.isCompleted = isComplete;
        }
        public void SetDeathCount(int count){
            levelSaveData.playerDeathCount = Mathf.Min(levelSaveData.playerDeathCount + count,int.MaxValue);
            
        }
        
        [ContextMenu("Save")]
        public void Save(){
            string data = JsonUtility.ToJson(levelSaveData,true);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(Application.persistentDataPath,"/","Level Data",sceneIndex));
            formatter.Serialize(file,data);
            file.Close();
        }

        [ContextMenu("Load")]
        public void Load(){
            if(File.Exists((string.Concat(Application.persistentDataPath,"/","Level Data",sceneIndex)))){
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream Stream = File.Open(string.Concat(Application.persistentDataPath,"/","Level Data",sceneIndex),FileMode.Open);
                JsonUtility.FromJsonOverwrite(formatter.Deserialize(Stream).ToString(),levelSaveData);
                Stream.Close();
            }
        }
        [ContextMenu("Reset Level")]
        public void Reset(){
            levelSaveData.isCompleted = false;
            SetDeathCount(0);
            Save();
        }
    }
    [System.Serializable]
    public class LevelSaveData{
        public int playerDeathCount;
        public bool isCompleted = false;
    }
    

}
