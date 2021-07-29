using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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
                .WithoutBurst()
                .ForEach((ref Player player, in DynamicBuffer<PlacedArrow> placedArrows) =>
                {
                    //Handle player AI system
                    if (player.playerNumber != 0)
                    {

                    }
                    
                    //Validate placed arrow
                    if (player.arrowToPlace.x != -1)
                    {
                        for (int i = 0; i < placedArrows.Length; i++)
                        {
                            var arrowTranslation = EntityManager.GetComponentData<Translation>(placedArrows[i].entity);
                            if ((int) arrowTranslation.Value.x == player.arrowToPlace.x &&
                                (int) arrowTranslation.Value.z == player.arrowToPlace.y)
                            {
                                player.arrowToPlace = new int2(-1, -1);
                                break;
                            }
                        }
                    }

                    //Place new arrow
                    if (player.arrowToPlace.x != -1)
                    {
                        CellStruct cell;
                        int cellIndex;
                        
                        //remove oldest arrow in board struct
                        if (player.arrowToRemove.x != -1)
                        {
                            cellIndex = player.arrowToRemove.y * gameState.boardSize + player.arrowToRemove.x;
                            cell = cellStructs[cellIndex];
                            cell.arrow = Direction.None;
                            cellStructs[cellIndex] = cell;
                        }

                        //update cell in board struct with newly placed arrow
                        cellIndex = player.arrowToPlace.y * gameState.boardSize + player.arrowToPlace.x;
                        cell = cellStructs[cellIndex];
                        cell.arrow = player.arrowDirection;
                        cellStructs[cellIndex] = cell;
                        
                        //update cell in board struct with newly placed arrow
                        Entity currentArrow = placedArrows[player.currentArrow].entity;
                        EntityManager.SetComponentData(currentArrow, new Translation() { Value = new float3(player.arrowToPlace.x, 0.05f, player.arrowToPlace.y) });

                        //set arrow rotation
                        quaternion rotation = quaternion.identity;
                        switch (player.arrowDirection)
                        {
                            case Direction.North:
                                rotation = new quaternion(0.7071068f,0,0,0.7071068f);
                                break;
                            case Direction.East:
                                rotation = new quaternion(0.5f,0.5f,-0.5f,0.5f);
                                break;
                            case Direction.South:
                                rotation = new quaternion(0,0.7071068f,-0.7071068f,0);
                                break;
                            case Direction.West:
                                rotation = new quaternion(-0.5f,0.5f,-0.5f,-0.5f);
                                break;
                        }
                        EntityManager.SetComponentData(currentArrow, new Rotation() { Value = rotation });
                        
                        //update arrow index
                        player.currentArrow++;
                        if (player.currentArrow == placedArrows.Length)
                            player.currentArrow = 0;
                        
                        //check if all arrows are placed, and mark next one to be removed
                        currentArrow = placedArrows[player.currentArrow].entity;
                        var oldestPos = EntityManager.GetComponentData<Translation>(currentArrow);
                        if (oldestPos.Value.x > 0)
                            player.arrowToRemove = new int2((int) oldestPos.Value.x, (int) oldestPos.Value.z);

                        player.arrowToPlace = new int2(-1, -1);
                    }
                }).Run();
        }
    }
}
