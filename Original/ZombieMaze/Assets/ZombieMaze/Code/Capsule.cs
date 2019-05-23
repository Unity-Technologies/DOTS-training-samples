using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieMaze {

	public class Capsule : MonoBehaviour {

		public float spinSpeed = 150;

		public int col { get; set; }
		public int row { get; set; }

		public bool captured { get; private set; }

		public void UpdatePosition() {
			transform.localPosition = Maze.GetTransformPosition(col, row, transform.localPosition.y);
		}

		public void Capture(){
			if (captured)
				return;
			captured = true;
			meshRenderer.enabled = false;
		}

		public void Reset(){
			if (!captured)
				return;
			captured = false;
			meshRenderer.enabled = true;
		}

		// Use this for initialization
		void Awake () {
			meshRenderer = GetComponent<MeshRenderer>();
		}
		
		// Update is called once per frame
		void Update () {

			// spin
			rotation += spinSpeed * Time.deltaTime;
			rotation -= Mathf.Floor(rotation / 360) * 360;

			transform.localRotation = Quaternion.Euler (0, rotation, 90);

		}

		float rotation = 0;

		MeshRenderer meshRenderer;

	}

}