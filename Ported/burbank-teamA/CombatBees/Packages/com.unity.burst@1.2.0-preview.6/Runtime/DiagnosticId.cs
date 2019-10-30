namespace Unity.Burst
{
    /// <summary>
    /// Specifies the possible diagnostic IDs.
    /// </summary>
#if BURST_INTERNAL
    public
#else
    internal
#endif
    enum DiagnosticId
    {
        // General
        ERR_InternalCompilerErrorInBackend = 100,
        ERR_InternalCompilerErrorInFunction = 101,
        ERR_InternalCompilerErrorInInstruction = 102,

        // ILBuilder
        ERR_OnlyStaticMethodsAllowed = 1000,
        ERR_UnableToAccessManagedMethod = 1001,
        ERR_UnableToFindInterfaceMethod = 1002,

        // ILCompiler
        ERR_UnexpectedEmptyMethodBody = 1003,
        ERR_ManagedArgumentsNotSupported = 1004,
        ERR_TryConstructionNotSupported = 1005,
        ERR_CatchConstructionNotSupported = 1006,
        ERR_CatchAndFilterConstructionNotSupported = 1007,
        ERR_LdfldaWithFixedArrayExpected = 1008,
        ERR_PointerExpected = 1009,
        ERR_LoadingFieldFromManagedObjectNotSupported = 1010,
        ERR_LoadingFieldWithManagedTypeNotSupported = 1011,
        ERR_LoadingArgumentWithManagedTypeNotSupported = 1012,
        ERR_CallingBurstDiscardMethodWithReturnValueNotSupported = 1015,
        ERR_CallingManagedMethodNotSupported = 1016,
        ERR_BinaryPointerOperationNotSupported = 1017,
        ERR_AddingPointersWithNonPointerResultNotSupported = 1018,
        ERR_InstructionUnboxNotSupported = 1019,
        ERR_InstructionBoxNotSupported = 1020,
        ERR_InstructionNewobjWithManagedTypeNotSupported = 1021,
        ERR_AccessingManagedArrayNotSupported = 1022,
        ERR_InstructionLdtokenFieldNotSupported = 1023,
        ERR_InstructionLdtokenMethodNotSupported = 1024,
        ERR_InstructionLdtokenTypeNotSupported = 1025,
        ERR_InstructionLdtokenNotSupported = 1026,
        ERR_InstructionLdvirtftnNotSupported = 1027,
        ERR_InstructionNewarrNotSupported = 1028,
        ERR_InstructionRethrowNotSupported = 1029,
        ERR_InstructionCastclassNotSupported = 1030,
        ERR_InstructionIsinstNotSupported = 1031,
        ERR_InstructionLdftnNotSupported = 1032,
        ERR_InstructionLdstrNotSupported = 1033,
        ERR_InstructionStsfldNotSupported = 1034,
        ERR_InstructionEndfilterNotSupported = 1035,
        ERR_InstructionEndfinallyNotSupported = 1036,
        ERR_InstructionLeaveNotSupported = 1037,
        ERR_InstructionNotSupported = 1038,
        ERR_LoadingFromStaticFieldNotSupported = 1039,
        ERR_LoadingFromNonReadonlyStaticFieldNotSupported = 1040,
        ERR_LoadingFromManagedStaticFieldNotSupported = 1041,
        ERR_LoadingFromManagedNonReadonlyStaticFieldNotSupported = 1042,
        ERR_InstructionStfldToManagedObjectNotSupported = 1043,
        ERR_InstructionLdlenNonConstantLengthNotSupported = 1044,
        ERR_StructWithAutoLayoutNotSupported = 1045,
        ERR_StructWithPackNotSupported = 1046,
        ERR_StructWithGenericParametersAndExplicitLayoutNotSupported = 1047,
        ERR_StructSizeNotSupported = 1048,
        ERR_StructZeroSizeNotSupported = 1049,
        ERR_MarshalAsOnFieldNotSupported = 1050,
        ERR_TypeNotSupported = 1051,
        ERR_RequiredTypeModifierNotSupported = 1052,
        ERR_ErrorWhileProcessingVariable = 1053,

        // CecilExtensions
        ERR_UnableToResolveType = 1054,

        // ILFunctionReference
        ERR_UnableToResolveMethod = 1055,
        ERR_ConstructorNotSupported = 1056,
        ERR_FunctionPointerMethodMissingBurstCompileAttribute = 1057,
        ERR_FunctionPointerTypeMissingBurstCompileAttribute = 1058,
        ERR_FunctionPointerMethodAndTypeMissingBurstCompileAttribute = 1059,

        // ILVisitor
        ERR_EntryPointFunctionCannotBeCalledInternally = 1060,

        // ExternalFunctionParameterChecks
        ERR_MarshalAsOnParameterNotSupported = 1061,
        ERR_MarshalAsOnReturnTypeNotSupported = 1062,
        ERR_TypeNotBlittableForFunctionPointer = 1063,
        ERR_StructByValueNotSupported = 1064,
        ERR_StructsWithNonUnicodeCharsNotSupported = 1066,
        ERR_VectorsByValueNotSupported = 1067,

        // JitCompiler
        ERR_MissingExternBindings = 1068,

        // AssertProcessor
        ERR_AssertTypeNotSupported = 1071,

        // ReadOnlyProcessor
        ERR_StoringToReadOnlyFieldNotAllowed = 1072,
        ERR_StoringToFieldInReadOnlyParameterNotAllowed = 1073,
        ERR_StoringToReadOnlyParameterNotAllowed = 1074,

        // TypeManagerProcessor
        ERR_TypeManagerStaticFieldNotCompatible = 1075,
        ERR_UnableToFindTypeIndexForTypeManagerType = 1076,
        ERR_UnableToFindFieldForTypeManager = 1077,

        // NoAliasAnalyzer
        WRN_DisablingNoaliasUnsupportedLdobjImplicitNativeContainer = 1078,
        WRN_DisablingNoaliasLoadingDirectlyFromFieldOfNativeArray = 1079,
        WRN_DisablingNoaliasWritingDirectlyToFieldOfNativeArray = 1080,
        WRN_DisablingNoaliasStoringImplicitNativeContainerToField = 1081,
        WRN_DisablingNoaliasStoringImplicitNativeContainerToLocalVariable = 1082,
        WRN_DisablingNoaliasStoringImplicitNativeContainerToPointer = 1083,
        WRN_DisablingNoaliasCannotLoadNativeContainerAsBothArgumentAndField = 1084,
        WRN_DisablingNoaliasSameArgumentPath = 1085,
        WRN_DisablingNoaliasCannotPassMultipleNativeContainersConcurrently = 1086,
        WRN_DisablingNoaliasUnsupportedNativeArrayUnsafeUtilityMethod = 1087,
        WRN_DisablingNoaliasUnsupportedNativeArrayMethod = 1088,
        WRN_DisablingNoaliasUnsupportedThisArgument = 1089,

        // StaticFieldAccessTransform
        ERR_CircularStaticConstructorUsage = 1090,
        ERR_ExternalInternalCallsInStaticConstructorsNotSupported = 1091,

        // AotCompiler
        ERR_PlatformNotSupported = 1092,
        ERR_InitModuleVerificationError = 1093,

        // NativeCompiler
        ERR_ModuleVerificationError = 1094,

        // TypeManagerProcessor
        ERR_UnableToFindTypeRequiredForTypeManager = 1095,

        // ILBuilder
        ERR_UnexpectedIntegerTypesForBinaryOperation = 1096,
        ERR_BinaryOperationNotSupported = 1097,
        ERR_CalliWithThisNotSupported = 1098,
        ERR_CalliNonCCallingConventionNotSupported = 1099,
    }
}