using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public enum CellType
{
    Invalid,
    Floor,
    TeamA,
    TeamB,
}

[Serializable]
public struct Box
{
    public float3 Center;
    public float3 Extents;
    public float3 Min;
    public float3 Max;

    public Box(float3 center, float3 extents)
    {
        Center = center;
        Extents = extents;
        Min = default;
        Max = default;
        
        Recalculate();
    }

    public void Recalculate()
    {
        Min = Center - Extents;
        Max = Center + Extents;
    }

    public float3 GetClosestPoint(float3 pos)
    {
        float3 result = pos;
        result.x = (result.x < Min.x) ? Min.x : result.x;
        result.y = (result.y < Min.y) ? Min.y : result.y;
        result.z = (result.z < Min.z) ? Min.z : result.z;
        result.x = (result.x > Max.x) ? Max.x : result.x;
        result.y = (result.y > Max.y) ? Max.y : result.y;
        result.z = (result.z > Max.z) ? Max.z : result.z;
        return result;
    }

    public bool IsInside(float3 pos)
    {
        if (pos.x < Min.x ||
            pos.y < Min.y ||
            pos.z < Min.z ||
            pos.x > Max.x ||
            pos.y > Max.y ||
            pos.z > Max.z)
        {
            return false;
        }

        return true;
    }
}

[Serializable]
public struct GridCharacteristics
{
    public int TeamCellCountX;
    public int CellCountX;
    public int CellCountZ;
    public float CellSize;
    
    public float3 Center;
    public float3 BottomCorner;
    public float TeamZoneHeight;
    
    public int TeamAStartX;
    public int FloorStartX;
    public int TeamBStartX;

    public Box TeamABounds;
    public Box TeamBBounds;
    public Box LevelBounds;

    public GridCharacteristics(
        float3 center,
        int floorCellCountX,
        int teamCellCountX,
        int cellCountZ,
        float cellSize,
        float teamZoneHeight)
    {
        TeamCellCountX = teamCellCountX;
        CellCountX = floorCellCountX + teamCellCountX + teamCellCountX;
        CellCountZ = cellCountZ;
        CellSize = cellSize;
        TeamZoneHeight = teamZoneHeight;
        
        float gridLengthX = CellSize * CellCountX;
        float gridLengthZ = CellSize * CellCountZ;

        Center = center;
        BottomCorner = new float3
        {
            x = center.x - (gridLengthX * 0.5f),
            y = center.y,
            z = center.z - (gridLengthZ * 0.5f),
        };

        TeamAStartX = 0;
        FloorStartX = TeamAStartX + teamCellCountX;
        TeamBStartX = FloorStartX + floorCellCountX;
        
        float totalLevelLength = CellCountX * CellSize;
        float totalLevelWidth = CellCountZ * CellSize;
        float3 teamZoneExtents = new float3(TeamCellCountX * CellSize * 0.5f, TeamZoneHeight * 0.5f, CellCountZ * CellSize * 0.5f);
        float3 levelExtents = new float3(totalLevelLength * 0.5f, TeamZoneHeight * 0.5f, totalLevelWidth * 0.5f);

        TeamABounds = new Box(BottomCorner + teamZoneExtents, teamZoneExtents);
        TeamBBounds = new Box(BottomCorner + new float3(totalLevelLength - teamZoneExtents.x, teamZoneExtents.y, teamZoneExtents.z), teamZoneExtents);
        LevelBounds = new Box(BottomCorner + levelExtents, levelExtents);
    }

    public int GetTotalCellCount()
    {
        return CellCountX * CellCountZ;
    }

    public int2 GetCellCoordinatesOfPosition(float3 pos)
    {
        int2 coords = new int2
        {
            x = (int)math.floor((pos.x - BottomCorner.x) / CellSize),
            y = (int)math.floor((pos.z - BottomCorner.z) / CellSize),
        };

        // Clamp
        if (coords.x < 0)
        {
            coords.x = 0;
        }
        if (coords.x >= CellCountX)
        {
            coords.x = CellCountX - 1;
        }
        if (coords.y < 0)
        {
            coords.y = 0;
        }
        if (coords.y >= CellCountZ)
        {
            coords.y = CellCountZ - 1;
        }

        return coords;
    }

    public int GetIndexOfCellCoordinates(int2 coords)
    {
        return (coords.y * CellCountX) + coords.x;
    }

    public int2 GetCoordinatesOfCellIndex(int cellIndex)
    {
        // X first, then Z
        return new int2
        {
            x = cellIndex % CellCountX,
            y = cellIndex / CellCountX,
        };
    }
    
    public float3 GetPositionOfCell(int2 coords)
    {
        float halfCellSize = CellSize * 0.5f;
        return new float3
        {
            x = BottomCorner.x + (coords.x * CellSize) + halfCellSize,
            y = -(CellSize * 0.5f),
            z = BottomCorner.z + (coords.y * CellSize) + halfCellSize,
        };
    }

    public CellType GetTypeOfCell(int2 coords)
    {
        if (coords.x >= TeamBStartX)
        {
            return CellType.TeamB;
        }
        else if (coords.x < FloorStartX)
        {
            return CellType.TeamA;
        }
        else
        {       
            return CellType.Floor;
        }
    }

    public float3 GetMapCenter()
    {
        return Center + (TeamZoneHeight * 0.5f);
    }
}

[Serializable]
public struct GameRuntimeData : IComponentData
{
    public Random Random;
    public GridCharacteristics GridCharacteristics;
}

[Serializable]
public struct ResourceSpawnEvent : IBufferElementData
{
    public float3 Position;
}

[Serializable]
public struct BeeSpawnEvent : IBufferElementData
{
    public float3 Position;
    public Team Team;
}
