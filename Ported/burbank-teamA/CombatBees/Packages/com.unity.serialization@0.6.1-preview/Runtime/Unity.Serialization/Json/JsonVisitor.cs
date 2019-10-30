using System.Text;
using Unity.Properties;

namespace Unity.Serialization.Json
{
    /// <summary>
    /// A visitor that traverses a property container and outputs a JSON string.
    ///
    /// You can enrich the serialization by
    ///     * Extend this class and overriding any methods.
    ///     * Implement a <see cref="JsonVisitorAdapter"/> and register it using <see cref="JsonVisitor.AddAdapter"/>
    /// 
    /// </summary>
    public class JsonVisitor : PropertyVisitor
    {
        /// <summary>
        /// Constants for styling and special keys.
        /// </summary>
        public static class Style
        {
            /// <summary>
            /// The key used when writing out custom type information.
            /// This can be consumed during deserialization to reconstruct the concrete type.
            /// </summary>
            public static string TypeInfoKey = "$type";

            /// <summary>
            /// Spaces for indentation
            /// </summary>
            public static int Space = 4;
        }

        /// <summary>
        /// The inner string builder being written to.
        /// </summary>
        public StringBuilder Builder { get; } = new StringBuilder(1024);

        /// <summary>
        /// Gets or sets the current indent level.
        /// </summary>
        public int Indent { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonVisitor"/> with the default adapters.
        /// </summary>
        public JsonVisitor()
        {
            AddAdapter(new JsonVisitorAdapterPrimitives(this));
            AddAdapter(new JsonVisitorAdapterSystem(this));
            AddAdapter(new JsonVisitorAdapterSystemIO(this));
            AddAdapter(new JsonVisitorAdapterUnityEngine(this));
            AddAdapter(new JsonVisitorAdapterUnityEditor(this));
        }

        /// <summary>
        /// Invoked before visiting a container type.
        /// </summary>
        /// <param name="property">The property being visited.</param>
        /// <param name="container">The container/host being visited.</param>
        /// <param name="value">The value being visited.</param>
        /// <param name="changeTracker">The change tracker for change propagation.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <typeparam name="TContainer">The container type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <returns>The result of the visit.</returns>
        protected override VisitStatus BeginContainer<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
        {
            if (property is ICollectionElementProperty)
            {
                Indent--;
                Builder.Length -= 1;
                Builder.Append(Builder[Builder.Length - 1] == ',' ? " {\n" : "{\n");
            }
            else
            {
                Builder.Append(' ', Style.Space * Indent);
                Builder.Append("\"");
                Builder.Append(property.GetName());
                Builder.Append("\": {\n");
            }

            Indent++;

            var typeInfo = GetTypeInfo(property, ref container, ref value);

            if (null != typeInfo)
            {
                Builder.Append(' ', Style.Space * Indent);
                Builder.Append($"\"{Style.TypeInfoKey}\": \"");
                Builder.Append(typeInfo);
                Builder.Append("\",\n");
            }

            return VisitStatus.Handled;
        }

        /// <summary>
        /// Invoked after visiting a container type.
        /// </summary>
        /// <param name="property">The property being visited.</param>
        /// <param name="container">The container/host being visited.</param>
        /// <param name="value">The value being visited.</param>
        /// <param name="changeTracker">The change tracker for change propagation.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <typeparam name="TContainer">The container type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        protected override void EndContainer<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
        {
            Indent--;

            if (Builder[Builder.Length - 2] == ',')
            {
                Builder.Length -= 2;
                Builder.Append('\n');
                Builder.Append(' ', Style.Space * Indent);
            }
            else
            {
                Builder.Length -= 1;
            }

            if (property is ICollectionElementProperty)
            {
                Indent++;
            }

            Builder.Append("},\n");
        }

        /// <summary>
        /// Invoked before visiting a collection type.
        /// </summary>
        /// <param name="property">The property being visited.</param>
        /// <param name="container">The container/host being visited.</param>
        /// <param name="value">The value being visited.</param>
        /// <param name="changeTracker">The change tracker for change propagation.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <typeparam name="TContainer">The container type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        protected override VisitStatus BeginCollection<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
        {
            Builder.Append(' ', Style.Space * Indent);
            Builder.Append('\"');
            Builder.Append(property.GetName());
            Builder.Append("\": [\n");
            Indent++;
            return VisitStatus.Handled;
        }

        /// <summary>
        /// Invoked after visiting a collection type.
        /// </summary>
        /// <param name="property">The property being visited.</param>
        /// <param name="container">The container/host being visited.</param>
        /// <param name="value">The value being visited.</param>
        /// <param name="changeTracker">The change tracker for change propagation.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <typeparam name="TContainer">The container type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        protected override void EndCollection<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
        {
            Indent--;

            if (Builder[Builder.Length - 2] == ',')
            {
                Builder.Length -= 2;
            }
            else
            {
                Builder.Length -= 1;
            }

            var skipNewline = Builder[Builder.Length - 1] == '}' &&
                              Builder[Builder.Length - 3] == ' ';

            skipNewline = skipNewline | Builder[Builder.Length - 1] == '[';

            if (!skipNewline)
            {
                Builder.Append("\n");
                Builder.Append(' ', Style.Space * Indent);
            }

            Builder.Append("],\n");
        }

        /// <summary>
        /// Override this method to provide your own type information in the serialized json.
        /// </summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <typeparam name="TContainer">The container type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <returns>The type identifier as a string.</returns>
        protected virtual string GetTypeInfo<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value)
            where TProperty : IProperty<TContainer, TValue>
        {
            return null;
        }
    }
}
