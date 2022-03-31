using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;


// Acount, Bcount, Ccount


namespace Assets.Scripts.Jobs
{
	[BurstCompile]
    public struct ContraintJob : IJob
    {
	    [NativeDisableContainerSafetyRestriction]
		public NativeArray<VerletPoints> points;
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<Link> links;
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<int> pointAllocators;
		[ReadOnly] public int iterations;
		[ReadOnly] public int islandIndex;
		[ReadOnly] public int islandStartLinkIndex;
		[ReadOnly] public int islandEndLinkIndex;
		[ReadOnly] public PhysicsSettings physicSettings;

		public void Execute()
        {
            for (int r = 0; r < iterations; r++)
            {
				for (int i = islandStartLinkIndex; i < islandEndLinkIndex; i++)
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
							newPoint.materialID = link.materialID;

							var c = pointAllocators[islandIndex];
							var allocatedIndex = c;							
							pointAllocators[islandIndex] = ++c;

							points[allocatedIndex] = newPoint;
							link.point2Index = allocatedIndex;
						}
						else if (point1.neighborCount > 1)
						{
							point1.neighborCount--;
							var newPoint = new VerletPoints(point1);
							newPoint.neighborCount = 1;
							newPoint.materialID = link.materialID;

							var c = pointAllocators[islandIndex];
							var allocatedIndex = c;
							pointAllocators[islandIndex] = ++c;

							points[allocatedIndex] = newPoint;
							link.point1Index = allocatedIndex;
						}
					}
										
					link.dirtyRotation = (dx / dist * link.direction.x + dy / dist * link.direction.y + dz / dist * link.direction.z < .99f) ? byte.MaxValue : (byte)0;
					link.direction.x = dx/dist;
					link.direction.y = dy/dist;
					link.direction.z = dz/dist;

					links[i] = link;
					points[originalPoint1Index] = point1;
					points[originalPoint2Index] = point2;
				}
			}
        }
	}
}
