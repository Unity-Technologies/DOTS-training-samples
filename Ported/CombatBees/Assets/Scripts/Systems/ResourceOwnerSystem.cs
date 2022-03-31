using System;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    // [UpdateAfter(typeof(GravitySystem))]
    public partial class ResourceOwnerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .ForEach((ref Components.Resource resource, in ResourceOwner resourceOwner) =>
                {
                    if (resourceOwner.Owner != Entity.Null)
                    {
                        if(HasComponent<Translation>(resourceOwner.Owner))
                            resource.OwnerPosition = GetComponent<Translation>(resourceOwner.Owner).Value - new float3(0, 2, 0);
                    }
                }).ScheduleParallel();
        }
    }
}