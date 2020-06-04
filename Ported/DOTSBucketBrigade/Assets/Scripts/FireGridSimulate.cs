using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FireGridSimulate : SystemBase
{

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BucketBrigadeConfig>();
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<BucketBrigadeConfig>();
        int2 dimensions = config.GridDimensions;
        float transfer = config.TemperatureIncreaseRate;
        float flashpoint = config.Flashpoint;
        int searchSize = config.HeatRadius;
        int size = dimensions.x * dimensions.y;

        float delta = Time.DeltaTime;

        Entities.ForEach((ref DynamicBuffer<FireGridCell> cells) =>
        {
            for (int i = 0; i < size; i++)
            {
                FireGridCell cell = cells[i];

                int cellRowIndex = i / dimensions.x;
                int cellColumnIndex = i % dimensions.x;

                float change = 0;

                for (int y = cellRowIndex - searchSize; y <= cellRowIndex + searchSize; y++)
                {
                    if (y >= 0 && y < dimensions.y)
                    {
                        for (int x = cellColumnIndex - searchSize; x <= cellColumnIndex + searchSize; x++)
                        {
                            if (x >= 0 && x < dimensions.x)
                            {
                                FireGridCell neighbourCell = cells[y * dimensions.x + x];

                                if (neighbourCell.Temperature > flashpoint)
                                {
                                    change += neighbourCell.Temperature * transfer * delta;
                                }
                            }
                        }
                    }
                }

                cell.Temperature = math.clamp(cell.Temperature + change, -1.0f, 1.0f);
                cells[i] = cell;
            }
        }
        ).ScheduleParallel();
    }
}
