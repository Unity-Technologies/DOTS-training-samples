using Unity.Entities;
using Unity.Mathematics;

public struct Cluster : IComponentData
{
    public float3 Position;
}

public struct ClusterGeneration : IComponentData
{
    public int NumberOfSubClusters;
    public Entity BarPrefab;
    // Remove these probably:
    public int MinTowerHeight;
    public int MaxTowerHeight;
}

public struct Joint : IBufferElementData
{
    public float3 Value;
    public float3 OldPos;
    public bool IsAnchored;
}

//TODO: Break into 2 components? (probably less necessary)
public struct Connection : IBufferElementData
{
    public int J1, J2;
    public float OriginalLength;
}

public struct InitializeJointNeighbours : IComponentData
{
    
}

public struct JointNeighbours : IBufferElementData
{
    public int Value;
}

public struct Bar : IBufferElementData
{
    public Entity Value;

    public static implicit operator Entity(in Bar b) => b.Value;
    public static implicit operator Bar(in Entity e) => new Bar() {Value = e};
}

public struct BarVisualizer : IComponentData {}