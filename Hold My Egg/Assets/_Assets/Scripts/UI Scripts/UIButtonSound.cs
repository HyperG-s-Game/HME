using UnityEngine;


namespace WolfGamer.Hold_My_Eggs {
    public class UIButtonSound : MonoBehaviour {
        public void PlayButtonClickSound(){
            AudioManager.i.PlayMusic(SoundType.ButtonClickSound);
        }
        
    }

}