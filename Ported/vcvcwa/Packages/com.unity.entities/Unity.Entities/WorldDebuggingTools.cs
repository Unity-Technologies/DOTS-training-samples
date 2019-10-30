#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

namespace Unity.Entities
{
    internal static class WorldDebuggingTools
    {
        internal static unsafe void MatchEntityInEntityQueries(World world, Entity entity,
            List<Tuple<ComponentSystemBase, List<EntityQuery>>> matchList)
        {
            foreach (var system in world.Systems)
            {
                if (system == null) continue;
                var queryList = new List<EntityQuery>();
                foreach (var query in system.EntityQueries)
                {
                    if (query.HasFilter())
                        continue;
                    var mask = world.EntityManager.GetEntityQueryMask(query);
                    if (mask.Matches(entity))
                        queryList.Add(query);
                }

                if (queryList.Count > 0)
                    matchList.Add(
                        new Tuple<ComponentSystemBase, List<EntityQuery>>(system, queryList));
            }
        }
    }
}
#endif
