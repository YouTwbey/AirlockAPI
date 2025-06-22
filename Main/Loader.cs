using AirlockAPI.Attributes;
using AirlockAPI.Main;
using AirlockAPI.Managers;
using MelonLoader;
using System.Reflection;

[assembly: MelonInfo(typeof(Loader), "AirlockAPI", "1.0.0", "YouTubey")]

namespace AirlockAPI.Main
{
    public class Loader : MelonPlugin
    {
        public override void OnLateInitializeMelon()
        {
            MelonBase[] assemblies = MelonAssembly.LoadedMelons.ToArray();

            foreach (MelonBase assembly in assemblies)
            {
                Assembly dll = assembly.MelonAssembly.Assembly;
                if (dll != null)
                {
                    foreach (Type type in dll.GetTypes())
                    {
                        MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);

                        foreach (MethodInfo method in methods)
                        {
                            AirlockRpc? rpc = method.GetCustomAttribute<AirlockRpc>();

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
