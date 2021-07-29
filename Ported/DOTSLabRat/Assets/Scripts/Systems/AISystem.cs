using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;

namespace DOTSRATS
{
    public class AISystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<GameState>();
            RequireSingletonForUpdate<CellStruct>();
        }
        
        protected override void OnUpdate()
        {
            var gameStateEntity = GetSingletonEntity<GameState>();
            var cellStructs = GetBuffer<CellStruct>(gameStateEntity);
            var gameState = EntityManager.GetComponentData<GameState>(gameStateEntity);

            Entities
                .WithAll<InPlay>()
                .WithReadOnly(cellStructs)
                .ForEach((ref Player player) =>
                {
                    //Handle player AI system
                    if (player.playerNumber != 0)
                    {
                        // If AI doesn't have a placement coord target, pick new one
                        if (player.arrowToPlace.x == -1)
                        {
                            int cellIndex;
                            CellStruct cell;
                            var goalCoord = Utils.PlayerNumberToGoalCoord(player.playerNumber, gameState.boardSize);
                            do
                            {
                                player.arrowToPlace = new int2(player.random.NextInt(0, gameState.boardSize), player.random.NextInt(0, gameState.boardSize));
                                cellIndex = player.arrowToPlace.y * gameState.boardSize + player.arrowToPlace.x;
                                cell = cellStructs[cellIndex];
                                // Pick a direction for the arrow
                                var dirToGoal = Utils.GetCardinalDirectionFromIntDirection(goalCoord - player.arrowToPlace);
                                player.arrowDirection = dirToGoal;
                                // Look for a coordinate that's: a hole, goal and does not have an arrow placed 
                            } while (cell.hole || // not a hole
                                     cell.goal != default || // does not have a goal
                                     cell.arrow != Direction.None || // no arrow is placed 
                                     (player.arrowDirection & cell.wallLayout) != 0); // AI's arrow would not be facing a wall
                        }
                    }
                }).ScheduleParallel();
        }
    }
}
