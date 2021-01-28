using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

public class BloodSplatSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        
        Entities
            .WithAll<BloodTag>()
            .WithNone<GroundedTime>()
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity e, in PhysicsData data) =>
            {
                EntityManager.AddComponentData(e, new GroundedTime
                {
                    Time = time,
                });
            }).Run();
            
        Entities
            .WithAll<BloodTag>()
            .ForEach((ref URPMaterialPropertySmoothness smoothness, in GroundedTime groundedTime) =>
            {
                smoothness.Value = (float) (time - groundedTime.Time);
            }).Run();
    }
}