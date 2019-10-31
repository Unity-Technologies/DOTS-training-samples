using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GameAI
{
    public struct AITagTaskNone : IComponentData
    {
    }

    public struct AITagTaskClearRock : IComponentData
    {
    }

    public struct AITagTaskTill : IComponentData
    {
    }

    public struct AITagTaskDeliver : IComponentData
    {
    }
}