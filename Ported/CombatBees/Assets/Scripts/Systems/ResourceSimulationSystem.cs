using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

class ResourceSimulationSystem: SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        const float gravity = 2.0f;
        Entities
            .ForEach((Entity entity, ref Resource resource, ref NewTranslation pos) =>
            {
                if(HasComponent<Translation>(resource.CarryingBee))
                {
                    var beePos = GetComponent<Translation>(resource.CarryingBee);
                    pos.translation.Value = beePos.Value - new float3(0.0f, 0.01f, 0.0f);
                }
                else
                    if(pos.translation.Value.y>0.0f)
                    {
                        pos.translation.Value.y += resource.Speed * deltaTime;
                        resource.Speed -= gravity * deltaTime;
                    }
            }).ScheduleParallel();
    }
}
