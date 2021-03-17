using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class MetroBlobAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public List<GameObject> LineMarkers;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var builder = new BlobBuilder(Allocator.Temp);
        ref MetroBlob metroBlob = ref builder.ConstructRoot<MetroBlob>();

        BlobBuilderArray<MetroLineBlob> lineArrayBuilder = builder.Allocate(ref metroBlob.Lines, 4);

        var platforms = new List<PlatformBlob>();

        var currentPlatform = 0;

        for (var i = 0; i < 4; i++)
        {
            var lineMarkerParent = LineMarkers[i];
            var railMarkers = lineMarkerParent.GetComponentsInChildren<RailMarker>();
            var generated = BlobCreationUtilities.Create_RailPath(railMarkers.ToList());

            BlobBuilderArray<LinePoint> linePointArrayBuilder = builder.Allocate(ref lineArrayBuilder[i].Path, generated.linePoints.Length);

            for (int j = 0; j < generated.linePoints.Length; j++)
            {
                linePointArrayBuilder[j] = generated.linePoints[j];
            }

            lineArrayBuilder[i].FirstPlatform = currentPlatform;
            lineArrayBuilder[i].PlatformCount = generated.platforms.Length;
            currentPlatform += generated.platforms.Length;
            
            platforms.AddRange(generated.platforms);
        }

        BlobBuilderArray<PlatformBlob> platformArrayBuilder = builder.Allocate(ref metroBlob.Platforms, platforms.Count);
        for (int i = 0; i < platforms.Count; i++)
        {
            platformArrayBuilder[i] = platforms[i];
        }

        var result = builder.CreateBlobAssetReference<MetroBlob>(Allocator.Persistent); 
        builder.Dispose();

        var blobContainer = new MetroBlobContaner {Blob = result};
        dstManager.AddComponentData(entity, blobContainer);
        
        // Debug.Log(result.Value.Lines.Length);
        //
        // ref var metro = ref result.Value;
        // for (var index = 0; index < metro.Lines.Length; index++)
        // {
        //     Debug.Log($"{metro.Lines[index].FirstPlatform} {metro.Lines[index].PlatformCount}");
        //     
        //     ref var points = ref metro.Lines[index].Path;
        //     for (var i = 0; i < points.Length; i++)
        //     {
        //         Debug.Log(points[i].location);
        //     }
        // }

        // Call methods on 'dstManager' to create runtime components on 'entity' here. Remember that:
        //
        // * You can add more than one component to the entity. It's also OK to not add any at all.
        //
        // * If you want to create more than one entity from the data in this class, use the 'conversionSystem'
        //   to do it, instead of adding entities through 'dstManager' directly.
        //
        // For example,
        //   dstManager.AddComponentData(entity, new Unity.Transforms.Scale { Value = scale });


    }
}
