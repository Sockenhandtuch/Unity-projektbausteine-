using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Der zurückgelassene Geldhaufen bzw. "Schatten" am Sterbeort.
    /// Gibt beim Berühren das verlorene Geld zurück.
    /// Wer mag, ersetzt das durch einen Schatten-Gegner, der zuerst besiegt werden muss
    /// (dann einfach Health + LootDropper statt dieses Skripts).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class LostCurrencyMarker : MonoBehaviour
    {
        private int amount;

        private void Awake() => GetComponent<Collider2D>().isTrigger = true;

        public void Setup(int lostAmount) => amount = lostAmount;

        private void OnTriggerEnter2D(Collider2D other)
        {
            Wallet wallet = other.GetComponentInParent<Wallet>();
            Health health = other.GetComponentInParent<Health>();
            // Nur ein lebender Spieler kann sein Geld zurückholen.
            if (wallet == null || (health != null && !health.IsAlive)) return;

            wallet.Add(amount);
            Destroy(gameObject);
        }
    }
}
