#if RW_15

using UnityEngine;

namespace DawnNewDay
{
    public static class RimWorld15Patches
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Rect MiddlePart(this Rect rect, float widthPercentage, float heightPercentage)
        {
            float width = rect.width * widthPercentage;
            float height = rect.height * heightPercentage;
            float x = rect.x + (rect.width - width) / 2f;
            float y = rect.y + (rect.height - height) / 2f;

            return new Rect(x, y, width, height);
        }
    }
}

#endif