using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;

namespace WolfGamer.Hold_My_Eggs{
    public class Firebaseanayltics : MonoBehaviour {
        
        #region singleton........
        public static Firebaseanayltics current;
        private void Awake(){
            if(current == null){
                current = this;
            }else{
                Destroy(current.gameObject);
            }
            DontDestroyOnLoad(current.gameObject);
        }

        #endregion




                
                
    }     

}