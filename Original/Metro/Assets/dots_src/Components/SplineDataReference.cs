using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct SplineDataReference : IComponentData
{
    public BlobAssetReference<SplineBlobAssetArray> BlobAssetReference;
}
