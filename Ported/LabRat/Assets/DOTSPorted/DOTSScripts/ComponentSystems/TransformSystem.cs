using System;
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

        var query = EntityManager.CreateEntityQuery(typeof(BoardArrow));
        var arrows = query.ToComponentDataArray<BoardArrow>(Allocator.TempJob);

        uint seed = (uint)DateTime.UtcNow.Millisecond + 1;
        Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed);

        Entities
            .WithDisposeOnCompletion(arrows)
            .WithAny<Mouse,Cat>()
            .ForEach((ref Rotation rotation, ref Translation translation, ref Direction direction, in Position position) =>
        {
            int x = (int)(translation.Value.x + 0.5f);
            int z = (int)(translation.Value.z + 0.5f);
            byte tile = TileUtils.GetTile(tiles, x, z, boardInfo.width);

            // test for wall collisions
            if((tile & 0xf) != 0)
            {
                int randInt = 0;//random.NextInt(0, 2);
                if (((tile & 0x1) != 0) && (translation.Value.z - z) >= 0f && direction.Value == EntityDirection.Up)
                {
                    direction.Value = randInt % 2 == 0 ? EntityDirection.Right : EntityDirection.Left;
                    translation.Value.z = (float)z;
                }
                else if (((tile & 0x2) != 0) && (translation.Value.x - x) >= 0f && direction.Value == EntityDirection.Right)
                {
                    direction.Value = randInt % 2 == 0 ? EntityDirection.Down : EntityDirection.Up;
                    translation.Value.x = (float)x;
                }
                else if (((tile & 0x4) != 0) && (translation.Value.z - z) <= 0f && direction.Value == EntityDirection.Down)
                {
                    direction.Value = randInt % 2 == 0 ? EntityDirection.Left : EntityDirection.Right;
                    translation.Value.z = (float)z;
                }
                else if (((tile & 0x8) != 0) && (translation.Value.x - x) <= 0f && direction.Value == EntityDirection.Left)
                {
                    direction.Value = randInt % 2 == 0 ? EntityDirection.Up : EntityDirection.Down;
                    translation.Value.x = (float)x;
                }
            }
            
            for (int i = 0; i < arrows.Length; i++)
            {
                if (arrows[i].gridPosition.x == x && arrows[i].gridPosition.y == z)
                {
                    direction.Value = arrows[i].direction;
                    if (direction.Value == EntityDirection.Left || direction.Value == EntityDirection.Right)
                        translation.Value.z = arrows[i].gridPosition.y;
                    else
                    {
                        translation.Value.x = arrows[i].gridPosition.x;
                    }
                }
            }

            rotation.Value = quaternion.RotateY(math.radians((float)direction.Value * 90));
        }).Schedule();
    }
}
