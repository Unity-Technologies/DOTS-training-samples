using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

// ReSharper disable once InconsistentNaming
public struct GridComponent : IComponentData
{
    public int Size;
}

public struct GridTile : IBufferElementData
{
    /* Grid Values */
    // Negative Values - rock offset
    
    // 1...[RockOffsetX][RockOffsetY]
    
    // 0 - nothing
    // 1 - Tilled
    // 2 - Shop
    // 3 - Plant
    // 4 - Rock
    // Larger Odd Numbers health of Plant
    // Larger Even Numbers health or Rock
    public int Value;
    
    public static implicit operator int(GridTile e)
    {
        return e.Value;
    }

    public static implicit operator GridTile(int e)
    {
        return new GridTile {Value = e};
    }

    public bool IsRock()
    {
        return Value < 0 || ((Value >= 4) && (Value % 2 == 0));
    }

    public bool IsRockOrigin()
    {
        return ((Value >= 4) && (Value % 2 == 0));
    }

    public bool IsPlant()
    {
        return (Value >= 3) && (Value % 2 == 1);
    }

    public bool IsTilled()
    {
        return Value == 1;
    }

    public bool IsShop()
    {
        return Value == 2;
    }

    public bool IsNothing()
    {
        return Value == 0;
    }

    public int2 GetRockOffset()
    {
        var offset = new int2();
        if (Value < 0)
        {
            offset.x = (Value & 0x38) >> 3;   // 00111000
            offset.y = Value & 0x7;     // 00000111
        }

        return offset;
    }

    public int GetRockHealth()
    {
        return (Value - 2) / 2;
    }

    public void SetRockHealth(int health)
    {
        Value = health == 0 ? 0 : (2 + health * 2);
    }

    public int GetPlantHealth()
    {
        return (Value - 3) / 2;
    }

    public void SetPlantHealth(int health)
    {
        Value = 3 + health * 2;
    }
    
    
    
}
