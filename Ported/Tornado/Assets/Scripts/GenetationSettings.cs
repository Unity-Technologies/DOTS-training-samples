using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class GenerationSetting : IComponentData
{
    public Material barMaterial;
    public Mesh barMesh;
    [Range(0f,1f)]
    public float damping;
    [Range(0f,1f)]
    public float friction;
    public float breakResistance;
    public float expForce;
    [Range(0f,1f)]
    public float tornadoForce;
    public float tornadoMaxForceDist;
    public float tornadoHeight;
    public float tornadoUpForce;
    public float tornadoInwardForce;
}
