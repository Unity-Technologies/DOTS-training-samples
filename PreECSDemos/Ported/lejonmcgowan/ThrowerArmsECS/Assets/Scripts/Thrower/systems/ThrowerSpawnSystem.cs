// using System.Linq;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Mathematics;
// using Unity.Transforms;
//
// [UpdateInGroup(typeof(InitializationSystemGroup))]
// public class ThrowerSpawnSystem: JobComponentSystem
// {
//     private BeginInitializationEntityCommandBufferSystem m_spawnerECB;
//
//     protected override void OnCreate()
//     {
//         m_spawnerECB = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
//     }
//     
//     private static Entity MakeEntityHelper(NativeArray<float3> chain, float thickness, 
//         float3 up, Entity prefab, EntityCommandBuffer.Concurrent ecb, int entityQueryIndex, int index)
//     {
//         
//         float3 delta = chain[index + 1] - chain[index];
//         Entity entity = ecb.Instantiate(entityQueryIndex, prefab);
//         NonUniformScale scale = new NonUniformScale()
//         {
//             Value = new float3(thickness, thickness, math.length(delta))
//         };
//         Translation translation = new Translation()
//         {
//             Value = chain[index] + delta * .5f
//         };
//         Rotation rotation = new Rotation()
//         {
//             Value = quaternion.LookRotation(math.normalize(delta), up)
//         };
//
//         ecb.SetComponent(entityQueryIndex, entity, translation);
//         ecb.SetComponent(entityQueryIndex, entity, rotation);
//         ecb.AddComponent(entityQueryIndex, entity, scale);
//         return entity;
//     }
//     
//     private static DynamicBuffer<ChildComponentData> CreateEntitiesBurst(NativeArray<float3> chain, float thickness, float3 up, float3 anchorRight,Entity prefab, 
//         EntityCommandBuffer.Concurrent ecb,int entityQueryIndex, DynamicBuffer<ChildComponentData>? parentBuffer, bool isArm = false)
//     {
//         var firstEntity = MakeEntityHelper(chain, thickness, up, prefab, ecb, entityQueryIndex,0);
//
//         if (isArm)
//         {
//             ArmBaseComponentData baseData = new ArmBaseComponentData();
//             baseData.anchorPosition = new Translation
//             {
//                 Value = chain[0]
//             };
//             baseData.target = new Translation()
//             {
//                 Value = baseData.anchorPosition.Value + new float3(0, 0, 5f)
//             };
//             baseData.anchorRight = anchorRight;
//             baseData.reach = 10;
//             ecb.AddComponent(entityQueryIndex, firstEntity,baseData);
//             ecb.AddComponent<ArmTag>(entityQueryIndex,firstEntity);
//         }
//         else
//         {
//             FingerBaseComponentData baseData = new FingerBaseComponentData();
//             ecb.AddComponent(entityQueryIndex,firstEntity,baseData);
//         }
//         
//         ecb.AddComponent<ArmFingerComponentData>(entityQueryIndex,firstEntity);
//
//         parentBuffer?.Add(firstEntity);
//         
//         Entity parentEntity = firstEntity;
//         DynamicBuffer<ChildComponentData> lastBuffer = new DynamicBuffer<ChildComponentData>();
//         // find the rendering matrices for an IK chain
//         // (each pair of neighboring points is connected by a beam)
//         for (int i = 1; i < chain.Length - 1; i++)
//         {
//             var entity = MakeEntityHelper(chain, thickness, up, prefab, ecb, entityQueryIndex, i);
//
//             var buffer = ecb.AddBuffer<ChildComponentData>(entityQueryIndex, parentEntity);
//             ecb.AddComponent<ArmFingerComponentData>(entityQueryIndex,entity);
//             
//             buffer.Add(entity);
//             
//             if(isArm)
//                 ecb.AddComponent<ArmTag>(entityQueryIndex,entity);
//             
//             parentEntity = entity;
//             lastBuffer = buffer;
//         }
//         
//         if(isArm)
//             lastBuffer = ecb.AddBuffer<ChildComponentData>(entityQueryIndex, parentEntity);
//         
//         
//         return lastBuffer;
//     }
//
//    
//
//     protected override JobHandle OnUpdate(JobHandle inputDeps)
//     {
//         var commandBuffer = m_spawnerECB.CreateCommandBuffer().ToConcurrent();
//
//         var jobHandle = Entities
//             .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
//             .ForEach((Entity entity, int entityInQueryIndex, in MeshSpawnComponentData spawnComponentData) =>
//             {
//                 //spawn arm render entities here
//                 for (int armIter = 0; armIter < spawnComponentData.numToSpawn; armIter++)
//                 {
//                     float3 offset = new float3(spawnComponentData.numToSpawn / 2.0f - armIter, 0, 0);
//                     
//                     //spawn arms here
//                     var armChain = new NativeArray<float3>(3,Allocator.Temp);
//                     var spawnPosition = spawnComponentData.position.Value - offset;
//                     float3 target = spawnPosition + new float3(0, 1, 1.5f);
//                     FABIK.Solve(armChain, 1f, spawnPosition, target, float3.zero);
//                     
//                     float3 handForward = math.normalize(armChain[armChain.Length - 1] - armChain[armChain.Length - 2]);
//                     float3 handUp = math.normalize(math.cross(handForward, spawnComponentData.right));
//                     float3 handRight = math.cross(handUp, handForward);
//
//                     var lastEntityBuffer = CreateEntitiesBurst(armChain, 0.15f, handUp, spawnComponentData.right,
//                         spawnComponentData.prefab,commandBuffer,entityInQueryIndex,null,true);
//                     
//                     //spawn fingers
//                     for (int fingerIndex = 0; fingerIndex < 4; fingerIndex++)
//                     {
//                         float3 armPos = armChain[armChain.Length - 1];
//
//                         float3 fingerPos = armPos + handRight * (-0.12f + fingerIndex * 0.08f);
//                         float3 fingerTtarget = fingerPos + handForward * 0.5f;
//                         var fingerChain = new NativeArray<float3>(4,Allocator.Temp);
//
//                         FABIK.Solve(fingerChain, 0.22f, fingerPos, fingerTtarget, 0.2f * handUp);
//
//                         CreateEntitiesBurst(fingerChain, 0.05f, handUp, spawnComponentData.right,spawnComponentData.prefab,
//                             commandBuffer,entityInQueryIndex,lastEntityBuffer,false);
//                     }
//                 }
//                 
//                 //done, only need to do this once and delete the spawner
//                 commandBuffer.DestroyEntity(entityInQueryIndex,entity);
//             }).Schedule(inputDeps);
//         
//         m_spawnerECB.AddJobHandleForProducer(jobHandle);
//         
//         return jobHandle;
//     }
// }
