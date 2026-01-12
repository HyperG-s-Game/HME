using System.IO;
using UnityEngine;

namespace WolfGamer.Hold_My_Eggs{

    [CreateAssetMenu(fileName = "Settings Data",menuName = "ScriptableObject/Settings Data")]
    public class SettingsSO : ScriptableObject {
        public string savePath =  "settings.json";
        public SettingsData settingsData;

        [ContextMenu("Save")]
        public void Save(){
            string json = JsonUtility.ToJson(settingsData, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, savePath), json);
        }

        [ContextMenu("Load")]
        public void Load(){
            string path = Path.Combine(Application.persistentDataPath, savePath);
            if(File.Exists(path)){
                string json = File.ReadAllText(path);
                settingsData = JsonUtility.FromJson<SettingsData>(json);
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
