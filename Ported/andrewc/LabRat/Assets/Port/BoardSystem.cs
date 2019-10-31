using System;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using UnityEngine;

public struct Tile
{
    public eTileType TileType
    {
        get
        {
            return (eTileType)(m_PackedData & 0xff);
        }
    }

    public Tile SetTileType(eTileType tileType)
    {
        Tile ret = new Tile();
        ret.m_PackedData = m_PackedData & 0xffffff00;
        ret.m_PackedData |= (uint)tileType;
        return ret;
    }

    public eColor Color
    {
        get
        {
            return (eColor)((m_PackedData >> 8) & 0xff);
        }
    }

    public Tile SetColor(eColor color)
    {
        Tile ret = new Tile();
        ret.m_PackedData = m_PackedData & 0xffff00ff;
        ret.m_PackedData |= ((uint)color) << 8;
        return ret;
    }

    public eDirection Direction
    {
        get
        {
            return (eDirection)((m_PackedData >> 16) & 0xff);
        }
    }

    public Tile SetDirection(eDirection direction)
    {
        Tile ret = new Tile();
        ret.m_PackedData = m_PackedData & 0xff00ffff;
        ret.m_PackedData |= ((uint)direction) << 16;
        return ret;
    }

    public byte RawWallBits
    {
        get => (byte)((m_PackedData >> 24) & 0xff);
    }

    public bool HasWall(eDirection direction)
    {
        int dirAsInt = (int)direction;
        uint shifted = (uint)(1 << (dirAsInt + 24));
        return (m_PackedData & shifted) != 0;
    }

    public Tile SetWall(eDirection direction, bool wall)
    {
        int rawWallBits = (int)RawWallBits;
        if (wall)
            rawWallBits |= 1 << ((int)direction);
        else
            rawWallBits &= 1 << ((int)direction);

        var ret = new Tile();
        ret.m_PackedData = m_PackedData;
        ret.m_PackedData &= 0x00ffffff;
        ret.m_PackedData |= (uint)((int)(rawWallBits << 24));
        return ret;
    }

    // first byte is eTileType
    // second byte is eColor
    // third byte is eDirection
    // fourth byte has packed bits for whether there's a wall in each direction
    public uint m_PackedData;
}

public struct Board : IDisposable
{
    public const int k_Width = 13;
    public const int k_Height = 13;

    public Vector2 Size
    {
        get => new Vector2(k_Width, k_Height);
    }

    public Vector2 CellSize
    {
        get => new Vector2(1f, 1f);
    }

    public static int2 ConvertWorldToTileCoordinates(float3 position)
    {
        return new int2((int)math.round(position.x), (int)math.round(position.z));
    }

    public static Vector3 GetTileCenterAtCoord(Vector2Int index)
    {
        return new Vector3(index.x, 0f, index.y);
    }

    public void Init()
    {
        m_Tiles = new NativeArray<Tile>(k_Width * k_Height, Allocator.Persistent);
        for (int y = 0; y < k_Height; ++y)
        {
            for (int x = 0; x < k_Width; ++x)
            {
                int index = y * k_Width + x;

                if (x == 0)
                    m_Tiles[index] = m_Tiles[index].SetWall(eDirection.West, true);
                else if (x == k_Width - 1)
                    m_Tiles[index] = m_Tiles[index].SetWall(eDirection.East, true);

                if (y == 0)
                    m_Tiles[index] = m_Tiles[index].SetWall(eDirection.South, true);
                else if (y == k_Height - 1)
                    m_Tiles[index] = m_Tiles[index].SetWall(eDirection.North, true);

                //if (x == 5 && y == 5)
                //    m_Tiles[index] = m_Tiles[index].SetTileType(eTileType.Hole);
                //if (x == 6 && y == 6)
                //    m_Tiles[index] = m_Tiles[index].SetTileType(eTileType.HomeBase);
            }
        }

        int numWalls = (int)(k_Width * k_Height * 0.2f);
        for (int c = 0; c < numWalls; ++c)
        {
            int x = UnityEngine.Random.Range(0, k_Width);
            int y = UnityEngine.Random.Range(0, k_Height);
            int dir = UnityEngine.Random.Range(0, 4);
            int index = y * k_Width + x;
            switch((eDirection)dir)
            {
                case eDirection.North:
                    m_Tiles[index] = m_Tiles[index].SetWall(eDirection.North, true);
                    if( y+1 < k_Height)
                    {
                        index = (y + 1) * k_Width + x;
                        m_Tiles[index] = m_Tiles[index].SetWall(eDirection.South, true);
                    }
                    break;
                case eDirection.South:
                    m_Tiles[index] = m_Tiles[index].SetWall(eDirection.South, true);
                    if (y - 1 >= 0)
                    {
                        index = (y - 1) * k_Width + x;
                        m_Tiles[index] = m_Tiles[index].SetWall(eDirection.North, true);
                    }
                    break;
                case eDirection.East:
                    m_Tiles[index] = m_Tiles[index].SetWall(eDirection.East, true);
                    if (x + 1 < k_Width)
                    {
                        index = y * k_Width + x + 1;
                        m_Tiles[index] = m_Tiles[index].SetWall(eDirection.West, true);
                    }
                    break;
                case eDirection.West:
                    m_Tiles[index] = m_Tiles[index].SetWall(eDirection.West, true);
                    if (x - 1 >= 0)
                    {
                        index = y * k_Width + x - 1;
                        m_Tiles[index] = m_Tiles[index].SetWall(eDirection.East, true);
                    }
                    break;
            }
           
        }

        // setup home bases
        float offset = 1f / 3f;
        int idx = (int)(k_Width * offset) + (int)(k_Height * offset) * k_Width;
        m_Tiles[idx] = m_Tiles[idx].SetTileType(eTileType.HomeBase);
        m_Tiles[idx] = m_Tiles[idx].SetColor(eColor.Red);

        idx = (int)(k_Width * 2f * offset) + (int)(k_Height * offset * 2f) * k_Width;
        m_Tiles[idx] = m_Tiles[idx].SetTileType(eTileType.HomeBase);
        m_Tiles[idx] = m_Tiles[idx].SetColor(eColor.Green);

        idx = (int)(k_Width * offset) + (int)(k_Height * offset * 2f) * k_Width;
        m_Tiles[idx] = m_Tiles[idx].SetTileType(eTileType.HomeBase);
        m_Tiles[idx] = m_Tiles[idx].SetColor(eColor.Black);

        idx = (int)(k_Width * 2f * offset) + (int)(k_Height * offset) * k_Width;
        m_Tiles[idx] = m_Tiles[idx].SetTileType(eTileType.HomeBase);
        m_Tiles[idx] = m_Tiles[idx].SetColor(eColor.Blue);

    }

    public void Dispose()
    {
        m_Tiles.Dispose();
    }

    public Tile this[int x, int y]
    {
        get
        {
            return m_Tiles[y * k_Width + x];
        }
        set
        {
            m_Tiles[y * k_Width + x] = value;
        }
    }

    private NativeArray<Tile> m_Tiles;
}

public class BoardSystem : ComponentSystem
{
    Board m_Board;

    public Board Board
    {
        get => m_Board;
    }

    protected override void OnCreate()
    {
        m_Board = new Board();
        m_Board.Init();
    }

    protected override void OnDestroy()
    {
        m_Board.Dispose();
    }

    protected override void OnUpdate()
    {
    }
}
