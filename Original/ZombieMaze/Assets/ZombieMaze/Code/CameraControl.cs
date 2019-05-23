using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieMaze {

	public class CameraControl : MonoBehaviour {

		public float yMin = 5;
		public float ySpeed = 5;

		public const float SMOOTH_DAMP_DURATION = .1f;

		public void InitialSetUp() {

			// reposition camera
			Camera.main.transform.position = new Vector3 (
				Maze.instance.Width * Maze.TILE_SPACING / 2,
				20,
				Maze.instance.Length * Maze.TILE_SPACING / 2
			);

			// initial tilt:
			Camera.main.transform.rotation = Quaternion.Euler(45, 0, 0);

			target = Camera.main.transform.position;

		}

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
			// control height with arrow keys
			if (Input.GetKey (KeyCode.DownArrow)) {
				target.y += ySpeed * Time.unscaledDeltaTime;
			} else if (Input.GetKey (KeyCode.UpArrow)) {
				target.y -= ySpeed * Time.unscaledDeltaTime;
			}
			target.y = Mathf.Max(yMin, target.y);

			// hover over ball
			target.x = Ball.instance.transform.position.x;
			target.z = Ball.instance.transform.position.z;

			// offset for camera rotation
			float zOff = (target.y - Ball.instance.transform.position.y) / Mathf.Tan(Camera.main.transform.rotation.eulerAngles.x * Mathf.Rad2Deg);
			target.z += zOff;

			// applying smooth damp
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, target, ref camVel, SMOOTH_DAMP_DURATION, float.MaxValue, Time.unscaledDeltaTime);

		}

		private Vector3 target = new Vector3();
		private Vector3 camVel = new Vector3();
	}

}