using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Erzeugt bei jedem Treffer eine schwebende Schadenszahl über dem Objekt.
    /// Einfach zusätzlich zu Health auf Gegner (oder Spieler) legen.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class DamagePopupSpawner : MonoBehaviour
    {
        [SerializeField] private DamagePopup popupPrefab;
        [SerializeField] private Vector2 offset = new Vector2(0f, 0.8f);

        private Health health;

        private void Awake() => health = GetComponent<Health>();
        private void OnEnable() => health.Damaged += OnDamaged;
        private void OnDisable() => health.Damaged -= OnDamaged;

        private void OnDamaged(DamageInfo info)
        {
            if (popupPrefab == null) return;
            DamagePopup popup = Instantiate(popupPrefab, (Vector2)transform.position + offset, Quaternion.identity);
            popup.Setup(info.Amount, info.IsCritical);
        }
    }
}
