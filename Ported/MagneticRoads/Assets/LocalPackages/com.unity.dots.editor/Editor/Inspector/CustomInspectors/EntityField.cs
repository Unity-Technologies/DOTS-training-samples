using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor.Inspectors
{
    /// <summary>
    ///   <para>Provides an Element displaying an <see cref="Entity"/>.</para>
    /// </summary>
    class EntityField : BaseField<Entity>
    {
        class EntityBinding : IBinding
        {
            readonly EntityField m_Field;

            public EntityBinding(EntityField field)
            {
                m_Field = field;
            }

            void IBinding.PreUpdate()
            {
                // Nothing to do.
            }

            void IBinding.Update()
            {
                m_Field.SetEntityName();
            }

            void IBinding.Release()
            {
                // Nothing to do.
            }
        }

        readonly VisualElement m_InputRoot;
        readonly Label m_EntityName;

        World m_World;

        /// <summary>
        /// Sets/Gets the <see cref="World"/> that contains the <see cref="Entity"/>. If <see langword="null"/>, the field will not
        /// show the entity name.
        /// </summary>
        public World World
        {
            get => m_World;
            set
            {
                if (m_World == value)
                    return;
                m_World = value;
                SetEntityName();
            }
        }

        EntityManager EntityManager => World.EntityManager;

        /// <summary>
        /// Constructs a new instance of <see cref="EntityField"/> with no label.
        /// </summary>
        public EntityField() : this(null)
        {
        }

        /// <summary>
        /// Constructs a new instance of <see cref="EntityField"/> with a label.
        /// </summary>
        /// <param name="label">The label.</param>
        public EntityField(string label) : base(label, null)
        {
            m_InputRoot = this.Q(className: UssClasses.UIToolkit.BaseField.Input);
            m_InputRoot.AddToClassList("unity-entity-field__input");
            m_InputRoot.pickingMode = PickingMode.Position;
            Resources.Templates.Inspector.InspectorStyle.AddStyles(this);
            Resources.Templates.Inspector.EntityField.Clone(m_InputRoot);
            m_EntityName = m_InputRoot.Q<Label>(className: "unity-entity-field__name");
            m_EntityName.binding = new EntityBinding(this);
            m_InputRoot.RegisterCallback<ClickEvent, Func<World>>(OnClicked, GetWorld);
        }

        /// <inheritdoc/>
        public override Entity value
        {
            get => base.value;
            set
            {
                if (this.value == value)
                    return;
                base.value = value;
                SetEntityName();
            }
        }

        /// <inheritdoc/>
        public override void SetValueWithoutNotify(Entity newValue)
        {
            if (value == newValue)
                return;
            base.SetValueWithoutNotify(newValue);
            SetEntityName();
        }

        World GetWorld()
        {
            return m_World;
        }

        void SetEntityName()
        {
            SetEntityName(GetEntityDisplayName());
        }

        void SetEntityName(string displayName)
        {
            if (displayName != m_EntityName.text)
                m_EntityName.text = displayName;
        }

        string GetEntityDisplayName()
        {
            var entity = value;
            if (null == World || !World.IsCreated)
            {
                return $"Entity {{{entity.Index}:{entity.Version}}}";
            }

            var entityManager = EntityManager;
            if (!entityManager.Exists(entity))
            {
                return entity == Entity.Null
                    ? $"None ({nameof(Entity)})"
                    : $"Invalid ({nameof(Entity)})";
            }

            var entityName = entityManager.GetName(entity);
            return $"{entityName} {{{entity.Index}:{entity.Version}}}";
        }

        void OnClicked(ClickEvent evt, Func<World> worldFunc)
        {
            var world = worldFunc();
            if (null == world || evt.clickCount <= 1)
                return;

            EntitySelectionProxy.SelectEntity(world, value);
        }
    }
}
