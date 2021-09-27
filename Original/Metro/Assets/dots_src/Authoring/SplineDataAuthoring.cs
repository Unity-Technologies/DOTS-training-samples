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
        foreach (var child in transform)
        {
            var railMarkers = GetComponentsInChildren<RailMarker>();
            var locations = railMarkers.Select(r => r.transform.position).ToList();

            BlobBuilder splineBlobBuilder = new BlobBuilder(Allocator.Temp);
            ref var newSplineBlobAsset =  ref splineBlobBuilder.ConstructRoot<SplineBlobAsset>();
            splineBlobBuilder.Allocate(ref newSplineBlobAsset.points, locations.Count() + 1);
            splineBlobBuilder.Allocate(ref newSplineBlobAsset.platformPositions, 1);
            var nbPoints = locations.Count();
            var totalLength = 0.0f;
            for (int i = 0; i < nbPoints; i++)
            {
                newSplineBlobAsset.points[i] = locations[i];
                if (i < nbPoints - 1)
                    totalLength += Vector3.Magnitude(locations[i + 1] - locations[i]);
            }

            newSplineBlobAsset.points[nbPoints] = locations[0];
            newSplineBlobAsset.platformPositions[0] = 0.5f;
            newSplineBlobAsset.length = totalLength;
        }
    }
}
