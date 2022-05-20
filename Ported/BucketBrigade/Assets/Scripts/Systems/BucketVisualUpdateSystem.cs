

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;

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
        
        int count = 0;

        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        NativeList<int> fireTiles = new NativeList<int>(heatBuffer.Length, allocator);
        foreach (HeatBufferElement heatElement in heatBuffer)
        {
            var heat = heatElement.Heat;
            if (heat > 0.0f)
            {
                fireTiles.Add(count);
            }
            count++;
        }
        
        void DouseHeat(ref SystemState state, int index, int tileGridSize) {
            foreach (var fireTile in fireTiles)
            {
                SetHeat(ref state, fireTile, 0f);
            
                // Spread to upper tile
                var upperTile = fireTile + tileGridSize;
                if (upperTile < heatBuffer.Length)
                {
                    SetHeat(ref state, upperTile, 0f);
                }

                // Spread to lower tile
                var lowerTile = fireTile - tileGridSize;
                if (lowerTile >= 0)
                {
                    SetHeat(ref state, lowerTile, 0f);
                }

                // Spread to left tile
                var leftTile = fireTile - 1;
                if (leftTile >= 0)
                {
                    SetHeat(ref state, leftTile, 0f);
                }

                // Spread to right tile
                var rightTile = fireTile + 1;
                if (rightTile < heatBuffer.Length)
                {
                    SetHeat(ref state, rightTile, 0f);
                }
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
            
            //TODO: Handle position based on holder. How do I get the position of a reference I hold? A GetComponent, in a way?
            //bucket.Position = bucket.Holder.Position
            
            //Handle bucket's interaction with the TileGrid
            if (bucket.Interactions == BucketInteractions.Drop) {
                //It was "told" to drop; put out nearby fires
                //TODO: Here 'index' refers to the index of the tile the bucket is on. Need to find out how to get this. 
                DouseHeat(ref state, 0, m_TileGridConfig.Size);
                bucket.Interactions = BucketInteractions.Dropped;
            }
        }
    }
}