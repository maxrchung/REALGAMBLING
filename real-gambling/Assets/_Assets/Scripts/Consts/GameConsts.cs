using UnityEngine;

public static class GameConsts
{
    public static readonly int SnakeValue = 1;
    public static readonly int BreadValue = 2;
    public static readonly int FishValue = 3;
    public static readonly int SixSevenValue = 4;

    public static int ReelIconsToValue(ReelIcons icon)
    {
        switch (icon)
        {
            case ReelIcons.Snake:
                return SnakeValue;
            case ReelIcons.Bread:
                return BreadValue;
            case ReelIcons.Fish:
                return FishValue;
            case ReelIcons.SixSeven:
                return SixSevenValue;
            default:
                Debug.LogError($"Icon {icon} passed in with no matching value");
                break;
        }

        return -1;
    }
}