using Verse;

namespace DawnNewDay
{
    [StaticConstructorOnStartup]
    public static class DawnData
    {
        public static string ModName => "Dawn of a New Day";

        #region Logging

        private static string FormatLog(string message) => $"[{ModName}] {message}.";
        public static void Warn(string message) => Log.Warning(FormatLog(message));
        public static void Info(string message) => Log.Message(FormatLog(message));
        public static void Error(string message) => Log.Error(FormatLog(message));

        #endregion

        #region Settings

        #region Appearance Section

        public static readonly float[] ScalePresets = [0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f, 2.5f, 3f, 4f];

        #endregion

        #region Sound Section

        public static string DefaultSoundDefName => "DawnSound_MajorasMask";

        #endregion

        #endregion
    }
}
