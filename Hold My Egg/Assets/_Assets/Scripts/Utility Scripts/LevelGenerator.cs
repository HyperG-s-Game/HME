using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WolfGamer.Hold_My_Eggs{
    public class LevelGenerator : MonoBehaviour {
        [SerializeField] private float verticalDistance,horizontalDistance;
        [SerializeField] private Coin pfCoin;
        [SerializeField] private Egg whiteEgg;
        [SerializeField] private Transform Nest;
        [SerializeField] private List<GameObject> sceneObjectList;

        [SerializeField] private float maxDistanceFromEggForBowls = 4f,maxDistanceFromObject = 12f;
        private List<Bowls> levelBowls;
        private GameHandler gameHandler;
        private void Start(){
            gameHandler = GameHandler.i;
        }
        private void Update(){
            for (int i = 0; i < sceneObjectList.Count; i++){
                float dist = Vector2.Distance(sceneObjectList[i].transform.position , gameHandler.GetEgg().transform.position);
                if(dist >= maxDistanceFromObject){
                    sceneObjectList[i].gameObject.SetActive(false);
                }else{
                    sceneObjectList[i].gameObject.SetActive(true);
                }
            }
            for (int i = 0; i < gameHandler.GetBowlsList().Count; i++){
                float dist = Vector2.Distance(gameHandler.GetBowlsList()[i].transform.position , gameHandler.GetEgg().transform.position);
                if(dist >= maxDistanceFromEggForBowls){
                    gameHandler.GetBowlsList()[i].gameObject.SetActive(false);
                }else{
                    gameHandler.GetBowlsList()[i].gameObject.SetActive(true);
                }
            }
        }

        public void CreateLeveL(LevelDataSO levelData){
            StartCoroutine(CreateBowlRoutine(levelData.bowlsList));
        }
        
        private IEnumerator CreateBowlRoutine(List<Bowls> bowls){
            yield return new WaitForSeconds(0.1f);
            CreateBowl(bowls);
        }
        private void CreateBowl(List<Bowls> bowls){
            for (int i = 0; i < bowls.Count; i++) {

                int randBowl = UnityEngine.Random.Range(0,bowls.Count);
                float randH = UnityEngine.Random.Range(-horizontalDistance,horizontalDistance);
                if(i == 0){
                    randBowl = 0;
                }
                /*else if(i == 1){
                    if(sceneIndex == Utils.SceneIndex.Level_21 || sceneIndex == Utils.SceneIndex.Level_1){
                        randBowl = 1;
                        randH = 0;
                    }
                }
                */
                Bowls b = Instantiate(bowls[randBowl],transform.position,Quaternion.identity);
                

                if(b.GetMovementType == MovementType.Moving){
                    // Spawn Moving Bowls...
                    b.transform.position = new Vector3((i > 0 ? randH : 0f),b.transform.position.y + i * verticalDistance,b.transform.position.z);
                }else{
                    // Spawn Stationery Bowls
                    b.transform.position = new Vector3(b.transform.position.x, b.transform.position.y + i * verticalDistance,b.transform.position.z);
                }
                // Spawn Coins.
                int randCoin = Random.Range(0,7);
                if(!b.GetHasCoinBowl()){
                    if(randCoin == 3){
                        Instantiate(pfCoin,b.transform.position + Vector3.up * (verticalDistance / 2f),Quaternion.identity);
                    }
                }
                if(i == bowls.Count - 1){
                    Vector3 nestPos = new Vector3(0f,b.transform.position.y + (verticalDistance + 1.5f),0f);
                    CreateNest(nestPos);
                }
                GameHandler.i.AddToBowlsList(b);
                
            }
            GameHandler.i.SetBowlSpeed();
            CreateEgg(bowls[0].GetBowlsType);
        }
        

        private void CreateEgg(BowlsType bT){
            Egg spawnedEgg = Instantiate(whiteEgg,transform.position + Vector3.up * 0.5f,Quaternion.identity);
            spawnedEgg.ChangeEggtype(bT);
            GameHandler.i.SetEgg(spawnedEgg);
        }
        private void CreateNest(Vector3 t){
            Transform n = Instantiate(Nest,t,Quaternion.identity);
            CamerFollow.i.SetNestPos(n);

            sceneObjectList.Add(n.gameObject);
        }
        
    }

}