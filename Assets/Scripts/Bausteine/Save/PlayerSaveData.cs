using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Alle Daten eines Spielstands als einfache, serialisierbare Felder.
    /// Gegenstände werden als ID gespeichert (nicht als Asset-Referenz).
    /// </summary>
    [Serializable]
    public class PlayerSaveData
    {
        public string sceneName;
        public Vector2 position;
        public Vector2 respawnPoint;

        public int health;
        public int gold;

        public int level;
        public int xp;
        public int unspentStatPoints;

        public List<SavedSlot> inventory = new List<SavedSlot>();
        public List<string> equipment = new List<string>();
        public List<AbilityType> abilities = new List<AbilityType>();
        public List<string> worldFlags = new List<string>();

        public int day;
        public Season season;
        public int year;
        public int minuteOfDay;
    }

    /// <summary>Ein Inventarplatz im Spielstand.</summary>
    [Serializable]
    public struct SavedSlot
    {
        public string itemId;
        public int amount;
    }
}
