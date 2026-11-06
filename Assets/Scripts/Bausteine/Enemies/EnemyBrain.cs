using System;
using UnityEngine;

namespace Bausteine
{
    /// <summary>Mögliche Zustände eines Gegners.</summary>
    public enum EnemyState
    {
        Idle,    // Steht herum
        Wander,  // Läuft ziellos in der Nähe seines Startpunkts
        Chase,   // Verfolgt den Spieler
        Attack,  // Greift an (EnemyAttack hat die Kontrolle)
        Return,  // Läuft zurück nach Hause (Spieler zu weit weg)
        Dead
    }

    /// <summary>
    /// Einfache Gegner-KI als Zustandsautomat ("State Machine").
    /// Kombiniert TargetDetector (sehen), EnemyAttack (angreifen) und Bewegung.
    /// Ohne EnemyAttack wird der Gegner zum reinen "Läufer" (gut mit ContactDamage).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(TargetDetector), typeof(Health))]
    public class EnemyBrain : MonoBehaviour
    {
        [Header("Bewegung")]
        [SerializeField] private float moveSpeed = 2.5f;
        [SerializeField] private float chaseSpeedMultiplier = 1.4f;

        [Header("Umherlaufen")]
        [SerializeField] private bool canWander = true;
        [SerializeField] private float wanderRadius = 2f;
        [SerializeField] private Vector2 idleTimeRange = new Vector2(1f, 3f);

        [Header("Leine")]
        [Tooltip("Entfernt sich der Gegner weiter als das von zu Hause, gibt er die Verfolgung auf.")]
        [SerializeField] private float leashDistance = 10f;

        private Rigidbody2D rb;
        private TargetDetector detector;
        private EnemyAttack attack;
        private Health health;
        private Knockback knockback;
        private FacingDirection facing;

        private Vector2 home;
        private Vector2 wanderTarget;
        private float stateTimer;

        public EnemyState State { get; private set; } = EnemyState.Idle;

        /// <summary>(alter Zustand, neuer Zustand) – für Animationen und Sounds.</summary>
        public event Action<EnemyState, EnemyState> StateChanged;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            detector = GetComponent<TargetDetector>();
            attack = GetComponent<EnemyAttack>();
            health = GetComponent<Health>();
            knockback = GetComponent<Knockback>();
            facing = GetComponent<FacingDirection>();

            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        private void OnEnable() => health.Died += OnDied;
        private void OnDisable() => health.Died -= OnDied;

        private void Start()
        {
            home = transform.position;
            EnterIdle();
        }

        // ---------- Entscheidungen (Update) ----------

        private void Update()
        {
            if (State == EnemyState.Dead) return;
            stateTimer -= Time.deltaTime;

            switch (State)
            {
                case EnemyState.Idle:
                    if (detector.HasTarget) ChangeState(EnemyState.Chase);
                    else if (stateTimer <= 0f && canWander) EnterWander();
                    break;

                case EnemyState.Wander:
                    if (detector.HasTarget) ChangeState(EnemyState.Chase);
                    // Ziel erreicht oder festgesteckt (Timer abgelaufen) → kurz verschnaufen.
                    else if (IsAt(wanderTarget) || stateTimer <= 0f) EnterIdle();
                    break;

                case EnemyState.Chase:
                    if (!detector.HasTarget || IsTooFarFromHome()) ChangeState(EnemyState.Return);
                    else if (attack != null && attack.CanAttack && attack.IsInRange(detector.Target))
                    {
                        ChangeState(EnemyState.Attack);
                        attack.Begin(detector.Target);
                    }
                    break;

                case EnemyState.Attack:
                    if (attack == null || !attack.IsAttacking) ChangeState(EnemyState.Chase);
                    break;

                case EnemyState.Return:
                    if (IsAt(home)) EnterIdle();
                    break;
            }
        }

        // ---------- Bewegung (FixedUpdate) ----------

        private void FixedUpdate()
        {
            if (State == EnemyState.Dead) return;
            if (knockback != null && knockback.IsKnockedBack) return;
            if (State == EnemyState.Attack) return; // Der Angriff steuert die Bewegung selbst.

            Vector2 velocity = Vector2.zero;
            switch (State)
            {
                case EnemyState.Wander: velocity = DirectionTo(wanderTarget) * moveSpeed; break;
                case EnemyState.Chase:  velocity = DirectionToTarget() * moveSpeed * chaseSpeedMultiplier; break;
                case EnemyState.Return: velocity = DirectionTo(home) * moveSpeed; break;
            }

            // Im Nahkampf nicht in den Spieler hineinlaufen.
            if (State == EnemyState.Chase && attack != null && attack.IsInRange(detector.Target))
                velocity = Vector2.zero;

            rb.linearVelocity = velocity;
            if (facing != null) facing.Face(velocity);
        }

        // ---------- Zustandswechsel ----------

        private void EnterIdle()
        {
            ChangeState(EnemyState.Idle);
            stateTimer = UnityEngine.Random.Range(idleTimeRange.x, idleTimeRange.y);
        }

        private void EnterWander()
        {
            wanderTarget = home + UnityEngine.Random.insideUnitCircle * wanderRadius;
            ChangeState(EnemyState.Wander);
            stateTimer = 4f; // Sicherheitsgrenze, falls der Gegner an einer Wand hängt
        }

        private void ChangeState(EnemyState newState)
        {
            if (newState == State) return;
            EnemyState old = State;
            State = newState;
            StateChanged?.Invoke(old, newState);
        }

        private void OnDied(DamageInfo info)
        {
            ChangeState(EnemyState.Dead);
            rb.linearVelocity = Vector2.zero;
            if (attack != null) attack.enabled = false;
        }

        // ---------- Hilfsfunktionen ----------

        private bool IsAt(Vector2 point) => Vector2.Distance(rb.position, point) < 0.15f;
        private bool IsTooFarFromHome() => Vector2.Distance(rb.position, home) > leashDistance;
        private Vector2 DirectionTo(Vector2 point) => (point - rb.position).normalized;
        private Vector2 DirectionToTarget() => detector.HasTarget ? DirectionTo(detector.Target.position) : Vector2.zero;

        /// <summary>Setzt den "Heimatpunkt" neu (z.B. beim Spawnen durch einen Spawner).</summary>
        public void SetHome(Vector2 position) => home = position;
    }
}
