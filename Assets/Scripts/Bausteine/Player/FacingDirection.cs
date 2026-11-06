using System;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Merkt sich, in welche Richtung ein Charakter schaut – auch wenn er stillsteht.
    /// Wird von Angriff, Interaktion und Animation benutzt.
    /// Spieler: PlayerInputReader zuweisen. Gegner: Face() aus dem eigenen Skript aufrufen.
    /// </summary>
    public class FacingDirection : MonoBehaviour
    {
        [Tooltip("Optional: Blickrichtung automatisch aus der Eingabe übernehmen.")]
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private Vector2 startDirection = Vector2.down;

        /// <summary>Genaue Blickrichtung (normalisiert, auch diagonal).</summary>
        public Vector2 Direction { get; private set; }

        /// <summary>Auf 4 Richtungen gerundete Blickrichtung – ideal für 4-Wege-Pixelart-Sprites.</summary>
        public Vector2 Cardinal => ToCardinal(Direction);

        /// <summary>Wenn true, ändert sich die Richtung nicht (z.B. während eines Schwerthiebs).</summary>
        public bool Locked { get; set; }

        /// <summary>Neue Blickrichtung.</summary>
        public event Action<Vector2> Changed;

        private void Awake() => Direction = startDirection.normalized;

        private void Update()
        {
            if (input != null) Face(input.Move);
        }

        /// <summary>Dreht den Charakter in die angegebene Richtung (Nullvektor wird ignoriert).</summary>
        public void Face(Vector2 direction)
        {
            if (Locked || direction.sqrMagnitude < 0.01f) return;

            Vector2 normalized = direction.normalized;
            if (normalized == Direction) return;

            Direction = normalized;
            Changed?.Invoke(Direction);
        }

        /// <summary>Rundet einen Vektor auf die nächste der 4 Hauptrichtungen.</summary>
        public static Vector2 ToCardinal(Vector2 v)
        {
            if (Mathf.Abs(v.x) > Mathf.Abs(v.y)) return new Vector2(Mathf.Sign(v.x), 0f);
            return new Vector2(0f, Mathf.Sign(v.y));
        }
    }
}
