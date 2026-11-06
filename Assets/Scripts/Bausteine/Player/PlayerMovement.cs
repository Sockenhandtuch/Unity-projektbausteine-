using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Top-Down-Bewegung in 8 Richtungen mit sanftem Beschleunigen und Abbremsen.
    /// Andere Bausteine können die Bewegung sperren (Dash) oder verlangsamen (Angriff).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;

        [Tooltip("Wird ignoriert, wenn CharacterStats mit 'MoveSpeed' vorhanden ist.")]
        [SerializeField] private float moveSpeed = 5f;

        [Tooltip("Wie schnell die Zielgeschwindigkeit erreicht wird. Hoch = knackig, niedrig = rutschig (Eis!).")]
        [SerializeField] private float acceleration = 80f;

        private Rigidbody2D rb;
        private CharacterStats stats;
        private Knockback knockback;

        /// <summary>Multiplikator für die Geschwindigkeit (1 = normal). Z.B. 0.3 während eines Angriffs.</summary>
        public float SpeedMultiplier { get; set; } = 1f;

        /// <summary>Wenn true, steuert dieses Skript die Geschwindigkeit nicht (Dash, Cutscene...).</summary>
        public bool MovementLocked { get; set; }

        public Vector2 Velocity => rb.linearVelocity;
        public bool IsMoving => rb.linearVelocity.sqrMagnitude > 0.01f;

        /// <summary>Aktuelle Basisgeschwindigkeit (aus Stats oder Inspector).</summary>
        public float BaseSpeed => stats != null ? stats.GetValue(StatType.MoveSpeed) : moveSpeed;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            stats = GetComponent<CharacterStats>();
            knockback = GetComponent<Knockback>();

            // Typische Top-Down-Einstellungen: keine Schwerkraft, kein Umkippen.
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void FixedUpdate()
        {
            if (MovementLocked) return;
            if (knockback != null && knockback.IsKnockedBack) return;

            Vector2 targetVelocity = input.Move * BaseSpeed * SpeedMultiplier;
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }

        /// <summary>Stoppt sofort jede Bewegung (z.B. beim Öffnen eines Dialogs).</summary>
        public void Stop() => rb.linearVelocity = Vector2.zero;
    }
}
