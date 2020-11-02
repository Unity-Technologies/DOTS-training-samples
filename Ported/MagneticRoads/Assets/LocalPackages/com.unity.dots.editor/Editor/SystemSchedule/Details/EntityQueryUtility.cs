using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Entities.Editor
{
    static class EntityQueryUtility
    {
        internal static int CompareTypes(ComponentType x, ComponentType y)
        {
            var accessModeOrder = SortOrderFromAccessMode(x.AccessModeType).CompareTo(SortOrderFromAccessMode(y.AccessModeType));
            return accessModeOrder != 0 ? accessModeOrder : string.Compare(x.GetManagedType().Name, y.GetManagedType().Name, StringComparison.InvariantCulture);
        }

        static int SortOrderFromAccessMode(ComponentType.AccessMode mode)
        {
            switch (mode)
            {
                case ComponentType.AccessMode.ReadOnly:
                    return 0;
                case ComponentType.AccessMode.ReadWrite:
                    return 1;
                case ComponentType.AccessMode.Exclude:
                    return 2;
                default:
                    throw new ArgumentException("Unrecognized AccessMode");
            }
        }

        public static string SpecifiedTypeName(Type type)
        {
            using (var pooled = type.GetGenericArguments().ToPooledQueue())
            {
                return GetResolvedTypeName(type, pooled.Queue);
            }
        }

        static string GetResolvedTypeName(Type type, Queue<Type> args)
        {
            var name = type.Name;

            if (type.IsGenericParameter)
                return name;

            if (type.IsNested)
                name = $"{GetResolvedTypeName(type.DeclaringType, args)}.{name}";

            if (type.IsGenericType)
            {
                var tickIndex = name.IndexOf('`');
                if (tickIndex > -1)
                    name = name.Remove(tickIndex);
                var genericTypes = type.GetGenericArguments();

                var genericTypeNames = new StringBuilder();
                for (var i = 0; i < genericTypes.Length && args.Count > 0; i++)
                {
                    if (i != 0)
                        genericTypeNames.Append(", ");
                    genericTypeNames.Append(SpecifiedTypeName(args.Dequeue()));
                }

                if (genericTypeNames.Length > 0)
                    name = $"{name}<{genericTypeNames}>";
            }

            return name;
        }

        public static string StyleForAccessMode(ComponentType.AccessMode mode)
        {
            switch (mode)
            {
                case ComponentType.AccessMode.ReadOnly:
                    return UssClasses.SystemScheduleWindow.Detail.ReadOnlyIcon;
                case ComponentType.AccessMode.ReadWrite:
                    return UssClasses.SystemScheduleWindow.Detail.ReadWriteIcon;
                case ComponentType.AccessMode.Exclude:
                    return UssClasses.SystemScheduleWindow.Detail.ExcludeIcon;

                default:
                    throw new ArgumentException("Unrecognized AccessMode");
            }
        }

        public static unsafe bool ContainsThisComponentType(SelectedSystem selectedSystem, string componentTypeName)
        {
            if (selectedSystem == null || !selectedSystem.Valid) return false;

            var ptr = selectedSystem.StatePointer;
            if (ptr != null && ptr->EntityQueries.length > 0)
            {
                var queries = ptr->EntityQueries;
                for (var i = 0; i < queries.length; i++)
                {
                    using (var queryTypeList = queries[i].GetQueryTypes().ToPooledList())
                    {
                        foreach (var queryType in queryTypeList.List)
                        {
                            var name = SpecifiedTypeName(queryType.GetManagedType());
                            if (name.IndexOf(componentTypeName, StringComparison.OrdinalIgnoreCase) >= 0)
                                return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
