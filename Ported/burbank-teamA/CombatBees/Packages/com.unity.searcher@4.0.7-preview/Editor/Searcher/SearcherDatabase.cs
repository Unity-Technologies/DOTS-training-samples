using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityEditor.Searcher
{
    [PublicAPI]
    public class SearcherDatabase : SearcherDatabaseBase
    {
        class Result
        {
            public SearcherItem item;
            public float maxScore;
        }

        const bool k_IsParallel = true;

        public Func<string, SearcherItem, bool> MatchFilter { get; set; }

        public static SearcherDatabase Create(
            List<SearcherItem> items,
            string databaseDirectory,
            bool serializeToFile = true
        )
        {
            if (serializeToFile && databaseDirectory != null && !Directory.Exists(databaseDirectory))
                Directory.CreateDirectory(databaseDirectory);

            var database = new SearcherDatabase(databaseDirectory, items);

            if (serializeToFile)
                database.SerializeToFile();

            return database;
        }

        public static SearcherDatabase Load(string databaseDirectory)
        {
            if (!Directory.Exists(databaseDirectory))
                throw new InvalidOperationException("databaseDirectory not found.");

            var database = new SearcherDatabase(databaseDirectory, null);
            database.LoadFromFile();

            return database;
        }

        public SearcherDatabase(IReadOnlyCollection<SearcherItem> db)
            : this("", db)
        {
        }

        SearcherDatabase(string databaseDirectory, IReadOnlyCollection<SearcherItem> db)
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
            // Match assumes the query is trimmed
            query = query.Trim(' ', '\t');
            localMaxScore = 0;

            if (string.IsNullOrWhiteSpace(query))
            {
                if (MatchFilter == null)
                    return m_ItemList;

                // ReSharper disable once RedundantLogicalConditionalExpressionOperand
                if (k_IsParallel && m_ItemList.Count > 100)
                    return FilterMultiThreaded(query);

                return FilterSingleThreaded(query);
            }

            var finalResults = new List<SearcherItem> { null };
            var max = new Result();

            // ReSharper disable once RedundantLogicalConditionalExpressionOperand
            if (k_IsParallel && m_ItemList.Count > 100)
                SearchMultithreaded(query, max, finalResults);
            else
                SearchSingleThreaded(query, max, finalResults);

            localMaxScore = max.maxScore;
            if (max.item != null)
                finalResults[0] = max.item;
            else
                finalResults.RemoveAt(0);

            return finalResults;
        }

        protected virtual bool Match(string query, SearcherItem item, out float score)
        {
            var filter = MatchFilter?.Invoke(query, item) ?? true;
            return Match(query, item.Path, out score) && filter;
        }

        List<SearcherItem> FilterSingleThreaded(string query)
        {
            var result = new List<SearcherItem>();

            foreach (var searcherItem in m_ItemList)
            {
                if (!MatchFilter.Invoke(query, searcherItem))
                    continue;

                result.Add(searcherItem);
            }

            return result;
        }

        List<SearcherItem> FilterMultiThreaded(string query)
        {
            var result = new List<SearcherItem>();
            var count = Environment.ProcessorCount;
            var tasks = new Task[count];
            var lists = new List<SearcherItem>[count];
            var itemsPerTask = (int)Math.Ceiling(m_ItemList.Count / (float)count);

            for (var i = 0; i < count; i++)
            {
                var i1 = i;
                tasks[i] = Task.Run(() =>
                {
                    lists[i1] = new List<SearcherItem>();

                    for (var j = 0; j < itemsPerTask; j++)
                    {
                        var index = j + itemsPerTask * i1;
                        if (index >= m_ItemList.Count)
                            break;

                        var item = m_ItemList[index];
                        if (!MatchFilter.Invoke(query, item))
                            continue;

                        lists[i1].Add(item);
                    }
                });
            }

            Task.WaitAll(tasks);

            for (var i = 0; i < count; i++)
            {
                result.AddRange(lists[i]);
            }

            return result;
        }

        void SearchSingleThreaded(string query, Result max, ICollection<SearcherItem> finalResults)
        {
            foreach (var item in m_ItemList)
            {
                float score = 0;
                if (query.Length == 0 || Match(query, item, out score))
                {
                    if (score > max.maxScore)
                    {
                        if (max.item != null)
                            finalResults.Add(max.item);
                        max.item = item;
                        max.maxScore = score;
                    }
                    else
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
                        float score = 0;
                        if (query.Length == 0 || Match(query, item, out score))
                        {
                            if (score > result.maxScore)
                            {
                                if (result.item != null)
                                    queue.Enqueue(result.item);
                                result.maxScore = score;
                                result.item = item;
                            }
                            else
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

        static int NextSeparator(string s, int index)
        {
            for (; index < s.Length; index++)
                if (IsWhiteSpace(s[index])) // || char.IsUpper(s[index]))
                    return index;
            return -1;
        }

        static bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t';
        }

        static char ToLowerAsciiInvariant(char c)
        {
            if ('A' <= c && c <= 'Z')
                c |= ' ';
            return c;
        }

        static bool StartsWith(string s, int sStart, int sCount, string prefix, int prefixStart, int prefixCount)
        {
            if (prefixCount > sCount)
                return false;
            for (var i = 0; i < prefixCount; i++)
            {
                if (ToLowerAsciiInvariant(s[sStart + i]) != ToLowerAsciiInvariant(prefix[prefixStart + i]))
                    return false;
            }

            return true;
        }

        static bool Match(string query, string itemPath, out float score)
        {
            int queryPartStart = 0;
            int pathPartStart = 0;

            score = 0;
            var skipped = 0;
            do
            {
                // skip remaining spaces in path
                while (pathPartStart < itemPath.Length && IsWhiteSpace(itemPath[pathPartStart]))
                    pathPartStart++;

                // query is not done, nothing remaining in path, failure
                if (pathPartStart > itemPath.Length - 1)
                {
                    score = 0;
                    return false;
                }

                // skip query spaces. notice the + 1
                while (queryPartStart < query.Length && IsWhiteSpace(query[queryPartStart]))
                    queryPartStart++;

                // find next separator in query
                int queryPartEnd = query.IndexOf(' ', queryPartStart);
                if (queryPartEnd == -1)
                    queryPartEnd = query.Length; // no spaces, take everything remaining

                // next space, starting after the path part last char
                int pathPartEnd = NextSeparator(itemPath, pathPartStart + 1);
                if (pathPartEnd == -1)
                    pathPartEnd = itemPath.Length;


                int queryPartLength = queryPartEnd - queryPartStart;
                int pathPartLength = pathPartEnd - pathPartStart;
                bool match = StartsWith(itemPath, pathPartStart, pathPartLength,
                    query, queryPartStart, queryPartLength);

                pathPartStart = pathPartEnd;

                if (!match)
                {
                    skipped++;
                    continue;
                }

                score += queryPartLength / (float)Mathf.Max(1, pathPartLength);
                if (queryPartEnd == query.Length)
                {
                    int pathPartCount = 1;
                    while (-1 != pathPartStart)
                    {
                        pathPartStart = NextSeparator(itemPath, pathPartStart + 1);
                        pathPartCount++;
                    }

                    int queryPartCount = 1;
                    while (-1 != queryPartStart)
                    {
                        queryPartStart = NextSeparator(query, queryPartStart + 1);
                        pathPartCount++;
                    }

                    score *= queryPartCount / (float)pathPartCount;
                    score *= 1 / (1.0f + skipped);

                    return true; // successfully matched all query parts
                }

                queryPartStart = queryPartEnd + 1;
            } while (true);
        }
    }
}
