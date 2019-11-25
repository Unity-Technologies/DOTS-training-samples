using System;

public struct OccupiedSides
{
    public byte Mask;

    public bool TopOccupied
    {
        get => (Mask & 0x1) != 0;
        set => Mask = (byte) ((Mask & ~0x1) | (value ? 1 : 0));
    }
	
    public bool BottomOccupied
    {
        get => (Mask & 0x2) != 0;
        set => Mask = (byte) ((Mask & ~0x2) | (value ? 2 : 0));
    }

    public bool this[int i]
    {
        get => i == 0 ? TopOccupied : BottomOccupied;
        set
        {
            if (i == 0)
                TopOccupied = value;
            else
                BottomOccupied = value;
        }
    }
}
