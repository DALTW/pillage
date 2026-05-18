public static class GameProgress
{
    public static bool HasCheckedSketchbook { get; private set; }
    public static bool HasPlayedSketchbookDrawing { get; private set; }

    public static void CheckSketchbook()
    {
        HasCheckedSketchbook = true;
    }

    public static void PlaySketchbookDrawing()
    {
        HasPlayedSketchbookDrawing = true;
    }
}
