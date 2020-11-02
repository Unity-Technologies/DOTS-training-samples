using System.Collections.Generic;
using System.Linq;

namespace Unity.Entities.Editor
{
    /// <summary>
    /// Base class representing an elements of the <see cref="UnityEngine.LowLevel.PlayerLoopSystem"/> for system scheduling purposes.
    /// </summary>
    /// <typeparam name="T">Type of the node's value.</typeparam>
    abstract class PlayerLoopNode<T> : IPlayerLoopNode
    {
        /// <inheritdoc/>
        public IPlayerLoopNode Parent { get; set; }

        /// <inheritdoc/>
        public List<IPlayerLoopNode> Children { get; } = new List<IPlayerLoopNode>();

        /// <summary>
        /// Returns the value of the current node.
        /// </summary>
        public T Value { get; set; }

        /// <inheritdoc/>
        public abstract string Name { get; }

        /// <inheritdoc/>
        public abstract string NameWithWorld { get; }

        /// <summary>
        /// Returns the full name of the node's value.
        /// </summary>
        public abstract string FullName { get; }

        /// <inheritdoc/>
        public abstract bool Enabled { get; set; }

        /// <inheritdoc/>
        public abstract bool EnabledInHierarchy { get; }

        /// <inheritdoc/>
        public abstract int Hash { get; }

        /// <inheritdoc/>
        public abstract bool ShowForWorld(World world);

        /// <inheritdoc/>
        public abstract bool IsRunning { get; }

        public virtual void Reset()
        {
            Children.Clear();
            Parent = null;
        }

        public virtual void ReturnToPool()
        {
            foreach (var child in Children.OfType<IPoolable>())
            {
                child.ReturnToPool();
            }
        }
    }
}
