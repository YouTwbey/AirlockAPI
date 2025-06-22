using static AirlockAPI.Managers.NetworkManager;
using Il2CppFusion;

namespace AirlockAPI.Data
{
    public class AirlockRpcInfo
    {
        public PlayerRef Sender { get; internal set; } = PlayerRef.None;
        public string Rpc { get; internal set; } = "";
        public RpcTarget Target { get; internal set; } = RpcTarget.All;
        public RpcCaller Caller { get; internal set; } = RpcCaller.All;
    }
}
