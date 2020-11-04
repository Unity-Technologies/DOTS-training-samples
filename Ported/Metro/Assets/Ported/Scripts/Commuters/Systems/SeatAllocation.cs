using MetroECS.Comuting.States;
using Unity.Entities;

namespace MetroECS.Comuting
{
    public class SeatAllocation : SystemBase
    {
        protected override void OnUpdate()
        {
            // Disembarking
            var disembarkingHandle = Entities
                .WithAll<WaitingForTrainTag>()
                .ForEach((in Commuter commuter, in PlatformID platformID) =>
                {
                    // var carriage = GetEmptiestCarriage(stationID)
                    // var seats = GetAvailableSeats(carriageID)
                    // var seat = AllocateSeat(seats[random], commuter)
                }).ScheduleParallel(Dependency);

            // Embarking
            var embarkingHandle = Entities.ForEach((in Commuter commuter) =>
            {

            }).ScheduleParallel(disembarkingHandle);

            Dependency = embarkingHandle;
        }
    }
}