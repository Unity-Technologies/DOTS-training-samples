using Authoring;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Aspects
{
	readonly partial struct BallAspect : IAspect
	{
		// An Entity field in an aspect provides access to the entity itself.
		// This is required for registering commands in an EntityCommandBuffer for example.
		public readonly Entity Self;

		// Aspects can contain other aspects.
		readonly TransformAspect Transform;

		// A RefRW field provides read write access to a component. If the aspect is taken as an "in"
		// parameter, the field will behave as if it was a RefRO and will throw exceptions on write attempts.
		readonly RefRW<Ball> Ball;

		// Properties like this are not mandatory, the Transform field could just have been made public instead.
		// But they improve readability by avoiding chains of "aspect.aspect.aspect.component.value.value".
		public float3 Position
		{
			get => Transform.LocalPosition;
			set => Transform.LocalPosition = value;
		}

		public float3 Speed
		{
			get => Ball.ValueRO.Speed;
			set => Ball.ValueRW.Speed = value;
		}
	}
}