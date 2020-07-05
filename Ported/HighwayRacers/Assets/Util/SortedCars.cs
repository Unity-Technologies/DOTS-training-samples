using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// namespace HighwayRacer
// {
//     public unsafe struct SortedCars
//     {
//         private NativeArray<UnsafeList> listStore;   // where the actual UnsafeList is allocated
//         private UnsafeList.ParallelWriter writer;
//
//         public UnsafeList Cars
//         {
//             get
//             {
//                 return *(writer.ListData);
//             }
//         }
//
//         public SortedCars(int nCars)
//         {
//             listStore = new NativeArray<UnsafeList>(1, Allocator.Persistent);
//             var list = (UnsafeList*) listStore.GetUnsafePtr();
//             *list = new UnsafeList(UnsafeUtility.SizeOf<SortedCar>(), UnsafeUtility.AlignOf<SortedCar>(), nCars, Allocator.Persistent);
//             writer = list->AsParallelWriter();
//         }
//
//         public void Dispose()
//         {
//             if (listStore.IsCreated)
//             {
//                 writer.ListData->Dispose();
//                 listStore.Dispose();
//             }
//         }
//
//         public void Clear()
//         {
//             writer.ListData->Clear();
//         }
//
//         // assumes space is left
//         public void AddCar(float pos, float speed, int lane)
//         {
//             writer.AddNoResize(
//                 new SortedCar()
//                 {
//                     Pos = pos,
//                     Speed = speed,
//                     Lane = lane,
//                 }
//             );
//         }
//
//         public void Sort()
//         {
//             writer.ListData->Sort<SortedCar, CarCompare>(new CarCompare());
//         }
//     }
//
//     public struct CarCompare : IComparer<SortedCar>
//     {
//         public int Compare(SortedCar x, SortedCar y)
//         {
//             if (x.Pos < y.Pos)
//             {
//                 return -1;
//             }
//             else if (x.Pos > y.Pos)
//             {
//                 return 1;
//             }
//
//             // lane is tie breaker
//             if (x.Lane < y.Lane)
//             {
//                 return -1;
//             }
//             else // no two cars with equal Pos should have equal Lane, so we can assume now that (x.Lane > y.Lane)
//             {
//                 return 1;
//             }
//         }
//     }
//
//     public struct SortedCar
//     {
//         public float Pos;
//         public int Lane;
//         public float Speed;
//     }
// }