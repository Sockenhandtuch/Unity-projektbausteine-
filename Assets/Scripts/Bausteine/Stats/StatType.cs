namespace Bausteine
{
    /// <summary>
    /// Alle Werte, die ein Charakter haben kann (Diablo-Stil).
    /// Neue Werte einfach hier ergänzen – das restliche System passt sich automatisch an.
    /// </summary>
    public enum StatType
    {
        MaxHealth,       // Maximale Lebenspunkte
        Damage,          // Zusätzlicher Schaden pro Treffer
        Armor,           // Verringert eingehenden Schaden
        MoveSpeed,       // Laufgeschwindigkeit (Einheiten/Sekunde)
        AttackSpeed,     // Multiplikator für Angriffstempo (1 = normal)
        CritChance,      // 0..1 → Wahrscheinlichkeit für kritische Treffer
        CritMultiplier,  // Schadensfaktor bei Krit (z.B. 1.5)
        MagicFind,       // Erhöht Chance auf seltene Beute (0.5 = +50 %)
        GoldFind         // Erhöht gefundenes Gold (0.5 = +50 %)
    }
}
