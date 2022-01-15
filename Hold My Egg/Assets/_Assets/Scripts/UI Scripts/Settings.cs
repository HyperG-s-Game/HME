using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Lean.Localization;
using System.Collections;
using Google.Play.Review;



namespace WolfGamer.Hold_My_Eggs{
    public class Settings : MonoBehaviour {
        public enum Langugages{
            English = 0,Brazil = 1,Mexican = 2,Argentina = 3,Thailand = 4,Malaysia = 5
        }
        [SerializeField] private LeanLocalization localization;
        [SerializeField] private TextMeshProUGUI languageText;
        [SerializeField] private SettingsSO settingsDataSO;
        [SerializeField] private Toggle soundToggle,musicToggle;
        [SerializeField] private Langugages[] languageNameArray;

        private void Start(){

            musicToggle.isOn = settingsDataSO.settingsData.isMusicOn;
            soundToggle.isOn = settingsDataSO.settingsData.isSoundOn;
            localization.CurrentLanguage = languageNameArray[settingsDataSO.settingsData.currentLanguageIndex].ToString();
            OnLanguageChange();
        }
        public void OnMusicToogleChangeing(){
            settingsDataSO.settingsData.isMusicOn = musicToggle.isOn;

        }
        public void OnsoundToogleChanging(){
            settingsDataSO.settingsData.isSoundOn = soundToggle.isOn;
        }
        public void OnLanguageChange(){
            languageText.SetText(languageNameArray[settingsDataSO.settingsData.currentLanguageIndex].ToString());
        }
        public void ChangeLanguageText(string text){
            languageText.SetText(text);
        }
        public void RateUs(){
        #if UNITY_ANDROID
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.PlayResume.HoldMyEgg2021");
        #endif
        }
        
        public void ShowPrivacyAndPolicy(){
            Application.OpenURL(settingsDataSO.settingsData.privacyPolicyURL);
        }
        
        

        

    }

}
