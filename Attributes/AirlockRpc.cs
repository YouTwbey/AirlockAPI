using AirlockAPI.Managers;

namespace AirlockAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AirlockRpc : Attribute
    {
        internal string RpcName;
        internal NetworkManager.RpcTarget RpcTarget;
        internal NetworkManager.RpcCaller RpcCaller;

        public AirlockRpc(string rpcName, NetworkManager.RpcTarget rpcTarget, NetworkManager.RpcCaller rpcCaller)
        {
            RpcName = rpcName;
            RpcTarget = rpcTarget;
            RpcCaller = rpcCaller;
        }
    }
}
