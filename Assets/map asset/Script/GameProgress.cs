using UnityEngine;

public enum PencilFragmentSource
{
    Tree,
    Fishing,
    Npc
}

public enum LetterFragmentSource
{
    Bottle,
    TreeChase,
    GrassSearch,
    WindTrace,
    GrassPatch,
    PondEdge
}

public static class GameProgress
{
    public const int RequiredLetterFragmentCount = 3;
    public const int RequiredFishingRodInsectCount = 6;

    public static bool HasCheckedSketchbook { get; private set; }
    public static bool HasPlayedSketchbookDrawing { get; private set; }
    public static bool HasDroppedTreePencilFragment { get; private set; }
    public static bool HasCollectedTreePencilFragment { get; private set; }
    public static bool HasDroppedFishingPencilFragment { get; private set; }
    public static bool HasCollectedFishingPencilFragment { get; private set; }
    public static bool HasDroppedNpcPencilFragment { get; private set; }
    public static bool HasCollectedNpcPencilFragment { get; private set; }
    public static bool HasMetQuestNpc { get; private set; }
    public static bool HasCollectedBottleLetterFragment { get; private set; }
    public static bool HasStartedLetterTreeChase { get; private set; }
    public static int ActiveLetterTreeIndex { get; private set; } = -1;
    public static bool HasCollectedTreeLetterFragment { get; private set; }
    public static bool HasRevealedGrassLetterFragment { get; private set; }
    public static bool HasCollectedGrassLetterFragment { get; private set; }
    public static bool HasDeliveredCompletedLetter { get; private set; }
    public static int FishingRodInsectCount { get; private set; }
    public static int UsedPencilFragmentCount { get; private set; }
    public static int UsedLetterFragmentCount { get; private set; }
    public static bool HasRevealedGreenCrayon { get; private set; }
    public static bool HasCollectedGreenCrayon { get; private set; }
    public static bool HasUsedGreenCrayon { get; private set; }
    public static bool HasExtendedSketchbookPencil { get; private set; }
    public static bool HasDrawnForestSketch { get; private set; }
    public static bool HasColoredVillageGreen { get; private set; }
    public static bool HasColoredForestGreen { get; private set; }

    public static bool HasCompletedLetter => AvailableLetterFragmentCount >= RequiredLetterFragmentCount;
    public static bool CanCatchLetterBottle => HasMetQuestNpc && !HasCollectedBottleLetterFragment;
    public static bool CanFindFinalGrassLetterFragment => HasCollectedTreeLetterFragment && !HasCollectedGrassLetterFragment;
    public static bool HasUncoloredGreenTarget => !HasColoredVillageGreen || (HasDrawnForestSketch && !HasColoredForestGreen);
    public static bool CanAddFishingRodInsect => HasUncoloredGreenTarget && FishingRodInsectCount < RequiredFishingRodInsectCount && !HasRevealedGreenCrayon && !HasCollectedGreenCrayon;
    public static bool CanRevealGreenCrayon => HasUncoloredGreenTarget && FishingRodInsectCount >= RequiredFishingRodInsectCount && !HasRevealedGreenCrayon && !HasCollectedGreenCrayon;
    public static bool CanExtendPencilAtSketchbook => AvailablePencilFragmentCount >= 3 && !HasExtendedSketchbookPencil;
    public static bool CanColorVillageAtSketchbook => HasCollectedGreenCrayon && HasUncoloredGreenTarget;

    public static int CollectedPencilFragmentCount
    {
        get
        {
            int count = 0;
            count += HasCollectedTreePencilFragment ? 1 : 0;
            count += HasCollectedFishingPencilFragment ? 1 : 0;
            count += HasCollectedNpcPencilFragment ? 1 : 0;
            return count;
        }
    }

    public static int AvailablePencilFragmentCount => Mathf.Max(0, CollectedPencilFragmentCount - UsedPencilFragmentCount);

    public static int CollectedLetterFragmentCount
    {
        get
        {
            int count = 0;
            count += HasCollectedBottleLetterFragment ? 1 : 0;
            count += HasCollectedTreeLetterFragment ? 1 : 0;
            count += HasCollectedGrassLetterFragment ? 1 : 0;
            return count;
        }
    }

    public static int AvailableLetterFragmentCount => Mathf.Max(0, CollectedLetterFragmentCount - UsedLetterFragmentCount);

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
        HasDroppedNpcPencilFragment = false;
        HasCollectedNpcPencilFragment = false;
        HasMetQuestNpc = false;
        HasCollectedBottleLetterFragment = false;
        HasStartedLetterTreeChase = false;
        ActiveLetterTreeIndex = -1;
        HasCollectedTreeLetterFragment = false;
        HasRevealedGrassLetterFragment = false;
        HasCollectedGrassLetterFragment = false;
        HasDeliveredCompletedLetter = false;
        FishingRodInsectCount = 0;
        UsedPencilFragmentCount = 0;
        UsedLetterFragmentCount = 0;
        HasRevealedGreenCrayon = false;
        HasCollectedGreenCrayon = false;
        HasUsedGreenCrayon = false;
        HasExtendedSketchbookPencil = false;
        HasDrawnForestSketch = false;
        HasColoredVillageGreen = false;
        HasColoredForestGreen = false;
    }

    public static void CheckSketchbook()
    {
        HasCheckedSketchbook = true;
    }

    public static void PlaySketchbookDrawing()
    {
        HasPlayedSketchbookDrawing = true;
    }

    public static void MeetQuestNpc()
    {
        HasMetQuestNpc = true;
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
            case PencilFragmentSource.Npc:
                return HasDroppedNpcPencilFragment;
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
            case PencilFragmentSource.Npc:
                return HasCollectedNpcPencilFragment;
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
            case PencilFragmentSource.Npc:
                HasDroppedNpcPencilFragment = true;
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
            case PencilFragmentSource.Npc:
                HasCollectedNpcPencilFragment = true;
                break;
        }
    }

    public static bool HasCollectedLetterFragmentFrom(LetterFragmentSource source)
    {
        switch (source)
        {
            case LetterFragmentSource.Bottle:
            case LetterFragmentSource.WindTrace:
                return HasCollectedBottleLetterFragment;
            case LetterFragmentSource.TreeChase:
            case LetterFragmentSource.PondEdge:
                return HasCollectedTreeLetterFragment;
            case LetterFragmentSource.GrassSearch:
            case LetterFragmentSource.GrassPatch:
                return HasCollectedGrassLetterFragment;
            default:
                return false;
        }
    }

    public static void CollectLetterFragmentFrom(LetterFragmentSource source)
    {
        switch (source)
        {
            case LetterFragmentSource.Bottle:
            case LetterFragmentSource.WindTrace:
                HasCollectedBottleLetterFragment = true;
                break;
            case LetterFragmentSource.TreeChase:
            case LetterFragmentSource.PondEdge:
                HasCollectedTreeLetterFragment = true;
                break;
            case LetterFragmentSource.GrassSearch:
            case LetterFragmentSource.GrassPatch:
                HasRevealedGrassLetterFragment = true;
                HasCollectedGrassLetterFragment = true;
                break;
        }
    }

    public static void RevealGrassLetterFragment()
    {
        if (HasCollectedGrassLetterFragment)
        {
            return;
        }

        HasRevealedGrassLetterFragment = true;
    }

    public static void StartLetterTreeChase()
    {
        if (!HasCollectedBottleLetterFragment || HasCollectedTreeLetterFragment)
        {
            return;
        }

        HasStartedLetterTreeChase = true;
        ActiveLetterTreeIndex = 0;
    }

    public static void MoveLetterToTree(int treeIndex)
    {
        if (!HasStartedLetterTreeChase || HasCollectedTreeLetterFragment)
        {
            return;
        }

        ActiveLetterTreeIndex = Mathf.Max(0, treeIndex);
    }

    public static bool IsLetterWaitingOnTree(int treeIndex)
    {
        return HasStartedLetterTreeChase
            && !HasCollectedTreeLetterFragment
            && ActiveLetterTreeIndex == treeIndex;
    }

    public static void CompleteLetterTreeChase()
    {
        if (!HasStartedLetterTreeChase || HasCollectedTreeLetterFragment)
        {
            return;
        }

        CollectLetterFragmentFrom(LetterFragmentSource.TreeChase);
        ActiveLetterTreeIndex = -1;
    }

    public static void DeliverCompletedLetter()
    {
        if (HasCompletedLetter)
        {
            HasDeliveredCompletedLetter = true;
            UsedLetterFragmentCount = Mathf.Min(
                CollectedLetterFragmentCount,
                UsedLetterFragmentCount + RequiredLetterFragmentCount);
        }
    }

    public static bool AddFishingRodInsect()
    {
        if (!CanAddFishingRodInsect)
        {
            return false;
        }

        FishingRodInsectCount = Mathf.Min(RequiredFishingRodInsectCount, FishingRodInsectCount + 1);
        return true;
    }

    public static void RevealGreenCrayon()
    {
        if (!CanRevealGreenCrayon)
        {
            return;
        }

        HasRevealedGreenCrayon = true;
    }

    public static void CollectGreenCrayon()
    {
        HasRevealedGreenCrayon = true;
        HasCollectedGreenCrayon = true;
    }

    public static void CompleteSketchbookForestDrawing()
    {
        if (AvailablePencilFragmentCount < 3)
        {
            return;
        }

        UsedPencilFragmentCount = Mathf.Min(CollectedPencilFragmentCount, UsedPencilFragmentCount + 3);
        HasExtendedSketchbookPencil = true;
        HasDrawnForestSketch = true;
        HasColoredForestGreen = HasColoredForestGreen || HasColoredVillageGreen;
    }

    public static void ColorVillageGreen()
    {
        if (!HasCollectedGreenCrayon)
        {
            return;
        }

        HasCollectedGreenCrayon = false;
        HasRevealedGreenCrayon = false;
        FishingRodInsectCount = 0;
        HasUsedGreenCrayon = true;
        HasColoredVillageGreen = true;
        if (HasDrawnForestSketch)
        {
            HasColoredForestGreen = true;
        }
    }
}
