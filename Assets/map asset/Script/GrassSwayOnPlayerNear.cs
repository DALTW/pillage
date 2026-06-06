using UnityEngine;

public class GrassSwayOnPlayerNear : MonoBehaviour, IInteractable
{
    [SerializeField] private float reactionRadius = 1.15f;
    [SerializeField] private float maxTiltAngle = 26f;
    [SerializeField] private float swaySpeed = 13f;
    [SerializeField] private float recoverSpeed = 8f;
    [SerializeField] private float squashAmount = 0.16f;
    [SerializeField, Range(0f, 1f)] private float insectSpawnChance = 0.25f;

    private const float LetterPromptDistance = 1.45f;
    private const int LetterPromptSortingOrder = 72;

    private Transform player;
    private Rigidbody2D playerRigidbody;
    private GameObject letterPromptObject;
    private CircleCollider2D letterInteractionCollider;
    private Vector3 baseScale;
    private Quaternion baseRotation;
    private float swayVelocity;
    private float currentTilt;
    private float currentSquash;
    private bool hasTriedInsectSpawn;
    private bool suppressInsectSpawn;
    private bool containsLetterFragment;
    private bool hasReleasedLetterFragment;

    public void MarkReservedForLetterFragment()
    {
        suppressInsectSpawn = true;
    }

    public void SuppressRewardSpawns()
    {
        suppressInsectSpawn = true;
        containsLetterFragment = false;
        hasReleasedLetterFragment = false;
        SetLetterInteractionAvailable(false);
    }

    public void MarkContainsLetterFragment()
    {
        suppressInsectSpawn = true;
        containsLetterFragment = true;
        hasReleasedLetterFragment = false;
        EnsureLetterInteraction();
        SetLetterInteractionAvailable(isActiveAndEnabled);
    }

    public void Interact(GameObject interactor)
    {
        if (!containsLetterFragment || hasReleasedLetterFragment || interactor == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, interactor.transform.position) > LetterPromptDistance)
        {
            return;
        }

        TryReleaseContainedLetterFragment();
    }

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

        SetLetterInteractionAvailable(containsLetterFragment && !hasReleasedLetterFragment);
    }

    private void OnDisable()
    {
        SetLetterInteractionAvailable(false);
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

                if (proximity >= 0.2f)
                {
                    if (containsLetterFragment)
                    {
                        TryReleaseContainedLetterFragment();
                    }
                    else if (!suppressInsectSpawn && IsPlayerMovingThroughGrass())
                    {
                        TrySpawnGlowingInsect();
                    }
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

        UpdateLetterPrompt();
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

    private void TryReleaseContainedLetterFragment()
    {
        if (hasReleasedLetterFragment)
        {
            return;
        }

        if (LetterQuestWorldDrop.TrySpawnFinalGrassLetterFragmentFrom(transform))
        {
            hasReleasedLetterFragment = true;
            containsLetterFragment = false;
            SetLetterInteractionAvailable(false);
        }
    }

    private void EnsureLetterInteraction()
    {
        if (letterInteractionCollider == null)
        {
            letterInteractionCollider = gameObject.AddComponent<CircleCollider2D>();
            letterInteractionCollider.isTrigger = true;
            letterInteractionCollider.radius = 0.78f;
            letterInteractionCollider.enabled = false;
        }

        if (letterPromptObject != null)
        {
            return;
        }

        letterPromptObject = new GameObject("GeneratedGrassLetterPrompt");
        letterPromptObject.transform.SetParent(transform, false);
        letterPromptObject.transform.localPosition = new Vector3(0f, 0.72f, 0f);

        TextMesh textMesh = letterPromptObject.AddComponent<TextMesh>();
        textMesh.text = "E";
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 64;
        textMesh.characterSize = 0.075f;
        textMesh.color = new Color32(35, 32, 28, 255);

        MeshRenderer meshRenderer = letterPromptObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = LetterPromptSortingOrder;
        }

        letterPromptObject.SetActive(false);
    }

    private void SetLetterInteractionAvailable(bool available)
    {
        if (available)
        {
            EnsureLetterInteraction();
        }

        if (letterInteractionCollider != null)
        {
            letterInteractionCollider.enabled = available;
        }

        if (!available && letterPromptObject != null)
        {
            letterPromptObject.SetActive(false);
        }
    }

    private void UpdateLetterPrompt()
    {
        if (!containsLetterFragment || hasReleasedLetterFragment || letterPromptObject == null)
        {
            if (letterPromptObject != null)
            {
                letterPromptObject.SetActive(false);
            }

            return;
        }

        bool shouldShowPrompt = player != null
            && Vector2.Distance(transform.position, player.position) <= LetterPromptDistance;
        letterPromptObject.SetActive(shouldShowPrompt);
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
