using Unity.Entities;

public struct IsBee : IComponentData {}
public struct IsResource : IComponentData { }
public struct IsCarried : IComponentData { }
public struct IsAttacking : IComponentData { }
public struct IsGathering : IComponentData { }
public struct IsReturning : IComponentData { }
public struct IsDead : IComponentData { }
public struct HasGravity : IComponentData { }
public struct OnCollision : IComponentData { }