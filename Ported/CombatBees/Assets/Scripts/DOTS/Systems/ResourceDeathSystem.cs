//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Transforms;

//public class ResourceDeathSystem : SystemBase
//{
//    private EntityQuery m_MainFieldQuery;
//    private EntityQuery m_TeamFieldsQuery;
//    private EntityCommandBufferSystem m_ECBSystem;

//    protected override void OnCreate()
//    {
//        m_MainFieldQuery = GetEntityQuery(new EntityQueryDesc
//        {
//            All = new[]
//            {
//                ComponentType.ReadOnly<FieldInfo>()
//            },
//            None = new[]
//            {
//                ComponentType.ReadOnly<TeamOne>(),
//                ComponentType.ReadOnly<TeamTwo>()
//            }
//        });

//        m_TeamFieldsQuery = GetEntityQuery(new EntityQueryDesc
//        {
//            All = new[]
//            {
//                ComponentType.ReadOnly<FieldInfo>()
//            },
//            Any = new[]
//            {
//                ComponentType.ReadOnly<TeamOne>(),
//                ComponentType.ReadOnly<TeamTwo>(),
//            }
//        });

//        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
//    }

//    protected override void OnUpdate()
//    {
//    //    var mainField = m_MainFieldQuery.ToComponentDataArrayAsync<FieldInfo>(Unity.Collections.Allocator.TempJob, out var mainFieldHandle);

//    //    Dependency = JobHandle.CombineDependencies(Dependency, mainFieldHandle);

//    //    var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

//    //    Entities.WithAll<Gravity>()
//    //        .WithDeallocateOnJobCompletion(mainField)
//    //        .WithNone<Dead>()
//    //        .ForEach((int entityInQueryIndex, Entity resourceEntity, ref Velocity v, in Translation t) =>
//    //        {
//    //            // Now check main field to see if resource should stop falling
//    //            for (int j = 0; j < mainField.Length; ++j)
//    //            {
//    //                Bounds bound = mainField[j].Bounds;

//    //                if (t.Value.y <= bound.Floor)
//    //                    v.Value = new Unity.Mathematics.float3(0, 0, 0);
//    //            }
//    //        }).ScheduleParallel();

//    //    m_ECBSystem.AddJobHandleForProducer(Dependency);
//    }
//}
