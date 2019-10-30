using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;

namespace Unity.Build
{
    /// <summary>
    /// Holds contextual information while a <see cref="Build.BuildPipeline"/> is executing.
    /// </summary>
    public sealed class BuildContext
    {
        readonly Dictionary<Type, object> m_Values = new Dictionary<Type, object>();

        /// <summary>
        /// Quick access to <see cref="Build.BuildSettings"/> value.
        /// </summary>
        public BuildSettings BuildSettings
        {
            get => Get<BuildSettings>();
            set => Set(value);
        }

        /// <summary>
        /// Quick access to <see cref="Build.BuildPipeline"/> value.
        /// </summary>
        public BuildPipeline BuildPipeline
        {
            get => Get<BuildPipeline>();
            set => Set(value);
        }

        /// <summary>
        /// Current <see cref="Build.BuildPipeline"/> execution status.
        /// </summary>
        public BuildPipelineResult BuildPipelineStatus { get; } = BuildPipelineResult.Success();

        /// <summary>
        /// Quick access to <see cref="Build.BuildProgress"/> value.
        /// </summary>
        public BuildProgress BuildProgress => Get<BuildProgress>();

        /// <summary>
        /// Quick access to <see cref="Build.BuildManifest"/> value.
        /// </summary>
        public BuildManifest BuildManifest => GetOrCreate<BuildManifest>();

        /// <summary>
        /// Create a new instance of <see cref="BuildContext"/>.
        /// </summary>
        public BuildContext() { }

        /// <summary>
        /// Create a new instance of <see cref="BuildContext"/> with the specified values.
        /// </summary>
        /// <param name="values">Values to set on context.</param>
        public BuildContext(params object[] values)
        {
            Array.ForEach(values, value => Set(value));
        }

        /// <summary>
        /// Get value of type <typeparamref name="T"/> if found, otherwise <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <returns>The value of type <typeparamref name="T"/> if found, otherwise <see langword="null"/>.</returns>
        public T Get<T>() where T : class => m_Values.FirstOrDefault(pair => typeof(T).IsAssignableFrom(pair.Key)).Value as T;

        /// <summary>
        /// Get value of type <typeparamref name="T"/> if found, otherwise a new instance of type <typeparamref name="T"/> constructed with <see cref="TypeConstruction"/>.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <returns>The value or new instance of type <typeparamref name="T"/>.</returns>
        public T GetOrCreate<T>() where T : class
        {
            var value = Get<T>();
            if (value == null)
            {
                value = TypeConstruction.Construct<T>();
                Set(value);
            }
            return value;
        }

        /// <summary>
        /// Set value of type <typeparamref name="T"/> to this <see cref="BuildContext"/>.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="value">The value to set.</param>
        public void Set<T>(T value) where T : class
        {
            if (value == null)
            {
                return;
            }

            var type = value.GetType();
            if (type == typeof(object))
            {
                return;
            }

            m_Values[type] = value;
        }

        /// <summary>
        /// Remove value of type <typeparamref name="T"/> from this <see cref="BuildContext"/>.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <returns><see langword="true"/> if the value was removed, otherwise <see langword="false"/>.</returns>
        public bool Remove<T>() where T : class => m_Values.Remove(typeof(T));

        internal IReadOnlyCollection<object> GetAll() => m_Values.Values;
    }
}
