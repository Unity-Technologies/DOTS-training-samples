using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
class CameraSingleton : MonoBehaviour
{
    public static UnityEngine.Camera Instance;

    void Awake()
    {
        //state.RequireForUpdate<Config>();
        Instance = GetComponent<UnityEngine.Camera>();
    }
}