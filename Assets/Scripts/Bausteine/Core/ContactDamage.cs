using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Verursacht Schaden bei Körperkontakt (typisch für Gegner in Hollow Knight / Zelda).
    /// Funktioniert mit Triggern UND normalen Collidern.
    /// Dauerkontakt ist kein Problem: Die Unverwundbarkeitsphase (Invincibility) des Ziels
    /// verhindert Schaden in jedem Frame.
    /// </summary>
    public class ContactDamage : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private float knockbackForce = 8f;

        private TeamMember ownTeam;
        private Health ownHealth;

        private void Awake()
        {
            ownTeam = GetComponentInParent<TeamMember>();
            ownHealth = GetComponentInParent<Health>();
        }

        private void OnTriggerStay2D(Collider2D other) => TryDamage(other);
        private void OnCollisionStay2D(Collision2D collision) => TryDamage(collision.collider);

        private void TryDamage(Collider2D other)
        {
            // Tote Gegner verletzen niemanden mehr.
            if (ownHealth != null && !ownHealth.IsAlive) return;

            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target == null || !target.IsAlive) return;

            TeamMember targetTeam = other.GetComponentInParent<TeamMember>();
            // Kontaktschaden nur gegen echte Feinde – Gegner sollen keine Krüge zerstören.
            if (ownTeam == null || targetTeam == null || ownTeam.Faction == targetTeam.Faction) return;
            if (targetTeam.Faction == Faction.Neutral) return;

            target.TakeDamage(new DamageInfo(damage, transform.position, knockbackForce, gameObject));
        }
    }
}
