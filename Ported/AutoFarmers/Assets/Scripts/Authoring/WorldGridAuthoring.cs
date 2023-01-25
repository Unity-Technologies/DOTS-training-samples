using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Rendering;

class WorldGridAuthoring : UnityEngine.MonoBehaviour
{
    public int GridSizeX = 1000;
    public int GridSizeY = 1000;

    class WorldBaker : Baker<WorldGridAuthoring>
    {
        public override void Bake(WorldGridAuthoring authoring)
        {
            AddComponent(new WorldGrid
            {
                typeGrid = new NativeArray<byte>(authoring.GridSizeX * authoring.GridSizeY, Allocator.Temp),
                entityGrid = new NativeArray<Entity>(authoring.GridSizeX * authoring.GridSizeY, Allocator.Temp),
                gridSize = new int2(authoring.GridSizeX, authoring.GridSizeY),
                offset = new float3(authoring.GridSizeX / 2.0f, 0, authoring.GridSizeY / 2.0f)
            });
        }
    }
}

struct WorldGridType : IComponentData
{
    public byte type;
}

struct WorldGrid : IComponentData
{
    //Use the type grid to figure out if there's any entities in the first place
    public NativeArray<byte> typeGrid;
    //Then use this to access the entity itself
    public NativeArray<Entity> entityGrid;
    public int2 gridSize;
    public float3 offset;

    //public bool needsRegeneration;
    //public int2 regenStartPos;
    //public int2 regenEndPos;

    public float3 GridToWorld(int x, int y)
    {
        
        return new float3(x, 0, y) - offset;
    }

    public int2 WorldToGrid(float3 pos)
    {
        return new int2((int)math.round(pos.x), (int)math.round(pos.z));
    }

    public int Get2Dto1DIndex(int x, int y)
    {
        return x + gridSize.x * y;
    }

    public int GetTypeAt(int x, int y)
    {
        return typeGrid[Get2Dto1DIndex(x,y)];
    }

    public void SetEntityAt(int x, int y, Entity e)
    {
        entityGrid[Get2Dto1DIndex(x,y)] = e;
    }

    public void SetTypeAt(int x, int y, byte type)
    {
        ////If we're already needing regeneration
        //if (needsRegeneration)
        //{
        //    //Let's see if our position expands the search bounds
        //    if (x < regenStartPos.x) regenStartPos.x = x;
        //    if (y < regenStartPos.y) regenStartPos.y = y;
        //    if (x > regenStartPos.x) regenStartPos.x = x;
        //    if (y > regenStartPos.y) regenStartPos.y = y;
        //}
        //else
        //{
        //    //If this is the first time, let's just set it to be one block.
        //    regenStartPos.x = x;
        //    regenStartPos.y = y;
        //    regenEndPos.x = x;
        //    regenEndPos.y = y;
        //}

        //needsRegeneration = true;
        typeGrid[Get2Dto1DIndex(x, y)] = type;
    }
}