using Unity.Jobs;

namespace Unity.Serialization.Json
{
    /// <summary>
    /// Pass through validation.
    /// </summary>
    class JsonNoneValidator : IJsonValidator
    {
        public void Dispose()
        {
        }

        public void Initialize()
        {
        }

        public JsonValidationResult GetResult()
        {
            return new JsonValidationResult();
        }

        public JobHandle ValidateAsync(UnsafeBuffer<char> buffer, int start, int count)
        {
            return default;
        }
    }
}
