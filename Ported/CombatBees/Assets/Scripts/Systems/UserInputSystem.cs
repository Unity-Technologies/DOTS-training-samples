using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial class UserInputSystem : SystemBase
    {
        private GameObjectRefs _refs;
        private Camera _camera;


        protected override void OnUpdate()
        {
            _refs = this.GetSingleton<GameObjectRefs>();
            _camera = _refs.Camera;

            var mouseRay = _camera.ScreenPointToRay(Input.mousePosition);

            var isMouseTouchingField = false;
            Vector3 worldMousePosition = new Vector3();
            for (int i = 0; i < 3; i++)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    Vector3 wallCenter = new Vector3();
                    wallCenter[i] = Field.size[i] * .5f * j;
                    Plane plane = new Plane(-wallCenter, wallCenter);
                    float hitDistance;
                    if (Vector3.Dot(plane.normal, mouseRay.direction) < 0f)
                    {
                        if (plane.Raycast(mouseRay, out hitDistance))
                        {
                            Vector3 hitPoint = mouseRay.GetPoint(hitDistance);
                            bool insideField = true;
                            for (int k = 0; k < 3; k++)
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
                    var ecb = new EntityCommandBuffer(Allocator.Temp);
                    var bee = ecb.Instantiate(_refs.ResourcePrefab);
                    var translation = new Translation { Value = worldMousePosition };
                    ecb.SetComponent(bee, translation);
                    ecb.Playback(EntityManager);
                    ecb.Dispose();
                }
            }
        }
    }
}