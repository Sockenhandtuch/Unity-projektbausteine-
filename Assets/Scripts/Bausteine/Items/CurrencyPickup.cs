using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Eine Münze/Geldbeutel in der Welt. Landet beim Berühren in der Wallet.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CurrencyPickup : MonoBehaviour
    {
        [SerializeField, Min(1)] private int value = 1;
        [SerializeField] private float pickupDelay = 0.3f;

        private float spawnTime;

        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
            spawnTime = Time.time;
        }

        public void Setup(int newValue) => value = newValue;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (Time.time - spawnTime < pickupDelay) return;

            Wallet wallet = other.GetComponentInParent<Wallet>();
            if (wallet == null) return;

            wallet.Add(value);
            Destroy(gameObject);
        }
    }
}
