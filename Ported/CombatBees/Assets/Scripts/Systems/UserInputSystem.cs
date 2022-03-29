using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial class UserInputSystem : SystemBase
    {
        private GameObjectRefs _refs;
        private Camera _camera;

        private Plane[] _worldPlanes;
        
        // protected override void OnCreate()
        // {
        //     _worldPlanes = new Plane[6];
        //     for (var i = 0; i < 3; i++)
        //     {
        //         for (var j = -1; j <= 1; j += 2)
        //         {
        //             var wallCenter = new Vector3
        //             {
        //                 [i] = Field.size[i] * .5f * j
        //             };
        //             var plane = new Plane(-wallCenter, wallCenter);
        //             if (Vector3.Dot(plane.normal, mouseRay.direction) < 0f)
        //             {
        //                 if (plane.Raycast(mouseRay, out var hitDistance))
        //                 {
        //                     var hitPoint = mouseRay.GetPoint(hitDistance);
        //                     var insideField = true;
        //                     for (var k = 0; k < 3; k++)
        //                     {
        //                         if (Mathf.Abs(hitPoint[k]) > Field.size[k] * .5f + .01f)
        //                         {
        //                             insideField = false;
        //                             break;
        //                         }
        //                     }
        //
        //                     if (insideField)
        //                     {
        //                         isMouseTouchingField = true;
        //                         worldMousePosition = hitPoint;
        //                         break;
        //                     }
        //                 }
        //             }
        //         }
        // }

        protected override void OnUpdate()
        {
            _refs = this.GetSingleton<GameObjectRefs>();
            _camera = _refs.Camera;

            var mouseRay = _camera.ScreenPointToRay(Input.mousePosition);

            var isMouseTouchingField = false;
            var worldMousePosition = new Vector3();
            for (var i = 0; i < 3; i++)
            {
                for (var j = -1; j <= 1; j += 2)
                {
                    var wallCenter = new Vector3
                    {
                        [i] = Field.size[i] * .5f * j
                    };
                    var plane = new Plane(-wallCenter, wallCenter);
                    if (Vector3.Dot(plane.normal, mouseRay.direction) < 0f)
                    {
                        if (plane.Raycast(mouseRay, out var hitDistance))
                        {
                            var hitPoint = mouseRay.GetPoint(hitDistance);
                            var insideField = true;
                            for (var k = 0; k < 3; k++)
                            {
                                if (Mathf.Abs(hitPoint[k]) > Field.size[k] * .5f + .01f)
                                {
                                    insideField = false;
                                    break;
                                }
                            }

                            if (insideField)
                            {
                                isMouseTouchingField = true;
                                worldMousePosition = hitPoint;
                                break;
                            }
                        }
                    }
                }

                if (isMouseTouchingField)
                {
                    break;
                }
            }

            if (isMouseTouchingField)
            {
                Entities.WithAll<MousePointer>().ForEach((ref Translation translation) =>
                {
                    translation.Value = worldMousePosition;
                }).Run();

                if (Input.GetMouseButtonDown(0))
                {
                    var landPosition = worldMousePosition;
                    landPosition.y = -10;
                    var ray = new Ray(worldMousePosition, Vector3.down);
                    
                    Entities
                        .WithAll<Components.Resource>()
                        .WithNone<KinematicBody>()
                        .ForEach((in Translation translation) =>
                        {
                            var pos = new Vector3(translation.Value.x, translation.Value.y, translation.Value.z);
                            var distance = Vector3.Cross(ray.direction, pos - ray.origin).magnitude;
                            if (distance < 0.75f)
                                landPosition = translation.Value + new float3(0, 2, 0);
                        }).Run();

                    var ecb = new EntityCommandBuffer(Allocator.Temp);
                    var resource = ecb.Instantiate(_refs.ResourcePrefab);
                    var translation = new Translation
                        { Value = new float3(landPosition.x, worldMousePosition.y, landPosition.z) };
                    var kinematicBody = new KinematicBody()
                        { Velocity = float3.zero, LandPosition = landPosition, Height = landPosition.y };
                    ecb.SetComponent(resource, translation);
                    ecb.SetComponent(resource, kinematicBody);
                    ecb.Playback(EntityManager);
                    ecb.Dispose();
                }
            }
        }
    }
}