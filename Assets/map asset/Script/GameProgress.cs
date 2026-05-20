using UnityEngine;

public static class GameProgress
{
    public static bool HasCheckedSketchbook { get; private set; }
    public static bool HasPlayedSketchbookDrawing { get; private set; }

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
    }

    public static void CheckSketchbook()
    {
        HasCheckedSketchbook = true;
    }

    public static void PlaySketchbookDrawing()
    {
        HasPlayedSketchbookDrawing = true;
    }
}
