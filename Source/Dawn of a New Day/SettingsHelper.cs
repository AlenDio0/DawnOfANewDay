using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public static class SettingsHelper
    {
        public static Rect TakeHeight(this ref Rect canva, float heightPct, float spacePct = 0f)
        {
            Rect part = canva.TopPart(heightPct);
            canva = canva.BottomPart(1f - heightPct - spacePct);
            return part;
        }

        public static FloatMenu CreateFloatMenu<T>(IEnumerable<T> values, Func<T, FloatMenuOption> optionFactory)
        {
            var options = new List<FloatMenuOption>();

            foreach (var value in values)
                options.Add(optionFactory(value));

            return new FloatMenu(options);
        }

        public static void LabeledTextFieldNumeric<T>(this Listing_Standard listing, string label, ref T value, ref string buffer, float min, float max, float labelWidthPct, float middleWidthPct = 0.75f, float height = 30f) where T : struct
        {
            Rect entryRect = listing.LabelWithWidget(label, labelWidthPct, middleWidthPct, height);
            Widgets.TextFieldNumeric(entryRect, ref value, ref buffer, min, max);
        }

        public static void LabeledTextEntry(this Listing_Standard listing, string label, ref string value, float labelWidthPct, float middleWidthPct = 0.75f, float height = 30f)
        {
            Rect entryRect = listing.LabelWithWidget(label, labelWidthPct, middleWidthPct, height);
            value = Widgets.TextField(entryRect, value);
        }

        public static Rect LabelWithWidget(this Listing_Standard listing, string label, float labelWidthPct, float middleWidthPct = 0.75f, float height = 30f)
        {
            Rect lineRect = listing.GetRect(height);
            Rect labelRect = lineRect.LeftPart(labelWidthPct).CenteredOnYIn(lineRect);

            TextAnchor defaultAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(labelRect, label);
            Text.Anchor = defaultAnchor;

            return lineRect.RightPart(1f - labelWidthPct).MiddlePart(middleWidthPct, 1f);
        }
    }
}
