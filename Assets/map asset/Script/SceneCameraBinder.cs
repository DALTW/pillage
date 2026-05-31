using UnityEngine;

public class SceneCameraBinder : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    private void Start()
    {
        BindNow();
    }

    public void BindNow()
    {
        Camera cameraToBind = targetCamera != null ? targetCamera : Camera.main;
        if (cameraToBind == null)
        {
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        CameraFollow2D follow = cameraToBind.GetComponent<CameraFollow2D>();
        if (follow != null)
        {
            follow.SetTarget(player.transform, true);
        }
    }
}
