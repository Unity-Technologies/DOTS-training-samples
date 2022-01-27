using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/**
 * This system compute keeps all ants in the defined world
 */
public partial class AntContainment : SystemBase
{
    private float m_MapSize = 128.0f;

    protected override void OnStartRunning()
    {
        var configurationEntity = GetSingletonEntity<Configuration>();
        var config = GetComponent<Configuration>(configurationEntity);
        m_MapSize = config.MapSize;
    }

    protected override void OnUpdate()
    {
        float mapSize = m_MapSize;
        Entities
            .ForEach((ref MapContainmentSteering mapContainmentSteering, in Translation translation) =>
            {
                if (translation.Value.x > mapSize)
                {
                    mapContainmentSteering.Value.x = -1.0f;
                }
                else if (translation.Value.x < -mapSize)
                {
                    mapContainmentSteering.Value.x = 1.0f;
                }
                else
                {
                    mapContainmentSteering.Value.x = 0.0f;
                }
                if (translation.Value.y > mapSize)
                {
                    mapContainmentSteering.Value.y = -1.0f;
                }
                else if (translation.Value.y < -mapSize)
                {
                    mapContainmentSteering.Value.y = 1.0f;
                }
                else
                {
                    mapContainmentSteering.Value.y = 0.0f;
                }
            }).Schedule();
    }
}