using Unity.Entities;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityMeshRenderer = UnityEngine.MeshRenderer;

[GenerateAuthoringComponent]
public struct Lake : IComponentData
{
    public float Volume;
}
