using Unity.Entities;

public enum eTileType : byte
{
    Blank,
    DirectionArrow,
    Hole,
    Confuse,
    HomeBase
}

public enum eColor : byte
{
    None,
    Black,
    Red,
    Blue,
    Green
}

public struct Wall
{

    internal byte RawBits
    {
        return m_Bits;
    }

    private byte m_Bits;
}

public struct Tile
{
    public eTileType TileType
    {
        get
        {
            return (eTileType)(m_PackedData & 0xff);
        }
        set
        {
            m_PackedData &= 0xffffff00;
            m_PackedData |= (int)value;
        }
    }

    public eColor Color
    {
        get
        {
            return (eColor)((m_PackedData >> 8) & 0xff);
        }
        set
        {
            m_PackedData &= 0xffff00ff;
            m_PackedData |= ((int)value << 8);
        }
    }

    public eDirection Direction
    {
        get
        {
            return (eDirection)((m_PackedData >> 16) & 0xff);
        }
        set
        {
            m_PackedData &= 0xff00ffff;
            m_PackedData |= ((int)value << 16);
        }
    }

    public Wall Wall
    {
        get
        {
            return new Wall((m_PackedData >> 24) & 0xff);
        }
        set
        {
            m_PackedData &= 0x00ffffff;
            m_PackedData |= ((int)value.RawBits << 24);
        }
    }

    public bool HasWall(eDirection direction)
    {
        int dirAsInt = (int)direction;
        return m_PackedData & (1 << (dirAsInt + 24);
    }

    public void SetWall(eDirection direction, bool wall)
    {
        int dirAsInt = (int)direction;
        if (wall)
            m_PackedData |= 1 << (dirAsInt + 24);
        else
            m_PackedData &= ~(1 << (dirAsInt + 24));
    }

    // first byte is eTileType
    // second byte is eColor
    // third byte is eDirection
    private int m_PackedData;
}

public class BoardSystem : ComponentSystem
{
    const int k_Width = 10;
    const int k_Height = 10;

    NativeArray<Tile> m_Board;

    public Tile this[int x, int y]
    {
        get
        {
            return m_Board[y * k_Width + x];
        }
        set
        {
            m_Board[y * k_Width + x] = value;
        }
    }

    protected override void OnCreate()
    {
        m_Board = new NativeArray<Tile>(k_Width * k_Height, Allocator.Persistent);
        for (int y = 0; y < k_Height; ++y)
        {
            for (int x = 0; x < k_Width; ++x)
            {
                int index = y * k_Width + x;
                m_Board[index].TileType = eTileType.Blank;
                if (x == 0)
                    m_Board[index].SetWall(eDirection.West, true);
                else if (x == k_Width - 1)
                    m_Board[index].SetWall(eDirection.East, true);
            }

            if (y == 0)
                m_Board[index].SetWall(eDirection.North, true);
            else if (y == k_Height - 1)
                m_Board[index].SetWall(eDirection.South, true);
        }
    }
asdf
    protected override void OnUpdate()
    {
    }
}
