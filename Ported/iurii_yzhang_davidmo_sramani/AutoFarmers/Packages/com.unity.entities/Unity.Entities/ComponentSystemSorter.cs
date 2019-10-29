using System;
using System.Collections.Generic;
#if !NET_DOTS
using System.Linq;
#endif
using Unity;
using Unity.Entities;

namespace Unity.Entities
{

    public class ComponentSystemSorter
    {
        public class CircularSystemDependencyException : Exception
        {
            public CircularSystemDependencyException(IEnumerable<Type> chain)
            {
                Chain = chain;
#if NET_DOTS
                var lines = new List<string>();
                Console.WriteLine($"The following systems form a circular dependency cycle (check their [UpdateBefore]/[UpdateAfter] attributes):");
                foreach (var s in Chain)
                {
                    int index = TypeManager.GetSystemTypeIndex(s);
                    string name = TypeManager.SystemNames[index];
                    Console.WriteLine(name);
                }
#endif
            }

            public IEnumerable<Type> Chain { get; }

#if !NET_DOTS
            public override string Message
            {
                get
                {
                    var lines = new List<string>
                    {
                        $"The following systems form a circular dependency cycle (check their [UpdateBefore]/[UpdateAfter] attributes):"
                    };
                    foreach (var s in Chain)
                    {
                        lines.Add($"- {s.ToString()}");
                    }

                    return lines.Aggregate((str1, str2) => str1 + "\n" + str2);
                }
            }
#endif
        }

        private class Heap<T>
            where T : IComparable<T>
        {
            private readonly T[] _elements;
            private int _size;
            private readonly int _capacity;
            private static readonly int BaseIndex = 1;

            public Heap(int capacity)
            {
                _capacity = capacity;
                _size = 0;
                _elements = new T[capacity + BaseIndex];
            }

            public bool Empty => _size <= 0;

            public void Insert(T e)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (_size >= _capacity)
                {
                    throw new InvalidOperationException($"Attempted to Insert() to a full heap.");
                }
#endif
                var i = BaseIndex + _size++;
                while (i > BaseIndex)
                {
                    var parent = i / 2;

                    if (e.CompareTo(_elements[parent]) > 0)
                    {
                        break;
                    }

                    _elements[i] = _elements[parent];
                    i = parent;
                }

                _elements[i] = e;
            }

            public T Peek()
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (Empty)
                {
                    throw new InvalidOperationException($"Attempted to Peek() an empty heap.");
                }
#endif
                return _elements[BaseIndex];
            }

            public T Extract()
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (Empty)
                {
                    throw new InvalidOperationException($"Attempted to Extract() from an empty heap.");
                }
#endif
                T top = _elements[BaseIndex];
                _elements[BaseIndex] = _elements[_size--];
                if (!Empty)
                {
                    Heapify(BaseIndex);
                }

                return top;
            }

            private void Heapify(int i)
            {
                // The index taken by this function is expected to be already biased by BaseIndex.
                // Thus, m_Heap[size] is a valid element (specifically, the final element in the heap)
                //Debug.Assert(i >= BaseIndex && i < (_size+BaseIndex), $"heap index {i} is out of range with size={_size}");
                T val = _elements[i];
                while (i <= _size / 2)
                {
                    var child = 2 * i;
                    if (child < _size && _elements[child + 1].CompareTo(_elements[child]) < 0)
                    {
                        child++;
                    }

                    if (val.CompareTo(_elements[child]) < 0)
                    {
                        break;
                    }

                    _elements[i] = _elements[child];
                    i = child;
                }

                _elements[i] = val;
            }
        }

        private struct SysAndDep<T>
        {
            public T item;
            public Type type;
            public List<Type> updateBefore;
            public int nAfter;
        }

        public struct TypeHeapElement : IComparable<TypeHeapElement>
        {
            private readonly string typeName;
            public int unsortedIndex;

            public TypeHeapElement(int index, Type t)
            {
                unsortedIndex = index;
                typeName = TypeManager.SystemName(t);
            }

            public int CompareTo(TypeHeapElement other)
            {
                // Workaround for missing string.CompareTo() in HPC#. This is not a fully compatible substitute,
                // but should be suitable for comparing system names.
                if (typeName.Length < other.typeName.Length)
                    return -1;
                if (typeName.Length > other.typeName.Length)
                    return 1;
                for (int i = 0; i < typeName.Length; ++i)
                {
                    if (typeName[i] < other.typeName[i])
                        return -1;
                    if (typeName[i] > other.typeName[i])
                        return 1;
                }
                return 0;
            }
        }

        // Tiny doesn't have a data structure that can take Type as a key.
        // For now, this gives Tiny a linear search. Would like to do better.
#if !NET_DOTS
        private static Dictionary<Type, int> lookupDictionary = null;

        private static int LookupSysAndDep<T>(Type t, SysAndDep<T>[] array)
        {
            if (lookupDictionary == null)
            {
                lookupDictionary = new Dictionary<Type, int>();
                for (int i = 0; i < array.Length; ++i)
                {
                    lookupDictionary[array[i].type] = i;
                }
            }

            if (lookupDictionary.ContainsKey(t))
                return lookupDictionary[t];
            return -1;
        }
#else
        private static int LookupSysAndDep<T>(Type t, SysAndDep<T>[] array)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                if (array[i].item != null && array[i].type == t)
                {
                    return i;
                }
            }
            return -1;
        }
#endif

        private static void WarningForBeforeCheck(Type sysType, Type depType)
        {
#if !NET_DOTS
            Debug.LogWarning(
                $"Ignoring redundant [UpdateBefore] attribute on {sysType} because {depType} is already restricted to be last.\n"
                + $"Set the target parameter of [UpdateBefore] to a different system class in the same {nameof(ComponentSystemGroup)} as {sysType}.");
#else
            Debug.LogWarning($"WARNING: invalid [UpdateBefore] attribute:");
            Debug.LogWarning(TypeManager.SystemName(sysType));
            Debug.LogWarning("  is a redundant update before a system that is restricted to be last: ");
            Debug.LogWarning(TypeManager.SystemName(depType));
            Debug.LogWarning("Set the target parameter of [UpdateBefore] to a system class in the same ComponentSystemGroup.");
#endif
        }

        private static void WarningForAfterCheck(Type sysType, Type depType)
        {
#if !NET_DOTS
            Debug.LogWarning(
                $"Ignoring redundant [UpdateAfter] attribute on {sysType} because {depType} is already restricted to be first.\n"
                + $"Set the target parameter of [UpdateAfter] to a different system class in the same {nameof(ComponentSystemGroup)} as {sysType}.");
#else
            Debug.LogWarning($"WARNING: invalid [UpdateAfter] attribute:");
            Debug.LogWarning(TypeManager.SystemName(sysType));
            Debug.LogWarning("  is a redundant update before a system that is restricted to be first: ");
            Debug.LogWarning(TypeManager.SystemName(depType));
            Debug.LogWarning("Set the target parameter of [UpdateBefore] to a system class in the same ComponentSystemGroup.");
#endif
        }

        internal delegate Type GetSystemType<T>(T item);

        internal static void Sort<T>(List<T> items, GetSystemType<T> getType, Type parentType)
        {
#if !NET_DOTS
            lookupDictionary = null;
#endif
            // Populate dictionary mapping systemType to system-and-before/after-types.
            // This is clunky - it is easier to understand, and cleaner code, to
            // just use a Dictionary<Type, SysAndDep>. However, Tiny doesn't currently have
            // the ability to use Type as a key to a NativeHash, so we're stuck until that gets addressed.
            //
            // Likewise, this is important shared code. It can be done cleaner with 2 versions, but then...
            // 2 sets of bugs and slightly different behavior will creep in.
            //
            var sysAndDep = new SysAndDep<T>[items.Count];

            for (int i = 0; i < items.Count; ++i)
            {
                var sys = items[i];

                sysAndDep[i] = new SysAndDep<T>
                {
                    item = sys,
                    type = getType(sys),
                    updateBefore = new List<Type>(),
                    nAfter = 0,
                };
            }

            for (int i = 0; i < sysAndDep.Length; ++i)
            {
                var systemType = sysAndDep[i].type;

                var before = TypeManager.GetSystemAttributes(systemType, typeof(UpdateBeforeAttribute));
                var after = TypeManager.GetSystemAttributes(systemType, typeof(UpdateAfterAttribute));
                foreach (var attr in before)
                {
                    var dep = attr as UpdateBeforeAttribute;
                    if (!typeof(ComponentSystemBase).IsAssignableFrom(dep.SystemType))
                    {
#if !NET_DOTS
                        Debug.LogWarning(
                            $"Ignoring invalid [UpdateBefore] attribute on {systemType} because {dep.SystemType} is not a subclass of {nameof(ComponentSystemBase)}.\n"
                            + $"Set the target parameter of [UpdateBefore] to a system class in the same {nameof(ComponentSystemGroup)} as {systemType}.");
#else
                        Debug.LogWarning($"WARNING: invalid [UpdateBefore] attribute:");
                        Debug.LogWarning(TypeManager.SystemName(dep.SystemType));
                        Debug.LogWarning(" is not derived from ComponentSystemBase. Set the target parameter of [UpdateBefore] to a system class in the same ComponentSystemGroup.");
#endif
                        continue;
                    }

                    if (dep.SystemType == systemType)
                    {
#if !NET_DOTS
                        Debug.LogWarning(
                            $"Ignoring invalid [UpdateBefore] attribute on {systemType} because a system cannot be updated before itself.\n"
                            + $"Set the target parameter of [UpdateBefore] to a different system class in the same {nameof(ComponentSystemGroup)} as {systemType}.");
#else
                        Debug.LogWarning($"WARNING: invalid [UpdateBefore] attribute:");
                        Debug.LogWarning(TypeManager.SystemName(systemType));
                        Debug.LogWarning("  depends on itself. Set the target parameter of [UpdateBefore] to a system class in the same ComponentSystemGroup.");
#endif
                        continue;
                    }

                    if (parentType == typeof(InitializationSystemGroup))
                    {
                        if (dep.SystemType == typeof(BeginInitializationEntityCommandBufferSystem))
                        {
#if !NET_DOTS
                            throw new ArgumentException(
                                $"Invalid [UpdateBefore] {dep.SystemType} attribute on {systemType}, because that system is already restricted to be first.");
#else
                            throw new ArgumentException($"Invalid [UpdateBefore] BeginInitializationEntityCommandBufferSystem, because that system is already restricted to be first.");
#endif
                        }

                        if (dep.SystemType == typeof(EndInitializationEntityCommandBufferSystem))
                        {
                            WarningForBeforeCheck(systemType, dep.SystemType);
                            continue;
                        }
                    }

                    if (parentType == typeof(SimulationSystemGroup))
                    {
                        if (dep.SystemType == typeof(BeginSimulationEntityCommandBufferSystem))
                        {
#if !NET_DOTS
                            throw new ArgumentException(
                                $"Invalid [UpdateBefore] {dep.SystemType} attribute on {systemType}, because that system is already restricted to be first.");
#else
                            throw new ArgumentException($"Invalid [UpdateBefore] BeginSimulationEntityCommandBufferSystem, because that system is already restricted to be first.");
#endif
                        }

                        if (dep.SystemType == typeof(LateSimulationSystemGroup))
                        {
                            WarningForBeforeCheck(systemType, dep.SystemType);
                            continue;
                        }

                        if (dep.SystemType == typeof(EndSimulationEntityCommandBufferSystem))
                        {
                            WarningForBeforeCheck(systemType, dep.SystemType);
                            continue;
                        }
                    }

                    if (parentType == typeof(PresentationSystemGroup))
                    {
                        if (dep.SystemType == typeof(BeginPresentationEntityCommandBufferSystem))
                        {
#if !NET_DOTS
                            throw new ArgumentException(
                                $"Invalid [UpdateBefore] {dep.SystemType} attribute on {systemType}, because that system is already restricted to be first.");
#else
                            throw new ArgumentException($"Invalid [UpdateBefore] BeginPreesntationEntityCommandBufferSystem, because that system is already restricted to be first.");
#endif
                        }
#pragma warning disable 0618
                        // warning CS0618: 'EndPresentationEntityCommandBufferSystem' is obsolete
                        if (dep.SystemType == typeof(EndPresentationEntityCommandBufferSystem))
                        {
                            WarningForBeforeCheck(systemType, dep.SystemType);
                            continue;
                        }
#pragma warning restore 0618
                    }

                    int depIndex = LookupSysAndDep(dep.SystemType, sysAndDep);
                    if (depIndex < 0)
                    {
#if !NET_DOTS
                        Debug.LogWarning(
                            $"Ignoring invalid [UpdateBefore] attribute on {systemType} because {dep.SystemType} belongs to a different {nameof(ComponentSystemGroup)}.\n"
                            + $"This attribute can only order systems that are children of the same {nameof(ComponentSystemGroup)}.\n"
                            + $"Make sure that both systems are in the same parent group with [UpdateInGroup(typeof({parentType})].\n"
                            + $"You can also change the relative order of groups when appropriate, by using [UpdateBefore] and [UpdateAfter] attributes at the group level.");
#else
                        Debug.LogWarning("WARNING: invalid [UpdateBefore] dependency:");
                        Debug.LogWarning(TypeManager.SystemName(systemType));
                        Debug.LogWarning("  depends on a non-sibling system: ");
                        Debug.LogWarning(TypeManager.SystemName(dep.SystemType));
#endif
                        continue;
                    }

                    sysAndDep[i].updateBefore.Add(dep.SystemType);
                    sysAndDep[depIndex].nAfter++;
                }

                foreach (var attr in after)
                {
                    var dep = attr as UpdateAfterAttribute;
                    if (!typeof(ComponentSystemBase).IsAssignableFrom(dep.SystemType))
                    {
#if !NET_DOTS
                        Debug.LogWarning(
                            $"Ignoring invalid [UpdateAfter] attribute on {systemType} because {dep.SystemType} is not a subclass of {nameof(ComponentSystemBase)}.\n"
                            + $"Set the target parameter of [UpdateAfter] to a system class in the same {nameof(ComponentSystemGroup)} as {systemType}.");
#else
                        Debug.LogWarning($"WARNING: invalid [UpdateAfter] attribute:");
                        Debug.LogWarning(TypeManager.SystemName(dep.SystemType));
                        Debug.LogWarning(" is not derived from ComponentSystemBase. Set the target parameter of [UpdateAfter] to a system class in the same ComponentSystemGroup.");
#endif
                        continue;
                    }

                    if (dep.SystemType == systemType)
                    {
#if !NET_DOTS
                        Debug.LogWarning(
                            $"Ignoring invalid [UpdateAfter] attribute on {systemType} because a system cannot be updated after itself.\n"
                            + $"Set the target parameter of [UpdateAfter] to a different system class in the same {nameof(ComponentSystemGroup)} as {systemType}.");
#else
                        Debug.LogWarning($"WARNING: invalid [UpdateAfter] attribute:");
                        Debug.LogWarning(TypeManager.SystemName(systemType));
                        Debug.LogWarning("  depends on itself. Set the target parameter of [UpdateAfter] to a system class in the same ComponentSystemGroup.");
#endif
                        continue;
                    }

                    if (parentType == typeof(InitializationSystemGroup))
                    {
                        if (dep.SystemType == typeof(BeginInitializationEntityCommandBufferSystem))
                        {
                            WarningForAfterCheck(systemType, dep.SystemType);
                            continue;
                        }

                        if (dep.SystemType == typeof(EndInitializationEntityCommandBufferSystem))
                        {
#if !NET_DOTS
                            throw new ArgumentException(
                                $"Invalid [UpdateAfter] {dep.SystemType} attribute on {systemType}, because that system is already restricted to be last.");
#else
                            throw new ArgumentException($"Invalid [UpdateAfter] EndInitializationEntityCommandBufferSystem, because that system is already restricted to be last.");
#endif
                        }
                    }

                    if (parentType == typeof(SimulationSystemGroup))
                    {
                        if (dep.SystemType == typeof(BeginSimulationEntityCommandBufferSystem))
                        {
                            WarningForAfterCheck(systemType, dep.SystemType);
                            continue;
                        }

                        if (dep.SystemType == typeof(LateSimulationSystemGroup))
                        {
#if !NET_DOTS
                            throw new ArgumentException(
                                $"Invalid [UpdateAfter] {dep.SystemType} attribute on {systemType}, because that system is already restricted to be last.");
#else
                            throw new ArgumentException($"Invalid [UpdateAfter] EndLateSimulationEntityCommandBufferSystem, because that system is already restricted to be last.");
#endif
                        }

                        if (dep.SystemType == typeof(EndSimulationEntityCommandBufferSystem))
                        {
#if !NET_DOTS
                            throw new ArgumentException(
                                $"Invalid [UpdateAfter] {dep.SystemType} attribute on {systemType}, because that system is already restricted to be last.");
#else
                            throw new ArgumentException($"Invalid [UpdateAfter] EndSimulationEntityCommandBufferSystem, because that system is already restricted to be last.");
#endif
                        }
                    }

                    if (parentType == typeof(PresentationSystemGroup))
                    {
                        if (dep.SystemType == typeof(BeginPresentationEntityCommandBufferSystem))
                        {
                            WarningForAfterCheck(systemType, dep.SystemType);
                            continue;
                        }
#pragma warning disable 0618
                        // warning CS0618: 'EndPresentationEntityCommandBufferSystem' is obsolete
                        if (dep.SystemType == typeof(EndPresentationEntityCommandBufferSystem))
                        {
#if !NET_DOTS
                            throw new ArgumentException(
                                $"Invalid [UpdateAfter] {dep.SystemType} attribute on {systemType}, because that system is already restricted to be last.");
#else
                            throw new ArgumentException($"Invalid [UpdateAfter] EndPresentationEntityCommandBufferSystem, because that system is already restricted to be last.");
#endif
                        }
#pragma warning restore 0618
                    }

                    int depIndex = LookupSysAndDep(dep.SystemType, sysAndDep);
                    if (depIndex < 0)
                    {
#if !NET_DOTS
                        Debug.LogWarning(
                            $"Ignoring invalid [UpdateAfter] attribute on {systemType} because {dep.SystemType} belongs to a different {nameof(ComponentSystemGroup)}.\n"
                            + $"This attribute can only order systems that are children of the same {nameof(ComponentSystemGroup)}.\n"
                            + $"Make sure that both systems are in the same parent group with [UpdateInGroup(typeof({parentType})].\n"
                            + $"You can also change the relative order of groups when appropriate, by using [UpdateBefore] and [UpdateAfter] attributes at the group level.");
#else
                        Debug.LogWarning("WARNING: invalid [UpdateAfter] dependency:");
                        Debug.LogWarning(TypeManager.SystemName(systemType));
                        Debug.LogWarning("  depends on a non-sibling system: ");
                        Debug.LogWarning(TypeManager.SystemName(dep.SystemType));
#endif
                        continue;
                    }

                    sysAndDep[depIndex].updateBefore.Add(systemType);
                    sysAndDep[i].nAfter++;
                }
            }

            // Clear the systems list and rebuild it in sorted order from the lookup table
            var readySystems = new Heap<TypeHeapElement>(items.Count);
            items.Clear();
            for (int i = 0; i < sysAndDep.Length; ++i)
            {
                if (sysAndDep[i].nAfter == 0)
                {
                    readySystems.Insert(new TypeHeapElement(i, sysAndDep[i].type));
                }
            }

            while (!readySystems.Empty)
            {
                var sysIndex = readySystems.Extract().unsortedIndex;
                var sd = sysAndDep[sysIndex];

                sysAndDep[sysIndex] = new SysAndDep<T>(); // "Remove()"
                items.Add(sd.item);
                foreach (var beforeType in sd.updateBefore)
                {
                    int beforeIndex = LookupSysAndDep(beforeType, sysAndDep);
                    if (beforeIndex < 0) throw new Exception("Bug in SortSystemUpdateList(), beforeIndex < 0");
                    if (sysAndDep[beforeIndex].nAfter <= 0)
                        throw new Exception("Bug in SortSystemUpdateList(), nAfter <= 0");

                    sysAndDep[beforeIndex].nAfter--;
                    if (sysAndDep[beforeIndex].nAfter == 0)
                    {
                        readySystems.Insert(new TypeHeapElement(beforeIndex, sysAndDep[beforeIndex].type));
                    }
                }
            }

            for (int i = 0; i < sysAndDep.Length; ++i)
            {
                if (sysAndDep[i].item != null)
                {
                    // Since no System in the circular dependency would have ever been added
                    // to the heap, we should have values for everything in sysAndDep. Check,
                    // just in case.
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    var visitedSystems = new List<Type>();
                    var startIndex = i;
                    var currentIndex = i;
                    while (true)
                    {
                        if (sysAndDep[currentIndex].item != null)
                            visitedSystems.Add(sysAndDep[currentIndex].type);

                        currentIndex = LookupSysAndDep(sysAndDep[currentIndex].updateBefore[0], sysAndDep);
                        if (currentIndex < 0 || currentIndex == startIndex || sysAndDep[currentIndex].item == null)
                        {
                            throw new CircularSystemDependencyException(visitedSystems);
                        }
                    }
#else
                    sysAndDep[i] = new SysAndDep<T>();
#endif
                }
            }
        }
    }
}
