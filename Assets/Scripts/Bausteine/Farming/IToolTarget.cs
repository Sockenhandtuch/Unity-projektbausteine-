using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Alles, was mit einem gehaltenen Gegenstand benutzt werden kann:
    /// Felder (Hacke, Gießkanne, Saatgut), Bäume (Axt), Steine (Spitzhacke)...
    /// </summary>
    public interface IToolTarget
    {
        /// <summary>
        /// Versucht, den Gegenstand hier anzuwenden.
        /// </summary>
        /// <param name="item">Der Gegenstand in der Hand (Werkzeug, Saatgut...).</param>
        /// <param name="user">Der Benutzer (meist der Spieler) – für Inventar-Zugriff.</param>
        /// <returns>true, wenn etwas passiert ist (dann wird z.B. Ausdauer abgezogen).</returns>
        bool UseItem(ItemData item, GameObject user);
    }
}
