using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Dots
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class TornadoSpawnerSystem : SystemBase
    {
        private EntityQuery m_TornadoSpawnerQuery;

        static readonly ComponentType k_SimulatedComponent = ComponentType.ReadOnly(typeof(Dots.Simulated));

        protected override void OnCreate()
        {
            m_TornadoSpawnerQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(TornadoSpawner) }
            });
            RequireForUpdate(m_TornadoSpawnerQuery);
        }

        protected override void OnUpdate()
        {
            var random = new Random(1234);

            using (var entities = m_TornadoSpawnerQuery.ToEntityArray(Allocator.TempJob))
            {
                foreach (Entity entity in entities)
                {
                    var spawner = EntityManager.GetComponentObject<TornadoSpawner>(entity);
                    var position = EntityManager.GetComponentData<Translation>(entity);
                    EntityManager.DestroyEntity(entity);

                    var tornadoEntity = EntityManager.CreateEntity();
                    var tornadoPos = new float3(position.Value.x, position.Value.y, position.Value.z);
                    EntityManager.AddComponentData(tornadoEntity, new Tornado()
                    {
                        initialPosition = tornadoPos,
                        position = tornadoPos,
                        force = spawner.force,
                        height = spawner.height,
                        inwardForce = spawner.inwardForce,
                        maxForceDist = spawner.maxForceDist,
                        spinRate = spawner.spinRate,
                        upForce = spawner.upForce,
                        upwardSpeed = spawner.upwardSpeed,
                        rotationModulation = random.NextFloat(-1f, 1f) * spawner.maxForceDist
                    });
                    if (spawner.simulate)
                        EntityManager.AddComponent(tornadoEntity, k_SimulatedComponent);

                    for (var i = 0; i < spawner.debrisCount; ++i)
                    {
                        var debrisEntity = EntityManager.CreateEntity();
                        EntityManager.AddComponentData(debrisEntity, new Debris()
                        {
                            tornado = tornadoEntity,
                            radiusMult = random.NextFloat()
                        });
                        if (spawner.simulate)
                            EntityManager.AddComponent(debrisEntity, k_SimulatedComponent);

                        var debrisPosition = new Vector3(position.Value.x + random.NextFloat(-spawner.initRange, spawner.initRange), position.Value.y + random.NextFloat(0f, spawner.height), position.Value.z + random.NextFloat(-spawner.initRange, spawner.initRange));
                        var rotation = Quaternion.identity;
                        var scale = Vector3.one * random.NextFloat(.2f, .7f);
                        var color = Color.white * random.NextFloat(.3f, .7f);

                        EntityManager.AddComponentData(debrisEntity, new Translation
                        {
                            Value = debrisPosition
                        });
                        EntityManager.AddComponentData(debrisEntity, new Rotation()
                        {
                            Value = rotation
                        });
                        EntityManager.AddComponentData(debrisEntity, new NonUniformScale()
                        {
                            Value = scale
                        });

                        var desc = new RenderMeshDescription(spawner.mesh, spawner.material);
                        RenderMeshUtility.AddComponents(debrisEntity, EntityManager, desc);
                    }
                }
            }
        }
    }
}