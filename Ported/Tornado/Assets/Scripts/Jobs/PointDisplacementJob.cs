using Components;
using Unity.Collections;
using Unity.Jobs;

namespace Assets.Scripts.Jobs
{
    public struct PointDisplacementJob : IJobParallelFor
    {
        public NativeArray<VerletPoints> points;       

        public float invDamping;   

        public void Execute(int i)
        {
            VerletPoints point = points[i];

            float startX = point.currentPosition.x;
            float startY = point.currentPosition.y;
            float startZ = point.currentPosition.z;

            //gravity 
            point.oldPosition.y += .01f;

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
}
