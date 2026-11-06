using System.Collections.Generic;

namespace Bausteine
{
    /// <summary>
    /// Ein einzelner Wert (z.B. Schaden) mit Basiswert und beliebig vielen Modifikatoren.
    /// Der Endwert wird nur neu berechnet, wenn sich etwas geändert hat ("Dirty Flag").
    ///
    /// Rechenreihenfolge:
    ///   (Basis + alle Flat) × (1 + Summe PercentAdd) × (1 + PercentMult₁) × (1 + PercentMult₂) ...
    /// </summary>
    public class Stat
    {
        private readonly List<StatModifier> modifiers = new List<StatModifier>();
        private float baseValue;
        private float cachedValue;
        private bool isDirty = true;

        public Stat(float baseValue)
        {
            this.baseValue = baseValue;
        }

        /// <summary>Der Grundwert ohne Modifikatoren (z.B. durch Stat-Punkte erhöhbar).</summary>
        public float BaseValue
        {
            get => baseValue;
            set { baseValue = value; isDirty = true; }
        }

        /// <summary>Der Endwert inklusive aller Modifikatoren.</summary>
        public float Value
        {
            get
            {
                if (isDirty)
                {
                    cachedValue = Calculate();
                    isDirty = false;
                }
                return cachedValue;
            }
        }

        public IReadOnlyList<StatModifier> Modifiers => modifiers;

        public void AddModifier(StatModifier modifier)
        {
            modifiers.Add(modifier);
            isDirty = true;
        }

        public bool RemoveModifier(StatModifier modifier)
        {
            bool removed = modifiers.Remove(modifier);
            if (removed) isDirty = true;
            return removed;
        }

        /// <summary>Entfernt alle Modifikatoren einer Quelle (z.B. eines abgelegten Rings).</summary>
        /// <returns>true, wenn mindestens einer entfernt wurde.</returns>
        public bool RemoveAllFromSource(object source)
        {
            int removed = modifiers.RemoveAll(m => Equals(m.Source, source));
            if (removed > 0) isDirty = true;
            return removed > 0;
        }

        private float Calculate()
        {
            float flat = baseValue;
            float percentAdd = 0f;
            float multiplier = 1f;

            foreach (StatModifier mod in modifiers)
            {
                switch (mod.Type)
                {
                    case ModifierType.Flat:        flat += mod.Value; break;
                    case ModifierType.PercentAdd:  percentAdd += mod.Value; break;
                    case ModifierType.PercentMult: multiplier *= 1f + mod.Value; break;
                }
            }

            return flat * (1f + percentAdd) * multiplier;
        }
    }
}
