using System;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Geldbörse des Spielers (Gold, Geo, Rubine – wie auch immer die Währung heißt).
    /// </summary>
    public class Wallet : MonoBehaviour
    {
        [SerializeField, Min(0)] private int startAmount = 0;

        public int Amount { get; private set; }

        /// <summary>Neuer Kontostand.</summary>
        public event Action<int> Changed;

        private void Awake() => Amount = startAmount;

        public void Add(int value)
        {
            if (value <= 0) return;
            Amount += value;
            Changed?.Invoke(Amount);
        }

        public bool CanAfford(int price) => Amount >= price;

        /// <summary>Bezahlt nur, wenn genug Geld vorhanden ist.</summary>
        public bool TrySpend(int price)
        {
            if (price < 0 || !CanAfford(price)) return false;
            Amount -= price;
            Changed?.Invoke(Amount);
            return true;
        }

        /// <summary>Nimmt das gesamte Geld heraus (z.B. beim Tod) und gibt die Menge zurück.</summary>
        public int TakeAll()
        {
            int all = Amount;
            Amount = 0;
            Changed?.Invoke(Amount);
            return all;
        }

        /// <summary>Setzt den Kontostand direkt (Spielstand laden).</summary>
        public void SetAmount(int value)
        {
            Amount = Mathf.Max(0, value);
            Changed?.Invoke(Amount);
        }
    }
}
