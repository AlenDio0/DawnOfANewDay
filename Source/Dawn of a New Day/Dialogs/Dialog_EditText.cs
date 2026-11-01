using System;
using UnityEngine;
using Verse;

namespace DawnNewDay.Dialogs
{
    public class Dialog_EditText : Window
    {
        public DawnText Text { get; }
        public int Index { get; }
        public float Scale { get; }
        public FormatContext Context { get; }

        public override Vector2 InitialSize => new(900f, 600f);

        public Dialog_EditText(DawnText textStyle, int textIndex, float scale, FormatContext context)
        {
            doCloseX = true;
            doCloseButton = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
            draggable = true;

            Text = textStyle;
            Index = textIndex;
            Scale = scale;
            Context = context;
        }

        public override void DoWindowContents(Rect canva)
        {
            Listing_Standard listing = new() { maxOneColumn = true };
            listing.Begin(canva);

            try
            {
                Rect previewRect = listing.GetRect(DawnData.BigRowHeight);

                float textScale = Text.FontSize <= previewRect.height ? 1f : previewRect.height / Text.FontSize;
                string label = Context.FormatText(Text.Format, Index, textScale);

                DawnMod.Settings.DrawPreviewText(previewRect, Text, label, textScale);

                listing.Gap();

                Text.DoContents(listing, Scale);
            }
            catch (Exception exception)
            {
                DawnData.Exception(exception);
            }
            finally
            {
                listing.End();
            }
        }

        public override void PostClose()
        {
            DawnMod.Settings.UpdateTextPreviewLabels();
        }
    }
}
