using ECSExamples;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class ArrowRenderingSystem : ComponentSystem
{
    private Material[] m_HomebaseMats;
    private GameObject[] m_Cursors;
    private Canvas m_Canvas;
    private ApplyOverlayColors m_Colors;

    protected override void OnCreate()
    {
        m_Canvas = GameObject.FindObjectOfType<Canvas>();
        m_Colors = World.GetOrCreateSystem<ApplyOverlayColors>();
    }

    public struct InitializedHomebase : IComponentData
    {}

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref ArrowComponent arrow) =>
        {
            var hoverOverlay = GetSingletonEntity<HoverOverlayComponentTag>();
            PostUpdateCommands.SetComponent(hoverOverlay, new Translation { Value = new float3(arrow.Coordinates.x,0.55f,arrow.Coordinates.y)});
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
            PostUpdateCommands.DestroyEntity(entity);
        });

        Entities.WithNone<InitializedHomebase>().ForEach((Entity entity, ref HomebaseComponent home) =>
        {
            if ((int)home.Color.x == 0 && (int)home.Color.y == 0 && (int)home.Color.z == 0 && (int)home.Color.w == 0)
                return;
            var linkedEntityBuffer = EntityManager.GetBuffer<LinkedEntityGroup>(entity);
            var colorIndex = home.PlayerId - 1;
            var sharedMesh = EntityManager.GetSharedComponentData<RenderMesh>(linkedEntityBuffer[2].Value);

            if (m_Colors.PlayerMats == null || (m_Colors.PlayerMats.Length >= PlayerConstants.MaxPlayers && m_Colors.PlayerMats[colorIndex] == null))
            {
                if (m_Colors.PlayerMats == null)
                    m_Colors.PlayerMats = new Material[PlayerConstants.MaxPlayers];

                if (m_Colors.PlayerMats[colorIndex] == null)
                {
                    var mat = new Material(sharedMesh.material);
                    mat.color = m_Colors.Colors[colorIndex];
                    m_Colors.PlayerMats[colorIndex] = mat;
                }
            }

            sharedMesh.material = m_Colors.PlayerMats[colorIndex];
            PostUpdateCommands.SetSharedComponent(linkedEntityBuffer[2].Value, sharedMesh);

            sharedMesh = EntityManager.GetSharedComponentData<RenderMesh>(linkedEntityBuffer[3].Value);
            sharedMesh.material = m_Colors.PlayerMats[colorIndex];
            PostUpdateCommands.SetSharedComponent(linkedEntityBuffer[3].Value, sharedMesh);

            PostUpdateCommands.AddComponent<InitializedHomebase>(entity);
        });

        Entities.ForEach((Entity entity, ref PlayerComponent player, ref Translation position) =>
        {
            // Player spawned but has not yet received snapshot update
            if (player.PlayerId == 0)
                return;

            if (m_Cursors == null)
                m_Cursors = new GameObject[PlayerConstants.MaxPlayers];
            var cursorIndex = player.PlayerId - 1;
            if (m_Cursors[cursorIndex] == null)
            {
                var cursorPrefab = (GameObject)Resources.Load("Cursor");
                m_Cursors[cursorIndex] = GameObject.Instantiate(cursorPrefab, Vector3.zero, Quaternion.identity, m_Canvas.transform);
                var cursorImage = m_Cursors[cursorIndex].GetComponentInChildren<Image>();
                cursorImage.color = m_Colors.PlayerMats[cursorIndex].color;
            }

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_Canvas.transform as RectTransform, new Vector2(position.Value.x, position.Value.y), m_Canvas.worldCamera, out pos);
            m_Cursors[cursorIndex].transform.position = m_Canvas.transform.TransformPoint(pos);
        });
    }
}
