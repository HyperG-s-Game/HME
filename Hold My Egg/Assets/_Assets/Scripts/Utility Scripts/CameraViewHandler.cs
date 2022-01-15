using UnityEngine;
namespace WolfGamer.Utils {
    [ExecuteInEditMode]
    [RequireComponent (typeof (Camera))]
    public class CameraViewHandler : MonoBehaviour {
        #region FIELDS
        [SerializeField] private Color wireColor = Color.white;
        [SerializeField] private float screenSize = 6.63f;
        [SerializeField] private Constraint constraint = Constraint.Portrait;
        public static CameraViewHandler Instance;
        private Camera m_Cam;

        private float _width;
        private float _height;
        //*** bottom screen
        private Vector3 _bl;
        private Vector3 _bc;
        private Vector3 _br;
        //*** middle screen
        private Vector3 _ml;
        private Vector3 _mc;
        private Vector3 _mr;
        //*** top screen
        private Vector3 _tl;
        private Vector3 _tc;
        private Vector3 _tr;
        #endregion

        #region PROPERTIES
        public float Width {
            get {
                return _width;
            }
        }
        public float Height {
            get {
                return _height;
            }
        }

        // helper points:
        public Vector3 BottomLeft {
            get {
                return _bl;
            }
        }
        public Vector3 BottomCenter {
            get {
                return _bc;
            }
        }
        public Vector3 BottomRight {
            get {
                return _br;
            }
        }
        public Vector3 MiddleLeft {
            get {
                return _ml;
            }
        }
        public Vector3 MiddleCenter {
            get {
                return _mc;
            }
        }
        public Vector3 MiddleRight {
            get {
                return _mr;
            }
        }
        public Vector3 TopLeft {
            get {
                return _tl;
            }
        }
        public Vector3 TopCenter {
            get {
                return _tc;
            }
        }
        public Vector3 TopRight {
            get {
                return _tr;
            }
        }
        #endregion

        #region METHODS
        private void Awake(){
            Instance = this;
            m_Cam = GetComponent<Camera>();
            ComputeResolution();
        }
        

        private void ComputeResolution(){
            float leftX, rightX, topY, bottomY;

            if(constraint == Constraint.Landscape){
                m_Cam.orthographicSize = 1f / m_Cam.aspect * screenSize / 2f;    
            }else{
                m_Cam.orthographicSize = screenSize / 2f;
            }

            _height = 2f * m_Cam.orthographicSize;
            _width = _height * m_Cam.aspect;

            float cameraX, cameraY;
            cameraX = m_Cam.transform.position.x;
            cameraY = m_Cam.transform.position.y;

            leftX = cameraX - _width / 2;
            rightX = cameraX + _width / 2;
            topY = cameraY + _height / 2;
            bottomY = cameraY - _height / 2;

            //*** bottom
            _bl = new Vector3(leftX, bottomY, 0);
            _bc = new Vector3(cameraX, bottomY, 0);
            _br = new Vector3(rightX, bottomY, 0);
            //*** middle
            _ml = new Vector3(leftX, cameraY, 0);
            _mc = new Vector3(cameraX, cameraY, 0);
            _mr = new Vector3(rightX, cameraY, 0);
            //*** top
            _tl = new Vector3(leftX, topY, 0);
            _tc = new Vector3(cameraX, topY , 0);
            _tr = new Vector3(rightX, topY, 0);           
        }

        private void Update(){
            ComputeResolution();
            
        }

        void OnDrawGizmos() {
            Gizmos.color = wireColor;

            Matrix4x4 temp = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            if (m_Cam.orthographic) {
                float spread = m_Cam.farClipPlane - m_Cam.nearClipPlane;
                float center = (m_Cam.farClipPlane + m_Cam.nearClipPlane)*0.5f;
                Gizmos.DrawWireCube(new Vector3(0,0,center), new Vector3(m_Cam.orthographicSize*2*m_Cam.aspect, m_Cam.orthographicSize*2, spread));
            } else {
                Gizmos.DrawFrustum(Vector3.zero, m_Cam.fieldOfView, m_Cam.farClipPlane, m_Cam.nearClipPlane, m_Cam.aspect);
            }
            Gizmos.matrix = temp;
        }
        #endregion

        public enum Constraint { Landscape, Portrait }
    }
}