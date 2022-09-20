using Unity.Entities;

//@rename TurretAuthoring_Step1 TurretAuthoring
//@rename TurretBaker_Step1 TurretBaker
//@rename Turret_Step1 Turret

//@rename TurretAuthoring_Step2 TurretAuthoring
//@rename TurretBaker_Step2 TurretBaker

#region step1
// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
class TurretAuthoring_Step1 : UnityEngine.MonoBehaviour
{
}

#endregion

#region step2
class TurretAuthoring_Step2 : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject CannonBallPrefab;
    public UnityEngine.Transform CannonBallSpawn;
}

#endregion

#region step3
class TurretAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject CannonBallPrefab;
    public UnityEngine.Transform CannonBallSpawn;
}

#endregion

#region step1
// Bakers convert authoring MonoBehaviours into entities and components.
class TurretBaker_Step1 : Baker<TurretAuthoring_Step1>
{
    public override void Bake(TurretAuthoring_Step1 authoring)
    {
        AddComponent<Turret_Step1>();
    }
}
#endregion

// TODO - the Shooting component should be added disabled https://jira.unity3d.com/browse/DOTS-6389
#region step2
class TurretBaker_Step2 : Baker<TurretAuthoring_Step2>
{
    public override void Bake(TurretAuthoring_Step2 authoring)
    {
        AddComponent(new Turret
        {
            // By default, each authoring GameObject turns into an Entity.
            // Given a GameObject (or authoring component), GetEntity looks up the resulting Entity.
            CannonBallPrefab = GetEntity(authoring.CannonBallPrefab),
            CannonBallSpawn = GetEntity(authoring.CannonBallSpawn)
        });
    }
}
#endregion

#region step3
class TurretBaker : Baker<TurretAuthoring>
{
    public override void Bake(TurretAuthoring authoring)
    {
        AddComponent(new Turret
        {
            CannonBallPrefab = GetEntity(authoring.CannonBallPrefab),
            CannonBallSpawn = GetEntity(authoring.CannonBallSpawn)
        });

        // Enableable components are always initially enabled.
        AddComponent<Shooting>();
    }
}
#endregion