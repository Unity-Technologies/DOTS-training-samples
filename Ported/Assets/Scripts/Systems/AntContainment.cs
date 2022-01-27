using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/**
 * This system compute keeps all ants in the defined world
 */
[UpdateBefore(typeof(SteeringSystem))]
public partial class AntContainment : SystemBase
{
    private float m_MapSize = 128.0f;

    protected override void OnStartRunning()
    {
        var configurationEntity = GetSingletonEntity<Configuration>();
        var config = GetComponent<Configuration>(configurationEntity);
        m_MapSize = 0.49f;
    }

    protected override void OnUpdate()
    {
        //float mapSize = m_MapSize;
        //Entities
        //    .ForEach((ref MapContainmentSteering mapContainmentSteering, in Translation translation) =>
        //    {
        //        if ((translation.Value.x > mapSize) || (translation.Value.x < -mapSize) || (translation.Value.y > mapSize) || (translation.Value.y < -mapSize))
        //        {
        //            mapContainmentSteering.Value = math.normalize(-translation.Value.xy);
        //        }
        //        else
        //        {
        //            mapContainmentSteering.Value = float2.zero;
        //        }

        //    }).ScheduleParallel();
    }
}