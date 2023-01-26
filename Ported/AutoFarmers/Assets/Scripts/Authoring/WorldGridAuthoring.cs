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
                typeGrid = new NativeArray<byte>(authoring.GridSizeX * authoring.GridSizeY, Allocator.Persistent),
                entityGrid = new NativeArray<Entity>(authoring.GridSizeX * authoring.GridSizeY, Allocator.Persistent),
                gridSize = new int2(authoring.GridSizeX, authoring.GridSizeY),
                offset = new float3(authoring.GridSizeX / 2.0f, 0, authoring.GridSizeY / 2.0f),
                arraySize = authoring.GridSizeX * authoring.GridSizeY
            });

            //AddBuffer<Grid>();
        }
    }
}

struct WorldGridType : IComponentData
{
    public byte type;
}

//TODO: Use this instead of having native collections directly in the struct.
struct GridCell : IBufferElementData
{
    public byte type;
    public Entity entity;
}

struct WorldGrid : IComponentData
{
    //Use the type grid to figure out if there's any entities in the first place
    public NativeArray<byte> typeGrid;
    //Then use this to access the entity itself
    public NativeArray<Entity> entityGrid;
    public int2 gridSize;
    public int arraySize;
    public float3 offset;

    //public bool needsRegeneration;
    //public int2 regenStartPos;
    //public int2 regenEndPos;

    public float3 GridToWorld(int x, int y)
    {
        return new float3(x, 0, y) - offset;
    }

    public float3 GridToWorld(int2 pos)
    {
        return GridToWorld(pos.x,pos.y);    
    }

    public int2 WorldToGrid(float3 pos)
    {
        return new int2((int)math.round(pos.x+offset.x), (int)math.round(pos.z+offset.z));
    }

    public int Get2Dto1DIndex(int x, int y)
    {
        int index = x + gridSize.x * y;
        if (index < 0 || index > arraySize - 1) return 0;
        return index;
    }

    public int GetTypeAt(int x, int y)
    {
        return typeGrid[Get2Dto1DIndex(x,y)];
    }

    public int GetTypeAt(int2 point)
    {
        return GetTypeAt(point.x, point.y);
    }

    public void SetEntityAt(int x, int y, Entity e)
    {
        entityGrid[Get2Dto1DIndex(x, y)] = e;
    }

    public void SetEntityAt(int2 point, Entity e)
    {
        SetEntityAt(point.x, point.y, e);
    }

    public Entity GetEntityAt(int x, int y)
    {
        return entityGrid[Get2Dto1DIndex(x, y)];
    }

    public Entity GetEntityAt(int2 point)
    {
        return GetEntityAt(point.x, point.y);
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

    public void SetTypeAt(int2 point, byte type)
    {
        SetTypeAt(point.x, point.y, type);
    }
}