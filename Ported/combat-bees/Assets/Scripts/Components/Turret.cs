using Unity.Entities;

//@rename Turret_Step1 Turret

#region step1
// An empty component is called a "tag component".
struct Turret_Step1 : IComponentData
{
}
#endregion

#region step2
struct Turret : IComponentData
{
    // This entity will reference the nozzle of the cannon, where cannon balls should be spawned.
    public Entity CannonBallSpawn;

    // This entity will reference the prefab to be spawned every time the cannon shoots.
    public Entity CannonBallPrefab;
}
#endregion
