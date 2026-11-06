using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Bausteine
{
    /// <summary>Wo ein Ausrüstungsteil getragen wird.</summary>
    public enum EquipSlot
    {
        Weapon,
        Offhand,
        Head,
        Body,
        Hands,
        Feet,
        Ring,
        Amulet
    }

    /// <summary>
    /// Ein ausrüstbarer Gegenstand mit Wertboni (z.B. "+3 Schaden, +10% Laufgeschwindigkeit").
    /// </summary>
    [CreateAssetMenu(menuName = "Bausteine/Items/Equipment", fileName = "NewEquipment")]
    public class EquipmentData : ItemData
    {
        [SerializeField] private EquipSlot slot;
        [SerializeField] private List<StatBonus> bonuses = new List<StatBonus>();

        public EquipSlot Slot => slot;
        public IReadOnlyList<StatBonus> Bonuses => bonuses;

        public override string GetTooltip()
        {
            var sb = new StringBuilder();
            sb.AppendLine(slot.ToString());
            foreach (StatBonus bonus in bonuses) sb.AppendLine(bonus.ToString());
            if (!string.IsNullOrEmpty(Description)) sb.Append(Description);
            return sb.ToString();
        }
    }
}
