using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

partial struct BeeTractionPulsionSystem : ISystem {
    private EntityQuery yellowTeamQuery;
    private EntityQuery blueTeamQuery;
    
    public void OnCreate(ref SystemState state) {
        yellowTeamQuery = state.GetEntityQuery(typeof(AttractionComponent), typeof(YellowTeam));
        blueTeamQuery = state.GetEntityQuery(typeof(AttractionComponent), typeof(BlueTeam));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state) {
        var yellowTeam = yellowTeamQuery.ToEntityArray(Allocator.TempJob);
        var blueTeam = blueTeamQuery.ToEntityArray(Allocator.TempJob);

        var yellowTeamPicked = new PickAttractionJob() {
            allies = yellowTeam
        }.ScheduleParallel(yellowTeamQuery, state.Dependency);
        var blueTeamPicked = new PickAttractionJob() {
            allies = blueTeam
        }.ScheduleParallel(blueTeamQuery, state.Dependency);

        var teamsPicked = JobHandle.CombineDependencies(yellowTeamPicked, blueTeamPicked);
        
        state.Dependency = new AttractionJob() {
            teamAttraction = 5,
            teamRepulsion = 4,
            deltaTime = state.Time.DeltaTime,
            positionLookup = state.GetComponentLookup<Position>()
        }.ScheduleParallel(teamsPicked);

        yellowTeam.Dispose(state.Dependency);
        blueTeam.Dispose(state.Dependency);
    }
}