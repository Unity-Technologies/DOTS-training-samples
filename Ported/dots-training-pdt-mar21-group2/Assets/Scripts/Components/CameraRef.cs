using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
// Way to get managed Camera
public class CameraRef : IComponentData
{
    public Camera Camera;
}
