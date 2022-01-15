using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace WolfGamer.Hold_My_Eggs{
    public class MovingBowls : Bowls {
        
        [SerializeField] private float rayLength = 0.3f;
        [SerializeField] private LayerMask obstacleMask;
        
        private bool move;
        
        
        
        protected override void Awake(){
            base.Awake();
        }
        
        protected override void Update(){
            base.Update();
            currenSpeed = moveSpeed;
            if(move){
                Movement();
            }else{
                currenSpeed = 0f;
            }
        }
        
        public override void OnOffTriggerCollider(bool _isOn){
            base.OnOffTriggerCollider(_isOn);
        }
        
        
        private void Movement(){
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
        private void OnCollisionEnter2D(Collision2D coli){
            if(coli.gameObject.CompareTag("Wall")){
                moveSpeed *= -1;
            }
        }
        private void OnTriggerEnter2D(Collider2D coli){
            if(coli.gameObject.CompareTag("Wall")){
                moveSpeed *= -1;
            }
        }
        private void OnTriggerStay2D(Collider2D coli){
            if(coli.gameObject.CompareTag("Movement Area")){
                if(startMove){
                    move = true;
                }else{
                    move = false;
                }
            }
        }
        private void OnTriggerExit2D(Collider2D coli){
            if(coli.gameObject.CompareTag("Movement Area")){
                move = false;
            }
        }
        
        
        private void OnDrawGizmosSelected(){
            Gizmos.color = Color.black;
            Gizmos.DrawRay(transform.position,Vector2.right * rayLength);
            Gizmos.DrawRay(transform.position,Vector2.left * rayLength);
        }
        
        
        
        
        
        

        


    }
}