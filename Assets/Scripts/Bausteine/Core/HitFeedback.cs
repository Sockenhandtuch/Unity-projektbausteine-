using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Verbindet eine Hitbox mit Feedback-Effekten: Hit Stop, Kamerawackeln und Treffer-Partikel.
    /// Auf dasselbe Objekt wie die Hitbox legen.
    /// </summary>
    [RequireComponent(typeof(Hitbox))]
    public class HitFeedback : MonoBehaviour
    {
        [SerializeField] private float hitStopDuration = 0.04f;
        [SerializeField] private float shakeDuration = 0.1f;
        [SerializeField] private float shakeStrength = 0.08f;

        [Tooltip("Optional: Partikel/Sprite, der am Trefferpunkt erscheint.")]
        [SerializeField] private GameObject hitEffectPrefab;

        private Hitbox hitbox;

        private void Awake() => hitbox = GetComponent<Hitbox>();
        private void OnEnable() => hitbox.Hit += OnHit;
        private void OnDisable() => hitbox.Hit -= OnHit;

        private void OnHit(IDamageable target, Vector2 hitPoint)
        {
            if (HitStop.Instance != null) HitStop.Instance.Freeze(hitStopDuration);
            if (CameraShake.Instance != null) CameraShake.Instance.Shake(shakeDuration, shakeStrength);
            if (hitEffectPrefab != null) Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
        }
    }
}
