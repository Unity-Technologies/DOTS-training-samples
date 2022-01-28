using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BarVisualizerSystem : SystemBase
{
    const float k_Scale = 0.3f;
    static readonly float3 k_Up = new float3(0, 1, 0);
    static readonly float3 k_Left = new float3(-1, 0, 0);

    protected override void OnUpdate()
    {
        var transformationMatrices = GetComponentDataFromEntity<LocalToWorld>();
        Entities
            .WithNativeDisableContainerSafetyRestriction(transformationMatrices)
            .ForEach((in DynamicBuffer<Joint> joints,
                in DynamicBuffer<Connection> connections,
                in DynamicBuffer<Bar> bars) =>
            {
                for (int c = 0; c < connections.Length; ++c)
                {
                    var p1 = joints[connections[c].J1].Value;
                    var p2 = joints[connections[c].J2].Value;
                    // position
                    var translation = (p1 + p2) * 0.5f;

                    var delta = p2 - p1;
                    var deltaLength = math.length(delta); // scale.z

                    // rotation
                    var fwd = delta / deltaLength;
                    var q = quaternion.LookRotation(fwd, fwd.Equals(k_Up) ? k_Left : k_Up);

                    // set transformation matrix
                    var matrix = transformationMatrices[bars[c].Value];
                    matrix.Value.c3.x = translation.x;
                    matrix.Value.c3.y = translation.y;
                    matrix.Value.c3.z = translation.z;

                    var qw = q.value.w;
                    var qx = q.value.x;
                    var qy = q.value.y;
                    var qz = q.value.z;
                    var qxqx2 = 2f * qx * qx;
                    var qxqy2 = 2f * qx * qy;
                    var qxqz2 = 2f * qx * qz;
                    var qxqw2 = 2f * qx * qw;
                    var qyqy2 = 2f * qy * qy;
                    var qyqz2 = 2f * qy * qz;
                    var qyqw2 = 2f * qy * qw;
                    var qzqz2 = 2f * qz * qz;
                    var qzqw2 = 2f * qz * qw;
                    matrix.Value.c0.x = (1 - qyqy2 - qzqz2) * k_Scale;
                    matrix.Value.c0.y = (qxqy2 + qzqw2) * k_Scale;
                    matrix.Value.c0.z = (qxqz2 - qyqw2) * k_Scale;
                    matrix.Value.c1.x = (qxqy2 - qzqw2) * k_Scale;
                    matrix.Value.c1.y = (1 - qxqx2 - qzqz2) * k_Scale;
                    matrix.Value.c1.z = (qyqz2 + qxqw2) * k_Scale;
                    matrix.Value.c2.x = (qxqz2 + qyqw2) * deltaLength;
                    matrix.Value.c2.y = (qyqz2 - qxqw2) * deltaLength;
                    matrix.Value.c2.z = (1 - qxqx2 - qyqy2) * deltaLength;
                    transformationMatrices[bars[c].Value] = matrix;
                }
            }).ScheduleParallel();
    }
}
