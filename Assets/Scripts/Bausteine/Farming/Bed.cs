using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Bett: Schlafen beendet den Tag, füllt Leben und Ausdauer auf und speichert.
    /// </summary>
    public class Bed : MonoBehaviour, IInteractable
    {
        [Tooltip("Ab dieser Stunde darf geschlafen werden.")]
        [SerializeField, Range(0, 23)] private int earliestSleepHour = 18;

        public string Prompt => "Schlafen";

        public bool CanInteract(PlayerInteractor interactor)
        {
            return GameClock.Instance != null && GameClock.Instance.Hour >= earliestSleepHour;
        }

        public void Interact(PlayerInteractor interactor)
        {
            GameClock.Instance.SleepUntilMorning();

            if (interactor.TryGetComponent(out Health health)) health.Heal(health.Max);
            if (interactor.TryGetComponent(out Stamina stamina)) stamina.RestoreFull();
            if (interactor.TryGetComponent(out PlayerSaveController save)) save.Save();
        }
    }
}
