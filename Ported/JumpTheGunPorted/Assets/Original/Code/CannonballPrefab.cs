using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpTheGun {

	public class CannonballPrefab : MonoBehaviour {

		[Header("Prefabs")]
		public GameObject cannonballPrefab;

		public static CannonballPrefab instance { get; private set; }

		void Awake() {
			if (instance != null) {
				Destroy(gameObject);
				return;
			}
			instance = this;
		}

		void OnDestroy() {
			if (instance == this) {
				instance = null;
			}
		}

	}

}