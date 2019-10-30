namespace Unity.Properties
{
    public static class TypeInstanceFactory
    {
        public interface ITypeConstructor<out TValue>
        {
            TValue Construct<TInput>(TInput source);
        }

        public interface ITypeConstructor<out TValue, in TInput> 
        {
            TValue Construct(TInput source);
        }

        struct Constructor<TValue>
        {
            public static ITypeConstructor<TValue> Construct;
        }

        struct Constructor<TValue, TInput>
        {
            public static ITypeConstructor<TValue, TInput> Construct;
        }

        public static void Register<TInput, TValue>(ITypeConstructor<TInput, TValue> handler)
        {
            Constructor<TInput, TValue>.Construct = handler;
        }
        
        public static void Register<TValue>(ITypeConstructor<TValue> handler)
        {
            Constructor<TValue>.Construct = handler;
        }

        public static bool IsRegistered<TValue, TInput>()
        {
            return null != Constructor<TInput, TValue>.Construct || null != Constructor<TValue>.Construct;
        }

        /// <summary>
        /// Constructs an instance of <see cref="TValue"/> based on the given <see cref="TInput"/> value.
        /// </summary>
        /// <param name="source">The source value containing the data for construction.</param>
        /// <typeparam name="TInput">The source type containing type information.</typeparam>
        /// <typeparam name="TValue">The destination type to construct.</typeparam>
        /// <returns>A new instance of the destination type</returns>
        public static TValue Construct<TValue, TInput>(TInput source)
        {
            // Try to use the fully typed implementation if one was provided.
            if (Constructor<TInput, TValue>.Construct != null)
            {
                return Constructor<TValue, TInput>.Construct.Construct(source);
            }
            
            // Try to use a value typed implementation if one was provided.
            if (Constructor<TValue>.Construct != null)
            {
                return Constructor<TValue>.Construct.Construct(source);
            }

            return default;
        }
        
        public static bool TryConstruct<TValue, TInput>(TInput source, out TValue value)
        {
            // Try to use the fully typed implementation if one was provided.
            if (Constructor<TInput, TValue>.Construct != null)
            {
                value = Constructor<TValue, TInput>.Construct.Construct(source);
                return true;
            }
            
            // Try to use a value typed implementation if one was provided.
            if (Constructor<TValue>.Construct != null)
            {
                value = Constructor<TValue>.Construct.Construct(source);
                return true;
            }

            if (typeof(TValue).IsValueType)
            {
                value = default;
                return true;
            }

            value = default;
            return false;
        }
    }
}