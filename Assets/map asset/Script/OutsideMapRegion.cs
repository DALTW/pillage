using UnityEngine;

public enum OutsideMapActivationCondition
{
    Always,
    ForestSketch,
    DeepForestSketch,
    FourthForestSketch,
    VillageGreenColored,
    BrownDetailsColored,
    WaterBlueColored,
    RootBridgeBuilt
}

public class OutsideMapRegion : MonoBehaviour
{
    [SerializeField] private string id = "Village";
    [SerializeField] private OutsideMapActivationCondition activationCondition = OutsideMapActivationCondition.Always;

    public string Id => id;
    public OutsideMapActivationCondition ActivationCondition => activationCondition;

    public void Configure(string regionId, OutsideMapActivationCondition condition)
    {
        id = regionId;
        activationCondition = condition;
    }

    public bool ShouldBeActive()
    {
        return OutsideMapProgressConditions.IsMet(activationCondition);
    }
}

public static class OutsideMapProgressConditions
{
    public static bool IsMet(OutsideMapActivationCondition condition)
    {
        switch (condition)
        {
            case OutsideMapActivationCondition.ForestSketch:
                return GameProgress.HasDrawnForestSketch;
            case OutsideMapActivationCondition.DeepForestSketch:
                return GameProgress.HasDrawnDeepForestSketch;
            case OutsideMapActivationCondition.FourthForestSketch:
                return GameProgress.HasDrawnFourthForestSketch;
            case OutsideMapActivationCondition.VillageGreenColored:
                return GameProgress.HasColoredVillageGreen;
            case OutsideMapActivationCondition.BrownDetailsColored:
                return GameProgress.HasColoredBrownDetails;
            case OutsideMapActivationCondition.WaterBlueColored:
                return GameProgress.HasColoredWaterBlue;
            case OutsideMapActivationCondition.RootBridgeBuilt:
                return GameProgress.HasBuiltFourthForestRootBridge;
            default:
                return true;
        }
    }
}
