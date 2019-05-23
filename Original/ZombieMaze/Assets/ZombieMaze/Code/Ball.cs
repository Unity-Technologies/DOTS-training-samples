using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieMaze {

	public class Ball : Character {

		public static Ball instance { get; private set; }

		public void MoveToHomeBase() {
			Stop();
			colF = Maze.instance.HomeBase.x;
			rowF = Maze.instance.HomeBase.y;
			UpdateLocalPosition();
		}

		public override float moveSpeed {
			get {
				return Maze.instance.ballSpeed;
			}
			set {
				Maze.instance.ballSpeed = value;
			}
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
		protected override void Update() {
			
			// getting local world position of mouse.  Is where camera ray intersects xz plane with y = ball.y
			Vector3 mouseWorldPos = new Vector3(0, transform.position.y, 0);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float t = (transform.position.y - ray.origin.y) / ray.direction.y;
			mouseWorldPos.x = ray.origin.x + t * ray.direction.x;
			mouseWorldPos.z = ray.origin.z + t * ray.direction.z;
			Vector3 mouseLocalPos = transform.parent.InverseTransformPoint(mouseWorldPos);

			Vector2 mouseTilePos = Maze.GetTilePositionF(mouseLocalPos);
			Vector2Int mouseTilePosR = new Vector2Int (
				Mathf.RoundToInt(mouseTilePos.x),
				Mathf.RoundToInt(mouseTilePos.y)
			);
			Vector2 diff = mouseTilePos - new Vector2(col, row);

			if (moveState == MoveState.IDLE) {
				// move towards mouse
				if (mouseTilePosR != new Vector2Int(col, row)) {
					Direction direction = Direction.NONE;
					if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y)) {
						direction = diff.x > 0 ? Direction.RIGHT : Direction.LEFT;
					} else {
						direction = diff.y > 0 ? Direction.UP : Direction.DOWN;
					}
					MoveDirection(direction);
				}
			}

			// movement
			base.Update();

			// capture capsule?
			if (moveState == MoveState.IDLE) {
				Capsule capsule = Maze.instance.GetCapsule(col, row);
				if (capsule != null && !capsule.captured) {
					capsule.Capture();
					if (Maze.instance.AllCapsulesCaptured()) {
						// reset game
						Maze.instance.GenerateMaze(Maze.instance.Width, Maze.instance.Length);
						Ball.instance.MoveToHomeBase();
					}
				}
			}

		}

		void OnDestroy() {
			if (instance == this) {
				instance = null;
			}
		}

	}

}