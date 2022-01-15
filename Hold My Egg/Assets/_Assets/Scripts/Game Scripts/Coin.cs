using UnityEngine;


namespace WolfGamer.Hold_My_Eggs{
    public class Coin : MonoBehaviour {
        [SerializeField] private ParticleSystem particalEffect;
         
        private void OnTriggerEnter2D(Collider2D coli){
            if(coli.gameObject.CompareTag("Egg")){
                CollideWithEgg();    
            }
        }
        public void CollideWithEgg(){
            particalEffect.transform.SetParent(null);
            particalEffect.Play();
            AudioManager.i.PlayOneShotMusic(SoundType.Coin_Collect);
            CoinCollecter.i.CollectCoin(1);
            Destroy(gameObject);

        }
    
    }

}