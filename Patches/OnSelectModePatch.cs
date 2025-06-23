using AirlockAPI.Data;
using AirlockAPI.Debug;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.UI.TitleScreen;
using HarmonyLib;

namespace AirlockAPI.Patches
{
    [HarmonyPatch(typeof(GamemodeSelectionMenu), nameof(GamemodeSelectionMenu.OnModeSelect))]
    internal static class OnModeSelectPatch
    {
        public static void Prefix(GamemodeSelectionMenu __instance, MapModeSelect modeSelect)
        {
            if (modeSelect.ModeInfo.ModeName.StartsWith("<size=0>MODDED</size><color=yellow>"))
            {
                CurrentMode.Modded = true;
                CurrentMode.Name = modeSelect.ModeInfo.ModeName.Replace("<size=0>MODDED</size><color=yellow>", "");
            }
            else
            {
                CurrentMode.Modded = false;
                CurrentMode.Name = modeSelect.ModeInfo.ModeName;
            }

            if (CurrentMode.Modded)
            {
                __instance._startButton._isDisabled = __instance._isQuickMatch;
            }
            else
            {
                __instance._startButton._isDisabled = false;
            }

            Logging.Debug_Log("Mode: " + CurrentMode.Name + " | Modded Gamemode: " + CurrentMode.Modded);
        }
    }
}
