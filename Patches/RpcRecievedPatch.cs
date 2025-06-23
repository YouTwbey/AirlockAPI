using AirlockAPI.Data;
using HarmonyLib;
using Il2CppFusion;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Text;
using UnityEngine;
using static AirlockAPI.Managers.NetworkManager;

namespace AirlockAPI.Patches
{
    [HarmonyPatch(typeof(NetworkRunner), nameof(NetworkRunner.Fusion_Simulation_ICallbacks_OnReliableData))]
    internal static class RpcRecievedPatch
    {
        public static void Prefix(NetworkRunner __instance, PlayerRef player, Il2CppStructArray<byte> dataArray)
        {
            using (var ms = new MemoryStream(dataArray))
            using (var reader = new BinaryReader(ms, Encoding.UTF8))
            {
                Network.TryGetHostPlayer(out PlayerRef host);
                string rpcName = reader.ReadString();

                int count = reader.ReadInt32();
                var args = new object[count];

                for (int i = 0; i < count; i++)
                {
                    byte tag = reader.ReadByte();
                    switch (tag)
                    {
                        case 1:
                            args[i] = reader.ReadInt32();
                            break;

                        case 2:
                            args[i] = reader.ReadSingle();
                            break;

                        case 3:
                            args[i] = reader.ReadByte();
                            break;

                        case 4:
                            args[i] = reader.ReadBoolean();
                            break;

                        case 5:
                            args[i] = reader.ReadString();
                            break;

                        case 6:
                            args[i] = GameObject.Find(reader.ReadString());
                            break;

                        case 7:
                            if (host == player)
                            {
                                AirlockRpcInfo info = new AirlockRpcInfo();
                                info.Rpc = reader.ReadString();
                                info.Sender = reader.ReadInt32();
                                info.Target = (RpcTarget)Enum.Parse(typeof(RpcTarget), reader.ReadString());
                                info.Caller = (RpcCaller)Enum.Parse(typeof(RpcCaller), reader.ReadString());

                                args[i] = info;
                            }
                            break;

                        default:
                            throw new InvalidOperationException($"Unknown tag: {tag}");
                    }
                }

                OnRecievedRpc(rpcName, player, args);
            }
        }
    }
}
