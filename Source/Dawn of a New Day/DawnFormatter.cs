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
    public struct FormatContext
    {
        public int AbsTicks;

        public bool StartAtZero;

        public World World;
        public Map Map;
        public Vector2 Location;

        public int GenDay(DayRelative dayRelative)
        {
            int startsAtZero = (StartAtZero ? 0 : 1);

            int day = 0;
            switch (dayRelative)
            {
                case DayRelative.Settle:
                    day = GenDate.DaysPassed + (GenDate.HourOfDay(AbsTicks, Location.x) >= 6 ? 0 : 1) + startsAtZero;
                    break;
                case DayRelative.Quadrum:
                    day = GenDate.DayOfQuadrum(AbsTicks, Location.x) + 1;
                    break;
                case DayRelative.Season:
                    day = GenDate.DayOfSeason(AbsTicks, Location.x) + startsAtZero;
                    break;
                case DayRelative.Year:
                    day = GenDate.DayOfYear(AbsTicks, Location.x) + startsAtZero;
                    break;

                default:
                    break;
            }

            return day;
        }
        public int GenYear() => GenDate.Year(AbsTicks, Location.x);
        public string GenQuadrum() => GenDate.Quadrum(AbsTicks, Location.x).Label();
        public string GenSeason() => GenDate.Season(AbsTicks, Location).LabelCap();
        public int GenHour() => GenDate.HourOfDay(AbsTicks, Location.x);

        public float Temperature => Map.mapTemperature.OutdoorTemp;
        public string CalcTemperatureHex()
        {
            Color coldColor = new Color(0.2f, 0.7f, 1f);
            Color hotColor = new Color(1f, 0.3f, 0f);

            float t = Mathf.InverseLerp(-10f, 50f, Temperature);
            Color temperatureColor = Color.Lerp(coldColor, hotColor, t);

            return "#" + ColorUtility.ToHtmlStringRGB(temperatureColor);
        }

        public string Terrain => Map.TileInfo.hilliness.GetLabel();
        public float Elevation => Map.TileInfo.elevation;
        public float Pollution => Map.TileInfo.pollution;

        public GameCondition ActiveCondition => Map.GameConditionManager.ActiveConditions.FirstOrFallback(null);

        public string FactionName => Map.ParentFaction.Name;
        public string SettlementName => Map.Parent is Settlement settlement ? settlement.Name : FactionName;
    }

    public enum DayRelative
    {
        Settle,
        Quadrum,
        Season,
        Year,
    }

    public class DawnFormatter
    {
        public Game Game { get; }

        public int CurrentAbsTicks => Game.tickManager.TicksAbs;

        public Vector2 CurrentLocation => Game.World.grid.LongLatOf(Game.CurrentMap.Tile);

        public DawnFormatter(Game game)
        {
            Game = game;
        }

        public FormatContext CreateFormatContext(bool startsAtZero)
        {
            if (Game.CurrentMap == null)
                return new FormatContext();

            return new FormatContext
            {
                AbsTicks = CurrentAbsTicks,

                World = Game.World,
                Map = Game.CurrentMap,
                Location = CurrentLocation,

                StartAtZero = startsAtZero,
            };
        }
    }

    public static class DawnFormatterUtility
    {
        private static DawnSettings Settings => DawnMod.Settings;

        private static readonly Regex TokenFormatRegex = new Regex(@"\{(?<token>[^{}]+)\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Dictionary<string, Func<FormatContext, string>> FormatTokens = new Dictionary<string, Func<FormatContext, string>>(StringComparer.OrdinalIgnoreCase)
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
            { "TEMPERATURE_COLOR", context => context.CalcTemperatureHex() },

            { "WORLD", context => context.World.info.name },
            { "BIOME", context => context.Map.Biome.LabelCap },
            { "TERRAIN", context => context.Terrain },

            { "ELEVATION", context => $"{Mathf.CeilToInt(context.Elevation)}m" },
            { "ELEVATION_KM", context => $"{context.Elevation / 1000f:F2}km" },

            { "POLLUTION", context => context.Pollution.ToStringPercent() },

            { "CONDITION", context => context.ActiveCondition?.LabelCap },

            { "FACTION", context => context.FactionName },
            { "SETTLEMENT", context => context.SettlementName },

            { "UPPER_FONTSIZE", _ => (Settings.UpperTextStyle.FontSize * Settings.Scale).ToString() },
            { "BOTTOM_FONTSIZE", _ => (Settings.BottomTextStyle.FontSize * Settings.Scale).ToString() },
        };

        private static readonly Regex ExtraRichTextTagRegex = new Regex(@"<(?<tag>\w+)>(?<content>.*?)</\1>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Dictionary<string, Func<string, string>> ExtraRichTextTags = new Dictionary<string, Func<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "title", text => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower())},
            { "upper", text => text.ToUpper() },
            { "lower", text => text.ToLower() },
        };

        private readonly struct ParsedOperand
        {
            public float Value { get; }
            public string Prefix { get; }
            public string Suffix { get; }
            public bool IsValid { get; }

            private static readonly Regex OperandValueRegex = new Regex(@"[-+]?\d+(?:[.,]\d+)?", RegexOptions.Compiled);

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

                int operatorIndex = token.IndexOfAny(new char[] { '+', '-', '*', '/' });
                bool isOperation = operatorIndex >= 0;

                if (!isOperation)
                {
                    if (FormatTokens.TryGetValue(token, out var replacer))
                        return replacer?.Invoke(context);

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

                if (FormatTokens.TryGetValue(leftPart, out var leftReplacer))
                    return leftReplacer?.Invoke(context);
                if (FormatTokens.TryGetValue(rightPart, out var rightReplacer))
                    return rightReplacer?.Invoke(context);

                return match.Value;
            });
        }

        private static float CalcStringMath(float left, float right, char operation)
        {
            switch (operation)
            {
                case '+':
                    return left + right;
                case '-':
                    return left - right;
                case '*':
                    return left * right;
                case '/':
                    return right != 0f ? left / right : 0f;

                default:
                    break;
            }

            return 0f;
        }

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

            string format = TokenFormatText(context, text);
            return ExtraRichTextTagText(format);
        }
    }
}
