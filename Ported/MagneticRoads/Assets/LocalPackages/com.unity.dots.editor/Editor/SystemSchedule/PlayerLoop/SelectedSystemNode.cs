namespace Unity.Entities.Editor
{
    interface ISelectedSystemNode
    {
        SelectedSystem SelectedSystem { get; }
    }

    abstract class SelectedSelectedSystemNode<TNode> : PlayerLoopNode<SelectedSystem>, ISelectedSystemNode
        where TNode : SelectedSelectedSystemNode<TNode>, new()
    {
        public override string Name => UnityEditor.ObjectNames.NicifyVariableName(Value.GetSystemType().Name);

        public override string FullName => Value.GetSystemType().FullName;
        public override string NameWithWorld => Name + " (" + Value.World?.Name + ")";

        public override unsafe bool Enabled
        {
            get => Value.StatePointer->Enabled;
            set => Value.StatePointer->Enabled = value;
        }

        public override bool EnabledInHierarchy => Enabled && (Parent?.EnabledInHierarchy ?? true);

        public SelectedSystem SelectedSystem
        {
            get
            {
                if (Value != null && Value is SelectedSystem systemSelection)
                    return systemSelection;

                return null;
            }
        }

        public override int Hash
        {
            get
            {
                unchecked
                {
                    var hashCode = NameWithWorld.GetHashCode();
                    hashCode = (hashCode * 397) ^ (Parent?.Name.GetHashCode() ?? 0);
                    hashCode = (hashCode * 397) ^ Value.GetHashCode();
                    return hashCode;
                }
            }
        }

        public override bool ShowForWorld(World world)
        {
            if (Value.World == null)
                return false;

            if (null == world)
                return true;

            foreach (var child in Children)
            {
                if (child.ShowForWorld(world))
                    return true;
            }

            return Value.World == world;
        }

        public override void Reset()
        {
            base.Reset();
            Value = null;
        }

        public override void ReturnToPool()
        {
            base.ReturnToPool();
            Pool<TNode>.Release((TNode)this);
        }

        public override unsafe bool IsRunning => Value.StatePointer->ShouldRunSystem();
    }

    class ComponentGroupNode : SelectedSelectedSystemNode<ComponentGroupNode>
    {
    }

    class SelectedSelectedSystemNode : SelectedSelectedSystemNode<SelectedSelectedSystemNode>
    {
    }
}
