using Unity.Entities;
using UnityCamera = UnityEngine.Camera;

[GenerateAuthoringComponent]
public class GameObjectRefs : IComponentData
{
    public UnityCamera Camera;

}
