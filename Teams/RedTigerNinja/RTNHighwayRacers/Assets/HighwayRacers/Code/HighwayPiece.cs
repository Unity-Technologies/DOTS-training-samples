using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighwayRacers
{

    public class HighwayPiece : MonoBehaviour
    {
        private Unity.Entities.Entity PieceEntity;
        public HighwayPiece PieceBefore;

        private void Start()
        {
            var em = Unity.Entities.World.Active.EntityManager;
            PieceEntity = em.CreateEntity();
            em.AddComponentData(this.PieceEntity, this.AsHighwayState);
        }

        public struct HighwayPieceState : Unity.Entities.IComponentData
        {
            //public Vector4 LaneLengths;
            public float startRotation, startX, startZ;

            public float lane0start, lane0length;
            public float lane4start, lane4length;

            public float lane0end { get { return this.lane0start + this.lane0length; } }
            public float lane4end { get { return this.lane4start + this.lane4length; } }

            public float startOfLane(float li)
            {
                float ul = (li / 4.0f);
                return Mathf.Lerp(lane0start, lane4start, ul);
            }

            public float lengthOfLane(float li)
            {
                float ul = (li / 4.0f);
                return Mathf.Lerp(lane0length, lane4length, ul);
            }
        }

        public void SetPiecePrevious(HighwayPiece other)
        {
            this.PieceBefore = other;
        }

        public virtual HighwayPieceState AsHighwayState
        {
            get 
            {
                HighwayPieceState ans = new HighwayPieceState();
                ans.startRotation = this.startRotation;
                ans.startX = this.startX;
                ans.startZ = this.startZ;
                ans.lane0length = this.length(0);
                ans.lane4length = this.length(4);
                if (this.PieceBefore)
                {
                    ans.lane0start = this.PieceBefore.AsHighwayState.lane0end;
                    ans.lane4start = this.PieceBefore.AsHighwayState.lane4end;
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