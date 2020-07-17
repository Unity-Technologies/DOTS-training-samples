#include "pch-cpp.hpp"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <limits>
#include <stdint.h>


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
template <typename T1>
struct VirtActionInvoker1
{
	typedef void (*Action)(void*, T1, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, invokeData.method);
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
struct VirtActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
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
struct InterfaceActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
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

// System.ArgumentException
struct ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00;
// System.ArgumentNullException
struct ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB;
// System.AsyncCallback
struct AsyncCallback_tA7921BEF974919C46FF8F9D9867C567B200BB0EA;
// System.Reflection.Binder
struct Binder_t2BEE27FD84737D1E79BC47FD67F6D3DD2F2DDA30;
// System.DelegateData
struct DelegateData_t17DD30660E330C49381DAA99F934BE75CB11F288;
// System.IAsyncResult
struct IAsyncResult_tC9F97BF36FCF122D29D3101D80642278297BF370;
// System.Collections.IComparer
struct IComparer_t624EE667DCB0D3765FF034F7150DA71B361B82C0;
// System.Collections.IDictionary
struct IDictionary_t99871C56B8EC2452AC5C4CF3831695E617B89D3A;
// System.Collections.IEqualityComparer
struct IEqualityComparer_t6C4C1F04B21BDE1E4B84BD6EC7DE494C186D6C68;
// System.Runtime.Serialization.IFormatterConverter
struct IFormatterConverter_t2A667D8777429024D8A3CB3D9AE29EA79FEA6176;
// System.Reflection.MemberFilter
struct MemberFilter_t48D0AA10105D186AF42428FA532D4B4332CF8B81;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// System.Runtime.Serialization.SafeSerializationManager
struct SafeSerializationManager_tDE44F029589A028F8A3053C5C06153FAB4AAE29F;
// System.Runtime.Serialization.SerializationInfo
struct SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1;
// System.String
struct String_t;
// System.Type
struct Type_t;
// System.Void
struct Void_t700C6383A2A510C2CF4DD86DABD5CA9FF70ADAC5;
// UnityEngine.Rendering.VolumeParameter
struct VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB;
// System.Collections.Generic.Comparer`1<System.Object>
struct Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84;
// System.Collections.Generic.Comparer`1<UnityEngine.Vector4>
struct Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD;
// System.Collections.Generic.Dictionary`2<System.String,System.Int32>
struct Dictionary_2_tC94E9875910491F8130C2DC8B11E4D1548A55162;
// System.Collections.Generic.EqualityComparer`1<System.Boolean>
struct EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7;
// System.Collections.Generic.EqualityComparer`1<UnityEngine.Color>
struct EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5;
// System.Collections.Generic.EqualityComparer`1<System.Int32>
struct EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62;
// System.Collections.Generic.EqualityComparer`1<System.Int32Enum>
struct EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F;
// System.Collections.Generic.EqualityComparer`1<UnityEngine.LayerMask>
struct EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A;
// System.Collections.Generic.EqualityComparer`1<System.Object>
struct EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20;
// System.Collections.Generic.EqualityComparer`1<System.Single>
struct EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F;
// System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector2>
struct EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09;
// System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector3>
struct EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7;
// System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector4>
struct EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E;
// System.Func`2<UnityEngine.Color,System.Boolean>
struct Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD;
// System.Func`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>
struct Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64;
// System.Func`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>
struct Func_2_tA55660D7B36BC919063457215A12594F309CFDF1;
// System.Func`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,System.Boolean>
struct Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A;
// System.Func`2<Unity.Entities.Entity,System.Boolean>
struct Func_2_t14BB53D120BF18F218ACE746215828AC2863F843;
// System.Func`2<Unity.Entities.Entity,System.Object>
struct Func_2_t895537CD65D26801427B03E05DD08125DE819919;
// System.Func`2<System.Int32,System.Int32>
struct Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA;
// System.Func`2<System.Int32,System.Boolean>
struct Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274;
// System.Func`2<System.Int32,System.Object>
struct Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123;
// System.Func`2<Unity.Entities.Conversion.LogEventData,System.Boolean>
struct Func_2_t15BD356B2F637699370FD7109071A37617770BBA;
// System.Func`2<Unity.Entities.Conversion.LogEventData,System.Object>
struct Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154;
// System.Func`2<UnityEngine.Vector3,System.Boolean>
struct Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269;
// System.Func`2<System.Object,System.Boolean>
struct Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8;
// System.Func`2<System.Object,System.Object>
struct Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436;
// System.Collections.Generic.IEnumerable`1<UnityEngine.Color>
struct IEnumerable_1_tABC441119E42D460CA5B9DED9C1D1A2BD8C836DD;
// System.Collections.Generic.IEnumerable`1<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex>
struct IEnumerable_1_tF316739C8BD63390C6735A523D11111CDB165479;
// System.Collections.Generic.IEnumerable`1<Unity.Entities.Entity>
struct IEnumerable_1_tF3EA67577F375F81F4E291BF5DEFCCAEAAC3D194;
// System.Collections.Generic.IEnumerable`1<System.Int32>
struct IEnumerable_1_t60929E1AA80B46746F987B99A4EBD004FD72D370;
// System.Collections.Generic.IEnumerable`1<Unity.Entities.Conversion.LogEventData>
struct IEnumerable_1_t582F46B3A2404D2FED50020E24A96F07A29CE921;
// System.Collections.Generic.IEnumerable`1<UnityEngine.Vector3>
struct IEnumerable_1_tDBC849B8248C833C53F1762E771EFC477EB8AF18;
// System.Collections.Generic.IEnumerable`1<System.Object>
struct IEnumerable_1_t52B1AC8D9E5E1ED28DF6C46A37C9A1B00B394F9D;
// System.Collections.Generic.IEnumerator`1<UnityEngine.Color>
struct IEnumerator_1_t02C24C8A9AB2516412261243B17D8239575F3E86;
// System.Collections.Generic.IEnumerator`1<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex>
struct IEnumerator_1_t78FAD0C53FCC05439B0133A0FC24595AD24EC918;
// System.Collections.Generic.IEnumerator`1<Unity.Entities.Entity>
struct IEnumerator_1_tF93E1FE005A8A2F20597563D53484A682C53143C;
// System.Collections.Generic.IEnumerator`1<System.Int32>
struct IEnumerator_1_t72AB4B40AF5290B386215B0BFADC8919D394DCAB;
// System.Collections.Generic.IEnumerator`1<Unity.Entities.Conversion.LogEventData>
struct IEnumerator_1_t410AD505F5B0B9E5A9C82C50C87D3589DC982676;
// System.Collections.Generic.IEnumerator`1<UnityEngine.Vector3>
struct IEnumerator_1_t9C426231952B863270D78D88F9DB5B4E9A16CC6A;
// System.Collections.Generic.IEnumerator`1<System.Object>
struct IEnumerator_1_t2DC97C7D486BF9E077C2BC2E517E434F393AA76E;
// System.Linq.Enumerable/Iterator`1<UnityEngine.Color>
struct Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76;
// System.Linq.Enumerable/Iterator`1<System.Int32>
struct Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379;
// System.Linq.Enumerable/Iterator`1<UnityEngine.Vector3>
struct Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF;
// System.Linq.Enumerable/Iterator`1<System.Object>
struct Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279;
// System.Collections.Generic.List`1<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex>
struct List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C;
// System.Collections.Generic.List`1<Unity.Entities.Entity>
struct List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF;
// System.Collections.Generic.List`1<System.Int32>
struct List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7;
// System.Collections.Generic.List`1<Unity.Entities.Conversion.LogEventData>
struct List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45;
// System.Collections.Generic.List`1<System.Object>
struct List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5;
// UnityEngine.Rendering.VolumeParameter`1<System.Boolean>
struct VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201;
// UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>
struct VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A;
// UnityEngine.Rendering.VolumeParameter`1<System.Int32>
struct VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7;
// UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>
struct VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740;
// UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>
struct VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D;
// UnityEngine.Rendering.VolumeParameter`1<System.Single>
struct VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7;
// UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>
struct VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7;
// UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>
struct VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F;
// UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>
struct VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C;
// UnityEngine.Rendering.VolumeParameter`1<System.Object>
struct VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0;
// System.WeakReference`1<System.Object>
struct WeakReference_1_t223E87F227F99044D112FF1E7FC7DA135365CB76;
// System.Linq.Enumerable/WhereArrayIterator`1<System.Object>
struct WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86;
// System.Linq.Enumerable/WhereEnumerableIterator`1<UnityEngine.Color>
struct WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2;
// System.Linq.Enumerable/WhereEnumerableIterator`1<System.Int32>
struct WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA;
// System.Linq.Enumerable/WhereEnumerableIterator`1<UnityEngine.Vector3>
struct WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8;
// System.Linq.Enumerable/WhereEnumerableIterator`1<System.Object>
struct WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0;
// System.Linq.Enumerable/WhereListIterator`1<System.Object>
struct WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD;
// System.Linq.Enumerable/WhereSelectArrayIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>
struct WhereSelectArrayIterator_2_tCB97637D799FCAD3115615E9FDA8E5CF80A108EC;
// System.Linq.Enumerable/WhereSelectArrayIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>
struct WhereSelectArrayIterator_2_t08DE4878AA443563B5940613DEBED682F6942F28;
// System.Linq.Enumerable/WhereSelectArrayIterator`2<Unity.Entities.Entity,System.Object>
struct WhereSelectArrayIterator_2_t96F399D19669EB095BE4F3F6DFFF0695945DF634;
// System.Linq.Enumerable/WhereSelectArrayIterator`2<System.Int32,System.Int32>
struct WhereSelectArrayIterator_2_t00D4CC16A86C24F0CC80D3112E1FA2D451FF5D0C;
// System.Linq.Enumerable/WhereSelectArrayIterator`2<System.Int32,System.Object>
struct WhereSelectArrayIterator_2_t6AF7279540EEA250525A2AFBDFAC2064A9B5C00B;
// System.Linq.Enumerable/WhereSelectArrayIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>
struct WhereSelectArrayIterator_2_tFC05154F4D80444AACE36E75937E5D42B87B3384;
// System.Linq.Enumerable/WhereSelectArrayIterator`2<System.Object,System.Object>
struct WhereSelectArrayIterator_2_tA706D5B1608A9A8F1BF43C6E5D9D682C901DB244;
// System.Linq.Enumerable/WhereSelectEnumerableIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>
struct WhereSelectEnumerableIterator_2_t082E1B5B13EA24679459FB2484F9A187BB83D681;
// System.Linq.Enumerable/WhereSelectEnumerableIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>
struct WhereSelectEnumerableIterator_2_t3B7C9E548F0B01F9C6D09D71FCA8E5ACDE0F3FCD;
// System.Linq.Enumerable/WhereSelectEnumerableIterator`2<Unity.Entities.Entity,System.Object>
struct WhereSelectEnumerableIterator_2_t51ACFE0E1ABAB81E89F3A922089D2AC4ACED9718;
// System.Linq.Enumerable/WhereSelectEnumerableIterator`2<System.Int32,System.Int32>
struct WhereSelectEnumerableIterator_2_t70C236BD8C0DBBA43FCE3CE4E4F6B545579333EF;
// System.Linq.Enumerable/WhereSelectEnumerableIterator`2<System.Int32,System.Object>
struct WhereSelectEnumerableIterator_2_tC12C4897F0443B7CCF4B7D0E720F91F10278FC0E;
// System.Linq.Enumerable/WhereSelectEnumerableIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>
struct WhereSelectEnumerableIterator_2_t575305DD8907FFFDC568246A6635414D5B81B807;
// System.Linq.Enumerable/WhereSelectEnumerableIterator`2<System.Object,System.Object>
struct WhereSelectEnumerableIterator_2_tDAA8BFBEA68F81670F3F51C6200EBD26D7A8FBAB;
// System.Linq.Enumerable/WhereSelectListIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>
struct WhereSelectListIterator_2_t08B768B4AD54F201B800C51471E1C1C5FD1A8E8C;
// System.Linq.Enumerable/WhereSelectListIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>
struct WhereSelectListIterator_2_t94F415FEA624816E42FAE1274A4EC29F5D5C1625;
// System.Linq.Enumerable/WhereSelectListIterator`2<Unity.Entities.Entity,System.Object>
struct WhereSelectListIterator_2_t9800F9AD3EEADD8B7707A9F9E67DFC4C12E2F924;
// System.Linq.Enumerable/WhereSelectListIterator`2<System.Int32,System.Int32>
struct WhereSelectListIterator_2_t4CC3FE3A35610DC6F761EE7DB863B845957AD325;
// System.Linq.Enumerable/WhereSelectListIterator`2<System.Int32,System.Object>
struct WhereSelectListIterator_2_tA41D93FF12E41BB5A5BEA27AEED367695ADACEA4;
// System.Linq.Enumerable/WhereSelectListIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>
struct WhereSelectListIterator_2_tCD2B0DD2D1D32234F3640C8D7B5C9C212AAAD6E4;
// System.Linq.Enumerable/WhereSelectListIterator`2<System.Object,System.Object>
struct WhereSelectListIterator_2_t85B78DFF0573BC95A62C79D6088FA39DFEBE1AF2;
// System.Char[]
struct CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34;
// UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex[]
struct ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36;
// System.Delegate[]
struct DelegateU5BU5D_t677D8FE08A5F99E8EE49150B73966CD6E9BF7DB8;
// Unity.Entities.Entity[]
struct EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5;
// System.Int32[]
struct Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32;
// System.IntPtr[]
struct IntPtrU5BU5D_t27FC72B0409D75AAF33EC42498E8094E95FEE9A6;
// Unity.Entities.Conversion.LogEventData[]
struct LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC;
// System.Object[]
struct ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE;
// System.Diagnostics.StackTrace[]
struct StackTraceU5BU5D_t4AD999C288CB6D1F38A299D12B1598D606588971;
// System.String[]
struct StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A;
// System.Type[]
struct TypeU5BU5D_t85B10489E46F06CEC7C4B1CCBD0E01FAB6649755;

IL2CPP_EXTERN_C RuntimeClass* ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IComparer_t624EE667DCB0D3765FF034F7150DA71B361B82C0_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IDisposable_t099785737FC6A1E3699919A94109383715A8D807_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEnumerator_t5956F3AFB7ECF1117E3BC5890E7FC7B7F7A04105_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEqualityComparer_t6C4C1F04B21BDE1E4B84BD6EC7DE494C186D6C68_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Type_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral1459AD7D3E0F8808A85528961118835E18AD1F96;
IL2CPP_EXTERN_C String_t* _stringLiteral5CA6E7C0AE72196B2817D93A78C719652EC691C0;
IL2CPP_EXTERN_C String_t* _stringLiteral758733BDBED83CBFF4F635AC26CA92AAE477F75D;
IL2CPP_EXTERN_C String_t* _stringLiteral7D20B8219CA0491872B2E811B262066A5DD875A7;
IL2CPP_EXTERN_C String_t* _stringLiteralA3DFC0C77ACADE0EE48DCC73E795A597D0270A73;
IL2CPP_EXTERN_C String_t* _stringLiteralA5E215A6DBE803E908043576B18C4FAD26AD44F7;
IL2CPP_EXTERN_C String_t* _stringLiteralA7B00F7F25C375B2501A6ADBC86D092B23977085;
IL2CPP_EXTERN_C String_t* _stringLiteralB3F14BF976EFD974E34846B742502C802FABAE9D;
IL2CPP_EXTERN_C String_t* _stringLiteralF7933083B6BA56CBC6D7BCA0F30688A30D0368F6;
IL2CPP_EXTERN_C const RuntimeMethod* ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m7233DE35FC3952A4832494EDCA1F3F2681AE09FE_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m8F9A1A82E5960A5A167B7B61237919AC9EA86869_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ValueTuple_3_System_IComparable_CompareTo_m565E54913F6D87E2DE47D69286058547E5C28155_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ValueTuple_3_System_IComparable_CompareTo_mDCBC6504198CCF4FD5A11911B785F625602F0A5A_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* WeakReference_1_GetObjectData_m7C63330FAC22CBE86AA1BDE2F34DFDA8B1E41272_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* WeakReference_1__ctor_mC76935DFFEF0678A77A4811865B9F4D350D72741_RuntimeMethod_var;
IL2CPP_EXTERN_C const uint32_t ValueTuple_3_GetHashCodeCore_m67CB7531D334ACBE07696D3CFC5DB57C4738F3C4_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ValueTuple_3_GetHashCodeCore_m6D7023B397C18B2B04D077227DDC88F0E0282731_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m7233DE35FC3952A4832494EDCA1F3F2681AE09FE_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m8F9A1A82E5960A5A167B7B61237919AC9EA86869_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m2B4EB2E18EAA9D66C46192CAFA91BD15AAAEF78E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m8D1BD92383DC97A390119BEE2ABAA659470A3252_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ValueTuple_3_System_IComparable_CompareTo_m565E54913F6D87E2DE47D69286058547E5C28155_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ValueTuple_3_System_IComparable_CompareTo_mDCBC6504198CCF4FD5A11911B785F625602F0A5A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ValueTuple_3_ToString_m2F574559A50EDFB3E99D4CCD1B85A772BB84798F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ValueTuple_3_ToString_mE918A7F63C8F4AD1C4BCF5C2F28D048307DBB1BF_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_Equals_m280BFF61F817FEAC2CBBD65A638562916BF3F692_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_Equals_m28EA57D1533109CDEFFC5AABE734AE6C14FA0851_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_Equals_m43533DA3167983D9D59102963A9256B1B32E87F3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_Equals_m7239C7067F96C0F6B9AE4886B308C3184819F259_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_Equals_m7D4639EC08F8D19DA960892524BA475332EB6E6A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_Equals_m831EC889B40C1B3D9763C7460C62A2C2986AFD5D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_Equals_m87E3D7F71B21A30E632D727C0B0F2CC1234AA260_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_Equals_m8ED0747D259C00F3410EFFAEFD9E26465CF42B65_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_Equals_m8FCF4CD51C849EC469EDDDEA30F81B3C0CC0A69F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_Equals_m9016DE26A9F96EDE9E8AC043EE8D06AFF4B8486B_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_ToString_m5B3102A3866568EDE9F2D5CD5092E9D0E244C3DA_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_ToString_m5DB1BD0BE00D6CBA3B55CE448643B9B519EB23A5_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_ToString_m70BB531EC1A1197D6B56D5B16D3DAB51D2753CBB_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_ToString_m846B3E448C84AEA3E560FF6ECCE12A0EE8E98862_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_ToString_m98DEF918D3848ED5AF582300AA707E816743AF66_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_ToString_m9C2B50A9346DD0AB59B08833586ECBCFB9519661_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_ToString_mE9E79D94F86F79CE0B3D50E47132499AF12ED674_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_ToString_mEF69CB91E8EFCB0C2253CA84E62B45374E69973E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_ToString_mF22D6B5BA00E0341023E67CE6DE6F2CEF402846D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VolumeParameter_1_ToString_mF6E839F3AA47EB989463C8B204124DC50009CFF6_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WeakReference_1_GetObjectData_m7C63330FAC22CBE86AA1BDE2F34DFDA8B1E41272_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WeakReference_1__ctor_mC76935DFFEF0678A77A4811865B9F4D350D72741_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereEnumerableIterator_1_Dispose_m4E1339513102BB6B49AD33EDB569D3FFD24ED023_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereEnumerableIterator_1_Dispose_m64C95774F2781EB65AC8FE5A6CD5043BF61CEED8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereEnumerableIterator_1_Dispose_mB841131399B8BA11B9D6DB37E11F90B1BFFBDA2D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereEnumerableIterator_1_Dispose_mCEF7D940B21C37FAC2966547F8428D8BE4C59338_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereEnumerableIterator_1_MoveNext_m1ABB7BE9CE39660B4C3E276C11F7757E9DDC4BD0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereEnumerableIterator_1_MoveNext_m3D143965FBF0E34F74297CA37DF3B3184262857A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereEnumerableIterator_1_MoveNext_m6D8A420AEB325BF252721010781EF31CF64D73FF_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereEnumerableIterator_1_MoveNext_m819D14CC69CC48B0B84E497DFF1953AAFFF13333_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereSelectEnumerableIterator_2_Dispose_m1F32BB3D970382EE067CF21C4DA2C95EBB1AA3BE_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereSelectEnumerableIterator_2_Dispose_m76E5F3DD7C637EC24BC0B6962F3852B5E4C9DCBB_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereSelectEnumerableIterator_2_Dispose_m7DDE555349A7D0B9DEFB7EB866B728A7D40E4BD9_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereSelectEnumerableIterator_2_Dispose_m82C0D4A9E151A1DAC0C017CA403BAB5CDED9CFD5_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereSelectEnumerableIterator_2_Dispose_mAA70577DEF67CEC98FE677984AE2175B7D4E4D00_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereSelectEnumerableIterator_2_Dispose_mB529F42305C163995535C0696422D8410CB02BB3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereSelectEnumerableIterator_2_Dispose_mB8D564C5BE6B9B60555B5D5A980D8F0FFBA8EE96_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereSelectEnumerableIterator_2_MoveNext_m4C5AF35515D4FD9CB3C05F9229032D97752B6EE3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereSelectEnumerableIterator_2_MoveNext_m6214F2D8E14596AF5302702554D6090B95C91977_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereSelectEnumerableIterator_2_MoveNext_m7422C90C467A2D0EF7E7D644EDB241F378A7AECD_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereSelectEnumerableIterator_2_MoveNext_m95AEE737A22EFFFE6557F448BF5AFCC6241D0BD7_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereSelectEnumerableIterator_2_MoveNext_mC11B40068E87BD32531EC0895238F0EF85DDB398_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereSelectEnumerableIterator_2_MoveNext_mE31162ED57F8EA0F26BCF79C89FC7D074BAED8E8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WhereSelectEnumerableIterator_2_MoveNext_mF775895EE064779EC9A579DA627D0ADE792BF57E_MetadataUsageId;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;

struct Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32;
struct ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE;
struct StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A;
struct LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC;
struct EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5;
struct ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36;

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


// System.Collections.Generic.Comparer`1<System.Object>
struct  Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84  : public RuntimeObject
{
public:

public:
};

struct Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84_StaticFields
{
public:
	// System.Collections.Generic.Comparer`1<T> modreq(System.Runtime.CompilerServices.IsVolatile) System.Collections.Generic.Comparer`1::defaultComparer
	Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 * ___defaultComparer_0;

public:
	inline static int32_t get_offset_of_defaultComparer_0() { return static_cast<int32_t>(offsetof(Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84_StaticFields, ___defaultComparer_0)); }
	inline Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 * get_defaultComparer_0() const { return ___defaultComparer_0; }
	inline Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 ** get_address_of_defaultComparer_0() { return &___defaultComparer_0; }
	inline void set_defaultComparer_0(Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 * value)
	{
		___defaultComparer_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultComparer_0), (void*)value);
	}
};


// System.Collections.Generic.Comparer`1<UnityEngine.Vector4>
struct  Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD  : public RuntimeObject
{
public:

public:
};

struct Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD_StaticFields
{
public:
	// System.Collections.Generic.Comparer`1<T> modreq(System.Runtime.CompilerServices.IsVolatile) System.Collections.Generic.Comparer`1::defaultComparer
	Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD * ___defaultComparer_0;

public:
	inline static int32_t get_offset_of_defaultComparer_0() { return static_cast<int32_t>(offsetof(Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD_StaticFields, ___defaultComparer_0)); }
	inline Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD * get_defaultComparer_0() const { return ___defaultComparer_0; }
	inline Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD ** get_address_of_defaultComparer_0() { return &___defaultComparer_0; }
	inline void set_defaultComparer_0(Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD * value)
	{
		___defaultComparer_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultComparer_0), (void*)value);
	}
};


// System.Collections.Generic.EqualityComparer`1<System.Boolean>
struct  EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7  : public RuntimeObject
{
public:

public:
};

struct EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7_StaticFields
{
public:
	// System.Collections.Generic.EqualityComparer`1<T> modreq(System.Runtime.CompilerServices.IsVolatile) System.Collections.Generic.EqualityComparer`1::defaultComparer
	EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7 * ___defaultComparer_0;

public:
	inline static int32_t get_offset_of_defaultComparer_0() { return static_cast<int32_t>(offsetof(EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7_StaticFields, ___defaultComparer_0)); }
	inline EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7 * get_defaultComparer_0() const { return ___defaultComparer_0; }
	inline EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7 ** get_address_of_defaultComparer_0() { return &___defaultComparer_0; }
	inline void set_defaultComparer_0(EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7 * value)
	{
		___defaultComparer_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultComparer_0), (void*)value);
	}
};


// System.Collections.Generic.EqualityComparer`1<System.Int32>
struct  EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62  : public RuntimeObject
{
public:

public:
};

struct EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62_StaticFields
{
public:
	// System.Collections.Generic.EqualityComparer`1<T> modreq(System.Runtime.CompilerServices.IsVolatile) System.Collections.Generic.EqualityComparer`1::defaultComparer
	EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62 * ___defaultComparer_0;

public:
	inline static int32_t get_offset_of_defaultComparer_0() { return static_cast<int32_t>(offsetof(EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62_StaticFields, ___defaultComparer_0)); }
	inline EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62 * get_defaultComparer_0() const { return ___defaultComparer_0; }
	inline EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62 ** get_address_of_defaultComparer_0() { return &___defaultComparer_0; }
	inline void set_defaultComparer_0(EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62 * value)
	{
		___defaultComparer_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultComparer_0), (void*)value);
	}
};


// System.Collections.Generic.EqualityComparer`1<System.Int32Enum>
struct  EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F  : public RuntimeObject
{
public:

public:
};

struct EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F_StaticFields
{
public:
	// System.Collections.Generic.EqualityComparer`1<T> modreq(System.Runtime.CompilerServices.IsVolatile) System.Collections.Generic.EqualityComparer`1::defaultComparer
	EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F * ___defaultComparer_0;

public:
	inline static int32_t get_offset_of_defaultComparer_0() { return static_cast<int32_t>(offsetof(EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F_StaticFields, ___defaultComparer_0)); }
	inline EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F * get_defaultComparer_0() const { return ___defaultComparer_0; }
	inline EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F ** get_address_of_defaultComparer_0() { return &___defaultComparer_0; }
	inline void set_defaultComparer_0(EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F * value)
	{
		___defaultComparer_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultComparer_0), (void*)value);
	}
};


// System.Collections.Generic.EqualityComparer`1<System.Object>
struct  EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20  : public RuntimeObject
{
public:

public:
};

struct EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20_StaticFields
{
public:
	// System.Collections.Generic.EqualityComparer`1<T> modreq(System.Runtime.CompilerServices.IsVolatile) System.Collections.Generic.EqualityComparer`1::defaultComparer
	EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 * ___defaultComparer_0;

public:
	inline static int32_t get_offset_of_defaultComparer_0() { return static_cast<int32_t>(offsetof(EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20_StaticFields, ___defaultComparer_0)); }
	inline EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 * get_defaultComparer_0() const { return ___defaultComparer_0; }
	inline EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 ** get_address_of_defaultComparer_0() { return &___defaultComparer_0; }
	inline void set_defaultComparer_0(EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 * value)
	{
		___defaultComparer_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultComparer_0), (void*)value);
	}
};


// System.Collections.Generic.EqualityComparer`1<System.Single>
struct  EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F  : public RuntimeObject
{
public:

public:
};

struct EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F_StaticFields
{
public:
	// System.Collections.Generic.EqualityComparer`1<T> modreq(System.Runtime.CompilerServices.IsVolatile) System.Collections.Generic.EqualityComparer`1::defaultComparer
	EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F * ___defaultComparer_0;

public:
	inline static int32_t get_offset_of_defaultComparer_0() { return static_cast<int32_t>(offsetof(EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F_StaticFields, ___defaultComparer_0)); }
	inline EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F * get_defaultComparer_0() const { return ___defaultComparer_0; }
	inline EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F ** get_address_of_defaultComparer_0() { return &___defaultComparer_0; }
	inline void set_defaultComparer_0(EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F * value)
	{
		___defaultComparer_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultComparer_0), (void*)value);
	}
};


// System.Collections.Generic.EqualityComparer`1<UnityEngine.Color>
struct  EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5  : public RuntimeObject
{
public:

public:
};

struct EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5_StaticFields
{
public:
	// System.Collections.Generic.EqualityComparer`1<T> modreq(System.Runtime.CompilerServices.IsVolatile) System.Collections.Generic.EqualityComparer`1::defaultComparer
	EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5 * ___defaultComparer_0;

public:
	inline static int32_t get_offset_of_defaultComparer_0() { return static_cast<int32_t>(offsetof(EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5_StaticFields, ___defaultComparer_0)); }
	inline EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5 * get_defaultComparer_0() const { return ___defaultComparer_0; }
	inline EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5 ** get_address_of_defaultComparer_0() { return &___defaultComparer_0; }
	inline void set_defaultComparer_0(EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5 * value)
	{
		___defaultComparer_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultComparer_0), (void*)value);
	}
};


// System.Collections.Generic.EqualityComparer`1<UnityEngine.LayerMask>
struct  EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A  : public RuntimeObject
{
public:

public:
};

struct EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A_StaticFields
{
public:
	// System.Collections.Generic.EqualityComparer`1<T> modreq(System.Runtime.CompilerServices.IsVolatile) System.Collections.Generic.EqualityComparer`1::defaultComparer
	EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A * ___defaultComparer_0;

public:
	inline static int32_t get_offset_of_defaultComparer_0() { return static_cast<int32_t>(offsetof(EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A_StaticFields, ___defaultComparer_0)); }
	inline EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A * get_defaultComparer_0() const { return ___defaultComparer_0; }
	inline EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A ** get_address_of_defaultComparer_0() { return &___defaultComparer_0; }
	inline void set_defaultComparer_0(EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A * value)
	{
		___defaultComparer_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultComparer_0), (void*)value);
	}
};


// System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector2>
struct  EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09  : public RuntimeObject
{
public:

public:
};

struct EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09_StaticFields
{
public:
	// System.Collections.Generic.EqualityComparer`1<T> modreq(System.Runtime.CompilerServices.IsVolatile) System.Collections.Generic.EqualityComparer`1::defaultComparer
	EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09 * ___defaultComparer_0;

public:
	inline static int32_t get_offset_of_defaultComparer_0() { return static_cast<int32_t>(offsetof(EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09_StaticFields, ___defaultComparer_0)); }
	inline EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09 * get_defaultComparer_0() const { return ___defaultComparer_0; }
	inline EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09 ** get_address_of_defaultComparer_0() { return &___defaultComparer_0; }
	inline void set_defaultComparer_0(EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09 * value)
	{
		___defaultComparer_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultComparer_0), (void*)value);
	}
};


// System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector3>
struct  EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7  : public RuntimeObject
{
public:

public:
};

struct EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7_StaticFields
{
public:
	// System.Collections.Generic.EqualityComparer`1<T> modreq(System.Runtime.CompilerServices.IsVolatile) System.Collections.Generic.EqualityComparer`1::defaultComparer
	EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7 * ___defaultComparer_0;

public:
	inline static int32_t get_offset_of_defaultComparer_0() { return static_cast<int32_t>(offsetof(EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7_StaticFields, ___defaultComparer_0)); }
	inline EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7 * get_defaultComparer_0() const { return ___defaultComparer_0; }
	inline EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7 ** get_address_of_defaultComparer_0() { return &___defaultComparer_0; }
	inline void set_defaultComparer_0(EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7 * value)
	{
		___defaultComparer_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultComparer_0), (void*)value);
	}
};


// System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector4>
struct  EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E  : public RuntimeObject
{
public:

public:
};

struct EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E_StaticFields
{
public:
	// System.Collections.Generic.EqualityComparer`1<T> modreq(System.Runtime.CompilerServices.IsVolatile) System.Collections.Generic.EqualityComparer`1::defaultComparer
	EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E * ___defaultComparer_0;

public:
	inline static int32_t get_offset_of_defaultComparer_0() { return static_cast<int32_t>(offsetof(EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E_StaticFields, ___defaultComparer_0)); }
	inline EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E * get_defaultComparer_0() const { return ___defaultComparer_0; }
	inline EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E ** get_address_of_defaultComparer_0() { return &___defaultComparer_0; }
	inline void set_defaultComparer_0(EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E * value)
	{
		___defaultComparer_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultComparer_0), (void*)value);
	}
};


// System.Collections.Generic.List`1<System.Int32>
struct  List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7  : public RuntimeObject
{
public:
	// T[] System.Collections.Generic.List`1::_items
	Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* ____items_1;
	// System.Int32 System.Collections.Generic.List`1::_size
	int32_t ____size_2;
	// System.Int32 System.Collections.Generic.List`1::_version
	int32_t ____version_3;
	// System.Object System.Collections.Generic.List`1::_syncRoot
	RuntimeObject * ____syncRoot_4;

public:
	inline static int32_t get_offset_of__items_1() { return static_cast<int32_t>(offsetof(List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7, ____items_1)); }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* get__items_1() const { return ____items_1; }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32** get_address_of__items_1() { return &____items_1; }
	inline void set__items_1(Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* value)
	{
		____items_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____items_1), (void*)value);
	}

	inline static int32_t get_offset_of__size_2() { return static_cast<int32_t>(offsetof(List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7, ____size_2)); }
	inline int32_t get__size_2() const { return ____size_2; }
	inline int32_t* get_address_of__size_2() { return &____size_2; }
	inline void set__size_2(int32_t value)
	{
		____size_2 = value;
	}

	inline static int32_t get_offset_of__version_3() { return static_cast<int32_t>(offsetof(List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7, ____version_3)); }
	inline int32_t get__version_3() const { return ____version_3; }
	inline int32_t* get_address_of__version_3() { return &____version_3; }
	inline void set__version_3(int32_t value)
	{
		____version_3 = value;
	}

	inline static int32_t get_offset_of__syncRoot_4() { return static_cast<int32_t>(offsetof(List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7, ____syncRoot_4)); }
	inline RuntimeObject * get__syncRoot_4() const { return ____syncRoot_4; }
	inline RuntimeObject ** get_address_of__syncRoot_4() { return &____syncRoot_4; }
	inline void set__syncRoot_4(RuntimeObject * value)
	{
		____syncRoot_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_4), (void*)value);
	}
};

struct List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7_StaticFields
{
public:
	// T[] System.Collections.Generic.List`1::_emptyArray
	Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* ____emptyArray_5;

public:
	inline static int32_t get_offset_of__emptyArray_5() { return static_cast<int32_t>(offsetof(List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7_StaticFields, ____emptyArray_5)); }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* get__emptyArray_5() const { return ____emptyArray_5; }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32** get_address_of__emptyArray_5() { return &____emptyArray_5; }
	inline void set__emptyArray_5(Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* value)
	{
		____emptyArray_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____emptyArray_5), (void*)value);
	}
};


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


// System.Collections.Generic.List`1<Unity.Entities.Conversion.LogEventData>
struct  List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45  : public RuntimeObject
{
public:
	// T[] System.Collections.Generic.List`1::_items
	LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC* ____items_1;
	// System.Int32 System.Collections.Generic.List`1::_size
	int32_t ____size_2;
	// System.Int32 System.Collections.Generic.List`1::_version
	int32_t ____version_3;
	// System.Object System.Collections.Generic.List`1::_syncRoot
	RuntimeObject * ____syncRoot_4;

public:
	inline static int32_t get_offset_of__items_1() { return static_cast<int32_t>(offsetof(List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45, ____items_1)); }
	inline LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC* get__items_1() const { return ____items_1; }
	inline LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC** get_address_of__items_1() { return &____items_1; }
	inline void set__items_1(LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC* value)
	{
		____items_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____items_1), (void*)value);
	}

	inline static int32_t get_offset_of__size_2() { return static_cast<int32_t>(offsetof(List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45, ____size_2)); }
	inline int32_t get__size_2() const { return ____size_2; }
	inline int32_t* get_address_of__size_2() { return &____size_2; }
	inline void set__size_2(int32_t value)
	{
		____size_2 = value;
	}

	inline static int32_t get_offset_of__version_3() { return static_cast<int32_t>(offsetof(List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45, ____version_3)); }
	inline int32_t get__version_3() const { return ____version_3; }
	inline int32_t* get_address_of__version_3() { return &____version_3; }
	inline void set__version_3(int32_t value)
	{
		____version_3 = value;
	}

	inline static int32_t get_offset_of__syncRoot_4() { return static_cast<int32_t>(offsetof(List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45, ____syncRoot_4)); }
	inline RuntimeObject * get__syncRoot_4() const { return ____syncRoot_4; }
	inline RuntimeObject ** get_address_of__syncRoot_4() { return &____syncRoot_4; }
	inline void set__syncRoot_4(RuntimeObject * value)
	{
		____syncRoot_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_4), (void*)value);
	}
};

struct List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45_StaticFields
{
public:
	// T[] System.Collections.Generic.List`1::_emptyArray
	LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC* ____emptyArray_5;

public:
	inline static int32_t get_offset_of__emptyArray_5() { return static_cast<int32_t>(offsetof(List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45_StaticFields, ____emptyArray_5)); }
	inline LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC* get__emptyArray_5() const { return ____emptyArray_5; }
	inline LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC** get_address_of__emptyArray_5() { return &____emptyArray_5; }
	inline void set__emptyArray_5(LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC* value)
	{
		____emptyArray_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____emptyArray_5), (void*)value);
	}
};


// System.Collections.Generic.List`1<Unity.Entities.Entity>
struct  List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF  : public RuntimeObject
{
public:
	// T[] System.Collections.Generic.List`1::_items
	EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5* ____items_1;
	// System.Int32 System.Collections.Generic.List`1::_size
	int32_t ____size_2;
	// System.Int32 System.Collections.Generic.List`1::_version
	int32_t ____version_3;
	// System.Object System.Collections.Generic.List`1::_syncRoot
	RuntimeObject * ____syncRoot_4;

public:
	inline static int32_t get_offset_of__items_1() { return static_cast<int32_t>(offsetof(List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF, ____items_1)); }
	inline EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5* get__items_1() const { return ____items_1; }
	inline EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5** get_address_of__items_1() { return &____items_1; }
	inline void set__items_1(EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5* value)
	{
		____items_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____items_1), (void*)value);
	}

	inline static int32_t get_offset_of__size_2() { return static_cast<int32_t>(offsetof(List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF, ____size_2)); }
	inline int32_t get__size_2() const { return ____size_2; }
	inline int32_t* get_address_of__size_2() { return &____size_2; }
	inline void set__size_2(int32_t value)
	{
		____size_2 = value;
	}

	inline static int32_t get_offset_of__version_3() { return static_cast<int32_t>(offsetof(List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF, ____version_3)); }
	inline int32_t get__version_3() const { return ____version_3; }
	inline int32_t* get_address_of__version_3() { return &____version_3; }
	inline void set__version_3(int32_t value)
	{
		____version_3 = value;
	}

	inline static int32_t get_offset_of__syncRoot_4() { return static_cast<int32_t>(offsetof(List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF, ____syncRoot_4)); }
	inline RuntimeObject * get__syncRoot_4() const { return ____syncRoot_4; }
	inline RuntimeObject ** get_address_of__syncRoot_4() { return &____syncRoot_4; }
	inline void set__syncRoot_4(RuntimeObject * value)
	{
		____syncRoot_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_4), (void*)value);
	}
};

struct List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF_StaticFields
{
public:
	// T[] System.Collections.Generic.List`1::_emptyArray
	EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5* ____emptyArray_5;

public:
	inline static int32_t get_offset_of__emptyArray_5() { return static_cast<int32_t>(offsetof(List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF_StaticFields, ____emptyArray_5)); }
	inline EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5* get__emptyArray_5() const { return ____emptyArray_5; }
	inline EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5** get_address_of__emptyArray_5() { return &____emptyArray_5; }
	inline void set__emptyArray_5(EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5* value)
	{
		____emptyArray_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____emptyArray_5), (void*)value);
	}
};


// System.Collections.Generic.List`1<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex>
struct  List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C  : public RuntimeObject
{
public:
	// T[] System.Collections.Generic.List`1::_items
	ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* ____items_1;
	// System.Int32 System.Collections.Generic.List`1::_size
	int32_t ____size_2;
	// System.Int32 System.Collections.Generic.List`1::_version
	int32_t ____version_3;
	// System.Object System.Collections.Generic.List`1::_syncRoot
	RuntimeObject * ____syncRoot_4;

public:
	inline static int32_t get_offset_of__items_1() { return static_cast<int32_t>(offsetof(List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C, ____items_1)); }
	inline ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* get__items_1() const { return ____items_1; }
	inline ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36** get_address_of__items_1() { return &____items_1; }
	inline void set__items_1(ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* value)
	{
		____items_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____items_1), (void*)value);
	}

	inline static int32_t get_offset_of__size_2() { return static_cast<int32_t>(offsetof(List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C, ____size_2)); }
	inline int32_t get__size_2() const { return ____size_2; }
	inline int32_t* get_address_of__size_2() { return &____size_2; }
	inline void set__size_2(int32_t value)
	{
		____size_2 = value;
	}

	inline static int32_t get_offset_of__version_3() { return static_cast<int32_t>(offsetof(List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C, ____version_3)); }
	inline int32_t get__version_3() const { return ____version_3; }
	inline int32_t* get_address_of__version_3() { return &____version_3; }
	inline void set__version_3(int32_t value)
	{
		____version_3 = value;
	}

	inline static int32_t get_offset_of__syncRoot_4() { return static_cast<int32_t>(offsetof(List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C, ____syncRoot_4)); }
	inline RuntimeObject * get__syncRoot_4() const { return ____syncRoot_4; }
	inline RuntimeObject ** get_address_of__syncRoot_4() { return &____syncRoot_4; }
	inline void set__syncRoot_4(RuntimeObject * value)
	{
		____syncRoot_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_4), (void*)value);
	}
};

struct List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C_StaticFields
{
public:
	// T[] System.Collections.Generic.List`1::_emptyArray
	ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* ____emptyArray_5;

public:
	inline static int32_t get_offset_of__emptyArray_5() { return static_cast<int32_t>(offsetof(List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C_StaticFields, ____emptyArray_5)); }
	inline ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* get__emptyArray_5() const { return ____emptyArray_5; }
	inline ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36** get_address_of__emptyArray_5() { return &____emptyArray_5; }
	inline void set__emptyArray_5(ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* value)
	{
		____emptyArray_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____emptyArray_5), (void*)value);
	}
};


// System.Linq.Enumerable_Iterator`1<System.Int32>
struct  Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379  : public RuntimeObject
{
public:
	// System.Int32 System.Linq.Enumerable_Iterator`1::threadId
	int32_t ___threadId_0;
	// System.Int32 System.Linq.Enumerable_Iterator`1::state
	int32_t ___state_1;
	// TSource System.Linq.Enumerable_Iterator`1::current
	int32_t ___current_2;

public:
	inline static int32_t get_offset_of_threadId_0() { return static_cast<int32_t>(offsetof(Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379, ___threadId_0)); }
	inline int32_t get_threadId_0() const { return ___threadId_0; }
	inline int32_t* get_address_of_threadId_0() { return &___threadId_0; }
	inline void set_threadId_0(int32_t value)
	{
		___threadId_0 = value;
	}

	inline static int32_t get_offset_of_state_1() { return static_cast<int32_t>(offsetof(Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379, ___state_1)); }
	inline int32_t get_state_1() const { return ___state_1; }
	inline int32_t* get_address_of_state_1() { return &___state_1; }
	inline void set_state_1(int32_t value)
	{
		___state_1 = value;
	}

	inline static int32_t get_offset_of_current_2() { return static_cast<int32_t>(offsetof(Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379, ___current_2)); }
	inline int32_t get_current_2() const { return ___current_2; }
	inline int32_t* get_address_of_current_2() { return &___current_2; }
	inline void set_current_2(int32_t value)
	{
		___current_2 = value;
	}
};


// System.Linq.Enumerable_Iterator`1<System.Object>
struct  Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279  : public RuntimeObject
{
public:
	// System.Int32 System.Linq.Enumerable_Iterator`1::threadId
	int32_t ___threadId_0;
	// System.Int32 System.Linq.Enumerable_Iterator`1::state
	int32_t ___state_1;
	// TSource System.Linq.Enumerable_Iterator`1::current
	RuntimeObject * ___current_2;

public:
	inline static int32_t get_offset_of_threadId_0() { return static_cast<int32_t>(offsetof(Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279, ___threadId_0)); }
	inline int32_t get_threadId_0() const { return ___threadId_0; }
	inline int32_t* get_address_of_threadId_0() { return &___threadId_0; }
	inline void set_threadId_0(int32_t value)
	{
		___threadId_0 = value;
	}

	inline static int32_t get_offset_of_state_1() { return static_cast<int32_t>(offsetof(Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279, ___state_1)); }
	inline int32_t get_state_1() const { return ___state_1; }
	inline int32_t* get_address_of_state_1() { return &___state_1; }
	inline void set_state_1(int32_t value)
	{
		___state_1 = value;
	}

	inline static int32_t get_offset_of_current_2() { return static_cast<int32_t>(offsetof(Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279, ___current_2)); }
	inline RuntimeObject * get_current_2() const { return ___current_2; }
	inline RuntimeObject ** get_address_of_current_2() { return &___current_2; }
	inline void set_current_2(RuntimeObject * value)
	{
		___current_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___current_2), (void*)value);
	}
};


// System.Reflection.MemberInfo
struct  MemberInfo_t  : public RuntimeObject
{
public:

public:
};


// System.Runtime.Serialization.SerializationInfo
struct  SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1  : public RuntimeObject
{
public:
	// System.String[] System.Runtime.Serialization.SerializationInfo::m_members
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* ___m_members_3;
	// System.Object[] System.Runtime.Serialization.SerializationInfo::m_data
	ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* ___m_data_4;
	// System.Type[] System.Runtime.Serialization.SerializationInfo::m_types
	TypeU5BU5D_t85B10489E46F06CEC7C4B1CCBD0E01FAB6649755* ___m_types_5;
	// System.Collections.Generic.Dictionary`2<System.String,System.Int32> System.Runtime.Serialization.SerializationInfo::m_nameToIndex
	Dictionary_2_tC94E9875910491F8130C2DC8B11E4D1548A55162 * ___m_nameToIndex_6;
	// System.Int32 System.Runtime.Serialization.SerializationInfo::m_currMember
	int32_t ___m_currMember_7;
	// System.Runtime.Serialization.IFormatterConverter System.Runtime.Serialization.SerializationInfo::m_converter
	RuntimeObject* ___m_converter_8;
	// System.String System.Runtime.Serialization.SerializationInfo::m_fullTypeName
	String_t* ___m_fullTypeName_9;
	// System.String System.Runtime.Serialization.SerializationInfo::m_assemName
	String_t* ___m_assemName_10;
	// System.Type System.Runtime.Serialization.SerializationInfo::objectType
	Type_t * ___objectType_11;
	// System.Boolean System.Runtime.Serialization.SerializationInfo::isFullTypeNameSetExplicit
	bool ___isFullTypeNameSetExplicit_12;
	// System.Boolean System.Runtime.Serialization.SerializationInfo::isAssemblyNameSetExplicit
	bool ___isAssemblyNameSetExplicit_13;
	// System.Boolean System.Runtime.Serialization.SerializationInfo::requireSameTokenInPartialTrust
	bool ___requireSameTokenInPartialTrust_14;

public:
	inline static int32_t get_offset_of_m_members_3() { return static_cast<int32_t>(offsetof(SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1, ___m_members_3)); }
	inline StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* get_m_members_3() const { return ___m_members_3; }
	inline StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A** get_address_of_m_members_3() { return &___m_members_3; }
	inline void set_m_members_3(StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* value)
	{
		___m_members_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_members_3), (void*)value);
	}

	inline static int32_t get_offset_of_m_data_4() { return static_cast<int32_t>(offsetof(SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1, ___m_data_4)); }
	inline ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* get_m_data_4() const { return ___m_data_4; }
	inline ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE** get_address_of_m_data_4() { return &___m_data_4; }
	inline void set_m_data_4(ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* value)
	{
		___m_data_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_data_4), (void*)value);
	}

	inline static int32_t get_offset_of_m_types_5() { return static_cast<int32_t>(offsetof(SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1, ___m_types_5)); }
	inline TypeU5BU5D_t85B10489E46F06CEC7C4B1CCBD0E01FAB6649755* get_m_types_5() const { return ___m_types_5; }
	inline TypeU5BU5D_t85B10489E46F06CEC7C4B1CCBD0E01FAB6649755** get_address_of_m_types_5() { return &___m_types_5; }
	inline void set_m_types_5(TypeU5BU5D_t85B10489E46F06CEC7C4B1CCBD0E01FAB6649755* value)
	{
		___m_types_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_types_5), (void*)value);
	}

	inline static int32_t get_offset_of_m_nameToIndex_6() { return static_cast<int32_t>(offsetof(SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1, ___m_nameToIndex_6)); }
	inline Dictionary_2_tC94E9875910491F8130C2DC8B11E4D1548A55162 * get_m_nameToIndex_6() const { return ___m_nameToIndex_6; }
	inline Dictionary_2_tC94E9875910491F8130C2DC8B11E4D1548A55162 ** get_address_of_m_nameToIndex_6() { return &___m_nameToIndex_6; }
	inline void set_m_nameToIndex_6(Dictionary_2_tC94E9875910491F8130C2DC8B11E4D1548A55162 * value)
	{
		___m_nameToIndex_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_nameToIndex_6), (void*)value);
	}

	inline static int32_t get_offset_of_m_currMember_7() { return static_cast<int32_t>(offsetof(SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1, ___m_currMember_7)); }
	inline int32_t get_m_currMember_7() const { return ___m_currMember_7; }
	inline int32_t* get_address_of_m_currMember_7() { return &___m_currMember_7; }
	inline void set_m_currMember_7(int32_t value)
	{
		___m_currMember_7 = value;
	}

	inline static int32_t get_offset_of_m_converter_8() { return static_cast<int32_t>(offsetof(SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1, ___m_converter_8)); }
	inline RuntimeObject* get_m_converter_8() const { return ___m_converter_8; }
	inline RuntimeObject** get_address_of_m_converter_8() { return &___m_converter_8; }
	inline void set_m_converter_8(RuntimeObject* value)
	{
		___m_converter_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_converter_8), (void*)value);
	}

	inline static int32_t get_offset_of_m_fullTypeName_9() { return static_cast<int32_t>(offsetof(SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1, ___m_fullTypeName_9)); }
	inline String_t* get_m_fullTypeName_9() const { return ___m_fullTypeName_9; }
	inline String_t** get_address_of_m_fullTypeName_9() { return &___m_fullTypeName_9; }
	inline void set_m_fullTypeName_9(String_t* value)
	{
		___m_fullTypeName_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_fullTypeName_9), (void*)value);
	}

	inline static int32_t get_offset_of_m_assemName_10() { return static_cast<int32_t>(offsetof(SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1, ___m_assemName_10)); }
	inline String_t* get_m_assemName_10() const { return ___m_assemName_10; }
	inline String_t** get_address_of_m_assemName_10() { return &___m_assemName_10; }
	inline void set_m_assemName_10(String_t* value)
	{
		___m_assemName_10 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_assemName_10), (void*)value);
	}

	inline static int32_t get_offset_of_objectType_11() { return static_cast<int32_t>(offsetof(SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1, ___objectType_11)); }
	inline Type_t * get_objectType_11() const { return ___objectType_11; }
	inline Type_t ** get_address_of_objectType_11() { return &___objectType_11; }
	inline void set_objectType_11(Type_t * value)
	{
		___objectType_11 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___objectType_11), (void*)value);
	}

	inline static int32_t get_offset_of_isFullTypeNameSetExplicit_12() { return static_cast<int32_t>(offsetof(SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1, ___isFullTypeNameSetExplicit_12)); }
	inline bool get_isFullTypeNameSetExplicit_12() const { return ___isFullTypeNameSetExplicit_12; }
	inline bool* get_address_of_isFullTypeNameSetExplicit_12() { return &___isFullTypeNameSetExplicit_12; }
	inline void set_isFullTypeNameSetExplicit_12(bool value)
	{
		___isFullTypeNameSetExplicit_12 = value;
	}

	inline static int32_t get_offset_of_isAssemblyNameSetExplicit_13() { return static_cast<int32_t>(offsetof(SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1, ___isAssemblyNameSetExplicit_13)); }
	inline bool get_isAssemblyNameSetExplicit_13() const { return ___isAssemblyNameSetExplicit_13; }
	inline bool* get_address_of_isAssemblyNameSetExplicit_13() { return &___isAssemblyNameSetExplicit_13; }
	inline void set_isAssemblyNameSetExplicit_13(bool value)
	{
		___isAssemblyNameSetExplicit_13 = value;
	}

	inline static int32_t get_offset_of_requireSameTokenInPartialTrust_14() { return static_cast<int32_t>(offsetof(SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1, ___requireSameTokenInPartialTrust_14)); }
	inline bool get_requireSameTokenInPartialTrust_14() const { return ___requireSameTokenInPartialTrust_14; }
	inline bool* get_address_of_requireSameTokenInPartialTrust_14() { return &___requireSameTokenInPartialTrust_14; }
	inline void set_requireSameTokenInPartialTrust_14(bool value)
	{
		___requireSameTokenInPartialTrust_14 = value;
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

// UnityEngine.Rendering.VolumeParameter
struct  VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB  : public RuntimeObject
{
public:
	// System.Boolean UnityEngine.Rendering.VolumeParameter::m_OverrideState
	bool ___m_OverrideState_1;

public:
	inline static int32_t get_offset_of_m_OverrideState_1() { return static_cast<int32_t>(offsetof(VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB, ___m_OverrideState_1)); }
	inline bool get_m_OverrideState_1() const { return ___m_OverrideState_1; }
	inline bool* get_address_of_m_OverrideState_1() { return &___m_OverrideState_1; }
	inline void set_m_OverrideState_1(bool value)
	{
		___m_OverrideState_1 = value;
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


// System.Collections.Generic.List`1_Enumerator<System.Int32>
struct  Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C 
{
public:
	// System.Collections.Generic.List`1<T> System.Collections.Generic.List`1_Enumerator::list
	List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * ___list_0;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::index
	int32_t ___index_1;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::version
	int32_t ___version_2;
	// T System.Collections.Generic.List`1_Enumerator::current
	int32_t ___current_3;

public:
	inline static int32_t get_offset_of_list_0() { return static_cast<int32_t>(offsetof(Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C, ___list_0)); }
	inline List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * get_list_0() const { return ___list_0; }
	inline List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 ** get_address_of_list_0() { return &___list_0; }
	inline void set_list_0(List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * value)
	{
		___list_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___list_0), (void*)value);
	}

	inline static int32_t get_offset_of_index_1() { return static_cast<int32_t>(offsetof(Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C, ___index_1)); }
	inline int32_t get_index_1() const { return ___index_1; }
	inline int32_t* get_address_of_index_1() { return &___index_1; }
	inline void set_index_1(int32_t value)
	{
		___index_1 = value;
	}

	inline static int32_t get_offset_of_version_2() { return static_cast<int32_t>(offsetof(Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C, ___version_2)); }
	inline int32_t get_version_2() const { return ___version_2; }
	inline int32_t* get_address_of_version_2() { return &___version_2; }
	inline void set_version_2(int32_t value)
	{
		___version_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C, ___current_3)); }
	inline int32_t get_current_3() const { return ___current_3; }
	inline int32_t* get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(int32_t value)
	{
		___current_3 = value;
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


// System.Linq.Enumerable_WhereArrayIterator`1<System.Object>
struct  WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// TSource[] System.Linq.Enumerable_WhereArrayIterator`1::source
	ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereArrayIterator`1::predicate
	Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate_4;
	// System.Int32 System.Linq.Enumerable_WhereArrayIterator`1::index
	int32_t ___index_5;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86, ___source_3)); }
	inline ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* get_source_3() const { return ___source_3; }
	inline ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86, ___predicate_4)); }
	inline Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_index_5() { return static_cast<int32_t>(offsetof(WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86, ___index_5)); }
	inline int32_t get_index_5() const { return ___index_5; }
	inline int32_t* get_address_of_index_5() { return &___index_5; }
	inline void set_index_5(int32_t value)
	{
		___index_5 = value;
	}
};


// System.Linq.Enumerable_WhereEnumerableIterator`1<System.Int32>
struct  WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA  : public Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379
{
public:
	// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1::source
	RuntimeObject* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereEnumerableIterator`1::predicate
	Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate_4;
	// System.Collections.Generic.IEnumerator`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1::enumerator
	RuntimeObject* ___enumerator_5;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA, ___source_3)); }
	inline RuntimeObject* get_source_3() const { return ___source_3; }
	inline RuntimeObject** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(RuntimeObject* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA, ___predicate_4)); }
	inline Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_5() { return static_cast<int32_t>(offsetof(WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA, ___enumerator_5)); }
	inline RuntimeObject* get_enumerator_5() const { return ___enumerator_5; }
	inline RuntimeObject** get_address_of_enumerator_5() { return &___enumerator_5; }
	inline void set_enumerator_5(RuntimeObject* value)
	{
		___enumerator_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumerator_5), (void*)value);
	}
};


// System.Linq.Enumerable_WhereEnumerableIterator`1<System.Object>
struct  WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1::source
	RuntimeObject* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereEnumerableIterator`1::predicate
	Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate_4;
	// System.Collections.Generic.IEnumerator`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1::enumerator
	RuntimeObject* ___enumerator_5;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0, ___source_3)); }
	inline RuntimeObject* get_source_3() const { return ___source_3; }
	inline RuntimeObject** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(RuntimeObject* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0, ___predicate_4)); }
	inline Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_5() { return static_cast<int32_t>(offsetof(WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0, ___enumerator_5)); }
	inline RuntimeObject* get_enumerator_5() const { return ___enumerator_5; }
	inline RuntimeObject** get_address_of_enumerator_5() { return &___enumerator_5; }
	inline void set_enumerator_5(RuntimeObject* value)
	{
		___enumerator_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumerator_5), (void*)value);
	}
};


// System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Int32,System.Int32>
struct  WhereSelectArrayIterator_2_t00D4CC16A86C24F0CC80D3112E1FA2D451FF5D0C  : public Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379
{
public:
	// TSource[] System.Linq.Enumerable_WhereSelectArrayIterator`2::source
	Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectArrayIterator`2::predicate
	Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2::selector
	Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * ___selector_5;
	// System.Int32 System.Linq.Enumerable_WhereSelectArrayIterator`2::index
	int32_t ___index_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t00D4CC16A86C24F0CC80D3112E1FA2D451FF5D0C, ___source_3)); }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* get_source_3() const { return ___source_3; }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t00D4CC16A86C24F0CC80D3112E1FA2D451FF5D0C, ___predicate_4)); }
	inline Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t00D4CC16A86C24F0CC80D3112E1FA2D451FF5D0C, ___selector_5)); }
	inline Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * get_selector_5() const { return ___selector_5; }
	inline Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_index_6() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t00D4CC16A86C24F0CC80D3112E1FA2D451FF5D0C, ___index_6)); }
	inline int32_t get_index_6() const { return ___index_6; }
	inline int32_t* get_address_of_index_6() { return &___index_6; }
	inline void set_index_6(int32_t value)
	{
		___index_6 = value;
	}
};


// System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Int32,System.Object>
struct  WhereSelectArrayIterator_2_t6AF7279540EEA250525A2AFBDFAC2064A9B5C00B  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// TSource[] System.Linq.Enumerable_WhereSelectArrayIterator`2::source
	Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectArrayIterator`2::predicate
	Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2::selector
	Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * ___selector_5;
	// System.Int32 System.Linq.Enumerable_WhereSelectArrayIterator`2::index
	int32_t ___index_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t6AF7279540EEA250525A2AFBDFAC2064A9B5C00B, ___source_3)); }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* get_source_3() const { return ___source_3; }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t6AF7279540EEA250525A2AFBDFAC2064A9B5C00B, ___predicate_4)); }
	inline Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t6AF7279540EEA250525A2AFBDFAC2064A9B5C00B, ___selector_5)); }
	inline Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * get_selector_5() const { return ___selector_5; }
	inline Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_index_6() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t6AF7279540EEA250525A2AFBDFAC2064A9B5C00B, ___index_6)); }
	inline int32_t get_index_6() const { return ___index_6; }
	inline int32_t* get_address_of_index_6() { return &___index_6; }
	inline void set_index_6(int32_t value)
	{
		___index_6 = value;
	}
};


// System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Object,System.Object>
struct  WhereSelectArrayIterator_2_tA706D5B1608A9A8F1BF43C6E5D9D682C901DB244  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// TSource[] System.Linq.Enumerable_WhereSelectArrayIterator`2::source
	ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectArrayIterator`2::predicate
	Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2::selector
	Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * ___selector_5;
	// System.Int32 System.Linq.Enumerable_WhereSelectArrayIterator`2::index
	int32_t ___index_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_tA706D5B1608A9A8F1BF43C6E5D9D682C901DB244, ___source_3)); }
	inline ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* get_source_3() const { return ___source_3; }
	inline ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_tA706D5B1608A9A8F1BF43C6E5D9D682C901DB244, ___predicate_4)); }
	inline Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_tA706D5B1608A9A8F1BF43C6E5D9D682C901DB244, ___selector_5)); }
	inline Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * get_selector_5() const { return ___selector_5; }
	inline Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_index_6() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_tA706D5B1608A9A8F1BF43C6E5D9D682C901DB244, ___index_6)); }
	inline int32_t get_index_6() const { return ___index_6; }
	inline int32_t* get_address_of_index_6() { return &___index_6; }
	inline void set_index_6(int32_t value)
	{
		___index_6 = value;
	}
};


// System.Linq.Enumerable_WhereSelectArrayIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>
struct  WhereSelectArrayIterator_2_tFC05154F4D80444AACE36E75937E5D42B87B3384  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// TSource[] System.Linq.Enumerable_WhereSelectArrayIterator`2::source
	LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectArrayIterator`2::predicate
	Func_2_t15BD356B2F637699370FD7109071A37617770BBA * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2::selector
	Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * ___selector_5;
	// System.Int32 System.Linq.Enumerable_WhereSelectArrayIterator`2::index
	int32_t ___index_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_tFC05154F4D80444AACE36E75937E5D42B87B3384, ___source_3)); }
	inline LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC* get_source_3() const { return ___source_3; }
	inline LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_tFC05154F4D80444AACE36E75937E5D42B87B3384, ___predicate_4)); }
	inline Func_2_t15BD356B2F637699370FD7109071A37617770BBA * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t15BD356B2F637699370FD7109071A37617770BBA ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t15BD356B2F637699370FD7109071A37617770BBA * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_tFC05154F4D80444AACE36E75937E5D42B87B3384, ___selector_5)); }
	inline Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * get_selector_5() const { return ___selector_5; }
	inline Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_index_6() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_tFC05154F4D80444AACE36E75937E5D42B87B3384, ___index_6)); }
	inline int32_t get_index_6() const { return ___index_6; }
	inline int32_t* get_address_of_index_6() { return &___index_6; }
	inline void set_index_6(int32_t value)
	{
		___index_6 = value;
	}
};


// System.Linq.Enumerable_WhereSelectArrayIterator`2<Unity.Entities.Entity,System.Object>
struct  WhereSelectArrayIterator_2_t96F399D19669EB095BE4F3F6DFFF0695945DF634  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// TSource[] System.Linq.Enumerable_WhereSelectArrayIterator`2::source
	EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectArrayIterator`2::predicate
	Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2::selector
	Func_2_t895537CD65D26801427B03E05DD08125DE819919 * ___selector_5;
	// System.Int32 System.Linq.Enumerable_WhereSelectArrayIterator`2::index
	int32_t ___index_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t96F399D19669EB095BE4F3F6DFFF0695945DF634, ___source_3)); }
	inline EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5* get_source_3() const { return ___source_3; }
	inline EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t96F399D19669EB095BE4F3F6DFFF0695945DF634, ___predicate_4)); }
	inline Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t96F399D19669EB095BE4F3F6DFFF0695945DF634, ___selector_5)); }
	inline Func_2_t895537CD65D26801427B03E05DD08125DE819919 * get_selector_5() const { return ___selector_5; }
	inline Func_2_t895537CD65D26801427B03E05DD08125DE819919 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_t895537CD65D26801427B03E05DD08125DE819919 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_index_6() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t96F399D19669EB095BE4F3F6DFFF0695945DF634, ___index_6)); }
	inline int32_t get_index_6() const { return ___index_6; }
	inline int32_t* get_address_of_index_6() { return &___index_6; }
	inline void set_index_6(int32_t value)
	{
		___index_6 = value;
	}
};


// System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Int32,System.Int32>
struct  WhereSelectEnumerableIterator_2_t70C236BD8C0DBBA43FCE3CE4E4F6B545579333EF  : public Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379
{
public:
	// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::source
	RuntimeObject* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::predicate
	Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::selector
	Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * ___selector_5;
	// System.Collections.Generic.IEnumerator`1<TSource> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::enumerator
	RuntimeObject* ___enumerator_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t70C236BD8C0DBBA43FCE3CE4E4F6B545579333EF, ___source_3)); }
	inline RuntimeObject* get_source_3() const { return ___source_3; }
	inline RuntimeObject** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(RuntimeObject* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t70C236BD8C0DBBA43FCE3CE4E4F6B545579333EF, ___predicate_4)); }
	inline Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t70C236BD8C0DBBA43FCE3CE4E4F6B545579333EF, ___selector_5)); }
	inline Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * get_selector_5() const { return ___selector_5; }
	inline Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_6() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t70C236BD8C0DBBA43FCE3CE4E4F6B545579333EF, ___enumerator_6)); }
	inline RuntimeObject* get_enumerator_6() const { return ___enumerator_6; }
	inline RuntimeObject** get_address_of_enumerator_6() { return &___enumerator_6; }
	inline void set_enumerator_6(RuntimeObject* value)
	{
		___enumerator_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumerator_6), (void*)value);
	}
};


// System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Int32,System.Object>
struct  WhereSelectEnumerableIterator_2_tC12C4897F0443B7CCF4B7D0E720F91F10278FC0E  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::source
	RuntimeObject* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::predicate
	Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::selector
	Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * ___selector_5;
	// System.Collections.Generic.IEnumerator`1<TSource> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::enumerator
	RuntimeObject* ___enumerator_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_tC12C4897F0443B7CCF4B7D0E720F91F10278FC0E, ___source_3)); }
	inline RuntimeObject* get_source_3() const { return ___source_3; }
	inline RuntimeObject** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(RuntimeObject* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_tC12C4897F0443B7CCF4B7D0E720F91F10278FC0E, ___predicate_4)); }
	inline Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_tC12C4897F0443B7CCF4B7D0E720F91F10278FC0E, ___selector_5)); }
	inline Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * get_selector_5() const { return ___selector_5; }
	inline Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_6() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_tC12C4897F0443B7CCF4B7D0E720F91F10278FC0E, ___enumerator_6)); }
	inline RuntimeObject* get_enumerator_6() const { return ___enumerator_6; }
	inline RuntimeObject** get_address_of_enumerator_6() { return &___enumerator_6; }
	inline void set_enumerator_6(RuntimeObject* value)
	{
		___enumerator_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumerator_6), (void*)value);
	}
};


// System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Object,System.Object>
struct  WhereSelectEnumerableIterator_2_tDAA8BFBEA68F81670F3F51C6200EBD26D7A8FBAB  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::source
	RuntimeObject* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::predicate
	Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::selector
	Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * ___selector_5;
	// System.Collections.Generic.IEnumerator`1<TSource> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::enumerator
	RuntimeObject* ___enumerator_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_tDAA8BFBEA68F81670F3F51C6200EBD26D7A8FBAB, ___source_3)); }
	inline RuntimeObject* get_source_3() const { return ___source_3; }
	inline RuntimeObject** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(RuntimeObject* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_tDAA8BFBEA68F81670F3F51C6200EBD26D7A8FBAB, ___predicate_4)); }
	inline Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_tDAA8BFBEA68F81670F3F51C6200EBD26D7A8FBAB, ___selector_5)); }
	inline Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * get_selector_5() const { return ___selector_5; }
	inline Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_6() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_tDAA8BFBEA68F81670F3F51C6200EBD26D7A8FBAB, ___enumerator_6)); }
	inline RuntimeObject* get_enumerator_6() const { return ___enumerator_6; }
	inline RuntimeObject** get_address_of_enumerator_6() { return &___enumerator_6; }
	inline void set_enumerator_6(RuntimeObject* value)
	{
		___enumerator_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumerator_6), (void*)value);
	}
};


// System.Linq.Enumerable_WhereSelectEnumerableIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>
struct  WhereSelectEnumerableIterator_2_t575305DD8907FFFDC568246A6635414D5B81B807  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::source
	RuntimeObject* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::predicate
	Func_2_t15BD356B2F637699370FD7109071A37617770BBA * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::selector
	Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * ___selector_5;
	// System.Collections.Generic.IEnumerator`1<TSource> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::enumerator
	RuntimeObject* ___enumerator_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t575305DD8907FFFDC568246A6635414D5B81B807, ___source_3)); }
	inline RuntimeObject* get_source_3() const { return ___source_3; }
	inline RuntimeObject** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(RuntimeObject* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t575305DD8907FFFDC568246A6635414D5B81B807, ___predicate_4)); }
	inline Func_2_t15BD356B2F637699370FD7109071A37617770BBA * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t15BD356B2F637699370FD7109071A37617770BBA ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t15BD356B2F637699370FD7109071A37617770BBA * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t575305DD8907FFFDC568246A6635414D5B81B807, ___selector_5)); }
	inline Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * get_selector_5() const { return ___selector_5; }
	inline Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_6() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t575305DD8907FFFDC568246A6635414D5B81B807, ___enumerator_6)); }
	inline RuntimeObject* get_enumerator_6() const { return ___enumerator_6; }
	inline RuntimeObject** get_address_of_enumerator_6() { return &___enumerator_6; }
	inline void set_enumerator_6(RuntimeObject* value)
	{
		___enumerator_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumerator_6), (void*)value);
	}
};


// System.Linq.Enumerable_WhereSelectEnumerableIterator`2<Unity.Entities.Entity,System.Object>
struct  WhereSelectEnumerableIterator_2_t51ACFE0E1ABAB81E89F3A922089D2AC4ACED9718  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::source
	RuntimeObject* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::predicate
	Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::selector
	Func_2_t895537CD65D26801427B03E05DD08125DE819919 * ___selector_5;
	// System.Collections.Generic.IEnumerator`1<TSource> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::enumerator
	RuntimeObject* ___enumerator_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t51ACFE0E1ABAB81E89F3A922089D2AC4ACED9718, ___source_3)); }
	inline RuntimeObject* get_source_3() const { return ___source_3; }
	inline RuntimeObject** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(RuntimeObject* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t51ACFE0E1ABAB81E89F3A922089D2AC4ACED9718, ___predicate_4)); }
	inline Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t51ACFE0E1ABAB81E89F3A922089D2AC4ACED9718, ___selector_5)); }
	inline Func_2_t895537CD65D26801427B03E05DD08125DE819919 * get_selector_5() const { return ___selector_5; }
	inline Func_2_t895537CD65D26801427B03E05DD08125DE819919 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_t895537CD65D26801427B03E05DD08125DE819919 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_6() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t51ACFE0E1ABAB81E89F3A922089D2AC4ACED9718, ___enumerator_6)); }
	inline RuntimeObject* get_enumerator_6() const { return ___enumerator_6; }
	inline RuntimeObject** get_address_of_enumerator_6() { return &___enumerator_6; }
	inline void set_enumerator_6(RuntimeObject* value)
	{
		___enumerator_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumerator_6), (void*)value);
	}
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


// System.ValueTuple`3<System.Object,System.Object,System.Object>
struct  ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D 
{
public:
	// T1 System.ValueTuple`3::Item1
	RuntimeObject * ___Item1_0;
	// T2 System.ValueTuple`3::Item2
	RuntimeObject * ___Item2_1;
	// T3 System.ValueTuple`3::Item3
	RuntimeObject * ___Item3_2;

public:
	inline static int32_t get_offset_of_Item1_0() { return static_cast<int32_t>(offsetof(ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D, ___Item1_0)); }
	inline RuntimeObject * get_Item1_0() const { return ___Item1_0; }
	inline RuntimeObject ** get_address_of_Item1_0() { return &___Item1_0; }
	inline void set_Item1_0(RuntimeObject * value)
	{
		___Item1_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Item1_0), (void*)value);
	}

	inline static int32_t get_offset_of_Item2_1() { return static_cast<int32_t>(offsetof(ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D, ___Item2_1)); }
	inline RuntimeObject * get_Item2_1() const { return ___Item2_1; }
	inline RuntimeObject ** get_address_of_Item2_1() { return &___Item2_1; }
	inline void set_Item2_1(RuntimeObject * value)
	{
		___Item2_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Item2_1), (void*)value);
	}

	inline static int32_t get_offset_of_Item3_2() { return static_cast<int32_t>(offsetof(ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D, ___Item3_2)); }
	inline RuntimeObject * get_Item3_2() const { return ___Item3_2; }
	inline RuntimeObject ** get_address_of_Item3_2() { return &___Item3_2; }
	inline void set_Item3_2(RuntimeObject * value)
	{
		___Item3_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Item3_2), (void*)value);
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


// UnityEngine.Color
struct  Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 
{
public:
	// System.Single UnityEngine.Color::r
	float ___r_0;
	// System.Single UnityEngine.Color::g
	float ___g_1;
	// System.Single UnityEngine.Color::b
	float ___b_2;
	// System.Single UnityEngine.Color::a
	float ___a_3;

public:
	inline static int32_t get_offset_of_r_0() { return static_cast<int32_t>(offsetof(Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659, ___r_0)); }
	inline float get_r_0() const { return ___r_0; }
	inline float* get_address_of_r_0() { return &___r_0; }
	inline void set_r_0(float value)
	{
		___r_0 = value;
	}

	inline static int32_t get_offset_of_g_1() { return static_cast<int32_t>(offsetof(Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659, ___g_1)); }
	inline float get_g_1() const { return ___g_1; }
	inline float* get_address_of_g_1() { return &___g_1; }
	inline void set_g_1(float value)
	{
		___g_1 = value;
	}

	inline static int32_t get_offset_of_b_2() { return static_cast<int32_t>(offsetof(Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659, ___b_2)); }
	inline float get_b_2() const { return ___b_2; }
	inline float* get_address_of_b_2() { return &___b_2; }
	inline void set_b_2(float value)
	{
		___b_2 = value;
	}

	inline static int32_t get_offset_of_a_3() { return static_cast<int32_t>(offsetof(Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659, ___a_3)); }
	inline float get_a_3() const { return ___a_3; }
	inline float* get_address_of_a_3() { return &___a_3; }
	inline void set_a_3(float value)
	{
		___a_3 = value;
	}
};


// UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.Vec3
struct  Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1 
{
public:
	// System.Single UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.Vec3::X
	float ___X_1;
	// System.Single UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.Vec3::Y
	float ___Y_2;
	// System.Single UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.Vec3::Z
	float ___Z_3;

public:
	inline static int32_t get_offset_of_X_1() { return static_cast<int32_t>(offsetof(Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1, ___X_1)); }
	inline float get_X_1() const { return ___X_1; }
	inline float* get_address_of_X_1() { return &___X_1; }
	inline void set_X_1(float value)
	{
		___X_1 = value;
	}

	inline static int32_t get_offset_of_Y_2() { return static_cast<int32_t>(offsetof(Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1, ___Y_2)); }
	inline float get_Y_2() const { return ___Y_2; }
	inline float* get_address_of_Y_2() { return &___Y_2; }
	inline void set_Y_2(float value)
	{
		___Y_2 = value;
	}

	inline static int32_t get_offset_of_Z_3() { return static_cast<int32_t>(offsetof(Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1, ___Z_3)); }
	inline float get_Z_3() const { return ___Z_3; }
	inline float* get_address_of_Z_3() { return &___Z_3; }
	inline void set_Z_3(float value)
	{
		___Z_3 = value;
	}
};

struct Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1_StaticFields
{
public:
	// UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.Vec3 UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.Vec3::Zero
	Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1  ___Zero_0;

public:
	inline static int32_t get_offset_of_Zero_0() { return static_cast<int32_t>(offsetof(Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1_StaticFields, ___Zero_0)); }
	inline Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1  get_Zero_0() const { return ___Zero_0; }
	inline Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1 * get_address_of_Zero_0() { return &___Zero_0; }
	inline void set_Zero_0(Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1  value)
	{
		___Zero_0 = value;
	}
};


// UnityEngine.LayerMask
struct  LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 
{
public:
	// System.Int32 UnityEngine.LayerMask::m_Mask
	int32_t ___m_Mask_0;

public:
	inline static int32_t get_offset_of_m_Mask_0() { return static_cast<int32_t>(offsetof(LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8, ___m_Mask_0)); }
	inline int32_t get_m_Mask_0() const { return ___m_Mask_0; }
	inline int32_t* get_address_of_m_Mask_0() { return &___m_Mask_0; }
	inline void set_m_Mask_0(int32_t value)
	{
		___m_Mask_0 = value;
	}
};


// UnityEngine.Rendering.VolumeParameter`1<System.Boolean>
struct  VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201  : public VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB
{
public:
	// T UnityEngine.Rendering.VolumeParameter`1::m_Value
	bool ___m_Value_2;

public:
	inline static int32_t get_offset_of_m_Value_2() { return static_cast<int32_t>(offsetof(VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201, ___m_Value_2)); }
	inline bool get_m_Value_2() const { return ___m_Value_2; }
	inline bool* get_address_of_m_Value_2() { return &___m_Value_2; }
	inline void set_m_Value_2(bool value)
	{
		___m_Value_2 = value;
	}
};


// UnityEngine.Rendering.VolumeParameter`1<System.Int32>
struct  VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7  : public VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB
{
public:
	// T UnityEngine.Rendering.VolumeParameter`1::m_Value
	int32_t ___m_Value_2;

public:
	inline static int32_t get_offset_of_m_Value_2() { return static_cast<int32_t>(offsetof(VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7, ___m_Value_2)); }
	inline int32_t get_m_Value_2() const { return ___m_Value_2; }
	inline int32_t* get_address_of_m_Value_2() { return &___m_Value_2; }
	inline void set_m_Value_2(int32_t value)
	{
		___m_Value_2 = value;
	}
};


// UnityEngine.Rendering.VolumeParameter`1<System.Object>
struct  VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0  : public VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB
{
public:
	// T UnityEngine.Rendering.VolumeParameter`1::m_Value
	RuntimeObject * ___m_Value_2;

public:
	inline static int32_t get_offset_of_m_Value_2() { return static_cast<int32_t>(offsetof(VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0, ___m_Value_2)); }
	inline RuntimeObject * get_m_Value_2() const { return ___m_Value_2; }
	inline RuntimeObject ** get_address_of_m_Value_2() { return &___m_Value_2; }
	inline void set_m_Value_2(RuntimeObject * value)
	{
		___m_Value_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Value_2), (void*)value);
	}
};


// UnityEngine.Rendering.VolumeParameter`1<System.Single>
struct  VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7  : public VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB
{
public:
	// T UnityEngine.Rendering.VolumeParameter`1::m_Value
	float ___m_Value_2;

public:
	inline static int32_t get_offset_of_m_Value_2() { return static_cast<int32_t>(offsetof(VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7, ___m_Value_2)); }
	inline float get_m_Value_2() const { return ___m_Value_2; }
	inline float* get_address_of_m_Value_2() { return &___m_Value_2; }
	inline void set_m_Value_2(float value)
	{
		___m_Value_2 = value;
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


// UnityEngine.Vector4
struct  Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 
{
public:
	// System.Single UnityEngine.Vector4::x
	float ___x_1;
	// System.Single UnityEngine.Vector4::y
	float ___y_2;
	// System.Single UnityEngine.Vector4::z
	float ___z_3;
	// System.Single UnityEngine.Vector4::w
	float ___w_4;

public:
	inline static int32_t get_offset_of_x_1() { return static_cast<int32_t>(offsetof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7, ___x_1)); }
	inline float get_x_1() const { return ___x_1; }
	inline float* get_address_of_x_1() { return &___x_1; }
	inline void set_x_1(float value)
	{
		___x_1 = value;
	}

	inline static int32_t get_offset_of_y_2() { return static_cast<int32_t>(offsetof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7, ___y_2)); }
	inline float get_y_2() const { return ___y_2; }
	inline float* get_address_of_y_2() { return &___y_2; }
	inline void set_y_2(float value)
	{
		___y_2 = value;
	}

	inline static int32_t get_offset_of_z_3() { return static_cast<int32_t>(offsetof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7, ___z_3)); }
	inline float get_z_3() const { return ___z_3; }
	inline float* get_address_of_z_3() { return &___z_3; }
	inline void set_z_3(float value)
	{
		___z_3 = value;
	}

	inline static int32_t get_offset_of_w_4() { return static_cast<int32_t>(offsetof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7, ___w_4)); }
	inline float get_w_4() const { return ___w_4; }
	inline float* get_address_of_w_4() { return &___w_4; }
	inline void set_w_4(float value)
	{
		___w_4 = value;
	}
};

struct Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7_StaticFields
{
public:
	// UnityEngine.Vector4 UnityEngine.Vector4::zeroVector
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___zeroVector_5;
	// UnityEngine.Vector4 UnityEngine.Vector4::oneVector
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___oneVector_6;
	// UnityEngine.Vector4 UnityEngine.Vector4::positiveInfinityVector
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___positiveInfinityVector_7;
	// UnityEngine.Vector4 UnityEngine.Vector4::negativeInfinityVector
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___negativeInfinityVector_8;

public:
	inline static int32_t get_offset_of_zeroVector_5() { return static_cast<int32_t>(offsetof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7_StaticFields, ___zeroVector_5)); }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  get_zeroVector_5() const { return ___zeroVector_5; }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * get_address_of_zeroVector_5() { return &___zeroVector_5; }
	inline void set_zeroVector_5(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  value)
	{
		___zeroVector_5 = value;
	}

	inline static int32_t get_offset_of_oneVector_6() { return static_cast<int32_t>(offsetof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7_StaticFields, ___oneVector_6)); }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  get_oneVector_6() const { return ___oneVector_6; }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * get_address_of_oneVector_6() { return &___oneVector_6; }
	inline void set_oneVector_6(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  value)
	{
		___oneVector_6 = value;
	}

	inline static int32_t get_offset_of_positiveInfinityVector_7() { return static_cast<int32_t>(offsetof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7_StaticFields, ___positiveInfinityVector_7)); }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  get_positiveInfinityVector_7() const { return ___positiveInfinityVector_7; }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * get_address_of_positiveInfinityVector_7() { return &___positiveInfinityVector_7; }
	inline void set_positiveInfinityVector_7(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  value)
	{
		___positiveInfinityVector_7 = value;
	}

	inline static int32_t get_offset_of_negativeInfinityVector_8() { return static_cast<int32_t>(offsetof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7_StaticFields, ___negativeInfinityVector_8)); }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  get_negativeInfinityVector_8() const { return ___negativeInfinityVector_8; }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * get_address_of_negativeInfinityVector_8() { return &___negativeInfinityVector_8; }
	inline void set_negativeInfinityVector_8(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  value)
	{
		___negativeInfinityVector_8 = value;
	}
};


// System.Collections.Generic.List`1_Enumerator<Unity.Entities.Entity>
struct  Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 
{
public:
	// System.Collections.Generic.List`1<T> System.Collections.Generic.List`1_Enumerator::list
	List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF * ___list_0;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::index
	int32_t ___index_1;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::version
	int32_t ___version_2;
	// T System.Collections.Generic.List`1_Enumerator::current
	Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  ___current_3;

public:
	inline static int32_t get_offset_of_list_0() { return static_cast<int32_t>(offsetof(Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62, ___list_0)); }
	inline List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF * get_list_0() const { return ___list_0; }
	inline List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF ** get_address_of_list_0() { return &___list_0; }
	inline void set_list_0(List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF * value)
	{
		___list_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___list_0), (void*)value);
	}

	inline static int32_t get_offset_of_index_1() { return static_cast<int32_t>(offsetof(Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62, ___index_1)); }
	inline int32_t get_index_1() const { return ___index_1; }
	inline int32_t* get_address_of_index_1() { return &___index_1; }
	inline void set_index_1(int32_t value)
	{
		___index_1 = value;
	}

	inline static int32_t get_offset_of_version_2() { return static_cast<int32_t>(offsetof(Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62, ___version_2)); }
	inline int32_t get_version_2() const { return ___version_2; }
	inline int32_t* get_address_of_version_2() { return &___version_2; }
	inline void set_version_2(int32_t value)
	{
		___version_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62, ___current_3)); }
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  get_current_3() const { return ___current_3; }
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  value)
	{
		___current_3 = value;
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

// System.Int32Enum
struct  Int32Enum_t9B63F771913F2B6D586F1173B44A41FBE26F6B5C 
{
public:
	// System.Int32 System.Int32Enum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(Int32Enum_t9B63F771913F2B6D586F1173B44A41FBE26F6B5C, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// System.Linq.Enumerable_Iterator`1<UnityEngine.Color>
struct  Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76  : public RuntimeObject
{
public:
	// System.Int32 System.Linq.Enumerable_Iterator`1::threadId
	int32_t ___threadId_0;
	// System.Int32 System.Linq.Enumerable_Iterator`1::state
	int32_t ___state_1;
	// TSource System.Linq.Enumerable_Iterator`1::current
	Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  ___current_2;

public:
	inline static int32_t get_offset_of_threadId_0() { return static_cast<int32_t>(offsetof(Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76, ___threadId_0)); }
	inline int32_t get_threadId_0() const { return ___threadId_0; }
	inline int32_t* get_address_of_threadId_0() { return &___threadId_0; }
	inline void set_threadId_0(int32_t value)
	{
		___threadId_0 = value;
	}

	inline static int32_t get_offset_of_state_1() { return static_cast<int32_t>(offsetof(Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76, ___state_1)); }
	inline int32_t get_state_1() const { return ___state_1; }
	inline int32_t* get_address_of_state_1() { return &___state_1; }
	inline void set_state_1(int32_t value)
	{
		___state_1 = value;
	}

	inline static int32_t get_offset_of_current_2() { return static_cast<int32_t>(offsetof(Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76, ___current_2)); }
	inline Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  get_current_2() const { return ___current_2; }
	inline Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 * get_address_of_current_2() { return &___current_2; }
	inline void set_current_2(Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  value)
	{
		___current_2 = value;
	}
};


// System.Linq.Enumerable_Iterator`1<UnityEngine.Vector3>
struct  Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF  : public RuntimeObject
{
public:
	// System.Int32 System.Linq.Enumerable_Iterator`1::threadId
	int32_t ___threadId_0;
	// System.Int32 System.Linq.Enumerable_Iterator`1::state
	int32_t ___state_1;
	// TSource System.Linq.Enumerable_Iterator`1::current
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___current_2;

public:
	inline static int32_t get_offset_of_threadId_0() { return static_cast<int32_t>(offsetof(Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF, ___threadId_0)); }
	inline int32_t get_threadId_0() const { return ___threadId_0; }
	inline int32_t* get_address_of_threadId_0() { return &___threadId_0; }
	inline void set_threadId_0(int32_t value)
	{
		___threadId_0 = value;
	}

	inline static int32_t get_offset_of_state_1() { return static_cast<int32_t>(offsetof(Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF, ___state_1)); }
	inline int32_t get_state_1() const { return ___state_1; }
	inline int32_t* get_address_of_state_1() { return &___state_1; }
	inline void set_state_1(int32_t value)
	{
		___state_1 = value;
	}

	inline static int32_t get_offset_of_current_2() { return static_cast<int32_t>(offsetof(Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF, ___current_2)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_current_2() const { return ___current_2; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_current_2() { return &___current_2; }
	inline void set_current_2(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___current_2 = value;
	}
};


// System.Linq.Enumerable_WhereListIterator`1<System.Object>
struct  WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// System.Collections.Generic.List`1<TSource> System.Linq.Enumerable_WhereListIterator`1::source
	List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereListIterator`1::predicate
	Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate_4;
	// System.Collections.Generic.List`1_Enumerator<TSource> System.Linq.Enumerable_WhereListIterator`1::enumerator
	Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6  ___enumerator_5;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD, ___source_3)); }
	inline List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * get_source_3() const { return ___source_3; }
	inline List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 ** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD, ___predicate_4)); }
	inline Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_5() { return static_cast<int32_t>(offsetof(WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD, ___enumerator_5)); }
	inline Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6  get_enumerator_5() const { return ___enumerator_5; }
	inline Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * get_address_of_enumerator_5() { return &___enumerator_5; }
	inline void set_enumerator_5(Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6  value)
	{
		___enumerator_5 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___enumerator_5))->___list_0), (void*)NULL);
		#if IL2CPP_ENABLE_STRICT_WRITE_BARRIERS
		Il2CppCodeGenWriteBarrier((void**)&(((&___enumerator_5))->___current_3), (void*)NULL);
		#endif
	}
};


// System.Linq.Enumerable_WhereSelectListIterator`2<System.Int32,System.Int32>
struct  WhereSelectListIterator_2_t4CC3FE3A35610DC6F761EE7DB863B845957AD325  : public Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379
{
public:
	// System.Collections.Generic.List`1<TSource> System.Linq.Enumerable_WhereSelectListIterator`2::source
	List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectListIterator`2::predicate
	Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectListIterator`2::selector
	Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * ___selector_5;
	// System.Collections.Generic.List`1_Enumerator<TSource> System.Linq.Enumerable_WhereSelectListIterator`2::enumerator
	Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C  ___enumerator_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t4CC3FE3A35610DC6F761EE7DB863B845957AD325, ___source_3)); }
	inline List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * get_source_3() const { return ___source_3; }
	inline List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 ** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t4CC3FE3A35610DC6F761EE7DB863B845957AD325, ___predicate_4)); }
	inline Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t4CC3FE3A35610DC6F761EE7DB863B845957AD325, ___selector_5)); }
	inline Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * get_selector_5() const { return ___selector_5; }
	inline Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_6() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t4CC3FE3A35610DC6F761EE7DB863B845957AD325, ___enumerator_6)); }
	inline Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C  get_enumerator_6() const { return ___enumerator_6; }
	inline Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C * get_address_of_enumerator_6() { return &___enumerator_6; }
	inline void set_enumerator_6(Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C  value)
	{
		___enumerator_6 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___enumerator_6))->___list_0), (void*)NULL);
	}
};


// System.Linq.Enumerable_WhereSelectListIterator`2<System.Int32,System.Object>
struct  WhereSelectListIterator_2_tA41D93FF12E41BB5A5BEA27AEED367695ADACEA4  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// System.Collections.Generic.List`1<TSource> System.Linq.Enumerable_WhereSelectListIterator`2::source
	List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectListIterator`2::predicate
	Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectListIterator`2::selector
	Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * ___selector_5;
	// System.Collections.Generic.List`1_Enumerator<TSource> System.Linq.Enumerable_WhereSelectListIterator`2::enumerator
	Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C  ___enumerator_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_tA41D93FF12E41BB5A5BEA27AEED367695ADACEA4, ___source_3)); }
	inline List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * get_source_3() const { return ___source_3; }
	inline List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 ** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_tA41D93FF12E41BB5A5BEA27AEED367695ADACEA4, ___predicate_4)); }
	inline Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_tA41D93FF12E41BB5A5BEA27AEED367695ADACEA4, ___selector_5)); }
	inline Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * get_selector_5() const { return ___selector_5; }
	inline Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_6() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_tA41D93FF12E41BB5A5BEA27AEED367695ADACEA4, ___enumerator_6)); }
	inline Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C  get_enumerator_6() const { return ___enumerator_6; }
	inline Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C * get_address_of_enumerator_6() { return &___enumerator_6; }
	inline void set_enumerator_6(Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C  value)
	{
		___enumerator_6 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___enumerator_6))->___list_0), (void*)NULL);
	}
};


// System.Linq.Enumerable_WhereSelectListIterator`2<System.Object,System.Object>
struct  WhereSelectListIterator_2_t85B78DFF0573BC95A62C79D6088FA39DFEBE1AF2  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// System.Collections.Generic.List`1<TSource> System.Linq.Enumerable_WhereSelectListIterator`2::source
	List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectListIterator`2::predicate
	Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectListIterator`2::selector
	Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * ___selector_5;
	// System.Collections.Generic.List`1_Enumerator<TSource> System.Linq.Enumerable_WhereSelectListIterator`2::enumerator
	Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6  ___enumerator_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t85B78DFF0573BC95A62C79D6088FA39DFEBE1AF2, ___source_3)); }
	inline List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * get_source_3() const { return ___source_3; }
	inline List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 ** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t85B78DFF0573BC95A62C79D6088FA39DFEBE1AF2, ___predicate_4)); }
	inline Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t85B78DFF0573BC95A62C79D6088FA39DFEBE1AF2, ___selector_5)); }
	inline Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * get_selector_5() const { return ___selector_5; }
	inline Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_6() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t85B78DFF0573BC95A62C79D6088FA39DFEBE1AF2, ___enumerator_6)); }
	inline Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6  get_enumerator_6() const { return ___enumerator_6; }
	inline Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * get_address_of_enumerator_6() { return &___enumerator_6; }
	inline void set_enumerator_6(Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6  value)
	{
		___enumerator_6 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___enumerator_6))->___list_0), (void*)NULL);
		#if IL2CPP_ENABLE_STRICT_WRITE_BARRIERS
		Il2CppCodeGenWriteBarrier((void**)&(((&___enumerator_6))->___current_3), (void*)NULL);
		#endif
	}
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


// System.Runtime.InteropServices.GCHandleType
struct  GCHandleType_t5D58978165671EDEFCCAE1E2B237BD5AE4E8BC38 
{
public:
	// System.Int32 System.Runtime.InteropServices.GCHandleType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(GCHandleType_t5D58978165671EDEFCCAE1E2B237BD5AE4E8BC38, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// System.Runtime.Serialization.StreamingContextStates
struct  StreamingContextStates_tF4C7FE6D6121BD4C67699869C8269A60B36B42C3 
{
public:
	// System.Int32 System.Runtime.Serialization.StreamingContextStates::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(StreamingContextStates_tF4C7FE6D6121BD4C67699869C8269A60B36B42C3, ___value___2)); }
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


// System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>
struct  ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 
{
public:
	// T1 System.ValueTuple`3::Item1
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___Item1_0;
	// T2 System.ValueTuple`3::Item2
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___Item2_1;
	// T3 System.ValueTuple`3::Item3
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___Item3_2;

public:
	inline static int32_t get_offset_of_Item1_0() { return static_cast<int32_t>(offsetof(ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5, ___Item1_0)); }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  get_Item1_0() const { return ___Item1_0; }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * get_address_of_Item1_0() { return &___Item1_0; }
	inline void set_Item1_0(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  value)
	{
		___Item1_0 = value;
	}

	inline static int32_t get_offset_of_Item2_1() { return static_cast<int32_t>(offsetof(ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5, ___Item2_1)); }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  get_Item2_1() const { return ___Item2_1; }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * get_address_of_Item2_1() { return &___Item2_1; }
	inline void set_Item2_1(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  value)
	{
		___Item2_1 = value;
	}

	inline static int32_t get_offset_of_Item3_2() { return static_cast<int32_t>(offsetof(ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5, ___Item3_2)); }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  get_Item3_2() const { return ___Item3_2; }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * get_address_of_Item3_2() { return &___Item3_2; }
	inline void set_Item3_2(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  value)
	{
		___Item3_2 = value;
	}
};


// System.WeakReference`1<System.Object>
struct  WeakReference_1_t223E87F227F99044D112FF1E7FC7DA135365CB76  : public RuntimeObject
{
public:
	// System.Runtime.InteropServices.GCHandle System.WeakReference`1::handle
	GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603  ___handle_0;
	// System.Boolean System.WeakReference`1::trackResurrection
	bool ___trackResurrection_1;

public:
	inline static int32_t get_offset_of_handle_0() { return static_cast<int32_t>(offsetof(WeakReference_1_t223E87F227F99044D112FF1E7FC7DA135365CB76, ___handle_0)); }
	inline GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603  get_handle_0() const { return ___handle_0; }
	inline GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 * get_address_of_handle_0() { return &___handle_0; }
	inline void set_handle_0(GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603  value)
	{
		___handle_0 = value;
	}

	inline static int32_t get_offset_of_trackResurrection_1() { return static_cast<int32_t>(offsetof(WeakReference_1_t223E87F227F99044D112FF1E7FC7DA135365CB76, ___trackResurrection_1)); }
	inline bool get_trackResurrection_1() const { return ___trackResurrection_1; }
	inline bool* get_address_of_trackResurrection_1() { return &___trackResurrection_1; }
	inline void set_trackResurrection_1(bool value)
	{
		___trackResurrection_1 = value;
	}
};


// UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex
struct  ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 
{
public:
	// UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.Vec3 UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex::Position
	Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1  ___Position_0;
	// System.Object UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex::Data
	RuntimeObject * ___Data_1;

public:
	inline static int32_t get_offset_of_Position_0() { return static_cast<int32_t>(offsetof(ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536, ___Position_0)); }
	inline Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1  get_Position_0() const { return ___Position_0; }
	inline Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1 * get_address_of_Position_0() { return &___Position_0; }
	inline void set_Position_0(Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1  value)
	{
		___Position_0 = value;
	}

	inline static int32_t get_offset_of_Data_1() { return static_cast<int32_t>(offsetof(ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536, ___Data_1)); }
	inline RuntimeObject * get_Data_1() const { return ___Data_1; }
	inline RuntimeObject ** get_address_of_Data_1() { return &___Data_1; }
	inline void set_Data_1(RuntimeObject * value)
	{
		___Data_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Data_1), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex
struct ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536_marshaled_pinvoke
{
	Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1  ___Position_0;
	Il2CppIUnknown* ___Data_1;
};
// Native definition for COM marshalling of UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex
struct ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536_marshaled_com
{
	Vec3_tDD913B31171F6A37E61E4625FEA6C7901A6B1BC1  ___Position_0;
	Il2CppIUnknown* ___Data_1;
};

// UnityEngine.LogType
struct  LogType_tF490DBF8368BD4EBA703B2824CB76A853820F773 
{
public:
	// System.Int32 UnityEngine.LogType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(LogType_tF490DBF8368BD4EBA703B2824CB76A853820F773, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>
struct  VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A  : public VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB
{
public:
	// T UnityEngine.Rendering.VolumeParameter`1::m_Value
	Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  ___m_Value_2;

public:
	inline static int32_t get_offset_of_m_Value_2() { return static_cast<int32_t>(offsetof(VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A, ___m_Value_2)); }
	inline Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  get_m_Value_2() const { return ___m_Value_2; }
	inline Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 * get_address_of_m_Value_2() { return &___m_Value_2; }
	inline void set_m_Value_2(Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  value)
	{
		___m_Value_2 = value;
	}
};


// UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>
struct  VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D  : public VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB
{
public:
	// T UnityEngine.Rendering.VolumeParameter`1::m_Value
	LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  ___m_Value_2;

public:
	inline static int32_t get_offset_of_m_Value_2() { return static_cast<int32_t>(offsetof(VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D, ___m_Value_2)); }
	inline LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  get_m_Value_2() const { return ___m_Value_2; }
	inline LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 * get_address_of_m_Value_2() { return &___m_Value_2; }
	inline void set_m_Value_2(LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  value)
	{
		___m_Value_2 = value;
	}
};


// UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>
struct  VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7  : public VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB
{
public:
	// T UnityEngine.Rendering.VolumeParameter`1::m_Value
	Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___m_Value_2;

public:
	inline static int32_t get_offset_of_m_Value_2() { return static_cast<int32_t>(offsetof(VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7, ___m_Value_2)); }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  get_m_Value_2() const { return ___m_Value_2; }
	inline Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 * get_address_of_m_Value_2() { return &___m_Value_2; }
	inline void set_m_Value_2(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  value)
	{
		___m_Value_2 = value;
	}
};


// UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>
struct  VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F  : public VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB
{
public:
	// T UnityEngine.Rendering.VolumeParameter`1::m_Value
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___m_Value_2;

public:
	inline static int32_t get_offset_of_m_Value_2() { return static_cast<int32_t>(offsetof(VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F, ___m_Value_2)); }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  get_m_Value_2() const { return ___m_Value_2; }
	inline Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * get_address_of_m_Value_2() { return &___m_Value_2; }
	inline void set_m_Value_2(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  value)
	{
		___m_Value_2 = value;
	}
};


// UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>
struct  VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C  : public VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB
{
public:
	// T UnityEngine.Rendering.VolumeParameter`1::m_Value
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___m_Value_2;

public:
	inline static int32_t get_offset_of_m_Value_2() { return static_cast<int32_t>(offsetof(VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C, ___m_Value_2)); }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  get_m_Value_2() const { return ___m_Value_2; }
	inline Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * get_address_of_m_Value_2() { return &___m_Value_2; }
	inline void set_m_Value_2(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  value)
	{
		___m_Value_2 = value;
	}
};


// System.Collections.Generic.List`1_Enumerator<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex>
struct  Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 
{
public:
	// System.Collections.Generic.List`1<T> System.Collections.Generic.List`1_Enumerator::list
	List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * ___list_0;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::index
	int32_t ___index_1;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::version
	int32_t ___version_2;
	// T System.Collections.Generic.List`1_Enumerator::current
	ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  ___current_3;

public:
	inline static int32_t get_offset_of_list_0() { return static_cast<int32_t>(offsetof(Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11, ___list_0)); }
	inline List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * get_list_0() const { return ___list_0; }
	inline List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C ** get_address_of_list_0() { return &___list_0; }
	inline void set_list_0(List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * value)
	{
		___list_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___list_0), (void*)value);
	}

	inline static int32_t get_offset_of_index_1() { return static_cast<int32_t>(offsetof(Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11, ___index_1)); }
	inline int32_t get_index_1() const { return ___index_1; }
	inline int32_t* get_address_of_index_1() { return &___index_1; }
	inline void set_index_1(int32_t value)
	{
		___index_1 = value;
	}

	inline static int32_t get_offset_of_version_2() { return static_cast<int32_t>(offsetof(Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11, ___version_2)); }
	inline int32_t get_version_2() const { return ___version_2; }
	inline int32_t* get_address_of_version_2() { return &___version_2; }
	inline void set_version_2(int32_t value)
	{
		___version_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11, ___current_3)); }
	inline ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  get_current_3() const { return ___current_3; }
	inline ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 * get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  value)
	{
		___current_3 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___current_3))->___Data_1), (void*)NULL);
	}
};


// System.Linq.Enumerable_WhereEnumerableIterator`1<UnityEngine.Color>
struct  WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2  : public Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76
{
public:
	// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1::source
	RuntimeObject* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereEnumerableIterator`1::predicate
	Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * ___predicate_4;
	// System.Collections.Generic.IEnumerator`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1::enumerator
	RuntimeObject* ___enumerator_5;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2, ___source_3)); }
	inline RuntimeObject* get_source_3() const { return ___source_3; }
	inline RuntimeObject** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(RuntimeObject* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2, ___predicate_4)); }
	inline Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_5() { return static_cast<int32_t>(offsetof(WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2, ___enumerator_5)); }
	inline RuntimeObject* get_enumerator_5() const { return ___enumerator_5; }
	inline RuntimeObject** get_address_of_enumerator_5() { return &___enumerator_5; }
	inline void set_enumerator_5(RuntimeObject* value)
	{
		___enumerator_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumerator_5), (void*)value);
	}
};


// System.Linq.Enumerable_WhereEnumerableIterator`1<UnityEngine.Vector3>
struct  WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8  : public Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF
{
public:
	// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1::source
	RuntimeObject* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereEnumerableIterator`1::predicate
	Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * ___predicate_4;
	// System.Collections.Generic.IEnumerator`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1::enumerator
	RuntimeObject* ___enumerator_5;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8, ___source_3)); }
	inline RuntimeObject* get_source_3() const { return ___source_3; }
	inline RuntimeObject** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(RuntimeObject* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8, ___predicate_4)); }
	inline Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_5() { return static_cast<int32_t>(offsetof(WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8, ___enumerator_5)); }
	inline RuntimeObject* get_enumerator_5() const { return ___enumerator_5; }
	inline RuntimeObject** get_address_of_enumerator_5() { return &___enumerator_5; }
	inline void set_enumerator_5(RuntimeObject* value)
	{
		___enumerator_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumerator_5), (void*)value);
	}
};


// System.Linq.Enumerable_WhereSelectArrayIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>
struct  WhereSelectArrayIterator_2_tCB97637D799FCAD3115615E9FDA8E5CF80A108EC  : public Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76
{
public:
	// TSource[] System.Linq.Enumerable_WhereSelectArrayIterator`2::source
	ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectArrayIterator`2::predicate
	Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2::selector
	Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * ___selector_5;
	// System.Int32 System.Linq.Enumerable_WhereSelectArrayIterator`2::index
	int32_t ___index_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_tCB97637D799FCAD3115615E9FDA8E5CF80A108EC, ___source_3)); }
	inline ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* get_source_3() const { return ___source_3; }
	inline ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_tCB97637D799FCAD3115615E9FDA8E5CF80A108EC, ___predicate_4)); }
	inline Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_tCB97637D799FCAD3115615E9FDA8E5CF80A108EC, ___selector_5)); }
	inline Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * get_selector_5() const { return ___selector_5; }
	inline Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_index_6() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_tCB97637D799FCAD3115615E9FDA8E5CF80A108EC, ___index_6)); }
	inline int32_t get_index_6() const { return ___index_6; }
	inline int32_t* get_address_of_index_6() { return &___index_6; }
	inline void set_index_6(int32_t value)
	{
		___index_6 = value;
	}
};


// System.Linq.Enumerable_WhereSelectArrayIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>
struct  WhereSelectArrayIterator_2_t08DE4878AA443563B5940613DEBED682F6942F28  : public Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF
{
public:
	// TSource[] System.Linq.Enumerable_WhereSelectArrayIterator`2::source
	ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectArrayIterator`2::predicate
	Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2::selector
	Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * ___selector_5;
	// System.Int32 System.Linq.Enumerable_WhereSelectArrayIterator`2::index
	int32_t ___index_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t08DE4878AA443563B5940613DEBED682F6942F28, ___source_3)); }
	inline ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* get_source_3() const { return ___source_3; }
	inline ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t08DE4878AA443563B5940613DEBED682F6942F28, ___predicate_4)); }
	inline Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t08DE4878AA443563B5940613DEBED682F6942F28, ___selector_5)); }
	inline Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * get_selector_5() const { return ___selector_5; }
	inline Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_index_6() { return static_cast<int32_t>(offsetof(WhereSelectArrayIterator_2_t08DE4878AA443563B5940613DEBED682F6942F28, ___index_6)); }
	inline int32_t get_index_6() const { return ___index_6; }
	inline int32_t* get_address_of_index_6() { return &___index_6; }
	inline void set_index_6(int32_t value)
	{
		___index_6 = value;
	}
};


// System.Linq.Enumerable_WhereSelectEnumerableIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>
struct  WhereSelectEnumerableIterator_2_t082E1B5B13EA24679459FB2484F9A187BB83D681  : public Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76
{
public:
	// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::source
	RuntimeObject* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::predicate
	Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::selector
	Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * ___selector_5;
	// System.Collections.Generic.IEnumerator`1<TSource> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::enumerator
	RuntimeObject* ___enumerator_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t082E1B5B13EA24679459FB2484F9A187BB83D681, ___source_3)); }
	inline RuntimeObject* get_source_3() const { return ___source_3; }
	inline RuntimeObject** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(RuntimeObject* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t082E1B5B13EA24679459FB2484F9A187BB83D681, ___predicate_4)); }
	inline Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t082E1B5B13EA24679459FB2484F9A187BB83D681, ___selector_5)); }
	inline Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * get_selector_5() const { return ___selector_5; }
	inline Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_6() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t082E1B5B13EA24679459FB2484F9A187BB83D681, ___enumerator_6)); }
	inline RuntimeObject* get_enumerator_6() const { return ___enumerator_6; }
	inline RuntimeObject** get_address_of_enumerator_6() { return &___enumerator_6; }
	inline void set_enumerator_6(RuntimeObject* value)
	{
		___enumerator_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumerator_6), (void*)value);
	}
};


// System.Linq.Enumerable_WhereSelectEnumerableIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>
struct  WhereSelectEnumerableIterator_2_t3B7C9E548F0B01F9C6D09D71FCA8E5ACDE0F3FCD  : public Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF
{
public:
	// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::source
	RuntimeObject* ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::predicate
	Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::selector
	Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * ___selector_5;
	// System.Collections.Generic.IEnumerator`1<TSource> System.Linq.Enumerable_WhereSelectEnumerableIterator`2::enumerator
	RuntimeObject* ___enumerator_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t3B7C9E548F0B01F9C6D09D71FCA8E5ACDE0F3FCD, ___source_3)); }
	inline RuntimeObject* get_source_3() const { return ___source_3; }
	inline RuntimeObject** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(RuntimeObject* value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t3B7C9E548F0B01F9C6D09D71FCA8E5ACDE0F3FCD, ___predicate_4)); }
	inline Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t3B7C9E548F0B01F9C6D09D71FCA8E5ACDE0F3FCD, ___selector_5)); }
	inline Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * get_selector_5() const { return ___selector_5; }
	inline Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_6() { return static_cast<int32_t>(offsetof(WhereSelectEnumerableIterator_2_t3B7C9E548F0B01F9C6D09D71FCA8E5ACDE0F3FCD, ___enumerator_6)); }
	inline RuntimeObject* get_enumerator_6() const { return ___enumerator_6; }
	inline RuntimeObject** get_address_of_enumerator_6() { return &___enumerator_6; }
	inline void set_enumerator_6(RuntimeObject* value)
	{
		___enumerator_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumerator_6), (void*)value);
	}
};


// System.Linq.Enumerable_WhereSelectListIterator`2<Unity.Entities.Entity,System.Object>
struct  WhereSelectListIterator_2_t9800F9AD3EEADD8B7707A9F9E67DFC4C12E2F924  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// System.Collections.Generic.List`1<TSource> System.Linq.Enumerable_WhereSelectListIterator`2::source
	List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF * ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectListIterator`2::predicate
	Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectListIterator`2::selector
	Func_2_t895537CD65D26801427B03E05DD08125DE819919 * ___selector_5;
	// System.Collections.Generic.List`1_Enumerator<TSource> System.Linq.Enumerable_WhereSelectListIterator`2::enumerator
	Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62  ___enumerator_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t9800F9AD3EEADD8B7707A9F9E67DFC4C12E2F924, ___source_3)); }
	inline List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF * get_source_3() const { return ___source_3; }
	inline List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF ** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF * value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t9800F9AD3EEADD8B7707A9F9E67DFC4C12E2F924, ___predicate_4)); }
	inline Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t9800F9AD3EEADD8B7707A9F9E67DFC4C12E2F924, ___selector_5)); }
	inline Func_2_t895537CD65D26801427B03E05DD08125DE819919 * get_selector_5() const { return ___selector_5; }
	inline Func_2_t895537CD65D26801427B03E05DD08125DE819919 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_t895537CD65D26801427B03E05DD08125DE819919 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_6() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t9800F9AD3EEADD8B7707A9F9E67DFC4C12E2F924, ___enumerator_6)); }
	inline Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62  get_enumerator_6() const { return ___enumerator_6; }
	inline Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 * get_address_of_enumerator_6() { return &___enumerator_6; }
	inline void set_enumerator_6(Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62  value)
	{
		___enumerator_6 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___enumerator_6))->___list_0), (void*)NULL);
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

// System.Runtime.Serialization.StreamingContext
struct  StreamingContext_t5888E7E8C81AB6EF3B14FDDA6674F458076A8505 
{
public:
	// System.Object System.Runtime.Serialization.StreamingContext::m_additionalContext
	RuntimeObject * ___m_additionalContext_0;
	// System.Runtime.Serialization.StreamingContextStates System.Runtime.Serialization.StreamingContext::m_state
	int32_t ___m_state_1;

public:
	inline static int32_t get_offset_of_m_additionalContext_0() { return static_cast<int32_t>(offsetof(StreamingContext_t5888E7E8C81AB6EF3B14FDDA6674F458076A8505, ___m_additionalContext_0)); }
	inline RuntimeObject * get_m_additionalContext_0() const { return ___m_additionalContext_0; }
	inline RuntimeObject ** get_address_of_m_additionalContext_0() { return &___m_additionalContext_0; }
	inline void set_m_additionalContext_0(RuntimeObject * value)
	{
		___m_additionalContext_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_additionalContext_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_state_1() { return static_cast<int32_t>(offsetof(StreamingContext_t5888E7E8C81AB6EF3B14FDDA6674F458076A8505, ___m_state_1)); }
	inline int32_t get_m_state_1() const { return ___m_state_1; }
	inline int32_t* get_address_of_m_state_1() { return &___m_state_1; }
	inline void set_m_state_1(int32_t value)
	{
		___m_state_1 = value;
	}
};

// Native definition for P/Invoke marshalling of System.Runtime.Serialization.StreamingContext
struct StreamingContext_t5888E7E8C81AB6EF3B14FDDA6674F458076A8505_marshaled_pinvoke
{
	Il2CppIUnknown* ___m_additionalContext_0;
	int32_t ___m_state_1;
};
// Native definition for COM marshalling of System.Runtime.Serialization.StreamingContext
struct StreamingContext_t5888E7E8C81AB6EF3B14FDDA6674F458076A8505_marshaled_com
{
	Il2CppIUnknown* ___m_additionalContext_0;
	int32_t ___m_state_1;
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


// Unity.Entities.Conversion.LogEventData
struct  LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 
{
public:
	// UnityEngine.LogType Unity.Entities.Conversion.LogEventData::Type
	int32_t ___Type_0;
	// System.String Unity.Entities.Conversion.LogEventData::Message
	String_t* ___Message_1;

public:
	inline static int32_t get_offset_of_Type_0() { return static_cast<int32_t>(offsetof(LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1, ___Type_0)); }
	inline int32_t get_Type_0() const { return ___Type_0; }
	inline int32_t* get_address_of_Type_0() { return &___Type_0; }
	inline void set_Type_0(int32_t value)
	{
		___Type_0 = value;
	}

	inline static int32_t get_offset_of_Message_1() { return static_cast<int32_t>(offsetof(LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1, ___Message_1)); }
	inline String_t* get_Message_1() const { return ___Message_1; }
	inline String_t** get_address_of_Message_1() { return &___Message_1; }
	inline void set_Message_1(String_t* value)
	{
		___Message_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Message_1), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of Unity.Entities.Conversion.LogEventData
struct LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1_marshaled_pinvoke
{
	int32_t ___Type_0;
	char* ___Message_1;
};
// Native definition for COM marshalling of Unity.Entities.Conversion.LogEventData
struct LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1_marshaled_com
{
	int32_t ___Type_0;
	Il2CppChar* ___Message_1;
};

// UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>
struct  VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740  : public VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB
{
public:
	// T UnityEngine.Rendering.VolumeParameter`1::m_Value
	int32_t ___m_Value_2;

public:
	inline static int32_t get_offset_of_m_Value_2() { return static_cast<int32_t>(offsetof(VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740, ___m_Value_2)); }
	inline int32_t get_m_Value_2() const { return ___m_Value_2; }
	inline int32_t* get_address_of_m_Value_2() { return &___m_Value_2; }
	inline void set_m_Value_2(int32_t value)
	{
		___m_Value_2 = value;
	}
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


// System.Collections.Generic.List`1_Enumerator<Unity.Entities.Conversion.LogEventData>
struct  Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE 
{
public:
	// System.Collections.Generic.List`1<T> System.Collections.Generic.List`1_Enumerator::list
	List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 * ___list_0;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::index
	int32_t ___index_1;
	// System.Int32 System.Collections.Generic.List`1_Enumerator::version
	int32_t ___version_2;
	// T System.Collections.Generic.List`1_Enumerator::current
	LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  ___current_3;

public:
	inline static int32_t get_offset_of_list_0() { return static_cast<int32_t>(offsetof(Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE, ___list_0)); }
	inline List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 * get_list_0() const { return ___list_0; }
	inline List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 ** get_address_of_list_0() { return &___list_0; }
	inline void set_list_0(List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 * value)
	{
		___list_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___list_0), (void*)value);
	}

	inline static int32_t get_offset_of_index_1() { return static_cast<int32_t>(offsetof(Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE, ___index_1)); }
	inline int32_t get_index_1() const { return ___index_1; }
	inline int32_t* get_address_of_index_1() { return &___index_1; }
	inline void set_index_1(int32_t value)
	{
		___index_1 = value;
	}

	inline static int32_t get_offset_of_version_2() { return static_cast<int32_t>(offsetof(Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE, ___version_2)); }
	inline int32_t get_version_2() const { return ___version_2; }
	inline int32_t* get_address_of_version_2() { return &___version_2; }
	inline void set_version_2(int32_t value)
	{
		___version_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE, ___current_3)); }
	inline LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  get_current_3() const { return ___current_3; }
	inline LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 * get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  value)
	{
		___current_3 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___current_3))->___Message_1), (void*)NULL);
	}
};


// System.Func`2<System.Int32,System.Boolean>
struct  Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274  : public MulticastDelegate_t
{
public:

public:
};


// System.Func`2<System.Int32,System.Int32>
struct  Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA  : public MulticastDelegate_t
{
public:

public:
};


// System.Func`2<System.Int32,System.Object>
struct  Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123  : public MulticastDelegate_t
{
public:

public:
};


// System.Func`2<System.Object,System.Boolean>
struct  Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8  : public MulticastDelegate_t
{
public:

public:
};


// System.Func`2<System.Object,System.Object>
struct  Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436  : public MulticastDelegate_t
{
public:

public:
};


// System.Func`2<Unity.Entities.Conversion.LogEventData,System.Boolean>
struct  Func_2_t15BD356B2F637699370FD7109071A37617770BBA  : public MulticastDelegate_t
{
public:

public:
};


// System.Func`2<Unity.Entities.Conversion.LogEventData,System.Object>
struct  Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154  : public MulticastDelegate_t
{
public:

public:
};


// System.Func`2<Unity.Entities.Entity,System.Boolean>
struct  Func_2_t14BB53D120BF18F218ACE746215828AC2863F843  : public MulticastDelegate_t
{
public:

public:
};


// System.Func`2<Unity.Entities.Entity,System.Object>
struct  Func_2_t895537CD65D26801427B03E05DD08125DE819919  : public MulticastDelegate_t
{
public:

public:
};


// System.Func`2<UnityEngine.Color,System.Boolean>
struct  Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD  : public MulticastDelegate_t
{
public:

public:
};


// System.Func`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,System.Boolean>
struct  Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A  : public MulticastDelegate_t
{
public:

public:
};


// System.Func`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>
struct  Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64  : public MulticastDelegate_t
{
public:

public:
};


// System.Func`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>
struct  Func_2_tA55660D7B36BC919063457215A12594F309CFDF1  : public MulticastDelegate_t
{
public:

public:
};


// System.Func`2<UnityEngine.Vector3,System.Boolean>
struct  Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269  : public MulticastDelegate_t
{
public:

public:
};


// System.Linq.Enumerable_WhereSelectListIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>
struct  WhereSelectListIterator_2_t08B768B4AD54F201B800C51471E1C1C5FD1A8E8C  : public Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76
{
public:
	// System.Collections.Generic.List`1<TSource> System.Linq.Enumerable_WhereSelectListIterator`2::source
	List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectListIterator`2::predicate
	Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectListIterator`2::selector
	Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * ___selector_5;
	// System.Collections.Generic.List`1_Enumerator<TSource> System.Linq.Enumerable_WhereSelectListIterator`2::enumerator
	Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11  ___enumerator_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t08B768B4AD54F201B800C51471E1C1C5FD1A8E8C, ___source_3)); }
	inline List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * get_source_3() const { return ___source_3; }
	inline List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C ** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t08B768B4AD54F201B800C51471E1C1C5FD1A8E8C, ___predicate_4)); }
	inline Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t08B768B4AD54F201B800C51471E1C1C5FD1A8E8C, ___selector_5)); }
	inline Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * get_selector_5() const { return ___selector_5; }
	inline Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_6() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t08B768B4AD54F201B800C51471E1C1C5FD1A8E8C, ___enumerator_6)); }
	inline Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11  get_enumerator_6() const { return ___enumerator_6; }
	inline Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 * get_address_of_enumerator_6() { return &___enumerator_6; }
	inline void set_enumerator_6(Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11  value)
	{
		___enumerator_6 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___enumerator_6))->___list_0), (void*)NULL);
		#if IL2CPP_ENABLE_STRICT_WRITE_BARRIERS
		Il2CppCodeGenWriteBarrier((void**)&((&(((&___enumerator_6))->___current_3))->___Data_1), (void*)NULL);
		#endif
	}
};


// System.Linq.Enumerable_WhereSelectListIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>
struct  WhereSelectListIterator_2_t94F415FEA624816E42FAE1274A4EC29F5D5C1625  : public Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF
{
public:
	// System.Collections.Generic.List`1<TSource> System.Linq.Enumerable_WhereSelectListIterator`2::source
	List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectListIterator`2::predicate
	Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectListIterator`2::selector
	Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * ___selector_5;
	// System.Collections.Generic.List`1_Enumerator<TSource> System.Linq.Enumerable_WhereSelectListIterator`2::enumerator
	Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11  ___enumerator_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t94F415FEA624816E42FAE1274A4EC29F5D5C1625, ___source_3)); }
	inline List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * get_source_3() const { return ___source_3; }
	inline List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C ** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t94F415FEA624816E42FAE1274A4EC29F5D5C1625, ___predicate_4)); }
	inline Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t94F415FEA624816E42FAE1274A4EC29F5D5C1625, ___selector_5)); }
	inline Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * get_selector_5() const { return ___selector_5; }
	inline Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_6() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_t94F415FEA624816E42FAE1274A4EC29F5D5C1625, ___enumerator_6)); }
	inline Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11  get_enumerator_6() const { return ___enumerator_6; }
	inline Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 * get_address_of_enumerator_6() { return &___enumerator_6; }
	inline void set_enumerator_6(Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11  value)
	{
		___enumerator_6 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___enumerator_6))->___list_0), (void*)NULL);
		#if IL2CPP_ENABLE_STRICT_WRITE_BARRIERS
		Il2CppCodeGenWriteBarrier((void**)&((&(((&___enumerator_6))->___current_3))->___Data_1), (void*)NULL);
		#endif
	}
};


// System.ArgumentNullException
struct  ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB  : public ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00
{
public:

public:
};


// System.Linq.Enumerable_WhereSelectListIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>
struct  WhereSelectListIterator_2_tCD2B0DD2D1D32234F3640C8D7B5C9C212AAAD6E4  : public Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279
{
public:
	// System.Collections.Generic.List`1<TSource> System.Linq.Enumerable_WhereSelectListIterator`2::source
	List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 * ___source_3;
	// System.Func`2<TSource,System.Boolean> System.Linq.Enumerable_WhereSelectListIterator`2::predicate
	Func_2_t15BD356B2F637699370FD7109071A37617770BBA * ___predicate_4;
	// System.Func`2<TSource,TResult> System.Linq.Enumerable_WhereSelectListIterator`2::selector
	Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * ___selector_5;
	// System.Collections.Generic.List`1_Enumerator<TSource> System.Linq.Enumerable_WhereSelectListIterator`2::enumerator
	Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE  ___enumerator_6;

public:
	inline static int32_t get_offset_of_source_3() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_tCD2B0DD2D1D32234F3640C8D7B5C9C212AAAD6E4, ___source_3)); }
	inline List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 * get_source_3() const { return ___source_3; }
	inline List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 ** get_address_of_source_3() { return &___source_3; }
	inline void set_source_3(List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 * value)
	{
		___source_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___source_3), (void*)value);
	}

	inline static int32_t get_offset_of_predicate_4() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_tCD2B0DD2D1D32234F3640C8D7B5C9C212AAAD6E4, ___predicate_4)); }
	inline Func_2_t15BD356B2F637699370FD7109071A37617770BBA * get_predicate_4() const { return ___predicate_4; }
	inline Func_2_t15BD356B2F637699370FD7109071A37617770BBA ** get_address_of_predicate_4() { return &___predicate_4; }
	inline void set_predicate_4(Func_2_t15BD356B2F637699370FD7109071A37617770BBA * value)
	{
		___predicate_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___predicate_4), (void*)value);
	}

	inline static int32_t get_offset_of_selector_5() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_tCD2B0DD2D1D32234F3640C8D7B5C9C212AAAD6E4, ___selector_5)); }
	inline Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * get_selector_5() const { return ___selector_5; }
	inline Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 ** get_address_of_selector_5() { return &___selector_5; }
	inline void set_selector_5(Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * value)
	{
		___selector_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___selector_5), (void*)value);
	}

	inline static int32_t get_offset_of_enumerator_6() { return static_cast<int32_t>(offsetof(WhereSelectListIterator_2_tCD2B0DD2D1D32234F3640C8D7B5C9C212AAAD6E4, ___enumerator_6)); }
	inline Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE  get_enumerator_6() const { return ___enumerator_6; }
	inline Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE * get_address_of_enumerator_6() { return &___enumerator_6; }
	inline void set_enumerator_6(Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE  value)
	{
		___enumerator_6 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___enumerator_6))->___list_0), (void*)NULL);
		#if IL2CPP_ENABLE_STRICT_WRITE_BARRIERS
		Il2CppCodeGenWriteBarrier((void**)&((&(((&___enumerator_6))->___current_3))->___Message_1), (void*)NULL);
		#endif
	}
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
// UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex[]
struct ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  m_Items[1];

public:
	inline ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 * GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)&((m_Items + index)->___Data_1), (void*)NULL);
	}
	inline ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 * GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)&((m_Items + index)->___Data_1), (void*)NULL);
	}
};
// Unity.Entities.Entity[]
struct EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  m_Items[1];

public:
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 * GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  value)
	{
		m_Items[index] = value;
	}
};
// System.Int32[]
struct Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) int32_t m_Items[1];

public:
	inline int32_t GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline int32_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, int32_t value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline int32_t GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline int32_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, int32_t value)
	{
		m_Items[index] = value;
	}
};
// Unity.Entities.Conversion.LogEventData[]
struct LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  m_Items[1];

public:
	inline LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 * GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)&((m_Items + index)->___Message_1), (void*)NULL);
	}
	inline LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 * GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)&((m_Items + index)->___Message_1), (void*)NULL);
	}
};


// System.Void System.ValueTuple`3<System.Object,System.Object,System.Object>::.ctor(T1,T2,T3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ValueTuple_3__ctor_m8868D9B867F41FEEDC7C736B39DAB0889188EC78_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___item10, RuntimeObject * ___item21, RuntimeObject * ___item32, const RuntimeMethod* method);
// System.Boolean System.ValueTuple`3<System.Object,System.Object,System.Object>::Equals(System.ValueTuple`3<T1,T2,T3>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ValueTuple_3_Equals_mD5979E2C620E3606530CD15B27D8BC4750CD37A5_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  ___other0, const RuntimeMethod* method);
// System.Boolean System.ValueTuple`3<System.Object,System.Object,System.Object>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ValueTuple_3_Equals_m3C24A212A23EBE783C3B3C61B0F5E45334DF6AB1_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Boolean System.ValueTuple`3<System.Object,System.Object,System.Object>::System.Collections.IStructuralEquatable.Equals(System.Object,System.Collections.IEqualityComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m8D1BD92383DC97A390119BEE2ABAA659470A3252_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::CompareTo(System.ValueTuple`3<T1,T2,T3>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_CompareTo_mCEB32C7146D5A53C51A72678D3F152EBB02E0495_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  ___other0, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::System.IComparable.CompareTo(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_System_IComparable_CompareTo_m565E54913F6D87E2DE47D69286058547E5C28155_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___other0, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::System.Collections.IStructuralComparable.CompareTo(System.Object,System.Collections.IComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m7233DE35FC3952A4832494EDCA1F3F2681AE09FE_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_GetHashCode_m7484E3361C746C8B62359592753CDFD38BCC2E38_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::GetHashCodeCore(System.Collections.IEqualityComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_GetHashCodeCore_m67CB7531D334ACBE07696D3CFC5DB57C4738F3C4_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_System_Collections_IStructuralEquatable_GetHashCode_mD601062CE782B8EDA3A9AEBD2DE16AC0423B61FE_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method);
// System.String System.ValueTuple`3<System.Object,System.Object,System.Object>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* ValueTuple_3_ToString_mE918A7F63C8F4AD1C4BCF5C2F28D048307DBB1BF_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, const RuntimeMethod* method);
// System.Void System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::.ctor(T1,T2,T3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ValueTuple_3__ctor_mCB5A2C8A72952508684BA5B641486528A773A670_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___item10, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___item21, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___item32, const RuntimeMethod* method);
// System.Boolean System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::Equals(System.ValueTuple`3<T1,T2,T3>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ValueTuple_3_Equals_m7F0287ACB5F228FE2C345AC086983A43BEF3B710_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  ___other0, const RuntimeMethod* method);
// System.Boolean System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ValueTuple_3_Equals_m3D96E39FD82E7C0FF90956B59AC55794A1F2D724_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Boolean System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::System.Collections.IStructuralEquatable.Equals(System.Object,System.Collections.IEqualityComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m2B4EB2E18EAA9D66C46192CAFA91BD15AAAEF78E_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::CompareTo(System.ValueTuple`3<T1,T2,T3>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_CompareTo_m82FF5B1B9AFC3BC475D59DB9BCC3948E8B6737B5_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  ___other0, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::System.IComparable.CompareTo(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_System_IComparable_CompareTo_mDCBC6504198CCF4FD5A11911B785F625602F0A5A_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject * ___other0, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::System.Collections.IStructuralComparable.CompareTo(System.Object,System.Collections.IComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m8F9A1A82E5960A5A167B7B61237919AC9EA86869_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_GetHashCode_mD7AC1A1D2BFFD62FC7C766D1595076CE613BFC4E_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::GetHashCodeCore(System.Collections.IEqualityComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_GetHashCodeCore_m6D7023B397C18B2B04D077227DDC88F0E0282731_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_System_Collections_IStructuralEquatable_GetHashCode_mBC40DB422DFF4D33476C79DE080198048A6A167F_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method);
// System.String System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* ValueTuple_3_ToString_m2F574559A50EDFB3E99D4CCD1B85A772BB84798F_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.List`1/Enumerator<System.Object>::get_Current()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject * Enumerator_get_Current_m9C4EBBD2108B51885E750F927D7936290C8E20EE_gshared_inline (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.List`1/Enumerator<System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Enumerator_MoveNext_m2E56233762839CE55C67E00AC8DD3D4D3F6C0DF0_gshared (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.List`1/Enumerator<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex>::get_Current()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  Enumerator_get_Current_mB9DED66EBA82669AB83832B40F60E1710B5179B4_gshared_inline (Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.List`1/Enumerator<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Enumerator_MoveNext_mACDC401A875ECF83AEF9477068CDF02545A1D997_gshared (Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.List`1/Enumerator<Unity.Entities.Entity>::get_Current()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  Enumerator_get_Current_m5478E2379F0D2D42C6D2FAF5C9B3297298D5BFC8_gshared_inline (Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.List`1/Enumerator<Unity.Entities.Entity>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Enumerator_MoveNext_m2505BC1BC82C7BB3A2DE21A19BA41FB8CA521671_gshared (Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.List`1/Enumerator<System.Int32>::get_Current()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t Enumerator_get_Current_m6BBD624C51F7E20D347FE5894A6ECA94B8011181_gshared_inline (Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.List`1/Enumerator<System.Int32>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Enumerator_MoveNext_m40FD166B6757334A2BBCF67238EFDF70D727A4A6_gshared (Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.List`1/Enumerator<Unity.Entities.Conversion.LogEventData>::get_Current()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  Enumerator_get_Current_mAB62DE0E08E4774E60465C9247E8B7A1E1831223_gshared_inline (Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.List`1/Enumerator<Unity.Entities.Conversion.LogEventData>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Enumerator_MoveNext_m7A7F9EE8CA53E7533A04E8E65B8BF08A2C88A34B_gshared (Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE * __this, const RuntimeMethod* method);

// System.Void System.ValueTuple`3<System.Object,System.Object,System.Object>::.ctor(T1,T2,T3)
inline void ValueTuple_3__ctor_m8868D9B867F41FEEDC7C736B39DAB0889188EC78 (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___item10, RuntimeObject * ___item21, RuntimeObject * ___item32, const RuntimeMethod* method)
{
	((  void (*) (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *, RuntimeObject *, RuntimeObject *, RuntimeObject *, const RuntimeMethod*))ValueTuple_3__ctor_m8868D9B867F41FEEDC7C736B39DAB0889188EC78_gshared)(__this, ___item10, ___item21, ___item32, method);
}
// System.Boolean System.ValueTuple`3<System.Object,System.Object,System.Object>::Equals(System.ValueTuple`3<T1,T2,T3>)
inline bool ValueTuple_3_Equals_mD5979E2C620E3606530CD15B27D8BC4750CD37A5 (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  ___other0, const RuntimeMethod* method)
{
	return ((  bool (*) (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *, ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D , const RuntimeMethod*))ValueTuple_3_Equals_mD5979E2C620E3606530CD15B27D8BC4750CD37A5_gshared)(__this, ___other0, method);
}
// System.Boolean System.ValueTuple`3<System.Object,System.Object,System.Object>::Equals(System.Object)
inline bool ValueTuple_3_Equals_m3C24A212A23EBE783C3B3C61B0F5E45334DF6AB1 (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	return ((  bool (*) (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *, RuntimeObject *, const RuntimeMethod*))ValueTuple_3_Equals_m3C24A212A23EBE783C3B3C61B0F5E45334DF6AB1_gshared)(__this, ___obj0, method);
}
// System.Boolean System.ValueTuple`3<System.Object,System.Object,System.Object>::System.Collections.IStructuralEquatable.Equals(System.Object,System.Collections.IEqualityComparer)
inline bool ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m8D1BD92383DC97A390119BEE2ABAA659470A3252 (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method)
{
	return ((  bool (*) (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *, RuntimeObject *, RuntimeObject*, const RuntimeMethod*))ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m8D1BD92383DC97A390119BEE2ABAA659470A3252_gshared)(__this, ___other0, ___comparer1, method);
}
// System.Type System.Object::GetType()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Type_t * Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B (RuntimeObject * __this, const RuntimeMethod* method);
// System.String SR::Format(System.String,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* SR_Format_m942E78AC3ABE13F58075ED90094D6074CA5A7DC8 (String_t* ___resourceFormat0, RuntimeObject * ___p11, const RuntimeMethod* method);
// System.Void System.ArgumentException::.ctor(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ArgumentException__ctor_m71044C2110E357B71A1C30D2561C3F861AF1DC0D (ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 * __this, String_t* ___message0, String_t* ___paramName1, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::CompareTo(System.ValueTuple`3<T1,T2,T3>)
inline int32_t ValueTuple_3_CompareTo_mCEB32C7146D5A53C51A72678D3F152EBB02E0495 (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  ___other0, const RuntimeMethod* method)
{
	return ((  int32_t (*) (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *, ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D , const RuntimeMethod*))ValueTuple_3_CompareTo_mCEB32C7146D5A53C51A72678D3F152EBB02E0495_gshared)(__this, ___other0, method);
}
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::System.IComparable.CompareTo(System.Object)
inline int32_t ValueTuple_3_System_IComparable_CompareTo_m565E54913F6D87E2DE47D69286058547E5C28155 (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___other0, const RuntimeMethod* method)
{
	return ((  int32_t (*) (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *, RuntimeObject *, const RuntimeMethod*))ValueTuple_3_System_IComparable_CompareTo_m565E54913F6D87E2DE47D69286058547E5C28155_gshared)(__this, ___other0, method);
}
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::System.Collections.IStructuralComparable.CompareTo(System.Object,System.Collections.IComparer)
inline int32_t ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m7233DE35FC3952A4832494EDCA1F3F2681AE09FE (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method)
{
	return ((  int32_t (*) (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *, RuntimeObject *, RuntimeObject*, const RuntimeMethod*))ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m7233DE35FC3952A4832494EDCA1F3F2681AE09FE_gshared)(__this, ___other0, ___comparer1, method);
}
// System.Int32 System.ValueTuple::CombineHashCodes(System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_CombineHashCodes_m6D3138F0BA3D04CA1B640620E47716F05EB2DEBE (int32_t ___h10, int32_t ___h21, int32_t ___h32, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::GetHashCode()
inline int32_t ValueTuple_3_GetHashCode_m7484E3361C746C8B62359592753CDFD38BCC2E38 (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *, const RuntimeMethod*))ValueTuple_3_GetHashCode_m7484E3361C746C8B62359592753CDFD38BCC2E38_gshared)(__this, method);
}
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::GetHashCodeCore(System.Collections.IEqualityComparer)
inline int32_t ValueTuple_3_GetHashCodeCore_m67CB7531D334ACBE07696D3CFC5DB57C4738F3C4 (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method)
{
	return ((  int32_t (*) (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *, RuntimeObject*, const RuntimeMethod*))ValueTuple_3_GetHashCodeCore_m67CB7531D334ACBE07696D3CFC5DB57C4738F3C4_gshared)(__this, ___comparer0, method);
}
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer)
inline int32_t ValueTuple_3_System_Collections_IStructuralEquatable_GetHashCode_mD601062CE782B8EDA3A9AEBD2DE16AC0423B61FE (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method)
{
	return ((  int32_t (*) (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *, RuntimeObject*, const RuntimeMethod*))ValueTuple_3_System_Collections_IStructuralEquatable_GetHashCode_mD601062CE782B8EDA3A9AEBD2DE16AC0423B61FE_gshared)(__this, ___comparer0, method);
}
// System.String System.String::Concat(System.String[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_mFEA7EFA1A6E75B96B1B7BC4526AAC864BFF83CC9 (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* ___values0, const RuntimeMethod* method);
// System.String System.ValueTuple`3<System.Object,System.Object,System.Object>::ToString()
inline String_t* ValueTuple_3_ToString_mE918A7F63C8F4AD1C4BCF5C2F28D048307DBB1BF (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, const RuntimeMethod* method)
{
	return ((  String_t* (*) (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *, const RuntimeMethod*))ValueTuple_3_ToString_mE918A7F63C8F4AD1C4BCF5C2F28D048307DBB1BF_gshared)(__this, method);
}
// System.Void System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::.ctor(T1,T2,T3)
inline void ValueTuple_3__ctor_mCB5A2C8A72952508684BA5B641486528A773A670 (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___item10, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___item21, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___item32, const RuntimeMethod* method)
{
	((  void (*) (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , const RuntimeMethod*))ValueTuple_3__ctor_mCB5A2C8A72952508684BA5B641486528A773A670_gshared)(__this, ___item10, ___item21, ___item32, method);
}
// System.Boolean System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::Equals(System.ValueTuple`3<T1,T2,T3>)
inline bool ValueTuple_3_Equals_m7F0287ACB5F228FE2C345AC086983A43BEF3B710 (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  ___other0, const RuntimeMethod* method)
{
	return ((  bool (*) (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *, ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 , const RuntimeMethod*))ValueTuple_3_Equals_m7F0287ACB5F228FE2C345AC086983A43BEF3B710_gshared)(__this, ___other0, method);
}
// System.Boolean System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::Equals(System.Object)
inline bool ValueTuple_3_Equals_m3D96E39FD82E7C0FF90956B59AC55794A1F2D724 (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	return ((  bool (*) (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *, RuntimeObject *, const RuntimeMethod*))ValueTuple_3_Equals_m3D96E39FD82E7C0FF90956B59AC55794A1F2D724_gshared)(__this, ___obj0, method);
}
// System.Boolean System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::System.Collections.IStructuralEquatable.Equals(System.Object,System.Collections.IEqualityComparer)
inline bool ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m2B4EB2E18EAA9D66C46192CAFA91BD15AAAEF78E (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method)
{
	return ((  bool (*) (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *, RuntimeObject *, RuntimeObject*, const RuntimeMethod*))ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m2B4EB2E18EAA9D66C46192CAFA91BD15AAAEF78E_gshared)(__this, ___other0, ___comparer1, method);
}
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::CompareTo(System.ValueTuple`3<T1,T2,T3>)
inline int32_t ValueTuple_3_CompareTo_m82FF5B1B9AFC3BC475D59DB9BCC3948E8B6737B5 (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  ___other0, const RuntimeMethod* method)
{
	return ((  int32_t (*) (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *, ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 , const RuntimeMethod*))ValueTuple_3_CompareTo_m82FF5B1B9AFC3BC475D59DB9BCC3948E8B6737B5_gshared)(__this, ___other0, method);
}
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::System.IComparable.CompareTo(System.Object)
inline int32_t ValueTuple_3_System_IComparable_CompareTo_mDCBC6504198CCF4FD5A11911B785F625602F0A5A (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject * ___other0, const RuntimeMethod* method)
{
	return ((  int32_t (*) (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *, RuntimeObject *, const RuntimeMethod*))ValueTuple_3_System_IComparable_CompareTo_mDCBC6504198CCF4FD5A11911B785F625602F0A5A_gshared)(__this, ___other0, method);
}
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::System.Collections.IStructuralComparable.CompareTo(System.Object,System.Collections.IComparer)
inline int32_t ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m8F9A1A82E5960A5A167B7B61237919AC9EA86869 (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method)
{
	return ((  int32_t (*) (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *, RuntimeObject *, RuntimeObject*, const RuntimeMethod*))ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m8F9A1A82E5960A5A167B7B61237919AC9EA86869_gshared)(__this, ___other0, ___comparer1, method);
}
// System.Int32 UnityEngine.Vector4::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Vector4_GetHashCode_mCA7B312F8CA141F6F25BABDDF406F3D2BDD5E895 (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * __this, const RuntimeMethod* method);
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::GetHashCode()
inline int32_t ValueTuple_3_GetHashCode_mD7AC1A1D2BFFD62FC7C766D1595076CE613BFC4E (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *, const RuntimeMethod*))ValueTuple_3_GetHashCode_mD7AC1A1D2BFFD62FC7C766D1595076CE613BFC4E_gshared)(__this, method);
}
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::GetHashCodeCore(System.Collections.IEqualityComparer)
inline int32_t ValueTuple_3_GetHashCodeCore_m6D7023B397C18B2B04D077227DDC88F0E0282731 (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method)
{
	return ((  int32_t (*) (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *, RuntimeObject*, const RuntimeMethod*))ValueTuple_3_GetHashCodeCore_m6D7023B397C18B2B04D077227DDC88F0E0282731_gshared)(__this, ___comparer0, method);
}
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer)
inline int32_t ValueTuple_3_System_Collections_IStructuralEquatable_GetHashCode_mBC40DB422DFF4D33476C79DE080198048A6A167F (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method)
{
	return ((  int32_t (*) (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *, RuntimeObject*, const RuntimeMethod*))ValueTuple_3_System_Collections_IStructuralEquatable_GetHashCode_mBC40DB422DFF4D33476C79DE080198048A6A167F_gshared)(__this, ___comparer0, method);
}
// System.String UnityEngine.Vector4::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Vector4_ToString_mF2D17142EBD75E91BC718B3E347F614AC45E9040 (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * __this, const RuntimeMethod* method);
// System.String System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::ToString()
inline String_t* ValueTuple_3_ToString_m2F574559A50EDFB3E99D4CCD1B85A772BB84798F (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, const RuntimeMethod* method)
{
	return ((  String_t* (*) (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *, const RuntimeMethod*))ValueTuple_3_ToString_m2F574559A50EDFB3E99D4CCD1B85A772BB84798F_gshared)(__this, method);
}
// System.Void UnityEngine.Rendering.VolumeParameter::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter__ctor_m0F5E882EFAE8F1B7AE0DFF62945878D71CA63D54 (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * __this, const RuntimeMethod* method);
// System.Int32 System.Boolean::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Boolean_GetHashCode_m03AF8B3CECAE9106C44A00E3B33E51CBFC45C411 (bool* __this, const RuntimeMethod* method);
// System.String System.String::Format(System.String,System.Object,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Format_m8D1CB0410C35E052A53AE957C914C841E54BAB66 (String_t* ___format0, RuntimeObject * ___arg01, RuntimeObject * ___arg12, const RuntimeMethod* method);
// System.Boolean System.Boolean::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Boolean_Equals_mA2FC01AF136159906F30A85C950097BE67C824B8 (bool* __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Boolean System.Type::op_Inequality(System.Type,System.Type)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Type_op_Inequality_m6DDC5E923203A79BF505F9275B694AD3FAA36DB0 (Type_t * ___left0, Type_t * ___right1, const RuntimeMethod* method);
// System.Int32 UnityEngine.Color::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Color_GetHashCode_mAF5E7EE6AFA983D3FA5E3D316E672EE1511F97CF (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Color::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Color_Equals_m90F8A5EF85416D809F5E3C0ACCADDD4F299AD8FC (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 * __this, RuntimeObject * ___other0, const RuntimeMethod* method);
// System.Int32 System.Int32::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Int32_GetHashCode_mEDD3F492A5F7CF021125AE3F38E2B8F8743FC667 (int32_t* __this, const RuntimeMethod* method);
// System.Boolean System.Int32::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Int32_Equals_m5F032BFC65C340C98050D3DF2D76101914774464 (int32_t* __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Int32 System.Single::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Single_GetHashCode_m7662E1812DDDBC85D464398740CFFC3588DFB2C9 (float* __this, const RuntimeMethod* method);
// System.Boolean System.Single::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Single_Equals_m94AA41817D00A9347BD3565F6BB8993361B81EB1 (float* __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Int32 UnityEngine.Vector2::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Vector2_GetHashCode_m9A5DD8406289F38806CC42C394E324C1C2AB3732 (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Vector2::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Vector2_Equals_m67A842D914AA5A4DCC076E9EA20019925E6A85A0 (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 * __this, RuntimeObject * ___other0, const RuntimeMethod* method);
// System.Int32 UnityEngine.Vector3::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Vector3_GetHashCode_m9F18401DA6025110A012F55BBB5ACABD36FA9A0A (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Vector3::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Vector3_Equals_m210CB160B594355581D44D4B87CF3D3994ABFED0 (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E * __this, RuntimeObject * ___other0, const RuntimeMethod* method);
// System.Boolean UnityEngine.Vector4::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Vector4_Equals_m71D14F39651C3FBEDE17214455DFA727921F07AA (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * __this, RuntimeObject * ___other0, const RuntimeMethod* method);
// System.Void System.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Runtime.InteropServices.GCHandle System.Runtime.InteropServices.GCHandle::Alloc(System.Object,System.Runtime.InteropServices.GCHandleType)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603  GCHandle_Alloc_mBF5C4C0E8605F97427BFDF96D68AACD4A4D6DDEC (RuntimeObject * ___value0, int32_t ___type1, const RuntimeMethod* method);
// System.Void System.ArgumentNullException::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ArgumentNullException__ctor_m81AB157B93BFE2FBFDB08B88F84B444293042F97 (ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB * __this, String_t* ___paramName0, const RuntimeMethod* method);
// System.Boolean System.Runtime.Serialization.SerializationInfo::GetBoolean(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SerializationInfo_GetBoolean_m705ADACFB52D6385DDB6B2525C1979ECBE6D5849 (SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 * __this, String_t* ___name0, const RuntimeMethod* method);
// System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Type_t * Type_GetTypeFromHandle_m8BB57524FF7F9DB1803BC561D2B3A4DBACEB385E (RuntimeTypeHandle_tC33965ADA3E041E0C94AF05E5CB527B56482CEF9  ___handle0, const RuntimeMethod* method);
// System.Object System.Runtime.Serialization.SerializationInfo::GetValue(System.String,System.Type)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * SerializationInfo_GetValue_mF6E311779D55AD7C80B2D19FF2A7E9683AEF2A99 (SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 * __this, String_t* ___name0, Type_t * ___type1, const RuntimeMethod* method);
// System.Void System.Runtime.Serialization.SerializationInfo::AddValue(System.String,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SerializationInfo_AddValue_m324F3E0B02B746D5F460499F5A25988FD608AD7B (SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 * __this, String_t* ___name0, bool ___value1, const RuntimeMethod* method);
// System.Boolean System.Runtime.InteropServices.GCHandle::get_IsAllocated()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GCHandle_get_IsAllocated_mEDA4DAC6AD6D881110E96CAFDAB78C068F5B144D (GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 * __this, const RuntimeMethod* method);
// System.Object System.Runtime.InteropServices.GCHandle::get_Target()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * GCHandle_get_Target_m6C296AD6520ECDAFC9498E9387677F522871F883 (GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 * __this, const RuntimeMethod* method);
// System.Void System.Runtime.Serialization.SerializationInfo::AddValue(System.String,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SerializationInfo_AddValue_mA50C2668EF700C2239DDC362F8DB409020BB763D (SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 * __this, String_t* ___name0, RuntimeObject * ___value1, const RuntimeMethod* method);
// System.Void System.Runtime.InteropServices.GCHandle::Free()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GCHandle_Free_mB4E9415544FC9F0075C02AB17E270E49AF006025 (GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 * __this, const RuntimeMethod* method);
// System.Void System.Object::Finalize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object_Finalize_mC59C83CF4F7707E425FFA6362931C25D4C36676A (RuntimeObject * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.List`1/Enumerator<System.Object>::get_Current()
inline RuntimeObject * Enumerator_get_Current_m9C4EBBD2108B51885E750F927D7936290C8E20EE_inline (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * __this, const RuntimeMethod* method)
{
	return ((  RuntimeObject * (*) (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 *, const RuntimeMethod*))Enumerator_get_Current_m9C4EBBD2108B51885E750F927D7936290C8E20EE_gshared_inline)(__this, method);
}
// System.Boolean System.Collections.Generic.List`1/Enumerator<System.Object>::MoveNext()
inline bool Enumerator_MoveNext_m2E56233762839CE55C67E00AC8DD3D4D3F6C0DF0 (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 *, const RuntimeMethod*))Enumerator_MoveNext_m2E56233762839CE55C67E00AC8DD3D4D3F6C0DF0_gshared)(__this, method);
}
// !0 System.Collections.Generic.List`1/Enumerator<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex>::get_Current()
inline ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  Enumerator_get_Current_mB9DED66EBA82669AB83832B40F60E1710B5179B4_inline (Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 * __this, const RuntimeMethod* method)
{
	return ((  ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  (*) (Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 *, const RuntimeMethod*))Enumerator_get_Current_mB9DED66EBA82669AB83832B40F60E1710B5179B4_gshared_inline)(__this, method);
}
// System.Boolean System.Collections.Generic.List`1/Enumerator<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex>::MoveNext()
inline bool Enumerator_MoveNext_mACDC401A875ECF83AEF9477068CDF02545A1D997 (Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 *, const RuntimeMethod*))Enumerator_MoveNext_mACDC401A875ECF83AEF9477068CDF02545A1D997_gshared)(__this, method);
}
// !0 System.Collections.Generic.List`1/Enumerator<Unity.Entities.Entity>::get_Current()
inline Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  Enumerator_get_Current_m5478E2379F0D2D42C6D2FAF5C9B3297298D5BFC8_inline (Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 * __this, const RuntimeMethod* method)
{
	return ((  Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  (*) (Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 *, const RuntimeMethod*))Enumerator_get_Current_m5478E2379F0D2D42C6D2FAF5C9B3297298D5BFC8_gshared_inline)(__this, method);
}
// System.Boolean System.Collections.Generic.List`1/Enumerator<Unity.Entities.Entity>::MoveNext()
inline bool Enumerator_MoveNext_m2505BC1BC82C7BB3A2DE21A19BA41FB8CA521671 (Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 *, const RuntimeMethod*))Enumerator_MoveNext_m2505BC1BC82C7BB3A2DE21A19BA41FB8CA521671_gshared)(__this, method);
}
// !0 System.Collections.Generic.List`1/Enumerator<System.Int32>::get_Current()
inline int32_t Enumerator_get_Current_m6BBD624C51F7E20D347FE5894A6ECA94B8011181_inline (Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C *, const RuntimeMethod*))Enumerator_get_Current_m6BBD624C51F7E20D347FE5894A6ECA94B8011181_gshared_inline)(__this, method);
}
// System.Boolean System.Collections.Generic.List`1/Enumerator<System.Int32>::MoveNext()
inline bool Enumerator_MoveNext_m40FD166B6757334A2BBCF67238EFDF70D727A4A6 (Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C *, const RuntimeMethod*))Enumerator_MoveNext_m40FD166B6757334A2BBCF67238EFDF70D727A4A6_gshared)(__this, method);
}
// !0 System.Collections.Generic.List`1/Enumerator<Unity.Entities.Conversion.LogEventData>::get_Current()
inline LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  Enumerator_get_Current_mAB62DE0E08E4774E60465C9247E8B7A1E1831223_inline (Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE * __this, const RuntimeMethod* method)
{
	return ((  LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  (*) (Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE *, const RuntimeMethod*))Enumerator_get_Current_mAB62DE0E08E4774E60465C9247E8B7A1E1831223_gshared_inline)(__this, method);
}
// System.Boolean System.Collections.Generic.List`1/Enumerator<Unity.Entities.Conversion.LogEventData>::MoveNext()
inline bool Enumerator_MoveNext_m7A7F9EE8CA53E7533A04E8E65B8BF08A2C88A34B (Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE *, const RuntimeMethod*))Enumerator_MoveNext_m7A7F9EE8CA53E7533A04E8E65B8BF08A2C88A34B_gshared)(__this, method);
}
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void System.ValueTuple`3<System.Object,System.Object,System.Object>::.ctor(T1,T2,T3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ValueTuple_3__ctor_m8868D9B867F41FEEDC7C736B39DAB0889188EC78_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___item10, RuntimeObject * ___item21, RuntimeObject * ___item32, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = ___item10;
		__this->set_Item1_0(L_0);
		RuntimeObject * L_1 = ___item21;
		__this->set_Item2_1(L_1);
		RuntimeObject * L_2 = ___item32;
		__this->set_Item3_2(L_2);
		return;
	}
}
IL2CPP_EXTERN_C  void ValueTuple_3__ctor_m8868D9B867F41FEEDC7C736B39DAB0889188EC78_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___item10, RuntimeObject * ___item21, RuntimeObject * ___item32, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * _thisAdjusted = reinterpret_cast<ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *>(__this + _offset);
	ValueTuple_3__ctor_m8868D9B867F41FEEDC7C736B39DAB0889188EC78(_thisAdjusted, ___item10, ___item21, ___item32, method);
}
// System.Boolean System.ValueTuple`3<System.Object,System.Object,System.Object>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ValueTuple_3_Equals_m3C24A212A23EBE783C3B3C61B0F5E45334DF6AB1_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = ___obj0;
		if (!((RuntimeObject *)IsInst((RuntimeObject*)L_0, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0))))
		{
			goto IL_0015;
		}
	}
	{
		RuntimeObject * L_1 = ___obj0;
		bool L_2 = ValueTuple_3_Equals_mD5979E2C620E3606530CD15B27D8BC4750CD37A5((ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)(ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)__this, (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D )((*(ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)((ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)UnBox(L_1, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0))))), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 1));
		return L_2;
	}

IL_0015:
	{
		return (bool)0;
	}
}
IL2CPP_EXTERN_C  bool ValueTuple_3_Equals_m3C24A212A23EBE783C3B3C61B0F5E45334DF6AB1_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * _thisAdjusted = reinterpret_cast<ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *>(__this + _offset);
	return ValueTuple_3_Equals_m3C24A212A23EBE783C3B3C61B0F5E45334DF6AB1(_thisAdjusted, ___obj0, method);
}
// System.Boolean System.ValueTuple`3<System.Object,System.Object,System.Object>::Equals(System.ValueTuple`3<T1,T2,T3>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ValueTuple_3_Equals_mD5979E2C620E3606530CD15B27D8BC4750CD37A5_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  ___other0, const RuntimeMethod* method)
{
	{
		EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 * L_0 = ((  EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2));
		RuntimeObject * L_1 = (RuntimeObject *)__this->get_Item1_0();
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_2 = ___other0;
		RuntimeObject * L_3 = (RuntimeObject *)L_2.get_Item1_0();
		NullCheck((EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 *)L_0);
		bool L_4 = VirtFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<System.Object>::Equals(T,T) */, (EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 *)L_0, (RuntimeObject *)L_1, (RuntimeObject *)L_3);
		if (!L_4)
		{
			goto IL_0047;
		}
	}
	{
		EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 * L_5 = ((  EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 5)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 5));
		RuntimeObject * L_6 = (RuntimeObject *)__this->get_Item2_1();
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_7 = ___other0;
		RuntimeObject * L_8 = (RuntimeObject *)L_7.get_Item2_1();
		NullCheck((EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 *)L_5);
		bool L_9 = VirtFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<System.Object>::Equals(T,T) */, (EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 *)L_5, (RuntimeObject *)L_6, (RuntimeObject *)L_8);
		if (!L_9)
		{
			goto IL_0047;
		}
	}
	{
		EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 * L_10 = ((  EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		RuntimeObject * L_11 = (RuntimeObject *)__this->get_Item3_2();
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_12 = ___other0;
		RuntimeObject * L_13 = (RuntimeObject *)L_12.get_Item3_2();
		NullCheck((EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 *)L_10);
		bool L_14 = VirtFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<System.Object>::Equals(T,T) */, (EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 *)L_10, (RuntimeObject *)L_11, (RuntimeObject *)L_13);
		return L_14;
	}

IL_0047:
	{
		return (bool)0;
	}
}
IL2CPP_EXTERN_C  bool ValueTuple_3_Equals_mD5979E2C620E3606530CD15B27D8BC4750CD37A5_AdjustorThunk (RuntimeObject * __this, ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * _thisAdjusted = reinterpret_cast<ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *>(__this + _offset);
	return ValueTuple_3_Equals_mD5979E2C620E3606530CD15B27D8BC4750CD37A5(_thisAdjusted, ___other0, method);
}
// System.Boolean System.ValueTuple`3<System.Object,System.Object,System.Object>::System.Collections.IStructuralEquatable.Equals(System.Object,System.Collections.IEqualityComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m8D1BD92383DC97A390119BEE2ABAA659470A3252_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m8D1BD92383DC97A390119BEE2ABAA659470A3252_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		RuntimeObject * L_0 = ___other0;
		if (!L_0)
		{
			goto IL_000b;
		}
	}
	{
		RuntimeObject * L_1 = ___other0;
		if (((RuntimeObject *)IsInst((RuntimeObject*)L_1, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0))))
		{
			goto IL_000d;
		}
	}

IL_000b:
	{
		return (bool)0;
	}

IL_000d:
	{
		RuntimeObject * L_2 = ___other0;
		V_0 = (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D )((*(ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)((ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)UnBox(L_2, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0)))));
		RuntimeObject* L_3 = ___comparer1;
		RuntimeObject * L_4 = (RuntimeObject *)__this->get_Item1_0();
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_5 = V_0;
		RuntimeObject * L_6 = (RuntimeObject *)L_5.get_Item1_0();
		NullCheck((RuntimeObject*)L_3);
		bool L_7 = InterfaceFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(0 /* System.Boolean System.Collections.IEqualityComparer::Equals(System.Object,System.Object) */, IEqualityComparer_t6C4C1F04B21BDE1E4B84BD6EC7DE494C186D6C68_il2cpp_TypeInfo_var, (RuntimeObject*)L_3, (RuntimeObject *)L_4, (RuntimeObject *)L_6);
		if (!L_7)
		{
			goto IL_006d;
		}
	}
	{
		RuntimeObject* L_8 = ___comparer1;
		RuntimeObject * L_9 = (RuntimeObject *)__this->get_Item2_1();
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_10 = V_0;
		RuntimeObject * L_11 = (RuntimeObject *)L_10.get_Item2_1();
		NullCheck((RuntimeObject*)L_8);
		bool L_12 = InterfaceFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(0 /* System.Boolean System.Collections.IEqualityComparer::Equals(System.Object,System.Object) */, IEqualityComparer_t6C4C1F04B21BDE1E4B84BD6EC7DE494C186D6C68_il2cpp_TypeInfo_var, (RuntimeObject*)L_8, (RuntimeObject *)L_9, (RuntimeObject *)L_11);
		if (!L_12)
		{
			goto IL_006d;
		}
	}
	{
		RuntimeObject* L_13 = ___comparer1;
		RuntimeObject * L_14 = (RuntimeObject *)__this->get_Item3_2();
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_15 = V_0;
		RuntimeObject * L_16 = (RuntimeObject *)L_15.get_Item3_2();
		NullCheck((RuntimeObject*)L_13);
		bool L_17 = InterfaceFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(0 /* System.Boolean System.Collections.IEqualityComparer::Equals(System.Object,System.Object) */, IEqualityComparer_t6C4C1F04B21BDE1E4B84BD6EC7DE494C186D6C68_il2cpp_TypeInfo_var, (RuntimeObject*)L_13, (RuntimeObject *)L_14, (RuntimeObject *)L_16);
		return L_17;
	}

IL_006d:
	{
		return (bool)0;
	}
}
IL2CPP_EXTERN_C  bool ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m8D1BD92383DC97A390119BEE2ABAA659470A3252_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * _thisAdjusted = reinterpret_cast<ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *>(__this + _offset);
	return ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m8D1BD92383DC97A390119BEE2ABAA659470A3252(_thisAdjusted, ___other0, ___comparer1, method);
}
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::System.IComparable.CompareTo(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_System_IComparable_CompareTo_m565E54913F6D87E2DE47D69286058547E5C28155_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___other0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ValueTuple_3_System_IComparable_CompareTo_m565E54913F6D87E2DE47D69286058547E5C28155_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject * L_0 = ___other0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		return 1;
	}

IL_0005:
	{
		RuntimeObject * L_1 = ___other0;
		if (((RuntimeObject *)IsInst((RuntimeObject*)L_1, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0))))
		{
			goto IL_0037;
		}
	}
	{
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_2 = (*(ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)__this);
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_3 = L_2;
		RuntimeObject * L_4 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0), &L_3);
		NullCheck((RuntimeObject *)L_4);
		Type_t * L_5 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)L_4, /*hidden argument*/NULL);
		NullCheck((RuntimeObject *)L_5);
		String_t* L_6 = VirtFuncInvoker0< String_t* >::Invoke(3 /* System.String System.Object::ToString() */, (RuntimeObject *)L_5);
		String_t* L_7 = SR_Format_m942E78AC3ABE13F58075ED90094D6074CA5A7DC8((String_t*)_stringLiteral1459AD7D3E0F8808A85528961118835E18AD1F96, (RuntimeObject *)L_6, /*hidden argument*/NULL);
		ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 * L_8 = (ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 *)il2cpp_codegen_object_new(ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00_il2cpp_TypeInfo_var);
		ArgumentException__ctor_m71044C2110E357B71A1C30D2561C3F861AF1DC0D(L_8, (String_t*)L_7, (String_t*)_stringLiteralF7933083B6BA56CBC6D7BCA0F30688A30D0368F6, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_8, ValueTuple_3_System_IComparable_CompareTo_m565E54913F6D87E2DE47D69286058547E5C28155_RuntimeMethod_var);
	}

IL_0037:
	{
		RuntimeObject * L_9 = ___other0;
		int32_t L_10 = ValueTuple_3_CompareTo_mCEB32C7146D5A53C51A72678D3F152EBB02E0495((ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)(ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)__this, (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D )((*(ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)((ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)UnBox(L_9, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0))))), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 14));
		return L_10;
	}
}
IL2CPP_EXTERN_C  int32_t ValueTuple_3_System_IComparable_CompareTo_m565E54913F6D87E2DE47D69286058547E5C28155_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * _thisAdjusted = reinterpret_cast<ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *>(__this + _offset);
	return ValueTuple_3_System_IComparable_CompareTo_m565E54913F6D87E2DE47D69286058547E5C28155(_thisAdjusted, ___other0, method);
}
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::CompareTo(System.ValueTuple`3<T1,T2,T3>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_CompareTo_mCEB32C7146D5A53C51A72678D3F152EBB02E0495_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 * L_0 = ((  Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 15)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 15));
		RuntimeObject * L_1 = (RuntimeObject *)__this->get_Item1_0();
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_2 = ___other0;
		RuntimeObject * L_3 = (RuntimeObject *)L_2.get_Item1_0();
		NullCheck((Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 *)L_0);
		int32_t L_4 = VirtFuncInvoker2< int32_t, RuntimeObject *, RuntimeObject * >::Invoke(6 /* System.Int32 System.Collections.Generic.Comparer`1<System.Object>::Compare(T,T) */, (Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 *)L_0, (RuntimeObject *)L_1, (RuntimeObject *)L_3);
		V_0 = (int32_t)L_4;
		int32_t L_5 = V_0;
		if (!L_5)
		{
			goto IL_001c;
		}
	}
	{
		int32_t L_6 = V_0;
		return L_6;
	}

IL_001c:
	{
		Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 * L_7 = ((  Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 18)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 18));
		RuntimeObject * L_8 = (RuntimeObject *)__this->get_Item2_1();
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_9 = ___other0;
		RuntimeObject * L_10 = (RuntimeObject *)L_9.get_Item2_1();
		NullCheck((Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 *)L_7);
		int32_t L_11 = VirtFuncInvoker2< int32_t, RuntimeObject *, RuntimeObject * >::Invoke(6 /* System.Int32 System.Collections.Generic.Comparer`1<System.Object>::Compare(T,T) */, (Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 *)L_7, (RuntimeObject *)L_8, (RuntimeObject *)L_10);
		V_0 = (int32_t)L_11;
		int32_t L_12 = V_0;
		if (!L_12)
		{
			goto IL_0038;
		}
	}
	{
		int32_t L_13 = V_0;
		return L_13;
	}

IL_0038:
	{
		Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 * L_14 = ((  Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 21)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 21));
		RuntimeObject * L_15 = (RuntimeObject *)__this->get_Item3_2();
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_16 = ___other0;
		RuntimeObject * L_17 = (RuntimeObject *)L_16.get_Item3_2();
		NullCheck((Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 *)L_14);
		int32_t L_18 = VirtFuncInvoker2< int32_t, RuntimeObject *, RuntimeObject * >::Invoke(6 /* System.Int32 System.Collections.Generic.Comparer`1<System.Object>::Compare(T,T) */, (Comparer_1_t33EA2A3D50A5D04C1A23DFF361A0AAD011657B84 *)L_14, (RuntimeObject *)L_15, (RuntimeObject *)L_17);
		return L_18;
	}
}
IL2CPP_EXTERN_C  int32_t ValueTuple_3_CompareTo_mCEB32C7146D5A53C51A72678D3F152EBB02E0495_AdjustorThunk (RuntimeObject * __this, ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * _thisAdjusted = reinterpret_cast<ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *>(__this + _offset);
	return ValueTuple_3_CompareTo_mCEB32C7146D5A53C51A72678D3F152EBB02E0495(_thisAdjusted, ___other0, method);
}
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::System.Collections.IStructuralComparable.CompareTo(System.Object,System.Collections.IComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m7233DE35FC3952A4832494EDCA1F3F2681AE09FE_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m7233DE35FC3952A4832494EDCA1F3F2681AE09FE_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  V_0;
	memset((&V_0), 0, sizeof(V_0));
	int32_t V_1 = 0;
	{
		RuntimeObject * L_0 = ___other0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		return 1;
	}

IL_0005:
	{
		RuntimeObject * L_1 = ___other0;
		if (((RuntimeObject *)IsInst((RuntimeObject*)L_1, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0))))
		{
			goto IL_0037;
		}
	}
	{
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_2 = (*(ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)__this);
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_3 = L_2;
		RuntimeObject * L_4 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0), &L_3);
		NullCheck((RuntimeObject *)L_4);
		Type_t * L_5 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)L_4, /*hidden argument*/NULL);
		NullCheck((RuntimeObject *)L_5);
		String_t* L_6 = VirtFuncInvoker0< String_t* >::Invoke(3 /* System.String System.Object::ToString() */, (RuntimeObject *)L_5);
		String_t* L_7 = SR_Format_m942E78AC3ABE13F58075ED90094D6074CA5A7DC8((String_t*)_stringLiteral1459AD7D3E0F8808A85528961118835E18AD1F96, (RuntimeObject *)L_6, /*hidden argument*/NULL);
		ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 * L_8 = (ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 *)il2cpp_codegen_object_new(ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00_il2cpp_TypeInfo_var);
		ArgumentException__ctor_m71044C2110E357B71A1C30D2561C3F861AF1DC0D(L_8, (String_t*)L_7, (String_t*)_stringLiteralF7933083B6BA56CBC6D7BCA0F30688A30D0368F6, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_8, ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m7233DE35FC3952A4832494EDCA1F3F2681AE09FE_RuntimeMethod_var);
	}

IL_0037:
	{
		RuntimeObject * L_9 = ___other0;
		V_0 = (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D )((*(ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)((ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)UnBox(L_9, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0)))));
		RuntimeObject* L_10 = ___comparer1;
		RuntimeObject * L_11 = (RuntimeObject *)__this->get_Item1_0();
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_12 = V_0;
		RuntimeObject * L_13 = (RuntimeObject *)L_12.get_Item1_0();
		NullCheck((RuntimeObject*)L_10);
		int32_t L_14 = InterfaceFuncInvoker2< int32_t, RuntimeObject *, RuntimeObject * >::Invoke(0 /* System.Int32 System.Collections.IComparer::Compare(System.Object,System.Object) */, IComparer_t624EE667DCB0D3765FF034F7150DA71B361B82C0_il2cpp_TypeInfo_var, (RuntimeObject*)L_10, (RuntimeObject *)L_11, (RuntimeObject *)L_13);
		V_1 = (int32_t)L_14;
		int32_t L_15 = V_1;
		if (!L_15)
		{
			goto IL_0060;
		}
	}
	{
		int32_t L_16 = V_1;
		return L_16;
	}

IL_0060:
	{
		RuntimeObject* L_17 = ___comparer1;
		RuntimeObject * L_18 = (RuntimeObject *)__this->get_Item2_1();
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_19 = V_0;
		RuntimeObject * L_20 = (RuntimeObject *)L_19.get_Item2_1();
		NullCheck((RuntimeObject*)L_17);
		int32_t L_21 = InterfaceFuncInvoker2< int32_t, RuntimeObject *, RuntimeObject * >::Invoke(0 /* System.Int32 System.Collections.IComparer::Compare(System.Object,System.Object) */, IComparer_t624EE667DCB0D3765FF034F7150DA71B361B82C0_il2cpp_TypeInfo_var, (RuntimeObject*)L_17, (RuntimeObject *)L_18, (RuntimeObject *)L_20);
		V_1 = (int32_t)L_21;
		int32_t L_22 = V_1;
		if (!L_22)
		{
			goto IL_0082;
		}
	}
	{
		int32_t L_23 = V_1;
		return L_23;
	}

IL_0082:
	{
		RuntimeObject* L_24 = ___comparer1;
		RuntimeObject * L_25 = (RuntimeObject *)__this->get_Item3_2();
		ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D  L_26 = V_0;
		RuntimeObject * L_27 = (RuntimeObject *)L_26.get_Item3_2();
		NullCheck((RuntimeObject*)L_24);
		int32_t L_28 = InterfaceFuncInvoker2< int32_t, RuntimeObject *, RuntimeObject * >::Invoke(0 /* System.Int32 System.Collections.IComparer::Compare(System.Object,System.Object) */, IComparer_t624EE667DCB0D3765FF034F7150DA71B361B82C0_il2cpp_TypeInfo_var, (RuntimeObject*)L_24, (RuntimeObject *)L_25, (RuntimeObject *)L_27);
		return L_28;
	}
}
IL2CPP_EXTERN_C  int32_t ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m7233DE35FC3952A4832494EDCA1F3F2681AE09FE_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * _thisAdjusted = reinterpret_cast<ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *>(__this + _offset);
	return ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m7233DE35FC3952A4832494EDCA1F3F2681AE09FE(_thisAdjusted, ___other0, ___comparer1, method);
}
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_GetHashCode_m7484E3361C746C8B62359592753CDFD38BCC2E38_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, const RuntimeMethod* method)
{
	RuntimeObject * V_0 = NULL;
	RuntimeObject * V_1 = NULL;
	RuntimeObject * V_2 = NULL;
	RuntimeObject ** G_B3_0 = NULL;
	RuntimeObject ** G_B1_0 = NULL;
	RuntimeObject ** G_B2_0 = NULL;
	int32_t G_B4_0 = 0;
	RuntimeObject ** G_B7_0 = NULL;
	int32_t G_B7_1 = 0;
	RuntimeObject ** G_B5_0 = NULL;
	int32_t G_B5_1 = 0;
	RuntimeObject ** G_B6_0 = NULL;
	int32_t G_B6_1 = 0;
	int32_t G_B8_0 = 0;
	int32_t G_B8_1 = 0;
	RuntimeObject ** G_B11_0 = NULL;
	int32_t G_B11_1 = 0;
	int32_t G_B11_2 = 0;
	RuntimeObject ** G_B9_0 = NULL;
	int32_t G_B9_1 = 0;
	int32_t G_B9_2 = 0;
	RuntimeObject ** G_B10_0 = NULL;
	int32_t G_B10_1 = 0;
	int32_t G_B10_2 = 0;
	int32_t G_B12_0 = 0;
	int32_t G_B12_1 = 0;
	int32_t G_B12_2 = 0;
	{
		RuntimeObject ** L_0 = (RuntimeObject **)__this->get_address_of_Item1_0();
		il2cpp_codegen_initobj((&V_0), sizeof(RuntimeObject *));
		RuntimeObject * L_1 = V_0;
		G_B1_0 = L_0;
		if (L_1)
		{
			G_B3_0 = L_0;
			goto IL_002a;
		}
	}
	{
		RuntimeObject * L_2 = (*(RuntimeObject **)G_B1_0);
		V_0 = (RuntimeObject *)L_2;
		RuntimeObject * L_3 = V_0;
		G_B2_0 = (&V_0);
		if (L_3)
		{
			G_B3_0 = (&V_0);
			goto IL_002a;
		}
	}
	{
		G_B4_0 = 0;
		goto IL_0035;
	}

IL_002a:
	{
		NullCheck((RuntimeObject *)(*G_B3_0));
		int32_t L_4 = VirtFuncInvoker0< int32_t >::Invoke(2 /* System.Int32 System.Object::GetHashCode() */, (RuntimeObject *)(*G_B3_0));
		G_B4_0 = L_4;
	}

IL_0035:
	{
		RuntimeObject ** L_5 = (RuntimeObject **)__this->get_address_of_Item2_1();
		il2cpp_codegen_initobj((&V_1), sizeof(RuntimeObject *));
		RuntimeObject * L_6 = V_1;
		G_B5_0 = L_5;
		G_B5_1 = G_B4_0;
		if (L_6)
		{
			G_B7_0 = L_5;
			G_B7_1 = G_B4_0;
			goto IL_005f;
		}
	}
	{
		RuntimeObject * L_7 = (*(RuntimeObject **)G_B5_0);
		V_1 = (RuntimeObject *)L_7;
		RuntimeObject * L_8 = V_1;
		G_B6_0 = (&V_1);
		G_B6_1 = G_B5_1;
		if (L_8)
		{
			G_B7_0 = (&V_1);
			G_B7_1 = G_B5_1;
			goto IL_005f;
		}
	}
	{
		G_B8_0 = 0;
		G_B8_1 = G_B6_1;
		goto IL_006a;
	}

IL_005f:
	{
		NullCheck((RuntimeObject *)(*G_B7_0));
		int32_t L_9 = VirtFuncInvoker0< int32_t >::Invoke(2 /* System.Int32 System.Object::GetHashCode() */, (RuntimeObject *)(*G_B7_0));
		G_B8_0 = L_9;
		G_B8_1 = G_B7_1;
	}

IL_006a:
	{
		RuntimeObject ** L_10 = (RuntimeObject **)__this->get_address_of_Item3_2();
		il2cpp_codegen_initobj((&V_2), sizeof(RuntimeObject *));
		RuntimeObject * L_11 = V_2;
		G_B9_0 = L_10;
		G_B9_1 = G_B8_0;
		G_B9_2 = G_B8_1;
		if (L_11)
		{
			G_B11_0 = L_10;
			G_B11_1 = G_B8_0;
			G_B11_2 = G_B8_1;
			goto IL_0094;
		}
	}
	{
		RuntimeObject * L_12 = (*(RuntimeObject **)G_B9_0);
		V_2 = (RuntimeObject *)L_12;
		RuntimeObject * L_13 = V_2;
		G_B10_0 = (&V_2);
		G_B10_1 = G_B9_1;
		G_B10_2 = G_B9_2;
		if (L_13)
		{
			G_B11_0 = (&V_2);
			G_B11_1 = G_B9_1;
			G_B11_2 = G_B9_2;
			goto IL_0094;
		}
	}
	{
		G_B12_0 = 0;
		G_B12_1 = G_B10_1;
		G_B12_2 = G_B10_2;
		goto IL_009f;
	}

IL_0094:
	{
		NullCheck((RuntimeObject *)(*G_B11_0));
		int32_t L_14 = VirtFuncInvoker0< int32_t >::Invoke(2 /* System.Int32 System.Object::GetHashCode() */, (RuntimeObject *)(*G_B11_0));
		G_B12_0 = L_14;
		G_B12_1 = G_B11_1;
		G_B12_2 = G_B11_2;
	}

IL_009f:
	{
		int32_t L_15 = ValueTuple_CombineHashCodes_m6D3138F0BA3D04CA1B640620E47716F05EB2DEBE((int32_t)G_B12_2, (int32_t)G_B12_1, (int32_t)G_B12_0, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t ValueTuple_3_GetHashCode_m7484E3361C746C8B62359592753CDFD38BCC2E38_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * _thisAdjusted = reinterpret_cast<ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *>(__this + _offset);
	return ValueTuple_3_GetHashCode_m7484E3361C746C8B62359592753CDFD38BCC2E38(_thisAdjusted, method);
}
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_System_Collections_IStructuralEquatable_GetHashCode_mD601062CE782B8EDA3A9AEBD2DE16AC0423B61FE_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = ___comparer0;
		int32_t L_1 = ValueTuple_3_GetHashCodeCore_m67CB7531D334ACBE07696D3CFC5DB57C4738F3C4((ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)(ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *)__this, (RuntimeObject*)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 24));
		return L_1;
	}
}
IL2CPP_EXTERN_C  int32_t ValueTuple_3_System_Collections_IStructuralEquatable_GetHashCode_mD601062CE782B8EDA3A9AEBD2DE16AC0423B61FE_AdjustorThunk (RuntimeObject * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * _thisAdjusted = reinterpret_cast<ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *>(__this + _offset);
	return ValueTuple_3_System_Collections_IStructuralEquatable_GetHashCode_mD601062CE782B8EDA3A9AEBD2DE16AC0423B61FE(_thisAdjusted, ___comparer0, method);
}
// System.Int32 System.ValueTuple`3<System.Object,System.Object,System.Object>::GetHashCodeCore(System.Collections.IEqualityComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_GetHashCodeCore_m67CB7531D334ACBE07696D3CFC5DB57C4738F3C4_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ValueTuple_3_GetHashCodeCore_m67CB7531D334ACBE07696D3CFC5DB57C4738F3C4_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = ___comparer0;
		RuntimeObject * L_1 = (RuntimeObject *)__this->get_Item1_0();
		NullCheck((RuntimeObject*)L_0);
		int32_t L_2 = InterfaceFuncInvoker1< int32_t, RuntimeObject * >::Invoke(1 /* System.Int32 System.Collections.IEqualityComparer::GetHashCode(System.Object) */, IEqualityComparer_t6C4C1F04B21BDE1E4B84BD6EC7DE494C186D6C68_il2cpp_TypeInfo_var, (RuntimeObject*)L_0, (RuntimeObject *)L_1);
		RuntimeObject* L_3 = ___comparer0;
		RuntimeObject * L_4 = (RuntimeObject *)__this->get_Item2_1();
		NullCheck((RuntimeObject*)L_3);
		int32_t L_5 = InterfaceFuncInvoker1< int32_t, RuntimeObject * >::Invoke(1 /* System.Int32 System.Collections.IEqualityComparer::GetHashCode(System.Object) */, IEqualityComparer_t6C4C1F04B21BDE1E4B84BD6EC7DE494C186D6C68_il2cpp_TypeInfo_var, (RuntimeObject*)L_3, (RuntimeObject *)L_4);
		RuntimeObject* L_6 = ___comparer0;
		RuntimeObject * L_7 = (RuntimeObject *)__this->get_Item3_2();
		NullCheck((RuntimeObject*)L_6);
		int32_t L_8 = InterfaceFuncInvoker1< int32_t, RuntimeObject * >::Invoke(1 /* System.Int32 System.Collections.IEqualityComparer::GetHashCode(System.Object) */, IEqualityComparer_t6C4C1F04B21BDE1E4B84BD6EC7DE494C186D6C68_il2cpp_TypeInfo_var, (RuntimeObject*)L_6, (RuntimeObject *)L_7);
		int32_t L_9 = ValueTuple_CombineHashCodes_m6D3138F0BA3D04CA1B640620E47716F05EB2DEBE((int32_t)L_2, (int32_t)L_5, (int32_t)L_8, /*hidden argument*/NULL);
		return L_9;
	}
}
IL2CPP_EXTERN_C  int32_t ValueTuple_3_GetHashCodeCore_m67CB7531D334ACBE07696D3CFC5DB57C4738F3C4_AdjustorThunk (RuntimeObject * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * _thisAdjusted = reinterpret_cast<ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *>(__this + _offset);
	return ValueTuple_3_GetHashCodeCore_m67CB7531D334ACBE07696D3CFC5DB57C4738F3C4(_thisAdjusted, ___comparer0, method);
}
// System.String System.ValueTuple`3<System.Object,System.Object,System.Object>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* ValueTuple_3_ToString_mE918A7F63C8F4AD1C4BCF5C2F28D048307DBB1BF_gshared (ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ValueTuple_3_ToString_mE918A7F63C8F4AD1C4BCF5C2F28D048307DBB1BF_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject * V_0 = NULL;
	RuntimeObject * V_1 = NULL;
	RuntimeObject * V_2 = NULL;
	RuntimeObject ** G_B3_0 = NULL;
	int32_t G_B3_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B3_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B3_3 = NULL;
	RuntimeObject ** G_B1_0 = NULL;
	int32_t G_B1_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B1_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B1_3 = NULL;
	RuntimeObject ** G_B2_0 = NULL;
	int32_t G_B2_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B2_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B2_3 = NULL;
	String_t* G_B4_0 = NULL;
	int32_t G_B4_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B4_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B4_3 = NULL;
	RuntimeObject ** G_B7_0 = NULL;
	int32_t G_B7_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B7_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B7_3 = NULL;
	RuntimeObject ** G_B5_0 = NULL;
	int32_t G_B5_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B5_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B5_3 = NULL;
	RuntimeObject ** G_B6_0 = NULL;
	int32_t G_B6_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B6_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B6_3 = NULL;
	String_t* G_B8_0 = NULL;
	int32_t G_B8_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B8_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B8_3 = NULL;
	RuntimeObject ** G_B11_0 = NULL;
	int32_t G_B11_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B11_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B11_3 = NULL;
	RuntimeObject ** G_B9_0 = NULL;
	int32_t G_B9_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B9_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B9_3 = NULL;
	RuntimeObject ** G_B10_0 = NULL;
	int32_t G_B10_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B10_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B10_3 = NULL;
	String_t* G_B12_0 = NULL;
	int32_t G_B12_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B12_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B12_3 = NULL;
	{
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_0 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)(StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)SZArrayNew(StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A_il2cpp_TypeInfo_var, (uint32_t)7);
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_1 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)L_0;
		NullCheck(L_1);
		ArrayElementTypeCheck (L_1, _stringLiteralA3DFC0C77ACADE0EE48DCC73E795A597D0270A73);
		(L_1)->SetAt(static_cast<il2cpp_array_size_t>(0), (String_t*)_stringLiteralA3DFC0C77ACADE0EE48DCC73E795A597D0270A73);
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_2 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)L_1;
		RuntimeObject ** L_3 = (RuntimeObject **)__this->get_address_of_Item1_0();
		il2cpp_codegen_initobj((&V_0), sizeof(RuntimeObject *));
		RuntimeObject * L_4 = V_0;
		G_B1_0 = L_3;
		G_B1_1 = 1;
		G_B1_2 = L_2;
		G_B1_3 = L_2;
		if (L_4)
		{
			G_B3_0 = L_3;
			G_B3_1 = 1;
			G_B3_2 = L_2;
			G_B3_3 = L_2;
			goto IL_003a;
		}
	}
	{
		RuntimeObject * L_5 = (*(RuntimeObject **)G_B1_0);
		V_0 = (RuntimeObject *)L_5;
		RuntimeObject * L_6 = V_0;
		G_B2_0 = (&V_0);
		G_B2_1 = G_B1_1;
		G_B2_2 = G_B1_2;
		G_B2_3 = G_B1_3;
		if (L_6)
		{
			G_B3_0 = (&V_0);
			G_B3_1 = G_B1_1;
			G_B3_2 = G_B1_2;
			G_B3_3 = G_B1_3;
			goto IL_003a;
		}
	}
	{
		G_B4_0 = ((String_t*)(NULL));
		G_B4_1 = G_B2_1;
		G_B4_2 = G_B2_2;
		G_B4_3 = G_B2_3;
		goto IL_0045;
	}

IL_003a:
	{
		NullCheck((RuntimeObject *)(*G_B3_0));
		String_t* L_7 = VirtFuncInvoker0< String_t* >::Invoke(3 /* System.String System.Object::ToString() */, (RuntimeObject *)(*G_B3_0));
		G_B4_0 = L_7;
		G_B4_1 = G_B3_1;
		G_B4_2 = G_B3_2;
		G_B4_3 = G_B3_3;
	}

IL_0045:
	{
		NullCheck(G_B4_2);
		ArrayElementTypeCheck (G_B4_2, G_B4_0);
		(G_B4_2)->SetAt(static_cast<il2cpp_array_size_t>(G_B4_1), (String_t*)G_B4_0);
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_8 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)G_B4_3;
		NullCheck(L_8);
		ArrayElementTypeCheck (L_8, _stringLiteral758733BDBED83CBFF4F635AC26CA92AAE477F75D);
		(L_8)->SetAt(static_cast<il2cpp_array_size_t>(2), (String_t*)_stringLiteral758733BDBED83CBFF4F635AC26CA92AAE477F75D);
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_9 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)L_8;
		RuntimeObject ** L_10 = (RuntimeObject **)__this->get_address_of_Item2_1();
		il2cpp_codegen_initobj((&V_1), sizeof(RuntimeObject *));
		RuntimeObject * L_11 = V_1;
		G_B5_0 = L_10;
		G_B5_1 = 3;
		G_B5_2 = L_9;
		G_B5_3 = L_9;
		if (L_11)
		{
			G_B7_0 = L_10;
			G_B7_1 = 3;
			G_B7_2 = L_9;
			G_B7_3 = L_9;
			goto IL_007a;
		}
	}
	{
		RuntimeObject * L_12 = (*(RuntimeObject **)G_B5_0);
		V_1 = (RuntimeObject *)L_12;
		RuntimeObject * L_13 = V_1;
		G_B6_0 = (&V_1);
		G_B6_1 = G_B5_1;
		G_B6_2 = G_B5_2;
		G_B6_3 = G_B5_3;
		if (L_13)
		{
			G_B7_0 = (&V_1);
			G_B7_1 = G_B5_1;
			G_B7_2 = G_B5_2;
			G_B7_3 = G_B5_3;
			goto IL_007a;
		}
	}
	{
		G_B8_0 = ((String_t*)(NULL));
		G_B8_1 = G_B6_1;
		G_B8_2 = G_B6_2;
		G_B8_3 = G_B6_3;
		goto IL_0085;
	}

IL_007a:
	{
		NullCheck((RuntimeObject *)(*G_B7_0));
		String_t* L_14 = VirtFuncInvoker0< String_t* >::Invoke(3 /* System.String System.Object::ToString() */, (RuntimeObject *)(*G_B7_0));
		G_B8_0 = L_14;
		G_B8_1 = G_B7_1;
		G_B8_2 = G_B7_2;
		G_B8_3 = G_B7_3;
	}

IL_0085:
	{
		NullCheck(G_B8_2);
		ArrayElementTypeCheck (G_B8_2, G_B8_0);
		(G_B8_2)->SetAt(static_cast<il2cpp_array_size_t>(G_B8_1), (String_t*)G_B8_0);
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_15 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)G_B8_3;
		NullCheck(L_15);
		ArrayElementTypeCheck (L_15, _stringLiteral758733BDBED83CBFF4F635AC26CA92AAE477F75D);
		(L_15)->SetAt(static_cast<il2cpp_array_size_t>(4), (String_t*)_stringLiteral758733BDBED83CBFF4F635AC26CA92AAE477F75D);
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_16 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)L_15;
		RuntimeObject ** L_17 = (RuntimeObject **)__this->get_address_of_Item3_2();
		il2cpp_codegen_initobj((&V_2), sizeof(RuntimeObject *));
		RuntimeObject * L_18 = V_2;
		G_B9_0 = L_17;
		G_B9_1 = 5;
		G_B9_2 = L_16;
		G_B9_3 = L_16;
		if (L_18)
		{
			G_B11_0 = L_17;
			G_B11_1 = 5;
			G_B11_2 = L_16;
			G_B11_3 = L_16;
			goto IL_00ba;
		}
	}
	{
		RuntimeObject * L_19 = (*(RuntimeObject **)G_B9_0);
		V_2 = (RuntimeObject *)L_19;
		RuntimeObject * L_20 = V_2;
		G_B10_0 = (&V_2);
		G_B10_1 = G_B9_1;
		G_B10_2 = G_B9_2;
		G_B10_3 = G_B9_3;
		if (L_20)
		{
			G_B11_0 = (&V_2);
			G_B11_1 = G_B9_1;
			G_B11_2 = G_B9_2;
			G_B11_3 = G_B9_3;
			goto IL_00ba;
		}
	}
	{
		G_B12_0 = ((String_t*)(NULL));
		G_B12_1 = G_B10_1;
		G_B12_2 = G_B10_2;
		G_B12_3 = G_B10_3;
		goto IL_00c5;
	}

IL_00ba:
	{
		NullCheck((RuntimeObject *)(*G_B11_0));
		String_t* L_21 = VirtFuncInvoker0< String_t* >::Invoke(3 /* System.String System.Object::ToString() */, (RuntimeObject *)(*G_B11_0));
		G_B12_0 = L_21;
		G_B12_1 = G_B11_1;
		G_B12_2 = G_B11_2;
		G_B12_3 = G_B11_3;
	}

IL_00c5:
	{
		NullCheck(G_B12_2);
		ArrayElementTypeCheck (G_B12_2, G_B12_0);
		(G_B12_2)->SetAt(static_cast<il2cpp_array_size_t>(G_B12_1), (String_t*)G_B12_0);
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_22 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)G_B12_3;
		NullCheck(L_22);
		ArrayElementTypeCheck (L_22, _stringLiteralB3F14BF976EFD974E34846B742502C802FABAE9D);
		(L_22)->SetAt(static_cast<il2cpp_array_size_t>(6), (String_t*)_stringLiteralB3F14BF976EFD974E34846B742502C802FABAE9D);
		String_t* L_23 = String_Concat_mFEA7EFA1A6E75B96B1B7BC4526AAC864BFF83CC9((StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)L_22, /*hidden argument*/NULL);
		return L_23;
	}
}
IL2CPP_EXTERN_C  String_t* ValueTuple_3_ToString_mE918A7F63C8F4AD1C4BCF5C2F28D048307DBB1BF_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D * _thisAdjusted = reinterpret_cast<ValueTuple_3_tEF9008762923C50FBA1F5E13EFAE26235274202D *>(__this + _offset);
	return ValueTuple_3_ToString_mE918A7F63C8F4AD1C4BCF5C2F28D048307DBB1BF(_thisAdjusted, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::.ctor(T1,T2,T3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ValueTuple_3__ctor_mCB5A2C8A72952508684BA5B641486528A773A670_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___item10, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___item21, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___item32, const RuntimeMethod* method)
{
	{
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_0 = ___item10;
		__this->set_Item1_0(L_0);
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_1 = ___item21;
		__this->set_Item2_1(L_1);
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_2 = ___item32;
		__this->set_Item3_2(L_2);
		return;
	}
}
IL2CPP_EXTERN_C  void ValueTuple_3__ctor_mCB5A2C8A72952508684BA5B641486528A773A670_AdjustorThunk (RuntimeObject * __this, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___item10, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___item21, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___item32, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * _thisAdjusted = reinterpret_cast<ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *>(__this + _offset);
	ValueTuple_3__ctor_mCB5A2C8A72952508684BA5B641486528A773A670(_thisAdjusted, ___item10, ___item21, ___item32, method);
}
// System.Boolean System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ValueTuple_3_Equals_m3D96E39FD82E7C0FF90956B59AC55794A1F2D724_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = ___obj0;
		if (!((RuntimeObject *)IsInst((RuntimeObject*)L_0, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0))))
		{
			goto IL_0015;
		}
	}
	{
		RuntimeObject * L_1 = ___obj0;
		bool L_2 = ValueTuple_3_Equals_m7F0287ACB5F228FE2C345AC086983A43BEF3B710((ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)(ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)__this, (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 )((*(ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)((ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)UnBox(L_1, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0))))), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 1));
		return L_2;
	}

IL_0015:
	{
		return (bool)0;
	}
}
IL2CPP_EXTERN_C  bool ValueTuple_3_Equals_m3D96E39FD82E7C0FF90956B59AC55794A1F2D724_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * _thisAdjusted = reinterpret_cast<ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *>(__this + _offset);
	return ValueTuple_3_Equals_m3D96E39FD82E7C0FF90956B59AC55794A1F2D724(_thisAdjusted, ___obj0, method);
}
// System.Boolean System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::Equals(System.ValueTuple`3<T1,T2,T3>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ValueTuple_3_Equals_m7F0287ACB5F228FE2C345AC086983A43BEF3B710_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  ___other0, const RuntimeMethod* method)
{
	{
		EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E * L_0 = ((  EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2));
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_1 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item1_0();
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_2 = ___other0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_3 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_2.get_Item1_0();
		NullCheck((EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E *)L_0);
		bool L_4 = VirtFuncInvoker2< bool, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector4>::Equals(T,T) */, (EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E *)L_0, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_1, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_3);
		if (!L_4)
		{
			goto IL_0047;
		}
	}
	{
		EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E * L_5 = ((  EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 5)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 5));
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_6 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item2_1();
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_7 = ___other0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_8 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_7.get_Item2_1();
		NullCheck((EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E *)L_5);
		bool L_9 = VirtFuncInvoker2< bool, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector4>::Equals(T,T) */, (EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E *)L_5, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_6, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_8);
		if (!L_9)
		{
			goto IL_0047;
		}
	}
	{
		EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E * L_10 = ((  EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_11 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item3_2();
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_12 = ___other0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_13 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_12.get_Item3_2();
		NullCheck((EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E *)L_10);
		bool L_14 = VirtFuncInvoker2< bool, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector4>::Equals(T,T) */, (EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E *)L_10, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_11, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_13);
		return L_14;
	}

IL_0047:
	{
		return (bool)0;
	}
}
IL2CPP_EXTERN_C  bool ValueTuple_3_Equals_m7F0287ACB5F228FE2C345AC086983A43BEF3B710_AdjustorThunk (RuntimeObject * __this, ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * _thisAdjusted = reinterpret_cast<ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *>(__this + _offset);
	return ValueTuple_3_Equals_m7F0287ACB5F228FE2C345AC086983A43BEF3B710(_thisAdjusted, ___other0, method);
}
// System.Boolean System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::System.Collections.IStructuralEquatable.Equals(System.Object,System.Collections.IEqualityComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m2B4EB2E18EAA9D66C46192CAFA91BD15AAAEF78E_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m2B4EB2E18EAA9D66C46192CAFA91BD15AAAEF78E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		RuntimeObject * L_0 = ___other0;
		if (!L_0)
		{
			goto IL_000b;
		}
	}
	{
		RuntimeObject * L_1 = ___other0;
		if (((RuntimeObject *)IsInst((RuntimeObject*)L_1, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0))))
		{
			goto IL_000d;
		}
	}

IL_000b:
	{
		return (bool)0;
	}

IL_000d:
	{
		RuntimeObject * L_2 = ___other0;
		V_0 = (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 )((*(ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)((ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)UnBox(L_2, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0)))));
		RuntimeObject* L_3 = ___comparer1;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_4 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item1_0();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_5 = L_4;
		RuntimeObject * L_6 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 11), &L_5);
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_7 = V_0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_8 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_7.get_Item1_0();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_9 = L_8;
		RuntimeObject * L_10 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 11), &L_9);
		NullCheck((RuntimeObject*)L_3);
		bool L_11 = InterfaceFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(0 /* System.Boolean System.Collections.IEqualityComparer::Equals(System.Object,System.Object) */, IEqualityComparer_t6C4C1F04B21BDE1E4B84BD6EC7DE494C186D6C68_il2cpp_TypeInfo_var, (RuntimeObject*)L_3, (RuntimeObject *)L_6, (RuntimeObject *)L_10);
		if (!L_11)
		{
			goto IL_006d;
		}
	}
	{
		RuntimeObject* L_12 = ___comparer1;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_13 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item2_1();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_14 = L_13;
		RuntimeObject * L_15 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 12), &L_14);
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_16 = V_0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_17 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_16.get_Item2_1();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_18 = L_17;
		RuntimeObject * L_19 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 12), &L_18);
		NullCheck((RuntimeObject*)L_12);
		bool L_20 = InterfaceFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(0 /* System.Boolean System.Collections.IEqualityComparer::Equals(System.Object,System.Object) */, IEqualityComparer_t6C4C1F04B21BDE1E4B84BD6EC7DE494C186D6C68_il2cpp_TypeInfo_var, (RuntimeObject*)L_12, (RuntimeObject *)L_15, (RuntimeObject *)L_19);
		if (!L_20)
		{
			goto IL_006d;
		}
	}
	{
		RuntimeObject* L_21 = ___comparer1;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_22 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item3_2();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_23 = L_22;
		RuntimeObject * L_24 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 13), &L_23);
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_25 = V_0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_26 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_25.get_Item3_2();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_27 = L_26;
		RuntimeObject * L_28 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 13), &L_27);
		NullCheck((RuntimeObject*)L_21);
		bool L_29 = InterfaceFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(0 /* System.Boolean System.Collections.IEqualityComparer::Equals(System.Object,System.Object) */, IEqualityComparer_t6C4C1F04B21BDE1E4B84BD6EC7DE494C186D6C68_il2cpp_TypeInfo_var, (RuntimeObject*)L_21, (RuntimeObject *)L_24, (RuntimeObject *)L_28);
		return L_29;
	}

IL_006d:
	{
		return (bool)0;
	}
}
IL2CPP_EXTERN_C  bool ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m2B4EB2E18EAA9D66C46192CAFA91BD15AAAEF78E_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * _thisAdjusted = reinterpret_cast<ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *>(__this + _offset);
	return ValueTuple_3_System_Collections_IStructuralEquatable_Equals_m2B4EB2E18EAA9D66C46192CAFA91BD15AAAEF78E(_thisAdjusted, ___other0, ___comparer1, method);
}
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::System.IComparable.CompareTo(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_System_IComparable_CompareTo_mDCBC6504198CCF4FD5A11911B785F625602F0A5A_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject * ___other0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ValueTuple_3_System_IComparable_CompareTo_mDCBC6504198CCF4FD5A11911B785F625602F0A5A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject * L_0 = ___other0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		return 1;
	}

IL_0005:
	{
		RuntimeObject * L_1 = ___other0;
		if (((RuntimeObject *)IsInst((RuntimeObject*)L_1, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0))))
		{
			goto IL_0037;
		}
	}
	{
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_2 = (*(ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)__this);
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_3 = L_2;
		RuntimeObject * L_4 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0), &L_3);
		NullCheck((RuntimeObject *)L_4);
		Type_t * L_5 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)L_4, /*hidden argument*/NULL);
		NullCheck((RuntimeObject *)L_5);
		String_t* L_6 = VirtFuncInvoker0< String_t* >::Invoke(3 /* System.String System.Object::ToString() */, (RuntimeObject *)L_5);
		String_t* L_7 = SR_Format_m942E78AC3ABE13F58075ED90094D6074CA5A7DC8((String_t*)_stringLiteral1459AD7D3E0F8808A85528961118835E18AD1F96, (RuntimeObject *)L_6, /*hidden argument*/NULL);
		ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 * L_8 = (ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 *)il2cpp_codegen_object_new(ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00_il2cpp_TypeInfo_var);
		ArgumentException__ctor_m71044C2110E357B71A1C30D2561C3F861AF1DC0D(L_8, (String_t*)L_7, (String_t*)_stringLiteralF7933083B6BA56CBC6D7BCA0F30688A30D0368F6, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_8, ValueTuple_3_System_IComparable_CompareTo_mDCBC6504198CCF4FD5A11911B785F625602F0A5A_RuntimeMethod_var);
	}

IL_0037:
	{
		RuntimeObject * L_9 = ___other0;
		int32_t L_10 = ValueTuple_3_CompareTo_m82FF5B1B9AFC3BC475D59DB9BCC3948E8B6737B5((ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)(ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)__this, (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 )((*(ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)((ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)UnBox(L_9, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0))))), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 14));
		return L_10;
	}
}
IL2CPP_EXTERN_C  int32_t ValueTuple_3_System_IComparable_CompareTo_mDCBC6504198CCF4FD5A11911B785F625602F0A5A_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * _thisAdjusted = reinterpret_cast<ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *>(__this + _offset);
	return ValueTuple_3_System_IComparable_CompareTo_mDCBC6504198CCF4FD5A11911B785F625602F0A5A(_thisAdjusted, ___other0, method);
}
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::CompareTo(System.ValueTuple`3<T1,T2,T3>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_CompareTo_m82FF5B1B9AFC3BC475D59DB9BCC3948E8B6737B5_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD * L_0 = ((  Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 15)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 15));
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_1 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item1_0();
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_2 = ___other0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_3 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_2.get_Item1_0();
		NullCheck((Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD *)L_0);
		int32_t L_4 = VirtFuncInvoker2< int32_t, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  >::Invoke(6 /* System.Int32 System.Collections.Generic.Comparer`1<UnityEngine.Vector4>::Compare(T,T) */, (Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD *)L_0, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_1, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_3);
		V_0 = (int32_t)L_4;
		int32_t L_5 = V_0;
		if (!L_5)
		{
			goto IL_001c;
		}
	}
	{
		int32_t L_6 = V_0;
		return L_6;
	}

IL_001c:
	{
		Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD * L_7 = ((  Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 18)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 18));
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_8 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item2_1();
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_9 = ___other0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_10 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_9.get_Item2_1();
		NullCheck((Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD *)L_7);
		int32_t L_11 = VirtFuncInvoker2< int32_t, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  >::Invoke(6 /* System.Int32 System.Collections.Generic.Comparer`1<UnityEngine.Vector4>::Compare(T,T) */, (Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD *)L_7, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_8, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_10);
		V_0 = (int32_t)L_11;
		int32_t L_12 = V_0;
		if (!L_12)
		{
			goto IL_0038;
		}
	}
	{
		int32_t L_13 = V_0;
		return L_13;
	}

IL_0038:
	{
		Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD * L_14 = ((  Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 21)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 21));
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_15 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item3_2();
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_16 = ___other0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_17 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_16.get_Item3_2();
		NullCheck((Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD *)L_14);
		int32_t L_18 = VirtFuncInvoker2< int32_t, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  >::Invoke(6 /* System.Int32 System.Collections.Generic.Comparer`1<UnityEngine.Vector4>::Compare(T,T) */, (Comparer_1_tDDF07F24B981E579AEB1E2A018BED4F9752478BD *)L_14, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_15, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_17);
		return L_18;
	}
}
IL2CPP_EXTERN_C  int32_t ValueTuple_3_CompareTo_m82FF5B1B9AFC3BC475D59DB9BCC3948E8B6737B5_AdjustorThunk (RuntimeObject * __this, ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * _thisAdjusted = reinterpret_cast<ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *>(__this + _offset);
	return ValueTuple_3_CompareTo_m82FF5B1B9AFC3BC475D59DB9BCC3948E8B6737B5(_thisAdjusted, ___other0, method);
}
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::System.Collections.IStructuralComparable.CompareTo(System.Object,System.Collections.IComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m8F9A1A82E5960A5A167B7B61237919AC9EA86869_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m8F9A1A82E5960A5A167B7B61237919AC9EA86869_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  V_0;
	memset((&V_0), 0, sizeof(V_0));
	int32_t V_1 = 0;
	{
		RuntimeObject * L_0 = ___other0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		return 1;
	}

IL_0005:
	{
		RuntimeObject * L_1 = ___other0;
		if (((RuntimeObject *)IsInst((RuntimeObject*)L_1, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0))))
		{
			goto IL_0037;
		}
	}
	{
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_2 = (*(ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)__this);
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_3 = L_2;
		RuntimeObject * L_4 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0), &L_3);
		NullCheck((RuntimeObject *)L_4);
		Type_t * L_5 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)L_4, /*hidden argument*/NULL);
		NullCheck((RuntimeObject *)L_5);
		String_t* L_6 = VirtFuncInvoker0< String_t* >::Invoke(3 /* System.String System.Object::ToString() */, (RuntimeObject *)L_5);
		String_t* L_7 = SR_Format_m942E78AC3ABE13F58075ED90094D6074CA5A7DC8((String_t*)_stringLiteral1459AD7D3E0F8808A85528961118835E18AD1F96, (RuntimeObject *)L_6, /*hidden argument*/NULL);
		ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 * L_8 = (ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00 *)il2cpp_codegen_object_new(ArgumentException_t505FA8C11E883F2D96C797AD9D396490794DEE00_il2cpp_TypeInfo_var);
		ArgumentException__ctor_m71044C2110E357B71A1C30D2561C3F861AF1DC0D(L_8, (String_t*)L_7, (String_t*)_stringLiteralF7933083B6BA56CBC6D7BCA0F30688A30D0368F6, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_8, ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m8F9A1A82E5960A5A167B7B61237919AC9EA86869_RuntimeMethod_var);
	}

IL_0037:
	{
		RuntimeObject * L_9 = ___other0;
		V_0 = (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 )((*(ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)((ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)UnBox(L_9, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 0)))));
		RuntimeObject* L_10 = ___comparer1;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_11 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item1_0();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_12 = L_11;
		RuntimeObject * L_13 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 11), &L_12);
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_14 = V_0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_15 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_14.get_Item1_0();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_16 = L_15;
		RuntimeObject * L_17 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 11), &L_16);
		NullCheck((RuntimeObject*)L_10);
		int32_t L_18 = InterfaceFuncInvoker2< int32_t, RuntimeObject *, RuntimeObject * >::Invoke(0 /* System.Int32 System.Collections.IComparer::Compare(System.Object,System.Object) */, IComparer_t624EE667DCB0D3765FF034F7150DA71B361B82C0_il2cpp_TypeInfo_var, (RuntimeObject*)L_10, (RuntimeObject *)L_13, (RuntimeObject *)L_17);
		V_1 = (int32_t)L_18;
		int32_t L_19 = V_1;
		if (!L_19)
		{
			goto IL_0060;
		}
	}
	{
		int32_t L_20 = V_1;
		return L_20;
	}

IL_0060:
	{
		RuntimeObject* L_21 = ___comparer1;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_22 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item2_1();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_23 = L_22;
		RuntimeObject * L_24 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 12), &L_23);
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_25 = V_0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_26 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_25.get_Item2_1();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_27 = L_26;
		RuntimeObject * L_28 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 12), &L_27);
		NullCheck((RuntimeObject*)L_21);
		int32_t L_29 = InterfaceFuncInvoker2< int32_t, RuntimeObject *, RuntimeObject * >::Invoke(0 /* System.Int32 System.Collections.IComparer::Compare(System.Object,System.Object) */, IComparer_t624EE667DCB0D3765FF034F7150DA71B361B82C0_il2cpp_TypeInfo_var, (RuntimeObject*)L_21, (RuntimeObject *)L_24, (RuntimeObject *)L_28);
		V_1 = (int32_t)L_29;
		int32_t L_30 = V_1;
		if (!L_30)
		{
			goto IL_0082;
		}
	}
	{
		int32_t L_31 = V_1;
		return L_31;
	}

IL_0082:
	{
		RuntimeObject* L_32 = ___comparer1;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_33 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item3_2();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_34 = L_33;
		RuntimeObject * L_35 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 13), &L_34);
		ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5  L_36 = V_0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_37 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_36.get_Item3_2();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_38 = L_37;
		RuntimeObject * L_39 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 13), &L_38);
		NullCheck((RuntimeObject*)L_32);
		int32_t L_40 = InterfaceFuncInvoker2< int32_t, RuntimeObject *, RuntimeObject * >::Invoke(0 /* System.Int32 System.Collections.IComparer::Compare(System.Object,System.Object) */, IComparer_t624EE667DCB0D3765FF034F7150DA71B361B82C0_il2cpp_TypeInfo_var, (RuntimeObject*)L_32, (RuntimeObject *)L_35, (RuntimeObject *)L_39);
		return L_40;
	}
}
IL2CPP_EXTERN_C  int32_t ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m8F9A1A82E5960A5A167B7B61237919AC9EA86869_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___other0, RuntimeObject* ___comparer1, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * _thisAdjusted = reinterpret_cast<ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *>(__this + _offset);
	return ValueTuple_3_System_Collections_IStructuralComparable_CompareTo_m8F9A1A82E5960A5A167B7B61237919AC9EA86869(_thisAdjusted, ___other0, ___comparer1, method);
}
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_GetHashCode_mD7AC1A1D2BFFD62FC7C766D1595076CE613BFC4E_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, const RuntimeMethod* method)
{
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  V_0;
	memset((&V_0), 0, sizeof(V_0));
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  V_1;
	memset((&V_1), 0, sizeof(V_1));
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  V_2;
	memset((&V_2), 0, sizeof(V_2));
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B3_0 = NULL;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B1_0 = NULL;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B2_0 = NULL;
	int32_t G_B4_0 = 0;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B7_0 = NULL;
	int32_t G_B7_1 = 0;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B5_0 = NULL;
	int32_t G_B5_1 = 0;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B6_0 = NULL;
	int32_t G_B6_1 = 0;
	int32_t G_B8_0 = 0;
	int32_t G_B8_1 = 0;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B11_0 = NULL;
	int32_t G_B11_1 = 0;
	int32_t G_B11_2 = 0;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B9_0 = NULL;
	int32_t G_B9_1 = 0;
	int32_t G_B9_2 = 0;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B10_0 = NULL;
	int32_t G_B10_1 = 0;
	int32_t G_B10_2 = 0;
	int32_t G_B12_0 = 0;
	int32_t G_B12_1 = 0;
	int32_t G_B12_2 = 0;
	{
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * L_0 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)__this->get_address_of_Item1_0();
		il2cpp_codegen_initobj((&V_0), sizeof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 ));
		G_B3_0 = L_0;
		goto IL_002a;
		G_B1_0 = L_0;
	}
	{
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_2 = (*(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)G_B1_0);
		V_0 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_2;
		G_B3_0 = (&V_0);
		goto IL_002a;
		G_B2_0 = (&V_0);
	}
	{
		G_B4_0 = 0;
		goto IL_0035;
	}

IL_002a:
	{
		int32_t L_4 = Vector4_GetHashCode_mCA7B312F8CA141F6F25BABDDF406F3D2BDD5E895((Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)G_B3_0, /*hidden argument*/NULL);
		G_B4_0 = L_4;
	}

IL_0035:
	{
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * L_5 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)__this->get_address_of_Item2_1();
		il2cpp_codegen_initobj((&V_1), sizeof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 ));
		G_B7_0 = L_5;
		G_B7_1 = G_B4_0;
		goto IL_005f;
		G_B5_0 = L_5;
		G_B5_1 = G_B4_0;
	}
	{
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_7 = (*(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)G_B5_0);
		V_1 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_7;
		G_B7_0 = (&V_1);
		G_B7_1 = G_B5_1;
		goto IL_005f;
		G_B6_0 = (&V_1);
		G_B6_1 = G_B5_1;
	}
	{
		G_B8_0 = 0;
		G_B8_1 = G_B6_1;
		goto IL_006a;
	}

IL_005f:
	{
		int32_t L_9 = Vector4_GetHashCode_mCA7B312F8CA141F6F25BABDDF406F3D2BDD5E895((Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)G_B7_0, /*hidden argument*/NULL);
		G_B8_0 = L_9;
		G_B8_1 = G_B7_1;
	}

IL_006a:
	{
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * L_10 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)__this->get_address_of_Item3_2();
		il2cpp_codegen_initobj((&V_2), sizeof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 ));
		G_B11_0 = L_10;
		G_B11_1 = G_B8_0;
		G_B11_2 = G_B8_1;
		goto IL_0094;
		G_B9_0 = L_10;
		G_B9_1 = G_B8_0;
		G_B9_2 = G_B8_1;
	}
	{
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_12 = (*(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)G_B9_0);
		V_2 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_12;
		G_B11_0 = (&V_2);
		G_B11_1 = G_B9_1;
		G_B11_2 = G_B9_2;
		goto IL_0094;
		G_B10_0 = (&V_2);
		G_B10_1 = G_B9_1;
		G_B10_2 = G_B9_2;
	}
	{
		G_B12_0 = 0;
		G_B12_1 = G_B10_1;
		G_B12_2 = G_B10_2;
		goto IL_009f;
	}

IL_0094:
	{
		int32_t L_14 = Vector4_GetHashCode_mCA7B312F8CA141F6F25BABDDF406F3D2BDD5E895((Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)G_B11_0, /*hidden argument*/NULL);
		G_B12_0 = L_14;
		G_B12_1 = G_B11_1;
		G_B12_2 = G_B11_2;
	}

IL_009f:
	{
		int32_t L_15 = ValueTuple_CombineHashCodes_m6D3138F0BA3D04CA1B640620E47716F05EB2DEBE((int32_t)G_B12_2, (int32_t)G_B12_1, (int32_t)G_B12_0, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t ValueTuple_3_GetHashCode_mD7AC1A1D2BFFD62FC7C766D1595076CE613BFC4E_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * _thisAdjusted = reinterpret_cast<ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *>(__this + _offset);
	return ValueTuple_3_GetHashCode_mD7AC1A1D2BFFD62FC7C766D1595076CE613BFC4E(_thisAdjusted, method);
}
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_System_Collections_IStructuralEquatable_GetHashCode_mBC40DB422DFF4D33476C79DE080198048A6A167F_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = ___comparer0;
		int32_t L_1 = ValueTuple_3_GetHashCodeCore_m6D7023B397C18B2B04D077227DDC88F0E0282731((ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)(ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *)__this, (RuntimeObject*)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 24));
		return L_1;
	}
}
IL2CPP_EXTERN_C  int32_t ValueTuple_3_System_Collections_IStructuralEquatable_GetHashCode_mBC40DB422DFF4D33476C79DE080198048A6A167F_AdjustorThunk (RuntimeObject * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * _thisAdjusted = reinterpret_cast<ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *>(__this + _offset);
	return ValueTuple_3_System_Collections_IStructuralEquatable_GetHashCode_mBC40DB422DFF4D33476C79DE080198048A6A167F(_thisAdjusted, ___comparer0, method);
}
// System.Int32 System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::GetHashCodeCore(System.Collections.IEqualityComparer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ValueTuple_3_GetHashCodeCore_m6D7023B397C18B2B04D077227DDC88F0E0282731_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ValueTuple_3_GetHashCodeCore_m6D7023B397C18B2B04D077227DDC88F0E0282731_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = ___comparer0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_1 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item1_0();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_2 = L_1;
		RuntimeObject * L_3 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 11), &L_2);
		NullCheck((RuntimeObject*)L_0);
		int32_t L_4 = InterfaceFuncInvoker1< int32_t, RuntimeObject * >::Invoke(1 /* System.Int32 System.Collections.IEqualityComparer::GetHashCode(System.Object) */, IEqualityComparer_t6C4C1F04B21BDE1E4B84BD6EC7DE494C186D6C68_il2cpp_TypeInfo_var, (RuntimeObject*)L_0, (RuntimeObject *)L_3);
		RuntimeObject* L_5 = ___comparer0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_6 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item2_1();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_7 = L_6;
		RuntimeObject * L_8 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 12), &L_7);
		NullCheck((RuntimeObject*)L_5);
		int32_t L_9 = InterfaceFuncInvoker1< int32_t, RuntimeObject * >::Invoke(1 /* System.Int32 System.Collections.IEqualityComparer::GetHashCode(System.Object) */, IEqualityComparer_t6C4C1F04B21BDE1E4B84BD6EC7DE494C186D6C68_il2cpp_TypeInfo_var, (RuntimeObject*)L_5, (RuntimeObject *)L_8);
		RuntimeObject* L_10 = ___comparer0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_11 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_Item3_2();
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_12 = L_11;
		RuntimeObject * L_13 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 13), &L_12);
		NullCheck((RuntimeObject*)L_10);
		int32_t L_14 = InterfaceFuncInvoker1< int32_t, RuntimeObject * >::Invoke(1 /* System.Int32 System.Collections.IEqualityComparer::GetHashCode(System.Object) */, IEqualityComparer_t6C4C1F04B21BDE1E4B84BD6EC7DE494C186D6C68_il2cpp_TypeInfo_var, (RuntimeObject*)L_10, (RuntimeObject *)L_13);
		int32_t L_15 = ValueTuple_CombineHashCodes_m6D3138F0BA3D04CA1B640620E47716F05EB2DEBE((int32_t)L_4, (int32_t)L_9, (int32_t)L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t ValueTuple_3_GetHashCodeCore_m6D7023B397C18B2B04D077227DDC88F0E0282731_AdjustorThunk (RuntimeObject * __this, RuntimeObject* ___comparer0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * _thisAdjusted = reinterpret_cast<ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *>(__this + _offset);
	return ValueTuple_3_GetHashCodeCore_m6D7023B397C18B2B04D077227DDC88F0E0282731(_thisAdjusted, ___comparer0, method);
}
// System.String System.ValueTuple`3<UnityEngine.Vector4,UnityEngine.Vector4,UnityEngine.Vector4>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* ValueTuple_3_ToString_m2F574559A50EDFB3E99D4CCD1B85A772BB84798F_gshared (ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ValueTuple_3_ToString_m2F574559A50EDFB3E99D4CCD1B85A772BB84798F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  V_0;
	memset((&V_0), 0, sizeof(V_0));
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  V_1;
	memset((&V_1), 0, sizeof(V_1));
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  V_2;
	memset((&V_2), 0, sizeof(V_2));
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B3_0 = NULL;
	int32_t G_B3_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B3_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B3_3 = NULL;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B1_0 = NULL;
	int32_t G_B1_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B1_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B1_3 = NULL;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B2_0 = NULL;
	int32_t G_B2_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B2_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B2_3 = NULL;
	String_t* G_B4_0 = NULL;
	int32_t G_B4_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B4_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B4_3 = NULL;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B7_0 = NULL;
	int32_t G_B7_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B7_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B7_3 = NULL;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B5_0 = NULL;
	int32_t G_B5_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B5_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B5_3 = NULL;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B6_0 = NULL;
	int32_t G_B6_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B6_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B6_3 = NULL;
	String_t* G_B8_0 = NULL;
	int32_t G_B8_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B8_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B8_3 = NULL;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B11_0 = NULL;
	int32_t G_B11_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B11_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B11_3 = NULL;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B9_0 = NULL;
	int32_t G_B9_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B9_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B9_3 = NULL;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * G_B10_0 = NULL;
	int32_t G_B10_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B10_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B10_3 = NULL;
	String_t* G_B12_0 = NULL;
	int32_t G_B12_1 = 0;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B12_2 = NULL;
	StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* G_B12_3 = NULL;
	{
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_0 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)(StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)SZArrayNew(StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A_il2cpp_TypeInfo_var, (uint32_t)7);
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_1 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)L_0;
		NullCheck(L_1);
		ArrayElementTypeCheck (L_1, _stringLiteralA3DFC0C77ACADE0EE48DCC73E795A597D0270A73);
		(L_1)->SetAt(static_cast<il2cpp_array_size_t>(0), (String_t*)_stringLiteralA3DFC0C77ACADE0EE48DCC73E795A597D0270A73);
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_2 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)L_1;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * L_3 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)__this->get_address_of_Item1_0();
		il2cpp_codegen_initobj((&V_0), sizeof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 ));
		G_B3_0 = L_3;
		G_B3_1 = 1;
		G_B3_2 = L_2;
		G_B3_3 = L_2;
		goto IL_003a;
		G_B1_0 = L_3;
		G_B1_1 = 1;
		G_B1_2 = L_2;
		G_B1_3 = L_2;
	}
	{
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_5 = (*(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)G_B1_0);
		V_0 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_5;
		G_B3_0 = (&V_0);
		G_B3_1 = G_B1_1;
		G_B3_2 = G_B1_2;
		G_B3_3 = G_B1_3;
		goto IL_003a;
		G_B2_0 = (&V_0);
		G_B2_1 = G_B1_1;
		G_B2_2 = G_B1_2;
		G_B2_3 = G_B1_3;
	}
	{
		G_B4_0 = ((String_t*)(NULL));
		G_B4_1 = G_B2_1;
		G_B4_2 = G_B2_2;
		G_B4_3 = G_B2_3;
		goto IL_0045;
	}

IL_003a:
	{
		String_t* L_7 = Vector4_ToString_mF2D17142EBD75E91BC718B3E347F614AC45E9040((Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)G_B3_0, /*hidden argument*/NULL);
		G_B4_0 = L_7;
		G_B4_1 = G_B3_1;
		G_B4_2 = G_B3_2;
		G_B4_3 = G_B3_3;
	}

IL_0045:
	{
		NullCheck(G_B4_2);
		ArrayElementTypeCheck (G_B4_2, G_B4_0);
		(G_B4_2)->SetAt(static_cast<il2cpp_array_size_t>(G_B4_1), (String_t*)G_B4_0);
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_8 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)G_B4_3;
		NullCheck(L_8);
		ArrayElementTypeCheck (L_8, _stringLiteral758733BDBED83CBFF4F635AC26CA92AAE477F75D);
		(L_8)->SetAt(static_cast<il2cpp_array_size_t>(2), (String_t*)_stringLiteral758733BDBED83CBFF4F635AC26CA92AAE477F75D);
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_9 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)L_8;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * L_10 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)__this->get_address_of_Item2_1();
		il2cpp_codegen_initobj((&V_1), sizeof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 ));
		G_B7_0 = L_10;
		G_B7_1 = 3;
		G_B7_2 = L_9;
		G_B7_3 = L_9;
		goto IL_007a;
		G_B5_0 = L_10;
		G_B5_1 = 3;
		G_B5_2 = L_9;
		G_B5_3 = L_9;
	}
	{
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_12 = (*(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)G_B5_0);
		V_1 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_12;
		G_B7_0 = (&V_1);
		G_B7_1 = G_B5_1;
		G_B7_2 = G_B5_2;
		G_B7_3 = G_B5_3;
		goto IL_007a;
		G_B6_0 = (&V_1);
		G_B6_1 = G_B5_1;
		G_B6_2 = G_B5_2;
		G_B6_3 = G_B5_3;
	}
	{
		G_B8_0 = ((String_t*)(NULL));
		G_B8_1 = G_B6_1;
		G_B8_2 = G_B6_2;
		G_B8_3 = G_B6_3;
		goto IL_0085;
	}

IL_007a:
	{
		String_t* L_14 = Vector4_ToString_mF2D17142EBD75E91BC718B3E347F614AC45E9040((Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)G_B7_0, /*hidden argument*/NULL);
		G_B8_0 = L_14;
		G_B8_1 = G_B7_1;
		G_B8_2 = G_B7_2;
		G_B8_3 = G_B7_3;
	}

IL_0085:
	{
		NullCheck(G_B8_2);
		ArrayElementTypeCheck (G_B8_2, G_B8_0);
		(G_B8_2)->SetAt(static_cast<il2cpp_array_size_t>(G_B8_1), (String_t*)G_B8_0);
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_15 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)G_B8_3;
		NullCheck(L_15);
		ArrayElementTypeCheck (L_15, _stringLiteral758733BDBED83CBFF4F635AC26CA92AAE477F75D);
		(L_15)->SetAt(static_cast<il2cpp_array_size_t>(4), (String_t*)_stringLiteral758733BDBED83CBFF4F635AC26CA92AAE477F75D);
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_16 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)L_15;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 * L_17 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)__this->get_address_of_Item3_2();
		il2cpp_codegen_initobj((&V_2), sizeof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 ));
		G_B11_0 = L_17;
		G_B11_1 = 5;
		G_B11_2 = L_16;
		G_B11_3 = L_16;
		goto IL_00ba;
		G_B9_0 = L_17;
		G_B9_1 = 5;
		G_B9_2 = L_16;
		G_B9_3 = L_16;
	}
	{
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_19 = (*(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)G_B9_0);
		V_2 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_19;
		G_B11_0 = (&V_2);
		G_B11_1 = G_B9_1;
		G_B11_2 = G_B9_2;
		G_B11_3 = G_B9_3;
		goto IL_00ba;
		G_B10_0 = (&V_2);
		G_B10_1 = G_B9_1;
		G_B10_2 = G_B9_2;
		G_B10_3 = G_B9_3;
	}
	{
		G_B12_0 = ((String_t*)(NULL));
		G_B12_1 = G_B10_1;
		G_B12_2 = G_B10_2;
		G_B12_3 = G_B10_3;
		goto IL_00c5;
	}

IL_00ba:
	{
		String_t* L_21 = Vector4_ToString_mF2D17142EBD75E91BC718B3E347F614AC45E9040((Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)G_B11_0, /*hidden argument*/NULL);
		G_B12_0 = L_21;
		G_B12_1 = G_B11_1;
		G_B12_2 = G_B11_2;
		G_B12_3 = G_B11_3;
	}

IL_00c5:
	{
		NullCheck(G_B12_2);
		ArrayElementTypeCheck (G_B12_2, G_B12_0);
		(G_B12_2)->SetAt(static_cast<il2cpp_array_size_t>(G_B12_1), (String_t*)G_B12_0);
		StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A* L_22 = (StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)G_B12_3;
		NullCheck(L_22);
		ArrayElementTypeCheck (L_22, _stringLiteralB3F14BF976EFD974E34846B742502C802FABAE9D);
		(L_22)->SetAt(static_cast<il2cpp_array_size_t>(6), (String_t*)_stringLiteralB3F14BF976EFD974E34846B742502C802FABAE9D);
		String_t* L_23 = String_Concat_mFEA7EFA1A6E75B96B1B7BC4526AAC864BFF83CC9((StringU5BU5D_tACEBFEDE350025B554CD507C9AE8FFE49359549A*)L_22, /*hidden argument*/NULL);
		return L_23;
	}
}
IL2CPP_EXTERN_C  String_t* ValueTuple_3_ToString_m2F574559A50EDFB3E99D4CCD1B85A772BB84798F_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 * _thisAdjusted = reinterpret_cast<ValueTuple_3_t119A9C2BAE92B64219BA73BB34C7AAA6E0716EF5 *>(__this + _offset);
	return ValueTuple_3_ToString_m2F574559A50EDFB3E99D4CCD1B85A772BB84798F(_thisAdjusted, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// T UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::get_value()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_get_value_m3E845D85BE539B633E083E8F1F89DB570FD83502_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * __this, const RuntimeMethod* method)
{
	{
		// get => m_Value;
		bool L_0 = (bool)__this->get_m_Value_2();
		return L_0;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::set_value(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_set_value_mEFB6F26C0CB02ED1C55D63E76BC5CD7E95CAAEA9_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * __this, bool ___value0, const RuntimeMethod* method)
{
	{
		// set => m_Value = value;
		bool L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_m0D0132882223A6E44C02FBB9D0F0B5FB59E8F761_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * __this, const RuntimeMethod* method)
{
	bool V_0 = false;
	{
		// : this(default, false)
		il2cpp_codegen_initobj((&V_0), sizeof(bool));
		bool L_0 = V_0;
		NullCheck((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)__this);
		((  void (*) (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *, bool, bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)__this, (bool)L_0, (bool)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::.ctor(T,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_m8B4B2A4C71BE551FC2F2E8F17DC0297E845400B9_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * __this, bool ___value0, bool ___overrideState1, const RuntimeMethod* method)
{
	{
		// protected VolumeParameter(T value, bool overrideState)
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VolumeParameter__ctor_m0F5E882EFAE8F1B7AE0DFF62945878D71CA63D54((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, /*hidden argument*/NULL);
		// m_Value = value;
		bool L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		// this.overrideState = overrideState;
		bool L_1 = ___overrideState1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)L_1);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::Interp(UnityEngine.Rendering.VolumeParameter,UnityEngine.Rendering.VolumeParameter,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_m0E5BE50E0E38628996AF2D9A15C60EF2A1F70A68_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___from0, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___to1, float ___t2, const RuntimeMethod* method)
{
	{
		// Interp(from.GetValue<T>(), to.GetValue<T>(), t);
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___from0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		bool L_1 = ((  bool (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_2 = ___to1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2);
		bool L_3 = ((  bool (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		float L_4 = ___t2;
		NullCheck((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)__this);
		VirtActionInvoker3< bool, bool, float >::Invoke(14 /* System.Void UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::Interp(T,T,System.Single) */, (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)__this, (bool)L_1, (bool)L_3, (float)L_4);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::Interp(T,T,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_m170C99960980C9E45B4031E54C6BBC2E43541D90_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * __this, bool ___from0, bool ___to1, float ___t2, const RuntimeMethod* method)
{
	VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * G_B2_0 = NULL;
	VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * G_B1_0 = NULL;
	bool G_B3_0 = false;
	VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * G_B3_1 = NULL;
	{
		// m_Value = t > 0f ? to : from;
		float L_0 = ___t2;
		G_B1_0 = ((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)(__this));
		if ((((float)L_0) > ((float)(0.0f))))
		{
			G_B2_0 = ((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)(__this));
			goto IL_000c;
		}
	}
	{
		bool L_1 = ___from0;
		G_B3_0 = L_1;
		G_B3_1 = ((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)(G_B1_0));
		goto IL_000d;
	}

IL_000c:
	{
		bool L_2 = ___to1;
		G_B3_0 = L_2;
		G_B3_1 = ((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)(G_B2_0));
	}

IL_000d:
	{
		NullCheck(G_B3_1);
		G_B3_1->set_m_Value_2(G_B3_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::Override(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Override_mDBC5EEB3B3A8B3D7E50A1B5F7CA95BF2ED7DBF3A_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * __this, bool ___x0, const RuntimeMethod* method)
{
	{
		// overrideState = true;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)1);
		// m_Value = x;
		bool L_0 = ___x0;
		__this->set_m_Value_2(L_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::SetValue(UnityEngine.Rendering.VolumeParameter)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_SetValue_m5088BA601DD325F53494B4E12BF34A966B546602_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___parameter0, const RuntimeMethod* method)
{
	{
		// m_Value = parameter.GetValue<T>();
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___parameter0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		bool L_1 = ((  bool (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		__this->set_m_Value_2(L_1);
		// }
		return;
	}
}
// System.Int32 UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t VolumeParameter_1_GetHashCode_m8B5099B9EC03E860C9F7FCC93B4C268A61039F5C_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	bool V_2 = false;
	{
		// int hash = 17;
		V_0 = (int32_t)((int32_t)17);
		// hash = hash * 23 + overrideState.GetHashCode();
		int32_t L_0 = V_0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_1 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		V_1 = (bool)L_1;
		int32_t L_2 = Boolean_GetHashCode_m03AF8B3CECAE9106C44A00E3B33E51CBFC45C411((bool*)(bool*)(&V_1), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)((int32_t)23))), (int32_t)L_2));
		// if (!EqualityComparer<T>.Default.Equals(value, default)) // Catches null for references with boxing of value types
		EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7 * L_3 = ((  EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		NullCheck((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)__this);
		bool L_4 = VirtFuncInvoker0< bool >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::get_value() */, (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)__this);
		il2cpp_codegen_initobj((&V_2), sizeof(bool));
		bool L_5 = V_2;
		NullCheck((EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7 *)L_3);
		bool L_6 = VirtFuncInvoker2< bool, bool, bool >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<System.Boolean>::Equals(!0,!0) */, (EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7 *)L_3, (bool)L_4, (bool)L_5);
		if (L_6)
		{
			goto IL_004c;
		}
	}
	{
		// hash = hash * 23 + value.GetHashCode();
		int32_t L_7 = V_0;
		NullCheck((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)__this);
		bool L_8 = VirtFuncInvoker0< bool >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::get_value() */, (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)__this);
		V_2 = (bool)L_8;
		int32_t L_9 = Boolean_GetHashCode_m03AF8B3CECAE9106C44A00E3B33E51CBFC45C411((bool*)(bool*)(&V_2), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_7, (int32_t)((int32_t)23))), (int32_t)L_9));
	}

IL_004c:
	{
		// return hash;
		int32_t L_10 = V_0;
		return L_10;
	}
}
// System.String UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* VolumeParameter_1_ToString_m5DB1BD0BE00D6CBA3B55CE448643B9B519EB23A5_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_ToString_m5DB1BD0BE00D6CBA3B55CE448643B9B519EB23A5_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public override string ToString() => $"{value} ({overrideState})";
		NullCheck((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)__this);
		bool L_0 = VirtFuncInvoker0< bool >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::get_value() */, (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)__this);
		bool L_1 = L_0;
		RuntimeObject * L_2 = Box(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7), &L_1);
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_3 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_4 = L_3;
		RuntimeObject * L_5 = Box(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_il2cpp_TypeInfo_var, &L_4);
		String_t* L_6 = String_Format_m8D1CB0410C35E052A53AE957C914C841E54BAB66((String_t*)_stringLiteralA5E215A6DBE803E908043576B18C4FAD26AD44F7, (RuntimeObject *)L_2, (RuntimeObject *)L_5, /*hidden argument*/NULL);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::op_Equality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Equality_mA4DBBA0F2A42E2B3E082A0600ABF875FDFCF3B60_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * ___lhs0, bool ___rhs1, const RuntimeMethod* method)
{
	bool V_0 = false;
	{
		// public static bool operator==(VolumeParameter<T> lhs, T rhs) => lhs != null && !ReferenceEquals(lhs.value, null) && lhs.value.Equals(rhs);
		VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * L_0 = ___lhs0;
		if (!L_0)
		{
			goto IL_002b;
		}
	}
	{
		VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * L_1 = ___lhs0;
		NullCheck((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)L_1);
		bool L_2 = VirtFuncInvoker0< bool >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::get_value() */, (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)L_1);
	}
	{
		VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * L_3 = ___lhs0;
		NullCheck((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)L_3);
		bool L_4 = VirtFuncInvoker0< bool >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::get_value() */, (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)L_3);
		V_0 = (bool)L_4;
		bool L_5 = ___rhs1;
		bool L_6 = L_5;
		RuntimeObject * L_7 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 7), &L_6);
		bool L_8 = Boolean_Equals_mA2FC01AF136159906F30A85C950097BE67C824B8((bool*)(bool*)(&V_0), (RuntimeObject *)L_7, /*hidden argument*/NULL);
		return L_8;
	}

IL_002b:
	{
		return (bool)0;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::op_Inequality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Inequality_mE8EF05EA985B67615FBB5F3364182178C08F17B6_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * ___lhs0, bool ___rhs1, const RuntimeMethod* method)
{
	{
		// public static bool operator!=(VolumeParameter<T> lhs, T rhs) => !(lhs == rhs);
		VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * L_0 = ___lhs0;
		bool L_1 = ___rhs1;
		bool L_2 = ((  bool (*) (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *, bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)L_0, (bool)L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		return (bool)((((int32_t)L_2) == ((int32_t)0))? 1 : 0);
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::Equals(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_mBD69E059AA04AD9A3F061C2830F69F4B1708A591_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * __this, VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * ___other0, const RuntimeMethod* method)
{
	{
		// if (ReferenceEquals(null, other))
		VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * L_0 = ___other0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, other))
		VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * L_1 = ___other0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)__this) == ((RuntimeObject*)(VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// return EqualityComparer<T>.Default.Equals(m_Value, other.m_Value);
		EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7 * L_2 = ((  EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		bool L_3 = (bool)__this->get_m_Value_2();
		VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * L_4 = ___other0;
		NullCheck(L_4);
		bool L_5 = (bool)L_4->get_m_Value_2();
		NullCheck((EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7 *)L_2);
		bool L_6 = VirtFuncInvoker2< bool, bool, bool >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<System.Boolean>::Equals(!0,!0) */, (EqualityComparer_1_tA00ECA27EEC6CA6AADD7F115EB7E6A654C8E96E7 *)L_2, (bool)L_3, (bool)L_5);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m7D4639EC08F8D19DA960892524BA475332EB6E6A_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_Equals_m7D4639EC08F8D19DA960892524BA475332EB6E6A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (ReferenceEquals(null, obj))
		RuntimeObject * L_0 = ___obj0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, obj))
		RuntimeObject * L_1 = ___obj0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)__this) == ((RuntimeObject*)(RuntimeObject *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// if (obj.GetType() != GetType())
		RuntimeObject * L_2 = ___obj0;
		NullCheck((RuntimeObject *)L_2);
		Type_t * L_3 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)L_2, /*hidden argument*/NULL);
		NullCheck((RuntimeObject *)__this);
		Type_t * L_4 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		bool L_5 = Type_op_Inequality_m6DDC5E923203A79BF505F9275B694AD3FAA36DB0((Type_t *)L_3, (Type_t *)L_4, /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_0020;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0020:
	{
		// return Equals((VolumeParameter<T>)obj);
		RuntimeObject * L_6 = ___obj0;
		NullCheck((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)__this);
		bool L_7 = ((  bool (*) (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *, VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)__this, (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)((VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 *)Castclass((RuntimeObject*)L_6, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10))), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_7;
	}
}
// T UnityEngine.Rendering.VolumeParameter`1<System.Boolean>::op_Explicit(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Explicit_mA7E752C87EC2EDA96C08C0AC028E3AAC16219401_gshared (VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * ___prop0, const RuntimeMethod* method)
{
	{
		// public static explicit operator T(VolumeParameter<T> prop) => prop.m_Value;
		VolumeParameter_1_t5096F05CCD6852CF262ACF232A401187B9E88201 * L_0 = ___prop0;
		NullCheck(L_0);
		bool L_1 = (bool)L_0->get_m_Value_2();
		return L_1;
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
// T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::get_value()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  VolumeParameter_1_get_value_mDDF741FD66D141BED17CE309F31F6DCF7DC0319B_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * __this, const RuntimeMethod* method)
{
	{
		// get => m_Value;
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_0 = (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )__this->get_m_Value_2();
		return L_0;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::set_value(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_set_value_m6481AFF824797BE91380D717E7F0845D9820CA59_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * __this, Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  ___value0, const RuntimeMethod* method)
{
	{
		// set => m_Value = value;
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_mEF13A33E2DA2F05E7D3BE67245171A57AF26799C_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * __this, const RuntimeMethod* method)
{
	Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// : this(default, false)
		il2cpp_codegen_initobj((&V_0), sizeof(Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 ));
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_0 = V_0;
		NullCheck((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)__this);
		((  void (*) (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *, Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 , bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)__this, (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )L_0, (bool)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::.ctor(T,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_m6515B91A42D9B7CE47313F03EC759265DD3FF06A_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * __this, Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  ___value0, bool ___overrideState1, const RuntimeMethod* method)
{
	{
		// protected VolumeParameter(T value, bool overrideState)
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VolumeParameter__ctor_m0F5E882EFAE8F1B7AE0DFF62945878D71CA63D54((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, /*hidden argument*/NULL);
		// m_Value = value;
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		// this.overrideState = overrideState;
		bool L_1 = ___overrideState1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)L_1);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::Interp(UnityEngine.Rendering.VolumeParameter,UnityEngine.Rendering.VolumeParameter,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_mF7FD4A0093506BB7FD6780EA89EAB7CFB043EE0E_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___from0, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___to1, float ___t2, const RuntimeMethod* method)
{
	{
		// Interp(from.GetValue<T>(), to.GetValue<T>(), t);
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___from0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_1 = ((  Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_2 = ___to1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_3 = ((  Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		float L_4 = ___t2;
		NullCheck((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)__this);
		VirtActionInvoker3< Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 , Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 , float >::Invoke(14 /* System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::Interp(T,T,System.Single) */, (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)__this, (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )L_1, (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )L_3, (float)L_4);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::Interp(T,T,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_m11C3452F9CD830816C7E67AC0F4B7B613AF99C6B_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * __this, Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  ___from0, Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  ___to1, float ___t2, const RuntimeMethod* method)
{
	VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * G_B2_0 = NULL;
	VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * G_B1_0 = NULL;
	Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  G_B3_0;
	memset((&G_B3_0), 0, sizeof(G_B3_0));
	VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * G_B3_1 = NULL;
	{
		// m_Value = t > 0f ? to : from;
		float L_0 = ___t2;
		G_B1_0 = ((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)(__this));
		if ((((float)L_0) > ((float)(0.0f))))
		{
			G_B2_0 = ((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)(__this));
			goto IL_000c;
		}
	}
	{
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_1 = ___from0;
		G_B3_0 = L_1;
		G_B3_1 = ((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)(G_B1_0));
		goto IL_000d;
	}

IL_000c:
	{
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_2 = ___to1;
		G_B3_0 = L_2;
		G_B3_1 = ((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)(G_B2_0));
	}

IL_000d:
	{
		NullCheck(G_B3_1);
		G_B3_1->set_m_Value_2(G_B3_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::Override(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Override_mE067FEFAD4F8A7F25CBE81FE9EA1DF29D38E0924_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * __this, Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  ___x0, const RuntimeMethod* method)
{
	{
		// overrideState = true;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)1);
		// m_Value = x;
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_0 = ___x0;
		__this->set_m_Value_2(L_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::SetValue(UnityEngine.Rendering.VolumeParameter)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_SetValue_mBC9FB554F476AFE074D70972EBA3486FBAB3BA65_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___parameter0, const RuntimeMethod* method)
{
	{
		// m_Value = parameter.GetValue<T>();
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___parameter0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_1 = ((  Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		__this->set_m_Value_2(L_1);
		// }
		return;
	}
}
// System.Int32 UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t VolumeParameter_1_GetHashCode_mB9788684E54C7803CAA413548B442D376A7770C7_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  V_2;
	memset((&V_2), 0, sizeof(V_2));
	{
		// int hash = 17;
		V_0 = (int32_t)((int32_t)17);
		// hash = hash * 23 + overrideState.GetHashCode();
		int32_t L_0 = V_0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_1 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		V_1 = (bool)L_1;
		int32_t L_2 = Boolean_GetHashCode_m03AF8B3CECAE9106C44A00E3B33E51CBFC45C411((bool*)(bool*)(&V_1), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)((int32_t)23))), (int32_t)L_2));
		// if (!EqualityComparer<T>.Default.Equals(value, default)) // Catches null for references with boxing of value types
		EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5 * L_3 = ((  EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		NullCheck((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)__this);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_4 = VirtFuncInvoker0< Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::get_value() */, (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)__this);
		il2cpp_codegen_initobj((&V_2), sizeof(Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 ));
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_5 = V_2;
		NullCheck((EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5 *)L_3);
		bool L_6 = VirtFuncInvoker2< bool, Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 , Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<UnityEngine.Color>::Equals(!0,!0) */, (EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5 *)L_3, (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )L_4, (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )L_5);
		if (L_6)
		{
			goto IL_004c;
		}
	}
	{
		// hash = hash * 23 + value.GetHashCode();
		int32_t L_7 = V_0;
		NullCheck((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)__this);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_8 = VirtFuncInvoker0< Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::get_value() */, (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)__this);
		V_2 = (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )L_8;
		int32_t L_9 = Color_GetHashCode_mAF5E7EE6AFA983D3FA5E3D316E672EE1511F97CF((Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 *)(Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 *)(&V_2), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_7, (int32_t)((int32_t)23))), (int32_t)L_9));
	}

IL_004c:
	{
		// return hash;
		int32_t L_10 = V_0;
		return L_10;
	}
}
// System.String UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* VolumeParameter_1_ToString_mEF69CB91E8EFCB0C2253CA84E62B45374E69973E_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_ToString_mEF69CB91E8EFCB0C2253CA84E62B45374E69973E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public override string ToString() => $"{value} ({overrideState})";
		NullCheck((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)__this);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_0 = VirtFuncInvoker0< Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::get_value() */, (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)__this);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_1 = L_0;
		RuntimeObject * L_2 = Box(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7), &L_1);
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_3 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_4 = L_3;
		RuntimeObject * L_5 = Box(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_il2cpp_TypeInfo_var, &L_4);
		String_t* L_6 = String_Format_m8D1CB0410C35E052A53AE957C914C841E54BAB66((String_t*)_stringLiteralA5E215A6DBE803E908043576B18C4FAD26AD44F7, (RuntimeObject *)L_2, (RuntimeObject *)L_5, /*hidden argument*/NULL);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::op_Equality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Equality_mCEC61DE2A0130E67B8C188632BB6B7D9B9862801_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * ___lhs0, Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  ___rhs1, const RuntimeMethod* method)
{
	Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// public static bool operator==(VolumeParameter<T> lhs, T rhs) => lhs != null && !ReferenceEquals(lhs.value, null) && lhs.value.Equals(rhs);
		VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * L_0 = ___lhs0;
		if (!L_0)
		{
			goto IL_002b;
		}
	}
	{
		VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * L_1 = ___lhs0;
		NullCheck((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)L_1);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_2 = VirtFuncInvoker0< Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::get_value() */, (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)L_1);
	}
	{
		VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * L_3 = ___lhs0;
		NullCheck((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)L_3);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_4 = VirtFuncInvoker0< Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::get_value() */, (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)L_3);
		V_0 = (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )L_4;
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_5 = ___rhs1;
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_6 = L_5;
		RuntimeObject * L_7 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 7), &L_6);
		bool L_8 = Color_Equals_m90F8A5EF85416D809F5E3C0ACCADDD4F299AD8FC((Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 *)(Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 *)(&V_0), (RuntimeObject *)L_7, /*hidden argument*/NULL);
		return L_8;
	}

IL_002b:
	{
		return (bool)0;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::op_Inequality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Inequality_m73D5AC0BDC5F9A8C4C7565B2A582005DB04CA26F_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * ___lhs0, Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  ___rhs1, const RuntimeMethod* method)
{
	{
		// public static bool operator!=(VolumeParameter<T> lhs, T rhs) => !(lhs == rhs);
		VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * L_0 = ___lhs0;
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_1 = ___rhs1;
		bool L_2 = ((  bool (*) (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *, Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)L_0, (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		return (bool)((((int32_t)L_2) == ((int32_t)0))? 1 : 0);
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::Equals(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m96318FE6036D18670F08C84037810D0F451C756C_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * __this, VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * ___other0, const RuntimeMethod* method)
{
	{
		// if (ReferenceEquals(null, other))
		VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * L_0 = ___other0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, other))
		VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * L_1 = ___other0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)__this) == ((RuntimeObject*)(VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// return EqualityComparer<T>.Default.Equals(m_Value, other.m_Value);
		EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5 * L_2 = ((  EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_3 = (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )__this->get_m_Value_2();
		VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * L_4 = ___other0;
		NullCheck(L_4);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_5 = (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )L_4->get_m_Value_2();
		NullCheck((EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5 *)L_2);
		bool L_6 = VirtFuncInvoker2< bool, Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 , Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<UnityEngine.Color>::Equals(!0,!0) */, (EqualityComparer_1_tDDC4EB900D1412B76DDE04391C11C8F3E7C9F0F5 *)L_2, (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )L_3, (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )L_5);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m87E3D7F71B21A30E632D727C0B0F2CC1234AA260_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_Equals_m87E3D7F71B21A30E632D727C0B0F2CC1234AA260_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (ReferenceEquals(null, obj))
		RuntimeObject * L_0 = ___obj0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, obj))
		RuntimeObject * L_1 = ___obj0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)__this) == ((RuntimeObject*)(RuntimeObject *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// if (obj.GetType() != GetType())
		RuntimeObject * L_2 = ___obj0;
		NullCheck((RuntimeObject *)L_2);
		Type_t * L_3 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)L_2, /*hidden argument*/NULL);
		NullCheck((RuntimeObject *)__this);
		Type_t * L_4 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		bool L_5 = Type_op_Inequality_m6DDC5E923203A79BF505F9275B694AD3FAA36DB0((Type_t *)L_3, (Type_t *)L_4, /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_0020;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0020:
	{
		// return Equals((VolumeParameter<T>)obj);
		RuntimeObject * L_6 = ___obj0;
		NullCheck((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)__this);
		bool L_7 = ((  bool (*) (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *, VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)__this, (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)((VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A *)Castclass((RuntimeObject*)L_6, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10))), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_7;
	}
}
// T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Color>::op_Explicit(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  VolumeParameter_1_op_Explicit_mD5BC40EAB6AD3C9DE19E0BF3E0A27175D0DB3290_gshared (VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * ___prop0, const RuntimeMethod* method)
{
	{
		// public static explicit operator T(VolumeParameter<T> prop) => prop.m_Value;
		VolumeParameter_1_t39D06592B6BAA00FD18C8FD8893203D1A4F7D11A * L_0 = ___prop0;
		NullCheck(L_0);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_1 = (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )L_0->get_m_Value_2();
		return L_1;
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
// T UnityEngine.Rendering.VolumeParameter`1<System.Int32>::get_value()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t VolumeParameter_1_get_value_mB51CC0592DA5AEB51C2A1002335D283B65D222D0_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * __this, const RuntimeMethod* method)
{
	{
		// get => m_Value;
		int32_t L_0 = (int32_t)__this->get_m_Value_2();
		return L_0;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32>::set_value(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_set_value_m7D157373E6F3E641D7A1D7FA26F3CA2D70C421F6_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		// set => m_Value = value;
		int32_t L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_mC490CE61A02B0A0653947343B22B53FABD8C8906_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		// : this(default, false)
		il2cpp_codegen_initobj((&V_0), sizeof(int32_t));
		int32_t L_0 = V_0;
		NullCheck((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)__this);
		((  void (*) (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *, int32_t, bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)__this, (int32_t)L_0, (bool)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32>::.ctor(T,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_mE923D5F987441B577192CC881DFDA6EE84DC4598_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * __this, int32_t ___value0, bool ___overrideState1, const RuntimeMethod* method)
{
	{
		// protected VolumeParameter(T value, bool overrideState)
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VolumeParameter__ctor_m0F5E882EFAE8F1B7AE0DFF62945878D71CA63D54((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, /*hidden argument*/NULL);
		// m_Value = value;
		int32_t L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		// this.overrideState = overrideState;
		bool L_1 = ___overrideState1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)L_1);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32>::Interp(UnityEngine.Rendering.VolumeParameter,UnityEngine.Rendering.VolumeParameter,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_mB700B2F00212FE7473038395F727F150CB5F1DF3_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___from0, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___to1, float ___t2, const RuntimeMethod* method)
{
	{
		// Interp(from.GetValue<T>(), to.GetValue<T>(), t);
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___from0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		int32_t L_1 = ((  int32_t (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_2 = ___to1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2);
		int32_t L_3 = ((  int32_t (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		float L_4 = ___t2;
		NullCheck((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)__this);
		VirtActionInvoker3< int32_t, int32_t, float >::Invoke(14 /* System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32>::Interp(T,T,System.Single) */, (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)__this, (int32_t)L_1, (int32_t)L_3, (float)L_4);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32>::Interp(T,T,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_m365FAA0E37B842EB5C3A0A71653FE5F08BE52811_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * __this, int32_t ___from0, int32_t ___to1, float ___t2, const RuntimeMethod* method)
{
	VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * G_B2_0 = NULL;
	VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * G_B1_0 = NULL;
	int32_t G_B3_0 = 0;
	VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * G_B3_1 = NULL;
	{
		// m_Value = t > 0f ? to : from;
		float L_0 = ___t2;
		G_B1_0 = ((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)(__this));
		if ((((float)L_0) > ((float)(0.0f))))
		{
			G_B2_0 = ((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)(__this));
			goto IL_000c;
		}
	}
	{
		int32_t L_1 = ___from0;
		G_B3_0 = L_1;
		G_B3_1 = ((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)(G_B1_0));
		goto IL_000d;
	}

IL_000c:
	{
		int32_t L_2 = ___to1;
		G_B3_0 = L_2;
		G_B3_1 = ((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)(G_B2_0));
	}

IL_000d:
	{
		NullCheck(G_B3_1);
		G_B3_1->set_m_Value_2(G_B3_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32>::Override(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Override_mF27318A7208C183C0E74249859343B82DDB54E9D_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * __this, int32_t ___x0, const RuntimeMethod* method)
{
	{
		// overrideState = true;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)1);
		// m_Value = x;
		int32_t L_0 = ___x0;
		__this->set_m_Value_2(L_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32>::SetValue(UnityEngine.Rendering.VolumeParameter)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_SetValue_m29087BA2004CB999953BAAFB68837B35D8229A88_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___parameter0, const RuntimeMethod* method)
{
	{
		// m_Value = parameter.GetValue<T>();
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___parameter0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		int32_t L_1 = ((  int32_t (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		__this->set_m_Value_2(L_1);
		// }
		return;
	}
}
// System.Int32 UnityEngine.Rendering.VolumeParameter`1<System.Int32>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t VolumeParameter_1_GetHashCode_m12AC65BDA8FAC3B149CC4A714B57D024F7EBB318_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	int32_t V_2 = 0;
	{
		// int hash = 17;
		V_0 = (int32_t)((int32_t)17);
		// hash = hash * 23 + overrideState.GetHashCode();
		int32_t L_0 = V_0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_1 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		V_1 = (bool)L_1;
		int32_t L_2 = Boolean_GetHashCode_m03AF8B3CECAE9106C44A00E3B33E51CBFC45C411((bool*)(bool*)(&V_1), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)((int32_t)23))), (int32_t)L_2));
		// if (!EqualityComparer<T>.Default.Equals(value, default)) // Catches null for references with boxing of value types
		EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62 * L_3 = ((  EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		NullCheck((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)__this);
		int32_t L_4 = VirtFuncInvoker0< int32_t >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Int32>::get_value() */, (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)__this);
		il2cpp_codegen_initobj((&V_2), sizeof(int32_t));
		int32_t L_5 = V_2;
		NullCheck((EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62 *)L_3);
		bool L_6 = VirtFuncInvoker2< bool, int32_t, int32_t >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<System.Int32>::Equals(!0,!0) */, (EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62 *)L_3, (int32_t)L_4, (int32_t)L_5);
		if (L_6)
		{
			goto IL_004c;
		}
	}
	{
		// hash = hash * 23 + value.GetHashCode();
		int32_t L_7 = V_0;
		NullCheck((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)__this);
		int32_t L_8 = VirtFuncInvoker0< int32_t >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Int32>::get_value() */, (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)__this);
		V_2 = (int32_t)L_8;
		int32_t L_9 = Int32_GetHashCode_mEDD3F492A5F7CF021125AE3F38E2B8F8743FC667((int32_t*)(int32_t*)(&V_2), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_7, (int32_t)((int32_t)23))), (int32_t)L_9));
	}

IL_004c:
	{
		// return hash;
		int32_t L_10 = V_0;
		return L_10;
	}
}
// System.String UnityEngine.Rendering.VolumeParameter`1<System.Int32>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* VolumeParameter_1_ToString_m846B3E448C84AEA3E560FF6ECCE12A0EE8E98862_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_ToString_m846B3E448C84AEA3E560FF6ECCE12A0EE8E98862_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public override string ToString() => $"{value} ({overrideState})";
		NullCheck((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)__this);
		int32_t L_0 = VirtFuncInvoker0< int32_t >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Int32>::get_value() */, (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)__this);
		int32_t L_1 = L_0;
		RuntimeObject * L_2 = Box(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7), &L_1);
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_3 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_4 = L_3;
		RuntimeObject * L_5 = Box(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_il2cpp_TypeInfo_var, &L_4);
		String_t* L_6 = String_Format_m8D1CB0410C35E052A53AE957C914C841E54BAB66((String_t*)_stringLiteralA5E215A6DBE803E908043576B18C4FAD26AD44F7, (RuntimeObject *)L_2, (RuntimeObject *)L_5, /*hidden argument*/NULL);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Int32>::op_Equality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Equality_mA1D747A92AB1547FDBA73CC4A8DA0F5A54C90C8A_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * ___lhs0, int32_t ___rhs1, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		// public static bool operator==(VolumeParameter<T> lhs, T rhs) => lhs != null && !ReferenceEquals(lhs.value, null) && lhs.value.Equals(rhs);
		VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * L_0 = ___lhs0;
		if (!L_0)
		{
			goto IL_002b;
		}
	}
	{
		VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * L_1 = ___lhs0;
		NullCheck((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)L_1);
		int32_t L_2 = VirtFuncInvoker0< int32_t >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Int32>::get_value() */, (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)L_1);
	}
	{
		VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * L_3 = ___lhs0;
		NullCheck((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)L_3);
		int32_t L_4 = VirtFuncInvoker0< int32_t >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Int32>::get_value() */, (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)L_3);
		V_0 = (int32_t)L_4;
		int32_t L_5 = ___rhs1;
		int32_t L_6 = L_5;
		RuntimeObject * L_7 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 7), &L_6);
		bool L_8 = Int32_Equals_m5F032BFC65C340C98050D3DF2D76101914774464((int32_t*)(int32_t*)(&V_0), (RuntimeObject *)L_7, /*hidden argument*/NULL);
		return L_8;
	}

IL_002b:
	{
		return (bool)0;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Int32>::op_Inequality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Inequality_m54FD44CB3256C974105FFC63C722E797C0AA981B_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * ___lhs0, int32_t ___rhs1, const RuntimeMethod* method)
{
	{
		// public static bool operator!=(VolumeParameter<T> lhs, T rhs) => !(lhs == rhs);
		VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * L_0 = ___lhs0;
		int32_t L_1 = ___rhs1;
		bool L_2 = ((  bool (*) (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)L_0, (int32_t)L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		return (bool)((((int32_t)L_2) == ((int32_t)0))? 1 : 0);
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Int32>::Equals(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m94BF0EB0F60D7AFB4645C5B4D2D04C1EE497E060_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * __this, VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * ___other0, const RuntimeMethod* method)
{
	{
		// if (ReferenceEquals(null, other))
		VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * L_0 = ___other0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, other))
		VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * L_1 = ___other0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)__this) == ((RuntimeObject*)(VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// return EqualityComparer<T>.Default.Equals(m_Value, other.m_Value);
		EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62 * L_2 = ((  EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		int32_t L_3 = (int32_t)__this->get_m_Value_2();
		VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * L_4 = ___other0;
		NullCheck(L_4);
		int32_t L_5 = (int32_t)L_4->get_m_Value_2();
		NullCheck((EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62 *)L_2);
		bool L_6 = VirtFuncInvoker2< bool, int32_t, int32_t >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<System.Int32>::Equals(!0,!0) */, (EqualityComparer_1_t20B8E5927E151143D1CBD8554CAF17F0EAC1CF62 *)L_2, (int32_t)L_3, (int32_t)L_5);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Int32>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m9016DE26A9F96EDE9E8AC043EE8D06AFF4B8486B_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_Equals_m9016DE26A9F96EDE9E8AC043EE8D06AFF4B8486B_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (ReferenceEquals(null, obj))
		RuntimeObject * L_0 = ___obj0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, obj))
		RuntimeObject * L_1 = ___obj0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)__this) == ((RuntimeObject*)(RuntimeObject *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// if (obj.GetType() != GetType())
		RuntimeObject * L_2 = ___obj0;
		NullCheck((RuntimeObject *)L_2);
		Type_t * L_3 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)L_2, /*hidden argument*/NULL);
		NullCheck((RuntimeObject *)__this);
		Type_t * L_4 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		bool L_5 = Type_op_Inequality_m6DDC5E923203A79BF505F9275B694AD3FAA36DB0((Type_t *)L_3, (Type_t *)L_4, /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_0020;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0020:
	{
		// return Equals((VolumeParameter<T>)obj);
		RuntimeObject * L_6 = ___obj0;
		NullCheck((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)__this);
		bool L_7 = ((  bool (*) (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *, VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)__this, (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)((VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 *)Castclass((RuntimeObject*)L_6, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10))), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_7;
	}
}
// T UnityEngine.Rendering.VolumeParameter`1<System.Int32>::op_Explicit(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t VolumeParameter_1_op_Explicit_mD323D3CEA3969828E3790F1573F2669DED5115F1_gshared (VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * ___prop0, const RuntimeMethod* method)
{
	{
		// public static explicit operator T(VolumeParameter<T> prop) => prop.m_Value;
		VolumeParameter_1_tDB3E73C28039660FD6C204191A4FDFA8EFBEC0F7 * L_0 = ___prop0;
		NullCheck(L_0);
		int32_t L_1 = (int32_t)L_0->get_m_Value_2();
		return L_1;
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
// T UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::get_value()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t VolumeParameter_1_get_value_mBBEE644D6FE116B4480B3FA4DEC36A30F6D39530_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * __this, const RuntimeMethod* method)
{
	{
		// get => m_Value;
		int32_t L_0 = (int32_t)__this->get_m_Value_2();
		return L_0;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::set_value(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_set_value_mAB06E348BE0D1E510C90410DD7C20E322B52EB90_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		// set => m_Value = value;
		int32_t L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_m93EAEE80ABDCAAEC8F2D2C9D9D1CC78328F95DC7_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		// : this(default, false)
		il2cpp_codegen_initobj((&V_0), sizeof(int32_t));
		int32_t L_0 = V_0;
		NullCheck((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)__this);
		((  void (*) (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *, int32_t, bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)__this, (int32_t)L_0, (bool)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::.ctor(T,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_m45867A2D95160443BF8C3F9066B6054EC521347E_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * __this, int32_t ___value0, bool ___overrideState1, const RuntimeMethod* method)
{
	{
		// protected VolumeParameter(T value, bool overrideState)
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VolumeParameter__ctor_m0F5E882EFAE8F1B7AE0DFF62945878D71CA63D54((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, /*hidden argument*/NULL);
		// m_Value = value;
		int32_t L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		// this.overrideState = overrideState;
		bool L_1 = ___overrideState1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)L_1);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::Interp(UnityEngine.Rendering.VolumeParameter,UnityEngine.Rendering.VolumeParameter,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_mBECEAE59B4B7BB4F419D0CAEE3255F1CACAFE6F1_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___from0, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___to1, float ___t2, const RuntimeMethod* method)
{
	{
		// Interp(from.GetValue<T>(), to.GetValue<T>(), t);
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___from0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		int32_t L_1 = ((  int32_t (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_2 = ___to1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2);
		int32_t L_3 = ((  int32_t (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		float L_4 = ___t2;
		NullCheck((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)__this);
		VirtActionInvoker3< int32_t, int32_t, float >::Invoke(14 /* System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::Interp(T,T,System.Single) */, (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)__this, (int32_t)L_1, (int32_t)L_3, (float)L_4);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::Interp(T,T,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_m1BF7D74C87C7EF48A3261363FA0F88E0E92C1479_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * __this, int32_t ___from0, int32_t ___to1, float ___t2, const RuntimeMethod* method)
{
	VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * G_B2_0 = NULL;
	VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * G_B1_0 = NULL;
	int32_t G_B3_0 = 0;
	VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * G_B3_1 = NULL;
	{
		// m_Value = t > 0f ? to : from;
		float L_0 = ___t2;
		G_B1_0 = ((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)(__this));
		if ((((float)L_0) > ((float)(0.0f))))
		{
			G_B2_0 = ((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)(__this));
			goto IL_000c;
		}
	}
	{
		int32_t L_1 = ___from0;
		G_B3_0 = L_1;
		G_B3_1 = ((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)(G_B1_0));
		goto IL_000d;
	}

IL_000c:
	{
		int32_t L_2 = ___to1;
		G_B3_0 = L_2;
		G_B3_1 = ((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)(G_B2_0));
	}

IL_000d:
	{
		NullCheck(G_B3_1);
		G_B3_1->set_m_Value_2(G_B3_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::Override(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Override_mFB3816FDE0134AAE8CEE22D020FD81CE77F42555_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * __this, int32_t ___x0, const RuntimeMethod* method)
{
	{
		// overrideState = true;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)1);
		// m_Value = x;
		int32_t L_0 = ___x0;
		__this->set_m_Value_2(L_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::SetValue(UnityEngine.Rendering.VolumeParameter)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_SetValue_m55A93CFC951EF6CACE3A49DD503F4A422B0CD9FC_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___parameter0, const RuntimeMethod* method)
{
	{
		// m_Value = parameter.GetValue<T>();
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___parameter0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		int32_t L_1 = ((  int32_t (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		__this->set_m_Value_2(L_1);
		// }
		return;
	}
}
// System.Int32 UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t VolumeParameter_1_GetHashCode_mE61DD2B10018C04F9D9607E7B1EB61420441B59D_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	int32_t V_2 = 0;
	{
		// int hash = 17;
		V_0 = (int32_t)((int32_t)17);
		// hash = hash * 23 + overrideState.GetHashCode();
		int32_t L_0 = V_0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_1 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		V_1 = (bool)L_1;
		int32_t L_2 = Boolean_GetHashCode_m03AF8B3CECAE9106C44A00E3B33E51CBFC45C411((bool*)(bool*)(&V_1), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)((int32_t)23))), (int32_t)L_2));
		// if (!EqualityComparer<T>.Default.Equals(value, default)) // Catches null for references with boxing of value types
		EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F * L_3 = ((  EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		NullCheck((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)__this);
		int32_t L_4 = VirtFuncInvoker0< int32_t >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::get_value() */, (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)__this);
		il2cpp_codegen_initobj((&V_2), sizeof(int32_t));
		int32_t L_5 = V_2;
		NullCheck((EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F *)L_3);
		bool L_6 = VirtFuncInvoker2< bool, int32_t, int32_t >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<System.Int32Enum>::Equals(!0,!0) */, (EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F *)L_3, (int32_t)L_4, (int32_t)L_5);
		if (L_6)
		{
			goto IL_004c;
		}
	}
	{
		// hash = hash * 23 + value.GetHashCode();
		int32_t L_7 = V_0;
		NullCheck((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)__this);
		int32_t L_8 = VirtFuncInvoker0< int32_t >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::get_value() */, (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)__this);
		V_2 = (int32_t)L_8;
		Il2CppFakeBox<int32_t> L_9(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7), (&V_2));
		int32_t L_10 = VirtFuncInvoker0< int32_t >::Invoke(2 /* System.Int32 System.Object::GetHashCode() */, (RuntimeObject *)(&L_9));
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_7, (int32_t)((int32_t)23))), (int32_t)L_10));
	}

IL_004c:
	{
		// return hash;
		int32_t L_11 = V_0;
		return L_11;
	}
}
// System.String UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* VolumeParameter_1_ToString_mF6E839F3AA47EB989463C8B204124DC50009CFF6_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_ToString_mF6E839F3AA47EB989463C8B204124DC50009CFF6_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public override string ToString() => $"{value} ({overrideState})";
		NullCheck((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)__this);
		int32_t L_0 = VirtFuncInvoker0< int32_t >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::get_value() */, (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)__this);
		int32_t L_1 = L_0;
		RuntimeObject * L_2 = Box(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7), &L_1);
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_3 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_4 = L_3;
		RuntimeObject * L_5 = Box(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_il2cpp_TypeInfo_var, &L_4);
		String_t* L_6 = String_Format_m8D1CB0410C35E052A53AE957C914C841E54BAB66((String_t*)_stringLiteralA5E215A6DBE803E908043576B18C4FAD26AD44F7, (RuntimeObject *)L_2, (RuntimeObject *)L_5, /*hidden argument*/NULL);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::op_Equality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Equality_m9F41E819F35BDE212783F8CA11920B0E5C3D4360_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * ___lhs0, int32_t ___rhs1, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		// public static bool operator==(VolumeParameter<T> lhs, T rhs) => lhs != null && !ReferenceEquals(lhs.value, null) && lhs.value.Equals(rhs);
		VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * L_0 = ___lhs0;
		if (!L_0)
		{
			goto IL_002b;
		}
	}
	{
		VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * L_1 = ___lhs0;
		NullCheck((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)L_1);
		int32_t L_2 = VirtFuncInvoker0< int32_t >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::get_value() */, (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)L_1);
	}
	{
		VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * L_3 = ___lhs0;
		NullCheck((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)L_3);
		int32_t L_4 = VirtFuncInvoker0< int32_t >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::get_value() */, (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)L_3);
		V_0 = (int32_t)L_4;
		int32_t L_5 = ___rhs1;
		int32_t L_6 = L_5;
		RuntimeObject * L_7 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 7), &L_6);
		Il2CppFakeBox<int32_t> L_8(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 7), (&V_0));
		bool L_9 = VirtFuncInvoker1< bool, RuntimeObject * >::Invoke(0 /* System.Boolean System.Object::Equals(System.Object) */, (RuntimeObject *)(&L_8), (RuntimeObject *)L_7);
		return L_9;
	}

IL_002b:
	{
		return (bool)0;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::op_Inequality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Inequality_m36ECF4F69BF3934CFBB63A484B778018EA783A4A_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * ___lhs0, int32_t ___rhs1, const RuntimeMethod* method)
{
	{
		// public static bool operator!=(VolumeParameter<T> lhs, T rhs) => !(lhs == rhs);
		VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * L_0 = ___lhs0;
		int32_t L_1 = ___rhs1;
		bool L_2 = ((  bool (*) (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)L_0, (int32_t)L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		return (bool)((((int32_t)L_2) == ((int32_t)0))? 1 : 0);
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::Equals(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_mCBE4B8CED7945F603436FCC6ADDE8D6BE244DC05_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * __this, VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * ___other0, const RuntimeMethod* method)
{
	{
		// if (ReferenceEquals(null, other))
		VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * L_0 = ___other0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, other))
		VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * L_1 = ___other0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)__this) == ((RuntimeObject*)(VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// return EqualityComparer<T>.Default.Equals(m_Value, other.m_Value);
		EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F * L_2 = ((  EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		int32_t L_3 = (int32_t)__this->get_m_Value_2();
		VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * L_4 = ___other0;
		NullCheck(L_4);
		int32_t L_5 = (int32_t)L_4->get_m_Value_2();
		NullCheck((EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F *)L_2);
		bool L_6 = VirtFuncInvoker2< bool, int32_t, int32_t >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<System.Int32Enum>::Equals(!0,!0) */, (EqualityComparer_1_t399C4B066E24442E62E52C1FD1CCF501E96C846F *)L_2, (int32_t)L_3, (int32_t)L_5);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m7239C7067F96C0F6B9AE4886B308C3184819F259_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_Equals_m7239C7067F96C0F6B9AE4886B308C3184819F259_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (ReferenceEquals(null, obj))
		RuntimeObject * L_0 = ___obj0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, obj))
		RuntimeObject * L_1 = ___obj0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)__this) == ((RuntimeObject*)(RuntimeObject *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// if (obj.GetType() != GetType())
		RuntimeObject * L_2 = ___obj0;
		NullCheck((RuntimeObject *)L_2);
		Type_t * L_3 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)L_2, /*hidden argument*/NULL);
		NullCheck((RuntimeObject *)__this);
		Type_t * L_4 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		bool L_5 = Type_op_Inequality_m6DDC5E923203A79BF505F9275B694AD3FAA36DB0((Type_t *)L_3, (Type_t *)L_4, /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_0020;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0020:
	{
		// return Equals((VolumeParameter<T>)obj);
		RuntimeObject * L_6 = ___obj0;
		NullCheck((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)__this);
		bool L_7 = ((  bool (*) (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *, VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)__this, (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)((VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 *)Castclass((RuntimeObject*)L_6, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10))), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_7;
	}
}
// T UnityEngine.Rendering.VolumeParameter`1<System.Int32Enum>::op_Explicit(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t VolumeParameter_1_op_Explicit_m00A3511E767D9D38E3B9ED1A49724A5C78592846_gshared (VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * ___prop0, const RuntimeMethod* method)
{
	{
		// public static explicit operator T(VolumeParameter<T> prop) => prop.m_Value;
		VolumeParameter_1_t6E848A55C514718131AA63DC30EC61DAA73E5740 * L_0 = ___prop0;
		NullCheck(L_0);
		int32_t L_1 = (int32_t)L_0->get_m_Value_2();
		return L_1;
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
// T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::get_value()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  VolumeParameter_1_get_value_m1A452E24E79DC734501EAA829F0FD576ACDEAA18_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * __this, const RuntimeMethod* method)
{
	{
		// get => m_Value;
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_0 = (LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 )__this->get_m_Value_2();
		return L_0;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::set_value(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_set_value_m6132B1E21104A6BB873C3A033B9C33485A292BE4_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * __this, LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  ___value0, const RuntimeMethod* method)
{
	{
		// set => m_Value = value;
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_mF7C6E50844833D908540F7E60942799C7DE7CDCC_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * __this, const RuntimeMethod* method)
{
	LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// : this(default, false)
		il2cpp_codegen_initobj((&V_0), sizeof(LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 ));
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_0 = V_0;
		NullCheck((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)__this);
		((  void (*) (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *, LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 , bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)__this, (LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 )L_0, (bool)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::.ctor(T,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_mD2764A7D25C53808FC366C16D92ECDD6B3508A22_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * __this, LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  ___value0, bool ___overrideState1, const RuntimeMethod* method)
{
	{
		// protected VolumeParameter(T value, bool overrideState)
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VolumeParameter__ctor_m0F5E882EFAE8F1B7AE0DFF62945878D71CA63D54((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, /*hidden argument*/NULL);
		// m_Value = value;
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		// this.overrideState = overrideState;
		bool L_1 = ___overrideState1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)L_1);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::Interp(UnityEngine.Rendering.VolumeParameter,UnityEngine.Rendering.VolumeParameter,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_m5BA48D486215FBBF4E622067C43F7F4491B84DBA_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___from0, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___to1, float ___t2, const RuntimeMethod* method)
{
	{
		// Interp(from.GetValue<T>(), to.GetValue<T>(), t);
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___from0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_1 = ((  LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_2 = ___to1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2);
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_3 = ((  LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		float L_4 = ___t2;
		NullCheck((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)__this);
		VirtActionInvoker3< LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 , LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 , float >::Invoke(14 /* System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::Interp(T,T,System.Single) */, (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)__this, (LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 )L_1, (LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 )L_3, (float)L_4);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::Interp(T,T,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_mE80035334EFA9DA520008D15B252BE6081B1D5B2_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * __this, LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  ___from0, LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  ___to1, float ___t2, const RuntimeMethod* method)
{
	VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * G_B2_0 = NULL;
	VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * G_B1_0 = NULL;
	LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  G_B3_0;
	memset((&G_B3_0), 0, sizeof(G_B3_0));
	VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * G_B3_1 = NULL;
	{
		// m_Value = t > 0f ? to : from;
		float L_0 = ___t2;
		G_B1_0 = ((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)(__this));
		if ((((float)L_0) > ((float)(0.0f))))
		{
			G_B2_0 = ((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)(__this));
			goto IL_000c;
		}
	}
	{
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_1 = ___from0;
		G_B3_0 = L_1;
		G_B3_1 = ((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)(G_B1_0));
		goto IL_000d;
	}

IL_000c:
	{
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_2 = ___to1;
		G_B3_0 = L_2;
		G_B3_1 = ((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)(G_B2_0));
	}

IL_000d:
	{
		NullCheck(G_B3_1);
		G_B3_1->set_m_Value_2(G_B3_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::Override(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Override_m1BC994E02F281E8B5253CD48316D3845C10598C7_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * __this, LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  ___x0, const RuntimeMethod* method)
{
	{
		// overrideState = true;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)1);
		// m_Value = x;
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_0 = ___x0;
		__this->set_m_Value_2(L_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::SetValue(UnityEngine.Rendering.VolumeParameter)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_SetValue_mC58E8773973CB91697CC3118992182E1E674A7C6_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___parameter0, const RuntimeMethod* method)
{
	{
		// m_Value = parameter.GetValue<T>();
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___parameter0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_1 = ((  LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		__this->set_m_Value_2(L_1);
		// }
		return;
	}
}
// System.Int32 UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t VolumeParameter_1_GetHashCode_m3062D60F025EA3F2E8FB86C5D897A16444100F2F_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  V_2;
	memset((&V_2), 0, sizeof(V_2));
	{
		// int hash = 17;
		V_0 = (int32_t)((int32_t)17);
		// hash = hash * 23 + overrideState.GetHashCode();
		int32_t L_0 = V_0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_1 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		V_1 = (bool)L_1;
		int32_t L_2 = Boolean_GetHashCode_m03AF8B3CECAE9106C44A00E3B33E51CBFC45C411((bool*)(bool*)(&V_1), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)((int32_t)23))), (int32_t)L_2));
		// if (!EqualityComparer<T>.Default.Equals(value, default)) // Catches null for references with boxing of value types
		EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A * L_3 = ((  EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		NullCheck((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)__this);
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_4 = VirtFuncInvoker0< LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::get_value() */, (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)__this);
		il2cpp_codegen_initobj((&V_2), sizeof(LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 ));
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_5 = V_2;
		NullCheck((EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A *)L_3);
		bool L_6 = VirtFuncInvoker2< bool, LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 , LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<UnityEngine.LayerMask>::Equals(!0,!0) */, (EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A *)L_3, (LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 )L_4, (LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 )L_5);
		if (L_6)
		{
			goto IL_004c;
		}
	}
	{
		// hash = hash * 23 + value.GetHashCode();
		int32_t L_7 = V_0;
		NullCheck((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)__this);
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_8 = VirtFuncInvoker0< LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::get_value() */, (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)__this);
		V_2 = (LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 )L_8;
		RuntimeObject * L_9 = Box(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7), (&V_2));
		NullCheck((RuntimeObject *)L_9);
		int32_t L_10 = VirtFuncInvoker0< int32_t >::Invoke(2 /* System.Int32 System.Object::GetHashCode() */, (RuntimeObject *)L_9);
		V_2 = *(LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 *)UnBox(L_9);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_7, (int32_t)((int32_t)23))), (int32_t)L_10));
	}

IL_004c:
	{
		// return hash;
		int32_t L_11 = V_0;
		return L_11;
	}
}
// System.String UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* VolumeParameter_1_ToString_m5B3102A3866568EDE9F2D5CD5092E9D0E244C3DA_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_ToString_m5B3102A3866568EDE9F2D5CD5092E9D0E244C3DA_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public override string ToString() => $"{value} ({overrideState})";
		NullCheck((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)__this);
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_0 = VirtFuncInvoker0< LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::get_value() */, (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)__this);
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_1 = L_0;
		RuntimeObject * L_2 = Box(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7), &L_1);
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_3 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_4 = L_3;
		RuntimeObject * L_5 = Box(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_il2cpp_TypeInfo_var, &L_4);
		String_t* L_6 = String_Format_m8D1CB0410C35E052A53AE957C914C841E54BAB66((String_t*)_stringLiteralA5E215A6DBE803E908043576B18C4FAD26AD44F7, (RuntimeObject *)L_2, (RuntimeObject *)L_5, /*hidden argument*/NULL);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::op_Equality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Equality_m24E433D8B52F6E7D56EA7FCA324C4C9E06E4E0D9_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * ___lhs0, LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  ___rhs1, const RuntimeMethod* method)
{
	LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// public static bool operator==(VolumeParameter<T> lhs, T rhs) => lhs != null && !ReferenceEquals(lhs.value, null) && lhs.value.Equals(rhs);
		VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * L_0 = ___lhs0;
		if (!L_0)
		{
			goto IL_002b;
		}
	}
	{
		VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * L_1 = ___lhs0;
		NullCheck((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)L_1);
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_2 = VirtFuncInvoker0< LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::get_value() */, (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)L_1);
	}
	{
		VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * L_3 = ___lhs0;
		NullCheck((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)L_3);
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_4 = VirtFuncInvoker0< LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::get_value() */, (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)L_3);
		V_0 = (LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 )L_4;
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_5 = ___rhs1;
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_6 = L_5;
		RuntimeObject * L_7 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 7), &L_6);
		RuntimeObject * L_8 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 7), (&V_0));
		NullCheck((RuntimeObject *)L_8);
		bool L_9 = VirtFuncInvoker1< bool, RuntimeObject * >::Invoke(0 /* System.Boolean System.Object::Equals(System.Object) */, (RuntimeObject *)L_8, (RuntimeObject *)L_7);
		V_0 = *(LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 *)UnBox(L_8);
		return L_9;
	}

IL_002b:
	{
		return (bool)0;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::op_Inequality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Inequality_mC03455C636B44C2B43B3C5E283A7CBF27ABCE998_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * ___lhs0, LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  ___rhs1, const RuntimeMethod* method)
{
	{
		// public static bool operator!=(VolumeParameter<T> lhs, T rhs) => !(lhs == rhs);
		VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * L_0 = ___lhs0;
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_1 = ___rhs1;
		bool L_2 = ((  bool (*) (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *, LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)L_0, (LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 )L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		return (bool)((((int32_t)L_2) == ((int32_t)0))? 1 : 0);
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::Equals(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m1648E1646367FAD8B37DA22087A1E040F5F7CA55_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * __this, VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * ___other0, const RuntimeMethod* method)
{
	{
		// if (ReferenceEquals(null, other))
		VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * L_0 = ___other0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, other))
		VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * L_1 = ___other0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)__this) == ((RuntimeObject*)(VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// return EqualityComparer<T>.Default.Equals(m_Value, other.m_Value);
		EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A * L_2 = ((  EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_3 = (LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 )__this->get_m_Value_2();
		VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * L_4 = ___other0;
		NullCheck(L_4);
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_5 = (LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 )L_4->get_m_Value_2();
		NullCheck((EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A *)L_2);
		bool L_6 = VirtFuncInvoker2< bool, LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 , LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<UnityEngine.LayerMask>::Equals(!0,!0) */, (EqualityComparer_1_t63FECC5291EFC8F9179EDADD0B89F2115F87379A *)L_2, (LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 )L_3, (LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 )L_5);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m28EA57D1533109CDEFFC5AABE734AE6C14FA0851_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_Equals_m28EA57D1533109CDEFFC5AABE734AE6C14FA0851_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (ReferenceEquals(null, obj))
		RuntimeObject * L_0 = ___obj0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, obj))
		RuntimeObject * L_1 = ___obj0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)__this) == ((RuntimeObject*)(RuntimeObject *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// if (obj.GetType() != GetType())
		RuntimeObject * L_2 = ___obj0;
		NullCheck((RuntimeObject *)L_2);
		Type_t * L_3 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)L_2, /*hidden argument*/NULL);
		NullCheck((RuntimeObject *)__this);
		Type_t * L_4 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		bool L_5 = Type_op_Inequality_m6DDC5E923203A79BF505F9275B694AD3FAA36DB0((Type_t *)L_3, (Type_t *)L_4, /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_0020;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0020:
	{
		// return Equals((VolumeParameter<T>)obj);
		RuntimeObject * L_6 = ___obj0;
		NullCheck((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)__this);
		bool L_7 = ((  bool (*) (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *, VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)__this, (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)((VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D *)Castclass((RuntimeObject*)L_6, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10))), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_7;
	}
}
// T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.LayerMask>::op_Explicit(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  VolumeParameter_1_op_Explicit_m83A9AA0ECFE958172E2A9D6987CFF16A609C0039_gshared (VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * ___prop0, const RuntimeMethod* method)
{
	{
		// public static explicit operator T(VolumeParameter<T> prop) => prop.m_Value;
		VolumeParameter_1_t410FC9C75201D3D960D598DA98C3FC8765A54F5D * L_0 = ___prop0;
		NullCheck(L_0);
		LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8  L_1 = (LayerMask_t5FA647D8C300EA0621360CA4424717C3C73190A8 )L_0->get_m_Value_2();
		return L_1;
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
// T UnityEngine.Rendering.VolumeParameter`1<System.Single>::get_value()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float VolumeParameter_1_get_value_mE65BB62B1CDB9AC51F677613A6AC89579269FD6B_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * __this, const RuntimeMethod* method)
{
	{
		// get => m_Value;
		float L_0 = (float)__this->get_m_Value_2();
		return L_0;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Single>::set_value(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_set_value_mE9E76837EC3E3273CB3DAF7849C80B3F1929D51C_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * __this, float ___value0, const RuntimeMethod* method)
{
	{
		// set => m_Value = value;
		float L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Single>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_mBEF459F8B4ADBA28FF6DB59591F77496245A2F89_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * __this, const RuntimeMethod* method)
{
	float V_0 = 0.0f;
	{
		// : this(default, false)
		il2cpp_codegen_initobj((&V_0), sizeof(float));
		float L_0 = V_0;
		NullCheck((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)__this);
		((  void (*) (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *, float, bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)__this, (float)L_0, (bool)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Single>::.ctor(T,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_m9388D8BBE0345E067BE973DBC40EF667B21035AC_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * __this, float ___value0, bool ___overrideState1, const RuntimeMethod* method)
{
	{
		// protected VolumeParameter(T value, bool overrideState)
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VolumeParameter__ctor_m0F5E882EFAE8F1B7AE0DFF62945878D71CA63D54((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, /*hidden argument*/NULL);
		// m_Value = value;
		float L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		// this.overrideState = overrideState;
		bool L_1 = ___overrideState1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)L_1);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Single>::Interp(UnityEngine.Rendering.VolumeParameter,UnityEngine.Rendering.VolumeParameter,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_m74F6A9A7D450447087107C3ABF028E88B27970F4_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___from0, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___to1, float ___t2, const RuntimeMethod* method)
{
	{
		// Interp(from.GetValue<T>(), to.GetValue<T>(), t);
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___from0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		float L_1 = ((  float (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_2 = ___to1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2);
		float L_3 = ((  float (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		float L_4 = ___t2;
		NullCheck((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)__this);
		VirtActionInvoker3< float, float, float >::Invoke(14 /* System.Void UnityEngine.Rendering.VolumeParameter`1<System.Single>::Interp(T,T,System.Single) */, (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)__this, (float)L_1, (float)L_3, (float)L_4);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Single>::Interp(T,T,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_mE3BDAA88E4F5097F27079893DB8C21E4CE3B019D_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * __this, float ___from0, float ___to1, float ___t2, const RuntimeMethod* method)
{
	VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * G_B2_0 = NULL;
	VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * G_B1_0 = NULL;
	float G_B3_0 = 0.0f;
	VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * G_B3_1 = NULL;
	{
		// m_Value = t > 0f ? to : from;
		float L_0 = ___t2;
		G_B1_0 = ((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)(__this));
		if ((((float)L_0) > ((float)(0.0f))))
		{
			G_B2_0 = ((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)(__this));
			goto IL_000c;
		}
	}
	{
		float L_1 = ___from0;
		G_B3_0 = L_1;
		G_B3_1 = ((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)(G_B1_0));
		goto IL_000d;
	}

IL_000c:
	{
		float L_2 = ___to1;
		G_B3_0 = L_2;
		G_B3_1 = ((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)(G_B2_0));
	}

IL_000d:
	{
		NullCheck(G_B3_1);
		G_B3_1->set_m_Value_2(G_B3_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Single>::Override(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Override_m2C542C9C6D00DE2C56B40529B66F5955032AD2C3_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * __this, float ___x0, const RuntimeMethod* method)
{
	{
		// overrideState = true;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)1);
		// m_Value = x;
		float L_0 = ___x0;
		__this->set_m_Value_2(L_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Single>::SetValue(UnityEngine.Rendering.VolumeParameter)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_SetValue_m45C1D4E723B126F1C19A5FB267143E471B71F020_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___parameter0, const RuntimeMethod* method)
{
	{
		// m_Value = parameter.GetValue<T>();
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___parameter0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		float L_1 = ((  float (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		__this->set_m_Value_2(L_1);
		// }
		return;
	}
}
// System.Int32 UnityEngine.Rendering.VolumeParameter`1<System.Single>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t VolumeParameter_1_GetHashCode_m20049225D5D48AC8488B575D0C55A4D8C77E22C5_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	float V_2 = 0.0f;
	{
		// int hash = 17;
		V_0 = (int32_t)((int32_t)17);
		// hash = hash * 23 + overrideState.GetHashCode();
		int32_t L_0 = V_0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_1 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		V_1 = (bool)L_1;
		int32_t L_2 = Boolean_GetHashCode_m03AF8B3CECAE9106C44A00E3B33E51CBFC45C411((bool*)(bool*)(&V_1), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)((int32_t)23))), (int32_t)L_2));
		// if (!EqualityComparer<T>.Default.Equals(value, default)) // Catches null for references with boxing of value types
		EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F * L_3 = ((  EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		NullCheck((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)__this);
		float L_4 = VirtFuncInvoker0< float >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Single>::get_value() */, (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)__this);
		il2cpp_codegen_initobj((&V_2), sizeof(float));
		float L_5 = V_2;
		NullCheck((EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F *)L_3);
		bool L_6 = VirtFuncInvoker2< bool, float, float >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<System.Single>::Equals(!0,!0) */, (EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F *)L_3, (float)L_4, (float)L_5);
		if (L_6)
		{
			goto IL_004c;
		}
	}
	{
		// hash = hash * 23 + value.GetHashCode();
		int32_t L_7 = V_0;
		NullCheck((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)__this);
		float L_8 = VirtFuncInvoker0< float >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Single>::get_value() */, (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)__this);
		V_2 = (float)L_8;
		int32_t L_9 = Single_GetHashCode_m7662E1812DDDBC85D464398740CFFC3588DFB2C9((float*)(float*)(&V_2), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_7, (int32_t)((int32_t)23))), (int32_t)L_9));
	}

IL_004c:
	{
		// return hash;
		int32_t L_10 = V_0;
		return L_10;
	}
}
// System.String UnityEngine.Rendering.VolumeParameter`1<System.Single>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* VolumeParameter_1_ToString_mF22D6B5BA00E0341023E67CE6DE6F2CEF402846D_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_ToString_mF22D6B5BA00E0341023E67CE6DE6F2CEF402846D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public override string ToString() => $"{value} ({overrideState})";
		NullCheck((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)__this);
		float L_0 = VirtFuncInvoker0< float >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Single>::get_value() */, (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)__this);
		float L_1 = L_0;
		RuntimeObject * L_2 = Box(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7), &L_1);
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_3 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_4 = L_3;
		RuntimeObject * L_5 = Box(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_il2cpp_TypeInfo_var, &L_4);
		String_t* L_6 = String_Format_m8D1CB0410C35E052A53AE957C914C841E54BAB66((String_t*)_stringLiteralA5E215A6DBE803E908043576B18C4FAD26AD44F7, (RuntimeObject *)L_2, (RuntimeObject *)L_5, /*hidden argument*/NULL);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Single>::op_Equality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Equality_mE98D231AFBB47C57BF307778505C205CD254A4D3_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * ___lhs0, float ___rhs1, const RuntimeMethod* method)
{
	float V_0 = 0.0f;
	{
		// public static bool operator==(VolumeParameter<T> lhs, T rhs) => lhs != null && !ReferenceEquals(lhs.value, null) && lhs.value.Equals(rhs);
		VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * L_0 = ___lhs0;
		if (!L_0)
		{
			goto IL_002b;
		}
	}
	{
		VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * L_1 = ___lhs0;
		NullCheck((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)L_1);
		float L_2 = VirtFuncInvoker0< float >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Single>::get_value() */, (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)L_1);
	}
	{
		VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * L_3 = ___lhs0;
		NullCheck((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)L_3);
		float L_4 = VirtFuncInvoker0< float >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Single>::get_value() */, (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)L_3);
		V_0 = (float)L_4;
		float L_5 = ___rhs1;
		float L_6 = L_5;
		RuntimeObject * L_7 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 7), &L_6);
		bool L_8 = Single_Equals_m94AA41817D00A9347BD3565F6BB8993361B81EB1((float*)(float*)(&V_0), (RuntimeObject *)L_7, /*hidden argument*/NULL);
		return L_8;
	}

IL_002b:
	{
		return (bool)0;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Single>::op_Inequality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Inequality_m97786AB4554649425964FD50184021748017459E_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * ___lhs0, float ___rhs1, const RuntimeMethod* method)
{
	{
		// public static bool operator!=(VolumeParameter<T> lhs, T rhs) => !(lhs == rhs);
		VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * L_0 = ___lhs0;
		float L_1 = ___rhs1;
		bool L_2 = ((  bool (*) (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)L_0, (float)L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		return (bool)((((int32_t)L_2) == ((int32_t)0))? 1 : 0);
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Single>::Equals(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m83BCF88BE9EF7A5DEF3FF8209A9AA068874BAE25_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * __this, VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * ___other0, const RuntimeMethod* method)
{
	{
		// if (ReferenceEquals(null, other))
		VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * L_0 = ___other0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, other))
		VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * L_1 = ___other0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)__this) == ((RuntimeObject*)(VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// return EqualityComparer<T>.Default.Equals(m_Value, other.m_Value);
		EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F * L_2 = ((  EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		float L_3 = (float)__this->get_m_Value_2();
		VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * L_4 = ___other0;
		NullCheck(L_4);
		float L_5 = (float)L_4->get_m_Value_2();
		NullCheck((EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F *)L_2);
		bool L_6 = VirtFuncInvoker2< bool, float, float >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<System.Single>::Equals(!0,!0) */, (EqualityComparer_1_t6C59536EBB4DD1217C6DBCECEC22F9F4202F710F *)L_2, (float)L_3, (float)L_5);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Single>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m8FCF4CD51C849EC469EDDDEA30F81B3C0CC0A69F_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_Equals_m8FCF4CD51C849EC469EDDDEA30F81B3C0CC0A69F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (ReferenceEquals(null, obj))
		RuntimeObject * L_0 = ___obj0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, obj))
		RuntimeObject * L_1 = ___obj0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)__this) == ((RuntimeObject*)(RuntimeObject *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// if (obj.GetType() != GetType())
		RuntimeObject * L_2 = ___obj0;
		NullCheck((RuntimeObject *)L_2);
		Type_t * L_3 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)L_2, /*hidden argument*/NULL);
		NullCheck((RuntimeObject *)__this);
		Type_t * L_4 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		bool L_5 = Type_op_Inequality_m6DDC5E923203A79BF505F9275B694AD3FAA36DB0((Type_t *)L_3, (Type_t *)L_4, /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_0020;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0020:
	{
		// return Equals((VolumeParameter<T>)obj);
		RuntimeObject * L_6 = ___obj0;
		NullCheck((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)__this);
		bool L_7 = ((  bool (*) (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *, VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)__this, (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)((VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 *)Castclass((RuntimeObject*)L_6, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10))), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_7;
	}
}
// T UnityEngine.Rendering.VolumeParameter`1<System.Single>::op_Explicit(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float VolumeParameter_1_op_Explicit_m488724D949156654B906ED61056D6E8197C020A2_gshared (VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * ___prop0, const RuntimeMethod* method)
{
	{
		// public static explicit operator T(VolumeParameter<T> prop) => prop.m_Value;
		VolumeParameter_1_tEAD8BA2E1993A0E02F8FDB436B5AB2F66E1036D7 * L_0 = ___prop0;
		NullCheck(L_0);
		float L_1 = (float)L_0->get_m_Value_2();
		return L_1;
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
// T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::get_value()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  VolumeParameter_1_get_value_m7DD26F8D47DDB2B92ED4DC40B30E31D9FE0ED3D5_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * __this, const RuntimeMethod* method)
{
	{
		// get => m_Value;
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_0 = (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 )__this->get_m_Value_2();
		return L_0;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::set_value(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_set_value_m713DB2249619116159C9470F79ADC60615FDCA32_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * __this, Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___value0, const RuntimeMethod* method)
{
	{
		// set => m_Value = value;
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_mE4A4AAE761BF96FF1AF03A491B449B3933A01F7B_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * __this, const RuntimeMethod* method)
{
	Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// : this(default, false)
		il2cpp_codegen_initobj((&V_0), sizeof(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 ));
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_0 = V_0;
		NullCheck((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)__this);
		((  void (*) (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *, Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 , bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)__this, (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 )L_0, (bool)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::.ctor(T,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_mB52103D1F81AA55DA4B112171931EA0CE7CC635E_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * __this, Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___value0, bool ___overrideState1, const RuntimeMethod* method)
{
	{
		// protected VolumeParameter(T value, bool overrideState)
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VolumeParameter__ctor_m0F5E882EFAE8F1B7AE0DFF62945878D71CA63D54((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, /*hidden argument*/NULL);
		// m_Value = value;
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		// this.overrideState = overrideState;
		bool L_1 = ___overrideState1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)L_1);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::Interp(UnityEngine.Rendering.VolumeParameter,UnityEngine.Rendering.VolumeParameter,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_mDF4558E944EAA8F36B5AAA49113A6B8316033D93_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___from0, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___to1, float ___t2, const RuntimeMethod* method)
{
	{
		// Interp(from.GetValue<T>(), to.GetValue<T>(), t);
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___from0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_1 = ((  Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_2 = ___to1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2);
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_3 = ((  Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		float L_4 = ___t2;
		NullCheck((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)__this);
		VirtActionInvoker3< Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 , Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 , float >::Invoke(14 /* System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::Interp(T,T,System.Single) */, (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)__this, (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 )L_1, (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 )L_3, (float)L_4);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::Interp(T,T,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_mB30FF2C03DE380C289E36F4DD6F92E4F0C306A14_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * __this, Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___from0, Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___to1, float ___t2, const RuntimeMethod* method)
{
	VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * G_B2_0 = NULL;
	VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * G_B1_0 = NULL;
	Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  G_B3_0;
	memset((&G_B3_0), 0, sizeof(G_B3_0));
	VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * G_B3_1 = NULL;
	{
		// m_Value = t > 0f ? to : from;
		float L_0 = ___t2;
		G_B1_0 = ((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)(__this));
		if ((((float)L_0) > ((float)(0.0f))))
		{
			G_B2_0 = ((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)(__this));
			goto IL_000c;
		}
	}
	{
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_1 = ___from0;
		G_B3_0 = L_1;
		G_B3_1 = ((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)(G_B1_0));
		goto IL_000d;
	}

IL_000c:
	{
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_2 = ___to1;
		G_B3_0 = L_2;
		G_B3_1 = ((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)(G_B2_0));
	}

IL_000d:
	{
		NullCheck(G_B3_1);
		G_B3_1->set_m_Value_2(G_B3_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::Override(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Override_m8AF26DC0CE8D1D57985EBFCCE314E3EC57EA6D1A_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * __this, Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___x0, const RuntimeMethod* method)
{
	{
		// overrideState = true;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)1);
		// m_Value = x;
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_0 = ___x0;
		__this->set_m_Value_2(L_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::SetValue(UnityEngine.Rendering.VolumeParameter)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_SetValue_mBB5BEC3ECF0A51BE89A05B6C1D78C63453A5E4FF_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___parameter0, const RuntimeMethod* method)
{
	{
		// m_Value = parameter.GetValue<T>();
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___parameter0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_1 = ((  Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		__this->set_m_Value_2(L_1);
		// }
		return;
	}
}
// System.Int32 UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t VolumeParameter_1_GetHashCode_mCF169E9F80762BF8A1E42C54B1FA2C0D6AA5C4F7_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  V_2;
	memset((&V_2), 0, sizeof(V_2));
	{
		// int hash = 17;
		V_0 = (int32_t)((int32_t)17);
		// hash = hash * 23 + overrideState.GetHashCode();
		int32_t L_0 = V_0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_1 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		V_1 = (bool)L_1;
		int32_t L_2 = Boolean_GetHashCode_m03AF8B3CECAE9106C44A00E3B33E51CBFC45C411((bool*)(bool*)(&V_1), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)((int32_t)23))), (int32_t)L_2));
		// if (!EqualityComparer<T>.Default.Equals(value, default)) // Catches null for references with boxing of value types
		EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09 * L_3 = ((  EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		NullCheck((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)__this);
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_4 = VirtFuncInvoker0< Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::get_value() */, (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)__this);
		il2cpp_codegen_initobj((&V_2), sizeof(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 ));
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_5 = V_2;
		NullCheck((EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09 *)L_3);
		bool L_6 = VirtFuncInvoker2< bool, Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 , Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector2>::Equals(!0,!0) */, (EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09 *)L_3, (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 )L_4, (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 )L_5);
		if (L_6)
		{
			goto IL_004c;
		}
	}
	{
		// hash = hash * 23 + value.GetHashCode();
		int32_t L_7 = V_0;
		NullCheck((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)__this);
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_8 = VirtFuncInvoker0< Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::get_value() */, (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)__this);
		V_2 = (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 )L_8;
		int32_t L_9 = Vector2_GetHashCode_m9A5DD8406289F38806CC42C394E324C1C2AB3732((Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 *)(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 *)(&V_2), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_7, (int32_t)((int32_t)23))), (int32_t)L_9));
	}

IL_004c:
	{
		// return hash;
		int32_t L_10 = V_0;
		return L_10;
	}
}
// System.String UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* VolumeParameter_1_ToString_mE9E79D94F86F79CE0B3D50E47132499AF12ED674_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_ToString_mE9E79D94F86F79CE0B3D50E47132499AF12ED674_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public override string ToString() => $"{value} ({overrideState})";
		NullCheck((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)__this);
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_0 = VirtFuncInvoker0< Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::get_value() */, (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)__this);
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_1 = L_0;
		RuntimeObject * L_2 = Box(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7), &L_1);
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_3 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_4 = L_3;
		RuntimeObject * L_5 = Box(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_il2cpp_TypeInfo_var, &L_4);
		String_t* L_6 = String_Format_m8D1CB0410C35E052A53AE957C914C841E54BAB66((String_t*)_stringLiteralA5E215A6DBE803E908043576B18C4FAD26AD44F7, (RuntimeObject *)L_2, (RuntimeObject *)L_5, /*hidden argument*/NULL);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::op_Equality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Equality_m122686C553098A9D2A1E4D6CD4612C2B408D259C_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * ___lhs0, Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___rhs1, const RuntimeMethod* method)
{
	Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// public static bool operator==(VolumeParameter<T> lhs, T rhs) => lhs != null && !ReferenceEquals(lhs.value, null) && lhs.value.Equals(rhs);
		VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * L_0 = ___lhs0;
		if (!L_0)
		{
			goto IL_002b;
		}
	}
	{
		VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * L_1 = ___lhs0;
		NullCheck((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)L_1);
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_2 = VirtFuncInvoker0< Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::get_value() */, (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)L_1);
	}
	{
		VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * L_3 = ___lhs0;
		NullCheck((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)L_3);
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_4 = VirtFuncInvoker0< Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::get_value() */, (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)L_3);
		V_0 = (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 )L_4;
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_5 = ___rhs1;
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_6 = L_5;
		RuntimeObject * L_7 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 7), &L_6);
		bool L_8 = Vector2_Equals_m67A842D914AA5A4DCC076E9EA20019925E6A85A0((Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 *)(Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 *)(&V_0), (RuntimeObject *)L_7, /*hidden argument*/NULL);
		return L_8;
	}

IL_002b:
	{
		return (bool)0;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::op_Inequality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Inequality_mF5351020BA475F25487B6424851368193DCBE27D_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * ___lhs0, Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  ___rhs1, const RuntimeMethod* method)
{
	{
		// public static bool operator!=(VolumeParameter<T> lhs, T rhs) => !(lhs == rhs);
		VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * L_0 = ___lhs0;
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_1 = ___rhs1;
		bool L_2 = ((  bool (*) (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *, Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)L_0, (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 )L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		return (bool)((((int32_t)L_2) == ((int32_t)0))? 1 : 0);
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::Equals(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m0DA25D290A4E860B65E3CCA55B7C08F02AA3DB41_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * __this, VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * ___other0, const RuntimeMethod* method)
{
	{
		// if (ReferenceEquals(null, other))
		VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * L_0 = ___other0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, other))
		VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * L_1 = ___other0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)__this) == ((RuntimeObject*)(VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// return EqualityComparer<T>.Default.Equals(m_Value, other.m_Value);
		EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09 * L_2 = ((  EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_3 = (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 )__this->get_m_Value_2();
		VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * L_4 = ___other0;
		NullCheck(L_4);
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_5 = (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 )L_4->get_m_Value_2();
		NullCheck((EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09 *)L_2);
		bool L_6 = VirtFuncInvoker2< bool, Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 , Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector2>::Equals(!0,!0) */, (EqualityComparer_1_t6779B568DF74AAD57393C6D2A62B852EC780CC09 *)L_2, (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 )L_3, (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 )L_5);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m280BFF61F817FEAC2CBBD65A638562916BF3F692_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_Equals_m280BFF61F817FEAC2CBBD65A638562916BF3F692_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (ReferenceEquals(null, obj))
		RuntimeObject * L_0 = ___obj0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, obj))
		RuntimeObject * L_1 = ___obj0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)__this) == ((RuntimeObject*)(RuntimeObject *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// if (obj.GetType() != GetType())
		RuntimeObject * L_2 = ___obj0;
		NullCheck((RuntimeObject *)L_2);
		Type_t * L_3 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)L_2, /*hidden argument*/NULL);
		NullCheck((RuntimeObject *)__this);
		Type_t * L_4 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		bool L_5 = Type_op_Inequality_m6DDC5E923203A79BF505F9275B694AD3FAA36DB0((Type_t *)L_3, (Type_t *)L_4, /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_0020;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0020:
	{
		// return Equals((VolumeParameter<T>)obj);
		RuntimeObject * L_6 = ___obj0;
		NullCheck((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)__this);
		bool L_7 = ((  bool (*) (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *, VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)__this, (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)((VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 *)Castclass((RuntimeObject*)L_6, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10))), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_7;
	}
}
// T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector2>::op_Explicit(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  VolumeParameter_1_op_Explicit_mA5DC14C41E311BC76D34FEEAC7AC29478EBC22D1_gshared (VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * ___prop0, const RuntimeMethod* method)
{
	{
		// public static explicit operator T(VolumeParameter<T> prop) => prop.m_Value;
		VolumeParameter_1_t54ABF950CC6A5A0584DBC3F81D8DDEF4D22270B7 * L_0 = ___prop0;
		NullCheck(L_0);
		Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9  L_1 = (Vector2_tBB32F2736AEC229A7BFBCE18197EC0F6AC7EC2D9 )L_0->get_m_Value_2();
		return L_1;
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
// T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::get_value()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  VolumeParameter_1_get_value_mF93101C5EDDA1B358CD201EB3C5E8FDF0B5A404A_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * __this, const RuntimeMethod* method)
{
	{
		// get => m_Value;
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_0 = (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )__this->get_m_Value_2();
		return L_0;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::set_value(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_set_value_m2F8D2E49FD6FBC3118DEC5F153A95085465BC59B_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * __this, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___value0, const RuntimeMethod* method)
{
	{
		// set => m_Value = value;
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_mE934FF123822927E73B5F780398087397E4017DA_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * __this, const RuntimeMethod* method)
{
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// : this(default, false)
		il2cpp_codegen_initobj((&V_0), sizeof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E ));
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_0 = V_0;
		NullCheck((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)__this);
		((  void (*) (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E , bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)__this, (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )L_0, (bool)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::.ctor(T,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_mD7068B78F5D082BFE2ABA8385B44A87897C659E1_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * __this, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___value0, bool ___overrideState1, const RuntimeMethod* method)
{
	{
		// protected VolumeParameter(T value, bool overrideState)
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VolumeParameter__ctor_m0F5E882EFAE8F1B7AE0DFF62945878D71CA63D54((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, /*hidden argument*/NULL);
		// m_Value = value;
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		// this.overrideState = overrideState;
		bool L_1 = ___overrideState1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)L_1);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::Interp(UnityEngine.Rendering.VolumeParameter,UnityEngine.Rendering.VolumeParameter,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_mAF7D9A651AA6C555402A0D188EB3AA72EC7022BA_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___from0, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___to1, float ___t2, const RuntimeMethod* method)
{
	{
		// Interp(from.GetValue<T>(), to.GetValue<T>(), t);
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___from0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_1 = ((  Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_2 = ___to1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_3 = ((  Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		float L_4 = ___t2;
		NullCheck((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)__this);
		VirtActionInvoker3< Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E , Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E , float >::Invoke(14 /* System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::Interp(T,T,System.Single) */, (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)__this, (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )L_1, (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )L_3, (float)L_4);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::Interp(T,T,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_m5273C2FF3EB0A07BB6C67BF60C1F3E63EBEFE081_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * __this, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___from0, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___to1, float ___t2, const RuntimeMethod* method)
{
	VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * G_B2_0 = NULL;
	VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * G_B1_0 = NULL;
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  G_B3_0;
	memset((&G_B3_0), 0, sizeof(G_B3_0));
	VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * G_B3_1 = NULL;
	{
		// m_Value = t > 0f ? to : from;
		float L_0 = ___t2;
		G_B1_0 = ((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)(__this));
		if ((((float)L_0) > ((float)(0.0f))))
		{
			G_B2_0 = ((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)(__this));
			goto IL_000c;
		}
	}
	{
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_1 = ___from0;
		G_B3_0 = L_1;
		G_B3_1 = ((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)(G_B1_0));
		goto IL_000d;
	}

IL_000c:
	{
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_2 = ___to1;
		G_B3_0 = L_2;
		G_B3_1 = ((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)(G_B2_0));
	}

IL_000d:
	{
		NullCheck(G_B3_1);
		G_B3_1->set_m_Value_2(G_B3_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::Override(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Override_m9C174EF8E9F5D3637072C0EAA8752389DCE83491_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * __this, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___x0, const RuntimeMethod* method)
{
	{
		// overrideState = true;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)1);
		// m_Value = x;
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_0 = ___x0;
		__this->set_m_Value_2(L_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::SetValue(UnityEngine.Rendering.VolumeParameter)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_SetValue_m0BBB417DE4415268AD2A66B4D0764982FE65C48D_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___parameter0, const RuntimeMethod* method)
{
	{
		// m_Value = parameter.GetValue<T>();
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___parameter0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_1 = ((  Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		__this->set_m_Value_2(L_1);
		// }
		return;
	}
}
// System.Int32 UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t VolumeParameter_1_GetHashCode_m713A112A8F84A7F27A4DB2933464D17BBE224E04_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  V_2;
	memset((&V_2), 0, sizeof(V_2));
	{
		// int hash = 17;
		V_0 = (int32_t)((int32_t)17);
		// hash = hash * 23 + overrideState.GetHashCode();
		int32_t L_0 = V_0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_1 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		V_1 = (bool)L_1;
		int32_t L_2 = Boolean_GetHashCode_m03AF8B3CECAE9106C44A00E3B33E51CBFC45C411((bool*)(bool*)(&V_1), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)((int32_t)23))), (int32_t)L_2));
		// if (!EqualityComparer<T>.Default.Equals(value, default)) // Catches null for references with boxing of value types
		EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7 * L_3 = ((  EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		NullCheck((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)__this);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_4 = VirtFuncInvoker0< Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::get_value() */, (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)__this);
		il2cpp_codegen_initobj((&V_2), sizeof(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E ));
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_5 = V_2;
		NullCheck((EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7 *)L_3);
		bool L_6 = VirtFuncInvoker2< bool, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E , Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector3>::Equals(!0,!0) */, (EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7 *)L_3, (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )L_4, (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )L_5);
		if (L_6)
		{
			goto IL_004c;
		}
	}
	{
		// hash = hash * 23 + value.GetHashCode();
		int32_t L_7 = V_0;
		NullCheck((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)__this);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_8 = VirtFuncInvoker0< Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::get_value() */, (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)__this);
		V_2 = (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )L_8;
		int32_t L_9 = Vector3_GetHashCode_m9F18401DA6025110A012F55BBB5ACABD36FA9A0A((Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E *)(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E *)(&V_2), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_7, (int32_t)((int32_t)23))), (int32_t)L_9));
	}

IL_004c:
	{
		// return hash;
		int32_t L_10 = V_0;
		return L_10;
	}
}
// System.String UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* VolumeParameter_1_ToString_m9C2B50A9346DD0AB59B08833586ECBCFB9519661_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_ToString_m9C2B50A9346DD0AB59B08833586ECBCFB9519661_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public override string ToString() => $"{value} ({overrideState})";
		NullCheck((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)__this);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_0 = VirtFuncInvoker0< Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::get_value() */, (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)__this);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_1 = L_0;
		RuntimeObject * L_2 = Box(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7), &L_1);
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_3 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_4 = L_3;
		RuntimeObject * L_5 = Box(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_il2cpp_TypeInfo_var, &L_4);
		String_t* L_6 = String_Format_m8D1CB0410C35E052A53AE957C914C841E54BAB66((String_t*)_stringLiteralA5E215A6DBE803E908043576B18C4FAD26AD44F7, (RuntimeObject *)L_2, (RuntimeObject *)L_5, /*hidden argument*/NULL);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::op_Equality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Equality_m59E7219C79A4ACD0AF0240D5981F052B0AB3706C_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * ___lhs0, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___rhs1, const RuntimeMethod* method)
{
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// public static bool operator==(VolumeParameter<T> lhs, T rhs) => lhs != null && !ReferenceEquals(lhs.value, null) && lhs.value.Equals(rhs);
		VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * L_0 = ___lhs0;
		if (!L_0)
		{
			goto IL_002b;
		}
	}
	{
		VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * L_1 = ___lhs0;
		NullCheck((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)L_1);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_2 = VirtFuncInvoker0< Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::get_value() */, (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)L_1);
	}
	{
		VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * L_3 = ___lhs0;
		NullCheck((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)L_3);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_4 = VirtFuncInvoker0< Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::get_value() */, (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)L_3);
		V_0 = (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )L_4;
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_5 = ___rhs1;
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_6 = L_5;
		RuntimeObject * L_7 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 7), &L_6);
		bool L_8 = Vector3_Equals_m210CB160B594355581D44D4B87CF3D3994ABFED0((Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E *)(Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E *)(&V_0), (RuntimeObject *)L_7, /*hidden argument*/NULL);
		return L_8;
	}

IL_002b:
	{
		return (bool)0;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::op_Inequality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Inequality_m9027012A345F02BCBD86BAFFE1E59488EC904A79_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * ___lhs0, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  ___rhs1, const RuntimeMethod* method)
{
	{
		// public static bool operator!=(VolumeParameter<T> lhs, T rhs) => !(lhs == rhs);
		VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * L_0 = ___lhs0;
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_1 = ___rhs1;
		bool L_2 = ((  bool (*) (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)L_0, (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		return (bool)((((int32_t)L_2) == ((int32_t)0))? 1 : 0);
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::Equals(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m510A1A4B4E767D2C44EBE02BB6DC85998A1B8E92_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * __this, VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * ___other0, const RuntimeMethod* method)
{
	{
		// if (ReferenceEquals(null, other))
		VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * L_0 = ___other0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, other))
		VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * L_1 = ___other0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)__this) == ((RuntimeObject*)(VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// return EqualityComparer<T>.Default.Equals(m_Value, other.m_Value);
		EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7 * L_2 = ((  EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_3 = (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )__this->get_m_Value_2();
		VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * L_4 = ___other0;
		NullCheck(L_4);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_5 = (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )L_4->get_m_Value_2();
		NullCheck((EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7 *)L_2);
		bool L_6 = VirtFuncInvoker2< bool, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E , Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector3>::Equals(!0,!0) */, (EqualityComparer_1_t3BB33804F138CAE0908623F6BFE2C7416362B9A7 *)L_2, (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )L_3, (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )L_5);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m8ED0747D259C00F3410EFFAEFD9E26465CF42B65_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_Equals_m8ED0747D259C00F3410EFFAEFD9E26465CF42B65_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (ReferenceEquals(null, obj))
		RuntimeObject * L_0 = ___obj0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, obj))
		RuntimeObject * L_1 = ___obj0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)__this) == ((RuntimeObject*)(RuntimeObject *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// if (obj.GetType() != GetType())
		RuntimeObject * L_2 = ___obj0;
		NullCheck((RuntimeObject *)L_2);
		Type_t * L_3 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)L_2, /*hidden argument*/NULL);
		NullCheck((RuntimeObject *)__this);
		Type_t * L_4 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		bool L_5 = Type_op_Inequality_m6DDC5E923203A79BF505F9275B694AD3FAA36DB0((Type_t *)L_3, (Type_t *)L_4, /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_0020;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0020:
	{
		// return Equals((VolumeParameter<T>)obj);
		RuntimeObject * L_6 = ___obj0;
		NullCheck((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)__this);
		bool L_7 = ((  bool (*) (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *, VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)__this, (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)((VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F *)Castclass((RuntimeObject*)L_6, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10))), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_7;
	}
}
// T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector3>::op_Explicit(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  VolumeParameter_1_op_Explicit_m9593E4E68C801602471B97A83E6B063EF3A73A92_gshared (VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * ___prop0, const RuntimeMethod* method)
{
	{
		// public static explicit operator T(VolumeParameter<T> prop) => prop.m_Value;
		VolumeParameter_1_t2E31F216E46FBD67713E740DF4AA855B19C4130F * L_0 = ___prop0;
		NullCheck(L_0);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_1 = (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )L_0->get_m_Value_2();
		return L_1;
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
// T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::get_value()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  VolumeParameter_1_get_value_m2EA653B325EEC0AEC56D6C83B8E5386F6E014BC5_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * __this, const RuntimeMethod* method)
{
	{
		// get => m_Value;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_0 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_m_Value_2();
		return L_0;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::set_value(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_set_value_m5D211AE58EC04A1C0C465D26D0BE128583E1521C_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * __this, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___value0, const RuntimeMethod* method)
{
	{
		// set => m_Value = value;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_mAB9A56F2C8AEDD0D99DAD5941AA305B3DD6E1C84_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * __this, const RuntimeMethod* method)
{
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// : this(default, false)
		il2cpp_codegen_initobj((&V_0), sizeof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 ));
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_0 = V_0;
		NullCheck((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)__this);
		((  void (*) (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)__this, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_0, (bool)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::.ctor(T,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_m459DEF559810C9A959598B2F88EEECF2843E0CF6_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * __this, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___value0, bool ___overrideState1, const RuntimeMethod* method)
{
	{
		// protected VolumeParameter(T value, bool overrideState)
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VolumeParameter__ctor_m0F5E882EFAE8F1B7AE0DFF62945878D71CA63D54((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, /*hidden argument*/NULL);
		// m_Value = value;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		// this.overrideState = overrideState;
		bool L_1 = ___overrideState1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)L_1);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::Interp(UnityEngine.Rendering.VolumeParameter,UnityEngine.Rendering.VolumeParameter,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_m050EE31E66335F6777B19DCD87F8770519A8A686_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___from0, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___to1, float ___t2, const RuntimeMethod* method)
{
	{
		// Interp(from.GetValue<T>(), to.GetValue<T>(), t);
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___from0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_1 = ((  Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_2 = ___to1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2);
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_3 = ((  Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		float L_4 = ___t2;
		NullCheck((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)__this);
		VirtActionInvoker3< Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , float >::Invoke(14 /* System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::Interp(T,T,System.Single) */, (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)__this, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_1, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_3, (float)L_4);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::Interp(T,T,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_m6F75A3B8404DB1DAE2349C5A57CA6ECF3CE5F556_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * __this, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___from0, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___to1, float ___t2, const RuntimeMethod* method)
{
	VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * G_B2_0 = NULL;
	VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * G_B1_0 = NULL;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  G_B3_0;
	memset((&G_B3_0), 0, sizeof(G_B3_0));
	VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * G_B3_1 = NULL;
	{
		// m_Value = t > 0f ? to : from;
		float L_0 = ___t2;
		G_B1_0 = ((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)(__this));
		if ((((float)L_0) > ((float)(0.0f))))
		{
			G_B2_0 = ((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)(__this));
			goto IL_000c;
		}
	}
	{
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_1 = ___from0;
		G_B3_0 = L_1;
		G_B3_1 = ((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)(G_B1_0));
		goto IL_000d;
	}

IL_000c:
	{
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_2 = ___to1;
		G_B3_0 = L_2;
		G_B3_1 = ((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)(G_B2_0));
	}

IL_000d:
	{
		NullCheck(G_B3_1);
		G_B3_1->set_m_Value_2(G_B3_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::Override(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Override_m7E10707423FD177F1152146C5F61105DF1F8D70C_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * __this, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___x0, const RuntimeMethod* method)
{
	{
		// overrideState = true;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)1);
		// m_Value = x;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_0 = ___x0;
		__this->set_m_Value_2(L_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::SetValue(UnityEngine.Rendering.VolumeParameter)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_SetValue_m82DDA24EB7110F139DC0012D85F6E3D52ABA9155_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___parameter0, const RuntimeMethod* method)
{
	{
		// m_Value = parameter.GetValue<T>();
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___parameter0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_1 = ((  Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		__this->set_m_Value_2(L_1);
		// }
		return;
	}
}
// System.Int32 UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t VolumeParameter_1_GetHashCode_m349692A683031E1772EBACF72A98E1B759B5F1AB_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  V_2;
	memset((&V_2), 0, sizeof(V_2));
	{
		// int hash = 17;
		V_0 = (int32_t)((int32_t)17);
		// hash = hash * 23 + overrideState.GetHashCode();
		int32_t L_0 = V_0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_1 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		V_1 = (bool)L_1;
		int32_t L_2 = Boolean_GetHashCode_m03AF8B3CECAE9106C44A00E3B33E51CBFC45C411((bool*)(bool*)(&V_1), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)((int32_t)23))), (int32_t)L_2));
		// if (!EqualityComparer<T>.Default.Equals(value, default)) // Catches null for references with boxing of value types
		EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E * L_3 = ((  EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		NullCheck((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)__this);
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_4 = VirtFuncInvoker0< Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::get_value() */, (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)__this);
		il2cpp_codegen_initobj((&V_2), sizeof(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 ));
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_5 = V_2;
		NullCheck((EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E *)L_3);
		bool L_6 = VirtFuncInvoker2< bool, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector4>::Equals(!0,!0) */, (EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E *)L_3, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_4, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_5);
		if (L_6)
		{
			goto IL_004c;
		}
	}
	{
		// hash = hash * 23 + value.GetHashCode();
		int32_t L_7 = V_0;
		NullCheck((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)__this);
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_8 = VirtFuncInvoker0< Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::get_value() */, (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)__this);
		V_2 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_8;
		int32_t L_9 = Vector4_GetHashCode_mCA7B312F8CA141F6F25BABDDF406F3D2BDD5E895((Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)(&V_2), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_7, (int32_t)((int32_t)23))), (int32_t)L_9));
	}

IL_004c:
	{
		// return hash;
		int32_t L_10 = V_0;
		return L_10;
	}
}
// System.String UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* VolumeParameter_1_ToString_m70BB531EC1A1197D6B56D5B16D3DAB51D2753CBB_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_ToString_m70BB531EC1A1197D6B56D5B16D3DAB51D2753CBB_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public override string ToString() => $"{value} ({overrideState})";
		NullCheck((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)__this);
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_0 = VirtFuncInvoker0< Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::get_value() */, (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)__this);
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_1 = L_0;
		RuntimeObject * L_2 = Box(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7), &L_1);
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_3 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_4 = L_3;
		RuntimeObject * L_5 = Box(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_il2cpp_TypeInfo_var, &L_4);
		String_t* L_6 = String_Format_m8D1CB0410C35E052A53AE957C914C841E54BAB66((String_t*)_stringLiteralA5E215A6DBE803E908043576B18C4FAD26AD44F7, (RuntimeObject *)L_2, (RuntimeObject *)L_5, /*hidden argument*/NULL);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::op_Equality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Equality_m69D10FDCEBA2FD496EA09620C635BD76CDB36FF0_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * ___lhs0, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___rhs1, const RuntimeMethod* method)
{
	Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// public static bool operator==(VolumeParameter<T> lhs, T rhs) => lhs != null && !ReferenceEquals(lhs.value, null) && lhs.value.Equals(rhs);
		VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * L_0 = ___lhs0;
		if (!L_0)
		{
			goto IL_002b;
		}
	}
	{
		VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * L_1 = ___lhs0;
		NullCheck((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)L_1);
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_2 = VirtFuncInvoker0< Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::get_value() */, (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)L_1);
	}
	{
		VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * L_3 = ___lhs0;
		NullCheck((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)L_3);
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_4 = VirtFuncInvoker0< Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::get_value() */, (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)L_3);
		V_0 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_4;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_5 = ___rhs1;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_6 = L_5;
		RuntimeObject * L_7 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 7), &L_6);
		bool L_8 = Vector4_Equals_m71D14F39651C3FBEDE17214455DFA727921F07AA((Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)(Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 *)(&V_0), (RuntimeObject *)L_7, /*hidden argument*/NULL);
		return L_8;
	}

IL_002b:
	{
		return (bool)0;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::op_Inequality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Inequality_m7FA56CEE1136929DA9697CCF037420643A2FA85B_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * ___lhs0, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  ___rhs1, const RuntimeMethod* method)
{
	{
		// public static bool operator!=(VolumeParameter<T> lhs, T rhs) => !(lhs == rhs);
		VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * L_0 = ___lhs0;
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_1 = ___rhs1;
		bool L_2 = ((  bool (*) (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)L_0, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		return (bool)((((int32_t)L_2) == ((int32_t)0))? 1 : 0);
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::Equals(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_mEB314431E51DD71B3F7BB22A786EE58C19218522_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * __this, VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * ___other0, const RuntimeMethod* method)
{
	{
		// if (ReferenceEquals(null, other))
		VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * L_0 = ___other0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, other))
		VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * L_1 = ___other0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)__this) == ((RuntimeObject*)(VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// return EqualityComparer<T>.Default.Equals(m_Value, other.m_Value);
		EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E * L_2 = ((  EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_3 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )__this->get_m_Value_2();
		VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * L_4 = ___other0;
		NullCheck(L_4);
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_5 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_4->get_m_Value_2();
		NullCheck((EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E *)L_2);
		bool L_6 = VirtFuncInvoker2< bool, Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 , Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<UnityEngine.Vector4>::Equals(!0,!0) */, (EqualityComparer_1_tF0279A3F5650C6035C7E9ABDE4237DCE38E8507E *)L_2, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_3, (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_5);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m831EC889B40C1B3D9763C7460C62A2C2986AFD5D_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_Equals_m831EC889B40C1B3D9763C7460C62A2C2986AFD5D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (ReferenceEquals(null, obj))
		RuntimeObject * L_0 = ___obj0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, obj))
		RuntimeObject * L_1 = ___obj0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)__this) == ((RuntimeObject*)(RuntimeObject *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// if (obj.GetType() != GetType())
		RuntimeObject * L_2 = ___obj0;
		NullCheck((RuntimeObject *)L_2);
		Type_t * L_3 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)L_2, /*hidden argument*/NULL);
		NullCheck((RuntimeObject *)__this);
		Type_t * L_4 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		bool L_5 = Type_op_Inequality_m6DDC5E923203A79BF505F9275B694AD3FAA36DB0((Type_t *)L_3, (Type_t *)L_4, /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_0020;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0020:
	{
		// return Equals((VolumeParameter<T>)obj);
		RuntimeObject * L_6 = ___obj0;
		NullCheck((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)__this);
		bool L_7 = ((  bool (*) (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *, VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)__this, (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)((VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C *)Castclass((RuntimeObject*)L_6, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10))), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_7;
	}
}
// T UnityEngine.Rendering.VolumeParameter`1<UnityEngine.Vector4>::op_Explicit(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  VolumeParameter_1_op_Explicit_m034A5A9009175E647D10A32FEB621BCA0237E93C_gshared (VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * ___prop0, const RuntimeMethod* method)
{
	{
		// public static explicit operator T(VolumeParameter<T> prop) => prop.m_Value;
		VolumeParameter_1_t3F7D5081F862B1E716B24AD243DB90F72E81139C * L_0 = ___prop0;
		NullCheck(L_0);
		Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7  L_1 = (Vector4_tA56A37FC5661BCC89C3DDC24BE12BA5BCB6A02C7 )L_0->get_m_Value_2();
		return L_1;
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
// T UnityEngine.Rendering.VolumeParameter`1<System.Object>::get_value()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * VolumeParameter_1_get_value_mE65E2B7A36089C5900E6CC6939A0765AE7E18610_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * __this, const RuntimeMethod* method)
{
	{
		// get => m_Value;
		RuntimeObject * L_0 = (RuntimeObject *)__this->get_m_Value_2();
		return L_0;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Object>::set_value(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_set_value_mD0E8FA4757B4E751ADEEFF7FBFC917E6A11A5A22_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * __this, RuntimeObject * ___value0, const RuntimeMethod* method)
{
	{
		// set => m_Value = value;
		RuntimeObject * L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_mB65184A34C60C6DB7175928E9C8C86AB15E3B751_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * __this, const RuntimeMethod* method)
{
	RuntimeObject * V_0 = NULL;
	{
		// : this(default, false)
		il2cpp_codegen_initobj((&V_0), sizeof(RuntimeObject *));
		RuntimeObject * L_0 = V_0;
		NullCheck((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)__this);
		((  void (*) (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *, RuntimeObject *, bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)__this, (RuntimeObject *)L_0, (bool)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Object>::.ctor(T,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1__ctor_m568F98FCF5CE4693885678BF2F5D3F94760E1E78_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * __this, RuntimeObject * ___value0, bool ___overrideState1, const RuntimeMethod* method)
{
	{
		// protected VolumeParameter(T value, bool overrideState)
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VolumeParameter__ctor_m0F5E882EFAE8F1B7AE0DFF62945878D71CA63D54((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, /*hidden argument*/NULL);
		// m_Value = value;
		RuntimeObject * L_0 = ___value0;
		__this->set_m_Value_2(L_0);
		// this.overrideState = overrideState;
		bool L_1 = ___overrideState1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)L_1);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Object>::Interp(UnityEngine.Rendering.VolumeParameter,UnityEngine.Rendering.VolumeParameter,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_m153C97DEC262ECFA05726BDCD61045B1DDB7BEE9_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___from0, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___to1, float ___t2, const RuntimeMethod* method)
{
	{
		// Interp(from.GetValue<T>(), to.GetValue<T>(), t);
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___from0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		RuntimeObject * L_1 = ((  RuntimeObject * (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_2 = ___to1;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2);
		RuntimeObject * L_3 = ((  RuntimeObject * (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		float L_4 = ___t2;
		NullCheck((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)__this);
		VirtActionInvoker3< RuntimeObject *, RuntimeObject *, float >::Invoke(14 /* System.Void UnityEngine.Rendering.VolumeParameter`1<System.Object>::Interp(T,T,System.Single) */, (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)__this, (RuntimeObject *)L_1, (RuntimeObject *)L_3, (float)L_4);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Object>::Interp(T,T,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Interp_m6E102B7FCADF33F4E90B71C9EB029A94AF3349E4_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * __this, RuntimeObject * ___from0, RuntimeObject * ___to1, float ___t2, const RuntimeMethod* method)
{
	VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * G_B2_0 = NULL;
	VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * G_B1_0 = NULL;
	RuntimeObject * G_B3_0 = NULL;
	VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * G_B3_1 = NULL;
	{
		// m_Value = t > 0f ? to : from;
		float L_0 = ___t2;
		G_B1_0 = ((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)(__this));
		if ((((float)L_0) > ((float)(0.0f))))
		{
			G_B2_0 = ((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)(__this));
			goto IL_000c;
		}
	}
	{
		RuntimeObject * L_1 = ___from0;
		G_B3_0 = L_1;
		G_B3_1 = ((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)(G_B1_0));
		goto IL_000d;
	}

IL_000c:
	{
		RuntimeObject * L_2 = ___to1;
		G_B3_0 = L_2;
		G_B3_1 = ((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)(G_B2_0));
	}

IL_000d:
	{
		NullCheck(G_B3_1);
		G_B3_1->set_m_Value_2(G_B3_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Object>::Override(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_Override_m0A117F79C518E97F3BE2C7101CC40072E6BC853B_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * __this, RuntimeObject * ___x0, const RuntimeMethod* method)
{
	{
		// overrideState = true;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.Rendering.VolumeParameter::set_overrideState(System.Boolean) */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this, (bool)1);
		// m_Value = x;
		RuntimeObject * L_0 = ___x0;
		__this->set_m_Value_2(L_0);
		// }
		return;
	}
}
// System.Void UnityEngine.Rendering.VolumeParameter`1<System.Object>::SetValue(UnityEngine.Rendering.VolumeParameter)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VolumeParameter_1_SetValue_m429A4BD58295726ADFD93F4A0FD6F1AAAC1D8DA2_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * __this, VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * ___parameter0, const RuntimeMethod* method)
{
	{
		// m_Value = parameter.GetValue<T>();
		VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB * L_0 = ___parameter0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0);
		RuntimeObject * L_1 = ((  RuntimeObject * (*) (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		__this->set_m_Value_2(L_1);
		// }
		return;
	}
}
// System.Int32 UnityEngine.Rendering.VolumeParameter`1<System.Object>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t VolumeParameter_1_GetHashCode_mFCB61FBFD813B341AAD95664777C561E4585CAD5_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	RuntimeObject * V_2 = NULL;
	{
		// int hash = 17;
		V_0 = (int32_t)((int32_t)17);
		// hash = hash * 23 + overrideState.GetHashCode();
		int32_t L_0 = V_0;
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_1 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		V_1 = (bool)L_1;
		int32_t L_2 = Boolean_GetHashCode_m03AF8B3CECAE9106C44A00E3B33E51CBFC45C411((bool*)(bool*)(&V_1), /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)((int32_t)23))), (int32_t)L_2));
		// if (!EqualityComparer<T>.Default.Equals(value, default)) // Catches null for references with boxing of value types
		EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 * L_3 = ((  EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		NullCheck((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)__this);
		RuntimeObject * L_4 = VirtFuncInvoker0< RuntimeObject * >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Object>::get_value() */, (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)__this);
		il2cpp_codegen_initobj((&V_2), sizeof(RuntimeObject *));
		RuntimeObject * L_5 = V_2;
		NullCheck((EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 *)L_3);
		bool L_6 = VirtFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<System.Object>::Equals(!0,!0) */, (EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 *)L_3, (RuntimeObject *)L_4, (RuntimeObject *)L_5);
		if (L_6)
		{
			goto IL_004c;
		}
	}
	{
		// hash = hash * 23 + value.GetHashCode();
		int32_t L_7 = V_0;
		NullCheck((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)__this);
		RuntimeObject * L_8 = VirtFuncInvoker0< RuntimeObject * >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Object>::get_value() */, (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)__this);
		V_2 = (RuntimeObject *)L_8;
		NullCheck((RuntimeObject *)(V_2));
		int32_t L_9 = VirtFuncInvoker0< int32_t >::Invoke(2 /* System.Int32 System.Object::GetHashCode() */, (RuntimeObject *)(V_2));
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_7, (int32_t)((int32_t)23))), (int32_t)L_9));
	}

IL_004c:
	{
		// return hash;
		int32_t L_10 = V_0;
		return L_10;
	}
}
// System.String UnityEngine.Rendering.VolumeParameter`1<System.Object>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* VolumeParameter_1_ToString_m98DEF918D3848ED5AF582300AA707E816743AF66_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_ToString_m98DEF918D3848ED5AF582300AA707E816743AF66_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public override string ToString() => $"{value} ({overrideState})";
		NullCheck((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)__this);
		RuntimeObject * L_0 = VirtFuncInvoker0< RuntimeObject * >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Object>::get_value() */, (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)__this);
		NullCheck((VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_1 = VirtFuncInvoker0< bool >::Invoke(4 /* System.Boolean UnityEngine.Rendering.VolumeParameter::get_overrideState() */, (VolumeParameter_tC12A1A4DFCC791C06995421E31A6AE4A458D7BCB *)__this);
		bool L_2 = L_1;
		RuntimeObject * L_3 = Box(Boolean_t07D1E3F34E4813023D64F584DFF7B34C9D922F37_il2cpp_TypeInfo_var, &L_2);
		String_t* L_4 = String_Format_m8D1CB0410C35E052A53AE957C914C841E54BAB66((String_t*)_stringLiteralA5E215A6DBE803E908043576B18C4FAD26AD44F7, (RuntimeObject *)L_0, (RuntimeObject *)L_3, /*hidden argument*/NULL);
		return L_4;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Object>::op_Equality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Equality_m786AC30A22D485466D5D38A6DC54203621DA8142_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * ___lhs0, RuntimeObject * ___rhs1, const RuntimeMethod* method)
{
	RuntimeObject * V_0 = NULL;
	{
		// public static bool operator==(VolumeParameter<T> lhs, T rhs) => lhs != null && !ReferenceEquals(lhs.value, null) && lhs.value.Equals(rhs);
		VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * L_0 = ___lhs0;
		if (!L_0)
		{
			goto IL_002b;
		}
	}
	{
		VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * L_1 = ___lhs0;
		NullCheck((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)L_1);
		RuntimeObject * L_2 = VirtFuncInvoker0< RuntimeObject * >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Object>::get_value() */, (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)L_1);
		if (!L_2)
		{
			goto IL_002b;
		}
	}
	{
		VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * L_3 = ___lhs0;
		NullCheck((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)L_3);
		RuntimeObject * L_4 = VirtFuncInvoker0< RuntimeObject * >::Invoke(12 /* T UnityEngine.Rendering.VolumeParameter`1<System.Object>::get_value() */, (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)L_3);
		V_0 = (RuntimeObject *)L_4;
		RuntimeObject * L_5 = ___rhs1;
		NullCheck((RuntimeObject *)(V_0));
		bool L_6 = VirtFuncInvoker1< bool, RuntimeObject * >::Invoke(0 /* System.Boolean System.Object::Equals(System.Object) */, (RuntimeObject *)(V_0), (RuntimeObject *)L_5);
		return L_6;
	}

IL_002b:
	{
		return (bool)0;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Object>::op_Inequality(UnityEngine.Rendering.VolumeParameter`1<T>,T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_op_Inequality_m6B249A4ED1C3BC3BC82780D4D159C1606CE613F5_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * ___lhs0, RuntimeObject * ___rhs1, const RuntimeMethod* method)
{
	{
		// public static bool operator!=(VolumeParameter<T> lhs, T rhs) => !(lhs == rhs);
		VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * L_0 = ___lhs0;
		RuntimeObject * L_1 = ___rhs1;
		bool L_2 = ((  bool (*) (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)L_0, (RuntimeObject *)L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		return (bool)((((int32_t)L_2) == ((int32_t)0))? 1 : 0);
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Object>::Equals(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m3E71487399AF0F166D8A1E689B968C6EE8702DAC_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * __this, VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * ___other0, const RuntimeMethod* method)
{
	{
		// if (ReferenceEquals(null, other))
		VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * L_0 = ___other0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, other))
		VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * L_1 = ___other0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)__this) == ((RuntimeObject*)(VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// return EqualityComparer<T>.Default.Equals(m_Value, other.m_Value);
		EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 * L_2 = ((  EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		RuntimeObject * L_3 = (RuntimeObject *)__this->get_m_Value_2();
		VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * L_4 = ___other0;
		NullCheck(L_4);
		RuntimeObject * L_5 = (RuntimeObject *)L_4->get_m_Value_2();
		NullCheck((EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 *)L_2);
		bool L_6 = VirtFuncInvoker2< bool, RuntimeObject *, RuntimeObject * >::Invoke(8 /* System.Boolean System.Collections.Generic.EqualityComparer`1<System.Object>::Equals(!0,!0) */, (EqualityComparer_1_t469B0BBE7B6765C576211BEF8F2803A5AD411A20 *)L_2, (RuntimeObject *)L_3, (RuntimeObject *)L_5);
		return L_6;
	}
}
// System.Boolean UnityEngine.Rendering.VolumeParameter`1<System.Object>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VolumeParameter_1_Equals_m43533DA3167983D9D59102963A9256B1B32E87F3_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VolumeParameter_1_Equals_m43533DA3167983D9D59102963A9256B1B32E87F3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (ReferenceEquals(null, obj))
		RuntimeObject * L_0 = ___obj0;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0005:
	{
		// if (ReferenceEquals(this, obj))
		RuntimeObject * L_1 = ___obj0;
		if ((!(((RuntimeObject*)(VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)__this) == ((RuntimeObject*)(RuntimeObject *)L_1))))
		{
			goto IL_000b;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_000b:
	{
		// if (obj.GetType() != GetType())
		RuntimeObject * L_2 = ___obj0;
		NullCheck((RuntimeObject *)L_2);
		Type_t * L_3 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)L_2, /*hidden argument*/NULL);
		NullCheck((RuntimeObject *)__this);
		Type_t * L_4 = Object_GetType_m571FE8360C10B98C23AAF1F066D92C08CC94F45B((RuntimeObject *)__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		bool L_5 = Type_op_Inequality_m6DDC5E923203A79BF505F9275B694AD3FAA36DB0((Type_t *)L_3, (Type_t *)L_4, /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_0020;
		}
	}
	{
		// return false;
		return (bool)0;
	}

IL_0020:
	{
		// return Equals((VolumeParameter<T>)obj);
		RuntimeObject * L_6 = ___obj0;
		NullCheck((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)__this);
		bool L_7 = ((  bool (*) (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *, VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)__this, (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)((VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 *)Castclass((RuntimeObject*)L_6, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10))), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_7;
	}
}
// T UnityEngine.Rendering.VolumeParameter`1<System.Object>::op_Explicit(UnityEngine.Rendering.VolumeParameter`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * VolumeParameter_1_op_Explicit_m1038E6089685A688180DD20373DAC79B240EC104_gshared (VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * ___prop0, const RuntimeMethod* method)
{
	{
		// public static explicit operator T(VolumeParameter<T> prop) => prop.m_Value;
		VolumeParameter_1_t632335EE356E4A6F435B0D046FC5716C1E5ED6D0 * L_0 = ___prop0;
		NullCheck(L_0);
		RuntimeObject * L_1 = (RuntimeObject *)L_0->get_m_Value_2();
		return L_1;
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
// System.Void System.WeakReference`1<System.Object>::.ctor(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WeakReference_1__ctor_m26CA3930ADE164EA48E474988EE6B82D26E5F9E9_gshared (WeakReference_1_t223E87F227F99044D112FF1E7FC7DA135365CB76 * __this, RuntimeObject * ___target0, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = ___target0;
		NullCheck((WeakReference_1_t223E87F227F99044D112FF1E7FC7DA135365CB76 *)__this);
		((  void (*) (WeakReference_1_t223E87F227F99044D112FF1E7FC7DA135365CB76 *, RuntimeObject *, bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((WeakReference_1_t223E87F227F99044D112FF1E7FC7DA135365CB76 *)__this, (RuntimeObject *)L_0, (bool)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		return;
	}
}
// System.Void System.WeakReference`1<System.Object>::.ctor(T,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WeakReference_1__ctor_mF2828744686B43E540BF1C0908FA8C14694F666F_gshared (WeakReference_1_t223E87F227F99044D112FF1E7FC7DA135365CB76 * __this, RuntimeObject * ___target0, bool ___trackResurrection1, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t G_B3_0 = 0;
	{
		NullCheck((RuntimeObject *)__this);
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405((RuntimeObject *)__this, /*hidden argument*/NULL);
		bool L_0 = ___trackResurrection1;
		__this->set_trackResurrection_1(L_0);
		bool L_1 = ___trackResurrection1;
		if (L_1)
		{
			goto IL_0013;
		}
	}
	{
		G_B3_0 = 0;
		goto IL_0014;
	}

IL_0013:
	{
		G_B3_0 = 1;
	}

IL_0014:
	{
		V_0 = (int32_t)G_B3_0;
		RuntimeObject * L_2 = ___target0;
		int32_t L_3 = V_0;
		GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603  L_4 = GCHandle_Alloc_mBF5C4C0E8605F97427BFDF96D68AACD4A4D6DDEC((RuntimeObject *)L_2, (int32_t)L_3, /*hidden argument*/NULL);
		__this->set_handle_0(L_4);
		return;
	}
}
// System.Void System.WeakReference`1<System.Object>::.ctor(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WeakReference_1__ctor_mC76935DFFEF0678A77A4811865B9F4D350D72741_gshared (WeakReference_1_t223E87F227F99044D112FF1E7FC7DA135365CB76 * __this, SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 * ___info0, StreamingContext_t5888E7E8C81AB6EF3B14FDDA6674F458076A8505  ___context1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WeakReference_1__ctor_mC76935DFFEF0678A77A4811865B9F4D350D72741_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject * V_0 = NULL;
	int32_t V_1 = 0;
	int32_t G_B5_0 = 0;
	{
		NullCheck((RuntimeObject *)__this);
		Object__ctor_m88880E0413421D13FD95325EDCE231707CE1F405((RuntimeObject *)__this, /*hidden argument*/NULL);
		SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 * L_0 = ___info0;
		if (L_0)
		{
			goto IL_0014;
		}
	}
	{
		ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB * L_1 = (ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB *)il2cpp_codegen_object_new(ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB_il2cpp_TypeInfo_var);
		ArgumentNullException__ctor_m81AB157B93BFE2FBFDB08B88F84B444293042F97(L_1, (String_t*)_stringLiteralA7B00F7F25C375B2501A6ADBC86D092B23977085, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_1, WeakReference_1__ctor_mC76935DFFEF0678A77A4811865B9F4D350D72741_RuntimeMethod_var);
	}

IL_0014:
	{
		SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 * L_2 = ___info0;
		NullCheck((SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 *)L_2);
		bool L_3 = SerializationInfo_GetBoolean_m705ADACFB52D6385DDB6B2525C1979ECBE6D5849((SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 *)L_2, (String_t*)_stringLiteral7D20B8219CA0491872B2E811B262066A5DD875A7, /*hidden argument*/NULL);
		__this->set_trackResurrection_1(L_3);
		SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 * L_4 = ___info0;
		RuntimeTypeHandle_tC33965ADA3E041E0C94AF05E5CB527B56482CEF9  L_5 = { reinterpret_cast<intptr_t> (IL2CPP_RGCTX_TYPE(method->klass->rgctx_data, 2)) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_6 = Type_GetTypeFromHandle_m8BB57524FF7F9DB1803BC561D2B3A4DBACEB385E((RuntimeTypeHandle_tC33965ADA3E041E0C94AF05E5CB527B56482CEF9 )L_5, /*hidden argument*/NULL);
		NullCheck((SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 *)L_4);
		RuntimeObject * L_7 = SerializationInfo_GetValue_mF6E311779D55AD7C80B2D19FF2A7E9683AEF2A99((SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 *)L_4, (String_t*)_stringLiteral5CA6E7C0AE72196B2817D93A78C719652EC691C0, (Type_t *)L_6, /*hidden argument*/NULL);
		V_0 = (RuntimeObject *)L_7;
		bool L_8 = (bool)__this->get_trackResurrection_1();
		if (L_8)
		{
			goto IL_0046;
		}
	}
	{
		G_B5_0 = 0;
		goto IL_0047;
	}

IL_0046:
	{
		G_B5_0 = 1;
	}

IL_0047:
	{
		V_1 = (int32_t)G_B5_0;
		RuntimeObject * L_9 = V_0;
		int32_t L_10 = V_1;
		GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603  L_11 = GCHandle_Alloc_mBF5C4C0E8605F97427BFDF96D68AACD4A4D6DDEC((RuntimeObject *)L_9, (int32_t)L_10, /*hidden argument*/NULL);
		__this->set_handle_0(L_11);
		return;
	}
}
// System.Void System.WeakReference`1<System.Object>::GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WeakReference_1_GetObjectData_m7C63330FAC22CBE86AA1BDE2F34DFDA8B1E41272_gshared (WeakReference_1_t223E87F227F99044D112FF1E7FC7DA135365CB76 * __this, SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 * ___info0, StreamingContext_t5888E7E8C81AB6EF3B14FDDA6674F458076A8505  ___context1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WeakReference_1_GetObjectData_m7C63330FAC22CBE86AA1BDE2F34DFDA8B1E41272_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 * L_0 = ___info0;
		if (L_0)
		{
			goto IL_000e;
		}
	}
	{
		ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB * L_1 = (ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB *)il2cpp_codegen_object_new(ArgumentNullException_tFB5C4621957BC53A7D1B4FDD5C38B4D6E15DB8FB_il2cpp_TypeInfo_var);
		ArgumentNullException__ctor_m81AB157B93BFE2FBFDB08B88F84B444293042F97(L_1, (String_t*)_stringLiteralA7B00F7F25C375B2501A6ADBC86D092B23977085, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_1, WeakReference_1_GetObjectData_m7C63330FAC22CBE86AA1BDE2F34DFDA8B1E41272_RuntimeMethod_var);
	}

IL_000e:
	{
		SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 * L_2 = ___info0;
		bool L_3 = (bool)__this->get_trackResurrection_1();
		NullCheck((SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 *)L_2);
		SerializationInfo_AddValue_m324F3E0B02B746D5F460499F5A25988FD608AD7B((SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 *)L_2, (String_t*)_stringLiteral7D20B8219CA0491872B2E811B262066A5DD875A7, (bool)L_3, /*hidden argument*/NULL);
		GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 * L_4 = (GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)__this->get_address_of_handle_0();
		bool L_5 = GCHandle_get_IsAllocated_mEDA4DAC6AD6D881110E96CAFDAB78C068F5B144D((GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)(GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)L_4, /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_0043;
		}
	}
	{
		SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 * L_6 = ___info0;
		GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 * L_7 = (GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)__this->get_address_of_handle_0();
		RuntimeObject * L_8 = GCHandle_get_Target_m6C296AD6520ECDAFC9498E9387677F522871F883((GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)(GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)L_7, /*hidden argument*/NULL);
		NullCheck((SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 *)L_6);
		SerializationInfo_AddValue_mA50C2668EF700C2239DDC362F8DB409020BB763D((SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 *)L_6, (String_t*)_stringLiteral5CA6E7C0AE72196B2817D93A78C719652EC691C0, (RuntimeObject *)L_8, /*hidden argument*/NULL);
		return;
	}

IL_0043:
	{
		SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 * L_9 = ___info0;
		NullCheck((SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 *)L_9);
		SerializationInfo_AddValue_mA50C2668EF700C2239DDC362F8DB409020BB763D((SerializationInfo_t097DA64D9DB49ED7F2458E964BE8CCCF63FC67C1 *)L_9, (String_t*)_stringLiteral5CA6E7C0AE72196B2817D93A78C719652EC691C0, (RuntimeObject *)NULL, /*hidden argument*/NULL);
		return;
	}
}
// System.Boolean System.WeakReference`1<System.Object>::TryGetTarget(T&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WeakReference_1_TryGetTarget_mA9F884507AABEFCC394D06FEEB7AD14ECEB5CD6B_gshared (WeakReference_1_t223E87F227F99044D112FF1E7FC7DA135365CB76 * __this, RuntimeObject ** ___target0, const RuntimeMethod* method)
{
	{
		GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 * L_0 = (GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)__this->get_address_of_handle_0();
		bool L_1 = GCHandle_get_IsAllocated_mEDA4DAC6AD6D881110E96CAFDAB78C068F5B144D((GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)(GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)L_0, /*hidden argument*/NULL);
		if (L_1)
		{
			goto IL_0016;
		}
	}
	{
		RuntimeObject ** L_2 = ___target0;
		il2cpp_codegen_initobj(L_2, sizeof(RuntimeObject *));
		return (bool)0;
	}

IL_0016:
	{
		RuntimeObject ** L_3 = ___target0;
		GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 * L_4 = (GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)__this->get_address_of_handle_0();
		RuntimeObject * L_5 = GCHandle_get_Target_m6C296AD6520ECDAFC9498E9387677F522871F883((GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)(GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)L_4, /*hidden argument*/NULL);
		*(RuntimeObject **)L_3 = ((RuntimeObject *)Castclass((RuntimeObject*)L_5, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 1)));
		Il2CppCodeGenWriteBarrier((void**)(RuntimeObject **)L_3, (void*)((RuntimeObject *)Castclass((RuntimeObject*)L_5, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 1))));
		RuntimeObject ** L_6 = ___target0;
		RuntimeObject * L_7 = (*(RuntimeObject **)L_6);
		return (bool)((!(((RuntimeObject*)(RuntimeObject *)L_7) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
	}
}
// System.Void System.WeakReference`1<System.Object>::Finalize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WeakReference_1_Finalize_mFFF6306032DA8256E4F7D125EF76342E83CD6F68_gshared (WeakReference_1_t223E87F227F99044D112FF1E7FC7DA135365CB76 * __this, const RuntimeMethod* method)
{
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 1);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);

IL_0000:
	try
	{ // begin try (depth: 1)
		GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 * L_0 = (GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)__this->get_address_of_handle_0();
		GCHandle_Free_mB4E9415544FC9F0075C02AB17E270E49AF006025((GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)(GCHandle_t757890BC4BBBEDE5A623A3C110013EDD24613603 *)L_0, /*hidden argument*/NULL);
		IL2CPP_LEAVE(0x14, FINALLY_000d);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000d;
	}

FINALLY_000d:
	{ // begin finally (depth: 1)
		NullCheck((RuntimeObject *)__this);
		Object_Finalize_mC59C83CF4F7707E425FFA6362931C25D4C36676A((RuntimeObject *)__this, /*hidden argument*/NULL);
		IL2CPP_END_FINALLY(13)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(13)
	{
		IL2CPP_JUMP_TBL(0x14, IL_0014)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0014:
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
// System.Void System.Linq.Enumerable_WhereArrayIterator`1<System.Object>::.ctor(TSource[],System.Func`2<TSource,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereArrayIterator_1__ctor_m5358A7C3085BC8A103D9793A6D2FCB7E36A2D2CE_gshared (WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86 * __this, ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* ___source0, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate1, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TSource> System.Linq.Enumerable_WhereArrayIterator`1<System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereArrayIterator_1_Clone_m2CF9FC5638704567A64DC86DD674EB4E6F380F79_gshared (WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86 * __this, const RuntimeMethod* method)
{
	{
		ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* L_0 = (ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*)__this->get_source_3();
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86 * L_2 = (WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86 *, ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_2, (ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*)L_0, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_2;
	}
}
// System.Boolean System.Linq.Enumerable_WhereArrayIterator`1<System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereArrayIterator_1_MoveNext_m37A95072CA5380DE7F2D6B57990507C92F045BB3_gshared (WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86 * __this, const RuntimeMethod* method)
{
	RuntimeObject * V_0 = NULL;
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		if ((!(((uint32_t)L_0) == ((uint32_t)1))))
		{
			goto IL_0058;
		}
	}
	{
		goto IL_0042;
	}

IL_000b:
	{
		ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* L_1 = (ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*)__this->get_source_3();
		int32_t L_2 = (int32_t)__this->get_index_5();
		NullCheck(L_1);
		int32_t L_3 = L_2;
		RuntimeObject * L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		V_0 = (RuntimeObject *)L_4;
		int32_t L_5 = (int32_t)__this->get_index_5();
		__this->set_index_5(((int32_t)il2cpp_codegen_add((int32_t)L_5, (int32_t)1)));
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_6 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		RuntimeObject * L_7 = V_0;
		NullCheck((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_6);
		bool L_8 = ((  bool (*) (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_6, (RuntimeObject *)L_7, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		if (!L_8)
		{
			goto IL_0042;
		}
	}
	{
		RuntimeObject * L_9 = V_0;
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_9);
		return (bool)1;
	}

IL_0042:
	{
		int32_t L_10 = (int32_t)__this->get_index_5();
		ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* L_11 = (ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*)__this->get_source_3();
		NullCheck(L_11);
		if ((((int32_t)L_10) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_11)->max_length)))))))
		{
			goto IL_000b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_0058:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereArrayIterator`1<System.Object>::Where(System.Func`2<TSource,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereArrayIterator_1_Where_m294488B1E62E494973DD4880121A17A0C8A08118_gshared (WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86 * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* L_0 = (ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*)__this->get_source_3();
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_2 = ___predicate0;
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_3 = ((  Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * (*) (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6)->methodPointer)((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_1, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6));
		WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86 * L_4 = (WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereArrayIterator_1_t7D84D638EB94F5CC3BE1B29D8FC781CA8CD15A86 *, ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_4, (ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*)L_0, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_4;
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
// System.Void System.Linq.Enumerable_WhereEnumerableIterator`1<UnityEngine.Color>::.ctor(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereEnumerableIterator_1__ctor_m0D7A8A052BC9DE6D842234A55DFCDD0FAB6C9057_gshared (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 * __this, RuntimeObject* ___source0, Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * ___predicate1, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this);
		((  void (*) (Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		RuntimeObject* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1<UnityEngine.Color>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 * WhereEnumerableIterator_1_Clone_mE6DD25223B01E5DA577A87A50EC1A60372BE6DF0_gshared (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * L_1 = (Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *)__this->get_predicate_4();
		WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 * L_2 = (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 *, RuntimeObject*, Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_2, (RuntimeObject*)L_0, (Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *)L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_2;
	}
}
// System.Void System.Linq.Enumerable_WhereEnumerableIterator`1<UnityEngine.Color>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereEnumerableIterator_1_Dispose_mCEF7D940B21C37FAC2966547F8428D8BE4C59338_gshared (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereEnumerableIterator_1_Dispose_mCEF7D940B21C37FAC2966547F8428D8BE4C59338_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_enumerator_5();
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		RuntimeObject* L_1 = (RuntimeObject*)__this->get_enumerator_5();
		NullCheck((RuntimeObject*)L_1);
		InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t099785737FC6A1E3699919A94109383715A8D807_il2cpp_TypeInfo_var, (RuntimeObject*)L_1);
	}

IL_0013:
	{
		__this->set_enumerator_5((RuntimeObject*)NULL);
		NullCheck((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this);
		((  void (*) (Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		return;
	}
}
// System.Boolean System.Linq.Enumerable_WhereEnumerableIterator`1<UnityEngine.Color>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereEnumerableIterator_1_MoveNext_m3D143965FBF0E34F74297CA37DF3B3184262857A_gshared (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereEnumerableIterator_1_MoveNext_m3D143965FBF0E34F74297CA37DF3B3184262857A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		int32_t L_0 = (int32_t)((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_004e;
		}
	}
	{
		goto IL_0061;
	}

IL_0011:
	{
		RuntimeObject* L_3 = (RuntimeObject*)__this->get_source_3();
		NullCheck((RuntimeObject*)L_3);
		RuntimeObject* L_4 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<!0> System.Collections.Generic.IEnumerable`1<UnityEngine.Color>::GetEnumerator() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 5), (RuntimeObject*)L_3);
		__this->set_enumerator_5(L_4);
		((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this)->set_state_1(2);
		goto IL_004e;
	}

IL_002b:
	{
		RuntimeObject* L_5 = (RuntimeObject*)__this->get_enumerator_5();
		NullCheck((RuntimeObject*)L_5);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_6 = InterfaceFuncInvoker0< Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  >::Invoke(0 /* !0 System.Collections.Generic.IEnumerator`1<UnityEngine.Color>::get_Current() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 6), (RuntimeObject*)L_5);
		V_1 = (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )L_6;
		Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * L_7 = (Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *)__this->get_predicate_4();
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_8 = V_1;
		NullCheck((Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *)L_7);
		bool L_9 = ((  bool (*) (Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *, Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *)L_7, (Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659 )L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		if (!L_9)
		{
			goto IL_004e;
		}
	}
	{
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_10 = V_1;
		((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this)->set_current_2(L_10);
		return (bool)1;
	}

IL_004e:
	{
		RuntimeObject* L_11 = (RuntimeObject*)__this->get_enumerator_5();
		NullCheck((RuntimeObject*)L_11);
		bool L_12 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t5956F3AFB7ECF1117E3BC5890E7FC7B7F7A04105_il2cpp_TypeInfo_var, (RuntimeObject*)L_11);
		if (L_12)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<UnityEngine.Color>::Dispose() */, (Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this);
	}

IL_0061:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1<UnityEngine.Color>::Where(System.Func`2<TSource,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereEnumerableIterator_1_Where_m80C6EF5D06412C00F80E8A7A490779AFEB13C2B8_gshared (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 * __this, Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * ___predicate0, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * L_1 = (Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *)__this->get_predicate_4();
		Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * L_2 = ___predicate0;
		Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * L_3 = ((  Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * (*) (Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *, Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *)L_1, (Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 * L_4 = (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 *, RuntimeObject*, Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_4, (RuntimeObject*)L_0, (Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *)L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_4;
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
// System.Void System.Linq.Enumerable_WhereEnumerableIterator`1<System.Int32>::.ctor(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereEnumerableIterator_1__ctor_mED9FE30D754A0ABE546A685684F523BC57509D0E_gshared (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA * __this, RuntimeObject* ___source0, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate1, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this);
		((  void (*) (Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		RuntimeObject* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1<System.Int32>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 * WhereEnumerableIterator_1_Clone_mB3FE252392A8FEA3638826A2C4D1195A4D3743BB_gshared (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA * L_2 = (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA *, RuntimeObject*, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_2, (RuntimeObject*)L_0, (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_2;
	}
}
// System.Void System.Linq.Enumerable_WhereEnumerableIterator`1<System.Int32>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereEnumerableIterator_1_Dispose_mB841131399B8BA11B9D6DB37E11F90B1BFFBDA2D_gshared (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereEnumerableIterator_1_Dispose_mB841131399B8BA11B9D6DB37E11F90B1BFFBDA2D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_enumerator_5();
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		RuntimeObject* L_1 = (RuntimeObject*)__this->get_enumerator_5();
		NullCheck((RuntimeObject*)L_1);
		InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t099785737FC6A1E3699919A94109383715A8D807_il2cpp_TypeInfo_var, (RuntimeObject*)L_1);
	}

IL_0013:
	{
		__this->set_enumerator_5((RuntimeObject*)NULL);
		NullCheck((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this);
		((  void (*) (Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		return;
	}
}
// System.Boolean System.Linq.Enumerable_WhereEnumerableIterator`1<System.Int32>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereEnumerableIterator_1_MoveNext_m819D14CC69CC48B0B84E497DFF1953AAFFF13333_gshared (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereEnumerableIterator_1_MoveNext_m819D14CC69CC48B0B84E497DFF1953AAFFF13333_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		int32_t L_0 = (int32_t)((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_004e;
		}
	}
	{
		goto IL_0061;
	}

IL_0011:
	{
		RuntimeObject* L_3 = (RuntimeObject*)__this->get_source_3();
		NullCheck((RuntimeObject*)L_3);
		RuntimeObject* L_4 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<!0> System.Collections.Generic.IEnumerable`1<System.Int32>::GetEnumerator() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 5), (RuntimeObject*)L_3);
		__this->set_enumerator_5(L_4);
		((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this)->set_state_1(2);
		goto IL_004e;
	}

IL_002b:
	{
		RuntimeObject* L_5 = (RuntimeObject*)__this->get_enumerator_5();
		NullCheck((RuntimeObject*)L_5);
		int32_t L_6 = InterfaceFuncInvoker0< int32_t >::Invoke(0 /* !0 System.Collections.Generic.IEnumerator`1<System.Int32>::get_Current() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 6), (RuntimeObject*)L_5);
		V_1 = (int32_t)L_6;
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_7 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		int32_t L_8 = V_1;
		NullCheck((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_7);
		bool L_9 = ((  bool (*) (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_7, (int32_t)L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		if (!L_9)
		{
			goto IL_004e;
		}
	}
	{
		int32_t L_10 = V_1;
		((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this)->set_current_2(L_10);
		return (bool)1;
	}

IL_004e:
	{
		RuntimeObject* L_11 = (RuntimeObject*)__this->get_enumerator_5();
		NullCheck((RuntimeObject*)L_11);
		bool L_12 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t5956F3AFB7ECF1117E3BC5890E7FC7B7F7A04105_il2cpp_TypeInfo_var, (RuntimeObject*)L_11);
		if (L_12)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Int32>::Dispose() */, (Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this);
	}

IL_0061:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1<System.Int32>::Where(System.Func`2<TSource,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereEnumerableIterator_1_Where_mD1B249C431E67DD9B73D781BEA79EF23E556B75E_gshared (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA * __this, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate0, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_2 = ___predicate0;
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_3 = ((  Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * (*) (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_1, (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA * L_4 = (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA *, RuntimeObject*, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_4, (RuntimeObject*)L_0, (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_4;
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
// System.Void System.Linq.Enumerable_WhereEnumerableIterator`1<UnityEngine.Vector3>::.ctor(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereEnumerableIterator_1__ctor_m591288EAD97C8D9D310610BFA96A98922E8AF347_gshared (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 * __this, RuntimeObject* ___source0, Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * ___predicate1, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this);
		((  void (*) (Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		RuntimeObject* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1<UnityEngine.Vector3>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF * WhereEnumerableIterator_1_Clone_m4ADE59B0DB8541F1FA457BE9CAD3EF2868817E56_gshared (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * L_1 = (Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *)__this->get_predicate_4();
		WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 * L_2 = (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 *, RuntimeObject*, Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_2, (RuntimeObject*)L_0, (Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *)L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_2;
	}
}
// System.Void System.Linq.Enumerable_WhereEnumerableIterator`1<UnityEngine.Vector3>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereEnumerableIterator_1_Dispose_m64C95774F2781EB65AC8FE5A6CD5043BF61CEED8_gshared (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereEnumerableIterator_1_Dispose_m64C95774F2781EB65AC8FE5A6CD5043BF61CEED8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_enumerator_5();
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		RuntimeObject* L_1 = (RuntimeObject*)__this->get_enumerator_5();
		NullCheck((RuntimeObject*)L_1);
		InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t099785737FC6A1E3699919A94109383715A8D807_il2cpp_TypeInfo_var, (RuntimeObject*)L_1);
	}

IL_0013:
	{
		__this->set_enumerator_5((RuntimeObject*)NULL);
		NullCheck((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this);
		((  void (*) (Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		return;
	}
}
// System.Boolean System.Linq.Enumerable_WhereEnumerableIterator`1<UnityEngine.Vector3>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereEnumerableIterator_1_MoveNext_m1ABB7BE9CE39660B4C3E276C11F7757E9DDC4BD0_gshared (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereEnumerableIterator_1_MoveNext_m1ABB7BE9CE39660B4C3E276C11F7757E9DDC4BD0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		int32_t L_0 = (int32_t)((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_004e;
		}
	}
	{
		goto IL_0061;
	}

IL_0011:
	{
		RuntimeObject* L_3 = (RuntimeObject*)__this->get_source_3();
		NullCheck((RuntimeObject*)L_3);
		RuntimeObject* L_4 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<!0> System.Collections.Generic.IEnumerable`1<UnityEngine.Vector3>::GetEnumerator() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 5), (RuntimeObject*)L_3);
		__this->set_enumerator_5(L_4);
		((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this)->set_state_1(2);
		goto IL_004e;
	}

IL_002b:
	{
		RuntimeObject* L_5 = (RuntimeObject*)__this->get_enumerator_5();
		NullCheck((RuntimeObject*)L_5);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_6 = InterfaceFuncInvoker0< Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  >::Invoke(0 /* !0 System.Collections.Generic.IEnumerator`1<UnityEngine.Vector3>::get_Current() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 6), (RuntimeObject*)L_5);
		V_1 = (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )L_6;
		Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * L_7 = (Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *)__this->get_predicate_4();
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_8 = V_1;
		NullCheck((Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *)L_7);
		bool L_9 = ((  bool (*) (Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *, Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *)L_7, (Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E )L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		if (!L_9)
		{
			goto IL_004e;
		}
	}
	{
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_10 = V_1;
		((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this)->set_current_2(L_10);
		return (bool)1;
	}

IL_004e:
	{
		RuntimeObject* L_11 = (RuntimeObject*)__this->get_enumerator_5();
		NullCheck((RuntimeObject*)L_11);
		bool L_12 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t5956F3AFB7ECF1117E3BC5890E7FC7B7F7A04105_il2cpp_TypeInfo_var, (RuntimeObject*)L_11);
		if (L_12)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<UnityEngine.Vector3>::Dispose() */, (Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this);
	}

IL_0061:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1<UnityEngine.Vector3>::Where(System.Func`2<TSource,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereEnumerableIterator_1_Where_m1F41645152C3BCC477C7D1665291990DC5239347_gshared (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 * __this, Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * ___predicate0, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * L_1 = (Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *)__this->get_predicate_4();
		Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * L_2 = ___predicate0;
		Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * L_3 = ((  Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * (*) (Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *, Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *)L_1, (Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 * L_4 = (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 *, RuntimeObject*, Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_4, (RuntimeObject*)L_0, (Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *)L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_4;
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
// System.Void System.Linq.Enumerable_WhereEnumerableIterator`1<System.Object>::.ctor(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereEnumerableIterator_1__ctor_mE8BFA73027409E16668838C4664CE7C48F3254DF_gshared (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * __this, RuntimeObject* ___source0, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate1, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		RuntimeObject* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1<System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereEnumerableIterator_1_Clone_mD8AFDE4531ADC466665EEE89C052BFF645C1BDD6_gshared (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * L_2 = (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_2, (RuntimeObject*)L_0, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_2;
	}
}
// System.Void System.Linq.Enumerable_WhereEnumerableIterator`1<System.Object>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereEnumerableIterator_1_Dispose_m4E1339513102BB6B49AD33EDB569D3FFD24ED023_gshared (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereEnumerableIterator_1_Dispose_m4E1339513102BB6B49AD33EDB569D3FFD24ED023_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_enumerator_5();
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		RuntimeObject* L_1 = (RuntimeObject*)__this->get_enumerator_5();
		NullCheck((RuntimeObject*)L_1);
		InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t099785737FC6A1E3699919A94109383715A8D807_il2cpp_TypeInfo_var, (RuntimeObject*)L_1);
	}

IL_0013:
	{
		__this->set_enumerator_5((RuntimeObject*)NULL);
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		return;
	}
}
// System.Boolean System.Linq.Enumerable_WhereEnumerableIterator`1<System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereEnumerableIterator_1_MoveNext_m6D8A420AEB325BF252721010781EF31CF64D73FF_gshared (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereEnumerableIterator_1_MoveNext_m6D8A420AEB325BF252721010781EF31CF64D73FF_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	RuntimeObject * V_1 = NULL;
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_004e;
		}
	}
	{
		goto IL_0061;
	}

IL_0011:
	{
		RuntimeObject* L_3 = (RuntimeObject*)__this->get_source_3();
		NullCheck((RuntimeObject*)L_3);
		RuntimeObject* L_4 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<!0> System.Collections.Generic.IEnumerable`1<System.Object>::GetEnumerator() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 5), (RuntimeObject*)L_3);
		__this->set_enumerator_5(L_4);
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_state_1(2);
		goto IL_004e;
	}

IL_002b:
	{
		RuntimeObject* L_5 = (RuntimeObject*)__this->get_enumerator_5();
		NullCheck((RuntimeObject*)L_5);
		RuntimeObject * L_6 = InterfaceFuncInvoker0< RuntimeObject * >::Invoke(0 /* !0 System.Collections.Generic.IEnumerator`1<System.Object>::get_Current() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 6), (RuntimeObject*)L_5);
		V_1 = (RuntimeObject *)L_6;
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_7 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		RuntimeObject * L_8 = V_1;
		NullCheck((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_7);
		bool L_9 = ((  bool (*) (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_7, (RuntimeObject *)L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		if (!L_9)
		{
			goto IL_004e;
		}
	}
	{
		RuntimeObject * L_10 = V_1;
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_10);
		return (bool)1;
	}

IL_004e:
	{
		RuntimeObject* L_11 = (RuntimeObject*)__this->get_enumerator_5();
		NullCheck((RuntimeObject*)L_11);
		bool L_12 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t5956F3AFB7ECF1117E3BC5890E7FC7B7F7A04105_il2cpp_TypeInfo_var, (RuntimeObject*)L_11);
		if (L_12)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_0061:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereEnumerableIterator`1<System.Object>::Where(System.Func`2<TSource,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereEnumerableIterator_1_Where_m4016C553BF1DF1102C2A7B769244B2DC2EA3E716_gshared (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_2 = ___predicate0;
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_3 = ((  Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * (*) (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_1, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * L_4 = (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_4, (RuntimeObject*)L_0, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_4;
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
// System.Void System.Linq.Enumerable_WhereListIterator`1<System.Object>::.ctor(System.Collections.Generic.List`1<TSource>,System.Func`2<TSource,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereListIterator_1__ctor_m3EB9274A8CE9B71A41BA4B8E9E22D9735713DFA3_gshared (WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD * __this, List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * ___source0, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate1, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TSource> System.Linq.Enumerable_WhereListIterator`1<System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereListIterator_1_Clone_mF7E182440FC39477FC20CA48E0FAB270FED512F4_gshared (WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD * __this, const RuntimeMethod* method)
{
	{
		List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * L_0 = (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *)__this->get_source_3();
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD * L_2 = (WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD *, List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_2, (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *)L_0, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_2;
	}
}
// System.Boolean System.Linq.Enumerable_WhereListIterator`1<System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereListIterator_1_MoveNext_m11D0FD0206FC9B236608A1150FB26790BA09B2E5_gshared (WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	RuntimeObject * V_1 = NULL;
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_004e;
		}
	}
	{
		goto IL_0061;
	}

IL_0011:
	{
		List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * L_3 = (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *)__this->get_source_3();
		NullCheck((List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *)L_3);
		Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6  L_4 = ((  Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6  (*) (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *)L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		__this->set_enumerator_5(L_4);
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_state_1(2);
		goto IL_004e;
	}

IL_002b:
	{
		Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * L_5 = (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 *)__this->get_address_of_enumerator_5();
		RuntimeObject * L_6 = Enumerator_get_Current_m9C4EBBD2108B51885E750F927D7936290C8E20EE_inline((Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 *)(Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 *)L_5, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		V_1 = (RuntimeObject *)L_6;
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_7 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		RuntimeObject * L_8 = V_1;
		NullCheck((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_7);
		bool L_9 = ((  bool (*) (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6)->methodPointer)((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_7, (RuntimeObject *)L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6));
		if (!L_9)
		{
			goto IL_004e;
		}
	}
	{
		RuntimeObject * L_10 = V_1;
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_10);
		return (bool)1;
	}

IL_004e:
	{
		Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * L_11 = (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 *)__this->get_address_of_enumerator_5();
		bool L_12 = Enumerator_MoveNext_m2E56233762839CE55C67E00AC8DD3D4D3F6C0DF0((Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 *)(Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 *)L_11, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		if (L_12)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_0061:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable_WhereListIterator`1<System.Object>::Where(System.Func`2<TSource,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereListIterator_1_Where_m1AD3C3728A42EA6188BB39667495E67F8A078501_gshared (WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * L_0 = (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *)__this->get_source_3();
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_2 = ___predicate0;
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_3 = ((  Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * (*) (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 9)->methodPointer)((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_1, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 9));
		WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD * L_4 = (WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereListIterator_1_t42618389DB998070E03A982D15FA39BCA1DB56BD *, List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_4, (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *)L_0, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_4;
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
// System.Void System.Linq.Enumerable_WhereSelectArrayIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>::.ctor(TSource[],System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectArrayIterator_2__ctor_m20B96BB5016BB8B192DE6BE4D013000E616192C1_gshared (WhereSelectArrayIterator_2_tCB97637D799FCAD3115615E9FDA8E5CF80A108EC * __this, ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* ___source0, Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * ___predicate1, Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this);
		((  void (*) (Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 * WhereSelectArrayIterator_2_Clone_mEA06DCCEA7274B9D65A4ED313657A35367180EC1_gshared (WhereSelectArrayIterator_2_tCB97637D799FCAD3115615E9FDA8E5CF80A108EC * __this, const RuntimeMethod* method)
{
	{
		ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* L_0 = (ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36*)__this->get_source_3();
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_1 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * L_2 = (Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)__this->get_selector_5();
		WhereSelectArrayIterator_2_tCB97637D799FCAD3115615E9FDA8E5CF80A108EC * L_3 = (WhereSelectArrayIterator_2_tCB97637D799FCAD3115615E9FDA8E5CF80A108EC *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectArrayIterator_2_tCB97637D799FCAD3115615E9FDA8E5CF80A108EC *, ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36*, Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *, Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36*)L_0, (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_1, (Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectArrayIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectArrayIterator_2_MoveNext_m59DCCEEF77DA2CD6C6B66BD230A805A71A87D114_gshared (WhereSelectArrayIterator_2_tCB97637D799FCAD3115615E9FDA8E5CF80A108EC * __this, const RuntimeMethod* method)
{
	ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		int32_t L_0 = (int32_t)((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this)->get_state_1();
		if ((!(((uint32_t)L_0) == ((uint32_t)1))))
		{
			goto IL_006b;
		}
	}
	{
		goto IL_0055;
	}

IL_000b:
	{
		ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* L_1 = (ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36*)__this->get_source_3();
		int32_t L_2 = (int32_t)__this->get_index_6();
		NullCheck(L_1);
		int32_t L_3 = L_2;
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		V_0 = (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_4;
		int32_t L_5 = (int32_t)__this->get_index_6();
		__this->set_index_6(((int32_t)il2cpp_codegen_add((int32_t)L_5, (int32_t)1)));
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_6 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		if (!L_6)
		{
			goto IL_0041;
		}
	}
	{
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_7 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_8 = V_0;
		NullCheck((Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_7);
		bool L_9 = ((  bool (*) (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *, ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_7, (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		if (!L_9)
		{
			goto IL_0055;
		}
	}

IL_0041:
	{
		Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * L_10 = (Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)__this->get_selector_5();
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_11 = V_0;
		NullCheck((Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)L_10);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_12 = ((  Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  (*) (Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *, ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5)->methodPointer)((Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)L_10, (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_11, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this)->set_current_2(L_12);
		return (bool)1;
	}

IL_0055:
	{
		int32_t L_13 = (int32_t)__this->get_index_6();
		ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* L_14 = (ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36*)__this->get_source_3();
		NullCheck(L_14);
		if ((((int32_t)L_13) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_14)->max_length)))))))
		{
			goto IL_000b;
		}
	}
	{
		NullCheck((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<UnityEngine.Color>::Dispose() */, (Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this);
	}

IL_006b:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectArrayIterator_2_Where_m9EFE855D1A705A9BC15F03CFBEAC5F8ADD21E0CF_gshared (WhereSelectArrayIterator_2_tCB97637D799FCAD3115615E9FDA8E5CF80A108EC * __this, Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * L_0 = ___predicate0;
		WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 * L_1 = (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7));
		((  void (*) (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 *, RuntimeObject*, Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectArrayIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>::.ctor(TSource[],System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectArrayIterator_2__ctor_m7A38A363236B22C36C8C2BF4F63516E2D3848AF2_gshared (WhereSelectArrayIterator_2_t08DE4878AA443563B5940613DEBED682F6942F28 * __this, ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* ___source0, Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * ___predicate1, Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this);
		((  void (*) (Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF * WhereSelectArrayIterator_2_Clone_m99C93E3FC25CEA91CE3D7656CF4F2B9C7BB3F76F_gshared (WhereSelectArrayIterator_2_t08DE4878AA443563B5940613DEBED682F6942F28 * __this, const RuntimeMethod* method)
{
	{
		ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* L_0 = (ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36*)__this->get_source_3();
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_1 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * L_2 = (Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)__this->get_selector_5();
		WhereSelectArrayIterator_2_t08DE4878AA443563B5940613DEBED682F6942F28 * L_3 = (WhereSelectArrayIterator_2_t08DE4878AA443563B5940613DEBED682F6942F28 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectArrayIterator_2_t08DE4878AA443563B5940613DEBED682F6942F28 *, ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36*, Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *, Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36*)L_0, (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_1, (Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectArrayIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectArrayIterator_2_MoveNext_mFD0BB15AE81818ACF2C3DE167FEEC039C9954A60_gshared (WhereSelectArrayIterator_2_t08DE4878AA443563B5940613DEBED682F6942F28 * __this, const RuntimeMethod* method)
{
	ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		int32_t L_0 = (int32_t)((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this)->get_state_1();
		if ((!(((uint32_t)L_0) == ((uint32_t)1))))
		{
			goto IL_006b;
		}
	}
	{
		goto IL_0055;
	}

IL_000b:
	{
		ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* L_1 = (ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36*)__this->get_source_3();
		int32_t L_2 = (int32_t)__this->get_index_6();
		NullCheck(L_1);
		int32_t L_3 = L_2;
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		V_0 = (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_4;
		int32_t L_5 = (int32_t)__this->get_index_6();
		__this->set_index_6(((int32_t)il2cpp_codegen_add((int32_t)L_5, (int32_t)1)));
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_6 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		if (!L_6)
		{
			goto IL_0041;
		}
	}
	{
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_7 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_8 = V_0;
		NullCheck((Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_7);
		bool L_9 = ((  bool (*) (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *, ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_7, (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		if (!L_9)
		{
			goto IL_0055;
		}
	}

IL_0041:
	{
		Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * L_10 = (Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)__this->get_selector_5();
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_11 = V_0;
		NullCheck((Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)L_10);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_12 = ((  Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  (*) (Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *, ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5)->methodPointer)((Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)L_10, (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_11, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this)->set_current_2(L_12);
		return (bool)1;
	}

IL_0055:
	{
		int32_t L_13 = (int32_t)__this->get_index_6();
		ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36* L_14 = (ContourVertexU5BU5D_tD78F2EA3E732B8F5B5201FA2D894087C252E6F36*)__this->get_source_3();
		NullCheck(L_14);
		if ((((int32_t)L_13) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_14)->max_length)))))))
		{
			goto IL_000b;
		}
	}
	{
		NullCheck((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<UnityEngine.Vector3>::Dispose() */, (Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this);
	}

IL_006b:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectArrayIterator_2_Where_mD7CEB242A6561C7394DED1C4CFE2CA44DCC8ED5B_gshared (WhereSelectArrayIterator_2_t08DE4878AA443563B5940613DEBED682F6942F28 * __this, Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 * L_1 = (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7));
		((  void (*) (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 *, RuntimeObject*, Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectArrayIterator`2<Unity.Entities.Entity,System.Object>::.ctor(TSource[],System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectArrayIterator_2__ctor_m69E576135283EE83EB769829DDD99D9DB4902FE9_gshared (WhereSelectArrayIterator_2_t96F399D19669EB095BE4F3F6DFFF0695945DF634 * __this, EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5* ___source0, Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * ___predicate1, Func_2_t895537CD65D26801427B03E05DD08125DE819919 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_t895537CD65D26801427B03E05DD08125DE819919 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2<Unity.Entities.Entity,System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereSelectArrayIterator_2_Clone_mC62EA5D96CCE5B34439B214288CA2BD290BC31F1_gshared (WhereSelectArrayIterator_2_t96F399D19669EB095BE4F3F6DFFF0695945DF634 * __this, const RuntimeMethod* method)
{
	{
		EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5* L_0 = (EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5*)__this->get_source_3();
		Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * L_1 = (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)__this->get_predicate_4();
		Func_2_t895537CD65D26801427B03E05DD08125DE819919 * L_2 = (Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)__this->get_selector_5();
		WhereSelectArrayIterator_2_t96F399D19669EB095BE4F3F6DFFF0695945DF634 * L_3 = (WhereSelectArrayIterator_2_t96F399D19669EB095BE4F3F6DFFF0695945DF634 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectArrayIterator_2_t96F399D19669EB095BE4F3F6DFFF0695945DF634 *, EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5*, Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *, Func_2_t895537CD65D26801427B03E05DD08125DE819919 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5*)L_0, (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)L_1, (Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectArrayIterator`2<Unity.Entities.Entity,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectArrayIterator_2_MoveNext_mF636757AFB96D136A819B0C985DEC3F1387F1A65_gshared (WhereSelectArrayIterator_2_t96F399D19669EB095BE4F3F6DFFF0695945DF634 * __this, const RuntimeMethod* method)
{
	Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		if ((!(((uint32_t)L_0) == ((uint32_t)1))))
		{
			goto IL_006b;
		}
	}
	{
		goto IL_0055;
	}

IL_000b:
	{
		EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5* L_1 = (EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5*)__this->get_source_3();
		int32_t L_2 = (int32_t)__this->get_index_6();
		NullCheck(L_1);
		int32_t L_3 = L_2;
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		V_0 = (Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 )L_4;
		int32_t L_5 = (int32_t)__this->get_index_6();
		__this->set_index_6(((int32_t)il2cpp_codegen_add((int32_t)L_5, (int32_t)1)));
		Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * L_6 = (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)__this->get_predicate_4();
		if (!L_6)
		{
			goto IL_0041;
		}
	}
	{
		Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * L_7 = (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)__this->get_predicate_4();
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_8 = V_0;
		NullCheck((Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)L_7);
		bool L_9 = ((  bool (*) (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)L_7, (Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 )L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		if (!L_9)
		{
			goto IL_0055;
		}
	}

IL_0041:
	{
		Func_2_t895537CD65D26801427B03E05DD08125DE819919 * L_10 = (Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)__this->get_selector_5();
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_11 = V_0;
		NullCheck((Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)L_10);
		RuntimeObject * L_12 = ((  RuntimeObject * (*) (Func_2_t895537CD65D26801427B03E05DD08125DE819919 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5)->methodPointer)((Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)L_10, (Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 )L_11, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_12);
		return (bool)1;
	}

IL_0055:
	{
		int32_t L_13 = (int32_t)__this->get_index_6();
		EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5* L_14 = (EntityU5BU5D_t76151D0F32E2BC0C7F603B63B2D6C9DACADC34B5*)__this->get_source_3();
		NullCheck(L_14);
		if ((((int32_t)L_13) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_14)->max_length)))))))
		{
			goto IL_000b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_006b:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2<Unity.Entities.Entity,System.Object>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectArrayIterator_2_Where_m61BBFA1B50949E7C7BE5D612C1EFC94BE08CEE7F_gshared (WhereSelectArrayIterator_2_t96F399D19669EB095BE4F3F6DFFF0695945DF634 * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * L_1 = (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7));
		((  void (*) (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Int32,System.Int32>::.ctor(TSource[],System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectArrayIterator_2__ctor_mF52E446394C3F8DFB90C987A3244FCCC6BA600AD_gshared (WhereSelectArrayIterator_2_t00D4CC16A86C24F0CC80D3112E1FA2D451FF5D0C * __this, Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* ___source0, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate1, Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this);
		((  void (*) (Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Int32,System.Int32>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 * WhereSelectArrayIterator_2_Clone_m22ABB7CEF3FF8768C972AF44377BA8D2267B1F4F_gshared (WhereSelectArrayIterator_2_t00D4CC16A86C24F0CC80D3112E1FA2D451FF5D0C * __this, const RuntimeMethod* method)
{
	{
		Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* L_0 = (Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32*)__this->get_source_3();
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * L_2 = (Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)__this->get_selector_5();
		WhereSelectArrayIterator_2_t00D4CC16A86C24F0CC80D3112E1FA2D451FF5D0C * L_3 = (WhereSelectArrayIterator_2_t00D4CC16A86C24F0CC80D3112E1FA2D451FF5D0C *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectArrayIterator_2_t00D4CC16A86C24F0CC80D3112E1FA2D451FF5D0C *, Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32*, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32*)L_0, (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_1, (Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Int32,System.Int32>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectArrayIterator_2_MoveNext_mE90B9B8E6833394962A9373FB647D6361B3B6796_gshared (WhereSelectArrayIterator_2_t00D4CC16A86C24F0CC80D3112E1FA2D451FF5D0C * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		int32_t L_0 = (int32_t)((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this)->get_state_1();
		if ((!(((uint32_t)L_0) == ((uint32_t)1))))
		{
			goto IL_006b;
		}
	}
	{
		goto IL_0055;
	}

IL_000b:
	{
		Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* L_1 = (Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32*)__this->get_source_3();
		int32_t L_2 = (int32_t)__this->get_index_6();
		NullCheck(L_1);
		int32_t L_3 = L_2;
		int32_t L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		V_0 = (int32_t)L_4;
		int32_t L_5 = (int32_t)__this->get_index_6();
		__this->set_index_6(((int32_t)il2cpp_codegen_add((int32_t)L_5, (int32_t)1)));
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_6 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		if (!L_6)
		{
			goto IL_0041;
		}
	}
	{
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_7 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		int32_t L_8 = V_0;
		NullCheck((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_7);
		bool L_9 = ((  bool (*) (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_7, (int32_t)L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		if (!L_9)
		{
			goto IL_0055;
		}
	}

IL_0041:
	{
		Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * L_10 = (Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)__this->get_selector_5();
		int32_t L_11 = V_0;
		NullCheck((Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)L_10);
		int32_t L_12 = ((  int32_t (*) (Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5)->methodPointer)((Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)L_10, (int32_t)L_11, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this)->set_current_2(L_12);
		return (bool)1;
	}

IL_0055:
	{
		int32_t L_13 = (int32_t)__this->get_index_6();
		Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* L_14 = (Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32*)__this->get_source_3();
		NullCheck(L_14);
		if ((((int32_t)L_13) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_14)->max_length)))))))
		{
			goto IL_000b;
		}
	}
	{
		NullCheck((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Int32>::Dispose() */, (Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this);
	}

IL_006b:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Int32,System.Int32>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectArrayIterator_2_Where_m51C6FAFCE81BD511E633CBBEC8E2A29EF144F1D7_gshared (WhereSelectArrayIterator_2_t00D4CC16A86C24F0CC80D3112E1FA2D451FF5D0C * __this, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA * L_1 = (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7));
		((  void (*) (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA *, RuntimeObject*, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Int32,System.Object>::.ctor(TSource[],System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectArrayIterator_2__ctor_m42B6FDAEDB2D48D1090A3E06A3D7F4339CA7DD4D_gshared (WhereSelectArrayIterator_2_t6AF7279540EEA250525A2AFBDFAC2064A9B5C00B * __this, Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* ___source0, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate1, Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Int32,System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereSelectArrayIterator_2_Clone_mA5824D44B54C8AF514D5683EDE0B1FC5834A69A9_gshared (WhereSelectArrayIterator_2_t6AF7279540EEA250525A2AFBDFAC2064A9B5C00B * __this, const RuntimeMethod* method)
{
	{
		Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* L_0 = (Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32*)__this->get_source_3();
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * L_2 = (Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)__this->get_selector_5();
		WhereSelectArrayIterator_2_t6AF7279540EEA250525A2AFBDFAC2064A9B5C00B * L_3 = (WhereSelectArrayIterator_2_t6AF7279540EEA250525A2AFBDFAC2064A9B5C00B *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectArrayIterator_2_t6AF7279540EEA250525A2AFBDFAC2064A9B5C00B *, Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32*, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32*)L_0, (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_1, (Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Int32,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectArrayIterator_2_MoveNext_m880029BC140EC15B1231301739BF3F74D63E452B_gshared (WhereSelectArrayIterator_2_t6AF7279540EEA250525A2AFBDFAC2064A9B5C00B * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		if ((!(((uint32_t)L_0) == ((uint32_t)1))))
		{
			goto IL_006b;
		}
	}
	{
		goto IL_0055;
	}

IL_000b:
	{
		Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* L_1 = (Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32*)__this->get_source_3();
		int32_t L_2 = (int32_t)__this->get_index_6();
		NullCheck(L_1);
		int32_t L_3 = L_2;
		int32_t L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		V_0 = (int32_t)L_4;
		int32_t L_5 = (int32_t)__this->get_index_6();
		__this->set_index_6(((int32_t)il2cpp_codegen_add((int32_t)L_5, (int32_t)1)));
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_6 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		if (!L_6)
		{
			goto IL_0041;
		}
	}
	{
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_7 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		int32_t L_8 = V_0;
		NullCheck((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_7);
		bool L_9 = ((  bool (*) (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_7, (int32_t)L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		if (!L_9)
		{
			goto IL_0055;
		}
	}

IL_0041:
	{
		Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * L_10 = (Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)__this->get_selector_5();
		int32_t L_11 = V_0;
		NullCheck((Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)L_10);
		RuntimeObject * L_12 = ((  RuntimeObject * (*) (Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5)->methodPointer)((Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)L_10, (int32_t)L_11, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_12);
		return (bool)1;
	}

IL_0055:
	{
		int32_t L_13 = (int32_t)__this->get_index_6();
		Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* L_14 = (Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32*)__this->get_source_3();
		NullCheck(L_14);
		if ((((int32_t)L_13) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_14)->max_length)))))))
		{
			goto IL_000b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_006b:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Int32,System.Object>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectArrayIterator_2_Where_m419FD3EEA73CF3A41AA87E1EEB86DC0206657EAE_gshared (WhereSelectArrayIterator_2_t6AF7279540EEA250525A2AFBDFAC2064A9B5C00B * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * L_1 = (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7));
		((  void (*) (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectArrayIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>::.ctor(TSource[],System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectArrayIterator_2__ctor_mC5D1F0B12C9BD9670EDAEBF1B0A62E40089D7EF4_gshared (WhereSelectArrayIterator_2_tFC05154F4D80444AACE36E75937E5D42B87B3384 * __this, LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC* ___source0, Func_2_t15BD356B2F637699370FD7109071A37617770BBA * ___predicate1, Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t15BD356B2F637699370FD7109071A37617770BBA * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereSelectArrayIterator_2_Clone_m28C06E575A657415217E871DEB132BBE6D0259EA_gshared (WhereSelectArrayIterator_2_tFC05154F4D80444AACE36E75937E5D42B87B3384 * __this, const RuntimeMethod* method)
{
	{
		LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC* L_0 = (LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC*)__this->get_source_3();
		Func_2_t15BD356B2F637699370FD7109071A37617770BBA * L_1 = (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)__this->get_predicate_4();
		Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * L_2 = (Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)__this->get_selector_5();
		WhereSelectArrayIterator_2_tFC05154F4D80444AACE36E75937E5D42B87B3384 * L_3 = (WhereSelectArrayIterator_2_tFC05154F4D80444AACE36E75937E5D42B87B3384 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectArrayIterator_2_tFC05154F4D80444AACE36E75937E5D42B87B3384 *, LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC*, Func_2_t15BD356B2F637699370FD7109071A37617770BBA *, Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC*)L_0, (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)L_1, (Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectArrayIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectArrayIterator_2_MoveNext_m015F6C1AF3122651F5592E56770D254EB9382C9A_gshared (WhereSelectArrayIterator_2_tFC05154F4D80444AACE36E75937E5D42B87B3384 * __this, const RuntimeMethod* method)
{
	LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		if ((!(((uint32_t)L_0) == ((uint32_t)1))))
		{
			goto IL_006b;
		}
	}
	{
		goto IL_0055;
	}

IL_000b:
	{
		LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC* L_1 = (LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC*)__this->get_source_3();
		int32_t L_2 = (int32_t)__this->get_index_6();
		NullCheck(L_1);
		int32_t L_3 = L_2;
		LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		V_0 = (LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 )L_4;
		int32_t L_5 = (int32_t)__this->get_index_6();
		__this->set_index_6(((int32_t)il2cpp_codegen_add((int32_t)L_5, (int32_t)1)));
		Func_2_t15BD356B2F637699370FD7109071A37617770BBA * L_6 = (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)__this->get_predicate_4();
		if (!L_6)
		{
			goto IL_0041;
		}
	}
	{
		Func_2_t15BD356B2F637699370FD7109071A37617770BBA * L_7 = (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)__this->get_predicate_4();
		LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  L_8 = V_0;
		NullCheck((Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)L_7);
		bool L_9 = ((  bool (*) (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *, LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)L_7, (LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 )L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		if (!L_9)
		{
			goto IL_0055;
		}
	}

IL_0041:
	{
		Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * L_10 = (Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)__this->get_selector_5();
		LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  L_11 = V_0;
		NullCheck((Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)L_10);
		RuntimeObject * L_12 = ((  RuntimeObject * (*) (Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *, LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5)->methodPointer)((Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)L_10, (LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 )L_11, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_12);
		return (bool)1;
	}

IL_0055:
	{
		int32_t L_13 = (int32_t)__this->get_index_6();
		LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC* L_14 = (LogEventDataU5BU5D_t297A076CEB7438F3BBF18FEC1A58490451063CCC*)__this->get_source_3();
		NullCheck(L_14);
		if ((((int32_t)L_13) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_14)->max_length)))))))
		{
			goto IL_000b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_006b:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectArrayIterator_2_Where_m41231E5DAE85866995C4BE28A705D417B4994F0F_gshared (WhereSelectArrayIterator_2_tFC05154F4D80444AACE36E75937E5D42B87B3384 * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * L_1 = (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7));
		((  void (*) (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Object,System.Object>::.ctor(TSource[],System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectArrayIterator_2__ctor_mCB64A76E8C03C791C57D44E38D025D1C6EC7B880_gshared (WhereSelectArrayIterator_2_tA706D5B1608A9A8F1BF43C6E5D9D682C901DB244 * __this, ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* ___source0, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate1, Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Object,System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereSelectArrayIterator_2_Clone_m4ACA936AD86CEAB027D82949C74DEDD882A800D3_gshared (WhereSelectArrayIterator_2_tA706D5B1608A9A8F1BF43C6E5D9D682C901DB244 * __this, const RuntimeMethod* method)
{
	{
		ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* L_0 = (ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*)__this->get_source_3();
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * L_2 = (Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)__this->get_selector_5();
		WhereSelectArrayIterator_2_tA706D5B1608A9A8F1BF43C6E5D9D682C901DB244 * L_3 = (WhereSelectArrayIterator_2_tA706D5B1608A9A8F1BF43C6E5D9D682C901DB244 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectArrayIterator_2_tA706D5B1608A9A8F1BF43C6E5D9D682C901DB244 *, ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*)L_0, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_1, (Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Object,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectArrayIterator_2_MoveNext_mCF08A119CF0CC000264B5B6BA5EC4B40CC9640CC_gshared (WhereSelectArrayIterator_2_tA706D5B1608A9A8F1BF43C6E5D9D682C901DB244 * __this, const RuntimeMethod* method)
{
	RuntimeObject * V_0 = NULL;
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		if ((!(((uint32_t)L_0) == ((uint32_t)1))))
		{
			goto IL_006b;
		}
	}
	{
		goto IL_0055;
	}

IL_000b:
	{
		ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* L_1 = (ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*)__this->get_source_3();
		int32_t L_2 = (int32_t)__this->get_index_6();
		NullCheck(L_1);
		int32_t L_3 = L_2;
		RuntimeObject * L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		V_0 = (RuntimeObject *)L_4;
		int32_t L_5 = (int32_t)__this->get_index_6();
		__this->set_index_6(((int32_t)il2cpp_codegen_add((int32_t)L_5, (int32_t)1)));
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_6 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		if (!L_6)
		{
			goto IL_0041;
		}
	}
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_7 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		RuntimeObject * L_8 = V_0;
		NullCheck((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_7);
		bool L_9 = ((  bool (*) (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_7, (RuntimeObject *)L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		if (!L_9)
		{
			goto IL_0055;
		}
	}

IL_0041:
	{
		Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * L_10 = (Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)__this->get_selector_5();
		RuntimeObject * L_11 = V_0;
		NullCheck((Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)L_10);
		RuntimeObject * L_12 = ((  RuntimeObject * (*) (Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5)->methodPointer)((Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)L_10, (RuntimeObject *)L_11, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_12);
		return (bool)1;
	}

IL_0055:
	{
		int32_t L_13 = (int32_t)__this->get_index_6();
		ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE* L_14 = (ObjectU5BU5D_tC1F4EE0DB0B7300255F5FD4AF64FE4C585CF5ADE*)__this->get_source_3();
		NullCheck(L_14);
		if ((((int32_t)L_13) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_14)->max_length)))))))
		{
			goto IL_000b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_006b:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectArrayIterator`2<System.Object,System.Object>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectArrayIterator_2_Where_m21B504E9811B348A8694F7C71E07ABCCDE69807B_gshared (WhereSelectArrayIterator_2_tA706D5B1608A9A8F1BF43C6E5D9D682C901DB244 * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * L_1 = (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 7));
		((  void (*) (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectEnumerableIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>::.ctor(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectEnumerableIterator_2__ctor_mA3721355CFE21C93FF4EBB3F0CF5456EEFA5EA19_gshared (WhereSelectEnumerableIterator_2_t082E1B5B13EA24679459FB2484F9A187BB83D681 * __this, RuntimeObject* ___source0, Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * ___predicate1, Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this);
		((  void (*) (Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		RuntimeObject* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 * WhereSelectEnumerableIterator_2_Clone_mE3EF354F45F18782587CF61E2DD555E539E59790_gshared (WhereSelectEnumerableIterator_2_t082E1B5B13EA24679459FB2484F9A187BB83D681 * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_1 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * L_2 = (Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)__this->get_selector_5();
		WhereSelectEnumerableIterator_2_t082E1B5B13EA24679459FB2484F9A187BB83D681 * L_3 = (WhereSelectEnumerableIterator_2_t082E1B5B13EA24679459FB2484F9A187BB83D681 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectEnumerableIterator_2_t082E1B5B13EA24679459FB2484F9A187BB83D681 *, RuntimeObject*, Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *, Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (RuntimeObject*)L_0, (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_1, (Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Void System.Linq.Enumerable_WhereSelectEnumerableIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectEnumerableIterator_2_Dispose_mB8D564C5BE6B9B60555B5D5A980D8F0FFBA8EE96_gshared (WhereSelectEnumerableIterator_2_t082E1B5B13EA24679459FB2484F9A187BB83D681 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereSelectEnumerableIterator_2_Dispose_mB8D564C5BE6B9B60555B5D5A980D8F0FFBA8EE96_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_enumerator_6();
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		RuntimeObject* L_1 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_1);
		InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t099785737FC6A1E3699919A94109383715A8D807_il2cpp_TypeInfo_var, (RuntimeObject*)L_1);
	}

IL_0013:
	{
		__this->set_enumerator_6((RuntimeObject*)NULL);
		NullCheck((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this);
		((  void (*) (Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		return;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectEnumerableIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectEnumerableIterator_2_MoveNext_mE31162ED57F8EA0F26BCF79C89FC7D074BAED8E8_gshared (WhereSelectEnumerableIterator_2_t082E1B5B13EA24679459FB2484F9A187BB83D681 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereSelectEnumerableIterator_2_MoveNext_mE31162ED57F8EA0F26BCF79C89FC7D074BAED8E8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		int32_t L_0 = (int32_t)((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0061;
		}
	}
	{
		goto IL_0074;
	}

IL_0011:
	{
		RuntimeObject* L_3 = (RuntimeObject*)__this->get_source_3();
		NullCheck((RuntimeObject*)L_3);
		RuntimeObject* L_4 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<!0> System.Collections.Generic.IEnumerable`1<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex>::GetEnumerator() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 5), (RuntimeObject*)L_3);
		__this->set_enumerator_6(L_4);
		((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this)->set_state_1(2);
		goto IL_0061;
	}

IL_002b:
	{
		RuntimeObject* L_5 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_5);
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_6 = InterfaceFuncInvoker0< ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  >::Invoke(0 /* !0 System.Collections.Generic.IEnumerator`1<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex>::get_Current() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 6), (RuntimeObject*)L_5);
		V_1 = (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_6;
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_7 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		if (!L_7)
		{
			goto IL_004d;
		}
	}
	{
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_8 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_9 = V_1;
		NullCheck((Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_8);
		bool L_10 = ((  bool (*) (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *, ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_8, (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		if (!L_10)
		{
			goto IL_0061;
		}
	}

IL_004d:
	{
		Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * L_11 = (Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)__this->get_selector_5();
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_12 = V_1;
		NullCheck((Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)L_11);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_13 = ((  Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  (*) (Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *, ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)L_11, (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this)->set_current_2(L_13);
		return (bool)1;
	}

IL_0061:
	{
		RuntimeObject* L_14 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_14);
		bool L_15 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t5956F3AFB7ECF1117E3BC5890E7FC7B7F7A04105_il2cpp_TypeInfo_var, (RuntimeObject*)L_14);
		if (L_15)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<UnityEngine.Color>::Dispose() */, (Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this);
	}

IL_0074:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectEnumerableIterator_2_Where_m900325C94A45FD735E79A83A8BFEFB12E97B3E70_gshared (WhereSelectEnumerableIterator_2_t082E1B5B13EA24679459FB2484F9A187BB83D681 * __this, Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * L_0 = ___predicate0;
		WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 * L_1 = (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 9));
		((  void (*) (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 *, RuntimeObject*, Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectEnumerableIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>::.ctor(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectEnumerableIterator_2__ctor_m61143B1918818C38D1D533443349245650AE074D_gshared (WhereSelectEnumerableIterator_2_t3B7C9E548F0B01F9C6D09D71FCA8E5ACDE0F3FCD * __this, RuntimeObject* ___source0, Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * ___predicate1, Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this);
		((  void (*) (Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		RuntimeObject* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF * WhereSelectEnumerableIterator_2_Clone_mA53E13C8384AB0743C1A233E1BFE3DDB7C97B9A6_gshared (WhereSelectEnumerableIterator_2_t3B7C9E548F0B01F9C6D09D71FCA8E5ACDE0F3FCD * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_1 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * L_2 = (Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)__this->get_selector_5();
		WhereSelectEnumerableIterator_2_t3B7C9E548F0B01F9C6D09D71FCA8E5ACDE0F3FCD * L_3 = (WhereSelectEnumerableIterator_2_t3B7C9E548F0B01F9C6D09D71FCA8E5ACDE0F3FCD *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectEnumerableIterator_2_t3B7C9E548F0B01F9C6D09D71FCA8E5ACDE0F3FCD *, RuntimeObject*, Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *, Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (RuntimeObject*)L_0, (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_1, (Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Void System.Linq.Enumerable_WhereSelectEnumerableIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectEnumerableIterator_2_Dispose_m1F32BB3D970382EE067CF21C4DA2C95EBB1AA3BE_gshared (WhereSelectEnumerableIterator_2_t3B7C9E548F0B01F9C6D09D71FCA8E5ACDE0F3FCD * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereSelectEnumerableIterator_2_Dispose_m1F32BB3D970382EE067CF21C4DA2C95EBB1AA3BE_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_enumerator_6();
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		RuntimeObject* L_1 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_1);
		InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t099785737FC6A1E3699919A94109383715A8D807_il2cpp_TypeInfo_var, (RuntimeObject*)L_1);
	}

IL_0013:
	{
		__this->set_enumerator_6((RuntimeObject*)NULL);
		NullCheck((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this);
		((  void (*) (Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		return;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectEnumerableIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectEnumerableIterator_2_MoveNext_m4C5AF35515D4FD9CB3C05F9229032D97752B6EE3_gshared (WhereSelectEnumerableIterator_2_t3B7C9E548F0B01F9C6D09D71FCA8E5ACDE0F3FCD * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereSelectEnumerableIterator_2_MoveNext_m4C5AF35515D4FD9CB3C05F9229032D97752B6EE3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		int32_t L_0 = (int32_t)((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0061;
		}
	}
	{
		goto IL_0074;
	}

IL_0011:
	{
		RuntimeObject* L_3 = (RuntimeObject*)__this->get_source_3();
		NullCheck((RuntimeObject*)L_3);
		RuntimeObject* L_4 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<!0> System.Collections.Generic.IEnumerable`1<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex>::GetEnumerator() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 5), (RuntimeObject*)L_3);
		__this->set_enumerator_6(L_4);
		((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this)->set_state_1(2);
		goto IL_0061;
	}

IL_002b:
	{
		RuntimeObject* L_5 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_5);
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_6 = InterfaceFuncInvoker0< ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  >::Invoke(0 /* !0 System.Collections.Generic.IEnumerator`1<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex>::get_Current() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 6), (RuntimeObject*)L_5);
		V_1 = (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_6;
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_7 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		if (!L_7)
		{
			goto IL_004d;
		}
	}
	{
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_8 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_9 = V_1;
		NullCheck((Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_8);
		bool L_10 = ((  bool (*) (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *, ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_8, (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		if (!L_10)
		{
			goto IL_0061;
		}
	}

IL_004d:
	{
		Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * L_11 = (Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)__this->get_selector_5();
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_12 = V_1;
		NullCheck((Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)L_11);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_13 = ((  Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  (*) (Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *, ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)L_11, (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this)->set_current_2(L_13);
		return (bool)1;
	}

IL_0061:
	{
		RuntimeObject* L_14 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_14);
		bool L_15 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t5956F3AFB7ECF1117E3BC5890E7FC7B7F7A04105_il2cpp_TypeInfo_var, (RuntimeObject*)L_14);
		if (L_15)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<UnityEngine.Vector3>::Dispose() */, (Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this);
	}

IL_0074:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectEnumerableIterator_2_Where_mBE95C93FF495925BFF0FBD9F83F0EBEA2A6BD00D_gshared (WhereSelectEnumerableIterator_2_t3B7C9E548F0B01F9C6D09D71FCA8E5ACDE0F3FCD * __this, Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 * L_1 = (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 9));
		((  void (*) (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 *, RuntimeObject*, Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectEnumerableIterator`2<Unity.Entities.Entity,System.Object>::.ctor(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectEnumerableIterator_2__ctor_m8DD3449D400DE60BE0415438D5213D18CA80B030_gshared (WhereSelectEnumerableIterator_2_t51ACFE0E1ABAB81E89F3A922089D2AC4ACED9718 * __this, RuntimeObject* ___source0, Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * ___predicate1, Func_2_t895537CD65D26801427B03E05DD08125DE819919 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		RuntimeObject* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_t895537CD65D26801427B03E05DD08125DE819919 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2<Unity.Entities.Entity,System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereSelectEnumerableIterator_2_Clone_m34BFA527BFB46B3A1F051179C4A141AEEBCD14B3_gshared (WhereSelectEnumerableIterator_2_t51ACFE0E1ABAB81E89F3A922089D2AC4ACED9718 * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * L_1 = (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)__this->get_predicate_4();
		Func_2_t895537CD65D26801427B03E05DD08125DE819919 * L_2 = (Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)__this->get_selector_5();
		WhereSelectEnumerableIterator_2_t51ACFE0E1ABAB81E89F3A922089D2AC4ACED9718 * L_3 = (WhereSelectEnumerableIterator_2_t51ACFE0E1ABAB81E89F3A922089D2AC4ACED9718 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectEnumerableIterator_2_t51ACFE0E1ABAB81E89F3A922089D2AC4ACED9718 *, RuntimeObject*, Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *, Func_2_t895537CD65D26801427B03E05DD08125DE819919 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (RuntimeObject*)L_0, (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)L_1, (Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Void System.Linq.Enumerable_WhereSelectEnumerableIterator`2<Unity.Entities.Entity,System.Object>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectEnumerableIterator_2_Dispose_m7DDE555349A7D0B9DEFB7EB866B728A7D40E4BD9_gshared (WhereSelectEnumerableIterator_2_t51ACFE0E1ABAB81E89F3A922089D2AC4ACED9718 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereSelectEnumerableIterator_2_Dispose_m7DDE555349A7D0B9DEFB7EB866B728A7D40E4BD9_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_enumerator_6();
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		RuntimeObject* L_1 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_1);
		InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t099785737FC6A1E3699919A94109383715A8D807_il2cpp_TypeInfo_var, (RuntimeObject*)L_1);
	}

IL_0013:
	{
		__this->set_enumerator_6((RuntimeObject*)NULL);
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		return;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectEnumerableIterator`2<Unity.Entities.Entity,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectEnumerableIterator_2_MoveNext_m6214F2D8E14596AF5302702554D6090B95C91977_gshared (WhereSelectEnumerableIterator_2_t51ACFE0E1ABAB81E89F3A922089D2AC4ACED9718 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereSelectEnumerableIterator_2_MoveNext_m6214F2D8E14596AF5302702554D6090B95C91977_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0061;
		}
	}
	{
		goto IL_0074;
	}

IL_0011:
	{
		RuntimeObject* L_3 = (RuntimeObject*)__this->get_source_3();
		NullCheck((RuntimeObject*)L_3);
		RuntimeObject* L_4 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<!0> System.Collections.Generic.IEnumerable`1<Unity.Entities.Entity>::GetEnumerator() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 5), (RuntimeObject*)L_3);
		__this->set_enumerator_6(L_4);
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_state_1(2);
		goto IL_0061;
	}

IL_002b:
	{
		RuntimeObject* L_5 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_5);
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_6 = InterfaceFuncInvoker0< Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  >::Invoke(0 /* !0 System.Collections.Generic.IEnumerator`1<Unity.Entities.Entity>::get_Current() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 6), (RuntimeObject*)L_5);
		V_1 = (Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 )L_6;
		Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * L_7 = (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)__this->get_predicate_4();
		if (!L_7)
		{
			goto IL_004d;
		}
	}
	{
		Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * L_8 = (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)__this->get_predicate_4();
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_9 = V_1;
		NullCheck((Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)L_8);
		bool L_10 = ((  bool (*) (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)L_8, (Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 )L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		if (!L_10)
		{
			goto IL_0061;
		}
	}

IL_004d:
	{
		Func_2_t895537CD65D26801427B03E05DD08125DE819919 * L_11 = (Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)__this->get_selector_5();
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_12 = V_1;
		NullCheck((Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)L_11);
		RuntimeObject * L_13 = ((  RuntimeObject * (*) (Func_2_t895537CD65D26801427B03E05DD08125DE819919 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)L_11, (Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 )L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_13);
		return (bool)1;
	}

IL_0061:
	{
		RuntimeObject* L_14 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_14);
		bool L_15 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t5956F3AFB7ECF1117E3BC5890E7FC7B7F7A04105_il2cpp_TypeInfo_var, (RuntimeObject*)L_14);
		if (L_15)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_0074:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2<Unity.Entities.Entity,System.Object>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectEnumerableIterator_2_Where_m647E003234B23706D0EDAFE36E3FF59A7F48FCAE_gshared (WhereSelectEnumerableIterator_2_t51ACFE0E1ABAB81E89F3A922089D2AC4ACED9718 * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * L_1 = (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 9));
		((  void (*) (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Int32,System.Int32>::.ctor(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectEnumerableIterator_2__ctor_m24BFAF64B8AEF6BAC15970E8CE4F9192236CCC11_gshared (WhereSelectEnumerableIterator_2_t70C236BD8C0DBBA43FCE3CE4E4F6B545579333EF * __this, RuntimeObject* ___source0, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate1, Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this);
		((  void (*) (Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		RuntimeObject* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Int32,System.Int32>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 * WhereSelectEnumerableIterator_2_Clone_m48529E6B525DC0A3DE4C3C65EC2F0A1D24A4E263_gshared (WhereSelectEnumerableIterator_2_t70C236BD8C0DBBA43FCE3CE4E4F6B545579333EF * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * L_2 = (Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)__this->get_selector_5();
		WhereSelectEnumerableIterator_2_t70C236BD8C0DBBA43FCE3CE4E4F6B545579333EF * L_3 = (WhereSelectEnumerableIterator_2_t70C236BD8C0DBBA43FCE3CE4E4F6B545579333EF *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectEnumerableIterator_2_t70C236BD8C0DBBA43FCE3CE4E4F6B545579333EF *, RuntimeObject*, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (RuntimeObject*)L_0, (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_1, (Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Void System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Int32,System.Int32>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectEnumerableIterator_2_Dispose_mB529F42305C163995535C0696422D8410CB02BB3_gshared (WhereSelectEnumerableIterator_2_t70C236BD8C0DBBA43FCE3CE4E4F6B545579333EF * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereSelectEnumerableIterator_2_Dispose_mB529F42305C163995535C0696422D8410CB02BB3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_enumerator_6();
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		RuntimeObject* L_1 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_1);
		InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t099785737FC6A1E3699919A94109383715A8D807_il2cpp_TypeInfo_var, (RuntimeObject*)L_1);
	}

IL_0013:
	{
		__this->set_enumerator_6((RuntimeObject*)NULL);
		NullCheck((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this);
		((  void (*) (Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		return;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Int32,System.Int32>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectEnumerableIterator_2_MoveNext_mC11B40068E87BD32531EC0895238F0EF85DDB398_gshared (WhereSelectEnumerableIterator_2_t70C236BD8C0DBBA43FCE3CE4E4F6B545579333EF * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereSelectEnumerableIterator_2_MoveNext_mC11B40068E87BD32531EC0895238F0EF85DDB398_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		int32_t L_0 = (int32_t)((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0061;
		}
	}
	{
		goto IL_0074;
	}

IL_0011:
	{
		RuntimeObject* L_3 = (RuntimeObject*)__this->get_source_3();
		NullCheck((RuntimeObject*)L_3);
		RuntimeObject* L_4 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<!0> System.Collections.Generic.IEnumerable`1<System.Int32>::GetEnumerator() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 5), (RuntimeObject*)L_3);
		__this->set_enumerator_6(L_4);
		((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this)->set_state_1(2);
		goto IL_0061;
	}

IL_002b:
	{
		RuntimeObject* L_5 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_5);
		int32_t L_6 = InterfaceFuncInvoker0< int32_t >::Invoke(0 /* !0 System.Collections.Generic.IEnumerator`1<System.Int32>::get_Current() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 6), (RuntimeObject*)L_5);
		V_1 = (int32_t)L_6;
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_7 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		if (!L_7)
		{
			goto IL_004d;
		}
	}
	{
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_8 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		int32_t L_9 = V_1;
		NullCheck((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_8);
		bool L_10 = ((  bool (*) (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_8, (int32_t)L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		if (!L_10)
		{
			goto IL_0061;
		}
	}

IL_004d:
	{
		Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * L_11 = (Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)__this->get_selector_5();
		int32_t L_12 = V_1;
		NullCheck((Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)L_11);
		int32_t L_13 = ((  int32_t (*) (Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)L_11, (int32_t)L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this)->set_current_2(L_13);
		return (bool)1;
	}

IL_0061:
	{
		RuntimeObject* L_14 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_14);
		bool L_15 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t5956F3AFB7ECF1117E3BC5890E7FC7B7F7A04105_il2cpp_TypeInfo_var, (RuntimeObject*)L_14);
		if (L_15)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Int32>::Dispose() */, (Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this);
	}

IL_0074:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Int32,System.Int32>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectEnumerableIterator_2_Where_m6B10C66012139D931A3FAC2CF5B589B789C2ABF4_gshared (WhereSelectEnumerableIterator_2_t70C236BD8C0DBBA43FCE3CE4E4F6B545579333EF * __this, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA * L_1 = (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 9));
		((  void (*) (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA *, RuntimeObject*, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Int32,System.Object>::.ctor(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectEnumerableIterator_2__ctor_m2C6BF870C887594C92C96638C777A6F8D363FBE6_gshared (WhereSelectEnumerableIterator_2_tC12C4897F0443B7CCF4B7D0E720F91F10278FC0E * __this, RuntimeObject* ___source0, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate1, Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		RuntimeObject* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Int32,System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereSelectEnumerableIterator_2_Clone_mFAB909F2F26271F2629A7E2424FC23F8BAF054B6_gshared (WhereSelectEnumerableIterator_2_tC12C4897F0443B7CCF4B7D0E720F91F10278FC0E * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * L_2 = (Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)__this->get_selector_5();
		WhereSelectEnumerableIterator_2_tC12C4897F0443B7CCF4B7D0E720F91F10278FC0E * L_3 = (WhereSelectEnumerableIterator_2_tC12C4897F0443B7CCF4B7D0E720F91F10278FC0E *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectEnumerableIterator_2_tC12C4897F0443B7CCF4B7D0E720F91F10278FC0E *, RuntimeObject*, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (RuntimeObject*)L_0, (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_1, (Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Void System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Int32,System.Object>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectEnumerableIterator_2_Dispose_m82C0D4A9E151A1DAC0C017CA403BAB5CDED9CFD5_gshared (WhereSelectEnumerableIterator_2_tC12C4897F0443B7CCF4B7D0E720F91F10278FC0E * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereSelectEnumerableIterator_2_Dispose_m82C0D4A9E151A1DAC0C017CA403BAB5CDED9CFD5_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_enumerator_6();
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		RuntimeObject* L_1 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_1);
		InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t099785737FC6A1E3699919A94109383715A8D807_il2cpp_TypeInfo_var, (RuntimeObject*)L_1);
	}

IL_0013:
	{
		__this->set_enumerator_6((RuntimeObject*)NULL);
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		return;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Int32,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectEnumerableIterator_2_MoveNext_m7422C90C467A2D0EF7E7D644EDB241F378A7AECD_gshared (WhereSelectEnumerableIterator_2_tC12C4897F0443B7CCF4B7D0E720F91F10278FC0E * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereSelectEnumerableIterator_2_MoveNext_m7422C90C467A2D0EF7E7D644EDB241F378A7AECD_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0061;
		}
	}
	{
		goto IL_0074;
	}

IL_0011:
	{
		RuntimeObject* L_3 = (RuntimeObject*)__this->get_source_3();
		NullCheck((RuntimeObject*)L_3);
		RuntimeObject* L_4 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<!0> System.Collections.Generic.IEnumerable`1<System.Int32>::GetEnumerator() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 5), (RuntimeObject*)L_3);
		__this->set_enumerator_6(L_4);
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_state_1(2);
		goto IL_0061;
	}

IL_002b:
	{
		RuntimeObject* L_5 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_5);
		int32_t L_6 = InterfaceFuncInvoker0< int32_t >::Invoke(0 /* !0 System.Collections.Generic.IEnumerator`1<System.Int32>::get_Current() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 6), (RuntimeObject*)L_5);
		V_1 = (int32_t)L_6;
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_7 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		if (!L_7)
		{
			goto IL_004d;
		}
	}
	{
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_8 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		int32_t L_9 = V_1;
		NullCheck((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_8);
		bool L_10 = ((  bool (*) (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_8, (int32_t)L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		if (!L_10)
		{
			goto IL_0061;
		}
	}

IL_004d:
	{
		Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * L_11 = (Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)__this->get_selector_5();
		int32_t L_12 = V_1;
		NullCheck((Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)L_11);
		RuntimeObject * L_13 = ((  RuntimeObject * (*) (Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)L_11, (int32_t)L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_13);
		return (bool)1;
	}

IL_0061:
	{
		RuntimeObject* L_14 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_14);
		bool L_15 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t5956F3AFB7ECF1117E3BC5890E7FC7B7F7A04105_il2cpp_TypeInfo_var, (RuntimeObject*)L_14);
		if (L_15)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_0074:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Int32,System.Object>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectEnumerableIterator_2_Where_m4A570F856A4C2CF183E921AA35177C326F8BAF1C_gshared (WhereSelectEnumerableIterator_2_tC12C4897F0443B7CCF4B7D0E720F91F10278FC0E * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * L_1 = (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 9));
		((  void (*) (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectEnumerableIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>::.ctor(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectEnumerableIterator_2__ctor_mC487BE835625ACD3EF34E0D0072B680211EDA890_gshared (WhereSelectEnumerableIterator_2_t575305DD8907FFFDC568246A6635414D5B81B807 * __this, RuntimeObject* ___source0, Func_2_t15BD356B2F637699370FD7109071A37617770BBA * ___predicate1, Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		RuntimeObject* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t15BD356B2F637699370FD7109071A37617770BBA * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereSelectEnumerableIterator_2_Clone_m1A7971B2D548FC841364EB4743344BA2AC4F1D60_gshared (WhereSelectEnumerableIterator_2_t575305DD8907FFFDC568246A6635414D5B81B807 * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_t15BD356B2F637699370FD7109071A37617770BBA * L_1 = (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)__this->get_predicate_4();
		Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * L_2 = (Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)__this->get_selector_5();
		WhereSelectEnumerableIterator_2_t575305DD8907FFFDC568246A6635414D5B81B807 * L_3 = (WhereSelectEnumerableIterator_2_t575305DD8907FFFDC568246A6635414D5B81B807 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectEnumerableIterator_2_t575305DD8907FFFDC568246A6635414D5B81B807 *, RuntimeObject*, Func_2_t15BD356B2F637699370FD7109071A37617770BBA *, Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (RuntimeObject*)L_0, (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)L_1, (Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Void System.Linq.Enumerable_WhereSelectEnumerableIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectEnumerableIterator_2_Dispose_m76E5F3DD7C637EC24BC0B6962F3852B5E4C9DCBB_gshared (WhereSelectEnumerableIterator_2_t575305DD8907FFFDC568246A6635414D5B81B807 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereSelectEnumerableIterator_2_Dispose_m76E5F3DD7C637EC24BC0B6962F3852B5E4C9DCBB_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_enumerator_6();
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		RuntimeObject* L_1 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_1);
		InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t099785737FC6A1E3699919A94109383715A8D807_il2cpp_TypeInfo_var, (RuntimeObject*)L_1);
	}

IL_0013:
	{
		__this->set_enumerator_6((RuntimeObject*)NULL);
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		return;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectEnumerableIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectEnumerableIterator_2_MoveNext_mF775895EE064779EC9A579DA627D0ADE792BF57E_gshared (WhereSelectEnumerableIterator_2_t575305DD8907FFFDC568246A6635414D5B81B807 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereSelectEnumerableIterator_2_MoveNext_mF775895EE064779EC9A579DA627D0ADE792BF57E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0061;
		}
	}
	{
		goto IL_0074;
	}

IL_0011:
	{
		RuntimeObject* L_3 = (RuntimeObject*)__this->get_source_3();
		NullCheck((RuntimeObject*)L_3);
		RuntimeObject* L_4 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<!0> System.Collections.Generic.IEnumerable`1<Unity.Entities.Conversion.LogEventData>::GetEnumerator() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 5), (RuntimeObject*)L_3);
		__this->set_enumerator_6(L_4);
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_state_1(2);
		goto IL_0061;
	}

IL_002b:
	{
		RuntimeObject* L_5 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_5);
		LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  L_6 = InterfaceFuncInvoker0< LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  >::Invoke(0 /* !0 System.Collections.Generic.IEnumerator`1<Unity.Entities.Conversion.LogEventData>::get_Current() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 6), (RuntimeObject*)L_5);
		V_1 = (LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 )L_6;
		Func_2_t15BD356B2F637699370FD7109071A37617770BBA * L_7 = (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)__this->get_predicate_4();
		if (!L_7)
		{
			goto IL_004d;
		}
	}
	{
		Func_2_t15BD356B2F637699370FD7109071A37617770BBA * L_8 = (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)__this->get_predicate_4();
		LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  L_9 = V_1;
		NullCheck((Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)L_8);
		bool L_10 = ((  bool (*) (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *, LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)L_8, (LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 )L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		if (!L_10)
		{
			goto IL_0061;
		}
	}

IL_004d:
	{
		Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * L_11 = (Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)__this->get_selector_5();
		LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  L_12 = V_1;
		NullCheck((Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)L_11);
		RuntimeObject * L_13 = ((  RuntimeObject * (*) (Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *, LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)L_11, (LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 )L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_13);
		return (bool)1;
	}

IL_0061:
	{
		RuntimeObject* L_14 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_14);
		bool L_15 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t5956F3AFB7ECF1117E3BC5890E7FC7B7F7A04105_il2cpp_TypeInfo_var, (RuntimeObject*)L_14);
		if (L_15)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_0074:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectEnumerableIterator_2_Where_m01711860702C8FA93C95EA7C8A8D553199C0C903_gshared (WhereSelectEnumerableIterator_2_t575305DD8907FFFDC568246A6635414D5B81B807 * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * L_1 = (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 9));
		((  void (*) (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Object,System.Object>::.ctor(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectEnumerableIterator_2__ctor_m6DFD3E949A8FA5121F520D501B78C17E84EBDFAC_gshared (WhereSelectEnumerableIterator_2_tDAA8BFBEA68F81670F3F51C6200EBD26D7A8FBAB * __this, RuntimeObject* ___source0, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate1, Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		RuntimeObject* L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Object,System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereSelectEnumerableIterator_2_Clone_mD5F17A02281E6D1529D117CFF2E0F8C347D1B13F_gshared (WhereSelectEnumerableIterator_2_tDAA8BFBEA68F81670F3F51C6200EBD26D7A8FBAB * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_source_3();
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * L_2 = (Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)__this->get_selector_5();
		WhereSelectEnumerableIterator_2_tDAA8BFBEA68F81670F3F51C6200EBD26D7A8FBAB * L_3 = (WhereSelectEnumerableIterator_2_tDAA8BFBEA68F81670F3F51C6200EBD26D7A8FBAB *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectEnumerableIterator_2_tDAA8BFBEA68F81670F3F51C6200EBD26D7A8FBAB *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (RuntimeObject*)L_0, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_1, (Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Void System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Object,System.Object>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectEnumerableIterator_2_Dispose_mAA70577DEF67CEC98FE677984AE2175B7D4E4D00_gshared (WhereSelectEnumerableIterator_2_tDAA8BFBEA68F81670F3F51C6200EBD26D7A8FBAB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereSelectEnumerableIterator_2_Dispose_mAA70577DEF67CEC98FE677984AE2175B7D4E4D00_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = (RuntimeObject*)__this->get_enumerator_6();
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		RuntimeObject* L_1 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_1);
		InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t099785737FC6A1E3699919A94109383715A8D807_il2cpp_TypeInfo_var, (RuntimeObject*)L_1);
	}

IL_0013:
	{
		__this->set_enumerator_6((RuntimeObject*)NULL);
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		return;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Object,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectEnumerableIterator_2_MoveNext_m95AEE737A22EFFFE6557F448BF5AFCC6241D0BD7_gshared (WhereSelectEnumerableIterator_2_tDAA8BFBEA68F81670F3F51C6200EBD26D7A8FBAB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WhereSelectEnumerableIterator_2_MoveNext_m95AEE737A22EFFFE6557F448BF5AFCC6241D0BD7_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	RuntimeObject * V_1 = NULL;
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0061;
		}
	}
	{
		goto IL_0074;
	}

IL_0011:
	{
		RuntimeObject* L_3 = (RuntimeObject*)__this->get_source_3();
		NullCheck((RuntimeObject*)L_3);
		RuntimeObject* L_4 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0 /* System.Collections.Generic.IEnumerator`1<!0> System.Collections.Generic.IEnumerable`1<System.Object>::GetEnumerator() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 5), (RuntimeObject*)L_3);
		__this->set_enumerator_6(L_4);
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_state_1(2);
		goto IL_0061;
	}

IL_002b:
	{
		RuntimeObject* L_5 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_5);
		RuntimeObject * L_6 = InterfaceFuncInvoker0< RuntimeObject * >::Invoke(0 /* !0 System.Collections.Generic.IEnumerator`1<System.Object>::get_Current() */, IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 6), (RuntimeObject*)L_5);
		V_1 = (RuntimeObject *)L_6;
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_7 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		if (!L_7)
		{
			goto IL_004d;
		}
	}
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_8 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		RuntimeObject * L_9 = V_1;
		NullCheck((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_8);
		bool L_10 = ((  bool (*) (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_8, (RuntimeObject *)L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		if (!L_10)
		{
			goto IL_0061;
		}
	}

IL_004d:
	{
		Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * L_11 = (Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)__this->get_selector_5();
		RuntimeObject * L_12 = V_1;
		NullCheck((Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)L_11);
		RuntimeObject * L_13 = ((  RuntimeObject * (*) (Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)L_11, (RuntimeObject *)L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_13);
		return (bool)1;
	}

IL_0061:
	{
		RuntimeObject* L_14 = (RuntimeObject*)__this->get_enumerator_6();
		NullCheck((RuntimeObject*)L_14);
		bool L_15 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean System.Collections.IEnumerator::MoveNext() */, IEnumerator_t5956F3AFB7ECF1117E3BC5890E7FC7B7F7A04105_il2cpp_TypeInfo_var, (RuntimeObject*)L_14);
		if (L_15)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_0074:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectEnumerableIterator`2<System.Object,System.Object>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectEnumerableIterator_2_Where_m72A38A8170E8B837F5C090642BE08E3845A8EB37_gshared (WhereSelectEnumerableIterator_2_tDAA8BFBEA68F81670F3F51C6200EBD26D7A8FBAB * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * L_1 = (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 9));
		((  void (*) (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectListIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>::.ctor(System.Collections.Generic.List`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectListIterator_2__ctor_m4ECBB9EDB81DC5D81C2F729F3A528B5345D36418_gshared (WhereSelectListIterator_2_t08B768B4AD54F201B800C51471E1C1C5FD1A8E8C * __this, List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * ___source0, Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * ___predicate1, Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this);
		((  void (*) (Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectListIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 * WhereSelectListIterator_2_Clone_m002D801C6321DC9447528A3FC50556077FBF5AAE_gshared (WhereSelectListIterator_2_t08B768B4AD54F201B800C51471E1C1C5FD1A8E8C * __this, const RuntimeMethod* method)
{
	{
		List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * L_0 = (List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C *)__this->get_source_3();
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_1 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * L_2 = (Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)__this->get_selector_5();
		WhereSelectListIterator_2_t08B768B4AD54F201B800C51471E1C1C5FD1A8E8C * L_3 = (WhereSelectListIterator_2_t08B768B4AD54F201B800C51471E1C1C5FD1A8E8C *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectListIterator_2_t08B768B4AD54F201B800C51471E1C1C5FD1A8E8C *, List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C *, Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *, Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C *)L_0, (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_1, (Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectListIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectListIterator_2_MoveNext_m5C99254EED16684A96D5361E3BE8A8406B86C533_gshared (WhereSelectListIterator_2_t08B768B4AD54F201B800C51471E1C1C5FD1A8E8C * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		int32_t L_0 = (int32_t)((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0061;
		}
	}
	{
		goto IL_0074;
	}

IL_0011:
	{
		List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * L_3 = (List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C *)__this->get_source_3();
		NullCheck((List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C *)L_3);
		Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11  L_4 = ((  Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11  (*) (List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C *)L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		__this->set_enumerator_6(L_4);
		((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this)->set_state_1(2);
		goto IL_0061;
	}

IL_002b:
	{
		Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 * L_5 = (Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 *)__this->get_address_of_enumerator_6();
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_6 = Enumerator_get_Current_mB9DED66EBA82669AB83832B40F60E1710B5179B4_inline((Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 *)(Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 *)L_5, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		V_1 = (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_6;
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_7 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		if (!L_7)
		{
			goto IL_004d;
		}
	}
	{
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_8 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_9 = V_1;
		NullCheck((Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_8);
		bool L_10 = ((  bool (*) (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *, ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6)->methodPointer)((Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_8, (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6));
		if (!L_10)
		{
			goto IL_0061;
		}
	}

IL_004d:
	{
		Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 * L_11 = (Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)__this->get_selector_5();
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_12 = V_1;
		NullCheck((Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)L_11);
		Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  L_13 = ((  Color_tF40DAF76C04FFECF3FE6024F85A294741C9CC659  (*) (Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *, ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_t07574F1E7EF84CF543A9B2FF0E62BB6B96696C64 *)L_11, (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this)->set_current_2(L_13);
		return (bool)1;
	}

IL_0061:
	{
		Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 * L_14 = (Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 *)__this->get_address_of_enumerator_6();
		bool L_15 = Enumerator_MoveNext_mACDC401A875ECF83AEF9477068CDF02545A1D997((Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 *)(Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 *)L_14, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		if (L_15)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<UnityEngine.Color>::Dispose() */, (Iterator_1_tDEF6AC46E52D8687C27A6E60B6E0200D50011D76 *)__this);
	}

IL_0074:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectListIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Color>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectListIterator_2_Where_mDECC30954B416D7786855DD9643B46A7E6DEB8C2_gshared (WhereSelectListIterator_2_t08B768B4AD54F201B800C51471E1C1C5FD1A8E8C * __this, Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD * L_0 = ___predicate0;
		WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 * L_1 = (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10));
		((  void (*) (WhereEnumerableIterator_1_tEDD9A2C48D1FE8BE0706121F2B6C8A796196FFF2 *, RuntimeObject*, Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t3985FFDAB0D05F8E97E8EA849D5A24F16EFFC4FD *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectListIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>::.ctor(System.Collections.Generic.List`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectListIterator_2__ctor_mD3F8F885B0AF6977BCFCC072276C87068F087FB2_gshared (WhereSelectListIterator_2_t94F415FEA624816E42FAE1274A4EC29F5D5C1625 * __this, List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * ___source0, Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * ___predicate1, Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this);
		((  void (*) (Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectListIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF * WhereSelectListIterator_2_Clone_m699590CC3733CE2DB41ABA02182719C678DAE562_gshared (WhereSelectListIterator_2_t94F415FEA624816E42FAE1274A4EC29F5D5C1625 * __this, const RuntimeMethod* method)
{
	{
		List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * L_0 = (List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C *)__this->get_source_3();
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_1 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * L_2 = (Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)__this->get_selector_5();
		WhereSelectListIterator_2_t94F415FEA624816E42FAE1274A4EC29F5D5C1625 * L_3 = (WhereSelectListIterator_2_t94F415FEA624816E42FAE1274A4EC29F5D5C1625 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectListIterator_2_t94F415FEA624816E42FAE1274A4EC29F5D5C1625 *, List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C *, Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *, Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C *)L_0, (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_1, (Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectListIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectListIterator_2_MoveNext_m9FEC951E923EFC23AC8A86016D6A795C3275319E_gshared (WhereSelectListIterator_2_t94F415FEA624816E42FAE1274A4EC29F5D5C1625 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		int32_t L_0 = (int32_t)((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0061;
		}
	}
	{
		goto IL_0074;
	}

IL_0011:
	{
		List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C * L_3 = (List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C *)__this->get_source_3();
		NullCheck((List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C *)L_3);
		Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11  L_4 = ((  Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11  (*) (List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((List_1_t793A994CE01AE29FEE85500B7E3540653BFE5A0C *)L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		__this->set_enumerator_6(L_4);
		((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this)->set_state_1(2);
		goto IL_0061;
	}

IL_002b:
	{
		Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 * L_5 = (Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 *)__this->get_address_of_enumerator_6();
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_6 = Enumerator_get_Current_mB9DED66EBA82669AB83832B40F60E1710B5179B4_inline((Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 *)(Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 *)L_5, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		V_1 = (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_6;
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_7 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		if (!L_7)
		{
			goto IL_004d;
		}
	}
	{
		Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A * L_8 = (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)__this->get_predicate_4();
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_9 = V_1;
		NullCheck((Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_8);
		bool L_10 = ((  bool (*) (Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *, ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6)->methodPointer)((Func_2_tD4EF074F88731E713305C48E156AB9F6F0F9324A *)L_8, (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6));
		if (!L_10)
		{
			goto IL_0061;
		}
	}

IL_004d:
	{
		Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 * L_11 = (Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)__this->get_selector_5();
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_12 = V_1;
		NullCheck((Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)L_11);
		Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  L_13 = ((  Vector3_t65B972D6A585A0A5B63153CF1177A90D3C90D65E  (*) (Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *, ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_tA55660D7B36BC919063457215A12594F309CFDF1 *)L_11, (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this)->set_current_2(L_13);
		return (bool)1;
	}

IL_0061:
	{
		Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 * L_14 = (Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 *)__this->get_address_of_enumerator_6();
		bool L_15 = Enumerator_MoveNext_mACDC401A875ECF83AEF9477068CDF02545A1D997((Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 *)(Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 *)L_14, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		if (L_15)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<UnityEngine.Vector3>::Dispose() */, (Iterator_1_t04F5D870FD247BBBEE27254587FA10F440D4EEFF *)__this);
	}

IL_0074:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectListIterator`2<UnityEngine.Experimental.Rendering.Universal.LibTessDotNet.ContourVertex,UnityEngine.Vector3>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectListIterator_2_Where_mA49B2D74E495EFA26A1F215E3E809BAD571BCDA2_gshared (WhereSelectListIterator_2_t94F415FEA624816E42FAE1274A4EC29F5D5C1625 * __this, Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 * L_1 = (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10));
		((  void (*) (WhereEnumerableIterator_1_t0E01F06572EA26BE9E79530811037753CF6B3BF8 *, RuntimeObject*, Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t3041FD3183D19FE8416AE2E43A6398B2C06B7269 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectListIterator`2<Unity.Entities.Entity,System.Object>::.ctor(System.Collections.Generic.List`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectListIterator_2__ctor_mB56F9FF681AEF98005C4194BD14F38B19F5F2B7E_gshared (WhereSelectListIterator_2_t9800F9AD3EEADD8B7707A9F9E67DFC4C12E2F924 * __this, List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF * ___source0, Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * ___predicate1, Func_2_t895537CD65D26801427B03E05DD08125DE819919 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF * L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_t895537CD65D26801427B03E05DD08125DE819919 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectListIterator`2<Unity.Entities.Entity,System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereSelectListIterator_2_Clone_mCEEB0EC7AE1A878EAD100C780BC564417C789F8F_gshared (WhereSelectListIterator_2_t9800F9AD3EEADD8B7707A9F9E67DFC4C12E2F924 * __this, const RuntimeMethod* method)
{
	{
		List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF * L_0 = (List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF *)__this->get_source_3();
		Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * L_1 = (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)__this->get_predicate_4();
		Func_2_t895537CD65D26801427B03E05DD08125DE819919 * L_2 = (Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)__this->get_selector_5();
		WhereSelectListIterator_2_t9800F9AD3EEADD8B7707A9F9E67DFC4C12E2F924 * L_3 = (WhereSelectListIterator_2_t9800F9AD3EEADD8B7707A9F9E67DFC4C12E2F924 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectListIterator_2_t9800F9AD3EEADD8B7707A9F9E67DFC4C12E2F924 *, List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF *, Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *, Func_2_t895537CD65D26801427B03E05DD08125DE819919 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF *)L_0, (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)L_1, (Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectListIterator`2<Unity.Entities.Entity,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectListIterator_2_MoveNext_m1BDBF39668E0246EAAC41832FAEA466E0023336D_gshared (WhereSelectListIterator_2_t9800F9AD3EEADD8B7707A9F9E67DFC4C12E2F924 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0061;
		}
	}
	{
		goto IL_0074;
	}

IL_0011:
	{
		List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF * L_3 = (List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF *)__this->get_source_3();
		NullCheck((List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF *)L_3);
		Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62  L_4 = ((  Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62  (*) (List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((List_1_t392B557248A8E4917D43F766886FBA9DDEA40BAF *)L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		__this->set_enumerator_6(L_4);
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_state_1(2);
		goto IL_0061;
	}

IL_002b:
	{
		Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 * L_5 = (Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 *)__this->get_address_of_enumerator_6();
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_6 = Enumerator_get_Current_m5478E2379F0D2D42C6D2FAF5C9B3297298D5BFC8_inline((Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 *)(Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 *)L_5, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		V_1 = (Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 )L_6;
		Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * L_7 = (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)__this->get_predicate_4();
		if (!L_7)
		{
			goto IL_004d;
		}
	}
	{
		Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 * L_8 = (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)__this->get_predicate_4();
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_9 = V_1;
		NullCheck((Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)L_8);
		bool L_10 = ((  bool (*) (Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6)->methodPointer)((Func_2_t14BB53D120BF18F218ACE746215828AC2863F843 *)L_8, (Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 )L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6));
		if (!L_10)
		{
			goto IL_0061;
		}
	}

IL_004d:
	{
		Func_2_t895537CD65D26801427B03E05DD08125DE819919 * L_11 = (Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)__this->get_selector_5();
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_12 = V_1;
		NullCheck((Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)L_11);
		RuntimeObject * L_13 = ((  RuntimeObject * (*) (Func_2_t895537CD65D26801427B03E05DD08125DE819919 *, Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_t895537CD65D26801427B03E05DD08125DE819919 *)L_11, (Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 )L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_13);
		return (bool)1;
	}

IL_0061:
	{
		Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 * L_14 = (Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 *)__this->get_address_of_enumerator_6();
		bool L_15 = Enumerator_MoveNext_m2505BC1BC82C7BB3A2DE21A19BA41FB8CA521671((Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 *)(Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 *)L_14, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		if (L_15)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_0074:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectListIterator`2<Unity.Entities.Entity,System.Object>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectListIterator_2_Where_mEB729F9E900D4DE4D4AD150F576342EAD7FC1CE1_gshared (WhereSelectListIterator_2_t9800F9AD3EEADD8B7707A9F9E67DFC4C12E2F924 * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * L_1 = (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10));
		((  void (*) (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectListIterator`2<System.Int32,System.Int32>::.ctor(System.Collections.Generic.List`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectListIterator_2__ctor_m895E21AE9AB1E3F19B3147EDC913BB567B1A65C7_gshared (WhereSelectListIterator_2_t4CC3FE3A35610DC6F761EE7DB863B845957AD325 * __this, List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * ___source0, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate1, Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this);
		((  void (*) (Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectListIterator`2<System.Int32,System.Int32>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 * WhereSelectListIterator_2_Clone_mEE6375B2C79172E13732CA49AAF389493C1C7100_gshared (WhereSelectListIterator_2_t4CC3FE3A35610DC6F761EE7DB863B845957AD325 * __this, const RuntimeMethod* method)
{
	{
		List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * L_0 = (List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 *)__this->get_source_3();
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * L_2 = (Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)__this->get_selector_5();
		WhereSelectListIterator_2_t4CC3FE3A35610DC6F761EE7DB863B845957AD325 * L_3 = (WhereSelectListIterator_2_t4CC3FE3A35610DC6F761EE7DB863B845957AD325 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectListIterator_2_t4CC3FE3A35610DC6F761EE7DB863B845957AD325 *, List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 *, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 *)L_0, (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_1, (Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectListIterator`2<System.Int32,System.Int32>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectListIterator_2_MoveNext_m637B802A50BA94CD511636CAF5D912C6B96B18A1_gshared (WhereSelectListIterator_2_t4CC3FE3A35610DC6F761EE7DB863B845957AD325 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		int32_t L_0 = (int32_t)((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0061;
		}
	}
	{
		goto IL_0074;
	}

IL_0011:
	{
		List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * L_3 = (List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 *)__this->get_source_3();
		NullCheck((List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 *)L_3);
		Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C  L_4 = ((  Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C  (*) (List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 *)L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		__this->set_enumerator_6(L_4);
		((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this)->set_state_1(2);
		goto IL_0061;
	}

IL_002b:
	{
		Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C * L_5 = (Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C *)__this->get_address_of_enumerator_6();
		int32_t L_6 = Enumerator_get_Current_m6BBD624C51F7E20D347FE5894A6ECA94B8011181_inline((Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C *)(Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C *)L_5, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		V_1 = (int32_t)L_6;
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_7 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		if (!L_7)
		{
			goto IL_004d;
		}
	}
	{
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_8 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		int32_t L_9 = V_1;
		NullCheck((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_8);
		bool L_10 = ((  bool (*) (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6)->methodPointer)((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_8, (int32_t)L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6));
		if (!L_10)
		{
			goto IL_0061;
		}
	}

IL_004d:
	{
		Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA * L_11 = (Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)__this->get_selector_5();
		int32_t L_12 = V_1;
		NullCheck((Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)L_11);
		int32_t L_13 = ((  int32_t (*) (Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_tFF6AE79EFD0857556AD37A1A1594C43F76012FEA *)L_11, (int32_t)L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this)->set_current_2(L_13);
		return (bool)1;
	}

IL_0061:
	{
		Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C * L_14 = (Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C *)__this->get_address_of_enumerator_6();
		bool L_15 = Enumerator_MoveNext_m40FD166B6757334A2BBCF67238EFDF70D727A4A6((Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C *)(Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C *)L_14, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		if (L_15)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Int32>::Dispose() */, (Iterator_1_tCFFC952B03DBE4E956DE317DB9704D936AEA2379 *)__this);
	}

IL_0074:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectListIterator`2<System.Int32,System.Int32>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectListIterator_2_Where_m7F75FF628D2E99D2BA127B84FDD08DD88048ADB0_gshared (WhereSelectListIterator_2_t4CC3FE3A35610DC6F761EE7DB863B845957AD325 * __this, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA * L_1 = (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10));
		((  void (*) (WhereEnumerableIterator_1_t9F4DDC70173BABD72AEC7AA00D62F4FAE2613CEA *, RuntimeObject*, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectListIterator`2<System.Int32,System.Object>::.ctor(System.Collections.Generic.List`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectListIterator_2__ctor_mA0D61B688D5EE4E84300EA96C87ED9F3E10D5EA9_gshared (WhereSelectListIterator_2_tA41D93FF12E41BB5A5BEA27AEED367695ADACEA4 * __this, List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * ___source0, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * ___predicate1, Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectListIterator`2<System.Int32,System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereSelectListIterator_2_Clone_mE40F022902D030D86E465678E5DD79B3058FE2CB_gshared (WhereSelectListIterator_2_tA41D93FF12E41BB5A5BEA27AEED367695ADACEA4 * __this, const RuntimeMethod* method)
{
	{
		List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * L_0 = (List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 *)__this->get_source_3();
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_1 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * L_2 = (Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)__this->get_selector_5();
		WhereSelectListIterator_2_tA41D93FF12E41BB5A5BEA27AEED367695ADACEA4 * L_3 = (WhereSelectListIterator_2_tA41D93FF12E41BB5A5BEA27AEED367695ADACEA4 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectListIterator_2_tA41D93FF12E41BB5A5BEA27AEED367695ADACEA4 *, List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 *, Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 *)L_0, (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_1, (Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectListIterator`2<System.Int32,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectListIterator_2_MoveNext_mDEB78D7AB98D0FDC13661615FDBC0C01A621E84F_gshared (WhereSelectListIterator_2_tA41D93FF12E41BB5A5BEA27AEED367695ADACEA4 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0061;
		}
	}
	{
		goto IL_0074;
	}

IL_0011:
	{
		List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 * L_3 = (List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 *)__this->get_source_3();
		NullCheck((List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 *)L_3);
		Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C  L_4 = ((  Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C  (*) (List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((List_1_t260B41F956D673396C33A4CF94E8D6C4389EACB7 *)L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		__this->set_enumerator_6(L_4);
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_state_1(2);
		goto IL_0061;
	}

IL_002b:
	{
		Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C * L_5 = (Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C *)__this->get_address_of_enumerator_6();
		int32_t L_6 = Enumerator_get_Current_m6BBD624C51F7E20D347FE5894A6ECA94B8011181_inline((Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C *)(Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C *)L_5, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		V_1 = (int32_t)L_6;
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_7 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		if (!L_7)
		{
			goto IL_004d;
		}
	}
	{
		Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 * L_8 = (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)__this->get_predicate_4();
		int32_t L_9 = V_1;
		NullCheck((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_8);
		bool L_10 = ((  bool (*) (Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6)->methodPointer)((Func_2_t2EBF98B0BA555D9F0633C9BCCBE3DF332B9C1274 *)L_8, (int32_t)L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6));
		if (!L_10)
		{
			goto IL_0061;
		}
	}

IL_004d:
	{
		Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 * L_11 = (Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)__this->get_selector_5();
		int32_t L_12 = V_1;
		NullCheck((Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)L_11);
		RuntimeObject * L_13 = ((  RuntimeObject * (*) (Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_t401E8A228CE43E56CCE9280AD9C6D87CC73A0123 *)L_11, (int32_t)L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_13);
		return (bool)1;
	}

IL_0061:
	{
		Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C * L_14 = (Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C *)__this->get_address_of_enumerator_6();
		bool L_15 = Enumerator_MoveNext_m40FD166B6757334A2BBCF67238EFDF70D727A4A6((Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C *)(Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C *)L_14, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		if (L_15)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_0074:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectListIterator`2<System.Int32,System.Object>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectListIterator_2_Where_m9633A851E09C546940C2D5C7EF8CB7C501784EB3_gshared (WhereSelectListIterator_2_tA41D93FF12E41BB5A5BEA27AEED367695ADACEA4 * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * L_1 = (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10));
		((  void (*) (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectListIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>::.ctor(System.Collections.Generic.List`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectListIterator_2__ctor_m44787FB713B0ADF820FF8562BDAAF12E80167BB7_gshared (WhereSelectListIterator_2_tCD2B0DD2D1D32234F3640C8D7B5C9C212AAAD6E4 * __this, List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 * ___source0, Func_2_t15BD356B2F637699370FD7109071A37617770BBA * ___predicate1, Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 * L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t15BD356B2F637699370FD7109071A37617770BBA * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectListIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereSelectListIterator_2_Clone_mAD27EB948F37C0169679402281821C277CE04E61_gshared (WhereSelectListIterator_2_tCD2B0DD2D1D32234F3640C8D7B5C9C212AAAD6E4 * __this, const RuntimeMethod* method)
{
	{
		List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 * L_0 = (List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 *)__this->get_source_3();
		Func_2_t15BD356B2F637699370FD7109071A37617770BBA * L_1 = (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)__this->get_predicate_4();
		Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * L_2 = (Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)__this->get_selector_5();
		WhereSelectListIterator_2_tCD2B0DD2D1D32234F3640C8D7B5C9C212AAAD6E4 * L_3 = (WhereSelectListIterator_2_tCD2B0DD2D1D32234F3640C8D7B5C9C212AAAD6E4 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectListIterator_2_tCD2B0DD2D1D32234F3640C8D7B5C9C212AAAD6E4 *, List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 *, Func_2_t15BD356B2F637699370FD7109071A37617770BBA *, Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 *)L_0, (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)L_1, (Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectListIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectListIterator_2_MoveNext_m8C67484DFB0AFF0499857263FAF16BEB65236170_gshared (WhereSelectListIterator_2_tCD2B0DD2D1D32234F3640C8D7B5C9C212AAAD6E4 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0061;
		}
	}
	{
		goto IL_0074;
	}

IL_0011:
	{
		List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 * L_3 = (List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 *)__this->get_source_3();
		NullCheck((List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 *)L_3);
		Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE  L_4 = ((  Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE  (*) (List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((List_1_t1AE521895023EE00F60182F49A2461BBE7FE3F45 *)L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		__this->set_enumerator_6(L_4);
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_state_1(2);
		goto IL_0061;
	}

IL_002b:
	{
		Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE * L_5 = (Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE *)__this->get_address_of_enumerator_6();
		LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  L_6 = Enumerator_get_Current_mAB62DE0E08E4774E60465C9247E8B7A1E1831223_inline((Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE *)(Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE *)L_5, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		V_1 = (LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 )L_6;
		Func_2_t15BD356B2F637699370FD7109071A37617770BBA * L_7 = (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)__this->get_predicate_4();
		if (!L_7)
		{
			goto IL_004d;
		}
	}
	{
		Func_2_t15BD356B2F637699370FD7109071A37617770BBA * L_8 = (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)__this->get_predicate_4();
		LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  L_9 = V_1;
		NullCheck((Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)L_8);
		bool L_10 = ((  bool (*) (Func_2_t15BD356B2F637699370FD7109071A37617770BBA *, LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6)->methodPointer)((Func_2_t15BD356B2F637699370FD7109071A37617770BBA *)L_8, (LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 )L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6));
		if (!L_10)
		{
			goto IL_0061;
		}
	}

IL_004d:
	{
		Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 * L_11 = (Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)__this->get_selector_5();
		LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  L_12 = V_1;
		NullCheck((Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)L_11);
		RuntimeObject * L_13 = ((  RuntimeObject * (*) (Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *, LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_t79DC2C3DD1B96310A0258D6DFEFD44CB6EAC5154 *)L_11, (LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 )L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_13);
		return (bool)1;
	}

IL_0061:
	{
		Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE * L_14 = (Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE *)__this->get_address_of_enumerator_6();
		bool L_15 = Enumerator_MoveNext_m7A7F9EE8CA53E7533A04E8E65B8BF08A2C88A34B((Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE *)(Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE *)L_14, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		if (L_15)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_0074:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectListIterator`2<Unity.Entities.Conversion.LogEventData,System.Object>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectListIterator_2_Where_mCE66D3D0A1BC4DD510AA9CE2EDBD3DBA468D9851_gshared (WhereSelectListIterator_2_tCD2B0DD2D1D32234F3640C8D7B5C9C212AAAD6E4 * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * L_1 = (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10));
		((  void (*) (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_1;
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
// System.Void System.Linq.Enumerable_WhereSelectListIterator`2<System.Object,System.Object>::.ctor(System.Collections.Generic.List`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WhereSelectListIterator_2__ctor_mCF313A191371C8CCC2E79D89A3BF21714EFDB20E_gshared (WhereSelectListIterator_2_t85B78DFF0573BC95A62C79D6088FA39DFEBE1AF2 * __this, List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * ___source0, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate1, Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * ___selector2, const RuntimeMethod* method)
{
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		((  void (*) (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * L_0 = ___source0;
		__this->set_source_3(L_0);
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = ___predicate1;
		__this->set_predicate_4(L_1);
		Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * L_2 = ___selector2;
		__this->set_selector_5(L_2);
		return;
	}
}
// System.Linq.Enumerable_Iterator`1<TResult> System.Linq.Enumerable_WhereSelectListIterator`2<System.Object,System.Object>::Clone()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 * WhereSelectListIterator_2_Clone_m667BCD94E83BB3A02AF2D66E07B089FA86971342_gshared (WhereSelectListIterator_2_t85B78DFF0573BC95A62C79D6088FA39DFEBE1AF2 * __this, const RuntimeMethod* method)
{
	{
		List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * L_0 = (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *)__this->get_source_3();
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_1 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * L_2 = (Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)__this->get_selector_5();
		WhereSelectListIterator_2_t85B78DFF0573BC95A62C79D6088FA39DFEBE1AF2 * L_3 = (WhereSelectListIterator_2_t85B78DFF0573BC95A62C79D6088FA39DFEBE1AF2 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (WhereSelectListIterator_2_t85B78DFF0573BC95A62C79D6088FA39DFEBE1AF2 *, List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_3, (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *)L_0, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_1, (Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return L_3;
	}
}
// System.Boolean System.Linq.Enumerable_WhereSelectListIterator`2<System.Object,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool WhereSelectListIterator_2_MoveNext_mEE0E8B173345B059100E0736D106FFAE0C2D29CA_gshared (WhereSelectListIterator_2_t85B78DFF0573BC95A62C79D6088FA39DFEBE1AF2 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	RuntimeObject * V_1 = NULL;
	{
		int32_t L_0 = (int32_t)((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->get_state_1();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) == ((int32_t)1)))
		{
			goto IL_0011;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0061;
		}
	}
	{
		goto IL_0074;
	}

IL_0011:
	{
		List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 * L_3 = (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *)__this->get_source_3();
		NullCheck((List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *)L_3);
		Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6  L_4 = ((  Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6  (*) (List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((List_1_t3F94120C77410A62EAE48421CF166B83AB95A2F5 *)L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		__this->set_enumerator_6(L_4);
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_state_1(2);
		goto IL_0061;
	}

IL_002b:
	{
		Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * L_5 = (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 *)__this->get_address_of_enumerator_6();
		RuntimeObject * L_6 = Enumerator_get_Current_m9C4EBBD2108B51885E750F927D7936290C8E20EE_inline((Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 *)(Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 *)L_5, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		V_1 = (RuntimeObject *)L_6;
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_7 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		if (!L_7)
		{
			goto IL_004d;
		}
	}
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_8 = (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)__this->get_predicate_4();
		RuntimeObject * L_9 = V_1;
		NullCheck((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_8);
		bool L_10 = ((  bool (*) (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6)->methodPointer)((Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_8, (RuntimeObject *)L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6));
		if (!L_10)
		{
			goto IL_0061;
		}
	}

IL_004d:
	{
		Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 * L_11 = (Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)__this->get_selector_5();
		RuntimeObject * L_12 = V_1;
		NullCheck((Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)L_11);
		RuntimeObject * L_13 = ((  RuntimeObject * (*) (Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((Func_2_tFF5BB8F40A35B1BEA00D4EBBC6CBE7184A584436 *)L_11, (RuntimeObject *)L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this)->set_current_2(L_13);
		return (bool)1;
	}

IL_0061:
	{
		Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * L_14 = (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 *)__this->get_address_of_enumerator_6();
		bool L_15 = Enumerator_MoveNext_m2E56233762839CE55C67E00AC8DD3D4D3F6C0DF0((Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 *)(Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 *)L_14, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		if (L_15)
		{
			goto IL_002b;
		}
	}
	{
		NullCheck((Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
		VirtActionInvoker0::Invoke(12 /* System.Void System.Linq.Enumerable/Iterator`1<System.Object>::Dispose() */, (Iterator_1_t674ABE41CF4096D4BE4D51E21FEBDADBF74CC279 *)__this);
	}

IL_0074:
	{
		return (bool)0;
	}
}
// System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable_WhereSelectListIterator`2<System.Object,System.Object>::Where(System.Func`2<TResult,System.Boolean>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* WhereSelectListIterator_2_Where_mAC87184664F73DD7F3EC4AB4CE2BDE71BE76249D_gshared (WhereSelectListIterator_2_t85B78DFF0573BC95A62C79D6088FA39DFEBE1AF2 * __this, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * ___predicate0, const RuntimeMethod* method)
{
	{
		Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 * L_0 = ___predicate0;
		WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 * L_1 = (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 10));
		((  void (*) (WhereEnumerableIterator_1_t1E9FDCFD8F8136C6A5A5740C1E093EF03F0B5CE0 *, RuntimeObject*, Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)(L_1, (RuntimeObject*)__this, (Func_2_t99409DECFF50F0FA9B427C863AC6C99C66E6F9F8 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		return L_1;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject * Enumerator_get_Current_m9C4EBBD2108B51885E750F927D7936290C8E20EE_gshared_inline (Enumerator_tB6009981BD4E3881E3EC83627255A24198F902D6 * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = (RuntimeObject *)__this->get_current_3();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  Enumerator_get_Current_mB9DED66EBA82669AB83832B40F60E1710B5179B4_gshared_inline (Enumerator_t0A364062D6EEDD0B306D570939B5709AEAA88D11 * __this, const RuntimeMethod* method)
{
	{
		ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536  L_0 = (ContourVertex_tF9E27CB6BCC62DF5F4202153BBBECDE5E3283536 )__this->get_current_3();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  Enumerator_get_Current_m5478E2379F0D2D42C6D2FAF5C9B3297298D5BFC8_gshared_inline (Enumerator_tDB43E7F96C9DB9F440CD1AB9816CD020CBD88A62 * __this, const RuntimeMethod* method)
{
	{
		Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4  L_0 = (Entity_tA8DE691EC83EB40E80A611C2E6D8C90C3C97AAF4 )__this->get_current_3();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t Enumerator_get_Current_m6BBD624C51F7E20D347FE5894A6ECA94B8011181_gshared_inline (Enumerator_t7BA00929E14A2F2A62CE085585044A3FEB2C5F3C * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = (int32_t)__this->get_current_3();
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  Enumerator_get_Current_mAB62DE0E08E4774E60465C9247E8B7A1E1831223_gshared_inline (Enumerator_tC5DC621F5FC5909320AE6F0CC38964B138A81EAE * __this, const RuntimeMethod* method)
{
	{
		LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1  L_0 = (LogEventData_t13585C99A55DB1722FFE891AD83B6D8EFAE593A1 )__this->get_current_3();
		return L_0;
	}
}
