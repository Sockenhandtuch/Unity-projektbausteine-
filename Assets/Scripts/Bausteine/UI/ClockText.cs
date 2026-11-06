using TMPro;
using UnityEngine;

namespace Bausteine
{
    /// <summary>Zeigt Uhrzeit, Tag und Jahreszeit an, z.B. "Spring 3 – 08:40".</summary>
    [RequireComponent(typeof(TMP_Text))]
    public class ClockText : MonoBehaviour
    {
        [Tooltip("Uhrzeit nur in 10-Minuten-Schritten anzeigen (wie Stardew Valley).")]
        [SerializeField] private bool roundToTenMinutes = true;

        private TMP_Text text;

        private void Awake() => text = GetComponent<TMP_Text>();

        private void Update()
        {
            GameClock clock = GameClock.Instance;
            if (clock == null) return;

            int minute = roundToTenMinutes ? clock.Minute / 10 * 10 : clock.Minute;
            text.text = $"{clock.Season} {clock.Day} – {clock.Hour:00}:{minute:00}";
        }
    }
}
