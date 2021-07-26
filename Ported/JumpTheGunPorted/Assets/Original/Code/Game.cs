using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JumpTheGun {

	public class Game : MonoBehaviour {

		[Tooltip("Width of the terrain, in boxes")]
		public int terrainWidth = 15;
		[Tooltip("Length of the terrain, in boxes")]
		public int terrainLength = 10;

		public float minTerrainHeight = 2.5f;

		public float maxTerrainHeight = 5.5f;

		[Tooltip("The amount a box's height decreases when getting hit by a cannonball")]
		public float boxHeightDamage = .4f;

		public int numTanks = 5;

		[Tooltip("Time in-between each time a tank launches a cannonball.")]
		public float tankLaunchPeriod = 1;

		[Tooltip("Increases the number of checks made detecting if a cannonball in its trajectory touches a box (which is also proportional on distance to the target).")]
		public float collisionStepMultiplier = 3;

		[Tooltip("Break the minimum arc height search when the bounds become this small")]
		public float playerParabolaPrecision = .1f;

        [Tooltip("I'm INVINCIBLE!")]
        public bool invincibility = false;

		[Header("Scene")]
		public Text timeText;

		[HideInInspector]
		public bool isPaused = false;

		public static Game instance { get; private set; }

		public void StartGame(){
			Cannonball.RecycleAllCannonballs();
			TerrainArea.instance.CreateBoxes(terrainWidth, terrainLength);
			Vector2Int plrPos = TerrainArea.instance.CreateTanks(numTanks);
			Player.instance.Spawn(plrPos.x, plrPos.y);

			CameraControl.instance.Follow();

			time = 0;
		}

		public float time { get; private set; }

		void Awake() {

			if (instance != null) {
				Destroy(gameObject);
				return;
			}
			instance = this;

		}

		void Start() {
			StartGame();
		}
		
		// Update is called once per frame
		void Update () {

			if (Game.instance.isPaused)
				return;

			time += Time.deltaTime;

			int mins = Mathf.FloorToInt(time / 60);
			int secs = Mathf.FloorToInt(time - mins * 60);
			timeText.text = "Time: " + mins.ToString("00") + ":" + secs.ToString("00");

		}

		void OnDestroy() {
			if (instance == this) {
				instance = null;
			}
		}
	}

}