using System.Collections.Generic;
using Assets.Scripts.Components;
using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

[UpdateAfter(typeof(PointSimulationSystem))]
public partial class ConstraintsSystem : SystemBase
{
	private static int AddNewPoint(
		DynamicBuffer<CurrentPoint> inputCurrentPoints,
		DynamicBuffer<PreviousPoint> inputPreviousPoints,
		DynamicBuffer<AnchorPoint> inputAnchors,
		DynamicBuffer<NeighborCount> inputNeighborBuffer,
		int indexTocopyFrom)
    {
		inputCurrentPoints.Add(new CurrentPoint() { Value = inputCurrentPoints[indexTocopyFrom].Value });
		inputPreviousPoints.Add(new PreviousPoint() { Value = inputPreviousPoints[indexTocopyFrom].Value });
		inputAnchors.Add(new AnchorPoint() { Value = inputAnchors[indexTocopyFrom].Value });
		inputNeighborBuffer.Add(new NeighborCount() { Value = 1 });
		return inputNeighborBuffer.Length - 1;
    }
	
	private static int AddNewPointNativeArray(
		DynamicBuffer<CurrentPoint> inputCurrentPoints,
		DynamicBuffer<PreviousPoint> inputPreviousPoints,
		DynamicBuffer<AnchorPoint> inputAnchors,
		
		DynamicBuffer<CurrentPoint> outputCurrentPoints,
		DynamicBuffer<PreviousPoint> outputPreviousPoints,
		DynamicBuffer<AnchorPoint> outputAnchors,
		DynamicBuffer<NeighborCount> outputNeighborBuffer,
		int indexToCopyFrom)
	{
		outputCurrentPoints.Add(new CurrentPoint() { Value = inputCurrentPoints[indexToCopyFrom].Value });
		outputPreviousPoints.Add(new PreviousPoint() { Value = inputPreviousPoints[indexToCopyFrom].Value });
		outputAnchors.Add(new AnchorPoint() { Value = inputAnchors[indexToCopyFrom].Value });
		outputNeighborBuffer.Add(new NeighborCount() { Value = 1 });
		return outputNeighborBuffer.Length-1;
	}

	[BurstCompile]
	private struct PointSimulationSystemJob : IJobParallelFor
	{
		public void Execute(int index)
		{
			throw new System.NotImplementedException();
		}
	}

	

	[BurstCompile]
	private struct ConstraintsSystemJob : IJob
	{
		
		[ReadOnly] 
		public PhysicalConstants InputConstants;

		[ReadOnly]
		public NativeArray<Beam> InputBeams;

		[ReadOnly]
		public DynamicBuffer<CurrentPoint> InputCurrentPoints;

		[ReadOnly] 
		public DynamicBuffer<PreviousPoint> InputPreviousPoints;

		[ReadOnly] 
		public DynamicBuffer<NeighborCount> InputNeighborCounts;
		
		[ReadOnly]
		public DynamicBuffer<AnchorPoint> InputAnchors;
		
		[WriteOnly]
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<Beam> OutputBeams;
		
		[WriteOnly]
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<URPMaterialPropertyBaseColor> OutputColors;
		
		[WriteOnly]
		[NativeDisableContainerSafetyRestriction]
		public DynamicBuffer<CurrentPoint> OutputCurrentPoints;
		
		[WriteOnly]
		[NativeDisableContainerSafetyRestriction]
		public DynamicBuffer<PreviousPoint> OutputPreviousPoints;
		
		[WriteOnly] 
		[NativeDisableContainerSafetyRestriction]
		public DynamicBuffer<NeighborCount> OutputNeighborCounts;
		
		[WriteOnly]
		[NativeDisableContainerSafetyRestriction]
		public DynamicBuffer<AnchorPoint> OutputAnchors;



		public void Execute()
		{
			for (var i = 0; i < InputBeams.Length; i++)
			{
				var beam = InputBeams[i];
				float3 point1 = InputCurrentPoints[beam.pointAIndex].Value;
				float3 point2 = InputCurrentPoints[beam.pointBIndex].Value;

				float3 delta = point2 - point1;
				float dist = math.length(delta);
				float extraDist = dist - beam.size;

				float3 push = delta / dist * extraDist * .5f;

				if (InputAnchors[beam.pointAIndex].Value == false
				    && InputAnchors[beam.pointBIndex].Value == false)
				{
					point1 += push;
					point2 -= push;
				}
				else if (InputAnchors[beam.pointAIndex].Value)
				{
					point2 -= push * 2f;
				}
				else if (InputAnchors[beam.pointBIndex].Value)
				{
					point1 += push * 2f;
				}

				OutputCurrentPoints[beam.pointAIndex] = new CurrentPoint() { Value = point1 };
				OutputCurrentPoints[beam.pointBIndex] = new CurrentPoint() { Value = point2 };
				
				if (math.abs(extraDist) > InputConstants.breakingDistance)
				{
					var pointANeighbourCount = InputNeighborCounts[beam.pointAIndex].Value;
					var pointBNeighbourCount = InputNeighborCounts[beam.pointBIndex].Value;
					
					OutputColors[i] = new URPMaterialPropertyBaseColor() { Value = new float4(0.6f, 0.3f, 0.2f, 1f) };

					if (pointBNeighbourCount > 1)
					{
						OutputNeighborCounts[beam.pointBIndex] = new NeighborCount()
							{ Value = pointBNeighbourCount - 1 };

						beam.pointBIndex = AddNewPointNativeArray(
							InputCurrentPoints,
							InputPreviousPoints,
							InputAnchors,
							
							OutputCurrentPoints,
							OutputPreviousPoints,
							OutputAnchors,
							OutputNeighborCounts,
							
							beam.pointBIndex);
						OutputBeams[i] = beam;
					}
					else if (pointANeighbourCount > 1)
					{
						OutputNeighborCounts[beam.pointAIndex] = new NeighborCount()
							{ Value = pointANeighbourCount - 1 };

						beam.pointAIndex = AddNewPointNativeArray(
							InputCurrentPoints,
							InputPreviousPoints,
							InputAnchors,
							
							OutputCurrentPoints,
							OutputPreviousPoints,
							OutputAnchors,
							OutputNeighborCounts,
							
							beam.pointAIndex);
						OutputBeams[i] = beam;
					}
				}
			}
		}
	}

	protected override void OnUpdate()
	{
		var constants = GetSingleton<PhysicalConstants>();
		
		var beamBatches = new List<BeamBatch>();
		EntityManager.GetAllUniqueSharedComponentData(beamBatches);
		for (int i = 0; i < beamBatches.Count; i++)
		{
			Debug.Log("beamBatch["+ i +"]: " + beamBatches[i].Value);
		}
		Assert.AreEqual(1, beamBatches.Count);



		var worldQuery = GetEntityQuery(typeof(World), typeof(BeamBatch));
		var beamQuery = GetEntityQuery(typeof(Beam), typeof(URPMaterialPropertyBaseColor), typeof(BeamBatch));
		
		
		

		for (var i = 0; i < beamBatches.Count; i++) {
			
			//beamQuery.SetSharedComponentFilter(beamBatches[i]);
			//worldQuery.SetSharedComponentFilter(beamBatches[i]);
			
			var worldEntities = worldQuery.ToEntityArray(Allocator.TempJob);
			Assert.AreEqual(1, worldEntities.Length);
			var worldEntity = worldEntities[0];
			
			var currentPoints = GetBuffer<CurrentPoint>(worldEntity);
			var previousPoints = GetBuffer<PreviousPoint>(worldEntity);
			var anchors = GetBuffer<AnchorPoint>(worldEntity);
			var neighborCounts = GetBuffer<NeighborCount>(worldEntity);

			
			//var entities = beamQuery.ToEntityArray(Allocator.TempJob);
			
			var beamComponents = beamQuery.ToComponentDataArray<Beam>(Allocator.TempJob);
			var colors = beamQuery.ToComponentDataArray<URPMaterialPropertyBaseColor>(Allocator.TempJob);

			//dispatch job
			var job = new ConstraintsSystemJob
			{
				InputConstants = constants,
				InputBeams = beamComponents,
				InputCurrentPoints = currentPoints,
				InputPreviousPoints = previousPoints,
				InputNeighborCounts = neighborCounts,
				InputAnchors = anchors,
				OutputBeams = beamComponents,
				OutputColors = colors,
				OutputCurrentPoints = currentPoints,
				OutputPreviousPoints = previousPoints,
				OutputNeighborCounts = neighborCounts,
				OutputAnchors = anchors,
			};
			//TODO: this should run each job in parallel
			var handle = job.Schedule();
			handle.Complete();
			
			beamQuery.CopyFromComponentDataArray(job.OutputBeams);
			beamQuery.CopyFromComponentDataArray(job.OutputColors);
			
			
			Dependency = JobHandle.CombineDependencies(Dependency, handle);


			//TODO: do we need to dispose the arrays?
			
		}
		
		Dependency.Complete();

	}
}
