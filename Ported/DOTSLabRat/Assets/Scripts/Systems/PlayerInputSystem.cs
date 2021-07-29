using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;

namespace DOTSRATS
{
    [UpdateAfter(typeof(PlayerSetupSystem))]
    public class PlayerInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var boardPlane = new Plane(Vector3.up, Vector3.zero);
            var camera = this.GetSingleton<GameObjectRefs>().camera;
            var gameStateEntity = GetSingletonEntity<GameState>();
            var gameState = EntityManager.GetComponentData<GameState>(gameStateEntity);
            var arrowPreviewEntity = GetSingletonEntity<ArrowPreview>();

            Entities
                .WithAll<InPlay>()
                .WithoutBurst()
                .ForEach((Entity entity, ref Player player) =>
                {
                    //Input only for main player
                    if (player.playerNumber == 0)
                    {
                        Ray ray = camera.ScreenPointToRay(UnityInput.mousePosition);
                        float enter = 0.0f;

                        if (boardPlane.Raycast(ray, out enter))
                        {
                            Vector3 hitPoint = ray.GetPoint(enter);
                            var coord = new int2((int) (hitPoint.x + 0.5), (int) (hitPoint.z + 0.5));

                            if (coord.x >= 0 && coord.x < gameState.boardSize && coord.y >= 0 && coord.y < gameState.boardSize)
                            {
                                Direction direction;
                                var offset = new float2(hitPoint.x - coord.x, hitPoint.z - coord.y);
                                if (Mathf.Abs(offset.y) > Mathf.Abs(offset.x))
                                    direction = offset.y > 0 ? Direction.North : Direction.South;
                                else
                                    direction = offset.x > 0 ? Direction.East : Direction.West;
                                
                                EntityManager.SetComponentData(arrowPreviewEntity, new Translation { Value = new float3(coord.x, 0.05f, coord.y) });
                                EntityManager.SetComponentData(arrowPreviewEntity, new Rotation { Value = direction.ToArrowRotation() });

                                if (UnityInput.GetMouseButtonDown(0))
                                {
                                    player.arrowToPlace = coord;
                                    player.arrowDirection = direction;
                                }
                            }
                            else
                            {
                                EntityManager.SetComponentData(arrowPreviewEntity, new Translation { Value = new float3(-100, 0, -100) });
                            }
                        }
                    }
                }).Run();
        }
    }
}