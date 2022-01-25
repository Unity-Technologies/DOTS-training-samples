using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class TornadoVisualizer : SystemBase
{
    private float3 center = float3.zero;
    private float maxDistanceSqrd = 10;
    private float strength = 1000;
    
    protected override void OnUpdate()
    {
        Entities.
            ForEach((ref Translation position,
                in TornadoParticle particle) =>
            {
                float3 diff = center - position.Value;
                float distSqrd = math.lengthsq(diff);
                if (distSqrd < maxDistanceSqrd)
                {
                    var calculus = strength * (diff / math.sqrt(distSqrd));
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
