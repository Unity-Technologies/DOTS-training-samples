using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
namespace CombatBees.Testing.BeeFlight
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class BufferInitializationSystem : SystemBase
    {
        protected override void OnCreate()
        { 
            RequireSingletonForUpdate<SingeltonBeeMovement>();
            RequireSingletonForUpdate<BufferSingelton>();
        }
        protected override void OnUpdate()
        {
            var pairBuffer = GetBuffer<BeeResourcePair>(GetSingletonEntity<BufferSingelton>());
            var resourceBuffer = GetBuffer<ResourceBuffer>(GetSingletonEntity<BufferSingelton>());
            
            // //initializing pair buffer 
            Entities.WithAll<Bee>().ForEach((Entity entity) =>
            {
                Enabled = false;
                pairBuffer.Add(new BeeResourcePair
                {   
                    ResourceEntity = Entity.Null,
                    BeeEntity = entity,
                    
                });
            
            }).WithoutBurst().Run();
            //
            // //initializing resource buffer 
            Entities.WithAll<Resource>().ForEach((Entity entity) =>
            {
                Enabled = false;
                resourceBuffer.Add(new ResourceBuffer()
                {   
                    Value = entity
                });
            
            }).WithoutBurst().Run();
        }
    }
}