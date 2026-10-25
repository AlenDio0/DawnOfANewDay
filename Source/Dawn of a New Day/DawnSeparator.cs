using DawnNewDay.Utils;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public class DawnSeparator : IDawnWidget, IExposable
    {
        public float WidthPercentage = 80f;
        public float Thickness = 4f;
        public float Padding = 6f;
        public Color Color = Color.gray;

        public DawnSeparator()
        {
        }

        public DawnSeparator(float width, float thickness, float padding)
        {
            WidthPercentage = width;
            Thickness = thickness;
            Padding = padding;
        }

        public void DoContents(Listing_Standard listing)
        {
            WidthPercentage = Mathf.Ceil(listing.SliderLabeled($"{DawnTranslation.Label_WidthPercentage} ({WidthPercentage}%)", WidthPercentage, 0f, 100f, 0.25f));
            Thickness = Mathf.Ceil(listing.SliderLabeled($"{DawnTranslation.Label_Thickness} ({Thickness} px)", Thickness, 0f, 20f, 0.25f));
            Padding = Mathf.Ceil(listing.SliderLabeled($"{DawnTranslation.Label_Padding} ({Padding} px)", Padding, 0f, 100f, 0.25f));

            listing.Gap();

            listing.LabeledRadioColorPresets(ref Color, DawnTranslation.Label_Color);
        }

        public void Draw(Rect rect, float alpha = 1f) => Widgets.DrawBoxSolid(rect, Color.WithAlpha(alpha));

        public Rect CalcFullRect(float top, float maxWidth, float paddingScale)
        {
            Vector2 size = new(maxWidth * (WidthPercentage / 100f), Thickness + (Padding * paddingScale * 2f));
            Vector2 position = new((maxWidth - size.x) / 2f, top);
            return new(position, size);
        }

        public Rect CalcDrawRect(Rect rect, float paddingScale)
        {
            float scaledPadding = Padding * paddingScale;

            rect.y += scaledPadding;
            rect.height -= scaledPadding * 2f;
            return rect;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref WidthPercentage, "WidthPercentage", 80f);
            Scribe_Values.Look(ref Thickness, "Thickness", 4f);
            Scribe_Values.Look(ref Padding, "Padding", 6f);
            Scribe_Values.Look(ref Color, "Color", Color.gray);
        }
    }
}
