using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct AITaskTagNone : IComponentData
{}

public struct AITaskTagClearRock : IComponentData
{}

public struct AITaskTagTill : IComponentData
{}

public struct AITaskTagPlant : IComponentData
{}

public struct AITaskTagCollect : IComponentData
{}

public struct AITaskTagDeliver : IComponentData
{}