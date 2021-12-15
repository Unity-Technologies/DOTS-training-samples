using Combatbees.Testing.Mahmoud;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Combatbees.Testing.Mahmoud
{
    
    public partial class FallingResources : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonSpawner>();
        }

        protected override void OnUpdate()
        {



            Entities.WithAll<ResourceTag>().ForEach((ref Translation translation, in Rotation rotation) =>
                {

                    if (translation.Value.y > -10)
                    {
                        translation.Value.y--;
                    }
                    
                    
                    
                }).Schedule();
        }
    }
}