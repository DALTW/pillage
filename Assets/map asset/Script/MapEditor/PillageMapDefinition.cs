using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pillage/Map Definition", fileName = "PillageMapDefinition")]
public class PillageMapDefinition : ScriptableObject
{
    [SerializeField] private List<MapRegionDefinition> regions = new List<MapRegionDefinition>();
    [SerializeField] private List<MapObjectPlacement> objects = new List<MapObjectPlacement>();

    private Dictionary<string, MapObjectPlacement> objectLookup;

    public List<MapRegionDefinition> Regions => regions;
    public List<MapObjectPlacement> Objects => objects;

    public bool TryGetObject(string id, out MapObjectPlacement placement)
    {
        EnsureLookup();
        return objectLookup.TryGetValue(id, out placement) && placement != null;
    }

    public MapObjectPlacement GetOrAddObject(string id, MapObjectType type)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Map object id is required.", nameof(id));
        }

        EnsureLookup();

        if (objectLookup.TryGetValue(id, out MapObjectPlacement existing) && existing != null)
        {
            return existing;
        }

        MapObjectPlacement placement = new MapObjectPlacement(id, type);
        objects.Add(placement);
        objectLookup[id] = placement;
        return placement;
    }

    public void RebuildLookup()
    {
        objectLookup = null;
        EnsureLookup();
    }

    private void EnsureLookup()
    {
        if (objectLookup != null)
        {
            return;
        }

        objectLookup = new Dictionary<string, MapObjectPlacement>();

        for (int i = 0; i < objects.Count; i++)
        {
            MapObjectPlacement placement = objects[i];
            if (placement == null || string.IsNullOrWhiteSpace(placement.Id))
            {
                continue;
            }

            objectLookup[placement.Id] = placement;
        }
    }

    private void OnValidate()
    {
        RebuildLookup();
    }
}

[Serializable]
public class MapRegionDefinition
{
    [SerializeField] private string id = "Village";
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector2 size = new Vector2(70f, 48f);

    public string Id
    {
        get => id;
        set => id = value;
    }

    public Vector3 Offset
    {
        get => offset;
        set => offset = value;
    }

    public Vector2 Size
    {
        get => size;
        set => size = value;
    }
}

[Serializable]
public class MapObjectPlacement
{
    [SerializeField] private string id;
    [SerializeField] private MapObjectType type;
    [SerializeField] private string region = "Village";
    [SerializeField] private bool enabled = true;
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 scale = Vector3.one;
    [SerializeField] private int sortingOrder;
    [SerializeField] private float promptDistance;
    [SerializeField] private int promptSortingOrder;
    [SerializeField] private Vector2 colliderOffset;
    [SerializeField] private Vector2 colliderSize = Vector2.one;
    [SerializeField] private float colliderRadius;
    [SerializeField] private float lineWidthMultiplier = 1f;
    [SerializeField] private Color lineColor = new Color32(35, 32, 28, 255);
    [SerializeField] private float revealDistance;
    [SerializeField] private List<Vector3> revealTriggerPositions = new List<Vector3>();
    [SerializeField] private string resourcePath;
    [SerializeField] private Sprite sprite;
    [SerializeField] private bool addGrassSway;
    [SerializeField] private float groundStrokeWidth;
    [SerializeField] private bool containsReward;
    [SerializeField] private int routeIndex = -1;
    [SerializeField] private int objectIndex = -1;

    public MapObjectPlacement()
    {
    }

    public MapObjectPlacement(string id, MapObjectType type)
    {
        this.id = id;
        this.type = type;
    }

    public string Id
    {
        get => id;
        set => id = value;
    }

    public MapObjectType Type
    {
        get => type;
        set => type = value;
    }

    public string Region
    {
        get => region;
        set => region = value;
    }

    public bool Enabled
    {
        get => enabled;
        set => enabled = value;
    }

    public Vector3 Position
    {
        get => position;
        set => position = value;
    }

    public Vector3 Scale
    {
        get => scale;
        set => scale = value;
    }

    public int SortingOrder
    {
        get => sortingOrder;
        set => sortingOrder = value;
    }

    public float PromptDistance
    {
        get => promptDistance;
        set => promptDistance = value;
    }

    public int PromptSortingOrder
    {
        get => promptSortingOrder;
        set => promptSortingOrder = value;
    }

    public Vector2 ColliderOffset
    {
        get => colliderOffset;
        set => colliderOffset = value;
    }

    public Vector2 ColliderSize
    {
        get => colliderSize;
        set => colliderSize = value;
    }

    public float ColliderRadius
    {
        get => colliderRadius;
        set => colliderRadius = value;
    }

    public float LineWidthMultiplier
    {
        get => lineWidthMultiplier;
        set => lineWidthMultiplier = value;
    }

    public Color LineColor
    {
        get => lineColor;
        set => lineColor = value;
    }

    public float RevealDistance
    {
        get => revealDistance;
        set => revealDistance = value;
    }

    public List<Vector3> RevealTriggerPositions => revealTriggerPositions;

    public string ResourcePath
    {
        get => resourcePath;
        set => resourcePath = value;
    }

    public Sprite Sprite
    {
        get => sprite;
        set => sprite = value;
    }

    public bool AddGrassSway
    {
        get => addGrassSway;
        set => addGrassSway = value;
    }

    public float GroundStrokeWidth
    {
        get => groundStrokeWidth;
        set => groundStrokeWidth = value;
    }

    public bool ContainsReward
    {
        get => containsReward;
        set => containsReward = value;
    }

    public int RouteIndex
    {
        get => routeIndex;
        set => routeIndex = value;
    }

    public int ObjectIndex
    {
        get => objectIndex;
        set => objectIndex = value;
    }
}

public enum MapObjectType
{
    Unknown,
    Background,
    House,
    Door,
    NeighborHouse,
    Mailbox,
    QuestNpc,
    ForestEntrance,
    Pond,
    FishingRod,
    Path,
    Fence,
    BoundaryCollider,
    PineTree,
    Grass,
    Rock,
    MoleHole,
    Bird,
    FallenBird,
    Well,
    Squirrel,
    HungrySquirrel,
    Bush,
    LeafPile,
    River,
    RootBridge,
    IslandDoor,
    BigTree,
    LetterFragment,
    GroundStroke,
    SpriteProp,
    LineDrawing
}
