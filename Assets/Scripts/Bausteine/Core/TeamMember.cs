using UnityEngine;

namespace Bausteine
{
    /// <summary>Zu welcher Seite ein Objekt gehört.</summary>
    public enum Faction
    {
        Player,   // Spieler und Verbündete
        Enemy,    // Gegner
        Neutral   // Krüge, Büsche, Kisten – können von allen getroffen werden
    }

    /// <summary>
    /// Markiert ein Objekt als Mitglied einer Fraktion.
    /// Verhindert z.B., dass Gegner sich gegenseitig treffen.
    /// Gehört auf das Wurzel-Objekt (Root) eines Charakters.
    /// </summary>
    public class TeamMember : MonoBehaviour
    {
        [SerializeField] private Faction faction = Faction.Neutral;

        public Faction Faction => faction;

        /// <summary>
        /// Entscheidet, ob Angreifer a das Ziel b treffen darf.
        /// Regeln: Ohne TeamMember oder bei Neutral → immer treffbar.
        /// Sonst nur, wenn die Fraktionen unterschiedlich sind.
        /// </summary>
        public static bool AreHostile(TeamMember a, TeamMember b)
        {
            if (a == null || b == null) return true;
            if (a.faction == Faction.Neutral || b.faction == Faction.Neutral) return true;
            return a.faction != b.faction;
        }
    }
}
