using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    [StaticConstructorOnStartup]
    public static class DawnTranslation
    {
        private static TaggedString DawnTranslate(string key, params NamedArgument[] args) => $"DawnNewDay.{key}".Translate(args);

        #region Misc

        public static string Label_Search => DawnTranslate("Label_Search");

        public static string Label_Legacy => DawnTranslate("Label_Legacy");

        #endregion

        #region Header

        public static string Label_Enabled => DawnTranslate("Label_Enabled");
        public static string Label_ScreenshotMode => DawnTranslate("Label_ScreenshotMode");

        public static string Label_ShowExample => DawnTranslate("Label_ShowExample");
        public static string Message_ShowExample => DawnTranslate("Message_ShowExample", DawnData.ModName);

        #endregion

        #region Appearance Section

        public static string Section_Appearance => DawnTranslate("Section_Appearance");

        public static string Label_ShowHighlight => DawnTranslate("Label_ShowHighlight");

        public static string Label_Scale => DawnTranslate("Label_Scale");

        public static string Label_Offset => DawnTranslate("Label_Offset");
        public static string Label_OffsetPresets => DawnTranslate("Label_OffsetPresets");

        public static (TaggedString Name, Vector2 Preset)[] OffsetPresets =>
        [
            (DawnTranslate("Offset_MiddleTop"), new(0.5f, 0.2f)),
            (DawnTranslate("Offset_MiddleBottom"), new(0.5f, 0.8f)),
            (DawnTranslate("Offset_MiddleMiddle"), new(0.5f, 0.5f)),
            (DawnTranslate("Offset_LeftMiddle"), new(0.2f, 0.5f)),
            (DawnTranslate("Offset_RightMiddle"), new(0.8f, 0.5f)),
            (DawnTranslate("Offset_LeftTop"), new(0.2f, 0.2f)),
            (DawnTranslate("Offset_RightTop"), new(0.8f, 0.2f)),
            (DawnTranslate("Offset_LeftBottom"), new(0.2f, 0.8f)),
            (DawnTranslate("Offset_RightBottom"), new(0.8f, 0.8f)),
        ];

        #endregion

        #region Duration Section

        public static string Section_Duration => DawnTranslate("Section_Duration");

        public static string Label_DisplayDuration => DawnTranslate("Label_DisplayDuration");
        public static string Label_FadeInDuration => DawnTranslate("Label_FadeInDuration");
        public static string Label_FadeOutDuration => DawnTranslate("Label_FadeOutDuration");

        #endregion

        #region Overlay Widget Section

        public static string Section_OverlayWidget => DawnTranslate("Section_OverlayWidget");

        public static string Label_Text => DawnTranslate("Label_Text");
        public static string Label_Separator => DawnTranslate("Label_Separator");

        public static string Message_ResetWarning => DawnTranslate("Message_ResetWarning");
        public static string Message_DeleteWarning => DawnTranslate("Message_DeleteWarning");

        public static string Label_BackupFormats => DawnTranslate("Label_BackupFormats");
        public static string Description_BackupFormat(string index) => DawnTranslate("Description_BackupFormat", index);

        public static string Label_DefaultFont => DawnTranslate("Label_DefaultFont");
        public static string Label_FontFamily => DawnTranslate("Label_FontFamily");
        public static string Label_FontSize => DawnTranslate("Label_FontSize");
        public static string Label_Bold => DawnTranslate("Label_Bold");
        public static string Label_Italic => DawnTranslate("Label_Italic");
        public static string Label_TextColor => DawnTranslate("Label_TextColor");
        public static string Label_OutlineThickness => DawnTranslate("Label_OutlineThickness");
        public static string Label_OutlineColor => DawnTranslate("Label_OutlineColor");

        public static string Label_WidthPercentage => DawnTranslate("Label_WidthPercentage");
        public static string Label_Thickness => DawnTranslate("Label_Thickness");
        public static string Label_Padding => DawnTranslate("Label_Padding");
        public static string Label_Color => DawnTranslate("Label_Color");

        #endregion

        #region Label Format Section

        public static string Label_TextFormat => DawnTranslate("Label_TextFormat");

        public static string Label_UpperTextFormat => DawnTranslate("Label_UpperTextFormat");
        public static string Label_BottomTextFormat => DawnTranslate("Label_BottomTextFormat");
        public static string Label_SubtitleTextFormat => DawnTranslate("Label_SubtitleTextFormat");

        public static string Label_LabelFormatPresets => DawnTranslate("Label_LabelFormatPresets");
        public static string Label_LabelFormatHints => DawnTranslate("Label_LabelFormatHints");
        public static string Label_RichTextHints => DawnTranslate("Label_RichTextHints");

        public static Dictionary<string, string> Hints_LabelFormatPresets => new()
        {
            { "DAY {DAY_SETTLE} <size=30>{HOUR_D2}:00</size>", DawnTranslate("Hint_Preset_DayAndHour") },
            { "{DAY_QUADRUM_ORDINAL} of {QUADRUM}, {YEAR}", DawnTranslate("Hint_Preset_Date") },
            { "<color={TEMPERATURE_COLOR}>{TEMPERATURE}</color>", DawnTranslate("Hint_Preset_TemperatureWithColor") },
            { "{SETTLEMENT} Y{YEAR}, somewhere in the <lower>{BIOME}</lower>", DawnTranslate("Hint_Preset_SomewherePlace") },
            { DawnDefault.UpperTextFormat, DawnTranslate("Hint_Preset_UpperFormatDefault") },
            { DawnDefault.BottomTextFormat, DawnTranslate("Hint_Preset_BottomFormatDefault") },
            { DawnDefault.SubtitleTextFormat, DawnTranslate("Hint_Preset_SubtitleFormatDefault") },
        };

        public static Dictionary<string, string> Hints_LabelFormat => new()
        {
            { "{DAY_SETTLE}", DawnTranslate("Hint_Format_DAY_SETTLE") },
            { "{DAY_QUADRUM}", DawnTranslate("Hint_Format_DAY_QUADRUM") },
            { "{DAY_SEASON}", DawnTranslate("Hint_Format_DAY_SEASON") },
            { "{DAY_YEAR}", DawnTranslate("Hint_Format_DAY_YEAR") },

            { "{DAY_QUADRUM_ORDINAL}", DawnTranslate("Hint_Format_DAY_QUADRUM_ORDINAL") },

            { "{YEAR}", DawnTranslate("Hint_Format_YEAR") },
            { "{YEAR_D2}", DawnTranslate("Hint_Format_YEAR_D2") },

            { "{QUADRUM}", DawnTranslate("Hint_Format_QUADRUM") },
            { "{SEASON}", DawnTranslate("Hint_Format_SEASON") },

            { "{HOUR}", DawnTranslate("Hint_Format_HOUR") },
            { "{HOUR_D2}", DawnTranslate("Hint_Format_HOUR_D2") },

            { "{WEATHER}", DawnTranslate("Hint_Format_WEATHER") },

            { "{TEMPERATURE}", DawnTranslate("Hint_Format_TEMPERATURE") },
            { "{TEMPERATURE_COLOR}", DawnTranslate("Hint_Format_TEMPERATURE_COLOR") },

            { "{WORLD}", DawnTranslate("Hint_Format_WORLD") },
            { "{BIOME}", DawnTranslate("Hint_Format_BIOME") },
            { "{TERRAIN}", DawnTranslate("Hint_Format_TERRAIN") },

            { "{ELEVATION}", DawnTranslate("Hint_Format_ELEVATION") },
            { "{ELEVATION_KM}", DawnTranslate("Hint_Format_ELEVATION_KM") },

            { "{POLLUTION}", DawnTranslate("Hint_Format_POLLUTION") },

            { "{CONDITION}", DawnTranslate("Hint_Format_CONDITION") },

            { "{FACTION}", DawnTranslate("Hint_Format_FACTION") },
            { "{SETTLEMENT}", DawnTranslate("Hint_Format_SETTLEMENT") },

            { "{ENDLINE}", DawnTranslate("Hint_Format_ENDLINE") },

            { "{FONTSIZE}", DawnTranslate("Hint_Format_FONTSIZE") },

            { "{VALUE1 + VALUE2}", DawnTranslate("Hint_Format_Addition") },
            { "{VALUE1 - VALUE2}", DawnTranslate("Hint_Format_Subtraction") },
            { "{VALUE1 * VALUE2}", DawnTranslate("Hint_Format_Multiplication") },
            { "{VALUE1 / VALUE2}", DawnTranslate("Hint_Format_Division") },
        };

        public static Dictionary<string, string> Hints_RichText => new()
        {
            { "<color=COLOR_HERE>TEXT_HERE</color>", DawnTranslate("Hint_RichText_Color") },
            { "<color=#RRGGBB_HERE>TEXT_HERE</color>", DawnTranslate("Hint_RichText_ColorRGB") },
            { "<color=#RRGGBBAA_HERE>TEXT_HERE</color>", DawnTranslate("Hint_RichText_ColorRGBA") },

            { "<b>TEXT_HERE</b>", DawnTranslate("Hint_RichText_Bold") },
            { "<i>TEXT_HERE</i>", DawnTranslate("Hint_RichText_Italic") },
            { "<size=FONTSIZE_HERE>TEXT_HERE</size>", DawnTranslate("Hint_RichText_FontSize") },

            { "<title>TEXT_HERE</title>", DawnTranslate("Hint_RichText_Title") },
            { "<upper>TEXT_HERE</upper>", DawnTranslate("Hint_RichText_Upper") },
            { "<lower>TEXT_HERE</lower>", DawnTranslate("Hint_RichText_Lower") },

            { "<ifnotempty=CONTENT_HERE>TEXT_HERE</ifnotempty>", DawnTranslate("Hint_RichText_IfNotEmpty") },
            { "<ifempty=CONTENT_HERE>TEXT_HERE</ifempty>", DawnTranslate("Hint_RichText_IfEmpty") },
        };

        #endregion

        #region Sound Section

        public static string Section_Sound => DawnTranslate("Section_Sound");

        public static string Label_Sound => DawnTranslate("Label_Sound");

        public static string Label_SoundVolume => DawnTranslate("Label_SoundVolume");
        public static string Label_SoundPitch => DawnTranslate("Label_SoundPitch");

        #endregion

        #region Extra Section

        public static string Section_Extra => DawnTranslate("Section_Extra");

        public static string Label_StartsAtZero => DawnTranslate("Label_StartsAtZero");
        public static string Label_ShowEveryXDays => DawnTranslate("Label_ShowEveryXDays");
        public static string Label_TriggerHour => DawnTranslate("Label_TriggerHour");

        #endregion

        #region Compatibility Section

        public static string Section_Compatibility => DawnTranslate("Section_Compatibility");

        public static string Label_ModIncompatibility => DawnTranslate("Label_ModIncompatibility");

        public static string Label_AddText => DawnTranslate("Label_AddText");
        public static string Label_MaximumDays => DawnTranslate("Label_MaximumDays");

        #region Modern Notifications Section

        public static readonly string MN_ModName = "Modern Notifications";
        private static string MN_DawnTranslate(string key) => $"[{MN_ModName}] {DawnTranslate(key)}";

        public static string Label_MN_ExcludeOccasions => DawnTranslate("Label_MN_ExcludeOccasions");

        public static string Label_MN_LabelFormatPresets => MN_DawnTranslate("Label_LabelFormatPresets");
        public static Dictionary<string, string> Hints_MN_LabelFormatPresets => new()
        {
            { "<color={MN_REMINDER_COLOR}>-{MN_REMINDER_REMAINING} Remain-</color>", MN_DawnTranslate("Hint_MN_Preset_ReminderRemainWithSeverityColor") },
            { "<color={MN_REMINDER_SEVERITY_COLOR}>-{MN_REMINDER_REMAINING} Remain-</color>", MN_DawnTranslate("Hint_MN_Preset_ReminderRemainWithTimeColor") },
            { "<color={MN_OCCASION_REMAINING_COLOR}>-{MN_OCCASION_REMAINING} Remain-</color>", MN_DawnTranslate("Hint_MN_Preset_OccasionRemainWithTimeColor") },
            { DawnDefault.MN_ReminderTextFormat, MN_DawnTranslate("Hint_MN_Preset_ReminderTextFormat") },
            { DawnDefault.MN_OccasionTextFormat, MN_DawnTranslate("Hint_MN_Preset_OccasionTextFormat") },
        };

        public static string Label_MN_LabelFormatHints => MN_DawnTranslate("Label_LabelFormatHints");
        public static Dictionary<string, string> Hints_MN_LabelFormat => new()
        {
            { "{MN_REMINDER_TITLE}", MN_DawnTranslate("Hint_MN_REMINDER_TITLE") },
            { "{MN_REMINDER_MESSAGE}", MN_DawnTranslate("Hint_MN_REMINDER_MESSAGE") },
            { "{MN_REMINDER_DAY_YEAR}", MN_DawnTranslate("Hint_MN_REMINDER_DAY_YEAR") },

            { "{MN_REMINDER_SEVERITY_COLOR}", MN_DawnTranslate("Hint_MN_REMINDER_SEVERITY_COLOR") },

            { "{MN_REMINDER_REMAINING}", MN_DawnTranslate("Hint_MN_REMINDER_REMAINING") },
            { "{MN_REMINDER_REMAINING_DAY}", MN_DawnTranslate("Hint_MN_REMINDER_REMAINING_DAY") },
            { "{MN_REMINDER_REMAINING_HOUR}", MN_DawnTranslate("Hint_MN_REMINDER_REMAINING_HOUR") },

            { "{MN_REMINDER_REMAINING_COLOR}", MN_DawnTranslate("Hint_MN_REMINDER_REMAINING_COLOR") },

            { "{MN_OCCASION_CATEGORY}", MN_DawnTranslate("Hint_MN_OCCASION_CATEGORY") },
            { "{MN_OCCASION_LABEL}", MN_DawnTranslate("Hint_MN_OCCASION_LABEL") },
            { "{MN_OCCASION_DETAIL}", MN_DawnTranslate("Hint_MN_OCCASION_DETAIL") },
            { "{MN_OCCASION_DAY_YEAR}", MN_DawnTranslate("Hint_MN_OCCASION_DAY_YEAR") },

            { "{MN_OCCASION_REMAINING}", MN_DawnTranslate("Hint_MN_OCCASION_REMAINING") },
            { "{MN_OCCASION_REMAINING_DAY}", MN_DawnTranslate("Hint_MN_OCCASION_REMAINING_DAY") },
            { "{MN_OCCASION_REMAINING_HOUR}", MN_DawnTranslate("Hint_MN_OCCASION_REMAINING_HOUR") },

            { "{MN_OCCASION_REMAINING_COLOR}", MN_DawnTranslate("Hint_MN_OCCASION_REMAINING_COLOR") },

            { "{MN_REMINDER_FONTSIZE}", MN_DawnTranslate("Hint_MN_REMINDER_FONTSIZE") },
            { "{MN_OCCASION_FONTSIZE}", MN_DawnTranslate("Hint_MN_OCCASION_FONTSIZE") },
        };

        public static string Section_MN_Reminder => DawnTranslate("Section_MN_Reminder");
        public static string Section_MN_Occasion => DawnTranslate("Section_MN_Occasion");

        #endregion

        #endregion
    }
}
