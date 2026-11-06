using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bausteine
{
    /// <summary>
    /// Übergang in eine andere Szene (Höhleneingang, Haustür, Dungeon-Treppe).
    /// Merkt sich die Ziel-SpawnPoint-ID, damit der Spieler im neuen Level
    /// an der richtigen Stelle erscheint.
    /// Die Zielszene muss in den Build Settings eingetragen sein.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class SceneDoor : MonoBehaviour
    {
        [SerializeField] private string targetScene;
        [Tooltip("ID des SpawnPoints in der Zielszene.")]
        [SerializeField] private string targetSpawnId;

        /// <summary>Wird von SpawnPoint in der neuen Szene gelesen.</summary>
        public static string PendingSpawnId { get; private set; }

        private bool isLoading;

        private void Awake() => GetComponent<Collider2D>().isTrigger = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isLoading || !other.CompareTag("Player")) return;

            isLoading = true;
            PendingSpawnId = targetSpawnId;
            SceneManager.LoadScene(targetScene);
        }

        /// <summary>Wird vom SpawnPoint aufgerufen, nachdem er den Spieler platziert hat.</summary>
        public static void ClearPendingSpawn() => PendingSpawnId = null;
    }
}
