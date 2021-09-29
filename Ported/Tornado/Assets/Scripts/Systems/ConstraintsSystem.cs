using Assets.Scripts.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(PointSimulationSystem))]
public partial class ConstraintsSystem : SystemBase
{
	private static int AddNewPoint(
		DynamicBuffer<CurrentPoint> currentPoints,
		DynamicBuffer<PreviousPoint> previousPoints,
		DynamicBuffer<AnchorPoint> anchors,
		DynamicBuffer<NeighborCount> neighborBuffer,
		int indexTocopyFrom)
    {
		currentPoints.Add(new CurrentPoint() { Value = currentPoints[indexTocopyFrom].Value });
		previousPoints.Add(new PreviousPoint() { Value = previousPoints[indexTocopyFrom].Value });
		anchors.Add(new AnchorPoint() { Value = anchors[indexTocopyFrom].Value });
		return neighborBuffer.Add(new NeighborCount() { Value = 1 }) - 1;
	}

    protected override void OnUpdate()
    {
		var worldEntity = GetSingletonEntity<World>();
	
		var currentPoints = GetBuffer<CurrentPoint>(worldEntity);
		var previousPoints = GetBuffer<PreviousPoint>(worldEntity);
		var anchors = GetBuffer<AnchorPoint>(worldEntity);
		var neighborCounts = GetBuffer<NeighborCount>(worldEntity);

		var constants = GetSingleton<PhysicalConstants>();

		Entities
			.ForEach((Entity beamEntity, ref Beam beam, ref URPMaterialPropertyBaseColor color) => 
			{
				float3 point1 = currentPoints[beam.pointAIndex].Value;
				float3 point2 = currentPoints[beam.pointBIndex].Value;

				float3 delta = point2 - point1;
				float dist = math.length(delta);
				float extraDist = dist - beam.size;

				float3 push = delta / dist * extraDist * .5f;

				if (anchors[beam.pointAIndex].Value == false 
					&& anchors[beam.pointBIndex].Value == false)
				{
					point1 += push;
					point2 -= push;
				}
				else if (anchors[beam.pointAIndex].Value)
				{
					point2 -= push * 2f;
				}
				else if (anchors[beam.pointBIndex].Value)
				{
					point1 += push * 2f;
				}

				currentPoints[beam.pointAIndex] = new CurrentPoint() { Value = point1 };
				currentPoints[beam.pointBIndex] = new CurrentPoint() { Value = point2 };

				if (math.abs(extraDist) > constants.breakingDistance)
				{
					var pointANeighbourCount = neighborCounts[beam.pointAIndex].Value;
					var pointBNeighbourCount = neighborCounts[beam.pointBIndex].Value;

					color.Value = new float4(0.6f, 0.3f, 0.2f, 1f);

					if (pointBNeighbourCount > 1)
					{
						neighborCounts[beam.pointBIndex] = new NeighborCount()
							{ Value = pointBNeighbourCount - 1 };

						beam.pointBIndex = AddNewPoint(
							currentPoints, 
							previousPoints, 
							anchors, 
							neighborCounts, 
							beam.pointBIndex);


					}
					else if (pointANeighbourCount > 1)
					{
						neighborCounts[beam.pointAIndex] = new NeighborCount()
							{ Value = pointANeighbourCount - 1 };

						beam.pointAIndex = AddNewPoint(
							currentPoints,
							previousPoints,
							anchors,
							neighborCounts,
							beam.pointAIndex);

					}
				}


		}).Run();
    }
}
