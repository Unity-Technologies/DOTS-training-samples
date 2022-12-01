using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

class ConfigAuthoring : MonoBehaviour
{
    public GameObject beePrefab;
    public GameObject particlePrefab;
    public int startBeeCount;
    public int beesPerResource;
    public float minimumBeeSize;
    public float maximumBeeSize;
    public Vector3 gravity = new Vector3(0f, -9.81f, 0f);
    public Renderer fieldRenderer;
    [FormerlySerializedAs("ResourcePrefab")] public GameObject resourcePrefab;
    [FormerlySerializedAs("ResourceCount")] public int resourceCount = 100;
    public float carryStiffness = 15;
    
    [Header("Bee Parameters")]
    public float speedStretch = .2f;
    public float rotationStiffness = 5f;
    [Space(10)]
    [Range(0f,1f)]
    public float aggression = .5f;
    public float flightJitter = 200f;
    public float teamAttraction = 5f;
    public float teamRepulsion = 4f;
    [Range(0f,1f)]
    public float damping = .1f;
    public float chaseForce = 50f;
    public float carryForce = 25f;
    public float grabDistance = .5f;
    public float attackDistance = 4f;
    public float attackForce = 500f;
    public float hitDistance = .5f;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config()
        {
            beePrefab = GetEntity(authoring.beePrefab),
            particlePrefab = GetEntity(authoring.particlePrefab),
            startBeeCount = authoring.startBeeCount,
            beesPerResource = authoring.beesPerResource,
            minimumBeeSize = authoring.minimumBeeSize,
            maximumBeeSize = authoring.maximumBeeSize,
            gravity = authoring.gravity,
            fieldSize = authoring.fieldRenderer.bounds.extents * 2f,
            resourcePrefab = GetEntity(authoring.resourcePrefab),
            resourceCount = authoring.resourceCount,
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
            attackDistance = authoring.attackDistance,
            attackForce = authoring.attackForce,
            hitDistance = authoring.hitDistance,
            carryStiffness = authoring.carryStiffness
        });
        AddBuffer<AvailableResources>();
    }
}