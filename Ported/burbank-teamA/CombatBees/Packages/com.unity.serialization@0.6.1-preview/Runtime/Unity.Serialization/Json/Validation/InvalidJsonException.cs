using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Unity.Serialization.Json
{
    /// <summary>
    /// The exception that is thrown when json input is invalid.
    /// </summary>
    [Serializable]
    public class InvalidJsonException : Exception
    {
        /// <summary>
        /// The line the validator stopped at.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// The character the validator stopped at.
        /// </summary>
        public int Character { get; set; }

        /// <summary>
        /// Initialized a new instance of the <see cref="InvalidJsonException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidJsonException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidJsonException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception reference.</param>
        public InvalidJsonException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidJsonException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected InvalidJsonException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Line = info.GetInt32(nameof(Line));
            Character = info.GetInt32(nameof(Character));
        }

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is a null reference</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(Line), Line);
            info.AddValue(nameof(Character), Character);
            base.GetObjectData(info, context);
        }
    }
}
