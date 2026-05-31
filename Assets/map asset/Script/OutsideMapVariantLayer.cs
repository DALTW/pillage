using UnityEngine;

public enum OutsideMapVariantType
{
    Green,
    Brown,
    Blue,
    Reward,
    Bridge,
    QuestState
}

public class OutsideMapVariantLayer : MonoBehaviour
{
    [SerializeField] private OutsideMapVariantType variantType;
    [SerializeField] private OutsideMapActivationCondition activationCondition = OutsideMapActivationCondition.Always;
    [SerializeField] private bool revealWithParent = true;

    public OutsideMapVariantType VariantType => variantType;
    public OutsideMapActivationCondition ActivationCondition => activationCondition;
    public bool RevealWithParent => revealWithParent;

    public void Configure(OutsideMapVariantType type, OutsideMapActivationCondition condition, bool shouldRevealWithParent)
    {
        variantType = type;
        activationCondition = condition;
        revealWithParent = shouldRevealWithParent;
    }

    public bool ShouldBeActive()
    {
        return OutsideMapProgressConditions.IsMet(activationCondition);
    }
}
