using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct PathMoverSafeFollowComponent : IComponentData
{
	public Entity m_FollowEntity;
}
