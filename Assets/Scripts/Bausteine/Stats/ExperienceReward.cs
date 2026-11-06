using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Gibt dem Töter dieses Objekts Erfahrungspunkte.
    /// Auf Gegner legen. Der Töter wird über DamageInfo.Source ermittelt.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class ExperienceReward : MonoBehaviour
    {
        [SerializeField] private int xp = 25;

        private Health health;

        private void Awake() => health = GetComponent<Health>();
        private void OnEnable() => health.Died += OnDied;
        private void OnDisable() => health.Died -= OnDied;

        private void OnDied(DamageInfo info)
        {
            if (info.Source == null) return;

            ExperienceSystem killer = info.Source.GetComponentInParent<ExperienceSystem>();
            if (killer != null) killer.AddXp(xp);
        }
    }
}
