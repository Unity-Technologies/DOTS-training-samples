using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.Rendering
{
    public static class LODGroupExtensions
    {
        public struct LODParams : IEqualityComparer<LODParams>, IEquatable<LODParams>
        {
            public float  distanceScale;
            public float3 cameraPos;
    
            public bool   isOrtho;
            public float  orthosize;
            
            public bool Equals(LODParams x, LODParams y)
            {
                return
                    x.distanceScale == y.distanceScale &&
                    x.cameraPos.Equals(y.cameraPos) &&
                    x.isOrtho == y.isOrtho &&
                    x.orthosize == y.orthosize;
            }
            
            public bool Equals(LODParams x)
            {
                return
                    x.distanceScale == distanceScale &&
                    x.cameraPos.Equals(cameraPos) &&
                    x.isOrtho == isOrtho &&
                    x.orthosize == orthosize;
            }
    
            public int GetHashCode(LODParams obj)
            {
                throw new System.NotImplementedException();
            }
        }
    
        static float CalculateLodDistanceScale(float fieldOfView, float globalLodBias, bool isOrtho, float orthoSize)
        {
            float distanceScale;
            if (isOrtho)
            {
                distanceScale = 2.0f * orthoSize / globalLodBias;
            }
            else
            {
                var halfAngle = math.tan(math.radians(fieldOfView * 0.5F));
                // Half angle at 90 degrees is 1.0 (So we skip halfAngle / 1.0 calculation)
                distanceScale = (2.0f * halfAngle) / globalLodBias;
            }
    
            return distanceScale;
        }
    
        public static LODParams CalculateLODParams(LODParameters parameters, float overrideLODBias = 0.0f)
        {        
            LODParams lodParams;
            lodParams.cameraPos = parameters.cameraPosition;
            lodParams.isOrtho = parameters.isOrthographic;
            lodParams.orthosize= parameters.orthoSize;
            if (overrideLODBias == 0.0F)
                lodParams.distanceScale = CalculateLodDistanceScale(parameters.fieldOfView, QualitySettings.lodBias, lodParams.isOrtho, lodParams.orthosize);
            else
            {
                // overrideLODBias is not affected by FOV etc
                // This is useful if the FOV is continously changing (breaking LOD temporal cache) or you want to explicit control LOD bias.
                lodParams.distanceScale = 1.0F / overrideLODBias;
            }
    
            return lodParams;
        }
     
        public static LODParams CalculateLODParams(Camera camera, float overrideLODBias = 0.0f)
        {
            LODParams lodParams;
            lodParams.cameraPos = camera.transform.position;
            lodParams.isOrtho = camera.orthographic;
            lodParams.orthosize= camera.orthographicSize;
            if (overrideLODBias == 0.0F)
                lodParams.distanceScale = CalculateLodDistanceScale(camera.fieldOfView, QualitySettings.lodBias, lodParams.isOrtho, lodParams.orthosize);
            else
            {
                // overrideLODBias is not affected by FOV etc.
                // This is useful if the FOV is continously changing (breaking LOD temporal cache) or you want to explicit control LOD bias.
                lodParams.distanceScale = 1.0F / overrideLODBias;
            }
    
            return lodParams;
        }
    
        public static float GetWorldSpaceSize(LODGroup lodGroup)
        {
            return GetWorldSpaceScale(lodGroup.transform) * lodGroup.size;
        }
    
        static float GetWorldSpaceScale(Transform t)
        {
            var scale = t.lossyScale;
            float largestAxis = Mathf.Abs(scale.x);
            largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.y));
            largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.z));
            return largestAxis;
        }
    
        public static int CalculateCurrentLODIndex(float4 lodDistances, float3 worldReferencePoint, ref LODParams lodParams)
        {
            var distanceSqr = CalculateDistanceSqr(worldReferencePoint, ref lodParams);
            var lodIndex = CalculateCurrentLODIndex(lodDistances, distanceSqr);
            return lodIndex;
        }
    
        public static int CalculateCurrentLODMask(float4 lodDistances, float3 worldReferencePoint, ref LODParams lodParams)
        {
            var distanceSqr = CalculateDistanceSqr(worldReferencePoint, ref lodParams);
            return CalculateCurrentLODMask(lodDistances, distanceSqr);
        }
    
        static int CalculateCurrentLODIndex(float4 lodDistances, float measuredDistanceSqr)
        {
            var lodResult = measuredDistanceSqr < (lodDistances * lodDistances);
            if (lodResult.x)
                return 0;
            else if (lodResult.y)
                return 1;
            else if (lodResult.z)
                return 2;
            else if (lodResult.w)
                return 3;
            else
    
                // Can return 0 or 16. Doesn't matter...
                return -1;
        }
    
        static int CalculateCurrentLODMask(float4 lodDistances, float measuredDistanceSqr)
        {
            var lodResult = measuredDistanceSqr < (lodDistances * lodDistances);
            if (lodResult.x)
                return 1;
            else if (lodResult.y)
                return 2;
            else if (lodResult.z)
                return 4;
            else if (lodResult.w)
                return 8;
            else
                // Can return 0 or 16. Doesn't matter...
                return 16;
        }
    
        static float CalculateDistanceSqr(float3 worldReferencePoint, ref LODParams lodParams)
        {
            if (lodParams.isOrtho)
            {
                return lodParams.distanceScale * lodParams.distanceScale;
            }
            else
            {
                return math.lengthsq(lodParams.cameraPos - worldReferencePoint) * (lodParams.distanceScale * lodParams.distanceScale);
            }
        }
    
        public static float3 GetWorldPosition(LODGroup group)
        {
            return group.GetComponent<Transform>().TransformPoint(group.localReferencePoint);
        }
    
        public static float CalculateLODSwitchDistance(float fieldOfView, LODGroup group, int lodIndex)
        {
            float halfAngle = math.tan(math.radians(fieldOfView) * 0.5F);
            return GetWorldSpaceSize(group) / (2 * group.GetLODs()[lodIndex].screenRelativeTransitionHeight * halfAngle);
        }
    }
}
