using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.Properties.Reflection
{
    /// <summary>
    /// Common interface between <see cref="FieldInfo"/> and <see cref="PropertyInfo"/> for getting and setting values.
    /// </summary>
    interface IMemberInfo
    {
        string Name { get; }
        Type PropertyType { get; }
        object GetValue(object obj);
        void SetValue(object obj, object value);
        IEnumerable<Attribute> GetCustomAttributes();
        bool CanWrite();
    }

    class ReflectedFieldPropertyGenerator : IReflectedPropertyGenerator
    {
        readonly struct FieldMember : IMemberInfo
        {
            readonly FieldInfo m_FieldInfo;

            public FieldMember(FieldInfo fieldInfo) => m_FieldInfo = fieldInfo;
            public string Name => m_FieldInfo.Name;
            public Type PropertyType => m_FieldInfo.FieldType;
            public object GetValue(object obj) => m_FieldInfo.GetValue(obj);
            public void SetValue(object obj, object value) => m_FieldInfo.SetValue(obj, value);
            public IEnumerable<Attribute> GetCustomAttributes() => m_FieldInfo.GetCustomAttributes();
            public bool CanWrite() => true;
        }

        readonly struct PropertyMember : IMemberInfo
        {
            readonly PropertyInfo m_PropertyInfo;

            public PropertyMember(PropertyInfo propertyInfo) => m_PropertyInfo = propertyInfo;
            public string Name => m_PropertyInfo.Name;
            public Type PropertyType => m_PropertyInfo.PropertyType;
            public object GetValue(object obj) => m_PropertyInfo.GetValue(obj);
            public void SetValue(object obj, object value) => m_PropertyInfo.SetValue(obj, value);
            public IEnumerable<Attribute> GetCustomAttributes() => m_PropertyInfo.GetCustomAttributes();
            public bool CanWrite() => m_PropertyInfo.CanWrite;
        }

        static readonly MethodInfo s_GenerateArrayPropertyMethod = 
            typeof(ReflectedFieldPropertyGenerator).GetMethod(nameof(GenerateArrayProperty), BindingFlags.Static | BindingFlags.NonPublic);
        
        static readonly MethodInfo s_GenerateGenericListPropertyMethod = 
            typeof(ReflectedFieldPropertyGenerator).GetMethod(nameof(GenerateGenericListProperty), BindingFlags.Static | BindingFlags.NonPublic);
        
        public bool Generate<TContainer, TValue>(FieldInfo fieldInfo, ReflectedPropertyBag<TContainer> propertyBag)
        {
            return Generate<TContainer, TValue>(new FieldMember(fieldInfo), propertyBag);
        }

        public bool Generate<TContainer, TValue>(PropertyInfo propertyInfo, ReflectedPropertyBag<TContainer> propertyBag)
        {
            return Generate<TContainer, TValue>(new PropertyMember(propertyInfo), propertyBag);
        }

        bool Generate<TContainer, TValue>(IMemberInfo memberInfo, ReflectedPropertyBag<TContainer> propertyBag)
        {
            if (typeof(TValue).IsArray)
            {
                var method = s_GenerateArrayPropertyMethod.MakeGenericMethod(typeof(TContainer), typeof(TValue).GetElementType());
                method.Invoke(this, new object[] {memberInfo, propertyBag});
            }
            else if (typeof(TValue).IsGenericType && 
                     (typeof(TValue).GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)) ||
                      typeof(TValue).GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>))))
            {
                var method = s_GenerateGenericListPropertyMethod.MakeGenericMethod(typeof(TContainer), memberInfo.PropertyType, typeof(TValue).GetGenericArguments()[0]);
                method.Invoke(this, new object[] {memberInfo, propertyBag});
            }
            else
            {
                if (memberInfo.PropertyType.IsPointer)
                    return false;
                propertyBag.AddProperty<ReflectedMemberProperty<TContainer, TValue>, TValue>(new ReflectedMemberProperty<TContainer, TValue>(memberInfo));
            }

            return true;
        }

        static void GenerateArrayProperty<TContainer, TElement>(IMemberInfo member, ReflectedPropertyBag<TContainer> propertyBag)
        {
            propertyBag.AddCollectionProperty<ReflectedArrayProperty<TContainer, TElement>, TElement[]>(new ReflectedArrayProperty<TContainer, TElement>(member));
        }

        static void GenerateGenericListProperty<TContainer, TValue, TElement>(IMemberInfo member, ReflectedPropertyBag<TContainer> propertyBag)
            where TValue : IList<TElement>
        {
            propertyBag.AddCollectionProperty<ReflectedGenericListProperty<TContainer, TValue, TElement>, TValue>(new ReflectedGenericListProperty<TContainer, TValue, TElement>(member));
        }
    }
}