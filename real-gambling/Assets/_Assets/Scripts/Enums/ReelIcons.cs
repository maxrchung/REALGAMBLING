using GambleCore;
using GambleCore.Interface;
using GambleCore.Util;

public enum ReelIcons
{
    None,
    Snake,
    Bread,
    Fish,
    SixSeven,
    PepperMan,
    Mouth,
    CarKey
}

public class ReelIconAdapter : GambleEnumSymbol<ReelIcons>
{
    public ReelIconAdapter(ReelIcons value) : base(value)
    {
    }

    public static ReelIconAdapter Random(IRng rng) => new ReelIconAdapter(rng.NextEnum<ReelIcons>());
}