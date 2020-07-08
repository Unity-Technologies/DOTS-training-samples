using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateAfter(typeof(FireSpreadSystem))]
public class FireExtinguishSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    const float coolingStrength = 5.0f;
    const float bucketCapacity = 3.0f;
    const float coolingStrength_falloff = 1.0f;

    protected override void OnCreate()
    {
        GetEntityQuery(typeof(FireCell));
        m_CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // Grab the fire grid data
        var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        var fireGridSetting = GetComponent<FireGridSettings>(fireGridEntity);
        var fireGridBounds = GetComponent<Bounds>(fireGridEntity);
        var buffer = EntityManager.GetBuffer<FireCell>(fireGridEntity);

        // Compute the min and max of the grid
        float2 minGridPos = fireGridBounds.BoundsCenter - fireGridBounds.BoundsExtent * 0.5f;
        float2 maxGridPos = fireGridBounds.BoundsCenter + fireGridBounds.BoundsExtent * 0.5f;

        // Grab the command buffer
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();

        // Loop through all the water spill events
        Entities
        .ForEach((Entity entity, in WaterSpill spill) =>
        {
            // If we clicked outside, nothing to do
            if (math.any(minGridPos > spill.SpillPosition) || math.any(spill.SpillPosition > maxGridPos))
            {
                ecb.DestroyEntity(entity);
                return;
            }

            // Compute the coordinate of the event on the grid
            int2 coord = (int2)((spill.SpillPosition - minGridPos) / fireGridBounds.BoundsExtent * (float2)(fireGridSetting.FireGridResolution));

            // Compute the range of pixels that are affected
            int kernelRadius = 4;
            int2 rangeX = math.clamp(new int2(coord.x - kernelRadius, coord.x + kernelRadius), 0, (int)fireGridSetting.FireGridResolution.x - 1);
            int2 rangeY = math.clamp(new int2(coord.y - kernelRadius, coord.y + kernelRadius), 0, (int)fireGridSetting.FireGridResolution.y - 1);

            // Loop through the neighborhood
            for (int v = rangeY.x; v <= rangeY.y; ++v)
            {
                for (int u = rangeX.x; u <= rangeX.y; ++u)
                {
                    // Convert the coordinate into an index in the grid
                    int index = u + v * (int)fireGridSetting.FireGridResolution.x;

                    // Compute the distance from the center click position
                    float rowShift = math.abs(coord.x - u);
                    float columnShift = math.abs(coord.x - v);

                    // Change the fire temperature of the target cell
                    FireCell cell = buffer[index];
                    float dowseCellStrength = 1f / (math.abs(rowShift * coolingStrength_falloff) + math.abs(columnShift * coolingStrength_falloff));
                    cell.FireTemperature = math.max(cell.FireTemperature - (coolingStrength * dowseCellStrength) * bucketCapacity, 0);

                    // Propagate the info
                    buffer[index] = cell;
                }
            }
            ecb.DestroyEntity(entity);

        }).Schedule();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
