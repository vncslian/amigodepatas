using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnZone
    {
        public string zoneName = "Zona 1";
        public Vector2 center;          
        public Vector2 size = new Vector2(10f, 10f); 
        public int maxMonstersInZone = 3;
    }

    [Header("Configuração de Spawn")]
    public List<GameObject> monsterPrefabs = new List<GameObject>();
    public List<SpawnZone> spawnZones = new List<SpawnZone>();

    [Header("Limites Globais")]
    public int maxMonstersTotal = 10;
    public float spawnInterval = 5f;    

    // controle interno
    List<GameObject> activeMonsters = new List<GameObject>();
    float spawnTimer = 0f;

    void Update()
    {
        activeMonsters.RemoveAll(m => m == null);

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnInterval;
            TrySpawn();
        }
    }

    void TrySpawn()
    {
        if (monsterPrefabs == null || monsterPrefabs.Count == 0) return;
        if (activeMonsters.Count >= maxMonstersTotal) return;
        if (spawnZones.Count == 0) return;

        SpawnZone zone = spawnZones[Random.Range(0, spawnZones.Count)];

        int inZone = 0;
        foreach (GameObject m in activeMonsters)
        {
            if (m == null) continue;
            Vector2 pos = m.transform.position;
            Vector2 min = zone.center - zone.size * 0.5f;
            Vector2 max = zone.center + zone.size * 0.5f;
            if (pos.x >= min.x && pos.x <= max.x &&
                pos.y >= min.y && pos.y <= max.y)
                inZone++;
        }

        if (inZone >= zone.maxMonstersInZone) return;

        float rx = Random.Range(zone.center.x - zone.size.x * 0.5f,
                                zone.center.x + zone.size.x * 0.5f);
        float ry = Random.Range(zone.center.y - zone.size.y * 0.5f,
                                zone.center.y + zone.size.y * 0.5f);
        Vector2 spawnPos = new Vector2(rx, ry);

        GameObject selectedPrefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Count)];

        if (selectedPrefab == null) return;

        GameObject spawned = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        spawned.name = selectedPrefab.name;
        activeMonsters.Add(spawned);

        Debug.Log($"[SpawnManager] Monstro '{spawned.name}' spawnado em {spawnPos} (zona: {zone.zoneName})");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.3f);
        foreach (SpawnZone z in spawnZones)
        {
            Gizmos.DrawCube((Vector3)(z.center), new Vector3(z.size.x, z.size.y, 0.1f));
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector3)(z.center), new Vector3(z.size.x, z.size.y, 0.1f));
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.3f);
        }
    }
}