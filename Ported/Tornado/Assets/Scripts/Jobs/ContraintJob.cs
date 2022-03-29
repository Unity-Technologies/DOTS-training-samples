using Components;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Assets.Scripts.Jobs
{
    public struct ContraintJob : IJob
    {
		public NativeArray<VerletPoints> points;
		[ReadOnly] public NativeArray<Link> links;


		public void Execute()
        {
            for (int i = 0; i < links.Length; i++)
            {
				Link link = links[i];

				VerletPoints point1 = points[link.startIndex];
				VerletPoints point2 = points[link.endIndex];

				float dx = point2.currentPosition.x - point1.currentPosition.x;
				float dy = point2.currentPosition.y - point1.currentPosition.y;
				float dz = point2.currentPosition.z - point1.currentPosition.z;

				float dist = math.sqrt(dx * dx + dy * dy + dz * dz);
				float extraDist = dist - link.length;

				float pushX = (dx / dist * extraDist) * .5f;
				float pushY = (dy / dist * extraDist) * .5f;
				float pushZ = (dz / dist * extraDist) * .5f;

				point1.currentPosition.x += pushX;
				point1.currentPosition.y += pushY;
				point1.currentPosition.z += pushZ;

				point2.currentPosition.x -= pushX;
				point2.currentPosition.y -= pushY;
				point2.currentPosition.z -= pushZ;

				points[link.startIndex] = point1;
				points[link.endIndex] = point2;
			}
			
		}
        
	}
}
