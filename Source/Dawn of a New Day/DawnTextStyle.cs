using DawnNewDay.Dialogs;
using DawnNewDay.Utils;
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

        public DawnTextStyle()
        {
            TextGUIStyle.alignment = TextAnchor.MiddleCenter;
            TextGUIStyle.font = null;
        }

        public DawnTextStyle(int fontSize, bool bold)
        {
            TextGUIStyle.alignment = TextAnchor.MiddleCenter;
            TextGUIStyle.font = null;

            FontSize = fontSize;
            Bold = bold;
        }

        public void DoContents(Listing_Standard listing, float scale)
        {
            try
            {
                if (listing.ButtonText($"{DawnData.Label_FontFamily} ({FontFamilyName.Fallback(DawnData.Label_DefaultFont)})"))
                {
                    Find.WindowStack.Add(new Dialog_ChooseFontFamily(font =>
                    {
                        FontFamilyName = font != DawnData.Label_DefaultFont ? font : "";
                        UpdateFontFamily();
                    }, FontFamilyName));
                }

                FontSize = Mathf.CeilToInt(listing.SliderLabeled($"{DawnData.Label_FontSize} ({FontSize})", FontSize, 0f, 256f, 0.35f));
                OutlineThickness = SettingsHelper.SnapToStep(listing.SliderLabeled($"{DawnData.Label_OutlineThickness} ({OutlineThickness})", OutlineThickness, 0f, 10f, 0.35f), 0.25f);

                listing.CheckboxLabeled(DawnData.Label_Bold, ref Bold);
                listing.CheckboxLabeled(DawnData.Label_Italic, ref Italic);

                listing.LabeledRadioColorPresets(ref TextColor, DawnData.Label_TextColor);
                listing.LabeledRadioColorPresets(ref OutlineColor, DawnData.Label_OutlineColor);

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
                if (TextGUIStyle.font != null)
                    UnityEngine.Object.Destroy(TextGUIStyle.font);

                if (FontFamilyName.NullOrEmpty())
                {
                    TextGUIStyle.font = null;
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
