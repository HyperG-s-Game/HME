using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace WolfGamer.Hold_My_Eggs{
    public class MainMenu : MonoBehaviour {
        
        [SerializeField] private UnityEvent OnGameStart;
        [SerializeField] private GameObject quitWindow;
        [SerializeField] private Button playButton;
        
        private void Start(){
            OnGameStart?.Invoke();
            AudioManager.i.PlayMusic(SoundType.Main_Sound);
        }
        
        
        private void Update(){
            if(Input.GetKeyDown(KeyCode.Escape)){
                quitWindow.SetActive(true);
                playButton.interactable = false;
            }
        }
        
        public void Quit(){
            
            playButton.interactable = true;
            Application.Quit();
        }
        public void ShowLeaderboard(){
            #if !UNITY_EDITOR
            PlayGamesController.ShowLeaderboardUI();
            #endif
        }
        
    }

}