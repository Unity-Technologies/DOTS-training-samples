using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;

namespace DOTSRATS
{
    public class PlayerInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var boardPlane = new Plane(Vector3.up, Vector3.zero);
            var camera = this.GetSingleton<GameObjectRefs>().camera;

            Entities
                .WithAll<InPlay>()
                .WithoutBurst()
                .ForEach((Entity entity, ref Player player) =>
                {
                    //Input only for main player
                    if (player.playerNumber == 0)
                    {
                        if (UnityInput.GetMouseButtonDown(0))
                        {
                            Ray ray = camera.ScreenPointToRay(UnityInput.mousePosition);
                            float enter = 0.0f;

                            if (boardPlane.Raycast(ray, out enter))
                            {
                                Vector3 hitPoint = ray.GetPoint(enter);
                                var coord = new int2((int) (hitPoint.x + 0.5), (int) (hitPoint.z + 0.5));
                                player.arrowToPlace = coord;

                                var offset = new float2(hitPoint.x - coord.x, hitPoint.z - coord.y);
                                if (Mathf.Abs(offset.y) > Mathf.Abs(offset.x))
                                    player.arrowDirection = offset.y > 0 ? Direction.North : Direction.South;
                                else
                                    player.arrowDirection = offset.x > 0 ? Direction.East : Direction.West;
                            }
                        }
                    }
                }).Run();
        }
    }
}