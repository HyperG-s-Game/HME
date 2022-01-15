using UnityEngine;
using UnityEngine.UI;

namespace WolfGamer.Hold_My_Eggs {
    public class IAPManager : MonoBehaviour {
        [SerializeField] private Button buyButton;
        [SerializeField] private SettingsSO settings;
        private void Start(){
            if(!settings.settingsData.hasAdInGame){
                buyButton.interactable = false;
            }else{
                buyButton.interactable = true;
            }
        }
        public void OnPurchaseSucces(){
            settings.settingsData.hasAdInGame = false;
            buyButton.interactable = false;
        }
        public void OnPurchaseFailed(){
            settings.settingsData.hasAdInGame = true;
            buyButton.interactable = true;
        }
        
    }

}