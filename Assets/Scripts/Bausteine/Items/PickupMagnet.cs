using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Zieht ein Pickup zum Spieler, sobald er in der Nähe ist (Stardew Valley / Diablo 3 Gefühl).
    /// Benötigt ein Objekt mit dem Tag "Player" in der Szene.
    /// </summary>
    public class PickupMagnet : MonoBehaviour
    {
        [SerializeField] private float radius = 1.5f;
        [SerializeField] private float speed = 8f;
        [Tooltip("Kurze Wartezeit nach dem Erscheinen, damit das Aufspringen sichtbar ist.")]
        [SerializeField] private float startDelay = 0.4f;

        private Transform player;
        private Rigidbody2D rb;
        private float timer;

        private void Awake() => rb = GetComponent<Rigidbody2D>();

        private void Start()
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
        }

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (player == null || timer < startDelay) return;

            Vector2 toPlayer = (Vector2)player.position - (Vector2)transform.position;
            if (toPlayer.sqrMagnitude > radius * radius) return;

            Vector2 velocity = toPlayer.normalized * speed;
            if (rb != null) rb.linearVelocity = velocity;
            else transform.position += (Vector3)(velocity * Time.fixedDeltaTime);
        }
    }
}
