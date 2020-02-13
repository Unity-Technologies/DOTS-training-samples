using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Metro : MonoBehaviour
{
    public static float CUSTOMER_SATISFACTION = 1f;
    public static float BEZIER_HANDLE_REACH = 0.1f;
    public static float BEZIER_PLATFORM_OFFSET = 3f;
    public static float PLATFORM_ADJACENCY_LIMIT = 12f;
    public const int BEZIER_MEASUREMENT_SUBDIVISIONS = 2;
    public const float PLATFORM_ARRIVAL_THRESHOLD = 0.975f;
    public const float RAIL_SPACING = 0.5f;
    public static Metro INSTANCE()
    {
        if(cachedInstance == null)
        {
            cachedInstance = FindObjectOfType<Metro>();
        }
        return cachedInstance;
    }

    static Metro cachedInstance = null;

    // PUBLICS
    [Tooltip("prefabs/Carriage")]
    public GameObject prefab_trainCarriage;
    [Tooltip("prefabs/Platform")]
    public GameObject prefab_platform;
    [Tooltip("prefabs/Commuter")]
    public GameObject prefab_commuter;
    [Tooltip("prefabs/Rail")]
    public GameObject prefab_rail;
    [Tooltip("Draw rail paths (eats CPU)")]
    public bool drawRailBeziers = false;
    [Tooltip("Number of commuters to spawn at the start")]
    public int maxCommuters = 2000;
    [Tooltip("Affects rail curve sharpness. 0 = no rounding, 1 = madness. Good value = 0.2 ish")]
    [Range(0f, 1f)] public float Bezier_HandleReach = 0.3f;
    [HideInInspector]
    public float Bezier_PlatformOffset = 3f;
    [Header("Trains")] public float Train_accelerationStrength = 0.001f;
    [Tooltip("How quickly trains lose speed")]
    public float Train_railFriction = 0.99f;
    [Tooltip("Once train has arrived, how long (in seconds) until doors open")]
    public float Train_delay_doors_OPEN = 2f;
    [Tooltip("Train load/unload is complete. Wait (X seconds) before closing doors")]
    public float Train_delay_doors_CLOSE = 1f;
    [Tooltip("Doors have closed, wait (X seconds) before departing")]
    public float Train_delay_departure = 1f;

    [Header("Commuters")]
    // walk speed etc
    [Header("MetroLines")]
    public string[] LineNames;
    public int[] maxTrains;
    public int[] carriagesPerTrain;
    public float[] maxTrainSpeed;
    private int totalLines = 0;
    public Color[] LineColours;

    private void Awake()
    {
    }

    private void Start()
    {
      
    }

    private void Update()
    {
       
    }
}