using UnityEngine;
using UnityEngine.Events;

namespace Bausteine
{
    /// <summary>
    /// Verschlossene Tür (Zelda-Dungeon). Öffnet sich, wenn der Spieler den passenden
    /// Schlüssel im Inventar hat. Der Schlüssel wird dabei verbraucht (einstellbar).
    /// </summary>
    public class LockedDoor : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemData requiredKey;
        [SerializeField] private bool consumeKey = true;

        [Tooltip("Collider, der den Weg blockiert. Wird beim Öffnen abgeschaltet.")]
        [SerializeField] private Collider2D blockingCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite openSprite;

        [Tooltip("Optional: Hinweis, wenn der Schlüssel fehlt.")]
        [SerializeField] private string lockedMessage = "Die Tür ist verschlossen.";

        [Header("Speichern (optional)")]
        [SerializeField] private string persistentId;

        [SerializeField] private UnityEvent onOpened;

        public bool IsOpen { get; private set; }
        public string Prompt => "Aufschließen";

        private void Start()
        {
            if (!string.IsNullOrEmpty(persistentId) && WorldFlags.Get(persistentId)) Open(silent: true);
        }

        public bool CanInteract(PlayerInteractor interactor) => !IsOpen;

        public void Interact(PlayerInteractor interactor)
        {
            Inventory inventory = interactor.GetComponent<Inventory>();
            bool hasKey = requiredKey == null || (inventory != null && inventory.Has(requiredKey));

            if (!hasKey)
            {
                if (DialogueBox.Instance != null && !string.IsNullOrEmpty(lockedMessage))
                    DialogueBox.Instance.Show(null, new[] { lockedMessage });
                return;
            }

            if (consumeKey && requiredKey != null) inventory.Remove(requiredKey);
            Open(silent: false);
        }

        private void Open(bool silent)
        {
            IsOpen = true;
            if (blockingCollider != null) blockingCollider.enabled = false;
            if (spriteRenderer != null && openSprite != null) spriteRenderer.sprite = openSprite;
            if (!string.IsNullOrEmpty(persistentId)) WorldFlags.Set(persistentId, true);
            if (!silent) onOpened?.Invoke();
        }
    }
}
