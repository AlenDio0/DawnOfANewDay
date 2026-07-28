using RimWorld;
using System;
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

        public static string SettingsLabel_Enabled => "SettingsLabel_Enabled".Translate();
        public static string SettingsLabel_ScreenshotMode => "SettingsLabel_ScreenshotMode".Translate();

        public static string SettingsLabel_ShowExample => "SettingsLabel_ShowExample".Translate();
        public static string SettingsMessage_ShowExample => "SettingsMessage_ShowExample".Translate(ModName);

        public static Color[] ColorPresets => new[] { Color.white, Color.gray, Color.black, Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.cyan };

        #region Appearance Section

        public static string SettingsSection_Appearance => "SettingsSection_Appearance".Translate();

        public static string SettingsLabel_ShowHighlight => "SettingsLabel_ShowHighlight".Translate();

        public static string SettingsLabel_Scale => "SettingsLabel_Scale".Translate();
        public static readonly float[] ScalePresets = { 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f, 2.5f, 3f, 4f };

        public static string SettingsLabel_Offset => "SettingsLabel_Offset".Translate();
        public static string SettingsLabel_OffsetPresets => "SettingsLabel_OffsetPresets".Translate();
        public static (TaggedString Name, Vector2 Mode)[] OffsetPresets => new[]
        {
            ("Offset_MiddleTop".Translate(), new Vector2(0.5f, 0.2f)),
            ("Offset_MiddleBottom".Translate(), new Vector2(0.5f, 0.8f)),
            ("Offset_MiddleMiddle".Translate(), new Vector2(0.5f, 0.5f)),
            ("Offset_LeftMiddle".Translate(), new Vector2(0.2f, 0.5f)),
            ("Offset_RightMiddle".Translate(), new Vector2(0.8f, 0.5f)),
            ("Offset_LeftTop".Translate(), new Vector2(0.2f, 0.2f)),
            ("Offset_RightTop".Translate(), new Vector2(0.8f, 0.2f)),
            ("Offset_LeftBottom".Translate(), new Vector2(0.2f, 0.8f)),
            ("Offset_RightBottom".Translate(), new Vector2(0.8f, 0.8f)),
        };

        public static string SettingsLabel_LineWidthPercentage => "SettingsLabel_LineWidthPercentage".Translate();
        public static string SettingsLabel_LineThickness => "SettingsLabel_LineThickness".Translate();
        public static string SettingsLabel_LinePadding => "SettingsLabel_LinePadding".Translate();
        public static string SettingsLabel_LineColor => "SettingsLabel_LineColor".Translate();

        #endregion

        #region Duration Section

        public static string SettingsSection_Duration => "SettingsSection_Duration".Translate();

        public static string SettingsLabel_DisplayDuration => "SettingsLabel_DisplayDuration".Translate();
        public static string SettingsLabel_FadeInDuration => "SettingsLabel_FadeInDuration".Translate();
        public static string SettingsLabel_FadeOutDuration => "SettingsLabel_FadeOutDuration".Translate();

        #endregion

        #region Text Section

        public static string SettingsSection_Text => "SettingsSection_Text".Translate();

        public static string SettingsLabel_UpperTextProperties => "SettingsLabel_UpperTextProperties".Translate();
        public static string SettingsLabel_BottomTextProperties => "SettingsLabel_BottomTextProperties".Translate();

        public static string SettingsLabel_DefaultFont => "SettingsLabel_DefaultFont".Translate();

        public static string SettingsLabel_FontFamily => "SettingsLabel_FontFamily".Translate();
        public static string SettingsLabel_FontSize => "SettingsLabel_FontSize".Translate();
        public static string SettingsLabel_Bold => "SettingsLabel_Bold".Translate();
        public static string SettingsLabel_Italic => "SettingsLabel_Italic".Translate();
        public static string SettingsLabel_TextColor => "SettingsLabel_TextColor".Translate();
        public static string SettingsLabel_OutlineThickness => "SettingsLabel_OutlineThickness".Translate();
        public static string SettingsLabel_OutlineColor => "SettingsLabel_OutlineColor".Translate();

        public static string SettingsLabel_Search => "SettingsLabel_Search".Translate();

        #endregion

        #region Label Format Section

        public static string SettingsSection_LabelFormat => "SettingsSection_LabelFormat".Translate();

        public static string SettingsLabel_UpperTextFormat => "SettingsLabel_UpperTextFormat".Translate();
        public static string SettingsLabel_BottomTextFormat => "SettingsLabel_BottomTextFormat".Translate();

        public static string SettingsHint_LabelFormat =>
            "SettingsHint_DayFormat".Translate() + "\n" +
            "SettingsHint_YearFormat".Translate() + "\n" +
            "SettingsHint_QuadrumFormat".Translate() + "\n" +
            "SettingsHint_SeasonFormat".Translate() + "\n" +
            "SettingsHint_HourFormat".Translate();

        public struct FormatContext
        {
            public string Day;
            public string Year;
            public string Quadrum;
            public string Season;
            public string Hour;
        }

        public static Dictionary<string, Func<FormatContext, string>> FormatTokens => new Dictionary<string, Func<FormatContext, string>>
        {
            { "{}",  context => context.Day },
            { "{D}", context => context.Day },
            { "{d}", context => context.Day },

            { "{Y}", context => context.Year },
            { "{y}", context => (context.Year.Length > 2 ? context.Year.Substring(context.Year.Length - 2) : context.Year) },

            { "{Q}", context => context.Quadrum.ToUpper() },
            { "{q}", context => context.Quadrum },

            { "{S}", context => context.Season.ToUpper() },
            { "{s}", context => context.Season },

            { "{H}", context => $"{context.Hour:00}" },
            { "{h}", context => context.Hour }
        };

        #endregion

        #region Extra Section

        public static string SettingsSection_Extra => "SettingsSection_Extra".Translate();

        public static string SettingsLabel_StartsAtZero => "SettingsLabel_StartsAtZero".Translate();
        public static string SettingsLabel_ShowEveryXDays => "SettingsLabel_ShowEveryXDays".Translate();
        public static string SettingsLabel_TriggerHour => "SettingsLabel_TriggerHour".Translate();

        public static string SettingsLabel_DayRelativeTo => "SettingsLabel_DayRelativeTo".Translate();

        public static Dictionary<DayRelative, string> SettingsLabel_DayRelative => new Dictionary<DayRelative, string>
        {
            { DayRelative.Settle, "SettingsLabel_DayRelativeSettle".Translate() },
            { DayRelative.Quadrum, "SettingsLabel_DayRelativeQuadrum".Translate() },
            { DayRelative.Season, "SettingsLabel_DayRelativeSeason".Translate() },
            { DayRelative.Year, "SettingsLabel_DayRelativeYear".Translate() }
        };

        public static Dictionary<DayRelative, string> SettingsTooltip_DayRelativeTo => new Dictionary<DayRelative, string>
        {
            { DayRelative.Settle, "SettingsTooltip_Settle".Translate() },
            { DayRelative.Quadrum, "SettingsTooltip_Quadrum".Translate() },
            { DayRelative.Season, "SettingsTooltip_Season".Translate() },
            { DayRelative.Year, "SettingsTooltip_Year".Translate() }
        };

        #endregion

        #endregion
    }
}
