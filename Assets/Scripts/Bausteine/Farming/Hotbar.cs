using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bausteine
{
    /// <summary>
    /// Schnellleiste: Die ersten N Inventarplätze, von denen einer "in der Hand" ist.
    /// Auswahl per Zifferntasten 1–9, Mausrad oder Schultertasten.
    /// </summary>
    [RequireComponent(typeof(Inventory))]
    public class Hotbar : MonoBehaviour
    {
        [SerializeField, Range(1, 9)] private int slotCount = 9;

        private Inventory inventory;

        public int SelectedIndex { get; private set; }
        public int SlotCount => slotCount;

        /// <summary>Der Gegenstand in der Hand (oder null).</summary>
        public ItemData SelectedItem => inventory.GetSlot(SelectedIndex).Item;

        public event Action<int> SelectionChanged;

        private void Awake() => inventory = GetComponent<Inventory>();

        private void Update()
        {
            Keyboard kb = Keyboard.current;
            if (kb != null)
            {
                // digit1Key .. digit9Key liegen im Enum direkt hintereinander.
                for (int i = 0; i < slotCount; i++)
                {
                    if (kb[Key.Digit1 + i].wasPressedThisFrame) Select(i);
                }
            }

            Mouse mouse = Mouse.current;
            if (mouse != null)
            {
                float scroll = mouse.scroll.ReadValue().y;
                if (scroll > 0f) Select(SelectedIndex - 1);
                else if (scroll < 0f) Select(SelectedIndex + 1);
            }

            Gamepad pad = Gamepad.current;
            if (pad != null)
            {
                if (pad.leftShoulder.wasPressedThisFrame) Select(SelectedIndex - 1);
                if (pad.rightShoulder.wasPressedThisFrame) Select(SelectedIndex + 1);
            }
        }

        /// <summary>Wählt einen Platz. Läuft am Ende/Anfang herum.</summary>
        public void Select(int index)
        {
            index = (index % slotCount + slotCount) % slotCount;
            if (index == SelectedIndex) return;

            SelectedIndex = index;
            SelectionChanged?.Invoke(SelectedIndex);
        }
    }
}
