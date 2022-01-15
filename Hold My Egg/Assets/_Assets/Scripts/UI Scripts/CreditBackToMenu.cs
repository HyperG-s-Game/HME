using UnityEngine;
using WolfGamer.Utils;

namespace WolfGamer.Hold_My_Eggs {
    public class CreditBackToMenu : MonoBehaviour {
        
        public void BackToMenu(){
            LevelLoader.instance.SwitchScene(SceneIndex.Main_Menu);
        }
    }

}