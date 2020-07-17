#include "pch-cpp.hpp"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <limits>
#include <stdint.h>


template <typename R>
struct VirtFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4>
struct VirtActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4>
struct GenericVirtActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename R>
struct GenericVirtFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename R>
struct InterfaceFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
struct InterfaceActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4>
struct InterfaceActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4>
struct GenericInterfaceActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename R>
struct GenericInterfaceFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};

// Unity.Burst.CompilerServices.AssumeRangeAttribute
struct AssumeRangeAttribute_t9CB4051727958D8EC81907FC2D1E8A4672DBA1B1;
// System.Reflection.Binder
struct Binder_t2BEE27FD84737D1E79BC47FD67F6D3DD2F2DDA30;
// Unity.Burst.BurstCompileAttribute
struct BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1;
// Unity.Burst.BurstCompilerOptions
struct BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34;
// System.Globalization.CodePageDataItem
struct CodePageDataItem_t09A62F57142BF0456C8F414898A37E79BCC9F09E;
// System.Text.DecoderFallback
struct DecoderFallback_tF86D337D6576E81E5DA285E5673183EBC66DEF8D;
// System.DelegateData
struct DelegateData_t17DD30660E330C49381DAA99F934BE75CB11F288;
// System.Text.EncoderFallback
struct EncoderFallback_t02AC990075E17EB09F0D7E4831C3B3F264025CC4;
// System.Collections.Hashtable
struct Hashtable_t7565AB92A12227AD5BADD6911F10D87EE52509AC;
// System.Collections.IDictionary
struct IDictionary_t99871C56B8EC2452AC5C4CF3831695E617B89D3A;
// System.Reflection.MemberFilter
struct MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// Unity.Burst.NoAliasAttribute
struct NoAliasAttribute_t444822FD22BC82F0E365DB1CEDC4AE7FC5E7DE13;
// System.Runtime.Serialization.SafeSerializationManager
struct SafeSerializationManager_tDE44F029589A028F8A3053C5C06153FAB4AAE29F;
// System.Void
struct Void_t700C6383A2A510C2CF4DD86DABD5CA9FF70ADAC5;
// Unity.Burst.BurstRuntime/NativeLogDelegate
struct NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182;
// Unity.Burst.BurstCompiler/BurstCompilerHelper/IsBurstEnabledDelegate
struct IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5;
// System.Collections.Generic.Dictionary`2<System.Int64,System.Object>
struct Dictionary_2_t240BB5F785CC3B2A17B14447F3C0E0BB6AAB8E26;
// System.Collections.Generic.Dictionary`2<System.Int64,System.Type>
struct Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D;
// System.Collections.Generic.IEnumerable`1<System.Attribute>
struct IEnumerable_1_t95FA6F2545E4B529565364A2F48515FE4B80AD31;
// System.Collections.Generic.IEqualityComparer`1<System.Int64>
struct IEqualityComparer_1_tBD7EB381E8B25356EF3AED6C41B65AECA6B91A19;
// System.Collections.Generic.Dictionary`2/KeyCollection<System.Int64,System.Type>
struct KeyCollection_t3F56F6EEC94E86F1DF2C49F41FBDEE053550D90A;
// System.Collections.Generic.Dictionary`2/ValueCollection<System.Int64,System.Type>
struct ValueCollection_t6AA94407EBA83DBB24A4BB8E756BFCECBC80A70C;
// System.Action
struct Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6;
// System.ArgumentException
struct ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00;
// System.ArgumentNullException
struct ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB;
// System.AsyncCallback
struct AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA;
// System.Attribute
struct Attribute_t037CA9D9F3B742C063DB364D2EEBBF9FC5772C71;
// System.Byte[]
struct ByteU5BU5D_tDBBEB0E8362242FA7223000D978B0DD19D4B0726;
// System.Char[]
struct CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34;
// System.Delegate
struct Delegate_t;
// System.Delegate[]
struct DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8;
// System.Text.Encoding
struct Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827;
// System.Collections.Generic.Dictionary`2/Entry<System.Int64,System.Type>[]
struct EntryU5BU5D_t09835765F8D6593FB6AEFA6582E0E23F2B6C871E;
// System.IAsyncResult
struct IAsyncResult_tC9F97BF36FCF122D29D3101D80642278297BF370;
// System.Int32[]
struct Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32;
// System.IntPtr[]
struct IntPtrU5BU5D_t27FC72B0409D75AAF33EC42498E8094E95FEE9A6;
// System.InvalidOperationException
struct InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB;
// System.Reflection.MemberInfo
struct MemberInfo_t;
// System.Diagnostics.StackTrace[]
struct StackTraceU5BU5D_t4AD999C288CB6D1F38A299D12B1598D606588971;
// System.String
struct String_t;
// System.String[]
struct StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A;
// System.Type
struct Type_t;
// System.Type[]
struct TypeU5BU5D_t85B10489E46F06CEC7C4B1CCBD0E01FAB6649755;

IL2CPP_EXTERN_C RuntimeClass* ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* BurstCompilerHelper_tBFAE069B1EED68EFA83AF35A2224700B2BDCB4FA_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* BurstRuntime_t725AE7FF2003494CEB5F987F3365BA847C762735_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Debug_tEB68BCBEB8EFD60F8043C67146DC05E7F50F374B_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IDisposable_t099785737FC6A1E3699919A94109383715A8D807_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEnumerable_1_t95FA6F2545E4B529565364A2F48515FE4B80AD31_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEnumerator_1_tD213EBCE7B3BCD6C10AD4828845AF17048A1D1B7_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEnumerator_t5956F3AFB7ECF1117E3BC5890E7FC7B7F7A04105_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* LogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Marshal_tEBAFAE20369FCB1B38C49C4E27A8D8C2C4B55058_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Path_tF1D95B78D57C1C1211BA6633FF2AC22FD6C48921_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* RuntimeObject_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SharedStatic_t4A478B3109091C26ED6F772B9BEC1EA0802B2FFC_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* String_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral02FEF73D0CEEEFD67DF0C54AE6860E3A4B589F86;
IL2CPP_EXTERN_C String_t* _stringLiteral052A90A585030A2D7669CC7D2884B0D78760C742;
IL2CPP_EXTERN_C String_t* _stringLiteral4784CA4E159D86F8767CCC55143536AF34801074;
IL2CPP_EXTERN_C String_t* _stringLiteral6B0170CB33499190716007E7ADF488D97D050004;
IL2CPP_EXTERN_C String_t* _stringLiteral79DA35A084D67D3A4C946D97765E49E456B15354;
IL2CPP_EXTERN_C String_t* _stringLiteral8C01367F6B784D1ABC182A2E9F6A403A0506773E;
IL2CPP_EXTERN_C String_t* _stringLiteral99558DD5BF091125B80584111852E84E79679E91;
IL2CPP_EXTERN_C String_t* _stringLiteralBEE03BDA1448C10795E764222E36CC6226EECA1A;
IL2CPP_EXTERN_C String_t* _stringLiteralCE4CE926FE1AADB8C64C184D24F6BBABF78EAFC1;
IL2CPP_EXTERN_C String_t* _stringLiteralE8A25E76E3168AD5402ECA0ECD4590B455D50C61;
IL2CPP_EXTERN_C const RuntimeMethod* BurstCompilerHelper_IsBurstEnabled_mC1B24958FFA3F3D5360E1F933CE0900E586B2A5D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* BurstCompilerOptions_HasBurstCompileAttribute_m4EF62614BD8EAAF2015D0B19230E79523213FC9D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* BurstRuntime_ManagedNativeLogImpl_m1F8647DF0D27A6D58B855F3341D46D664CFF0D7F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* CustomAttributeExtensions_GetCustomAttribute_TisBurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1_m9B8B3869855183A66586F91FE26BFB947E0A919F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2__ctor_mEDDD9C0EDBF84B20D8EEC0295487B7446286353C_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FunctionPointer_1__ctor_m56A4237238EC535C80FC92413132650C96CA61A0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FunctionPointer_1_get_Invoke_m659D9685A98FA383A19E4C692F098FBCD3564904_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Marshal_GetFunctionPointerForDelegate_TisNativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182_mDAD2FE8DBFF51AB6A1CC3027EE51628264E1EAEB_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SharedStatic_1_GetOrCreate_TisLogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_m7F86F658D4E8546206B6F6A9256A36C117205E06_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SharedStatic_1_get_Data_m871D2E22DD229D10E8D37902879F825ED4A4CD3F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SharedStatic_GetOrCreateSharedStaticInternal_m1FDD96EE55D49263DA5EA34EB6502507F1EAD254_RuntimeMethod_var;
IL2CPP_EXTERN_C const uint32_t BurstCompilerHelper_IsBurstEnabled_mC1B24958FFA3F3D5360E1F933CE0900E586B2A5D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t BurstCompilerHelper_IsCompiledByBurst_m8CC7FEC39A7D2459CD8AAA3DC71AC04ACEC7BFFC_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t BurstCompilerHelper__cctor_mD76FFC76096643AA6180877EE5FCCC86CD37E02B_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t BurstCompilerOptions_GetBurstCompileAttribute_mF55455921BF75D6447C87EB4F5B3E8CA69DC47A6_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t BurstCompilerOptions_HasBurstCompileAttribute_m4EF62614BD8EAAF2015D0B19230E79523213FC9D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t BurstCompilerOptions_TryGetAttribute_m515A5B89905066961F2AF4D93D37189F97CB7886_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t BurstCompilerOptions__cctor_m998B13B9D2A1813E93900FFAE79A23357CB611A6_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t BurstCompilerOptions_set_EnableBurstCompilation_m2C0458F30D5BAF07E4BACA9835A6AADC0ECB90BF_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t BurstCompiler__cctor_m6482E2EA0A198C07EB665E4A7A1E0923257D4FD8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t BurstCompiler_get_Options_mA554F95C4DEC38EFEAAD61E81FC2A90740CA8899_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t BurstRuntime_GetHashCode32_m4B9A69F912BBF47693AF4C21D45E66A27D550A32_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t BurstRuntime_GetHashCode64_m18D20B8481BF87E670E8CB864EA0CA5111B8488E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t BurstRuntime_Log_mA326750916D8B27060FFE335CFE3BB74F12E8632_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t BurstRuntime_ManagedNativeLogImpl_m1F8647DF0D27A6D58B855F3341D46D664CFF0D7F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t BurstRuntime__cctor_mD32471BFBA84067CCC8708B7591D6B3028D6CC30_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t LogHelper__cctor_mA6709BC920D30A0816AB8B35E7420B14A320DDE0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NativeLogDelegate_BeginInvoke_m6CF8743C2116BE0A550203DFFF3594BE12FB2A28_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SharedStatic_GetOrCreateSharedStaticInternal_m1FDD96EE55D49263DA5EA34EB6502507F1EAD254_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SharedStatic__cctor_m91BEC2BB24D9ABA5B5998E30913C1C0DE51E9E36_MetadataUsageId;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;

struct DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8;
struct StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// <Module>
struct  U3CModuleU3E_tCD5BF6D3AAB147F28F15DED35776F8117AE07F16 
{
public:

public:
};


// System.Object

struct Il2CppArrayBounds;

// System.Array


// System.Attribute
struct  Attribute_t037CA9D9F3B742C063DB364D2EEBBF9FC5772C71  : public RuntimeObject
{
public:

public:
};


// System.Collections.Generic.Dictionary`2<System.Int64,System.Type>
struct  Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D  : public RuntimeObject
{
public:
	// System.Int32[] System.Collections.Generic.Dictionary`2::buckets
	Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* ___buckets_0;
	// System.Collections.Generic.Dictionary`2_Entry<TKey,TValue>[] System.Collections.Generic.Dictionary`2::entries
	EntryU5BU5D_t09835765F8D6593FB6AEFA6582E0E23F2B6C871E* ___entries_1;
	// System.Int32 System.Collections.Generic.Dictionary`2::count
	int32_t ___count_2;
	// System.Int32 System.Collections.Generic.Dictionary`2::version
	int32_t ___version_3;
	// System.Int32 System.Collections.Generic.Dictionary`2::freeList
	int32_t ___freeList_4;
	// System.Int32 System.Collections.Generic.Dictionary`2::freeCount
	int32_t ___freeCount_5;
	// System.Collections.Generic.IEqualityComparer`1<TKey> System.Collections.Generic.Dictionary`2::comparer
	RuntimeObject* ___comparer_6;
	// System.Collections.Generic.Dictionary`2_KeyCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::keys
	KeyCollection_t3F56F6EEC94E86F1DF2C49F41FBDEE053550D90A * ___keys_7;
	// System.Collections.Generic.Dictionary`2_ValueCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::values
	ValueCollection_t6AA94407EBA83DBB24A4BB8E756BFCECBC80A70C * ___values_8;
	// System.Object System.Collections.Generic.Dictionary`2::_syncRoot
	RuntimeObject * ____syncRoot_9;

public:
	inline static int32_t get_offset_of_buckets_0() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___buckets_0)); }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* get_buckets_0() const { return ___buckets_0; }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32** get_address_of_buckets_0() { return &___buckets_0; }
	inline void set_buckets_0(Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* value)
	{
		___buckets_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___buckets_0), (void*)value);
	}

	inline static int32_t get_offset_of_entries_1() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___entries_1)); }
	inline EntryU5BU5D_t09835765F8D6593FB6AEFA6582E0E23F2B6C871E* get_entries_1() const { return ___entries_1; }
	inline EntryU5BU5D_t09835765F8D6593FB6AEFA6582E0E23F2B6C871E** get_address_of_entries_1() { return &___entries_1; }
	inline void set_entries_1(EntryU5BU5D_t09835765F8D6593FB6AEFA6582E0E23F2B6C871E* value)
	{
		___entries_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___entries_1), (void*)value);
	}

	inline static int32_t get_offset_of_count_2() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___count_2)); }
	inline int32_t get_count_2() const { return ___count_2; }
	inline int32_t* get_address_of_count_2() { return &___count_2; }
	inline void set_count_2(int32_t value)
	{
		___count_2 = value;
	}

	inline static int32_t get_offset_of_version_3() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___version_3)); }
	inline int32_t get_version_3() const { return ___version_3; }
	inline int32_t* get_address_of_version_3() { return &___version_3; }
	inline void set_version_3(int32_t value)
	{
		___version_3 = value;
	}

	inline static int32_t get_offset_of_freeList_4() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___freeList_4)); }
	inline int32_t get_freeList_4() const { return ___freeList_4; }
	inline int32_t* get_address_of_freeList_4() { return &___freeList_4; }
	inline void set_freeList_4(int32_t value)
	{
		___freeList_4 = value;
	}

	inline static int32_t get_offset_of_freeCount_5() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___freeCount_5)); }
	inline int32_t get_freeCount_5() const { return ___freeCount_5; }
	inline int32_t* get_address_of_freeCount_5() { return &___freeCount_5; }
	inline void set_freeCount_5(int32_t value)
	{
		___freeCount_5 = value;
	}

	inline static int32_t get_offset_of_comparer_6() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___comparer_6)); }
	inline RuntimeObject* get_comparer_6() const { return ___comparer_6; }
	inline RuntimeObject** get_address_of_comparer_6() { return &___comparer_6; }
	inline void set_comparer_6(RuntimeObject* value)
	{
		___comparer_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___comparer_6), (void*)value);
	}

	inline static int32_t get_offset_of_keys_7() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___keys_7)); }
	inline KeyCollection_t3F56F6EEC94E86F1DF2C49F41FBDEE053550D90A * get_keys_7() const { return ___keys_7; }
	inline KeyCollection_t3F56F6EEC94E86F1DF2C49F41FBDEE053550D90A ** get_address_of_keys_7() { return &___keys_7; }
	inline void set_keys_7(KeyCollection_t3F56F6EEC94E86F1DF2C49F41FBDEE053550D90A * value)
	{
		___keys_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___keys_7), (void*)value);
	}

	inline static int32_t get_offset_of_values_8() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___values_8)); }
	inline ValueCollection_t6AA94407EBA83DBB24A4BB8E756BFCECBC80A70C * get_values_8() const { return ___values_8; }
	inline ValueCollection_t6AA94407EBA83DBB24A4BB8E756BFCECBC80A70C ** get_address_of_values_8() { return &___values_8; }
	inline void set_values_8(ValueCollection_t6AA94407EBA83DBB24A4BB8E756BFCECBC80A70C * value)
	{
		___values_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___values_8), (void*)value);
	}

	inline static int32_t get_offset_of__syncRoot_9() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ____syncRoot_9)); }
	inline RuntimeObject * get__syncRoot_9() const { return ____syncRoot_9; }
	inline RuntimeObject ** get_address_of__syncRoot_9() { return &____syncRoot_9; }
	inline void set__syncRoot_9(RuntimeObject * value)
	{
		____syncRoot_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_9), (void*)value);
	}
};


// System.Reflection.MemberInfo
struct  MemberInfo_t  : public RuntimeObject
{
public:

public:
};


// System.String
struct  String_t  : public RuntimeObject
{
public:
	// System.Int32 System.String::m_stringLength
	int32_t ___m_stringLength_0;
	// System.Char System.String::m_firstChar
	Il2CppChar ___m_firstChar_1;

public:
	inline static int32_t get_offset_of_m_stringLength_0() { return static_cast<int32_t>(offsetof(String_t, ___m_stringLength_0)); }
	inline int32_t get_m_stringLength_0() const { return ___m_stringLength_0; }
	inline int32_t* get_address_of_m_stringLength_0() { return &___m_stringLength_0; }
	inline void set_m_stringLength_0(int32_t value)
	{
		___m_stringLength_0 = value;
	}

	inline static int32_t get_offset_of_m_firstChar_1() { return static_cast<int32_t>(offsetof(String_t, ___m_firstChar_1)); }
	inline Il2CppChar get_m_firstChar_1() const { return ___m_firstChar_1; }
	inline Il2CppChar* get_address_of_m_firstChar_1() { return &___m_firstChar_1; }
	inline void set_m_firstChar_1(Il2CppChar value)
	{
		___m_firstChar_1 = value;
	}
};

struct String_t_StaticFields
{
public:
	// System.String System.String::Empty
	String_t* ___Empty_5;

public:
	inline static int32_t get_offset_of_Empty_5() { return static_cast<int32_t>(offsetof(String_t_StaticFields, ___Empty_5)); }
	inline String_t* get_Empty_5() const { return ___Empty_5; }
	inline String_t** get_address_of_Empty_5() { return &___Empty_5; }
	inline void set_Empty_5(String_t* value)
	{
		___Empty_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Empty_5), (void*)value);
	}
};


// System.Text.Encoding
struct  Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827  : public RuntimeObject
{
public:
	// System.Int32 System.Text.Encoding::m_codePage
	int32_t ___m_codePage_9;
	// System.Globalization.CodePageDataItem System.Text.Encoding::dataItem
	CodePageDataItem_t09A62F57142BF0456C8F414898A37E79BCC9F09E * ___dataItem_10;
	// System.Boolean System.Text.Encoding::m_deserializedFromEverett
	bool ___m_deserializedFromEverett_11;
	// System.Boolean System.Text.Encoding::m_isReadOnly
	bool ___m_isReadOnly_12;
	// System.Text.EncoderFallback System.Text.Encoding::encoderFallback
	EncoderFallback_t02AC990075E17EB09F0D7E4831C3B3F264025CC4 * ___encoderFallback_13;
	// System.Text.DecoderFallback System.Text.Encoding::decoderFallback
	DecoderFallback_tF86D337D6576E81E5DA285E5673183EBC66DEF8D * ___decoderFallback_14;

public:
	inline static int32_t get_offset_of_m_codePage_9() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827, ___m_codePage_9)); }
	inline int32_t get_m_codePage_9() const { return ___m_codePage_9; }
	inline int32_t* get_address_of_m_codePage_9() { return &___m_codePage_9; }
	inline void set_m_codePage_9(int32_t value)
	{
		___m_codePage_9 = value;
	}

	inline static int32_t get_offset_of_dataItem_10() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827, ___dataItem_10)); }
	inline CodePageDataItem_t09A62F57142BF0456C8F414898A37E79BCC9F09E * get_dataItem_10() const { return ___dataItem_10; }
	inline CodePageDataItem_t09A62F57142BF0456C8F414898A37E79BCC9F09E ** get_address_of_dataItem_10() { return &___dataItem_10; }
	inline void set_dataItem_10(CodePageDataItem_t09A62F57142BF0456C8F414898A37E79BCC9F09E * value)
	{
		___dataItem_10 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dataItem_10), (void*)value);
	}

	inline static int32_t get_offset_of_m_deserializedFromEverett_11() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827, ___m_deserializedFromEverett_11)); }
	inline bool get_m_deserializedFromEverett_11() const { return ___m_deserializedFromEverett_11; }
	inline bool* get_address_of_m_deserializedFromEverett_11() { return &___m_deserializedFromEverett_11; }
	inline void set_m_deserializedFromEverett_11(bool value)
	{
		___m_deserializedFromEverett_11 = value;
	}

	inline static int32_t get_offset_of_m_isReadOnly_12() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827, ___m_isReadOnly_12)); }
	inline bool get_m_isReadOnly_12() const { return ___m_isReadOnly_12; }
	inline bool* get_address_of_m_isReadOnly_12() { return &___m_isReadOnly_12; }
	inline void set_m_isReadOnly_12(bool value)
	{
		___m_isReadOnly_12 = value;
	}

	inline static int32_t get_offset_of_encoderFallback_13() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827, ___encoderFallback_13)); }
	inline EncoderFallback_t02AC990075E17EB09F0D7E4831C3B3F264025CC4 * get_encoderFallback_13() const { return ___encoderFallback_13; }
	inline EncoderFallback_t02AC990075E17EB09F0D7E4831C3B3F264025CC4 ** get_address_of_encoderFallback_13() { return &___encoderFallback_13; }
	inline void set_encoderFallback_13(EncoderFallback_t02AC990075E17EB09F0D7E4831C3B3F264025CC4 * value)
	{
		___encoderFallback_13 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___encoderFallback_13), (void*)value);
	}

	inline static int32_t get_offset_of_decoderFallback_14() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827, ___decoderFallback_14)); }
	inline DecoderFallback_tF86D337D6576E81E5DA285E5673183EBC66DEF8D * get_decoderFallback_14() const { return ___decoderFallback_14; }
	inline DecoderFallback_tF86D337D6576E81E5DA285E5673183EBC66DEF8D ** get_address_of_decoderFallback_14() { return &___decoderFallback_14; }
	inline void set_decoderFallback_14(DecoderFallback_tF86D337D6576E81E5DA285E5673183EBC66DEF8D * value)
	{
		___decoderFallback_14 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___decoderFallback_14), (void*)value);
	}
};

struct Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827_StaticFields
{
public:
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::defaultEncoding
	Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * ___defaultEncoding_0;
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::unicodeEncoding
	Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * ___unicodeEncoding_1;
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::bigEndianUnicode
	Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * ___bigEndianUnicode_2;
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::utf7Encoding
	Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * ___utf7Encoding_3;
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::utf8Encoding
	Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * ___utf8Encoding_4;
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::utf32Encoding
	Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * ___utf32Encoding_5;
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::asciiEncoding
	Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * ___asciiEncoding_6;
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::latin1Encoding
	Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * ___latin1Encoding_7;
	// System.Collections.Hashtable modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::encodings
	Hashtable_t7565AB92A12227AD5BADD6911F10D87EE52509AC * ___encodings_8;
	// System.Object System.Text.Encoding::s_InternalSyncObject
	RuntimeObject * ___s_InternalSyncObject_15;

public:
	inline static int32_t get_offset_of_defaultEncoding_0() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827_StaticFields, ___defaultEncoding_0)); }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * get_defaultEncoding_0() const { return ___defaultEncoding_0; }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 ** get_address_of_defaultEncoding_0() { return &___defaultEncoding_0; }
	inline void set_defaultEncoding_0(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * value)
	{
		___defaultEncoding_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultEncoding_0), (void*)value);
	}

	inline static int32_t get_offset_of_unicodeEncoding_1() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827_StaticFields, ___unicodeEncoding_1)); }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * get_unicodeEncoding_1() const { return ___unicodeEncoding_1; }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 ** get_address_of_unicodeEncoding_1() { return &___unicodeEncoding_1; }
	inline void set_unicodeEncoding_1(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * value)
	{
		___unicodeEncoding_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___unicodeEncoding_1), (void*)value);
	}

	inline static int32_t get_offset_of_bigEndianUnicode_2() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827_StaticFields, ___bigEndianUnicode_2)); }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * get_bigEndianUnicode_2() const { return ___bigEndianUnicode_2; }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 ** get_address_of_bigEndianUnicode_2() { return &___bigEndianUnicode_2; }
	inline void set_bigEndianUnicode_2(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * value)
	{
		___bigEndianUnicode_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___bigEndianUnicode_2), (void*)value);
	}

	inline static int32_t get_offset_of_utf7Encoding_3() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827_StaticFields, ___utf7Encoding_3)); }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * get_utf7Encoding_3() const { return ___utf7Encoding_3; }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 ** get_address_of_utf7Encoding_3() { return &___utf7Encoding_3; }
	inline void set_utf7Encoding_3(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * value)
	{
		___utf7Encoding_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___utf7Encoding_3), (void*)value);
	}

	inline static int32_t get_offset_of_utf8Encoding_4() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827_StaticFields, ___utf8Encoding_4)); }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * get_utf8Encoding_4() const { return ___utf8Encoding_4; }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 ** get_address_of_utf8Encoding_4() { return &___utf8Encoding_4; }
	inline void set_utf8Encoding_4(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * value)
	{
		___utf8Encoding_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___utf8Encoding_4), (void*)value);
	}

	inline static int32_t get_offset_of_utf32Encoding_5() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827_StaticFields, ___utf32Encoding_5)); }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * get_utf32Encoding_5() const { return ___utf32Encoding_5; }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 ** get_address_of_utf32Encoding_5() { return &___utf32Encoding_5; }
	inline void set_utf32Encoding_5(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * value)
	{
		___utf32Encoding_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___utf32Encoding_5), (void*)value);
	}

	inline static int32_t get_offset_of_asciiEncoding_6() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827_StaticFields, ___asciiEncoding_6)); }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * get_asciiEncoding_6() const { return ___asciiEncoding_6; }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 ** get_address_of_asciiEncoding_6() { return &___asciiEncoding_6; }
	inline void set_asciiEncoding_6(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * value)
	{
		___asciiEncoding_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___asciiEncoding_6), (void*)value);
	}

	inline static int32_t get_offset_of_latin1Encoding_7() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827_StaticFields, ___latin1Encoding_7)); }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * get_latin1Encoding_7() const { return ___latin1Encoding_7; }
	inline Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 ** get_address_of_latin1Encoding_7() { return &___latin1Encoding_7; }
	inline void set_latin1Encoding_7(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * value)
	{
		___latin1Encoding_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___latin1Encoding_7), (void*)value);
	}

	inline static int32_t get_offset_of_encodings_8() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827_StaticFields, ___encodings_8)); }
	inline Hashtable_t7565AB92A12227AD5BADD6911F10D87EE52509AC * get_encodings_8() const { return ___encodings_8; }
	inline Hashtable_t7565AB92A12227AD5BADD6911F10D87EE52509AC ** get_address_of_encodings_8() { return &___encodings_8; }
	inline void set_encodings_8(Hashtable_t7565AB92A12227AD5BADD6911F10D87EE52509AC * value)
	{
		___encodings_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___encodings_8), (void*)value);
	}

	inline static int32_t get_offset_of_s_InternalSyncObject_15() { return static_cast<int32_t>(offsetof(Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827_StaticFields, ___s_InternalSyncObject_15)); }
	inline RuntimeObject * get_s_InternalSyncObject_15() const { return ___s_InternalSyncObject_15; }
	inline RuntimeObject ** get_address_of_s_InternalSyncObject_15() { return &___s_InternalSyncObject_15; }
	inline void set_s_InternalSyncObject_15(RuntimeObject * value)
	{
		___s_InternalSyncObject_15 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_InternalSyncObject_15), (void*)value);
	}
};


// System.ValueType
struct  ValueType_tDBF999C1B75C48C68621878250DBF6CDBCF51E52  : public RuntimeObject
{
public:

public:
};

// Native definition for P/Invoke marshalling of System.ValueType
struct ValueType_tDBF999C1B75C48C68621878250DBF6CDBCF51E52_marshaled_pinvoke
{
};
// Native definition for COM marshalling of System.ValueType
struct ValueType_tDBF999C1B75C48C68621878250DBF6CDBCF51E52_marshaled_com
{
};

// Unity.Burst.BurstCompiler
struct  BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2  : public RuntimeObject
{
public:

public:
};

struct BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_StaticFields
{
public:
	// System.Object Unity.Burst.BurstCompiler::GlobalLock
	RuntimeObject * ___GlobalLock_0;
	// Unity.Burst.BurstCompilerOptions Unity.Burst.BurstCompiler::_global
	BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * ____global_1;

public:
	inline static int32_t get_offset_of_GlobalLock_0() { return static_cast<int32_t>(offsetof(BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_StaticFields, ___GlobalLock_0)); }
	inline RuntimeObject * get_GlobalLock_0() const { return ___GlobalLock_0; }
	inline RuntimeObject ** get_address_of_GlobalLock_0() { return &___GlobalLock_0; }
	inline void set_GlobalLock_0(RuntimeObject * value)
	{
		___GlobalLock_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___GlobalLock_0), (void*)value);
	}

	inline static int32_t get_offset_of__global_1() { return static_cast<int32_t>(offsetof(BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_StaticFields, ____global_1)); }
	inline BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * get__global_1() const { return ____global_1; }
	inline BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 ** get_address_of__global_1() { return &____global_1; }
	inline void set__global_1(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * value)
	{
		____global_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____global_1), (void*)value);
	}
};


// Unity.Burst.BurstCompiler_BurstCompilerHelper
struct  BurstCompilerHelper_tBFAE069B1EED68EFA83AF35A2224700B2BDCB4FA  : public RuntimeObject
{
public:

public:
};

struct BurstCompilerHelper_tBFAE069B1EED68EFA83AF35A2224700B2BDCB4FA_StaticFields
{
public:
	// Unity.Burst.BurstCompiler_BurstCompilerHelper_IsBurstEnabledDelegate Unity.Burst.BurstCompiler_BurstCompilerHelper::IsBurstEnabledImpl
	IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5 * ___IsBurstEnabledImpl_0;
	// System.Boolean Unity.Burst.BurstCompiler_BurstCompilerHelper::IsBurstGenerated
	bool ___IsBurstGenerated_1;

public:
	inline static int32_t get_offset_of_IsBurstEnabledImpl_0() { return static_cast<int32_t>(offsetof(BurstCompilerHelper_tBFAE069B1EED68EFA83AF35A2224700B2BDCB4FA_StaticFields, ___IsBurstEnabledImpl_0)); }
	inline IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5 * get_IsBurstEnabledImpl_0() const { return ___IsBurstEnabledImpl_0; }
	inline IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5 ** get_address_of_IsBurstEnabledImpl_0() { return &___IsBurstEnabledImpl_0; }
	inline void set_IsBurstEnabledImpl_0(IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5 * value)
	{
		___IsBurstEnabledImpl_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___IsBurstEnabledImpl_0), (void*)value);
	}

	inline static int32_t get_offset_of_IsBurstGenerated_1() { return static_cast<int32_t>(offsetof(BurstCompilerHelper_tBFAE069B1EED68EFA83AF35A2224700B2BDCB4FA_StaticFields, ___IsBurstGenerated_1)); }
	inline bool get_IsBurstGenerated_1() const { return ___IsBurstGenerated_1; }
	inline bool* get_address_of_IsBurstGenerated_1() { return &___IsBurstGenerated_1; }
	inline void set_IsBurstGenerated_1(bool value)
	{
		___IsBurstGenerated_1 = value;
	}
};


// Unity.Burst.BurstCompilerOptions
struct  BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34  : public RuntimeObject
{
public:
	// System.Boolean Unity.Burst.BurstCompilerOptions::_enableBurstCompilation
	bool ____enableBurstCompilation_3;
	// System.Boolean Unity.Burst.BurstCompilerOptions::_enableBurstSafetyChecks
	bool ____enableBurstSafetyChecks_4;
	// System.Boolean Unity.Burst.BurstCompilerOptions::<IsGlobal>k__BackingField
	bool ___U3CIsGlobalU3Ek__BackingField_5;
	// System.Action Unity.Burst.BurstCompilerOptions::<OptionsChanged>k__BackingField
	Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * ___U3COptionsChangedU3Ek__BackingField_6;

public:
	inline static int32_t get_offset_of__enableBurstCompilation_3() { return static_cast<int32_t>(offsetof(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34, ____enableBurstCompilation_3)); }
	inline bool get__enableBurstCompilation_3() const { return ____enableBurstCompilation_3; }
	inline bool* get_address_of__enableBurstCompilation_3() { return &____enableBurstCompilation_3; }
	inline void set__enableBurstCompilation_3(bool value)
	{
		____enableBurstCompilation_3 = value;
	}

	inline static int32_t get_offset_of__enableBurstSafetyChecks_4() { return static_cast<int32_t>(offsetof(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34, ____enableBurstSafetyChecks_4)); }
	inline bool get__enableBurstSafetyChecks_4() const { return ____enableBurstSafetyChecks_4; }
	inline bool* get_address_of__enableBurstSafetyChecks_4() { return &____enableBurstSafetyChecks_4; }
	inline void set__enableBurstSafetyChecks_4(bool value)
	{
		____enableBurstSafetyChecks_4 = value;
	}

	inline static int32_t get_offset_of_U3CIsGlobalU3Ek__BackingField_5() { return static_cast<int32_t>(offsetof(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34, ___U3CIsGlobalU3Ek__BackingField_5)); }
	inline bool get_U3CIsGlobalU3Ek__BackingField_5() const { return ___U3CIsGlobalU3Ek__BackingField_5; }
	inline bool* get_address_of_U3CIsGlobalU3Ek__BackingField_5() { return &___U3CIsGlobalU3Ek__BackingField_5; }
	inline void set_U3CIsGlobalU3Ek__BackingField_5(bool value)
	{
		___U3CIsGlobalU3Ek__BackingField_5 = value;
	}

	inline static int32_t get_offset_of_U3COptionsChangedU3Ek__BackingField_6() { return static_cast<int32_t>(offsetof(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34, ___U3COptionsChangedU3Ek__BackingField_6)); }
	inline Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * get_U3COptionsChangedU3Ek__BackingField_6() const { return ___U3COptionsChangedU3Ek__BackingField_6; }
	inline Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 ** get_address_of_U3COptionsChangedU3Ek__BackingField_6() { return &___U3COptionsChangedU3Ek__BackingField_6; }
	inline void set_U3COptionsChangedU3Ek__BackingField_6(Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * value)
	{
		___U3COptionsChangedU3Ek__BackingField_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3COptionsChangedU3Ek__BackingField_6), (void*)value);
	}
};

struct BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_StaticFields
{
public:
	// System.String Unity.Burst.BurstCompilerOptions::DefaultCacheFolder
	String_t* ___DefaultCacheFolder_0;
	// System.Boolean Unity.Burst.BurstCompilerOptions::ForceDisableBurstCompilation
	bool ___ForceDisableBurstCompilation_1;
	// System.Boolean Unity.Burst.BurstCompilerOptions::ForceBurstCompilationSynchronously
	bool ___ForceBurstCompilationSynchronously_2;

public:
	inline static int32_t get_offset_of_DefaultCacheFolder_0() { return static_cast<int32_t>(offsetof(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_StaticFields, ___DefaultCacheFolder_0)); }
	inline String_t* get_DefaultCacheFolder_0() const { return ___DefaultCacheFolder_0; }
	inline String_t** get_address_of_DefaultCacheFolder_0() { return &___DefaultCacheFolder_0; }
	inline void set_DefaultCacheFolder_0(String_t* value)
	{
		___DefaultCacheFolder_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___DefaultCacheFolder_0), (void*)value);
	}

	inline static int32_t get_offset_of_ForceDisableBurstCompilation_1() { return static_cast<int32_t>(offsetof(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_StaticFields, ___ForceDisableBurstCompilation_1)); }
	inline bool get_ForceDisableBurstCompilation_1() const { return ___ForceDisableBurstCompilation_1; }
	inline bool* get_address_of_ForceDisableBurstCompilation_1() { return &___ForceDisableBurstCompilation_1; }
	inline void set_ForceDisableBurstCompilation_1(bool value)
	{
		___ForceDisableBurstCompilation_1 = value;
	}

	inline static int32_t get_offset_of_ForceBurstCompilationSynchronously_2() { return static_cast<int32_t>(offsetof(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_StaticFields, ___ForceBurstCompilationSynchronously_2)); }
	inline bool get_ForceBurstCompilationSynchronously_2() const { return ___ForceBurstCompilationSynchronously_2; }
	inline bool* get_address_of_ForceBurstCompilationSynchronously_2() { return &___ForceBurstCompilationSynchronously_2; }
	inline void set_ForceBurstCompilationSynchronously_2(bool value)
	{
		___ForceBurstCompilationSynchronously_2 = value;
	}
};


// Unity.Burst.BurstRuntime
struct  BurstRuntime_t725AE7FF2003494CEB5F987F3365BA847C762735  : public RuntimeObject
{
public:

public:
};

struct BurstRuntime_t725AE7FF2003494CEB5F987F3365BA847C762735_StaticFields
{
public:
	// Unity.Burst.BurstRuntime_NativeLogDelegate Unity.Burst.BurstRuntime::ManagedNativeLog
	NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * ___ManagedNativeLog_0;

public:
	inline static int32_t get_offset_of_ManagedNativeLog_0() { return static_cast<int32_t>(offsetof(BurstRuntime_t725AE7FF2003494CEB5F987F3365BA847C762735_StaticFields, ___ManagedNativeLog_0)); }
	inline NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * get_ManagedNativeLog_0() const { return ___ManagedNativeLog_0; }
	inline NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 ** get_address_of_ManagedNativeLog_0() { return &___ManagedNativeLog_0; }
	inline void set_ManagedNativeLog_0(NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * value)
	{
		___ManagedNativeLog_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___ManagedNativeLog_0), (void*)value);
	}
};


// Unity.Burst.SharedStatic
struct  SharedStatic_t4A478B3109091C26ED6F772B9BEC1EA0802B2FFC  : public RuntimeObject
{
public:

public:
};

struct SharedStatic_t4A478B3109091C26ED6F772B9BEC1EA0802B2FFC_StaticFields
{
public:
	// System.Collections.Generic.Dictionary`2<System.Int64,System.Type> Unity.Burst.SharedStatic::HashToType
	Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D * ___HashToType_0;

public:
	inline static int32_t get_offset_of_HashToType_0() { return static_cast<int32_t>(offsetof(SharedStatic_t4A478B3109091C26ED6F772B9BEC1EA0802B2FFC_StaticFields, ___HashToType_0)); }
	inline Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D * get_HashToType_0() const { return ___HashToType_0; }
	inline Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D ** get_address_of_HashToType_0() { return &___HashToType_0; }
	inline void set_HashToType_0(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D * value)
	{
		___HashToType_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___HashToType_0), (void*)value);
	}
};


// System.Boolean
struct  Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37 
{
public:
	// System.Boolean System.Boolean::m_value
	bool ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37, ___m_value_0)); }
	inline bool get_m_value_0() const { return ___m_value_0; }
	inline bool* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(bool value)
	{
		___m_value_0 = value;
	}
};

struct Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_StaticFields
{
public:
	// System.String System.Boolean::TrueString
	String_t* ___TrueString_5;
	// System.String System.Boolean::FalseString
	String_t* ___FalseString_6;

public:
	inline static int32_t get_offset_of_TrueString_5() { return static_cast<int32_t>(offsetof(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_StaticFields, ___TrueString_5)); }
	inline String_t* get_TrueString_5() const { return ___TrueString_5; }
	inline String_t** get_address_of_TrueString_5() { return &___TrueString_5; }
	inline void set_TrueString_5(String_t* value)
	{
		___TrueString_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___TrueString_5), (void*)value);
	}

	inline static int32_t get_offset_of_FalseString_6() { return static_cast<int32_t>(offsetof(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_StaticFields, ___FalseString_6)); }
	inline String_t* get_FalseString_6() const { return ___FalseString_6; }
	inline String_t** get_address_of_FalseString_6() { return &___FalseString_6; }
	inline void set_FalseString_6(String_t* value)
	{
		___FalseString_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___FalseString_6), (void*)value);
	}
};


// System.Byte
struct  Byte_t0111FAB8B8685667EDDAF77683F0D8F86B659056 
{
public:
	// System.Byte System.Byte::m_value
	uint8_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Byte_t0111FAB8B8685667EDDAF77683F0D8F86B659056, ___m_value_0)); }
	inline uint8_t get_m_value_0() const { return ___m_value_0; }
	inline uint8_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint8_t value)
	{
		___m_value_0 = value;
	}
};


// System.Char
struct  Char_tFF60D8E7E89A20BE2294A003734341BD1DF43E14 
{
public:
	// System.Char System.Char::m_value
	Il2CppChar ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Char_tFF60D8E7E89A20BE2294A003734341BD1DF43E14, ___m_value_0)); }
	inline Il2CppChar get_m_value_0() const { return ___m_value_0; }
	inline Il2CppChar* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(Il2CppChar value)
	{
		___m_value_0 = value;
	}
};

struct Char_tFF60D8E7E89A20BE2294A003734341BD1DF43E14_StaticFields
{
public:
	// System.Byte[] System.Char::categoryForLatin1
	ByteU5BU5D_tDBBEB0E8362242FA7223000D978B0DD19D4B0726* ___categoryForLatin1_3;

public:
	inline static int32_t get_offset_of_categoryForLatin1_3() { return static_cast<int32_t>(offsetof(Char_tFF60D8E7E89A20BE2294A003734341BD1DF43E14_StaticFields, ___categoryForLatin1_3)); }
	inline ByteU5BU5D_tDBBEB0E8362242FA7223000D978B0DD19D4B0726* get_categoryForLatin1_3() const { return ___categoryForLatin1_3; }
	inline ByteU5BU5D_tDBBEB0E8362242FA7223000D978B0DD19D4B0726** get_address_of_categoryForLatin1_3() { return &___categoryForLatin1_3; }
	inline void set_categoryForLatin1_3(ByteU5BU5D_tDBBEB0E8362242FA7223000D978B0DD19D4B0726* value)
	{
		___categoryForLatin1_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___categoryForLatin1_3), (void*)value);
	}
};


// System.Enum
struct  Enum_t23B90B40F60E677A8025267341651C94AE079CDA  : public ValueType_tDBF999C1B75C48C68621878250DBF6CDBCF51E52
{
public:

public:
};

struct Enum_t23B90B40F60E677A8025267341651C94AE079CDA_StaticFields
{
public:
	// System.Char[] System.Enum::enumSeperatorCharArray
	CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34* ___enumSeperatorCharArray_0;

public:
	inline static int32_t get_offset_of_enumSeperatorCharArray_0() { return static_cast<int32_t>(offsetof(Enum_t23B90B40F60E677A8025267341651C94AE079CDA_StaticFields, ___enumSeperatorCharArray_0)); }
	inline CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34* get_enumSeperatorCharArray_0() const { return ___enumSeperatorCharArray_0; }
	inline CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34** get_address_of_enumSeperatorCharArray_0() { return &___enumSeperatorCharArray_0; }
	inline void set_enumSeperatorCharArray_0(CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34* value)
	{
		___enumSeperatorCharArray_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumSeperatorCharArray_0), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.Enum
struct Enum_t23B90B40F60E677A8025267341651C94AE079CDA_marshaled_pinvoke
{
};
// Native definition for COM marshalling of System.Enum
struct Enum_t23B90B40F60E677A8025267341651C94AE079CDA_marshaled_com
{
};

// System.Int32
struct  Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046 
{
public:
	// System.Int32 System.Int32::m_value
	int32_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046, ___m_value_0)); }
	inline int32_t get_m_value_0() const { return ___m_value_0; }
	inline int32_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(int32_t value)
	{
		___m_value_0 = value;
	}
};


// System.Int64
struct  Int64_t378EE0D608BD3107E77238E85F30D2BBD46981F3 
{
public:
	// System.Int64 System.Int64::m_value
	int64_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Int64_t378EE0D608BD3107E77238E85F30D2BBD46981F3, ___m_value_0)); }
	inline int64_t get_m_value_0() const { return ___m_value_0; }
	inline int64_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(int64_t value)
	{
		___m_value_0 = value;
	}
};


// System.IntPtr
struct  IntPtr_t 
{
public:
	// System.Void* System.IntPtr::m_value
	void* ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(IntPtr_t, ___m_value_0)); }
	inline void* get_m_value_0() const { return ___m_value_0; }
	inline void** get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(void* value)
	{
		___m_value_0 = value;
	}
};

struct IntPtr_t_StaticFields
{
public:
	// System.IntPtr System.IntPtr::Zero
	intptr_t ___Zero_1;

public:
	inline static int32_t get_offset_of_Zero_1() { return static_cast<int32_t>(offsetof(IntPtr_t_StaticFields, ___Zero_1)); }
	inline intptr_t get_Zero_1() const { return ___Zero_1; }
	inline intptr_t* get_address_of_Zero_1() { return &___Zero_1; }
	inline void set_Zero_1(intptr_t value)
	{
		___Zero_1 = value;
	}
};


// System.UInt32
struct  UInt32_tE60352A06233E4E69DD198BCC67142159F686B15 
{
public:
	// System.UInt32 System.UInt32::m_value
	uint32_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(UInt32_tE60352A06233E4E69DD198BCC67142159F686B15, ___m_value_0)); }
	inline uint32_t get_m_value_0() const { return ___m_value_0; }
	inline uint32_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint32_t value)
	{
		___m_value_0 = value;
	}
};


// System.UInt64
struct  UInt64_tEC57511B3E3CA2DBA1BEBD434C6983E31C943281 
{
public:
	// System.UInt64 System.UInt64::m_value
	uint64_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(UInt64_tEC57511B3E3CA2DBA1BEBD434C6983E31C943281, ___m_value_0)); }
	inline uint64_t get_m_value_0() const { return ___m_value_0; }
	inline uint64_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint64_t value)
	{
		___m_value_0 = value;
	}
};


// System.Void
struct  Void_t700C6383A2A510C2CF4DD86DABD5CA9FF70ADAC5 
{
public:
	union
	{
		struct
		{
		};
		uint8_t Void_t700C6383A2A510C2CF4DD86DABD5CA9FF70ADAC5__padding[1];
	};

public:
};


// Unity.Burst.CompilerServices.AssumeRangeAttribute
struct  AssumeRangeAttribute_t9CB4051727958D8EC81907FC2D1E8A4672DBA1B1  : public Attribute_t037CA9D9F3B742C063DB364D2EEBBF9FC5772C71
{
public:

public:
};


// Unity.Burst.NoAliasAttribute
struct  NoAliasAttribute_t444822FD22BC82F0E365DB1CEDC4AE7FC5E7DE13  : public Attribute_t037CA9D9F3B742C063DB364D2EEBBF9FC5772C71
{
public:

public:
};


// Unity.Burst.SharedStatic`1<Unity.Burst.FunctionPointer`1<System.Object>>
struct  SharedStatic_1_t2EB4786D756326416AA992B56495A5243927424C 
{
public:
	// System.Void* Unity.Burst.SharedStatic`1::_buffer
	void* ____buffer_0;

public:
	inline static int32_t get_offset_of__buffer_0() { return static_cast<int32_t>(offsetof(SharedStatic_1_t2EB4786D756326416AA992B56495A5243927424C, ____buffer_0)); }
	inline void* get__buffer_0() const { return ____buffer_0; }
	inline void** get_address_of__buffer_0() { return &____buffer_0; }
	inline void set__buffer_0(void* value)
	{
		____buffer_0 = value;
	}
};


// Unity.Burst.SharedStatic`1<Unity.Burst.FunctionPointer`1<Unity.Burst.BurstRuntime_NativeLogDelegate>>
struct  SharedStatic_1_tB7D634D22A39EF1E5DEADA4A7C833FBA010D2417 
{
public:
	// System.Void* Unity.Burst.SharedStatic`1::_buffer
	void* ____buffer_0;

public:
	inline static int32_t get_offset_of__buffer_0() { return static_cast<int32_t>(offsetof(SharedStatic_1_tB7D634D22A39EF1E5DEADA4A7C833FBA010D2417, ____buffer_0)); }
	inline void* get__buffer_0() const { return ____buffer_0; }
	inline void** get_address_of__buffer_0() { return &____buffer_0; }
	inline void set__buffer_0(void* value)
	{
		____buffer_0 = value;
	}
};


// UnityEngine.Hash128
struct  Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A 
{
public:
	// System.UInt32 UnityEngine.Hash128::m_u32_0
	uint32_t ___m_u32_0_0;
	// System.UInt32 UnityEngine.Hash128::m_u32_1
	uint32_t ___m_u32_1_1;
	// System.UInt32 UnityEngine.Hash128::m_u32_2
	uint32_t ___m_u32_2_2;
	// System.UInt32 UnityEngine.Hash128::m_u32_3
	uint32_t ___m_u32_3_3;

public:
	inline static int32_t get_offset_of_m_u32_0_0() { return static_cast<int32_t>(offsetof(Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A, ___m_u32_0_0)); }
	inline uint32_t get_m_u32_0_0() const { return ___m_u32_0_0; }
	inline uint32_t* get_address_of_m_u32_0_0() { return &___m_u32_0_0; }
	inline void set_m_u32_0_0(uint32_t value)
	{
		___m_u32_0_0 = value;
	}

	inline static int32_t get_offset_of_m_u32_1_1() { return static_cast<int32_t>(offsetof(Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A, ___m_u32_1_1)); }
	inline uint32_t get_m_u32_1_1() const { return ___m_u32_1_1; }
	inline uint32_t* get_address_of_m_u32_1_1() { return &___m_u32_1_1; }
	inline void set_m_u32_1_1(uint32_t value)
	{
		___m_u32_1_1 = value;
	}

	inline static int32_t get_offset_of_m_u32_2_2() { return static_cast<int32_t>(offsetof(Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A, ___m_u32_2_2)); }
	inline uint32_t get_m_u32_2_2() const { return ___m_u32_2_2; }
	inline uint32_t* get_address_of_m_u32_2_2() { return &___m_u32_2_2; }
	inline void set_m_u32_2_2(uint32_t value)
	{
		___m_u32_2_2 = value;
	}

	inline static int32_t get_offset_of_m_u32_3_3() { return static_cast<int32_t>(offsetof(Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A, ___m_u32_3_3)); }
	inline uint32_t get_m_u32_3_3() const { return ___m_u32_3_3; }
	inline uint32_t* get_address_of_m_u32_3_3() { return &___m_u32_3_3; }
	inline void set_m_u32_3_3(uint32_t value)
	{
		___m_u32_3_3 = value;
	}
};


// System.Delegate
struct  Delegate_t  : public RuntimeObject
{
public:
	// System.IntPtr System.Delegate::method_ptr
	Il2CppMethodPointer ___method_ptr_0;
	// System.IntPtr System.Delegate::invoke_impl
	intptr_t ___invoke_impl_1;
	// System.Object System.Delegate::m_target
	RuntimeObject * ___m_target_2;
	// System.IntPtr System.Delegate::method
	intptr_t ___method_3;
	// System.IntPtr System.Delegate::delegate_trampoline
	intptr_t ___delegate_trampoline_4;
	// System.IntPtr System.Delegate::extra_arg
	intptr_t ___extra_arg_5;
	// System.IntPtr System.Delegate::method_code
	intptr_t ___method_code_6;
	// System.Reflection.MethodInfo System.Delegate::method_info
	MethodInfo_t * ___method_info_7;
	// System.Reflection.MethodInfo System.Delegate::original_method_info
	MethodInfo_t * ___original_method_info_8;
	// System.DelegateData System.Delegate::data
	DelegateData_t17DD30660E330C49381DAA99F934BE75CB11F288 * ___data_9;
	// System.Boolean System.Delegate::method_is_virtual
	bool ___method_is_virtual_10;

public:
	inline static int32_t get_offset_of_method_ptr_0() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_ptr_0)); }
	inline Il2CppMethodPointer get_method_ptr_0() const { return ___method_ptr_0; }
	inline Il2CppMethodPointer* get_address_of_method_ptr_0() { return &___method_ptr_0; }
	inline void set_method_ptr_0(Il2CppMethodPointer value)
	{
		___method_ptr_0 = value;
	}

	inline static int32_t get_offset_of_invoke_impl_1() { return static_cast<int32_t>(offsetof(Delegate_t, ___invoke_impl_1)); }
	inline intptr_t get_invoke_impl_1() const { return ___invoke_impl_1; }
	inline intptr_t* get_address_of_invoke_impl_1() { return &___invoke_impl_1; }
	inline void set_invoke_impl_1(intptr_t value)
	{
		___invoke_impl_1 = value;
	}

	inline static int32_t get_offset_of_m_target_2() { return static_cast<int32_t>(offsetof(Delegate_t, ___m_target_2)); }
	inline RuntimeObject * get_m_target_2() const { return ___m_target_2; }
	inline RuntimeObject ** get_address_of_m_target_2() { return &___m_target_2; }
	inline void set_m_target_2(RuntimeObject * value)
	{
		___m_target_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_target_2), (void*)value);
	}

	inline static int32_t get_offset_of_method_3() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_3)); }
	inline intptr_t get_method_3() const { return ___method_3; }
	inline intptr_t* get_address_of_method_3() { return &___method_3; }
	inline void set_method_3(intptr_t value)
	{
		___method_3 = value;
	}

	inline static int32_t get_offset_of_delegate_trampoline_4() { return static_cast<int32_t>(offsetof(Delegate_t, ___delegate_trampoline_4)); }
	inline intptr_t get_delegate_trampoline_4() const { return ___delegate_trampoline_4; }
	inline intptr_t* get_address_of_delegate_trampoline_4() { return &___delegate_trampoline_4; }
	inline void set_delegate_trampoline_4(intptr_t value)
	{
		___delegate_trampoline_4 = value;
	}

	inline static int32_t get_offset_of_extra_arg_5() { return static_cast<int32_t>(offsetof(Delegate_t, ___extra_arg_5)); }
	inline intptr_t get_extra_arg_5() const { return ___extra_arg_5; }
	inline intptr_t* get_address_of_extra_arg_5() { return &___extra_arg_5; }
	inline void set_extra_arg_5(intptr_t value)
	{
		___extra_arg_5 = value;
	}

	inline static int32_t get_offset_of_method_code_6() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_code_6)); }
	inline intptr_t get_method_code_6() const { return ___method_code_6; }
	inline intptr_t* get_address_of_method_code_6() { return &___method_code_6; }
	inline void set_method_code_6(intptr_t value)
	{
		___method_code_6 = value;
	}

	inline static int32_t get_offset_of_method_info_7() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_info_7)); }
	inline MethodInfo_t * get_method_info_7() const { return ___method_info_7; }
	inline MethodInfo_t ** get_address_of_method_info_7() { return &___method_info_7; }
	inline void set_method_info_7(MethodInfo_t * value)
	{
		___method_info_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___method_info_7), (void*)value);
	}

	inline static int32_t get_offset_of_original_method_info_8() { return static_cast<int32_t>(offsetof(Delegate_t, ___original_method_info_8)); }
	inline MethodInfo_t * get_original_method_info_8() const { return ___original_method_info_8; }
	inline MethodInfo_t ** get_address_of_original_method_info_8() { return &___original_method_info_8; }
	inline void set_original_method_info_8(MethodInfo_t * value)
	{
		___original_method_info_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___original_method_info_8), (void*)value);
	}

	inline static int32_t get_offset_of_data_9() { return static_cast<int32_t>(offsetof(Delegate_t, ___data_9)); }
	inline DelegateData_t17DD30660E330C49381DAA99F934BE75CB11F288 * get_data_9() const { return ___data_9; }
	inline DelegateData_t17DD30660E330C49381DAA99F934BE75CB11F288 ** get_address_of_data_9() { return &___data_9; }
	inline void set_data_9(DelegateData_t17DD30660E330C49381DAA99F934BE75CB11F288 * value)
	{
		___data_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___data_9), (void*)value);
	}

	inline static int32_t get_offset_of_method_is_virtual_10() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_is_virtual_10)); }
	inline bool get_method_is_virtual_10() const { return ___method_is_virtual_10; }
	inline bool* get_address_of_method_is_virtual_10() { return &___method_is_virtual_10; }
	inline void set_method_is_virtual_10(bool value)
	{
		___method_is_virtual_10 = value;
	}
};

// Native definition for P/Invoke marshalling of System.Delegate
struct Delegate_t_marshaled_pinvoke
{
	intptr_t ___method_ptr_0;
	intptr_t ___invoke_impl_1;
	Il2CppIUnknown* ___m_target_2;
	intptr_t ___method_3;
	intptr_t ___delegate_trampoline_4;
	intptr_t ___extra_arg_5;
	intptr_t ___method_code_6;
	MethodInfo_t * ___method_info_7;
	MethodInfo_t * ___original_method_info_8;
	DelegateData_t17DD30660E330C49381DAA99F934BE75CB11F288 * ___data_9;
	int32_t ___method_is_virtual_10;
};
// Native definition for COM marshalling of System.Delegate
struct Delegate_t_marshaled_com
{
	intptr_t ___method_ptr_0;
	intptr_t ___invoke_impl_1;
	Il2CppIUnknown* ___m_target_2;
	intptr_t ___method_3;
	intptr_t ___delegate_trampoline_4;
	intptr_t ___extra_arg_5;
	intptr_t ___method_code_6;
	MethodInfo_t * ___method_info_7;
	MethodInfo_t * ___original_method_info_8;
	DelegateData_t17DD30660E330C49381DAA99F934BE75CB11F288 * ___data_9;
	int32_t ___method_is_virtual_10;
};

// System.Exception
struct  Exception_t  : public RuntimeObject
{
public:
	// System.String System.Exception::_className
	String_t* ____className_1;
	// System.String System.Exception::_message
	String_t* ____message_2;
	// System.Collections.IDictionary System.Exception::_data
	RuntimeObject* ____data_3;
	// System.Exception System.Exception::_innerException
	Exception_t * ____innerException_4;
	// System.String System.Exception::_helpURL
	String_t* ____helpURL_5;
	// System.Object System.Exception::_stackTrace
	RuntimeObject * ____stackTrace_6;
	// System.String System.Exception::_stackTraceString
	String_t* ____stackTraceString_7;
	// System.String System.Exception::_remoteStackTraceString
	String_t* ____remoteStackTraceString_8;
	// System.Int32 System.Exception::_remoteStackIndex
	int32_t ____remoteStackIndex_9;
	// System.Object System.Exception::_dynamicMethods
	RuntimeObject * ____dynamicMethods_10;
	// System.Int32 System.Exception::_HResult
	int32_t ____HResult_11;
	// System.String System.Exception::_source
	String_t* ____source_12;
	// System.Runtime.Serialization.SafeSerializationManager System.Exception::_safeSerializationManager
	SafeSerializationManager_tDE44F029589A028F8A3053C5C06153FAB4AAE29F * ____safeSerializationManager_13;
	// System.Diagnostics.StackTrace[] System.Exception::captured_traces
	StackTraceU5BU5D_t4AD999C288CB6D1F38A299D12B1598D606588971* ___captured_traces_14;
	// System.IntPtr[] System.Exception::native_trace_ips
	IntPtrU5BU5D_t27FC72B0409D75AAF33EC42498E8094E95FEE9A6* ___native_trace_ips_15;

public:
	inline static int32_t get_offset_of__className_1() { return static_cast<int32_t>(offsetof(Exception_t, ____className_1)); }
	inline String_t* get__className_1() const { return ____className_1; }
	inline String_t** get_address_of__className_1() { return &____className_1; }
	inline void set__className_1(String_t* value)
	{
		____className_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____className_1), (void*)value);
	}

	inline static int32_t get_offset_of__message_2() { return static_cast<int32_t>(offsetof(Exception_t, ____message_2)); }
	inline String_t* get__message_2() const { return ____message_2; }
	inline String_t** get_address_of__message_2() { return &____message_2; }
	inline void set__message_2(String_t* value)
	{
		____message_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____message_2), (void*)value);
	}

	inline static int32_t get_offset_of__data_3() { return static_cast<int32_t>(offsetof(Exception_t, ____data_3)); }
	inline RuntimeObject* get__data_3() const { return ____data_3; }
	inline RuntimeObject** get_address_of__data_3() { return &____data_3; }
	inline void set__data_3(RuntimeObject* value)
	{
		____data_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____data_3), (void*)value);
	}

	inline static int32_t get_offset_of__innerException_4() { return static_cast<int32_t>(offsetof(Exception_t, ____innerException_4)); }
	inline Exception_t * get__innerException_4() const { return ____innerException_4; }
	inline Exception_t ** get_address_of__innerException_4() { return &____innerException_4; }
	inline void set__innerException_4(Exception_t * value)
	{
		____innerException_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____innerException_4), (void*)value);
	}

	inline static int32_t get_offset_of__helpURL_5() { return static_cast<int32_t>(offsetof(Exception_t, ____helpURL_5)); }
	inline String_t* get__helpURL_5() const { return ____helpURL_5; }
	inline String_t** get_address_of__helpURL_5() { return &____helpURL_5; }
	inline void set__helpURL_5(String_t* value)
	{
		____helpURL_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____helpURL_5), (void*)value);
	}

	inline static int32_t get_offset_of__stackTrace_6() { return static_cast<int32_t>(offsetof(Exception_t, ____stackTrace_6)); }
	inline RuntimeObject * get__stackTrace_6() const { return ____stackTrace_6; }
	inline RuntimeObject ** get_address_of__stackTrace_6() { return &____stackTrace_6; }
	inline void set__stackTrace_6(RuntimeObject * value)
	{
		____stackTrace_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____stackTrace_6), (void*)value);
	}

	inline static int32_t get_offset_of__stackTraceString_7() { return static_cast<int32_t>(offsetof(Exception_t, ____stackTraceString_7)); }
	inline String_t* get__stackTraceString_7() const { return ____stackTraceString_7; }
	inline String_t** get_address_of__stackTraceString_7() { return &____stackTraceString_7; }
	inline void set__stackTraceString_7(String_t* value)
	{
		____stackTraceString_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____stackTraceString_7), (void*)value);
	}

	inline static int32_t get_offset_of__remoteStackTraceString_8() { return static_cast<int32_t>(offsetof(Exception_t, ____remoteStackTraceString_8)); }
	inline String_t* get__remoteStackTraceString_8() const { return ____remoteStackTraceString_8; }
	inline String_t** get_address_of__remoteStackTraceString_8() { return &____remoteStackTraceString_8; }
	inline void set__remoteStackTraceString_8(String_t* value)
	{
		____remoteStackTraceString_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____remoteStackTraceString_8), (void*)value);
	}

	inline static int32_t get_offset_of__remoteStackIndex_9() { return static_cast<int32_t>(offsetof(Exception_t, ____remoteStackIndex_9)); }
	inline int32_t get__remoteStackIndex_9() const { return ____remoteStackIndex_9; }
	inline int32_t* get_address_of__remoteStackIndex_9() { return &____remoteStackIndex_9; }
	inline void set__remoteStackIndex_9(int32_t value)
	{
		____remoteStackIndex_9 = value;
	}

	inline static int32_t get_offset_of__dynamicMethods_10() { return static_cast<int32_t>(offsetof(Exception_t, ____dynamicMethods_10)); }
	inline RuntimeObject * get__dynamicMethods_10() const { return ____dynamicMethods_10; }
	inline RuntimeObject ** get_address_of__dynamicMethods_10() { return &____dynamicMethods_10; }
	inline void set__dynamicMethods_10(RuntimeObject * value)
	{
		____dynamicMethods_10 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____dynamicMethods_10), (void*)value);
	}

	inline static int32_t get_offset_of__HResult_11() { return static_cast<int32_t>(offsetof(Exception_t, ____HResult_11)); }
	inline int32_t get__HResult_11() const { return ____HResult_11; }
	inline int32_t* get_address_of__HResult_11() { return &____HResult_11; }
	inline void set__HResult_11(int32_t value)
	{
		____HResult_11 = value;
	}

	inline static int32_t get_offset_of__source_12() { return static_cast<int32_t>(offsetof(Exception_t, ____source_12)); }
	inline String_t* get__source_12() const { return ____source_12; }
	inline String_t** get_address_of__source_12() { return &____source_12; }
	inline void set__source_12(String_t* value)
	{
		____source_12 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____source_12), (void*)value);
	}

	inline static int32_t get_offset_of__safeSerializationManager_13() { return static_cast<int32_t>(offsetof(Exception_t, ____safeSerializationManager_13)); }
	inline SafeSerializationManager_tDE44F029589A028F8A3053C5C06153FAB4AAE29F * get__safeSerializationManager_13() const { return ____safeSerializationManager_13; }
	inline SafeSerializationManager_tDE44F029589A028F8A3053C5C06153FAB4AAE29F ** get_address_of__safeSerializationManager_13() { return &____safeSerializationManager_13; }
	inline void set__safeSerializationManager_13(SafeSerializationManager_tDE44F029589A028F8A3053C5C06153FAB4AAE29F * value)
	{
		____safeSerializationManager_13 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____safeSerializationManager_13), (void*)value);
	}

	inline static int32_t get_offset_of_captured_traces_14() { return static_cast<int32_t>(offsetof(Exception_t, ___captured_traces_14)); }
	inline StackTraceU5BU5D_t4AD999C288CB6D1F38A299D12B1598D606588971* get_captured_traces_14() const { return ___captured_traces_14; }
	inline StackTraceU5BU5D_t4AD999C288CB6D1F38A299D12B1598D606588971** get_address_of_captured_traces_14() { return &___captured_traces_14; }
	inline void set_captured_traces_14(StackTraceU5BU5D_t4AD999C288CB6D1F38A299D12B1598D606588971* value)
	{
		___captured_traces_14 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___captured_traces_14), (void*)value);
	}

	inline static int32_t get_offset_of_native_trace_ips_15() { return static_cast<int32_t>(offsetof(Exception_t, ___native_trace_ips_15)); }
	inline IntPtrU5BU5D_t27FC72B0409D75AAF33EC42498E8094E95FEE9A6* get_native_trace_ips_15() const { return ___native_trace_ips_15; }
	inline IntPtrU5BU5D_t27FC72B0409D75AAF33EC42498E8094E95FEE9A6** get_address_of_native_trace_ips_15() { return &___native_trace_ips_15; }
	inline void set_native_trace_ips_15(IntPtrU5BU5D_t27FC72B0409D75AAF33EC42498E8094E95FEE9A6* value)
	{
		___native_trace_ips_15 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___native_trace_ips_15), (void*)value);
	}
};

struct Exception_t_StaticFields
{
public:
	// System.Object System.Exception::s_EDILock
	RuntimeObject * ___s_EDILock_0;

public:
	inline static int32_t get_offset_of_s_EDILock_0() { return static_cast<int32_t>(offsetof(Exception_t_StaticFields, ___s_EDILock_0)); }
	inline RuntimeObject * get_s_EDILock_0() const { return ___s_EDILock_0; }
	inline RuntimeObject ** get_address_of_s_EDILock_0() { return &___s_EDILock_0; }
	inline void set_s_EDILock_0(RuntimeObject * value)
	{
		___s_EDILock_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_EDILock_0), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.Exception
struct Exception_t_marshaled_pinvoke
{
	char* ____className_1;
	char* ____message_2;
	RuntimeObject* ____data_3;
	Exception_t_marshaled_pinvoke* ____innerException_4;
	char* ____helpURL_5;
	Il2CppIUnknown* ____stackTrace_6;
	char* ____stackTraceString_7;
	char* ____remoteStackTraceString_8;
	int32_t ____remoteStackIndex_9;
	Il2CppIUnknown* ____dynamicMethods_10;
	int32_t ____HResult_11;
	char* ____source_12;
	SafeSerializationManager_tDE44F029589A028F8A3053C5C06153FAB4AAE29F * ____safeSerializationManager_13;
	StackTraceU5BU5D_t4AD999C288CB6D1F38A299D12B1598D606588971* ___captured_traces_14;
	Il2CppSafeArray/*NONE*/* ___native_trace_ips_15;
};
// Native definition for COM marshalling of System.Exception
struct Exception_t_marshaled_com
{
	Il2CppChar* ____className_1;
	Il2CppChar* ____message_2;
	RuntimeObject* ____data_3;
	Exception_t_marshaled_com* ____innerException_4;
	Il2CppChar* ____helpURL_5;
	Il2CppIUnknown* ____stackTrace_6;
	Il2CppChar* ____stackTraceString_7;
	Il2CppChar* ____remoteStackTraceString_8;
	int32_t ____remoteStackIndex_9;
	Il2CppIUnknown* ____dynamicMethods_10;
	int32_t ____HResult_11;
	Il2CppChar* ____source_12;
	SafeSerializationManager_tDE44F029589A028F8A3053C5C06153FAB4AAE29F * ____safeSerializationManager_13;
	StackTraceU5BU5D_t4AD999C288CB6D1F38A299D12B1598D606588971* ___captured_traces_14;
	Il2CppSafeArray/*NONE*/* ___native_trace_ips_15;
};

// System.Reflection.BindingFlags
struct  BindingFlags_tAAAB07D9AC588F0D55D844E51D7035E96DF94733 
{
public:
	// System.Int32 System.Reflection.BindingFlags::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(BindingFlags_tAAAB07D9AC588F0D55D844E51D7035E96DF94733, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// System.RuntimeTypeHandle
struct  RuntimeTypeHandle_tC33965ADA3E041E0C94AF05E5CB527B56482CEF9 
{
public:
	// System.IntPtr System.RuntimeTypeHandle::value
	intptr_t ___value_0;

public:
	inline static int32_t get_offset_of_value_0() { return static_cast<int32_t>(offsetof(RuntimeTypeHandle_tC33965ADA3E041E0C94AF05E5CB527B56482CEF9, ___value_0)); }
	inline intptr_t get_value_0() const { return ___value_0; }
	inline intptr_t* get_address_of_value_0() { return &___value_0; }
	inline void set_value_0(intptr_t value)
	{
		___value_0 = value;
	}
};


// Unity.Burst.BurstRuntime_LogHelper
struct  LogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A  : public RuntimeObject
{
public:

public:
};

struct LogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_StaticFields
{
public:
	// Unity.Burst.SharedStatic`1<Unity.Burst.FunctionPointer`1<Unity.Burst.BurstRuntime_NativeLogDelegate>> Unity.Burst.BurstRuntime_LogHelper::Instance
	SharedStatic_1_tB7D634D22A39EF1E5DEADA4A7C833FBA010D2417  ___Instance_0;

public:
	inline static int32_t get_offset_of_Instance_0() { return static_cast<int32_t>(offsetof(LogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_StaticFields, ___Instance_0)); }
	inline SharedStatic_1_tB7D634D22A39EF1E5DEADA4A7C833FBA010D2417  get_Instance_0() const { return ___Instance_0; }
	inline SharedStatic_1_tB7D634D22A39EF1E5DEADA4A7C833FBA010D2417 * get_address_of_Instance_0() { return &___Instance_0; }
	inline void set_Instance_0(SharedStatic_1_tB7D634D22A39EF1E5DEADA4A7C833FBA010D2417  value)
	{
		___Instance_0 = value;
	}
};


// Unity.Burst.FloatMode
struct  FloatMode_t38741ACC50724A284056372B5D90095D40ACB1E4 
{
public:
	// System.Int32 Unity.Burst.FloatMode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(FloatMode_t38741ACC50724A284056372B5D90095D40ACB1E4, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Unity.Burst.FloatPrecision
struct  FloatPrecision_tF6B76A9F4B20E5525B4B38902AA661AAB9E199F5 
{
public:
	// System.Int32 Unity.Burst.FloatPrecision::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(FloatPrecision_tF6B76A9F4B20E5525B4B38902AA661AAB9E199F5, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Unity.Burst.FunctionPointer`1<System.Object>
struct  FunctionPointer_1_t34D59AD2EA448B624FAA01B7CC28902A058C40A9 
{
public:
	// System.IntPtr Unity.Burst.FunctionPointer`1::_ptr
	intptr_t ____ptr_0;

public:
	inline static int32_t get_offset_of__ptr_0() { return static_cast<int32_t>(offsetof(FunctionPointer_1_t34D59AD2EA448B624FAA01B7CC28902A058C40A9, ____ptr_0)); }
	inline intptr_t get__ptr_0() const { return ____ptr_0; }
	inline intptr_t* get_address_of__ptr_0() { return &____ptr_0; }
	inline void set__ptr_0(intptr_t value)
	{
		____ptr_0 = value;
	}
};


// Unity.Burst.FunctionPointer`1<Unity.Burst.BurstRuntime_NativeLogDelegate>
struct  FunctionPointer_1_t3BAD2EA113092DD39EAB68EB32E8F57DD46E6C04 
{
public:
	// System.IntPtr Unity.Burst.FunctionPointer`1::_ptr
	intptr_t ____ptr_0;

public:
	inline static int32_t get_offset_of__ptr_0() { return static_cast<int32_t>(offsetof(FunctionPointer_1_t3BAD2EA113092DD39EAB68EB32E8F57DD46E6C04, ____ptr_0)); }
	inline intptr_t get__ptr_0() const { return ____ptr_0; }
	inline intptr_t* get_address_of__ptr_0() { return &____ptr_0; }
	inline void set__ptr_0(intptr_t value)
	{
		____ptr_0 = value;
	}
};


// System.MulticastDelegate
struct  MulticastDelegate_t  : public Delegate_t
{
public:
	// System.Delegate[] System.MulticastDelegate::delegates
	DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8* ___delegates_11;

public:
	inline static int32_t get_offset_of_delegates_11() { return static_cast<int32_t>(offsetof(MulticastDelegate_t, ___delegates_11)); }
	inline DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8* get_delegates_11() const { return ___delegates_11; }
	inline DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8** get_address_of_delegates_11() { return &___delegates_11; }
	inline void set_delegates_11(DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8* value)
	{
		___delegates_11 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___delegates_11), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.MulticastDelegate
struct MulticastDelegate_t_marshaled_pinvoke : public Delegate_t_marshaled_pinvoke
{
	Delegate_t_marshaled_pinvoke** ___delegates_11;
};
// Native definition for COM marshalling of System.MulticastDelegate
struct MulticastDelegate_t_marshaled_com : public Delegate_t_marshaled_com
{
	Delegate_t_marshaled_com** ___delegates_11;
};

// System.SystemException
struct  SystemException_tC551B4D6EE3772B5F32C71EE8C719F4B43ECCC62  : public Exception_t
{
public:

public:
};


// System.Type
struct  Type_t  : public MemberInfo_t
{
public:
	// System.RuntimeTypeHandle System.Type::_impl
	RuntimeTypeHandle_tC33965ADA3E041E0C94AF05E5CB527B56482CEF9  ____impl_9;

public:
	inline static int32_t get_offset_of__impl_9() { return static_cast<int32_t>(offsetof(Type_t, ____impl_9)); }
	inline RuntimeTypeHandle_tC33965ADA3E041E0C94AF05E5CB527B56482CEF9  get__impl_9() const { return ____impl_9; }
	inline RuntimeTypeHandle_tC33965ADA3E041E0C94AF05E5CB527B56482CEF9 * get_address_of__impl_9() { return &____impl_9; }
	inline void set__impl_9(RuntimeTypeHandle_tC33965ADA3E041E0C94AF05E5CB527B56482CEF9  value)
	{
		____impl_9 = value;
	}
};

struct Type_t_StaticFields
{
public:
	// System.Reflection.MemberFilter System.Type::FilterAttribute
	MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81 * ___FilterAttribute_0;
	// System.Reflection.MemberFilter System.Type::FilterName
	MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81 * ___FilterName_1;
	// System.Reflection.MemberFilter System.Type::FilterNameIgnoreCase
	MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81 * ___FilterNameIgnoreCase_2;
	// System.Object System.Type::Missing
	RuntimeObject * ___Missing_3;
	// System.Char System.Type::Delimiter
	Il2CppChar ___Delimiter_4;
	// System.Type[] System.Type::EmptyTypes
	TypeU5BU5D_t85B10489E46F06CEC7C4B1CCBD0E01FAB6649755* ___EmptyTypes_5;
	// System.Reflection.Binder System.Type::defaultBinder
	Binder_t2BEE27FD84737D1E79BC47FD67F6D3DD2F2DDA30 * ___defaultBinder_6;

public:
	inline static int32_t get_offset_of_FilterAttribute_0() { return static_cast<int32_t>(offsetof(Type_t_StaticFields, ___FilterAttribute_0)); }
	inline MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81 * get_FilterAttribute_0() const { return ___FilterAttribute_0; }
	inline MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81 ** get_address_of_FilterAttribute_0() { return &___FilterAttribute_0; }
	inline void set_FilterAttribute_0(MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81 * value)
	{
		___FilterAttribute_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___FilterAttribute_0), (void*)value);
	}

	inline static int32_t get_offset_of_FilterName_1() { return static_cast<int32_t>(offsetof(Type_t_StaticFields, ___FilterName_1)); }
	inline MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81 * get_FilterName_1() const { return ___FilterName_1; }
	inline MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81 ** get_address_of_FilterName_1() { return &___FilterName_1; }
	inline void set_FilterName_1(MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81 * value)
	{
		___FilterName_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___FilterName_1), (void*)value);
	}

	inline static int32_t get_offset_of_FilterNameIgnoreCase_2() { return static_cast<int32_t>(offsetof(Type_t_StaticFields, ___FilterNameIgnoreCase_2)); }
	inline MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81 * get_FilterNameIgnoreCase_2() const { return ___FilterNameIgnoreCase_2; }
	inline MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81 ** get_address_of_FilterNameIgnoreCase_2() { return &___FilterNameIgnoreCase_2; }
	inline void set_FilterNameIgnoreCase_2(MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81 * value)
	{
		___FilterNameIgnoreCase_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___FilterNameIgnoreCase_2), (void*)value);
	}

	inline static int32_t get_offset_of_Missing_3() { return static_cast<int32_t>(offsetof(Type_t_StaticFields, ___Missing_3)); }
	inline RuntimeObject * get_Missing_3() const { return ___Missing_3; }
	inline RuntimeObject ** get_address_of_Missing_3() { return &___Missing_3; }
	inline void set_Missing_3(RuntimeObject * value)
	{
		___Missing_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Missing_3), (void*)value);
	}

	inline static int32_t get_offset_of_Delimiter_4() { return static_cast<int32_t>(offsetof(Type_t_StaticFields, ___Delimiter_4)); }
	inline Il2CppChar get_Delimiter_4() const { return ___Delimiter_4; }
	inline Il2CppChar* get_address_of_Delimiter_4() { return &___Delimiter_4; }
	inline void set_Delimiter_4(Il2CppChar value)
	{
		___Delimiter_4 = value;
	}

	inline static int32_t get_offset_of_EmptyTypes_5() { return static_cast<int32_t>(offsetof(Type_t_StaticFields, ___EmptyTypes_5)); }
	inline TypeU5BU5D_t85B10489E46F06CEC7C4B1CCBD0E01FAB6649755* get_EmptyTypes_5() const { return ___EmptyTypes_5; }
	inline TypeU5BU5D_t85B10489E46F06CEC7C4B1CCBD0E01FAB6649755** get_address_of_EmptyTypes_5() { return &___EmptyTypes_5; }
	inline void set_EmptyTypes_5(TypeU5BU5D_t85B10489E46F06CEC7C4B1CCBD0E01FAB6649755* value)
	{
		___EmptyTypes_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___EmptyTypes_5), (void*)value);
	}

	inline static int32_t get_offset_of_defaultBinder_6() { return static_cast<int32_t>(offsetof(Type_t_StaticFields, ___defaultBinder_6)); }
	inline Binder_t2BEE27FD84737D1E79BC47FD67F6D3DD2F2DDA30 * get_defaultBinder_6() const { return ___defaultBinder_6; }
	inline Binder_t2BEE27FD84737D1E79BC47FD67F6D3DD2F2DDA30 ** get_address_of_defaultBinder_6() { return &___defaultBinder_6; }
	inline void set_defaultBinder_6(Binder_t2BEE27FD84737D1E79BC47FD67F6D3DD2F2DDA30 * value)
	{
		___defaultBinder_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultBinder_6), (void*)value);
	}
};


// Unity.Burst.BurstCompileAttribute
struct  BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1  : public Attribute_t037CA9D9F3B742C063DB364D2EEBBF9FC5772C71
{
public:
	// Unity.Burst.FloatMode Unity.Burst.BurstCompileAttribute::<FloatMode>k__BackingField
	int32_t ___U3CFloatModeU3Ek__BackingField_0;
	// Unity.Burst.FloatPrecision Unity.Burst.BurstCompileAttribute::<FloatPrecision>k__BackingField
	int32_t ___U3CFloatPrecisionU3Ek__BackingField_1;
	// System.Boolean Unity.Burst.BurstCompileAttribute::<CompileSynchronously>k__BackingField
	bool ___U3CCompileSynchronouslyU3Ek__BackingField_2;

public:
	inline static int32_t get_offset_of_U3CFloatModeU3Ek__BackingField_0() { return static_cast<int32_t>(offsetof(BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1, ___U3CFloatModeU3Ek__BackingField_0)); }
	inline int32_t get_U3CFloatModeU3Ek__BackingField_0() const { return ___U3CFloatModeU3Ek__BackingField_0; }
	inline int32_t* get_address_of_U3CFloatModeU3Ek__BackingField_0() { return &___U3CFloatModeU3Ek__BackingField_0; }
	inline void set_U3CFloatModeU3Ek__BackingField_0(int32_t value)
	{
		___U3CFloatModeU3Ek__BackingField_0 = value;
	}

	inline static int32_t get_offset_of_U3CFloatPrecisionU3Ek__BackingField_1() { return static_cast<int32_t>(offsetof(BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1, ___U3CFloatPrecisionU3Ek__BackingField_1)); }
	inline int32_t get_U3CFloatPrecisionU3Ek__BackingField_1() const { return ___U3CFloatPrecisionU3Ek__BackingField_1; }
	inline int32_t* get_address_of_U3CFloatPrecisionU3Ek__BackingField_1() { return &___U3CFloatPrecisionU3Ek__BackingField_1; }
	inline void set_U3CFloatPrecisionU3Ek__BackingField_1(int32_t value)
	{
		___U3CFloatPrecisionU3Ek__BackingField_1 = value;
	}

	inline static int32_t get_offset_of_U3CCompileSynchronouslyU3Ek__BackingField_2() { return static_cast<int32_t>(offsetof(BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1, ___U3CCompileSynchronouslyU3Ek__BackingField_2)); }
	inline bool get_U3CCompileSynchronouslyU3Ek__BackingField_2() const { return ___U3CCompileSynchronouslyU3Ek__BackingField_2; }
	inline bool* get_address_of_U3CCompileSynchronouslyU3Ek__BackingField_2() { return &___U3CCompileSynchronouslyU3Ek__BackingField_2; }
	inline void set_U3CCompileSynchronouslyU3Ek__BackingField_2(bool value)
	{
		___U3CCompileSynchronouslyU3Ek__BackingField_2 = value;
	}
};


// System.Action
struct  Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6  : public MulticastDelegate_t
{
public:

public:
};


// System.ArgumentException
struct  ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00  : public SystemException_tC551B4D6EE3772B5F32C71EE8C719F4B43ECCC62
{
public:
	// System.String System.ArgumentException::m_paramName
	String_t* ___m_paramName_17;

public:
	inline static int32_t get_offset_of_m_paramName_17() { return static_cast<int32_t>(offsetof(ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00, ___m_paramName_17)); }
	inline String_t* get_m_paramName_17() const { return ___m_paramName_17; }
	inline String_t** get_address_of_m_paramName_17() { return &___m_paramName_17; }
	inline void set_m_paramName_17(String_t* value)
	{
		___m_paramName_17 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_paramName_17), (void*)value);
	}
};


// System.AsyncCallback
struct  AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA  : public MulticastDelegate_t
{
public:

public:
};


// System.InvalidOperationException
struct  InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB  : public SystemException_tC551B4D6EE3772B5F32C71EE8C719F4B43ECCC62
{
public:

public:
};


// Unity.Burst.BurstCompiler_BurstCompilerHelper_IsBurstEnabledDelegate
struct  IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Burst.BurstRuntime_NativeLogDelegate
struct  NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182  : public MulticastDelegate_t
{
public:

public:
};


// System.ArgumentNullException
struct  ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB  : public ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00
{
public:

public:
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
// System.String[]
struct StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) String_t* m_Items[1];

public:
	inline String_t* GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline String_t** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, String_t* value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline String_t* GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline String_t** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, String_t* value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// System.Delegate[]
struct DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Delegate_t * m_Items[1];

public:
	inline Delegate_t * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Delegate_t ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Delegate_t * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Delegate_t * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Delegate_t ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Delegate_t * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};


// !!0 System.Reflection.CustomAttributeExtensions::GetCustomAttribute<System.Object>(System.Reflection.MemberInfo)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * CustomAttributeExtensions_GetCustomAttribute_TisRuntimeObject_mCFC7690084D2E45FB609A39D2DD9C11B0195D052_gshared (MemberInfo_t * ___element0, const RuntimeMethod* method);
// T& Unity.Burst.SharedStatic`1<Unity.Burst.FunctionPointer`1<System.Object>>::get_Data()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR FunctionPointer_1_t34D59AD2EA448B624FAA01B7CC28902A058C40A9 * SharedStatic_1_get_Data_m651496F7784F7CB65AA05AFC0F5240DAC8E67055_gshared (SharedStatic_1_t2EB4786D756326416AA992B56495A5243927424C * __this, const RuntimeMethod* method);
// T Unity.Burst.FunctionPointer`1<System.Object>::get_Invoke()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * FunctionPointer_1_get_Invoke_m142905CFEEF6A1EC8290685C247CBDA721AFA95D_gshared (FunctionPointer_1_t34D59AD2EA448B624FAA01B7CC28902A058C40A9 * __this, const RuntimeMethod* method);
// System.IntPtr System.Runtime.InteropServices.Marshal::GetFunctionPointerForDelegate<System.Object>(!!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t Marshal_GetFunctionPointerForDelegate_TisRuntimeObject_m352EF9B202EE82C0ACF5978B202F0F2F5E48D2EE_gshared (RuntimeObject * ___d0, const RuntimeMethod* method);
// System.Void Unity.Burst.FunctionPointer`1<System.Object>::.ctor(System.IntPtr)
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void FunctionPointer_1__ctor_mCF847800D918BA18150DD1FD8F9A6FA34C2DD9F4_gshared_inline (FunctionPointer_1_t34D59AD2EA448B624FAA01B7CC28902A058C40A9 * __this, intptr_t ___ptr0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int64,System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2__ctor_m8333BBB4DDAE242DCF8984A6A5D7A2E469405BDB_gshared (Dictionary_2_t240BB5F785CC3B2A17B14447F3C0E0BB6AAB8E26 * __this, const RuntimeMethod* method);
// Unity.Burst.SharedStatic`1<T> Unity.Burst.SharedStatic`1<Unity.Burst.FunctionPointer`1<System.Object>>::GetOrCreate<System.Object>(System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SharedStatic_1_t2EB4786D756326416AA992B56495A5243927424C  SharedStatic_1_GetOrCreate_TisRuntimeObject_m3E05C5DE285F777960722DB053F25E0F8602AD7B_gshared (uint32_t ___alignment0, const RuntimeMethod* method);

// System.Void System.Attribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Attribute__ctor_m5C1862A7DFC2C25A4797A8C5F681FBB5CB53ECE1 (Attribute_t037CA9D9F3B742C063DB364D2EEBBF9FC5772C71 * __this, const RuntimeMethod* method);
// System.Void Unity.Burst.BurstCompileAttribute::set_FloatMode(Unity.Burst.FloatMode)
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void BurstCompileAttribute_set_FloatMode_m87B9F1667557E4A62D1564F6BBCE9C0DD983784F_inline (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * __this, int32_t ___value0, const RuntimeMethod* method);
// System.Void Unity.Burst.BurstCompileAttribute::set_FloatPrecision(Unity.Burst.FloatPrecision)
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void BurstCompileAttribute_set_FloatPrecision_mF892081799E15A1C1ED18CA8B4B2733F0D5DC4B3_inline (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * __this, int32_t ___value0, const RuntimeMethod* method);
// System.Void System.Threading.Monitor::Enter(System.Object,System.Boolean&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Monitor_Enter_mBEB6CC84184B46F26375EC3FC8921D16E48EA4C4 (RuntimeObject * ___obj0, bool* ___lockTaken1, const RuntimeMethod* method);
// System.Void Unity.Burst.BurstCompilerOptions::.ctor(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerOptions__ctor_m8C50F25244F5A8847E6EEF94895E61B17AB7B17D (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, bool ___isGlobal0, const RuntimeMethod* method);
// System.Void System.Threading.Monitor::Exit(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Monitor_Exit_mA776B403DA88AC77CDEEF67AB9F0D0E77ABD254A (RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Void System.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Void Unity.Burst.BurstCompilerOptions::set_EnableBurstCompilation(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerOptions_set_EnableBurstCompilation_m2C0458F30D5BAF07E4BACA9835A6AADC0ECB90BF (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, bool ___value0, const RuntimeMethod* method);
// System.Void Unity.Burst.BurstCompilerOptions::set_EnableBurstSafetyChecks(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerOptions_set_EnableBurstSafetyChecks_m77E05637197E1564EBF921D3360FE58FDE0AD7C7 (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, bool ___value0, const RuntimeMethod* method);
// System.Boolean Unity.Burst.BurstCompilerOptions::get_IsGlobal()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool BurstCompilerOptions_get_IsGlobal_m90DDCB92AAF9312ADB8B243151C3BEC19AEA0F97_inline (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, const RuntimeMethod* method);
// System.Void Unity.Jobs.LowLevel.Unsafe.JobsUtility::set_JobCompilerEnabled(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void JobsUtility_set_JobCompilerEnabled_m4B38B836F8D2CBD730C1300E9D5FE04383DA54AC (bool ___value0, const RuntimeMethod* method);
// System.Void Unity.Burst.BurstCompiler::Enable()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompiler_Enable_m4BF5BBC8650309FA456F627D24329C611D3BCC47 (const RuntimeMethod* method);
// System.Void Unity.Burst.BurstCompilerOptions::MaybeTriggerRecompilation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerOptions_MaybeTriggerRecompilation_mC4F331073D5318A674AC176209A3ADE68756F79C (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, const RuntimeMethod* method);
// System.Void Unity.Burst.BurstCompiler::Disable()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompiler_Disable_m94FE9CD9E268BCA0692C08B219F21FAE48AAF90C (const RuntimeMethod* method);
// System.Void Unity.Burst.BurstCompilerOptions::OnOptionsChanged()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerOptions_OnOptionsChanged_m48DD95130188EC8554576E89025CBCEA71FDC731 (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, const RuntimeMethod* method);
// System.Boolean System.Reflection.MemberInfo::op_Equality(System.Reflection.MemberInfo,System.Reflection.MemberInfo)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MemberInfo_op_Equality_mE9FA8D3493294DDF178B8E8150E76C94F1CD03A9 (MemberInfo_t * ___left0, MemberInfo_t * ___right1, const RuntimeMethod* method);
// Unity.Burst.BurstCompileAttribute Unity.Burst.BurstCompilerOptions::GetBurstCompileAttribute(System.Reflection.MemberInfo)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * BurstCompilerOptions_GetBurstCompileAttribute_mF55455921BF75D6447C87EB4F5B3E8CA69DC47A6 (MemberInfo_t * ___memberInfo0, const RuntimeMethod* method);
// !!0 System.Reflection.CustomAttributeExtensions::GetCustomAttribute<Unity.Burst.BurstCompileAttribute>(System.Reflection.MemberInfo)
inline BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * CustomAttributeExtensions_GetCustomAttribute_TisBurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1_m9B8B3869855183A66586F91FE26BFB947E0A919F (MemberInfo_t * ___element0, const RuntimeMethod* method)
{
	return ((  BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * (*) (MemberInfo_t *, const RuntimeMethod*))CustomAttributeExtensions_GetCustomAttribute_TisRuntimeObject_mCFC7690084D2E45FB609A39D2DD9C11B0195D052_gshared)(___element0, method);
}
// System.Collections.Generic.IEnumerable`1<System.Attribute> System.Reflection.CustomAttributeExtensions::GetCustomAttributes(System.Reflection.MemberInfo)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* CustomAttributeExtensions_GetCustomAttributes_m9269C5EF7E5168EFDCE4D620B8FA890DF6B3E65B (MemberInfo_t * ___element0, const RuntimeMethod* method);
// System.Type System.Object::GetType()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Type_t * Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B (RuntimeObject * __this, const RuntimeMethod* method);
// System.Boolean System.String::op_Equality(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool String_op_Equality_m2B91EE68355F142F67095973D32EB5828B7B73CB (String_t* ___a0, String_t* ___b1, const RuntimeMethod* method);
// System.Void Unity.Burst.BurstCompileAttribute::.ctor(Unity.Burst.FloatPrecision,Unity.Burst.FloatMode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompileAttribute__ctor_m17500B823B3C461F030F57D6D0F32C5C3030113E (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * __this, int32_t ___floatPrecision0, int32_t ___floatMode1, const RuntimeMethod* method);
// System.Void Unity.Burst.BurstCompileAttribute::set_CompileSynchronously(System.Boolean)
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void BurstCompileAttribute_set_CompileSynchronously_mB83EBCE3125C95CB2A5B429B07F66F2310056025_inline (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * __this, bool ___value0, const RuntimeMethod* method);
// System.Void System.ArgumentNullException::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ArgumentNullException__ctor_m81AB157B93BFE2FBFDB08B88F84B444293042F97 (ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB * __this, String_t* ___paramName0, const RuntimeMethod* method);
// System.Boolean Unity.Burst.BurstCompilerOptions::TryGetAttribute(System.Reflection.MemberInfo,Unity.Burst.BurstCompileAttribute&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool BurstCompilerOptions_TryGetAttribute_m515A5B89905066961F2AF4D93D37189F97CB7886 (MemberInfo_t * ___member0, BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 ** ___attribute1, const RuntimeMethod* method);
// System.Action Unity.Burst.BurstCompilerOptions::get_OptionsChanged()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * BurstCompilerOptions_get_OptionsChanged_mD6BBC85F20B1732F0C570A7994AC5BDF9AE164EE_inline (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, const RuntimeMethod* method);
// System.Void System.Action::Invoke()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Action_Invoke_m3FFA5BE3D64F0FF8E1E1CB6F953913FADB5EB89E (Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * __this, const RuntimeMethod* method);
// System.String System.Environment::get_CurrentDirectory()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Environment_get_CurrentDirectory_m8D0A1C500E08D9D4A48A24258093CD39ED18935C (const RuntimeMethod* method);
// System.String System.IO.Path::Combine(System.String,System.String,System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Path_Combine_mDEAC65F07A793450CFB9D0086463CAEBD28D098E (String_t* ___path10, String_t* ___path21, String_t* ___path32, String_t* ___path43, const RuntimeMethod* method);
// System.String[] System.Environment::GetCommandLineArgs()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* Environment_GetCommandLineArgs_m12F7E5CE17BB9D41283D95ADF989C4F75463C329 (const RuntimeMethod* method);
// System.Void Unity.Burst.BurstRuntime::ManagedNativeLogImpl(System.Byte*,System.Int32,System.Byte*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstRuntime_ManagedNativeLogImpl_m1F8647DF0D27A6D58B855F3341D46D664CFF0D7F (uint8_t* ___message0, int32_t ___logType1, uint8_t* ___filename2, int32_t ___lineNumber3, const RuntimeMethod* method);
// System.Int32 Unity.Burst.BurstRuntime::HashStringWithFNV1A32(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t BurstRuntime_HashStringWithFNV1A32_m2A66CE121AF66D19756D5B9065F18BF232CA1C4A (String_t* ___text0, const RuntimeMethod* method);
// System.Int64 Unity.Burst.BurstRuntime::HashStringWithFNV1A64(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t BurstRuntime_HashStringWithFNV1A64_m36354952510DAE5551D8576CAEE0CC23BC49864E (String_t* ___text0, const RuntimeMethod* method);
// System.Char System.String::get_Chars(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppChar String_get_Chars_m9B1A5E4C8D70AA33A60F03735AF7B77AB9DBBA70 (String_t* __this, int32_t ___index0, const RuntimeMethod* method);
// System.Int32 System.String::get_Length()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t String_get_Length_m129FC0ADA02FECBED3C0B1A809AE84A5AEE1CF09_inline (String_t* __this, const RuntimeMethod* method);
// T& Unity.Burst.SharedStatic`1<Unity.Burst.FunctionPointer`1<Unity.Burst.BurstRuntime/NativeLogDelegate>>::get_Data()
inline FunctionPointer_1_t3BAD2EA113092DD39EAB68EB32E8F57DD46E6C04 * SharedStatic_1_get_Data_m871D2E22DD229D10E8D37902879F825ED4A4CD3F (SharedStatic_1_tB7D634D22A39EF1E5DEADA4A7C833FBA010D2417 * __this, const RuntimeMethod* method)
{
	return ((  FunctionPointer_1_t3BAD2EA113092DD39EAB68EB32E8F57DD46E6C04 * (*) (SharedStatic_1_tB7D634D22A39EF1E5DEADA4A7C833FBA010D2417 *, const RuntimeMethod*))SharedStatic_1_get_Data_m651496F7784F7CB65AA05AFC0F5240DAC8E67055_gshared)(__this, method);
}
// T Unity.Burst.FunctionPointer`1<Unity.Burst.BurstRuntime/NativeLogDelegate>::get_Invoke()
inline NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * FunctionPointer_1_get_Invoke_m659D9685A98FA383A19E4C692F098FBCD3564904 (FunctionPointer_1_t3BAD2EA113092DD39EAB68EB32E8F57DD46E6C04 * __this, const RuntimeMethod* method)
{
	return ((  NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * (*) (FunctionPointer_1_t3BAD2EA113092DD39EAB68EB32E8F57DD46E6C04 *, const RuntimeMethod*))FunctionPointer_1_get_Invoke_m142905CFEEF6A1EC8290685C247CBDA721AFA95D_gshared)(__this, method);
}
// System.Void Unity.Burst.BurstRuntime/NativeLogDelegate::Invoke(System.Byte*,System.Int32,System.Byte*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NativeLogDelegate_Invoke_m291E2A0DAA99C8F052A4646144830E69BC61A4C2 (NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * __this, uint8_t* ___message0, int32_t ___logType1, uint8_t* ___filename2, int32_t ___lineNumber3, const RuntimeMethod* method);
// System.Text.Encoding System.Text.Encoding::get_UTF8()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * Encoding_get_UTF8_mC877FB3137BBD566AEE7B15F9BF61DC4EF8F5E5E (const RuntimeMethod* method);
// System.String System.Text.Encoding::GetString(System.Byte*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Encoding_GetString_m9163935E9BF17372B6F547A3A3818D8BFE23A3AF (Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * __this, uint8_t* ___bytes0, int32_t ___byteCount1, const RuntimeMethod* method);
// System.Void UnityEngine.Debug::LogWarning(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_LogWarning_m24085D883C9E74D7AB423F0625E13259923960E7 (RuntimeObject * ___message0, const RuntimeMethod* method);
// System.Void UnityEngine.Debug::LogError(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_LogError_m8850D65592770A364D494025FF3A73E8D4D70485 (RuntimeObject * ___message0, const RuntimeMethod* method);
// System.Void UnityEngine.Debug::Log(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_Log_mC26E5AD0D8D156C7FFD173AA15827F69225E9DB8 (RuntimeObject * ___message0, const RuntimeMethod* method);
// System.Void Unity.Burst.BurstRuntime/NativeLogDelegate::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NativeLogDelegate__ctor_m836B513C92DEAE6BBC4AB75FDA9231BAD1DA01BD (NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method);
// System.IntPtr System.Runtime.InteropServices.Marshal::GetFunctionPointerForDelegate<Unity.Burst.BurstRuntime/NativeLogDelegate>(!!0)
inline intptr_t Marshal_GetFunctionPointerForDelegate_TisNativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182_mDAD2FE8DBFF51AB6A1CC3027EE51628264E1EAEB (NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * ___d0, const RuntimeMethod* method)
{
	return ((  intptr_t (*) (NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 *, const RuntimeMethod*))Marshal_GetFunctionPointerForDelegate_TisRuntimeObject_m352EF9B202EE82C0ACF5978B202F0F2F5E48D2EE_gshared)(___d0, method);
}
// System.Void Unity.Burst.FunctionPointer`1<Unity.Burst.BurstRuntime/NativeLogDelegate>::.ctor(System.IntPtr)
inline void FunctionPointer_1__ctor_m56A4237238EC535C80FC92413132650C96CA61A0_inline (FunctionPointer_1_t3BAD2EA113092DD39EAB68EB32E8F57DD46E6C04 * __this, intptr_t ___ptr0, const RuntimeMethod* method)
{
	((  void (*) (FunctionPointer_1_t3BAD2EA113092DD39EAB68EB32E8F57DD46E6C04 *, intptr_t, const RuntimeMethod*))FunctionPointer_1__ctor_mCF847800D918BA18150DD1FD8F9A6FA34C2DD9F4_gshared_inline)(__this, ___ptr0, method);
}
// System.Void System.ArgumentException::.ctor(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ArgumentException__ctor_m71044C2110E357B71A1C30D2561C3F861AF1DC0D (ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 * __this, String_t* ___message0, String_t* ___paramName1, const RuntimeMethod* method);
// System.Void UnityEngine.Hash128::.ctor(System.UInt64,System.UInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Hash128__ctor_m7958BAB68961412F06FACF1F09E04B4CEAE8FB9D (Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A * __this, uint64_t ___u64_00, uint64_t ___u64_11, const RuntimeMethod* method);
// System.Void* Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory(UnityEngine.Hash128&,System.UInt32,System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* BurstCompilerService_GetOrCreateSharedMemory_m861188CFBCAC0D14519A4E88D5485ACADA2C0858 (Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A * ___key0, uint32_t ___size_of1, uint32_t ___alignment2, const RuntimeMethod* method);
// System.Void System.InvalidOperationException::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void InvalidOperationException__ctor_mC012CE552988309733C896F3FEA8249171E4402E (InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB * __this, String_t* ___message0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int64,System.Type>::.ctor()
inline void Dictionary_2__ctor_mEDDD9C0EDBF84B20D8EEC0295487B7446286353C (Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D * __this, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D *, const RuntimeMethod*))Dictionary_2__ctor_m8333BBB4DDAE242DCF8984A6A5D7A2E469405BDB_gshared)(__this, method);
}
// System.Boolean Unity.Burst.BurstCompiler/BurstCompilerHelper::IsBurstEnabled()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool BurstCompilerHelper_IsBurstEnabled_mC1B24958FFA3F3D5360E1F933CE0900E586B2A5D (const RuntimeMethod* method);
// System.Void Unity.Burst.BurstCompiler/BurstCompilerHelper::DiscardedMethod(System.Boolean&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerHelper_DiscardedMethod_mAB545E9EE144EE8946BA1025C50B0A62CC126FBC (bool* ___value0, const RuntimeMethod* method);
// System.Int32 Unity.Burst.LowLevel.BurstCompilerService::CompileAsyncDelegateMethod(System.Object,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t BurstCompilerService_CompileAsyncDelegateMethod_mA76294773B6989F87A291642A00722F5F5266726 (RuntimeObject * ___delegateMethod0, String_t* ___compilerOptions1, const RuntimeMethod* method);
// System.Void* Unity.Burst.LowLevel.BurstCompilerService::GetAsyncCompiledAsyncDelegateMethod(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* BurstCompilerService_GetAsyncCompiledAsyncDelegateMethod_m94255AE10D8C3F7B875D69D3889E029A936DE84F (int32_t ___userID0, const RuntimeMethod* method);
// System.Void Unity.Burst.BurstCompiler/BurstCompilerHelper/IsBurstEnabledDelegate::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IsBurstEnabledDelegate__ctor_m1C0571B67EF7C95E4536E122295FA8E3851ED0ED (IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method);
// System.Boolean Unity.Burst.BurstCompiler/BurstCompilerHelper::IsCompiledByBurst(System.Delegate)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool BurstCompilerHelper_IsCompiledByBurst_m8CC7FEC39A7D2459CD8AAA3DC71AC04ACEC7BFFC (Delegate_t * ___del0, const RuntimeMethod* method);
// Unity.Burst.SharedStatic`1<T> Unity.Burst.SharedStatic`1<Unity.Burst.FunctionPointer`1<Unity.Burst.BurstRuntime/NativeLogDelegate>>::GetOrCreate<Unity.Burst.BurstRuntime/LogHelper>(System.UInt32)
inline SharedStatic_1_tB7D634D22A39EF1E5DEADA4A7C833FBA010D2417  SharedStatic_1_GetOrCreate_TisLogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_m7F86F658D4E8546206B6F6A9256A36C117205E06 (uint32_t ___alignment0, const RuntimeMethod* method)
{
	return ((  SharedStatic_1_tB7D634D22A39EF1E5DEADA4A7C833FBA010D2417  (*) (uint32_t, const RuntimeMethod*))SharedStatic_1_GetOrCreate_TisRuntimeObject_m3E05C5DE285F777960722DB053F25E0F8602AD7B_gshared)(___alignment0, method);
}
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Unity.Burst.CompilerServices.AssumeRangeAttribute::.ctor(System.Int64,System.Int64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AssumeRangeAttribute__ctor_m1FE1F4857280AE42B275E543E1424E5B44C455CC (AssumeRangeAttribute_t9CB4051727958D8EC81907FC2D1E8A4672DBA1B1 * __this, int64_t ___min0, int64_t ___max1, const RuntimeMethod* method)
{
	{
		// public AssumeRangeAttribute(long min, long max) { }
		Attribute__ctor_m5C1862A7DFC2C25A4797A8C5F681FBB5CB53ECE1(__this, /*hidden argument*/NULL);
		// public AssumeRangeAttribute(long min, long max) { }
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Unity.Burst.BurstCompileAttribute::set_FloatMode(Unity.Burst.FloatMode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompileAttribute_set_FloatMode_m87B9F1667557E4A62D1564F6BBCE9C0DD983784F (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		// public FloatMode FloatMode { get; set; }
		int32_t L_0 = ___value0;
		__this->set_U3CFloatModeU3Ek__BackingField_0(L_0);
		return;
	}
}
// System.Void Unity.Burst.BurstCompileAttribute::set_FloatPrecision(Unity.Burst.FloatPrecision)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompileAttribute_set_FloatPrecision_mF892081799E15A1C1ED18CA8B4B2733F0D5DC4B3 (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		// public FloatPrecision FloatPrecision { get; set; }
		int32_t L_0 = ___value0;
		__this->set_U3CFloatPrecisionU3Ek__BackingField_1(L_0);
		return;
	}
}
// System.Void Unity.Burst.BurstCompileAttribute::set_CompileSynchronously(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompileAttribute_set_CompileSynchronously_mB83EBCE3125C95CB2A5B429B07F66F2310056025 (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * __this, bool ___value0, const RuntimeMethod* method)
{
	{
		// public bool CompileSynchronously { get; set; }
		bool L_0 = ___value0;
		__this->set_U3CCompileSynchronouslyU3Ek__BackingField_2(L_0);
		return;
	}
}
// System.Void Unity.Burst.BurstCompileAttribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompileAttribute__ctor_m75F0EF699FB9E5AD644F26B04ACE674ED2C81628 (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * __this, const RuntimeMethod* method)
{
	{
		// public BurstCompileAttribute()
		Attribute__ctor_m5C1862A7DFC2C25A4797A8C5F681FBB5CB53ECE1(__this, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void Unity.Burst.BurstCompileAttribute::.ctor(Unity.Burst.FloatPrecision,Unity.Burst.FloatMode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompileAttribute__ctor_m17500B823B3C461F030F57D6D0F32C5C3030113E (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * __this, int32_t ___floatPrecision0, int32_t ___floatMode1, const RuntimeMethod* method)
{
	{
		// public BurstCompileAttribute(FloatPrecision floatPrecision, FloatMode floatMode)
		Attribute__ctor_m5C1862A7DFC2C25A4797A8C5F681FBB5CB53ECE1(__this, /*hidden argument*/NULL);
		// FloatMode = floatMode;
		int32_t L_0 = ___floatMode1;
		BurstCompileAttribute_set_FloatMode_m87B9F1667557E4A62D1564F6BBCE9C0DD983784F_inline(__this, L_0, /*hidden argument*/NULL);
		// FloatPrecision = floatPrecision;
		int32_t L_1 = ___floatPrecision0;
		BurstCompileAttribute_set_FloatPrecision_mF892081799E15A1C1ED18CA8B4B2733F0D5DC4B3_inline(__this, L_1, /*hidden argument*/NULL);
		// }
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Unity.Burst.BurstCompilerOptions Unity.Burst.BurstCompiler::get_Options()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * BurstCompiler_get_Options_mA554F95C4DEC38EFEAAD61E81FC2A90740CA8899 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstCompiler_get_Options_mA554F95C4DEC38EFEAAD61E81FC2A90740CA8899_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject * V_0 = NULL;
	bool V_1 = false;
	BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * V_2 = NULL;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 1);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * G_B3_0 = NULL;
	BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * G_B2_0 = NULL;
	{
		// lock (GlobalLock)
		IL2CPP_RUNTIME_CLASS_INIT(BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_il2cpp_TypeInfo_var);
		RuntimeObject * L_0 = ((BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_StaticFields*)il2cpp_codegen_static_fields_for(BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_il2cpp_TypeInfo_var))->get_GlobalLock_0();
		V_0 = L_0;
		V_1 = (bool)0;
	}

IL_0008:
	try
	{ // begin try (depth: 1)
		{
			RuntimeObject * L_1 = V_0;
			Monitor_Enter_mBEB6CC84184B46F26375EC3FC8921D16E48EA4C4(L_1, (bool*)(&V_1), /*hidden argument*/NULL);
			// return _global ?? (_global = new BurstCompilerOptions(true));
			IL2CPP_RUNTIME_CLASS_INIT(BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_il2cpp_TypeInfo_var);
			BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * L_2 = ((BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_StaticFields*)il2cpp_codegen_static_fields_for(BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_il2cpp_TypeInfo_var))->get__global_1();
			BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * L_3 = L_2;
			G_B2_0 = L_3;
			if (L_3)
			{
				G_B3_0 = L_3;
				goto IL_0025;
			}
		}

IL_0018:
		{
			BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * L_4 = (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 *)il2cpp_codegen_object_new(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_il2cpp_TypeInfo_var);
			BurstCompilerOptions__ctor_m8C50F25244F5A8847E6EEF94895E61B17AB7B17D(L_4, (bool)1, /*hidden argument*/NULL);
			BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * L_5 = L_4;
			IL2CPP_RUNTIME_CLASS_INIT(BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_il2cpp_TypeInfo_var);
			((BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_StaticFields*)il2cpp_codegen_static_fields_for(BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_il2cpp_TypeInfo_var))->set__global_1(L_5);
			G_B3_0 = L_5;
		}

IL_0025:
		{
			V_2 = G_B3_0;
			IL2CPP_LEAVE(0x32, FINALLY_0028);
		}
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0028;
	}

FINALLY_0028:
	{ // begin finally (depth: 1)
		{
			bool L_6 = V_1;
			if (!L_6)
			{
				goto IL_0031;
			}
		}

IL_002b:
		{
			RuntimeObject * L_7 = V_0;
			Monitor_Exit_mA776B403DA88AC77CDEEF67AB9F0D0E77ABD254A(L_7, /*hidden argument*/NULL);
		}

IL_0031:
		{
			IL2CPP_END_FINALLY(40)
		}
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(40)
	{
		IL2CPP_JUMP_TBL(0x32, IL_0032)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0032:
	{
		// }
		BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * L_8 = V_2;
		return L_8;
	}
}
// System.Void Unity.Burst.BurstCompiler::Enable()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompiler_Enable_m4BF5BBC8650309FA456F627D24329C611D3BCC47 (const RuntimeMethod* method)
{
	{
		// }
		return;
	}
}
// System.Void Unity.Burst.BurstCompiler::Disable()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompiler_Disable_m94FE9CD9E268BCA0692C08B219F21FAE48AAF90C (const RuntimeMethod* method)
{
	{
		// }
		return;
	}
}
// System.Void Unity.Burst.BurstCompiler::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompiler__cctor_m6482E2EA0A198C07EB665E4A7A1E0923257D4FD8 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstCompiler__cctor_m6482E2EA0A198C07EB665E4A7A1E0923257D4FD8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// private static readonly object GlobalLock = new object();
		RuntimeObject * L_0 = (RuntimeObject *)il2cpp_codegen_object_new(RuntimeObject_il2cpp_TypeInfo_var);
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405(L_0, /*hidden argument*/NULL);
		((BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_StaticFields*)il2cpp_codegen_static_fields_for(BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_il2cpp_TypeInfo_var))->set_GlobalLock_0(L_0);
		// private static BurstCompilerOptions _global = null;
		((BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_StaticFields*)il2cpp_codegen_static_fields_for(BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_il2cpp_TypeInfo_var))->set__global_1((BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 *)NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Unity.Burst.BurstCompilerOptions::.ctor(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerOptions__ctor_m8C50F25244F5A8847E6EEF94895E61B17AB7B17D (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, bool ___isGlobal0, const RuntimeMethod* method)
{
	{
		// internal BurstCompilerOptions(bool isGlobal)
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405(__this, /*hidden argument*/NULL);
		// IsGlobal = isGlobal;
		bool L_0 = ___isGlobal0;
		__this->set_U3CIsGlobalU3Ek__BackingField_5(L_0);
		// EnableBurstCompilation = true;
		BurstCompilerOptions_set_EnableBurstCompilation_m2C0458F30D5BAF07E4BACA9835A6AADC0ECB90BF(__this, (bool)1, /*hidden argument*/NULL);
		// EnableBurstSafetyChecks = true;
		BurstCompilerOptions_set_EnableBurstSafetyChecks_m77E05637197E1564EBF921D3360FE58FDE0AD7C7(__this, (bool)1, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Boolean Unity.Burst.BurstCompilerOptions::get_IsGlobal()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool BurstCompilerOptions_get_IsGlobal_m90DDCB92AAF9312ADB8B243151C3BEC19AEA0F97 (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, const RuntimeMethod* method)
{
	{
		// private bool IsGlobal { get; }
		bool L_0 = __this->get_U3CIsGlobalU3Ek__BackingField_5();
		return L_0;
	}
}
// System.Boolean Unity.Burst.BurstCompilerOptions::get_EnableBurstCompilation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool BurstCompilerOptions_get_EnableBurstCompilation_mF6AF625D845CE154ADDC6B1AE86DA7103CA0EA3F (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, const RuntimeMethod* method)
{
	{
		// get => _enableBurstCompilation;
		bool L_0 = __this->get__enableBurstCompilation_3();
		return L_0;
	}
}
// System.Void Unity.Burst.BurstCompilerOptions::set_EnableBurstCompilation(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerOptions_set_EnableBurstCompilation_m2C0458F30D5BAF07E4BACA9835A6AADC0ECB90BF (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, bool ___value0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstCompilerOptions_set_EnableBurstCompilation_m2C0458F30D5BAF07E4BACA9835A6AADC0ECB90BF_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		// if (IsGlobal && ForceDisableBurstCompilation) value = false;
		bool L_0 = BurstCompilerOptions_get_IsGlobal_m90DDCB92AAF9312ADB8B243151C3BEC19AEA0F97_inline(__this, /*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_0012;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_il2cpp_TypeInfo_var);
		bool L_1 = ((BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_StaticFields*)il2cpp_codegen_static_fields_for(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_il2cpp_TypeInfo_var))->get_ForceDisableBurstCompilation_1();
		if (!L_1)
		{
			goto IL_0012;
		}
	}
	{
		// if (IsGlobal && ForceDisableBurstCompilation) value = false;
		___value0 = (bool)0;
	}

IL_0012:
	{
		// bool changed = _enableBurstCompilation != value;
		bool L_2 = __this->get__enableBurstCompilation_3();
		bool L_3 = ___value0;
		V_0 = (bool)((((int32_t)((((int32_t)L_2) == ((int32_t)L_3))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		// _enableBurstCompilation = value;
		bool L_4 = ___value0;
		__this->set__enableBurstCompilation_3(L_4);
		// if (IsGlobal)
		bool L_5 = BurstCompilerOptions_get_IsGlobal_m90DDCB92AAF9312ADB8B243151C3BEC19AEA0F97_inline(__this, /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_004c;
		}
	}
	{
		// JobsUtility.JobCompilerEnabled = value;
		bool L_6 = ___value0;
		JobsUtility_set_JobCompilerEnabled_m4B38B836F8D2CBD730C1300E9D5FE04383DA54AC(L_6, /*hidden argument*/NULL);
		// if (changed)
		bool L_7 = V_0;
		if (!L_7)
		{
			goto IL_004c;
		}
	}
	{
		// if (value)
		bool L_8 = ___value0;
		if (!L_8)
		{
			goto IL_0047;
		}
	}
	{
		// BurstCompiler.Enable();
		IL2CPP_RUNTIME_CLASS_INIT(BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_il2cpp_TypeInfo_var);
		BurstCompiler_Enable_m4BF5BBC8650309FA456F627D24329C611D3BCC47(/*hidden argument*/NULL);
		// MaybeTriggerRecompilation();
		BurstCompilerOptions_MaybeTriggerRecompilation_mC4F331073D5318A674AC176209A3ADE68756F79C(__this, /*hidden argument*/NULL);
		// }
		goto IL_004c;
	}

IL_0047:
	{
		// BurstCompiler.Disable();
		IL2CPP_RUNTIME_CLASS_INIT(BurstCompiler_t99F34BDE62961CF5A5ACEFBB6522F1343BF085D2_il2cpp_TypeInfo_var);
		BurstCompiler_Disable_m94FE9CD9E268BCA0692C08B219F21FAE48AAF90C(/*hidden argument*/NULL);
	}

IL_004c:
	{
		// if (changed)
		bool L_9 = V_0;
		if (!L_9)
		{
			goto IL_0055;
		}
	}
	{
		// OnOptionsChanged();
		BurstCompilerOptions_OnOptionsChanged_m48DD95130188EC8554576E89025CBCEA71FDC731(__this, /*hidden argument*/NULL);
	}

IL_0055:
	{
		// }
		return;
	}
}
// System.Void Unity.Burst.BurstCompilerOptions::set_EnableBurstSafetyChecks(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerOptions_set_EnableBurstSafetyChecks_m77E05637197E1564EBF921D3360FE58FDE0AD7C7 (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, bool ___value0, const RuntimeMethod* method)
{
	{
		// bool changed = _enableBurstSafetyChecks != value;
		bool L_0 = __this->get__enableBurstSafetyChecks_4();
		bool L_1 = ___value0;
		// _enableBurstSafetyChecks = value;
		bool L_2 = ___value0;
		__this->set__enableBurstSafetyChecks_4(L_2);
		// if (changed)
		if (!((((int32_t)((((int32_t)L_0) == ((int32_t)L_1))? 1 : 0)) == ((int32_t)0))? 1 : 0))
		{
			goto IL_0021;
		}
	}
	{
		// OnOptionsChanged();
		BurstCompilerOptions_OnOptionsChanged_m48DD95130188EC8554576E89025CBCEA71FDC731(__this, /*hidden argument*/NULL);
		// MaybeTriggerRecompilation();
		BurstCompilerOptions_MaybeTriggerRecompilation_mC4F331073D5318A674AC176209A3ADE68756F79C(__this, /*hidden argument*/NULL);
	}

IL_0021:
	{
		// }
		return;
	}
}
// System.Action Unity.Burst.BurstCompilerOptions::get_OptionsChanged()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * BurstCompilerOptions_get_OptionsChanged_mD6BBC85F20B1732F0C570A7994AC5BDF9AE164EE (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, const RuntimeMethod* method)
{
	{
		// internal Action OptionsChanged { get; set; }
		Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * L_0 = __this->get_U3COptionsChangedU3Ek__BackingField_6();
		return L_0;
	}
}
// System.Boolean Unity.Burst.BurstCompilerOptions::TryGetAttribute(System.Reflection.MemberInfo,Unity.Burst.BurstCompileAttribute&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool BurstCompilerOptions_TryGetAttribute_m515A5B89905066961F2AF4D93D37189F97CB7886 (MemberInfo_t * ___member0, BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 ** ___attribute1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstCompilerOptions_TryGetAttribute_m515A5B89905066961F2AF4D93D37189F97CB7886_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// attribute = null;
		BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 ** L_0 = ___attribute1;
		*((RuntimeObject **)L_0) = (RuntimeObject *)NULL;
		Il2CppCodeGenWriteBarrier((void**)(RuntimeObject **)L_0, (void*)(RuntimeObject *)NULL);
		// if (member == null)
		MemberInfo_t * L_1 = ___member0;
		bool L_2 = MemberInfo_op_Equality_mE9FA8D3493294DDF178B8E8150E76C94F1CD03A9(L_1, (MemberInfo_t *)NULL, /*hidden argument*/NULL);
		if (!L_2)
		{
			goto IL_000e;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_000e:
	{
		// attribute = GetBurstCompileAttribute(member);
		BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 ** L_3 = ___attribute1;
		MemberInfo_t * L_4 = ___member0;
		IL2CPP_RUNTIME_CLASS_INIT(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_il2cpp_TypeInfo_var);
		BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * L_5 = BurstCompilerOptions_GetBurstCompileAttribute_mF55455921BF75D6447C87EB4F5B3E8CA69DC47A6(L_4, /*hidden argument*/NULL);
		*((RuntimeObject **)L_3) = (RuntimeObject *)L_5;
		Il2CppCodeGenWriteBarrier((void**)(RuntimeObject **)L_3, (void*)(RuntimeObject *)L_5);
		// return attribute != null;
		BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 ** L_6 = ___attribute1;
		BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * L_7 = *((BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 **)L_6);
		return (bool)((!(((RuntimeObject*)(BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 *)L_7) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
	}
}
// Unity.Burst.BurstCompileAttribute Unity.Burst.BurstCompilerOptions::GetBurstCompileAttribute(System.Reflection.MemberInfo)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * BurstCompilerOptions_GetBurstCompileAttribute_mF55455921BF75D6447C87EB4F5B3E8CA69DC47A6 (MemberInfo_t * ___memberInfo0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstCompilerOptions_GetBurstCompileAttribute_mF55455921BF75D6447C87EB4F5B3E8CA69DC47A6_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * V_0 = NULL;
	RuntimeObject* V_1 = NULL;
	BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * V_2 = NULL;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 2);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		// var result = memberInfo.GetCustomAttribute<BurstCompileAttribute>();
		MemberInfo_t * L_0 = ___memberInfo0;
		BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * L_1 = CustomAttributeExtensions_GetCustomAttribute_TisBurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1_m9B8B3869855183A66586F91FE26BFB947E0A919F(L_0, /*hidden argument*/CustomAttributeExtensions_GetCustomAttribute_TisBurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1_m9B8B3869855183A66586F91FE26BFB947E0A919F_RuntimeMethod_var);
		V_0 = L_1;
		// if (result != null)
		BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * L_2 = V_0;
		if (!L_2)
		{
			goto IL_000c;
		}
	}
	{
		// return result;
		BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * L_3 = V_0;
		return L_3;
	}

IL_000c:
	{
		// foreach (var a in memberInfo.GetCustomAttributes())
		MemberInfo_t * L_4 = ___memberInfo0;
		RuntimeObject* L_5 = CustomAttributeExtensions_GetCustomAttributes_m9269C5EF7E5168EFDCE4D620B8FA890DF6B3E65B(L_4, /*hidden argument*/NULL);
		NullCheck(L_5);
		RuntimeObject* L_6 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<!0> System.Collections.Generic.IEnumerable`1<System.Attribute>::GetEnumerator() */, IEnumerable_1_t95FA6F2545E4B529565364A2F48515FE4B80AD31_il2cpp_TypeInfo_var, L_5);
		V_1 = L_6;
	}

IL_0018:
	try
	{ // begin try (depth: 1)
		{
			goto IL_0047;
		}

IL_001a:
		{
			// foreach (var a in memberInfo.GetCustomAttributes())
			RuntimeObject* L_7 = V_1;
			NullCheck(L_7);
			Attribute_t037CA9D9F3B742C063DB364D2EEBBF9FC5772C71 * L_8 = InterfaceFuncInvoker0< Attribute_t037CA9D9F3B742C063DB364D2EEBBF9FC5772C71 * >::Invoke(0 /* !0 System.Collections.Generic.IEnumerator`1<System.Attribute>::get_Current() */, IEnumerator_1_tD213EBCE7B3BCD6C10AD4828845AF17048A1D1B7_il2cpp_TypeInfo_var, L_7);
			// if (a.GetType().FullName == "Burst.Compiler.IL.Tests.TestCompilerAttribute")
			NullCheck(L_8);
			Type_t * L_9 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B(L_8, /*hidden argument*/NULL);
			NullCheck(L_9);
			String_t* L_10 = VirtFuncInvoker0< String_t* >::Invoke(26 /* System.String System.Type::get_FullName() */, L_9);
			bool L_11 = String_op_Equality_m2B91EE68355F142F67095973D32EB5828B7B73CB(L_10, _stringLiteralBEE03BDA1448C10795E764222E36CC6226EECA1A, /*hidden argument*/NULL);
			if (!L_11)
			{
				goto IL_0047;
			}
		}

IL_0036:
		{
			// return new BurstCompileAttribute(FloatPrecision.Standard, FloatMode.Default) { CompileSynchronously = true };
			BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * L_12 = (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 *)il2cpp_codegen_object_new(BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1_il2cpp_TypeInfo_var);
			BurstCompileAttribute__ctor_m17500B823B3C461F030F57D6D0F32C5C3030113E(L_12, 0, 0, /*hidden argument*/NULL);
			BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * L_13 = L_12;
			NullCheck(L_13);
			BurstCompileAttribute_set_CompileSynchronously_mB83EBCE3125C95CB2A5B429B07F66F2310056025_inline(L_13, (bool)1, /*hidden argument*/NULL);
			V_2 = L_13;
			IL2CPP_LEAVE(0x5D, FINALLY_0051);
		}

IL_0047:
		{
			// foreach (var a in memberInfo.GetCustomAttributes())
			RuntimeObject* L_14 = V_1;
			NullCheck(L_14);
			bool L_15 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t5956F3AFB7ECF1117E3BC5890E7FC7B7F7A04105_il2cpp_TypeInfo_var, L_14);
			if (L_15)
			{
				goto IL_001a;
			}
		}

IL_004f:
		{
			IL2CPP_LEAVE(0x5B, FINALLY_0051);
		}
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0051;
	}

FINALLY_0051:
	{ // begin finally (depth: 1)
		{
			RuntimeObject* L_16 = V_1;
			if (!L_16)
			{
				goto IL_005a;
			}
		}

IL_0054:
		{
			RuntimeObject* L_17 = V_1;
			NullCheck(L_17);
			InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t099785737FC6A1E3699919A94109383715A8D807_il2cpp_TypeInfo_var, L_17);
		}

IL_005a:
		{
			IL2CPP_END_FINALLY(81)
		}
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(81)
	{
		IL2CPP_JUMP_TBL(0x5D, IL_005d)
		IL2CPP_JUMP_TBL(0x5B, IL_005b)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_005b:
	{
		// return null;
		return (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 *)NULL;
	}

IL_005d:
	{
		// }
		BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * L_18 = V_2;
		return L_18;
	}
}
// System.Boolean Unity.Burst.BurstCompilerOptions::HasBurstCompileAttribute(System.Reflection.MemberInfo)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool BurstCompilerOptions_HasBurstCompileAttribute_m4EF62614BD8EAAF2015D0B19230E79523213FC9D (MemberInfo_t * ___member0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstCompilerOptions_HasBurstCompileAttribute_m4EF62614BD8EAAF2015D0B19230E79523213FC9D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * V_0 = NULL;
	{
		// if (member == null) throw new ArgumentNullException(nameof(member));
		MemberInfo_t * L_0 = ___member0;
		bool L_1 = MemberInfo_op_Equality_mE9FA8D3493294DDF178B8E8150E76C94F1CD03A9(L_0, (MemberInfo_t *)NULL, /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0014;
		}
	}
	{
		// if (member == null) throw new ArgumentNullException(nameof(member));
		ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB * L_2 = (ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB *)il2cpp_codegen_object_new(ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB_il2cpp_TypeInfo_var);
		ArgumentNullException__ctor_m81AB157B93BFE2FBFDB08B88F84B444293042F97(L_2, _stringLiteral4784CA4E159D86F8767CCC55143536AF34801074, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_2, BurstCompilerOptions_HasBurstCompileAttribute_m4EF62614BD8EAAF2015D0B19230E79523213FC9D_RuntimeMethod_var);
	}

IL_0014:
	{
		// return TryGetAttribute(member, out attr);
		MemberInfo_t * L_3 = ___member0;
		IL2CPP_RUNTIME_CLASS_INIT(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_il2cpp_TypeInfo_var);
		bool L_4 = BurstCompilerOptions_TryGetAttribute_m515A5B89905066961F2AF4D93D37189F97CB7886(L_3, (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 **)(&V_0), /*hidden argument*/NULL);
		return L_4;
	}
}
// System.Void Unity.Burst.BurstCompilerOptions::OnOptionsChanged()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerOptions_OnOptionsChanged_m48DD95130188EC8554576E89025CBCEA71FDC731 (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, const RuntimeMethod* method)
{
	Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * G_B2_0 = NULL;
	Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * G_B1_0 = NULL;
	{
		// OptionsChanged?.Invoke();
		Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * L_0 = BurstCompilerOptions_get_OptionsChanged_mD6BBC85F20B1732F0C570A7994AC5BDF9AE164EE_inline(__this, /*hidden argument*/NULL);
		Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * L_1 = L_0;
		G_B1_0 = L_1;
		if (L_1)
		{
			G_B2_0 = L_1;
			goto IL_000b;
		}
	}
	{
		return;
	}

IL_000b:
	{
		NullCheck(G_B2_0);
		Action_Invoke_m3FFA5BE3D64F0FF8E1E1CB6F953913FADB5EB89E(G_B2_0, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void Unity.Burst.BurstCompilerOptions::MaybeTriggerRecompilation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerOptions_MaybeTriggerRecompilation_mC4F331073D5318A674AC176209A3ADE68756F79C (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, const RuntimeMethod* method)
{
	{
		// }
		return;
	}
}
// System.Void Unity.Burst.BurstCompilerOptions::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerOptions__cctor_m998B13B9D2A1813E93900FFAE79A23357CB611A6 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstCompilerOptions__cctor_m998B13B9D2A1813E93900FFAE79A23357CB611A6_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* V_0 = NULL;
	int32_t V_1 = 0;
	String_t* V_2 = NULL;
	{
		// internal static readonly string DefaultCacheFolder = Path.Combine(Environment.CurrentDirectory, "Library", "BurstCache", "JIT");
		String_t* L_0 = Environment_get_CurrentDirectory_m8D0A1C500E08D9D4A48A24258093CD39ED18935C(/*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Path_tF1D95B78D57C1C1211BA6633FF2AC22FD6C48921_il2cpp_TypeInfo_var);
		String_t* L_1 = Path_Combine_mDEAC65F07A793450CFB9D0086463CAEBD28D098E(L_0, _stringLiteral02FEF73D0CEEEFD67DF0C54AE6860E3A4B589F86, _stringLiteral6B0170CB33499190716007E7ADF488D97D050004, _stringLiteralCE4CE926FE1AADB8C64C184D24F6BBABF78EAFC1, /*hidden argument*/NULL);
		((BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_StaticFields*)il2cpp_codegen_static_fields_for(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_il2cpp_TypeInfo_var))->set_DefaultCacheFolder_0(L_1);
		// foreach (var arg in Environment.GetCommandLineArgs())
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_2 = Environment_GetCommandLineArgs_m12F7E5CE17BB9D41283D95ADF989C4F75463C329(/*hidden argument*/NULL);
		V_0 = L_2;
		V_1 = 0;
		goto IL_005a;
	}

IL_0028:
	{
		// foreach (var arg in Environment.GetCommandLineArgs())
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_3 = V_0;
		int32_t L_4 = V_1;
		NullCheck(L_3);
		int32_t L_5 = L_4;
		String_t* L_6 = (L_3)->GetAt(static_cast<il2cpp_array_size_t>(L_5));
		V_2 = L_6;
		// switch (arg)
		String_t* L_7 = V_2;
		bool L_8 = String_op_Equality_m2B91EE68355F142F67095973D32EB5828B7B73CB(L_7, _stringLiteral052A90A585030A2D7669CC7D2884B0D78760C742, /*hidden argument*/NULL);
		if (L_8)
		{
			goto IL_0048;
		}
	}
	{
		String_t* L_9 = V_2;
		bool L_10 = String_op_Equality_m2B91EE68355F142F67095973D32EB5828B7B73CB(L_9, _stringLiteral79DA35A084D67D3A4C946D97765E49E456B15354, /*hidden argument*/NULL);
		if (L_10)
		{
			goto IL_0050;
		}
	}
	{
		goto IL_0056;
	}

IL_0048:
	{
		// ForceDisableBurstCompilation = true;
		((BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_StaticFields*)il2cpp_codegen_static_fields_for(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_il2cpp_TypeInfo_var))->set_ForceDisableBurstCompilation_1((bool)1);
		// break;
		goto IL_0056;
	}

IL_0050:
	{
		// ForceBurstCompilationSynchronously = false;
		((BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_StaticFields*)il2cpp_codegen_static_fields_for(BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34_il2cpp_TypeInfo_var))->set_ForceBurstCompilationSynchronously_2((bool)0);
	}

IL_0056:
	{
		int32_t L_11 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_11, (int32_t)1));
	}

IL_005a:
	{
		// foreach (var arg in Environment.GetCommandLineArgs())
		int32_t L_12 = V_1;
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_13 = V_0;
		NullCheck(L_13);
		if ((((int32_t)L_12) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_13)->max_length)))))))
		{
			goto IL_0028;
		}
	}
	{
		// }
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
extern "C" void DEFAULT_CALL ReversePInvokeWrapper_BurstRuntime_ManagedNativeLogImpl_m1F8647DF0D27A6D58B855F3341D46D664CFF0D7F(uint8_t* ___message0, int32_t ___logType1, uint8_t* ___filename2, int32_t ___lineNumber3)
{
	il2cpp::vm::ScopedThreadAttacher _vmThreadHelper;

	// Managed method invocation
	BurstRuntime_ManagedNativeLogImpl_m1F8647DF0D27A6D58B855F3341D46D664CFF0D7F(___message0, ___logType1, ___filename2, ___lineNumber3, NULL);

}
// System.Int32 Unity.Burst.BurstRuntime::GetHashCode32(System.Type)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t BurstRuntime_GetHashCode32_m4B9A69F912BBF47693AF4C21D45E66A27D550A32 (Type_t * ___type0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstRuntime_GetHashCode32_m4B9A69F912BBF47693AF4C21D45E66A27D550A32_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// return HashStringWithFNV1A32(type.AssemblyQualifiedName);
		Type_t * L_0 = ___type0;
		NullCheck(L_0);
		String_t* L_1 = VirtFuncInvoker0< String_t* >::Invoke(28 /* System.String System.Type::get_AssemblyQualifiedName() */, L_0);
		IL2CPP_RUNTIME_CLASS_INIT(BurstRuntime_t725AE7FF2003494CEB5F987F3365BA847C762735_il2cpp_TypeInfo_var);
		int32_t L_2 = BurstRuntime_HashStringWithFNV1A32_m2A66CE121AF66D19756D5B9065F18BF232CA1C4A(L_1, /*hidden argument*/NULL);
		return L_2;
	}
}
// System.Int64 Unity.Burst.BurstRuntime::GetHashCode64(System.Type)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t BurstRuntime_GetHashCode64_m18D20B8481BF87E670E8CB864EA0CA5111B8488E (Type_t * ___type0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstRuntime_GetHashCode64_m18D20B8481BF87E670E8CB864EA0CA5111B8488E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// return HashStringWithFNV1A64(type.AssemblyQualifiedName);
		Type_t * L_0 = ___type0;
		NullCheck(L_0);
		String_t* L_1 = VirtFuncInvoker0< String_t* >::Invoke(28 /* System.String System.Type::get_AssemblyQualifiedName() */, L_0);
		IL2CPP_RUNTIME_CLASS_INIT(BurstRuntime_t725AE7FF2003494CEB5F987F3365BA847C762735_il2cpp_TypeInfo_var);
		int64_t L_2 = BurstRuntime_HashStringWithFNV1A64_m36354952510DAE5551D8576CAEE0CC23BC49864E(L_1, /*hidden argument*/NULL);
		return L_2;
	}
}
// System.Int32 Unity.Burst.BurstRuntime::HashStringWithFNV1A32(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t BurstRuntime_HashStringWithFNV1A32_m2A66CE121AF66D19756D5B9065F18BF232CA1C4A (String_t* ___text0, const RuntimeMethod* method)
{
	uint32_t V_0 = 0;
	String_t* V_1 = NULL;
	int32_t V_2 = 0;
	Il2CppChar V_3 = 0x0;
	{
		// uint result = offsetBasis;
		V_0 = ((int32_t)-2128831035);
		// foreach (var c in text)
		String_t* L_0 = ___text0;
		V_1 = L_0;
		V_2 = 0;
		goto IL_0036;
	}

IL_000c:
	{
		// foreach (var c in text)
		String_t* L_1 = V_1;
		int32_t L_2 = V_2;
		NullCheck(L_1);
		Il2CppChar L_3 = String_get_Chars_m9B1A5E4C8D70AA33A60F03735AF7B77AB9DBBA70(L_1, L_2, /*hidden argument*/NULL);
		V_3 = L_3;
		// result = prime * (result ^ (byte)(c & 255));
		uint32_t L_4 = V_0;
		Il2CppChar L_5 = V_3;
		V_0 = ((int32_t)il2cpp_codegen_multiply((int32_t)((int32_t)16777619), (int32_t)((int32_t)((int32_t)L_4^(int32_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_5&(int32_t)((int32_t)255))))))))));
		// result = prime * (result ^ (byte)(c >> 8));
		uint32_t L_6 = V_0;
		Il2CppChar L_7 = V_3;
		V_0 = ((int32_t)il2cpp_codegen_multiply((int32_t)((int32_t)16777619), (int32_t)((int32_t)((int32_t)L_6^(int32_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_7>>(int32_t)8)))))))));
		int32_t L_8 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_8, (int32_t)1));
	}

IL_0036:
	{
		// foreach (var c in text)
		int32_t L_9 = V_2;
		String_t* L_10 = V_1;
		NullCheck(L_10);
		int32_t L_11 = String_get_Length_m129FC0ADA02FECBED3C0B1A809AE84A5AEE1CF09_inline(L_10, /*hidden argument*/NULL);
		if ((((int32_t)L_9) < ((int32_t)L_11)))
		{
			goto IL_000c;
		}
	}
	{
		// return (int)result;
		uint32_t L_12 = V_0;
		return L_12;
	}
}
// System.Int64 Unity.Burst.BurstRuntime::HashStringWithFNV1A64(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t BurstRuntime_HashStringWithFNV1A64_m36354952510DAE5551D8576CAEE0CC23BC49864E (String_t* ___text0, const RuntimeMethod* method)
{
	uint64_t V_0 = 0;
	String_t* V_1 = NULL;
	int32_t V_2 = 0;
	Il2CppChar V_3 = 0x0;
	{
		// ulong result = offsetBasis;
		V_0 = ((int64_t)-3750763034362895579LL);
		// foreach (var c in text)
		String_t* L_0 = ___text0;
		V_1 = L_0;
		V_2 = 0;
		goto IL_0044;
	}

IL_0010:
	{
		// foreach (var c in text)
		String_t* L_1 = V_1;
		int32_t L_2 = V_2;
		NullCheck(L_1);
		Il2CppChar L_3 = String_get_Chars_m9B1A5E4C8D70AA33A60F03735AF7B77AB9DBBA70(L_1, L_2, /*hidden argument*/NULL);
		V_3 = L_3;
		// result = prime * (result ^ (byte)(c & 255));
		uint64_t L_4 = V_0;
		Il2CppChar L_5 = V_3;
		V_0 = ((int64_t)il2cpp_codegen_multiply((int64_t)((int64_t)1099511628211LL), (int64_t)((int64_t)((int64_t)L_4^(int64_t)(((int64_t)((uint64_t)(((uint32_t)((uint32_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_5&(int32_t)((int32_t)255))))))))))))))));
		// result = prime * (result ^ (byte)(c >> 8));
		uint64_t L_6 = V_0;
		Il2CppChar L_7 = V_3;
		V_0 = ((int64_t)il2cpp_codegen_multiply((int64_t)((int64_t)1099511628211LL), (int64_t)((int64_t)((int64_t)L_6^(int64_t)(((int64_t)((uint64_t)(((uint32_t)((uint32_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_7>>(int32_t)8)))))))))))))));
		int32_t L_8 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_8, (int32_t)1));
	}

IL_0044:
	{
		// foreach (var c in text)
		int32_t L_9 = V_2;
		String_t* L_10 = V_1;
		NullCheck(L_10);
		int32_t L_11 = String_get_Length_m129FC0ADA02FECBED3C0B1A809AE84A5AEE1CF09_inline(L_10, /*hidden argument*/NULL);
		if ((((int32_t)L_9) < ((int32_t)L_11)))
		{
			goto IL_0010;
		}
	}
	{
		// return (long)result;
		uint64_t L_12 = V_0;
		return L_12;
	}
}
// System.Void Unity.Burst.BurstRuntime::Log(System.Byte*,System.Int32,System.Byte*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstRuntime_Log_mA326750916D8B27060FFE335CFE3BB74F12E8632 (uint8_t* ___message0, int32_t ___logType1, uint8_t* ___fileName2, int32_t ___lineNumber3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstRuntime_Log_mA326750916D8B27060FFE335CFE3BB74F12E8632_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// LogHelper.Instance.Data.Invoke(message, logType, fileName, lineNumber);
		IL2CPP_RUNTIME_CLASS_INIT(LogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_il2cpp_TypeInfo_var);
		FunctionPointer_1_t3BAD2EA113092DD39EAB68EB32E8F57DD46E6C04 * L_0 = SharedStatic_1_get_Data_m871D2E22DD229D10E8D37902879F825ED4A4CD3F((SharedStatic_1_tB7D634D22A39EF1E5DEADA4A7C833FBA010D2417 *)(((LogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_StaticFields*)il2cpp_codegen_static_fields_for(LogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_il2cpp_TypeInfo_var))->get_address_of_Instance_0()), /*hidden argument*/SharedStatic_1_get_Data_m871D2E22DD229D10E8D37902879F825ED4A4CD3F_RuntimeMethod_var);
		NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * L_1 = FunctionPointer_1_get_Invoke_m659D9685A98FA383A19E4C692F098FBCD3564904((FunctionPointer_1_t3BAD2EA113092DD39EAB68EB32E8F57DD46E6C04 *)L_0, /*hidden argument*/FunctionPointer_1_get_Invoke_m659D9685A98FA383A19E4C692F098FBCD3564904_RuntimeMethod_var);
		uint8_t* L_2 = ___message0;
		int32_t L_3 = ___logType1;
		uint8_t* L_4 = ___fileName2;
		int32_t L_5 = ___lineNumber3;
		NullCheck(L_1);
		NativeLogDelegate_Invoke_m291E2A0DAA99C8F052A4646144830E69BC61A4C2(L_1, (uint8_t*)(uint8_t*)L_2, L_3, (uint8_t*)(uint8_t*)L_4, L_5, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void Unity.Burst.BurstRuntime::ManagedNativeLogImpl(System.Byte*,System.Int32,System.Byte*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstRuntime_ManagedNativeLogImpl_m1F8647DF0D27A6D58B855F3341D46D664CFF0D7F (uint8_t* ___message0, int32_t ___logType1, uint8_t* ___filename2, int32_t ___lineNumber3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstRuntime_ManagedNativeLogImpl_m1F8647DF0D27A6D58B855F3341D46D664CFF0D7F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	String_t* V_1 = NULL;
	{
		// if (message == null) return;
		uint8_t* L_0 = ___message0;
		if ((!(((uintptr_t)L_0) == ((uintptr_t)(((uintptr_t)0))))))
		{
			goto IL_0006;
		}
	}
	{
		// if (message == null) return;
		return;
	}

IL_0006:
	{
		// int byteCount = 0;
		V_0 = 0;
		goto IL_000e;
	}

IL_000a:
	{
		// while (message[byteCount] != 0) byteCount++;
		int32_t L_1 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_1, (int32_t)1));
	}

IL_000e:
	{
		// while (message[byteCount] != 0) byteCount++;
		uint8_t* L_2 = ___message0;
		int32_t L_3 = V_0;
		int32_t L_4 = *((uint8_t*)((uint8_t*)il2cpp_codegen_add((intptr_t)L_2, (int32_t)L_3)));
		if (L_4)
		{
			goto IL_000a;
		}
	}
	{
		// var managedText = Encoding.UTF8.GetString(message, byteCount);
		Encoding_tE901442411E2E70039D2A4AE77FB81C3D6064827 * L_5 = Encoding_get_UTF8_mC877FB3137BBD566AEE7B15F9BF61DC4EF8F5E5E(/*hidden argument*/NULL);
		uint8_t* L_6 = ___message0;
		int32_t L_7 = V_0;
		NullCheck(L_5);
		String_t* L_8 = Encoding_GetString_m9163935E9BF17372B6F547A3A3818D8BFE23A3AF(L_5, (uint8_t*)(uint8_t*)L_6, L_7, /*hidden argument*/NULL);
		V_1 = L_8;
		// switch (logType)
		int32_t L_9 = ___logType1;
		if ((((int32_t)L_9) == ((int32_t)1)))
		{
			goto IL_002b;
		}
	}
	{
		int32_t L_10 = ___logType1;
		if ((((int32_t)L_10) == ((int32_t)2)))
		{
			goto IL_0032;
		}
	}
	{
		goto IL_0039;
	}

IL_002b:
	{
		// UnityEngine.Debug.LogWarning(managedText);
		String_t* L_11 = V_1;
		IL2CPP_RUNTIME_CLASS_INIT(Debug_tEB68BCBEB8EFD60F8043C67146DC05E7F50F374B_il2cpp_TypeInfo_var);
		Debug_LogWarning_m24085D883C9E74D7AB423F0625E13259923960E7(L_11, /*hidden argument*/NULL);
		// break;
		return;
	}

IL_0032:
	{
		// UnityEngine.Debug.LogError(managedText);
		String_t* L_12 = V_1;
		IL2CPP_RUNTIME_CLASS_INIT(Debug_tEB68BCBEB8EFD60F8043C67146DC05E7F50F374B_il2cpp_TypeInfo_var);
		Debug_LogError_m8850D65592770A364D494025FF3A73E8D4D70485(L_12, /*hidden argument*/NULL);
		// break;
		return;
	}

IL_0039:
	{
		// UnityEngine.Debug.Log(managedText);
		String_t* L_13 = V_1;
		IL2CPP_RUNTIME_CLASS_INIT(Debug_tEB68BCBEB8EFD60F8043C67146DC05E7F50F374B_il2cpp_TypeInfo_var);
		Debug_Log_mC26E5AD0D8D156C7FFD173AA15827F69225E9DB8(L_13, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void Unity.Burst.BurstRuntime::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstRuntime__cctor_mD32471BFBA84067CCC8708B7591D6B3028D6CC30 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstRuntime__cctor_mD32471BFBA84067CCC8708B7591D6B3028D6CC30_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// private static readonly unsafe NativeLogDelegate ManagedNativeLog = ManagedNativeLogImpl;
		NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * L_0 = (NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 *)il2cpp_codegen_object_new(NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182_il2cpp_TypeInfo_var);
		NativeLogDelegate__ctor_m836B513C92DEAE6BBC4AB75FDA9231BAD1DA01BD(L_0, NULL, (intptr_t)((intptr_t)BurstRuntime_ManagedNativeLogImpl_m1F8647DF0D27A6D58B855F3341D46D664CFF0D7F_RuntimeMethod_var), /*hidden argument*/NULL);
		((BurstRuntime_t725AE7FF2003494CEB5F987F3365BA847C762735_StaticFields*)il2cpp_codegen_static_fields_for(BurstRuntime_t725AE7FF2003494CEB5F987F3365BA847C762735_il2cpp_TypeInfo_var))->set_ManagedNativeLog_0(L_0);
		// LogHelper.Instance.Data = new FunctionPointer<NativeLogDelegate>(Marshal.GetFunctionPointerForDelegate(ManagedNativeLog));
		IL2CPP_RUNTIME_CLASS_INIT(LogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_il2cpp_TypeInfo_var);
		FunctionPointer_1_t3BAD2EA113092DD39EAB68EB32E8F57DD46E6C04 * L_1 = SharedStatic_1_get_Data_m871D2E22DD229D10E8D37902879F825ED4A4CD3F((SharedStatic_1_tB7D634D22A39EF1E5DEADA4A7C833FBA010D2417 *)(((LogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_StaticFields*)il2cpp_codegen_static_fields_for(LogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_il2cpp_TypeInfo_var))->get_address_of_Instance_0()), /*hidden argument*/SharedStatic_1_get_Data_m871D2E22DD229D10E8D37902879F825ED4A4CD3F_RuntimeMethod_var);
		NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * L_2 = ((BurstRuntime_t725AE7FF2003494CEB5F987F3365BA847C762735_StaticFields*)il2cpp_codegen_static_fields_for(BurstRuntime_t725AE7FF2003494CEB5F987F3365BA847C762735_il2cpp_TypeInfo_var))->get_ManagedNativeLog_0();
		IL2CPP_RUNTIME_CLASS_INIT(Marshal_tEBAFAE20369FCB1B38C49C4E27A8D8C2C4B55058_il2cpp_TypeInfo_var);
		intptr_t L_3 = Marshal_GetFunctionPointerForDelegate_TisNativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182_mDAD2FE8DBFF51AB6A1CC3027EE51628264E1EAEB(L_2, /*hidden argument*/Marshal_GetFunctionPointerForDelegate_TisNativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182_mDAD2FE8DBFF51AB6A1CC3027EE51628264E1EAEB_RuntimeMethod_var);
		FunctionPointer_1_t3BAD2EA113092DD39EAB68EB32E8F57DD46E6C04  L_4;
		memset((&L_4), 0, sizeof(L_4));
		FunctionPointer_1__ctor_m56A4237238EC535C80FC92413132650C96CA61A0_inline((&L_4), (intptr_t)L_3, /*hidden argument*/FunctionPointer_1__ctor_m56A4237238EC535C80FC92413132650C96CA61A0_RuntimeMethod_var);
		*(FunctionPointer_1_t3BAD2EA113092DD39EAB68EB32E8F57DD46E6C04 *)L_1 = L_4;
		// }
		return;
	}
}
// System.Void Unity.Burst.BurstRuntime::Initialize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstRuntime_Initialize_mA441B3F9BF1C4C667A1FFC6B41FF1092CDF22E36 (const RuntimeMethod* method)
{
	{
		// }
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Unity.Burst.NoAliasAttribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NoAliasAttribute__ctor_m65CF044E3C3FA402BC99AB689CC86F4A566E2D4A (NoAliasAttribute_t444822FD22BC82F0E365DB1CEDC4AE7FC5E7DE13 * __this, const RuntimeMethod* method)
{
	{
		Attribute__ctor_m5C1862A7DFC2C25A4797A8C5F681FBB5CB53ECE1(__this, /*hidden argument*/NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void* Unity.Burst.SharedStatic::GetOrCreateSharedStaticInternal(System.Int64,System.Int64,System.UInt32,System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* SharedStatic_GetOrCreateSharedStaticInternal_m1FDD96EE55D49263DA5EA34EB6502507F1EAD254 (int64_t ___getHashCode640, int64_t ___getSubHashCode641, uint32_t ___sizeOf2, uint32_t ___alignment3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SharedStatic_GetOrCreateSharedStaticInternal_m1FDD96EE55D49263DA5EA34EB6502507F1EAD254_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A  V_0;
	memset((&V_0), 0, sizeof(V_0));
	void* G_B4_0 = NULL;
	void* G_B3_0 = NULL;
	{
		// if (sizeOf == 0) throw new ArgumentException("sizeOf must be > 0", nameof(sizeOf));
		uint32_t L_0 = ___sizeOf2;
		if (L_0)
		{
			goto IL_0013;
		}
	}
	{
		// if (sizeOf == 0) throw new ArgumentException("sizeOf must be > 0", nameof(sizeOf));
		ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 * L_1 = (ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 *)il2cpp_codegen_object_new(ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00_il2cpp_TypeInfo_var);
		ArgumentException__ctor_m71044C2110E357B71A1C30D2561C3F861AF1DC0D(L_1, _stringLiteral8C01367F6B784D1ABC182A2E9F6A403A0506773E, _stringLiteral99558DD5BF091125B80584111852E84E79679E91, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_1, SharedStatic_GetOrCreateSharedStaticInternal_m1FDD96EE55D49263DA5EA34EB6502507F1EAD254_RuntimeMethod_var);
	}

IL_0013:
	{
		// var hash128 = new Hash128((ulong)getHashCode64, (ulong)getSubHashCode64);
		int64_t L_2 = ___getHashCode640;
		int64_t L_3 = ___getSubHashCode641;
		Hash128__ctor_m7958BAB68961412F06FACF1F09E04B4CEAE8FB9D((Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A *)(&V_0), L_2, L_3, /*hidden argument*/NULL);
		// var result = BurstCompilerService.GetOrCreateSharedMemory(ref hash128, sizeOf, alignment);
		uint32_t L_4 = ___sizeOf2;
		uint32_t L_5 = ___alignment3;
		void* L_6 = BurstCompilerService_GetOrCreateSharedMemory_m861188CFBCAC0D14519A4E88D5485ACADA2C0858((Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A *)(&V_0), L_4, L_5, /*hidden argument*/NULL);
		// if (result == null)
		void* L_7 = (void*)L_6;
		G_B3_0 = L_7;
		if ((!(((uintptr_t)L_7) == ((uintptr_t)(((uintptr_t)0))))))
		{
			G_B4_0 = L_7;
			goto IL_0035;
		}
	}
	{
		// throw new InvalidOperationException("Unable to create a SharedStatic for this key. It is likely that the same key was used to allocate a shared memory with a smaller size while the new size requested is bigger");
		InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB * L_8 = (InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB *)il2cpp_codegen_object_new(InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB_il2cpp_TypeInfo_var);
		InvalidOperationException__ctor_mC012CE552988309733C896F3FEA8249171E4402E(L_8, _stringLiteralE8A25E76E3168AD5402ECA0ECD4590B455D50C61, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_8, SharedStatic_GetOrCreateSharedStaticInternal_m1FDD96EE55D49263DA5EA34EB6502507F1EAD254_RuntimeMethod_var);
	}

IL_0035:
	{
		// return result;
		return (void*)(G_B4_0);
	}
}
// System.Void Unity.Burst.SharedStatic::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SharedStatic__cctor_m91BEC2BB24D9ABA5B5998E30913C1C0DE51E9E36 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SharedStatic__cctor_m91BEC2BB24D9ABA5B5998E30913C1C0DE51E9E36_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// private static readonly Dictionary<long, Type> HashToType = new Dictionary<long, Type>();
		Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D * L_0 = (Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D *)il2cpp_codegen_object_new(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D_il2cpp_TypeInfo_var);
		Dictionary_2__ctor_mEDDD9C0EDBF84B20D8EEC0295487B7446286353C(L_0, /*hidden argument*/Dictionary_2__ctor_mEDDD9C0EDBF84B20D8EEC0295487B7446286353C_RuntimeMethod_var);
		((SharedStatic_t4A478B3109091C26ED6F772B9BEC1EA0802B2FFC_StaticFields*)il2cpp_codegen_static_fields_for(SharedStatic_t4A478B3109091C26ED6F772B9BEC1EA0802B2FFC_il2cpp_TypeInfo_var))->set_HashToType_0(L_0);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
extern "C" int32_t CDECL ReversePInvokeWrapper_BurstCompilerHelper_IsBurstEnabled_mC1B24958FFA3F3D5360E1F933CE0900E586B2A5D()
{
	il2cpp::vm::ScopedThreadAttacher _vmThreadHelper;

	// Managed method invocation
	bool returnValue;
	returnValue = BurstCompilerHelper_IsBurstEnabled_mC1B24958FFA3F3D5360E1F933CE0900E586B2A5D(NULL);

	return static_cast<int32_t>(returnValue);
}
// System.Boolean Unity.Burst.BurstCompiler_BurstCompilerHelper::IsBurstEnabled()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool BurstCompilerHelper_IsBurstEnabled_mC1B24958FFA3F3D5360E1F933CE0900E586B2A5D (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstCompilerHelper_IsBurstEnabled_mC1B24958FFA3F3D5360E1F933CE0900E586B2A5D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		// bool result = true;
		V_0 = (bool)1;
		// DiscardedMethod(ref result);
		IL2CPP_RUNTIME_CLASS_INIT(BurstCompilerHelper_tBFAE069B1EED68EFA83AF35A2224700B2BDCB4FA_il2cpp_TypeInfo_var);
		BurstCompilerHelper_DiscardedMethod_mAB545E9EE144EE8946BA1025C50B0A62CC126FBC((bool*)(&V_0), /*hidden argument*/NULL);
		// return result;
		bool L_0 = V_0;
		return L_0;
	}
}
// System.Void Unity.Burst.BurstCompiler_BurstCompilerHelper::DiscardedMethod(System.Boolean&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerHelper_DiscardedMethod_mAB545E9EE144EE8946BA1025C50B0A62CC126FBC (bool* ___value0, const RuntimeMethod* method)
{
	{
		// value = false;
		bool* L_0 = ___value0;
		*((int8_t*)L_0) = (int8_t)0;
		// }
		return;
	}
}
// System.Boolean Unity.Burst.BurstCompiler_BurstCompilerHelper::IsCompiledByBurst(System.Delegate)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool BurstCompilerHelper_IsCompiledByBurst_m8CC7FEC39A7D2459CD8AAA3DC71AC04ACEC7BFFC (Delegate_t * ___del0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstCompilerHelper_IsCompiledByBurst_m8CC7FEC39A7D2459CD8AAA3DC71AC04ACEC7BFFC_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// var delegateMethodId = Unity.Burst.LowLevel.BurstCompilerService.CompileAsyncDelegateMethod(del, string.Empty);
		Delegate_t * L_0 = ___del0;
		String_t* L_1 = ((String_t_StaticFields*)il2cpp_codegen_static_fields_for(String_t_il2cpp_TypeInfo_var))->get_Empty_5();
		int32_t L_2 = BurstCompilerService_CompileAsyncDelegateMethod_mA76294773B6989F87A291642A00722F5F5266726(L_0, L_1, /*hidden argument*/NULL);
		// return Unity.Burst.LowLevel.BurstCompilerService.GetAsyncCompiledAsyncDelegateMethod(delegateMethodId) != (void*)0;
		void* L_3 = BurstCompilerService_GetAsyncCompiledAsyncDelegateMethod_m94255AE10D8C3F7B875D69D3889E029A936DE84F(L_2, /*hidden argument*/NULL);
		return (bool)((((int32_t)((((intptr_t)L_3) == ((intptr_t)(((intptr_t)0))))? 1 : 0)) == ((int32_t)0))? 1 : 0);
	}
}
// System.Void Unity.Burst.BurstCompiler_BurstCompilerHelper::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerHelper__cctor_mD76FFC76096643AA6180877EE5FCCC86CD37E02B (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (BurstCompilerHelper__cctor_mD76FFC76096643AA6180877EE5FCCC86CD37E02B_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// private static readonly IsBurstEnabledDelegate IsBurstEnabledImpl = new IsBurstEnabledDelegate(IsBurstEnabled);
		IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5 * L_0 = (IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5 *)il2cpp_codegen_object_new(IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5_il2cpp_TypeInfo_var);
		IsBurstEnabledDelegate__ctor_m1C0571B67EF7C95E4536E122295FA8E3851ED0ED(L_0, NULL, (intptr_t)((intptr_t)BurstCompilerHelper_IsBurstEnabled_mC1B24958FFA3F3D5360E1F933CE0900E586B2A5D_RuntimeMethod_var), /*hidden argument*/NULL);
		((BurstCompilerHelper_tBFAE069B1EED68EFA83AF35A2224700B2BDCB4FA_StaticFields*)il2cpp_codegen_static_fields_for(BurstCompilerHelper_tBFAE069B1EED68EFA83AF35A2224700B2BDCB4FA_il2cpp_TypeInfo_var))->set_IsBurstEnabledImpl_0(L_0);
		// public static readonly bool IsBurstGenerated = IsCompiledByBurst(IsBurstEnabledImpl);
		IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5 * L_1 = ((BurstCompilerHelper_tBFAE069B1EED68EFA83AF35A2224700B2BDCB4FA_StaticFields*)il2cpp_codegen_static_fields_for(BurstCompilerHelper_tBFAE069B1EED68EFA83AF35A2224700B2BDCB4FA_il2cpp_TypeInfo_var))->get_IsBurstEnabledImpl_0();
		bool L_2 = BurstCompilerHelper_IsCompiledByBurst_m8CC7FEC39A7D2459CD8AAA3DC71AC04ACEC7BFFC(L_1, /*hidden argument*/NULL);
		((BurstCompilerHelper_tBFAE069B1EED68EFA83AF35A2224700B2BDCB4FA_StaticFields*)il2cpp_codegen_static_fields_for(BurstCompilerHelper_tBFAE069B1EED68EFA83AF35A2224700B2BDCB4FA_il2cpp_TypeInfo_var))->set_IsBurstGenerated_1(L_2);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Unity.Burst.BurstRuntime_LogHelper::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void LogHelper__cctor_mA6709BC920D30A0816AB8B35E7420B14A320DDE0 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (LogHelper__cctor_mA6709BC920D30A0816AB8B35E7420B14A320DDE0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public static readonly SharedStatic<FunctionPointer<NativeLogDelegate>> Instance = SharedStatic<FunctionPointer<NativeLogDelegate>>.GetOrCreate<LogHelper>();
		SharedStatic_1_tB7D634D22A39EF1E5DEADA4A7C833FBA010D2417  L_0 = SharedStatic_1_GetOrCreate_TisLogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_m7F86F658D4E8546206B6F6A9256A36C117205E06(0, /*hidden argument*/SharedStatic_1_GetOrCreate_TisLogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_m7F86F658D4E8546206B6F6A9256A36C117205E06_RuntimeMethod_var);
		((LogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_StaticFields*)il2cpp_codegen_static_fields_for(LogHelper_t8FBF106CD32B69F1AFA858B0E7FCBF7FD5676E4A_il2cpp_TypeInfo_var))->set_Instance_0(L_0);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper_NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 (NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * __this, uint8_t* ___message0, int32_t ___logType1, uint8_t* ___filename2, int32_t ___lineNumber3, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(uint8_t*, int32_t, uint8_t*, int32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(___message0, ___logType1, ___filename2, ___lineNumber3);

}
// System.Void Unity.Burst.BurstRuntime_NativeLogDelegate::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NativeLogDelegate__ctor_m836B513C92DEAE6BBC4AB75FDA9231BAD1DA01BD (NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void Unity.Burst.BurstRuntime_NativeLogDelegate::Invoke(System.Byte*,System.Int32,System.Byte*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NativeLogDelegate_Invoke_m291E2A0DAA99C8F052A4646144830E69BC61A4C2 (NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * __this, uint8_t* ___message0, int32_t ___logType1, uint8_t* ___filename2, int32_t ___lineNumber3, const RuntimeMethod* method)
{
	DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8* delegateArrayToInvoke = __this->get_delegates_11();
	Delegate_t** delegatesToInvoke;
	il2cpp_array_size_t length;
	if (delegateArrayToInvoke != NULL)
	{
		length = delegateArrayToInvoke->max_length;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(delegateArrayToInvoke->GetAddressAtUnchecked(0));
	}
	else
	{
		length = 1;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(&__this);
	}

	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		Delegate_t* currentDelegate = delegatesToInvoke[i];
		Il2CppMethodPointer targetMethodPointer = currentDelegate->get_method_ptr_0();
		RuntimeObject* targetThis = currentDelegate->get_m_target_2();
		RuntimeMethod* targetMethod = (RuntimeMethod*)(currentDelegate->get_method_3());
		if (!il2cpp_codegen_method_is_virtual(targetMethod))
		{
			il2cpp_codegen_raise_execution_engine_exception_if_method_is_not_found(targetMethod);
		}
		bool ___methodIsStatic = MethodIsStatic(targetMethod);
		int ___parameterCount = il2cpp_codegen_method_parameter_count(targetMethod);
		if (___methodIsStatic)
		{
			if (___parameterCount == 4)
			{
				// open
				typedef void (*FunctionPointerType) (uint8_t*, int32_t, uint8_t*, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___message0, ___logType1, ___filename2, ___lineNumber3, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, uint8_t*, int32_t, uint8_t*, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___message0, ___logType1, ___filename2, ___lineNumber3, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (uint8_t*, int32_t, uint8_t*, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___message0, ___logType1, ___filename2, ___lineNumber3, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker4< uint8_t*, int32_t, uint8_t*, int32_t >::Invoke(targetMethod, targetThis, ___message0, ___logType1, ___filename2, ___lineNumber3);
					else
						GenericVirtActionInvoker4< uint8_t*, int32_t, uint8_t*, int32_t >::Invoke(targetMethod, targetThis, ___message0, ___logType1, ___filename2, ___lineNumber3);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker4< uint8_t*, int32_t, uint8_t*, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___message0, ___logType1, ___filename2, ___lineNumber3);
					else
						VirtActionInvoker4< uint8_t*, int32_t, uint8_t*, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___message0, ___logType1, ___filename2, ___lineNumber3);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, int32_t, uint8_t*, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___message0) - 1), ___logType1, ___filename2, ___lineNumber3, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, uint8_t*, int32_t, uint8_t*, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___message0, ___logType1, ___filename2, ___lineNumber3, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult Unity.Burst.BurstRuntime_NativeLogDelegate::BeginInvoke(System.Byte*,System.Int32,System.Byte*,System.Int32,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* NativeLogDelegate_BeginInvoke_m6CF8743C2116BE0A550203DFFF3594BE12FB2A28 (NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * __this, uint8_t* ___message0, int32_t ___logType1, uint8_t* ___filename2, int32_t ___lineNumber3, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback4, RuntimeObject * ___object5, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NativeLogDelegate_BeginInvoke_m6CF8743C2116BE0A550203DFFF3594BE12FB2A28_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[5] = {0};
	__d_args[0] = ___message0;
	__d_args[1] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___logType1);
	__d_args[2] = ___filename2;
	__d_args[3] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___lineNumber3);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback4, (RuntimeObject*)___object5);
}
// System.Void Unity.Burst.BurstRuntime_NativeLogDelegate::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NativeLogDelegate_EndInvoke_mD1134B3DEA647E3AE9EA319A65A6556F652F24D3 (NativeLogDelegate_t8378C3E85C02C67A1B0D1F60C96280B1EE04C182 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result0, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C  bool DelegatePInvokeWrapper_IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5 (IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5 * __this, const RuntimeMethod* method)
{
	typedef int32_t (CDECL *PInvokeFunc)();
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	int32_t returnValue = il2cppPInvokeFunc();

	return static_cast<bool>(returnValue);
}
// System.Void Unity.Burst.BurstCompiler_BurstCompilerHelper_IsBurstEnabledDelegate::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IsBurstEnabledDelegate__ctor_m1C0571B67EF7C95E4536E122295FA8E3851ED0ED (IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Boolean Unity.Burst.BurstCompiler_BurstCompilerHelper_IsBurstEnabledDelegate::Invoke()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool IsBurstEnabledDelegate_Invoke_m985ABAE72EF555F9EDC4B03FDACDAF132B56AD6A (IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5 * __this, const RuntimeMethod* method)
{
	bool result = false;
	DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8* delegateArrayToInvoke = __this->get_delegates_11();
	Delegate_t** delegatesToInvoke;
	il2cpp_array_size_t length;
	if (delegateArrayToInvoke != NULL)
	{
		length = delegateArrayToInvoke->max_length;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(delegateArrayToInvoke->GetAddressAtUnchecked(0));
	}
	else
	{
		length = 1;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(&__this);
	}

	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		Delegate_t* currentDelegate = delegatesToInvoke[i];
		Il2CppMethodPointer targetMethodPointer = currentDelegate->get_method_ptr_0();
		RuntimeObject* targetThis = currentDelegate->get_m_target_2();
		RuntimeMethod* targetMethod = (RuntimeMethod*)(currentDelegate->get_method_3());
		if (!il2cpp_codegen_method_is_virtual(targetMethod))
		{
			il2cpp_codegen_raise_execution_engine_exception_if_method_is_not_found(targetMethod);
		}
		bool ___methodIsStatic = MethodIsStatic(targetMethod);
		int ___parameterCount = il2cpp_codegen_method_parameter_count(targetMethod);
		if (___methodIsStatic)
		{
			if (___parameterCount == 0)
			{
				// open
				typedef bool (*FunctionPointerType) (const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetMethod);
			}
			else
			{
				// closed
				typedef bool (*FunctionPointerType) (void*, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker0< bool >::Invoke(targetMethod, targetThis);
					else
						result = GenericVirtFuncInvoker0< bool >::Invoke(targetMethod, targetThis);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker0< bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis);
					else
						result = VirtFuncInvoker0< bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis);
				}
			}
			else
			{
				typedef bool (*FunctionPointerType) (void*, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, targetMethod);
			}
		}
	}
	return result;
}
// System.IAsyncResult Unity.Burst.BurstCompiler_BurstCompilerHelper_IsBurstEnabledDelegate::BeginInvoke(System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* IsBurstEnabledDelegate_BeginInvoke_m82BEF01F13F57DFB377E8AF66A63ADCD23FA4678 (IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5 * __this, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback0, RuntimeObject * ___object1, const RuntimeMethod* method)
{
	void *__d_args[1] = {0};
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback0, (RuntimeObject*)___object1);
}
// System.Boolean Unity.Burst.BurstCompiler_BurstCompilerHelper_IsBurstEnabledDelegate::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool IsBurstEnabledDelegate_EndInvoke_mAC335D885BC871F4290ACB7E630D1C9A8E1D1272 (IsBurstEnabledDelegate_t678871E0385FC62AC50FBA769FDD3F0113807BD5 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result0, 0);
	return *(bool*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void BurstCompileAttribute_set_FloatMode_m87B9F1667557E4A62D1564F6BBCE9C0DD983784F_inline (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		// public FloatMode FloatMode { get; set; }
		int32_t L_0 = ___value0;
		__this->set_U3CFloatModeU3Ek__BackingField_0(L_0);
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void BurstCompileAttribute_set_FloatPrecision_mF892081799E15A1C1ED18CA8B4B2733F0D5DC4B3_inline (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		// public FloatPrecision FloatPrecision { get; set; }
		int32_t L_0 = ___value0;
		__this->set_U3CFloatPrecisionU3Ek__BackingField_1(L_0);
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool BurstCompilerOptions_get_IsGlobal_m90DDCB92AAF9312ADB8B243151C3BEC19AEA0F97_inline (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, const RuntimeMethod* method)
{
	{
		// private bool IsGlobal { get; }
		bool L_0 = __this->get_U3CIsGlobalU3Ek__BackingField_5();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void BurstCompileAttribute_set_CompileSynchronously_mB83EBCE3125C95CB2A5B429B07F66F2310056025_inline (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * __this, bool ___value0, const RuntimeMethod* method)
{
	{
		// public bool CompileSynchronously { get; set; }
		bool L_0 = ___value0;
		__this->set_U3CCompileSynchronouslyU3Ek__BackingField_2(L_0);
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * BurstCompilerOptions_get_OptionsChanged_mD6BBC85F20B1732F0C570A7994AC5BDF9AE164EE_inline (BurstCompilerOptions_t544328048213260A97D2986702A9041F4237FC34 * __this, const RuntimeMethod* method)
{
	{
		// internal Action OptionsChanged { get; set; }
		Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * L_0 = __this->get_U3COptionsChangedU3Ek__BackingField_6();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t String_get_Length_m129FC0ADA02FECBED3C0B1A809AE84A5AEE1CF09_inline (String_t* __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->get_m_stringLength_0();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void FunctionPointer_1__ctor_mCF847800D918BA18150DD1FD8F9A6FA34C2DD9F4_gshared_inline (FunctionPointer_1_t34D59AD2EA448B624FAA01B7CC28902A058C40A9 * __this, intptr_t ___ptr0, const RuntimeMethod* method)
{
	{
		// _ptr = ptr;
		intptr_t L_0 = ___ptr0;
		__this->set__ptr_0((intptr_t)L_0);
		// }
		return;
	}
}
