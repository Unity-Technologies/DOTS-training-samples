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
	private NativeArray<Entity>[] m_WorldEntititesCache = null;
	private NativeArray<ArchetypeChunk>[] m_ArchetypeChuncksCache;
	private DynamicBuffer<CurrentPoint>[] m_CurrentPointsCache;
	private DynamicBuffer<PreviousPoint>[] m_PreviousPointsCache;
	private DynamicBuffer<AnchorPoint>[] m_AnchorsCache;
	private DynamicBuffer<NeighborCount>[] m_NeighborCountsCache;
	private PhysicalConstants m_Constants;
	private List<BeamBatch> m_BeamBatches;

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
	private struct ConstraintsSystemJob : IJob
	{
		
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



		public void Execute()
		{
			for (var j = 0; j < BeamEntityArchetypeChunks.Length; j++)
			{
				var chunck = BeamEntityArchetypeChunks[j];
				var BeamNativeChunk = chunck.GetNativeArray(BeamComponentTypeHandle);
				var ColorNativeChunk = chunck.GetNativeArray(ColorComponentTypeHandle);

				for (var i = 0; i < chunck.Count; i++)
				{

					var beam = BeamNativeChunk[i];
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

						var color = ColorNativeChunk[i];
						color.Value = new float4(0.6f, 0.3f, 0.2f, 1f);
						ColorNativeChunk[i] = color;

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
							BeamNativeChunk[i] = beam;
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
							BeamNativeChunk[i] = beam;
						}
					}
				}
			}
		}
	}
	
	
	protected override void OnCreate()
	{
		
		m_BeamComponentTypeHandle = EntityManager.GetComponentTypeHandle<Beam>(true);
		m_ColorComponentTypeHandle = EntityManager.GetComponentTypeHandle<URPMaterialPropertyBaseColor>(true);

		m_BeamBatches = new List<BeamBatch>();
	}
	
	protected override void OnUpdate()
	{
		
		m_Constants = GetSingleton<PhysicalConstants>();
		

		if (m_BeamBatches.Count == 0)
		{
			EntityManager.GetAllUniqueSharedComponentData(m_BeamBatches);
		}
		
		/*for (int i = 0; i < beamBatches.Count; i++)
		{
			Debug.Log("beamBatch["+ i +"]: " + beamBatches[i].Value);
		}*/
		//Assert.AreEqual(1, beamBatches.Count);



		var worldQuery = GetEntityQuery(typeof(World), typeof(BeamBatch));
		var beamQuery = GetEntityQuery(typeof(Beam), typeof(URPMaterialPropertyBaseColor), typeof(BeamBatch));

		var allHandle = new JobHandle();
		var aggregatedHandle = new JobHandle();

		
		if(m_WorldEntititesCache == null && m_BeamBatches.Count > 0) {
			m_WorldEntititesCache = new NativeArray<Entity>[m_BeamBatches.Count];
			m_ArchetypeChuncksCache = new NativeArray<ArchetypeChunk>[m_BeamBatches.Count];
			m_CurrentPointsCache = new DynamicBuffer<CurrentPoint>[m_BeamBatches.Count];
			m_PreviousPointsCache = new DynamicBuffer<PreviousPoint>[m_BeamBatches.Count];
			m_AnchorsCache = new DynamicBuffer<AnchorPoint>[m_BeamBatches.Count];
			m_NeighborCountsCache = new DynamicBuffer<NeighborCount>[m_BeamBatches.Count];
			for (var i = 0; i < m_BeamBatches.Count; i++)
			{
				worldQuery.SetSharedComponentFilter(m_BeamBatches[i]);
				beamQuery.SetSharedComponentFilter(m_BeamBatches[i]);

				m_WorldEntititesCache[i] = worldQuery.ToEntityArray(Allocator.Persistent);
				m_ArchetypeChuncksCache[i] = beamQuery.CreateArchetypeChunkArray(Allocator.Persistent);
				
				var worldEntity = m_WorldEntititesCache[i][0];
				m_CurrentPointsCache[i] = GetBuffer<CurrentPoint>(worldEntity);
				m_PreviousPointsCache[i] = GetBuffer<PreviousPoint>(worldEntity);
				m_AnchorsCache[i] = GetBuffer<AnchorPoint>(worldEntity);
				m_NeighborCountsCache[i] = GetBuffer<NeighborCount>(worldEntity);

			}
		}
		
		
		m_BeamComponentTypeHandle.Update(this);
		m_ColorComponentTypeHandle.Update(this);
		//m_BeamComponentTypeHandle = GetComponentTypeHandle<Beam>();
		//m_ColorComponentTypeHandle = GetComponentTypeHandle<URPMaterialPropertyBaseColor>();

		for (var i = 0; i < m_BeamBatches.Count; i++) {
			
			beamQuery.SetSharedComponentFilter(m_BeamBatches[i]);
			//worldQuery.SetSharedComponentFilter(beamBatches[i]);

			//var worldEntities = worldQuery.ToEntityArray(Allocator.TempJob);
			//Assert.AreEqual(1, worldEntities.Length);
			//m_WorldEntititesCache[i] = worldQuery.ToEntityArray(Allocator.Temp);
			/*var worldEntity = m_WorldEntititesCache[i][0];
			
			var currentPoints = GetBuffer<CurrentPoint>(worldEntity);
			var previousPoints = GetBuffer<PreviousPoint>(worldEntity);
			var anchors = GetBuffer<AnchorPoint>(worldEntity);
			var neighborCounts = GetBuffer<NeighborCount>(worldEntity);*/

			
			//var entities = beamQuery.ToEntityArray(Allocator.TempJob);
			
			//var beamComponents = beamQuery.ToComponentDataArray<Beam>(Allocator.TempJob);
			//var colors = beamQuery.ToComponentDataArray<URPMaterialPropertyBaseColor>(Allocator.TempJob);
			
			//var archetypeChuncks = beamQuery.CreateArchetypeChunkArray(Allocator.TempJob);

			//dispatch job
			var job = new ConstraintsSystemJob
			{
				InputConstants = m_Constants,
				//InputBeams = beamComponents,
				InputCurrentPoints = m_CurrentPointsCache[i],
				InputPreviousPoints = m_PreviousPointsCache[i],
				InputNeighborCounts = m_NeighborCountsCache[i],
				InputAnchors = m_AnchorsCache[i],
				//OutputBeams = beamComponents,
				//OutputColors = colors,
				OutputCurrentPoints = m_CurrentPointsCache[i],
				OutputPreviousPoints = m_PreviousPointsCache[i],
				OutputNeighborCounts = m_NeighborCountsCache[i],
				OutputAnchors = m_AnchorsCache[i],
				BeamComponentTypeHandle = m_BeamComponentTypeHandle,
				BeamEntityArchetypeChunks = m_ArchetypeChuncksCache[i],
				ColorComponentTypeHandle = m_ColorComponentTypeHandle,
					
			};
			//TODO: this should run each job in parallel
			var handle = job.Schedule(allHandle);
			//handle.Complete();
			
			//beamQuery.CopyFromComponentDataArray(job.OutputBeams);
			//beamQuery.CopyFromComponentDataArray(job.OutputColors);
			
			aggregatedHandle = JobHandle.CombineDependencies(handle, aggregatedHandle);

			


			//TODO: do we need to dispose the arrays?
			
		}
		aggregatedHandle.Complete();
		//Dependency = JobHandle.CombineDependencies(Dependency, allHandle);
		
		Dependency.Complete();

	}
}
