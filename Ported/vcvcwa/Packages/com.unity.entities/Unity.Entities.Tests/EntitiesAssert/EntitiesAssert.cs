using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using NUnit.Framework;
using Unity.Collections;

namespace Unity.Entities.Tests
{
    /// <summary>
    /// Contains various test utilities to validate the contents of ECS for comprehensively checking test results.
    /// Important: this is currently only intended for small datasets to keep the code and API simple.
    /// </summary>
    public static class EntitiesAssert
    {
        internal static void AreEqual(IReadOnlyList<DebugEntity> expected, IReadOnlyList<DebugEntity> actual) =>
            Assert.True(PrivateAreEqual(expected, actual));

        internal static void AreNotEqual(IReadOnlyList<DebugEntity> expected, IReadOnlyList<DebugEntity> actual) =>
            Assert.False(PrivateAreEqual(expected, actual));

        static bool PrivateAreEqual(IReadOnlyList<DebugEntity> expected, IReadOnlyList<DebugEntity> actual)
        {
            //@TODO: do this with proper Equals/Equatable impls so can just use nunit's CollectionAssert stuff
            
            if (actual.Count != expected.Count)
                return false;

            for (var i = 0; i < expected.Count; ++i)
            {
                var (de0, de1) = (expected[i], actual[i]);
                if (de0.Entity != de1.Entity)
                    return false;
                if (de0.Components.Count != de1.Components.Count)
                    return false;

                for (var j = 0; j < de0.Components.Count; ++j)
                {
                    var (dc0, dc1) = (de0.Components[j], de1.Components[j]);
                    if (!ReferenceEquals(dc0.Type, dc1.Type))
                        return false;
                    if (dc0.Data is IEnumerable dce0 && dc1.Data is IEnumerable dce1)
                        return dce0.Cast<object>().SequenceEqual(dce1.Cast<object>());
                    if (!Equals(dc0.Data, dc1.Data))
                        return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// Assert that there are no entities in `entityManager`. It will dump a report of what it finds on failure.
        /// </summary>
        public static void IsEmpty(EntityManager entityManager)
        {
            Contains(entityManager, Array.Empty<EntityMatch>(), true);
        }

        /// <summary>
        /// Use this function to validate that the contents of ECS exactly match what is expected. It works like this:
        ///
        /// 1. All entities are retrieved
        /// 2. For each entity, every matcher is tested against. Entities may be in any order, but matchers are tested 
        ///    in the order they are given to this method. This means that the more specific matchers should appear
        ///    earlier in the array.
        /// 3. When a match is found, both the entity and the matcher are paired and removed
        /// 4. If, at the end, there are either matchers or entities remaining, the assertion fails and it dumps a report
        ///
        /// (For how a match is determined in step 3, see docs on the EntityMatch.* methods.)
        /// </summary>
        public static void ContainsOnly(EntityManager entityManager, params EntityMatch[] matchers)
        {
            if (matchers == null || matchers.Length == 0)
                throw new ArgumentException($"Use {nameof(IsEmpty)} to test for empty");

            Contains(entityManager, matchers, true);
        }

        /// <summary>
        /// This method works almost exactly the same as ContainsOnly except that it will only assert if there are
        /// matchers that do not pair with entities. If there are any unmatched entities, it will not assert.
        /// </summary>
        public static void Contains(EntityManager entityManager, params EntityMatch[] matchers)
        {
            if (matchers == null || matchers.Length == 0)
                throw new ArgumentException($"Use {nameof(IsEmpty)} to test for empty");

            Contains(entityManager, matchers, false);
        }
        
        static void Contains(EntityManager entityManager, IEnumerable<EntityMatch> matchers, bool only)
        {
            var entitiesList = DebugEntity
                .GetAllEntities(entityManager)
                .Select((e, i) => (e, mi: -1))
                .ToList();
            var matchersList = matchers
                .Select((m, i) => (m, ei: -1))
                .ToList();

            var (remainingEntities, remainingMatchers) = (entitiesList.Count, matchersList.Count);
            for (var ei = 0; ei < entitiesList.Count; ++ei)
            {
                var entity = entitiesList[ei].e;
                
                for (var mi = 0; mi < matchersList.Count; ++mi)
                {
                    var matcher = matchersList[mi];
                    if (matcher.ei < 0 && matcher.m.IsMatch(entity))
                    {
                        matchersList[mi] = (matcher.m, ei);
                        entitiesList[ei] = (entity, mi);
                        --remainingEntities;
                        --remainingMatchers;
                        break;
                    }
                }
            }

            var fail = false;
            if (remainingMatchers > 0)
                fail = true;
            else if (only && remainingEntities > 0)
                fail = true;
            
            if (fail)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Entities do not match exactly what is expected");
                sb.AppendLine();
                
                sb.AppendLine("Matchers");
                for (var mi = 0; mi < matchersList.Count; ++mi)
                {
                    var ei = matchersList[mi].ei;
                    var es = ei < 0 ? '?' : (char)(ei + 'a');
                    sb.AppendLine($"  {mi}»{es} - {matchersList[mi].m.Desc}");
                }
                sb.AppendLine();
                
                sb.AppendLine("Entities");
                for (var ei = 0; ei < entitiesList.Count; ++ei)
                {
                    var mi = entitiesList[ei].mi;
                    var ms = mi < 0 ? "?" : mi.ToString();
                    var entity = entitiesList[ei].e;
                    var components = string.Join(", ", entity.Components.Select(c => c.ToString(20)).OrderBy(_ => _));
                    sb.AppendLine($"  {(char)(ei + 'a')}»{ms} - {entity.Entity} <{components}>");
                }
                sb.AppendLine();

                throw new AssertionException(sb.ToString());
            }
        }
    }

    public class EntityMatch
    {
        Func<string> m_DescMaker;
        Func<DebugEntity, bool> m_Matcher; 

        EntityMatch(Func<string> descMaker, Func<DebugEntity, bool> matcher)
            => (m_DescMaker, m_Matcher) = (descMaker, matcher);

        internal string Desc => m_DescMaker();
        internal bool IsMatch(DebugEntity entity) => m_Matcher(entity);

        /// <summary>
        /// Custom matcher that is fully user-defined with lambdas.
        /// </summary>
        /// <param name="descMaker">Return a string that describes the matcher. This is called by the assertion failure report</param>
        /// <param name="matcher">Return true when an entity matches.</param>
        public static EntityMatch Where(Func<string> descMaker, Func<DebugEntity, bool> matcher)
            => new EntityMatch(descMaker, matcher);

        /// <summary>
        /// This will do a simple match for the given Entity, ignoring any components.   
        /// </summary>
        public static EntityMatch Any(Entity entity)
            => new EntityMatch(() => $"{entity} /any/", de => de.Entity == entity);

        class DelegateEquals<T>
        {
            Func<T, bool> m_Comparer;
            
            public DelegateEquals(Func<T, bool> comparer) => m_Comparer = comparer;
            public override bool Equals(object obj) => obj is T data && m_Comparer(data);
            public override int GetHashCode() => throw new InvalidOperationException();

            public override string ToString() => $"{typeof(T).Name}=>{{...}}";
        }
        
        /// <summary>
        /// Component matcher that can take a lambda for more flexible matching of components. Pass this in as a `matchData`.
        /// </summary>
        public static object Component<T>(Func<T, bool> comparer) => new DelegateEquals<T>(comparer);
        
        /// <summary>
        /// Use this matcher to validate that the components of an Entity exactly match what is expected. It works like this:
        ///
        /// 1. All components are retrieved
        /// 2. Comparisons for each `matchData` element are are run over each entity
        /// 3. When a match is found, both the component and the `matchData` element are paired and removed
        /// 4. If, at the end, there are either `matchData` elements or components remaining, the matcher returns false
        /// 
        /// (If `matchData` is empty, then it will only match an entity that has no components.)
        ///
        /// Elements in `matchData` can be any of:
        ///
        ///   * An Entity
        ///   * A Type that will return true if it matches the component type
        ///   * An `IEnumerable<Type>` that will return true if all Types are matched against component types
        ///   * An object to compare with Equals against components to validate both the type and its data
        ///     (Use `EntityMatch.Component()` if you want a more dynamic match)
        ///
        /// Note that a IBufferElementData component becomes an array of the component element type, so if you pass in
        /// a MyComponentType[] in `matchData`, the size is verified and also the individual elements are tested using Equals.
        /// </summary>
        public static EntityMatch Exact(params object[] matchData)
            => ExactMatch(Array.Empty<Type>(), matchData);
        public static EntityMatch Exact<T0>(params object[] matchData)
            => ExactMatch(new[] { typeof(T0) }, matchData);
        public static EntityMatch Exact<T0, T1>(params object[] matchData)
            => ExactMatch(new[] { typeof(T0), typeof(T1) }, matchData);
        public static EntityMatch Exact<T0, T1, T2>(params object[] matchData)
            => ExactMatch(new[] { typeof(T0), typeof(T1), typeof(T2) }, matchData);
        public static EntityMatch Exact<T0, T1, T2, T3>(params object[] matchData)
            => ExactMatch(new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3) }, matchData);
        public static EntityMatch Exact<T0, T1, T2, T3, T4>(params object[] matchData)
            => ExactMatch(new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, matchData);
        public static EntityMatch Exact<T0, T1, T2, T3, T4, T5>(params object[] matchData)
            => ExactMatch(new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, matchData);
        public static EntityMatch Exact<T0, T1, T2, T3, T4, T5, T6>(params object[] matchData)
            => ExactMatch(new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) }, matchData);

        static EntityMatch ExactMatch(Type[] componentTypes, object[] matchData)
        {
            if (componentTypes == null || componentTypes.Any(t => t is null))
                throw new ArgumentNullException(nameof(componentTypes), "Expected type cannot be null");
            if (matchData == null || matchData.Any(d => d is null))
                throw new ArgumentNullException(nameof(matchData), "Expected type cannot be null");

            Entity? entity = null;
            var componentTypeList = componentTypes.ToList();
            var componentDataList = new List<object>();
            
            foreach (var data in matchData)
            {
                switch (data) 
                {
                    case Entity e:
                        if (entity != null)
                            throw new ArgumentException("Found multiple Entity componentDatas");
                        entity = e;
                        break;
                    case Type type:
                        componentTypeList.Add(type);
                        break;
                    case IEnumerable<Type> types:
                        componentTypeList.AddRange(types);
                        break;
                    case IEnumerable datas:
                        componentDataList.Add(new DelegateEquals<IEnumerable>(test =>
                            datas.Cast<object>().SequenceEqual(test.Cast<object>())));
                        break;
                    default:
                        componentDataList.Add(data);
                        break;
                }
            }
            
            bool Match(DebugEntity debugEntity)
            {
                if (entity != null && debugEntity.Entity != entity)
                    return false;
                if (debugEntity.Components.Count != componentTypeList.Count + componentDataList.Count)
                    return false;
                
                var debugComponents = debugEntity.Components.ToList();
                
                foreach (var type in componentTypeList)
                {
                    if (!debugComponents.RemoveSwapBack(v => v.Type == type))
                        return false;
                }

                foreach (var data in componentDataList)
                {
                    if (!debugComponents.RemoveSwapBack(v => data.Equals(v.Data)))
                        return false;
                }

                return !debugComponents.Any();
            }

            string MakeDesc()
            {
                var sb = new StringBuilder();
                
                if (entity != null)
                    sb.Append(entity.Value);
                else
                    sb.Append("*");

                var components = Enumerable
                    .Concat(
                        componentTypeList.Select(t => t.Name),
                        componentDataList.Select(d =>
                        {
                            if (d.GetType().IsGenericType && d.GetType().GetGenericTypeDefinition() == typeof(DelegateEquals<>))
                                return d.ToString();
                            return new DebugComponent { Data = d }.ToString(20);
                        }))
                    .OrderBy(_ => _);
                
                var componentsStr = string.Join(", ", components);
                if (componentsStr.Length != 0)
                    sb.Append($" <{componentsStr}>");
                
                sb.Append(" /exact/");

                return sb.ToString();
            }
            
            return new EntityMatch(MakeDesc, Match);
        }
    }
}
