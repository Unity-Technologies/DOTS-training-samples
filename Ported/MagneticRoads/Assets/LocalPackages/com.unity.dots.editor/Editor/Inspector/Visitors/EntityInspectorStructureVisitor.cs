using System.Collections.Generic;
using System.Linq;
using Unity.Properties;

namespace Unity.Entities.Editor
{
    /// <summary>
    /// Helper type to contain the component order of a given entity.
    /// </summary>
    class EntityInspectorComponentOrder
    {
        public bool Equals(EntityInspectorComponentOrder other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this == (EntityInspectorComponentOrder)obj;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Components != null ? Components.GetHashCode() : 0) * 397) ^
                       (Tags != null ? Tags.GetHashCode() : 0);
            }
        }

        public readonly List<string> Components = new List<string>();
        public readonly List<string> Tags = new List<string>();

        public void Reset()
        {
            Components.Clear();
            Tags.Clear();
        }

        public static bool operator ==(EntityInspectorComponentOrder lhs, EntityInspectorComponentOrder rhs)
        {
            if (ReferenceEquals(null, lhs))
                return ReferenceEquals(null, rhs);

            if (ReferenceEquals(null, rhs))
                return false;

            return lhs.Components.SequenceEqual(rhs.Components)
                   && lhs.Tags.SequenceEqual(rhs.Tags);
        }

        public static bool operator !=(EntityInspectorComponentOrder lhs, EntityInspectorComponentOrder rhs)
        {
            return !(lhs == rhs);
        }
    }

    class EntityInspectorStructureVisitor : PropertyVisitor
    {
        public EntityInspectorComponentOrder ComponentOrder { get; }

        public EntityInspectorStructureVisitor()
        {
            ComponentOrder = new EntityInspectorComponentOrder();
        }

        public void Reset()
        {
            ComponentOrder.Reset();
        }

        protected override void VisitProperty<TContainer, TValue>(Property<TContainer, TValue> property,
            ref TContainer container, ref TValue value)
        {
            if (property is IComponentProperty componentProperty)
            {
                if (componentProperty.Type == ComponentPropertyType.Tag)
                    ComponentOrder.Tags.Add(componentProperty.Name);
                else
                    ComponentOrder.Components.Add(componentProperty.Name);
            }
        }

        protected override void VisitList<TContainer, TList, TElement>(Property<TContainer, TList> property,
            ref TContainer container, ref TList value)
        {
            if (property is IComponentProperty componentProperty)
            {
                ComponentOrder.Components.Add(componentProperty.Name);
            }
        }
    }
}
