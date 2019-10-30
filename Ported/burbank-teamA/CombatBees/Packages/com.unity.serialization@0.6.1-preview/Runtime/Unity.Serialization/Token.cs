namespace Unity.Serialization
{
    /// <summary>
    /// The <see cref="TokenType"/> is used to describe the high level structure of a data tree.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Unknown token type. Usually this means the token is not initialized.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The token holds a reference to all characters between object characters '{'..'}'.
        ///
        /// @NOTE This includes the "begin" and "end" object characters.
        /// </summary>
        Object,

        /// <summary>
        /// The token holds a reference to all characters between array characters '['..']'.
        ///
        /// @NOTE This includes the "begin" and "end" array characters.
        /// </summary>
        Array,

        /// <summary>
        /// The token holds a reference to all characters between string characters '"'..'"'.
        ///
        /// @NOTE This includes the "begin" and "end" double quote characters.
        /// </summary>
        String,

        /// <summary>
        /// Holds a reference to characters that represent any value that does not fit into the above categories.
        /// </summary>
        Primitive,
    }

    struct Token
    {
        /// <summary>
        /// The token type.
        /// </summary>
        public TokenType Type;

        /// <summary>
        /// The parent token. This can be an object, array, member, or part of a split token.
        /// </summary>
        public int Parent;

        /// <summary>
        /// The start position in the original input data.
        /// </summary>
        public int Start;

        /// <summary>
        /// The end position in the original input data.
        ///
        /// This points to the character after the data.
        /// </summary>
        public int End;

        public override string ToString()
        {
            return $"Type=[{Type}] Range=[{Start}..{End}] Parent=[{Parent}]";
        }
    }
}
