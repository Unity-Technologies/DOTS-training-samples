using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Entities;
using Unity.Mathematics;

public class AIMoveCursorSystem : SystemBase
{
    private EntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        this.ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = this.ecbSystem.CreateCommandBuffer();
        Entities
            .WithStructuralChanges()
            .WithAll<AIPlayerMovingCursorTag>()
            .ForEach((Entity player, ref MousePosition current, ref AIMousePositionTarget target) =>
            {
                var dir = math.normalize(target.Value - current.Value);

                current.Value += dir * 5;

                var dist = math.distance(current.Value, target.Value);

                if (dist <= 3f)
                {
                    ecb.RemoveComponent<AIPlayerMovingCursorTag>(player);

                    var newArrow = ecb.CreateEntity();
                    ecb.AddComponent(newArrow, new PlaceArrowEvent
                    {
                        Player = player
                    });

                    ecb.AddComponent(newArrow, new Direction
                    {
                        Value = target.Direction
                    });

                    ecb.AddComponent(newArrow, new PositionXZ
                    {
                        Value = new float2(target.Tile.x, target.Tile.y)
                    });
                }
            }).Run();
    }
}