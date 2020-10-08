using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(SpawningSystem))]
public class TransformSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<TileMap>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var boardInfo = GetSingleton<BoardInfo>();

        Entity tilemapEntity = GetSingletonEntity<TileMap>();
        TileMap tileMap = EntityManager.GetComponentObject<TileMap>(tilemapEntity);
        NativeArray<byte> tiles = tileMap.tiles;

        Entities.ForEach((ref Rotation rotation, ref Translation translation, ref Direction direction, in Position position) =>
        {
            int x = (int)(translation.Value.x + 0.5f);
            int z = (int)(translation.Value.z + 0.5f);
            byte tile = TileUtils.GetTile(tiles, x, z, boardInfo.width);

            // test for wall collisions
            if((tile & 0xf) != 0)
            {
                if (((tile & 0x1) != 0) && (translation.Value.z - z) >= 0f && direction.Value == EntityDirection.Up)
                {
                    direction.Value = EntityDirection.Right;
                    translation.Value.z = (float)z;
                }
                else if (((tile & 0x2) != 0) && (translation.Value.x - x) >= 0f && direction.Value == EntityDirection.Right)
                {
                    direction.Value = EntityDirection.Down;
                    translation.Value.x = (float)x;
                }
                else if (((tile & 0x4) != 0) && (translation.Value.z - z) <= 0f && direction.Value == EntityDirection.Down)
                {
                    direction.Value = EntityDirection.Left;
                    translation.Value.z = (float)z;
                }
                else if (((tile & 0x8) != 0) && (translation.Value.x - x) <= 0f && direction.Value == EntityDirection.Left)
                {
                    direction.Value = EntityDirection.Up;
                    translation.Value.x = (float)x;
                }
            }
            
            rotation.Value = quaternion.RotateY(math.radians((float)direction.Value * 90));
        }).Schedule();
    }
}
