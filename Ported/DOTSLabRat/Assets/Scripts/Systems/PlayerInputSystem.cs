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
                                player.arrowToPlace = new int2((int) hitPoint.x, (int) hitPoint.z);
                            }
                        }
                    }
                }).Run();
        }
    }
}