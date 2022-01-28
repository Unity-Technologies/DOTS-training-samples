using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/**
 * This system compute keeps all ants in the defined world
 */
[UpdateBefore(typeof(SteeringSystem))]
public partial class AntContainment : SystemBase
{
    private float m_MapSize = 0.5f;
    private float m_MapSizeMargin = 0.01f;

    protected override void OnStartRunning()
    {
        m_MapSize = Configuration.MapSizeWorld;
    }

    protected override void OnUpdate()
    {
        float border = m_MapSize - m_MapSizeMargin;
        Entities
            .ForEach((ref MapContainmentSteering mapContainmentSteering, in Translation translation, in Velocity antVelocity) =>
            {
                if ((translation.Value.x > border) || (translation.Value.x < -border))
                {
                    mapContainmentSteering.Value = math.normalize(new float2(-translation.Value.x, 0.0f));
                }
                if ((translation.Value.y > border) || (translation.Value.y < -border))
                {
                    mapContainmentSteering.Value = math.normalize( new float2(0.0f, -translation.Value.y));
                }

            }).ScheduleParallel();
    }
}