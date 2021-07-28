using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class EmptyBucketPasserUpdate : BucketWorkerUpdateBase
    {
        protected override QueryBuckets WhichBucketsToQuery { get => QueryBuckets.Empty; }

        protected override void OnUpdate()
        {
            // TODO
            //throw new NotImplementedException();
        }
    }
}