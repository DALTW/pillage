using UnityEngine;

public enum PencilFragmentSource
{
    Tree,
    Fishing
}

public static class GameProgress
{
    public static bool HasCheckedSketchbook { get; private set; }
    public static bool HasPlayedSketchbookDrawing { get; private set; }
    public static bool HasDroppedTreePencilFragment { get; private set; }
    public static bool HasCollectedTreePencilFragment { get; private set; }
    public static bool HasDroppedFishingPencilFragment { get; private set; }
    public static bool HasCollectedFishingPencilFragment { get; private set; }

    public static int CollectedPencilFragmentCount
    {
        get
        {
            int count = 0;
            count += HasCollectedTreePencilFragment ? 1 : 0;
            count += HasCollectedFishingPencilFragment ? 1 : 0;
            return count;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnSubsystemRegistration()
    {
        Reset();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetBeforeSceneLoad()
    {
        Reset();
    }

    private static void Reset()
    {
        HasCheckedSketchbook = false;
        HasPlayedSketchbookDrawing = false;
        HasDroppedTreePencilFragment = false;
        HasCollectedTreePencilFragment = false;
        HasDroppedFishingPencilFragment = false;
        HasCollectedFishingPencilFragment = false;
    }

    public static void CheckSketchbook()
    {
        HasCheckedSketchbook = true;
    }

    public static void PlaySketchbookDrawing()
    {
        HasPlayedSketchbookDrawing = true;
    }

    public static bool CanDropPencilFragmentFrom(PencilFragmentSource source)
    {
        return !HasDroppedPencilFragmentFrom(source) && !HasCollectedPencilFragmentFrom(source);
    }

    public static bool HasDroppedPencilFragmentFrom(PencilFragmentSource source)
    {
        switch (source)
        {
            case PencilFragmentSource.Tree:
                return HasDroppedTreePencilFragment;
            case PencilFragmentSource.Fishing:
                return HasDroppedFishingPencilFragment;
            default:
                return false;
        }
    }

    public static bool HasCollectedPencilFragmentFrom(PencilFragmentSource source)
    {
        switch (source)
        {
            case PencilFragmentSource.Tree:
                return HasCollectedTreePencilFragment;
            case PencilFragmentSource.Fishing:
                return HasCollectedFishingPencilFragment;
            default:
                return false;
        }
    }

    public static void DropPencilFragmentFrom(PencilFragmentSource source)
    {
        switch (source)
        {
            case PencilFragmentSource.Tree:
                HasDroppedTreePencilFragment = true;
                break;
            case PencilFragmentSource.Fishing:
                HasDroppedFishingPencilFragment = true;
                break;
        }
    }

    public static void CollectPencilFragmentFrom(PencilFragmentSource source)
    {
        DropPencilFragmentFrom(source);

        switch (source)
        {
            case PencilFragmentSource.Tree:
                HasCollectedTreePencilFragment = true;
                break;
            case PencilFragmentSource.Fishing:
                HasCollectedFishingPencilFragment = true;
                break;
        }
    }
}
