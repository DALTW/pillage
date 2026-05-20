using System.Collections.Generic;
using UnityEngine;

public class PlayerMove2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float interactRange = 1.2f;
    [SerializeField] private LayerMask interactableLayers = ~0;

    private readonly List<IInteractable> nearbyInteractables = new List<IInteractable>();
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(x, y).normalized;

        if (animator != null)
        {
            animator.SetBool("IsMoving", moveInput.sqrMagnitude > 0f);
            animator.SetFloat("MoveX", x);
            animator.SetFloat("MoveY", y);
        }

        if (x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (x < 0)
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
        rb.linearVelocity = moveInput * moveSpeed;
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

        for (int i = nearbyInteractables.Count - 1; i >= 0; i--)
        {
            IInteractable interactable = nearbyInteractables[i];
            Component component = interactable as Component;

            if (component == null)
            {
                nearbyInteractables.RemoveAt(i);
                continue;
            }

            float distance = Vector2.Distance(transform.position, component.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = interactable;
            }
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange, interactableLayers);

        foreach (Collider2D hit in hits)
        {
            IInteractable interactable = GetInteractableFromCollider(hit);
            if (interactable == null)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, hit.ClosestPoint(transform.position));

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
        nearbyInteractables.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
