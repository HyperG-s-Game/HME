using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace WolfGamer.Hold_My_Eggs{
    public class MovingBowls : Bowls {
        
        [SerializeField] private float rayLength = 0.3f;
        [SerializeField] private LayerMask obstacleMask;
        
        private bool inMovementArea;
        
        
        
        protected override void Awake(){
            base.Awake();
        }
        
        protected override void Update(){
            base.Update();

            // Move only when game is running and bowl is inside movement area
            bool canMove = startMove && inMovementArea;
            currenSpeed = canMove ? moveSpeed : 0f;

            if(canMove){
                transform.Translate(Vector2.right * currenSpeed * Time.deltaTime);
            }
        }
        
        public override void OnOffTriggerCollider(bool _isOn){
            base.OnOffTriggerCollider(_isOn);
        }
        
        private void OnCollisionEnter2D(Collision2D coli){
            if(coli.gameObject.CompareTag("Wall")){
                moveSpeed *= -1f;
            }
        }
        private void OnTriggerEnter2D(Collider2D coli){
            if(coli.gameObject.CompareTag("Wall")){
                moveSpeed *= -1f;
            }
        }
        private void OnTriggerStay2D(Collider2D coli){
            if(coli.gameObject.CompareTag("Movement Area")){
                inMovementArea = true;
            }
        }
        private void OnTriggerExit2D(Collider2D coli){
            if(coli.gameObject.CompareTag("Movement Area")){
                inMovementArea = false;
            }
        }
        
        
        private void OnDrawGizmosSelected(){
            Gizmos.color = Color.black;
            Gizmos.DrawRay(transform.position,Vector2.right * rayLength);
            Gizmos.DrawRay(transform.position,Vector2.left * rayLength);
        }
        
        
        
        
        
        

        


    }
}