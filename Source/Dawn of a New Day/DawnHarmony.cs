using HarmonyLib;
using Verse;

namespace DawnNewDay
{
    [StaticConstructorOnStartup]
    public static class DawnHarmony
    {
        static DawnHarmony()
        {
            Harmony harmony = new Harmony("com.alendio.dawnofanewday");
            harmony.PatchAll();
        }
    }
}