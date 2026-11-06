using System.Collections.Generic;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Liste aller Gegenstände im Spiel. Wird benötigt, um gespeicherte IDs
    /// beim Laden wieder in ItemData-Assets zurückzuverwandeln.
    /// </summary>
    [CreateAssetMenu(menuName = "Bausteine/Items/Item Database", fileName = "ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<ItemData> items = new List<ItemData>();

        private Dictionary<string, ItemData> lookup;

        /// <summary>Sucht einen Gegenstand anhand seiner ID. Gibt null zurück, wenn unbekannt.</summary>
        public ItemData GetById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            // Nachschlagetabelle beim ersten Zugriff aufbauen.
            if (lookup == null)
            {
                lookup = new Dictionary<string, ItemData>();
                foreach (ItemData item in items)
                    if (item != null) lookup[item.Id] = item;
            }

            lookup.TryGetValue(id, out ItemData result);
            return result;
        }
    }
}
