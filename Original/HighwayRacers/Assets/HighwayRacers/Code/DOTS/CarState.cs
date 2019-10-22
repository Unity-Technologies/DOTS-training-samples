using Unity.Entities;

namespace HighwayRacers
{
    struct CarState : IComponentData
    {
        public float TargetFwdSpeed; //desired FwdSpeed
        public float FwdSpeed; // velocityPosition
        public float LeftSpeed; // velocityLane

        public float PositionOnTrack;
        public float Lane;
        public float TargetLane;
        public float TimeOvertakeCarSet;

        public enum State {
            NORMAL,
            MERGE_RIGHT,
            MERGE_LEFT,
            OVERTAKING,
        }
        public State CurrentState;
    }
}
