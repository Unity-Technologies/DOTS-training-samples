using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public struct IsArena : IComponentData { }
public struct YellowBase : IComponentData { }
public struct BlueBase : IComponentData { }

public struct IsBee : IComponentData { }
public struct IsResource : IComponentData { }

public struct IsGathering : IComponentData { }
public struct IsCarried : IComponentData { }
public struct IsReturning : IComponentData { }
public struct IsAttacking : IComponentData { }
public struct IsDead : IComponentData { }

public struct HasGravity : IComponentData { }
public struct OnCollision : IComponentData { }
