using UnityEngine;
using UnityEngine.UI;

namespace Bausteine
{
    /// <summary>
    /// Allgemeine Füllleiste (Leben-Kugel à la Diablo, Ausdauer, Erfahrung).
    /// Image auf "Image Type: Filled" stellen. Wählt automatisch die passende Quelle.
    /// </summary>
    public class ResourceBar : MonoBehaviour
    {
        public enum Source { Health, Stamina, Experience }

        [SerializeField] private Source source = Source.Health;
        [SerializeField] private Image fillImage;
        [Tooltip("Weiches Nachziehen der Leiste. 0 = sofort.")]
        [SerializeField] private float smoothSpeed = 5f;

        private float targetFill = 1f;

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            switch (source)
            {
                case Source.Health when player.TryGetComponent(out Health h):
                    h.Changed += (c, m) => SetFill(c, m);
                    SetFill(h.Current, h.Max);
                    break;
                case Source.Stamina when player.TryGetComponent(out Stamina s):
                    s.Changed += SetFill;
                    SetFill(s.Current, s.Max);
                    break;
                case Source.Experience when player.TryGetComponent(out ExperienceSystem xp):
                    xp.XpChanged += (c, m) => SetFill(c, m);
                    SetFill(xp.CurrentXp, xp.XpToNextLevel);
                    break;
            }
            fillImage.fillAmount = targetFill;
        }

        private void SetFill(float current, float max)
        {
            targetFill = max > 0f ? Mathf.Clamp01(current / max) : 0f;
        }

        private void Update()
        {
            fillImage.fillAmount = smoothSpeed <= 0f
                ? targetFill
                : Mathf.MoveTowards(fillImage.fillAmount, targetFill, smoothSpeed * Time.unscaledDeltaTime);
        }
    }
}
