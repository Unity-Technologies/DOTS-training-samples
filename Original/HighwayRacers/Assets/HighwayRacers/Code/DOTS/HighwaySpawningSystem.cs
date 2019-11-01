using System;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class HighwaySpawningSystem : ComponentSystem
{
    // Once cars are spawned we add this component to the spawner entity so that we don't get to spawn again.
    [Serializable]
    public struct DeactiveSpawner : IComponentData
    { }

    protected override void OnUpdate()
    {
        Entities.WithNone<DeactiveSpawner>().ForEach((Entity e, ref HighwayProperties hiway, ref LocalToWorld localToWorld) =>
        {
            float totalLen = hiway.highwayLength;
            float totalCurve = HighwayConstants.CURVE_LANE0_RADIUS * Mathf.PI * 2;
            float straightLen = (totalLen - totalCurve) / 4.0f; // Length of each straight piece.
            float scaleFactor = straightLen / HighwayConstants.STRAIGHT_BASE_LENGTH;// * HighwayConstants.STRAIGHT_BASE_SCALE;

            // We know there are 8 pieces -- 4 curve and 4 straight.
            var entities = new NativeArray<Entity>(8, Allocator.Temp);

            Vector3 pos = Vector3.zero;
            float rot = 0;
            Quaternion rotQ = Quaternion.identity;
            for (int i = 0; i < 8; i++)
            {
                HighwayPieceProperties hpp;
                if (i % 2 == 0)
                {
                    entities[i] = EntityManager.Instantiate(hiway.straightPrefab);
                    EntityManager.AddComponent(entities[i], typeof(HighwayPieceProperties));

                    hpp.isStraight = true;
                    hpp.length = straightLen;
                    hpp.startRotation = rot;
                    EntityManager.SetComponentData(entities[i], hpp);
                    // Set the transform of the entity.
                    float4x4 xForm = float4x4.TRS(localToWorld.Position + (float3)pos,
                                    rotQ,
                                    new float3(1, 1, scaleFactor));
                    PostUpdateCommands.SetComponent<LocalToWorld>(entities[i], new LocalToWorld
                    {
                        Value = xForm
                    });
                    PostUpdateCommands.RemoveComponent<Translation>(entities[i]);
                    PostUpdateCommands.RemoveComponent<Rotation>(entities[i]);
                    pos += rotQ * new Vector3(0, 0, straightLen);
                }
                else 
                {
                    entities[i] = EntityManager.Instantiate(hiway.curvePrefab);
                    EntityManager.AddComponent(entities[i], typeof(HighwayPieceProperties));

                    hpp.isStraight = false;
                    hpp.length = HighwayConstants.CURVE_LANE0_RADIUS * Mathf.PI / 2;
                    hpp.startRotation = rot;
                    EntityManager.SetComponentData(entities[i], hpp);
                    Vector3 xformedPos = pos;
                    float4x4 xForm = float4x4.TRS(localToWorld.Position + (float3)pos,
                                    rotQ,
                                    new float3(1, 1, 1));
                    PostUpdateCommands.SetComponent(entities[i], new LocalToWorld
                    {
                        Value = xForm
                    });
                    PostUpdateCommands.RemoveComponent<Translation>(entities[i]);
                    PostUpdateCommands.RemoveComponent<Rotation>(entities[i]);

                    pos += rotQ * new Vector3(HighwayConstants.MID_RADIUS, 0, HighwayConstants.MID_RADIUS);
                    rot = Mathf.PI / 2 * (i / 2 + 1);
                    rotQ = Quaternion.Euler(0, rot * Mathf.Rad2Deg, 0);
                }
            }

            // Tag the highway component with a "deactivate" so we can still query it,
            // but not re-update again.
            EntityManager.AddComponent<DeactiveSpawner>(e);
            entities.Dispose();
        });
    }
}
