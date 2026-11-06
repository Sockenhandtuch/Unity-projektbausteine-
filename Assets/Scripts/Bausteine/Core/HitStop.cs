using System.Collections;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Friert das Spiel für einen winzigen Moment ein, wenn ein Treffer landet.
    /// Dieser "Hit Stop" macht Schläge viel wuchtiger (siehe Hollow Knight).
    /// Einmal pro Szene auf ein beliebiges Objekt legen.
    /// </summary>
    public class HitStop : MonoBehaviour
    {
        public static HitStop Instance { get; private set; }

        private bool isFrozen;

        private void Awake() => Instance = this;

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            Time.timeScale = 1f;
        }

        /// <summary>Hält die Zeit für die angegebene Dauer (Echtzeit-Sekunden) an.</summary>
        public void Freeze(float seconds)
        {
            if (isFrozen || seconds <= 0f) return;
            StartCoroutine(FreezeRoutine(seconds));
        }

        private IEnumerator FreezeRoutine(float seconds)
        {
            isFrozen = true;
            float previousScale = Time.timeScale;
            Time.timeScale = 0f;

            // Realtime, weil normale WaitForSeconds bei timeScale 0 nie enden würde.
            yield return new WaitForSecondsRealtime(seconds);

            Time.timeScale = previousScale;
            isFrozen = false;
        }
    }
}
