using Unity.Entities;
using Unity.Rendering;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(PropagateFireSystem))]
public partial class SetupGameSystem : SystemBase
{
    [NotBurstCompatible]
    protected override void OnUpdate()
    {
        if (!TryGetSingletonEntity<SpawnMarker>(out Entity marker))
            return;

        EntityManager.DestroyEntity(marker);

        var gameConstants = GetSingleton<GameConstants>();

        var random = new Random(12345);

        // TODO: Spawn Lakes
        {
            // TODO: These should be spawned by conversion (remove this comment when that is done)
        }

        // TODO: Spawn Teams of Firefighters
        {

        }

        // TODO: Spawn Buckets
        {

        }

        // Fire
        {
            var fireField = EntityManager.CreateEntity(typeof(FireField));
            var heatBuffer = EntityManager.AddBuffer<FireHeat>(fireField);

            // Array of 0's
            var heatArray = new NativeArray<FireHeat>(gameConstants.FieldSize.x * gameConstants.FieldSize.y, Allocator.Temp, NativeArrayOptions.ClearMemory);

            // Add fire
            var cellsOnFire = (int)(gameConstants.FireSpawnDensity * heatArray.Length);

            for (int i = 0; i < cellsOnFire; i++)
                heatArray[random.NextInt(heatArray.Length)] = random.NextFloat(gameConstants.FireHeatFlashPoint, 1f);

            // Set fire
            heatBuffer.AddRange(heatArray);



            // Show where we spawned fire
            for (int y = 0; y < gameConstants.FieldSize.y; y++)
            {
                for (int x = 0; x < gameConstants.FieldSize.x; x++)
                {
                    if (heatArray[x + y * gameConstants.FieldSize.x] < 0.2f)
                        continue;

                    var flameEntity = EntityManager.Instantiate(gameConstants.FlamePrefab);

                    EntityManager.SetComponentData(flameEntity, new Translation { Value = new float3(x, 0, y) });
                }
            }
        }
    }
}
