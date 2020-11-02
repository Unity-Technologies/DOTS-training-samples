using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Entities.Editor
{
    static class SystemScheduleSearchBuilder
    {
        public struct ParseResult : IEquatable<ParseResult>
        {
            public bool IsEmpty => string.IsNullOrWhiteSpace(Input);
            public string Input;
            public IEnumerable<string> ComponentNames;
            public IEnumerable<string> DependencySystemNames;
            public IEnumerable<Type> DependencySystemTypes;
            public IEnumerable<string> SystemNames;
            public string ErrorComponentType;
            public string ErrorSdSystemName;

            public bool Equals(ParseResult other)
            {
                return Input == other.Input;
            }

            public static readonly ParseResult EmptyResult = new ParseResult
            {
                Input = string.Empty,
                ComponentNames = Array.Empty<string>(),
                DependencySystemNames = Array.Empty<string>(),
                DependencySystemTypes = Array.Empty<Type>(),
                SystemNames = Array.Empty<string>(),
                ErrorComponentType = string.Empty,
                ErrorSdSystemName = string.Empty
            };
        }

        static string CheckComponentTypeExistence(IEnumerable<string> componentTypes)
        {
            foreach (var componentType in componentTypes)
            {
                if (!ComponentTypeCache.GetFuzzyMatchingTypes(componentType).Any())
                    return componentType;
            }

            return string.Empty;
        }

        public static ParseResult ParseSearchString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return ParseResult.EmptyResult;

            var componentNameList = new Lazy<List<string>>();
            var dependencySystemNameList = new Lazy<List<string>>();
            var dependencySystemTypeList = new Lazy<List<Type>>();
            var systemNameList = new Lazy<List<string>>();
            var errorSdSystemName = string.Empty;

            // TODO: Once we integrate "SearchElement", this "SplitSearchStringBySpace" will be removed.
            foreach (var singleString in SearchUtility.SplitSearchStringBySpace(input))
            {
                var findSdSystem = false;

                if (singleString.StartsWith(Constants.SystemSchedule.k_ComponentToken, StringComparison.OrdinalIgnoreCase))
                {
                    componentNameList.Value.Add(singleString.Substring(Constants.SystemSchedule.k_ComponentTokenLength));
                }
                else if (singleString.StartsWith(Constants.SystemSchedule.k_SystemDependencyToken, StringComparison.OrdinalIgnoreCase))
                {
                    var singleSystemName = singleString.Substring(Constants.SystemSchedule.k_SystemDependencyTokenLength);
                    dependencySystemNameList.Value.Add(singleSystemName);

                    foreach (var system in PlayerLoopSystemGraph.Current.AllSystems)
                    {
                        var type = system.GetType();
                        if (!string.Equals(type.Name, singleSystemName, StringComparison.OrdinalIgnoreCase))
                            continue;

                        dependencySystemTypeList.Value.Add(type);
                        findSdSystem = true;
                    }

                    if (!findSdSystem)
                    {
                        errorSdSystemName = singleSystemName;
                        break;
                    }
                }
                else
                {
                    systemNameList.Value.Add(singleString);
                }
            }

            var errorComponentType = componentNameList.Value.Any() ? CheckComponentTypeExistence(componentNameList.Value) : string.Empty;

            return new ParseResult
            {
                Input = input,
                ComponentNames = componentNameList.Value,
                DependencySystemNames = dependencySystemNameList.Value,
                DependencySystemTypes = dependencySystemTypeList.Value,
                SystemNames = systemNameList.Value,
                ErrorComponentType = errorComponentType,
                ErrorSdSystemName = errorSdSystemName
            };
        }
    }
}
