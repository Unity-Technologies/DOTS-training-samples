namespace Unity.Platforms
{
    public class RunLoop
    {
        public delegate bool RunLoopDelegate();
        public static void EnterMainLoop(RunLoopDelegate runLoopDelegate)
        {
            RunLoopImpl.EnterMainLoop(runLoopDelegate);
        }
    }
}
