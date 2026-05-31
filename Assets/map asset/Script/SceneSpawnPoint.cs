using UnityEngine;

public class SceneSpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnPointId;

    public string SpawnPointId => spawnPointId;

    public static SceneSpawnPoint Find(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        SceneSpawnPoint[] spawnPoints = FindObjectsByType<SceneSpawnPoint>(FindObjectsSortMode.None);
        foreach (SceneSpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint != null && spawnPoint.spawnPointId == id)
            {
                return spawnPoint;
            }
        }

        GameObject fallback = GameObject.Find(id);
        return fallback != null ? fallback.GetComponent<SceneSpawnPoint>() : null;
    }
}
