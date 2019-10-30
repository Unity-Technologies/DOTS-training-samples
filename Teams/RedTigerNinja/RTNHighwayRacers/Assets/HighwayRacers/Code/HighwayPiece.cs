using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighwayRacers
{

    public class HighwayPiece : MonoBehaviour
    {
        private Unity.Entities.Entity PieceEntity;

        private void Start()
        {
            var em = Unity.Entities.World.Active.EntityManager;
            PieceEntity = em.CreateEntity();
            em.AddComponentData(this.PieceEntity, this.AsHighwayState);
        }

        public struct HighwayPieceState : Unity.Entities.IComponentData
        {
            public Vector4 LaneLengths;
            public float startRotation, startX, startZ;

            public float length(float li)
            {
                int ndx = (int)li;
                if ((ndx < 0)) ndx = 0;
                if (ndx > 3) ndx = 3;
                return this.LaneLengths[ndx];
            }
        }

        public virtual HighwayPieceState AsHighwayState
        {
            get 
            {
                HighwayPieceState ans = new HighwayPieceState();
                ans.startRotation = this.startRotation;
                ans.startX = this.startX;
                ans.startZ = this.startZ;
                for (var li=0; li<Highway.NUM_LANES; li++)
                {
                    ans.LaneLengths[li] = this.length((float)li);
                }
                return ans;
            }
        }


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