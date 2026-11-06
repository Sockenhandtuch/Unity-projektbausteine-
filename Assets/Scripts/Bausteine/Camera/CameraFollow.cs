using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Kamera folgt dem Spieler weich und bleibt innerhalb der Raumgrenzen.
    ///
    /// Aufbau (wichtig für CameraShake):
    ///   CameraRig   ← CameraFollow (bewegt sich)
    ///    └─ Main Camera ← CameraShake (wackelt lokal)
    ///
    /// Für Pixel-Perfect: Unitys "Pixel Perfect Camera"-Komponente auf die Kamera legen
    /// und pixelsPerUnit hier passend einstellen.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        public static CameraFollow Instance { get; private set; }

        [SerializeField] private Transform target;
        [Tooltip("0 = sofort, höher = weicher/träger.")]
        [SerializeField] private float smoothTime = 0.12f;
        [SerializeField] private Vector2 offset = Vector2.zero;

        [Header("Pixelart")]
        [Tooltip("Auf das Pixelraster einrasten, um Zittern zu vermeiden. 0 = aus.")]
        [SerializeField] private int pixelsPerUnit = 16;

        [Header("Grenzen")]
        [SerializeField] private Camera cam;

        private Vector3 velocity;
        private Bounds? bounds;

        private void Awake()
        {
            Instance = this;
            if (cam == null) cam = GetComponentInChildren<Camera>();
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) target = player.transform;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desired = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
            desired = ClampToBounds(desired);

            Vector3 position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
            transform.position = SnapToPixelGrid(position);
        }

        /// <summary>Setzt die Grenzen des aktuellen Raums (wird von CameraRoom aufgerufen).</summary>
        public void SetBounds(Bounds newBounds) => bounds = newBounds;

        /// <summary>Entfernt alle Grenzen (offene Oberwelt).</summary>
        public void ClearBounds() => bounds = null;

        /// <summary>Springt sofort zum Ziel (z.B. nach Teleport/Szenenwechsel).</summary>
        public void SnapToTarget()
        {
            if (target == null) return;
            velocity = Vector3.zero;
            Vector3 p = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
            transform.position = SnapToPixelGrid(ClampToBounds(p));
        }

        // Hält die Kamera so, dass ihr Sichtfeld nicht über den Raum hinausragt.
        private Vector3 ClampToBounds(Vector3 position)
        {
            if (!bounds.HasValue || cam == null) return position;

            Bounds b = bounds.Value;
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;

            // Ist der Raum kleiner als das Sichtfeld, wird zentriert.
            position.x = b.size.x <= halfWidth * 2f ? b.center.x : Mathf.Clamp(position.x, b.min.x + halfWidth, b.max.x - halfWidth);
            position.y = b.size.y <= halfHeight * 2f ? b.center.y : Mathf.Clamp(position.y, b.min.y + halfHeight, b.max.y - halfHeight);
            return position;
        }

        private Vector3 SnapToPixelGrid(Vector3 position)
        {
            if (pixelsPerUnit <= 0) return position;
            position.x = Mathf.Round(position.x * pixelsPerUnit) / pixelsPerUnit;
            position.y = Mathf.Round(position.y * pixelsPerUnit) / pixelsPerUnit;
            return position;
        }
    }
}
