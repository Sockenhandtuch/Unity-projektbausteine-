using System;
using System.Collections;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Nahkampfangriff (Schwert / Nagel) in Blickrichtung.
    ///
    /// Aufbau im Prefab:
    ///   Player
    ///    └─ AttackPivot        (leeres Objekt in der Mitte des Spielers, wird gedreht)
    ///        └─ Hitbox         (rechts vom Pivot, z.B. x = 0.6; Collider2D als Trigger + Hitbox)
    ///
    /// Ablauf eines Schlags: Ausholen (windup) → Hitbox aktiv (active) → Erholung (recovery).
    /// </summary>
    public class PlayerMeleeAttack : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private FacingDirection facing;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private Transform attackPivot;
        [SerializeField] private Hitbox hitbox;

        [Header("Schaden")]
        [SerializeField] private int baseDamage = 1;

        [Header("Timing (Sekunden, bei AttackSpeed 1)")]
        [SerializeField] private float windup = 0.05f;
        [SerializeField] private float activeTime = 0.1f;
        [SerializeField] private float recovery = 0.15f;

        [Header("Bewegung")]
        [Tooltip("Laufgeschwindigkeit während des Angriffs (0 = stehen bleiben, 1 = normal).")]
        [SerializeField, Range(0f, 1f)] private float moveSpeedWhileAttacking = 0.3f;

        [Header("Optional")]
        [SerializeField] private PlayerDash dash;

        private CharacterStats stats;

        public bool IsAttacking { get; private set; }

        /// <summary>Wird beim Start jedes Schlags ausgelöst (Animation, Sound).</summary>
        public event Action AttackStarted;

        private void Awake()
        {
            stats = GetComponent<CharacterStats>();
            hitbox.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (input.AttackPressed && CanAttack()) StartCoroutine(AttackRoutine());
        }

        private bool CanAttack()
        {
            if (IsAttacking) return false;
            if (dash != null && dash.IsDashing) return false;
            return true;
        }

        private IEnumerator AttackRoutine()
        {
            IsAttacking = true;
            facing.Locked = true;
            movement.SpeedMultiplier = moveSpeedWhileAttacking;

            AimPivot(facing.Cardinal);
            hitbox.Damage = DamageCalculator.CalculateOutgoing(baseDamage, stats, out bool isCritical);
            hitbox.IsCritical = isCritical;
            AttackStarted?.Invoke();

            // AttackSpeed 2 → doppelt so schneller Schlag.
            float speed = stats != null ? Mathf.Max(0.1f, stats.GetValue(StatType.AttackSpeed)) : 1f;

            yield return new WaitForSeconds(windup / speed);
            hitbox.gameObject.SetActive(true);

            yield return new WaitForSeconds(activeTime / speed);
            hitbox.gameObject.SetActive(false);

            yield return new WaitForSeconds(recovery / speed);
            EndAttack();
        }

        // Dreht den Pivot so, dass die Hitbox (die rechts liegt) in Blickrichtung zeigt.
        private void AimPivot(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            attackPivot.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void EndAttack()
        {
            IsAttacking = false;
            facing.Locked = false;
            movement.SpeedMultiplier = 1f;
            hitbox.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (IsAttacking) EndAttack();
        }
    }
}
