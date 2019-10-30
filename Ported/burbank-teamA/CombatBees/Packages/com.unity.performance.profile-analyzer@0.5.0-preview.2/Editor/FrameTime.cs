
using System;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    public struct FrameTime : IComparable<FrameTime>
    {
        public float ms;
        public int frameIndex;
        public int count;

        public FrameTime(int index, float msTime, int _count)
        {
            frameIndex = index;
            ms = msTime;
            count = _count;
        }
        
        public FrameTime(FrameTime t)
        {
            frameIndex = t.frameIndex;
            ms = t.ms;
            count = t.count;
        }

        public int CompareTo(FrameTime other)
        {
            return ms.CompareTo(other.ms);
        }
        
        public static int CompareMs(FrameTime a, FrameTime b)
        {
            return a.ms.CompareTo(b.ms);
        }
        
        public static int CompareCount(FrameTime a, FrameTime b)
        {
            return a.count.CompareTo(b.count);
        }
    }
}
