using System.IO;
using UnityEngine;
using WolfGamer.Utils;
using System.Collections.Generic;

namespace WolfGamer.Hold_My_Eggs{

    [CreateAssetMenu(fileName = "Level Data",menuName = "ScriptableObject/Level Data")]
    public class LevelDataSO : ScriptableObject {

        public SceneIndex sceneIndex = SceneIndex.Level_1;
        public List<BowlsType> levelBowlsType;
        public List<Bowls> bowlsList;
        public float minBowlSpeed = 30f, maxBowlSpeed = 40f;
        public LevelSaveData levelSaveData;

        public void SetLevelComplete(bool isComplete){
            levelSaveData.isCompleted = isComplete;
        }
        public void SetDeathCount(int count){
            levelSaveData.playerDeathCount = Mathf.Min(levelSaveData.playerDeathCount + count, int.MaxValue);
        }

        [ContextMenu("Save")]
        public void Save(){
            string json = JsonUtility.ToJson(levelSaveData, true);
            string fileName = $"LevelData_{(int)sceneIndex}.json";
            File.WriteAllText(Path.Combine(Application.persistentDataPath, fileName), json);
        }

        [ContextMenu("Load")]
        public void Load(){
            string fileName = $"LevelData_{(int)sceneIndex}.json";
            string path = Path.Combine(Application.persistentDataPath, fileName);
            if(File.Exists(path)){
                string json = File.ReadAllText(path);
                levelSaveData = JsonUtility.FromJson<LevelSaveData>(json);
            }
        }

        [ContextMenu("Reset Level")]
        public void Reset(){
            levelSaveData.isCompleted = false;
            levelSaveData.playerDeathCount = 0;
            Save();
        }
    }

    [System.Serializable]
    public class LevelSaveData{
        public int playerDeathCount;
        public bool isCompleted = false;
    }
}
