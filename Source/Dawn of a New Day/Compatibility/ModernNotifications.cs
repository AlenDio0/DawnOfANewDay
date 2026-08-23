using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DawnNewDay.Compatibility
{
    /*
     *  Modern Notifications — Calendar API
     *  Class copied from Modern Notification - Calendar API (made by Astryl).
    */
    public static class ModernNotifications
    {
        public static readonly Type Api = AccessTools.TypeByName("ModernNotifications.CalendarAPI");
        public static bool Present => Api != null;

        static object Call(string name, Type[] sig, params object[] args) => Api == null ? null :
            AccessTools.Method(Api, name, sig)?.Invoke(null, args);

        public static IList Reminders() => (IList)Call("GetReminders", Type.EmptyTypes);
        public static IList AllOccasions() => (IList)Call("GetAllOccasions", Type.EmptyTypes);

        public static int Today() => (int)(Call("TodayAbsDay", Type.EmptyTypes) ?? -1);
        public static int Year() => (int)(Call("CurrentYear", Type.EmptyTypes) ?? -1);
    }

    public static class ModernNotificationUtility
    {
        public record struct YearTime(int DayOfYear, int Year);

        public struct Reminder
        {
            public enum RawReminderIndex
            {
                Year = 0,
                DayOfYear = 1,
                Title = 2,
                Message = 3,
                Severity = 4,
                Recurrence = 5,
                Fired = 6,
                IconIndex = 7,

                Length,
            }

            public enum Severity
            {
                Info = 0,
                Good = 1,
                Caution = 2,
                Urgent = 3,
            }

            public bool IsValid = false;

            public YearTime Time;
            public string Title;
            public string Message;
            public Severity SeverityType;
            public int Recurrence;
            public bool Fired;
            public int IconIndex;

            public Reminder() { }

            public Reminder(object[] rawReminder)
            {
                if (rawReminder.Length < (int)RawReminderIndex.Length)
                    return;
                IsValid = true;

                Time.Year = Retrieve<int>(rawReminder, RawReminderIndex.Year);
                Time.DayOfYear = Retrieve<int>(rawReminder, RawReminderIndex.DayOfYear);
                Title = Retrieve<string>(rawReminder, RawReminderIndex.Title);
                Message = Retrieve<string>(rawReminder, RawReminderIndex.Message);
                SeverityType = Retrieve<Severity>(rawReminder, RawReminderIndex.Severity);
                Recurrence = Retrieve<int>(rawReminder, RawReminderIndex.Recurrence);
                Fired = Retrieve<bool>(rawReminder, RawReminderIndex.Fired);
                IconIndex = Retrieve<int>(rawReminder, RawReminderIndex.IconIndex);
            }

            private readonly T Retrieve<T>(object[] rawReminder, RawReminderIndex index) => (T)rawReminder[(int)index];
        }

        public struct Occasion
        {
            public enum RawOccasionIndex
            {
                Category = 0,
                Label = 1,
                Detail = 2,
                DayOfYear = 3,
                Year = 4,
                OneShot = 5,
                Celebratory = 6,

                Length,
            }

            public bool IsValid = false;

            public string Category;
            public string Label;
            public string Detail;
            public YearTime Time;
            public bool OneShot;
            public bool Celebratory;

            public Occasion() { }

            public Occasion(object[] rawOccasion)
            {
                if (rawOccasion.Length < (int)RawOccasionIndex.Length)
                    return;
                IsValid = true;

                Category = Retrieve<string>(rawOccasion, RawOccasionIndex.Category);
                Label = Retrieve<string>(rawOccasion, RawOccasionIndex.Label);
                Detail = Retrieve<string>(rawOccasion, RawOccasionIndex.Detail);
                Time.DayOfYear = Retrieve<int>(rawOccasion, RawOccasionIndex.DayOfYear);
                Time.Year = Retrieve<int>(rawOccasion, RawOccasionIndex.Year);
                OneShot = Retrieve<bool>(rawOccasion, RawOccasionIndex.OneShot);
                Celebratory = Retrieve<bool>(rawOccasion, RawOccasionIndex.Celebratory);
            }

            private readonly T Retrieve<T>(object[] rawOccasion, RawOccasionIndex index) => (T)rawOccasion[(int)index];
        }

        public static IEnumerable<Reminder> GetReminders() => ModernNotifications.Reminders()
            .Cast<object[]>().Select(rawReminder => new Reminder(rawReminder));

        public static IEnumerable<Occasion> GetOccasions() => ModernNotifications.AllOccasions()
            .Cast<object[]>().Select(rawOccasion => new Occasion(rawOccasion));
    }
}
