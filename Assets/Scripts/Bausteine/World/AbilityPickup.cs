using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Ein Objekt in der Welt, das beim Berühren eine Fähigkeit freischaltet
    /// (z.B. "Mantel des Windes" → Dash).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class AbilityPickup : MonoBehaviour
    {
        [SerializeField] private AbilityType ability;
        [SerializeField, TextArea] private string message = "Du hast eine neue Fähigkeit erlernt!";

        private void Awake() => GetComponent<Collider2D>().isTrigger = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerAbilities abilities = other.GetComponentInParent<PlayerAbilities>();
            if (abilities == null) return;

            abilities.Unlock(ability);
            if (DialogueBox.Instance != null && !string.IsNullOrEmpty(message))
                DialogueBox.Instance.Show(null, new[] { message });

            Destroy(gameObject);
        }
    }
}
