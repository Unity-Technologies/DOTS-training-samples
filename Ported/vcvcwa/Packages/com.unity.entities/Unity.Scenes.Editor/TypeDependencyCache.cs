using System;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using AssetImportContext = UnityEditor.Experimental.AssetImporters.AssetImportContext;
using System.Reflection;
using Unity.Profiling;

namespace Unity.Scenes.Editor
{
    [InitializeOnLoad]
    class TypeDependencyCache
    {
        const string SystemsVersion = "DOTSAllSystemsVersion";

        struct NameAndVersion : IComparable<NameAndVersion>
        {
            public string FullName;
            public int    Version;
            public int CompareTo(NameAndVersion other)
            {
                return FullName.CompareTo(other.FullName);
            }
        }

        static ProfilerMarker kRegisterComponentTypes = new ProfilerMarker("TypeDependencyCache.RegisterComponentTypes");
        static ProfilerMarker kRegisterConversionSystemVersions = new ProfilerMarker("TypeDependencyCache.RegisterConversionSystems");

        static TypeDependencyCache()
        {
            // Custom dependencies are transmitted to the import worker so dont spent time on registering them
            if (UnityEditor.Experimental.AssetDatabaseExperimental.IsAssetImportWorkerProcess())
                return;

            using(kRegisterComponentTypes.Auto())
                RegisterComponentTypes();
        
            using(kRegisterConversionSystemVersions.Auto())
                RegisterConversionSystems();
        }
    
        static void RegisterComponentTypes()
        {
            TypeManager.Initialize();

            UnityEditor.Experimental.AssetDatabaseExperimental.UnregisterCustomDependencyPrefixFilter("DOTSType/");
            int typeCount = TypeManager.GetTypeCount();

            for (int i = 1; i < typeCount; ++i)
            {
                var typeInfo = TypeManager.GetTypeInfo(i);
                var hash = typeInfo.StableTypeHash;
                UnityEditor.Experimental.AssetDatabaseExperimental.RegisterCustomDependency(TypeString(typeInfo.Type),
                    new UnityEngine.Hash128(hash, hash));
            }
        }

        static unsafe void RegisterConversionSystems()
        {
            var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.GameObjectConversion |
                                                         WorldSystemFilterFlags.EntitySceneOptimizations);
            var nameAndVersion = new NameAndVersion[systems.Count];

            for (int i = 0; i != nameAndVersion.Length; i++)
            {
                nameAndVersion[i].FullName = systems[i].FullName;

                var systemVersionAttribute = systems[i].GetCustomAttribute<ConverterVersionAttribute>();
                if (systemVersionAttribute != null)
                    nameAndVersion[i].Version = systemVersionAttribute.Version;
            }
        
            Array.Sort(nameAndVersion);

            UnityEngine.Hash128 hash = default;
            for (int i = 0; i != nameAndVersion.Length; i++)
            {
                var fullName = nameAndVersion[i].FullName;
                fixed (char* str = fullName)
                {
                    HashUnsafeUtilities.ComputeHash128(str, (ulong)(sizeof(char) * fullName.Length), &hash);
                }

                int version = nameAndVersion[i].Version;
                HashUnsafeUtilities.ComputeHash128(&version, sizeof(int), &hash);
            }

            UnityEditor.Experimental.AssetDatabaseExperimental.RegisterCustomDependency(SystemsVersion, hash);
        }

        public static void AddDependency(AssetImportContext ctx, ComponentType type)
        {
            var typeString = TypeString(type.GetManagedType());
            ctx.DependsOnCustomDependency(typeString);
        }

        public static void AddAllSystemsDependency(AssetImportContext ctx)
        {
            ctx.DependsOnCustomDependency(SystemsVersion);
        }
    
        static string TypeString(Type type) => $"DOTSType/{type.FullName}";
    }
}