namespace Bausteine
{
    /// <summary>
    /// Alles, was Schaden nehmen kann (Spieler, Gegner, Krüge, Kisten...),
    /// implementiert dieses Interface. Angreifer kennen dadurch nur das Interface
    /// und nicht die konkrete Klasse.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>Lebt das Objekt noch? Tote Objekte werden von Angriffen ignoriert.</summary>
        bool IsAlive { get; }

        /// <summary>Wendet einen Treffer auf dieses Objekt an.</summary>
        void TakeDamage(DamageInfo info);
    }
}
