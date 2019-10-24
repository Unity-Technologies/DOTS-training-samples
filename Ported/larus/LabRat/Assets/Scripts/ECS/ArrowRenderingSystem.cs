using ECSExamples;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEditor;
using UnityEditor.iOS;
using UnityEngine;

public class ArrowRenderingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //var overlays = GetEntityQuery(typeof(OverlayComponentTag), typeof(Translation)).ToComponentDataArray<Translation>(Allocator.TempJob);
        //var overlayRotations = GetEntityQuery(typeof(OverlayComponentTag), typeof(Rotation)).ToComponentDataArray<Rotation>(Allocator.TempJob);
        var overlayEntities = GetEntityQuery(typeof(OverlayComponentTag), typeof(Translation)).ToEntityArray(Allocator.TempJob);
        Entities.ForEach((Entity entity, ref ArrowComponent arrow, ref Translation position) =>
        {
            float z;
            Material mat;
            Color color;
            Vector3 localScale;
            RenderMesh mesh;

            /*var childEntities = EntityManager.GetBuffer<Child>(entity);
            var overlayEntity = childEntities[0].Value;
            var sharedMesh = EntityManager.GetSharedComponentData<RenderMesh>(overlayEntity);
            sharedMesh.material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Arrow.mat");
            sharedMesh.material.color = new Color(1f, 1f, 1f, 0.5f);;
            PostUpdateCommands.SetSharedComponent(overlayEntity, sharedMesh);*/

            PostUpdateCommands.SetComponent(overlayEntities[0], new Translation { Value = position.Value + new float3(0,0.53f,0)});
            //var rotation = quaternion.AxisAngle(new float3(1,0,0), math.PI/2);;
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
            PostUpdateCommands.SetComponent(overlayEntities[0], new Rotation{Value = rotation});

            //var sharedMesh = EntityManager.GetSharedComponentData<RenderMesh>(overlayEntities[0]);
            //sharedMesh.material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Arrow.mat");
            //sharedMesh.material.color = new Color(1f, 1f, 1f, 0.5f);;
            //PostUpdateCommands.SetSharedComponent(overlayEntities[0], sharedMesh);

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
        //overlays.Dispose();
        overlayEntities.Dispose();
    }
}
