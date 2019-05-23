using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieMaze {

	[RequireComponent(typeof(MeshRenderer))]
	public class Tile : MonoBehaviour {

		public Color homeBaseColor = Color.yellow;

		public bool IsHomeBase {
			get {
				return homeBase;
			}
			set {
				if (value == homeBase)
					return;
				homeBase = value;
				if (homeBase) {
					meshRenderer.material.color = homeBaseColor;
				} else {
					meshRenderer.material.color = defaultColor;
				}
			}
		}


		void Awake() {
			meshRenderer = GetComponent<MeshRenderer>();
			defaultColor = meshRenderer.material.color;
		}

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		MeshRenderer meshRenderer;
		Color defaultColor;
		bool homeBase = false;

	}

}