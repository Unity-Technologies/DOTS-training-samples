using System.Collections.Generic;
using Unity.Entities;

public partial struct CarEntity : IBufferElementData
{
    public Entity Value;
    public float position;
}

public struct CarEntityComparer : IComparer<CarEntity>
{
    public int Compare(CarEntity x, CarEntity y)
    {
        return x.position.CompareTo(y.position);
    }
}