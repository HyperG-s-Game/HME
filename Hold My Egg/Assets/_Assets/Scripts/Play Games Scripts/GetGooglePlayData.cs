using UnityEngine;
using WolfGamer.Utils;
using WolfGamer.Hold_My_Eggs;
public class GetGooglePlayData : MonoBehaviour {
    
    
    public static GetGooglePlayData i;

    private void Awake(){
        if(i == null){
            i = this;
        }
    }

    public void SetAchivementData(LevelDataSO levelData){
        
    #if UNITY_ANDROID
        switch(levelData.sceneIndex){
            case SceneIndex.Level_1:
                PlayGamesController.PostAchivements(GPGSIds.achievement_welcom_abord);
            break;
            case SceneIndex.Level_5:
                PlayGamesController.PostAchivements(GPGSIds.achievement_level_5_complete);
            break;
            
            case SceneIndex.Level_20:
                PlayGamesController.PostAchivements(GPGSIds.achievement_level_20_complete);
            break;
            case SceneIndex.Level_50:
                PlayGamesController.PostAchivements(GPGSIds.achievement_reached_level_50);
            break;
            case SceneIndex.Level_80:
                PlayGamesController.PostAchivements(GPGSIds.achievement_reached_level_50_2);
            break;
            case SceneIndex.Level_100:
                PlayGamesController.PostAchivements(GPGSIds.achievement_all_level_completed);
            break;
        }
        
    #endif
    }
    
    
}
