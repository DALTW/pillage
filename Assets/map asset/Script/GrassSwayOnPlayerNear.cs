using UnityEngine;

public class GrassSwayOnPlayerNear : MonoBehaviour
{
    [SerializeField] private float reactionRadius = 1.15f;
    [SerializeField] private float maxTiltAngle = 13f;
    [SerializeField] private float swaySpeed = 13f;
    [SerializeField] private float recoverSpeed = 8f;
    [SerializeField] private float squashAmount = 0.08f;
    [SerializeField, Range(0f, 1f)] private float insectSpawnChance = 0.1f;

    private Transform player;
    private Rigidbody2D playerRigidbody;
    private Vector3 baseScale;
    private Quaternion baseRotation;
    private float swayVelocity;
    private float currentTilt;
    private float currentSquash;
    private bool hasTriedInsectSpawn;

    private void Awake()
    {
        baseScale = transform.localScale;
        baseRotation = transform.localRotation;
    }

    private void OnEnable()
    {
        FindPlayer();
        currentTilt = 0f;
        currentSquash = 0f;
        transform.localRotation = baseRotation;
        transform.localScale = baseScale;
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
        }

        float targetTilt = 0f;
        float targetSquash = 0f;

        if (player != null)
        {
            Vector2 toGrass = transform.position - player.position;
            float distance = toGrass.magnitude;

            if (distance <= reactionRadius)
            {
                float proximity = 1f - Mathf.Clamp01(distance / reactionRadius);
                float side = Mathf.Abs(toGrass.x) > 0.04f ? Mathf.Sign(toGrass.x) : GetPlayerMoveSide();
                float flutter = Mathf.Sin(Time.time * swaySpeed + transform.position.x * 0.67f) * 0.28f;

                targetTilt = side * maxTiltAngle * proximity + flutter * maxTiltAngle * proximity;
                targetSquash = squashAmount * proximity;

                if (proximity >= 0.2f && IsPlayerMovingThroughGrass())
                {
                    TrySpawnGlowingInsect();
                }
            }
        }

        float damping = Mathf.Approximately(targetTilt, 0f) ? recoverSpeed : swaySpeed;
        currentTilt = Mathf.SmoothDampAngle(currentTilt, targetTilt, ref swayVelocity, 1f / damping);
        currentSquash = Mathf.MoveTowards(currentSquash, targetSquash, Time.deltaTime * damping * squashAmount);

        transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, currentTilt);
        transform.localScale = new Vector3(
            baseScale.x * (1f + currentSquash * 0.35f),
            baseScale.y * (1f - currentSquash),
            baseScale.z);
    }

    private void TrySpawnGlowingInsect()
    {
        if (hasTriedInsectSpawn)
        {
            return;
        }

        hasTriedInsectSpawn = true;

        if (Random.value > insectSpawnChance)
        {
            return;
        }

        Vector3 spawnOffset = new Vector3(Random.Range(-0.18f, 0.18f), Random.Range(0.2f, 0.45f), 0f);
        GlowingInsectInteract.Spawn(transform.position + spawnOffset, transform.parent);
    }

    private bool IsPlayerMovingThroughGrass()
    {
        return playerRigidbody != null && playerRigidbody.linearVelocity.sqrMagnitude > 0.05f;
    }

    private float GetPlayerMoveSide()
    {
        if (playerRigidbody != null && Mathf.Abs(playerRigidbody.linearVelocity.x) > 0.04f)
        {
            return Mathf.Sign(playerRigidbody.linearVelocity.x);
        }

        return transform.position.x >= player.position.x ? 1f : -1f;
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            player = null;
            playerRigidbody = null;
            return;
        }

        player = playerObject.transform;
        playerRigidbody = playerObject.GetComponent<Rigidbody2D>();
    }
}
