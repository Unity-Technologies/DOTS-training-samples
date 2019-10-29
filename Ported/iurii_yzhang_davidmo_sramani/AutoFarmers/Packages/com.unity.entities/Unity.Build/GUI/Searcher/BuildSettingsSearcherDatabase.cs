using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Properties;
using UnityEditor;
using UnityEditor.Searcher;

namespace Unity.Build
{
    sealed class BuildSettingsSearcherDatabase : SearcherDatabaseBase
    {
        class Result
        {
            public SearcherItem item;
            public float maxScore;
        }

        Func<string, SearcherItem, bool> MatchFilter { get; set; }

        public BuildSettingsSearcherDatabase(IReadOnlyCollection<SearcherItem> db)
            : this("", db)
        {
        }

        BuildSettingsSearcherDatabase(string databaseDirectory, IReadOnlyCollection<SearcherItem> db)
            : base(databaseDirectory)
        {
            m_ItemList = new List<SearcherItem>();
            var nextId = 0;

            if (db != null)
                foreach (var item in db)
                    AddItemToIndex(item, ref nextId, null);
        }

        public override List<SearcherItem> Search(string query, out float localMaxScore)
        {
            localMaxScore = 0;
            if (!string.IsNullOrEmpty(query))
            {
                var filter = FilterUtility.CreateAddComponentFilter(query);

                MatchFilter = (s, item) =>
                {
                    if (!(item is TypeSearcherItem))
                    {
                        return false;
                    }

                    return filter.Keep(item.Name);
                };
            }
            else
            {
                MatchFilter = null;
                return m_ItemList;
            }

            var finalResults = new List<SearcherItem> { null };
            var max = new Result();

            // ReSharper disable once RedundantLogicalConditionalExpressionOperand
            if (m_ItemList.Count > 100)
            {
                SearchMultithreaded(query, max, finalResults);
            }
            else
            {
                SearchSingleThreaded(query, max, finalResults);
            }

            localMaxScore = max.maxScore;
            if (max.item != null)
            {
                finalResults[0] = max.item;
            }
            else
            {
                finalResults.RemoveAt(0);
            }

            return finalResults;
        }

        bool Match(string query, SearcherItem item)
        {
            return MatchFilter?.Invoke(query, item) ?? true;
        }

        void SearchSingleThreaded(string query, Result max, ICollection<SearcherItem> finalResults)
        {
            foreach (var item in m_ItemList)
            {
                if (query.Length == 0 || Match(query, item))
                {
                    finalResults.Add(item);
                }
            }
        }

        void SearchMultithreaded(string query, Result max, List<SearcherItem> finalResults)
        {
            var count = Environment.ProcessorCount;
            var tasks = new Task[count];
            var localResults = new Result[count];
            var queue = new ConcurrentQueue<SearcherItem>();
            var itemsPerTask = (int)Math.Ceiling(m_ItemList.Count / (float)count);

            for (var i = 0; i < count; i++)
            {
                var i1 = i;
                localResults[i1] = new Result();
                tasks[i] = Task.Run(() =>
                {
                    var result = localResults[i1];
                    for (var j = 0; j < itemsPerTask; j++)
                    {
                        var index = j + itemsPerTask * i1;
                        if (index >= m_ItemList.Count)
                            break;
                        var item = m_ItemList[index];
                        if (query.Length == 0 || Match(query, item))
                        {
                            queue.Enqueue(item);
                        }
                    }
                });
            }

            Task.WaitAll(tasks);

            for (var i = 0; i < count; i++)
            {
                if (localResults[i].maxScore > max.maxScore)
                {
                    max.maxScore = localResults[i].maxScore;
                    if (max.item != null)
                        queue.Enqueue(max.item);
                    max.item = localResults[i].item;
                }
                else if (localResults[i].item != null)
                    queue.Enqueue(localResults[i].item);
            }

            finalResults.AddRange(queue.OrderBy(i => i.Id));
        }
    }

    static class ComponentSearcherDatabases
    {
        public static BuildSettingsSearcherDatabase GetBuildSettingsDatabase(HashSet<Type> types)
        {
            return Populate<IBuildSettingsComponent>(types, null);
        }
        
        public static BuildSettingsSearcherDatabase GetBuildStepsDatabase(HashSet<Type> types, Func<Type, string> displayNameResolver)
        {
            return Populate<IBuildStep>(types, displayNameResolver);
        }

        static BuildSettingsSearcherDatabase Populate<T>(HashSet<Type> types, Func<Type, string> displayNameResolver)
        {
                var list = new List<SearcherItem>();
                var dict = new Dictionary<string, SearcherItem>();

                var collection = TypeCache.GetTypesDerivedFrom<T>();
                foreach (var type in collection)
                {
                    if (type.IsGenericType || type.IsAbstract || type.ContainsGenericParameters || type.IsInterface)
                    {
                        continue;
                    }

                    if (!TypeConstruction.HasParameterLessConstructor(type))
                    {
                        continue;
                    }

                    try
                    {
                        if (types.Contains(type))
                        {
                            continue;
                        }

                        var category = string.Empty;
                        TypeSearcherItem typeItem = null;

                        // Fully type-based
                        if (null == displayNameResolver)
                        {
                            typeItem = new TypeSearcherItem(type);
                            category = type.Namespace ?? "Global";
                        }
                        // We control the naming
                        else
                        {
                            var displayName = displayNameResolver.Invoke(type);
                            var prefixIndex = displayName.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);
                            category = prefixIndex >= 0 ? displayName.Substring(0, prefixIndex) : "Other";
                            var name = displayName.Substring(prefixIndex >= 0 ? prefixIndex + 1 : 0);
                            typeItem = new TypeSearcherItem(type, name);
                        }

                        if (!dict.TryGetValue(category, out var item))
                        {
                            dict[category] = item = new SearcherItem(category);
                            list.Add(item);
                        }

                        item.AddChild(typeItem);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                foreach (var kvp in dict)
                {
                    kvp.Value.Children.Sort(CompareByName);
                }

                list.Sort(CompareByName);

                return new BuildSettingsSearcherDatabase(list);
        }

        static int CompareByName(SearcherItem x, SearcherItem y)
        {
            return string.Compare(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
