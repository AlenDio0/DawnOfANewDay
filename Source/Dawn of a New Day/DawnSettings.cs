using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public class DawnSettings : ModSettings
    {
        public bool Enabled = true;
        public bool ScreenshotMode = false;

        private bool m_ShowExample = false;
        public bool ConsumeShowExample
        {
            get
            {
                if (m_ShowExample)
                {
                    m_ShowExample = false;
                    return true;
                }
                return false;
            }
        }

        #region Appearance Section

        public bool ShowHighlight = true;

        public float Scale = 4f;
        public Vector2 Offset = new Vector2(UI.screenWidth, UI.screenHeight) / 2f;

        public float LineWidthPercentage = 80f;
        public float LineThickness = 4f;
        public float LinePadding = 8f;
        public Color LineColor = Color.gray;

        #endregion

        #region Duration Section

        public float DisplayDurationSeconds = 7.5f;
        public float FadeInDurationSeconds = 1f;
        public float FadeOutDurationSeconds = 2f;

        #endregion

        #region Text Section

        public GUIStyle UpperTextGUIStyle = new GUIStyle(Text.CurFontStyle);
        private string m_UpperFontFamilyName = "";
        private int m_UpperFontSize = 40;
        private bool m_UpperTextBold = true;
        private bool m_UpperTextItalic = false;
        private Color m_UpperTextColor = Color.white;

        public float UpperOutlineThickness = 1f;
        public Color UpperOutlineColor = Color.black;

        public GUIStyle BottomTextGUIStyle = new GUIStyle(Text.CurFontStyle);
        private string m_BottomFontFamilyName = "";
        private int m_BottomFontSize = 24;
        private bool m_BottomTextBold = false;
        private bool m_BottomTextItalic = false;
        private Color m_BottomTextColor = Color.white;

        public float BottomOutlineThickness = 1f;
        public Color BottomOutlineColor = Color.black;

        #endregion

        #region Label Format Section

        public string UpperTextFormat = "DAY {}";
        public string BottomTextFormat = "YEAR {Y} | {Q} | {S}";

        #endregion

        #region Extra Section

        public bool StartsAtZero = false;
        public int ShowEveryXDays = 1;
        public int TriggerHour = 6;
        public DayRelative DayRelativeTo = DayRelative.Settle;

        #endregion

        #region Buffers

        private string m_DisplayDurationBuffer;
        private string m_FadeInDurationBuffer;
        private string m_FadeOutDurationBuffer;

        private string m_LineWidthPercentageBuffer;
        private string m_LineThicknessBuffer;
        private string m_LinePaddingBuffer;

        private string m_DayFontSizeBuffer;
        private string m_DateFontSizeBuffer;

        private string m_DayOutlineThicknessBuffer;
        private string m_DateOutlineThicknessBuffer;

        private string m_ShowEveryXDaysBuffer;

        private string m_TriggerHourBuffer;

        #endregion

        bool m_AppearanceSection = false;
        bool m_DurationSection = false;
        bool m_TextSection = false;
        bool m_LabelFormatSection = false;
        bool m_ExtraSection = false;

        private Vector2 m_ScrollPosition = Vector2.zero;
        private float m_CachedScrollViewHeight = 0f;

        public void DoWindowContents(Rect canva)
        {
            try
            {
                canva = canva.LeftPart(0.95f);
                canva = canva.MiddlePart(1f, 0.95f);

                Listing_Standard listing = new Listing_Standard { maxOneColumn = true };
                listing.Begin(canva);

                ShowHeader(listing);
                float headerHeight = listing.CurHeight;

                listing.End();

                Rect outRect = new Rect(canva.x, canva.y + headerHeight, canva.width, canva.height - headerHeight);
                Rect viewRect = new Rect(0f, 0f, outRect.width - 32f, Mathf.Max(outRect.height, m_CachedScrollViewHeight));
                Widgets.BeginScrollView(outRect, ref m_ScrollPosition, viewRect);

                listing.Begin(viewRect);

                if (listing.SectionButton(DawnData.SettingsSection_Appearance, ref m_AppearanceSection))
                    listing.Indented(() => ShowAppearanceSection(listing));

                if (listing.SectionButton(DawnData.SettingsSection_Duration, ref m_DurationSection))
                    listing.Indented(() => ShowDurationSection(listing));

                if (listing.SectionButton(DawnData.SettingsSection_Text, ref m_TextSection))
                    listing.Indented(() => ShowTextSection(listing));

                if (listing.SectionButton(DawnData.SettingsSection_LabelFormat, ref m_LabelFormatSection))
                    listing.Indented(() => ShowLabelFormatSection(listing));

                if (listing.SectionButton(DawnData.SettingsSection_Extra, ref m_ExtraSection))
                    listing.Indented(() => ShowExtraSection(listing));

                m_CachedScrollViewHeight = listing.CurHeight;

                listing.End();

                Widgets.EndScrollView();
            }
            catch (Exception exception)
            {
                DawnData.Error($"Exception catched into DawnSettings.DoWindowContents(canva: {canva})\nException: {exception}");
            }
        }

        private void ShowHeader(Listing_Standard listing)
        {
            listing.CheckboxLabeled(DawnData.SettingsLabel_Enabled, ref Enabled);
            listing.CheckboxLabeled(DawnData.SettingsLabel_ScreenshotMode, ref ScreenshotMode);

            listing.Gap();

            if (listing.ButtonText(DawnData.SettingsLabel_ShowExample))
            {
                Messages.Message(DawnData.SettingsMessage_ShowExample, MessageTypeDefOf.PositiveEvent, false);
                m_ShowExample = true;
            }

            listing.Gap();
        }

        private void ShowAppearanceSection(Listing_Standard listing)
        {
            listing.CheckboxLabeled(DawnData.SettingsLabel_ShowHighlight, ref ShowHighlight);

            listing.Gap();

            if (listing.ButtonText($"{DawnData.SettingsLabel_Scale}: {Scale}x"))
            {
                var scaleModes = SettingsHelper.CreateFloatMenu(DawnData.ScaleModes, scale => new FloatMenuOption($"{scale}x", () => Scale = scale));
                Find.WindowStack.Add(scaleModes);
            }

            if (listing.ButtonText(DawnData.SettingsLabel_OffsetPresets))
            {
                var offsetPresets = SettingsHelper.CreateFloatMenu(DawnData.OffsetPresets, item => new FloatMenuOption($"{item.Name}", () =>
                    Offset = new Vector2(UI.screenWidth, UI.screenHeight) * item.Mode));
                Find.WindowStack.Add(offsetPresets);
            }

            Offset.x = Mathf.Ceil(listing.SliderLabeled($"{DawnData.SettingsLabel_Offset} X ({Offset.x} px)", Offset.x, 0f, UI.screenWidth, 0.25f));
            Offset.y = Mathf.Ceil(listing.SliderLabeled($"{DawnData.SettingsLabel_Offset} Y ({Offset.y} px)", Offset.y, 0f, UI.screenHeight, 0.25f));

            listing.Gap();

            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_LineWidthPercentage} (%)", ref LineWidthPercentage, ref m_LineWidthPercentageBuffer, 0f, 100f);
            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_LineThickness} (px)", ref LineThickness, ref m_LineThicknessBuffer, 0f, 100f);
            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_LinePadding} (px)", ref LinePadding, ref m_LinePaddingBuffer, 0f, 100f);
            listing.LabeledRadioColorPresets(ref LineColor, DawnData.SettingsLabel_LineColor);
        }

        private void ShowDurationSection(Listing_Standard listing)
        {
            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_DisplayDuration} (seconds)", ref DisplayDurationSeconds, ref m_DisplayDurationBuffer, 0f, 120f);
            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_FadeInDuration} (seconds)", ref FadeInDurationSeconds, ref m_FadeInDurationBuffer, 0f, 120f);
            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_FadeOutDuration} (seconds)", ref FadeOutDurationSeconds, ref m_FadeOutDurationBuffer, 0f, 120f);
        }

        private void ShowTextSection(Listing_Standard listing)
        {
            GameFont defaultFont = Text.Font;

            Text.Font = GameFont.Medium;

            listing.Label(DawnData.SettingsLabel_UpperTextProperties);

            Text.Font = defaultFont;

            if (listing.ButtonText($"{DawnData.SettingsLabel_FontFamily} ({m_UpperFontFamilyName.Fallback(DawnData.SettingsLabel_DefaultFont)})"))
            {
                Find.WindowStack.Add(new Dialog_ChooseFontFamily(m_UpperFontFamilyName, font =>
                {
                    m_UpperFontFamilyName = font;
                    if (!m_UpperFontFamilyName.NullOrEmpty())
                        UpperTextGUIStyle.font = Font.CreateDynamicFontFromOSFont(m_UpperFontFamilyName, 16);
                }));
            }

            listing.LabeledTextFieldNumeric(DawnData.SettingsLabel_FontSize, ref m_UpperFontSize, ref m_DayFontSizeBuffer, 0f, 256f);
            listing.LabeledTextFieldNumeric(DawnData.SettingsLabel_OutlineThickness, ref UpperOutlineThickness, ref m_DayOutlineThicknessBuffer, 0f, 10f);

            listing.CheckboxLabeled(DawnData.SettingsLabel_Bold, ref m_UpperTextBold);
            listing.CheckboxLabeled(DawnData.SettingsLabel_Italic, ref m_UpperTextItalic);
            UpperTextGUIStyle.fontStyle = SettingsHelper.MapFontStyle(m_UpperTextBold, m_UpperTextItalic);

            UpperTextGUIStyle.normal.textColor = listing.LabeledRadioColorPresets(ref m_UpperTextColor, DawnData.SettingsLabel_TextColor);
            listing.LabeledRadioColorPresets(ref UpperOutlineColor, DawnData.SettingsLabel_OutlineColor);

            listing.Gap();

            Text.Font = GameFont.Medium;

            listing.Label(DawnData.SettingsLabel_BottomTextProperties);

            Text.Font = defaultFont;

            if (listing.ButtonText($"{DawnData.SettingsLabel_FontFamily} ({m_BottomFontFamilyName.Fallback(DawnData.SettingsLabel_DefaultFont)})"))
            {
                Find.WindowStack.Add(new Dialog_ChooseFontFamily(m_BottomFontFamilyName, font =>
                {
                    m_BottomFontFamilyName = font;
                    if (!m_BottomFontFamilyName.NullOrEmpty())
                        BottomTextGUIStyle.font = Font.CreateDynamicFontFromOSFont(m_BottomFontFamilyName, 16);
                }));
            }

            listing.LabeledTextFieldNumeric(DawnData.SettingsLabel_FontSize, ref m_BottomFontSize, ref m_DateFontSizeBuffer, 0f, 256f);
            listing.LabeledTextFieldNumeric(DawnData.SettingsLabel_OutlineThickness, ref BottomOutlineThickness, ref m_DateOutlineThicknessBuffer, 0f, 10f);

            listing.CheckboxLabeled(DawnData.SettingsLabel_Bold, ref m_BottomTextBold);
            listing.CheckboxLabeled(DawnData.SettingsLabel_Italic, ref m_BottomTextItalic);
            BottomTextGUIStyle.fontStyle = SettingsHelper.MapFontStyle(m_BottomTextBold, m_BottomTextItalic);

            BottomTextGUIStyle.normal.textColor = listing.LabeledRadioColorPresets(ref m_BottomTextColor, DawnData.SettingsLabel_TextColor);
            listing.LabeledRadioColorPresets(ref BottomOutlineColor, DawnData.SettingsLabel_OutlineColor);
        }

        private void ShowLabelFormatSection(Listing_Standard listing)
        {
            listing.LabeledTextEntry(DawnData.SettingsLabel_UpperTextFormat, ref UpperTextFormat, 0.5f);

            listing.Gap();

            listing.LabeledTextEntry(DawnData.SettingsLabel_BottomTextFormat, ref BottomTextFormat, 0.5f);

            listing.Gap();

            Color defaultColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.75f);

            listing.Label(DawnData.SettingsHint_LabelFormat);

            GUI.color = defaultColor;
        }

        private void ShowExtraSection(Listing_Standard listing)
        {
            listing.CheckboxLabeled(DawnData.SettingsLabel_StartsAtZero, ref StartsAtZero);

            listing.Gap();

            listing.LabeledTextFieldNumeric(DawnData.SettingsLabel_ShowEveryXDays, ref ShowEveryXDays, ref m_ShowEveryXDaysBuffer, 1f, 1200f);
            listing.LabeledTextFieldNumeric(DawnData.SettingsLabel_TriggerHour, ref TriggerHour, ref m_TriggerHourBuffer, 0f, 23f);

            listing.Gap();

            GameFont defaultFont = Text.Font;
            Text.Font = GameFont.Medium;

            listing.Label(DawnData.SettingsLabel_DayRelativeTo);

            Text.Font = defaultFont;

            Rect radioRect = listing.GetRect(30f);

            var dayRelativeModes = Enum.GetValues(typeof(DayRelative)).Cast<DayRelative>().ToArray();
            float itemWidth = radioRect.width / dayRelativeModes.Length;

            for (int i = 0; i < dayRelativeModes.Length; i++)
            {
                DayRelative mode = dayRelativeModes[i];

                Rect buttonRect = new Rect(radioRect.x + (i * itemWidth), radioRect.y, itemWidth, radioRect.height).MiddlePart(0.5f, 1f);

                TooltipHandler.TipRegion(buttonRect, DawnData.SettingsTooltip_DayRelativeTo.TryGetValue(mode, ""));
                if (Widgets.RadioButtonLabeled(buttonRect, mode.ToStringSafe(), DayRelativeTo == mode))
                    DayRelativeTo = mode;
            }
        }

        public override void ExposeData()
        {
            try
            {
                base.ExposeData();

                Scribe_Values.Look(ref Enabled, "Enabled", true);
                Scribe_Values.Look(ref ScreenshotMode, "ScreenshotMode", false);

                #region Appearance Section

                Scribe_Values.Look(ref ShowHighlight, "ShowHighlight", true);

                Scribe_Values.Look(ref Scale, "Scale", 1.5f);
                Scribe_Values.Look(ref Offset, "Offset", new Vector2(UI.screenWidth, UI.screenHeight) / 2f);

                Scribe_Values.Look(ref LineThickness, "LineThickness", 4f);
                Scribe_Values.Look(ref LineWidthPercentage, "LineWidthPercentage100", 80f);
                Scribe_Values.Look(ref LinePadding, "LinePadding", 8f);
                Scribe_Values.Look(ref LineColor, "LineColor", Color.gray);

                #endregion

                #region Duration Section

                Scribe_Values.Look(ref DisplayDurationSeconds, "DisplayDurationSeconds", 7.5f);
                Scribe_Values.Look(ref FadeInDurationSeconds, "FadeInDurationSeconds", 1f);
                Scribe_Values.Look(ref FadeOutDurationSeconds, "FadeOutDurationSeconds", 2f);

                #endregion

                #region Text Section

                if (UpperTextGUIStyle == null)
                    UpperTextGUIStyle = new GUIStyle(Text.CurFontStyle);

                Scribe_Values.Look(ref m_UpperFontFamilyName, "UpperFontFamilyName", "");
                Scribe_Values.Look(ref m_UpperFontSize, "UpperFontSize", 40);
                Scribe_Values.Look(ref m_UpperTextColor, "UpperTextColor", Color.white);

                Scribe_Values.Look(ref UpperOutlineThickness, "UpperOutlineThickness", 1f);
                Scribe_Values.Look(ref UpperOutlineColor, "UpperOutlineColor", Color.black);

                UpperTextGUIStyle.fontSize = Mathf.CeilToInt(m_UpperFontSize * Scale);
                UpperTextGUIStyle.fontStyle = ScribeByValue(UpperTextGUIStyle.fontStyle, "DayStyleFontStyle", FontStyle.Bold);
                UpperTextGUIStyle.normal.textColor = m_UpperTextColor;
                UpperTextGUIStyle.alignment = TextAnchor.MiddleCenter;

                if (BottomTextGUIStyle == null)
                    BottomTextGUIStyle = new GUIStyle(Text.CurFontStyle);

                Scribe_Values.Look(ref m_BottomFontFamilyName, "BottomFontFamilyName", "");
                Scribe_Values.Look(ref m_BottomFontSize, "BottomFontSize", 24);
                Scribe_Values.Look(ref m_BottomTextColor, "BottomTextColor", Color.white);

                Scribe_Values.Look(ref BottomOutlineThickness, "BottomOutlineThickness", 1f);
                Scribe_Values.Look(ref BottomOutlineColor, "BottomOutlineColor", Color.black);

                BottomTextGUIStyle.fontSize = Mathf.CeilToInt(m_BottomFontSize * Scale);
                BottomTextGUIStyle.fontStyle = ScribeByValue(BottomTextGUIStyle.fontStyle, "DateStyleFontStyle", FontStyle.Normal);
                BottomTextGUIStyle.normal.textColor = m_BottomTextColor;
                BottomTextGUIStyle.alignment = TextAnchor.MiddleCenter;

                #endregion

                #region Label Format Section

                Scribe_Values.Look(ref UpperTextFormat, "UpperTextFormat", "DAY {}");
                Scribe_Values.Look(ref BottomTextFormat, "BottomTextFormat", "YEAR {Y} | {Q} | {S}");

                #endregion

                #region Extra Section

                Scribe_Values.Look(ref StartsAtZero, "StartsAtZero", false);
                Scribe_Values.Look(ref ShowEveryXDays, "ShowEveryXDays", 1);
                Scribe_Values.Look(ref TriggerHour, "TriggerHour", 6);
                Scribe_Values.Look(ref DayRelativeTo, "DayRelativeTo", DayRelative.Settle);

                #endregion
            }
            catch (Exception exception)
            {
                DawnData.Error($"Exception catched into DawnSettings.ExposeData()\nException: {exception}");
            }
        }

        public void UpdateTextFont()
        {
            try
            {
                if (!m_UpperFontFamilyName.NullOrEmpty())
                    UpperTextGUIStyle.font = Font.CreateDynamicFontFromOSFont(m_UpperFontFamilyName, 16);
                else
                    UpperTextGUIStyle.font = Text.CurFontStyle.font;


                if (!m_BottomFontFamilyName.NullOrEmpty())
                    BottomTextGUIStyle.font = Font.CreateDynamicFontFromOSFont(m_BottomFontFamilyName, 16);
                else
                    BottomTextGUIStyle.font = Text.CurFontStyle.font;
            }
            catch (Exception exception)
            {
                DawnData.Error($"DawnSettings.UpdateTextFont() Failed!\nUpperFontFamilyName: '{m_UpperFontFamilyName}', BottomFontFamilyName: '{m_BottomFontFamilyName}'\nException: {exception}");
            }
        }

        private T ScribeByValue<T>(T value, string label, T defaultValue)
        {
            Scribe_Values.Look<T>(ref value, label, defaultValue);
            return value;
        }
    }
}
