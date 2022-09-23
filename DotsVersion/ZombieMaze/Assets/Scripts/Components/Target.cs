using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct Target : IComponentData
{
    public bool TargetPlayer;
    public int2 TargetPosition;
    public NativeList<int2> TargetPath;
    public int PathIndex;
}