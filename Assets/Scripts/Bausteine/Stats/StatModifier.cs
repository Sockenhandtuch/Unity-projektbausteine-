using System;
using UnityEngine;

namespace Bausteine
{
    /// <summary>Wie ein Modifikator auf einen Wert wirkt.</summary>
    public enum ModifierType
    {
        Flat = 100,        // +5 Schaden       → wird zuerst addiert
        PercentAdd = 200,  // +10 %, +20 %     → werden summiert (= +30 %)
        PercentMult = 300  // ×1.1, ×1.2       → werden einzeln multipliziert
    }

    /// <summary>
    /// Eine einzelne Veränderung eines Werts, z.B. "+5 Schaden durch Schwert".
    /// Source merkt sich, woher der Modifikator stammt, damit man beim Ablegen
    /// eines Gegenstands genau dessen Modifikatoren wieder entfernen kann.
    /// </summary>
    public class StatModifier
    {
        public readonly float Value;
        public readonly ModifierType Type;
        public readonly object Source;

        public StatModifier(float value, ModifierType type, object source = null)
        {
            Value = value;
            Type = type;
            Source = source;
        }
    }

    /// <summary>
    /// Im Inspector einstellbare Variante eines Modifikators (z.B. auf Ausrüstung).
    /// Wird zur Laufzeit in einen StatModifier umgewandelt.
    /// </summary>
    [Serializable]
    public struct StatBonus
    {
        public StatType Stat;
        public ModifierType Type;
        [Tooltip("Flat: absoluter Wert. Prozent: 0.1 = 10 %.")]
        public float Value;

        public StatModifier ToModifier(object source) => new StatModifier(Value, Type, source);

        /// <summary>Lesbarer Text für Tooltips, z.B. "+5 Damage" oder "+10% MoveSpeed".</summary>
        public override string ToString()
        {
            string sign = Value >= 0 ? "+" : "";
            return Type == ModifierType.Flat
                ? $"{sign}{Value:0.##} {Stat}"
                : $"{sign}{Value * 100f:0.#}% {Stat}";
        }
    }
}
