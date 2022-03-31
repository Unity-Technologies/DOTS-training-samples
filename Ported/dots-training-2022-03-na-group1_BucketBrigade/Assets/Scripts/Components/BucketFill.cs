using Unity.Entities;

namespace Components
{
    /// <summary>
    ///     Holds the cooldown time for a bot filling a bucket.
    /// </summary>
    struct BucketFill : IComponentData
    {
        /// <summary>
        ///     The time the filling process is finished.
        /// </summary>
        public float cooldown;
    }
}
