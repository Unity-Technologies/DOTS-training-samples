using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class GameObjectRefs : IComponentData
{
    public Camera Camera;
}