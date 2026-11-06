using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Übersetzt den Spielzustand in Animator-Parameter. Keine Spiellogik, nur Darstellung.
    ///
    /// Benötigte Parameter im Animator Controller:
    ///   Float   "MoveX", "MoveY"  → Blickrichtung (für 4-Wege Blend Trees)
    ///   Float   "Speed"           → 0 = Idle, > 0 = Laufen
    ///   Bool    "IsDashing"
    ///   Trigger "Attack", "Hurt", "Die"
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimatorBridge : MonoBehaviour
    {
        // Hashes sind schneller als Strings und verhindern Tippfehler zur Laufzeit.
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsDashing = Animator.StringToHash("IsDashing");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Hurt = Animator.StringToHash("Hurt");
        private static readonly int Die = Animator.StringToHash("Die");

        [SerializeField] private FacingDirection facing;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerDash dash;
        [SerializeField] private PlayerMeleeAttack attack;
        [SerializeField] private Health health;

        private Animator animator;

        private void Awake() => animator = GetComponent<Animator>();

        private void OnEnable()
        {
            if (attack != null) attack.AttackStarted += OnAttack;
            if (health != null)
            {
                health.Damaged += OnDamaged;
                health.Died += OnDied;
            }
        }

        private void OnDisable()
        {
            if (attack != null) attack.AttackStarted -= OnAttack;
            if (health != null)
            {
                health.Damaged -= OnDamaged;
                health.Died -= OnDied;
            }
        }

        private void Update()
        {
            Vector2 dir = facing.Cardinal;
            animator.SetFloat(MoveX, dir.x);
            animator.SetFloat(MoveY, dir.y);
            animator.SetFloat(Speed, movement.Velocity.magnitude);
            if (dash != null) animator.SetBool(IsDashing, dash.IsDashing);
        }

        private void OnAttack() => animator.SetTrigger(Attack);

        private void OnDamaged(DamageInfo info)
        {
            if (health.IsAlive) animator.SetTrigger(Hurt);
        }

        private void OnDied(DamageInfo info) => animator.SetTrigger(Die);
    }
}
