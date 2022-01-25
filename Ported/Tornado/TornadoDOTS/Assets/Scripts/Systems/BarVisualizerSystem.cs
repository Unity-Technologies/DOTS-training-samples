using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BarVisualizerSystem : SystemBase
{
    //TODO: Extract to math constants class
    static readonly float3 k_Up = new float3(0, 1, 0);
    static readonly float3 k_Left = new float3(-1, 0, 0);
    
    protected override void OnUpdate()
    {
        var gcfe = GetComponentDataFromEntity<Translation>();
        Entities
            .WithNativeDisableContainerSafetyRestriction(gcfe)
            .ForEach((ref Translation translation, ref Rotation rotation, ref NonUniformScale scale,
                in BarConnection connection) =>
            {
                var joint1Pos = gcfe[connection.Joint1].Value;
                var joint2Pos = gcfe[connection.Joint2].Value;
                var delta = joint2Pos - joint1Pos;
        
                translation.Value = (joint1Pos + joint2Pos) * 0.5f;
                scale.Value.z = math.length(delta);
                
                var fwd = math.normalize(delta);
                var refAxis = fwd.Equals(k_Up) ? k_Left : k_Up;
                rotation.Value = quaternion.LookRotation(fwd, refAxis);
            }).ScheduleParallel();
    }
}