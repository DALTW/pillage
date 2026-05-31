using UnityEngine;

public class OutsideMapRevealTarget : MonoBehaviour
{
    [SerializeField] private float revealDistance = 5.8f;
    [SerializeField] private Vector3[] triggerPositions;
    [SerializeField] private SketchWorldLineDrawing lineDrawing;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Behaviour enableWhenRevealed;

    public float RevealDistance => revealDistance;
    public Vector3[] TriggerPositions => triggerPositions;
    public SketchWorldLineDrawing LineDrawing => lineDrawing;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public Behaviour EnableWhenRevealed => enableWhenRevealed;

    public void ConfigureLine(float distance, Vector3[] triggers, SketchWorldLineDrawing drawing, Behaviour revealedBehaviour)
    {
        revealDistance = distance;
        triggerPositions = triggers;
        lineDrawing = drawing;
        spriteRenderer = null;
        enableWhenRevealed = revealedBehaviour;
    }

    public void ConfigureSprite(float distance, Vector3[] triggers, SpriteRenderer renderer, Behaviour revealedBehaviour)
    {
        revealDistance = distance;
        triggerPositions = triggers;
        lineDrawing = null;
        spriteRenderer = renderer;
        enableWhenRevealed = revealedBehaviour;
    }
}
