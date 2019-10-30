namespace Unity.Properties.Editor
{
    /// <summary>
    /// Allows to tag a <see cref="IInspector"/> to override the drawing of that field.
    /// </summary>
    /// <typeparam name="TDrawerAttribute">The <see cref="PropertyDrawer"/> attribute for which this drawer is for.</typeparam>
    public interface IPropertyDrawer<TDrawerAttribute>
        where TDrawerAttribute : UnityEngine.PropertyAttribute
    {
    }
}