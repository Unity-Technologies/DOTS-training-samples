
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SetupBotChainSystem : SystemBase
{
    static float3 GetChainPosition(int _index, int _chainLength, float3 _startPos, float3 _endPos)
    {
        // adds two to pad between the SCOOPER AND THROWER
        float progress = (float)_index / _chainLength;
        float curveOffset = math.sin(progress * math.PI) * 1f;
        // get Vec2 data
        float2 heading = new float2(_startPos.x, _startPos.z) - new float2(_endPos.x, _endPos.y);
        float distance = math.length(heading);
        float2 direction = heading / distance;
        float2 perpendicular = new float2(direction.y, -direction.x);
        //Debug.Log("chain progress: " + progress + ",  curveOffset: " + curveOffset);
        return math.lerp(_startPos, _endPos, (float)_index / (float)_chainLength) + (new float3(perpendicular.x, 0f, perpendicular.y) * curveOffset);
    }

    protected override void OnUpdate()
    {
        var onFireCellsEntities = FindOnFireCellSystem.onFireCells.ToEntityArray(Allocator.Temp);
        var onFireCellsTranslations = FindOnFireCellSystem.onFireCells.ToComponentDataArray<Translation>(Allocator.Temp);

        if (onFireCellsEntities.Length != 0)
        {
            var waterCellsEntities = FindWaterCellSystem.waterCells.ToEntityArray(Allocator.Temp);
            var waterCellsTranslations = FindWaterCellSystem.waterCells.ToComponentDataArray<Translation>(Allocator.Temp);

            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            Entities
                .WithNone<BotChain>()
                .ForEach((Entity entity, in DynamicBuffer<PasserFullBufferElement> fullChain, in DynamicBuffer<PasserEmptyBufferElement> emptyChain) =>
                {
                    var bot0Position = GetComponent<Translation>(fullChain[0]).Value;
                    var closestWater = FireSim.GetClosestEntity(bot0Position, waterCellsEntities, waterCellsTranslations);

                    var waterPosition = GetComponent<Translation>(closestWater).Value;
                    var closestFire = FireSim.GetClosestEntity(waterPosition, onFireCellsEntities, onFireCellsTranslations);
                    var firePosition = GetComponent<Translation>(closestFire).Value;

                    waterPosition.y = 1.6f;
                    firePosition.y = 1.6f;

                    ecb.AddComponent(entity, new BotChain()
                    {
                        StartChain = waterPosition,
                        EndChain = firePosition
                    });

                    for (int i = 0; i < fullChain.Length; i++)
                    {
                        var pickupPosition = GetChainPosition(i, fullChain.Length, waterPosition, firePosition);
                        var dropoffPosition = GetChainPosition(i + 1, fullChain.Length, waterPosition, firePosition);
                        ecb.AddComponent(fullChain[i], new PasserBot { PickupPosition = pickupPosition, DropoffPosition = dropoffPosition });
                        ecb.AddComponent(fullChain[i], new GotoPickupLocation());
                    }

                    for (int i = 0; i < emptyChain.Length; i++)
                    {
                        var pickupPosition = GetChainPosition(i, emptyChain.Length, firePosition, waterPosition);
                        var dropoffPosition = GetChainPosition(i + 1, emptyChain.Length, firePosition, waterPosition);
                        ecb.AddComponent(emptyChain[i], new PasserBot { PickupPosition = pickupPosition, DropoffPosition = dropoffPosition });
                        ecb.AddComponent(emptyChain[i], new GotoPickupLocation());
                    }
                }).Run();

            ecb.Playback(World.EntityManager);
            ecb.Dispose();

            waterCellsEntities.Dispose();
            waterCellsTranslations.Dispose();
        }

        onFireCellsEntities.Dispose();
        onFireCellsTranslations.Dispose();
    }
}
