using TMPro;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Zeigt einen Hinweis wie "[E] Öffnen" an, solange der Spieler etwas anvisiert.
    /// Am besten in einen World-Space-Canvas oder ans HUD hängen.
    /// </summary>
    public class InteractPrompt : MonoBehaviour
    {
        [SerializeField] private PlayerInteractor interactor;
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text label;
        [SerializeField] private string keyHint = "[E]";

        private void Start()
        {
            if (interactor == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) interactor = player.GetComponent<PlayerInteractor>();
            }
            if (interactor != null) interactor.TargetChanged += OnTargetChanged;
            OnTargetChanged(null);
        }

        private void OnDestroy()
        {
            if (interactor != null) interactor.TargetChanged -= OnTargetChanged;
        }

        private void OnTargetChanged(IInteractable target)
        {
            root.SetActive(target != null);
            if (target != null) label.text = $"{keyHint} {target.Prompt}";
        }
    }
}
