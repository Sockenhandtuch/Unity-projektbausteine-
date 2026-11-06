using System;

namespace Bausteine
{
    /// <summary>
    /// Ein Stapel gleicher Gegenstände: "Was" (Item) und "Wie viele" (Amount).
    /// Wird für Inventarplätze, Beute und Belohnungen benutzt.
    /// </summary>
    [Serializable]
    public class ItemStack
    {
        public ItemData Item;
        public int Amount;

        public ItemStack(ItemData item, int amount)
        {
            Item = item;
            Amount = amount;
        }

        public bool IsEmpty => Item == null || Amount <= 0;

        /// <summary>Wie viele passen noch auf diesen Stapel?</summary>
        public int SpaceLeft => Item == null ? 0 : Item.MaxStack - Amount;

        public void Clear()
        {
            Item = null;
            Amount = 0;
        }
    }
}
