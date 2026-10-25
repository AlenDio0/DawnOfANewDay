using System;
using UnityEngine;
using Verse;

namespace DawnNewDay.Dialogs
{
    public class Dialog_EditSeparator : Window
    {
        public DawnSeparator Separator { get; }

        public override Vector2 InitialSize => new(800f, 600f);

        public Dialog_EditSeparator(DawnSeparator separator)
        {
            doCloseButton = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
            draggable = true;

            Separator = separator;
        }

        public override void DoWindowContents(Rect canva)
        {
            Listing_Standard listing = new() { maxOneColumn = true };
            listing.Begin(canva);

            try
            {
                listing.Gap();

                Rect previewRect = listing.GetRect(100f);

                const float cPaddingScale = 0.25f;
                Rect separatorFullRect = Separator.CalcFullRect(previewRect.y, previewRect.width, cPaddingScale);
                Separator.Draw(Separator.CalcDrawRect(separatorFullRect, cPaddingScale));

                listing.Gap();

                Separator.DoContents(listing);
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
    }
}
