using Unity.Entities;
using Unity.Rendering;
using Unity.Collections;
using Unity.Mathematics;

[UpdateBefore(typeof(FireSystem))]
public partial class SetupGame : SystemBase
{
    [NotBurstCompatible]
    protected override void OnUpdate()
    {
        if (!TryGetSingletonEntity<SpawnMarker>(out Entity marker))
            return;

        EntityManager.DestroyEntity(marker);

        // TODO: Opt-out fire

        var gameConstants = GetSingleton<GameConstants>();

        var random = new Random(12345);

        // TODO: Spawn teams, firebrigades, buckets

        // Fire
        {
            var fireField = EntityManager.CreateEntity(typeof(FireField));
            var heatBuffer = EntityManager.AddBuffer<FireHeat>(fireField);

            // Array of 0's
            var heatArray = new NativeArray<FireHeat>(gameConstants.FieldSize.x * gameConstants.FieldSize.y, Allocator.Temp, NativeArrayOptions.ClearMemory);

            // TODO: Add fire
            var cellsOnFire = (int)(gameConstants.FireSpawnDensity * heatArray.Length);

            for (int i = 0; i < cellsOnFire; i++)
                heatArray[random.NextInt(heatArray.Length)] = random.NextFloat(gameConstants.FireHeatFlashPoint, 1f);

            // Set fire
            heatBuffer.AddRange(heatArray);
        }
    }
}
