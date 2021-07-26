using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpTheGun {

	public class Player : MonoBehaviour {

		/// <summary>
		/// Raise player's origin this much above the top of a box.
		/// </summary>
		public const float Y_OFFSET = .3f;

		public const float BOUNCE_HEIGHT = 2;

		public const float BOUNCE_BASE_DURATION = .7f;

		public Box startBox { get; private set; }
		public Box endBox { get; private set; }

		public enum State {
			NONE,
			IDLE,
			BOUNCING
		}
		public State state { get; private set; }

		public static Player instance { get; private set; }

		public void Spawn(int col, int row){
			
			startBox = TerrainArea.instance.GetBox(col, row);
			endBox = startBox;

			state = State.IDLE;
			transform.localPosition = TerrainArea.instance.LocalPositionFromBox(col, row, startBox.top + Y_OFFSET);

		}

		public Bounds GetBounds(){
			return new Bounds(transform.position, new Vector3(Y_OFFSET * 2, Y_OFFSET * 2, Y_OFFSET * 2));
		}

		void Awake() {
			if (instance != null) {
				Destroy(gameObject);
				return;
			}
			instance = this;
		}

		// Use this for initialization
		void Start () {
			
		}


		
		// Update is called once per frame
		void Update () {

			if (Game.instance.isPaused)
				return;

			// getting local world position of mouse.  Is where camera ray intersects xz plane with y = 
			float y = (Game.instance.minTerrainHeight + Game.instance.maxTerrainHeight) / 2;

			Vector3 mouseWorldPos = new Vector3(0, y, 0);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float t = (y - ray.origin.y) / ray.direction.y;
			mouseWorldPos.x = ray.origin.x + t * ray.direction.x;
			mouseWorldPos.z = ray.origin.z + t * ray.direction.z;
			Vector3 mouseLocalPos = mouseWorldPos;
			if (transform.parent != null) {
				mouseLocalPos = transform.parent.InverseTransformPoint(mouseWorldPos);
			}
			Vector2Int mouseBoxPos = TerrainArea.instance.BoxFromLocalPosition(mouseLocalPos);

			// get position targeted by mouse
			Vector2Int movePos = mouseBoxPos;
			Vector2Int currentPos = TerrainArea.instance.BoxFromLocalPosition(transform.localPosition);
			if (Mathf.Abs(mouseBoxPos.x - currentPos.x) > 1 || Mathf.Abs(mouseBoxPos.y - currentPos.y) > 1) {
				// mouse position is too far away.  Find closest position
				movePos = currentPos;
				if (mouseBoxPos.x != currentPos.x) {
					movePos.x += mouseBoxPos.x > currentPos.x ? 1 : -1;
				}
				if (mouseBoxPos.y != currentPos.y) {
					movePos.y += mouseBoxPos.y > currentPos.y ? 1 : -1;
				}

			}

			// don't move if target is occupied
			if (TerrainArea.instance.OccupiedBox(movePos.x, movePos.y)) {
				movePos.Set(endBox.col, endBox.row);
			}




			time += Time.deltaTime;
			switch (state) {
			case State.IDLE:

				startBox = endBox;

				// look for box to bounce to
				if (movePos == new Vector2Int(startBox.col, startBox.row)) {
					// don't go to new box
					endBox = startBox;
				} else {

					endBox = TerrainArea.instance.GetBox(movePos.x, movePos.y);

					// failsafe
					if (endBox == null)
						endBox = startBox;

				}

				// start bouncing
				Bounce(endBox);

				break;
			case State.BOUNCING:

				if (time >= duration) {
					transform.localPosition = TerrainArea.instance.LocalPositionFromBox(endBox.col, endBox.row, endBox.top + Y_OFFSET);
					state = State.IDLE;
					time = 0;
				} else {
					transform.localPosition = bouncePosition(time / duration);
				}

				break;
			}



		}


		public void Bounce(Box boxTo){
			if (state != State.IDLE)
				return;

			// set start and end positions
			endBox = boxTo;

			// solving parabola path
			float startY = startBox.top + Y_OFFSET;
			float endY = endBox.top + Y_OFFSET;
			float height = Mathf.Max(startY, endY);
			// make height max of adjacent boxes when moving diagonally
			if (startBox.col != endBox.col && startBox.row != endBox.row) {
				height = Mathf.Max(height, TerrainArea.instance.GetBox(startBox.col, endBox.row).top, TerrainArea.instance.GetBox(endBox.col, startBox.row).top);
			}
			height += BOUNCE_HEIGHT;

			Parabola.Create(startY, height, endY, out paraA, out paraB, out paraC);

			state = State.BOUNCING;
			time = 0;

			// duration affected by distance to end box
			Vector2 startPos = new Vector2(startBox.col, startBox.row);
			Vector2 endPos = new Vector2(endBox.col, endBox.row);
			float dist = Vector2.Distance(startPos, endPos);
			duration = Mathf.Max(1, dist) * BOUNCE_BASE_DURATION;

		}

		float time = 0;
		float duration = 1;

		void OnDestroy(){
			if (instance == this) {
				instance = null;
			}
		}



		private float paraA = 1;
		private float paraB = 0;
		private float paraC = 0;

		/// <summary>
		/// Uses parabola to determine bounce position at time t.
		/// </summary>
		/// <param name="t">t in [0, 1].</param>
		private Vector3 bouncePosition(float t){

			float y = Parabola.Solve(paraA, paraB, paraC, t);
			Vector3 startPos = TerrainArea.instance.LocalPositionFromBox(startBox.col, startBox.row);
			Vector3 endPos = TerrainArea.instance.LocalPositionFromBox(endBox.col, endBox.row);
			float x = Mathf.Lerp(startPos.x, endPos.x, t);
			float z = Mathf.Lerp(startPos.z, endPos.z, t);

			return new Vector3(x, y, z);
		}

	}

}