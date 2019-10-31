using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;


public class ParticleJobSystem : JobComponentSystem
{
    private EntityQuery m_particleQuery;
    protected override void OnCreate()
    {
        m_particleQuery = GetEntityQuery( new EntityQueryDesc
        {
            All = new[] {ComponentType.ReadWrite<Translation>(), ComponentType.ReadOnly<Rotation>()}
        });
    }

    [BurstCompile]
    [RequireComponentTag(typeof(TornadoData))]
    struct TornadoParticleSystem : IJobForEach<Translation>
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly] public TornadoData tornado;
        public void Execute(ref Translation translation)
        {
            float3 tornadoPos = new float3(
                Mathf.Cos(deltaTime / 6f) * 30f + Mathf.Sin((translation.Value.y) / 5f + deltaTime / 4f) * 3f,
                translation.Value.y,
                Mathf.Sin(deltaTime / 6f * 1.618f) * 30f);
            float3 delta = (tornadoPos - translation.Value);
            float dist = math.length(delta);
            delta /= dist;
            float inForce = dist - Mathf.Clamp01(tornadoPos.y / 50f) * 30f * tornado.radiusMultiplier + 2f;
            translation.Value += new float3(-delta.z * tornado.spinRate + delta.x * inForce, tornado.upwardSpeed, delta.x * tornado.spinRate + delta.z * inForce) * deltaTime;
            if (translation.Value.y > 50f)
            {
                translation.Value = new Vector3(translation.Value.x, translation.Value.y - 50f, translation.Value.z);
            }
            // Matrix4x4 matrix = partData.matrix;
            // matrix.m03 = translation.Value.x;
            // matrix.m13 = translation.Value.y;
            // matrix.m23 = translation.Value.z;
            // partData.matrix = matrix;


        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new TornadoParticleSystem
        {
            tornado = GetSingleton<TornadoData>(),
            deltaTime = Time.deltaTime

        };

        return job.Schedule(m_particleQuery, inputDependencies);
    }

    /*
    protected override void OnUpdate() {
		tornadoX = Mathf.Cos(Time.time/6f) * 30f;
		tornadoZ = Mathf.Sin(Time.time/6f * 1.618f) * 30f;
        tornadoY = UnityEngine.Random.Range(-0.3f, 0.3f);

        Entities.ForEach((Entity e, ParticleSharedData tornado, ref Point point, ref PartData partData) => {
			float3 tornadoPos = new float3(
                tornadoX + TornadoSway(translation.Value.y),
                translation.Value.y, 
                tornadoZ);
			float3 delta = (tornadoPos - translation.Value);
			float dist = math.length(delta);
			delta /= dist;
			float inForce = dist - Mathf.Clamp01(tornadoPos.y / 50f)*30f*partData.radiusMult+2f;
			translation.Value += new float3(-delta.z*tornado.spinRate+delta.x*inForce,tornado.upwardSpeed,delta.x*tornado.spinRate+delta.z*inForce)*Time.deltaTime;
			if (translation.Value.y>50f) {
				translation.Value = new Vector3(translation.Value.x,0f+ tornadoY,translation.Value.z);
			}

            Matrix4x4 matrix = partData.matrix;
            matrix.m03 = translation.Value.x;
            matrix.m13 = translation.Value.y;
            matrix.m23 = translation.Value.z;
            partData.matrix = matrix;
            Graphics.DrawMesh(tornado.particleMesh, partData.matrix, tornado.particleMaterial, 1);
        });
        
    }*/
}