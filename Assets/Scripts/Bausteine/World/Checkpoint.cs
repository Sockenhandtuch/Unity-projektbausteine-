using UnityEngine;
using UnityEngine.Events;

namespace Bausteine
{
    /// <summary>
    /// Rastplatz (Hollow-Knight-Bank / Stardew-Bett / Diablo-Wegpunkt).
    /// Interaktion: Leben auffüllen, Wiedereinstiegspunkt setzen und automatisch speichern.
    /// </summary>
    public class Checkpoint : MonoBehaviour, IInteractable
    {
        [Tooltip("Wo der Spieler nach dem Tod erscheint. Leer = Position der Bank.")]
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private bool healPlayer = true;
        [SerializeField] private bool saveGame = true;
        [SerializeField] private string prompt = "Ausruhen";
        [SerializeField] private UnityEvent onRested;

        public string Prompt => prompt;

        public bool CanInteract(PlayerInteractor interactor) => true;

        public void Interact(PlayerInteractor interactor)
        {
            Vector2 point = respawnPoint != null ? respawnPoint.position : transform.position;

            if (interactor.TryGetComponent(out PlayerRespawn respawn)) respawn.SetRespawnPoint(point);
            if (healPlayer && interactor.TryGetComponent(out Health health)) health.Heal(health.Max);
            if (saveGame && interactor.TryGetComponent(out PlayerSaveController save)) save.Save();

            onRested?.Invoke();
        }
    }
}
