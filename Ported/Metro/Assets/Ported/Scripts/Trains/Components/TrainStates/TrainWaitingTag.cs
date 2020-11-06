using Unity.Entities;

namespace MetroECS.Trains.States
{
    public struct TrainWaitingTag : IComponentData
    {
        public double TimeStartedWaiting;
        public float TimeToWait;
    }
}