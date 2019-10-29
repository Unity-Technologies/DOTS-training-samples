using System;
using Unity.Entities.CodeGeneratedJobForEach;
using static Unity.Entities.LambdaJobDescriptionConstructionMethods;

namespace Unity.Entities
{
    public static class LambdaJobQueryConstructionMethods
    {
        //Start of query creating functions for ForEachLambdaJobDescription.  Unfortunately there's no C# way to use generics to make these work for multiple jobdescription types, so we're lowteching it with t4 here.
        [AllowMultipleInvocationsAttribute]
        public static ForEachLambdaJobDescription WithNone<T>(this ForEachLambdaJobDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static ForEachLambdaJobDescription WithNone<T1,T2>(this ForEachLambdaJobDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static ForEachLambdaJobDescription WithNone<T1,T2,T3>(this ForEachLambdaJobDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static ForEachLambdaJobDescription WithAny<T>(this ForEachLambdaJobDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static ForEachLambdaJobDescription WithAny<T1,T2>(this ForEachLambdaJobDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static ForEachLambdaJobDescription WithAny<T1,T2,T3>(this ForEachLambdaJobDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static ForEachLambdaJobDescription WithAll<T>(this ForEachLambdaJobDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static ForEachLambdaJobDescription WithAll<T1,T2>(this ForEachLambdaJobDescription description)  => description;
        [AllowMultipleInvocationsAttribute]
        public static ForEachLambdaJobDescription WithAll<T1,T2,T3>(this ForEachLambdaJobDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static ForEachLambdaJobDescription WithChangeFilter<T>(this ForEachLambdaJobDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static ForEachLambdaJobDescription WithChangeFilter<T1,T2>(this ForEachLambdaJobDescription description) => description;
        
        public static ForEachLambdaJobDescription WithEntityQueryOptions(this ForEachLambdaJobDescription description, EntityQueryOptions options) => description;
        public static ForEachLambdaJobDescription WithSharedComponentFilter<T>(this ForEachLambdaJobDescription description, [AllowDynamicValue] T sharedComponent) where T : struct, ISharedComponentData => description;        
        //Start of query creating functions for LambdaJobChunkDescription.  Unfortunately there's no C# way to use generics to make these work for multiple jobdescription types, so we're lowteching it with t4 here.
        [AllowMultipleInvocationsAttribute]
        public static LambdaJobChunkDescription WithNone<T>(this LambdaJobChunkDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static LambdaJobChunkDescription WithNone<T1,T2>(this LambdaJobChunkDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static LambdaJobChunkDescription WithNone<T1,T2,T3>(this LambdaJobChunkDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static LambdaJobChunkDescription WithAny<T>(this LambdaJobChunkDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static LambdaJobChunkDescription WithAny<T1,T2>(this LambdaJobChunkDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static LambdaJobChunkDescription WithAny<T1,T2,T3>(this LambdaJobChunkDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static LambdaJobChunkDescription WithAll<T>(this LambdaJobChunkDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static LambdaJobChunkDescription WithAll<T1,T2>(this LambdaJobChunkDescription description)  => description;
        [AllowMultipleInvocationsAttribute]
        public static LambdaJobChunkDescription WithAll<T1,T2,T3>(this LambdaJobChunkDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static LambdaJobChunkDescription WithChangeFilter<T>(this LambdaJobChunkDescription description) => description;
        [AllowMultipleInvocationsAttribute]
        public static LambdaJobChunkDescription WithChangeFilter<T1,T2>(this LambdaJobChunkDescription description) => description;
        
        public static LambdaJobChunkDescription WithEntityQueryOptions(this LambdaJobChunkDescription description, EntityQueryOptions options) => description;
        public static LambdaJobChunkDescription WithSharedComponentFilter<T>(this LambdaJobChunkDescription description, [AllowDynamicValue] T sharedComponent) where T : struct, ISharedComponentData => description;        
   }    
}
