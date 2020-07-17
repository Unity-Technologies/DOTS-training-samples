#include "pch-cpp.hpp"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <limits>
#include <stdint.h>


template <typename T1>
struct InterfaceActionInvoker1
{
	typedef void (*Action)(void*, T1, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};

// System.AsyncCallback
struct AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA;
// System.DelegateData
struct DelegateData_t17DD30660E330C49381DAA99F934BE75CB11F288;
// System.IAsyncResult
struct IAsyncResult_tC9F97BF36FCF122D29D3101D80642278297BF370;
// UnityEngine.ISubsystem
struct ISubsystem_t64464AB5EA37383499172853FA932A96C7841789;
// UnityEngine.ISubsystemDescriptor
struct ISubsystemDescriptor_tEB935323042076ECFC076435FBD756B1E7953A14;
// UnityEngine.ISubsystemDescriptorImpl
struct ISubsystemDescriptorImpl_t5C7DB694E962FEA9DE7EE89B059F7EF3ED69F77B;
// UnityEngine.IntegratedSubsystem
struct IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002;
// UnityEngine.IntegratedSubsystemDescriptor
struct IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// UnityEngine.SubsystemDescriptor
struct SubsystemDescriptor_tF663011CB44AB1D342821BBEF7B6811E799A7245;
// System.Void
struct Void_t700C6383A2A510C2CF4DD86DABD5CA9FF70ADAC5;
// System.Collections.Generic.List`1<UnityEngine.ISubsystem>
struct List_1_t5FED37944BE574E3B297791169909363750C8A64;
// System.Collections.Generic.List`1<UnityEngine.ISubsystemDescriptor>
struct List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B;
// System.Collections.Generic.List`1<UnityEngine.ISubsystemDescriptorImpl>
struct List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C;
// System.Collections.Generic.List`1<System.Object>
struct List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5;
// System.Action
struct Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6;
// System.Delegate[]
struct DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8;
// UnityEngine.ISubsystemDescriptorImpl[]
struct ISubsystemDescriptorImplU5BU5D_t7AE7A630B75C30CB4E6FEE2FF5CCB481329D97A8;
// UnityEngine.ISubsystemDescriptor[]
struct ISubsystemDescriptorU5BU5D_t36C48FCFDCC1507B173B0867F5A6F472E69E24F5;
// UnityEngine.ISubsystem[]
struct ISubsystemU5BU5D_t6A68D7EB2FB6AB7DDEAB4A16526B361E66BE4E4E;
// System.Object[]
struct ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE;
// System.String
struct String_t;

IL2CPP_EXTERN_C RuntimeClass* ISubsystemDescriptorImpl_t5C7DB694E962FEA9DE7EE89B059F7EF3ED69F77B_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IntPtr_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* List_1_t5FED37944BE574E3B297791169909363750C8A64_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_Dispose_m5EA13D6FA77939E22384B5FA182D4266776DD244_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_Dispose_mE79C91F4CB323DA8E36737C9513F9D81728EEB75_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_Dispose_mEFBC03EAF32221CE654B48D4D21F15D5B57F1C35_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_MoveNext_m4476AE4E998C854351889A11F106494DF5282590_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_MoveNext_m6090B713C1599612E9E959204C495E69FF635A6B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_MoveNext_mBAB8D6690C8037262A8A1058928C8BB1EE9A6ED0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_get_Current_m08AAF79B6D539D33DC62A4630DD67D1D8CF22D9E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_get_Current_m25D3F1FAA6111CD2A2E1AC23462EE96164991215_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_get_Current_m59B82C82B00B6E477627FF890657E19A0596EC40_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_Add_m0CE24B326592DD284E9F4D653E3BF1891CB613A4_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_Add_m15F562F509A431E4C6172383F81BBFEC7B766A2C_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_Add_mB612D0F6235C1599F80835BA7170C7602CD44DF5_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_Clear_m2755F780FB1D50F6D26CE478AF38D661AA868589_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_Clear_mBBDF59B5B02BFD11B20EC27E5092D9C55A82179D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_GetEnumerator_m0D7B7D04AAB181B6AA7525709026E68258ED5740_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_GetEnumerator_m1CC6277818506CE6B5DFA48BD0A26E4B5FBCCE93_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_GetEnumerator_m7985F077822F6454C777325341202546D1323C91_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_RemoveAt_m87C2B6E364B977B74BB97E0D0B6D573661CB3481_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1__ctor_m11E48E7BBD9D92B1240BE22DFEC0A835B3E5545F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1__ctor_mE3E1C3F22FB1EE5389CA8E3BCE698AB8A4DF5F25_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1__ctor_mE7AF9A04C421B1097120D9179F9AE7E559E3DB83_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_get_Count_m5655FD369C25BA7B877F8C8E5139B8CAFB9A31B2_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_get_Item_m4C4EC958972680EB68F10F5666D97A09BC69B609_RuntimeMethod_var;
IL2CPP_EXTERN_C const uint32_t Internal_SubsystemDescriptors_Internal_AddDescriptor_m3BC579382C5363C54D0B37C60FE844C82AA63D1E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Internal_SubsystemDescriptors_Internal_ClearManagedDescriptors_m53D23CFAF2763F6BBFD9DFA92981C94F9CE2FE15_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Internal_SubsystemDescriptors_Internal_InitializeManagedDescriptor_m5D035D990571A95960C76DCA0AA7070CFC643911_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Internal_SubsystemDescriptors__cctor_m58666797C448C50D9788277A3FB77D29B1C411C8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Internal_SubsystemInstances_Internal_ClearManagedInstances_mCDA0963D5AE8FBA27A15EB9BFDB9FC82AB6452A4_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Internal_SubsystemInstances_Internal_GetInstanceByPtr_m5C2B49DC08EBCC5036465D8928090D71F1D420F5_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Internal_SubsystemInstances_Internal_InitializeManagedInstance_m2B53F9AACC98A102FB805164FD73F7366DAAB299_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Internal_SubsystemInstances_Internal_RemoveInstanceByPtr_mEBDEF7FC25B00E83E633298B6FB494CDDA093F26_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Internal_SubsystemInstances__cctor_m98CFC5909AD1A6B7C085D0F1FFE1D5E8BDA14B83_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SubsystemManager_Internal_ReloadSubsystemsCompleted_m25F9188AF832B1DDF30524CF460B06E06307E6D3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SubsystemManager_Internal_ReloadSubsystemsStarted_mF2B81F13FA0F97A5C0BE6FAF36B5CB4F2130ED12_MetadataUsageId;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;

struct ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// <Module>
struct  U3CModuleU3E_t7D657B68C133361A594C708A6FD672221623F690 
{
public:

public:
};


// System.Object

struct Il2CppArrayBounds;

// System.Array


// System.Collections.Generic.List`1<System.Object>
struct  List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5  : public RuntimeObject
{
public:
	// T[] System.Collections.Generic.List`1::_items
	ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* ____items_1;
	// System.Int32 System.Collections.Generic.List`1::_size
	int32_t ____size_2;
	// System.Int32 System.Collections.Generic.List`1::_version
	int32_t ____version_3;
	// System.Object System.Collections.Generic.List`1::_syncRoot
	RuntimeObject * ____syncRoot_4;

public:
	inline static int32_t get_offset_of__items_1() { return static_cast<int32_t>(offsetof(List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5, ____items_1)); }
	inline ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* get__items_1() const { return ____items_1; }
	inline ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE** get_address_of__items_1() { return &____items_1; }
	inline void set__items_1(ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* value)
	{
		____items_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____items_1), (void*)value);
	}

	inline static int32_t get_offset_of__size_2() { return static_cast<int32_t>(offsetof(List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5, ____size_2)); }
	inline int32_t get__size_2() const { return ____size_2; }
	inline int32_t* get_address_of__size_2() { return &____size_2; }
	inline void set__size_2(int32_t value)
	{
		____size_2 = value;
	}

	inline static int32_t get_offset_of__version_3() { return static_cast<int32_t>(offsetof(List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5, ____version_3)); }
	inline int32_t get__version_3() const { return ____version_3; }
	inline int32_t* get_address_of__version_3() { return &____version_3; }
	inline void set__version_3(int32_t value)
	{
		____version_3 = value;
	}

	inline static int32_t get_offset_of__syncRoot_4() { return static_cast<int32_t>(offsetof(List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5, ____syncRoot_4)); }
	inline RuntimeObject * get__syncRoot_4() const { return ____syncRoot_4; }
	inline RuntimeObject ** get_address_of__syncRoot_4() { return &____syncRoot_4; }
	inline void set__syncRoot_4(RuntimeObject * value)
	{
		____syncRoot_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_4), (void*)value);
	}
};

struct List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5_StaticFields
{
public:
	// T[] System.Collections.Generic.List`1::_emptyArray
	ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* ____emptyArray_5;

public:
	inline static int32_t get_offset_of__emptyArray_5() { return static_cast<int32_t>(offsetof(List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5_StaticFields, ____emptyArray_5)); }
	inline ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* get__emptyArray_5() const { return ____emptyArray_5; }
	inline ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE** get_address_of__emptyArray_5() { return &____emptyArray_5; }
	inline void set__emptyArray_5(ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* value)
	{
		____emptyArray_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____emptyArray_5), (void*)value);
	}
};


// System.Collections.Generic.List`1<UnityEngine.ISubsystem>
struct  List_1_t5FED37944BE574E3B297791169909363750C8A64  : public RuntimeObject
{
public:
	// T[] System.Collections.Generic.List`1::_items
	ISubsystemU5BU5D_t6A68D7EB2FB6AB7DDEAB4A16526B361E66BE4E4E* ____items_1;
	// System.Int32 System.Collections.Generic.List`1::_size
	int32_t ____size_2;
	// System.Int32 System.Collections.Generic.List`1::_version
	int32_t ____version_3;
	// System.Object System.Collections.Generic.List`1::_syncRoot
	RuntimeObject * ____syncRoot_4;

public:
	inline static int32_t get_offset_of__items_1() { return static_cast<int32_t>(offsetof(List_1_t5FED37944BE574E3B297791169909363750C8A64, ____items_1)); }
	inline ISubsystemU5BU5D_t6A68D7EB2FB6AB7DDEAB4A16526B361E66BE4E4E* get__items_1() const { return ____items_1; }
	inline ISubsystemU5BU5D_t6A68D7EB2FB6AB7DDEAB4A16526B361E66BE4E4E** get_address_of__items_1() { return &____items_1; }
	inline void set__items_1(ISubsystemU5BU5D_t6A68D7EB2FB6AB7DDEAB4A16526B361E66BE4E4E* value)
	{
		____items_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____items_1), (void*)value);
	}

	inline static int32_t get_offset_of__size_2() { return static_cast<int32_t>(offsetof(List_1_t5FED37944BE574E3B297791169909363750C8A64, ____size_2)); }
	inline int32_t get__size_2() const { return ____size_2; }
	inline int32_t* get_address_of__size_2() { return &____size_2; }
	inline void set__size_2(int32_t value)
	{
		____size_2 = value;
	}

	inline static int32_t get_offset_of__version_3() { return static_cast<int32_t>(offsetof(List_1_t5FED37944BE574E3B297791169909363750C8A64, ____version_3)); }
	inline int32_t get__version_3() const { return ____version_3; }
	inline int32_t* get_address_of__version_3() { return &____version_3; }
	inline void set__version_3(int32_t value)
	{
		____version_3 = value;
	}

	inline static int32_t get_offset_of__syncRoot_4() { return static_cast<int32_t>(offsetof(List_1_t5FED37944BE574E3B297791169909363750C8A64, ____syncRoot_4)); }
	inline RuntimeObject * get__syncRoot_4() const { return ____syncRoot_4; }
	inline RuntimeObject ** get_address_of__syncRoot_4() { return &____syncRoot_4; }
	inline void set__syncRoot_4(RuntimeObject * value)
	{
		____syncRoot_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_4), (void*)value);
	}
};

struct List_1_t5FED37944BE574E3B297791169909363750C8A64_StaticFields
{
public:
	// T[] System.Collections.Generic.List`1::_emptyArray
	ISubsystemU5BU5D_t6A68D7EB2FB6AB7DDEAB4A16526B361E66BE4E4E* ____emptyArray_5;

public:
	inline static int32_t get_offset_of__emptyArray_5() { return static_cast<int32_t>(offsetof(List_1_t5FED37944BE574E3B297791169909363750C8A64_StaticFields, ____emptyArray_5)); }
	inline ISubsystemU5BU5D_t6A68D7EB2FB6AB7DDEAB4A16526B361E66BE4E4E* get__emptyArray_5() const { return ____emptyArray_5; }
	inline ISubsystemU5BU5D_t6A68D7EB2FB6AB7DDEAB4A16526B361E66BE4E4E** get_address_of__emptyArray_5() { return &____emptyArray_5; }
	inline void set__emptyArray_5(ISubsystemU5BU5D_t6A68D7EB2FB6AB7DDEAB4A16526B361E66BE4E4E* value)
	{
		____emptyArray_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____emptyArray_5), (void*)value);
	}
};


// System.Collections.Generic.List`1<UnityEngine.ISubsystemDescriptor>
struct  List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B  : public RuntimeObject
{
public:
	// T[] System.Collections.Generic.List`1::_items
	ISubsystemDescriptorU5BU5D_t36C48FCFDCC1507B173B0867F5A6F472E69E24F5* ____items_1;
	// System.Int32 System.Collections.Generic.List`1::_size
	int32_t ____size_2;
	// System.Int32 System.Collections.Generic.List`1::_version
	int32_t ____version_3;
	// System.Object System.Collections.Generic.List`1::_syncRoot
	RuntimeObject * ____syncRoot_4;

public:
	inline static int32_t get_offset_of__items_1() { return static_cast<int32_t>(offsetof(List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B, ____items_1)); }
	inline ISubsystemDescriptorU5BU5D_t36C48FCFDCC1507B173B0867F5A6F472E69E24F5* get__items_1() const { return ____items_1; }
	inline ISubsystemDescriptorU5BU5D_t36C48FCFDCC1507B173B0867F5A6F472E69E24F5** get_address_of__items_1() { return &____items_1; }
	inline void set__items_1(ISubsystemDescriptorU5BU5D_t36C48FCFDCC1507B173B0867F5A6F472E69E24F5* value)
	{
		____items_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____items_1), (void*)value);
	}

	inline static int32_t get_offset_of__size_2() { return static_cast<int32_t>(offsetof(List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B, ____size_2)); }
	inline int32_t get__size_2() const { return ____size_2; }
	inline int32_t* get_address_of__size_2() { return &____size_2; }
	inline void set__size_2(int32_t value)
	{
		____size_2 = value;
	}

	inline static int32_t get_offset_of__version_3() { return static_cast<int32_t>(offsetof(List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B, ____version_3)); }
	inline int32_t get__version_3() const { return ____version_3; }
	inline int32_t* get_address_of__version_3() { return &____version_3; }
	inline void set__version_3(int32_t value)
	{
		____version_3 = value;
	}

	inline static int32_t get_offset_of__syncRoot_4() { return static_cast<int32_t>(offsetof(List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B, ____syncRoot_4)); }
	inline RuntimeObject * get__syncRoot_4() const { return ____syncRoot_4; }
	inline RuntimeObject ** get_address_of__syncRoot_4() { return &____syncRoot_4; }
	inline void set__syncRoot_4(RuntimeObject * value)
	{
		____syncRoot_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_4), (void*)value);
	}
};

struct List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B_StaticFields
{
public:
	// T[] System.Collections.Generic.List`1::_emptyArray
	ISubsystemDescriptorU5BU5D_t36C48FCFDCC1507B173B0867F5A6F472E69E24F5* ____emptyArray_5;

public:
	inline static int32_t get_offset_of__emptyArray_5() { return static_cast<int32_t>(offsetof(List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B_StaticFields, ____emptyArray_5)); }
	inline ISubsystemDescriptorU5BU5D_t36C48FCFDCC1507B173B0867F5A6F472E69E24F5* get__emptyArray_5() const { return ____emptyArray_5; }
	inline ISubsystemDescriptorU5BU5D_t36C48FCFDCC1507B173B0867F5A6F472E69E24F5** get_address_of__emptyArray_5() { return &____emptyArray_5; }
	inline void set__emptyArray_5(ISubsystemDescriptorU5BU5D_t36C48FCFDCC1507B173B0867F5A6F472E69E24F5* value)
	{
		____emptyArray_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____emptyArray_5), (void*)value);
	}
};


// System.Collections.Generic.List`1<UnityEngine.ISubsystemDescriptorImpl>
struct  List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C  : public RuntimeObject
{
public:
	// T[] System.Collections.Generic.List`1::_items
	ISubsystemDescriptorImplU5BU5D_t7AE7A630B75C30CB4E6FEE2FF5CCB481329D97A8* ____items_1;
	// System.Int32 System.Collections.Generic.List`1::_size
	int32_t ____size_2;
	// System.Int32 System.Collections.Generic.List`1::_version
	int32_t ____version_3;
	// System.Object System.Collections.Generic.List`1::_syncRoot
	RuntimeObject * ____syncRoot_4;

public:
	inline static int32_t get_offset_of__items_1() { return static_cast<int32_t>(offsetof(List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C, ____items_1)); }
	inline ISubsystemDescriptorImplU5BU5D_t7AE7A630B75C30CB4E6FEE2FF5CCB481329D97A8* get__items_1() const { return ____items_1; }
	inline ISubsystemDescriptorImplU5BU5D_t7AE7A630B75C30CB4E6FEE2FF5CCB481329D97A8** get_address_of__items_1() { return &____items_1; }
	inline void set__items_1(ISubsystemDescriptorImplU5BU5D_t7AE7A630B75C30CB4E6FEE2FF5CCB481329D97A8* value)
	{
		____items_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____items_1), (void*)value);
	}

	inline static int32_t get_offset_of__size_2() { return static_cast<int32_t>(offsetof(List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C, ____size_2)); }
	inline int32_t get__size_2() const { return ____size_2; }
	inline int32_t* get_address_of__size_2() { return &____size_2; }
	inline void set__size_2(int32_t value)
	{
		____size_2 = value;
	}

	inline static int32_t get_offset_of__version_3() { return static_cast<int32_t>(offsetof(List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C, ____version_3)); }
	inline int32_t get__version_3() const { return ____version_3; }
	inline int32_t* get_address_of__version_3() { return &____version_3; }
	inline void set__version_3(int32_t value)
	{
		____version_3 = value;
	}

	inline static int32_t get_offset_of__syncRoot_4() { return static_cast<int32_t>(offsetof(List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C, ____syncRoot_4)); }
	inline RuntimeObject * get__syncRoot_4() const { return ____syncRoot_4; }
	inline RuntimeObject ** get_address_of__syncRoot_4() { return &____syncRoot_4; }
	inline void set__syncRoot_4(RuntimeObject * value)
	{
		____syncRoot_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_4), (void*)value);
	}
};

struct List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C_StaticFields
{
public:
	// T[] System.Collections.Generic.List`1::_emptyArray
	ISubsystemDescriptorImplU5BU5D_t7AE7A630B75C30CB4E6FEE2FF5CCB481329D97A8* ____emptyArray_5;

public:
	inline static int32_t get_offset_of__emptyArray_5() { return static_cast<int32_t>(offsetof(List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C_StaticFields, ____emptyArray_5)); }
	inline ISubsystemDescriptorImplU5BU5D_t7AE7A630B75C30CB4E6FEE2FF5CCB481329D97A8* get__emptyArray_5() const { return ____emptyArray_5; }
	inline ISubsystemDescriptorImplU5BU5D_t7AE7A630B75C30CB4E6FEE2FF5CCB481329D97A8** get_address_of__emptyArray_5() { return &____emptyArray_5; }
	inline void set__emptyArray_5(ISubsystemDescriptorImplU5BU5D_t7AE7A630B75C30CB4E6FEE2FF5CCB481329D97A8* value)
	{
		____emptyArray_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____emptyArray_5), (void*)value);
	}
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

// UnityEngine.Internal_SubsystemDescriptors
struct  Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5  : public RuntimeObject
{
public:

public:
};

struct Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_StaticFields
{
public:
	// System.Collections.Generic.List`1<UnityEngine.ISubsystemDescriptorImpl> UnityEngine.Internal_SubsystemDescriptors::s_IntegratedSubsystemDescriptors
	List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C * ___s_IntegratedSubsystemDescriptors_0;
	// System.Collections.Generic.List`1<UnityEngine.ISubsystemDescriptor> UnityEngine.Internal_SubsystemDescriptors::s_StandaloneSubsystemDescriptors
	List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B * ___s_StandaloneSubsystemDescriptors_1;

public:
	inline static int32_t get_offset_of_s_IntegratedSubsystemDescriptors_0() { return static_cast<int32_t>(offsetof(Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_StaticFields, ___s_IntegratedSubsystemDescriptors_0)); }
	inline List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C * get_s_IntegratedSubsystemDescriptors_0() const { return ___s_IntegratedSubsystemDescriptors_0; }
	inline List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C ** get_address_of_s_IntegratedSubsystemDescriptors_0() { return &___s_IntegratedSubsystemDescriptors_0; }
	inline void set_s_IntegratedSubsystemDescriptors_0(List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C * value)
	{
		___s_IntegratedSubsystemDescriptors_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_IntegratedSubsystemDescriptors_0), (void*)value);
	}

	inline static int32_t get_offset_of_s_StandaloneSubsystemDescriptors_1() { return static_cast<int32_t>(offsetof(Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_StaticFields, ___s_StandaloneSubsystemDescriptors_1)); }
	inline List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B * get_s_StandaloneSubsystemDescriptors_1() const { return ___s_StandaloneSubsystemDescriptors_1; }
	inline List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B ** get_address_of_s_StandaloneSubsystemDescriptors_1() { return &___s_StandaloneSubsystemDescriptors_1; }
	inline void set_s_StandaloneSubsystemDescriptors_1(List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B * value)
	{
		___s_StandaloneSubsystemDescriptors_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_StandaloneSubsystemDescriptors_1), (void*)value);
	}
};


// UnityEngine.Internal_SubsystemInstances
struct  Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395  : public RuntimeObject
{
public:

public:
};

struct Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_StaticFields
{
public:
	// System.Collections.Generic.List`1<UnityEngine.ISubsystem> UnityEngine.Internal_SubsystemInstances::s_IntegratedSubsystemInstances
	List_1_t5FED37944BE574E3B297791169909363750C8A64 * ___s_IntegratedSubsystemInstances_0;
	// System.Collections.Generic.List`1<UnityEngine.ISubsystem> UnityEngine.Internal_SubsystemInstances::s_StandaloneSubsystemInstances
	List_1_t5FED37944BE574E3B297791169909363750C8A64 * ___s_StandaloneSubsystemInstances_1;

public:
	inline static int32_t get_offset_of_s_IntegratedSubsystemInstances_0() { return static_cast<int32_t>(offsetof(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_StaticFields, ___s_IntegratedSubsystemInstances_0)); }
	inline List_1_t5FED37944BE574E3B297791169909363750C8A64 * get_s_IntegratedSubsystemInstances_0() const { return ___s_IntegratedSubsystemInstances_0; }
	inline List_1_t5FED37944BE574E3B297791169909363750C8A64 ** get_address_of_s_IntegratedSubsystemInstances_0() { return &___s_IntegratedSubsystemInstances_0; }
	inline void set_s_IntegratedSubsystemInstances_0(List_1_t5FED37944BE574E3B297791169909363750C8A64 * value)
	{
		___s_IntegratedSubsystemInstances_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_IntegratedSubsystemInstances_0), (void*)value);
	}

	inline static int32_t get_offset_of_s_StandaloneSubsystemInstances_1() { return static_cast<int32_t>(offsetof(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_StaticFields, ___s_StandaloneSubsystemInstances_1)); }
	inline List_1_t5FED37944BE574E3B297791169909363750C8A64 * get_s_StandaloneSubsystemInstances_1() const { return ___s_StandaloneSubsystemInstances_1; }
	inline List_1_t5FED37944BE574E3B297791169909363750C8A64 ** get_address_of_s_StandaloneSubsystemInstances_1() { return &___s_StandaloneSubsystemInstances_1; }
	inline void set_s_StandaloneSubsystemInstances_1(List_1_t5FED37944BE574E3B297791169909363750C8A64 * value)
	{
		___s_StandaloneSubsystemInstances_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_StandaloneSubsystemInstances_1), (void*)value);
	}
};


// UnityEngine.SubsystemDescriptor
struct  SubsystemDescriptor_tF663011CB44AB1D342821BBEF7B6811E799A7245  : public RuntimeObject
{
public:
	// System.String UnityEngine.SubsystemDescriptor::<id>k__BackingField
	String_t* ___U3CidU3Ek__BackingField_0;

public:
	inline static int32_t get_offset_of_U3CidU3Ek__BackingField_0() { return static_cast<int32_t>(offsetof(SubsystemDescriptor_tF663011CB44AB1D342821BBEF7B6811E799A7245, ___U3CidU3Ek__BackingField_0)); }
	inline String_t* get_U3CidU3Ek__BackingField_0() const { return ___U3CidU3Ek__BackingField_0; }
	inline String_t** get_address_of_U3CidU3Ek__BackingField_0() { return &___U3CidU3Ek__BackingField_0; }
	inline void set_U3CidU3Ek__BackingField_0(String_t* value)
	{
		___U3CidU3Ek__BackingField_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CidU3Ek__BackingField_0), (void*)value);
	}
};


// UnityEngine.SubsystemManager
struct  SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9  : public RuntimeObject
{
public:

public:
};

struct SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_StaticFields
{
public:
	// System.Action UnityEngine.SubsystemManager::reloadSubsytemsStarted
	Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * ___reloadSubsytemsStarted_0;
	// System.Action UnityEngine.SubsystemManager::reloadSubsytemsCompleted
	Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * ___reloadSubsytemsCompleted_1;

public:
	inline static int32_t get_offset_of_reloadSubsytemsStarted_0() { return static_cast<int32_t>(offsetof(SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_StaticFields, ___reloadSubsytemsStarted_0)); }
	inline Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * get_reloadSubsytemsStarted_0() const { return ___reloadSubsytemsStarted_0; }
	inline Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 ** get_address_of_reloadSubsytemsStarted_0() { return &___reloadSubsytemsStarted_0; }
	inline void set_reloadSubsytemsStarted_0(Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * value)
	{
		___reloadSubsytemsStarted_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___reloadSubsytemsStarted_0), (void*)value);
	}

	inline static int32_t get_offset_of_reloadSubsytemsCompleted_1() { return static_cast<int32_t>(offsetof(SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_StaticFields, ___reloadSubsytemsCompleted_1)); }
	inline Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * get_reloadSubsytemsCompleted_1() const { return ___reloadSubsytemsCompleted_1; }
	inline Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 ** get_address_of_reloadSubsytemsCompleted_1() { return &___reloadSubsytemsCompleted_1; }
	inline void set_reloadSubsytemsCompleted_1(Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * value)
	{
		___reloadSubsytemsCompleted_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___reloadSubsytemsCompleted_1), (void*)value);
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


// System.Collections.Generic.List`1_Enumerator<System.Object>
struct  Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 
{
public:
	// System.Collections.Generic.List`1<T> System.Collections.Generic.List`1_Enumerator::list
	List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * ___list_0;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::index
	int32_t ___index_1;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::version
	int32_t ___version_2;
	// T System.Collections.Generic.List`1_Enumerator::current
	RuntimeObject * ___current_3;

public:
	inline static int32_t get_offset_of_list_0() { return static_cast<int32_t>(offsetof(Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6, ___list_0)); }
	inline List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * get_list_0() const { return ___list_0; }
	inline List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 ** get_address_of_list_0() { return &___list_0; }
	inline void set_list_0(List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * value)
	{
		___list_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___list_0), (void*)value);
	}

	inline static int32_t get_offset_of_index_1() { return static_cast<int32_t>(offsetof(Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6, ___index_1)); }
	inline int32_t get_index_1() const { return ___index_1; }
	inline int32_t* get_address_of_index_1() { return &___index_1; }
	inline void set_index_1(int32_t value)
	{
		___index_1 = value;
	}

	inline static int32_t get_offset_of_version_2() { return static_cast<int32_t>(offsetof(Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6, ___version_2)); }
	inline int32_t get_version_2() const { return ___version_2; }
	inline int32_t* get_address_of_version_2() { return &___version_2; }
	inline void set_version_2(int32_t value)
	{
		___version_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6, ___current_3)); }
	inline RuntimeObject * get_current_3() const { return ___current_3; }
	inline RuntimeObject ** get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(RuntimeObject * value)
	{
		___current_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___current_3), (void*)value);
	}
};


// System.Collections.Generic.List`1_Enumerator<UnityEngine.ISubsystem>
struct  Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658 
{
public:
	// System.Collections.Generic.List`1<T> System.Collections.Generic.List`1_Enumerator::list
	List_1_t5FED37944BE574E3B297791169909363750C8A64 * ___list_0;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::index
	int32_t ___index_1;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::version
	int32_t ___version_2;
	// T System.Collections.Generic.List`1_Enumerator::current
	RuntimeObject* ___current_3;

public:
	inline static int32_t get_offset_of_list_0() { return static_cast<int32_t>(offsetof(Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658, ___list_0)); }
	inline List_1_t5FED37944BE574E3B297791169909363750C8A64 * get_list_0() const { return ___list_0; }
	inline List_1_t5FED37944BE574E3B297791169909363750C8A64 ** get_address_of_list_0() { return &___list_0; }
	inline void set_list_0(List_1_t5FED37944BE574E3B297791169909363750C8A64 * value)
	{
		___list_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___list_0), (void*)value);
	}

	inline static int32_t get_offset_of_index_1() { return static_cast<int32_t>(offsetof(Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658, ___index_1)); }
	inline int32_t get_index_1() const { return ___index_1; }
	inline int32_t* get_address_of_index_1() { return &___index_1; }
	inline void set_index_1(int32_t value)
	{
		___index_1 = value;
	}

	inline static int32_t get_offset_of_version_2() { return static_cast<int32_t>(offsetof(Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658, ___version_2)); }
	inline int32_t get_version_2() const { return ___version_2; }
	inline int32_t* get_address_of_version_2() { return &___version_2; }
	inline void set_version_2(int32_t value)
	{
		___version_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658, ___current_3)); }
	inline RuntimeObject* get_current_3() const { return ___current_3; }
	inline RuntimeObject** get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(RuntimeObject* value)
	{
		___current_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___current_3), (void*)value);
	}
};


// System.Collections.Generic.List`1_Enumerator<UnityEngine.ISubsystemDescriptor>
struct  Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9 
{
public:
	// System.Collections.Generic.List`1<T> System.Collections.Generic.List`1_Enumerator::list
	List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B * ___list_0;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::index
	int32_t ___index_1;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::version
	int32_t ___version_2;
	// T System.Collections.Generic.List`1_Enumerator::current
	RuntimeObject* ___current_3;

public:
	inline static int32_t get_offset_of_list_0() { return static_cast<int32_t>(offsetof(Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9, ___list_0)); }
	inline List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B * get_list_0() const { return ___list_0; }
	inline List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B ** get_address_of_list_0() { return &___list_0; }
	inline void set_list_0(List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B * value)
	{
		___list_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___list_0), (void*)value);
	}

	inline static int32_t get_offset_of_index_1() { return static_cast<int32_t>(offsetof(Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9, ___index_1)); }
	inline int32_t get_index_1() const { return ___index_1; }
	inline int32_t* get_address_of_index_1() { return &___index_1; }
	inline void set_index_1(int32_t value)
	{
		___index_1 = value;
	}

	inline static int32_t get_offset_of_version_2() { return static_cast<int32_t>(offsetof(Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9, ___version_2)); }
	inline int32_t get_version_2() const { return ___version_2; }
	inline int32_t* get_address_of_version_2() { return &___version_2; }
	inline void set_version_2(int32_t value)
	{
		___version_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9, ___current_3)); }
	inline RuntimeObject* get_current_3() const { return ___current_3; }
	inline RuntimeObject** get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(RuntimeObject* value)
	{
		___current_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___current_3), (void*)value);
	}
};


// System.Collections.Generic.List`1_Enumerator<UnityEngine.ISubsystemDescriptorImpl>
struct  Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714 
{
public:
	// System.Collections.Generic.List`1<T> System.Collections.Generic.List`1_Enumerator::list
	List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C * ___list_0;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::index
	int32_t ___index_1;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::version
	int32_t ___version_2;
	// T System.Collections.Generic.List`1_Enumerator::current
	RuntimeObject* ___current_3;

public:
	inline static int32_t get_offset_of_list_0() { return static_cast<int32_t>(offsetof(Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714, ___list_0)); }
	inline List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C * get_list_0() const { return ___list_0; }
	inline List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C ** get_address_of_list_0() { return &___list_0; }
	inline void set_list_0(List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C * value)
	{
		___list_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___list_0), (void*)value);
	}

	inline static int32_t get_offset_of_index_1() { return static_cast<int32_t>(offsetof(Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714, ___index_1)); }
	inline int32_t get_index_1() const { return ___index_1; }
	inline int32_t* get_address_of_index_1() { return &___index_1; }
	inline void set_index_1(int32_t value)
	{
		___index_1 = value;
	}

	inline static int32_t get_offset_of_version_2() { return static_cast<int32_t>(offsetof(Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714, ___version_2)); }
	inline int32_t get_version_2() const { return ___version_2; }
	inline int32_t* get_address_of_version_2() { return &___version_2; }
	inline void set_version_2(int32_t value)
	{
		___version_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714, ___current_3)); }
	inline RuntimeObject* get_current_3() const { return ___current_3; }
	inline RuntimeObject** get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(RuntimeObject* value)
	{
		___current_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___current_3), (void*)value);
	}
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

// UnityEngine.IntegratedSubsystem
struct  IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002  : public RuntimeObject
{
public:
	// System.IntPtr UnityEngine.IntegratedSubsystem::m_Ptr
	intptr_t ___m_Ptr_0;
	// UnityEngine.ISubsystemDescriptor UnityEngine.IntegratedSubsystem::m_subsystemDescriptor
	RuntimeObject* ___m_subsystemDescriptor_1;

public:
	inline static int32_t get_offset_of_m_Ptr_0() { return static_cast<int32_t>(offsetof(IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002, ___m_Ptr_0)); }
	inline intptr_t get_m_Ptr_0() const { return ___m_Ptr_0; }
	inline intptr_t* get_address_of_m_Ptr_0() { return &___m_Ptr_0; }
	inline void set_m_Ptr_0(intptr_t value)
	{
		___m_Ptr_0 = value;
	}

	inline static int32_t get_offset_of_m_subsystemDescriptor_1() { return static_cast<int32_t>(offsetof(IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002, ___m_subsystemDescriptor_1)); }
	inline RuntimeObject* get_m_subsystemDescriptor_1() const { return ___m_subsystemDescriptor_1; }
	inline RuntimeObject** get_address_of_m_subsystemDescriptor_1() { return &___m_subsystemDescriptor_1; }
	inline void set_m_subsystemDescriptor_1(RuntimeObject* value)
	{
		___m_subsystemDescriptor_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_subsystemDescriptor_1), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.IntegratedSubsystem
struct IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_marshaled_pinvoke
{
	intptr_t ___m_Ptr_0;
	RuntimeObject* ___m_subsystemDescriptor_1;
};
// Native definition for COM marshalling of UnityEngine.IntegratedSubsystem
struct IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_marshaled_com
{
	intptr_t ___m_Ptr_0;
	RuntimeObject* ___m_subsystemDescriptor_1;
};

// UnityEngine.IntegratedSubsystemDescriptor
struct  IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A  : public RuntimeObject
{
public:
	// System.IntPtr UnityEngine.IntegratedSubsystemDescriptor::m_Ptr
	intptr_t ___m_Ptr_0;

public:
	inline static int32_t get_offset_of_m_Ptr_0() { return static_cast<int32_t>(offsetof(IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A, ___m_Ptr_0)); }
	inline intptr_t get_m_Ptr_0() const { return ___m_Ptr_0; }
	inline intptr_t* get_address_of_m_Ptr_0() { return &___m_Ptr_0; }
	inline void set_m_Ptr_0(intptr_t value)
	{
		___m_Ptr_0 = value;
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.IntegratedSubsystemDescriptor
struct IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshaled_pinvoke
{
	intptr_t ___m_Ptr_0;
};
// Native definition for COM marshalling of UnityEngine.IntegratedSubsystemDescriptor
struct IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshaled_com
{
	intptr_t ___m_Ptr_0;
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

// System.Action
struct  Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6  : public MulticastDelegate_t
{
public:

public:
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
// System.Object[]
struct ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) RuntimeObject * m_Items[1];

public:
	inline RuntimeObject * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline RuntimeObject ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, RuntimeObject * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline RuntimeObject * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline RuntimeObject ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, RuntimeObject * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};


// System.Collections.Generic.List`1/Enumerator<!0> System.Collections.Generic.List`1<System.Object>::GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6  List_1_GetEnumerator_m1739A5E25DF502A6984F9B98CFCAC2D3FABCF233_gshared (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.List`1/Enumerator<System.Object>::get_Current()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject * Enumerator_get_Current_m9C4EBBD2108B51885E750F927D7936290C8E20EE_gshared_inline (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.List`1/Enumerator<System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Enumerator_MoveNext_m2E56233762839CE55C67E00AC8DD3D4D3F6C0DF0_gshared (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1/Enumerator<System.Object>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Enumerator_Dispose_mCFB225D9E5E597A1CC8F958E53BEA1367D8AC7B8_gshared (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1<System.Object>::Add(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void List_1_Add_mE5B3CBB3A625606D9BC4337FEAAF1D66BCB6F96E_gshared (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * __this, RuntimeObject * ___item0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1<System.Object>::Clear()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void List_1_Clear_m5FB5A9C59D8625FDFB06876C4D8848F0F07ABFD0_gshared (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1<System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void List_1__ctor_m0F0E00088CF56FEACC9E32D8B7D91B93D91DAA3B_gshared (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * __this, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.List`1<System.Object>::get_Count()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t List_1_get_Count_m5D847939ABB9A78203B062CAFFE975792174D00F_gshared_inline (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.List`1<System.Object>::get_Item(System.Int32)
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject * List_1_get_Item_mF00B574E58FB078BB753B05A3B86DD0A7A266B63_gshared_inline (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * __this, int32_t ___index0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1<System.Object>::RemoveAt(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void List_1_RemoveAt_m66148860899ECCAE9B323372032BFC1C255393D2_gshared (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * __this, int32_t ___index0, const RuntimeMethod* method);

// System.Void System.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Collections.Generic.List`1/Enumerator<!0> System.Collections.Generic.List`1<UnityEngine.ISubsystemDescriptor>::GetEnumerator()
inline Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9  List_1_GetEnumerator_m7985F077822F6454C777325341202546D1323C91 (List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B * __this, const RuntimeMethod* method)
{
	return ((  Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9  (*) (List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B *, const RuntimeMethod*))List_1_GetEnumerator_m1739A5E25DF502A6984F9B98CFCAC2D3FABCF233_gshared)(__this, method);
}
// !0 System.Collections.Generic.List`1/Enumerator<UnityEngine.ISubsystemDescriptor>::get_Current()
inline RuntimeObject* Enumerator_get_Current_m25D3F1FAA6111CD2A2E1AC23462EE96164991215_inline (Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9 * __this, const RuntimeMethod* method)
{
	return ((  RuntimeObject* (*) (Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9 *, const RuntimeMethod*))Enumerator_get_Current_m9C4EBBD2108B51885E750F927D7936290C8E20EE_gshared_inline)(__this, method);
}
// System.Boolean System.Collections.Generic.List`1/Enumerator<UnityEngine.ISubsystemDescriptor>::MoveNext()
inline bool Enumerator_MoveNext_m4476AE4E998C854351889A11F106494DF5282590 (Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9 * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9 *, const RuntimeMethod*))Enumerator_MoveNext_m2E56233762839CE55C67E00AC8DD3D4D3F6C0DF0_gshared)(__this, method);
}
// System.Void System.Collections.Generic.List`1/Enumerator<UnityEngine.ISubsystemDescriptor>::Dispose()
inline void Enumerator_Dispose_mE79C91F4CB323DA8E36737C9513F9D81728EEB75 (Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9 * __this, const RuntimeMethod* method)
{
	((  void (*) (Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9 *, const RuntimeMethod*))Enumerator_Dispose_mCFB225D9E5E597A1CC8F958E53BEA1367D8AC7B8_gshared)(__this, method);
}
// System.Void System.Collections.Generic.List`1<UnityEngine.ISubsystemDescriptor>::Add(!0)
inline void List_1_Add_mB612D0F6235C1599F80835BA7170C7602CD44DF5 (List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B * __this, RuntimeObject* ___item0, const RuntimeMethod* method)
{
	((  void (*) (List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B *, RuntimeObject*, const RuntimeMethod*))List_1_Add_mE5B3CBB3A625606D9BC4337FEAAF1D66BCB6F96E_gshared)(__this, ___item0, method);
}
// System.String UnityEngine.SubsystemDescriptor::get_id()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR String_t* SubsystemDescriptor_get_id_mC3D7972588D4B57F906A06CEE54A61B55D1306DB_inline (SubsystemDescriptor_tF663011CB44AB1D342821BBEF7B6811E799A7245 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.SubsystemManager::ReportSingleSubsystemAnalytics(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SubsystemManager_ReportSingleSubsystemAnalytics_mEFB0497B98F43D9D4174E870062A06C9313AC783 (String_t* ___id0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1<UnityEngine.ISubsystemDescriptorImpl>::Add(!0)
inline void List_1_Add_m15F562F509A431E4C6172383F81BBFEC7B766A2C (List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C * __this, RuntimeObject* ___item0, const RuntimeMethod* method)
{
	((  void (*) (List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C *, RuntimeObject*, const RuntimeMethod*))List_1_Add_mE5B3CBB3A625606D9BC4337FEAAF1D66BCB6F96E_gshared)(__this, ___item0, method);
}
// System.Collections.Generic.List`1/Enumerator<!0> System.Collections.Generic.List`1<UnityEngine.ISubsystemDescriptorImpl>::GetEnumerator()
inline Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714  List_1_GetEnumerator_m1CC6277818506CE6B5DFA48BD0A26E4B5FBCCE93 (List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C * __this, const RuntimeMethod* method)
{
	return ((  Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714  (*) (List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C *, const RuntimeMethod*))List_1_GetEnumerator_m1739A5E25DF502A6984F9B98CFCAC2D3FABCF233_gshared)(__this, method);
}
// !0 System.Collections.Generic.List`1/Enumerator<UnityEngine.ISubsystemDescriptorImpl>::get_Current()
inline RuntimeObject* Enumerator_get_Current_m59B82C82B00B6E477627FF890657E19A0596EC40_inline (Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714 * __this, const RuntimeMethod* method)
{
	return ((  RuntimeObject* (*) (Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714 *, const RuntimeMethod*))Enumerator_get_Current_m9C4EBBD2108B51885E750F927D7936290C8E20EE_gshared_inline)(__this, method);
}
// System.Boolean System.Collections.Generic.List`1/Enumerator<UnityEngine.ISubsystemDescriptorImpl>::MoveNext()
inline bool Enumerator_MoveNext_mBAB8D6690C8037262A8A1058928C8BB1EE9A6ED0 (Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714 * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714 *, const RuntimeMethod*))Enumerator_MoveNext_m2E56233762839CE55C67E00AC8DD3D4D3F6C0DF0_gshared)(__this, method);
}
// System.Void System.Collections.Generic.List`1/Enumerator<UnityEngine.ISubsystemDescriptorImpl>::Dispose()
inline void Enumerator_Dispose_m5EA13D6FA77939E22384B5FA182D4266776DD244 (Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714 * __this, const RuntimeMethod* method)
{
	((  void (*) (Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714 *, const RuntimeMethod*))Enumerator_Dispose_mCFB225D9E5E597A1CC8F958E53BEA1367D8AC7B8_gshared)(__this, method);
}
// System.Void System.Collections.Generic.List`1<UnityEngine.ISubsystemDescriptorImpl>::Clear()
inline void List_1_Clear_mBBDF59B5B02BFD11B20EC27E5092D9C55A82179D (List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C * __this, const RuntimeMethod* method)
{
	((  void (*) (List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C *, const RuntimeMethod*))List_1_Clear_m5FB5A9C59D8625FDFB06876C4D8848F0F07ABFD0_gshared)(__this, method);
}
// System.Void System.Collections.Generic.List`1<UnityEngine.ISubsystemDescriptorImpl>::.ctor()
inline void List_1__ctor_mE3E1C3F22FB1EE5389CA8E3BCE698AB8A4DF5F25 (List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C * __this, const RuntimeMethod* method)
{
	((  void (*) (List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C *, const RuntimeMethod*))List_1__ctor_m0F0E00088CF56FEACC9E32D8B7D91B93D91DAA3B_gshared)(__this, method);
}
// System.Void System.Collections.Generic.List`1<UnityEngine.ISubsystemDescriptor>::.ctor()
inline void List_1__ctor_m11E48E7BBD9D92B1240BE22DFEC0A835B3E5545F (List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B * __this, const RuntimeMethod* method)
{
	((  void (*) (List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B *, const RuntimeMethod*))List_1__ctor_m0F0E00088CF56FEACC9E32D8B7D91B93D91DAA3B_gshared)(__this, method);
}
// System.Void UnityEngine.IntegratedSubsystem::SetHandle(UnityEngine.IntegratedSubsystem)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IntegratedSubsystem_SetHandle_m951ABC336E4544AAD792A7118E2261AC8F2A297E (IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * __this, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * ___inst0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1<UnityEngine.ISubsystem>::Add(!0)
inline void List_1_Add_m0CE24B326592DD284E9F4D653E3BF1891CB613A4 (List_1_t5FED37944BE574E3B297791169909363750C8A64 * __this, RuntimeObject* ___item0, const RuntimeMethod* method)
{
	((  void (*) (List_1_t5FED37944BE574E3B297791169909363750C8A64 *, RuntimeObject*, const RuntimeMethod*))List_1_Add_mE5B3CBB3A625606D9BC4337FEAAF1D66BCB6F96E_gshared)(__this, ___item0, method);
}
// System.Collections.Generic.List`1/Enumerator<!0> System.Collections.Generic.List`1<UnityEngine.ISubsystem>::GetEnumerator()
inline Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658  List_1_GetEnumerator_m0D7B7D04AAB181B6AA7525709026E68258ED5740 (List_1_t5FED37944BE574E3B297791169909363750C8A64 * __this, const RuntimeMethod* method)
{
	return ((  Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658  (*) (List_1_t5FED37944BE574E3B297791169909363750C8A64 *, const RuntimeMethod*))List_1_GetEnumerator_m1739A5E25DF502A6984F9B98CFCAC2D3FABCF233_gshared)(__this, method);
}
// !0 System.Collections.Generic.List`1/Enumerator<UnityEngine.ISubsystem>::get_Current()
inline RuntimeObject* Enumerator_get_Current_m08AAF79B6D539D33DC62A4630DD67D1D8CF22D9E_inline (Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658 * __this, const RuntimeMethod* method)
{
	return ((  RuntimeObject* (*) (Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658 *, const RuntimeMethod*))Enumerator_get_Current_m9C4EBBD2108B51885E750F927D7936290C8E20EE_gshared_inline)(__this, method);
}
// System.Boolean System.Collections.Generic.List`1/Enumerator<UnityEngine.ISubsystem>::MoveNext()
inline bool Enumerator_MoveNext_m6090B713C1599612E9E959204C495E69FF635A6B (Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658 * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658 *, const RuntimeMethod*))Enumerator_MoveNext_m2E56233762839CE55C67E00AC8DD3D4D3F6C0DF0_gshared)(__this, method);
}
// System.Void System.Collections.Generic.List`1/Enumerator<UnityEngine.ISubsystem>::Dispose()
inline void Enumerator_Dispose_mEFBC03EAF32221CE654B48D4D21F15D5B57F1C35 (Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658 * __this, const RuntimeMethod* method)
{
	((  void (*) (Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658 *, const RuntimeMethod*))Enumerator_Dispose_mCFB225D9E5E597A1CC8F958E53BEA1367D8AC7B8_gshared)(__this, method);
}
// System.Void System.Collections.Generic.List`1<UnityEngine.ISubsystem>::Clear()
inline void List_1_Clear_m2755F780FB1D50F6D26CE478AF38D661AA868589 (List_1_t5FED37944BE574E3B297791169909363750C8A64 * __this, const RuntimeMethod* method)
{
	((  void (*) (List_1_t5FED37944BE574E3B297791169909363750C8A64 *, const RuntimeMethod*))List_1_Clear_m5FB5A9C59D8625FDFB06876C4D8848F0F07ABFD0_gshared)(__this, method);
}
// System.Int32 System.Collections.Generic.List`1<UnityEngine.ISubsystem>::get_Count()
inline int32_t List_1_get_Count_m5655FD369C25BA7B877F8C8E5139B8CAFB9A31B2_inline (List_1_t5FED37944BE574E3B297791169909363750C8A64 * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (List_1_t5FED37944BE574E3B297791169909363750C8A64 *, const RuntimeMethod*))List_1_get_Count_m5D847939ABB9A78203B062CAFFE975792174D00F_gshared_inline)(__this, method);
}
// !0 System.Collections.Generic.List`1<UnityEngine.ISubsystem>::get_Item(System.Int32)
inline RuntimeObject* List_1_get_Item_m4C4EC958972680EB68F10F5666D97A09BC69B609_inline (List_1_t5FED37944BE574E3B297791169909363750C8A64 * __this, int32_t ___index0, const RuntimeMethod* method)
{
	return ((  RuntimeObject* (*) (List_1_t5FED37944BE574E3B297791169909363750C8A64 *, int32_t, const RuntimeMethod*))List_1_get_Item_mF00B574E58FB078BB753B05A3B86DD0A7A266B63_gshared_inline)(__this, ___index0, method);
}
// System.Boolean System.IntPtr::op_Equality(System.IntPtr,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool IntPtr_op_Equality_mD94F3FE43A65684EFF984A7B95E70D2520C0AC73 (intptr_t ___value10, intptr_t ___value21, const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1<UnityEngine.ISubsystem>::RemoveAt(System.Int32)
inline void List_1_RemoveAt_m87C2B6E364B977B74BB97E0D0B6D573661CB3481 (List_1_t5FED37944BE574E3B297791169909363750C8A64 * __this, int32_t ___index0, const RuntimeMethod* method)
{
	((  void (*) (List_1_t5FED37944BE574E3B297791169909363750C8A64 *, int32_t, const RuntimeMethod*))List_1_RemoveAt_m66148860899ECCAE9B323372032BFC1C255393D2_gshared)(__this, ___index0, method);
}
// System.Void System.Collections.Generic.List`1<UnityEngine.ISubsystem>::.ctor()
inline void List_1__ctor_mE7AF9A04C421B1097120D9179F9AE7E559E3DB83 (List_1_t5FED37944BE574E3B297791169909363750C8A64 * __this, const RuntimeMethod* method)
{
	((  void (*) (List_1_t5FED37944BE574E3B297791169909363750C8A64 *, const RuntimeMethod*))List_1__ctor_m0F0E00088CF56FEACC9E32D8B7D91B93D91DAA3B_gshared)(__this, method);
}
// System.Void UnityEngine.SubsystemManager::StaticConstructScriptingClassMap()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SubsystemManager_StaticConstructScriptingClassMap_m2D54CD8E480B334F4EC0EAAF49DE4186895885EE (const RuntimeMethod* method);
// System.Void System.Action::Invoke()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Action_Invoke_m3FFA5BE3D64F0FF8E1E1CB6F953913FADB5EB89E (Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * __this, const RuntimeMethod* method);
// System.Void System.ThrowHelper::ThrowArgumentOutOfRangeException()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ThrowHelper_ThrowArgumentOutOfRangeException_m4841366ABC2B2AFA37C10900551D7E07522C0929 (const RuntimeMethod* method);
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
// Conversion methods for marshalling of: UnityEngine.IntegratedSubsystem
IL2CPP_EXTERN_C void IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_marshal_pinvoke(const IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002& unmarshaled, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_marshaled_pinvoke& marshaled)
{
	Exception_t* ___m_subsystemDescriptor_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'm_subsystemDescriptor' of type 'IntegratedSubsystem': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___m_subsystemDescriptor_1Exception, NULL);
}
IL2CPP_EXTERN_C void IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_marshal_pinvoke_back(const IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_marshaled_pinvoke& marshaled, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002& unmarshaled)
{
	Exception_t* ___m_subsystemDescriptor_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'm_subsystemDescriptor' of type 'IntegratedSubsystem': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___m_subsystemDescriptor_1Exception, NULL);
}
// Conversion method for clean up from marshalling of: UnityEngine.IntegratedSubsystem
IL2CPP_EXTERN_C void IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_marshal_pinvoke_cleanup(IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: UnityEngine.IntegratedSubsystem
IL2CPP_EXTERN_C void IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_marshal_com(const IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002& unmarshaled, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_marshaled_com& marshaled)
{
	Exception_t* ___m_subsystemDescriptor_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'm_subsystemDescriptor' of type 'IntegratedSubsystem': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___m_subsystemDescriptor_1Exception, NULL);
}
IL2CPP_EXTERN_C void IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_marshal_com_back(const IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_marshaled_com& marshaled, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002& unmarshaled)
{
	Exception_t* ___m_subsystemDescriptor_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'm_subsystemDescriptor' of type 'IntegratedSubsystem': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___m_subsystemDescriptor_1Exception, NULL);
}
// Conversion method for clean up from marshalling of: UnityEngine.IntegratedSubsystem
IL2CPP_EXTERN_C void IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_marshal_com_cleanup(IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_marshaled_com& marshaled)
{
}
// System.Void UnityEngine.IntegratedSubsystem::SetHandle(UnityEngine.IntegratedSubsystem)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IntegratedSubsystem_SetHandle_m951ABC336E4544AAD792A7118E2261AC8F2A297E (IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * __this, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * ___inst0, const RuntimeMethod* method)
{
	typedef void (*IntegratedSubsystem_SetHandle_m951ABC336E4544AAD792A7118E2261AC8F2A297E_ftn) (IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 *, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 *);
	static IntegratedSubsystem_SetHandle_m951ABC336E4544AAD792A7118E2261AC8F2A297E_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (IntegratedSubsystem_SetHandle_m951ABC336E4544AAD792A7118E2261AC8F2A297E_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.IntegratedSubsystem::SetHandle(UnityEngine.IntegratedSubsystem)");
	_il2cpp_icall_func(__this, ___inst0);
}
// System.Void UnityEngine.IntegratedSubsystem::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IntegratedSubsystem__ctor_mCD638F6C367F1B83B1CB62130E570CA64A757DCE (IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405(__this, /*hidden argument*/NULL);
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
// Conversion methods for marshalling of: UnityEngine.IntegratedSubsystemDescriptor
IL2CPP_EXTERN_C void IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshal_pinvoke(const IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A& unmarshaled, IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshaled_pinvoke& marshaled)
{
	marshaled.___m_Ptr_0 = unmarshaled.get_m_Ptr_0();
}
IL2CPP_EXTERN_C void IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshal_pinvoke_back(const IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshaled_pinvoke& marshaled, IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A& unmarshaled)
{
	intptr_t unmarshaled_m_Ptr_temp_0;
	memset((&unmarshaled_m_Ptr_temp_0), 0, sizeof(unmarshaled_m_Ptr_temp_0));
	unmarshaled_m_Ptr_temp_0 = marshaled.___m_Ptr_0;
	unmarshaled.set_m_Ptr_0(unmarshaled_m_Ptr_temp_0);
}
// Conversion method for clean up from marshalling of: UnityEngine.IntegratedSubsystemDescriptor
IL2CPP_EXTERN_C void IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshal_pinvoke_cleanup(IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: UnityEngine.IntegratedSubsystemDescriptor
IL2CPP_EXTERN_C void IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshal_com(const IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A& unmarshaled, IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshaled_com& marshaled)
{
	marshaled.___m_Ptr_0 = unmarshaled.get_m_Ptr_0();
}
IL2CPP_EXTERN_C void IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshal_com_back(const IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshaled_com& marshaled, IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A& unmarshaled)
{
	intptr_t unmarshaled_m_Ptr_temp_0;
	memset((&unmarshaled_m_Ptr_temp_0), 0, sizeof(unmarshaled_m_Ptr_temp_0));
	unmarshaled_m_Ptr_temp_0 = marshaled.___m_Ptr_0;
	unmarshaled.set_m_Ptr_0(unmarshaled_m_Ptr_temp_0);
}
// Conversion method for clean up from marshalling of: UnityEngine.IntegratedSubsystemDescriptor
IL2CPP_EXTERN_C void IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshal_com_cleanup(IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshaled_com& marshaled)
{
}
// System.Void UnityEngine.IntegratedSubsystemDescriptor::UnityEngine.ISubsystemDescriptorImpl.set_ptr(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IntegratedSubsystemDescriptor_UnityEngine_ISubsystemDescriptorImpl_set_ptr_m6B3A3578CFF36369428345C4ED50EAC8853C6F35 (IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A * __this, intptr_t ___value0, const RuntimeMethod* method)
{
	{
		intptr_t L_0 = ___value0;
		__this->set_m_Ptr_0((intptr_t)L_0);
		return;
	}
}
// System.Void UnityEngine.IntegratedSubsystemDescriptor::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IntegratedSubsystemDescriptor__ctor_m06A2C2DB3D3004DFAC7344C2B8FDEF89434CBF3A (IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405(__this, /*hidden argument*/NULL);
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
// System.Boolean UnityEngine.Internal_SubsystemDescriptors::Internal_AddDescriptor(UnityEngine.SubsystemDescriptor)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Internal_SubsystemDescriptors_Internal_AddDescriptor_m3BC579382C5363C54D0B37C60FE844C82AA63D1E (SubsystemDescriptor_tF663011CB44AB1D342821BBEF7B6811E799A7245 * ___descriptor0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Internal_SubsystemDescriptors_Internal_AddDescriptor_m3BC579382C5363C54D0B37C60FE844C82AA63D1E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9  V_0;
	memset((&V_0), 0, sizeof(V_0));
	RuntimeObject* V_1 = NULL;
	bool V_2 = false;
	bool V_3 = false;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 2);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		IL2CPP_RUNTIME_CLASS_INIT(Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_il2cpp_TypeInfo_var);
		List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B * L_0 = ((Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_il2cpp_TypeInfo_var))->get_s_StandaloneSubsystemDescriptors_1();
		NullCheck(L_0);
		Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9  L_1 = List_1_GetEnumerator_m7985F077822F6454C777325341202546D1323C91(L_0, /*hidden argument*/List_1_GetEnumerator_m7985F077822F6454C777325341202546D1323C91_RuntimeMethod_var);
		V_0 = L_1;
	}

IL_000d:
	try
	{ // begin try (depth: 1)
		{
			goto IL_0025;
		}

IL_000f:
		{
			RuntimeObject* L_2 = Enumerator_get_Current_m25D3F1FAA6111CD2A2E1AC23462EE96164991215_inline((Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9 *)(&V_0), /*hidden argument*/Enumerator_get_Current_m25D3F1FAA6111CD2A2E1AC23462EE96164991215_RuntimeMethod_var);
			V_1 = L_2;
			RuntimeObject* L_3 = V_1;
			SubsystemDescriptor_tF663011CB44AB1D342821BBEF7B6811E799A7245 * L_4 = ___descriptor0;
			V_2 = (bool)((((RuntimeObject*)(RuntimeObject*)L_3) == ((RuntimeObject*)(SubsystemDescriptor_tF663011CB44AB1D342821BBEF7B6811E799A7245 *)L_4))? 1 : 0);
			bool L_5 = V_2;
			if (!L_5)
			{
				goto IL_0024;
			}
		}

IL_0020:
		{
			V_3 = (bool)0;
			IL2CPP_LEAVE(0x5B, FINALLY_0030);
		}

IL_0024:
		{
		}

IL_0025:
		{
			bool L_6 = Enumerator_MoveNext_m4476AE4E998C854351889A11F106494DF5282590((Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9 *)(&V_0), /*hidden argument*/Enumerator_MoveNext_m4476AE4E998C854351889A11F106494DF5282590_RuntimeMethod_var);
			if (L_6)
			{
				goto IL_000f;
			}
		}

IL_002e:
		{
			IL2CPP_LEAVE(0x3F, FINALLY_0030);
		}
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0030;
	}

FINALLY_0030:
	{ // begin finally (depth: 1)
		Enumerator_Dispose_mE79C91F4CB323DA8E36737C9513F9D81728EEB75((Enumerator_t87BFBB3A07505E0220113039BEF418FEC96B4FC9 *)(&V_0), /*hidden argument*/Enumerator_Dispose_mE79C91F4CB323DA8E36737C9513F9D81728EEB75_RuntimeMethod_var);
		IL2CPP_END_FINALLY(48)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(48)
	{
		IL2CPP_JUMP_TBL(0x5B, IL_005b)
		IL2CPP_JUMP_TBL(0x3F, IL_003f)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_003f:
	{
		IL2CPP_RUNTIME_CLASS_INIT(Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_il2cpp_TypeInfo_var);
		List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B * L_7 = ((Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_il2cpp_TypeInfo_var))->get_s_StandaloneSubsystemDescriptors_1();
		SubsystemDescriptor_tF663011CB44AB1D342821BBEF7B6811E799A7245 * L_8 = ___descriptor0;
		NullCheck(L_7);
		List_1_Add_mB612D0F6235C1599F80835BA7170C7602CD44DF5(L_7, L_8, /*hidden argument*/List_1_Add_mB612D0F6235C1599F80835BA7170C7602CD44DF5_RuntimeMethod_var);
		SubsystemDescriptor_tF663011CB44AB1D342821BBEF7B6811E799A7245 * L_9 = ___descriptor0;
		NullCheck(L_9);
		String_t* L_10 = SubsystemDescriptor_get_id_mC3D7972588D4B57F906A06CEE54A61B55D1306DB_inline(L_9, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_il2cpp_TypeInfo_var);
		SubsystemManager_ReportSingleSubsystemAnalytics_mEFB0497B98F43D9D4174E870062A06C9313AC783(L_10, /*hidden argument*/NULL);
		V_3 = (bool)1;
		goto IL_005b;
	}

IL_005b:
	{
		bool L_11 = V_3;
		return L_11;
	}
}
// System.Void UnityEngine.Internal_SubsystemDescriptors::Internal_InitializeManagedDescriptor(System.IntPtr,UnityEngine.ISubsystemDescriptorImpl)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Internal_SubsystemDescriptors_Internal_InitializeManagedDescriptor_m5D035D990571A95960C76DCA0AA7070CFC643911 (intptr_t ___ptr0, RuntimeObject* ___desc1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Internal_SubsystemDescriptors_Internal_InitializeManagedDescriptor_m5D035D990571A95960C76DCA0AA7070CFC643911_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = ___desc1;
		intptr_t L_1 = ___ptr0;
		NullCheck(L_0);
		InterfaceActionInvoker1< intptr_t >::Invoke(0 /* System.Void UnityEngine.ISubsystemDescriptorImpl::set_ptr(System.IntPtr) */, ISubsystemDescriptorImpl_t5C7DB694E962FEA9DE7EE89B059F7EF3ED69F77B_il2cpp_TypeInfo_var, L_0, (intptr_t)L_1);
		IL2CPP_RUNTIME_CLASS_INIT(Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_il2cpp_TypeInfo_var);
		List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C * L_2 = ((Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_il2cpp_TypeInfo_var))->get_s_IntegratedSubsystemDescriptors_0();
		RuntimeObject* L_3 = ___desc1;
		NullCheck(L_2);
		List_1_Add_m15F562F509A431E4C6172383F81BBFEC7B766A2C(L_2, L_3, /*hidden argument*/List_1_Add_m15F562F509A431E4C6172383F81BBFEC7B766A2C_RuntimeMethod_var);
		return;
	}
}
// System.Void UnityEngine.Internal_SubsystemDescriptors::Internal_ClearManagedDescriptors()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Internal_SubsystemDescriptors_Internal_ClearManagedDescriptors_m53D23CFAF2763F6BBFD9DFA92981C94F9CE2FE15 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Internal_SubsystemDescriptors_Internal_ClearManagedDescriptors_m53D23CFAF2763F6BBFD9DFA92981C94F9CE2FE15_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714  V_0;
	memset((&V_0), 0, sizeof(V_0));
	RuntimeObject* V_1 = NULL;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 1);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		IL2CPP_RUNTIME_CLASS_INIT(Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_il2cpp_TypeInfo_var);
		List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C * L_0 = ((Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_il2cpp_TypeInfo_var))->get_s_IntegratedSubsystemDescriptors_0();
		NullCheck(L_0);
		Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714  L_1 = List_1_GetEnumerator_m1CC6277818506CE6B5DFA48BD0A26E4B5FBCCE93(L_0, /*hidden argument*/List_1_GetEnumerator_m1CC6277818506CE6B5DFA48BD0A26E4B5FBCCE93_RuntimeMethod_var);
		V_0 = L_1;
	}

IL_000d:
	try
	{ // begin try (depth: 1)
		{
			goto IL_0025;
		}

IL_000f:
		{
			RuntimeObject* L_2 = Enumerator_get_Current_m59B82C82B00B6E477627FF890657E19A0596EC40_inline((Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714 *)(&V_0), /*hidden argument*/Enumerator_get_Current_m59B82C82B00B6E477627FF890657E19A0596EC40_RuntimeMethod_var);
			V_1 = L_2;
			RuntimeObject* L_3 = V_1;
			NullCheck(L_3);
			InterfaceActionInvoker1< intptr_t >::Invoke(0 /* System.Void UnityEngine.ISubsystemDescriptorImpl::set_ptr(System.IntPtr) */, ISubsystemDescriptorImpl_t5C7DB694E962FEA9DE7EE89B059F7EF3ED69F77B_il2cpp_TypeInfo_var, L_3, (intptr_t)(0));
		}

IL_0025:
		{
			bool L_4 = Enumerator_MoveNext_mBAB8D6690C8037262A8A1058928C8BB1EE9A6ED0((Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714 *)(&V_0), /*hidden argument*/Enumerator_MoveNext_mBAB8D6690C8037262A8A1058928C8BB1EE9A6ED0_RuntimeMethod_var);
			if (L_4)
			{
				goto IL_000f;
			}
		}

IL_002e:
		{
			IL2CPP_LEAVE(0x3F, FINALLY_0030);
		}
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0030;
	}

FINALLY_0030:
	{ // begin finally (depth: 1)
		Enumerator_Dispose_m5EA13D6FA77939E22384B5FA182D4266776DD244((Enumerator_t7ED94D286DE714CA2DCFEC31007053A4E2C82714 *)(&V_0), /*hidden argument*/Enumerator_Dispose_m5EA13D6FA77939E22384B5FA182D4266776DD244_RuntimeMethod_var);
		IL2CPP_END_FINALLY(48)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(48)
	{
		IL2CPP_JUMP_TBL(0x3F, IL_003f)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_003f:
	{
		IL2CPP_RUNTIME_CLASS_INIT(Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_il2cpp_TypeInfo_var);
		List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C * L_5 = ((Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_il2cpp_TypeInfo_var))->get_s_IntegratedSubsystemDescriptors_0();
		NullCheck(L_5);
		List_1_Clear_mBBDF59B5B02BFD11B20EC27E5092D9C55A82179D(L_5, /*hidden argument*/List_1_Clear_mBBDF59B5B02BFD11B20EC27E5092D9C55A82179D_RuntimeMethod_var);
		return;
	}
}
// System.Void UnityEngine.Internal_SubsystemDescriptors::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Internal_SubsystemDescriptors__cctor_m58666797C448C50D9788277A3FB77D29B1C411C8 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Internal_SubsystemDescriptors__cctor_m58666797C448C50D9788277A3FB77D29B1C411C8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C * L_0 = (List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C *)il2cpp_codegen_object_new(List_1_t618F75F9E61EDBC8DCC4BA0372F5608C52DA027C_il2cpp_TypeInfo_var);
		List_1__ctor_mE3E1C3F22FB1EE5389CA8E3BCE698AB8A4DF5F25(L_0, /*hidden argument*/List_1__ctor_mE3E1C3F22FB1EE5389CA8E3BCE698AB8A4DF5F25_RuntimeMethod_var);
		((Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_il2cpp_TypeInfo_var))->set_s_IntegratedSubsystemDescriptors_0(L_0);
		List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B * L_1 = (List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B *)il2cpp_codegen_object_new(List_1_t0D19BAE9CDA40DDBC1D487F2F5B1907D5334C95B_il2cpp_TypeInfo_var);
		List_1__ctor_m11E48E7BBD9D92B1240BE22DFEC0A835B3E5545F(L_1, /*hidden argument*/List_1__ctor_m11E48E7BBD9D92B1240BE22DFEC0A835B3E5545F_RuntimeMethod_var);
		((Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemDescriptors_tE02B181DE901DC42D96F1726BD97F696190A08B5_il2cpp_TypeInfo_var))->set_s_StandaloneSubsystemDescriptors_1(L_1);
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
// System.Void UnityEngine.Internal_SubsystemInstances::Internal_InitializeManagedInstance(System.IntPtr,UnityEngine.IntegratedSubsystem)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Internal_SubsystemInstances_Internal_InitializeManagedInstance_m2B53F9AACC98A102FB805164FD73F7366DAAB299 (intptr_t ___ptr0, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * ___inst1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Internal_SubsystemInstances_Internal_InitializeManagedInstance_m2B53F9AACC98A102FB805164FD73F7366DAAB299_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * L_0 = ___inst1;
		intptr_t L_1 = ___ptr0;
		NullCheck(L_0);
		L_0->set_m_Ptr_0((intptr_t)L_1);
		IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * L_2 = ___inst1;
		IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * L_3 = ___inst1;
		NullCheck(L_2);
		IntegratedSubsystem_SetHandle_m951ABC336E4544AAD792A7118E2261AC8F2A297E(L_2, L_3, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var);
		List_1_t5FED37944BE574E3B297791169909363750C8A64 * L_4 = ((Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var))->get_s_IntegratedSubsystemInstances_0();
		IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * L_5 = ___inst1;
		NullCheck(L_4);
		List_1_Add_m0CE24B326592DD284E9F4D653E3BF1891CB613A4(L_4, L_5, /*hidden argument*/List_1_Add_m0CE24B326592DD284E9F4D653E3BF1891CB613A4_RuntimeMethod_var);
		return;
	}
}
// System.Void UnityEngine.Internal_SubsystemInstances::Internal_ClearManagedInstances()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Internal_SubsystemInstances_Internal_ClearManagedInstances_mCDA0963D5AE8FBA27A15EB9BFDB9FC82AB6452A4 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Internal_SubsystemInstances_Internal_ClearManagedInstances_mCDA0963D5AE8FBA27A15EB9BFDB9FC82AB6452A4_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658  V_0;
	memset((&V_0), 0, sizeof(V_0));
	RuntimeObject* V_1 = NULL;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 1);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		IL2CPP_RUNTIME_CLASS_INIT(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var);
		List_1_t5FED37944BE574E3B297791169909363750C8A64 * L_0 = ((Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var))->get_s_IntegratedSubsystemInstances_0();
		NullCheck(L_0);
		Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658  L_1 = List_1_GetEnumerator_m0D7B7D04AAB181B6AA7525709026E68258ED5740(L_0, /*hidden argument*/List_1_GetEnumerator_m0D7B7D04AAB181B6AA7525709026E68258ED5740_RuntimeMethod_var);
		V_0 = L_1;
	}

IL_000d:
	try
	{ // begin try (depth: 1)
		{
			goto IL_0029;
		}

IL_000f:
		{
			RuntimeObject* L_2 = Enumerator_get_Current_m08AAF79B6D539D33DC62A4630DD67D1D8CF22D9E_inline((Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658 *)(&V_0), /*hidden argument*/Enumerator_get_Current_m08AAF79B6D539D33DC62A4630DD67D1D8CF22D9E_RuntimeMethod_var);
			V_1 = L_2;
			RuntimeObject* L_3 = V_1;
			NullCheck(((IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 *)CastclassClass((RuntimeObject*)L_3, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_il2cpp_TypeInfo_var)));
			((IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 *)CastclassClass((RuntimeObject*)L_3, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_il2cpp_TypeInfo_var))->set_m_Ptr_0((intptr_t)(0));
		}

IL_0029:
		{
			bool L_4 = Enumerator_MoveNext_m6090B713C1599612E9E959204C495E69FF635A6B((Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658 *)(&V_0), /*hidden argument*/Enumerator_MoveNext_m6090B713C1599612E9E959204C495E69FF635A6B_RuntimeMethod_var);
			if (L_4)
			{
				goto IL_000f;
			}
		}

IL_0032:
		{
			IL2CPP_LEAVE(0x43, FINALLY_0034);
		}
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0034;
	}

FINALLY_0034:
	{ // begin finally (depth: 1)
		Enumerator_Dispose_mEFBC03EAF32221CE654B48D4D21F15D5B57F1C35((Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658 *)(&V_0), /*hidden argument*/Enumerator_Dispose_mEFBC03EAF32221CE654B48D4D21F15D5B57F1C35_RuntimeMethod_var);
		IL2CPP_END_FINALLY(52)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(52)
	{
		IL2CPP_JUMP_TBL(0x43, IL_0043)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0043:
	{
		IL2CPP_RUNTIME_CLASS_INIT(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var);
		List_1_t5FED37944BE574E3B297791169909363750C8A64 * L_5 = ((Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var))->get_s_IntegratedSubsystemInstances_0();
		NullCheck(L_5);
		List_1_Clear_m2755F780FB1D50F6D26CE478AF38D661AA868589(L_5, /*hidden argument*/List_1_Clear_m2755F780FB1D50F6D26CE478AF38D661AA868589_RuntimeMethod_var);
		List_1_t5FED37944BE574E3B297791169909363750C8A64 * L_6 = ((Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var))->get_s_StandaloneSubsystemInstances_1();
		NullCheck(L_6);
		List_1_Clear_m2755F780FB1D50F6D26CE478AF38D661AA868589(L_6, /*hidden argument*/List_1_Clear_m2755F780FB1D50F6D26CE478AF38D661AA868589_RuntimeMethod_var);
		return;
	}
}
// System.Void UnityEngine.Internal_SubsystemInstances::Internal_RemoveInstanceByPtr(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Internal_SubsystemInstances_Internal_RemoveInstanceByPtr_mEBDEF7FC25B00E83E633298B6FB494CDDA093F26 (intptr_t ___ptr0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Internal_SubsystemInstances_Internal_RemoveInstanceByPtr_mEBDEF7FC25B00E83E633298B6FB494CDDA093F26_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	bool V_1 = false;
	bool V_2 = false;
	{
		IL2CPP_RUNTIME_CLASS_INIT(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var);
		List_1_t5FED37944BE574E3B297791169909363750C8A64 * L_0 = ((Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var))->get_s_IntegratedSubsystemInstances_0();
		NullCheck(L_0);
		int32_t L_1 = List_1_get_Count_m5655FD369C25BA7B877F8C8E5139B8CAFB9A31B2_inline(L_0, /*hidden argument*/List_1_get_Count_m5655FD369C25BA7B877F8C8E5139B8CAFB9A31B2_RuntimeMethod_var);
		V_0 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_1, (int32_t)1));
		goto IL_005d;
	}

IL_0010:
	{
		IL2CPP_RUNTIME_CLASS_INIT(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var);
		List_1_t5FED37944BE574E3B297791169909363750C8A64 * L_2 = ((Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var))->get_s_IntegratedSubsystemInstances_0();
		int32_t L_3 = V_0;
		NullCheck(L_2);
		RuntimeObject* L_4 = List_1_get_Item_m4C4EC958972680EB68F10F5666D97A09BC69B609_inline(L_2, L_3, /*hidden argument*/List_1_get_Item_m4C4EC958972680EB68F10F5666D97A09BC69B609_RuntimeMethod_var);
		NullCheck(((IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 *)CastclassClass((RuntimeObject*)L_4, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_il2cpp_TypeInfo_var)));
		intptr_t L_5 = ((IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 *)CastclassClass((RuntimeObject*)L_4, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_il2cpp_TypeInfo_var))->get_m_Ptr_0();
		intptr_t L_6 = ___ptr0;
		bool L_7 = IntPtr_op_Equality_mD94F3FE43A65684EFF984A7B95E70D2520C0AC73((intptr_t)L_5, (intptr_t)L_6, /*hidden argument*/NULL);
		V_1 = L_7;
		bool L_8 = V_1;
		if (!L_8)
		{
			goto IL_0058;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var);
		List_1_t5FED37944BE574E3B297791169909363750C8A64 * L_9 = ((Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var))->get_s_IntegratedSubsystemInstances_0();
		int32_t L_10 = V_0;
		NullCheck(L_9);
		RuntimeObject* L_11 = List_1_get_Item_m4C4EC958972680EB68F10F5666D97A09BC69B609_inline(L_9, L_10, /*hidden argument*/List_1_get_Item_m4C4EC958972680EB68F10F5666D97A09BC69B609_RuntimeMethod_var);
		NullCheck(((IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 *)CastclassClass((RuntimeObject*)L_11, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_il2cpp_TypeInfo_var)));
		((IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 *)CastclassClass((RuntimeObject*)L_11, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_il2cpp_TypeInfo_var))->set_m_Ptr_0((intptr_t)(0));
		List_1_t5FED37944BE574E3B297791169909363750C8A64 * L_12 = ((Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var))->get_s_IntegratedSubsystemInstances_0();
		int32_t L_13 = V_0;
		NullCheck(L_12);
		List_1_RemoveAt_m87C2B6E364B977B74BB97E0D0B6D573661CB3481(L_12, L_13, /*hidden argument*/List_1_RemoveAt_m87C2B6E364B977B74BB97E0D0B6D573661CB3481_RuntimeMethod_var);
	}

IL_0058:
	{
		int32_t L_14 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_14, (int32_t)1));
	}

IL_005d:
	{
		int32_t L_15 = V_0;
		V_2 = (bool)((((int32_t)((((int32_t)L_15) < ((int32_t)0))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_16 = V_2;
		if (L_16)
		{
			goto IL_0010;
		}
	}
	{
		return;
	}
}
// UnityEngine.IntegratedSubsystem UnityEngine.Internal_SubsystemInstances::Internal_GetInstanceByPtr(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * Internal_SubsystemInstances_Internal_GetInstanceByPtr_m5C2B49DC08EBCC5036465D8928090D71F1D420F5 (intptr_t ___ptr0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Internal_SubsystemInstances_Internal_GetInstanceByPtr_m5C2B49DC08EBCC5036465D8928090D71F1D420F5_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658  V_0;
	memset((&V_0), 0, sizeof(V_0));
	IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * V_1 = NULL;
	bool V_2 = false;
	IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * V_3 = NULL;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 2);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		IL2CPP_RUNTIME_CLASS_INIT(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var);
		List_1_t5FED37944BE574E3B297791169909363750C8A64 * L_0 = ((Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var))->get_s_IntegratedSubsystemInstances_0();
		NullCheck(L_0);
		Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658  L_1 = List_1_GetEnumerator_m0D7B7D04AAB181B6AA7525709026E68258ED5740(L_0, /*hidden argument*/List_1_GetEnumerator_m0D7B7D04AAB181B6AA7525709026E68258ED5740_RuntimeMethod_var);
		V_0 = L_1;
	}

IL_000d:
	try
	{ // begin try (depth: 1)
		{
			goto IL_0032;
		}

IL_000f:
		{
			RuntimeObject* L_2 = Enumerator_get_Current_m08AAF79B6D539D33DC62A4630DD67D1D8CF22D9E_inline((Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658 *)(&V_0), /*hidden argument*/Enumerator_get_Current_m08AAF79B6D539D33DC62A4630DD67D1D8CF22D9E_RuntimeMethod_var);
			V_1 = ((IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 *)CastclassClass((RuntimeObject*)L_2, IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002_il2cpp_TypeInfo_var));
			IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * L_3 = V_1;
			NullCheck(L_3);
			intptr_t L_4 = L_3->get_m_Ptr_0();
			intptr_t L_5 = ___ptr0;
			bool L_6 = IntPtr_op_Equality_mD94F3FE43A65684EFF984A7B95E70D2520C0AC73((intptr_t)L_4, (intptr_t)L_5, /*hidden argument*/NULL);
			V_2 = L_6;
			bool L_7 = V_2;
			if (!L_7)
			{
				goto IL_0031;
			}
		}

IL_002d:
		{
			IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * L_8 = V_1;
			V_3 = L_8;
			IL2CPP_LEAVE(0x50, FINALLY_003d);
		}

IL_0031:
		{
		}

IL_0032:
		{
			bool L_9 = Enumerator_MoveNext_m6090B713C1599612E9E959204C495E69FF635A6B((Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658 *)(&V_0), /*hidden argument*/Enumerator_MoveNext_m6090B713C1599612E9E959204C495E69FF635A6B_RuntimeMethod_var);
			if (L_9)
			{
				goto IL_000f;
			}
		}

IL_003b:
		{
			IL2CPP_LEAVE(0x4C, FINALLY_003d);
		}
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_003d;
	}

FINALLY_003d:
	{ // begin finally (depth: 1)
		Enumerator_Dispose_mEFBC03EAF32221CE654B48D4D21F15D5B57F1C35((Enumerator_tFAF2BD91451AE46FB0C7D13F5773B47E2F91F658 *)(&V_0), /*hidden argument*/Enumerator_Dispose_mEFBC03EAF32221CE654B48D4D21F15D5B57F1C35_RuntimeMethod_var);
		IL2CPP_END_FINALLY(61)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(61)
	{
		IL2CPP_JUMP_TBL(0x50, IL_0050)
		IL2CPP_JUMP_TBL(0x4C, IL_004c)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_004c:
	{
		V_3 = (IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 *)NULL;
		goto IL_0050;
	}

IL_0050:
	{
		IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * L_10 = V_3;
		return L_10;
	}
}
// System.Void UnityEngine.Internal_SubsystemInstances::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Internal_SubsystemInstances__cctor_m98CFC5909AD1A6B7C085D0F1FFE1D5E8BDA14B83 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Internal_SubsystemInstances__cctor_m98CFC5909AD1A6B7C085D0F1FFE1D5E8BDA14B83_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		List_1_t5FED37944BE574E3B297791169909363750C8A64 * L_0 = (List_1_t5FED37944BE574E3B297791169909363750C8A64 *)il2cpp_codegen_object_new(List_1_t5FED37944BE574E3B297791169909363750C8A64_il2cpp_TypeInfo_var);
		List_1__ctor_mE7AF9A04C421B1097120D9179F9AE7E559E3DB83(L_0, /*hidden argument*/List_1__ctor_mE7AF9A04C421B1097120D9179F9AE7E559E3DB83_RuntimeMethod_var);
		((Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var))->set_s_IntegratedSubsystemInstances_0(L_0);
		List_1_t5FED37944BE574E3B297791169909363750C8A64 * L_1 = (List_1_t5FED37944BE574E3B297791169909363750C8A64 *)il2cpp_codegen_object_new(List_1_t5FED37944BE574E3B297791169909363750C8A64_il2cpp_TypeInfo_var);
		List_1__ctor_mE7AF9A04C421B1097120D9179F9AE7E559E3DB83(L_1, /*hidden argument*/List_1__ctor_mE7AF9A04C421B1097120D9179F9AE7E559E3DB83_RuntimeMethod_var);
		((Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_StaticFields*)il2cpp_codegen_static_fields_for(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var))->set_s_StandaloneSubsystemInstances_1(L_1);
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
// System.String UnityEngine.SubsystemDescriptor::get_id()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* SubsystemDescriptor_get_id_mC3D7972588D4B57F906A06CEE54A61B55D1306DB (SubsystemDescriptor_tF663011CB44AB1D342821BBEF7B6811E799A7245 * __this, const RuntimeMethod* method)
{
	{
		String_t* L_0 = __this->get_U3CidU3Ek__BackingField_0();
		return L_0;
	}
}
// System.Void UnityEngine.SubsystemDescriptor::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SubsystemDescriptor__ctor_m6BE761B6ADD71B136BCECB0F54B82801B7222114 (SubsystemDescriptor_tF663011CB44AB1D342821BBEF7B6811E799A7245 * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405(__this, /*hidden argument*/NULL);
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
// System.Void UnityEngine.SubsystemManager::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SubsystemManager__cctor_m623E9CEA1E2F73608F88CAA9F087A9A9A8A139AB (const RuntimeMethod* method)
{
	{
		SubsystemManager_StaticConstructScriptingClassMap_m2D54CD8E480B334F4EC0EAAF49DE4186895885EE(/*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.SubsystemManager::ReportSingleSubsystemAnalytics(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SubsystemManager_ReportSingleSubsystemAnalytics_mEFB0497B98F43D9D4174E870062A06C9313AC783 (String_t* ___id0, const RuntimeMethod* method)
{
	typedef void (*SubsystemManager_ReportSingleSubsystemAnalytics_mEFB0497B98F43D9D4174E870062A06C9313AC783_ftn) (String_t*);
	static SubsystemManager_ReportSingleSubsystemAnalytics_mEFB0497B98F43D9D4174E870062A06C9313AC783_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (SubsystemManager_ReportSingleSubsystemAnalytics_mEFB0497B98F43D9D4174E870062A06C9313AC783_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.SubsystemManager::ReportSingleSubsystemAnalytics(System.String)");
	_il2cpp_icall_func(___id0);
}
// System.Void UnityEngine.SubsystemManager::StaticConstructScriptingClassMap()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SubsystemManager_StaticConstructScriptingClassMap_m2D54CD8E480B334F4EC0EAAF49DE4186895885EE (const RuntimeMethod* method)
{
	typedef void (*SubsystemManager_StaticConstructScriptingClassMap_m2D54CD8E480B334F4EC0EAAF49DE4186895885EE_ftn) ();
	static SubsystemManager_StaticConstructScriptingClassMap_m2D54CD8E480B334F4EC0EAAF49DE4186895885EE_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (SubsystemManager_StaticConstructScriptingClassMap_m2D54CD8E480B334F4EC0EAAF49DE4186895885EE_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.SubsystemManager::StaticConstructScriptingClassMap()");
	_il2cpp_icall_func();
}
// System.Void UnityEngine.SubsystemManager::Internal_ReloadSubsystemsStarted()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SubsystemManager_Internal_ReloadSubsystemsStarted_mF2B81F13FA0F97A5C0BE6FAF36B5CB4F2130ED12 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SubsystemManager_Internal_ReloadSubsystemsStarted_mF2B81F13FA0F97A5C0BE6FAF36B5CB4F2130ED12_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		IL2CPP_RUNTIME_CLASS_INIT(SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_il2cpp_TypeInfo_var);
		Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * L_0 = ((SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_StaticFields*)il2cpp_codegen_static_fields_for(SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_il2cpp_TypeInfo_var))->get_reloadSubsytemsStarted_0();
		V_0 = (bool)((!(((RuntimeObject*)(Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 *)L_0) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_1 = V_0;
		if (!L_1)
		{
			goto IL_001a;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_il2cpp_TypeInfo_var);
		Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * L_2 = ((SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_StaticFields*)il2cpp_codegen_static_fields_for(SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_il2cpp_TypeInfo_var))->get_reloadSubsytemsStarted_0();
		NullCheck(L_2);
		Action_Invoke_m3FFA5BE3D64F0FF8E1E1CB6F953913FADB5EB89E(L_2, /*hidden argument*/NULL);
	}

IL_001a:
	{
		return;
	}
}
// System.Void UnityEngine.SubsystemManager::Internal_ReloadSubsystemsCompleted()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SubsystemManager_Internal_ReloadSubsystemsCompleted_m25F9188AF832B1DDF30524CF460B06E06307E6D3 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SubsystemManager_Internal_ReloadSubsystemsCompleted_m25F9188AF832B1DDF30524CF460B06E06307E6D3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		IL2CPP_RUNTIME_CLASS_INIT(SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_il2cpp_TypeInfo_var);
		Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * L_0 = ((SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_StaticFields*)il2cpp_codegen_static_fields_for(SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_il2cpp_TypeInfo_var))->get_reloadSubsytemsCompleted_1();
		V_0 = (bool)((!(((RuntimeObject*)(Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 *)L_0) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_1 = V_0;
		if (!L_1)
		{
			goto IL_001a;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_il2cpp_TypeInfo_var);
		Action_tAF41423D285AE0862865348CF6CE51CD085ABBA6 * L_2 = ((SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_StaticFields*)il2cpp_codegen_static_fields_for(SubsystemManager_t4397CEF2ED795CB9B3DDBA2BB468BCB6B45B76D9_il2cpp_TypeInfo_var))->get_reloadSubsytemsCompleted_1();
		NullCheck(L_2);
		Action_Invoke_m3FFA5BE3D64F0FF8E1E1CB6F953913FADB5EB89E(L_2, /*hidden argument*/NULL);
	}

IL_001a:
	{
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR String_t* SubsystemDescriptor_get_id_mC3D7972588D4B57F906A06CEE54A61B55D1306DB_inline (SubsystemDescriptor_tF663011CB44AB1D342821BBEF7B6811E799A7245 * __this, const RuntimeMethod* method)
{
	{
		String_t* L_0 = __this->get_U3CidU3Ek__BackingField_0();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject * Enumerator_get_Current_m9C4EBBD2108B51885E750F927D7936290C8E20EE_gshared_inline (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = (RuntimeObject *)__this->get_current_3();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t List_1_get_Count_m5D847939ABB9A78203B062CAFFE975792174D00F_gshared_inline (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = (int32_t)__this->get__size_2();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject * List_1_get_Item_mF00B574E58FB078BB753B05A3B86DD0A7A266B63_gshared_inline (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * __this, int32_t ___index0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___index0;
		int32_t L_1 = (int32_t)__this->get__size_2();
		if ((!(((uint32_t)L_0) >= ((uint32_t)L_1))))
		{
			goto IL_000e;
		}
	}
	{
		ThrowHelper_ThrowArgumentOutOfRangeException_m4841366ABC2B2AFA37C10900551D7E07522C0929(/*hidden argument*/NULL);
	}

IL_000e:
	{
		ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* L_2 = (ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*)__this->get__items_1();
		int32_t L_3 = ___index0;
		RuntimeObject * L_4 = IL2CPP_ARRAY_UNSAFE_LOAD((ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*)L_2, (int32_t)L_3);
		return L_4;
	}
}
