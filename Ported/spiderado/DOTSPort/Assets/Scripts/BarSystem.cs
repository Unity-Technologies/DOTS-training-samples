using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct ModifiedPoint {
    public BarPoint point;
    public Entity pointEntity;
    public int isInitialized;
}

public struct NewPoint {
    public BarPoint point;
    public Bar bar;
    public Entity barEntity;
    public int isPoint1;
    public int isInitialized;
}

public class BarSystem : JobComponentSystem
{
    public static void CopyFromToPoint(BarPoint p1, BarPoint p2) {
        p2.pos = p1.pos;
        p2.oldPos = p1.oldPos;
        p2.anchor = p1.anchor;
        p2.neighborCount = p1.neighborCount;
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        int maxNumBars = 9;

        var points = GetComponentDataFromEntity<BarPoint>(true);
        var modifiedPointList = new NativeArray<ModifiedPoint>(maxNumBars * 2, Allocator.TempJob);
        var newPointList = new NativeArray<NewPoint>(maxNumBars, Allocator.TempJob);
        var breakResistance = 0.5f;

        var barJob = Entities
            .WithReadOnly(points)
            .ForEach((int entityInQueryIndex, Entity e, ref Bar bar, ref LocalToWorld localToWorld, ref Translation position) => {

                BarPoint point1 = points[bar.point1];
                BarPoint point2 = points[bar.point2];

                float dx = point2.pos.x - point1.pos.x;
                float dy = point2.pos.y - point1.pos.y;
                float dz = point2.pos.z - point1.pos.z;

                float dist = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
                float extraDist = dist - bar.length;

                float pushX = (dx / dist * extraDist) * .5f;
                float pushY = (dy / dist * extraDist) * .5f;
                float pushZ = (dz / dist * extraDist) * .5f;

                if (point1.anchor == 0 && point2.anchor == 0) {
                    point1.pos.x += pushX;
                    point1.pos.y += pushY;
                    point1.pos.z += pushZ;
                    point2.pos.x -= pushX;
                    point2.pos.y -= pushY;
                    point2.pos.z -= pushZ;
                } else if (point1.anchor == 1) {
                    point2.pos.x -= pushX * 2f;
                    point2.pos.y -= pushY * 2f;
                    point2.pos.z -= pushZ * 2f;
                } else if (point2.anchor == 1) {
                    point1.pos.x += pushX * 2f;
                    point1.pos.y += pushY * 2f;
                    point1.pos.z += pushZ * 2f;
                }

                if (dx / dist * bar.oldPos.x + dy / dist * bar.oldPos.y + dz / dist * bar.oldPos.z < .99f) {
                    // bar has rotated: expensive full-matrix computation
                    position.Value = new float3(
                        (point1.pos.x + point2.pos.x) * .5f,
                        (point1.pos.y + point2.pos.y) * .5f,
                        (point1.pos.z + point2.pos.z) * .5f
                    );
                    // TODO: Set rotation

                    //localToWorld.Value = Matrix4x4.TRS(
                    //    new Vector3(
                    //        (point1.pos.x + point2.pos.x) * .5f,
                    //        (point1.pos.y + point2.pos.y) * .5f,
                    //        (point1.pos.z + point2.pos.z) * .5f),
                    //    Quaternion.LookRotation(new Vector3(dx, dy, dz)) as quaternion,
                    //    new Vector3(bar.thickness, bar.thickness, bar.length));
                    bar.oldPos.x = dx / dist;
                    bar.oldPos.y = dy / dist;
                    bar.oldPos.z = dz / dist;
                } else {
                    // bar hasn't rotated: set Transform directly
                    position.Value = new float3(
                        (point1.pos.x + point2.pos.x) * .5f,
                        (point1.pos.y + point2.pos.y) * .5f,
                        (point1.pos.z + point2.pos.z) * .5f
                    );
                }
                if (Mathf.Abs(extraDist) > breakResistance) {
                    if (point2.neighborCount > 1) {
                        point2.neighborCount--;
                        BarPoint newPoint = new BarPoint();
                        CopyFromToPoint(point2, newPoint);
                        newPoint.neighborCount = 1;
                        newPointList[entityInQueryIndex] = new NewPoint {
                            bar = bar,
                            barEntity = e,
                            isPoint1 = 0,
                            point = newPoint,
                            isInitialized = 1
                        };
                    } else if (point1.neighborCount > 1) {
                        point1.neighborCount--;
                        BarPoint newPoint = new BarPoint();
                        CopyFromToPoint(point1, newPoint);
                        newPoint.neighborCount = 1;

                        newPointList[entityInQueryIndex] = new NewPoint {
                            bar = bar,
                            barEntity = e,
                            isPoint1 = 1,
                            point = newPoint,
                            isInitialized = 1
                        };
                    }
                }

                bar.minBounds.x = Mathf.Min(point1.pos.x, point2.pos.x);
                bar.maxBounds.x = Mathf.Max(point1.pos.x, point2.pos.x);
                bar.minBounds.y = Mathf.Min(point1.pos.y, point2.pos.y);
                bar.maxBounds.y = Mathf.Max(point1.pos.y, point2.pos.y);
                bar.minBounds.z = Mathf.Min(point1.pos.z, point2.pos.z);
                bar.maxBounds.z = Mathf.Max(point1.pos.z, point2.pos.z);

                modifiedPointList[entityInQueryIndex] = new ModifiedPoint {
                    isInitialized = 1,
                    point = point1,
                    pointEntity = bar.point1
                };
                modifiedPointList[entityInQueryIndex] = new ModifiedPoint {
                    isInitialized = 1,
                    point = point2,
                    pointEntity = bar.point2
                };
                //Debug.Log(entityInQueryIndex);
            }).Schedule(inputDeps);

        barJob.Complete();
        // Job done!

        int modifiedCount = 0;
        for (int i = 0; i < modifiedPointList.Length; i++) {
            if (modifiedPointList[i].isInitialized == 0)
                continue;
            var modifiedPoint = modifiedPointList[i];
            EntityManager.SetComponentData(modifiedPoint.pointEntity, modifiedPoint.point);
            modifiedCount++;
        }
        Debug.Log(modifiedCount + " points modified");

        for (int i = 0; i < newPointList.Length; i++) {
            if (newPointList[i].isInitialized == 0)
                continue;
            Entity pointEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(pointEntity, newPointList[i].point);
            Bar bar = newPointList[i].bar;
            if (newPointList[i].isPoint1 == 1)
                bar.point1 = pointEntity;
            else
                bar.point2 = pointEntity;
            EntityManager.SetComponentData(newPointList[i].barEntity, bar);
        }

        modifiedPointList.Dispose();
        newPointList.Dispose();

        return barJob;
    }
}

public class PointSystem : JobComponentSystem {
    private float tornadoForceMaxDistVal = 20.0f;

	public static float TornadoSway(float y, float time) {
		return Mathf.Sin(y / 5f + time/4f) * 3f;
	}

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        TornadoData tornadoData = GetSingleton<TornadoData>();

        float tornadoX = tornadoData.tornadoX;
        float tornadoZ = tornadoData.tornadoZ;
        float curTime = Time.time;
        float tornadoForceMaxDist = tornadoForceMaxDistVal;

        var pointJob = Entities.ForEach((Entity e, ref BarPoint point) => {
            if (point.anchor == 0) {
                float startX = point.pos.x;
                float startY = point.pos.y;
                float startZ = point.pos.z;
            }
            point.oldPos.y += 0.01f;

            // Tornado force
            float tdx = tornadoX+TornadoSway(point.pos.y, curTime) - point.pos.x;
            float tdz = tornadoZ - point.pos.z;
            float tornadoDist = Mathf.Sqrt(tdx * tdx + tdz * tdz);
            tdx /= tornadoDist;
            tdz /= tornadoDist;
            if (tornadoDist < tornadoForceMaxDist) {
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