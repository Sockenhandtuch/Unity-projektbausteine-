using System;
using UnityEngine;

namespace Bausteine
{
    public enum Season { Spring, Summer, Autumn, Winter }

    /// <summary>
    /// Spielzeit mit Tagen, Jahreszeiten und Jahren (Stardew-Valley-Stil).
    /// Andere Systeme (Pflanzen, Licht, Läden) hören auf die Events.
    /// Einmal pro Szene anlegen.
    /// </summary>
    public class GameClock : MonoBehaviour
    {
        public static GameClock Instance { get; private set; }

        private const int MinutesPerDay = 24 * 60;

        [Tooltip("Echte Sekunden pro Spielminute. 0.7 ≈ ein Tag dauert ~14 echte Minuten.")]
        [SerializeField] private float realSecondsPerGameMinute = 0.7f;
        [SerializeField, Range(0, 23)] private int morningHour = 6;
        [SerializeField, Min(1)] private int daysPerSeason = 28;

        private float accumulator;

        /// <summary>Minuten seit Mitternacht (0..1439).</summary>
        public int MinuteOfDay { get; private set; }
        public int Day { get; private set; } = 1;
        public Season Season { get; private set; } = Season.Spring;
        public int Year { get; private set; } = 1;

        public int Hour => MinuteOfDay / 60;
        public int Minute => MinuteOfDay % 60;

        /// <summary>Tagesfortschritt 0..1 (0 = Mitternacht, 0.5 = Mittag).</summary>
        public float DayProgress => MinuteOfDay / (float)MinutesPerDay;

        /// <summary>Uhrzeit als Text, z.B. "06:30".</summary>
        public string TimeText => $"{Hour:00}:{Minute:00}";

        /// <summary>Zeit anhalten (Menüs, Dialoge, Cutscenes).</summary>
        public bool IsPaused { get; set; }

        public event Action<int> MinuteChanged;  // Minute des Tages
        public event Action<int> HourChanged;    // Neue Stunde
        public event Action<int> DayStarted;     // Neuer Tag (Tag der Jahreszeit)
        public event Action<Season> SeasonChanged;

        private void Awake()
        {
            Instance = this;
            MinuteOfDay = morningHour * 60;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            if (IsPaused) return;

            accumulator += Time.deltaTime;
            while (accumulator >= realSecondsPerGameMinute)
            {
                accumulator -= realSecondsPerGameMinute;
                AdvanceMinute();
            }
        }

        private void AdvanceMinute()
        {
            MinuteOfDay++;

            // Um Mitternacht: Neuer Tag beginnt automatisch am Morgen
            // (in Stardew "fällt man um", wenn man nicht schlafen geht).
            if (MinuteOfDay >= MinutesPerDay)
            {
                StartNextDay();
                return;
            }

            MinuteChanged?.Invoke(MinuteOfDay);
            if (Minute == 0) HourChanged?.Invoke(Hour);
        }

        /// <summary>Schläft bis zum nächsten Morgen (Bett).</summary>
        public void SleepUntilMorning() => StartNextDay();

        private void StartNextDay()
        {
            Day++;
            if (Day > daysPerSeason)
            {
                Day = 1;
                Season = (Season)(((int)Season + 1) % 4);
                if (Season == Season.Spring) Year++;
                SeasonChanged?.Invoke(Season);
            }

            MinuteOfDay = morningHour * 60;
            accumulator = 0f;
            DayStarted?.Invoke(Day);
            HourChanged?.Invoke(Hour);
        }

        /// <summary>Stellt einen gespeicherten Zeitpunkt wieder her.</summary>
        public void Restore(int day, Season season, int year, int minuteOfDay)
        {
            Day = Mathf.Max(1, day);
            Season = season;
            Year = Mathf.Max(1, year);
            MinuteOfDay = Mathf.Clamp(minuteOfDay, 0, MinutesPerDay - 1);
            HourChanged?.Invoke(Hour);
        }
    }
}
