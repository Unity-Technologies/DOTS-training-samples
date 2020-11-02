using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Entities.Editor
{
    static class WorldCategoryHelper
    {
        static readonly WorldsChangeDetector WorldsChangeDetector = new WorldsChangeDetector();
        static readonly WorldFlags[] OrderedFlags = { WorldFlags.Live, WorldFlags.Conversion, WorldFlags.Streaming, WorldFlags.Shadow };

        static readonly Dictionary<WorldFlags, string> CategoryNames = new Dictionary<WorldFlags, string>
        {
            { WorldFlags.Live, "Live Worlds" },
            { WorldFlags.Conversion, "Conversion Worlds" },
            { WorldFlags.Streaming, "Streaming Worlds" },
            { WorldFlags.Shadow, "Shadow Worlds" }
        };

        static readonly Dictionary<WorldFlags, List<World>> Collector = new Dictionary<WorldFlags, List<World>>();
        static Category[] s_CachedCategories;

        public static Category[] Categories
        {
            get
            {
                Update();
                return s_CachedCategories;
            }
        }

        static void Update()
        {
            if (!WorldsChangeDetector.WorldsChanged())
                return;

            foreach (var v in Collector.Values)
                v.Clear();

            foreach (var world in World.All)
            {
                var mainFlag = GetMainFlag(world);
                if (mainFlag == WorldFlags.None)
                    continue;

                if (!Collector.TryGetValue(mainFlag, out var category))
                {
                    category = new List<World>();
                    Collector.Add(mainFlag, category);
                }

                category.Add(world);
            }

            var categories = new List<Category>(OrderedFlags.Length);
            for (var i = 0; i < OrderedFlags.Length; i++)
            {
                var flag = OrderedFlags[i];
                if (!Collector.TryGetValue(flag, out var worlds) || worlds.Count == 0)
                    continue;

                categories.Add(new Category(flag, CategoryNames[flag], worlds));
            }

            Array.Resize(ref s_CachedCategories, categories.Count);
            categories.CopyTo(s_CachedCategories);
        }

        static WorldFlags GetMainFlag(World world)
        {
            if ((world.Flags & WorldFlags.Shadow) != 0) return WorldFlags.Shadow;
            if ((world.Flags & WorldFlags.Conversion) != 0) return WorldFlags.Conversion;
            if ((world.Flags & WorldFlags.Live) != 0) return WorldFlags.Live;
            if ((world.Flags & WorldFlags.Streaming) != 0) return WorldFlags.Streaming;
            if ((world.Flags & WorldFlags.Staging) != 0) return WorldFlags.Staging;

            return WorldFlags.None;
        }

        internal class Category
        {
            public Category(WorldFlags flag, string name, IEnumerable<World> worlds)
            {
                Flag = flag;
                Name = name;
                Worlds = worlds.OrderBy(w => w.Name).ToArray();
            }

            public WorldFlags Flag { get; }

            public string Name { get; }

            public World[] Worlds { get; }
        }
    }
}
