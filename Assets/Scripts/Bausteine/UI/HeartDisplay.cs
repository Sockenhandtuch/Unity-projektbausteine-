using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bausteine
{
    /// <summary>
    /// Zeigt Leben als Herzen/Masken an (Zelda / Hollow Knight).
    /// Ein Herz = 1 Lebenspunkt. Herzen werden automatisch erzeugt, wenn das Maximum steigt.
    /// Aufbau: Leeres UI-Objekt mit HorizontalLayoutGroup + diesem Skript.
    /// </summary>
    public class HeartDisplay : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Image heartPrefab;
        [SerializeField] private Sprite fullHeart;
        [SerializeField] private Sprite emptyHeart;

        private readonly List<Image> hearts = new List<Image>();

        private void Start()
        {
            if (health == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) health = player.GetComponent<Health>();
            }
            if (health == null) return;

            health.Changed += Redraw;
            Redraw(health.Current, health.Max);
        }

        private void OnDestroy()
        {
            if (health != null) health.Changed -= Redraw;
        }

        private void Redraw(int current, int max)
        {
            // Fehlende Herzen erzeugen, überzählige ausblenden.
            while (hearts.Count < max) hearts.Add(Instantiate(heartPrefab, transform));

            for (int i = 0; i < hearts.Count; i++)
            {
                hearts[i].gameObject.SetActive(i < max);
                hearts[i].sprite = i < current ? fullHeart : emptyHeart;
            }
        }
    }
}
