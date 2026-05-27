using UnityEngine;

public enum PencilFragmentSource
{
    Tree,
    Fishing,
    Npc,
    Bird,
    FallenBird,
    RockCrack,
    Squirrel,
    LeafPile,
    HungrySquirrel
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
    public const int RequiredMoleHoleCount = 5;

    private static readonly bool[] InteractedMoleHoles = new bool[RequiredMoleHoleCount];

    public static bool HasCheckedSketchbook { get; private set; }
    public static bool HasPlayedSketchbookDrawing { get; private set; }
    public static bool HasDroppedTreePencilFragment { get; private set; }
    public static bool HasCollectedTreePencilFragment { get; private set; }
    public static bool HasDroppedFishingPencilFragment { get; private set; }
    public static bool HasCollectedFishingPencilFragment { get; private set; }
    public static bool HasDroppedNpcPencilFragment { get; private set; }
    public static bool HasCollectedNpcPencilFragment { get; private set; }
    public static bool HasDroppedBirdPencilFragment { get; private set; }
    public static bool HasCollectedBirdPencilFragment { get; private set; }
    public static bool HasDroppedFallenBirdPencilFragment { get; private set; }
    public static bool HasCollectedFallenBirdPencilFragment { get; private set; }
    public static bool HasDroppedRockCrackPencilFragment { get; private set; }
    public static bool HasCollectedRockCrackPencilFragment { get; private set; }
    public static bool HasDroppedSquirrelPencilFragment { get; private set; }
    public static bool HasCollectedSquirrelPencilFragment { get; private set; }
    public static bool HasDroppedLeafPilePencilFragment { get; private set; }
    public static bool HasCollectedLeafPilePencilFragment { get; private set; }
    public static bool HasDroppedHungrySquirrelPencilFragment { get; private set; }
    public static bool HasCollectedHungrySquirrelPencilFragment { get; private set; }
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
    public static bool HasExtendedDeepForestPencil { get; private set; }
    public static bool HasDrawnDeepForestSketch { get; private set; }
    public static bool HasExtendedFourthForestPencil { get; private set; }
    public static bool HasDrawnFourthForestSketch { get; private set; }
    public static bool HasColoredVillageGreen { get; private set; }
    public static bool HasColoredForestGreen { get; private set; }
    public static bool HasColoredDeepForestGreen { get; private set; }
    public static bool HasColoredFourthForestGreen { get; private set; }
    public static bool HasMetForestBird { get; private set; }
    public static bool HasHelpedFallenBird { get; private set; }
    public static bool HasCollectedForestBranch { get; private set; }
    public static bool HasCollectedWellWaterBottle { get; private set; }
    public static bool HasCollectedBrownCrayon { get; private set; }
    public static bool HasUsedBrownCrayon { get; private set; }
    public static bool HasColoredBrownDetails { get; private set; }
    public static bool HasCollectedAcorn { get; private set; }
    public static bool HasCollectedMushroom { get; private set; }

    public static bool HasCompletedLetter => AvailableLetterFragmentCount >= RequiredLetterFragmentCount;
    public static bool CanCatchLetterBottle => HasMetQuestNpc && !HasCollectedBottleLetterFragment;
    public static bool CanFindFinalGrassLetterFragment => HasCollectedTreeLetterFragment && !HasCollectedGrassLetterFragment;
    public static bool HasUncoloredGreenTarget => !HasColoredVillageGreen || (HasDrawnForestSketch && !HasColoredForestGreen) || (HasDrawnDeepForestSketch && !HasColoredDeepForestGreen) || (HasDrawnFourthForestSketch && !HasColoredFourthForestGreen);
    public static bool CanAddFishingRodInsect => HasUncoloredGreenTarget && FishingRodInsectCount < RequiredFishingRodInsectCount && !HasRevealedGreenCrayon && !HasCollectedGreenCrayon;
    public static bool CanRevealGreenCrayon => HasUncoloredGreenTarget && FishingRodInsectCount >= RequiredFishingRodInsectCount && !HasRevealedGreenCrayon && !HasCollectedGreenCrayon;
    public static bool CanExtendPencilAtSketchbook => AvailablePencilFragmentCount >= 3 && !HasExtendedSketchbookPencil;
    public static bool CanExtendDeepForestAtSketchbook => HasDrawnForestSketch && AvailablePencilFragmentCount >= 3 && !HasExtendedDeepForestPencil;
    public static bool CanExtendFourthForestAtSketchbook => HasDrawnDeepForestSketch && AvailablePencilFragmentCount >= 3 && !HasExtendedFourthForestPencil;
    public static bool CanColorVillageAtSketchbook => HasCollectedGreenCrayon && HasUncoloredGreenTarget;
    public static bool CanColorBrownDetailsAtSketchbook => HasCollectedBrownCrayon && !HasColoredBrownDetails;
    public static bool HasCompletedMoleHoles => InteractedMoleHoleCount >= RequiredMoleHoleCount;

    public static int InteractedMoleHoleCount
    {
        get
        {
            int count = 0;

            for (int i = 0; i < InteractedMoleHoles.Length; i++)
            {
                count += InteractedMoleHoles[i] ? 1 : 0;
            }

            return count;
        }
    }

    public static int CollectedPencilFragmentCount
    {
        get
        {
            int count = 0;
            count += HasCollectedTreePencilFragment ? 1 : 0;
            count += HasCollectedFishingPencilFragment ? 1 : 0;
            count += HasCollectedNpcPencilFragment ? 1 : 0;
            count += HasCollectedBirdPencilFragment ? 1 : 0;
            count += HasCollectedFallenBirdPencilFragment ? 1 : 0;
            count += HasCollectedRockCrackPencilFragment ? 1 : 0;
            count += HasCollectedSquirrelPencilFragment ? 1 : 0;
            count += HasCollectedLeafPilePencilFragment ? 1 : 0;
            count += HasCollectedHungrySquirrelPencilFragment ? 1 : 0;
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
        HasDroppedBirdPencilFragment = false;
        HasCollectedBirdPencilFragment = false;
        HasDroppedFallenBirdPencilFragment = false;
        HasCollectedFallenBirdPencilFragment = false;
        HasDroppedRockCrackPencilFragment = false;
        HasCollectedRockCrackPencilFragment = false;
        HasDroppedSquirrelPencilFragment = false;
        HasCollectedSquirrelPencilFragment = false;
        HasDroppedLeafPilePencilFragment = false;
        HasCollectedLeafPilePencilFragment = false;
        HasDroppedHungrySquirrelPencilFragment = false;
        HasCollectedHungrySquirrelPencilFragment = false;
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
        HasExtendedDeepForestPencil = false;
        HasDrawnDeepForestSketch = false;
        HasExtendedFourthForestPencil = false;
        HasDrawnFourthForestSketch = false;
        HasColoredVillageGreen = false;
        HasColoredForestGreen = false;
        HasColoredDeepForestGreen = false;
        HasColoredFourthForestGreen = false;
        HasMetForestBird = false;
        HasHelpedFallenBird = false;
        HasCollectedForestBranch = false;
        HasCollectedWellWaterBottle = false;
        HasCollectedBrownCrayon = false;
        HasUsedBrownCrayon = false;
        HasColoredBrownDetails = false;
        HasCollectedAcorn = false;
        HasCollectedMushroom = false;

        for (int i = 0; i < InteractedMoleHoles.Length; i++)
        {
            InteractedMoleHoles[i] = false;
        }
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

    public static void MeetForestBird()
    {
        HasMetForestBird = true;
    }

    public static void HelpFallenBird()
    {
        HasHelpedFallenBird = true;
    }

    public static void CollectForestBranch()
    {
        HasCollectedForestBranch = true;
    }

    public static void CollectWellWaterBottle()
    {
        HasCollectedWellWaterBottle = true;
    }

    public static void CollectAcorn()
    {
        HasCollectedAcorn = true;
    }

    public static void CollectMushroom()
    {
        HasCollectedMushroom = true;
    }

    public static bool HasInteractedMoleHole(int holeIndex)
    {
        return holeIndex >= 0 && holeIndex < InteractedMoleHoles.Length && InteractedMoleHoles[holeIndex];
    }

    public static bool InteractMoleHole(int holeIndex)
    {
        if (holeIndex < 0 || holeIndex >= InteractedMoleHoles.Length || InteractedMoleHoles[holeIndex])
        {
            return false;
        }

        InteractedMoleHoles[holeIndex] = true;
        return true;
    }

    public static void CollectBrownCrayon()
    {
        HasCollectedBrownCrayon = true;
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
            case PencilFragmentSource.Bird:
                return HasDroppedBirdPencilFragment;
            case PencilFragmentSource.FallenBird:
                return HasDroppedFallenBirdPencilFragment;
            case PencilFragmentSource.RockCrack:
                return HasDroppedRockCrackPencilFragment;
            case PencilFragmentSource.Squirrel:
                return HasDroppedSquirrelPencilFragment;
            case PencilFragmentSource.LeafPile:
                return HasDroppedLeafPilePencilFragment;
            case PencilFragmentSource.HungrySquirrel:
                return HasDroppedHungrySquirrelPencilFragment;
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
            case PencilFragmentSource.Bird:
                return HasCollectedBirdPencilFragment;
            case PencilFragmentSource.FallenBird:
                return HasCollectedFallenBirdPencilFragment;
            case PencilFragmentSource.RockCrack:
                return HasCollectedRockCrackPencilFragment;
            case PencilFragmentSource.Squirrel:
                return HasCollectedSquirrelPencilFragment;
            case PencilFragmentSource.LeafPile:
                return HasCollectedLeafPilePencilFragment;
            case PencilFragmentSource.HungrySquirrel:
                return HasCollectedHungrySquirrelPencilFragment;
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
            case PencilFragmentSource.Bird:
                HasDroppedBirdPencilFragment = true;
                break;
            case PencilFragmentSource.FallenBird:
                HasDroppedFallenBirdPencilFragment = true;
                break;
            case PencilFragmentSource.RockCrack:
                HasDroppedRockCrackPencilFragment = true;
                break;
            case PencilFragmentSource.Squirrel:
                HasDroppedSquirrelPencilFragment = true;
                break;
            case PencilFragmentSource.LeafPile:
                HasDroppedLeafPilePencilFragment = true;
                break;
            case PencilFragmentSource.HungrySquirrel:
                HasDroppedHungrySquirrelPencilFragment = true;
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
            case PencilFragmentSource.Bird:
                HasCollectedBirdPencilFragment = true;
                break;
            case PencilFragmentSource.FallenBird:
                HasCollectedFallenBirdPencilFragment = true;
                break;
            case PencilFragmentSource.RockCrack:
                HasCollectedRockCrackPencilFragment = true;
                break;
            case PencilFragmentSource.Squirrel:
                HasCollectedSquirrelPencilFragment = true;
                break;
            case PencilFragmentSource.LeafPile:
                HasCollectedLeafPilePencilFragment = true;
                break;
            case PencilFragmentSource.HungrySquirrel:
                HasCollectedHungrySquirrelPencilFragment = true;
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

    public static void CompleteSketchbookDeepForestDrawing()
    {
        if (AvailablePencilFragmentCount < 3 || !HasDrawnForestSketch)
        {
            return;
        }

        UsedPencilFragmentCount = Mathf.Min(CollectedPencilFragmentCount, UsedPencilFragmentCount + 3);
        HasExtendedDeepForestPencil = true;
        HasDrawnDeepForestSketch = true;
        HasColoredDeepForestGreen = HasColoredDeepForestGreen || HasColoredVillageGreen;
    }

    public static void CompleteSketchbookFourthForestDrawing()
    {
        if (AvailablePencilFragmentCount < 3 || !HasDrawnDeepForestSketch)
        {
            return;
        }

        UsedPencilFragmentCount = Mathf.Min(CollectedPencilFragmentCount, UsedPencilFragmentCount + 3);
        HasExtendedFourthForestPencil = true;
        HasDrawnFourthForestSketch = true;
        HasColoredFourthForestGreen = HasColoredFourthForestGreen || HasColoredVillageGreen;
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
        if (HasDrawnDeepForestSketch)
        {
            HasColoredDeepForestGreen = true;
        }
        if (HasDrawnFourthForestSketch)
        {
            HasColoredFourthForestGreen = true;
        }
    }

    public static void ColorBrownDetails()
    {
        if (!CanColorBrownDetailsAtSketchbook)
        {
            return;
        }

        HasCollectedBrownCrayon = false;
        HasUsedBrownCrayon = true;
        HasColoredBrownDetails = true;
    }
}
