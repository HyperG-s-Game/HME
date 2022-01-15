using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WolfGamer.Hold_My_Eggs{
    public class Ground : MonoBehaviour {
        [SerializeField]private float moveSpeed = 2f;

        [SerializeField] private bool startMoveing;

        private void Update(){
            if(startMoveing){
                transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            }
        }
        public void Move(bool move){
            startMoveing = move;
        }
        
        


    }

}