using System.Collections.Generic;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Erzeugt Gegner in einem Bereich und hält eine Maximalanzahl am Leben.
    /// Optional: Respawn nach Zeit (Diablo-artige Gebiete) oder einmalig (Zelda-Räume).
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField, Min(1)] private int maxAlive = 3;
        [SerializeField] private float spawnRadius = 3f;

        [Tooltip("0 = kein Respawn (nur einmal beim Start füllen).")]
        [SerializeField] private float respawnDelay = 0f;

        private readonly List<GameObject> alive = new List<GameObject>();
        private float nextSpawnTime;

        private void Start()
        {
            for (int i = 0; i < maxAlive; i++) Spawn();
        }

        private void Update()
        {
            if (respawnDelay <= 0f) return;

            // Zerstörte Gegner aus der Liste werfen (Unity-Objekte sind nach Destroy "== null").
            int before = alive.Count;
            alive.RemoveAll(e => e == null);
            if (alive.Count < before) nextSpawnTime = Time.time + respawnDelay;

            if (alive.Count < maxAlive && Time.time >= nextSpawnTime)
            {
                Spawn();
                nextSpawnTime = Time.time + respawnDelay;
            }
        }

        private void Spawn()
        {
            if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Vector2 position = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            GameObject enemy = Instantiate(prefab, position, Quaternion.identity);

            if (enemy.TryGetComponent(out EnemyBrain brain)) brain.SetHome(position);
            alive.Add(enemy);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
