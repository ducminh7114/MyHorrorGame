using UnityEngine;

public class randomItemSpawnPoint : MonoBehaviour
{
    [Header("Object To Move")]
    public GameObject targetObject;

    [Header("Possible Locations")]
    public Transform[] spawnPoints;

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("No target object assigned!");
            return;
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);

        targetObject.transform.position = spawnPoints[randomIndex].position;
        targetObject.transform.rotation = spawnPoints[randomIndex].rotation;
    }
}
