using UnityEngine;
using System.Collections;

namespace WolfGamer.Utils{
	[ExecuteInEditMode]
	public class ScreenAnchor : MonoBehaviour {
		public enum AnchorType {
			BottomLeft,
			BottomCenter,
			BottomRight,
			MiddleLeft,
			MiddleCenter,
			MiddleRight,
			TopLeft,
			TopCenter,
			TopRight,
		};
		public AnchorType anchorType;
		public Vector3 anchorOffset;
		public bool playOnce;

		IEnumerator updateAnchorRoutine; //Coroutine handle so we don't start it if it's already running

		private void Start () {
			updateAnchorRoutine = UpdateAnchorAsync();
			StartCoroutine(updateAnchorRoutine);
			
		}
		IEnumerator UpdateAnchorAsync() {
			uint cameraWaitCycles = 0;
			while(CameraViewHandler.Instance == null) {
				++cameraWaitCycles;
				yield return new WaitForEndOfFrame();
			}
			if (cameraWaitCycles > 0) {
				print(string.Format("CameraAnchor found ViewportHandler instance after waiting {0} frame(s). You might want to check that ViewportHandler has an earlie execution order.", cameraWaitCycles));
			}
			UpdateAnchor();
			updateAnchorRoutine = null;
		}

		void UpdateAnchor() {
			switch(anchorType) {
			case AnchorType.BottomLeft:
				SetAnchor(CameraViewHandler.Instance.BottomLeft);
				break;
			case AnchorType.BottomCenter:
				SetAnchor(CameraViewHandler.Instance.BottomCenter);
				break;
			case AnchorType.BottomRight:
				SetAnchor(CameraViewHandler.Instance.BottomRight);
				break;
			case AnchorType.MiddleLeft:
				SetAnchor(CameraViewHandler.Instance.MiddleLeft);
				break;
			case AnchorType.MiddleCenter:
				SetAnchor(CameraViewHandler.Instance.MiddleCenter);
				break;
			case AnchorType.MiddleRight:
				SetAnchor(CameraViewHandler.Instance.MiddleRight);
				break;
			case AnchorType.TopLeft:
				SetAnchor(CameraViewHandler.Instance.TopLeft);
				break;
			case AnchorType.TopCenter:
				SetAnchor(CameraViewHandler.Instance.TopCenter);
				break;
			case AnchorType.TopRight:
				SetAnchor(CameraViewHandler.Instance.TopRight);
				break;
			}
		}

		void SetAnchor(Vector3 anchor) {
			Vector3 newPos = anchor + anchorOffset;
			if (!transform.position.Equals(newPos)) {
				transform.position = newPos;
			}
		}

		
		void Update () {
			if (updateAnchorRoutine == null) {
				updateAnchorRoutine = UpdateAnchorAsync();
				StartCoroutine(updateAnchorRoutine);
			}
			if(Application.isPlaying){
				if(playOnce){
					Destroy(this);
				}

			}
			
		}
		

	}
}
