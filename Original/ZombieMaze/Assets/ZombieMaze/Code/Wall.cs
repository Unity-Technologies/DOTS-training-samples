using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieMaze {

	[RequireComponent(typeof(MeshRenderer))]
	public class Wall : MonoBehaviour {

		public Color color1 = Color.gray;
		public Color color2 = Color.black;

		// Use this for initialization
		void Awake() {
			GetComponent<MeshRenderer>().material.color = Color.Lerp(color1, color2, Random.value);
		}

	}

}