using System.IO;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Speichert und lädt beliebige Datenklassen als JSON-Datei.
    /// Dateien liegen in Application.persistentDataPath (pro Plattform unterschiedlich).
    /// Die Datenklasse muss [System.Serializable] sein und darf nur Felder enthalten,
    /// die JsonUtility versteht (keine Dictionaries, keine Properties).
    /// </summary>
    public static class SaveSystem
    {
        private static string GetPath(string slot) => Path.Combine(Application.persistentDataPath, slot + ".json");

        public static void Save<T>(string slot, T data)
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);

            // Erst in eine Temp-Datei schreiben, dann austauschen – schützt vor kaputten
            // Spielständen, falls das Spiel mitten im Speichern abstürzt.
            string path = GetPath(slot);
            string tempPath = path + ".tmp";
            File.WriteAllText(tempPath, json);
            if (File.Exists(path)) File.Delete(path);
            File.Move(tempPath, path);
        }

        /// <summary>Lädt einen Spielstand.</summary>
        /// <returns>false, wenn keiner existiert oder die Datei beschädigt ist.</returns>
        public static bool TryLoad<T>(string slot, out T data)
        {
            data = default;
            string path = GetPath(slot);
            if (!File.Exists(path)) return false;

            try
            {
                data = JsonUtility.FromJson<T>(File.ReadAllText(path));
                return data != null;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Spielstand '{slot}' konnte nicht gelesen werden: {e.Message}");
                return false;
            }
        }

        public static bool Exists(string slot) => File.Exists(GetPath(slot));

        public static void Delete(string slot)
        {
            string path = GetPath(slot);
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
