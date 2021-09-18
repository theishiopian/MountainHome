public static class Extensions
{
    public static float Remap(this float input, float inputMin, float inputMax, float min, float max)
    {
        return min + (input - inputMin) * (max - min) / (inputMax - inputMin);
    }

    public static int Remap(this int input, int inputMin, int inputMax, int min, int max)
    {
        return min + (input - inputMin) * (max - min) / (inputMax - inputMin);
    }

    public static bool IsWithin(this float input, float min, float max)
    {
        return input >= min && input <= max;
    }
}