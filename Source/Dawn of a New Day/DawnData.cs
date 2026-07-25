using Verse;

namespace DawnNewDay
{
    [StaticConstructorOnStartup]
    public static class DawnData
    {
        public static string ModName => "Dawn of a New Day";

        private static string FormatLog(string message) => $"{ModName}: {message}.";
        public static void Warn(string message) => Log.Warning(FormatLog(message));
        public static void Info(string message) => Log.Message(FormatLog(message));
        public static void Error(string message) => Log.Error(FormatLog(message));

        public static string SettingsLabel_Enabled => "Enabled";
        public static string SettingsLabel_StartsAtZero => "Starts at Zero";
        public static string SettingsLabel_ShowHighlight => "Show Highlight";
        public static string SettingsLabel_DayRelativeTo => "Relative To";

        public static string SettingsLabel_ShowExample => "Show Example";
        public static string SettingsMessage_ShowExample => $"'{ModName}' Example will be triggered in the next tick.";

        public static string SettingsLabel_Scale => "Scale";

        public static string SettingsLabel_Offset => "Offset";
        public static string SettingsLabel_SetOffsetCenter => "Offset Presets";

        public static string SettingsLabel_DisplayDuration => "Display Duration";
        public static string SettingsLabel_FadeInDuration => "Fade In Duration";
        public static string SettingsLabel_FadeOutDuration => "Fade Out Duration";

        public static string SettingsLabel_LineWidthPercentage => "Line Width";
        public static string SettingsLabel_LineThickness => "Line Thickness";
        public static string SettingsLabel_LinePadding => "Line Padding";

        public static string SettingsLabel_DayText => "Day Text";
        public static string SettingsLabel_YearText => "Year Text";
        public static string SettingsLabel_DayFontSize => "Day Font Size";
        public static string SettingsLabel_DateFontSize => "Date Font Size";

        public static string SettingsLabel_DayFontStyle => "Day Font Style";
        public static string SettingsLabel_DateFontStyle => "Date Font Style";

        public static string SettingsLabel_DayTextColor => "Day Text Color";
        public static string SettingsLabel_DateTextColor => "Date Text Color";

        public static string SettingsLabel_DayOutlineThickness => "Day Outline Thickness";
        public static string SettingsLabel_DateOutlineThickness => "Date Outline Thickness";

        public static string SettingsLabel_DayOutlineColor => "Day Outline Color";
        public static string SettingsLabel_DateOutlineColor => "Date Outline Color";

        public static string SettingsLabel_ShowEveryXDays => "Show Every X Days";
    }
}
