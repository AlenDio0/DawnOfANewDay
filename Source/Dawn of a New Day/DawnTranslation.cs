using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    [StaticConstructorOnStartup]
    public static class DawnTranslation
    {
        #region Header

        public static string Label_Enabled => "DawnNewDay.Label_Enabled".Translate();
        public static string Label_ScreenshotMode => "DawnNewDay.Label_ScreenshotMode".Translate();

        public static string Label_ShowExample => "DawnNewDay.Label_ShowExample".Translate();
        public static string Message_ShowExample => "DawnNewDay.Message_ShowExample".Translate(DawnData.ModName);

        #endregion

        #region Appearance Section

        public static string Section_Appearance => "DawnNewDay.Section_Appearance".Translate();

        public static string Label_ShowHighlight => "DawnNewDay.Label_ShowHighlight".Translate();

        public static string Label_Scale => "DawnNewDay.Label_Scale".Translate();

        public static string Label_Offset => "DawnNewDay.Label_Offset".Translate();
        public static string Label_OffsetPresets => "DawnNewDay.Label_OffsetPresets".Translate();

        public static (TaggedString Name, Vector2 Preset)[] OffsetPresets =>
        [
            ("DawnNewDay.Offset_MiddleTop".Translate(), new Vector2(0.5f, 0.2f)),
            ("DawnNewDay.Offset_MiddleBottom".Translate(), new Vector2(0.5f, 0.8f)),
            ("DawnNewDay.Offset_MiddleMiddle".Translate(), new Vector2(0.5f, 0.5f)),
            ("DawnNewDay.Offset_LeftMiddle".Translate(), new Vector2(0.2f, 0.5f)),
            ("DawnNewDay.Offset_RightMiddle".Translate(), new Vector2(0.8f, 0.5f)),
            ("DawnNewDay.Offset_LeftTop".Translate(), new Vector2(0.2f, 0.2f)),
            ("DawnNewDay.Offset_RightTop".Translate(), new Vector2(0.8f, 0.2f)),
            ("DawnNewDay.Offset_LeftBottom".Translate(), new Vector2(0.2f, 0.8f)),
            ("DawnNewDay.Offset_RightBottom".Translate(), new Vector2(0.8f, 0.8f)),
        ];

        public static string Label_SubtitleGap => "DawnNewDay.Label_SubtitleGap".Translate();

        public static string Label_LineWidthPercentage => "DawnNewDay.Label_LineWidthPercentage".Translate();
        public static string Label_LineThickness => "DawnNewDay.Label_LineThickness".Translate();
        public static string Label_LinePadding => "DawnNewDay.Label_LinePadding".Translate();
        public static string Label_LineColor => "DawnNewDay.Label_LineColor".Translate();

        #endregion

        #region Duration Section

        public static string Section_Duration => "DawnNewDay.Section_Duration".Translate();

        public static string Label_DisplayDuration => "DawnNewDay.Label_DisplayDuration".Translate();
        public static string Label_FadeInDuration => "DawnNewDay.Label_FadeInDuration".Translate();
        public static string Label_FadeOutDuration => "DawnNewDay.Label_FadeOutDuration".Translate();

        #endregion

        #region Text Section

        public static string Section_Text => "DawnNewDay.Section_Text".Translate();

        public static string Section_UpperText => "DawnNewDay.Section_UpperText".Translate();
        public static string Section_BottomText => "DawnNewDay.Section_BottomText".Translate();
        public static string Section_SubtitleText => "DawnNewDay.Section_SubtitleText".Translate();

        public static string Label_DefaultFont => "DawnNewDay.Label_DefaultFont".Translate();

        public static string Label_FontFamily => "DawnNewDay.Label_FontFamily".Translate();
        public static string Label_FontSize => "DawnNewDay.Label_FontSize".Translate();
        public static string Label_Bold => "DawnNewDay.Label_Bold".Translate();
        public static string Label_Italic => "DawnNewDay.Label_Italic".Translate();
        public static string Label_TextColor => "DawnNewDay.Label_TextColor".Translate();
        public static string Label_OutlineThickness => "DawnNewDay.Label_OutlineThickness".Translate();
        public static string Label_OutlineColor => "DawnNewDay.Label_OutlineColor".Translate();

        public static string Label_Search => "DawnNewDay.Label_Search".Translate();

        #endregion

        #region Label Format Section

        public static string Section_LabelFormat => "DawnNewDay.Section_LabelFormat".Translate();

        public static string Label_UpperTextFormat => "DawnNewDay.Label_UpperTextFormat".Translate();
        public static string Label_BottomTextFormat => "DawnNewDay.Label_BottomTextFormat".Translate();
        public static string Label_SubtitleTextFormat => "DawnNewDay.Label_SubtitleTextFormat".Translate();

        public static string Label_LabelFormatPresets => "DawnNewDay.Label_LabelFormatPresets".Translate();
        public static string Label_LabelFormatHints => "DawnNewDay.Label_LabelFormatHints".Translate();
        public static string Label_RichTextHints => "DawnNewDay.Label_RichTextHints".Translate();

        public static Dictionary<string, string> Hints_LabelFormatPreset => new()
        {
            { "DAY {DAY_SETTLE} <size=30>{HOUR_D2}:00</size>", "DawnNewDay.Hint_Preset_DayAndHour".Translate() },
            { "{DAY_QUADRUM_ORDINAL} of {QUADRUM}, {YEAR}", "DawnNewDay.Hint_Preset_Date".Translate() },
            { "<color={TEMPERATURE_COLOR}>{TEMPERATURE}</color>", "DawnNewDay.Hint_Preset_TemperatureWithColor".Translate() },
            { "{SETTLEMENT} Y{YEAR}, somewhere in the <lower>{BIOME}</lower>", "DawnNewDay.Hint_Preset_SomewherePlace".Translate() },
        };

        public static Dictionary<string, string> Hints_LabelFormat => new()
        {
            { "{DAY_SETTLE}", "DawnNewDay.Hint_Format_DAY_SETTLE".Translate() },
            { "{DAY_QUADRUM}", "DawnNewDay.Hint_Format_DAY_QUADRUM".Translate() },
            { "{DAY_SEASON}", "DawnNewDay.Hint_Format_DAY_SEASON".Translate() },
            { "{DAY_YEAR}", "DawnNewDay.Hint_Format_DAY_YEAR".Translate() },

            { "{DAY_QUADRUM_ORDINAL}", "DawnNewDay.Hint_Format_DAY_QUADRUM_ORDINAL".Translate() },

            { "{YEAR}", "DawnNewDay.Hint_Format_YEAR".Translate() },
            { "{YEAR_D2}", "DawnNewDay.Hint_Format_YEAR_D2".Translate() },

            { "{QUADRUM}", "DawnNewDay.Hint_Format_QUADRUM".Translate() },
            { "{SEASON}", "DawnNewDay.Hint_Format_SEASON".Translate() },

            { "{HOUR}", "DawnNewDay.Hint_Format_HOUR".Translate() },
            { "{HOUR_D2}", "DawnNewDay.Hint_Format_HOUR_D2".Translate() },

            { "{WEATHER}", "DawnNewDay.Hint_Format_WEATHER".Translate() },

            { "{TEMPERATURE}", "DawnNewDay.Hint_Format_TEMPERATURE".Translate() },
            { "{TEMPERATURE_COLOR}", "DawnNewDay.Hint_Format_TEMPERATURE_COLOR".Translate() },

            { "{WORLD}", "DawnNewDay.Hint_Format_WORLD".Translate() },
            { "{BIOME}", "DawnNewDay.Hint_Format_BIOME".Translate() },
            { "{TERRAIN}", "DawnNewDay.Hint_Format_TERRAIN".Translate() },

            { "{ELEVATION}", "DawnNewDay.Hint_Format_ELEVATION".Translate() },
            { "{ELEVATION_KM}", "DawnNewDay.Hint_Format_ELEVATION_KM".Translate() },

            { "{POLLUTION}", "DawnNewDay.Hint_Format_POLLUTION".Translate() },

            { "{CONDITION}", "DawnNewDay.Hint_Format_CONDITION".Translate() },

            { "{FACTION}", "DawnNewDay.Hint_Format_FACTION".Translate() },
            { "{SETTLEMENT}", "DawnNewDay.Hint_Format_SETTLEMENT".Translate() },

            { "{ENDLINE}", "DawnNewDay.Hint_Format_ENDLINE".Translate() },

            { "{UPPER_FONTSIZE}", "DawnNewDay.Hint_Format_UPPER_FONTSIZE".Translate() },
            { "{BOTTOM_FONTSIZE}", "DawnNewDay.Hint_Format_BOTTOM_FONTSIZE".Translate() },
            { "{SUBTITLE_FONTSIZE}", "DawnNewDay.Hint_Format_SUBTITLE_FONTSIZE".Translate() },

            { "{VALUE1 + VALUE2}", "DawnNewDay.Hint_Format_Addition".Translate() },
            { "{VALUE1 - VALUE2}", "DawnNewDay.Hint_Format_Subtraction".Translate() },
            { "{VALUE1 * VALUE2}", "DawnNewDay.Hint_Format_Multiplication".Translate() },
            { "{VALUE1 / VALUE2}", "DawnNewDay.Hint_Format_Division".Translate() },
        };

        public static Dictionary<string, string> Hints_RichText => new()
        {
            { "<color=COLOR_HERE>TEXT_HERE</color>", "DawnNewDay.Hint_RichText_Color".Translate() },
            { "<color=#RRGGBB_HERE>TEXT_HERE</color>", "DawnNewDay.Hint_RichText_ColorRGB".Translate() },
            { "<color=#RRGGBBAA_HERE>TEXT_HERE</color>", "DawnNewDay.Hint_RichText_ColorRGBA".Translate() },

            { "<b>TEXT_HERE</b>", "DawnNewDay.Hint_RichText_Bold".Translate() },
            { "<i>TEXT_HERE</i>", "DawnNewDay.Hint_RichText_Italic".Translate() },
            { "<size=FONTSIZE_HERE>TEXT_HERE</size>", "DawnNewDay.Hint_RichText_FontSize".Translate() },

            { "<title>TEXT_HERE</title>", "DawnNewDay.Hint_RichText_Title".Translate() },
            { "<upper>TEXT_HERE</upper>", "DawnNewDay.Hint_RichText_Upper".Translate() },
            { "<lower>TEXT_HERE</lower>", "DawnNewDay.Hint_RichText_Lower".Translate() },
        };

        #endregion

        #region Sound Section

        public static string Section_Sound => "DawnNewDay.Section_Sound".Translate();

        public static string Label_Sound => "DawnNewDay.Label_Sound".Translate();

        public static string Label_SoundVolume => "DawnNewDay.Label_SoundVolume".Translate();
        public static string Label_SoundPitch => "DawnNewDay.Label_SoundPitch".Translate();

        #endregion

        #region Extra Section

        public static string Section_Extra => "DawnNewDay.Section_Extra".Translate();

        public static string Label_StartsAtZero => "DawnNewDay.Label_StartsAtZero".Translate();
        public static string Label_ShowEveryXDays => "DawnNewDay.Label_ShowEveryXDays".Translate();
        public static string Label_TriggerHour => "DawnNewDay.Label_TriggerHour".Translate();

        #endregion
    }
}
