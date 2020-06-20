using Unity.Entities;

namespace HighwayRacer
{
    public struct OvertakingRight : IComponentData
    {
        public double Time;      // timestamp set when component is added
                                // Removed when blocked by car ahead.
                                // 1. After initiating overtake, car waits some fixed time before attempting to merge right.
                                // 2. Attempts to merge for some fixed time. If time expires, gives up and removes OvertakingRight.
    }
}