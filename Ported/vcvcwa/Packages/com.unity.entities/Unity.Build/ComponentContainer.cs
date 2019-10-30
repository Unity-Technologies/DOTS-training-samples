using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.Properties;
using Unity.Serialization.Json;
using UnityEditor;
using UnityEngine;
using Property = Unity.Properties.PropertyAttribute;

namespace Unity.Build
{
    /// <summary>
    /// Base class that stores a set of unique component.
    /// Other <see cref="ComponentContainer{TObject}"/> can be added as dependencies to get inherited or overridden items.
    /// </summary>
    /// <typeparam name="TComponent">Components base type.</typeparam>
    public class ComponentContainer<TComponent> : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] string m_AssetAsJson;
        [Property] internal readonly List<ComponentContainer<TComponent>> Dependencies = new List<ComponentContainer<TComponent>>();
        [Property] internal readonly List<TComponent> Components = new List<TComponent>();
        internal static event Action<ComponentContainer<TComponent>> AssetChanged;

        /// <summary>
        /// Event invoked when <see cref="ComponentContainer{TObject}"/> registers <see cref="JsonVisitor"/> used for serialization.
        /// It provides an opportunity to register additional property visitor adapters.
        /// </summary>
        public static event Action<JsonVisitor> JsonVisitorRegistration = delegate { };

        /// <summary>
        /// Determine if a <see cref="Type"/> component is stored in this <see cref="ComponentContainer{TObject}"/> or its dependencies.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the component.</param>
        public bool HasComponent(Type type) => HasComponentOnSelf(type) || HasComponentOnDependency(type);

        /// <summary>
        /// Determine if a <typeparamref name="T"/> component is stored in this <see cref="ComponentContainer{TObject}"/> or its dependencies.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        public bool HasComponent<T>() where T : TComponent => HasComponent(typeof(T));

        /// <summary>
        /// Determine if a <see cref="Type"/> component is inherited from a dependency.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the component.</param>
        public bool IsComponentInherited(Type type) => !HasComponentOnSelf(type) && HasComponentOnDependency(type);

        /// <summary>
        /// Determine if a <typeparamref name="T"/> component is inherited from a dependency.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        public bool IsComponentInherited<T>() where T : TComponent => IsComponentInherited(typeof(T));

        /// <summary>
        /// Determine if a <see cref="Type"/> component overrides a dependency.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the component.</param>
        public bool IsComponentOverridden(Type type) => HasComponentOnSelf(type) && HasComponentOnDependency(type);

        /// <summary>
        /// Determine if a <typeparamref name="T"/> component overrides a dependency.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        public bool IsComponentOverridden<T>() where T : TComponent => IsComponentOverridden(typeof(T));

        /// <summary>
        /// Get the value of a <see cref="Type"/> component.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the component.</param>
        public TComponent GetComponent(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException(nameof(type));
            }

            if (type == typeof(object))
            {
                throw new InvalidOperationException($"{nameof(type)} cannot be '{typeof(object).FullName}'.");
            }

            if (!TryGetComponent(type, out var value))
            {
                throw new InvalidOperationException($"Component type '{type.FullName}' not found.");
            }

            return value;
        }

        /// <summary>
        /// Get the value of a <typeparamref name="T"/> component.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        public T GetComponent<T>() where T : TComponent => (T)GetComponent(typeof(T));

        /// <summary>
        /// Try to get the value of a <see cref="Type"/> component.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the component.</param>
        /// <param name="value">Out value of the component.</param>
        public bool TryGetComponent(Type type, out TComponent value)
        {
            if (!TryGetDerivedTypeFromBaseType(type, out type) || !HasComponent(type))
            {
                value = default;
                return false;
            }

            TComponent result;
            try
            {
                result = TypeConstruction.Construct<TComponent>(type);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to construct type '{type.FullName}'.\n{e.Message}");
                value = default;
                return false;
            }

            for (var i = 0; i < Dependencies.Count; ++i)
            {
                var dependency = Dependencies[i];
                if (dependency == null || !dependency)
                {
                    continue;
                }

                if (dependency.TryGetComponent(type, out var component))
                {
                    CopyComponent(ref result, ref component);
                }
            }

            for (var i = 0; i < Components.Count; ++i)
            {
                var component = Components[i];
                if (component.GetType() == type)
                {
                    CopyComponent(ref result, ref component);
                    break;
                }
            }

            value = result;
            return true;
        }

        /// <summary>
        /// Try to get the value of a <typeparamref name="T"/> component.
        /// </summary>
        /// <param name="value">Out value of the component.</param>
        /// <typeparam name="T">Type of the component.</typeparam>
        public bool TryGetComponent<T>(out T value) where T : TComponent
        {
            if (TryGetComponent(typeof(T), out var result))
            {
                value = (T)result;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Get a flatten list of all components from this <see cref="ComponentContainer{TObject}"/> and its dependencies.
        /// </summary>
        public List<TComponent> GetComponents()
        {
            var lookup = new Dictionary<Type, TComponent>();

            foreach (var dependency in Dependencies)
            {
                if (dependency == null || !dependency)
                {
                    continue;
                }

                var components = dependency.GetComponents();
                foreach (var component in components)
                {
                    lookup[component.GetType()] = component;
                }
            }

            foreach (var component in Components)
            {
                lookup[component.GetType()] = CopyComponent(component);
            }

            return lookup.Values.ToList();
        }

        /// <summary>
        /// Set the value of a <see cref="Type"/> component.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the component.</param>
        /// <param name="value">Value of the component to set.</param>
        public void SetComponent(Type type, TComponent value)
        {
            if (type == null)
            {
                throw new NullReferenceException(nameof(type));
            }

            if (type == typeof(object))
            {
                throw new InvalidOperationException($"{nameof(type)} cannot be '{typeof(object).FullName}'.");
            }

            if (type.IsInterface || type.IsAbstract)
            {
                throw new InvalidOperationException($"{nameof(type)} cannot be interface or abstract.");
            }

            for (var i = 0; i < Components.Count; ++i)
            {
                if (Components[i].GetType() == type)
                {
                    Components[i] = CopyComponent(value);
                    return;
                }
            }

            Components.Add(CopyComponent(value));
        }

        /// <summary>
        /// Set the value of a <typeparamref name="T"/> component.
        /// </summary>
        /// <param name="value">Value of the component to set.</param>
        /// <typeparam name="T">Type of the component.</typeparam>
        public void SetComponent<T>(T value) where T : TComponent => SetComponent(typeof(T), value);

        /// <summary>
        /// Remove a <see cref="Type"/> component from this <see cref="ComponentContainer{TObject}"/>.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the component.</param>
        public bool RemoveComponent(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException(nameof(type));
            }

            if (type == typeof(object))
            {
                throw new InvalidOperationException($"{nameof(type)} cannot be '{typeof(object).FullName}'.");
            }

            for (var i = 0; i < Components.Count; ++i)
            {
                if (type.IsAssignableFrom(Components[i].GetType()))
                {
                    Components.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Remove a <typeparamref name="T"/> component from this <see cref="ComponentContainer{TObject}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        public bool RemoveComponent<T>() where T : TComponent => RemoveComponent(typeof(T));

        /// <summary>
        /// Remove all components from this <see cref="ComponentContainer{TObject}"/>.
        /// </summary>
        public void ClearComponents() => Components.Clear();

        /// <summary>
        /// Visit a flatten list of all components from this <see cref="ComponentContainer{TObject}"/> and its dependencies.
        /// </summary>
        /// <param name="visitor">The visitor to use for visiting each component.</param>
        public void VisitComponents(IPropertyVisitor visitor)
        {
            var components = GetComponents();
            for (var i = 0; i < components.Count; ++i)
            {
                var component = components[i];
                PropertyContainer.Visit(ref component, visitor);
            }
        }

        /// <summary>
        /// Add a <see cref="ComponentContainer{TObject}"/> dependency.
        /// </summary>
        /// <param name="dependency">The dependency to add to this <see cref="ComponentContainer{TObject}"/>.</param>
        public void AddDependency(ComponentContainer<TComponent> dependency)
        {
            if (dependency == null || !dependency)
            {
                throw new ArgumentNullException(nameof(dependency));
            }

            if (!Dependencies.Contains(dependency))
            {
                Dependencies.Add(dependency);
            }
        }

        /// <summary>
        /// Add multiple <see cref="ComponentContainer{TObject}"/> dependencies.
        /// </summary>
        /// <param name="dependencies">The dependencies to add to this <see cref="ComponentContainer{TObject}"/>.</param>
        public void AddDependencies(params ComponentContainer<TComponent>[] dependencies)
        {
            foreach (var dependency in dependencies)
            {
                AddDependency(dependency);
            }
        }

        /// <summary>
        /// Get a list of all the dependencies for this <see cref="ComponentContainer{TObject}"/>.
        /// </summary>
        public IReadOnlyList<ComponentContainer<TComponent>> GetDependencies() => Dependencies;

        /// <summary>
        /// Remove a <see cref="ComponentContainer{TObject}"/> dependency.
        /// </summary>
        /// <param name="dependency">The dependency to remove from this <see cref="ComponentContainer{TObject}"/>.</param>
        public bool RemoveDependency(ComponentContainer<TComponent> dependency)
        {
            if (dependency == null)
            {
                throw new ArgumentNullException(nameof(dependency));
            }
            return Dependencies.Remove(dependency);
        }

        /// <summary>
        /// Remove multiple <see cref="ComponentContainer{TObject}"/> dependencies.
        /// </summary>
        /// <param name="dependencies">The dependencies to remove from this <see cref="ComponentContainer{TObject}"/>.</param>
        public void RemoveDependencies(params ComponentContainer<TComponent>[] dependencies)
        {
            foreach (var dependency in dependencies)
            {
                RemoveDependency(dependency);
            }
        }

        /// <summary>
        /// Remove all dependencies from this <see cref="ComponentContainer{TObject}"/>.
        /// </summary>
        public void ClearDependencies() => Dependencies.Clear();

        /// <summary>
        /// Serialize this <see cref="ComponentContainer{TObject}"/> to a JSON <see cref="string"/>.
        /// </summary>
        public string SerializeToJson() => JsonSerialization.Serialize(this, new ComponentContainerJsonVisitor());

        /// <summary>
        /// Deserialize a JSON <see cref="string"/> into the <paramref name="container"/>.
        /// </summary>
        /// <param name="container">The container to deserialize into.</param>
        /// <param name="json">The JSON string to deserialize from.</param>
        /// <returns><see langword="true"/> if deserialization was successful, <see langword="false"/> otherwise.</returns>
        public static bool DeserializeFromJson(ComponentContainer<TComponent> container, string json)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return TryDeserialize(container, stream);
            }
        }

        /// <summary>
        /// Serialize this <see cref="ComponentContainer{TObject}"/> to a file.
        /// </summary>
        /// <param name="path">The file path to write into.</param>
        public void SerializeToPath(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(path, SerializeToJson());
        }

        /// <summary>
        /// Deserialize the content of a file into the <paramref name="container"/>.
        /// </summary>
        /// <param name="container">The container to deserialize into.</param>
        /// <param name="path">The file path to deserialize from.</param>
        /// <returns><see langword="true"/> if deserialization was successful, <see langword="false"/> otherwise.</returns>
        public static bool DeserializeFromPath(ComponentContainer<TComponent> container, string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return TryDeserialize(container, stream, path);
            }
        }

        /// <summary>
        /// Serialize this <see cref="ComponentContainer{TObject}"/> to a stream.
        /// </summary>
        /// <param name="stream">The stream to write into.</param>
        public void SerializeToStream(Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(SerializeToJson());
            }
        }

        /// <summary>
        /// Deserialize a stream into the <paramref name="container"/>.
        /// </summary>
        /// <param name="container">The container to deserialize into.</param>
        /// <param name="stream">The stream to deserialize from.</param>
        /// <returns><see langword="true"/> if deserialization was successful, <see langword="false"/> otherwise.</returns>
        public static bool DeserializeFromStream(ComponentContainer<TComponent> container, Stream stream)
        {
            return TryDeserialize(container, stream);
        }

        static bool TryDeserialize(ComponentContainer<TComponent> container, Stream stream, string path = null)
        {
            try
            {
                container.ClearComponents();
                container.ClearDependencies();
                using (var result = JsonSerialization.DeserializeFromStream(stream, ref container))
                {
                    if (!result.Succeeded)
                    {
                        var assetPath = AssetDatabase.GetAssetPath(container);
                        var instanceMessage = string.IsNullOrEmpty(assetPath) ? $"in memory container of type {container.GetType().Name}" : assetPath.ToHyperLink();
                        var builder = new StringBuilder();
                        builder.AppendLine($"Encountered problems while deserializing {instanceMessage}:");
                        foreach (var evt in result.AllEvents)
                        {
                            builder.AppendLine($"    {evt}");
                        }
                        Debug.LogError(builder.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
            container.m_AssetAsJson = !string.IsNullOrEmpty(path) ? File.ReadAllText(path) : null;

            container.Components.RemoveAll(c => null == c);
            return true;
        }

        bool HasComponentOnSelf(Type type)
        {
            if (type == null || type == typeof(object))
            {
                return false;
            }
            return Components.Any(component => type.IsAssignableFrom(component.GetType()));
        }

        bool HasComponentOnDependency(Type type)
        {
            if (type == null || type == typeof(object))
            {
                return false;
            }
            return Dependencies.Any(dependency =>
            {
                if (dependency == null || !dependency)
                {
                    return false;
                }
                return dependency.HasComponent(type);
            });
        }

        bool TryGetDerivedTypeFromBaseType(Type baseType, out Type value)
        {
            value = baseType;

            if (baseType == null || baseType == typeof(object))
            {
                return false;
            }

            if (!baseType.IsInterface && !baseType.IsAbstract)
            {
                return true;
            }

            foreach (var dependency in Dependencies)
            {
                if (null == dependency && !dependency)
                {
                    continue;
                }

                if (dependency.TryGetDerivedTypeFromBaseType(baseType, out var type))
                {
                    value = type;
                }
            }

            foreach (var component in Components)
            {
                var type = component.GetType();
                if (baseType.IsAssignableFrom(type))
                {
                    value = type;
                    break;
                }
            }

            return true;
        }

        T CopyComponent<T>(T value) where T : TComponent
        {
            var result = TypeConstruction.Construct<T>(value.GetType());
            PropertyContainer.Construct(ref result, ref value).Dispose();
            PropertyContainer.Transfer(ref result, ref value).Dispose();
            return result;
        }

        void CopyComponent(ref TComponent dst, ref TComponent src)
        {
            PropertyContainer.Construct(ref dst, ref src).Dispose();
            PropertyContainer.Transfer(ref dst, ref src).Dispose();
        }

        public void OnBeforeSerialize()
        {
            m_AssetAsJson = SerializeToJson();
        }

        public void OnAfterDeserialize()
        {
            // Can't deserialize here, throws: "CreateJobReflectionData is not allowed to be called during serialization, call it from OnEnable instead."
        }

        public void OnEnable()
        {
            var assetPath = AssetDatabase.GetAssetPath(this);
            var assetContent = m_AssetAsJson;
            if (!string.IsNullOrEmpty(assetPath))
            {
                assetContent = File.ReadAllText(assetPath);
            }

            if (!string.IsNullOrEmpty(assetContent))
            {
                DeserializeFromJson(this, assetContent);
                if (assetContent != m_AssetAsJson)
                {
                    m_AssetAsJson = assetContent;
                    AssetChanged?.Invoke(this);
                }
            }
        }
        
        class ComponentContainerJsonVisitor : JsonVisitor
        {
            public ComponentContainerJsonVisitor()
            {
                JsonVisitorRegistration.Invoke(this);
            }

            protected override string GetTypeInfo<TProperty, TContainer, T>(TProperty property,
                ref TContainer container, ref T value)
            {
                if (null != value && container is ComponentContainer<TComponent>)
                {
                    var type = value.GetType();
                    return $"{type}, {type.Assembly.GetName().Name}";
                }
                return null;
            }
        }
    }
}
