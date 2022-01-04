using Combatbees.Testing.Elfi;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Combatbees.Testing.Elfi
{

    public partial class EGravitySystem : SystemBase
    {
        // will be moved to field component in future? 


        protected override void OnCreate()
        {
            RequireSingletonForUpdate<ESingletonSpawner>();
        }

        protected override void OnUpdate()
        {
            float gravity = -20f;
            float deltaTime = World.Time.DeltaTime;
            Entities.WithAll<EResourceTag>().ForEach((ref Translation translation, in Rotation rotation) =>
            {

                if (translation.Value.y > 2)
                {
                    translation.Value.y += gravity * deltaTime;
                }



            }).Schedule();
        }
    }
}