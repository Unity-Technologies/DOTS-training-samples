using Unity.Entities;

namespace dots_src.Components
{
    public enum BoardingStates
    {
        NoTrain,
        Arriving,
        Leaving,
        Embarking,
        Disembarking
    }
    
    [GenerateAuthoringComponent]
    public struct BoardingState : IComponentData
    {
        public BoardingStates State;
        public static implicit operator BoardingStates(BoardingState _states) => _states.State;
    }
}
