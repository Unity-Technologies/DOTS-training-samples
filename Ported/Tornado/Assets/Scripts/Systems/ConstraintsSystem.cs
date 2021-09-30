using System;
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
	private ComponentTypeHandle<Beam> m_BeamComponentTypeHandle;
	private ComponentTypeHandle<URPMaterialPropertyBaseColor> m_ColorComponentTypeHandle;
	private DynamicBuffer<Cache> m_CacheBuffer;
	private DynamicBuffer<NativeArray<Entity>> m_WorldEntititesCache;
	private DynamicBuffer<NativeArray<ArchetypeChunk>> m_ArchetypeChuncksCache;
	private DynamicBuffer<DynamicBuffer<CurrentPoint>> m_CurrentPointsCache;
	private DynamicBuffer<DynamicBuffer<PreviousPoint>> m_PreviousPointsCache;
	private DynamicBuffer<DynamicBuffer<AnchorPoint>> m_AnchorsCache;
	private DynamicBuffer<DynamicBuffer<NeighborCount>> m_NeighborCountsCache;

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


	protected override void OnCreate()
	{
		
			m_BeamComponentTypeHandle = EntityManager.GetComponentTypeHandle<Beam>(true);
			m_ColorComponentTypeHandle = EntityManager.GetComponentTypeHandle<URPMaterialPropertyBaseColor>(true);
			
			
	}


	[BurstCompile]
	private struct ConstraintsSystemJob : IJobParallelFor
	{
		[ReadOnly] 
		public NativeArray<int> beginIndex;
		[ReadOnly] 
		public NativeArray<int> endIndex;
		
		[ReadOnly] 
		public PhysicalConstants InputConstants;

		//[ReadOnly]
		//public NativeArray<Beam> InputBeams;

		[ReadOnly]
		[NativeDisableContainerSafetyRestriction]
		public DynamicBuffer<CurrentPoint> InputCurrentPoints;

		[ReadOnly] 
		[NativeDisableContainerSafetyRestriction]
		public DynamicBuffer<PreviousPoint> InputPreviousPoints;

		[ReadOnly] 
		[NativeDisableContainerSafetyRestriction]

		public DynamicBuffer<NeighborCount> InputNeighborCounts;
		
		[ReadOnly]
		[NativeDisableContainerSafetyRestriction]
		public DynamicBuffer<AnchorPoint> InputAnchors;
		
		//[WriteOnly]
		//[NativeDisableContainerSafetyRestriction]
		//public NativeArray<Beam> OutputBeams;
		
		/*[WriteOnly]
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<URPMaterialPropertyBaseColor> OutputColors;*/
		
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

		
		public NativeArray<ArchetypeChunk> BeamEntityArchetypeChunks;

		//[ReadOnly] 
		[NativeDisableContainerSafetyRestriction]
		public ComponentTypeHandle<Beam> BeamComponentTypeHandle;

		//[ReadOnly] 
		[NativeDisableContainerSafetyRestriction]
		public ComponentTypeHandle<URPMaterialPropertyBaseColor> ColorComponentTypeHandle;

		
		public void Execute(int towerIndex)
		{
			for (var j = 0; j < BeamEntityArchetypeChunks.Length; j++)
			{
				var chunck = BeamEntityArchetypeChunks[j];
				var BeamNativeChunk = chunck[index].GetNativeArray(BeamComponentTypeHandle);
				var ColorNativeChunk = chunck[index].GetNativeArray(ColorComponentTypeHandle);

				for (var i = 0; i < chunck[index].Count; i++)
				{

					var beam = BeamNativeChunk[i];
					float3 point1 = InputCurrentPoints[index][beam.pointAIndex].Value;
					float3 point2 = InputCurrentPoints[index][beam.pointBIndex].Value;

					float3 delta = point2 - point1;
					float dist = math.length(delta);
					float extraDist = dist - beam.size;

					float3 push = delta / dist * extraDist * .5f;

					if (InputAnchors[index][beam.pointAIndex].Value == false
					    && InputAnchors[index][beam.pointBIndex].Value == false)
					{
						point1 += push;
						point2 -= push;
					}
					else if (InputAnchors[index][beam.pointAIndex].Value)
					{
						point2 -= push * 2f;
					}
					else if (InputAnchors[index][beam.pointBIndex].Value)
					{
						point1 += push * 2f;
					}

					var output = OutputCurrentPoints[index];

					output[beam.pointAIndex] = new CurrentPoint() { Value = point1 };
					output[beam.pointBIndex] = new CurrentPoint() { Value = point2 };

					if (math.abs(extraDist) > InputConstants.breakingDistance)
					{
						var pointANeighbourCount = InputNeighborCounts[index][beam.pointAIndex].Value;
						var pointBNeighbourCount = InputNeighborCounts[index][beam.pointBIndex].Value;

						var color = ColorNativeChunk[i];
						color.Value = new float4(0.6f, 0.3f, 0.2f, 1f);
						ColorNativeChunk[i] = color;		
						
						var neighbor = OutputNeighborCounts[index];

						if (pointBNeighbourCount > 1)
						{
							neighbor[beam.pointBIndex] = new NeighborCount()
								{ Value = pointBNeighbourCount - 1 };

							beam.pointBIndex = AddNewPointNativeArray(
								InputCurrentPoints[index],
								InputPreviousPoints[index],
								InputAnchors[index],

								OutputCurrentPoints[index],
								OutputPreviousPoints[index],
								OutputAnchors[index],
								OutputNeighborCounts[index],

								beam.pointBIndex);
							BeamNativeChunk[i] = beam;
						} 
						else if (pointANeighbourCount > 1)
						{
							neighbor[beam.pointAIndex] = new NeighborCount()
								{ Value = pointANeighbourCount - 1 };

							beam.pointAIndex = AddNewPointNativeArray(
								InputCurrentPoints[index],
								InputPreviousPoints[index],
								InputAnchors[index],

								OutputCurrentPoints[index],
								OutputPreviousPoints[index],
								OutputAnchors[index],
								OutputNeighborCounts[index],

								beam.pointAIndex);
							BeamNativeChunk[i] = beam;
						}
					}
				}
			}
		}
	}


	[Serializable]
	struct Cache : IBufferElementData
	{
		public NativeArray<Entity> worldEntititesCache;
		public NativeArray<ArchetypeChunk> archetypeChuncksCache;
		public DynamicBuffer<CurrentPoint> currentPointsCache;
		public DynamicBuffer<PreviousPoint> previousPointsCache;
		public DynamicBuffer<AnchorPoint> anchorsCache;
		public DynamicBuffer<NeighborCount> neighborCountsCache;
	}
	

	protected override void OnUpdate()
	{
		var constants = GetSingleton<PhysicalConstants>();
		var constantsEntity = GetSingletonEntity<PhysicalConstants>();
		
		var beamBatches = new List<BeamBatch>();
		EntityManager.GetAllUniqueSharedComponentData(beamBatches);
		/*for (int i = 0; i < beamBatches.Count; i++)
		{
			Debug.Log("beamBatch["+ i +"]: " + beamBatches[i].Value);
		}*/
		//Assert.AreEqual(1, beamBatches.Count);
		var ecb = new EntityCommandBuffer(Allocator.Temp);  



		var worldQuery = GetEntityQuery(typeof(World), typeof(BeamBatch));
		var beamQuery = GetEntityQuery(typeof(Beam), typeof(URPMaterialPropertyBaseColor), typeof(BeamBatch));

		var allHandle = new JobHandle();
		var aggregatedHandle = new JobHandle();

		
		if(!m_CacheBuffer.IsCreated && beamBatches.Count > 0)
		{
			m_CacheBuffer = ecb.AddBuffer<Cache>(constantsEntity);
			for (var i = 0; i < beamBatches.Count; i++)
			{
				worldQuery.SetSharedComponentFilter(beamBatches[i]);
				beamQuery.SetSharedComponentFilter(beamBatches[i]);
				var worldEntity = m_WorldEntititesCache[i][0];


				m_CacheBuffer.Add(new Cache()
				{
					worldEntititesCache = worldQuery.ToEntityArray(Allocator.Persistent),
					archetypeChuncksCache = beamQuery.CreateArchetypeChunkArray(Allocator.Persistent),
					currentPointsCache = GetBuffer<CurrentPoint>(worldEntity),
					previousPointsCache = GetBuffer<PreviousPoint>(worldEntity),
					anchorsCache = GetBuffer<AnchorPoint>(worldEntity),
					neighborCountsCache = GetBuffer<NeighborCount>(worldEntity)
				});

			}
		}
		

		//for (var i = 0; i < beamBatches.Count; i++) {
			
			//beamQuery.SetSharedComponentFilter(beamBatches[i]);
			//worldQuery.SetSharedComponentFilter(beamBatches[i]);

			//var worldEntities = worldQuery.ToEntityArray(Allocator.TempJob);
			//Assert.AreEqual(1, worldEntities.Length);
			//m_WorldEntititesCache[i] = worldQuery.ToEntityArray(Allocator.Temp);
			var worldEntity = m_WorldEntititesCache[i][0];
			
			var currentPoints = GetBuffer<CurrentPoint>(worldEntity);
			var previousPoints = GetBuffer<PreviousPoint>(worldEntity);
			var anchors = GetBuffer<AnchorPoint>(worldEntity);
			var neighborCounts = GetBuffer<NeighborCount>(worldEntity);

			
			//var entities = beamQuery.ToEntityArray(Allocator.TempJob);
			
			//var beamComponents = beamQuery.ToComponentDataArray<Beam>(Allocator.TempJob);
			//var colors = beamQuery.ToComponentDataArray<URPMaterialPropertyBaseColor>(Allocator.TempJob);
			
			//var archetypeChuncks = beamQuery.CreateArchetypeChunkArray(Allocator.TempJob);


			m_BeamComponentTypeHandle.Update(this);
			m_ColorComponentTypeHandle.Update(this);
			//m_BeamComponentTypeHandle = GetComponentTypeHandle<Beam>();
			//m_ColorComponentTypeHandle = GetComponentTypeHandle<URPMaterialPropertyBaseColor>();


			//dispatch job
			var job = new ConstraintsSystemJob
			{
				InputConstants = constants,
				//InputBeams = beamComponents,
				InputCurrentPoints = currentPoints,
				InputPreviousPoints = previousPoints,
				InputNeighborCounts = neighborCounts,
				InputAnchors = anchors,
				//OutputBeams = beamComponents,
				//OutputColors = colors,
				OutputCurrentPoints = currentPoints,
				OutputPreviousPoints = previousPoints,
				OutputNeighborCounts = neighborCounts,
				OutputAnchors = anchors,
				BeamComponentTypeHandle = m_BeamComponentTypeHandle,
				BeamEntityArchetypeChunks = m_ArchetypeChuncksCache,
				ColorComponentTypeHandle = m_ColorComponentTypeHandle,
					
			};
			//TODO: this should run each job in parallel
			var handle = job.Schedule(beamBatches.Count, 10 ,allHandle);
			//handle.Complete();
			
			//beamQuery.CopyFromComponentDataArray(job.OutputBeams);
			//beamQuery.CopyFromComponentDataArray(job.OutputColors);
			
			aggregatedHandle = JobHandle.CombineDependencies(handle, aggregatedHandle);

			


			//TODO: do we need to dispose the arrays?
			
		//}
		aggregatedHandle.Complete();
		//Dependency = JobHandle.CombineDependencies(Dependency, allHandle);
		
		Dependency.Complete();

	}
}
