using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace WolfGamer.Hold_My_Eggs{

    public class LanguageChangeButton : MonoBehaviour {
        [SerializeField] private LeanLocalization leanLocalization;
        [SerializeField] private Settings.Langugages langugage;
        [SerializeField] private SettingsSO settingsSO;
        
        public void ChangeToLanguage(){
            settingsSO.settingsData.currentLanguageIndex = (int)langugage;
            leanLocalization.SetCurrentLanguage(langugage.ToString());
            
        }
        
        
    }

}