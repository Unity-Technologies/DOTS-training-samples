using Onboarding.BezierPath;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class TrackAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    public PathData splineObject;

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem) {

        BlobBuilder builder = new BlobBuilder(Allocator.TempJob);
        ref SplineData data = ref builder.ConstructRoot<SplineData>();

        BlobBuilderArray<float3> pointArray = builder.Allocate(ref data.bezierControlPoints, splineObject.m_BezierControlPoints.Length);
        for (int i = 0; i < pointArray.Length; i++) {
            pointArray[i] = splineObject.m_BezierControlPoints[i];
        }

        BlobBuilderArray<ApproximatedCurveSegment> segmentArray = builder.Allocate(ref data.distanceToParametric, splineObject.m_DistanceToParametric.Length);
        for (int i = 0; i < segmentArray.Length; i++) {
            segmentArray[i] = splineObject.m_DistanceToParametric[i];
        }

        data.pathLength = splineObject.PathLength;

        BlobAssetReference<SplineData> splineBlob = builder.CreateBlobAssetReference<SplineData>(Allocator.Persistent);
        builder.Dispose();
        
        // Station Distance BLobArray<>() To replace or move
        BlobBuilder stationDistanceBuilder = new BlobBuilder(Allocator.TempJob);
        ref StationDistanceArrayData stationArray = ref stationDistanceBuilder.ConstructRoot<StationDistanceArrayData>();
        BlobBuilderArray<float> lengths = stationDistanceBuilder.Allocate(ref stationArray.Distances, 5); //Replace 5 with number of stations
        lengths[0] = 40;
        lengths[1] = 90;
        lengths[2] = 140;
        lengths[3] = 220;
        lengths[4] = 330;
        BlobAssetReference<StationDistanceArrayData> stationBlob = stationDistanceBuilder.CreateBlobAssetReference<StationDistanceArrayData>(Allocator.Temp);
        stationDistanceBuilder.Dispose();
        

        dstManager.AddComponentData(entity, new Spline { splinePath = splineBlob });
        dstManager.AddComponent<Track>(entity);
        dstManager.AddBuffer<FloatBufferElement>(entity);
        
        dstManager.AddComponentData(entity, new StationDistanceArray{StationDistances = stationBlob});
    }
}