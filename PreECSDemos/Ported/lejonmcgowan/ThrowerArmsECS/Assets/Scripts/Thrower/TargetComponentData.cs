using Unity.Entities;

public struct TargetComponentData: IComponentData
{
    public float velocityX;
    public float rangeXMin;
    public float rangeXMax;
}