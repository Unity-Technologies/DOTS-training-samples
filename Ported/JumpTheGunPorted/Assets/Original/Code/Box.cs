using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpTheGun {

	public class Box : MonoBehaviour {

		public const float SPACING = 1;
		public const float Y_OFFSET = 0;
		public const float HEIGHT_MIN = .5f;
		public static Color MIN_HEIGHT_COLOR = Color.green;
		public static Color MAX_HEIGHT_COLOR = new Color(99 /255f, 47 /255f, 0 /255f);

		public int col { get; private set; }
		public int row { get; private set; }

		public float height {
			get {
				return _height;
			}
			set {
				_height = value;
				UpdateTransform();
			}
		}

		public float top {
			get {
				return transform.localPosition.y + height / 2;
			}
		}

		public Color color {
			get {
				return meshRenderer.material.color;
			}
			set {
				meshRenderer.material.color = value;
			}
		}

		public void SetBox(int col, int row, float height){
			this.col = col;
			this.row = row;
			this.height = height;
			UpdateTransform();
		}

		public bool HitsCube(Vector3 center, float width) {

			Bounds boxBounds = new Bounds(transform.localPosition, new Vector3(SPACING, height, SPACING));
			Bounds cubeBounds = new Bounds(center, new Vector3(width, width, width));

			return boxBounds.Intersects(cubeBounds);
		}

		public void UpdateTransform(){

			Vector3 pos = new Vector3(col * SPACING, height / 2 + Y_OFFSET, row * SPACING);
			Vector3 scale = new Vector3(1, height, 1);

			transform.localPosition = pos;
			transform.localScale = scale;

			// change color based on height
			if (Mathf.Approximately(Game.instance.maxTerrainHeight, HEIGHT_MIN)) {
				color = MIN_HEIGHT_COLOR;
			} else {
				color = Color.Lerp(MIN_HEIGHT_COLOR, MAX_HEIGHT_COLOR, (height - HEIGHT_MIN) / (Game.instance.maxTerrainHeight - HEIGHT_MIN));
			}

		}

		public void TakeDamage(){
			height = Mathf.Max(HEIGHT_MIN, height - Game.instance.boxHeightDamage);
		}

		// Use this for initialization
		void Awake() {
			meshRenderer = GetComponent<MeshRenderer>();
		}

		void Start(){
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		private float _height = 1;

		private MeshRenderer meshRenderer;

	}

}