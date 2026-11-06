using UnityEngine;
using UnityEngine.UI;

namespace Bausteine
{
    /// <summary>
    /// Färbt ein bildschirmfüllendes UI-Bild abhängig von der Uhrzeit ein
    /// (Morgenrot, heller Tag, Abendlicht, dunkelblaue Nacht).
    ///
    /// Aufbau: Canvas → Image über den ganzen Bildschirm, "Raycast Target" AUS.
    /// Den Farbverlauf im Inspector einstellen; Alpha bestimmt die Stärke
    /// (Mittag z.B. Alpha 0, Mitternacht Alpha ~0.6 dunkelblau).
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class DayNightOverlay : MonoBehaviour
    {
        [Tooltip("Links = 00:00 Uhr, Mitte = 12:00 Uhr, rechts = 24:00 Uhr.")]
        [SerializeField] private Gradient colorOverDay = CreateDefaultGradient();

        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
            image.raycastTarget = false;
        }

        // Sinnvoller Startwert: Nacht dunkelblau, Morgen-/Abendrot, mittags komplett durchsichtig.
        // (Ein leerer Gradient wäre weiß und undurchsichtig und würde das Bild verdecken.)
        private static Gradient CreateDefaultGradient()
        {
            var night = new Color(0.05f, 0.05f, 0.25f);
            var dawn = new Color(1f, 0.55f, 0.3f);
            var gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(night, 0f),
                    new GradientColorKey(dawn, 0.27f),
                    new GradientColorKey(Color.white, 0.4f),
                    new GradientColorKey(dawn, 0.78f),
                    new GradientColorKey(night, 0.9f),
                },
                new[]
                {
                    new GradientAlphaKey(0.55f, 0f),
                    new GradientAlphaKey(0.55f, 0.2f),
                    new GradientAlphaKey(0f, 0.38f),
                    new GradientAlphaKey(0f, 0.7f),
                    new GradientAlphaKey(0.25f, 0.8f),
                    new GradientAlphaKey(0.55f, 0.92f),
                });
            return gradient;
        }

        private void Update()
        {
            if (GameClock.Instance == null) return;
            image.color = colorOverDay.Evaluate(GameClock.Instance.DayProgress);
        }
    }
}
