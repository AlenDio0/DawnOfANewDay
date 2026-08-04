using HarmonyLib;
using System;
using System.Collections;

namespace DawnNewDay.Compatibility
{
    public static class ModernNotifications
    {
        public static readonly Type Api = AccessTools.TypeByName("ModernNotifications.CalendarAPI");
        public static bool Present => Api != null;

        static object Call(string name, Type[] sig, params object[] args) => Api == null ? null :
            AccessTools.Method(Api, name, sig)?.Invoke(null, args);

        public static IList GetReminders() => (IList)Call("GetReminders", Type.EmptyTypes);

        public static IList GetAllOccasions() => (IList)Call("GetAllOccasions", Type.EmptyTypes);
        public static IList OnDay(int d, int y) => (IList)Call("GetOccasionsOnDay", new[] { typeof(int), typeof(int) }, d, y);

        public static int TodayAbsDay() => (int)(Call("TodayAbsDay", Type.EmptyTypes) ?? -1);
        public static int CurrentYear() => (int)(Call("CurrentYear", Type.EmptyTypes) ?? -1);
    }

    public static class ModernNotificationUtils
    {
        public class Reminder
        {
            public int Year;
            public int DayOfYear;
            public string Title;
            public string Message;
            public int Severity;
            public int Recurrence;
            public bool Fired;
            public int IconIndex;
        }

        public static Reminder MapReminder(object[] rawReminder)
        {
            if (rawReminder.Length < 8)
                return null;

            return new Reminder
            {
                Year = (int)rawReminder[0],
                DayOfYear = (int)rawReminder[1],
                Title = (string)rawReminder[2],
                Message = (string)rawReminder[3],
                Severity = (int)rawReminder[4],
                Recurrence = (int)rawReminder[5],
                Fired = (bool)rawReminder[6],
                IconIndex = (int)rawReminder[7]
            };
        }
    }
}
