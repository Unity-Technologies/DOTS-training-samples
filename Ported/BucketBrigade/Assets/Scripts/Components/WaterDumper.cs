using Unity.Entities;

// An empty component is called a "tag component".
struct WaterDumper : IComponentData
{
    public enum WaterDumperState
    {
        GoToBucket,
        GoToFire
    };
    public WaterDumperState state;
}