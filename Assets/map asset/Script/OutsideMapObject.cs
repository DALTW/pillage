using UnityEngine;

public enum OutsideMapObjectRuntimeState
{
    Reveal,
    AlwaysVisible,
    HiddenUntilRegionUnlocked
}

public class OutsideMapObject : MonoBehaviour
{
    [SerializeField] private string id;
    [SerializeField] private MapObjectType type;
    [SerializeField] private string region = "Village";
    [SerializeField] private bool defaultVisible = true;
    [SerializeField] private int sortingOrder;
    [SerializeField] private float promptDistance;
    [SerializeField] private float revealDistance = 5.8f;
    [SerializeField] private OutsideMapObjectRuntimeState runtimeState = OutsideMapObjectRuntimeState.Reveal;

    public string Id => id;
    public MapObjectType Type => type;
    public string Region => region;
    public bool DefaultVisible => defaultVisible;
    public int SortingOrder => sortingOrder;
    public float PromptDistance => promptDistance;
    public float RevealDistance => revealDistance;
    public OutsideMapObjectRuntimeState RuntimeState => runtimeState;

    public void Configure(
        string objectId,
        MapObjectType objectType,
        string objectRegion,
        bool visibleByDefault,
        int objectSortingOrder,
        float objectPromptDistance,
        float objectRevealDistance,
        OutsideMapObjectRuntimeState objectRuntimeState)
    {
        id = objectId;
        type = objectType;
        region = objectRegion;
        defaultVisible = visibleByDefault;
        sortingOrder = objectSortingOrder;
        promptDistance = objectPromptDistance;
        revealDistance = objectRevealDistance;
        runtimeState = objectRuntimeState;
    }
}
