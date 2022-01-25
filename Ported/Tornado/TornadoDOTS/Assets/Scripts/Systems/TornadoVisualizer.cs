using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class TornadoVisualizer : SystemBase
{
    static readonly float3 k_Center = float3.zero;
    const float k_MAXDistanceSqrd = 10;
    const float k_Strength = 1000;
    
    protected override void OnUpdate()
    {
        Entities.
            ForEach((ref Translation position,
                in TornadoParticle particle) =>
            {
                float3 diff = k_Center - position.Value;
                float distSqrd = math.lengthsq(diff);
                if (distSqrd < k_MAXDistanceSqrd)
                {
                    var calculus = k_Strength * (diff / math.sqrt(distSqrd));
                    position.Value += calculus;
                }
            }).ScheduleParallel();
        
        // Entities.WithAll<TornadoParticle>()
        //     .ForEach((ref Translation position) =>
        //     {
        //         float3 diff = center - position.Value;
        //         float distSqrd = math.lengthsq(diff);
        //         if (distSqrd < maxDistanceSqrd)
        //         {
        //             var calculus = strength * (diff / math.sqrt(distSqrd));
        //             position.Value += calculus;
        //         }
        //     }).ScheduleParallel();
    }
}
