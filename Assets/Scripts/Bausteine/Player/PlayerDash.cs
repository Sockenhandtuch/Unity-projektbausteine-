using System;
using System.Collections;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Kurzer, schneller Sprint/Ausweichrolle mit optionaler Unverwundbarkeit
    /// (Hollow Knight Dash / Zelda Rolle). Kann über PlayerAbilities freigeschaltet werden.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerDash : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private FacingDirection facing;

        [Header("Werte")]
        [SerializeField] private float dashSpeed = 14f;
        [SerializeField] private float dashDuration = 0.15f;
        [SerializeField] private float cooldown = 0.4f;
        [SerializeField] private bool invulnerableWhileDashing = true;

        [Header("Optional")]
        [Tooltip("Wenn gesetzt, muss die Fähigkeit 'Dash' erst freigeschaltet werden.")]
        [SerializeField] private PlayerAbilities abilities;
        [Tooltip("Z.B. Staubwolke oder Nachbild beim Dash.")]
        [SerializeField] private GameObject dashEffectPrefab;

        private Rigidbody2D rb;
        private Health health;
        private float cooldownTimer;

        public bool IsDashing { get; private set; }

        public event Action DashStarted;
        public event Action DashEnded;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
        }

        private void Update()
        {
            cooldownTimer -= Time.deltaTime;
            if (input.DashPressed && CanDash()) StartCoroutine(DashRoutine());
        }

        public bool CanDash()
        {
            if (IsDashing || cooldownTimer > 0f) return false;
            if (abilities != null && !abilities.Has(AbilityType.Dash)) return false;
            return health == null || health.IsAlive;
        }

        private IEnumerator DashRoutine()
        {
            // Richtung: Eingabe hat Vorrang, sonst Blickrichtung.
            Vector2 direction = input.Move.sqrMagnitude > 0.01f ? input.Move.normalized : facing.Direction;

            BeginDash();
            if (dashEffectPrefab != null) Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);

            // Geschwindigkeit in jedem Physik-Schritt setzen, damit nichts den Dash ausbremst.
            float elapsed = 0f;
            while (elapsed < dashDuration)
            {
                rb.linearVelocity = direction * dashSpeed;
                elapsed += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            EndDash();
        }

        private void BeginDash()
        {
            IsDashing = true;
            movement.MovementLocked = true;
            if (invulnerableWhileDashing && health != null) health.AddInvulnerability();
            DashStarted?.Invoke();
        }

        private void EndDash()
        {
            if (!IsDashing) return;

            IsDashing = false;
            movement.MovementLocked = false;
            if (invulnerableWhileDashing && health != null) health.RemoveInvulnerability();
            cooldownTimer = cooldown;
            DashEnded?.Invoke();
        }

        // Wird das Objekt mitten im Dash deaktiviert, stoppen Coroutinen automatisch → aufräumen.
        private void OnDisable() => EndDash();
    }
}
