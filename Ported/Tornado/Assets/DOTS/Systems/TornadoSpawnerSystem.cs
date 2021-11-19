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

        static readonly ComponentType k_SimulatedComponent = ComponentType.ReadOnly(typeof(Simulated));
        static readonly ComponentType k_TornadoComponent = ComponentType.ReadWrite<Tornado>();
        static readonly ComponentType k_DebrisComponent = ComponentType.ReadWrite<Debris>();

        EntityArchetype m_SimulatedTornadoArchetype;
        EntityArchetype m_TornadoArchetype;
        EntityArchetype m_SimulatedDebrisArchetype;
        EntityArchetype m_DebrisArchetype;

        protected override void OnCreate()
        {
            m_SimulatedTornadoArchetype = EntityManager.CreateArchetype(k_TornadoComponent, k_SimulatedComponent);
            m_TornadoArchetype = EntityManager.CreateArchetype(k_TornadoComponent);
            m_SimulatedDebrisArchetype = EntityManager.CreateArchetype(k_DebrisComponent, k_SimulatedComponent,
                ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadWrite<Rotation>(),
                ComponentType.ReadWrite<NonUniformScale>());
            m_DebrisArchetype = EntityManager.CreateArchetype(k_DebrisComponent,
                ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadWrite<Rotation>(),
                ComponentType.ReadWrite<NonUniformScale>());
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

                    Entity tornadoEntity;
                    if (spawner.simulate)
                        tornadoEntity = EntityManager.CreateEntity(m_SimulatedTornadoArchetype);
                    else
                        tornadoEntity = EntityManager.CreateEntity(m_TornadoArchetype);
                    var tornadoPos = new float3(position.Value.x, position.Value.y, position.Value.z);
                    EntityManager.SetComponentData(tornadoEntity, new Tornado()
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

                    for (var i = 0; i < spawner.debrisCount; ++i)
                    {
                        Entity debrisEntity;
                        if (spawner.simulate)
                            debrisEntity = EntityManager.CreateEntity(m_SimulatedDebrisArchetype);
                        else
                            debrisEntity = EntityManager.CreateEntity(m_DebrisArchetype);
                        EntityManager.SetComponentData(debrisEntity, new Debris()
                        {
                            tornado = tornadoEntity,
                            radiusMult = random.NextFloat()
                        });

                        var debrisPosition = new Vector3(position.Value.x + random.NextFloat(-spawner.initRange, spawner.initRange), position.Value.y + random.NextFloat(0f, spawner.height), position.Value.z + random.NextFloat(-spawner.initRange, spawner.initRange));
                        var rotation = Quaternion.identity;
                        var scale = Vector3.one * random.NextFloat(.2f, .7f);
                        var color = Color.white * random.NextFloat(.3f, .7f);

                        EntityManager.SetComponentData(debrisEntity, new Translation
                        {
                            Value = debrisPosition
                        });
                        EntityManager.SetComponentData(debrisEntity, new Rotation()
                        {
                            Value = rotation
                        });
                        EntityManager.SetComponentData(debrisEntity, new NonUniformScale()
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