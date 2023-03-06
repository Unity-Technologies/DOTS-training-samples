using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighwayRacers
{

    public class HighwayPiece : MonoBehaviour
    {

        public float startX
        {
            get { return transform.localPosition.x; }
        }
        public float startZ
        {
            get { return transform.localPosition.z; }
        }
        public void SetStartPosition(Vector3 position)
        {
            transform.localPosition = position;
        }
        /// <summary>
        /// In radians.
        /// </summary>
        public float startRotation
        {
            get
            {
                return _startRotation;
            }
            set
            {
                _startRotation = value;
                transform.localRotation = Quaternion.Euler(0, _startRotation * Mathf.Rad2Deg, 0);
            }
        }
        private float _startRotation = 0;

        public Quaternion startRotationQ
        {
            get
            {
                return Quaternion.Euler(0, startRotation * Mathf.Rad2Deg, 0);
            }
        }

        public virtual float length(float lane)
        {
            return 0;
        }
        
    }

}