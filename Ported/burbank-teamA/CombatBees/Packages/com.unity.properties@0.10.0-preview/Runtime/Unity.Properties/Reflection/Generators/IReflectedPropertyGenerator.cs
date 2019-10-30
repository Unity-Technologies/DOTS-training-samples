using System.Reflection;

namespace Unity.Properties.Reflection
{
    public interface IReflectedPropertyGenerator
    {
        bool Generate<TContainer, TValue>(FieldInfo fieldInfo, ReflectedPropertyBag<TContainer> propertyBag);
        bool Generate<TContainer, TValue>(PropertyInfo propertyInfo, ReflectedPropertyBag<TContainer> propertyBag);
    }
}
