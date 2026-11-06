using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Ankunftspunkt in einer Szene. Passt die ID zur SceneDoor, über die der Spieler
    /// gekommen ist, wird er hierhin gesetzt.
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private string spawnId;
        [Tooltip("Blickrichtung nach der Ankunft (z.B. weg von der Tür).")]
        [SerializeField] private Vector2 facingOnArrival = Vector2.down;

        private void Start()
        {
            if (string.IsNullOrEmpty(SceneDoor.PendingSpawnId) || SceneDoor.PendingSpawnId != spawnId) return;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            player.transform.position = transform.position;
            if (player.TryGetComponent(out Rigidbody2D rb)) rb.position = transform.position;
            if (player.TryGetComponent(out FacingDirection facing)) facing.Face(facingOnArrival);

            SceneDoor.ClearPendingSpawn();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)facingOnArrival * 0.5f);
        }
    }
}
