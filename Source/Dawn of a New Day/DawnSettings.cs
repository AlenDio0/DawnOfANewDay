using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public class DawnSettings : ModSettings
    {
        private bool m_ShowExample = false;
        public bool ConsumeShowExample()
        {
            if (m_ShowExample)
            {
                m_ShowExample = false;
                return true;
            }
            return false;
        }

        public bool Enabled = true;
        public bool StartsAtZero = false;
        public bool ShowHighlight = true;

        public enum DayRelative
        {
            None,
            Quadrum,
            Season,
            Year
        }
        public DayRelative DayRelativeTo = DayRelative.None;

        private static readonly float[] ScaleModes = { 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f, 2.5f, 3f, 4f };
        private static readonly (string Name, Vector2 Mode)[] OffsetModes = new[]
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

        public float Scale = 4f;
        public Vector2 Offset = new Vector2(UI.screenWidth, UI.screenHeight) / 2f;

        public float DisplayDurationSeconds = 7.5f;
        public float FadeInDurationSeconds = 1f;
        public float FadeOutDurationSeconds = 2f;

        private string m_DisplayDurationBuffer;
        private string m_FadeInDurationBuffer;
        private string m_FadeOutDurationBuffer;

        public float LineWidthPercentage = 0.8f;
        public float LineThickness = 4f;
        public float LinePadding = 8f;

        public string m_LineWidthPercentageBuffer;
        public string m_LineThicknessBuffer;
        public string m_LinePaddingBuffer;

        public string DayText = "DAY";
        public string YearText = "YEAR";

        public int DayFontSize = 40;
        public int DateFontSize = 24;

        public string m_DayFontSizeBuffer;
        public string m_DateFontSizeBuffer;

        private static readonly (string Name, Color Color)[] ColorPresets = new[]
        {
            ("White", Color.white),
            ("Gray", Color.gray),
            ("Black", Color.black),
            ("Red", Color.red),
            ("Green", Color.green),
            ("Blue", Color.blue),
            ("Yellow", Color.yellow),
            ("Grey", Color.grey),
            ("Magenta", Color.magenta),
            ("Cyan", Color.cyan)
        };

        public GUIStyle DayTextStyle = new GUIStyle(Text.CurFontStyle);
        public GUIStyle DateTextStyle = new GUIStyle(Text.CurFontStyle);

        private string m_DayTextColorBuffer = "White";
        private string m_DateTextColorBuffer = "White";

        public float DayOutlineThickness = 1f;
        public float DateOutlineThickness = 1f;
        public Color DayOutlineColor = Color.black;
        public Color DateOutlineColor = Color.black;

        private string m_DayOutlineThicknessBuffer;
        private string m_DateOutlineThicknessBuffer;
        private string m_DayOutlineColorBuffer = "Black";
        private string m_DateOutlineColorBuffer = "Black";

        public int ShowEveryXDays = 1;

        private string m_ShowEveryXDaysBuffer;

        public void DoWindowContents(Rect canva)
        {
            canva.TakeHeight(0.05f);

            ShowBoolsButtonsProps(canva.TakeHeight(0.1f));
            ShowScaleOffsetProps(canva.TakeHeight(0.2f));
            ShowDurationProps(canva.TakeHeight(0.1f, 0.05f));
            ShowLineProps(canva.TakeHeight(0.1f, 0.05f));
            ShowTextStringProps(canva.TakeHeight(0.15f));
            ShowTextStyleProps(canva.TakeHeight(0.2f, 0.05f));
            ShowTextOutlineProps(canva.TakeHeight(0.2f, 0.05f));
            ShowExtraProps(canva.TakeHeight(0.25f, 0.05f));
        }

        private void ShowBoolsButtonsProps(Rect part)
        {
            Listing_Standard listing = new Listing_Standard { ColumnWidth = part.width / 5.5f };
            listing.Begin(part);

            listing.CheckboxLabeled(DawnData.SettingsLabel_Enabled, ref Enabled);

            listing.NewColumn();
            listing.CheckboxLabeled(DawnData.SettingsLabel_StartsAtZero, ref StartsAtZero);

            listing.NewColumn();
            listing.CheckboxLabeled(DawnData.SettingsLabel_ShowHighlight, ref ShowHighlight);

            listing.NewColumn();
            if (listing.ButtonText($"{DawnData.SettingsLabel_DayRelativeTo}: {DayRelativeTo}"))
            {
                var dayRelativeModes = Enum.GetValues(typeof(DayRelative)).Cast<DayRelative>();
                var modes = SettingsHelper.CreateFloatMenu(dayRelativeModes, mode => new FloatMenuOption($"{mode}", () => DayRelativeTo = mode));
                Find.WindowStack.Add(modes);
            }

            listing.NewColumn();
            if (listing.ButtonText(DawnData.SettingsLabel_ShowExample))
            {
                Messages.Message(DawnData.SettingsMessage_ShowExample, MessageTypeDefOf.PositiveEvent);
                m_ShowExample = true;
            }
            listing.End();
        }

        private void ShowScaleOffsetProps(Rect part)
        {
            Listing_Standard listing = new Listing_Standard { ColumnWidth = part.width / 2.05f };
            listing.Begin(part);

            if (listing.ButtonText($"{DawnData.SettingsLabel_Scale}: {Scale}x"))
            {
                var scaleModes = SettingsHelper.CreateFloatMenu(ScaleModes, scale => new FloatMenuOption($"{scale}x", () => Scale = scale));
                Find.WindowStack.Add(scaleModes);
            }

            listing.Gap();
            Offset.x = Mathf.Ceil(listing.SliderLabeled($"{DawnData.SettingsLabel_Offset} X ({Offset.x} px)", Offset.x, 0f, UI.screenWidth));

            listing.NewColumn();
            if (listing.ButtonText(DawnData.SettingsLabel_SetOffsetCenter))
            {
                var offsetPresets = SettingsHelper.CreateFloatMenu(OffsetModes, item => new FloatMenuOption($"{item.Name}",
                    () => Offset = new Vector2(UI.screenWidth, UI.screenHeight) * item.Mode));
                Find.WindowStack.Add(offsetPresets);
            }

            listing.Gap();
            Offset.y = Mathf.Ceil(listing.SliderLabeled($"{DawnData.SettingsLabel_Offset} Y ({Offset.y} px)", Offset.y, 0f, UI.screenHeight));

            listing.End();
        }

        private void ShowDurationProps(Rect part)
        {
            Listing_Standard listing = new Listing_Standard { ColumnWidth = part.width / 3.125f };
            listing.Begin(part);

            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_DisplayDuration} (s)", ref DisplayDurationSeconds, ref m_DisplayDurationBuffer, 0f, 60f, 0.6f);

            listing.NewColumn();
            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_FadeInDuration} (s)", ref FadeInDurationSeconds, ref m_FadeInDurationBuffer, 0f, 60f, 0.6f);

            listing.NewColumn();
            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_FadeOutDuration} (s)", ref FadeOutDurationSeconds, ref m_FadeOutDurationBuffer, 0f, 60f, 0.6f);

            listing.End();
        }

        private void ShowLineProps(Rect part)
        {
            Listing_Standard listing = new Listing_Standard { ColumnWidth = part.width / 3.125f };
            listing.Begin(part);

            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_LineWidthPercentage} (%)", ref LineWidthPercentage, ref m_LineWidthPercentageBuffer, 0f, 1f, 0.5f);

            listing.NewColumn();
            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_LineThickness} (px)", ref LineThickness, ref m_LineThicknessBuffer, 0f, 100f, 0.5f);

            listing.NewColumn();
            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_LinePadding} (px)", ref LinePadding, ref m_LinePaddingBuffer, 0f, 100f, 0.5f);

            listing.End();
        }

        private void ShowTextStringProps(Rect part)
        {
            Listing_Standard listing = new Listing_Standard { ColumnWidth = part.width / 4.25f };
            listing.Begin(part);

            listing.LabeledTextEntry(DawnData.SettingsLabel_DayText, ref DayText, 0.5f);

            listing.NewColumn();
            listing.LabeledTextEntry(DawnData.SettingsLabel_YearText, ref YearText, 0.5f);

            listing.NewColumn();
            listing.LabeledTextFieldNumeric(DawnData.SettingsLabel_DayFontSize, ref DayFontSize, ref m_DayFontSizeBuffer, 0f, 256f, 0.5f);

            listing.NewColumn();
            listing.LabeledTextFieldNumeric(DawnData.SettingsLabel_DateFontSize, ref DateFontSize, ref m_DateFontSizeBuffer, 0f, 256f, 0.5f);

            listing.End();
        }

        private void ShowTextStyleProps(Rect part)
        {
            Listing_Standard listing = new Listing_Standard { ColumnWidth = part.width / 4.25f };
            listing.Begin(part);

            if (listing.ButtonText($"{DawnData.SettingsLabel_DayFontStyle}: {DayTextStyle.fontStyle}"))
            {
                var fontStyles = Enum.GetValues(typeof(FontStyle)).Cast<FontStyle>();
                var styles = SettingsHelper.CreateFloatMenu(fontStyles, style => new FloatMenuOption($"{style}", () => DayTextStyle.fontStyle = style));
                Find.WindowStack.Add(styles);
            }

            listing.NewColumn();
            if (listing.ButtonText($"{DawnData.SettingsLabel_DateFontStyle}: {DateTextStyle.fontStyle}"))
            {
                var fontStyles = Enum.GetValues(typeof(FontStyle)).Cast<FontStyle>();
                var styles = SettingsHelper.CreateFloatMenu(fontStyles, style => new FloatMenuOption($"{style}", () => DateTextStyle.fontStyle = style));
                Find.WindowStack.Add(styles);
            }

            listing.NewColumn();
            if (listing.ButtonText($"{DawnData.SettingsLabel_DayTextColor}: {m_DayTextColorBuffer}"))
            {
                var colors = SettingsHelper.CreateFloatMenu(ColorPresets, item => new FloatMenuOption(item.Name, delegate
                {
                    m_DayTextColorBuffer = item.Name;
                    DayTextStyle.normal.textColor = item.Color;
                }, BaseContent.WhiteTex, item.Color));
                Find.WindowStack.Add(colors);
            }

            listing.NewColumn();
            if (listing.ButtonText($"{DawnData.SettingsLabel_DateTextColor}: {m_DateTextColorBuffer}"))
            {
                var colors = SettingsHelper.CreateFloatMenu(ColorPresets, item => new FloatMenuOption(item.Name, delegate
                {
                    m_DateTextColorBuffer = item.Name;
                    DateTextStyle.normal.textColor = item.Color;
                }, BaseContent.WhiteTex, item.Color));
                Find.WindowStack.Add(colors);
            }

            listing.End();
        }

        private void ShowTextOutlineProps(Rect part)
        {
            Listing_Standard listing = new Listing_Standard { ColumnWidth = part.width / 4.25f };
            listing.Begin(part);

            listing.LabeledTextFieldNumeric(DawnData.SettingsLabel_DayOutlineThickness, ref DayOutlineThickness, ref m_DayOutlineThicknessBuffer, 0f, 10f, 0.75f);

            listing.NewColumn();
            listing.LabeledTextFieldNumeric(DawnData.SettingsLabel_DateOutlineThickness, ref DateOutlineThickness, ref m_DateOutlineThicknessBuffer, 0f, 10f, 0.75f);

            listing.NewColumn();
            if (listing.ButtonText($"{DawnData.SettingsLabel_DayOutlineColor}: {m_DayOutlineColorBuffer}"))
            {
                var colors = SettingsHelper.CreateFloatMenu(ColorPresets, item => new FloatMenuOption(item.Name, delegate
                {
                    m_DayOutlineColorBuffer = item.Name;
                    DayOutlineColor = item.Color;
                }, BaseContent.WhiteTex, item.Color));
                Find.WindowStack.Add(colors);
            }

            listing.NewColumn();
            if (listing.ButtonText($"{DawnData.SettingsLabel_DateOutlineColor}: {m_DateOutlineColorBuffer}"))
            {
                var colors = SettingsHelper.CreateFloatMenu(ColorPresets, item => new FloatMenuOption(item.Name, delegate
                {
                    m_DateOutlineColorBuffer = item.Name;
                    DateOutlineColor = item.Color;
                }, BaseContent.WhiteTex, item.Color));
                Find.WindowStack.Add(colors);
            }

            listing.End();
        }

        private void ShowExtraProps(Rect part)
        {
            Listing_Standard listing = new Listing_Standard { ColumnWidth = part.width / 4.25f };
            listing.Begin(part);

            listing.LabeledTextFieldNumeric(DawnData.SettingsLabel_ShowEveryXDays, ref ShowEveryXDays, ref m_ShowEveryXDaysBuffer, 1f, 1200f, 0.75f);

            listing.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref Enabled, "Enabled", true);
            Scribe_Values.Look(ref StartsAtZero, "StartsAtZero", false);
            Scribe_Values.Look(ref ShowHighlight, "ShowHighlight", true);

            Scribe_Values.Look(ref DayRelativeTo, "DayRelativeTo", DayRelative.None);

            Scribe_Values.Look(ref DisplayDurationSeconds, "DisplayDurationSeconds", 7.5f);
            Scribe_Values.Look(ref FadeInDurationSeconds, "FadeInDurationSeconds", 1f);
            Scribe_Values.Look(ref FadeOutDurationSeconds, "FadeOutDurationSeconds", 2f);

            Scribe_Values.Look(ref Scale, "Scale", 1.5f);
            Scribe_Values.Look(ref Offset, "Offset", new Vector2(UI.screenWidth, UI.screenHeight) / 2f);

            Scribe_Values.Look(ref LineThickness, "LineThickness", 4f);
            Scribe_Values.Look(ref LineWidthPercentage, "LineWidthPercentage", 0.8f);
            Scribe_Values.Look(ref LinePadding, "LinePadding", 8f);

            Scribe_Values.Look(ref DayText, "DayText", "DAY");
            Scribe_Values.Look(ref YearText, "YearText", "YEAR");

            if (DayTextStyle == null)
                DayTextStyle = new GUIStyle(Text.CurFontStyle);

            Scribe_Values.Look(ref DayFontSize, "DayFontSize", 40);
            DayTextStyle.fontSize = Mathf.CeilToInt(DayFontSize * Scale);
            DayTextStyle.fontStyle = ScribeByValue(DayTextStyle.fontStyle, "DayStyleFontStyle", FontStyle.Bold);
            DayTextStyle.normal.textColor = ScribeByValue(DayTextStyle.normal.textColor.WithAlpha(1f), "DayStyleTextColor", Color.white);
            DayTextStyle.alignment = TextAnchor.MiddleCenter;

            if (DateTextStyle == null)
                DateTextStyle = new GUIStyle(Text.CurFontStyle);

            Scribe_Values.Look(ref DateFontSize, "DateFontSize", 24);
            DateTextStyle.fontSize = Mathf.CeilToInt(DateFontSize * Scale);
            DateTextStyle.fontStyle = ScribeByValue(DateTextStyle.fontStyle, "DateStyleFontStyle", FontStyle.Normal);
            DateTextStyle.normal.textColor = ScribeByValue(DateTextStyle.normal.textColor.WithAlpha(1f), "DateStyleTextColor", new Color(0.9f, 0.9f, 0.9f));
            DateTextStyle.alignment = TextAnchor.MiddleCenter;

            Scribe_Values.Look(ref m_DayTextColorBuffer, "DayTextColorBuffer", "White");
            Scribe_Values.Look(ref m_DateTextColorBuffer, "DateTextColorBuffer", "White");

            Scribe_Values.Look(ref DayOutlineThickness, "DayOutlineThickness", 1f);
            Scribe_Values.Look(ref DateOutlineThickness, "DateOutlineThickness", 1f);
            Scribe_Values.Look(ref DayOutlineColor, "DayOutlineColor", Color.black);
            Scribe_Values.Look(ref DateOutlineColor, "DateOutlineColor", Color.black);

            Scribe_Values.Look(ref m_DayOutlineColorBuffer, "DayOutlineColorBuffer", "Black");
            Scribe_Values.Look(ref m_DateOutlineColorBuffer, "DateOutlineColorBuffer", "Black");
        }

        private T ScribeByValue<T>(T value, string label, T defaultValue)
        {
            Scribe_Values.Look<T>(ref value, label, defaultValue);
            return value;
        }
    }
}
