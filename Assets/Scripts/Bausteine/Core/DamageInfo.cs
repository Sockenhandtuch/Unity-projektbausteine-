using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Bündelt alle Informationen zu einem einzelnen Treffer.
    /// Wird von Angreifern (Hitbox, Projektil, ContactDamage) erzeugt
    /// und an das getroffene Objekt (IDamageable) übergeben.
    /// </summary>
    public struct DamageInfo
    {
        /// <summary>Wie viel Schaden der Treffer verursacht.</summary>
        public int Amount;

        /// <summary>Weltposition, von der der Treffer ausging (für Rückstoß-Richtung).</summary>
        public Vector2 SourcePosition;

        /// <summary>Stärke des Rückstoßes. 0 = kein Rückstoß.</summary>
        public float KnockbackForce;

        /// <summary>Das Objekt, das den Schaden verursacht hat (z.B. der Spieler). Kann null sein.</summary>
        public GameObject Source;

        /// <summary>War der Treffer ein kritischer Treffer? (Für Schadenszahlen, Effekte...)</summary>
        public bool IsCritical;

        public DamageInfo(int amount, Vector2 sourcePosition, float knockbackForce = 0f,
                          GameObject source = null, bool isCritical = false)
        {
            Amount = amount;
            SourcePosition = sourcePosition;
            KnockbackForce = knockbackForce;
            Source = source;
            IsCritical = isCritical;
        }

        /// <summary>
        /// Liefert die normalisierte Richtung von der Schadensquelle zum Ziel.
        /// Praktisch, um Rückstoß in die richtige Richtung zu schieben.
        /// </summary>
        public Vector2 DirectionTo(Vector2 targetPosition)
        {
            return (targetPosition - SourcePosition).normalized;
        }
    }
}
