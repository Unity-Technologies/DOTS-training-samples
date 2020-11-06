using Unity.Entities;

namespace MetroECS.Trains.States
{
    public struct TrainDoorsOpeningTag : IComponentData
    {
        public float Progress;
    }
}