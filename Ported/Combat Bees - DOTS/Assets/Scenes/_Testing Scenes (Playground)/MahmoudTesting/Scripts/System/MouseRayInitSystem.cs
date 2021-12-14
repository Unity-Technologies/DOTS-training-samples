using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))] // Ensures that it will be executed before the "MouseRaySystem"
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
