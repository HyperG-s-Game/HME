using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
namespace WolfGamer.Hold_My_Eggs{

    [CreateAssetMenu(fileName = "Settings Data",menuName = "ScriptableObject/Settings Data")]
    public class SettingsSO : ScriptableObject {
        public string savePath =  "settings.dat";    
        public SettingsData settingsData;



        [ContextMenu("Save")]
        public void Save(){
            string data = JsonUtility.ToJson(settingsData,true);
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
                JsonUtility.FromJsonOverwrite(formatter.Deserialize(Stream).ToString(),settingsData);
                Stream.Close();
            }
        }
    }
    [System.Serializable]
    public class SettingsData{
        public bool hasAdInGame = true;
        public bool isMusicOn;
        public bool isSoundOn;
        public int currentLanguageIndex;
        public string privacyPolicyURL = "";
    }
    
}
