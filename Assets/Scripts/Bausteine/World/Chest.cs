using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Bausteine
{
    /// <summary>
    /// Truhe, die einmalig geöffnet werden kann.
    /// Inhalt: feste Gegenstände (Zelda: "Du hast den Bumerang gefunden!")
    /// und/oder zufällige Beute aus einer LootTable (Diablo).
    /// </summary>
    public class Chest : MonoBehaviour, IInteractable
    {
        [Header("Inhalt")]
        [SerializeField] private List<ItemStack> fixedContents = new List<ItemStack>();
        [SerializeField] private LootTable lootTable;

        [Tooltip("true = Gegenstände direkt ins Inventar. false = sie springen heraus auf den Boden.")]
        [SerializeField] private bool giveDirectly = true;

        [Header("Darstellung")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite openSprite;

        [Header("Speichern (optional)")]
        [Tooltip("Eindeutige ID, damit die Truhe nach dem Laden offen bleibt. Leer = wird nicht gespeichert.")]
        [SerializeField] private string persistentId;

        [SerializeField] private UnityEvent onOpened;

        public bool IsOpen { get; private set; }
        public string Prompt => "Öffnen";

        private void Start()
        {
            // Bereits in einem früheren Durchgang geöffnet?
            if (!string.IsNullOrEmpty(persistentId) && WorldFlags.Get(persistentId)) SetOpenVisual();
        }

        public bool CanInteract(PlayerInteractor interactor) => !IsOpen;

        public void Interact(PlayerInteractor interactor)
        {
            if (IsOpen) return;

            SetOpenVisual();
            if (!string.IsNullOrEmpty(persistentId)) WorldFlags.Set(persistentId, true);

            var contents = new List<ItemStack>(fixedContents);
            if (lootTable != null) contents.AddRange(lootTable.RollItems());

            Inventory inventory = interactor.GetComponent<Inventory>();
            foreach (ItemStack stack in contents)
            {
                if (stack == null || stack.IsEmpty) continue;

                int leftover = stack.Amount;
                if (giveDirectly && inventory != null) leftover = inventory.Add(stack.Item, stack.Amount);

                // Was nicht ins Inventar passt, landet auf dem Boden.
                if (leftover > 0 && PickupSpawner.Instance != null)
                    PickupSpawner.Instance.SpawnItem(new ItemStack(stack.Item, leftover), transform.position);
            }

            if (lootTable != null && PickupSpawner.Instance != null)
                PickupSpawner.Instance.SpawnGold(lootTable.RollGold(), transform.position);

            onOpened?.Invoke();
        }

        private void SetOpenVisual()
        {
            IsOpen = true;
            if (spriteRenderer != null && openSprite != null) spriteRenderer.sprite = openSprite;
        }
    }
}
