using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(SpawningSystem))]
public class TransformSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var boardInfo = GetSingleton<BoardInfo>();
        int2[] extents = new int2[]
        {
            new int2(0, boardInfo.height),
            new int2(boardInfo.width, 0),
            new int2(boardInfo.height, boardInfo.width),
            new int2(0,0), 
        };
        
        
        Entities.ForEach((ref Rotation rotation, ref Translation translation, ref Direction direction, in Position position) =>
        {
            //translation.Value += new float3(position.position, 0.0f) * deltaTime;
            if (translation.Value.z >= (boardInfo.height - 1) && direction.Value == EntityDirection.Up)
            {
                direction.Value = EntityDirection.Right;
            }
            else if (translation.Value.x >= (boardInfo.width - 1) && direction.Value == EntityDirection.Right)
            {
                direction.Value = EntityDirection.Down;
            }
            else if (translation.Value.z <= 0 && direction.Value == EntityDirection.Down)
            {
                direction.Value = EntityDirection.Left;
            }
            else if (translation.Value.x <= 0)
            {
                direction.Value = EntityDirection.Up;
            }
            
            rotation.Value = quaternion.RotateY(math.radians((float)direction.Value * 90));
        }).Schedule();
    }
}
