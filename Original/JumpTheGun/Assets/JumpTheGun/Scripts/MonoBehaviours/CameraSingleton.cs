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
    
    void OnUpdate()
    {

    }

    public float3 ReturnRayOrigin(){
        var ray = Instance.ScreenPointToRay(UnityEngine.Input.mousePosition);
        return ray.origin; 
    }
    public float3 ReturnRayDirection(){
        var ray = Instance.ScreenPointToRay(UnityEngine.Input.mousePosition);
        return ray.direction; 
    }


}