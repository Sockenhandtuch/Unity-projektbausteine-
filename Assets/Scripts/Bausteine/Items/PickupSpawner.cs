using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Zentrale Stelle zum Erzeugen von Beute in der Welt.
    /// Einmal pro Szene anlegen und die Prefabs zuweisen. Truhen, Gegner usw.
    /// müssen dann keine eigenen Prefab-Referenzen mehr pflegen.
    /// </summary>
    public class PickupSpawner : MonoBehaviour
    {
        public static PickupSpawner Instance { get; private set; }

        [SerializeField] private ItemPickup itemPickupPrefab;
        [SerializeField] private CurrencyPickup currencyPickupPrefab;

        [Tooltip("Gold wird in Münzen dieser Größe aufgeteilt (mehrere Münzen sehen besser aus).")]
        [SerializeField, Min(1)] private int coinValue = 5;
        [SerializeField, Min(1)] private int maxCoins = 10;

        [Tooltip("Wie weit Beute beim Erscheinen wegspringt.")]
        [SerializeField] private float scatterForce = 3f;

        private void Awake() => Instance = this;

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void SpawnItem(ItemStack stack, Vector2 position)
        {
            if (stack == null || stack.IsEmpty || itemPickupPrefab == null) return;

            ItemPickup pickup = Instantiate(itemPickupPrefab, position, Quaternion.identity);
            pickup.Setup(stack.Item, stack.Amount);
            Scatter(pickup.gameObject);
        }

        public void SpawnGold(int amount, Vector2 position)
        {
            if (amount <= 0 || currencyPickupPrefab == null) return;

            // Gold auf mehrere Münzen verteilen; die letzte Münze nimmt den Rest.
            int coins = Mathf.Clamp(amount / coinValue, 1, maxCoins);
            int perCoin = amount / coins;
            for (int i = 0; i < coins; i++)
            {
                int value = i == coins - 1 ? amount - perCoin * (coins - 1) : perCoin;
                CurrencyPickup coin = Instantiate(currencyPickupPrefab, position, Quaternion.identity);
                coin.Setup(value);
                Scatter(coin.gameObject);
            }
        }

        // Lässt die Beute in eine zufällige Richtung "aufspringen".
        private void Scatter(GameObject obj)
        {
            if (obj.TryGetComponent(out Rigidbody2D rb))
                rb.linearVelocity = Random.insideUnitCircle.normalized * scatterForce;
        }
    }
}
