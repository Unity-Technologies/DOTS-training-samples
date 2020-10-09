using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
/*
public class UpdateClosestFireSystem : SystemBase
{
    private EntityQuery waterQuery;

    protected override void OnCreate()
    {
        waterQuery = GetEntityQuery(ComponentType.ReadOnly<WaterTag>(), ComponentType.ReadOnly<Pos>());
    }
    protected override void OnUpdate()
    {
        NativeArray<Pos> waterPositions = waterQuery.ToComponentDataArray<Pos>(Allocator.TempJob);

        var chainSharedComponents = new List<SharedChainComponent>();
        EntityManager.GetAllUniqueSharedComponentData(chainSharedComponents);

        Entities
            .WithName("SetClosestFirePosition")
            .WithoutBurst()
            .WithDisposeOnCompletion(waterPositions)
            .ForEach((ref ChainStart start, ref ChainEnd end, ref ClosestFireRequest closestFire, in ChainID chainID) =>
            {
                end.Value = closestFire.closestFirePosition;
                closestFire = new ClosestFireRequest(end.Value);    // Don't want to move far looking for fire
                    
                start.Value = Utility.GetNearestPos(end.Value, waterPositions, out _);

                for (int i = 0; i < chainSharedComponents.Count; i++)
                {
                    var sharedChain = chainSharedComponents[i]; 
                    if (sharedChain.chainID != chainID.Value)
                        continue;
                 
                    sharedChain.start = start.Value;
                    sharedChain.end = end.Value;   
                }
            }).Run();

        waterPositions.Dispose();
    }
}*/