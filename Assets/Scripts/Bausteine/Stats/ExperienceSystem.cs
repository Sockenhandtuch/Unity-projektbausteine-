using System;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Erfahrung und Stufenaufstieg (Diablo-Stil).
    /// Pro Stufe gibt es Stat-Punkte, die der Spieler frei verteilen kann.
    /// </summary>
    [RequireComponent(typeof(CharacterStats))]
    public class ExperienceSystem : MonoBehaviour
    {
        [Header("Erfahrungskurve")]
        [Tooltip("Benötigte EP für Stufe 1 → 2.")]
        [SerializeField] private int baseXpRequired = 100;
        [Tooltip("Wie stark die Anforderung pro Stufe steigt (1 = linear, 1.5 = deutlich steiler).")]
        [SerializeField] private float growthExponent = 1.5f;
        [SerializeField] private int maxLevel = 50;

        [Header("Belohnungen pro Stufe")]
        [SerializeField] private int statPointsPerLevel = 3;
        [SerializeField] private bool healOnLevelUp = true;

        [Header("Wert pro investiertem Stat-Punkt")]
        [SerializeField] private float healthPerPoint = 1f;
        [SerializeField] private float damagePerPoint = 0.5f;
        [SerializeField] private float armorPerPoint = 2f;

        private CharacterStats stats;

        public int Level { get; private set; } = 1;
        public int CurrentXp { get; private set; }
        public int UnspentStatPoints { get; private set; }
        public int XpToNextLevel => GetXpRequired(Level);
        public bool IsMaxLevel => Level >= maxLevel;

        /// <summary>(aktuelle EP, benötigte EP) – z.B. für eine EP-Leiste.</summary>
        public event Action<int, int> XpChanged;
        /// <summary>Neue Stufe.</summary>
        public event Action<int> LeveledUp;
        /// <summary>Freie Stat-Punkte haben sich geändert.</summary>
        public event Action<int> StatPointsChanged;

        private void Awake() => stats = GetComponent<CharacterStats>();

        /// <summary>Benötigte EP, um von "level" auf "level + 1" zu steigen.</summary>
        public int GetXpRequired(int level)
        {
            return Mathf.RoundToInt(baseXpRequired * Mathf.Pow(level, growthExponent));
        }

        /// <summary>Fügt Erfahrung hinzu. Mehrere Stufenaufstiege auf einmal sind möglich.</summary>
        public void AddXp(int amount)
        {
            if (amount <= 0 || IsMaxLevel) return;

            CurrentXp += amount;
            while (!IsMaxLevel && CurrentXp >= XpToNextLevel)
            {
                CurrentXp -= XpToNextLevel;
                LevelUp();
            }
            if (IsMaxLevel) CurrentXp = 0;

            XpChanged?.Invoke(CurrentXp, XpToNextLevel);
        }

        private void LevelUp()
        {
            Level++;
            UnspentStatPoints += statPointsPerLevel;
            LeveledUp?.Invoke(Level);
            StatPointsChanged?.Invoke(UnspentStatPoints);

            if (healOnLevelUp && TryGetComponent(out Health health))
                health.Heal(health.Max);
        }

        /// <summary>Investiert einen Stat-Punkt. Erlaubt: MaxHealth, Damage, Armor.</summary>
        /// <returns>false, wenn keine Punkte frei sind oder der Wert nicht steigerbar ist.</returns>
        public bool SpendStatPoint(StatType type)
        {
            if (UnspentStatPoints <= 0) return false;

            float amount;
            switch (type)
            {
                case StatType.MaxHealth: amount = healthPerPoint; break;
                case StatType.Damage:    amount = damagePerPoint; break;
                case StatType.Armor:     amount = armorPerPoint;  break;
                default: return false;
            }

            stats.AddToBase(type, amount);
            UnspentStatPoints--;
            StatPointsChanged?.Invoke(UnspentStatPoints);
            return true;
        }

        /// <summary>Stellt einen gespeicherten Zustand wieder her (Spielstand laden).</summary>
        public void Restore(int level, int xp, int unspentPoints)
        {
            Level = Mathf.Clamp(level, 1, maxLevel);
            CurrentXp = Mathf.Max(0, xp);
            UnspentStatPoints = Mathf.Max(0, unspentPoints);
            XpChanged?.Invoke(CurrentXp, XpToNextLevel);
            StatPointsChanged?.Invoke(UnspentStatPoints);
        }
    }
}
