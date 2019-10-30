using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace UnityEditor.Searcher
{
    [PublicAPI]
    public class Searcher
    {
        public ISearcherAdapter Adapter { get; }
        public Comparison<SearcherItem> SortComparison { get; set; }

        readonly List<SearcherDatabaseBase> m_Databases;

        public Searcher(SearcherDatabaseBase database, string title)
            : this(new List<SearcherDatabaseBase> { database }, title, null)
        { }

        public Searcher(IEnumerable<SearcherDatabaseBase> databases, string title)
            : this(databases, title, null)
        { }

        public Searcher(SearcherDatabaseBase database, ISearcherAdapter adapter = null)
            : this(new List<SearcherDatabaseBase> { database }, adapter)
        { }

        public Searcher(IEnumerable<SearcherDatabaseBase> databases, ISearcherAdapter adapter = null)
            : this(databases, string.Empty, adapter)
        { }

        Searcher(IEnumerable<SearcherDatabaseBase> databases, string title, ISearcherAdapter adapter)
        {
            m_Databases = new List<SearcherDatabaseBase>();
            var databaseId = 0;
            foreach (var database in databases)
            {
                // This is needed for sorting items between databases.
                database.OverwriteId(databaseId);
                databaseId++;

                m_Databases.Add(database);
            }

            Adapter = adapter ?? new SearcherAdapter(title);
        }

        public IEnumerable<SearcherItem> Search(string query)
        {
            // TODO: Fix properly!
            // See: https://gitlab.internal.unity3d.com/upm-packages/editor/com.unity.searcher/issues/22
            query = query.ToLower();

            var results = new List<SearcherItem>();
            float maxScore = 0;
            foreach (var database in m_Databases)
            {
                var localResults = database.Search(query, out var localMaxScore);
                if (localMaxScore > maxScore)
                {
                    // skip the highest scored item in the local results and
                    // insert it back as the first item. The first item should always be
                    // the highest scored item. The order of the other items does not matter
                    // because they will be reordered to recreate the tree properly.
                    if (results.Count > 0)
                    {
                        // backup previous best result
                        results.Add(results[0]);
                        // replace it with the new best result
                        results[0] = localResults[0];
                        // add remaining results at the end
                        results.AddRange(localResults.Skip(1));
                    }
                    else // best result will be the first item
                        results.AddRange(localResults);

                    maxScore = localMaxScore;
                }
                else // no new best result just append everything
                {
                    results.AddRange(localResults);
                }
            }

            return results;
        }

        [PublicAPI]
        public class AnalyticsEvent
        {
            [PublicAPI]
            public enum EventType{ Pending, Picked, Cancelled }
            public readonly EventType eventType;
            public readonly string currentSearchFieldText;
            public AnalyticsEvent(EventType eventType, string currentSearchFieldText)
            {
                this.eventType = eventType;
                this.currentSearchFieldText = currentSearchFieldText;
            }
        }
    }
}
