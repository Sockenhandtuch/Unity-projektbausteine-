using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Ein Gegenstand, der in der Welt liegt und beim Berühren ins Inventar wandert.
    /// Passt nicht alles hinein, bleibt der Rest liegen.
    /// Prefab-Aufbau: SpriteRenderer + Trigger-Collider2D (+ optional Rigidbody2D, PickupMagnet).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private ItemData item;
        [SerializeField, Min(1)] private int amount = 1;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Tooltip("Kurze Sperre nach dem Erscheinen, damit Beute nicht sofort eingesammelt wird.")]
        [SerializeField] private float pickupDelay = 0.3f;

        private float spawnTime;

        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spawnTime = Time.time;
            Refresh();
        }

        /// <summary>Wird vom PickupSpawner nach dem Erzeugen aufgerufen.</summary>
        public void Setup(ItemData newItem, int newAmount)
        {
            item = newItem;
            amount = newAmount;
            Refresh();
        }

        private void Refresh()
        {
            if (spriteRenderer != null && item != null) spriteRenderer.sprite = item.Icon;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (Time.time - spawnTime < pickupDelay || item == null) return;

            Inventory inventory = other.GetComponentInParent<Inventory>();
            if (inventory == null) return;

            amount = inventory.Add(item, amount); // Rückgabe = was nicht gepasst hat
            if (amount <= 0) Destroy(gameObject);
        }
    }
}
