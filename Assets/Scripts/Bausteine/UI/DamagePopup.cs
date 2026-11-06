using TMPro;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Eine schwebende Schadenszahl (Diablo). Steigt auf, wird kleiner und blendet aus.
    /// Prefab: Objekt mit TextMeshPro (3D-Text, nicht UGUI) + diesem Skript.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField] private float lifetime = 0.7f;
        [SerializeField] private float riseSpeed = 1.5f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color criticalColor = new Color(1f, 0.85f, 0.2f);
        [SerializeField] private float criticalScale = 1.4f;

        private TMP_Text text;
        private float age;
        private Vector3 startScale;
        private Vector3 drift;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            startScale = transform.localScale;
            drift = new Vector3(Random.Range(-0.5f, 0.5f), riseSpeed, 0f); // leicht seitlich streuen
        }

        public void Setup(int amount, bool isCritical)
        {
            text.text = isCritical ? $"{amount}!" : amount.ToString();
            text.color = isCritical ? criticalColor : normalColor;
            if (isCritical) startScale *= criticalScale;
            transform.localScale = startScale;
        }

        private void Update()
        {
            age += Time.deltaTime;
            float t = age / lifetime;

            transform.position += drift * Time.deltaTime;
            transform.localScale = startScale * Mathf.Lerp(1f, 0.6f, t);

            Color c = text.color;
            c.a = 1f - t;
            text.color = c;

            if (age >= lifetime) Destroy(gameObject);
        }
    }
}
