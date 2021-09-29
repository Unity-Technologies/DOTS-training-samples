using Assets.Scripts.Components;
using Unity.Burst;
using Unity.Collections;
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

	protected override void OnUpdate()
    {
		var ecb = new EntityCommandBuffer(Allocator.Temp);

		var worldEntity = GetSingletonEntity<World>();
		var currentPoints = GetBuffer<CurrentPoint>(worldEntity);
		var previousPoints = GetBuffer<PreviousPoint>(worldEntity);
		var anchors = GetBuffer<AnchorPoint>(worldEntity);

		var tornado = GetSingleton<Tornado>();
		var constants = GetSingleton<PhysicalConstants>();

		var myTime = (float)Time.ElapsedTime;

		var random = new Random(0x123467);

		tornado.fader = math.clamp(tornado.fader + Time.DeltaTime / 10f, 0f, 1f);
		SetSingleton<Tornado>(tornado);

		float invDamping = 1f - constants.airResistance;
		for (int i = 0; i < currentPoints.Length; i++)
		{
			float3 point = currentPoints[i].Value;
			float3 previousPoint = previousPoints[i].Value;

			if (anchors[i].Value == false)
			{
				float startX = point.x;
				float startY = point.y;
				float startZ = point.z;

				previousPoint.y += .01f;
		//		previousPoint.y += constants.gravity * Time.DeltaTime;

				// tornado force
				float tdx = tornado.tornadoX + TornadoSway(point.y, myTime) - point.x;
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

				//collision with the ground
				if (point.y < 0f)
				{
					point.y = 0f;
					previousPoint.y = - previousPoint.y;
					previousPoint.x += (point.x - previousPoint.x) * constants.friction;
					previousPoint.z += (point.z - previousPoint.z) * constants.friction;
				}

				currentPoints[i] = new CurrentPoint() { Value = point };
				previousPoints[i] = new PreviousPoint() { Value = previousPoint };
				
				UnityEngine.Debug.DrawLine(new UnityEngine.Vector3(previousPoint.x, previousPoint.y, previousPoint.z), new UnityEngine.Vector3(point.x, point.y, point.z));
			}
		}


    }
}
