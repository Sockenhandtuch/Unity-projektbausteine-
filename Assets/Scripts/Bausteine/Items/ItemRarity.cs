using UnityEngine;

namespace Bausteine
{
    /// <summary>Seltenheitsstufen nach Diablo-Vorbild.</summary>
    public enum ItemRarity
    {
        Common,   // Weiß
        Magic,    // Blau
        Rare,     // Gelb
        Set,      // Grün
        Unique    // Gold/Orange
    }

    /// <summary>Hilfsfunktionen rund um Seltenheit.</summary>
    public static class ItemRarityExtensions
    {
        /// <summary>Typische Textfarbe für Namen, Tooltips und Beute-Labels.</summary>
        public static Color ToColor(this ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Magic:  return new Color(0.41f, 0.41f, 1f);
                case ItemRarity.Rare:   return new Color(1f, 1f, 0.39f);
                case ItemRarity.Set:    return new Color(0f, 1f, 0f);
                case ItemRarity.Unique: return new Color(0.78f, 0.6f, 0.36f);
                default:                return Color.white;
            }
        }
    }
}
