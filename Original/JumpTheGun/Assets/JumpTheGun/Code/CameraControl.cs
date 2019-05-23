using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpTheGun {

	public class CameraControl : MonoBehaviour {

		public const float FOLLOW_HEIGHT_OFFSET_MIN = 4;
		public const float SMOOTH_DAMP_DURATION = .5f;

		public float ySpeed = 5;

		public static CameraControl instance { get; private set; }

		public enum Mode {
			NONE,
			FOLLOW
		}
		public Mode mode { get; private set; }

		public void Follow() {
			mode = Mode.FOLLOW;
			Vector3 target = Player.instance.transform.localPosition;
			maxTerrainHeight = Game.instance.maxTerrainHeight;
			target.y = maxTerrainHeight + followHeightOffset;
			transform.localPosition = ApplyCameraRotationOffset(target);
			camVel.Set(0, 0, 0);
		}

		// Use this for initialization
		void Awake() {
			if (instance != null) {
				Destroy(gameObject);
				return;
			}
			instance = this;

		}

		private Vector3 ApplyCameraRotationOffset(Vector3 pos){
			float zOff = (pos.y - maxTerrainHeight) / Mathf.Tan(Camera.main.transform.rotation.eulerAngles.x * Mathf.Rad2Deg);
			pos.z += zOff;
			return pos;
		}

		void Update() {

			Vector3 pos = transform.localPosition;

			switch (mode) {
			case Mode.FOLLOW:

				// controlling height offset
				if (Input.GetKey(KeyCode.DownArrow)) {
					followHeightOffset += ySpeed * Time.unscaledDeltaTime;
				} else if (Input.GetKey(KeyCode.UpArrow)) {
					followHeightOffset -= ySpeed * Time.unscaledDeltaTime;
				}
				followHeightOffset = Mathf.Max(followHeightOffset, FOLLOW_HEIGHT_OFFSET_MIN);

				float height = maxTerrainHeight + followHeightOffset;
				Vector3 target = Player.instance.transform.localPosition;
				target.y = height;

				target = ApplyCameraRotationOffset(target);

				pos = Vector3.SmoothDamp(transform.localPosition, target, ref camVel, SMOOTH_DAMP_DURATION, float.MaxValue, Time.unscaledDeltaTime);
				break;
			}


			transform.localPosition = pos;

		}
		private float maxTerrainHeight = 0;
		private Vector3 camVel = new Vector3();
		private float followHeightOffset = 10;



//		public void setPosition(float height){
//
//			Vector3 pos = new Vector3(
//				TerrainArea.instance.width / 2f * Box.SPACING,
//				height,
//				TerrainArea.instance.length / 2f * Box.SPACING
//			);
//
//			float terrainHeightBase = (Game.instance.minTerrainHeight + Game.instance.maxTerrainHeight) / 2;
//			float zOff = (height - terrainHeightBase) / Mathf.Tan(Camera.main.transform.rotation.eulerAngles.x * Mathf.Rad2Deg);
//			pos.z += zOff;
//
//			Camera.main.transform.localPosition = pos;
//
//		}
//
//
//		// Update is called once per frame
//		void Update () {
//
//			Vector3 pos = transform.localPosition;
//
//			float yMin = Game.instance.maxTerrainHeight;
//
//			// camera control
//			if (Input.GetKey(KeyCode.DownArrow)) {
//				pos.y += ySpeed * Time.unscaledDeltaTime;
//			} else if (Input.GetKey(KeyCode.UpArrow)) {
//				pos.y -= ySpeed * Time.unscaledDeltaTime;
//			}
//			pos.y = Mathf.Max(yMin, pos.y);
//
//			setPosition(pos.y);
//
//		}

//		public void PositionCamera() {
//
//			// determine height
//			float area = Mathf.Max(width * Box.SPACING, length * Box.SPACING * Camera.main.aspect);
//			float height = area / 2 / Mathf.Tan(Camera.main.fieldOfView / 2 * Mathf.Deg2Rad);
//			float terrainHeightBase = (Game.instance.minTerrainHeight + Game.instance.maxTerrainHeight) / 2;
//			height += terrainHeightBase;
//
//			CameraControl.instance.setPosition(height);
//
//		}

		void OnDestroy(){
			if (instance == this) {
				instance = null;
			}
		}
	}

}