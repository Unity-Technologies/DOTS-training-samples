using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighwayRacers
{

    public class StraightPiece : HighwayPiece
    {

        public float baseLength = 6;
        public float baseScaleY = 6;

        [Header("Children")]
        public Transform meshTransform;

        public override float length(float lane)
        {
            return _length;
        }
        public void SetLength(float value)
        {
            if (_length == value) return;
            _length = value;
            meshTransform.localScale = new Vector3(meshTransform.localScale.x, _length / baseLength * baseScaleY, transform.localScale.z);
            meshTransform.localPosition = new Vector3(0, 0, _length / 2);
            UpdateTiling();
        }

        protected void Awake()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();

            UpdateTiling();
        }

        private void UpdateTiling()
        {
            meshRenderer.material.mainTextureScale = new Vector2(1, transform.localScale.y * baseScaleY);
        }

        private MeshRenderer meshRenderer;
        private float _length = -1;

    }

}