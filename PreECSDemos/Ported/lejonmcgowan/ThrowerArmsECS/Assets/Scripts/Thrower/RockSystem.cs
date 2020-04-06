using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(ArmSystem))]
public class RockSystem: SystemBase
{
    protected override void OnUpdate()
    {
        float t = (float)Time.ElapsedTime;
        float3 centerPos = new float3(0,0,1.5f);
        float orbitRadius = 0.75f;
        
        Entities.
            WithNone<DebugRockGrabbedTag>().
            ForEach((ref Translation pos, in RockReservedTag _) =>
        {
            float x = orbitRadius * math.cos(t);
            float z = orbitRadius * math.sin(t);
            
            pos.Value = new float3(x,pos.Value.y,z) + centerPos;
            
        }).ScheduleParallel();
    }
}
