using UnityEngine;

public class OutsideMapRoot : MonoBehaviour
{
    [SerializeField] private GameObject contentRoot;
    [SerializeField] private GameObject propRoot;
    [SerializeField] private Transform boundaryRoot;
    [SerializeField] private SketchOutsideTransition transition;

    public GameObject ContentRoot => contentRoot;
    public GameObject PropRoot => propRoot;
    public Transform BoundaryRoot => boundaryRoot;
    public SketchOutsideTransition Transition => transition;

    public void Configure(
        GameObject mapContentRoot,
        GameObject mapPropRoot,
        Transform mapBoundaryRoot,
        SketchOutsideTransition mapTransition)
    {
        contentRoot = mapContentRoot;
        propRoot = mapPropRoot;
        boundaryRoot = mapBoundaryRoot;
        transition = mapTransition;
    }
}
