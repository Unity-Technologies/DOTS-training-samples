using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace JumpTheGun {

	public class Options : MonoBehaviour {

		[Header("Children")]
		public SliderProp terrainWidth;
		public SliderProp terrainLength;
		public SliderProp minTerrainHeight;
		public SliderProp maxTerrainHeight;


		public SliderProp boxHeightDamage;
		public SliderProp numTanks;

		public SliderProp tankLaunchPeriod;
		public SliderProp collisionStepMultiplier;
		public SliderProp invPlayerParabolaPrecision;

		public void UpdateGame(){

			Game.instance.terrainWidth = Mathf.RoundToInt(terrainWidth.value);
			Game.instance.terrainLength = Mathf.RoundToInt(terrainLength.value);

			Game.instance.StartGame();
		}

		void Awake() {
			

		}

		void Start() {
			
			terrainWidth.SetBounds(1, 100);
			terrainWidth.value = Game.instance.terrainWidth;

			terrainLength.SetBounds(1, 100);
			terrainLength.value = Game.instance.terrainLength;

			minTerrainHeight.SetBounds(Box.HEIGHT_MIN, 10);
			minTerrainHeight.value = Game.instance.minTerrainHeight;

			maxTerrainHeight.SetBounds(Box.HEIGHT_MIN, 10);
			maxTerrainHeight.value = Game.instance.maxTerrainHeight;

			boxHeightDamage.SetBounds(0, 10);
			boxHeightDamage.value = Game.instance.boxHeightDamage;

			numTanks.SetBounds(0, 1000);
			numTanks.value = Game.instance.numTanks;

			tankLaunchPeriod.SetBounds(.1f, 20);
			tankLaunchPeriod.value = Game.instance.tankLaunchPeriod;

			collisionStepMultiplier.SetBounds(.1f, 10);
			collisionStepMultiplier.value = Game.instance.collisionStepMultiplier;

			invPlayerParabolaPrecision.SetBounds(1/1f, 1/.001f);
			invPlayerParabolaPrecision.value = 1 / Game.instance.playerParabolaPrecision;


			UpdateSliders();
		}

		void UpdateSliders(){

			Game.instance.terrainWidth = Mathf.RoundToInt(terrainWidth.value);
			terrainWidth.SetText("Terrain Width: " + Game.instance.terrainWidth);

			Game.instance.terrainLength = Mathf.RoundToInt(terrainLength.value);
			terrainLength.SetText("Terrain Length: " + Game.instance.terrainLength);


			Game.instance.minTerrainHeight = minTerrainHeight.value;
			minTerrainHeight.SetText("Min Terrain Height: " + Game.instance.minTerrainHeight.ToString("0.0"));

			if (maxTerrainHeight.value < minTerrainHeight.value) {
				maxTerrainHeight.value = minTerrainHeight.value;
			}
			Game.instance.maxTerrainHeight = maxTerrainHeight.value;
			maxTerrainHeight.SetText("Max Terrain Height: " + Game.instance.maxTerrainHeight.ToString("0.0"));

			Game.instance.boxHeightDamage = boxHeightDamage.value;
			boxHeightDamage.SetText("Terrain Height Decrease When Hit: " + Game.instance.boxHeightDamage.ToString("0.0"));

			Game.instance.numTanks = Mathf.RoundToInt(numTanks.value);
			numTanks.SetText("Number of Tanks: " + Game.instance.numTanks);

			Game.instance.tankLaunchPeriod = tankLaunchPeriod.value;
			tankLaunchPeriod.SetText("Tank Reload Time: " + Game.instance.tankLaunchPeriod.ToString("0.0"));

			Game.instance.collisionStepMultiplier = collisionStepMultiplier.value;
			collisionStepMultiplier.SetText("Cannonball Parabola Collision Tests per Tile: " + Game.instance.collisionStepMultiplier.ToString("0.0"));

			Game.instance.playerParabolaPrecision = 1 / invPlayerParabolaPrecision.value;
			invPlayerParabolaPrecision.SetText("Player Parabola Precision: " + (1 / Game.instance.playerParabolaPrecision).ToString("0.0"));

		}

		
		// Update is called once per frame
		void Update () {

			UpdateSliders();
		}
	}

}