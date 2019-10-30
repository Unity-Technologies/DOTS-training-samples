using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct State : IComponentData
{
    public enum StateType {Idle, Collecting, Chasing, Attacking, Carrying, Dropping, Dying, Dead};
    public StateType Value; 
}