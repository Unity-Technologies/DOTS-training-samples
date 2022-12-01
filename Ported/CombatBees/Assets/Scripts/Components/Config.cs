using Unity.Entities;
using Unity.Mathematics;

public struct Config : IComponentData
{
    public Entity beePrefab;
    public Entity particlePrefab;
    public int startBeeCount;
    public int beesPerResource;
    public float minimumBeeSize;
    public float maximumBeeSize;
    public float3 gravity;
    public float3 fieldSize;
    public Entity resourcePrefab;
    public int resourceCount;
    public float carryStiffness;

    // Bee parameters
    public float speedStretch;
    public float rotationStiffness;
    public float aggression;
    public float flightJitter;
    public float teamAttraction;
    public float teamRepulsion;
    public float damping;
    public float chaseForce;
    public float carryForce;
    public float grabDistance;
    public float attackDistance;
    public float attackForce;
    public float hitDistance;
}