using Unity.Entities;

public struct IsArena : IComponentData { }
public struct YellowBase : IComponentData { }
public struct BlueBase : IComponentData { }

public struct IsBee : IComponentData { }
public struct IsResource : IComponentData { }

public struct IsGathering : IComponentData { }
public struct IsCarried : IComponentData { }
public struct IsReturning : IComponentData { }
public struct IsAttacking : IComponentData { }
public struct IsDead : IComponentData { }

public struct HasGravity : IComponentData { }
public struct OnCollision : IComponentData { }

public struct IsOriented : IComponentData { }
public struct IsStretched : IComponentData { }
public struct YellowTeam : IComponentData {}
public struct BlueTeam : IComponentData {}
