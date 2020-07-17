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
template <typename R, typename T1>
struct VirtFuncInvoker1
{
	typedef R (*Func)(void*, T1, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};

// System.AsyncCallback
struct AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA;
// System.DelegateData
struct DelegateData_t17DD30660E330C49381DAA99F934BE75CB11F288;
// System.IAsyncResult
struct IAsyncResult_tC9F97BF36FCF122D29D3101D80642278297BF370;
// System.Collections.IDictionary
struct IDictionary_t99871C56B8EC2452AC5C4CF3831695E617B89D3A;
// UnityEngine.ISubsystemDescriptor
struct ISubsystemDescriptor_tEB935323042076ECFC076435FBD756B1E7953A14;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// System.Runtime.Serialization.SafeSerializationManager
struct SafeSerializationManager_tDE44F029589A028F8A3053C5C06153FAB4AAE29F;
// System.Void
struct Void_t700C6383A2A510C2CF4DD86DABD5CA9FF70ADAC5;
// UnityEngine.XR.XRDisplaySubsystem
struct XRDisplaySubsystem_tF8B46605B6D1199C52306D4EC7D83CFA90564A93;
// UnityEngine.XR.XRDisplaySubsystemDescriptor
struct XRDisplaySubsystemDescriptor_tBBE6956FF61EACF13E72BFEF58ADC5930C760833;
// UnityEngine.XR.XRInputSubsystem
struct XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09;
// UnityEngine.XR.XRInputSubsystemDescriptor
struct XRInputSubsystemDescriptor_t98C4233948EC9169B71D2A58C2C6ED1AF6FDABC2;
// UnityEngine.XR.XRMeshSubsystem
struct XRMeshSubsystem_t60BD977DF1B014CF5D48C8EBCC91DED767520D63;
// UnityEngine.XR.XRMeshSubsystemDescriptor
struct XRMeshSubsystemDescriptor_t428853FE3628F349D46DFD6841B50058F09F5FCC;
// System.Action`1<UnityEngine.XR.InputDevice>
struct Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8;
// System.Action`1<UnityEngine.XR.MeshGenerationResult>
struct Action_1_tB125CDA27D619FDBF92F767804A14CF83EA85A3C;
// System.Action`1<UnityEngine.XR.XRInputSubsystem>
struct Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1;
// System.Action`1<UnityEngine.XR.XRNodeState>
struct Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603;
// System.Action`1<System.Boolean>
struct Action_1_tCE2D770918A65CAD277C08C4E8C05385EA267E83;
// System.Action`1<System.Object>
struct Action_1_tD9663D9715FAA4E62035CFCF1AD4D094EE7872DC;
// UnityEngine.IntegratedSubsystemDescriptor`1<UnityEngine.XR.XRDisplaySubsystem>
struct IntegratedSubsystemDescriptor_1_tFDF96CDD8FD2E980FF0C62E8161C66AF9FC212E2;
// UnityEngine.IntegratedSubsystemDescriptor`1<UnityEngine.XR.XRInputSubsystem>
struct IntegratedSubsystemDescriptor_1_t7D61E241AA40ECC23A367A5FAF509A54B1F77EF2;
// UnityEngine.IntegratedSubsystemDescriptor`1<UnityEngine.XR.XRMeshSubsystem>
struct IntegratedSubsystemDescriptor_1_t822E08B2CE1EC68FE74F71A682C9ECC6D52A6E89;
// UnityEngine.IntegratedSubsystemDescriptor`1<System.Object>
struct IntegratedSubsystemDescriptor_1_t4BFDAEC6A4D96827E7D4D0B2E85EB1AFA1911939;
// UnityEngine.IntegratedSubsystem`1<UnityEngine.XR.XRDisplaySubsystemDescriptor>
struct IntegratedSubsystem_1_t2737E0F52E6DC7B2E3D42D1B05C5FD7C9FDE4EA4;
// UnityEngine.IntegratedSubsystem`1<UnityEngine.XR.XRInputSubsystemDescriptor>
struct IntegratedSubsystem_1_tD5C4AF38726B9433CFC3CA0F889D8C8C2535AEFE;
// UnityEngine.IntegratedSubsystem`1<UnityEngine.XR.XRMeshSubsystemDescriptor>
struct IntegratedSubsystem_1_t902A5B61CE879B3CD855E5CE6CAEEB1B9752E840;
// UnityEngine.IntegratedSubsystem`1<System.Object>
struct IntegratedSubsystem_1_t0B19871ED45EAD9F0E0DD6AB41BABCAFBD8C56E4;
// System.ArgumentException
struct ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00;
// System.Char[]
struct CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34;
// System.Delegate[]
struct DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8;
// System.Int32[]
struct Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32;
// System.IntPtr[]
struct IntPtrU5BU5D_t27FC72B0409D75AAF33EC42498E8094E95FEE9A6;
// UnityEngine.IntegratedSubsystem
struct IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002;
// UnityEngine.Mesh
struct Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6;
// UnityEngine.MeshCollider
struct MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98;
// System.Diagnostics.StackTrace[]
struct StackTraceU5BU5D_t4AD999C288CB6D1F38A299D12B1598D606588971;
// System.String
struct String_t;

IL2CPP_EXTERN_C RuntimeClass* ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Hand_tB64007EC8D01384426C93432737BA9C5F636A690_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* MeshGenerationStatus_t25EB712EAD94A279AD7D5A00E0CB6EDC8AB1FE79_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* MeshVertexAttributes_t7CCF6BE6BB4E908E1ECF9F9AF76968FA38A672CE_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TrackingStateEventType_t301E0DD44D089E06B0BBA994F682CE9F23505BA5_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteralF953F17BB91EBF78300169DEE55CE060B4F1C569;
IL2CPP_EXTERN_C String_t* _stringLiteralFBAF124AB08242B7785EC2B6DBC3C6459CB98BC8;
IL2CPP_EXTERN_C String_t* _stringLiteralFDA1C52D0E58360F4E8FD608757CCD98D8772D4F;
IL2CPP_EXTERN_C const RuntimeMethod* Action_1_Invoke_m95E5C9FC67F7B0BDC3CD5E00AC5D4C8A00C97AC5_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Action_1_Invoke_mA460B359B13BDCF71CDDE9A8A960C827A4E3E970_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Action_1_Invoke_mA71F13E5E1EFDEB1DB1D9ED4C7ED037B21A89939_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Action_1_Invoke_mC3DCAEAD9DC81FE145B4FE115F830C0767728604_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Action_1_Invoke_mD7440CB91FE64B4EAD0D34248075E0F39797C946_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* InputTracking_InvokeTrackingEvent_mF9CC9853D284F640ACEB29225EF35646166061A0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* IntegratedSubsystemDescriptor_1__ctor_m3E9F6A2B441E056953C153C3B3182C0EB6BD0AFE_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* IntegratedSubsystemDescriptor_1__ctor_m98CC72EADB42D92099DBE358C296423D7751A741_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* IntegratedSubsystemDescriptor_1__ctor_mBD46E84CF05A1E63F8FE6AB0C2F1C07AA8D2DAFB_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* IntegratedSubsystem_1__ctor_m19C9BE11CA13915E2E14D5B4EC3EAF29CCC633E5_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* IntegratedSubsystem_1__ctor_m33699A39FA5AEAE5A383689E4E0E3523FB67E558_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* IntegratedSubsystem_1__ctor_m5D5CDD514B75369B0797B55401D9DD35908A2A26_RuntimeMethod_var;
IL2CPP_EXTERN_C const uint32_t Bone_Equals_m2FBDCFEA8B90663E546294EBEB4763538DEEA412_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Eyes_Equals_m58897DB2EEC48809233B94BB4CEA166B8ACEBFF2_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Hand_Equals_m727113281F30E554A3A60DEFC4ED61CC94901775_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t InputDevice_Equals_mF29A225E81A87941551F70A2351CB803A6D94063_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t InputDevices_InvokeConnectionEvent_m19E87BB6671D4B4CE3EB322EEE3621B0146A7077_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t InputFeatureUsage_Equals_mC0A1A665A98F42B2D5896BB9BC4CBA42FB59D582_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t InputTracking_InvokeTrackingEvent_mF9CC9853D284F640ACEB29225EF35646166061A0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t InputTracking__cctor_m8C342AE21A5D67A0378CE91016DBCCFFC62E34ED_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t MeshGenerationResult_Equals_m511B6FD46B1187D90919F4C0D2E853DE4A16BD44_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t MeshGenerationResult_Equals_mA9685353D67F169013BF8CC6B34EAFE69DB341B4_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t MeshId_Equals_m77D4535F7643D5C1FEA20600C92B73818DD8675E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t MeshId_ToString_mA9CDBA01DD0C110252F6C4AA7437C507B2025705_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t MeshId__cctor_m98D91783008597CCFFBC675648A50107318509D7_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t XRDisplaySubsystemDescriptor__ctor_mB045E1EBFB4D2B7CBE05D85D5AC622F7A971E056_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t XRDisplaySubsystem_InvokeDisplayFocusChanged_mF8F7E4D08B964907140FD3F8841F130159C7DBA7_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t XRDisplaySubsystem__ctor_mCC516BAAAC7175CC9CEECA04E488F5D9BF0FB774_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t XRInputSubsystemDescriptor__ctor_m1620DD409E907F5AAA03D3DE504AC48D9D3E9576_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t XRInputSubsystem_InvokeBoundaryChangedEvent_m795C2897F3A0047BBA6834D3F97B5DAFDEC4AE7A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t XRInputSubsystem_InvokeTrackingOriginUpdatedEvent_m8A70C0391D141C9189155AAAD3D16493243A23D5_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t XRInputSubsystem__ctor_m80AE13105C9C373B38E4814244886DCB7AA3E7E8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t XRMeshSubsystemDescriptor__ctor_m52169EE2174077BA5575410A1031C23915BBA6D2_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t XRMeshSubsystem_InvokeMeshReadyDelegate_mDD6390D57F2CD0F7EBD64B628DB780D23424FD8F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t XRMeshSubsystem__ctor_mBA5B9B6A417BB2B477705E8BF6D1BFACF94AEF74_MetadataUsageId;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;


IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// <Module>
struct  U3CModuleU3E_t1E1B852027794298A682FBC1BEE318B6EABAD94F 
{
public:

public:
};


// System.Object

struct Il2CppArrayBounds;

// System.Array


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

// UnityEngine.XR.HashCodeHelper
struct  HashCodeHelper_t55705027308438F4124A7ABBE1F3A2E503C9200B  : public RuntimeObject
{
public:

public:
};


// UnityEngine.XR.InputDevices
struct  InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC  : public RuntimeObject
{
public:

public:
};

struct InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_StaticFields
{
public:
	// System.Action`1<UnityEngine.XR.InputDevice> UnityEngine.XR.InputDevices::deviceConnected
	Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * ___deviceConnected_0;
	// System.Action`1<UnityEngine.XR.InputDevice> UnityEngine.XR.InputDevices::deviceDisconnected
	Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * ___deviceDisconnected_1;
	// System.Action`1<UnityEngine.XR.InputDevice> UnityEngine.XR.InputDevices::deviceConfigChanged
	Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * ___deviceConfigChanged_2;

public:
	inline static int32_t get_offset_of_deviceConnected_0() { return static_cast<int32_t>(offsetof(InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_StaticFields, ___deviceConnected_0)); }
	inline Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * get_deviceConnected_0() const { return ___deviceConnected_0; }
	inline Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 ** get_address_of_deviceConnected_0() { return &___deviceConnected_0; }
	inline void set_deviceConnected_0(Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * value)
	{
		___deviceConnected_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___deviceConnected_0), (void*)value);
	}

	inline static int32_t get_offset_of_deviceDisconnected_1() { return static_cast<int32_t>(offsetof(InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_StaticFields, ___deviceDisconnected_1)); }
	inline Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * get_deviceDisconnected_1() const { return ___deviceDisconnected_1; }
	inline Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 ** get_address_of_deviceDisconnected_1() { return &___deviceDisconnected_1; }
	inline void set_deviceDisconnected_1(Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * value)
	{
		___deviceDisconnected_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___deviceDisconnected_1), (void*)value);
	}

	inline static int32_t get_offset_of_deviceConfigChanged_2() { return static_cast<int32_t>(offsetof(InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_StaticFields, ___deviceConfigChanged_2)); }
	inline Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * get_deviceConfigChanged_2() const { return ___deviceConfigChanged_2; }
	inline Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 ** get_address_of_deviceConfigChanged_2() { return &___deviceConfigChanged_2; }
	inline void set_deviceConfigChanged_2(Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * value)
	{
		___deviceConfigChanged_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___deviceConfigChanged_2), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.XR.InputDevices
struct InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_marshaled_pinvoke
{
};
// Native definition for COM marshalling of UnityEngine.XR.InputDevices
struct InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_marshaled_com
{
};

// UnityEngine.XR.InputTracking
struct  InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D  : public RuntimeObject
{
public:

public:
};

struct InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_StaticFields
{
public:
	// System.Action`1<UnityEngine.XR.XRNodeState> UnityEngine.XR.InputTracking::trackingAcquired
	Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * ___trackingAcquired_0;
	// System.Action`1<UnityEngine.XR.XRNodeState> UnityEngine.XR.InputTracking::trackingLost
	Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * ___trackingLost_1;
	// System.Action`1<UnityEngine.XR.XRNodeState> UnityEngine.XR.InputTracking::nodeAdded
	Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * ___nodeAdded_2;
	// System.Action`1<UnityEngine.XR.XRNodeState> UnityEngine.XR.InputTracking::nodeRemoved
	Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * ___nodeRemoved_3;

public:
	inline static int32_t get_offset_of_trackingAcquired_0() { return static_cast<int32_t>(offsetof(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_StaticFields, ___trackingAcquired_0)); }
	inline Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * get_trackingAcquired_0() const { return ___trackingAcquired_0; }
	inline Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 ** get_address_of_trackingAcquired_0() { return &___trackingAcquired_0; }
	inline void set_trackingAcquired_0(Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * value)
	{
		___trackingAcquired_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___trackingAcquired_0), (void*)value);
	}

	inline static int32_t get_offset_of_trackingLost_1() { return static_cast<int32_t>(offsetof(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_StaticFields, ___trackingLost_1)); }
	inline Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * get_trackingLost_1() const { return ___trackingLost_1; }
	inline Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 ** get_address_of_trackingLost_1() { return &___trackingLost_1; }
	inline void set_trackingLost_1(Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * value)
	{
		___trackingLost_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___trackingLost_1), (void*)value);
	}

	inline static int32_t get_offset_of_nodeAdded_2() { return static_cast<int32_t>(offsetof(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_StaticFields, ___nodeAdded_2)); }
	inline Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * get_nodeAdded_2() const { return ___nodeAdded_2; }
	inline Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 ** get_address_of_nodeAdded_2() { return &___nodeAdded_2; }
	inline void set_nodeAdded_2(Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * value)
	{
		___nodeAdded_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___nodeAdded_2), (void*)value);
	}

	inline static int32_t get_offset_of_nodeRemoved_3() { return static_cast<int32_t>(offsetof(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_StaticFields, ___nodeRemoved_3)); }
	inline Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * get_nodeRemoved_3() const { return ___nodeRemoved_3; }
	inline Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 ** get_address_of_nodeRemoved_3() { return &___nodeRemoved_3; }
	inline void set_nodeRemoved_3(Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * value)
	{
		___nodeRemoved_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___nodeRemoved_3), (void*)value);
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


// UnityEngine.Quaternion
struct  Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4 
{
public:
	// System.Single UnityEngine.Quaternion::x
	float ___x_0;
	// System.Single UnityEngine.Quaternion::y
	float ___y_1;
	// System.Single UnityEngine.Quaternion::z
	float ___z_2;
	// System.Single UnityEngine.Quaternion::w
	float ___w_3;

public:
	inline static int32_t get_offset_of_x_0() { return static_cast<int32_t>(offsetof(Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4, ___x_0)); }
	inline float get_x_0() const { return ___x_0; }
	inline float* get_address_of_x_0() { return &___x_0; }
	inline void set_x_0(float value)
	{
		___x_0 = value;
	}

	inline static int32_t get_offset_of_y_1() { return static_cast<int32_t>(offsetof(Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4, ___y_1)); }
	inline float get_y_1() const { return ___y_1; }
	inline float* get_address_of_y_1() { return &___y_1; }
	inline void set_y_1(float value)
	{
		___y_1 = value;
	}

	inline static int32_t get_offset_of_z_2() { return static_cast<int32_t>(offsetof(Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4, ___z_2)); }
	inline float get_z_2() const { return ___z_2; }
	inline float* get_address_of_z_2() { return &___z_2; }
	inline void set_z_2(float value)
	{
		___z_2 = value;
	}

	inline static int32_t get_offset_of_w_3() { return static_cast<int32_t>(offsetof(Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4, ___w_3)); }
	inline float get_w_3() const { return ___w_3; }
	inline float* get_address_of_w_3() { return &___w_3; }
	inline void set_w_3(float value)
	{
		___w_3 = value;
	}
};

struct Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4_StaticFields
{
public:
	// UnityEngine.Quaternion UnityEngine.Quaternion::identityQuaternion
	Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4  ___identityQuaternion_4;

public:
	inline static int32_t get_offset_of_identityQuaternion_4() { return static_cast<int32_t>(offsetof(Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4_StaticFields, ___identityQuaternion_4)); }
	inline Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4  get_identityQuaternion_4() const { return ___identityQuaternion_4; }
	inline Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4 * get_address_of_identityQuaternion_4() { return &___identityQuaternion_4; }
	inline void set_identityQuaternion_4(Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4  value)
	{
		___identityQuaternion_4 = value;
	}
};


// UnityEngine.Vector3
struct  Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E 
{
public:
	// System.Single UnityEngine.Vector3::x
	float ___x_2;
	// System.Single UnityEngine.Vector3::y
	float ___y_3;
	// System.Single UnityEngine.Vector3::z
	float ___z_4;

public:
	inline static int32_t get_offset_of_x_2() { return static_cast<int32_t>(offsetof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E, ___x_2)); }
	inline float get_x_2() const { return ___x_2; }
	inline float* get_address_of_x_2() { return &___x_2; }
	inline void set_x_2(float value)
	{
		___x_2 = value;
	}

	inline static int32_t get_offset_of_y_3() { return static_cast<int32_t>(offsetof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E, ___y_3)); }
	inline float get_y_3() const { return ___y_3; }
	inline float* get_address_of_y_3() { return &___y_3; }
	inline void set_y_3(float value)
	{
		___y_3 = value;
	}

	inline static int32_t get_offset_of_z_4() { return static_cast<int32_t>(offsetof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E, ___z_4)); }
	inline float get_z_4() const { return ___z_4; }
	inline float* get_address_of_z_4() { return &___z_4; }
	inline void set_z_4(float value)
	{
		___z_4 = value;
	}
};

struct Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E_StaticFields
{
public:
	// UnityEngine.Vector3 UnityEngine.Vector3::zeroVector
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___zeroVector_5;
	// UnityEngine.Vector3 UnityEngine.Vector3::oneVector
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___oneVector_6;
	// UnityEngine.Vector3 UnityEngine.Vector3::upVector
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___upVector_7;
	// UnityEngine.Vector3 UnityEngine.Vector3::downVector
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___downVector_8;
	// UnityEngine.Vector3 UnityEngine.Vector3::leftVector
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___leftVector_9;
	// UnityEngine.Vector3 UnityEngine.Vector3::rightVector
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___rightVector_10;
	// UnityEngine.Vector3 UnityEngine.Vector3::forwardVector
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___forwardVector_11;
	// UnityEngine.Vector3 UnityEngine.Vector3::backVector
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___backVector_12;
	// UnityEngine.Vector3 UnityEngine.Vector3::positiveInfinityVector
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___positiveInfinityVector_13;
	// UnityEngine.Vector3 UnityEngine.Vector3::negativeInfinityVector
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___negativeInfinityVector_14;

public:
	inline static int32_t get_offset_of_zeroVector_5() { return static_cast<int32_t>(offsetof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E_StaticFields, ___zeroVector_5)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_zeroVector_5() const { return ___zeroVector_5; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_zeroVector_5() { return &___zeroVector_5; }
	inline void set_zeroVector_5(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___zeroVector_5 = value;
	}

	inline static int32_t get_offset_of_oneVector_6() { return static_cast<int32_t>(offsetof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E_StaticFields, ___oneVector_6)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_oneVector_6() const { return ___oneVector_6; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_oneVector_6() { return &___oneVector_6; }
	inline void set_oneVector_6(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___oneVector_6 = value;
	}

	inline static int32_t get_offset_of_upVector_7() { return static_cast<int32_t>(offsetof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E_StaticFields, ___upVector_7)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_upVector_7() const { return ___upVector_7; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_upVector_7() { return &___upVector_7; }
	inline void set_upVector_7(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___upVector_7 = value;
	}

	inline static int32_t get_offset_of_downVector_8() { return static_cast<int32_t>(offsetof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E_StaticFields, ___downVector_8)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_downVector_8() const { return ___downVector_8; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_downVector_8() { return &___downVector_8; }
	inline void set_downVector_8(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___downVector_8 = value;
	}

	inline static int32_t get_offset_of_leftVector_9() { return static_cast<int32_t>(offsetof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E_StaticFields, ___leftVector_9)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_leftVector_9() const { return ___leftVector_9; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_leftVector_9() { return &___leftVector_9; }
	inline void set_leftVector_9(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___leftVector_9 = value;
	}

	inline static int32_t get_offset_of_rightVector_10() { return static_cast<int32_t>(offsetof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E_StaticFields, ___rightVector_10)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_rightVector_10() const { return ___rightVector_10; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_rightVector_10() { return &___rightVector_10; }
	inline void set_rightVector_10(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___rightVector_10 = value;
	}

	inline static int32_t get_offset_of_forwardVector_11() { return static_cast<int32_t>(offsetof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E_StaticFields, ___forwardVector_11)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_forwardVector_11() const { return ___forwardVector_11; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_forwardVector_11() { return &___forwardVector_11; }
	inline void set_forwardVector_11(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___forwardVector_11 = value;
	}

	inline static int32_t get_offset_of_backVector_12() { return static_cast<int32_t>(offsetof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E_StaticFields, ___backVector_12)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_backVector_12() const { return ___backVector_12; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_backVector_12() { return &___backVector_12; }
	inline void set_backVector_12(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___backVector_12 = value;
	}

	inline static int32_t get_offset_of_positiveInfinityVector_13() { return static_cast<int32_t>(offsetof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E_StaticFields, ___positiveInfinityVector_13)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_positiveInfinityVector_13() const { return ___positiveInfinityVector_13; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_positiveInfinityVector_13() { return &___positiveInfinityVector_13; }
	inline void set_positiveInfinityVector_13(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___positiveInfinityVector_13 = value;
	}

	inline static int32_t get_offset_of_negativeInfinityVector_14() { return static_cast<int32_t>(offsetof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E_StaticFields, ___negativeInfinityVector_14)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_negativeInfinityVector_14() const { return ___negativeInfinityVector_14; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_negativeInfinityVector_14() { return &___negativeInfinityVector_14; }
	inline void set_negativeInfinityVector_14(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___negativeInfinityVector_14 = value;
	}
};


// UnityEngine.XR.Bone
struct  Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 
{
public:
	// System.UInt64 UnityEngine.XR.Bone::m_DeviceId
	uint64_t ___m_DeviceId_0;
	// System.UInt32 UnityEngine.XR.Bone::m_FeatureIndex
	uint32_t ___m_FeatureIndex_1;

public:
	inline static int32_t get_offset_of_m_DeviceId_0() { return static_cast<int32_t>(offsetof(Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070, ___m_DeviceId_0)); }
	inline uint64_t get_m_DeviceId_0() const { return ___m_DeviceId_0; }
	inline uint64_t* get_address_of_m_DeviceId_0() { return &___m_DeviceId_0; }
	inline void set_m_DeviceId_0(uint64_t value)
	{
		___m_DeviceId_0 = value;
	}

	inline static int32_t get_offset_of_m_FeatureIndex_1() { return static_cast<int32_t>(offsetof(Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070, ___m_FeatureIndex_1)); }
	inline uint32_t get_m_FeatureIndex_1() const { return ___m_FeatureIndex_1; }
	inline uint32_t* get_address_of_m_FeatureIndex_1() { return &___m_FeatureIndex_1; }
	inline void set_m_FeatureIndex_1(uint32_t value)
	{
		___m_FeatureIndex_1 = value;
	}
};


// UnityEngine.XR.Eyes
struct  Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D 
{
public:
	// System.UInt64 UnityEngine.XR.Eyes::m_DeviceId
	uint64_t ___m_DeviceId_0;
	// System.UInt32 UnityEngine.XR.Eyes::m_FeatureIndex
	uint32_t ___m_FeatureIndex_1;

public:
	inline static int32_t get_offset_of_m_DeviceId_0() { return static_cast<int32_t>(offsetof(Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D, ___m_DeviceId_0)); }
	inline uint64_t get_m_DeviceId_0() const { return ___m_DeviceId_0; }
	inline uint64_t* get_address_of_m_DeviceId_0() { return &___m_DeviceId_0; }
	inline void set_m_DeviceId_0(uint64_t value)
	{
		___m_DeviceId_0 = value;
	}

	inline static int32_t get_offset_of_m_FeatureIndex_1() { return static_cast<int32_t>(offsetof(Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D, ___m_FeatureIndex_1)); }
	inline uint32_t get_m_FeatureIndex_1() const { return ___m_FeatureIndex_1; }
	inline uint32_t* get_address_of_m_FeatureIndex_1() { return &___m_FeatureIndex_1; }
	inline void set_m_FeatureIndex_1(uint32_t value)
	{
		___m_FeatureIndex_1 = value;
	}
};


// UnityEngine.XR.Hand
struct  Hand_tB64007EC8D01384426C93432737BA9C5F636A690 
{
public:
	// System.UInt64 UnityEngine.XR.Hand::m_DeviceId
	uint64_t ___m_DeviceId_0;
	// System.UInt32 UnityEngine.XR.Hand::m_FeatureIndex
	uint32_t ___m_FeatureIndex_1;

public:
	inline static int32_t get_offset_of_m_DeviceId_0() { return static_cast<int32_t>(offsetof(Hand_tB64007EC8D01384426C93432737BA9C5F636A690, ___m_DeviceId_0)); }
	inline uint64_t get_m_DeviceId_0() const { return ___m_DeviceId_0; }
	inline uint64_t* get_address_of_m_DeviceId_0() { return &___m_DeviceId_0; }
	inline void set_m_DeviceId_0(uint64_t value)
	{
		___m_DeviceId_0 = value;
	}

	inline static int32_t get_offset_of_m_FeatureIndex_1() { return static_cast<int32_t>(offsetof(Hand_tB64007EC8D01384426C93432737BA9C5F636A690, ___m_FeatureIndex_1)); }
	inline uint32_t get_m_FeatureIndex_1() const { return ___m_FeatureIndex_1; }
	inline uint32_t* get_address_of_m_FeatureIndex_1() { return &___m_FeatureIndex_1; }
	inline void set_m_FeatureIndex_1(uint32_t value)
	{
		___m_FeatureIndex_1 = value;
	}
};


// UnityEngine.XR.InputDevice
struct  InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E 
{
public:
	// System.UInt64 UnityEngine.XR.InputDevice::m_DeviceId
	uint64_t ___m_DeviceId_0;
	// System.Boolean UnityEngine.XR.InputDevice::m_Initialized
	bool ___m_Initialized_1;

public:
	inline static int32_t get_offset_of_m_DeviceId_0() { return static_cast<int32_t>(offsetof(InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E, ___m_DeviceId_0)); }
	inline uint64_t get_m_DeviceId_0() const { return ___m_DeviceId_0; }
	inline uint64_t* get_address_of_m_DeviceId_0() { return &___m_DeviceId_0; }
	inline void set_m_DeviceId_0(uint64_t value)
	{
		___m_DeviceId_0 = value;
	}

	inline static int32_t get_offset_of_m_Initialized_1() { return static_cast<int32_t>(offsetof(InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E, ___m_Initialized_1)); }
	inline bool get_m_Initialized_1() const { return ___m_Initialized_1; }
	inline bool* get_address_of_m_Initialized_1() { return &___m_Initialized_1; }
	inline void set_m_Initialized_1(bool value)
	{
		___m_Initialized_1 = value;
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.XR.InputDevice
struct InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_marshaled_pinvoke
{
	uint64_t ___m_DeviceId_0;
	int32_t ___m_Initialized_1;
};
// Native definition for COM marshalling of UnityEngine.XR.InputDevice
struct InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_marshaled_com
{
	uint64_t ___m_DeviceId_0;
	int32_t ___m_Initialized_1;
};

// UnityEngine.XR.MeshId
struct  MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 
{
public:
	// System.UInt64 UnityEngine.XR.MeshId::m_SubId1
	uint64_t ___m_SubId1_1;
	// System.UInt64 UnityEngine.XR.MeshId::m_SubId2
	uint64_t ___m_SubId2_2;

public:
	inline static int32_t get_offset_of_m_SubId1_1() { return static_cast<int32_t>(offsetof(MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767, ___m_SubId1_1)); }
	inline uint64_t get_m_SubId1_1() const { return ___m_SubId1_1; }
	inline uint64_t* get_address_of_m_SubId1_1() { return &___m_SubId1_1; }
	inline void set_m_SubId1_1(uint64_t value)
	{
		___m_SubId1_1 = value;
	}

	inline static int32_t get_offset_of_m_SubId2_2() { return static_cast<int32_t>(offsetof(MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767, ___m_SubId2_2)); }
	inline uint64_t get_m_SubId2_2() const { return ___m_SubId2_2; }
	inline uint64_t* get_address_of_m_SubId2_2() { return &___m_SubId2_2; }
	inline void set_m_SubId2_2(uint64_t value)
	{
		___m_SubId2_2 = value;
	}
};

struct MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767_StaticFields
{
public:
	// UnityEngine.XR.MeshId UnityEngine.XR.MeshId::s_InvalidId
	MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  ___s_InvalidId_0;

public:
	inline static int32_t get_offset_of_s_InvalidId_0() { return static_cast<int32_t>(offsetof(MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767_StaticFields, ___s_InvalidId_0)); }
	inline MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  get_s_InvalidId_0() const { return ___s_InvalidId_0; }
	inline MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 * get_address_of_s_InvalidId_0() { return &___s_InvalidId_0; }
	inline void set_s_InvalidId_0(MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  value)
	{
		___s_InvalidId_0 = value;
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

// UnityEngine.CubemapFace
struct  CubemapFace_t74FBCA71A21252C2E10E256E61FE0B1E09D7B9E5 
{
public:
	// System.Int32 UnityEngine.CubemapFace::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(CubemapFace_t74FBCA71A21252C2E10E256E61FE0B1E09D7B9E5, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Experimental.Rendering.GraphicsFormat
struct  GraphicsFormat_t07A3C024BC77B843C53A369D6FC02ABD27D2AB1D 
{
public:
	// System.Int32 UnityEngine.Experimental.Rendering.GraphicsFormat::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(GraphicsFormat_t07A3C024BC77B843C53A369D6FC02ABD27D2AB1D, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
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

// UnityEngine.Object
struct  Object_tF2F3778131EFF286AF62B7B013A170F95A91571A  : public RuntimeObject
{
public:
	// System.IntPtr UnityEngine.Object::m_CachedPtr
	intptr_t ___m_CachedPtr_0;

public:
	inline static int32_t get_offset_of_m_CachedPtr_0() { return static_cast<int32_t>(offsetof(Object_tF2F3778131EFF286AF62B7B013A170F95A91571A, ___m_CachedPtr_0)); }
	inline intptr_t get_m_CachedPtr_0() const { return ___m_CachedPtr_0; }
	inline intptr_t* get_address_of_m_CachedPtr_0() { return &___m_CachedPtr_0; }
	inline void set_m_CachedPtr_0(intptr_t value)
	{
		___m_CachedPtr_0 = value;
	}
};

struct Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_StaticFields
{
public:
	// System.Int32 UnityEngine.Object::OffsetOfInstanceIDInCPlusPlusObject
	int32_t ___OffsetOfInstanceIDInCPlusPlusObject_1;

public:
	inline static int32_t get_offset_of_OffsetOfInstanceIDInCPlusPlusObject_1() { return static_cast<int32_t>(offsetof(Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_StaticFields, ___OffsetOfInstanceIDInCPlusPlusObject_1)); }
	inline int32_t get_OffsetOfInstanceIDInCPlusPlusObject_1() const { return ___OffsetOfInstanceIDInCPlusPlusObject_1; }
	inline int32_t* get_address_of_OffsetOfInstanceIDInCPlusPlusObject_1() { return &___OffsetOfInstanceIDInCPlusPlusObject_1; }
	inline void set_OffsetOfInstanceIDInCPlusPlusObject_1(int32_t value)
	{
		___OffsetOfInstanceIDInCPlusPlusObject_1 = value;
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.Object
struct Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_marshaled_pinvoke
{
	intptr_t ___m_CachedPtr_0;
};
// Native definition for COM marshalling of UnityEngine.Object
struct Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_marshaled_com
{
	intptr_t ___m_CachedPtr_0;
};

// UnityEngine.RenderTextureCreationFlags
struct  RenderTextureCreationFlags_t24A9C99A84202C1F13828D9F5693BE46CFBD61F3 
{
public:
	// System.Int32 UnityEngine.RenderTextureCreationFlags::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(RenderTextureCreationFlags_t24A9C99A84202C1F13828D9F5693BE46CFBD61F3, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.RenderTextureMemoryless
struct  RenderTextureMemoryless_t37547D68C2186D2650440F719302CDA4A3BB7F67 
{
public:
	// System.Int32 UnityEngine.RenderTextureMemoryless::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(RenderTextureMemoryless_t37547D68C2186D2650440F719302CDA4A3BB7F67, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Rendering.BuiltinRenderTextureType
struct  BuiltinRenderTextureType_t89FFB8A7C9095150BCA40E573A73664CC37F023A 
{
public:
	// System.Int32 UnityEngine.Rendering.BuiltinRenderTextureType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(BuiltinRenderTextureType_t89FFB8A7C9095150BCA40E573A73664CC37F023A, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Rendering.ShadowSamplingMode
struct  ShadowSamplingMode_t864AB52A05C1F54A738E06F76F47CDF4C26CF7F9 
{
public:
	// System.Int32 UnityEngine.Rendering.ShadowSamplingMode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(ShadowSamplingMode_t864AB52A05C1F54A738E06F76F47CDF4C26CF7F9, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Rendering.TextureDimension
struct  TextureDimension_tADCCB7C1D30E4D1182651BA9094B4DE61B63EACC 
{
public:
	// System.Int32 UnityEngine.Rendering.TextureDimension::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(TextureDimension_tADCCB7C1D30E4D1182651BA9094B4DE61B63EACC, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.VRTextureUsage
struct  VRTextureUsage_t3C09DF3DD90B5620BC0AB6F8078DFEF4E607F645 
{
public:
	// System.Int32 UnityEngine.VRTextureUsage::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(VRTextureUsage_t3C09DF3DD90B5620BC0AB6F8078DFEF4E607F645, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.XR.AvailableTrackingData
struct  AvailableTrackingData_tECF9F41E063E32F92AF43156E0C61190C82B47FC 
{
public:
	// System.Int32 UnityEngine.XR.AvailableTrackingData::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(AvailableTrackingData_tECF9F41E063E32F92AF43156E0C61190C82B47FC, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.XR.ConnectionChangeType
struct  ConnectionChangeType_tDCBB141E97849FA7B1FDA5E3BE878B51A124AD8A 
{
public:
	// System.UInt32 UnityEngine.XR.ConnectionChangeType::value__
	uint32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(ConnectionChangeType_tDCBB141E97849FA7B1FDA5E3BE878B51A124AD8A, ___value___2)); }
	inline uint32_t get_value___2() const { return ___value___2; }
	inline uint32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(uint32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.XR.InputFeatureType
struct  InputFeatureType_t3581EE01C178BF1CC9BAFE6443BEF6B0C0B2609C 
{
public:
	// System.UInt32 UnityEngine.XR.InputFeatureType::value__
	uint32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(InputFeatureType_t3581EE01C178BF1CC9BAFE6443BEF6B0C0B2609C, ___value___2)); }
	inline uint32_t get_value___2() const { return ___value___2; }
	inline uint32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(uint32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.XR.InputTracking_TrackingStateEventType
struct  TrackingStateEventType_t301E0DD44D089E06B0BBA994F682CE9F23505BA5 
{
public:
	// System.Int32 UnityEngine.XR.InputTracking_TrackingStateEventType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(TrackingStateEventType_t301E0DD44D089E06B0BBA994F682CE9F23505BA5, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.XR.MeshGenerationStatus
struct  MeshGenerationStatus_t25EB712EAD94A279AD7D5A00E0CB6EDC8AB1FE79 
{
public:
	// System.Int32 UnityEngine.XR.MeshGenerationStatus::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(MeshGenerationStatus_t25EB712EAD94A279AD7D5A00E0CB6EDC8AB1FE79, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.XR.MeshVertexAttributes
struct  MeshVertexAttributes_t7CCF6BE6BB4E908E1ECF9F9AF76968FA38A672CE 
{
public:
	// System.Int32 UnityEngine.XR.MeshVertexAttributes::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(MeshVertexAttributes_t7CCF6BE6BB4E908E1ECF9F9AF76968FA38A672CE, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.XR.XRDisplaySubsystem_XRMirrorViewBlitDesc
struct  XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5 
{
public:
	// System.IntPtr UnityEngine.XR.XRDisplaySubsystem_XRMirrorViewBlitDesc::displaySubsystemInstance
	intptr_t ___displaySubsystemInstance_0;
	// System.Boolean UnityEngine.XR.XRDisplaySubsystem_XRMirrorViewBlitDesc::nativeBlitAvailable
	bool ___nativeBlitAvailable_1;
	// System.Boolean UnityEngine.XR.XRDisplaySubsystem_XRMirrorViewBlitDesc::nativeBlitInvalidStates
	bool ___nativeBlitInvalidStates_2;
	// System.Int32 UnityEngine.XR.XRDisplaySubsystem_XRMirrorViewBlitDesc::blitParamsCount
	int32_t ___blitParamsCount_3;

public:
	inline static int32_t get_offset_of_displaySubsystemInstance_0() { return static_cast<int32_t>(offsetof(XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5, ___displaySubsystemInstance_0)); }
	inline intptr_t get_displaySubsystemInstance_0() const { return ___displaySubsystemInstance_0; }
	inline intptr_t* get_address_of_displaySubsystemInstance_0() { return &___displaySubsystemInstance_0; }
	inline void set_displaySubsystemInstance_0(intptr_t value)
	{
		___displaySubsystemInstance_0 = value;
	}

	inline static int32_t get_offset_of_nativeBlitAvailable_1() { return static_cast<int32_t>(offsetof(XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5, ___nativeBlitAvailable_1)); }
	inline bool get_nativeBlitAvailable_1() const { return ___nativeBlitAvailable_1; }
	inline bool* get_address_of_nativeBlitAvailable_1() { return &___nativeBlitAvailable_1; }
	inline void set_nativeBlitAvailable_1(bool value)
	{
		___nativeBlitAvailable_1 = value;
	}

	inline static int32_t get_offset_of_nativeBlitInvalidStates_2() { return static_cast<int32_t>(offsetof(XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5, ___nativeBlitInvalidStates_2)); }
	inline bool get_nativeBlitInvalidStates_2() const { return ___nativeBlitInvalidStates_2; }
	inline bool* get_address_of_nativeBlitInvalidStates_2() { return &___nativeBlitInvalidStates_2; }
	inline void set_nativeBlitInvalidStates_2(bool value)
	{
		___nativeBlitInvalidStates_2 = value;
	}

	inline static int32_t get_offset_of_blitParamsCount_3() { return static_cast<int32_t>(offsetof(XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5, ___blitParamsCount_3)); }
	inline int32_t get_blitParamsCount_3() const { return ___blitParamsCount_3; }
	inline int32_t* get_address_of_blitParamsCount_3() { return &___blitParamsCount_3; }
	inline void set_blitParamsCount_3(int32_t value)
	{
		___blitParamsCount_3 = value;
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.XR.XRDisplaySubsystem/XRMirrorViewBlitDesc
struct XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5_marshaled_pinvoke
{
	intptr_t ___displaySubsystemInstance_0;
	int32_t ___nativeBlitAvailable_1;
	int32_t ___nativeBlitInvalidStates_2;
	int32_t ___blitParamsCount_3;
};
// Native definition for COM marshalling of UnityEngine.XR.XRDisplaySubsystem/XRMirrorViewBlitDesc
struct XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5_marshaled_com
{
	intptr_t ___displaySubsystemInstance_0;
	int32_t ___nativeBlitAvailable_1;
	int32_t ___nativeBlitInvalidStates_2;
	int32_t ___blitParamsCount_3;
};

// UnityEngine.XR.XRNode
struct  XRNode_t07B789D60F5B3A4F0E4A169143881ABCA4176DBD 
{
public:
	// System.Int32 UnityEngine.XR.XRNode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(XRNode_t07B789D60F5B3A4F0E4A169143881ABCA4176DBD, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
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


// UnityEngine.Component
struct  Component_t62FBC8D2420DA4BE9037AFE430740F6B3EECA684  : public Object_tF2F3778131EFF286AF62B7B013A170F95A91571A
{
public:

public:
};


// UnityEngine.IntegratedSubsystemDescriptor`1<UnityEngine.XR.XRDisplaySubsystem>
struct  IntegratedSubsystemDescriptor_1_tFDF96CDD8FD2E980FF0C62E8161C66AF9FC212E2  : public IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A
{
public:

public:
};

// Native definition for P/Invoke marshalling of UnityEngine.IntegratedSubsystemDescriptor`1
#ifndef IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_pinvoke_define
#define IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_pinvoke_define
struct IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_pinvoke : public IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshaled_pinvoke
{
};
#endif
// Native definition for COM marshalling of UnityEngine.IntegratedSubsystemDescriptor`1
#ifndef IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_com_define
#define IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_com_define
struct IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_com : public IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshaled_com
{
};
#endif

// UnityEngine.IntegratedSubsystemDescriptor`1<UnityEngine.XR.XRInputSubsystem>
struct  IntegratedSubsystemDescriptor_1_t7D61E241AA40ECC23A367A5FAF509A54B1F77EF2  : public IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A
{
public:

public:
};

// Native definition for P/Invoke marshalling of UnityEngine.IntegratedSubsystemDescriptor`1
#ifndef IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_pinvoke_define
#define IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_pinvoke_define
struct IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_pinvoke : public IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshaled_pinvoke
{
};
#endif
// Native definition for COM marshalling of UnityEngine.IntegratedSubsystemDescriptor`1
#ifndef IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_com_define
#define IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_com_define
struct IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_com : public IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshaled_com
{
};
#endif

// UnityEngine.IntegratedSubsystemDescriptor`1<UnityEngine.XR.XRMeshSubsystem>
struct  IntegratedSubsystemDescriptor_1_t822E08B2CE1EC68FE74F71A682C9ECC6D52A6E89  : public IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A
{
public:

public:
};

// Native definition for P/Invoke marshalling of UnityEngine.IntegratedSubsystemDescriptor`1
#ifndef IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_pinvoke_define
#define IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_pinvoke_define
struct IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_pinvoke : public IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshaled_pinvoke
{
};
#endif
// Native definition for COM marshalling of UnityEngine.IntegratedSubsystemDescriptor`1
#ifndef IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_com_define
#define IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_com_define
struct IntegratedSubsystemDescriptor_1_t887CBD2C6B2D4D32DE18C1E1EB73CF2F1167F58B_marshaled_com : public IntegratedSubsystemDescriptor_tDC8AF8E5B67B983E4492D784A419F01693926D7A_marshaled_com
{
};
#endif

// UnityEngine.IntegratedSubsystem`1<UnityEngine.XR.XRDisplaySubsystemDescriptor>
struct  IntegratedSubsystem_1_t2737E0F52E6DC7B2E3D42D1B05C5FD7C9FDE4EA4  : public IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002
{
public:

public:
};


// UnityEngine.IntegratedSubsystem`1<UnityEngine.XR.XRInputSubsystemDescriptor>
struct  IntegratedSubsystem_1_tD5C4AF38726B9433CFC3CA0F889D8C8C2535AEFE  : public IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002
{
public:

public:
};


// UnityEngine.IntegratedSubsystem`1<UnityEngine.XR.XRMeshSubsystemDescriptor>
struct  IntegratedSubsystem_1_t902A5B61CE879B3CD855E5CE6CAEEB1B9752E840  : public IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002
{
public:

public:
};


// UnityEngine.Mesh
struct  Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6  : public Object_tF2F3778131EFF286AF62B7B013A170F95A91571A
{
public:

public:
};


// UnityEngine.RenderTextureDescriptor
struct  RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47 
{
public:
	// System.Int32 UnityEngine.RenderTextureDescriptor::<width>k__BackingField
	int32_t ___U3CwidthU3Ek__BackingField_0;
	// System.Int32 UnityEngine.RenderTextureDescriptor::<height>k__BackingField
	int32_t ___U3CheightU3Ek__BackingField_1;
	// System.Int32 UnityEngine.RenderTextureDescriptor::<msaaSamples>k__BackingField
	int32_t ___U3CmsaaSamplesU3Ek__BackingField_2;
	// System.Int32 UnityEngine.RenderTextureDescriptor::<volumeDepth>k__BackingField
	int32_t ___U3CvolumeDepthU3Ek__BackingField_3;
	// System.Int32 UnityEngine.RenderTextureDescriptor::<mipCount>k__BackingField
	int32_t ___U3CmipCountU3Ek__BackingField_4;
	// UnityEngine.Experimental.Rendering.GraphicsFormat UnityEngine.RenderTextureDescriptor::_graphicsFormat
	int32_t ____graphicsFormat_5;
	// UnityEngine.Experimental.Rendering.GraphicsFormat UnityEngine.RenderTextureDescriptor::<stencilFormat>k__BackingField
	int32_t ___U3CstencilFormatU3Ek__BackingField_6;
	// System.Int32 UnityEngine.RenderTextureDescriptor::_depthBufferBits
	int32_t ____depthBufferBits_7;
	// UnityEngine.Rendering.TextureDimension UnityEngine.RenderTextureDescriptor::<dimension>k__BackingField
	int32_t ___U3CdimensionU3Ek__BackingField_9;
	// UnityEngine.Rendering.ShadowSamplingMode UnityEngine.RenderTextureDescriptor::<shadowSamplingMode>k__BackingField
	int32_t ___U3CshadowSamplingModeU3Ek__BackingField_10;
	// UnityEngine.VRTextureUsage UnityEngine.RenderTextureDescriptor::<vrUsage>k__BackingField
	int32_t ___U3CvrUsageU3Ek__BackingField_11;
	// UnityEngine.RenderTextureCreationFlags UnityEngine.RenderTextureDescriptor::_flags
	int32_t ____flags_12;
	// UnityEngine.RenderTextureMemoryless UnityEngine.RenderTextureDescriptor::<memoryless>k__BackingField
	int32_t ___U3CmemorylessU3Ek__BackingField_13;

public:
	inline static int32_t get_offset_of_U3CwidthU3Ek__BackingField_0() { return static_cast<int32_t>(offsetof(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47, ___U3CwidthU3Ek__BackingField_0)); }
	inline int32_t get_U3CwidthU3Ek__BackingField_0() const { return ___U3CwidthU3Ek__BackingField_0; }
	inline int32_t* get_address_of_U3CwidthU3Ek__BackingField_0() { return &___U3CwidthU3Ek__BackingField_0; }
	inline void set_U3CwidthU3Ek__BackingField_0(int32_t value)
	{
		___U3CwidthU3Ek__BackingField_0 = value;
	}

	inline static int32_t get_offset_of_U3CheightU3Ek__BackingField_1() { return static_cast<int32_t>(offsetof(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47, ___U3CheightU3Ek__BackingField_1)); }
	inline int32_t get_U3CheightU3Ek__BackingField_1() const { return ___U3CheightU3Ek__BackingField_1; }
	inline int32_t* get_address_of_U3CheightU3Ek__BackingField_1() { return &___U3CheightU3Ek__BackingField_1; }
	inline void set_U3CheightU3Ek__BackingField_1(int32_t value)
	{
		___U3CheightU3Ek__BackingField_1 = value;
	}

	inline static int32_t get_offset_of_U3CmsaaSamplesU3Ek__BackingField_2() { return static_cast<int32_t>(offsetof(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47, ___U3CmsaaSamplesU3Ek__BackingField_2)); }
	inline int32_t get_U3CmsaaSamplesU3Ek__BackingField_2() const { return ___U3CmsaaSamplesU3Ek__BackingField_2; }
	inline int32_t* get_address_of_U3CmsaaSamplesU3Ek__BackingField_2() { return &___U3CmsaaSamplesU3Ek__BackingField_2; }
	inline void set_U3CmsaaSamplesU3Ek__BackingField_2(int32_t value)
	{
		___U3CmsaaSamplesU3Ek__BackingField_2 = value;
	}

	inline static int32_t get_offset_of_U3CvolumeDepthU3Ek__BackingField_3() { return static_cast<int32_t>(offsetof(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47, ___U3CvolumeDepthU3Ek__BackingField_3)); }
	inline int32_t get_U3CvolumeDepthU3Ek__BackingField_3() const { return ___U3CvolumeDepthU3Ek__BackingField_3; }
	inline int32_t* get_address_of_U3CvolumeDepthU3Ek__BackingField_3() { return &___U3CvolumeDepthU3Ek__BackingField_3; }
	inline void set_U3CvolumeDepthU3Ek__BackingField_3(int32_t value)
	{
		___U3CvolumeDepthU3Ek__BackingField_3 = value;
	}

	inline static int32_t get_offset_of_U3CmipCountU3Ek__BackingField_4() { return static_cast<int32_t>(offsetof(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47, ___U3CmipCountU3Ek__BackingField_4)); }
	inline int32_t get_U3CmipCountU3Ek__BackingField_4() const { return ___U3CmipCountU3Ek__BackingField_4; }
	inline int32_t* get_address_of_U3CmipCountU3Ek__BackingField_4() { return &___U3CmipCountU3Ek__BackingField_4; }
	inline void set_U3CmipCountU3Ek__BackingField_4(int32_t value)
	{
		___U3CmipCountU3Ek__BackingField_4 = value;
	}

	inline static int32_t get_offset_of__graphicsFormat_5() { return static_cast<int32_t>(offsetof(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47, ____graphicsFormat_5)); }
	inline int32_t get__graphicsFormat_5() const { return ____graphicsFormat_5; }
	inline int32_t* get_address_of__graphicsFormat_5() { return &____graphicsFormat_5; }
	inline void set__graphicsFormat_5(int32_t value)
	{
		____graphicsFormat_5 = value;
	}

	inline static int32_t get_offset_of_U3CstencilFormatU3Ek__BackingField_6() { return static_cast<int32_t>(offsetof(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47, ___U3CstencilFormatU3Ek__BackingField_6)); }
	inline int32_t get_U3CstencilFormatU3Ek__BackingField_6() const { return ___U3CstencilFormatU3Ek__BackingField_6; }
	inline int32_t* get_address_of_U3CstencilFormatU3Ek__BackingField_6() { return &___U3CstencilFormatU3Ek__BackingField_6; }
	inline void set_U3CstencilFormatU3Ek__BackingField_6(int32_t value)
	{
		___U3CstencilFormatU3Ek__BackingField_6 = value;
	}

	inline static int32_t get_offset_of__depthBufferBits_7() { return static_cast<int32_t>(offsetof(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47, ____depthBufferBits_7)); }
	inline int32_t get__depthBufferBits_7() const { return ____depthBufferBits_7; }
	inline int32_t* get_address_of__depthBufferBits_7() { return &____depthBufferBits_7; }
	inline void set__depthBufferBits_7(int32_t value)
	{
		____depthBufferBits_7 = value;
	}

	inline static int32_t get_offset_of_U3CdimensionU3Ek__BackingField_9() { return static_cast<int32_t>(offsetof(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47, ___U3CdimensionU3Ek__BackingField_9)); }
	inline int32_t get_U3CdimensionU3Ek__BackingField_9() const { return ___U3CdimensionU3Ek__BackingField_9; }
	inline int32_t* get_address_of_U3CdimensionU3Ek__BackingField_9() { return &___U3CdimensionU3Ek__BackingField_9; }
	inline void set_U3CdimensionU3Ek__BackingField_9(int32_t value)
	{
		___U3CdimensionU3Ek__BackingField_9 = value;
	}

	inline static int32_t get_offset_of_U3CshadowSamplingModeU3Ek__BackingField_10() { return static_cast<int32_t>(offsetof(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47, ___U3CshadowSamplingModeU3Ek__BackingField_10)); }
	inline int32_t get_U3CshadowSamplingModeU3Ek__BackingField_10() const { return ___U3CshadowSamplingModeU3Ek__BackingField_10; }
	inline int32_t* get_address_of_U3CshadowSamplingModeU3Ek__BackingField_10() { return &___U3CshadowSamplingModeU3Ek__BackingField_10; }
	inline void set_U3CshadowSamplingModeU3Ek__BackingField_10(int32_t value)
	{
		___U3CshadowSamplingModeU3Ek__BackingField_10 = value;
	}

	inline static int32_t get_offset_of_U3CvrUsageU3Ek__BackingField_11() { return static_cast<int32_t>(offsetof(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47, ___U3CvrUsageU3Ek__BackingField_11)); }
	inline int32_t get_U3CvrUsageU3Ek__BackingField_11() const { return ___U3CvrUsageU3Ek__BackingField_11; }
	inline int32_t* get_address_of_U3CvrUsageU3Ek__BackingField_11() { return &___U3CvrUsageU3Ek__BackingField_11; }
	inline void set_U3CvrUsageU3Ek__BackingField_11(int32_t value)
	{
		___U3CvrUsageU3Ek__BackingField_11 = value;
	}

	inline static int32_t get_offset_of__flags_12() { return static_cast<int32_t>(offsetof(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47, ____flags_12)); }
	inline int32_t get__flags_12() const { return ____flags_12; }
	inline int32_t* get_address_of__flags_12() { return &____flags_12; }
	inline void set__flags_12(int32_t value)
	{
		____flags_12 = value;
	}

	inline static int32_t get_offset_of_U3CmemorylessU3Ek__BackingField_13() { return static_cast<int32_t>(offsetof(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47, ___U3CmemorylessU3Ek__BackingField_13)); }
	inline int32_t get_U3CmemorylessU3Ek__BackingField_13() const { return ___U3CmemorylessU3Ek__BackingField_13; }
	inline int32_t* get_address_of_U3CmemorylessU3Ek__BackingField_13() { return &___U3CmemorylessU3Ek__BackingField_13; }
	inline void set_U3CmemorylessU3Ek__BackingField_13(int32_t value)
	{
		___U3CmemorylessU3Ek__BackingField_13 = value;
	}
};

struct RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47_StaticFields
{
public:
	// System.Int32[] UnityEngine.RenderTextureDescriptor::depthFormatBits
	Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* ___depthFormatBits_8;

public:
	inline static int32_t get_offset_of_depthFormatBits_8() { return static_cast<int32_t>(offsetof(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47_StaticFields, ___depthFormatBits_8)); }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* get_depthFormatBits_8() const { return ___depthFormatBits_8; }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32** get_address_of_depthFormatBits_8() { return &___depthFormatBits_8; }
	inline void set_depthFormatBits_8(Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* value)
	{
		___depthFormatBits_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___depthFormatBits_8), (void*)value);
	}
};


// UnityEngine.Rendering.RenderTargetIdentifier
struct  RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13 
{
public:
	// UnityEngine.Rendering.BuiltinRenderTextureType UnityEngine.Rendering.RenderTargetIdentifier::m_Type
	int32_t ___m_Type_0;
	// System.Int32 UnityEngine.Rendering.RenderTargetIdentifier::m_NameID
	int32_t ___m_NameID_1;
	// System.Int32 UnityEngine.Rendering.RenderTargetIdentifier::m_InstanceID
	int32_t ___m_InstanceID_2;
	// System.IntPtr UnityEngine.Rendering.RenderTargetIdentifier::m_BufferPointer
	intptr_t ___m_BufferPointer_3;
	// System.Int32 UnityEngine.Rendering.RenderTargetIdentifier::m_MipLevel
	int32_t ___m_MipLevel_4;
	// UnityEngine.CubemapFace UnityEngine.Rendering.RenderTargetIdentifier::m_CubeFace
	int32_t ___m_CubeFace_5;
	// System.Int32 UnityEngine.Rendering.RenderTargetIdentifier::m_DepthSlice
	int32_t ___m_DepthSlice_6;

public:
	inline static int32_t get_offset_of_m_Type_0() { return static_cast<int32_t>(offsetof(RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13, ___m_Type_0)); }
	inline int32_t get_m_Type_0() const { return ___m_Type_0; }
	inline int32_t* get_address_of_m_Type_0() { return &___m_Type_0; }
	inline void set_m_Type_0(int32_t value)
	{
		___m_Type_0 = value;
	}

	inline static int32_t get_offset_of_m_NameID_1() { return static_cast<int32_t>(offsetof(RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13, ___m_NameID_1)); }
	inline int32_t get_m_NameID_1() const { return ___m_NameID_1; }
	inline int32_t* get_address_of_m_NameID_1() { return &___m_NameID_1; }
	inline void set_m_NameID_1(int32_t value)
	{
		___m_NameID_1 = value;
	}

	inline static int32_t get_offset_of_m_InstanceID_2() { return static_cast<int32_t>(offsetof(RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13, ___m_InstanceID_2)); }
	inline int32_t get_m_InstanceID_2() const { return ___m_InstanceID_2; }
	inline int32_t* get_address_of_m_InstanceID_2() { return &___m_InstanceID_2; }
	inline void set_m_InstanceID_2(int32_t value)
	{
		___m_InstanceID_2 = value;
	}

	inline static int32_t get_offset_of_m_BufferPointer_3() { return static_cast<int32_t>(offsetof(RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13, ___m_BufferPointer_3)); }
	inline intptr_t get_m_BufferPointer_3() const { return ___m_BufferPointer_3; }
	inline intptr_t* get_address_of_m_BufferPointer_3() { return &___m_BufferPointer_3; }
	inline void set_m_BufferPointer_3(intptr_t value)
	{
		___m_BufferPointer_3 = value;
	}

	inline static int32_t get_offset_of_m_MipLevel_4() { return static_cast<int32_t>(offsetof(RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13, ___m_MipLevel_4)); }
	inline int32_t get_m_MipLevel_4() const { return ___m_MipLevel_4; }
	inline int32_t* get_address_of_m_MipLevel_4() { return &___m_MipLevel_4; }
	inline void set_m_MipLevel_4(int32_t value)
	{
		___m_MipLevel_4 = value;
	}

	inline static int32_t get_offset_of_m_CubeFace_5() { return static_cast<int32_t>(offsetof(RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13, ___m_CubeFace_5)); }
	inline int32_t get_m_CubeFace_5() const { return ___m_CubeFace_5; }
	inline int32_t* get_address_of_m_CubeFace_5() { return &___m_CubeFace_5; }
	inline void set_m_CubeFace_5(int32_t value)
	{
		___m_CubeFace_5 = value;
	}

	inline static int32_t get_offset_of_m_DepthSlice_6() { return static_cast<int32_t>(offsetof(RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13, ___m_DepthSlice_6)); }
	inline int32_t get_m_DepthSlice_6() const { return ___m_DepthSlice_6; }
	inline int32_t* get_address_of_m_DepthSlice_6() { return &___m_DepthSlice_6; }
	inline void set_m_DepthSlice_6(int32_t value)
	{
		___m_DepthSlice_6 = value;
	}
};


// UnityEngine.XR.InputFeatureUsage
struct  InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE 
{
public:
	// System.String UnityEngine.XR.InputFeatureUsage::m_Name
	String_t* ___m_Name_0;
	// UnityEngine.XR.InputFeatureType UnityEngine.XR.InputFeatureUsage::m_InternalType
	uint32_t ___m_InternalType_1;

public:
	inline static int32_t get_offset_of_m_Name_0() { return static_cast<int32_t>(offsetof(InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE, ___m_Name_0)); }
	inline String_t* get_m_Name_0() const { return ___m_Name_0; }
	inline String_t** get_address_of_m_Name_0() { return &___m_Name_0; }
	inline void set_m_Name_0(String_t* value)
	{
		___m_Name_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Name_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_InternalType_1() { return static_cast<int32_t>(offsetof(InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE, ___m_InternalType_1)); }
	inline uint32_t get_m_InternalType_1() const { return ___m_InternalType_1; }
	inline uint32_t* get_address_of_m_InternalType_1() { return &___m_InternalType_1; }
	inline void set_m_InternalType_1(uint32_t value)
	{
		___m_InternalType_1 = value;
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.XR.InputFeatureUsage
struct InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_marshaled_pinvoke
{
	char* ___m_Name_0;
	uint32_t ___m_InternalType_1;
};
// Native definition for COM marshalling of UnityEngine.XR.InputFeatureUsage
struct InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_marshaled_com
{
	Il2CppChar* ___m_Name_0;
	uint32_t ___m_InternalType_1;
};

// UnityEngine.XR.MeshGenerationResult
struct  MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF 
{
public:
	// UnityEngine.XR.MeshId UnityEngine.XR.MeshGenerationResult::<MeshId>k__BackingField
	MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  ___U3CMeshIdU3Ek__BackingField_0;
	// UnityEngine.Mesh UnityEngine.XR.MeshGenerationResult::<Mesh>k__BackingField
	Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 * ___U3CMeshU3Ek__BackingField_1;
	// UnityEngine.MeshCollider UnityEngine.XR.MeshGenerationResult::<MeshCollider>k__BackingField
	MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 * ___U3CMeshColliderU3Ek__BackingField_2;
	// UnityEngine.XR.MeshGenerationStatus UnityEngine.XR.MeshGenerationResult::<Status>k__BackingField
	int32_t ___U3CStatusU3Ek__BackingField_3;
	// UnityEngine.XR.MeshVertexAttributes UnityEngine.XR.MeshGenerationResult::<Attributes>k__BackingField
	int32_t ___U3CAttributesU3Ek__BackingField_4;

public:
	inline static int32_t get_offset_of_U3CMeshIdU3Ek__BackingField_0() { return static_cast<int32_t>(offsetof(MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF, ___U3CMeshIdU3Ek__BackingField_0)); }
	inline MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  get_U3CMeshIdU3Ek__BackingField_0() const { return ___U3CMeshIdU3Ek__BackingField_0; }
	inline MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 * get_address_of_U3CMeshIdU3Ek__BackingField_0() { return &___U3CMeshIdU3Ek__BackingField_0; }
	inline void set_U3CMeshIdU3Ek__BackingField_0(MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  value)
	{
		___U3CMeshIdU3Ek__BackingField_0 = value;
	}

	inline static int32_t get_offset_of_U3CMeshU3Ek__BackingField_1() { return static_cast<int32_t>(offsetof(MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF, ___U3CMeshU3Ek__BackingField_1)); }
	inline Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 * get_U3CMeshU3Ek__BackingField_1() const { return ___U3CMeshU3Ek__BackingField_1; }
	inline Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 ** get_address_of_U3CMeshU3Ek__BackingField_1() { return &___U3CMeshU3Ek__BackingField_1; }
	inline void set_U3CMeshU3Ek__BackingField_1(Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 * value)
	{
		___U3CMeshU3Ek__BackingField_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CMeshU3Ek__BackingField_1), (void*)value);
	}

	inline static int32_t get_offset_of_U3CMeshColliderU3Ek__BackingField_2() { return static_cast<int32_t>(offsetof(MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF, ___U3CMeshColliderU3Ek__BackingField_2)); }
	inline MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 * get_U3CMeshColliderU3Ek__BackingField_2() const { return ___U3CMeshColliderU3Ek__BackingField_2; }
	inline MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 ** get_address_of_U3CMeshColliderU3Ek__BackingField_2() { return &___U3CMeshColliderU3Ek__BackingField_2; }
	inline void set_U3CMeshColliderU3Ek__BackingField_2(MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 * value)
	{
		___U3CMeshColliderU3Ek__BackingField_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CMeshColliderU3Ek__BackingField_2), (void*)value);
	}

	inline static int32_t get_offset_of_U3CStatusU3Ek__BackingField_3() { return static_cast<int32_t>(offsetof(MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF, ___U3CStatusU3Ek__BackingField_3)); }
	inline int32_t get_U3CStatusU3Ek__BackingField_3() const { return ___U3CStatusU3Ek__BackingField_3; }
	inline int32_t* get_address_of_U3CStatusU3Ek__BackingField_3() { return &___U3CStatusU3Ek__BackingField_3; }
	inline void set_U3CStatusU3Ek__BackingField_3(int32_t value)
	{
		___U3CStatusU3Ek__BackingField_3 = value;
	}

	inline static int32_t get_offset_of_U3CAttributesU3Ek__BackingField_4() { return static_cast<int32_t>(offsetof(MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF, ___U3CAttributesU3Ek__BackingField_4)); }
	inline int32_t get_U3CAttributesU3Ek__BackingField_4() const { return ___U3CAttributesU3Ek__BackingField_4; }
	inline int32_t* get_address_of_U3CAttributesU3Ek__BackingField_4() { return &___U3CAttributesU3Ek__BackingField_4; }
	inline void set_U3CAttributesU3Ek__BackingField_4(int32_t value)
	{
		___U3CAttributesU3Ek__BackingField_4 = value;
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.XR.MeshGenerationResult
struct MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_marshaled_pinvoke
{
	MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  ___U3CMeshIdU3Ek__BackingField_0;
	Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 * ___U3CMeshU3Ek__BackingField_1;
	MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 * ___U3CMeshColliderU3Ek__BackingField_2;
	int32_t ___U3CStatusU3Ek__BackingField_3;
	int32_t ___U3CAttributesU3Ek__BackingField_4;
};
// Native definition for COM marshalling of UnityEngine.XR.MeshGenerationResult
struct MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_marshaled_com
{
	MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  ___U3CMeshIdU3Ek__BackingField_0;
	Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 * ___U3CMeshU3Ek__BackingField_1;
	MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 * ___U3CMeshColliderU3Ek__BackingField_2;
	int32_t ___U3CStatusU3Ek__BackingField_3;
	int32_t ___U3CAttributesU3Ek__BackingField_4;
};

// UnityEngine.XR.XRNodeState
struct  XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 
{
public:
	// UnityEngine.XR.XRNode UnityEngine.XR.XRNodeState::m_Type
	int32_t ___m_Type_0;
	// UnityEngine.XR.AvailableTrackingData UnityEngine.XR.XRNodeState::m_AvailableFields
	int32_t ___m_AvailableFields_1;
	// UnityEngine.Vector3 UnityEngine.XR.XRNodeState::m_Position
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___m_Position_2;
	// UnityEngine.Quaternion UnityEngine.XR.XRNodeState::m_Rotation
	Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4  ___m_Rotation_3;
	// UnityEngine.Vector3 UnityEngine.XR.XRNodeState::m_Velocity
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___m_Velocity_4;
	// UnityEngine.Vector3 UnityEngine.XR.XRNodeState::m_AngularVelocity
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___m_AngularVelocity_5;
	// UnityEngine.Vector3 UnityEngine.XR.XRNodeState::m_Acceleration
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___m_Acceleration_6;
	// UnityEngine.Vector3 UnityEngine.XR.XRNodeState::m_AngularAcceleration
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___m_AngularAcceleration_7;
	// System.Int32 UnityEngine.XR.XRNodeState::m_Tracked
	int32_t ___m_Tracked_8;
	// System.UInt64 UnityEngine.XR.XRNodeState::m_UniqueID
	uint64_t ___m_UniqueID_9;

public:
	inline static int32_t get_offset_of_m_Type_0() { return static_cast<int32_t>(offsetof(XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33, ___m_Type_0)); }
	inline int32_t get_m_Type_0() const { return ___m_Type_0; }
	inline int32_t* get_address_of_m_Type_0() { return &___m_Type_0; }
	inline void set_m_Type_0(int32_t value)
	{
		___m_Type_0 = value;
	}

	inline static int32_t get_offset_of_m_AvailableFields_1() { return static_cast<int32_t>(offsetof(XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33, ___m_AvailableFields_1)); }
	inline int32_t get_m_AvailableFields_1() const { return ___m_AvailableFields_1; }
	inline int32_t* get_address_of_m_AvailableFields_1() { return &___m_AvailableFields_1; }
	inline void set_m_AvailableFields_1(int32_t value)
	{
		___m_AvailableFields_1 = value;
	}

	inline static int32_t get_offset_of_m_Position_2() { return static_cast<int32_t>(offsetof(XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33, ___m_Position_2)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_m_Position_2() const { return ___m_Position_2; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_m_Position_2() { return &___m_Position_2; }
	inline void set_m_Position_2(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___m_Position_2 = value;
	}

	inline static int32_t get_offset_of_m_Rotation_3() { return static_cast<int32_t>(offsetof(XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33, ___m_Rotation_3)); }
	inline Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4  get_m_Rotation_3() const { return ___m_Rotation_3; }
	inline Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4 * get_address_of_m_Rotation_3() { return &___m_Rotation_3; }
	inline void set_m_Rotation_3(Quaternion_t6D28618CF65156D4A0AD747370DDFD0C514A31B4  value)
	{
		___m_Rotation_3 = value;
	}

	inline static int32_t get_offset_of_m_Velocity_4() { return static_cast<int32_t>(offsetof(XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33, ___m_Velocity_4)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_m_Velocity_4() const { return ___m_Velocity_4; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_m_Velocity_4() { return &___m_Velocity_4; }
	inline void set_m_Velocity_4(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___m_Velocity_4 = value;
	}

	inline static int32_t get_offset_of_m_AngularVelocity_5() { return static_cast<int32_t>(offsetof(XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33, ___m_AngularVelocity_5)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_m_AngularVelocity_5() const { return ___m_AngularVelocity_5; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_m_AngularVelocity_5() { return &___m_AngularVelocity_5; }
	inline void set_m_AngularVelocity_5(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___m_AngularVelocity_5 = value;
	}

	inline static int32_t get_offset_of_m_Acceleration_6() { return static_cast<int32_t>(offsetof(XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33, ___m_Acceleration_6)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_m_Acceleration_6() const { return ___m_Acceleration_6; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_m_Acceleration_6() { return &___m_Acceleration_6; }
	inline void set_m_Acceleration_6(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___m_Acceleration_6 = value;
	}

	inline static int32_t get_offset_of_m_AngularAcceleration_7() { return static_cast<int32_t>(offsetof(XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33, ___m_AngularAcceleration_7)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_m_AngularAcceleration_7() const { return ___m_AngularAcceleration_7; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_m_AngularAcceleration_7() { return &___m_AngularAcceleration_7; }
	inline void set_m_AngularAcceleration_7(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___m_AngularAcceleration_7 = value;
	}

	inline static int32_t get_offset_of_m_Tracked_8() { return static_cast<int32_t>(offsetof(XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33, ___m_Tracked_8)); }
	inline int32_t get_m_Tracked_8() const { return ___m_Tracked_8; }
	inline int32_t* get_address_of_m_Tracked_8() { return &___m_Tracked_8; }
	inline void set_m_Tracked_8(int32_t value)
	{
		___m_Tracked_8 = value;
	}

	inline static int32_t get_offset_of_m_UniqueID_9() { return static_cast<int32_t>(offsetof(XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33, ___m_UniqueID_9)); }
	inline uint64_t get_m_UniqueID_9() const { return ___m_UniqueID_9; }
	inline uint64_t* get_address_of_m_UniqueID_9() { return &___m_UniqueID_9; }
	inline void set_m_UniqueID_9(uint64_t value)
	{
		___m_UniqueID_9 = value;
	}
};


// System.Action`1<System.Boolean>
struct  Action_1_tCE2D770918A65CAD277C08C4E8C05385EA267E83  : public MulticastDelegate_t
{
public:

public:
};


// System.Action`1<UnityEngine.XR.InputDevice>
struct  Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8  : public MulticastDelegate_t
{
public:

public:
};


// System.Action`1<UnityEngine.XR.MeshGenerationResult>
struct  Action_1_tB125CDA27D619FDBF92F767804A14CF83EA85A3C  : public MulticastDelegate_t
{
public:

public:
};


// System.Action`1<UnityEngine.XR.XRInputSubsystem>
struct  Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1  : public MulticastDelegate_t
{
public:

public:
};


// System.Action`1<UnityEngine.XR.XRNodeState>
struct  Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603  : public MulticastDelegate_t
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


// UnityEngine.Collider
struct  Collider_t5E81E43C2ECA0209A7C4528E84A632712D192B02  : public Component_t62FBC8D2420DA4BE9037AFE430740F6B3EECA684
{
public:

public:
};


// UnityEngine.XR.XRDisplaySubsystem
struct  XRDisplaySubsystem_tF8B46605B6D1199C52306D4EC7D83CFA90564A93  : public IntegratedSubsystem_1_t2737E0F52E6DC7B2E3D42D1B05C5FD7C9FDE4EA4
{
public:
	// System.Action`1<System.Boolean> UnityEngine.XR.XRDisplaySubsystem::displayFocusChanged
	Action_1_tCE2D770918A65CAD277C08C4E8C05385EA267E83 * ___displayFocusChanged_2;

public:
	inline static int32_t get_offset_of_displayFocusChanged_2() { return static_cast<int32_t>(offsetof(XRDisplaySubsystem_tF8B46605B6D1199C52306D4EC7D83CFA90564A93, ___displayFocusChanged_2)); }
	inline Action_1_tCE2D770918A65CAD277C08C4E8C05385EA267E83 * get_displayFocusChanged_2() const { return ___displayFocusChanged_2; }
	inline Action_1_tCE2D770918A65CAD277C08C4E8C05385EA267E83 ** get_address_of_displayFocusChanged_2() { return &___displayFocusChanged_2; }
	inline void set_displayFocusChanged_2(Action_1_tCE2D770918A65CAD277C08C4E8C05385EA267E83 * value)
	{
		___displayFocusChanged_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___displayFocusChanged_2), (void*)value);
	}
};


// UnityEngine.XR.XRDisplaySubsystem_XRRenderPass
struct  XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB 
{
public:
	// System.IntPtr UnityEngine.XR.XRDisplaySubsystem_XRRenderPass::displaySubsystemInstance
	intptr_t ___displaySubsystemInstance_0;
	// System.Int32 UnityEngine.XR.XRDisplaySubsystem_XRRenderPass::renderPassIndex
	int32_t ___renderPassIndex_1;
	// UnityEngine.Rendering.RenderTargetIdentifier UnityEngine.XR.XRDisplaySubsystem_XRRenderPass::renderTarget
	RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13  ___renderTarget_2;
	// UnityEngine.RenderTextureDescriptor UnityEngine.XR.XRDisplaySubsystem_XRRenderPass::renderTargetDesc
	RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47  ___renderTargetDesc_3;
	// System.Boolean UnityEngine.XR.XRDisplaySubsystem_XRRenderPass::shouldFillOutDepth
	bool ___shouldFillOutDepth_4;
	// System.Int32 UnityEngine.XR.XRDisplaySubsystem_XRRenderPass::cullingPassIndex
	int32_t ___cullingPassIndex_5;

public:
	inline static int32_t get_offset_of_displaySubsystemInstance_0() { return static_cast<int32_t>(offsetof(XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB, ___displaySubsystemInstance_0)); }
	inline intptr_t get_displaySubsystemInstance_0() const { return ___displaySubsystemInstance_0; }
	inline intptr_t* get_address_of_displaySubsystemInstance_0() { return &___displaySubsystemInstance_0; }
	inline void set_displaySubsystemInstance_0(intptr_t value)
	{
		___displaySubsystemInstance_0 = value;
	}

	inline static int32_t get_offset_of_renderPassIndex_1() { return static_cast<int32_t>(offsetof(XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB, ___renderPassIndex_1)); }
	inline int32_t get_renderPassIndex_1() const { return ___renderPassIndex_1; }
	inline int32_t* get_address_of_renderPassIndex_1() { return &___renderPassIndex_1; }
	inline void set_renderPassIndex_1(int32_t value)
	{
		___renderPassIndex_1 = value;
	}

	inline static int32_t get_offset_of_renderTarget_2() { return static_cast<int32_t>(offsetof(XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB, ___renderTarget_2)); }
	inline RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13  get_renderTarget_2() const { return ___renderTarget_2; }
	inline RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13 * get_address_of_renderTarget_2() { return &___renderTarget_2; }
	inline void set_renderTarget_2(RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13  value)
	{
		___renderTarget_2 = value;
	}

	inline static int32_t get_offset_of_renderTargetDesc_3() { return static_cast<int32_t>(offsetof(XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB, ___renderTargetDesc_3)); }
	inline RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47  get_renderTargetDesc_3() const { return ___renderTargetDesc_3; }
	inline RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47 * get_address_of_renderTargetDesc_3() { return &___renderTargetDesc_3; }
	inline void set_renderTargetDesc_3(RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47  value)
	{
		___renderTargetDesc_3 = value;
	}

	inline static int32_t get_offset_of_shouldFillOutDepth_4() { return static_cast<int32_t>(offsetof(XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB, ___shouldFillOutDepth_4)); }
	inline bool get_shouldFillOutDepth_4() const { return ___shouldFillOutDepth_4; }
	inline bool* get_address_of_shouldFillOutDepth_4() { return &___shouldFillOutDepth_4; }
	inline void set_shouldFillOutDepth_4(bool value)
	{
		___shouldFillOutDepth_4 = value;
	}

	inline static int32_t get_offset_of_cullingPassIndex_5() { return static_cast<int32_t>(offsetof(XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB, ___cullingPassIndex_5)); }
	inline int32_t get_cullingPassIndex_5() const { return ___cullingPassIndex_5; }
	inline int32_t* get_address_of_cullingPassIndex_5() { return &___cullingPassIndex_5; }
	inline void set_cullingPassIndex_5(int32_t value)
	{
		___cullingPassIndex_5 = value;
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.XR.XRDisplaySubsystem/XRRenderPass
struct XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB_marshaled_pinvoke
{
	intptr_t ___displaySubsystemInstance_0;
	int32_t ___renderPassIndex_1;
	RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13  ___renderTarget_2;
	RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47  ___renderTargetDesc_3;
	int32_t ___shouldFillOutDepth_4;
	int32_t ___cullingPassIndex_5;
};
// Native definition for COM marshalling of UnityEngine.XR.XRDisplaySubsystem/XRRenderPass
struct XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB_marshaled_com
{
	intptr_t ___displaySubsystemInstance_0;
	int32_t ___renderPassIndex_1;
	RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13  ___renderTarget_2;
	RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47  ___renderTargetDesc_3;
	int32_t ___shouldFillOutDepth_4;
	int32_t ___cullingPassIndex_5;
};

// UnityEngine.XR.XRDisplaySubsystemDescriptor
struct  XRDisplaySubsystemDescriptor_tBBE6956FF61EACF13E72BFEF58ADC5930C760833  : public IntegratedSubsystemDescriptor_1_tFDF96CDD8FD2E980FF0C62E8161C66AF9FC212E2
{
public:

public:
};


// UnityEngine.XR.XRInputSubsystem
struct  XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09  : public IntegratedSubsystem_1_tD5C4AF38726B9433CFC3CA0F889D8C8C2535AEFE
{
public:
	// System.Action`1<UnityEngine.XR.XRInputSubsystem> UnityEngine.XR.XRInputSubsystem::trackingOriginUpdated
	Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 * ___trackingOriginUpdated_2;
	// System.Action`1<UnityEngine.XR.XRInputSubsystem> UnityEngine.XR.XRInputSubsystem::boundaryChanged
	Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 * ___boundaryChanged_3;

public:
	inline static int32_t get_offset_of_trackingOriginUpdated_2() { return static_cast<int32_t>(offsetof(XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09, ___trackingOriginUpdated_2)); }
	inline Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 * get_trackingOriginUpdated_2() const { return ___trackingOriginUpdated_2; }
	inline Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 ** get_address_of_trackingOriginUpdated_2() { return &___trackingOriginUpdated_2; }
	inline void set_trackingOriginUpdated_2(Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 * value)
	{
		___trackingOriginUpdated_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___trackingOriginUpdated_2), (void*)value);
	}

	inline static int32_t get_offset_of_boundaryChanged_3() { return static_cast<int32_t>(offsetof(XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09, ___boundaryChanged_3)); }
	inline Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 * get_boundaryChanged_3() const { return ___boundaryChanged_3; }
	inline Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 ** get_address_of_boundaryChanged_3() { return &___boundaryChanged_3; }
	inline void set_boundaryChanged_3(Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 * value)
	{
		___boundaryChanged_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___boundaryChanged_3), (void*)value);
	}
};


// UnityEngine.XR.XRInputSubsystemDescriptor
struct  XRInputSubsystemDescriptor_t98C4233948EC9169B71D2A58C2C6ED1AF6FDABC2  : public IntegratedSubsystemDescriptor_1_t7D61E241AA40ECC23A367A5FAF509A54B1F77EF2
{
public:

public:
};


// UnityEngine.XR.XRMeshSubsystem
struct  XRMeshSubsystem_t60BD977DF1B014CF5D48C8EBCC91DED767520D63  : public IntegratedSubsystem_1_t902A5B61CE879B3CD855E5CE6CAEEB1B9752E840
{
public:

public:
};


// UnityEngine.XR.XRMeshSubsystemDescriptor
struct  XRMeshSubsystemDescriptor_t428853FE3628F349D46DFD6841B50058F09F5FCC  : public IntegratedSubsystemDescriptor_1_t822E08B2CE1EC68FE74F71A682C9ECC6D52A6E89
{
public:

public:
};


// UnityEngine.MeshCollider
struct  MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98  : public Collider_t5E81E43C2ECA0209A7C4528E84A632712D192B02
{
public:

public:
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif


// System.Void System.Action`1<UnityEngine.XR.InputDevice>::Invoke(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Action_1_Invoke_mA460B359B13BDCF71CDDE9A8A960C827A4E3E970_gshared (Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * __this, InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E  ___obj0, const RuntimeMethod* method);
// System.Void System.Action`1<UnityEngine.XR.XRNodeState>::Invoke(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Action_1_Invoke_mD7440CB91FE64B4EAD0D34248075E0F39797C946_gshared (Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * __this, XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33  ___obj0, const RuntimeMethod* method);
// System.Void System.Action`1<System.Boolean>::Invoke(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Action_1_Invoke_m95E5C9FC67F7B0BDC3CD5E00AC5D4C8A00C97AC5_gshared (Action_1_tCE2D770918A65CAD277C08C4E8C05385EA267E83 * __this, bool ___obj0, const RuntimeMethod* method);
// System.Void UnityEngine.IntegratedSubsystem`1<System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IntegratedSubsystem_1__ctor_m429238094F2A54BAF2F6F89270D5D43241541E72_gshared (IntegratedSubsystem_1_t0B19871ED45EAD9F0E0DD6AB41BABCAFBD8C56E4 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.IntegratedSubsystemDescriptor`1<System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IntegratedSubsystemDescriptor_1__ctor_m74AFBEADF13921EE528F5629B38B89C8C51EB2DC_gshared (IntegratedSubsystemDescriptor_1_t4BFDAEC6A4D96827E7D4D0B2E85EB1AFA1911939 * __this, const RuntimeMethod* method);
// System.Void System.Action`1<System.Object>::Invoke(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Action_1_Invoke_m587509C88BB83721D7918D89DF07606BB752D744_gshared (Action_1_tD9663D9715FAA4E62035CFCF1AD4D094EE7872DC * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Void System.Action`1<UnityEngine.XR.MeshGenerationResult>::Invoke(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Action_1_Invoke_mC3DCAEAD9DC81FE145B4FE115F830C0767728604_gshared (Action_1_tB125CDA27D619FDBF92F767804A14CF83EA85A3C * __this, MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF  ___obj0, const RuntimeMethod* method);

// System.UInt64 UnityEngine.XR.Bone::get_deviceId()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint64_t Bone_get_deviceId_mF1EBAD176E69C1074F55BFFF36372A48840B2FA2 (Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * __this, const RuntimeMethod* method);
// System.UInt32 UnityEngine.XR.Bone::get_featureIndex()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t Bone_get_featureIndex_m291227F499665BECC9D40723157367ECB56781C3 (Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.XR.Bone::Equals(UnityEngine.XR.Bone)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Bone_Equals_m025222EEBF2374226C62C32E1A7ADE3538C13AFB (Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * __this, Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070  ___other0, const RuntimeMethod* method);
// System.Boolean UnityEngine.XR.Bone::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Bone_Equals_m2FBDCFEA8B90663E546294EBEB4763538DEEA412 (Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Int32 System.UInt64::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t UInt64_GetHashCode_mCDF662897A3F02CED11A9F9E66C5BF4E28C02B33 (uint64_t* __this, const RuntimeMethod* method);
// System.Int32 System.UInt32::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t UInt32_GetHashCode_m60E3A243F3D79311A64836148AE1AC23C679FC45 (uint32_t* __this, const RuntimeMethod* method);
// System.Int32 UnityEngine.XR.Bone::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Bone_GetHashCode_m0ED7925C274DE8439749951FBE4E4EC28A797BFB (Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * __this, const RuntimeMethod* method);
// System.UInt64 UnityEngine.XR.Eyes::get_deviceId()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint64_t Eyes_get_deviceId_m5587223FE834DE2ABDC32F9E8FE6D768D4DDF936 (Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * __this, const RuntimeMethod* method);
// System.UInt32 UnityEngine.XR.Eyes::get_featureIndex()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t Eyes_get_featureIndex_mBABAD5CFCF0585EE4791C0E1C3E2562627491237 (Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.XR.Eyes::Equals(UnityEngine.XR.Eyes)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Eyes_Equals_m60083B041FECD045D717F2873BF2E21AA9A4FD01 (Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * __this, Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D  ___other0, const RuntimeMethod* method);
// System.Boolean UnityEngine.XR.Eyes::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Eyes_Equals_m58897DB2EEC48809233B94BB4CEA166B8ACEBFF2 (Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Int32 UnityEngine.XR.Eyes::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Eyes_GetHashCode_mA9742A7C19500C6C17EDCD5EED7954A9FE91C0DD (Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * __this, const RuntimeMethod* method);
// System.UInt64 UnityEngine.XR.Hand::get_deviceId()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint64_t Hand_get_deviceId_mD45907CDFDBF2CE754444358020196A8C6BEE3D1 (Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * __this, const RuntimeMethod* method);
// System.UInt32 UnityEngine.XR.Hand::get_featureIndex()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t Hand_get_featureIndex_mB8A458180449B2C2D38D5405BA26577CF1A60897 (Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.XR.Hand::Equals(UnityEngine.XR.Hand)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Hand_Equals_m099C7921633095BB40C830B6F5558F99ED0D7BD0 (Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * __this, Hand_tB64007EC8D01384426C93432737BA9C5F636A690  ___other0, const RuntimeMethod* method);
// System.Boolean UnityEngine.XR.Hand::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Hand_Equals_m727113281F30E554A3A60DEFC4ED61CC94901775 (Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Int32 UnityEngine.XR.Hand::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Hand_GetHashCode_mFC8C4732F0B728122C3ABCC623699DA7E47D1CCB (Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.XR.InputDevice::.ctor(System.UInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void InputDevice__ctor_m610666CA01BCAF92464BE9C17BFB133A23A663BE (InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * __this, uint64_t ___deviceId0, const RuntimeMethod* method);
// System.UInt64 UnityEngine.XR.InputDevice::get_deviceId()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint64_t InputDevice_get_deviceId_m86C962E24F26D879240A6E889E9E8D3D94F546A5 (InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.XR.InputDevice::Equals(UnityEngine.XR.InputDevice)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool InputDevice_Equals_m9661F95B50387146BC3C6F5DC63FDF2B1303ABB7 (InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * __this, InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E  ___other0, const RuntimeMethod* method);
// System.Boolean UnityEngine.XR.InputDevice::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool InputDevice_Equals_mF29A225E81A87941551F70A2351CB803A6D94063 (InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Int32 UnityEngine.XR.InputDevice::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t InputDevice_GetHashCode_mFD9C9A6015B91E254DD53E14651C8D08F715D7F8 (InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * __this, const RuntimeMethod* method);
// System.Void System.Action`1<UnityEngine.XR.InputDevice>::Invoke(!0)
inline void Action_1_Invoke_mA460B359B13BDCF71CDDE9A8A960C827A4E3E970 (Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * __this, InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E  ___obj0, const RuntimeMethod* method)
{
	((  void (*) (Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 *, InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E , const RuntimeMethod*))Action_1_Invoke_mA460B359B13BDCF71CDDE9A8A960C827A4E3E970_gshared)(__this, ___obj0, method);
}
// System.String UnityEngine.XR.InputFeatureUsage::get_name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* InputFeatureUsage_get_name_m08FB0101027B503D080EE38F4273BB61C59E3988 (InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * __this, const RuntimeMethod* method);
// UnityEngine.XR.InputFeatureType UnityEngine.XR.InputFeatureUsage::get_internalType()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t InputFeatureUsage_get_internalType_m55D87EBFF5CA6781AA831A3FAAA75BD80A13B065 (InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.XR.InputFeatureUsage::Equals(UnityEngine.XR.InputFeatureUsage)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool InputFeatureUsage_Equals_mD7107D9A754188766EACB7FAAF015E184FE706B9 (InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * __this, InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE  ___other0, const RuntimeMethod* method);
// System.Boolean UnityEngine.XR.InputFeatureUsage::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool InputFeatureUsage_Equals_mC0A1A665A98F42B2D5896BB9BC4CBA42FB59D582 (InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Boolean System.String::op_Equality(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool String_op_Equality_m2B91EE68355F142F67095973D32EB5828B7B73CB (String_t* ___a0, String_t* ___b1, const RuntimeMethod* method);
// System.Int32 UnityEngine.XR.InputFeatureUsage::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t InputFeatureUsage_GetHashCode_m08673D24DA7804D87915443A647AA65447C511D1 (InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * __this, const RuntimeMethod* method);
// System.Void UnityEngine.XR.XRNodeState::set_uniqueID(System.UInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRNodeState_set_uniqueID_m09D6E80AD1B81D7485242CFA27A7173A6231CA87 (XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 * __this, uint64_t ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.XR.XRNodeState::set_nodeType(UnityEngine.XR.XRNode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRNodeState_set_nodeType_m9ADECAA5D665042FCA2F8E81726AFD1A1FA8B30D (XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 * __this, int32_t ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.XR.XRNodeState::set_tracked(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRNodeState_set_tracked_m27DDD443D10F5F43B5B9AA83BFE901DC12316B9C (XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 * __this, bool ___value0, const RuntimeMethod* method);
// System.String System.String::Concat(System.Object,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_m4D0DDA7FEDB75304E5FDAF8489A0478EE58A45F2 (RuntimeObject * ___arg00, RuntimeObject * ___arg11, const RuntimeMethod* method);
// System.Void System.ArgumentException::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ArgumentException__ctor_m2D35EAD113C2ADC99EB17B940A2097A93FD23EFC (ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 * __this, String_t* ___message0, const RuntimeMethod* method);
// System.Void System.Action`1<UnityEngine.XR.XRNodeState>::Invoke(!0)
inline void Action_1_Invoke_mD7440CB91FE64B4EAD0D34248075E0F39797C946 (Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * __this, XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33  ___obj0, const RuntimeMethod* method)
{
	((  void (*) (Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 *, XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 , const RuntimeMethod*))Action_1_Invoke_mD7440CB91FE64B4EAD0D34248075E0F39797C946_gshared)(__this, ___obj0, method);
}
// UnityEngine.XR.MeshId UnityEngine.XR.MeshGenerationResult::get_MeshId()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  MeshGenerationResult_get_MeshId_m55663E958B980188CFD406BB2469B24D8089BA9E_inline (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method);
// UnityEngine.Mesh UnityEngine.XR.MeshGenerationResult::get_Mesh()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 * MeshGenerationResult_get_Mesh_mDBEB5E83FC729856B17AF62B9438C9B9A79A0200_inline (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method);
// UnityEngine.MeshCollider UnityEngine.XR.MeshGenerationResult::get_MeshCollider()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 * MeshGenerationResult_get_MeshCollider_m0285F3BFFFF0778DC8DDA97BFDAE30A19AEBF283_inline (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method);
// UnityEngine.XR.MeshGenerationStatus UnityEngine.XR.MeshGenerationResult::get_Status()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t MeshGenerationResult_get_Status_m5AF51D2745EC947BB722550DC95665D430CDB178_inline (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method);
// UnityEngine.XR.MeshVertexAttributes UnityEngine.XR.MeshGenerationResult::get_Attributes()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t MeshGenerationResult_get_Attributes_m24D66B0694F827D4DFFE210069EF0F5578F4845F_inline (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.XR.MeshGenerationResult::Equals(UnityEngine.XR.MeshGenerationResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MeshGenerationResult_Equals_mA9685353D67F169013BF8CC6B34EAFE69DB341B4 (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF  ___other0, const RuntimeMethod* method);
// System.Boolean UnityEngine.XR.MeshGenerationResult::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MeshGenerationResult_Equals_m511B6FD46B1187D90919F4C0D2E853DE4A16BD44 (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Boolean UnityEngine.XR.MeshId::Equals(UnityEngine.XR.MeshId)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MeshId_Equals_m685A94F74A3A06E6E51C60F1D40386CAA8F01834 (MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 * __this, MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  ___other0, const RuntimeMethod* method);
// System.Int32 UnityEngine.XR.MeshId::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t MeshId_GetHashCode_mCD3E4355DB5EE90C389CE1D742E4CCA6471E2AA6 (MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 * __this, const RuntimeMethod* method);
// System.Int32 UnityEngine.XR.HashCodeHelper::Combine(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t HashCodeHelper_Combine_m5F8B051AFC305B7FD377632031048F6549A1A543 (int32_t ___hash10, int32_t ___hash21, const RuntimeMethod* method);
// System.Int32 System.Int32::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Int32_GetHashCode_mEDD3F492A5F7CF021125AE3F38E2B8F8743FC667 (int32_t* __this, const RuntimeMethod* method);
// System.Int32 UnityEngine.XR.MeshGenerationResult::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t MeshGenerationResult_GetHashCode_m511BFBE4F21B162C59E462DF9DC9883A06CF9CC7 (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method);
// System.String System.UInt64::ToString(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* UInt64_ToString_mFE48F1D174A1F741AB0795C1164BF45BF37F86E6 (uint64_t* __this, String_t* ___format0, const RuntimeMethod* method);
// System.String System.String::Format(System.String,System.Object,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Format_m8D1CB0410C35E052A53AE957C914C841E54BAB66 (String_t* ___format0, RuntimeObject * ___arg01, RuntimeObject * ___arg12, const RuntimeMethod* method);
// System.String UnityEngine.XR.MeshId::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* MeshId_ToString_mA9CDBA01DD0C110252F6C4AA7437C507B2025705 (MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.XR.MeshId::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MeshId_Equals_m77D4535F7643D5C1FEA20600C92B73818DD8675E (MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Void System.Action`1<System.Boolean>::Invoke(!0)
inline void Action_1_Invoke_m95E5C9FC67F7B0BDC3CD5E00AC5D4C8A00C97AC5 (Action_1_tCE2D770918A65CAD277C08C4E8C05385EA267E83 * __this, bool ___obj0, const RuntimeMethod* method)
{
	((  void (*) (Action_1_tCE2D770918A65CAD277C08C4E8C05385EA267E83 *, bool, const RuntimeMethod*))Action_1_Invoke_m95E5C9FC67F7B0BDC3CD5E00AC5D4C8A00C97AC5_gshared)(__this, ___obj0, method);
}
// System.Void UnityEngine.IntegratedSubsystem`1<UnityEngine.XR.XRDisplaySubsystemDescriptor>::.ctor()
inline void IntegratedSubsystem_1__ctor_m33699A39FA5AEAE5A383689E4E0E3523FB67E558 (IntegratedSubsystem_1_t2737E0F52E6DC7B2E3D42D1B05C5FD7C9FDE4EA4 * __this, const RuntimeMethod* method)
{
	((  void (*) (IntegratedSubsystem_1_t2737E0F52E6DC7B2E3D42D1B05C5FD7C9FDE4EA4 *, const RuntimeMethod*))IntegratedSubsystem_1__ctor_m429238094F2A54BAF2F6F89270D5D43241541E72_gshared)(__this, method);
}
// System.Void UnityEngine.IntegratedSubsystemDescriptor`1<UnityEngine.XR.XRDisplaySubsystem>::.ctor()
inline void IntegratedSubsystemDescriptor_1__ctor_m3E9F6A2B441E056953C153C3B3182C0EB6BD0AFE (IntegratedSubsystemDescriptor_1_tFDF96CDD8FD2E980FF0C62E8161C66AF9FC212E2 * __this, const RuntimeMethod* method)
{
	((  void (*) (IntegratedSubsystemDescriptor_1_tFDF96CDD8FD2E980FF0C62E8161C66AF9FC212E2 *, const RuntimeMethod*))IntegratedSubsystemDescriptor_1__ctor_m74AFBEADF13921EE528F5629B38B89C8C51EB2DC_gshared)(__this, method);
}
// UnityEngine.IntegratedSubsystem UnityEngine.Internal_SubsystemInstances::Internal_GetInstanceByPtr(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * Internal_SubsystemInstances_Internal_GetInstanceByPtr_m5C2B49DC08EBCC5036465D8928090D71F1D420F5 (intptr_t ___ptr0, const RuntimeMethod* method);
// System.Void System.Action`1<UnityEngine.XR.XRInputSubsystem>::Invoke(!0)
inline void Action_1_Invoke_mA71F13E5E1EFDEB1DB1D9ED4C7ED037B21A89939 (Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 * __this, XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 * ___obj0, const RuntimeMethod* method)
{
	((  void (*) (Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 *, XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 *, const RuntimeMethod*))Action_1_Invoke_m587509C88BB83721D7918D89DF07606BB752D744_gshared)(__this, ___obj0, method);
}
// System.Void UnityEngine.IntegratedSubsystem`1<UnityEngine.XR.XRInputSubsystemDescriptor>::.ctor()
inline void IntegratedSubsystem_1__ctor_m19C9BE11CA13915E2E14D5B4EC3EAF29CCC633E5 (IntegratedSubsystem_1_tD5C4AF38726B9433CFC3CA0F889D8C8C2535AEFE * __this, const RuntimeMethod* method)
{
	((  void (*) (IntegratedSubsystem_1_tD5C4AF38726B9433CFC3CA0F889D8C8C2535AEFE *, const RuntimeMethod*))IntegratedSubsystem_1__ctor_m429238094F2A54BAF2F6F89270D5D43241541E72_gshared)(__this, method);
}
// System.Void UnityEngine.IntegratedSubsystemDescriptor`1<UnityEngine.XR.XRInputSubsystem>::.ctor()
inline void IntegratedSubsystemDescriptor_1__ctor_m98CC72EADB42D92099DBE358C296423D7751A741 (IntegratedSubsystemDescriptor_1_t7D61E241AA40ECC23A367A5FAF509A54B1F77EF2 * __this, const RuntimeMethod* method)
{
	((  void (*) (IntegratedSubsystemDescriptor_1_t7D61E241AA40ECC23A367A5FAF509A54B1F77EF2 *, const RuntimeMethod*))IntegratedSubsystemDescriptor_1__ctor_m74AFBEADF13921EE528F5629B38B89C8C51EB2DC_gshared)(__this, method);
}
// System.Void System.Action`1<UnityEngine.XR.MeshGenerationResult>::Invoke(!0)
inline void Action_1_Invoke_mC3DCAEAD9DC81FE145B4FE115F830C0767728604 (Action_1_tB125CDA27D619FDBF92F767804A14CF83EA85A3C * __this, MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF  ___obj0, const RuntimeMethod* method)
{
	((  void (*) (Action_1_tB125CDA27D619FDBF92F767804A14CF83EA85A3C *, MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF , const RuntimeMethod*))Action_1_Invoke_mC3DCAEAD9DC81FE145B4FE115F830C0767728604_gshared)(__this, ___obj0, method);
}
// System.Void UnityEngine.IntegratedSubsystem`1<UnityEngine.XR.XRMeshSubsystemDescriptor>::.ctor()
inline void IntegratedSubsystem_1__ctor_m5D5CDD514B75369B0797B55401D9DD35908A2A26 (IntegratedSubsystem_1_t902A5B61CE879B3CD855E5CE6CAEEB1B9752E840 * __this, const RuntimeMethod* method)
{
	((  void (*) (IntegratedSubsystem_1_t902A5B61CE879B3CD855E5CE6CAEEB1B9752E840 *, const RuntimeMethod*))IntegratedSubsystem_1__ctor_m429238094F2A54BAF2F6F89270D5D43241541E72_gshared)(__this, method);
}
// System.Void UnityEngine.IntegratedSubsystemDescriptor`1<UnityEngine.XR.XRMeshSubsystem>::.ctor()
inline void IntegratedSubsystemDescriptor_1__ctor_mBD46E84CF05A1E63F8FE6AB0C2F1C07AA8D2DAFB (IntegratedSubsystemDescriptor_1_t822E08B2CE1EC68FE74F71A682C9ECC6D52A6E89 * __this, const RuntimeMethod* method)
{
	((  void (*) (IntegratedSubsystemDescriptor_1_t822E08B2CE1EC68FE74F71A682C9ECC6D52A6E89 *, const RuntimeMethod*))IntegratedSubsystemDescriptor_1__ctor_m74AFBEADF13921EE528F5629B38B89C8C51EB2DC_gshared)(__this, method);
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
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.UInt64 UnityEngine.XR.Bone::get_deviceId()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint64_t Bone_get_deviceId_mF1EBAD176E69C1074F55BFFF36372A48840B2FA2 (Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * __this, const RuntimeMethod* method)
{
	uint64_t V_0 = 0;
	{
		uint64_t L_0 = __this->get_m_DeviceId_0();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		uint64_t L_1 = V_0;
		return L_1;
	}
}
IL2CPP_EXTERN_C  uint64_t Bone_get_deviceId_mF1EBAD176E69C1074F55BFFF36372A48840B2FA2_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * _thisAdjusted = reinterpret_cast<Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 *>(__this + _offset);
	return Bone_get_deviceId_mF1EBAD176E69C1074F55BFFF36372A48840B2FA2(_thisAdjusted, method);
}
// System.UInt32 UnityEngine.XR.Bone::get_featureIndex()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t Bone_get_featureIndex_m291227F499665BECC9D40723157367ECB56781C3 (Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * __this, const RuntimeMethod* method)
{
	uint32_t V_0 = 0;
	{
		uint32_t L_0 = __this->get_m_FeatureIndex_1();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		uint32_t L_1 = V_0;
		return L_1;
	}
}
IL2CPP_EXTERN_C  uint32_t Bone_get_featureIndex_m291227F499665BECC9D40723157367ECB56781C3_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * _thisAdjusted = reinterpret_cast<Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 *>(__this + _offset);
	return Bone_get_featureIndex_m291227F499665BECC9D40723157367ECB56781C3(_thisAdjusted, method);
}
// System.Boolean UnityEngine.XR.Bone::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Bone_Equals_m2FBDCFEA8B90663E546294EBEB4763538DEEA412 (Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Bone_Equals_m2FBDCFEA8B90663E546294EBEB4763538DEEA412_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	bool V_1 = false;
	{
		RuntimeObject * L_0 = ___obj0;
		V_0 = (bool)((((int32_t)((!(((RuntimeObject*)(RuntimeObject *)((RuntimeObject *)IsInstSealed((RuntimeObject*)L_0, Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070_il2cpp_TypeInfo_var))) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_1 = V_0;
		if (!L_1)
		{
			goto IL_0015;
		}
	}
	{
		V_1 = (bool)0;
		goto IL_0024;
	}

IL_0015:
	{
		RuntimeObject * L_2 = ___obj0;
		bool L_3 = Bone_Equals_m025222EEBF2374226C62C32E1A7ADE3538C13AFB((Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 *)__this, ((*(Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 *)((Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 *)UnBox(L_2, Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070_il2cpp_TypeInfo_var)))), /*hidden argument*/NULL);
		V_1 = L_3;
		goto IL_0024;
	}

IL_0024:
	{
		bool L_4 = V_1;
		return L_4;
	}
}
IL2CPP_EXTERN_C  bool Bone_Equals_m2FBDCFEA8B90663E546294EBEB4763538DEEA412_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * _thisAdjusted = reinterpret_cast<Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 *>(__this + _offset);
	return Bone_Equals_m2FBDCFEA8B90663E546294EBEB4763538DEEA412(_thisAdjusted, ___obj0, method);
}
// System.Boolean UnityEngine.XR.Bone::Equals(UnityEngine.XR.Bone)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Bone_Equals_m025222EEBF2374226C62C32E1A7ADE3538C13AFB (Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * __this, Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070  ___other0, const RuntimeMethod* method)
{
	bool V_0 = false;
	int32_t G_B3_0 = 0;
	{
		uint64_t L_0 = Bone_get_deviceId_mF1EBAD176E69C1074F55BFFF36372A48840B2FA2((Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 *)__this, /*hidden argument*/NULL);
		uint64_t L_1 = Bone_get_deviceId_mF1EBAD176E69C1074F55BFFF36372A48840B2FA2((Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 *)(&___other0), /*hidden argument*/NULL);
		if ((!(((uint64_t)L_0) == ((uint64_t)L_1))))
		{
			goto IL_0021;
		}
	}
	{
		uint32_t L_2 = Bone_get_featureIndex_m291227F499665BECC9D40723157367ECB56781C3((Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 *)__this, /*hidden argument*/NULL);
		uint32_t L_3 = Bone_get_featureIndex_m291227F499665BECC9D40723157367ECB56781C3((Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 *)(&___other0), /*hidden argument*/NULL);
		G_B3_0 = ((((int32_t)L_2) == ((int32_t)L_3))? 1 : 0);
		goto IL_0022;
	}

IL_0021:
	{
		G_B3_0 = 0;
	}

IL_0022:
	{
		V_0 = (bool)G_B3_0;
		goto IL_0025;
	}

IL_0025:
	{
		bool L_4 = V_0;
		return L_4;
	}
}
IL2CPP_EXTERN_C  bool Bone_Equals_m025222EEBF2374226C62C32E1A7ADE3538C13AFB_AdjustorThunk (RuntimeObject * __this, Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * _thisAdjusted = reinterpret_cast<Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 *>(__this + _offset);
	return Bone_Equals_m025222EEBF2374226C62C32E1A7ADE3538C13AFB(_thisAdjusted, ___other0, method);
}
// System.Int32 UnityEngine.XR.Bone::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Bone_GetHashCode_m0ED7925C274DE8439749951FBE4E4EC28A797BFB (Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * __this, const RuntimeMethod* method)
{
	uint64_t V_0 = 0;
	uint32_t V_1 = 0;
	int32_t V_2 = 0;
	{
		uint64_t L_0 = Bone_get_deviceId_mF1EBAD176E69C1074F55BFFF36372A48840B2FA2((Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 *)__this, /*hidden argument*/NULL);
		V_0 = L_0;
		int32_t L_1 = UInt64_GetHashCode_mCDF662897A3F02CED11A9F9E66C5BF4E28C02B33((uint64_t*)(&V_0), /*hidden argument*/NULL);
		uint32_t L_2 = Bone_get_featureIndex_m291227F499665BECC9D40723157367ECB56781C3((Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 *)__this, /*hidden argument*/NULL);
		V_1 = L_2;
		int32_t L_3 = UInt32_GetHashCode_m60E3A243F3D79311A64836148AE1AC23C679FC45((uint32_t*)(&V_1), /*hidden argument*/NULL);
		V_2 = ((int32_t)((int32_t)L_1^(int32_t)((int32_t)((int32_t)L_3<<(int32_t)1))));
		goto IL_0023;
	}

IL_0023:
	{
		int32_t L_4 = V_2;
		return L_4;
	}
}
IL2CPP_EXTERN_C  int32_t Bone_GetHashCode_m0ED7925C274DE8439749951FBE4E4EC28A797BFB_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 * _thisAdjusted = reinterpret_cast<Bone_t8EDF2FA2139528015195AF2EA866A28947C3F070 *>(__this + _offset);
	return Bone_GetHashCode_m0ED7925C274DE8439749951FBE4E4EC28A797BFB(_thisAdjusted, method);
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
// System.UInt64 UnityEngine.XR.Eyes::get_deviceId()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint64_t Eyes_get_deviceId_m5587223FE834DE2ABDC32F9E8FE6D768D4DDF936 (Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * __this, const RuntimeMethod* method)
{
	uint64_t V_0 = 0;
	{
		uint64_t L_0 = __this->get_m_DeviceId_0();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		uint64_t L_1 = V_0;
		return L_1;
	}
}
IL2CPP_EXTERN_C  uint64_t Eyes_get_deviceId_m5587223FE834DE2ABDC32F9E8FE6D768D4DDF936_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * _thisAdjusted = reinterpret_cast<Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D *>(__this + _offset);
	return Eyes_get_deviceId_m5587223FE834DE2ABDC32F9E8FE6D768D4DDF936(_thisAdjusted, method);
}
// System.UInt32 UnityEngine.XR.Eyes::get_featureIndex()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t Eyes_get_featureIndex_mBABAD5CFCF0585EE4791C0E1C3E2562627491237 (Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * __this, const RuntimeMethod* method)
{
	uint32_t V_0 = 0;
	{
		uint32_t L_0 = __this->get_m_FeatureIndex_1();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		uint32_t L_1 = V_0;
		return L_1;
	}
}
IL2CPP_EXTERN_C  uint32_t Eyes_get_featureIndex_mBABAD5CFCF0585EE4791C0E1C3E2562627491237_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * _thisAdjusted = reinterpret_cast<Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D *>(__this + _offset);
	return Eyes_get_featureIndex_mBABAD5CFCF0585EE4791C0E1C3E2562627491237(_thisAdjusted, method);
}
// System.Boolean UnityEngine.XR.Eyes::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Eyes_Equals_m58897DB2EEC48809233B94BB4CEA166B8ACEBFF2 (Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Eyes_Equals_m58897DB2EEC48809233B94BB4CEA166B8ACEBFF2_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	bool V_1 = false;
	{
		RuntimeObject * L_0 = ___obj0;
		V_0 = (bool)((((int32_t)((!(((RuntimeObject*)(RuntimeObject *)((RuntimeObject *)IsInstSealed((RuntimeObject*)L_0, Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D_il2cpp_TypeInfo_var))) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_1 = V_0;
		if (!L_1)
		{
			goto IL_0015;
		}
	}
	{
		V_1 = (bool)0;
		goto IL_0024;
	}

IL_0015:
	{
		RuntimeObject * L_2 = ___obj0;
		bool L_3 = Eyes_Equals_m60083B041FECD045D717F2873BF2E21AA9A4FD01((Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D *)__this, ((*(Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D *)((Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D *)UnBox(L_2, Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D_il2cpp_TypeInfo_var)))), /*hidden argument*/NULL);
		V_1 = L_3;
		goto IL_0024;
	}

IL_0024:
	{
		bool L_4 = V_1;
		return L_4;
	}
}
IL2CPP_EXTERN_C  bool Eyes_Equals_m58897DB2EEC48809233B94BB4CEA166B8ACEBFF2_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * _thisAdjusted = reinterpret_cast<Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D *>(__this + _offset);
	return Eyes_Equals_m58897DB2EEC48809233B94BB4CEA166B8ACEBFF2(_thisAdjusted, ___obj0, method);
}
// System.Boolean UnityEngine.XR.Eyes::Equals(UnityEngine.XR.Eyes)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Eyes_Equals_m60083B041FECD045D717F2873BF2E21AA9A4FD01 (Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * __this, Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D  ___other0, const RuntimeMethod* method)
{
	bool V_0 = false;
	int32_t G_B3_0 = 0;
	{
		uint64_t L_0 = Eyes_get_deviceId_m5587223FE834DE2ABDC32F9E8FE6D768D4DDF936((Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D *)__this, /*hidden argument*/NULL);
		uint64_t L_1 = Eyes_get_deviceId_m5587223FE834DE2ABDC32F9E8FE6D768D4DDF936((Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D *)(&___other0), /*hidden argument*/NULL);
		if ((!(((uint64_t)L_0) == ((uint64_t)L_1))))
		{
			goto IL_0021;
		}
	}
	{
		uint32_t L_2 = Eyes_get_featureIndex_mBABAD5CFCF0585EE4791C0E1C3E2562627491237((Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D *)__this, /*hidden argument*/NULL);
		uint32_t L_3 = Eyes_get_featureIndex_mBABAD5CFCF0585EE4791C0E1C3E2562627491237((Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D *)(&___other0), /*hidden argument*/NULL);
		G_B3_0 = ((((int32_t)L_2) == ((int32_t)L_3))? 1 : 0);
		goto IL_0022;
	}

IL_0021:
	{
		G_B3_0 = 0;
	}

IL_0022:
	{
		V_0 = (bool)G_B3_0;
		goto IL_0025;
	}

IL_0025:
	{
		bool L_4 = V_0;
		return L_4;
	}
}
IL2CPP_EXTERN_C  bool Eyes_Equals_m60083B041FECD045D717F2873BF2E21AA9A4FD01_AdjustorThunk (RuntimeObject * __this, Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * _thisAdjusted = reinterpret_cast<Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D *>(__this + _offset);
	return Eyes_Equals_m60083B041FECD045D717F2873BF2E21AA9A4FD01(_thisAdjusted, ___other0, method);
}
// System.Int32 UnityEngine.XR.Eyes::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Eyes_GetHashCode_mA9742A7C19500C6C17EDCD5EED7954A9FE91C0DD (Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * __this, const RuntimeMethod* method)
{
	uint64_t V_0 = 0;
	uint32_t V_1 = 0;
	int32_t V_2 = 0;
	{
		uint64_t L_0 = Eyes_get_deviceId_m5587223FE834DE2ABDC32F9E8FE6D768D4DDF936((Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D *)__this, /*hidden argument*/NULL);
		V_0 = L_0;
		int32_t L_1 = UInt64_GetHashCode_mCDF662897A3F02CED11A9F9E66C5BF4E28C02B33((uint64_t*)(&V_0), /*hidden argument*/NULL);
		uint32_t L_2 = Eyes_get_featureIndex_mBABAD5CFCF0585EE4791C0E1C3E2562627491237((Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D *)__this, /*hidden argument*/NULL);
		V_1 = L_2;
		int32_t L_3 = UInt32_GetHashCode_m60E3A243F3D79311A64836148AE1AC23C679FC45((uint32_t*)(&V_1), /*hidden argument*/NULL);
		V_2 = ((int32_t)((int32_t)L_1^(int32_t)((int32_t)((int32_t)L_3<<(int32_t)1))));
		goto IL_0023;
	}

IL_0023:
	{
		int32_t L_4 = V_2;
		return L_4;
	}
}
IL2CPP_EXTERN_C  int32_t Eyes_GetHashCode_mA9742A7C19500C6C17EDCD5EED7954A9FE91C0DD_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D * _thisAdjusted = reinterpret_cast<Eyes_t8097B0BA9FC12824F6ABD076309CEAC1047C094D *>(__this + _offset);
	return Eyes_GetHashCode_mA9742A7C19500C6C17EDCD5EED7954A9FE91C0DD(_thisAdjusted, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.UInt64 UnityEngine.XR.Hand::get_deviceId()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint64_t Hand_get_deviceId_mD45907CDFDBF2CE754444358020196A8C6BEE3D1 (Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * __this, const RuntimeMethod* method)
{
	uint64_t V_0 = 0;
	{
		uint64_t L_0 = __this->get_m_DeviceId_0();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		uint64_t L_1 = V_0;
		return L_1;
	}
}
IL2CPP_EXTERN_C  uint64_t Hand_get_deviceId_mD45907CDFDBF2CE754444358020196A8C6BEE3D1_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * _thisAdjusted = reinterpret_cast<Hand_tB64007EC8D01384426C93432737BA9C5F636A690 *>(__this + _offset);
	return Hand_get_deviceId_mD45907CDFDBF2CE754444358020196A8C6BEE3D1(_thisAdjusted, method);
}
// System.UInt32 UnityEngine.XR.Hand::get_featureIndex()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t Hand_get_featureIndex_mB8A458180449B2C2D38D5405BA26577CF1A60897 (Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * __this, const RuntimeMethod* method)
{
	uint32_t V_0 = 0;
	{
		uint32_t L_0 = __this->get_m_FeatureIndex_1();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		uint32_t L_1 = V_0;
		return L_1;
	}
}
IL2CPP_EXTERN_C  uint32_t Hand_get_featureIndex_mB8A458180449B2C2D38D5405BA26577CF1A60897_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * _thisAdjusted = reinterpret_cast<Hand_tB64007EC8D01384426C93432737BA9C5F636A690 *>(__this + _offset);
	return Hand_get_featureIndex_mB8A458180449B2C2D38D5405BA26577CF1A60897(_thisAdjusted, method);
}
// System.Boolean UnityEngine.XR.Hand::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Hand_Equals_m727113281F30E554A3A60DEFC4ED61CC94901775 (Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Hand_Equals_m727113281F30E554A3A60DEFC4ED61CC94901775_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	bool V_1 = false;
	{
		RuntimeObject * L_0 = ___obj0;
		V_0 = (bool)((((int32_t)((!(((RuntimeObject*)(RuntimeObject *)((RuntimeObject *)IsInstSealed((RuntimeObject*)L_0, Hand_tB64007EC8D01384426C93432737BA9C5F636A690_il2cpp_TypeInfo_var))) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_1 = V_0;
		if (!L_1)
		{
			goto IL_0015;
		}
	}
	{
		V_1 = (bool)0;
		goto IL_0024;
	}

IL_0015:
	{
		RuntimeObject * L_2 = ___obj0;
		bool L_3 = Hand_Equals_m099C7921633095BB40C830B6F5558F99ED0D7BD0((Hand_tB64007EC8D01384426C93432737BA9C5F636A690 *)__this, ((*(Hand_tB64007EC8D01384426C93432737BA9C5F636A690 *)((Hand_tB64007EC8D01384426C93432737BA9C5F636A690 *)UnBox(L_2, Hand_tB64007EC8D01384426C93432737BA9C5F636A690_il2cpp_TypeInfo_var)))), /*hidden argument*/NULL);
		V_1 = L_3;
		goto IL_0024;
	}

IL_0024:
	{
		bool L_4 = V_1;
		return L_4;
	}
}
IL2CPP_EXTERN_C  bool Hand_Equals_m727113281F30E554A3A60DEFC4ED61CC94901775_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * _thisAdjusted = reinterpret_cast<Hand_tB64007EC8D01384426C93432737BA9C5F636A690 *>(__this + _offset);
	return Hand_Equals_m727113281F30E554A3A60DEFC4ED61CC94901775(_thisAdjusted, ___obj0, method);
}
// System.Boolean UnityEngine.XR.Hand::Equals(UnityEngine.XR.Hand)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Hand_Equals_m099C7921633095BB40C830B6F5558F99ED0D7BD0 (Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * __this, Hand_tB64007EC8D01384426C93432737BA9C5F636A690  ___other0, const RuntimeMethod* method)
{
	bool V_0 = false;
	int32_t G_B3_0 = 0;
	{
		uint64_t L_0 = Hand_get_deviceId_mD45907CDFDBF2CE754444358020196A8C6BEE3D1((Hand_tB64007EC8D01384426C93432737BA9C5F636A690 *)__this, /*hidden argument*/NULL);
		uint64_t L_1 = Hand_get_deviceId_mD45907CDFDBF2CE754444358020196A8C6BEE3D1((Hand_tB64007EC8D01384426C93432737BA9C5F636A690 *)(&___other0), /*hidden argument*/NULL);
		if ((!(((uint64_t)L_0) == ((uint64_t)L_1))))
		{
			goto IL_0021;
		}
	}
	{
		uint32_t L_2 = Hand_get_featureIndex_mB8A458180449B2C2D38D5405BA26577CF1A60897((Hand_tB64007EC8D01384426C93432737BA9C5F636A690 *)__this, /*hidden argument*/NULL);
		uint32_t L_3 = Hand_get_featureIndex_mB8A458180449B2C2D38D5405BA26577CF1A60897((Hand_tB64007EC8D01384426C93432737BA9C5F636A690 *)(&___other0), /*hidden argument*/NULL);
		G_B3_0 = ((((int32_t)L_2) == ((int32_t)L_3))? 1 : 0);
		goto IL_0022;
	}

IL_0021:
	{
		G_B3_0 = 0;
	}

IL_0022:
	{
		V_0 = (bool)G_B3_0;
		goto IL_0025;
	}

IL_0025:
	{
		bool L_4 = V_0;
		return L_4;
	}
}
IL2CPP_EXTERN_C  bool Hand_Equals_m099C7921633095BB40C830B6F5558F99ED0D7BD0_AdjustorThunk (RuntimeObject * __this, Hand_tB64007EC8D01384426C93432737BA9C5F636A690  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * _thisAdjusted = reinterpret_cast<Hand_tB64007EC8D01384426C93432737BA9C5F636A690 *>(__this + _offset);
	return Hand_Equals_m099C7921633095BB40C830B6F5558F99ED0D7BD0(_thisAdjusted, ___other0, method);
}
// System.Int32 UnityEngine.XR.Hand::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Hand_GetHashCode_mFC8C4732F0B728122C3ABCC623699DA7E47D1CCB (Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * __this, const RuntimeMethod* method)
{
	uint64_t V_0 = 0;
	uint32_t V_1 = 0;
	int32_t V_2 = 0;
	{
		uint64_t L_0 = Hand_get_deviceId_mD45907CDFDBF2CE754444358020196A8C6BEE3D1((Hand_tB64007EC8D01384426C93432737BA9C5F636A690 *)__this, /*hidden argument*/NULL);
		V_0 = L_0;
		int32_t L_1 = UInt64_GetHashCode_mCDF662897A3F02CED11A9F9E66C5BF4E28C02B33((uint64_t*)(&V_0), /*hidden argument*/NULL);
		uint32_t L_2 = Hand_get_featureIndex_mB8A458180449B2C2D38D5405BA26577CF1A60897((Hand_tB64007EC8D01384426C93432737BA9C5F636A690 *)__this, /*hidden argument*/NULL);
		V_1 = L_2;
		int32_t L_3 = UInt32_GetHashCode_m60E3A243F3D79311A64836148AE1AC23C679FC45((uint32_t*)(&V_1), /*hidden argument*/NULL);
		V_2 = ((int32_t)((int32_t)L_1^(int32_t)((int32_t)((int32_t)L_3<<(int32_t)1))));
		goto IL_0023;
	}

IL_0023:
	{
		int32_t L_4 = V_2;
		return L_4;
	}
}
IL2CPP_EXTERN_C  int32_t Hand_GetHashCode_mFC8C4732F0B728122C3ABCC623699DA7E47D1CCB_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Hand_tB64007EC8D01384426C93432737BA9C5F636A690 * _thisAdjusted = reinterpret_cast<Hand_tB64007EC8D01384426C93432737BA9C5F636A690 *>(__this + _offset);
	return Hand_GetHashCode_mFC8C4732F0B728122C3ABCC623699DA7E47D1CCB(_thisAdjusted, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Int32 UnityEngine.XR.HashCodeHelper::Combine(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t HashCodeHelper_Combine_m5F8B051AFC305B7FD377632031048F6549A1A543 (int32_t ___hash10, int32_t ___hash21, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		int32_t L_0 = ___hash10;
		int32_t L_1 = ___hash21;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)((int32_t)486187739))), (int32_t)L_1));
		goto IL_000d;
	}

IL_000d:
	{
		int32_t L_2 = V_0;
		return L_2;
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
// Conversion methods for marshalling of: UnityEngine.XR.InputDevice
IL2CPP_EXTERN_C void InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_marshal_pinvoke(const InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E& unmarshaled, InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_marshaled_pinvoke& marshaled)
{
	marshaled.___m_DeviceId_0 = unmarshaled.get_m_DeviceId_0();
	marshaled.___m_Initialized_1 = static_cast<int32_t>(unmarshaled.get_m_Initialized_1());
}
IL2CPP_EXTERN_C void InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_marshal_pinvoke_back(const InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_marshaled_pinvoke& marshaled, InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E& unmarshaled)
{
	uint64_t unmarshaled_m_DeviceId_temp_0 = 0;
	unmarshaled_m_DeviceId_temp_0 = marshaled.___m_DeviceId_0;
	unmarshaled.set_m_DeviceId_0(unmarshaled_m_DeviceId_temp_0);
	bool unmarshaled_m_Initialized_temp_1 = false;
	unmarshaled_m_Initialized_temp_1 = static_cast<bool>(marshaled.___m_Initialized_1);
	unmarshaled.set_m_Initialized_1(unmarshaled_m_Initialized_temp_1);
}
// Conversion method for clean up from marshalling of: UnityEngine.XR.InputDevice
IL2CPP_EXTERN_C void InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_marshal_pinvoke_cleanup(InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: UnityEngine.XR.InputDevice
IL2CPP_EXTERN_C void InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_marshal_com(const InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E& unmarshaled, InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_marshaled_com& marshaled)
{
	marshaled.___m_DeviceId_0 = unmarshaled.get_m_DeviceId_0();
	marshaled.___m_Initialized_1 = static_cast<int32_t>(unmarshaled.get_m_Initialized_1());
}
IL2CPP_EXTERN_C void InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_marshal_com_back(const InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_marshaled_com& marshaled, InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E& unmarshaled)
{
	uint64_t unmarshaled_m_DeviceId_temp_0 = 0;
	unmarshaled_m_DeviceId_temp_0 = marshaled.___m_DeviceId_0;
	unmarshaled.set_m_DeviceId_0(unmarshaled_m_DeviceId_temp_0);
	bool unmarshaled_m_Initialized_temp_1 = false;
	unmarshaled_m_Initialized_temp_1 = static_cast<bool>(marshaled.___m_Initialized_1);
	unmarshaled.set_m_Initialized_1(unmarshaled_m_Initialized_temp_1);
}
// Conversion method for clean up from marshalling of: UnityEngine.XR.InputDevice
IL2CPP_EXTERN_C void InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_marshal_com_cleanup(InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_marshaled_com& marshaled)
{
}
// System.Void UnityEngine.XR.InputDevice::.ctor(System.UInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void InputDevice__ctor_m610666CA01BCAF92464BE9C17BFB133A23A663BE (InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * __this, uint64_t ___deviceId0, const RuntimeMethod* method)
{
	{
		uint64_t L_0 = ___deviceId0;
		__this->set_m_DeviceId_0(L_0);
		__this->set_m_Initialized_1((bool)1);
		return;
	}
}
IL2CPP_EXTERN_C  void InputDevice__ctor_m610666CA01BCAF92464BE9C17BFB133A23A663BE_AdjustorThunk (RuntimeObject * __this, uint64_t ___deviceId0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * _thisAdjusted = reinterpret_cast<InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E *>(__this + _offset);
	InputDevice__ctor_m610666CA01BCAF92464BE9C17BFB133A23A663BE(_thisAdjusted, ___deviceId0, method);
}
// System.UInt64 UnityEngine.XR.InputDevice::get_deviceId()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint64_t InputDevice_get_deviceId_m86C962E24F26D879240A6E889E9E8D3D94F546A5 (InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * __this, const RuntimeMethod* method)
{
	uint64_t V_0 = 0;
	uint64_t G_B3_0 = 0;
	{
		bool L_0 = __this->get_m_Initialized_1();
		if (L_0)
		{
			goto IL_000d;
		}
	}
	{
		G_B3_0 = ((uint64_t)((((int64_t)((int64_t)(-1))))));
		goto IL_0013;
	}

IL_000d:
	{
		uint64_t L_1 = __this->get_m_DeviceId_0();
		G_B3_0 = L_1;
	}

IL_0013:
	{
		V_0 = G_B3_0;
		goto IL_0016;
	}

IL_0016:
	{
		uint64_t L_2 = V_0;
		return L_2;
	}
}
IL2CPP_EXTERN_C  uint64_t InputDevice_get_deviceId_m86C962E24F26D879240A6E889E9E8D3D94F546A5_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * _thisAdjusted = reinterpret_cast<InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E *>(__this + _offset);
	return InputDevice_get_deviceId_m86C962E24F26D879240A6E889E9E8D3D94F546A5(_thisAdjusted, method);
}
// System.Boolean UnityEngine.XR.InputDevice::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool InputDevice_Equals_mF29A225E81A87941551F70A2351CB803A6D94063 (InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (InputDevice_Equals_mF29A225E81A87941551F70A2351CB803A6D94063_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	bool V_1 = false;
	{
		RuntimeObject * L_0 = ___obj0;
		V_0 = (bool)((((int32_t)((!(((RuntimeObject*)(RuntimeObject *)((RuntimeObject *)IsInstSealed((RuntimeObject*)L_0, InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_il2cpp_TypeInfo_var))) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_1 = V_0;
		if (!L_1)
		{
			goto IL_0015;
		}
	}
	{
		V_1 = (bool)0;
		goto IL_0024;
	}

IL_0015:
	{
		RuntimeObject * L_2 = ___obj0;
		bool L_3 = InputDevice_Equals_m9661F95B50387146BC3C6F5DC63FDF2B1303ABB7((InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E *)__this, ((*(InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E *)((InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E *)UnBox(L_2, InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E_il2cpp_TypeInfo_var)))), /*hidden argument*/NULL);
		V_1 = L_3;
		goto IL_0024;
	}

IL_0024:
	{
		bool L_4 = V_1;
		return L_4;
	}
}
IL2CPP_EXTERN_C  bool InputDevice_Equals_mF29A225E81A87941551F70A2351CB803A6D94063_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * _thisAdjusted = reinterpret_cast<InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E *>(__this + _offset);
	return InputDevice_Equals_mF29A225E81A87941551F70A2351CB803A6D94063(_thisAdjusted, ___obj0, method);
}
// System.Boolean UnityEngine.XR.InputDevice::Equals(UnityEngine.XR.InputDevice)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool InputDevice_Equals_m9661F95B50387146BC3C6F5DC63FDF2B1303ABB7 (InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * __this, InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E  ___other0, const RuntimeMethod* method)
{
	bool V_0 = false;
	{
		uint64_t L_0 = InputDevice_get_deviceId_m86C962E24F26D879240A6E889E9E8D3D94F546A5((InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E *)__this, /*hidden argument*/NULL);
		uint64_t L_1 = InputDevice_get_deviceId_m86C962E24F26D879240A6E889E9E8D3D94F546A5((InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E *)(&___other0), /*hidden argument*/NULL);
		V_0 = (bool)((((int64_t)L_0) == ((int64_t)L_1))? 1 : 0);
		goto IL_0013;
	}

IL_0013:
	{
		bool L_2 = V_0;
		return L_2;
	}
}
IL2CPP_EXTERN_C  bool InputDevice_Equals_m9661F95B50387146BC3C6F5DC63FDF2B1303ABB7_AdjustorThunk (RuntimeObject * __this, InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * _thisAdjusted = reinterpret_cast<InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E *>(__this + _offset);
	return InputDevice_Equals_m9661F95B50387146BC3C6F5DC63FDF2B1303ABB7(_thisAdjusted, ___other0, method);
}
// System.Int32 UnityEngine.XR.InputDevice::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t InputDevice_GetHashCode_mFD9C9A6015B91E254DD53E14651C8D08F715D7F8 (InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * __this, const RuntimeMethod* method)
{
	uint64_t V_0 = 0;
	int32_t V_1 = 0;
	{
		uint64_t L_0 = InputDevice_get_deviceId_m86C962E24F26D879240A6E889E9E8D3D94F546A5((InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E *)__this, /*hidden argument*/NULL);
		V_0 = L_0;
		int32_t L_1 = UInt64_GetHashCode_mCDF662897A3F02CED11A9F9E66C5BF4E28C02B33((uint64_t*)(&V_0), /*hidden argument*/NULL);
		V_1 = L_1;
		goto IL_0012;
	}

IL_0012:
	{
		int32_t L_2 = V_1;
		return L_2;
	}
}
IL2CPP_EXTERN_C  int32_t InputDevice_GetHashCode_mFD9C9A6015B91E254DD53E14651C8D08F715D7F8_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E * _thisAdjusted = reinterpret_cast<InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E *>(__this + _offset);
	return InputDevice_GetHashCode_mFD9C9A6015B91E254DD53E14651C8D08F715D7F8(_thisAdjusted, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Conversion methods for marshalling of: UnityEngine.XR.InputDevices
IL2CPP_EXTERN_C void InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_marshal_pinvoke(const InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC& unmarshaled, InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_marshaled_pinvoke& marshaled)
{
}
IL2CPP_EXTERN_C void InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_marshal_pinvoke_back(const InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_marshaled_pinvoke& marshaled, InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC& unmarshaled)
{
}
// Conversion method for clean up from marshalling of: UnityEngine.XR.InputDevices
IL2CPP_EXTERN_C void InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_marshal_pinvoke_cleanup(InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: UnityEngine.XR.InputDevices
IL2CPP_EXTERN_C void InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_marshal_com(const InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC& unmarshaled, InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_marshaled_com& marshaled)
{
}
IL2CPP_EXTERN_C void InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_marshal_com_back(const InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_marshaled_com& marshaled, InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC& unmarshaled)
{
}
// Conversion method for clean up from marshalling of: UnityEngine.XR.InputDevices
IL2CPP_EXTERN_C void InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_marshal_com_cleanup(InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_marshaled_com& marshaled)
{
}
// System.Void UnityEngine.XR.InputDevices::InvokeConnectionEvent(System.UInt64,UnityEngine.XR.ConnectionChangeType)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void InputDevices_InvokeConnectionEvent_m19E87BB6671D4B4CE3EB322EEE3621B0146A7077 (uint64_t ___deviceId0, uint32_t ___change1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (InputDevices_InvokeConnectionEvent_m19E87BB6671D4B4CE3EB322EEE3621B0146A7077_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint32_t V_0 = 0;
	bool V_1 = false;
	bool V_2 = false;
	bool V_3 = false;
	{
		uint32_t L_0 = ___change1;
		V_0 = L_0;
		uint32_t L_1 = V_0;
		switch (L_1)
		{
			case 0:
			{
				goto IL_0017;
			}
			case 1:
			{
				goto IL_0037;
			}
			case 2:
			{
				goto IL_0057;
			}
		}
	}
	{
		goto IL_0077;
	}

IL_0017:
	{
		Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * L_2 = ((InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_StaticFields*)il2cpp_codegen_static_fields_for(InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_il2cpp_TypeInfo_var))->get_deviceConnected_0();
		V_1 = (bool)((!(((RuntimeObject*)(Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 *)L_2) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_3 = V_1;
		if (!L_3)
		{
			goto IL_0035;
		}
	}
	{
		Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * L_4 = ((InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_StaticFields*)il2cpp_codegen_static_fields_for(InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_il2cpp_TypeInfo_var))->get_deviceConnected_0();
		uint64_t L_5 = ___deviceId0;
		InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E  L_6;
		memset((&L_6), 0, sizeof(L_6));
		InputDevice__ctor_m610666CA01BCAF92464BE9C17BFB133A23A663BE((&L_6), L_5, /*hidden argument*/NULL);
		NullCheck(L_4);
		Action_1_Invoke_mA460B359B13BDCF71CDDE9A8A960C827A4E3E970(L_4, L_6, /*hidden argument*/Action_1_Invoke_mA460B359B13BDCF71CDDE9A8A960C827A4E3E970_RuntimeMethod_var);
	}

IL_0035:
	{
		goto IL_0077;
	}

IL_0037:
	{
		Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * L_7 = ((InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_StaticFields*)il2cpp_codegen_static_fields_for(InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_il2cpp_TypeInfo_var))->get_deviceDisconnected_1();
		V_2 = (bool)((!(((RuntimeObject*)(Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 *)L_7) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_8 = V_2;
		if (!L_8)
		{
			goto IL_0055;
		}
	}
	{
		Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * L_9 = ((InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_StaticFields*)il2cpp_codegen_static_fields_for(InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_il2cpp_TypeInfo_var))->get_deviceDisconnected_1();
		uint64_t L_10 = ___deviceId0;
		InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E  L_11;
		memset((&L_11), 0, sizeof(L_11));
		InputDevice__ctor_m610666CA01BCAF92464BE9C17BFB133A23A663BE((&L_11), L_10, /*hidden argument*/NULL);
		NullCheck(L_9);
		Action_1_Invoke_mA460B359B13BDCF71CDDE9A8A960C827A4E3E970(L_9, L_11, /*hidden argument*/Action_1_Invoke_mA460B359B13BDCF71CDDE9A8A960C827A4E3E970_RuntimeMethod_var);
	}

IL_0055:
	{
		goto IL_0077;
	}

IL_0057:
	{
		Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * L_12 = ((InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_StaticFields*)il2cpp_codegen_static_fields_for(InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_il2cpp_TypeInfo_var))->get_deviceConfigChanged_2();
		V_3 = (bool)((!(((RuntimeObject*)(Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 *)L_12) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_13 = V_3;
		if (!L_13)
		{
			goto IL_0075;
		}
	}
	{
		Action_1_tD14DA73DE0FBEFB24671F37EB0148705E00E11E8 * L_14 = ((InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_StaticFields*)il2cpp_codegen_static_fields_for(InputDevices_t50F530D78AE16C2F160416FBAE9BC04024C448CC_il2cpp_TypeInfo_var))->get_deviceConfigChanged_2();
		uint64_t L_15 = ___deviceId0;
		InputDevice_t69B790C68145C769BA3819DE33AA94155C77207E  L_16;
		memset((&L_16), 0, sizeof(L_16));
		InputDevice__ctor_m610666CA01BCAF92464BE9C17BFB133A23A663BE((&L_16), L_15, /*hidden argument*/NULL);
		NullCheck(L_14);
		Action_1_Invoke_mA460B359B13BDCF71CDDE9A8A960C827A4E3E970(L_14, L_16, /*hidden argument*/Action_1_Invoke_mA460B359B13BDCF71CDDE9A8A960C827A4E3E970_RuntimeMethod_var);
	}

IL_0075:
	{
		goto IL_0077;
	}

IL_0077:
	{
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
// Conversion methods for marshalling of: UnityEngine.XR.InputFeatureUsage
IL2CPP_EXTERN_C void InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_marshal_pinvoke(const InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE& unmarshaled, InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_marshaled_pinvoke& marshaled)
{
	marshaled.___m_Name_0 = il2cpp_codegen_marshal_string(unmarshaled.get_m_Name_0());
	marshaled.___m_InternalType_1 = unmarshaled.get_m_InternalType_1();
}
IL2CPP_EXTERN_C void InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_marshal_pinvoke_back(const InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_marshaled_pinvoke& marshaled, InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE& unmarshaled)
{
	unmarshaled.set_m_Name_0(il2cpp_codegen_marshal_string_result(marshaled.___m_Name_0));
	uint32_t unmarshaled_m_InternalType_temp_1 = 0;
	unmarshaled_m_InternalType_temp_1 = marshaled.___m_InternalType_1;
	unmarshaled.set_m_InternalType_1(unmarshaled_m_InternalType_temp_1);
}
// Conversion method for clean up from marshalling of: UnityEngine.XR.InputFeatureUsage
IL2CPP_EXTERN_C void InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_marshal_pinvoke_cleanup(InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_marshaled_pinvoke& marshaled)
{
	il2cpp_codegen_marshal_free(marshaled.___m_Name_0);
	marshaled.___m_Name_0 = NULL;
}
// Conversion methods for marshalling of: UnityEngine.XR.InputFeatureUsage
IL2CPP_EXTERN_C void InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_marshal_com(const InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE& unmarshaled, InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_marshaled_com& marshaled)
{
	marshaled.___m_Name_0 = il2cpp_codegen_marshal_bstring(unmarshaled.get_m_Name_0());
	marshaled.___m_InternalType_1 = unmarshaled.get_m_InternalType_1();
}
IL2CPP_EXTERN_C void InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_marshal_com_back(const InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_marshaled_com& marshaled, InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE& unmarshaled)
{
	unmarshaled.set_m_Name_0(il2cpp_codegen_marshal_bstring_result(marshaled.___m_Name_0));
	uint32_t unmarshaled_m_InternalType_temp_1 = 0;
	unmarshaled_m_InternalType_temp_1 = marshaled.___m_InternalType_1;
	unmarshaled.set_m_InternalType_1(unmarshaled_m_InternalType_temp_1);
}
// Conversion method for clean up from marshalling of: UnityEngine.XR.InputFeatureUsage
IL2CPP_EXTERN_C void InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_marshal_com_cleanup(InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_marshaled_com& marshaled)
{
	il2cpp_codegen_marshal_free_bstring(marshaled.___m_Name_0);
	marshaled.___m_Name_0 = NULL;
}
// System.String UnityEngine.XR.InputFeatureUsage::get_name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* InputFeatureUsage_get_name_m08FB0101027B503D080EE38F4273BB61C59E3988 (InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * __this, const RuntimeMethod* method)
{
	String_t* V_0 = NULL;
	{
		String_t* L_0 = __this->get_m_Name_0();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		String_t* L_1 = V_0;
		return L_1;
	}
}
IL2CPP_EXTERN_C  String_t* InputFeatureUsage_get_name_m08FB0101027B503D080EE38F4273BB61C59E3988_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * _thisAdjusted = reinterpret_cast<InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE *>(__this + _offset);
	return InputFeatureUsage_get_name_m08FB0101027B503D080EE38F4273BB61C59E3988(_thisAdjusted, method);
}
// UnityEngine.XR.InputFeatureType UnityEngine.XR.InputFeatureUsage::get_internalType()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t InputFeatureUsage_get_internalType_m55D87EBFF5CA6781AA831A3FAAA75BD80A13B065 (InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * __this, const RuntimeMethod* method)
{
	uint32_t V_0 = 0;
	{
		uint32_t L_0 = __this->get_m_InternalType_1();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		uint32_t L_1 = V_0;
		return L_1;
	}
}
IL2CPP_EXTERN_C  uint32_t InputFeatureUsage_get_internalType_m55D87EBFF5CA6781AA831A3FAAA75BD80A13B065_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * _thisAdjusted = reinterpret_cast<InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE *>(__this + _offset);
	return InputFeatureUsage_get_internalType_m55D87EBFF5CA6781AA831A3FAAA75BD80A13B065(_thisAdjusted, method);
}
// System.Boolean UnityEngine.XR.InputFeatureUsage::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool InputFeatureUsage_Equals_mC0A1A665A98F42B2D5896BB9BC4CBA42FB59D582 (InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (InputFeatureUsage_Equals_mC0A1A665A98F42B2D5896BB9BC4CBA42FB59D582_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	bool V_1 = false;
	{
		RuntimeObject * L_0 = ___obj0;
		V_0 = (bool)((((int32_t)((!(((RuntimeObject*)(RuntimeObject *)((RuntimeObject *)IsInstSealed((RuntimeObject*)L_0, InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_il2cpp_TypeInfo_var))) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_1 = V_0;
		if (!L_1)
		{
			goto IL_0015;
		}
	}
	{
		V_1 = (bool)0;
		goto IL_0024;
	}

IL_0015:
	{
		RuntimeObject * L_2 = ___obj0;
		bool L_3 = InputFeatureUsage_Equals_mD7107D9A754188766EACB7FAAF015E184FE706B9((InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE *)__this, ((*(InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE *)((InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE *)UnBox(L_2, InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE_il2cpp_TypeInfo_var)))), /*hidden argument*/NULL);
		V_1 = L_3;
		goto IL_0024;
	}

IL_0024:
	{
		bool L_4 = V_1;
		return L_4;
	}
}
IL2CPP_EXTERN_C  bool InputFeatureUsage_Equals_mC0A1A665A98F42B2D5896BB9BC4CBA42FB59D582_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * _thisAdjusted = reinterpret_cast<InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE *>(__this + _offset);
	return InputFeatureUsage_Equals_mC0A1A665A98F42B2D5896BB9BC4CBA42FB59D582(_thisAdjusted, ___obj0, method);
}
// System.Boolean UnityEngine.XR.InputFeatureUsage::Equals(UnityEngine.XR.InputFeatureUsage)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool InputFeatureUsage_Equals_mD7107D9A754188766EACB7FAAF015E184FE706B9 (InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * __this, InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE  ___other0, const RuntimeMethod* method)
{
	bool V_0 = false;
	int32_t G_B3_0 = 0;
	{
		String_t* L_0 = InputFeatureUsage_get_name_m08FB0101027B503D080EE38F4273BB61C59E3988((InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE *)__this, /*hidden argument*/NULL);
		String_t* L_1 = InputFeatureUsage_get_name_m08FB0101027B503D080EE38F4273BB61C59E3988((InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE *)(&___other0), /*hidden argument*/NULL);
		bool L_2 = String_op_Equality_m2B91EE68355F142F67095973D32EB5828B7B73CB(L_0, L_1, /*hidden argument*/NULL);
		if (!L_2)
		{
			goto IL_0026;
		}
	}
	{
		uint32_t L_3 = InputFeatureUsage_get_internalType_m55D87EBFF5CA6781AA831A3FAAA75BD80A13B065((InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE *)__this, /*hidden argument*/NULL);
		uint32_t L_4 = InputFeatureUsage_get_internalType_m55D87EBFF5CA6781AA831A3FAAA75BD80A13B065((InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE *)(&___other0), /*hidden argument*/NULL);
		G_B3_0 = ((((int32_t)L_3) == ((int32_t)L_4))? 1 : 0);
		goto IL_0027;
	}

IL_0026:
	{
		G_B3_0 = 0;
	}

IL_0027:
	{
		V_0 = (bool)G_B3_0;
		goto IL_002a;
	}

IL_002a:
	{
		bool L_5 = V_0;
		return L_5;
	}
}
IL2CPP_EXTERN_C  bool InputFeatureUsage_Equals_mD7107D9A754188766EACB7FAAF015E184FE706B9_AdjustorThunk (RuntimeObject * __this, InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * _thisAdjusted = reinterpret_cast<InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE *>(__this + _offset);
	return InputFeatureUsage_Equals_mD7107D9A754188766EACB7FAAF015E184FE706B9(_thisAdjusted, ___other0, method);
}
// System.Int32 UnityEngine.XR.InputFeatureUsage::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t InputFeatureUsage_GetHashCode_m08673D24DA7804D87915443A647AA65447C511D1 (InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * __this, const RuntimeMethod* method)
{
	uint32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		String_t* L_0 = InputFeatureUsage_get_name_m08FB0101027B503D080EE38F4273BB61C59E3988((InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE *)__this, /*hidden argument*/NULL);
		NullCheck(L_0);
		int32_t L_1 = VirtFuncInvoker0< int32_t >::Invoke(2 /* System.Int32 System.Object::GetHashCode() */, L_0);
		uint32_t L_2 = InputFeatureUsage_get_internalType_m55D87EBFF5CA6781AA831A3FAAA75BD80A13B065((InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE *)__this, /*hidden argument*/NULL);
		V_0 = L_2;
		int32_t L_3 = UInt32_GetHashCode_m60E3A243F3D79311A64836148AE1AC23C679FC45((uint32_t*)(&V_0), /*hidden argument*/NULL);
		V_1 = ((int32_t)((int32_t)L_1^(int32_t)((int32_t)((int32_t)L_3<<(int32_t)1))));
		goto IL_0026;
	}

IL_0026:
	{
		int32_t L_4 = V_1;
		return L_4;
	}
}
IL2CPP_EXTERN_C  int32_t InputFeatureUsage_GetHashCode_m08673D24DA7804D87915443A647AA65447C511D1_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE * _thisAdjusted = reinterpret_cast<InputFeatureUsage_tB971D811B38B1DA549F529BB15E60672940FB0EE *>(__this + _offset);
	return InputFeatureUsage_GetHashCode_m08673D24DA7804D87915443A647AA65447C511D1(_thisAdjusted, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.XR.InputTracking::InvokeTrackingEvent(UnityEngine.XR.InputTracking_TrackingStateEventType,UnityEngine.XR.XRNode,System.Int64,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void InputTracking_InvokeTrackingEvent_mF9CC9853D284F640ACEB29225EF35646166061A0 (int32_t ___eventType0, int32_t ___nodeType1, int64_t ___uniqueID2, bool ___tracked3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (InputTracking_InvokeTrackingEvent_mF9CC9853D284F640ACEB29225EF35646166061A0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * V_0 = NULL;
	XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33  V_1;
	memset((&V_1), 0, sizeof(V_1));
	int32_t V_2 = 0;
	bool V_3 = false;
	{
		V_0 = (Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 *)NULL;
		il2cpp_codegen_initobj((&V_1), sizeof(XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 ));
		int64_t L_0 = ___uniqueID2;
		XRNodeState_set_uniqueID_m09D6E80AD1B81D7485242CFA27A7173A6231CA87((XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 *)(&V_1), L_0, /*hidden argument*/NULL);
		int32_t L_1 = ___nodeType1;
		XRNodeState_set_nodeType_m9ADECAA5D665042FCA2F8E81726AFD1A1FA8B30D((XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 *)(&V_1), L_1, /*hidden argument*/NULL);
		bool L_2 = ___tracked3;
		XRNodeState_set_tracked_m27DDD443D10F5F43B5B9AA83BFE901DC12316B9C((XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 *)(&V_1), L_2, /*hidden argument*/NULL);
		int32_t L_3 = ___eventType0;
		V_2 = L_3;
		int32_t L_4 = V_2;
		switch (L_4)
		{
			case 0:
			{
				goto IL_0050;
			}
			case 1:
			{
				goto IL_0058;
			}
			case 2:
			{
				goto IL_0040;
			}
			case 3:
			{
				goto IL_0048;
			}
		}
	}
	{
		goto IL_0060;
	}

IL_0040:
	{
		IL2CPP_RUNTIME_CLASS_INIT(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_il2cpp_TypeInfo_var);
		Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * L_5 = ((InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_StaticFields*)il2cpp_codegen_static_fields_for(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_il2cpp_TypeInfo_var))->get_trackingAcquired_0();
		V_0 = L_5;
		goto IL_0076;
	}

IL_0048:
	{
		IL2CPP_RUNTIME_CLASS_INIT(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_il2cpp_TypeInfo_var);
		Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * L_6 = ((InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_StaticFields*)il2cpp_codegen_static_fields_for(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_il2cpp_TypeInfo_var))->get_trackingLost_1();
		V_0 = L_6;
		goto IL_0076;
	}

IL_0050:
	{
		IL2CPP_RUNTIME_CLASS_INIT(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_il2cpp_TypeInfo_var);
		Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * L_7 = ((InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_StaticFields*)il2cpp_codegen_static_fields_for(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_il2cpp_TypeInfo_var))->get_nodeAdded_2();
		V_0 = L_7;
		goto IL_0076;
	}

IL_0058:
	{
		IL2CPP_RUNTIME_CLASS_INIT(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_il2cpp_TypeInfo_var);
		Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * L_8 = ((InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_StaticFields*)il2cpp_codegen_static_fields_for(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_il2cpp_TypeInfo_var))->get_nodeRemoved_3();
		V_0 = L_8;
		goto IL_0076;
	}

IL_0060:
	{
		int32_t L_9 = ___eventType0;
		int32_t L_10 = L_9;
		RuntimeObject * L_11 = Box(TrackingStateEventType_t301E0DD44D089E06B0BBA994F682CE9F23505BA5_il2cpp_TypeInfo_var, &L_10);
		String_t* L_12 = String_Concat_m4D0DDA7FEDB75304E5FDAF8489A0478EE58A45F2(_stringLiteralF953F17BB91EBF78300169DEE55CE060B4F1C569, L_11, /*hidden argument*/NULL);
		ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 * L_13 = (ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 *)il2cpp_codegen_object_new(ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00_il2cpp_TypeInfo_var);
		ArgumentException__ctor_m2D35EAD113C2ADC99EB17B940A2097A93FD23EFC(L_13, L_12, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_13, InputTracking_InvokeTrackingEvent_mF9CC9853D284F640ACEB29225EF35646166061A0_RuntimeMethod_var);
	}

IL_0076:
	{
		Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * L_14 = V_0;
		V_3 = (bool)((!(((RuntimeObject*)(Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 *)L_14) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_15 = V_3;
		if (!L_15)
		{
			goto IL_0088;
		}
	}
	{
		Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 * L_16 = V_0;
		XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33  L_17 = V_1;
		NullCheck(L_16);
		Action_1_Invoke_mD7440CB91FE64B4EAD0D34248075E0F39797C946(L_16, L_17, /*hidden argument*/Action_1_Invoke_mD7440CB91FE64B4EAD0D34248075E0F39797C946_RuntimeMethod_var);
	}

IL_0088:
	{
		return;
	}
}
// System.Void UnityEngine.XR.InputTracking::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void InputTracking__cctor_m8C342AE21A5D67A0378CE91016DBCCFFC62E34ED (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (InputTracking__cctor_m8C342AE21A5D67A0378CE91016DBCCFFC62E34ED_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		((InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_StaticFields*)il2cpp_codegen_static_fields_for(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_il2cpp_TypeInfo_var))->set_trackingAcquired_0((Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 *)NULL);
		((InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_StaticFields*)il2cpp_codegen_static_fields_for(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_il2cpp_TypeInfo_var))->set_trackingLost_1((Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 *)NULL);
		((InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_StaticFields*)il2cpp_codegen_static_fields_for(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_il2cpp_TypeInfo_var))->set_nodeAdded_2((Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 *)NULL);
		((InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_StaticFields*)il2cpp_codegen_static_fields_for(InputTracking_t2CCE92D4A5FE0AEBC14996566D93ED4B08F4CB6D_il2cpp_TypeInfo_var))->set_nodeRemoved_3((Action_1_t016EBE9560F0A12616F6E8C2FB15578C134D1603 *)NULL);
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
// Conversion methods for marshalling of: UnityEngine.XR.MeshGenerationResult
IL2CPP_EXTERN_C void MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_marshal_pinvoke(const MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF& unmarshaled, MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_marshaled_pinvoke& marshaled)
{
	Exception_t* ___U3CMeshU3Ek__BackingField_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field '<Mesh>k__BackingField' of type 'MeshGenerationResult': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___U3CMeshU3Ek__BackingField_1Exception, NULL);
}
IL2CPP_EXTERN_C void MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_marshal_pinvoke_back(const MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_marshaled_pinvoke& marshaled, MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF& unmarshaled)
{
	Exception_t* ___U3CMeshU3Ek__BackingField_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field '<Mesh>k__BackingField' of type 'MeshGenerationResult': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___U3CMeshU3Ek__BackingField_1Exception, NULL);
}
// Conversion method for clean up from marshalling of: UnityEngine.XR.MeshGenerationResult
IL2CPP_EXTERN_C void MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_marshal_pinvoke_cleanup(MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: UnityEngine.XR.MeshGenerationResult
IL2CPP_EXTERN_C void MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_marshal_com(const MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF& unmarshaled, MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_marshaled_com& marshaled)
{
	Exception_t* ___U3CMeshU3Ek__BackingField_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field '<Mesh>k__BackingField' of type 'MeshGenerationResult': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___U3CMeshU3Ek__BackingField_1Exception, NULL);
}
IL2CPP_EXTERN_C void MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_marshal_com_back(const MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_marshaled_com& marshaled, MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF& unmarshaled)
{
	Exception_t* ___U3CMeshU3Ek__BackingField_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field '<Mesh>k__BackingField' of type 'MeshGenerationResult': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___U3CMeshU3Ek__BackingField_1Exception, NULL);
}
// Conversion method for clean up from marshalling of: UnityEngine.XR.MeshGenerationResult
IL2CPP_EXTERN_C void MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_marshal_com_cleanup(MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_marshaled_com& marshaled)
{
}
// UnityEngine.XR.MeshId UnityEngine.XR.MeshGenerationResult::get_MeshId()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  MeshGenerationResult_get_MeshId_m55663E958B980188CFD406BB2469B24D8089BA9E (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method)
{
	{
		MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  L_0 = __this->get_U3CMeshIdU3Ek__BackingField_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C  MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  MeshGenerationResult_get_MeshId_m55663E958B980188CFD406BB2469B24D8089BA9E_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * _thisAdjusted = reinterpret_cast<MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *>(__this + _offset);
	return MeshGenerationResult_get_MeshId_m55663E958B980188CFD406BB2469B24D8089BA9E_inline(_thisAdjusted, method);
}
// UnityEngine.Mesh UnityEngine.XR.MeshGenerationResult::get_Mesh()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 * MeshGenerationResult_get_Mesh_mDBEB5E83FC729856B17AF62B9438C9B9A79A0200 (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method)
{
	{
		Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 * L_0 = __this->get_U3CMeshU3Ek__BackingField_1();
		return L_0;
	}
}
IL2CPP_EXTERN_C  Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 * MeshGenerationResult_get_Mesh_mDBEB5E83FC729856B17AF62B9438C9B9A79A0200_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * _thisAdjusted = reinterpret_cast<MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *>(__this + _offset);
	return MeshGenerationResult_get_Mesh_mDBEB5E83FC729856B17AF62B9438C9B9A79A0200_inline(_thisAdjusted, method);
}
// UnityEngine.MeshCollider UnityEngine.XR.MeshGenerationResult::get_MeshCollider()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 * MeshGenerationResult_get_MeshCollider_m0285F3BFFFF0778DC8DDA97BFDAE30A19AEBF283 (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method)
{
	{
		MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 * L_0 = __this->get_U3CMeshColliderU3Ek__BackingField_2();
		return L_0;
	}
}
IL2CPP_EXTERN_C  MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 * MeshGenerationResult_get_MeshCollider_m0285F3BFFFF0778DC8DDA97BFDAE30A19AEBF283_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * _thisAdjusted = reinterpret_cast<MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *>(__this + _offset);
	return MeshGenerationResult_get_MeshCollider_m0285F3BFFFF0778DC8DDA97BFDAE30A19AEBF283_inline(_thisAdjusted, method);
}
// UnityEngine.XR.MeshGenerationStatus UnityEngine.XR.MeshGenerationResult::get_Status()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t MeshGenerationResult_get_Status_m5AF51D2745EC947BB722550DC95665D430CDB178 (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->get_U3CStatusU3Ek__BackingField_3();
		return L_0;
	}
}
IL2CPP_EXTERN_C  int32_t MeshGenerationResult_get_Status_m5AF51D2745EC947BB722550DC95665D430CDB178_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * _thisAdjusted = reinterpret_cast<MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *>(__this + _offset);
	return MeshGenerationResult_get_Status_m5AF51D2745EC947BB722550DC95665D430CDB178_inline(_thisAdjusted, method);
}
// UnityEngine.XR.MeshVertexAttributes UnityEngine.XR.MeshGenerationResult::get_Attributes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t MeshGenerationResult_get_Attributes_m24D66B0694F827D4DFFE210069EF0F5578F4845F (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->get_U3CAttributesU3Ek__BackingField_4();
		return L_0;
	}
}
IL2CPP_EXTERN_C  int32_t MeshGenerationResult_get_Attributes_m24D66B0694F827D4DFFE210069EF0F5578F4845F_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * _thisAdjusted = reinterpret_cast<MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *>(__this + _offset);
	return MeshGenerationResult_get_Attributes_m24D66B0694F827D4DFFE210069EF0F5578F4845F_inline(_thisAdjusted, method);
}
// System.Boolean UnityEngine.XR.MeshGenerationResult::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MeshGenerationResult_Equals_m511B6FD46B1187D90919F4C0D2E853DE4A16BD44 (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (MeshGenerationResult_Equals_m511B6FD46B1187D90919F4C0D2E853DE4A16BD44_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	bool V_1 = false;
	{
		RuntimeObject * L_0 = ___obj0;
		V_0 = (bool)((((int32_t)((!(((RuntimeObject*)(RuntimeObject *)((RuntimeObject *)IsInstSealed((RuntimeObject*)L_0, MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_il2cpp_TypeInfo_var))) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_1 = V_0;
		if (!L_1)
		{
			goto IL_0015;
		}
	}
	{
		V_1 = (bool)0;
		goto IL_0024;
	}

IL_0015:
	{
		RuntimeObject * L_2 = ___obj0;
		bool L_3 = MeshGenerationResult_Equals_mA9685353D67F169013BF8CC6B34EAFE69DB341B4((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)__this, ((*(MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)UnBox(L_2, MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF_il2cpp_TypeInfo_var)))), /*hidden argument*/NULL);
		V_1 = L_3;
		goto IL_0024;
	}

IL_0024:
	{
		bool L_4 = V_1;
		return L_4;
	}
}
IL2CPP_EXTERN_C  bool MeshGenerationResult_Equals_m511B6FD46B1187D90919F4C0D2E853DE4A16BD44_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * _thisAdjusted = reinterpret_cast<MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *>(__this + _offset);
	return MeshGenerationResult_Equals_m511B6FD46B1187D90919F4C0D2E853DE4A16BD44(_thisAdjusted, ___obj0, method);
}
// System.Boolean UnityEngine.XR.MeshGenerationResult::Equals(UnityEngine.XR.MeshGenerationResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MeshGenerationResult_Equals_mA9685353D67F169013BF8CC6B34EAFE69DB341B4 (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF  ___other0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (MeshGenerationResult_Equals_mA9685353D67F169013BF8CC6B34EAFE69DB341B4_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  V_0;
	memset((&V_0), 0, sizeof(V_0));
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	bool V_3 = false;
	int32_t G_B6_0 = 0;
	{
		MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  L_0 = MeshGenerationResult_get_MeshId_m55663E958B980188CFD406BB2469B24D8089BA9E_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)__this, /*hidden argument*/NULL);
		V_0 = L_0;
		MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  L_1 = MeshGenerationResult_get_MeshId_m55663E958B980188CFD406BB2469B24D8089BA9E_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)(&___other0), /*hidden argument*/NULL);
		bool L_2 = MeshId_Equals_m685A94F74A3A06E6E51C60F1D40386CAA8F01834((MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 *)(&V_0), L_1, /*hidden argument*/NULL);
		if (!L_2)
		{
			goto IL_0084;
		}
	}
	{
		Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 * L_3 = MeshGenerationResult_get_Mesh_mDBEB5E83FC729856B17AF62B9438C9B9A79A0200_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)__this, /*hidden argument*/NULL);
		Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 * L_4 = MeshGenerationResult_get_Mesh_mDBEB5E83FC729856B17AF62B9438C9B9A79A0200_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)(&___other0), /*hidden argument*/NULL);
		NullCheck(L_3);
		bool L_5 = VirtFuncInvoker1< bool, RuntimeObject * >::Invoke(0 /* System.Boolean System.Object::Equals(System.Object) */, L_3, L_4);
		if (!L_5)
		{
			goto IL_0084;
		}
	}
	{
		MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 * L_6 = MeshGenerationResult_get_MeshCollider_m0285F3BFFFF0778DC8DDA97BFDAE30A19AEBF283_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)__this, /*hidden argument*/NULL);
		MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 * L_7 = MeshGenerationResult_get_MeshCollider_m0285F3BFFFF0778DC8DDA97BFDAE30A19AEBF283_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)(&___other0), /*hidden argument*/NULL);
		NullCheck(L_6);
		bool L_8 = VirtFuncInvoker1< bool, RuntimeObject * >::Invoke(0 /* System.Boolean System.Object::Equals(System.Object) */, L_6, L_7);
		if (!L_8)
		{
			goto IL_0084;
		}
	}
	{
		int32_t L_9 = MeshGenerationResult_get_Status_m5AF51D2745EC947BB722550DC95665D430CDB178_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)__this, /*hidden argument*/NULL);
		V_1 = L_9;
		int32_t L_10 = MeshGenerationResult_get_Status_m5AF51D2745EC947BB722550DC95665D430CDB178_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)(&___other0), /*hidden argument*/NULL);
		int32_t L_11 = L_10;
		RuntimeObject * L_12 = Box(MeshGenerationStatus_t25EB712EAD94A279AD7D5A00E0CB6EDC8AB1FE79_il2cpp_TypeInfo_var, &L_11);
		RuntimeObject * L_13 = Box(MeshGenerationStatus_t25EB712EAD94A279AD7D5A00E0CB6EDC8AB1FE79_il2cpp_TypeInfo_var, (&V_1));
		NullCheck(L_13);
		bool L_14 = VirtFuncInvoker1< bool, RuntimeObject * >::Invoke(0 /* System.Boolean System.Object::Equals(System.Object) */, L_13, L_12);
		V_1 = *(int32_t*)UnBox(L_13);
		if (!L_14)
		{
			goto IL_0084;
		}
	}
	{
		int32_t L_15 = MeshGenerationResult_get_Attributes_m24D66B0694F827D4DFFE210069EF0F5578F4845F_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)__this, /*hidden argument*/NULL);
		V_2 = L_15;
		int32_t L_16 = MeshGenerationResult_get_Attributes_m24D66B0694F827D4DFFE210069EF0F5578F4845F_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)(&___other0), /*hidden argument*/NULL);
		int32_t L_17 = L_16;
		RuntimeObject * L_18 = Box(MeshVertexAttributes_t7CCF6BE6BB4E908E1ECF9F9AF76968FA38A672CE_il2cpp_TypeInfo_var, &L_17);
		RuntimeObject * L_19 = Box(MeshVertexAttributes_t7CCF6BE6BB4E908E1ECF9F9AF76968FA38A672CE_il2cpp_TypeInfo_var, (&V_2));
		NullCheck(L_19);
		bool L_20 = VirtFuncInvoker1< bool, RuntimeObject * >::Invoke(0 /* System.Boolean System.Object::Equals(System.Object) */, L_19, L_18);
		V_2 = *(int32_t*)UnBox(L_19);
		G_B6_0 = ((int32_t)(L_20));
		goto IL_0085;
	}

IL_0084:
	{
		G_B6_0 = 0;
	}

IL_0085:
	{
		V_3 = (bool)G_B6_0;
		goto IL_0088;
	}

IL_0088:
	{
		bool L_21 = V_3;
		return L_21;
	}
}
IL2CPP_EXTERN_C  bool MeshGenerationResult_Equals_mA9685353D67F169013BF8CC6B34EAFE69DB341B4_AdjustorThunk (RuntimeObject * __this, MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * _thisAdjusted = reinterpret_cast<MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *>(__this + _offset);
	return MeshGenerationResult_Equals_mA9685353D67F169013BF8CC6B34EAFE69DB341B4(_thisAdjusted, ___other0, method);
}
// System.Int32 UnityEngine.XR.MeshGenerationResult::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t MeshGenerationResult_GetHashCode_m511BFBE4F21B162C59E462DF9DC9883A06CF9CC7 (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method)
{
	MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  V_0;
	memset((&V_0), 0, sizeof(V_0));
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  L_0 = MeshGenerationResult_get_MeshId_m55663E958B980188CFD406BB2469B24D8089BA9E_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)__this, /*hidden argument*/NULL);
		V_0 = L_0;
		int32_t L_1 = MeshId_GetHashCode_mCD3E4355DB5EE90C389CE1D742E4CCA6471E2AA6((MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 *)(&V_0), /*hidden argument*/NULL);
		Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 * L_2 = MeshGenerationResult_get_Mesh_mDBEB5E83FC729856B17AF62B9438C9B9A79A0200_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)__this, /*hidden argument*/NULL);
		NullCheck(L_2);
		int32_t L_3 = VirtFuncInvoker0< int32_t >::Invoke(2 /* System.Int32 System.Object::GetHashCode() */, L_2);
		int32_t L_4 = HashCodeHelper_Combine_m5F8B051AFC305B7FD377632031048F6549A1A543(L_1, L_3, /*hidden argument*/NULL);
		MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 * L_5 = MeshGenerationResult_get_MeshCollider_m0285F3BFFFF0778DC8DDA97BFDAE30A19AEBF283_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)__this, /*hidden argument*/NULL);
		NullCheck(L_5);
		int32_t L_6 = VirtFuncInvoker0< int32_t >::Invoke(2 /* System.Int32 System.Object::GetHashCode() */, L_5);
		int32_t L_7 = HashCodeHelper_Combine_m5F8B051AFC305B7FD377632031048F6549A1A543(L_4, L_6, /*hidden argument*/NULL);
		int32_t L_8 = MeshGenerationResult_get_Status_m5AF51D2745EC947BB722550DC95665D430CDB178_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)__this, /*hidden argument*/NULL);
		V_1 = L_8;
		int32_t L_9 = Int32_GetHashCode_mEDD3F492A5F7CF021125AE3F38E2B8F8743FC667((int32_t*)(&V_1), /*hidden argument*/NULL);
		int32_t L_10 = HashCodeHelper_Combine_m5F8B051AFC305B7FD377632031048F6549A1A543(L_7, L_9, /*hidden argument*/NULL);
		int32_t L_11 = MeshGenerationResult_get_Attributes_m24D66B0694F827D4DFFE210069EF0F5578F4845F_inline((MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *)__this, /*hidden argument*/NULL);
		V_2 = L_11;
		int32_t L_12 = Int32_GetHashCode_mEDD3F492A5F7CF021125AE3F38E2B8F8743FC667((int32_t*)(&V_2), /*hidden argument*/NULL);
		int32_t L_13 = HashCodeHelper_Combine_m5F8B051AFC305B7FD377632031048F6549A1A543(L_10, L_12, /*hidden argument*/NULL);
		V_3 = L_13;
		goto IL_006a;
	}

IL_006a:
	{
		int32_t L_14 = V_3;
		return L_14;
	}
}
IL2CPP_EXTERN_C  int32_t MeshGenerationResult_GetHashCode_m511BFBE4F21B162C59E462DF9DC9883A06CF9CC7_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * _thisAdjusted = reinterpret_cast<MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF *>(__this + _offset);
	return MeshGenerationResult_GetHashCode_m511BFBE4F21B162C59E462DF9DC9883A06CF9CC7(_thisAdjusted, method);
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
// System.String UnityEngine.XR.MeshId::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* MeshId_ToString_mA9CDBA01DD0C110252F6C4AA7437C507B2025705 (MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (MeshId_ToString_mA9CDBA01DD0C110252F6C4AA7437C507B2025705_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	String_t* V_0 = NULL;
	{
		uint64_t* L_0 = __this->get_address_of_m_SubId1_1();
		String_t* L_1 = UInt64_ToString_mFE48F1D174A1F741AB0795C1164BF45BF37F86E6((uint64_t*)L_0, _stringLiteralFDA1C52D0E58360F4E8FD608757CCD98D8772D4F, /*hidden argument*/NULL);
		uint64_t* L_2 = __this->get_address_of_m_SubId2_2();
		String_t* L_3 = UInt64_ToString_mFE48F1D174A1F741AB0795C1164BF45BF37F86E6((uint64_t*)L_2, _stringLiteralFDA1C52D0E58360F4E8FD608757CCD98D8772D4F, /*hidden argument*/NULL);
		String_t* L_4 = String_Format_m8D1CB0410C35E052A53AE957C914C841E54BAB66(_stringLiteralFBAF124AB08242B7785EC2B6DBC3C6459CB98BC8, L_1, L_3, /*hidden argument*/NULL);
		V_0 = L_4;
		goto IL_002e;
	}

IL_002e:
	{
		String_t* L_5 = V_0;
		return L_5;
	}
}
IL2CPP_EXTERN_C  String_t* MeshId_ToString_mA9CDBA01DD0C110252F6C4AA7437C507B2025705_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 * _thisAdjusted = reinterpret_cast<MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 *>(__this + _offset);
	return MeshId_ToString_mA9CDBA01DD0C110252F6C4AA7437C507B2025705(_thisAdjusted, method);
}
// System.Int32 UnityEngine.XR.MeshId::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t MeshId_GetHashCode_mCD3E4355DB5EE90C389CE1D742E4CCA6471E2AA6 (MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		uint64_t* L_0 = __this->get_address_of_m_SubId1_1();
		int32_t L_1 = UInt64_GetHashCode_mCDF662897A3F02CED11A9F9E66C5BF4E28C02B33((uint64_t*)L_0, /*hidden argument*/NULL);
		uint64_t* L_2 = __this->get_address_of_m_SubId2_2();
		int32_t L_3 = UInt64_GetHashCode_mCDF662897A3F02CED11A9F9E66C5BF4E28C02B33((uint64_t*)L_2, /*hidden argument*/NULL);
		V_0 = ((int32_t)((int32_t)L_1^(int32_t)L_3));
		goto IL_001b;
	}

IL_001b:
	{
		int32_t L_4 = V_0;
		return L_4;
	}
}
IL2CPP_EXTERN_C  int32_t MeshId_GetHashCode_mCD3E4355DB5EE90C389CE1D742E4CCA6471E2AA6_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 * _thisAdjusted = reinterpret_cast<MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 *>(__this + _offset);
	return MeshId_GetHashCode_mCD3E4355DB5EE90C389CE1D742E4CCA6471E2AA6(_thisAdjusted, method);
}
// System.Boolean UnityEngine.XR.MeshId::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MeshId_Equals_m77D4535F7643D5C1FEA20600C92B73818DD8675E (MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (MeshId_Equals_m77D4535F7643D5C1FEA20600C92B73818DD8675E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	int32_t G_B3_0 = 0;
	{
		RuntimeObject * L_0 = ___obj0;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_0, MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767_il2cpp_TypeInfo_var)))
		{
			goto IL_0017;
		}
	}
	{
		RuntimeObject * L_1 = ___obj0;
		bool L_2 = MeshId_Equals_m685A94F74A3A06E6E51C60F1D40386CAA8F01834((MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 *)__this, ((*(MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 *)((MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 *)UnBox(L_1, MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767_il2cpp_TypeInfo_var)))), /*hidden argument*/NULL);
		G_B3_0 = ((int32_t)(L_2));
		goto IL_0018;
	}

IL_0017:
	{
		G_B3_0 = 0;
	}

IL_0018:
	{
		V_0 = (bool)G_B3_0;
		goto IL_001b;
	}

IL_001b:
	{
		bool L_3 = V_0;
		return L_3;
	}
}
IL2CPP_EXTERN_C  bool MeshId_Equals_m77D4535F7643D5C1FEA20600C92B73818DD8675E_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 * _thisAdjusted = reinterpret_cast<MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 *>(__this + _offset);
	return MeshId_Equals_m77D4535F7643D5C1FEA20600C92B73818DD8675E(_thisAdjusted, ___obj0, method);
}
// System.Boolean UnityEngine.XR.MeshId::Equals(UnityEngine.XR.MeshId)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MeshId_Equals_m685A94F74A3A06E6E51C60F1D40386CAA8F01834 (MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 * __this, MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  ___other0, const RuntimeMethod* method)
{
	bool V_0 = false;
	int32_t G_B3_0 = 0;
	{
		uint64_t L_0 = __this->get_m_SubId1_1();
		MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  L_1 = ___other0;
		uint64_t L_2 = L_1.get_m_SubId1_1();
		if ((!(((uint64_t)L_0) == ((uint64_t)L_2))))
		{
			goto IL_001f;
		}
	}
	{
		uint64_t L_3 = __this->get_m_SubId2_2();
		MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  L_4 = ___other0;
		uint64_t L_5 = L_4.get_m_SubId2_2();
		G_B3_0 = ((((int64_t)L_3) == ((int64_t)L_5))? 1 : 0);
		goto IL_0020;
	}

IL_001f:
	{
		G_B3_0 = 0;
	}

IL_0020:
	{
		V_0 = (bool)G_B3_0;
		goto IL_0023;
	}

IL_0023:
	{
		bool L_6 = V_0;
		return L_6;
	}
}
IL2CPP_EXTERN_C  bool MeshId_Equals_m685A94F74A3A06E6E51C60F1D40386CAA8F01834_AdjustorThunk (RuntimeObject * __this, MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 * _thisAdjusted = reinterpret_cast<MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 *>(__this + _offset);
	return MeshId_Equals_m685A94F74A3A06E6E51C60F1D40386CAA8F01834(_thisAdjusted, ___other0, method);
}
// System.Void UnityEngine.XR.MeshId::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MeshId__cctor_m98D91783008597CCFFBC675648A50107318509D7 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (MeshId__cctor_m98D91783008597CCFFBC675648A50107318509D7_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_initobj((((MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767_StaticFields*)il2cpp_codegen_static_fields_for(MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767_il2cpp_TypeInfo_var))->get_address_of_s_InvalidId_0()), sizeof(MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767 ));
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
// System.Void UnityEngine.XR.XRDisplaySubsystem::InvokeDisplayFocusChanged(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRDisplaySubsystem_InvokeDisplayFocusChanged_mF8F7E4D08B964907140FD3F8841F130159C7DBA7 (XRDisplaySubsystem_tF8B46605B6D1199C52306D4EC7D83CFA90564A93 * __this, bool ___focus0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (XRDisplaySubsystem_InvokeDisplayFocusChanged_mF8F7E4D08B964907140FD3F8841F130159C7DBA7_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		Action_1_tCE2D770918A65CAD277C08C4E8C05385EA267E83 * L_0 = __this->get_displayFocusChanged_2();
		V_0 = (bool)((!(((RuntimeObject*)(Action_1_tCE2D770918A65CAD277C08C4E8C05385EA267E83 *)L_0) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_1 = V_0;
		if (!L_1)
		{
			goto IL_001b;
		}
	}
	{
		Action_1_tCE2D770918A65CAD277C08C4E8C05385EA267E83 * L_2 = __this->get_displayFocusChanged_2();
		bool L_3 = ___focus0;
		NullCheck(L_2);
		Action_1_Invoke_m95E5C9FC67F7B0BDC3CD5E00AC5D4C8A00C97AC5(L_2, L_3, /*hidden argument*/Action_1_Invoke_m95E5C9FC67F7B0BDC3CD5E00AC5D4C8A00C97AC5_RuntimeMethod_var);
	}

IL_001b:
	{
		return;
	}
}
// System.Void UnityEngine.XR.XRDisplaySubsystem::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRDisplaySubsystem__ctor_mCC516BAAAC7175CC9CEECA04E488F5D9BF0FB774 (XRDisplaySubsystem_tF8B46605B6D1199C52306D4EC7D83CFA90564A93 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (XRDisplaySubsystem__ctor_mCC516BAAAC7175CC9CEECA04E488F5D9BF0FB774_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IntegratedSubsystem_1__ctor_m33699A39FA5AEAE5A383689E4E0E3523FB67E558(__this, /*hidden argument*/IntegratedSubsystem_1__ctor_m33699A39FA5AEAE5A383689E4E0E3523FB67E558_RuntimeMethod_var);
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
// System.Void UnityEngine.XR.XRDisplaySubsystemDescriptor::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRDisplaySubsystemDescriptor__ctor_mB045E1EBFB4D2B7CBE05D85D5AC622F7A971E056 (XRDisplaySubsystemDescriptor_tBBE6956FF61EACF13E72BFEF58ADC5930C760833 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (XRDisplaySubsystemDescriptor__ctor_mB045E1EBFB4D2B7CBE05D85D5AC622F7A971E056_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IntegratedSubsystemDescriptor_1__ctor_m3E9F6A2B441E056953C153C3B3182C0EB6BD0AFE(__this, /*hidden argument*/IntegratedSubsystemDescriptor_1__ctor_m3E9F6A2B441E056953C153C3B3182C0EB6BD0AFE_RuntimeMethod_var);
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
// System.Void UnityEngine.XR.XRInputSubsystem::InvokeTrackingOriginUpdatedEvent(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRInputSubsystem_InvokeTrackingOriginUpdatedEvent_m8A70C0391D141C9189155AAAD3D16493243A23D5 (intptr_t ___internalPtr0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (XRInputSubsystem_InvokeTrackingOriginUpdatedEvent_m8A70C0391D141C9189155AAAD3D16493243A23D5_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * V_0 = NULL;
	XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 * V_1 = NULL;
	bool V_2 = false;
	int32_t G_B3_0 = 0;
	{
		intptr_t L_0 = ___internalPtr0;
		IL2CPP_RUNTIME_CLASS_INIT(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var);
		IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * L_1 = Internal_SubsystemInstances_Internal_GetInstanceByPtr_m5C2B49DC08EBCC5036465D8928090D71F1D420F5((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * L_2 = V_0;
		V_1 = ((XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 *)IsInstClass((RuntimeObject*)L_2, XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09_il2cpp_TypeInfo_var));
		XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 * L_3 = V_1;
		if (!L_3)
		{
			goto IL_001d;
		}
	}
	{
		XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 * L_4 = V_1;
		NullCheck(L_4);
		Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 * L_5 = L_4->get_trackingOriginUpdated_2();
		G_B3_0 = ((!(((RuntimeObject*)(Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 *)L_5) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		goto IL_001e;
	}

IL_001d:
	{
		G_B3_0 = 0;
	}

IL_001e:
	{
		V_2 = (bool)G_B3_0;
		bool L_6 = V_2;
		if (!L_6)
		{
			goto IL_002f;
		}
	}
	{
		XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 * L_7 = V_1;
		NullCheck(L_7);
		Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 * L_8 = L_7->get_trackingOriginUpdated_2();
		XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 * L_9 = V_1;
		NullCheck(L_8);
		Action_1_Invoke_mA71F13E5E1EFDEB1DB1D9ED4C7ED037B21A89939(L_8, L_9, /*hidden argument*/Action_1_Invoke_mA71F13E5E1EFDEB1DB1D9ED4C7ED037B21A89939_RuntimeMethod_var);
	}

IL_002f:
	{
		return;
	}
}
// System.Void UnityEngine.XR.XRInputSubsystem::InvokeBoundaryChangedEvent(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRInputSubsystem_InvokeBoundaryChangedEvent_m795C2897F3A0047BBA6834D3F97B5DAFDEC4AE7A (intptr_t ___internalPtr0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (XRInputSubsystem_InvokeBoundaryChangedEvent_m795C2897F3A0047BBA6834D3F97B5DAFDEC4AE7A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * V_0 = NULL;
	XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 * V_1 = NULL;
	bool V_2 = false;
	int32_t G_B3_0 = 0;
	{
		intptr_t L_0 = ___internalPtr0;
		IL2CPP_RUNTIME_CLASS_INIT(Internal_SubsystemInstances_tA72C27A074ABEA48D0E813D2247349B55D160395_il2cpp_TypeInfo_var);
		IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * L_1 = Internal_SubsystemInstances_Internal_GetInstanceByPtr_m5C2B49DC08EBCC5036465D8928090D71F1D420F5((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IntegratedSubsystem_t8FB3A371F812CF9521903AC016C64E95C7412002 * L_2 = V_0;
		V_1 = ((XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 *)IsInstClass((RuntimeObject*)L_2, XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09_il2cpp_TypeInfo_var));
		XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 * L_3 = V_1;
		if (!L_3)
		{
			goto IL_001d;
		}
	}
	{
		XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 * L_4 = V_1;
		NullCheck(L_4);
		Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 * L_5 = L_4->get_boundaryChanged_3();
		G_B3_0 = ((!(((RuntimeObject*)(Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 *)L_5) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		goto IL_001e;
	}

IL_001d:
	{
		G_B3_0 = 0;
	}

IL_001e:
	{
		V_2 = (bool)G_B3_0;
		bool L_6 = V_2;
		if (!L_6)
		{
			goto IL_002f;
		}
	}
	{
		XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 * L_7 = V_1;
		NullCheck(L_7);
		Action_1_t6A8185B84663FAD87D88ACA618FB6E60131C81F1 * L_8 = L_7->get_boundaryChanged_3();
		XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 * L_9 = V_1;
		NullCheck(L_8);
		Action_1_Invoke_mA71F13E5E1EFDEB1DB1D9ED4C7ED037B21A89939(L_8, L_9, /*hidden argument*/Action_1_Invoke_mA71F13E5E1EFDEB1DB1D9ED4C7ED037B21A89939_RuntimeMethod_var);
	}

IL_002f:
	{
		return;
	}
}
// System.Void UnityEngine.XR.XRInputSubsystem::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRInputSubsystem__ctor_m80AE13105C9C373B38E4814244886DCB7AA3E7E8 (XRInputSubsystem_t74B79E3971C396F02CD32F74AE73A5DFB2A0EC09 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (XRInputSubsystem__ctor_m80AE13105C9C373B38E4814244886DCB7AA3E7E8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IntegratedSubsystem_1__ctor_m19C9BE11CA13915E2E14D5B4EC3EAF29CCC633E5(__this, /*hidden argument*/IntegratedSubsystem_1__ctor_m19C9BE11CA13915E2E14D5B4EC3EAF29CCC633E5_RuntimeMethod_var);
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
// System.Void UnityEngine.XR.XRInputSubsystemDescriptor::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRInputSubsystemDescriptor__ctor_m1620DD409E907F5AAA03D3DE504AC48D9D3E9576 (XRInputSubsystemDescriptor_t98C4233948EC9169B71D2A58C2C6ED1AF6FDABC2 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (XRInputSubsystemDescriptor__ctor_m1620DD409E907F5AAA03D3DE504AC48D9D3E9576_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IntegratedSubsystemDescriptor_1__ctor_m98CC72EADB42D92099DBE358C296423D7751A741(__this, /*hidden argument*/IntegratedSubsystemDescriptor_1__ctor_m98CC72EADB42D92099DBE358C296423D7751A741_RuntimeMethod_var);
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
// System.Void UnityEngine.XR.XRMeshSubsystem::InvokeMeshReadyDelegate(UnityEngine.XR.MeshGenerationResult,System.Action`1<UnityEngine.XR.MeshGenerationResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRMeshSubsystem_InvokeMeshReadyDelegate_mDD6390D57F2CD0F7EBD64B628DB780D23424FD8F (XRMeshSubsystem_t60BD977DF1B014CF5D48C8EBCC91DED767520D63 * __this, MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF  ___result0, Action_1_tB125CDA27D619FDBF92F767804A14CF83EA85A3C * ___onMeshGenerationComplete1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (XRMeshSubsystem_InvokeMeshReadyDelegate_mDD6390D57F2CD0F7EBD64B628DB780D23424FD8F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		Action_1_tB125CDA27D619FDBF92F767804A14CF83EA85A3C * L_0 = ___onMeshGenerationComplete1;
		V_0 = (bool)((!(((RuntimeObject*)(Action_1_tB125CDA27D619FDBF92F767804A14CF83EA85A3C *)L_0) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_1 = V_0;
		if (!L_1)
		{
			goto IL_0011;
		}
	}
	{
		Action_1_tB125CDA27D619FDBF92F767804A14CF83EA85A3C * L_2 = ___onMeshGenerationComplete1;
		MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF  L_3 = ___result0;
		NullCheck(L_2);
		Action_1_Invoke_mC3DCAEAD9DC81FE145B4FE115F830C0767728604(L_2, L_3, /*hidden argument*/Action_1_Invoke_mC3DCAEAD9DC81FE145B4FE115F830C0767728604_RuntimeMethod_var);
	}

IL_0011:
	{
		return;
	}
}
// System.Void UnityEngine.XR.XRMeshSubsystem::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRMeshSubsystem__ctor_mBA5B9B6A417BB2B477705E8BF6D1BFACF94AEF74 (XRMeshSubsystem_t60BD977DF1B014CF5D48C8EBCC91DED767520D63 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (XRMeshSubsystem__ctor_mBA5B9B6A417BB2B477705E8BF6D1BFACF94AEF74_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IntegratedSubsystem_1__ctor_m5D5CDD514B75369B0797B55401D9DD35908A2A26(__this, /*hidden argument*/IntegratedSubsystem_1__ctor_m5D5CDD514B75369B0797B55401D9DD35908A2A26_RuntimeMethod_var);
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
// System.Void UnityEngine.XR.XRMeshSubsystemDescriptor::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRMeshSubsystemDescriptor__ctor_m52169EE2174077BA5575410A1031C23915BBA6D2 (XRMeshSubsystemDescriptor_t428853FE3628F349D46DFD6841B50058F09F5FCC * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (XRMeshSubsystemDescriptor__ctor_m52169EE2174077BA5575410A1031C23915BBA6D2_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IntegratedSubsystemDescriptor_1__ctor_mBD46E84CF05A1E63F8FE6AB0C2F1C07AA8D2DAFB(__this, /*hidden argument*/IntegratedSubsystemDescriptor_1__ctor_mBD46E84CF05A1E63F8FE6AB0C2F1C07AA8D2DAFB_RuntimeMethod_var);
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
// System.Void UnityEngine.XR.XRNodeState::set_uniqueID(System.UInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRNodeState_set_uniqueID_m09D6E80AD1B81D7485242CFA27A7173A6231CA87 (XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 * __this, uint64_t ___value0, const RuntimeMethod* method)
{
	{
		uint64_t L_0 = ___value0;
		__this->set_m_UniqueID_9(L_0);
		return;
	}
}
IL2CPP_EXTERN_C  void XRNodeState_set_uniqueID_m09D6E80AD1B81D7485242CFA27A7173A6231CA87_AdjustorThunk (RuntimeObject * __this, uint64_t ___value0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 * _thisAdjusted = reinterpret_cast<XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 *>(__this + _offset);
	XRNodeState_set_uniqueID_m09D6E80AD1B81D7485242CFA27A7173A6231CA87(_thisAdjusted, ___value0, method);
}
// System.Void UnityEngine.XR.XRNodeState::set_nodeType(UnityEngine.XR.XRNode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRNodeState_set_nodeType_m9ADECAA5D665042FCA2F8E81726AFD1A1FA8B30D (XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___value0;
		__this->set_m_Type_0(L_0);
		return;
	}
}
IL2CPP_EXTERN_C  void XRNodeState_set_nodeType_m9ADECAA5D665042FCA2F8E81726AFD1A1FA8B30D_AdjustorThunk (RuntimeObject * __this, int32_t ___value0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 * _thisAdjusted = reinterpret_cast<XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 *>(__this + _offset);
	XRNodeState_set_nodeType_m9ADECAA5D665042FCA2F8E81726AFD1A1FA8B30D(_thisAdjusted, ___value0, method);
}
// System.Void UnityEngine.XR.XRNodeState::set_tracked(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void XRNodeState_set_tracked_m27DDD443D10F5F43B5B9AA83BFE901DC12316B9C (XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 * __this, bool ___value0, const RuntimeMethod* method)
{
	XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 * G_B2_0 = NULL;
	XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 * G_B1_0 = NULL;
	int32_t G_B3_0 = 0;
	XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 * G_B3_1 = NULL;
	{
		bool L_0 = ___value0;
		G_B1_0 = __this;
		if (L_0)
		{
			G_B2_0 = __this;
			goto IL_0008;
		}
	}
	{
		G_B3_0 = 0;
		G_B3_1 = G_B1_0;
		goto IL_0009;
	}

IL_0008:
	{
		G_B3_0 = 1;
		G_B3_1 = G_B2_0;
	}

IL_0009:
	{
		G_B3_1->set_m_Tracked_8(G_B3_0);
		return;
	}
}
IL2CPP_EXTERN_C  void XRNodeState_set_tracked_m27DDD443D10F5F43B5B9AA83BFE901DC12316B9C_AdjustorThunk (RuntimeObject * __this, bool ___value0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 * _thisAdjusted = reinterpret_cast<XRNodeState_t6DC58D0C1BF2C4323D16B3905FDBEE7C03E27D33 *>(__this + _offset);
	XRNodeState_set_tracked_m27DDD443D10F5F43B5B9AA83BFE901DC12316B9C(_thisAdjusted, ___value0, method);
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
// Conversion methods for marshalling of: UnityEngine.XR.XRDisplaySubsystem/XRMirrorViewBlitDesc
IL2CPP_EXTERN_C void XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5_marshal_pinvoke(const XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5& unmarshaled, XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5_marshaled_pinvoke& marshaled)
{
	marshaled.___displaySubsystemInstance_0 = unmarshaled.get_displaySubsystemInstance_0();
	marshaled.___nativeBlitAvailable_1 = static_cast<int32_t>(unmarshaled.get_nativeBlitAvailable_1());
	marshaled.___nativeBlitInvalidStates_2 = static_cast<int32_t>(unmarshaled.get_nativeBlitInvalidStates_2());
	marshaled.___blitParamsCount_3 = unmarshaled.get_blitParamsCount_3();
}
IL2CPP_EXTERN_C void XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5_marshal_pinvoke_back(const XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5_marshaled_pinvoke& marshaled, XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5& unmarshaled)
{
	intptr_t unmarshaled_displaySubsystemInstance_temp_0;
	memset((&unmarshaled_displaySubsystemInstance_temp_0), 0, sizeof(unmarshaled_displaySubsystemInstance_temp_0));
	unmarshaled_displaySubsystemInstance_temp_0 = marshaled.___displaySubsystemInstance_0;
	unmarshaled.set_displaySubsystemInstance_0(unmarshaled_displaySubsystemInstance_temp_0);
	bool unmarshaled_nativeBlitAvailable_temp_1 = false;
	unmarshaled_nativeBlitAvailable_temp_1 = static_cast<bool>(marshaled.___nativeBlitAvailable_1);
	unmarshaled.set_nativeBlitAvailable_1(unmarshaled_nativeBlitAvailable_temp_1);
	bool unmarshaled_nativeBlitInvalidStates_temp_2 = false;
	unmarshaled_nativeBlitInvalidStates_temp_2 = static_cast<bool>(marshaled.___nativeBlitInvalidStates_2);
	unmarshaled.set_nativeBlitInvalidStates_2(unmarshaled_nativeBlitInvalidStates_temp_2);
	int32_t unmarshaled_blitParamsCount_temp_3 = 0;
	unmarshaled_blitParamsCount_temp_3 = marshaled.___blitParamsCount_3;
	unmarshaled.set_blitParamsCount_3(unmarshaled_blitParamsCount_temp_3);
}
// Conversion method for clean up from marshalling of: UnityEngine.XR.XRDisplaySubsystem/XRMirrorViewBlitDesc
IL2CPP_EXTERN_C void XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5_marshal_pinvoke_cleanup(XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: UnityEngine.XR.XRDisplaySubsystem/XRMirrorViewBlitDesc
IL2CPP_EXTERN_C void XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5_marshal_com(const XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5& unmarshaled, XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5_marshaled_com& marshaled)
{
	marshaled.___displaySubsystemInstance_0 = unmarshaled.get_displaySubsystemInstance_0();
	marshaled.___nativeBlitAvailable_1 = static_cast<int32_t>(unmarshaled.get_nativeBlitAvailable_1());
	marshaled.___nativeBlitInvalidStates_2 = static_cast<int32_t>(unmarshaled.get_nativeBlitInvalidStates_2());
	marshaled.___blitParamsCount_3 = unmarshaled.get_blitParamsCount_3();
}
IL2CPP_EXTERN_C void XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5_marshal_com_back(const XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5_marshaled_com& marshaled, XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5& unmarshaled)
{
	intptr_t unmarshaled_displaySubsystemInstance_temp_0;
	memset((&unmarshaled_displaySubsystemInstance_temp_0), 0, sizeof(unmarshaled_displaySubsystemInstance_temp_0));
	unmarshaled_displaySubsystemInstance_temp_0 = marshaled.___displaySubsystemInstance_0;
	unmarshaled.set_displaySubsystemInstance_0(unmarshaled_displaySubsystemInstance_temp_0);
	bool unmarshaled_nativeBlitAvailable_temp_1 = false;
	unmarshaled_nativeBlitAvailable_temp_1 = static_cast<bool>(marshaled.___nativeBlitAvailable_1);
	unmarshaled.set_nativeBlitAvailable_1(unmarshaled_nativeBlitAvailable_temp_1);
	bool unmarshaled_nativeBlitInvalidStates_temp_2 = false;
	unmarshaled_nativeBlitInvalidStates_temp_2 = static_cast<bool>(marshaled.___nativeBlitInvalidStates_2);
	unmarshaled.set_nativeBlitInvalidStates_2(unmarshaled_nativeBlitInvalidStates_temp_2);
	int32_t unmarshaled_blitParamsCount_temp_3 = 0;
	unmarshaled_blitParamsCount_temp_3 = marshaled.___blitParamsCount_3;
	unmarshaled.set_blitParamsCount_3(unmarshaled_blitParamsCount_temp_3);
}
// Conversion method for clean up from marshalling of: UnityEngine.XR.XRDisplaySubsystem/XRMirrorViewBlitDesc
IL2CPP_EXTERN_C void XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5_marshal_com_cleanup(XRMirrorViewBlitDesc_t3BD136F0BF088017ABB0EF1856191541211848A5_marshaled_com& marshaled)
{
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Conversion methods for marshalling of: UnityEngine.XR.XRDisplaySubsystem/XRRenderPass
IL2CPP_EXTERN_C void XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB_marshal_pinvoke(const XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB& unmarshaled, XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB_marshaled_pinvoke& marshaled)
{
	marshaled.___displaySubsystemInstance_0 = unmarshaled.get_displaySubsystemInstance_0();
	marshaled.___renderPassIndex_1 = unmarshaled.get_renderPassIndex_1();
	marshaled.___renderTarget_2 = unmarshaled.get_renderTarget_2();
	marshaled.___renderTargetDesc_3 = unmarshaled.get_renderTargetDesc_3();
	marshaled.___shouldFillOutDepth_4 = static_cast<int32_t>(unmarshaled.get_shouldFillOutDepth_4());
	marshaled.___cullingPassIndex_5 = unmarshaled.get_cullingPassIndex_5();
}
IL2CPP_EXTERN_C void XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB_marshal_pinvoke_back(const XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB_marshaled_pinvoke& marshaled, XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB& unmarshaled)
{
	intptr_t unmarshaled_displaySubsystemInstance_temp_0;
	memset((&unmarshaled_displaySubsystemInstance_temp_0), 0, sizeof(unmarshaled_displaySubsystemInstance_temp_0));
	unmarshaled_displaySubsystemInstance_temp_0 = marshaled.___displaySubsystemInstance_0;
	unmarshaled.set_displaySubsystemInstance_0(unmarshaled_displaySubsystemInstance_temp_0);
	int32_t unmarshaled_renderPassIndex_temp_1 = 0;
	unmarshaled_renderPassIndex_temp_1 = marshaled.___renderPassIndex_1;
	unmarshaled.set_renderPassIndex_1(unmarshaled_renderPassIndex_temp_1);
	RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13  unmarshaled_renderTarget_temp_2;
	memset((&unmarshaled_renderTarget_temp_2), 0, sizeof(unmarshaled_renderTarget_temp_2));
	unmarshaled_renderTarget_temp_2 = marshaled.___renderTarget_2;
	unmarshaled.set_renderTarget_2(unmarshaled_renderTarget_temp_2);
	RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47  unmarshaled_renderTargetDesc_temp_3;
	memset((&unmarshaled_renderTargetDesc_temp_3), 0, sizeof(unmarshaled_renderTargetDesc_temp_3));
	unmarshaled_renderTargetDesc_temp_3 = marshaled.___renderTargetDesc_3;
	unmarshaled.set_renderTargetDesc_3(unmarshaled_renderTargetDesc_temp_3);
	bool unmarshaled_shouldFillOutDepth_temp_4 = false;
	unmarshaled_shouldFillOutDepth_temp_4 = static_cast<bool>(marshaled.___shouldFillOutDepth_4);
	unmarshaled.set_shouldFillOutDepth_4(unmarshaled_shouldFillOutDepth_temp_4);
	int32_t unmarshaled_cullingPassIndex_temp_5 = 0;
	unmarshaled_cullingPassIndex_temp_5 = marshaled.___cullingPassIndex_5;
	unmarshaled.set_cullingPassIndex_5(unmarshaled_cullingPassIndex_temp_5);
}
// Conversion method for clean up from marshalling of: UnityEngine.XR.XRDisplaySubsystem/XRRenderPass
IL2CPP_EXTERN_C void XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB_marshal_pinvoke_cleanup(XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: UnityEngine.XR.XRDisplaySubsystem/XRRenderPass
IL2CPP_EXTERN_C void XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB_marshal_com(const XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB& unmarshaled, XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB_marshaled_com& marshaled)
{
	marshaled.___displaySubsystemInstance_0 = unmarshaled.get_displaySubsystemInstance_0();
	marshaled.___renderPassIndex_1 = unmarshaled.get_renderPassIndex_1();
	marshaled.___renderTarget_2 = unmarshaled.get_renderTarget_2();
	marshaled.___renderTargetDesc_3 = unmarshaled.get_renderTargetDesc_3();
	marshaled.___shouldFillOutDepth_4 = static_cast<int32_t>(unmarshaled.get_shouldFillOutDepth_4());
	marshaled.___cullingPassIndex_5 = unmarshaled.get_cullingPassIndex_5();
}
IL2CPP_EXTERN_C void XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB_marshal_com_back(const XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB_marshaled_com& marshaled, XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB& unmarshaled)
{
	intptr_t unmarshaled_displaySubsystemInstance_temp_0;
	memset((&unmarshaled_displaySubsystemInstance_temp_0), 0, sizeof(unmarshaled_displaySubsystemInstance_temp_0));
	unmarshaled_displaySubsystemInstance_temp_0 = marshaled.___displaySubsystemInstance_0;
	unmarshaled.set_displaySubsystemInstance_0(unmarshaled_displaySubsystemInstance_temp_0);
	int32_t unmarshaled_renderPassIndex_temp_1 = 0;
	unmarshaled_renderPassIndex_temp_1 = marshaled.___renderPassIndex_1;
	unmarshaled.set_renderPassIndex_1(unmarshaled_renderPassIndex_temp_1);
	RenderTargetIdentifier_t70F41F3016FFCC4AAF4D7C57F280818114534C13  unmarshaled_renderTarget_temp_2;
	memset((&unmarshaled_renderTarget_temp_2), 0, sizeof(unmarshaled_renderTarget_temp_2));
	unmarshaled_renderTarget_temp_2 = marshaled.___renderTarget_2;
	unmarshaled.set_renderTarget_2(unmarshaled_renderTarget_temp_2);
	RenderTextureDescriptor_t67FF189E1F35AEB5D6C43A2D7103F3A8A8CA0B47  unmarshaled_renderTargetDesc_temp_3;
	memset((&unmarshaled_renderTargetDesc_temp_3), 0, sizeof(unmarshaled_renderTargetDesc_temp_3));
	unmarshaled_renderTargetDesc_temp_3 = marshaled.___renderTargetDesc_3;
	unmarshaled.set_renderTargetDesc_3(unmarshaled_renderTargetDesc_temp_3);
	bool unmarshaled_shouldFillOutDepth_temp_4 = false;
	unmarshaled_shouldFillOutDepth_temp_4 = static_cast<bool>(marshaled.___shouldFillOutDepth_4);
	unmarshaled.set_shouldFillOutDepth_4(unmarshaled_shouldFillOutDepth_temp_4);
	int32_t unmarshaled_cullingPassIndex_temp_5 = 0;
	unmarshaled_cullingPassIndex_temp_5 = marshaled.___cullingPassIndex_5;
	unmarshaled.set_cullingPassIndex_5(unmarshaled_cullingPassIndex_temp_5);
}
// Conversion method for clean up from marshalling of: UnityEngine.XR.XRDisplaySubsystem/XRRenderPass
IL2CPP_EXTERN_C void XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB_marshal_com_cleanup(XRRenderPass_tCB4A9F3B07C2C59889BD3EE40F44E9347A2BC9BB_marshaled_com& marshaled)
{
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  MeshGenerationResult_get_MeshId_m55663E958B980188CFD406BB2469B24D8089BA9E_inline (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method)
{
	{
		MeshId_t583996FC9E6BA652AA2C6B0D0F60D88E4498D767  L_0 = __this->get_U3CMeshIdU3Ek__BackingField_0();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 * MeshGenerationResult_get_Mesh_mDBEB5E83FC729856B17AF62B9438C9B9A79A0200_inline (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method)
{
	{
		Mesh_t2F5992DBA650D5862B43D3823ACD997132A57DA6 * L_0 = __this->get_U3CMeshU3Ek__BackingField_1();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 * MeshGenerationResult_get_MeshCollider_m0285F3BFFFF0778DC8DDA97BFDAE30A19AEBF283_inline (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method)
{
	{
		MeshCollider_t1983F4E7E53D8C6B65FE21A8B4E2345A84D57E98 * L_0 = __this->get_U3CMeshColliderU3Ek__BackingField_2();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t MeshGenerationResult_get_Status_m5AF51D2745EC947BB722550DC95665D430CDB178_inline (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->get_U3CStatusU3Ek__BackingField_3();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t MeshGenerationResult_get_Attributes_m24D66B0694F827D4DFFE210069EF0F5578F4845F_inline (MeshGenerationResult_t081845588E8932BB4BA2D6F087D2F2F0EE3373CF * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->get_U3CAttributesU3Ek__BackingField_4();
		return L_0;
	}
}
