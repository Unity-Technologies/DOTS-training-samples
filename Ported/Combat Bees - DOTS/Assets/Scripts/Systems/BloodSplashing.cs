using Unity.Entities;
using Unity.Transforms;

public partial class BloodSplashing : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SingletonMainScene>();
    }
    
    protected override void OnUpdate()
    {
        float floorHeight = GetSingleton<Container>().MinPosition.y;

        Entities.WithAll<BloodTag>().ForEach(
            (ref NonUniformScale nonUniformScale,ref RandomState randomState, in Translation translation) =>
            {
                if (translation.Value.y <= floorHeight)
                {
                    // BUG: Random generates always the same number
                    float bloodSplashRadius = randomState.Value.NextFloat(nonUniformScale.Value.x, 1f);
                    nonUniformScale.Value.x = bloodSplashRadius;
                    nonUniformScale.Value.z = bloodSplashRadius;
                }
            }).ScheduleParallel();
    }
}
