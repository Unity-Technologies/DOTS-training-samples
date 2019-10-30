using UnityEditor;

namespace Unity.Build.Common
{
    [BuildStep(description = k_Description, category = "Classic")]
    public sealed class BuildStepBuildClassicLiveLink : BuildStep
    {
        const string k_Description = "Build LiveLink Player";

        TemporaryFileTracker m_TemporaryFileTracker;

        public override string Description => k_Description;

        public override BuildStepResult RunStep(BuildContext context)
        {
            m_TemporaryFileTracker = new TemporaryFileTracker();
            if (!BuildStepBuildClassicPlayer.Prepare(context, this, true, m_TemporaryFileTracker, out var failure, out var options))
                return failure;

            //@TODO: Allow debugging should  be based on profile...
            //@TODO: BuildOptions.AutoRunPlayer shouldn't be enabled, it is just a workaround until Run is properly supported
            options.options = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectToHost | BuildOptions.AutoRunPlayer;

            var report = UnityEditor.BuildPipeline.BuildPlayer(options);
            return BuildStepBuildClassicPlayer.ReturnBuildPlayerResult(context, this, report);
        }

        public override BuildStepResult CleanupStep(BuildContext context)
        {
            m_TemporaryFileTracker.Dispose();
            return Success();
        }
    }
}