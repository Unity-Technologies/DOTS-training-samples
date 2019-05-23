using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace ZombieMaze {

	public class Options : MonoBehaviour {

		[Header("Children")]
		public SliderProp mazeWidth;
		public SliderProp mazeLength;
		public SliderProp numMovingWalls;
		public SliderProp numZombies;
		public SliderProp numCapsules;
		public SliderProp openStripsWidth;
		public SliderProp mazeStripsWidth;
		public SliderProp movingWallSize;

		public void UpdateGame(){

			Maze.instance.GenerateMaze(Mathf.RoundToInt(mazeWidth.value), Mathf.RoundToInt(mazeLength.value));
			Ball.instance.MoveToHomeBase();
		}

		void Awake() {


		}

		void Start() {

			mazeWidth.SetBounds(1, 200);
			mazeWidth.value = Maze.instance.initialWidth;

			mazeLength.SetBounds(1, 200);
			mazeLength.value = Maze.instance.initialLength;

			numMovingWalls.SetBounds(0, 50);
			numMovingWalls.value = Maze.instance.numMovingWalls;

			numZombies.SetBounds(0, 500);
			numZombies.value = Maze.instance.numZombies;

			numCapsules.SetBounds(0, 100);
			numCapsules.value = Maze.instance.numCapsules;

			openStripsWidth.SetBounds(0, 100);
			openStripsWidth.value = Maze.instance.openStripsWidth;

			mazeStripsWidth.SetBounds(0, 100);
			mazeStripsWidth.value = Maze.instance.mazeStripsWidth;

			movingWallSize.SetBounds(1, 100);
			movingWallSize.value = Maze.instance.movingWallSize;


			UpdateSliders();
		}

		void UpdateSliders(){

			Maze.instance.initialWidth = Mathf.RoundToInt(mazeWidth.value);
			mazeWidth.SetText("Maze Width: " + Maze.instance.initialWidth);

			Maze.instance.initialLength = Mathf.RoundToInt(mazeLength.value);
			mazeLength.SetText("Maze Length: " + Maze.instance.initialLength);

			Maze.instance.numMovingWalls = Mathf.RoundToInt(numMovingWalls.value);
			numMovingWalls.SetText("Number of Moving Walls: " + Maze.instance.numMovingWalls);

			Maze.instance.numZombies = Mathf.RoundToInt(numZombies.value);
			numZombies.SetText("Number of Zombies: " + Maze.instance.numZombies);

			Maze.instance.numCapsules = Mathf.RoundToInt(numCapsules.value);
			numCapsules.SetText("Number of Capsules: " + Maze.instance.numCapsules);

			Maze.instance.openStripsWidth = Mathf.RoundToInt(openStripsWidth.value);
			openStripsWidth.SetText("Open Strips Width: " + Maze.instance.openStripsWidth);

			Maze.instance.mazeStripsWidth = Mathf.RoundToInt(mazeStripsWidth.value);
			mazeStripsWidth.SetText("Maze Strips Width: " + Maze.instance.mazeStripsWidth);

			Maze.instance.movingWallSize = Mathf.RoundToInt(movingWallSize.value);
			movingWallSize.SetText("Moving Wall Size: " + Maze.instance.movingWallSize);


		}


		// Update is called once per frame
		void Update () {

			UpdateSliders();
		}
	}

}