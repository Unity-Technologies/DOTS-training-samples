using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges().ForEach((Entity spawnerEntity, in Board board) =>
        {
            //var offsets = OffsetGenerator.CreateRandomOffsets(board.SizeX, board.SizeY, board.MinHeight, board.MaxHeight);
            
            for (int y = 0; y < board.SizeY; ++y)
            {
                for (int x = 0; x < board.SizeX; ++x)
                {
                    var instance = EntityManager.Instantiate(board.PlaformPrefab);
                   
                    var translation = new float3(x, -0.5f, y);
                    var scale = new float3(1f, 1f, 1f);//offsets[y * board.SizeX + x]);

                    EntityManager.SetComponentData(instance, new Translation {Value = translation});
                    EntityManager.AddComponentData(instance, new NonUniformScale {Value = scale});
                }
            }

            EntityManager.DestroyEntity(spawnerEntity);
        }).Run();
    }
}