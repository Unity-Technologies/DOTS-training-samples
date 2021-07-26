using System;
using Unity.Entities;
using UnityEngine;

namespace src.Components
{
    /// <summary>
    ///     All config data for the simulation.
    /// </summary>
    [GenerateAuthoringComponent]
    public struct FireSimConfig : IComponentData
    {
        public Entity FullBucketPasserWorkerPrefab;
        public Entity EmptyBucketPasserWorkerPrefab;
        
        public Entity BucketThrowerWorkerPrefab;
        public Entity OmniWorkerPrefab;
        
        public Entity BucketPrefab;
    }
}