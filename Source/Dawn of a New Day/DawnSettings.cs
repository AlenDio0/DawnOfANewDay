using DawnNewDay.Compatibility;
using DawnNewDay.Dialogs;
using DawnNewDay.Utils;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
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

        private bool m_AppearanceSection = false;

        public bool ShowHighlight = DawnDefault.ShowHighlight;

        public float Scale = DawnDefault.Scale;
        public Vector2 Offset = DawnDefault.Offset;

        public float SubtitleGap = DawnDefault.SubtitleGap;

        public float LineWidthPercentage = DawnDefault.LineWidthPercentage;
        public float LineThickness = DawnDefault.LineThickness;
        public float LinePadding = DawnDefault.LinePadding;
        public Color LineColor = DawnDefault.LineColor;

        #endregion

        #region Duration Section

        private bool m_DurationSection = false;

        public float DisplayDurationSeconds = DawnDefault.DisplayDurationSeconds;
        public float FadeInDurationSeconds = DawnDefault.FadeInDurationSeconds;
        public float FadeOutDurationSeconds = DawnDefault.FadeOutDurationSeconds;

        #endregion

        #region Text Section

        private bool m_TextSection = false;

        private bool m_UpperTextSection = false;
        public DawnTextStyle UpperTextStyle = DawnDefault.UpperTextStyle;

        private bool m_BottomTextSection = false;
        public DawnTextStyle BottomTextStyle = DawnDefault.BottomTextStyle;

        private bool m_SubtitleTextSection = false;
        public DawnTextStyle SubtitleTextStyle = DawnDefault.SubtitleTextStyle;

        #endregion

        #region Label Format Section

        private bool m_LabelFormatSection = false;

        public string UpperTextFormat = DawnDefault.UpperTextFormat;
        public string BottomTextFormat = DawnDefault.BottomTextFormat;
        public string SubtitleTextFormat = DawnDefault.SubtitleTextFormat;

        #endregion

        #region Sound Section

        private bool m_SoundSection = false;

        public bool SoundEnabled = DawnDefault.SoundEnabled;

        private string m_SoundDefName = DawnData.DefaultSoundDefName;

        public SoundDef Sound => DefDatabase<SoundDef>.GetNamedSilentFail(m_SoundDefName) ?? DefDatabase<SoundDef>.GetNamedSilentFail(DawnData.DefaultSoundDefName);

        public float SoundVolume = DawnDefault.SoundVolume;
        public float SoundPitch = DawnDefault.SoundPitch;

        #endregion

        #region Extra Section

        private bool m_ExtraSection = false;

        public bool StartsAtZero = DawnDefault.StartsAtZero;
        public int ShowEveryXDays = DawnDefault.ShowEveryXDays;
        public int TriggerHour = DawnDefault.TriggerHour;

        #endregion

        #region Compatibility Section

        private bool m_CompatibilitySection = false;

        #region Modern Notifications Section

        private bool m_MN_CompatibilitySection = false;

        private bool m_MN_ReminderTextSection = false;
        public ModernNotificationsSettings MN_Reminder = DawnDefault.MN_Reminder;

        private bool m_MN_OccasionTextSection = false;
        public ModernNotificationsSettings MN_Occasion = DawnDefault.MN_Occasion;

        #endregion

        #endregion

        #region Buffers

        private string m_DisplayDurationBuffer;
        private string m_FadeInDurationBuffer;
        private string m_FadeOutDurationBuffer;

        private string m_ShowEveryXDaysBuffer;

        #endregion

        private Vector2 m_ScrollPosition = Vector2.zero;
        private float m_CachedScrollViewHeight = 0f;

        public void DoWindowContents(Rect canva)
        {
            canva = canva.LeftPart(0.95f);
            canva = canva.MiddlePart(1f, 0.95f);

            Listing_Standard listing = new() { maxOneColumn = true };
            listing.Begin(canva);

            try
            {
                ShowHeader(listing);
                float headerHeight = listing.CurHeight;

                listing.End();

                Rect outRect = new(canva.x, canva.y + headerHeight, canva.width, canva.height - headerHeight);
                Rect viewRect = new(0f, 0f, outRect.width - 32f, Mathf.Max(outRect.height, m_CachedScrollViewHeight));
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

                if (listing.SectionButton(DawnTranslation.Section_Compatibility, ref m_CompatibilitySection))
                    listing.Indented(() => ShowCompatibilitySection(listing), 16f);

            }
            catch (Exception exception)
            {
                DawnData.Exception(exception);
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
                var offsetPresets = SettingsHelper.CreateFloatMenu(DawnTranslation.OffsetPresets, item => new FloatMenuOption(item.Name, () =>
                    Offset = new Vector2(UI.screenWidth, UI.screenHeight) * item.Preset));
                Find.WindowStack.Add(offsetPresets);
            }

            Offset.x = Mathf.Ceil(listing.SliderLabeled($"{DawnTranslation.Label_Offset} X ({Offset.x} px)", Offset.x, 0f, UI.screenWidth, 0.25f));
            Offset.y = Mathf.Ceil(listing.SliderLabeled($"{DawnTranslation.Label_Offset} Y ({Offset.y} px)", Offset.y, 0f, UI.screenHeight, 0.25f));

            listing.Gap();

            SubtitleGap = Mathf.Ceil(listing.SliderLabeled($"{DawnTranslation.Label_SubtitleGap} ({SubtitleGap} px)", SubtitleGap, 0f, 100f, 0.25f));

            listing.Gap();

            LineWidthPercentage = Mathf.Ceil(listing.SliderLabeled($"{DawnTranslation.Label_LineWidthPercentage} ({LineWidthPercentage}%)", LineWidthPercentage, 0f, 100f, 0.25f));
            LineThickness = Mathf.Ceil(listing.SliderLabeled($"{DawnTranslation.Label_LineThickness} ({LineThickness} px)", LineThickness, 0f, 100f, 0.25f));
            LinePadding = Mathf.Ceil(listing.SliderLabeled($"{DawnTranslation.Label_LinePadding} ({LinePadding} px)", LinePadding, 0f, 100f, 0.25f));
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
            if (listing.SectionButton(DawnTranslation.Section_UpperText, ref m_UpperTextSection))
                listing.Indented(() => UpperTextStyle.DoContents(listing, Scale), 32f);

            if (listing.SectionButton(DawnTranslation.Section_BottomText, ref m_BottomTextSection))
                listing.Indented(() => BottomTextStyle.DoContents(listing, Scale), 32f);

            if (listing.SectionButton(DawnTranslation.Section_SubtitleText, ref m_SubtitleTextSection))
                listing.Indented(() => SubtitleTextStyle.DoContents(listing, Scale), 32f);
        }

        private void ShowLabelFormatSection(Listing_Standard listing)
        {
            listing.Label(DawnTranslation.Label_UpperTextFormat);
            UpperTextFormat = listing.TextEntry(UpperTextFormat, 2);

            listing.Gap();

            listing.Label(DawnTranslation.Label_BottomTextFormat);
            BottomTextFormat = listing.TextEntry(BottomTextFormat, 2);

            listing.Gap();

            listing.Label(DawnTranslation.Label_SubtitleTextFormat);
            SubtitleTextFormat = listing.TextEntry(SubtitleTextFormat, 2);

            listing.Gap();

            if (listing.ButtonText(DawnTranslation.Label_LabelFormatPresets))
                Find.WindowStack.Add(new Dialog_Hint(DawnTranslation.Label_LabelFormatPresets, DawnTranslation.Hints_LabelFormatPreset));

            listing.Gap();

            Rect hintsRect = listing.GetRect(30f);

            if (Widgets.ButtonText(hintsRect.LeftHalf(), DawnTranslation.Label_LabelFormatHints))
            {
                var hints = DawnTranslation.Hints_LabelFormat;
                if (ModernNotifications.Present)
                    hints.AddRange(DawnTranslation.Hints_MN_LabelFormat);

                Find.WindowStack.Add(new Dialog_Hint(DawnTranslation.Label_LabelFormatHints, hints));
            }

            if (Widgets.ButtonText(hintsRect.RightHalf(), DawnTranslation.Label_RichTextHints))
                Find.WindowStack.Add(new Dialog_Hint(DawnTranslation.Label_RichTextHints, DawnTranslation.Hints_RichText));
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
        }

        private void ShowCompatibilitySection(Listing_Standard listing)
        {
            if (listing.SectionButton(DawnTranslation.MN_ModName, ref m_MN_CompatibilitySection))
                listing.Indented(() => ShowMNCompatibilitySection(listing));
        }

        private void ShowMNCompatibilitySection(Listing_Standard listing)
        {
            if (!ModernNotifications.Present)
            {
                SettingsHelper.WithGUIColor(Color.grey, () => listing.Label("Mod is not installed or isn't compatibile with the current version!"));
                return;
            }

            if (listing.SectionButton(DawnTranslation.Section_MN_Reminder, ref m_MN_ReminderTextSection))
                listing.Indented(() => MN_Reminder.DoContents(listing, Scale), 32f);

            if (listing.SectionButton(DawnTranslation.Section_MN_Occasion, ref m_MN_OccasionTextSection))
                listing.Indented(() => MN_Occasion.DoContents(listing, Scale), 32f);
        }

        public void UpdateText()
        {
            UpperTextStyle.UpdateFontFamily();
            UpperTextStyle.ApplyToGUIStyle(Scale);
         
            BottomTextStyle.UpdateFontFamily();
            BottomTextStyle.ApplyToGUIStyle(Scale);
            
            SubtitleTextStyle.UpdateFontFamily();
            SubtitleTextStyle.ApplyToGUIStyle(Scale);

            if (ModernNotifications.Present)
            {
                MN_Reminder.TextStyle.UpdateFontFamily();
                MN_Reminder.TextStyle.ApplyToGUIStyle(Scale);

                MN_Occasion.TextStyle.UpdateFontFamily();
                MN_Occasion.TextStyle.ApplyToGUIStyle(Scale);
            }
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
                Scribe_Deep.Look(ref SubtitleTextStyle, "SubtitleTextStyle");

                if (Scribe.mode == LoadSaveMode.PostLoadInit || Scribe.mode == LoadSaveMode.LoadingVars)
                {
                    UpperTextStyle ??= DawnDefault.UpperTextStyle;
                    BottomTextStyle ??= DawnDefault.BottomTextStyle;
                    SubtitleTextStyle ??= DawnDefault.SubtitleTextStyle;
                }

                #endregion

                #region Label Format Section

                Scribe_Values.Look(ref UpperTextFormat, "UpperTextFormat1", DawnDefault.UpperTextFormat);
                Scribe_Values.Look(ref BottomTextFormat, "BottomTextFormat1", DawnDefault.BottomTextFormat);
                Scribe_Values.Look(ref SubtitleTextFormat, "SubtitleTextFormat", DawnDefault.SubtitleTextFormat);

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

                #endregion

                #region Compatibility Section

                #region Modern Notifications Section

                Scribe_Deep.Look(ref MN_Reminder, "MN_Reminder");
                Scribe_Deep.Look(ref MN_Occasion, "MN_Occasion");

                if (Scribe.mode == LoadSaveMode.PostLoadInit || Scribe.mode == LoadSaveMode.LoadingVars)
                {
                    MN_Reminder ??= DawnDefault.MN_Reminder;
                    MN_Reminder.TextStyle ??= DawnDefault.MN_ReminderTextStyle;

                    MN_Occasion ??= DawnDefault.MN_Occasion;
                    MN_Occasion.TextStyle ??= DawnDefault.MN_OccasionTextStyle;
                }

                #endregion

                #endregion
            }
            catch (Exception exception)
            {
                DawnData.Exception(exception);
            }
        }
    }
}
