using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
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
            var random = Random.CreateFromIndex((uint)System.DateTime.Now.Ticks);
            var elapsedTime = Time.ElapsedTime;
            
            Entities
                .WithAll<InPlay>()
                .ForEach((ref Player player) =>
                {
                    //Handle player AI system
                    if (player.playerNumber != 0)
                    {
                        if (player.nextArrowTime < elapsedTime)
                        {
                            if (player.nextArrowTime != 0)
                            {
                                int cellIndex;
                                CellStruct cell;
                                do
                                {
                                    player.arrowToPlace = new int2(random.NextInt(0, gameState.boardSize), random.NextInt(0, gameState.boardSize));
                                    cellIndex = player.arrowToPlace.y * gameState.boardSize + player.arrowToPlace.x;
                                    cell = cellStructs[cellIndex]; 
                                    // Look for a coordinate that's not a hole, goal and does not have an arrow placed 
                                } while (cell.hole || cell.goal != default || cell.arrow != Direction.None);

                                cellIndex = player.arrowToPlace.y * gameState.boardSize + player.arrowToPlace.x;
                                cell = cellStructs[cellIndex];
                                player.arrowDirection = Utils.GetRandomCardinalDirection(ref random);
                                cell.arrow = player.arrowDirection;
                                cellStructs[cellIndex] = cell;
                                
                                Debug.Log($"AI {player.playerNumber} placed arrow {(int)cell.arrow} at {player.arrowToPlace}");
                            }
                            
                            var arrowDelayRange = player.arrowPlacementDelayRange;
                            player.nextArrowTime = elapsedTime + random.NextFloat(arrowDelayRange.x, arrowDelayRange.y);
                            Debug.Log($"AI {player.playerNumber} will place next arrow after {player.nextArrowTime - elapsedTime}");
                        }
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
