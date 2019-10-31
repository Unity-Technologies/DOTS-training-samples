using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

public struct ModifiedPoint {
    public BarPoint point;
    public Entity pointEntity;
}

public class BarSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var points = GetComponentDataFromEntity<BarPoint>(true);
        var modifiedPointList = new NativeArray<ModifiedPoint>(3, Allocator.TempJob);

        var barJob = Entities.WithReadOnly(points).ForEach((int entityInQueryIndex, Entity e, ref Bar bar) => {
            BarPoint modifiedPoint = new BarPoint { pos = new float3(1f, 1f, 1f) };
            BarPoint modifiedPoint2 = new BarPoint { pos = new float3(2f, 2f, 2f) };
            var modPoint = new ModifiedPoint { point = modifiedPoint, pointEntity = bar.point1 };
            var modPoint2 = new ModifiedPoint { point = modifiedPoint2, pointEntity = bar.point2 };
            modifiedPointList[entityInQueryIndex] = modPoint;
            Debug.Log(points);
        }).Schedule(inputDeps);

        barJob.Complete();
        // Job done!
        for (int i = 0; i < modifiedPointList.Length; i++) {
            var modifiedPoint = modifiedPointList[i];
            EntityManager.SetComponentData(modifiedPoint.pointEntity, modifiedPoint.point);
        }

        return barJob;
    }
}

public class PointSystem : JobComponentSystem {
    private float tornadoMaxForceDist = 20.0f;

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float tornadoX = ParticleSystem.tornadoX;
        float tornadoZ = ParticleSystem.tornadoZ;
        float curTime = Time.time;

        var pointJob = Entities.ForEach((Entity e, ref BarPoint point) => {
            if (!point.anchor) {
				float startX = point.pos.x;
				float startY = point.pos.y;
				float startZ = point.pos.z;
            }
            point.oldPos.y += 0.01f;

            // Tornado force
			float tdx = tornadoX+ParticleSystem.TornadoSway(point.pos.y, curTime) - point.pos.x;
            float tdz = tornadoZ - point.pos.z;
			float tornadoDist = Mathf.Sqrt(tdx * tdx + tdz * tdz);
			tdx /= tornadoDist;
			tdz /= tornadoDist;
            if (tornadoDist < tornadoMaxForceDist) {
            }
            //	float force = (1f - tornadoDist / tornadoMaxForceDist);
            //	float yFader= Mathf.Clamp01(1f - point.y / tornadoHeight);
            //	force *= tornadoFader*tornadoForce*Random.Range(-.3f,1.3f);
            //	float forceY = tornadoUpForce;
            //	point.oldY -= forceY * force;
            //	float forceX = -tdz + tdx * tornadoInwardForce*yFader;
            //	float forceZ = tdx + tdz * tornadoInwardForce*yFader;
            //	point.oldX -= forceX * force;
            //	point.oldZ -= forceZ * force;
            //}
            //point.x += (point.x - point.oldX) * invDamping;
            //point.y += (point.y - point.oldY) * invDamping;
            //point.z += (point.z - point.oldZ) * invDamping;
            //point.oldX = startX;
            //point.oldY = startY;
            //point.oldZ = startZ;
            //if (point.y < 0f) {
            //	point.y = 0f;
            //	point.oldY = -point.oldY;
            //	point.oldX += (point.x - point.oldX) * friction;
            //	point.oldZ += (point.z - point.oldZ) * friction;
            //}

        }).Schedule(inputDeps);

        return pointJob;
    }
}

/*
Point point = points[i];
			if (point.anchor == false) {
				float startX = point.x;
				float startY = point.y;
				float startZ = point.z;

				point.oldY += .01f;

				// tornado force
				float tdx = tornadoX+TornadoSway(point.y) - point.x;
				float tdz = tornadoZ - point.z;
				float tornadoDist = Mathf.Sqrt(tdx * tdx + tdz * tdz);
				tdx /= tornadoDist;
				tdz /= tornadoDist;
				if (tornadoDist<tornadoMaxForceDist) {
					float force = (1f - tornadoDist / tornadoMaxForceDist);
					float yFader= Mathf.Clamp01(1f - point.y / tornadoHeight);
					force *= tornadoFader*tornadoForce*Random.Range(-.3f,1.3f);
					float forceY = tornadoUpForce;
					point.oldY -= forceY * force;
					float forceX = -tdz + tdx * tornadoInwardForce*yFader;
					float forceZ = tdx + tdz * tornadoInwardForce*yFader;
					point.oldX -= forceX * force;
					point.oldZ -= forceZ * force;
				}

				point.x += (point.x - point.oldX) * invDamping;
				point.y += (point.y - point.oldY) * invDamping;
				point.z += (point.z - point.oldZ) * invDamping;

				point.oldX = startX;
				point.oldY = startY;
				point.oldZ = startZ;
				if (point.y < 0f) {
					point.y = 0f;
					point.oldY = -point.oldY;
					point.oldX += (point.x - point.oldX) * friction;
					point.oldZ += (point.z - point.oldZ) * friction;
				}
			}
*/
