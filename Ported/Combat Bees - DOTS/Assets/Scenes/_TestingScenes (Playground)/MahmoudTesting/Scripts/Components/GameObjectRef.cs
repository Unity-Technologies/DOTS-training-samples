using Unity.Entities;
using UnityEngine;
using UnityCamera = UnityEngine.Camera;

[GenerateAuthoringComponent]
public class GameObjectRef : IComponentData
{
    public UnityCamera Camera;
    public Material MarkerMaterial;
}

