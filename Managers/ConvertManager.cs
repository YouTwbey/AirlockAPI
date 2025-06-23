using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.IO;

namespace AirlockAPI.Managers
{
    internal static class ConvertManager
    {
        public static Il2CppArrayBase<PlayerRef> PlayerIEnumeratorToArray<PlayerRef>(this Il2CppSystem.Collections.Generic.IEnumerable<PlayerRef> source)
        {
            return source.ToArray();
        }
    }
}
