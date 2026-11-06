using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Freischaltbare Fähigkeiten (Metroidvania-Prinzip aus Hollow Knight und Zelda):
    /// Neue Fähigkeiten öffnen neue Wege in bereits besuchten Gebieten.
    /// </summary>
    public enum AbilityType
    {
        Dash,        // Ausweichen / Abgründe überwinden
        Swim,        // Tiefes Wasser durchqueren
        PowerGlove,  // Schwere Felsen heben
        Lantern,     // Dunkle Höhlen betreten
        DoubleDash   // Zweiter Dash in der Luft/hintereinander
    }

    /// <summary>
    /// Speichert, welche Fähigkeiten der Spieler bereits besitzt.
    /// </summary>
    public class PlayerAbilities : MonoBehaviour
    {
        [Tooltip("Fähigkeiten, die der Spieler von Anfang an hat.")]
        [SerializeField] private List<AbilityType> startingAbilities = new List<AbilityType>();

        private readonly HashSet<AbilityType> unlocked = new HashSet<AbilityType>();

        /// <summary>Neue Fähigkeit freigeschaltet (für Popup "Du kannst jetzt dashen!").</summary>
        public event Action<AbilityType> Unlocked;

        private void Awake()
        {
            foreach (AbilityType ability in startingAbilities) unlocked.Add(ability);
        }

        public bool Has(AbilityType ability) => unlocked.Contains(ability);

        public void Unlock(AbilityType ability)
        {
            if (unlocked.Add(ability)) Unlocked?.Invoke(ability);
        }

        public List<AbilityType> GetAll() => new List<AbilityType>(unlocked);
    }
}
