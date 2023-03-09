using System.Collections;
using System.Collections.Generic;
using Authoring;
using UnityEngine;

[ExecuteInEditMode]
public class Templates : MonoBehaviour {
    [Header("Use Context Menu to switch templates :)")]
    [Tooltip("Use Context Menu to switch templates :)")]
    [Multiline()]
    public string Instructions = "Use the context menu ^ to switch templates";
    private ConfigAuthoring config;

    public void DEFAULT_SETTINGS()
    {
        config = FindObjectOfType<ConfigAuthoring>();

        // Water
        config.splashRadius = 4;
        config.coolingStrength = 2;
        config.coolingStrengthFalloff = 4f;
        config.totalBuckets = 20;
        config.bucketCapacity = 3f;
        config.bucketFillRate = 0.1f;
        config.bucketSizeEmpty = 0.2f;
        config.bucketSizeFull = 0.4f;

        // FIRE
        config.startingFireCount = 10;
        config.maxFlameHeight = 1;
        config.cellSize = 0.3f;
        config.numRows = 50;
        config.numColumns = 50;
        config.flashpoint = 0.2f;
        config.heatRadius = 2;
        config.heatTransferRate = 0.0001f;

        // Bots

        config.botSpeed = 0.075f;
        config.waterCarryEffect = 0.5f;
        config.numOmnibots = 0;
        // config.bucketChain_Configs = new List<BucketChain_Config>();

    }
    [ContextMenu("1: One Big Chain")]
    public void Template_OneBigChain(){
        DEFAULT_SETTINGS();
        config.totalBuckets = 30;
        config.heatTransferRate = 0.0003f;
        // config.AddBucketChain_CONFIG(15, 15);

        PrintSummary();
    }
    [ContextMenu("2: Two Big Chains")]
    public void Template_TwoBigChains()
    {
        DEFAULT_SETTINGS();
        config.totalBuckets = 40;
        config.heatTransferRate = 0.0003f;

        // config.AddBucketChain_CONFIG(15, 15);
        // config.AddBucketChain_CONFIG(15, 15);

        PrintSummary();
    }
    [ContextMenu("3: Lots Of Chains")]
    public void Template_LotsOfChains()
    {
        DEFAULT_SETTINGS();
        config.totalBuckets = 50;
        config.heatTransferRate = 0.0004f;
        // config.AddBucketChain_CONFIG(3, 3);
        // config.AddBucketChain_CONFIG(3, 3);
        // config.AddBucketChain_CONFIG(3, 3);
        // config.AddBucketChain_CONFIG(2, 2);
        // config.AddBucketChain_CONFIG(2, 2);
        // config.AddBucketChain_CONFIG(2, 2);
        // config.AddBucketChain_CONFIG(1, 1);
        // config.AddBucketChain_CONFIG(1, 1);
        // config.AddBucketChain_CONFIG(1, 1);
        // config.AddBucketChain_CONFIG(1, 1);

        PrintSummary();
    }
    [ContextMenu("4: Omnibots")]
    public void Template_omnibots()
    {
        DEFAULT_SETTINGS();
        config.totalBuckets = 30;
        config.fireSimUpdateRate = 0.01f;
        config.heatTransferRate = 0.002f;
        config.splashRadius = 6;
        config.numOmnibots = 30;

        PrintSummary();
    }

    [ContextMenu("5: EL GIGANTE")]
    public void Template_GIGANTE()
    {
        DEFAULT_SETTINGS();
        config.totalBuckets = 1;
        config.heatTransferRate = 0.001f;
        config.splashRadius = 30;
        config.coolingStrength = 10f;
        config.coolingStrengthFalloff = 1f;
        config.numOmnibots = 1;
        config.bucketSizeEmpty = 0.2f;
        config.bucketSizeFull = 3f;

        PrintSummary();
    }

    [ContextMenu("6: CPU KILLER (fire, no bots)")]
    public void Template_CPU_KILLER()
    {
        DEFAULT_SETTINGS();
        config.totalBuckets = 0;
        config.heatTransferRate = 0.01f;
        config.cellSize = 0.1f;
        config.numRows = 200;
        config.numColumns = 200;

        PrintSummary();
    }

    void PrintSummary(){
        Debug.Log("New Simulation Settings...");

        Debug.Log("splashRadius: "+config.splashRadius+"| coolingStrength: "+config.coolingStrength+"| coolingStrength_falloff: " + config.coolingStrengthFalloff);
        Debug.Log("Fires:"+config.startingFireCount+" | flameHeight:"+config.maxFlameHeight+" | cellSize:"+config.cellSize+" | rows:" + config.numRows+" | columns:" + config.numColumns);
        Debug.Log("flashpoint:" + config.flashpoint+" | heatRadius:" + config.heatRadius+" | heatTransferRate:" + config.heatTransferRate);
        Debug.Log("Buckets:" + config.totalBuckets + " | bucketCapacity:" + config.bucketCapacity + " | bucketFillRate:" + config.bucketFillRate + " | waterCarryAffect:" + config.waterCarryEffect);
        Debug.Log("totalOmnibots:" + config.numOmnibots);
        // for (int i = 0; i < config.bucketChain_Configs.Count; i++)
        // {
        //     BucketChain_Config _Config = config.bucketChain_Configs[i];
        //     Debug.Log("Chain " + i + ": passers_EMPTY ["+_Config.passers_EMPTY+"], passers_FULL["+_Config.passers_FULL+"]");
        // }

    }
}
