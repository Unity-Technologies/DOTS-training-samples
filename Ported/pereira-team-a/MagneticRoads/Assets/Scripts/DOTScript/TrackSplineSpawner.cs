using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[RequiresEntityConversion]
public class TrackSplineSpawner : MonoBehaviour
{
    //public int maxSplines;
    //public static NativeArray<TrackSplineDOTS> mySplines;

    //void Start()
    //{
    //    var entityManager = World.Active.EntityManager;
    //    mySplines = new NativeArray<TrackSplineDOTS>(maxSplines,Allocator.Persistent,NativeArrayOptions.ClearMemory);

    //    for (int i = 0; i < maxSplines; i++)
    //    {
    //        TrackSplineDOTS newSpline = new TrackSplineDOTS();
    //        //newSpline.position = new float3(10 * (i + 1), 0, 0);
    //        mySplines[i] = newSpline;
    //    }
    //}

    //private void OnDisable()
    //{
    //    mySplines.Dispose();
    //}
}