

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
partial struct BucketVisualUpdateSystem : ISystem {
    //This system will handle reading the fill level and using that to
    //calculate the bucket's size and color

    private float4 cyan;
    private float4 blue;
    
    //Tile access references
    TileGridConfig m_TileGridConfig;
    TileGrid m_TileGrid;

    public void OnCreate(ref SystemState state) {
        cyan = new float4(0, 1, 1, 1);
        blue = new float4(0, 0, 1, 1);
        
        state.RequireForUpdate<TileGridConfig>();
        state.RequireForUpdate<TileGrid>();
        state.RequireForUpdate<HeatBufferElement>();
    }

    public void OnDestroy(ref SystemState state) {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        //Initialize heat grid in order to manipulate it. 
        //Can I get those singleton references OnCreate, so that I don't have to request them every time? 
        // Better way to do this?
        m_TileGridConfig = SystemAPI.GetSingleton<TileGridConfig>();
        m_TileGrid = SystemAPI.GetSingleton<TileGrid>();
        var heatBuffer = state.EntityManager.GetBuffer<HeatBufferElement>(m_TileGrid.entity);
        
        void DouseHeat(ref SystemState state, int index, int tileGridSize) {
            SetHeat(ref state, index, 0f);
            
            // Spread to upper tile
            var upperTile = index + tileGridSize;
            if (upperTile < heatBuffer.Length)
            {
                SetHeat(ref state, upperTile, 0f);
            }

            // Spread to lower tile
            var lowerTile = index - tileGridSize;
            if (lowerTile >= 0)
            {
                SetHeat(ref state, lowerTile, 0f);
            }

            // Spread to left tile
            var leftTile = index - 1;
            if (leftTile >= 0)
            {
                SetHeat(ref state, leftTile, 0f);
            }

            // Spread to right tile
            var rightTile = index + 1;
            if (rightTile < heatBuffer.Length)
            {
                SetHeat(ref state, rightTile, 0f);
            }
            
            // Spread to up left tile
            var upLeftTile = upperTile - 1;
            if(upLeftTile < heatBuffer.Length && upLeftTile >= 0)
            {
                SetHeat(ref state, upLeftTile, 0f);
            }
            
            // Spread to up right tile
            var upRightTile = upperTile + 1;
            if (upRightTile < heatBuffer.Length && upRightTile >= 0)
            {
                SetHeat(ref state, upRightTile, 0f);
            }
            
            // Spread to down left tile
            var downLeftTile = lowerTile - 1;
            if (downLeftTile < heatBuffer.Length && downLeftTile >= 0)
            {
                SetHeat(ref state, downLeftTile, 0f);
            }
            
            // Spread to down right tile
            var downRightTile = lowerTile + 1;
            if (downRightTile < heatBuffer.Length && downRightTile >= 0)
            {
                SetHeat(ref state, downRightTile, 0f);
            } 
        }

        void SetHeat(ref SystemState state, int tileIndex, float amount) {
            var heat = heatBuffer[tileIndex];
            heat.Heat = amount;
            heatBuffer[tileIndex] = heat;
        }
        
        foreach (var bucket in SystemAPI.Query<BucketAspect>()) {
            //Handle fill-dependent elements
            bucket.Scale = 0.5f + (bucket.FillLevel * 2);
            bucket.Color = math.lerp(cyan, blue, bucket.FillLevel);

            //Handle bucket's interaction with the TileGrid
            if (bucket.Interactions == BucketInteractions.Pour) {
                // Convert bucket position to tile position
                var bucketX = bucket.Position.x;
                var bucketZ = bucket.Position.z;
                var tileX = Mathf.RoundToInt(bucketX / m_TileGridConfig.CellSize);
                var tileZ = Mathf.RoundToInt(bucketZ / m_TileGridConfig.CellSize);
                var tileIndex = tileX * m_TileGridConfig.Size + tileZ;
                
                DouseHeat(ref state, tileIndex, m_TileGridConfig.Size);
                bucket.Interactions = BucketInteractions.Dropped;
                bucket.FillLevel = 0;
            }
        }
    }
}