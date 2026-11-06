using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Zentrale Schadensformel. An einer Stelle gebündelt, damit Balancing leicht fällt.
    /// Nutzt CharacterStats (Diablo-artige Werte), funktioniert aber auch ohne.
    /// </summary>
    public static class DamageCalculator
    {
        private const float DefaultCritMultiplier = 1.5f;

        /// <summary>
        /// Berechnet den ausgehenden Schaden eines Angriffs.
        /// Schaden = (Basisschaden + Stat "Damage") × kritischer Multiplikator (bei Krit).
        /// </summary>
        public static int CalculateOutgoing(int baseDamage, CharacterStats attacker, out bool isCritical)
        {
            isCritical = false;
            if (attacker == null) return baseDamage;

            float damage = baseDamage + attacker.GetValue(StatType.Damage);

            isCritical = Random.value < attacker.GetValue(StatType.CritChance);
            if (isCritical)
            {
                float multiplier = attacker.GetValue(StatType.CritMultiplier);
                damage *= multiplier > 0f ? multiplier : DefaultCritMultiplier;
            }

            return Mathf.Max(1, Mathf.RoundToInt(damage));
        }

        /// <summary>
        /// Verringert eingehenden Schaden durch Rüstung.
        /// Formel: Schaden × 100 / (100 + Rüstung) → abnehmender Nutzen, nie 0.
        /// </summary>
        public static int ApplyArmor(int incomingDamage, CharacterStats defender)
        {
            if (defender == null) return incomingDamage;

            float armor = Mathf.Max(0f, defender.GetValue(StatType.Armor));
            float reduced = incomingDamage * 100f / (100f + armor);
            return Mathf.Max(1, Mathf.RoundToInt(reduced));
        }
    }
}
