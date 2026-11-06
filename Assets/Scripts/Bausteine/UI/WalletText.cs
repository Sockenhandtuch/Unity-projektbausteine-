using TMPro;
using UnityEngine;

namespace Bausteine
{
    /// <summary>Zeigt den Kontostand als Text an, z.B. "125".</summary>
    [RequireComponent(typeof(TMP_Text))]
    public class WalletText : MonoBehaviour
    {
        [SerializeField] private Wallet wallet;
        [SerializeField] private string format = "{0}";

        private TMP_Text text;

        private void Awake() => text = GetComponent<TMP_Text>();

        private void Start()
        {
            if (wallet == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) wallet = player.GetComponent<Wallet>();
            }
            if (wallet == null) return;

            wallet.Changed += Show;
            Show(wallet.Amount);
        }

        private void OnDestroy()
        {
            if (wallet != null) wallet.Changed -= Show;
        }

        private void Show(int amount) => text.text = string.Format(format, amount);
    }
}
