using Unity.Entities;

 // Ensures that it will be executed before the "MouseRaySystem"

namespace Combatbees.Testing.Mahmoud
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class MouseRayInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonSpawner>();
        }

        protected override void OnUpdate()
        {
            Enabled = false; // Run only once

            // Create a sphere entity here?
        }
    }
}