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

        dstManager.AddComponentData(entity, new Spline { splinePath = splineBlob });
        dstManager.AddComponent<Track>(entity);
        dstManager.AddBuffer<FloatBufferElement>(entity);
        
    }
}