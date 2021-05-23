using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public struct ArrowSpawner : IJobEntity
{
    [ReadSingleton] public GameConfig gameConfig;
    [ReadSingleton] public NativeArray<Entity> cellArray;

    [Read] public Lookup<Translation> translationData;
    [Read] public Lookup<ForcedDirection> forcedDirectionData;
    
    public void Execute(ref DynamicBuffer<CreatedArrowData> ArrowBuffer, 
            in PlayerInput playerInput, in PlayerColor color, in PlayerIndex PlayerIndex )
    {
        if (playerInput.IsMouseDown && playerInput.TileIndex < cellArray.Length && playerInput.TileIndex >= 0)
        {
            Entity cellEntity = cellArray[playerInput.TileIndex];
            var cellTranslation = translationData[cellEntity];

            bool foundOwnArrow = false;
            foreach (var alreadyCreatedArrows in ArrowBuffer)
            {
                if (alreadyCreatedArrows.TileEntity == cellEntity)
                {
                    RemoveArrow(ref ArrowBuffer);
                    foundOwnArrow = true;
                }
            }

            if (foundOwnArrow || // Removed our own arrow
                !foundOwnArrow && forcedDirectionData[cellEntity].Value != Cardinals.None) // Don't allow clearing other players arrows
                return;

            Entity arrowEntity = Instantiate(gameConfig.ArrowPrefab);
            
            Set(arrowEntity, cellTranslation);

            Rotation rotation = new Rotation();
            Cardinals direction = playerInput.ArrowDirection;
            Add(arrowEntity, new Direction(direction));
            Set(cellEntity, new ForcedDirection() { Value = direction });
            Add(arrowEntity, new Arrow());
            Add(arrowEntity, new PlayerIndex() { Index = PlayerIndex.Index });
            Add(arrowEntity, new URPMaterialPropertyBaseColor() { Value = color.Color });
           
            AppendToBuffer(new CreatedArrowData()
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
            
            Set(arrowEntity, rotation);
        }
    }
    
    // Can Burst compile instance methods of a job?
    void RemoveArrow(ref DynamicBuffer<CreatedArrowData> arrowBuffer)
    {
        // Destroy arrow entity
        Entity arrowToDestroy = arrowBuffer[0].CreatedArrow;
        Destroy(arrowToDestroy);
                    
        // Reset tile forced direction
        Entity arrowTile = arrowBuffer[0].TileEntity;
        Set(arrowTile, new ForcedDirection());
                    
        arrowBuffer.RemoveAt(0);
    }
}