using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    public partial class PointDisplacementSystem : SystemBase
    {
        public NativeArray<VerletPoints> points;
        public NativeArray<Link> links;        

        public bool isInitialized;

        protected override void OnUpdate()
        {
            if (!isInitialized) return;

            float invDamping = 1f - 0.012f;

            for (int i = 0; i < points.Length; i++)
            {
				VerletPoints point = points[i];
				
				float startX = point.currentPosition.x;
				float startY = point.currentPosition.y;
				float startZ = point.currentPosition.z;

				//gravity ?????
				point.oldPosition.y += .01f;

				// tornado force

				point.currentPosition.x += (point.currentPosition.x - point.oldPosition.x) * invDamping;
				point.currentPosition.y += (point.currentPosition.y - point.oldPosition.y) * invDamping;
				point.currentPosition.z += (point.currentPosition.z - point.oldPosition.z) * invDamping;

				point.oldPosition.x = startX;
				point.oldPosition.y = startY;
				point.oldPosition.z = startZ;

				if (point.currentPosition.y < 0f)
				{
					point.currentPosition.y = 0f;
					point.oldPosition.y = -point.oldPosition.y;

                    point.oldPosition.x += (point.currentPosition.x - point.oldPosition.x) * 0.4f;
                    point.oldPosition.z += (point.currentPosition.z - point.oldPosition.z) * 0.4f;
                }

                points[i] = point;
			}
            

        }

       

        public void Initialize(NativeArray<VerletPoints> points, NativeArray<Link> links)
        {
            this.links = links;
            this.points = points;
            isInitialized = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(points != null) points.Dispose();
            if(links != null) links.Dispose();
        }
    }
}