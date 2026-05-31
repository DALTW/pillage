using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScenePersistence : MonoBehaviour
{
    private static PlayerScenePersistence instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneTravelState.ShouldDestroyPersistentPlayer(scene.name))
        {
            Destroy(gameObject);
            return;
        }

        ApplyPendingSpawn(scene.name);
        BindCamera();
    }

    private void ApplyPendingSpawn(string sceneName)
    {
        if (!SceneTravelState.TryConsumeSpawn(sceneName, out string spawnPointId))
        {
            return;
        }

        SceneSpawnPoint spawnPoint = SceneSpawnPoint.Find(spawnPointId);
        if (spawnPoint == null)
        {
            return;
        }

        PlayerMove2D playerMove = GetComponent<PlayerMove2D>();
        if (playerMove != null)
        {
            playerMove.ForceIdleMotion();
        }

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
        }

        transform.position = spawnPoint.transform.position;
    }

    private void BindCamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        CameraFollow2D follow = camera.GetComponent<CameraFollow2D>();
        if (follow != null)
        {
            follow.SetTarget(transform, true);
        }
    }
}
