using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GameAI
{
    public struct AISubTaskTagFindUntilledTile : IComponentData
    {
    }

    public struct AISubTaskTagTillGroundTile : IComponentData
    {
    }

    public struct AISubTaskTagComplete : IComponentData
    {
    }
}