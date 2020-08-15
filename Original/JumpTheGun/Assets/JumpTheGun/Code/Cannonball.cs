using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpTheGun {

	public class Cannonball : MonoBehaviour {

		public const float RADIUS = .25f;
		public const float SPEED = 2.5f;

		public static Cannonball Create(Transform parent, Vector3 localPosition) {
			Cannonball ret;
			if (recycledCannonballs.Count > 0) {
				ret = recycledCannonballs.Pop();
				ret.recycled = false;
			} else {
				ret = Instantiate(CannonballPrefab.instance.cannonballPrefab).GetComponent<Cannonball>();
			}
			ret.gameObject.SetActive(true);
			ret.transform.SetParent(parent, false);
			ret.transform.localPosition = localPosition;
			activeCannonballs.AddLast(ret);
			return ret;
		}
		public static void Recycle(Cannonball cannonball) {
			if (cannonball.recycled)
				return;
			cannonball.recycled = true;
			cannonball.gameObject.SetActive(false);
			recycledCannonballs.Push(cannonball);
			activeCannonballs.Remove(cannonball);
		}
		public static void RecycleAllCannonballs(){
			while (activeCannonballs.Count > 0) {
				Recycle(activeCannonballs.First.Value);
			}
		}
		private static Stack<Cannonball> recycledCannonballs = new Stack<Cannonball>();
		public bool recycled { get; private set; }
		private static LinkedList<Cannonball> activeCannonballs = new LinkedList<Cannonball>();


		public void Launch(Vector3 targetPosition, float height, float duration) {

			startPosition = transform.localPosition;
			this.targetPosition = targetPosition;
			Parabola.Create(startPosition.y, height, targetPosition.y, out paraA, out paraB, out paraC);

			time = 0;
			this.duration = duration;
		}

		/// <summary>
		/// Simulates firing a cannonball with the given trajectory.
		/// Returns true if the cannonball would hit a box on the way there.
		/// </summary>
		public static bool CheckBoxCollision(Vector3 start, Vector3 end, float paraA, float paraB, float paraC){

			Vector3 diff = end - start;
			float distance = (new Vector2(diff.x, diff.z)).magnitude;

			int steps = Mathf.Max(2, Mathf.CeilToInt(distance / Box.SPACING) + 1);

			steps = Mathf.CeilToInt(steps * Game.instance.collisionStepMultiplier);

			for (int i = 0; i < steps; i++) {
				float t = i / (steps - 1f);

				Vector3 pos = GetSimulatedPosition(start, end, paraA, paraB, paraC, t);

				if (TerrainArea.instance.HitsCube(pos, RADIUS)) {
					return true;
				}

			}

			return false;
		}

		private float time;
		private float duration = 1;
		private Vector3 startPosition;
		private Vector3 targetPosition;
		private float paraA;
		private float paraB;
		private float paraC;


		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {

			if (Game.instance.isPaused)
				return;

			time += Time.deltaTime;

			transform.localPosition = GetSimulatedPosition(startPosition, targetPosition, paraA, paraB, paraC, time / duration);

			// destroy
			if (time >= duration) {
				// damage box cannonball hit
				Vector2Int boxPos = TerrainArea.instance.BoxFromLocalPosition(targetPosition);
				Box box = TerrainArea.instance.GetBox(boxPos.x, boxPos.y);
				box.TakeDamage();

				Recycle(this);
				return;
			}

			// detect hitting player
			Bounds bounds = new Bounds(transform.position, new Vector3(RADIUS*2, RADIUS*2, RADIUS*2));
			if (bounds.Intersects(Player.instance.GetBounds()) && !Game.instance.invincibility) {
				// reset game
				Game.instance.StartGame();
			}


		}

		public static Vector3 GetSimulatedPosition(Vector3 start, Vector3 end, float paraA, float paraB, float paraC, float t){
			return new Vector3(
				Mathf.LerpUnclamped(start.x, end.x, t),
				Parabola.Solve(paraA, paraB, paraC, t),
				Mathf.LerpUnclamped(start.z, end.z, t)
			);
		}
	}

}