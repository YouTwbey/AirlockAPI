using AirlockAPI.Attributes;
using AirlockAPI.Data;
using AirlockAPI.Patches;
using Il2CppFusion;
using Il2CppSG.Airlock.Network;
using Il2CppSG.CoreLite;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace AirlockAPI.Managers
{
    public static class NetworkManager
    {
        internal static AirlockNetworkRunner Network;

        /// <summary>
        /// Who will recieve the Rpc.
        /// </summary>
        public enum RpcTarget
        {
            /// <summary>
            /// Rpc will be sent to everyone but you.
            /// </summary>
            All,
            /// <summary>
            /// Rpc will be sent to everyone including you.
            /// </summary>
            AllInclusive,
            /// <summary>
            /// Rpc will be sent to the host only.
            /// </summary>
            Host
        }

        /// <summary>
        /// Who can call the Rpc.
        /// </summary>
        public enum RpcCaller
        {
            /// <summary>
            /// Rpc can be called by anyone.
            /// </summary>
            All,
            /// <summary>
            /// Rpc can only be called by the host only.
            /// </summary>
            Host
        }


        internal static Dictionary<string, AirlockRpc> StringToAirlockRpc = new Dictionary<string, AirlockRpc>();
        internal static Dictionary<AirlockRpc, MethodBase> RegisteredRpcs = new Dictionary<AirlockRpc, MethodBase>();

        /// <summary>
        /// Send an RPC by it's name along with parameters.
        /// </summary>
        /// <param name="rpc">Name of the RPC</param>
        /// <param name="param">Parameters of the RPC</param>
        public static void SendRpc(string rpc, params object[] param)
        {
            ProcessSendingRpc(rpc, -1, param);
        }

        public static void SendRpc(string rpc, int targetPlayer = -1, params object[] param)
        {
            ProcessSendingRpc(rpc, targetPlayer, param);
        }

        internal static void ProcessSendingRpc(string rpc, int targetPlayer = -1, params object[] param)
        {
            if (Network == null)
            {
                Network = UnityEngine.Object.FindObjectOfType<AirlockNetworkRunner>();

                if (Network == null)
                {
                    return;
                }
            }

            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
            {
                writer.Write(rpc);
                writer.Write(param.Length);

                foreach (var arg in param)
                {
                    switch (arg)
                    {
                        case int i:
                            writer.Write((byte)1);
                            writer.Write(i);
                            break;

                        case float f:
                            writer.Write((byte)2);
                            writer.Write(f);
                            break;

                        case byte b:
                            writer.Write((byte)3);
                            writer.Write(b);
                            break;

                        case bool bo:
                            writer.Write((byte)4);
                            writer.Write(bo);
                            break;

                        case string s:
                            writer.Write((byte)5);
                            writer.Write(s);
                            break;

                        case GameObject go:
                            writer.Write((byte)6);
                            writer.Write(go.transform.GetTransformPath());
                            break;

                        case AirlockRpcInfo info:
                            writer.Write((byte)7);
                            writer.Write(info.Rpc);
                            writer.Write(info.Sender.PlayerId);
                            writer.Write(info.Target.ToString());
                            writer.Write(info.Caller.ToString());
                            break;

                        default:
                            throw new InvalidOperationException($"Unsupported arg type {arg.GetType()}");
                    }
                }

                writer.Flush();

                byte[] dataToSend = ms.ToArray();
                Network.TryGetHostPlayer(out PlayerRef host);

                if (host == Network.LocalPlayer)
                {
                    if (targetPlayer != -1)
                    {
                        Network.SendReliableDataToPlayer(targetPlayer, dataToSend);
                    }
                    else
                    {
                        foreach (PlayerRef player in Network.ActivePlayers.PlayerIEnumeratorToArray())
                        {
                            if (player != host)
                            {
                                Network.SendReliableDataToPlayer(player, dataToSend);
                            }
                        }

                        RpcRecievedPatch.Prefix(Network, host, dataToSend);
                    }
                }
                else
                {
                    Network.SendReliableDataToServer(dataToSend);
                }
            }
        }

        internal static void OnRecievedRpc(string rpc, int sender, params object[] args)
        {
            AirlockRpc airlockRpc = StringToAirlockRpc[rpc];

            if (airlockRpc != null)
            {
                MethodBase method = RegisteredRpcs[airlockRpc];

                if (method != null)
                {
                    Network.TryGetHostPlayer(out PlayerRef host);

                    if (airlockRpc.RpcCaller == RpcCaller.Host)
                    {
                        if (host != sender) return;
                    }

                    if (args[args.Length - 1] is AirlockRpcInfo) { } else
                    {
                        ParameterInfo[] param = method.GetParameters();

                        if (param.Last() != null)
                        {
                            if (param.Last().ParameterType == typeof(AirlockRpcInfo))
                            {
                                AirlockRpcInfo rpcInfo = new AirlockRpcInfo()
                                {
                                    Rpc = rpc,
                                    Target = airlockRpc.RpcTarget,
                                    Caller = airlockRpc.RpcCaller,
                                    Sender = sender
                                };

                                args = args.Append(rpcInfo).ToArray();
                            }
                        }
                    }

                    switch (airlockRpc.RpcTarget)
                    {
                        case RpcTarget.All:
                            if (sender != Network.LocalPlayer.PlayerId)
                            {
                                InvokeRpc(method, args);
                            }

                            Echo(host, sender, rpc, args);
                            break;

                        case RpcTarget.AllInclusive:
                            InvokeRpc(method, args);

                            Echo(host, sender, rpc, args);
                            break;

                        case RpcTarget.Host:
                            if (host == Network.LocalPlayer)
                            {
                                InvokeRpc(method, args);
                            }
                            break;
                    }
                }
            }
        }

        internal static void Echo(PlayerRef host, int sender, string rpc, params object[] args)
        {
            if (sender != host && Network.LocalPlayer == host)
            {
                ProcessSendingRpc(rpc, -1, args);
            }
        }

        internal static void InvokeRpc(MethodBase method, params object[] args)
        {
            method.Invoke(null, args);
        }
    }
}
