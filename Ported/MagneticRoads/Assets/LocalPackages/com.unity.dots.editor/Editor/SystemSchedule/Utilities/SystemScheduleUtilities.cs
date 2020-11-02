using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Entities.Editor
{
    static class SystemScheduleUtilities
    {
        /// <summary>
        /// Get <see cref="Type"/> for update before/after system list for given system type.
        /// <param name="systemType">The given <see cref="ComponentSystemBase"/>.</param>
        /// </summary>
        public static IEnumerable<Type> GetSystemAttributes<TAttribute>(Type systemType)
            where TAttribute : System.Attribute
        {
            var attrArray = TypeManager.GetSystemAttributes(systemType, typeof(TAttribute)).OfType<TAttribute>();
            foreach (var attr in attrArray)
            {
                switch (attr)
                {
                    case UpdateAfterAttribute afterDep:
                        yield return afterDep.SystemType;
                        break;
                    case UpdateBeforeAttribute beforeDep:
                        yield return beforeDep.SystemType;
                        break;
                }
            }
        }

        /// <summary>
        /// Get list of <see cref="Type"/> for update before/after system list for given system types.
        /// <param name="systemType">The given system <see cref="Type"/>.</param>
        /// </summary>
        public static void GetSystemDepListFromSystemTypes(IEnumerable<Type> systemType, List<Type> resultList)
        {
            var index = 0;

            using (var hashPool = PooledHashSet<Type>.Make())
            {
                var hashset = hashPool.Set;

                foreach (var singleSystemType in systemType)
                {
                    var updateBeforeList = GetSystemAttributes<UpdateBeforeAttribute>(singleSystemType);
                    var updateAfterList = GetSystemAttributes<UpdateAfterAttribute>(singleSystemType);

                    if (index == 0)
                    {
                        hashset.UnionWith(updateBeforeList);
                        hashset.UnionWith(updateAfterList);
                    }
                    else
                    {
                        hashset.IntersectWith(updateBeforeList);
                        hashset.IntersectWith(updateAfterList);
                    }

                    index++;
                }

                resultList.Clear();
                resultList.AddRange(hashset);

                if (systemType.Count() == 1)
                    resultList.Add(systemType.First());
            }
        }
    }
}
