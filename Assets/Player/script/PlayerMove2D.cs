using System.Collections.Generic;
using UnityEngine;

public class PlayerMove2D : MonoBehaviour
{
    private const string IdleStateName = "Player2_Idle";
    private const string RunUpStateName = "Player2_RunUp";
    private const string RunDownStateName = "Player2_RunDown";
    private const string RunRightStateName = "Player2_RunRight";
    private const string RunLeftStateName = "Player2_RunLeft";

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float interactRange = 1.2f;
    [SerializeField] private LayerMask interactableLayers = ~0;

    private readonly List<IInteractable> nearbyInteractables = new List<IInteractable>();
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 moveInput;
    private int currentAnimationHash;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(x, y).normalized;

        if (animator != null && animator.isActiveAndEnabled)
        {
            animator.SetBool("IsMoving", moveInput.sqrMagnitude > 0f);
            animator.SetFloat("MoveX", x);
            animator.SetFloat("MoveY", y);
        }

        bool usingDirectionalAnimator = UpdateDirectionalAnimation(x, y);

        if (spriteRenderer != null && usingDirectionalAnimator)
        {
            spriteRenderer.flipX = false;
        }
        else if (spriteRenderer != null && x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (spriteRenderer != null && x < 0)
        {
            spriteRenderer.flipX = true;
        }

        if (Input.GetKeyDown(interactKey))
        {
            InteractWithNearbyTarget();
        }
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;
    }

    public void ForceIdleMotion()
    {
        moveInput = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (animator != null && animator.isActiveAndEnabled)
        {
            animator.SetBool("IsMoving", false);
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", 0f);
            currentAnimationHash = 0;
            UpdateDirectionalAnimation(0f, 0f);
            animator.Update(0f);
        }
    }

    private void InteractWithNearbyTarget()
    {
        IInteractable target = FindNearbyInteractable();

        if (target == null)
        {
            Debug.Log("No interactable target in range.");
            return;
        }

        target.Interact(gameObject);
    }

    private IInteractable FindNearbyInteractable()
    {
        IInteractable closestTarget = null;
        float closestDistance = float.MaxValue;
        Vector2 interactionCenter = GetInteractionCenter();

        for (int i = nearbyInteractables.Count - 1; i >= 0; i--)
        {
            IInteractable interactable = nearbyInteractables[i];
            Component component = interactable as Component;

            if (component == null)
            {
                nearbyInteractables.RemoveAt(i);
                continue;
            }

            float distance = Vector2.Distance(interactionCenter, component.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = interactable;
            }
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(interactionCenter, interactRange, interactableLayers);

        foreach (Collider2D hit in hits)
        {
            IInteractable interactable = GetInteractableFromCollider(hit);
            if (interactable == null)
            {
                continue;
            }

            float distance = Vector2.Distance(interactionCenter, hit.ClosestPoint(interactionCenter));

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = interactable;
            }
        }

        return closestTarget;
    }

    private IInteractable GetInteractableFromCollider(Collider2D other)
    {
        MonoBehaviour[] behaviours = other.GetComponentsInParent<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            IInteractable interactable = behaviour as IInteractable;

            if (interactable != null)
            {
                return interactable;
            }
        }

        return null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = GetInteractableFromCollider(other);

        if (interactable != null && !nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = GetInteractableFromCollider(other);

        if (interactable != null)
        {
            nearbyInteractables.Remove(interactable);
        }
    }

    private void OnDisable()
    {
        ForceIdleMotion();
        nearbyInteractables.Clear();
    }

    private Vector2 GetInteractionCenter()
    {
        Collider2D centerCollider = playerCollider;

        if (centerCollider == null)
        {
            centerCollider = GetComponent<Collider2D>();
        }

        return centerCollider != null ? centerCollider.bounds.center : transform.position;
    }

    private bool UpdateDirectionalAnimation(float x, float y)
    {
        if (animator == null || !animator.isActiveAndEnabled)
        {
            return false;
        }

        string stateName = IdleStateName;

        if (moveInput.sqrMagnitude > 0f)
        {
            if (Mathf.Abs(x) >= Mathf.Abs(y))
            {
                stateName = x >= 0f ? RunRightStateName : RunLeftStateName;
            }
            else
            {
                stateName = y >= 0f ? RunUpStateName : RunDownStateName;
            }
        }

        return PlayAnimatorStateIfAvailable(stateName);
    }

    private bool PlayAnimatorStateIfAvailable(string stateName)
    {
        int stateHash = Animator.StringToHash(stateName);
        int fullPathHash = Animator.StringToHash("Base Layer." + stateName);
        int playableHash = 0;

        if (animator.HasState(0, stateHash))
        {
            playableHash = stateHash;
        }
        else if (animator.HasState(0, fullPathHash))
        {
            playableHash = fullPathHash;
        }

        if (playableHash == 0)
        {
            return false;
        }

        if (currentAnimationHash != playableHash)
        {
            animator.Play(playableHash);
            currentAnimationHash = playableHash;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetInteractionCenter(), interactRange);
    }
}
