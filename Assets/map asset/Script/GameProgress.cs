public static class GameProgress
{
    public static bool HasCheckedSketchbook { get; private set; }

    public static void CheckSketchbook()
    {
        HasCheckedSketchbook = true;
    }
}
