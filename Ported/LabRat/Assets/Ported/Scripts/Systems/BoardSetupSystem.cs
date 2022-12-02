using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[BurstCompile]
public partial struct BoardSetupSystem : ISystem
{
    ComponentLookup<Unity.Transforms.LocalToWorld> m_LocalToWorldTransformFromEntity;

    public NativeArray<Entity> tiles;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BoardConfig>();
        m_LocalToWorldTransformFromEntity = state.GetComponentLookup<LocalToWorld>(true);
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        Debug.LogError("BoardSetupSystem OnUpdate");
        
        m_LocalToWorldTransformFromEntity.Update(ref state);
        
        var config = SystemAPI.GetSingleton<BoardConfig>();
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        //var tiles = CollectionHelper.CreateNativeArray<Entity>((config.breadth * config.width), Allocator.Temp);

        tiles = new NativeArray<Entity>((config.height * config.width), Allocator.Temp);


        bool whiteTile = true;
        int posX = 0;
        int posZ = 0;
        
        int tileIndex = 0;
        
        for (int x = 0; x < config.height; x++)
        {
            for (int z = 0; z < config.width; z++)
            {
                float3 worldPosition = new float3(posX, 0.0f, posZ);
                var spawnTransform = LocalTransform.FromPosition(worldPosition);

                int2 southTile = new int2(-1, -1);
                int2 northTile = new int2(-1, -1);
                int2 westTile = new int2(-1, -1);
                int2 eastTile = new int2(-1, -1);
                
                int2 nextTile = new int2(x,z);
                if (z < (config.width - 1))
                {
                    nextTile[1]++;
                }
                else
                {
                    nextTile[0]++;
                }

                if (x == (config.height) && z == (config.width))
                {
                    nextTile.x = 0;
                    nextTile.y = 0;
                }

                //West
                if (x > 0)
                {
                    westTile = new int2((x - 1), z);
                }
                
                //East
                if (x < (config.height - 1))
                {
                    eastTile = new int2((x + 1), z);
                }
                
                //North
                if (z > 0)
                {
                    northTile = new int2(x, (z - 1));
                }
                
                //South
                if (z < (config.width - 1))
                {
                    southTile = new int2(x, (z + 1));
                }
                
                /*
                 *                 case MovementDirection.North:
                    output.x = 0;
                    output.y = -1;
                    break;
                case MovementDirection.South:
                    output.x = 0;
                    output.y = 1;
                    break;
                case MovementDirection.East:
                    output.x = 1;
                    output.y = 0;
                    break;
                case MovementDirection.West:
                    output.x = -1;
                    output.y = 0;
                 */
                
                whiteTile = !whiteTile;
                
                Entity entity = ecb.Instantiate(whiteTile ? config.whiteTileEntity : config.grayTileEntity);
                ecb.SetComponent(entity, spawnTransform);

                TileComponent tileComponent = new TileComponent
                {
                    nextTile = nextTile, 
                    gridIndex = new int2(x,z),
                    southTile = southTile,
                    northTile = northTile,
                    westTile = westTile,
                    eastTile = eastTile,
                };
                ecb.SetComponent(entity, tileComponent);
                
                tiles[tileIndex] = entity;
                tileIndex++;
                
                //tiles.Add(entity);
                
                /*
                 * var spawnLocalToWorld = LocalToWorldTransformFromEntity[unit.spawnPoint].Position;
        var spawnTransform = LocalTransform.FromPosition(spawnLocalToWorld); //Unity.Transforms.WorldTransform.FromMatrix(spawnLocalToWorld.Value);

        //var random = Unity.Mathematics.Random.CreateFromIndex((uint)instance.Index);
        //Debug.Log($"Getting random seed with with idx={randomSeed}");
        
        var random = Unity.Mathematics.Random.CreateFromIndex((uint)randomSeed);
        
        ECB.SetComponent(instance, spawnTransform);
                 */
                
                
                posZ++;
            }

            posZ = 0;
            posX++;
        }

        /*
        for (int i = 0; i < tiles.Length; i++)
        {
            Entity tileA = tiles[(i)];
            Entity tileB = tiles[i == (tiles.Length-1) ? 0 : (i + 1)];

            TileComponent component = new TileComponent { nextTile = tileB };
            ecb.SetComponent(tileA, component);            
        }
        */
        
        //Go through the tile entities, set neighbours
        /*
        for (int x = 0; x < config.height; x++)
        {
            for (int z = 0; z < config.width; z++)
            {
                //just start with a "get next" to demo how this looks technically

                //ComponentLookup<TileComponent> myTypeFromEntity = tiles[0] ComponentLookup<TileComponent>(true);

                //TileComponent tileA = tiles[0].GetComponentDataFromEntity<TileComponent>(true);
                
                //TileComponent tileA = ;

                Entity tileA = tiles[(x + z)];
                Entity tileB = tiles[x == config.height && z == config.width ? 0 : (x + z + 1)];

                TileComponent component = new TileComponent { nextTile = tileB };
                ecb.SetComponent(tileA, component);
            }
        }
        */


        /*
         * 
           var vehicles = CollectionHelper.CreateNativeArray<Entity>(config.TankCount, Allocator.Temp);
           ecb.Instantiate(config.TankPrefab, vehicles);
         */   
     
        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}
