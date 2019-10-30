using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.Properties.Editor
{
    static class CustomInspectorDatabase
    {
        public class UxmlPostProcessor : AssetPostprocessor
        {
            private const string k_UxmlExtension = ".uxml";

            static void OnPostprocessAllAssets(
                string[] importedAssets,
                string[] deletedAssets,
                string[] movedAssets,
                string[] movedFromAssetPaths)
            {
                if (Process(importedAssets)) return;
                if (Process(deletedAssets)) return;
                if (Process(movedAssets)) return;
            }

            static bool Process(string[] paths)
            {
                if (!paths.Any(path => path.EndsWith(k_UxmlExtension, StringComparison.InvariantCultureIgnoreCase)))
                    return false;
                // TODO: Signal property elements to potentially refresh themselves. 
                return true;
            }
        }

        static readonly Dictionary<Type, List<Type>> s_InspectorsPerType;

        static CustomInspectorDatabase()
        {
            s_InspectorsPerType = new Dictionary<Type, List<Type>>();
            RegisterCustomInspectors();
        }
        
        public static IInspector<TValue> GetInspector<TValue>(IProperty property)
        {
            var inspector = GetPropertyDrawer<TValue>(property);
            return inspector ?? GetInspectorInstance<TValue>(s_InspectorsPerType, new NoPropertyDrawers());
        }
        
        public static IInspector<TValue> GetPropertyDrawer<TValue>(IProperty property)
        {
            foreach(var drawerAttribute in property.Attributes?.GetAttributes<UnityEngine.PropertyAttribute>() ?? Array.Empty<UnityEngine.PropertyAttribute>())
            {
                var drawer = GetInspectorInstance<TValue>(s_InspectorsPerType, new WithPropertyDrawers(drawerAttribute.GetType()));
                if (null != drawer)
                {
                    return drawer;
                }
            }

            return null;
        }
        
        static IInspector<TValue> GetInspectorInstance<TValue>(Dictionary<Type, List<Type>> typeMap, IInspectorResolver resolver)
        {
            var type = typeof(TValue);
            if (!typeMap.TryGetValue(type, out var inspectors))
            {
                return null;
            }
            
            foreach (var inspector in inspectors)
            {
                if (resolver.Resolve(inspector))
                {
                    return (IInspector<TValue>) Activator.CreateInstance(inspector);
                }
            }
            return null;
        }

        static void RegisterCustomInspectors()
        {
            foreach (var type in TypeCache.GetTypesDerivedFrom(typeof(IInspector<>)))
            {
                RegisterInspectorType(s_InspectorsPerType, typeof(IInspector<>), type);
            }
        }

        static void RegisterInspectorType(Dictionary<Type, List<Type>> typeMap, Type interfaceType, Type inspectorType)
        {
            var inspectorInterface = inspectorType.GetInterface(interfaceType.FullName);
            if (null == inspectorInterface || inspectorType.IsAbstract || inspectorType.ContainsGenericParameters)
            {
                return;
            }

            var genericArguments = inspectorInterface.GetGenericArguments();
            var componentType = genericArguments[0];

            if (!TypeConstruction.HasParameterLessConstructor(inspectorType))
            {
                Debug.LogError($"Could not create a custom inspector for type `{inspectorType.Name}`: no default or empty constructor found.");
            }

            if (!typeMap.TryGetValue(componentType, out var list))
            {
                typeMap[componentType] = list = new List<Type>();
            }

            list.Add(inspectorType);
        }
    }
}
