using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[ExecuteAlways]
[UpdateInGroup(typeof(PresentationSystemGroup))]
class SimpleMeshRendererSystem : ComponentSystem
{
    override protected void OnUpdate()
    {
        Entities.ForEach((SimpleMeshRenderer renderer, ref LocalToWorld localToWorld) =>
        {
            Graphics.DrawMesh(renderer.Mesh, localToWorld.Value, renderer.Material, 0);
        });
    }
}