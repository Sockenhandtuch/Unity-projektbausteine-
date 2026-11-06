using System;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Ausdauer/Energie (Stardew): Werkzeuge verbrauchen sie, Schlafen und Essen füllen sie auf.
    /// </summary>
    public class Stamina : MonoBehaviour
    {
        [SerializeField, Min(1)] private float max = 100f;

        public float Current { get; private set; }
        public float Max => max;

        /// <summary>(aktuell, maximal) – für eine Energieleiste.</summary>
        public event Action<float, float> Changed;

        private void Awake() => Current = max;

        /// <summary>Verbraucht Ausdauer, wenn genug vorhanden ist.</summary>
        public bool TryUse(float amount)
        {
            if (amount <= 0f) return true;
            if (Current < amount) return false;

            Current -= amount;
            Changed?.Invoke(Current, max);
            return true;
        }

        public void Restore(float amount)
        {
            Current = Mathf.Min(max, Current + amount);
            Changed?.Invoke(Current, max);
        }

        public void RestoreFull() => Restore(max);
    }
}
