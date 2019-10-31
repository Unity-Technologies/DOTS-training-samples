using ECSExamples;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class ArrowRenderingSystem : ComponentSystem
{
    private Material[] m_HomebaseMats;

    public struct InitializedHomebase : IComponentData
    {}

    protected override void OnUpdate()
    {
        //var overlayEntities = GetEntityQuery(typeof(OverlayComponentTag), typeof(Translation)).ToEntityArray(Allocator.TempJob);
        //var overlayPositions = GetEntityQuery(typeof(OverlayComponentTag), typeof(Translation)).ToComponentDataArray<Translation>(Allocator.TempJob);
        //var overlayRotations = GetEntityQuery(typeof(OverlayComponentTag), typeof(Rotation)).ToComponentDataArray<Rotation>(Allocator.TempJob);
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


            /*if (blockState == BlockState.Confuse) {
                z = 0f;
                mat = ConfuseMaterial;
                color = Color.white;
                localScale = Vector3.one;
            } else {*/
            //z = ((int)arrow.Direction) * -90f;
            /*mat = ArrowMaterial;
            color = ArrowColor;
            localScale = arrowStrength == ArrowStrength.Small ? new Vector3(0.55f, 0.55f, 0.55f) : new Vector3(0.85f, 0.85f, 0.85f);

            OverlayRenderer.transform.localRotation = Quaternion.Euler(90f, 0, z);
            OverlayRenderer.transform.localScale = localScale;
            OverlayRenderer.sharedMaterial = mat;
            bool hasOverlay = color.a > 0f || blockState == BlockState.Confuse;

            OverlayRenderer.gameObject.SetActive(hasOverlay);
            OverlayColorRenderer.enabled = hasOverlay && HasArrow;

            props.SetColor(_ColorID, color);
            OverlayRenderer.SetPropertyBlock(props);

            props.SetColor(_ColorID, overlayColor);
            OverlayColorRenderer.SetPropertyBlock(props);*/

            PostUpdateCommands.RemoveComponent<ArrowComponent>(entity);
        });

        /*Entities.ForEach((Entity entity, ref ArrowComponent arrow, ref Translation position) =>
        {
            const int maxArrows = 3;
            var startIndex = arrow.PlayerId * maxArrows;
            Entity arrowEntity = Entity.Null;
            for (int i = startIndex; i < maxArrows; ++i)
            {
                // TODO: Handle case where
                if (overlayPositions[i].Value.y <= -10)
                {
                    arrowEntity = overlayEntities[i];
                    break;
                }
            }
            arrowEntity = overlayEntities[startIndex + arrow.PlayerId];
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
        overlayRotations.Dispose();
        overlayPositions.Dispose();
        overlayEntities.Dispose();*/

        Entities.WithNone<InitializedHomebase>().ForEach((Entity entity, ref HomebaseComponent home) =>
        {
            if ((int)home.Color.x == 0 && (int)home.Color.y == 0 && (int)home.Color.z == 0 && (int)home.Color.w == 0)
                return;
            var linkedEntityBuffer = EntityManager.GetBuffer<LinkedEntityGroup>(entity);
            /*PostUpdateCommands.SetComponent(linkedEntityBuffer[2].Value, new MaterialColor{Value = home.Color});
            PostUpdateCommands.SetComponent(linkedEntityBuffer[3].Value, new MaterialColor{Value = home.Color});*/

            var colorIndex = home.PlayerId - 1;
            var sharedMesh = EntityManager.GetSharedComponentData<RenderMesh>(linkedEntityBuffer[2].Value);
            /*if (m_HomebaseMats == null || (m_HomebaseMats.Length >= PlayerConstants.MaxPlayers && m_HomebaseMats[colorIndex] == null))
            {
                if (m_HomebaseMats == null)
                    m_HomebaseMats = new Material[PlayerConstants.MaxPlayers];

                if (m_HomebaseMats[colorIndex] == null)
                {
                    var mat = new Material(sharedMesh.material);
                    mat.color = new Color(home.Color.x, home.Color.y, home.Color.z, home.Color.w);
                    m_HomebaseMats[colorIndex] = mat;
                }
                //Debug.Log("Client set index " + colorIndex + " entity " + entity + " as color " + PlayerMats[colorIndex].color);
            }*/
            var colors = World.GetExistingSystem<ApplyOverlayColors>();

            /*var mat = new Material(sharedMesh.material);
            mat.color = new Color(home.Color.x, home.Color.y, home.Color.z, home.Color.w);
            sharedMesh.material = mat;*/
            //sharedMesh.material = m_HomebaseMats[colorIndex];
            sharedMesh.material = colors.PlayerMats[colorIndex];
            PostUpdateCommands.SetSharedComponent(linkedEntityBuffer[2].Value, sharedMesh);

            sharedMesh = EntityManager.GetSharedComponentData<RenderMesh>(linkedEntityBuffer[3].Value);
            /*mat = new Material(sharedMesh.material);
            mat.color = new Color(home.Color.x, home.Color.y, home.Color.z, home.Color.w);
            sharedMesh.material = mat;*/
            //sharedMesh.material = m_HomebaseMats[colorIndex];
            sharedMesh.material = colors.PlayerMats[colorIndex];
            PostUpdateCommands.SetSharedComponent(linkedEntityBuffer[3].Value, sharedMesh);

            PostUpdateCommands.AddComponent<InitializedHomebase>(entity);
        });
    }
}
