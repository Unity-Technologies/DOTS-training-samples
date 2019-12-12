using System.Drawing;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class BarSimulationSystem : JobComponentSystem
{
    EntityQuery q;
    EntityQuery bq;
    
    protected override void OnCreate()
    {
        q = GetEntityQuery(typeof(DynamicBuffer<ConstrainedPointEntry>));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var e = q.GetSingletonEntity();
        var pointsBuf = EntityManager.GetBuffer<ConstrainedPointEntry>(e).AsNativeArray().Reinterpret<ConstrainedPoint>();

        var entities = GetEntityQuery(typeof(DynamicBuffer<BarEntry>)).ToEntityArray(Allocator.Temp);

        foreach (var be in entities)
        {
            NativeArray<Bar> bars = EntityManager.GetBuffer<BarEntry>(be).AsNativeArray().Reinterpret<Bar>();
            
            for (int i = 0; i < bars.Length; i++)
            {
                Bar bar = bars[i];

                var point1 = pointsBuf[bar.p1];
                var point2 = pointsBuf[bar.p2];

                //float dx = point2.x - point1.x;
                //float dy = point2.y - point1.y;
                //float dz = point2.z - point1.z;
                var dd = point2.position - point1.position;

                float dist = Unity.Mathematics.math.length(dd);
                float extraDist = dist - bar.length;

//                float pushX = (dx / dist * extraDist) * .5f;
//                float pushY = (dy / dist * extraDist) * .5f;
//                float pushZ = (dz / dist * extraDist) * .5f;
                var push = dd/dist * extraDist * 0.5f;

                if (point1.anchor == false && point2.anchor == false)
                {
//                    point1.x += pushX;
//                    point1.y += pushY;
//                    point1.z += pushZ;
//                    point2.x -= pushX;
//                    point2.y -= pushY;
//                    point2.z -= pushZ;
                    point1.position += push;
                    point2.position -= push;
                }
                else if (point1.anchor)
                {
//                    point2.x -= pushX * 2f;
//                    point2.y -= pushY * 2f;
//                    point2.z -= pushZ * 2f;
                    point2.position -= push * 2f;
                }
                else if (point2.anchor)
                {
//                    point1.x += pushX * 2f;
//                    point1.y += pushY * 2f;
//                    point1.z += pushZ * 2f;
                    point1.position += push * 2f;
                }

//                if (dx / dist * bar.oldDX + dy / dist * bar.oldDY + dz / dist * bar.oldDZ < .99f)
//                {
//                    // bar has rotated: expensive full-matrix computation
//                    bar.matrix = Matrix4x4.TRS(new Vector3((point1.x + point2.x) * .5f, (point1.y + point2.y) * .5f, (point1.z + point2.z) * .5f),
//                        Quaternion.LookRotation(new Vector3(dx, dy, dz)),
//                        new Vector3(bar.thickness, bar.thickness, bar.length));
//                    bar.oldDX = dx / dist;
//                    bar.oldDY = dy / dist;
//                    bar.oldDZ = dz / dist;
//                }
//                else
//                {
//                    // bar hasn't rotated: only update the position elements
//                    Matrix4x4 matrix = bar.matrix;
//                    matrix.m03 = (point1.x + point2.x) * .5f;
//                    matrix.m13 = (point1.y + point2.y) * .5f;
//                    matrix.m23 = (point1.z + point2.z) * .5f;
//                    bar.matrix = matrix;
//                }

//                if (Mathf.Abs(extraDist) > breakResistance)
//                {
//                    if (point2.neighborCount > 1)
//                    {
//                        point2.neighborCount--;
//                        Point newPoint = new Point();
//                        newPoint.CopyFrom(point2);
//                        newPoint.neighborCount = 1;
//                        points[pointCount] = newPoint;
//                        bar.point2 = newPoint;
//                        pointCount++;
//                    }
//                    else if (point1.neighborCount > 1)
//                    {
//                        point1.neighborCount--;
//                        Point newPoint = new Point();
//                        newPoint.CopyFrom(point1);
//                        newPoint.neighborCount = 1;
//                        points[pointCount] = newPoint;
//                        bar.point1 = newPoint;
//                        pointCount++;
//                    }
//                }
            }
        }

        return inputDeps;
    }
}
