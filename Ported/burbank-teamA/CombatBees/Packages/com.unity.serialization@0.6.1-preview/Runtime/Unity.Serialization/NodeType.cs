using System;

namespace Unity.Serialization
{
    /// <summary>
    /// Step instructions for the high-level reader API.
    ///
    /// This is used as input to control the parser.
    /// </summary>
    [Flags]
    public enum NodeType
    {
        /// <summary>
        /// Continue reading until there are no more characters.
        /// </summary>
        None               = 0,

        /// <summary>
        /// Start of an object.
        /// </summary>
        BeginObject        = 1 << 0,

        /// <summary>
        /// Start of a new member.
        /// </summary>
        ObjectKey          = 1 << 1,

        /// <summary>
        /// End of an object.
        /// </summary>
        EndObject          = 1 << 2,

        /// <summary>
        /// Start of an array/collection.
        /// </summary>
        BeginArray         = 1 << 3,

        /// <summary>
        /// End of an array/collection.
        /// </summary>
        EndArray           = 1 << 4,

        /// <summary>
        /// End of a string.
        /// </summary>
        String             = 1 << 5,

        /// <summary>
        /// End of a primitive (number, boolean, nan, etc.).
        /// </summary>
        Primitive          = 1 << 6,

        /// <summary>
        /// Any node type.
        /// </summary>
        Any                = ~0
    }
}
