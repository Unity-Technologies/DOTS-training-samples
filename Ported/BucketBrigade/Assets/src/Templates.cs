using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Templates : MonoBehaviour {
    [Header("Use Context Menu to switch templates :)")]
    [Tooltip("Use Context Menu to switch templates :)")]
    [Multiline()]
    public string Instructions = "Use the context menu ^ to switch templates";
    private FireSim fs;

    public void DEFAULT_SETTINGS()
    {
        fs = FindObjectOfType<FireSim>();

        // Water
        fs.splashRadius = 4;
        fs.coolingStrength = 2;
        fs.coolingStrength_falloff = 4f;
        fs.totalBuckets = 20;
        fs.bucketCapacity = 3f;
        fs.bucketFillRate = 0.1f;
        fs.bucketSize_EMPTY = 0.2f;
        fs.bucketSize_FULL = 0.4f;

        // FIRE
        fs.startingFireCount = 10;
        fs.maxFlameHeight = 1;
        fs.cellSize = 0.3f;
        fs.rows = 50;
        fs.columns = 50;
        fs.flashpoint = 0.2f;
        fs.heatRadius = 2;
        fs.heatTransferRate = 0.0001f;

        // Bots

        fs.botSpeed = 0.075f;
        fs.waterCarryAffect = 0.5f;
        fs.total_OMNIBOTS = 0;
        fs.bucketChain_Configs = new List<BucketChain_Config>();

    }
    [ContextMenu("1: One Big Chain")]
    public void Template_OneBigChain(){
        DEFAULT_SETTINGS();
        fs.totalBuckets = 30;
        fs.heatTransferRate = 0.0003f;
        fs.AddBucketChain_CONFIG(15, 15);

        PrintSummary();
    }
    [ContextMenu("2: Two Big Chains")]
    public void Template_TwoBigChains()
    {
        DEFAULT_SETTINGS();
        fs.totalBuckets = 40;
        fs.heatTransferRate = 0.0003f;

        fs.AddBucketChain_CONFIG(15, 15);
        fs.AddBucketChain_CONFIG(15, 15);

        PrintSummary();
    }
    [ContextMenu("3: Lots Of Chains")]
    public void Template_LotsOfChains()
    {
        DEFAULT_SETTINGS();
        fs.totalBuckets = 50;
        fs.heatTransferRate = 0.0004f;
        fs.AddBucketChain_CONFIG(3, 3);
        fs.AddBucketChain_CONFIG(3, 3);
        fs.AddBucketChain_CONFIG(3, 3);
        fs.AddBucketChain_CONFIG(2, 2);
        fs.AddBucketChain_CONFIG(2, 2);
        fs.AddBucketChain_CONFIG(2, 2);
        fs.AddBucketChain_CONFIG(1, 1);
        fs.AddBucketChain_CONFIG(1, 1);
        fs.AddBucketChain_CONFIG(1, 1);
        fs.AddBucketChain_CONFIG(1, 1);

        PrintSummary();
    }
    [ContextMenu("4: Omnibots")]
    public void Template_omnibots()
    {
        DEFAULT_SETTINGS();
        fs.totalBuckets = 30;
        fs.fireSimUpdateRate = 0.01f;
        fs.heatTransferRate = 0.002f;
        fs.splashRadius = 6;
        fs.total_OMNIBOTS = 30;

        PrintSummary();
    }

    [ContextMenu("5: EL GIGANTE")]
    public void Template_GIGANTE()
    {
        DEFAULT_SETTINGS();
        fs.totalBuckets = 1;
        fs.heatTransferRate = 0.001f;
        fs.splashRadius = 30;
        fs.coolingStrength = 10f;
        fs.coolingStrength_falloff = 1f;
        fs.total_OMNIBOTS = 1;
        fs.bucketSize_EMPTY = 0.2f;
        fs.bucketSize_FULL = 3f;

        PrintSummary();
    }

    [ContextMenu("6: CPU KILLER (fire, no bots)")]
    public void Template_CPU_KILLER()
    {
        DEFAULT_SETTINGS();
        fs.totalBuckets = 0;
        fs.heatTransferRate = 0.01f;
        fs.cellSize = 0.1f;
        fs.rows = 200;
        fs.columns = 200;

        PrintSummary();
    }

    void PrintSummary(){
        Debug.Log("New Simulation Settings...");

        Debug.Log("splashRadius: "+fs.splashRadius+"| coolingStrength: "+fs.coolingStrength+"| coolingStrength_falloff: " + fs.coolingStrength_falloff);
        Debug.Log("Fires:"+fs.startingFireCount+" | flameHeight:"+fs.maxFlameHeight+" | cellSize:"+fs.cellSize+" | rows:" + fs.rows+" | columns:" + fs.columns);
        Debug.Log("flashpoint:" + fs.flashpoint+" | heatRadius:" + fs.heatRadius+" | heatTransferRate:" + fs.heatTransferRate);
        Debug.Log("Buckets:" + fs.totalBuckets + " | bucketCapacity:" + fs.bucketCapacity + " | bucketFillRate:" + fs.bucketFillRate + " | waterCarryAffect:" + fs.waterCarryAffect);
        Debug.Log("totalOmnibots:" + fs.total_OMNIBOTS);
        for (int i = 0; i < fs.bucketChain_Configs.Count; i++)
        {
            BucketChain_Config _Config = fs.bucketChain_Configs[i];
            Debug.Log("Chain " + i + ": passers_EMPTY ["+_Config.passers_EMPTY+"], passers_FULL["+_Config.passers_FULL+"]");
        }

    }
}
