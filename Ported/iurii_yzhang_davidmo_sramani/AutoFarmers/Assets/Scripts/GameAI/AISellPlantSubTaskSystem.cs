using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameAI
{
    public class AISellPlantkSubTaskSystem : JobComponentSystem
    {
        BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

        protected override void OnCreate()
        {
            m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var ecb1 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var ecb2 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var ecb3 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var ecb4 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            
            var job = Entities
                .WithAll<AITagTaskDeliver>()
                .WithNone<AISubTaskTagFindPlant>()
                .WithNone<AISubTaskTagFindShop>()
                .WithNone<AISubTaskSellPlant>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb1.AddComponent<AISubTaskTagFindPlant>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);

            var hashMap = World.GetOrCreateSystem<WorldCreatorSystem>().hashMap;
            var health = GetComponentDataFromEntity<HealthComponent>(true);

            var job2 = Entities
                .WithAll<AITagTaskDeliver>()
                .WithAll<AISubTaskTagFindPlant>()
                .WithReadOnly(hashMap)
                .WithReadOnly(health)
//                .WithoutBurst()
                .ForEach((Entity entity, int entityInQueryIndex, in AISubTaskTagComplete target) =>
                {
                    Entity plantEntity;
                    var has = hashMap.TryGetValue(target.targetPos, out plantEntity);
//                    Debug.Log($"Found plant {plantEntity} at {target.targetPos}");
                    if (health.Exists(plantEntity)) {
//                        Debug.Log($"Removing plant {plantEntity}");
                        ecb2.SetComponent(entityInQueryIndex, plantEntity, new HealthComponent() {Value = 0});
                        ecb2.RemoveComponent<TagFullyGrownPlant>(entityInQueryIndex, plantEntity);
                    }
                    ecb2.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb2.RemoveComponent<AISubTaskTagFindPlant>(entityInQueryIndex, entity);
                    ecb2.AddComponent(entityInQueryIndex, entity, new AISubTaskTagFindShop() {plantEntity = plantEntity});
                }).Schedule(inputDeps);
            

            var job3 = Entities
                .WithAll<AITagTaskDeliver>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagFindShop>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb3.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb3.RemoveComponent<AISubTaskTagFindShop>(entityInQueryIndex, entity);
                    ecb3.AddComponent<AISubTaskSellPlant>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);
            
            var job4 = Entities
                .WithAll<AITagTaskDeliver>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskSellPlant>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb4.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb4.RemoveComponent<AISubTaskSellPlant>(entityInQueryIndex, entity);
                    ecb4.RemoveComponent<AITagTaskDeliver>(entityInQueryIndex, entity);
                    ecb4.AddComponent<AITagTaskNone>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);
            
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job3);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job4);

            return inputDeps;
        }
    }
}