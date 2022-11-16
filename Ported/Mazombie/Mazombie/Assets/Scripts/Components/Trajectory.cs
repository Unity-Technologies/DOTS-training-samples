using Unity.Entities;

public struct Trajectory : IBufferElementData
{
    public int Value;
    
    // Implicit conversions
    public static implicit operator int(Trajectory e) { return e.Value; }
    public static implicit operator Trajectory(int e) { return new Trajectory { Value = e }; }
}
