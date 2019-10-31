using System;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;

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
    const int k_Width = 13;
    const int k_Height = 13;

    public static int2 ConvertWorldToTileCoordinates(float3 position)
    {
        return new int2((int)math.round(position.x), (int)math.round(position.z));
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
            }
        }
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
