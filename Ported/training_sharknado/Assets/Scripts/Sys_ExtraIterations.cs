using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(ConnectionSystem))]
public class Sys_ExtraIterations : JobComponentSystem
{
    public Random random;
    [BurstCompile]
    struct UpdateBarsJob : IJobForEach<BarPoint1, BarPoint2, BarLength>
    {
        public Random random;
        public float time;
        public float3 tornadoPos;
        public float damping;
        public float friction;
        public float invDamping;
        public float tornadoMaxForceDist;
        public float tornadoHeight;
        public float tornadoUpForce;
        public float tornadoInwardForce;
        public float tornadoForce;
        public float breakResistance;

        public static float TornadoSway(float y, float t)
        {
            return math.sin(y / 5f + t / 4f) * 3f;
        }

        public void Execute(ref BarPoint1 point1, ref BarPoint2 point2, ref BarLength length)
        {
            var time = this.time;

            // TODO(wyatt): move these to a component
            // {


            // }

            var tornadoFader = math.saturate(time / 10f);

            float3 pos1 = point1.pos;
            float3 pos2 = point2.pos;
            // Keep bar points together
            pos1 = point1.pos;
            pos2 = point2.pos;

            float dx = pos2.x - pos1.x;
            float dy = pos2.y - pos1.y;
            float dz = pos2.z - pos1.z;

            float dist = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
            float extraDist = dist - length.value;

            float pushX = (dx / dist * extraDist) * .5f;
            float pushY = (dy / dist * extraDist) * .5f;
            float pushZ = (dz / dist * extraDist) * .5f;

            if (point1.neighbors != -1 && point2.neighbors != -1)
            {
                pos1.x += pushX;
                pos1.y += pushY;
                pos1.z += pushZ;
                pos2.x -= pushX;
                pos2.y -= pushY;
                pos2.z -= pushZ;
            }
            else if (point1.neighbors == -1)
            {
                pos2.x -= pushX * 2;
                pos2.y -= pushY * 2;
                pos2.z -= pushZ * 2;
            }
            else if (point2.neighbors == -1)
            {
                pos1.x += pushX * 2;
                pos1.y += pushY * 2;
                pos1.z += pushZ * 2;
            }

            if (Mathf.Abs(extraDist) > breakResistance)
            {
                if (point2.neighbors > 0)
                {
                    point2.neighbors = 0;
                    point2.activated = 1;
                }
                else if (point1.neighbors > 0)
                {
                    point1.neighbors = 0;
                    point1.activated = 1;
                }
            }
            point1.pos = pos1;
            point2.pos = pos2;
        }
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        random = new Random(1337u);
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var tornadoData = GetSingleton<TornadoSpawner>();
        var job = new UpdateBarsJob()
        {

            tornadoPos = GetSingleton<TornadoPosition>().position,
            random = this.random,
            time = Time.time,
            damping = tornadoData.damping,
            friction = tornadoData.friction,
            invDamping = 1 - tornadoData.damping,
            tornadoMaxForceDist = tornadoData.maxForceDist,
            tornadoHeight = tornadoData.height,
            tornadoUpForce = tornadoData.upForce,
            tornadoInwardForce = tornadoData.inwardForce,
            tornadoForce = tornadoData.force,
            breakResistance = tornadoData.breakResist
        }.Schedule(this, inputDeps);
        var barPoints1 = GetComponentDataFromEntity<BarPoint1>(false);
        var barPoints2 = GetComponentDataFromEntity<BarPoint2>(false);
        var barAvg1 = GetComponentDataFromEntity<BarAveragedPoints1>(false);
        var barAvg2 = GetComponentDataFromEntity<BarAveragedPoints2>(false);
        var job2 = new ConnectionSystem.KeepThingsTogether()
        {
            barPoints1 = barPoints1,
            barPoints2 = barPoints2,
            barAvgPoints1 = barAvg1,
            barAvgPoints2 = barAvg2
        }.Schedule(this, job);
        var job3 = new ConnectionSystem.CopyAverages()
        {
        }.Schedule(this, job2);
        return job3;
        // return inputDeps;
    }
}