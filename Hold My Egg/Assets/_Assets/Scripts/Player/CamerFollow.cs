using UnityEngine;


namespace WolfGamer.Hold_My_Eggs{

    public class CamerFollow : MonoBehaviour {
        [SerializeField] private Vector3 offset;
        private Transform targetT;
        private bool startFollow;
        [SerializeField] private float moveSpeed;
        [SerializeField] private Transform nestPos;
        [SerializeField] private Vector3 offsetToNest;

        #region Singelton.........
        public static CamerFollow i;
        private void Awake(){
            if(i == null){
                i = this;
            }
        }

        #endregion



        private void LateUpdate(){
            if(startFollow && targetT != null){
                Vector3 pos = targetT.position + offset;
                Vector3 targetpos = new Vector3 (transform.position.x,pos.y,transform.position.z);
                
                if(targetpos.y < nestPos.position.y){
                    transform.position = Vector3.Lerp(transform.position,targetpos,moveSpeed * Time.deltaTime);
                }
            }
        }
        public void SetTarget(Transform _T){
            targetT = _T;
        }
        
        public void StartFollowTarget(bool _follow){
            startFollow = _follow;
        }
        public void SetNestPos(Transform nestT){
            nestPos = nestT;
        }

    }

}