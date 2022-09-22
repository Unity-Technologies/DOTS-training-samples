using Unity.Entities;

class BeeConfigAuthoring : UnityEngine.MonoBehaviour
{
    public float minBeeSize = 0.25f;
    public float maxBeeSize = 0.5f;
    public float speedStretch = 0.2f;
    public float rotationStiffness = 5f;
    [UnityEngine.Range(0f, 1f)]
    public float aggression = .5f;
    public float flightJitter = 200f;
    public float teamAttraction = 5;
    public float teamRepulsion = 4;
    [UnityEngine.Range(0f, 1f)]
    public float damping = 0.1f;
    public float chaseForce = 50f;
    public float carryForce = 25f;
    public float grabDistance = 0.5f;
    public float attackDistance = 4f;
    public float attackForce = 500;
    public float hitDistance = 0.5f;
    public float maxSpawnSpeed = 75f;
    public int startBeeCount= 100;

    public UnityEngine.GameObject beePrefab;
}

class BeeConfigBaker : Baker<BeeConfigAuthoring>
{
    public override void Bake(BeeConfigAuthoring authoring)
    {
        AddComponent(new BeeConfig
        {
            minBeeSize = authoring.minBeeSize,
            maxBeeSize = authoring.maxBeeSize,
            speedStretch = authoring.speedStretch,
            rotationStiffness = authoring.rotationStiffness,
            aggression = authoring.aggression,
            flightJitter = authoring.flightJitter,
            teamAttraction = authoring.teamAttraction,
            teamRepulsion = authoring.teamRepulsion,
            damping = authoring.damping,
            chaseForce = authoring.chaseForce,
            carryForce = authoring.carryForce,
            grabDistance = authoring.grabDistance,
            attackDistance = authoring.grabDistance,
            attackForce = authoring.attackForce,
            hitDistance = authoring.hitDistance,
            maxSpawnSpeed = authoring.maxSpawnSpeed,
            startBeeCount = authoring.startBeeCount,
            beePrefab = GetEntity(authoring.beePrefab)
    });
    }
}
