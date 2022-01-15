using UnityEngine;
using UnityEngine.UI;
using WolfGamer.Utils;
namespace WolfGamer.Hold_My_Eggs{
    public class OptionMenu : MonoBehaviour {
        
        [SerializeField] private Button resumeButton;
        private void Start(){
            if(LevelLoader.isReset){
                resumeButton.interactable = false;
            }else{
                resumeButton.interactable = true;
            }
        }
        
        public void PlayNewGame(){
            SavingAndLoadingManager.instance.ResetGame();
            LevelLoader.instance.ResetGame();
        }
        public void ResumeGame(){
            LevelLoader.instance.PlayLevel();
        }
        
        
    }

}