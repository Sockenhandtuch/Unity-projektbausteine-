using System.Collections;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Nahkampfangriff mit Ankündigung ("Telegraph") und optionalem Vorstoß (Lunge).
    /// Die Ankündigung gibt dem Spieler Zeit zum Ausweichen – wichtig für faire Kämpfe.
    /// Aufbau wie beim Spieler: AttackPivot → Hitbox (rechts vom Pivot).
    /// </summary>
    public class EnemyMeleeAttack : EnemyAttack
    {
        [SerializeField] private Transform attackPivot;
        [SerializeField] private Hitbox hitbox;
        [SerializeField] private int baseDamage = 1;

        [Header("Timing")]
        [SerializeField] private float telegraphTime = 0.4f;
        [SerializeField] private float activeTime = 0.15f;
        [SerializeField] private float recoveryTime = 0.3f;

        [Header("Vorstoß")]
        [Tooltip("0 = kein Vorstoß. Sonst Geschwindigkeit während der aktiven Phase.")]
        [SerializeField] private float lungeSpeed = 0f;

        [Header("Ankündigung (optional)")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color telegraphColor = new Color(1f, 0.8f, 0.8f);

        private Rigidbody2D rb;
        private CharacterStats stats;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            stats = GetComponent<CharacterStats>();
            hitbox.gameObject.SetActive(false);
        }

        protected override IEnumerator PerformAttack(Transform target)
        {
            Vector2 direction = ((Vector2)(target.position - transform.position)).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            attackPivot.localRotation = Quaternion.Euler(0f, 0f, angle);

            if (rb != null) rb.linearVelocity = Vector2.zero;

            // 1. Ankündigen
            Color original = spriteRenderer != null ? spriteRenderer.color : Color.white;
            if (spriteRenderer != null) spriteRenderer.color = telegraphColor;
            yield return new WaitForSeconds(telegraphTime);
            if (spriteRenderer != null) spriteRenderer.color = original;

            // 2. Zuschlagen
            hitbox.Damage = DamageCalculator.CalculateOutgoing(baseDamage, stats, out bool crit);
            hitbox.IsCritical = crit;
            hitbox.gameObject.SetActive(true);
            if (rb != null && lungeSpeed > 0f) rb.linearVelocity = direction * lungeSpeed;

            yield return new WaitForSeconds(activeTime);

            hitbox.gameObject.SetActive(false);
            if (rb != null) rb.linearVelocity = Vector2.zero;

            // 3. Erholen – hier ist der Gegner verwundbar und angreifbar.
            yield return new WaitForSeconds(recoveryTime);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (hitbox != null) hitbox.gameObject.SetActive(false);
        }
    }
}
