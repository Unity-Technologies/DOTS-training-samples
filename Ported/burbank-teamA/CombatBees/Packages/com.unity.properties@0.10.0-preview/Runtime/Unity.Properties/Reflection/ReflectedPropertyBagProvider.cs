using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.Properties.Reflection
{
    public class ReflectedPropertyBagProvider
    {
        readonly MethodInfo m_GenerateMethod;
        readonly MethodInfo m_CreatePropertyFromFieldInfoMethod;
        readonly MethodInfo m_CreatePropertyFromPropertyInfoMethod;
        readonly List<IReflectedPropertyGenerator> m_Generators;

        public ReflectedPropertyBagProvider()
        {
            m_GenerateMethod = typeof(ReflectedPropertyBagProvider).GetMethods().First(x => x.Name == nameof(Generate) && x.IsGenericMethod);
            m_CreatePropertyFromFieldInfoMethod = typeof(ReflectedPropertyBagProvider).GetMethod(nameof(CreatePropertyFromFieldInfo), BindingFlags.Instance | BindingFlags.NonPublic);
            m_CreatePropertyFromPropertyInfoMethod = typeof(ReflectedPropertyBagProvider).GetMethod(nameof(CreatePropertyFromPropertyInfo), BindingFlags.Instance | BindingFlags.NonPublic);
            m_Generators = new List<IReflectedPropertyGenerator>();

            // Register default generators.
            AddGenerator(new ReflectedFieldPropertyGenerator()); // baseline FieldInfo property
            AddGenerator(new UnmanagedPropertyGenerator());      // unmanaged offset based property
        }

        public void AddGenerator(IReflectedPropertyGenerator generator)
        {
            m_Generators.Add(generator);
        }

        public IPropertyBag<TContainer> Generate<TContainer>()
        {
            if (typeof(TContainer).IsEnum)
            {
                return null;
            }

            var propertyBag = new ReflectedPropertyBag<TContainer>();

            var fields = typeof(TContainer).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.DeclaringType != typeof(TContainer))
                {
                    continue;
                }

                if (field.IsPrivate && field.GetCustomAttribute<PropertyAttribute>() == null)
                {
                    continue;
                }
                
                var method = m_CreatePropertyFromFieldInfoMethod.MakeGenericMethod(typeof(TContainer), field.FieldType);
                method.Invoke(this, new object[] {field, propertyBag});
            }
            
            var properties = typeof(TContainer).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var property in properties)
            {
                if (property.DeclaringType != typeof(TContainer))
                {
                    continue;
                }

                if (property.GetCustomAttribute<PropertyAttribute>() == null)
                {
                    continue;
                }
                
                var method = m_CreatePropertyFromPropertyInfoMethod.MakeGenericMethod(typeof(TContainer), property.PropertyType);
                method.Invoke(this, new object[] {property, propertyBag});
            }

            var baseType = typeof(TContainer).BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                fields = baseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    if (field.DeclaringType != baseType)
                    {
                        continue;
                    }

                    if (field.IsPrivate && field.GetCustomAttribute<PropertyAttribute>() == null)
                    {
                        continue;
                    }

                    var method = m_CreatePropertyFromFieldInfoMethod.MakeGenericMethod(typeof(TContainer), field.FieldType);
                    method.Invoke(this, new object[] { field, propertyBag });
                }

                properties = baseType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    if (property.DeclaringType != baseType)
                    {
                        continue;
                    }

                    if (property.GetCustomAttribute<PropertyAttribute>() == null)
                    {
                        continue;
                    }

                    var method = m_CreatePropertyFromPropertyInfoMethod.MakeGenericMethod(typeof(TContainer), property.PropertyType);
                    method.Invoke(this, new object[] { property, propertyBag });
                }

                baseType = baseType.BaseType;
            }

            return propertyBag;
        }

        public IPropertyBag Generate(Type type)
        {
            var method = m_GenerateMethod.MakeGenericMethod(type);
            return (IPropertyBag) method.Invoke(this, null);
        }

        void CreatePropertyFromFieldInfo<TContainer, TValue>(FieldInfo field, ReflectedPropertyBag<TContainer> propertyBag)
        {
            for (var index = m_Generators.Count - 1; index >= 0; index--)
            {
                var generator = m_Generators[index];

                if (!generator.Generate<TContainer, TValue>(field, propertyBag))
                {
                    continue;
                }

                break;
            }
        }

        void CreatePropertyFromPropertyInfo<TContainer, TValue>(PropertyInfo property, ReflectedPropertyBag<TContainer> propertyBag)
        {
            for (var index = m_Generators.Count - 1; index >= 0; index--)
            {
                var generator = m_Generators[index];

                if (!generator.Generate<TContainer, TValue>(property, propertyBag))
                {
                    continue;
                }

                break;
            }
        }
    }
}
