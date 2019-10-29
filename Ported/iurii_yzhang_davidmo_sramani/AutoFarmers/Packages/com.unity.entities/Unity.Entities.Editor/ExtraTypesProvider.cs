using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Entities;
using UnityEditor;
using UnityEditor.Build.Player;
using Unity.Jobs.LowLevel.Unsafe;
    
namespace Unity.Entities.Editor
{
    [InitializeOnLoad]
    public sealed class ExtraTypesProvider
    {
        static void AddIJobForEach(Type type, HashSet<string> extraTypes)
        {
            foreach (var typeInterface in type.GetInterfaces())
            {
                if (typeInterface.Name.StartsWith("IJobForEach"))
                {
                    var genericArgumentList = new List<Type> { type };
                    genericArgumentList.AddRange(typeInterface.GetGenericArguments());

                    var producerAttribute = (JobProducerTypeAttribute) typeInterface.GetCustomAttribute(typeof(JobProducerTypeAttribute), true);

                    if (producerAttribute == null)
                        throw new System.ArgumentException("IJobForEach interface must have [JobProducerType]");

                    var generatedType = producerAttribute.ProducerType.MakeGenericType(genericArgumentList.ToArray());
                    extraTypes.Add(generatedType.ToString());

                    return;
                }
            }
        }

        static ExtraTypesProvider()
        {
            //@TODO: Only produce JobForEachExtensions.JobStruct_Process1
            //       if there is any use of that specific type in deployed code.

            PlayerBuildInterface.ExtraTypesProvider += () =>
            {
                var extraTypes = new HashSet<string>();
                var visitedTypes = new HashSet<Type>();

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!TypeManager.IsAssemblyReferencingEntities(assembly))
                        continue;

                    foreach (var type in assembly.GetTypes())
                    {
                        AddIJobForEachForTypeHierarchy(extraTypes, visitedTypes, type);
                    }
                }

                TypeManager.Initialize();

                foreach (var typeInfo in TypeManager.AllTypes)
                {
                    Type type = TypeManager.GetType(typeInfo.TypeIndex);
                    if (type != null)
                    {
                        FastEquality.AddExtraAOTTypes(type, extraTypes);
                    }
                }

                return extraTypes;
            };
        }

        private static void AddIJobForEachForTypeHierarchy(HashSet<string> extraTypes, HashSet<Type> visitedTypes, Type type)
        {
            while (type != null)
            {
                if (type.IsGenericTypeDefinition)
                    return;

                if (visitedTypes.Contains(type))
                    return;

                visitedTypes.Add(type);

                if (typeof(JobForEachExtensions.IBaseJobForEach).IsAssignableFrom(type) && !type.IsAbstract)
                    AddIJobForEach(type, extraTypes);

                foreach (var nestedType in type.GetNestedTypes(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    AddIJobForEachForTypeHierarchy(extraTypes, visitedTypes, ResolveGenericParameters(nestedType, type));

                type = type.BaseType;
            }
        }

        private static Type ResolveGenericParameters(Type nestedType, Type type)
        {
            var genericParameters = nestedType.GetGenericArguments();
            if (genericParameters.Length == 0)
            { 
                // Nested type isn't generic
                return nestedType;
            }

            var parentTypeArgs = type.GetGenericArguments();
            if (parentTypeArgs.Length < genericParameters.Length)
            {
                // Nested type has its own generic parameters so we can't fully resolve the type
                return nestedType;
            }

            return nestedType.MakeGenericType(parentTypeArgs);
        }
    }
}