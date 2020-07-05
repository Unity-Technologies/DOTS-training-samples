using HighwayRacer;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace HighwayRacer
{
    public unsafe struct CarBuckets
    {
        public int nSegments;
        public bool IsCreated;
        
        private NativeArray<OtherCarsWriter> otherCarsWriters;
        private NativeArray<UnsafeList> lists;
        
        public CarBuckets(int nSegments)
        {
            this.nSegments = nSegments;
            IsCreated = true;
            
            int nCarsPerSegment = RoadSys.NumCarsFitInStraightLane() * 2;
            var nBuckets = RoadSys.nLanes * nSegments;
            
            otherCarsWriters = new NativeArray<OtherCarsWriter>(nBuckets, Allocator.Persistent);
            lists =  new NativeArray<UnsafeList>(nBuckets * 2, Allocator.Persistent);
            
            var ptr = (UnsafeList*) lists.GetUnsafePtr();

            var idx = 0;
            for (int i = 0; i < nBuckets; i++, idx += 2)
            {
                var positions = ptr + idx;
                var speeds = ptr + idx + 1;
                
                *positions = new UnsafeList(UnsafeUtility.SizeOf<TrackPos>(), UnsafeUtility.AlignOf<TrackPos>(), nCarsPerSegment , Allocator.Persistent);
                *speeds = new UnsafeList(UnsafeUtility.SizeOf<Speed>(), UnsafeUtility.AlignOf<Speed>(), nCarsPerSegment , Allocator.Persistent);
                
                OtherCarsWriter ocw;
                ocw.positions = positions->AsParallelWriter();
                ocw.speeds = speeds->AsParallelWriter();
                otherCarsWriters[i] = ocw;
            }
        }

        public UnsafeList<TrackPos> GetPositions(int lane, int segment)
        {
            var list = otherCarsWriters[lane * nSegments + segment].positions.ListData;
            return new UnsafeList<TrackPos>((TrackPos*)list->Ptr, list->Length);
        }
        
        public UnsafeList<Speed> GetSpeeds(int lane, int segment)
        {
            var list = otherCarsWriters[lane * nSegments + segment].speeds.ListData;
            return new UnsafeList<Speed>((Speed*)list->Ptr, list->Length);
        }
        
        public void AddCar(int lane, TrackSegment trackSegment, TrackPos trackPos, Speed speed, int nSegments)
        {
            var ocw = otherCarsWriters[lane * nSegments + trackSegment.Val];

            ocw.positions.AddNoResize(trackPos);
            ocw.speeds.AddNoResize(speed);
        }

        public void Clear()
        {
            // clear all the lists
            for (int i = 0; i < RoadSys.nLanes * nSegments; i++)
            {
                var ocw = otherCarsWriters[i];

                var posTemp = ocw.positions.ListData;
                posTemp->Clear();

                var speedTemp = ocw.speeds.ListData;
                speedTemp->Clear();
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < lists.Length; i++)
            {
                lists[i].Dispose();
            }

            lists.Dispose();
            otherCarsWriters.Dispose();
        }
    }
    
    public struct OtherCarsWriter
    {
        public UnsafeList.ParallelWriter positions;
        public UnsafeList.ParallelWriter speeds;
    }
}
