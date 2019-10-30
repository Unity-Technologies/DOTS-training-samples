using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEditor.Compilation;
using Debug = UnityEngine.Debug;

namespace Unity.Burst.Editor
{
    using static BurstCompilerOptions;

    internal static class BurstReflection
    {
        public static List<BurstCompileTarget> FindExecuteMethods(AssembliesType assemblyTypes)
        {
            var methodsToCompile = new List<BurstCompileTarget>();
            var methodsToCompileSet = new HashSet<MethodInfo>();

            var valueTypes = new List<TypeToVisit>();
            var interfaceToProducer = new Dictionary<Type, Type>();

            var assemblyList = GetAssemblyList(assemblyTypes);
            //Debug.Log("Filtered Assembly List: " + string.Join(", ", assemblyList.Select(assembly => assembly.GetName().Name)));

            // Find all ways to execute job types (via producer attributes)
            var typesVisited = new HashSet<string>();
            var typesToVisit = new HashSet<string>();
            foreach (var assembly in assemblyList)
            {
                var types = new List<Type>();
                try
                {
                    types.AddRange(assembly.GetTypes());
                    // Collect all generic type instances (excluding indirect instances)
                    CollectGenericTypeInstances(assembly, types);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Unexpected exception while collecting types in assembly `" + assembly.FullName + "` Exception: " + ex);
                }

                for (var i = 0; i < types.Count; i++)
                {
                    var t = types[i];
                    if (typesToVisit.Add(t.FullName))
                    {
                        // Because the list of types returned by CollectGenericTypeInstances does not detect nested generic classes that are not
                        // used explicitly, we need to create them if a declaring type is actually used
                        // so for example if we have:
                        // class MyClass<T> { class MyNestedClass { } }
                        // class MyDerived : MyClass<int> { }
                        // The CollectGenericTypeInstances will return typically the type MyClass<int>, but will not list MyClass<int>.MyNestedClass
                        // So the following code is correcting this in order to fully query the full graph of generic instance types, including indirect types
                        var nestedTypes = t.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var nestedType in nestedTypes)
                        {
                            if (t.IsGenericType && !t.IsGenericTypeDefinition)
                            {
                                var parentGenericTypeArguments = t.GetGenericArguments();
                                // Only create nested types that are closed generic types (full generic instance types)
                                // It happens if for example the parent class is `class MClass<T> { class MyNestedGeneric<T1> {} }`
                                // In that case, MyNestedGeneric<T1> is closed in the context of MClass<int>, so we don't process them
                                if (nestedType.GetGenericArguments().Length == parentGenericTypeArguments.Length)
                                {
                                    var instanceNestedType = nestedType.MakeGenericType(parentGenericTypeArguments);
                                    types.Add(instanceNestedType);
                                }
                            }
                            else
                            {
                                types.Add(nestedType);
                            }

                        }
                    }
                }

                foreach (var t in types)
                {
                    // If the type has been already visited, don't try to visit it
                    if (!typesVisited.Add(t.FullName) || (t.IsGenericTypeDefinition && !t.IsInterface))
                    {
                        continue;
                    }

                    try
                    {
                        // collect methods with types having a [BurstCompile] attribute
                        bool visitStaticMethods = HasBurstCompileAttribute(t);
                        bool isValueType = false;
                        
                        if (t.IsInterface)
                        {
                            object[] attrs = t.GetCustomAttributes(typeof(JobProducerTypeAttribute), false);
                            if (attrs.Length == 0)
                                continue;

                            JobProducerTypeAttribute attr = (JobProducerTypeAttribute)attrs[0];

                            interfaceToProducer.Add(t, attr.ProducerType);

                            //Debug.Log($"{t} has producer {attr.ProducerType}");
                        }
                        else if (t.IsValueType)
                        {
                            // NOTE: Make sure that we don't use a value type generic definition (e.g `class Outer<T> { struct Inner { } }`)
                            // We are only working on plain type or generic type instance!
                            if (!t.IsGenericTypeDefinition)
                                isValueType = true;
                        }

                        if (isValueType || visitStaticMethods)
                        {
                            valueTypes.Add(new TypeToVisit(t, visitStaticMethods));
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("Unexpected exception while inspecting type `" + t +
                                  "` IsConstructedGenericType: " + t.IsConstructedGenericType +
                                  " IsGenericTypeDef: " + t.IsGenericTypeDefinition +
                                  " IsGenericParam: " + t.IsGenericParameter +
                                  " Exception: " + ex);
                    }
                }
            }

            //Debug.Log($"Mapped {interfaceToProducer.Count} producers; {valueTypes.Count} value types");

            // Revisit all types to find things that are compilable using the above producers.
            foreach (var typePair in valueTypes)
            {
                var type = typePair.Type;

                // collect static [BurstCompile] methods
                if (typePair.CollectStaticMethods)
                {
                    try
                    {
                        var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var method in methods)
                        {
                            if (HasBurstCompileAttribute(method))
                            {
                                var target = new BurstCompileTarget(method, type, null, true);
                                methodsToCompile.Add(target);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }

                // If the type is not a value type, we don't need to proceed with struct Jobs
                if (!type.IsValueType)
                {
                    continue;
                }

                // Otherwise try to find if we have an interface producer setup on the class
                foreach (var interfaceType in type.GetInterfaces())
                {
                    var genericLessInterface = interfaceType;
                    if (interfaceType.IsGenericType)
                    {
                        genericLessInterface = interfaceType.GetGenericTypeDefinition();
                    }

                    Type foundProducer;
                    if (interfaceToProducer.TryGetValue(genericLessInterface, out foundProducer))
                    {
                        var genericParams = new List<Type> {type};
                        if (interfaceType.IsGenericType)
                        {
                            genericParams.AddRange(interfaceType.GenericTypeArguments);
                        }

                        try
                        {
                            var executeType = foundProducer.MakeGenericType(genericParams.ToArray());
                            var executeMethod = executeType.GetMethod("Execute");
                            if (executeMethod == null)
                            {
                                throw new InvalidOperationException($"Burst reflection error. The type `{executeType}` does not contain an `Execute` method");
                            }

                            // We will not try to record more than once a method in the methods to compile
                            // This can happen if a job interface is inheriting from another job interface which are using in the end the same
                            // job producer type
                            if (methodsToCompileSet.Add(executeMethod))
                            {
                                var target = new BurstCompileTarget(executeMethod, type, interfaceType, false);
                                methodsToCompile.Add(target);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogException(ex);
                        }
                    }
                }
            }

            return methodsToCompile;
        }


        /// <summary>
        /// Collects all assemblies - transitively that are valid for the specified type `Player` or `Editor`
        /// </summary>
        /// <param name="assemblyTypes">The assembly type</param>
        /// <returns>The list of assemblies valid for this platform</returns>
        private static List<System.Reflection.Assembly> GetAssemblyList(AssembliesType assemblyTypes)
        {
            // TODO: Not sure there is a better way to match assemblies returned by CompilationPipeline.GetAssemblies
            // with runtime assemblies contained in the AppDomain.CurrentDomain.GetAssemblies()

            // Filter the assemblies
            var assemblyList = CompilationPipeline.GetAssemblies(assemblyTypes);

            var assemblyNames = new HashSet<string>();
            foreach (var assembly in assemblyList)
            {
                CollectAssemblyNames(assembly, assemblyNames);
            }

            var allAssemblies = new HashSet<System.Reflection.Assembly>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assemblyNames.Contains(assembly.GetName().Name))
                {
                    continue;
                }
                CollectAssembly(assembly, allAssemblies);
            }

            return allAssemblies.ToList();
        }

        private static void CollectAssembly(System.Reflection.Assembly assembly, HashSet<System.Reflection.Assembly> collect)
        {
            if (!collect.Add(assembly))
            {
                return;
            }

            foreach (var assemblyName in assembly.GetReferencedAssemblies())
            {
                try
                {
                    CollectAssembly(System.Reflection.Assembly.Load(assemblyName), collect);
                }
                catch (Exception)
                {
                    Debug.LogWarning("Could not load assembly " + assemblyName);
                }
            }
        }

        private static void CollectAssemblyNames(UnityEditor.Compilation.Assembly assembly, HashSet<string> collect)
        {
            if (assembly == null || assembly.name == null) return;

            if (!collect.Add(assembly.name))
            {
                return;
            }

            foreach (var assemblyRef in assembly.assemblyReferences)
            {
                CollectAssemblyNames(assemblyRef, collect);
            }
        }

        /// <summary>
        /// Gets the list of concrete generic type instances used in an assembly.
        /// See remarks
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <param name="types"></param>
        /// <returns>The list of generic type instances</returns>
        /// <remarks>
        /// Note that this method fetchs only direct type instanecs but
        /// cannot fetch transitive generic type instances.
        /// </remarks>
        private static void CollectGenericTypeInstances(System.Reflection.Assembly assembly, List<Type> types)
        {
            // From: https://gist.github.com/xoofx/710aaf86e0e8c81649d1261b1ef9590e
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            // Token base id for TypeSpec
            const int mdTypeSpec = 0x1B000000;
            const int mdTypeSpecCount = 1 << 24;
            foreach (var module in assembly.Modules)
            {
                for (int i = 1; i < mdTypeSpecCount; i++)
                {
                    try
                    {
                        var type = module.ResolveType(mdTypeSpec | i);
                        if (type.IsConstructedGenericType && !type.ContainsGenericParameters)
                        {
                            types.Add(type);
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        break;
                    }
                    catch (ArgumentException)
                    {
                        // Can happen on ResolveType on certain generic types, so we continue
                    }
                }
            }
        }

        [DebuggerDisplay("{Type} (static methods: {CollectStaticMethods})")]
        private struct TypeToVisit
        {
            public TypeToVisit(Type type, bool collectStaticMethods)
            {
                Type = type;
                CollectStaticMethods = collectStaticMethods;
            }

            public readonly Type Type;

            public readonly bool CollectStaticMethods;
        }
    }
}
