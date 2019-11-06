using ECSExamples;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class ArrowRenderingSystem : ComponentSystem
{
    private Material[] m_HomebaseMats;
    private GameObject[] m_Cursors;
    private Canvas m_Canvas;

    protected override void OnCreate()
    {
        m_Canvas = GameObject.FindObjectOfType<Canvas>();
    }

    public struct InitializedHomebase : IComponentData
    {}

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref ArrowComponent arrow, ref Translation position) =>
        {
            var hoverOverlay = GetSingletonEntity<HoverOverlayComponentTag>();
            PostUpdateCommands.SetComponent(hoverOverlay, new Translation { Value = position.Value + new float3(0,0.53f,0)});
            var rotation = quaternion.RotateX(math.PI / 2);
            switch (arrow.Direction) {
                case Direction.South:
                    rotation = math.mul(rotation, quaternion.RotateZ(math.PI));
                    break;
                case Direction.East:
                    rotation = math.mul(rotation, quaternion.RotateZ(3*math.PI/2));
                    break;
                case Direction.West:
                    rotation = math.mul(rotation, quaternion.RotateZ(math.PI/2));
                    break;
            }
            PostUpdateCommands.SetComponent(hoverOverlay, new Rotation{Value = rotation});
            PostUpdateCommands.RemoveComponent<ArrowComponent>(entity);
        });

        // TODO: Move elsewhere or rename this file
        Entities.WithNone<InitializedHomebase>().ForEach((Entity entity, ref HomebaseComponent home) =>
        {
            if ((int)home.Color.x == 0 && (int)home.Color.y == 0 && (int)home.Color.z == 0 && (int)home.Color.w == 0)
                return;
            var linkedEntityBuffer = EntityManager.GetBuffer<LinkedEntityGroup>(entity);
            var colorIndex = home.PlayerId - 1;
            var sharedMesh = EntityManager.GetSharedComponentData<RenderMesh>(linkedEntityBuffer[2].Value);

            var colors = World.GetExistingSystem<ApplyOverlayColors>();
            if (colors.PlayerMats == null || (colors.PlayerMats.Length >= PlayerConstants.MaxPlayers && colors.PlayerMats[colorIndex] == null))
            {
                if (colors.PlayerMats == null)
                    colors.PlayerMats = new Material[PlayerConstants.MaxPlayers];

                if (colors.PlayerMats[colorIndex] == null)
                {
                    var mat = new Material(sharedMesh.material);
                    mat.color = colors.Colors[colorIndex];
                    colors.PlayerMats[colorIndex] = mat;
                }
            }

            sharedMesh.material = colors.PlayerMats[colorIndex];
            PostUpdateCommands.SetSharedComponent(linkedEntityBuffer[2].Value, sharedMesh);

            sharedMesh = EntityManager.GetSharedComponentData<RenderMesh>(linkedEntityBuffer[3].Value);
            sharedMesh.material = colors.PlayerMats[colorIndex];
            PostUpdateCommands.SetSharedComponent(linkedEntityBuffer[3].Value, sharedMesh);

            PostUpdateCommands.AddComponent<InitializedHomebase>(entity);
        });
    }
}
