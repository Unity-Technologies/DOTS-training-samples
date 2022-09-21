using Unity.Entities;

struct BeeConfig : IComponentData
{
    //public Mesh beeMesh;
    //public Material beeMaterial;
    //public Color[] teamColors;
    public float minBeeSize;
    public float maxBeeSize;
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
    public float maxSpawnSpeed;
    public int startBeeCount;

    public Entity beePrefab;
}
