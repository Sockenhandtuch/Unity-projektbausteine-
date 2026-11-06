using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Verwaltet die angelegte Ausrüstung und überträgt deren Boni auf CharacterStats.
    /// Als "Quelle" der Modifikatoren dient der Slot – so werden beim Wechsel
    /// genau die Boni des alten Teils entfernt.
    /// </summary>
    [RequireComponent(typeof(CharacterStats))]
    public class Equipment : MonoBehaviour
    {
        private readonly Dictionary<EquipSlot, EquipmentData> equipped = new Dictionary<EquipSlot, EquipmentData>();
        private CharacterStats stats;
        private Inventory inventory;

        /// <summary>(Slot, neues Teil oder null) – UI aktualisieren.</summary>
        public event Action<EquipSlot, EquipmentData> Changed;

        private void Awake()
        {
            stats = GetComponent<CharacterStats>();
            inventory = GetComponent<Inventory>();
        }

        public EquipmentData Get(EquipSlot slot)
        {
            equipped.TryGetValue(slot, out EquipmentData item);
            return item;
        }

        /// <summary>Legt ein Teil an.</summary>
        /// <returns>Das vorher getragene Teil (oder null).</returns>
        public EquipmentData Equip(EquipmentData item)
        {
            if (item == null) return null;

            EquipmentData previous = Unequip(item.Slot);

            equipped[item.Slot] = item;
            foreach (StatBonus bonus in item.Bonuses)
                stats.AddModifier(bonus.Stat, bonus.ToModifier(item.Slot));

            Changed?.Invoke(item.Slot, item);
            return previous;
        }

        /// <summary>Legt das Teil in einem Slot ab und entfernt seine Boni.</summary>
        /// <returns>Das abgelegte Teil (oder null).</returns>
        public EquipmentData Unequip(EquipSlot slot)
        {
            if (!equipped.TryGetValue(slot, out EquipmentData item)) return null;

            equipped.Remove(slot);
            stats.RemoveAllModifiersFromSource(slot);
            Changed?.Invoke(slot, null);
            return item;
        }

        /// <summary>
        /// Bequemlichkeit: Legt ein Teil direkt aus dem Inventar an.
        /// Das alte Teil wandert zurück ins Inventar.
        /// </summary>
        public bool EquipFromInventory(int slotIndex)
        {
            if (inventory == null) return false;

            ItemStack stack = inventory.GetSlot(slotIndex);
            if (!(stack.Item is EquipmentData item)) return false;

            inventory.RemoveAt(slotIndex);
            EquipmentData previous = Equip(item);
            if (previous != null) inventory.Add(previous);
            return true;
        }

        /// <summary>Legt ein Teil ab und steckt es ins Inventar (falls Platz).</summary>
        public bool UnequipToInventory(EquipSlot slot)
        {
            if (inventory == null || Get(slot) == null) return false;
            if (inventory.Add(Get(slot)) > 0) return false; // Kein Platz → angelegt lassen

            Unequip(slot);
            return true;
        }
    }
}
