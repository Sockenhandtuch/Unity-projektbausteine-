using System.Collections;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Lässt den Spieler nach dem Tod am letzten Checkpoint wiederauferstehen.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class PlayerRespawn : MonoBehaviour
    {
        [SerializeField] private float respawnDelay = 2f;

        private Health health;
        private Rigidbody2D rb;
        private Vector2 respawnPoint;

        public Vector2 RespawnPoint => respawnPoint;

        private void Awake()
        {
            health = GetComponent<Health>();
            rb = GetComponent<Rigidbody2D>();
            respawnPoint = transform.position; // Startpunkt als erster Checkpoint
        }

        private void OnEnable() => health.Died += OnDied;
        private void OnDisable() => health.Died -= OnDied;

        public void SetRespawnPoint(Vector2 point) => respawnPoint = point;

        private void OnDied(DamageInfo info) => StartCoroutine(RespawnRoutine());

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);

            if (rb != null)
            {
                rb.position = respawnPoint;
                rb.linearVelocity = Vector2.zero;
            }
            transform.position = respawnPoint;

            health.Revive();
        }
    }
}
