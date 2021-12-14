using Unity.Entities;

namespace CombatBees.Testing.BeeFlight
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class BeeInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
        }
        
        protected override void OnUpdate()
        {
            Enabled = false; // Run only once

            // Entities.ForEach((ref BeeTargets beeTargets) =>
            // {
            //     beeTargets.CurrentTarget = beeTargets.LeftTarget; // Set initial target
            // }).ScheduleParallel();
        }
    }
}