using Unity.Entities;

[DisableAutoCreation]
public partial class BeeInitSystem : SystemBase
{
    protected override void OnUpdate()
    {
        this.Enabled = false; // Run only once

        Entities.ForEach((ref BeeTargets beeTargets) =>
        {
            beeTargets.CurrentTarget = beeTargets.LeftTarget; // Set initial target
        }).Run();
    }
}
