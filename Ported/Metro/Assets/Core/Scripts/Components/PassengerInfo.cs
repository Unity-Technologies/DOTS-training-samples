using Unity.Entities;

struct PassengerInfo : IComponentData
{
    public int TrainID;
    public int Seat;
}
