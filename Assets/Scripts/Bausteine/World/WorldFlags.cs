using System.Collections.Generic;

namespace Bausteine
{
    /// <summary>
    /// Globales Gedächtnis der Welt: "Truhe_07 geöffnet", "Boss_1 besiegt", "Brücke repariert".
    /// Einfache Schlüssel-Wert-Paare, die mit dem Spielstand gespeichert werden.
    /// </summary>
    public static class WorldFlags
    {
        private static readonly HashSet<string> flags = new HashSet<string>();

        public static bool Get(string id) => flags.Contains(id);

        public static void Set(string id, bool value)
        {
            if (value) flags.Add(id);
            else flags.Remove(id);
        }

        /// <summary>Alle gesetzten Flags (für den Spielstand).</summary>
        public static List<string> GetAll() => new List<string>(flags);

        /// <summary>Ersetzt alle Flags (beim Laden).</summary>
        public static void LoadAll(IEnumerable<string> ids)
        {
            flags.Clear();
            if (ids == null) return;
            foreach (string id in ids) flags.Add(id);
        }
    }
}
