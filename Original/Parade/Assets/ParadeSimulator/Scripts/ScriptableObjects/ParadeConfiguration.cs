using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Container for all Parade test parameters. 
/// </summary>
[Serializable]
[CreateAssetMenu(menuName = "Parade Configuration")]
public class ParadeConfiguration : ScriptableObject {

    [Header("Configuration Basics")]
    [SerializeField]
    private string configName = "My Config";
    public string ConfigName {
        get { return configName; }
    }

    [SerializeField, Tooltip("If true, camera can be moved with mouse to look around")]
    private bool enableMouseLook = true;
    public bool EnableMouseLook {
        get { return enableMouseLook; }
    }

    [SerializeField, Tooltip("If true, basic UI readout will be visible during test")]
    private bool enableTestUI = true;
    public bool EnableTestUI {
        get { return enableTestUI; }
    }

    [SerializeField, Tooltip("Number of blocks in the test run (0 blocks will do infinite run)")]
    private int numberOfTestBlocks = 0;
    public int NumberOfTestBlocks {
        get { return numberOfTestBlocks; }
    }

    [SerializeField, Tooltip("Number of blocks to 'pre-load' in front of start position (counts toward total test block count). Default is 5.")]
    private int leadingBlocks = 5;
    public int LeadingBlocks {
        get { return leadingBlocks; }
    }

    [SerializeField, Tooltip("Distance behind camera objects need to be before they are considered 'out of view' and will be cleaned up. Default is 50.")]
    private float rearCleanupDistance = 50.0f;
    public float RearCleanupDistance {
        get { return rearCleanupDistance; }
    }

    [SerializeField, Tooltip("The speed at which the camera will move down the street")]
    private float cameraSpeed = 5.0f;
    public float CameraSpeed {
        get { return cameraSpeed; }
    }

    [Header("City Block Complexity")]
    [SerializeField, Tooltip("If true, roads will be generated for each block")]
    private bool generateRoads = true;
    public bool GenerateRoads {
        get { return generateRoads; }
    }

    [SerializeField, Tooltip("If true, buildings will be generated for each block")]
    private bool generateBuildings = true;
    public bool GenerateBuildings {
        get { return generateBuildings; }
    }

    [SerializeField, Tooltip("If true, crowds will be generated for each block")]
    private bool generateCrowds = true;
    public bool GenerateCrowds {
        get { return generateCrowds; }
    }

    [Header("Crowd Configuration")]
    [Range(1,6), SerializeField, Tooltip("Density of crowd. 1 is lowest, 6 is highest.")]
    private int crowdDensity = 6;
    public int CrowdDensity {
        get { return crowdDensity; }
    }

    [Range(1, 50), SerializeField, Tooltip("1 in N chance a person in the crowd will be holding a balloon")]
    private int balloonChance = 50;
    public int BalloonChance {
        get { return balloonChance; }
    }

    [Range(1, 500), SerializeField, Tooltip("1 in N chance (per frame) a person holding a balloon will accidentally let it go.")]
    private int balloonLetGoChance = 500;
    public int BalloonLetGoChance {
        get { return balloonLetGoChance; }
    }

    [Range(1, 50), SerializeField, Tooltip("1 in N chance a person will be willing to strike up conversation if they are not otherwise distracted or talking already.")]
    private int talkChance = 5;
    public int TalkChance {
        get { return talkChance; }
    }

    [SerializeField, Tooltip("If true, each Person in the crowd will be given randomized behaviour offset, so crowd won't all be on the 'same clock'.")]
    private bool randomizedTimeOffset = true;
    public bool RandomizedTimeOffset {
        get { return randomizedTimeOffset; }
    }

    [MinMaxRange(0.0f, 10.0f), SerializeField, Tooltip("Frequency Range (in seconds) at which a Person may become distracted.")]
    private MinMaxRange distractionCheckFrequency = new MinMaxRange(0.25f, 1.5f);
    public MinMaxRange DistractionCheckFrequency {
        get { return distractionCheckFrequency; }
    }

    [MinMaxRange(0.0f, 10.0f), SerializeField, Tooltip("Frequency Range (in seconds) for which a Person can be distracted.")]
    private MinMaxRange distractionDuration = new MinMaxRange(1.5f, 3.0f);
    public MinMaxRange DistractionDuration {
        get { return distractionDuration; }
    }

    [MinMaxRange(0.0f, 10.0f), SerializeField, Tooltip("Frequency Range (in seconds) at which a Person will randomly change behaviour (e.g. Go from watching parade to chatting to their neighbour).")]
    private MinMaxRange behaviourChangeInterval = new MinMaxRange(1.0f, 5.0f);
    public MinMaxRange BehaviourChangeInterval {
        get { return behaviourChangeInterval; }
    }

    [Header("Building Configuration")]

    [SerializeField, Tooltip("If true, buildings will be fancy (have windows, doors, etc.)")]
    private bool makeFancyBuildings = false;
    public bool MakeFancyBuildings {
        get { return makeFancyBuildings; }
    }

    [MinMaxRange(1, 10), SerializeField, Tooltip("Range of storeys each building can have (randomly chosen)")]
    private MinMaxRange numberOfBuildingStoreys = new MinMaxRange(1, 3);
    public MinMaxRange NumberOfBuildingStoreys {
        get { return numberOfBuildingStoreys; }
    }

}
