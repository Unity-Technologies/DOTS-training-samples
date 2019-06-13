using Unity.Burst;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Jobs;

public class BeeManager : MonoBehaviour
{
    public static BeeManager S { get; private set; }

    private void Awake()
    {
        if (S == null) { S = this; }
        else { Debug.LogError("Attempt to create multiple BeeManagers."); }

        Rand.InitState();
    }

    [SerializeField]
    private int beesAtStart = 0;

    [SerializeField] [Range(0, 1)] private float damping = 0;
    public float Damping { get { return damping; } }

    [SerializeField] private float flightJitter = 0;
    public float FlightJitter { get { return flightJitter; } }

    [SerializeField] private float minBeeSize = 0;
    public float MinBeeSize { get { return minBeeSize; } }

    [SerializeField] private float maxBeeSize = 0;
    public float MaxBeeSize { get { return maxBeeSize; } }

    [SerializeField] private float chaseForce = 0;
    public float ChaseForce { get { return chaseForce; } }

    [SerializeField] private float attackDistance = 0;
    public float AttackDistance { get { return attackDistance; } }

    [SerializeField] private float attackForce = 0;
    public float AttackForce { get { return attackForce; } }

    [SerializeField] private float hitDistance = 0;
    public float HitDistance { get { return hitDistance; } }

    [SerializeField] private float teamAttraction = 0;
    public float TeamAttraction { get { return teamAttraction; } }

    [SerializeField] private float teamRepulsion = 0;
    public float TeamRepulsion { get { return teamRepulsion; } }

    [SerializeField] private float speedStretch = 0;
    public float SpeedStretch { get { return speedStretch; } }

    [SerializeField] [Range(0, 1)] private float aggression = 0;
    public float Aggression { get { return aggression; } }

    [SerializeField] private float carryForce = 0;
    public float CarryForce { get { return carryForce; } }

    [SerializeField] private float grabDistance = 0;
    public float GrabDistance { get { return grabDistance; } }

    [SerializeField] private float beeTimeToDeath = 0;
    public float BeeTimeToDeath { get { return beeTimeToDeath; } }

    public Transform PurpleSpawnPoint;
    public Transform YellowSpawnPoint;


    [HideInInspector]
    public Unity.Mathematics.Random Rand;

    public GameObject YellowBeePrefab;
    public GameObject PurpleBeePrefab;
    public GameObject SpawnerPrefab;

    public Entity YellowBeeEnt;
    public Entity PurpleBeeEnt;
    public Entity SpawnerEnt;

    // Start is called before the first frame update
    void Start()
    {
        EntityManager entityManager = World.Active.EntityManager;
        
        YellowBeeEnt = GameObjectConversionUtility.ConvertGameObjectHierarchy(YellowBeePrefab, World.Active);
        PurpleBeeEnt = GameObjectConversionUtility.ConvertGameObjectHierarchy(PurpleBeePrefab, World.Active);

        SpawnerEnt = GameObjectConversionUtility.ConvertGameObjectHierarchy(SpawnerPrefab, World.Active);

        C_Spawner yellowSpawnData = new C_Spawner()
        {
            Count = beesAtStart / 2,
            Prefab = YellowBeeEnt
        };
        C_Spawner purpleSpawnData = new C_Spawner()
        {
            Count = beesAtStart / 2,
            Prefab = PurpleBeeEnt
        };

        //Spawn yellows
        var spawner = entityManager.CreateEntity();

        entityManager.AddComponentData(spawner, yellowSpawnData);
        entityManager.AddComponentData(spawner, new Unity.Transforms.LocalToWorld() { Value = YellowSpawnPoint.localToWorldMatrix });

        //Spawn purples
        spawner = entityManager.CreateEntity();

        entityManager.AddComponentData(spawner, purpleSpawnData);
        entityManager.AddComponentData(spawner, new Unity.Transforms.LocalToWorld() { Value = PurpleSpawnPoint.localToWorldMatrix });
    }
}
