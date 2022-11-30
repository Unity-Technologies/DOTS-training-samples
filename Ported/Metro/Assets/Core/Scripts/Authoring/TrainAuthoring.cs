using Unity.Entities;
using Unity.Mathematics;

class TrainAuthoring : UnityEngine.MonoBehaviour
{
    public float MaxSpeed;
    public float StopTime;
}
 
class TrainBaker : Baker<TrainAuthoring>
{
    public override void Bake(TrainAuthoring authoring)
    {
        AddComponent(new IdleTime
        {
            Value = authoring.StopTime
        });
        AddComponent(new Speed
        {
            Value = authoring.MaxSpeed
        });
        AddComponent(new Position
        {
            Value = new float2(0f, 0f)
        });
        AddComponent(new Direction
        {
            Value = new float2(0f, 1f)
        });
        AddComponent<TargetPosition>();

        AddComponent<Waypoint>();
        AddComponent<TrainTag>();

    }
}