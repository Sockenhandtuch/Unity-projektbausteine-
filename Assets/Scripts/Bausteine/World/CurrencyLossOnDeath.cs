using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Hollow-Knight-Mechanik: Beim Tod bleibt das gesamte Geld am Sterbeort zurück
    /// (als "Schatten"/Seelenmarker). Wer es zurückholt, bekommt alles wieder.
    /// Stirbt man vorher erneut, ist das alte Geld für immer verloren.
    /// </summary>
    [RequireComponent(typeof(Health), typeof(Wallet))]
    public class CurrencyLossOnDeath : MonoBehaviour
    {
        [Tooltip("Prefab mit LostCurrencyMarker + Trigger-Collider2D.")]
        [SerializeField] private LostCurrencyMarker markerPrefab;

        private Health health;
        private Wallet wallet;
        private LostCurrencyMarker activeMarker;

        private void Awake()
        {
            health = GetComponent<Health>();
            wallet = GetComponent<Wallet>();
        }

        private void OnEnable() => health.Died += OnDied;
        private void OnDisable() => health.Died -= OnDied;

        private void OnDied(DamageInfo info)
        {
            // Alter, nicht eingesammelter Marker verfällt.
            if (activeMarker != null) Destroy(activeMarker.gameObject);

            int lost = wallet.TakeAll();
            if (lost <= 0 || markerPrefab == null) return;

            activeMarker = Instantiate(markerPrefab, transform.position, Quaternion.identity);
            activeMarker.Setup(lost);
        }
    }
}
