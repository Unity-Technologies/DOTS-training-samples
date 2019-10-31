using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GameAI
{
    public struct AISubTaskTagComplete : IComponentData
    {
        public int2 targetPos;
    }

    public struct AISubTaskTagFindUntilledTile : IComponentData {}
    public struct AISubTaskTagTillGroundTile : IComponentData {}
    public struct AISubTaskTagPlantSeed : IComponentData {}

    public struct AISubTaskTagFindRock : IComponentData {}

    public struct AISubTaskTagClearRock : IComponentData
    {
        public Entity requestEntity;
        public Entity rockEntity;
    }
    
    public struct AISubTaskTagFindPlant : IComponentData {}
    public struct AISubTaskTagFindShop : IComponentData {}
    public struct AISubTaskSellPlant : IComponentData {}
}