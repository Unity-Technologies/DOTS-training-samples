using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using static BucketBrigadeUtility;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial class TeamReformSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var entityManager = EntityManager;
        Entities.WithName("ReformTeam")
            .ForEach((ref DynamicBuffer<TeamReformCommand> teamReformCommand) =>
            {
                for (var iTeam = 0; iTeam < teamReformCommand.Length; iTeam++)
                {
                    var command = teamReformCommand[iTeam];

                    var teamInfo = GetComponent<TeamInfo>(command.Team);
                    var members = GetBuffer<Member>(command.Team);
                
                    var captainHome = GetComponent<Home>(teamInfo.Captain);
                    var fetcherHome = GetComponent<Home>(teamInfo.Fetcher);

                    var outMembers = members.Length / 2;

                    var deltaT = 1f / (outMembers + 1);
                    var t = deltaT;

                    for (var i = 0; i < outMembers; i++)
                    {
                        var home = new Home() { Value = CalculateLeftArc(fetcherHome.Value, captainHome.Value, t) };
                        entityManager.SetComponentData<Home>(members[i].Value, home);
                        t += deltaT;
                    }

                    deltaT = 1f / (members.Length - outMembers + 1);
                    t = deltaT;

                    for (var i = outMembers; i < members.Length; i++)
                    {
                        var home = new Home() { Value = CalculateLeftArc(captainHome.Value, fetcherHome.Value, t) };
                        entityManager.SetComponentData<Home>(members[i].Value, home);
                        t += deltaT;
                    }
                }
                
                teamReformCommand.Clear();
            }).Run();
    }
}
