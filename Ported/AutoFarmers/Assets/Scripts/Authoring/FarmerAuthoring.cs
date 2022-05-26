using Unity.Entities;
using Unity.Mathematics;

class FarmerAuthoring : UnityEngine.MonoBehaviour
{
    public float Speed;
}

class FarmerBaker : Baker<FarmerAuthoring>
{
    public override void Bake(FarmerAuthoring authoring)
    {
        AddComponent(new Farmer
        {
        });
        
        AddComponent(new Mover
        {
            Speed = authoring.Speed,
            YOffset=1
        });
        AddComponent(new FarmerIntent
        {
            value = FarmerIntentState.None,
            elapsed = 0,
            random = new Random((uint)UnityEngine.Random.Range(0, uint.MaxValue))
        });
        AddComponent(new PathfindingIntent
        {
            destinationType = PathfindingDestination.None,
            navigatorType = NavigatorType.Farmer
        });
        AddComponent(new FarmerCombat { });
    }
}