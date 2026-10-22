using UnityEngine;
using System.Collections.Generic;

public class RandomItemSpawner : MonoBehaviour
{
    [Header("Must Spawn Item")]
    public GameObject requiredItem;

    [Header("Random Items")]
    public List<GameObject> randomItems = new List<GameObject>();

    [Header("Spawn Positions")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Total Items To Spawn")]
    public int itemsToSpawn = 5;

    [Header("Spawn On Start")]
    public bool spawnOnStart = true;

    private List<GameObject> spawnedItems = new List<GameObject>();

    private void Start()
    { 
            SpawnItems();
    }

    public void SpawnItems()
    {
        // Remove old spawned items
        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
                Destroy(item);
        }

        spawnedItems.Clear();

        if (spawnPoints.Count == 0)
            return;

        List<Transform> availableSpawns = new List<Transform>(spawnPoints);

        // -----------------------------
        // Spawn the required item first
        // -----------------------------
        if (requiredItem != null)
        {
            int spawnIndex = Random.Range(0, availableSpawns.Count);
            Transform spawnPoint = availableSpawns[spawnIndex];

            availableSpawns.RemoveAt(spawnIndex);

            GameObject spawned = Instantiate(
                requiredItem,
                spawnPoint.position,
                spawnPoint.rotation
            );

            spawnedItems.Add(spawned);
        }

        // -----------------------------
        // Spawn remaining random items
        // -----------------------------
        int remainingItems = itemsToSpawn - spawnedItems.Count;
        remainingItems = Mathf.Min(remainingItems, availableSpawns.Count);

        for (int i = 0; i < remainingItems; i++)
        {
            if (randomItems.Count == 0)
                break;

            int spawnIndex = Random.Range(0, availableSpawns.Count);
            Transform spawnPoint = availableSpawns[spawnIndex];

            availableSpawns.RemoveAt(spawnIndex);

            GameObject prefab =
                randomItems[Random.Range(0, randomItems.Count)];

            GameObject spawned = Instantiate(
                prefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            spawnedItems.Add(spawned);
        }
    }
}
