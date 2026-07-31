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

        #region General

        public static string Label_Enabled => "Label_Enabled".Translate();
        public static string Label_ScreenshotMode => "Label_ScreenshotMode".Translate();

        public static string Label_ShowExample => "Label_ShowExample".Translate();
        public static string Message_ShowExample => "Message_ShowExample".Translate(ModName);

        #endregion

        #region Appearance Section

        public static string Section_Appearance => "Section_Appearance".Translate();

        public static string Label_ShowHighlight => "Label_ShowHighlight".Translate();

        public static string Label_Scale => "Label_Scale".Translate();
        public static readonly float[] ScalePresets = { 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f, 2.5f, 3f, 4f };

        public static string Label_Offset => "Label_Offset".Translate();
        public static string Label_OffsetPresets => "Label_OffsetPresets".Translate();
        public static (TaggedString Name, Vector2 Preset)[] OffsetPresets => new[]
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

        public static string Label_LineWidthPercentage => "Label_LineWidthPercentage".Translate();
        public static string Label_LineThickness => "Label_LineThickness".Translate();
        public static string Label_LinePadding => "Label_LinePadding".Translate();
        public static string Label_LineColor => "Label_LineColor".Translate();

        #endregion

        #region Duration Section

        public static string Section_Duration => "Section_Duration".Translate();

        public static string Label_DisplayDuration => "Label_DisplayDuration".Translate();
        public static string Label_FadeInDuration => "Label_FadeInDuration".Translate();
        public static string Label_FadeOutDuration => "Label_FadeOutDuration".Translate();

        #endregion

        #region Text Section

        public static string Section_Text => "Section_Text".Translate();

        public static string Label_UpperTextProperties => "Label_UpperTextProperties".Translate();
        public static string Label_BottomTextProperties => "Label_BottomTextProperties".Translate();

        public static string Label_DefaultFont => "Label_DefaultFont".Translate();

        public static string Label_FontFamily => "Label_FontFamily".Translate();
        public static string Label_FontSize => "Label_FontSize".Translate();
        public static string Label_Bold => "Label_Bold".Translate();
        public static string Label_Italic => "Label_Italic".Translate();
        public static string Label_TextColor => "Label_TextColor".Translate();
        public static string Label_OutlineThickness => "Label_OutlineThickness".Translate();
        public static string Label_OutlineColor => "Label_OutlineColor".Translate();

        public static string Label_Search => "Label_Search".Translate();

        #endregion

        #region Label Format Section

        public static string Section_LabelFormat => "Section_LabelFormat".Translate();

        public static string Label_UpperTextFormat => "Label_UpperTextFormat".Translate();
        public static string Label_BottomTextFormat => "Label_BottomTextFormat".Translate();

        public static string Hint_LabelFormat =>
            "Hint_DayFormat".Translate() + "\n" +
            "Hint_YearFormat".Translate() + "\n" +
            "Hint_QuadrumFormat".Translate() + "\n" +
            "Hint_SeasonFormat".Translate() + "\n" +
            "Hint_HourFormat".Translate();

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

        #region Sound Section

        public static string Section_Sound => "Section_Sound".Translate();

        public static string Label_Sound => "Label_Sound".Translate();

        public static string Label_SoundVolume => "Label_SoundVolume".Translate();
        public static string Label_SoundPitch => "Label_SoundPitch".Translate();

        public static string DefaultSoundDefName => "DawnSound_MajorasMask";

        #endregion

        #region Extra Section

        public static string Section_Extra => "Section_Extra".Translate();

        public static string Label_StartsAtZero => "Label_StartsAtZero".Translate();
        public static string Label_ShowEveryXDays => "Label_ShowEveryXDays".Translate();
        public static string Label_TriggerHour => "Label_TriggerHour".Translate();

        public static string Label_DayRelativeTo => "Label_DayRelativeTo".Translate();

        public static Dictionary<DayRelative, string> Label_DayRelative => new Dictionary<DayRelative, string>
        {
            { DayRelative.Settle, "Label_DayRelativeSettle".Translate() },
            { DayRelative.Quadrum, "Label_DayRelativeQuadrum".Translate() },
            { DayRelative.Season, "Label_DayRelativeSeason".Translate() },
            { DayRelative.Year, "Label_DayRelativeYear".Translate() }
        };

        public static Dictionary<DayRelative, string> SettingsTooltip_DayRelativeTo => new Dictionary<DayRelative, string>
        {
            { DayRelative.Settle, "Tooltip_Settle".Translate() },
            { DayRelative.Quadrum, "Tooltip_Quadrum".Translate() },
            { DayRelative.Season, "Tooltip_Season".Translate() },
            { DayRelative.Year, "Tooltip_Year".Translate() }
        };

        #endregion

        #endregion
    }
}
