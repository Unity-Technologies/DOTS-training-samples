using Unity.Entities;
using Unity.Mathematics;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;

namespace DOTSRATS
{
    public class PlayerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var gameStateEntity = GetSingletonEntity<GameState>();
            var cellStructs = GetBuffer<CellStruct>(gameStateEntity);
            var gameState = EntityManager.GetComponentData<GameState>(gameStateEntity);
            
            Entities
                .WithAll<InPlay>()
                .ForEach((ref Player player) =>
                {
                    //Handle player AI system
                    if (player.playerNumber != 0)
                    {

                    }

                    //Check for new arrow and place them
                    if (player.arrowToPlace.x != -1)
                    {
                        var cell = cellStructs[player.arrowToPlace.y * gameState.boardSize + player.arrowToPlace.x];
                        cell.arrow = player.arrowDirection;
                        cellStructs[player.arrowToPlace.y * gameState.boardSize + player.arrowToPlace.x] = cell;
                        player.arrowToPlace = new int2(-1, -1);
                    }
                }).Schedule();
        }
    }
}
