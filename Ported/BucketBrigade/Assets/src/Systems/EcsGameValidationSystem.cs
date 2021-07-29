using System;
using System.Collections.Generic;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace src.Systems
{
    [UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class EcsGameValidationSystem : BucketWorkerUpdateBase
    {
        protected override void OnCreate()
        {
            if (Application.isEditor)
            {
                // Bucket rules:
                ARequiresB<FillingUpBucketTag, EcsBucket>();
                ARequiresB<FullBucketTag, EcsBucket>();
                ARequiresB<BucketIsHeld, EcsBucket>();
                ARequiresB<ThrowBucketAtFire, EcsBucket>();
                ARequiresB<FillingUpBucketTag, BucketIsHeld>();
                ARequiresB<PickUpBucketRequest, EcsBucket>();
                CannotBeAddedToSameEntity<FillingUpBucketTag, FullBucketTag>();
                CannotBeAddedToSameEntity<PickUpBucketRequest, BucketIsHeld>();

                // Worker rules:
                ARequiresB<OmniWorkerTag, WorkerTag>();
                ARequiresB<BucketFetcherWorkerTag, WorkerTag>();
                ARequiresB<EmptyBucketPasserTag, WorkerTag>();
                ARequiresB<FullBucketPasserTag, WorkerTag>();
                ARequiresB<BucketFillersTag, WorkerTag>();
                ARequiresB<WorkerIsHoldingBucket, WorkerTag>();
                CannotBeAddedToSameEntity<WorkerTag, EcsBucket>();
            }
        }

        List<(string, EntityQuery)> m_ValidationQueries = new List<(string, EntityQuery)>(8);
        
        void CannotBeAddedToSameEntity<T1, T2>()
        {
            m_ValidationQueries.Add(($"{typeof(T1)} and {typeof(T2)}.CannotBeAddedToSameEntity", GetEntityQuery(ComponentType.ReadOnly<T1>(), ComponentType.ReadOnly<T2>())));
        }  
        
        void ARequiresB<TA, TB>()
        {
            m_ValidationQueries.Add(($"{typeof(TA)} and {typeof(TB)}.ARequiresB", GetEntityQuery(ComponentType.ReadOnly<TA>(), ComponentType.Exclude<TB>())));
        }  
        
        protected override void OnUpdate()
        {
            foreach (var validationQuery in m_ValidationQueries)
            {
                var validationName = validationQuery.Item1;
                var query = validationQuery.Item2;
                if (!query.IsEmpty)
                {
                    using var invalidEntities = query.ToEntityArray(Allocator.Temp);
                    var errorAggregation = "";
                    for (var i = 0; i < invalidEntities.Length; i++)
                    {
                        var invalidEntity = invalidEntities[i];
                        errorAggregation += $"\n{invalidEntity} '{EntityManager.GetName(invalidEntity)}'";
                    }
                    Debug.LogError($"Validation failed on query: {validationName}! {invalidEntities.Length} invalid entities: {errorAggregation}");
                }
            }
        }

        protected override QueryBuckets WhichBucketsToQuery { get; }
    }
}
