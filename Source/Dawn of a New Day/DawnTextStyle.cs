using System;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public class DawnTextStyle : IExposable
    {
        public GUIStyle TextGUIStyle = new GUIStyle(Text.CurFontStyle);

        public string FontFamilyName = "";
        public int FontSize = 24;
        public Color TextColor = Color.white;

        public bool Bold = false;
        public bool Italic = false;

        public float OutlineThickness = 1f;
        public Color OutlineColor = Color.black;

        private string m_FontSizeBuffer;
        private string m_OutlineThicknessBuffer;

        public DawnTextStyle()
        {
            TextGUIStyle.alignment = TextAnchor.MiddleCenter;
        }

        public DawnTextStyle(int fontSize, bool bold)
        {
            TextGUIStyle.alignment = TextAnchor.MiddleCenter;

            FontSize = fontSize;
            Bold = bold;
        }

        public void DoContents(Listing_Standard listing, float scale)
        {
            try
            {
                if (listing.ButtonText($"{DawnData.SettingsLabel_FontFamily} ({FontFamilyName.Fallback(DawnData.SettingsLabel_DefaultFont)})"))
                {
                    Find.WindowStack.Add(new Dialog_ChooseFontFamily(FontFamilyName, font =>
                    {
                        FontFamilyName = font;
                        UpdateFontFamily();
                    }));
                }

                listing.LabeledTextFieldNumeric(DawnData.SettingsLabel_FontSize, ref FontSize, ref m_FontSizeBuffer, 0f, 256f);
                listing.LabeledTextFieldNumeric(DawnData.SettingsLabel_OutlineThickness, ref OutlineThickness, ref m_OutlineThicknessBuffer, 0f, 10f);

                listing.CheckboxLabeled(DawnData.SettingsLabel_Bold, ref Bold);
                listing.CheckboxLabeled(DawnData.SettingsLabel_Italic, ref Italic);

                listing.LabeledRadioColorPresets(ref TextColor, DawnData.SettingsLabel_TextColor);
                listing.LabeledRadioColorPresets(ref OutlineColor, DawnData.SettingsLabel_OutlineColor);

                ApplyToGUIStyle(scale);
            }
            catch (Exception exception)
            {
                DawnData.Error($"Exception catched into DawnTextStyle.DoContents(listing: {listing})\nException: {exception}");
            }
        }

        public void ExposeData()
        {
            if (TextGUIStyle == null)
                TextGUIStyle = new GUIStyle(Text.CurFontStyle);

            Scribe_Values.Look(ref FontFamilyName, "FontFamilyName", "");
            Scribe_Values.Look(ref FontSize, "FontSize", 24);
            Scribe_Values.Look(ref TextColor, "TextColor", Color.white);

            Scribe_Values.Look(ref Bold, "Bold", false);
            Scribe_Values.Look(ref Italic, "Italic", false);

            Scribe_Values.Look(ref OutlineThickness, "OutlineThickness", 1f);
            Scribe_Values.Look(ref OutlineColor, "OutlineColor", Color.black);
        }

        public void ApplyToGUIStyle(float scale)
        {
            if (TextGUIStyle == null)
                return;

            TextGUIStyle.fontSize = Mathf.CeilToInt(FontSize * scale);
            TextGUIStyle.normal.textColor = TextColor;
            TextGUIStyle.fontStyle = SettingsHelper.MapFontStyle(Bold, Italic);
        }

        public void UpdateFontFamily()
        {
            if (TextGUIStyle == null)
                return;

            try
            {
                if (TextGUIStyle.font != null && TextGUIStyle.font != Text.CurFontStyle.font)
                    UnityEngine.Object.Destroy(TextGUIStyle.font);

                if (FontFamilyName.NullOrEmpty())
                {
                    TextGUIStyle.font = Text.CurFontStyle.font;
                    return;
                }

                TextGUIStyle.font = Font.CreateDynamicFontFromOSFont(FontFamilyName, 16);
            }
            catch (Exception exception)
            {
                DawnData.Error($"DawnTextStyle.UpdateFontFamily() Failed!\nFontFamilyName: '{FontFamilyName}'\nException: {exception}");
            }
        }
    }
}
