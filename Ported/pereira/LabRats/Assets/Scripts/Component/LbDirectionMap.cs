using Unity.Entities;

public struct LbDirectionMap : IBufferElementData
{
    public static implicit operator short(LbDirectionMap e) { return e.Value; }
    public static implicit operator LbDirectionMap(short e) { return new LbDirectionMap { Value = e }; }

    // Direction    BitInfo     Float3
    // North        0x00        0,0,1
    // East         0x01        1,0,0
    // South        0x10        0,0,-1
    // West         0x11        -1,0,0

    // Player       BitInfo
    // Player1      0x00
    // Player2      0x01
    // Player3      0x10
    // Player4      0x11

    // Byte distribution:
    // |                First Byte                                      |            Second Byte        |
    // | [] [] [] []  | []                | [] []     | []              | [] [] | [] [] | [] [] | [] [] |
    // | Not used     | Home base Flag    | Player ID |  Hole Flag      | North | East  | South | West  |

    public short Value;
}

//            Flag |Player | Dir |
//  [] [] []    []   [][]   [][]
public struct LbArrowDirectionMap : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator byte(LbArrowDirectionMap e) { return e.Value; }
    public static implicit operator LbArrowDirectionMap(byte e) { return new LbArrowDirectionMap { Value = e }; }

    public byte Value;
}