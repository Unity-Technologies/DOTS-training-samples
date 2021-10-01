using System.Collections.Generic;
using Assets.Scripts.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class PointSimulationSystem : SystemBase
{
	private Random m_Random;
	private List<BeamBatch> m_BeamBatches;
	private NativeArray<Entity>[] m_WorldEntititesCache = null;
	private NativeArray<CurrentPoint>[] m_CurrentPointsCache;
	private NativeArray<PreviousPoint>[] m_PreviousPointsCache;
	private NativeArray<AnchorPoint>[] m_AnchorsCache;
	private NativeArray<NeighborCount>[] m_NeighborCountsCache;

	public static float TornadoSway(float y, float time)
	{
		return math.sin(y / 5f + time / 4f) * 3f;
	}

	[BurstCompile]
	private struct PointSimulationSystemJob : IJobParallelFor
	{
		[ReadOnly]
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<CurrentPoint> InputCurrentPoints;
		[ReadOnly]
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<PreviousPoint> InputPreviousPoints;
		[ReadOnly]
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<AnchorPoint> Anchors;
		[ReadOnly]
		public Tornado tornado;
		[ReadOnly]
		public Random random;
		[ReadOnly]
		public PhysicalConstants constants;
		[ReadOnly]
		public float elapsedTime;

		[WriteOnly]
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<CurrentPoint> OutputCurrentPoints;
		[WriteOnly]
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<PreviousPoint> OutputPreviousPoints;
		  
		public void Execute(int i)
		{

			//var tornado = GetSingleton<Tornado>();

			float3 point = InputCurrentPoints[i].Value;
			float3 previousPoint = InputPreviousPoints[i].Value;
			if (Anchors[i].Value == false)
			{
				float startX = point.x;
				float startY = point.y;
				float startZ = point.z;

				previousPoint.y += .01f;

				// tornado force
				float tdx = tornado.tornadoX + TornadoSway(point.y, elapsedTime) - point.x;
				float tdz = tornado.tornadoZ - point.z;
				float tornadoDist = math.sqrt(tdx * tdx + tdz * tdz);
				tdx /= tornadoDist;
				tdz /= tornadoDist;

				if (tornadoDist < tornado.maxForceDist)
				{
					float force = (1f - tornadoDist / tornado.maxForceDist);
					float yFader = math.clamp(1f - point.y / tornado.height, 0f, 1f);
					force *= tornado.fader * tornado.force * random.NextFloat(-.3f, 1.3f);
					float forceY = tornado.upForce;

					previousPoint.y -= forceY * force;

					float forceX = -tdz + tdx * tornado.inwardForce * yFader;
					float forceZ = tdx + tdz * tornado.inwardForce * yFader;

					previousPoint.x -= forceX * force;
					previousPoint.z -= forceZ * force;
				}

				float invDamping = 1f - constants.airResistance;

				point.x += (point.x - previousPoint.x) * invDamping;
				point.y += (point.y - previousPoint.y) * invDamping;
				point.z += (point.z - previousPoint.z) * invDamping;

				previousPoint.x = startX;
				previousPoint.y = startY;
				previousPoint.z = startZ;

				if (point.y < 0f)
				{
					point.y = 0f;
					previousPoint.y = -previousPoint.y;
					previousPoint.x += (point.x - previousPoint.x) * constants.friction;
					previousPoint.z += (point.z - previousPoint.z) * constants.friction;
				}

				OutputCurrentPoints[i] = new CurrentPoint() { Value = point };
				OutputPreviousPoints[i] = new PreviousPoint() { Value = previousPoint };

			}
		}
		 
    }

	protected override void OnCreate()
	{
		m_Random = new Random(0x123467);
		m_BeamBatches = new List<BeamBatch>();

	}


	protected override void OnUpdate()
    {
		/*var ecb = new EntityCommandBuffer(Allocator.Temp);

		var worldEntity = GetSingletonEntity<World>();
		var currentPoints = GetBuffer<CurrentPoint>(worldEntity);
		var previousPoints = GetBuffer<PreviousPoint>(worldEntity);
		var anchors = GetBuffer<AnchorPoint>(worldEntity);*/

		var tornado = GetSingleton<Tornado>();
		var constants = GetSingleton<PhysicalConstants>();


		tornado.fader = math.clamp(tornado.fader + Time.DeltaTime / 10f, 0f, 1f);
		SetSingleton(tornado);

		if (m_BeamBatches.Count == 0)
		{
			EntityManager.GetAllUniqueSharedComponentData(m_BeamBatches);
		}
		var worldQuery = GetEntityQuery(typeof(World), typeof(BeamBatch));
		
		if(m_WorldEntititesCache == null && m_BeamBatches.Count > 0) {
			m_WorldEntititesCache = new NativeArray<Entity>[m_BeamBatches.Count];
			m_CurrentPointsCache = new NativeArray<CurrentPoint>[m_BeamBatches.Count];
			m_PreviousPointsCache = new NativeArray<PreviousPoint>[m_BeamBatches.Count];
			m_AnchorsCache = new NativeArray<AnchorPoint>[m_BeamBatches.Count];
			m_NeighborCountsCache = new NativeArray<NeighborCount>[m_BeamBatches.Count];
			for (var i = 0; i < m_BeamBatches.Count; i++)
			{
				worldQuery.SetSharedComponentFilter(m_BeamBatches[i]);

				m_WorldEntititesCache[i] = worldQuery.ToEntityArray(Allocator.Persistent);
				
				var worldEntity = m_WorldEntititesCache[i][0];
				m_CurrentPointsCache[i] = GetBuffer<CurrentPoint>(worldEntity).AsNativeArray();
				m_PreviousPointsCache[i] = GetBuffer<PreviousPoint>(worldEntity).AsNativeArray();
				m_AnchorsCache[i] = GetBuffer<AnchorPoint>(worldEntity).AsNativeArray();
				m_NeighborCountsCache[i] = GetBuffer<NeighborCount>(worldEntity).AsNativeArray();

			}
		}


		for (var i = 0; i < m_BeamBatches.Count; i++) {

			var job = new PointSimulationSystemJob();

			job.elapsedTime = (float)Time.ElapsedTime;
			job.InputCurrentPoints = m_CurrentPointsCache[i];
			job.InputPreviousPoints = m_PreviousPointsCache[i];
			job.Anchors = m_AnchorsCache[i];
			job.OutputCurrentPoints = m_CurrentPointsCache[i];
			job.OutputPreviousPoints = m_PreviousPointsCache[i];
			job.tornado = tornado;
			job.random = m_Random;
			job.constants = constants;

			var handle = job.Schedule(m_CurrentPointsCache[i].Length, 1024*1024*64, Dependency);
			Dependency = JobHandle.CombineDependencies(Dependency, handle);
		}

		//TODO: complete in the optimal place
		Dependency.Complete();
		 

	}
}
