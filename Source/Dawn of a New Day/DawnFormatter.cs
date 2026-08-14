using DawnNewDay.Compatibility;
using DawnNewDay.Utils;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public enum DayRelative
    {
        Settle,
        Quadrum,
        Season,
        Year,
    }

    public struct FormatContext
    {
        public int AbsTicks;

        public bool StartAtZero;

        public World World;
        public Map Map;
        public List<Map> AllMaps;

        public readonly Vector2 Location => World.grid?.LongLatOf(Map.Tile) ?? Vector2.zero;

        #region Date

        public readonly int GenDay(DayRelative dayRelative)
        {
            int startsAtZero = (StartAtZero ? 0 : 1);
            return dayRelative switch
            {
                DayRelative.Settle => GenDate.DaysPassed + (GenDate.HourOfDay(AbsTicks, Location.x) >= 6 ? 0 : 1) + startsAtZero,
                DayRelative.Quadrum => GenDate.DayOfQuadrum(AbsTicks, Location.x) + 1,
                DayRelative.Season => GenDate.DayOfSeason(AbsTicks, Location.x) + startsAtZero,
                DayRelative.Year => GenDate.DayOfYear(AbsTicks, Location.x) + startsAtZero,

                _ => -1,
            };
        }
        public readonly int GenYear() => GenDate.Year(AbsTicks, Location.x);
        public readonly string GenQuadrum() => GenDate.Quadrum(AbsTicks, Location.x).Label();
        public readonly string GenSeason() => GenDate.Season(AbsTicks, Location).LabelCap();
        public readonly int GenHour() => GenDate.HourOfDay(AbsTicks, Location.x);

        #endregion

        #region Tile Info

        public readonly float Temperature => Map.mapTemperature?.OutdoorTemp ?? 0f;
        public readonly string TemperatureHex
        {
            get
            {
                Color coldColor = new(0.2f, 0.7f, 1f);
                Color hotColor = new(1f, 0.3f, 0f);

                float t = Mathf.InverseLerp(-10f, 50f, Temperature);
                Color temperatureColor = Color.Lerp(coldColor, hotColor, t);

                return "#" + ColorUtility.ToHtmlStringRGB(temperatureColor);
            }
        }

        public readonly Tile Tile => World.grid[Map.Tile];

        public readonly string Terrain => Tile?.hilliness.GetLabel() ?? "";
        public readonly float Elevation => Tile?.elevation ?? 0f;
        public readonly float Pollution => Tile?.pollution ?? 0f;

        public readonly List<GameCondition> ActiveConditions => Map.GameConditionManager?.ActiveConditions ?? [];
        public readonly GameCondition ActiveCondition => ActiveConditions.FirstOrFallback(null);

        public readonly string FactionName
        {
            get
            {
                if (Map.ParentFaction != null)
                    return Map.ParentFaction.Name;

                Faction firstFaction = AllMaps.FirstOrFallback(map => map.ParentFaction != null)?.ParentFaction;
                if (firstFaction != null)
                    return firstFaction.Name;

                return "Faction";
            }
        }
        public readonly string SettlementName
        {
            get
            {
                if (Map.Parent is Settlement settlement)
                    return settlement.Name;

#if RW_15
#else
                if (Map.Parent is Camp camp)
                    return camp.LabelCap;
#endif

                if (AllMaps.FirstOrFallback(map => map.Parent is Settlement settlement && !settlement.Name.NullOrEmpty())?.Parent is Settlement firstSettlement)
                    return firstSettlement.Name;

#if RW_15
#else
                if (AllMaps.FirstOrFallback(map => map.Parent is Camp camp && !camp.LabelCap.NullOrEmpty())?.Parent is Camp firstCamp)
                    return firstCamp.LabelCap;
#endif

                return "Settlement";
            }
        }

        #endregion

        #region Settings

        public readonly DawnSettings Settings => DawnMod.Settings;

        public readonly int UpperFontSize => Mathf.CeilToInt(Settings.UpperTextStyle.FontSize * Settings.Scale);
        public readonly int BottomFontSize => Mathf.CeilToInt(Settings.BottomTextStyle.FontSize * Settings.Scale);
        public readonly int SubtitleFontSize => Mathf.CeilToInt(Settings.SubtitleTextStyle.FontSize * Settings.Scale);

        #endregion

        #region Compatibility

        #region Modern Notifications

        public readonly ModernNotificationUtility.Reminder NextReminder
        {
            get
            {
                if (!ModernNotifications.Present)
                    return new();

                var reminders = ModernNotificationUtility.GetReminders();

                ModernNotificationUtility.Reminder nextReminder = new();
                foreach (ModernNotificationUtility.Reminder reminder in reminders)
                {
                    if (!reminder.IsValid || reminder.Fired)
                        continue;

                    if (!nextReminder.IsValid)
                    {
                        nextReminder = reminder;
                        continue;
                    }

                    if (nextReminder.Year > reminder.Year)
                        nextReminder = reminder;
                    else if (nextReminder.DayOfYear > reminder.DayOfYear)
                        nextReminder = reminder;
                }

                return nextReminder;
            }
        }

        public readonly string ReminderRemaining
        {
            get
            {
                const int cToleranceHour = 72;

                int remainingHour = ReminderRemainingHour;
                int remainingTicks = remainingHour * GenDate.TicksPerHour;

                return remainingHour > cToleranceHour ? remainingTicks.ToStringTicksToPeriod(true, false, false) : $"{remainingHour}h";
            }
        }
        public readonly float ReminderRemainingDay(int remainingHour)
        {
            float remainingDay = remainingHour / 24f;
            return remainingDay >= 2f ? Mathf.Ceil(remainingDay) : SettingsHelper.SnapToStep(remainingDay, 0.1f);
        }
        public readonly int ReminderRemainingHour
        {
            get
            {
                const int cYearLengthDay = 60;
                const int cDayLengthHour = 24;

                var reminder = NextReminder;
                int currentDayOfYear = ModernNotifications.Today();
                int currentYear = ModernNotifications.Year();
                int currentHour = GenHour();

                int yearsInDays = (reminder.Year - currentYear) * cYearLengthDay;
                int remainingHours = (reminder.DayOfYear - currentDayOfYear + yearsInDays) * cDayLengthHour - currentHour;

                return Mathf.Max(remainingHours, 0);
            }
        }

        #endregion

        #endregion
    }

    public class DawnFormatter(Game game)
    {
        public Game Game { get; } = game;

        public FormatContext CreateFormatContext(bool startsAtZero)
        {
            return new FormatContext
            {
                AbsTicks = Game.tickManager.TicksAbs,

                World = Game.World,
                Map = Game.CurrentMap,
                AllMaps = Game.Maps,

                StartAtZero = startsAtZero,
            };
        }
    }

    public static class DawnFormatterUtility
    {
        private static readonly Regex TokenFormatRegex = new(@"\{(?<token>[^{}]+)\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Dictionary<string, Func<FormatContext, string>> FormatTokens = new(StringComparer.OrdinalIgnoreCase)
        {
            { "DAY",  context => context.GenDay(DayRelative.Settle).ToString() },
            { "DAY_SETTLE",  context => context.GenDay(DayRelative.Settle).ToString() },
            { "DAY_QUADRUM",  context => context.GenDay(DayRelative.Quadrum).ToString() },
            { "DAY_QUADRUM_ORDINAL", context => Find.ActiveLanguageWorker.OrdinalNumber(context.GenDay(DayRelative.Quadrum)) },
            { "DAY_SEASON",  context => context.GenDay(DayRelative.Season).ToString() },
            { "DAY_YEAR",  context => context.GenDay(DayRelative.Year).ToString() },

            { "YEAR", context => context.GenYear().ToString() },
            { "YEAR_D2", context => (context.GenYear() % 100).ToString("D2") },

            { "QUADRUM", context => context.GenQuadrum() },
            { "SEASON", context => context.GenSeason() },

            { "HOUR", context => context.GenHour().ToString() },
            { "HOUR_D2", context => context.GenHour().ToString("D2") },

            { "WEATHER", context => context.Map.weatherManager.curWeather.LabelCap },

            { "TEMPERATURE", context => context.Temperature.ToStringTemperature() },
            { "TEMPERATURE_COLOR", context => context.TemperatureHex },

            { "WORLD", context => context.World.info.name },
            { "BIOME", context => context.Map.Biome.LabelCap },
            { "TERRAIN", context => context.Terrain },

            { "ELEVATION", context => $"{Mathf.CeilToInt(context.Elevation)}m" },
            { "ELEVATION_KM", context => $"{context.Elevation / 1000f:F2}km" },

            { "POLLUTION", context => context.Pollution.ToStringPercent() },

            { "CONDITION", context => context.ActiveCondition?.LabelCap },
            { "CONDITIONS", context => string.Join(", ", context.ActiveConditions.Select(condition => condition.LabelCap)) },

            { "FACTION", context => context.FactionName },
            { "SETTLEMENT", context => context.SettlementName },

            { "ENDLINE", _ => "\n" },

            { "UPPER_FONTSIZE", context => context.UpperFontSize.ToString() },
            { "BOTTOM_FONTSIZE", context => context.BottomFontSize.ToString() },
            { "SUBTITLE_FONTSIZE", context => context.SubtitleFontSize.ToString() },
        };

        #region Compatibility

        #region Modern Notifications

        private static readonly Dictionary<string, Func<FormatContext, string>> MN_FormatTokens = new(StringComparer.OrdinalIgnoreCase)
        {
            { "MN_REMINDER_TITLE", context => context.NextReminder.Title },
            { "MN_REMINDER_DAY_YEAR", context => context.NextReminder.DayOfYear.ToString() },

            { "MN_REMINDER_REMAINING", context => context.ReminderRemaining.ToString() },
            { "MN_REMINDER_REMAINING_DAY", context => context.ReminderRemainingDay(context.ReminderRemainingHour).ToString() },
            { "MN_REMINDER_REMAINING_HOUR", context => context.ReminderRemainingHour.ToString() },
        };

        #endregion

        #endregion

        private static readonly Regex ExtraRichTextTagRegex = new(@"<(?<tag>\w+)>(?<content>.*?)</\1>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Dictionary<string, Func<string, string>> ExtraRichTextTags = new(StringComparer.OrdinalIgnoreCase)
        {
            { "title", text => Find.ActiveLanguageWorker.ToTitleCase(text.ToLower())},
            { "upper", text => text.ToUpper() },
            { "lower", text => text.ToLower() },
        };

        private readonly struct ParsedOperand
        {
            public float Value { get; }
            public string Prefix { get; }
            public string Suffix { get; }
            public bool IsValid { get; }

            private static readonly Regex OperandValueRegex = new(@"[-+]?\d+(?:[.,]\d+)?", RegexOptions.Compiled);

            private ParsedOperand(bool isValid)
            {
                Value = 0f;
                Prefix = "";
                Suffix = "";
                IsValid = isValid;
            }

            private ParsedOperand(float value, string prefix, string suffix)
            {
                Value = value;
                Prefix = prefix;
                Suffix = suffix;
                IsValid = true;
            }

            public static ParsedOperand Parse(string text, FormatContext context)
            {
                string format = text.Replace(',', '.');

                if (FormatTokens.TryGetValue(format, out var replacer))
                    format = replacer?.Invoke(context);

                Match match = OperandValueRegex.Match(format);
                if (match.Success)
                {
                    string number = match.Value.Trim();

                    if (float.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out float value))
                    {
                        string prefix = format.Substring(0, match.Index);
                        string suffix = format.Substring(match.Index + match.Length);

                        return new ParsedOperand(value, prefix, suffix);
                    }
                }

                return new ParsedOperand(false);
            }

            public string Format(float newValue) => $"{Prefix}{Mathf.RoundToInt(newValue)}{Suffix}";
        }

        private static string TokenFormatText(FormatContext context, string text)
        {
            return TokenFormatRegex.Replace(text, match =>
            {
                string token = match.Groups["token"].Value.Trim();

                int operatorIndex = token.IndexOfAny(['+', '-', '*', '/']);
                bool isOperation = operatorIndex >= 0;

                if (!isOperation)
                {
                    if (TryReplaceToken(context, token, out string value))
                        return value;

                    return match.Value;
                }

                string leftPart = token.Substring(0, operatorIndex).Trim();
                char operation = token.ElementAt(operatorIndex);
                string rightPart = token.Substring(operatorIndex + 1).Trim();

                var leftOperand = ParsedOperand.Parse(leftPart, context);
                var rightOperand = ParsedOperand.Parse(rightPart, context);

                if (leftOperand.IsValid && rightOperand.IsValid)
                {
                    float result = CalcStringMath(leftOperand.Value, rightOperand.Value, operation);

                    var targetOperand = (!leftOperand.Prefix.NullOrEmpty() || !leftOperand.Suffix.NullOrEmpty()) ? leftOperand : rightOperand;
                    return targetOperand.Format(result);
                }

                if (TryReplaceToken(context, token, out string leftValue))
                    return leftValue;
                if (TryReplaceToken(context, token, out string rightValue))
                    return rightValue;

                return match.Value;
            });
        }

        private static bool TryReplaceToken(FormatContext context, string token, out string value)
        {
            if (FormatTokens.TryGetValue(token, out var replacer))
            {
                value = replacer?.Invoke(context);
                return true;
            }

            if (ModernNotifications.Present && MN_FormatTokens.TryGetValue(token, out var mnReplacer))
            {
                value = mnReplacer?.Invoke(context);
                return true;
            }

            value = "";
            return false;
        }

        private static float CalcStringMath(float left, float right, char operation) => operation switch
        {
            '+' => left + right,
            '-' => left - right,
            '*' => left * right,
            '/' => right != 0f ? left / right : 0f,

            _ => 0,
        };

        private static string ExtraRichTextTagText(string text)
        {
            return ExtraRichTextTagRegex.Replace(text, match =>
            {
                string tag = match.Groups["tag"].Value;
                string content = match.Groups["content"].Value;

                if (ExtraRichTextTags.TryGetValue(tag, out var replacer))
                    return replacer?.Invoke(content);

                return match.Value;
            });
        }


        public static string FormatText(this FormatContext context, string text)
        {
            if (text.NullOrEmpty())
                return text;

            string format = "";
            try
            {
                format = TokenFormatText(context, text);
                format = ExtraRichTextTagText(format);
                return format;
            }
            catch (Exception exception)
            {
                DawnData.Exception(exception);
                return format;
            }
        }
    }
}
