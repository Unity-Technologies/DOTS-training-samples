using MetroECS.Comuting.States;
using Unity.Entities;

namespace MetroECS.Comuting
{
    public class CommuterSeatAllocation : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<WaitingForTrainTag>()
                .ForEach((in CommuterTag commuter, in PlatformID platformID) =>
                {
                    // var carriage = GetEmptiestCarriage(stationID)
                    // var seats = GetAvailableSeats(carriageID)
                    // var seat = AllocateSeat(seats[random], commuter)
                }).ScheduleParallel();
        }
    }
}