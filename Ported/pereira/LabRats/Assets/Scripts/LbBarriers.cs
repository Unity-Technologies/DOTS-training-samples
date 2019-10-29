using Unity.Entities;


[UpdateBefore(typeof(LbSimulationBarrier))]
[UpdateBefore(typeof(LbCheckBarrier))]
public class LbCreationBarrier : EntityCommandBufferSystem { }

public class LbSimulationBarrier : EntityCommandBufferSystem { }

public class LbCheckBarrier : EntityCommandBufferSystem { }

[UpdateAfter(typeof(LbSimulationBarrier))]
[UpdateAfter(typeof(LbCheckBarrier))]
public class LbDestroyBarrier : EntityCommandBufferSystem { }