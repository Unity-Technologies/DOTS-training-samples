using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityCamera = UnityEngine.Camera;

// Pay attention that this component is a class, in other words it's a managed component.
// Please check the package documentation to understand the implications of this.
[GenerateAuthoringComponent]
public class GameObjectRefs : IComponentData
{
    public Config Config;

    [Header("Camera")]
    public UnityCamera Camera;
    public float3 CameraOffset;
    public float CameraInitialDistance = 3f;
    
    [Header("Prefabs")]
    public Entity BoxPrefab;
    public Entity PlayerPrefab;
    public Entity TankPrefab;
    public Entity CannonballPrefab;
}
