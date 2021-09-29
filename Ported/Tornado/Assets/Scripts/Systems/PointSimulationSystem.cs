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
	public static float TornadoSway(float y, float time)
	{
		return math.sin(y / 5f + time / 4f) * 3f;
	}

	[BurstCompile]
	private struct PointSimulationSystemJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<SpawnerSystem.CurrentPoint> InputCurrentPoints;
		[ReadOnly]
		public NativeArray<SpawnerSystem.PreviousPoint> InputPreviousPoints;
		[ReadOnly]
		public Tornado tornado;
		[ReadOnly]
		public Random random;
		[ReadOnly]
		public PhysicalConstants constants;

		[WriteOnly]
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<SpawnerSystem.CurrentPoint> OutputCurrentPoints;
		[WriteOnly]
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<SpawnerSystem.PreviousPoint> OutputPreviousPoints;
		  
		public void Execute(int i)
		{

			//var tornado = GetSingleton<Tornado>();

			float3 point = InputCurrentPoints[i].Value;
			float3 previousPoint = InputPreviousPoints[i].Value;
			//	if (point.anchor == false)
			{
				float startX = point.x;
				float startY = point.y;
				float startZ = point.z;

				previousPoint.y += .01f;

				// tornado force
				float tdx = tornado.tornadoX + TornadoSway(point.y, tornado.internalTime) - point.x;
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

				OutputCurrentPoints[i] = new SpawnerSystem.CurrentPoint() { Value = point };
				OutputPreviousPoints[i] = new SpawnerSystem.PreviousPoint() { Value = previousPoint };

			}
		}
		 
    }


	protected override void OnUpdate()
    {
		var ecb = new EntityCommandBuffer(Allocator.Temp);

		var worldEntity = GetSingletonEntity<World>();
		var currentPoints = GetBuffer<SpawnerSystem.CurrentPoint>(worldEntity);
		var previousPoints = GetBuffer<SpawnerSystem.PreviousPoint>(worldEntity);
		 

		var tornado = GetSingleton<Tornado>();
		var constants = GetSingleton<PhysicalConstants>();

		tornado.internalTime += Time.DeltaTime;
		var myTime = Time.DeltaTime;

		var random = new Random(0x123467);

		tornado.fader = math.clamp(tornado.fader + myTime / 10f, 0f, 1f);
		SetSingleton<Tornado>(tornado);

		float invDamping = 1f - constants.airResistance;

		var job = new PointSimulationSystemJob();


		job.InputCurrentPoints = currentPoints.AsNativeArray();
		job.InputPreviousPoints = previousPoints.AsNativeArray();
		job.OutputCurrentPoints = currentPoints.AsNativeArray();
		job.OutputPreviousPoints = previousPoints.AsNativeArray();
		job.tornado = tornado;
		job.random = random;
		job.constants = constants;

		var handle = job.Schedule(currentPoints.Length, 64, Dependency);
		handle.Complete();

		/*
		for (int i = 0; i < currentPointsNativeArray.Length; i++)
		{
			float3 point = currentPointsNativeArray[i].Value;
			float3 previousPoint = previousPointsNativeArray[i].Value;
			//	if (point.anchor == false)
			{
				float startX = point.x;
				float startY = point.y;
				float startZ = point.z;

				previousPoint.y += .01f;

				// tornado force
				float tdx = tornado.tornadoX + TornadoSway(point.y, tornado.internalTime) - point.x;
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

				point.x += (point.x - previousPoint.x) * invDamping;
				point.y += (point.y - previousPoint.y) * invDamping;
				point.z += (point.z - previousPoint.z) * invDamping;

				previousPoint.x = startX;
				previousPoint.y = startY;
				previousPoint.z = startZ;

				if (point.y < 0f)
				{
					point.y = 0f;
					previousPoint.y = - previousPoint.y;
					previousPoint.x += (point.x - previousPoint.x) * constants.friction;
					previousPoint.z += (point.z - previousPoint.z) * constants.friction;
				}

				currentPointsNativeArray[i] = new SpawnerSystem.CurrentPoint() { Value = point };
				previousPointsNativeArray[i] = new SpawnerSystem.PreviousPoint() { Value = previousPoint };
				 
			}
		}
		*/


	}
}
