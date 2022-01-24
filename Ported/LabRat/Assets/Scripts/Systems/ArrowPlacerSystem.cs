using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class ArrowPlacerSystem : SystemBase
{
    (int2, DirectionComponent.DirectionEnum) FromSceneToTableCoords(float2 position)
    {
        return (new int2((int)position.x, (int)position.y), DirectionComponent.DirectionEnum.FORWARD);
    }

    protected override void OnUpdate()
    {
        var maxArrowUsages = GetSingleton<MaxArrowUsagesPerPlayerComponent>();
        var playerInputEntity = GetSingletonEntity<PlayerInputTag>();
        var playerPos = GetComponent<PositionComponent>(playerInputEntity);
        var playerColor = GetComponent<ColorComponent>(playerInputEntity);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        if (Input.GetMouseButtonDown(0))
        {
            var (coordinate, direction) = FromSceneToTableCoords(playerPos.pos);

            var entity = ecb.CreateEntity();
            ecb.AddComponent<TileComponent>(entity, new TileComponent(coordinate));
            ecb.AddComponent<DirectionComponent>(entity, new DirectionComponent(direction));
            ecb.AddComponent<ColorComponent>(entity, playerColor);
            ecb.AddComponent(entity, new ArrowMiceCountComponent(maxArrowUsages.maxArrowUsages));
            //TODO: Add time since placed if we need it.
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
