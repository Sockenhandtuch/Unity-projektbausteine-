using System;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Inventar mit fester Anzahl an Plätzen (Slots).
    /// Stapelbare Gegenstände werden zuerst auf vorhandene Stapel verteilt,
    /// danach auf freie Plätze.
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        [SerializeField, Min(1)] private int size = 24;

        private ItemStack[] slots;

        /// <summary>Wird bei jeder Änderung ausgelöst – UI neu zeichnen.</summary>
        public event Action Changed;

        public int Size => size;

        private void Awake()
        {
            slots = new ItemStack[size];
            for (int i = 0; i < size; i++) slots[i] = new ItemStack(null, 0);
        }

        /// <summary>Liest einen Platz (null-sicher, nie null).</summary>
        public ItemStack GetSlot(int index) => slots[index];

        /// <summary>
        /// Fügt Gegenstände hinzu.
        /// </summary>
        /// <returns>Anzahl, die NICHT mehr hineingepasst hat (0 = alles aufgenommen).</returns>
        public int Add(ItemData item, int amount = 1)
        {
            if (item == null || amount <= 0) return amount;

            int remaining = amount;

            // 1. Auf vorhandene Stapel desselben Typs verteilen.
            if (item.IsStackable)
            {
                foreach (ItemStack slot in slots)
                {
                    if (remaining == 0) break;
                    if (slot.Item != item || slot.SpaceLeft <= 0) continue;

                    int moved = Mathf.Min(slot.SpaceLeft, remaining);
                    slot.Amount += moved;
                    remaining -= moved;
                }
            }

            // 2. Rest auf leere Plätze verteilen.
            foreach (ItemStack slot in slots)
            {
                if (remaining == 0) break;
                if (!slot.IsEmpty) continue;

                int moved = Mathf.Min(item.MaxStack, remaining);
                slot.Item = item;
                slot.Amount = moved;
                remaining -= moved;
            }

            if (remaining != amount) Changed?.Invoke();
            return remaining;
        }

        /// <summary>
        /// Entfernt Gegenstände – aber nur, wenn genug vorhanden sind (alles oder nichts).
        /// </summary>
        public bool Remove(ItemData item, int amount = 1)
        {
            if (item == null || amount <= 0 || Count(item) < amount) return false;

            int remaining = amount;
            // Von hinten nach vorne leeren, damit vordere Plätze (Hotbar) möglichst gefüllt bleiben.
            for (int i = slots.Length - 1; i >= 0 && remaining > 0; i--)
            {
                ItemStack slot = slots[i];
                if (slot.Item != item) continue;

                int taken = Mathf.Min(slot.Amount, remaining);
                slot.Amount -= taken;
                remaining -= taken;
                if (slot.Amount <= 0) slot.Clear();
            }

            Changed?.Invoke();
            return true;
        }

        /// <summary>Entfernt eine Menge aus einem bestimmten Platz.</summary>
        public bool RemoveAt(int index, int amount = 1)
        {
            ItemStack slot = slots[index];
            if (slot.IsEmpty || slot.Amount < amount) return false;

            slot.Amount -= amount;
            if (slot.Amount <= 0) slot.Clear();
            Changed?.Invoke();
            return true;
        }

        /// <summary>Zählt, wie viele Exemplare eines Gegenstands insgesamt vorhanden sind.</summary>
        public int Count(ItemData item)
        {
            int total = 0;
            foreach (ItemStack slot in slots)
                if (slot.Item == item) total += slot.Amount;
            return total;
        }

        public bool Has(ItemData item, int amount = 1) => Count(item) >= amount;

        /// <summary>Tauscht zwei Plätze (für Drag & Drop in der UI).</summary>
        public void Swap(int indexA, int indexB)
        {
            if (indexA == indexB) return;
            (slots[indexA], slots[indexB]) = (slots[indexB], slots[indexA]);
            Changed?.Invoke();
        }

        /// <summary>Leert das Inventar komplett (z.B. vor dem Laden eines Spielstands).</summary>
        public void Clear()
        {
            foreach (ItemStack slot in slots) slot.Clear();
            Changed?.Invoke();
        }

        /// <summary>Setzt einen Platz direkt (z.B. beim Laden eines Spielstands).</summary>
        public void SetSlot(int index, ItemData item, int amount)
        {
            slots[index].Item = item;
            slots[index].Amount = item == null ? 0 : amount;
            Changed?.Invoke();
        }
    }
}
