using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Sammlung aller Werte eines Charakters. Basiswerte werden im Inspector eingetragen,
    /// Ausrüstung, Buffs usw. fügen zur Laufzeit Modifikatoren hinzu.
    /// Nicht eingetragene Werte gelten als 0.
    /// </summary>
    public class CharacterStats : MonoBehaviour
    {
        [Serializable]
        private struct BaseStatEntry
        {
            public StatType Type;
            public float Value;
        }

        [SerializeField] private List<BaseStatEntry> baseStats = new List<BaseStatEntry>
        {
            new BaseStatEntry { Type = StatType.MaxHealth,      Value = 5f },
            new BaseStatEntry { Type = StatType.MoveSpeed,      Value = 5f },
            new BaseStatEntry { Type = StatType.AttackSpeed,    Value = 1f },
            new BaseStatEntry { Type = StatType.CritChance,     Value = 0.05f },
            new BaseStatEntry { Type = StatType.CritMultiplier, Value = 1.5f },
        };

        private readonly Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();

        /// <summary>Wird ausgelöst, wenn sich irgendein Wert verändert hat (Parameter = welcher).</summary>
        public event Action<StatType> StatChanged;

        private void Awake()
        {
            foreach (BaseStatEntry entry in baseStats)
                stats[entry.Type] = new Stat(entry.Value);
        }

        /// <summary>Liefert das Stat-Objekt. Fehlende Werte werden mit 0 angelegt.</summary>
        public Stat GetStat(StatType type)
        {
            if (!stats.TryGetValue(type, out Stat stat))
            {
                stat = new Stat(0f);
                stats[type] = stat;
            }
            return stat;
        }

        /// <summary>Der fertige Endwert inklusive aller Boni.</summary>
        public float GetValue(StatType type) => GetStat(type).Value;

        public void AddModifier(StatType type, StatModifier modifier)
        {
            GetStat(type).AddModifier(modifier);
            StatChanged?.Invoke(type);
        }

        /// <summary>Entfernt alle Modifikatoren einer Quelle aus ALLEN Werten.</summary>
        public void RemoveAllModifiersFromSource(object source)
        {
            foreach (KeyValuePair<StatType, Stat> pair in stats)
            {
                if (pair.Value.RemoveAllFromSource(source))
                    StatChanged?.Invoke(pair.Key);
            }
        }

        /// <summary>Erhöht den Basiswert dauerhaft (z.B. durch Stat-Punkte beim Level-Up).</summary>
        public void AddToBase(StatType type, float amount)
        {
            GetStat(type).BaseValue += amount;
            StatChanged?.Invoke(type);
        }
    }
}
