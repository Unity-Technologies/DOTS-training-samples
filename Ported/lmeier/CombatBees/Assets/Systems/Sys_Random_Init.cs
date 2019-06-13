using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class Sys_Random_Init : ComponentSystem
{
    protected override void OnUpdate()
    {
        var query = GetEntityQuery(typeof(C_Random), ComponentType.ReadOnly<Tag_Random_Init>());

        var entities = query.ToEntityArray(Allocator.TempJob);

        
        for(int i = 0; i < entities.Length; ++i)
        {
            C_Random rand = new C_Random()
            {
                Generator = new Random()
            };

            rand.Generator.InitState(BeeManager.S.Rand.NextUInt());

            EntityManager.SetComponentData(entities[i], rand);
            EntityManager.RemoveComponent<Tag_Random_Init>(entities[i]);
        }

        entities.Dispose();
    }
}