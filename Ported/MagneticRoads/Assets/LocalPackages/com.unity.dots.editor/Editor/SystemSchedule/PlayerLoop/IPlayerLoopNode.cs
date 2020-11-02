using System.Collections.Generic;

namespace Unity.Entities.Editor
{
    /// <summary>
    /// Interface to abstract heterogeneous elements of the <see cref="UnityEngine.LowLevel.PlayerLoopSystem"/>.
    /// </summary>
    interface IPlayerLoopNode : IPoolable
    {
        /// <summary>
        /// Returns the <see cref="IPlayerLoopNode"/> parent of the node.
        /// </summary>
        /// <remarks>Will be <see langword="null"/> for root items.</remarks>
        IPlayerLoopNode Parent { get; set; }

        /// <summary>
        /// Returns the children of the node.
        /// </summary>
        List<IPlayerLoopNode> Children { get; }

        /// <summary>
        /// Returns the display name of the node.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the display name of the node when inspecting the full player loop.
        /// </summary>
        string NameWithWorld { get; }

        /// <summary>
        /// Returns <see langword="true"/> if the current node is enabled, <see langword="false"/> otherwise.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Returns <see langword="true"/> if the current node and all its ancestors are enabled, <see langword="false"/> otherwise.
        /// </summary>
        bool EnabledInHierarchy { get; }

        /// <summary>
        /// Returns the hash code of the fullname.
        /// </summary>
        int Hash { get; }

        /// <summary>
        /// Returns <see langword="true"/> if the current node is part of the provided world.
        /// </summary>
        /// <param name="world">The World.</param>
        /// <returns><see langword="true"/> if we should show the node for the current world; <see langword="false"/> otherwise.</returns>
        bool ShowForWorld(World world);

        /// <summary>
        /// Returns <see langword="true"/> if the current node is considered as running.
        /// </summary>
        bool IsRunning { get; }
    }
}
