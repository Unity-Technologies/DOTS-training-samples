using System;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Serialization.Json
{
    /// <summary>
    /// @TODO
    /// </summary>
    class JsonSimpleValidator : IJsonValidator, IDisposable
    {
        public JsonSimpleValidator(Allocator label = SerializationConfiguration.DefaultAllocatorLabel)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public JsonValidationResult GetResult()
        {
            throw new NotImplementedException();
        }

        public JobHandle ValidateAsync(UnsafeBuffer<char> buffer, int start, int count)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
