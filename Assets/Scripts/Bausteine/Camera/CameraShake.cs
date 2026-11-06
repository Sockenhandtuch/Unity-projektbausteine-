using System.Collections;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Bildschirmwackeln bei Treffern, Explosionen, Bosslandungen.
    /// Gehört auf die Kamera, die ein KIND des CameraRig ist (siehe CameraFollow),
    /// weil hier nur die lokale Position verändert wird.
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }

        private Vector3 originalLocalPosition;
        private float remaining;
        private float strength;

        private void Awake()
        {
            Instance = this;
            originalLocalPosition = transform.localPosition;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>Startet ein Wackeln. Stärkeres Wackeln überschreibt schwächeres.</summary>
        public void Shake(float duration, float magnitude)
        {
            remaining = Mathf.Max(remaining, duration);
            strength = Mathf.Max(strength, magnitude);
        }

        private void LateUpdate()
        {
            if (remaining <= 0f) return;

            // unscaledDeltaTime: wackelt auch während des Hit Stops weiter.
            remaining -= Time.unscaledDeltaTime;
            if (remaining <= 0f)
            {
                strength = 0f;
                transform.localPosition = originalLocalPosition;
                return;
            }

            transform.localPosition = originalLocalPosition + (Vector3)(Random.insideUnitCircle * strength);
        }
    }
}
