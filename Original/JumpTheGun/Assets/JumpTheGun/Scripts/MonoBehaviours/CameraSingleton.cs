using Unity.Mathematics; 
using Unity.Entities;

class CameraSingleton : UnityEngine.MonoBehaviour 
{
    public static UnityEngine.Camera Instance;


    void Awake()
    {
        //state.RequireForUpdate<Config>();
        Instance = GetComponent<UnityEngine.Camera>();
    }

    void Update(){
        var config = SystemAPI.GetSingleton<Config>();
        config.rayDirection = ReturnRayDirection();
        config.rayOrigin = ReturnRayOrigin();
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