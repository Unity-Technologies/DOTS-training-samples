using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class DestroyForestSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var gameTime = GetSingleton<GameTime>();
        float deltaTime = gameTime.DeltaTime;
        
        float choppingDelay = 2f;
        
        Entities.
            ForEach((ref DestroyForest destroyForest, ref NonUniformScale forestScale) =>
            {
                destroyForest.Completion = destroyForest.Completion + deltaTime / choppingDelay;
                float3 scale = (1f - destroyForest.Completion) * destroyForest.OriginalScale;
                scale.x = math.floor(25f * scale.x) / 25f;
                scale.y = math.floor(25f * scale.y) / 25f;
                scale.z = math.floor(25f * scale.z) / 25f;
                forestScale.Value = scale;
            }).ScheduleParallel();
    }
}