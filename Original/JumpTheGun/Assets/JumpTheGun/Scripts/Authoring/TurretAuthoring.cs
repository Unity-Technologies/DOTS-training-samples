using Unity.Entities;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
class TurretAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject cannonBallPrefab;
    public UnityEngine.Transform cannonBallSpawn;
}

// Bakers convert authoring MonoBehaviours into entities and components.
class TurretBaker : Baker<TurretAuthoring>
{
    public override void Bake(TurretAuthoring authoring)
    {
        //AddComponent<Turret>();

        AddComponent(new Turret
        {
            cannonBall = GetEntity(authoring.cannonBallPrefab),
            cannonBallSpawn = GetEntity(authoring.cannonBallSpawn)
        }) ;
    }
}