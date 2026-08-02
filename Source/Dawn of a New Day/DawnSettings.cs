using DawnNewDay.Dialogs;
using DawnNewDay.Utils;
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

        public DawnTextStyle UpperTextStyle = new DawnTextStyle(40, true);
        public DawnTextStyle BottomTextStyle = new DawnTextStyle();

        #endregion

        #region Label Format Section

        public string UpperTextFormat = "DAY {d}";
        public string BottomTextFormat = "YEAR {Y} | {Q} | {S}";

        #endregion

        #region Sound Section

        public bool SoundEnabled = true;

        private string m_SoundDefName = DawnData.DefaultSoundDefName;
        private SoundDef m_Sound;

        public SoundDef Sound => SoundDef.NamedSilentFail(m_SoundDefName) ?? SoundDef.NamedSilentFail(DawnData.DefaultSoundDefName);

        public float SoundVolume = 0.25f;
        public float SoundPitch = 1f;

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

        private string m_LineThicknessBuffer;
        private string m_LinePaddingBuffer;

        private string m_ShowEveryXDaysBuffer;

        #endregion

        bool m_AppearanceSection = false;
        bool m_DurationSection = false;
        bool m_TextSection = false;
        bool m_LabelFormatSection = false;
        bool m_SoundSection = false;
        bool m_ExtraSection = false;

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

                if (listing.SectionButton(DawnData.SettingsSection_Appearance, ref m_AppearanceSection))
                    listing.Indented(() => ShowAppearanceSection(listing));

                if (listing.SectionButton(DawnData.SettingsSection_Duration, ref m_DurationSection))
                    listing.Indented(() => ShowDurationSection(listing));

                if (listing.SectionButton(DawnData.SettingsSection_Text, ref m_TextSection))
                    listing.Indented(() => ShowTextSection(listing));

                if (listing.SectionButton(DawnData.SettingsSection_LabelFormat, ref m_LabelFormatSection))
                    listing.Indented(() => ShowLabelFormatSection(listing));

                if (listing.SectionButton(DawnData.SettingsSection_Sound, ref m_SoundSection))
                    listing.Indented(() => ShowSoundSection(listing));

                if (listing.SectionButton(DawnData.SettingsSection_Extra, ref m_ExtraSection))
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
                var scaleModes = SettingsHelper.CreateFloatMenu(DawnData.ScalePresets, scale => new FloatMenuOption($"{scale}x", () => Scale = scale));
                Find.WindowStack.Add(scaleModes);
            }

            if (listing.ButtonText(DawnData.SettingsLabel_OffsetPresets))
            {
                var offsetPresets = SettingsHelper.CreateFloatMenu(DawnData.OffsetPresets, item => new FloatMenuOption(item.Name, () =>
                    Offset = new Vector2(UI.screenWidth, UI.screenHeight) * item.Preset));
                Find.WindowStack.Add(offsetPresets);
            }

            Offset.x = Mathf.Ceil(listing.SliderLabeled($"{DawnData.SettingsLabel_Offset} X ({Offset.x} px)", Offset.x, 0f, UI.screenWidth, 0.25f));
            Offset.y = Mathf.Ceil(listing.SliderLabeled($"{DawnData.SettingsLabel_Offset} Y ({Offset.y} px)", Offset.y, 0f, UI.screenHeight, 0.25f));

            listing.Gap();

            LineWidthPercentage = Mathf.Ceil(listing.SliderLabeled($"{DawnData.SettingsLabel_LineWidthPercentage} ({LineWidthPercentage} %)", LineWidthPercentage, 0f, 100f, 0.25f));
            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_LineThickness} (px)", ref LineThickness, ref m_LineThicknessBuffer, 0f, 100f);
            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_LinePadding} (px)", ref LinePadding, ref m_LinePaddingBuffer, 0f, 100f);
            listing.LabeledRadioColorPresets(ref LineColor, DawnData.SettingsLabel_LineColor);
        }

        private void ShowDurationSection(Listing_Standard listing)
        {
            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_DisplayDuration} (seconds)", ref DisplayDurationSeconds, ref m_DisplayDurationBuffer, 0f, 120f);
            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_FadeInDuration} (seconds)", ref FadeInDurationSeconds, ref m_FadeInDurationBuffer, 0f, DisplayDurationSeconds);
            listing.LabeledTextFieldNumeric($"{DawnData.SettingsLabel_FadeOutDuration} (seconds)", ref FadeOutDurationSeconds, ref m_FadeOutDurationBuffer, 0f, DisplayDurationSeconds);
        }

        private void ShowTextSection(Listing_Standard listing)
        {
            GameFont defaultFont = Text.Font;

            Text.Font = GameFont.Medium;

            listing.Label(DawnData.SettingsLabel_UpperTextProperties);

            Text.Font = defaultFont;

            UpperTextStyle.DoContents(listing, Scale);

            listing.Gap();

            Text.Font = GameFont.Medium;

            listing.Label(DawnData.SettingsLabel_BottomTextProperties);

            Text.Font = defaultFont;

            BottomTextStyle.DoContents(listing, Scale);
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

        private void ShowSoundSection(Listing_Standard listing)
        {
            listing.CheckboxLabeled(DawnData.SettingsLabel_Enabled, ref SoundEnabled);

            listing.Gap();

            if (listing.ButtonText($"{DawnData.SettingsLabel_Sound} ({m_SoundDefName})"))
                Find.WindowStack.Add(new Dialog_ChooseSound(soundDefName => m_SoundDefName = soundDefName, m_SoundDefName));

            SoundVolume = SettingsHelper.SnapToStep(listing.SliderLabeled($"{DawnData.SettingsLabel_SoundVolume} ({SoundVolume.ToStringPercent()})", SoundVolume, 0.01f, 2f), 0.01f);
            SoundPitch = SettingsHelper.SnapToStep(listing.SliderLabeled($"{DawnData.SettingsLabel_SoundPitch} ({SoundPitch.ToStringPercent()})", SoundPitch, 0.01f, 2f), 0.01f);
        }

        private void ShowExtraSection(Listing_Standard listing)
        {
            listing.CheckboxLabeled(DawnData.SettingsLabel_StartsAtZero, ref StartsAtZero);

            listing.Gap();

            listing.LabeledTextFieldNumeric(DawnData.SettingsLabel_ShowEveryXDays, ref ShowEveryXDays, ref m_ShowEveryXDaysBuffer, 1f, 1200f);
            TriggerHour = Mathf.CeilToInt(listing.SliderLabeled($"{DawnData.SettingsLabel_TriggerHour} ({TriggerHour:00}h)", TriggerHour, 0f, 23f, 0.35f));

            listing.Gap();

            GameFont defaultFont = Text.Font;
            Text.Font = GameFont.Medium;

            listing.Label(DawnData.SettingsLabel_DayRelativeTo);

            Text.Font = defaultFont;

            Rect radioRect = listing.GetRect(30f);

            var dayRelativeRadio = Enum.GetValues(typeof(DayRelative)).Cast<DayRelative>().ToArray();
            float itemWidth = radioRect.width / dayRelativeRadio.Length;

            for (int i = 0; i < dayRelativeRadio.Length; i++)
            {
                DayRelative dayRelative = dayRelativeRadio[i];

                Rect buttonRect = new Rect(radioRect.x + (i * itemWidth), radioRect.y, itemWidth, radioRect.height).MiddlePart(0.5f, 1f);

                TooltipHandler.TipRegion(buttonRect, DawnData.SettingsTooltip_DayRelativeTo.TryGetValue(dayRelative, ""));
                if (Widgets.RadioButtonLabeled(buttonRect, DawnData.SettingsLabel_DayRelative.TryGetValue(dayRelative, ""), DayRelativeTo == dayRelative))
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

                Scribe_Values.Look(ref SoundEnabled, "SoundEnabled", true);
                Scribe_Values.Look(ref m_SoundDefName, "SoundDefName", DawnData.DefaultSoundDefName);

                Scribe_Values.Look(ref SoundVolume, "SoundVolume", 0.25f);
                Scribe_Values.Look(ref SoundPitch, "SoundPitch", 1f);

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
    }
}
