using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public enum DayRelative
    {
        Settle,
        Quadrum,
        Season,
        Year
    }

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

        public static string SettingsLabel_Enabled => "Enabled";
        public static string SettingsLabel_ScreenshotMode => "Screenshot Mode";

        public static string SettingsLabel_ShowExample => "Show Example";
        public static string SettingsMessage_ShowExample => $"'{ModName}' Example will be triggered in the next tick.";

        public static readonly (string Name, Color Color)[] ColorPresets = new[]
        {
            ("White", Color.white),
            ("Gray", Color.gray),
            ("Black", Color.black),
            ("Red", Color.red),
            ("Green", Color.green),
            ("Blue", Color.blue),
            ("Yellow", Color.yellow),
            ("Magenta", Color.magenta),
            ("Cyan", Color.cyan)
        };

        #region Appearance Section

        public static string SettingsSection_Appearance = "Appearance";

        public static string SettingsLabel_ShowHighlight => "Show Highlight";

        public static string SettingsLabel_Scale => "Scale";
        public static readonly float[] ScaleModes = { 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f, 2.5f, 3f, 4f };

        public static string SettingsLabel_Offset => "Offset";
        public static string SettingsLabel_OffsetPresets => "Offset Presets";
        public static readonly (string Name, Vector2 Mode)[] OffsetPresets = new[]
        {
            ("(Middle, Top)     ↑", new Vector2(0.5f, 0.2f)),
            ("(Middle, Bottom)  ↓", new Vector2(0.5f, 0.8f)),
            ("(Middle, Middle)  ▪", new Vector2(0.5f, 0.5f)),
            ("(Left, Middle)    ←", new Vector2(0.2f, 0.5f)),
            ("(Right, Middle)   →", new Vector2(0.8f, 0.5f)),
            ("(Left, Top)       ↖", new Vector2(0.2f, 0.2f)),
            ("(Right, Top)      ↗", new Vector2(0.8f, 0.2f)),
            ("(Left, Bottom)    ↙", new Vector2(0.2f, 0.8f)),
            ("(Right, Bottom)   ↘", new Vector2(0.8f, 0.8f)),
        };

        public static string SettingsLabel_LineWidthPercentage => "Line Width";
        public static string SettingsLabel_LineThickness => "Line Thickness";
        public static string SettingsLabel_LinePadding => "Line Padding";
        public static string SettingsLabel_LineColor => "Line Color";

        #endregion

        #region Duration Section

        public static string SettingsSection_Duration = "Duration";

        public static string SettingsLabel_DisplayDuration => "Total Display Duration";
        public static string SettingsLabel_FadeInDuration => "Fade In Duration";
        public static string SettingsLabel_FadeOutDuration => "Fade Out Duration";

        #endregion

        #region Text Section

        public static string SettingsSection_Text = "Text";

        public static string SettingsLabel_UpperTextProperties => "Upper Text Properties";
        public static string SettingsLabel_BottomTextProperties => "Bottom Text Properties";

        public static string SettingsLabel_DefaultFont => "Default Game Font";

        public static string SettingsLabel_FontFamily => "Choose Font Family";
        public static string SettingsLabel_FontSize => "Font Size";
        public static string SettingsLabel_Bold => "Bold";
        public static string SettingsLabel_Italic => "Italic";
        public static string SettingsLabel_TextColor => "Text Color";
        public static string SettingsLabel_OutlineThickness => "Outline Thickness";
        public static string SettingsLabel_OutlineColor => "Outline Color";

        public static string SettingsLabel_Search => "Search";

        #endregion

        #region Label Format Section

        public static string SettingsSection_LabelFormat = "Label Format";

        public static string SettingsLabel_UpperTextFormat => "Upper Text Format";
        public static string SettingsLabel_BottomTextFormat => "Bottom Text Format";

        public static string SettingsHint_LabelFormat =>
@"{D/d} => Full Day           
{Y/y} => Full Year [eg. 5500] / 2-Digit Year [eg. 00]
{Q/q} => CAPS Quadrum / Quadrum
{S/s} => CAPS Season / Season
{H/h} => Full Hour [eg. 06] / Hour [eg. 6]";

        #endregion

        #region Extra Section

        public static string SettingsSection_Extra = "Extra";

        public static string SettingsLabel_StartsAtZero => "Starts at Zero";
        public static string SettingsLabel_ShowEveryXDays => "Show Every X Days";
        public static string SettingsLabel_TriggerHour => "Trigger Hour";

        public static string SettingsLabel_DayRelativeTo => "Day Relative";
        public static readonly Dictionary<DayRelative, string> SettingsTooltip_DayRelativeTo = new Dictionary<DayRelative, string>
        {
            { DayRelative.Settle, "Day will be relative to the day the colonists settled."},
            { DayRelative.Quadrum, "Day will be relative to the quadrum resetting itself after 15 days."},
            { DayRelative.Season, "Day will be relative to the season resetting itself after it ends."},
            { DayRelative.Year, "Day will be relative to the year resetting itself after 60 days."}
        };

        #endregion

        #endregion
    }
}
