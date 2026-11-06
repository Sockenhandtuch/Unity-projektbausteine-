using UnityEngine;
using UnityEngine.InputSystem;

namespace Bausteine
{
    /// <summary>
    /// Liest Tastatur, Maus und Gamepad aus und stellt die Eingaben als einfache Werte bereit.
    /// Alle anderen Spieler-Bausteine lesen NUR hier – so lässt sich die Steuerung an einer
    /// Stelle ändern (z.B. später auf ein Input-Action-Asset umstellen).
    ///
    /// Benötigt das Paket "Input System" (in Unity 6 Standard).
    ///
    /// Belegung:
    ///   Bewegen     WASD / Pfeiltasten / linker Stick
    ///   Angriff     J / linke Maustaste / X (Xbox) bzw. □
    ///   Dash        Leertaste / K / B (Xbox) bzw. ○
    ///   Interagieren E / A (Xbox) bzw. ✕
    ///   Werkzeug    F / rechte Maustaste / Y (Xbox) bzw. △
    /// </summary>
    [DefaultExecutionOrder(-100)] // Vor allen anderen Skripten lesen, damit alle denselben Stand sehen
    public class PlayerInputReader : MonoBehaviour
    {
        /// <summary>Wenn false, liefern alle Spielaktionen "nichts" (z.B. während eines Dialogs).</summary>
        public bool GameplayEnabled { get; set; } = true;

        // --- Spielaktionen (respektieren GameplayEnabled) ---
        public Vector2 Move { get; private set; }
        public bool AttackPressed { get; private set; }
        public bool DashPressed { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool UseToolPressed { get; private set; }

        // --- Menü/Dialog (immer aktiv) ---
        /// <summary>Bestätigen-Taste, unabhängig von GameplayEnabled (für Dialoge und Menüs).</summary>
        public bool SubmitPressed { get; private set; }

        private void Update()
        {
            Keyboard kb = Keyboard.current;
            Mouse mouse = Mouse.current;
            Gamepad pad = Gamepad.current;

            bool attack = Pressed(kb?.jKey) || Pressed(mouse?.leftButton) || Pressed(pad?.buttonWest);
            bool dash = Pressed(kb?.spaceKey) || Pressed(kb?.kKey) || Pressed(pad?.buttonEast);
            bool interact = Pressed(kb?.eKey) || Pressed(pad?.buttonSouth);
            bool tool = Pressed(kb?.fKey) || Pressed(mouse?.rightButton) || Pressed(pad?.buttonNorth);

            SubmitPressed = interact || Pressed(kb?.enterKey);

            if (!GameplayEnabled)
            {
                Move = Vector2.zero;
                AttackPressed = DashPressed = InteractPressed = UseToolPressed = false;
                return;
            }

            Move = ReadMove(kb, pad);
            AttackPressed = attack;
            DashPressed = dash;
            InteractPressed = interact;
            UseToolPressed = tool;
        }

        private static Vector2 ReadMove(Keyboard kb, Gamepad pad)
        {
            Vector2 move = Vector2.zero;

            if (kb != null)
            {
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    move.y += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  move.y -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) move.x += 1f;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  move.x -= 1f;
            }

            // Der Stick gewinnt, wenn er stärker ausgelenkt ist als die Tastatur.
            if (pad != null)
            {
                Vector2 stick = pad.leftStick.ReadValue();
                if (stick.sqrMagnitude > move.sqrMagnitude) move = stick;
            }

            // Diagonal nicht schneller laufen als geradeaus.
            return Vector2.ClampMagnitude(move, 1f);
        }

        // Null-sichere Abfrage "wurde in diesem Frame gedrückt".
        private static bool Pressed(UnityEngine.InputSystem.Controls.ButtonControl button)
        {
            return button != null && button.wasPressedThisFrame;
        }
    }
}
