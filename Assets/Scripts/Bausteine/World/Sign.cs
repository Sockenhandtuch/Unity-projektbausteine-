using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Ein lesbares Schild, eine Inschrift oder ein einfacher NPC mit festen Sätzen.
    /// Zeigt die Zeilen über die DialogueBox an.
    /// </summary>
    public class Sign : MonoBehaviour, IInteractable
    {
        [SerializeField] private string speakerName;
        [SerializeField, TextArea(2, 5)] private string[] lines = { "Willkommen im Dorf!" };
        [SerializeField] private string prompt = "Lesen";

        public string Prompt => prompt;

        public bool CanInteract(PlayerInteractor interactor)
        {
            return DialogueBox.Instance != null && !DialogueBox.Instance.IsOpen;
        }

        public void Interact(PlayerInteractor interactor)
        {
            DialogueBox.Instance.Show(speakerName, lines);
        }
    }
}
