using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Basisdaten eines Gegenstands als ScriptableObject (Rechtsklick → Create → Bausteine → Items).
    /// Ein Asset beschreibt die ART des Gegenstands, nicht ein einzelnes Exemplar.
    /// Spezialisierte Gegenstände (Ausrüstung, Saatgut, Werkzeug...) erben hiervon.
    /// </summary>
    [CreateAssetMenu(menuName = "Bausteine/Items/Item", fileName = "NewItem")]
    public class ItemData : ScriptableObject
    {
        [Tooltip("Eindeutige ID – wird zum Speichern benutzt. Nie nachträglich ändern!")]
        [SerializeField] private string id;
        [SerializeField] private string displayName = "Neuer Gegenstand";
        [SerializeField, TextArea] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private ItemRarity rarity = ItemRarity.Common;
        [SerializeField, Min(1)] private int maxStack = 1;
        [SerializeField, Min(0)] private int sellPrice = 1;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public ItemRarity Rarity => rarity;
        public int MaxStack => maxStack;
        public int SellPrice => sellPrice;
        public bool IsStackable => maxStack > 1;

        /// <summary>
        /// Text für Tooltips. Unterklassen ergänzen ihn (z.B. Ausrüstung um ihre Boni).
        /// </summary>
        public virtual string GetTooltip() => description;

#if UNITY_EDITOR
        // Vergibt automatisch eine ID, falls keine eingetragen ist.
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id)) id = name;
        }
#endif
    }
}
