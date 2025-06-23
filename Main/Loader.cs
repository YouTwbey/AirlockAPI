using AirlockAPI.Attributes;
using AirlockAPI.Managers;
using MelonLoader;
using System.Reflection;

namespace AirlockAPI.Main
{
    internal class Loader : MelonPlugin
    {
        public override void OnApplicationStarted() 
        {
            MelonBase[] melons = MelonBase.RegisteredMelons.ToArray();

            foreach (MelonBase melon in melons)
            {
                Assembly dll = melon.MelonAssembly.Assembly;
                if (dll != null)
                {
                    foreach (Type type in dll.GetTypes())
                    {
                        MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                        foreach (MethodInfo method in methods)
                        {
                            AirlockRpc rpc = method.GetCustomAttribute<AirlockRpc>();

                            if (rpc != null)
                            {
                                NetworkManager.StringToAirlockRpc.Add(rpc.RpcName, rpc);
                                NetworkManager.RegisteredRpcs.Add(rpc, method);
                            }
                        }
                    }
                }
            }
        }

        public override void OnUpdate()
        {
            GamemodeManager.Update();
        }
    }
}
