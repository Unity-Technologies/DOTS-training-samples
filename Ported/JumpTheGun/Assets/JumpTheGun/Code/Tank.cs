using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpTheGun {

	public class Tank : MonoBehaviour {

		[Header("Children")]
		public Transform cannon;

		/// <summary>
		/// Raise tank's origin this much above the top of a box.
		/// </summary>
		public const float Y_OFFSET = .4f;

		public Box box { get; set; }

		public float rotation {
			get {
				return transform.rotation.eulerAngles.y;
			}
			set {
				transform.rotation = Quaternion.Euler(0, value, 0);
			}
		}

		public void SetTank(Box box) {
			this.box = box;
			UpdateTranslation();
		}

		public void UpdateTranslation(){
			if (box == null)
				return;
			Vector3 pos = new Vector3();
			pos = box.transform.localPosition;
			pos.y = box.top + Y_OFFSET;
			transform.localPosition = pos;
		}

		public void SetCannonRotation(float rotation){
			cannon.localRotation = Quaternion.Euler(-rotation, 0, 0);
		}

		public void RotateTowardsPlayer() {

			Vector3 diff = Player.instance.transform.position - transform.position;
			float angle = Mathf.Atan2(diff.x, diff.z);
			rotation = angle * Mathf.Rad2Deg;

		}

		// Use this for initialization
		void Start () {
			// offset launch time for each tank
			time = Random.Range(0, Game.instance.tankLaunchPeriod);
		}
		
		// Update is called once per frame
		void Update () {

			if (Game.instance.isPaused)
				return;

			UpdateTranslation();
			RotateTowardsPlayer();

			time += Time.deltaTime;
			if (time >= Game.instance.tankLaunchPeriod) {
				
				// launching cannonball
				time -= Game.instance.tankLaunchPeriod;

				// start and end positions
				Vector3 start = transform.localPosition;
				Vector2Int playerBox = TerrainArea.instance.BoxFromLocalPosition(Player.instance.transform.localPosition);
				Vector3 end = TerrainArea.instance.LocalPositionFromBox(playerBox.x, playerBox.y, TerrainArea.instance.GetBox(playerBox.x, playerBox.y).top + Cannonball.RADIUS);
				float distance = (new Vector2(end.z - start.z, end.x - start.x)).magnitude;
				float duration = distance / Cannonball.SPEED;
				if (duration < .0001f)
					duration = 1;

				// binary searching to determine height of cannonball arc
				float low = Mathf.Max(start.y, end.y);
				float high = low * 2;
				float paraA, paraB, paraC;

				// look for height of arc that won't hit boxes
				while (true) {
					Parabola.Create(start.y, high, end.y, out paraA, out paraB, out paraC);
					if (!Cannonball.CheckBoxCollision(start, end, paraA, paraB, paraC)) {
						// high enough
						break;
					}
					// not high enough.  Double value
					low = high;
					high *= 2;
					// failsafe
					if (high > 9999) {
						return; // skip launch
					}
				}

				// do binary searches to narrow down
				while (high - low > Game.instance.playerParabolaPrecision) {
					float mid = (low + high) / 2;
					Parabola.Create(start.y, mid, end.y, out paraA, out paraB, out paraC);
					if (Cannonball.CheckBoxCollision(start, end, paraA, paraB, paraC)) {
						// not high enough
						low = mid;
					} else {
						// too high
						high = mid;
					}
				}

				// launch with calculated height
				float height = (low + high) / 2;
				Cannonball cannonball = Cannonball.Create(transform.parent, start);
				cannonball.Launch(end, height, duration);

				// set cannon rotation
				SetCannonRotation(Mathf.Atan2(Parabola.Solve(paraA, paraB, paraC, .1f) - Parabola.Solve(paraA, paraB, paraC, 0), .1f) * Mathf.Rad2Deg);

			}

		}

		void OnDestroy() {
			box = null;
		}

		float time = 0;

	}

}