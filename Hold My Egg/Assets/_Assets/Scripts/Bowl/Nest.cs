using UnityEngine;
using System.Collections;
namespace WolfGamer.Hold_My_Eggs{

    public class Nest : MonoBehaviour{
        [SerializeField] private ParticleSystem confetiEffect;
        [SerializeField] private GameObject standingBird,birdIdle,Background;
        [SerializeField] private Animator birdAnimator;
        private void OnTriggerEnter2D(Collider2D coli){
            Egg egg = coli.GetComponent<Egg>();
            if(egg != null){
                AudioManager.i.PlayOneShotMusic(SoundType.Game_Win);
                confetiEffect.Play();
                Background.SetActive(true);
                standingBird.SetActive(true);
                birdAnimator.SetTrigger("Egg Recived");
                StartCoroutine(GetJoyFull(coli));
            }
        }


        private IEnumerator GetJoyFull(Collider2D coli){
            standingBird.SetActive(false);
            yield return new WaitForSeconds(0.01f);
            birdIdle.SetActive(true);
            yield return new WaitForSeconds(1f);
            if(coli.gameObject.CompareTag("Egg")){
                GameHandler.i.SetGameOver(true);
            }
        }
        


    }
}
