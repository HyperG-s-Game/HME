using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
namespace WolfGamer.Hold_My_Eggs{
    public enum BowlsType{
        Red,Blue,Green,White
    }
    public enum MovementType{
        Stationery,Moving
    }
    public class Bowls : MonoBehaviour {

        [SerializeField] private MovementType movementType;
        [SerializeField]private float colliderActivationTimerMax = 0.3f;
        [SerializeField] private UnityEvent OnTimeUP,OnTimeReset;
        [SerializeField] protected Collider2D[] colliders;
        [SerializeField] protected float moveSpeed,currenSpeed;
        
        [SerializeField] private BowlsType bowlsType;
        [SerializeField] private bool hasCoin;
        protected bool startMove;
        public bool iscarringEgg;
        public bool startTimer = false;
        
        private float currentTime;
        
        protected virtual void Awake(){
            
        }
        private void Start(){
            currentTime = colliderActivationTimerMax;

        }
        public void StartGame(bool _state){
            startMove = _state;
        }
        
        protected virtual void Update(){
            
        }
        public void SetDeactivateBowl(){
            OnTimeUP?.Invoke();
        }
        public void Revive(){
            startTimer = false;
            OnTimeReset?.Invoke();
            currentTime = colliderActivationTimerMax;
            currentTime += colliderActivationTimerMax;
        }
        public virtual void OnOffTriggerCollider(bool _isOn){
            for (int i = 0; i < colliders.Length; i++){
                colliders[i].enabled = _isOn;
            }
        }
        public void SetSpeed(float _speed){
            int rand = Random.Range(0,4);
            if(rand == 3){
                _speed *= -1;
            }
            moveSpeed = _speed;
            currenSpeed = _speed;
        }
        
        public MovementType GetMovementType{
            get{
                return movementType;

            }
        }
        public BowlsType GetBowlsType{
            get{
                return bowlsType;
            }
        }
        
        public bool GetHasCoinBowl(){
            return hasCoin;
        }
        
        
        
        
        
        
    }

}