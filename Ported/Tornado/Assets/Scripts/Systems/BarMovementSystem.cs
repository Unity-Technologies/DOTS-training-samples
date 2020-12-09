using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Rendering;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(BarSpawningSystem))]
public class BarMovementSytem : SystemBase
{
    EntityQuery buildingsQuery;
    float tornadoFader;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<TornadoSettings>();
        RequireSingletonForUpdate<Tornado>();

        buildingsQuery = GetEntityQuery(ComponentType.ReadOnly<Building>());
    }

    protected override void OnUpdate()
    {
        var settings = GetSingleton<TornadoSettings>();
        var tornadoTranslation = EntityManager.GetComponentData<Translation>(GetSingletonEntity<Tornado>());

        Random random = new Random(1234);

        tornadoFader = math.clamp(tornadoFader + Time.DeltaTime / 10f, 0f, 1f);
        float invDamping = 1f - settings.Damping;
        float deltaTime = Time.DeltaTime;

        Entities.WithoutBurst().ForEach((ref Node node, ref Translation translation) =>
        {

            if (!node.anchor)
            {
                float3 start = translation.Value;

                node.oldPosition.y += .01f;

                float tornadoSway = math.sin(translation.Value.y / 5f + deltaTime / 4f) * 3f;

                float2 td = new float2(
                    tornadoTranslation.Value.x + tornadoSway - translation.Value.x,
                    tornadoTranslation.Value.z - translation.Value.z);

                float tornadoDist = math.length(td);

                td = math.normalize(td);

                if (tornadoDist < settings.TornadoMaxForceDistance)
                {
                    float force = (1f - tornadoDist / settings.TornadoMaxForceDistance);
                    float yFader = math.saturate(1f - translation.Value.y / settings.TornadoHeight);
                    force *= tornadoFader * settings.TornadoForce * random.NextFloat(-0.3f, 1.3f);
                    float3 force3 = new float3(0);
                    force3.y = settings.TornadoUpForce;
                    force3.x = -td.y + td.x * settings.TornadoInwardForce * yFader;
                    force3.z =  td.x + td.y * settings.TornadoInwardForce * yFader;

                    node.oldPosition -= force3 * force;
                }

                translation.Value += (translation.Value - node.oldPosition) * invDamping;
                node.oldPosition = start;

                if (translation.Value.y < 0f)
                {
                    translation.Value.y = 0f;
                    node.oldPosition.y = -node.oldPosition.y;
                    node.oldPosition.xz += (translation.Value.xz - node.oldPosition.xz) * settings.Friction;
                }
            }

            Debug.DrawRay(translation.Value, Vector3.up, Color.red);

        }).Run();

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((in DynamicBuffer<Constraint> constraints) =>
        {
            for (int i = 0; i < constraints.Length; i++)
            {
                Constraint constraint = constraints[i];

                Node point1 = GetComponent<Node>(constraint.pointA);
                float3 point1Pos = GetComponent<Translation>(constraint.pointA).Value;
                Node point2 = GetComponent<Node>(constraint.pointB);
                float3 point2Pos = GetComponent<Translation>(constraint.pointB).Value;

                float3 d = point2Pos - point1Pos;
                float dist = math.length(d);
                float extraDist = dist - constraint.distance;

                float3 push = (d / dist * extraDist) * .5f;

                if (point1.anchor == false && point2.anchor == false)
                {
                    point1Pos += push;
                    point2Pos -= push;
                }
                else if (point1.anchor)
                {
                    point2Pos -= push * 2f;
                }
                else
                {
                    point1Pos += push * 2f;
                }

                if (math.abs(extraDist) > settings.BreakResistance)
                {
                    if (point2.neighborCount > 1)
                    {
                        point2.neighborCount--;

                        var newPoint = point2;
                        newPoint.neighborCount = 1;

                        Entity newPointEntity = ecb.CreateEntity();
                        ecb.AddComponent(newPointEntity, newPoint);
                        ecb.AddComponent(newPointEntity, new Translation { Value = point2Pos });

                        constraint.pointB = newPointEntity;
                    }
                    else if (point1.neighborCount > 1)
                    {
                        point1.neighborCount--;

                        var newPoint = point1;
                        newPoint.neighborCount = 1;

                        Entity newPointEntity = ecb.CreateEntity();
                        ecb.AddComponent(newPointEntity, newPoint);
                        ecb.AddComponent(newPointEntity, new Translation { Value = point1Pos });

                        constraint.pointA = newPointEntity;
                    }
                }

                ecb.SetComponent(constraint.pointA, new Translation { Value = point1Pos });
                ecb.SetComponent(constraint.pointB, new Translation { Value = point2Pos }); 
                
                Debug.DrawLine(point1Pos, point2Pos, Color.green);
            }
        }).Run();

        ecb.Playback(EntityManager);

        var buildingEntities = buildingsQuery.ToEntityArray(Allocator.TempJob);
        var constraintsBuffer = GetBuffer<Constraint>(buildingEntities[0]);
        var constraintsArray = constraintsBuffer.AsNativeArray();

        Entities.WithAll<NonUniformScale>().ForEach((int entityInQueryIndex, Entity entity) =>
        {
            var pointAEntity = constraintsArray[entityInQueryIndex].pointA;
            var pointBEntity = constraintsArray[entityInQueryIndex].pointB;
            var pointA = GetComponent<Translation>(pointAEntity).Value;
            var pointB = GetComponent<Translation>(pointBEntity).Value;
        
            SetComponent(entity, new Translation { Value = (pointA + pointB) * 0.5f });
            SetComponent(entity, new Rotation { Value = Quaternion.LookRotation(((Vector3)(pointA - pointB)).normalized) });
        }).Run();

        buildingEntities.Dispose();
    }
}
