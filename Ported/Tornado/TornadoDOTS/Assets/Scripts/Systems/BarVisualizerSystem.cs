using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Rendering;

public partial class BarVisualizerSystem : SystemBase
{
    //TODO: Extract to math constants class
    static readonly float3 k_Up = new float3(0, 1, 0);
    static readonly float3 k_Left = new float3(-1, 0, 0);
    
    protected override void OnUpdate()
    {
        //TODO (safety): Is there a better way to interact with two queries like this.
        //What if bars had a reference back to it's cluster and connection index?
        var translations = GetComponentDataFromEntity<Translation>();
        var rotations = GetComponentDataFromEntity<Rotation>();
        var scales = GetComponentDataFromEntity<NonUniformScale>();
        Entities
            .WithNativeDisableContainerSafetyRestriction(translations)
            .WithNativeDisableContainerSafetyRestriction(rotations)
            .WithNativeDisableContainerSafetyRestriction(scales)
            .ForEach((in DynamicBuffer<Joint> joints,
                in DynamicBuffer<Connection> connections, 
                in DynamicBuffer<Bar> bars) =>
            {
                for (int c = 0; c < connections.Length; ++c)
                {
                    var p1 = joints[connections[c].J1].Value;
                    var p2 = joints[connections[c].J2].Value;
                    // position
                    translations[bars[c].Value] = new Translation
                    {
                        Value = (p1 + p2) * 0.5f
                    };
                    
                    var delta = p2 - p1;
                    var deltaLength = math.length(delta);
                    
                    // rotation
                    var fwd = delta / deltaLength;
                    var rotation = rotations[bars[c].Value];
                    rotation.Value = quaternion.LookRotation(fwd, fwd.Equals(k_Up) ? k_Left : k_Up);
                    rotations[bars[c].Value] = rotation;
                    
                    // scale
                    var scale = scales[bars[c].Value];
                    scale.Value.z = deltaLength;
                    scales[bars[c].Value] = scale;
                }
            }).ScheduleParallel();
    }
}