using System.Collections;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Lässt das Sprite bei einem Treffer kurz in einer Farbe aufleuchten (z.B. Weiß/Rot).
    /// Gibt Treffern sofort sichtbares Feedback – wichtig fürs Kampfgefühl.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class DamageFlash : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color flashColor = new Color(1f, 0.3f, 0.3f);
        [SerializeField] private float flashDuration = 0.08f;

        private Health health;
        private Color originalColor;

        private void Awake()
        {
            health = GetComponent<Health>();
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            originalColor = spriteRenderer.color;
        }

        private void OnEnable() => health.Damaged += OnDamaged;

        private void OnDisable()
        {
            health.Damaged -= OnDamaged;
            spriteRenderer.color = originalColor;
        }

        private void OnDamaged(DamageInfo info)
        {
            StopAllCoroutines();
            StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }
}
