using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace WolfGamer.Hold_My_Eggs{
    
    public enum SoundType{
        Main_Sound,
        jump_Sound,
        EggCracking,
        Egg_placemnet_in_Bowl,
        ButtonClickSound,
        Coin_Collect,
        Game_Win,
        Game_Loss,

    }
    
    
    public class AudioManager : MonoBehaviour{
        public static AudioManager i{get;private set;}
        
        [System.Serializable]
        public class Sounds{
            public SoundType soundType;
            public AudioClip audioClip;
            public bool isLooping;
            public bool playOnAwake;
            public bool playonShot;
            [Range(0f,1f)]
            public float volumeSlider;
            [Range(-3f,3f)]
            public float pitchSlider;
            public bool isMute;
            public bool isSfx;
            

            [HideInInspector]
            public AudioSource source;

        }

        [SerializeField] private Sounds[] sounds;
        [SerializeField] private SettingsSO soundSettings;
        private List<AudioSource> sfxSourceList;
        

        
        private void Awake(){
            if(i == null){
                i = this;
            }else{
                Destroy(i.gameObject);
                Debug.Log($"Another Audio Manager is Found And Destroyed");
            }
            DontDestroyOnLoad(i.gameObject);
        }
        private void Start(){
            sfxSourceList = new List<AudioSource>();
            foreach(Sounds s in sounds){
                s.source = gameObject.AddComponent<AudioSource>();
                if(s.isSfx){
                    sfxSourceList.Add(s.source);
                }
                s.source.loop = s.isLooping;
                s.source.pitch = s.pitchSlider;
                s.source.volume = s.volumeSlider;
                s.source.playOnAwake = s.playOnAwake;
                s.source.clip = s.audioClip;
                s.source.mute = !soundSettings.settingsData.isMusicOn;
            }
             
        }
        private void Update(){
            MuteMusic(!soundSettings.settingsData.isMusicOn);
            MuteSFX(!soundSettings.settingsData.isSoundOn);
        }

        public void MuteMusic(bool Mute){
            
            for (int i = 0; i < sounds.Length; i++){
                if(sounds[i].soundType == SoundType.Main_Sound){
                    sounds[i].source.mute = Mute;
                }
                
            }
            
        }
        public void MuteSFX(bool mute){
            for (int i = 0; i < sfxSourceList.Count; i++){
                sfxSourceList[i].mute = mute;
            }
        }
        
        
        public void PlayMusic(SoundType soundType){
            Sounds s = Array.Find(sounds ,s => s.soundType == soundType);
            s.source.Play();
        }
        public void PauseMusic(SoundType soundType){
            Sounds s = Array.Find(sounds ,s => s.soundType == soundType);
            s.source.Pause();
        }
        public void PlayOneShotMusic(SoundType soundType){
            Sounds s = Array.Find(sounds ,s => s.soundType == soundType);
            s.source.PlayOneShot(s.audioClip);
        }
        
    }

}