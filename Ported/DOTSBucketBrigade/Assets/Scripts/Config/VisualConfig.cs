using UnityEngine;
using Unity.Mathematics;

public class VisualConfig : MonoBehaviour
{
    static public float kSplayMin = 0.125f;
    static public float kSplayMax = 1.0f;
    static public float kSplayStart = 0.0f;
    static public float kSplayEnd = 40.0f;
    
    static public float4 kEmptyBotColor = new float4(0,0,0.25f,1f);
    static public float4 kFullBotColor = new float4(0,0,1f,1f);
};
