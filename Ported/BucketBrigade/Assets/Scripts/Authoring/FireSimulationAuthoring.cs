using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;

public class FireSimulationAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [Header("FIRE")]
    [Tooltip("Prefabs / FlameCell")]
    public GameObject prefab_flameCell;
    [Tooltip("How many random fires do you want to battle?")]
    public int startingFireCount = 1;
    [Tooltip("How high the flames reach at max temperature")]
    public float maxFlameHeight = 0.1f;
    [Tooltip("Size of an individual flame. Full grid will be (rows * cellSize)")]
    public float cellSize = 0.05f;
    [Tooltip("How many cells WIDE the simulation will be")]
    public int rows = 20;
    [Tooltip("How many cells DEEP the simulation will be")]
    public int columns = 20;
    [Tooltip("When temperature reaches *flashpoint* the cell is on fire")]
    public float flashpoint = 0.5f;
    [Tooltip("How far does heat travel? Note: Higher heat radius significantly increases CPU usage")]
    public int heatRadius = 1;
    [Tooltip("How fast will adjacent cells heat up?")]
    public float heatTransferRate = 0.7f;
    [Range(0.0001f, 2f)]
    [Tooltip("How often the fire cells update. 1 = once per second. Lower = faster")]
    public float fireSimUpdateRate = 0.5f;

    [Header("Colors")]
    // cell colours
    public Color fireCellColorNeutral = new Color(0.4888f, 0.7924f, 0.4597f);
    public Color fireCellColorCool = new Color(1.0f, 0.8884f, 0.5141f);
    public Color fireCellColorHot = new Color(1.0f, 0.0f, 0.0f);
    // bot colours
    public Color botColorScoop = new Color(0.0f, 1.0f, 0.1510f);
    public Color botColorPassFull = new Color(0.7729f, 0.9245f, 0.7370f);
    public Color botColorPassEmpty = new Color(0.9339f, 0.7533f, 0.9255f);
    public Color botColorThrow = new Color(1.0f, 0.4764f, 0.9354f);
    public Color botColorOmnibot = new Color(0.0f, 0.0f, 0.0f);
    // bucket Colours
    public Color bucketColorEmpty = new Color(1.0f, 0.4103f, 0.4589f);
    public Color bucketColorFull = new Color(0.0f, 0.9797f, 1.0f);

    public float bucketSizeEmpty = 0.2f;
    public float bucketSizeFull = 0.8f;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(prefab_flameCell);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new FireSimulation
        {
            fireCellPrefab = conversionSystem.GetPrimaryEntity(prefab_flameCell),

            startingFireCount = startingFireCount,
            maxFlameHeight = maxFlameHeight,
            cellSize = cellSize,
            rows = rows,
            columns = columns,
            flashpoint = flashpoint,
            heatRadius = heatRadius,
            heatTransferRate = heatTransferRate,
            fireSimUpdateRate = fireSimUpdateRate,

            fireCellColorNeutral = fireCellColorNeutral,
            fireCellColorCool = fireCellColorCool,
            fireCellColorHot = fireCellColorHot,
            botColorScoop = botColorScoop,
            botColorPassFull = botColorPassFull,
            botColorPassEmpty = botColorPassEmpty,
            botColorThrow = botColorThrow,
            botColorOmnibot = botColorOmnibot,
            bucketColorEmpty = bucketColorEmpty,
            bucketColorFull = bucketColorFull,
            
            bucketSizeEmpty = bucketSizeEmpty,
            bucketSizeFull = bucketSizeFull,
        });

        dstManager.AddBuffer<SimulationTemperature>(entity);
    }
}

public struct FireSimulation : IComponentData
{
    public Entity fireCellPrefab;

    public int startingFireCount;
    public float maxFlameHeight;
    public float cellSize;
    public int rows;
    public int columns;
    private float simulation_WIDTH, simulation_DEPTH;
    public float flashpoint;
    public int heatRadius;
    public float heatTransferRate;
    public float fireSimUpdateRate;

    public Color fireCellColorNeutral;
    public Color fireCellColorCool;
    public Color fireCellColorHot;
    public Color botColorScoop;
    public Color botColorPassFull;
    public Color botColorPassEmpty;
    public Color botColorThrow;
    public Color botColorOmnibot;
    public Color bucketColorEmpty;
    public Color bucketColorFull;

    public float bucketSizeEmpty;
    public float bucketSizeFull;
}

public struct SimulationTemperature : IBufferElementData
{
    float Value;

    public static implicit operator float(SimulationTemperature e)
    {
        return e.Value;
    }

    public static implicit operator SimulationTemperature(float e)
    {
        return new SimulationTemperature { Value = e };
    }
}

