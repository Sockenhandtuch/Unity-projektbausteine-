using UnityEngine;

namespace Bausteine
{
    public enum ToolType
    {
        Hoe,          // Boden umgraben
        WateringCan,  // Gießen
        Axe,          // Bäume fällen
        Pickaxe,      // Steine abbauen
        Scythe        // Gras/Pflanzen entfernen
    }

    /// <summary>Ein Werkzeug als Gegenstand.</summary>
    [CreateAssetMenu(menuName = "Bausteine/Items/Tool", fileName = "NewTool")]
    public class ToolData : ItemData
    {
        [SerializeField] private ToolType toolType;
        [Tooltip("Verbesserte Werkzeuge (Kupfer, Eisen, Gold...) machen mehr 'Schaden' an Steinen/Bäumen.")]
        [SerializeField, Min(1)] private int power = 1;
        [SerializeField, Min(0)] private float staminaCost = 2f;

        public ToolType ToolType => toolType;
        public int Power => power;
        public float StaminaCost => staminaCost;
    }
}
