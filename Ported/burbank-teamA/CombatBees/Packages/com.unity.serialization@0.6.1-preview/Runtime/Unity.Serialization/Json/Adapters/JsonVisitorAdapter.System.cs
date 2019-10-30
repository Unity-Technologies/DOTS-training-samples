using System;
using System.Globalization;
using Unity.Properties;

namespace Unity.Serialization.Json
{
    class JsonVisitorAdapterSystem : JsonVisitorAdapter,
        IVisitAdapter<Guid>,
        IVisitAdapter<DateTime>,
        IVisitAdapter<TimeSpan>
    {
        public JsonVisitorAdapterSystem(JsonVisitor visitor) : base(visitor) { }

        public static void RegisterTypes()
        {
            TypeConversion.Register<SerializedStringView, Guid>(view => Guid.TryParseExact(view.ToString(), "N", out var guid) ? guid : default);
            TypeConversion.Register<SerializedStringView, DateTime>(view => DateTime.TryParseExact(view.ToString(), "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateTime) ? dateTime.ToLocalTime() : default);
            TypeConversion.Register<SerializedStringView, TimeSpan>(view => TimeSpan.TryParseExact(view.ToString(), "c", CultureInfo.InvariantCulture, out var timeSpan) ? timeSpan : default);
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref Guid value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, Guid>
        {
            AppendJsonString(property, value.ToString("N", CultureInfo.InvariantCulture));
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref DateTime value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, DateTime>
        {
            AppendJsonString(property, value.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture));
            return VisitStatus.Override;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TimeSpan value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TimeSpan>
        {
            AppendJsonString(property, value.ToString("c", CultureInfo.InvariantCulture));
            return VisitStatus.Override;
        }
    }
}
