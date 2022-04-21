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
		public NativeArray<VerletPoint> points;
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

					VerletPoint point1 = points[link.point1Index];
					VerletPoint point2 = points[link.point2Index];

					float3 d = point2.currentPosition - point1.currentPosition;

					float dist = math.sqrt(d.x * d.x + d.y * d.y + d.z * d.z);
					float extraDist = dist - link.length;

					float3 push = (d / dist * extraDist) * .5f;

					if (point1.anchored == 0 && point2.anchored == 0)
					{
						point1.currentPosition += push;
						point2.currentPosition -= push;
					}
					else if (point1.anchored > 0)
					{
						point2.currentPosition -= push * 2f;
					}
					else if (point2.anchored > 0)
					{
						point1.currentPosition += push * 2f;
					}

					int originalPoint1Index = link.point1Index;
					int originalPoint2Index = link.point2Index;

					
					if (math.abs(extraDist) > physicSettings.breakResistance)
					{
						if (point2.neighborCount > 1)
						{
							point2.neighborCount--;
							var newPoint = new VerletPoint(point2);
							newPoint.neighborCount = 1;
							newPoint.materialID = link.materialID;

							var c = pointAllocators[islandIndex];					

							points[c] = newPoint;
							link.point2Index = c;
							pointAllocators[islandIndex] = ++c;
						}
						else if (point1.neighborCount > 1)
						{
							point1.neighborCount--;
							var newPoint = new VerletPoint(point1);
							newPoint.neighborCount = 1;
							newPoint.materialID = link.materialID;

							var c = pointAllocators[islandIndex];					

							points[c] = newPoint;
							link.point1Index = c;
							pointAllocators[islandIndex] = ++c;
						}
					}
										
					//link.dirtyRotation = (dx / dist * link.direction.x + dy / dist * link.direction.y + dz / dist * link.direction.z < .999f) ? byte.MaxValue : (byte)0;
					link.direction = d/dist;
			

					links[i] = link;
					points[originalPoint1Index] = point1;
					points[originalPoint2Index] = point2;
				}
			}
        }
	}
}
