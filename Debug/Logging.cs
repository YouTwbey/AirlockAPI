using UnityEngine;
using static MelonLoader.MelonLogger;

namespace AirlockAPI.Debug
{
    internal static class Logging
    {
        public static void Log(string message)
        {
            Msg(message);
        }

        public static void Warn(string message)
        {
            Warning(message);
        }

        public static void Error(string message, bool crash = false)
        {
            Error(message);

            if (crash)
            {
                Application.Quit();
            }
        }

        public static void Debug_Log(string message)
        {
#if DEBUG
            Msg("[DEBUG] " + message);
#endif
        }

        public static void Debug_Warn(string message)
        {
#if DEBUG
            Warning("[DEBUG] " + message);
#endif
        }

        public static void Debug_Error(string message)
        {
#if DEBUG
            Error("[DEBUG] " + message);
#endif
        }
    }
}
