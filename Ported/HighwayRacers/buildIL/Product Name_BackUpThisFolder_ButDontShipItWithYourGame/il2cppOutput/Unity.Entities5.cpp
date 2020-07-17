#include "pch-cpp.hpp"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <limits>
#include <stdint.h>


struct VirtActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
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
template <typename R, typename T1, typename T2, typename T3>
struct VirtFuncInvoker3
{
	typedef R (*Func)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4, typename T5>
struct VirtActionInvoker5
{
	typedef void (*Action)(void*, T1, T2, T3, T4, T5, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, p5, invokeData.method);
	}
};
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
template <typename R, typename T1, typename T2>
struct VirtFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
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
template <typename R, typename T1, typename T2, typename T3>
struct GenericVirtFuncInvoker3
{
	typedef R (*Func)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4, typename T5>
struct GenericVirtActionInvoker5
{
	typedef void (*Action)(void*, T1, T2, T3, T4, T5, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, p5, invokeData.method);
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
template <typename R, typename T1, typename T2>
struct GenericVirtFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
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
template <typename R, typename T1, typename T2, typename T3>
struct InterfaceFuncInvoker3
{
	typedef R (*Func)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4, typename T5>
struct InterfaceActionInvoker5
{
	typedef void (*Action)(void*, T1, T2, T3, T4, T5, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, p5, invokeData.method);
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
template <typename R, typename T1, typename T2>
struct InterfaceFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
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
template <typename R, typename T1, typename T2, typename T3>
struct GenericInterfaceFuncInvoker3
{
	typedef R (*Func)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4, typename T5>
struct GenericInterfaceActionInvoker5
{
	typedef void (*Action)(void*, T1, T2, T3, T4, T5, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, p5, invokeData.method);
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
template <typename R, typename T1, typename T2>
struct GenericInterfaceFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
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

// Unity.Entities.Archetype
struct Archetype_t14B6BCF53EF0F918C8B503D7100F6217E480CD94;
// System.Reflection.Binder
struct Binder_t2BEE27FD84737D1E79BC47FD67F6D3DD2F2DDA30;
// Unity.Entities.BlobAssetHeader
struct BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F;
// Unity.Entities.Chunk
struct Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248;
// Unity.Entities.ComponentDependencyManager
struct ComponentDependencyManager_tAFE29BF05294E4C3CFE1B599D078ACF14D0FAF91;
// System.Reflection.ConstructorInfo
struct ConstructorInfo_t449AEC508DCA508EE46784C4F2716545488ACD5B;
// System.Delegate
struct Delegate_t;
// System.DelegateData
struct DelegateData_t17DD30660E330C49381DAA99F934BE75CB11F288;
// Unity.Entities.Entity
struct Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4;
// Unity.Entities.EntityCommandBufferData
struct EntityCommandBufferData_t54C8C95E18B79C35853D6A104DCD8ECC9F66CBAA;
// Unity.Entities.EntityComponentStore
struct EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA;
// Unity.Entities.EntityDataAccess
struct EntityDataAccess_t14BF0F7A8F7622E7E73B5E3C0D52313B1F8D73D2;
// Unity.Entities.EntityInChunk
struct EntityInChunk_tB28DC9CD18AA63C3C99B85007BC5009DFF9F4EA3;
// Unity.Entities.EntityQueryImpl
struct EntityQueryImpl_tF4EC3E0E8B8074B8B417E3B175695ED99F01B421;
// System.Collections.IDictionary
struct IDictionary_t99871C56B8EC2452AC5C4CF3831695E617B89D3A;
// System.Reflection.MemberFilter
struct MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// Unity.Entities.PostLoadCommandBuffer
struct PostLoadCommandBuffer_tD45726986D9D5D6BF6AB396D8DD997EB3D7AD634;
// Unity.Entities.RetainBlobAssetSystem
struct RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14;
// System.Runtime.Serialization.SafeSerializationManager
struct SafeSerializationManager_tDE44F029589A028F8A3053C5C06153FAB4AAE29F;
// Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider
struct StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994;
// Unity.Entities.SystemState
struct SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95;
// Unity.Collections.LowLevel.Unsafe.UnsafeHashMapData
struct UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82;
// Unity.Collections.LowLevel.Unsafe.UnsafeList
struct UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA;
// System.Void
struct Void_t700C6383A2A510C2CF4DD86DABD5CA9FF70ADAC5;
// Unity.Entities.World
struct World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07;
// System.Reflection.CustomAttributeData/LazyCAttrData
struct LazyCAttrData_tD37F889F6B356AF76AB242D449CAEEFAE826F8C3;
// Unity.Entities.EntityCommandBuffer/PlaybackChainChunkDelegate
struct PlaybackChainChunkDelegate_tD9D47AE24B96575D178F50ABEFCB079BBCA115E9;
// Unity.Entities.EntityCommandBuffer/PlaybackUnmanagedCommandDelegate
struct PlaybackUnmanagedCommandDelegate_t2AB4784F8A033A2DEB0C88C9FA7A80D923312767;
// Unity.Entities.EntityRemapUtility/EntityRemapInfo
struct EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334;
// Unity.Entities.ScriptBehaviourUpdateOrder/DummyDelegateWrapper
struct DummyDelegateWrapper_t4DF24307E0E5460A6AE6062ED870C20D2AAB0D80;
// Unity.Entities.Serialization.SerializeUtility/ManagedObjectSerializeAdapter
struct ManagedObjectSerializeAdapter_t69285139BD4DB32D092086C7F33A7182FBD24CE9;
// Unity.Entities.Serialization.SerializeUtility/MangedObjectBlobAssetReader
struct MangedObjectBlobAssetReader_tD15931A858A4079E24BE3FBD8CE40C053A09B172;
// Unity.Entities.StructuralChange/_dlg_AddComponentChunks
struct _dlg_AddComponentChunks_tD9DDFAADFC2F439BBB9B8CEAA163B6CFF9D95D92;
// Unity.Entities.StructuralChange/_dlg_AddComponentEntitiesBatch
struct _dlg_AddComponentEntitiesBatch_tA45112FB52366C0ADDEBC039028B24BEFC59B175;
// Unity.Entities.StructuralChange/_dlg_AddComponentEntity
struct _dlg_AddComponentEntity_tCDE23991E1FA8C395DCCC00BCD0316E834300D95;
// Unity.Entities.StructuralChange/_dlg_AddSharedComponentChunks
struct _dlg_AddSharedComponentChunks_t0BF4E29BC434846F82C9401CDF67A851FA5A58E3;
// Unity.Entities.StructuralChange/_dlg_CreateEntity
struct _dlg_CreateEntity_t418D1599A9300D4C8883E329439F01C2CB94856F;
// Unity.Entities.StructuralChange/_dlg_InstantiateEntities
struct _dlg_InstantiateEntities_t1F5957B60C35DE9F8691CBDC46C67DCD4A29B678;
// Unity.Entities.StructuralChange/_dlg_MoveEntityArchetype
struct _dlg_MoveEntityArchetype_t8C03493F5C6FB58AAECDD0B6DBFA736F5BD204A5;
// Unity.Entities.StructuralChange/_dlg_RemoveComponentChunks
struct _dlg_RemoveComponentChunks_t156D7CDE1CB0A8A575514C20309E1392F8BE65F9;
// Unity.Entities.StructuralChange/_dlg_RemoveComponentEntitiesBatch
struct _dlg_RemoveComponentEntitiesBatch_t092F3F9E00E3B1F1F6A33A456E19C0E7ABE992E1;
// Unity.Entities.StructuralChange/_dlg_RemoveComponentEntity
struct _dlg_RemoveComponentEntity_t031F2F7279291D7654E794B4B7EB273464D764E6;
// Unity.Entities.StructuralChange/_dlg_SetChunkComponent
struct _dlg_SetChunkComponent_t58428EAC3CAA8BEF48D7DEB8E71516FB3619524E;
// Unity.Entities.SystemBaseRegistry/ForwardingFunc
struct ForwardingFunc_t830DB8E27A008A51CBCBAAB812E1F2A06C2E89F1;
// Unity.Entities.TypeHash/<>c
struct U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A;
// Unity.Entities.TypeManager/<>c
struct U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2;
// Unity.Entities.TypeManager/EntityOffsetInfo
struct EntityOffsetInfo_t398BFA31DBBAAA0488E257A761D3D254D716DE89;
// Unity.Entities.TypeManager/TypeInfo
struct TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38;
// Unity.Properties.Generated.Unity_Entities_Entity_PropertyBag/Index_Property
struct Index_Property_t835C3FA617A3E01A668DA48CE5835BE4ECB9F808;
// Unity.Properties.Generated.Unity_Entities_Entity_PropertyBag/Version_Property
struct Version_Property_tA459AC543294951D12631B8CC94B7319B6423CC1;
// Unity.Properties.Generated.Unity_Entities_Hash128_PropertyBag/Value_Property
struct Value_Property_t9606F88DB5D8B3CB45FE05D0D3CC24AE4CCCA547;
// Unity.Properties.Generated.Unity_Entities_PostLoadCommandBuffer_PropertyBag/CommandBuffer_Property
struct CommandBuffer_Property_tDCB2D8FF230260A52B1CB3A97E5BC39EEAE1BBFF;
// Unity.Properties.Generated.Unity_Entities_SceneSection_PropertyBag/SceneGUID_Property
struct SceneGUID_Property_t286F056FA890C76A85CAEED96FB8F2DC18718F24;
// Unity.Properties.Generated.Unity_Entities_SceneSection_PropertyBag/Section_Property
struct Section_Property_t1AB3E9286E418F5B1D6EA8E68E25BFF01B901BDB;
// Unity.Properties.Generated.Unity_Entities_SceneTag_PropertyBag/SceneEntity_Property
struct SceneEntity_Property_tDA7084AEB4F0BEF5863A78E97A21028595F2E2E0;
// Unity.Properties.Generated.Unity_Entities_SectionMetadataSetup_PropertyBag/SceneSectionIndex_Property
struct SceneSectionIndex_Property_t123ADC180D58E59803781FA9CA0C27F3F4AD9FA6;
// Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag/w_Property
struct w_Property_tE1DFF192A694126418686CC4C65AD495A7670315;
// Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag/x_Property
struct x_Property_tC992C828E7E67B8457C9631A95F7502D91029124;
// Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag/y_Property
struct y_Property_t917A1CEE407CE842A5B28FC2850257DC84226D5D;
// Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag/z_Property
struct z_Property_tCC6F6AC68C86D8C1E6BCFFE6762B55DA5B817676;
// Unity.Entities.World/StateAllocLevel1
struct StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82;
// Unity.Entities.EntityPatcher/EntityDiffPatcher/EntityPatchAdapter
struct EntityPatchAdapter_tE216B50635D7EEC4301D9C54E3F1894F658D59AC;
// Unity.Entities.FastEquality/TypeInfo/CompareEqualDelegate
struct CompareEqualDelegate_t15914FD0CE4F07A1C8E6EA6B2A931D83586220D7;
// Unity.Entities.FastEquality/TypeInfo/GetHashCodeDelegate
struct GetHashCodeDelegate_t0463D39E4D6E298D2C15D08C75CAFB0CBF02E353;
// Unity.Entities.FastEquality/TypeInfo/ManagedCompareEqualDelegate
struct ManagedCompareEqualDelegate_t1D9D97E36B8D0245138610749B6F2B74D6CEBB06;
// Unity.Entities.FastEquality/TypeInfo/ManagedGetHashCodeDelegate
struct ManagedGetHashCodeDelegate_tDC6EDBDBB5F0F94C90DFCB41F6692CBA3B2A75DC;
// Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders
struct LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31;
// Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders/Runtimes
struct Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8;
// System.Collections.Generic.Dictionary`2<System.Type,Unity.Entities.ComponentSystemBase>
struct Dictionary_2_tD6066C27E39214D9F5B08C241F9C5FB62A7B63B3;
// System.Collections.Generic.Dictionary`2<System.Type,System.Exception>
struct Dictionary_2_tB94CC7A94D357C2FE46DCF5A87B29DE8F7128C4E;
// System.Collections.Generic.Dictionary`2<System.Type,System.Int32>
struct Dictionary_2_t6CE7336785D73EB9AC6DBFDBCB55D4CF15047CB7;
// System.Func`2<System.Reflection.CustomAttributeData,System.Boolean>
struct Func_2_t59D027CBCE861EDD5A5B228EE56B105FB06A1B49;
// System.Func`2<System.Reflection.CustomAttributeTypedArgument,System.Boolean>
struct Func_2_tB3B29867956DE52CE627DBF0C158133CD7A2F46E;
// System.Func`2<System.Reflection.FieldInfo,System.Boolean>
struct Func_2_t0BE5B54DD65017DAF1DC5DEC5A896A2B0550F8AE;
// System.Collections.Generic.IList`1<System.Reflection.CustomAttributeNamedArgument>
struct IList_1_tC94A6A591E58FD9BB826AF5D15001E425B682707;
// System.Collections.Generic.IList`1<System.Reflection.CustomAttributeTypedArgument>
struct IList_1_tA9B3F6D4DDBA3A555103C2DDC65AD75936EAB181;
// System.Collections.Generic.List`1<Unity.Entities.ComponentSystemBase>
struct List_1_t3F15B202D7C7B7EEB8B86FFA8C5D2156C6F32C78;
// System.Collections.Generic.List`1<Unity.Entities.World>
struct List_1_t84AC7439EE119A84B7D9198C8EB4AC483255C192;
// System.Collections.Generic.List`1<Unity.Entities.FastEquality/TypeInfo>
struct List_1_tB206DD21A43E0C00DFC00E7D80011FF29ECC2BBB;
// System.Collections.Generic.List`1<System.Attribute>
struct List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10;
// System.Collections.Generic.List`1<System.String>
struct List_1_t6C9F81EDBF0F4A31A9B0DA372D2EF34BDA3A1AF3;
// System.Collections.Generic.List`1<System.Type>
struct List_1_t7CFD5FCE8366620F593F2C9DAC3A870E5D6506D7;
// Unity.Properties.Property`2<Unity.Entities.Entity,System.Int32>
struct Property_2_tCABF4DCC1CD1B4A9380E57084825CBE0CD5650CF;
// Unity.Properties.Property`2<Unity.Entities.Hash128,Unity.Mathematics.uint4>
struct Property_2_tC735BFFF1BC3639C475ACE130B7C17946AADC830;
// Unity.Properties.Property`2<Unity.Entities.PostLoadCommandBuffer,Unity.Entities.EntityCommandBuffer>
struct Property_2_t40BE748BD8D19926CED3273AD0DAECECE3176A34;
// Unity.Properties.Property`2<Unity.Entities.SceneSection,Unity.Entities.Hash128>
struct Property_2_t2BF85B4F1B3C1A7B861BCF86F0135D4C29B3E71A;
// Unity.Properties.Property`2<Unity.Entities.SceneSection,System.Int32>
struct Property_2_t429FDAC3FDDA865DCCA0BEA2701D06815301B7A7;
// Unity.Properties.Property`2<Unity.Entities.SceneTag,Unity.Entities.Entity>
struct Property_2_tEAB8BD93E42D6310C52481E57E5BE3E4F63917B0;
// Unity.Properties.Property`2<Unity.Entities.SectionMetadataSetup,System.Int32>
struct Property_2_t300D32A567141F1F72A198697AF2D006E2898B23;
// Unity.Properties.Property`2<System.Object,Unity.Entities.EntityCommandBuffer>
struct Property_2_t1AAAEA67B372BF7DAC1D25BD2C1DC5976FE80FEE;
// Unity.Properties.Property`2<Unity.Mathematics.uint4,System.UInt32>
struct Property_2_tB965C4D1501C79D2FADA36497D75F6F4BEC5DAED;
// System.AsyncCallback
struct AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA;
// System.Byte
struct Byte_t0111FAB8B8685667EDDAF77683F0D8F86B659056;
// System.Char[]
struct CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34;
// Unity.Entities.ComponentSystemBase
struct ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC;
// System.Reflection.CustomAttributeData
struct CustomAttributeData_t4F8D66DDB6D3F7E8C39AF85752A0CC9679A4CE85;
// System.Delegate[]
struct DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8;
// System.EventArgs
struct EventArgs_tBCAACA538A5195B6D6C8DFCC3524A2A4A67FD8BA;
// System.EventHandler
struct EventHandler_t084491E53EC706ACA0A15CA17488C075B4ECA44B;
// System.Reflection.FieldInfo
struct FieldInfo_t;
// System.IAsyncResult
struct IAsyncResult_tC9F97BF36FCF122D29D3101D80642278297BF370;
// System.Int32
struct Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046;
// System.IntPtr[]
struct IntPtrU5BU5D_t27FC72B0409D75AAF33EC42498E8094E95FEE9A6;
// System.InvalidOperationException
struct InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB;
// System.SByte[]
struct SByteU5BU5D_t7D94C53295E6116625EA7CC7DEA21FEDC39869E7;
// System.Diagnostics.StackTrace[]
struct StackTraceU5BU5D_t4AD999C288CB6D1F38A299D12B1598D606588971;
// System.String
struct String_t;
// System.Type
struct Type_t;
// System.Type[]
struct TypeU5BU5D_t85B10489E46F06CEC7C4B1CCBD0E01FAB6649755;
// System.UInt32
struct UInt32_tE60352A06233E4E69DD198BCC67142159F686B15;
// Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider/PerformLambdaDelegate
struct PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377;

IL2CPP_EXTERN_C RuntimeClass* Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IntPtr_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SharedBlobAssetRefOffset_tE4EC29E58257EEC230D11A35AB22B47D9D9DC0E6_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SharedEntityOffsetInfo_tE11E3366E1064E671CD7B5C0B457A4A09DCCC068_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SharedTypeInfo_t6D9232712A2DC192E8DCC16F0AE24EE2508CCD05_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SharedWriteGroup_t43861216B5BB89853E40798D5170B5574E8A46FB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Type_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral062DB096C728515E033CF8C48A1C1F0B9A79384B;
IL2CPP_EXTERN_C String_t* _stringLiteral1C3B83E7128DFE5344885801249731AA7F849057;
IL2CPP_EXTERN_C String_t* _stringLiteral2B34CB86005E87B20947F06933F7E295DE02BF17;
IL2CPP_EXTERN_C String_t* _stringLiteral3BD5E620A47C36E674F4EB9456A8E5DECDEE7FF7;
IL2CPP_EXTERN_C String_t* _stringLiteral3EBA566FB73F37AAE39B99437ACB61532CD06333;
IL2CPP_EXTERN_C String_t* _stringLiteral71086E7E934954D519A3A2106B8936D9C7D8CCD4;
IL2CPP_EXTERN_C String_t* _stringLiteral86FADB31129B6F40C720A97600D69389EA3567E3;
IL2CPP_EXTERN_C String_t* _stringLiteral9384C6EF2DA5C0BD5274A0DACFF291D0ABBFD8B1;
IL2CPP_EXTERN_C String_t* _stringLiteral96532ED47BDF4CA75B598293EEC8CE8C6A78536E;
IL2CPP_EXTERN_C String_t* _stringLiteral9CE1604D659135925CCC4DD1F526AFFE42E689F1;
IL2CPP_EXTERN_C String_t* _stringLiteralA1A661195E7ABA07DF5C7981C6DCA906B7120C55;
IL2CPP_EXTERN_C String_t* _stringLiteralA9FEAF5F50923952C1AC3A473DE3C7E17D23B907;
IL2CPP_EXTERN_C String_t* _stringLiteralBC29A683F5FE1BCDDC8B571A7EF16BE16E4142B2;
IL2CPP_EXTERN_C String_t* _stringLiteralBF45CADC16AD267EA891B4231D162B68FDED748D;
IL2CPP_EXTERN_C String_t* _stringLiteralDB47297909F3BD6EDB8AD67A8511975233214355;
IL2CPP_EXTERN_C String_t* _stringLiteralE200AC1425952F4F5CEAAA9C773B6D17B90E47C1;
IL2CPP_EXTERN_C String_t* _stringLiteralE3AB954C27345B5777E41817C31696D6AC0E87C1;
IL2CPP_EXTERN_C String_t* _stringLiteralE6E963A8B9868C07D45F9CC0146A363316992337;
IL2CPP_EXTERN_C String_t* _stringLiteralE8F1C389BD6A872E97A9FA999AECFCFB55A3E3A7;
IL2CPP_EXTERN_C String_t* _stringLiteralF06A9A7490586E316F66A3CF172167416FF081DE;
IL2CPP_EXTERN_C const RuntimeMethod* LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_m2D6A743E5140666061CD5420D95250903AFED096_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mD35E9088251B87C54D4C9FE9CE4B2C68F7661646_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mFB4F12F46E5EC3C3B81DFD695837BB9B81A28A3E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* LambdaParameterValueProvider_ISharedComponentData_1_PrepareToExecuteWithStructuralChanges_m2A619A397B3B2222985218AA7CEA6259FEEA0337_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Deserialize_m4B3E05769084529A62D42C23485B55B1AB626B4A_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Serialize_m9E1CC1A14745154680CA5776052A64EDBBD221E3_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Deserialize_m4D1F57847E41AD0699B8F8204A2CD77733C8937E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Serialize_mAA193EB803F57CD76DD8852924B5B95079C5B73D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Serialize_m8A95B0E4D0A1D8928B0068194A6227BCB6BC87E0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* NativeHashMap_2_TryGetValue_mF556662A50EBD61DC1759DC04065BB6A7BE7CDF2_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Property_2__ctor_m0B519A8910790B943F6709C19F8F47DA8C45B78F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Property_2__ctor_m2B068683806A6A583C633FCEE64198D60266B4E1_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Property_2__ctor_m715A2E230FCB05B99C9E2E5ECF6C9660116D19E7_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Property_2__ctor_m7DE529508EC4AD847F196ED89B9D7608D3976EAD_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Property_2__ctor_mA10C37A89F64B051C9DA6010078BDDCF63F886B2_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Property_2__ctor_mAF509B7CDC2036D7E9EBBF46BD4B439073CDE45C_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Property_2__ctor_mAF911EBD92AA72B392E830E0A324C3D398CF7AB7_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Property_2__ctor_mED553372B69CC9749E50E387DE73DD0DDAC6A108_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Reader_ReadNext_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m660DE3F4792554904EAA494B9CD2C4BAE76C0C41_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedBlobAssetRefOffset_tE4EC29E58257EEC230D11A35AB22B47D9D9DC0E6_m09EB536037556C2CC390754DB83DA6B73AD52912_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedEntityOffsetInfo_tE11E3366E1064E671CD7B5C0B457A4A09DCCC068_mEA2E884221A4CEEBE506AB0F64472ABEC48A85DE_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedTypeInfo_t6D9232712A2DC192E8DCC16F0AE24EE2508CCD05_m4D2E48203248786C9D3BFF7BC2205CE779FE7814_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedWriteGroup_t43861216B5BB89853E40798D5170B5574E8A46FB_mFA700697C79F3EDB2BC6703FE1267C962261E842_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SharedStatic_1_GetOrCreate_m379EE72FB569F3A26DCF93108ABBF8F81A1CE4E5_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SharedStatic_1_get_Data_m6B89546E4ADA23EA53D35BF24C7B9DF1194CFE25_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* StructuralChangeRuntime_For_m5BC634C6DEE1A6F713D79A04682F72C67D2C0EA1_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* StructuralChangeRuntime_For_mC3966AA37DF1BC7582DF6CA3FDAE580A9B61BA9F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* StructuralChangeRuntime_WriteBack_m4F94AC6D28EDD77E7AE73F2E887D0D0D2C6DA0F4_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* StructuralChangeRuntime_WriteBack_mD7AE5E6DE2459A9CFB3A07C9BAAB44576C4587F7_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_PerformLambda_mD6D6751E15D9AE4174AD71C60EC10E32D656D267_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* UnsafeAppendBuffer_Add_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m64B262F4DBD6071954E99B75483BA5039BD3F49B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* UnsafeUtilityEx_AsRef_TisRuntimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_m196732DD907ED66A6BBB3E110D5E3B755064D826_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* UnsafeUtilityEx_AsRef_TisU3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_mC32F5BF943A75A98DEF186E75D33781DF05A00A9_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeType* TypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_0_0_0_var;
IL2CPP_EXTERN_C const uint32_t CommandBuffer_Property__ctor_mD65857B5D94D9A5B1A0894359E790EEFF1B858BE_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t CommandBuffer_Property_get_Name_m7A58893B7B27FFDAC8EB01461B1DB64A332F8820_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ForwardingFunc_BeginInvoke_m9BE4143ED642DE88540FE07C6D91BF0B9E0F7F7E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Index_Property__ctor_m0365092A2589D0566ACEA4FB53EDC1FBF118CDBF_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Index_Property_get_Name_m1EE91534EE9827009132CEAA903EF8D4E8F9D3A3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_m06AE7D081E5B1F77F983D9608E29D6E801B94B94_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_m5D4BC103CAC65745AB7DF1CB09E4DC7CEF6DFE9D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_mAC1D60747A9EA28974FBF31EE269AE7161E901D8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Deserialize_m4B3E05769084529A62D42C23485B55B1AB626B4A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Serialize_m9E1CC1A14745154680CA5776052A64EDBBD221E3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Deserialize_m4D1F57847E41AD0699B8F8204A2CD77733C8937E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Serialize_m7B87363B869D66839BC2D8CBCBC581A76D25211E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Deserialize_m990A987C1B24232DBAD02BA6FF9E5589E18EB8F3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Serialize_mAA193EB803F57CD76DD8852924B5B95079C5B73D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Deserialize_m5C92A0DB524EC491E26776958C177490A86DBCBA_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Serialize_m8A95B0E4D0A1D8928B0068194A6227BCB6BC87E0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PerformLambdaDelegate_BeginInvoke_m77A90CD6A1AF9013DFD3C84EAA6A5A9925406C13_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SceneEntity_Property__ctor_m4F1C45701726F5AA5BA7AA50EE3BE4E050DD5922_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SceneEntity_Property_get_Name_mFE241352612BB2281F37EF4A63E14A77ABEAACDF_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SceneGUID_Property__ctor_m5626743899E620550A96D3FFE3DB260E87CCB8C1_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SceneGUID_Property_get_Name_mD2175AEBD47F37A29024711879155CF8D56BDAC6_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SceneSectionIndex_Property__ctor_m4D79DC51D6459002B01F1A444DFE53C2DC5B395C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SceneSectionIndex_Property_get_Name_m1C00A80D33667C071D4A4F60CFC614EDFD683C24_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Section_Property__ctor_m765F4D5B13102563E873DF8D9B369DEE0BA86B2E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Section_Property_get_Name_m07068B673845F0CAA7920BCFB234C3B2FF235B68_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SharedBlobAssetRefOffset__cctor_m1865D544722C86A5F9ACD4D021162D19F44E1944_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SharedEntityOffsetInfo__cctor_mA47563747CD46ED81C5996C17DDA196B92504377_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SharedTypeIndex_Get_m823250A31F835709E0B87C8C5D29472F8712B37C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SharedTypeInfo__cctor_m7BAB99895484D0E3D3E5C2455EC9A59095F050A0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SharedWriteGroup__cctor_m21C0D1BC37FB6ED00482B41D1D1EADD0BCF8ABCB_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TypeInfo_get_Type_m966C85EAB51370AE906FC730D10936E331F86A93_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CU3Ec_U3CCalculateMemoryOrderingU3Eb__11_0_m0658D174D3687AFE70C7CE87804DB2DEFE0895B3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CU3Ec_U3CCalculateMemoryOrderingU3Eb__11_1_m06A0D3D7E3AB041CB2773B2309CC4118FCAC529B_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CU3Ec_U3CHashVersionAttributeU3Eb__7_0_m005F5ABE146B2F1B4FC02CF8B514EBF4C2FDA269_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CU3Ec_U3CHashVersionAttributeU3Eb__7_1_mDA18EA52F03F78817E0ADB3854E9D9D846A9D3DE_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CU3Ec_U3CInitializeU3Eb__80_0_m11CF863FFFFD5B08197FAB68478BD120D93CD06E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CU3Ec_U3CIsZeroSizeStructU3Eb__113_0_mE15E768980DA3CB9C546597A26A608EEA207D844_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_Execute_m42FFAB4AF04DA43543F52A7D37FC0DBE46632952_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_PerformLambda_mD6D6751E15D9AE4174AD71C60EC10E32D656D267_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2__cctor_mDDEEBAB2A3BBAA86821146E7C56E1F45229E8817_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CU3Ec__cctor_m631D18F5F25646C0A46FC0C558EA2393072F392E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CU3Ec__cctor_mC79ADB26176127E222A9CBF9F39F858B8B7375E9_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Value_Property__ctor_m63FF8583BDEAD99890E15799B4FEFB3F5CBFB4B1_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Value_Property_get_Name_m91BDFD5706CDECF057C3473B1ED3B2E9D985B1C8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Version_Property__ctor_mD380F341F45811DD41CFC377B17A40EE7FF1CB1F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Version_Property_get_Name_m32E62DA78321922E4520108641F397222967630E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _dlg_AddComponentChunks_BeginInvoke_mC267BD0DB0AE2BA18C10578ED5475458AFD6C811_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _dlg_AddComponentEntitiesBatch_BeginInvoke_m87A7EF41430ED10895A5DC0D558A11077C9A84ED_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _dlg_AddComponentEntity_BeginInvoke_mE3DFBBCE973CC4CA8E79D20918AF51D2E4364950_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _dlg_AddSharedComponentChunks_BeginInvoke_m009E6E1F120D698256D3A01D8FC89CF6F1CF7BDC_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _dlg_CreateEntity_BeginInvoke_mC94624274AAFC20D0C15497F2FD6F72BA90C51A4_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _dlg_InstantiateEntities_BeginInvoke_m4A258509A42C18578D8BC4F9841882BAC6AF670E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _dlg_RemoveComponentChunks_BeginInvoke_m38B404E4A10C3436F0DAEC70087FF7AA69A2C8FA_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _dlg_RemoveComponentEntitiesBatch_BeginInvoke_mDB2EE88F6AFB5912A8896D6A13FEDA04EA42C323_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _dlg_RemoveComponentEntity_BeginInvoke_mEB560C483E03EB71F4961EB807CA6CB841EF08BD_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _dlg_SetChunkComponent_BeginInvoke_m3F143241350F543B1D56C4E99576F34A390E300B_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t w_Property__ctor_m3DBEC411D2077B7F0039591E04FDACDDE899DD55_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t w_Property_get_Name_m8FF791A2369404ACB515FB598BBAACF466C94271_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t x_Property__ctor_m47D3D48E4684A939CEC75D9483B77835B7DD0F2A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t x_Property_get_Name_m47585DCC880E4FFF8E58CACE1B2A5C48CAFC4353_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t y_Property__ctor_m8653823CCFE3F1E47EDD0F08563248004C3289DC_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t y_Property_get_Name_m4C38239EACA3A992C12CF469F4C43C2C67E81CBA_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t z_Property__ctor_m66D527839149FE89BC3DB1177FAFF3D418A2BE58_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t z_Property_get_Name_m3ED396DB84C1F4938F600674C1407BB917A83144_MetadataUsageId;
struct Archetype_t14B6BCF53EF0F918C8B503D7100F6217E480CD94 ;
struct Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 ;
struct ComponentDependencyManager_tAFE29BF05294E4C3CFE1B599D078ACF14D0FAF91 ;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;
struct EntityCommandBufferData_t54C8C95E18B79C35853D6A104DCD8ECC9F66CBAA ;
struct EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA ;
struct EntityDataAccess_t14BF0F7A8F7622E7E73B5E3C0D52313B1F8D73D2 ;
struct EntityInChunk_tB28DC9CD18AA63C3C99B85007BC5009DFF9F4EA3 ;
struct EntityOffsetInfo_t398BFA31DBBAAA0488E257A761D3D254D716DE89 ;
struct EntityQueryImpl_tF4EC3E0E8B8074B8B417E3B175695ED99F01B421 ;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;
struct LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31;;
struct LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_com;
struct LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_com;;
struct LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_pinvoke;
struct LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_pinvoke;;
struct Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 ;
struct StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 ;
struct StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994;;
struct StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_com;
struct StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_com;;
struct StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_pinvoke;
struct StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_pinvoke;;
struct SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 ;
struct TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 ;

struct DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// System.Object

struct Il2CppArrayBounds;

// System.Array


// System.EventArgs
struct  EventArgs_tBCAACA538A5195B6D6C8DFCC3524A2A4A67FD8BA  : public RuntimeObject
{
public:

public:
};

struct EventArgs_tBCAACA538A5195B6D6C8DFCC3524A2A4A67FD8BA_StaticFields
{
public:
	// System.EventArgs System.EventArgs::Empty
	EventArgs_tBCAACA538A5195B6D6C8DFCC3524A2A4A67FD8BA * ___Empty_0;

public:
	inline static int32_t get_offset_of_Empty_0() { return static_cast<int32_t>(offsetof(EventArgs_tBCAACA538A5195B6D6C8DFCC3524A2A4A67FD8BA_StaticFields, ___Empty_0)); }
	inline EventArgs_tBCAACA538A5195B6D6C8DFCC3524A2A4A67FD8BA * get_Empty_0() const { return ___Empty_0; }
	inline EventArgs_tBCAACA538A5195B6D6C8DFCC3524A2A4A67FD8BA ** get_address_of_Empty_0() { return &___Empty_0; }
	inline void set_Empty_0(EventArgs_tBCAACA538A5195B6D6C8DFCC3524A2A4A67FD8BA * value)
	{
		___Empty_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Empty_0), (void*)value);
	}
};


// System.Reflection.CustomAttributeData
struct  CustomAttributeData_t4F8D66DDB6D3F7E8C39AF85752A0CC9679A4CE85  : public RuntimeObject
{
public:
	// System.Reflection.ConstructorInfo System.Reflection.CustomAttributeData::ctorInfo
	ConstructorInfo_t449AEC508DCA508EE46784C4F2716545488ACD5B * ___ctorInfo_0;
	// System.Collections.Generic.IList`1<System.Reflection.CustomAttributeTypedArgument> System.Reflection.CustomAttributeData::ctorArgs
	RuntimeObject* ___ctorArgs_1;
	// System.Collections.Generic.IList`1<System.Reflection.CustomAttributeNamedArgument> System.Reflection.CustomAttributeData::namedArgs
	RuntimeObject* ___namedArgs_2;
	// System.Reflection.CustomAttributeData_LazyCAttrData System.Reflection.CustomAttributeData::lazyData
	LazyCAttrData_tD37F889F6B356AF76AB242D449CAEEFAE826F8C3 * ___lazyData_3;

public:
	inline static int32_t get_offset_of_ctorInfo_0() { return static_cast<int32_t>(offsetof(CustomAttributeData_t4F8D66DDB6D3F7E8C39AF85752A0CC9679A4CE85, ___ctorInfo_0)); }
	inline ConstructorInfo_t449AEC508DCA508EE46784C4F2716545488ACD5B * get_ctorInfo_0() const { return ___ctorInfo_0; }
	inline ConstructorInfo_t449AEC508DCA508EE46784C4F2716545488ACD5B ** get_address_of_ctorInfo_0() { return &___ctorInfo_0; }
	inline void set_ctorInfo_0(ConstructorInfo_t449AEC508DCA508EE46784C4F2716545488ACD5B * value)
	{
		___ctorInfo_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___ctorInfo_0), (void*)value);
	}

	inline static int32_t get_offset_of_ctorArgs_1() { return static_cast<int32_t>(offsetof(CustomAttributeData_t4F8D66DDB6D3F7E8C39AF85752A0CC9679A4CE85, ___ctorArgs_1)); }
	inline RuntimeObject* get_ctorArgs_1() const { return ___ctorArgs_1; }
	inline RuntimeObject** get_address_of_ctorArgs_1() { return &___ctorArgs_1; }
	inline void set_ctorArgs_1(RuntimeObject* value)
	{
		___ctorArgs_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___ctorArgs_1), (void*)value);
	}

	inline static int32_t get_offset_of_namedArgs_2() { return static_cast<int32_t>(offsetof(CustomAttributeData_t4F8D66DDB6D3F7E8C39AF85752A0CC9679A4CE85, ___namedArgs_2)); }
	inline RuntimeObject* get_namedArgs_2() const { return ___namedArgs_2; }
	inline RuntimeObject** get_address_of_namedArgs_2() { return &___namedArgs_2; }
	inline void set_namedArgs_2(RuntimeObject* value)
	{
		___namedArgs_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___namedArgs_2), (void*)value);
	}

	inline static int32_t get_offset_of_lazyData_3() { return static_cast<int32_t>(offsetof(CustomAttributeData_t4F8D66DDB6D3F7E8C39AF85752A0CC9679A4CE85, ___lazyData_3)); }
	inline LazyCAttrData_tD37F889F6B356AF76AB242D449CAEEFAE826F8C3 * get_lazyData_3() const { return ___lazyData_3; }
	inline LazyCAttrData_tD37F889F6B356AF76AB242D449CAEEFAE826F8C3 ** get_address_of_lazyData_3() { return &___lazyData_3; }
	inline void set_lazyData_3(LazyCAttrData_tD37F889F6B356AF76AB242D449CAEEFAE826F8C3 * value)
	{
		___lazyData_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___lazyData_3), (void*)value);
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

// Unity.Entities.ComponentSystemBase
struct  ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC  : public RuntimeObject
{
public:
	// Unity.Entities.SystemState* Unity.Entities.ComponentSystemBase::m_StatePtr
	SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * ___m_StatePtr_0;

public:
	inline static int32_t get_offset_of_m_StatePtr_0() { return static_cast<int32_t>(offsetof(ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC, ___m_StatePtr_0)); }
	inline SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * get_m_StatePtr_0() const { return ___m_StatePtr_0; }
	inline SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 ** get_address_of_m_StatePtr_0() { return &___m_StatePtr_0; }
	inline void set_m_StatePtr_0(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * value)
	{
		___m_StatePtr_0 = value;
	}
};


// Unity.Entities.ScriptBehaviourUpdateOrder_DummyDelegateWrapper
struct  DummyDelegateWrapper_t4DF24307E0E5460A6AE6062ED870C20D2AAB0D80  : public RuntimeObject
{
public:
	// Unity.Entities.ComponentSystemBase Unity.Entities.ScriptBehaviourUpdateOrder_DummyDelegateWrapper::m_System
	ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___m_System_0;

public:
	inline static int32_t get_offset_of_m_System_0() { return static_cast<int32_t>(offsetof(DummyDelegateWrapper_t4DF24307E0E5460A6AE6062ED870C20D2AAB0D80, ___m_System_0)); }
	inline ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * get_m_System_0() const { return ___m_System_0; }
	inline ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC ** get_address_of_m_System_0() { return &___m_System_0; }
	inline void set_m_System_0(ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * value)
	{
		___m_System_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_System_0), (void*)value);
	}
};


// Unity.Entities.Serialization.SerializeUtility_MangedObjectBlobAssetReader
struct  MangedObjectBlobAssetReader_tD15931A858A4079E24BE3FBD8CE40C053A09B172  : public RuntimeObject
{
public:
	// System.Byte* Unity.Entities.Serialization.SerializeUtility_MangedObjectBlobAssetReader::m_BlobAssetBatch
	uint8_t* ___m_BlobAssetBatch_0;

public:
	inline static int32_t get_offset_of_m_BlobAssetBatch_0() { return static_cast<int32_t>(offsetof(MangedObjectBlobAssetReader_tD15931A858A4079E24BE3FBD8CE40C053A09B172, ___m_BlobAssetBatch_0)); }
	inline uint8_t* get_m_BlobAssetBatch_0() const { return ___m_BlobAssetBatch_0; }
	inline uint8_t** get_address_of_m_BlobAssetBatch_0() { return &___m_BlobAssetBatch_0; }
	inline void set_m_BlobAssetBatch_0(uint8_t* value)
	{
		___m_BlobAssetBatch_0 = value;
	}
};


// Unity.Entities.TypeHash_<>c
struct  U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A  : public RuntimeObject
{
public:

public:
};

struct U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A_StaticFields
{
public:
	// Unity.Entities.TypeHash_<>c Unity.Entities.TypeHash_<>c::<>9
	U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A * ___U3CU3E9_0;
	// System.Func`2<System.Reflection.CustomAttributeData,System.Boolean> Unity.Entities.TypeHash_<>c::<>9__7_0
	Func_2_t59D027CBCE861EDD5A5B228EE56B105FB06A1B49 * ___U3CU3E9__7_0_1;
	// System.Func`2<System.Reflection.CustomAttributeTypedArgument,System.Boolean> Unity.Entities.TypeHash_<>c::<>9__7_1
	Func_2_tB3B29867956DE52CE627DBF0C158133CD7A2F46E * ___U3CU3E9__7_1_2;
	// System.Func`2<System.Reflection.CustomAttributeData,System.Boolean> Unity.Entities.TypeHash_<>c::<>9__11_0
	Func_2_t59D027CBCE861EDD5A5B228EE56B105FB06A1B49 * ___U3CU3E9__11_0_3;
	// System.Func`2<System.Reflection.CustomAttributeTypedArgument,System.Boolean> Unity.Entities.TypeHash_<>c::<>9__11_1
	Func_2_tB3B29867956DE52CE627DBF0C158133CD7A2F46E * ___U3CU3E9__11_1_4;

public:
	inline static int32_t get_offset_of_U3CU3E9_0() { return static_cast<int32_t>(offsetof(U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A_StaticFields, ___U3CU3E9_0)); }
	inline U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A * get_U3CU3E9_0() const { return ___U3CU3E9_0; }
	inline U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A ** get_address_of_U3CU3E9_0() { return &___U3CU3E9_0; }
	inline void set_U3CU3E9_0(U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A * value)
	{
		___U3CU3E9_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CU3E9_0), (void*)value);
	}

	inline static int32_t get_offset_of_U3CU3E9__7_0_1() { return static_cast<int32_t>(offsetof(U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A_StaticFields, ___U3CU3E9__7_0_1)); }
	inline Func_2_t59D027CBCE861EDD5A5B228EE56B105FB06A1B49 * get_U3CU3E9__7_0_1() const { return ___U3CU3E9__7_0_1; }
	inline Func_2_t59D027CBCE861EDD5A5B228EE56B105FB06A1B49 ** get_address_of_U3CU3E9__7_0_1() { return &___U3CU3E9__7_0_1; }
	inline void set_U3CU3E9__7_0_1(Func_2_t59D027CBCE861EDD5A5B228EE56B105FB06A1B49 * value)
	{
		___U3CU3E9__7_0_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CU3E9__7_0_1), (void*)value);
	}

	inline static int32_t get_offset_of_U3CU3E9__7_1_2() { return static_cast<int32_t>(offsetof(U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A_StaticFields, ___U3CU3E9__7_1_2)); }
	inline Func_2_tB3B29867956DE52CE627DBF0C158133CD7A2F46E * get_U3CU3E9__7_1_2() const { return ___U3CU3E9__7_1_2; }
	inline Func_2_tB3B29867956DE52CE627DBF0C158133CD7A2F46E ** get_address_of_U3CU3E9__7_1_2() { return &___U3CU3E9__7_1_2; }
	inline void set_U3CU3E9__7_1_2(Func_2_tB3B29867956DE52CE627DBF0C158133CD7A2F46E * value)
	{
		___U3CU3E9__7_1_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CU3E9__7_1_2), (void*)value);
	}

	inline static int32_t get_offset_of_U3CU3E9__11_0_3() { return static_cast<int32_t>(offsetof(U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A_StaticFields, ___U3CU3E9__11_0_3)); }
	inline Func_2_t59D027CBCE861EDD5A5B228EE56B105FB06A1B49 * get_U3CU3E9__11_0_3() const { return ___U3CU3E9__11_0_3; }
	inline Func_2_t59D027CBCE861EDD5A5B228EE56B105FB06A1B49 ** get_address_of_U3CU3E9__11_0_3() { return &___U3CU3E9__11_0_3; }
	inline void set_U3CU3E9__11_0_3(Func_2_t59D027CBCE861EDD5A5B228EE56B105FB06A1B49 * value)
	{
		___U3CU3E9__11_0_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CU3E9__11_0_3), (void*)value);
	}

	inline static int32_t get_offset_of_U3CU3E9__11_1_4() { return static_cast<int32_t>(offsetof(U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A_StaticFields, ___U3CU3E9__11_1_4)); }
	inline Func_2_tB3B29867956DE52CE627DBF0C158133CD7A2F46E * get_U3CU3E9__11_1_4() const { return ___U3CU3E9__11_1_4; }
	inline Func_2_tB3B29867956DE52CE627DBF0C158133CD7A2F46E ** get_address_of_U3CU3E9__11_1_4() { return &___U3CU3E9__11_1_4; }
	inline void set_U3CU3E9__11_1_4(Func_2_tB3B29867956DE52CE627DBF0C158133CD7A2F46E * value)
	{
		___U3CU3E9__11_1_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CU3E9__11_1_4), (void*)value);
	}
};


// Unity.Entities.TypeManager_<>c
struct  U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2  : public RuntimeObject
{
public:

public:
};

struct U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2_StaticFields
{
public:
	// Unity.Entities.TypeManager_<>c Unity.Entities.TypeManager_<>c::<>9
	U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2 * ___U3CU3E9_0;
	// System.EventHandler Unity.Entities.TypeManager_<>c::<>9__80_0
	EventHandler_t084491E53EC706ACA0A15CA17488C075B4ECA44B * ___U3CU3E9__80_0_1;
	// System.Func`2<System.Reflection.FieldInfo,System.Boolean> Unity.Entities.TypeManager_<>c::<>9__113_0
	Func_2_t0BE5B54DD65017DAF1DC5DEC5A896A2B0550F8AE * ___U3CU3E9__113_0_2;

public:
	inline static int32_t get_offset_of_U3CU3E9_0() { return static_cast<int32_t>(offsetof(U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2_StaticFields, ___U3CU3E9_0)); }
	inline U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2 * get_U3CU3E9_0() const { return ___U3CU3E9_0; }
	inline U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2 ** get_address_of_U3CU3E9_0() { return &___U3CU3E9_0; }
	inline void set_U3CU3E9_0(U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2 * value)
	{
		___U3CU3E9_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CU3E9_0), (void*)value);
	}

	inline static int32_t get_offset_of_U3CU3E9__80_0_1() { return static_cast<int32_t>(offsetof(U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2_StaticFields, ___U3CU3E9__80_0_1)); }
	inline EventHandler_t084491E53EC706ACA0A15CA17488C075B4ECA44B * get_U3CU3E9__80_0_1() const { return ___U3CU3E9__80_0_1; }
	inline EventHandler_t084491E53EC706ACA0A15CA17488C075B4ECA44B ** get_address_of_U3CU3E9__80_0_1() { return &___U3CU3E9__80_0_1; }
	inline void set_U3CU3E9__80_0_1(EventHandler_t084491E53EC706ACA0A15CA17488C075B4ECA44B * value)
	{
		___U3CU3E9__80_0_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CU3E9__80_0_1), (void*)value);
	}

	inline static int32_t get_offset_of_U3CU3E9__113_0_2() { return static_cast<int32_t>(offsetof(U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2_StaticFields, ___U3CU3E9__113_0_2)); }
	inline Func_2_t0BE5B54DD65017DAF1DC5DEC5A896A2B0550F8AE * get_U3CU3E9__113_0_2() const { return ___U3CU3E9__113_0_2; }
	inline Func_2_t0BE5B54DD65017DAF1DC5DEC5A896A2B0550F8AE ** get_address_of_U3CU3E9__113_0_2() { return &___U3CU3E9__113_0_2; }
	inline void set_U3CU3E9__113_0_2(Func_2_t0BE5B54DD65017DAF1DC5DEC5A896A2B0550F8AE * value)
	{
		___U3CU3E9__113_0_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CU3E9__113_0_2), (void*)value);
	}
};


// Unity.Entities.TypeManager_SharedTypeIndex
struct  SharedTypeIndex_t3645D21F618FE92A44043118C07DD6AC0F12C56B  : public RuntimeObject
{
public:

public:
};


// Unity.Entities.TypeManager_TypeManagerKeyContext
struct  TypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218  : public RuntimeObject
{
public:

public:
};


// Unity.Properties.Property`2<Unity.Entities.Entity,System.Int32>
struct  Property_2_tCABF4DCC1CD1B4A9380E57084825CBE0CD5650CF  : public RuntimeObject
{
public:
	// System.Collections.Generic.List`1<System.Attribute> Unity.Properties.Property`2::m_Attributes
	List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * ___m_Attributes_0;

public:
	inline static int32_t get_offset_of_m_Attributes_0() { return static_cast<int32_t>(offsetof(Property_2_tCABF4DCC1CD1B4A9380E57084825CBE0CD5650CF, ___m_Attributes_0)); }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * get_m_Attributes_0() const { return ___m_Attributes_0; }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 ** get_address_of_m_Attributes_0() { return &___m_Attributes_0; }
	inline void set_m_Attributes_0(List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * value)
	{
		___m_Attributes_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Attributes_0), (void*)value);
	}
};


// Unity.Properties.Property`2<Unity.Entities.Hash128,Unity.Mathematics.uint4>
struct  Property_2_tC735BFFF1BC3639C475ACE130B7C17946AADC830  : public RuntimeObject
{
public:
	// System.Collections.Generic.List`1<System.Attribute> Unity.Properties.Property`2::m_Attributes
	List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * ___m_Attributes_0;

public:
	inline static int32_t get_offset_of_m_Attributes_0() { return static_cast<int32_t>(offsetof(Property_2_tC735BFFF1BC3639C475ACE130B7C17946AADC830, ___m_Attributes_0)); }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * get_m_Attributes_0() const { return ___m_Attributes_0; }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 ** get_address_of_m_Attributes_0() { return &___m_Attributes_0; }
	inline void set_m_Attributes_0(List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * value)
	{
		___m_Attributes_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Attributes_0), (void*)value);
	}
};


// Unity.Properties.Property`2<Unity.Entities.PostLoadCommandBuffer,Unity.Entities.EntityCommandBuffer>
struct  Property_2_t40BE748BD8D19926CED3273AD0DAECECE3176A34  : public RuntimeObject
{
public:
	// System.Collections.Generic.List`1<System.Attribute> Unity.Properties.Property`2::m_Attributes
	List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * ___m_Attributes_0;

public:
	inline static int32_t get_offset_of_m_Attributes_0() { return static_cast<int32_t>(offsetof(Property_2_t40BE748BD8D19926CED3273AD0DAECECE3176A34, ___m_Attributes_0)); }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * get_m_Attributes_0() const { return ___m_Attributes_0; }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 ** get_address_of_m_Attributes_0() { return &___m_Attributes_0; }
	inline void set_m_Attributes_0(List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * value)
	{
		___m_Attributes_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Attributes_0), (void*)value);
	}
};


// Unity.Properties.Property`2<Unity.Entities.SceneSection,System.Int32>
struct  Property_2_t429FDAC3FDDA865DCCA0BEA2701D06815301B7A7  : public RuntimeObject
{
public:
	// System.Collections.Generic.List`1<System.Attribute> Unity.Properties.Property`2::m_Attributes
	List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * ___m_Attributes_0;

public:
	inline static int32_t get_offset_of_m_Attributes_0() { return static_cast<int32_t>(offsetof(Property_2_t429FDAC3FDDA865DCCA0BEA2701D06815301B7A7, ___m_Attributes_0)); }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * get_m_Attributes_0() const { return ___m_Attributes_0; }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 ** get_address_of_m_Attributes_0() { return &___m_Attributes_0; }
	inline void set_m_Attributes_0(List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * value)
	{
		___m_Attributes_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Attributes_0), (void*)value);
	}
};


// Unity.Properties.Property`2<Unity.Entities.SceneSection,Unity.Entities.Hash128>
struct  Property_2_t2BF85B4F1B3C1A7B861BCF86F0135D4C29B3E71A  : public RuntimeObject
{
public:
	// System.Collections.Generic.List`1<System.Attribute> Unity.Properties.Property`2::m_Attributes
	List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * ___m_Attributes_0;

public:
	inline static int32_t get_offset_of_m_Attributes_0() { return static_cast<int32_t>(offsetof(Property_2_t2BF85B4F1B3C1A7B861BCF86F0135D4C29B3E71A, ___m_Attributes_0)); }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * get_m_Attributes_0() const { return ___m_Attributes_0; }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 ** get_address_of_m_Attributes_0() { return &___m_Attributes_0; }
	inline void set_m_Attributes_0(List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * value)
	{
		___m_Attributes_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Attributes_0), (void*)value);
	}
};


// Unity.Properties.Property`2<Unity.Entities.SceneTag,Unity.Entities.Entity>
struct  Property_2_tEAB8BD93E42D6310C52481E57E5BE3E4F63917B0  : public RuntimeObject
{
public:
	// System.Collections.Generic.List`1<System.Attribute> Unity.Properties.Property`2::m_Attributes
	List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * ___m_Attributes_0;

public:
	inline static int32_t get_offset_of_m_Attributes_0() { return static_cast<int32_t>(offsetof(Property_2_tEAB8BD93E42D6310C52481E57E5BE3E4F63917B0, ___m_Attributes_0)); }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * get_m_Attributes_0() const { return ___m_Attributes_0; }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 ** get_address_of_m_Attributes_0() { return &___m_Attributes_0; }
	inline void set_m_Attributes_0(List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * value)
	{
		___m_Attributes_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Attributes_0), (void*)value);
	}
};


// Unity.Properties.Property`2<Unity.Entities.SectionMetadataSetup,System.Int32>
struct  Property_2_t300D32A567141F1F72A198697AF2D006E2898B23  : public RuntimeObject
{
public:
	// System.Collections.Generic.List`1<System.Attribute> Unity.Properties.Property`2::m_Attributes
	List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * ___m_Attributes_0;

public:
	inline static int32_t get_offset_of_m_Attributes_0() { return static_cast<int32_t>(offsetof(Property_2_t300D32A567141F1F72A198697AF2D006E2898B23, ___m_Attributes_0)); }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * get_m_Attributes_0() const { return ___m_Attributes_0; }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 ** get_address_of_m_Attributes_0() { return &___m_Attributes_0; }
	inline void set_m_Attributes_0(List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * value)
	{
		___m_Attributes_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Attributes_0), (void*)value);
	}
};


// Unity.Properties.Property`2<Unity.Mathematics.uint4,System.UInt32>
struct  Property_2_tB965C4D1501C79D2FADA36497D75F6F4BEC5DAED  : public RuntimeObject
{
public:
	// System.Collections.Generic.List`1<System.Attribute> Unity.Properties.Property`2::m_Attributes
	List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * ___m_Attributes_0;

public:
	inline static int32_t get_offset_of_m_Attributes_0() { return static_cast<int32_t>(offsetof(Property_2_tB965C4D1501C79D2FADA36497D75F6F4BEC5DAED, ___m_Attributes_0)); }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * get_m_Attributes_0() const { return ___m_Attributes_0; }
	inline List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 ** get_address_of_m_Attributes_0() { return &___m_Attributes_0; }
	inline void set_m_Attributes_0(List_1_tD9744FC9973F836851CCD7BEBF915691AB28EC10 * value)
	{
		___m_Attributes_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Attributes_0), (void*)value);
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


// System.Reflection.CustomAttributeTypedArgument
struct  CustomAttributeTypedArgument_tE7152E8FACDD29A8E0040E151C86F436FA8E6910 
{
public:
	// System.Type System.Reflection.CustomAttributeTypedArgument::argumentType
	Type_t * ___argumentType_0;
	// System.Object System.Reflection.CustomAttributeTypedArgument::value
	RuntimeObject * ___value_1;

public:
	inline static int32_t get_offset_of_argumentType_0() { return static_cast<int32_t>(offsetof(CustomAttributeTypedArgument_tE7152E8FACDD29A8E0040E151C86F436FA8E6910, ___argumentType_0)); }
	inline Type_t * get_argumentType_0() const { return ___argumentType_0; }
	inline Type_t ** get_address_of_argumentType_0() { return &___argumentType_0; }
	inline void set_argumentType_0(Type_t * value)
	{
		___argumentType_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___argumentType_0), (void*)value);
	}

	inline static int32_t get_offset_of_value_1() { return static_cast<int32_t>(offsetof(CustomAttributeTypedArgument_tE7152E8FACDD29A8E0040E151C86F436FA8E6910, ___value_1)); }
	inline RuntimeObject * get_value_1() const { return ___value_1; }
	inline RuntimeObject ** get_address_of_value_1() { return &___value_1; }
	inline void set_value_1(RuntimeObject * value)
	{
		___value_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___value_1), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.Reflection.CustomAttributeTypedArgument
struct CustomAttributeTypedArgument_tE7152E8FACDD29A8E0040E151C86F436FA8E6910_marshaled_pinvoke
{
	Type_t * ___argumentType_0;
	Il2CppIUnknown* ___value_1;
};
// Native definition for COM marshalling of System.Reflection.CustomAttributeTypedArgument
struct CustomAttributeTypedArgument_tE7152E8FACDD29A8E0040E151C86F436FA8E6910_marshaled_com
{
	Type_t * ___argumentType_0;
	Il2CppIUnknown* ___value_1;
};

// System.Reflection.FieldInfo
struct  FieldInfo_t  : public MemberInfo_t
{
public:

public:
};


// System.Reflection.MethodBase
struct  MethodBase_t  : public MemberInfo_t
{
public:

public:
};


// System.Runtime.InteropServices.GCHandle
struct  GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 
{
public:
	// System.Int32 System.Runtime.InteropServices.GCHandle::handle
	int32_t ___handle_0;

public:
	inline static int32_t get_offset_of_handle_0() { return static_cast<int32_t>(offsetof(GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603, ___handle_0)); }
	inline int32_t get_handle_0() const { return ___handle_0; }
	inline int32_t* get_address_of_handle_0() { return &___handle_0; }
	inline void set_handle_0(int32_t value)
	{
		___handle_0 = value;
	}
};


// System.UInt16
struct  UInt16_t894EA9D4FB7C799B244E7BBF2DF0EEEDBC77A8BD 
{
public:
	// System.UInt16 System.UInt16::m_value
	uint16_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(UInt16_t894EA9D4FB7C799B244E7BBF2DF0EEEDBC77A8BD, ___m_value_0)); }
	inline uint16_t get_m_value_0() const { return ___m_value_0; }
	inline uint16_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint16_t value)
	{
		___m_value_0 = value;
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


// Unity.Burst.SharedStatic`1<System.Int32>
struct  SharedStatic_1_t6DAF6BFFE7CB6295851C9E4567A9874B214BD088 
{
public:
	// System.Void* Unity.Burst.SharedStatic`1::_buffer
	void* ____buffer_0;

public:
	inline static int32_t get_offset_of__buffer_0() { return static_cast<int32_t>(offsetof(SharedStatic_1_t6DAF6BFFE7CB6295851C9E4567A9874B214BD088, ____buffer_0)); }
	inline void* get__buffer_0() const { return ____buffer_0; }
	inline void** get_address_of__buffer_0() { return &____buffer_0; }
	inline void set__buffer_0(void* value)
	{
		____buffer_0 = value;
	}
};


// Unity.Burst.SharedStatic`1<System.IntPtr>
struct  SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6 
{
public:
	// System.Void* Unity.Burst.SharedStatic`1::_buffer
	void* ____buffer_0;

public:
	inline static int32_t get_offset_of__buffer_0() { return static_cast<int32_t>(offsetof(SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6, ____buffer_0)); }
	inline void* get__buffer_0() const { return ____buffer_0; }
	inline void** get_address_of__buffer_0() { return &____buffer_0; }
	inline void set__buffer_0(void* value)
	{
		____buffer_0 = value;
	}
};


// Unity.Burst.SharedStatic`1<System.UInt64>
struct  SharedStatic_1_t3BF4833AD74181586FD1838757D84AD9921C1258 
{
public:
	// System.Void* Unity.Burst.SharedStatic`1::_buffer
	void* ____buffer_0;

public:
	inline static int32_t get_offset_of__buffer_0() { return static_cast<int32_t>(offsetof(SharedStatic_1_t3BF4833AD74181586FD1838757D84AD9921C1258, ____buffer_0)); }
	inline void* get__buffer_0() const { return ____buffer_0; }
	inline void** get_address_of__buffer_0() { return &____buffer_0; }
	inline void set__buffer_0(void* value)
	{
		____buffer_0 = value;
	}
};


// Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore_ChunkStore>
struct  SharedStatic_1_t8A0B77C3063A5BD031494FB6E0ACE2DC30197C8F 
{
public:
	// System.Void* Unity.Burst.SharedStatic`1::_buffer
	void* ____buffer_0;

public:
	inline static int32_t get_offset_of__buffer_0() { return static_cast<int32_t>(offsetof(SharedStatic_1_t8A0B77C3063A5BD031494FB6E0ACE2DC30197C8F, ____buffer_0)); }
	inline void* get__buffer_0() const { return ____buffer_0; }
	inline void** get_address_of__buffer_0() { return &____buffer_0; }
	inline void set__buffer_0(void* value)
	{
		____buffer_0 = value;
	}
};


// Unity.Collections.AllocatorManager_AllocatorHandle
struct  AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A 
{
public:
	// System.Int32 Unity.Collections.AllocatorManager_AllocatorHandle::Value
	int32_t ___Value_0;

public:
	inline static int32_t get_offset_of_Value_0() { return static_cast<int32_t>(offsetof(AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A, ___Value_0)); }
	inline int32_t get_Value_0() const { return ___Value_0; }
	inline int32_t* get_address_of_Value_0() { return &___Value_0; }
	inline void set_Value_0(int32_t value)
	{
		___Value_0 = value;
	}
};


// Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer_Reader
struct  Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF 
{
public:
	// System.Byte* Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer_Reader::Ptr
	uint8_t* ___Ptr_0;
	// System.Int32 Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer_Reader::Size
	int32_t ___Size_1;
	// System.Int32 Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer_Reader::Offset
	int32_t ___Offset_2;

public:
	inline static int32_t get_offset_of_Ptr_0() { return static_cast<int32_t>(offsetof(Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF, ___Ptr_0)); }
	inline uint8_t* get_Ptr_0() const { return ___Ptr_0; }
	inline uint8_t** get_address_of_Ptr_0() { return &___Ptr_0; }
	inline void set_Ptr_0(uint8_t* value)
	{
		___Ptr_0 = value;
	}

	inline static int32_t get_offset_of_Size_1() { return static_cast<int32_t>(offsetof(Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF, ___Size_1)); }
	inline int32_t get_Size_1() const { return ___Size_1; }
	inline int32_t* get_address_of_Size_1() { return &___Size_1; }
	inline void set_Size_1(int32_t value)
	{
		___Size_1 = value;
	}

	inline static int32_t get_offset_of_Offset_2() { return static_cast<int32_t>(offsetof(Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF, ___Offset_2)); }
	inline int32_t get_Offset_2() const { return ___Offset_2; }
	inline int32_t* get_address_of_Offset_2() { return &___Offset_2; }
	inline void set_Offset_2(int32_t value)
	{
		___Offset_2 = value;
	}
};


// Unity.Core.TimeData
struct  TimeData_t1892793CB71746290FBEED4D53C605AF3A3FA91F 
{
public:
	// System.Double Unity.Core.TimeData::ElapsedTime
	double ___ElapsedTime_0;
	// System.Single Unity.Core.TimeData::DeltaTime
	float ___DeltaTime_1;

public:
	inline static int32_t get_offset_of_ElapsedTime_0() { return static_cast<int32_t>(offsetof(TimeData_t1892793CB71746290FBEED4D53C605AF3A3FA91F, ___ElapsedTime_0)); }
	inline double get_ElapsedTime_0() const { return ___ElapsedTime_0; }
	inline double* get_address_of_ElapsedTime_0() { return &___ElapsedTime_0; }
	inline void set_ElapsedTime_0(double value)
	{
		___ElapsedTime_0 = value;
	}

	inline static int32_t get_offset_of_DeltaTime_1() { return static_cast<int32_t>(offsetof(TimeData_t1892793CB71746290FBEED4D53C605AF3A3FA91F, ___DeltaTime_1)); }
	inline float get_DeltaTime_1() const { return ___DeltaTime_1; }
	inline float* get_address_of_DeltaTime_1() { return &___DeltaTime_1; }
	inline void set_DeltaTime_1(float value)
	{
		___DeltaTime_1 = value;
	}
};


// Unity.Entities.ArchetypeChunk
struct  ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D 
{
public:
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					// Unity.Entities.Chunk* Unity.Entities.ArchetypeChunk::m_Chunk
					Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 * ___m_Chunk_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 * ___m_Chunk_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___m_EntityComponentStore_1_OffsetPadding[8];
					// Unity.Entities.EntityComponentStore* Unity.Entities.ArchetypeChunk::m_EntityComponentStore
					EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___m_EntityComponentStore_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___m_EntityComponentStore_1_OffsetPadding_forAlignmentOnly[8];
					EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___m_EntityComponentStore_1_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___m_BatchStartEntityIndex_2_OffsetPadding[16];
					// System.Int32 Unity.Entities.ArchetypeChunk::m_BatchStartEntityIndex
					int32_t ___m_BatchStartEntityIndex_2;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___m_BatchStartEntityIndex_2_OffsetPadding_forAlignmentOnly[16];
					int32_t ___m_BatchStartEntityIndex_2_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___m_BatchEntityCount_3_OffsetPadding[20];
					// System.Int32 Unity.Entities.ArchetypeChunk::m_BatchEntityCount
					int32_t ___m_BatchEntityCount_3;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___m_BatchEntityCount_3_OffsetPadding_forAlignmentOnly[20];
					int32_t ___m_BatchEntityCount_3_forAlignmentOnly;
				};
			};
		};
		uint8_t ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D__padding[32];
	};

public:
	inline static int32_t get_offset_of_m_Chunk_0() { return static_cast<int32_t>(offsetof(ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D, ___m_Chunk_0)); }
	inline Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 * get_m_Chunk_0() const { return ___m_Chunk_0; }
	inline Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 ** get_address_of_m_Chunk_0() { return &___m_Chunk_0; }
	inline void set_m_Chunk_0(Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 * value)
	{
		___m_Chunk_0 = value;
	}

	inline static int32_t get_offset_of_m_EntityComponentStore_1() { return static_cast<int32_t>(offsetof(ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D, ___m_EntityComponentStore_1)); }
	inline EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * get_m_EntityComponentStore_1() const { return ___m_EntityComponentStore_1; }
	inline EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA ** get_address_of_m_EntityComponentStore_1() { return &___m_EntityComponentStore_1; }
	inline void set_m_EntityComponentStore_1(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * value)
	{
		___m_EntityComponentStore_1 = value;
	}

	inline static int32_t get_offset_of_m_BatchStartEntityIndex_2() { return static_cast<int32_t>(offsetof(ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D, ___m_BatchStartEntityIndex_2)); }
	inline int32_t get_m_BatchStartEntityIndex_2() const { return ___m_BatchStartEntityIndex_2; }
	inline int32_t* get_address_of_m_BatchStartEntityIndex_2() { return &___m_BatchStartEntityIndex_2; }
	inline void set_m_BatchStartEntityIndex_2(int32_t value)
	{
		___m_BatchStartEntityIndex_2 = value;
	}

	inline static int32_t get_offset_of_m_BatchEntityCount_3() { return static_cast<int32_t>(offsetof(ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D, ___m_BatchEntityCount_3)); }
	inline int32_t get_m_BatchEntityCount_3() const { return ___m_BatchEntityCount_3; }
	inline int32_t* get_address_of_m_BatchEntityCount_3() { return &___m_BatchEntityCount_3; }
	inline void set_m_BatchEntityCount_3(int32_t value)
	{
		___m_BatchEntityCount_3 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.ArchetypeChunk
struct ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D_marshaled_pinvoke
{
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 * ___m_Chunk_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 * ___m_Chunk_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___m_EntityComponentStore_1_OffsetPadding[8];
					EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___m_EntityComponentStore_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___m_EntityComponentStore_1_OffsetPadding_forAlignmentOnly[8];
					EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___m_EntityComponentStore_1_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___m_BatchStartEntityIndex_2_OffsetPadding[16];
					int32_t ___m_BatchStartEntityIndex_2;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___m_BatchStartEntityIndex_2_OffsetPadding_forAlignmentOnly[16];
					int32_t ___m_BatchStartEntityIndex_2_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___m_BatchEntityCount_3_OffsetPadding[20];
					int32_t ___m_BatchEntityCount_3;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___m_BatchEntityCount_3_OffsetPadding_forAlignmentOnly[20];
					int32_t ___m_BatchEntityCount_3_forAlignmentOnly;
				};
			};
		};
		uint8_t ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D__padding[32];
	};
};
// Native definition for COM marshalling of Unity.Entities.ArchetypeChunk
struct ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D_marshaled_com
{
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 * ___m_Chunk_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 * ___m_Chunk_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___m_EntityComponentStore_1_OffsetPadding[8];
					EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___m_EntityComponentStore_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___m_EntityComponentStore_1_OffsetPadding_forAlignmentOnly[8];
					EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___m_EntityComponentStore_1_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___m_BatchStartEntityIndex_2_OffsetPadding[16];
					int32_t ___m_BatchStartEntityIndex_2;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___m_BatchStartEntityIndex_2_OffsetPadding_forAlignmentOnly[16];
					int32_t ___m_BatchStartEntityIndex_2_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___m_BatchEntityCount_3_OffsetPadding[20];
					int32_t ___m_BatchEntityCount_3;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___m_BatchEntityCount_3_OffsetPadding_forAlignmentOnly[20];
					int32_t ___m_BatchEntityCount_3_forAlignmentOnly;
				};
			};
		};
		uint8_t ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D__padding[32];
	};
};

// Unity.Entities.ArchetypeChunkComponentType`1<Unity.Entities.RetainBlobAssetBatchPtr>
struct  ArchetypeChunkComponentType_1_tFD5C7F1417F5147765D69E09498A481C21F64CA5 
{
public:
	// System.Int32 Unity.Entities.ArchetypeChunkComponentType`1::m_TypeIndex
	int32_t ___m_TypeIndex_0;
	// System.UInt32 Unity.Entities.ArchetypeChunkComponentType`1::m_GlobalSystemVersion
	uint32_t ___m_GlobalSystemVersion_1;
	// System.Boolean Unity.Entities.ArchetypeChunkComponentType`1::m_IsReadOnly
	bool ___m_IsReadOnly_2;
	// System.Boolean Unity.Entities.ArchetypeChunkComponentType`1::m_IsZeroSized
	bool ___m_IsZeroSized_3;
	// System.Int32 Unity.Entities.ArchetypeChunkComponentType`1::m_Length
	int32_t ___m_Length_4;

public:
	inline static int32_t get_offset_of_m_TypeIndex_0() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_tFD5C7F1417F5147765D69E09498A481C21F64CA5, ___m_TypeIndex_0)); }
	inline int32_t get_m_TypeIndex_0() const { return ___m_TypeIndex_0; }
	inline int32_t* get_address_of_m_TypeIndex_0() { return &___m_TypeIndex_0; }
	inline void set_m_TypeIndex_0(int32_t value)
	{
		___m_TypeIndex_0 = value;
	}

	inline static int32_t get_offset_of_m_GlobalSystemVersion_1() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_tFD5C7F1417F5147765D69E09498A481C21F64CA5, ___m_GlobalSystemVersion_1)); }
	inline uint32_t get_m_GlobalSystemVersion_1() const { return ___m_GlobalSystemVersion_1; }
	inline uint32_t* get_address_of_m_GlobalSystemVersion_1() { return &___m_GlobalSystemVersion_1; }
	inline void set_m_GlobalSystemVersion_1(uint32_t value)
	{
		___m_GlobalSystemVersion_1 = value;
	}

	inline static int32_t get_offset_of_m_IsReadOnly_2() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_tFD5C7F1417F5147765D69E09498A481C21F64CA5, ___m_IsReadOnly_2)); }
	inline bool get_m_IsReadOnly_2() const { return ___m_IsReadOnly_2; }
	inline bool* get_address_of_m_IsReadOnly_2() { return &___m_IsReadOnly_2; }
	inline void set_m_IsReadOnly_2(bool value)
	{
		___m_IsReadOnly_2 = value;
	}

	inline static int32_t get_offset_of_m_IsZeroSized_3() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_tFD5C7F1417F5147765D69E09498A481C21F64CA5, ___m_IsZeroSized_3)); }
	inline bool get_m_IsZeroSized_3() const { return ___m_IsZeroSized_3; }
	inline bool* get_address_of_m_IsZeroSized_3() { return &___m_IsZeroSized_3; }
	inline void set_m_IsZeroSized_3(bool value)
	{
		___m_IsZeroSized_3 = value;
	}

	inline static int32_t get_offset_of_m_Length_4() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_tFD5C7F1417F5147765D69E09498A481C21F64CA5, ___m_Length_4)); }
	inline int32_t get_m_Length_4() const { return ___m_Length_4; }
	inline int32_t* get_address_of_m_Length_4() { return &___m_Length_4; }
	inline void set_m_Length_4(int32_t value)
	{
		___m_Length_4 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.ArchetypeChunkComponentType`1
#ifndef ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_pinvoke_define
#define ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_pinvoke_define
struct ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_pinvoke
{
	int32_t ___m_TypeIndex_0;
	uint32_t ___m_GlobalSystemVersion_1;
	int32_t ___m_IsReadOnly_2;
	int32_t ___m_IsZeroSized_3;
	int32_t ___m_Length_4;
};
#endif
// Native definition for COM marshalling of Unity.Entities.ArchetypeChunkComponentType`1
#ifndef ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_com_define
#define ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_com_define
struct ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_com
{
	int32_t ___m_TypeIndex_0;
	uint32_t ___m_GlobalSystemVersion_1;
	int32_t ___m_IsReadOnly_2;
	int32_t ___m_IsZeroSized_3;
	int32_t ___m_Length_4;
};
#endif

// Unity.Entities.ArchetypeChunkComponentType`1<Unity.Entities.RetainBlobAssetPtr>
struct  ArchetypeChunkComponentType_1_tB290CDB5AD796AE285F864C9808F9D98EA41BB84 
{
public:
	// System.Int32 Unity.Entities.ArchetypeChunkComponentType`1::m_TypeIndex
	int32_t ___m_TypeIndex_0;
	// System.UInt32 Unity.Entities.ArchetypeChunkComponentType`1::m_GlobalSystemVersion
	uint32_t ___m_GlobalSystemVersion_1;
	// System.Boolean Unity.Entities.ArchetypeChunkComponentType`1::m_IsReadOnly
	bool ___m_IsReadOnly_2;
	// System.Boolean Unity.Entities.ArchetypeChunkComponentType`1::m_IsZeroSized
	bool ___m_IsZeroSized_3;
	// System.Int32 Unity.Entities.ArchetypeChunkComponentType`1::m_Length
	int32_t ___m_Length_4;

public:
	inline static int32_t get_offset_of_m_TypeIndex_0() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_tB290CDB5AD796AE285F864C9808F9D98EA41BB84, ___m_TypeIndex_0)); }
	inline int32_t get_m_TypeIndex_0() const { return ___m_TypeIndex_0; }
	inline int32_t* get_address_of_m_TypeIndex_0() { return &___m_TypeIndex_0; }
	inline void set_m_TypeIndex_0(int32_t value)
	{
		___m_TypeIndex_0 = value;
	}

	inline static int32_t get_offset_of_m_GlobalSystemVersion_1() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_tB290CDB5AD796AE285F864C9808F9D98EA41BB84, ___m_GlobalSystemVersion_1)); }
	inline uint32_t get_m_GlobalSystemVersion_1() const { return ___m_GlobalSystemVersion_1; }
	inline uint32_t* get_address_of_m_GlobalSystemVersion_1() { return &___m_GlobalSystemVersion_1; }
	inline void set_m_GlobalSystemVersion_1(uint32_t value)
	{
		___m_GlobalSystemVersion_1 = value;
	}

	inline static int32_t get_offset_of_m_IsReadOnly_2() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_tB290CDB5AD796AE285F864C9808F9D98EA41BB84, ___m_IsReadOnly_2)); }
	inline bool get_m_IsReadOnly_2() const { return ___m_IsReadOnly_2; }
	inline bool* get_address_of_m_IsReadOnly_2() { return &___m_IsReadOnly_2; }
	inline void set_m_IsReadOnly_2(bool value)
	{
		___m_IsReadOnly_2 = value;
	}

	inline static int32_t get_offset_of_m_IsZeroSized_3() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_tB290CDB5AD796AE285F864C9808F9D98EA41BB84, ___m_IsZeroSized_3)); }
	inline bool get_m_IsZeroSized_3() const { return ___m_IsZeroSized_3; }
	inline bool* get_address_of_m_IsZeroSized_3() { return &___m_IsZeroSized_3; }
	inline void set_m_IsZeroSized_3(bool value)
	{
		___m_IsZeroSized_3 = value;
	}

	inline static int32_t get_offset_of_m_Length_4() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_tB290CDB5AD796AE285F864C9808F9D98EA41BB84, ___m_Length_4)); }
	inline int32_t get_m_Length_4() const { return ___m_Length_4; }
	inline int32_t* get_address_of_m_Length_4() { return &___m_Length_4; }
	inline void set_m_Length_4(int32_t value)
	{
		___m_Length_4 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.ArchetypeChunkComponentType`1
#ifndef ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_pinvoke_define
#define ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_pinvoke_define
struct ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_pinvoke
{
	int32_t ___m_TypeIndex_0;
	uint32_t ___m_GlobalSystemVersion_1;
	int32_t ___m_IsReadOnly_2;
	int32_t ___m_IsZeroSized_3;
	int32_t ___m_Length_4;
};
#endif
// Native definition for COM marshalling of Unity.Entities.ArchetypeChunkComponentType`1
#ifndef ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_com_define
#define ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_com_define
struct ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_com
{
	int32_t ___m_TypeIndex_0;
	uint32_t ___m_GlobalSystemVersion_1;
	int32_t ___m_IsReadOnly_2;
	int32_t ___m_IsZeroSized_3;
	int32_t ___m_Length_4;
};
#endif

// Unity.Entities.ArchetypeChunkComponentType`1<Unity.Entities.RetainBlobAssets>
struct  ArchetypeChunkComponentType_1_t09298C5D6AADE312DF38D90B33DFF235D5F27659 
{
public:
	// System.Int32 Unity.Entities.ArchetypeChunkComponentType`1::m_TypeIndex
	int32_t ___m_TypeIndex_0;
	// System.UInt32 Unity.Entities.ArchetypeChunkComponentType`1::m_GlobalSystemVersion
	uint32_t ___m_GlobalSystemVersion_1;
	// System.Boolean Unity.Entities.ArchetypeChunkComponentType`1::m_IsReadOnly
	bool ___m_IsReadOnly_2;
	// System.Boolean Unity.Entities.ArchetypeChunkComponentType`1::m_IsZeroSized
	bool ___m_IsZeroSized_3;
	// System.Int32 Unity.Entities.ArchetypeChunkComponentType`1::m_Length
	int32_t ___m_Length_4;

public:
	inline static int32_t get_offset_of_m_TypeIndex_0() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_t09298C5D6AADE312DF38D90B33DFF235D5F27659, ___m_TypeIndex_0)); }
	inline int32_t get_m_TypeIndex_0() const { return ___m_TypeIndex_0; }
	inline int32_t* get_address_of_m_TypeIndex_0() { return &___m_TypeIndex_0; }
	inline void set_m_TypeIndex_0(int32_t value)
	{
		___m_TypeIndex_0 = value;
	}

	inline static int32_t get_offset_of_m_GlobalSystemVersion_1() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_t09298C5D6AADE312DF38D90B33DFF235D5F27659, ___m_GlobalSystemVersion_1)); }
	inline uint32_t get_m_GlobalSystemVersion_1() const { return ___m_GlobalSystemVersion_1; }
	inline uint32_t* get_address_of_m_GlobalSystemVersion_1() { return &___m_GlobalSystemVersion_1; }
	inline void set_m_GlobalSystemVersion_1(uint32_t value)
	{
		___m_GlobalSystemVersion_1 = value;
	}

	inline static int32_t get_offset_of_m_IsReadOnly_2() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_t09298C5D6AADE312DF38D90B33DFF235D5F27659, ___m_IsReadOnly_2)); }
	inline bool get_m_IsReadOnly_2() const { return ___m_IsReadOnly_2; }
	inline bool* get_address_of_m_IsReadOnly_2() { return &___m_IsReadOnly_2; }
	inline void set_m_IsReadOnly_2(bool value)
	{
		___m_IsReadOnly_2 = value;
	}

	inline static int32_t get_offset_of_m_IsZeroSized_3() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_t09298C5D6AADE312DF38D90B33DFF235D5F27659, ___m_IsZeroSized_3)); }
	inline bool get_m_IsZeroSized_3() const { return ___m_IsZeroSized_3; }
	inline bool* get_address_of_m_IsZeroSized_3() { return &___m_IsZeroSized_3; }
	inline void set_m_IsZeroSized_3(bool value)
	{
		___m_IsZeroSized_3 = value;
	}

	inline static int32_t get_offset_of_m_Length_4() { return static_cast<int32_t>(offsetof(ArchetypeChunkComponentType_1_t09298C5D6AADE312DF38D90B33DFF235D5F27659, ___m_Length_4)); }
	inline int32_t get_m_Length_4() const { return ___m_Length_4; }
	inline int32_t* get_address_of_m_Length_4() { return &___m_Length_4; }
	inline void set_m_Length_4(int32_t value)
	{
		___m_Length_4 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.ArchetypeChunkComponentType`1
#ifndef ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_pinvoke_define
#define ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_pinvoke_define
struct ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_pinvoke
{
	int32_t ___m_TypeIndex_0;
	uint32_t ___m_GlobalSystemVersion_1;
	int32_t ___m_IsReadOnly_2;
	int32_t ___m_IsZeroSized_3;
	int32_t ___m_Length_4;
};
#endif
// Native definition for COM marshalling of Unity.Entities.ArchetypeChunkComponentType`1
#ifndef ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_com_define
#define ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_com_define
struct ArchetypeChunkComponentType_1_t30B92C6C4AA57C416BD9CCB9036E351CFF3C7419_marshaled_com
{
	int32_t ___m_TypeIndex_0;
	uint32_t ___m_GlobalSystemVersion_1;
	int32_t ___m_IsReadOnly_2;
	int32_t ___m_IsZeroSized_3;
	int32_t ___m_Length_4;
};
#endif

// Unity.Entities.ArchetypeChunkEntityType
struct  ArchetypeChunkEntityType_t9283D156272DAE65135E83A92ABA7778CE43B640 
{
public:
	// System.Int32 Unity.Entities.ArchetypeChunkEntityType::m_Length
	int32_t ___m_Length_0;

public:
	inline static int32_t get_offset_of_m_Length_0() { return static_cast<int32_t>(offsetof(ArchetypeChunkEntityType_t9283D156272DAE65135E83A92ABA7778CE43B640, ___m_Length_0)); }
	inline int32_t get_m_Length_0() const { return ___m_Length_0; }
	inline int32_t* get_address_of_m_Length_0() { return &___m_Length_0; }
	inline void set_m_Length_0(int32_t value)
	{
		___m_Length_0 = value;
	}
};


// Unity.Entities.ArchetypeChunkSharedComponentType`1<Unity.Entities.BlobAssetOwner>
struct  ArchetypeChunkSharedComponentType_1_t848742A20BE275A0E107A9BC6C465BA4E524BF4E 
{
public:
	// System.Int32 Unity.Entities.ArchetypeChunkSharedComponentType`1::m_TypeIndex
	int32_t ___m_TypeIndex_0;
	// System.Int32 Unity.Entities.ArchetypeChunkSharedComponentType`1::m_Length
	int32_t ___m_Length_1;

public:
	inline static int32_t get_offset_of_m_TypeIndex_0() { return static_cast<int32_t>(offsetof(ArchetypeChunkSharedComponentType_1_t848742A20BE275A0E107A9BC6C465BA4E524BF4E, ___m_TypeIndex_0)); }
	inline int32_t get_m_TypeIndex_0() const { return ___m_TypeIndex_0; }
	inline int32_t* get_address_of_m_TypeIndex_0() { return &___m_TypeIndex_0; }
	inline void set_m_TypeIndex_0(int32_t value)
	{
		___m_TypeIndex_0 = value;
	}

	inline static int32_t get_offset_of_m_Length_1() { return static_cast<int32_t>(offsetof(ArchetypeChunkSharedComponentType_1_t848742A20BE275A0E107A9BC6C465BA4E524BF4E, ___m_Length_1)); }
	inline int32_t get_m_Length_1() const { return ___m_Length_1; }
	inline int32_t* get_address_of_m_Length_1() { return &___m_Length_1; }
	inline void set_m_Length_1(int32_t value)
	{
		___m_Length_1 = value;
	}
};


// Unity.Entities.BlobAssetReferenceData
struct  BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1 
{
public:
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					// System.Byte* Unity.Entities.BlobAssetReferenceData::m_Ptr
					uint8_t* ___m_Ptr_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					uint8_t* ___m_Ptr_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					// System.Int64 Unity.Entities.BlobAssetReferenceData::m_Align8Union
					int64_t ___m_Align8Union_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					int64_t ___m_Align8Union_1_forAlignmentOnly;
				};
			};
		};
		uint8_t BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1__padding[8];
	};

public:
	inline static int32_t get_offset_of_m_Ptr_0() { return static_cast<int32_t>(offsetof(BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1, ___m_Ptr_0)); }
	inline uint8_t* get_m_Ptr_0() const { return ___m_Ptr_0; }
	inline uint8_t** get_address_of_m_Ptr_0() { return &___m_Ptr_0; }
	inline void set_m_Ptr_0(uint8_t* value)
	{
		___m_Ptr_0 = value;
	}

	inline static int32_t get_offset_of_m_Align8Union_1() { return static_cast<int32_t>(offsetof(BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1, ___m_Align8Union_1)); }
	inline int64_t get_m_Align8Union_1() const { return ___m_Align8Union_1; }
	inline int64_t* get_address_of_m_Align8Union_1() { return &___m_Align8Union_1; }
	inline void set_m_Align8Union_1(int64_t value)
	{
		___m_Align8Union_1 = value;
	}
};


// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_Entity_StructuralChangeRuntime
struct  StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA 
{
public:
	union
	{
		struct
		{
		};
		uint8_t StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA__padding[1];
	};

public:
};


// Unity.Entities.Entity
struct  Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 
{
public:
	// System.Int32 Unity.Entities.Entity::Index
	int32_t ___Index_0;
	// System.Int32 Unity.Entities.Entity::Version
	int32_t ___Version_1;

public:
	inline static int32_t get_offset_of_Index_0() { return static_cast<int32_t>(offsetof(Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4, ___Index_0)); }
	inline int32_t get_Index_0() const { return ___Index_0; }
	inline int32_t* get_address_of_Index_0() { return &___Index_0; }
	inline void set_Index_0(int32_t value)
	{
		___Index_0 = value;
	}

	inline static int32_t get_offset_of_Version_1() { return static_cast<int32_t>(offsetof(Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4, ___Version_1)); }
	inline int32_t get_Version_1() const { return ___Version_1; }
	inline int32_t* get_address_of_Version_1() { return &___Version_1; }
	inline void set_Version_1(int32_t value)
	{
		___Version_1 = value;
	}
};


// Unity.Entities.EntityCommandBuffer
struct  EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764 
{
public:
	// Unity.Entities.EntityCommandBufferData* Unity.Entities.EntityCommandBuffer::m_Data
	EntityCommandBufferData_t54C8C95E18B79C35853D6A104DCD8ECC9F66CBAA * ___m_Data_0;

public:
	inline static int32_t get_offset_of_m_Data_0() { return static_cast<int32_t>(offsetof(EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764, ___m_Data_0)); }
	inline EntityCommandBufferData_t54C8C95E18B79C35853D6A104DCD8ECC9F66CBAA * get_m_Data_0() const { return ___m_Data_0; }
	inline EntityCommandBufferData_t54C8C95E18B79C35853D6A104DCD8ECC9F66CBAA ** get_address_of_m_Data_0() { return &___m_Data_0; }
	inline void set_m_Data_0(EntityCommandBufferData_t54C8C95E18B79C35853D6A104DCD8ECC9F66CBAA * value)
	{
		___m_Data_0 = value;
	}
};

struct EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764_StaticFields
{
public:
	// Unity.Entities.EntityCommandBuffer_PlaybackChainChunkDelegate Unity.Entities.EntityCommandBuffer::PlaybackChainChunk
	PlaybackChainChunkDelegate_tD9D47AE24B96575D178F50ABEFCB079BBCA115E9 * ___PlaybackChainChunk_1;
	// Unity.Entities.EntityCommandBuffer_PlaybackUnmanagedCommandDelegate Unity.Entities.EntityCommandBuffer::PlaybackUnmanagedCommand
	PlaybackUnmanagedCommandDelegate_t2AB4784F8A033A2DEB0C88C9FA7A80D923312767 * ___PlaybackUnmanagedCommand_2;

public:
	inline static int32_t get_offset_of_PlaybackChainChunk_1() { return static_cast<int32_t>(offsetof(EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764_StaticFields, ___PlaybackChainChunk_1)); }
	inline PlaybackChainChunkDelegate_tD9D47AE24B96575D178F50ABEFCB079BBCA115E9 * get_PlaybackChainChunk_1() const { return ___PlaybackChainChunk_1; }
	inline PlaybackChainChunkDelegate_tD9D47AE24B96575D178F50ABEFCB079BBCA115E9 ** get_address_of_PlaybackChainChunk_1() { return &___PlaybackChainChunk_1; }
	inline void set_PlaybackChainChunk_1(PlaybackChainChunkDelegate_tD9D47AE24B96575D178F50ABEFCB079BBCA115E9 * value)
	{
		___PlaybackChainChunk_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___PlaybackChainChunk_1), (void*)value);
	}

	inline static int32_t get_offset_of_PlaybackUnmanagedCommand_2() { return static_cast<int32_t>(offsetof(EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764_StaticFields, ___PlaybackUnmanagedCommand_2)); }
	inline PlaybackUnmanagedCommandDelegate_t2AB4784F8A033A2DEB0C88C9FA7A80D923312767 * get_PlaybackUnmanagedCommand_2() const { return ___PlaybackUnmanagedCommand_2; }
	inline PlaybackUnmanagedCommandDelegate_t2AB4784F8A033A2DEB0C88C9FA7A80D923312767 ** get_address_of_PlaybackUnmanagedCommand_2() { return &___PlaybackUnmanagedCommand_2; }
	inline void set_PlaybackUnmanagedCommand_2(PlaybackUnmanagedCommandDelegate_t2AB4784F8A033A2DEB0C88C9FA7A80D923312767 * value)
	{
		___PlaybackUnmanagedCommand_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___PlaybackUnmanagedCommand_2), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.EntityCommandBuffer
struct EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764_marshaled_pinvoke
{
	EntityCommandBufferData_t54C8C95E18B79C35853D6A104DCD8ECC9F66CBAA * ___m_Data_0;
};
// Native definition for COM marshalling of Unity.Entities.EntityCommandBuffer
struct EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764_marshaled_com
{
	EntityCommandBufferData_t54C8C95E18B79C35853D6A104DCD8ECC9F66CBAA * ___m_Data_0;
};

// Unity.Entities.EntityComponentStore_ArchetypeChunkFilter_<SharedComponentValues>e__FixedBuffer
struct  U3CSharedComponentValuesU3Ee__FixedBuffer_tFD606D260E17791302344C58DAA266485C55A154 
{
public:
	union
	{
		struct
		{
			// System.Int32 Unity.Entities.EntityComponentStore_ArchetypeChunkFilter_<SharedComponentValues>e__FixedBuffer::FixedElementField
			int32_t ___FixedElementField_0;
		};
		uint8_t U3CSharedComponentValuesU3Ee__FixedBuffer_tFD606D260E17791302344C58DAA266485C55A154__padding[32];
	};

public:
	inline static int32_t get_offset_of_FixedElementField_0() { return static_cast<int32_t>(offsetof(U3CSharedComponentValuesU3Ee__FixedBuffer_tFD606D260E17791302344C58DAA266485C55A154, ___FixedElementField_0)); }
	inline int32_t get_FixedElementField_0() const { return ___FixedElementField_0; }
	inline int32_t* get_address_of_FixedElementField_0() { return &___FixedElementField_0; }
	inline void set_FixedElementField_0(int32_t value)
	{
		___FixedElementField_0 = value;
	}
};


// Unity.Entities.EntityManager_DeprecatedRegistry_Cell
struct  Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75 
{
public:
	// Unity.Entities.World Unity.Entities.EntityManager_DeprecatedRegistry_Cell::World
	World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07 * ___World_0;
	// System.UInt32 Unity.Entities.EntityManager_DeprecatedRegistry_Cell::WorldId
	uint32_t ___WorldId_1;

public:
	inline static int32_t get_offset_of_World_0() { return static_cast<int32_t>(offsetof(Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75, ___World_0)); }
	inline World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07 * get_World_0() const { return ___World_0; }
	inline World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07 ** get_address_of_World_0() { return &___World_0; }
	inline void set_World_0(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07 * value)
	{
		___World_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___World_0), (void*)value);
	}

	inline static int32_t get_offset_of_WorldId_1() { return static_cast<int32_t>(offsetof(Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75, ___WorldId_1)); }
	inline uint32_t get_WorldId_1() const { return ___WorldId_1; }
	inline uint32_t* get_address_of_WorldId_1() { return &___WorldId_1; }
	inline void set_WorldId_1(uint32_t value)
	{
		___WorldId_1 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.EntityManager/DeprecatedRegistry/Cell
struct Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75_marshaled_pinvoke
{
	World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07 * ___World_0;
	uint32_t ___WorldId_1;
};
// Native definition for COM marshalling of Unity.Entities.EntityManager/DeprecatedRegistry/Cell
struct Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75_marshaled_com
{
	World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07 * ___World_0;
	uint32_t ___WorldId_1;
};

// Unity.Entities.EntityQuery
struct  EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109 
{
public:
	// Unity.Entities.EntityQueryImpl* Unity.Entities.EntityQuery::__impl
	EntityQueryImpl_tF4EC3E0E8B8074B8B417E3B175695ED99F01B421 * _____impl_0;
	// System.UInt64 Unity.Entities.EntityQuery::__seqno
	uint64_t _____seqno_1;

public:
	inline static int32_t get_offset_of___impl_0() { return static_cast<int32_t>(offsetof(EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109, _____impl_0)); }
	inline EntityQueryImpl_tF4EC3E0E8B8074B8B417E3B175695ED99F01B421 * get___impl_0() const { return _____impl_0; }
	inline EntityQueryImpl_tF4EC3E0E8B8074B8B417E3B175695ED99F01B421 ** get_address_of___impl_0() { return &_____impl_0; }
	inline void set___impl_0(EntityQueryImpl_tF4EC3E0E8B8074B8B417E3B175695ED99F01B421 * value)
	{
		_____impl_0 = value;
	}

	inline static int32_t get_offset_of___seqno_1() { return static_cast<int32_t>(offsetof(EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109, _____seqno_1)); }
	inline uint64_t get___seqno_1() const { return _____seqno_1; }
	inline uint64_t* get_address_of___seqno_1() { return &_____seqno_1; }
	inline void set___seqno_1(uint64_t value)
	{
		_____seqno_1 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.EntityQuery
struct EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109_marshaled_pinvoke
{
	EntityQueryImpl_tF4EC3E0E8B8074B8B417E3B175695ED99F01B421 * _____impl_0;
	uint64_t _____seqno_1;
};
// Native definition for COM marshalling of Unity.Entities.EntityQuery
struct EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109_marshaled_com
{
	EntityQueryImpl_tF4EC3E0E8B8074B8B417E3B175695ED99F01B421 * _____impl_0;
	uint64_t _____seqno_1;
};

// Unity.Entities.EntityQueryFilter_ChangedFilter_<IndexInEntityQuery>e__FixedBuffer
struct  U3CIndexInEntityQueryU3Ee__FixedBuffer_t28227728B47E789DEDD30E3AC589A24221064449 
{
public:
	union
	{
		struct
		{
			// System.Int32 Unity.Entities.EntityQueryFilter_ChangedFilter_<IndexInEntityQuery>e__FixedBuffer::FixedElementField
			int32_t ___FixedElementField_0;
		};
		uint8_t U3CIndexInEntityQueryU3Ee__FixedBuffer_t28227728B47E789DEDD30E3AC589A24221064449__padding[8];
	};

public:
	inline static int32_t get_offset_of_FixedElementField_0() { return static_cast<int32_t>(offsetof(U3CIndexInEntityQueryU3Ee__FixedBuffer_t28227728B47E789DEDD30E3AC589A24221064449, ___FixedElementField_0)); }
	inline int32_t get_FixedElementField_0() const { return ___FixedElementField_0; }
	inline int32_t* get_address_of_FixedElementField_0() { return &___FixedElementField_0; }
	inline void set_FixedElementField_0(int32_t value)
	{
		___FixedElementField_0 = value;
	}
};


// Unity.Entities.EntityQueryFilter_SharedComponentData_<IndexInEntityQuery>e__FixedBuffer
struct  U3CIndexInEntityQueryU3Ee__FixedBuffer_tDA349274AACF056EA2ECB854D24F2E173B4172C9 
{
public:
	union
	{
		struct
		{
			// System.Int32 Unity.Entities.EntityQueryFilter_SharedComponentData_<IndexInEntityQuery>e__FixedBuffer::FixedElementField
			int32_t ___FixedElementField_0;
		};
		uint8_t U3CIndexInEntityQueryU3Ee__FixedBuffer_tDA349274AACF056EA2ECB854D24F2E173B4172C9__padding[8];
	};

public:
	inline static int32_t get_offset_of_FixedElementField_0() { return static_cast<int32_t>(offsetof(U3CIndexInEntityQueryU3Ee__FixedBuffer_tDA349274AACF056EA2ECB854D24F2E173B4172C9, ___FixedElementField_0)); }
	inline int32_t get_FixedElementField_0() const { return ___FixedElementField_0; }
	inline int32_t* get_address_of_FixedElementField_0() { return &___FixedElementField_0; }
	inline void set_FixedElementField_0(int32_t value)
	{
		___FixedElementField_0 = value;
	}
};


// Unity.Entities.EntityQueryFilter_SharedComponentData_<SharedComponentIndex>e__FixedBuffer
struct  U3CSharedComponentIndexU3Ee__FixedBuffer_t8F3ED9BD576230445DE36A06D81462165067FA22 
{
public:
	union
	{
		struct
		{
			// System.Int32 Unity.Entities.EntityQueryFilter_SharedComponentData_<SharedComponentIndex>e__FixedBuffer::FixedElementField
			int32_t ___FixedElementField_0;
		};
		uint8_t U3CSharedComponentIndexU3Ee__FixedBuffer_t8F3ED9BD576230445DE36A06D81462165067FA22__padding[8];
	};

public:
	inline static int32_t get_offset_of_FixedElementField_0() { return static_cast<int32_t>(offsetof(U3CSharedComponentIndexU3Ee__FixedBuffer_t8F3ED9BD576230445DE36A06D81462165067FA22, ___FixedElementField_0)); }
	inline int32_t get_FixedElementField_0() const { return ___FixedElementField_0; }
	inline int32_t* get_address_of_FixedElementField_0() { return &___FixedElementField_0; }
	inline void set_FixedElementField_0(int32_t value)
	{
		___FixedElementField_0 = value;
	}
};


// Unity.Entities.RetainBlobAssetPtr
struct  RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 
{
public:
	// Unity.Entities.BlobAssetHeader* Unity.Entities.RetainBlobAssetPtr::BlobAsset
	BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * ___BlobAsset_0;

public:
	inline static int32_t get_offset_of_BlobAsset_0() { return static_cast<int32_t>(offsetof(RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801, ___BlobAsset_0)); }
	inline BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * get_BlobAsset_0() const { return ___BlobAsset_0; }
	inline BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F ** get_address_of_BlobAsset_0() { return &___BlobAsset_0; }
	inline void set_BlobAsset_0(BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * value)
	{
		___BlobAsset_0 = value;
	}
};


// Unity.Entities.SectionMetadataSetup
struct  SectionMetadataSetup_t572811581DABF646857E271928C5F8412802BDCE 
{
public:
	// System.Int32 Unity.Entities.SectionMetadataSetup::SceneSectionIndex
	int32_t ___SceneSectionIndex_0;

public:
	inline static int32_t get_offset_of_SceneSectionIndex_0() { return static_cast<int32_t>(offsetof(SectionMetadataSetup_t572811581DABF646857E271928C5F8412802BDCE, ___SceneSectionIndex_0)); }
	inline int32_t get_SceneSectionIndex_0() const { return ___SceneSectionIndex_0; }
	inline int32_t* get_address_of_SceneSectionIndex_0() { return &___SceneSectionIndex_0; }
	inline void set_SceneSectionIndex_0(int32_t value)
	{
		___SceneSectionIndex_0 = value;
	}
};


// Unity.Entities.Serialization.SerializeUtility_BlobAssetPtr
struct  BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 
{
public:
	// Unity.Entities.BlobAssetHeader* Unity.Entities.Serialization.SerializeUtility_BlobAssetPtr::header
	BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * ___header_0;

public:
	inline static int32_t get_offset_of_header_0() { return static_cast<int32_t>(offsetof(BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447, ___header_0)); }
	inline BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * get_header_0() const { return ___header_0; }
	inline BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F ** get_address_of_header_0() { return &___header_0; }
	inline void set_header_0(BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * value)
	{
		___header_0 = value;
	}
};


// Unity.Entities.Serialization.SerializeUtility_BufferPatchRecord
struct  BufferPatchRecord_t08EDFAE34A19F6FE4B3CC675E3202A943DF54072 
{
public:
	// System.Int32 Unity.Entities.Serialization.SerializeUtility_BufferPatchRecord::ChunkOffset
	int32_t ___ChunkOffset_0;
	// System.Int32 Unity.Entities.Serialization.SerializeUtility_BufferPatchRecord::AllocSizeBytes
	int32_t ___AllocSizeBytes_1;

public:
	inline static int32_t get_offset_of_ChunkOffset_0() { return static_cast<int32_t>(offsetof(BufferPatchRecord_t08EDFAE34A19F6FE4B3CC675E3202A943DF54072, ___ChunkOffset_0)); }
	inline int32_t get_ChunkOffset_0() const { return ___ChunkOffset_0; }
	inline int32_t* get_address_of_ChunkOffset_0() { return &___ChunkOffset_0; }
	inline void set_ChunkOffset_0(int32_t value)
	{
		___ChunkOffset_0 = value;
	}

	inline static int32_t get_offset_of_AllocSizeBytes_1() { return static_cast<int32_t>(offsetof(BufferPatchRecord_t08EDFAE34A19F6FE4B3CC675E3202A943DF54072, ___AllocSizeBytes_1)); }
	inline int32_t get_AllocSizeBytes_1() const { return ___AllocSizeBytes_1; }
	inline int32_t* get_address_of_AllocSizeBytes_1() { return &___AllocSizeBytes_1; }
	inline void set_AllocSizeBytes_1(int32_t value)
	{
		___AllocSizeBytes_1 = value;
	}
};


// Unity.Entities.Serialization.SerializeUtility_SharedComponentRecord
struct  SharedComponentRecord_t89129D0A5F42E59C3417EF62E96B678C6D5882EB 
{
public:
	// System.UInt64 Unity.Entities.Serialization.SerializeUtility_SharedComponentRecord::StableTypeHash
	uint64_t ___StableTypeHash_0;
	// System.Int32 Unity.Entities.Serialization.SerializeUtility_SharedComponentRecord::ComponentSize
	int32_t ___ComponentSize_1;

public:
	inline static int32_t get_offset_of_StableTypeHash_0() { return static_cast<int32_t>(offsetof(SharedComponentRecord_t89129D0A5F42E59C3417EF62E96B678C6D5882EB, ___StableTypeHash_0)); }
	inline uint64_t get_StableTypeHash_0() const { return ___StableTypeHash_0; }
	inline uint64_t* get_address_of_StableTypeHash_0() { return &___StableTypeHash_0; }
	inline void set_StableTypeHash_0(uint64_t value)
	{
		___StableTypeHash_0 = value;
	}

	inline static int32_t get_offset_of_ComponentSize_1() { return static_cast<int32_t>(offsetof(SharedComponentRecord_t89129D0A5F42E59C3417EF62E96B678C6D5882EB, ___ComponentSize_1)); }
	inline int32_t get_ComponentSize_1() const { return ___ComponentSize_1; }
	inline int32_t* get_address_of_ComponentSize_1() { return &___ComponentSize_1; }
	inline void set_ComponentSize_1(int32_t value)
	{
		___ComponentSize_1 = value;
	}
};


// Unity.Entities.SystemBaseRegistry_Dummy
struct  Dummy_t535D96BBDED5CC782E12E2C7E9D0A4A66AF3C1A4 
{
public:
	union
	{
		struct
		{
		};
		uint8_t Dummy_t535D96BBDED5CC782E12E2C7E9D0A4A66AF3C1A4__padding[1];
	};

public:
};


// Unity.Entities.SystemDependencySafetyUtility_SafetyErrorDetails
struct  SafetyErrorDetails_t704FCE70A3AFA54793A18FA3014F8286C3957FFC 
{
public:
	union
	{
		struct
		{
		};
		uint8_t SafetyErrorDetails_t704FCE70A3AFA54793A18FA3014F8286C3957FFC__padding[1];
	};

public:
};


// Unity.Entities.SystemState_JobHandleData
struct  JobHandleData_t86A8EC33FD280E5A3918BC1A8B992DACA9B81C94 
{
public:
	// System.Void* Unity.Entities.SystemState_JobHandleData::jobGroup
	void* ___jobGroup_0;
	// System.Int32 Unity.Entities.SystemState_JobHandleData::version
	int32_t ___version_1;

public:
	inline static int32_t get_offset_of_jobGroup_0() { return static_cast<int32_t>(offsetof(JobHandleData_t86A8EC33FD280E5A3918BC1A8B992DACA9B81C94, ___jobGroup_0)); }
	inline void* get_jobGroup_0() const { return ___jobGroup_0; }
	inline void** get_address_of_jobGroup_0() { return &___jobGroup_0; }
	inline void set_jobGroup_0(void* value)
	{
		___jobGroup_0 = value;
	}

	inline static int32_t get_offset_of_version_1() { return static_cast<int32_t>(offsetof(JobHandleData_t86A8EC33FD280E5A3918BC1A8B992DACA9B81C94, ___version_1)); }
	inline int32_t get_version_1() const { return ___version_1; }
	inline int32_t* get_address_of_version_1() { return &___version_1; }
	inline void set_version_1(int32_t value)
	{
		___version_1 = value;
	}
};


// Unity.Entities.TypeManager_EntityOffsetInfo
struct  EntityOffsetInfo_t398BFA31DBBAAA0488E257A761D3D254D716DE89 
{
public:
	// System.Int32 Unity.Entities.TypeManager_EntityOffsetInfo::Offset
	int32_t ___Offset_0;

public:
	inline static int32_t get_offset_of_Offset_0() { return static_cast<int32_t>(offsetof(EntityOffsetInfo_t398BFA31DBBAAA0488E257A761D3D254D716DE89, ___Offset_0)); }
	inline int32_t get_Offset_0() const { return ___Offset_0; }
	inline int32_t* get_address_of_Offset_0() { return &___Offset_0; }
	inline void set_Offset_0(int32_t value)
	{
		___Offset_0 = value;
	}
};


// Unity.Entities.TypeManager_ObjectOffsetType
struct  ObjectOffsetType_t6F3C1C66D1540CCB0940C6CB4EE21429C0F9E312 
{
public:
	// System.Void* Unity.Entities.TypeManager_ObjectOffsetType::v0
	void* ___v0_0;
	// System.Void* Unity.Entities.TypeManager_ObjectOffsetType::v1
	void* ___v1_1;

public:
	inline static int32_t get_offset_of_v0_0() { return static_cast<int32_t>(offsetof(ObjectOffsetType_t6F3C1C66D1540CCB0940C6CB4EE21429C0F9E312, ___v0_0)); }
	inline void* get_v0_0() const { return ___v0_0; }
	inline void** get_address_of_v0_0() { return &___v0_0; }
	inline void set_v0_0(void* value)
	{
		___v0_0 = value;
	}

	inline static int32_t get_offset_of_v1_1() { return static_cast<int32_t>(offsetof(ObjectOffsetType_t6F3C1C66D1540CCB0940C6CB4EE21429C0F9E312, ___v1_1)); }
	inline void* get_v1_1() const { return ___v1_1; }
	inline void** get_address_of_v1_1() { return &___v1_1; }
	inline void set_v1_1(void* value)
	{
		___v1_1 = value;
	}
};


// Unity.Entities.UnmanagedComponentSystemDelegates_<BurstFunctions>e__FixedBuffer
struct  U3CBurstFunctionsU3Ee__FixedBuffer_tDB2F24C3B98CCCCCB70B369A41760EB76BCEF27A 
{
public:
	union
	{
		struct
		{
			// System.UInt64 Unity.Entities.UnmanagedComponentSystemDelegates_<BurstFunctions>e__FixedBuffer::FixedElementField
			uint64_t ___FixedElementField_0;
		};
		uint8_t U3CBurstFunctionsU3Ee__FixedBuffer_tDB2F24C3B98CCCCCB70B369A41760EB76BCEF27A__padding[24];
	};

public:
	inline static int32_t get_offset_of_FixedElementField_0() { return static_cast<int32_t>(offsetof(U3CBurstFunctionsU3Ee__FixedBuffer_tDB2F24C3B98CCCCCB70B369A41760EB76BCEF27A, ___FixedElementField_0)); }
	inline uint64_t get_FixedElementField_0() const { return ___FixedElementField_0; }
	inline uint64_t* get_address_of_FixedElementField_0() { return &___FixedElementField_0; }
	inline void set_FixedElementField_0(uint64_t value)
	{
		___FixedElementField_0 = value;
	}
};


// Unity.Entities.UnmanagedComponentSystemDelegates_<GCDefeat1>e__FixedBuffer
struct  U3CGCDefeat1U3Ee__FixedBuffer_t79E93F8FB89C00EE644FF99E71676E28670CB8D3 
{
public:
	union
	{
		struct
		{
			// System.UInt64 Unity.Entities.UnmanagedComponentSystemDelegates_<GCDefeat1>e__FixedBuffer::FixedElementField
			uint64_t ___FixedElementField_0;
		};
		uint8_t U3CGCDefeat1U3Ee__FixedBuffer_t79E93F8FB89C00EE644FF99E71676E28670CB8D3__padding[24];
	};

public:
	inline static int32_t get_offset_of_FixedElementField_0() { return static_cast<int32_t>(offsetof(U3CGCDefeat1U3Ee__FixedBuffer_t79E93F8FB89C00EE644FF99E71676E28670CB8D3, ___FixedElementField_0)); }
	inline uint64_t get_FixedElementField_0() const { return ___FixedElementField_0; }
	inline uint64_t* get_address_of_FixedElementField_0() { return &___FixedElementField_0; }
	inline void set_FixedElementField_0(uint64_t value)
	{
		___FixedElementField_0 = value;
	}
};


// Unity.Entities.UnmanagedComponentSystemDelegates_<ManagedFunctions>e__FixedBuffer
struct  U3CManagedFunctionsU3Ee__FixedBuffer_tE5A50190D12E6C9DFD1AD81BCC01DD1CF8873039 
{
public:
	union
	{
		struct
		{
			// System.UInt64 Unity.Entities.UnmanagedComponentSystemDelegates_<ManagedFunctions>e__FixedBuffer::FixedElementField
			uint64_t ___FixedElementField_0;
		};
		uint8_t U3CManagedFunctionsU3Ee__FixedBuffer_tE5A50190D12E6C9DFD1AD81BCC01DD1CF8873039__padding[24];
	};

public:
	inline static int32_t get_offset_of_FixedElementField_0() { return static_cast<int32_t>(offsetof(U3CManagedFunctionsU3Ee__FixedBuffer_tE5A50190D12E6C9DFD1AD81BCC01DD1CF8873039, ___FixedElementField_0)); }
	inline uint64_t get_FixedElementField_0() const { return ___FixedElementField_0; }
	inline uint64_t* get_address_of_FixedElementField_0() { return &___FixedElementField_0; }
	inline void set_FixedElementField_0(uint64_t value)
	{
		___FixedElementField_0 = value;
	}
};


// Unity.Entities.World_NoAllocReadOnlyCollection`1<Unity.Entities.ComponentSystemBase>
struct  NoAllocReadOnlyCollection_1_t3ECE9AA8150FFEF9B06892853E38309BF4091FB5 
{
public:
	// System.Collections.Generic.List`1<T> Unity.Entities.World_NoAllocReadOnlyCollection`1::m_Source
	List_1_t3F15B202D7C7B7EEB8B86FFA8C5D2156C6F32C78 * ___m_Source_0;

public:
	inline static int32_t get_offset_of_m_Source_0() { return static_cast<int32_t>(offsetof(NoAllocReadOnlyCollection_1_t3ECE9AA8150FFEF9B06892853E38309BF4091FB5, ___m_Source_0)); }
	inline List_1_t3F15B202D7C7B7EEB8B86FFA8C5D2156C6F32C78 * get_m_Source_0() const { return ___m_Source_0; }
	inline List_1_t3F15B202D7C7B7EEB8B86FFA8C5D2156C6F32C78 ** get_address_of_m_Source_0() { return &___m_Source_0; }
	inline void set_m_Source_0(List_1_t3F15B202D7C7B7EEB8B86FFA8C5D2156C6F32C78 * value)
	{
		___m_Source_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Source_0), (void*)value);
	}
};


// Unity.Entities.World_NoAllocReadOnlyCollection`1<Unity.Entities.World>
struct  NoAllocReadOnlyCollection_1_t80544BA80B59053667CC4B79E3461635AA6E5EE4 
{
public:
	// System.Collections.Generic.List`1<T> Unity.Entities.World_NoAllocReadOnlyCollection`1::m_Source
	List_1_t84AC7439EE119A84B7D9198C8EB4AC483255C192 * ___m_Source_0;

public:
	inline static int32_t get_offset_of_m_Source_0() { return static_cast<int32_t>(offsetof(NoAllocReadOnlyCollection_1_t80544BA80B59053667CC4B79E3461635AA6E5EE4, ___m_Source_0)); }
	inline List_1_t84AC7439EE119A84B7D9198C8EB4AC483255C192 * get_m_Source_0() const { return ___m_Source_0; }
	inline List_1_t84AC7439EE119A84B7D9198C8EB4AC483255C192 ** get_address_of_m_Source_0() { return &___m_Source_0; }
	inline void set_m_Source_0(List_1_t84AC7439EE119A84B7D9198C8EB4AC483255C192 * value)
	{
		___m_Source_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Source_0), (void*)value);
	}
};


// Unity.Entities.World_StateAllocLevel1_<SystemPointer>e__FixedBuffer
struct  U3CSystemPointerU3Ee__FixedBuffer_t19C0D38015C844F379AB29453AE403CBDD5E16B1 
{
public:
	union
	{
		struct
		{
			// System.UInt64 Unity.Entities.World_StateAllocLevel1_<SystemPointer>e__FixedBuffer::FixedElementField
			uint64_t ___FixedElementField_0;
		};
		uint8_t U3CSystemPointerU3Ee__FixedBuffer_t19C0D38015C844F379AB29453AE403CBDD5E16B1__padding[512];
	};

public:
	inline static int32_t get_offset_of_FixedElementField_0() { return static_cast<int32_t>(offsetof(U3CSystemPointerU3Ee__FixedBuffer_t19C0D38015C844F379AB29453AE403CBDD5E16B1, ___FixedElementField_0)); }
	inline uint64_t get_FixedElementField_0() const { return ___FixedElementField_0; }
	inline uint64_t* get_address_of_FixedElementField_0() { return &___FixedElementField_0; }
	inline void set_FixedElementField_0(uint64_t value)
	{
		___FixedElementField_0 = value;
	}
};


// Unity.Entities.World_StateAllocLevel1_<TypeHash>e__FixedBuffer
struct  U3CTypeHashU3Ee__FixedBuffer_tEE844B127F8E28AF008BEC971AE9386972B3D6D5 
{
public:
	union
	{
		struct
		{
			// System.Int64 Unity.Entities.World_StateAllocLevel1_<TypeHash>e__FixedBuffer::FixedElementField
			int64_t ___FixedElementField_0;
		};
		uint8_t U3CTypeHashU3Ee__FixedBuffer_tEE844B127F8E28AF008BEC971AE9386972B3D6D5__padding[512];
	};

public:
	inline static int32_t get_offset_of_FixedElementField_0() { return static_cast<int32_t>(offsetof(U3CTypeHashU3Ee__FixedBuffer_tEE844B127F8E28AF008BEC971AE9386972B3D6D5, ___FixedElementField_0)); }
	inline int64_t get_FixedElementField_0() const { return ___FixedElementField_0; }
	inline int64_t* get_address_of_FixedElementField_0() { return &___FixedElementField_0; }
	inline void set_FixedElementField_0(int64_t value)
	{
		___FixedElementField_0 = value;
	}
};


// Unity.Entities.World_StateAllocLevel1_<Version>e__FixedBuffer
struct  U3CVersionU3Ee__FixedBuffer_tB3AA164CD6E485CFAEFC56242AE5BE61BE39446F 
{
public:
	union
	{
		struct
		{
			// System.UInt16 Unity.Entities.World_StateAllocLevel1_<Version>e__FixedBuffer::FixedElementField
			uint16_t ___FixedElementField_0;
		};
		uint8_t U3CVersionU3Ee__FixedBuffer_tB3AA164CD6E485CFAEFC56242AE5BE61BE39446F__padding[128];
	};

public:
	inline static int32_t get_offset_of_FixedElementField_0() { return static_cast<int32_t>(offsetof(U3CVersionU3Ee__FixedBuffer_tB3AA164CD6E485CFAEFC56242AE5BE61BE39446F, ___FixedElementField_0)); }
	inline uint16_t get_FixedElementField_0() const { return ___FixedElementField_0; }
	inline uint16_t* get_address_of_FixedElementField_0() { return &___FixedElementField_0; }
	inline void set_FixedElementField_0(uint16_t value)
	{
		___FixedElementField_0 = value;
	}
};


// Unity.Entities.World_StateAllocator
struct  StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB 
{
public:
	// System.UInt64 Unity.Entities.World_StateAllocator::m_FreeBits
	uint64_t ___m_FreeBits_0;
	// Unity.Entities.World_StateAllocLevel1* Unity.Entities.World_StateAllocator::m_Level1
	StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * ___m_Level1_1;

public:
	inline static int32_t get_offset_of_m_FreeBits_0() { return static_cast<int32_t>(offsetof(StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB, ___m_FreeBits_0)); }
	inline uint64_t get_m_FreeBits_0() const { return ___m_FreeBits_0; }
	inline uint64_t* get_address_of_m_FreeBits_0() { return &___m_FreeBits_0; }
	inline void set_m_FreeBits_0(uint64_t value)
	{
		___m_FreeBits_0 = value;
	}

	inline static int32_t get_offset_of_m_Level1_1() { return static_cast<int32_t>(offsetof(StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB, ___m_Level1_1)); }
	inline StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * get_m_Level1_1() const { return ___m_Level1_1; }
	inline StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 ** get_address_of_m_Level1_1() { return &___m_Level1_1; }
	inline void set_m_Level1_1(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * value)
	{
		___m_Level1_1 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.World/StateAllocator
struct StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB_marshaled_pinvoke
{
	uint64_t ___m_FreeBits_0;
	StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * ___m_Level1_1;
};
// Native definition for COM marshalling of Unity.Entities.World/StateAllocator
struct StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB_marshaled_com
{
	uint64_t ___m_FreeBits_0;
	StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * ___m_Level1_1;
};

// Unity.Mathematics.uint4
struct  uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 
{
public:
	// System.UInt32 Unity.Mathematics.uint4::x
	uint32_t ___x_0;
	// System.UInt32 Unity.Mathematics.uint4::y
	uint32_t ___y_1;
	// System.UInt32 Unity.Mathematics.uint4::z
	uint32_t ___z_2;
	// System.UInt32 Unity.Mathematics.uint4::w
	uint32_t ___w_3;

public:
	inline static int32_t get_offset_of_x_0() { return static_cast<int32_t>(offsetof(uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92, ___x_0)); }
	inline uint32_t get_x_0() const { return ___x_0; }
	inline uint32_t* get_address_of_x_0() { return &___x_0; }
	inline void set_x_0(uint32_t value)
	{
		___x_0 = value;
	}

	inline static int32_t get_offset_of_y_1() { return static_cast<int32_t>(offsetof(uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92, ___y_1)); }
	inline uint32_t get_y_1() const { return ___y_1; }
	inline uint32_t* get_address_of_y_1() { return &___y_1; }
	inline void set_y_1(uint32_t value)
	{
		___y_1 = value;
	}

	inline static int32_t get_offset_of_z_2() { return static_cast<int32_t>(offsetof(uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92, ___z_2)); }
	inline uint32_t get_z_2() const { return ___z_2; }
	inline uint32_t* get_address_of_z_2() { return &___z_2; }
	inline void set_z_2(uint32_t value)
	{
		___z_2 = value;
	}

	inline static int32_t get_offset_of_w_3() { return static_cast<int32_t>(offsetof(uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92, ___w_3)); }
	inline uint32_t get_w_3() const { return ___w_3; }
	inline uint32_t* get_address_of_w_3() { return &___w_3; }
	inline void set_w_3(uint32_t value)
	{
		___w_3 = value;
	}
};

struct uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92_StaticFields
{
public:
	// Unity.Mathematics.uint4 Unity.Mathematics.uint4::zero
	uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92  ___zero_4;

public:
	inline static int32_t get_offset_of_zero_4() { return static_cast<int32_t>(offsetof(uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92_StaticFields, ___zero_4)); }
	inline uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92  get_zero_4() const { return ___zero_4; }
	inline uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * get_address_of_zero_4() { return &___zero_4; }
	inline void set_zero_4(uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92  value)
	{
		___zero_4 = value;
	}
};


// Unity.Properties.Generated.Unity_Entities_Entity_PropertyBag_Index_Property
struct  Index_Property_t835C3FA617A3E01A668DA48CE5835BE4ECB9F808  : public Property_2_tCABF4DCC1CD1B4A9380E57084825CBE0CD5650CF
{
public:

public:
};


// Unity.Properties.Generated.Unity_Entities_Entity_PropertyBag_Version_Property
struct  Version_Property_tA459AC543294951D12631B8CC94B7319B6423CC1  : public Property_2_tCABF4DCC1CD1B4A9380E57084825CBE0CD5650CF
{
public:

public:
};


// Unity.Properties.Generated.Unity_Entities_Hash128_PropertyBag_Value_Property
struct  Value_Property_t9606F88DB5D8B3CB45FE05D0D3CC24AE4CCCA547  : public Property_2_tC735BFFF1BC3639C475ACE130B7C17946AADC830
{
public:

public:
};


// Unity.Properties.Generated.Unity_Entities_PostLoadCommandBuffer_PropertyBag_CommandBuffer_Property
struct  CommandBuffer_Property_tDCB2D8FF230260A52B1CB3A97E5BC39EEAE1BBFF  : public Property_2_t40BE748BD8D19926CED3273AD0DAECECE3176A34
{
public:

public:
};


// Unity.Properties.Generated.Unity_Entities_SceneSection_PropertyBag_SceneGUID_Property
struct  SceneGUID_Property_t286F056FA890C76A85CAEED96FB8F2DC18718F24  : public Property_2_t2BF85B4F1B3C1A7B861BCF86F0135D4C29B3E71A
{
public:

public:
};


// Unity.Properties.Generated.Unity_Entities_SceneSection_PropertyBag_Section_Property
struct  Section_Property_t1AB3E9286E418F5B1D6EA8E68E25BFF01B901BDB  : public Property_2_t429FDAC3FDDA865DCCA0BEA2701D06815301B7A7
{
public:

public:
};


// Unity.Properties.Generated.Unity_Entities_SceneTag_PropertyBag_SceneEntity_Property
struct  SceneEntity_Property_tDA7084AEB4F0BEF5863A78E97A21028595F2E2E0  : public Property_2_tEAB8BD93E42D6310C52481E57E5BE3E4F63917B0
{
public:

public:
};


// Unity.Properties.Generated.Unity_Entities_SectionMetadataSetup_PropertyBag_SceneSectionIndex_Property
struct  SceneSectionIndex_Property_t123ADC180D58E59803781FA9CA0C27F3F4AD9FA6  : public Property_2_t300D32A567141F1F72A198697AF2D006E2898B23
{
public:

public:
};


// Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_w_Property
struct  w_Property_tE1DFF192A694126418686CC4C65AD495A7670315  : public Property_2_tB965C4D1501C79D2FADA36497D75F6F4BEC5DAED
{
public:

public:
};


// Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_x_Property
struct  x_Property_tC992C828E7E67B8457C9631A95F7502D91029124  : public Property_2_tB965C4D1501C79D2FADA36497D75F6F4BEC5DAED
{
public:

public:
};


// Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_y_Property
struct  y_Property_t917A1CEE407CE842A5B28FC2850257DC84226D5D  : public Property_2_tB965C4D1501C79D2FADA36497D75F6F4BEC5DAED
{
public:

public:
};


// Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_z_Property
struct  z_Property_tCC6F6AC68C86D8C1E6BCFFE6762B55DA5B817676  : public Property_2_tB965C4D1501C79D2FADA36497D75F6F4BEC5DAED
{
public:

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


// System.Reflection.ConstructorInfo
struct  ConstructorInfo_t449AEC508DCA508EE46784C4F2716545488ACD5B  : public MethodBase_t
{
public:

public:
};

struct ConstructorInfo_t449AEC508DCA508EE46784C4F2716545488ACD5B_StaticFields
{
public:
	// System.String System.Reflection.ConstructorInfo::ConstructorName
	String_t* ___ConstructorName_0;
	// System.String System.Reflection.ConstructorInfo::TypeConstructorName
	String_t* ___TypeConstructorName_1;

public:
	inline static int32_t get_offset_of_ConstructorName_0() { return static_cast<int32_t>(offsetof(ConstructorInfo_t449AEC508DCA508EE46784C4F2716545488ACD5B_StaticFields, ___ConstructorName_0)); }
	inline String_t* get_ConstructorName_0() const { return ___ConstructorName_0; }
	inline String_t** get_address_of_ConstructorName_0() { return &___ConstructorName_0; }
	inline void set_ConstructorName_0(String_t* value)
	{
		___ConstructorName_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___ConstructorName_0), (void*)value);
	}

	inline static int32_t get_offset_of_TypeConstructorName_1() { return static_cast<int32_t>(offsetof(ConstructorInfo_t449AEC508DCA508EE46784C4F2716545488ACD5B_StaticFields, ___TypeConstructorName_1)); }
	inline String_t* get_TypeConstructorName_1() const { return ___TypeConstructorName_1; }
	inline String_t** get_address_of_TypeConstructorName_1() { return &___TypeConstructorName_1; }
	inline void set_TypeConstructorName_1(String_t* value)
	{
		___TypeConstructorName_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___TypeConstructorName_1), (void*)value);
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


// Unity.Collections.Allocator
struct  Allocator_t9888223DEF4F46F3419ECFCCD0753599BEE52A05 
{
public:
	// System.Int32 Unity.Collections.Allocator::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(Allocator_t9888223DEF4F46F3419ECFCCD0753599BEE52A05, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Unity.Collections.LowLevel.Unsafe.UnsafeList
struct  UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA 
{
public:
	// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeList::Ptr
	void* ___Ptr_0;
	// System.Int32 Unity.Collections.LowLevel.Unsafe.UnsafeList::Length
	int32_t ___Length_1;
	// System.Int32 Unity.Collections.LowLevel.Unsafe.UnsafeList::Capacity
	int32_t ___Capacity_2;
	// Unity.Collections.AllocatorManager_AllocatorHandle Unity.Collections.LowLevel.Unsafe.UnsafeList::Allocator
	AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  ___Allocator_3;

public:
	inline static int32_t get_offset_of_Ptr_0() { return static_cast<int32_t>(offsetof(UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA, ___Ptr_0)); }
	inline void* get_Ptr_0() const { return ___Ptr_0; }
	inline void** get_address_of_Ptr_0() { return &___Ptr_0; }
	inline void set_Ptr_0(void* value)
	{
		___Ptr_0 = value;
	}

	inline static int32_t get_offset_of_Length_1() { return static_cast<int32_t>(offsetof(UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA, ___Length_1)); }
	inline int32_t get_Length_1() const { return ___Length_1; }
	inline int32_t* get_address_of_Length_1() { return &___Length_1; }
	inline void set_Length_1(int32_t value)
	{
		___Length_1 = value;
	}

	inline static int32_t get_offset_of_Capacity_2() { return static_cast<int32_t>(offsetof(UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA, ___Capacity_2)); }
	inline int32_t get_Capacity_2() const { return ___Capacity_2; }
	inline int32_t* get_address_of_Capacity_2() { return &___Capacity_2; }
	inline void set_Capacity_2(int32_t value)
	{
		___Capacity_2 = value;
	}

	inline static int32_t get_offset_of_Allocator_3() { return static_cast<int32_t>(offsetof(UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA, ___Allocator_3)); }
	inline AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  get_Allocator_3() const { return ___Allocator_3; }
	inline AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A * get_address_of_Allocator_3() { return &___Allocator_3; }
	inline void set_Allocator_3(AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  value)
	{
		___Allocator_3 = value;
	}
};


// Unity.Collections.LowLevel.Unsafe.UnsafePtrList
struct  UnsafePtrList_tB7C4F6B8D4EFEF738EDE139C9C61C737865AF579 
{
public:
	// System.Void** Unity.Collections.LowLevel.Unsafe.UnsafePtrList::Ptr
	void** ___Ptr_0;
	// System.Int32 Unity.Collections.LowLevel.Unsafe.UnsafePtrList::length
	int32_t ___length_1;
	// System.Int32 Unity.Collections.LowLevel.Unsafe.UnsafePtrList::capacity
	int32_t ___capacity_2;
	// Unity.Collections.AllocatorManager_AllocatorHandle Unity.Collections.LowLevel.Unsafe.UnsafePtrList::Allocator
	AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  ___Allocator_3;

public:
	inline static int32_t get_offset_of_Ptr_0() { return static_cast<int32_t>(offsetof(UnsafePtrList_tB7C4F6B8D4EFEF738EDE139C9C61C737865AF579, ___Ptr_0)); }
	inline void** get_Ptr_0() const { return ___Ptr_0; }
	inline void*** get_address_of_Ptr_0() { return &___Ptr_0; }
	inline void set_Ptr_0(void** value)
	{
		___Ptr_0 = value;
	}

	inline static int32_t get_offset_of_length_1() { return static_cast<int32_t>(offsetof(UnsafePtrList_tB7C4F6B8D4EFEF738EDE139C9C61C737865AF579, ___length_1)); }
	inline int32_t get_length_1() const { return ___length_1; }
	inline int32_t* get_address_of_length_1() { return &___length_1; }
	inline void set_length_1(int32_t value)
	{
		___length_1 = value;
	}

	inline static int32_t get_offset_of_capacity_2() { return static_cast<int32_t>(offsetof(UnsafePtrList_tB7C4F6B8D4EFEF738EDE139C9C61C737865AF579, ___capacity_2)); }
	inline int32_t get_capacity_2() const { return ___capacity_2; }
	inline int32_t* get_address_of_capacity_2() { return &___capacity_2; }
	inline void set_capacity_2(int32_t value)
	{
		___capacity_2 = value;
	}

	inline static int32_t get_offset_of_Allocator_3() { return static_cast<int32_t>(offsetof(UnsafePtrList_tB7C4F6B8D4EFEF738EDE139C9C61C737865AF579, ___Allocator_3)); }
	inline AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  get_Allocator_3() const { return ___Allocator_3; }
	inline AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A * get_address_of_Allocator_3() { return &___Allocator_3; }
	inline void set_Allocator_3(AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  value)
	{
		___Allocator_3 = value;
	}
};


// Unity.Entities.BlobAssetReference`1<System.Byte>
struct  BlobAssetReference_1_t9DE40F3E122A46CD5D0B6486AC40AB530B4D1C34 
{
public:
	// Unity.Entities.BlobAssetReferenceData Unity.Entities.BlobAssetReference`1::m_data
	BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1  ___m_data_0;

public:
	inline static int32_t get_offset_of_m_data_0() { return static_cast<int32_t>(offsetof(BlobAssetReference_1_t9DE40F3E122A46CD5D0B6486AC40AB530B4D1C34, ___m_data_0)); }
	inline BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1  get_m_data_0() const { return ___m_data_0; }
	inline BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1 * get_address_of_m_data_0() { return &___m_data_0; }
	inline void set_m_data_0(BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1  value)
	{
		___m_data_0 = value;
	}
};


// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_Entity
struct  LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13 
{
public:
	// Unity.Entities.ArchetypeChunkEntityType Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_Entity::_type
	ArchetypeChunkEntityType_t9283D156272DAE65135E83A92ABA7778CE43B640  ____type_0;

public:
	inline static int32_t get_offset_of__type_0() { return static_cast<int32_t>(offsetof(LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13, ____type_0)); }
	inline ArchetypeChunkEntityType_t9283D156272DAE65135E83A92ABA7778CE43B640  get__type_0() const { return ____type_0; }
	inline ArchetypeChunkEntityType_t9283D156272DAE65135E83A92ABA7778CE43B640 * get_address_of__type_0() { return &____type_0; }
	inline void set__type_0(ArchetypeChunkEntityType_t9283D156272DAE65135E83A92ABA7778CE43B640  value)
	{
		____type_0 = value;
	}
};


// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1<Unity.Entities.RetainBlobAssetBatchPtr>
struct  LambdaParameterValueProvider_IComponentData_1_t1D786064C86636813BA4C8371E0CF96EAC3BAB81 
{
public:
	// Unity.Entities.ArchetypeChunkComponentType`1<T> Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1::_type
	ArchetypeChunkComponentType_1_tFD5C7F1417F5147765D69E09498A481C21F64CA5  ____type_0;

public:
	inline static int32_t get_offset_of__type_0() { return static_cast<int32_t>(offsetof(LambdaParameterValueProvider_IComponentData_1_t1D786064C86636813BA4C8371E0CF96EAC3BAB81, ____type_0)); }
	inline ArchetypeChunkComponentType_1_tFD5C7F1417F5147765D69E09498A481C21F64CA5  get__type_0() const { return ____type_0; }
	inline ArchetypeChunkComponentType_1_tFD5C7F1417F5147765D69E09498A481C21F64CA5 * get_address_of__type_0() { return &____type_0; }
	inline void set__type_0(ArchetypeChunkComponentType_1_tFD5C7F1417F5147765D69E09498A481C21F64CA5  value)
	{
		____type_0 = value;
	}
};


// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1<Unity.Entities.RetainBlobAssetPtr>
struct  LambdaParameterValueProvider_IComponentData_1_t2EFFCFAFE87229878D536B5C44DC62239C46601A 
{
public:
	// Unity.Entities.ArchetypeChunkComponentType`1<T> Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1::_type
	ArchetypeChunkComponentType_1_tB290CDB5AD796AE285F864C9808F9D98EA41BB84  ____type_0;

public:
	inline static int32_t get_offset_of__type_0() { return static_cast<int32_t>(offsetof(LambdaParameterValueProvider_IComponentData_1_t2EFFCFAFE87229878D536B5C44DC62239C46601A, ____type_0)); }
	inline ArchetypeChunkComponentType_1_tB290CDB5AD796AE285F864C9808F9D98EA41BB84  get__type_0() const { return ____type_0; }
	inline ArchetypeChunkComponentType_1_tB290CDB5AD796AE285F864C9808F9D98EA41BB84 * get_address_of__type_0() { return &____type_0; }
	inline void set__type_0(ArchetypeChunkComponentType_1_tB290CDB5AD796AE285F864C9808F9D98EA41BB84  value)
	{
		____type_0 = value;
	}
};


// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1<Unity.Entities.RetainBlobAssets>
struct  LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4 
{
public:
	// Unity.Entities.ArchetypeChunkComponentType`1<T> Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1::_type
	ArchetypeChunkComponentType_1_t09298C5D6AADE312DF38D90B33DFF235D5F27659  ____type_0;

public:
	inline static int32_t get_offset_of__type_0() { return static_cast<int32_t>(offsetof(LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4, ____type_0)); }
	inline ArchetypeChunkComponentType_1_t09298C5D6AADE312DF38D90B33DFF235D5F27659  get__type_0() const { return ____type_0; }
	inline ArchetypeChunkComponentType_1_t09298C5D6AADE312DF38D90B33DFF235D5F27659 * get_address_of__type_0() { return &____type_0; }
	inline void set__type_0(ArchetypeChunkComponentType_1_t09298C5D6AADE312DF38D90B33DFF235D5F27659  value)
	{
		____type_0 = value;
	}
};


// Unity.Entities.ComponentType_AccessMode
struct  AccessMode_tC18A32540E98A6711B943BEE8E79B672A111E30F 
{
public:
	// System.Int32 Unity.Entities.ComponentType_AccessMode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(AccessMode_tC18A32540E98A6711B943BEE8E79B672A111E30F, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Unity.Entities.EntityPatcher_OffsetEntityPair
struct  OffsetEntityPair_tEA2146CEDACEF29E97936D20BC88923609DC4B8A 
{
public:
	// System.Int32 Unity.Entities.EntityPatcher_OffsetEntityPair::Offset
	int32_t ___Offset_0;
	// Unity.Entities.Entity Unity.Entities.EntityPatcher_OffsetEntityPair::TargetEntity
	Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___TargetEntity_1;

public:
	inline static int32_t get_offset_of_Offset_0() { return static_cast<int32_t>(offsetof(OffsetEntityPair_tEA2146CEDACEF29E97936D20BC88923609DC4B8A, ___Offset_0)); }
	inline int32_t get_Offset_0() const { return ___Offset_0; }
	inline int32_t* get_address_of_Offset_0() { return &___Offset_0; }
	inline void set_Offset_0(int32_t value)
	{
		___Offset_0 = value;
	}

	inline static int32_t get_offset_of_TargetEntity_1() { return static_cast<int32_t>(offsetof(OffsetEntityPair_tEA2146CEDACEF29E97936D20BC88923609DC4B8A, ___TargetEntity_1)); }
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  get_TargetEntity_1() const { return ___TargetEntity_1; }
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * get_address_of_TargetEntity_1() { return &___TargetEntity_1; }
	inline void set_TargetEntity_1(Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  value)
	{
		___TargetEntity_1 = value;
	}
};


// Unity.Entities.EntityRemapUtility_EntityRemapInfo
struct  EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334 
{
public:
	// System.Int32 Unity.Entities.EntityRemapUtility_EntityRemapInfo::SourceVersion
	int32_t ___SourceVersion_0;
	// Unity.Entities.Entity Unity.Entities.EntityRemapUtility_EntityRemapInfo::Target
	Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___Target_1;

public:
	inline static int32_t get_offset_of_SourceVersion_0() { return static_cast<int32_t>(offsetof(EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334, ___SourceVersion_0)); }
	inline int32_t get_SourceVersion_0() const { return ___SourceVersion_0; }
	inline int32_t* get_address_of_SourceVersion_0() { return &___SourceVersion_0; }
	inline void set_SourceVersion_0(int32_t value)
	{
		___SourceVersion_0 = value;
	}

	inline static int32_t get_offset_of_Target_1() { return static_cast<int32_t>(offsetof(EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334, ___Target_1)); }
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  get_Target_1() const { return ___Target_1; }
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * get_address_of_Target_1() { return &___Target_1; }
	inline void set_Target_1(Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  value)
	{
		___Target_1 = value;
	}
};


// Unity.Entities.Hash128
struct  Hash128_t8214C0670F24DF267392561913434E82117B6131 
{
public:
	// Unity.Mathematics.uint4 Unity.Entities.Hash128::Value
	uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92  ___Value_0;

public:
	inline static int32_t get_offset_of_Value_0() { return static_cast<int32_t>(offsetof(Hash128_t8214C0670F24DF267392561913434E82117B6131, ___Value_0)); }
	inline uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92  get_Value_0() const { return ___Value_0; }
	inline uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * get_address_of_Value_0() { return &___Value_0; }
	inline void set_Value_0(uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92  value)
	{
		___Value_0 = value;
	}
};

struct Hash128_t8214C0670F24DF267392561913434E82117B6131_StaticFields
{
public:
	// System.Char[] Unity.Entities.Hash128::k_HexToLiteral
	CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34* ___k_HexToLiteral_1;
	// System.SByte[] Unity.Entities.Hash128::k_LiteralToHex
	SByteU5BU5D_t7D94C53295E6116625EA7CC7DEA21FEDC39869E7* ___k_LiteralToHex_2;

public:
	inline static int32_t get_offset_of_k_HexToLiteral_1() { return static_cast<int32_t>(offsetof(Hash128_t8214C0670F24DF267392561913434E82117B6131_StaticFields, ___k_HexToLiteral_1)); }
	inline CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34* get_k_HexToLiteral_1() const { return ___k_HexToLiteral_1; }
	inline CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34** get_address_of_k_HexToLiteral_1() { return &___k_HexToLiteral_1; }
	inline void set_k_HexToLiteral_1(CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34* value)
	{
		___k_HexToLiteral_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___k_HexToLiteral_1), (void*)value);
	}

	inline static int32_t get_offset_of_k_LiteralToHex_2() { return static_cast<int32_t>(offsetof(Hash128_t8214C0670F24DF267392561913434E82117B6131_StaticFields, ___k_LiteralToHex_2)); }
	inline SByteU5BU5D_t7D94C53295E6116625EA7CC7DEA21FEDC39869E7* get_k_LiteralToHex_2() const { return ___k_LiteralToHex_2; }
	inline SByteU5BU5D_t7D94C53295E6116625EA7CC7DEA21FEDC39869E7** get_address_of_k_LiteralToHex_2() { return &___k_LiteralToHex_2; }
	inline void set_k_LiteralToHex_2(SByteU5BU5D_t7D94C53295E6116625EA7CC7DEA21FEDC39869E7* value)
	{
		___k_LiteralToHex_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___k_LiteralToHex_2), (void*)value);
	}
};


// Unity.Entities.PostLoadCommandBuffer
struct  PostLoadCommandBuffer_tD45726986D9D5D6BF6AB396D8DD997EB3D7AD634  : public RuntimeObject
{
public:
	// Unity.Entities.EntityCommandBuffer Unity.Entities.PostLoadCommandBuffer::CommandBuffer
	EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764  ___CommandBuffer_0;
	// System.Int32 Unity.Entities.PostLoadCommandBuffer::RefCount
	int32_t ___RefCount_1;

public:
	inline static int32_t get_offset_of_CommandBuffer_0() { return static_cast<int32_t>(offsetof(PostLoadCommandBuffer_tD45726986D9D5D6BF6AB396D8DD997EB3D7AD634, ___CommandBuffer_0)); }
	inline EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764  get_CommandBuffer_0() const { return ___CommandBuffer_0; }
	inline EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764 * get_address_of_CommandBuffer_0() { return &___CommandBuffer_0; }
	inline void set_CommandBuffer_0(EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764  value)
	{
		___CommandBuffer_0 = value;
	}

	inline static int32_t get_offset_of_RefCount_1() { return static_cast<int32_t>(offsetof(PostLoadCommandBuffer_tD45726986D9D5D6BF6AB396D8DD997EB3D7AD634, ___RefCount_1)); }
	inline int32_t get_RefCount_1() const { return ___RefCount_1; }
	inline int32_t* get_address_of_RefCount_1() { return &___RefCount_1; }
	inline void set_RefCount_1(int32_t value)
	{
		___RefCount_1 = value;
	}
};


// Unity.Entities.SceneTag
struct  SceneTag_t080CBEF3258F69B515ED15B950061C372200F206 
{
public:
	// Unity.Entities.Entity Unity.Entities.SceneTag::SceneEntity
	Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___SceneEntity_0;

public:
	inline static int32_t get_offset_of_SceneEntity_0() { return static_cast<int32_t>(offsetof(SceneTag_t080CBEF3258F69B515ED15B950061C372200F206, ___SceneEntity_0)); }
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  get_SceneEntity_0() const { return ___SceneEntity_0; }
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * get_address_of_SceneEntity_0() { return &___SceneEntity_0; }
	inline void set_SceneEntity_0(Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  value)
	{
		___SceneEntity_0 = value;
	}
};


// Unity.Entities.TypeManager_SharedBlobAssetRefOffset
struct  SharedBlobAssetRefOffset_tE4EC29E58257EEC230D11A35AB22B47D9D9DC0E6  : public RuntimeObject
{
public:

public:
};

struct SharedBlobAssetRefOffset_tE4EC29E58257EEC230D11A35AB22B47D9D9DC0E6_StaticFields
{
public:
	// Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager_SharedBlobAssetRefOffset::Ref
	SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  ___Ref_0;

public:
	inline static int32_t get_offset_of_Ref_0() { return static_cast<int32_t>(offsetof(SharedBlobAssetRefOffset_tE4EC29E58257EEC230D11A35AB22B47D9D9DC0E6_StaticFields, ___Ref_0)); }
	inline SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  get_Ref_0() const { return ___Ref_0; }
	inline SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6 * get_address_of_Ref_0() { return &___Ref_0; }
	inline void set_Ref_0(SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  value)
	{
		___Ref_0 = value;
	}
};


// Unity.Entities.TypeManager_SharedEntityOffsetInfo
struct  SharedEntityOffsetInfo_tE11E3366E1064E671CD7B5C0B457A4A09DCCC068  : public RuntimeObject
{
public:

public:
};

struct SharedEntityOffsetInfo_tE11E3366E1064E671CD7B5C0B457A4A09DCCC068_StaticFields
{
public:
	// Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager_SharedEntityOffsetInfo::Ref
	SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  ___Ref_0;

public:
	inline static int32_t get_offset_of_Ref_0() { return static_cast<int32_t>(offsetof(SharedEntityOffsetInfo_tE11E3366E1064E671CD7B5C0B457A4A09DCCC068_StaticFields, ___Ref_0)); }
	inline SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  get_Ref_0() const { return ___Ref_0; }
	inline SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6 * get_address_of_Ref_0() { return &___Ref_0; }
	inline void set_Ref_0(SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  value)
	{
		___Ref_0 = value;
	}
};


// Unity.Entities.TypeManager_SharedTypeInfo
struct  SharedTypeInfo_t6D9232712A2DC192E8DCC16F0AE24EE2508CCD05  : public RuntimeObject
{
public:

public:
};

struct SharedTypeInfo_t6D9232712A2DC192E8DCC16F0AE24EE2508CCD05_StaticFields
{
public:
	// Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager_SharedTypeInfo::Ref
	SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  ___Ref_0;

public:
	inline static int32_t get_offset_of_Ref_0() { return static_cast<int32_t>(offsetof(SharedTypeInfo_t6D9232712A2DC192E8DCC16F0AE24EE2508CCD05_StaticFields, ___Ref_0)); }
	inline SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  get_Ref_0() const { return ___Ref_0; }
	inline SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6 * get_address_of_Ref_0() { return &___Ref_0; }
	inline void set_Ref_0(SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  value)
	{
		___Ref_0 = value;
	}
};


// Unity.Entities.TypeManager_SharedWriteGroup
struct  SharedWriteGroup_t43861216B5BB89853E40798D5170B5574E8A46FB  : public RuntimeObject
{
public:

public:
};

struct SharedWriteGroup_t43861216B5BB89853E40798D5170B5574E8A46FB_StaticFields
{
public:
	// Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager_SharedWriteGroup::Ref
	SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  ___Ref_0;

public:
	inline static int32_t get_offset_of_Ref_0() { return static_cast<int32_t>(offsetof(SharedWriteGroup_t43861216B5BB89853E40798D5170B5574E8A46FB_StaticFields, ___Ref_0)); }
	inline SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  get_Ref_0() const { return ___Ref_0; }
	inline SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6 * get_address_of_Ref_0() { return &___Ref_0; }
	inline void set_Ref_0(SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  value)
	{
		___Ref_0 = value;
	}
};


// Unity.Entities.TypeManager_TypeCategory
struct  TypeCategory_t080465BD86AA2F85C227225B685965CDCCB1922C 
{
public:
	// System.Int32 Unity.Entities.TypeManager_TypeCategory::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(TypeCategory_t080465BD86AA2F85C227225B685965CDCCB1922C, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Unity.Entities.UnsafeArchetypePtrList
struct  UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7 
{
public:
	// Unity.Entities.Archetype** Unity.Entities.UnsafeArchetypePtrList::Ptr
	Archetype_t14B6BCF53EF0F918C8B503D7100F6217E480CD94 ** ___Ptr_0;
	// System.Int32 Unity.Entities.UnsafeArchetypePtrList::Length
	int32_t ___Length_1;
	// System.Int32 Unity.Entities.UnsafeArchetypePtrList::Capacity
	int32_t ___Capacity_2;
	// Unity.Collections.AllocatorManager_AllocatorHandle Unity.Entities.UnsafeArchetypePtrList::Allocator
	AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  ___Allocator_3;

public:
	inline static int32_t get_offset_of_Ptr_0() { return static_cast<int32_t>(offsetof(UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7, ___Ptr_0)); }
	inline Archetype_t14B6BCF53EF0F918C8B503D7100F6217E480CD94 ** get_Ptr_0() const { return ___Ptr_0; }
	inline Archetype_t14B6BCF53EF0F918C8B503D7100F6217E480CD94 *** get_address_of_Ptr_0() { return &___Ptr_0; }
	inline void set_Ptr_0(Archetype_t14B6BCF53EF0F918C8B503D7100F6217E480CD94 ** value)
	{
		___Ptr_0 = value;
	}

	inline static int32_t get_offset_of_Length_1() { return static_cast<int32_t>(offsetof(UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7, ___Length_1)); }
	inline int32_t get_Length_1() const { return ___Length_1; }
	inline int32_t* get_address_of_Length_1() { return &___Length_1; }
	inline void set_Length_1(int32_t value)
	{
		___Length_1 = value;
	}

	inline static int32_t get_offset_of_Capacity_2() { return static_cast<int32_t>(offsetof(UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7, ___Capacity_2)); }
	inline int32_t get_Capacity_2() const { return ___Capacity_2; }
	inline int32_t* get_address_of_Capacity_2() { return &___Capacity_2; }
	inline void set_Capacity_2(int32_t value)
	{
		___Capacity_2 = value;
	}

	inline static int32_t get_offset_of_Allocator_3() { return static_cast<int32_t>(offsetof(UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7, ___Allocator_3)); }
	inline AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  get_Allocator_3() const { return ___Allocator_3; }
	inline AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A * get_address_of_Allocator_3() { return &___Allocator_3; }
	inline void set_Allocator_3(AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  value)
	{
		___Allocator_3 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.UnsafeArchetypePtrList
struct UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7_marshaled_pinvoke
{
	Archetype_t14B6BCF53EF0F918C8B503D7100F6217E480CD94 ** ___Ptr_0;
	int32_t ___Length_1;
	int32_t ___Capacity_2;
	AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  ___Allocator_3;
};
// Native definition for COM marshalling of Unity.Entities.UnsafeArchetypePtrList
struct UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7_marshaled_com
{
	Archetype_t14B6BCF53EF0F918C8B503D7100F6217E480CD94 ** ___Ptr_0;
	int32_t ___Length_1;
	int32_t ___Capacity_2;
	AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  ___Allocator_3;
};

// Unity.Entities.UnsafeChunkPtrList
struct  UnsafeChunkPtrList_tF6FE39C1771CD6E6E5CA3B86195038FECD946D58 
{
public:
	// Unity.Entities.Chunk** Unity.Entities.UnsafeChunkPtrList::Ptr
	Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 ** ___Ptr_0;
	// System.Int32 Unity.Entities.UnsafeChunkPtrList::Length
	int32_t ___Length_1;
	// System.Int32 Unity.Entities.UnsafeChunkPtrList::Capacity
	int32_t ___Capacity_2;
	// Unity.Collections.AllocatorManager_AllocatorHandle Unity.Entities.UnsafeChunkPtrList::Allocator
	AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  ___Allocator_3;

public:
	inline static int32_t get_offset_of_Ptr_0() { return static_cast<int32_t>(offsetof(UnsafeChunkPtrList_tF6FE39C1771CD6E6E5CA3B86195038FECD946D58, ___Ptr_0)); }
	inline Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 ** get_Ptr_0() const { return ___Ptr_0; }
	inline Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 *** get_address_of_Ptr_0() { return &___Ptr_0; }
	inline void set_Ptr_0(Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 ** value)
	{
		___Ptr_0 = value;
	}

	inline static int32_t get_offset_of_Length_1() { return static_cast<int32_t>(offsetof(UnsafeChunkPtrList_tF6FE39C1771CD6E6E5CA3B86195038FECD946D58, ___Length_1)); }
	inline int32_t get_Length_1() const { return ___Length_1; }
	inline int32_t* get_address_of_Length_1() { return &___Length_1; }
	inline void set_Length_1(int32_t value)
	{
		___Length_1 = value;
	}

	inline static int32_t get_offset_of_Capacity_2() { return static_cast<int32_t>(offsetof(UnsafeChunkPtrList_tF6FE39C1771CD6E6E5CA3B86195038FECD946D58, ___Capacity_2)); }
	inline int32_t get_Capacity_2() const { return ___Capacity_2; }
	inline int32_t* get_address_of_Capacity_2() { return &___Capacity_2; }
	inline void set_Capacity_2(int32_t value)
	{
		___Capacity_2 = value;
	}

	inline static int32_t get_offset_of_Allocator_3() { return static_cast<int32_t>(offsetof(UnsafeChunkPtrList_tF6FE39C1771CD6E6E5CA3B86195038FECD946D58, ___Allocator_3)); }
	inline AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  get_Allocator_3() const { return ___Allocator_3; }
	inline AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A * get_address_of_Allocator_3() { return &___Allocator_3; }
	inline void set_Allocator_3(AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  value)
	{
		___Allocator_3 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.UnsafeChunkPtrList
struct UnsafeChunkPtrList_tF6FE39C1771CD6E6E5CA3B86195038FECD946D58_marshaled_pinvoke
{
	Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 ** ___Ptr_0;
	int32_t ___Length_1;
	int32_t ___Capacity_2;
	AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  ___Allocator_3;
};
// Native definition for COM marshalling of Unity.Entities.UnsafeChunkPtrList
struct UnsafeChunkPtrList_tF6FE39C1771CD6E6E5CA3B86195038FECD946D58_marshaled_com
{
	Chunk_t0E54C3DC5422B5BF5ABDACFC6746C3F37A972248 ** ___Ptr_0;
	int32_t ___Length_1;
	int32_t ___Capacity_2;
	AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  ___Allocator_3;
};

// Unity.Entities.UnsafeIntList
struct  UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B 
{
public:
	// System.Int32* Unity.Entities.UnsafeIntList::Ptr
	int32_t* ___Ptr_0;
	// System.Int32 Unity.Entities.UnsafeIntList::Length
	int32_t ___Length_1;
	// System.Int32 Unity.Entities.UnsafeIntList::Capacity
	int32_t ___Capacity_2;
	// Unity.Collections.AllocatorManager_AllocatorHandle Unity.Entities.UnsafeIntList::Allocator
	AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  ___Allocator_3;

public:
	inline static int32_t get_offset_of_Ptr_0() { return static_cast<int32_t>(offsetof(UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B, ___Ptr_0)); }
	inline int32_t* get_Ptr_0() const { return ___Ptr_0; }
	inline int32_t** get_address_of_Ptr_0() { return &___Ptr_0; }
	inline void set_Ptr_0(int32_t* value)
	{
		___Ptr_0 = value;
	}

	inline static int32_t get_offset_of_Length_1() { return static_cast<int32_t>(offsetof(UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B, ___Length_1)); }
	inline int32_t get_Length_1() const { return ___Length_1; }
	inline int32_t* get_address_of_Length_1() { return &___Length_1; }
	inline void set_Length_1(int32_t value)
	{
		___Length_1 = value;
	}

	inline static int32_t get_offset_of_Capacity_2() { return static_cast<int32_t>(offsetof(UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B, ___Capacity_2)); }
	inline int32_t get_Capacity_2() const { return ___Capacity_2; }
	inline int32_t* get_address_of_Capacity_2() { return &___Capacity_2; }
	inline void set_Capacity_2(int32_t value)
	{
		___Capacity_2 = value;
	}

	inline static int32_t get_offset_of_Allocator_3() { return static_cast<int32_t>(offsetof(UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B, ___Allocator_3)); }
	inline AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  get_Allocator_3() const { return ___Allocator_3; }
	inline AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A * get_address_of_Allocator_3() { return &___Allocator_3; }
	inline void set_Allocator_3(AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  value)
	{
		___Allocator_3 = value;
	}
};


// Unity.Entities.UnsafeUintList
struct  UnsafeUintList_t0B7D19A5A44059F75D85BFD01040619224D6220E 
{
public:
	// System.UInt32* Unity.Entities.UnsafeUintList::Ptr
	uint32_t* ___Ptr_0;
	// System.Int32 Unity.Entities.UnsafeUintList::Length
	int32_t ___Length_1;
	// System.Int32 Unity.Entities.UnsafeUintList::Capacity
	int32_t ___Capacity_2;
	// Unity.Collections.AllocatorManager_AllocatorHandle Unity.Entities.UnsafeUintList::Allocator
	AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  ___Allocator_3;

public:
	inline static int32_t get_offset_of_Ptr_0() { return static_cast<int32_t>(offsetof(UnsafeUintList_t0B7D19A5A44059F75D85BFD01040619224D6220E, ___Ptr_0)); }
	inline uint32_t* get_Ptr_0() const { return ___Ptr_0; }
	inline uint32_t** get_address_of_Ptr_0() { return &___Ptr_0; }
	inline void set_Ptr_0(uint32_t* value)
	{
		___Ptr_0 = value;
	}

	inline static int32_t get_offset_of_Length_1() { return static_cast<int32_t>(offsetof(UnsafeUintList_t0B7D19A5A44059F75D85BFD01040619224D6220E, ___Length_1)); }
	inline int32_t get_Length_1() const { return ___Length_1; }
	inline int32_t* get_address_of_Length_1() { return &___Length_1; }
	inline void set_Length_1(int32_t value)
	{
		___Length_1 = value;
	}

	inline static int32_t get_offset_of_Capacity_2() { return static_cast<int32_t>(offsetof(UnsafeUintList_t0B7D19A5A44059F75D85BFD01040619224D6220E, ___Capacity_2)); }
	inline int32_t get_Capacity_2() const { return ___Capacity_2; }
	inline int32_t* get_address_of_Capacity_2() { return &___Capacity_2; }
	inline void set_Capacity_2(int32_t value)
	{
		___Capacity_2 = value;
	}

	inline static int32_t get_offset_of_Allocator_3() { return static_cast<int32_t>(offsetof(UnsafeUintList_t0B7D19A5A44059F75D85BFD01040619224D6220E, ___Allocator_3)); }
	inline AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  get_Allocator_3() const { return ___Allocator_3; }
	inline AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A * get_address_of_Allocator_3() { return &___Allocator_3; }
	inline void set_Allocator_3(AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  value)
	{
		___Allocator_3 = value;
	}
};


// Unity.Entities.World_StateAllocLevel1
struct  StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 
{
public:
	// System.UInt64 Unity.Entities.World_StateAllocLevel1::FreeBits
	uint64_t ___FreeBits_0;
	// Unity.Entities.SystemState* Unity.Entities.World_StateAllocLevel1::States
	SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * ___States_1;
	// Unity.Entities.World_StateAllocLevel1_<Version>e__FixedBuffer Unity.Entities.World_StateAllocLevel1::Version
	U3CVersionU3Ee__FixedBuffer_tB3AA164CD6E485CFAEFC56242AE5BE61BE39446F  ___Version_2;
	// Unity.Entities.World_StateAllocLevel1_<TypeHash>e__FixedBuffer Unity.Entities.World_StateAllocLevel1::TypeHash
	U3CTypeHashU3Ee__FixedBuffer_tEE844B127F8E28AF008BEC971AE9386972B3D6D5  ___TypeHash_3;
	// Unity.Entities.World_StateAllocLevel1_<SystemPointer>e__FixedBuffer Unity.Entities.World_StateAllocLevel1::SystemPointer
	U3CSystemPointerU3Ee__FixedBuffer_t19C0D38015C844F379AB29453AE403CBDD5E16B1  ___SystemPointer_4;

public:
	inline static int32_t get_offset_of_FreeBits_0() { return static_cast<int32_t>(offsetof(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82, ___FreeBits_0)); }
	inline uint64_t get_FreeBits_0() const { return ___FreeBits_0; }
	inline uint64_t* get_address_of_FreeBits_0() { return &___FreeBits_0; }
	inline void set_FreeBits_0(uint64_t value)
	{
		___FreeBits_0 = value;
	}

	inline static int32_t get_offset_of_States_1() { return static_cast<int32_t>(offsetof(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82, ___States_1)); }
	inline SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * get_States_1() const { return ___States_1; }
	inline SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 ** get_address_of_States_1() { return &___States_1; }
	inline void set_States_1(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * value)
	{
		___States_1 = value;
	}

	inline static int32_t get_offset_of_Version_2() { return static_cast<int32_t>(offsetof(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82, ___Version_2)); }
	inline U3CVersionU3Ee__FixedBuffer_tB3AA164CD6E485CFAEFC56242AE5BE61BE39446F  get_Version_2() const { return ___Version_2; }
	inline U3CVersionU3Ee__FixedBuffer_tB3AA164CD6E485CFAEFC56242AE5BE61BE39446F * get_address_of_Version_2() { return &___Version_2; }
	inline void set_Version_2(U3CVersionU3Ee__FixedBuffer_tB3AA164CD6E485CFAEFC56242AE5BE61BE39446F  value)
	{
		___Version_2 = value;
	}

	inline static int32_t get_offset_of_TypeHash_3() { return static_cast<int32_t>(offsetof(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82, ___TypeHash_3)); }
	inline U3CTypeHashU3Ee__FixedBuffer_tEE844B127F8E28AF008BEC971AE9386972B3D6D5  get_TypeHash_3() const { return ___TypeHash_3; }
	inline U3CTypeHashU3Ee__FixedBuffer_tEE844B127F8E28AF008BEC971AE9386972B3D6D5 * get_address_of_TypeHash_3() { return &___TypeHash_3; }
	inline void set_TypeHash_3(U3CTypeHashU3Ee__FixedBuffer_tEE844B127F8E28AF008BEC971AE9386972B3D6D5  value)
	{
		___TypeHash_3 = value;
	}

	inline static int32_t get_offset_of_SystemPointer_4() { return static_cast<int32_t>(offsetof(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82, ___SystemPointer_4)); }
	inline U3CSystemPointerU3Ee__FixedBuffer_t19C0D38015C844F379AB29453AE403CBDD5E16B1  get_SystemPointer_4() const { return ___SystemPointer_4; }
	inline U3CSystemPointerU3Ee__FixedBuffer_t19C0D38015C844F379AB29453AE403CBDD5E16B1 * get_address_of_SystemPointer_4() { return &___SystemPointer_4; }
	inline void set_SystemPointer_4(U3CSystemPointerU3Ee__FixedBuffer_t19C0D38015C844F379AB29453AE403CBDD5E16B1  value)
	{
		___SystemPointer_4 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.World/StateAllocLevel1
struct StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82_marshaled_pinvoke
{
	uint64_t ___FreeBits_0;
	SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * ___States_1;
	U3CVersionU3Ee__FixedBuffer_tB3AA164CD6E485CFAEFC56242AE5BE61BE39446F  ___Version_2;
	U3CTypeHashU3Ee__FixedBuffer_tEE844B127F8E28AF008BEC971AE9386972B3D6D5  ___TypeHash_3;
	U3CSystemPointerU3Ee__FixedBuffer_t19C0D38015C844F379AB29453AE403CBDD5E16B1  ___SystemPointer_4;
};
// Native definition for COM marshalling of Unity.Entities.World/StateAllocLevel1
struct StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82_marshaled_com
{
	uint64_t ___FreeBits_0;
	SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * ___States_1;
	U3CVersionU3Ee__FixedBuffer_tB3AA164CD6E485CFAEFC56242AE5BE61BE39446F  ___Version_2;
	U3CTypeHashU3Ee__FixedBuffer_tEE844B127F8E28AF008BEC971AE9386972B3D6D5  ___TypeHash_3;
	U3CSystemPointerU3Ee__FixedBuffer_t19C0D38015C844F379AB29453AE403CBDD5E16B1  ___SystemPointer_4;
};

// Unity.Entities.WorldFlags
struct  WorldFlags_t01F95DCEF170D1964775219CCB0196C0F3254F3C 
{
public:
	// System.Byte Unity.Entities.WorldFlags::value__
	uint8_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(WorldFlags_t01F95DCEF170D1964775219CCB0196C0F3254F3C, ___value___2)); }
	inline uint8_t get_value___2() const { return ___value___2; }
	inline uint8_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(uint8_t value)
	{
		___value___2 = value;
	}
};


// Unity.Jobs.JobHandle
struct  JobHandle_t8AEB8D31C25D7774C71D62B0C662525E6E36D847 
{
public:
	// System.IntPtr Unity.Jobs.JobHandle::jobGroup
	intptr_t ___jobGroup_0;
	// System.Int32 Unity.Jobs.JobHandle::version
	int32_t ___version_1;

public:
	inline static int32_t get_offset_of_jobGroup_0() { return static_cast<int32_t>(offsetof(JobHandle_t8AEB8D31C25D7774C71D62B0C662525E6E36D847, ___jobGroup_0)); }
	inline intptr_t get_jobGroup_0() const { return ___jobGroup_0; }
	inline intptr_t* get_address_of_jobGroup_0() { return &___jobGroup_0; }
	inline void set_jobGroup_0(intptr_t value)
	{
		___jobGroup_0 = value;
	}

	inline static int32_t get_offset_of_version_1() { return static_cast<int32_t>(offsetof(JobHandle_t8AEB8D31C25D7774C71D62B0C662525E6E36D847, ___version_1)); }
	inline int32_t get_version_1() const { return ___version_1; }
	inline int32_t* get_address_of_version_1() { return &___version_1; }
	inline void set_version_1(int32_t value)
	{
		___version_1 = value;
	}
};


// Unity.Profiling.ProfilerMarker
struct  ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1 
{
public:
	// System.IntPtr Unity.Profiling.ProfilerMarker::m_Ptr
	intptr_t ___m_Ptr_0;

public:
	inline static int32_t get_offset_of_m_Ptr_0() { return static_cast<int32_t>(offsetof(ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1, ___m_Ptr_0)); }
	inline intptr_t get_m_Ptr_0() const { return ___m_Ptr_0; }
	inline intptr_t* get_address_of_m_Ptr_0() { return &___m_Ptr_0; }
	inline void set_m_Ptr_0(intptr_t value)
	{
		___m_Ptr_0 = value;
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


// Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer
struct  UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C 
{
public:
	// System.Byte* Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer::Ptr
	uint8_t* ___Ptr_0;
	// System.Int32 Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer::Length
	int32_t ___Length_1;
	// System.Int32 Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer::Capacity
	int32_t ___Capacity_2;
	// Unity.Collections.Allocator Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer::Allocator
	int32_t ___Allocator_3;
	// System.Int32 Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer::Alignment
	int32_t ___Alignment_4;

public:
	inline static int32_t get_offset_of_Ptr_0() { return static_cast<int32_t>(offsetof(UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C, ___Ptr_0)); }
	inline uint8_t* get_Ptr_0() const { return ___Ptr_0; }
	inline uint8_t** get_address_of_Ptr_0() { return &___Ptr_0; }
	inline void set_Ptr_0(uint8_t* value)
	{
		___Ptr_0 = value;
	}

	inline static int32_t get_offset_of_Length_1() { return static_cast<int32_t>(offsetof(UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C, ___Length_1)); }
	inline int32_t get_Length_1() const { return ___Length_1; }
	inline int32_t* get_address_of_Length_1() { return &___Length_1; }
	inline void set_Length_1(int32_t value)
	{
		___Length_1 = value;
	}

	inline static int32_t get_offset_of_Capacity_2() { return static_cast<int32_t>(offsetof(UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C, ___Capacity_2)); }
	inline int32_t get_Capacity_2() const { return ___Capacity_2; }
	inline int32_t* get_address_of_Capacity_2() { return &___Capacity_2; }
	inline void set_Capacity_2(int32_t value)
	{
		___Capacity_2 = value;
	}

	inline static int32_t get_offset_of_Allocator_3() { return static_cast<int32_t>(offsetof(UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C, ___Allocator_3)); }
	inline int32_t get_Allocator_3() const { return ___Allocator_3; }
	inline int32_t* get_address_of_Allocator_3() { return &___Allocator_3; }
	inline void set_Allocator_3(int32_t value)
	{
		___Allocator_3 = value;
	}

	inline static int32_t get_offset_of_Alignment_4() { return static_cast<int32_t>(offsetof(UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C, ___Alignment_4)); }
	inline int32_t get_Alignment_4() const { return ___Alignment_4; }
	inline int32_t* get_address_of_Alignment_4() { return &___Alignment_4; }
	inline void set_Alignment_4(int32_t value)
	{
		___Alignment_4 = value;
	}
};


// Unity.Collections.LowLevel.Unsafe.UnsafeHashMap`2<System.UInt64,System.Int32>
struct  UnsafeHashMap_2_t48AE5E3C55FDD98B6C4D2C6449B1EE8EA6D531F7 
{
public:
	// Unity.Collections.LowLevel.Unsafe.UnsafeHashMapData* Unity.Collections.LowLevel.Unsafe.UnsafeHashMap`2::m_Buffer
	UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 * ___m_Buffer_0;
	// Unity.Collections.Allocator Unity.Collections.LowLevel.Unsafe.UnsafeHashMap`2::m_AllocatorLabel
	int32_t ___m_AllocatorLabel_1;

public:
	inline static int32_t get_offset_of_m_Buffer_0() { return static_cast<int32_t>(offsetof(UnsafeHashMap_2_t48AE5E3C55FDD98B6C4D2C6449B1EE8EA6D531F7, ___m_Buffer_0)); }
	inline UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 * get_m_Buffer_0() const { return ___m_Buffer_0; }
	inline UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 ** get_address_of_m_Buffer_0() { return &___m_Buffer_0; }
	inline void set_m_Buffer_0(UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 * value)
	{
		___m_Buffer_0 = value;
	}

	inline static int32_t get_offset_of_m_AllocatorLabel_1() { return static_cast<int32_t>(offsetof(UnsafeHashMap_2_t48AE5E3C55FDD98B6C4D2C6449B1EE8EA6D531F7, ___m_AllocatorLabel_1)); }
	inline int32_t get_m_AllocatorLabel_1() const { return ___m_AllocatorLabel_1; }
	inline int32_t* get_address_of_m_AllocatorLabel_1() { return &___m_AllocatorLabel_1; }
	inline void set_m_AllocatorLabel_1(int32_t value)
	{
		___m_AllocatorLabel_1 = value;
	}
};


// Unity.Collections.LowLevel.Unsafe.UnsafeHashMap`2<Unity.Entities.Serialization.SerializeUtility_BlobAssetPtr,System.Int32>
struct  UnsafeHashMap_2_t337A0F0DB9C26F7409D3D9BB22F421B4C7A6893A 
{
public:
	// Unity.Collections.LowLevel.Unsafe.UnsafeHashMapData* Unity.Collections.LowLevel.Unsafe.UnsafeHashMap`2::m_Buffer
	UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 * ___m_Buffer_0;
	// Unity.Collections.Allocator Unity.Collections.LowLevel.Unsafe.UnsafeHashMap`2::m_AllocatorLabel
	int32_t ___m_AllocatorLabel_1;

public:
	inline static int32_t get_offset_of_m_Buffer_0() { return static_cast<int32_t>(offsetof(UnsafeHashMap_2_t337A0F0DB9C26F7409D3D9BB22F421B4C7A6893A, ___m_Buffer_0)); }
	inline UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 * get_m_Buffer_0() const { return ___m_Buffer_0; }
	inline UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 ** get_address_of_m_Buffer_0() { return &___m_Buffer_0; }
	inline void set_m_Buffer_0(UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 * value)
	{
		___m_Buffer_0 = value;
	}

	inline static int32_t get_offset_of_m_AllocatorLabel_1() { return static_cast<int32_t>(offsetof(UnsafeHashMap_2_t337A0F0DB9C26F7409D3D9BB22F421B4C7A6893A, ___m_AllocatorLabel_1)); }
	inline int32_t get_m_AllocatorLabel_1() const { return ___m_AllocatorLabel_1; }
	inline int32_t* get_address_of_m_AllocatorLabel_1() { return &___m_AllocatorLabel_1; }
	inline void set_m_AllocatorLabel_1(int32_t value)
	{
		___m_AllocatorLabel_1 = value;
	}
};


// Unity.Collections.LowLevel.Unsafe.UnsafeMultiHashMap`2<System.Int64,System.UInt16>
struct  UnsafeMultiHashMap_2_tFDF632D7E17D69CF95BBF036CEBC421A871BF004 
{
public:
	// Unity.Collections.LowLevel.Unsafe.UnsafeHashMapData* Unity.Collections.LowLevel.Unsafe.UnsafeMultiHashMap`2::m_Buffer
	UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 * ___m_Buffer_0;
	// Unity.Collections.Allocator Unity.Collections.LowLevel.Unsafe.UnsafeMultiHashMap`2::m_AllocatorLabel
	int32_t ___m_AllocatorLabel_1;

public:
	inline static int32_t get_offset_of_m_Buffer_0() { return static_cast<int32_t>(offsetof(UnsafeMultiHashMap_2_tFDF632D7E17D69CF95BBF036CEBC421A871BF004, ___m_Buffer_0)); }
	inline UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 * get_m_Buffer_0() const { return ___m_Buffer_0; }
	inline UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 ** get_address_of_m_Buffer_0() { return &___m_Buffer_0; }
	inline void set_m_Buffer_0(UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 * value)
	{
		___m_Buffer_0 = value;
	}

	inline static int32_t get_offset_of_m_AllocatorLabel_1() { return static_cast<int32_t>(offsetof(UnsafeMultiHashMap_2_tFDF632D7E17D69CF95BBF036CEBC421A871BF004, ___m_AllocatorLabel_1)); }
	inline int32_t get_m_AllocatorLabel_1() const { return ___m_AllocatorLabel_1; }
	inline int32_t* get_address_of_m_AllocatorLabel_1() { return &___m_AllocatorLabel_1; }
	inline void set_m_AllocatorLabel_1(int32_t value)
	{
		___m_AllocatorLabel_1 = value;
	}
};


// Unity.Collections.LowLevel.Unsafe.UnsafeMultiHashMap`2<Unity.Entities.EntityPatcher_EntityComponentPair,Unity.Entities.EntityPatcher_OffsetEntityPair>
struct  UnsafeMultiHashMap_2_t1516AF586287D8D01621118A3D63CC75A85B1A2F 
{
public:
	// Unity.Collections.LowLevel.Unsafe.UnsafeHashMapData* Unity.Collections.LowLevel.Unsafe.UnsafeMultiHashMap`2::m_Buffer
	UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 * ___m_Buffer_0;
	// Unity.Collections.Allocator Unity.Collections.LowLevel.Unsafe.UnsafeMultiHashMap`2::m_AllocatorLabel
	int32_t ___m_AllocatorLabel_1;

public:
	inline static int32_t get_offset_of_m_Buffer_0() { return static_cast<int32_t>(offsetof(UnsafeMultiHashMap_2_t1516AF586287D8D01621118A3D63CC75A85B1A2F, ___m_Buffer_0)); }
	inline UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 * get_m_Buffer_0() const { return ___m_Buffer_0; }
	inline UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 ** get_address_of_m_Buffer_0() { return &___m_Buffer_0; }
	inline void set_m_Buffer_0(UnsafeHashMapData_tBE4CCB191438EEFAF585DE1AD96D342825E64C82 * value)
	{
		___m_Buffer_0 = value;
	}

	inline static int32_t get_offset_of_m_AllocatorLabel_1() { return static_cast<int32_t>(offsetof(UnsafeMultiHashMap_2_t1516AF586287D8D01621118A3D63CC75A85B1A2F, ___m_AllocatorLabel_1)); }
	inline int32_t get_m_AllocatorLabel_1() const { return ___m_AllocatorLabel_1; }
	inline int32_t* get_address_of_m_AllocatorLabel_1() { return &___m_AllocatorLabel_1; }
	inline void set_m_AllocatorLabel_1(int32_t value)
	{
		___m_AllocatorLabel_1 = value;
	}
};


// Unity.Collections.NativeArray`1<System.Int32>
struct  NativeArray_1_tD60079F06B473C85CF6C2BB4F2D203F38191AE99 
{
public:
	// System.Void* Unity.Collections.NativeArray`1::m_Buffer
	void* ___m_Buffer_0;
	// System.Int32 Unity.Collections.NativeArray`1::m_Length
	int32_t ___m_Length_1;
	// Unity.Collections.Allocator Unity.Collections.NativeArray`1::m_AllocatorLabel
	int32_t ___m_AllocatorLabel_2;

public:
	inline static int32_t get_offset_of_m_Buffer_0() { return static_cast<int32_t>(offsetof(NativeArray_1_tD60079F06B473C85CF6C2BB4F2D203F38191AE99, ___m_Buffer_0)); }
	inline void* get_m_Buffer_0() const { return ___m_Buffer_0; }
	inline void** get_address_of_m_Buffer_0() { return &___m_Buffer_0; }
	inline void set_m_Buffer_0(void* value)
	{
		___m_Buffer_0 = value;
	}

	inline static int32_t get_offset_of_m_Length_1() { return static_cast<int32_t>(offsetof(NativeArray_1_tD60079F06B473C85CF6C2BB4F2D203F38191AE99, ___m_Length_1)); }
	inline int32_t get_m_Length_1() const { return ___m_Length_1; }
	inline int32_t* get_address_of_m_Length_1() { return &___m_Length_1; }
	inline void set_m_Length_1(int32_t value)
	{
		___m_Length_1 = value;
	}

	inline static int32_t get_offset_of_m_AllocatorLabel_2() { return static_cast<int32_t>(offsetof(NativeArray_1_tD60079F06B473C85CF6C2BB4F2D203F38191AE99, ___m_AllocatorLabel_2)); }
	inline int32_t get_m_AllocatorLabel_2() const { return ___m_AllocatorLabel_2; }
	inline int32_t* get_address_of_m_AllocatorLabel_2() { return &___m_AllocatorLabel_2; }
	inline void set_m_AllocatorLabel_2(int32_t value)
	{
		___m_AllocatorLabel_2 = value;
	}
};


// Unity.Collections.NativeArray`1<Unity.Entities.Entity>
struct  NativeArray_1_t587B0E555A435D1A1EACD16A8F3C3EBCF3497F5E 
{
public:
	// System.Void* Unity.Collections.NativeArray`1::m_Buffer
	void* ___m_Buffer_0;
	// System.Int32 Unity.Collections.NativeArray`1::m_Length
	int32_t ___m_Length_1;
	// Unity.Collections.Allocator Unity.Collections.NativeArray`1::m_AllocatorLabel
	int32_t ___m_AllocatorLabel_2;

public:
	inline static int32_t get_offset_of_m_Buffer_0() { return static_cast<int32_t>(offsetof(NativeArray_1_t587B0E555A435D1A1EACD16A8F3C3EBCF3497F5E, ___m_Buffer_0)); }
	inline void* get_m_Buffer_0() const { return ___m_Buffer_0; }
	inline void** get_address_of_m_Buffer_0() { return &___m_Buffer_0; }
	inline void set_m_Buffer_0(void* value)
	{
		___m_Buffer_0 = value;
	}

	inline static int32_t get_offset_of_m_Length_1() { return static_cast<int32_t>(offsetof(NativeArray_1_t587B0E555A435D1A1EACD16A8F3C3EBCF3497F5E, ___m_Length_1)); }
	inline int32_t get_m_Length_1() const { return ___m_Length_1; }
	inline int32_t* get_address_of_m_Length_1() { return &___m_Length_1; }
	inline void set_m_Length_1(int32_t value)
	{
		___m_Length_1 = value;
	}

	inline static int32_t get_offset_of_m_AllocatorLabel_2() { return static_cast<int32_t>(offsetof(NativeArray_1_t587B0E555A435D1A1EACD16A8F3C3EBCF3497F5E, ___m_AllocatorLabel_2)); }
	inline int32_t get_m_AllocatorLabel_2() const { return ___m_AllocatorLabel_2; }
	inline int32_t* get_address_of_m_AllocatorLabel_2() { return &___m_AllocatorLabel_2; }
	inline void set_m_AllocatorLabel_2(int32_t value)
	{
		___m_AllocatorLabel_2 = value;
	}
};


// Unity.Collections.NativeArray`1<Unity.Entities.TypeManager_TypeInfo>
struct  NativeArray_1_t663B008ABD2F6A4E7DCC3EC82886219F3AB8D5EA 
{
public:
	// System.Void* Unity.Collections.NativeArray`1::m_Buffer
	void* ___m_Buffer_0;
	// System.Int32 Unity.Collections.NativeArray`1::m_Length
	int32_t ___m_Length_1;
	// Unity.Collections.Allocator Unity.Collections.NativeArray`1::m_AllocatorLabel
	int32_t ___m_AllocatorLabel_2;

public:
	inline static int32_t get_offset_of_m_Buffer_0() { return static_cast<int32_t>(offsetof(NativeArray_1_t663B008ABD2F6A4E7DCC3EC82886219F3AB8D5EA, ___m_Buffer_0)); }
	inline void* get_m_Buffer_0() const { return ___m_Buffer_0; }
	inline void** get_address_of_m_Buffer_0() { return &___m_Buffer_0; }
	inline void set_m_Buffer_0(void* value)
	{
		___m_Buffer_0 = value;
	}

	inline static int32_t get_offset_of_m_Length_1() { return static_cast<int32_t>(offsetof(NativeArray_1_t663B008ABD2F6A4E7DCC3EC82886219F3AB8D5EA, ___m_Length_1)); }
	inline int32_t get_m_Length_1() const { return ___m_Length_1; }
	inline int32_t* get_address_of_m_Length_1() { return &___m_Length_1; }
	inline void set_m_Length_1(int32_t value)
	{
		___m_Length_1 = value;
	}

	inline static int32_t get_offset_of_m_AllocatorLabel_2() { return static_cast<int32_t>(offsetof(NativeArray_1_t663B008ABD2F6A4E7DCC3EC82886219F3AB8D5EA, ___m_AllocatorLabel_2)); }
	inline int32_t get_m_AllocatorLabel_2() const { return ___m_AllocatorLabel_2; }
	inline int32_t* get_address_of_m_AllocatorLabel_2() { return &___m_AllocatorLabel_2; }
	inline void set_m_AllocatorLabel_2(int32_t value)
	{
		___m_AllocatorLabel_2 = value;
	}
};


// Unity.Collections.NativeList`1<System.Boolean>
struct  NativeList_1_t3933DBBB090E35386DA7B0DEFB0D5FAE42A3B8CC 
{
public:
	// Unity.Collections.LowLevel.Unsafe.UnsafeList* Unity.Collections.NativeList`1::m_ListData
	UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * ___m_ListData_0;
	// Unity.Collections.Allocator Unity.Collections.NativeList`1::m_DeprecatedAllocator
	int32_t ___m_DeprecatedAllocator_1;

public:
	inline static int32_t get_offset_of_m_ListData_0() { return static_cast<int32_t>(offsetof(NativeList_1_t3933DBBB090E35386DA7B0DEFB0D5FAE42A3B8CC, ___m_ListData_0)); }
	inline UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * get_m_ListData_0() const { return ___m_ListData_0; }
	inline UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA ** get_address_of_m_ListData_0() { return &___m_ListData_0; }
	inline void set_m_ListData_0(UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * value)
	{
		___m_ListData_0 = value;
	}

	inline static int32_t get_offset_of_m_DeprecatedAllocator_1() { return static_cast<int32_t>(offsetof(NativeList_1_t3933DBBB090E35386DA7B0DEFB0D5FAE42A3B8CC, ___m_DeprecatedAllocator_1)); }
	inline int32_t get_m_DeprecatedAllocator_1() const { return ___m_DeprecatedAllocator_1; }
	inline int32_t* get_address_of_m_DeprecatedAllocator_1() { return &___m_DeprecatedAllocator_1; }
	inline void set_m_DeprecatedAllocator_1(int32_t value)
	{
		___m_DeprecatedAllocator_1 = value;
	}
};


// Unity.Collections.NativeList`1<System.Int32>
struct  NativeList_1_t37924D67F0E4D83DE506A261B7A05AE619630C80 
{
public:
	// Unity.Collections.LowLevel.Unsafe.UnsafeList* Unity.Collections.NativeList`1::m_ListData
	UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * ___m_ListData_0;
	// Unity.Collections.Allocator Unity.Collections.NativeList`1::m_DeprecatedAllocator
	int32_t ___m_DeprecatedAllocator_1;

public:
	inline static int32_t get_offset_of_m_ListData_0() { return static_cast<int32_t>(offsetof(NativeList_1_t37924D67F0E4D83DE506A261B7A05AE619630C80, ___m_ListData_0)); }
	inline UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * get_m_ListData_0() const { return ___m_ListData_0; }
	inline UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA ** get_address_of_m_ListData_0() { return &___m_ListData_0; }
	inline void set_m_ListData_0(UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * value)
	{
		___m_ListData_0 = value;
	}

	inline static int32_t get_offset_of_m_DeprecatedAllocator_1() { return static_cast<int32_t>(offsetof(NativeList_1_t37924D67F0E4D83DE506A261B7A05AE619630C80, ___m_DeprecatedAllocator_1)); }
	inline int32_t get_m_DeprecatedAllocator_1() const { return ___m_DeprecatedAllocator_1; }
	inline int32_t* get_address_of_m_DeprecatedAllocator_1() { return &___m_DeprecatedAllocator_1; }
	inline void set_m_DeprecatedAllocator_1(int32_t value)
	{
		___m_DeprecatedAllocator_1 = value;
	}
};


// Unity.Collections.NativeList`1<Unity.Entities.TypeManager_EntityOffsetInfo>
struct  NativeList_1_t1DA88E55F30826046A76336D9F8D3AA26A9377FF 
{
public:
	// Unity.Collections.LowLevel.Unsafe.UnsafeList* Unity.Collections.NativeList`1::m_ListData
	UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * ___m_ListData_0;
	// Unity.Collections.Allocator Unity.Collections.NativeList`1::m_DeprecatedAllocator
	int32_t ___m_DeprecatedAllocator_1;

public:
	inline static int32_t get_offset_of_m_ListData_0() { return static_cast<int32_t>(offsetof(NativeList_1_t1DA88E55F30826046A76336D9F8D3AA26A9377FF, ___m_ListData_0)); }
	inline UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * get_m_ListData_0() const { return ___m_ListData_0; }
	inline UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA ** get_address_of_m_ListData_0() { return &___m_ListData_0; }
	inline void set_m_ListData_0(UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * value)
	{
		___m_ListData_0 = value;
	}

	inline static int32_t get_offset_of_m_DeprecatedAllocator_1() { return static_cast<int32_t>(offsetof(NativeList_1_t1DA88E55F30826046A76336D9F8D3AA26A9377FF, ___m_DeprecatedAllocator_1)); }
	inline int32_t get_m_DeprecatedAllocator_1() const { return ___m_DeprecatedAllocator_1; }
	inline int32_t* get_address_of_m_DeprecatedAllocator_1() { return &___m_DeprecatedAllocator_1; }
	inline void set_m_DeprecatedAllocator_1(int32_t value)
	{
		___m_DeprecatedAllocator_1 = value;
	}
};


// Unity.Entities.ArchetypeListMap
struct  ArchetypeListMap_tB86167946EED7570CC5D89081659CE9A7B6A0847 
{
public:
	// Unity.Entities.UnsafeUintList Unity.Entities.ArchetypeListMap::hashes
	UnsafeUintList_t0B7D19A5A44059F75D85BFD01040619224D6220E  ___hashes_2;
	// Unity.Entities.UnsafeArchetypePtrList Unity.Entities.ArchetypeListMap::archetypes
	UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7  ___archetypes_3;
	// System.Int32 Unity.Entities.ArchetypeListMap::emptyNodes
	int32_t ___emptyNodes_4;
	// System.Int32 Unity.Entities.ArchetypeListMap::skipNodes
	int32_t ___skipNodes_5;

public:
	inline static int32_t get_offset_of_hashes_2() { return static_cast<int32_t>(offsetof(ArchetypeListMap_tB86167946EED7570CC5D89081659CE9A7B6A0847, ___hashes_2)); }
	inline UnsafeUintList_t0B7D19A5A44059F75D85BFD01040619224D6220E  get_hashes_2() const { return ___hashes_2; }
	inline UnsafeUintList_t0B7D19A5A44059F75D85BFD01040619224D6220E * get_address_of_hashes_2() { return &___hashes_2; }
	inline void set_hashes_2(UnsafeUintList_t0B7D19A5A44059F75D85BFD01040619224D6220E  value)
	{
		___hashes_2 = value;
	}

	inline static int32_t get_offset_of_archetypes_3() { return static_cast<int32_t>(offsetof(ArchetypeListMap_tB86167946EED7570CC5D89081659CE9A7B6A0847, ___archetypes_3)); }
	inline UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7  get_archetypes_3() const { return ___archetypes_3; }
	inline UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7 * get_address_of_archetypes_3() { return &___archetypes_3; }
	inline void set_archetypes_3(UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7  value)
	{
		___archetypes_3 = value;
	}

	inline static int32_t get_offset_of_emptyNodes_4() { return static_cast<int32_t>(offsetof(ArchetypeListMap_tB86167946EED7570CC5D89081659CE9A7B6A0847, ___emptyNodes_4)); }
	inline int32_t get_emptyNodes_4() const { return ___emptyNodes_4; }
	inline int32_t* get_address_of_emptyNodes_4() { return &___emptyNodes_4; }
	inline void set_emptyNodes_4(int32_t value)
	{
		___emptyNodes_4 = value;
	}

	inline static int32_t get_offset_of_skipNodes_5() { return static_cast<int32_t>(offsetof(ArchetypeListMap_tB86167946EED7570CC5D89081659CE9A7B6A0847, ___skipNodes_5)); }
	inline int32_t get_skipNodes_5() const { return ___skipNodes_5; }
	inline int32_t* get_address_of_skipNodes_5() { return &___skipNodes_5; }
	inline void set_skipNodes_5(int32_t value)
	{
		___skipNodes_5 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.ArchetypeListMap
struct ArchetypeListMap_tB86167946EED7570CC5D89081659CE9A7B6A0847_marshaled_pinvoke
{
	UnsafeUintList_t0B7D19A5A44059F75D85BFD01040619224D6220E  ___hashes_2;
	UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7_marshaled_pinvoke ___archetypes_3;
	int32_t ___emptyNodes_4;
	int32_t ___skipNodes_5;
};
// Native definition for COM marshalling of Unity.Entities.ArchetypeListMap
struct ArchetypeListMap_tB86167946EED7570CC5D89081659CE9A7B6A0847_marshaled_com
{
	UnsafeUintList_t0B7D19A5A44059F75D85BFD01040619224D6220E  ___hashes_2;
	UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7_marshaled_com ___archetypes_3;
	int32_t ___emptyNodes_4;
	int32_t ___skipNodes_5;
};

// Unity.Entities.BlobAssetHeader
struct  BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F 
{
public:
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					// System.Void* Unity.Entities.BlobAssetHeader::ValidationPtr
					void* ___ValidationPtr_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					void* ___ValidationPtr_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___Length_1_OffsetPadding[8];
					// System.Int32 Unity.Entities.BlobAssetHeader::Length
					int32_t ___Length_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___Length_1_OffsetPadding_forAlignmentOnly[8];
					int32_t ___Length_1_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___Allocator_2_OffsetPadding[12];
					// Unity.Collections.Allocator Unity.Entities.BlobAssetHeader::Allocator
					int32_t ___Allocator_2;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___Allocator_2_OffsetPadding_forAlignmentOnly[12];
					int32_t ___Allocator_2_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___Hash_3_OffsetPadding[16];
					// System.UInt64 Unity.Entities.BlobAssetHeader::Hash
					uint64_t ___Hash_3;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___Hash_3_OffsetPadding_forAlignmentOnly[16];
					uint64_t ___Hash_3_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___Padding_4_OffsetPadding[24];
					// System.UInt64 Unity.Entities.BlobAssetHeader::Padding
					uint64_t ___Padding_4;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___Padding_4_OffsetPadding_forAlignmentOnly[24];
					uint64_t ___Padding_4_forAlignmentOnly;
				};
			};
		};
		uint8_t BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F__padding[32];
	};

public:
	inline static int32_t get_offset_of_ValidationPtr_0() { return static_cast<int32_t>(offsetof(BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F, ___ValidationPtr_0)); }
	inline void* get_ValidationPtr_0() const { return ___ValidationPtr_0; }
	inline void** get_address_of_ValidationPtr_0() { return &___ValidationPtr_0; }
	inline void set_ValidationPtr_0(void* value)
	{
		___ValidationPtr_0 = value;
	}

	inline static int32_t get_offset_of_Length_1() { return static_cast<int32_t>(offsetof(BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F, ___Length_1)); }
	inline int32_t get_Length_1() const { return ___Length_1; }
	inline int32_t* get_address_of_Length_1() { return &___Length_1; }
	inline void set_Length_1(int32_t value)
	{
		___Length_1 = value;
	}

	inline static int32_t get_offset_of_Allocator_2() { return static_cast<int32_t>(offsetof(BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F, ___Allocator_2)); }
	inline int32_t get_Allocator_2() const { return ___Allocator_2; }
	inline int32_t* get_address_of_Allocator_2() { return &___Allocator_2; }
	inline void set_Allocator_2(int32_t value)
	{
		___Allocator_2 = value;
	}

	inline static int32_t get_offset_of_Hash_3() { return static_cast<int32_t>(offsetof(BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F, ___Hash_3)); }
	inline uint64_t get_Hash_3() const { return ___Hash_3; }
	inline uint64_t* get_address_of_Hash_3() { return &___Hash_3; }
	inline void set_Hash_3(uint64_t value)
	{
		___Hash_3 = value;
	}

	inline static int32_t get_offset_of_Padding_4() { return static_cast<int32_t>(offsetof(BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F, ___Padding_4)); }
	inline uint64_t get_Padding_4() const { return ___Padding_4; }
	inline uint64_t* get_address_of_Padding_4() { return &___Padding_4; }
	inline void set_Padding_4(uint64_t value)
	{
		___Padding_4 = value;
	}
};


// Unity.Entities.BlockAllocator
struct  BlockAllocator_t43F1EF54B5CF45084293051ACE1D0AD8F18DAB07 
{
public:
	// Unity.Collections.LowLevel.Unsafe.UnsafePtrList Unity.Entities.BlockAllocator::m_blocks
	UnsafePtrList_tB7C4F6B8D4EFEF738EDE139C9C61C737865AF579  ___m_blocks_0;
	// Unity.Entities.UnsafeIntList Unity.Entities.BlockAllocator::m_allocations
	UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B  ___m_allocations_1;
	// Unity.Collections.AllocatorManager_AllocatorHandle Unity.Entities.BlockAllocator::m_handle
	AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  ___m_handle_2;
	// System.Int32 Unity.Entities.BlockAllocator::m_nextByteOffset
	int32_t ___m_nextByteOffset_3;

public:
	inline static int32_t get_offset_of_m_blocks_0() { return static_cast<int32_t>(offsetof(BlockAllocator_t43F1EF54B5CF45084293051ACE1D0AD8F18DAB07, ___m_blocks_0)); }
	inline UnsafePtrList_tB7C4F6B8D4EFEF738EDE139C9C61C737865AF579  get_m_blocks_0() const { return ___m_blocks_0; }
	inline UnsafePtrList_tB7C4F6B8D4EFEF738EDE139C9C61C737865AF579 * get_address_of_m_blocks_0() { return &___m_blocks_0; }
	inline void set_m_blocks_0(UnsafePtrList_tB7C4F6B8D4EFEF738EDE139C9C61C737865AF579  value)
	{
		___m_blocks_0 = value;
	}

	inline static int32_t get_offset_of_m_allocations_1() { return static_cast<int32_t>(offsetof(BlockAllocator_t43F1EF54B5CF45084293051ACE1D0AD8F18DAB07, ___m_allocations_1)); }
	inline UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B  get_m_allocations_1() const { return ___m_allocations_1; }
	inline UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B * get_address_of_m_allocations_1() { return &___m_allocations_1; }
	inline void set_m_allocations_1(UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B  value)
	{
		___m_allocations_1 = value;
	}

	inline static int32_t get_offset_of_m_handle_2() { return static_cast<int32_t>(offsetof(BlockAllocator_t43F1EF54B5CF45084293051ACE1D0AD8F18DAB07, ___m_handle_2)); }
	inline AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  get_m_handle_2() const { return ___m_handle_2; }
	inline AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A * get_address_of_m_handle_2() { return &___m_handle_2; }
	inline void set_m_handle_2(AllocatorHandle_tAFA82A7B19AC002D983535C10C63DE0AD2EE3F1A  value)
	{
		___m_handle_2 = value;
	}

	inline static int32_t get_offset_of_m_nextByteOffset_3() { return static_cast<int32_t>(offsetof(BlockAllocator_t43F1EF54B5CF45084293051ACE1D0AD8F18DAB07, ___m_nextByteOffset_3)); }
	inline int32_t get_m_nextByteOffset_3() const { return ___m_nextByteOffset_3; }
	inline int32_t* get_address_of_m_nextByteOffset_3() { return &___m_nextByteOffset_3; }
	inline void set_m_nextByteOffset_3(int32_t value)
	{
		___m_nextByteOffset_3 = value;
	}
};


// Unity.Entities.ComponentType
struct  ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370 
{
public:
	// System.Int32 Unity.Entities.ComponentType::TypeIndex
	int32_t ___TypeIndex_0;
	// Unity.Entities.ComponentType_AccessMode Unity.Entities.ComponentType::AccessModeType
	int32_t ___AccessModeType_1;

public:
	inline static int32_t get_offset_of_TypeIndex_0() { return static_cast<int32_t>(offsetof(ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370, ___TypeIndex_0)); }
	inline int32_t get_TypeIndex_0() const { return ___TypeIndex_0; }
	inline int32_t* get_address_of_TypeIndex_0() { return &___TypeIndex_0; }
	inline void set_TypeIndex_0(int32_t value)
	{
		___TypeIndex_0 = value;
	}

	inline static int32_t get_offset_of_AccessModeType_1() { return static_cast<int32_t>(offsetof(ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370, ___AccessModeType_1)); }
	inline int32_t get_AccessModeType_1() const { return ___AccessModeType_1; }
	inline int32_t* get_address_of_AccessModeType_1() { return &___AccessModeType_1; }
	inline void set_AccessModeType_1(int32_t value)
	{
		___AccessModeType_1 = value;
	}
};


// Unity.Entities.EntityManager
struct  EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0 
{
public:
	// Unity.Entities.EntityDataAccess* Unity.Entities.EntityManager::m_EntityDataAccess
	EntityDataAccess_t14BF0F7A8F7622E7E73B5E3C0D52313B1F8D73D2 * ___m_EntityDataAccess_0;

public:
	inline static int32_t get_offset_of_m_EntityDataAccess_0() { return static_cast<int32_t>(offsetof(EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0, ___m_EntityDataAccess_0)); }
	inline EntityDataAccess_t14BF0F7A8F7622E7E73B5E3C0D52313B1F8D73D2 * get_m_EntityDataAccess_0() const { return ___m_EntityDataAccess_0; }
	inline EntityDataAccess_t14BF0F7A8F7622E7E73B5E3C0D52313B1F8D73D2 ** get_address_of_m_EntityDataAccess_0() { return &___m_EntityDataAccess_0; }
	inline void set_m_EntityDataAccess_0(EntityDataAccess_t14BF0F7A8F7622E7E73B5E3C0D52313B1F8D73D2 * value)
	{
		___m_EntityDataAccess_0 = value;
	}
};

struct EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_StaticFields
{
public:
	// Unity.Profiling.ProfilerMarker Unity.Entities.EntityManager::k_ProfileMoveSharedComponents
	ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  ___k_ProfileMoveSharedComponents_1;
	// Unity.Profiling.ProfilerMarker Unity.Entities.EntityManager::k_ProfileMoveManagedComponents
	ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  ___k_ProfileMoveManagedComponents_2;

public:
	inline static int32_t get_offset_of_k_ProfileMoveSharedComponents_1() { return static_cast<int32_t>(offsetof(EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_StaticFields, ___k_ProfileMoveSharedComponents_1)); }
	inline ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  get_k_ProfileMoveSharedComponents_1() const { return ___k_ProfileMoveSharedComponents_1; }
	inline ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1 * get_address_of_k_ProfileMoveSharedComponents_1() { return &___k_ProfileMoveSharedComponents_1; }
	inline void set_k_ProfileMoveSharedComponents_1(ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  value)
	{
		___k_ProfileMoveSharedComponents_1 = value;
	}

	inline static int32_t get_offset_of_k_ProfileMoveManagedComponents_2() { return static_cast<int32_t>(offsetof(EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_StaticFields, ___k_ProfileMoveManagedComponents_2)); }
	inline ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  get_k_ProfileMoveManagedComponents_2() const { return ___k_ProfileMoveManagedComponents_2; }
	inline ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1 * get_address_of_k_ProfileMoveManagedComponents_2() { return &___k_ProfileMoveManagedComponents_2; }
	inline void set_k_ProfileMoveManagedComponents_2(ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  value)
	{
		___k_ProfileMoveManagedComponents_2 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.EntityManager
struct EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_marshaled_pinvoke
{
	EntityDataAccess_t14BF0F7A8F7622E7E73B5E3C0D52313B1F8D73D2 * ___m_EntityDataAccess_0;
};
// Native definition for COM marshalling of Unity.Entities.EntityManager
struct EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_marshaled_com
{
	EntityDataAccess_t14BF0F7A8F7622E7E73B5E3C0D52313B1F8D73D2 * ___m_EntityDataAccess_0;
};

// Unity.Entities.JobComponentSystem
struct  JobComponentSystem_t6E53C63CFD72D32106593148904568D2182B9B9B  : public ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC
{
public:
	// Unity.Jobs.JobHandle Unity.Entities.JobComponentSystem::m_PreviousFrameDependency
	JobHandle_t8AEB8D31C25D7774C71D62B0C662525E6E36D847  ___m_PreviousFrameDependency_1;
	// System.Boolean Unity.Entities.JobComponentSystem::m_AlwaysSynchronizeSystem
	bool ___m_AlwaysSynchronizeSystem_2;

public:
	inline static int32_t get_offset_of_m_PreviousFrameDependency_1() { return static_cast<int32_t>(offsetof(JobComponentSystem_t6E53C63CFD72D32106593148904568D2182B9B9B, ___m_PreviousFrameDependency_1)); }
	inline JobHandle_t8AEB8D31C25D7774C71D62B0C662525E6E36D847  get_m_PreviousFrameDependency_1() const { return ___m_PreviousFrameDependency_1; }
	inline JobHandle_t8AEB8D31C25D7774C71D62B0C662525E6E36D847 * get_address_of_m_PreviousFrameDependency_1() { return &___m_PreviousFrameDependency_1; }
	inline void set_m_PreviousFrameDependency_1(JobHandle_t8AEB8D31C25D7774C71D62B0C662525E6E36D847  value)
	{
		___m_PreviousFrameDependency_1 = value;
	}

	inline static int32_t get_offset_of_m_AlwaysSynchronizeSystem_2() { return static_cast<int32_t>(offsetof(JobComponentSystem_t6E53C63CFD72D32106593148904568D2182B9B9B, ___m_AlwaysSynchronizeSystem_2)); }
	inline bool get_m_AlwaysSynchronizeSystem_2() const { return ___m_AlwaysSynchronizeSystem_2; }
	inline bool* get_address_of_m_AlwaysSynchronizeSystem_2() { return &___m_AlwaysSynchronizeSystem_2; }
	inline void set_m_AlwaysSynchronizeSystem_2(bool value)
	{
		___m_AlwaysSynchronizeSystem_2 = value;
	}
};


// Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1_LambdaParameterValueProviders
struct  LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF 
{
public:
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_Entity Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1_LambdaParameterValueProviders::forParameter_e
	LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  ___forParameter_e_0;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1<Unity.Entities.RetainBlobAssets> Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1_LambdaParameterValueProviders::forParameter_retain
	LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  ___forParameter_retain_1;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1<Unity.Entities.RetainBlobAssetBatchPtr> Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1_LambdaParameterValueProviders::forParameter_retainPtr
	LambdaParameterValueProvider_IComponentData_1_t1D786064C86636813BA4C8371E0CF96EAC3BAB81  ___forParameter_retainPtr_2;

public:
	inline static int32_t get_offset_of_forParameter_e_0() { return static_cast<int32_t>(offsetof(LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF, ___forParameter_e_0)); }
	inline LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  get_forParameter_e_0() const { return ___forParameter_e_0; }
	inline LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13 * get_address_of_forParameter_e_0() { return &___forParameter_e_0; }
	inline void set_forParameter_e_0(LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  value)
	{
		___forParameter_e_0 = value;
	}

	inline static int32_t get_offset_of_forParameter_retain_1() { return static_cast<int32_t>(offsetof(LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF, ___forParameter_retain_1)); }
	inline LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  get_forParameter_retain_1() const { return ___forParameter_retain_1; }
	inline LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4 * get_address_of_forParameter_retain_1() { return &___forParameter_retain_1; }
	inline void set_forParameter_retain_1(LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  value)
	{
		___forParameter_retain_1 = value;
	}

	inline static int32_t get_offset_of_forParameter_retainPtr_2() { return static_cast<int32_t>(offsetof(LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF, ___forParameter_retainPtr_2)); }
	inline LambdaParameterValueProvider_IComponentData_1_t1D786064C86636813BA4C8371E0CF96EAC3BAB81  get_forParameter_retainPtr_2() const { return ___forParameter_retainPtr_2; }
	inline LambdaParameterValueProvider_IComponentData_1_t1D786064C86636813BA4C8371E0CF96EAC3BAB81 * get_address_of_forParameter_retainPtr_2() { return &___forParameter_retainPtr_2; }
	inline void set_forParameter_retainPtr_2(LambdaParameterValueProvider_IComponentData_1_t1D786064C86636813BA4C8371E0CF96EAC3BAB81  value)
	{
		___forParameter_retainPtr_2 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1/LambdaParameterValueProviders
struct LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF_marshaled_pinvoke
{
	LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  ___forParameter_e_0;
	LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  ___forParameter_retain_1;
	LambdaParameterValueProvider_IComponentData_1_t1D786064C86636813BA4C8371E0CF96EAC3BAB81  ___forParameter_retainPtr_2;
};
// Native definition for COM marshalling of Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1/LambdaParameterValueProviders
struct LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF_marshaled_com
{
	LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  ___forParameter_e_0;
	LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  ___forParameter_retain_1;
	LambdaParameterValueProvider_IComponentData_1_t1D786064C86636813BA4C8371E0CF96EAC3BAB81  ___forParameter_retainPtr_2;
};

// Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2_LambdaParameterValueProviders
struct  LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31 
{
public:
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_Entity Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2_LambdaParameterValueProviders::forParameter_e
	LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  ___forParameter_e_0;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1<Unity.Entities.RetainBlobAssets> Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2_LambdaParameterValueProviders::forParameter_retain
	LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  ___forParameter_retain_1;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1<Unity.Entities.RetainBlobAssetPtr> Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2_LambdaParameterValueProviders::forParameter_retainPtr
	LambdaParameterValueProvider_IComponentData_1_t2EFFCFAFE87229878D536B5C44DC62239C46601A  ___forParameter_retainPtr_2;

public:
	inline static int32_t get_offset_of_forParameter_e_0() { return static_cast<int32_t>(offsetof(LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31, ___forParameter_e_0)); }
	inline LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  get_forParameter_e_0() const { return ___forParameter_e_0; }
	inline LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13 * get_address_of_forParameter_e_0() { return &___forParameter_e_0; }
	inline void set_forParameter_e_0(LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  value)
	{
		___forParameter_e_0 = value;
	}

	inline static int32_t get_offset_of_forParameter_retain_1() { return static_cast<int32_t>(offsetof(LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31, ___forParameter_retain_1)); }
	inline LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  get_forParameter_retain_1() const { return ___forParameter_retain_1; }
	inline LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4 * get_address_of_forParameter_retain_1() { return &___forParameter_retain_1; }
	inline void set_forParameter_retain_1(LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  value)
	{
		___forParameter_retain_1 = value;
	}

	inline static int32_t get_offset_of_forParameter_retainPtr_2() { return static_cast<int32_t>(offsetof(LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31, ___forParameter_retainPtr_2)); }
	inline LambdaParameterValueProvider_IComponentData_1_t2EFFCFAFE87229878D536B5C44DC62239C46601A  get_forParameter_retainPtr_2() const { return ___forParameter_retainPtr_2; }
	inline LambdaParameterValueProvider_IComponentData_1_t2EFFCFAFE87229878D536B5C44DC62239C46601A * get_address_of_forParameter_retainPtr_2() { return &___forParameter_retainPtr_2; }
	inline void set_forParameter_retainPtr_2(LambdaParameterValueProvider_IComponentData_1_t2EFFCFAFE87229878D536B5C44DC62239C46601A  value)
	{
		___forParameter_retainPtr_2 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders
struct LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_pinvoke
{
	LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  ___forParameter_e_0;
	LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  ___forParameter_retain_1;
	LambdaParameterValueProvider_IComponentData_1_t2EFFCFAFE87229878D536B5C44DC62239C46601A  ___forParameter_retainPtr_2;
};
// Native definition for COM marshalling of Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders
struct LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_com
{
	LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  ___forParameter_e_0;
	LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  ___forParameter_retain_1;
	LambdaParameterValueProvider_IComponentData_1_t2EFFCFAFE87229878D536B5C44DC62239C46601A  ___forParameter_retainPtr_2;
};

// Unity.Entities.RetainBlobAssets
struct  RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA 
{
public:
	// Unity.Entities.BlobAssetReference`1<System.Byte> Unity.Entities.RetainBlobAssets::DummyBlobAssetReference
	BlobAssetReference_1_t9DE40F3E122A46CD5D0B6486AC40AB530B4D1C34  ___DummyBlobAssetReference_0;
	// System.Int32 Unity.Entities.RetainBlobAssets::FramesToRetainBlobAssets
	int32_t ___FramesToRetainBlobAssets_1;

public:
	inline static int32_t get_offset_of_DummyBlobAssetReference_0() { return static_cast<int32_t>(offsetof(RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA, ___DummyBlobAssetReference_0)); }
	inline BlobAssetReference_1_t9DE40F3E122A46CD5D0B6486AC40AB530B4D1C34  get_DummyBlobAssetReference_0() const { return ___DummyBlobAssetReference_0; }
	inline BlobAssetReference_1_t9DE40F3E122A46CD5D0B6486AC40AB530B4D1C34 * get_address_of_DummyBlobAssetReference_0() { return &___DummyBlobAssetReference_0; }
	inline void set_DummyBlobAssetReference_0(BlobAssetReference_1_t9DE40F3E122A46CD5D0B6486AC40AB530B4D1C34  value)
	{
		___DummyBlobAssetReference_0 = value;
	}

	inline static int32_t get_offset_of_FramesToRetainBlobAssets_1() { return static_cast<int32_t>(offsetof(RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA, ___FramesToRetainBlobAssets_1)); }
	inline int32_t get_FramesToRetainBlobAssets_1() const { return ___FramesToRetainBlobAssets_1; }
	inline int32_t* get_address_of_FramesToRetainBlobAssets_1() { return &___FramesToRetainBlobAssets_1; }
	inline void set_FramesToRetainBlobAssets_1(int32_t value)
	{
		___FramesToRetainBlobAssets_1 = value;
	}
};


// Unity.Entities.SceneSection
struct  SceneSection_t3FEF24F2D1C6132CA8F4B8E9767C170609E30044 
{
public:
	// Unity.Entities.Hash128 Unity.Entities.SceneSection::SceneGUID
	Hash128_t8214C0670F24DF267392561913434E82117B6131  ___SceneGUID_0;
	// System.Int32 Unity.Entities.SceneSection::Section
	int32_t ___Section_1;

public:
	inline static int32_t get_offset_of_SceneGUID_0() { return static_cast<int32_t>(offsetof(SceneSection_t3FEF24F2D1C6132CA8F4B8E9767C170609E30044, ___SceneGUID_0)); }
	inline Hash128_t8214C0670F24DF267392561913434E82117B6131  get_SceneGUID_0() const { return ___SceneGUID_0; }
	inline Hash128_t8214C0670F24DF267392561913434E82117B6131 * get_address_of_SceneGUID_0() { return &___SceneGUID_0; }
	inline void set_SceneGUID_0(Hash128_t8214C0670F24DF267392561913434E82117B6131  value)
	{
		___SceneGUID_0 = value;
	}

	inline static int32_t get_offset_of_Section_1() { return static_cast<int32_t>(offsetof(SceneSection_t3FEF24F2D1C6132CA8F4B8E9767C170609E30044, ___Section_1)); }
	inline int32_t get_Section_1() const { return ___Section_1; }
	inline int32_t* get_address_of_Section_1() { return &___Section_1; }
	inline void set_Section_1(int32_t value)
	{
		___Section_1 = value;
	}
};


// Unity.Entities.TypeManager_TypeInfo
struct  TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 
{
public:
	// System.Int32 Unity.Entities.TypeManager_TypeInfo::TypeIndex
	int32_t ___TypeIndex_0;
	// System.Int32 Unity.Entities.TypeManager_TypeInfo::SizeInChunk
	int32_t ___SizeInChunk_1;
	// System.Int32 Unity.Entities.TypeManager_TypeInfo::AlignmentInBytes
	int32_t ___AlignmentInBytes_2;
	// System.Int32 Unity.Entities.TypeManager_TypeInfo::ElementSize
	int32_t ___ElementSize_3;
	// System.Int32 Unity.Entities.TypeManager_TypeInfo::BufferCapacity
	int32_t ___BufferCapacity_4;
	// Unity.Entities.TypeManager_TypeCategory Unity.Entities.TypeManager_TypeInfo::Category
	int32_t ___Category_5;
	// System.UInt64 Unity.Entities.TypeManager_TypeInfo::MemoryOrdering
	uint64_t ___MemoryOrdering_6;
	// System.UInt64 Unity.Entities.TypeManager_TypeInfo::StableTypeHash
	uint64_t ___StableTypeHash_7;
	// System.Int32 Unity.Entities.TypeManager_TypeInfo::EntityOffsetCount
	int32_t ___EntityOffsetCount_8;
	// System.Int32 Unity.Entities.TypeManager_TypeInfo::EntityOffsetStartIndex
	int32_t ___EntityOffsetStartIndex_9;
	// System.Int32 Unity.Entities.TypeManager_TypeInfo::BlobAssetRefOffsetCount
	int32_t ___BlobAssetRefOffsetCount_10;
	// System.Int32 Unity.Entities.TypeManager_TypeInfo::BlobAssetRefOffsetStartIndex
	int32_t ___BlobAssetRefOffsetStartIndex_11;
	// System.Int32 Unity.Entities.TypeManager_TypeInfo::WriteGroupCount
	int32_t ___WriteGroupCount_12;
	// System.Int32 Unity.Entities.TypeManager_TypeInfo::WriteGroupStartIndex
	int32_t ___WriteGroupStartIndex_13;
	// System.Int32 Unity.Entities.TypeManager_TypeInfo::MaximumChunkCapacity
	int32_t ___MaximumChunkCapacity_14;
	// System.Int32 Unity.Entities.TypeManager_TypeInfo::FastEqualityIndex
	int32_t ___FastEqualityIndex_15;
	// System.Int32 Unity.Entities.TypeManager_TypeInfo::TypeSize
	int32_t ___TypeSize_16;

public:
	inline static int32_t get_offset_of_TypeIndex_0() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___TypeIndex_0)); }
	inline int32_t get_TypeIndex_0() const { return ___TypeIndex_0; }
	inline int32_t* get_address_of_TypeIndex_0() { return &___TypeIndex_0; }
	inline void set_TypeIndex_0(int32_t value)
	{
		___TypeIndex_0 = value;
	}

	inline static int32_t get_offset_of_SizeInChunk_1() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___SizeInChunk_1)); }
	inline int32_t get_SizeInChunk_1() const { return ___SizeInChunk_1; }
	inline int32_t* get_address_of_SizeInChunk_1() { return &___SizeInChunk_1; }
	inline void set_SizeInChunk_1(int32_t value)
	{
		___SizeInChunk_1 = value;
	}

	inline static int32_t get_offset_of_AlignmentInBytes_2() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___AlignmentInBytes_2)); }
	inline int32_t get_AlignmentInBytes_2() const { return ___AlignmentInBytes_2; }
	inline int32_t* get_address_of_AlignmentInBytes_2() { return &___AlignmentInBytes_2; }
	inline void set_AlignmentInBytes_2(int32_t value)
	{
		___AlignmentInBytes_2 = value;
	}

	inline static int32_t get_offset_of_ElementSize_3() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___ElementSize_3)); }
	inline int32_t get_ElementSize_3() const { return ___ElementSize_3; }
	inline int32_t* get_address_of_ElementSize_3() { return &___ElementSize_3; }
	inline void set_ElementSize_3(int32_t value)
	{
		___ElementSize_3 = value;
	}

	inline static int32_t get_offset_of_BufferCapacity_4() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___BufferCapacity_4)); }
	inline int32_t get_BufferCapacity_4() const { return ___BufferCapacity_4; }
	inline int32_t* get_address_of_BufferCapacity_4() { return &___BufferCapacity_4; }
	inline void set_BufferCapacity_4(int32_t value)
	{
		___BufferCapacity_4 = value;
	}

	inline static int32_t get_offset_of_Category_5() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___Category_5)); }
	inline int32_t get_Category_5() const { return ___Category_5; }
	inline int32_t* get_address_of_Category_5() { return &___Category_5; }
	inline void set_Category_5(int32_t value)
	{
		___Category_5 = value;
	}

	inline static int32_t get_offset_of_MemoryOrdering_6() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___MemoryOrdering_6)); }
	inline uint64_t get_MemoryOrdering_6() const { return ___MemoryOrdering_6; }
	inline uint64_t* get_address_of_MemoryOrdering_6() { return &___MemoryOrdering_6; }
	inline void set_MemoryOrdering_6(uint64_t value)
	{
		___MemoryOrdering_6 = value;
	}

	inline static int32_t get_offset_of_StableTypeHash_7() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___StableTypeHash_7)); }
	inline uint64_t get_StableTypeHash_7() const { return ___StableTypeHash_7; }
	inline uint64_t* get_address_of_StableTypeHash_7() { return &___StableTypeHash_7; }
	inline void set_StableTypeHash_7(uint64_t value)
	{
		___StableTypeHash_7 = value;
	}

	inline static int32_t get_offset_of_EntityOffsetCount_8() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___EntityOffsetCount_8)); }
	inline int32_t get_EntityOffsetCount_8() const { return ___EntityOffsetCount_8; }
	inline int32_t* get_address_of_EntityOffsetCount_8() { return &___EntityOffsetCount_8; }
	inline void set_EntityOffsetCount_8(int32_t value)
	{
		___EntityOffsetCount_8 = value;
	}

	inline static int32_t get_offset_of_EntityOffsetStartIndex_9() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___EntityOffsetStartIndex_9)); }
	inline int32_t get_EntityOffsetStartIndex_9() const { return ___EntityOffsetStartIndex_9; }
	inline int32_t* get_address_of_EntityOffsetStartIndex_9() { return &___EntityOffsetStartIndex_9; }
	inline void set_EntityOffsetStartIndex_9(int32_t value)
	{
		___EntityOffsetStartIndex_9 = value;
	}

	inline static int32_t get_offset_of_BlobAssetRefOffsetCount_10() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___BlobAssetRefOffsetCount_10)); }
	inline int32_t get_BlobAssetRefOffsetCount_10() const { return ___BlobAssetRefOffsetCount_10; }
	inline int32_t* get_address_of_BlobAssetRefOffsetCount_10() { return &___BlobAssetRefOffsetCount_10; }
	inline void set_BlobAssetRefOffsetCount_10(int32_t value)
	{
		___BlobAssetRefOffsetCount_10 = value;
	}

	inline static int32_t get_offset_of_BlobAssetRefOffsetStartIndex_11() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___BlobAssetRefOffsetStartIndex_11)); }
	inline int32_t get_BlobAssetRefOffsetStartIndex_11() const { return ___BlobAssetRefOffsetStartIndex_11; }
	inline int32_t* get_address_of_BlobAssetRefOffsetStartIndex_11() { return &___BlobAssetRefOffsetStartIndex_11; }
	inline void set_BlobAssetRefOffsetStartIndex_11(int32_t value)
	{
		___BlobAssetRefOffsetStartIndex_11 = value;
	}

	inline static int32_t get_offset_of_WriteGroupCount_12() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___WriteGroupCount_12)); }
	inline int32_t get_WriteGroupCount_12() const { return ___WriteGroupCount_12; }
	inline int32_t* get_address_of_WriteGroupCount_12() { return &___WriteGroupCount_12; }
	inline void set_WriteGroupCount_12(int32_t value)
	{
		___WriteGroupCount_12 = value;
	}

	inline static int32_t get_offset_of_WriteGroupStartIndex_13() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___WriteGroupStartIndex_13)); }
	inline int32_t get_WriteGroupStartIndex_13() const { return ___WriteGroupStartIndex_13; }
	inline int32_t* get_address_of_WriteGroupStartIndex_13() { return &___WriteGroupStartIndex_13; }
	inline void set_WriteGroupStartIndex_13(int32_t value)
	{
		___WriteGroupStartIndex_13 = value;
	}

	inline static int32_t get_offset_of_MaximumChunkCapacity_14() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___MaximumChunkCapacity_14)); }
	inline int32_t get_MaximumChunkCapacity_14() const { return ___MaximumChunkCapacity_14; }
	inline int32_t* get_address_of_MaximumChunkCapacity_14() { return &___MaximumChunkCapacity_14; }
	inline void set_MaximumChunkCapacity_14(int32_t value)
	{
		___MaximumChunkCapacity_14 = value;
	}

	inline static int32_t get_offset_of_FastEqualityIndex_15() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___FastEqualityIndex_15)); }
	inline int32_t get_FastEqualityIndex_15() const { return ___FastEqualityIndex_15; }
	inline int32_t* get_address_of_FastEqualityIndex_15() { return &___FastEqualityIndex_15; }
	inline void set_FastEqualityIndex_15(int32_t value)
	{
		___FastEqualityIndex_15 = value;
	}

	inline static int32_t get_offset_of_TypeSize_16() { return static_cast<int32_t>(offsetof(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38, ___TypeSize_16)); }
	inline int32_t get_TypeSize_16() const { return ___TypeSize_16; }
	inline int32_t* get_address_of_TypeSize_16() { return &___TypeSize_16; }
	inline void set_TypeSize_16(int32_t value)
	{
		___TypeSize_16 = value;
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


// Unity.Collections.NativeHashMap`2<System.UInt64,System.Int32>
struct  NativeHashMap_2_t2702DC826E1F3A2D98EA4CEE7798F11E0C91F608 
{
public:
	// Unity.Collections.LowLevel.Unsafe.UnsafeHashMap`2<TKey,TValue> Unity.Collections.NativeHashMap`2::m_HashMapData
	UnsafeHashMap_2_t48AE5E3C55FDD98B6C4D2C6449B1EE8EA6D531F7  ___m_HashMapData_0;

public:
	inline static int32_t get_offset_of_m_HashMapData_0() { return static_cast<int32_t>(offsetof(NativeHashMap_2_t2702DC826E1F3A2D98EA4CEE7798F11E0C91F608, ___m_HashMapData_0)); }
	inline UnsafeHashMap_2_t48AE5E3C55FDD98B6C4D2C6449B1EE8EA6D531F7  get_m_HashMapData_0() const { return ___m_HashMapData_0; }
	inline UnsafeHashMap_2_t48AE5E3C55FDD98B6C4D2C6449B1EE8EA6D531F7 * get_address_of_m_HashMapData_0() { return &___m_HashMapData_0; }
	inline void set_m_HashMapData_0(UnsafeHashMap_2_t48AE5E3C55FDD98B6C4D2C6449B1EE8EA6D531F7  value)
	{
		___m_HashMapData_0 = value;
	}
};


// Unity.Collections.NativeHashMap`2<Unity.Entities.Serialization.SerializeUtility_BlobAssetPtr,System.Int32>
struct  NativeHashMap_2_tD236D8A3C81093EDA5D488C9CBBFCA0E8431A1C6 
{
public:
	// Unity.Collections.LowLevel.Unsafe.UnsafeHashMap`2<TKey,TValue> Unity.Collections.NativeHashMap`2::m_HashMapData
	UnsafeHashMap_2_t337A0F0DB9C26F7409D3D9BB22F421B4C7A6893A  ___m_HashMapData_0;

public:
	inline static int32_t get_offset_of_m_HashMapData_0() { return static_cast<int32_t>(offsetof(NativeHashMap_2_tD236D8A3C81093EDA5D488C9CBBFCA0E8431A1C6, ___m_HashMapData_0)); }
	inline UnsafeHashMap_2_t337A0F0DB9C26F7409D3D9BB22F421B4C7A6893A  get_m_HashMapData_0() const { return ___m_HashMapData_0; }
	inline UnsafeHashMap_2_t337A0F0DB9C26F7409D3D9BB22F421B4C7A6893A * get_address_of_m_HashMapData_0() { return &___m_HashMapData_0; }
	inline void set_m_HashMapData_0(UnsafeHashMap_2_t337A0F0DB9C26F7409D3D9BB22F421B4C7A6893A  value)
	{
		___m_HashMapData_0 = value;
	}
};


// Unity.Collections.NativeMultiHashMap`2<Unity.Entities.EntityPatcher_EntityComponentPair,Unity.Entities.EntityPatcher_OffsetEntityPair>
struct  NativeMultiHashMap_2_t60EC0F9BEBB301A3A42CC426583D29FDF8CCEF37 
{
public:
	// Unity.Collections.LowLevel.Unsafe.UnsafeMultiHashMap`2<TKey,TValue> Unity.Collections.NativeMultiHashMap`2::m_MultiHashMapData
	UnsafeMultiHashMap_2_t1516AF586287D8D01621118A3D63CC75A85B1A2F  ___m_MultiHashMapData_0;

public:
	inline static int32_t get_offset_of_m_MultiHashMapData_0() { return static_cast<int32_t>(offsetof(NativeMultiHashMap_2_t60EC0F9BEBB301A3A42CC426583D29FDF8CCEF37, ___m_MultiHashMapData_0)); }
	inline UnsafeMultiHashMap_2_t1516AF586287D8D01621118A3D63CC75A85B1A2F  get_m_MultiHashMapData_0() const { return ___m_MultiHashMapData_0; }
	inline UnsafeMultiHashMap_2_t1516AF586287D8D01621118A3D63CC75A85B1A2F * get_address_of_m_MultiHashMapData_0() { return &___m_MultiHashMapData_0; }
	inline void set_m_MultiHashMapData_0(UnsafeMultiHashMap_2_t1516AF586287D8D01621118A3D63CC75A85B1A2F  value)
	{
		___m_MultiHashMapData_0 = value;
	}
};


// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1_StructuralChangeRuntime<Unity.Entities.RetainBlobAssetBatchPtr>
struct  StructuralChangeRuntime_t91CF003F24C722618BFAB2EC15E454A10524C5F5 
{
public:
	// Unity.Entities.EntityManager Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1_StructuralChangeRuntime::_manager
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  ____manager_0;
	// System.Int32 Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1_StructuralChangeRuntime::_typeIndex
	int32_t ____typeIndex_1;

public:
	inline static int32_t get_offset_of__manager_0() { return static_cast<int32_t>(offsetof(StructuralChangeRuntime_t91CF003F24C722618BFAB2EC15E454A10524C5F5, ____manager_0)); }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  get__manager_0() const { return ____manager_0; }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0 * get_address_of__manager_0() { return &____manager_0; }
	inline void set__manager_0(EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  value)
	{
		____manager_0 = value;
	}

	inline static int32_t get_offset_of__typeIndex_1() { return static_cast<int32_t>(offsetof(StructuralChangeRuntime_t91CF003F24C722618BFAB2EC15E454A10524C5F5, ____typeIndex_1)); }
	inline int32_t get__typeIndex_1() const { return ____typeIndex_1; }
	inline int32_t* get_address_of__typeIndex_1() { return &____typeIndex_1; }
	inline void set__typeIndex_1(int32_t value)
	{
		____typeIndex_1 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime
#ifndef StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_pinvoke_define
#define StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_pinvoke_define
struct StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_pinvoke
{
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_marshaled_pinvoke ____manager_0;
	int32_t ____typeIndex_1;
};
#endif
// Native definition for COM marshalling of Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime
#ifndef StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_com_define
#define StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_com_define
struct StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_com
{
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_marshaled_com ____manager_0;
	int32_t ____typeIndex_1;
};
#endif

// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1_StructuralChangeRuntime<Unity.Entities.RetainBlobAssetPtr>
struct  StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088 
{
public:
	// Unity.Entities.EntityManager Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1_StructuralChangeRuntime::_manager
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  ____manager_0;
	// System.Int32 Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1_StructuralChangeRuntime::_typeIndex
	int32_t ____typeIndex_1;

public:
	inline static int32_t get_offset_of__manager_0() { return static_cast<int32_t>(offsetof(StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088, ____manager_0)); }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  get__manager_0() const { return ____manager_0; }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0 * get_address_of__manager_0() { return &____manager_0; }
	inline void set__manager_0(EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  value)
	{
		____manager_0 = value;
	}

	inline static int32_t get_offset_of__typeIndex_1() { return static_cast<int32_t>(offsetof(StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088, ____typeIndex_1)); }
	inline int32_t get__typeIndex_1() const { return ____typeIndex_1; }
	inline int32_t* get_address_of__typeIndex_1() { return &____typeIndex_1; }
	inline void set__typeIndex_1(int32_t value)
	{
		____typeIndex_1 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime
#ifndef StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_pinvoke_define
#define StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_pinvoke_define
struct StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_pinvoke
{
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_marshaled_pinvoke ____manager_0;
	int32_t ____typeIndex_1;
};
#endif
// Native definition for COM marshalling of Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime
#ifndef StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_com_define
#define StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_com_define
struct StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_com
{
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_marshaled_com ____manager_0;
	int32_t ____typeIndex_1;
};
#endif

// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1_StructuralChangeRuntime<Unity.Entities.RetainBlobAssets>
struct  StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B 
{
public:
	// Unity.Entities.EntityManager Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1_StructuralChangeRuntime::_manager
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  ____manager_0;
	// System.Int32 Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1_StructuralChangeRuntime::_typeIndex
	int32_t ____typeIndex_1;

public:
	inline static int32_t get_offset_of__manager_0() { return static_cast<int32_t>(offsetof(StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B, ____manager_0)); }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  get__manager_0() const { return ____manager_0; }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0 * get_address_of__manager_0() { return &____manager_0; }
	inline void set__manager_0(EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  value)
	{
		____manager_0 = value;
	}

	inline static int32_t get_offset_of__typeIndex_1() { return static_cast<int32_t>(offsetof(StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B, ____typeIndex_1)); }
	inline int32_t get__typeIndex_1() const { return ____typeIndex_1; }
	inline int32_t* get_address_of__typeIndex_1() { return &____typeIndex_1; }
	inline void set__typeIndex_1(int32_t value)
	{
		____typeIndex_1 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime
#ifndef StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_pinvoke_define
#define StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_pinvoke_define
struct StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_pinvoke
{
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_marshaled_pinvoke ____manager_0;
	int32_t ____typeIndex_1;
};
#endif
// Native definition for COM marshalling of Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime
#ifndef StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_com_define
#define StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_com_define
struct StructuralChangeRuntime_t6179906746B2C9E74816610C15C756D5B9A2BEBA_marshaled_com
{
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_marshaled_com ____manager_0;
	int32_t ____typeIndex_1;
};
#endif

// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_ISharedComponentData`1_StructuralChangeRuntime<Unity.Entities.BlobAssetOwner>
struct  StructuralChangeRuntime_t73F4A22CB7388EB3FB14FDAB733CD0AA00539B60 
{
public:
	// Unity.Entities.EntityManager Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_ISharedComponentData`1_StructuralChangeRuntime::_manager
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  ____manager_0;

public:
	inline static int32_t get_offset_of__manager_0() { return static_cast<int32_t>(offsetof(StructuralChangeRuntime_t73F4A22CB7388EB3FB14FDAB733CD0AA00539B60, ____manager_0)); }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  get__manager_0() const { return ____manager_0; }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0 * get_address_of__manager_0() { return &____manager_0; }
	inline void set__manager_0(EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  value)
	{
		____manager_0 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_ISharedComponentData`1/StructuralChangeRuntime
#ifndef StructuralChangeRuntime_t4CAC0323B8E694C8AFC54C057127842FBA82FD35_marshaled_pinvoke_define
#define StructuralChangeRuntime_t4CAC0323B8E694C8AFC54C057127842FBA82FD35_marshaled_pinvoke_define
struct StructuralChangeRuntime_t4CAC0323B8E694C8AFC54C057127842FBA82FD35_marshaled_pinvoke
{
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_marshaled_pinvoke ____manager_0;
};
#endif
// Native definition for COM marshalling of Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_ISharedComponentData`1/StructuralChangeRuntime
#ifndef StructuralChangeRuntime_t4CAC0323B8E694C8AFC54C057127842FBA82FD35_marshaled_com_define
#define StructuralChangeRuntime_t4CAC0323B8E694C8AFC54C057127842FBA82FD35_marshaled_com_define
struct StructuralChangeRuntime_t4CAC0323B8E694C8AFC54C057127842FBA82FD35_marshaled_com
{
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_marshaled_com ____manager_0;
};
#endif

// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_ISharedComponentData`1<Unity.Entities.BlobAssetOwner>
struct  LambdaParameterValueProvider_ISharedComponentData_1_tE871547CB822CE66B2C193D94183ACD389503DF6 
{
public:
	// Unity.Entities.ArchetypeChunkSharedComponentType`1<T> Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_ISharedComponentData`1::_type
	ArchetypeChunkSharedComponentType_1_t848742A20BE275A0E107A9BC6C465BA4E524BF4E  ____type_0;
	// Unity.Entities.EntityManager Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_ISharedComponentData`1::_entityManager
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  ____entityManager_1;

public:
	inline static int32_t get_offset_of__type_0() { return static_cast<int32_t>(offsetof(LambdaParameterValueProvider_ISharedComponentData_1_tE871547CB822CE66B2C193D94183ACD389503DF6, ____type_0)); }
	inline ArchetypeChunkSharedComponentType_1_t848742A20BE275A0E107A9BC6C465BA4E524BF4E  get__type_0() const { return ____type_0; }
	inline ArchetypeChunkSharedComponentType_1_t848742A20BE275A0E107A9BC6C465BA4E524BF4E * get_address_of__type_0() { return &____type_0; }
	inline void set__type_0(ArchetypeChunkSharedComponentType_1_t848742A20BE275A0E107A9BC6C465BA4E524BF4E  value)
	{
		____type_0 = value;
	}

	inline static int32_t get_offset_of__entityManager_1() { return static_cast<int32_t>(offsetof(LambdaParameterValueProvider_ISharedComponentData_1_tE871547CB822CE66B2C193D94183ACD389503DF6, ____entityManager_1)); }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  get__entityManager_1() const { return ____entityManager_1; }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0 * get_address_of__entityManager_1() { return &____entityManager_1; }
	inline void set__entityManager_1(EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  value)
	{
		____entityManager_1 = value;
	}
};


// Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider_PerformLambdaDelegate
struct  PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.EntityPatcher_EntityComponentPair
struct  EntityComponentPair_tF1A2F420287859D321D85CB20D9846DB9101294C 
{
public:
	// Unity.Entities.Entity Unity.Entities.EntityPatcher_EntityComponentPair::Entity
	Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___Entity_0;
	// Unity.Entities.ComponentType Unity.Entities.EntityPatcher_EntityComponentPair::Component
	ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370  ___Component_1;

public:
	inline static int32_t get_offset_of_Entity_0() { return static_cast<int32_t>(offsetof(EntityComponentPair_tF1A2F420287859D321D85CB20D9846DB9101294C, ___Entity_0)); }
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  get_Entity_0() const { return ___Entity_0; }
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * get_address_of_Entity_0() { return &___Entity_0; }
	inline void set_Entity_0(Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  value)
	{
		___Entity_0 = value;
	}

	inline static int32_t get_offset_of_Component_1() { return static_cast<int32_t>(offsetof(EntityComponentPair_tF1A2F420287859D321D85CB20D9846DB9101294C, ___Component_1)); }
	inline ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370  get_Component_1() const { return ___Component_1; }
	inline ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370 * get_address_of_Component_1() { return &___Component_1; }
	inline void set_Component_1(ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370  value)
	{
		___Component_1 = value;
	}
};


// Unity.Entities.EntityQuery_GatherEntitiesResult
struct  GatherEntitiesResult_t731EB8DD7C87CA907966DC48C8786447A5E39B4E 
{
public:
	// System.Int32 Unity.Entities.EntityQuery_GatherEntitiesResult::StartingOffset
	int32_t ___StartingOffset_0;
	// System.Int32 Unity.Entities.EntityQuery_GatherEntitiesResult::EntityCount
	int32_t ___EntityCount_1;
	// Unity.Entities.Entity* Unity.Entities.EntityQuery_GatherEntitiesResult::EntityBuffer
	Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___EntityBuffer_2;
	// Unity.Collections.NativeArray`1<Unity.Entities.Entity> Unity.Entities.EntityQuery_GatherEntitiesResult::EntityArray
	NativeArray_1_t587B0E555A435D1A1EACD16A8F3C3EBCF3497F5E  ___EntityArray_3;

public:
	inline static int32_t get_offset_of_StartingOffset_0() { return static_cast<int32_t>(offsetof(GatherEntitiesResult_t731EB8DD7C87CA907966DC48C8786447A5E39B4E, ___StartingOffset_0)); }
	inline int32_t get_StartingOffset_0() const { return ___StartingOffset_0; }
	inline int32_t* get_address_of_StartingOffset_0() { return &___StartingOffset_0; }
	inline void set_StartingOffset_0(int32_t value)
	{
		___StartingOffset_0 = value;
	}

	inline static int32_t get_offset_of_EntityCount_1() { return static_cast<int32_t>(offsetof(GatherEntitiesResult_t731EB8DD7C87CA907966DC48C8786447A5E39B4E, ___EntityCount_1)); }
	inline int32_t get_EntityCount_1() const { return ___EntityCount_1; }
	inline int32_t* get_address_of_EntityCount_1() { return &___EntityCount_1; }
	inline void set_EntityCount_1(int32_t value)
	{
		___EntityCount_1 = value;
	}

	inline static int32_t get_offset_of_EntityBuffer_2() { return static_cast<int32_t>(offsetof(GatherEntitiesResult_t731EB8DD7C87CA907966DC48C8786447A5E39B4E, ___EntityBuffer_2)); }
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * get_EntityBuffer_2() const { return ___EntityBuffer_2; }
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 ** get_address_of_EntityBuffer_2() { return &___EntityBuffer_2; }
	inline void set_EntityBuffer_2(Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * value)
	{
		___EntityBuffer_2 = value;
	}

	inline static int32_t get_offset_of_EntityArray_3() { return static_cast<int32_t>(offsetof(GatherEntitiesResult_t731EB8DD7C87CA907966DC48C8786447A5E39B4E, ___EntityArray_3)); }
	inline NativeArray_1_t587B0E555A435D1A1EACD16A8F3C3EBCF3497F5E  get_EntityArray_3() const { return ___EntityArray_3; }
	inline NativeArray_1_t587B0E555A435D1A1EACD16A8F3C3EBCF3497F5E * get_address_of_EntityArray_3() { return &___EntityArray_3; }
	inline void set_EntityArray_3(NativeArray_1_t587B0E555A435D1A1EACD16A8F3C3EBCF3497F5E  value)
	{
		___EntityArray_3 = value;
	}
};


// Unity.Entities.FastEquality_TypeInfo_CompareEqualDelegate
struct  CompareEqualDelegate_t15914FD0CE4F07A1C8E6EA6B2A931D83586220D7  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.FastEquality_TypeInfo_GetHashCodeDelegate
struct  GetHashCodeDelegate_t0463D39E4D6E298D2C15D08C75CAFB0CBF02E353  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.FastEquality_TypeInfo_ManagedCompareEqualDelegate
struct  ManagedCompareEqualDelegate_t1D9D97E36B8D0245138610749B6F2B74D6CEBB06  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.FastEquality_TypeInfo_ManagedGetHashCodeDelegate
struct  ManagedGetHashCodeDelegate_tDC6EDBDBB5F0F94C90DFCB41F6692CBA3B2A75DC  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.ManagedDeferredCommands
struct  ManagedDeferredCommands_tD9C6F59E91B0E55519A28C84158F57A14B1CEFC5 
{
public:
	// Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer Unity.Entities.ManagedDeferredCommands::CommandBuffer
	UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C  ___CommandBuffer_0;

public:
	inline static int32_t get_offset_of_CommandBuffer_0() { return static_cast<int32_t>(offsetof(ManagedDeferredCommands_tD9C6F59E91B0E55519A28C84158F57A14B1CEFC5, ___CommandBuffer_0)); }
	inline UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C  get_CommandBuffer_0() const { return ___CommandBuffer_0; }
	inline UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C * get_address_of_CommandBuffer_0() { return &___CommandBuffer_0; }
	inline void set_CommandBuffer_0(UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C  value)
	{
		___CommandBuffer_0 = value;
	}
};


// Unity.Entities.RetainBlobAssetSystem
struct  RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14  : public JobComponentSystem_t6E53C63CFD72D32106593148904568D2182B9B9B
{
public:
	// Unity.Entities.EntityQuery Unity.Entities.RetainBlobAssetSystem::<>OnUpdate_LambdaJob0_entityQuery
	EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___U3CU3EOnUpdate_LambdaJob0_entityQuery_3;
	// Unity.Profiling.ProfilerMarker Unity.Entities.RetainBlobAssetSystem::<>OnUpdate_LambdaJob0_profilerMarker
	ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  ___U3CU3EOnUpdate_LambdaJob0_profilerMarker_4;
	// Unity.Entities.EntityQuery Unity.Entities.RetainBlobAssetSystem::<>OnUpdate_LambdaJob1_entityQuery
	EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___U3CU3EOnUpdate_LambdaJob1_entityQuery_5;
	// Unity.Profiling.ProfilerMarker Unity.Entities.RetainBlobAssetSystem::<>OnUpdate_LambdaJob1_profilerMarker
	ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  ___U3CU3EOnUpdate_LambdaJob1_profilerMarker_6;
	// Unity.Entities.EntityQuery Unity.Entities.RetainBlobAssetSystem::<>OnUpdate_LambdaJob2_entityQuery
	EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___U3CU3EOnUpdate_LambdaJob2_entityQuery_7;
	// Unity.Profiling.ProfilerMarker Unity.Entities.RetainBlobAssetSystem::<>OnUpdate_LambdaJob2_profilerMarker
	ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  ___U3CU3EOnUpdate_LambdaJob2_profilerMarker_8;

public:
	inline static int32_t get_offset_of_U3CU3EOnUpdate_LambdaJob0_entityQuery_3() { return static_cast<int32_t>(offsetof(RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14, ___U3CU3EOnUpdate_LambdaJob0_entityQuery_3)); }
	inline EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  get_U3CU3EOnUpdate_LambdaJob0_entityQuery_3() const { return ___U3CU3EOnUpdate_LambdaJob0_entityQuery_3; }
	inline EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109 * get_address_of_U3CU3EOnUpdate_LambdaJob0_entityQuery_3() { return &___U3CU3EOnUpdate_LambdaJob0_entityQuery_3; }
	inline void set_U3CU3EOnUpdate_LambdaJob0_entityQuery_3(EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  value)
	{
		___U3CU3EOnUpdate_LambdaJob0_entityQuery_3 = value;
	}

	inline static int32_t get_offset_of_U3CU3EOnUpdate_LambdaJob0_profilerMarker_4() { return static_cast<int32_t>(offsetof(RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14, ___U3CU3EOnUpdate_LambdaJob0_profilerMarker_4)); }
	inline ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  get_U3CU3EOnUpdate_LambdaJob0_profilerMarker_4() const { return ___U3CU3EOnUpdate_LambdaJob0_profilerMarker_4; }
	inline ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1 * get_address_of_U3CU3EOnUpdate_LambdaJob0_profilerMarker_4() { return &___U3CU3EOnUpdate_LambdaJob0_profilerMarker_4; }
	inline void set_U3CU3EOnUpdate_LambdaJob0_profilerMarker_4(ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  value)
	{
		___U3CU3EOnUpdate_LambdaJob0_profilerMarker_4 = value;
	}

	inline static int32_t get_offset_of_U3CU3EOnUpdate_LambdaJob1_entityQuery_5() { return static_cast<int32_t>(offsetof(RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14, ___U3CU3EOnUpdate_LambdaJob1_entityQuery_5)); }
	inline EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  get_U3CU3EOnUpdate_LambdaJob1_entityQuery_5() const { return ___U3CU3EOnUpdate_LambdaJob1_entityQuery_5; }
	inline EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109 * get_address_of_U3CU3EOnUpdate_LambdaJob1_entityQuery_5() { return &___U3CU3EOnUpdate_LambdaJob1_entityQuery_5; }
	inline void set_U3CU3EOnUpdate_LambdaJob1_entityQuery_5(EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  value)
	{
		___U3CU3EOnUpdate_LambdaJob1_entityQuery_5 = value;
	}

	inline static int32_t get_offset_of_U3CU3EOnUpdate_LambdaJob1_profilerMarker_6() { return static_cast<int32_t>(offsetof(RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14, ___U3CU3EOnUpdate_LambdaJob1_profilerMarker_6)); }
	inline ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  get_U3CU3EOnUpdate_LambdaJob1_profilerMarker_6() const { return ___U3CU3EOnUpdate_LambdaJob1_profilerMarker_6; }
	inline ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1 * get_address_of_U3CU3EOnUpdate_LambdaJob1_profilerMarker_6() { return &___U3CU3EOnUpdate_LambdaJob1_profilerMarker_6; }
	inline void set_U3CU3EOnUpdate_LambdaJob1_profilerMarker_6(ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  value)
	{
		___U3CU3EOnUpdate_LambdaJob1_profilerMarker_6 = value;
	}

	inline static int32_t get_offset_of_U3CU3EOnUpdate_LambdaJob2_entityQuery_7() { return static_cast<int32_t>(offsetof(RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14, ___U3CU3EOnUpdate_LambdaJob2_entityQuery_7)); }
	inline EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  get_U3CU3EOnUpdate_LambdaJob2_entityQuery_7() const { return ___U3CU3EOnUpdate_LambdaJob2_entityQuery_7; }
	inline EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109 * get_address_of_U3CU3EOnUpdate_LambdaJob2_entityQuery_7() { return &___U3CU3EOnUpdate_LambdaJob2_entityQuery_7; }
	inline void set_U3CU3EOnUpdate_LambdaJob2_entityQuery_7(EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  value)
	{
		___U3CU3EOnUpdate_LambdaJob2_entityQuery_7 = value;
	}

	inline static int32_t get_offset_of_U3CU3EOnUpdate_LambdaJob2_profilerMarker_8() { return static_cast<int32_t>(offsetof(RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14, ___U3CU3EOnUpdate_LambdaJob2_profilerMarker_8)); }
	inline ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  get_U3CU3EOnUpdate_LambdaJob2_profilerMarker_8() const { return ___U3CU3EOnUpdate_LambdaJob2_profilerMarker_8; }
	inline ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1 * get_address_of_U3CU3EOnUpdate_LambdaJob2_profilerMarker_8() { return &___U3CU3EOnUpdate_LambdaJob2_profilerMarker_8; }
	inline void set_U3CU3EOnUpdate_LambdaJob2_profilerMarker_8(ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  value)
	{
		___U3CU3EOnUpdate_LambdaJob2_profilerMarker_8 = value;
	}
};


// Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2
struct  U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E 
{
public:
	// Unity.Entities.RetainBlobAssetSystem Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2::hostInstance
	RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14 * ___hostInstance_0;
	// Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2_LambdaParameterValueProviders Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2::_lambdaParameterValueProviders
	LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31  ____lambdaParameterValueProviders_1;
	// Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2_LambdaParameterValueProviders_Runtimes* Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2::_runtimes
	Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * ____runtimes_2;

public:
	inline static int32_t get_offset_of_hostInstance_0() { return static_cast<int32_t>(offsetof(U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E, ___hostInstance_0)); }
	inline RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14 * get_hostInstance_0() const { return ___hostInstance_0; }
	inline RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14 ** get_address_of_hostInstance_0() { return &___hostInstance_0; }
	inline void set_hostInstance_0(RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14 * value)
	{
		___hostInstance_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___hostInstance_0), (void*)value);
	}

	inline static int32_t get_offset_of__lambdaParameterValueProviders_1() { return static_cast<int32_t>(offsetof(U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E, ____lambdaParameterValueProviders_1)); }
	inline LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31  get__lambdaParameterValueProviders_1() const { return ____lambdaParameterValueProviders_1; }
	inline LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31 * get_address_of__lambdaParameterValueProviders_1() { return &____lambdaParameterValueProviders_1; }
	inline void set__lambdaParameterValueProviders_1(LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31  value)
	{
		____lambdaParameterValueProviders_1 = value;
	}

	inline static int32_t get_offset_of__runtimes_2() { return static_cast<int32_t>(offsetof(U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E, ____runtimes_2)); }
	inline Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * get__runtimes_2() const { return ____runtimes_2; }
	inline Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 ** get_address_of__runtimes_2() { return &____runtimes_2; }
	inline void set__runtimes_2(Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * value)
	{
		____runtimes_2 = value;
	}
};

struct U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_StaticFields
{
public:
	// Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider_PerformLambdaDelegate Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2::_performLambdaDelegate
	PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 * ____performLambdaDelegate_3;

public:
	inline static int32_t get_offset_of__performLambdaDelegate_3() { return static_cast<int32_t>(offsetof(U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_StaticFields, ____performLambdaDelegate_3)); }
	inline PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 * get__performLambdaDelegate_3() const { return ____performLambdaDelegate_3; }
	inline PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 ** get_address_of__performLambdaDelegate_3() { return &____performLambdaDelegate_3; }
	inline void set__performLambdaDelegate_3(PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 * value)
	{
		____performLambdaDelegate_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____performLambdaDelegate_3), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2
struct U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_marshaled_pinvoke
{
	RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14 * ___hostInstance_0;
	LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_pinvoke ____lambdaParameterValueProviders_1;
	Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * ____runtimes_2;
};
// Native definition for COM marshalling of Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2
struct U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_marshaled_com
{
	RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14 * ___hostInstance_0;
	LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_com ____lambdaParameterValueProviders_1;
	Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * ____runtimes_2;
};

// Unity.Entities.SystemBaseRegistry_ForwardingFunc
struct  ForwardingFunc_t830DB8E27A008A51CBCBAAB812E1F2A06C2E89F1  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.SystemState
struct  SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 
{
public:
	// System.Int32 Unity.Entities.SystemState::m_UnmanagedMetaIndex
	int32_t ___m_UnmanagedMetaIndex_0;
	// System.Void* Unity.Entities.SystemState::m_SystemPtr
	void* ___m_SystemPtr_1;
	// Unity.Collections.LowLevel.Unsafe.UnsafeList Unity.Entities.SystemState::m_EntityQueries
	UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA  ___m_EntityQueries_2;
	// Unity.Collections.LowLevel.Unsafe.UnsafeList Unity.Entities.SystemState::m_RequiredEntityQueries
	UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA  ___m_RequiredEntityQueries_3;
	// Unity.Entities.UnsafeIntList Unity.Entities.SystemState::m_JobDependencyForReadingSystems
	UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B  ___m_JobDependencyForReadingSystems_4;
	// Unity.Entities.UnsafeIntList Unity.Entities.SystemState::m_JobDependencyForWritingSystems
	UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B  ___m_JobDependencyForWritingSystems_5;
	// System.UInt32 Unity.Entities.SystemState::m_LastSystemVersion
	uint32_t ___m_LastSystemVersion_6;
	// Unity.Entities.EntityManager Unity.Entities.SystemState::m_EntityManager
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  ___m_EntityManager_7;
	// Unity.Entities.EntityComponentStore* Unity.Entities.SystemState::m_EntityComponentStore
	EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___m_EntityComponentStore_8;
	// Unity.Entities.ComponentDependencyManager* Unity.Entities.SystemState::m_DependencyManager
	ComponentDependencyManager_tAFE29BF05294E4C3CFE1B599D078ACF14D0FAF91 * ___m_DependencyManager_9;
	// System.Runtime.InteropServices.GCHandle Unity.Entities.SystemState::m_World
	GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603  ___m_World_10;
	// System.Boolean Unity.Entities.SystemState::m_AlwaysUpdateSystem
	bool ___m_AlwaysUpdateSystem_11;
	// System.Boolean Unity.Entities.SystemState::m_Enabled
	bool ___m_Enabled_12;
	// System.Boolean Unity.Entities.SystemState::m_PreviouslyEnabled
	bool ___m_PreviouslyEnabled_13;
	// System.Boolean Unity.Entities.SystemState::m_GetDependencyFromSafetyManager
	bool ___m_GetDependencyFromSafetyManager_14;
	// Unity.Jobs.JobHandle Unity.Entities.SystemState::m_JobHandle
	JobHandle_t8AEB8D31C25D7774C71D62B0C662525E6E36D847  ___m_JobHandle_15;
	// Unity.Profiling.ProfilerMarker Unity.Entities.SystemState::m_ProfilerMarker
	ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  ___m_ProfilerMarker_16;

public:
	inline static int32_t get_offset_of_m_UnmanagedMetaIndex_0() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_UnmanagedMetaIndex_0)); }
	inline int32_t get_m_UnmanagedMetaIndex_0() const { return ___m_UnmanagedMetaIndex_0; }
	inline int32_t* get_address_of_m_UnmanagedMetaIndex_0() { return &___m_UnmanagedMetaIndex_0; }
	inline void set_m_UnmanagedMetaIndex_0(int32_t value)
	{
		___m_UnmanagedMetaIndex_0 = value;
	}

	inline static int32_t get_offset_of_m_SystemPtr_1() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_SystemPtr_1)); }
	inline void* get_m_SystemPtr_1() const { return ___m_SystemPtr_1; }
	inline void** get_address_of_m_SystemPtr_1() { return &___m_SystemPtr_1; }
	inline void set_m_SystemPtr_1(void* value)
	{
		___m_SystemPtr_1 = value;
	}

	inline static int32_t get_offset_of_m_EntityQueries_2() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_EntityQueries_2)); }
	inline UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA  get_m_EntityQueries_2() const { return ___m_EntityQueries_2; }
	inline UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * get_address_of_m_EntityQueries_2() { return &___m_EntityQueries_2; }
	inline void set_m_EntityQueries_2(UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA  value)
	{
		___m_EntityQueries_2 = value;
	}

	inline static int32_t get_offset_of_m_RequiredEntityQueries_3() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_RequiredEntityQueries_3)); }
	inline UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA  get_m_RequiredEntityQueries_3() const { return ___m_RequiredEntityQueries_3; }
	inline UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * get_address_of_m_RequiredEntityQueries_3() { return &___m_RequiredEntityQueries_3; }
	inline void set_m_RequiredEntityQueries_3(UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA  value)
	{
		___m_RequiredEntityQueries_3 = value;
	}

	inline static int32_t get_offset_of_m_JobDependencyForReadingSystems_4() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_JobDependencyForReadingSystems_4)); }
	inline UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B  get_m_JobDependencyForReadingSystems_4() const { return ___m_JobDependencyForReadingSystems_4; }
	inline UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B * get_address_of_m_JobDependencyForReadingSystems_4() { return &___m_JobDependencyForReadingSystems_4; }
	inline void set_m_JobDependencyForReadingSystems_4(UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B  value)
	{
		___m_JobDependencyForReadingSystems_4 = value;
	}

	inline static int32_t get_offset_of_m_JobDependencyForWritingSystems_5() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_JobDependencyForWritingSystems_5)); }
	inline UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B  get_m_JobDependencyForWritingSystems_5() const { return ___m_JobDependencyForWritingSystems_5; }
	inline UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B * get_address_of_m_JobDependencyForWritingSystems_5() { return &___m_JobDependencyForWritingSystems_5; }
	inline void set_m_JobDependencyForWritingSystems_5(UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B  value)
	{
		___m_JobDependencyForWritingSystems_5 = value;
	}

	inline static int32_t get_offset_of_m_LastSystemVersion_6() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_LastSystemVersion_6)); }
	inline uint32_t get_m_LastSystemVersion_6() const { return ___m_LastSystemVersion_6; }
	inline uint32_t* get_address_of_m_LastSystemVersion_6() { return &___m_LastSystemVersion_6; }
	inline void set_m_LastSystemVersion_6(uint32_t value)
	{
		___m_LastSystemVersion_6 = value;
	}

	inline static int32_t get_offset_of_m_EntityManager_7() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_EntityManager_7)); }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  get_m_EntityManager_7() const { return ___m_EntityManager_7; }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0 * get_address_of_m_EntityManager_7() { return &___m_EntityManager_7; }
	inline void set_m_EntityManager_7(EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  value)
	{
		___m_EntityManager_7 = value;
	}

	inline static int32_t get_offset_of_m_EntityComponentStore_8() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_EntityComponentStore_8)); }
	inline EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * get_m_EntityComponentStore_8() const { return ___m_EntityComponentStore_8; }
	inline EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA ** get_address_of_m_EntityComponentStore_8() { return &___m_EntityComponentStore_8; }
	inline void set_m_EntityComponentStore_8(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * value)
	{
		___m_EntityComponentStore_8 = value;
	}

	inline static int32_t get_offset_of_m_DependencyManager_9() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_DependencyManager_9)); }
	inline ComponentDependencyManager_tAFE29BF05294E4C3CFE1B599D078ACF14D0FAF91 * get_m_DependencyManager_9() const { return ___m_DependencyManager_9; }
	inline ComponentDependencyManager_tAFE29BF05294E4C3CFE1B599D078ACF14D0FAF91 ** get_address_of_m_DependencyManager_9() { return &___m_DependencyManager_9; }
	inline void set_m_DependencyManager_9(ComponentDependencyManager_tAFE29BF05294E4C3CFE1B599D078ACF14D0FAF91 * value)
	{
		___m_DependencyManager_9 = value;
	}

	inline static int32_t get_offset_of_m_World_10() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_World_10)); }
	inline GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603  get_m_World_10() const { return ___m_World_10; }
	inline GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 * get_address_of_m_World_10() { return &___m_World_10; }
	inline void set_m_World_10(GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603  value)
	{
		___m_World_10 = value;
	}

	inline static int32_t get_offset_of_m_AlwaysUpdateSystem_11() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_AlwaysUpdateSystem_11)); }
	inline bool get_m_AlwaysUpdateSystem_11() const { return ___m_AlwaysUpdateSystem_11; }
	inline bool* get_address_of_m_AlwaysUpdateSystem_11() { return &___m_AlwaysUpdateSystem_11; }
	inline void set_m_AlwaysUpdateSystem_11(bool value)
	{
		___m_AlwaysUpdateSystem_11 = value;
	}

	inline static int32_t get_offset_of_m_Enabled_12() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_Enabled_12)); }
	inline bool get_m_Enabled_12() const { return ___m_Enabled_12; }
	inline bool* get_address_of_m_Enabled_12() { return &___m_Enabled_12; }
	inline void set_m_Enabled_12(bool value)
	{
		___m_Enabled_12 = value;
	}

	inline static int32_t get_offset_of_m_PreviouslyEnabled_13() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_PreviouslyEnabled_13)); }
	inline bool get_m_PreviouslyEnabled_13() const { return ___m_PreviouslyEnabled_13; }
	inline bool* get_address_of_m_PreviouslyEnabled_13() { return &___m_PreviouslyEnabled_13; }
	inline void set_m_PreviouslyEnabled_13(bool value)
	{
		___m_PreviouslyEnabled_13 = value;
	}

	inline static int32_t get_offset_of_m_GetDependencyFromSafetyManager_14() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_GetDependencyFromSafetyManager_14)); }
	inline bool get_m_GetDependencyFromSafetyManager_14() const { return ___m_GetDependencyFromSafetyManager_14; }
	inline bool* get_address_of_m_GetDependencyFromSafetyManager_14() { return &___m_GetDependencyFromSafetyManager_14; }
	inline void set_m_GetDependencyFromSafetyManager_14(bool value)
	{
		___m_GetDependencyFromSafetyManager_14 = value;
	}

	inline static int32_t get_offset_of_m_JobHandle_15() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_JobHandle_15)); }
	inline JobHandle_t8AEB8D31C25D7774C71D62B0C662525E6E36D847  get_m_JobHandle_15() const { return ___m_JobHandle_15; }
	inline JobHandle_t8AEB8D31C25D7774C71D62B0C662525E6E36D847 * get_address_of_m_JobHandle_15() { return &___m_JobHandle_15; }
	inline void set_m_JobHandle_15(JobHandle_t8AEB8D31C25D7774C71D62B0C662525E6E36D847  value)
	{
		___m_JobHandle_15 = value;
	}

	inline static int32_t get_offset_of_m_ProfilerMarker_16() { return static_cast<int32_t>(offsetof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95, ___m_ProfilerMarker_16)); }
	inline ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  get_m_ProfilerMarker_16() const { return ___m_ProfilerMarker_16; }
	inline ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1 * get_address_of_m_ProfilerMarker_16() { return &___m_ProfilerMarker_16; }
	inline void set_m_ProfilerMarker_16(ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  value)
	{
		___m_ProfilerMarker_16 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.SystemState
struct SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95_marshaled_pinvoke
{
	int32_t ___m_UnmanagedMetaIndex_0;
	void* ___m_SystemPtr_1;
	UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA  ___m_EntityQueries_2;
	UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA  ___m_RequiredEntityQueries_3;
	UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B  ___m_JobDependencyForReadingSystems_4;
	UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B  ___m_JobDependencyForWritingSystems_5;
	uint32_t ___m_LastSystemVersion_6;
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_marshaled_pinvoke ___m_EntityManager_7;
	EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___m_EntityComponentStore_8;
	ComponentDependencyManager_tAFE29BF05294E4C3CFE1B599D078ACF14D0FAF91 * ___m_DependencyManager_9;
	GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603  ___m_World_10;
	int32_t ___m_AlwaysUpdateSystem_11;
	int32_t ___m_Enabled_12;
	int32_t ___m_PreviouslyEnabled_13;
	int32_t ___m_GetDependencyFromSafetyManager_14;
	JobHandle_t8AEB8D31C25D7774C71D62B0C662525E6E36D847  ___m_JobHandle_15;
	ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  ___m_ProfilerMarker_16;
};
// Native definition for COM marshalling of Unity.Entities.SystemState
struct SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95_marshaled_com
{
	int32_t ___m_UnmanagedMetaIndex_0;
	void* ___m_SystemPtr_1;
	UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA  ___m_EntityQueries_2;
	UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA  ___m_RequiredEntityQueries_3;
	UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B  ___m_JobDependencyForReadingSystems_4;
	UnsafeIntList_t9DD6A58803E6EC9DCAF3BCFF1F5B9BE471F6A29B  ___m_JobDependencyForWritingSystems_5;
	uint32_t ___m_LastSystemVersion_6;
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_marshaled_com ___m_EntityManager_7;
	EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___m_EntityComponentStore_8;
	ComponentDependencyManager_tAFE29BF05294E4C3CFE1B599D078ACF14D0FAF91 * ___m_DependencyManager_9;
	GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603  ___m_World_10;
	int32_t ___m_AlwaysUpdateSystem_11;
	int32_t ___m_Enabled_12;
	int32_t ___m_PreviouslyEnabled_13;
	int32_t ___m_GetDependencyFromSafetyManager_14;
	JobHandle_t8AEB8D31C25D7774C71D62B0C662525E6E36D847  ___m_JobHandle_15;
	ProfilerMarker_tAE86534C80C5D67768DB3B244D8D139A2E6495E1  ___m_ProfilerMarker_16;
};

// Unity.Entities.World
struct  World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07  : public RuntimeObject
{
public:
	// System.Collections.Generic.Dictionary`2<System.Type,Unity.Entities.ComponentSystemBase> Unity.Entities.World::m_SystemLookup
	Dictionary_2_tD6066C27E39214D9F5B08C241F9C5FB62A7B63B3 * ___m_SystemLookup_2;
	// Unity.Entities.World_StateAllocator Unity.Entities.World::m_StateMemory
	StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB  ___m_StateMemory_4;
	// Unity.Collections.LowLevel.Unsafe.UnsafeMultiHashMap`2<System.Int64,System.UInt16> Unity.Entities.World::m_UnmanagedSlotByTypeHash
	UnsafeMultiHashMap_2_tFDF632D7E17D69CF95BBF036CEBC421A871BF004  ___m_UnmanagedSlotByTypeHash_5;
	// System.Collections.Generic.List`1<Unity.Entities.ComponentSystemBase> Unity.Entities.World::m_Systems
	List_1_t3F15B202D7C7B7EEB8B86FFA8C5D2156C6F32C78 * ___m_Systems_6;
	// Unity.Entities.World_NoAllocReadOnlyCollection`1<Unity.Entities.ComponentSystemBase> Unity.Entities.World::<Systems>k__BackingField
	NoAllocReadOnlyCollection_1_t3ECE9AA8150FFEF9B06892853E38309BF4091FB5  ___U3CSystemsU3Ek__BackingField_7;
	// Unity.Entities.EntityManager Unity.Entities.World::m_EntityManager
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  ___m_EntityManager_8;
	// System.UInt64 Unity.Entities.World::m_SequenceNumber
	uint64_t ___m_SequenceNumber_9;
	// Unity.Entities.WorldFlags Unity.Entities.World::Flags
	uint8_t ___Flags_12;
	// System.String Unity.Entities.World::<Name>k__BackingField
	String_t* ___U3CNameU3Ek__BackingField_13;
	// System.Int32 Unity.Entities.World::<Version>k__BackingField
	int32_t ___U3CVersionU3Ek__BackingField_14;
	// Unity.Core.TimeData Unity.Entities.World::m_CurrentTime
	TimeData_t1892793CB71746290FBEED4D53C605AF3A3FA91F  ___m_CurrentTime_15;
	// Unity.Entities.EntityQuery Unity.Entities.World::m_TimeSingletonQuery
	EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___m_TimeSingletonQuery_16;
	// System.UInt32 Unity.Entities.World::m_WorldId
	uint32_t ___m_WorldId_18;
	// System.Boolean Unity.Entities.World::<QuitUpdate>k__BackingField
	bool ___U3CQuitUpdateU3Ek__BackingField_19;

public:
	inline static int32_t get_offset_of_m_SystemLookup_2() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07, ___m_SystemLookup_2)); }
	inline Dictionary_2_tD6066C27E39214D9F5B08C241F9C5FB62A7B63B3 * get_m_SystemLookup_2() const { return ___m_SystemLookup_2; }
	inline Dictionary_2_tD6066C27E39214D9F5B08C241F9C5FB62A7B63B3 ** get_address_of_m_SystemLookup_2() { return &___m_SystemLookup_2; }
	inline void set_m_SystemLookup_2(Dictionary_2_tD6066C27E39214D9F5B08C241F9C5FB62A7B63B3 * value)
	{
		___m_SystemLookup_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_SystemLookup_2), (void*)value);
	}

	inline static int32_t get_offset_of_m_StateMemory_4() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07, ___m_StateMemory_4)); }
	inline StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB  get_m_StateMemory_4() const { return ___m_StateMemory_4; }
	inline StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * get_address_of_m_StateMemory_4() { return &___m_StateMemory_4; }
	inline void set_m_StateMemory_4(StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB  value)
	{
		___m_StateMemory_4 = value;
	}

	inline static int32_t get_offset_of_m_UnmanagedSlotByTypeHash_5() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07, ___m_UnmanagedSlotByTypeHash_5)); }
	inline UnsafeMultiHashMap_2_tFDF632D7E17D69CF95BBF036CEBC421A871BF004  get_m_UnmanagedSlotByTypeHash_5() const { return ___m_UnmanagedSlotByTypeHash_5; }
	inline UnsafeMultiHashMap_2_tFDF632D7E17D69CF95BBF036CEBC421A871BF004 * get_address_of_m_UnmanagedSlotByTypeHash_5() { return &___m_UnmanagedSlotByTypeHash_5; }
	inline void set_m_UnmanagedSlotByTypeHash_5(UnsafeMultiHashMap_2_tFDF632D7E17D69CF95BBF036CEBC421A871BF004  value)
	{
		___m_UnmanagedSlotByTypeHash_5 = value;
	}

	inline static int32_t get_offset_of_m_Systems_6() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07, ___m_Systems_6)); }
	inline List_1_t3F15B202D7C7B7EEB8B86FFA8C5D2156C6F32C78 * get_m_Systems_6() const { return ___m_Systems_6; }
	inline List_1_t3F15B202D7C7B7EEB8B86FFA8C5D2156C6F32C78 ** get_address_of_m_Systems_6() { return &___m_Systems_6; }
	inline void set_m_Systems_6(List_1_t3F15B202D7C7B7EEB8B86FFA8C5D2156C6F32C78 * value)
	{
		___m_Systems_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Systems_6), (void*)value);
	}

	inline static int32_t get_offset_of_U3CSystemsU3Ek__BackingField_7() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07, ___U3CSystemsU3Ek__BackingField_7)); }
	inline NoAllocReadOnlyCollection_1_t3ECE9AA8150FFEF9B06892853E38309BF4091FB5  get_U3CSystemsU3Ek__BackingField_7() const { return ___U3CSystemsU3Ek__BackingField_7; }
	inline NoAllocReadOnlyCollection_1_t3ECE9AA8150FFEF9B06892853E38309BF4091FB5 * get_address_of_U3CSystemsU3Ek__BackingField_7() { return &___U3CSystemsU3Ek__BackingField_7; }
	inline void set_U3CSystemsU3Ek__BackingField_7(NoAllocReadOnlyCollection_1_t3ECE9AA8150FFEF9B06892853E38309BF4091FB5  value)
	{
		___U3CSystemsU3Ek__BackingField_7 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___U3CSystemsU3Ek__BackingField_7))->___m_Source_0), (void*)NULL);
	}

	inline static int32_t get_offset_of_m_EntityManager_8() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07, ___m_EntityManager_8)); }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  get_m_EntityManager_8() const { return ___m_EntityManager_8; }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0 * get_address_of_m_EntityManager_8() { return &___m_EntityManager_8; }
	inline void set_m_EntityManager_8(EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  value)
	{
		___m_EntityManager_8 = value;
	}

	inline static int32_t get_offset_of_m_SequenceNumber_9() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07, ___m_SequenceNumber_9)); }
	inline uint64_t get_m_SequenceNumber_9() const { return ___m_SequenceNumber_9; }
	inline uint64_t* get_address_of_m_SequenceNumber_9() { return &___m_SequenceNumber_9; }
	inline void set_m_SequenceNumber_9(uint64_t value)
	{
		___m_SequenceNumber_9 = value;
	}

	inline static int32_t get_offset_of_Flags_12() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07, ___Flags_12)); }
	inline uint8_t get_Flags_12() const { return ___Flags_12; }
	inline uint8_t* get_address_of_Flags_12() { return &___Flags_12; }
	inline void set_Flags_12(uint8_t value)
	{
		___Flags_12 = value;
	}

	inline static int32_t get_offset_of_U3CNameU3Ek__BackingField_13() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07, ___U3CNameU3Ek__BackingField_13)); }
	inline String_t* get_U3CNameU3Ek__BackingField_13() const { return ___U3CNameU3Ek__BackingField_13; }
	inline String_t** get_address_of_U3CNameU3Ek__BackingField_13() { return &___U3CNameU3Ek__BackingField_13; }
	inline void set_U3CNameU3Ek__BackingField_13(String_t* value)
	{
		___U3CNameU3Ek__BackingField_13 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CNameU3Ek__BackingField_13), (void*)value);
	}

	inline static int32_t get_offset_of_U3CVersionU3Ek__BackingField_14() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07, ___U3CVersionU3Ek__BackingField_14)); }
	inline int32_t get_U3CVersionU3Ek__BackingField_14() const { return ___U3CVersionU3Ek__BackingField_14; }
	inline int32_t* get_address_of_U3CVersionU3Ek__BackingField_14() { return &___U3CVersionU3Ek__BackingField_14; }
	inline void set_U3CVersionU3Ek__BackingField_14(int32_t value)
	{
		___U3CVersionU3Ek__BackingField_14 = value;
	}

	inline static int32_t get_offset_of_m_CurrentTime_15() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07, ___m_CurrentTime_15)); }
	inline TimeData_t1892793CB71746290FBEED4D53C605AF3A3FA91F  get_m_CurrentTime_15() const { return ___m_CurrentTime_15; }
	inline TimeData_t1892793CB71746290FBEED4D53C605AF3A3FA91F * get_address_of_m_CurrentTime_15() { return &___m_CurrentTime_15; }
	inline void set_m_CurrentTime_15(TimeData_t1892793CB71746290FBEED4D53C605AF3A3FA91F  value)
	{
		___m_CurrentTime_15 = value;
	}

	inline static int32_t get_offset_of_m_TimeSingletonQuery_16() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07, ___m_TimeSingletonQuery_16)); }
	inline EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  get_m_TimeSingletonQuery_16() const { return ___m_TimeSingletonQuery_16; }
	inline EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109 * get_address_of_m_TimeSingletonQuery_16() { return &___m_TimeSingletonQuery_16; }
	inline void set_m_TimeSingletonQuery_16(EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  value)
	{
		___m_TimeSingletonQuery_16 = value;
	}

	inline static int32_t get_offset_of_m_WorldId_18() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07, ___m_WorldId_18)); }
	inline uint32_t get_m_WorldId_18() const { return ___m_WorldId_18; }
	inline uint32_t* get_address_of_m_WorldId_18() { return &___m_WorldId_18; }
	inline void set_m_WorldId_18(uint32_t value)
	{
		___m_WorldId_18 = value;
	}

	inline static int32_t get_offset_of_U3CQuitUpdateU3Ek__BackingField_19() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07, ___U3CQuitUpdateU3Ek__BackingField_19)); }
	inline bool get_U3CQuitUpdateU3Ek__BackingField_19() const { return ___U3CQuitUpdateU3Ek__BackingField_19; }
	inline bool* get_address_of_U3CQuitUpdateU3Ek__BackingField_19() { return &___U3CQuitUpdateU3Ek__BackingField_19; }
	inline void set_U3CQuitUpdateU3Ek__BackingField_19(bool value)
	{
		___U3CQuitUpdateU3Ek__BackingField_19 = value;
	}
};

struct World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07_StaticFields
{
public:
	// System.Collections.Generic.List`1<Unity.Entities.World> Unity.Entities.World::s_AllWorlds
	List_1_t84AC7439EE119A84B7D9198C8EB4AC483255C192 * ___s_AllWorlds_0;
	// Unity.Entities.World Unity.Entities.World::<DefaultGameObjectInjectionWorld>k__BackingField
	World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07 * ___U3CDefaultGameObjectInjectionWorldU3Ek__BackingField_1;
	// Unity.Entities.World_NoAllocReadOnlyCollection`1<Unity.Entities.World> Unity.Entities.World::<All>k__BackingField
	NoAllocReadOnlyCollection_1_t80544BA80B59053667CC4B79E3461635AA6E5EE4  ___U3CAllU3Ek__BackingField_3;
	// System.Int32 Unity.Entities.World::ms_SystemIDAllocator
	int32_t ___ms_SystemIDAllocator_10;
	// Unity.Burst.SharedStatic`1<System.UInt64> Unity.Entities.World::ms_NextSequenceNumber
	SharedStatic_1_t3BF4833AD74181586FD1838757D84AD9921C1258  ___ms_NextSequenceNumber_11;
	// System.UInt32 Unity.Entities.World::s_WorldId
	uint32_t ___s_WorldId_17;

public:
	inline static int32_t get_offset_of_s_AllWorlds_0() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07_StaticFields, ___s_AllWorlds_0)); }
	inline List_1_t84AC7439EE119A84B7D9198C8EB4AC483255C192 * get_s_AllWorlds_0() const { return ___s_AllWorlds_0; }
	inline List_1_t84AC7439EE119A84B7D9198C8EB4AC483255C192 ** get_address_of_s_AllWorlds_0() { return &___s_AllWorlds_0; }
	inline void set_s_AllWorlds_0(List_1_t84AC7439EE119A84B7D9198C8EB4AC483255C192 * value)
	{
		___s_AllWorlds_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_AllWorlds_0), (void*)value);
	}

	inline static int32_t get_offset_of_U3CDefaultGameObjectInjectionWorldU3Ek__BackingField_1() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07_StaticFields, ___U3CDefaultGameObjectInjectionWorldU3Ek__BackingField_1)); }
	inline World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07 * get_U3CDefaultGameObjectInjectionWorldU3Ek__BackingField_1() const { return ___U3CDefaultGameObjectInjectionWorldU3Ek__BackingField_1; }
	inline World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07 ** get_address_of_U3CDefaultGameObjectInjectionWorldU3Ek__BackingField_1() { return &___U3CDefaultGameObjectInjectionWorldU3Ek__BackingField_1; }
	inline void set_U3CDefaultGameObjectInjectionWorldU3Ek__BackingField_1(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07 * value)
	{
		___U3CDefaultGameObjectInjectionWorldU3Ek__BackingField_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CDefaultGameObjectInjectionWorldU3Ek__BackingField_1), (void*)value);
	}

	inline static int32_t get_offset_of_U3CAllU3Ek__BackingField_3() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07_StaticFields, ___U3CAllU3Ek__BackingField_3)); }
	inline NoAllocReadOnlyCollection_1_t80544BA80B59053667CC4B79E3461635AA6E5EE4  get_U3CAllU3Ek__BackingField_3() const { return ___U3CAllU3Ek__BackingField_3; }
	inline NoAllocReadOnlyCollection_1_t80544BA80B59053667CC4B79E3461635AA6E5EE4 * get_address_of_U3CAllU3Ek__BackingField_3() { return &___U3CAllU3Ek__BackingField_3; }
	inline void set_U3CAllU3Ek__BackingField_3(NoAllocReadOnlyCollection_1_t80544BA80B59053667CC4B79E3461635AA6E5EE4  value)
	{
		___U3CAllU3Ek__BackingField_3 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___U3CAllU3Ek__BackingField_3))->___m_Source_0), (void*)NULL);
	}

	inline static int32_t get_offset_of_ms_SystemIDAllocator_10() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07_StaticFields, ___ms_SystemIDAllocator_10)); }
	inline int32_t get_ms_SystemIDAllocator_10() const { return ___ms_SystemIDAllocator_10; }
	inline int32_t* get_address_of_ms_SystemIDAllocator_10() { return &___ms_SystemIDAllocator_10; }
	inline void set_ms_SystemIDAllocator_10(int32_t value)
	{
		___ms_SystemIDAllocator_10 = value;
	}

	inline static int32_t get_offset_of_ms_NextSequenceNumber_11() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07_StaticFields, ___ms_NextSequenceNumber_11)); }
	inline SharedStatic_1_t3BF4833AD74181586FD1838757D84AD9921C1258  get_ms_NextSequenceNumber_11() const { return ___ms_NextSequenceNumber_11; }
	inline SharedStatic_1_t3BF4833AD74181586FD1838757D84AD9921C1258 * get_address_of_ms_NextSequenceNumber_11() { return &___ms_NextSequenceNumber_11; }
	inline void set_ms_NextSequenceNumber_11(SharedStatic_1_t3BF4833AD74181586FD1838757D84AD9921C1258  value)
	{
		___ms_NextSequenceNumber_11 = value;
	}

	inline static int32_t get_offset_of_s_WorldId_17() { return static_cast<int32_t>(offsetof(World_tC5274CBB5238670C9A07FA5705DD82B5196C9A07_StaticFields, ___s_WorldId_17)); }
	inline uint32_t get_s_WorldId_17() const { return ___s_WorldId_17; }
	inline uint32_t* get_address_of_s_WorldId_17() { return &___s_WorldId_17; }
	inline void set_s_WorldId_17(uint32_t value)
	{
		___s_WorldId_17 = value;
	}
};


// Unity.Collections.NativeMultiHashMapIterator`1<Unity.Entities.EntityPatcher_EntityComponentPair>
struct  NativeMultiHashMapIterator_1_t6B2775FD56E4580AACD742CE7830FE0C58F0EE12 
{
public:
	// TKey Unity.Collections.NativeMultiHashMapIterator`1::key
	EntityComponentPair_tF1A2F420287859D321D85CB20D9846DB9101294C  ___key_0;
	// System.Int32 Unity.Collections.NativeMultiHashMapIterator`1::NextEntryIndex
	int32_t ___NextEntryIndex_1;
	// System.Int32 Unity.Collections.NativeMultiHashMapIterator`1::EntryIndex
	int32_t ___EntryIndex_2;

public:
	inline static int32_t get_offset_of_key_0() { return static_cast<int32_t>(offsetof(NativeMultiHashMapIterator_1_t6B2775FD56E4580AACD742CE7830FE0C58F0EE12, ___key_0)); }
	inline EntityComponentPair_tF1A2F420287859D321D85CB20D9846DB9101294C  get_key_0() const { return ___key_0; }
	inline EntityComponentPair_tF1A2F420287859D321D85CB20D9846DB9101294C * get_address_of_key_0() { return &___key_0; }
	inline void set_key_0(EntityComponentPair_tF1A2F420287859D321D85CB20D9846DB9101294C  value)
	{
		___key_0 = value;
	}

	inline static int32_t get_offset_of_NextEntryIndex_1() { return static_cast<int32_t>(offsetof(NativeMultiHashMapIterator_1_t6B2775FD56E4580AACD742CE7830FE0C58F0EE12, ___NextEntryIndex_1)); }
	inline int32_t get_NextEntryIndex_1() const { return ___NextEntryIndex_1; }
	inline int32_t* get_address_of_NextEntryIndex_1() { return &___NextEntryIndex_1; }
	inline void set_NextEntryIndex_1(int32_t value)
	{
		___NextEntryIndex_1 = value;
	}

	inline static int32_t get_offset_of_EntryIndex_2() { return static_cast<int32_t>(offsetof(NativeMultiHashMapIterator_1_t6B2775FD56E4580AACD742CE7830FE0C58F0EE12, ___EntryIndex_2)); }
	inline int32_t get_EntryIndex_2() const { return ___EntryIndex_2; }
	inline int32_t* get_address_of_EntryIndex_2() { return &___EntryIndex_2; }
	inline void set_EntryIndex_2(int32_t value)
	{
		___EntryIndex_2 = value;
	}
};


// Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider
struct  StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994 
{
public:
	// Unity.Entities.EntityManager Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider::_manager
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  ____manager_0;
	// Unity.Entities.EntityQuery Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider::_query
	EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ____query_1;
	// Unity.Entities.EntityQuery_GatherEntitiesResult Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider::_gatherEntitiesResult
	GatherEntitiesResult_t731EB8DD7C87CA907966DC48C8786447A5E39B4E  ____gatherEntitiesResult_2;

public:
	inline static int32_t get_offset_of__manager_0() { return static_cast<int32_t>(offsetof(StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994, ____manager_0)); }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  get__manager_0() const { return ____manager_0; }
	inline EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0 * get_address_of__manager_0() { return &____manager_0; }
	inline void set__manager_0(EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0  value)
	{
		____manager_0 = value;
	}

	inline static int32_t get_offset_of__query_1() { return static_cast<int32_t>(offsetof(StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994, ____query_1)); }
	inline EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  get__query_1() const { return ____query_1; }
	inline EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109 * get_address_of__query_1() { return &____query_1; }
	inline void set__query_1(EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  value)
	{
		____query_1 = value;
	}

	inline static int32_t get_offset_of__gatherEntitiesResult_2() { return static_cast<int32_t>(offsetof(StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994, ____gatherEntitiesResult_2)); }
	inline GatherEntitiesResult_t731EB8DD7C87CA907966DC48C8786447A5E39B4E  get__gatherEntitiesResult_2() const { return ____gatherEntitiesResult_2; }
	inline GatherEntitiesResult_t731EB8DD7C87CA907966DC48C8786447A5E39B4E * get_address_of__gatherEntitiesResult_2() { return &____gatherEntitiesResult_2; }
	inline void set__gatherEntitiesResult_2(GatherEntitiesResult_t731EB8DD7C87CA907966DC48C8786447A5E39B4E  value)
	{
		____gatherEntitiesResult_2 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider
struct StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_pinvoke
{
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_marshaled_pinvoke ____manager_0;
	EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109_marshaled_pinvoke ____query_1;
	GatherEntitiesResult_t731EB8DD7C87CA907966DC48C8786447A5E39B4E  ____gatherEntitiesResult_2;
};
// Native definition for COM marshalling of Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider
struct StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_com
{
	EntityManager_t375E301E0D409D57A32EB6EEFEE4DCFA936B3FD0_marshaled_com ____manager_0;
	EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109_marshaled_com ____query_1;
	GatherEntitiesResult_t731EB8DD7C87CA907966DC48C8786447A5E39B4E  ____gatherEntitiesResult_2;
};

// Unity.Entities.EntityComponentStore
struct  EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA 
{
public:
	// System.Int32* Unity.Entities.EntityComponentStore::m_VersionByEntity
	int32_t* ___m_VersionByEntity_0;
	// Unity.Entities.Archetype** Unity.Entities.EntityComponentStore::m_ArchetypeByEntity
	Archetype_t14B6BCF53EF0F918C8B503D7100F6217E480CD94 ** ___m_ArchetypeByEntity_1;
	// Unity.Entities.EntityInChunk* Unity.Entities.EntityComponentStore::m_EntityInChunkByEntity
	EntityInChunk_tB28DC9CD18AA63C3C99B85007BC5009DFF9F4EA3 * ___m_EntityInChunkByEntity_2;
	// System.Int32* Unity.Entities.EntityComponentStore::m_ComponentTypeOrderVersion
	int32_t* ___m_ComponentTypeOrderVersion_3;
	// Unity.Entities.BlockAllocator Unity.Entities.EntityComponentStore::m_ArchetypeChunkAllocator
	BlockAllocator_t43F1EF54B5CF45084293051ACE1D0AD8F18DAB07  ___m_ArchetypeChunkAllocator_4;
	// Unity.Entities.UnsafeChunkPtrList Unity.Entities.EntityComponentStore::m_EmptyChunks
	UnsafeChunkPtrList_tF6FE39C1771CD6E6E5CA3B86195038FECD946D58  ___m_EmptyChunks_5;
	// Unity.Entities.UnsafeArchetypePtrList Unity.Entities.EntityComponentStore::m_Archetypes
	UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7  ___m_Archetypes_6;
	// Unity.Entities.ArchetypeListMap Unity.Entities.EntityComponentStore::m_TypeLookup
	ArchetypeListMap_tB86167946EED7570CC5D89081659CE9A7B6A0847  ___m_TypeLookup_7;
	// System.Int32 Unity.Entities.EntityComponentStore::m_ManagedComponentIndex
	int32_t ___m_ManagedComponentIndex_8;
	// System.Int32 Unity.Entities.EntityComponentStore::m_ManagedComponentIndexCapacity
	int32_t ___m_ManagedComponentIndexCapacity_9;
	// Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer Unity.Entities.EntityComponentStore::m_ManagedComponentFreeIndex
	UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C  ___m_ManagedComponentFreeIndex_10;
	// Unity.Entities.ManagedDeferredCommands Unity.Entities.EntityComponentStore::ManagedChangesTracker
	ManagedDeferredCommands_tD9C6F59E91B0E55519A28C84158F57A14B1CEFC5  ___ManagedChangesTracker_11;
	// System.UInt64 Unity.Entities.EntityComponentStore::m_NextChunkSequenceNumber
	uint64_t ___m_NextChunkSequenceNumber_12;
	// System.Int32 Unity.Entities.EntityComponentStore::m_NextFreeEntityIndex
	int32_t ___m_NextFreeEntityIndex_13;
	// System.UInt32 Unity.Entities.EntityComponentStore::m_GlobalSystemVersion
	uint32_t ___m_GlobalSystemVersion_14;
	// System.Int32 Unity.Entities.EntityComponentStore::m_EntitiesCapacity
	int32_t ___m_EntitiesCapacity_15;
	// System.UInt32 Unity.Entities.EntityComponentStore::m_ArchetypeTrackingVersion
	uint32_t ___m_ArchetypeTrackingVersion_16;
	// System.Int32 Unity.Entities.EntityComponentStore::m_LinkedGroupType
	int32_t ___m_LinkedGroupType_17;
	// System.Int32 Unity.Entities.EntityComponentStore::m_ChunkHeaderType
	int32_t ___m_ChunkHeaderType_18;
	// System.Int32 Unity.Entities.EntityComponentStore::m_PrefabType
	int32_t ___m_PrefabType_19;
	// System.Int32 Unity.Entities.EntityComponentStore::m_CleanupEntityType
	int32_t ___m_CleanupEntityType_20;
	// System.Int32 Unity.Entities.EntityComponentStore::m_DisabledType
	int32_t ___m_DisabledType_21;
	// System.Int32 Unity.Entities.EntityComponentStore::m_EntityType
	int32_t ___m_EntityType_22;
	// Unity.Entities.ComponentType Unity.Entities.EntityComponentStore::m_ChunkHeaderComponentType
	ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370  ___m_ChunkHeaderComponentType_23;
	// Unity.Entities.ComponentType Unity.Entities.EntityComponentStore::m_EntityComponentType
	ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370  ___m_EntityComponentType_24;
	// Unity.Entities.TypeManager_TypeInfo* Unity.Entities.EntityComponentStore::m_TypeInfos
	TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * ___m_TypeInfos_25;
	// Unity.Entities.TypeManager_EntityOffsetInfo* Unity.Entities.EntityComponentStore::m_EntityOffsetInfos
	EntityOffsetInfo_t398BFA31DBBAAA0488E257A761D3D254D716DE89 * ___m_EntityOffsetInfos_26;
	// System.Byte Unity.Entities.EntityComponentStore::memoryInitPattern
	uint8_t ___memoryInitPattern_27;
	// System.Byte Unity.Entities.EntityComponentStore::useMemoryInitPattern
	uint8_t ___useMemoryInitPattern_28;

public:
	inline static int32_t get_offset_of_m_VersionByEntity_0() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_VersionByEntity_0)); }
	inline int32_t* get_m_VersionByEntity_0() const { return ___m_VersionByEntity_0; }
	inline int32_t** get_address_of_m_VersionByEntity_0() { return &___m_VersionByEntity_0; }
	inline void set_m_VersionByEntity_0(int32_t* value)
	{
		___m_VersionByEntity_0 = value;
	}

	inline static int32_t get_offset_of_m_ArchetypeByEntity_1() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_ArchetypeByEntity_1)); }
	inline Archetype_t14B6BCF53EF0F918C8B503D7100F6217E480CD94 ** get_m_ArchetypeByEntity_1() const { return ___m_ArchetypeByEntity_1; }
	inline Archetype_t14B6BCF53EF0F918C8B503D7100F6217E480CD94 *** get_address_of_m_ArchetypeByEntity_1() { return &___m_ArchetypeByEntity_1; }
	inline void set_m_ArchetypeByEntity_1(Archetype_t14B6BCF53EF0F918C8B503D7100F6217E480CD94 ** value)
	{
		___m_ArchetypeByEntity_1 = value;
	}

	inline static int32_t get_offset_of_m_EntityInChunkByEntity_2() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_EntityInChunkByEntity_2)); }
	inline EntityInChunk_tB28DC9CD18AA63C3C99B85007BC5009DFF9F4EA3 * get_m_EntityInChunkByEntity_2() const { return ___m_EntityInChunkByEntity_2; }
	inline EntityInChunk_tB28DC9CD18AA63C3C99B85007BC5009DFF9F4EA3 ** get_address_of_m_EntityInChunkByEntity_2() { return &___m_EntityInChunkByEntity_2; }
	inline void set_m_EntityInChunkByEntity_2(EntityInChunk_tB28DC9CD18AA63C3C99B85007BC5009DFF9F4EA3 * value)
	{
		___m_EntityInChunkByEntity_2 = value;
	}

	inline static int32_t get_offset_of_m_ComponentTypeOrderVersion_3() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_ComponentTypeOrderVersion_3)); }
	inline int32_t* get_m_ComponentTypeOrderVersion_3() const { return ___m_ComponentTypeOrderVersion_3; }
	inline int32_t** get_address_of_m_ComponentTypeOrderVersion_3() { return &___m_ComponentTypeOrderVersion_3; }
	inline void set_m_ComponentTypeOrderVersion_3(int32_t* value)
	{
		___m_ComponentTypeOrderVersion_3 = value;
	}

	inline static int32_t get_offset_of_m_ArchetypeChunkAllocator_4() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_ArchetypeChunkAllocator_4)); }
	inline BlockAllocator_t43F1EF54B5CF45084293051ACE1D0AD8F18DAB07  get_m_ArchetypeChunkAllocator_4() const { return ___m_ArchetypeChunkAllocator_4; }
	inline BlockAllocator_t43F1EF54B5CF45084293051ACE1D0AD8F18DAB07 * get_address_of_m_ArchetypeChunkAllocator_4() { return &___m_ArchetypeChunkAllocator_4; }
	inline void set_m_ArchetypeChunkAllocator_4(BlockAllocator_t43F1EF54B5CF45084293051ACE1D0AD8F18DAB07  value)
	{
		___m_ArchetypeChunkAllocator_4 = value;
	}

	inline static int32_t get_offset_of_m_EmptyChunks_5() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_EmptyChunks_5)); }
	inline UnsafeChunkPtrList_tF6FE39C1771CD6E6E5CA3B86195038FECD946D58  get_m_EmptyChunks_5() const { return ___m_EmptyChunks_5; }
	inline UnsafeChunkPtrList_tF6FE39C1771CD6E6E5CA3B86195038FECD946D58 * get_address_of_m_EmptyChunks_5() { return &___m_EmptyChunks_5; }
	inline void set_m_EmptyChunks_5(UnsafeChunkPtrList_tF6FE39C1771CD6E6E5CA3B86195038FECD946D58  value)
	{
		___m_EmptyChunks_5 = value;
	}

	inline static int32_t get_offset_of_m_Archetypes_6() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_Archetypes_6)); }
	inline UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7  get_m_Archetypes_6() const { return ___m_Archetypes_6; }
	inline UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7 * get_address_of_m_Archetypes_6() { return &___m_Archetypes_6; }
	inline void set_m_Archetypes_6(UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7  value)
	{
		___m_Archetypes_6 = value;
	}

	inline static int32_t get_offset_of_m_TypeLookup_7() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_TypeLookup_7)); }
	inline ArchetypeListMap_tB86167946EED7570CC5D89081659CE9A7B6A0847  get_m_TypeLookup_7() const { return ___m_TypeLookup_7; }
	inline ArchetypeListMap_tB86167946EED7570CC5D89081659CE9A7B6A0847 * get_address_of_m_TypeLookup_7() { return &___m_TypeLookup_7; }
	inline void set_m_TypeLookup_7(ArchetypeListMap_tB86167946EED7570CC5D89081659CE9A7B6A0847  value)
	{
		___m_TypeLookup_7 = value;
	}

	inline static int32_t get_offset_of_m_ManagedComponentIndex_8() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_ManagedComponentIndex_8)); }
	inline int32_t get_m_ManagedComponentIndex_8() const { return ___m_ManagedComponentIndex_8; }
	inline int32_t* get_address_of_m_ManagedComponentIndex_8() { return &___m_ManagedComponentIndex_8; }
	inline void set_m_ManagedComponentIndex_8(int32_t value)
	{
		___m_ManagedComponentIndex_8 = value;
	}

	inline static int32_t get_offset_of_m_ManagedComponentIndexCapacity_9() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_ManagedComponentIndexCapacity_9)); }
	inline int32_t get_m_ManagedComponentIndexCapacity_9() const { return ___m_ManagedComponentIndexCapacity_9; }
	inline int32_t* get_address_of_m_ManagedComponentIndexCapacity_9() { return &___m_ManagedComponentIndexCapacity_9; }
	inline void set_m_ManagedComponentIndexCapacity_9(int32_t value)
	{
		___m_ManagedComponentIndexCapacity_9 = value;
	}

	inline static int32_t get_offset_of_m_ManagedComponentFreeIndex_10() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_ManagedComponentFreeIndex_10)); }
	inline UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C  get_m_ManagedComponentFreeIndex_10() const { return ___m_ManagedComponentFreeIndex_10; }
	inline UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C * get_address_of_m_ManagedComponentFreeIndex_10() { return &___m_ManagedComponentFreeIndex_10; }
	inline void set_m_ManagedComponentFreeIndex_10(UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C  value)
	{
		___m_ManagedComponentFreeIndex_10 = value;
	}

	inline static int32_t get_offset_of_ManagedChangesTracker_11() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___ManagedChangesTracker_11)); }
	inline ManagedDeferredCommands_tD9C6F59E91B0E55519A28C84158F57A14B1CEFC5  get_ManagedChangesTracker_11() const { return ___ManagedChangesTracker_11; }
	inline ManagedDeferredCommands_tD9C6F59E91B0E55519A28C84158F57A14B1CEFC5 * get_address_of_ManagedChangesTracker_11() { return &___ManagedChangesTracker_11; }
	inline void set_ManagedChangesTracker_11(ManagedDeferredCommands_tD9C6F59E91B0E55519A28C84158F57A14B1CEFC5  value)
	{
		___ManagedChangesTracker_11 = value;
	}

	inline static int32_t get_offset_of_m_NextChunkSequenceNumber_12() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_NextChunkSequenceNumber_12)); }
	inline uint64_t get_m_NextChunkSequenceNumber_12() const { return ___m_NextChunkSequenceNumber_12; }
	inline uint64_t* get_address_of_m_NextChunkSequenceNumber_12() { return &___m_NextChunkSequenceNumber_12; }
	inline void set_m_NextChunkSequenceNumber_12(uint64_t value)
	{
		___m_NextChunkSequenceNumber_12 = value;
	}

	inline static int32_t get_offset_of_m_NextFreeEntityIndex_13() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_NextFreeEntityIndex_13)); }
	inline int32_t get_m_NextFreeEntityIndex_13() const { return ___m_NextFreeEntityIndex_13; }
	inline int32_t* get_address_of_m_NextFreeEntityIndex_13() { return &___m_NextFreeEntityIndex_13; }
	inline void set_m_NextFreeEntityIndex_13(int32_t value)
	{
		___m_NextFreeEntityIndex_13 = value;
	}

	inline static int32_t get_offset_of_m_GlobalSystemVersion_14() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_GlobalSystemVersion_14)); }
	inline uint32_t get_m_GlobalSystemVersion_14() const { return ___m_GlobalSystemVersion_14; }
	inline uint32_t* get_address_of_m_GlobalSystemVersion_14() { return &___m_GlobalSystemVersion_14; }
	inline void set_m_GlobalSystemVersion_14(uint32_t value)
	{
		___m_GlobalSystemVersion_14 = value;
	}

	inline static int32_t get_offset_of_m_EntitiesCapacity_15() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_EntitiesCapacity_15)); }
	inline int32_t get_m_EntitiesCapacity_15() const { return ___m_EntitiesCapacity_15; }
	inline int32_t* get_address_of_m_EntitiesCapacity_15() { return &___m_EntitiesCapacity_15; }
	inline void set_m_EntitiesCapacity_15(int32_t value)
	{
		___m_EntitiesCapacity_15 = value;
	}

	inline static int32_t get_offset_of_m_ArchetypeTrackingVersion_16() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_ArchetypeTrackingVersion_16)); }
	inline uint32_t get_m_ArchetypeTrackingVersion_16() const { return ___m_ArchetypeTrackingVersion_16; }
	inline uint32_t* get_address_of_m_ArchetypeTrackingVersion_16() { return &___m_ArchetypeTrackingVersion_16; }
	inline void set_m_ArchetypeTrackingVersion_16(uint32_t value)
	{
		___m_ArchetypeTrackingVersion_16 = value;
	}

	inline static int32_t get_offset_of_m_LinkedGroupType_17() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_LinkedGroupType_17)); }
	inline int32_t get_m_LinkedGroupType_17() const { return ___m_LinkedGroupType_17; }
	inline int32_t* get_address_of_m_LinkedGroupType_17() { return &___m_LinkedGroupType_17; }
	inline void set_m_LinkedGroupType_17(int32_t value)
	{
		___m_LinkedGroupType_17 = value;
	}

	inline static int32_t get_offset_of_m_ChunkHeaderType_18() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_ChunkHeaderType_18)); }
	inline int32_t get_m_ChunkHeaderType_18() const { return ___m_ChunkHeaderType_18; }
	inline int32_t* get_address_of_m_ChunkHeaderType_18() { return &___m_ChunkHeaderType_18; }
	inline void set_m_ChunkHeaderType_18(int32_t value)
	{
		___m_ChunkHeaderType_18 = value;
	}

	inline static int32_t get_offset_of_m_PrefabType_19() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_PrefabType_19)); }
	inline int32_t get_m_PrefabType_19() const { return ___m_PrefabType_19; }
	inline int32_t* get_address_of_m_PrefabType_19() { return &___m_PrefabType_19; }
	inline void set_m_PrefabType_19(int32_t value)
	{
		___m_PrefabType_19 = value;
	}

	inline static int32_t get_offset_of_m_CleanupEntityType_20() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_CleanupEntityType_20)); }
	inline int32_t get_m_CleanupEntityType_20() const { return ___m_CleanupEntityType_20; }
	inline int32_t* get_address_of_m_CleanupEntityType_20() { return &___m_CleanupEntityType_20; }
	inline void set_m_CleanupEntityType_20(int32_t value)
	{
		___m_CleanupEntityType_20 = value;
	}

	inline static int32_t get_offset_of_m_DisabledType_21() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_DisabledType_21)); }
	inline int32_t get_m_DisabledType_21() const { return ___m_DisabledType_21; }
	inline int32_t* get_address_of_m_DisabledType_21() { return &___m_DisabledType_21; }
	inline void set_m_DisabledType_21(int32_t value)
	{
		___m_DisabledType_21 = value;
	}

	inline static int32_t get_offset_of_m_EntityType_22() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_EntityType_22)); }
	inline int32_t get_m_EntityType_22() const { return ___m_EntityType_22; }
	inline int32_t* get_address_of_m_EntityType_22() { return &___m_EntityType_22; }
	inline void set_m_EntityType_22(int32_t value)
	{
		___m_EntityType_22 = value;
	}

	inline static int32_t get_offset_of_m_ChunkHeaderComponentType_23() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_ChunkHeaderComponentType_23)); }
	inline ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370  get_m_ChunkHeaderComponentType_23() const { return ___m_ChunkHeaderComponentType_23; }
	inline ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370 * get_address_of_m_ChunkHeaderComponentType_23() { return &___m_ChunkHeaderComponentType_23; }
	inline void set_m_ChunkHeaderComponentType_23(ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370  value)
	{
		___m_ChunkHeaderComponentType_23 = value;
	}

	inline static int32_t get_offset_of_m_EntityComponentType_24() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_EntityComponentType_24)); }
	inline ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370  get_m_EntityComponentType_24() const { return ___m_EntityComponentType_24; }
	inline ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370 * get_address_of_m_EntityComponentType_24() { return &___m_EntityComponentType_24; }
	inline void set_m_EntityComponentType_24(ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370  value)
	{
		___m_EntityComponentType_24 = value;
	}

	inline static int32_t get_offset_of_m_TypeInfos_25() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_TypeInfos_25)); }
	inline TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * get_m_TypeInfos_25() const { return ___m_TypeInfos_25; }
	inline TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 ** get_address_of_m_TypeInfos_25() { return &___m_TypeInfos_25; }
	inline void set_m_TypeInfos_25(TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * value)
	{
		___m_TypeInfos_25 = value;
	}

	inline static int32_t get_offset_of_m_EntityOffsetInfos_26() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___m_EntityOffsetInfos_26)); }
	inline EntityOffsetInfo_t398BFA31DBBAAA0488E257A761D3D254D716DE89 * get_m_EntityOffsetInfos_26() const { return ___m_EntityOffsetInfos_26; }
	inline EntityOffsetInfo_t398BFA31DBBAAA0488E257A761D3D254D716DE89 ** get_address_of_m_EntityOffsetInfos_26() { return &___m_EntityOffsetInfos_26; }
	inline void set_m_EntityOffsetInfos_26(EntityOffsetInfo_t398BFA31DBBAAA0488E257A761D3D254D716DE89 * value)
	{
		___m_EntityOffsetInfos_26 = value;
	}

	inline static int32_t get_offset_of_memoryInitPattern_27() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___memoryInitPattern_27)); }
	inline uint8_t get_memoryInitPattern_27() const { return ___memoryInitPattern_27; }
	inline uint8_t* get_address_of_memoryInitPattern_27() { return &___memoryInitPattern_27; }
	inline void set_memoryInitPattern_27(uint8_t value)
	{
		___memoryInitPattern_27 = value;
	}

	inline static int32_t get_offset_of_useMemoryInitPattern_28() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA, ___useMemoryInitPattern_28)); }
	inline uint8_t get_useMemoryInitPattern_28() const { return ___useMemoryInitPattern_28; }
	inline uint8_t* get_address_of_useMemoryInitPattern_28() { return &___useMemoryInitPattern_28; }
	inline void set_useMemoryInitPattern_28(uint8_t value)
	{
		___useMemoryInitPattern_28 = value;
	}
};

struct EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA_StaticFields
{
public:
	// Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore_ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore
	SharedStatic_1_t8A0B77C3063A5BD031494FB6E0ACE2DC30197C8F  ___s_chunkStore_32;

public:
	inline static int32_t get_offset_of_s_chunkStore_32() { return static_cast<int32_t>(offsetof(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA_StaticFields, ___s_chunkStore_32)); }
	inline SharedStatic_1_t8A0B77C3063A5BD031494FB6E0ACE2DC30197C8F  get_s_chunkStore_32() const { return ___s_chunkStore_32; }
	inline SharedStatic_1_t8A0B77C3063A5BD031494FB6E0ACE2DC30197C8F * get_address_of_s_chunkStore_32() { return &___s_chunkStore_32; }
	inline void set_s_chunkStore_32(SharedStatic_1_t8A0B77C3063A5BD031494FB6E0ACE2DC30197C8F  value)
	{
		___s_chunkStore_32 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.EntityComponentStore
struct EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA_marshaled_pinvoke
{
	int32_t* ___m_VersionByEntity_0;
	Archetype_t14B6BCF53EF0F918C8B503D7100F6217E480CD94 ** ___m_ArchetypeByEntity_1;
	EntityInChunk_tB28DC9CD18AA63C3C99B85007BC5009DFF9F4EA3 * ___m_EntityInChunkByEntity_2;
	int32_t* ___m_ComponentTypeOrderVersion_3;
	BlockAllocator_t43F1EF54B5CF45084293051ACE1D0AD8F18DAB07  ___m_ArchetypeChunkAllocator_4;
	UnsafeChunkPtrList_tF6FE39C1771CD6E6E5CA3B86195038FECD946D58_marshaled_pinvoke ___m_EmptyChunks_5;
	UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7_marshaled_pinvoke ___m_Archetypes_6;
	ArchetypeListMap_tB86167946EED7570CC5D89081659CE9A7B6A0847_marshaled_pinvoke ___m_TypeLookup_7;
	int32_t ___m_ManagedComponentIndex_8;
	int32_t ___m_ManagedComponentIndexCapacity_9;
	UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C  ___m_ManagedComponentFreeIndex_10;
	ManagedDeferredCommands_tD9C6F59E91B0E55519A28C84158F57A14B1CEFC5  ___ManagedChangesTracker_11;
	uint64_t ___m_NextChunkSequenceNumber_12;
	int32_t ___m_NextFreeEntityIndex_13;
	uint32_t ___m_GlobalSystemVersion_14;
	int32_t ___m_EntitiesCapacity_15;
	uint32_t ___m_ArchetypeTrackingVersion_16;
	int32_t ___m_LinkedGroupType_17;
	int32_t ___m_ChunkHeaderType_18;
	int32_t ___m_PrefabType_19;
	int32_t ___m_CleanupEntityType_20;
	int32_t ___m_DisabledType_21;
	int32_t ___m_EntityType_22;
	ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370  ___m_ChunkHeaderComponentType_23;
	ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370  ___m_EntityComponentType_24;
	TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * ___m_TypeInfos_25;
	EntityOffsetInfo_t398BFA31DBBAAA0488E257A761D3D254D716DE89 * ___m_EntityOffsetInfos_26;
	uint8_t ___memoryInitPattern_27;
	uint8_t ___useMemoryInitPattern_28;
};
// Native definition for COM marshalling of Unity.Entities.EntityComponentStore
struct EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA_marshaled_com
{
	int32_t* ___m_VersionByEntity_0;
	Archetype_t14B6BCF53EF0F918C8B503D7100F6217E480CD94 ** ___m_ArchetypeByEntity_1;
	EntityInChunk_tB28DC9CD18AA63C3C99B85007BC5009DFF9F4EA3 * ___m_EntityInChunkByEntity_2;
	int32_t* ___m_ComponentTypeOrderVersion_3;
	BlockAllocator_t43F1EF54B5CF45084293051ACE1D0AD8F18DAB07  ___m_ArchetypeChunkAllocator_4;
	UnsafeChunkPtrList_tF6FE39C1771CD6E6E5CA3B86195038FECD946D58_marshaled_com ___m_EmptyChunks_5;
	UnsafeArchetypePtrList_t246600A381271D00FDEC1F27A8DB67ED1F2779C7_marshaled_com ___m_Archetypes_6;
	ArchetypeListMap_tB86167946EED7570CC5D89081659CE9A7B6A0847_marshaled_com ___m_TypeLookup_7;
	int32_t ___m_ManagedComponentIndex_8;
	int32_t ___m_ManagedComponentIndexCapacity_9;
	UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C  ___m_ManagedComponentFreeIndex_10;
	ManagedDeferredCommands_tD9C6F59E91B0E55519A28C84158F57A14B1CEFC5  ___ManagedChangesTracker_11;
	uint64_t ___m_NextChunkSequenceNumber_12;
	int32_t ___m_NextFreeEntityIndex_13;
	uint32_t ___m_GlobalSystemVersion_14;
	int32_t ___m_EntitiesCapacity_15;
	uint32_t ___m_ArchetypeTrackingVersion_16;
	int32_t ___m_LinkedGroupType_17;
	int32_t ___m_ChunkHeaderType_18;
	int32_t ___m_PrefabType_19;
	int32_t ___m_CleanupEntityType_20;
	int32_t ___m_DisabledType_21;
	int32_t ___m_EntityType_22;
	ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370  ___m_ChunkHeaderComponentType_23;
	ComponentType_tEA24A4AB5BA78DEF7C2CEF25A05426D1F51D8370  ___m_EntityComponentType_24;
	TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * ___m_TypeInfos_25;
	EntityOffsetInfo_t398BFA31DBBAAA0488E257A761D3D254D716DE89 * ___m_EntityOffsetInfos_26;
	uint8_t ___memoryInitPattern_27;
	uint8_t ___useMemoryInitPattern_28;
};

// Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0_LambdaParameterValueProviders
struct  LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7 
{
public:
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_Entity Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0_LambdaParameterValueProviders::forParameter_e
	LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  ___forParameter_e_0;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_ISharedComponentData`1<Unity.Entities.BlobAssetOwner> Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0_LambdaParameterValueProviders::forParameter_blobOwner
	LambdaParameterValueProvider_ISharedComponentData_1_tE871547CB822CE66B2C193D94183ACD389503DF6  ___forParameter_blobOwner_1;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1<Unity.Entities.RetainBlobAssets> Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0_LambdaParameterValueProviders::forParameter_retain
	LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  ___forParameter_retain_2;

public:
	inline static int32_t get_offset_of_forParameter_e_0() { return static_cast<int32_t>(offsetof(LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7, ___forParameter_e_0)); }
	inline LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  get_forParameter_e_0() const { return ___forParameter_e_0; }
	inline LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13 * get_address_of_forParameter_e_0() { return &___forParameter_e_0; }
	inline void set_forParameter_e_0(LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  value)
	{
		___forParameter_e_0 = value;
	}

	inline static int32_t get_offset_of_forParameter_blobOwner_1() { return static_cast<int32_t>(offsetof(LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7, ___forParameter_blobOwner_1)); }
	inline LambdaParameterValueProvider_ISharedComponentData_1_tE871547CB822CE66B2C193D94183ACD389503DF6  get_forParameter_blobOwner_1() const { return ___forParameter_blobOwner_1; }
	inline LambdaParameterValueProvider_ISharedComponentData_1_tE871547CB822CE66B2C193D94183ACD389503DF6 * get_address_of_forParameter_blobOwner_1() { return &___forParameter_blobOwner_1; }
	inline void set_forParameter_blobOwner_1(LambdaParameterValueProvider_ISharedComponentData_1_tE871547CB822CE66B2C193D94183ACD389503DF6  value)
	{
		___forParameter_blobOwner_1 = value;
	}

	inline static int32_t get_offset_of_forParameter_retain_2() { return static_cast<int32_t>(offsetof(LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7, ___forParameter_retain_2)); }
	inline LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  get_forParameter_retain_2() const { return ___forParameter_retain_2; }
	inline LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4 * get_address_of_forParameter_retain_2() { return &___forParameter_retain_2; }
	inline void set_forParameter_retain_2(LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  value)
	{
		___forParameter_retain_2 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0/LambdaParameterValueProviders
struct LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7_marshaled_pinvoke
{
	LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  ___forParameter_e_0;
	LambdaParameterValueProvider_ISharedComponentData_1_tE871547CB822CE66B2C193D94183ACD389503DF6  ___forParameter_blobOwner_1;
	LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  ___forParameter_retain_2;
};
// Native definition for COM marshalling of Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0/LambdaParameterValueProviders
struct LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7_marshaled_com
{
	LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13  ___forParameter_e_0;
	LambdaParameterValueProvider_ISharedComponentData_1_tE871547CB822CE66B2C193D94183ACD389503DF6  ___forParameter_blobOwner_1;
	LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4  ___forParameter_retain_2;
};

// Unity.Entities.Serialization.SerializeUtility_ManagedObjectSerializeAdapter
struct  ManagedObjectSerializeAdapter_t69285139BD4DB32D092086C7F33A7182FBD24CE9  : public RuntimeObject
{
public:
	// Unity.Entities.EntityRemapUtility_EntityRemapInfo* Unity.Entities.Serialization.SerializeUtility_ManagedObjectSerializeAdapter::m_EntityRemapInfo
	EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334 * ___m_EntityRemapInfo_0;
	// Unity.Collections.NativeHashMap`2<Unity.Entities.Serialization.SerializeUtility_BlobAssetPtr,System.Int32> Unity.Entities.Serialization.SerializeUtility_ManagedObjectSerializeAdapter::m_BlobAssetMap
	NativeHashMap_2_tD236D8A3C81093EDA5D488C9CBBFCA0E8431A1C6  ___m_BlobAssetMap_1;
	// Unity.Collections.NativeArray`1<System.Int32> Unity.Entities.Serialization.SerializeUtility_ManagedObjectSerializeAdapter::m_BlobAssetOffsets
	NativeArray_1_tD60079F06B473C85CF6C2BB4F2D203F38191AE99  ___m_BlobAssetOffsets_2;

public:
	inline static int32_t get_offset_of_m_EntityRemapInfo_0() { return static_cast<int32_t>(offsetof(ManagedObjectSerializeAdapter_t69285139BD4DB32D092086C7F33A7182FBD24CE9, ___m_EntityRemapInfo_0)); }
	inline EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334 * get_m_EntityRemapInfo_0() const { return ___m_EntityRemapInfo_0; }
	inline EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334 ** get_address_of_m_EntityRemapInfo_0() { return &___m_EntityRemapInfo_0; }
	inline void set_m_EntityRemapInfo_0(EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334 * value)
	{
		___m_EntityRemapInfo_0 = value;
	}

	inline static int32_t get_offset_of_m_BlobAssetMap_1() { return static_cast<int32_t>(offsetof(ManagedObjectSerializeAdapter_t69285139BD4DB32D092086C7F33A7182FBD24CE9, ___m_BlobAssetMap_1)); }
	inline NativeHashMap_2_tD236D8A3C81093EDA5D488C9CBBFCA0E8431A1C6  get_m_BlobAssetMap_1() const { return ___m_BlobAssetMap_1; }
	inline NativeHashMap_2_tD236D8A3C81093EDA5D488C9CBBFCA0E8431A1C6 * get_address_of_m_BlobAssetMap_1() { return &___m_BlobAssetMap_1; }
	inline void set_m_BlobAssetMap_1(NativeHashMap_2_tD236D8A3C81093EDA5D488C9CBBFCA0E8431A1C6  value)
	{
		___m_BlobAssetMap_1 = value;
	}

	inline static int32_t get_offset_of_m_BlobAssetOffsets_2() { return static_cast<int32_t>(offsetof(ManagedObjectSerializeAdapter_t69285139BD4DB32D092086C7F33A7182FBD24CE9, ___m_BlobAssetOffsets_2)); }
	inline NativeArray_1_tD60079F06B473C85CF6C2BB4F2D203F38191AE99  get_m_BlobAssetOffsets_2() const { return ___m_BlobAssetOffsets_2; }
	inline NativeArray_1_tD60079F06B473C85CF6C2BB4F2D203F38191AE99 * get_address_of_m_BlobAssetOffsets_2() { return &___m_BlobAssetOffsets_2; }
	inline void set_m_BlobAssetOffsets_2(NativeArray_1_tD60079F06B473C85CF6C2BB4F2D203F38191AE99  value)
	{
		___m_BlobAssetOffsets_2 = value;
	}
};


// Unity.Entities.TypeManager
struct  TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134  : public RuntimeObject
{
public:

public:
};

struct TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields
{
public:
	// System.Int32 Unity.Entities.TypeManager::s_SystemCount
	int32_t ___s_SystemCount_0;
	// System.Collections.Generic.List`1<System.Type> Unity.Entities.TypeManager::s_SystemTypes
	List_1_t7CFD5FCE8366620F593F2C9DAC3A870E5D6506D7 * ___s_SystemTypes_1;
	// System.Collections.Generic.List`1<System.String> Unity.Entities.TypeManager::s_SystemTypeNames
	List_1_t6C9F81EDBF0F4A31A9B0DA372D2EF34BDA3A1AF3 * ___s_SystemTypeNames_2;
	// Unity.Collections.NativeList`1<System.Boolean> Unity.Entities.TypeManager::s_SystemIsGroupList
	NativeList_1_t3933DBBB090E35386DA7B0DEFB0D5FAE42A3B8CC  ___s_SystemIsGroupList_3;
	// System.Int32 Unity.Entities.TypeManager::s_TypeCount
	int32_t ___s_TypeCount_4;
	// System.Boolean Unity.Entities.TypeManager::s_Initialized
	bool ___s_Initialized_5;
	// Unity.Collections.NativeArray`1<Unity.Entities.TypeManager_TypeInfo> Unity.Entities.TypeManager::s_TypeInfos
	NativeArray_1_t663B008ABD2F6A4E7DCC3EC82886219F3AB8D5EA  ___s_TypeInfos_6;
	// Unity.Collections.NativeHashMap`2<System.UInt64,System.Int32> Unity.Entities.TypeManager::s_StableTypeHashToTypeIndex
	NativeHashMap_2_t2702DC826E1F3A2D98EA4CEE7798F11E0C91F608  ___s_StableTypeHashToTypeIndex_7;
	// Unity.Collections.NativeList`1<Unity.Entities.TypeManager_EntityOffsetInfo> Unity.Entities.TypeManager::s_EntityOffsetList
	NativeList_1_t1DA88E55F30826046A76336D9F8D3AA26A9377FF  ___s_EntityOffsetList_8;
	// Unity.Collections.NativeList`1<Unity.Entities.TypeManager_EntityOffsetInfo> Unity.Entities.TypeManager::s_BlobAssetRefOffsetList
	NativeList_1_t1DA88E55F30826046A76336D9F8D3AA26A9377FF  ___s_BlobAssetRefOffsetList_9;
	// Unity.Collections.NativeList`1<System.Int32> Unity.Entities.TypeManager::s_WriteGroupList
	NativeList_1_t37924D67F0E4D83DE506A261B7A05AE619630C80  ___s_WriteGroupList_10;
	// System.Collections.Generic.List`1<Unity.Entities.FastEquality_TypeInfo> Unity.Entities.TypeManager::s_FastEqualityTypeInfoList
	List_1_tB206DD21A43E0C00DFC00E7D80011FF29ECC2BBB * ___s_FastEqualityTypeInfoList_11;
	// System.Collections.Generic.List`1<System.Type> Unity.Entities.TypeManager::s_Types
	List_1_t7CFD5FCE8366620F593F2C9DAC3A870E5D6506D7 * ___s_Types_12;
	// System.Collections.Generic.List`1<System.String> Unity.Entities.TypeManager::s_TypeNames
	List_1_t6C9F81EDBF0F4A31A9B0DA372D2EF34BDA3A1AF3 * ___s_TypeNames_13;
	// System.Boolean Unity.Entities.TypeManager::s_AppDomainUnloadRegistered
	bool ___s_AppDomainUnloadRegistered_14;
	// System.Double Unity.Entities.TypeManager::s_TypeInitializationTime
	double ___s_TypeInitializationTime_15;
	// System.Collections.Generic.Dictionary`2<System.Type,System.Int32> Unity.Entities.TypeManager::s_ManagedTypeToIndex
	Dictionary_2_t6CE7336785D73EB9AC6DBFDBCB55D4CF15047CB7 * ___s_ManagedTypeToIndex_16;
	// System.Collections.Generic.Dictionary`2<System.Type,System.Exception> Unity.Entities.TypeManager::s_FailedTypeBuildException
	Dictionary_2_tB94CC7A94D357C2FE46DCF5A87B29DE8F7128C4E * ___s_FailedTypeBuildException_17;
	// System.Int32 Unity.Entities.TypeManager::ObjectOffset
	int32_t ___ObjectOffset_18;
	// System.Type Unity.Entities.TypeManager::UnityEngineObjectType
	Type_t * ___UnityEngineObjectType_19;
	// System.Type Unity.Entities.TypeManager::GameObjectEntityType
	Type_t * ___GameObjectEntityType_20;

public:
	inline static int32_t get_offset_of_s_SystemCount_0() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_SystemCount_0)); }
	inline int32_t get_s_SystemCount_0() const { return ___s_SystemCount_0; }
	inline int32_t* get_address_of_s_SystemCount_0() { return &___s_SystemCount_0; }
	inline void set_s_SystemCount_0(int32_t value)
	{
		___s_SystemCount_0 = value;
	}

	inline static int32_t get_offset_of_s_SystemTypes_1() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_SystemTypes_1)); }
	inline List_1_t7CFD5FCE8366620F593F2C9DAC3A870E5D6506D7 * get_s_SystemTypes_1() const { return ___s_SystemTypes_1; }
	inline List_1_t7CFD5FCE8366620F593F2C9DAC3A870E5D6506D7 ** get_address_of_s_SystemTypes_1() { return &___s_SystemTypes_1; }
	inline void set_s_SystemTypes_1(List_1_t7CFD5FCE8366620F593F2C9DAC3A870E5D6506D7 * value)
	{
		___s_SystemTypes_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_SystemTypes_1), (void*)value);
	}

	inline static int32_t get_offset_of_s_SystemTypeNames_2() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_SystemTypeNames_2)); }
	inline List_1_t6C9F81EDBF0F4A31A9B0DA372D2EF34BDA3A1AF3 * get_s_SystemTypeNames_2() const { return ___s_SystemTypeNames_2; }
	inline List_1_t6C9F81EDBF0F4A31A9B0DA372D2EF34BDA3A1AF3 ** get_address_of_s_SystemTypeNames_2() { return &___s_SystemTypeNames_2; }
	inline void set_s_SystemTypeNames_2(List_1_t6C9F81EDBF0F4A31A9B0DA372D2EF34BDA3A1AF3 * value)
	{
		___s_SystemTypeNames_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_SystemTypeNames_2), (void*)value);
	}

	inline static int32_t get_offset_of_s_SystemIsGroupList_3() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_SystemIsGroupList_3)); }
	inline NativeList_1_t3933DBBB090E35386DA7B0DEFB0D5FAE42A3B8CC  get_s_SystemIsGroupList_3() const { return ___s_SystemIsGroupList_3; }
	inline NativeList_1_t3933DBBB090E35386DA7B0DEFB0D5FAE42A3B8CC * get_address_of_s_SystemIsGroupList_3() { return &___s_SystemIsGroupList_3; }
	inline void set_s_SystemIsGroupList_3(NativeList_1_t3933DBBB090E35386DA7B0DEFB0D5FAE42A3B8CC  value)
	{
		___s_SystemIsGroupList_3 = value;
	}

	inline static int32_t get_offset_of_s_TypeCount_4() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_TypeCount_4)); }
	inline int32_t get_s_TypeCount_4() const { return ___s_TypeCount_4; }
	inline int32_t* get_address_of_s_TypeCount_4() { return &___s_TypeCount_4; }
	inline void set_s_TypeCount_4(int32_t value)
	{
		___s_TypeCount_4 = value;
	}

	inline static int32_t get_offset_of_s_Initialized_5() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_Initialized_5)); }
	inline bool get_s_Initialized_5() const { return ___s_Initialized_5; }
	inline bool* get_address_of_s_Initialized_5() { return &___s_Initialized_5; }
	inline void set_s_Initialized_5(bool value)
	{
		___s_Initialized_5 = value;
	}

	inline static int32_t get_offset_of_s_TypeInfos_6() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_TypeInfos_6)); }
	inline NativeArray_1_t663B008ABD2F6A4E7DCC3EC82886219F3AB8D5EA  get_s_TypeInfos_6() const { return ___s_TypeInfos_6; }
	inline NativeArray_1_t663B008ABD2F6A4E7DCC3EC82886219F3AB8D5EA * get_address_of_s_TypeInfos_6() { return &___s_TypeInfos_6; }
	inline void set_s_TypeInfos_6(NativeArray_1_t663B008ABD2F6A4E7DCC3EC82886219F3AB8D5EA  value)
	{
		___s_TypeInfos_6 = value;
	}

	inline static int32_t get_offset_of_s_StableTypeHashToTypeIndex_7() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_StableTypeHashToTypeIndex_7)); }
	inline NativeHashMap_2_t2702DC826E1F3A2D98EA4CEE7798F11E0C91F608  get_s_StableTypeHashToTypeIndex_7() const { return ___s_StableTypeHashToTypeIndex_7; }
	inline NativeHashMap_2_t2702DC826E1F3A2D98EA4CEE7798F11E0C91F608 * get_address_of_s_StableTypeHashToTypeIndex_7() { return &___s_StableTypeHashToTypeIndex_7; }
	inline void set_s_StableTypeHashToTypeIndex_7(NativeHashMap_2_t2702DC826E1F3A2D98EA4CEE7798F11E0C91F608  value)
	{
		___s_StableTypeHashToTypeIndex_7 = value;
	}

	inline static int32_t get_offset_of_s_EntityOffsetList_8() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_EntityOffsetList_8)); }
	inline NativeList_1_t1DA88E55F30826046A76336D9F8D3AA26A9377FF  get_s_EntityOffsetList_8() const { return ___s_EntityOffsetList_8; }
	inline NativeList_1_t1DA88E55F30826046A76336D9F8D3AA26A9377FF * get_address_of_s_EntityOffsetList_8() { return &___s_EntityOffsetList_8; }
	inline void set_s_EntityOffsetList_8(NativeList_1_t1DA88E55F30826046A76336D9F8D3AA26A9377FF  value)
	{
		___s_EntityOffsetList_8 = value;
	}

	inline static int32_t get_offset_of_s_BlobAssetRefOffsetList_9() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_BlobAssetRefOffsetList_9)); }
	inline NativeList_1_t1DA88E55F30826046A76336D9F8D3AA26A9377FF  get_s_BlobAssetRefOffsetList_9() const { return ___s_BlobAssetRefOffsetList_9; }
	inline NativeList_1_t1DA88E55F30826046A76336D9F8D3AA26A9377FF * get_address_of_s_BlobAssetRefOffsetList_9() { return &___s_BlobAssetRefOffsetList_9; }
	inline void set_s_BlobAssetRefOffsetList_9(NativeList_1_t1DA88E55F30826046A76336D9F8D3AA26A9377FF  value)
	{
		___s_BlobAssetRefOffsetList_9 = value;
	}

	inline static int32_t get_offset_of_s_WriteGroupList_10() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_WriteGroupList_10)); }
	inline NativeList_1_t37924D67F0E4D83DE506A261B7A05AE619630C80  get_s_WriteGroupList_10() const { return ___s_WriteGroupList_10; }
	inline NativeList_1_t37924D67F0E4D83DE506A261B7A05AE619630C80 * get_address_of_s_WriteGroupList_10() { return &___s_WriteGroupList_10; }
	inline void set_s_WriteGroupList_10(NativeList_1_t37924D67F0E4D83DE506A261B7A05AE619630C80  value)
	{
		___s_WriteGroupList_10 = value;
	}

	inline static int32_t get_offset_of_s_FastEqualityTypeInfoList_11() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_FastEqualityTypeInfoList_11)); }
	inline List_1_tB206DD21A43E0C00DFC00E7D80011FF29ECC2BBB * get_s_FastEqualityTypeInfoList_11() const { return ___s_FastEqualityTypeInfoList_11; }
	inline List_1_tB206DD21A43E0C00DFC00E7D80011FF29ECC2BBB ** get_address_of_s_FastEqualityTypeInfoList_11() { return &___s_FastEqualityTypeInfoList_11; }
	inline void set_s_FastEqualityTypeInfoList_11(List_1_tB206DD21A43E0C00DFC00E7D80011FF29ECC2BBB * value)
	{
		___s_FastEqualityTypeInfoList_11 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_FastEqualityTypeInfoList_11), (void*)value);
	}

	inline static int32_t get_offset_of_s_Types_12() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_Types_12)); }
	inline List_1_t7CFD5FCE8366620F593F2C9DAC3A870E5D6506D7 * get_s_Types_12() const { return ___s_Types_12; }
	inline List_1_t7CFD5FCE8366620F593F2C9DAC3A870E5D6506D7 ** get_address_of_s_Types_12() { return &___s_Types_12; }
	inline void set_s_Types_12(List_1_t7CFD5FCE8366620F593F2C9DAC3A870E5D6506D7 * value)
	{
		___s_Types_12 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_Types_12), (void*)value);
	}

	inline static int32_t get_offset_of_s_TypeNames_13() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_TypeNames_13)); }
	inline List_1_t6C9F81EDBF0F4A31A9B0DA372D2EF34BDA3A1AF3 * get_s_TypeNames_13() const { return ___s_TypeNames_13; }
	inline List_1_t6C9F81EDBF0F4A31A9B0DA372D2EF34BDA3A1AF3 ** get_address_of_s_TypeNames_13() { return &___s_TypeNames_13; }
	inline void set_s_TypeNames_13(List_1_t6C9F81EDBF0F4A31A9B0DA372D2EF34BDA3A1AF3 * value)
	{
		___s_TypeNames_13 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_TypeNames_13), (void*)value);
	}

	inline static int32_t get_offset_of_s_AppDomainUnloadRegistered_14() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_AppDomainUnloadRegistered_14)); }
	inline bool get_s_AppDomainUnloadRegistered_14() const { return ___s_AppDomainUnloadRegistered_14; }
	inline bool* get_address_of_s_AppDomainUnloadRegistered_14() { return &___s_AppDomainUnloadRegistered_14; }
	inline void set_s_AppDomainUnloadRegistered_14(bool value)
	{
		___s_AppDomainUnloadRegistered_14 = value;
	}

	inline static int32_t get_offset_of_s_TypeInitializationTime_15() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_TypeInitializationTime_15)); }
	inline double get_s_TypeInitializationTime_15() const { return ___s_TypeInitializationTime_15; }
	inline double* get_address_of_s_TypeInitializationTime_15() { return &___s_TypeInitializationTime_15; }
	inline void set_s_TypeInitializationTime_15(double value)
	{
		___s_TypeInitializationTime_15 = value;
	}

	inline static int32_t get_offset_of_s_ManagedTypeToIndex_16() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_ManagedTypeToIndex_16)); }
	inline Dictionary_2_t6CE7336785D73EB9AC6DBFDBCB55D4CF15047CB7 * get_s_ManagedTypeToIndex_16() const { return ___s_ManagedTypeToIndex_16; }
	inline Dictionary_2_t6CE7336785D73EB9AC6DBFDBCB55D4CF15047CB7 ** get_address_of_s_ManagedTypeToIndex_16() { return &___s_ManagedTypeToIndex_16; }
	inline void set_s_ManagedTypeToIndex_16(Dictionary_2_t6CE7336785D73EB9AC6DBFDBCB55D4CF15047CB7 * value)
	{
		___s_ManagedTypeToIndex_16 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_ManagedTypeToIndex_16), (void*)value);
	}

	inline static int32_t get_offset_of_s_FailedTypeBuildException_17() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___s_FailedTypeBuildException_17)); }
	inline Dictionary_2_tB94CC7A94D357C2FE46DCF5A87B29DE8F7128C4E * get_s_FailedTypeBuildException_17() const { return ___s_FailedTypeBuildException_17; }
	inline Dictionary_2_tB94CC7A94D357C2FE46DCF5A87B29DE8F7128C4E ** get_address_of_s_FailedTypeBuildException_17() { return &___s_FailedTypeBuildException_17; }
	inline void set_s_FailedTypeBuildException_17(Dictionary_2_tB94CC7A94D357C2FE46DCF5A87B29DE8F7128C4E * value)
	{
		___s_FailedTypeBuildException_17 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_FailedTypeBuildException_17), (void*)value);
	}

	inline static int32_t get_offset_of_ObjectOffset_18() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___ObjectOffset_18)); }
	inline int32_t get_ObjectOffset_18() const { return ___ObjectOffset_18; }
	inline int32_t* get_address_of_ObjectOffset_18() { return &___ObjectOffset_18; }
	inline void set_ObjectOffset_18(int32_t value)
	{
		___ObjectOffset_18 = value;
	}

	inline static int32_t get_offset_of_UnityEngineObjectType_19() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___UnityEngineObjectType_19)); }
	inline Type_t * get_UnityEngineObjectType_19() const { return ___UnityEngineObjectType_19; }
	inline Type_t ** get_address_of_UnityEngineObjectType_19() { return &___UnityEngineObjectType_19; }
	inline void set_UnityEngineObjectType_19(Type_t * value)
	{
		___UnityEngineObjectType_19 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___UnityEngineObjectType_19), (void*)value);
	}

	inline static int32_t get_offset_of_GameObjectEntityType_20() { return static_cast<int32_t>(offsetof(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields, ___GameObjectEntityType_20)); }
	inline Type_t * get_GameObjectEntityType_20() const { return ___GameObjectEntityType_20; }
	inline Type_t ** get_address_of_GameObjectEntityType_20() { return &___GameObjectEntityType_20; }
	inline void set_GameObjectEntityType_20(Type_t * value)
	{
		___GameObjectEntityType_20 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___GameObjectEntityType_20), (void*)value);
	}
};


// Unity.Collections.NativeMultiHashMap`2_Enumerator<Unity.Entities.EntityPatcher_EntityComponentPair,Unity.Entities.EntityPatcher_OffsetEntityPair>
struct  Enumerator_t22279CE04A52106502C7CC851E8ADC1CA6BA1D0E 
{
public:
	// Unity.Collections.NativeMultiHashMap`2<TKey,TValue> Unity.Collections.NativeMultiHashMap`2_Enumerator::hashmap
	NativeMultiHashMap_2_t60EC0F9BEBB301A3A42CC426583D29FDF8CCEF37  ___hashmap_0;
	// TKey Unity.Collections.NativeMultiHashMap`2_Enumerator::key
	EntityComponentPair_tF1A2F420287859D321D85CB20D9846DB9101294C  ___key_1;
	// System.Boolean Unity.Collections.NativeMultiHashMap`2_Enumerator::isFirst
	bool ___isFirst_2;
	// TValue Unity.Collections.NativeMultiHashMap`2_Enumerator::value
	OffsetEntityPair_tEA2146CEDACEF29E97936D20BC88923609DC4B8A  ___value_3;
	// Unity.Collections.NativeMultiHashMapIterator`1<TKey> Unity.Collections.NativeMultiHashMap`2_Enumerator::iterator
	NativeMultiHashMapIterator_1_t6B2775FD56E4580AACD742CE7830FE0C58F0EE12  ___iterator_4;

public:
	inline static int32_t get_offset_of_hashmap_0() { return static_cast<int32_t>(offsetof(Enumerator_t22279CE04A52106502C7CC851E8ADC1CA6BA1D0E, ___hashmap_0)); }
	inline NativeMultiHashMap_2_t60EC0F9BEBB301A3A42CC426583D29FDF8CCEF37  get_hashmap_0() const { return ___hashmap_0; }
	inline NativeMultiHashMap_2_t60EC0F9BEBB301A3A42CC426583D29FDF8CCEF37 * get_address_of_hashmap_0() { return &___hashmap_0; }
	inline void set_hashmap_0(NativeMultiHashMap_2_t60EC0F9BEBB301A3A42CC426583D29FDF8CCEF37  value)
	{
		___hashmap_0 = value;
	}

	inline static int32_t get_offset_of_key_1() { return static_cast<int32_t>(offsetof(Enumerator_t22279CE04A52106502C7CC851E8ADC1CA6BA1D0E, ___key_1)); }
	inline EntityComponentPair_tF1A2F420287859D321D85CB20D9846DB9101294C  get_key_1() const { return ___key_1; }
	inline EntityComponentPair_tF1A2F420287859D321D85CB20D9846DB9101294C * get_address_of_key_1() { return &___key_1; }
	inline void set_key_1(EntityComponentPair_tF1A2F420287859D321D85CB20D9846DB9101294C  value)
	{
		___key_1 = value;
	}

	inline static int32_t get_offset_of_isFirst_2() { return static_cast<int32_t>(offsetof(Enumerator_t22279CE04A52106502C7CC851E8ADC1CA6BA1D0E, ___isFirst_2)); }
	inline bool get_isFirst_2() const { return ___isFirst_2; }
	inline bool* get_address_of_isFirst_2() { return &___isFirst_2; }
	inline void set_isFirst_2(bool value)
	{
		___isFirst_2 = value;
	}

	inline static int32_t get_offset_of_value_3() { return static_cast<int32_t>(offsetof(Enumerator_t22279CE04A52106502C7CC851E8ADC1CA6BA1D0E, ___value_3)); }
	inline OffsetEntityPair_tEA2146CEDACEF29E97936D20BC88923609DC4B8A  get_value_3() const { return ___value_3; }
	inline OffsetEntityPair_tEA2146CEDACEF29E97936D20BC88923609DC4B8A * get_address_of_value_3() { return &___value_3; }
	inline void set_value_3(OffsetEntityPair_tEA2146CEDACEF29E97936D20BC88923609DC4B8A  value)
	{
		___value_3 = value;
	}

	inline static int32_t get_offset_of_iterator_4() { return static_cast<int32_t>(offsetof(Enumerator_t22279CE04A52106502C7CC851E8ADC1CA6BA1D0E, ___iterator_4)); }
	inline NativeMultiHashMapIterator_1_t6B2775FD56E4580AACD742CE7830FE0C58F0EE12  get_iterator_4() const { return ___iterator_4; }
	inline NativeMultiHashMapIterator_1_t6B2775FD56E4580AACD742CE7830FE0C58F0EE12 * get_address_of_iterator_4() { return &___iterator_4; }
	inline void set_iterator_4(NativeMultiHashMapIterator_1_t6B2775FD56E4580AACD742CE7830FE0C58F0EE12  value)
	{
		___iterator_4 = value;
	}
};


// Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0_LambdaParameterValueProviders_Runtimes
struct  Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879 
{
public:
	// Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0_LambdaParameterValueProviders_Runtimes::_entityProvider
	StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994  ____entityProvider_0;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_Entity_StructuralChangeRuntime Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0_LambdaParameterValueProviders_Runtimes::runtime_e
	StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  ___runtime_e_1;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_ISharedComponentData`1_StructuralChangeRuntime<Unity.Entities.BlobAssetOwner> Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0_LambdaParameterValueProviders_Runtimes::runtime_blobOwner
	StructuralChangeRuntime_t73F4A22CB7388EB3FB14FDAB733CD0AA00539B60  ___runtime_blobOwner_2;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1_StructuralChangeRuntime<Unity.Entities.RetainBlobAssets> Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0_LambdaParameterValueProviders_Runtimes::runtime_retain
	StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  ___runtime_retain_3;

public:
	inline static int32_t get_offset_of__entityProvider_0() { return static_cast<int32_t>(offsetof(Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879, ____entityProvider_0)); }
	inline StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994  get__entityProvider_0() const { return ____entityProvider_0; }
	inline StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994 * get_address_of__entityProvider_0() { return &____entityProvider_0; }
	inline void set__entityProvider_0(StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994  value)
	{
		____entityProvider_0 = value;
	}

	inline static int32_t get_offset_of_runtime_e_1() { return static_cast<int32_t>(offsetof(Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879, ___runtime_e_1)); }
	inline StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  get_runtime_e_1() const { return ___runtime_e_1; }
	inline StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA * get_address_of_runtime_e_1() { return &___runtime_e_1; }
	inline void set_runtime_e_1(StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  value)
	{
		___runtime_e_1 = value;
	}

	inline static int32_t get_offset_of_runtime_blobOwner_2() { return static_cast<int32_t>(offsetof(Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879, ___runtime_blobOwner_2)); }
	inline StructuralChangeRuntime_t73F4A22CB7388EB3FB14FDAB733CD0AA00539B60  get_runtime_blobOwner_2() const { return ___runtime_blobOwner_2; }
	inline StructuralChangeRuntime_t73F4A22CB7388EB3FB14FDAB733CD0AA00539B60 * get_address_of_runtime_blobOwner_2() { return &___runtime_blobOwner_2; }
	inline void set_runtime_blobOwner_2(StructuralChangeRuntime_t73F4A22CB7388EB3FB14FDAB733CD0AA00539B60  value)
	{
		___runtime_blobOwner_2 = value;
	}

	inline static int32_t get_offset_of_runtime_retain_3() { return static_cast<int32_t>(offsetof(Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879, ___runtime_retain_3)); }
	inline StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  get_runtime_retain_3() const { return ___runtime_retain_3; }
	inline StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B * get_address_of_runtime_retain_3() { return &___runtime_retain_3; }
	inline void set_runtime_retain_3(StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  value)
	{
		___runtime_retain_3 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0/LambdaParameterValueProviders/Runtimes
struct Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879_marshaled_pinvoke
{
	StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_pinvoke ____entityProvider_0;
	StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  ___runtime_e_1;
	StructuralChangeRuntime_t73F4A22CB7388EB3FB14FDAB733CD0AA00539B60  ___runtime_blobOwner_2;
	StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  ___runtime_retain_3;
};
// Native definition for COM marshalling of Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0/LambdaParameterValueProviders/Runtimes
struct Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879_marshaled_com
{
	StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_com ____entityProvider_0;
	StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  ___runtime_e_1;
	StructuralChangeRuntime_t73F4A22CB7388EB3FB14FDAB733CD0AA00539B60  ___runtime_blobOwner_2;
	StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  ___runtime_retain_3;
};

// Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1_LambdaParameterValueProviders_Runtimes
struct  Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF 
{
public:
	// Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1_LambdaParameterValueProviders_Runtimes::_entityProvider
	StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994  ____entityProvider_0;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_Entity_StructuralChangeRuntime Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1_LambdaParameterValueProviders_Runtimes::runtime_e
	StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  ___runtime_e_1;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1_StructuralChangeRuntime<Unity.Entities.RetainBlobAssets> Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1_LambdaParameterValueProviders_Runtimes::runtime_retain
	StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  ___runtime_retain_2;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1_StructuralChangeRuntime<Unity.Entities.RetainBlobAssetBatchPtr> Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1_LambdaParameterValueProviders_Runtimes::runtime_retainPtr
	StructuralChangeRuntime_t91CF003F24C722618BFAB2EC15E454A10524C5F5  ___runtime_retainPtr_3;

public:
	inline static int32_t get_offset_of__entityProvider_0() { return static_cast<int32_t>(offsetof(Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF, ____entityProvider_0)); }
	inline StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994  get__entityProvider_0() const { return ____entityProvider_0; }
	inline StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994 * get_address_of__entityProvider_0() { return &____entityProvider_0; }
	inline void set__entityProvider_0(StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994  value)
	{
		____entityProvider_0 = value;
	}

	inline static int32_t get_offset_of_runtime_e_1() { return static_cast<int32_t>(offsetof(Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF, ___runtime_e_1)); }
	inline StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  get_runtime_e_1() const { return ___runtime_e_1; }
	inline StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA * get_address_of_runtime_e_1() { return &___runtime_e_1; }
	inline void set_runtime_e_1(StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  value)
	{
		___runtime_e_1 = value;
	}

	inline static int32_t get_offset_of_runtime_retain_2() { return static_cast<int32_t>(offsetof(Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF, ___runtime_retain_2)); }
	inline StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  get_runtime_retain_2() const { return ___runtime_retain_2; }
	inline StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B * get_address_of_runtime_retain_2() { return &___runtime_retain_2; }
	inline void set_runtime_retain_2(StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  value)
	{
		___runtime_retain_2 = value;
	}

	inline static int32_t get_offset_of_runtime_retainPtr_3() { return static_cast<int32_t>(offsetof(Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF, ___runtime_retainPtr_3)); }
	inline StructuralChangeRuntime_t91CF003F24C722618BFAB2EC15E454A10524C5F5  get_runtime_retainPtr_3() const { return ___runtime_retainPtr_3; }
	inline StructuralChangeRuntime_t91CF003F24C722618BFAB2EC15E454A10524C5F5 * get_address_of_runtime_retainPtr_3() { return &___runtime_retainPtr_3; }
	inline void set_runtime_retainPtr_3(StructuralChangeRuntime_t91CF003F24C722618BFAB2EC15E454A10524C5F5  value)
	{
		___runtime_retainPtr_3 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1/LambdaParameterValueProviders/Runtimes
struct Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF_marshaled_pinvoke
{
	StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_pinvoke ____entityProvider_0;
	StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  ___runtime_e_1;
	StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  ___runtime_retain_2;
	StructuralChangeRuntime_t91CF003F24C722618BFAB2EC15E454A10524C5F5  ___runtime_retainPtr_3;
};
// Native definition for COM marshalling of Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1/LambdaParameterValueProviders/Runtimes
struct Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF_marshaled_com
{
	StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_com ____entityProvider_0;
	StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  ___runtime_e_1;
	StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  ___runtime_retain_2;
	StructuralChangeRuntime_t91CF003F24C722618BFAB2EC15E454A10524C5F5  ___runtime_retainPtr_3;
};

// Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2_LambdaParameterValueProviders_Runtimes
struct  Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 
{
public:
	// Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2_LambdaParameterValueProviders_Runtimes::_entityProvider
	StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994  ____entityProvider_0;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_Entity_StructuralChangeRuntime Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2_LambdaParameterValueProviders_Runtimes::runtime_e
	StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  ___runtime_e_1;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1_StructuralChangeRuntime<Unity.Entities.RetainBlobAssets> Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2_LambdaParameterValueProviders_Runtimes::runtime_retain
	StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  ___runtime_retain_2;
	// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1_StructuralChangeRuntime<Unity.Entities.RetainBlobAssetPtr> Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2_LambdaParameterValueProviders_Runtimes::runtime_retainPtr
	StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088  ___runtime_retainPtr_3;

public:
	inline static int32_t get_offset_of__entityProvider_0() { return static_cast<int32_t>(offsetof(Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8, ____entityProvider_0)); }
	inline StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994  get__entityProvider_0() const { return ____entityProvider_0; }
	inline StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994 * get_address_of__entityProvider_0() { return &____entityProvider_0; }
	inline void set__entityProvider_0(StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994  value)
	{
		____entityProvider_0 = value;
	}

	inline static int32_t get_offset_of_runtime_e_1() { return static_cast<int32_t>(offsetof(Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8, ___runtime_e_1)); }
	inline StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  get_runtime_e_1() const { return ___runtime_e_1; }
	inline StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA * get_address_of_runtime_e_1() { return &___runtime_e_1; }
	inline void set_runtime_e_1(StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  value)
	{
		___runtime_e_1 = value;
	}

	inline static int32_t get_offset_of_runtime_retain_2() { return static_cast<int32_t>(offsetof(Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8, ___runtime_retain_2)); }
	inline StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  get_runtime_retain_2() const { return ___runtime_retain_2; }
	inline StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B * get_address_of_runtime_retain_2() { return &___runtime_retain_2; }
	inline void set_runtime_retain_2(StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  value)
	{
		___runtime_retain_2 = value;
	}

	inline static int32_t get_offset_of_runtime_retainPtr_3() { return static_cast<int32_t>(offsetof(Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8, ___runtime_retainPtr_3)); }
	inline StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088  get_runtime_retainPtr_3() const { return ___runtime_retainPtr_3; }
	inline StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088 * get_address_of_runtime_retainPtr_3() { return &___runtime_retainPtr_3; }
	inline void set_runtime_retainPtr_3(StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088  value)
	{
		___runtime_retainPtr_3 = value;
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders/Runtimes
struct Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_marshaled_pinvoke
{
	StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_pinvoke ____entityProvider_0;
	StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  ___runtime_e_1;
	StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  ___runtime_retain_2;
	StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088  ___runtime_retainPtr_3;
};
// Native definition for COM marshalling of Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders/Runtimes
struct Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_marshaled_com
{
	StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_com ____entityProvider_0;
	StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  ___runtime_e_1;
	StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  ___runtime_retain_2;
	StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088  ___runtime_retainPtr_3;
};

// Unity.Entities.StructuralChange__dlg_AddComponentChunks
struct  _dlg_AddComponentChunks_tD9DDFAADFC2F439BBB9B8CEAA163B6CFF9D95D92  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.StructuralChange__dlg_AddComponentEntitiesBatch
struct  _dlg_AddComponentEntitiesBatch_tA45112FB52366C0ADDEBC039028B24BEFC59B175  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.StructuralChange__dlg_AddComponentEntity
struct  _dlg_AddComponentEntity_tCDE23991E1FA8C395DCCC00BCD0316E834300D95  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.StructuralChange__dlg_AddSharedComponentChunks
struct  _dlg_AddSharedComponentChunks_t0BF4E29BC434846F82C9401CDF67A851FA5A58E3  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.StructuralChange__dlg_CreateEntity
struct  _dlg_CreateEntity_t418D1599A9300D4C8883E329439F01C2CB94856F  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.StructuralChange__dlg_InstantiateEntities
struct  _dlg_InstantiateEntities_t1F5957B60C35DE9F8691CBDC46C67DCD4A29B678  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.StructuralChange__dlg_MoveEntityArchetype
struct  _dlg_MoveEntityArchetype_t8C03493F5C6FB58AAECDD0B6DBFA736F5BD204A5  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.StructuralChange__dlg_RemoveComponentChunks
struct  _dlg_RemoveComponentChunks_t156D7CDE1CB0A8A575514C20309E1392F8BE65F9  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.StructuralChange__dlg_RemoveComponentEntitiesBatch
struct  _dlg_RemoveComponentEntitiesBatch_t092F3F9E00E3B1F1F6A33A456E19C0E7ABE992E1  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.StructuralChange__dlg_RemoveComponentEntity
struct  _dlg_RemoveComponentEntity_t031F2F7279291D7654E794B4B7EB273464D764E6  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.StructuralChange__dlg_SetChunkComponent
struct  _dlg_SetChunkComponent_t58428EAC3CAA8BEF48D7DEB8E71516FB3619524E  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Entities.EntityPatcher_EntityDiffPatcher_EntityPatchAdapter
struct  EntityPatchAdapter_tE216B50635D7EEC4301D9C54E3F1894F658D59AC  : public RuntimeObject
{
public:
	// Unity.Collections.NativeMultiHashMap`2_Enumerator<Unity.Entities.EntityPatcher_EntityComponentPair,Unity.Entities.EntityPatcher_OffsetEntityPair> Unity.Entities.EntityPatcher_EntityDiffPatcher_EntityPatchAdapter::Patches
	Enumerator_t22279CE04A52106502C7CC851E8ADC1CA6BA1D0E  ___Patches_0;

public:
	inline static int32_t get_offset_of_Patches_0() { return static_cast<int32_t>(offsetof(EntityPatchAdapter_tE216B50635D7EEC4301D9C54E3F1894F658D59AC, ___Patches_0)); }
	inline Enumerator_t22279CE04A52106502C7CC851E8ADC1CA6BA1D0E  get_Patches_0() const { return ___Patches_0; }
	inline Enumerator_t22279CE04A52106502C7CC851E8ADC1CA6BA1D0E * get_address_of_Patches_0() { return &___Patches_0; }
	inline void set_Patches_0(Enumerator_t22279CE04A52106502C7CC851E8ADC1CA6BA1D0E  value)
	{
		___Patches_0 = value;
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
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

IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshal_pinvoke(const LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31& unmarshaled, LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_pinvoke& marshaled);
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshal_pinvoke_back(const LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_pinvoke& marshaled, LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31& unmarshaled);
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshal_pinvoke_cleanup(LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_pinvoke& marshaled);
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshal_com(const LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31& unmarshaled, LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_com& marshaled);
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshal_com_back(const LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_com& marshaled, LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31& unmarshaled);
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshal_com_cleanup(LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_com& marshaled);
IL2CPP_EXTERN_C void StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshal_pinvoke(const StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994& unmarshaled, StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_pinvoke& marshaled);
IL2CPP_EXTERN_C void StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshal_pinvoke_back(const StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_pinvoke& marshaled, StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994& unmarshaled);
IL2CPP_EXTERN_C void StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshal_pinvoke_cleanup(StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_pinvoke& marshaled);
IL2CPP_EXTERN_C void StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshal_com(const StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994& unmarshaled, StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_com& marshaled);
IL2CPP_EXTERN_C void StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshal_com_back(const StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_com& marshaled, StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994& unmarshaled);
IL2CPP_EXTERN_C void StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshal_com_cleanup(StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994_marshaled_com& marshaled);

// !!0& Unity.Collections.LowLevel.Unsafe.UnsafeUtilityEx::AsRef<Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders/Runtimes>(System.Void*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * UnsafeUtilityEx_AsRef_TisRuntimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_m196732DD907ED66A6BBB3E110D5E3B755064D826_gshared (void* ___ptr0, const RuntimeMethod* method);
// !0 Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime<Unity.Entities.RetainBlobAssets>::For(Unity.Entities.Entity,!0&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA  StructuralChangeRuntime_For_mC3966AA37DF1BC7582DF6CA3FDAE580A9B61BA9F_gshared (StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___entity0, RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA * ___originalComponent1, const RuntimeMethod* method);
// !0 Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime<Unity.Entities.RetainBlobAssetPtr>::For(Unity.Entities.Entity,!0&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801  StructuralChangeRuntime_For_m5BC634C6DEE1A6F713D79A04682F72C67D2C0EA1_gshared (StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088 * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___entity0, RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 * ___originalComponent1, const RuntimeMethod* method);
// !!0& Unity.Collections.LowLevel.Unsafe.UnsafeUtilityEx::AsRef<Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2>(System.Void*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E * UnsafeUtilityEx_AsRef_TisU3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_mC32F5BF943A75A98DEF186E75D33781DF05A00A9_gshared (void* ___ptr0, const RuntimeMethod* method);
// System.Void Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime<Unity.Entities.RetainBlobAssets>::WriteBack(Unity.Entities.Entity,!0&,!0&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StructuralChangeRuntime_WriteBack_m4F94AC6D28EDD77E7AE73F2E887D0D0D2C6DA0F4_gshared (StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___entity0, RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA * ___lambdaComponent1, RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA * ___originalComponent2, const RuntimeMethod* method);
// System.Void Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime<Unity.Entities.RetainBlobAssetPtr>::WriteBack(Unity.Entities.Entity,!0&,!0&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StructuralChangeRuntime_WriteBack_mD7AE5E6DE2459A9CFB3A07C9BAAB44576C4587F7_gshared (StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088 * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___entity0, RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 * ___lambdaComponent1, RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 * ___originalComponent2, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer::Add<System.Int32>(!!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeAppendBuffer_Add_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m64B262F4DBD6071954E99B75483BA5039BD3F49B_gshared (UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C * __this, int32_t ___value0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.NativeHashMap`2<Unity.Entities.Serialization.SerializeUtility/BlobAssetPtr,System.Int32>::TryGetValue(!0,!1&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool NativeHashMap_2_TryGetValue_mF556662A50EBD61DC1759DC04065BB6A7BE7CDF2_gshared (NativeHashMap_2_tD236D8A3C81093EDA5D488C9CBBFCA0E8431A1C6 * __this, BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447  ___key0, int32_t* ___item1, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer/Reader::ReadNext<System.Int32>(!!0&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Reader_ReadNext_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m660DE3F4792554904EAA494B9CD2C4BAE76C0C41_gshared (Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF * __this, int32_t* ___value0, const RuntimeMethod* method);
// Unity.Burst.SharedStatic`1<!0> Unity.Burst.SharedStatic`1<System.IntPtr>::GetOrCreate<System.Object,System.Object>(System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  SharedStatic_1_GetOrCreate_TisRuntimeObject_TisRuntimeObject_m963F075A174A821A277D3397114CD3ADCDB6BA01_gshared (uint32_t ___alignment0, const RuntimeMethod* method);
// Unity.Burst.SharedStatic`1<!0> Unity.Burst.SharedStatic`1<System.Int32>::GetOrCreate(System.Type,System.Type,System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SharedStatic_1_t6DAF6BFFE7CB6295851C9E4567A9874B214BD088  SharedStatic_1_GetOrCreate_m379EE72FB569F3A26DCF93108ABBF8F81A1CE4E5_gshared (Type_t * ___contextType0, Type_t * ___subContextType1, uint32_t ___alignment2, const RuntimeMethod* method);
// !0& Unity.Burst.SharedStatic`1<System.Int32>::get_Data()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t* SharedStatic_1_get_Data_m6B89546E4ADA23EA53D35BF24C7B9DF1194CFE25_gshared (SharedStatic_1_t6DAF6BFFE7CB6295851C9E4567A9874B214BD088 * __this, const RuntimeMethod* method);
// System.Void Unity.Properties.Property`2<Unity.Entities.Entity,System.Int32>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Property_2__ctor_m2B068683806A6A583C633FCEE64198D60266B4E1_gshared (Property_2_tCABF4DCC1CD1B4A9380E57084825CBE0CD5650CF * __this, const RuntimeMethod* method);
// System.Void Unity.Properties.Property`2<Unity.Entities.Hash128,Unity.Mathematics.uint4>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Property_2__ctor_mA10C37A89F64B051C9DA6010078BDDCF63F886B2_gshared (Property_2_tC735BFFF1BC3639C475ACE130B7C17946AADC830 * __this, const RuntimeMethod* method);
// System.Void Unity.Properties.Property`2<System.Object,Unity.Entities.EntityCommandBuffer>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Property_2__ctor_m2325A83CE7B2A7D91CB7089F9498C3A1CEDA688E_gshared (Property_2_t1AAAEA67B372BF7DAC1D25BD2C1DC5976FE80FEE * __this, const RuntimeMethod* method);
// System.Void Unity.Properties.Property`2<Unity.Entities.SceneSection,Unity.Entities.Hash128>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Property_2__ctor_m7DE529508EC4AD847F196ED89B9D7608D3976EAD_gshared (Property_2_t2BF85B4F1B3C1A7B861BCF86F0135D4C29B3E71A * __this, const RuntimeMethod* method);
// System.Void Unity.Properties.Property`2<Unity.Entities.SceneSection,System.Int32>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Property_2__ctor_mAF911EBD92AA72B392E830E0A324C3D398CF7AB7_gshared (Property_2_t429FDAC3FDDA865DCCA0BEA2701D06815301B7A7 * __this, const RuntimeMethod* method);
// System.Void Unity.Properties.Property`2<Unity.Entities.SceneTag,Unity.Entities.Entity>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Property_2__ctor_mED553372B69CC9749E50E387DE73DD0DDAC6A108_gshared (Property_2_tEAB8BD93E42D6310C52481E57E5BE3E4F63917B0 * __this, const RuntimeMethod* method);
// System.Void Unity.Properties.Property`2<Unity.Entities.SectionMetadataSetup,System.Int32>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Property_2__ctor_m0B519A8910790B943F6709C19F8F47DA8C45B78F_gshared (Property_2_t300D32A567141F1F72A198697AF2D006E2898B23 * __this, const RuntimeMethod* method);
// System.Void Unity.Properties.Property`2<Unity.Mathematics.uint4,System.UInt32>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Property_2__ctor_m715A2E230FCB05B99C9E2E5ECF6C9660116D19E7_gshared (Property_2_tB965C4D1501C79D2FADA36497D75F6F4BEC5DAED * __this, const RuntimeMethod* method);
// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_ISharedComponentData`1/StructuralChangeRuntime<!0> Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_ISharedComponentData`1<Unity.Entities.BlobAssetOwner>::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR StructuralChangeRuntime_t73F4A22CB7388EB3FB14FDAB733CD0AA00539B60  LambdaParameterValueProvider_ISharedComponentData_1_PrepareToExecuteWithStructuralChanges_m2A619A397B3B2222985218AA7CEA6259FEEA0337_gshared (LambdaParameterValueProvider_ISharedComponentData_1_tE871547CB822CE66B2C193D94183ACD389503DF6 * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___componentSystem0, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___query1, const RuntimeMethod* method);
// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime<!0> Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1<Unity.Entities.RetainBlobAssets>::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mD35E9088251B87C54D4C9FE9CE4B2C68F7661646_gshared (LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4 * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___componentSystem0, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___query1, const RuntimeMethod* method);
// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime<!0> Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1<Unity.Entities.RetainBlobAssetBatchPtr>::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR StructuralChangeRuntime_t91CF003F24C722618BFAB2EC15E454A10524C5F5  LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mFB4F12F46E5EC3C3B81DFD695837BB9B81A28A3E_gshared (LambdaParameterValueProvider_IComponentData_1_t1D786064C86636813BA4C8371E0CF96EAC3BAB81 * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___componentSystem0, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___query1, const RuntimeMethod* method);
// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime<!0> Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1<Unity.Entities.RetainBlobAssetPtr>::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088  LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_m2D6A743E5140666061CD5420D95250903AFED096_gshared (LambdaParameterValueProvider_IComponentData_1_t2EFFCFAFE87229878D536B5C44DC62239C46601A * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___componentSystem0, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___query1, const RuntimeMethod* method);

// System.Void Unity.Entities.RetainBlobAssetSystem::<OnUpdate>b__0_2(Unity.Entities.Entity,Unity.Entities.RetainBlobAssets&,Unity.Entities.RetainBlobAssetPtr&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void RetainBlobAssetSystem_U3COnUpdateU3Eb__0_2_mCE794E2D4DDC41A46E983B76814ED3B3002D5797 (RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14 * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___e0, RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA * ___retain1, RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 * ___retainPtr2, const RuntimeMethod* method);
// System.Void Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2::OriginalLambdaBody(Unity.Entities.Entity,Unity.Entities.RetainBlobAssets&,Unity.Entities.RetainBlobAssetPtr&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_OriginalLambdaBody_m55AFF383E4A186B8644608FFE1A88DE8E5CCF1C3 (U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___e0, RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA * ___retain1, RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 * ___retainPtr2, const RuntimeMethod* method);
// System.Void Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider/PerformLambdaDelegate::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PerformLambdaDelegate__ctor_m937C0635EDDDB108699E0518AA9FA488F28FE33A (PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method);
// !!0& Unity.Collections.LowLevel.Unsafe.UnsafeUtilityEx::AsRef<Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders/Runtimes>(System.Void*)
inline Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * UnsafeUtilityEx_AsRef_TisRuntimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_m196732DD907ED66A6BBB3E110D5E3B755064D826 (void* ___ptr0, const RuntimeMethod* method)
{
	return ((  Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * (*) (void*, const RuntimeMethod*))UnsafeUtilityEx_AsRef_TisRuntimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_m196732DD907ED66A6BBB3E110D5E3B755064D826_gshared)(___ptr0, method);
}
// Unity.Entities.Entity Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_Entity/StructuralChangeRuntime::For(Unity.Entities.Entity)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  StructuralChangeRuntime_For_m22977D179C490D9CB74BDFE1010EAF47B53F370A (StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___e0, const RuntimeMethod* method);
// !0 Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime<Unity.Entities.RetainBlobAssets>::For(Unity.Entities.Entity,!0&)
inline RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA  StructuralChangeRuntime_For_mC3966AA37DF1BC7582DF6CA3FDAE580A9B61BA9F (StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___entity0, RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA * ___originalComponent1, const RuntimeMethod* method)
{
	return ((  RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA  (*) (StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA *, const RuntimeMethod*))StructuralChangeRuntime_For_mC3966AA37DF1BC7582DF6CA3FDAE580A9B61BA9F_gshared)(__this, ___entity0, ___originalComponent1, method);
}
// !0 Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime<Unity.Entities.RetainBlobAssetPtr>::For(Unity.Entities.Entity,!0&)
inline RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801  StructuralChangeRuntime_For_m5BC634C6DEE1A6F713D79A04682F72C67D2C0EA1 (StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088 * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___entity0, RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 * ___originalComponent1, const RuntimeMethod* method)
{
	return ((  RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801  (*) (StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 *, const RuntimeMethod*))StructuralChangeRuntime_For_m5BC634C6DEE1A6F713D79A04682F72C67D2C0EA1_gshared)(__this, ___entity0, ___originalComponent1, method);
}
// !!0& Unity.Collections.LowLevel.Unsafe.UnsafeUtilityEx::AsRef<Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2>(System.Void*)
inline U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E * UnsafeUtilityEx_AsRef_TisU3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_mC32F5BF943A75A98DEF186E75D33781DF05A00A9 (void* ___ptr0, const RuntimeMethod* method)
{
	return ((  U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E * (*) (void*, const RuntimeMethod*))UnsafeUtilityEx_AsRef_TisU3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_mC32F5BF943A75A98DEF186E75D33781DF05A00A9_gshared)(___ptr0, method);
}
// System.Void Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime<Unity.Entities.RetainBlobAssets>::WriteBack(Unity.Entities.Entity,!0&,!0&)
inline void StructuralChangeRuntime_WriteBack_m4F94AC6D28EDD77E7AE73F2E887D0D0D2C6DA0F4 (StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___entity0, RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA * ___lambdaComponent1, RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA * ___originalComponent2, const RuntimeMethod* method)
{
	((  void (*) (StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA *, RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA *, const RuntimeMethod*))StructuralChangeRuntime_WriteBack_m4F94AC6D28EDD77E7AE73F2E887D0D0D2C6DA0F4_gshared)(__this, ___entity0, ___lambdaComponent1, ___originalComponent2, method);
}
// System.Void Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime<Unity.Entities.RetainBlobAssetPtr>::WriteBack(Unity.Entities.Entity,!0&,!0&)
inline void StructuralChangeRuntime_WriteBack_mD7AE5E6DE2459A9CFB3A07C9BAAB44576C4587F7 (StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088 * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___entity0, RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 * ___lambdaComponent1, RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 * ___originalComponent2, const RuntimeMethod* method)
{
	((  void (*) (StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 *, RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 *, const RuntimeMethod*))StructuralChangeRuntime_WriteBack_mD7AE5E6DE2459A9CFB3A07C9BAAB44576C4587F7_gshared)(__this, ___entity0, ___lambdaComponent1, ___originalComponent2, method);
}
// Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders/Runtimes Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8  LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_m06AE7D081E5B1F77F983D9608E29D6E801B94B94 (LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31 * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___p00, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___p11, const RuntimeMethod* method);
// System.Void Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider::IterateEntities(System.Void*,System.Void*,Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider/PerformLambdaDelegate)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StructuralChangeEntityProvider_IterateEntities_mBB4379AFF0221F0534021DA21F1532C4A2340B57 (StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994 * __this, void* ___jobStruct0, void* ___runtimes1, PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 * ___action2, const RuntimeMethod* method);
// System.Void Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2::Execute(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_Execute_m42FFAB4AF04DA43543F52A7D37FC0DBE46632952 (U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___componentSystem0, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___query1, const RuntimeMethod* method);
// System.Void Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2::ScheduleTimeInitialize(Unity.Entities.RetainBlobAssetSystem)
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_ScheduleTimeInitialize_m474B58EB99B27E8410150472E373A8B1CFB2E236_inline (U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E * __this, RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14 * ___componentSystem0, const RuntimeMethod* method);
// System.Void System.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Void Unity.Entities.Serialization.SerializeUtility/BlobAssetPtr::.ctor(Unity.Entities.BlobAssetHeader*)
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void BlobAssetPtr__ctor_mA9ADEAB1ED9BDEA24267B1396FFED6278159ADF9_inline (BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 * __this, BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * ___header0, const RuntimeMethod* method);
// System.Boolean Unity.Entities.Serialization.SerializeUtility/BlobAssetPtr::Equals(Unity.Entities.Serialization.SerializeUtility/BlobAssetPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool BlobAssetPtr_Equals_m2A2556081B84EA4DC2FC4E55901ED25C71AFDD6E (BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 * __this, BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447  ___other0, const RuntimeMethod* method);
// System.UInt32 Unity.Mathematics.math::hash(System.Void*,System.Int32,System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t math_hash_mA46DF71F8A49EB5383D32455AC4406A62F28125E (void* ___pBuffer0, int32_t ___numBytes1, uint32_t ___seed2, const RuntimeMethod* method);
// System.Int32 Unity.Entities.Serialization.SerializeUtility/BlobAssetPtr::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t BlobAssetPtr_GetHashCode_mB8700EEFD0D8D608FF0A764AC2051B01819697F8 (BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 * __this, const RuntimeMethod* method);
// Unity.Entities.Entity Unity.Entities.EntityRemapUtility::RemapEntity(Unity.Entities.EntityRemapUtility/EntityRemapInfo*,Unity.Entities.Entity)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  EntityRemapUtility_RemapEntity_mA93F138D8D97A50C334CC618B0B56DA27E8D1525 (EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334 * ___remapping0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___source1, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer::Add<System.Int32>(!!0)
inline void UnsafeAppendBuffer_Add_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m64B262F4DBD6071954E99B75483BA5039BD3F49B (UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C * __this, int32_t ___value0, const RuntimeMethod* method)
{
	((  void (*) (UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C *, int32_t, const RuntimeMethod*))UnsafeAppendBuffer_Add_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m64B262F4DBD6071954E99B75483BA5039BD3F49B_gshared)(__this, ___value0, method);
}
// Unity.Entities.BlobAssetHeader* Unity.Entities.BlobAssetReferenceData::get_Header()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * BlobAssetReferenceData_get_Header_mD60D183AC9001F365C7388CEE97CC9058CFB5C57 (BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1 * __this, const RuntimeMethod* method);
// System.Boolean Unity.Collections.NativeHashMap`2<Unity.Entities.Serialization.SerializeUtility/BlobAssetPtr,System.Int32>::TryGetValue(!0,!1&)
inline bool NativeHashMap_2_TryGetValue_mF556662A50EBD61DC1759DC04065BB6A7BE7CDF2 (NativeHashMap_2_tD236D8A3C81093EDA5D488C9CBBFCA0E8431A1C6 * __this, BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447  ___key0, int32_t* ___item1, const RuntimeMethod* method)
{
	return ((  bool (*) (NativeHashMap_2_tD236D8A3C81093EDA5D488C9CBBFCA0E8431A1C6 *, BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 , int32_t*, const RuntimeMethod*))NativeHashMap_2_TryGetValue_mF556662A50EBD61DC1759DC04065BB6A7BE7CDF2_gshared)(__this, ___key0, ___item1, method);
}
// System.Void System.InvalidOperationException::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void InvalidOperationException__ctor_mC012CE552988309733C896F3FEA8249171E4402E (InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB * __this, String_t* ___message0, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer/Reader::ReadNext<System.Int32>(!!0&)
inline void Reader_ReadNext_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m660DE3F4792554904EAA494B9CD2C4BAE76C0C41 (Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF * __this, int32_t* ___value0, const RuntimeMethod* method)
{
	((  void (*) (Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF *, int32_t*, const RuntimeMethod*))Reader_ReadNext_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m660DE3F4792554904EAA494B9CD2C4BAE76C0C41_gshared)(__this, ___value0, method);
}
// System.Void Unity.Entities.TypeHash/<>c::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__ctor_m53B99CFD5ED8954C89E683DB5B4DE6ADB46CBA7D (U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A * __this, const RuntimeMethod* method);
// System.Boolean System.String::op_Equality(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool String_op_Equality_m2B91EE68355F142F67095973D32EB5828B7B73CB (String_t* ___a0, String_t* ___b1, const RuntimeMethod* method);
// System.Type System.Reflection.CustomAttributeTypedArgument::get_ArgumentType()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Type_t * CustomAttributeTypedArgument_get_ArgumentType_m87769FA596B93DC490F158996486CA1D42C4E84C_inline (CustomAttributeTypedArgument_tE7152E8FACDD29A8E0040E151C86F436FA8E6910 * __this, const RuntimeMethod* method);
// System.Void Unity.Entities.TypeManager/<>c::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__ctor_m36364331876FE744E6AE044372D6F056AB30B79C (U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2 * __this, const RuntimeMethod* method);
// System.Void Unity.Entities.TypeManager::Shutdown()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TypeManager_Shutdown_m1C46D942520BD3565AFD36961B215187F6472471 (const RuntimeMethod* method);
// System.Boolean Unity.Entities.TypeManager::IsZeroSizeStruct(System.Type)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TypeManager_IsZeroSizeStruct_m2D8778E94C86EA3AB136D51FD9C5587B685DE077 (Type_t * ___t0, const RuntimeMethod* method);
// Unity.Burst.SharedStatic`1<!0> Unity.Burst.SharedStatic`1<System.IntPtr>::GetOrCreate<Unity.Entities.TypeManager/TypeManagerKeyContext,Unity.Entities.TypeManager/SharedBlobAssetRefOffset>(System.UInt32)
inline SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedBlobAssetRefOffset_tE4EC29E58257EEC230D11A35AB22B47D9D9DC0E6_m09EB536037556C2CC390754DB83DA6B73AD52912 (uint32_t ___alignment0, const RuntimeMethod* method)
{
	return ((  SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  (*) (uint32_t, const RuntimeMethod*))SharedStatic_1_GetOrCreate_TisRuntimeObject_TisRuntimeObject_m963F075A174A821A277D3397114CD3ADCDB6BA01_gshared)(___alignment0, method);
}
// Unity.Burst.SharedStatic`1<!0> Unity.Burst.SharedStatic`1<System.IntPtr>::GetOrCreate<Unity.Entities.TypeManager/TypeManagerKeyContext,Unity.Entities.TypeManager/SharedEntityOffsetInfo>(System.UInt32)
inline SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedEntityOffsetInfo_tE11E3366E1064E671CD7B5C0B457A4A09DCCC068_mEA2E884221A4CEEBE506AB0F64472ABEC48A85DE (uint32_t ___alignment0, const RuntimeMethod* method)
{
	return ((  SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  (*) (uint32_t, const RuntimeMethod*))SharedStatic_1_GetOrCreate_TisRuntimeObject_TisRuntimeObject_m963F075A174A821A277D3397114CD3ADCDB6BA01_gshared)(___alignment0, method);
}
// System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Type_t * Type_GetTypeFromHandle_m8BB57524FF7F9DB1803BC561D2B3A4DBACEB385E (RuntimeTypeHandle_tC33965ADA3E041E0C94AF05E5CB527B56482CEF9  ___handle0, const RuntimeMethod* method);
// Unity.Burst.SharedStatic`1<!0> Unity.Burst.SharedStatic`1<System.Int32>::GetOrCreate(System.Type,System.Type,System.UInt32)
inline SharedStatic_1_t6DAF6BFFE7CB6295851C9E4567A9874B214BD088  SharedStatic_1_GetOrCreate_m379EE72FB569F3A26DCF93108ABBF8F81A1CE4E5 (Type_t * ___contextType0, Type_t * ___subContextType1, uint32_t ___alignment2, const RuntimeMethod* method)
{
	return ((  SharedStatic_1_t6DAF6BFFE7CB6295851C9E4567A9874B214BD088  (*) (Type_t *, Type_t *, uint32_t, const RuntimeMethod*))SharedStatic_1_GetOrCreate_m379EE72FB569F3A26DCF93108ABBF8F81A1CE4E5_gshared)(___contextType0, ___subContextType1, ___alignment2, method);
}
// !0& Unity.Burst.SharedStatic`1<System.Int32>::get_Data()
inline int32_t* SharedStatic_1_get_Data_m6B89546E4ADA23EA53D35BF24C7B9DF1194CFE25 (SharedStatic_1_t6DAF6BFFE7CB6295851C9E4567A9874B214BD088 * __this, const RuntimeMethod* method)
{
	return ((  int32_t* (*) (SharedStatic_1_t6DAF6BFFE7CB6295851C9E4567A9874B214BD088 *, const RuntimeMethod*))SharedStatic_1_get_Data_m6B89546E4ADA23EA53D35BF24C7B9DF1194CFE25_gshared)(__this, method);
}
// Unity.Burst.SharedStatic`1<!0> Unity.Burst.SharedStatic`1<System.IntPtr>::GetOrCreate<Unity.Entities.TypeManager/TypeManagerKeyContext,Unity.Entities.TypeManager/SharedTypeInfo>(System.UInt32)
inline SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedTypeInfo_t6D9232712A2DC192E8DCC16F0AE24EE2508CCD05_m4D2E48203248786C9D3BFF7BC2205CE779FE7814 (uint32_t ___alignment0, const RuntimeMethod* method)
{
	return ((  SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  (*) (uint32_t, const RuntimeMethod*))SharedStatic_1_GetOrCreate_TisRuntimeObject_TisRuntimeObject_m963F075A174A821A277D3397114CD3ADCDB6BA01_gshared)(___alignment0, method);
}
// Unity.Burst.SharedStatic`1<!0> Unity.Burst.SharedStatic`1<System.IntPtr>::GetOrCreate<Unity.Entities.TypeManager/TypeManagerKeyContext,Unity.Entities.TypeManager/SharedWriteGroup>(System.UInt32)
inline SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedWriteGroup_t43861216B5BB89853E40798D5170B5574E8A46FB_mFA700697C79F3EDB2BC6703FE1267C962261E842 (uint32_t ___alignment0, const RuntimeMethod* method)
{
	return ((  SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  (*) (uint32_t, const RuntimeMethod*))SharedStatic_1_GetOrCreate_TisRuntimeObject_TisRuntimeObject_m963F075A174A821A277D3397114CD3ADCDB6BA01_gshared)(___alignment0, method);
}
// System.Void Unity.Entities.TypeManager/TypeInfo::.ctor(System.Int32,Unity.Entities.TypeManager/TypeCategory,System.Int32,System.Int32,System.UInt64,System.UInt64,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TypeInfo__ctor_m0DFE1CADCF1CA7D60549849B8F7CE8BE574AC099 (TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * __this, int32_t ___typeIndex0, int32_t ___category1, int32_t ___entityOffsetCount2, int32_t ___entityOffsetStartIndex3, uint64_t ___memoryOrdering4, uint64_t ___stableTypeHash5, int32_t ___bufferCapacity6, int32_t ___sizeInChunk7, int32_t ___elementSize8, int32_t ___alignmentInBytes9, int32_t ___maximumChunkCapacity10, int32_t ___writeGroupCount11, int32_t ___writeGroupStartIndex12, int32_t ___blobAssetRefOffsetCount13, int32_t ___blobAssetRefOffsetStartIndex14, int32_t ___fastEqualityIndex15, int32_t ___typeSize16, const RuntimeMethod* method);
// System.Boolean Unity.Entities.TypeManager/TypeInfo::get_IsZeroSized()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TypeInfo_get_IsZeroSized_m3C6016DECE42210DECE2A92633685A569B2E7F0C (TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * __this, const RuntimeMethod* method);
// System.Type Unity.Entities.TypeManager::GetType(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Type_t * TypeManager_GetType_m36FDA8D6746C13AD965E70DF364C6821E1832E35 (int32_t ___typeIndex0, const RuntimeMethod* method);
// System.Type Unity.Entities.TypeManager/TypeInfo::get_Type()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Type_t * TypeInfo_get_Type_m966C85EAB51370AE906FC730D10936E331F86A93 (TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * __this, const RuntimeMethod* method);
// System.Boolean Unity.Entities.TypeManager/TypeInfo::get_HasEntities()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TypeInfo_get_HasEntities_mF0ABFAF0ECC3FDE2CA213C5EE1A730D07E488875 (TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * __this, const RuntimeMethod* method);
// System.Void Unity.Properties.Property`2<Unity.Entities.Entity,System.Int32>::.ctor()
inline void Property_2__ctor_m2B068683806A6A583C633FCEE64198D60266B4E1 (Property_2_tCABF4DCC1CD1B4A9380E57084825CBE0CD5650CF * __this, const RuntimeMethod* method)
{
	((  void (*) (Property_2_tCABF4DCC1CD1B4A9380E57084825CBE0CD5650CF *, const RuntimeMethod*))Property_2__ctor_m2B068683806A6A583C633FCEE64198D60266B4E1_gshared)(__this, method);
}
// System.Void Unity.Properties.Property`2<Unity.Entities.Hash128,Unity.Mathematics.uint4>::.ctor()
inline void Property_2__ctor_mA10C37A89F64B051C9DA6010078BDDCF63F886B2 (Property_2_tC735BFFF1BC3639C475ACE130B7C17946AADC830 * __this, const RuntimeMethod* method)
{
	((  void (*) (Property_2_tC735BFFF1BC3639C475ACE130B7C17946AADC830 *, const RuntimeMethod*))Property_2__ctor_mA10C37A89F64B051C9DA6010078BDDCF63F886B2_gshared)(__this, method);
}
// System.Void Unity.Properties.Property`2<Unity.Entities.PostLoadCommandBuffer,Unity.Entities.EntityCommandBuffer>::.ctor()
inline void Property_2__ctor_mAF509B7CDC2036D7E9EBBF46BD4B439073CDE45C (Property_2_t40BE748BD8D19926CED3273AD0DAECECE3176A34 * __this, const RuntimeMethod* method)
{
	((  void (*) (Property_2_t40BE748BD8D19926CED3273AD0DAECECE3176A34 *, const RuntimeMethod*))Property_2__ctor_m2325A83CE7B2A7D91CB7089F9498C3A1CEDA688E_gshared)(__this, method);
}
// System.Void Unity.Properties.Property`2<Unity.Entities.SceneSection,Unity.Entities.Hash128>::.ctor()
inline void Property_2__ctor_m7DE529508EC4AD847F196ED89B9D7608D3976EAD (Property_2_t2BF85B4F1B3C1A7B861BCF86F0135D4C29B3E71A * __this, const RuntimeMethod* method)
{
	((  void (*) (Property_2_t2BF85B4F1B3C1A7B861BCF86F0135D4C29B3E71A *, const RuntimeMethod*))Property_2__ctor_m7DE529508EC4AD847F196ED89B9D7608D3976EAD_gshared)(__this, method);
}
// System.Void Unity.Properties.Property`2<Unity.Entities.SceneSection,System.Int32>::.ctor()
inline void Property_2__ctor_mAF911EBD92AA72B392E830E0A324C3D398CF7AB7 (Property_2_t429FDAC3FDDA865DCCA0BEA2701D06815301B7A7 * __this, const RuntimeMethod* method)
{
	((  void (*) (Property_2_t429FDAC3FDDA865DCCA0BEA2701D06815301B7A7 *, const RuntimeMethod*))Property_2__ctor_mAF911EBD92AA72B392E830E0A324C3D398CF7AB7_gshared)(__this, method);
}
// System.Void Unity.Properties.Property`2<Unity.Entities.SceneTag,Unity.Entities.Entity>::.ctor()
inline void Property_2__ctor_mED553372B69CC9749E50E387DE73DD0DDAC6A108 (Property_2_tEAB8BD93E42D6310C52481E57E5BE3E4F63917B0 * __this, const RuntimeMethod* method)
{
	((  void (*) (Property_2_tEAB8BD93E42D6310C52481E57E5BE3E4F63917B0 *, const RuntimeMethod*))Property_2__ctor_mED553372B69CC9749E50E387DE73DD0DDAC6A108_gshared)(__this, method);
}
// System.Void Unity.Properties.Property`2<Unity.Entities.SectionMetadataSetup,System.Int32>::.ctor()
inline void Property_2__ctor_m0B519A8910790B943F6709C19F8F47DA8C45B78F (Property_2_t300D32A567141F1F72A198697AF2D006E2898B23 * __this, const RuntimeMethod* method)
{
	((  void (*) (Property_2_t300D32A567141F1F72A198697AF2D006E2898B23 *, const RuntimeMethod*))Property_2__ctor_m0B519A8910790B943F6709C19F8F47DA8C45B78F_gshared)(__this, method);
}
// System.Void Unity.Properties.Property`2<Unity.Mathematics.uint4,System.UInt32>::.ctor()
inline void Property_2__ctor_m715A2E230FCB05B99C9E2E5ECF6C9660116D19E7 (Property_2_tB965C4D1501C79D2FADA36497D75F6F4BEC5DAED * __this, const RuntimeMethod* method)
{
	((  void (*) (Property_2_tB965C4D1501C79D2FADA36497D75F6F4BEC5DAED *, const RuntimeMethod*))Property_2__ctor_m715A2E230FCB05B99C9E2E5ECF6C9660116D19E7_gshared)(__this, method);
}
// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility::Malloc(System.Int64,System.Int32,Unity.Collections.Allocator)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* UnsafeUtility_Malloc_m18FCC67A056C48A4E0F939D08C43F9E876CA1CF6 (int64_t ___size0, int32_t ___alignment1, int32_t ___allocator2, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MemClear(System.Void*,System.Int64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeUtility_MemClear_m9A2B75C85CB8B6637B1286A562A8E35C82772D09 (void* ___destination0, int64_t ___size1, const RuntimeMethod* method);
// System.Void Unity.Entities.World/StateAllocator::Init()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StateAllocator_Init_mB98D03A98BB079DC98429B88AD7F3A518C687F48 (StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * __this, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility::Free(System.Void*,Unity.Collections.Allocator)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeUtility_Free_mA805168FF1B6728E7DF3AD1DE47400B37F3441F9 (void* ___memory0, int32_t ___allocator1, const RuntimeMethod* method);
// System.Void Unity.Entities.World/StateAllocator::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StateAllocator_Dispose_m3D825FC8909235B193459C939BF77334AA21B756 (StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * __this, const RuntimeMethod* method);
// Unity.Entities.SystemState* Unity.Entities.World/StateAllocator::Resolve(System.UInt16,System.UInt16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * StateAllocator_Resolve_m9CDD04D63F464059F7A7B242B6F3DA872D9C3EA0 (StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * __this, uint16_t ___handle0, uint16_t ___version1, const RuntimeMethod* method);
// Unity.Entities.SystemState* Unity.Entities.World/StateAllocator::ResolveNoCheck(System.UInt16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * StateAllocator_ResolveNoCheck_mFE33F97618D65134C886467FC951763D203338F1 (StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * __this, uint16_t ___handle0, const RuntimeMethod* method);
// System.Void Unity.Entities.World/StateAllocator::IncVersion(System.UInt16&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StateAllocator_IncVersion_m8B09B61A0169E5A9020595FF2EAC194A8FB8B308 (uint16_t* ___v0, const RuntimeMethod* method);
// System.Void Unity.Entities.World/StateAllocator::Free(System.UInt16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StateAllocator_Free_m45D1DFD0FB9181C5082B461716267176A8CCC779 (StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * __this, uint16_t ___handle0, const RuntimeMethod* method);
// System.Void Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StructuralChangeEntityProvider_PrepareToExecuteWithStructuralChanges_m869CCD0EEF00648F7E495A94C7309A61B02B5949 (StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994 * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___componentSystem0, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___query1, const RuntimeMethod* method);
// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_Entity/StructuralChangeRuntime Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_Entity::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  LambdaParameterValueProvider_Entity_PrepareToExecuteWithStructuralChanges_m3D096D764543E8D1024D628FDE07EC81C10DF8E3 (LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13 * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___componentSystem0, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___query1, const RuntimeMethod* method);
// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_ISharedComponentData`1/StructuralChangeRuntime<!0> Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_ISharedComponentData`1<Unity.Entities.BlobAssetOwner>::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
inline StructuralChangeRuntime_t73F4A22CB7388EB3FB14FDAB733CD0AA00539B60  LambdaParameterValueProvider_ISharedComponentData_1_PrepareToExecuteWithStructuralChanges_m2A619A397B3B2222985218AA7CEA6259FEEA0337 (LambdaParameterValueProvider_ISharedComponentData_1_tE871547CB822CE66B2C193D94183ACD389503DF6 * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___componentSystem0, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___query1, const RuntimeMethod* method)
{
	return ((  StructuralChangeRuntime_t73F4A22CB7388EB3FB14FDAB733CD0AA00539B60  (*) (LambdaParameterValueProvider_ISharedComponentData_1_tE871547CB822CE66B2C193D94183ACD389503DF6 *, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC *, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109 , const RuntimeMethod*))LambdaParameterValueProvider_ISharedComponentData_1_PrepareToExecuteWithStructuralChanges_m2A619A397B3B2222985218AA7CEA6259FEEA0337_gshared)(__this, ___componentSystem0, ___query1, method);
}
// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime<!0> Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1<Unity.Entities.RetainBlobAssets>::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
inline StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mD35E9088251B87C54D4C9FE9CE4B2C68F7661646 (LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4 * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___componentSystem0, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___query1, const RuntimeMethod* method)
{
	return ((  StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  (*) (LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4 *, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC *, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109 , const RuntimeMethod*))LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mD35E9088251B87C54D4C9FE9CE4B2C68F7661646_gshared)(__this, ___componentSystem0, ___query1, method);
}
// Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0/LambdaParameterValueProviders/Runtimes Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0/LambdaParameterValueProviders::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879  LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_m5D4BC103CAC65745AB7DF1CB09E4DC7CEF6DFE9D (LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7 * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___p00, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___p11, const RuntimeMethod* method);
// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime<!0> Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1<Unity.Entities.RetainBlobAssetBatchPtr>::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
inline StructuralChangeRuntime_t91CF003F24C722618BFAB2EC15E454A10524C5F5  LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mFB4F12F46E5EC3C3B81DFD695837BB9B81A28A3E (LambdaParameterValueProvider_IComponentData_1_t1D786064C86636813BA4C8371E0CF96EAC3BAB81 * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___componentSystem0, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___query1, const RuntimeMethod* method)
{
	return ((  StructuralChangeRuntime_t91CF003F24C722618BFAB2EC15E454A10524C5F5  (*) (LambdaParameterValueProvider_IComponentData_1_t1D786064C86636813BA4C8371E0CF96EAC3BAB81 *, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC *, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109 , const RuntimeMethod*))LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mFB4F12F46E5EC3C3B81DFD695837BB9B81A28A3E_gshared)(__this, ___componentSystem0, ___query1, method);
}
// Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1/LambdaParameterValueProviders/Runtimes Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1/LambdaParameterValueProviders::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF  LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_mAC1D60747A9EA28974FBF31EE269AE7161E901D8 (LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___p00, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___p11, const RuntimeMethod* method);
// Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1/StructuralChangeRuntime<!0> Unity.Entities.CodeGeneratedJobForEach.LambdaParameterValueProvider_IComponentData`1<Unity.Entities.RetainBlobAssetPtr>::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
inline StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088  LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_m2D6A743E5140666061CD5420D95250903AFED096 (LambdaParameterValueProvider_IComponentData_1_t2EFFCFAFE87229878D536B5C44DC62239C46601A * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___componentSystem0, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___query1, const RuntimeMethod* method)
{
	return ((  StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088  (*) (LambdaParameterValueProvider_IComponentData_1_t2EFFCFAFE87229878D536B5C44DC62239C46601A *, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC *, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109 , const RuntimeMethod*))LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_m2D6A743E5140666061CD5420D95250903AFED096_gshared)(__this, ___componentSystem0, ___query1, method);
}
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif


// Conversion methods for marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2
IL2CPP_EXTERN_C void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_marshal_pinvoke(const U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E& unmarshaled, U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_marshaled_pinvoke& marshaled)
{
	Exception_t* ___hostInstance_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'hostInstance' of type '<>c__DisplayClass_OnUpdate_LambdaJob2': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___hostInstance_0Exception, NULL);
}
IL2CPP_EXTERN_C void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_marshal_pinvoke_back(const U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_marshaled_pinvoke& marshaled, U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E& unmarshaled)
{
	Exception_t* ___hostInstance_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'hostInstance' of type '<>c__DisplayClass_OnUpdate_LambdaJob2': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___hostInstance_0Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2
IL2CPP_EXTERN_C void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_marshal_pinvoke_cleanup(U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_marshaled_pinvoke& marshaled)
{
}


// Conversion methods for marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2
IL2CPP_EXTERN_C void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_marshal_com(const U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E& unmarshaled, U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_marshaled_com& marshaled)
{
	Exception_t* ___hostInstance_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'hostInstance' of type '<>c__DisplayClass_OnUpdate_LambdaJob2': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___hostInstance_0Exception, NULL);
}
IL2CPP_EXTERN_C void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_marshal_com_back(const U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_marshaled_com& marshaled, U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E& unmarshaled)
{
	Exception_t* ___hostInstance_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'hostInstance' of type '<>c__DisplayClass_OnUpdate_LambdaJob2': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___hostInstance_0Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2
IL2CPP_EXTERN_C void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_marshal_com_cleanup(U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_marshaled_com& marshaled)
{
}
// System.Void Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2::OriginalLambdaBody(Unity.Entities.Entity,Unity.Entities.RetainBlobAssets&,Unity.Entities.RetainBlobAssetPtr&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_OriginalLambdaBody_m55AFF383E4A186B8644608FFE1A88DE8E5CCF1C3 (U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___e0, RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA * ___retain1, RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 * ___retainPtr2, const RuntimeMethod* method)
{
	{
		RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14 * L_0 = __this->get_hostInstance_0();
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_1 = ___e0;
		RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA * L_2 = ___retain1;
		RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 * L_3 = ___retainPtr2;
		NullCheck(L_0);
		RetainBlobAssetSystem_U3COnUpdateU3Eb__0_2_mCE794E2D4DDC41A46E983B76814ED3B3002D5797(L_0, L_1, (RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA *)L_2, (RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 *)L_3, /*hidden argument*/NULL);
		return;
	}
}
IL2CPP_EXTERN_C  void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_OriginalLambdaBody_m55AFF383E4A186B8644608FFE1A88DE8E5CCF1C3_AdjustorThunk (RuntimeObject * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___e0, RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA * ___retain1, RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 * ___retainPtr2, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E * _thisAdjusted = reinterpret_cast<U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E *>(__this + _offset);
	U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_OriginalLambdaBody_m55AFF383E4A186B8644608FFE1A88DE8E5CCF1C3(_thisAdjusted, ___e0, ___retain1, ___retainPtr2, method);
}
// System.Void Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2__cctor_mDDEEBAB2A3BBAA86821146E7C56E1F45229E8817 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2__cctor_mDDEEBAB2A3BBAA86821146E7C56E1F45229E8817_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 * L_0 = (PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 *)il2cpp_codegen_object_new(PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377_il2cpp_TypeInfo_var);
		PerformLambdaDelegate__ctor_m937C0635EDDDB108699E0518AA9FA488F28FE33A(L_0, NULL, (intptr_t)((intptr_t)U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_PerformLambda_mD6D6751E15D9AE4174AD71C60EC10E32D656D267_RuntimeMethod_var), /*hidden argument*/NULL);
		((U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_StaticFields*)il2cpp_codegen_static_fields_for(U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_il2cpp_TypeInfo_var))->set__performLambdaDelegate_3(L_0);
		return;
	}
}
// System.Void Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2::PerformLambda(System.Void*,System.Void*,Unity.Entities.Entity)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_PerformLambda_mD6D6751E15D9AE4174AD71C60EC10E32D656D267 (void* ___jobStructPtr0, void* ___runtimesPtr1, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___entity2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_PerformLambda_mD6D6751E15D9AE4174AD71C60EC10E32D656D267_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * V_0 = NULL;
	Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  V_1;
	memset((&V_1), 0, sizeof(V_1));
	RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA  V_2;
	memset((&V_2), 0, sizeof(V_2));
	RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA  V_3;
	memset((&V_3), 0, sizeof(V_3));
	RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801  V_4;
	memset((&V_4), 0, sizeof(V_4));
	RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801  V_5;
	memset((&V_5), 0, sizeof(V_5));
	{
		void* L_0 = ___runtimesPtr1;
		Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * L_1 = UnsafeUtilityEx_AsRef_TisRuntimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_m196732DD907ED66A6BBB3E110D5E3B755064D826((void*)(void*)L_0, /*hidden argument*/UnsafeUtilityEx_AsRef_TisRuntimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_m196732DD907ED66A6BBB3E110D5E3B755064D826_RuntimeMethod_var);
		V_0 = (Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 *)L_1;
		Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * L_2 = V_0;
		StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA * L_3 = L_2->get_address_of_runtime_e_1();
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_4 = ___entity2;
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_5 = StructuralChangeRuntime_For_m22977D179C490D9CB74BDFE1010EAF47B53F370A((StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA *)L_3, L_4, /*hidden argument*/NULL);
		V_1 = L_5;
		Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * L_6 = V_0;
		StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B * L_7 = L_6->get_address_of_runtime_retain_2();
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_8 = ___entity2;
		RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA  L_9 = StructuralChangeRuntime_For_mC3966AA37DF1BC7582DF6CA3FDAE580A9B61BA9F((StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B *)L_7, L_8, (RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA *)(&V_2), /*hidden argument*/StructuralChangeRuntime_For_mC3966AA37DF1BC7582DF6CA3FDAE580A9B61BA9F_RuntimeMethod_var);
		V_3 = L_9;
		Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * L_10 = V_0;
		StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088 * L_11 = L_10->get_address_of_runtime_retainPtr_3();
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_12 = ___entity2;
		RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801  L_13 = StructuralChangeRuntime_For_m5BC634C6DEE1A6F713D79A04682F72C67D2C0EA1((StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088 *)L_11, L_12, (RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 *)(&V_4), /*hidden argument*/StructuralChangeRuntime_For_m5BC634C6DEE1A6F713D79A04682F72C67D2C0EA1_RuntimeMethod_var);
		V_5 = L_13;
		void* L_14 = ___jobStructPtr0;
		U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E * L_15 = UnsafeUtilityEx_AsRef_TisU3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_mC32F5BF943A75A98DEF186E75D33781DF05A00A9((void*)(void*)L_14, /*hidden argument*/UnsafeUtilityEx_AsRef_TisU3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_mC32F5BF943A75A98DEF186E75D33781DF05A00A9_RuntimeMethod_var);
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_16 = V_1;
		U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_OriginalLambdaBody_m55AFF383E4A186B8644608FFE1A88DE8E5CCF1C3((U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E *)L_15, L_16, (RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA *)(&V_3), (RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 *)(&V_5), /*hidden argument*/NULL);
		Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * L_17 = V_0;
		StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B * L_18 = L_17->get_address_of_runtime_retain_2();
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_19 = ___entity2;
		StructuralChangeRuntime_WriteBack_m4F94AC6D28EDD77E7AE73F2E887D0D0D2C6DA0F4((StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B *)L_18, L_19, (RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA *)(&V_3), (RetainBlobAssets_t216CEB719DD3972DE5D64216EADDD767673736CA *)(&V_2), /*hidden argument*/StructuralChangeRuntime_WriteBack_m4F94AC6D28EDD77E7AE73F2E887D0D0D2C6DA0F4_RuntimeMethod_var);
		Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * L_20 = V_0;
		StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088 * L_21 = L_20->get_address_of_runtime_retainPtr_3();
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_22 = ___entity2;
		StructuralChangeRuntime_WriteBack_mD7AE5E6DE2459A9CFB3A07C9BAAB44576C4587F7((StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088 *)L_21, L_22, (RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 *)(&V_5), (RetainBlobAssetPtr_tA4C0558BE753AC5606A9B987FF86C56D09E87801 *)(&V_4), /*hidden argument*/StructuralChangeRuntime_WriteBack_mD7AE5E6DE2459A9CFB3A07C9BAAB44576C4587F7_RuntimeMethod_var);
		return;
	}
}
// System.Void Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2::Execute(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_Execute_m42FFAB4AF04DA43543F52A7D37FC0DBE46632952 (U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___componentSystem0, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___query1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_Execute_m42FFAB4AF04DA43543F52A7D37FC0DBE46632952_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31 * L_0 = __this->get_address_of__lambdaParameterValueProviders_1();
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_1 = ___componentSystem0;
		EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  L_2 = ___query1;
		Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8  L_3 = LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_m06AE7D081E5B1F77F983D9608E29D6E801B94B94((LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31 *)L_0, L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		__this->set__runtimes_2((Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 *)(((uintptr_t)(&V_0))));
		StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994 * L_4 = (&V_0)->get_address_of__entityProvider_0();
		Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 * L_5 = __this->get__runtimes_2();
		IL2CPP_RUNTIME_CLASS_INIT(U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_il2cpp_TypeInfo_var);
		PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 * L_6 = ((U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_StaticFields*)il2cpp_codegen_static_fields_for(U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E_il2cpp_TypeInfo_var))->get__performLambdaDelegate_3();
		StructuralChangeEntityProvider_IterateEntities_mBB4379AFF0221F0534021DA21F1532C4A2340B57((StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994 *)L_4, (void*)(void*)__this, (void*)(void*)L_5, L_6, /*hidden argument*/NULL);
		return;
	}
}
IL2CPP_EXTERN_C  void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_Execute_m42FFAB4AF04DA43543F52A7D37FC0DBE46632952_AdjustorThunk (RuntimeObject * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___componentSystem0, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___query1, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E * _thisAdjusted = reinterpret_cast<U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E *>(__this + _offset);
	U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_Execute_m42FFAB4AF04DA43543F52A7D37FC0DBE46632952(_thisAdjusted, ___componentSystem0, ___query1, method);
}
// System.Void Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2::ScheduleTimeInitialize(Unity.Entities.RetainBlobAssetSystem)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_ScheduleTimeInitialize_m474B58EB99B27E8410150472E373A8B1CFB2E236 (U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E * __this, RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14 * ___componentSystem0, const RuntimeMethod* method)
{
	{
		RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14 * L_0 = ___componentSystem0;
		__this->set_hostInstance_0(L_0);
		return;
	}
}
IL2CPP_EXTERN_C  void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_ScheduleTimeInitialize_m474B58EB99B27E8410150472E373A8B1CFB2E236_AdjustorThunk (RuntimeObject * __this, RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14 * ___componentSystem0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E * _thisAdjusted = reinterpret_cast<U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E *>(__this + _offset);
	U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_ScheduleTimeInitialize_m474B58EB99B27E8410150472E373A8B1CFB2E236_inline(_thisAdjusted, ___componentSystem0, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Unity.Entities.ScriptBehaviourUpdateOrder_DummyDelegateWrapper::.ctor(Unity.Entities.ComponentSystemBase)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void DummyDelegateWrapper__ctor_mC9FEC195E1C5909745A50DDF1EE44D90DB284E80 (DummyDelegateWrapper_t4DF24307E0E5460A6AE6062ED870C20D2AAB0D80 * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___sys0, const RuntimeMethod* method)
{
	{
		// public DummyDelegateWrapper(ComponentSystemBase sys)
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405(__this, /*hidden argument*/NULL);
		// m_System = sys;
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_0 = ___sys0;
		__this->set_m_System_0(L_0);
		// }
		return;
	}
}
// System.Void Unity.Entities.ScriptBehaviourUpdateOrder_DummyDelegateWrapper::TriggerUpdate()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void DummyDelegateWrapper_TriggerUpdate_m3B7DBA52BFE3B84CBC41FD2B80E6A8DD75954EB0 (DummyDelegateWrapper_t4DF24307E0E5460A6AE6062ED870C20D2AAB0D80 * __this, const RuntimeMethod* method)
{
	{
		// if (m_System.m_StatePtr != null)
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_0 = __this->get_m_System_0();
		NullCheck(L_0);
		SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * L_1 = L_0->get_m_StatePtr_0();
		if ((((intptr_t)L_1) == ((intptr_t)(((uintptr_t)0)))))
		{
			goto IL_001a;
		}
	}
	{
		// m_System.Update();
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_2 = __this->get_m_System_0();
		NullCheck(L_2);
		VirtActionInvoker0::Invoke(10 /* System.Void Unity.Entities.ComponentSystemBase::Update() */, L_2);
	}

IL_001a:
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
// System.Void Unity.Entities.Serialization.SerializeUtility_BlobAssetPtr::.ctor(Unity.Entities.BlobAssetHeader*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BlobAssetPtr__ctor_mA9ADEAB1ED9BDEA24267B1396FFED6278159ADF9 (BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 * __this, BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * ___header0, const RuntimeMethod* method)
{
	{
		// this.header = header;
		BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * L_0 = ___header0;
		__this->set_header_0((BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F *)L_0);
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void BlobAssetPtr__ctor_mA9ADEAB1ED9BDEA24267B1396FFED6278159ADF9_AdjustorThunk (RuntimeObject * __this, BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * ___header0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 * _thisAdjusted = reinterpret_cast<BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 *>(__this + _offset);
	BlobAssetPtr__ctor_mA9ADEAB1ED9BDEA24267B1396FFED6278159ADF9_inline(_thisAdjusted, ___header0, method);
}
// System.Boolean Unity.Entities.Serialization.SerializeUtility_BlobAssetPtr::Equals(Unity.Entities.Serialization.SerializeUtility_BlobAssetPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool BlobAssetPtr_Equals_m2A2556081B84EA4DC2FC4E55901ED25C71AFDD6E (BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 * __this, BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447  ___other0, const RuntimeMethod* method)
{
	{
		// return header == other.header;
		BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * L_0 = __this->get_header_0();
		BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447  L_1 = ___other0;
		BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * L_2 = L_1.get_header_0();
		return (bool)((((intptr_t)L_0) == ((intptr_t)L_2))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool BlobAssetPtr_Equals_m2A2556081B84EA4DC2FC4E55901ED25C71AFDD6E_AdjustorThunk (RuntimeObject * __this, BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 * _thisAdjusted = reinterpret_cast<BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 *>(__this + _offset);
	return BlobAssetPtr_Equals_m2A2556081B84EA4DC2FC4E55901ED25C71AFDD6E(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Entities.Serialization.SerializeUtility_BlobAssetPtr::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t BlobAssetPtr_GetHashCode_mB8700EEFD0D8D608FF0A764AC2051B01819697F8 (BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 * __this, const RuntimeMethod* method)
{
	BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * V_0 = NULL;
	{
		// BlobAssetHeader* onStack = header;
		BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * L_0 = __this->get_header_0();
		V_0 = (BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F *)L_0;
		// return (int)math.hash(&onStack, sizeof(BlobAssetHeader*));
		uint32_t L_1 = sizeof(BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F *);
		uint32_t L_2 = math_hash_mA46DF71F8A49EB5383D32455AC4406A62F28125E((void*)(void*)(((uintptr_t)(&V_0))), L_1, 0, /*hidden argument*/NULL);
		return L_2;
	}
}
IL2CPP_EXTERN_C  int32_t BlobAssetPtr_GetHashCode_mB8700EEFD0D8D608FF0A764AC2051B01819697F8_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 * _thisAdjusted = reinterpret_cast<BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 *>(__this + _offset);
	return BlobAssetPtr_GetHashCode_mB8700EEFD0D8D608FF0A764AC2051B01819697F8(_thisAdjusted, method);
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
// System.Void Unity.Entities.Serialization.SerializeUtility_ManagedObjectSerializeAdapter::.ctor(Unity.Entities.EntityRemapUtility_EntityRemapInfo*,Unity.Collections.NativeHashMap`2<Unity.Entities.Serialization.SerializeUtility_BlobAssetPtr,System.Int32>,Unity.Collections.NativeArray`1<System.Int32>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ManagedObjectSerializeAdapter__ctor_m3DCA8FBE88B29CD6D048DE26B753D8D390257A1B (ManagedObjectSerializeAdapter_t69285139BD4DB32D092086C7F33A7182FBD24CE9 * __this, EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334 * ___entityRemapInfo0, NativeHashMap_2_tD236D8A3C81093EDA5D488C9CBBFCA0E8431A1C6  ___blobAssetMap1, NativeArray_1_tD60079F06B473C85CF6C2BB4F2D203F38191AE99  ___blobAssetOffsets2, const RuntimeMethod* method)
{
	{
		// public ManagedObjectSerializeAdapter(
		//     EntityRemapUtility.EntityRemapInfo* entityRemapInfo,
		//     NativeHashMap<BlobAssetPtr, int> blobAssetMap,
		//     NativeArray<int> blobAssetOffsets)
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405(__this, /*hidden argument*/NULL);
		// m_EntityRemapInfo = entityRemapInfo;
		EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334 * L_0 = ___entityRemapInfo0;
		__this->set_m_EntityRemapInfo_0((EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334 *)L_0);
		// m_BlobAssetMap = blobAssetMap;
		NativeHashMap_2_tD236D8A3C81093EDA5D488C9CBBFCA0E8431A1C6  L_1 = ___blobAssetMap1;
		__this->set_m_BlobAssetMap_1(L_1);
		// m_BlobAssetOffsets = blobAssetOffsets;
		NativeArray_1_tD60079F06B473C85CF6C2BB4F2D203F38191AE99  L_2 = ___blobAssetOffsets2;
		__this->set_m_BlobAssetOffsets_2(L_2);
		// }
		return;
	}
}
// System.Void Unity.Entities.Serialization.SerializeUtility_ManagedObjectSerializeAdapter::Unity.Serialization.Binary.Adapters.IBinaryAdapter<Unity.Entities.Entity>.Serialize(Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer*,Unity.Entities.Entity)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Serialize_m7B87363B869D66839BC2D8CBCBC581A76D25211E (ManagedObjectSerializeAdapter_t69285139BD4DB32D092086C7F33A7182FBD24CE9 * __this, UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C * ___writer0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___value1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Serialize_m7B87363B869D66839BC2D8CBCBC581A76D25211E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// value = EntityRemapUtility.RemapEntity(m_EntityRemapInfo, value);
		EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334 * L_0 = __this->get_m_EntityRemapInfo_0();
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_1 = ___value1;
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_2 = EntityRemapUtility_RemapEntity_mA93F138D8D97A50C334CC618B0B56DA27E8D1525((EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334 *)(EntityRemapInfo_t646D1453C9CB3176DE318F40D7D61A0A32290334 *)L_0, L_1, /*hidden argument*/NULL);
		___value1 = L_2;
		// writer->Add(value.Index);
		UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C * L_3 = ___writer0;
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_4 = ___value1;
		int32_t L_5 = L_4.get_Index_0();
		UnsafeAppendBuffer_Add_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m64B262F4DBD6071954E99B75483BA5039BD3F49B((UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C *)(UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C *)L_3, L_5, /*hidden argument*/UnsafeAppendBuffer_Add_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m64B262F4DBD6071954E99B75483BA5039BD3F49B_RuntimeMethod_var);
		// writer->Add(value.Version);
		UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C * L_6 = ___writer0;
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_7 = ___value1;
		int32_t L_8 = L_7.get_Version_1();
		UnsafeAppendBuffer_Add_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m64B262F4DBD6071954E99B75483BA5039BD3F49B((UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C *)(UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C *)L_6, L_8, /*hidden argument*/UnsafeAppendBuffer_Add_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m64B262F4DBD6071954E99B75483BA5039BD3F49B_RuntimeMethod_var);
		// }
		return;
	}
}
// System.Void Unity.Entities.Serialization.SerializeUtility_ManagedObjectSerializeAdapter::Unity.Serialization.Binary.Adapters.IBinaryAdapter<Unity.Entities.BlobAssetReferenceData>.Serialize(Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer*,Unity.Entities.BlobAssetReferenceData)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Serialize_m9E1CC1A14745154680CA5776052A64EDBBD221E3 (ManagedObjectSerializeAdapter_t69285139BD4DB32D092086C7F33A7182FBD24CE9 * __this, UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C * ___writer0, BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1  ___value1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Serialize_m9E1CC1A14745154680CA5776052A64EDBBD221E3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	NativeHashMap_2_tD236D8A3C81093EDA5D488C9CBBFCA0E8431A1C6  V_2;
	memset((&V_2), 0, sizeof(V_2));
	NativeArray_1_tD60079F06B473C85CF6C2BB4F2D203F38191AE99  V_3;
	memset((&V_3), 0, sizeof(V_3));
	{
		// var offset = -1;
		V_0 = (-1);
		// if (value.m_Ptr != null)
		BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1  L_0 = ___value1;
		uint8_t* L_1 = L_0.get_m_Ptr_0();
		if ((((intptr_t)L_1) == ((intptr_t)(((uintptr_t)0)))))
		{
			goto IL_0045;
		}
	}
	{
		// if (!m_BlobAssetMap.TryGetValue(new BlobAssetPtr(value.Header), out var index))
		NativeHashMap_2_tD236D8A3C81093EDA5D488C9CBBFCA0E8431A1C6  L_2 = __this->get_m_BlobAssetMap_1();
		V_2 = L_2;
		BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * L_3 = BlobAssetReferenceData_get_Header_mD60D183AC9001F365C7388CEE97CC9058CFB5C57((BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1 *)(&___value1), /*hidden argument*/NULL);
		BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447  L_4;
		memset((&L_4), 0, sizeof(L_4));
		BlobAssetPtr__ctor_mA9ADEAB1ED9BDEA24267B1396FFED6278159ADF9_inline((&L_4), (BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F *)(BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F *)L_3, /*hidden argument*/NULL);
		bool L_5 = NativeHashMap_2_TryGetValue_mF556662A50EBD61DC1759DC04065BB6A7BE7CDF2((NativeHashMap_2_tD236D8A3C81093EDA5D488C9CBBFCA0E8431A1C6 *)(&V_2), L_4, (int32_t*)(&V_1), /*hidden argument*/NativeHashMap_2_TryGetValue_mF556662A50EBD61DC1759DC04065BB6A7BE7CDF2_RuntimeMethod_var);
		if (L_5)
		{
			goto IL_0035;
		}
	}
	{
		// throw new InvalidOperationException($"Trying to serialize a BlobAssetReference but the asset has not been included in the batch.");
		InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB * L_6 = (InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB *)il2cpp_codegen_object_new(InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB_il2cpp_TypeInfo_var);
		InvalidOperationException__ctor_mC012CE552988309733C896F3FEA8249171E4402E(L_6, _stringLiteral96532ED47BDF4CA75B598293EEC8CE8C6A78536E, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_6, ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Serialize_m9E1CC1A14745154680CA5776052A64EDBBD221E3_RuntimeMethod_var);
	}

IL_0035:
	{
		// offset = m_BlobAssetOffsets[index];
		NativeArray_1_tD60079F06B473C85CF6C2BB4F2D203F38191AE99  L_7 = __this->get_m_BlobAssetOffsets_2();
		V_3 = L_7;
		int32_t L_8 = V_1;
		int32_t L_9 = IL2CPP_NATIVEARRAY_GET_ITEM(int32_t, ((NativeArray_1_tD60079F06B473C85CF6C2BB4F2D203F38191AE99 *)(&V_3))->___m_Buffer_0, L_8);
		V_0 = L_9;
	}

IL_0045:
	{
		// writer->Add(offset);
		UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C * L_10 = ___writer0;
		int32_t L_11 = V_0;
		UnsafeAppendBuffer_Add_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m64B262F4DBD6071954E99B75483BA5039BD3F49B((UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C *)(UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C *)L_10, L_11, /*hidden argument*/UnsafeAppendBuffer_Add_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m64B262F4DBD6071954E99B75483BA5039BD3F49B_RuntimeMethod_var);
		// }
		return;
	}
}
// Unity.Entities.Entity Unity.Entities.Serialization.SerializeUtility_ManagedObjectSerializeAdapter::Unity.Serialization.Binary.Adapters.IBinaryAdapter<Unity.Entities.Entity>.Deserialize(Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer_Reader*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Deserialize_m4D1F57847E41AD0699B8F8204A2CD77733C8937E (ManagedObjectSerializeAdapter_t69285139BD4DB32D092086C7F33A7182FBD24CE9 * __this, Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF * ___reader0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Deserialize_m4D1F57847E41AD0699B8F8204A2CD77733C8937E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// => throw new InvalidOperationException($"{nameof(ManagedObjectSerializeAdapter)} should only be used for writing and never for reading!");
		InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB * L_0 = (InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB *)il2cpp_codegen_object_new(InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB_il2cpp_TypeInfo_var);
		InvalidOperationException__ctor_mC012CE552988309733C896F3FEA8249171E4402E(L_0, _stringLiteralF06A9A7490586E316F66A3CF172167416FF081DE, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Deserialize_m4D1F57847E41AD0699B8F8204A2CD77733C8937E_RuntimeMethod_var);
	}
}
// Unity.Entities.BlobAssetReferenceData Unity.Entities.Serialization.SerializeUtility_ManagedObjectSerializeAdapter::Unity.Serialization.Binary.Adapters.IBinaryAdapter<Unity.Entities.BlobAssetReferenceData>.Deserialize(Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer_Reader*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1  ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Deserialize_m4B3E05769084529A62D42C23485B55B1AB626B4A (ManagedObjectSerializeAdapter_t69285139BD4DB32D092086C7F33A7182FBD24CE9 * __this, Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF * ___reader0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Deserialize_m4B3E05769084529A62D42C23485B55B1AB626B4A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// => throw new InvalidOperationException($"{nameof(ManagedObjectSerializeAdapter)} should only be used for writing and never for reading!");
		InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB * L_0 = (InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB *)il2cpp_codegen_object_new(InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB_il2cpp_TypeInfo_var);
		InvalidOperationException__ctor_mC012CE552988309733C896F3FEA8249171E4402E(L_0, _stringLiteralF06A9A7490586E316F66A3CF172167416FF081DE, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, ManagedObjectSerializeAdapter_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Deserialize_m4B3E05769084529A62D42C23485B55B1AB626B4A_RuntimeMethod_var);
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
// System.Void Unity.Entities.Serialization.SerializeUtility_MangedObjectBlobAssetReader::.ctor(System.Byte*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MangedObjectBlobAssetReader__ctor_mC74BB08296F2EF779D27FBEFBE2CAEF7E308F00D (MangedObjectBlobAssetReader_tD15931A858A4079E24BE3FBD8CE40C053A09B172 * __this, uint8_t* ___blobAssetBatch0, const RuntimeMethod* method)
{
	{
		// public MangedObjectBlobAssetReader(byte* blobAssetBatch)
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405(__this, /*hidden argument*/NULL);
		// m_BlobAssetBatch = blobAssetBatch;
		uint8_t* L_0 = ___blobAssetBatch0;
		__this->set_m_BlobAssetBatch_0((uint8_t*)L_0);
		// }
		return;
	}
}
// System.Void Unity.Entities.Serialization.SerializeUtility_MangedObjectBlobAssetReader::Unity.Serialization.Binary.Adapters.IBinaryAdapter<Unity.Entities.BlobAssetReferenceData>.Serialize(Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer*,Unity.Entities.BlobAssetReferenceData)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Serialize_mAA193EB803F57CD76DD8852924B5B95079C5B73D (MangedObjectBlobAssetReader_tD15931A858A4079E24BE3FBD8CE40C053A09B172 * __this, UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C * ___writer0, BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1  ___value1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Serialize_mAA193EB803F57CD76DD8852924B5B95079C5B73D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// => throw new InvalidOperationException($"{nameof(MangedObjectBlobAssetReader)} should only be used for reading and never for writing!");
		InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB * L_0 = (InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB *)il2cpp_codegen_object_new(InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB_il2cpp_TypeInfo_var);
		InvalidOperationException__ctor_mC012CE552988309733C896F3FEA8249171E4402E(L_0, _stringLiteral71086E7E934954D519A3A2106B8936D9C7D8CCD4, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Serialize_mAA193EB803F57CD76DD8852924B5B95079C5B73D_RuntimeMethod_var);
	}
}
// System.Void Unity.Entities.Serialization.SerializeUtility_MangedObjectBlobAssetReader::Unity.Serialization.Binary.Adapters.IBinaryAdapter<Unity.Entities.Entity>.Serialize(Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer*,Unity.Entities.Entity)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Serialize_m8A95B0E4D0A1D8928B0068194A6227BCB6BC87E0 (MangedObjectBlobAssetReader_tD15931A858A4079E24BE3FBD8CE40C053A09B172 * __this, UnsafeAppendBuffer_t9F49FA29F059CF05D0740B0ECF40229C7230F08C * ___writer0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___value1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Serialize_m8A95B0E4D0A1D8928B0068194A6227BCB6BC87E0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// => throw new InvalidOperationException($"{nameof(MangedObjectBlobAssetReader)} should only be used for reading and never for writing!");
		InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB * L_0 = (InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB *)il2cpp_codegen_object_new(InvalidOperationException_t10D3EE59AD28EC641ACEE05BCA4271A527E5ECAB_il2cpp_TypeInfo_var);
		InvalidOperationException__ctor_mC012CE552988309733C896F3FEA8249171E4402E(L_0, _stringLiteral71086E7E934954D519A3A2106B8936D9C7D8CCD4, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Serialize_m8A95B0E4D0A1D8928B0068194A6227BCB6BC87E0_RuntimeMethod_var);
	}
}
// Unity.Entities.Entity Unity.Entities.Serialization.SerializeUtility_MangedObjectBlobAssetReader::Unity.Serialization.Binary.Adapters.IBinaryAdapter<Unity.Entities.Entity>.Deserialize(Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer_Reader*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Deserialize_m5C92A0DB524EC491E26776958C177490A86DBCBA (MangedObjectBlobAssetReader_tD15931A858A4079E24BE3FBD8CE40C053A09B172 * __this, Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF * ___reader0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_EntityU3E_Deserialize_m5C92A0DB524EC491E26776958C177490A86DBCBA_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  V_2;
	memset((&V_2), 0, sizeof(V_2));
	{
		// reader->ReadNext(out int index);
		Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF * L_0 = ___reader0;
		Reader_ReadNext_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m660DE3F4792554904EAA494B9CD2C4BAE76C0C41((Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF *)(Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF *)L_0, (int32_t*)(&V_0), /*hidden argument*/Reader_ReadNext_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m660DE3F4792554904EAA494B9CD2C4BAE76C0C41_RuntimeMethod_var);
		// reader->ReadNext(out int version);
		Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF * L_1 = ___reader0;
		Reader_ReadNext_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m660DE3F4792554904EAA494B9CD2C4BAE76C0C41((Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF *)(Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF *)L_1, (int32_t*)(&V_1), /*hidden argument*/Reader_ReadNext_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m660DE3F4792554904EAA494B9CD2C4BAE76C0C41_RuntimeMethod_var);
		// return new Entity {Index = index, Version = version};
		il2cpp_codegen_initobj((&V_2), sizeof(Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 ));
		int32_t L_2 = V_0;
		(&V_2)->set_Index_0(L_2);
		int32_t L_3 = V_1;
		(&V_2)->set_Version_1(L_3);
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_4 = V_2;
		return L_4;
	}
}
// Unity.Entities.BlobAssetReferenceData Unity.Entities.Serialization.SerializeUtility_MangedObjectBlobAssetReader::Unity.Serialization.Binary.Adapters.IBinaryAdapter<Unity.Entities.BlobAssetReferenceData>.Deserialize(Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer_Reader*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1  MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Deserialize_m990A987C1B24232DBAD02BA6FF9E5589E18EB8F3 (MangedObjectBlobAssetReader_tD15931A858A4079E24BE3FBD8CE40C053A09B172 * __this, Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF * ___reader0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (MangedObjectBlobAssetReader_Unity_Serialization_Binary_Adapters_IBinaryAdapterU3CUnity_Entities_BlobAssetReferenceDataU3E_Deserialize_m990A987C1B24232DBAD02BA6FF9E5589E18EB8F3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		// reader->ReadNext(out int offset);
		Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF * L_0 = ___reader0;
		Reader_ReadNext_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m660DE3F4792554904EAA494B9CD2C4BAE76C0C41((Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF *)(Reader_t080F3BDBAC4801CEAA57ED0A084B929BB8FA3FDF *)L_0, (int32_t*)(&V_0), /*hidden argument*/Reader_ReadNext_TisInt32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_m660DE3F4792554904EAA494B9CD2C4BAE76C0C41_RuntimeMethod_var);
		// return offset == -1 ? default : new BlobAssetReferenceData {m_Ptr = m_BlobAssetBatch + offset};
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)(-1))))
		{
			goto IL_0025;
		}
	}
	{
		il2cpp_codegen_initobj((&V_1), sizeof(BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1 ));
		uint8_t* L_2 = __this->get_m_BlobAssetBatch_0();
		int32_t L_3 = V_0;
		(&V_1)->set_m_Ptr_0((uint8_t*)((uint8_t*)il2cpp_codegen_add((intptr_t)L_2, (int32_t)L_3)));
		BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1  L_4 = V_1;
		return L_4;
	}

IL_0025:
	{
		il2cpp_codegen_initobj((&V_1), sizeof(BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1 ));
		BlobAssetReferenceData_tF3CF19F0706AAF634166E8B725BBFD76D79C6DB1  L_5 = V_1;
		return L_5;
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper__dlg_AddComponentChunks_tD9DDFAADFC2F439BBB9B8CEAA163B6CFF9D95D92 (_dlg_AddComponentChunks_tD9DDFAADFC2F439BBB9B8CEAA163B6CFF9D95D92 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D * ___chunks1, int32_t ___chunkCount2, int32_t ___typeIndex3, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3);

}
// System.Void Unity.Entities.StructuralChange__dlg_AddComponentChunks::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_AddComponentChunks__ctor_mC18FEDA507B561661E102C000B6CF0CB22159D4C (_dlg_AddComponentChunks_tD9DDFAADFC2F439BBB9B8CEAA163B6CFF9D95D92 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void Unity.Entities.StructuralChange__dlg_AddComponentChunks::Invoke(Unity.Entities.EntityComponentStore*,Unity.Entities.ArchetypeChunk*,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_AddComponentChunks_Invoke_mFC53E45EFB696D032660B2B88ABC44376EE3CCB1 (_dlg_AddComponentChunks_tD9DDFAADFC2F439BBB9B8CEAA163B6CFF9D95D92 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D * ___chunks1, int32_t ___chunkCount2, int32_t ___typeIndex3, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3);
					else
						GenericVirtActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3);
					else
						VirtActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___entityComponentStore0) - 1), ___chunks1, ___chunkCount2, ___typeIndex3, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult Unity.Entities.StructuralChange__dlg_AddComponentChunks::BeginInvoke(Unity.Entities.EntityComponentStore*,Unity.Entities.ArchetypeChunk*,System.Int32,System.Int32,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* _dlg_AddComponentChunks_BeginInvoke_mC267BD0DB0AE2BA18C10578ED5475458AFD6C811 (_dlg_AddComponentChunks_tD9DDFAADFC2F439BBB9B8CEAA163B6CFF9D95D92 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D * ___chunks1, int32_t ___chunkCount2, int32_t ___typeIndex3, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback4, RuntimeObject * ___object5, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_dlg_AddComponentChunks_BeginInvoke_mC267BD0DB0AE2BA18C10578ED5475458AFD6C811_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[5] = {0};
	__d_args[0] = ___entityComponentStore0;
	__d_args[1] = ___chunks1;
	__d_args[2] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___chunkCount2);
	__d_args[3] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___typeIndex3);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback4, (RuntimeObject*)___object5);
}
// System.Void Unity.Entities.StructuralChange__dlg_AddComponentChunks::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_AddComponentChunks_EndInvoke_m13B28C8D574C4CF8FEBF86317BE4F3D0C0894BD3 (_dlg_AddComponentChunks_tD9DDFAADFC2F439BBB9B8CEAA163B6CFF9D95D92 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper__dlg_AddComponentEntitiesBatch_tA45112FB52366C0ADDEBC039028B24BEFC59B175 (_dlg_AddComponentEntitiesBatch_tA45112FB52366C0ADDEBC039028B24BEFC59B175 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * ___entityBatchList1, int32_t ___typeIndex2, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(___entityComponentStore0, ___entityBatchList1, ___typeIndex2);

}
// System.Void Unity.Entities.StructuralChange__dlg_AddComponentEntitiesBatch::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_AddComponentEntitiesBatch__ctor_mF6A133874718F5AF839A2C9495EB1660A2F8F26A (_dlg_AddComponentEntitiesBatch_tA45112FB52366C0ADDEBC039028B24BEFC59B175 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void Unity.Entities.StructuralChange__dlg_AddComponentEntitiesBatch::Invoke(Unity.Entities.EntityComponentStore*,Unity.Collections.LowLevel.Unsafe.UnsafeList*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_AddComponentEntitiesBatch_Invoke_mEC8742B011249575283DAF0EE53B3D3DB4535D87 (_dlg_AddComponentEntitiesBatch_tA45112FB52366C0ADDEBC039028B24BEFC59B175 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * ___entityBatchList1, int32_t ___typeIndex2, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___entityBatchList1, ___typeIndex2, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___entityBatchList1, ___typeIndex2, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___entityBatchList1, ___typeIndex2, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker3< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___entityBatchList1, ___typeIndex2);
					else
						GenericVirtActionInvoker3< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___entityBatchList1, ___typeIndex2);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker3< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___entityComponentStore0, ___entityBatchList1, ___typeIndex2);
					else
						VirtActionInvoker3< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___entityComponentStore0, ___entityBatchList1, ___typeIndex2);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___entityComponentStore0) - 1), ___entityBatchList1, ___typeIndex2, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___entityBatchList1, ___typeIndex2, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult Unity.Entities.StructuralChange__dlg_AddComponentEntitiesBatch::BeginInvoke(Unity.Entities.EntityComponentStore*,Unity.Collections.LowLevel.Unsafe.UnsafeList*,System.Int32,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* _dlg_AddComponentEntitiesBatch_BeginInvoke_m87A7EF41430ED10895A5DC0D558A11077C9A84ED (_dlg_AddComponentEntitiesBatch_tA45112FB52366C0ADDEBC039028B24BEFC59B175 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * ___entityBatchList1, int32_t ___typeIndex2, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback3, RuntimeObject * ___object4, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_dlg_AddComponentEntitiesBatch_BeginInvoke_m87A7EF41430ED10895A5DC0D558A11077C9A84ED_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[4] = {0};
	__d_args[0] = ___entityComponentStore0;
	__d_args[1] = ___entityBatchList1;
	__d_args[2] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___typeIndex2);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback3, (RuntimeObject*)___object4);
}
// System.Void Unity.Entities.StructuralChange__dlg_AddComponentEntitiesBatch::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_AddComponentEntitiesBatch_EndInvoke_m38E4C0C2CD579DC87889CE7813FAA9C085F051F7 (_dlg_AddComponentEntitiesBatch_tA45112FB52366C0ADDEBC039028B24BEFC59B175 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  bool DelegatePInvokeWrapper__dlg_AddComponentEntity_tCDE23991E1FA8C395DCCC00BCD0316E834300D95 (_dlg_AddComponentEntity_tCDE23991E1FA8C395DCCC00BCD0316E834300D95 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___entity1, int32_t ___typeIndex2, const RuntimeMethod* method)
{
	typedef int32_t (DEFAULT_CALL *PInvokeFunc)(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	int32_t returnValue = il2cppPInvokeFunc(___entityComponentStore0, ___entity1, ___typeIndex2);

	return static_cast<bool>(returnValue);
}
// System.Void Unity.Entities.StructuralChange__dlg_AddComponentEntity::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_AddComponentEntity__ctor_m9DBDAF1AFDCE29806B9550D45520AA60FB47C7F0 (_dlg_AddComponentEntity_tCDE23991E1FA8C395DCCC00BCD0316E834300D95 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Boolean Unity.Entities.StructuralChange__dlg_AddComponentEntity::Invoke(Unity.Entities.EntityComponentStore*,Unity.Entities.Entity*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool _dlg_AddComponentEntity_Invoke_m49EDFA664C4C3784B6D5D7BAB9596AF3A1735BF1 (_dlg_AddComponentEntity_tCDE23991E1FA8C395DCCC00BCD0316E834300D95 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___entity1, int32_t ___typeIndex2, const RuntimeMethod* method)
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
			if (___parameterCount == 3)
			{
				// open
				typedef bool (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___entity1, ___typeIndex2, targetMethod);
			}
			else
			{
				// closed
				typedef bool (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___entity1, ___typeIndex2, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef bool (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___entity1, ___typeIndex2, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker3< bool, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___entity1, ___typeIndex2);
					else
						result = GenericVirtFuncInvoker3< bool, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___entity1, ___typeIndex2);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker3< bool, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___entityComponentStore0, ___entity1, ___typeIndex2);
					else
						result = VirtFuncInvoker3< bool, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___entityComponentStore0, ___entity1, ___typeIndex2);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef bool (*FunctionPointerType) (RuntimeObject*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___entityComponentStore0) - 1), ___entity1, ___typeIndex2, targetMethod);
				}
				else
				{
					typedef bool (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___entity1, ___typeIndex2, targetMethod);
				}
			}
		}
	}
	return result;
}
// System.IAsyncResult Unity.Entities.StructuralChange__dlg_AddComponentEntity::BeginInvoke(Unity.Entities.EntityComponentStore*,Unity.Entities.Entity*,System.Int32,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* _dlg_AddComponentEntity_BeginInvoke_mE3DFBBCE973CC4CA8E79D20918AF51D2E4364950 (_dlg_AddComponentEntity_tCDE23991E1FA8C395DCCC00BCD0316E834300D95 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___entity1, int32_t ___typeIndex2, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback3, RuntimeObject * ___object4, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_dlg_AddComponentEntity_BeginInvoke_mE3DFBBCE973CC4CA8E79D20918AF51D2E4364950_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[4] = {0};
	__d_args[0] = ___entityComponentStore0;
	__d_args[1] = ___entity1;
	__d_args[2] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___typeIndex2);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback3, (RuntimeObject*)___object4);
}
// System.Boolean Unity.Entities.StructuralChange__dlg_AddComponentEntity::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool _dlg_AddComponentEntity_EndInvoke_m2E6F1411287A063EA9051721A81AAE98FAD619D9 (_dlg_AddComponentEntity_tCDE23991E1FA8C395DCCC00BCD0316E834300D95 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper__dlg_AddSharedComponentChunks_t0BF4E29BC434846F82C9401CDF67A851FA5A58E3 (_dlg_AddSharedComponentChunks_t0BF4E29BC434846F82C9401CDF67A851FA5A58E3 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D * ___chunks1, int32_t ___chunkCount2, int32_t ___componentTypeIndex3, int32_t ___sharedComponentIndex4, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, int32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentTypeIndex3, ___sharedComponentIndex4);

}
// System.Void Unity.Entities.StructuralChange__dlg_AddSharedComponentChunks::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_AddSharedComponentChunks__ctor_m69B4DC9861EABA060EFEF7FF877D85F37A9BEEF3 (_dlg_AddSharedComponentChunks_t0BF4E29BC434846F82C9401CDF67A851FA5A58E3 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void Unity.Entities.StructuralChange__dlg_AddSharedComponentChunks::Invoke(Unity.Entities.EntityComponentStore*,Unity.Entities.ArchetypeChunk*,System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_AddSharedComponentChunks_Invoke_m98FBDE7ED8701A69BFAF44882C3C0271CABDCF3B (_dlg_AddSharedComponentChunks_t0BF4E29BC434846F82C9401CDF67A851FA5A58E3 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D * ___chunks1, int32_t ___chunkCount2, int32_t ___componentTypeIndex3, int32_t ___sharedComponentIndex4, const RuntimeMethod* method)
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
			if (___parameterCount == 5)
			{
				// open
				typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentTypeIndex3, ___sharedComponentIndex4, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentTypeIndex3, ___sharedComponentIndex4, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentTypeIndex3, ___sharedComponentIndex4, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker5< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentTypeIndex3, ___sharedComponentIndex4);
					else
						GenericVirtActionInvoker5< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentTypeIndex3, ___sharedComponentIndex4);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker5< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentTypeIndex3, ___sharedComponentIndex4);
					else
						VirtActionInvoker5< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentTypeIndex3, ___sharedComponentIndex4);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___entityComponentStore0) - 1), ___chunks1, ___chunkCount2, ___componentTypeIndex3, ___sharedComponentIndex4, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentTypeIndex3, ___sharedComponentIndex4, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult Unity.Entities.StructuralChange__dlg_AddSharedComponentChunks::BeginInvoke(Unity.Entities.EntityComponentStore*,Unity.Entities.ArchetypeChunk*,System.Int32,System.Int32,System.Int32,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* _dlg_AddSharedComponentChunks_BeginInvoke_m009E6E1F120D698256D3A01D8FC89CF6F1CF7BDC (_dlg_AddSharedComponentChunks_t0BF4E29BC434846F82C9401CDF67A851FA5A58E3 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D * ___chunks1, int32_t ___chunkCount2, int32_t ___componentTypeIndex3, int32_t ___sharedComponentIndex4, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback5, RuntimeObject * ___object6, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_dlg_AddSharedComponentChunks_BeginInvoke_m009E6E1F120D698256D3A01D8FC89CF6F1CF7BDC_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[6] = {0};
	__d_args[0] = ___entityComponentStore0;
	__d_args[1] = ___chunks1;
	__d_args[2] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___chunkCount2);
	__d_args[3] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___componentTypeIndex3);
	__d_args[4] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___sharedComponentIndex4);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback5, (RuntimeObject*)___object6);
}
// System.Void Unity.Entities.StructuralChange__dlg_AddSharedComponentChunks::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_AddSharedComponentChunks_EndInvoke_m48967ACA680C1329563984B7E0E1349ED6F3C563 (_dlg_AddSharedComponentChunks_t0BF4E29BC434846F82C9401CDF67A851FA5A58E3 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper__dlg_CreateEntity_t418D1599A9300D4C8883E329439F01C2CB94856F (_dlg_CreateEntity_t418D1599A9300D4C8883E329439F01C2CB94856F * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, void* ___archetype1, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___outEntities2, int32_t ___count3, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(___entityComponentStore0, ___archetype1, ___outEntities2, ___count3);

}
// System.Void Unity.Entities.StructuralChange__dlg_CreateEntity::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_CreateEntity__ctor_mEEBEEF2C8BF56A4C5A9DA6A473FFB78A08568C71 (_dlg_CreateEntity_t418D1599A9300D4C8883E329439F01C2CB94856F * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void Unity.Entities.StructuralChange__dlg_CreateEntity::Invoke(Unity.Entities.EntityComponentStore*,System.Void*,Unity.Entities.Entity*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_CreateEntity_Invoke_m92DF59CF9F7F0910B8936A5790AD86EF6CD28B81 (_dlg_CreateEntity_t418D1599A9300D4C8883E329439F01C2CB94856F * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, void* ___archetype1, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___outEntities2, int32_t ___count3, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___archetype1, ___outEntities2, ___count3, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___archetype1, ___outEntities2, ___count3, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___archetype1, ___outEntities2, ___count3, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___archetype1, ___outEntities2, ___count3);
					else
						GenericVirtActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___archetype1, ___outEntities2, ___count3);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___entityComponentStore0, ___archetype1, ___outEntities2, ___count3);
					else
						VirtActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___entityComponentStore0, ___archetype1, ___outEntities2, ___count3);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___entityComponentStore0) - 1), ___archetype1, ___outEntities2, ___count3, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___archetype1, ___outEntities2, ___count3, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult Unity.Entities.StructuralChange__dlg_CreateEntity::BeginInvoke(Unity.Entities.EntityComponentStore*,System.Void*,Unity.Entities.Entity*,System.Int32,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* _dlg_CreateEntity_BeginInvoke_mC94624274AAFC20D0C15497F2FD6F72BA90C51A4 (_dlg_CreateEntity_t418D1599A9300D4C8883E329439F01C2CB94856F * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, void* ___archetype1, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___outEntities2, int32_t ___count3, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback4, RuntimeObject * ___object5, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_dlg_CreateEntity_BeginInvoke_mC94624274AAFC20D0C15497F2FD6F72BA90C51A4_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[5] = {0};
	__d_args[0] = ___entityComponentStore0;
	__d_args[1] = ___archetype1;
	__d_args[2] = ___outEntities2;
	__d_args[3] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___count3);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback4, (RuntimeObject*)___object5);
}
// System.Void Unity.Entities.StructuralChange__dlg_CreateEntity::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_CreateEntity_EndInvoke_m97B152F5FD1AF53C10F9EF556DF53D864607397F (_dlg_CreateEntity_t418D1599A9300D4C8883E329439F01C2CB94856F * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper__dlg_InstantiateEntities_t1F5957B60C35DE9F8691CBDC46C67DCD4A29B678 (_dlg_InstantiateEntities_t1F5957B60C35DE9F8691CBDC46C67DCD4A29B678 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___srcEntity1, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___outputEntities2, int32_t ___instanceCount3, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(___entityComponentStore0, ___srcEntity1, ___outputEntities2, ___instanceCount3);

}
// System.Void Unity.Entities.StructuralChange__dlg_InstantiateEntities::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_InstantiateEntities__ctor_m6D5B84920570E0889610381F665F5BD60DA592BE (_dlg_InstantiateEntities_t1F5957B60C35DE9F8691CBDC46C67DCD4A29B678 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void Unity.Entities.StructuralChange__dlg_InstantiateEntities::Invoke(Unity.Entities.EntityComponentStore*,Unity.Entities.Entity*,Unity.Entities.Entity*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_InstantiateEntities_Invoke_m1ED47F88ECFAE294F1178A823B9CB30D2BCC4A3F (_dlg_InstantiateEntities_t1F5957B60C35DE9F8691CBDC46C67DCD4A29B678 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___srcEntity1, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___outputEntities2, int32_t ___instanceCount3, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___srcEntity1, ___outputEntities2, ___instanceCount3, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___srcEntity1, ___outputEntities2, ___instanceCount3, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___srcEntity1, ___outputEntities2, ___instanceCount3, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___srcEntity1, ___outputEntities2, ___instanceCount3);
					else
						GenericVirtActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___srcEntity1, ___outputEntities2, ___instanceCount3);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___entityComponentStore0, ___srcEntity1, ___outputEntities2, ___instanceCount3);
					else
						VirtActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___entityComponentStore0, ___srcEntity1, ___outputEntities2, ___instanceCount3);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___entityComponentStore0) - 1), ___srcEntity1, ___outputEntities2, ___instanceCount3, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___srcEntity1, ___outputEntities2, ___instanceCount3, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult Unity.Entities.StructuralChange__dlg_InstantiateEntities::BeginInvoke(Unity.Entities.EntityComponentStore*,Unity.Entities.Entity*,Unity.Entities.Entity*,System.Int32,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* _dlg_InstantiateEntities_BeginInvoke_m4A258509A42C18578D8BC4F9841882BAC6AF670E (_dlg_InstantiateEntities_t1F5957B60C35DE9F8691CBDC46C67DCD4A29B678 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___srcEntity1, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___outputEntities2, int32_t ___instanceCount3, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback4, RuntimeObject * ___object5, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_dlg_InstantiateEntities_BeginInvoke_m4A258509A42C18578D8BC4F9841882BAC6AF670E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[5] = {0};
	__d_args[0] = ___entityComponentStore0;
	__d_args[1] = ___srcEntity1;
	__d_args[2] = ___outputEntities2;
	__d_args[3] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___instanceCount3);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback4, (RuntimeObject*)___object5);
}
// System.Void Unity.Entities.StructuralChange__dlg_InstantiateEntities::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_InstantiateEntities_EndInvoke_m859C4CBBD3DF8D6E6105BEFCB4620AD08B6D77B8 (_dlg_InstantiateEntities_t1F5957B60C35DE9F8691CBDC46C67DCD4A29B678 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper__dlg_MoveEntityArchetype_t8C03493F5C6FB58AAECDD0B6DBFA736F5BD204A5 (_dlg_MoveEntityArchetype_t8C03493F5C6FB58AAECDD0B6DBFA736F5BD204A5 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___entity1, void* ___dstArchetype2, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, void*);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(___entityComponentStore0, ___entity1, ___dstArchetype2);

}
// System.Void Unity.Entities.StructuralChange__dlg_MoveEntityArchetype::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_MoveEntityArchetype__ctor_m35EF019F6A476FBF94627E2F94879FE0F0916A8F (_dlg_MoveEntityArchetype_t8C03493F5C6FB58AAECDD0B6DBFA736F5BD204A5 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void Unity.Entities.StructuralChange__dlg_MoveEntityArchetype::Invoke(Unity.Entities.EntityComponentStore*,Unity.Entities.Entity*,System.Void*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_MoveEntityArchetype_Invoke_mEC79098DC1250FDF810BB5B6D4135A683BD73899 (_dlg_MoveEntityArchetype_t8C03493F5C6FB58AAECDD0B6DBFA736F5BD204A5 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___entity1, void* ___dstArchetype2, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, void*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___entity1, ___dstArchetype2, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, void*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___entity1, ___dstArchetype2, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, void*, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___entity1, ___dstArchetype2, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker3< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, void* >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___entity1, ___dstArchetype2);
					else
						GenericVirtActionInvoker3< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, void* >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___entity1, ___dstArchetype2);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker3< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, void* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___entityComponentStore0, ___entity1, ___dstArchetype2);
					else
						VirtActionInvoker3< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, void* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___entityComponentStore0, ___entity1, ___dstArchetype2);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, void*, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___entityComponentStore0) - 1), ___entity1, ___dstArchetype2, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, void*, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___entity1, ___dstArchetype2, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult Unity.Entities.StructuralChange__dlg_MoveEntityArchetype::BeginInvoke(Unity.Entities.EntityComponentStore*,Unity.Entities.Entity*,System.Void*,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* _dlg_MoveEntityArchetype_BeginInvoke_mD696FD8C0E13570B758DDA0410F9D605648664E9 (_dlg_MoveEntityArchetype_t8C03493F5C6FB58AAECDD0B6DBFA736F5BD204A5 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___entity1, void* ___dstArchetype2, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback3, RuntimeObject * ___object4, const RuntimeMethod* method)
{
	void *__d_args[4] = {0};
	__d_args[0] = ___entityComponentStore0;
	__d_args[1] = ___entity1;
	__d_args[2] = ___dstArchetype2;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback3, (RuntimeObject*)___object4);
}
// System.Void Unity.Entities.StructuralChange__dlg_MoveEntityArchetype::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_MoveEntityArchetype_EndInvoke_m526F7272A91568090E1449DD76A21889008C4B18 (_dlg_MoveEntityArchetype_t8C03493F5C6FB58AAECDD0B6DBFA736F5BD204A5 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper__dlg_RemoveComponentChunks_t156D7CDE1CB0A8A575514C20309E1392F8BE65F9 (_dlg_RemoveComponentChunks_t156D7CDE1CB0A8A575514C20309E1392F8BE65F9 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D * ___chunks1, int32_t ___chunkCount2, int32_t ___typeIndex3, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3);

}
// System.Void Unity.Entities.StructuralChange__dlg_RemoveComponentChunks::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_RemoveComponentChunks__ctor_mED5B40D53789458AA090516BB6EF62A206237C87 (_dlg_RemoveComponentChunks_t156D7CDE1CB0A8A575514C20309E1392F8BE65F9 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void Unity.Entities.StructuralChange__dlg_RemoveComponentChunks::Invoke(Unity.Entities.EntityComponentStore*,Unity.Entities.ArchetypeChunk*,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_RemoveComponentChunks_Invoke_m3CDFA248711C2BA6BE0EDB5810CC9ACFB12F1515 (_dlg_RemoveComponentChunks_t156D7CDE1CB0A8A575514C20309E1392F8BE65F9 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D * ___chunks1, int32_t ___chunkCount2, int32_t ___typeIndex3, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3);
					else
						GenericVirtActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3);
					else
						VirtActionInvoker4< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___entityComponentStore0) - 1), ___chunks1, ___chunkCount2, ___typeIndex3, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___typeIndex3, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult Unity.Entities.StructuralChange__dlg_RemoveComponentChunks::BeginInvoke(Unity.Entities.EntityComponentStore*,Unity.Entities.ArchetypeChunk*,System.Int32,System.Int32,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* _dlg_RemoveComponentChunks_BeginInvoke_m38B404E4A10C3436F0DAEC70087FF7AA69A2C8FA (_dlg_RemoveComponentChunks_t156D7CDE1CB0A8A575514C20309E1392F8BE65F9 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D * ___chunks1, int32_t ___chunkCount2, int32_t ___typeIndex3, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback4, RuntimeObject * ___object5, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_dlg_RemoveComponentChunks_BeginInvoke_m38B404E4A10C3436F0DAEC70087FF7AA69A2C8FA_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[5] = {0};
	__d_args[0] = ___entityComponentStore0;
	__d_args[1] = ___chunks1;
	__d_args[2] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___chunkCount2);
	__d_args[3] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___typeIndex3);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback4, (RuntimeObject*)___object5);
}
// System.Void Unity.Entities.StructuralChange__dlg_RemoveComponentChunks::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_RemoveComponentChunks_EndInvoke_m9E13995D1DBB5277C516EF4C4BB13F28C1C4B292 (_dlg_RemoveComponentChunks_t156D7CDE1CB0A8A575514C20309E1392F8BE65F9 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper__dlg_RemoveComponentEntitiesBatch_t092F3F9E00E3B1F1F6A33A456E19C0E7ABE992E1 (_dlg_RemoveComponentEntitiesBatch_t092F3F9E00E3B1F1F6A33A456E19C0E7ABE992E1 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * ___entityBatchList1, int32_t ___typeIndex2, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(___entityComponentStore0, ___entityBatchList1, ___typeIndex2);

}
// System.Void Unity.Entities.StructuralChange__dlg_RemoveComponentEntitiesBatch::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_RemoveComponentEntitiesBatch__ctor_m678CBEA563241D38E046E78CF4CB490476BEF76B (_dlg_RemoveComponentEntitiesBatch_t092F3F9E00E3B1F1F6A33A456E19C0E7ABE992E1 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void Unity.Entities.StructuralChange__dlg_RemoveComponentEntitiesBatch::Invoke(Unity.Entities.EntityComponentStore*,Unity.Collections.LowLevel.Unsafe.UnsafeList*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_RemoveComponentEntitiesBatch_Invoke_mBA40F3293C260339E2A395F9BE3E2D531DC159BF (_dlg_RemoveComponentEntitiesBatch_t092F3F9E00E3B1F1F6A33A456E19C0E7ABE992E1 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * ___entityBatchList1, int32_t ___typeIndex2, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___entityBatchList1, ___typeIndex2, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___entityBatchList1, ___typeIndex2, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___entityBatchList1, ___typeIndex2, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker3< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___entityBatchList1, ___typeIndex2);
					else
						GenericVirtActionInvoker3< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___entityBatchList1, ___typeIndex2);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker3< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___entityComponentStore0, ___entityBatchList1, ___typeIndex2);
					else
						VirtActionInvoker3< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___entityComponentStore0, ___entityBatchList1, ___typeIndex2);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___entityComponentStore0) - 1), ___entityBatchList1, ___typeIndex2, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA *, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___entityBatchList1, ___typeIndex2, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult Unity.Entities.StructuralChange__dlg_RemoveComponentEntitiesBatch::BeginInvoke(Unity.Entities.EntityComponentStore*,Unity.Collections.LowLevel.Unsafe.UnsafeList*,System.Int32,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* _dlg_RemoveComponentEntitiesBatch_BeginInvoke_mDB2EE88F6AFB5912A8896D6A13FEDA04EA42C323 (_dlg_RemoveComponentEntitiesBatch_t092F3F9E00E3B1F1F6A33A456E19C0E7ABE992E1 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, UnsafeList_t45363E05DB545743D4FBBA9793AA68E6A32634AA * ___entityBatchList1, int32_t ___typeIndex2, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback3, RuntimeObject * ___object4, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_dlg_RemoveComponentEntitiesBatch_BeginInvoke_mDB2EE88F6AFB5912A8896D6A13FEDA04EA42C323_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[4] = {0};
	__d_args[0] = ___entityComponentStore0;
	__d_args[1] = ___entityBatchList1;
	__d_args[2] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___typeIndex2);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback3, (RuntimeObject*)___object4);
}
// System.Void Unity.Entities.StructuralChange__dlg_RemoveComponentEntitiesBatch::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_RemoveComponentEntitiesBatch_EndInvoke_m03F64F208E3D88DE0A8D1F79B6BBDEA0822B3368 (_dlg_RemoveComponentEntitiesBatch_t092F3F9E00E3B1F1F6A33A456E19C0E7ABE992E1 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  bool DelegatePInvokeWrapper__dlg_RemoveComponentEntity_t031F2F7279291D7654E794B4B7EB273464D764E6 (_dlg_RemoveComponentEntity_t031F2F7279291D7654E794B4B7EB273464D764E6 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___entity1, int32_t ___typeIndex2, const RuntimeMethod* method)
{
	typedef int32_t (DEFAULT_CALL *PInvokeFunc)(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	int32_t returnValue = il2cppPInvokeFunc(___entityComponentStore0, ___entity1, ___typeIndex2);

	return static_cast<bool>(returnValue);
}
// System.Void Unity.Entities.StructuralChange__dlg_RemoveComponentEntity::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_RemoveComponentEntity__ctor_m3AB6E34E5E0D29D7FD952BBBD5E09A118C09C65C (_dlg_RemoveComponentEntity_t031F2F7279291D7654E794B4B7EB273464D764E6 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Boolean Unity.Entities.StructuralChange__dlg_RemoveComponentEntity::Invoke(Unity.Entities.EntityComponentStore*,Unity.Entities.Entity*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool _dlg_RemoveComponentEntity_Invoke_m82B65469368FDECB9F6106510492DFDCF07F22DF (_dlg_RemoveComponentEntity_t031F2F7279291D7654E794B4B7EB273464D764E6 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___entity1, int32_t ___typeIndex2, const RuntimeMethod* method)
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
			if (___parameterCount == 3)
			{
				// open
				typedef bool (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___entity1, ___typeIndex2, targetMethod);
			}
			else
			{
				// closed
				typedef bool (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___entity1, ___typeIndex2, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef bool (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___entity1, ___typeIndex2, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker3< bool, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___entity1, ___typeIndex2);
					else
						result = GenericVirtFuncInvoker3< bool, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___entity1, ___typeIndex2);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker3< bool, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___entityComponentStore0, ___entity1, ___typeIndex2);
					else
						result = VirtFuncInvoker3< bool, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___entityComponentStore0, ___entity1, ___typeIndex2);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef bool (*FunctionPointerType) (RuntimeObject*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___entityComponentStore0) - 1), ___entity1, ___typeIndex2, targetMethod);
				}
				else
				{
					typedef bool (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 *, int32_t, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___entity1, ___typeIndex2, targetMethod);
				}
			}
		}
	}
	return result;
}
// System.IAsyncResult Unity.Entities.StructuralChange__dlg_RemoveComponentEntity::BeginInvoke(Unity.Entities.EntityComponentStore*,Unity.Entities.Entity*,System.Int32,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* _dlg_RemoveComponentEntity_BeginInvoke_mEB560C483E03EB71F4961EB807CA6CB841EF08BD (_dlg_RemoveComponentEntity_t031F2F7279291D7654E794B4B7EB273464D764E6 * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___entity1, int32_t ___typeIndex2, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback3, RuntimeObject * ___object4, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_dlg_RemoveComponentEntity_BeginInvoke_mEB560C483E03EB71F4961EB807CA6CB841EF08BD_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[4] = {0};
	__d_args[0] = ___entityComponentStore0;
	__d_args[1] = ___entity1;
	__d_args[2] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___typeIndex2);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback3, (RuntimeObject*)___object4);
}
// System.Boolean Unity.Entities.StructuralChange__dlg_RemoveComponentEntity::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool _dlg_RemoveComponentEntity_EndInvoke_m9DDCD09158D3BCDEEB3B8E07ECA6A223F33CA1FF (_dlg_RemoveComponentEntity_t031F2F7279291D7654E794B4B7EB273464D764E6 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper__dlg_SetChunkComponent_t58428EAC3CAA8BEF48D7DEB8E71516FB3619524E (_dlg_SetChunkComponent_t58428EAC3CAA8BEF48D7DEB8E71516FB3619524E * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D * ___chunks1, int32_t ___chunkCount2, void* ___componentData3, int32_t ___componentTypeIndex4, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, void*, int32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentData3, ___componentTypeIndex4);

}
// System.Void Unity.Entities.StructuralChange__dlg_SetChunkComponent::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_SetChunkComponent__ctor_mC5E7BE52AB021D362123D0DACED4AF847A73FB4F (_dlg_SetChunkComponent_t58428EAC3CAA8BEF48D7DEB8E71516FB3619524E * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void Unity.Entities.StructuralChange__dlg_SetChunkComponent::Invoke(Unity.Entities.EntityComponentStore*,Unity.Entities.ArchetypeChunk*,System.Int32,System.Void*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_SetChunkComponent_Invoke_m8000718090401FE5A02D274F6F08C6A0B31B5D44 (_dlg_SetChunkComponent_t58428EAC3CAA8BEF48D7DEB8E71516FB3619524E * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D * ___chunks1, int32_t ___chunkCount2, void* ___componentData3, int32_t ___componentTypeIndex4, const RuntimeMethod* method)
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
			if (___parameterCount == 5)
			{
				// open
				typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, void*, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentData3, ___componentTypeIndex4, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, void*, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentData3, ___componentTypeIndex4, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, void*, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentData3, ___componentTypeIndex4, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker5< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, void*, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentData3, ___componentTypeIndex4);
					else
						GenericVirtActionInvoker5< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, void*, int32_t >::Invoke(targetMethod, targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentData3, ___componentTypeIndex4);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker5< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, void*, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentData3, ___componentTypeIndex4);
					else
						VirtActionInvoker5< EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, void*, int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentData3, ___componentTypeIndex4);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, void*, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___entityComponentStore0) - 1), ___chunks1, ___chunkCount2, ___componentData3, ___componentTypeIndex4, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA *, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D *, int32_t, void*, int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___entityComponentStore0, ___chunks1, ___chunkCount2, ___componentData3, ___componentTypeIndex4, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult Unity.Entities.StructuralChange__dlg_SetChunkComponent::BeginInvoke(Unity.Entities.EntityComponentStore*,Unity.Entities.ArchetypeChunk*,System.Int32,System.Void*,System.Int32,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* _dlg_SetChunkComponent_BeginInvoke_m3F143241350F543B1D56C4E99576F34A390E300B (_dlg_SetChunkComponent_t58428EAC3CAA8BEF48D7DEB8E71516FB3619524E * __this, EntityComponentStore_t1DFB6E6F991F5926D3FD53F6151873F9E5AE28EA * ___entityComponentStore0, ArchetypeChunk_t75295C4582284E1ACB977DD61C6A44E3B9BDFF5D * ___chunks1, int32_t ___chunkCount2, void* ___componentData3, int32_t ___componentTypeIndex4, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback5, RuntimeObject * ___object6, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_dlg_SetChunkComponent_BeginInvoke_m3F143241350F543B1D56C4E99576F34A390E300B_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[6] = {0};
	__d_args[0] = ___entityComponentStore0;
	__d_args[1] = ___chunks1;
	__d_args[2] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___chunkCount2);
	__d_args[3] = ___componentData3;
	__d_args[4] = Box(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046_il2cpp_TypeInfo_var, &___componentTypeIndex4);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback5, (RuntimeObject*)___object6);
}
// System.Void Unity.Entities.StructuralChange__dlg_SetChunkComponent::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _dlg_SetChunkComponent_EndInvoke_mF85B4BAF00D2511A9F6C6E481B1ADF71D97A1AAF (_dlg_SetChunkComponent_t58428EAC3CAA8BEF48D7DEB8E71516FB3619524E * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper_PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 (PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 * __this, void* ___jobStruct0, void* ___runtimes1, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___entity2, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(void*, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 );
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(___jobStruct0, ___runtimes1, ___entity2);

}
// System.Void Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider_PerformLambdaDelegate::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PerformLambdaDelegate__ctor_m937C0635EDDDB108699E0518AA9FA488F28FE33A (PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider_PerformLambdaDelegate::Invoke(System.Void*,System.Void*,Unity.Entities.Entity)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PerformLambdaDelegate_Invoke_mD5D06044801A1C877C141C7B868E50A9E338A558 (PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 * __this, void* ___jobStruct0, void* ___runtimes1, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___entity2, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (void*, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___jobStruct0, ___runtimes1, ___entity2, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, void*, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___jobStruct0, ___runtimes1, ___entity2, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (void*, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___jobStruct0, ___runtimes1, ___entity2, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker3< void*, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  >::Invoke(targetMethod, targetThis, ___jobStruct0, ___runtimes1, ___entity2);
					else
						GenericVirtActionInvoker3< void*, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  >::Invoke(targetMethod, targetThis, ___jobStruct0, ___runtimes1, ___entity2);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker3< void*, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___jobStruct0, ___runtimes1, ___entity2);
					else
						VirtActionInvoker3< void*, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___jobStruct0, ___runtimes1, ___entity2);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___jobStruct0) - 1), ___runtimes1, ___entity2, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, void*, void*, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___jobStruct0, ___runtimes1, ___entity2, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider_PerformLambdaDelegate::BeginInvoke(System.Void*,System.Void*,Unity.Entities.Entity,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* PerformLambdaDelegate_BeginInvoke_m77A90CD6A1AF9013DFD3C84EAA6A5A9925406C13 (PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 * __this, void* ___jobStruct0, void* ___runtimes1, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___entity2, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback3, RuntimeObject * ___object4, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PerformLambdaDelegate_BeginInvoke_m77A90CD6A1AF9013DFD3C84EAA6A5A9925406C13_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[4] = {0};
	__d_args[0] = ___jobStruct0;
	__d_args[1] = ___runtimes1;
	__d_args[2] = Box(Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4_il2cpp_TypeInfo_var, &___entity2);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback3, (RuntimeObject*)___object4);
}
// System.Void Unity.Entities.CodeGeneratedJobForEach.StructuralChangeEntityProvider_PerformLambdaDelegate::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PerformLambdaDelegate_EndInvoke_mE5BFF85C3C29D9862DDDD20DCD68743AC9BB83D3 (PerformLambdaDelegate_tDEE43A2B4C408E9B674E59FEEA27794D7199B377 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper_ForwardingFunc_t830DB8E27A008A51CBCBAAB812E1F2A06C2E89F1 (ForwardingFunc_t830DB8E27A008A51CBCBAAB812E1F2A06C2E89F1 * __this, intptr_t ___systemPtr0, intptr_t ___state1, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(intptr_t, intptr_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(___systemPtr0, ___state1);

}
// System.Void Unity.Entities.SystemBaseRegistry_ForwardingFunc::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ForwardingFunc__ctor_mC82830BF1F7AB80F7CB50A6D0FC78DD10425A53B (ForwardingFunc_t830DB8E27A008A51CBCBAAB812E1F2A06C2E89F1 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void Unity.Entities.SystemBaseRegistry_ForwardingFunc::Invoke(System.IntPtr,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ForwardingFunc_Invoke_m2E9F9A9BF4CA4B2CA009220855E1454D27D76FC0 (ForwardingFunc_t830DB8E27A008A51CBCBAAB812E1F2A06C2E89F1 * __this, intptr_t ___systemPtr0, intptr_t ___state1, const RuntimeMethod* method)
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
			if (___parameterCount == 2)
			{
				// open
				typedef void (*FunctionPointerType) (intptr_t, intptr_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___systemPtr0, ___state1, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, intptr_t, intptr_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___systemPtr0, ___state1, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (intptr_t, intptr_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___systemPtr0, ___state1, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker2< intptr_t, intptr_t >::Invoke(targetMethod, targetThis, ___systemPtr0, ___state1);
					else
						GenericVirtActionInvoker2< intptr_t, intptr_t >::Invoke(targetMethod, targetThis, ___systemPtr0, ___state1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker2< intptr_t, intptr_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___systemPtr0, ___state1);
					else
						VirtActionInvoker2< intptr_t, intptr_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___systemPtr0, ___state1);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, intptr_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(&___systemPtr0) - 1), ___state1, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, intptr_t, intptr_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___systemPtr0, ___state1, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult Unity.Entities.SystemBaseRegistry_ForwardingFunc::BeginInvoke(System.IntPtr,System.IntPtr,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* ForwardingFunc_BeginInvoke_m9BE4143ED642DE88540FE07C6D91BF0B9E0F7F7E (ForwardingFunc_t830DB8E27A008A51CBCBAAB812E1F2A06C2E89F1 * __this, intptr_t ___systemPtr0, intptr_t ___state1, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback2, RuntimeObject * ___object3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ForwardingFunc_BeginInvoke_m9BE4143ED642DE88540FE07C6D91BF0B9E0F7F7E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[3] = {0};
	__d_args[0] = Box(IntPtr_t_il2cpp_TypeInfo_var, &___systemPtr0);
	__d_args[1] = Box(IntPtr_t_il2cpp_TypeInfo_var, &___state1);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback2, (RuntimeObject*)___object3);
}
// System.Void Unity.Entities.SystemBaseRegistry_ForwardingFunc::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ForwardingFunc_EndInvoke_mB1237489D2F4D5DDACDA439506E505483B85F9EF (ForwardingFunc_t830DB8E27A008A51CBCBAAB812E1F2A06C2E89F1 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Unity.Entities.TypeHash_<>c::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__cctor_m631D18F5F25646C0A46FC0C558EA2393072F392E (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CU3Ec__cctor_m631D18F5F25646C0A46FC0C558EA2393072F392E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A * L_0 = (U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A *)il2cpp_codegen_object_new(U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A_il2cpp_TypeInfo_var);
		U3CU3Ec__ctor_m53B99CFD5ED8954C89E683DB5B4DE6ADB46CBA7D(L_0, /*hidden argument*/NULL);
		((U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A_StaticFields*)il2cpp_codegen_static_fields_for(U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A_il2cpp_TypeInfo_var))->set_U3CU3E9_0(L_0);
		return;
	}
}
// System.Void Unity.Entities.TypeHash_<>c::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__ctor_m53B99CFD5ED8954C89E683DB5B4DE6ADB46CBA7D (U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Boolean Unity.Entities.TypeHash_<>c::<HashVersionAttribute>b__7_0(System.Reflection.CustomAttributeData)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CU3Ec_U3CHashVersionAttributeU3Eb__7_0_m005F5ABE146B2F1B4FC02CF8B514EBF4C2FDA269 (U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A * __this, CustomAttributeData_t4F8D66DDB6D3F7E8C39AF85752A0CC9679A4CE85 * ___ca0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CU3Ec_U3CHashVersionAttributeU3Eb__7_0_m005F5ABE146B2F1B4FC02CF8B514EBF4C2FDA269_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// var versionAttribute = type.CustomAttributes.FirstOrDefault(ca => ca.Constructor.DeclaringType.Name == "TypeVersionAttribute");
		CustomAttributeData_t4F8D66DDB6D3F7E8C39AF85752A0CC9679A4CE85 * L_0 = ___ca0;
		NullCheck(L_0);
		ConstructorInfo_t449AEC508DCA508EE46784C4F2716545488ACD5B * L_1 = VirtFuncInvoker0< ConstructorInfo_t449AEC508DCA508EE46784C4F2716545488ACD5B * >::Invoke(4 /* System.Reflection.ConstructorInfo System.Reflection.CustomAttributeData::get_Constructor() */, L_0);
		NullCheck(L_1);
		Type_t * L_2 = VirtFuncInvoker0< Type_t * >::Invoke(8 /* System.Type System.Reflection.MemberInfo::get_DeclaringType() */, L_1);
		NullCheck(L_2);
		String_t* L_3 = VirtFuncInvoker0< String_t* >::Invoke(7 /* System.String System.Reflection.MemberInfo::get_Name() */, L_2);
		bool L_4 = String_op_Equality_m2B91EE68355F142F67095973D32EB5828B7B73CB(L_3, _stringLiteralE6E963A8B9868C07D45F9CC0146A363316992337, /*hidden argument*/NULL);
		return L_4;
	}
}
// System.Boolean Unity.Entities.TypeHash_<>c::<HashVersionAttribute>b__7_1(System.Reflection.CustomAttributeTypedArgument)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CU3Ec_U3CHashVersionAttributeU3Eb__7_1_mDA18EA52F03F78817E0ADB3854E9D9D846A9D3DE (U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A * __this, CustomAttributeTypedArgument_tE7152E8FACDD29A8E0040E151C86F436FA8E6910  ___arg0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CU3Ec_U3CHashVersionAttributeU3Eb__7_1_mDA18EA52F03F78817E0ADB3854E9D9D846A9D3DE_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// .First(arg => arg.ArgumentType.Name == "Int32")
		Type_t * L_0 = CustomAttributeTypedArgument_get_ArgumentType_m87769FA596B93DC490F158996486CA1D42C4E84C_inline((CustomAttributeTypedArgument_tE7152E8FACDD29A8E0040E151C86F436FA8E6910 *)(&___arg0), /*hidden argument*/NULL);
		NullCheck(L_0);
		String_t* L_1 = VirtFuncInvoker0< String_t* >::Invoke(7 /* System.String System.Reflection.MemberInfo::get_Name() */, L_0);
		bool L_2 = String_op_Equality_m2B91EE68355F142F67095973D32EB5828B7B73CB(L_1, _stringLiteralDB47297909F3BD6EDB8AD67A8511975233214355, /*hidden argument*/NULL);
		return L_2;
	}
}
// System.Boolean Unity.Entities.TypeHash_<>c::<CalculateMemoryOrdering>b__11_0(System.Reflection.CustomAttributeData)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CU3Ec_U3CCalculateMemoryOrderingU3Eb__11_0_m0658D174D3687AFE70C7CE87804DB2DEFE0895B3 (U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A * __this, CustomAttributeData_t4F8D66DDB6D3F7E8C39AF85752A0CC9679A4CE85 * ___ca0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CU3Ec_U3CCalculateMemoryOrderingU3Eb__11_0_m0658D174D3687AFE70C7CE87804DB2DEFE0895B3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// var forcedMemoryOrderAttribute = type.CustomAttributes.FirstOrDefault(ca => ca.Constructor.DeclaringType.Name == "ForcedMemoryOrderingAttribute");
		CustomAttributeData_t4F8D66DDB6D3F7E8C39AF85752A0CC9679A4CE85 * L_0 = ___ca0;
		NullCheck(L_0);
		ConstructorInfo_t449AEC508DCA508EE46784C4F2716545488ACD5B * L_1 = VirtFuncInvoker0< ConstructorInfo_t449AEC508DCA508EE46784C4F2716545488ACD5B * >::Invoke(4 /* System.Reflection.ConstructorInfo System.Reflection.CustomAttributeData::get_Constructor() */, L_0);
		NullCheck(L_1);
		Type_t * L_2 = VirtFuncInvoker0< Type_t * >::Invoke(8 /* System.Type System.Reflection.MemberInfo::get_DeclaringType() */, L_1);
		NullCheck(L_2);
		String_t* L_3 = VirtFuncInvoker0< String_t* >::Invoke(7 /* System.String System.Reflection.MemberInfo::get_Name() */, L_2);
		bool L_4 = String_op_Equality_m2B91EE68355F142F67095973D32EB5828B7B73CB(L_3, _stringLiteral3BD5E620A47C36E674F4EB9456A8E5DECDEE7FF7, /*hidden argument*/NULL);
		return L_4;
	}
}
// System.Boolean Unity.Entities.TypeHash_<>c::<CalculateMemoryOrdering>b__11_1(System.Reflection.CustomAttributeTypedArgument)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CU3Ec_U3CCalculateMemoryOrderingU3Eb__11_1_m06A0D3D7E3AB041CB2773B2309CC4118FCAC529B (U3CU3Ec_t2774BC59D8978F3603A95CBD6CAA4A638A164A8A * __this, CustomAttributeTypedArgument_tE7152E8FACDD29A8E0040E151C86F436FA8E6910  ___arg0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CU3Ec_U3CCalculateMemoryOrderingU3Eb__11_1_m06A0D3D7E3AB041CB2773B2309CC4118FCAC529B_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// .First(arg => arg.ArgumentType.Name == "UInt64" || arg.ArgumentType.Name == "ulong")
		Type_t * L_0 = CustomAttributeTypedArgument_get_ArgumentType_m87769FA596B93DC490F158996486CA1D42C4E84C_inline((CustomAttributeTypedArgument_tE7152E8FACDD29A8E0040E151C86F436FA8E6910 *)(&___arg0), /*hidden argument*/NULL);
		NullCheck(L_0);
		String_t* L_1 = VirtFuncInvoker0< String_t* >::Invoke(7 /* System.String System.Reflection.MemberInfo::get_Name() */, L_0);
		bool L_2 = String_op_Equality_m2B91EE68355F142F67095973D32EB5828B7B73CB(L_1, _stringLiteralBF45CADC16AD267EA891B4231D162B68FDED748D, /*hidden argument*/NULL);
		if (L_2)
		{
			goto IL_002f;
		}
	}
	{
		Type_t * L_3 = CustomAttributeTypedArgument_get_ArgumentType_m87769FA596B93DC490F158996486CA1D42C4E84C_inline((CustomAttributeTypedArgument_tE7152E8FACDD29A8E0040E151C86F436FA8E6910 *)(&___arg0), /*hidden argument*/NULL);
		NullCheck(L_3);
		String_t* L_4 = VirtFuncInvoker0< String_t* >::Invoke(7 /* System.String System.Reflection.MemberInfo::get_Name() */, L_3);
		bool L_5 = String_op_Equality_m2B91EE68355F142F67095973D32EB5828B7B73CB(L_4, _stringLiteralE3AB954C27345B5777E41817C31696D6AC0E87C1, /*hidden argument*/NULL);
		return L_5;
	}

IL_002f:
	{
		return (bool)1;
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
// System.Void Unity.Entities.TypeManager_<>c::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__cctor_mC79ADB26176127E222A9CBF9F39F858B8B7375E9 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CU3Ec__cctor_mC79ADB26176127E222A9CBF9F39F858B8B7375E9_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2 * L_0 = (U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2 *)il2cpp_codegen_object_new(U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2_il2cpp_TypeInfo_var);
		U3CU3Ec__ctor_m36364331876FE744E6AE044372D6F056AB30B79C(L_0, /*hidden argument*/NULL);
		((U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2_StaticFields*)il2cpp_codegen_static_fields_for(U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2_il2cpp_TypeInfo_var))->set_U3CU3E9_0(L_0);
		return;
	}
}
// System.Void Unity.Entities.TypeManager_<>c::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__ctor_m36364331876FE744E6AE044372D6F056AB30B79C (U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2 * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Void Unity.Entities.TypeManager_<>c::<Initialize>b__80_0(System.Object,System.EventArgs)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec_U3CInitializeU3Eb__80_0_m11CF863FFFFD5B08197FAB68478BD120D93CD06E (U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2 * __this, RuntimeObject * ____0, EventArgs_tBCAACA538A5195B6D6C8DFCC3524A2A4A67FD8BA * _____1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CU3Ec_U3CInitializeU3Eb__80_0_m11CF863FFFFD5B08197FAB68478BD120D93CD06E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (s_Initialized)
		IL2CPP_RUNTIME_CLASS_INIT(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_il2cpp_TypeInfo_var);
		bool L_0 = ((TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_StaticFields*)il2cpp_codegen_static_fields_for(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_il2cpp_TypeInfo_var))->get_s_Initialized_5();
		if (!L_0)
		{
			goto IL_000c;
		}
	}
	{
		// Shutdown();
		IL2CPP_RUNTIME_CLASS_INIT(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_il2cpp_TypeInfo_var);
		TypeManager_Shutdown_m1C46D942520BD3565AFD36961B215187F6472471(/*hidden argument*/NULL);
	}

IL_000c:
	{
		// };
		return;
	}
}
// System.Boolean Unity.Entities.TypeManager_<>c::<IsZeroSizeStruct>b__113_0(System.Reflection.FieldInfo)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CU3Ec_U3CIsZeroSizeStructU3Eb__113_0_mE15E768980DA3CB9C546597A26A608EEA207D844 (U3CU3Ec_tD636312F0A8EE6A3C1A034E03E104AB7B2BE19C2 * __this, FieldInfo_t * ___fi0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CU3Ec_U3CIsZeroSizeStructU3Eb__113_0_mE15E768980DA3CB9C546597A26A608EEA207D844_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// t.GetFields((BindingFlags)0x34).All(fi => IsZeroSizeStruct(fi.FieldType));
		FieldInfo_t * L_0 = ___fi0;
		NullCheck(L_0);
		Type_t * L_1 = VirtFuncInvoker0< Type_t * >::Invoke(19 /* System.Type System.Reflection.FieldInfo::get_FieldType() */, L_0);
		IL2CPP_RUNTIME_CLASS_INIT(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_il2cpp_TypeInfo_var);
		bool L_2 = TypeManager_IsZeroSizeStruct_m2D8778E94C86EA3AB136D51FD9C5587B685DE077(L_1, /*hidden argument*/NULL);
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
// System.Void Unity.Entities.TypeManager_SharedBlobAssetRefOffset::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SharedBlobAssetRefOffset__cctor_m1865D544722C86A5F9ACD4D021162D19F44E1944 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SharedBlobAssetRefOffset__cctor_m1865D544722C86A5F9ACD4D021162D19F44E1944_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public static readonly SharedStatic<IntPtr> Ref = SharedStatic<IntPtr>.GetOrCreate<TypeManagerKeyContext, SharedBlobAssetRefOffset>();
		SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  L_0 = SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedBlobAssetRefOffset_tE4EC29E58257EEC230D11A35AB22B47D9D9DC0E6_m09EB536037556C2CC390754DB83DA6B73AD52912(0, /*hidden argument*/SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedBlobAssetRefOffset_tE4EC29E58257EEC230D11A35AB22B47D9D9DC0E6_m09EB536037556C2CC390754DB83DA6B73AD52912_RuntimeMethod_var);
		((SharedBlobAssetRefOffset_tE4EC29E58257EEC230D11A35AB22B47D9D9DC0E6_StaticFields*)il2cpp_codegen_static_fields_for(SharedBlobAssetRefOffset_tE4EC29E58257EEC230D11A35AB22B47D9D9DC0E6_il2cpp_TypeInfo_var))->set_Ref_0(L_0);
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
// System.Void Unity.Entities.TypeManager_SharedEntityOffsetInfo::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SharedEntityOffsetInfo__cctor_mA47563747CD46ED81C5996C17DDA196B92504377 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SharedEntityOffsetInfo__cctor_mA47563747CD46ED81C5996C17DDA196B92504377_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public static readonly SharedStatic<IntPtr> Ref = SharedStatic<IntPtr>.GetOrCreate<TypeManagerKeyContext, SharedEntityOffsetInfo>();
		SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  L_0 = SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedEntityOffsetInfo_tE11E3366E1064E671CD7B5C0B457A4A09DCCC068_mEA2E884221A4CEEBE506AB0F64472ABEC48A85DE(0, /*hidden argument*/SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedEntityOffsetInfo_tE11E3366E1064E671CD7B5C0B457A4A09DCCC068_mEA2E884221A4CEEBE506AB0F64472ABEC48A85DE_RuntimeMethod_var);
		((SharedEntityOffsetInfo_tE11E3366E1064E671CD7B5C0B457A4A09DCCC068_StaticFields*)il2cpp_codegen_static_fields_for(SharedEntityOffsetInfo_tE11E3366E1064E671CD7B5C0B457A4A09DCCC068_il2cpp_TypeInfo_var))->set_Ref_0(L_0);
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
// System.Int32& Unity.Entities.TypeManager_SharedTypeIndex::Get(System.Type)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t* SharedTypeIndex_Get_m823250A31F835709E0B87C8C5D29472F8712B37C (Type_t * ___componentType0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SharedTypeIndex_Get_m823250A31F835709E0B87C8C5D29472F8712B37C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	SharedStatic_1_t6DAF6BFFE7CB6295851C9E4567A9874B214BD088  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// return ref SharedStatic<int>.GetOrCreate(typeof(TypeManagerKeyContext), componentType).Data;
		RuntimeTypeHandle_tC33965ADA3E041E0C94AF05E5CB527B56482CEF9  L_0 = { reinterpret_cast<intptr_t> (TypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_1 = Type_GetTypeFromHandle_m8BB57524FF7F9DB1803BC561D2B3A4DBACEB385E(L_0, /*hidden argument*/NULL);
		Type_t * L_2 = ___componentType0;
		SharedStatic_1_t6DAF6BFFE7CB6295851C9E4567A9874B214BD088  L_3 = SharedStatic_1_GetOrCreate_m379EE72FB569F3A26DCF93108ABBF8F81A1CE4E5(L_1, L_2, 0, /*hidden argument*/SharedStatic_1_GetOrCreate_m379EE72FB569F3A26DCF93108ABBF8F81A1CE4E5_RuntimeMethod_var);
		V_0 = L_3;
		int32_t* L_4 = SharedStatic_1_get_Data_m6B89546E4ADA23EA53D35BF24C7B9DF1194CFE25((SharedStatic_1_t6DAF6BFFE7CB6295851C9E4567A9874B214BD088 *)(&V_0), /*hidden argument*/SharedStatic_1_get_Data_m6B89546E4ADA23EA53D35BF24C7B9DF1194CFE25_RuntimeMethod_var);
		return (int32_t*)(L_4);
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
// System.Void Unity.Entities.TypeManager_SharedTypeInfo::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SharedTypeInfo__cctor_m7BAB99895484D0E3D3E5C2455EC9A59095F050A0 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SharedTypeInfo__cctor_m7BAB99895484D0E3D3E5C2455EC9A59095F050A0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public static readonly SharedStatic<IntPtr> Ref = SharedStatic<IntPtr>.GetOrCreate<TypeManagerKeyContext, SharedTypeInfo>();
		SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  L_0 = SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedTypeInfo_t6D9232712A2DC192E8DCC16F0AE24EE2508CCD05_m4D2E48203248786C9D3BFF7BC2205CE779FE7814(0, /*hidden argument*/SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedTypeInfo_t6D9232712A2DC192E8DCC16F0AE24EE2508CCD05_m4D2E48203248786C9D3BFF7BC2205CE779FE7814_RuntimeMethod_var);
		((SharedTypeInfo_t6D9232712A2DC192E8DCC16F0AE24EE2508CCD05_StaticFields*)il2cpp_codegen_static_fields_for(SharedTypeInfo_t6D9232712A2DC192E8DCC16F0AE24EE2508CCD05_il2cpp_TypeInfo_var))->set_Ref_0(L_0);
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
// System.Void Unity.Entities.TypeManager_SharedWriteGroup::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SharedWriteGroup__cctor_m21C0D1BC37FB6ED00482B41D1D1EADD0BCF8ABCB (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SharedWriteGroup__cctor_m21C0D1BC37FB6ED00482B41D1D1EADD0BCF8ABCB_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public static readonly SharedStatic<IntPtr> Ref = SharedStatic<IntPtr>.GetOrCreate<TypeManagerKeyContext, SharedWriteGroup>();
		SharedStatic_1_t863CDF56C5388EE162A39C9F8459F60F245008F6  L_0 = SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedWriteGroup_t43861216B5BB89853E40798D5170B5574E8A46FB_mFA700697C79F3EDB2BC6703FE1267C962261E842(0, /*hidden argument*/SharedStatic_1_GetOrCreate_TisTypeManagerKeyContext_tF2D4BD273527A8E24CF849EE0E287A63916DA218_TisSharedWriteGroup_t43861216B5BB89853E40798D5170B5574E8A46FB_mFA700697C79F3EDB2BC6703FE1267C962261E842_RuntimeMethod_var);
		((SharedWriteGroup_t43861216B5BB89853E40798D5170B5574E8A46FB_StaticFields*)il2cpp_codegen_static_fields_for(SharedWriteGroup_t43861216B5BB89853E40798D5170B5574E8A46FB_il2cpp_TypeInfo_var))->set_Ref_0(L_0);
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
// System.Void Unity.Entities.TypeManager_TypeInfo::.ctor(System.Int32,Unity.Entities.TypeManager_TypeCategory,System.Int32,System.Int32,System.UInt64,System.UInt64,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TypeInfo__ctor_m0DFE1CADCF1CA7D60549849B8F7CE8BE574AC099 (TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * __this, int32_t ___typeIndex0, int32_t ___category1, int32_t ___entityOffsetCount2, int32_t ___entityOffsetStartIndex3, uint64_t ___memoryOrdering4, uint64_t ___stableTypeHash5, int32_t ___bufferCapacity6, int32_t ___sizeInChunk7, int32_t ___elementSize8, int32_t ___alignmentInBytes9, int32_t ___maximumChunkCapacity10, int32_t ___writeGroupCount11, int32_t ___writeGroupStartIndex12, int32_t ___blobAssetRefOffsetCount13, int32_t ___blobAssetRefOffsetStartIndex14, int32_t ___fastEqualityIndex15, int32_t ___typeSize16, const RuntimeMethod* method)
{
	{
		// TypeIndex = typeIndex;
		int32_t L_0 = ___typeIndex0;
		__this->set_TypeIndex_0(L_0);
		// Category = category;
		int32_t L_1 = ___category1;
		__this->set_Category_5(L_1);
		// EntityOffsetCount = entityOffsetCount;
		int32_t L_2 = ___entityOffsetCount2;
		__this->set_EntityOffsetCount_8(L_2);
		// EntityOffsetStartIndex = entityOffsetStartIndex;
		int32_t L_3 = ___entityOffsetStartIndex3;
		__this->set_EntityOffsetStartIndex_9(L_3);
		// MemoryOrdering = memoryOrdering;
		uint64_t L_4 = ___memoryOrdering4;
		__this->set_MemoryOrdering_6(L_4);
		// StableTypeHash = stableTypeHash;
		uint64_t L_5 = ___stableTypeHash5;
		__this->set_StableTypeHash_7(L_5);
		// BufferCapacity = bufferCapacity;
		int32_t L_6 = ___bufferCapacity6;
		__this->set_BufferCapacity_4(L_6);
		// SizeInChunk = sizeInChunk;
		int32_t L_7 = ___sizeInChunk7;
		__this->set_SizeInChunk_1(L_7);
		// ElementSize = elementSize;
		int32_t L_8 = ___elementSize8;
		__this->set_ElementSize_3(L_8);
		// AlignmentInBytes = alignmentInBytes;
		int32_t L_9 = ___alignmentInBytes9;
		__this->set_AlignmentInBytes_2(L_9);
		// MaximumChunkCapacity = maximumChunkCapacity;
		int32_t L_10 = ___maximumChunkCapacity10;
		__this->set_MaximumChunkCapacity_14(L_10);
		// WriteGroupCount = writeGroupCount;
		int32_t L_11 = ___writeGroupCount11;
		__this->set_WriteGroupCount_12(L_11);
		// WriteGroupStartIndex = writeGroupStartIndex;
		int32_t L_12 = ___writeGroupStartIndex12;
		__this->set_WriteGroupStartIndex_13(L_12);
		// BlobAssetRefOffsetCount = blobAssetRefOffsetCount;
		int32_t L_13 = ___blobAssetRefOffsetCount13;
		__this->set_BlobAssetRefOffsetCount_10(L_13);
		// BlobAssetRefOffsetStartIndex = blobAssetRefOffsetStartIndex;
		int32_t L_14 = ___blobAssetRefOffsetStartIndex14;
		__this->set_BlobAssetRefOffsetStartIndex_11(L_14);
		// FastEqualityIndex = fastEqualityIndex; // Only used for Hybrid types (should be removed once we code gen all equality cases)
		int32_t L_15 = ___fastEqualityIndex15;
		__this->set_FastEqualityIndex_15(L_15);
		// TypeSize = typeSize;
		int32_t L_16 = ___typeSize16;
		__this->set_TypeSize_16(L_16);
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void TypeInfo__ctor_m0DFE1CADCF1CA7D60549849B8F7CE8BE574AC099_AdjustorThunk (RuntimeObject * __this, int32_t ___typeIndex0, int32_t ___category1, int32_t ___entityOffsetCount2, int32_t ___entityOffsetStartIndex3, uint64_t ___memoryOrdering4, uint64_t ___stableTypeHash5, int32_t ___bufferCapacity6, int32_t ___sizeInChunk7, int32_t ___elementSize8, int32_t ___alignmentInBytes9, int32_t ___maximumChunkCapacity10, int32_t ___writeGroupCount11, int32_t ___writeGroupStartIndex12, int32_t ___blobAssetRefOffsetCount13, int32_t ___blobAssetRefOffsetStartIndex14, int32_t ___fastEqualityIndex15, int32_t ___typeSize16, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * _thisAdjusted = reinterpret_cast<TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 *>(__this + _offset);
	TypeInfo__ctor_m0DFE1CADCF1CA7D60549849B8F7CE8BE574AC099(_thisAdjusted, ___typeIndex0, ___category1, ___entityOffsetCount2, ___entityOffsetStartIndex3, ___memoryOrdering4, ___stableTypeHash5, ___bufferCapacity6, ___sizeInChunk7, ___elementSize8, ___alignmentInBytes9, ___maximumChunkCapacity10, ___writeGroupCount11, ___writeGroupStartIndex12, ___blobAssetRefOffsetCount13, ___blobAssetRefOffsetStartIndex14, ___fastEqualityIndex15, ___typeSize16, method);
}
// System.Boolean Unity.Entities.TypeManager_TypeInfo::get_IsZeroSized()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TypeInfo_get_IsZeroSized_m3C6016DECE42210DECE2A92633685A569B2E7F0C (TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * __this, const RuntimeMethod* method)
{
	{
		// public bool IsZeroSized => SizeInChunk == 0;
		int32_t L_0 = __this->get_SizeInChunk_1();
		return (bool)((((int32_t)L_0) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool TypeInfo_get_IsZeroSized_m3C6016DECE42210DECE2A92633685A569B2E7F0C_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * _thisAdjusted = reinterpret_cast<TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 *>(__this + _offset);
	return TypeInfo_get_IsZeroSized_m3C6016DECE42210DECE2A92633685A569B2E7F0C(_thisAdjusted, method);
}
// System.Type Unity.Entities.TypeManager_TypeInfo::get_Type()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Type_t * TypeInfo_get_Type_m966C85EAB51370AE906FC730D10936E331F86A93 (TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TypeInfo_get_Type_m966C85EAB51370AE906FC730D10936E331F86A93_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public Type Type => TypeManager.GetType(TypeIndex);
		int32_t L_0 = __this->get_TypeIndex_0();
		IL2CPP_RUNTIME_CLASS_INIT(TypeManager_t86367CFDF39ACA45C54EEA6FB9DDAA1AD4E9B134_il2cpp_TypeInfo_var);
		Type_t * L_1 = TypeManager_GetType_m36FDA8D6746C13AD965E70DF364C6821E1832E35(L_0, /*hidden argument*/NULL);
		return L_1;
	}
}
IL2CPP_EXTERN_C  Type_t * TypeInfo_get_Type_m966C85EAB51370AE906FC730D10936E331F86A93_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * _thisAdjusted = reinterpret_cast<TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 *>(__this + _offset);
	return TypeInfo_get_Type_m966C85EAB51370AE906FC730D10936E331F86A93(_thisAdjusted, method);
}
// System.Boolean Unity.Entities.TypeManager_TypeInfo::get_HasEntities()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TypeInfo_get_HasEntities_mF0ABFAF0ECC3FDE2CA213C5EE1A730D07E488875 (TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * __this, const RuntimeMethod* method)
{
	{
		// public bool HasEntities => EntityOffsetCount > 0;
		int32_t L_0 = __this->get_EntityOffsetCount_8();
		return (bool)((((int32_t)L_0) > ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool TypeInfo_get_HasEntities_mF0ABFAF0ECC3FDE2CA213C5EE1A730D07E488875_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 * _thisAdjusted = reinterpret_cast<TypeInfo_tA535ECED2C2B6BA2F82DCFEBA0F083FA13EC2F38 *>(__this + _offset);
	return TypeInfo_get_HasEntities_mF0ABFAF0ECC3FDE2CA213C5EE1A730D07E488875(_thisAdjusted, method);
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
// System.String Unity.Properties.Generated.Unity_Entities_Entity_PropertyBag_Index_Property::get_Name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Index_Property_get_Name_m1EE91534EE9827009132CEAA903EF8D4E8F9D3A3 (Index_Property_t835C3FA617A3E01A668DA48CE5835BE4ECB9F808 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Index_Property_get_Name_m1EE91534EE9827009132CEAA903EF8D4E8F9D3A3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		return _stringLiteral1C3B83E7128DFE5344885801249731AA7F849057;
	}
}
// System.Boolean Unity.Properties.Generated.Unity_Entities_Entity_PropertyBag_Index_Property::get_IsReadOnly()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Index_Property_get_IsReadOnly_m54502C2CB9B54975B9714E5A1ADAA6C933E76921 (Index_Property_t835C3FA617A3E01A668DA48CE5835BE4ECB9F808 * __this, const RuntimeMethod* method)
{
	{
		return (bool)0;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_Entity_PropertyBag_Index_Property::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Index_Property__ctor_m0365092A2589D0566ACEA4FB53EDC1FBF118CDBF (Index_Property_t835C3FA617A3E01A668DA48CE5835BE4ECB9F808 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Index_Property__ctor_m0365092A2589D0566ACEA4FB53EDC1FBF118CDBF_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Property_2__ctor_m2B068683806A6A583C633FCEE64198D60266B4E1(__this, /*hidden argument*/Property_2__ctor_m2B068683806A6A583C633FCEE64198D60266B4E1_RuntimeMethod_var);
		return;
	}
}
// System.Int32 Unity.Properties.Generated.Unity_Entities_Entity_PropertyBag_Index_Property::GetValue(Unity.Entities.Entity&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Index_Property_GetValue_m52C47DBF2794CBD7EFE744E68301E00D534550E2 (Index_Property_t835C3FA617A3E01A668DA48CE5835BE4ECB9F808 * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___container0, const RuntimeMethod* method)
{
	{
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * L_0 = ___container0;
		int32_t L_1 = L_0->get_Index_0();
		return L_1;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_Entity_PropertyBag_Index_Property::SetValue(Unity.Entities.Entity&,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Index_Property_SetValue_mAB2CFE8E8545147BF73F9C01B59A79B7BA6FF999 (Index_Property_t835C3FA617A3E01A668DA48CE5835BE4ECB9F808 * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___container0, int32_t ___value1, const RuntimeMethod* method)
{
	{
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * L_0 = ___container0;
		int32_t L_1 = ___value1;
		L_0->set_Index_0(L_1);
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
// System.String Unity.Properties.Generated.Unity_Entities_Entity_PropertyBag_Version_Property::get_Name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Version_Property_get_Name_m32E62DA78321922E4520108641F397222967630E (Version_Property_tA459AC543294951D12631B8CC94B7319B6423CC1 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Version_Property_get_Name_m32E62DA78321922E4520108641F397222967630E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		return _stringLiteralE200AC1425952F4F5CEAAA9C773B6D17B90E47C1;
	}
}
// System.Boolean Unity.Properties.Generated.Unity_Entities_Entity_PropertyBag_Version_Property::get_IsReadOnly()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Version_Property_get_IsReadOnly_mD1F86337E04423B889EB3ADCF2530FA86A8B7778 (Version_Property_tA459AC543294951D12631B8CC94B7319B6423CC1 * __this, const RuntimeMethod* method)
{
	{
		return (bool)0;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_Entity_PropertyBag_Version_Property::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Version_Property__ctor_mD380F341F45811DD41CFC377B17A40EE7FF1CB1F (Version_Property_tA459AC543294951D12631B8CC94B7319B6423CC1 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Version_Property__ctor_mD380F341F45811DD41CFC377B17A40EE7FF1CB1F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Property_2__ctor_m2B068683806A6A583C633FCEE64198D60266B4E1(__this, /*hidden argument*/Property_2__ctor_m2B068683806A6A583C633FCEE64198D60266B4E1_RuntimeMethod_var);
		return;
	}
}
// System.Int32 Unity.Properties.Generated.Unity_Entities_Entity_PropertyBag_Version_Property::GetValue(Unity.Entities.Entity&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Version_Property_GetValue_m9C2BCA0272B03C59BA4B972AD41F0517C4F36996 (Version_Property_tA459AC543294951D12631B8CC94B7319B6423CC1 * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___container0, const RuntimeMethod* method)
{
	{
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * L_0 = ___container0;
		int32_t L_1 = L_0->get_Version_1();
		return L_1;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_Entity_PropertyBag_Version_Property::SetValue(Unity.Entities.Entity&,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Version_Property_SetValue_mFEB9B7A947C8421A2046422DEE4ED82DD83AF309 (Version_Property_tA459AC543294951D12631B8CC94B7319B6423CC1 * __this, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * ___container0, int32_t ___value1, const RuntimeMethod* method)
{
	{
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * L_0 = ___container0;
		int32_t L_1 = ___value1;
		L_0->set_Version_1(L_1);
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
// System.String Unity.Properties.Generated.Unity_Entities_Hash128_PropertyBag_Value_Property::get_Name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Value_Property_get_Name_m91BDFD5706CDECF057C3473B1ED3B2E9D985B1C8 (Value_Property_t9606F88DB5D8B3CB45FE05D0D3CC24AE4CCCA547 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Value_Property_get_Name_m91BDFD5706CDECF057C3473B1ED3B2E9D985B1C8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		return _stringLiteral86FADB31129B6F40C720A97600D69389EA3567E3;
	}
}
// System.Boolean Unity.Properties.Generated.Unity_Entities_Hash128_PropertyBag_Value_Property::get_IsReadOnly()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Value_Property_get_IsReadOnly_m9939B1E19DC0918BFE1FC8483B99DA3D7816FAC0 (Value_Property_t9606F88DB5D8B3CB45FE05D0D3CC24AE4CCCA547 * __this, const RuntimeMethod* method)
{
	{
		return (bool)0;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_Hash128_PropertyBag_Value_Property::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Value_Property__ctor_m63FF8583BDEAD99890E15799B4FEFB3F5CBFB4B1 (Value_Property_t9606F88DB5D8B3CB45FE05D0D3CC24AE4CCCA547 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Value_Property__ctor_m63FF8583BDEAD99890E15799B4FEFB3F5CBFB4B1_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Property_2__ctor_mA10C37A89F64B051C9DA6010078BDDCF63F886B2(__this, /*hidden argument*/Property_2__ctor_mA10C37A89F64B051C9DA6010078BDDCF63F886B2_RuntimeMethod_var);
		return;
	}
}
// Unity.Mathematics.uint4 Unity.Properties.Generated.Unity_Entities_Hash128_PropertyBag_Value_Property::GetValue(Unity.Entities.Hash128&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92  Value_Property_GetValue_m5BD01D16B31081F1FDC28B49240B0E3712C3B84A (Value_Property_t9606F88DB5D8B3CB45FE05D0D3CC24AE4CCCA547 * __this, Hash128_t8214C0670F24DF267392561913434E82117B6131 * ___container0, const RuntimeMethod* method)
{
	{
		Hash128_t8214C0670F24DF267392561913434E82117B6131 * L_0 = ___container0;
		uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92  L_1 = L_0->get_Value_0();
		return L_1;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_Hash128_PropertyBag_Value_Property::SetValue(Unity.Entities.Hash128&,Unity.Mathematics.uint4)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Value_Property_SetValue_mCA14F6797883D69C5BF25EAA34CF150B4DB033DE (Value_Property_t9606F88DB5D8B3CB45FE05D0D3CC24AE4CCCA547 * __this, Hash128_t8214C0670F24DF267392561913434E82117B6131 * ___container0, uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92  ___value1, const RuntimeMethod* method)
{
	{
		Hash128_t8214C0670F24DF267392561913434E82117B6131 * L_0 = ___container0;
		uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92  L_1 = ___value1;
		L_0->set_Value_0(L_1);
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
// System.String Unity.Properties.Generated.Unity_Entities_PostLoadCommandBuffer_PropertyBag_CommandBuffer_Property::get_Name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* CommandBuffer_Property_get_Name_m7A58893B7B27FFDAC8EB01461B1DB64A332F8820 (CommandBuffer_Property_tDCB2D8FF230260A52B1CB3A97E5BC39EEAE1BBFF * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (CommandBuffer_Property_get_Name_m7A58893B7B27FFDAC8EB01461B1DB64A332F8820_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		return _stringLiteralBC29A683F5FE1BCDDC8B571A7EF16BE16E4142B2;
	}
}
// System.Boolean Unity.Properties.Generated.Unity_Entities_PostLoadCommandBuffer_PropertyBag_CommandBuffer_Property::get_IsReadOnly()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool CommandBuffer_Property_get_IsReadOnly_m933FD393F7374337587C8E4F9D01DF2F00FB25C0 (CommandBuffer_Property_tDCB2D8FF230260A52B1CB3A97E5BC39EEAE1BBFF * __this, const RuntimeMethod* method)
{
	{
		return (bool)0;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_PostLoadCommandBuffer_PropertyBag_CommandBuffer_Property::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CommandBuffer_Property__ctor_mD65857B5D94D9A5B1A0894359E790EEFF1B858BE (CommandBuffer_Property_tDCB2D8FF230260A52B1CB3A97E5BC39EEAE1BBFF * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (CommandBuffer_Property__ctor_mD65857B5D94D9A5B1A0894359E790EEFF1B858BE_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Property_2__ctor_mAF509B7CDC2036D7E9EBBF46BD4B439073CDE45C(__this, /*hidden argument*/Property_2__ctor_mAF509B7CDC2036D7E9EBBF46BD4B439073CDE45C_RuntimeMethod_var);
		return;
	}
}
// Unity.Entities.EntityCommandBuffer Unity.Properties.Generated.Unity_Entities_PostLoadCommandBuffer_PropertyBag_CommandBuffer_Property::GetValue(Unity.Entities.PostLoadCommandBuffer&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764  CommandBuffer_Property_GetValue_m66DB339EE0D1ABB47FB81A81F7A4F3CE08F10642 (CommandBuffer_Property_tDCB2D8FF230260A52B1CB3A97E5BC39EEAE1BBFF * __this, PostLoadCommandBuffer_tD45726986D9D5D6BF6AB396D8DD997EB3D7AD634 ** ___container0, const RuntimeMethod* method)
{
	{
		PostLoadCommandBuffer_tD45726986D9D5D6BF6AB396D8DD997EB3D7AD634 ** L_0 = ___container0;
		PostLoadCommandBuffer_tD45726986D9D5D6BF6AB396D8DD997EB3D7AD634 * L_1 = *((PostLoadCommandBuffer_tD45726986D9D5D6BF6AB396D8DD997EB3D7AD634 **)L_0);
		NullCheck(L_1);
		EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764  L_2 = L_1->get_CommandBuffer_0();
		return L_2;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_PostLoadCommandBuffer_PropertyBag_CommandBuffer_Property::SetValue(Unity.Entities.PostLoadCommandBuffer&,Unity.Entities.EntityCommandBuffer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CommandBuffer_Property_SetValue_mF10152FC1F828584030A76812ABBF4208A72B7F5 (CommandBuffer_Property_tDCB2D8FF230260A52B1CB3A97E5BC39EEAE1BBFF * __this, PostLoadCommandBuffer_tD45726986D9D5D6BF6AB396D8DD997EB3D7AD634 ** ___container0, EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764  ___value1, const RuntimeMethod* method)
{
	{
		PostLoadCommandBuffer_tD45726986D9D5D6BF6AB396D8DD997EB3D7AD634 ** L_0 = ___container0;
		PostLoadCommandBuffer_tD45726986D9D5D6BF6AB396D8DD997EB3D7AD634 * L_1 = *((PostLoadCommandBuffer_tD45726986D9D5D6BF6AB396D8DD997EB3D7AD634 **)L_0);
		EntityCommandBuffer_t1392F60C43F18D06C981E0F5BCDC2255DE40A764  L_2 = ___value1;
		NullCheck(L_1);
		L_1->set_CommandBuffer_0(L_2);
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
// System.String Unity.Properties.Generated.Unity_Entities_SceneSection_PropertyBag_SceneGUID_Property::get_Name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* SceneGUID_Property_get_Name_mD2175AEBD47F37A29024711879155CF8D56BDAC6 (SceneGUID_Property_t286F056FA890C76A85CAEED96FB8F2DC18718F24 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SceneGUID_Property_get_Name_mD2175AEBD47F37A29024711879155CF8D56BDAC6_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		return _stringLiteral2B34CB86005E87B20947F06933F7E295DE02BF17;
	}
}
// System.Boolean Unity.Properties.Generated.Unity_Entities_SceneSection_PropertyBag_SceneGUID_Property::get_IsReadOnly()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SceneGUID_Property_get_IsReadOnly_m34874016AA52248AAC4A57AB0272FE286DC442C4 (SceneGUID_Property_t286F056FA890C76A85CAEED96FB8F2DC18718F24 * __this, const RuntimeMethod* method)
{
	{
		return (bool)0;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_SceneSection_PropertyBag_SceneGUID_Property::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SceneGUID_Property__ctor_m5626743899E620550A96D3FFE3DB260E87CCB8C1 (SceneGUID_Property_t286F056FA890C76A85CAEED96FB8F2DC18718F24 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SceneGUID_Property__ctor_m5626743899E620550A96D3FFE3DB260E87CCB8C1_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Property_2__ctor_m7DE529508EC4AD847F196ED89B9D7608D3976EAD(__this, /*hidden argument*/Property_2__ctor_m7DE529508EC4AD847F196ED89B9D7608D3976EAD_RuntimeMethod_var);
		return;
	}
}
// Unity.Entities.Hash128 Unity.Properties.Generated.Unity_Entities_SceneSection_PropertyBag_SceneGUID_Property::GetValue(Unity.Entities.SceneSection&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Hash128_t8214C0670F24DF267392561913434E82117B6131  SceneGUID_Property_GetValue_m39C9D7AAF44AC3D9966361CC70FA23F172DD0828 (SceneGUID_Property_t286F056FA890C76A85CAEED96FB8F2DC18718F24 * __this, SceneSection_t3FEF24F2D1C6132CA8F4B8E9767C170609E30044 * ___container0, const RuntimeMethod* method)
{
	{
		SceneSection_t3FEF24F2D1C6132CA8F4B8E9767C170609E30044 * L_0 = ___container0;
		Hash128_t8214C0670F24DF267392561913434E82117B6131  L_1 = L_0->get_SceneGUID_0();
		return L_1;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_SceneSection_PropertyBag_SceneGUID_Property::SetValue(Unity.Entities.SceneSection&,Unity.Entities.Hash128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SceneGUID_Property_SetValue_m46309FCD600DEC4C3D5A38E618C79F1142F4C471 (SceneGUID_Property_t286F056FA890C76A85CAEED96FB8F2DC18718F24 * __this, SceneSection_t3FEF24F2D1C6132CA8F4B8E9767C170609E30044 * ___container0, Hash128_t8214C0670F24DF267392561913434E82117B6131  ___value1, const RuntimeMethod* method)
{
	{
		SceneSection_t3FEF24F2D1C6132CA8F4B8E9767C170609E30044 * L_0 = ___container0;
		Hash128_t8214C0670F24DF267392561913434E82117B6131  L_1 = ___value1;
		L_0->set_SceneGUID_0(L_1);
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
// System.String Unity.Properties.Generated.Unity_Entities_SceneSection_PropertyBag_Section_Property::get_Name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Section_Property_get_Name_m07068B673845F0CAA7920BCFB234C3B2FF235B68 (Section_Property_t1AB3E9286E418F5B1D6EA8E68E25BFF01B901BDB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Section_Property_get_Name_m07068B673845F0CAA7920BCFB234C3B2FF235B68_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		return _stringLiteralE8F1C389BD6A872E97A9FA999AECFCFB55A3E3A7;
	}
}
// System.Boolean Unity.Properties.Generated.Unity_Entities_SceneSection_PropertyBag_Section_Property::get_IsReadOnly()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Section_Property_get_IsReadOnly_mC8AC4F9940EDE63CFB4325D5D4FAD629FD9FF693 (Section_Property_t1AB3E9286E418F5B1D6EA8E68E25BFF01B901BDB * __this, const RuntimeMethod* method)
{
	{
		return (bool)0;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_SceneSection_PropertyBag_Section_Property::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Section_Property__ctor_m765F4D5B13102563E873DF8D9B369DEE0BA86B2E (Section_Property_t1AB3E9286E418F5B1D6EA8E68E25BFF01B901BDB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Section_Property__ctor_m765F4D5B13102563E873DF8D9B369DEE0BA86B2E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Property_2__ctor_mAF911EBD92AA72B392E830E0A324C3D398CF7AB7(__this, /*hidden argument*/Property_2__ctor_mAF911EBD92AA72B392E830E0A324C3D398CF7AB7_RuntimeMethod_var);
		return;
	}
}
// System.Int32 Unity.Properties.Generated.Unity_Entities_SceneSection_PropertyBag_Section_Property::GetValue(Unity.Entities.SceneSection&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Section_Property_GetValue_mFBB7640A29385C5CF9787F2D3D670684503D9E1C (Section_Property_t1AB3E9286E418F5B1D6EA8E68E25BFF01B901BDB * __this, SceneSection_t3FEF24F2D1C6132CA8F4B8E9767C170609E30044 * ___container0, const RuntimeMethod* method)
{
	{
		SceneSection_t3FEF24F2D1C6132CA8F4B8E9767C170609E30044 * L_0 = ___container0;
		int32_t L_1 = L_0->get_Section_1();
		return L_1;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_SceneSection_PropertyBag_Section_Property::SetValue(Unity.Entities.SceneSection&,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Section_Property_SetValue_m9013E5AC69450EF6C6665AC437426D5F0DF85892 (Section_Property_t1AB3E9286E418F5B1D6EA8E68E25BFF01B901BDB * __this, SceneSection_t3FEF24F2D1C6132CA8F4B8E9767C170609E30044 * ___container0, int32_t ___value1, const RuntimeMethod* method)
{
	{
		SceneSection_t3FEF24F2D1C6132CA8F4B8E9767C170609E30044 * L_0 = ___container0;
		int32_t L_1 = ___value1;
		L_0->set_Section_1(L_1);
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
// System.String Unity.Properties.Generated.Unity_Entities_SceneTag_PropertyBag_SceneEntity_Property::get_Name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* SceneEntity_Property_get_Name_mFE241352612BB2281F37EF4A63E14A77ABEAACDF (SceneEntity_Property_tDA7084AEB4F0BEF5863A78E97A21028595F2E2E0 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SceneEntity_Property_get_Name_mFE241352612BB2281F37EF4A63E14A77ABEAACDF_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		return _stringLiteral3EBA566FB73F37AAE39B99437ACB61532CD06333;
	}
}
// System.Boolean Unity.Properties.Generated.Unity_Entities_SceneTag_PropertyBag_SceneEntity_Property::get_IsReadOnly()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SceneEntity_Property_get_IsReadOnly_m039D4FD1FF90B7166081E4BB8E16FCE4DDD5C5CB (SceneEntity_Property_tDA7084AEB4F0BEF5863A78E97A21028595F2E2E0 * __this, const RuntimeMethod* method)
{
	{
		return (bool)0;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_SceneTag_PropertyBag_SceneEntity_Property::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SceneEntity_Property__ctor_m4F1C45701726F5AA5BA7AA50EE3BE4E050DD5922 (SceneEntity_Property_tDA7084AEB4F0BEF5863A78E97A21028595F2E2E0 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SceneEntity_Property__ctor_m4F1C45701726F5AA5BA7AA50EE3BE4E050DD5922_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Property_2__ctor_mED553372B69CC9749E50E387DE73DD0DDAC6A108(__this, /*hidden argument*/Property_2__ctor_mED553372B69CC9749E50E387DE73DD0DDAC6A108_RuntimeMethod_var);
		return;
	}
}
// Unity.Entities.Entity Unity.Properties.Generated.Unity_Entities_SceneTag_PropertyBag_SceneEntity_Property::GetValue(Unity.Entities.SceneTag&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  SceneEntity_Property_GetValue_m7F3F15815FE754FD54380BDA68558EFA9C3DE098 (SceneEntity_Property_tDA7084AEB4F0BEF5863A78E97A21028595F2E2E0 * __this, SceneTag_t080CBEF3258F69B515ED15B950061C372200F206 * ___container0, const RuntimeMethod* method)
{
	{
		SceneTag_t080CBEF3258F69B515ED15B950061C372200F206 * L_0 = ___container0;
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_1 = L_0->get_SceneEntity_0();
		return L_1;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_SceneTag_PropertyBag_SceneEntity_Property::SetValue(Unity.Entities.SceneTag&,Unity.Entities.Entity)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SceneEntity_Property_SetValue_m2260C072D0120622FD39CE1E1FA3D940BB1D9C94 (SceneEntity_Property_tDA7084AEB4F0BEF5863A78E97A21028595F2E2E0 * __this, SceneTag_t080CBEF3258F69B515ED15B950061C372200F206 * ___container0, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___value1, const RuntimeMethod* method)
{
	{
		SceneTag_t080CBEF3258F69B515ED15B950061C372200F206 * L_0 = ___container0;
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_1 = ___value1;
		L_0->set_SceneEntity_0(L_1);
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
// System.String Unity.Properties.Generated.Unity_Entities_SectionMetadataSetup_PropertyBag_SceneSectionIndex_Property::get_Name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* SceneSectionIndex_Property_get_Name_m1C00A80D33667C071D4A4F60CFC614EDFD683C24 (SceneSectionIndex_Property_t123ADC180D58E59803781FA9CA0C27F3F4AD9FA6 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SceneSectionIndex_Property_get_Name_m1C00A80D33667C071D4A4F60CFC614EDFD683C24_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		return _stringLiteralA1A661195E7ABA07DF5C7981C6DCA906B7120C55;
	}
}
// System.Boolean Unity.Properties.Generated.Unity_Entities_SectionMetadataSetup_PropertyBag_SceneSectionIndex_Property::get_IsReadOnly()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SceneSectionIndex_Property_get_IsReadOnly_mEC78A98FA52B926890104F5E4A1FCAD3A689B981 (SceneSectionIndex_Property_t123ADC180D58E59803781FA9CA0C27F3F4AD9FA6 * __this, const RuntimeMethod* method)
{
	{
		return (bool)0;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_SectionMetadataSetup_PropertyBag_SceneSectionIndex_Property::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SceneSectionIndex_Property__ctor_m4D79DC51D6459002B01F1A444DFE53C2DC5B395C (SceneSectionIndex_Property_t123ADC180D58E59803781FA9CA0C27F3F4AD9FA6 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SceneSectionIndex_Property__ctor_m4D79DC51D6459002B01F1A444DFE53C2DC5B395C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Property_2__ctor_m0B519A8910790B943F6709C19F8F47DA8C45B78F(__this, /*hidden argument*/Property_2__ctor_m0B519A8910790B943F6709C19F8F47DA8C45B78F_RuntimeMethod_var);
		return;
	}
}
// System.Int32 Unity.Properties.Generated.Unity_Entities_SectionMetadataSetup_PropertyBag_SceneSectionIndex_Property::GetValue(Unity.Entities.SectionMetadataSetup&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SceneSectionIndex_Property_GetValue_mC11B8F612F6380EED39E09D9E9ED8D8BFB4B6FD4 (SceneSectionIndex_Property_t123ADC180D58E59803781FA9CA0C27F3F4AD9FA6 * __this, SectionMetadataSetup_t572811581DABF646857E271928C5F8412802BDCE * ___container0, const RuntimeMethod* method)
{
	{
		SectionMetadataSetup_t572811581DABF646857E271928C5F8412802BDCE * L_0 = ___container0;
		int32_t L_1 = L_0->get_SceneSectionIndex_0();
		return L_1;
	}
}
// System.Void Unity.Properties.Generated.Unity_Entities_SectionMetadataSetup_PropertyBag_SceneSectionIndex_Property::SetValue(Unity.Entities.SectionMetadataSetup&,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SceneSectionIndex_Property_SetValue_mEEAEB352B6DE831F169AC4B8562249FC1D5B0E7F (SceneSectionIndex_Property_t123ADC180D58E59803781FA9CA0C27F3F4AD9FA6 * __this, SectionMetadataSetup_t572811581DABF646857E271928C5F8412802BDCE * ___container0, int32_t ___value1, const RuntimeMethod* method)
{
	{
		SectionMetadataSetup_t572811581DABF646857E271928C5F8412802BDCE * L_0 = ___container0;
		int32_t L_1 = ___value1;
		L_0->set_SceneSectionIndex_0(L_1);
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
// System.String Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_w_Property::get_Name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* w_Property_get_Name_m8FF791A2369404ACB515FB598BBAACF466C94271 (w_Property_tE1DFF192A694126418686CC4C65AD495A7670315 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (w_Property_get_Name_m8FF791A2369404ACB515FB598BBAACF466C94271_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		return _stringLiteralA9FEAF5F50923952C1AC3A473DE3C7E17D23B907;
	}
}
// System.Boolean Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_w_Property::get_IsReadOnly()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool w_Property_get_IsReadOnly_mEE45B0E8CA367DCAA909CB62439C27D755B36467 (w_Property_tE1DFF192A694126418686CC4C65AD495A7670315 * __this, const RuntimeMethod* method)
{
	{
		return (bool)0;
	}
}
// System.Void Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_w_Property::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void w_Property__ctor_m3DBEC411D2077B7F0039591E04FDACDDE899DD55 (w_Property_tE1DFF192A694126418686CC4C65AD495A7670315 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (w_Property__ctor_m3DBEC411D2077B7F0039591E04FDACDDE899DD55_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Property_2__ctor_m715A2E230FCB05B99C9E2E5ECF6C9660116D19E7(__this, /*hidden argument*/Property_2__ctor_m715A2E230FCB05B99C9E2E5ECF6C9660116D19E7_RuntimeMethod_var);
		return;
	}
}
// System.UInt32 Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_w_Property::GetValue(Unity.Mathematics.uint4&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t w_Property_GetValue_m2EC157C92575C36A7C1FAB2F6B416485A5E8E502 (w_Property_tE1DFF192A694126418686CC4C65AD495A7670315 * __this, uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * ___container0, const RuntimeMethod* method)
{
	{
		uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * L_0 = ___container0;
		uint32_t L_1 = L_0->get_w_3();
		return L_1;
	}
}
// System.Void Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_w_Property::SetValue(Unity.Mathematics.uint4&,System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void w_Property_SetValue_mB34C22E0648A041016E2F546373FA8A121993EBB (w_Property_tE1DFF192A694126418686CC4C65AD495A7670315 * __this, uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * ___container0, uint32_t ___value1, const RuntimeMethod* method)
{
	{
		uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * L_0 = ___container0;
		uint32_t L_1 = ___value1;
		L_0->set_w_3(L_1);
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
// System.String Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_x_Property::get_Name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* x_Property_get_Name_m47585DCC880E4FFF8E58CACE1B2A5C48CAFC4353 (x_Property_tC992C828E7E67B8457C9631A95F7502D91029124 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (x_Property_get_Name_m47585DCC880E4FFF8E58CACE1B2A5C48CAFC4353_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		return _stringLiteral062DB096C728515E033CF8C48A1C1F0B9A79384B;
	}
}
// System.Boolean Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_x_Property::get_IsReadOnly()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool x_Property_get_IsReadOnly_m674EF2E1E58F88996564D528B1EFB0E8E43F492F (x_Property_tC992C828E7E67B8457C9631A95F7502D91029124 * __this, const RuntimeMethod* method)
{
	{
		return (bool)0;
	}
}
// System.Void Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_x_Property::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void x_Property__ctor_m47D3D48E4684A939CEC75D9483B77835B7DD0F2A (x_Property_tC992C828E7E67B8457C9631A95F7502D91029124 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (x_Property__ctor_m47D3D48E4684A939CEC75D9483B77835B7DD0F2A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Property_2__ctor_m715A2E230FCB05B99C9E2E5ECF6C9660116D19E7(__this, /*hidden argument*/Property_2__ctor_m715A2E230FCB05B99C9E2E5ECF6C9660116D19E7_RuntimeMethod_var);
		return;
	}
}
// System.UInt32 Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_x_Property::GetValue(Unity.Mathematics.uint4&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t x_Property_GetValue_m5123EDD40DF7003AE9374E7B1EA2457F724E9EB8 (x_Property_tC992C828E7E67B8457C9631A95F7502D91029124 * __this, uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * ___container0, const RuntimeMethod* method)
{
	{
		uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * L_0 = ___container0;
		uint32_t L_1 = L_0->get_x_0();
		return L_1;
	}
}
// System.Void Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_x_Property::SetValue(Unity.Mathematics.uint4&,System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void x_Property_SetValue_m617436A5CD219BAAFC42854837212D1AD70FE2DD (x_Property_tC992C828E7E67B8457C9631A95F7502D91029124 * __this, uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * ___container0, uint32_t ___value1, const RuntimeMethod* method)
{
	{
		uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * L_0 = ___container0;
		uint32_t L_1 = ___value1;
		L_0->set_x_0(L_1);
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
// System.String Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_y_Property::get_Name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* y_Property_get_Name_m4C38239EACA3A992C12CF469F4C43C2C67E81CBA (y_Property_t917A1CEE407CE842A5B28FC2850257DC84226D5D * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (y_Property_get_Name_m4C38239EACA3A992C12CF469F4C43C2C67E81CBA_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		return _stringLiteral9384C6EF2DA5C0BD5274A0DACFF291D0ABBFD8B1;
	}
}
// System.Boolean Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_y_Property::get_IsReadOnly()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool y_Property_get_IsReadOnly_m602C5DE45E38D455AB9B818B01E059597C0CDBB1 (y_Property_t917A1CEE407CE842A5B28FC2850257DC84226D5D * __this, const RuntimeMethod* method)
{
	{
		return (bool)0;
	}
}
// System.Void Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_y_Property::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void y_Property__ctor_m8653823CCFE3F1E47EDD0F08563248004C3289DC (y_Property_t917A1CEE407CE842A5B28FC2850257DC84226D5D * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (y_Property__ctor_m8653823CCFE3F1E47EDD0F08563248004C3289DC_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Property_2__ctor_m715A2E230FCB05B99C9E2E5ECF6C9660116D19E7(__this, /*hidden argument*/Property_2__ctor_m715A2E230FCB05B99C9E2E5ECF6C9660116D19E7_RuntimeMethod_var);
		return;
	}
}
// System.UInt32 Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_y_Property::GetValue(Unity.Mathematics.uint4&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t y_Property_GetValue_mF3A0B411C72A63FAD25D9DAF957AE2812715EA02 (y_Property_t917A1CEE407CE842A5B28FC2850257DC84226D5D * __this, uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * ___container0, const RuntimeMethod* method)
{
	{
		uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * L_0 = ___container0;
		uint32_t L_1 = L_0->get_y_1();
		return L_1;
	}
}
// System.Void Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_y_Property::SetValue(Unity.Mathematics.uint4&,System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void y_Property_SetValue_m0CBD29B406359790651CDAB59FDCCBD6A23C395C (y_Property_t917A1CEE407CE842A5B28FC2850257DC84226D5D * __this, uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * ___container0, uint32_t ___value1, const RuntimeMethod* method)
{
	{
		uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * L_0 = ___container0;
		uint32_t L_1 = ___value1;
		L_0->set_y_1(L_1);
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
// System.String Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_z_Property::get_Name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* z_Property_get_Name_m3ED396DB84C1F4938F600674C1407BB917A83144 (z_Property_tCC6F6AC68C86D8C1E6BCFFE6762B55DA5B817676 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (z_Property_get_Name_m3ED396DB84C1F4938F600674C1407BB917A83144_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		return _stringLiteral9CE1604D659135925CCC4DD1F526AFFE42E689F1;
	}
}
// System.Boolean Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_z_Property::get_IsReadOnly()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool z_Property_get_IsReadOnly_m988F545349AF30E1BBA43B92A5F6E94DF641FDC4 (z_Property_tCC6F6AC68C86D8C1E6BCFFE6762B55DA5B817676 * __this, const RuntimeMethod* method)
{
	{
		return (bool)0;
	}
}
// System.Void Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_z_Property::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void z_Property__ctor_m66D527839149FE89BC3DB1177FAFF3D418A2BE58 (z_Property_tCC6F6AC68C86D8C1E6BCFFE6762B55DA5B817676 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (z_Property__ctor_m66D527839149FE89BC3DB1177FAFF3D418A2BE58_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Property_2__ctor_m715A2E230FCB05B99C9E2E5ECF6C9660116D19E7(__this, /*hidden argument*/Property_2__ctor_m715A2E230FCB05B99C9E2E5ECF6C9660116D19E7_RuntimeMethod_var);
		return;
	}
}
// System.UInt32 Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_z_Property::GetValue(Unity.Mathematics.uint4&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t z_Property_GetValue_mAEF8C5CB0620539B1B5AE4EA178F499DFE392DC7 (z_Property_tCC6F6AC68C86D8C1E6BCFFE6762B55DA5B817676 * __this, uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * ___container0, const RuntimeMethod* method)
{
	{
		uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * L_0 = ___container0;
		uint32_t L_1 = L_0->get_z_2();
		return L_1;
	}
}
// System.Void Unity.Properties.Generated.Unity_Mathematics_uint4_PropertyBag_z_Property::SetValue(Unity.Mathematics.uint4&,System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void z_Property_SetValue_m7BDB3AEEAD92E343B0F1CE30F67A0466D0BA2CDB (z_Property_tCC6F6AC68C86D8C1E6BCFFE6762B55DA5B817676 * __this, uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * ___container0, uint32_t ___value1, const RuntimeMethod* method)
{
	{
		uint4_t6C7A8C67DCDD20CC3282CA4AA5382744FB27EE92 * L_0 = ___container0;
		uint32_t L_1 = ___value1;
		L_0->set_z_2(L_1);
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
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Conversion methods for marshalling of: Unity.Entities.World/StateAllocLevel1
IL2CPP_EXTERN_C void StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82_marshal_pinvoke(const StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82& unmarshaled, StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82_marshaled_pinvoke& marshaled)
{
	marshaled.___FreeBits_0 = unmarshaled.get_FreeBits_0();
	marshaled.___States_1 = unmarshaled.get_States_1();
	marshaled.___Version_2 = unmarshaled.get_Version_2();
	marshaled.___TypeHash_3 = unmarshaled.get_TypeHash_3();
	marshaled.___SystemPointer_4 = unmarshaled.get_SystemPointer_4();
}
IL2CPP_EXTERN_C void StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82_marshal_pinvoke_back(const StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82_marshaled_pinvoke& marshaled, StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82& unmarshaled)
{
	uint64_t unmarshaled_FreeBits_temp_0 = 0;
	unmarshaled_FreeBits_temp_0 = marshaled.___FreeBits_0;
	unmarshaled.set_FreeBits_0(unmarshaled_FreeBits_temp_0);
	unmarshaled.set_States_1(marshaled.___States_1);
	U3CVersionU3Ee__FixedBuffer_tB3AA164CD6E485CFAEFC56242AE5BE61BE39446F  unmarshaled_Version_temp_2;
	memset((&unmarshaled_Version_temp_2), 0, sizeof(unmarshaled_Version_temp_2));
	unmarshaled_Version_temp_2 = marshaled.___Version_2;
	unmarshaled.set_Version_2(unmarshaled_Version_temp_2);
	U3CTypeHashU3Ee__FixedBuffer_tEE844B127F8E28AF008BEC971AE9386972B3D6D5  unmarshaled_TypeHash_temp_3;
	memset((&unmarshaled_TypeHash_temp_3), 0, sizeof(unmarshaled_TypeHash_temp_3));
	unmarshaled_TypeHash_temp_3 = marshaled.___TypeHash_3;
	unmarshaled.set_TypeHash_3(unmarshaled_TypeHash_temp_3);
	U3CSystemPointerU3Ee__FixedBuffer_t19C0D38015C844F379AB29453AE403CBDD5E16B1  unmarshaled_SystemPointer_temp_4;
	memset((&unmarshaled_SystemPointer_temp_4), 0, sizeof(unmarshaled_SystemPointer_temp_4));
	unmarshaled_SystemPointer_temp_4 = marshaled.___SystemPointer_4;
	unmarshaled.set_SystemPointer_4(unmarshaled_SystemPointer_temp_4);
}
// Conversion method for clean up from marshalling of: Unity.Entities.World/StateAllocLevel1
IL2CPP_EXTERN_C void StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82_marshal_pinvoke_cleanup(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: Unity.Entities.World/StateAllocLevel1
IL2CPP_EXTERN_C void StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82_marshal_com(const StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82& unmarshaled, StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82_marshaled_com& marshaled)
{
	marshaled.___FreeBits_0 = unmarshaled.get_FreeBits_0();
	marshaled.___States_1 = unmarshaled.get_States_1();
	marshaled.___Version_2 = unmarshaled.get_Version_2();
	marshaled.___TypeHash_3 = unmarshaled.get_TypeHash_3();
	marshaled.___SystemPointer_4 = unmarshaled.get_SystemPointer_4();
}
IL2CPP_EXTERN_C void StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82_marshal_com_back(const StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82_marshaled_com& marshaled, StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82& unmarshaled)
{
	uint64_t unmarshaled_FreeBits_temp_0 = 0;
	unmarshaled_FreeBits_temp_0 = marshaled.___FreeBits_0;
	unmarshaled.set_FreeBits_0(unmarshaled_FreeBits_temp_0);
	unmarshaled.set_States_1(marshaled.___States_1);
	U3CVersionU3Ee__FixedBuffer_tB3AA164CD6E485CFAEFC56242AE5BE61BE39446F  unmarshaled_Version_temp_2;
	memset((&unmarshaled_Version_temp_2), 0, sizeof(unmarshaled_Version_temp_2));
	unmarshaled_Version_temp_2 = marshaled.___Version_2;
	unmarshaled.set_Version_2(unmarshaled_Version_temp_2);
	U3CTypeHashU3Ee__FixedBuffer_tEE844B127F8E28AF008BEC971AE9386972B3D6D5  unmarshaled_TypeHash_temp_3;
	memset((&unmarshaled_TypeHash_temp_3), 0, sizeof(unmarshaled_TypeHash_temp_3));
	unmarshaled_TypeHash_temp_3 = marshaled.___TypeHash_3;
	unmarshaled.set_TypeHash_3(unmarshaled_TypeHash_temp_3);
	U3CSystemPointerU3Ee__FixedBuffer_t19C0D38015C844F379AB29453AE403CBDD5E16B1  unmarshaled_SystemPointer_temp_4;
	memset((&unmarshaled_SystemPointer_temp_4), 0, sizeof(unmarshaled_SystemPointer_temp_4));
	unmarshaled_SystemPointer_temp_4 = marshaled.___SystemPointer_4;
	unmarshaled.set_SystemPointer_4(unmarshaled_SystemPointer_temp_4);
}
// Conversion method for clean up from marshalling of: Unity.Entities.World/StateAllocLevel1
IL2CPP_EXTERN_C void StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82_marshal_com_cleanup(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82_marshaled_com& marshaled)
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
// Conversion methods for marshalling of: Unity.Entities.World/StateAllocator
IL2CPP_EXTERN_C void StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB_marshal_pinvoke(const StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB& unmarshaled, StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB_marshaled_pinvoke& marshaled)
{
	marshaled.___m_FreeBits_0 = unmarshaled.get_m_FreeBits_0();
	marshaled.___m_Level1_1 = unmarshaled.get_m_Level1_1();
}
IL2CPP_EXTERN_C void StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB_marshal_pinvoke_back(const StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB_marshaled_pinvoke& marshaled, StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB& unmarshaled)
{
	uint64_t unmarshaled_m_FreeBits_temp_0 = 0;
	unmarshaled_m_FreeBits_temp_0 = marshaled.___m_FreeBits_0;
	unmarshaled.set_m_FreeBits_0(unmarshaled_m_FreeBits_temp_0);
	unmarshaled.set_m_Level1_1(marshaled.___m_Level1_1);
}
// Conversion method for clean up from marshalling of: Unity.Entities.World/StateAllocator
IL2CPP_EXTERN_C void StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB_marshal_pinvoke_cleanup(StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: Unity.Entities.World/StateAllocator
IL2CPP_EXTERN_C void StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB_marshal_com(const StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB& unmarshaled, StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB_marshaled_com& marshaled)
{
	marshaled.___m_FreeBits_0 = unmarshaled.get_m_FreeBits_0();
	marshaled.___m_Level1_1 = unmarshaled.get_m_Level1_1();
}
IL2CPP_EXTERN_C void StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB_marshal_com_back(const StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB_marshaled_com& marshaled, StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB& unmarshaled)
{
	uint64_t unmarshaled_m_FreeBits_temp_0 = 0;
	unmarshaled_m_FreeBits_temp_0 = marshaled.___m_FreeBits_0;
	unmarshaled.set_m_FreeBits_0(unmarshaled_m_FreeBits_temp_0);
	unmarshaled.set_m_Level1_1(marshaled.___m_Level1_1);
}
// Conversion method for clean up from marshalling of: Unity.Entities.World/StateAllocator
IL2CPP_EXTERN_C void StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB_marshal_com_cleanup(StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB_marshaled_com& marshaled)
{
}
// System.Void Unity.Entities.World_StateAllocator::Init()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StateAllocator_Init_mB98D03A98BB079DC98429B88AD7F3A518C687F48 (StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * V_1 = NULL;
	StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * V_2 = NULL;
	int32_t V_3 = 0;
	{
		// m_FreeBits = ~0ul;
		__this->set_m_FreeBits_0((((int64_t)((int64_t)(-1)))));
		// int allocSize = sizeof(StateAllocLevel1) * 64;
		uint32_t L_0 = sizeof(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 );
		V_0 = ((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)((int32_t)64)));
		// var l1 = m_Level1 = (StateAllocLevel1*)UnsafeUtility.Malloc(allocSize, 16, Allocator.Persistent);
		int32_t L_1 = V_0;
		void* L_2 = UnsafeUtility_Malloc_m18FCC67A056C48A4E0F939D08C43F9E876CA1CF6((((int64_t)((int64_t)L_1))), ((int32_t)16), 4, /*hidden argument*/NULL);
		void* L_3 = (void*)L_2;
		V_2 = (StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)L_3;
		__this->set_m_Level1_1((StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)L_3);
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_4 = V_2;
		V_1 = (StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)L_4;
		// UnsafeUtility.MemClear(l1, allocSize);
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_5 = V_1;
		int32_t L_6 = V_0;
		UnsafeUtility_MemClear_m9A2B75C85CB8B6637B1286A562A8E35C82772D09((void*)(void*)L_5, (((int64_t)((int64_t)L_6))), /*hidden argument*/NULL);
		// for (int i = 0; i < 64; ++i)
		V_3 = 0;
		goto IL_0048;
	}

IL_0032:
	{
		// l1[i].FreeBits = ~0ul;
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_7 = V_1;
		int32_t L_8 = V_3;
		uint32_t L_9 = sizeof(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 );
		NullCheck(((StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)il2cpp_codegen_add((intptr_t)L_7, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_8)), (int32_t)L_9)))));
		((StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)il2cpp_codegen_add((intptr_t)L_7, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_8)), (int32_t)L_9))))->set_FreeBits_0((((int64_t)((int64_t)(-1)))));
		// for (int i = 0; i < 64; ++i)
		int32_t L_10 = V_3;
		V_3 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0048:
	{
		// for (int i = 0; i < 64; ++i)
		int32_t L_11 = V_3;
		if ((((int32_t)L_11) < ((int32_t)((int32_t)64))))
		{
			goto IL_0032;
		}
	}
	{
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void StateAllocator_Init_mB98D03A98BB079DC98429B88AD7F3A518C687F48_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * _thisAdjusted = reinterpret_cast<StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB *>(__this + _offset);
	StateAllocator_Init_mB98D03A98BB079DC98429B88AD7F3A518C687F48(_thisAdjusted, method);
}
// System.Void Unity.Entities.World_StateAllocator::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StateAllocator_Dispose_m3D825FC8909235B193459C939BF77334AA21B756 (StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * __this, const RuntimeMethod* method)
{
	StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * V_0 = NULL;
	int32_t V_1 = 0;
	{
		// var l1 = m_Level1;
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_0 = __this->get_m_Level1_1();
		V_0 = (StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)L_0;
		// for (int i = 0; i < 64; ++i)
		V_1 = 0;
		goto IL_0039;
	}

IL_000b:
	{
		// if (l1[i].States != null)
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_1 = V_0;
		int32_t L_2 = V_1;
		uint32_t L_3 = sizeof(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 );
		NullCheck(((StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)il2cpp_codegen_add((intptr_t)L_1, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_2)), (int32_t)L_3)))));
		SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * L_4 = ((StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)il2cpp_codegen_add((intptr_t)L_1, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_2)), (int32_t)L_3))))->get_States_1();
		if ((((intptr_t)L_4) == ((intptr_t)(((uintptr_t)0)))))
		{
			goto IL_0035;
		}
	}
	{
		// UnsafeUtility.Free(l1[i].States, Allocator.Persistent);
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_5 = V_0;
		int32_t L_6 = V_1;
		uint32_t L_7 = sizeof(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 );
		NullCheck(((StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)il2cpp_codegen_add((intptr_t)L_5, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_6)), (int32_t)L_7)))));
		SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * L_8 = ((StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)il2cpp_codegen_add((intptr_t)L_5, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_6)), (int32_t)L_7))))->get_States_1();
		UnsafeUtility_Free_mA805168FF1B6728E7DF3AD1DE47400B37F3441F9((void*)(void*)L_8, 4, /*hidden argument*/NULL);
	}

IL_0035:
	{
		// for (int i = 0; i < 64; ++i)
		int32_t L_9 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_9, (int32_t)1));
	}

IL_0039:
	{
		// for (int i = 0; i < 64; ++i)
		int32_t L_10 = V_1;
		if ((((int32_t)L_10) < ((int32_t)((int32_t)64))))
		{
			goto IL_000b;
		}
	}
	{
		// UnsafeUtility.Free(l1, Allocator.Persistent);
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_11 = V_0;
		UnsafeUtility_Free_mA805168FF1B6728E7DF3AD1DE47400B37F3441F9((void*)(void*)L_11, 4, /*hidden argument*/NULL);
		// m_Level1 = null;
		__this->set_m_Level1_1((StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)(((uintptr_t)0)));
		// this = default;
		il2cpp_codegen_initobj(__this, sizeof(StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB ));
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void StateAllocator_Dispose_m3D825FC8909235B193459C939BF77334AA21B756_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * _thisAdjusted = reinterpret_cast<StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB *>(__this + _offset);
	StateAllocator_Dispose_m3D825FC8909235B193459C939BF77334AA21B756(_thisAdjusted, method);
}
// Unity.Entities.SystemState* Unity.Entities.World_StateAllocator::Resolve(System.UInt16,System.UInt16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * StateAllocator_Resolve_m9CDD04D63F464059F7A7B242B6F3DA872D9C3EA0 (StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * __this, uint16_t ___handle0, uint16_t ___version1, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * V_2 = NULL;
	{
		// int index = handle >> 6;
		uint16_t L_0 = ___handle0;
		V_0 = ((int32_t)((int32_t)L_0>>(int32_t)6));
		// int subIndex = handle & 63;
		uint16_t L_1 = ___handle0;
		V_1 = ((int32_t)((int32_t)L_1&(int32_t)((int32_t)63)));
		// ref var leaf = ref m_Level1[index];
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_2 = __this->get_m_Level1_1();
		int32_t L_3 = V_0;
		uint32_t L_4 = sizeof(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 );
		V_2 = (StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)((StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)il2cpp_codegen_add((intptr_t)L_2, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_3)), (int32_t)L_4))));
		// return leaf.Version[subIndex] == version ? leaf.States + subIndex : null;
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_5 = V_2;
		U3CVersionU3Ee__FixedBuffer_tB3AA164CD6E485CFAEFC56242AE5BE61BE39446F * L_6 = L_5->get_address_of_Version_2();
		uint16_t* L_7 = L_6->get_address_of_FixedElementField_0();
		int32_t L_8 = V_1;
		int32_t L_9 = *((uint16_t*)((uint16_t*)il2cpp_codegen_add((intptr_t)L_7, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_8)), (int32_t)2)))));
		uint16_t L_10 = ___version1;
		if ((((int32_t)L_9) == ((int32_t)L_10)))
		{
			goto IL_0031;
		}
	}
	{
		return (SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 *)((((uintptr_t)0)));
	}

IL_0031:
	{
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_11 = V_2;
		SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * L_12 = L_11->get_States_1();
		int32_t L_13 = V_1;
		uint32_t L_14 = sizeof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 );
		return (SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 *)(((SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 *)il2cpp_codegen_add((intptr_t)L_12, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_13)), (int32_t)L_14)))));
	}
}
IL2CPP_EXTERN_C  SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * StateAllocator_Resolve_m9CDD04D63F464059F7A7B242B6F3DA872D9C3EA0_AdjustorThunk (RuntimeObject * __this, uint16_t ___handle0, uint16_t ___version1, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * _thisAdjusted = reinterpret_cast<StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB *>(__this + _offset);
	return StateAllocator_Resolve_m9CDD04D63F464059F7A7B242B6F3DA872D9C3EA0(_thisAdjusted, ___handle0, ___version1, method);
}
// Unity.Entities.SystemState* Unity.Entities.World_StateAllocator::ResolveNoCheck(System.UInt16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * StateAllocator_ResolveNoCheck_mFE33F97618D65134C886467FC951763D203338F1 (StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * __this, uint16_t ___handle0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		// int index = handle >> 6;
		uint16_t L_0 = ___handle0;
		V_0 = ((int32_t)((int32_t)L_0>>(int32_t)6));
		// int subIndex = handle & 63;
		uint16_t L_1 = ___handle0;
		V_1 = ((int32_t)((int32_t)L_1&(int32_t)((int32_t)63)));
		// return m_Level1[index].States + subIndex;
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_2 = __this->get_m_Level1_1();
		int32_t L_3 = V_0;
		uint32_t L_4 = sizeof(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 );
		NullCheck(((StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)il2cpp_codegen_add((intptr_t)L_2, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_3)), (int32_t)L_4)))));
		SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * L_5 = ((StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)il2cpp_codegen_add((intptr_t)L_2, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_3)), (int32_t)L_4))))->get_States_1();
		int32_t L_6 = V_1;
		uint32_t L_7 = sizeof(SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 );
		return (SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 *)(((SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 *)il2cpp_codegen_add((intptr_t)L_5, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_6)), (int32_t)L_7)))));
	}
}
IL2CPP_EXTERN_C  SystemState_t36E4EBEDC1A8DE324669E6A495E2CD70B8B12D95 * StateAllocator_ResolveNoCheck_mFE33F97618D65134C886467FC951763D203338F1_AdjustorThunk (RuntimeObject * __this, uint16_t ___handle0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * _thisAdjusted = reinterpret_cast<StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB *>(__this + _offset);
	return StateAllocator_ResolveNoCheck_mFE33F97618D65134C886467FC951763D203338F1(_thisAdjusted, ___handle0, method);
}
// System.Void Unity.Entities.World_StateAllocator::Free(System.UInt16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StateAllocator_Free_m45D1DFD0FB9181C5082B461716267176A8CCC779 (StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * __this, uint16_t ___handle0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		// int index = handle >> 6;
		uint16_t L_0 = ___handle0;
		V_0 = ((int32_t)((int32_t)L_0>>(int32_t)6));
		// int subIndex = handle & 63;
		uint16_t L_1 = ___handle0;
		V_1 = ((int32_t)((int32_t)L_1&(int32_t)((int32_t)63)));
		// m_FreeBits |= 1ul << index;
		uint64_t L_2 = __this->get_m_FreeBits_0();
		int32_t L_3 = V_0;
		__this->set_m_FreeBits_0(((int64_t)((int64_t)L_2|(int64_t)((int64_t)((int64_t)(((int64_t)((int64_t)1)))<<(int32_t)((int32_t)((int32_t)L_3&(int32_t)((int32_t)63))))))));
		// ref var leaf = ref *(m_Level1 + index);
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_4 = __this->get_m_Level1_1();
		int32_t L_5 = V_0;
		uint32_t L_6 = sizeof(StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 );
		// leaf.FreeBits |= (1ul << subIndex);
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_7 = (StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)((StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)il2cpp_codegen_add((intptr_t)L_4, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_5)), (int32_t)L_6))));
		NullCheck(L_7);
		uint64_t* L_8 = L_7->get_address_of_FreeBits_0();
		uint64_t* L_9 = L_8;
		int64_t L_10 = *((int64_t*)L_9);
		int32_t L_11 = V_1;
		*((int64_t*)L_9) = (int64_t)((int64_t)((int64_t)L_10|(int64_t)((int64_t)((int64_t)(((int64_t)((int64_t)1)))<<(int32_t)((int32_t)((int32_t)L_11&(int32_t)((int32_t)63)))))));
		// IncVersion(ref leaf.Version[subIndex]);
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_12 = (StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)L_7;
		NullCheck(L_12);
		U3CVersionU3Ee__FixedBuffer_tB3AA164CD6E485CFAEFC56242AE5BE61BE39446F * L_13 = L_12->get_address_of_Version_2();
		uint16_t* L_14 = L_13->get_address_of_FixedElementField_0();
		int32_t L_15 = V_1;
		StateAllocator_IncVersion_m8B09B61A0169E5A9020595FF2EAC194A8FB8B308((uint16_t*)((uint16_t*)il2cpp_codegen_add((intptr_t)L_14, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_15)), (int32_t)2)))), /*hidden argument*/NULL);
		// leaf.SystemPointer[subIndex] = 0;
		StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 * L_16 = (StateAllocLevel1_tB628A9A32ACFF2D952B6D0B85F8E8429A8114C82 *)L_12;
		NullCheck(L_16);
		U3CSystemPointerU3Ee__FixedBuffer_t19C0D38015C844F379AB29453AE403CBDD5E16B1 * L_17 = L_16->get_address_of_SystemPointer_4();
		uint64_t* L_18 = L_17->get_address_of_FixedElementField_0();
		int32_t L_19 = V_1;
		*((int64_t*)((uint64_t*)il2cpp_codegen_add((intptr_t)L_18, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_19)), (int32_t)8))))) = (int64_t)(((int64_t)((int64_t)0)));
		// leaf.TypeHash[subIndex] = 0;
		NullCheck(L_16);
		U3CTypeHashU3Ee__FixedBuffer_tEE844B127F8E28AF008BEC971AE9386972B3D6D5 * L_20 = L_16->get_address_of_TypeHash_3();
		int64_t* L_21 = L_20->get_address_of_FixedElementField_0();
		int32_t L_22 = V_1;
		*((int64_t*)((int64_t*)il2cpp_codegen_add((intptr_t)L_21, (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_22)), (int32_t)8))))) = (int64_t)(((int64_t)((int64_t)0)));
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void StateAllocator_Free_m45D1DFD0FB9181C5082B461716267176A8CCC779_AdjustorThunk (RuntimeObject * __this, uint16_t ___handle0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB * _thisAdjusted = reinterpret_cast<StateAllocator_t6D96E72B2D6B5E9C0AF816CDC89BF633C356B9CB *>(__this + _offset);
	StateAllocator_Free_m45D1DFD0FB9181C5082B461716267176A8CCC779(_thisAdjusted, ___handle0, method);
}
// System.Void Unity.Entities.World_StateAllocator::IncVersion(System.UInt16&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StateAllocator_IncVersion_m8B09B61A0169E5A9020595FF2EAC194A8FB8B308 (uint16_t* ___v0, const RuntimeMethod* method)
{
	uint32_t V_0 = 0;
	{
		// uint m = v;
		uint16_t* L_0 = ___v0;
		int32_t L_1 = *((uint16_t*)L_0);
		V_0 = L_1;
		// m += 1;
		uint32_t L_2 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_2, (int32_t)1));
		// m = (m >> 16) | m; // Fold overflow bit down to make 0xffff wrap to 0x0001, avoiding zero which is reserved for "unused"
		uint32_t L_3 = V_0;
		uint32_t L_4 = V_0;
		V_0 = ((int32_t)((int32_t)((int32_t)((uint32_t)L_3>>((int32_t)16)))|(int32_t)L_4));
		// v = (ushort)m;
		uint16_t* L_5 = ___v0;
		uint32_t L_6 = V_0;
		*((int16_t*)L_5) = (int16_t)(((int32_t)((uint16_t)L_6)));
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
// Conversion methods for marshalling of: Unity.Entities.EntityManager/DeprecatedRegistry/Cell
IL2CPP_EXTERN_C void Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75_marshal_pinvoke(const Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75& unmarshaled, Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75_marshaled_pinvoke& marshaled)
{
	Exception_t* ___World_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'World' of type 'Cell': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___World_0Exception, NULL);
}
IL2CPP_EXTERN_C void Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75_marshal_pinvoke_back(const Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75_marshaled_pinvoke& marshaled, Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75& unmarshaled)
{
	Exception_t* ___World_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'World' of type 'Cell': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___World_0Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.EntityManager/DeprecatedRegistry/Cell
IL2CPP_EXTERN_C void Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75_marshal_pinvoke_cleanup(Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: Unity.Entities.EntityManager/DeprecatedRegistry/Cell
IL2CPP_EXTERN_C void Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75_marshal_com(const Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75& unmarshaled, Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75_marshaled_com& marshaled)
{
	Exception_t* ___World_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'World' of type 'Cell': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___World_0Exception, NULL);
}
IL2CPP_EXTERN_C void Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75_marshal_com_back(const Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75_marshaled_com& marshaled, Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75& unmarshaled)
{
	Exception_t* ___World_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'World' of type 'Cell': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___World_0Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.EntityManager/DeprecatedRegistry/Cell
IL2CPP_EXTERN_C void Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75_marshal_com_cleanup(Cell_t36C5CA86BBC206E7B8F3A41B36F1835332FDCA75_marshaled_com& marshaled)
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
// System.Void Unity.Entities.EntityPatcher_EntityDiffPatcher_EntityPatchAdapter::.ctor(Unity.Collections.NativeMultiHashMap`2_Enumerator<Unity.Entities.EntityPatcher_EntityComponentPair,Unity.Entities.EntityPatcher_OffsetEntityPair>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EntityPatchAdapter__ctor_m6250A628B7B9F8F06233C0CC4BEC85E38032D808 (EntityPatchAdapter_tE216B50635D7EEC4301D9C54E3F1894F658D59AC * __this, Enumerator_t22279CE04A52106502C7CC851E8ADC1CA6BA1D0E  ___patches0, const RuntimeMethod* method)
{
	{
		// public EntityPatchAdapter(NativeMultiHashMap<EntityComponentPair, OffsetEntityPair>.Enumerator patches)
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405(__this, /*hidden argument*/NULL);
		// Patches = patches;
		Enumerator_t22279CE04A52106502C7CC851E8ADC1CA6BA1D0E  L_0 = ___patches0;
		__this->set_Patches_0(L_0);
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
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C  bool DelegatePInvokeWrapper_CompareEqualDelegate_t15914FD0CE4F07A1C8E6EA6B2A931D83586220D7 (CompareEqualDelegate_t15914FD0CE4F07A1C8E6EA6B2A931D83586220D7 * __this, void* ___lhs0, void* ___rhs1, const RuntimeMethod* method)
{
	typedef int32_t (DEFAULT_CALL *PInvokeFunc)(void*, void*);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	int32_t returnValue = il2cppPInvokeFunc(___lhs0, ___rhs1);

	return static_cast<bool>(returnValue);
}
// System.Void Unity.Entities.FastEquality_TypeInfo_CompareEqualDelegate::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CompareEqualDelegate__ctor_mDC7E7C7CE8D7A1ADF1A38DA8E48C5B25B69EAC34 (CompareEqualDelegate_t15914FD0CE4F07A1C8E6EA6B2A931D83586220D7 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Boolean Unity.Entities.FastEquality_TypeInfo_CompareEqualDelegate::Invoke(System.Void*,System.Void*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool CompareEqualDelegate_Invoke_m5739F9C2FF003E85B71A04ED84DED61719339A59 (CompareEqualDelegate_t15914FD0CE4F07A1C8E6EA6B2A931D83586220D7 * __this, void* ___lhs0, void* ___rhs1, const RuntimeMethod* method)
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
			if (___parameterCount == 2)
			{
				// open
				typedef bool (*FunctionPointerType) (void*, void*, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___lhs0, ___rhs1, targetMethod);
			}
			else
			{
				// closed
				typedef bool (*FunctionPointerType) (void*, void*, void*, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___lhs0, ___rhs1, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef bool (*FunctionPointerType) (void*, void*, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(___lhs0, ___rhs1, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker2< bool, void*, void* >::Invoke(targetMethod, targetThis, ___lhs0, ___rhs1);
					else
						result = GenericVirtFuncInvoker2< bool, void*, void* >::Invoke(targetMethod, targetThis, ___lhs0, ___rhs1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker2< bool, void*, void* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___lhs0, ___rhs1);
					else
						result = VirtFuncInvoker2< bool, void*, void* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___lhs0, ___rhs1);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef bool (*FunctionPointerType) (RuntimeObject*, void*, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___lhs0) - 1), ___rhs1, targetMethod);
				}
				else
				{
					typedef bool (*FunctionPointerType) (void*, void*, void*, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___lhs0, ___rhs1, targetMethod);
				}
			}
		}
	}
	return result;
}
// System.IAsyncResult Unity.Entities.FastEquality_TypeInfo_CompareEqualDelegate::BeginInvoke(System.Void*,System.Void*,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* CompareEqualDelegate_BeginInvoke_m27F541D963EE3EFAE29183A1AF3C34C2627883E0 (CompareEqualDelegate_t15914FD0CE4F07A1C8E6EA6B2A931D83586220D7 * __this, void* ___lhs0, void* ___rhs1, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback2, RuntimeObject * ___object3, const RuntimeMethod* method)
{
	void *__d_args[3] = {0};
	__d_args[0] = ___lhs0;
	__d_args[1] = ___rhs1;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback2, (RuntimeObject*)___object3);
}
// System.Boolean Unity.Entities.FastEquality_TypeInfo_CompareEqualDelegate::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool CompareEqualDelegate_EndInvoke_mD46A664F92F6CAD5E812E0A249DC3D37A6B9A78E (CompareEqualDelegate_t15914FD0CE4F07A1C8E6EA6B2A931D83586220D7 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  int32_t DelegatePInvokeWrapper_GetHashCodeDelegate_t0463D39E4D6E298D2C15D08C75CAFB0CBF02E353 (GetHashCodeDelegate_t0463D39E4D6E298D2C15D08C75CAFB0CBF02E353 * __this, void* ___obj0, const RuntimeMethod* method)
{
	typedef int32_t (DEFAULT_CALL *PInvokeFunc)(void*);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	int32_t returnValue = il2cppPInvokeFunc(___obj0);

	return returnValue;
}
// System.Void Unity.Entities.FastEquality_TypeInfo_GetHashCodeDelegate::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GetHashCodeDelegate__ctor_m07F0D904B4A5B04D177D4412BAE3939D5A8EC7D3 (GetHashCodeDelegate_t0463D39E4D6E298D2C15D08C75CAFB0CBF02E353 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Int32 Unity.Entities.FastEquality_TypeInfo_GetHashCodeDelegate::Invoke(System.Void*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t GetHashCodeDelegate_Invoke_m783694E429215B3FB9BD22CE0BCF621878DF7CBE (GetHashCodeDelegate_t0463D39E4D6E298D2C15D08C75CAFB0CBF02E353 * __this, void* ___obj0, const RuntimeMethod* method)
{
	int32_t result = 0;
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
				typedef int32_t (*FunctionPointerType) (void*, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___obj0, targetMethod);
			}
			else
			{
				// closed
				typedef int32_t (*FunctionPointerType) (void*, void*, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___obj0, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef int32_t (*FunctionPointerType) (void*, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(___obj0, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker1< int32_t, void* >::Invoke(targetMethod, targetThis, ___obj0);
					else
						result = GenericVirtFuncInvoker1< int32_t, void* >::Invoke(targetMethod, targetThis, ___obj0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker1< int32_t, void* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___obj0);
					else
						result = VirtFuncInvoker1< int32_t, void* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___obj0);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef int32_t (*FunctionPointerType) (RuntimeObject*, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___obj0) - 1), targetMethod);
				}
				else
				{
					typedef int32_t (*FunctionPointerType) (void*, void*, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___obj0, targetMethod);
				}
			}
		}
	}
	return result;
}
// System.IAsyncResult Unity.Entities.FastEquality_TypeInfo_GetHashCodeDelegate::BeginInvoke(System.Void*,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* GetHashCodeDelegate_BeginInvoke_m517EF72A4BC3798F52A74285CE8D135F6EDDDC82 (GetHashCodeDelegate_t0463D39E4D6E298D2C15D08C75CAFB0CBF02E353 * __this, void* ___obj0, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback1, RuntimeObject * ___object2, const RuntimeMethod* method)
{
	void *__d_args[2] = {0};
	__d_args[0] = ___obj0;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback1, (RuntimeObject*)___object2);
}
// System.Int32 Unity.Entities.FastEquality_TypeInfo_GetHashCodeDelegate::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t GetHashCodeDelegate_EndInvoke_m80074FC2AB1B13AA8339A4A98C5F4E8A89FEAF1D (GetHashCodeDelegate_t0463D39E4D6E298D2C15D08C75CAFB0CBF02E353 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result0, 0);
	return *(int32_t*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Unity.Entities.FastEquality_TypeInfo_ManagedCompareEqualDelegate::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ManagedCompareEqualDelegate__ctor_mFF2865C3E4B4FD75165E57DD475086AD7B9EAC59 (ManagedCompareEqualDelegate_t1D9D97E36B8D0245138610749B6F2B74D6CEBB06 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Boolean Unity.Entities.FastEquality_TypeInfo_ManagedCompareEqualDelegate::Invoke(System.Object,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ManagedCompareEqualDelegate_Invoke_mF924A0A354F43994592EF8E2AEF5B43411533063 (ManagedCompareEqualDelegate_t1D9D97E36B8D0245138610749B6F2B74D6CEBB06 * __this, RuntimeObject * ___lhs0, RuntimeObject * ___rhs1, const RuntimeMethod* method)
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
			if (___parameterCount == 2)
			{
				// open
				typedef bool (*FunctionPointerType) (RuntimeObject *, RuntimeObject *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___lhs0, ___rhs1, targetMethod);
			}
			else
			{
				// closed
				typedef bool (*FunctionPointerType) (void*, RuntimeObject *, RuntimeObject *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___lhs0, ___rhs1, targetMethod);
			}
		}
		else if (___parameterCount != 2)
		{
			// open
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker1< bool, RuntimeObject * >::Invoke(targetMethod, ___lhs0, ___rhs1);
					else
						result = GenericVirtFuncInvoker1< bool, RuntimeObject * >::Invoke(targetMethod, ___lhs0, ___rhs1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker1< bool, RuntimeObject * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___lhs0, ___rhs1);
					else
						result = VirtFuncInvoker1< bool, RuntimeObject * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___lhs0, ___rhs1);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef bool (*FunctionPointerType) (RuntimeObject*, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___rhs1) - 1), targetMethod);
				}
				else
				{
					typedef bool (*FunctionPointerType) (RuntimeObject *, RuntimeObject *, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(___lhs0, ___rhs1, targetMethod);
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
					typedef bool (*FunctionPointerType) (RuntimeObject *, RuntimeObject *, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(___lhs0, ___rhs1, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(targetMethod, targetThis, ___lhs0, ___rhs1);
					else
						result = GenericVirtFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(targetMethod, targetThis, ___lhs0, ___rhs1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___lhs0, ___rhs1);
					else
						result = VirtFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___lhs0, ___rhs1);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef bool (*FunctionPointerType) (RuntimeObject*, RuntimeObject *, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___lhs0) - 1), ___rhs1, targetMethod);
				}
				else
				{
					typedef bool (*FunctionPointerType) (void*, RuntimeObject *, RuntimeObject *, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___lhs0, ___rhs1, targetMethod);
				}
			}
		}
	}
	return result;
}
// System.IAsyncResult Unity.Entities.FastEquality_TypeInfo_ManagedCompareEqualDelegate::BeginInvoke(System.Object,System.Object,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* ManagedCompareEqualDelegate_BeginInvoke_mCCD8420BE86BF03B5C8D752340CA37B43B75D92F (ManagedCompareEqualDelegate_t1D9D97E36B8D0245138610749B6F2B74D6CEBB06 * __this, RuntimeObject * ___lhs0, RuntimeObject * ___rhs1, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback2, RuntimeObject * ___object3, const RuntimeMethod* method)
{
	void *__d_args[3] = {0};
	__d_args[0] = ___lhs0;
	__d_args[1] = ___rhs1;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback2, (RuntimeObject*)___object3);
}
// System.Boolean Unity.Entities.FastEquality_TypeInfo_ManagedCompareEqualDelegate::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ManagedCompareEqualDelegate_EndInvoke_m60591EB37FB069FF6E2F7724CB590F44269D96C3 (ManagedCompareEqualDelegate_t1D9D97E36B8D0245138610749B6F2B74D6CEBB06 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
// System.Void Unity.Entities.FastEquality_TypeInfo_ManagedGetHashCodeDelegate::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ManagedGetHashCodeDelegate__ctor_m2E84C8A3916B2CF4EF01B49641FA9032291BE6A1 (ManagedGetHashCodeDelegate_tDC6EDBDBB5F0F94C90DFCB41F6692CBA3B2A75DC * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Int32 Unity.Entities.FastEquality_TypeInfo_ManagedGetHashCodeDelegate::Invoke(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ManagedGetHashCodeDelegate_Invoke_mEC2DBF42DCBE0A4D9590D9BC39494778A40BDE60 (ManagedGetHashCodeDelegate_tDC6EDBDBB5F0F94C90DFCB41F6692CBA3B2A75DC * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t result = 0;
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
				typedef int32_t (*FunctionPointerType) (RuntimeObject *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___obj0, targetMethod);
			}
			else
			{
				// closed
				typedef int32_t (*FunctionPointerType) (void*, RuntimeObject *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___obj0, targetMethod);
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
						result = GenericInterfaceFuncInvoker0< int32_t >::Invoke(targetMethod, ___obj0);
					else
						result = GenericVirtFuncInvoker0< int32_t >::Invoke(targetMethod, ___obj0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker0< int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___obj0);
					else
						result = VirtFuncInvoker0< int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___obj0);
				}
			}
			else
			{
				typedef int32_t (*FunctionPointerType) (RuntimeObject *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___obj0, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef int32_t (*FunctionPointerType) (RuntimeObject *, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(___obj0, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker1< int32_t, RuntimeObject * >::Invoke(targetMethod, targetThis, ___obj0);
					else
						result = GenericVirtFuncInvoker1< int32_t, RuntimeObject * >::Invoke(targetMethod, targetThis, ___obj0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker1< int32_t, RuntimeObject * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___obj0);
					else
						result = VirtFuncInvoker1< int32_t, RuntimeObject * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___obj0);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef int32_t (*FunctionPointerType) (RuntimeObject*, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___obj0) - 1), targetMethod);
				}
				else
				{
					typedef int32_t (*FunctionPointerType) (void*, RuntimeObject *, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___obj0, targetMethod);
				}
			}
		}
	}
	return result;
}
// System.IAsyncResult Unity.Entities.FastEquality_TypeInfo_ManagedGetHashCodeDelegate::BeginInvoke(System.Object,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* ManagedGetHashCodeDelegate_BeginInvoke_mF7A01C9D7460268FF85C55FB7970B554C43F08D7 (ManagedGetHashCodeDelegate_tDC6EDBDBB5F0F94C90DFCB41F6692CBA3B2A75DC * __this, RuntimeObject * ___obj0, AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA * ___callback1, RuntimeObject * ___object2, const RuntimeMethod* method)
{
	void *__d_args[2] = {0};
	__d_args[0] = ___obj0;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback1, (RuntimeObject*)___object2);
}
// System.Int32 Unity.Entities.FastEquality_TypeInfo_ManagedGetHashCodeDelegate::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ManagedGetHashCodeDelegate_EndInvoke_m45F89CEB6406763C422345E0F5B8BDF7DA1C9DB8 (ManagedGetHashCodeDelegate_tDC6EDBDBB5F0F94C90DFCB41F6692CBA3B2A75DC * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result0, 0);
	return *(int32_t*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Conversion methods for marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0/LambdaParameterValueProviders
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7_marshal_pinvoke(const LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7& unmarshaled, LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7_marshaled_pinvoke& marshaled)
{
	Exception_t* ___forParameter_blobOwner_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'forParameter_blobOwner' of type 'LambdaParameterValueProviders'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___forParameter_blobOwner_1Exception, NULL);
}
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7_marshal_pinvoke_back(const LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7_marshaled_pinvoke& marshaled, LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7& unmarshaled)
{
	Exception_t* ___forParameter_blobOwner_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'forParameter_blobOwner' of type 'LambdaParameterValueProviders'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___forParameter_blobOwner_1Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0/LambdaParameterValueProviders
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7_marshal_pinvoke_cleanup(LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0/LambdaParameterValueProviders
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7_marshal_com(const LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7& unmarshaled, LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7_marshaled_com& marshaled)
{
	Exception_t* ___forParameter_blobOwner_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'forParameter_blobOwner' of type 'LambdaParameterValueProviders'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___forParameter_blobOwner_1Exception, NULL);
}
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7_marshal_com_back(const LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7_marshaled_com& marshaled, LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7& unmarshaled)
{
	Exception_t* ___forParameter_blobOwner_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'forParameter_blobOwner' of type 'LambdaParameterValueProviders'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___forParameter_blobOwner_1Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0/LambdaParameterValueProviders
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7_marshal_com_cleanup(LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7_marshaled_com& marshaled)
{
}
// Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0_LambdaParameterValueProviders_Runtimes Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0_LambdaParameterValueProviders::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879  LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_m5D4BC103CAC65745AB7DF1CB09E4DC7CEF6DFE9D (LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7 * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___p00, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___p11, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_m5D4BC103CAC65745AB7DF1CB09E4DC7CEF6DFE9D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		il2cpp_codegen_initobj((&V_0), sizeof(Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879 ));
		StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994 * L_0 = (&V_0)->get_address_of__entityProvider_0();
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_1 = ___p00;
		EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  L_2 = ___p11;
		StructuralChangeEntityProvider_PrepareToExecuteWithStructuralChanges_m869CCD0EEF00648F7E495A94C7309A61B02B5949((StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994 *)L_0, L_1, L_2, /*hidden argument*/NULL);
		LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13 * L_3 = __this->get_address_of_forParameter_e_0();
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_4 = ___p00;
		EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  L_5 = ___p11;
		StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  L_6 = LambdaParameterValueProvider_Entity_PrepareToExecuteWithStructuralChanges_m3D096D764543E8D1024D628FDE07EC81C10DF8E3((LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13 *)L_3, L_4, L_5, /*hidden argument*/NULL);
		(&V_0)->set_runtime_e_1(L_6);
		LambdaParameterValueProvider_ISharedComponentData_1_tE871547CB822CE66B2C193D94183ACD389503DF6 * L_7 = __this->get_address_of_forParameter_blobOwner_1();
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_8 = ___p00;
		EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  L_9 = ___p11;
		StructuralChangeRuntime_t73F4A22CB7388EB3FB14FDAB733CD0AA00539B60  L_10 = LambdaParameterValueProvider_ISharedComponentData_1_PrepareToExecuteWithStructuralChanges_m2A619A397B3B2222985218AA7CEA6259FEEA0337((LambdaParameterValueProvider_ISharedComponentData_1_tE871547CB822CE66B2C193D94183ACD389503DF6 *)L_7, L_8, L_9, /*hidden argument*/LambdaParameterValueProvider_ISharedComponentData_1_PrepareToExecuteWithStructuralChanges_m2A619A397B3B2222985218AA7CEA6259FEEA0337_RuntimeMethod_var);
		(&V_0)->set_runtime_blobOwner_2(L_10);
		LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4 * L_11 = __this->get_address_of_forParameter_retain_2();
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_12 = ___p00;
		EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  L_13 = ___p11;
		StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  L_14 = LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mD35E9088251B87C54D4C9FE9CE4B2C68F7661646((LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4 *)L_11, L_12, L_13, /*hidden argument*/LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mD35E9088251B87C54D4C9FE9CE4B2C68F7661646_RuntimeMethod_var);
		(&V_0)->set_runtime_retain_3(L_14);
		Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879  L_15 = V_0;
		return L_15;
	}
}
IL2CPP_EXTERN_C  Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879  LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_m5D4BC103CAC65745AB7DF1CB09E4DC7CEF6DFE9D_AdjustorThunk (RuntimeObject * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___p00, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___p11, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7 * _thisAdjusted = reinterpret_cast<LambdaParameterValueProviders_tDAEA00694BAD6D11B29E3F91EE80AB8923218BB7 *>(__this + _offset);
	return LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_m5D4BC103CAC65745AB7DF1CB09E4DC7CEF6DFE9D(_thisAdjusted, ___p00, ___p11, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Conversion methods for marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1/LambdaParameterValueProviders
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF_marshal_pinvoke(const LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF& unmarshaled, LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF_marshaled_pinvoke& marshaled)
{
	Exception_t* ___forParameter_retain_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'forParameter_retain' of type 'LambdaParameterValueProviders'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___forParameter_retain_1Exception, NULL);
}
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF_marshal_pinvoke_back(const LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF_marshaled_pinvoke& marshaled, LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF& unmarshaled)
{
	Exception_t* ___forParameter_retain_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'forParameter_retain' of type 'LambdaParameterValueProviders'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___forParameter_retain_1Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1/LambdaParameterValueProviders
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF_marshal_pinvoke_cleanup(LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1/LambdaParameterValueProviders
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF_marshal_com(const LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF& unmarshaled, LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF_marshaled_com& marshaled)
{
	Exception_t* ___forParameter_retain_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'forParameter_retain' of type 'LambdaParameterValueProviders'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___forParameter_retain_1Exception, NULL);
}
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF_marshal_com_back(const LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF_marshaled_com& marshaled, LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF& unmarshaled)
{
	Exception_t* ___forParameter_retain_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'forParameter_retain' of type 'LambdaParameterValueProviders'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___forParameter_retain_1Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1/LambdaParameterValueProviders
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF_marshal_com_cleanup(LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF_marshaled_com& marshaled)
{
}
// Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1_LambdaParameterValueProviders_Runtimes Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1_LambdaParameterValueProviders::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF  LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_mAC1D60747A9EA28974FBF31EE269AE7161E901D8 (LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___p00, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___p11, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_mAC1D60747A9EA28974FBF31EE269AE7161E901D8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		il2cpp_codegen_initobj((&V_0), sizeof(Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF ));
		StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994 * L_0 = (&V_0)->get_address_of__entityProvider_0();
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_1 = ___p00;
		EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  L_2 = ___p11;
		StructuralChangeEntityProvider_PrepareToExecuteWithStructuralChanges_m869CCD0EEF00648F7E495A94C7309A61B02B5949((StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994 *)L_0, L_1, L_2, /*hidden argument*/NULL);
		LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13 * L_3 = __this->get_address_of_forParameter_e_0();
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_4 = ___p00;
		EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  L_5 = ___p11;
		StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  L_6 = LambdaParameterValueProvider_Entity_PrepareToExecuteWithStructuralChanges_m3D096D764543E8D1024D628FDE07EC81C10DF8E3((LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13 *)L_3, L_4, L_5, /*hidden argument*/NULL);
		(&V_0)->set_runtime_e_1(L_6);
		LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4 * L_7 = __this->get_address_of_forParameter_retain_1();
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_8 = ___p00;
		EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  L_9 = ___p11;
		StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  L_10 = LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mD35E9088251B87C54D4C9FE9CE4B2C68F7661646((LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4 *)L_7, L_8, L_9, /*hidden argument*/LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mD35E9088251B87C54D4C9FE9CE4B2C68F7661646_RuntimeMethod_var);
		(&V_0)->set_runtime_retain_2(L_10);
		LambdaParameterValueProvider_IComponentData_1_t1D786064C86636813BA4C8371E0CF96EAC3BAB81 * L_11 = __this->get_address_of_forParameter_retainPtr_2();
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_12 = ___p00;
		EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  L_13 = ___p11;
		StructuralChangeRuntime_t91CF003F24C722618BFAB2EC15E454A10524C5F5  L_14 = LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mFB4F12F46E5EC3C3B81DFD695837BB9B81A28A3E((LambdaParameterValueProvider_IComponentData_1_t1D786064C86636813BA4C8371E0CF96EAC3BAB81 *)L_11, L_12, L_13, /*hidden argument*/LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mFB4F12F46E5EC3C3B81DFD695837BB9B81A28A3E_RuntimeMethod_var);
		(&V_0)->set_runtime_retainPtr_3(L_14);
		Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF  L_15 = V_0;
		return L_15;
	}
}
IL2CPP_EXTERN_C  Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF  LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_mAC1D60747A9EA28974FBF31EE269AE7161E901D8_AdjustorThunk (RuntimeObject * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___p00, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___p11, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF * _thisAdjusted = reinterpret_cast<LambdaParameterValueProviders_tF6E1A4EB1AD9BA7FBCFFFDC4A06DF58590F1FBAF *>(__this + _offset);
	return LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_mAC1D60747A9EA28974FBF31EE269AE7161E901D8(_thisAdjusted, ___p00, ___p11, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Conversion methods for marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshal_pinvoke(const LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31& unmarshaled, LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_pinvoke& marshaled)
{
	Exception_t* ___forParameter_retain_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'forParameter_retain' of type 'LambdaParameterValueProviders'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___forParameter_retain_1Exception, NULL);
}
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshal_pinvoke_back(const LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_pinvoke& marshaled, LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31& unmarshaled)
{
	Exception_t* ___forParameter_retain_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'forParameter_retain' of type 'LambdaParameterValueProviders'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___forParameter_retain_1Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshal_pinvoke_cleanup(LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshal_com(const LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31& unmarshaled, LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_com& marshaled)
{
	Exception_t* ___forParameter_retain_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'forParameter_retain' of type 'LambdaParameterValueProviders'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___forParameter_retain_1Exception, NULL);
}
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshal_com_back(const LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_com& marshaled, LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31& unmarshaled)
{
	Exception_t* ___forParameter_retain_1Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'forParameter_retain' of type 'LambdaParameterValueProviders'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___forParameter_retain_1Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders
IL2CPP_EXTERN_C void LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshal_com_cleanup(LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31_marshaled_com& marshaled)
{
}
// Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2_LambdaParameterValueProviders_Runtimes Unity.Entities.RetainBlobAssetSystem_Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2_LambdaParameterValueProviders::PrepareToExecuteWithStructuralChanges(Unity.Entities.ComponentSystemBase,Unity.Entities.EntityQuery)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8  LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_m06AE7D081E5B1F77F983D9608E29D6E801B94B94 (LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31 * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___p00, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___p11, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_m06AE7D081E5B1F77F983D9608E29D6E801B94B94_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		il2cpp_codegen_initobj((&V_0), sizeof(Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8 ));
		StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994 * L_0 = (&V_0)->get_address_of__entityProvider_0();
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_1 = ___p00;
		EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  L_2 = ___p11;
		StructuralChangeEntityProvider_PrepareToExecuteWithStructuralChanges_m869CCD0EEF00648F7E495A94C7309A61B02B5949((StructuralChangeEntityProvider_t2D7D9ADD6A3EDCDED056180F5A4D9D3D70D42994 *)L_0, L_1, L_2, /*hidden argument*/NULL);
		LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13 * L_3 = __this->get_address_of_forParameter_e_0();
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_4 = ___p00;
		EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  L_5 = ___p11;
		StructuralChangeRuntime_t71A0711C8BB9ED10F5E8DB42FDB7BD1EED5881CA  L_6 = LambdaParameterValueProvider_Entity_PrepareToExecuteWithStructuralChanges_m3D096D764543E8D1024D628FDE07EC81C10DF8E3((LambdaParameterValueProvider_Entity_t2058F0281B3B1F906E0077AEAB24FF52DFF8DA13 *)L_3, L_4, L_5, /*hidden argument*/NULL);
		(&V_0)->set_runtime_e_1(L_6);
		LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4 * L_7 = __this->get_address_of_forParameter_retain_1();
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_8 = ___p00;
		EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  L_9 = ___p11;
		StructuralChangeRuntime_t26DCD423015987103FDFB8CCC83AFD43AAC79B6B  L_10 = LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mD35E9088251B87C54D4C9FE9CE4B2C68F7661646((LambdaParameterValueProvider_IComponentData_1_tC68065510FDA500FB842AF78473919255425D7D4 *)L_7, L_8, L_9, /*hidden argument*/LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_mD35E9088251B87C54D4C9FE9CE4B2C68F7661646_RuntimeMethod_var);
		(&V_0)->set_runtime_retain_2(L_10);
		LambdaParameterValueProvider_IComponentData_1_t2EFFCFAFE87229878D536B5C44DC62239C46601A * L_11 = __this->get_address_of_forParameter_retainPtr_2();
		ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * L_12 = ___p00;
		EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  L_13 = ___p11;
		StructuralChangeRuntime_tB8BFA81C1D9AD3605B830A56686F111754A8B088  L_14 = LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_m2D6A743E5140666061CD5420D95250903AFED096((LambdaParameterValueProvider_IComponentData_1_t2EFFCFAFE87229878D536B5C44DC62239C46601A *)L_11, L_12, L_13, /*hidden argument*/LambdaParameterValueProvider_IComponentData_1_PrepareToExecuteWithStructuralChanges_m2D6A743E5140666061CD5420D95250903AFED096_RuntimeMethod_var);
		(&V_0)->set_runtime_retainPtr_3(L_14);
		Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8  L_15 = V_0;
		return L_15;
	}
}
IL2CPP_EXTERN_C  Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8  LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_m06AE7D081E5B1F77F983D9608E29D6E801B94B94_AdjustorThunk (RuntimeObject * __this, ComponentSystemBase_t8008ABC5BDE453061672EA262B5698047849A3BC * ___p00, EntityQuery_tDCA25A292AF939DB4C25632A20819FADAF418109  ___p11, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31 * _thisAdjusted = reinterpret_cast<LambdaParameterValueProviders_tF75461480A91D43DE1648B0666354EFE27690F31 *>(__this + _offset);
	return LambdaParameterValueProviders_PrepareToExecuteWithStructuralChanges_m06AE7D081E5B1F77F983D9608E29D6E801B94B94(_thisAdjusted, ___p00, ___p11, method);
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
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif


// Conversion methods for marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0/LambdaParameterValueProviders/Runtimes
IL2CPP_EXTERN_C void Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879_marshal_pinvoke(const Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879& unmarshaled, Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879_marshaled_pinvoke& marshaled)
{
	Exception_t* ___runtime_blobOwner_2Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'runtime_blobOwner' of type 'Runtimes'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___runtime_blobOwner_2Exception, NULL);
}
IL2CPP_EXTERN_C void Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879_marshal_pinvoke_back(const Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879_marshaled_pinvoke& marshaled, Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879& unmarshaled)
{
	Exception_t* ___runtime_blobOwner_2Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'runtime_blobOwner' of type 'Runtimes'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___runtime_blobOwner_2Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0/LambdaParameterValueProviders/Runtimes
IL2CPP_EXTERN_C void Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879_marshal_pinvoke_cleanup(Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879_marshaled_pinvoke& marshaled)
{
}


// Conversion methods for marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0/LambdaParameterValueProviders/Runtimes
IL2CPP_EXTERN_C void Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879_marshal_com(const Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879& unmarshaled, Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879_marshaled_com& marshaled)
{
	Exception_t* ___runtime_blobOwner_2Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'runtime_blobOwner' of type 'Runtimes'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___runtime_blobOwner_2Exception, NULL);
}
IL2CPP_EXTERN_C void Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879_marshal_com_back(const Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879_marshaled_com& marshaled, Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879& unmarshaled)
{
	Exception_t* ___runtime_blobOwner_2Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'runtime_blobOwner' of type 'Runtimes'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___runtime_blobOwner_2Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob0/LambdaParameterValueProviders/Runtimes
IL2CPP_EXTERN_C void Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879_marshal_com_cleanup(Runtimes_tA956A575E146E30E12C94ECBF3453CC491368879_marshaled_com& marshaled)
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


// Conversion methods for marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1/LambdaParameterValueProviders/Runtimes
IL2CPP_EXTERN_C void Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF_marshal_pinvoke(const Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF& unmarshaled, Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF_marshaled_pinvoke& marshaled)
{
	Exception_t* ___runtime_retain_2Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'runtime_retain' of type 'Runtimes'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___runtime_retain_2Exception, NULL);
}
IL2CPP_EXTERN_C void Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF_marshal_pinvoke_back(const Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF_marshaled_pinvoke& marshaled, Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF& unmarshaled)
{
	Exception_t* ___runtime_retain_2Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'runtime_retain' of type 'Runtimes'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___runtime_retain_2Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1/LambdaParameterValueProviders/Runtimes
IL2CPP_EXTERN_C void Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF_marshal_pinvoke_cleanup(Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF_marshaled_pinvoke& marshaled)
{
}


// Conversion methods for marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1/LambdaParameterValueProviders/Runtimes
IL2CPP_EXTERN_C void Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF_marshal_com(const Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF& unmarshaled, Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF_marshaled_com& marshaled)
{
	Exception_t* ___runtime_retain_2Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'runtime_retain' of type 'Runtimes'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___runtime_retain_2Exception, NULL);
}
IL2CPP_EXTERN_C void Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF_marshal_com_back(const Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF_marshaled_com& marshaled, Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF& unmarshaled)
{
	Exception_t* ___runtime_retain_2Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'runtime_retain' of type 'Runtimes'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___runtime_retain_2Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob1/LambdaParameterValueProviders/Runtimes
IL2CPP_EXTERN_C void Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF_marshal_com_cleanup(Runtimes_tEC4ECB104A241687AD80E04180F3E611AF5B5AEF_marshaled_com& marshaled)
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


// Conversion methods for marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders/Runtimes
IL2CPP_EXTERN_C void Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_marshal_pinvoke(const Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8& unmarshaled, Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_marshaled_pinvoke& marshaled)
{
	Exception_t* ___runtime_retain_2Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'runtime_retain' of type 'Runtimes'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___runtime_retain_2Exception, NULL);
}
IL2CPP_EXTERN_C void Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_marshal_pinvoke_back(const Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_marshaled_pinvoke& marshaled, Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8& unmarshaled)
{
	Exception_t* ___runtime_retain_2Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'runtime_retain' of type 'Runtimes'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___runtime_retain_2Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders/Runtimes
IL2CPP_EXTERN_C void Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_marshal_pinvoke_cleanup(Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_marshaled_pinvoke& marshaled)
{
}


// Conversion methods for marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders/Runtimes
IL2CPP_EXTERN_C void Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_marshal_com(const Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8& unmarshaled, Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_marshaled_com& marshaled)
{
	Exception_t* ___runtime_retain_2Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'runtime_retain' of type 'Runtimes'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___runtime_retain_2Exception, NULL);
}
IL2CPP_EXTERN_C void Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_marshal_com_back(const Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_marshaled_com& marshaled, Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8& unmarshaled)
{
	Exception_t* ___runtime_retain_2Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'runtime_retain' of type 'Runtimes'.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___runtime_retain_2Exception, NULL);
}
// Conversion method for clean up from marshalling of: Unity.Entities.RetainBlobAssetSystem/Unity.Entities.<>c__DisplayClass_OnUpdate_LambdaJob2/LambdaParameterValueProviders/Runtimes
IL2CPP_EXTERN_C void Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_marshal_com_cleanup(Runtimes_t7BF097608A5D2CFF88BAD1168C9F9F8A598389C8_marshaled_com& marshaled)
{
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_ScheduleTimeInitialize_m474B58EB99B27E8410150472E373A8B1CFB2E236_inline (U3CU3Ec__DisplayClass_OnUpdate_LambdaJob2_tA56437A63AF4B15DE4F44B34C979BED75F504F9E * __this, RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14 * ___componentSystem0, const RuntimeMethod* method)
{
	{
		RetainBlobAssetSystem_tACB22AB0481B74373B4542C63BDF55E3C5E0CA14 * L_0 = ___componentSystem0;
		__this->set_hostInstance_0(L_0);
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void BlobAssetPtr__ctor_mA9ADEAB1ED9BDEA24267B1396FFED6278159ADF9_inline (BlobAssetPtr_t46383DEAA6C0219CDA08453AE4262BF725D99447 * __this, BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * ___header0, const RuntimeMethod* method)
{
	{
		// this.header = header;
		BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F * L_0 = ___header0;
		__this->set_header_0((BlobAssetHeader_tF73E3BAE9394BE8A43C3FD58BBBC6B061136303F *)L_0);
		// }
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Type_t * CustomAttributeTypedArgument_get_ArgumentType_m87769FA596B93DC490F158996486CA1D42C4E84C_inline (CustomAttributeTypedArgument_tE7152E8FACDD29A8E0040E151C86F436FA8E6910 * __this, const RuntimeMethod* method)
{
	{
		Type_t * L_0 = __this->get_argumentType_0();
		return L_0;
	}
}
