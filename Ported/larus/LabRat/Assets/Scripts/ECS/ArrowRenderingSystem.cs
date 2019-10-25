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
    }
}
