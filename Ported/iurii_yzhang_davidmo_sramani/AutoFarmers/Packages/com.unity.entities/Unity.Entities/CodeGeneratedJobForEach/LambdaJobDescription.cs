using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities.CodeGeneratedJobForEach;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using AllowMultipleInvocationsAttribute = Unity.Entities.LambdaJobDescriptionConstructionMethods.AllowMultipleInvocationsAttribute;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public delegate System.Object Invalid_ForEach_Signature_See_ForEach_Documentation_For_Rules_And_Restrictions(System.Object o);

namespace Unity.Entities.CodeGeneratedJobForEach
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal class AllowDynamicValueAttribute : Attribute
    {
    }

    public interface ILambdaJobDescription
    {
    }
    
    public interface ISupportForEachWithUniversalDelegate
    {
    }
    
    public struct ForEachLambdaJobDescription : ILambdaJobDescription, ISupportForEachWithUniversalDelegate
    {
        //this overload exists here with the sole purpose of being able to give the user a not-totally-horrible
        //experience when they try to use an unsupported lambda signature. When this happens, the C# compiler
        //will go through its overload resolution, take the first candidate, and explain the user why the users
        //lambda is incompatible with that first candidates' parametertype.  We put this method here, instead
        //of with the other .ForEach overloads, to make sure this is the overload that the c# compiler will pick
        //when generating its compiler error.  If we didn't do that, it might pick a completely unrelated .ForEach
        //extention method, like the one for IJobChunk.
        //
        //The only communication channel we have to the user to guide them to figuring out what their problem is
        //is the name of the expected delegate type, as the c# compiler will put that in its compiler error message.
        //so we take this very unconventional approach here of encoding a message for the user in that type name,
        //that is easily googlable, so they will end up at a documentation page that describes why some lambda
        //signatures are compatible, and why some aren't, and what to do about that.
        //
        //the reason the delegate type is in the global namespace, is that it makes for a cleaner error message
        //it's marked with an attribute to prevent it from showing up in intellisense.
        public void ForEach(Invalid_ForEach_Signature_See_ForEach_Documentation_For_Rules_And_Restrictions ed)
        {
        }
    }

    public struct LambdaSingleJobDescription : ILambdaJobDescription
    {
    }

    public struct LambdaJobChunkDescription : ILambdaJobDescription
    {
    }
}

namespace Unity.Entities
{
    public static class LambdaJobDescriptionConstructionMethods
    {
        [AttributeUsage(AttributeTargets.Method)]
        internal class AllowMultipleInvocationsAttribute : Attribute
        {
        }

        public static TDescription WithoutBurst<TDescription>(this TDescription description) where TDescription : ILambdaJobDescription => description;
        
        [Obsolete("To turn off burst, please use .WithoutBurst() instead of .WithBurst(false)")]
        public static TDescription WithBurst<TDescription>(this TDescription description, bool enabled) where TDescription : ILambdaJobDescription => description;
        
        public static TDescription WithBurst<TDescription>(this TDescription description, FloatMode floatMode = FloatMode.Default, FloatPrecision floatPrecision = FloatPrecision.Standard, bool synchronousCompilation = false) where TDescription : ILambdaJobDescription => description;
        public static TDescription WithName<TDescription>(this TDescription description, string name) where TDescription : ILambdaJobDescription => description;
        
        [AllowMultipleInvocations]
        public static TDescription WithReadOnly<TDescription, TCapturedVariableType>(this TDescription description, [AllowDynamicValue] TCapturedVariableType capturedVariable) where TDescription : ILambdaJobDescription => description;
        [AllowMultipleInvocations]
        public static TDescription WithWriteOnly<TDescription, TCapturedVariableType>(this TDescription description, [AllowDynamicValue] TCapturedVariableType capturedVariable) where TDescription : ILambdaJobDescription => description;
        [AllowMultipleInvocations]
        public static TDescription WithDeallocateOnJobCompletion<TDescription, TCapturedVariableType>(this TDescription description, [AllowDynamicValue] TCapturedVariableType capturedVariable) where TDescription : ILambdaJobDescription => description;
        [AllowMultipleInvocations]
        public static TDescription WithNativeDisableContainerSafetyRestriction<TDescription, TCapturedVariableType>(this TDescription description, [AllowDynamicValue] TCapturedVariableType capturedVariable) where TDescription : ILambdaJobDescription => description;
        [AllowMultipleInvocations]
        public static unsafe TDescription WithNativeDisableUnsafePtrRestriction<TDescription, TCapturedVariableType>(this TDescription description, [AllowDynamicValue] TCapturedVariableType* capturedVariable) where TDescription : ILambdaJobDescription where TCapturedVariableType : unmanaged => description;
        [Obsolete("Use WithNativeDisableUnsafePtrRestriction instead", true)] //<-- remove soon, never shipped, only used in a2-dots-shooter
        public static TDescription WithNativeDisableUnsafePtrRestrictionAttribute<TDescription, TCapturedVariableType>(this TDescription description, [AllowDynamicValue] TCapturedVariableType capturedVariable) where TDescription : ILambdaJobDescription => description;
        [AllowMultipleInvocations]
        public static TDescription WithNativeDisableParallelForRestriction<TDescription, TCapturedVariableType>(this TDescription description, [AllowDynamicValue] TCapturedVariableType capturedVariable) where TDescription : ILambdaJobDescription => description;

        //do not remove this obsolete method. It is not really obsolete, it never existed, but it is created to give a better error message for when you try to use .Schedule() without argument.  Without this method signature,
        //c#'s overload resolution will try to match a completely different Schedule extension method, and explain why that one doesn't work, which results in an error message that sends the user in a wrong direction.
        [Obsolete("You must provide a JobHandle argument to .Schedule()", true)]
        public static JobHandle Schedule<TDescription>(this TDescription description) where TDescription : ILambdaJobDescription => ThrowCodeGenException();
        
        public static JobHandle Schedule<TDescription>(this TDescription description, [AllowDynamicValue] JobHandle dependency) where TDescription : ILambdaJobDescription => ThrowCodeGenException();
        
        public static void Run<TDescription>(this TDescription description) where TDescription : ILambdaJobDescription => ThrowCodeGenException();

        static JobHandle ThrowCodeGenException() => throw new Exception("This method should have been replaced by codegen");
    }

    public static class LambdaSimpleJobDescriptionConstructionMethods
    {
        public static LambdaSingleJobDescription WithCode(this LambdaSingleJobDescription description,  [AllowDynamicValue] Action code) =>description;
    }
    
    public static class LambdaJobChunkDescriptionConstructionMethods
    {
        public delegate void JobChunkDelegate(ArchetypeChunk chunk, int chunkIndex, int queryIndexOfFirstEntityInChunk);
        public static LambdaJobChunkDescription ForEach(this LambdaJobChunkDescription description,  [AllowDynamicValue] JobChunkDelegate code) =>description;
    }
    
    public static class LambdaJobChunkDescription_SetSharedComponent
    {
        public static LambdaJobChunkDescription SetSharedComponentFilterOnQuery<T>(LambdaJobChunkDescription description, T sharedComponent, EntityQuery query) where T : struct, ISharedComponentData
        {
            query.SetSharedComponentFilter(sharedComponent);
            return description;
        }
    }
    
    public static class ForEachLambdaJobDescription_SetSharedComponent
    {
        public static ForEachLambdaJobDescription SetSharedComponentFilterOnQuery<T>(ForEachLambdaJobDescription description, T sharedComponent, EntityQuery query) where T : struct, ISharedComponentData
        {
            query.SetSharedComponentFilter(sharedComponent);
            return description;
        }
    }
    
    public static class InternalCompilerInterface
    {
        public static JobRunWithoutJobSystemDelegate BurstCompile(JobRunWithoutJobSystemDelegate d) => 
#if !NET_DOTS
            BurstCompiler.CompileFunctionPointer(d).Invoke;
#else
            d;
#endif
        
        public static JobChunkRunWithoutJobSystemDelegate BurstCompile(JobChunkRunWithoutJobSystemDelegate d) =>
#if !NET_DOTS
            BurstCompiler.CompileFunctionPointer(d).Invoke;
#else
            d;
#endif
        
        
        public unsafe delegate void JobChunkRunWithoutJobSystemDelegate(ArchetypeChunkIterator* iterator, void* job);
        public unsafe delegate void JobRunWithoutJobSystemDelegate(void* job);
        
        public static unsafe void RunIJob<T>(ref T jobData, JobRunWithoutJobSystemDelegate functionPointer) where T : struct, IJob
        {
            functionPointer(UnsafeUtility.AddressOf(ref jobData));
        }
        
        public static unsafe void RunJobChunk<T>(ref T jobData, EntityQuery query, JobChunkRunWithoutJobSystemDelegate functionPointer) where T : struct, IJobChunk
        {
            var myIterator = query.GetArchetypeChunkIterator();

            #if ENABLE_UNITY_COLLECTIONS_CHECKS
            try
            {
                query.SafetyManager->IsInForEachDisallowStructuralChange++;
                functionPointer(&myIterator, UnsafeUtility.AddressOf(ref jobData));
            }
            finally
            {
                query.SafetyManager->IsInForEachDisallowStructuralChange--;
            }
            #else
            functionPointer(&myIterator, UnsafeUtility.AddressOf(ref jobData));
            #endif
        }
    }
}

public static partial class LambdaForEachDescriptionConstructionMethods
{
    static TDescription ThrowCodeGenException<TDescription>() => throw new Exception("This method should have been replaced by codegen");
}