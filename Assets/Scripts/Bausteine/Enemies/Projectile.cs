using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Ein fliegendes Geschoss (Pfeil, Feuerball, Zauber).
    /// Trifft feindliche Ziele, zerschellt an Wänden und verschwindet nach einer Lebensdauer.
    /// Prefab: SpriteRenderer + Collider2D (Trigger) + Rigidbody2D (Kinematic) + Projectile.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 8f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private float knockbackForce = 4f;
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private GameObject impactEffectPrefab;

        private Rigidbody2D rb;
        private GameObject owner;
        private TeamMember ownerTeam;
        private int damage;
        private bool isCritical;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            GetComponent<Collider2D>().isTrigger = true;
        }

        /// <summary>Startet das Geschoss. Wird direkt nach Instantiate aufgerufen.</summary>
        public void Launch(Vector2 direction, int damageAmount, GameObject shooter, bool critical = false)
        {
            owner = shooter;
            ownerTeam = shooter != null ? shooter.GetComponentInParent<TeamMember>() : null;
            damage = damageAmount;
            isCritical = critical;

            rb.linearVelocity = direction.normalized * speed;

            // Sprite in Flugrichtung drehen (Sprite muss nach rechts zeigen).
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Wand getroffen?
            if ((obstacleMask.value & (1 << other.gameObject.layer)) != 0)
            {
                Impact();
                return;
            }

            if (owner != null && other.transform.IsChildOf(owner.transform)) return;

            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target == null || !target.IsAlive) return;
            if (!TeamMember.AreHostile(ownerTeam, other.GetComponentInParent<TeamMember>())) return;

            target.TakeDamage(new DamageInfo(damage, transform.position, knockbackForce, owner, isCritical));
            Impact();
        }

        private void Impact()
        {
            if (impactEffectPrefab != null) Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
