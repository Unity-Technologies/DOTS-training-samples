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
        using (var splineBlobBuilder = new BlobBuilder(Allocator.Temp))
        {
            ref var splineDataArray = ref splineBlobBuilder.ConstructRoot<SplineBlobAssetArray>();

            var splineArray = splineBlobBuilder.Allocate(ref splineDataArray.splineBlobAssets, transform.childCount);
            int lineId = 0;
            foreach (Transform child in transform)
            {
                var railMarkers = child.GetComponentsInChildren<RailMarker>();
                var locations = railMarkers.Select(r => r.transform.position).ToList();
                ref var newSplineBlobAsset = ref splineArray[lineId++];
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
            }

            BlobAssetReference<SplineBlobAssetArray> blobAssetReference =
                splineBlobBuilder.CreateBlobAssetReference<SplineBlobAssetArray>(Allocator.Persistent);
            dstManager.AddComponentData(entity, new SplineDataReference
            {
                BlobAssetReference = blobAssetReference
            });
        }
    }
}
