using Components;
using System.Threading;
using Systems;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Assets.Scripts.Jobs
{
    public struct ContraintJob : IJob
    {
		public NativeArray<VerletPoints> points;
		public NativeArray<Link> links;

		public int iterations;
		

		[ReadOnly] public PhysicsSettings physicSettings;

		public void Execute()
        {
            for (int r = 0; r < iterations; r++)
            {
				for (int i = 0; i < links.Length; i++)
				{
					Link link = links[i];

					VerletPoints point1 = points[link.point1Index];
					VerletPoints point2 = points[link.point2Index];

					float dx = point2.currentPosition.x - point1.currentPosition.x;
					float dy = point2.currentPosition.y - point1.currentPosition.y;
					float dz = point2.currentPosition.z - point1.currentPosition.z;

					float dist = math.sqrt(dx * dx + dy * dy + dz * dz);
					float extraDist = dist - link.length;

					float pushX = (dx / dist * extraDist) * .5f;
					float pushY = (dy / dist * extraDist) * .5f;
					float pushZ = (dz / dist * extraDist) * .5f;

					if (point1.anchored == 0 && point2.anchored == 0)
					{
						point1.currentPosition.x += pushX;
						point1.currentPosition.y += pushY;
						point1.currentPosition.z += pushZ;

						point2.currentPosition.x -= pushX;
						point2.currentPosition.y -= pushY;
						point2.currentPosition.z -= pushZ;
					}
					else if (point1.anchored > 0)
					{
						point2.currentPosition.x -= pushX * 2f;
						point2.currentPosition.y -= pushY * 2f;
						point2.currentPosition.z -= pushZ * 2f;
					}
					else if (point1.anchored > 0)
					{
						point1.currentPosition.x += pushX * 2f;
						point1.currentPosition.y += pushY * 2f;
						point1.currentPosition.z += pushZ * 2f;
					}

					int originalPoint1Index = link.point1Index;
					int originalPoint2Index = link.point2Index;

					if (math.abs(extraDist) > physicSettings.breakResistance)
					{
						if (point2.neighborCount > 1)
						{
							point2.neighborCount--;
							var newPoint = new VerletPoints(point2);							
							newPoint.neighborCount = 1;
							var allocatedIndex = Interlocked.Increment(ref PointDisplacementSystem.AllocatedPointCount) -1;
							points[allocatedIndex] = newPoint;
							link.point2Index = allocatedIndex;
						}
						else if (point1.neighborCount > 1)
						{
							point1.neighborCount--;
							var newPoint = new VerletPoints(point1);
							newPoint.neighborCount = 1;
							var allocatedIndex = Interlocked.Increment(ref PointDisplacementSystem.AllocatedPointCount) - 1;
							points[allocatedIndex] = newPoint;
							link.point1Index = allocatedIndex;
						}
					}


					links[i] = link;
					points[originalPoint1Index] = point1;
					points[originalPoint2Index] = point2;
				}
			}
            
			
		}
        
	}
}
