using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Burst.Benchmarks")]

namespace UnityBenchShared
{
    internal interface IArgumentProvider
    {
        object Value { get; }
    }
}
