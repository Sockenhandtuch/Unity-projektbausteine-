using System.Collections;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// "I-Frames": Nach einem Treffer ist das Objekt kurz unverwundbar und blinkt.
    /// Klassisch für den Spieler (Zelda, Hollow Knight).
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class Invincibility : MonoBehaviour
    {
        [SerializeField] private float duration = 1f;
        [SerializeField] private float blinkInterval = 0.08f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Health health;
        private Coroutine routine;

        public bool IsActive => routine != null;

        private void Awake()
        {
            health = GetComponent<Health>();
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void OnEnable() => health.Damaged += OnDamaged;

        private void OnDisable()
        {
            health.Damaged -= OnDamaged;
            Stop(); // Aufräumen, falls das Objekt mitten in der Phase deaktiviert wird
        }

        private void OnDamaged(DamageInfo info)
        {
            if (health.IsAlive) Trigger(duration);
        }

        /// <summary>Startet eine Unverwundbarkeitsphase mit beliebiger Dauer.</summary>
        public void Trigger(float seconds)
        {
            Stop();
            routine = StartCoroutine(Routine(seconds));
        }

        private IEnumerator Routine(float seconds)
        {
            health.AddInvulnerability();

            float elapsed = 0f;
            while (elapsed < seconds)
            {
                if (spriteRenderer != null) spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval;
            }

            Stop();
        }

        private void Stop()
        {
            if (routine == null) return;

            StopCoroutine(routine);
            routine = null;
            health.RemoveInvulnerability();
            if (spriteRenderer != null) spriteRenderer.enabled = true;
        }
    }
}
