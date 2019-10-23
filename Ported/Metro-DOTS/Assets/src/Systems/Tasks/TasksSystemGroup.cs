using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransitionSystemGroup))]
public class TasksSystemGroup : ComponentSystemGroup { }
