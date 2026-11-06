using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Gewichtete Beutetabelle (Diablo-Stil) als ScriptableObject.
    /// Beispiel: Schwert Gewicht 1, Heiltrank Gewicht 5, "Nichts" Gewicht 10
    /// → Heiltrank ist 5x so wahrscheinlich wie das Schwert.
    /// </summary>
    [CreateAssetMenu(menuName = "Bausteine/Items/Loot Table", fileName = "NewLootTable")]
    public class LootTable : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public ItemData Item;
            [Min(0)] public float Weight;
            [Min(1)] public int MinAmount;
            [Min(1)] public int MaxAmount;
        }

        [SerializeField] private List<Entry> entries = new List<Entry>();

        [Tooltip("Gewicht für 'nichts fällt'. Höher = seltenere Beute.")]
        [SerializeField, Min(0)] private float nothingWeight = 0f;

        [Tooltip("Wie oft gewürfelt wird (z.B. 3 für einen Boss).")]
        [SerializeField, Min(1)] private int rolls = 1;

        [Header("Gold")]
        [SerializeField, Min(0)] private int minGold = 0;
        [SerializeField, Min(0)] private int maxGold = 0;

        /// <summary>
        /// Würfelt die Beute aus.
        /// </summary>
        /// <param name="magicFind">Bonus für seltene Gegenstände (0.5 = +50 % Gewicht für alles ab "Magic").</param>
        public List<ItemStack> RollItems(float magicFind = 0f)
        {
            var result = new List<ItemStack>();

            for (int i = 0; i < rolls; i++)
            {
                ItemStack drop = RollOnce(magicFind);
                if (drop != null) result.Add(drop);
            }
            return result;
        }

        /// <summary>Würfelt die Goldmenge aus.</summary>
        /// <param name="goldFind">0.5 = +50 % Gold.</param>
        public int RollGold(float goldFind = 0f)
        {
            if (maxGold <= 0) return 0;
            int gold = UnityEngine.Random.Range(minGold, maxGold + 1);
            return Mathf.RoundToInt(gold * (1f + goldFind));
        }

        private ItemStack RollOnce(float magicFind)
        {
            // Gesamtgewicht berechnen – seltene Gegenstände werden durch Magic Find aufgewertet.
            float total = nothingWeight;
            foreach (Entry e in entries) total += GetWeight(e, magicFind);
            if (total <= 0f) return null;

            // Zufallszahl ziehen und durch die Einträge "laufen", bis sie verbraucht ist.
            float roll = UnityEngine.Random.value * total;
            foreach (Entry e in entries)
            {
                roll -= GetWeight(e, magicFind);
                if (roll < 0f)
                {
                    int amount = UnityEngine.Random.Range(e.MinAmount, Mathf.Max(e.MinAmount, e.MaxAmount) + 1);
                    return new ItemStack(e.Item, amount);
                }
            }
            return null; // "Nichts" gewürfelt
        }

        private static float GetWeight(Entry e, float magicFind)
        {
            if (e.Item == null) return 0f;
            return e.Item.Rarity == ItemRarity.Common ? e.Weight : e.Weight * (1f + magicFind);
        }
    }
}
