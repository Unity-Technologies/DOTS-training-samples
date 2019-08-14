
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[RequiresEntityConversion]
public class TrackSplineSpawner_TEST : MonoBehaviour
{

    public static NativeArray<TrackSplineDOTS_TEST> myNativeArray;

    void Start()
    {

        //Hardcoded Path Spline

        TrackSplineDOTS_TEST newT = new TrackSplineDOTS_TEST();
        newT.id = 0;
        newT.startPoint = new float3(0, 0, 0);
        newT.endPoint = new float3(10, 0, 0);
        newT.rotation = Quaternion.FromToRotation(Vector3.up, newT.endPoint - newT.startPoint);
        newT.forward = newT.endPoint - newT.startPoint;
        newT.SetNeighbors(-1, -1, -1);

        TrackSplineDOTS_TEST newTL = new TrackSplineDOTS_TEST();
        newTL.id = 1;
        newTL.startPoint = new float3(10, 0, 0);
        newTL.endPoint = new float3(10, 0, 10);
        newTL.rotation = Quaternion.FromToRotation(Vector3.up, newTL.endPoint - newTL.startPoint);
        newTL.forward = newTL.endPoint - newTL.startPoint;
        newTL.SetNeighbors(-1, -1, -1);

        TrackSplineDOTS_TEST newTR = new TrackSplineDOTS_TEST();
        newTR.id = 2;
        newTR.startPoint = new float3(10, 0, 0);
        newTR.endPoint = new float3(10, 0, -10);
        newTR.rotation = Quaternion.FromToRotation(Vector3.up, newTR.endPoint - newTR.startPoint);
        newTR.forward = newTR.endPoint - newTR.startPoint;
        newTR.SetNeighbors(-1, -1, -1);


        TrackSplineDOTS_TEST newT2 = new TrackSplineDOTS_TEST();
        newT2.id = 3;
        newT2.startPoint = new float3(10, 0, 0);
        newT2.endPoint = new float3(10, 7, 0);
        newT2.rotation = Quaternion.FromToRotation(Vector3.up, newT2.endPoint - newT2.startPoint);
        newT2.forward = newT2.endPoint - newT2.startPoint;
        newT2.SetNeighbors(-1, -1, -1);

        TrackSplineDOTS_TEST newT2L = new TrackSplineDOTS_TEST();
        newT2L.id = 4;
        newT2L.startPoint = new float3(10, 7, 0);
        newT2L.endPoint = new float3(10, 7, 10);
        newT2L.rotation = Quaternion.FromToRotation(Vector3.up, newT2L.endPoint - newT2L.startPoint);
        newT2L.forward = newTL.endPoint - newTL.startPoint;
        newT2L.SetNeighbors(-1, -1, -1);

        TrackSplineDOTS_TEST newT2R = new TrackSplineDOTS_TEST();
        newT2R.id = 5;
        newT2R.startPoint = new float3(10, 7, 0);
        newT2R.endPoint = new float3(10, 7, -10);
        newT2R.rotation = Quaternion.FromToRotation(Vector3.up, newT2R.endPoint - newT2R.startPoint);
        newT2R.forward = newT2R.endPoint - newT2R.startPoint;
        newT2R.SetNeighbors(-1, -1, -1);


        TrackSplineDOTS_TEST newT3 = new TrackSplineDOTS_TEST();
        newT3.id = 6;
        newT3.startPoint = new float3(10, 7, 0);
        newT3.endPoint = new float3(20, 10, 0);
        newT3.rotation = Quaternion.FromToRotation(Vector3.up, newT3.endPoint - newT3.startPoint);
        newT3.forward = newT3.endPoint - newT3.startPoint;
        newT3.SetNeighbors(-1, -1, -1);

        TrackSplineDOTS_TEST newT3L = new TrackSplineDOTS_TEST();
        newT3L.id = 7;
        newT3L.startPoint = new float3(20, 10, 0);
        newT3L.endPoint = new float3(20, 10, 10);
        newT3L.rotation = Quaternion.FromToRotation(Vector3.up, newT3L.endPoint - newT3L.startPoint);
        newT3L.forward = newT3L.endPoint - newT3L.startPoint;
        newT3L.SetNeighbors(-1, -1, -1);

        TrackSplineDOTS_TEST newT3R = new TrackSplineDOTS_TEST();
        newT3R.id = 8;
        newT3R.startPoint = new float3(20, 10, 0);
        newT3R.endPoint = new float3(20, 10, -10);
        newT3R.rotation = Quaternion.FromToRotation(Vector3.up, newT3R.endPoint - newT3R.startPoint);
        newT3R.forward = newT3R.endPoint - newT3R.startPoint;
        newT3R.SetNeighbors(-1, -1, -1);


        TrackSplineDOTS_TEST newT4 = new TrackSplineDOTS_TEST();
        newT4.id = 9;
        newT4.startPoint = new float3(20, 10, 0);
        newT4.endPoint = new float3(25, 5, 0);
        newT4.rotation = Quaternion.FromToRotation(Vector3.up, newT4.endPoint - newT4.startPoint);
        newT4.forward = newT4.endPoint - newT4.startPoint;
        newT4.SetNeighbors(-1, -1, -1);

        TrackSplineDOTS_TEST newT4L = new TrackSplineDOTS_TEST();
        newT4L.id = 10;
        newT4L.startPoint = new float3(25, 5, 0);
        newT4L.endPoint = new float3(25, 5, 10);
        newT4L.rotation = Quaternion.FromToRotation(Vector3.up, newT4L.endPoint - newT4L.startPoint);
        newT4L.forward = newT4L.endPoint - newT4L.startPoint;
        newT4L.SetNeighbors(-1, -1, -1);

        TrackSplineDOTS_TEST newT4R = new TrackSplineDOTS_TEST();
        newT4R.id = 11;
        newT4R.startPoint = new float3(25, 5, 0);
        newT4R.endPoint = new float3(25, 5, -10);
        newT4R.rotation = Quaternion.FromToRotation(Vector3.up, newT4R.endPoint - newT4R.startPoint);
        newT4R.forward = newT4R.endPoint - newT4R.startPoint;
        newT4R.SetNeighbors(-1,-1,-1);

        newT.SetNeighbors(newT2.id, newTL.id, newTR.id);
        newT2.SetNeighbors(newT3.id, newT2L.id, newT2R.id);
        newT3.SetNeighbors(newT4.id, newT3L.id, newT3R.id);
        newT4.SetNeighbors(newT4.id, newT4L.id, newT4R.id);

        TrackSplineDOTS_TEST[] test = new TrackSplineDOTS_TEST[12];
        test[0] = newT;
        test[1] = newT2;
        test[2] = newT3;
        test[3] = newT4;
        test[4] = newTL;
        test[5] = newT2L;
        test[6] = newT3L;
        test[7] = newT4L;
        test[8] = newTR;
        test[9] = newT2R;
        test[10] = newT3R;
        test[11] = newT4R;

        myNativeArray = new NativeArray<TrackSplineDOTS_TEST>(test,Allocator.Persistent);

    }

    public void OnDestroy()
    {
        myNativeArray.Dispose();
    }

    public void OnDrawGizmos()
    {
        for (int i = 0; i < myNativeArray.Length; i++)
        {
            Gizmos.DrawWireCube(myNativeArray[i].startPoint,new Vector3(1,1,1));
            Gizmos.DrawWireCube(myNativeArray[i].endPoint, new Vector3(1, 1, 1));
        }
    }
}
