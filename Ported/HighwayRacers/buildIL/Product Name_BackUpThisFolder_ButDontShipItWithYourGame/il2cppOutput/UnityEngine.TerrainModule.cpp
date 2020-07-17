#include "pch-cpp.hpp"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <limits>
#include <stdint.h>


template <typename T1, typename T2>
struct VirtActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3>
struct VirtActionInvoker3
{
	typedef void (*Action)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
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
template <typename T1, typename T2>
struct GenericVirtActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3>
struct GenericVirtActionInvoker3
{
	typedef void (*Action)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
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
template <typename R, typename T1>
struct GenericVirtFuncInvoker1
{
	typedef R (*Func)(void*, T1, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename T1, typename T2>
struct InterfaceActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3>
struct InterfaceActionInvoker3
{
	typedef void (*Action)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
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
template <typename R, typename T1>
struct InterfaceFuncInvoker1
{
	typedef R (*Func)(void*, T1, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename T1, typename T2>
struct GenericInterfaceActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3>
struct GenericInterfaceActionInvoker3
{
	typedef void (*Action)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
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
template <typename R, typename T1>
struct GenericInterfaceFuncInvoker1
{
	typedef R (*Func)(void*, T1, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};

// System.Delegate
struct Delegate_t;
// System.DelegateData
struct DelegateData_t17DD30660E330C49381DAA99F934BE75CB11F288;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// UnityEngine.Terrain
struct Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836;
// UnityEngine.TerrainData
struct TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4;
// System.Void
struct Void_t700C6383A2A510C2CF4DD86DABD5CA9FF70ADAC5;
// UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/HeightmapChangedCallback
struct HeightmapChangedCallback_tB00DA531F9C32468E88700A5C2D55E05189E0FA0;
// UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/TextureChangedCallback
struct TextureChangedCallback_tD8BA8EA99CC9FA597E4AA143944720822EFB7D9F;
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_0
struct U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8;
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_1
struct U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696;
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainGroups
struct TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288;
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap
struct TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453;
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/<>c__DisplayClass4_0
struct U3CU3Ec__DisplayClass4_0_tBEB3CC092598F0D16C66B724FF1AE52EF06C0A8F;
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter
struct TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7;
// System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>
struct Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C;
// System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>
struct Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C;
// System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>
struct Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725;
// System.Collections.Generic.Dictionary`2<System.Int32,System.Object>
struct Dictionary_2_tE1E5B6327FFA2C7AE34A69E0011815C914771C2F;
// System.Collections.Generic.IEqualityComparer`1<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord>
struct IEqualityComparer_1_tA69FAEE2FF62FD94D03621391FBBAA1F820E8B92;
// System.Collections.Generic.IEqualityComparer`1<System.Int32>
struct IEqualityComparer_1_t62010156673DE1460AB1D1CEBE5DCD48665E1A38;
// System.Collections.Generic.Dictionary`2/KeyCollection<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>
struct KeyCollection_t35C4534DEF9EFE5AABB676279D2BD96D97E040EC;
// System.Collections.Generic.Dictionary`2/KeyCollection<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>
struct KeyCollection_t7A3E9764F18C1DE44C90DF9D411158787E20A943;
// System.Collections.Generic.Dictionary`2/KeyCollection<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>
struct KeyCollection_t749DBEFA13BA24F77DF2C12137D5331F541F3B15;
// System.Collections.Generic.Dictionary`2/ValueCollection<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>
struct ValueCollection_tA937879944B4CC06877A7FDB0FD1D2B941D9221B;
// System.Collections.Generic.Dictionary`2/ValueCollection<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>
struct ValueCollection_tF5A29AA52483C44A9B166F23F45001565015B4EB;
// System.AsyncCallback
struct AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA;
// UnityEngine.Behaviour
struct Behaviour_t1A3DDDCF73B4627928FBFE02ED52B7251777DBD9;
// System.Char[]
struct CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34;
// UnityEngine.Component
struct Component_t62FBC8D2420DA4BE9037AFE430740F6B3EECA684;
// System.Delegate[]
struct DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8;
// System.Collections.Generic.Dictionary`2/Entry<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>[]
struct EntryU5BU5D_t8F8773833E17C6A0E66C03DDD293977F035F44EC;
// System.Collections.Generic.Dictionary`2/Entry<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>[]
struct EntryU5BU5D_tAFCE4E7A6A0D15A8B29BA1FBF1D4C566393DA40C;
// System.IAsyncResult
struct IAsyncResult_tC9F97BF36FCF122D29D3101D80642278297BF370;
// System.Int32[]
struct Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32;
// UnityEngine.Object
struct Object_tF2F3778131EFF286AF62B7B013A170F95A91571A;
// System.String
struct String_t;
// UnityEngine.Terrain[]
struct TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57;
// UnityEngine.Transform
struct Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1;

IL2CPP_EXTERN_C RuntimeClass* Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Mathf_t4D4AC358D24F6DDC32EC291DDE1DF2C3B752A194_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TerrainCallbacks_tF292CB70850DEF93A2AFD0005B4FF75C7FC8ECD0_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass4_0_tBEB3CC092598F0D16C66B724FF1AE52EF06C0A8F_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_Add_m267342D40CFF0F8B5BAD87A5CE00E3909531BD96_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_Add_mE1C4F9D19A66168F6DAD8690E89CDB2A36353F0D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_ContainsKey_mAB645E14BEA5777BD44ADAE7A50A6F0A8093BC9C_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_GetEnumerator_m147C3BB4ED736CEA41232F46C074D6B974011AA4_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_GetEnumerator_m2FABB49D216C87FA099BA346CB5CD03DCD24C952_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_TryGetValue_m98F72F5729EC6BC5A95E45EE02F330D48C274FE8_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2__ctor_mCCBF2E85C3037B87300EF879DD1791B4EB6DD230_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2__ctor_mE808665E6AFDBF2A2BA8A0F50089B72EE98DBBA1_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_get_Count_m83CBEC6C5312F7F9158B9FDA00ACD5FDC0169F27_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_get_Count_mFA499FD6C7C49BC3D70E56F18B385FC96BB1122D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_get_Keys_mAE2D87453C7973972A5637C6F4EAD27613692826_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_Dispose_m07DC0EE5F0A8163D940559D768B774000D6D38AB_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_Dispose_m1F1794448E1C1BD96E362C46BA0DB16B018D57E4_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_Dispose_mAC047D3AE663114F2D8DFDA994E2C9D78A3E9EB0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_MoveNext_m0CCC05F07B62FCEE6591F765FC15D804D05BAD28_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_MoveNext_m5380371A16E1990E23859EED5F3C2F1843B39B38_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_MoveNext_m821A025A3826D2405C49130607D842500F1ECEF4_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_get_Current_m13B3FF8E2918832E70E30374C13E9939E9AA3894_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_get_Current_mB812E91C3669D0EE3EACF5F58E0A9BBD03D43711_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_get_Current_mF013CBBAC5FC4D1B2E4A4DF551C8FE254F675FC3_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* KeyCollection_GetEnumerator_m6405FB5505A9993F393EA3F5C33A46514043AA2A_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* KeyValuePair_2_get_Key_mE6C14010B6C03B4E060CEF852A6F22FDC4713D0E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* KeyValuePair_2_get_Value_m011C84EFA22A68B46C33DD7DF651E3B2A65D0A8E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CU3Ec__DisplayClass4_0_U3CCreateFromPlacementU3Eb__0_mA0E2295171D220FA7ABA12660D2CB357BC721653_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CU3Ec__DisplayClass4_1_U3CCollectTerrainsU3Eb__0_m539C07F9B8F371A9E9C09A8AFD003DD4163C7810_RuntimeMethod_var;
IL2CPP_EXTERN_C const uint32_t HeightmapChangedCallback_BeginInvoke_m590DAC8F14CB6AC982D6FE89C27ACF10CFA17E05_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainCallbacks_InvokeHeightmapChangedCallback_m394735D1416B00373916335213992D011D5FDA86_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainCallbacks_InvokeTextureChangedCallback_m10A2EFE8E490EC932777717717CC61709FCA3307_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainData__cctor_m64E6CF88BD21FC182D29D169EBCA04D965C46517_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainData__ctor_m09DE788EE93388ACD3E80CB586FC2ED551B66ED7_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainGroups__ctor_mA9F11D4BE52D80563D0D31788BA80C8F5381FFB1_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap_AddTerrainInternal_m82F62E3018D1D2A6E48FB7361DB6531F0E9BEB79_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap_CreateFromPlacement_m8BCE09C1C736432F61D78CED8868DC43F9CCD25D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap_CreateFromPlacement_mBF5B980BA13C9390739DFEA1644596CA54D44337_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap_TryToAddTerrain_m49A7085766F102EADE7E4A29259232F399735C61_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap_ValidateTerrain_mFE264FDE78C3D68285943250BC9FABAC89D85764_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap_Validate_m9CD6FAF70E4F90C896BF25F083BC0A7F21C8FA56_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap__ctor_m0A16A2E6ED5C4EFB2F87D72A5665EF7C4E62F761_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainUtility_AutoConnect_m8526A29E63B328915E516505E3195637A1F100EF_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainUtility_CollectTerrains_m4630246A7274A15FB2AE8C13E653E8B73C129F9B_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TextureChangedCallback_BeginInvoke_mE19FD540CF24CED1C990B54DE4A84A270C5BA37C_MetadataUsageId;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;

struct DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8;
struct TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// <Module>
struct  U3CModuleU3E_t1E32A317E34BC1FD0A7614BD67748813CD043632 
{
public:

public:
};


// System.Object

struct Il2CppArrayBounds;

// System.Array


// System.Collections.Generic.Dictionary`2_KeyCollection<UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TileCoord,UnityEngine.Terrain>
struct  KeyCollection_t35C4534DEF9EFE5AABB676279D2BD96D97E040EC  : public RuntimeObject
{
public:
	// System.Collections.Generic.Dictionary`2<TKey,TValue> System.Collections.Generic.Dictionary`2_KeyCollection::dictionary
	Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * ___dictionary_0;

public:
	inline static int32_t get_offset_of_dictionary_0() { return static_cast<int32_t>(offsetof(KeyCollection_t35C4534DEF9EFE5AABB676279D2BD96D97E040EC, ___dictionary_0)); }
	inline Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * get_dictionary_0() const { return ___dictionary_0; }
	inline Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C ** get_address_of_dictionary_0() { return &___dictionary_0; }
	inline void set_dictionary_0(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * value)
	{
		___dictionary_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dictionary_0), (void*)value);
	}
};


// System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap>
struct  Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725  : public RuntimeObject
{
public:
	// System.Int32[] System.Collections.Generic.Dictionary`2::buckets
	Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* ___buckets_0;
	// System.Collections.Generic.Dictionary`2_Entry<TKey,TValue>[] System.Collections.Generic.Dictionary`2::entries
	EntryU5BU5D_t8F8773833E17C6A0E66C03DDD293977F035F44EC* ___entries_1;
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
	KeyCollection_t749DBEFA13BA24F77DF2C12137D5331F541F3B15 * ___keys_7;
	// System.Collections.Generic.Dictionary`2_ValueCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::values
	ValueCollection_tF5A29AA52483C44A9B166F23F45001565015B4EB * ___values_8;
	// System.Object System.Collections.Generic.Dictionary`2::_syncRoot
	RuntimeObject * ____syncRoot_9;

public:
	inline static int32_t get_offset_of_buckets_0() { return static_cast<int32_t>(offsetof(Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725, ___buckets_0)); }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* get_buckets_0() const { return ___buckets_0; }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32** get_address_of_buckets_0() { return &___buckets_0; }
	inline void set_buckets_0(Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* value)
	{
		___buckets_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___buckets_0), (void*)value);
	}

	inline static int32_t get_offset_of_entries_1() { return static_cast<int32_t>(offsetof(Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725, ___entries_1)); }
	inline EntryU5BU5D_t8F8773833E17C6A0E66C03DDD293977F035F44EC* get_entries_1() const { return ___entries_1; }
	inline EntryU5BU5D_t8F8773833E17C6A0E66C03DDD293977F035F44EC** get_address_of_entries_1() { return &___entries_1; }
	inline void set_entries_1(EntryU5BU5D_t8F8773833E17C6A0E66C03DDD293977F035F44EC* value)
	{
		___entries_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___entries_1), (void*)value);
	}

	inline static int32_t get_offset_of_count_2() { return static_cast<int32_t>(offsetof(Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725, ___count_2)); }
	inline int32_t get_count_2() const { return ___count_2; }
	inline int32_t* get_address_of_count_2() { return &___count_2; }
	inline void set_count_2(int32_t value)
	{
		___count_2 = value;
	}

	inline static int32_t get_offset_of_version_3() { return static_cast<int32_t>(offsetof(Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725, ___version_3)); }
	inline int32_t get_version_3() const { return ___version_3; }
	inline int32_t* get_address_of_version_3() { return &___version_3; }
	inline void set_version_3(int32_t value)
	{
		___version_3 = value;
	}

	inline static int32_t get_offset_of_freeList_4() { return static_cast<int32_t>(offsetof(Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725, ___freeList_4)); }
	inline int32_t get_freeList_4() const { return ___freeList_4; }
	inline int32_t* get_address_of_freeList_4() { return &___freeList_4; }
	inline void set_freeList_4(int32_t value)
	{
		___freeList_4 = value;
	}

	inline static int32_t get_offset_of_freeCount_5() { return static_cast<int32_t>(offsetof(Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725, ___freeCount_5)); }
	inline int32_t get_freeCount_5() const { return ___freeCount_5; }
	inline int32_t* get_address_of_freeCount_5() { return &___freeCount_5; }
	inline void set_freeCount_5(int32_t value)
	{
		___freeCount_5 = value;
	}

	inline static int32_t get_offset_of_comparer_6() { return static_cast<int32_t>(offsetof(Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725, ___comparer_6)); }
	inline RuntimeObject* get_comparer_6() const { return ___comparer_6; }
	inline RuntimeObject** get_address_of_comparer_6() { return &___comparer_6; }
	inline void set_comparer_6(RuntimeObject* value)
	{
		___comparer_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___comparer_6), (void*)value);
	}

	inline static int32_t get_offset_of_keys_7() { return static_cast<int32_t>(offsetof(Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725, ___keys_7)); }
	inline KeyCollection_t749DBEFA13BA24F77DF2C12137D5331F541F3B15 * get_keys_7() const { return ___keys_7; }
	inline KeyCollection_t749DBEFA13BA24F77DF2C12137D5331F541F3B15 ** get_address_of_keys_7() { return &___keys_7; }
	inline void set_keys_7(KeyCollection_t749DBEFA13BA24F77DF2C12137D5331F541F3B15 * value)
	{
		___keys_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___keys_7), (void*)value);
	}

	inline static int32_t get_offset_of_values_8() { return static_cast<int32_t>(offsetof(Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725, ___values_8)); }
	inline ValueCollection_tF5A29AA52483C44A9B166F23F45001565015B4EB * get_values_8() const { return ___values_8; }
	inline ValueCollection_tF5A29AA52483C44A9B166F23F45001565015B4EB ** get_address_of_values_8() { return &___values_8; }
	inline void set_values_8(ValueCollection_tF5A29AA52483C44A9B166F23F45001565015B4EB * value)
	{
		___values_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___values_8), (void*)value);
	}

	inline static int32_t get_offset_of__syncRoot_9() { return static_cast<int32_t>(offsetof(Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725, ____syncRoot_9)); }
	inline RuntimeObject * get__syncRoot_9() const { return ____syncRoot_9; }
	inline RuntimeObject ** get_address_of__syncRoot_9() { return &____syncRoot_9; }
	inline void set__syncRoot_9(RuntimeObject * value)
	{
		____syncRoot_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_9), (void*)value);
	}
};


// System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TileCoord,UnityEngine.Terrain>
struct  Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C  : public RuntimeObject
{
public:
	// System.Int32[] System.Collections.Generic.Dictionary`2::buckets
	Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* ___buckets_0;
	// System.Collections.Generic.Dictionary`2_Entry<TKey,TValue>[] System.Collections.Generic.Dictionary`2::entries
	EntryU5BU5D_tAFCE4E7A6A0D15A8B29BA1FBF1D4C566393DA40C* ___entries_1;
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
	KeyCollection_t35C4534DEF9EFE5AABB676279D2BD96D97E040EC * ___keys_7;
	// System.Collections.Generic.Dictionary`2_ValueCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::values
	ValueCollection_tA937879944B4CC06877A7FDB0FD1D2B941D9221B * ___values_8;
	// System.Object System.Collections.Generic.Dictionary`2::_syncRoot
	RuntimeObject * ____syncRoot_9;

public:
	inline static int32_t get_offset_of_buckets_0() { return static_cast<int32_t>(offsetof(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C, ___buckets_0)); }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* get_buckets_0() const { return ___buckets_0; }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32** get_address_of_buckets_0() { return &___buckets_0; }
	inline void set_buckets_0(Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* value)
	{
		___buckets_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___buckets_0), (void*)value);
	}

	inline static int32_t get_offset_of_entries_1() { return static_cast<int32_t>(offsetof(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C, ___entries_1)); }
	inline EntryU5BU5D_tAFCE4E7A6A0D15A8B29BA1FBF1D4C566393DA40C* get_entries_1() const { return ___entries_1; }
	inline EntryU5BU5D_tAFCE4E7A6A0D15A8B29BA1FBF1D4C566393DA40C** get_address_of_entries_1() { return &___entries_1; }
	inline void set_entries_1(EntryU5BU5D_tAFCE4E7A6A0D15A8B29BA1FBF1D4C566393DA40C* value)
	{
		___entries_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___entries_1), (void*)value);
	}

	inline static int32_t get_offset_of_count_2() { return static_cast<int32_t>(offsetof(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C, ___count_2)); }
	inline int32_t get_count_2() const { return ___count_2; }
	inline int32_t* get_address_of_count_2() { return &___count_2; }
	inline void set_count_2(int32_t value)
	{
		___count_2 = value;
	}

	inline static int32_t get_offset_of_version_3() { return static_cast<int32_t>(offsetof(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C, ___version_3)); }
	inline int32_t get_version_3() const { return ___version_3; }
	inline int32_t* get_address_of_version_3() { return &___version_3; }
	inline void set_version_3(int32_t value)
	{
		___version_3 = value;
	}

	inline static int32_t get_offset_of_freeList_4() { return static_cast<int32_t>(offsetof(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C, ___freeList_4)); }
	inline int32_t get_freeList_4() const { return ___freeList_4; }
	inline int32_t* get_address_of_freeList_4() { return &___freeList_4; }
	inline void set_freeList_4(int32_t value)
	{
		___freeList_4 = value;
	}

	inline static int32_t get_offset_of_freeCount_5() { return static_cast<int32_t>(offsetof(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C, ___freeCount_5)); }
	inline int32_t get_freeCount_5() const { return ___freeCount_5; }
	inline int32_t* get_address_of_freeCount_5() { return &___freeCount_5; }
	inline void set_freeCount_5(int32_t value)
	{
		___freeCount_5 = value;
	}

	inline static int32_t get_offset_of_comparer_6() { return static_cast<int32_t>(offsetof(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C, ___comparer_6)); }
	inline RuntimeObject* get_comparer_6() const { return ___comparer_6; }
	inline RuntimeObject** get_address_of_comparer_6() { return &___comparer_6; }
	inline void set_comparer_6(RuntimeObject* value)
	{
		___comparer_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___comparer_6), (void*)value);
	}

	inline static int32_t get_offset_of_keys_7() { return static_cast<int32_t>(offsetof(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C, ___keys_7)); }
	inline KeyCollection_t35C4534DEF9EFE5AABB676279D2BD96D97E040EC * get_keys_7() const { return ___keys_7; }
	inline KeyCollection_t35C4534DEF9EFE5AABB676279D2BD96D97E040EC ** get_address_of_keys_7() { return &___keys_7; }
	inline void set_keys_7(KeyCollection_t35C4534DEF9EFE5AABB676279D2BD96D97E040EC * value)
	{
		___keys_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___keys_7), (void*)value);
	}

	inline static int32_t get_offset_of_values_8() { return static_cast<int32_t>(offsetof(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C, ___values_8)); }
	inline ValueCollection_tA937879944B4CC06877A7FDB0FD1D2B941D9221B * get_values_8() const { return ___values_8; }
	inline ValueCollection_tA937879944B4CC06877A7FDB0FD1D2B941D9221B ** get_address_of_values_8() { return &___values_8; }
	inline void set_values_8(ValueCollection_tA937879944B4CC06877A7FDB0FD1D2B941D9221B * value)
	{
		___values_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___values_8), (void*)value);
	}

	inline static int32_t get_offset_of__syncRoot_9() { return static_cast<int32_t>(offsetof(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C, ____syncRoot_9)); }
	inline RuntimeObject * get__syncRoot_9() const { return ____syncRoot_9; }
	inline RuntimeObject ** get_address_of__syncRoot_9() { return &____syncRoot_9; }
	inline void set__syncRoot_9(RuntimeObject * value)
	{
		____syncRoot_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_9), (void*)value);
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

// UnityEngine.Experimental.TerrainAPI.TerrainCallbacks
struct  TerrainCallbacks_tF292CB70850DEF93A2AFD0005B4FF75C7FC8ECD0  : public RuntimeObject
{
public:

public:
};

struct TerrainCallbacks_tF292CB70850DEF93A2AFD0005B4FF75C7FC8ECD0_StaticFields
{
public:
	// UnityEngine.Experimental.TerrainAPI.TerrainCallbacks_HeightmapChangedCallback UnityEngine.Experimental.TerrainAPI.TerrainCallbacks::heightmapChanged
	HeightmapChangedCallback_tB00DA531F9C32468E88700A5C2D55E05189E0FA0 * ___heightmapChanged_0;
	// UnityEngine.Experimental.TerrainAPI.TerrainCallbacks_TextureChangedCallback UnityEngine.Experimental.TerrainAPI.TerrainCallbacks::textureChanged
	TextureChangedCallback_tD8BA8EA99CC9FA597E4AA143944720822EFB7D9F * ___textureChanged_1;

public:
	inline static int32_t get_offset_of_heightmapChanged_0() { return static_cast<int32_t>(offsetof(TerrainCallbacks_tF292CB70850DEF93A2AFD0005B4FF75C7FC8ECD0_StaticFields, ___heightmapChanged_0)); }
	inline HeightmapChangedCallback_tB00DA531F9C32468E88700A5C2D55E05189E0FA0 * get_heightmapChanged_0() const { return ___heightmapChanged_0; }
	inline HeightmapChangedCallback_tB00DA531F9C32468E88700A5C2D55E05189E0FA0 ** get_address_of_heightmapChanged_0() { return &___heightmapChanged_0; }
	inline void set_heightmapChanged_0(HeightmapChangedCallback_tB00DA531F9C32468E88700A5C2D55E05189E0FA0 * value)
	{
		___heightmapChanged_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___heightmapChanged_0), (void*)value);
	}

	inline static int32_t get_offset_of_textureChanged_1() { return static_cast<int32_t>(offsetof(TerrainCallbacks_tF292CB70850DEF93A2AFD0005B4FF75C7FC8ECD0_StaticFields, ___textureChanged_1)); }
	inline TextureChangedCallback_tD8BA8EA99CC9FA597E4AA143944720822EFB7D9F * get_textureChanged_1() const { return ___textureChanged_1; }
	inline TextureChangedCallback_tD8BA8EA99CC9FA597E4AA143944720822EFB7D9F ** get_address_of_textureChanged_1() { return &___textureChanged_1; }
	inline void set_textureChanged_1(TextureChangedCallback_tD8BA8EA99CC9FA597E4AA143944720822EFB7D9F * value)
	{
		___textureChanged_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___textureChanged_1), (void*)value);
	}
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility
struct  TerrainUtility_tDDD67DE494266AFC6E82B297619E3B84DF2CF37D  : public RuntimeObject
{
public:

public:
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility_<>c__DisplayClass4_0
struct  U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8  : public RuntimeObject
{
public:
	// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility_<>c__DisplayClass4_0::onlyAutoConnectedTerrains
	bool ___onlyAutoConnectedTerrains_0;

public:
	inline static int32_t get_offset_of_onlyAutoConnectedTerrains_0() { return static_cast<int32_t>(offsetof(U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8, ___onlyAutoConnectedTerrains_0)); }
	inline bool get_onlyAutoConnectedTerrains_0() const { return ___onlyAutoConnectedTerrains_0; }
	inline bool* get_address_of_onlyAutoConnectedTerrains_0() { return &___onlyAutoConnectedTerrains_0; }
	inline void set_onlyAutoConnectedTerrains_0(bool value)
	{
		___onlyAutoConnectedTerrains_0 = value;
	}
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility_<>c__DisplayClass4_1
struct  U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696  : public RuntimeObject
{
public:
	// UnityEngine.Terrain UnityEngine.Experimental.TerrainAPI.TerrainUtility_<>c__DisplayClass4_1::t
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___t_0;
	// UnityEngine.Experimental.TerrainAPI.TerrainUtility_<>c__DisplayClass4_0 UnityEngine.Experimental.TerrainAPI.TerrainUtility_<>c__DisplayClass4_1::CSU24<>8__locals1
	U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8 * ___CSU24U3CU3E8__locals1_1;

public:
	inline static int32_t get_offset_of_t_0() { return static_cast<int32_t>(offsetof(U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696, ___t_0)); }
	inline Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * get_t_0() const { return ___t_0; }
	inline Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 ** get_address_of_t_0() { return &___t_0; }
	inline void set_t_0(Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * value)
	{
		___t_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___t_0), (void*)value);
	}

	inline static int32_t get_offset_of_CSU24U3CU3E8__locals1_1() { return static_cast<int32_t>(offsetof(U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696, ___CSU24U3CU3E8__locals1_1)); }
	inline U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8 * get_CSU24U3CU3E8__locals1_1() const { return ___CSU24U3CU3E8__locals1_1; }
	inline U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8 ** get_address_of_CSU24U3CU3E8__locals1_1() { return &___CSU24U3CU3E8__locals1_1; }
	inline void set_CSU24U3CU3E8__locals1_1(U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8 * value)
	{
		___CSU24U3CU3E8__locals1_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___CSU24U3CU3E8__locals1_1), (void*)value);
	}
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_<>c__DisplayClass4_0
struct  U3CU3Ec__DisplayClass4_0_tBEB3CC092598F0D16C66B724FF1AE52EF06C0A8F  : public RuntimeObject
{
public:
	// System.Int32 UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_<>c__DisplayClass4_0::groupID
	int32_t ___groupID_0;

public:
	inline static int32_t get_offset_of_groupID_0() { return static_cast<int32_t>(offsetof(U3CU3Ec__DisplayClass4_0_tBEB3CC092598F0D16C66B724FF1AE52EF06C0A8F, ___groupID_0)); }
	inline int32_t get_groupID_0() const { return ___groupID_0; }
	inline int32_t* get_address_of_groupID_0() { return &___groupID_0; }
	inline void set_groupID_0(int32_t value)
	{
		___groupID_0 = value;
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


// System.Collections.Generic.KeyValuePair`2<System.Int32,System.Object>
struct  KeyValuePair_2_t56E20A5489EE435FD8BBE3EFACF6219A626E04C0 
{
public:
	// TKey System.Collections.Generic.KeyValuePair`2::key
	int32_t ___key_0;
	// TValue System.Collections.Generic.KeyValuePair`2::value
	RuntimeObject * ___value_1;

public:
	inline static int32_t get_offset_of_key_0() { return static_cast<int32_t>(offsetof(KeyValuePair_2_t56E20A5489EE435FD8BBE3EFACF6219A626E04C0, ___key_0)); }
	inline int32_t get_key_0() const { return ___key_0; }
	inline int32_t* get_address_of_key_0() { return &___key_0; }
	inline void set_key_0(int32_t value)
	{
		___key_0 = value;
	}

	inline static int32_t get_offset_of_value_1() { return static_cast<int32_t>(offsetof(KeyValuePair_2_t56E20A5489EE435FD8BBE3EFACF6219A626E04C0, ___value_1)); }
	inline RuntimeObject * get_value_1() const { return ___value_1; }
	inline RuntimeObject ** get_address_of_value_1() { return &___value_1; }
	inline void set_value_1(RuntimeObject * value)
	{
		___value_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___value_1), (void*)value);
	}
};


// System.Collections.Generic.KeyValuePair`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap>
struct  KeyValuePair_2_t4D2678F5DF760772ED05557D6F41001894311AD9 
{
public:
	// TKey System.Collections.Generic.KeyValuePair`2::key
	int32_t ___key_0;
	// TValue System.Collections.Generic.KeyValuePair`2::value
	TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * ___value_1;

public:
	inline static int32_t get_offset_of_key_0() { return static_cast<int32_t>(offsetof(KeyValuePair_2_t4D2678F5DF760772ED05557D6F41001894311AD9, ___key_0)); }
	inline int32_t get_key_0() const { return ___key_0; }
	inline int32_t* get_address_of_key_0() { return &___key_0; }
	inline void set_key_0(int32_t value)
	{
		___key_0 = value;
	}

	inline static int32_t get_offset_of_value_1() { return static_cast<int32_t>(offsetof(KeyValuePair_2_t4D2678F5DF760772ED05557D6F41001894311AD9, ___value_1)); }
	inline TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * get_value_1() const { return ___value_1; }
	inline TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 ** get_address_of_value_1() { return &___value_1; }
	inline void set_value_1(TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * value)
	{
		___value_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___value_1), (void*)value);
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


// System.Single
struct  Single_tE07797BA3C98D4CA9B5A19413C19A76688AB899E 
{
public:
	// System.Single System.Single::m_value
	float ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Single_tE07797BA3C98D4CA9B5A19413C19A76688AB899E, ___m_value_0)); }
	inline float get_m_value_0() const { return ___m_value_0; }
	inline float* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(float value)
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


// UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainGroups
struct  TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288  : public Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725
{
public:

public:
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TileCoord
struct  TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901 
{
public:
	// System.Int32 UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TileCoord::tileX
	int32_t ___tileX_0;
	// System.Int32 UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TileCoord::tileZ
	int32_t ___tileZ_1;

public:
	inline static int32_t get_offset_of_tileX_0() { return static_cast<int32_t>(offsetof(TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901, ___tileX_0)); }
	inline int32_t get_tileX_0() const { return ___tileX_0; }
	inline int32_t* get_address_of_tileX_0() { return &___tileX_0; }
	inline void set_tileX_0(int32_t value)
	{
		___tileX_0 = value;
	}

	inline static int32_t get_offset_of_tileZ_1() { return static_cast<int32_t>(offsetof(TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901, ___tileZ_1)); }
	inline int32_t get_tileZ_1() const { return ___tileZ_1; }
	inline int32_t* get_address_of_tileZ_1() { return &___tileZ_1; }
	inline void set_tileZ_1(int32_t value)
	{
		___tileZ_1 = value;
	}
};


// UnityEngine.RectInt
struct  RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 
{
public:
	// System.Int32 UnityEngine.RectInt::m_XMin
	int32_t ___m_XMin_0;
	// System.Int32 UnityEngine.RectInt::m_YMin
	int32_t ___m_YMin_1;
	// System.Int32 UnityEngine.RectInt::m_Width
	int32_t ___m_Width_2;
	// System.Int32 UnityEngine.RectInt::m_Height
	int32_t ___m_Height_3;

public:
	inline static int32_t get_offset_of_m_XMin_0() { return static_cast<int32_t>(offsetof(RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49, ___m_XMin_0)); }
	inline int32_t get_m_XMin_0() const { return ___m_XMin_0; }
	inline int32_t* get_address_of_m_XMin_0() { return &___m_XMin_0; }
	inline void set_m_XMin_0(int32_t value)
	{
		___m_XMin_0 = value;
	}

	inline static int32_t get_offset_of_m_YMin_1() { return static_cast<int32_t>(offsetof(RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49, ___m_YMin_1)); }
	inline int32_t get_m_YMin_1() const { return ___m_YMin_1; }
	inline int32_t* get_address_of_m_YMin_1() { return &___m_YMin_1; }
	inline void set_m_YMin_1(int32_t value)
	{
		___m_YMin_1 = value;
	}

	inline static int32_t get_offset_of_m_Width_2() { return static_cast<int32_t>(offsetof(RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49, ___m_Width_2)); }
	inline int32_t get_m_Width_2() const { return ___m_Width_2; }
	inline int32_t* get_address_of_m_Width_2() { return &___m_Width_2; }
	inline void set_m_Width_2(int32_t value)
	{
		___m_Width_2 = value;
	}

	inline static int32_t get_offset_of_m_Height_3() { return static_cast<int32_t>(offsetof(RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49, ___m_Height_3)); }
	inline int32_t get_m_Height_3() const { return ___m_Height_3; }
	inline int32_t* get_address_of_m_Height_3() { return &___m_Height_3; }
	inline void set_m_Height_3(int32_t value)
	{
		___m_Height_3 = value;
	}
};


// UnityEngine.Vector2
struct  Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 
{
public:
	// System.Single UnityEngine.Vector2::x
	float ___x_0;
	// System.Single UnityEngine.Vector2::y
	float ___y_1;

public:
	inline static int32_t get_offset_of_x_0() { return static_cast<int32_t>(offsetof(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9, ___x_0)); }
	inline float get_x_0() const { return ___x_0; }
	inline float* get_address_of_x_0() { return &___x_0; }
	inline void set_x_0(float value)
	{
		___x_0 = value;
	}

	inline static int32_t get_offset_of_y_1() { return static_cast<int32_t>(offsetof(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9, ___y_1)); }
	inline float get_y_1() const { return ___y_1; }
	inline float* get_address_of_y_1() { return &___y_1; }
	inline void set_y_1(float value)
	{
		___y_1 = value;
	}
};

struct Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9_StaticFields
{
public:
	// UnityEngine.Vector2 UnityEngine.Vector2::zeroVector
	Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___zeroVector_2;
	// UnityEngine.Vector2 UnityEngine.Vector2::oneVector
	Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___oneVector_3;
	// UnityEngine.Vector2 UnityEngine.Vector2::upVector
	Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___upVector_4;
	// UnityEngine.Vector2 UnityEngine.Vector2::downVector
	Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___downVector_5;
	// UnityEngine.Vector2 UnityEngine.Vector2::leftVector
	Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___leftVector_6;
	// UnityEngine.Vector2 UnityEngine.Vector2::rightVector
	Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___rightVector_7;
	// UnityEngine.Vector2 UnityEngine.Vector2::positiveInfinityVector
	Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___positiveInfinityVector_8;
	// UnityEngine.Vector2 UnityEngine.Vector2::negativeInfinityVector
	Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___negativeInfinityVector_9;

public:
	inline static int32_t get_offset_of_zeroVector_2() { return static_cast<int32_t>(offsetof(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9_StaticFields, ___zeroVector_2)); }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  get_zeroVector_2() const { return ___zeroVector_2; }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 * get_address_of_zeroVector_2() { return &___zeroVector_2; }
	inline void set_zeroVector_2(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  value)
	{
		___zeroVector_2 = value;
	}

	inline static int32_t get_offset_of_oneVector_3() { return static_cast<int32_t>(offsetof(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9_StaticFields, ___oneVector_3)); }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  get_oneVector_3() const { return ___oneVector_3; }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 * get_address_of_oneVector_3() { return &___oneVector_3; }
	inline void set_oneVector_3(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  value)
	{
		___oneVector_3 = value;
	}

	inline static int32_t get_offset_of_upVector_4() { return static_cast<int32_t>(offsetof(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9_StaticFields, ___upVector_4)); }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  get_upVector_4() const { return ___upVector_4; }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 * get_address_of_upVector_4() { return &___upVector_4; }
	inline void set_upVector_4(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  value)
	{
		___upVector_4 = value;
	}

	inline static int32_t get_offset_of_downVector_5() { return static_cast<int32_t>(offsetof(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9_StaticFields, ___downVector_5)); }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  get_downVector_5() const { return ___downVector_5; }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 * get_address_of_downVector_5() { return &___downVector_5; }
	inline void set_downVector_5(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  value)
	{
		___downVector_5 = value;
	}

	inline static int32_t get_offset_of_leftVector_6() { return static_cast<int32_t>(offsetof(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9_StaticFields, ___leftVector_6)); }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  get_leftVector_6() const { return ___leftVector_6; }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 * get_address_of_leftVector_6() { return &___leftVector_6; }
	inline void set_leftVector_6(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  value)
	{
		___leftVector_6 = value;
	}

	inline static int32_t get_offset_of_rightVector_7() { return static_cast<int32_t>(offsetof(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9_StaticFields, ___rightVector_7)); }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  get_rightVector_7() const { return ___rightVector_7; }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 * get_address_of_rightVector_7() { return &___rightVector_7; }
	inline void set_rightVector_7(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  value)
	{
		___rightVector_7 = value;
	}

	inline static int32_t get_offset_of_positiveInfinityVector_8() { return static_cast<int32_t>(offsetof(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9_StaticFields, ___positiveInfinityVector_8)); }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  get_positiveInfinityVector_8() const { return ___positiveInfinityVector_8; }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 * get_address_of_positiveInfinityVector_8() { return &___positiveInfinityVector_8; }
	inline void set_positiveInfinityVector_8(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  value)
	{
		___positiveInfinityVector_8 = value;
	}

	inline static int32_t get_offset_of_negativeInfinityVector_9() { return static_cast<int32_t>(offsetof(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9_StaticFields, ___negativeInfinityVector_9)); }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  get_negativeInfinityVector_9() const { return ___negativeInfinityVector_9; }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 * get_address_of_negativeInfinityVector_9() { return &___negativeInfinityVector_9; }
	inline void set_negativeInfinityVector_9(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  value)
	{
		___negativeInfinityVector_9 = value;
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


// System.Collections.Generic.Dictionary`2_Enumerator<System.Int32,System.Object>
struct  Enumerator_t1AD96AD2810CD9FF13D02CD49EC9D4D447C1485C 
{
public:
	// System.Collections.Generic.Dictionary`2<TKey,TValue> System.Collections.Generic.Dictionary`2_Enumerator::dictionary
	Dictionary_2_tE1E5B6327FFA2C7AE34A69E0011815C914771C2F * ___dictionary_0;
	// System.Int32 System.Collections.Generic.Dictionary`2_Enumerator::version
	int32_t ___version_1;
	// System.Int32 System.Collections.Generic.Dictionary`2_Enumerator::index
	int32_t ___index_2;
	// System.Collections.Generic.KeyValuePair`2<TKey,TValue> System.Collections.Generic.Dictionary`2_Enumerator::current
	KeyValuePair_2_t56E20A5489EE435FD8BBE3EFACF6219A626E04C0  ___current_3;
	// System.Int32 System.Collections.Generic.Dictionary`2_Enumerator::getEnumeratorRetType
	int32_t ___getEnumeratorRetType_4;

public:
	inline static int32_t get_offset_of_dictionary_0() { return static_cast<int32_t>(offsetof(Enumerator_t1AD96AD2810CD9FF13D02CD49EC9D4D447C1485C, ___dictionary_0)); }
	inline Dictionary_2_tE1E5B6327FFA2C7AE34A69E0011815C914771C2F * get_dictionary_0() const { return ___dictionary_0; }
	inline Dictionary_2_tE1E5B6327FFA2C7AE34A69E0011815C914771C2F ** get_address_of_dictionary_0() { return &___dictionary_0; }
	inline void set_dictionary_0(Dictionary_2_tE1E5B6327FFA2C7AE34A69E0011815C914771C2F * value)
	{
		___dictionary_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dictionary_0), (void*)value);
	}

	inline static int32_t get_offset_of_version_1() { return static_cast<int32_t>(offsetof(Enumerator_t1AD96AD2810CD9FF13D02CD49EC9D4D447C1485C, ___version_1)); }
	inline int32_t get_version_1() const { return ___version_1; }
	inline int32_t* get_address_of_version_1() { return &___version_1; }
	inline void set_version_1(int32_t value)
	{
		___version_1 = value;
	}

	inline static int32_t get_offset_of_index_2() { return static_cast<int32_t>(offsetof(Enumerator_t1AD96AD2810CD9FF13D02CD49EC9D4D447C1485C, ___index_2)); }
	inline int32_t get_index_2() const { return ___index_2; }
	inline int32_t* get_address_of_index_2() { return &___index_2; }
	inline void set_index_2(int32_t value)
	{
		___index_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_t1AD96AD2810CD9FF13D02CD49EC9D4D447C1485C, ___current_3)); }
	inline KeyValuePair_2_t56E20A5489EE435FD8BBE3EFACF6219A626E04C0  get_current_3() const { return ___current_3; }
	inline KeyValuePair_2_t56E20A5489EE435FD8BBE3EFACF6219A626E04C0 * get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(KeyValuePair_2_t56E20A5489EE435FD8BBE3EFACF6219A626E04C0  value)
	{
		___current_3 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___current_3))->___value_1), (void*)NULL);
	}

	inline static int32_t get_offset_of_getEnumeratorRetType_4() { return static_cast<int32_t>(offsetof(Enumerator_t1AD96AD2810CD9FF13D02CD49EC9D4D447C1485C, ___getEnumeratorRetType_4)); }
	inline int32_t get_getEnumeratorRetType_4() const { return ___getEnumeratorRetType_4; }
	inline int32_t* get_address_of_getEnumeratorRetType_4() { return &___getEnumeratorRetType_4; }
	inline void set_getEnumeratorRetType_4(int32_t value)
	{
		___getEnumeratorRetType_4 = value;
	}
};


// System.Collections.Generic.Dictionary`2_Enumerator<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap>
struct  Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C 
{
public:
	// System.Collections.Generic.Dictionary`2<TKey,TValue> System.Collections.Generic.Dictionary`2_Enumerator::dictionary
	Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725 * ___dictionary_0;
	// System.Int32 System.Collections.Generic.Dictionary`2_Enumerator::version
	int32_t ___version_1;
	// System.Int32 System.Collections.Generic.Dictionary`2_Enumerator::index
	int32_t ___index_2;
	// System.Collections.Generic.KeyValuePair`2<TKey,TValue> System.Collections.Generic.Dictionary`2_Enumerator::current
	KeyValuePair_2_t4D2678F5DF760772ED05557D6F41001894311AD9  ___current_3;
	// System.Int32 System.Collections.Generic.Dictionary`2_Enumerator::getEnumeratorRetType
	int32_t ___getEnumeratorRetType_4;

public:
	inline static int32_t get_offset_of_dictionary_0() { return static_cast<int32_t>(offsetof(Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C, ___dictionary_0)); }
	inline Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725 * get_dictionary_0() const { return ___dictionary_0; }
	inline Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725 ** get_address_of_dictionary_0() { return &___dictionary_0; }
	inline void set_dictionary_0(Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725 * value)
	{
		___dictionary_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dictionary_0), (void*)value);
	}

	inline static int32_t get_offset_of_version_1() { return static_cast<int32_t>(offsetof(Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C, ___version_1)); }
	inline int32_t get_version_1() const { return ___version_1; }
	inline int32_t* get_address_of_version_1() { return &___version_1; }
	inline void set_version_1(int32_t value)
	{
		___version_1 = value;
	}

	inline static int32_t get_offset_of_index_2() { return static_cast<int32_t>(offsetof(Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C, ___index_2)); }
	inline int32_t get_index_2() const { return ___index_2; }
	inline int32_t* get_address_of_index_2() { return &___index_2; }
	inline void set_index_2(int32_t value)
	{
		___index_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C, ___current_3)); }
	inline KeyValuePair_2_t4D2678F5DF760772ED05557D6F41001894311AD9  get_current_3() const { return ___current_3; }
	inline KeyValuePair_2_t4D2678F5DF760772ED05557D6F41001894311AD9 * get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(KeyValuePair_2_t4D2678F5DF760772ED05557D6F41001894311AD9  value)
	{
		___current_3 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___current_3))->___value_1), (void*)NULL);
	}

	inline static int32_t get_offset_of_getEnumeratorRetType_4() { return static_cast<int32_t>(offsetof(Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C, ___getEnumeratorRetType_4)); }
	inline int32_t get_getEnumeratorRetType_4() const { return ___getEnumeratorRetType_4; }
	inline int32_t* get_address_of_getEnumeratorRetType_4() { return &___getEnumeratorRetType_4; }
	inline void set_getEnumeratorRetType_4(int32_t value)
	{
		___getEnumeratorRetType_4 = value;
	}
};


// System.Collections.Generic.Dictionary`2_KeyCollection_Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TileCoord,System.Object>
struct  Enumerator_t914132A1A7488464276E36385DFE4C80616AB5E9 
{
public:
	// System.Collections.Generic.Dictionary`2<TKey,TValue> System.Collections.Generic.Dictionary`2_KeyCollection_Enumerator::dictionary
	Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C * ___dictionary_0;
	// System.Int32 System.Collections.Generic.Dictionary`2_KeyCollection_Enumerator::index
	int32_t ___index_1;
	// System.Int32 System.Collections.Generic.Dictionary`2_KeyCollection_Enumerator::version
	int32_t ___version_2;
	// TKey System.Collections.Generic.Dictionary`2_KeyCollection_Enumerator::currentKey
	TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  ___currentKey_3;

public:
	inline static int32_t get_offset_of_dictionary_0() { return static_cast<int32_t>(offsetof(Enumerator_t914132A1A7488464276E36385DFE4C80616AB5E9, ___dictionary_0)); }
	inline Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C * get_dictionary_0() const { return ___dictionary_0; }
	inline Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C ** get_address_of_dictionary_0() { return &___dictionary_0; }
	inline void set_dictionary_0(Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C * value)
	{
		___dictionary_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dictionary_0), (void*)value);
	}

	inline static int32_t get_offset_of_index_1() { return static_cast<int32_t>(offsetof(Enumerator_t914132A1A7488464276E36385DFE4C80616AB5E9, ___index_1)); }
	inline int32_t get_index_1() const { return ___index_1; }
	inline int32_t* get_address_of_index_1() { return &___index_1; }
	inline void set_index_1(int32_t value)
	{
		___index_1 = value;
	}

	inline static int32_t get_offset_of_version_2() { return static_cast<int32_t>(offsetof(Enumerator_t914132A1A7488464276E36385DFE4C80616AB5E9, ___version_2)); }
	inline int32_t get_version_2() const { return ___version_2; }
	inline int32_t* get_address_of_version_2() { return &___version_2; }
	inline void set_version_2(int32_t value)
	{
		___version_2 = value;
	}

	inline static int32_t get_offset_of_currentKey_3() { return static_cast<int32_t>(offsetof(Enumerator_t914132A1A7488464276E36385DFE4C80616AB5E9, ___currentKey_3)); }
	inline TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  get_currentKey_3() const { return ___currentKey_3; }
	inline TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901 * get_address_of_currentKey_3() { return &___currentKey_3; }
	inline void set_currentKey_3(TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  value)
	{
		___currentKey_3 = value;
	}
};


// System.Collections.Generic.Dictionary`2_KeyCollection_Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TileCoord,UnityEngine.Terrain>
struct  Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E 
{
public:
	// System.Collections.Generic.Dictionary`2<TKey,TValue> System.Collections.Generic.Dictionary`2_KeyCollection_Enumerator::dictionary
	Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * ___dictionary_0;
	// System.Int32 System.Collections.Generic.Dictionary`2_KeyCollection_Enumerator::index
	int32_t ___index_1;
	// System.Int32 System.Collections.Generic.Dictionary`2_KeyCollection_Enumerator::version
	int32_t ___version_2;
	// TKey System.Collections.Generic.Dictionary`2_KeyCollection_Enumerator::currentKey
	TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  ___currentKey_3;

public:
	inline static int32_t get_offset_of_dictionary_0() { return static_cast<int32_t>(offsetof(Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E, ___dictionary_0)); }
	inline Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * get_dictionary_0() const { return ___dictionary_0; }
	inline Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C ** get_address_of_dictionary_0() { return &___dictionary_0; }
	inline void set_dictionary_0(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * value)
	{
		___dictionary_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dictionary_0), (void*)value);
	}

	inline static int32_t get_offset_of_index_1() { return static_cast<int32_t>(offsetof(Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E, ___index_1)); }
	inline int32_t get_index_1() const { return ___index_1; }
	inline int32_t* get_address_of_index_1() { return &___index_1; }
	inline void set_index_1(int32_t value)
	{
		___index_1 = value;
	}

	inline static int32_t get_offset_of_version_2() { return static_cast<int32_t>(offsetof(Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E, ___version_2)); }
	inline int32_t get_version_2() const { return ___version_2; }
	inline int32_t* get_address_of_version_2() { return &___version_2; }
	inline void set_version_2(int32_t value)
	{
		___version_2 = value;
	}

	inline static int32_t get_offset_of_currentKey_3() { return static_cast<int32_t>(offsetof(Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E, ___currentKey_3)); }
	inline TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  get_currentKey_3() const { return ___currentKey_3; }
	inline TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901 * get_address_of_currentKey_3() { return &___currentKey_3; }
	inline void set_currentKey_3(TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  value)
	{
		___currentKey_3 = value;
	}
};


// System.Collections.Generic.KeyValuePair`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TileCoord,System.Object>
struct  KeyValuePair_2_tCBAAE4FBE6091373C1916EE17527311382CF4551 
{
public:
	// TKey System.Collections.Generic.KeyValuePair`2::key
	TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  ___key_0;
	// TValue System.Collections.Generic.KeyValuePair`2::value
	RuntimeObject * ___value_1;

public:
	inline static int32_t get_offset_of_key_0() { return static_cast<int32_t>(offsetof(KeyValuePair_2_tCBAAE4FBE6091373C1916EE17527311382CF4551, ___key_0)); }
	inline TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  get_key_0() const { return ___key_0; }
	inline TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901 * get_address_of_key_0() { return &___key_0; }
	inline void set_key_0(TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  value)
	{
		___key_0 = value;
	}

	inline static int32_t get_offset_of_value_1() { return static_cast<int32_t>(offsetof(KeyValuePair_2_tCBAAE4FBE6091373C1916EE17527311382CF4551, ___value_1)); }
	inline RuntimeObject * get_value_1() const { return ___value_1; }
	inline RuntimeObject ** get_address_of_value_1() { return &___value_1; }
	inline void set_value_1(RuntimeObject * value)
	{
		___value_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___value_1), (void*)value);
	}
};


// System.Collections.Generic.KeyValuePair`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TileCoord,UnityEngine.Terrain>
struct  KeyValuePair_2_t41025882EB77BC66901763733CF9F6733F84B2CD 
{
public:
	// TKey System.Collections.Generic.KeyValuePair`2::key
	TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  ___key_0;
	// TValue System.Collections.Generic.KeyValuePair`2::value
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___value_1;

public:
	inline static int32_t get_offset_of_key_0() { return static_cast<int32_t>(offsetof(KeyValuePair_2_t41025882EB77BC66901763733CF9F6733F84B2CD, ___key_0)); }
	inline TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  get_key_0() const { return ___key_0; }
	inline TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901 * get_address_of_key_0() { return &___key_0; }
	inline void set_key_0(TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  value)
	{
		___key_0 = value;
	}

	inline static int32_t get_offset_of_value_1() { return static_cast<int32_t>(offsetof(KeyValuePair_2_t41025882EB77BC66901763733CF9F6733F84B2CD, ___value_1)); }
	inline Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * get_value_1() const { return ___value_1; }
	inline Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 ** get_address_of_value_1() { return &___value_1; }
	inline void set_value_1(Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * value)
	{
		___value_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___value_1), (void*)value);
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

// UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_ErrorCode
struct  ErrorCode_t5533C7D1F39FAA2C0E95C82A736DF461B0B2FCE6 
{
public:
	// System.Int32 UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_ErrorCode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(ErrorCode_t5533C7D1F39FAA2C0E95C82A736DF461B0B2FCE6, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
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

// UnityEngine.TerrainData_BoundaryValueType
struct  BoundaryValueType_t5B5317FD7A95A68B0FA9B3DD30EB5CF9E3E6883D 
{
public:
	// System.Int32 UnityEngine.TerrainData_BoundaryValueType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(BoundaryValueType_t5B5317FD7A95A68B0FA9B3DD30EB5CF9E3E6883D, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// System.Collections.Generic.Dictionary`2_Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TileCoord,System.Object>
struct  Enumerator_t81F0095C2D5C396203071D667A6252EFAB3D76D6 
{
public:
	// System.Collections.Generic.Dictionary`2<TKey,TValue> System.Collections.Generic.Dictionary`2_Enumerator::dictionary
	Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C * ___dictionary_0;
	// System.Int32 System.Collections.Generic.Dictionary`2_Enumerator::version
	int32_t ___version_1;
	// System.Int32 System.Collections.Generic.Dictionary`2_Enumerator::index
	int32_t ___index_2;
	// System.Collections.Generic.KeyValuePair`2<TKey,TValue> System.Collections.Generic.Dictionary`2_Enumerator::current
	KeyValuePair_2_tCBAAE4FBE6091373C1916EE17527311382CF4551  ___current_3;
	// System.Int32 System.Collections.Generic.Dictionary`2_Enumerator::getEnumeratorRetType
	int32_t ___getEnumeratorRetType_4;

public:
	inline static int32_t get_offset_of_dictionary_0() { return static_cast<int32_t>(offsetof(Enumerator_t81F0095C2D5C396203071D667A6252EFAB3D76D6, ___dictionary_0)); }
	inline Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C * get_dictionary_0() const { return ___dictionary_0; }
	inline Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C ** get_address_of_dictionary_0() { return &___dictionary_0; }
	inline void set_dictionary_0(Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C * value)
	{
		___dictionary_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dictionary_0), (void*)value);
	}

	inline static int32_t get_offset_of_version_1() { return static_cast<int32_t>(offsetof(Enumerator_t81F0095C2D5C396203071D667A6252EFAB3D76D6, ___version_1)); }
	inline int32_t get_version_1() const { return ___version_1; }
	inline int32_t* get_address_of_version_1() { return &___version_1; }
	inline void set_version_1(int32_t value)
	{
		___version_1 = value;
	}

	inline static int32_t get_offset_of_index_2() { return static_cast<int32_t>(offsetof(Enumerator_t81F0095C2D5C396203071D667A6252EFAB3D76D6, ___index_2)); }
	inline int32_t get_index_2() const { return ___index_2; }
	inline int32_t* get_address_of_index_2() { return &___index_2; }
	inline void set_index_2(int32_t value)
	{
		___index_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_t81F0095C2D5C396203071D667A6252EFAB3D76D6, ___current_3)); }
	inline KeyValuePair_2_tCBAAE4FBE6091373C1916EE17527311382CF4551  get_current_3() const { return ___current_3; }
	inline KeyValuePair_2_tCBAAE4FBE6091373C1916EE17527311382CF4551 * get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(KeyValuePair_2_tCBAAE4FBE6091373C1916EE17527311382CF4551  value)
	{
		___current_3 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___current_3))->___value_1), (void*)NULL);
	}

	inline static int32_t get_offset_of_getEnumeratorRetType_4() { return static_cast<int32_t>(offsetof(Enumerator_t81F0095C2D5C396203071D667A6252EFAB3D76D6, ___getEnumeratorRetType_4)); }
	inline int32_t get_getEnumeratorRetType_4() const { return ___getEnumeratorRetType_4; }
	inline int32_t* get_address_of_getEnumeratorRetType_4() { return &___getEnumeratorRetType_4; }
	inline void set_getEnumeratorRetType_4(int32_t value)
	{
		___getEnumeratorRetType_4 = value;
	}
};


// System.Collections.Generic.Dictionary`2_Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TileCoord,UnityEngine.Terrain>
struct  Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99 
{
public:
	// System.Collections.Generic.Dictionary`2<TKey,TValue> System.Collections.Generic.Dictionary`2_Enumerator::dictionary
	Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * ___dictionary_0;
	// System.Int32 System.Collections.Generic.Dictionary`2_Enumerator::version
	int32_t ___version_1;
	// System.Int32 System.Collections.Generic.Dictionary`2_Enumerator::index
	int32_t ___index_2;
	// System.Collections.Generic.KeyValuePair`2<TKey,TValue> System.Collections.Generic.Dictionary`2_Enumerator::current
	KeyValuePair_2_t41025882EB77BC66901763733CF9F6733F84B2CD  ___current_3;
	// System.Int32 System.Collections.Generic.Dictionary`2_Enumerator::getEnumeratorRetType
	int32_t ___getEnumeratorRetType_4;

public:
	inline static int32_t get_offset_of_dictionary_0() { return static_cast<int32_t>(offsetof(Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99, ___dictionary_0)); }
	inline Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * get_dictionary_0() const { return ___dictionary_0; }
	inline Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C ** get_address_of_dictionary_0() { return &___dictionary_0; }
	inline void set_dictionary_0(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * value)
	{
		___dictionary_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dictionary_0), (void*)value);
	}

	inline static int32_t get_offset_of_version_1() { return static_cast<int32_t>(offsetof(Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99, ___version_1)); }
	inline int32_t get_version_1() const { return ___version_1; }
	inline int32_t* get_address_of_version_1() { return &___version_1; }
	inline void set_version_1(int32_t value)
	{
		___version_1 = value;
	}

	inline static int32_t get_offset_of_index_2() { return static_cast<int32_t>(offsetof(Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99, ___index_2)); }
	inline int32_t get_index_2() const { return ___index_2; }
	inline int32_t* get_address_of_index_2() { return &___index_2; }
	inline void set_index_2(int32_t value)
	{
		___index_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99, ___current_3)); }
	inline KeyValuePair_2_t41025882EB77BC66901763733CF9F6733F84B2CD  get_current_3() const { return ___current_3; }
	inline KeyValuePair_2_t41025882EB77BC66901763733CF9F6733F84B2CD * get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(KeyValuePair_2_t41025882EB77BC66901763733CF9F6733F84B2CD  value)
	{
		___current_3 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___current_3))->___value_1), (void*)NULL);
	}

	inline static int32_t get_offset_of_getEnumeratorRetType_4() { return static_cast<int32_t>(offsetof(Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99, ___getEnumeratorRetType_4)); }
	inline int32_t get_getEnumeratorRetType_4() const { return ___getEnumeratorRetType_4; }
	inline int32_t* get_address_of_getEnumeratorRetType_4() { return &___getEnumeratorRetType_4; }
	inline void set_getEnumeratorRetType_4(int32_t value)
	{
		___getEnumeratorRetType_4 = value;
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

// UnityEngine.Component
struct  Component_t62FBC8D2420DA4BE9037AFE430740F6B3EECA684  : public Object_tF2F3778131EFF286AF62B7B013A170F95A91571A
{
public:

public:
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap
struct  TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453  : public RuntimeObject
{
public:
	// UnityEngine.Vector3 UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap::m_patchSize
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___m_patchSize_0;
	// UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_ErrorCode UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap::m_errorCode
	int32_t ___m_errorCode_1;
	// System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TileCoord,UnityEngine.Terrain> UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap::m_terrainTiles
	Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * ___m_terrainTiles_2;

public:
	inline static int32_t get_offset_of_m_patchSize_0() { return static_cast<int32_t>(offsetof(TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453, ___m_patchSize_0)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_m_patchSize_0() const { return ___m_patchSize_0; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_m_patchSize_0() { return &___m_patchSize_0; }
	inline void set_m_patchSize_0(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___m_patchSize_0 = value;
	}

	inline static int32_t get_offset_of_m_errorCode_1() { return static_cast<int32_t>(offsetof(TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453, ___m_errorCode_1)); }
	inline int32_t get_m_errorCode_1() const { return ___m_errorCode_1; }
	inline int32_t* get_address_of_m_errorCode_1() { return &___m_errorCode_1; }
	inline void set_m_errorCode_1(int32_t value)
	{
		___m_errorCode_1 = value;
	}

	inline static int32_t get_offset_of_m_terrainTiles_2() { return static_cast<int32_t>(offsetof(TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453, ___m_terrainTiles_2)); }
	inline Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * get_m_terrainTiles_2() const { return ___m_terrainTiles_2; }
	inline Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C ** get_address_of_m_terrainTiles_2() { return &___m_terrainTiles_2; }
	inline void set_m_terrainTiles_2(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * value)
	{
		___m_terrainTiles_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_terrainTiles_2), (void*)value);
	}
};


// UnityEngine.TerrainData
struct  TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4  : public Object_tF2F3778131EFF286AF62B7B013A170F95A91571A
{
public:

public:
};

struct TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields
{
public:
	// System.Int32 UnityEngine.TerrainData::k_MaximumResolution
	int32_t ___k_MaximumResolution_4;
	// System.Int32 UnityEngine.TerrainData::k_MinimumDetailResolutionPerPatch
	int32_t ___k_MinimumDetailResolutionPerPatch_5;
	// System.Int32 UnityEngine.TerrainData::k_MaximumDetailResolutionPerPatch
	int32_t ___k_MaximumDetailResolutionPerPatch_6;
	// System.Int32 UnityEngine.TerrainData::k_MaximumDetailPatchCount
	int32_t ___k_MaximumDetailPatchCount_7;
	// System.Int32 UnityEngine.TerrainData::k_MaximumDetailsPerRes
	int32_t ___k_MaximumDetailsPerRes_8;
	// System.Int32 UnityEngine.TerrainData::k_MinimumAlphamapResolution
	int32_t ___k_MinimumAlphamapResolution_9;
	// System.Int32 UnityEngine.TerrainData::k_MaximumAlphamapResolution
	int32_t ___k_MaximumAlphamapResolution_10;
	// System.Int32 UnityEngine.TerrainData::k_MinimumBaseMapResolution
	int32_t ___k_MinimumBaseMapResolution_11;
	// System.Int32 UnityEngine.TerrainData::k_MaximumBaseMapResolution
	int32_t ___k_MaximumBaseMapResolution_12;

public:
	inline static int32_t get_offset_of_k_MaximumResolution_4() { return static_cast<int32_t>(offsetof(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields, ___k_MaximumResolution_4)); }
	inline int32_t get_k_MaximumResolution_4() const { return ___k_MaximumResolution_4; }
	inline int32_t* get_address_of_k_MaximumResolution_4() { return &___k_MaximumResolution_4; }
	inline void set_k_MaximumResolution_4(int32_t value)
	{
		___k_MaximumResolution_4 = value;
	}

	inline static int32_t get_offset_of_k_MinimumDetailResolutionPerPatch_5() { return static_cast<int32_t>(offsetof(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields, ___k_MinimumDetailResolutionPerPatch_5)); }
	inline int32_t get_k_MinimumDetailResolutionPerPatch_5() const { return ___k_MinimumDetailResolutionPerPatch_5; }
	inline int32_t* get_address_of_k_MinimumDetailResolutionPerPatch_5() { return &___k_MinimumDetailResolutionPerPatch_5; }
	inline void set_k_MinimumDetailResolutionPerPatch_5(int32_t value)
	{
		___k_MinimumDetailResolutionPerPatch_5 = value;
	}

	inline static int32_t get_offset_of_k_MaximumDetailResolutionPerPatch_6() { return static_cast<int32_t>(offsetof(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields, ___k_MaximumDetailResolutionPerPatch_6)); }
	inline int32_t get_k_MaximumDetailResolutionPerPatch_6() const { return ___k_MaximumDetailResolutionPerPatch_6; }
	inline int32_t* get_address_of_k_MaximumDetailResolutionPerPatch_6() { return &___k_MaximumDetailResolutionPerPatch_6; }
	inline void set_k_MaximumDetailResolutionPerPatch_6(int32_t value)
	{
		___k_MaximumDetailResolutionPerPatch_6 = value;
	}

	inline static int32_t get_offset_of_k_MaximumDetailPatchCount_7() { return static_cast<int32_t>(offsetof(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields, ___k_MaximumDetailPatchCount_7)); }
	inline int32_t get_k_MaximumDetailPatchCount_7() const { return ___k_MaximumDetailPatchCount_7; }
	inline int32_t* get_address_of_k_MaximumDetailPatchCount_7() { return &___k_MaximumDetailPatchCount_7; }
	inline void set_k_MaximumDetailPatchCount_7(int32_t value)
	{
		___k_MaximumDetailPatchCount_7 = value;
	}

	inline static int32_t get_offset_of_k_MaximumDetailsPerRes_8() { return static_cast<int32_t>(offsetof(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields, ___k_MaximumDetailsPerRes_8)); }
	inline int32_t get_k_MaximumDetailsPerRes_8() const { return ___k_MaximumDetailsPerRes_8; }
	inline int32_t* get_address_of_k_MaximumDetailsPerRes_8() { return &___k_MaximumDetailsPerRes_8; }
	inline void set_k_MaximumDetailsPerRes_8(int32_t value)
	{
		___k_MaximumDetailsPerRes_8 = value;
	}

	inline static int32_t get_offset_of_k_MinimumAlphamapResolution_9() { return static_cast<int32_t>(offsetof(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields, ___k_MinimumAlphamapResolution_9)); }
	inline int32_t get_k_MinimumAlphamapResolution_9() const { return ___k_MinimumAlphamapResolution_9; }
	inline int32_t* get_address_of_k_MinimumAlphamapResolution_9() { return &___k_MinimumAlphamapResolution_9; }
	inline void set_k_MinimumAlphamapResolution_9(int32_t value)
	{
		___k_MinimumAlphamapResolution_9 = value;
	}

	inline static int32_t get_offset_of_k_MaximumAlphamapResolution_10() { return static_cast<int32_t>(offsetof(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields, ___k_MaximumAlphamapResolution_10)); }
	inline int32_t get_k_MaximumAlphamapResolution_10() const { return ___k_MaximumAlphamapResolution_10; }
	inline int32_t* get_address_of_k_MaximumAlphamapResolution_10() { return &___k_MaximumAlphamapResolution_10; }
	inline void set_k_MaximumAlphamapResolution_10(int32_t value)
	{
		___k_MaximumAlphamapResolution_10 = value;
	}

	inline static int32_t get_offset_of_k_MinimumBaseMapResolution_11() { return static_cast<int32_t>(offsetof(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields, ___k_MinimumBaseMapResolution_11)); }
	inline int32_t get_k_MinimumBaseMapResolution_11() const { return ___k_MinimumBaseMapResolution_11; }
	inline int32_t* get_address_of_k_MinimumBaseMapResolution_11() { return &___k_MinimumBaseMapResolution_11; }
	inline void set_k_MinimumBaseMapResolution_11(int32_t value)
	{
		___k_MinimumBaseMapResolution_11 = value;
	}

	inline static int32_t get_offset_of_k_MaximumBaseMapResolution_12() { return static_cast<int32_t>(offsetof(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields, ___k_MaximumBaseMapResolution_12)); }
	inline int32_t get_k_MaximumBaseMapResolution_12() const { return ___k_MaximumBaseMapResolution_12; }
	inline int32_t* get_address_of_k_MaximumBaseMapResolution_12() { return &___k_MaximumBaseMapResolution_12; }
	inline void set_k_MaximumBaseMapResolution_12(int32_t value)
	{
		___k_MaximumBaseMapResolution_12 = value;
	}
};


// System.AsyncCallback
struct  AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Behaviour
struct  Behaviour_t1A3DDDCF73B4627928FBFE02ED52B7251777DBD9  : public Component_t62FBC8D2420DA4BE9037AFE430740F6B3EECA684
{
public:

public:
};


// UnityEngine.Experimental.TerrainAPI.TerrainCallbacks_HeightmapChangedCallback
struct  HeightmapChangedCallback_tB00DA531F9C32468E88700A5C2D55E05189E0FA0  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Experimental.TerrainAPI.TerrainCallbacks_TextureChangedCallback
struct  TextureChangedCallback_tD8BA8EA99CC9FA597E4AA143944720822EFB7D9F  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TerrainFilter
struct  TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Transform
struct  Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1  : public Component_t62FBC8D2420DA4BE9037AFE430740F6B3EECA684
{
public:

public:
};


// UnityEngine.Terrain
struct  Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836  : public Behaviour_t1A3DDDCF73B4627928FBFE02ED52B7251777DBD9
{
public:

public:
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
// UnityEngine.Terrain[]
struct TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * m_Items[1];

public:
	inline Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * value)
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


// System.Boolean System.Collections.Generic.Dictionary`2<System.Int32,System.Object>::ContainsKey(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Dictionary_2_ContainsKey_mE6DB9458466D0F98B67E2C6CAEFABBF9576AC4D7_gshared (Dictionary_2_tE1E5B6327FFA2C7AE34A69E0011815C914771C2F * __this, int32_t ___key0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int32,System.Object>::Add(!0,!1)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2_Add_m39BC00F21EE9459BB8DEF5479F95F79C5C740682_gshared (Dictionary_2_tE1E5B6327FFA2C7AE34A69E0011815C914771C2F * __this, int32_t ___key0, RuntimeObject * ___value1, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.Dictionary`2<System.Int32,System.Object>::get_Count()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Dictionary_2_get_Count_m12374F2F6F3D2DE9CBF98D3BD63CBB0DA19C69C5_gshared (Dictionary_2_tE1E5B6327FFA2C7AE34A69E0011815C914771C2F * __this, const RuntimeMethod* method);
// System.Collections.Generic.Dictionary`2/Enumerator<!0,!1> System.Collections.Generic.Dictionary`2<System.Int32,System.Object>::GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Enumerator_t1AD96AD2810CD9FF13D02CD49EC9D4D447C1485C  Dictionary_2_GetEnumerator_m17437D82A5AF502166F10DD12B5C5830DDB95444_gshared (Dictionary_2_tE1E5B6327FFA2C7AE34A69E0011815C914771C2F * __this, const RuntimeMethod* method);
// System.Collections.Generic.KeyValuePair`2<!0,!1> System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,System.Object>::get_Current()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR KeyValuePair_2_t56E20A5489EE435FD8BBE3EFACF6219A626E04C0  Enumerator_get_Current_mE5033FC555E7BC63DDC919B903A8A305C3AADBEB_gshared_inline (Enumerator_t1AD96AD2810CD9FF13D02CD49EC9D4D447C1485C * __this, const RuntimeMethod* method);
// !1 System.Collections.Generic.KeyValuePair`2<System.Int32,System.Object>::get_Value()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject * KeyValuePair_2_get_Value_mC1E2EFCF98529D0550A547CF87C6EAB6821741BF_gshared_inline (KeyValuePair_2_t56E20A5489EE435FD8BBE3EFACF6219A626E04C0 * __this, const RuntimeMethod* method);
// System.Collections.Generic.Dictionary`2/Enumerator<!0,!1> System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Enumerator_t81F0095C2D5C396203071D667A6252EFAB3D76D6  Dictionary_2_GetEnumerator_m42E0EB162F4066A0FB7F3A2A02C8CC0318A39399_gshared (Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C * __this, const RuntimeMethod* method);
// System.Collections.Generic.KeyValuePair`2<!0,!1> System.Collections.Generic.Dictionary`2/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::get_Current()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR KeyValuePair_2_tCBAAE4FBE6091373C1916EE17527311382CF4551  Enumerator_get_Current_m23B8CC855231000EC661C87C6F73B91516A0DC30_gshared_inline (Enumerator_t81F0095C2D5C396203071D667A6252EFAB3D76D6 * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.KeyValuePair`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::get_Key()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  KeyValuePair_2_get_Key_m212B0FE7898E52C6B11FD6DD2C01E618B497E2AD_gshared_inline (KeyValuePair_2_tCBAAE4FBE6091373C1916EE17527311382CF4551 * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Enumerator_MoveNext_m0715DCFF0F848F5E633EB00D96EFD56764336D83_gshared (Enumerator_t81F0095C2D5C396203071D667A6252EFAB3D76D6 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Enumerator_Dispose_mD481D9AD8B5BA10E0F7D4C5FE195450387A2D77F_gshared (Enumerator_t81F0095C2D5C396203071D667A6252EFAB3D76D6 * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Enumerator_MoveNext_mEEAA9A380252BB2F9B2403853F4C00F2F643ADC4_gshared (Enumerator_t1AD96AD2810CD9FF13D02CD49EC9D4D447C1485C * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,System.Object>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Enumerator_Dispose_m7567E65C01E35A09AD2AD4814D708A8E76469D31_gshared (Enumerator_t1AD96AD2810CD9FF13D02CD49EC9D4D447C1485C * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int32,System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2__ctor_mE7F9D51201F5A72BF4995CA0F3F0E866DB21E638_gshared (Dictionary_2_tE1E5B6327FFA2C7AE34A69E0011815C914771C2F * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::TryGetValue(!0,!1&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Dictionary_2_TryGetValue_mCCC44A3A72C551AAFFAC85CBF1112F4AECA55E1C_gshared (Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C * __this, TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  ___key0, RuntimeObject ** ___value1, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::get_Count()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Dictionary_2_get_Count_mAD01C6490D7FB912583AA3512A48322E17DE103A_gshared (Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2__ctor_mAA4C8F456AA58EC1FB154441680BCFAA27AD6183_gshared (Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::Add(!0,!1)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2_Add_mF229F30CFDA8329990CF5541CC1C2D4E1E6E32DC_gshared (Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C * __this, TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  ___key0, RuntimeObject * ___value1, const RuntimeMethod* method);
// System.Collections.Generic.Dictionary`2/KeyCollection<!0,!1> System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::get_Keys()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyCollection_t7A3E9764F18C1DE44C90DF9D411158787E20A943 * Dictionary_2_get_Keys_mC4EA837159240291D78B6A50F7FFB1079128A54A_gshared (Dictionary_2_tBF9E2338C61ABCF5ED32053F509AFB44385F774C * __this, const RuntimeMethod* method);
// System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<!0,!1> System.Collections.Generic.Dictionary`2/KeyCollection<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Enumerator_t914132A1A7488464276E36385DFE4C80616AB5E9  KeyCollection_GetEnumerator_m86D73466A076A57AF442E78EA6DA84CA56459CE1_gshared (KeyCollection_t7A3E9764F18C1DE44C90DF9D411158787E20A943 * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::get_Current()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  Enumerator_get_Current_m3B16C98F89532B1B1779AEA703E89F4C00B89023_gshared_inline (Enumerator_t914132A1A7488464276E36385DFE4C80616AB5E9 * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Enumerator_MoveNext_mE8A672309D0D29E63163309C18A5DAA83FC9DB33_gshared (Enumerator_t914132A1A7488464276E36385DFE4C80616AB5E9 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Enumerator_Dispose_m7DA7AE89AA8C789574C279CA9A741A2D41D5B3D1_gshared (Enumerator_t914132A1A7488464276E36385DFE4C80616AB5E9 * __this, const RuntimeMethod* method);

// System.Void UnityEngine.Behaviour::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Behaviour__ctor_mCACD3614226521EA607B0F3640C0FAC7EACCBCE0 (Behaviour_t1A3DDDCF73B4627928FBFE02ED52B7251777DBD9 * __this, const RuntimeMethod* method);
// UnityEngine.Terrain[] UnityEngine.TerrainData::get_users()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* TerrainData_get_users_m4BBC80BD0296525664EB84FE7DD6F1ABAE1CAF0F (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/HeightmapChangedCallback::Invoke(UnityEngine.Terrain,UnityEngine.RectInt,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HeightmapChangedCallback_Invoke_m24BDB8F85D5AC1B4B183E8C698905E3281CB4489 (HeightmapChangedCallback_tB00DA531F9C32468E88700A5C2D55E05189E0FA0 * __this, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___terrain0, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49  ___heightRegion1, bool ___synched2, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/TextureChangedCallback::Invoke(UnityEngine.Terrain,System.String,UnityEngine.RectInt,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TextureChangedCallback_Invoke_mC92D41CF0240EA1783C1A1816696EA19895F5569 (TextureChangedCallback_tD8BA8EA99CC9FA597E4AA143944720822EFB7D9F * __this, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___terrain0, String_t* ___textureName1, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49  ___texelRegion2, bool ___synched3, const RuntimeMethod* method);
// System.Void UnityEngine.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m4DCF5CDB32C2C69290894101A81F473865169279 (Object_tF2F3778131EFF286AF62B7B013A170F95A91571A * __this, const RuntimeMethod* method);
// System.Void UnityEngine.TerrainData::Internal_Create(UnityEngine.TerrainData)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainData_Internal_Create_mA483D4EF29C637A9855A8825AB257DC97374A424 (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * ___terrainData0, const RuntimeMethod* method);
// System.Void UnityEngine.TerrainData::get_size_Injected(UnityEngine.Vector3&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainData_get_size_Injected_m181495692C7B755ACD1D7F7F115A2CE8DC6A9E64 (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * __this, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * ___ret0, const RuntimeMethod* method);
// System.Int32 UnityEngine.TerrainData::GetBoundaryValue(UnityEngine.TerrainData/BoundaryValueType)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t TerrainData_GetBoundaryValue_mDDB33647E2918B15F5499701A647695B8EF9763C (int32_t ___type0, const RuntimeMethod* method);
// UnityEngine.Terrain[] UnityEngine.Terrain::get_activeTerrains()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* Terrain_get_activeTerrains_m4F358455EB7630E59F2AB221B142A11B750D23F9 (const RuntimeMethod* method);
// System.Void UnityEngine.Terrain::SetNeighbors(UnityEngine.Terrain,UnityEngine.Terrain,UnityEngine.Terrain,UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Terrain_SetNeighbors_m8D84FD4852DE0F39C99BF04E6D4363C1869BF59F (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * __this, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___left0, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___top1, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___right2, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___bottom3, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_0::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass4_0__ctor_m857F329AF653D7F052DCF0BE6511BFE40CD13653 (U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8 * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility::HasValidTerrains()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TerrainUtility_HasValidTerrains_mA6E2D0BE718C6B58CD4C1400C910CBF73AF3172D (const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainGroups::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainGroups__ctor_mA9F11D4BE52D80563D0D31788BA80C8F5381FFB1 (TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_1::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass4_1__ctor_m2B5F521527B39BE091B856058F67DC7E3DE4B345 (U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696 * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Terrain::get_allowAutoConnect()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Terrain_get_allowAutoConnect_mC1B0AC480E9AA5E33EDF412E8F9AA3EB4832BA67 (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * __this, const RuntimeMethod* method);
// System.Int32 UnityEngine.Terrain::get_groupingID()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Terrain_get_groupingID_m8390315914A192A424C890605D780E638F5E1CC9 (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::ContainsKey(!0)
inline bool Dictionary_2_ContainsKey_mAB645E14BEA5777BD44ADAE7A50A6F0A8093BC9C (Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725 * __this, int32_t ___key0, const RuntimeMethod* method)
{
	return ((  bool (*) (Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725 *, int32_t, const RuntimeMethod*))Dictionary_2_ContainsKey_mE6DB9458466D0F98B67E2C6CAEFABBF9576AC4D7_gshared)(__this, ___key0, method);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainFilter__ctor_m6A1F2AE7CF7A3B502AFBCB351B615EBBE942B838 (TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method);
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::CreateFromPlacement(UnityEngine.Terrain,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * TerrainMap_CreateFromPlacement_mBF5B980BA13C9390739DFEA1644596CA54D44337 (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___originTerrain0, TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * ___filter1, bool ___fullValidation2, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::Add(!0,!1)
inline void Dictionary_2_Add_m267342D40CFF0F8B5BAD87A5CE00E3909531BD96 (Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725 * __this, int32_t ___key0, TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * ___value1, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725 *, int32_t, TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 *, const RuntimeMethod*))Dictionary_2_Add_m39BC00F21EE9459BB8DEF5479F95F79C5C740682_gshared)(__this, ___key0, ___value1, method);
}
// System.Int32 System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::get_Count()
inline int32_t Dictionary_2_get_Count_mFA499FD6C7C49BC3D70E56F18B385FC96BB1122D (Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725 * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725 *, const RuntimeMethod*))Dictionary_2_get_Count_m12374F2F6F3D2DE9CBF98D3BD63CBB0DA19C69C5_gshared)(__this, method);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility::ClearConnectivity()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainUtility_ClearConnectivity_mC50EAA8DA06ED94944F6168505271B127389EC5A (const RuntimeMethod* method);
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainGroups UnityEngine.Experimental.TerrainAPI.TerrainUtility::CollectTerrains(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * TerrainUtility_CollectTerrains_m4630246A7274A15FB2AE8C13E653E8B73C129F9B (bool ___onlyAutoConnectedTerrains0, const RuntimeMethod* method);
// System.Collections.Generic.Dictionary`2/Enumerator<!0,!1> System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::GetEnumerator()
inline Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C  Dictionary_2_GetEnumerator_m2FABB49D216C87FA099BA346CB5CD03DCD24C952 (Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725 * __this, const RuntimeMethod* method)
{
	return ((  Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C  (*) (Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725 *, const RuntimeMethod*))Dictionary_2_GetEnumerator_m17437D82A5AF502166F10DD12B5C5830DDB95444_gshared)(__this, method);
}
// System.Collections.Generic.KeyValuePair`2<!0,!1> System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::get_Current()
inline KeyValuePair_2_t4D2678F5DF760772ED05557D6F41001894311AD9  Enumerator_get_Current_mF013CBBAC5FC4D1B2E4A4DF551C8FE254F675FC3_inline (Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C * __this, const RuntimeMethod* method)
{
	return ((  KeyValuePair_2_t4D2678F5DF760772ED05557D6F41001894311AD9  (*) (Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C *, const RuntimeMethod*))Enumerator_get_Current_mE5033FC555E7BC63DDC919B903A8A305C3AADBEB_gshared_inline)(__this, method);
}
// !1 System.Collections.Generic.KeyValuePair`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::get_Value()
inline TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * KeyValuePair_2_get_Value_m011C84EFA22A68B46C33DD7DF651E3B2A65D0A8E_inline (KeyValuePair_2_t4D2678F5DF760772ED05557D6F41001894311AD9 * __this, const RuntimeMethod* method)
{
	return ((  TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * (*) (KeyValuePair_2_t4D2678F5DF760772ED05557D6F41001894311AD9 *, const RuntimeMethod*))KeyValuePair_2_get_Value_mC1E2EFCF98529D0550A547CF87C6EAB6821741BF_gshared_inline)(__this, method);
}
// System.Collections.Generic.Dictionary`2/Enumerator<!0,!1> System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::GetEnumerator()
inline Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99  Dictionary_2_GetEnumerator_m147C3BB4ED736CEA41232F46C074D6B974011AA4 (Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * __this, const RuntimeMethod* method)
{
	return ((  Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99  (*) (Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C *, const RuntimeMethod*))Dictionary_2_GetEnumerator_m42E0EB162F4066A0FB7F3A2A02C8CC0318A39399_gshared)(__this, method);
}
// System.Collections.Generic.KeyValuePair`2<!0,!1> System.Collections.Generic.Dictionary`2/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::get_Current()
inline KeyValuePair_2_t41025882EB77BC66901763733CF9F6733F84B2CD  Enumerator_get_Current_mB812E91C3669D0EE3EACF5F58E0A9BBD03D43711_inline (Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99 * __this, const RuntimeMethod* method)
{
	return ((  KeyValuePair_2_t41025882EB77BC66901763733CF9F6733F84B2CD  (*) (Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99 *, const RuntimeMethod*))Enumerator_get_Current_m23B8CC855231000EC661C87C6F73B91516A0DC30_gshared_inline)(__this, method);
}
// !0 System.Collections.Generic.KeyValuePair`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::get_Key()
inline TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  KeyValuePair_2_get_Key_mE6C14010B6C03B4E060CEF852A6F22FDC4713D0E_inline (KeyValuePair_2_t41025882EB77BC66901763733CF9F6733F84B2CD * __this, const RuntimeMethod* method)
{
	return ((  TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  (*) (KeyValuePair_2_t41025882EB77BC66901763733CF9F6733F84B2CD *, const RuntimeMethod*))KeyValuePair_2_get_Key_m212B0FE7898E52C6B11FD6DD2C01E618B497E2AD_gshared_inline)(__this, method);
}
// UnityEngine.Terrain UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::GetTerrain(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006 (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * __this, int32_t ___tileX0, int32_t ___tileZ1, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::MoveNext()
inline bool Enumerator_MoveNext_m0CCC05F07B62FCEE6591F765FC15D804D05BAD28 (Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99 * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99 *, const RuntimeMethod*))Enumerator_MoveNext_m0715DCFF0F848F5E633EB00D96EFD56764336D83_gshared)(__this, method);
}
// System.Void System.Collections.Generic.Dictionary`2/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::Dispose()
inline void Enumerator_Dispose_m07DC0EE5F0A8163D940559D768B774000D6D38AB (Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99 * __this, const RuntimeMethod* method)
{
	((  void (*) (Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99 *, const RuntimeMethod*))Enumerator_Dispose_mD481D9AD8B5BA10E0F7D4C5FE195450387A2D77F_gshared)(__this, method);
}
// System.Boolean System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::MoveNext()
inline bool Enumerator_MoveNext_m5380371A16E1990E23859EED5F3C2F1843B39B38 (Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C *, const RuntimeMethod*))Enumerator_MoveNext_mEEAA9A380252BB2F9B2403853F4C00F2F643ADC4_gshared)(__this, method);
}
// System.Void System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::Dispose()
inline void Enumerator_Dispose_m1F1794448E1C1BD96E362C46BA0DB16B018D57E4 (Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C * __this, const RuntimeMethod* method)
{
	((  void (*) (Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C *, const RuntimeMethod*))Enumerator_Dispose_m7567E65C01E35A09AD2AD4814D708A8E76469D31_gshared)(__this, method);
}
// System.Void System.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::.ctor()
inline void Dictionary_2__ctor_mE808665E6AFDBF2A2BA8A0F50089B72EE98DBBA1 (Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725 * __this, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_t8BE99204247C1C97B2675C8E9AB2B482BADCD725 *, const RuntimeMethod*))Dictionary_2__ctor_mE7F9D51201F5A72BF4995CA0F3F0E866DB21E638_gshared)(__this, method);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord::.ctor(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileCoord__ctor_m9EED41FD3E08320CDA102E34DC65236E5137F155 (TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901 * __this, int32_t ___tileX0, int32_t ___tileZ1, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::TryGetValue(!0,!1&)
inline bool Dictionary_2_TryGetValue_m98F72F5729EC6BC5A95E45EE02F330D48C274FE8 (Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * __this, TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  ___key0, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 ** ___value1, const RuntimeMethod* method)
{
	return ((  bool (*) (Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C *, TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901 , Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 **, const RuntimeMethod*))Dictionary_2_TryGetValue_mCCC44A3A72C551AAFFAC85CBF1112F4AECA55E1C_gshared)(__this, ___key0, ___value1, method);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/<>c__DisplayClass4_0::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass4_0__ctor_mF6CE52C3D202B71510907E3EDCA198C369468888 (U3CU3Ec__DisplayClass4_0_tBEB3CC092598F0D16C66B724FF1AE52EF06C0A8F * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Object::op_Equality(UnityEngine.Object,UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Object_op_Equality_mEE9EC7EB5C7DC3E95B94AB904E1986FC4D566D54 (Object_tF2F3778131EFF286AF62B7B013A170F95A91571A * ___x0, Object_tF2F3778131EFF286AF62B7B013A170F95A91571A * ___y1, const RuntimeMethod* method);
// UnityEngine.TerrainData UnityEngine.Terrain::get_terrainData()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41 (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * __this, const RuntimeMethod* method);
// UnityEngine.Transform UnityEngine.Component::get_transform()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F (Component_t62FBC8D2420DA4BE9037AFE430740F6B3EECA684 * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Transform::get_position()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341 (Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.TerrainData::get_size()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  TerrainData_get_size_mF68B76A49498AE26C506D77483EA81E4F816EB15 (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Vector2::.ctor(System.Single,System.Single)
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Vector2__ctor_m9F1F2D5EB5D1FF7091BB527AC8A72CBB309D115E_inline (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 * __this, float ___x0, float ___y1, const RuntimeMethod* method);
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::CreateFromPlacement(UnityEngine.Vector2,UnityEngine.Vector2,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * TerrainMap_CreateFromPlacement_m8BCE09C1C736432F61D78CED8868DC43F9CCD25D (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___gridOrigin0, Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___gridSize1, TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * ___filter2, bool ___fullValidation3, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainMap__ctor_m0A16A2E6ED5C4EFB2F87D72A5665EF7C4E62F761 (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter::Invoke(UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TerrainFilter_Invoke_m48E69E662BC21917E57559702D1F9D94E4F762F7 (TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * __this, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___terrain0, const RuntimeMethod* method);
// System.Int32 UnityEngine.Mathf::RoundToInt(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Mathf_RoundToInt_m56850BDF60FF9E3441CE57E5EFEFEF36EDCDE6DD (float ___f0, const RuntimeMethod* method);
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::TryToAddTerrain(System.Int32,System.Int32,UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TerrainMap_TryToAddTerrain_m49A7085766F102EADE7E4A29259232F399735C61 (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * __this, int32_t ___tileX0, int32_t ___tileZ1, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___terrain2, const RuntimeMethod* method);
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/ErrorCode UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::Validate()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t TerrainMap_Validate_m9CD6FAF70E4F90C896BF25F083BC0A7F21C8FA56 (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * __this, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::get_Count()
inline int32_t Dictionary_2_get_Count_m83CBEC6C5312F7F9158B9FDA00ACD5FDC0169F27 (Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C *, const RuntimeMethod*))Dictionary_2_get_Count_mAD01C6490D7FB912583AA3512A48322E17DE103A_gshared)(__this, method);
}
// System.Void System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::.ctor()
inline void Dictionary_2__ctor_mCCBF2E85C3037B87300EF879DD1791B4EB6DD230 (Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * __this, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C *, const RuntimeMethod*))Dictionary_2__ctor_mAA4C8F456AA58EC1FB154441680BCFAA27AD6183_gshared)(__this, method);
}
// System.Boolean UnityEngine.Vector3::op_Inequality(UnityEngine.Vector3,UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Vector3_op_Inequality_m15190A795B416EB699E69E6190DE6F1C1F208710 (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___lhs0, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___rhs1, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::Add(!0,!1)
inline void Dictionary_2_Add_mE1C4F9D19A66168F6DAD8690E89CDB2A36353F0D (Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * __this, TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  ___key0, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___value1, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C *, TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901 , Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, const RuntimeMethod*))Dictionary_2_Add_mF229F30CFDA8329990CF5541CC1C2D4E1E6E32DC_gshared)(__this, ___key0, ___value1, method);
}
// System.Boolean UnityEngine.Object::op_Inequality(UnityEngine.Object,UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Object_op_Inequality_mE1F187520BD83FB7D86A6D850710C4D42B864E90 (Object_tF2F3778131EFF286AF62B7B013A170F95A91571A * ___x0, Object_tF2F3778131EFF286AF62B7B013A170F95A91571A * ___y1, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::AddTerrainInternal(System.Int32,System.Int32,UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainMap_AddTerrainInternal_m82F62E3018D1D2A6E48FB7361DB6531F0E9BEB79 (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * __this, int32_t ___x0, int32_t ___z1, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___terrain2, const RuntimeMethod* method);
// System.Boolean UnityEngine.Object::op_Implicit(UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Object_op_Implicit_mC8214E4F028CC2F036CC82BDB81D102A02893499 (Object_tF2F3778131EFF286AF62B7B013A170F95A91571A * ___exists0, const RuntimeMethod* method);
// System.Boolean UnityEngine.Mathf::Approximately(System.Single,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Mathf_Approximately_mC2A3F657E3FD0CCAD4A4936CEE2F67D624A2AA55 (float ___a0, float ___b1, const RuntimeMethod* method);
// System.Collections.Generic.Dictionary`2/KeyCollection<!0,!1> System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::get_Keys()
inline KeyCollection_t35C4534DEF9EFE5AABB676279D2BD96D97E040EC * Dictionary_2_get_Keys_mAE2D87453C7973972A5637C6F4EAD27613692826 (Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * __this, const RuntimeMethod* method)
{
	return ((  KeyCollection_t35C4534DEF9EFE5AABB676279D2BD96D97E040EC * (*) (Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C *, const RuntimeMethod*))Dictionary_2_get_Keys_mC4EA837159240291D78B6A50F7FFB1079128A54A_gshared)(__this, method);
}
// System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<!0,!1> System.Collections.Generic.Dictionary`2/KeyCollection<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::GetEnumerator()
inline Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E  KeyCollection_GetEnumerator_m6405FB5505A9993F393EA3F5C33A46514043AA2A (KeyCollection_t35C4534DEF9EFE5AABB676279D2BD96D97E040EC * __this, const RuntimeMethod* method)
{
	return ((  Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E  (*) (KeyCollection_t35C4534DEF9EFE5AABB676279D2BD96D97E040EC *, const RuntimeMethod*))KeyCollection_GetEnumerator_m86D73466A076A57AF442E78EA6DA84CA56459CE1_gshared)(__this, method);
}
// !0 System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::get_Current()
inline TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  Enumerator_get_Current_m13B3FF8E2918832E70E30374C13E9939E9AA3894_inline (Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E * __this, const RuntimeMethod* method)
{
	return ((  TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  (*) (Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E *, const RuntimeMethod*))Enumerator_get_Current_m3B16C98F89532B1B1779AEA703E89F4C00B89023_gshared_inline)(__this, method);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::ValidateTerrain(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainMap_ValidateTerrain_mFE264FDE78C3D68285943250BC9FABAC89D85764 (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * __this, int32_t ___tileX0, int32_t ___tileZ1, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::MoveNext()
inline bool Enumerator_MoveNext_m821A025A3826D2405C49130607D842500F1ECEF4 (Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E *, const RuntimeMethod*))Enumerator_MoveNext_mE8A672309D0D29E63163309C18A5DAA83FC9DB33_gshared)(__this, method);
}
// System.Void System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::Dispose()
inline void Enumerator_Dispose_mAC047D3AE663114F2D8DFDA994E2C9D78A3E9EB0 (Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E * __this, const RuntimeMethod* method)
{
	((  void (*) (Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E *, const RuntimeMethod*))Enumerator_Dispose_m7DA7AE89AA8C789574C279CA9A741A2D41D5B3D1_gshared)(__this, method);
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
// UnityEngine.TerrainData UnityEngine.Terrain::get_terrainData()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41 (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * __this, const RuntimeMethod* method)
{
	typedef TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * (*Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41_ftn) (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *);
	static Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Terrain::get_terrainData()");
	TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// System.Boolean UnityEngine.Terrain::get_allowAutoConnect()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Terrain_get_allowAutoConnect_mC1B0AC480E9AA5E33EDF412E8F9AA3EB4832BA67 (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * __this, const RuntimeMethod* method)
{
	typedef bool (*Terrain_get_allowAutoConnect_mC1B0AC480E9AA5E33EDF412E8F9AA3EB4832BA67_ftn) (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *);
	static Terrain_get_allowAutoConnect_mC1B0AC480E9AA5E33EDF412E8F9AA3EB4832BA67_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Terrain_get_allowAutoConnect_mC1B0AC480E9AA5E33EDF412E8F9AA3EB4832BA67_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Terrain::get_allowAutoConnect()");
	bool retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// System.Int32 UnityEngine.Terrain::get_groupingID()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Terrain_get_groupingID_m8390315914A192A424C890605D780E638F5E1CC9 (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * __this, const RuntimeMethod* method)
{
	typedef int32_t (*Terrain_get_groupingID_m8390315914A192A424C890605D780E638F5E1CC9_ftn) (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *);
	static Terrain_get_groupingID_m8390315914A192A424C890605D780E638F5E1CC9_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Terrain_get_groupingID_m8390315914A192A424C890605D780E638F5E1CC9_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Terrain::get_groupingID()");
	int32_t retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// System.Void UnityEngine.Terrain::SetNeighbors(UnityEngine.Terrain,UnityEngine.Terrain,UnityEngine.Terrain,UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Terrain_SetNeighbors_m8D84FD4852DE0F39C99BF04E6D4363C1869BF59F (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * __this, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___left0, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___top1, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___right2, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___bottom3, const RuntimeMethod* method)
{
	typedef void (*Terrain_SetNeighbors_m8D84FD4852DE0F39C99BF04E6D4363C1869BF59F_ftn) (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *);
	static Terrain_SetNeighbors_m8D84FD4852DE0F39C99BF04E6D4363C1869BF59F_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Terrain_SetNeighbors_m8D84FD4852DE0F39C99BF04E6D4363C1869BF59F_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Terrain::SetNeighbors(UnityEngine.Terrain,UnityEngine.Terrain,UnityEngine.Terrain,UnityEngine.Terrain)");
	_il2cpp_icall_func(__this, ___left0, ___top1, ___right2, ___bottom3);
}
// UnityEngine.Terrain[] UnityEngine.Terrain::get_activeTerrains()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* Terrain_get_activeTerrains_m4F358455EB7630E59F2AB221B142A11B750D23F9 (const RuntimeMethod* method)
{
	typedef TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* (*Terrain_get_activeTerrains_m4F358455EB7630E59F2AB221B142A11B750D23F9_ftn) ();
	static Terrain_get_activeTerrains_m4F358455EB7630E59F2AB221B142A11B750D23F9_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Terrain_get_activeTerrains_m4F358455EB7630E59F2AB221B142A11B750D23F9_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Terrain::get_activeTerrains()");
	TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* retVal = _il2cpp_icall_func();
	return retVal;
}
// System.Void UnityEngine.Terrain::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Terrain__ctor_m3E411CBA0F2F20E56475F1755B7AEDF0C9F57464 (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * __this, const RuntimeMethod* method)
{
	{
		Behaviour__ctor_mCACD3614226521EA607B0F3640C0FAC7EACCBCE0(__this, /*hidden argument*/NULL);
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
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks::InvokeHeightmapChangedCallback(UnityEngine.TerrainData,UnityEngine.RectInt,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainCallbacks_InvokeHeightmapChangedCallback_m394735D1416B00373916335213992D011D5FDA86 (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * ___terrainData0, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49  ___heightRegion1, bool ___synched2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainCallbacks_InvokeHeightmapChangedCallback_m394735D1416B00373916335213992D011D5FDA86_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* V_1 = NULL;
	int32_t V_2 = 0;
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_3 = NULL;
	{
		HeightmapChangedCallback_tB00DA531F9C32468E88700A5C2D55E05189E0FA0 * L_0 = ((TerrainCallbacks_tF292CB70850DEF93A2AFD0005B4FF75C7FC8ECD0_StaticFields*)il2cpp_codegen_static_fields_for(TerrainCallbacks_tF292CB70850DEF93A2AFD0005B4FF75C7FC8ECD0_il2cpp_TypeInfo_var))->get_heightmapChanged_0();
		V_0 = (bool)((!(((RuntimeObject*)(HeightmapChangedCallback_tB00DA531F9C32468E88700A5C2D55E05189E0FA0 *)L_0) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_1 = V_0;
		if (!L_1)
		{
			goto IL_0037;
		}
	}
	{
		TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * L_2 = ___terrainData0;
		NullCheck(L_2);
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_3 = TerrainData_get_users_m4BBC80BD0296525664EB84FE7DD6F1ABAE1CAF0F(L_2, /*hidden argument*/NULL);
		V_1 = L_3;
		V_2 = 0;
		goto IL_0030;
	}

IL_001a:
	{
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_4 = V_1;
		int32_t L_5 = V_2;
		NullCheck(L_4);
		int32_t L_6 = L_5;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_7 = (L_4)->GetAt(static_cast<il2cpp_array_size_t>(L_6));
		V_3 = L_7;
		HeightmapChangedCallback_tB00DA531F9C32468E88700A5C2D55E05189E0FA0 * L_8 = ((TerrainCallbacks_tF292CB70850DEF93A2AFD0005B4FF75C7FC8ECD0_StaticFields*)il2cpp_codegen_static_fields_for(TerrainCallbacks_tF292CB70850DEF93A2AFD0005B4FF75C7FC8ECD0_il2cpp_TypeInfo_var))->get_heightmapChanged_0();
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_9 = V_3;
		RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49  L_10 = ___heightRegion1;
		bool L_11 = ___synched2;
		NullCheck(L_8);
		HeightmapChangedCallback_Invoke_m24BDB8F85D5AC1B4B183E8C698905E3281CB4489(L_8, L_9, L_10, L_11, /*hidden argument*/NULL);
		int32_t L_12 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_12, (int32_t)1));
	}

IL_0030:
	{
		int32_t L_13 = V_2;
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_14 = V_1;
		NullCheck(L_14);
		if ((((int32_t)L_13) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_14)->max_length)))))))
		{
			goto IL_001a;
		}
	}
	{
	}

IL_0037:
	{
		return;
	}
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks::InvokeTextureChangedCallback(UnityEngine.TerrainData,System.String,UnityEngine.RectInt,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainCallbacks_InvokeTextureChangedCallback_m10A2EFE8E490EC932777717717CC61709FCA3307 (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * ___terrainData0, String_t* ___textureName1, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49  ___texelRegion2, bool ___synched3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainCallbacks_InvokeTextureChangedCallback_m10A2EFE8E490EC932777717717CC61709FCA3307_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* V_1 = NULL;
	int32_t V_2 = 0;
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_3 = NULL;
	{
		TextureChangedCallback_tD8BA8EA99CC9FA597E4AA143944720822EFB7D9F * L_0 = ((TerrainCallbacks_tF292CB70850DEF93A2AFD0005B4FF75C7FC8ECD0_StaticFields*)il2cpp_codegen_static_fields_for(TerrainCallbacks_tF292CB70850DEF93A2AFD0005B4FF75C7FC8ECD0_il2cpp_TypeInfo_var))->get_textureChanged_1();
		V_0 = (bool)((!(((RuntimeObject*)(TextureChangedCallback_tD8BA8EA99CC9FA597E4AA143944720822EFB7D9F *)L_0) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_1 = V_0;
		if (!L_1)
		{
			goto IL_0038;
		}
	}
	{
		TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * L_2 = ___terrainData0;
		NullCheck(L_2);
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_3 = TerrainData_get_users_m4BBC80BD0296525664EB84FE7DD6F1ABAE1CAF0F(L_2, /*hidden argument*/NULL);
		V_1 = L_3;
		V_2 = 0;
		goto IL_0031;
	}

IL_001a:
	{
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_4 = V_1;
		int32_t L_5 = V_2;
		NullCheck(L_4);
		int32_t L_6 = L_5;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_7 = (L_4)->GetAt(static_cast<il2cpp_array_size_t>(L_6));
		V_3 = L_7;
		TextureChangedCallback_tD8BA8EA99CC9FA597E4AA143944720822EFB7D9F * L_8 = ((TerrainCallbacks_tF292CB70850DEF93A2AFD0005B4FF75C7FC8ECD0_StaticFields*)il2cpp_codegen_static_fields_for(TerrainCallbacks_tF292CB70850DEF93A2AFD0005B4FF75C7FC8ECD0_il2cpp_TypeInfo_var))->get_textureChanged_1();
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_9 = V_3;
		String_t* L_10 = ___textureName1;
		RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49  L_11 = ___texelRegion2;
		bool L_12 = ___synched3;
		NullCheck(L_8);
		TextureChangedCallback_Invoke_mC92D41CF0240EA1783C1A1816696EA19895F5569(L_8, L_9, L_10, L_11, L_12, /*hidden argument*/NULL);
		int32_t L_13 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_13, (int32_t)1));
	}

IL_0031:
	{
		int32_t L_14 = V_2;
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_15 = V_1;
		NullCheck(L_15);
		if ((((int32_t)L_14) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_15)->max_length)))))))
		{
			goto IL_001a;
		}
	}
	{
	}

IL_0038:
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
// System.Int32 UnityEngine.TerrainData::GetBoundaryValue(UnityEngine.TerrainData_BoundaryValueType)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t TerrainData_GetBoundaryValue_mDDB33647E2918B15F5499701A647695B8EF9763C (int32_t ___type0, const RuntimeMethod* method)
{
	typedef int32_t (*TerrainData_GetBoundaryValue_mDDB33647E2918B15F5499701A647695B8EF9763C_ftn) (int32_t);
	static TerrainData_GetBoundaryValue_mDDB33647E2918B15F5499701A647695B8EF9763C_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (TerrainData_GetBoundaryValue_mDDB33647E2918B15F5499701A647695B8EF9763C_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.TerrainData::GetBoundaryValue(UnityEngine.TerrainData/BoundaryValueType)");
	int32_t retVal = _il2cpp_icall_func(___type0);
	return retVal;
}
// System.Void UnityEngine.TerrainData::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainData__ctor_m09DE788EE93388ACD3E80CB586FC2ED551B66ED7 (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainData__ctor_m09DE788EE93388ACD3E80CB586FC2ED551B66ED7_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_il2cpp_TypeInfo_var);
		Object__ctor_m4DCF5CDB32C2C69290894101A81F473865169279(__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_il2cpp_TypeInfo_var);
		TerrainData_Internal_Create_mA483D4EF29C637A9855A8825AB257DC97374A424(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.TerrainData::Internal_Create(UnityEngine.TerrainData)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainData_Internal_Create_mA483D4EF29C637A9855A8825AB257DC97374A424 (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * ___terrainData0, const RuntimeMethod* method)
{
	typedef void (*TerrainData_Internal_Create_mA483D4EF29C637A9855A8825AB257DC97374A424_ftn) (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 *);
	static TerrainData_Internal_Create_mA483D4EF29C637A9855A8825AB257DC97374A424_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (TerrainData_Internal_Create_mA483D4EF29C637A9855A8825AB257DC97374A424_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.TerrainData::Internal_Create(UnityEngine.TerrainData)");
	_il2cpp_icall_func(___terrainData0);
}
// UnityEngine.Vector3 UnityEngine.TerrainData::get_size()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  TerrainData_get_size_mF68B76A49498AE26C506D77483EA81E4F816EB15 (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * __this, const RuntimeMethod* method)
{
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		TerrainData_get_size_Injected_m181495692C7B755ACD1D7F7F115A2CE8DC6A9E64(__this, (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E *)(&V_0), /*hidden argument*/NULL);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_0 = V_0;
		return L_0;
	}
}
// System.Single UnityEngine.TerrainData::GetAlphamapResolutionInternal()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float TerrainData_GetAlphamapResolutionInternal_mB3D8631E512C887B38CE96496428B803C3837CCB (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * __this, const RuntimeMethod* method)
{
	typedef float (*TerrainData_GetAlphamapResolutionInternal_mB3D8631E512C887B38CE96496428B803C3837CCB_ftn) (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 *);
	static TerrainData_GetAlphamapResolutionInternal_mB3D8631E512C887B38CE96496428B803C3837CCB_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (TerrainData_GetAlphamapResolutionInternal_mB3D8631E512C887B38CE96496428B803C3837CCB_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.TerrainData::GetAlphamapResolutionInternal()");
	float retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// UnityEngine.Terrain[] UnityEngine.TerrainData::get_users()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* TerrainData_get_users_m4BBC80BD0296525664EB84FE7DD6F1ABAE1CAF0F (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * __this, const RuntimeMethod* method)
{
	typedef TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* (*TerrainData_get_users_m4BBC80BD0296525664EB84FE7DD6F1ABAE1CAF0F_ftn) (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 *);
	static TerrainData_get_users_m4BBC80BD0296525664EB84FE7DD6F1ABAE1CAF0F_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (TerrainData_get_users_m4BBC80BD0296525664EB84FE7DD6F1ABAE1CAF0F_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.TerrainData::get_users()");
	TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// System.Void UnityEngine.TerrainData::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainData__cctor_m64E6CF88BD21FC182D29D169EBCA04D965C46517 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainData__cctor_m64E6CF88BD21FC182D29D169EBCA04D965C46517_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		int32_t L_0 = TerrainData_GetBoundaryValue_mDDB33647E2918B15F5499701A647695B8EF9763C(0, /*hidden argument*/NULL);
		((TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_il2cpp_TypeInfo_var))->set_k_MaximumResolution_4(L_0);
		int32_t L_1 = TerrainData_GetBoundaryValue_mDDB33647E2918B15F5499701A647695B8EF9763C(1, /*hidden argument*/NULL);
		((TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_il2cpp_TypeInfo_var))->set_k_MinimumDetailResolutionPerPatch_5(L_1);
		int32_t L_2 = TerrainData_GetBoundaryValue_mDDB33647E2918B15F5499701A647695B8EF9763C(2, /*hidden argument*/NULL);
		((TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_il2cpp_TypeInfo_var))->set_k_MaximumDetailResolutionPerPatch_6(L_2);
		int32_t L_3 = TerrainData_GetBoundaryValue_mDDB33647E2918B15F5499701A647695B8EF9763C(3, /*hidden argument*/NULL);
		((TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_il2cpp_TypeInfo_var))->set_k_MaximumDetailPatchCount_7(L_3);
		int32_t L_4 = TerrainData_GetBoundaryValue_mDDB33647E2918B15F5499701A647695B8EF9763C(4, /*hidden argument*/NULL);
		((TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_il2cpp_TypeInfo_var))->set_k_MaximumDetailsPerRes_8(L_4);
		int32_t L_5 = TerrainData_GetBoundaryValue_mDDB33647E2918B15F5499701A647695B8EF9763C(5, /*hidden argument*/NULL);
		((TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_il2cpp_TypeInfo_var))->set_k_MinimumAlphamapResolution_9(L_5);
		int32_t L_6 = TerrainData_GetBoundaryValue_mDDB33647E2918B15F5499701A647695B8EF9763C(6, /*hidden argument*/NULL);
		((TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_il2cpp_TypeInfo_var))->set_k_MaximumAlphamapResolution_10(L_6);
		int32_t L_7 = TerrainData_GetBoundaryValue_mDDB33647E2918B15F5499701A647695B8EF9763C(7, /*hidden argument*/NULL);
		((TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_il2cpp_TypeInfo_var))->set_k_MinimumBaseMapResolution_11(L_7);
		int32_t L_8 = TerrainData_GetBoundaryValue_mDDB33647E2918B15F5499701A647695B8EF9763C(8, /*hidden argument*/NULL);
		((TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4_il2cpp_TypeInfo_var))->set_k_MaximumBaseMapResolution_12(L_8);
		return;
	}
}
// System.Void UnityEngine.TerrainData::get_size_Injected(UnityEngine.Vector3&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainData_get_size_Injected_m181495692C7B755ACD1D7F7F115A2CE8DC6A9E64 (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * __this, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * ___ret0, const RuntimeMethod* method)
{
	typedef void (*TerrainData_get_size_Injected_m181495692C7B755ACD1D7F7F115A2CE8DC6A9E64_ftn) (TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 *, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E *);
	static TerrainData_get_size_Injected_m181495692C7B755ACD1D7F7F115A2CE8DC6A9E64_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (TerrainData_get_size_Injected_m181495692C7B755ACD1D7F7F115A2CE8DC6A9E64_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.TerrainData::get_size_Injected(UnityEngine.Vector3&)");
	_il2cpp_icall_func(__this, ___ret0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility::HasValidTerrains()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TerrainUtility_HasValidTerrains_mA6E2D0BE718C6B58CD4C1400C910CBF73AF3172D (const RuntimeMethod* method)
{
	bool V_0 = false;
	int32_t G_B3_0 = 0;
	{
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_0 = Terrain_get_activeTerrains_m4F358455EB7630E59F2AB221B142A11B750D23F9(/*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_1 = Terrain_get_activeTerrains_m4F358455EB7630E59F2AB221B142A11B750D23F9(/*hidden argument*/NULL);
		NullCheck(L_1);
		G_B3_0 = ((!(((uint32_t)(((RuntimeArray*)L_1)->max_length)) <= ((uint32_t)0)))? 1 : 0);
		goto IL_0014;
	}

IL_0013:
	{
		G_B3_0 = 0;
	}

IL_0014:
	{
		V_0 = (bool)G_B3_0;
		goto IL_0017;
	}

IL_0017:
	{
		bool L_2 = V_0;
		return L_2;
	}
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility::ClearConnectivity()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainUtility_ClearConnectivity_mC50EAA8DA06ED94944F6168505271B127389EC5A (const RuntimeMethod* method)
{
	TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* V_0 = NULL;
	int32_t V_1 = 0;
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_2 = NULL;
	{
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_0 = Terrain_get_activeTerrains_m4F358455EB7630E59F2AB221B142A11B750D23F9(/*hidden argument*/NULL);
		V_0 = L_0;
		V_1 = 0;
		goto IL_001f;
	}

IL_000c:
	{
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_1 = V_0;
		int32_t L_2 = V_1;
		NullCheck(L_1);
		int32_t L_3 = L_2;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		V_2 = L_4;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_5 = V_2;
		NullCheck(L_5);
		Terrain_SetNeighbors_m8D84FD4852DE0F39C99BF04E6D4363C1869BF59F(L_5, (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *)NULL, (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *)NULL, (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *)NULL, (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *)NULL, /*hidden argument*/NULL);
		int32_t L_6 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1));
	}

IL_001f:
	{
		int32_t L_7 = V_1;
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_8 = V_0;
		NullCheck(L_8);
		if ((((int32_t)L_7) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_8)->max_length)))))))
		{
			goto IL_000c;
		}
	}
	{
		return;
	}
}
// UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainGroups UnityEngine.Experimental.TerrainAPI.TerrainUtility::CollectTerrains(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * TerrainUtility_CollectTerrains_m4630246A7274A15FB2AE8C13E653E8B73C129F9B (bool ___onlyAutoConnectedTerrains0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainUtility_CollectTerrains_m4630246A7274A15FB2AE8C13E653E8B73C129F9B_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8 * V_0 = NULL;
	TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * V_1 = NULL;
	bool V_2 = false;
	TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * V_3 = NULL;
	TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* V_4 = NULL;
	int32_t V_5 = 0;
	U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696 * V_6 = NULL;
	bool V_7 = false;
	bool V_8 = false;
	TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * V_9 = NULL;
	bool V_10 = false;
	int32_t G_B6_0 = 0;
	TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * G_B18_0 = NULL;
	{
		U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8 * L_0 = (U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8 *)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass4_0__ctor_m857F329AF653D7F052DCF0BE6511BFE40CD13653(L_0, /*hidden argument*/NULL);
		V_0 = L_0;
		U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8 * L_1 = V_0;
		bool L_2 = ___onlyAutoConnectedTerrains0;
		NullCheck(L_1);
		L_1->set_onlyAutoConnectedTerrains_0(L_2);
		bool L_3 = TerrainUtility_HasValidTerrains_mA6E2D0BE718C6B58CD4C1400C910CBF73AF3172D(/*hidden argument*/NULL);
		V_2 = (bool)((((int32_t)L_3) == ((int32_t)0))? 1 : 0);
		bool L_4 = V_2;
		if (!L_4)
		{
			goto IL_0021;
		}
	}
	{
		V_3 = (TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 *)NULL;
		goto IL_00f5;
	}

IL_0021:
	{
		TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * L_5 = (TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 *)il2cpp_codegen_object_new(TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288_il2cpp_TypeInfo_var);
		TerrainGroups__ctor_mA9F11D4BE52D80563D0D31788BA80C8F5381FFB1(L_5, /*hidden argument*/NULL);
		V_1 = L_5;
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_6 = Terrain_get_activeTerrains_m4F358455EB7630E59F2AB221B142A11B750D23F9(/*hidden argument*/NULL);
		V_4 = L_6;
		V_5 = 0;
		goto IL_00db;
	}

IL_0037:
	{
		U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696 * L_7 = (U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696 *)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass4_1__ctor_m2B5F521527B39BE091B856058F67DC7E3DE4B345(L_7, /*hidden argument*/NULL);
		V_6 = L_7;
		U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696 * L_8 = V_6;
		U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8 * L_9 = V_0;
		NullCheck(L_8);
		L_8->set_CSU24U3CU3E8__locals1_1(L_9);
		U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696 * L_10 = V_6;
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_11 = V_4;
		int32_t L_12 = V_5;
		NullCheck(L_11);
		int32_t L_13 = L_12;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_14 = (L_11)->GetAt(static_cast<il2cpp_array_size_t>(L_13));
		NullCheck(L_10);
		L_10->set_t_0(L_14);
		U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696 * L_15 = V_6;
		NullCheck(L_15);
		U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8 * L_16 = L_15->get_CSU24U3CU3E8__locals1_1();
		NullCheck(L_16);
		bool L_17 = L_16->get_onlyAutoConnectedTerrains_0();
		if (!L_17)
		{
			goto IL_0072;
		}
	}
	{
		U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696 * L_18 = V_6;
		NullCheck(L_18);
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_19 = L_18->get_t_0();
		NullCheck(L_19);
		bool L_20 = Terrain_get_allowAutoConnect_mC1B0AC480E9AA5E33EDF412E8F9AA3EB4832BA67(L_19, /*hidden argument*/NULL);
		G_B6_0 = ((((int32_t)L_20) == ((int32_t)0))? 1 : 0);
		goto IL_0073;
	}

IL_0072:
	{
		G_B6_0 = 0;
	}

IL_0073:
	{
		V_7 = (bool)G_B6_0;
		bool L_21 = V_7;
		if (!L_21)
		{
			goto IL_007b;
		}
	}
	{
		goto IL_00d5;
	}

IL_007b:
	{
		TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * L_22 = V_1;
		U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696 * L_23 = V_6;
		NullCheck(L_23);
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_24 = L_23->get_t_0();
		NullCheck(L_24);
		int32_t L_25 = Terrain_get_groupingID_m8390315914A192A424C890605D780E638F5E1CC9(L_24, /*hidden argument*/NULL);
		NullCheck(L_22);
		bool L_26 = Dictionary_2_ContainsKey_mAB645E14BEA5777BD44ADAE7A50A6F0A8093BC9C(L_22, L_25, /*hidden argument*/Dictionary_2_ContainsKey_mAB645E14BEA5777BD44ADAE7A50A6F0A8093BC9C_RuntimeMethod_var);
		V_8 = (bool)((((int32_t)L_26) == ((int32_t)0))? 1 : 0);
		bool L_27 = V_8;
		if (!L_27)
		{
			goto IL_00d4;
		}
	}
	{
		U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696 * L_28 = V_6;
		NullCheck(L_28);
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_29 = L_28->get_t_0();
		U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696 * L_30 = V_6;
		TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * L_31 = (TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 *)il2cpp_codegen_object_new(TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7_il2cpp_TypeInfo_var);
		TerrainFilter__ctor_m6A1F2AE7CF7A3B502AFBCB351B615EBBE942B838(L_31, L_30, (intptr_t)((intptr_t)U3CU3Ec__DisplayClass4_1_U3CCollectTerrainsU3Eb__0_m539C07F9B8F371A9E9C09A8AFD003DD4163C7810_RuntimeMethod_var), /*hidden argument*/NULL);
		TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_32 = TerrainMap_CreateFromPlacement_mBF5B980BA13C9390739DFEA1644596CA54D44337(L_29, L_31, (bool)1, /*hidden argument*/NULL);
		V_9 = L_32;
		TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_33 = V_9;
		V_10 = (bool)((!(((RuntimeObject*)(TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 *)L_33) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_34 = V_10;
		if (!L_34)
		{
			goto IL_00d3;
		}
	}
	{
		TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * L_35 = V_1;
		U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696 * L_36 = V_6;
		NullCheck(L_36);
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_37 = L_36->get_t_0();
		NullCheck(L_37);
		int32_t L_38 = Terrain_get_groupingID_m8390315914A192A424C890605D780E638F5E1CC9(L_37, /*hidden argument*/NULL);
		TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_39 = V_9;
		NullCheck(L_35);
		Dictionary_2_Add_m267342D40CFF0F8B5BAD87A5CE00E3909531BD96(L_35, L_38, L_39, /*hidden argument*/Dictionary_2_Add_m267342D40CFF0F8B5BAD87A5CE00E3909531BD96_RuntimeMethod_var);
	}

IL_00d3:
	{
	}

IL_00d4:
	{
	}

IL_00d5:
	{
		int32_t L_40 = V_5;
		V_5 = ((int32_t)il2cpp_codegen_add((int32_t)L_40, (int32_t)1));
	}

IL_00db:
	{
		int32_t L_41 = V_5;
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_42 = V_4;
		NullCheck(L_42);
		if ((((int32_t)L_41) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_42)->max_length)))))))
		{
			goto IL_0037;
		}
	}
	{
		TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * L_43 = V_1;
		NullCheck(L_43);
		int32_t L_44 = Dictionary_2_get_Count_mFA499FD6C7C49BC3D70E56F18B385FC96BB1122D(L_43, /*hidden argument*/Dictionary_2_get_Count_mFA499FD6C7C49BC3D70E56F18B385FC96BB1122D_RuntimeMethod_var);
		if (L_44)
		{
			goto IL_00f1;
		}
	}
	{
		G_B18_0 = ((TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 *)(NULL));
		goto IL_00f2;
	}

IL_00f1:
	{
		TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * L_45 = V_1;
		G_B18_0 = L_45;
	}

IL_00f2:
	{
		V_3 = G_B18_0;
		goto IL_00f5;
	}

IL_00f5:
	{
		TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * L_46 = V_3;
		return L_46;
	}
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility::AutoConnect()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainUtility_AutoConnect_m8526A29E63B328915E516505E3195637A1F100EF (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainUtility_AutoConnect_m8526A29E63B328915E516505E3195637A1F100EF_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * V_0 = NULL;
	bool V_1 = false;
	bool V_2 = false;
	Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C  V_3;
	memset((&V_3), 0, sizeof(V_3));
	KeyValuePair_2_t4D2678F5DF760772ED05557D6F41001894311AD9  V_4;
	memset((&V_4), 0, sizeof(V_4));
	TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * V_5 = NULL;
	Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99  V_6;
	memset((&V_6), 0, sizeof(V_6));
	KeyValuePair_2_t41025882EB77BC66901763733CF9F6733F84B2CD  V_7;
	memset((&V_7), 0, sizeof(V_7));
	TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  V_8;
	memset((&V_8), 0, sizeof(V_8));
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_9 = NULL;
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_10 = NULL;
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_11 = NULL;
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_12 = NULL;
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_13 = NULL;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 2);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		bool L_0 = TerrainUtility_HasValidTerrains_mA6E2D0BE718C6B58CD4C1400C910CBF73AF3172D(/*hidden argument*/NULL);
		V_1 = (bool)((((int32_t)L_0) == ((int32_t)0))? 1 : 0);
		bool L_1 = V_1;
		if (!L_1)
		{
			goto IL_0012;
		}
	}
	{
		goto IL_013a;
	}

IL_0012:
	{
		TerrainUtility_ClearConnectivity_mC50EAA8DA06ED94944F6168505271B127389EC5A(/*hidden argument*/NULL);
		TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * L_2 = TerrainUtility_CollectTerrains_m4630246A7274A15FB2AE8C13E653E8B73C129F9B((bool)1, /*hidden argument*/NULL);
		V_0 = L_2;
		TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * L_3 = V_0;
		V_2 = (bool)((((RuntimeObject*)(TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 *)L_3) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0);
		bool L_4 = V_2;
		if (!L_4)
		{
			goto IL_002c;
		}
	}
	{
		goto IL_013a;
	}

IL_002c:
	{
		TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * L_5 = V_0;
		NullCheck(L_5);
		Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C  L_6 = Dictionary_2_GetEnumerator_m2FABB49D216C87FA099BA346CB5CD03DCD24C952(L_5, /*hidden argument*/Dictionary_2_GetEnumerator_m2FABB49D216C87FA099BA346CB5CD03DCD24C952_RuntimeMethod_var);
		V_3 = L_6;
	}

IL_0034:
	try
	{ // begin try (depth: 1)
		{
			goto IL_011d;
		}

IL_0039:
		{
			KeyValuePair_2_t4D2678F5DF760772ED05557D6F41001894311AD9  L_7 = Enumerator_get_Current_mF013CBBAC5FC4D1B2E4A4DF551C8FE254F675FC3_inline((Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C *)(&V_3), /*hidden argument*/Enumerator_get_Current_mF013CBBAC5FC4D1B2E4A4DF551C8FE254F675FC3_RuntimeMethod_var);
			V_4 = L_7;
			TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_8 = KeyValuePair_2_get_Value_m011C84EFA22A68B46C33DD7DF651E3B2A65D0A8E_inline((KeyValuePair_2_t4D2678F5DF760772ED05557D6F41001894311AD9 *)(&V_4), /*hidden argument*/KeyValuePair_2_get_Value_m011C84EFA22A68B46C33DD7DF651E3B2A65D0A8E_RuntimeMethod_var);
			V_5 = L_8;
			TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_9 = V_5;
			NullCheck(L_9);
			Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * L_10 = L_9->get_m_terrainTiles_2();
			NullCheck(L_10);
			Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99  L_11 = Dictionary_2_GetEnumerator_m147C3BB4ED736CEA41232F46C074D6B974011AA4(L_10, /*hidden argument*/Dictionary_2_GetEnumerator_m147C3BB4ED736CEA41232F46C074D6B974011AA4_RuntimeMethod_var);
			V_6 = L_11;
		}

IL_005b:
		try
		{ // begin try (depth: 2)
			{
				goto IL_00ff;
			}

IL_0060:
			{
				KeyValuePair_2_t41025882EB77BC66901763733CF9F6733F84B2CD  L_12 = Enumerator_get_Current_mB812E91C3669D0EE3EACF5F58E0A9BBD03D43711_inline((Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99 *)(&V_6), /*hidden argument*/Enumerator_get_Current_mB812E91C3669D0EE3EACF5F58E0A9BBD03D43711_RuntimeMethod_var);
				V_7 = L_12;
				TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_13 = KeyValuePair_2_get_Key_mE6C14010B6C03B4E060CEF852A6F22FDC4713D0E_inline((KeyValuePair_2_t41025882EB77BC66901763733CF9F6733F84B2CD *)(&V_7), /*hidden argument*/KeyValuePair_2_get_Key_mE6C14010B6C03B4E060CEF852A6F22FDC4713D0E_RuntimeMethod_var);
				V_8 = L_13;
				TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_14 = V_5;
				TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_15 = V_8;
				int32_t L_16 = L_15.get_tileX_0();
				TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_17 = V_8;
				int32_t L_18 = L_17.get_tileZ_1();
				NullCheck(L_14);
				Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_19 = TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006(L_14, L_16, L_18, /*hidden argument*/NULL);
				V_9 = L_19;
				TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_20 = V_5;
				TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_21 = V_8;
				int32_t L_22 = L_21.get_tileX_0();
				TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_23 = V_8;
				int32_t L_24 = L_23.get_tileZ_1();
				NullCheck(L_20);
				Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_25 = TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006(L_20, ((int32_t)il2cpp_codegen_subtract((int32_t)L_22, (int32_t)1)), L_24, /*hidden argument*/NULL);
				V_10 = L_25;
				TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_26 = V_5;
				TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_27 = V_8;
				int32_t L_28 = L_27.get_tileX_0();
				TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_29 = V_8;
				int32_t L_30 = L_29.get_tileZ_1();
				NullCheck(L_26);
				Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_31 = TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006(L_26, ((int32_t)il2cpp_codegen_add((int32_t)L_28, (int32_t)1)), L_30, /*hidden argument*/NULL);
				V_11 = L_31;
				TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_32 = V_5;
				TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_33 = V_8;
				int32_t L_34 = L_33.get_tileX_0();
				TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_35 = V_8;
				int32_t L_36 = L_35.get_tileZ_1();
				NullCheck(L_32);
				Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_37 = TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006(L_32, L_34, ((int32_t)il2cpp_codegen_add((int32_t)L_36, (int32_t)1)), /*hidden argument*/NULL);
				V_12 = L_37;
				TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_38 = V_5;
				TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_39 = V_8;
				int32_t L_40 = L_39.get_tileX_0();
				TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_41 = V_8;
				int32_t L_42 = L_41.get_tileZ_1();
				NullCheck(L_38);
				Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_43 = TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006(L_38, L_40, ((int32_t)il2cpp_codegen_subtract((int32_t)L_42, (int32_t)1)), /*hidden argument*/NULL);
				V_13 = L_43;
				Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_44 = V_9;
				Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_45 = V_10;
				Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_46 = V_12;
				Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_47 = V_11;
				Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_48 = V_13;
				NullCheck(L_44);
				Terrain_SetNeighbors_m8D84FD4852DE0F39C99BF04E6D4363C1869BF59F(L_44, L_45, L_46, L_47, L_48, /*hidden argument*/NULL);
			}

IL_00ff:
			{
				bool L_49 = Enumerator_MoveNext_m0CCC05F07B62FCEE6591F765FC15D804D05BAD28((Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99 *)(&V_6), /*hidden argument*/Enumerator_MoveNext_m0CCC05F07B62FCEE6591F765FC15D804D05BAD28_RuntimeMethod_var);
				if (L_49)
				{
					goto IL_0060;
				}
			}

IL_010b:
			{
				IL2CPP_LEAVE(0x11C, FINALLY_010d);
			}
		} // end try (depth: 2)
		catch(Il2CppExceptionWrapper& e)
		{
			__last_unhandled_exception = (Exception_t *)e.ex;
			goto FINALLY_010d;
		}

FINALLY_010d:
		{ // begin finally (depth: 2)
			Enumerator_Dispose_m07DC0EE5F0A8163D940559D768B774000D6D38AB((Enumerator_t602CC79896470B03AE89FC5C57383F716CFEFB99 *)(&V_6), /*hidden argument*/Enumerator_Dispose_m07DC0EE5F0A8163D940559D768B774000D6D38AB_RuntimeMethod_var);
			IL2CPP_END_FINALLY(269)
		} // end finally (depth: 2)
		IL2CPP_CLEANUP(269)
		{
			IL2CPP_JUMP_TBL(0x11C, IL_011c)
			IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
		}

IL_011c:
		{
		}

IL_011d:
		{
			bool L_50 = Enumerator_MoveNext_m5380371A16E1990E23859EED5F3C2F1843B39B38((Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C *)(&V_3), /*hidden argument*/Enumerator_MoveNext_m5380371A16E1990E23859EED5F3C2F1843B39B38_RuntimeMethod_var);
			if (L_50)
			{
				goto IL_0039;
			}
		}

IL_0129:
		{
			IL2CPP_LEAVE(0x13A, FINALLY_012b);
		}
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_012b;
	}

FINALLY_012b:
	{ // begin finally (depth: 1)
		Enumerator_Dispose_m1F1794448E1C1BD96E362C46BA0DB16B018D57E4((Enumerator_t97F91102C6E0DCF3B950AF7DDC5CA42AAB7C429C *)(&V_3), /*hidden argument*/Enumerator_Dispose_m1F1794448E1C1BD96E362C46BA0DB16B018D57E4_RuntimeMethod_var);
		IL2CPP_END_FINALLY(299)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(299)
	{
		IL2CPP_JUMP_TBL(0x13A, IL_013a)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_013a:
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
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks_HeightmapChangedCallback::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HeightmapChangedCallback__ctor_mB63473491843FCAFE4EC51977A276DF20F11B1D0 (HeightmapChangedCallback_tB00DA531F9C32468E88700A5C2D55E05189E0FA0 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks_HeightmapChangedCallback::Invoke(UnityEngine.Terrain,UnityEngine.RectInt,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HeightmapChangedCallback_Invoke_m24BDB8F85D5AC1B4B183E8C698905E3281CB4489 (HeightmapChangedCallback_tB00DA531F9C32468E88700A5C2D55E05189E0FA0 * __this, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___terrain0, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49  ___heightRegion1, bool ___synched2, const RuntimeMethod* method)
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
			if (___parameterCount == 3)
			{
				// open
				typedef void (*FunctionPointerType) (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___terrain0, ___heightRegion1, ___synched2, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___terrain0, ___heightRegion1, ___synched2, targetMethod);
			}
		}
		else if (___parameterCount != 3)
		{
			// open
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker2< RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(targetMethod, ___terrain0, ___heightRegion1, ___synched2);
					else
						GenericVirtActionInvoker2< RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(targetMethod, ___terrain0, ___heightRegion1, ___synched2);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker2< RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___terrain0, ___heightRegion1, ___synched2);
					else
						VirtActionInvoker2< RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___terrain0, ___heightRegion1, ___synched2);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(&___heightRegion1) - 1), ___synched2, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___terrain0, ___heightRegion1, ___synched2, targetMethod);
				}
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___terrain0, ___heightRegion1, ___synched2, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker3< Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(targetMethod, targetThis, ___terrain0, ___heightRegion1, ___synched2);
					else
						GenericVirtActionInvoker3< Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(targetMethod, targetThis, ___terrain0, ___heightRegion1, ___synched2);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker3< Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___terrain0, ___heightRegion1, ___synched2);
					else
						VirtActionInvoker3< Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___terrain0, ___heightRegion1, ___synched2);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___terrain0) - 1), ___heightRegion1, ___synched2, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___terrain0, ___heightRegion1, ___synched2, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult UnityEngine.Experimental.TerrainAPI.TerrainCallbacks_HeightmapChangedCallback::BeginInvoke(UnityEngine.Terrain,UnityEngine.RectInt,System.Boolean,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* HeightmapChangedCallback_BeginInvoke_m590DAC8F14CB6AC982D6FE89C27ACF10CFA17E05 (HeightmapChangedCallback_tB00DA531F9C32468E88700A5C2D55E05189E0FA0 * __this, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___terrain0, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49  ___heightRegion1, bool ___synched2, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback3, RuntimeObject * ___object4, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (HeightmapChangedCallback_BeginInvoke_m590DAC8F14CB6AC982D6FE89C27ACF10CFA17E05_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[4] = {0};
	__d_args[0] = ___terrain0;
	__d_args[1] = Box(RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49_il2cpp_TypeInfo_var, &___heightRegion1);
	__d_args[2] = Box(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_il2cpp_TypeInfo_var, &___synched2);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback3, (RuntimeObject*)___object4);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks_HeightmapChangedCallback::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HeightmapChangedCallback_EndInvoke_m015EB6B241A8FC17A0870FC57A1048520DCEB1E0 (HeightmapChangedCallback_tB00DA531F9C32468E88700A5C2D55E05189E0FA0 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks_TextureChangedCallback::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TextureChangedCallback__ctor_m7086172D805BDFEFEF9901EAC1C78904DBB63D29 (TextureChangedCallback_tD8BA8EA99CC9FA597E4AA143944720822EFB7D9F * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks_TextureChangedCallback::Invoke(UnityEngine.Terrain,System.String,UnityEngine.RectInt,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TextureChangedCallback_Invoke_mC92D41CF0240EA1783C1A1816696EA19895F5569 (TextureChangedCallback_tD8BA8EA99CC9FA597E4AA143944720822EFB7D9F * __this, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___terrain0, String_t* ___textureName1, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49  ___texelRegion2, bool ___synched3, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, String_t*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___terrain0, ___textureName1, ___texelRegion2, ___synched3, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, String_t*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___terrain0, ___textureName1, ___texelRegion2, ___synched3, targetMethod);
			}
		}
		else if (___parameterCount != 4)
		{
			// open
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker3< String_t*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(targetMethod, ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
					else
						GenericVirtActionInvoker3< String_t*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(targetMethod, ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker3< String_t*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
					else
						VirtActionInvoker3< String_t*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___textureName1) - 1), ___texelRegion2, ___synched3, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, String_t*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___terrain0, ___textureName1, ___texelRegion2, ___synched3, targetMethod);
				}
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, String_t*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___terrain0, ___textureName1, ___texelRegion2, ___synched3, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker4< Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, String_t*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(targetMethod, targetThis, ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
					else
						GenericVirtActionInvoker4< Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, String_t*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(targetMethod, targetThis, ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker4< Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, String_t*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
					else
						VirtActionInvoker4< Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, String_t*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, String_t*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___terrain0) - 1), ___textureName1, ___texelRegion2, ___synched3, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, String_t*, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49 , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___terrain0, ___textureName1, ___texelRegion2, ___synched3, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult UnityEngine.Experimental.TerrainAPI.TerrainCallbacks_TextureChangedCallback::BeginInvoke(UnityEngine.Terrain,System.String,UnityEngine.RectInt,System.Boolean,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TextureChangedCallback_BeginInvoke_mE19FD540CF24CED1C990B54DE4A84A270C5BA37C (TextureChangedCallback_tD8BA8EA99CC9FA597E4AA143944720822EFB7D9F * __this, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___terrain0, String_t* ___textureName1, RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49  ___texelRegion2, bool ___synched3, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback4, RuntimeObject * ___object5, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TextureChangedCallback_BeginInvoke_mE19FD540CF24CED1C990B54DE4A84A270C5BA37C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[5] = {0};
	__d_args[0] = ___terrain0;
	__d_args[1] = ___textureName1;
	__d_args[2] = Box(RectInt_tE7B8105A280C1AC73A4157ED41F9B86C9BD91E49_il2cpp_TypeInfo_var, &___texelRegion2);
	__d_args[3] = Box(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_il2cpp_TypeInfo_var, &___synched3);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback4, (RuntimeObject*)___object5);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks_TextureChangedCallback::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TextureChangedCallback_EndInvoke_mE86E8C09F0F8EB087F90979A86270204AB551B84 (TextureChangedCallback_tD8BA8EA99CC9FA597E4AA143944720822EFB7D9F * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility_<>c__DisplayClass4_0::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass4_0__ctor_m857F329AF653D7F052DCF0BE6511BFE40CD13653 (U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8 * __this, const RuntimeMethod* method)
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
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility_<>c__DisplayClass4_1::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass4_1__ctor_m2B5F521527B39BE091B856058F67DC7E3DE4B345 (U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696 * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility_<>c__DisplayClass4_1::<CollectTerrains>b__0(UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CU3Ec__DisplayClass4_1_U3CCollectTerrainsU3Eb__0_m539C07F9B8F371A9E9C09A8AFD003DD4163C7810 (U3CU3Ec__DisplayClass4_1_t4628C2311DC3CEECE17200D3AD3113D667B36696 * __this, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___x0, const RuntimeMethod* method)
{
	int32_t G_B4_0 = 0;
	int32_t G_B6_0 = 0;
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_0 = ___x0;
		NullCheck(L_0);
		int32_t L_1 = Terrain_get_groupingID_m8390315914A192A424C890605D780E638F5E1CC9(L_0, /*hidden argument*/NULL);
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_2 = __this->get_t_0();
		NullCheck(L_2);
		int32_t L_3 = Terrain_get_groupingID_m8390315914A192A424C890605D780E638F5E1CC9(L_2, /*hidden argument*/NULL);
		if ((!(((uint32_t)L_1) == ((uint32_t)L_3))))
		{
			goto IL_002b;
		}
	}
	{
		U3CU3Ec__DisplayClass4_0_t3074FF30377E883DD9C65B310F07325DB61E1EA8 * L_4 = __this->get_CSU24U3CU3E8__locals1_1();
		NullCheck(L_4);
		bool L_5 = L_4->get_onlyAutoConnectedTerrains_0();
		if (!L_5)
		{
			goto IL_0028;
		}
	}
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_6 = ___x0;
		NullCheck(L_6);
		bool L_7 = Terrain_get_allowAutoConnect_mC1B0AC480E9AA5E33EDF412E8F9AA3EB4832BA67(L_6, /*hidden argument*/NULL);
		G_B4_0 = ((int32_t)(L_7));
		goto IL_0029;
	}

IL_0028:
	{
		G_B4_0 = 1;
	}

IL_0029:
	{
		G_B6_0 = G_B4_0;
		goto IL_002c;
	}

IL_002b:
	{
		G_B6_0 = 0;
	}

IL_002c:
	{
		return (bool)G_B6_0;
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
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainGroups::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainGroups__ctor_mA9F11D4BE52D80563D0D31788BA80C8F5381FFB1 (TerrainGroups_t7252F67656E98D75852FF5CE365E82AB2ADB9288 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainGroups__ctor_mA9F11D4BE52D80563D0D31788BA80C8F5381FFB1_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Dictionary_2__ctor_mE808665E6AFDBF2A2BA8A0F50089B72EE98DBBA1(__this, /*hidden argument*/Dictionary_2__ctor_mE808665E6AFDBF2A2BA8A0F50089B72EE98DBBA1_RuntimeMethod_var);
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
// UnityEngine.Terrain UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap::GetTerrain(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006 (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * __this, int32_t ___tileX0, int32_t ___tileZ1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_0 = NULL;
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_1 = NULL;
	{
		V_0 = (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *)NULL;
		Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * L_0 = __this->get_m_terrainTiles_2();
		int32_t L_1 = ___tileX0;
		int32_t L_2 = ___tileZ1;
		TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_3;
		memset((&L_3), 0, sizeof(L_3));
		TileCoord__ctor_m9EED41FD3E08320CDA102E34DC65236E5137F155((&L_3), L_1, L_2, /*hidden argument*/NULL);
		NullCheck(L_0);
		Dictionary_2_TryGetValue_m98F72F5729EC6BC5A95E45EE02F330D48C274FE8(L_0, L_3, (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 **)(&V_0), /*hidden argument*/Dictionary_2_TryGetValue_m98F72F5729EC6BC5A95E45EE02F330D48C274FE8_RuntimeMethod_var);
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_4 = V_0;
		V_1 = L_4;
		goto IL_001c;
	}

IL_001c:
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_5 = V_1;
		return L_5;
	}
}
// UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap::CreateFromPlacement(UnityEngine.Terrain,UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TerrainFilter,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * TerrainMap_CreateFromPlacement_mBF5B980BA13C9390739DFEA1644596CA54D44337 (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___originTerrain0, TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * ___filter1, bool ___fullValidation2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap_CreateFromPlacement_mBF5B980BA13C9390739DFEA1644596CA54D44337_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass4_0_tBEB3CC092598F0D16C66B724FF1AE52EF06C0A8F * V_0 = NULL;
	float V_1 = 0.0f;
	float V_2 = 0.0f;
	float V_3 = 0.0f;
	float V_4 = 0.0f;
	bool V_5 = false;
	TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * V_6 = NULL;
	bool V_7 = false;
	bool V_8 = false;
	int32_t G_B4_0 = 0;
	{
		U3CU3Ec__DisplayClass4_0_tBEB3CC092598F0D16C66B724FF1AE52EF06C0A8F * L_0 = (U3CU3Ec__DisplayClass4_0_tBEB3CC092598F0D16C66B724FF1AE52EF06C0A8F *)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass4_0_tBEB3CC092598F0D16C66B724FF1AE52EF06C0A8F_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass4_0__ctor_mF6CE52C3D202B71510907E3EDCA198C369468888(L_0, /*hidden argument*/NULL);
		V_0 = L_0;
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_1 = Terrain_get_activeTerrains_m4F358455EB7630E59F2AB221B142A11B750D23F9(/*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_001f;
		}
	}
	{
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_2 = Terrain_get_activeTerrains_m4F358455EB7630E59F2AB221B142A11B750D23F9(/*hidden argument*/NULL);
		NullCheck(L_2);
		if (!(((RuntimeArray*)L_2)->max_length))
		{
			goto IL_001f;
		}
	}
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_3 = ___originTerrain0;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_il2cpp_TypeInfo_var);
		bool L_4 = Object_op_Equality_mEE9EC7EB5C7DC3E95B94AB904E1986FC4D566D54(L_3, (Object_tF2F3778131EFF286AF62B7B013A170F95A91571A *)NULL, /*hidden argument*/NULL);
		G_B4_0 = ((int32_t)(L_4));
		goto IL_0020;
	}

IL_001f:
	{
		G_B4_0 = 1;
	}

IL_0020:
	{
		V_5 = (bool)G_B4_0;
		bool L_5 = V_5;
		if (!L_5)
		{
			goto IL_002e;
		}
	}
	{
		V_6 = (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 *)NULL;
		goto IL_00cb;
	}

IL_002e:
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_6 = ___originTerrain0;
		NullCheck(L_6);
		TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * L_7 = Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41(L_6, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_il2cpp_TypeInfo_var);
		bool L_8 = Object_op_Equality_mEE9EC7EB5C7DC3E95B94AB904E1986FC4D566D54(L_7, (Object_tF2F3778131EFF286AF62B7B013A170F95A91571A *)NULL, /*hidden argument*/NULL);
		V_7 = L_8;
		bool L_9 = V_7;
		if (!L_9)
		{
			goto IL_0048;
		}
	}
	{
		V_6 = (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 *)NULL;
		goto IL_00cb;
	}

IL_0048:
	{
		U3CU3Ec__DisplayClass4_0_tBEB3CC092598F0D16C66B724FF1AE52EF06C0A8F * L_10 = V_0;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_11 = ___originTerrain0;
		NullCheck(L_11);
		int32_t L_12 = Terrain_get_groupingID_m8390315914A192A424C890605D780E638F5E1CC9(L_11, /*hidden argument*/NULL);
		NullCheck(L_10);
		L_10->set_groupID_0(L_12);
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_13 = ___originTerrain0;
		NullCheck(L_13);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_14 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_13, /*hidden argument*/NULL);
		NullCheck(L_14);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_15 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_14, /*hidden argument*/NULL);
		float L_16 = L_15.get_x_2();
		V_1 = L_16;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_17 = ___originTerrain0;
		NullCheck(L_17);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_18 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_17, /*hidden argument*/NULL);
		NullCheck(L_18);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_19 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_18, /*hidden argument*/NULL);
		float L_20 = L_19.get_z_4();
		V_2 = L_20;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_21 = ___originTerrain0;
		NullCheck(L_21);
		TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * L_22 = Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41(L_21, /*hidden argument*/NULL);
		NullCheck(L_22);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_23 = TerrainData_get_size_mF68B76A49498AE26C506D77483EA81E4F816EB15(L_22, /*hidden argument*/NULL);
		float L_24 = L_23.get_x_2();
		V_3 = L_24;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_25 = ___originTerrain0;
		NullCheck(L_25);
		TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * L_26 = Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41(L_25, /*hidden argument*/NULL);
		NullCheck(L_26);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_27 = TerrainData_get_size_mF68B76A49498AE26C506D77483EA81E4F816EB15(L_26, /*hidden argument*/NULL);
		float L_28 = L_27.get_z_4();
		V_4 = L_28;
		TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * L_29 = ___filter1;
		V_8 = (bool)((((RuntimeObject*)(TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 *)L_29) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0);
		bool L_30 = V_8;
		if (!L_30)
		{
			goto IL_00b1;
		}
	}
	{
		U3CU3Ec__DisplayClass4_0_tBEB3CC092598F0D16C66B724FF1AE52EF06C0A8F * L_31 = V_0;
		TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * L_32 = (TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 *)il2cpp_codegen_object_new(TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7_il2cpp_TypeInfo_var);
		TerrainFilter__ctor_m6A1F2AE7CF7A3B502AFBCB351B615EBBE942B838(L_32, L_31, (intptr_t)((intptr_t)U3CU3Ec__DisplayClass4_0_U3CCreateFromPlacementU3Eb__0_mA0E2295171D220FA7ABA12660D2CB357BC721653_RuntimeMethod_var), /*hidden argument*/NULL);
		___filter1 = L_32;
	}

IL_00b1:
	{
		float L_33 = V_1;
		float L_34 = V_2;
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_35;
		memset((&L_35), 0, sizeof(L_35));
		Vector2__ctor_m9F1F2D5EB5D1FF7091BB527AC8A72CBB309D115E_inline((&L_35), L_33, L_34, /*hidden argument*/NULL);
		float L_36 = V_3;
		float L_37 = V_4;
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_38;
		memset((&L_38), 0, sizeof(L_38));
		Vector2__ctor_m9F1F2D5EB5D1FF7091BB527AC8A72CBB309D115E_inline((&L_38), L_36, L_37, /*hidden argument*/NULL);
		TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * L_39 = ___filter1;
		bool L_40 = ___fullValidation2;
		TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_41 = TerrainMap_CreateFromPlacement_m8BCE09C1C736432F61D78CED8868DC43F9CCD25D(L_35, L_38, L_39, L_40, /*hidden argument*/NULL);
		V_6 = L_41;
		goto IL_00cb;
	}

IL_00cb:
	{
		TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_42 = V_6;
		return L_42;
	}
}
// UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap::CreateFromPlacement(UnityEngine.Vector2,UnityEngine.Vector2,UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TerrainFilter,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * TerrainMap_CreateFromPlacement_m8BCE09C1C736432F61D78CED8868DC43F9CCD25D (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___gridOrigin0, Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___gridSize1, TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * ___filter2, bool ___fullValidation3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap_CreateFromPlacement_m8BCE09C1C736432F61D78CED8868DC43F9CCD25D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * V_0 = NULL;
	float V_1 = 0.0f;
	float V_2 = 0.0f;
	bool V_3 = false;
	TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * V_4 = NULL;
	TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* V_5 = NULL;
	int32_t V_6 = 0;
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_7 = NULL;
	bool V_8 = false;
	bool V_9 = false;
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  V_10;
	memset((&V_10), 0, sizeof(V_10));
	int32_t V_11 = 0;
	int32_t V_12 = 0;
	bool V_13 = false;
	int32_t G_B3_0 = 0;
	int32_t G_B11_0 = 0;
	TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * G_B21_0 = NULL;
	{
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_0 = Terrain_get_activeTerrains_m4F358455EB7630E59F2AB221B142A11B750D23F9(/*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_1 = Terrain_get_activeTerrains_m4F358455EB7630E59F2AB221B142A11B750D23F9(/*hidden argument*/NULL);
		NullCheck(L_1);
		G_B3_0 = ((((int32_t)(((RuntimeArray*)L_1)->max_length)) == ((int32_t)0))? 1 : 0);
		goto IL_0014;
	}

IL_0013:
	{
		G_B3_0 = 1;
	}

IL_0014:
	{
		V_3 = (bool)G_B3_0;
		bool L_2 = V_3;
		if (!L_2)
		{
			goto IL_0020;
		}
	}
	{
		V_4 = (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 *)NULL;
		goto IL_0102;
	}

IL_0020:
	{
		TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_3 = (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 *)il2cpp_codegen_object_new(TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453_il2cpp_TypeInfo_var);
		TerrainMap__ctor_m0A16A2E6ED5C4EFB2F87D72A5665EF7C4E62F761(L_3, /*hidden argument*/NULL);
		V_0 = L_3;
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_4 = ___gridSize1;
		float L_5 = L_4.get_x_0();
		V_1 = ((float)((float)(1.0f)/(float)L_5));
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_6 = ___gridSize1;
		float L_7 = L_6.get_y_1();
		V_2 = ((float)((float)(1.0f)/(float)L_7));
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_8 = Terrain_get_activeTerrains_m4F358455EB7630E59F2AB221B142A11B750D23F9(/*hidden argument*/NULL);
		V_5 = L_8;
		V_6 = 0;
		goto IL_00d3;
	}

IL_0050:
	{
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_9 = V_5;
		int32_t L_10 = V_6;
		NullCheck(L_9);
		int32_t L_11 = L_10;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_12 = (L_9)->GetAt(static_cast<il2cpp_array_size_t>(L_11));
		V_7 = L_12;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_13 = V_7;
		NullCheck(L_13);
		TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * L_14 = Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41(L_13, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_il2cpp_TypeInfo_var);
		bool L_15 = Object_op_Equality_mEE9EC7EB5C7DC3E95B94AB904E1986FC4D566D54(L_14, (Object_tF2F3778131EFF286AF62B7B013A170F95A91571A *)NULL, /*hidden argument*/NULL);
		V_8 = L_15;
		bool L_16 = V_8;
		if (!L_16)
		{
			goto IL_006d;
		}
	}
	{
		goto IL_00cd;
	}

IL_006d:
	{
		TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * L_17 = ___filter2;
		if (!L_17)
		{
			goto IL_007a;
		}
	}
	{
		TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * L_18 = ___filter2;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_19 = V_7;
		NullCheck(L_18);
		bool L_20 = TerrainFilter_Invoke_m48E69E662BC21917E57559702D1F9D94E4F762F7(L_18, L_19, /*hidden argument*/NULL);
		G_B11_0 = ((int32_t)(L_20));
		goto IL_007b;
	}

IL_007a:
	{
		G_B11_0 = 1;
	}

IL_007b:
	{
		V_9 = (bool)G_B11_0;
		bool L_21 = V_9;
		if (!L_21)
		{
			goto IL_00cc;
		}
	}
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_22 = V_7;
		NullCheck(L_22);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_23 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_22, /*hidden argument*/NULL);
		NullCheck(L_23);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_24 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_23, /*hidden argument*/NULL);
		V_10 = L_24;
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_25 = V_10;
		float L_26 = L_25.get_x_2();
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_27 = ___gridOrigin0;
		float L_28 = L_27.get_x_0();
		float L_29 = V_1;
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_t4D4AC358D24F6DDC32EC291DDE1DF2C3B752A194_il2cpp_TypeInfo_var);
		int32_t L_30 = Mathf_RoundToInt_m56850BDF60FF9E3441CE57E5EFEFEF36EDCDE6DD(((float)il2cpp_codegen_multiply((float)((float)il2cpp_codegen_subtract((float)L_26, (float)L_28)), (float)L_29)), /*hidden argument*/NULL);
		V_11 = L_30;
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_31 = V_10;
		float L_32 = L_31.get_z_4();
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_33 = ___gridOrigin0;
		float L_34 = L_33.get_y_1();
		float L_35 = V_2;
		int32_t L_36 = Mathf_RoundToInt_m56850BDF60FF9E3441CE57E5EFEFEF36EDCDE6DD(((float)il2cpp_codegen_multiply((float)((float)il2cpp_codegen_subtract((float)L_32, (float)L_34)), (float)L_35)), /*hidden argument*/NULL);
		V_12 = L_36;
		TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_37 = V_0;
		int32_t L_38 = V_11;
		int32_t L_39 = V_12;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_40 = V_7;
		NullCheck(L_37);
		TerrainMap_TryToAddTerrain_m49A7085766F102EADE7E4A29259232F399735C61(L_37, L_38, L_39, L_40, /*hidden argument*/NULL);
	}

IL_00cc:
	{
	}

IL_00cd:
	{
		int32_t L_41 = V_6;
		V_6 = ((int32_t)il2cpp_codegen_add((int32_t)L_41, (int32_t)1));
	}

IL_00d3:
	{
		int32_t L_42 = V_6;
		TerrainU5BU5D_t3F6CC238FDF6EE231EDBF5ECD9C6CDE42F003C57* L_43 = V_5;
		NullCheck(L_43);
		if ((((int32_t)L_42) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_43)->max_length)))))))
		{
			goto IL_0050;
		}
	}
	{
		bool L_44 = ___fullValidation3;
		V_13 = L_44;
		bool L_45 = V_13;
		if (!L_45)
		{
			goto IL_00ec;
		}
	}
	{
		TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_46 = V_0;
		NullCheck(L_46);
		TerrainMap_Validate_m9CD6FAF70E4F90C896BF25F083BC0A7F21C8FA56(L_46, /*hidden argument*/NULL);
	}

IL_00ec:
	{
		TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_47 = V_0;
		NullCheck(L_47);
		Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * L_48 = L_47->get_m_terrainTiles_2();
		NullCheck(L_48);
		int32_t L_49 = Dictionary_2_get_Count_m83CBEC6C5312F7F9158B9FDA00ACD5FDC0169F27(L_48, /*hidden argument*/Dictionary_2_get_Count_m83CBEC6C5312F7F9158B9FDA00ACD5FDC0169F27_RuntimeMethod_var);
		if ((((int32_t)L_49) > ((int32_t)0)))
		{
			goto IL_00fd;
		}
	}
	{
		G_B21_0 = ((TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 *)(NULL));
		goto IL_00fe;
	}

IL_00fd:
	{
		TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_50 = V_0;
		G_B21_0 = L_50;
	}

IL_00fe:
	{
		V_4 = G_B21_0;
		goto IL_0102;
	}

IL_0102:
	{
		TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * L_51 = V_4;
		return L_51;
	}
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainMap__ctor_m0A16A2E6ED5C4EFB2F87D72A5665EF7C4E62F761 (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap__ctor_m0A16A2E6ED5C4EFB2F87D72A5665EF7C4E62F761_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405(__this, /*hidden argument*/NULL);
		__this->set_m_errorCode_1(0);
		Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * L_0 = (Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C *)il2cpp_codegen_object_new(Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C_il2cpp_TypeInfo_var);
		Dictionary_2__ctor_mCCBF2E85C3037B87300EF879DD1791B4EB6DD230(L_0, /*hidden argument*/Dictionary_2__ctor_mCCBF2E85C3037B87300EF879DD1791B4EB6DD230_RuntimeMethod_var);
		__this->set_m_terrainTiles_2(L_0);
		return;
	}
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap::AddTerrainInternal(System.Int32,System.Int32,UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainMap_AddTerrainInternal_m82F62E3018D1D2A6E48FB7361DB6531F0E9BEB79 (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * __this, int32_t ___x0, int32_t ___z1, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___terrain2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap_AddTerrainInternal_m82F62E3018D1D2A6E48FB7361DB6531F0E9BEB79_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	bool V_1 = false;
	{
		Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * L_0 = __this->get_m_terrainTiles_2();
		NullCheck(L_0);
		int32_t L_1 = Dictionary_2_get_Count_m83CBEC6C5312F7F9158B9FDA00ACD5FDC0169F27(L_0, /*hidden argument*/Dictionary_2_get_Count_m83CBEC6C5312F7F9158B9FDA00ACD5FDC0169F27_RuntimeMethod_var);
		V_0 = (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
		bool L_2 = V_0;
		if (!L_2)
		{
			goto IL_0026;
		}
	}
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_3 = ___terrain2;
		NullCheck(L_3);
		TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * L_4 = Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41(L_3, /*hidden argument*/NULL);
		NullCheck(L_4);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_5 = TerrainData_get_size_mF68B76A49498AE26C506D77483EA81E4F816EB15(L_4, /*hidden argument*/NULL);
		__this->set_m_patchSize_0(L_5);
		goto IL_0052;
	}

IL_0026:
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_6 = ___terrain2;
		NullCheck(L_6);
		TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * L_7 = Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41(L_6, /*hidden argument*/NULL);
		NullCheck(L_7);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_8 = TerrainData_get_size_mF68B76A49498AE26C506D77483EA81E4F816EB15(L_7, /*hidden argument*/NULL);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_9 = __this->get_m_patchSize_0();
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E_il2cpp_TypeInfo_var);
		bool L_10 = Vector3_op_Inequality_m15190A795B416EB699E69E6190DE6F1C1F208710(L_8, L_9, /*hidden argument*/NULL);
		V_1 = L_10;
		bool L_11 = V_1;
		if (!L_11)
		{
			goto IL_0051;
		}
	}
	{
		int32_t L_12 = __this->get_m_errorCode_1();
		__this->set_m_errorCode_1(((int32_t)((int32_t)L_12|(int32_t)4)));
	}

IL_0051:
	{
	}

IL_0052:
	{
		Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * L_13 = __this->get_m_terrainTiles_2();
		int32_t L_14 = ___x0;
		int32_t L_15 = ___z1;
		TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_16;
		memset((&L_16), 0, sizeof(L_16));
		TileCoord__ctor_m9EED41FD3E08320CDA102E34DC65236E5137F155((&L_16), L_14, L_15, /*hidden argument*/NULL);
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_17 = ___terrain2;
		NullCheck(L_13);
		Dictionary_2_Add_mE1C4F9D19A66168F6DAD8690E89CDB2A36353F0D(L_13, L_16, L_17, /*hidden argument*/Dictionary_2_Add_mE1C4F9D19A66168F6DAD8690E89CDB2A36353F0D_RuntimeMethod_var);
		return;
	}
}
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap::TryToAddTerrain(System.Int32,System.Int32,UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TerrainMap_TryToAddTerrain_m49A7085766F102EADE7E4A29259232F399735C61 (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * __this, int32_t ___tileX0, int32_t ___tileZ1, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___terrain2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap_TryToAddTerrain_m49A7085766F102EADE7E4A29259232F399735C61_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	bool V_1 = false;
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_2 = NULL;
	bool V_3 = false;
	bool V_4 = false;
	bool V_5 = false;
	{
		V_0 = (bool)0;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_0 = ___terrain2;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_il2cpp_TypeInfo_var);
		bool L_1 = Object_op_Inequality_mE1F187520BD83FB7D86A6D850710C4D42B864E90(L_0, (Object_tF2F3778131EFF286AF62B7B013A170F95A91571A *)NULL, /*hidden argument*/NULL);
		V_1 = L_1;
		bool L_2 = V_1;
		if (!L_2)
		{
			goto IL_0053;
		}
	}
	{
		int32_t L_3 = ___tileX0;
		int32_t L_4 = ___tileZ1;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_5 = TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006(__this, L_3, L_4, /*hidden argument*/NULL);
		V_2 = L_5;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_6 = V_2;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_il2cpp_TypeInfo_var);
		bool L_7 = Object_op_Inequality_mE1F187520BD83FB7D86A6D850710C4D42B864E90(L_6, (Object_tF2F3778131EFF286AF62B7B013A170F95A91571A *)NULL, /*hidden argument*/NULL);
		V_3 = L_7;
		bool L_8 = V_3;
		if (!L_8)
		{
			goto IL_0044;
		}
	}
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_9 = V_2;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_10 = ___terrain2;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_il2cpp_TypeInfo_var);
		bool L_11 = Object_op_Inequality_mE1F187520BD83FB7D86A6D850710C4D42B864E90(L_9, L_10, /*hidden argument*/NULL);
		V_4 = L_11;
		bool L_12 = V_4;
		if (!L_12)
		{
			goto IL_0041;
		}
	}
	{
		int32_t L_13 = __this->get_m_errorCode_1();
		__this->set_m_errorCode_1(((int32_t)((int32_t)L_13|(int32_t)1)));
	}

IL_0041:
	{
		goto IL_0052;
	}

IL_0044:
	{
		int32_t L_14 = ___tileX0;
		int32_t L_15 = ___tileZ1;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_16 = ___terrain2;
		TerrainMap_AddTerrainInternal_m82F62E3018D1D2A6E48FB7361DB6531F0E9BEB79(__this, L_14, L_15, L_16, /*hidden argument*/NULL);
		V_0 = (bool)1;
	}

IL_0052:
	{
	}

IL_0053:
	{
		bool L_17 = V_0;
		V_5 = L_17;
		goto IL_0058;
	}

IL_0058:
	{
		bool L_18 = V_5;
		return L_18;
	}
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap::ValidateTerrain(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainMap_ValidateTerrain_mFE264FDE78C3D68285943250BC9FABAC89D85764 (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * __this, int32_t ___tileX0, int32_t ___tileZ1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap_ValidateTerrain_mFE264FDE78C3D68285943250BC9FABAC89D85764_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_0 = NULL;
	bool V_1 = false;
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_2 = NULL;
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_3 = NULL;
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_4 = NULL;
	Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * V_5 = NULL;
	bool V_6 = false;
	bool V_7 = false;
	bool V_8 = false;
	bool V_9 = false;
	bool V_10 = false;
	bool V_11 = false;
	bool V_12 = false;
	bool V_13 = false;
	int32_t G_B5_0 = 0;
	int32_t G_B12_0 = 0;
	int32_t G_B19_0 = 0;
	int32_t G_B26_0 = 0;
	{
		int32_t L_0 = ___tileX0;
		int32_t L_1 = ___tileZ1;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_2 = TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006(__this, L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_3 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_il2cpp_TypeInfo_var);
		bool L_4 = Object_op_Inequality_mE1F187520BD83FB7D86A6D850710C4D42B864E90(L_3, (Object_tF2F3778131EFF286AF62B7B013A170F95A91571A *)NULL, /*hidden argument*/NULL);
		V_1 = L_4;
		bool L_5 = V_1;
		if (!L_5)
		{
			goto IL_026d;
		}
	}
	{
		int32_t L_6 = ___tileX0;
		int32_t L_7 = ___tileZ1;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_8 = TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006(__this, ((int32_t)il2cpp_codegen_subtract((int32_t)L_6, (int32_t)1)), L_7, /*hidden argument*/NULL);
		V_2 = L_8;
		int32_t L_9 = ___tileX0;
		int32_t L_10 = ___tileZ1;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_11 = TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006(__this, ((int32_t)il2cpp_codegen_add((int32_t)L_9, (int32_t)1)), L_10, /*hidden argument*/NULL);
		V_3 = L_11;
		int32_t L_12 = ___tileX0;
		int32_t L_13 = ___tileZ1;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_14 = TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006(__this, L_12, ((int32_t)il2cpp_codegen_add((int32_t)L_13, (int32_t)1)), /*hidden argument*/NULL);
		V_4 = L_14;
		int32_t L_15 = ___tileX0;
		int32_t L_16 = ___tileZ1;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_17 = TerrainMap_GetTerrain_mF027E4E4677131A19CA44E9A22CCB89101145006(__this, L_15, ((int32_t)il2cpp_codegen_subtract((int32_t)L_16, (int32_t)1)), /*hidden argument*/NULL);
		V_5 = L_17;
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_18 = V_2;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_il2cpp_TypeInfo_var);
		bool L_19 = Object_op_Implicit_mC8214E4F028CC2F036CC82BDB81D102A02893499(L_18, /*hidden argument*/NULL);
		V_6 = L_19;
		bool L_20 = V_6;
		if (!L_20)
		{
			goto IL_00cf;
		}
	}
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_21 = V_0;
		NullCheck(L_21);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_22 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_21, /*hidden argument*/NULL);
		NullCheck(L_22);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_23 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_22, /*hidden argument*/NULL);
		float L_24 = L_23.get_x_2();
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_25 = V_2;
		NullCheck(L_25);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_26 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_25, /*hidden argument*/NULL);
		NullCheck(L_26);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_27 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_26, /*hidden argument*/NULL);
		float L_28 = L_27.get_x_2();
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_29 = V_2;
		NullCheck(L_29);
		TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * L_30 = Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41(L_29, /*hidden argument*/NULL);
		NullCheck(L_30);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_31 = TerrainData_get_size_mF68B76A49498AE26C506D77483EA81E4F816EB15(L_30, /*hidden argument*/NULL);
		float L_32 = L_31.get_x_2();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_t4D4AC358D24F6DDC32EC291DDE1DF2C3B752A194_il2cpp_TypeInfo_var);
		bool L_33 = Mathf_Approximately_mC2A3F657E3FD0CCAD4A4936CEE2F67D624A2AA55(L_24, ((float)il2cpp_codegen_add((float)L_28, (float)L_32)), /*hidden argument*/NULL);
		if (!L_33)
		{
			goto IL_00b7;
		}
	}
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_34 = V_0;
		NullCheck(L_34);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_35 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_34, /*hidden argument*/NULL);
		NullCheck(L_35);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_36 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_35, /*hidden argument*/NULL);
		float L_37 = L_36.get_z_4();
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_38 = V_2;
		NullCheck(L_38);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_39 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_38, /*hidden argument*/NULL);
		NullCheck(L_39);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_40 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_39, /*hidden argument*/NULL);
		float L_41 = L_40.get_z_4();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_t4D4AC358D24F6DDC32EC291DDE1DF2C3B752A194_il2cpp_TypeInfo_var);
		bool L_42 = Mathf_Approximately_mC2A3F657E3FD0CCAD4A4936CEE2F67D624A2AA55(L_37, L_41, /*hidden argument*/NULL);
		G_B5_0 = ((((int32_t)L_42) == ((int32_t)0))? 1 : 0);
		goto IL_00b8;
	}

IL_00b7:
	{
		G_B5_0 = 1;
	}

IL_00b8:
	{
		V_7 = (bool)G_B5_0;
		bool L_43 = V_7;
		if (!L_43)
		{
			goto IL_00ce;
		}
	}
	{
		int32_t L_44 = __this->get_m_errorCode_1();
		__this->set_m_errorCode_1(((int32_t)((int32_t)L_44|(int32_t)8)));
	}

IL_00ce:
	{
	}

IL_00cf:
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_45 = V_3;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_il2cpp_TypeInfo_var);
		bool L_46 = Object_op_Implicit_mC8214E4F028CC2F036CC82BDB81D102A02893499(L_45, /*hidden argument*/NULL);
		V_8 = L_46;
		bool L_47 = V_8;
		if (!L_47)
		{
			goto IL_0156;
		}
	}
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_48 = V_0;
		NullCheck(L_48);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_49 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_48, /*hidden argument*/NULL);
		NullCheck(L_49);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_50 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_49, /*hidden argument*/NULL);
		float L_51 = L_50.get_x_2();
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_52 = V_0;
		NullCheck(L_52);
		TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * L_53 = Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41(L_52, /*hidden argument*/NULL);
		NullCheck(L_53);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_54 = TerrainData_get_size_mF68B76A49498AE26C506D77483EA81E4F816EB15(L_53, /*hidden argument*/NULL);
		float L_55 = L_54.get_x_2();
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_56 = V_3;
		NullCheck(L_56);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_57 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_56, /*hidden argument*/NULL);
		NullCheck(L_57);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_58 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_57, /*hidden argument*/NULL);
		float L_59 = L_58.get_x_2();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_t4D4AC358D24F6DDC32EC291DDE1DF2C3B752A194_il2cpp_TypeInfo_var);
		bool L_60 = Mathf_Approximately_mC2A3F657E3FD0CCAD4A4936CEE2F67D624A2AA55(((float)il2cpp_codegen_add((float)L_51, (float)L_55)), L_59, /*hidden argument*/NULL);
		if (!L_60)
		{
			goto IL_013e;
		}
	}
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_61 = V_0;
		NullCheck(L_61);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_62 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_61, /*hidden argument*/NULL);
		NullCheck(L_62);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_63 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_62, /*hidden argument*/NULL);
		float L_64 = L_63.get_z_4();
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_65 = V_3;
		NullCheck(L_65);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_66 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_65, /*hidden argument*/NULL);
		NullCheck(L_66);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_67 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_66, /*hidden argument*/NULL);
		float L_68 = L_67.get_z_4();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_t4D4AC358D24F6DDC32EC291DDE1DF2C3B752A194_il2cpp_TypeInfo_var);
		bool L_69 = Mathf_Approximately_mC2A3F657E3FD0CCAD4A4936CEE2F67D624A2AA55(L_64, L_68, /*hidden argument*/NULL);
		G_B12_0 = ((((int32_t)L_69) == ((int32_t)0))? 1 : 0);
		goto IL_013f;
	}

IL_013e:
	{
		G_B12_0 = 1;
	}

IL_013f:
	{
		V_9 = (bool)G_B12_0;
		bool L_70 = V_9;
		if (!L_70)
		{
			goto IL_0155;
		}
	}
	{
		int32_t L_71 = __this->get_m_errorCode_1();
		__this->set_m_errorCode_1(((int32_t)((int32_t)L_71|(int32_t)8)));
	}

IL_0155:
	{
	}

IL_0156:
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_72 = V_4;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_il2cpp_TypeInfo_var);
		bool L_73 = Object_op_Implicit_mC8214E4F028CC2F036CC82BDB81D102A02893499(L_72, /*hidden argument*/NULL);
		V_10 = L_73;
		bool L_74 = V_10;
		if (!L_74)
		{
			goto IL_01e0;
		}
	}
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_75 = V_0;
		NullCheck(L_75);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_76 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_75, /*hidden argument*/NULL);
		NullCheck(L_76);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_77 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_76, /*hidden argument*/NULL);
		float L_78 = L_77.get_x_2();
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_79 = V_4;
		NullCheck(L_79);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_80 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_79, /*hidden argument*/NULL);
		NullCheck(L_80);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_81 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_80, /*hidden argument*/NULL);
		float L_82 = L_81.get_x_2();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_t4D4AC358D24F6DDC32EC291DDE1DF2C3B752A194_il2cpp_TypeInfo_var);
		bool L_83 = Mathf_Approximately_mC2A3F657E3FD0CCAD4A4936CEE2F67D624A2AA55(L_78, L_82, /*hidden argument*/NULL);
		if (!L_83)
		{
			goto IL_01c8;
		}
	}
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_84 = V_0;
		NullCheck(L_84);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_85 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_84, /*hidden argument*/NULL);
		NullCheck(L_85);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_86 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_85, /*hidden argument*/NULL);
		float L_87 = L_86.get_z_4();
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_88 = V_0;
		NullCheck(L_88);
		TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * L_89 = Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41(L_88, /*hidden argument*/NULL);
		NullCheck(L_89);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_90 = TerrainData_get_size_mF68B76A49498AE26C506D77483EA81E4F816EB15(L_89, /*hidden argument*/NULL);
		float L_91 = L_90.get_z_4();
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_92 = V_4;
		NullCheck(L_92);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_93 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_92, /*hidden argument*/NULL);
		NullCheck(L_93);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_94 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_93, /*hidden argument*/NULL);
		float L_95 = L_94.get_z_4();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_t4D4AC358D24F6DDC32EC291DDE1DF2C3B752A194_il2cpp_TypeInfo_var);
		bool L_96 = Mathf_Approximately_mC2A3F657E3FD0CCAD4A4936CEE2F67D624A2AA55(((float)il2cpp_codegen_add((float)L_87, (float)L_91)), L_95, /*hidden argument*/NULL);
		G_B19_0 = ((((int32_t)L_96) == ((int32_t)0))? 1 : 0);
		goto IL_01c9;
	}

IL_01c8:
	{
		G_B19_0 = 1;
	}

IL_01c9:
	{
		V_11 = (bool)G_B19_0;
		bool L_97 = V_11;
		if (!L_97)
		{
			goto IL_01df;
		}
	}
	{
		int32_t L_98 = __this->get_m_errorCode_1();
		__this->set_m_errorCode_1(((int32_t)((int32_t)L_98|(int32_t)8)));
	}

IL_01df:
	{
	}

IL_01e0:
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_99 = V_5;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tF2F3778131EFF286AF62B7B013A170F95A91571A_il2cpp_TypeInfo_var);
		bool L_100 = Object_op_Implicit_mC8214E4F028CC2F036CC82BDB81D102A02893499(L_99, /*hidden argument*/NULL);
		V_12 = L_100;
		bool L_101 = V_12;
		if (!L_101)
		{
			goto IL_026b;
		}
	}
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_102 = V_0;
		NullCheck(L_102);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_103 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_102, /*hidden argument*/NULL);
		NullCheck(L_103);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_104 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_103, /*hidden argument*/NULL);
		float L_105 = L_104.get_x_2();
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_106 = V_5;
		NullCheck(L_106);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_107 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_106, /*hidden argument*/NULL);
		NullCheck(L_107);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_108 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_107, /*hidden argument*/NULL);
		float L_109 = L_108.get_x_2();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_t4D4AC358D24F6DDC32EC291DDE1DF2C3B752A194_il2cpp_TypeInfo_var);
		bool L_110 = Mathf_Approximately_mC2A3F657E3FD0CCAD4A4936CEE2F67D624A2AA55(L_105, L_109, /*hidden argument*/NULL);
		if (!L_110)
		{
			goto IL_0253;
		}
	}
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_111 = V_0;
		NullCheck(L_111);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_112 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_111, /*hidden argument*/NULL);
		NullCheck(L_112);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_113 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_112, /*hidden argument*/NULL);
		float L_114 = L_113.get_z_4();
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_115 = V_5;
		NullCheck(L_115);
		Transform_tA8193BB29D4D2C7EC04918F3ED1816345186C3F1 * L_116 = Component_get_transform_mE8496EBC45BEB1BADB5F314960F1DF1C952FA11F(L_115, /*hidden argument*/NULL);
		NullCheck(L_116);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_117 = Transform_get_position_m40A8A9895568D56FFC687B57F30E8D53CB5EA341(L_116, /*hidden argument*/NULL);
		float L_118 = L_117.get_z_4();
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_119 = V_5;
		NullCheck(L_119);
		TerrainData_tAD3780D3C4DE5B9BE122BECE6D08C4AE169ED2A4 * L_120 = Terrain_get_terrainData_mDB60C324B3424339C3C9FA6CDF6DC1C9B47D6E41(L_119, /*hidden argument*/NULL);
		NullCheck(L_120);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_121 = TerrainData_get_size_mF68B76A49498AE26C506D77483EA81E4F816EB15(L_120, /*hidden argument*/NULL);
		float L_122 = L_121.get_z_4();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_t4D4AC358D24F6DDC32EC291DDE1DF2C3B752A194_il2cpp_TypeInfo_var);
		bool L_123 = Mathf_Approximately_mC2A3F657E3FD0CCAD4A4936CEE2F67D624A2AA55(L_114, ((float)il2cpp_codegen_add((float)L_118, (float)L_122)), /*hidden argument*/NULL);
		G_B26_0 = ((((int32_t)L_123) == ((int32_t)0))? 1 : 0);
		goto IL_0254;
	}

IL_0253:
	{
		G_B26_0 = 1;
	}

IL_0254:
	{
		V_13 = (bool)G_B26_0;
		bool L_124 = V_13;
		if (!L_124)
		{
			goto IL_026a;
		}
	}
	{
		int32_t L_125 = __this->get_m_errorCode_1();
		__this->set_m_errorCode_1(((int32_t)((int32_t)L_125|(int32_t)8)));
	}

IL_026a:
	{
	}

IL_026b:
	{
	}

IL_026d:
	{
		return;
	}
}
// UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_ErrorCode UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap::Validate()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t TerrainMap_Validate_m9CD6FAF70E4F90C896BF25F083BC0A7F21C8FA56 (TerrainMap_tDD61065279F906812F404E67C65CB7F40CA49453 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap_Validate_m9CD6FAF70E4F90C896BF25F083BC0A7F21C8FA56_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E  V_0;
	memset((&V_0), 0, sizeof(V_0));
	TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  V_1;
	memset((&V_1), 0, sizeof(V_1));
	int32_t V_2 = 0;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 1);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		Dictionary_2_t4990FF96A726883A9DEEF78473DD04BB3125158C * L_0 = __this->get_m_terrainTiles_2();
		NullCheck(L_0);
		KeyCollection_t35C4534DEF9EFE5AABB676279D2BD96D97E040EC * L_1 = Dictionary_2_get_Keys_mAE2D87453C7973972A5637C6F4EAD27613692826(L_0, /*hidden argument*/Dictionary_2_get_Keys_mAE2D87453C7973972A5637C6F4EAD27613692826_RuntimeMethod_var);
		NullCheck(L_1);
		Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E  L_2 = KeyCollection_GetEnumerator_m6405FB5505A9993F393EA3F5C33A46514043AA2A(L_1, /*hidden argument*/KeyCollection_GetEnumerator_m6405FB5505A9993F393EA3F5C33A46514043AA2A_RuntimeMethod_var);
		V_0 = L_2;
	}

IL_0013:
	try
	{ // begin try (depth: 1)
		{
			goto IL_0032;
		}

IL_0015:
		{
			TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_3 = Enumerator_get_Current_m13B3FF8E2918832E70E30374C13E9939E9AA3894_inline((Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E *)(&V_0), /*hidden argument*/Enumerator_get_Current_m13B3FF8E2918832E70E30374C13E9939E9AA3894_RuntimeMethod_var);
			V_1 = L_3;
			TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_4 = V_1;
			int32_t L_5 = L_4.get_tileX_0();
			TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_6 = V_1;
			int32_t L_7 = L_6.get_tileZ_1();
			TerrainMap_ValidateTerrain_mFE264FDE78C3D68285943250BC9FABAC89D85764(__this, L_5, L_7, /*hidden argument*/NULL);
		}

IL_0032:
		{
			bool L_8 = Enumerator_MoveNext_m821A025A3826D2405C49130607D842500F1ECEF4((Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E *)(&V_0), /*hidden argument*/Enumerator_MoveNext_m821A025A3826D2405C49130607D842500F1ECEF4_RuntimeMethod_var);
			if (L_8)
			{
				goto IL_0015;
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
		Enumerator_Dispose_mAC047D3AE663114F2D8DFDA994E2C9D78A3E9EB0((Enumerator_t98E92197A96F9CB97165498E8742DC9D7C602D7E *)(&V_0), /*hidden argument*/Enumerator_Dispose_mAC047D3AE663114F2D8DFDA994E2C9D78A3E9EB0_RuntimeMethod_var);
		IL2CPP_END_FINALLY(61)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(61)
	{
		IL2CPP_JUMP_TBL(0x4C, IL_004c)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_004c:
	{
		int32_t L_9 = __this->get_m_errorCode_1();
		V_2 = L_9;
		goto IL_0055;
	}

IL_0055:
	{
		int32_t L_10 = V_2;
		return L_10;
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
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_<>c__DisplayClass4_0::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass4_0__ctor_mF6CE52C3D202B71510907E3EDCA198C369468888 (U3CU3Ec__DisplayClass4_0_tBEB3CC092598F0D16C66B724FF1AE52EF06C0A8F * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_<>c__DisplayClass4_0::<CreateFromPlacement>b__0(UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CU3Ec__DisplayClass4_0_U3CCreateFromPlacementU3Eb__0_mA0E2295171D220FA7ABA12660D2CB357BC721653 (U3CU3Ec__DisplayClass4_0_tBEB3CC092598F0D16C66B724FF1AE52EF06C0A8F * __this, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___x0, const RuntimeMethod* method)
{
	{
		Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * L_0 = ___x0;
		NullCheck(L_0);
		int32_t L_1 = Terrain_get_groupingID_m8390315914A192A424C890605D780E638F5E1CC9(L_0, /*hidden argument*/NULL);
		int32_t L_2 = __this->get_groupID_0();
		return (bool)((((int32_t)L_1) == ((int32_t)L_2))? 1 : 0);
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
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TerrainFilter::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainFilter__ctor_m6A1F2AE7CF7A3B502AFBCB351B615EBBE942B838 (TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TerrainFilter::Invoke(UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TerrainFilter_Invoke_m48E69E662BC21917E57559702D1F9D94E4F762F7 (TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * __this, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___terrain0, const RuntimeMethod* method)
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
			if (___parameterCount == 1)
			{
				// open
				typedef bool (*FunctionPointerType) (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___terrain0, targetMethod);
			}
			else
			{
				// closed
				typedef bool (*FunctionPointerType) (void*, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___terrain0, targetMethod);
			}
		}
		else if (___parameterCount != 1)
		{
			// open
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker0< bool >::Invoke(targetMethod, ___terrain0);
					else
						result = GenericVirtFuncInvoker0< bool >::Invoke(targetMethod, ___terrain0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker0< bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___terrain0);
					else
						result = VirtFuncInvoker0< bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___terrain0);
				}
			}
			else
			{
				typedef bool (*FunctionPointerType) (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___terrain0, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef bool (*FunctionPointerType) (Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(___terrain0, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker1< bool, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * >::Invoke(targetMethod, targetThis, ___terrain0);
					else
						result = GenericVirtFuncInvoker1< bool, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * >::Invoke(targetMethod, targetThis, ___terrain0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker1< bool, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___terrain0);
					else
						result = VirtFuncInvoker1< bool, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___terrain0);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef bool (*FunctionPointerType) (RuntimeObject*, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___terrain0) - 1), targetMethod);
				}
				else
				{
					typedef bool (*FunctionPointerType) (void*, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 *, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___terrain0, targetMethod);
				}
			}
		}
	}
	return result;
}
// System.IAsyncResult UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TerrainFilter::BeginInvoke(UnityEngine.Terrain,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TerrainFilter_BeginInvoke_m4C17FEFE5AE0498C9A88E63A7ABEA673CD31C949 (TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * __this, Terrain_t2C0E3B3A2895E81446EFF4F5AFD601CF977D1836 * ___terrain0, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback1, RuntimeObject * ___object2, const RuntimeMethod* method)
{
	void *__d_args[2] = {0};
	__d_args[0] = ___terrain0;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback1, (RuntimeObject*)___object2);
}
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TerrainFilter::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TerrainFilter_EndInvoke_m483F3A9363FE8FDF3B5022AE2C284ACE661857B0 (TerrainFilter_t1A8786164AA07CE2D019E2B70A3217FD0F4A46E7 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result0, 0);
	return *(bool*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility_TerrainMap_TileCoord::.ctor(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileCoord__ctor_m9EED41FD3E08320CDA102E34DC65236E5137F155 (TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901 * __this, int32_t ___tileX0, int32_t ___tileZ1, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___tileX0;
		__this->set_tileX_0(L_0);
		int32_t L_1 = ___tileZ1;
		__this->set_tileZ_1(L_1);
		return;
	}
}
IL2CPP_EXTERN_C  void TileCoord__ctor_m9EED41FD3E08320CDA102E34DC65236E5137F155_AdjustorThunk (RuntimeObject * __this, int32_t ___tileX0, int32_t ___tileZ1, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901 * _thisAdjusted = reinterpret_cast<TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901 *>(__this + _offset);
	TileCoord__ctor_m9EED41FD3E08320CDA102E34DC65236E5137F155(_thisAdjusted, ___tileX0, ___tileZ1, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Vector2__ctor_m9F1F2D5EB5D1FF7091BB527AC8A72CBB309D115E_inline (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 * __this, float ___x0, float ___y1, const RuntimeMethod* method)
{
	{
		float L_0 = ___x0;
		__this->set_x_0(L_0);
		float L_1 = ___y1;
		__this->set_y_1(L_1);
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR KeyValuePair_2_t56E20A5489EE435FD8BBE3EFACF6219A626E04C0  Enumerator_get_Current_mE5033FC555E7BC63DDC919B903A8A305C3AADBEB_gshared_inline (Enumerator_t1AD96AD2810CD9FF13D02CD49EC9D4D447C1485C * __this, const RuntimeMethod* method)
{
	{
		KeyValuePair_2_t56E20A5489EE435FD8BBE3EFACF6219A626E04C0  L_0 = (KeyValuePair_2_t56E20A5489EE435FD8BBE3EFACF6219A626E04C0 )__this->get_current_3();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject * KeyValuePair_2_get_Value_mC1E2EFCF98529D0550A547CF87C6EAB6821741BF_gshared_inline (KeyValuePair_2_t56E20A5489EE435FD8BBE3EFACF6219A626E04C0 * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = (RuntimeObject *)__this->get_value_1();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR KeyValuePair_2_tCBAAE4FBE6091373C1916EE17527311382CF4551  Enumerator_get_Current_m23B8CC855231000EC661C87C6F73B91516A0DC30_gshared_inline (Enumerator_t81F0095C2D5C396203071D667A6252EFAB3D76D6 * __this, const RuntimeMethod* method)
{
	{
		KeyValuePair_2_tCBAAE4FBE6091373C1916EE17527311382CF4551  L_0 = (KeyValuePair_2_tCBAAE4FBE6091373C1916EE17527311382CF4551 )__this->get_current_3();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  KeyValuePair_2_get_Key_m212B0FE7898E52C6B11FD6DD2C01E618B497E2AD_gshared_inline (KeyValuePair_2_tCBAAE4FBE6091373C1916EE17527311382CF4551 * __this, const RuntimeMethod* method)
{
	{
		TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_0 = (TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901 )__this->get_key_0();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  Enumerator_get_Current_m3B16C98F89532B1B1779AEA703E89F4C00B89023_gshared_inline (Enumerator_t914132A1A7488464276E36385DFE4C80616AB5E9 * __this, const RuntimeMethod* method)
{
	{
		TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901  L_0 = (TileCoord_t491EABF2B90DFB255C8F7624FF5528F2DE2CC901 )__this->get_currentKey_3();
		return L_0;
	}
}
