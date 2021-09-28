using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
public class SplineDataAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    
    {
        BlobBuilder splineBlobBuilder = new BlobBuilder(Allocator.Temp);

        foreach (Transform child in transform)
        {
            var railMarkers = child.GetComponentsInChildren<RailMarker>();
            var locations = railMarkers.Select(r => r.transform.position).ToList();
            ref var newSplineBlobAsset =  ref splineBlobBuilder.ConstructRoot<SplineBlobAsset>();
            var splinePoints = splineBlobBuilder.Allocate(ref newSplineBlobAsset.points, locations.Count() + 1);
            var splinePlatformPositions = splineBlobBuilder.Allocate(ref newSplineBlobAsset.platformPositions, 1);
            var nbPoints = locations.Count();
            var totalLength = 0.0f;
            for (int i = 0; i < nbPoints; i++)
            {
                splinePoints[i] = locations[i];
                if (i < nbPoints - 1)
                    totalLength += Vector3.Magnitude(locations[i + 1] - locations[i]);
            }
            splinePoints[nbPoints] = locations[0];
            splinePlatformPositions[0] = 0.5f;
            newSplineBlobAsset.length = totalLength;
            break;
        }
        
        dstManager.AddComponentData(entity, new SplineDataReference
        {
            BlobAssetReference = splineBlobBuilder.CreateBlobAssetReference<SplineBlobAsset>(Allocator.Persistent)
        });


    }
}
