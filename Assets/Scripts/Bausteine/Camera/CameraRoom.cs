using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Ein Raum mit eigenen Kameragrenzen (Zelda-Dungeon / Hollow-Knight-Areal).
    /// Betritt der Spieler den Trigger, bleibt die Kamera innerhalb dieses Bereichs.
    /// BoxCollider2D als Trigger so groß wie den Raum ziehen.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class CameraRoom : MonoBehaviour
    {
        private BoxCollider2D area;

        private void Awake()
        {
            area = GetComponent<BoxCollider2D>();
            area.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && CameraFollow.Instance != null)
                CameraFollow.Instance.SetBounds(area.bounds);
        }

        private void OnDrawGizmos()
        {
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            if (box == null) return;
            Gizmos.color = new Color(0f, 0.6f, 1f, 0.5f);
            Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
        }
    }
}
