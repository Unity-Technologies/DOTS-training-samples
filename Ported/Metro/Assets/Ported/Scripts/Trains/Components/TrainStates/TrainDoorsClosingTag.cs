using Unity.Entities;

namespace MetroECS.Trains.States
{
    public struct TrainDoorsClosingTag : IComponentData
    {
        public float Progress;
    }
}