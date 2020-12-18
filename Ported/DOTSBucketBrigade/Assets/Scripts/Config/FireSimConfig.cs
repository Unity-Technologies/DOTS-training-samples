using UnityEngine;
using Unity.Mathematics;

public class FireSimConfig : MonoBehaviour
{
	static public int xDim = 370;
	static public int yDim = 370;
    static public float heatTransferRate = 0.05f;
	static public int heatRadius = 1;
	static public float flashPoint = 0.5f;
	static public float fireThreshold = 0.2f;

    static public int numFireStarters = 20;
    static public int numWaterSources = 5;
    static public float bucketFillRate = 0.02f;

    static public int maxTeams = 5;
    static public float throwerSpeed = 2.0f;
    static public float kBucketSpeed = 1.0f / 16.0f;

    static public int numEmptyBots = 10;
    static public int numFullBots = 10;

    static public float4 emptyBotColor = new float4(0,0,0.5f,1f);
    static public float4 fullBotColor = new float4(0,0,1f,1f);

    static public float4 color_watersource = new float4(0.0f, 0.0f, 1.0f, 1.0f);
    static public float4 color_ground = new float4(0.0f, 0.8f, 0.2f, 1.0f);
    static public float4 color_fire_low = new float4(0.5f, 0.0f, 0.0f, 1.0f);
    static public float4 color_fire_high = new float4(1.0f, 0.3f, 0.0f, 1.0f);
    static public float4 color_fire_high1 = new float4(0.9f, 0.2f, 0.0f, 1.0f);
    static public float4 color_fire_high2 = new float4(0.8f, 0.1f, 0.0f, 1.0f);
    static public float4 color_fire_high3 = new float4(0.7f, 0.0f, 0.0f, 1.0f);
    static public float4 color_fire_high4 = new float4(0.6f, 0.0f, 0.0f, 1.0f);
    //static public Color color_ground = Color.green;
    //static public Color color_watersource = Color.blue; // new Color(0, 0, 0.5f);
    //static public Color color_fire_low = new Color(0.5f, 0, 0);
    //static public Color color_fire_high = new Color(1.0f, 0.3f, 0);
    static public Color color_bucket_empty = Color.black;
    static public Color color_bucket_full = Color.blue;
    static public Color color_role_scooper = Color.yellow;
    static public Color color_role_thrower = Color.white;
    static public Color color_role_passer_full = Color.cyan;
    static public Color color_role_passer_empty = Color.gray;
};
