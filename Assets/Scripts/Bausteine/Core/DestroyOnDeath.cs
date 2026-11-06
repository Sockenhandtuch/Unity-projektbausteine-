using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Entfernt das Objekt nach dem Tod (Gegner, Krüge, Büsche...).
    /// Collider werden sofort abgeschaltet, damit die Leiche nicht mehr im Weg ist,
    /// das Objekt selbst bleibt kurz für eine Todesanimation erhalten.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class DestroyOnDeath : MonoBehaviour
    {
        [SerializeField] private float delay = 0.5f;
        [SerializeField] private GameObject deathEffectPrefab;

        private Health health;

        private void Awake() => health = GetComponent<Health>();
        private void OnEnable() => health.Died += OnDied;
        private void OnDisable() => health.Died -= OnDied;

        private void OnDied(DamageInfo info)
        {
            foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
                col.enabled = false;

            if (deathEffectPrefab != null)
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject, delay);
        }
    }
}
