namespace UnityEditor.Build.Pipeline
{
    /// <summary>
    /// Return codes for the scriptable build pipeline. Int values of these return codes are standardized to 0 or greater for Success and -1 or less for Error.
    /// </summary>
    public enum ReturnCode
    {
        // Success Codes are Positive!
        Success = 0,
        SuccessCached = 1,
        SuccessNotRun = 2,
        // Error Codes are Negative!
        Error = -1,
        Exception = -2,
        Canceled = -3,
        UnsavedChanges = -4,
        MissingRequiredObjects = -5
    }
}
