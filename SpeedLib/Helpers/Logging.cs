using System;

namespace SpeedLib.SpeedLib.Helpers
{
    internal static class Logging
    {
        public static void Log(this Exception e) => Svc.Log.Error($"{e.Message}\n{e.StackTrace ?? ""}");

        public static void Log(this Exception e, string ErrorMessage) => Svc.Log.Error($"{ErrorMessage}\n{e.Message}\n{e.StackTrace ?? ""}");
    }
}
