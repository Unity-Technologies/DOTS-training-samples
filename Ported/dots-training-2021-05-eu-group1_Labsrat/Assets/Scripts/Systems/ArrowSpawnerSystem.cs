using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

[UpdateInGroup(typeof(ChuChuRocketUpdateGroup))]
public class ArrowSpawnerSystem : SystemBase
{
    EntityCommandBufferSystem m_EcbSystem;
    protected override void OnCreate()
    {
        m_EcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    static void RemoveArrow(ref DynamicBuffer<CreatedArrowData> arrowBuffer, ref EntityCommandBuffer ecb)
    {
        // Destroy arrow entity
        Entity arrowToDestroy = arrowBuffer[0].CreatedArrow;
        ecb.DestroyEntity(arrowToDestroy);
                    
        // Reset tile forced direction
        Entity arrowTile = arrowBuffer[0].TileEntity;
        ecb.SetComponent(arrowTile, new ForcedDirection());
                    
        arrowBuffer.RemoveAt(0);
    }
    
    protected override void OnUpdate()
    {
        var gameConfig = GetSingleton<GameConfig>();
        var translationData = GetComponentDataFromEntity<Translation>();
        var forcedDirectionData = GetComponentDataFromEntity<ForcedDirection>();

        var ecb = m_EcbSystem.CreateCommandBuffer();
        var cellArray = World.GetExistingSystem<BoardSpawner>().cells;

        Entities
            .WithoutBurst()
            .WithReadOnly(forcedDirectionData)
            .ForEach((Entity Player, ref DynamicBuffer<CreatedArrowData> ArrowBuffer, in PlayerInput playerInput, in PlayerColor color, in PlayerIndex PlayerIndex ) => {

            if (playerInput.IsMouseDown && playerInput.TileIndex < cellArray.Length && playerInput.TileIndex >= 0)
            {
                Entity cellEntity = cellArray[playerInput.TileIndex];
                var cellTranslation = translationData[cellEntity];

                bool foundOwnArrow = false;
                foreach (var alreadyCreatedArrows in ArrowBuffer)
                {
                    if (alreadyCreatedArrows.TileEntity == cellEntity)
                    {
                        RemoveArrow(ref ArrowBuffer, ref ecb);
                        foundOwnArrow = true;
                    }
                }

                if (foundOwnArrow || // Removed our own arrow
                    !foundOwnArrow && forcedDirectionData[cellEntity].Value != Cardinals.None) // Don't allow clearing other players arrows
                    return;

                Entity arrowEntity = ecb.Instantiate(gameConfig.ArrowPrefab);
                
                ecb.SetComponent(arrowEntity, cellTranslation);

                Rotation rotation = new Rotation();
                Cardinals direction = playerInput.ArrowDirection;
                ecb.AddComponent(arrowEntity, new Direction(direction));
                ecb.SetComponent(cellEntity, new ForcedDirection() { Value = direction });
                ecb.AddComponent(arrowEntity, new Arrow());
                ecb.AddComponent(arrowEntity, new PlayerIndex() { Index = PlayerIndex.Index });
                ecb.AddComponent(arrowEntity, new URPMaterialPropertyBaseColor() { Value = color.Color });
               
                ecb.AppendToBuffer(Player,new CreatedArrowData()
                {
                    CreatedArrow = arrowEntity, 
                    TileEntity = cellEntity
                });

                if (ArrowBuffer.Length > gameConfig.MaximumArrows-1)
                {
                    RemoveArrow(ref ArrowBuffer, ref ecb);
                }

                switch (direction)
                {
                    default:
                    case Cardinals.North:
                        rotation.Value = quaternion.RotateY(math.radians(180));
                        break;
                    case Cardinals.South:
                        break;
                    case Cardinals.East:
                        rotation.Value = quaternion.RotateY(math.radians(270));
                        break;
                    case Cardinals.West:
                        rotation.Value = quaternion.RotateY(math.radians(90));
                        break;
                }
                
                ecb.SetComponent(arrowEntity, rotation);
            }
        }).Schedule();
        
        m_EcbSystem.AddJobHandleForProducer(Dependency);
    }
}
