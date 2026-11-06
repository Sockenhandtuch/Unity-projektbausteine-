using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Ein Hindernis, das nur mit einer bestimmten Fähigkeit passierbar ist
    /// (tiefes Wasser → Schwimmen, schwerer Fels → Krafthandschuh).
    /// Der Collider blockiert weiterhin alle anderen (Gegner!), nur der Spieler
    /// mit der passenden Fähigkeit ignoriert ihn.
    /// Collider NICHT als Trigger einstellen.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class AbilityGate : MonoBehaviour
    {
        [SerializeField] private AbilityType requiredAbility;

        private Collider2D gateCollider;

        private void Awake() => gateCollider = GetComponent<Collider2D>();

        // Läuft der Spieler gegen das Hindernis, prüfen wir, ob er hindurch darf.
        // Schaltet er die Fähigkeit später frei, greift die Prüfung beim nächsten Kontakt.
        private void OnCollisionEnter2D(Collision2D collision) => Evaluate(collision.collider);
        private void OnCollisionStay2D(Collision2D collision) => Evaluate(collision.collider);

        private void Evaluate(Collider2D other)
        {
            PlayerAbilities abilities = other.GetComponentInParent<PlayerAbilities>();
            if (abilities != null && abilities.Has(requiredAbility))
                Physics2D.IgnoreCollision(gateCollider, other, true);
        }
    }
}
