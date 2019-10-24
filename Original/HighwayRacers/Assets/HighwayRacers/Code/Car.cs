using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighwayRacers
{
    public class Car : MonoBehaviour
    {
        [Header("Children")]
		public MeshRenderer topRenderer;
        public MeshRenderer baseRenderer;
		public Transform cameraPos;

        public Color color
        {
            get
            {
                return topRenderer.material.color;
            }
            set
            {
				topRenderer.material.color = value;
                baseRenderer.material.color = value;
            }
        }

		public void Show() {
			if (!hidden)
				return;
			topRenderer.enabled = true;
			baseRenderer.enabled = true;
			hidden = false;
		}

		public void Hide() {
			if (hidden)
				return;
			topRenderer.enabled = false;
			baseRenderer.enabled = false;
			hidden = true;

		}
		private bool hidden = false;
    }
}
