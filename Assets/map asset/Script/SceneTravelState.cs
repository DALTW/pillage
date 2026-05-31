using UnityEngine.SceneManagement;

public static class SceneTravelState
{
    public const string SampleSceneName = "SampleScene";
    public const string MainSceneName = "MainScene";
    public const string StartSceneName = "StartScene";
    public const string EndSceneName = "EndScene";
    public const string OutsideSpawnPointId = "OutsideHouseDoor";
    public const string InteriorReturnSpawnPointId = "WoodhouseDoorReturnPoint";

    private static string pendingSceneName;
    private static string pendingSpawnPointId;

    public static void LoadScene(string sceneName, string spawnPointId)
    {
        pendingSceneName = string.IsNullOrEmpty(spawnPointId) ? null : sceneName;
        pendingSpawnPointId = spawnPointId;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public static bool TryConsumeSpawn(string sceneName, out string spawnPointId)
    {
        spawnPointId = null;

        if (string.IsNullOrEmpty(pendingSpawnPointId))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(pendingSceneName) && pendingSceneName != sceneName)
        {
            return false;
        }

        spawnPointId = pendingSpawnPointId;
        pendingSceneName = null;
        pendingSpawnPointId = null;
        return true;
    }

    public static bool ShouldDestroyPersistentPlayer(string sceneName)
    {
        return sceneName == StartSceneName || sceneName == EndSceneName;
    }
}
