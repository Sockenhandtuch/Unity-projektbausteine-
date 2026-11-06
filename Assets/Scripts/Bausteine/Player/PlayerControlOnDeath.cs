using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Sperrt die Steuerung, solange der Spieler tot ist, und gibt sie bei der Wiederbelebung frei.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class PlayerControlOnDeath : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private PlayerMovement movement;

        private Health health;

        private void Awake() => health = GetComponent<Health>();

        private void OnEnable()
        {
            health.Died += OnDied;
            health.Revived += OnRevived;
        }

        private void OnDisable()
        {
            health.Died -= OnDied;
            health.Revived -= OnRevived;
        }

        private void OnDied(DamageInfo info)
        {
            input.GameplayEnabled = false;
            movement.Stop();
        }

        private void OnRevived() => input.GameplayEnabled = true;
    }
}
