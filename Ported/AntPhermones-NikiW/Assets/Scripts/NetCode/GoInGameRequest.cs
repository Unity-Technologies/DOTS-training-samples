using System;
using AOT;
using Unity.NetCode;
using Unity.Burst;
using UnityEngine;

// RPC request from client to server for game to go "in game" and send snapshots / inputs
public struct GoInGameRequest : IRpcCommand
{
}