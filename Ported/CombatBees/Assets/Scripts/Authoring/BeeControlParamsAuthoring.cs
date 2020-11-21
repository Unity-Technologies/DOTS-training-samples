using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BeeControlParamsAuthoring : IComponentData
{

    //public Mesh beeMesh;
    //public Material beeMaterial;
    //public Color[] teamColors;
    public float minBeeSize;
    public float maxBeeSize;
    public float speedStretch;
    public float rotationStiffness;
    [Space(10)]
    [Range(0f, 1f)]
    public float aggression;
    public float flightJitter;
    public float teamAttraction;
    public float teamRepulsion;
    [Range(0f, 1f)]
    public float damping;
    public float chaseForce;
    public float carryForce;
    public float grabDistance;
    public float attackDistance;
    public float attackForce;
    public float hitDistance;
    public float maxSpawnSpeed;
    [Space(10)]
    public int maxBeeCount;
}
