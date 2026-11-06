using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Hält Health.Max synchron mit dem Wert "MaxHealth" aus CharacterStats.
    /// So erhöhen Ausrüstung und Level-Ups automatisch das maximale Leben.
    /// </summary>
    [RequireComponent(typeof(CharacterStats), typeof(Health))]
    public class StatsToHealthLink : MonoBehaviour
    {
        private CharacterStats stats;
        private Health health;

        private void Awake()
        {
            stats = GetComponent<CharacterStats>();
            health = GetComponent<Health>();
        }

        private void OnEnable() => stats.StatChanged += OnStatChanged;
        private void OnDisable() => stats.StatChanged -= OnStatChanged;

        private void Start() => Apply(fillUp: true);

        private void OnStatChanged(StatType type)
        {
            if (type == StatType.MaxHealth) Apply(fillUp: false);
        }

        private void Apply(bool fillUp)
        {
            int newMax = Mathf.Max(1, Mathf.RoundToInt(stats.GetValue(StatType.MaxHealth)));
            health.SetMax(newMax, fillUp);
        }
    }
}
