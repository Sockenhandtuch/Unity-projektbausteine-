using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Schiebt das Objekt bei Schaden von der Schadensquelle weg.
    /// Bewegungsskripte fragen IsKnockedBack ab und pausieren solange ihre Steuerung.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Knockback : MonoBehaviour
    {
        [Tooltip("Wie lange der Rückstoß die eigene Steuerung übernimmt (Sekunden).")]
        [SerializeField] private float duration = 0.15f;

        [Tooltip("0 = voller Rückstoß, 1 = immun (z.B. schwere Bossgegner).")]
        [SerializeField, Range(0f, 1f)] private float resistance = 0f;

        private Rigidbody2D rb;
        private Health health;
        private float timer;

        public bool IsKnockedBack => timer > 0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            if (health != null) health.Damaged += OnDamaged;
        }

        private void OnDisable()
        {
            if (health != null) health.Damaged -= OnDamaged;
        }

        private void OnDamaged(DamageInfo info)
        {
            Apply(info.DirectionTo(rb.position), info.KnockbackForce);
        }

        /// <summary>Löst manuell einen Rückstoß aus (z.B. beim Treffen einer Wand mit dem Schwert).</summary>
        public void Apply(Vector2 direction, float force)
        {
            force *= 1f - resistance;
            if (force <= 0f || direction == Vector2.zero) return;

            rb.linearVelocity = direction.normalized * force;
            timer = duration;
        }

        private void FixedUpdate()
        {
            if (timer <= 0f) return;

            timer -= Time.fixedDeltaTime;
            if (timer <= 0f) rb.linearVelocity = Vector2.zero; // Rückstoß vorbei → abbremsen
        }
    }
}
