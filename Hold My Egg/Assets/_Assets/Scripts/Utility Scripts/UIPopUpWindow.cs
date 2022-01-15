using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace WolfGamer.Utils {
    public class UIPopUpWindow : MonoBehaviour {
        
        [SerializeField] private float popUPSpeed = 0.3f,popDonwSpeed = 0.3f;


        [SerializeField] private UnityEvent OnPopUp,OnPopDown;
        [SerializeField] private float delay;
        private Vector3 popScale;
        private bool isPopedUp,isPopDowned;
        
        private void Start(){
            popScale = transform.localScale * 1f;
        }
        public void PopUp(){
            if(!gameObject.activeInHierarchy){
                transform.localScale = Vector3.zero;
                gameObject.SetActive(true);
                StartCoroutine(PopUPRoutine());
            }
            

            
        }
        private IEnumerator PopUPRoutine(){
            while(transform.localScale != popScale){
                iTween.ScaleTo(gameObject,popScale,popUPSpeed);
                yield return null;
            }
            yield return new WaitForSeconds(delay);
            iTween.ScaleTo(gameObject,Vector3.one,popUPSpeed);
            yield return new WaitForSeconds(0.1f);
            OnPopUp?.Invoke();
            
        }
        public void PopDown(){
            if(gameObject.activeInHierarchy){
                StartCoroutine(PopDownRoutine());
            }
            
        }
        private IEnumerator PopDownRoutine(){
            while(transform.localScale != popScale){
                iTween.ScaleTo(gameObject,popScale,popDonwSpeed);
                yield return null;
            }
            OnPopDown?.Invoke();
            yield return new WaitForSeconds(delay);
            iTween.ScaleTo(gameObject,Vector3.zero,popDonwSpeed);
            gameObject.SetActive(false);
        }
    }

}
