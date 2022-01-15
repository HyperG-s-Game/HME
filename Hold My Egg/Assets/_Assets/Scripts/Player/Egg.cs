using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using System.Collections.Generic;
namespace WolfGamer.Hold_My_Eggs{
    
    [System.Serializable]
    public struct EggData{
        public BowlsType bowlsType;
        public int eggLayer;
        public Sprite liveEggSprite,deadEggSprite;
        public int bowlsCheckMask;
    }
    [RequireComponent(typeof(Rigidbody2D))]
    public class Egg : MonoBehaviour {

        #region Exposed Variables........
        
        [SerializeField,Range(1,50)] private float jumpForce = 5f;
        [SerializeField] private SpriteRenderer gfxSpriteRenderer;
        [SerializeField,Range(1,500)] private float rotationForce = 300f;
        [SerializeField] private ParticleSystem eggCrackEffect;
        [SerializeField] private GameObject gfxSpriteLive,gfxSpriteDead,gfxEggCrack_2;
        [SerializeField] private SpriteRenderer liveGFXrenderer,deadGFXrenderer;
        [SerializeField] private float maxRayDistanceDown = 0.3f,maxRayDistanceUP = 1f;
        [SerializeField] private LayerMask coinMask,obstacleMask;
        [SerializeField] private LayerMask bowlCheckMaskNumber;
        [SerializeField] private EggData[] eggData;
        private const int whiteBowlLayer = 9;
        private bool isRevived = false;
        #endregion

        #region private Variables...........
        private bool isDead;
        private Rigidbody2D rb2D;
        private bool startJumping;
        private bool resetJump;
        private Bowls currentBowl;
        private Collider2D coli;
        private int initialSortOrder;
        private bool isPlayedCollectedSound;
        private GameHandler gameHandler;
        
        #endregion



        #region Private Methods......
        
        private void Awake(){
            coli = GetComponent<Collider2D>();
            rb2D = GetComponent<Rigidbody2D>();
        }
        private void Start(){
            gameHandler = GameHandler.i;
            resetJump = false;   
            initialSortOrder = gfxSpriteRenderer.sortingOrder;
            
            currentBowl = SearchCurrentBowl();
        }
        private void Update(){
            if(iscollidingBottom()){
                resetJump = true;
            }
            if(startJumping){
                #if UNITY_EDITOR
                    KeyBoardJumping();
                #elif UNITY_ANDROID
                    TouchJump();
                #endif

            }
            if(SearcheCoins() != null){
                SearcheCoins().CollideWithEgg();
            }
            if(rb2D.velocity.y > 1f){
                if(iscollidingTop()){
                    coli.enabled = true;
                }else{
                    coli.enabled = false;
                }
            }
            else if(rb2D.velocity.y <= 0.1f){
                coli.enabled = true;
            }
            if(rb2D.velocity.y <= 0.1f || transform.parent != null){
                gfxSpriteRenderer.sortingOrder = initialSortOrder;
            }
            if(currentBowl != null) {
                if(!currentBowl.iscarringEgg){
                    resetJump = true;
                    currentBowl.iscarringEgg = true;
                }
                gameHandler.SetCurrentBowl(currentBowl);
            }
            ChangeDeadLiveSprite(isDead);
        }
        private void KeyBoardJumping(){
            if((Input.GetKeyDown(KeyCode.Space) | Input.GetMouseButtonDown(0)) && !EventSystem.current.IsPointerOverGameObject()){
                Jump();
            }
        }
        private void TouchJump() {
            if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began){
                int id = Input.touches[0].fingerId;
                if(!EventSystem.current.IsPointerOverGameObject(id)){
                    Jump();
                }
            }
        }
        private void Jump(){
            if(resetJump){
                AudioManager.i.PlayOneShotMusic(SoundType.jump_Sound);
                SetParent(null,rb2D.position);
                resetJump = false;
                rb2D.bodyType = RigidbodyType2D.Dynamic;
                AddJumpForce();
                if(currentBowl != null){
                    currentBowl.SetDeactivateBowl();
                }
                Invoke(nameof(ChangeSortingOrder),0.2f);
            }
        }
        private void ChangeSortingOrder(){
            gfxSpriteRenderer.sortingOrder = 20;
        }
        private void AddJumpForce(){
            rb2D.velocity = new Vector2(0f,0f);
            rb2D.AddForce(Vector2.up * jumpForce,ForceMode2D.Impulse);
            rb2D.AddTorque(-rotationForce);
        }
        
        
        private void ChangeDeadLiveSprite(bool _isDead){
            if(_isDead){
                gfxSpriteDead.SetActive(true);
                gfxSpriteLive.SetActive(false);
            }else{
                gfxSpriteDead.SetActive(false);
                gfxSpriteLive.SetActive(true);
            }
        }
        #endregion



        #region Seraching Methods........

        private Bowls SearchCurrentBowl(){
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position,(Vector2.up * -1),maxRayDistanceDown,bowlCheckMaskNumber);
            if(hit2D.collider != null){
                Bowls bowl = hit2D.transform.GetComponent<Bowls>();
                if(bowl != null){
                    return bowl;
                }else{
                    return null;
                }
            }
            return null;
        }
        private bool iscollidingBottom(){
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position,(Vector2.up * -1f),maxRayDistanceDown,obstacleMask);
            if(hit2D.collider != null){
                Debug.Log("hit with " + hit2D.collider.name);
                return true;
            }
            return false;
        }
        private bool iscollidingTop(){
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position,(Vector2.up),maxRayDistanceUP,obstacleMask);
            
            if(hit2D.collider != null){
                return true;
            }
            return false;
        }
        private Coin SearcheCoins(){
            RaycastHit2D[] hit2D = Physics2D.CircleCastAll((Vector2)transform.position,0.1f,Vector2.zero,0f,coinMask);
            if(hit2D.Length > 0){
                foreach (RaycastHit2D hits in hit2D){
                    Coin coin = hits.collider.GetComponent<Coin>();
                    return coin;
                }

            }
            return null;
            
        }
        
        #endregion
        
        #region Collision Detection..........
        
        private void OnCollisionEnter2D(Collision2D _coli){
            if(!isDead){
            if(_coli.collider != null){
                    if(_coli.gameObject.CompareTag("Ground")){
                        AudioManager.i.PlayMusic(SoundType.EggCracking);
                        rb2D.freezeRotation = true;
                        transform.eulerAngles = Vector3.zero;
                        rb2D.velocity = Vector2.zero;
                        rb2D.isKinematic = true;
                        eggCrackEffect.gameObject.SetActive(true);
                        eggCrackEffect.Play();
                        SetParent(null,transform.position);
                        SetJumping(false);
                        isDead = true;
                        
                        Debug.Log("Revive The Player");
                        AnalyticsResult result = Analytics.CustomEvent("Player Data",new Dictionary<string,object>{
                            {"Level Died",gameHandler.GetLevelData.sceneIndex},
                            {"Position",Mathf.RoundToInt(transform.position.y / 20f)}
                        });
                        
                        Debug.Log(result + " from " + this.name);
                    }
                    
                }
            }
            
            
        }
        private bool playedAlready;
        private void OnTriggerEnter2D(Collider2D coli){
            if(!isDead){
                if(coli != null){
                    if(coli.gameObject.CompareTag("BowlObject") && !iscollidingBottom()){
                        currentBowl = SearchCurrentBowl();
                        SetParent(coli.transform,transform.position);
                        resetJump = true;
                        if(!playedAlready && currentBowl != null){
                            playedAlready = true;
                            AudioManager.i.PlayOneShotMusic(SoundType.Egg_placemnet_in_Bowl);
                        }
                    }
                }
            }
        }
        private void OnTriggerExit2D(Collider2D coli){
            playedAlready = false;
        }
        #endregion



        #region Public Setters.......
        public void SetDead(bool _dead){
            isDead = _dead;
            GameHandler.i.IncreasePlayerDeath(1);
        }
        
        
        public void SetParent(Transform _parent,Vector3 Pos){
            transform.SetParent(_parent);
            transform.position = Pos;
        }
        public void SetJumping(bool canJump){
            startJumping = canJump;
        }
        
        public void Revive(Bowls _activeBowl){
            if(currentBowl.GetBowlsType != BowlsType.White){
                ChangeEggtype(currentBowl.GetBowlsType);
            }
            rb2D.freezeRotation = false;
            _activeBowl.Revive();
            transform.eulerAngles = Vector3.zero;
            isDead = false;
            SetJumping(true);
            Vector3 newPos = _activeBowl.transform.position + Vector3.up * 0.5f;
            SetParent(_activeBowl.transform,newPos);
            rb2D.isKinematic = false;
            resetJump = true;
            
        }
        #endregion

        
        #region Public Getters......
        public Bowls GetCurrentBowl(){
            return currentBowl;
        }
        public bool GetIsDead{
            get{
                return isDead;
            }
        }

        #endregion

        #region Changing Egg Type........
        public void ChangeEggTypeWithJump(BowlsType bowlsType){
            if(!isDead){
                if(resetJump){
                    ChangeEggtype(bowlsType);
                }
                Jump();
            }
        }
        public void ChangeEggtype(BowlsType bowlsType){
            for (int i = 0; i < eggData.Length; i++){
                if(eggData[i].bowlsType == bowlsType){
                    liveGFXrenderer.color = Color.white;
                    deadGFXrenderer.color = Color.white;
                    liveGFXrenderer.sprite = eggData[i].liveEggSprite;
                    deadGFXrenderer.sprite = eggData[i].deadEggSprite;
                    gfxEggCrack_2.SetActive(false);
                    gameObject.layer = eggData[i].eggLayer;
                    bowlCheckMaskNumber = (1 << eggData[i].bowlsCheckMask | 1 << whiteBowlLayer);
                }
            }
        }
        #endregion
        

        #region Gizmos.......
        private void OnDrawGizmosSelected(){

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position,(Vector2.up * -1f) * maxRayDistanceDown);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position,(Vector2.down) * maxRayDistanceDown);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position,(Vector2.up) * maxRayDistanceUP);
        }
        #endregion
        
    }
}
