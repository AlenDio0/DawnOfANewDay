using DawnNewDay.Dialogs;
using DawnNewDay.Utils;
using RimWorld;
using System;
using System.Linq;
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

    public class DawnSettings : ModSettings
    {
        #region Header

        public bool Enabled = DawnDefault.Enabled;
        public bool ScreenshotMode = DawnDefault.ScreenshotMode;

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

        #endregion

        #region Appearance Section

        bool m_AppearanceSection = false;

        public bool ShowHighlight = DawnDefault.ShowHighlight;

        public float Scale = DawnDefault.Scale;
        public Vector2 Offset = DawnDefault.Offset;

        public float LineWidthPercentage = DawnDefault.LineWidthPercentage;
        public float LineThickness = DawnDefault.LineThickness;
        public float LinePadding = DawnDefault.LinePadding;
        public Color LineColor = DawnDefault.LineColor;

        #endregion

        #region Duration Section

        bool m_DurationSection = false;

        public float DisplayDurationSeconds = DawnDefault.DisplayDurationSeconds;
        public float FadeInDurationSeconds = DawnDefault.FadeInDurationSeconds;
        public float FadeOutDurationSeconds = DawnDefault.FadeOutDurationSeconds;

        #endregion

        #region Text Section

        bool m_TextSection = false;

        bool m_TextUpperSection = true;
        public DawnTextStyle UpperTextStyle = new DawnTextStyle(40, true);

        bool m_TextBottomSection = true;
        public DawnTextStyle BottomTextStyle = new DawnTextStyle();

        #endregion

        #region Label Format Section

        bool m_LabelFormatSection = false;

        public string UpperTextFormat = DawnDefault.UpperTextFormat;
        public string BottomTextFormat = DawnDefault.BottomTextFormat;

        #endregion

        #region Sound Section

        bool m_SoundSection = false;

        public bool SoundEnabled = DawnDefault.SoundEnabled;

        private string m_SoundDefName = DawnData.DefaultSoundDefName;

        public SoundDef Sound => DefDatabase<SoundDef>.GetNamedSilentFail(m_SoundDefName) ?? DefDatabase<SoundDef>.GetNamedSilentFail(DawnData.DefaultSoundDefName);

        public float SoundVolume = DawnDefault.SoundVolume;
        public float SoundPitch = DawnDefault.SoundPitch;

        #endregion

        #region Extra Section

        bool m_ExtraSection = false;

        public bool StartsAtZero = DawnDefault.StartsAtZero;
        public int ShowEveryXDays = DawnDefault.ShowEveryXDays;
        public int TriggerHour = DawnDefault.TriggerHour;
        public DayRelative DayRelativeTo = DawnDefault.DayRelativeTo;

        #endregion

        #region Buffers

        private string m_DisplayDurationBuffer;
        private string m_FadeInDurationBuffer;
        private string m_FadeOutDurationBuffer;

        private string m_LineThicknessBuffer;
        private string m_LinePaddingBuffer;

        private string m_ShowEveryXDaysBuffer;

        #endregion

        private Vector2 m_ScrollPosition = Vector2.zero;
        private float m_CachedScrollViewHeight = 0f;

        public void DoWindowContents(Rect canva)
        {
            canva = canva.LeftPart(0.95f);
            canva = canva.MiddlePart(1f, 0.95f);

            Listing_Standard listing = new Listing_Standard { maxOneColumn = true };
            listing.Begin(canva);

            try
            {
                ShowHeader(listing);
                float headerHeight = listing.CurHeight;

                listing.End();

                Rect outRect = new Rect(canva.x, canva.y + headerHeight, canva.width, canva.height - headerHeight);
                Rect viewRect = new Rect(0f, 0f, outRect.width - 32f, Mathf.Max(outRect.height, m_CachedScrollViewHeight));
                Widgets.BeginScrollView(outRect, ref m_ScrollPosition, viewRect);

                listing.Begin(viewRect);

                if (listing.SectionButton(DawnTranslation.Section_Appearance, ref m_AppearanceSection))
                    listing.Indented(() => ShowAppearanceSection(listing));

                if (listing.SectionButton(DawnTranslation.Section_Duration, ref m_DurationSection))
                    listing.Indented(() => ShowDurationSection(listing));

                if (listing.SectionButton(DawnTranslation.Section_Text, ref m_TextSection))
                    listing.Indented(() => ShowTextSection(listing));

                if (listing.SectionButton(DawnTranslation.Section_LabelFormat, ref m_LabelFormatSection))
                    listing.Indented(() => ShowLabelFormatSection(listing));

                if (listing.SectionButton(DawnTranslation.Section_Sound, ref m_SoundSection))
                    listing.Indented(() => ShowSoundSection(listing));

                if (listing.SectionButton(DawnTranslation.Section_Extra, ref m_ExtraSection))
                    listing.Indented(() => ShowExtraSection(listing));

            }
            catch (Exception exception)
            {
                DawnData.Error($"Exception catched into DawnSettings.DoWindowContents(canva: {canva})\nException: {exception}");
            }
            finally
            {
                m_CachedScrollViewHeight = listing.CurHeight;

                listing.End();

                Widgets.EndScrollView();
            }
        }

        private void ShowHeader(Listing_Standard listing)
        {
            listing.CheckboxLabeled(DawnTranslation.Label_Enabled, ref Enabled);
            listing.CheckboxLabeled(DawnTranslation.Label_ScreenshotMode, ref ScreenshotMode);

            listing.Gap();

            if (listing.ButtonText(DawnTranslation.Label_ShowExample))
            {
                Messages.Message(DawnTranslation.Message_ShowExample, MessageTypeDefOf.PositiveEvent, false);
                m_ShowExample = true;
            }

            listing.Gap();
        }

        private void ShowAppearanceSection(Listing_Standard listing)
        {
            listing.CheckboxLabeled(DawnTranslation.Label_ShowHighlight, ref ShowHighlight);

            listing.Gap();

            if (listing.ButtonText($"{DawnTranslation.Label_Scale}: {Scale}x"))
            {
                var scaleModes = SettingsHelper.CreateFloatMenu(DawnData.ScalePresets, scale => new FloatMenuOption($"{scale}x", () => Scale = scale));
                Find.WindowStack.Add(scaleModes);
            }

            if (listing.ButtonText(DawnTranslation.Label_OffsetPresets))
            {
                var offsetPresets = SettingsHelper.CreateFloatMenu(DawnData.OffsetPresets, item => new FloatMenuOption(item.Name, () =>
                    Offset = new Vector2(UI.screenWidth, UI.screenHeight) * item.Preset));
                Find.WindowStack.Add(offsetPresets);
            }

            Offset.x = Mathf.Ceil(listing.SliderLabeled($"{DawnTranslation.Label_Offset} X ({Offset.x} px)", Offset.x, 0f, UI.screenWidth, 0.25f));
            Offset.y = Mathf.Ceil(listing.SliderLabeled($"{DawnTranslation.Label_Offset} Y ({Offset.y} px)", Offset.y, 0f, UI.screenHeight, 0.25f));

            listing.Gap();

            LineWidthPercentage = Mathf.Ceil(listing.SliderLabeled($"{DawnTranslation.Label_LineWidthPercentage} ({LineWidthPercentage} %)", LineWidthPercentage, 0f, 100f, 0.25f));
            listing.LabeledTextFieldNumeric($"{DawnTranslation.Label_LineThickness} (px)", ref LineThickness, ref m_LineThicknessBuffer, 0f, 100f);
            listing.LabeledTextFieldNumeric($"{DawnTranslation.Label_LinePadding} (px)", ref LinePadding, ref m_LinePaddingBuffer, 0f, 100f);
            listing.LabeledRadioColorPresets(ref LineColor, DawnTranslation.Label_LineColor);
        }

        private void ShowDurationSection(Listing_Standard listing)
        {
            listing.LabeledTextFieldNumeric($"{DawnTranslation.Label_DisplayDuration} (seconds)", ref DisplayDurationSeconds, ref m_DisplayDurationBuffer, 0f, 120f);
            listing.LabeledTextFieldNumeric($"{DawnTranslation.Label_FadeInDuration} (seconds)", ref FadeInDurationSeconds, ref m_FadeInDurationBuffer, 0f, DisplayDurationSeconds);
            listing.LabeledTextFieldNumeric($"{DawnTranslation.Label_FadeOutDuration} (seconds)", ref FadeOutDurationSeconds, ref m_FadeOutDurationBuffer, 0f, DisplayDurationSeconds);
        }

        private void ShowTextSection(Listing_Standard listing)
        {
            if (listing.SectionButton(DawnTranslation.Section_UpperText, ref m_TextUpperSection))
                listing.Indented(() => UpperTextStyle.DoContents(listing, Scale), 32f);

            if (listing.SectionButton(DawnTranslation.Section_BottomText, ref m_TextBottomSection))
                listing.Indented(() => BottomTextStyle.DoContents(listing, Scale), 32f);
        }

        private void ShowLabelFormatSection(Listing_Standard listing)
        {
            listing.LabeledTextEntry(DawnTranslation.Label_UpperTextFormat, ref UpperTextFormat, 0.5f);

            listing.Gap();

            listing.LabeledTextEntry(DawnTranslation.Label_BottomTextFormat, ref BottomTextFormat, 0.5f);

            listing.Gap();

            Color defaultColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.75f);

            listing.Label(DawnTranslation.Hint_LabelFormat);

            GUI.color = defaultColor;
        }

        private void ShowSoundSection(Listing_Standard listing)
        {
            listing.CheckboxLabeled(DawnTranslation.Label_Enabled, ref SoundEnabled);

            listing.Gap();

            if (listing.ButtonText($"{DawnTranslation.Label_Sound} ({m_SoundDefName})"))
                Find.WindowStack.Add(new Dialog_ChooseSound(soundDefName => m_SoundDefName = soundDefName, m_SoundDefName));

            SoundVolume = SettingsHelper.SnapToStep(listing.SliderLabeled($"{DawnTranslation.Label_SoundVolume} ({SoundVolume.ToStringPercent()})", SoundVolume, 0.01f, 2f), 0.01f);
            SoundPitch = SettingsHelper.SnapToStep(listing.SliderLabeled($"{DawnTranslation.Label_SoundPitch} ({SoundPitch.ToStringPercent()})", SoundPitch, 0.01f, 2f), 0.01f);
        }

        private void ShowExtraSection(Listing_Standard listing)
        {
            listing.CheckboxLabeled(DawnTranslation.Label_StartsAtZero, ref StartsAtZero);

            listing.Gap();

            listing.LabeledTextFieldNumeric(DawnTranslation.Label_ShowEveryXDays, ref ShowEveryXDays, ref m_ShowEveryXDaysBuffer, 1f, 1200f);
            TriggerHour = Mathf.CeilToInt(listing.SliderLabeled($"{DawnTranslation.Label_TriggerHour} ({TriggerHour:00}h)", TriggerHour, 0f, 23f, 0.35f));

            listing.Gap();

            GameFont defaultFont = Text.Font;
            Text.Font = GameFont.Medium;

            listing.Label(DawnTranslation.Label_DayRelativeTo);

            Text.Font = defaultFont;

            Rect radioRect = listing.GetRect(30f);

            var dayRelativeRadio = Enum.GetValues(typeof(DayRelative)).Cast<DayRelative>().ToArray();
            float itemWidth = radioRect.width / dayRelativeRadio.Length;

            for (int i = 0; i < dayRelativeRadio.Length; i++)
            {
                DayRelative dayRelative = dayRelativeRadio[i];

                Rect buttonRect = new Rect(radioRect.x + (i * itemWidth), radioRect.y, itemWidth, radioRect.height).MiddlePart(0.5f, 1f);

                TooltipHandler.TipRegion(buttonRect, DawnTranslation.SettingsTooltip_DayRelativeTo.TryGetValue(dayRelative, ""));
                if (Widgets.RadioButtonLabeled(buttonRect, DawnTranslation.Label_DayRelative.TryGetValue(dayRelative, ""), DayRelativeTo == dayRelative))
                    DayRelativeTo = dayRelative;
            }
        }

        public void UpdateText()
        {
            UpperTextStyle.UpdateFontFamily();
            BottomTextStyle.UpdateFontFamily();

            UpperTextStyle.ApplyToGUIStyle(Scale);
            BottomTextStyle.ApplyToGUIStyle(Scale);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            try
            {
                #region Header

                Scribe_Values.Look(ref Enabled, "Enabled", DawnDefault.Enabled);
                Scribe_Values.Look(ref ScreenshotMode, "ScreenshotMode", DawnDefault.ScreenshotMode);

                #endregion

                #region Appearance Section

                Scribe_Values.Look(ref ShowHighlight, "ShowHighlight", DawnDefault.ShowHighlight);

                Scribe_Values.Look(ref Scale, "Scale", DawnDefault.Scale);
                Scribe_Values.Look(ref Offset, "Offset", DawnDefault.Offset);

                Scribe_Values.Look(ref LineThickness, "LineThickness", DawnDefault.LineThickness);
                Scribe_Values.Look(ref LineWidthPercentage, "LineWidthPercentage100", DawnDefault.LineWidthPercentage);
                Scribe_Values.Look(ref LinePadding, "LinePadding", DawnDefault.LinePadding);
                Scribe_Values.Look(ref LineColor, "LineColor", DawnDefault.LineColor);

                #endregion

                #region Duration Section

                Scribe_Values.Look(ref DisplayDurationSeconds, "DisplayDurationSeconds", DawnDefault.DisplayDurationSeconds);
                Scribe_Values.Look(ref FadeInDurationSeconds, "FadeInDurationSeconds", DawnDefault.FadeInDurationSeconds);
                Scribe_Values.Look(ref FadeOutDurationSeconds, "FadeOutDurationSeconds", DawnDefault.FadeOutDurationSeconds);

                #endregion

                #region Text Section

                Scribe_Deep.Look(ref UpperTextStyle, "UpperTextStyle");
                Scribe_Deep.Look(ref BottomTextStyle, "BottomTextStyle");

                if (Scribe.mode == LoadSaveMode.PostLoadInit || Scribe.mode == LoadSaveMode.LoadingVars)
                {
                    if (UpperTextStyle == null)
                        UpperTextStyle = new DawnTextStyle(40, true);

                    if (BottomTextStyle == null)
                        BottomTextStyle = new DawnTextStyle();
                }

                #endregion

                #region Label Format Section

                Scribe_Values.Look(ref UpperTextFormat, "UpperTextFormat", "DAY {d}");
                Scribe_Values.Look(ref BottomTextFormat, "BottomTextFormat", "YEAR {Y} | {Q} | {S}");

                #endregion

                #region Sound Section

                Scribe_Values.Look(ref SoundEnabled, "SoundEnabled", DawnDefault.SoundEnabled);
                Scribe_Values.Look(ref m_SoundDefName, "SoundDefName", DawnData.DefaultSoundDefName);

                Scribe_Values.Look(ref SoundVolume, "SoundVolume", DawnDefault.SoundVolume);
                Scribe_Values.Look(ref SoundPitch, "SoundPitch", DawnDefault.SoundPitch);

                #endregion

                #region Extra Section

                Scribe_Values.Look(ref StartsAtZero, "StartsAtZero", DawnDefault.StartsAtZero);
                Scribe_Values.Look(ref ShowEveryXDays, "ShowEveryXDays", DawnDefault.ShowEveryXDays);
                Scribe_Values.Look(ref TriggerHour, "TriggerHour", DawnDefault.TriggerHour);
                Scribe_Values.Look(ref DayRelativeTo, "DayRelativeTo", DawnDefault.DayRelativeTo);

                #endregion
            }
            catch (Exception exception)
            {
                DawnData.Error($"Exception catched into DawnSettings.ExposeData()\nException: {exception}");
            }
        }
    }
}
