using Unity.Entities;

public class DroneAuthoring : UnityEngine.MonoBehaviour
{
    public float Speed;
}

class DroneBaker : Baker<DroneAuthoring>
{
    public override void Bake(DroneAuthoring authoring)
    {
        AddComponent(new Drone
        {
        });
        AddComponent(new Mover
        {
            HasDestination = true,
            Speed = authoring.Speed,
            YOffset = 2,
            DesiredLocation = new Unity.Mathematics.int2(
                UnityEngine.Mathf.RoundToInt(authoring.transform.position.x),
                UnityEngine.Mathf.RoundToInt(authoring.transform.position.z)
            )
        });
        AddComponent(new DroneFindPlantIntent { });
        AddComponent(new PathfindingIntent
        {
            destinationType = PathfindingDestination.None,
            navigatorType = NavigatorType.Drone
        });
    }
}