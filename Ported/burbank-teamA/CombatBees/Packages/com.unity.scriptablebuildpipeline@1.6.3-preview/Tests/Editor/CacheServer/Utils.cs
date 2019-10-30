using System;
using System.IO;

namespace UnityEditor.CacheServerTests
{
    public static class Utils
    {
        public static class Paths
        {
            public static string Combine(params string[] components)
            {
                if (components.Length < 1)
                    throw new ArgumentException("At least one component must be provided!");

                var path1 = components[0];

                for (var index = 1; index < components.Length; ++index)
                    path1 = Path.Combine(path1, components[index]);

                return path1;
            }
        }
    }
}
