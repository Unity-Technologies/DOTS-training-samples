using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Profiling;
using static Unity.Mathematics.math;

public class Sys_Bee_ChaseTarget : JobComponentSystem
{

    struct Sys_Bee_ChaseEnemyJob : IJobChunk
    {

        [ReadOnly] public float dt;
        [ReadOnly] public float ChaseForce;
        [ReadOnly] public float AttackForce;
        [ReadOnly] public float AttackDistance;
        [ReadOnly] public float HitDistance;
        [ReadOnly] public float BeeTimeToDeath;
        [ReadOnly] public ComponentDataFromEntity<Translation> TranslationData;
        [ReadOnly] public ComponentDataFromEntity<Tag_IsDying> DyingType;

        [NativeDisableContainerSafetyRestriction]public ArchetypeChunkComponentType<C_Velocity> VelocityType;
        [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;
        [ReadOnly] public ArchetypeChunkComponentType<C_Target> TargetType;
        [ReadOnly] public ArchetypeChunkEntityType EntityType;

        [ReadOnly] public Entity BloodPrefab;

        public EntityCommandBuffer.Concurrent ecb;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var Velocities = chunk.GetNativeArray(VelocityType);
            var Translations = chunk.GetNativeArray(TranslationType);
            var Targets = chunk.GetNativeArray(TargetType);
            var Entities = chunk.GetNativeArray(EntityType);

            for (int i = 0; i < chunk.Count; ++i)
            {
                var Target = Targets[i];

                if (DyingType.Exists(Target.Value))//This code reads: "If Target is dead, stop following it"
                {
                    ecb.RemoveComponent(chunkIndex, Entities[i], typeof(C_Target));
                    continue;
                }
                
                var translation = Translations[i];
                var velocity = Velocities[i];

                float3 targetPos = TranslationData[Target.Value].Value;

                float3 delta = targetPos - translation.Value;
                float distanceSquared = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;

                if (distanceSquared < HitDistance * HitDistance)
                {
                    C_DeathTimer timer = new C_DeathTimer()
                    {
                        TimeRemaining = BeeTimeToDeath
                    };

                    Tag_IsDying tag;
                    ecb.AddComponent(chunkIndex, Target.Value, tag);
                    ecb.SetComponent(chunkIndex, Target.Value, timer);
                    ecb.RemoveComponent(chunkIndex, Entities[i], typeof(C_Target));

                    //Spawn particles;
                    var bloodVel = new C_Velocity()
                    {
                        Value = velocity.Value * .35f
                    };

                    for (int particle = 0; particle < 6; ++particle)
                    {
                        var blood = ecb.Instantiate(chunkIndex, BloodPrefab);
                        ecb.SetComponent(chunkIndex, blood, translation);
                        ecb.SetComponent(chunkIndex, blood, bloodVel);
                    }

                    continue;
                }

                float force = max(1.0, distanceSquared) < AttackDistance * AttackDistance ? AttackForce : ChaseForce;

                velocity.Value += delta * (force * dt / sqrt(distanceSquared));
                Velocities[i] = velocity;
                
            }
        }
        
    }

    struct Sys_Bee_ChaseResourceJob : IJobChunk
    {
        [ReadOnly] public float dt;
        [ReadOnly] public float ChaseForce;
        [ReadOnly] public float GrabDistance;
        [ReadOnly] public ComponentDataFromEntity<Translation> TranslationData;
        [ReadOnly] public ComponentDataFromEntity<Tag_IsHeld> HeldType;

        [NativeDisableContainerSafetyRestriction]public ArchetypeChunkComponentType<C_Velocity> VelocityType;
        [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;
        [ReadOnly] public ArchetypeChunkComponentType<C_Target> TargetType;
        [ReadOnly] public ArchetypeChunkEntityType EntityType;

        [ReadOnly] public int2 GridCounts;
        private int GridIndex(int x, int y)
        {
            return GridCounts.x * y + x;
        }

        [ReadOnly] public NativeArray<int> StackHeights;
        [ReadOnly] public ComponentDataFromEntity<C_Stack> StackType;
        [ReadOnly] public ComponentDataFromEntity<C_GridIndex> GridType;

        public EntityCommandBuffer.Concurrent ecb;
        
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var Velocities = chunk.GetNativeArray(VelocityType);
            var Translations = chunk.GetNativeArray(TranslationType);
            var Targets = chunk.GetNativeArray(TargetType);
            var Entities = chunk.GetNativeArray(EntityType);

            for (int i = 0; i < chunk.Count; ++i)
            {
                var Target = Targets[i];

                if (HeldType.Exists(Target.Value))
                {
                    ecb.RemoveComponent(chunkIndex, Entities[i], typeof(C_Target));
                    continue;
                }
                
                var translation = Translations[i];
                var velocity = Velocities[i];

                var targetPos = TranslationData[Target.Value].Value;

                var delta = targetPos - translation.Value;
                var distanceSquared = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
                
                var grid = GridType[Target.Value];

                if (StackType[Target.Value].index < StackHeights[GridIndex(grid.x, grid.y)] - 1)
                {
                    //Not top of stack anymore.
                    ecb.RemoveComponent(chunkIndex, Entities[i], typeof(C_Target));
                    continue;
                }

                if (distanceSquared < GrabDistance * GrabDistance)
                {
                    C_Held held;
                    held.Holder = Entities[i];

                    Tag_IsHeld tag;
                    ecb.SetComponent(chunkIndex, Target.Value, held);
                    ecb.AddComponent(chunkIndex, Target.Value, tag);
                    ecb.RemoveComponent(chunkIndex, Entities[i], typeof(C_Target));
                    C_Holding hold;
                    hold.ent = Target.Value;
                    ecb.AddComponent(chunkIndex, Entities[i], hold);
                    continue;
                }

                velocity.Value += delta * (ChaseForce * dt / sqrt(distanceSquared));
                Velocities[i] = velocity;
            }
        }
    }

    private EntityCommandBufferSystem m_EntityCommandBufferSystem;

    private EntityQuery _enemyChase;
    private EntityQuery _resourceChase;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();

        var entityQueryDesc = new EntityQueryDesc()
        {
            None = new ComponentType[] { ComponentType.ReadOnly<Tag_IsDying>(), ComponentType.ReadOnly<Tag_IsDead>() },
            All = new ComponentType[]
            {
                ComponentType.ReadOnly<Tag_Bee>(),
                typeof(C_Velocity), ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<C_Target>(),
                ComponentType.ReadOnly<C_TargetType>()
            }
        };

        _enemyChase = GetEntityQuery(entityQueryDesc);
        _resourceChase = GetEntityQuery(entityQueryDesc);

    }
    
    private ProfilerMarker _schedMarker = new ProfilerMarker("Scheduling");
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var enemyJob = new Sys_Bee_ChaseEnemyJob()
        {
            dt = UnityEngine.Time.deltaTime,
            ChaseForce = BeeManager.S.ChaseForce,
            AttackForce = BeeManager.S.AttackForce,
            AttackDistance = BeeManager.S.AttackDistance,
            HitDistance = BeeManager.S.HitDistance,
            BeeTimeToDeath = BeeManager.S.BeeTimeToDeath,
            TranslationData = GetComponentDataFromEntity<Translation>(true),
            VelocityType = GetArchetypeChunkComponentType<C_Velocity>(false),
            TranslationType = GetArchetypeChunkComponentType<Translation>(true),
            DyingType = GetComponentDataFromEntity<Tag_IsDying>(true),
            TargetType = GetArchetypeChunkComponentType<C_Target>(true),
            ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            EntityType = GetArchetypeChunkEntityType(),

            BloodPrefab = GameConstants.S.BloodEnt
        };

        _enemyChase.SetFilter( new C_TargetType{Type = TargetTypes.Enemy});
            
        _schedMarker.Begin();
        var enemyHandle = enemyJob.Schedule(_enemyChase, inputDependencies);
        _schedMarker.End();
        
        var resourceJob = new Sys_Bee_ChaseResourceJob()
        {
            dt = UnityEngine.Time.deltaTime,
            ChaseForce = BeeManager.S.ChaseForce,
            TranslationData = GetComponentDataFromEntity<Translation>(true),
            VelocityType = GetArchetypeChunkComponentType<C_Velocity>(false),
            TranslationType = GetArchetypeChunkComponentType<Translation>(true),
            HeldType = GetComponentDataFromEntity<Tag_IsHeld>(true),
            TargetType = GetArchetypeChunkComponentType<C_Target>(true),
            ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            EntityType = GetArchetypeChunkEntityType(),
            GrabDistance = BeeManager.S.GrabDistance,

            GridCounts = ResourceManager.S.GridCounts,
            StackHeights = ResourceManager.S.StackHeights,
            StackType = GetComponentDataFromEntity<C_Stack>(true),
            GridType = GetComponentDataFromEntity<C_GridIndex>(true)
        };
        
        
        _resourceChase.SetFilter(new C_TargetType{Type = TargetTypes.Resource});

        _schedMarker.Begin();
        var resourceHandle = resourceJob.Schedule(_resourceChase, inputDependencies);
        _schedMarker.End();
        var handle = JobHandle.CombineDependencies(enemyHandle, resourceHandle);
        
        m_EntityCommandBufferSystem.AddJobHandleForProducer(handle);
        return handle;
    }
}
 