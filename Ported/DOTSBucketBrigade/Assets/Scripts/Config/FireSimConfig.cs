using UnityEngine;

public class FireSimConfig : MonoBehaviour
{
	static public int xDim = 50;
	static public int yDim = 50;
    static public float heatTransferRate = 0.05f;
	static public int heatRadius = 1;
	static public float flashPoint = 0.5f;
	static public float fireThreshold = 0.2f;

    static public int numFireStarters = 10;
    static public int numWaterSources = 5;

    static public int maxTeams = 2;
    static public float throwerSpeed = 2.0f;

    static public Color color_ground = Color.green;
    static public Color color_watersource = Color.blue; // new Color(0, 0, 0.5f);
    static public Color color_fire_low = new Color(0.5f, 0, 0);
    static public Color color_fire_high = Color.red;
    static public Color color_bucket_empty = Color.black;
    static public Color color_bucket_full = Color.blue;
    static public Color color_role_scooper = Color.yellow;
    static public Color color_role_thrower = Color.white;
    static public Color color_role_passer_full = Color.cyan;
    static public Color color_role_passer_empty = Color.gray;
};
