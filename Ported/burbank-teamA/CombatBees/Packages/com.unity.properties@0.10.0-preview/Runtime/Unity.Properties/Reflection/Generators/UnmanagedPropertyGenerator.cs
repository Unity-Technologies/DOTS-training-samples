using System;
using System.Linq;
using System.Reflection;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Properties.Reflection
{
    public class UnmanagedPropertyGenerator : IReflectedPropertyGenerator
    {
        public bool Generate<TContainer, TValue>(FieldInfo fieldInfo, ReflectedPropertyBag<TContainer> propertyBag)
        {
            if (!typeof(TContainer).IsValueType)
            {
                return false;
            }

            if (!fieldInfo.FieldType.IsValueType)
            {
                return false;
            }
            
            if (!UnsafeUtility.IsBlittable(fieldInfo.FieldType) && fieldInfo.FieldType != typeof(char))
            {
                return false;
            }

            var propertyType = typeof(UnmanagedProperty<,>).MakeGenericType(typeof(TContainer), fieldInfo.FieldType);
            var property = Activator.CreateInstance(propertyType, 
                                                    fieldInfo.Name, 
                                                    UnsafeUtility.GetFieldOffset(fieldInfo), 
                                                    null != fieldInfo.GetCustomAttribute<ReadOnlyAttribute>(),
                                                    new PropertyAttributeCollection(fieldInfo.GetCustomAttributes().ToArray()));
            propertyBag.AddProperty<IProperty<TContainer, TValue>, TValue>((IProperty<TContainer, TValue>) property);
            return true;
        }

        public bool Generate<TContainer, TValue>(PropertyInfo propertyInfo, ReflectedPropertyBag<TContainer> propertyBag)
        {
            // Can't do unmanaged C# properties.
            return false;
        }
    }
}
