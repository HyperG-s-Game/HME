using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace WolfGamer.Hold_My_Eggs{
    public enum BowlsType{
        Red,Blue,Green,White
    }
    public enum MovementType{
        Stationery,Moving
    }

    public class Bowls : MonoBehaviour {

        [Header("Setup")]
        [SerializeField] private MovementType movementType;
        [SerializeField] private BowlsType bowlsType;
        [SerializeField] private bool hasCoin;

        [Header("Colliders & Timing")]
        [SerializeField] private Collider2D[] colliders;
        [SerializeField] private float colliderActivationTimerMax = 0.3f;
        [SerializeField] private UnityEvent OnTimeUP, OnTimeReset;

        [Header("Movement")]
        [SerializeField] protected float moveSpeed;
        protected float currenSpeed;

        protected bool startMove;
        public bool iscarringEgg;

        // Runtime
        public bool startTimer = false;
        private float currentTimer = 0f;

        protected virtual void Awake(){}

        private void Start(){
            // Ensure colliders start enabled
            OnOffTriggerCollider(true);
            currentTimer = 0f;
            currenSpeed = moveSpeed;
        }

        protected virtual void Update(){
            // Handle the collider re-enable timer
            if(startTimer){
                currentTimer -= Time.deltaTime;
                if(currentTimer <= 0f){
                    startTimer = false;
                    OnOffTriggerCollider(true);
                    OnTimeReset?.Invoke();
                }
            }
        }

        public void StartGame(bool _state){
            startMove = _state;
        }

        // Called when egg leaves the bowl (Egg.Jump)
        public void SetDeactivateBowl(){
            // Disable colliders for a short window to prevent instant re-catch
            OnOffTriggerCollider(false);
            startTimer = true;
            currentTimer = colliderActivationTimerMax;
            OnTimeUP?.Invoke();
        }

        // Called on revive to immediately make bowl catchable again
        public void Revive(){
            startTimer = false;
            currentTimer = 0f;
            OnOffTriggerCollider(true);
            OnTimeReset?.Invoke();
        }

        public virtual void OnOffTriggerCollider(bool _isOn){
            if(colliders == null) return;
            for (int i = 0; i < colliders.Length; i++){
                if(colliders[i] != null){
                    colliders[i].enabled = _isOn;
                }
            }
        }

        public void SetSpeed(float _speed){
            // Randomize initial direction
            int rand = Random.Range(0, 4);
            if(rand == 3){
                _speed *= -1f;
            }
            moveSpeed = _speed;
            currenSpeed = _speed;
        }

        // Properties kept for existing usages
        public MovementType GetMovementType{
            get{ return movementType; }
        }

        public BowlsType GetBowlsType{
            get{ return bowlsType; }
        }

        public bool GetHasCoinBowl(){
            return hasCoin;
        }

    }
}